using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalPhase;
using PIM_Mini_Tests_WPF.Common;

namespace PIM_Mini_Tests_WPF.FRAM
{
    public class FRAM : HardwareTest
    {
        private ushort portNumber;
        private int handle;
        private int bitrate;
        private int polarity;
        private ushort pageSize;
        private HardwareTest caller;

        public FRAM(string name = "FRAM") : base(name, new HardwareTest[] { new TestPortReady(), new TestOpenClose(), new TestWriteReadSingle(), new TestWriteReadOverflow() })
        {
            this.portNumber = Properties.Settings.Default.framPortNumber;
            this.bitrate = Properties.Settings.Default.framBitrate;
            this.polarity = Properties.Settings.Default.framPolarity;
            this.pageSize = Properties.Settings.Default.framPageSize;
        }

        public override void Test()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets up the test
        /// </summary>
        internal void SetUp(HardwareTest caller)
        {
            this.caller = caller;
            this.handle = AardvarkApi.aa_open(this.portNumber);
            if (!this.caller.AssertNotEqual(this.handle, (int)AardvarkStatus.AA_UNABLE_TO_OPEN, "The specified port is not connected to an Aardvark device or the port is already in use.")) return;
            if (!this.caller.AssertNotEqual(this.handle, (int)AardvarkStatus.AA_INCOMPATIBLE_DEVICE, "There is a version mismatch between the DLL and the firmware. The DLL is not of a sufficient version for interoperability with the firmware version or vice versa.")) return;

            var handleConfig = AardvarkApi.aa_configure(this.handle, AardvarkConfig.AA_CONFIG_SPI_I2C);
            if (!this.caller.AssertNotEqual(handleConfig, (int)AardvarkStatus.AA_CONFIG_ERROR, "The I2C or SPI subsystem is currently active and the new configuration requires the subsystem to be deactivated.")) return;
            if (!this.caller.AssertEqual(handleConfig, (int)AardvarkConfig.AA_CONFIG_SPI_I2C, "The Aardvark adapter could not be set so that I2C and SPI are enabled, and GPIO is disabled.")) return;

            var powerStatus = AardvarkApi.aa_target_power(this.handle, AardvarkApi.AA_TARGET_POWER_NONE);
            if (!this.caller.AssertNotEqual(powerStatus, (int)AardvarkStatus.AA_INCOMPATIBLE_DEVICE, "The hardware version is not compatible with this feature. Only hardware versions 2.00 or greater support switchable target power pins.")) return;
            if (!this.caller.AssertEqual(AardvarkApi.AA_TARGET_POWER_NONE, powerStatus, "The Aardvark adapter could not be set to disable the target power pins.")) return;

            var clockPhase = AardvarkApi.aa_spi_configure(this.handle, (AardvarkSpiPolarity)(polarity >> 1), (AardvarkSpiPhase)(polarity & 1), AardvarkSpiBitorder.AA_SPI_BITORDER_MSB);
            if (!this.caller.AssertEqual(clockPhase, (int)AardvarkStatus.AA_OK, "The SPI interface could not be configured.")) return;

            var bitrate = AardvarkApi.aa_spi_bitrate(this.handle, this.bitrate);
            if (!this.caller.AssertEqual(bitrate, this.bitrate, "The bitrate for the Aardvark adapter could not be set.")) return;

            var busTimeOut = AardvarkApi.aa_i2c_bus_timeout(this.handle, 10);
            if (!this.caller.AssertEqual(busTimeOut, 10, "The bus timeout for the Aardvark adapter could not be set.")) return;

            bool status = AardvarkExtensions.GetStatus(this.portNumber);
            if (!this.caller.AssertEqual(status, false, "The specified port is not available.")) return;
        }

        /// <summary>
        /// Cleans up after the test
        /// </summary>
        /// <param name=""></param>
        internal void TearDown()
        {
            if (!this.caller.AssertEqual(AardvarkApi.aa_close(this.handle), (int)AardvarkStatus.AA_UNABLE_TO_CLOSE, "The handle failed to close.")) return;
        }

        /// <summary>
        /// Writes to the FRAM.However, there is an artificial page size enforced, with size of 3KB.
        /// </summary>
        /// <param name="number">The number to be sent to the address</param>
        /// <param name="address">The first address to send data to</param>
        /// <param name="size">The number of addresses to write to</param>
        internal void Write(byte number, int address, int size)
        {
            var count = 0;
            address -= this.pageSize;
            while (count < size)
            {
                // Send write enable command
                int write = AardvarkApi.aa_spi_write(this.handle, 1, new byte[] { 0x06 }, 0, new byte[0]);
                if (!this.caller.AssertNotEqual(write, (int)AardvarkStatus.AA_SPI_WRITE_ERROR, "There was an error writing to the Aardvark adapter. This is most likely a result of a communication error. Make sure that out_num_bytes is less than 4 KiB.")) return;
                if (!this.caller.AssertEqual(write, 1, "The number of bytes written does not match the expected amount.")) return;

                count += this.pageSize;
                address += this.pageSize;

                // Assemble data
                List<byte> dataOut = new List<byte>
                {
                    [0] = 0x02,
                    [1] = (byte)((address >> 8) & 0xff),
                    [2] = (byte)((address >> 0) & 0xff)
                };
                for (int i = 0; i < this.pageSize; i++)
                {
                    dataOut.Add(number);
                }

                if (count > size)
                {
                    int index = (size % this.pageSize) + 3;
                    dataOut.RemoveRange(index, dataOut.Count - 1 - index);
                }

                // Write the transaction
                write = AardvarkApi.aa_spi_write(this.handle, (ushort)dataOut.Count, dataOut.ToArray(), 0, new byte[0]);
                if (!this.caller.AssertNotEqual(write, (int)AardvarkStatus.AA_SPI_WRITE_ERROR, "There was an error writing to the Aardvark adapter. This is most likely a result of a communication error. Make sure that out_num_bytes is less than 4 KiB.")) return;
                if (!this.caller.AssertEqual(write, 1, "The number of bytes written does not match the expected amount.")) return;

                uint sleep = AardvarkApi.aa_sleep_ms(10);
                if (!this.caller.AssertEqual(sleep, 10, "The amount of time the device slept for does not match the expected amount.")) return;
            }
        }

        /// <summary>
        /// Reads from the FRAM.However, there is an artificial page size enforced, with size of 3 KB.
        /// </summary>
        /// <param name="number">The number which should be held in the address</param>
        /// <param name="address">The first address to read data from</param>
        /// <param name="size">The number of addresses to read from</param>
        internal void Read(byte number, int address, int size)
        {
            var count = 0;
            address -= this.pageSize;
            while (count < size)
            {
                count += this.pageSize;
                address += this.pageSize;

                // NOTE: FIX BELOW HERE
                List<byte> dataOut = new List<byte>
                {
                    [0] = 0x03,
                    [1] = (byte)((address >> 8) & 0xff),
                    [2] = (byte)((address >> 0) & 0xff)
                };
                for (int i = 0; i < size; i++)
                {
                    dataOut.Add(0);
                }

                byte[] dataIn = new byte[size + 3];
                // Assemble read command and address

                if (count > size)
                {
                    int index = (size % this.pageSize) + 3;
                    dataIn = new byte[index];
                    dataOut.RemoveRange(index, dataOut.Count - 1 - index);
                }

                // Write length+3 bytes for data plus command and 2 address bytes
                count = AardvarkApi.aa_spi_write(this.handle, (ushort)dataOut.Count, dataOut.ToArray(), (ushort)dataIn.Length, dataIn.ToArray());
                if (!this.caller.AssertNotEqual(count, (int)AardvarkStatus.AA_SPI_WRITE_ERROR, "There was an error writing to the Aardvark adapter. This is most likely a result of a communication error. Make sure that out_num_bytes is less than 4 KiB.")) return;
                if (!this.caller.AssertGreaterEqual(count, 0, "No data was read from the slave.")) return;
                if (!this.caller.AssertEqual(count, size, "The amount of data read did not match the expected amount.")) return;

                byte[] expected_input = new byte[count];
                for (int i = 0; i < expected_input.Length; i++)
                {
                    expected_input[i] = number;
                }

                var dataInList = new List<byte>(dataIn);
                dataInList.RemoveRange(0, 3);

                if (!this.caller.AssertEqual(dataInList.ToArray(), expected_input, "The amount of data read from the FRAM did not match the expected amount", "The data read from the FRAM did not match the expected data.")) return;
            }
        }
    }
}

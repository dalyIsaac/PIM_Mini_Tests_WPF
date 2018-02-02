using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIM_Mini_Tests_WPF.Common;
using TotalPhase;

namespace PIM_Mini_Tests_WPF.FRAM
{
    public class TestWriteReadOverflow : HardwareTest
    {
        private int handle;
        private ushort portNumber;
        private int bitrate;
        private int polarity;
        private ushort pageSize;
        private int initialAddress;
        private int numAddress;

        /// <summary>
        /// Writes and then reads from every address, with overflow
        /// </summary>
        /// <param name="name"></param>
        public TestWriteReadOverflow(string name = "Writes and then reads from every address with overflow") : base(name)
        {
            this.portNumber = Properties.Settings.Default.framPortNumber;
            this.pageSize = Properties.Settings.Default.framPageSize;
            this.initialAddress = Properties.Settings.Default.framInitialAddress;
            this.numAddress = Properties.Settings.Default.framNumAddress;
            this.bitrate = Properties.Settings.Default.framBitrate;
            this.polarity = Properties.Settings.Default.framPolarity;
        }

        /// <summary>
        /// Sets up the test
        /// </summary>
        private void SetUp()
        {
            this.handle = AardvarkApi.aa_open(this.portNumber);
            if (!this.AssertNotEqual(this.handle, (int)AardvarkStatus.AA_UNABLE_TO_OPEN, "The specified port is not connected to an Aardvark device or the port is already in use.")) return;
            if (!this.AssertNotEqual(this.handle, (int)AardvarkStatus.AA_INCOMPATIBLE_DEVICE, "There is a version mismatch between the DLL and the firmware. The DLL is not of a sufficient version for interoperability with the firmware version or vice versa.")) return;

            var handleConfig = AardvarkApi.aa_configure(this.handle, AardvarkConfig.AA_CONFIG_SPI_I2C);
            if (!this.AssertNotEqual(handleConfig, (int)AardvarkStatus.AA_CONFIG_ERROR, "The I2C or SPI subsystem is currently active and the new configuration requires the subsystem to be deactivated.")) return;
            if (!this.AssertEqual(handleConfig, (int)AardvarkConfig.AA_CONFIG_SPI_I2C, "The Aardvark adapter could not be set so that I2C and SPI are enabled, and GPIO is disabled.")) return;

            var powerStatus = AardvarkApi.aa_target_power(this.handle, AardvarkApi.AA_TARGET_POWER_NONE);
            if (!this.AssertNotEqual(powerStatus, (int)AardvarkStatus.AA_INCOMPATIBLE_DEVICE, "The hardware version is not compatible with this feature. Only hardware versions 2.00 or greater support switchable target power pins.")) return;
            if (!this.AssertEqual(AardvarkApi.AA_TARGET_POWER_NONE, powerStatus, "The Aardvark adapter could not be set to disable the target power pins.")) return;

            var clockPhase = AardvarkApi.aa_spi_configure(this.handle, (AardvarkSpiPolarity)(polarity >> 1), (AardvarkSpiPhase)(polarity & 1), AardvarkSpiBitorder.AA_SPI_BITORDER_MSB);
            if (!this.AssertEqual(clockPhase, (int)AardvarkStatus.AA_OK, "The SPI interface could not be configured.")) return;

            var bitrate = AardvarkApi.aa_spi_bitrate(this.handle, this.bitrate);
            if (!this.AssertEqual(bitrate, this.bitrate, "The bitrate for the Aardvark adapter could not be set.")) return;

            var busTimeOut = AardvarkApi.aa_i2c_bus_timeout(this.handle, 10);
            if (!this.AssertEqual(busTimeOut, 10, "The bus timeout for the Aardvark adapter could not be set.")) return;

            bool status = AardvarkExtensions.GetStatus(this.portNumber);
            if (!this.AssertEqual(status, false, "The specified port is not available.")) return;
        }

        /// <summary>
        /// Cleans up after the test
        /// </summary>
        /// <param name=""></param>
        private void TearDown()
        {
            if (!this.AssertEqual(AardvarkApi.aa_close(this.handle), (int)AardvarkStatus.AA_UNABLE_TO_CLOSE, "The handle failed to close.")) return;
        }

        /// <summary>
        /// Writes to the FRAM.However, there is an artificial page size enforced, with size of 3KB.
        /// </summary>
        /// <param name="number">The number to be sent to the address</param>
        /// <param name="address">The first address to send data to</param>
        /// <param name="size">The number of addresses to write to</param>
        private void Write(byte number, int address, int size)
        {
            var count = 0;
            address -= this.pageSize;
            while (count < size)
            {
                // Send write enable command
                int write = AardvarkApi.aa_spi_write(this.handle, 1, new byte[] { 0x06 }, 0, new byte[0]);
                if (!this.AssertNotEqual(write, (int)AardvarkStatus.AA_SPI_WRITE_ERROR, "There was an error writing to the Aardvark adapter. This is most likely a result of a communication error. Make sure that out_num_bytes is less than 4 KiB.")) return;
                if (!this.AssertEqual(write, 1, "The number of bytes written does not match the expected amount.")) return;

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
                if (!this.AssertNotEqual(write, (int)AardvarkStatus.AA_SPI_WRITE_ERROR, "There was an error writing to the Aardvark adapter. This is most likely a result of a communication error. Make sure that out_num_bytes is less than 4 KiB.")) return;
                if (!this.AssertEqual(write, 1, "The number of bytes written does not match the expected amount.")) return;

                uint sleep = AardvarkApi.aa_sleep_ms(10);
                if (!this.AssertEqual(sleep, 10, "The amount of time the device slept for does not match the expected amount.")) return;
            }
        }

        /// <summary>
        /// Reads from the FRAM.However, there is an artificial page size enforced, with size of 3 KB.
        /// </summary>
        /// <param name="number">The number which should be held in the address</param>
        /// <param name="address">The first address to read data from</param>
        /// <param name="size">The number of addresses to read from</param>
        private void Read(byte number, int address, int size)
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
                if (!this.AssertNotEqual(count, (int)AardvarkStatus.AA_SPI_WRITE_ERROR, "There was an error writing to the Aardvark adapter. This is most likely a result of a communication error. Make sure that out_num_bytes is less than 4 KiB.")) return;
                if (!this.AssertGreaterEqual(count, 0, "No data was read from the slave.")) return;
                if (!this.AssertEqual(count, size, "The amount of data read did not match the expected amount.")) return;

                byte[] expected_input = new byte[count];
                for (int i = 0; i < expected_input.Length; i++)
                {
                    expected_input[i] = number;
                }

                var dataInList = new List<byte>(dataIn);
                dataInList.RemoveRange(0, 3);

                if (!this.AssertEqual(dataInList.ToArray(), expected_input, "The amount of data read from the FRAM did not match the expected amount", "The data read from the FRAM did not match the expected data.")) return;
            }
        }

        /// <summary>
        /// Writes and then reads from every address, in an interative, singular manner
        /// </summary>
        public override void Test()
        {
            for (byte i = 0; i < 17; i += 2)
            {
                this.Write(i, this.initialAddress, this.numAddress);
                this.Read(i, this.initialAddress, this.numAddress);
            }
            if (this.TestStatus != Status.Failed)
            {
                this.TestStatus = Status.Passed;
            }
        }
    }
}

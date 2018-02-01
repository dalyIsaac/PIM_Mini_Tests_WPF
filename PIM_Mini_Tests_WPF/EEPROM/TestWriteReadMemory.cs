﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalPhase;
using PIM_Mini_Tests_WPF.Common;

namespace PIM_Mini_Tests_WPF.EEPROM
{
    public class TestWriteReadMemory : HardwareTest
    {
        private int handle;
        private ushort pageSize;
        private int numPages;
        private ushort slaveAddress;
        private ushort port;
        private int bitrate;

        public TestWriteReadMemory() : base("Write and read memory")
        {
            this.pageSize = Properties.Settings.Default.eepromPageSize;
            this.numPages = Properties.Settings.Default.eepromNumPages;
            this.slaveAddress = Properties.Settings.Default.eepromSlaveAddress;
            this.port = Properties.Settings.Default.eepromPortNumber;
            this.bitrate = Properties.Settings.Default.eepromBitrate;
        }

        /// <summary>
        /// Writes to memory and verifies that the correct amount of data is written
        /// </summary>
        /// <param name="number">Value which is going to be written</param>
        private void WriteMemory(int number)
        {
            number %= 256;  // ensures that the number can be represented with 8-bits, or 1-byte
                            // creates a page with a space for the address
            var dataOut = new byte[1 + this.pageSize];
            for (int i = 0; i < dataOut.Length; i++)
            {
                dataOut[i] = (byte)number;
            }
            for (int i = 0; i < this.numPages; i++)
            {
                dataOut[0] = (byte)(i & 0xff);
                // I'm hoping that Aardvark will assemble the 7-bit slave address,
                // as that's what I think the documentation says
                var numBytesWritten = AardvarkApi.aa_i2c_write(this.handle, this.slaveAddress, AardvarkI2cFlags.AA_I2C_NO_FLAGS, this.pageSize, dataOut);
                if (!this.AssertGreater(numBytesWritten, 0, "No bytes were written to the device"))
                    return;
                if (!this.AssertEqual(numBytesWritten, this.pageSize + 1, "The number of bytes written to the device was different to the page size"))
                    return;
            }
        }

        /// <summary>
        /// Reads to memory and verifies that the data read is correct
        /// </summary>
        /// <param name="number">Expected number</param>
        private void ReadMemory(int number)
        {
            number %= 256;  // ensures that the number can fit inside the 16-bit page size
            var lengthError = "The number of bytes read is different to the expected value";
            for (int i = 0; i < this.numPages; i++)
            {
                // I'm hoping that Aardvark will assemble the 7-bit slave address,
                // as that's what I think the documentation says
                int bytesWritten = AardvarkApi.aa_i2c_write(this.handle, this.slaveAddress, AardvarkI2cFlags.AA_I2C_NO_FLAGS, this.pageSize, new byte[] { (byte)(i & 0xff) });
                if (!this.AssertEqual(bytesWritten, this.pageSize, "The amount of bytes written was different to the expected amount")) return;

                var dataIn = new byte[this.pageSize];
                int count = AardvarkApi.aa_i2c_read(this.handle, this.slaveAddress, AardvarkI2cFlags.AA_I2C_NO_FLAGS, this.pageSize, dataIn);

                var expected_input = new byte[this.pageSize];
                for (int j = 0; j < expected_input.Length; j++)
                {
                    expected_input[j] = (byte)number;
                }

                if (!this.AssertEqual(expected_input, dataIn, "The data read from the device is different from the expected data", lengthError))
                    return;
                if (!this.AssertEqual(count, expected_input.Length, lengthError))
                    return;
            }
        }

        public override void Test()
        {
            if (!this.AssertGreater(this.handle, 0, "The specified port did not open")) return;

            var handleConfig = AardvarkApi.aa_configure(this.handle, AardvarkConfig.AA_CONFIG_GPIO_I2C);
            if (!this.AssertEqual(handleConfig, (int)AardvarkConfig.AA_CONFIG_GPIO_I2C, "The Aardvark adapter could not be set so that the SPI pins are configured as GPIO pins, and enable I2C.")) return;

            var i2cPullupResistors = AardvarkApi.aa_i2c_pullup(this.handle, AardvarkApi.AA_I2C_PULLUP_BOTH);
            if (!this.AssertEqual(i2cPullupResistors, (int)AardvarkApi.AA_I2C_PULLUP_BOTH, "The Aardvark adapter could not be set so that the SCL/SDA pull-resistors are enabled.")) return;

            var powerStatus = AardvarkApi.aa_target_power(this.handle, AardvarkApi.AA_TARGET_POWER_BOTH);
            if (!this.AssertEqual(AardvarkApi.AA_TARGET_POWER_BOTH, powerStatus, "The Aardvark adapter could not be set to enable the target power pins")) return;

            var bitrate = AardvarkApi.aa_i2c_bitrate(this.handle, this.bitrate);
            if (!this.AssertEqual(bitrate, this.bitrate, "The bitrate for the Aardvark adapter could not be set")) return;

            var busTimeOut = AardvarkApi.aa_i2c_bus_timeout(this.handle, 10);
            if (!this.AssertEqual(busTimeOut, 10, "The bus timeout for the Aardvark adapter could not be set")) return;

            bool status = AardvarkExtensions.GetStatus(this.port);
            if (!this.AssertEqual(status, false, "The specified port is not available")) return;

            var numClosed = AardvarkApi.aa_close(this.handle);
            if (!this.AssertEqual(numClosed, 1, "An incorrect number of Aardvark adapters was closed")) return;

            for (int i = 0; i < 18; i += 2)
            {
                this.WriteMemory(i);
                this.ReadMemory(i);
            }
            if (this.AssertNotEqual(AardvarkApi.aa_close(this.handle), (int)AardvarkStatus.AA_UNABLE_TO_CLOSE, "The Aardvark handle could not close"))
            {
                this.TestStatus = Status.Passed;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalPhase;
using PIM_Mini_Tests_WPF.Common;

namespace PIM_Mini_Tests_WPF.EEPROM
{
    public class TestOpenClose : HardwareTest
    {
        private int handle;
        private ushort pageSize;
        private int numPages;
        private ushort slaveAddress;
        private ushort port;
        private int bitrate;

        public TestOpenClose() : base("Open/close")
        {
            this.pageSize = Properties.Settings.Default.eepromPageSize;
            this.numPages = Properties.Settings.Default.eepromNumPages;
            this.slaveAddress = Properties.Settings.Default.eepromSlaveAddress;
            this.port = Properties.Settings.Default.eepromPortNumber;
            this.bitrate = Properties.Settings.Default.eepromBitrate;
        }

        /// <summary>
        /// Tests that the port can be successfully opened and closed
        /// </summary>
        public override void Test()
        {
            this.handle = AardvarkApi.aa_open(this.port);
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

            this.TestStatus = Status.Passed;
        }
    }
}

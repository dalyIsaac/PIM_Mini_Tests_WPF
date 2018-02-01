using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalPhase;
using PIM_Mini_Tests_WPF.Common;

namespace PIM_Mini_Tests_WPF.FRAM
{
    public class TestOpenClose : HardwareTest
    {
        private int handle;
        private ushort port;
        private int bitrate;
        private int polarity;

        public TestOpenClose() : base("Open/close")
        {
            this.port = Properties.Settings.Default.framPortNumber;
            this.bitrate = Properties.Settings.Default.framBitrate;
            this.polarity = Properties.Settings.Default.framPolarity;
        }

        /// <summary>
        /// Tests that the port can be successfully opened and closed
        /// </summary>
        public override void Test()
        {
            this.handle = AardvarkApi.aa_open(this.port);
            if (!this.AssertGreater(this.handle, 0, "The specified port did not open")) return;

            var handleConfig = AardvarkApi.aa_configure(this.handle, AardvarkConfig.AA_CONFIG_SPI_I2C);
            if (!this.AssertEqual(handleConfig, (int)AardvarkConfig.AA_CONFIG_SPI_I2C, "The Aardvark adapter could not be set so that I2C and SPI are enabled, and GPIO is disabled")) return;

            var powerStatus = AardvarkApi.aa_target_power(this.handle, AardvarkApi.AA_TARGET_POWER_NONE);
            if (!this.AssertEqual(AardvarkApi.AA_TARGET_POWER_NONE, powerStatus, "The Aardvark adapter could not be set to disable the target power pins")) return;

            var clockPhase = AardvarkApi.aa_spi_configure(this.handle, (AardvarkSpiPolarity)(polarity >> 1), (AardvarkSpiPhase)(polarity & 1), AardvarkSpiBitorder.AA_SPI_BITORDER_MSB);
            if (!this.AssertEqual(clockPhase, (int)AardvarkStatus.AA_OK, "The SPI interface could not be configured")) return;

            var bitrate = AardvarkApi.aa_spi_bitrate(this.handle, this.bitrate);
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

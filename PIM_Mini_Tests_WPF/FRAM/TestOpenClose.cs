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
            if (!this.AssertNotEqual(this.handle, (int)AardvarkStatus.AA_UNABLE_TO_OPEN, "The specified port is not connected to an Aardvark device or the port is already in use.")) return;
            if (!this.AssertNotEqual(this.handle, (int)AardvarkStatus.AA_INCOMPATIBLE_DEVICE, "There is a version mismatch between the DLL and the firmware. The DLL is not of a sufficient version for interoperability with the firmware version or vice versa..")) return;

            var handleConfig = AardvarkApi.aa_configure(this.handle, AardvarkConfig.AA_CONFIG_SPI_I2C);
            if (!this.AssertNotEqual(handleConfig, (int)AardvarkStatus.AA_CONFIG_ERROR, "The I2C or SPI subsystem is currently active and the new configuration requires the subsystem to be deactivated..")) return;
            if (!this.AssertEqual(handleConfig, (int)AardvarkConfig.AA_CONFIG_SPI_I2C, "The Aardvark adapter could not be set so that I2C and SPI are enabled, and GPIO is disabled.")) return;

            var powerStatus = AardvarkApi.aa_target_power(this.handle, AardvarkApi.AA_TARGET_POWER_NONE);
            if (!this.AssertEqual(AardvarkApi.AA_TARGET_POWER_NONE, powerStatus, "The Aardvark adapter could not be set to disable the target power pins.")) return;
            if (!this.AssertNotEqual(powerStatus, (int)AardvarkStatus.AA_INCOMPATIBLE_DEVICE, "The hardware version is not compatible with this feature. Only hardware versions 2.00 or greater support switchable target power pins..")) return;

            var clockPhase = AardvarkApi.aa_spi_configure(this.handle, (AardvarkSpiPolarity)(polarity >> 1), (AardvarkSpiPhase)(polarity & 1), AardvarkSpiBitorder.AA_SPI_BITORDER_MSB);
            if (!this.AssertEqual(clockPhase, (int)AardvarkStatus.AA_OK, "The SPI interface could not be configured.")) return;

            var bitrate = AardvarkApi.aa_spi_bitrate(this.handle, this.bitrate);
            if (!this.AssertEqual(bitrate, this.bitrate, "The bitrate for the Aardvark adapter could not be set.")) return;

            bool status = AardvarkExtensions.GetStatus(this.port);
            if (!this.AssertEqual(status, false, "The specified port is not available.")) return;

            var numClosed = AardvarkApi.aa_close(this.handle);
            if (!this.AssertEqual(numClosed, 1, "An incorrect number of Aardvark adapters was closed.")) return;

            this.TestStatus = Status.Passed;
        }
    }
}

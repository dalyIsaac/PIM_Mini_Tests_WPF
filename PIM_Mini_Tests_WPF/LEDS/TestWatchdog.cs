using PIM_Mini_Tests_WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.LEDS
{
    class TestWatchdog : HardwareTest
    {
        public TestWatchdog() : base("Watchdog")
        {
        }

        public override void Test()
        {
            bool status = this.GetUserInput("Please turn on the device. Once the device is on, WAIT 10 SECONDS, and then check if the WATCHDOG LED is on.\nIs the WATCHDOG LED on?");
            if (!this.AssertEqual(status, true, "The WATCHDOG LED is not on, despite the device being turned on.")) return;
            this.TestStatus = Status.Passed;
        }
    }
}

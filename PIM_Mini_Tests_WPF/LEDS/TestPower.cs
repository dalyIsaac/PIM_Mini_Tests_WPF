using PIM_Mini_Tests_WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.LEDS
{
    class TestPower : HardwareTest
    {
        public TestPower() : base("Power")
        {
        }

        public override void Test()
        {
            bool status = this.GetUserInput("Please turn on the device. Once the device is on, check if the device's POWER LED is on.\nIs the POWER LED on?");
            if (!this.AssertEqual(status, true, "The POWER LED does not turn on when the device is on.")) return;
            this.TestStatus = Status.Passed;
        }
    }
}

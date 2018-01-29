using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.Startup
{
    public class TestWatchDog : HardwareTest
    {
        public TestWatchDog() : base("Watchdog") { }
        public override void Test()
        {
            bool status = this.GetUserInput("Can the device successfully perform a warm startup?");
            this.AssertEqual(status, true, "The device did not succesfully perform a warm startup");
        }
    }
}

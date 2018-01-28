using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF
{
    public class Startup : HardwareTest
    {
        public Startup() : base()
        {
            Name = "Startup Tests";
        }
        
        public void TestColdStart()
        {
            bool status = GetUserInput("Can the device successfully perform a cold startup?");
            this.AssertEqual(status, true, "The device did not successfully perform a cold startup");
        }

        public void TestWarmStart()
        {
            bool status = GetUserInput("Did the device successfully perform a warm startup?");
            this.AssertEqual(status, true, "The device did not successfully perform a warm startup");
        }

        public void TestWatchdog()
        {
            bool status = GetUserInput("Is the watchdog performing as expected?");
            this.AssertEqual(status, true, "The watchdog was not performing as expected");
        }
    }
}

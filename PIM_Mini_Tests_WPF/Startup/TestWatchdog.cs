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
            if ((bool)this.IsChecked)
            {
                bool status = this.GetUserInput("Is the watchdog performing as expected?");
                if (status)
                {
                    this.TestStatus = Status.Passed;
                }
                else
                {
                    this.TestStatus = Status.Failed;
                    this.ErrorMessage = "The watchdog was not performing as expected";
                }
            }
        }
    }
}

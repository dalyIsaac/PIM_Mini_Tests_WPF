using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIM_Mini_Tests_WPF.Common;

namespace PIM_Mini_Tests_WPF.Startup
{
    public class TestWarmStart : HardwareTest
    {
        public TestWarmStart() : base("Warm Start") { }
        public override void Test()
        {
            if ((bool)this.IsChecked)
            {
                bool status = this.GetUserInput("Can the device successfully perform a warm startup?");
                if (this.AssertEqual(status, true, "The device did not succesfully perform a warm startup"))
                {
                    this.TestStatus = Status.Passed;
                }
            }
        }
    }
}

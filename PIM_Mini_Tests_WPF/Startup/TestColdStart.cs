﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.Startup
{
    public class TestColdStart : HardwareTest
    {
        public TestColdStart() : base("Cold Start") { }
        public override void Test()
        {
            if ((bool)this.IsChecked)
            {
                bool status = this.GetUserInput("Can the device successfully perform a cold startup?");
                if (status)
                {
                    this.TestStatus = Status.Passed;
                }
                else
                {
                    this.TestStatus = Status.Failed;
                    this.ErrorMessage = "The device did not succesfully perform a cold startup";
                }
            }
        }
    }
}

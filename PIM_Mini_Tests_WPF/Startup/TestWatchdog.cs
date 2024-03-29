﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIM_Mini_Tests_WPF.Common;

namespace PIM_Mini_Tests_WPF.Startup
{
    public class TestWatchDog : HardwareTest
    {
        public TestWatchDog() : base("Watchdog") { }
        public override void Test()
        {
            bool status = this.GetUserInput("Is the watchdog performing as expected?");
            if (!this.AssertEqual(status, true, "The watchdog was not performing as expected.")) return;
            this.TestStatus = Status.Passed;
        }
    }
}

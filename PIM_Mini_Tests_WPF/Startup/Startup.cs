﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIM_Mini_Tests_WPF.Common;

namespace PIM_Mini_Tests_WPF.Startup
{
    public class Startup : HardwareTest
    {
        public Startup() : base("Startup", new HardwareTest[] { new TestColdStart(), new TestWarmStart(), new TestWatchDog() }) { }

        public override void Test()
        {
            throw new NotImplementedException();
        }
    }
}

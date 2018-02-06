﻿using PIM_Mini_Tests_WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.UserInputs
{
    class UserInputTwo : HardwareTest
    {
        public UserInputTwo() : base("User input two")
        {
        }

        public override void Test()
        {
            var parent = this._parent as UserInputs;
            var result = parent._Test(this.GetType());
            if (!this.AssertEqual(result.ToString(), DaemonResponse.Success.ToString(), result.ToString())) return;
            this.TestStatus = Status.Passed;
        }
    }
}

using PIM_Mini_Tests_WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.UserInputs
{
    class TestUserInputThree : HardwareTest
    {
        public TestUserInputThree() : base("User input three")
        {
        }

        public override void Test()
        {
            var parent = this._parent as UserInputs;
            var result = parent.Check(this.GetType(), "high");
            if (!this.AssertEqual(result, DaemonResponse.Success, $"Failed for pin level 'high' : {result.ToString()}")) return;
            result = parent.Check(this.GetType(), "low");
            if (!this.AssertEqual(result, DaemonResponse.Success, $"Failed for pin level 'low' : {result.ToString()}")) return;
            this.TestStatus = Status.Passed;
        }
    }
}

using PIM_Mini_Tests_WPF.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.UserInputs
{
    public class UserInputs : HardwareTest
    {
        public UserInputs() : base("User Inputs", new HardwareTest[]{ })
        {
        }

        internal DaemonResponse Check(Type caller, string level)
        {
            string testName = "";
            if (caller == typeof(UserInputOne))
            {
                testName = "UserInput One";
            }
            else if (caller == typeof(UserInputTwo))
            {
                testName = "UserInput Two";
            }
            else if (caller == typeof(UserInputThree))
            {
                testName = "UserInput Three";
            }

            var result = Controller.ExecuteTest(testName + " " + level);
            return result;
        }



        public override void Test()
        {
            throw new NotImplementedException();
        }
    }
}

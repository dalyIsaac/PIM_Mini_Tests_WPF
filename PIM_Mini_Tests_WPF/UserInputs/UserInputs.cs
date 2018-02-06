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

        internal DaemonResponse _Test(Type caller)
        {
            string testName = "";
            if (caller == typeof(UserInputOne))
            {
                testName = "UserInputOne";
            }
            else if (caller == typeof(UserInputTwo))
            {
                testName = "UserInputTwo";
            }
            else if (caller == typeof(UserInputThree))
            {
                testName = "UserInputThree";
            }
            var result = Controller.ExecuteTest(testName);
            return result;
        }

        public override void Test()
        {
            throw new NotImplementedException();
        }
    }
}

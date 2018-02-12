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
        public UserInputs() : base("User Inputs", new HardwareTest[] { new TestUserInputOne(), new TestUserInputTwo(), new TestUserInputThree() })
        {
        }

        internal DaemonResponse Check(Type caller, string level)
        {
            Controller.StartDaemon();
            string testName = "";
            if (caller == typeof(TestUserInputOne))
            {
                testName = "UserInput_One";
            }
            else if (caller == typeof(TestUserInputTwo))
            {
                testName = "UserInput_Two";
            }
            else if (caller == typeof(TestUserInputThree))
            {
                testName = "UserInput_Three";
            }

            var result = Controller.SendTcpMessage(testName + "_" + level);
            Controller.KillDaemonTcp();
            return result;
        }



        public override void Test()
        {
            throw new NotImplementedException();
        }
    }
}

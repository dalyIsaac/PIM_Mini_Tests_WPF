using PIM_Mini_Tests_WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.COMMS.CCP
{
    class CCP : HardwareTest
    {
        public CCP() : base("CCP", new HardwareTest[] { new TestTTL(), new TestRS232(), new TestRS485() })
        {
        }

        internal void Check(HardwareTest caller)
        {
            var status = caller.GetUserInput($"Is the PIM Mini ready for the CCP {caller.Name} test?");
            if (!caller.AssertEqual(true, status, "The user indicated that the test is not ready.")) return;

            string message = $"{this.Name}_{caller.Name}";
            var result = Controller.SendTcpMessage(message);
            if (!caller.AssertEqual(result.ToString(), DaemonResponse.Success.ToString(), "")) return;
            caller.TestStatus = Status.Passed;
        }

        public override void Test()
        {
            throw new NotImplementedException();
        }
    }
}

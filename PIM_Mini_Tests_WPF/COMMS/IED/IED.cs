using PIM_Mini_Tests_WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.COMMS.IED
{
    class IED : HardwareTest
    {
        public IED() : base("IED", new HardwareTest[] {
            new TestRS232PoliteWrite(),
            new TestRS232PoliteReceive(),
            new TestRS232RudeWrite(),
            new TestRS232RudeReceive(),
            new TestRS485() })
        {
        }

        internal void Check(HardwareTest caller, string name)
        {
            var status = caller.GetUserInput($"Is the PIM Mini ready for the IED {caller.Name} test?");
            if (!caller.AssertEqual(true, status, "The user indicated that the test is not ready.")) return;

            string message = $"comms_{this.Name}_{name}";
            var result = Controller.SendTcpMessage(message);
            if (!caller.AssertEqual(result, DaemonResponse.Success, "")) return;
            caller.TestStatus = Status.Passed;
        }

        public override void Test()
        {
            throw new NotImplementedException();
        }
    }
}

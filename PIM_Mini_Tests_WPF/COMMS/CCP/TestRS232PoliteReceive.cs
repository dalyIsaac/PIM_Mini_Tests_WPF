using PIM_Mini_Tests_WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.COMMS.CCP
{
    public class TestRS232PoliteReceive : HardwareTest
    {
        public TestRS232PoliteReceive() : base("RS232 Receive with CTS/RTS")
        {
        }

        public override void Test()
        {
            var parent = this._parent as CCP;
            parent.Check(this, "RS232_politeReceive");
        }
    }
}

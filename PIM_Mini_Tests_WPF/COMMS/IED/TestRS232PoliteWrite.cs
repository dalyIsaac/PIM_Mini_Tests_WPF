using PIM_Mini_Tests_WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.COMMS.IED
{
    public class TestRS232PoliteWrite : HardwareTest
    {
        public TestRS232PoliteWrite() : base("RS232 Write with CTS/RTS")
        {
        }

        public override void Test()
        {
            var parent = this._parent as IED;
            parent.Check(this, "RS232_politeWrite");
        }
    }
}

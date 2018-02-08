using PIM_Mini_Tests_WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.COMMS.CCP
{
    public class TestRS232 : HardwareTest
    {
        public TestRS232() : base("RS232")
        {
        }

        public override void Test()
        {
            var parent = this._parent as CCP;
            parent.Check(this);
        }
    }
}

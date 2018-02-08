using PIM_Mini_Tests_WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.COMMS.IED
{
    public class TestTTL : HardwareTest
    {
        public TestTTL() : base("TTL")
        {
        }

        public override void Test()
        {
            var parent = this._parent as IED;
            parent.Check(this);
        }
    }
}

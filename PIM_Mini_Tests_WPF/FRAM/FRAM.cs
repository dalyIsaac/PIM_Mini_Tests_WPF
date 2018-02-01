using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalPhase;
using PIM_Mini_Tests_WPF.Common;

namespace PIM_Mini_Tests_WPF.FRAM
{
    public class FRAM : HardwareTest
    {
        public FRAM(string name = "FRAM") : base(name, new HardwareTest[] { new TestPortReady(), new TestOpenClose(), new TestWriteReadSingle(), new TestWriteReadOverflow() }) { }

        public override void Test()
        {
            throw new NotImplementedException();
        }
    }
}

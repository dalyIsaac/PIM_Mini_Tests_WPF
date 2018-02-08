using PIM_Mini_Tests_WPF.Common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.COMMS
{
    public class COMMS : HardwareTest
    {
        public COMMS() : base("Comms", new HardwareTest[] { new CCP.CCP() })
        {
        }

        public override void Test()
        {
            throw new NotImplementedException();
        }
    }
}

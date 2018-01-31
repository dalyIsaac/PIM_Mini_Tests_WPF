using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.EEPROM
{
    public class EEPROM : HardwareTest
    {
        public EEPROM(string name = "EEPROM") : base(name, new HardwareTest[] { new TestPortReady(), new TestOpenClose(), new TestWriteReadMemory() }) { }

        public override void Test()
        {
            throw new NotImplementedException();
        }
    }
}

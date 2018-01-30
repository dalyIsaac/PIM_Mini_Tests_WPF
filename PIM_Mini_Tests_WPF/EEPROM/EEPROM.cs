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
        public EEPROM() : base("EEPROM", new HardwareTest[] { new TestWriteReadMemory() })
        {
        }

        public override void Test()
        {
            if (this.IsChecked != false)
            {
                foreach (var child in this.Children)
                {
                    child.Test();
                }
            }
        }
    }
}

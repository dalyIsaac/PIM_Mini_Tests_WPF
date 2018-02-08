using PIM_Mini_Tests_WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.LEDS
{
    public class TestIedDataRx : HardwareTest
    {
        public TestIedDataRx() : base("IED Data Rx")
        {
        }

        /// <summary>
        /// Turns the LED on, asks for user input, and then turns it off
        /// </summary>
        public override void Test()
        {
            var parent = this._parent as LEDS;
            parent.Check(this);
        }
    }
}

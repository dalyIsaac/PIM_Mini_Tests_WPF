using PIM_Mini_Tests_WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.RTC
{
    /// <summary>
    /// Checks that the seconds increment correctly
    /// </summary>
    public class TestSetSecondIncr : HardwareTest
    {
        public TestSetSecondIncr() : base("Checks that the seconds increment correctly")
        {
        }

        public override void Test()
        {
            ClockData data = new ClockData()
            {
                Year = 2017,
                Month = 2,
                DayOfMonth = 28,
                DayOfWeek = 2,
                Hours = 23,
                Minutes = 59,
                Seconds = 55
            };
            RTC parent = this._parent as RTC;
            parent.Check(this, data);
        }
    }
}

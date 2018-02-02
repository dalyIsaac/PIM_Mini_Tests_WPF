using PIM_Mini_Tests_WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.RTC
{
    /// <summary>
    /// Checks that the years increment correctly
    /// </summary>
    public class TestSetYearIncr : HardwareTest
    {
        public TestSetYearIncr() : base("Checks that the years increment correctly")
        {
        }

        public override void Test()
        {
            ClockData data = new ClockData()
            {
                Year = 2017,
                Month = 12,
                DayOfMonth = 31,
                DayOfWeek  = 7,
                Hours = 23,
                Minutes = 59,
                Seconds = 55
            };
            RTC parent = this._parent as RTC;
            parent.Check(this, data);
        }
    }
}

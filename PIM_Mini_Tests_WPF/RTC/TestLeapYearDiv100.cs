using PIM_Mini_Tests_WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.RTC
{
    /// <summary>
    /// Checks that the days turn over the month correctly for an invalid leap year (div 100)
    /// </summary>
    public class TestLeapYearDiv100 : HardwareTest
    {
        public TestLeapYearDiv100() : base("Checks that the days turn over the month correctly for an invalid leap year (div 100)")
        {
        }

        public override void Test()
        {
            ClockData data = new ClockData()
            {
                Year = 2100,
                Month = 2,
                DayOfMonth = 28,
                DayOfWeek = 7,
                Hours = 23,
                Minutes = 59,
                Seconds = 55
            };
            RTC parent = this._parent as RTC;
            parent.Check(this, data);
        }
    }
}

using PIM_Mini_Tests_WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.RTC
{
    /// <summary>
    /// Checks that the days turn over the month correctly for a valid leap year (div 400)
    /// </summary>
    public class TestLeapYearDiv400 : HardwareTest
    {
        public TestLeapYearDiv400() : base("Checks that the days turn over the month correctly for a valid leap year (div 400)")
        {
        }

        public override void Test()
        {
            ClockData data = new ClockData()
            {
                Year = 2000,
                Month = 2,
                DayOfMonth = 28,
                DayOfWeek = 1,
                Hours = 23,
                Minutes = 59,
                Seconds = 55
            };
            RTC parent = this._parent as RTC;
            parent.Check(this, data);
        }
    }
}

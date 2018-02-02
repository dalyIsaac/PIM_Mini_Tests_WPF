using PIM_Mini_Tests_WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.RTC
{
    /// <summary>
    /// Checks that the days turn over a 31-day month correctly
    /// </summary>
    public class TestSetDay30 : HardwareTest
    {
        public TestSetDay30() : base("Checks that the days turn over a 31-day month correctly")
        {
        }

        public override void Test()
        {
            ClockData data = new ClockData()
            {
                Year = 2017,
                Month = 9,
                DayOfMonth = 30,
                DayOfWeek = 6,
                Hours = 23,
                Minutes = 59,
                Seconds = 55
            };
            RTC parent = this._parent as RTC;
            parent.Check(data);
        }
    }
}

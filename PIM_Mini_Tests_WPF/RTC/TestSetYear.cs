using PIM_Mini_Tests_WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.RTC
{
    /// <summary>
    /// Checks that the years turn over a century correctly
    /// </summary>
    public class TestSetYear : HardwareTest
    {
        public TestSetYear() : base("Checks that the years turn over a century correctly")
        {
        }

        public override void Test()
        {
            ClockData data = new ClockData()
            {
                Year = 2099,
                Month = 12,
                DayOfMonth = 31,
                DayOfWeek = 4,
                Hours = 23,
                Minutes = 59,
                Seconds = 55
            };
            RTC parent = this._parent as RTC;
            parent.Check(this, data);
        }
    }
}

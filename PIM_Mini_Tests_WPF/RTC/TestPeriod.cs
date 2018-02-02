using PIM_Mini_Tests_WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.RTC
{
    /// <summary>
    /// Checks that the time passes correctly, over a user-set duration
    /// </summary>
    public class TestPeriod : HardwareTest
    {
        public TestPeriod() : base("Checks that the time passes correctly, over a user-set duration")
        {
        }

        public override void Test()
        {
            ClockData data = new ClockData()
            {
                Year = 2000,
                Month = 1,
                DayOfMonth = 1,
                DayOfWeek = 1,
                Hours = 0,
                Minutes = 0,
                Seconds = 0
            };
            RTC parent = this._parent as RTC;
            parent.Check(data);
        }
    }
}

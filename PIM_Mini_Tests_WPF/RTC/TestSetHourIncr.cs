﻿using PIM_Mini_Tests_WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.RTC
{
    /// <summary>
    /// Checks that the hours increment correctly
    /// </summary>
    public class TestSetHourIncr : HardwareTest
    {
        public TestSetHourIncr() : base("Checks that the hours increment correctly")
        {
        }

        public override void Test()
        {
            ClockData data = new ClockData()
            {
                Year = 2017,
                Month = 12,
                DayOfMonth = 8,
                DayOfWeek = 5,
                Hours = 14,
                Minutes = 59,
                Seconds = 55
            };
            RTC parent = this._parent as RTC;
            parent.Check(this, data);
        }
    }
}

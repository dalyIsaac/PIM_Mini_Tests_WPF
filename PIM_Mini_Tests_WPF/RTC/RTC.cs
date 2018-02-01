using PIM_Mini_Tests_WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalPhase;

namespace PIM_Mini_Tests_WPF.RTC
{
    public class RTC : HardwareTest
    {
        public RTC() : base(name: "RTC") { }
        
        public override void Test()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets up the tests inside the FRAM namespace
        /// </summary>
        /// <returns></returns>
        internal static int SetUp()
        {
            int handle = AardvarkApi.aa_open(Properties.Settings.Default.rtcPortNumber);
            if (handle < 0) return handle;
            int config = AardvarkApi.aa_configure(handle, AardvarkConfig.AA_CONFIG_GPIO_I2C);
            if (config < 0) return config;
            int pullup = AardvarkApi.aa_i2c_pullup(handle, AardvarkApi.AA_I2C_PULLUP_BOTH);
            if (pullup < 0) return config;
            int targetPower = AardvarkApi.aa_target_power(handle, AardvarkApi.AA_TARGET_POWER_BOTH);
            AardvarkApi.aa_i2c_bitrate(handle, 400); //  bitrate = 400
            AardvarkApi.aa_i2c_bus_timeout(handle, 10);  // timeout = 10ms
            return handle;
        }
    }
}

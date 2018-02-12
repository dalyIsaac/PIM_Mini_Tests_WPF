using PIM_Mini_Tests_WPF.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF.LEDS
{
    public class LEDS : HardwareTest
    {
        public LEDS() : base("LEDs", new HardwareTest[] 
        {
            new TestPower(),
            new TestWatchdog(),
            new TestCcpOk(),
            new TestIedOk(),
            new TestFault(),
            new TestCcpDataTx(),
            new TestCcpDataRx(),
            new TestIedDataTx(),
            new TestIedDataRx()
        })
        {
        }


        /// <summary>
        /// Sets LEDs
        /// </summary>
        /// <param name="caller">The caller instance</param>
        /// <param name="level">true is on, false is off</param>
        /// <returns></returns>
        internal void Check(HardwareTest caller)
        {
            var result = this.SetLED(caller, true);
            if (result == DaemonResponse.Success)
            {
                bool userInput = caller.GetUserInput($"Is the {caller.Name} LED on?");
                caller.AssertEqual(userInput, true, $"The {caller.Name} LED could not be turned on.");
            }
            else
            {
                caller.AssertEqual(result, DaemonResponse.Success, $"Could not set the {caller.Name} LED"); // this fails the test
            }
            var resetResult = this.SetLED(caller, false);
            if (!caller.AssertEqual(resetResult.ToString(), DaemonResponse.Success.ToString(), $"The {caller.Name} LED could not be turned off")) return;
            caller.TestStatus = Status.Passed;
        }

        internal DaemonResponse SetLED(HardwareTest caller, bool level)
        {
            string ledLevel = level == true ? "on" : "off";
            return Controller.SendTcpMessage("LED_" + caller.Name.Remove(' ') + "_" + ledLevel);
        }

        public override void Test()
        {
            throw new NotImplementedException();
        }
    }
    internal enum TargetLEDs
    {
        CCP_OK,
        IED_OK,
        FAULT,
        CCP_DATA_TX,
        CCP_DATA_RX,
        IED_DATA_TX,
        IED_DATA_RX
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalPhase;
using PIM_Mini_Tests_WPF.Common;

namespace PIM_Mini_Tests_WPF.Common
{
    public class TestPortReady : HardwareTest
    {
        private ushort port;

        public TestPortReady() : base("Port ready")
        {
            this.port = Properties.Settings.Default.framPortNumber;
        }

        /// <summary>
        /// Tests that the port is ready
        /// </summary>
        public override void Test()
        {
            ushort[] ports = new ushort[16];
            uint[] uniqueIds = new uint[16];
            int numElem = 16; // number of elements (port numbers and unique IDs) to return

            // Find all the attached devices
            int count = AardvarkApi.aa_find_devices_ext(numElem, ports,
                                                        numElem, uniqueIds);
            if (!this.AssertGreater(count, 0, "No Aardvark devices were found.")) return;

            List<ushort> portNumbers = new List<ushort>(ports);
            if (!this.AssertEqual(portNumbers.Contains(this.port), true, "The specified port was not detected.")) return;
            bool status = AardvarkExtensions.GetStatus(this.port);
            if (!this.AssertEqual(status, false, "The specified port is not available.")) return;
            this.TestStatus = Status.Passed;
        }
    }
}

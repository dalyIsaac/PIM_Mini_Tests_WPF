using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalPhase;

namespace PIM_Mini_Tests_WPF.EEPROM
{
    public class TestPortReady : HardwareTest
    {
        private ushort pageSize;
        private int numPages;
        private ushort slaveAddress;
        private ushort port;

        public TestPortReady() : base("Port ready")
        {
            this.pageSize = Properties.Settings.Default.eepromPageSize;
            this.numPages = Properties.Settings.Default.eepromNumPages;
            this.slaveAddress = Properties.Settings.Default.eepromSlaveAddress;
            this.port = Properties.Settings.Default.eepromPortNumber;
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
            if (!this.AssertGreater(count, 0, "No Aardvark devices were found")) return;

            List<ushort> portNumbers = new List<ushort>(ports);
            if (!this.AssertEqual(portNumbers.Contains(this.port), true, "The specified port was not detected")) return;
            bool status = AardvarkExtensions.GetStatus(this.port);
            if (!this.AssertEqual(status, false, "The specified port is not available")) return;
            this.TestStatus = Status.Passed;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PIM_Mini_Tests_WPF.Common;
using TotalPhase;

namespace PIM_Mini_Tests_WPF.FRAM
{
    public class TestWriteReadOverflow : HardwareTest
    {
        private int initialAddress;
        private int numAddress;

        /// <summary>
        /// Writes and then reads from every address, with overflow
        /// </summary>
        /// <param name="name"></param>
        public TestWriteReadOverflow(string name = "Writes and then reads from every address with overflow") : base(name)
        {
            this.initialAddress = Properties.Settings.Default.framInitialAddress;
            this.numAddress = Properties.Settings.Default.framNumAddress;
        }

        /// <summary>
        /// Writes and then reads from every address, in an interative, singular manner
        /// </summary>
        public override void Test()
        {
            FRAM parent = this._parent as FRAM;
            parent.SetUp(this);

            for (byte i = 0; i < 17; i += 2)
            {
                parent.Write(i, this.initialAddress, this.numAddress);
                parent.Read(i, this.initialAddress, this.numAddress);
            }
            parent.TearDown();
            if (this.TestStatus != Status.Failed)
            {
                this.TestStatus = Status.Passed;
            }
        }
    }
}

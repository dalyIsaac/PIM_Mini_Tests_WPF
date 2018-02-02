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
    /// <summary>
    /// Writes and then reads from every address, in a iterative, singular manner
    /// </summary>
    public class TestWriteReadSingle : HardwareTest
    {
        private int initialAddress;

        /// <summary>
        /// Writes and then reads from every address, in a iterative, singular manner
        /// </summary>
        /// <param name="name"></param>
        public TestWriteReadSingle(string name = "Writes and then reads from every address, iteratively") : base(name)
        {
            this.initialAddress = Properties.Settings.Default.framInitialAddress;
        }

        /// <summary>
        /// Writes and then reads from every address, in an interative, singular manner
        /// </summary>
        public override void Test()
        {
            FRAM parent = this._parent as FRAM;
            parent.SetUp(this);

            for (byte i = 0; i < 18; i += 2)
            {
                parent.Write(i, this.initialAddress, 1);
                parent.Read(i, this.initialAddress, 1);
            }
            parent.TearDown();
            if (this.TestStatus != Status.Failed)
            {
                this.TestStatus = Status.Passed;
            }
        }
    }
}

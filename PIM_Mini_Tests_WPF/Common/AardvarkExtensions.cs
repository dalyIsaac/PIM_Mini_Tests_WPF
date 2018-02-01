using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TotalPhase;

namespace PIM_Mini_Tests_WPF.Common
{
    public static class AardvarkExtensions
    {
        /// <summary>
        /// Returns the status of the port
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool GetStatus(ushort port)
        {
            if ((port & AardvarkApi.AA_PORT_NOT_FREE) != 0)
            {
                return true;
            }
            return false;
        }
    }
}

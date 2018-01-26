using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIM_Mini_Tests_WPF
{
    public class AssertionDefinitions
    {
        /// <summary>
        /// Asserts that two strings are equal. Throws an exception if they are not.
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AssertEqual(string val1, string val2, string message = "")
        {
            if (val1 != val2)
            {
                throw new Exception($"{message}\n{val1} is not equal to {val2}");
            }
            return val1 == val2;
        }

        /// <summary>
        /// Asserts that two bools are equal. Throws an exception if they are not.
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool AssertEqual(bool val1, bool val2, string message = "")
        {
            if (val1 != val2)
            {
                throw new Exception($"{message}\n{val1} is not equal to {val2}");
            }
            return val1 == val2;
        }
    }
}

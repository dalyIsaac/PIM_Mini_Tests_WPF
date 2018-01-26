using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PIM_Mini_Tests_WPF
{
    public class HardwareTest : AssertionDefinitions
    {
        public ObservableCollection<MethodInfo> TestMethods { get; }
        public string Caption { get; set; }

        public HardwareTest()
        {
            var testMethods = this.GetType().GetMethods().ToList()
                 .Where(x => x.Name.StartsWith("Test"));
            TestMethods = new ObservableCollection<MethodInfo>(testMethods);
        }

        /// <summary>
        /// Runs all the methods in this class which begin with "test"
        /// </summary>
        /// <returns>Test failure, and the error message if it did fail</returns>
        public IEnumerable<(bool, string)> RunTests(List<MethodInfo> methods)
        {
            foreach (var method in methods)
            {
                Exception ex = null;
                try
                {
                    method.Invoke(this, null);
                }
                catch (Exception localException)
                {
                    ex = localException;
                }

                if (ex != null)
                {
                    yield return (false, ex.Message);
                }
                else
                {
                    yield return (true, "");
                }
            }
        }

        /// <summary>
        /// Gets yes/no user input and returns it
        /// </summary>
        /// <param name="message">Message to show the user</param>
        /// <param name="caption">Caption for the window</param>
        /// <returns>User selection</returns>
        public bool GetUserInput(string message)
        {
            MessageBoxResult response = MessageBox.Show(message, Caption, MessageBoxButton.YesNo);
            if (response == MessageBoxResult.Yes)
                return true;
            return false;
        }
    }
}

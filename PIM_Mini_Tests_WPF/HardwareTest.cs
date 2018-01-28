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
        public ObservableCollection<Method> TestMethods { get; }
        public string Name { get; set; }

        public HardwareTest()
        {
            var testMethods = this.GetType().GetMethods().ToList()
                 .Where(x => x.Name.StartsWith("Test"));
            TestMethods = new ObservableCollection<Method>(testMethods.Select(x => new Method(x)));
        }

        /// <summary>
        /// Runs all the methods in this class which begin with "test"
        /// </summary>
        /// <returns>Test failure, and the error message if it did fail</returns>
        public IEnumerable<(bool, string)> RunTests(List<Method> methods)
        {
            foreach (var method in methods)
            {
                Exception ex = null;
                try
                {
                    method.MethodData.Invoke(this, null);
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
            MessageBoxResult response = MessageBox.Show(message, Name, MessageBoxButton.YesNo);
            if (response == MessageBoxResult.Yes)
                return true;
            return false;
        }
    }
}

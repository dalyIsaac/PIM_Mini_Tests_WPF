using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PIM_Mini_Tests_WPF
{
    public abstract class HardwareTest : INotifyPropertyChanged
    {
        private bool? _isChecked = false;
        private Status _testStatus = Status.NotRun;
        private HardwareTest _parent;
        private Visibility _outputVisibility = Visibility.Collapsed;
        private string _errorMessage;

        public ObservableCollection<HardwareTest> Children { get; private set; }
        public bool IsInitiallySelected { get; private set; }
        public string Name { get; set; }

        public HardwareTest(string name, HardwareTest[] children = null)
        {
            this.Name = name;
            this.Children = children == null ? new ObservableCollection<HardwareTest>() : new ObservableCollection<HardwareTest>(children);
            this.Initialize();
        }

        private void Initialize()
        {
            foreach (var child in this.Children)
            {
                child._parent = this;
                child.Initialize();
            }
        }

        public bool? IsChecked
        {
            get { return this._isChecked; }
            set { this.SetIsChecked(value, true, true); }
        }

        private void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == this._isChecked)
                return;

            _isChecked = value;

            if (updateChildren && this._isChecked.HasValue)
            {
                foreach (var child in this.Children)
                {
                    child.SetIsChecked(_isChecked, true, false);
                }
            }

            if (updateParent && this._parent != null)
            {
                this._parent.VerifyCheckState();
            }

            this.OnPropertyChanged("IsChecked");
        }

        private void VerifyCheckState()
        {
            bool? state = null;
            for (int i = 0; i < this.Children.Count; ++i)
            {
                bool? current = this.Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }
            this.SetIsChecked(state, false, true);
        }

        public Status TestStatus
        {
            get { return this._testStatus; }
            set { this.SetTestStatus(value); }
        }

        private void SetTestStatus(Status testStatus)
        {
            if (this._parent != null)
            {
                if (this._parent.TestStatus == Status.Mixed || testStatus == Status.Mixed)
                    this._parent.TestStatus = Status.Mixed;
                else if (this._parent.TestStatus == Status.NotRun || testStatus == Status.NotRun)
                    this._parent.TestStatus = testStatus != Status.NotRun ? testStatus : this._parent.TestStatus;
                else if (this._parent.TestStatus == testStatus) { }
                else
                    this._parent.TestStatus = Status.Mixed;
            }
            this._testStatus = testStatus;
            this.OnPropertyChanged("TestStatus");
        }

        public string ErrorMessage
        {
            get { return this._errorMessage; }
            set { this.SetErrorMessage(value); }
        }

        private void SetErrorMessage(string message)
        {
            this._errorMessage = message;
            this.OnPropertyChanged("ErrorMessage");
            if (this._parent != null)
            {
                if (this._parent.ErrorMessage == null || this._parent.ErrorMessage == "")
                {
                    this._parent.ErrorMessage = $"{this.Name}: {message}";
                }
                else
                {
                    this._parent.ErrorMessage += $"\n{this.Name}: {message}";
                }
            }
        }

        /// <summary>
        /// Sets the test status of the test and all it's children to NotRun
        /// </summary>
        public void ResetTestData()
        {
            this._testStatus = Status.NotRun;
            this.OnPropertyChanged("TestStatus");
            this._errorMessage = "";
            this.OnPropertyChanged("ErrorMessage");
            foreach (var child in this.Children)
            {
                child.ResetTestData();
            }
        }

        public Visibility OutputVisibility
        {
            get { return this._outputVisibility; }
            set { this._outputVisibility = value; this.OnPropertyChanged("OutputVisibility");}
        }

        /// <summary>
        /// Executes a test. MUST ASSIGN A VALUE TO this.TestStatus
        /// </summary>
        /// <returns></returns>
        public abstract void Test();

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
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

        #region Assertions

        /// <summary>
        /// Asserts that two strings are equal.
        /// If the test passes, the TestStatus should be set by the caller.
        /// If the test fails, TestStatus is set by this method.
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="message">Error message for the user</param>
        /// <returns>Boolean indicating whether the test was successful</returns>
        public bool AssertEqual(string val1, string val2, string message)
        {
            if (val1 != val2)
            {
                this.TestStatus = Status.Failed;
                this.ErrorMessage = message;
            }
            return val1 == val2;
        }

        /// <summary>
        /// Asserts that two bools are equal.
        /// If the test passes, the TestStatus should be set by the caller.
        /// If the test fails, TestStatus is set by this method.
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="message">Error message for the user</param>
        /// <returns>Boolean indicating whether the test was successful</returns>
        public bool AssertEqual(bool val1, bool val2, string message)
        {
            if (val1 != val2)
            {
                this.TestStatus = Status.Failed;
                this.ErrorMessage = message;
            }
            return val1 == val2;
        }

        /// <summary>
        /// Asserts that two integers are equal.
        /// If the test passes, the TestStatus should be set by the caller.
        /// If the test fails, TestStatus is set by this method.
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="message">Error message for the user</param>
        /// <returns>Boolean indicating whether the test was successful</returns>
        public bool AssertEqual(int val1, int val2, string message)
        {
            if (val1 != val2)
            {
                this.TestStatus = Status.Failed;
                this.ErrorMessage = message;
            }
            return val1 == val2;
        }


        /// <summary>
        /// Asserts that two byte arrays are equal in length and have identical contents.
        /// If the test passes, the TestStatus should be set by the caller.
        /// If the test fails, TestStatus is set by this method.
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="differentContents">Error message if the contents are different</param>
        /// <param name="differentLength">Error message if the length is different</param>
        /// <returns></returns>
        public bool AssertEqual(byte[] val1, byte[] val2, string differentContents, string differentLength)
        {
            if (val1.Length != val2.Length)
            {
                this.TestStatus = Status.Failed;
                this.ErrorMessage = differentLength;
                return false;
            }
            for (int i = 0; i < val1.Length; i++)
            {
                if (val1[i] != val2[i])
                {
                    this.TestStatus = Status.Failed;
                    this.ErrorMessage = differentContents;
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Asserts that val1 is greater than or equal to val2. 
        /// If the test passes, the TestStatus should be set by the caller.
        /// If the test fails, TestStatus is set by this method.
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="message">Error message for the user</param>
        /// <returns>Boolean indicating whether the test was successful</returns>
        public bool AssertGreaterEqual(int val1, int val2, string message)
        {
            if (val1 < val2)
            {
                this.TestStatus = Status.Failed;
                this.ErrorMessage = message;
            }
            return val1 >= val2;
        }

        /// <summary>
        /// Asserts that val1 is greater than val2. 
        /// If the test passes, the TestStatus should be set by the caller.
        /// If the test fails, TestStatus is set by this method.
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="message">Error message for the user</param>
        /// <returns>Boolean indicating whether the test was successful</returns>
        public bool AssertGreater(int val1, int val2, string message)
        {
            if (val1 <= val2)
            {
                this.TestStatus = Status.Failed;
                this.ErrorMessage = message;
            }
            return val1 > val2;
        }


        /// <summary>
        /// Asserts that val1 is not equal to val2. 
        /// If the test passes, the TestStatus should be set by the caller.
        /// If the test fails, TestStatus is set by this method.
        /// </summary>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="message">Error message for the user</param>
        /// <returns>Boolean indicating whether the test was successful</returns>
        public bool AssertNotEqual(int val1, int val2, string message)
        {
            if (val1 == val2)
            {
                this.TestStatus = Status.Failed;
                this.ErrorMessage = message;
            }
            return val1 != val2;
        }
        #endregion
    }
}

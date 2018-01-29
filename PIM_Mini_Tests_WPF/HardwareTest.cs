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
    public enum Status { Passed, Failed, NotRun, Mixed }

    public abstract class HardwareTest : INotifyPropertyChanged
    {
        private bool? _isChecked = false;
        private Status _testStatus = Status.NotRun;
        private HardwareTest _parent;

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
                this._parent.TestStatus = testStatus;
            }
            this._testStatus = testStatus;
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
        #endregion
    }
}

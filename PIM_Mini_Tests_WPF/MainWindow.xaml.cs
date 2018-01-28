using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PIM_Mini_Tests_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<HardwareTest> TestList { get; set; }

        public MainWindow()
        {
            TestList = new ObservableCollection<HardwareTest>
            {
                new Startup()
            };
            InitializeComponent();
            TestTreeView.DataContext = TestList;
            TestTreeView.ItemsSource = TestList;
        }

        private void TestMethod_Checked(object sender, RoutedEventArgs e)
        {
            Method senderMethod = (Method)((CheckBox)sender).DataContext;
            senderMethod.IsSelected = true;
        }

        private void TestMethod_Unchecked(object sender, RoutedEventArgs e)
        {
            Method senderMethod = (Method)((CheckBox)sender).DataContext;
            senderMethod.IsSelected = false;
        }

        private void Class_Checked(object sender, RoutedEventArgs e)
        {
            HardwareTest senderClass = (HardwareTest)((CheckBox)sender).DataContext;
            foreach (var method in senderClass.TestMethods)
            {
                method.IsSelected = true;
            }
        }

        private void Class_Unchecked(object sender, RoutedEventArgs e)
        {
            HardwareTest senderClass = (HardwareTest)((CheckBox)sender).DataContext;
            foreach (var method in senderClass.TestMethods)
            {
                method.IsSelected = false;
            }
        }
    }
}

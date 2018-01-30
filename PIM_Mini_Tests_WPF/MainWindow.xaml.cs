using Serilog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        private ObservableCollection<HardwareTest> tests;

        public MainWindow()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("logs\\PIM_Mini_Tests.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            InitializeComponent();
            tests = new ObservableCollection<HardwareTest>()
            {
                new Startup.Startup()
            };
            this.tree.DataContext = this.tests;
            this.tree.ItemsSource = this.tests;
            this.tree.Focus();
        }

        private void SelectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var test in this.tests)
            {
                test.IsChecked = true;
            }
        }

        private void UnselectAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var test in this.tests)
            {
                test.IsChecked = false;
            }
        }

        private void ErrorOutput_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var dataContext = (HardwareTest)button.DataContext;
            if ((string)button.Content == "↓")
            {
                button.Content = "↑";
                dataContext.OutputVisibility = Visibility.Visible;
            }
            else
            {
                button.Content = "↓";
                dataContext.OutputVisibility = Visibility.Collapsed;
            }
        }

        private void RunTests_Click(object sender, RoutedEventArgs e)
        {
            foreach (var test in this.tests)
            {
                test.ResetTestData();
            }
            foreach (var test in this.tests)
            {
                test.StartChildTests();
            }
        }
    }
}

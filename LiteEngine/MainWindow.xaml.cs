using AnotherExternalMemoryLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

namespace LiteEngine
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public static MainWindow Instance { get; private set; }
        public MainWindow()
        {
            Instance = this;
            InitializeComponent();
        }

        public BindingList<Process> ProcessList { get; } = new BindingList<Process>();
        public BindingList<ScanResult> SavedResults { get; } = new BindingList<ScanResult>();

        public Type[] SUPPORTED_SCAN_TYPES { get; } = 
        { 
            typeof(bool), 
            typeof(byte), 
            typeof(sbyte),
            typeof(char),
            typeof(decimal),
            typeof(double),
            typeof(float),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(short), 
            typeof(ushort), 
        };

        private ScanInstance scanInstance;
        public ScanInstance ScanInstance
        {
            get => scanInstance;
            private set
            {
                scanInstance = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ScanInstance)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void RefreshProcessList()
        {
            var allProcesses = Process.GetProcesses();
            foreach ( var process in ProcessList.ToList() )
            {
                if ( !allProcesses.Any(p => p.Id == process.Id) )
                {
                    ProcessList.Remove(process);
                }
            }
            foreach ( var process in allProcesses )
            {
                if ( !ProcessList.Any(p => p.Id == process.Id) )
                {
                    ProcessList.Add(process);
                }
            }
        }
         
        private void SelectedProcessComboBox_DropDownOpened( object sender, EventArgs e )
        {
            RefreshProcessList();
        }

        private async void ExecuteScanButton_Click( object sender, RoutedEventArgs e )
        {
            if( ScanInstance == null )
            {
                ScanInstance = new ScanInstance();
            }
            await ScanInstance.NextScan(Convert.ChangeType(ScanInputTextBox.Text, ScanDataTypeComboBox.SelectedItem as Type));
        }

        private void ResetScanButton_Click( object sender, RoutedEventArgs e )
        {
            ScanInstance = null;
        }

        private void ScanResultsListbox_MouseDoubleClick( object sender, MouseButtonEventArgs e )
        {
            if(ScanResultsListbox.SelectedItem is ScanResult scanResult)
            {
                SavedResults.Add(scanResult);
            }
        }

        private void SelectedProcessComboBox_SelectionChanged( object sender, SelectionChangedEventArgs e )
        {
            SelectedModuleComboBox.SelectedIndex = 0;
        }
    }
}

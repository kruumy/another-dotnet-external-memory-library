using AnotherExternalMemoryLibrary;
using AnotherExternalMemoryLibrary.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LiteEngine
{
    public class ScanInstance
    {
        public ScanInstance(  )
        {
        }

        public BindingList<ScanResult> Results { get; } = new BindingList<ScanResult>();

        public async Task NextScan( object value )
        {
            if ( Results.Count <= 0 )
            {
                var results = await Task.Run(() =>
                {
                    return ScanProcessMemory.Scan(
                        MainWindow.Instance.Dispatcher.Invoke(() => MainWindow.Instance.SelectedProcessComboBox.SelectedItem as Process).Handle,
                        MainWindow.Instance.Dispatcher.Invoke(() => MainWindow.Instance.SelectedProcessComboBox.SelectedItem as Process).MainModule,
                        value.GetType() == typeof(byte[]),
                        ( float progress ) => { MainWindow.Instance.Dispatcher.Invoke(() => { MainWindow.Instance.MainProgressBar.Value = progress; });  },
                        value.ToByteArrayUnsafe()).ToList();
                });
                MainWindow.Instance.MainProgressBar.Value = 0;
                foreach ( IntPtrEx result in results )
                {
                    Results.Add(new ScanResult(result, MainWindow.Instance.ScanDataTypeComboBox.SelectedItem as Type, MainWindow.Instance.SelectedProcessComboBox.SelectedItem as Process));
                }
            }
            else
            {
                foreach ( ScanResult result in Results.ToList() )
                {
                    object resultValue = ReadProcessMemory.Read<byte>((MainWindow.Instance.SelectedProcessComboBox.SelectedItem as Process).Handle, result.Address, Marshal.SizeOf(MainWindow.Instance.ScanDataTypeComboBox.SelectedItem as Type) ).ToStruct(MainWindow.Instance.ScanDataTypeComboBox.SelectedItem as Type);
                    if ( !resultValue.Equals(value) )
                    {
                        Results.Remove(result);
                    }
                }
            }
        }
    }
}

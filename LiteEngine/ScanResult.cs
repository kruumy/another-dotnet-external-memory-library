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
using System.Timers;
using System.Windows.Threading;

namespace LiteEngine
{
    public class ScanResult : INotifyPropertyChanged
    {
        public Timer ValueRefresher { get; } = new Timer();
        public IntPtrEx Address { get; }

        private object value = null;

        public Type ValueType { get; }
        public object Value
        {
            get => value;
            private set
            {
                this.value = Convert.ChangeType(value, ValueType);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WritableValue)));
            }
        }

        public object WritableValue
        {
            get => value;
            set
            {
                var realValue = Convert.ChangeType(value, ValueType);
                WriteProcessMemory.Write<byte>((MainWindow.Instance.SelectedProcessComboBox.SelectedItem as Process).Handle,Address, realValue.ToByteArrayUnsafe());
                this.value = realValue;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WritableValue)));
            }
        }

        public ScanResult(IntPtrEx Address, Type ValueType, Process TargetProcess)
        {
            this.Address = Address;
            this.ValueType = ValueType;
            ValueRefresher.Interval = 1000;
            ValueRefresher.Elapsed += ValueRefresher_Elapsed;
            ValueRefresher.Start();
        }

        private void ValueRefresher_Elapsed( object sender, ElapsedEventArgs e )
        {
            MainWindow.Instance.Dispatcher.Invoke(() =>
            {
                object newValue = ReadProcessMemory.Read<byte>((MainWindow.Instance.SelectedProcessComboBox.SelectedItem as Process).Handle, Address, Marshal.SizeOf(ValueType)).ToStruct(ValueType);
                if ( !newValue.Equals(Value) )
                {
                    Value = newValue;
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

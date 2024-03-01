using AnotherExternalMemoryLibrary;
using System;
using System.CodeDom;
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
using System.Windows.Shapes;

namespace LiteEngine
{
    /// <summary>
    /// Interaction logic for RPCWindow.xaml
    /// </summary>
    public partial class RPCWindow : Window
    {
        public RPCWindow()
        {
            InitializeComponent();
        }

        private void AddParameterButton_Click( object sender, RoutedEventArgs e )
        {
            Grid grid = new Grid();

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            ComboBox comboBox1 = new ComboBox
            {
                Margin = new Thickness(3),
                ItemsSource = new List<Type>(MainWindow.Instance.SUPPORTED_SCAN_TYPES)
                {
                    typeof(string)
                },
                DisplayMemberPath = "Name",
                SelectedIndex = 7
            };
            ComboBox comboBox2 = new ComboBox
            {
                Margin = new Thickness(3),
                ItemsSource = new string[] { "EAX", "ECX", "EDX", "EBX", "ESP", "EBP", "ESI", "EDI" },
                SelectedIndex = 0
            };

            Binding binding = new Binding
            {
                Source = ChooseCallingMethodComboBox,
                Path = new PropertyPath("SelectedItem.Content"),
                Converter = new EqualityConverter(),
                ConverterParameter = "UserCallx86"
            };
            comboBox2.SetBinding(ComboBox.IsEnabledProperty, binding);

            TextBox textBox = new TextBox
            {
                Margin = new Thickness(3)
            };

            Grid.SetColumn(comboBox1, 0);
            grid.Children.Add(comboBox1);

            Grid.SetColumn(comboBox2, 1);
            grid.Children.Add(comboBox2);

            Grid.SetColumn(textBox, 2);
            grid.Children.Add(textBox);

            ParameterStackPanel.Children.Add(grid);
        }

        private void RemoveParameterButton_Click( object sender, RoutedEventArgs e )
        {   
            if( ParameterStackPanel.Children.Count > 0)
            {
                ParameterStackPanel.Children.RemoveAt(ParameterStackPanel.Children.Count - 1);
            }
        }

        private async void ExecuteCallButton_Click( object sender, RoutedEventArgs e )
        {
            try
            {
                if ( ChooseCallingMethodComboBox.SelectedItem is ComboBoxItem comboBoxItem && comboBoxItem.Content is string callTypeName )
                {
                    ExecuteCallButton.IsEnabled = false;
                    switch ( callTypeName )
                    {
                        case "Callx86":
                            {
                                int returninfo = await HandleCallx86();
                                MessageBox.Show(returninfo.ToString(), "Callx86 Return Information");
                                break;
                            }
                        case "Callx64":
                            {
                                HandleCallx64();
                                break;
                            }
                        case "UserCallx86":
                            {
                                HandleUserCallx86();
                                break;
                            }
                    }
                    await Task.Delay(1000);
                    ExecuteCallButton.IsEnabled = true;
                }
            }
            catch(Win32Exception)
            {
                MessageBox.Show("Administrator Privaliges Required","Error",MessageBoxButton.OK,MessageBoxImage.Error);
                return;
            }
        }


        private async Task<int> HandleCallx86()
        {
            Process process = MainWindow.Instance.SelectedProcessComboBox.SelectedItem as Process;
            IntPtrEx address = Convert.ToInt32(CallingAddressTextBox.Text.Replace("0x",string.Empty), 16);
            if( ParameterStackPanel.Children.Count <= 0)
            {
                return await RemoteProcedureCall.Callx86(process.Handle, address, maxReturnAttempts: 10, Array.Empty<ValueType>());
            }
            else
            {
                List<object> parameters = new List<object>(ParameterStackPanel.Children.Count);
                foreach (var gridobj in ParameterStackPanel.Children )
                {
                    Grid grid = gridobj as Grid;
                    ComboBox dataTypeComboBox = grid.Children[ 0 ] as ComboBox;
                    TextBox parameterInputTextBox = grid.Children[ 2 ] as TextBox;
                    Type dataType = dataTypeComboBox.SelectedItem as Type;

                    parameters.Add(Convert.ChangeType(parameterInputTextBox.Text,dataType));
                }

                return await RemoteProcedureCall.Callx86(process.Handle, address, maxReturnAttempts: 5, parameters: parameters.ToArray());
            }
        }
        private void HandleCallx64()
        {
            Process process = MainWindow.Instance.SelectedProcessComboBox.SelectedItem as Process;


            throw new NotImplementedException();
            //RemoteProcedureCall.Callx64(process.Handle);
        }
        private void HandleUserCallx86()
        {
            Process process = MainWindow.Instance.SelectedProcessComboBox.SelectedItem as Process;
            IntPtrEx address = Convert.ToInt32(CallingAddressTextBox.Text.Replace("0x", string.Empty), 16);

            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                [ "EAX" ] = null,
                [ "ECX" ] = null,
                [ "EDX" ] = null,
                [ "EBX" ] = null,
                [ "ESP" ] = null,
                [ "EBP" ] = null,
                [ "ESI" ] = null,
                [ "EDI" ] = null
            };
            foreach ( var gridobj in ParameterStackPanel.Children )
            {
                Grid grid = gridobj as Grid;
                ComboBox dataTypeComboBox = grid.Children[ 0 ] as ComboBox;
                ComboBox registerComboBox = grid.Children[ 1 ] as ComboBox;
                TextBox parameterInputTextBox = grid.Children[ 2 ] as TextBox;
                Type dataType = dataTypeComboBox.SelectedItem as Type;
                string register = registerComboBox.SelectedItem as string;

                parameters[ register ] = Convert.ChangeType(parameterInputTextBox.Text, dataType);
            }
            RemoteProcedureCall.UserCallx86(process.Handle, address, 
                parameters[ "EAX" ], 
                parameters[ "ECX" ], 
                parameters[ "EDX" ], 
                parameters[ "EBX" ], 
                parameters[ "ESP" ],
                parameters[ "EBP" ],
                parameters[ "ESI" ], 
                parameters[ "EDI" ]);
        }

        
    }
}

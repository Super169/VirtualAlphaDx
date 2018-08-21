using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

namespace VirtualAlphaDX
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Slider[] joints = new Slider[17];
        private bool updateFromAlpha = false;

        public MainWindow()
        {
            InitializeComponent();

            Alpha.Initialization();
            Alpha.ServoMoved += ServoMoved_EventHandler;

            InitializeVariable();
            InitializeSerialPort();
            FindPorts((string)Util.ReadRegistry(Util.KEY.LAST_CONNECTION));
            Closed += (s, e) => {
                if (DataContext is IDisposable)
                {
                    (DataContext as IDisposable).Dispose();
                }
                Alpha.StopAnimation();
            };
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (serialPort.IsOpen) serialPort.Close();
        }

        private void InitializeVariable()
        {
            joints[1] = joint1;
            joints[2] = joint2;
            joints[3] = joint3;
            joints[4] = joint4;
            joints[5] = joint5;
            joints[6] = joint6;
            joints[7] = joint7;
            joints[8] = joint8;
            joints[9] = joint9;
            joints[10] = joint10;
            joints[11] = joint11;
            joints[12] = joint12;
            joints[13] = joint13;
            joints[14] = joint14;
            joints[15] = joint15;
            joints[16] = joint16;
            updateFromAlpha = true;
            for (int id = 1; id <= 16; id++)
            {
                double angle;
                double minAngle;
                double maxAngle;
                if (Alpha.GetServoInfo(id, out angle, out minAngle, out maxAngle))
                {
                    joints[id].Value = angle;
                    joints[id].Minimum = minAngle;
                    joints[id].Maximum = maxAngle;

                }
            }
            updateFromAlpha = false;
        }

        private byte GetInputByte(string data)
        {
            if (data.EndsWith("."))
            {
                data = data.Substring(0, data.Length - 1);
                return (byte)Convert.ToInt32(data, 10);
            }
            return (byte)Convert.ToInt32(data, 16);
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            UpdateInfo();
            string sCommand = txtCommand.Text.Trim();
            txtSend.Text = "";
            txtReturn.Text = "";

            if (sCommand == "")
            {
                txtSend.Text = "Please exnter HEX command";
                UpdateInfo("Please exnter HEX command", Util.InfoType.alert);
                return;
            }

            if (rbServo.IsChecked == true)
            {
                ExecuteServoCommand(sCommand);
            }
            else {
                ExecuteControlBoard(sCommand);
            }
        }

        private void ServoMoved_EventHandler(int id, double angle)
        {
            updateFromAlpha = true;
            joints[id].Value = angle;
            updateFromAlpha = false;
        }

        private void joint_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (updateFromAlpha) return;
            Slider s = (Slider)sender;
            string tag = (string) s.Tag;
            try
            {
                int id = int.Parse(tag);
                double angle = s.Value;
                Alpha.MoveTo(id, angle, 0);
                Alpha.StartAnimation();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void tb_PreviewHexInput(object sender, TextCompositionEventArgs e)
        {
            if (rbServo.IsChecked == true)
            {
                e.Handled = new Regex("[^0-9A-Fa-f.]+").IsMatch(e.Text);
            } else
            {
                e.Handled = false;
            }
        }

        private void findPortButton_Click(object sender, RoutedEventArgs e)
        {
            FindPorts((string)portsComboBox.SelectedValue);
        }

        private void SetStatus()
        {
            bool connected = serialPort.IsOpen;
            portsComboBox.IsEnabled = !connected;
            findPortButton.IsEnabled = !connected;
            findPortButton.Visibility = (connected ? Visibility.Hidden : Visibility.Visible);
            connectButton.Content = (connected ? "斷開" : "連線");
            gridConnection.Background = new SolidColorBrush(connected ? Colors.LightGreen : Colors.Aqua);
        }

        private void connectButton_Click(object sender, RoutedEventArgs e)
        {
            if (serialPort.IsOpen)
            {
                SerialDisconnect();
            }
            else
            {
                SerialConnect((string)portsComboBox.SelectedValue);
            }
            SetStatus();

        }

        private void rbServo_Checked(object sender, RoutedEventArgs e)
        {
            txtCommand.Text = "";
        }

        private void rbControlBoard_Checked(object sender, RoutedEventArgs e)
        {
            txtCommand.Text = "";
        }

    }
}

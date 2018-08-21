using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace VirtualAlphaDX
{
    public partial class MainWindow : Window
    {
        private SerialPort serialPort = new SerialPort();
        private List<byte> receiveBuffer = new List<byte>();
        private long lastSerialTick;
        private long maxCommandTicks = 100 * TimeSpan.TicksPerMillisecond;
        // maximum wait for a complete command

        private Timer serialTimer;

        private void InitializeSerialPort()
        {
            serialPort.DataReceived += SerialPort_DataReceived;

            serialTimer = new Timer();
            serialTimer.Interval = 10;
            serialTimer.Tick += new EventHandler(serialTimer_TickHandler);

       }

        private void serialTimer_TickHandler(object sender, EventArgs e)
        {
            serialTimer.Stop();
            if (receiveBuffer.Count > 0)
            {
                if (rbServo.IsChecked==true)
                {
                    HandleServoCommandBuffer();
                } else
                {
                    HandleControlBoardBuffer();
                }
            }
            serialTimer.Start();
        }


        private void FindPorts(string defaultPort)
        {
            portsComboBox.ItemsSource = SerialPort.GetPortNames();
            if (portsComboBox.Items.Count > 0)
            {
                if (defaultPort == null)
                {
                    portsComboBox.SelectedIndex = 0;
                }
                else
                {
                    portsComboBox.SelectedIndex = portsComboBox.Items.IndexOf(defaultPort);
                }
                portsComboBox.IsEnabled = true;
            }
            else
            {
                portsComboBox.IsEnabled = false;
            }
        }

        private bool SerialConnect(String portName)
        {
            bool flag = false;

            UpdateInfo();

            if (serialPort.IsOpen)
            {
                UpdateInfo(string.Format("Port {0} already connected - 115200, N, 8, 1", serialPort.PortName), Util.InfoType.alert);
                return true;
            }

            serialPort.PortName = portName;
            serialPort.BaudRate = 115200;
            serialPort.Parity = Parity.None;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;

            try
            {
                serialPort.Open();
                if (serialPort.IsOpen)
                {
                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();
                    UpdateInfo(string.Format("Port {0} connected - 115200, N, 8, 1", serialPort.PortName));
                    serialTimer.Start();
                    Util.WriteRegistry(Util.KEY.LAST_CONNECTION, portName);
                    flag = true;
                } else
                {
                    UpdateInfo(string.Format("Fail connecting to Port {0} - 115200, N, 8, 1", serialPort.PortName), Util.InfoType.error);
                }
            }
            catch (Exception ex)
            {
                UpdateInfo("Error: " + ex.Message, Util.InfoType.error);
            }
            return flag;
        }

        private bool SerialDisconnect()
        {

            if (!serialPort.IsOpen)
            {
                UpdateInfo(string.Format("Port {0} not yet connected", serialPort.PortName), Util.InfoType.alert);
                serialTimer.Stop();
                return true;
            }

            UpdateInfo();

            serialPort.Close();

            if (serialPort.IsOpen)
            {
                UpdateInfo(string.Format("Fail to disconnect Port {0}", serialPort.PortName), Util.InfoType.error);
                return false;
            }

            UpdateInfo(string.Format("Port {0} disconnected", serialPort.PortName));
            return true;
        }

        private void SerialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            System.IO.Ports.SerialPort sp = sender as System.IO.Ports.SerialPort;

            if (sp == null) return;

            int bytesToRead = sp.BytesToRead;
            byte[] tempBuffer = new byte[bytesToRead];

            sp.Read(tempBuffer, 0, bytesToRead);

            //TODO: May need to lock receiveBuffer first
            receiveBuffer.AddRange(tempBuffer);
        }


    }

}

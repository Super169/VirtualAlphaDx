using System;
using System.Collections.Generic;
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

namespace VirtualAlphaDX
{
    /// <summary>
    /// Interaction logic for UcAlpha.xaml
    /// </summary>
    public partial class UcAlpha : UserControl
    {
        public delegate void ServoMovedEventHandler(int id, double angle);
        public event ServoMovedEventHandler ServoMoved = null;
        byte[] servo_version = { 0x21, 0x16, 0x13, 0x01 };

        public void ServoMovedNotification(int id, double angle)
        {
            ServoMoved?.Invoke(id, angle);
        }

        UcAlphaViewModel Alpha;
        public UcAlpha()
        {
            InitializeComponent();
            Alpha = new UcAlphaViewModel(this.viewport3DX, this.ServoMovedNotification);
            this.DataContext = Alpha;
        }

        public void Initialization(ServoMovedEventHandler servoMovedEventHandler = null)
        {
            this.ServoMoved = servoMovedEventHandler;

            actionTable = new ActionTable();
            ReadSPIFFS();
            Alpha.Initialization();
            object value = Util.ReadRegistry(Util.KEY.SERVO_VERSION);
            if ((value == null) || (value.GetType().Name != "Byte[]"))
            {
                Util.WriteRegistry(Util.KEY.SERVO_VERSION, servo_version);
            }
            else 
            {
                byte[] version = (byte[])value;
                if (version.Length == 4)
                {
                    for (int idx = 0; idx < 4; idx++)
                    {
                        servo_version[idx] = version[idx];
                    }
                } else
                {
                    Util.WriteRegistry(Util.KEY.SERVO_VERSION, servo_version);
                }
            }
        }

        public void DummyAction()
        {
            Alpha.DummyAction();
        }

        public bool MoveTo(int id, double angle, int ms)
        {
            return Alpha.MoveTo(id, angle, ms);
        }

        public double GetServoAngle(int id)
        {
            if ((id < 1) || (id > 16)) return -1;
            return Alpha.GetServoAngle(id);
        }

        public bool GetServoInfo(int id, out double angle, out double minAngle, out Double maxAngle)
        {
            Part part = Alpha.GetPart(id);
            if (part == null)
            {
                angle = 0;
                minAngle = 0;
                maxAngle = 180;
                return false;
            }
            angle = part.angle;
            minAngle = part.minAngle;
            maxAngle = part.maxAngle;
            return true;
        }

        public void StartAnimation()
        {
            Alpha.StartAnimation();
        }

        public void StopAnimation()
        {
            Alpha.StopAnimation();
        }

    }
}

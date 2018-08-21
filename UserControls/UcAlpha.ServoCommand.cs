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

        public byte[] ServoCommand(byte[] command)
        {
            byte[] result = new byte[10];
            for (int i = 0; i < 10; i++) result[i] = 0;

            if (command.Count() != 10) return null;
            if (command[9] != 0xED) return null;

            // Only 16 servo available
            if (command[2] > 16) return null;
            byte id = command[2];

            byte checksum = CalCheckSum(command);

            for (int i = 0; i < 3; i++) result[i] = command[i];
            result[9] = 0xED;

            if (command[8] != checksum) return null;

            // no return when working
            if (Alpha.IsAnimation(id)) return null;

            if ((command[0] == 0xFC) && (command[1] == 0xCF))
            {
                result[3] = 0xAA;
                // new command 0xFF to set servo version
                if ((command[2] == 0) && (command[3] == 0xFF))
                {
                    // Default version: 21 16 13 01 
                    for (int idx = 0; idx < 4; idx++)
                    {
                        servo_version[idx] = command[4 + idx];
                    }
                    Util.WriteRegistry(Util.KEY.SERVO_VERSION, servo_version);
                }
                for (int idx = 0; idx < 4; idx++)
                {
                    result[4 + idx] = servo_version[idx];
                }
                result[8] = CalCheckSum(result);
                return result;
            }

            if ((command[0] == 0xFA) && (command[1] == 0xAF))
            {
                switch (command[3])
                {
                    case 0x01: // Move
                        {
                            int angle = command[4];
                            int time = command[5];
                            time = time * 1000 / 40;   // assume 40 = 1s
                            if (id == 0)
                            {
                                for (int i = 1; i < 17; i++)
                                {
                                    Alpha.MoveTo(i, angle, time);
                                }
                            }
                            else
                            {
                                Alpha.MoveTo(id, angle, time);
                                Alpha.StartAnimation();
                            }
                            result = new byte[1];
                            result[0] = (byte)(0xAA + id);
                            return result;
                        }
                    case 0x02:  // Get Angle
                    case 0x03:  // Same return
                        {
                            if (id == 0) return null;
                            double dAngle = Alpha.GetServoAngle(id);
                            byte angle = (byte)Math.Round(dAngle, 0);
                            result[3] = 0xAA;
                            result[4] = 0;
                            result[5] = angle;
                            result[6] = 0;
                            result[7] = angle;
                            break;
                        }
                    case 0x04:
                        {
                            result = new byte[1];
                            result[0] = (byte)(0xAA + id);
                            return result;
                        }
                    case 0xCD:
                        {
                            result[3] = 0xEE;
                            result[4] = 0;
                            result[5] = id;
                            result[6] = 0;
                            break;
                        }
                    case 0xD2:
                        {
                            result[3] = 0xEE;
                            result[4] = 0;
                            result[5] = id;
                            result[6] = 0;
                            break;
                        }
                    case 0xD4:
                        {
                            result[3] = 0xAA;
                            break;
                        }
                    default:
                        return null;
                }
                result[8] = CalCheckSum(result);
                return result;
            }

            // Unexpected command, return error
            result[3] = (byte)(0xEE + result[2]);
            for (int i = 4; i < 8; i++) result[i] = command[i];
            result[8] = CalCheckSum(result);
            return result;
        }

        private byte CalCheckSum(byte[] data)
        {
            int sum = 0;
            for (int i = 2; i < 8; i++)
            {
                sum += data[i];
            }
            sum %= 256;
            return (byte)sum;
        }

    }
}

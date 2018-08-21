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
        private void GetInputCommand(string data, List<byte> result)
        {
            if (data.StartsWith("'"))
            {
                result.Add((byte)data[1]);
            }
            else if (data.EndsWith("."))
            {
                data = data.Substring(0, data.Length - 1);
                result.Add((byte)Convert.ToInt32(data, 10));
            }
            else
            {
                result.Add((byte)Convert.ToInt32(data, 16));
            }
        }

        private void ExecuteControlBoard(string sCommand)
        {
            Regex rx = new Regex("\\s+");
            string input = rx.Replace(sCommand, " ");
            string[] sData = input.Split(' ');
            List<byte> lCommand = new List<byte>();
            for (int i = 0; i < sData.Length; i++)
            {
                GetInputCommand(sData[i], lCommand);
            }

            byte[] command = lCommand.ToArray();

            string output = BitConverter.ToString(command);
            output = output.Replace("-", ", ");
            txtSend.Text = output;

            byte[] result = Alpha.ControlBoardV1Command(command);
            if (result == null)
            {
                txtReturn.Text = "";
            }
            else
            {
                Alpha.StartAnimation();
                output = BitConverter.ToString(result);
                output = output.Replace("-", ", ");
                txtReturn.Text = output;
                Alpha.DummyAction();
            }
        }

        private void HandleControlBoardBuffer()
        {
            int removeCnt = 0;
            long maxWaitMs = 10;    // each command should be received within 10ms
            long waitEnd = 0;
            while (removeCnt < receiveBuffer.Count)
            {
                int processedCnt = 1;
                byte cmd = receiveBuffer[removeCnt];
                bool pending = false;
                switch (cmd)
                {
                    // V2 command set
                    case 0xA9:
                        {
                            // each V2 command has at least 6 bytes
                            if ((receiveBuffer.Count - removeCnt) < 6)
                            {
                                pending = true;
                                break;
                            }
                            if (receiveBuffer[removeCnt + 1] != 0x9A)
                            {
                                // Invalid start code
                                break;
                            }
                            byte len = receiveBuffer[removeCnt + 2];

                            if ((receiveBuffer.Count - removeCnt) < len + 4)
                            {
                                // Incomplete command
                                pending = true;
                                break;
                            }

                            if (receiveBuffer[removeCnt + len + 3] != 0xED)
                            {
                                // Invalid end code
                                break;
                            }

                            byte[] command = new byte[len + 4];
                            processedCnt = len + 4;
                            for (int i = 0; i < processedCnt; i++)
                            {
                                command[i] = receiveBuffer[removeCnt + i];
                            }
                            byte[] result = Alpha.ControlBoardV2Command(command);
                            if (result != null)
                            {
                                serialPort.Write(result, 0, result.Length);
                            }
                        }
                        break;


                    // single byte commands
                    case (byte)'A':
                    case (byte)'B':
                    case (byte)'b':
                    case (byte)'D':
                    case (byte)'R':
                    case (byte)'S':
                    case (byte)'T':
                    case (byte)'W':
                    case (byte)'Z':
                        {
                            byte[] command = { (byte)cmd };
                            byte[] result = Alpha.ControlBoardV1Command(command);
                            if (result != null)
                            {
                                serialPort.Write(result, 0, result.Length);
                            }
                        }
                        break;

                    case (byte)'M':
                        {
                            // search for end mark: 0x00 0x00 0x00
                            int endPtr = removeCnt + 1;
                            pending = true;
                            while (endPtr + 2 < receiveBuffer.Count)
                            {
                                if ((receiveBuffer[endPtr] == 0) &&
                                    (receiveBuffer[endPtr + 1] == 0) &&
                                    (receiveBuffer[endPtr + 2] == 0))
                                {
                                    pending = false;
                                    break;
                                }
                                endPtr += 3;
                            }
                            if (pending)
                            {
                                if (endPtr - removeCnt > 48 )
                                {
                                    // calready checked on 49, 50, 51, still not ended.
                                    // There must have error, but cannot handle it as this version does not have start code
                                    // so just skip this byte, GOOD LUCK
                                    pending = false;
                                }
                                break;
                            }
                            processedCnt = endPtr + 3 - removeCnt;
                            byte[] command = new byte[processedCnt];
                            for (int i = 0; i < processedCnt; i++)
                            {
                                command[i] = receiveBuffer[removeCnt + i];
                            }
                            byte[] result = Alpha.ControlBoardV1Command(command);
                            if (result != null)
                            {
                                serialPort.Write(result, 0, result.Length);
                            }
                            if (result[1] == 0x00)
                            {
                                Alpha.StartAnimation();
                            }
                        }

                        break;

                    case (byte)'P':
                        {
                            // only 1 byte paramter required
                            pending = (removeCnt + 1 >= receiveBuffer.Count);
                            if (pending) break;
                            processedCnt = 2;
                            byte[] command = { (byte)'P', receiveBuffer[removeCnt + 1] };
                            byte[] result = Alpha.ControlBoardV1Command(command);
                            if (result != null)
                            {
                                serialPort.Write(result, 0, result.Length);
                            }
                        }
                        break;
                }
                if (pending)
                {
                    if (waitEnd == 0)
                    {
                        waitEnd = DateTime.Now.Ticks + maxWaitMs * TimeSpan.TicksPerMillisecond;
                    } else
                    {
                        if (DateTime.Now.Ticks > waitEnd)
                        {
                            pending = false;
                        }
                    }
                    if (pending) break;
                }
                removeCnt += processedCnt;
            }

            //TODO: May need to lock receiveBuffer first
            receiveBuffer.RemoveRange(0, removeCnt);
        }
    }
}

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

        private void ExecuteServoCommand(string sCommand)
        {
            Regex rx = new Regex("\\s+");
            string input = rx.Replace(sCommand, " ");
            string[] sData = input.Split(' ');
            byte[] command = new byte[10];
            try
            {
                for (int i = 0; i < 10; i++)
                {
                    if (i < sData.Length)
                    {
                        command[i] = GetInputByte(sData[i]);
                    }
                    else
                    {
                        command[i] = 0x00;
                    }
                }
                command[8] = CalCheckSum(command);
                command[9] = 0xED;
                string output = BitConverter.ToString(command);
                output = output.Replace("-", ", ");
                txtSend.Text = output;

                byte[] result = Alpha.ServoCommand(command);
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
            catch (Exception ex)
            {
                txtReturn.Text = ex.Message;
            }

        }

        private void HandleServoCommandBuffer()
        {
            int removeCnt = 0;
            // Command mode with for "FA" or "FC"
            while (removeCnt < receiveBuffer.Count)
            {
                int processedCnt = 1;
                if ((receiveBuffer[removeCnt] == 0xFA) ||
                    (receiveBuffer[removeCnt] == 0xFC))
                {
                    if (removeCnt + 10 <= receiveBuffer.Count)
                    {
                        if ((((receiveBuffer[removeCnt] == 0xFA) &&
                              (receiveBuffer[removeCnt + 1] == 0xAF)) ||
                             ((receiveBuffer[removeCnt] == 0xFC) &&
                              (receiveBuffer[removeCnt + 1] == 0xCF))
                             ) &&
                             (receiveBuffer[removeCnt + 9] == 0xED)
                            )
                        {
                            // valid start & end
                            byte checkSum = Util.UBTCheckSum(receiveBuffer, removeCnt);
                            if (receiveBuffer[removeCnt + 8] == checkSum)
                            {
                                byte[] command = new byte[10];
                                for (int i = 0; i < 10; i++)
                                {
                                    command[i] = receiveBuffer[removeCnt + i];
                                    byte[] result = Alpha.ServoCommand(command);
                                    if (result != null)
                                    {
                                        serialPort.Write(result, 0, result.Length);
                                    }

                                }
                                processedCnt = 10;
                            }
                        }
                    }
                    else
                    {
                        if (lastSerialTick == 0)
                        {
                            // wait for commmand
                            lastSerialTick = DateTime.Now.Ticks;
                            break;
                        }
                        long currTick = DateTime.Now.Ticks;
                        if (currTick < (lastSerialTick + maxCommandTicks))
                        {
                            // wait for command
                            break;
                        }
                        // Otherwise, no action can take, skip this header
                    }

                }
                removeCnt += processedCnt;
            }

            //TODO: May need to lock receiveBuffer first
            receiveBuffer.RemoveRange(0, removeCnt);
        }
    }
}

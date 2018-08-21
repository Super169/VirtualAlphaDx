using System;
using System.Collections.Generic;
using System.IO;
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
        public ActionTable actionTable;
        public bool actionTableReady = false;

        public bool ReadSPIFFS()
        {
            actionTableReady = false;
            if (File.Exists(CONST.SPIFFS_FILE))
            {
                try
                {
                    FileStream fs = new FileStream(CONST.SPIFFS_FILE, FileMode.Open, FileAccess.Read);
                    BinaryReader br = new BinaryReader(fs);
                    byte[] actionData = null;
                    actionData = br.ReadBytes(CONST.ACTION_TABLE_SIZE);
                    br.Close();
                    fs.Close();
                    actionTableReady = actionTable.ReadFromArray(actionData);
                } catch (Exception)
                {

                }
            }
            return actionTableReady;
        }

        public bool WriteSPIFFS()
        {
            bool success = false;
            try
            {
                FileStream fs = File.Create(CONST.SPIFFS_FILE, 2048, FileOptions.None);
                BinaryWriter bw = new BinaryWriter(fs);
                byte[] actionData = new byte[CONST.ACTION_TABLE_SIZE];
                actionTable.WriteToArray(actionData);
                bw.Write(actionData);
                bw.Close();
                fs.Close();
                success = true;
            }
            catch (Exception)
            {
            }
            return success;
        }

        public byte[] ControlBoardCommand(byte[] command)
        {
            return ControlBoardV1Command(command);
        }


        public byte[] ControlBoardV1Command(byte[] command)
        {
            if ((command == null) || (command.Length == 0)) return null;

            byte cmd = command[0];
            switch (cmd)
            {
                case (byte)'A':
                case (byte)'T':  // no detection in virtual robot
                    {
                        List<byte> data = new List<byte>();
                        for (int id = 1; id <= 16; id++)
                        {
                            Part part = Alpha.parts.Find(x => x.id == id);
                            data.Add((byte)part.angle);
                            data.Add((byte)(part.isLocked ? 1 : 0));
                        }
                        byte[] result = data.ToArray();
                        return result;
                    }

                // Commands no need to implemented
                case (byte)'B':
                    break;
                case (byte)'b':
                    break;
                case (byte)'S':
                    break;
                case (byte)'Z':
                    break;

                case (byte)'D':  // download action data
                    {
                        byte[] actionData = new byte[CONST.ACTION_TABLE_SIZE];
                        if (actionTableReady)
                        {
                            actionTable.WriteToArray(actionData);
                        }
                        return actionData;
                    }

                case (byte)'M':  // move servo
                    {
                        byte[] buffer;
                        int moveCnt = MoveMultiServo(command, out buffer);
                        int resultSize = moveCnt + 3;
                        byte[] result = new byte[resultSize];
                        Buffer.BlockCopy(buffer, 0, result, 0, resultSize);
                        return result;
                    }

                case (byte)'R':  // read from SPIFFS
                    {
                        byte[] result = { (byte)'R', 1 };
                        if (ReadSPIFFS())
                        {
                            result[1] = 0;
                        }
                        return result;
                    }

                case (byte)'U':  // download action data
                    break;

                case (byte)'W':  // write to SPIFFS
                    {
                        byte[] result = { (byte)'W', 1 };
                        if (WriteSPIFFS())
                        {
                            result[1] = 0;
                        }
                        return result;
                    }

                case (byte)'P':  // play action
                    { }
                    break;

            }
            return null;
        }

        public byte[] ControlBoardV2Command(byte[] command)
        {
            return null;
        }

        private int MoveMultiServo(byte[] command, out byte[] result)
        {
            result = new byte[20];
            result[0] = (byte)'M';
            result[1] = 0x00;
            result[2] = 0x00;

            if ((command.Length < 7) || ((command.Length - 1) % 3 != 0))
            {
                result[1] = CONST.MOVE_ERR_PARM_CNT;
                return 0;
            }

            if ((command[command.Length - 1] != 0) ||
                (command[command.Length - 2] != 0) ||
                (command[command.Length - 3] != 0))
            {
                result[1] = CONST.MOVE_ERR_PARM_END;
                return 0;
            }

            int servoCnt = (command.Length - 4) / 3;
            int moveCnt = 0;
            byte[,] moveData = new byte[16, 3];
            if ((servoCnt == 1) && (command[1] == 0))
            {
                byte angle = command[2];
                byte time = command[3];
                if (angle > 0xF0)
                {
                    result[1] = CONST.MOVE_ERR_PARM_ALL_ANGLE;
                    return 0;
                }
                // Move all servo
                for (int id = 1; id <= 16; id++)
                {
                    moveData[moveCnt,0] = (byte) id;
                    moveData[moveCnt, 1] = angle;
                    moveData[moveCnt, 2] = time;
                    moveCnt++;
                }
            } else
            {
                moveCnt = 0;
                for (int i = 0; i < servoCnt; i++)
                {
                    byte id = command[i * 3 + 1];
                    byte angle = command[i * 3 + 2];
                    byte time = command[i * 3 + 3];
                    if (id > 16)
                    {
                        result[1] = CONST.MOVE_ERR_PARM_ONE_ID;
                        return 0;
                    }
                    if (angle > 0xF0)
                    {
                        result[1] = CONST.MOVE_ERR_PARM_ONE_ANGLE;
                    }
                    for (int j = 0; j < moveCnt; j++)
                    {
                        if (moveData[j,0] == id)
                        {
                            result[1] = CONST.MOVE_ERR_PARM_DUP_ID;
                            return 0;
                        }
                    }
                    // no need to check servo availibility in Virtual Robot

                    moveData[moveCnt, 0] = id;
                    moveData[moveCnt, 1] = angle;
                    moveData[moveCnt, 2] = time;
                    moveCnt++;
                }
            }

            result[1] = CONST.MOVE_OK;
            result[2] = (byte) moveCnt;
            if (moveCnt == 0) return 0;  // should not happen in Virtual Robot as all servo available

            for (int i = 0; i < moveCnt; i++)
            {
                int timeMs = moveData[i, 2] * 20;
                Alpha.MoveTo(moveData[i, 0], moveData[i, 1], timeMs);
                result[i + 3] = moveData[i, 0];
            }
            return moveCnt;
        }

    }
}

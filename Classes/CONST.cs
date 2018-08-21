using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualAlphaDX
{
    public static class CONST
    {
        public const string SPIFFS_FILE = "SPIFFS.dat";

        public const int MAX_ACTION = 26;
        public const int MAX_POSES = 30;
        public const int MAX_POSES_SIZE = 20;
        public const int ACTION_TABLE_SIZE = MAX_ACTION * MAX_POSES * MAX_POSES_SIZE;

        public const int ENABLE_FLAG = 0;
        public const int ID_OFFSET = 0;
        public const int EXECUTE_TIME = 17;
        public const int WAIT_TIME_HIGH = 18;
        public const int WAIT_TIME_LOW = 19;


        public const char CMD_GET_ANGLE = 'A';
        public const char CMD_DEBUG_ENBLE = 'B';
        public const char CMD_DEBUG_DISABLE = 'b';
        public const char CMD_DOWNLOAD = 'D';
        public const char CMD_UPLOAD = 'U';
        public const char CMD_READ_SPIFFS = 'R';
        public const char CMD_WRITE_SPIFFS = 'W';
        public const char CMD_LOCK_SERVO = 'L';
        public const char CMD_FREE_SERVO = 'F';
        public const char CMD_MOVE_SERVO = 'M';
        public const char CMD_PLAY = 'P';
        public const char CMD_DETECT_SERVO = 'T';
        public const char CMD_RESET_CONN = 'Z';

        public const byte MOVE_OK = 0x00;
        public const byte MOVE_ERR_PARM_CNT = 0x01;
        public const byte MOVE_ERR_PARM_VALUE = 0x02;
        public const byte MOVE_ERR_PARM_CONTENT = 0x03;
        public const byte MOVE_ERR_PARM_END = 0x04;
        public const byte MOVE_ERR_PARM_ALL_CNT = 0x11;
        public const byte MOVE_ERR_PARM_ALL_ANGLE = 0x12;
        public const byte MOVE_ERR_PARM_ONE_ID = 0x21;
        public const byte MOVE_ERR_PARM_ONE_ANGLE = 0x22;
        public const byte MOVE_ERR_PARM_DUP_ID = 0x23;


    }
}

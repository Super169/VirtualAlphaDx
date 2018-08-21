using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualAlphaDX
{
    public class PoseInfo
    {
        public int actionId { get; set; }
        public int poseId { get; set; }
        public bool enabled { get; set; }
        public byte[] angle = new byte[17];  // to sync with id, use id directly, so 17 item here.
        public byte time { get; set; }
        public int waitTime { get; set; }
        public string s01 { get { return ServoDisplayStr(1); } }
        public string s02 { get { return ServoDisplayStr(2); } }
        public string s03 { get { return ServoDisplayStr(3); } }
        public string s04 { get { return ServoDisplayStr(4); } }
        public string s05 { get { return ServoDisplayStr(5); } }
        public string s06 { get { return ServoDisplayStr(6); } }
        public string s07 { get { return ServoDisplayStr(7); } }
        public string s08 { get { return ServoDisplayStr(8); } }
        public string s09 { get { return ServoDisplayStr(9); } }
        public string s10 { get { return ServoDisplayStr(10); } }
        public string s11 { get { return ServoDisplayStr(11); } }
        public string s12 { get { return ServoDisplayStr(12); } }
        public string s13 { get { return ServoDisplayStr(13); } }
        public string s14 { get { return ServoDisplayStr(14); } }
        public string s15 { get { return ServoDisplayStr(15); } }
        public string s16 { get { return ServoDisplayStr(16); } }


        private string ServoDisplayStr(byte id)
        {
            if (waitTime == 0) return "---";
            byte angle = this.angle[id];
            if (angle > 240)
            {
                return "---";
            }
            else
            {
                return angle.ToString();
            }
        }

        private byte[] emptyPose = {0x00,
                                    0xff,0xff,0xff,
                                    0xff,0xff,0xff,
                                    0xff,0xff,0xff,0xff,0xff,
                                    0xff,0xff,0xff,0xff,0xff,
                                    0x00,0x00,0x00
                                   };
        

        public PoseInfo(int actionId, int poseId)
        {
            this.actionId = actionId;
            this.poseId = poseId;
            this.Reset();
        }

        public void Reset()
        {
            this.enabled = false;
            for (int i = 1; i <= 16; i++) angle[i] = 0xFF;
            this.time = 0;
            this.waitTime = 0;
        }

        public bool Update(int actionId, int poseId, byte[] angle, byte time, int waitTime)
        {
            if ((this.actionId != actionId) || (this.poseId != poseId)) return false;
            for (int id = 1; id <= 16; id++) this.angle[id] = angle[id];
            this.time = time;
            this.waitTime = waitTime;
            return true;
        }

        public bool ReadFromArray(byte[] actionData, int actionId, int poseId)
        {
            if ((this.actionId != actionId) || (this.poseId != poseId)) return false;
            int startPos = (this.actionId * CONST.MAX_POSES + this.poseId) * CONST.MAX_POSES_SIZE;
            enabled = (actionData[startPos + CONST.ENABLE_FLAG] == 1);
            for (int id = 1; id <= 16; id++) angle[id] = actionData[startPos + CONST.ID_OFFSET + id];
            time = actionData[startPos + CONST.EXECUTE_TIME];
            waitTime = 256 * actionData[startPos + CONST.WAIT_TIME_HIGH] + actionData[startPos + CONST.WAIT_TIME_LOW];
            return true;
        }

        public void WriteToArray(byte[] actionData)
        {
            int startPos = (actionId * CONST.MAX_POSES + poseId) * CONST.MAX_POSES_SIZE;
            if (waitTime > 0)
            {
                actionData[startPos + CONST.ENABLE_FLAG] = (byte)(enabled ? 1 : 0);
                for (int id = 1; id <= 16; id++) actionData[startPos + CONST.ID_OFFSET + id] = angle[id];
                actionData[startPos + CONST.EXECUTE_TIME] = time;
                actionData[startPos + CONST.WAIT_TIME_HIGH] = (byte)(waitTime >> 8);
                actionData[startPos + CONST.WAIT_TIME_LOW] = (byte)(waitTime & 0xFF);
            }
            else
            {
                for (int i = 0; i < CONST.MAX_POSES_SIZE; i++)
                {
                    actionData[startPos + i] = emptyPose[i];
                }
            }
        }
    }

}

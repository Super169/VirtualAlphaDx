using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualAlphaDX
{
    public class ActionInfo
    {
        public int actionId;
        public char actionCode { get; set; }
        public bool isEmpty { get { return (poseCnt == 0); } }
        public PoseInfo[] pose { get; set; }
        public int lastPose;
        public int poseCnt { get; set; }
        public long totalTime { get; set; }
        public List<byte> relatedServo;
        public int relatedServoCnt { get { return (relatedServo == null ? 0 : relatedServo.Count); } }
        public string relatedServoStr { get; set; }

        public ActionInfo(int actionId)
        {
            this.InitObject(actionId);
        }

        private void InitObject(int actionId)
        {
            this.actionId = actionId;
            this.actionCode = (char)(((byte)'A') + actionId);
            this.pose = new PoseInfo[CONST.MAX_POSES];
            for (int pId = 0; pId < CONST.MAX_POSES; pId++)
            {
                pose[pId] = new PoseInfo(actionId, pId);
            }
        }

        public void Reset()
        {
            lastPose = 0;
            poseCnt = 0;
            totalTime = 0;
            relatedServo = new List<byte>();
            relatedServoStr = "";
            for (int poseId = 0; poseId < CONST.MAX_POSES; poseId++)
            {
                pose[poseId].Reset();
            }
        }

        public void CheckPoses()
        {
            lastPose = -1;
            poseCnt = 0;
            totalTime = 0;
            relatedServo = new List<byte>();
            relatedServoStr = "";
            for (int poseId = 0; poseId < CONST.MAX_POSES; poseId++)
            {
                if ((pose[poseId].waitTime == 0) && (pose[poseId].time == 0))
                {
                    break;
                }
                else
                {
                    totalTime += pose[poseId].waitTime;
                    lastPose = poseId;
                    for (byte id = 1; id <= 16; id++)
                    {
                        if (pose[poseId].angle[id] <= 240)
                        {
                            if (!relatedServo.Exists(x => x == id))
                            {
                                relatedServo.Add(id);
                            }
                        }
                    }
                }
            }
            poseCnt = lastPose + 1;
            if (relatedServo.Count > 0)
            {
                if (relatedServo.Count < 16)
                {
                    relatedServoStr = String.Join(",", relatedServo);
                }
                else
                {
                    relatedServoStr = "ALL Servos";
                }
            }
        }

        public bool ReadFromArray(byte[] actionData, int action)
        {
            this.Reset();
            if (this.actionId != action) return false;
            actionId = action;
            for (int poseId = 0; poseId < CONST.MAX_POSES; poseId++)
            {
                if (!pose[poseId].ReadFromArray(actionData, actionId, poseId))
                {
                    this.Reset();
                    return false;
                }
            }
            CheckPoses();
            return true;
        }

        public void WriteToArray(byte[] actionData)
        {
            for (int poseId = 0; poseId < CONST.MAX_POSES; poseId++)
            {
                pose[poseId].WriteToArray(actionData);
            }
        }
    }
}

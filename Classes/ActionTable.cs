using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualAlphaDX
{
    public class ActionTable
    {
        public ActionInfo[] action = new ActionInfo[CONST.MAX_ACTION];

        public ActionTable()
        {
            InitObject();
        }

        public void InitObject()
        {
            for (int actionId = 0; actionId < CONST.MAX_ACTION; actionId++)
            {
                action[actionId] = new ActionInfo(actionId);
            }
        }

        public void Reset()
        {
            for (int actionId = 0; actionId < CONST.MAX_ACTION; actionId++)
            {
                action[actionId].Reset();
            }
        }

        public bool ReadFromArray(byte[] actionData)
        {
            this.Reset();
            for (int actionId = 0; actionId < CONST.MAX_ACTION; actionId++)
            {
                if (!action[actionId].ReadFromArray(actionData, actionId))
                {
                    this.Reset();
                    return false;
                }
            }
            return true;
        }

        public void WriteToArray(byte[] actionData)
        {
            for (int actionId = 0; actionId < CONST.MAX_ACTION; actionId++)
            {
                action[actionId].WriteToArray(actionData);
            }
        }

    }
}

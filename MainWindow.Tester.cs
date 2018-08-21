using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace VirtualAlphaDX
{
    public partial class MainWindow : Window
    {

        private void btnGo_01_Click(object sender, RoutedEventArgs e)
        {
            Alpha.MoveTo(1, 90, 1000);
            Alpha.MoveTo(2, 0, 1000);
            Alpha.MoveTo(3, 90, 1000);
            Alpha.MoveTo(4, 90, 1000);
            Alpha.MoveTo(5, 180, 1000);
            Alpha.MoveTo(6, 90, 1000);
            Alpha.MoveTo(7, 90, 1000);
            Alpha.MoveTo(8, 60, 1000);
            Alpha.MoveTo(9, 76, 1000);
            Alpha.MoveTo(10, 110, 1000);
            Alpha.MoveTo(11, 90, 1000);
            Alpha.MoveTo(12, 90, 1000);
            Alpha.MoveTo(13, 120, 1000);
            Alpha.MoveTo(14, 104, 1000);
            Alpha.MoveTo(15, 70, 1000);
            Alpha.MoveTo(16, 90, 1000);
            Alpha.StartAnimation();
        }

        private void btnGo_02_Click(object sender, RoutedEventArgs e)
        {
            Alpha.MoveTo(1, 0, 5000);
            Alpha.MoveTo(2, 10, 5000);
            Alpha.MoveTo(3, 0, 5000);
            Alpha.MoveTo(4, 180, 5000);
            Alpha.MoveTo(5, 170, 5000);
            Alpha.MoveTo(6, 180, 5000);
            Alpha.MoveTo(7, 60, 1000);
            Alpha.MoveTo(8, 0, 1000);
            Alpha.MoveTo(9, 180, 1000);
            Alpha.MoveTo(10, 0, 1000);
            Alpha.MoveTo(11, 90, 2000);
            Alpha.MoveTo(12, 120, 2000);
            Alpha.MoveTo(13, 180, 2000);
            Alpha.MoveTo(14, 0, 2000);
            Alpha.MoveTo(15, 180, 2000);
            Alpha.MoveTo(16, 90, 2000);
            Alpha.StartAnimation();
        }

        private void btnGo_03_Click(object sender, RoutedEventArgs e)
        {
            Alpha.MoveTo(1, 90, 5000);
            Alpha.MoveTo(2, 90, 5000);
            Alpha.MoveTo(3, 180, 5000);
            Alpha.MoveTo(4, 90, 5000);
            Alpha.MoveTo(5, 90, 5000);
            Alpha.MoveTo(6, 0, 5000);
            Alpha.MoveTo(7, 0, 5000);
            Alpha.MoveTo(8, 0, 5000);
            Alpha.MoveTo(9, 135, 5000);
            Alpha.MoveTo(10, 75, 5000);
            Alpha.MoveTo(11, 180, 5000);
            Alpha.MoveTo(12, 180, 5000);
            Alpha.MoveTo(13, 180, 5000);
            Alpha.MoveTo(14, 45, 5000);
            Alpha.MoveTo(15, 105, 5000);
            Alpha.MoveTo(16, 0, 5000);
            Alpha.StartAnimation();

        }

        int[,] action = new int[,] { { 1, 90, 0, 90,90,180,90,90,60,76,110,90,90,120,104,70,90,40},
                                       { 2,90,90,180,90,90,0,0,0,135,75,180,180,180,45,105,0,80 },
                                       { 1,255,255,255,255,255,255,90,60,76,110,90,90,120,104,70,90,40 },
                                       { 1,255,255,255,255,255,255,0,255,255,255,255,255,255,255,255,255,40 },
                                       { 1,255,255,255,255,255,255,140,255,255,255,255,255,255,255,255,255,80 },
                                       { 1,255,255,255,255,255,255,255,255,255,255,255,180,255,255,255,255,40 },
                                       { 2,255,255,255,255,255,255,0,255,255,255,255,40,255,255,255,255,80 },
                                       { 2,255,255,255,255,255,255,140,255,255,255,255,180,255,255,255,255,80 },
                                       { 1,255,255,255,255,255,255,90,255,255,255,255,90,255,255,255,255,40 },
                                       { 1,90,174,87,90,160,111,255,255,255,255,255,255,255,255,255,255,40 },
                                       { 0,255,255,255,255,0,255,255,255,255,255,255,255,255,255,255,255,240 },
                                       { 1,255,67,180,255,255,255,255,255,255,255,255,255,255,255,255,255,40 },
                                       { 1,255,174,87,255,255,255,255,255,255,255,255,255,255,255,255,255,40 },
                                       { 1,255,67,180,255,255,255,255,255,255,255,255,255,255,255,255,255,40 },
                                       { 1,255,174,87,255,255,255,255,255,255,255,255,255,255,255,255,255,40 },
                                       { 1,255,67,180,255,255,255,255,255,255,255,255,255,255,255,255,255,40 },
                                       { 1,255,174,87,255,255,255,255,255,255,255,255,255,255,255,255,255,40 },
                                       { 1, 90, 0, 90,90,180,90,90,60,76,110,90,90,120,104,70,90,40},
                                       { 1,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,255,40 },
                                     };

        int actionPtr = 0;
        int ubtActionPtr = 0;
        Timer actionTimer;
        Timer ubtTimer;

        private void btnGo_04_Click(object sender, RoutedEventArgs e)
        {
            actionPtr = 0;
            actionTimer = new Timer();
            actionTimer.Interval = 1;
            actionTimer.Tick += new EventHandler(actionTimer_TickHandler);
            actionTimer.Start();
            UpdateInfo("Action Start");
        }

        private void actionTimer_TickHandler(object sender, EventArgs e)
        {
            int execTimeMs;
            actionTimer.Stop();
            if (actionPtr >= action.GetLength(0))
            {
                UpdateInfo("Action Completed");
                return;
            }
            execTimeMs = action[actionPtr, 17] * 1000 / 40;
            for (int id = 1; id < 17; id++)
            {
                if (action[actionPtr, id] <= 0xf0)
                {
                    Alpha.MoveTo(id, action[actionPtr, id], execTimeMs);
                }
            }
            Alpha.StartAnimation();
            int elapse = 1000 * action[actionPtr, 0];
            if (elapse < 1) elapse = 1;
            actionTimer.Interval = elapse;
            actionPtr++;
            actionTimer.Start();

        }

        int[,] UBTaction;

        private void btnGo_05_Click(object sender, RoutedEventArgs e)
        {
            int actionId = cboAction.SelectedIndex;
            ActionInfo ai = Alpha.actionTable.action[actionId];
            ai.CheckPoses();
            if (ai.poseCnt == 0) return;
            UBTaction = new int[ai.poseCnt, 18];
            for (int poseId = 0; poseId < ai.poseCnt; poseId++)
            {
                UBTaction[poseId, 0] = ai.pose[poseId].waitTime;
                for (int id = 1; id < 17; id++)
                {
                    UBTaction[poseId, id] = ai.pose[poseId].angle[id];
                }
                UBTaction[poseId, 17] = ai.pose[poseId].time;
            }

            ubtActionPtr = 0;
            ubtTimer = new Timer();
            ubtTimer.Interval = 1;
            ubtTimer.Tick += new EventHandler(ubtTimer_TickHandler);
            ubtTimer.Start();
            UpdateInfo("UBT Action Start");
        }

        private void ubtTimer_TickHandler(object sender, EventArgs e)
        {
            int execTimeMs;
            ubtTimer.Stop();
            if (ubtActionPtr >= UBTaction.GetLength(0))
            {
                UpdateInfo("UBT Action Completed");
                return;
            }
            execTimeMs = UBTaction[ubtActionPtr, 17];
            for (int id = 1; id < 17; id++)
            {
                if (UBTaction[ubtActionPtr, id] <= 0xf0)
                {
                    Alpha.MoveTo(id, UBTaction[ubtActionPtr, id], execTimeMs);
                }
            }
            Alpha.StartAnimation();
            int elapse = UBTaction[ubtActionPtr, 0];
            if (elapse < 1) elapse = 1;
            ubtTimer.Interval = elapse;
            ubtActionPtr++;
            ubtTimer.Start();
        }
    }
}

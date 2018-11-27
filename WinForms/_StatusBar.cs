using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sharpAHK;
using sharpAHK_Dev;
using AHKExpressions;
using TelerikExpressions;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Timers;

namespace sharpAHK_Dev
{
    public class _StatusBar
    {
        #region === StatusBar ===

        /// StatusBar Lib v1.1 | LucidMethod
        /// Place in Project Startup/Form Load 
        /// 
        ///     _StatusBar sb = new _StatusBar(); 
        ///      sb.CreateStatusBar(this);
        ///      

        sharpAHK._AHK ahk = new sharpAHK._AHK();

        public class SBGui
        {
            public static Form thisForm { get; set; }  // form statusbar is attached to
            public static StatusBar thisStatusBar { get; set; }  // form statusbar is attached to
            public static StatusBarPanel thisDatePanel { get; set; }  // form statusbar is attached to
            public static StatusBarPanel thisStatusPanel { get; set; }  // form statusbar is attached to

        }


        // Create New StatusBar
        public StatusBar mainStatusBar = new StatusBar();
        public StatusBarPanel statusPanel = new StatusBarPanel();
        public StatusBarPanel datetimePanel = new StatusBarPanel();

        //====== Status Bar =======
        public void CreateStatusBar(Form formName)
        {
            SBGui.thisForm = formName;

            // Set first panel properties and add to StatusBar
            statusPanel.BorderStyle = StatusBarPanelBorderStyle.Sunken;
            statusPanel.Text = formName.Text;
            statusPanel.ToolTipText = "Last Activity";
            statusPanel.AutoSize = StatusBarPanelAutoSize.Spring;
            mainStatusBar.Panels.Add(statusPanel);

            // Set second panel properties and add to StatusBar
            datetimePanel.BorderStyle = StatusBarPanelBorderStyle.Raised;
            datetimePanel.ToolTipText = "DateTime: " + System.DateTime.Today.ToString();
            datetimePanel.Text = System.DateTime.Today.ToLongDateString();
            datetimePanel.AutoSize = StatusBarPanelAutoSize.Contents;
            mainStatusBar.Panels.Add(datetimePanel);

            mainStatusBar.ShowPanels = true;
            // Add StatusBar to Form controls
            formName.Controls.Add(mainStatusBar);


            // click on panel action
            mainStatusBar.PanelClick += new System.Windows.Forms.StatusBarPanelClickEventHandler(StatusBar_PanelClick);

            SBGui.thisStatusBar = mainStatusBar;
            SBGui.thisDatePanel = datetimePanel;
            SBGui.thisStatusPanel = statusPanel;
        }

        public void StatusBar_PanelClick(object sender, System.Windows.Forms.StatusBarPanelClickEventArgs e)
        {
            // determine if left or right mouse click
            bool LeftClick = true; if (e.Button.ToString().ToUpper() == "RIGHT") { LeftClick = false; }

            switch (mainStatusBar.Panels.IndexOf(e.StatusBarPanel))
            {

                case 0:
                    if (LeftClick) { MessageBox.Show("You have clicked Panel One - LEFT Click"); }
                    if (!LeftClick) { MessageBox.Show("You have clicked Panel One - RIGHT Click"); }
                    break;
                case 1:
                    if (LeftClick) { MessageBox.Show("You have clicked Panel Two - LEFT Click"); }
                    if (!LeftClick) { MessageBox.Show("You have clicked Panel Two - RIGHT Click"); }
                    break;
            }
        }

        public void StatusBar(string UpdateText, int section = 1, string Timer = "", string JobName = "")   //update statusbar text
        {
            if (mainStatusBar == null) { return; }  // statusbar wasn't initiated in project startup

            // updates status bar text (from any thread)

            if (section == 2) { sbStopWatchJobName = JobName; } // store job name in sb while executing with timer

            if (Timer.ToUpper() == "START") { statusBarTimerThread("Start"); }

            if (Timer.ToUpper() == "STOP") { statusBarTimerThread("Stop"); }

            if (section == 1)
            {
                try
                {
                    MethodInvoker inv = delegate { if (SBGui.thisStatusPanel != null) { SBGui.thisStatusPanel.Text = UpdateText; } };
                    SBGui.thisForm.Invoke(inv);
                }
                catch
                {
                    if (SBGui.thisStatusPanel != null) { SBGui.thisStatusPanel.Text = UpdateText; }
                }
            }

            if (section == 2)
            {
                try
                {
                    MethodInvoker inv = delegate { if (SBGui.thisDatePanel != null) { SBGui.thisDatePanel.Text = UpdateText; } };
                    { };
                    SBGui.thisForm.Invoke(inv);
                }
                catch
                {
                    if (SBGui.thisDatePanel != null) { SBGui.thisDatePanel.Text = UpdateText; }
                }
            }
        }

        public void StatusBar_Icon(string ICOPath, int section = 1)
        {
            _Images img = new _Images();

            if (section == 1)
            {
                // show icon in bottom left statusbar (works)
                try { statusPanel.Icon = img.ImagePath_to_Icon(ICOPath, 32); }
                catch (Exception ex) { ahk.MsgBox(ex.Message); }
            }

            if (section == 2)
            {
                // show icon in bottom right statusbar (works)
                try { datetimePanel.Icon = img.ImagePath_to_Icon(ICOPath, 32); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }


        }

        public void statusBarTimer_()
        {
            Thread dirToDbThread = new Thread(() => statusBarTimerThread("Start"));
            dirToDbThread.Start();
        }

        public void statusBarUpdate(object sender, ElapsedEventArgs e)
        {
            string updateMsg = "RunTime: " + sbStopwatch.Elapsed.Seconds.ToString() + " Seconds";

            if (sbStopWatchJobName != "") { updateMsg = "JobName: " + sbStopWatchJobName + " | " + updateMsg; }

            StatusBar(updateMsg, 2, "", sbStopWatchJobName);
        }

        public string sbStopWatchJobName = ""; // displays job name while stopwatch display updates
        Stopwatch sbStopwatch = new Stopwatch();

        public void statusBarTimerThread(string StopStart = "Start")
        {
            if (StopStart.ToUpper() == "START")  // timer not created yet
            {
                _sBtimer = new System.Timers.Timer(1000); // Set up the timer for 3 seconds
                _sBtimer.Elapsed += new ElapsedEventHandler(statusBarUpdate);
                _sBtimer.Enabled = true; // Enable it
                sbStopwatch.Start();
                return;
            }

            if (StopStart.ToUpper() == "STOP")
            {
                _sBtimer.Enabled = false;
                sbStopwatch.Stop();

                StatusBar(sbStopwatch.Elapsed.Seconds.ToString() + " (DONE)", 2);
                return;
            }

            if (StopStart.ToUpper() == "RESUME")
            {
                _sBtimer.Enabled = true;
                sbStopwatch.Start();
                return;
            }

            if (StopStart.ToUpper() == "RESET")
            {
                _sBtimer.Enabled = true;
                sbStopwatch.Reset();
                return;
            }
        }

        static System.Timers.Timer _sBtimer; // using System.Timers

        #endregion

    }
}

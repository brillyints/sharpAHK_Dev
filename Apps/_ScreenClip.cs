using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.WinControls.UI;
using sharpAHK;
using AHKExpressions;
using static sharpAHK_Dev._Sites.SceneSource;
using System.IO;
using System.Diagnostics;
using TelerikExpressions;
using System.Timers;
using static sharpAHK_Dev._WinForms;

namespace sharpAHK_Dev
{
    public partial class _Apps
    {

        public class ScreenClip
        {
            #region === Startup ===

            private static _AHK ahk = new _AHK();
            private static _Database.SQLite sqlite = new _Database.SQLite();
            private static _Lists lst = new _Lists();
            private static _GridControl grid = new _GridControl();
            _WinForms frm = new _WinForms();

            #endregion

            #region === ScreenClip ===

            public string ScreenClipPath = "";
            public string ScreenClipNameSpace = "";

            public void Launch_ScreenClip_Click(object sender, EventArgs e)
            {
                Run();
            }

            /// <summary>
            /// Launch ScreenClip (Extracts Files If Not Extacted)
            /// </summary>
            /// <param name="Options"></param>
            public void Run(string Options = "")
            {
                if (ScreenClipNameSpace == "") { ScreenClipNameSpace = Options; }
                ScreenClipPath = Extract(ScreenClipNameSpace, true);
            }

            /// <summary>
            /// Extract ScreenClip From Embeded Resources
            /// </summary>
            /// <param name="NameSpace">NameSpace of Project Containing ScreenClip Files</param>
            /// <param name="Launch">Option to Launch SharpAHK After Extracting</param>
            /// <returns>Returns Path to SharpAHK EXE Local Path</returns>
            public string Extract(string NameSpace, bool Launch = false)
            {
                ScreenClipNameSpace = NameSpace;
                ScreenClipPath = frm.ExtractApp(AppList.ScreenClip, Launch, NameSpace);
                return ScreenClipPath;
            }

            /// <summary>
            /// Returns Path to Extracted ScreenClipINI 
            /// </summary>
            /// <param name="Open">Option to Open ScreneClip INI</param>
            /// <returns>Returns File Path to ScrenClip INI</returns>
            public string INI(bool Open = false)
            {
                string path = ahk.AppDir() + "\\Apps\\ScreenClip\\ScreenClip.ini";
                if (Open) { ahk.Run(path); }
                return path;
            }

            /// <summary>
            /// Force ScreenClip Process to Close
            /// </summary>
            public void Close()
            {
                foreach (var process in Process.GetProcessesByName("ScreenClip"))
                {
                    process.Kill();
                }
            }

            /// <summary>
            /// Returns # of ScreenClip Processes Running
            /// </summary>
            /// <returns></returns>
            public int Count()
            {
                int count = 0;
                foreach (var process in Process.GetProcessesByName("ScreenClip"))
                {
                    count++;
                }
                return count;
            }

            /// <summary>
            /// Returns True if ScreenClip Process Is Running
            /// </summary>
            /// <returns></returns>
            public bool Running()
            {
                int count = 0;
                foreach (var process in Process.GetProcessesByName("ScreenClip"))
                {
                    count++;
                }

                if (count == 0) { return false; }
                return true;
            }


            #endregion
        }


    }
}

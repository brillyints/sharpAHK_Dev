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

namespace sharpAHK_Dev
{
    public partial class _Apps
    {

        public class VisualStudio
        {

            // Returns list of all Visual Studio Window Titles
            public List<string> VS_Titles()
            {
                _Lists lst = new _Lists();
                List<string> MPCWins = new List<string>();

                List<Process> aList = lst.Process_List("devenv");  // list of processes before launching
                int i = 0;
                foreach (Process proc in aList)
                {
                    MPCWins.Add(proc.MainWindowTitle);
                }

                return MPCWins;
            }



        }


    }
}

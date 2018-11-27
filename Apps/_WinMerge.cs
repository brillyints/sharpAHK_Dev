using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sharpAHK;

namespace sharpAHK_Dev
{
    public partial class _Apps
    {

        public class WinMerge
        {
            /// <summary>
            /// Launch WinMerge With Files To Compare Loaded
            /// </summary>
            /// <param name="fileOne">First FilePath To Compare in WinMerge</param>
            /// <param name="fileTwo">Second FilePath to Compare in WinMerge</param>
            /// <param name="WinMergePath">Path to WinMergeU.exe (Comes with WinMerge Installation)</param>
            /// <returns>Returns True if WinMerge Found to Launch, False if WinMergeU.exe Not Found</returns>
            public bool Merge(string fileOne = "", string fileTwo = "", string WinMergePath = @"C:\Program Files(x86)\WinMerge\WinMergeU.exe")
            {
                _AHK ahk = new _AHK();

                if (File.Exists(WinMergePath))
                {
                    string runCmd = WinMergePath + " " + fileOne + " " + fileTwo;
                    ahk.Run(runCmd);
                    return true;
                }

                return false; // WinMergeU.exe Not Found
            }

        }

    }
}

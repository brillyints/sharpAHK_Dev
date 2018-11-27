using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using sharpAHK;
using Telerik.WinControls.UI;

namespace sharpAHK_Dev
{
    public partial class _Apps
    {

        public class WinRAR
        {
            public void RAR_EachFileInDir(RadProgressBar radProgress, string DirPath, bool NewThread = true, bool EpNumRARName = true, string Prefix = "")
            {
                _AHK ahk = new _AHK();

                if (NewThread)
                {
                    Thread imdbTVParseThread = new Thread(() => RAR_EachFileInDir(radProgress, DirPath, false, EpNumRARName, Prefix));
                    imdbTVParseThread.Start();
                }
                else
                {
                    _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();
                    _Lists lst = new _Lists();
                    _Parse prs = new _Parse();

                    string rar = @"C:\_Code\LucidProjects\ADBindex\ComPress\bin\Debug\Lib\rar.exe";

                    List<string> files = lst.FileList(DirPath);

                    pro.SetupProgressBar(radProgress, files.Count);

                    string RelativePath = "-ep";

                    string fileDir = ahk.FileDir(DirPath); int i = 0;
                    foreach (string file in files)
                    {
                        if (ahk.FileExt(file).ToUpper() == ".URL") { continue; }


                        // extract season/ep number from file name
                        string epNum = prs.SeasonEpNums(file);

                        i++; pro.UpdateProgress(radProgress, ahk.FileName(file) + " " + i + "/" + files.Count);
                        string newRAR = fileDir + "\\" + Prefix + epNum + ".rar";

                        // use the file name as the zip file name 
                        if (!EpNumRARName)
                        {
                            newRAR = fileDir + "\\" + ahk.FileNameNoExt(file) + ".rar";
                        }

                        if (File.Exists(newRAR)) { continue; }


                        string FIle = file.Replace(",", "`,");

                        string cmd = rar + " A -m0 " + RelativePath + " " + "\"" + newRAR + "\" \"" + FIle + "\"";
                        ahk.RunWait(cmd, "", "Hide");
                    }

                    ahk.MsgBox("Finished RARing " + files.Count + " Files");
                }


            }

        }

    }
}

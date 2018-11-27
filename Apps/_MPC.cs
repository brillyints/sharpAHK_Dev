using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sharpAHK;
using AHKExpressions;
using System.Diagnostics;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using System.IO;
using TelerikExpressions;
using System.Timers;

namespace sharpAHK_Dev
{
    public partial class _Apps
    {

        public class Mpc
        {
            private static _AHK ahk = new _AHK();
            private static _Lists lst = new _Lists();



            public struct MPCWin
            {
                public int WinPID { get; set; }
                public string WinTitle { get; set; }
                public string CurrentFilePath { get; set; }
                public string CurrentFileName { get; set; }
                public string CurrentFileDir { get; set; }
                public string ProcessName { get; set; }
                public IntPtr WinHwnd { get; set; }
                //public List<string> WinList { get; set; }
                public Process Player { get; set; }

            }

            public List<MPCWin> MPCWinList(bool Move4Qs = false)
            {
                _Lists lst = new _Lists();
                List<MPCWin> Wins = new List<MPCWin>();

                List<Process> aList = lst.Process_List("mpc-hc64");  // list of processes before launching

                foreach (Process proc in aList)
                {
                    MPCWin win = new MPCWin();
                    //MPCWins.Add(proc.MainWindowTitle);
                    win.WinTitle = proc.MainWindowTitle;
                    win.WinPID = proc.Id;
                    win.WinHwnd = proc.Handle;
                    win.ProcessName = proc.ProcessName;
                    Wins.Add(win);
                }


                aList = lst.Process_List("mpc-hc");  // list of processes before launching

                foreach (Process proc in aList)
                {
                    MPCWin win = new MPCWin();
                    //MPCWins.Add(proc.MainWindowTitle);
                    win.WinTitle = proc.MainWindowTitle;
                    win.WinPID = proc.Id;
                    win.WinHwnd = proc.Handle;
                    win.ProcessName = proc.ProcessName;
                    Wins.Add(win);
                }

                if (Move4Qs)
                {
                    int i = 1;
                    foreach (MPCWin win in Wins)
                    {
                        if (i == 1) { WinMovePID_Q1(win.WinPID); }
                        if (i == 2) { WinMovePID_Q2(win.WinPID); }
                        if (i == 3) { WinMovePID_Q3(win.WinPID); }
                        if (i == 4) { WinMovePID_Q4(win.WinPID); }
                        i++;
                    }
                }


                return Wins;
            }


            // list of all MPC WinTitles
            public List<string> MPC_WinTitleList()
            {

                List<string> MPCWins = new List<string>();

                List<Process> aList = lst.Process_List("mpc-hc64");  // list of processes before launching
                int i = 0;
                foreach (Process proc in aList)
                {
                    MPCWins.Add(proc.MainWindowTitle);
                }

                aList = lst.Process_List("mpc-hc");  // list of processes before launching
                foreach (Process proc in aList)
                {
                    MPCWins.Add(proc.MainWindowTitle);
                }

                return MPCWins;
            }


            #region === Player ===

            List<Process> captureD;

            /// <summary> </summary>
            /// <param name="TreeView TV"> </param>
            public void Capture_MPC(TreeView TV)
            {
                _TreeViewControl tv = new _TreeViewControl();
                _Lists lst = new _Lists();

                if (captureD == null) { captureD = new List<Process>(); }

                // unique list of captured mpc handles 
                List<Process> aList = lst.Process_List("mpc-hc64");  // list of processes before launching
                foreach (Process proc in aList)
                {
                    if (!Process_In_List(proc, captureD))
                    {
                        captureD.Add(proc);
                    }
                }


                TV.Nodes.Clear();
                foreach (Process t in captureD)
                {
                    tv.Node_Add(TV, t.MainWindowTitle, "", "", "Yellow");
                }

            }

            /// <summary> check to see if a process handle is in an existing list of processes</summary>
            /// <param name="Process p"> </param>
            /// <param name=" List<Process> List"> </param>
            public bool Process_In_List(Process p, List<Process> List)
            {
                if (List == null) { return false; }

                foreach (Process proc in List)
                {
                    if (p.MainWindowHandle == proc.MainWindowHandle) { return true; }
                }

                return false;
            }

            /// <summary> check to see if a process handle is in an existing list of processes</summary>
            /// <param name="TreeView TV"> </param>
            public void Show_Captured_Process(TreeView TV)
            {
                _TreeViewControl tv = new _TreeViewControl();

                if (captureD == null) { return; }

                foreach (Process proc in captureD)
                {
                    tv.Node_Add(TV, proc.MainWindowHandle.ToString());
                }

            }

            /// <summary> </summary>
            /// <param name="TreeView TV"> </param>
            public void Show_Active_Processes(TreeView TV)
            {
                _TreeViewControl tv = new _TreeViewControl();
                _Lists lst = new _Lists();

                TV.Nodes.Clear();

                List<Process> aList = lst.Process_List("mpc-hc64");  // list of processes before launching
                int i = 0;
                foreach (Process proc in aList)
                {
                    tv.Node_Add(TV, proc.MainWindowHandle.ToString(), "", "", "");
                    //if (i > 0) { MPC_Tree(TV, proc, false); i++; continue; }
                    //if (i == 0) { MPC_Tree(TV, proc, true); i++; continue; }
                }

            }

            /// <summary> </summary>
            /// <param name="Dir"> </param>
            public mpcWin PlayDir(string Dir = @"K:\")
            {
                mpcWin player = new mpcWin();

                string PlayList = Create_Playlist(Dir);

                _Lists lst = new _Lists();
                List<Process> pList = lst.Process_List("mpc-hc64");  // list of processes before launching

                //ahk.MsgBox("MPC Count = " + pList.Count.ToString()); 

                ahk.Run(PlayList);

                ahk.Sleep("1000");
                Process newProcess = null;
                List<Process> aList = lst.Process_List("mpc-hc64");  // list of processes after launching
                foreach (Process proc in aList)
                {
                    IntPtr handle = proc.Handle;

                    // compare previous mpc handle list and launched 
                    bool AlreadyCaptured = false;
                    foreach (Process p in pList)
                    {
                        if (p.Handle == handle) { AlreadyCaptured = true; continue; }
                    }

                    //if (!AlreadyCaptured)
                    newProcess = proc;

                }

                //if (newProcess != null)
                //{
                //    ahk.MsgBox("Captured Handle = " + newProcess.Handle);
                //}

                player.Player = newProcess;

                return player;
            }

            /// <summary>Create playlist for a directory</summary>
            /// <param name="Dir">Directory to Create Playlist from</param>
            /// <param name="PlayListName">Optional Name of Playlist - Default = Directory Name</param>
            /// <param name="OrderByDateMod">Option to Sort Playlist by Modified Date</param>
            /// <returns>Returns Path to New Playlist File</returns>
            public string Create_Playlist(string Dir, string PlayListName = "", bool Shuffle = true, bool OrderByDateMod = false)
            {
                if (PlayListName == "")
                {
                    string[] words = Dir.Split('\\'); // parse the dir path for the dir name to use as playlist name
                    foreach (string word in words) { PlayListName = word; }
                }

                // ensure playlist tempdir exists, clear previous playlist temp
                string PlayListDir = ahk.AppDir() + "\\PlayLists";
                string PlayList = PlayListDir + "\\" + PlayListName + ".mpcpl";
                ahk.FileCreateDir(PlayListDir);
                ahk.FileDelete(PlayList);
                ahk.FileAppend("MPCPLAYLIST", PlayList);


                // Optional Sort Methods: List all files in directory - sort by date modified - create playlist

                if (OrderByDateMod)
                {

                    DirectoryInfo di = new DirectoryInfo(Dir);
                    FileInfo[] logFiles = di.GetFiles("*.*");

                    DateCompareFileInfo dateCompareFileInfo = new DateCompareFileInfo();

                    Array.Sort(logFiles, dateCompareFileInfo);

                    int FCount = 1;
                    foreach (FileInfo filenfo in logFiles)  // loop through list of files and write file details to SQLite db
                    {
                        string file = filenfo.FullName;

                        //if (ImageList) { bool ImageCheck = IsImage(file); if (!ImageCheck) { continue; } }

                        ahk.FileAppend(FCount.ToString() + ",type,0", PlayList);
                        ahk.FileAppend(FCount.ToString() + ",filename," + file, PlayList);
                        FCount++;
                    }

                    return PlayList;
                }

                // list all files in directory to create mpc playlist

                string[] files = Directory.GetFiles(Dir, "*.*", SearchOption.AllDirectories);

                if (Shuffle) { files.Shuffle(); }

                int FileCount = 1;
                foreach (string file in files)  // loop through list of files and write file details to SQLite db
                {
                    //if (ImageList) { bool ImageCheck = IsImage(file); if (!ImageCheck) { continue; } }

                    ahk.FileAppend(FileCount.ToString() + ",type,0", PlayList);
                    ahk.FileAppend(FileCount.ToString() + ",filename," + file, PlayList);
                    FileCount++;
                }

                return PlayList;
            }

            /// <summary>create mpc playlist from a list of file paths</summary>
            /// <param name="Dir"> </param>
            /// <param name="PlayListName"> </param>
            /// <param name="ImageList"> </param>
            /// <param name="OrderByDateMod"> </param>
            public string MPCPlaylist_FromList(List<string> FilePathList, string PlayListName = "")
            {
                if (PlayListName == "")
                {
                    PlayListName = "TempPlayList";
                }

                string PlayListDir = ahk.AppDir() + "\\PlayLists";
                string PlayList = PlayListDir + "\\" + PlayListName + ".mpcpl";
                ahk.FileCreateDir(PlayListDir);
                ahk.FileDelete(PlayList);

                ahk.FileAppend("MPCPLAYLIST", PlayList);

                int FCount = 1;
                foreach (string FilePath in FilePathList)  // loop through list of files and write file details to SQLite db
                {
                    bool video = ahk.isVideo(FilePath);

                    if (video)
                    {
                        ahk.FileAppend(FCount.ToString() + ",type,0", PlayList);
                        ahk.FileAppend(FCount.ToString() + ",filename," + FilePath, PlayList);
                        FCount++;
                    }
                }

                return PlayList;
            }


            // create playlist from sqlite db search, returns list of files in playlist, option to run playlist
            public List<string> dbPlaylist(string DbFile, string DirName, bool RunPlaylist = true)
            {
                string PlayListDir = ahk.AppDir() + "\\PlayLists";
                ahk.FileCreateDir(PlayListDir);

                string PlayList = PlayListDir + "\\" + DirName + ".mpcpl";
                ahk.FileDelete(PlayList);

                List<string> PlaylistList = new List<string>();
                ahk.FileAppend("MPCPLAYLIST", PlayList);

                // list all files in directory - sort by date modified - create playlist

                _Database.SQLite sqlite = new _Database.SQLite();
                _Lists lst = new _Lists();

                string cmd = "Select [FilePath] from [FileIndex] where [DirName] = '" + DirName + "'";

                cmd = cmd + " Order by FileName";


                List<string> files = lst.SQLite_To_List(DbFile, cmd);

                int FCount = 1;
                foreach (string file in files)  // loop through list of files and write file details to SQLite db
                {
                    //if (ImageList) { bool ImageCheck = IsImage(file); if (!ImageCheck) { continue; } }

                    ahk.FileAppend(FCount.ToString() + ",type,0", PlayList);
                    ahk.FileAppend(FCount.ToString() + ",filename," + file, PlayList);
                    PlaylistList.Add(file);
                    FCount++;
                }

                if (RunPlaylist) { ahk.Run(PlayList); }

                //return PlayList; // return playlist file path
                return PlaylistList;
            }

            /// <summary> </summary>
            /// <param name="TreeView TV"> </param>
            /// <param name=" Process proc"> </param>
            /// <param name="ClearNodes"> </param>
            public void MPC_Tree(TreeView TV, Process proc, bool ClearNodes = true)
            {
                _TreeViewControl tv = new _TreeViewControl();
                if (ClearNodes) { TV.Nodes.Clear(); }

                TreeNode parent = new TreeNode();  // level 1
                parent.Text = ahk.FileName(proc.MainWindowTitle);
                parent.Tag = proc.MainWindowHandle.ToString();


                List<string> Options = new List<string> { "Playback", "PlayPause", "Stop", "Close", "Next", "Previous", "Forward", "Backward", "GoTo15" };
                Tree_SubMenu(parent, Options, proc.MainWindowHandle);

                Options = new List<string> { "Volume", "Volume Up", "Volume Down", "Mute" };
                Tree_SubMenu(parent, Options, proc.MainWindowHandle);

                Options = new List<string> { "WinPos", "Zoom 100", "Zoom 200", "Full Screen", "Q1", "Q2", "Q3", "Q4" };
                Tree_SubMenu(parent, Options, proc.MainWindowHandle);



                TV.Nodes.Add(parent);  // populate tree
                tv.Expand(TV);
            }

            /// <summary> </summary>
            /// <param name="TreeNode parent"> </param>
            /// <param name=" List<string> Options"> </param>
            /// <param name="object hWnd"> </param>
            public void Tree_SubMenu(TreeNode parent, List<string> Options, object hWnd = null)
            {
                int i = 0;
                TreeNode subMenu = new TreeNode();  // level 2
                foreach (string opt in Options)
                {
                    if (i == 0) // level 2 parent
                    {
                        subMenu.Text = opt;
                        if (hWnd != null) { subMenu.Tag = hWnd.ToString(); }
                        parent.Nodes.Add(subMenu);
                        i++;
                        continue;
                    }

                    if (i != 0) // level 3 children
                    {
                        TreeNode section = new TreeNode();  // level 2
                        section.Text = opt;
                        if (hWnd != null) { section.Tag = hWnd.ToString(); }
                        subMenu.Nodes.Add(section);
                    }
                }
            }

            /// <summary> </summary>
            /// <param name="TreeNode parent"> </param>
            /// <param name=" List<string> Options"> </param>
            /// <param name="Opt"> </param>
            public void PlayList_Tree_SubMenu(TreeNode parent, List<string> Options, string Opt = "")
            {
                int i = 0;
                TreeNode subMenu = new TreeNode();  // level 2
                foreach (string opt in Options)
                {
                    if (i == 0) // level 2 parent
                    {
                        subMenu.Text = ahk.FileName(opt);
                        subMenu.Tag = opt;
                        parent.Nodes.Add(subMenu);
                        i++;
                        continue;
                    }

                    if (i != 0) // level 3 children
                    {
                        TreeNode section = new TreeNode();  // level 2
                        section.Text = ahk.FileName(opt);
                        section.Tag = opt;
                        subMenu.Nodes.Add(section);
                    }
                }
            }


            /// <summary> all mpc players</summary>
            /// <param name="TreeView TV"> </param>
            /// <param name="ClearNodes"> </param>
            public void MPC_Trees(TreeView TV, bool ClearNodes = true)
            {
                _Lists lst = new _Lists();

                if (ClearNodes) { TV.Nodes.Clear(); }

                List<Process> aList = lst.Process_List("mpc-hc64");  // list of processes before launching
                int i = 0;
                foreach (Process pros in aList)
                {
                    MPC_Tree(TV, pros, false);
                }
            }

            /// <summary> all mpc players</summary>
            /// <param name="TreeView TV1"> </param>
            /// <param name=" TreeView TV2"> </param>
            /// <param name=" TreeView TV3"> </param>
            /// <param name=" TreeView TV4"> </param>
            public void MPC_4Trees(TreeView TV1, TreeView TV2, TreeView TV3, TreeView TV4)
            {
                _Lists lst = new _Lists();

                TV1.Nodes.Clear();
                TV2.Nodes.Clear();
                TV3.Nodes.Clear();
                TV4.Nodes.Clear();

                List<Process> aList = lst.Process_List("mpc-hc64");  // list of processes before launching
                int i = 0;
                foreach (Process pros in aList)
                {
                    if (i == 0) { MPC_Tree(TV1, pros, false); TV1.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(MPCtreeView_NodeMouseClick); WinMove_Q1(pros.MainWindowHandle); }
                    if (i == 1) { MPC_Tree(TV2, pros, false); TV2.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(MPCtreeView_NodeMouseClick); WinMove_Q2(pros.MainWindowHandle); }
                    if (i == 2) { MPC_Tree(TV3, pros, false); TV3.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(MPCtreeView_NodeMouseClick); WinMove_Q3(pros.MainWindowHandle); }
                    if (i == 3) { MPC_Tree(TV4, pros, false); TV4.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(MPCtreeView_NodeMouseClick); WinMove_Q4(pros.MainWindowHandle); }
                    i++;
                }

            }

            /// <summary> </summary>
            /// <param name="object sender"> </param>
            /// <param name=" TreeNodeMouseClickEventArgs e"> </param>
            private void MPCtreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
            {
                TreeView clickedTree = (TreeView)sender;
                TreeNode clickedNode = e.Node;

                IntPtr WinHandle = ahk.ToIntPtr(clickedNode.Tag);

                MPC_Actions(WinHandle, MPCAction(clickedNode.Text));
            }

            /// <summary> </summary>
            /// <param name="IntPtr WinHandle"> </param>
            /// <param name=" Action"> </param>
            /// <param name="Param"> </param>
            public string MPC_Actions(object WinHandle, Actions Action, string Param = "-1")
            {
                if (Action == Actions.PlayPause) { TogglePlayPause(WinHandle); }
                if (Action == Actions.Next) { Next(WinHandle); }
                if (Action == Actions.Previous) { Previous(WinHandle); }
                if (Action == Actions.Forward) { Jump_Forward(WinHandle, Param.ToInt()); }
                if (Action == Actions.Backward) { Jump_Backward(WinHandle, Param.ToInt()); }

                if (Action == Actions.Close) { ahk.WinClose("ahk_ID " + WinHandle); }

                if (Action == Actions.VolumeUp) { VolumeUp(WinHandle); }
                if (Action == Actions.VolumeDown) { VolumeDown(WinHandle); }
                if (Action == Actions.Mute) { Mute(WinHandle); }

                if (Action == Actions.Zoom100) { Zoom100(WinHandle); }
                if (Action == Actions.Zoom200) { Zoom200(WinHandle); }
                if (Action == Actions.FullScreen) { FullScreen(WinHandle); }

                if (Action == Actions.Q1) { WinMove_Q1(WinHandle, Param.ToInt()); }
                if (Action == Actions.Q2) { WinMove_Q2(WinHandle, Param.ToInt()); }
                if (Action == Actions.Q3) { WinMove_Q3(WinHandle, Param.ToInt()); }
                if (Action == Actions.Q4) { WinMove_Q4(WinHandle, Param.ToInt()); }

                if (Action == Actions.GoTo15) { GoToTime(WinHandle, "0500100"); }

                return "";
            }

            /// <summary>
            /// Convert Text to MPC Actions Object
            /// </summary>
            /// <param name="Action">MPC Action to Convert to Object</param>
            /// <returns></returns>
            public Actions MPCAction(string Action)
            {
                Action = Action.Replace(" ", "");
                if (Action.ToUpper() == "PLAYPAUSE") { return Actions.PlayPause; }
                if (Action.ToUpper() == "NEXT") { return Actions.Next; }
                if (Action.ToUpper() == "PREVIOUS") { return Actions.Previous; }
                if (Action.ToUpper() == "FORWARD") { return Actions.Forward; }
                if (Action.ToUpper() == "BACKWARD") { return Actions.Backward; }
                if (Action.ToUpper() == "CLOSE") { return Actions.Close; }
                if (Action.ToUpper() == "VOLUMEUP") { return Actions.VolumeUp; }
                if (Action.ToUpper() == "VOLUMEDOWN") { return Actions.VolumeDown; }
                if (Action.ToUpper() == "MUTE") { return Actions.Mute; }
                if (Action.ToUpper() == "ZOOM100") { return Actions.Zoom100; }
                if (Action.ToUpper() == "ZOOM200") { return Actions.Zoom200; }
                if (Action.ToUpper() == "FULLSCREEN") { return Actions.FullScreen; }
                if (Action.ToUpper() == "Q1") { return Actions.Q1; }
                if (Action.ToUpper() == "Q2") { return Actions.Q2; }
                if (Action.ToUpper() == "Q3") { return Actions.Q3; }
                if (Action.ToUpper() == "Q4") { return Actions.Q4; }
                if (Action.ToUpper() == "GOTO15") { return Actions.GoTo15; }
                return Actions.PlayPause;
            }

            /// <summary>
            /// MPC Player Actions
            /// </summary>
            public enum Actions
            {
                PlayPause,
                Next,
                Previous,
                Forward,
                Backward,
                Close,
                VolumeUp,
                VolumeDown,
                Mute,
                Zoom100,
                Zoom200,
                FullScreen,
                Q1,
                Q2,
                Q3,
                Q4,
                GoTo15
            }

            /// <summary> </summary>
            public void WinMoveQs()
            {
                _Lists lst = new _Lists();
                List<Process> aList = lst.Process_List("mpc-hc64");  // list of processes before launching
                int i = 0;
                foreach (Process pros in aList)
                {
                    if (i == 0) { WinMove_Q1(pros.MainWindowHandle); }
                    if (i == 1) { WinMove_Q2(pros.MainWindowHandle); }
                    if (i == 2) { WinMove_Q3(pros.MainWindowHandle); }
                    if (i == 3) { WinMove_Q4(pros.MainWindowHandle); }
                    i++;
                }

            }

            public void WinMovePID_Q1(int WinPID)
            {
                int screenW = System.Windows.Forms.SystemInformation.WorkingArea.Width;
                int screenH = System.Windows.Forms.SystemInformation.WorkingArea.Height;
                //ahk.MsgBox("Screen Width = " + screenW + "\nScreen Height = " + screenH);
                int W = screenW / 2;
                int H = screenH / 2;

                //string ahkID = "";
                //if (HWNd == null) { ahkID = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle.ToString(); }
                //else { ahkID = HWNd.ToString(); }

                string ahkID = "ahk_PID " + WinPID.ToString();
                ahk.WinMove(ahkID, "", "0", "0", W.ToString(), H.ToString());  // q1
            }
            public void WinMovePID_Q2(int WinPID)
            {
                int screenW = System.Windows.Forms.SystemInformation.WorkingArea.Width;
                int screenH = System.Windows.Forms.SystemInformation.WorkingArea.Height;
                //ahk.MsgBox("Screen Width = " + screenW + "\nScreen Height = " + screenH);
                int W = screenW / 2;
                int H = screenH / 2;

                //string ahkID = "";
                //if (HWNd == null) { ahkID = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle.ToString(); }
                //else { ahkID = HWNd.ToString(); }

                string ahkID = "ahk_PID " + WinPID.ToString();
                ahk.WinMove(ahkID, "", W.ToString(), "0", W.ToString(), H.ToString());  // q2
            }
            public void WinMovePID_Q3(int WinPID)
            {
                int screenW = System.Windows.Forms.SystemInformation.WorkingArea.Width;
                int screenH = System.Windows.Forms.SystemInformation.WorkingArea.Height;
                //ahk.MsgBox("Screen Width = " + screenW + "\nScreen Height = " + screenH);
                int W = screenW / 2;
                int H = screenH / 2;

                //string ahkID = "";
                //if (HWNd == null) { ahkID = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle.ToString(); }
                //else { ahkID = HWNd.ToString(); }

                string ahkID = "ahk_PID " + WinPID.ToString();
                ahk.WinMove(ahkID, "", "0", H.ToString(), W.ToString(), H.ToString());  // q3
            }
            public void WinMovePID_Q4(int WinPID)
            {
                int screenW = System.Windows.Forms.SystemInformation.WorkingArea.Width;
                int screenH = System.Windows.Forms.SystemInformation.WorkingArea.Height;
                //ahk.MsgBox("Screen Width = " + screenW + "\nScreen Height = " + screenH);
                int W = screenW / 2;
                int H = screenH / 2;

                //string ahkID = "";
                //if (HWNd == null) { ahkID = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle.ToString(); }
                //else { ahkID = HWNd.ToString(); }

                string ahkID = "ahk_PID " + WinPID.ToString();
                ahk.WinMove(ahkID, "", W.ToString(), H.ToString(), W.ToString(), H.ToString());  // q4
            }



            /// <summary> </summary>
            /// <param name="IntPtr HWNd"> </param>
            public void WinMove_Q1(object HWNd = null, int Monitor = -1)
            {
                int screenW = System.Windows.Forms.SystemInformation.WorkingArea.Width;
                int screenH = System.Windows.Forms.SystemInformation.WorkingArea.Height;
                //ahk.MsgBox("Screen Width = " + screenW + "\nScreen Height = " + screenH);

                string x = "0";
                string y = "0";
                int W = screenW / 2;
                int H = screenH / 2;

                bool WorkingArea = true;
                string ahkID = "";

                if (Monitor != -1)
                {
                    // get bounds of specific monitor number
                    string coords = MonitorCoords(Monitor, WorkingArea);
                    List<string> split = ahk.StringSplit_List(coords, "|");
                    string boundsX = split[0]; string boundsY = split[1]; screenW = split[2].ToInt(); screenH = split[3].ToInt();

                    x = boundsX;
                    y = boundsY;
                    W = screenW;
                    H = screenH;

                    if (HWNd == null) { ahkID = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle.ToString(); }
                    else { ahkID = HWNd.ToString(); }
                    if (!ahkID.Contains("ahk_ID")) { ahkID = "ahk_ID " + ahkID; }
                    ahk.WinMove(ahkID, "", x, y, W.ToString(), H.ToString());  // q1
                    return;
                }


                // move to q1 
                if (HWNd == null) { ahkID = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle.ToString(); }
                else { ahkID = HWNd.ToString(); }
                if (!ahkID.Contains("ahk_ID")) { ahkID = "ahk_ID " + ahkID; }
                ahk.WinMove(ahkID, "", x, y, W.ToString(), H.ToString());  // q1
            }

            /// <summary> </summary>
            /// <param name="IntPtr HWNd"> </param>
            public void WinMove_Q2(object HWNd, int Monitor = -1)
            {
                int screenW = System.Windows.Forms.SystemInformation.WorkingArea.Width;
                int screenH = System.Windows.Forms.SystemInformation.WorkingArea.Height;
                //ahk.WinMove(HWNd, screenW / 2, 0, screenW / 2, screenH / 2);  // q2

                string ahkID = HWNd.ToString();
                int W = screenW / 2;
                int H = screenH / 2;
                string x = W.ToString();
                string y = "0";
                bool WorkingArea = true;

                if (Monitor != -1)
                {
                    // get bounds of specific monitor number
                    string coords = MonitorCoords(Monitor, WorkingArea);
                    List<string> split = ahk.StringSplit_List(coords, "|");
                    string boundsX = split[0]; string boundsY = split[1]; screenW = split[2].ToInt(); screenH = split[3].ToInt();

                    x = boundsX;
                    y = boundsY;
                    W = screenW;
                    H = screenH;

                    if (HWNd == null) { ahkID = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle.ToString(); }
                    else { ahkID = HWNd.ToString(); }
                    if (!ahkID.Contains("ahk_ID")) { ahkID = "ahk_ID " + ahkID; }
                    ahk.WinMove(ahkID, "", x, y, W.ToString(), H.ToString());  // q1
                    return;
                }


                if (!ahkID.Contains("ahk_ID")) { ahkID = "ahk_ID " + ahkID; }
                ahk.WinMove(ahkID, "", x, y, W.ToString(), H.ToString());  // q2
            }

            /// <summary> </summary>
            /// <param name="IntPtr HWNd"> </param>
            public void WinMove_Q3(object HWNd, int Monitor = -1)
            {
                int screenW = System.Windows.Forms.SystemInformation.WorkingArea.Width;
                int screenH = System.Windows.Forms.SystemInformation.WorkingArea.Height;
                //ahk.WinMove(HWNd, 0, screenH / 2, screenW / 2, screenH / 2);  // q3

                string ahkID = HWNd.ToString();
                int W = screenW / 2;
                int H = screenH / 2;
                string x = "0";
                string y = H.ToString();
                bool WorkingArea = true;

                if (Monitor != -1)
                {
                    // get bounds of specific monitor number
                    string coords = MonitorCoords(Monitor, WorkingArea);
                    List<string> split = ahk.StringSplit_List(coords, "|");
                    string boundsX = split[0]; string boundsY = split[1]; screenW = split[2].ToInt(); screenH = split[3].ToInt();

                    x = boundsX;
                    y = boundsY;
                    W = screenW;
                    H = screenH;

                    if (HWNd == null) { ahkID = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle.ToString(); }
                    else { ahkID = HWNd.ToString(); }
                    if (!ahkID.Contains("ahk_ID")) { ahkID = "ahk_ID " + ahkID; }
                    ahk.WinMove(ahkID, "", x, y, W.ToString(), H.ToString());  // q1
                    return;
                }


                if (HWNd.ToString().ToUpper().Contains("PID"))
                {
                    ahk.WinMove("ahk_PID " + HWNd, "", x, y, W.ToString(), H.ToString());  // q3
                }
                else
                {
                    ahk.WinMove("ahk_ID " + HWNd, "", x, y, W.ToString(), H.ToString());  // q3
                }

            }

            /// <summary> </summary>
            /// <param name="IntPtr HWNd"> </param>
            public void WinMove_Q4(object HWNd, int Monitor = -1)
            {
                int screenW = System.Windows.Forms.SystemInformation.WorkingArea.Width;
                int screenH = System.Windows.Forms.SystemInformation.WorkingArea.Height;
                //ahk.WinMove(HWNd, screenW / 2, screenH / 2, screenW / 2, screenH / 2);  // q4

                int W = screenW / 2;
                int H = screenH / 2;
                string x = W.ToString();
                string y = H.ToString();
                bool WorkingArea = true;

                string ahkID = HWNd.ToString();
                if (Monitor != -1)
                {
                    // get bounds of specific monitor number
                    string coords = MonitorCoords(Monitor, WorkingArea);
                    List<string> split = ahk.StringSplit_List(coords, "|");
                    string boundsX = split[0]; string boundsY = split[1]; screenW = split[2].ToInt(); screenH = split[3].ToInt();

                    x = boundsX;
                    y = boundsY;
                    W = screenW;
                    H = screenH;

                    if (HWNd == null) { ahkID = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle.ToString(); }
                    else { ahkID = HWNd.ToString(); }
                    if (!ahkID.Contains("ahk_ID")) { ahkID = "ahk_ID " + ahkID; }
                    ahk.WinMove(ahkID, "", x, y, W, H); 
                    return;
                }


                if (HWNd.ToString().ToUpper().Contains("PID"))
                {
                    ahk.WinMove("ahk_PID " + HWNd, "", x, y, W.ToString(), H.ToString());  // q4
                }
                else
                {
                    ahk.WinMove("ahk_ID " + HWNd, "", x, y, W.ToString(), H.ToString());  // q4
                }
            }



            public string MonitorCoords(int MonNum = 0, bool WorkingArea = true)
            {
                Screen[] screens = Screen.AllScreens;
                for (int i = 0; i < screens.Length; i++)
                {
                    string bounds = screens[i].Bounds.ToString().Replace("{", "");
                    bounds = bounds.Replace("}", "");
                    bounds = bounds.Replace(",", "=");

                    List<string> split = ahk.StringSplit_List(bounds, "=");
                    string boundsX = split[1]; string boundsY = split[3]; string boundsW = split[5]; string boundsH = split[7];

                    bounds = screens[i].WorkingArea.ToString().Replace("{", "");
                    bounds = bounds.Replace("}", "");
                    bounds = bounds.Replace(",", "=");

                    split = ahk.StringSplit_List(bounds, "=");
                    string workingX = split[1]; string workingY = split[3]; string workingW = split[5]; string workingH = split[7];

                    string deviceName = screens[i].DeviceName.Replace(@"\\.\", "");

                    string Y = boundsY;
                    string X = boundsX;

                    if (MonNum == i)
                    {
                        if (WorkingArea) { return workingX + "|" + workingY + "|" + workingW + "|" + workingH; }
                        else { return boundsX + "|" + boundsY + "|" + boundsW + "|" + boundsH; }
                    }
                    //ahk.MsgBox("Bounds: " + screens[i].Bounds.ToString() + "\nDevice: " + screens[i].DeviceName + "\nWorkingArea: " + screens[i].WorkingArea.ToString());
                }

                return "";
            }







            public void GoTo15(object HWNd)
            {
                int screenW = System.Windows.Forms.SystemInformation.WorkingArea.Width;
                int screenH = System.Windows.Forms.SystemInformation.WorkingArea.Height;
                //ahk.WinMove(HWNd, screenW / 2, screenH / 2, screenW / 2, screenH / 2);  // q4

                int W = screenW / 2;
                int H = screenH / 2;

                if (HWNd.ToString().ToUpper().Contains("PID"))
                {
                    ahk.WinMove("ahk_PID " + HWNd, "", W.ToString(), H.ToString(), W.ToString(), H.ToString());  // q4
                }
                else
                {
                    ahk.WinMove("ahk_ID " + HWNd, "", W.ToString(), H.ToString(), W.ToString(), H.ToString());  // q4
                }
            }


            /// <summary> </summary>
            /// <param name="IntPtr HWNd"> </param>
            public sharpAHK.winInfo WinPos(IntPtr HWNd)
            {
                // based on the window handle/window title, return a rectangle object populated with top/left/w/h
                sharpAHK.winInfo WinCoords = ahk.WinGetPos("ahk_ID " + HWNd);
                return WinCoords;
            }

            #endregion

            /// <summary> </summary>
            /// <param name="hWnd"> </param>
            /// <param name="StepsForward"> </param>
            public void Jump_Forward(object hWnd, int StepsForward = 3)
            {
                // jump forward in the video (multiple times to jump further)

                int i = 0;
                do
                {
                    JumpForwardLarge(hWnd);
                    i++;
                } while (i < StepsForward);

                //UpdateTimerDisplay();
            }

            /// <summary> </summary>
            /// <param name="hWnd"> </param>
            /// <param name="StepsForward"> </param>
            public void Jump_Backward(object hWnd, int StepsForward = 3)
            {
                // jump forward in the video (multiple times to jump further)

                int i = 0;
                do
                {
                    JumpBackwardLarge(hWnd);
                    i++;
                } while (i < StepsForward);

                //UpdateTimerDisplay();
            }

            class DateCompareFileInfo : IComparer<FileInfo>
            {
                /// <summary>
                /// Compare the last dates of the File infos
                /// </summary>
                /// <param name="fi1">First FileInfo to check</param>
                /// <param name="fi2">Second FileInfo to check</param>
                /// <returns></returns>
                public int Compare(FileInfo fi1, FileInfo fi2)
                {
                    int result;
                    if (fi1.LastWriteTime == fi2.LastWriteTime)
                    {
                        result = 0;
                    }
                    else if (fi1.LastWriteTime < fi2.LastWriteTime)
                    {
                        result = 1;
                    }
                    else
                    {
                        result = -1;
                    }

                    return result;
                }
            }


            #region === MPC TimeCode ===

            public void TimeCode_OLD(IntPtr hWnd, out string CurrentTimeOut, out string TotalTimeOut)  // returns current timecode from MediaPlayerClassic player's statusbar
            {
                //Ex:
                //string CurrentTime = "";
                //string TotalTime = "";
                //mpc.TimeCode(hWnd, out CurrentTime, out TotalTime);

                //string TimeCode = CurrentTime + @" / " + TotalTime;
                //lblDisplay.Text = TimeCode;

                string SBText = ahk.WinGetText("ahk_ID" + hWnd);
                string CurrentTime = "";
                string TotalTime = "";

                // parse by new line
                string[] lines = SBText.Split(Environment.NewLine.ToCharArray());
                int Index = 1;
                string TimeLine = "";
                foreach (string line in lines)
                {
                    if (Index == 1) { TimeLine = line; break; }
                    Index++;
                }

                string[] TC = TimeLine.Split('/');
                Index = 1;
                foreach (string part in TC)
                {
                    if (Index == 1) { CurrentTime = part; }
                    if (Index == 2) { TotalTime = part; }
                    Index++;
                }

                //ahk.MsgBox("Current: " + CurrentTime);
                //ahk.MsgBox("TotalTime: " + TotalTime);

                CurrentTimeOut = CurrentTime;
                TotalTimeOut = TotalTime;
            }

            /// <summary>Read the text from a Windows Media Player, Parse the Time Code, and Return the Current/Total Time for Current Playing Video</summary>
            /// <param name="WinID">Window Title, IntPtr Window Handle, or AHK_ID Handle</param>
            public string TimeCode(object WinID, out string CurrentTime, out string TotalTime)  // returns current timecode from MediaPlayerClassic player's statusbar
            {
                //Ex:
                //string CurrentTime = "";
                //string TotalTime = "";
                //mpc.TimeCode(hWnd, out CurrentTime, out TotalTime);

                //string TimeCode = CurrentTime + @" / " + TotalTime;
                //lblDisplay.Text = TimeCode;

                string SBText = "";

                string VarType = WinID.GetType().ToString();  //determine what kind of variable was passed into function

                // user passed in the Window Title or AHK_ID as string
                if (VarType == "System.String")
                {
                    SBText = ahk.WinGetText(WinID.ToString());
                }

                // user passed in a IntPtr handle
                if (VarType == "System.IntPtr")
                {
                    SBText = ahk.WinGetText("ahk_ID" + WinID);
                }

                CurrentTime = "";
                TotalTime = "";
                //string TotalTime = "";

                // parse by new line
                string[] lines = SBText.Split(Environment.NewLine.ToCharArray());
                int Index = 1;
                string TimeLine = "";
                foreach (string line in lines)
                {
                    if (Index == 1) { TimeLine = line; break; }
                    Index++;
                }

                string[] TC = TimeLine.Split('/');
                Index = 1;
                foreach (string part in TC)
                {
                    if (Index == 1) { CurrentTime = part; }
                    if (Index == 2) { TotalTime = part; }
                    Index++;
                }

                //ahk.MsgBox("Current: " + CurrentTime);
                //ahk.MsgBox("TotalTime: " + TotalTime);

                string outTime = CurrentTime + " / " + TotalTime;

                return outTime;
            }

            public string TimeCodeDisplay(object WinID, TextBox currentTime, TextBox totalTime)  // returns current timecode from MediaPlayerClassic player's statusbar
            {
                string SBText = "";

                string VarType = WinID.GetType().ToString();  //determine what kind of variable was passed into function

                // user passed in the Window Title or AHK_ID as string
                if (VarType == "System.String")
                {
                    SBText = ahk.WinGetText(WinID.ToString());
                }

                // user passed in a IntPtr handle
                if (VarType == "System.IntPtr")
                {
                    SBText = ahk.WinGetText("ahk_ID" + WinID);
                }


                string CurrentTime = "";
                string TotalTime = "";

                // parse by new line
                string[] lines = SBText.Split(Environment.NewLine.ToCharArray());
                int Index = 1;
                string TimeLine = "";
                foreach (string line in lines)
                {
                    if (Index == 1) { TimeLine = line; break; }
                    Index++;
                }

                string[] TC = TimeLine.Split('/');
                Index = 1;
                foreach (string part in TC)
                {
                    if (Index == 1) { CurrentTime = part; }
                    if (Index == 2) { TotalTime = part; }
                    Index++;
                }

                //ahk.MsgBox("Current: " + CurrentTime);
                //ahk.MsgBox("TotalTime: " + TotalTime);

                //currentTime.Text = CurrentTime;
                //totalTime.Text = TotalTime;

                _WinForms frm = new _WinForms();

                frm.UpdateText(currentTime, CurrentTime);
                frm.UpdateText(totalTime, TotalTime);


                return CurrentTime + " / " + TotalTime;
            }


            /// <summary>
            /// Reads MPC StatusBar, Returns True if Status = Playing
            /// </summary>
            /// <param name="WinID"></param>
            /// <returns></returns>
            public bool IsPlaying(object WinID)
            {
                string SBText = "";

                string VarType = WinID.GetType().ToString();  //determine what kind of variable was passed into function

                // user passed in the Window Title or AHK_ID as string
                if (VarType == "System.String")
                {
                    SBText = ahk.WinGetText(WinID.ToString());
                }

                // user passed in a IntPtr handle
                if (VarType == "System.IntPtr")
                {
                    SBText = ahk.WinGetText("ahk_ID" + WinID);
                }

                // parse by new line
                string[] lines = SBText.Split(Environment.NewLine.ToCharArray());
                int Index = 1;
                foreach (string line in lines)
                {
                    if (line.Trim() == "") { continue; }

                    if (Index == 2)
                    {
                        if (line.Contains("Playing"))
                        {
                            return true;
                        }
                        else
                            return false;
                    }
                    Index++;
                }

                return false;
            }



            public void UpdateTimerThread(object sender, ElapsedEventArgs e)
            {
                TimeCodeDisplay(mpcID, TCcurrentTime, TCtotalTime);
            }

            public void ToggleTimeCodeDisplayTimer(object WinID, TextBox currentTime, TextBox totalTime, bool Start = true)
            {
                mpcID = WinID.ToString();
                TCcurrentTime = currentTime;
                TCtotalTime = totalTime;


                if (TimeCode_timer == null)  // timer not created yet
                {
                    // start timer to run every 3 seconds to log active window title
                    TimeCode_timer = new System.Timers.Timer(1000); // Set up the timer for 3 seconds
                    TimeCode_timer.Elapsed += new ElapsedEventHandler(UpdateTimerThread);
                    TimeCode_timer.Enabled = true; // Enable it
                    return;
                }

                if (TimeCode_timer.Enabled)
                {
                    TimeCode_timer.Enabled = false;
                    return;
                }

                if (!TimeCode_timer.Enabled)
                {
                    TimeCode_timer.Enabled = true; // Enable it
                    return;
                }
            }

            public void Restart_TimeCodeDisplayTimer(object WinID, TextBox currentTime, TextBox totalTime, bool Start = true)
            {
                mpcID = WinID.ToString();
                TCcurrentTime = currentTime;
                TCtotalTime = totalTime;

                // start timer to run every 3 seconds to log active window title
                TimeCode_timer = new System.Timers.Timer(1000); // Set up the timer for 3 seconds
                TimeCode_timer.Elapsed += new ElapsedEventHandler(UpdateTimerThread);
                TimeCode_timer.Enabled = true; // Enable it

                return;
            }

            static System.Timers.Timer _timer; // using System.Timers
            static System.Timers.Timer TimeCode_timer; // using System.Timers
            public TextBox TCcurrentTime;
            public TextBox TCtotalTime;
            public string mpcID;


            public int Parse_TimeCode_Hours(string TotalTime)  // returns the hours digit(s) for current video
            {
                //string TimeCode = CurrentTime + @" / " + TotalTime;

                string[] words = TotalTime.Split(':');
                int count = 0;
                string time1 = "0";
                string time2 = "0";
                string time3 = "0";
                foreach (string word in words)
                {
                    count++;
                    //ahk.MsgBox(word); 
                    if (count == 1) { time1 = word; }
                    if (count == 2) { time2 = word; }
                    if (count == 3) { time3 = word; }
                }

                string hours = "0";
                string min = "0";
                string sec = "0";

                if (count == 3)
                {
                    hours = time1;
                    min = time2;
                    sec = time3;
                }

                if (count == 2)
                {
                    hours = "0";
                    min = time1;
                    sec = time2;
                }

                //ahk.MsgBox("Hours: " + hours + " Min: " + min + " Sec: " + sec);
                int Hours = ahk.ToInt(hours);
                return Hours;
            }
            public int Parse_TimeCode_Minutes(string TotalTime)  // returns the minutes digit(s) for current video
            {
                //string TimeCode = CurrentTime + @" / " + TotalTime;

                string[] words = TotalTime.Split(':');
                int count = 0;
                string time1 = "0";
                string time2 = "0";
                string time3 = "0";
                foreach (string word in words)
                {
                    count++;
                    //ahk.MsgBox(word); 
                    if (count == 1) { time1 = word; }
                    if (count == 2) { time2 = word; }
                    if (count == 3) { time3 = word; }
                }

                string hours = "0";
                string min = "0";
                string sec = "0";

                if (count == 3)
                {
                    hours = time1;
                    min = time2;
                    sec = time3;
                }

                if (count == 2)
                {
                    hours = "0";
                    min = time1;
                    sec = time2;
                }

                //ahk.MsgBox("Hours: " + hours + " Min: " + min + " Sec: " + sec);
                int Min = ahk.ToInt(min);
                return Min;
            }
            public int Parse_TimeCode_Seconds(string TotalTime)  // returns the secnods digit(s) for current video
            {
                string[] words = TotalTime.Split(':');
                int count = 0;
                string time1 = "0";
                string time2 = "0";
                string time3 = "0";
                foreach (string word in words)
                {
                    count++;
                    //ahk.MsgBox(word); 
                    if (count == 1) { time1 = word; }
                    if (count == 2) { time2 = word; }
                    if (count == 3) { time3 = word; }
                }

                string hours = "0";
                string min = "0";
                string sec = "0";

                if (count == 3)
                {
                    hours = time1;
                    min = time2;
                    sec = time3;
                }

                if (count == 2)
                {
                    hours = "0";
                    min = time1;
                    sec = time2;
                }

                //ahk.MsgBox("Hours: " + hours + " Min: " + min + " Sec: " + sec);
                int Sec = ahk.ToInt(sec);
                return Sec;
            }


            #endregion


            #region === MediaPlayerClassic ===

            private string GetWinID(object hWnd)
            {
                string winID = hWnd.ToString();
                if (hWnd.ToString().ToUpper().Contains("AHK_PID")) { winID = hWnd.ToString(); }
                if (hWnd.ToString().ToUpper().Contains("AHK_ID")) { winID = hWnd.ToString(); }
                else { winID = "ahk_ID " + hWnd.ToString(); }
                return winID;
            }



            public enum mpcActions
            {
                WinMoveQ1, WinMoveQ2, WinMoveQ3, WinMoveQ4,
                Pause, Open, Mute, Play, Stop, Close, FullScreen, Zoom200, IncreaseRate, DecreaseRate, ResetRate,
                JumpForwardLarge, JumpForwardMedium, JumpForwardSmall, NextPlaylistItem, ViewNormal, AlwaysOnTop,
                QuickOpenFile, OpenDVD, OpenDevice, SaveAs, SaveImage, SaveImageAuto, LoadSubtitle,
                SaveSubtitle, Properties, Exit, TogglePlayPause, Framestep, FramestepBack, GoTo,
                AudioDelayPlus10ms, AudioDelayMinus10ms, JumpBackwardSmall, JumpBackwardMedium, JumpBackwardLarge,
                JumpForwardKeyframe, JumpBackwardKeyframe, Next, Previous, PreviousPlaylistItem, ToggleCaptionMenu,
                ToggleSeeker, ToggleControls, ToggleInformation, ToggleStatistics, ToggleStatus, ToggleSubresyncBar,
                TogglePlaylistBar, ToggleCaptureBar, ToggleShaderEditorBar, ViewMinimal, ViewCompact, FullscreenNoResChange,
                Zoom50, Zoom100, ZoomAutoFit, NextARPreset, VidFrmHalf, VidFrmNormal, VidFrmDouble, VidFrmStretch,
                VidFrmInside, VidFrmOutside, PnSReset, PnSIncSize, PnSIncWidth, PnSIncHeigh, PnSDecSize, PnSDecWidth,
                PnSDecHeight, PnSCenter, PnSLeft, PnSRight, PnSUp, PnSDown, PnSUpLeft, PnSUpRight, PnSDownLeft,
                PnSDownRight, PnSRotateXPlus, PnSRotateXMinus, PnSRotateYPlus, PnSRotateYMinus, PnSRotateZPlus,
                PnSRotateZMinus, VolumeUp, VolumeDown, VolumeBoostIncrease, VolumeBoostDecrease, VolumeBoostMin,
                VolumeBoostMax, DVDTitleMenu, DVDRootMenu, DVDSubtitleMenu, DVDAudioMenu, DVDAngleMenu, DVDChapterMenu,
                DVDMenuLeft, DVDMenuRight, DVDMenuUp, DVDMenuDown, DVDMenuActive, DVDMenuBack, DVDMenuLeave, BossKey,
                PlayerMenuShort, PlayerMenuLong, FiltersMenu, Options, NextAudio, PrevAudio, NextSubtitle, PrevSubtitle, ToggleSubtitle,
                NextAudioOGM, PrevAudioOGM, NextSubtitleOGM, PrevSubtitleOGM, NextAngleOGM, PrevAngleOGM, NextAudioDVD,
                PrevAudioDVD, NextSubtitleDVD, PrevSubtitleDVD, OnOffSubtitleDVD, SaveThumbnails, ReloadSubtitles, TearingTest,
                RemainingTime, TogglePixelShader, ToggleDirect3DFullscreen, GotoPrevSubtitle, GotoNextSubtitle,
                ShiftSubtitleLeft, ShiftSubtitleRight, DisplayStats, SubtitleDelayMinus, SubtitleDelayPlus, DVDMenuActivate
            }

            public void MpcAction(string hWnd, mpcActions action, string param = "")
            {
                //string hWnd = "";

                _AHK ahk = new _AHK();

                //if (action == mpcActions.WinMoveQ1) { WinMovePID_Q1(Win.PID.ToInt()); }
                //if (action == mpcActions.WinMoveQ2) { WinMovePID_Q2(Win.PID.ToInt()); }
                //if (action == mpcActions.WinMoveQ3) { WinMovePID_Q3(Win.PID.ToInt()); }
                //if (action == mpcActions.WinMoveQ4) { WinMovePID_Q4(Win.PID.ToInt()); }
                if (action == mpcActions.Pause) { Pause(hWnd); }
                if (action == mpcActions.Open) { Open(hWnd); }
                if (action == mpcActions.Mute) { Mute(hWnd); }
                if (action == mpcActions.Play) { Play(hWnd); }
                if (action == mpcActions.Stop) { Stop(hWnd); }
                if (action == mpcActions.Close) { Close(hWnd); }
                if (action == mpcActions.FullScreen) { FullScreen(hWnd); }
                if (action == mpcActions.Zoom200) { Zoom200(hWnd); }
                if (action == mpcActions.IncreaseRate) { IncreaseRate(hWnd); }
                if (action == mpcActions.DecreaseRate) { DecreaseRate(hWnd); }
                if (action == mpcActions.ResetRate) { ResetRate(hWnd); }
                if (action == mpcActions.JumpForwardLarge) { JumpForwardLarge(hWnd); }
                if (action == mpcActions.JumpForwardMedium) { JumpForwardMedium(hWnd); }
                if (action == mpcActions.JumpForwardSmall) { JumpForwardSmall(hWnd); }
                if (action == mpcActions.NextPlaylistItem) { NextPlaylistItem(hWnd); }
                if (action == mpcActions.ViewNormal) { ViewNormal(hWnd); }
                if (action == mpcActions.AlwaysOnTop) { AlwaysOnTop(hWnd); }
                if (action == mpcActions.QuickOpenFile) { QuickOpenFile(hWnd); }
                if (action == mpcActions.OpenDVD) { OpenDVD(hWnd); }
                if (action == mpcActions.OpenDevice) { OpenDevice(hWnd); }
                if (action == mpcActions.SaveAs) { SaveAs(hWnd); }
                if (action == mpcActions.SaveImage) { SaveImage(hWnd); }
                if (action == mpcActions.SaveImageAuto) { SaveImageAuto(hWnd); }
                if (action == mpcActions.LoadSubtitle) { LoadSubtitle(hWnd); }
                if (action == mpcActions.SaveSubtitle) { SaveSubtitle(hWnd); }
                if (action == mpcActions.Properties) { Properties(hWnd); }
                if (action == mpcActions.Exit) { Exit(hWnd); }
                if (action == mpcActions.TogglePlayPause) { TogglePlayPause(hWnd); }
                if (action == mpcActions.Framestep) { Framestep(hWnd); }
                if (action == mpcActions.FramestepBack) { FramestepBack(hWnd); }
                if (action == mpcActions.GoTo) { GoTo(hWnd); }
                if (action == mpcActions.AudioDelayPlus10ms) { AudioDelayPlus10ms(hWnd); }
                if (action == mpcActions.AudioDelayMinus10ms) { AudioDelayMinus10ms(hWnd); }
                if (action == mpcActions.JumpBackwardSmall) { JumpBackwardSmall(hWnd); }
                if (action == mpcActions.JumpBackwardMedium) { JumpBackwardMedium(hWnd); }
                if (action == mpcActions.JumpBackwardLarge) { JumpBackwardLarge(hWnd); }
                if (action == mpcActions.JumpForwardKeyframe) { JumpForwardKeyframe(hWnd); }
                if (action == mpcActions.JumpBackwardKeyframe) { JumpBackwardKeyframe(hWnd); }
                if (action == mpcActions.Next) { Next(hWnd); }
                if (action == mpcActions.Previous) { Previous(hWnd); }
                if (action == mpcActions.PreviousPlaylistItem) { PreviousPlaylistItem(hWnd); }
                if (action == mpcActions.ToggleCaptionMenu) { ToggleCaptionMenu(hWnd); }
                if (action == mpcActions.ToggleSeeker) { ToggleSeeker(hWnd); }
                if (action == mpcActions.ToggleControls) { ToggleControls(hWnd); }
                if (action == mpcActions.ToggleInformation) { ToggleInformation(hWnd); }
                if (action == mpcActions.ToggleStatistics) { ToggleStatistics(hWnd); }
                if (action == mpcActions.ToggleStatus) { ToggleStatus(hWnd); }
                if (action == mpcActions.ToggleSubresyncBar) { ToggleSubresyncBar(hWnd); }
                if (action == mpcActions.TogglePlaylistBar) { TogglePlaylistBar(hWnd); }
                if (action == mpcActions.ToggleCaptureBar) { ToggleCaptureBar(hWnd); }
                if (action == mpcActions.ToggleShaderEditorBar) { ToggleShaderEditorBar(hWnd); }
                if (action == mpcActions.ViewMinimal) { ViewMinimal(hWnd); }
                if (action == mpcActions.ViewCompact) { ViewCompact(hWnd); }
                if (action == mpcActions.FullscreenNoResChange) { FullscreenNoResChange(hWnd); }
                if (action == mpcActions.Zoom50) { Zoom50(hWnd); }
                if (action == mpcActions.Zoom100) { Zoom100(hWnd); }
                if (action == mpcActions.ZoomAutoFit) { ZoomAutoFit(hWnd); }
                if (action == mpcActions.NextARPreset) { NextARPreset(hWnd); }
                if (action == mpcActions.VidFrmHalf) { VidFrmHalf(hWnd); }
                if (action == mpcActions.VidFrmNormal) { VidFrmNormal(hWnd); }
                if (action == mpcActions.VidFrmDouble) { VidFrmDouble(hWnd); }
                if (action == mpcActions.VidFrmStretch) { VidFrmStretch(hWnd); }
                if (action == mpcActions.VidFrmInside) { VidFrmInside(hWnd); }
                if (action == mpcActions.VidFrmOutside) { VidFrmOutside(hWnd); }
                if (action == mpcActions.PnSReset) { PnSReset(hWnd); }
                if (action == mpcActions.PnSIncSize) { PnSIncSize(hWnd); }
                if (action == mpcActions.PnSIncWidth) { PnSIncWidth(hWnd); }
                if (action == mpcActions.PnSIncHeigh) { PnSIncHeigh(hWnd); }
                if (action == mpcActions.PnSDecSize) { PnSDecSize(hWnd); }
                if (action == mpcActions.PnSDecWidth) { PnSDecWidth(hWnd); }
                if (action == mpcActions.PnSDecHeight) { PnSDecHeight(hWnd); }
                if (action == mpcActions.PnSCenter) { PnSCenter(hWnd); }
                if (action == mpcActions.PnSLeft) { PnSLeft(hWnd); }
                if (action == mpcActions.PnSRight) { PnSRight(hWnd); }
                if (action == mpcActions.PnSUp) { PnSUp(hWnd); }
                if (action == mpcActions.PnSDown) { PnSDown(hWnd); }
                if (action == mpcActions.PnSUpLeft) { PnSUpLeft(hWnd); }
                if (action == mpcActions.PnSUpRight) { PnSUpRight(hWnd); }
                if (action == mpcActions.PnSDownLeft) { PnSDownLeft(hWnd); }
                if (action == mpcActions.PnSDownRight) { PnSDownRight(hWnd); }
                if (action == mpcActions.PnSRotateXPlus) { PnSRotateXPlus(hWnd); }
                if (action == mpcActions.PnSRotateXMinus) { PnSRotateXMinus(hWnd); }
                if (action == mpcActions.PnSRotateYPlus) { PnSRotateYPlus(hWnd); }
                if (action == mpcActions.PnSRotateYMinus) { PnSRotateYMinus(hWnd); }
                if (action == mpcActions.PnSRotateZPlus) { PnSRotateZPlus(hWnd); }
                if (action == mpcActions.PnSRotateZMinus) { PnSRotateZMinus(hWnd); }
                if (action == mpcActions.VolumeUp) { VolumeUp(hWnd); }
                if (action == mpcActions.VolumeDown) { VolumeDown(hWnd); }
                if (action == mpcActions.VolumeBoostIncrease) { VolumeBoostIncrease(hWnd); }
                if (action == mpcActions.VolumeBoostDecrease) { VolumeBoostDecrease(hWnd); }
                if (action == mpcActions.VolumeBoostMin) { VolumeBoostMin(hWnd); }
                if (action == mpcActions.VolumeBoostMax) { VolumeBoostMax(hWnd); }
                if (action == mpcActions.DVDTitleMenu) { DVDTitleMenu(hWnd); }
                if (action == mpcActions.DVDRootMenu) { DVDRootMenu(hWnd); }
                if (action == mpcActions.DVDSubtitleMenu) { DVDSubtitleMenu(hWnd); }
                if (action == mpcActions.DVDAudioMenu) { DVDAudioMenu(hWnd); }
                if (action == mpcActions.DVDAngleMenu) { DVDAngleMenu(hWnd); }
                if (action == mpcActions.DVDChapterMenu) { DVDChapterMenu(hWnd); }
                if (action == mpcActions.DVDMenuLeft) { DVDMenuLeft(hWnd); }
                if (action == mpcActions.DVDMenuRight) { DVDMenuRight(hWnd); }
                if (action == mpcActions.DVDMenuUp) { DVDMenuUp(hWnd); }
                if (action == mpcActions.DVDMenuDown) { DVDMenuDown(hWnd); }
                if (action == mpcActions.DVDMenuActivate) { DVDMenuActivate(hWnd); }
                if (action == mpcActions.DVDMenuBack) { DVDMenuBack(hWnd); }
                if (action == mpcActions.DVDMenuLeave) { DVDMenuLeave(hWnd); }
                if (action == mpcActions.BossKey) { BossKey(hWnd); }
                if (action == mpcActions.PlayerMenuShort) { PlayerMenuShort(hWnd); }
                if (action == mpcActions.PlayerMenuLong) { PlayerMenuLong(hWnd); }
                if (action == mpcActions.FiltersMenu) { FiltersMenu(hWnd); }
                if (action == mpcActions.Options) { Options(hWnd); }
                if (action == mpcActions.NextAudio) { NextAudio(hWnd); }
                if (action == mpcActions.PrevAudio) { PrevAudio(hWnd); }
                if (action == mpcActions.NextSubtitle) { NextSubtitle(hWnd); }
                if (action == mpcActions.PrevSubtitle) { PrevSubtitle(hWnd); }
                if (action == mpcActions.ToggleSubtitle) { ToggleSubtitle(hWnd); }
                if (action == mpcActions.NextAudioOGM) { NextAudioOGM(hWnd); }
                if (action == mpcActions.PrevAudioOGM) { PrevAudioOGM(hWnd); }
                if (action == mpcActions.NextSubtitleOGM) { NextSubtitleOGM(hWnd); }
                if (action == mpcActions.PrevSubtitleOGM) { PrevSubtitleOGM(hWnd); }
                if (action == mpcActions.NextAngleOGM) { NextAngleOGM(hWnd); }
                if (action == mpcActions.PrevAngleOGM) { PrevAngleOGM(hWnd); }
                if (action == mpcActions.NextAudioDVD) { NextAudioDVD(hWnd); }
                if (action == mpcActions.PrevAudioDVD) { PrevAudioDVD(hWnd); }
                if (action == mpcActions.NextSubtitleDVD) { NextSubtitleDVD(hWnd); }
                if (action == mpcActions.PrevSubtitleDVD) { PrevSubtitleDVD(hWnd); }
                if (action == mpcActions.OnOffSubtitleDVD) { OnOffSubtitleDVD(hWnd); }
                if (action == mpcActions.SaveThumbnails) { SaveThumbnails(hWnd); }
                if (action == mpcActions.ReloadSubtitles) { ReloadSubtitles(hWnd); }
                if (action == mpcActions.TearingTest) { TearingTest(hWnd); }
                if (action == mpcActions.RemainingTime) { RemainingTime(hWnd); }
                if (action == mpcActions.TogglePixelShader) { TogglePixelShader(hWnd); }
                if (action == mpcActions.ToggleDirect3DFullscreen) { ToggleDirect3DFullscreen(hWnd); }
                if (action == mpcActions.GotoPrevSubtitle) { GotoPrevSubtitle(hWnd); }
                if (action == mpcActions.GotoNextSubtitle) { GotoNextSubtitle(hWnd); }
                if (action == mpcActions.ShiftSubtitleLeft) { ShiftSubtitleLeft(hWnd); }
                if (action == mpcActions.ShiftSubtitleRight) { ShiftSubtitleRight(hWnd); }
                if (action == mpcActions.DisplayStats) { DisplayStats(hWnd); }
                if (action == mpcActions.SubtitleDelayMinus) { SubtitleDelayMinus(hWnd); }
                if (action == mpcActions.SubtitleDelayPlus) { SubtitleDelayPlus(hWnd); }


            }



            #region === Actions ===

            // jump to specific time in video
            public void GoToTime(object hWnd, string TimeString = "0500100")
            {
                GoTo(hWnd);
                ahk.WinWait("Go To... ahk_class #32770");
                ahk.WinActivate("Go To... ahk_class #32770");
                //ahk.Send(TimeString + "{Enter");
                ahk.Send(TimeString);
                ahk.ControlClick("button1", "Go To... ahk_class #32770");
            }

            /// <summary>Pause MPC Player</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void Pause(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(888);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>Open File in MPC</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void Open(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(800);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>Mute</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void Mute(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(909);
                IntPtr lParam = ahk.ToIntPtr(0);
                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>Play</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void Play(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(887);
                IntPtr lParam = ahk.ToIntPtr(0);
                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>Stop</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void Stop(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(888);
                IntPtr lParam = ahk.ToIntPtr(0);
                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>Close</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void Close(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(804);
                IntPtr lParam = ahk.ToIntPtr(0);
                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>FullScreen</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void FullScreen(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(830);
                IntPtr lParam = ahk.ToIntPtr(0);
                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>Zoom 200%</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void Zoom200(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(834);
                IntPtr lParam = ahk.ToIntPtr(0);
                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>Increase Rate</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void IncreaseRate(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(893);
                IntPtr lParam = ahk.ToIntPtr(0);
                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>Decrease Rate</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void DecreaseRate(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(894);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>Reset Rate</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void ResetRate(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(896);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>Jump Forward Large</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void JumpForwardLarge(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(904);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>Jump Forward Medium</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void JumpForwardMedium(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(902);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>Jump Forward Small</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void JumpForwardSmall(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(900);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>Next Playlist Item</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void NextPlaylistItem(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(919);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>View Normal</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void ViewNormal(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(829);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>AlwaysOnTop</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void AlwaysOnTop(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(884);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>QuickOpenFile</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void QuickOpenFile(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(968);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>OpenDVD</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void OpenDVD(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(801);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>OpenDevice</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void OpenDevice(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(802);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>SaveAs</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void SaveAs(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(805);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>SaveImage</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void SaveImage(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(806);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>SaveImageAuto</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void SaveImageAuto(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(807);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>LoadSubtitle</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void LoadSubtitle(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(809);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>SaveSubtitle</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void SaveSubtitle(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(810);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>Properties</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void Properties(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(814);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>Exit</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void Exit(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(816);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>TogglePlayPause</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void TogglePlayPause(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(889);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>Framestep</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void Framestep(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(891);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>FramestepBack</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void FramestepBack(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(892);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>GoTo</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void GoTo(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(893);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>AudioDelayPlus10ms</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void AudioDelayPlus10ms(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(905);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>AudioDelayMinus10ms</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void AudioDelayMinus10ms(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(906);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>JumpBackwardSmall</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void JumpBackwardSmall(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(899);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>JumpBackwardMedium</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void JumpBackwardMedium(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(901);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>JumpBackwardLarge</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void JumpBackwardLarge(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(903);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>JumpForwardKeyframe</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void JumpForwardKeyframe(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(898);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>JumpBackwardKeyframe</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void JumpBackwardKeyframe(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(897);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>Next</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void Next(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(920);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>Previous</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void Previous(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(921);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PreviousPlaylistItem</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PreviousPlaylistItem(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(918);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>ToggleCaptionMenu</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void ToggleCaptionMenu(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(817);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>ToggleSeeker</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void ToggleSeeker(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(818);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>ToggleControls</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void ToggleControls(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(819);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>ToggleInformation</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void ToggleInformation(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(820);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>ToggleStatistics</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void ToggleStatistics(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(821);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>ToggleStatus</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void ToggleStatus(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(822);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>ToggleSubresyncBar</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void ToggleSubresyncBar(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(823);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>TogglePlaylistBar</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void TogglePlaylistBar(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(824);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>ToggleCaptureBar</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void ToggleCaptureBar(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(825);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>ToggleShaderEditorBar</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void ToggleShaderEditorBar(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(826);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>ViewMinimal </summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void ViewMinimal(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(827);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>ViewCompact</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void ViewCompact(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(828);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>FullscreenNoResChange</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void FullscreenNoResChange(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(831);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>Zoom50</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void Zoom50(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(832);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>Zoom100</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void Zoom100(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(833);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>ZoomAutoFit</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void ZoomAutoFit(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(967);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>NextARPreset</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void NextARPreset(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(860);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>VidFrmHalf</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void VidFrmHalf(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(835);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>VidFrmNormal</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void VidFrmNormal(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(836);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>VidFrmDouble</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void VidFrmDouble(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(837);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>VidFrmStretch</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void VidFrmStretch(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(838);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>VidFrmInside</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void VidFrmInside(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(839);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>VidFrmOutside</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void VidFrmOutside(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(840);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PnSReset</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PnSReset(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(861);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PnSIncSize</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PnSIncSize(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(862);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PnSIncWidth</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PnSIncWidth(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(864);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PnSIncHeigh</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PnSIncHeigh(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(866);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PnSDecSize</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PnSDecSize(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(863);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PnSDecWidth</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PnSDecWidth(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(865);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PnSDecHeight</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PnSDecHeight(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(867);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PnSCenter</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PnSCenter(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(876);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PnSLeft</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PnSLeft(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(868);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PnSRight</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PnSRight(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(869);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PnSUp</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PnSUp(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(870);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PnSDown</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PnSDown(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(871);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PnSUpLeft</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PnSUpLeft(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(872);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PnSUpRight</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PnSUpRight(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(873);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PnSDownLeft</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PnSDownLeft(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(874);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PnSDownRight</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PnSDownRight(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(875);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PnSRotateXPlus</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PnSRotateXPlus(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(877);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PnSRotateXMinus</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PnSRotateXMinus(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(878);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PnSRotateYPlus</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PnSRotateYPlus(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(879);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PnSRotateYMinus</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PnSRotateYMinus(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(880);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PnSRotateZPlus</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PnSRotateZPlus(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(881);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PnSRotateZMinus</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PnSRotateZMinus(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(882);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>VolumeUp</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void VolumeUp(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(907);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>VolumeDown</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void VolumeDown(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(908);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>VolumeBoostIncrease</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void VolumeBoostIncrease(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(969);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>VolumeBoostDecrease</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void VolumeBoostDecrease(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(970);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>VolumeBoostMin</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void VolumeBoostMin(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(971);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>VolumeBoostMax</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void VolumeBoostMax(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(972);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>DVDTitleMenu</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void DVDTitleMenu(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(922);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>DVDRootMenu</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void DVDRootMenu(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(923);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>DVDSubtitleMenu</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void DVDSubtitleMenu(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(924);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>DVDAudioMenu</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void DVDAudioMenu(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(925);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>DVDAngleMenu</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void DVDAngleMenu(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(926);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>DVDChapterMenu</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void DVDChapterMenu(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(927);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>DVDMenuLeft</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void DVDMenuLeft(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(928);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>DVDMenuRight</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void DVDMenuRight(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(929);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>DVDMenuUp</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void DVDMenuUp(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(930);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>DVDMenuDown</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void DVDMenuDown(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(931);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>DVDMenuActivate</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void DVDMenuActivate(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(932);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>DVDMenuBack</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void DVDMenuBack(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(933);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>DVDMenuLeave</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void DVDMenuLeave(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(934);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>BossKey</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void BossKey(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(943);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PlayerMenuShort</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PlayerMenuShort(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(948);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PlayerMenuLong</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PlayerMenuLong(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(949);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>FiltersMenu</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void FiltersMenu(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(950);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>Options</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void Options(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(886);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>NextAudio</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void NextAudio(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(951);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PrevAudio</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PrevAudio(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(952);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>NextSubtitle</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void NextSubtitle(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(953);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PrevSubtitle</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PrevSubtitle(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(954);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>ToggleSubtitle</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void ToggleSubtitle(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(955);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>NextAudioOGM</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void NextAudioOGM(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(956);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PrevAudioOGM</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PrevAudioOGM(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(957);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>NextSubtitleOGM</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void NextSubtitleOGM(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(958);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PrevSubtitleOGM</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PrevSubtitleOGM(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(959);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>NextAngleOGM</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void NextAngleOGM(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(960);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PrevAngleOGM</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PrevAngleOGM(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(961);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>NextAudioDVD</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void NextAudioDVD(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(962);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PrevAudioDVD</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PrevAudioDVD(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(963);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>NextSubtitleDVD</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void NextSubtitleDVD(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(964);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>PrevSubtitleDVD</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void PrevSubtitleDVD(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(965);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>OnOffSubtitleDVD</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void OnOffSubtitleDVD(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(966);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>SaveThumbnails</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void SaveThumbnails(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(808);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>ReloadSubtitles</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void ReloadSubtitles(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(2302);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>TearingTest</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void TearingTest(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(32769);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>RemainingTime</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void RemainingTime(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(32778);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>TogglePixelShader</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void TogglePixelShader(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(32770);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>ToggleDirect3DFullscreen</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void ToggleDirect3DFullscreen(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(32779);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>GotoPrevSubtitle</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void GotoPrevSubtitle(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(32780);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>GotoNextSubtitle</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void GotoNextSubtitle(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(32781);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>ShiftSubtitleLeft</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void ShiftSubtitleLeft(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(32782);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>ShiftSubtitleRight</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void ShiftSubtitleRight(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(32783);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>DisplayStats</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void DisplayStats(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(32784);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>SubtitleDelayMinus</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void SubtitleDelayMinus(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(24000);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }
            /// <summary>SubtitleDelayPlus</summary>
            /// <param name="hWnd">Handle for MPC Player Window</param>
            public void SubtitleDelayPlus(object hWnd)
            {
                const uint NpadMsg = 0x111;
                IntPtr wParam = ahk.ToIntPtr(24001);
                IntPtr lParam = ahk.ToIntPtr(0);

                ahk.PostMessage(NpadMsg.ToString(), wParam.ToString(), lParam.ToString(), "", GetWinID(hWnd));
            }


            #endregion

            #endregion


            #region === MPC TELERIK ===

            /// <summary> </summary>
            /// <param name="TreeView TV"> </param>
            public void Capture_MPC(RadTreeView TV)
            {
                _TelerikLib.RadTree tv = new _TelerikLib.RadTree();
                _Lists lst = new _Lists();

                if (captureD == null) { captureD = new List<Process>(); }

                // unique list of captured mpc handles 
                List<Process> aList = lst.Process_List("mpc-hc64");  // list of processes before launching
                foreach (Process proc in aList)
                {
                    if (!Process_In_List(proc, captureD))
                    {
                        captureD.Add(proc);
                    }
                }

                TV.ClearTree();

                foreach (Process t in captureD)
                {
                    RadTreeNode mpcWin = new RadTreeNode(t.MainWindowTitle);
                    mpcWin.Tag = t.MainWindowHandle;
                    tv.AddNode(TV, mpcWin);
                }

            }

            /// <summary> </summary>
            /// <param name="TreeView TV"> </param>
            /// <param name=" Process proc"> </param>
            /// <param name="ClearNodes"> </param>
            public void MPC_RadTree(RadTreeView TV, Process proc, bool ClearNodes = true)
            {
                _TelerikLib.RadTree tv = new _TelerikLib.RadTree();
                if (ClearNodes) { TV.ClearTree(); }


                RadTreeNode parent = new RadTreeNode(ahk.FileName(proc.MainWindowTitle));
                //parent.Tag = "MPC:" + proc.MainWindowHandle;
                parent.Tag = proc.MainWindowHandle + "|" + proc.MainWindowTitle;
                tv.AddNode(TV, parent);

                // Add Options Node with List of SubNode Options
                List<string> Options = new List<string> { "Playback", "PlayPause", "Stop", "Close", "Next", "Previous", "Forward", "Backward" };
                RadTreeNode sParent = new RadTreeNode("Playback");
                sParent.Tag = "Options";
                tv.AddSubNode(parent, sParent, TV);
                tv.RadTree_SubNodeList(Options, sParent, TV, "MPC:" + proc.MainWindowHandle);

                // Add Volume Node with List of SubNode Options
                Options = new List<string> { "Volume", "Volume Up", "Volume Down", "Mute" };
                RadTreeNode sParents = new RadTreeNode("Volume");
                sParents.Tag = "Volume";
                tv.AddSubNode(parent, sParents, TV);
                tv.RadTree_SubNodeList(Options, sParents, TV, "MPC:" + proc.MainWindowHandle);


                // Add WinPos Node with List of SubNode Options
                Options = new List<string> { "WinPos", "Zoom 100", "Zoom 200", "Full Screen", "Q1", "Q2", "Q3", "Q4" };
                RadTreeNode SParents = new RadTreeNode("WinPos");
                sParents.Tag = "WinPos";
                tv.AddSubNode(parent, SParents, TV);
                tv.RadTree_SubNodeList(Options, SParents, TV, "MPC:" + proc.MainWindowHandle);


                //TV.Nodes.Add(parent);  // populate tree
                //tv.Expand(TV);
            }


            public RadTreeNode MPC_ControlNode(RadTreeView TV, Process proc, bool ClearNodes = true)
            {
                _TelerikLib.RadTree tv = new _TelerikLib.RadTree();
                if (ClearNodes) { TV.ClearTree(); }


                RadTreeNode parent = new RadTreeNode(ahk.FileName(proc.MainWindowTitle));
                //parent.Tag = "MPC:" + proc.MainWindowHandle;
                parent.Tag = proc.MainWindowHandle + "|" + proc.MainWindowTitle;
                tv.AddNode(TV, parent);

                // Add Options Node with List of SubNode Options
                List<string> Options = new List<string> { "Playback", "PlayPause", "Stop", "Close", "Next", "Previous", "Forward", "Backward" };
                RadTreeNode sParent = new RadTreeNode("Playback");
                sParent.Tag = "Options";
                tv.AddSubNode(parent, sParent, TV);
                //tv.RadTree_SubNodeList(Options, sParent, TV, "MPC:" + proc.MainWindowHandle);
                tv.RadTree_SubNodeList(Options, sParent, TV);

                // Add Volume Node with List of SubNode Options
                Options = new List<string> { "Volume", "Volume Up", "Volume Down", "Mute" };
                RadTreeNode sParents = new RadTreeNode("Volume");
                sParents.Tag = "Volume";
                tv.AddSubNode(parent, sParents, TV);
                //tv.RadTree_SubNodeList(Options, sParents, TV, "MPC:" + proc.MainWindowHandle);
                tv.RadTree_SubNodeList(Options, sParents, TV);


                // Add WinPos Node with List of SubNode Options
                Options = new List<string> { "WinPos", "Zoom 100", "Zoom 200", "Full Screen", "Q1", "Q2", "Q3", "Q4" };
                RadTreeNode SParents = new RadTreeNode("WinPos");
                sParents.Tag = "WinPos";
                tv.AddSubNode(parent, SParents, TV);
                tv.RadTree_SubNodeList(Options, SParents, TV, "MPC:" + proc.MainWindowHandle);


                //TV.Nodes.Add(parent);  // populate tree
                //tv.Expand(TV);

                return parent;
            }


            /// <summary> all mpc players</summary>
            /// <param name="TreeView TV"> </param>
            /// <param name="ClearNodes"> </param>
            public List<MPCWin> MPC_RadTrees(RadTreeView RadTree, bool ClearNodes = true)
            {
                _Lists lst = new _Lists();

                if (ClearNodes) { RadTree.ClearTree(); }

                // MPC Options Node
                RadTreeNode parent = new RadTreeNode();
                parent.Text = "MPC RadTree"; parent.Tag = "MPCRadTree";

                // List of MPC Options to add to MPC Options Node
                List<string> MPCParentOptions = new List<string>();
                MPCParentOptions.Add("ReCapture MPC");

                foreach (string opt in MPCParentOptions)
                {
                    RadTreeNode child = new RadTreeNode();
                    child.Text = opt; child.Tag = "MPCRadTree";
                    parent.Nodes.Add(child);
                }

                RadTree.Nodes.Add(parent);  // Add MPC Options Node to RadTree

                // List of All MPC Windows
                List<Process> aList = lst.Process_List("mpc-hc64");  // list of processes before launching
                int i = 0;
                foreach (Process pros in aList)
                {
                    // Add MPC Process to MPC RadTree
                    MPC_RadTree(RadTree, pros, false);
                }


                // populate list of mpcwin objects 
                List<MPCWin> Wins = new List<MPCWin>();
                aList = lst.Process_List("mpc-hc64");  // list of processes before launching
                foreach (Process proc in aList)
                {
                    MPCWin win = new MPCWin();
                    win.WinTitle = proc.MainWindowTitle;
                    win.WinPID = proc.Id;
                    win.WinHwnd = proc.Handle;
                    win.ProcessName = proc.ProcessName;
                    Wins.Add(win);
                }

                aList = lst.Process_List("mpc-hc");  // list of processes before launching
                foreach (Process proc in aList)
                {
                    MPCWin win = new MPCWin();
                    win.WinTitle = proc.MainWindowTitle;
                    win.WinPID = proc.Id;
                    win.WinHwnd = proc.Handle;
                    win.ProcessName = proc.ProcessName;
                    Wins.Add(win);
                }



                return Wins;
            }


            #endregion


            /// <summary>
            /// Returns List of MPC Window Objects
            /// </summary>
            /// <example>
            ///             List<_Apps.Mpc.mpcWin> mpcWins = mpc.MPCWinList();
            /// </example>
            /// <returns>Returns List of MPC Window Objects</returns>
            public List<mpcWin> MPCWinList()
            {
                List<mpcWin> wins = new List<mpcWin>();
                List<Process> aList = lst.Process_List("mpc-hc64");  // list of processes before launching
                int i = 0;
                foreach (Process pros in aList)
                {
                    mpcWin proc = Info(pros.MainWindowTitle);
                    wins.Add(proc);
                }
                return wins;
            }

            /// <summary>
            /// Returns Current Info from MPC Player
            /// </summary>
            /// <param name="WinTitle">Match Search by Current Window Title</param>
            /// <param name="PID">Match Search by Process ID</param>
            /// <returns></returns>
            public mpcWin Info(string WinTitle = "", string PID = "")
            {
                mpcWin mpcWin = new mpcWin();

                List<Process> aList = lst.Process_List("mpc-hc64");  // list of processes before launching
                int i = 0;
                foreach (Process proc in aList)
                {
                    string title = proc.MainWindowTitle;
                    //title = title.FileName();
                    if (WinTitle.Trim() == title.Trim() || PID == proc.Id.ToString()) ;
                    {

                        // based on the window handle/window title, return a rectangle object populated with top/left/w/h
                        sharpAHK.winInfo WinCoords = ahk.WinGetPos(WinTitle);
                        mpcWin.WinH = WinCoords.WinH;
                        mpcWin.WinW = WinCoords.WinW;
                        mpcWin.WinX = WinCoords.WinX;
                        mpcWin.WinY = WinCoords.WinY;

                        mpcWin.WinHwnd = proc.Handle;

                        mpcWin.FilePath = WinTitle.Trim();
                        mpcWin.FileName = ahk.FileName(mpcWin.FilePath);
                        mpcWin.PID = proc.Id.ToString();
                        mpcWin.ProcessName = proc.ProcessName;
                        mpcWin.IsMax = ahk.IsMaximized();
                        mpcWin.IsMin = ahk.IsMinimized();
                        mpcWin.Class = ahk.WinGetClass(mpcWin.FilePath);

                        //mpcWin.WinID

                        mpcWin.MPCProcess = proc;

                        mpcWin.IsPlaying = IsPlaying(mpcWin.FilePath);

                        string current = ""; string total = "";
                        mpcWin.TimeCode = TimeCode(mpcWin.FilePath, out current, out total);
                        mpcWin.CurrentTime = current;
                        mpcWin.TotalTime = total;

                        mpcWin.TotalTimeMin = Parse_TimeCode_Minutes(mpcWin.TotalTime);
                        mpcWin.TotalTimeSeconds = Parse_TimeCode_Seconds(mpcWin.TotalTime);
                        mpcWin.TotalTimeHours = Parse_TimeCode_Hours(mpcWin.TotalTime);

                        mpcWin.CurrentTimeMin = Parse_TimeCode_Minutes(mpcWin.CurrentTime);
                        mpcWin.CurrentTimeSeconds = Parse_TimeCode_Seconds(mpcWin.CurrentTime);
                        mpcWin.CurrentTimeHours = Parse_TimeCode_Hours(mpcWin.CurrentTime);

                    }
                }

                return mpcWin;
            }


            /// <summary>Stores Window Coordinates and Additional Details Returned from AHK Functions</summary>
            public class mpcWin
            {
                // ex:
                // winInfo win = new winInfo();   // define object reference

                public Process Player { get; set; }
                public Process MPCProcess { get; set; }
                public IntPtr WinHwnd { get; set; }
                public string FilePath { get; set; }
                public string FileName { get; set; }
                public string WinText { get; set; }
                public string WinID { get; set; }
                public string Class { get; set; }
                //public string MinMax { get; set; }
                public bool IsMin { get; set; }
                public bool IsMax { get; set; }

                public bool IsPlaying { get; set; }
                //public string Count { get; set; }

                public string TimeCode { get; set; }
                public string TotalTime { get; set; }
                public string CurrentTime { get; set; }

                public int CurrentTimeSeconds { get; set; }
                public int CurrentTimeMin { get; set; }
                public int CurrentTimeHours { get; set; }

                public int TotalTimeSeconds { get; set; }
                public int TotalTimeMin { get; set; }
                public int TotalTimeHours { get; set; }


                public int WinX { get; set; }
                public int WinY { get; set; }
                public int WinW { get; set; }
                public int WinH { get; set; }
                public string PID { get; set; }
                public string ProcessName { get; set; }

                public string mpcID { get; set; }
                public bool IsCurrentPlayer { get; set; }

            }



        }

    }
}

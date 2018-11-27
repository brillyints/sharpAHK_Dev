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
using My.JDownloader.Api.ApiObjects.AccountV2;
using My.JDownloader.Api;
using My.JDownloader.Api.ApiObjects.Devices;

namespace sharpAHK_Dev
{
    public partial class _Apps
    {

        // v2.0
        public class JDownloader
        {
            //#region === Startup ===

            //_AHK ahk = new _AHK();
            //_Parse prs = new _Parse();
            ////_Database.SQL sql = new _Database.SQL();
            ////_Database.SQLite sqlite = new _Database.SQLite();

            //#endregion

            //public string JDownloaderPath { get; set; }
            //public string FolderWatchPath { get; set; }
            //public string DownloadRootPath { get; set; }

            //public void Launch()
            //{
            //    if (JDownloaderPath == "" || JDownloaderPath == null) { JDownloaderPath = @"C:\Users\jason\AppData\Local\JDownloader v2.0\JDownloader2.exe"; }
            //    bool running = Running(); // check to see if already running
            //    if (!running)
            //    {
            //        ahk.Run(JDownloaderPath);
            //    }
            //}

            ///// <summary>
            ///// Check to See if Jownloader Process Is Running
            ///// </summary>
            ///// <returns></returns>
            //public bool Running()
            //{
            //    foreach (Process process in Process.GetProcessesByName("JDownloader2"))
            //    {
            //        return true;
            //    }

            //    return false;
            //}

            //#region === Interface Interaction ===

            ///// <summary>
            ///// JDownloader (Download Client)
            ///// Paste Contents of Clipboard onto Jdownloader Gui to Initiate Import
            ///// </summary>
            //public void Jdownloader_Paste()
            //{
            //    ahk.WinActivate("JDownloader 2");
            //    ahk.Sleep(1000);
            //    ahk.SendInput("^v"); // paste
            //}


            //#endregion

            //int RunningCount = 0; 
            //public void WriteDownloadLink(string LinkURL, string PackageName = "TV")
            //{
            //    if (DownloadRootPath == null) { ahk.MsgBox("Must Define JDownloader Download Root Path First"); return; }
            //    if (FolderWatchPath == null) { ahk.MsgBox("Must Define JDownloader FolderWatch Path First"); return; }

            //    string linkURL = LinkURL;
            //    string fileName = prs.LinkFileName(linkURL).Replace(".html", "");
            //    string showName = PackageName;


            //    string DLDir = DownloadRootPath;
            //    bool ExtractAfterDownload = true;
            //    bool Enabled = true;
            //    bool AutoStart = true;
            //    bool ForcedStart = true;
            //    bool AutoConfirm = true;

            //    //#this is a property file format. You can put all these entries in a jsonarray [{..},{...}] as well.
            //    //#save this as myFile.crawljob
            //    //#Start a new entry with anything but a comment (#...) or key=. An empty line would be ok as well
            //    //#\ must be double encoded! \ -> \\
            //    //->NEW ENTRY<-
            //    //   chunks = 6
            //    //#a comment
            //    //#   extractPasswords=["Password1","Password2"]
            //    //   text = https://rapidgator.net/file/3aef7553f517698dad4d6d0c5d505e17/The.China.Hustle.2017.WEB.x264-MUSiCANA.mkv
            //    //   filename = The.China.Hustle.2017.WEB.x264 - MUSiCANA.mkv
            //    //   packageName = ShowName
            //    //   extractAfterDownload = TRUE
            //    //   downloadFolder = D:\\Media\\ThisShowName
            //    //# priority=DEFAULT
            //    //# forcedStart=UNSET
            //    //# downloadPassword=null
            //    //# use only if text contains one single link
            //    //# overwritePackagizerEnabled=true
            //    //# setBeforePackagizerEnabled=true
            //    //# comment=null
            //    //# deepAnalyseEnabled=true
            //    //# addOfflineLink=true
            //    //   enabled = true
            //    //   autoStart = TRUE
            //    //   forcedStart = TRUE
            //    //   autoConfirm = TRUE


            //    string lines = "# JDownloader FolderWatch Link";
            //    lines = lines + "\n" + "->NEW ENTRY<-";
            //    lines = lines + "\n" + "chunks=6";
            //    lines = lines + "\n" + "text=" + linkURL;
            //    lines = lines + "\n" + "filename=" + fileName;
            //    lines = lines + "\n" + "packageName=" + showName;
            //    lines = lines + "\n" + "downloadFolder=" + DLDir + "\\" + showName;
            //    lines = lines + "\n" + "extractAfterDownload=" + ExtractAfterDownload.ToString();
            //    lines = lines + "\n" + "enabled=" + Enabled.ToString();
            //    lines = lines + "\n" + "autostart=" + AutoStart.ToString();
            //    lines = lines + "\n" + "forcedStart=" + ForcedStart.ToString();
            //    lines = lines + "\n" + "autoConfirm=" + AutoConfirm.ToString();
            //    lines = lines + "\n" + "";

            //    string savePath = FolderWatchPath + "\\" + RunningCount++ + ".crawljob";
            //    ahk.FileAppend(lines, savePath);
            //}


            ///// <summary>
            ///// Return # of Files in Download Dir Current UnFinished
            ///// </summary>
            ///// <returns></returns>
            //public int DownloadCount()
            //{
            //    if (DownloadRootPath == null) { ahk.MsgBox("Must Define JDownloader Download Root Path First"); return 0; }

            //    List<string> downloading = ahk.FileList(DownloadRootPath, "*.part", true, false, true);
            //    return downloading.Count;
            //}


            #region === Startup ===

            _AHK ahk = new _AHK();
            _Parse prs = new _Parse();
            //_Database.SQL sql = new _Database.SQL();
            //_Database.SQLite sqlite = new _Database.SQLite();

            static System.Timers.Timer _timer; // From System.Timers

            #endregion

            /// <summary>
            /// Parse RGLinks From Show into Size/Format, find working copy of average to highest quality available, start download in jdownloader and log in db
            /// </summary>
            /// <param name="show"></param>
            /// <param name="CheckLinks"></param>
            /// <returns></returns>
            public bool JDownloadShow(ssTv show, bool CheckLinks = true)
            {
                //SceneSourceLib ss = new SceneSourceLib();

                //ssTv epPost = show;

                //if (CheckLinks) { epPost = ss.ParseRGLinks(show, true); }

                bool Downloaded = false;

                //if (epPost.RapidGator_WEB != null) { Downloaded = true; WriteDownloadLink(epPost.RapidGator_WEB, epPost.ShowName); }
                //if (!Downloaded) { if (epPost.RapidGator_HDTV != null) { Downloaded = true; WriteDownloadLink(epPost.RapidGator_HDTV, epPost.ShowName); } }
                //if (!Downloaded) { if (epPost.RapidGator_720P != null) { Downloaded = true; WriteDownloadLink(epPost.RapidGator_720P, epPost.ShowName); } }
                //if (!Downloaded) { if (epPost.RapidGator_1080P != null) { Downloaded = true; WriteDownloadLink(epPost.RapidGator_1080P, epPost.ShowName); } }

                //if (Downloaded)
                //{
                //    // mark show as downloaded in database
                //    ssTv obj = epPost;
                //    obj.InCollection = true;
                //    ss.ssTv_UpdateInCollection(obj);
                //}

                return Downloaded;
            }

            public string JDownloaderPath { get; set; }
            public string FolderWatchPath { get; set; }
            public string DownloadRootPath { get; set; }

            public void Launch(string jdownloaderPath = "")
            {
                if (jdownloaderPath != "") { JDownloaderPath = jdownloaderPath; }

                if (JDownloaderPath == "" || JDownloaderPath == null) { JDownloaderPath = @"C:\Users\jason\AppData\Local\JDownloader v2.0\JDownloader2.exe"; }
                bool running = Running(); // check to see if already running
                if (!running)
                {
                    if (File.Exists(JDownloaderPath)) { ahk.Run(JDownloaderPath); }
                }
            }

            /// <summary>
            /// Check to See if Jownloader Process Is Running
            /// </summary>
            /// <returns></returns>
            public bool Running()
            {
                foreach (Process process in Process.GetProcessesByName("JDownloader2"))
                {
                    return true;
                }

                return false;
            }

            public void Activate()
            {
                ahk.WinActivate("ahk_class SunAwtFrame");
            }

            public void WinMaximize()
            {
                ahk.WinMaximize("ahk_class SunAwtFrame");
            }
            public void WinMinimize()
            {
                ahk.WinMinimize("ahk_class SunAwtFrame");
            }

            #region === Interface Interaction ===

            /// <summary>
            /// JDownloader (Download Client)
            /// Paste Contents of Clipboard onto Jdownloader Gui to Initiate Import
            /// </summary>
            public void Jdownloader_Paste()
            {
                ahk.WinActivate("JDownloader 2");
                ahk.Sleep(1000);
                ahk.SendInput("^v"); // paste
            }


            #endregion


            int LinkCounter = 0;
            public void WriteDownloadLink(string LinkURL, string PackageName = "TV")
            {
                if (DownloadRootPath == null) { ahk.MsgBox("Must Define JDownloader Download Root Path First"); return; }
                if (FolderWatchPath == null) { ahk.MsgBox("Must Define JDownloader FolderWatch Path First"); return; }

                string linkURL = LinkURL;
                string fileName = prs.LinkFileName(linkURL).Replace(".html", "");
                string showName = PackageName;


                string DLDir = DownloadRootPath;
                bool ExtractAfterDownload = true;
                bool Enabled = true;
                bool AutoStart = true;
                bool ForcedStart = true;
                bool AutoConfirm = true;

                //#this is a property file format. You can put all these entries in a jsonarray [{..},{...}] as well.
                //#save this as myFile.crawljob
                //#Start a new entry with anything but a comment (#...) or key=. An empty line would be ok as well
                //#\ must be double encoded! \ -> \\
                //->NEW ENTRY<-
                //   chunks = 6
                //#a comment
                //#   extractPasswords=["Password1","Password2"]
                //   text = https://rapidgator.net/file/3aef7553f517698dad4d6d0c5d505e17/The.China.Hustle.2017.WEB.x264-MUSiCANA.mkv
                //   filename = The.China.Hustle.2017.WEB.x264 - MUSiCANA.mkv
                //   packageName = ShowName
                //   extractAfterDownload = TRUE
                //   downloadFolder = D:\\Media\\ThisShowName
                //# priority=DEFAULT
                //# forcedStart=UNSET
                //# downloadPassword=null
                //# use only if text contains one single link
                //# overwritePackagizerEnabled=true
                //# setBeforePackagizerEnabled=true
                //# comment=null
                //# deepAnalyseEnabled=true
                //# addOfflineLink=true
                //   enabled = true
                //   autoStart = TRUE
                //   forcedStart = TRUE
                //   autoConfirm = TRUE


                string lines = "# JDownloader FolderWatch Link";
                lines = lines + "\n" + "->NEW ENTRY<-";
                lines = lines + "\n" + "chunks=6";
                lines = lines + "\n" + "text=" + linkURL;

                // add missing file format? 
                lines = lines + "\n" + "filename=" + fileName + ".mp4";

                lines = lines + "\n" + "packageName=" + showName;
                lines = lines + "\n" + "downloadFolder=" + DLDir + "\\" + showName;
                lines = lines + "\n" + "extractAfterDownload=" + ExtractAfterDownload.ToString();
                lines = lines + "\n" + "enabled=" + Enabled.ToString();
                lines = lines + "\n" + "autostart=" + AutoStart.ToString();
                lines = lines + "\n" + "forcedStart=" + ForcedStart.ToString();
                lines = lines + "\n" + "autoConfirm=" + AutoConfirm.ToString();
                lines = lines + "\n" + "";

                string saveName = fileName.Replace("|", "_");

                saveName = ahk.Decode(fileName);

                LinkCounter++;

                string savePath = FolderWatchPath + "\\" + LinkCounter + ".crawljob";
                ahk.FileAppend(lines, savePath);
            }


            public void WriteMovieLink(string LinkURL, string MovieTitle = "MovieTitle (Year)", string IMDbID = "", bool AutoStart = true)
            {
                if (LinkURL == null) { return; }
                string FileLink = LinkURL;
                if (FileLink.Contains("|")) { FileLink = ahk.StringSplit(FileLink, "|", 0); }

                string JDownloaderPath = @"C:\Users\jason\AppData\Local\JDownloader v2.0\JDownloader2.exe";
                string DownloadRoot = "D:\\Media\\Movies";

                MovieTitle = MovieTitle.Replace(":", "-");
                MovieTitle = MovieTitle.Replace("?", "-");

                ahk.FileCreateDir(DownloadRoot);

                string FolderWatchPath = "D:\\WinFiles\\JDownloaderFolderWatch";

                string fileName = prs.LinkFileName(FileLink).Replace(".html", "");

                bool ExtractAfterDownload = true;
                bool Enabled = true;
                bool ForcedStart = true;
                bool AutoConfirm = true;

                //#this is a property file format. You can put all these entries in a jsonarray [{..},{...}] as well.
                //#save this as myFile.crawljob
                //#Start a new entry with anything but a comment (#...) or key=. An empty line would be ok as well
                //#\ must be double encoded! \ -> \\
                //->NEW ENTRY<-
                //   chunks = 6
                //#a comment
                //#   extractPasswords=["Password1","Password2"]
                //   text = https://rapidgator.net/file/3aef7553f517698dad4d6d0c5d505e17/The.China.Hustle.2017.WEB.x264-MUSiCANA.mkv
                //   filename = The.China.Hustle.2017.WEB.x264 - MUSiCANA.mkv
                //   packageName = ShowName
                //   extractAfterDownload = TRUE
                //   downloadFolder = D:\\Media\\ThisShowName
                //# priority=DEFAULT
                //# forcedStart=UNSET
                //# downloadPassword=null
                //# use only if text contains one single link
                //# overwritePackagizerEnabled=true
                //# setBeforePackagizerEnabled=true
                //# comment=null
                //# deepAnalyseEnabled=true
                //# addOfflineLink=true
                //   enabled = true
                //   autoStart = TRUE
                //   forcedStart = TRUE
                //   autoConfirm = TRUE


                if (IMDbID != "")  // Create IMDb .URL File in Download Dir
                {
                    _Sites.OMDB omdb = new _Sites.OMDB();
                    omdb.OMDb_NewDirURL(IMDbID, DownloadRoot);
                }


                string lines = "# JDownloader FolderWatch Link";
                lines = lines + "\n" + "->NEW ENTRY<-";
                lines = lines + "\n" + "chunks=6";
                lines = lines + "\n" + "text=" + FileLink;

                // add missing file format? 
                //lines = lines + "\n" + "filename=" + fileName + ".mp4";
                lines = lines + "\n" + "filename=" + fileName;  // includes .rar

                lines = lines + "\n" + "packageName=" + MovieTitle;
                lines = lines + "\n" + "downloadFolder=" + DownloadRoot + "\\" + MovieTitle;
                lines = lines + "\n" + "extractAfterDownload=" + ExtractAfterDownload.ToString();
                lines = lines + "\n" + "enabled=" + Enabled.ToString();
                lines = lines + "\n" + "autostart=" + AutoStart.ToString();
                lines = lines + "\n" + "forcedStart=" + ForcedStart.ToString();
                lines = lines + "\n" + "autoConfirm=" + AutoConfirm.ToString();
                lines = lines + "\n" + "";

                string saveName = fileName.Replace("|", "_");

                saveName = ahk.Decode(fileName);

                LinkCounter++;

                string date = DateTime.Now.ToString("M_d_yyyy_mm_ss_fff");
                //string date = DateTime.Now.Date.ToShortDateString();

                string savePath = FolderWatchPath + "\\" + date + ".crawljob";
                if (File.Exists(savePath))
                {
                    ahk.Sleep(500);
                    savePath = DateTime.Now.ToString("M_d_yyyy_mm_ss_fff"); // try next current time
                }
                bool wroteLink = ahk.FileAppend(lines, savePath);
                if (!wroteLink)
                {
                    //ahk.Sleep(10)
                    //ahk.MsgBox("Writing Link File Failed, Retrying...");
                    //WriteMovieLink(LinkURL, MovieTitle, IMDbID, AutoStart);
                }
            }

            /// <summary>
            /// Return # of Files in Download Dir Current UnFinished
            /// </summary>
            /// <returns></returns>
            public int DownloadCount()
            {
                if (DownloadRootPath == null) { ahk.MsgBox("Must Define JDownloader Download Root Path First"); return 0; }

                // Compare Existing Downloading List, see if any finished downloaded and add to finished downloading list
                if (JDownloadingList != null)
                {
                    foreach (string file in JDownloadingList)
                    {
                        string actualFile = file.Replace(".part", "");

                        if (File.Exists(actualFile))
                        {
                            if (JDownloadedList == null) { JDownloadedList = new List<string>(); }
                            JDownloadedList.Add(actualFile);
                        }
                    }
                }

                List<string> downloading = ahk.FileList(DownloadRootPath, "*.part", true, false, true);

                JDownloadingList = downloading;

                return downloading.Count;
            }

            /// <summary>
            /// Returns List of Files Currently Downloading in JDownloader
            /// </summary>
            /// <returns></returns>
            public List<string> CurrentJDownloads()
            {
                List<string> downloads = new List<string>();
                if (DownloadRootPath == null) { ahk.MsgBox("Must Define JDownloader Download Root Path First"); return downloads; }

                // Compare Existing Downloading List, see if any finished downloaded and add to finished downloading list
                if (JDownloadingList != null)
                {
                    foreach (string file in JDownloadingList)
                    {
                        string actualFile = file.Replace(".part", "");

                        if (File.Exists(actualFile))
                        {
                            if (JDownloadedList == null) { JDownloadedList = new List<string>(); }
                            JDownloadedList.Add(actualFile);
                        }
                    }
                }


                List<string> downloading = ahk.FileList(DownloadRootPath, "*.part", true, false, true);

                foreach (string file in downloading)
                {
                    AddJDownloadListHist(file);
                }

                JDownloadingList = downloading;

                return JDownloadingList;
            }

            public static List<string> JDownloadingList { get; set; }
            public static List<string> JDownloadedList { get; set; }

            /// <summary>
            /// Add File to JDownloader History List (used while running)
            /// </summary>
            /// <param name="DLFile"></param>
            public void AddJDownloadListHist(string DLFile)
            {
                bool found = false;
                if (JDownloadingList != null)
                {
                    foreach (string item in JDownloadingList)
                    {
                        if (item == DLFile) { found = true; }
                    }
                }
                else
                {
                    JDownloadingList = new List<string>();
                }

                if (!found) { JDownloadingList.Add(DLFile); }
            }


            #region === JDownloader RadTree ===

            public void SetupJDownloaderTree(RadTreeView RadTree, RadStatusStrip Strip = null, bool StartDLWatchTimer = true)
            {
                if (Strip != null) { JDLStatusBar = Strip; }
                if (RadTree == null) { return; }

                JDownloadTree = RadTree;

                RadTree.SelectedNodeChanged += new Telerik.WinControls.UI.RadTreeView.RadTreeViewEventHandler(JDownloadTree_SelectedNodeChanged);
                RadTree.NodeMouseDoubleClick += new Telerik.WinControls.UI.RadTreeView.TreeViewEventHandler(JDownloadTree_NodeMouseDoubleClick);

                if (StartDLWatchTimer)
                {
                    _timer = new System.Timers.Timer(10000); // Set up the timer for 10 seconds
                    _timer.Elapsed += new ElapsedEventHandler(JDownloader_TimerEvent);
                    _timer.Enabled = true; // Enable it
                }
            }
            public void JDownloader_TimerEvent(object sender, ElapsedEventArgs e)
            {
                JdownloaderTree(JDownloadTree, true);
            }
            public void JdownloaderTree(RadTreeView RadTree, bool ClearTree = true)
            {
                _TelerikLib.RadTree tree = new _TelerikLib.RadTree();

                if (ClearTree) { RadTree.ClearTree(); }

                //==== Nodes for Files Currently Downloading ===============
                List<string> downloads = CurrentJDownloads();
                RadTreeNode downloading = new RadTreeNode();
                downloading.Text = "Downloading (" + downloads.Count + ")";
                downloading.Tag = "Downloading";
                tree.AddNodeIfMissing(RadTree, downloading);

                foreach (string file in downloads)
                {
                    RadTreeNode node = new RadTreeNode();
                    node.Text = file.FileName();
                    node.Tag = file;

                    tree.AddNodeIfMissing(RadTree, node, false, downloading);
                }

                if (JDownloadedList != null)
                {
                    //==== Nodes for Files Already Downloaded ===============
                    RadTreeNode downloaded = new RadTreeNode();
                    downloaded.Text = "Downloaded (" + JDownloadedList.Count + ")";
                    downloaded.Tag = "Downloaded";
                    tree.AddNodeIfMissing(RadTree, downloaded);

                    foreach (string file in JDownloadedList)
                    {
                        string actualFile = file.Replace(".part", "");

                        if (File.Exists(actualFile))
                        {
                            RadTreeNode node = new RadTreeNode();
                            node.Text = actualFile.FileName();
                            node.Tag = actualFile;
                            tree.AddNodeIfMissing(RadTree, node, false, downloaded);
                        }
                    }

                    downloaded.Expand();
                }

                //RadTree.ExpandAll();
                downloading.Expand();
            }
            private void JDownloadTree_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
            {
                _Sites.SceneSourceLib ss = new _Sites.SceneSourceLib();

                string Text = ""; string Tag = "";
                if (e.Node != null)
                {
                    Text = e.Node.Text;
                    try { Tag = e.Node.Tag.ToString(); } catch { }

                    if (Tag != "")
                    {
                        ssTv Current = CurrentShow;

                        Current.SeasonEp = ss.ShowEp_FromFilePath(Tag);

                        CurrentShow = Current;

                        sb(Tag);
                    }
                }

                sb(Text + " | " + Tag);
            }
            private void JDownloadTree_NodeMouseDoubleClick(object sender, RadTreeViewEventArgs e)
            {
                //SceneSourceLib ss = new SceneSourceLib();

                //string Text = "";

                //if (e.TreeView != null)
                //{
                //    if (e.TreeView.SelectedNode != null)
                //    {
                //        Text = e.TreeView.SelectedNode.Text;
                //        string Tag = "";

                //        try { Tag = e.TreeView.SelectedNode.Tag.ToString(); } catch { }

                //        if (Tag != "")
                //        {
                //            if (File.Exists(Tag)) { ahk.Run(Tag); }
                //        }
                //    }
                //}

            }

            public static ssTv CurrentShow { get; set; }

            #endregion

            public static RadStatusStrip JDLStatusBar { get; set; }

            public static RadTreeView JDownloadTree { get; set; }

            public void sb(string Text, string CodeGen = "")
            {
                if (JDLStatusBar != null)
                {
                    if (JDLStatusBar.Items.Count > 0)
                    {
                        (JDLStatusBar.Items[0] as RadLabelElement).Text = Text;
                    }
                    if (JDLStatusBar.Items.Count > 1)
                    {
                        (JDLStatusBar.Items[1] as RadLabelElement).Text = CodeGen;
                    }
                }
            }



            //=======================================
            // JDownloader API
            // 

            public void Credit()
            {
                string url = "https://github.com/Cr1TiKa7/My.Jdownloader.Api";
                ahk.Run(url);
            }

        }


        public class JDLib
        {
            JDownloaderHandler _jdownloaderHandler;
            DeviceHandler myDevice;
            _AHK ahk = new _AHK();
            _Lists lst = new _Lists();


            public static ListAccountResponseObject Lucid { get; set; }
            public static ListAccountResponseObject JLMobile { get; set; }

            public static ListAccountResponseObject Current { get; set; }

            /// <summary>
            /// Populate Account Info Objects
            /// </summary>
            /// <param name="RGLucid"></param>
            public void GetIDs(bool RGLucid = true)
            {
                List<ListAccountResponseObject> Hosts = ListAccounts();

                foreach (ListAccountResponseObject host in Hosts)
                {
                    if (host.Hostname.ToLower().Trim() == "rapidgator.net" && host.Username == "lucidmethod@gmail.com")
                    {
                        Lucid = host;
                        //ahk.MsgBox("HostName: " + host.Hostname + "\nUserName: " + host.Username + "\nValid: " + host.Valid + "\nTrafficLeft: " + host.TrafficLeft + "\nTrafficMax: " + host.TrafficMax + "\nEnabled: " + host.Enabled + "\nErrorString: " + host.ErrorString + "\nErrorType: " + host.ErrorType + "\nID: " + host.Uuid);
                    }
                    if (host.Hostname.ToLower().Trim() == "rapidgator.net" && host.Username == "jasonlangan.mobile@gmail.com")
                    {
                        JLMobile = host;
                        //ahk.MsgBox("HostName: " + host.Hostname + "\nUserName: " + host.Username + "\nValid: " + host.Valid + "\nTrafficLeft: " + host.TrafficLeft + "\nTrafficMax: " + host.TrafficMax + "\nEnabled: " + host.Enabled + "\nErrorString: " + host.ErrorString + "\nErrorType: " + host.ErrorType + "\nID: " + host.Uuid);
                    }
                }
            }


            // Lookup Lucid/JDL RG Account ID (used to update passwords)
            public long LucidID(bool RGLucid = true)
            {
                List<ListAccountResponseObject> Hosts = ListAccounts();

                foreach (ListAccountResponseObject host in Hosts)
                {
                    if (host.Hostname.ToLower().Trim() == "rapidgator.net" && host.Username == "lucidmethod@gmail.com")
                    {
                        if (RGLucid)
                        {
                            Lucid = host;
                            return host.Uuid;
                        }
                        //ahk.MsgBox("HostName: " + host.Hostname + "\nUserName: " + host.Username + "\nValid: " + host.Valid + "\nTrafficLeft: " + host.TrafficLeft + "\nTrafficMax: " + host.TrafficMax + "\nEnabled: " + host.Enabled + "\nErrorString: " + host.ErrorString + "\nErrorType: " + host.ErrorType + "\nID: " + host.Uuid);
                    }
                    if (host.Hostname.ToLower().Trim() == "rapidgator.net" && host.Username == "jasonlangan.mobile@gmail.com")
                    {
                        if (!RGLucid)
                        {
                            JLMobile = host;
                            return host.Uuid;
                        }
                        //ahk.MsgBox("HostName: " + host.Hostname + "\nUserName: " + host.Username + "\nValid: " + host.Valid + "\nTrafficLeft: " + host.TrafficLeft + "\nTrafficMax: " + host.TrafficMax + "\nEnabled: " + host.Enabled + "\nErrorString: " + host.ErrorString + "\nErrorType: " + host.ErrorType + "\nID: " + host.Uuid);
                    }
                }

                return 0;
            }


            /// <summary>
            /// Connect to JDownloader Application
            /// </summary>
            /// <returns></returns>
            public bool Connect()
            {
                if (_jdownloaderHandler != null && _jdownloaderHandler.IsConnected) { return true; }
                else
                {
                    _jdownloaderHandler = new JDownloaderHandler("jasonlangan.mobile@gmail.com", "Pac1kman", "jasonlangan.mobile@gmail.com");
                    return _jdownloaderHandler.IsConnected;
                }
            }


            /// <summary>
            /// Check to see if JDownloader Update is Available
            /// </summary>
            /// <returns></returns>
            public bool UpdateAvailable()
            {
                if (MyDevice() != null)
                {
                    return MyDevice().Update.IsUpdateAvailable();
                }
                return false;
            }

            /// <summary>
            /// Updates JDownloader Client (If Update Available) then Restarts JDownloader 
            /// </summary>
            public void UpdateRestart()
            {
                if (MyDevice() != null)
                {
                    MyDevice().Update.RestartAndUpdate();
                }
            }


            /// <summary>
            /// Returns Device Handler (Defaults to First Found (0))
            /// </summary>
            /// <returns></returns>
            public DeviceHandler MyDevice(int DeviceNum = 0)
            {
                DeviceHandler mydevice = null;

                if (Connect())
                {
                    List<DeviceObject> devices = _jdownloaderHandler.GetDevices(); // GetDeviceInstance(devices[0]);

                    if (devices.Count == 0)
                        return null;

                    mydevice = _jdownloaderHandler.GetDeviceHandler(devices[DeviceNum]);
                }

                return mydevice;
            }


            public bool AddAccount(string host = "mega.co.nz", string email = "test123", string pass = "test123")
            {
                if (MyDevice() != null)
                {
                    return MyDevice().AccountsV2.AddAccount(host, email, pass);
                }
                return false;
            }


            public bool UpdateAccount(string email = "test123", string pass = "test123", long accountID = 0)
            {
                if (MyDevice() != null)
                {
                    return MyDevice().AccountsV2.SetUsernameAndPassword(accountID, "lucidmethod@gmail.com", pass);
                }
                return false;
            }


            /// <summary>
            /// Returns List of All Premium Sites Available to use with JDownloader
            /// </summary>
            /// <returns></returns>
            public List<string> AllPremiumSites(bool Display = false)
            {
                List<string> premiumHosts = new List<string>();

                if (MyDevice() != null)
                {
                    premiumHosts = MyDevice().AccountsV2.ListPremiumHoster().ToList();

                    if (Display) { foreach (string premium in premiumHosts) { ahk.MsgBox(premium); } }
                }

                return premiumHosts;
            }


            /// <summary>
            /// List of HTTP (Basic Authentication) Entries with Details
            /// </summary>
            /// <param name="Display"></param>
            /// <returns></returns>
            public List<ListBasicAuthResponseObject> BasicAuthList(bool Display = false)
            {
                List<ListBasicAuthResponseObject> basicHosts = new List<ListBasicAuthResponseObject>();

                if (MyDevice() != null)
                {
                    basicHosts = MyDevice().AccountsV2.ListBasicAuth().ToList();

                    if (Display)
                    {
                        foreach (ListBasicAuthResponseObject host in basicHosts)
                        {
                            ahk.MsgBox("Created: " + host.Created + "\nEnabled: " + host.Enabled + "\nHostMask: " + host.Hostmask + "\nID: " + host.Id + "\nLast Validated: " + host.LastValidated + "\nPassword: " + host.Password + "\nType: " + host.Type + "\nUserName: " + host.Username);
                        }
                    }
                }

                return basicHosts;
            }


            public List<ListAccountResponseObject> ListAccounts(bool Display = false)
            {
                List<ListAccountResponseObject> Hosts = new List<ListAccountResponseObject>();

                if (MyDevice() != null)
                {
                    ListAccountRequestObject obj = new ListAccountRequestObject();
                    //obj.MaxResults = 10;
                    obj.Enabled = true;
                    //obj.StartAt = 0;
                    obj.TrafficLeft = true;
                    obj.TrafficMax = true;
                    obj.Username = true;
                    obj.Valid = true;
                    obj.ValidUntil = true;


                    Hosts = MyDevice().AccountsV2.ListAccounts(obj).ToList();

                    if (Display)
                    {
                        foreach (ListAccountResponseObject host in Hosts)
                        {
                            ahk.MsgBox("HostName: " + host.Hostname + "\nUserName: " + host.Username + "\nValid: " + host.Valid + "\nTrafficLeft: " + host.TrafficLeft + "\nTrafficMax: " + host.TrafficMax + "\nEnabled: " + host.Enabled + "\nErrorString: " + host.ErrorString + "\nErrorType: " + host.ErrorType + "\nID: " + host.Uuid);
                        }
                    }
                }

                return Hosts;
            }



        }


    }
}

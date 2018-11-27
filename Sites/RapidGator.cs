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
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Text.RegularExpressions;
using static sharpAHK_Dev._Web.DL_Accounts;
using System.Windows.Forms;
using System.Net;

namespace sharpAHK_Dev
{
    public partial class _Sites
    {
        // v2.2
        public class RapidGator
        {
            #region === Startup ===

            //_Database.goDaddy goDad = new _Database.goDaddy();
            sharpAHK._AHK ahk = new sharpAHK._AHK();
            _Database.SQL sql = new _Database.SQL();
            _Database.SQLite sqlite = new _Database.SQLite();
            _Lists lst = new _Lists();
            _Parse prs = new _Parse();
            _StatusBar sb = new _StatusBar();
            _Web web = new _Web();
            _TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            _Images img = new _Images();

            SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["epGuideDb"].ConnectionString);

            #endregion


            // Define RapidGator Passwords Used for Info Lookup

            /// <summary>
            /// Writes Passwords to Global Var used during RapidGator Actions. If Blank will Pull SQLite Values. If Provide will save to SQLite
            /// </summary>
            /// <param name="LucidPass"></param>
            /// <param name="JLPass"></param>
            public void SetPasswords(string LucidPass = "", string JLPass = "")
            {
                // Lookup Existing Passwords
                if (LucidPass == "") { LucidRapidPass = sqlite.Setting("RapidGator_Lucid"); }
                if (JLPass == "") { LucidRapidPass = sqlite.Setting("RapidGator_JL"); }

                // Apply Provided Passwords and Save
                if (LucidPass != "") { LucidRapidPass = LucidPass; sqlite.Setting("RapidGator_Lucid", LucidPass); }
                if (JLPass != "") { JLRapidPass = JLPass; sqlite.Setting("RapidGator_JL", JLPass); }
            }

            public static string JLRapidPass { get; set; }
            public static string LucidRapidPass { get; set; }

            public static RadProgressBar LinkCheckProgress { get; set; }
            public static RadTextBox PostURL { get; set; }

            public static PictureBox ShowImageBox { get; set; }
            public static Image ShowImage { get; set; }
            public static string ShowName { get; set; }



            // Latest RG Parse


            /// <summary>
            /// Download URL HTML, Extract List of RapidGator Links, Populate RadTree if Provided
            /// </summary>
            /// <param name="URL"></param>
            /// <param name="RadTree"></param>
            /// <param name="ClearTree"></param>
            /// <param name="CheckOnlineStatus"></param>
            /// <returns></returns>
            public List<string> RGLinks(string URL, RadTreeView RadTree = null, bool ClearTree = true, bool CheckOnlineStatus = true, RadTreeNode Parent = null)
            {
                if (RadTree != null)
                {
                    if (ClearTree) { RadTree.ClearTree(); }
                }


                string showName = "";
                if (Parent.Nodes != null) { showName = Parent.Text; }

                string html = ahk.Download_HTML(URL);

                List<string> links = Regex_RGLinks(html);


                // Extract / Display Images From Page

                List<string> images = JpgImageLinks(html);  // list of jpg images found on page
                string savename = "";
                if (images != null && images.Count > 0)
                {
                    int i = 0;
                    foreach (string image in images)
                    {
                        string imagedir = ahk.AppDir() + "\\ShowImages";
                        ahk.FileCreateDir(imagedir);
                        savename = imagedir + "\\" + showName + ".jpg";

                        if (i > 0) { savename = imagedir + "\\" + showName + "_" + i + ".jpg"; }
                        i++;

                        web.DownloadFile(image, savename, true);
                    }
                }

                if (savename != "")
                {
                    if (ShowImageBox != null)
                    {
                        if (ShowImageBox.InvokeRequired) { ShowImageBox.BeginInvoke((MethodInvoker)delegate () { ShowImageBox.Image = img.To_Image(savename); }); }
                        else { ShowImageBox.Image = img.To_Image(savename); }
                    }
                }


                if (RadTree != null)
                {
                    // add list to treeview (without checking online status)
                    if (!CheckOnlineStatus) { tree.ListRadTree(RadTree, links); }


                    if (CheckOnlineStatus)
                    {
                        foreach (string link in links)
                        {
                            bool online = CheckStatus(link);

                            RGInfo info = RapidGatorCheckStatus(link);

                            RadTreeNode node = new RadTreeNode();
                            node.Tag = link;


                            string epFileName = info.FileName;

                            //SceneSource SS = new SceneSource();
                            //ssTv show = SS.SceneSource_TV_LineParse(epFileName, SSCats.TVShows, false);

                            string SeasonEpNum = prs.SeasonEpNums("C:\\" + epFileName, false);

                            //ssTv show = new ssTv();

                            if (info.FileOnline)  // file is online
                            {
                                node.Text = info.FileName + " (" + info.FileSize + ")";

                                node.ForeColor = Color.White;
                                node.BackColor = Color.Green;
                                node.BackColor2 = Color.Green;
                                node.BackColor3 = Color.Green;
                                node.BackColor4 = Color.Green;
                            }
                            else  // file is offline
                            {
                                node.Text = info.FileName;

                                node.ForeColor = Color.White;
                                node.BackColor = Color.Red;
                                node.BackColor2 = Color.Red;
                                node.BackColor3 = Color.Red;
                                node.BackColor4 = Color.Red;

                                continue;
                            }

                            if (Parent == null) { tree.AddNode(RadTree, node); }
                            else
                            {
                                string EpNodeText = showName + "_" + SeasonEpNum;
                                bool EpNodeExists = tree.NodeExist(RadTree, EpNodeText, true);

                                if (!EpNodeExists)  // Add EpNum Node under ShowName if not Found
                                {
                                    RadTreeNode EpNumNode = tree.ReturnNode(RadTree, SeasonEpNum, EpNodeText);
                                    tree.AddSubNode(Parent, EpNumNode, RadTree);
                                    tree.AddSubNode(EpNumNode, node, RadTree);
                                }
                                if (EpNodeExists) // EpNode Found - Add to EpNode
                                {
                                    RadTreeNode EpNumNode = tree.ReturnNode(RadTree, SeasonEpNum, EpNodeText);
                                    tree.AddSubNode(EpNumNode, node, RadTree);
                                }
                            }

                        }

                    }

                }


                //ahk.MsgBox("Found " + links.Count + " Links");

                return links;
            }


            // sorts radtree display of links based on file size
            public List<string> RGLinks_SizeSort(string URL, RadTreeView RadTree = null, bool ClearTree = true)
            {
                //sb("Downloading / Parsing RapidGator Links (SizeSort)...");

                if (RadTree != null)
                {
                    if (ClearTree) { RadTree.ClearTree(); }
                }

                string html = ahk.Download_HTML(URL);

                List<string> links = Regex_RGLinks(html);

                List<string> sizeList = new List<string>();

                if (RadTree != null)
                {
                    List<RGInfo> linkInfos = new List<RGInfo>();

                    // build new list that include online status / size
                    foreach (string link in links)
                    {
                        RGInfo info = RapidGatorCheckStatus(link);
                        linkInfos.Add(info);

                        bool sizeAdded = lst.InList(sizeList, info.FileSize);
                        if (!sizeAdded) { sizeList.Add(info.FileSize); }
                    }


                    RadTreeNode ShowName = new RadTreeNode();
                    foreach (RGInfo info in linkInfos)
                    {
                        string dispName = info.FileName;
                        dispName = dispName.Replace(".", " ");
                        dispName = dispName.Replace("_", " ");

                        ShowName.Tag = dispName;
                        ShowName.Text = dispName;
                        tree.AddNode(RadTree, ShowName);
                        break;
                    }

                    foreach (string size in sizeList)
                    {
                        RadTreeNode node = new RadTreeNode();
                        node.Text = size;
                        node.Tag = size;
                        //tree.AddNode(radTree, node);
                        tree.AddSubNode(ShowName, node, RadTree);

                        foreach (RGInfo info in linkInfos)
                        {
                            if (info.FileSize == size && info.FileOnline)
                            {
                                RadTreeNode snode = new RadTreeNode();
                                snode.Tag = info.FileURL;
                                snode.Text = info.FileName + " (" + info.FileSize + ")";

                                snode.BackColor = Color.Green;
                                snode.BackColor2 = Color.Green;
                                snode.BackColor3 = Color.Green;
                                snode.BackColor4 = Color.Green;
                                snode.ForeColor = Color.White;

                                tree.AddSubNode(node, snode, RadTree);
                            }
                        }
                    }

                }


                //ahk.MsgBox("Found " + links.Count + " Links");

                return links;
            }


            // parse file link, return only file name from URL
            public string LinkFileName(string link)
            {
                return ahk.StringSplit(link, "/", 0, true);
            }

            public List<string> Regex_RGLinks(string HTML)
            {
                List<string> matches = new List<string>();

                string cmd = @"(http|ftp|https)://([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?";

                Regex ItemRegex = new Regex(cmd, RegexOptions.IgnoreCase);
                foreach (Match ItemMatch in ItemRegex.Matches(HTML))
                {
                    //Console.WriteLine(ItemMatch);
                    string val = ItemMatch.Value;

                    if (val.Contains("rapidgator.net/images/")) { continue; }
                    if (val.Contains("rapidgator.net/article/")) { continue; }

                    if (val.Contains("rapidgator.net")) { matches.Add(val); }
                    if (val.Contains("rg.net")) { matches.Add(val); }
                }


                return matches;
            }

            /// <summary>
            /// Regex Jpg Image Links from HTML to List
            /// </summary>
            /// <param name="HTML"></param>
            /// <returns></returns>
            public List<string> JpgImageLinks(string HTML)
            {
                List<string> matches = new List<string>();

                string cmd = @"(http|ftp|https)://([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?";

                Regex ItemRegex = new Regex(cmd, RegexOptions.IgnoreCase);
                foreach (Match ItemMatch in ItemRegex.Matches(HTML))
                {
                    //Console.WriteLine(ItemMatch);
                    string val = ItemMatch.Value;

                    if (val.Contains(".jpg") || val.Contains(".png"))
                    {
                        //ahk.MsgBox(val); 
                        matches.Add(val);
                    }
                }



                return matches;
            }



            /// <summary>
            /// Regex RapidGator Links + JPG/PNG Links
            /// </summary>
            /// <param name="HTML"></param>
            /// <returns></returns>
            public List<string> RGLinksImageLinks(string HTML)
            {
                List<string> matches = new List<string>();

                string cmd = @"(http|ftp|https)://([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?";

                Regex ItemRegex = new Regex(cmd, RegexOptions.IgnoreCase);
                foreach (Match ItemMatch in ItemRegex.Matches(HTML))
                {
                    string val = ItemMatch.Value;

                    if (val.Contains(".jpg") || val.Contains(".png"))
                    {
                        matches.Add(val);
                    }

                    if (val.Contains("rapidgator.net/images/")) { continue; }
                    if (val.Contains("rapidgator.net/article/")) { continue; }

                    if (val.Contains("rapidgator.net") || val.Contains("rg.net") || val.Contains("rg.to")) { matches.Add(val); }
                }

                return matches;
            }




            public struct RGInfo
            {
                public string FileURL { get; set; }
                public string FileName { get; set; }
                public string FileSize { get; set; }
                public bool FileOnline { get; set; }
            }

            private static string login { get; set; }
            private static string pass { get; set; }
            private static string sessionID { get; set; }
            private static string trafficLeft { get; set; }
            private string responseStatus { get; set; }


            public struct RGAccountStatus
            {
                public Sites SiteName { get; set; }
                public int AccountOrder { get; set; }
                public string AccountUserName { get; set; }
                public string AccountPass { get; set; }
                public bool AccountVerified { get; set; }
                public bool AccountEnabled { get; set; }
                public DateTime LastChecked { get; set; }
            }

            /// <summary>
            /// Checks to see if RapidGator Login Is Valid (Returns SessionID/Traffic Remaining/Response Status)
            /// </summary>
            /// <param name="login">RapidGator Login to Check</param>
            /// <param name="pass">RapidGator Password to Check</param>
            /// <param name="SessionID">Returned SessionID</param>
            /// <param name="TrafficLeft">Returned Traffic Remaining</param>
            /// <param name="ResponseStatus">Returned Response Status</param>
            /// <returns></returns>
            public bool RapidLoginCheck(string login, string pass, out string SessionID, out string TrafficLeft, out string ResponseStatus)
            {
                string URL = "https://rapidgator.net/api/user/login?username=" + login + "&password=" + pass;

                string html = web.DownloadHTML(URL);

                bool FoundSessionID = false;
                bool FoundTraffic = false;
                bool FoundStatus = false;

                SessionID = "";
                TrafficLeft = "";
                ResponseStatus = "";

                if (html.Trim() == "") { return false; }

                // "{\"response\":{\"session_id\":\"feps5ksc2s94k5grgno52q71g4\",\"expire_date\":1502504883,\"traffic_left\":\"2655431988666\"},\"response_status\":200,\"response_details\":null}"

                List<string> segs = ahk.StringSplit_List(html, "\"");


                foreach (string seg in segs)
                {
                    if (seg == ":") { continue; }
                    if (seg.Contains("session_id")) { FoundSessionID = true; continue; }
                    if (FoundSessionID) { SessionID = seg; FoundSessionID = false; }

                    if (seg.Contains("traffic_left")) { FoundTraffic = true; continue; }
                    if (FoundTraffic) { TrafficLeft = seg; FoundTraffic = false; }

                    if (seg.Contains("response_status")) { FoundStatus = true; continue; }
                    if (FoundStatus) { ResponseStatus = seg; FoundStatus = false; }
                }

                //ahk.MsgBox("SessionID = " + sessionID + "\nTraffic Left = " + trafficLeft + "\nResponse Status = " + responseStatus); 
                //sb.StatusBar("SessionID = " + SessionID + " | Traffic Left = " + trafficLeft + " | Response Status = " + responseStatus);

                if (SessionID != "") { return true; }
                else { return false; }
            }


            public bool UpdateRapidLogin(string Login, string Pass)
            {
                _Sites.DL_Accounts act = new _Sites.DL_Accounts();
                bool updated = act.RapidGatorAccount_Update(Login, Pass, 1, true);
                return updated;
            }


            public string GrabSessionID(string login = "lucidmethod@gmail.com")
            {
                // first call with l/p - returns session ID used in other calls
                //login = "lucidmethod@gmail.com";
                //pass = "zStj6X";
                //LucidRapidPass = "PIYi2R";
                //if (login == "lucidmethod@gmail.com") { pass = LucidRapidPass; }
                //if (login == "jasonlangan.mobile@gmail.com") { pass = JLRapidPass; }

                // pull latest pass from database based on login name
                _Sites.DL_Accounts act = new _Sites.DL_Accounts();
                pass = act.RapidGatorPass(login);
                LucidRapidPass = pass;

                //if (LucidRapidPass == null)
                //{
                //    SetPasswords(); 
                //}

                if (LucidRapidPass == null) { ahk.MsgBox("RapidGator Pass Null"); return ""; }

                string URL = "https://rapidgator.net/api/user/login?username=" + login + "&password=" + pass;

                string html = web.DownloadHTML(URL);

                if (html.Trim() == "")
                {
                    // retry once if no html returned
                    ahk.Sleep(500);
                    html = web.DownloadHTML(URL);

                    if (html.Trim() == "")
                    {
                        ahk.MsgBox("RapidGator Pass Invalid");
                        return "";
                    }
                }

                // "{\"response\":{\"session_id\":\"feps5ksc2s94k5grgno52q71g4\",\"expire_date\":1502504883,\"traffic_left\":\"2655431988666\"},\"response_status\":200,\"response_details\":null}"

                List<string> segs = ahk.StringSplit_List(html, "\"");
                bool FoundSessionID = false;
                bool FoundTraffic = false;
                bool FoundStatus = false;

                foreach (string seg in segs)
                {
                    if (seg == ":") { continue; }
                    if (seg.Contains("session_id")) { FoundSessionID = true; continue; }
                    if (FoundSessionID) { sessionID = seg; FoundSessionID = false; }

                    if (seg.Contains("traffic_left")) { FoundTraffic = true; continue; }
                    if (FoundTraffic) { trafficLeft = seg; FoundTraffic = false; }

                    if (seg.Contains("response_status")) { FoundStatus = true; continue; }
                    if (FoundStatus) { responseStatus = seg; FoundStatus = false; }
                }

                //ahk.MsgBox("SessionID = " + sessionID + "\nTraffic Left = " + trafficLeft + "\nResponse Status = " + responseStatus); 
                sb.StatusBar("SessionID = " + sessionID + " | Traffic Left = " + trafficLeft + " | Response Status = " + responseStatus);

                if (sessionID == null)
                {
                    if (login == "lucidmethod@gmail.com")
                    {
                        return GrabSessionID("jasonlangan.mobile@gmail.com"); // rety with 2nd account if first fails
                    }
                }

                if (sessionID != null) { return sessionID; }
                else { return ""; }
            }

            // check to see if RapidGator link is ONLINE (returns true if access allowed)
            public bool CheckStatus(string LinkURL)
            {
                if (sessionID == null || sessionID == "") { GrabSessionID(); }  // populate session id if not found yet

                string link = LinkURL.Replace("/", @"\/");
                string URL = "https://rapidgator.net/api/file/check_link?sid=" + sessionID + "&url=[\"" + link + "\"]";

                string html = web.DownloadHTML(URL);

                //{"response":[{"url":"http:\/\/rapidgator.net\/file\/91c8b4e3bf332120a6045c9f678adc11\/Lady_Snowblood_2_Love_Song_of_Vengeance_1974_REMASTERED_BDRip_x264_VoMiT.rar.html","filename":"Lady_Snowblood_2_Love_Song_of_Vengeance_1974_REMASTERED_BDRip_x264_VoMiT.rar","size":"797266449","status":"ACCESS"}],"response_status":200,"response_details":null}

                List<string> segs = ahk.StringSplit_List(html, "\"");
                bool FoundURL = false; string linkURL = "";
                bool FoundFileName = false; string fileName = "";
                bool FoundSize = false; string fileSize = "";
                bool FoundStatus = false; string fileStatus = "";

                foreach (string seg in segs)
                {
                    if (seg == ":") { continue; }

                    if (seg.Contains("url")) { FoundURL = true; continue; }
                    if (FoundURL) { linkURL = seg; FoundURL = false; }

                    if (seg.Contains("filename")) { FoundFileName = true; continue; }
                    if (FoundFileName) { fileName = seg; FoundFileName = false; }

                    if (seg.Contains("size")) { FoundSize = true; continue; }
                    if (FoundSize) { fileSize = seg; FoundSize = false; }

                    if (seg == "status") { FoundStatus = true; continue; }
                    if (FoundStatus) { fileStatus = seg; FoundStatus = false; }
                }

                sb.StatusBar("FileName = " + fileName + " | FileSize = " + fileSize + " | FileStatus = " + fileStatus);
                //ahk.MsgBox("URL = " + LinkURL + "\nFileName = " + fileName + "\nFileSize = " + fileSize + "\nFileStatus = " + fileStatus); 


                if (fileStatus.ToUpper() == "ACCESS") { return true; }
                return false;
            }

            /// <summary>
            /// check to see if RapidGator link is Online, Returns Object with Returned Info 
            /// </summary>
            /// <param name="LinkURL">Rapidgator Link to Check</param>
            /// <returns>Returns object with Rapidgator file status</returns>
            public RGInfo RapidGatorCheckStatus(string LinkURL)
            {
                if (sessionID == null || sessionID == "") { GrabSessionID(); }  // populate session id if not found yet

                RGInfo rg = new RGInfo();


                string link = LinkURL.Replace("/", @"\/");
                string URL = "http://rapidgator.net/api/file/check_link?sid=" + sessionID + "&url=[\"" + link + "\"]";

                string html = web.DownloadHTML(URL);

                //{"response":[{"url":"http:\/\/rapidgator.net\/file\/91c8b4e3bf332120a6045c9f678adc11\/Lady_Snowblood_2_Love_Song_of_Vengeance_1974_REMASTERED_BDRip_x264_VoMiT.rar.html","filename":"Lady_Snowblood_2_Love_Song_of_Vengeance_1974_REMASTERED_BDRip_x264_VoMiT.rar","size":"797266449","status":"ACCESS"}],"response_status":200,"response_details":null}

                List<string> segs = ahk.StringSplit_List(html, "\"");
                bool FoundURL = false; string linkURL = "";
                bool FoundFileName = false; string fileName = "";
                bool FoundSize = false; string fileSize = "";
                bool FoundStatus = false; string fileStatus = "";

                foreach (string seg in segs)
                {
                    if (seg == ":") { continue; }

                    if (seg.Contains("url")) { FoundURL = true; continue; }
                    if (FoundURL) { linkURL = seg; FoundURL = false; rg.FileURL = linkURL; }

                    if (seg.Contains("filename")) { FoundFileName = true; continue; }
                    if (FoundFileName) { fileName = seg; FoundFileName = false; rg.FileName = fileName; }

                    if (seg.Contains("size")) { FoundSize = true; continue; }
                    if (FoundSize) { fileSize = seg; FoundSize = false; rg.FileSize = ahk.FormatBytes(fileSize); }

                    if (seg == "status") { FoundStatus = true; continue; }
                    if (FoundStatus) { fileStatus = seg; FoundStatus = false; }
                }

                sb.StatusBar("FileName = " + fileName + " | FileSize = " + fileSize + " | FileStatus = " + fileStatus);
                //ahk.MsgBox("URL = " + LinkURL + "\nFileName = " + fileName + "\nFileSize = " + fileSize + "\nFileStatus = " + fileStatus); 


                if (fileStatus.ToUpper() == "ACCESS") { rg.FileOnline = true; }
                else { rg.FileOnline = false; }

                return rg;
            }


            /// <summary>
            /// Takes Link of Checked RGLinks and Totals Size of All Links in List
            /// </summary>
            /// <param name="RgLinks"></param>
            /// <returns></returns>
            public string RGLinks_FileSizeTotal(List<_Sites.RapidGator.RGInfo> RgLinks)
            {
                long TotalBytes = 0;
                foreach (_Sites.RapidGator.RGInfo RgLink in RgLinks)
                {
                    string size = RgLink.FileSize;
                    int bytes = ahk.ToBytes(size);
                    TotalBytes += bytes;
                    //ahk.MsgBox(size + "\n" + bytes.ToString() + "\n" + TotalBytes);
                }
                return ahk.FormatBytes(TotalBytes);
            }


            /// <summary>
            /// Converts RGLInfo List to String with Verified RGLinks
            /// </summary>
            /// <param name="RgLinks"></param>
            /// <returns></returns>
            public string RGLinks_String(List<_Sites.RapidGator.RGInfo> RgLinks)
            {
                string links = "";
                foreach (_Sites.RapidGator.RGInfo RgLink in RgLinks)
                {
                    if (links == "") { links = RgLink.FileURL; }
                    else { links = links + "\n" + RgLink.FileURL; }
                }
                return links;
            }



            // parse html for list of rapidgator links
            public List<string> Extract_RapidGatorLinks_HTML(string html, bool ToClipboard = false)
            {
                List<string> lines = lst.Text_To_List(html, true, true, false);
                List<string> links = new List<string>();

                foreach (string line in lines)
                {
                    if (line.Contains("rapidgator.net"))
                    {
                        string link = ahk.StringSplit(line, "\"", 1);
                        if (!link.ToUpper().Contains("THANK.YOU"))
                        {
                            links.Add(link);
                            //ahk.MsgBox("Link Captured: " + link);
                        }
                    }
                }

                string rapidlinks = "";
                foreach (string lnk in links)
                {
                    if (rapidlinks != "") { rapidlinks = rapidlinks + "\n" + lnk; }
                    if (rapidlinks == "") { rapidlinks = lnk; }
                }

                // copy links to clipboard
                if (ToClipboard) { ahk.ClipWrite(rapidlinks); }


                return links;
            }


            // Check to see if SafeLinking URL is ONLINE
            public bool SafeLinkCheck(string safeLinkURL)
            {
                string url = "https://safelinking.net/check?link=" + safeLinkURL;

                string html = web.DownloadHTML(url);

                //<link_status>Online</link_status>
                //<title>Download file STS.0701.720p.HEVC.x265.rar</title>
                //<host>RapidGator</host>

                List<string> lines = lst.Text_To_List(html);
                string linkStatus = "";
                string fileName = "";
                string host = "";
                foreach (string line in lines)
                {
                    if (line.Contains("<link_status>"))
                    {
                        linkStatus = line.Replace("<link_status>", "");
                        linkStatus = linkStatus.Replace("</link_status>", "");
                    }
                    if (line.Contains("<title>"))
                    {
                        fileName = line.Replace("<title>", "");
                        fileName = fileName.Replace("</title>", "");
                    }
                    if (line.Contains("<host>"))
                    {
                        host = line.Replace("<host>", "");
                        host = host.Replace("</host>", "");
                    }
                }

                fileName = fileName.Replace("Download file", "");

                ahk.MsgBox("FileName = " + fileName + "\nLink Status = " + linkStatus + "\nHost = " + host);

                if (linkStatus == "Online") { return true; }

                //Online	Links are online.	
                //Not yet checked	Links have not been checked yet.
                //Broken	Not all the links are online.	
                //Unknown	The links are unknown to the robot.	
                //Offline	Links are offline.	

                return false;
            }


            /// <summary>
            /// downloads rapidgator links from provided url as list
            /// </summary>
            /// <param name="url"></param>
            /// <returns></returns>
            public List<string> RapidGatorLinks_FromURL(string url)
            {
                List<string> links = new List<string>();

                string html = web.DownloadHTML(url);

                // parse html for list of links

                if (html.Trim() != "")
                {
                    var linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                    foreach (Match m in linkParser.Matches(html))
                    {
                        if (m.Value != null)
                        {
                            string val = m.Value;
                            if (val.Contains("rapidgator.net")) { links.Add(val); }
                            if (val.Contains("rg.to")) { links.Add(val); }
                            if (val.Contains("safelinking.net")) { links.Add(val); }
                        }
                    }
                }

                return links;
            }

            /// <summary>
            /// parses rapidgator links from provided local file as list
            /// </summary>
            /// <param name="filePath"></param>
            /// <returns></returns>
            public List<string> RapidGatorLinks_FromFile(string filePath)
            {
                List<string> links = new List<string>();

                string html = ahk.FileRead(filePath);

                // parse html for list of links

                if (html.Trim() != "")
                {
                    var linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                    foreach (Match m in linkParser.Matches(html))
                    {
                        if (m.Value != null)
                        {
                            string val = m.Value;
                            if (val.Contains("rapidgator.net")) { links.Add(val); }
                            if (val.Contains("rg.to")) { links.Add(val); }
                            if (val.Contains("safelinking.net")) { links.Add(val); }
                        }
                    }
                }

                return links;
            }

            /// <summary>
            /// parses rapidgator links from html 
            /// </summary>
            /// <param name="html"></param>
            /// <returns></returns>
            public List<string> RapidGatorLinks_FromHTML(string html)
            {
                List<string> links = new List<string>();

                // parse html for list of links

                if (html.Trim() != "")
                {
                    var linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);

                    foreach (Match m in linkParser.Matches(html))
                    {
                        if (m.Value != null)
                        {
                            string val = m.Value;
                            if (val.Contains("rapidgator.net")) { links.Add(val); }
                            if (val.Contains("rg.to")) { links.Add(val); }
                            if (val.Contains("safelinking.net")) { links.Add(val); }
                        }
                    }
                }

                return links;
            }




            // example scripts for dealing with rapidgator web 
            public void RapidGator_Javascript()
            {

                //                import Rapidgator from "rapidgator";

                //                const loginOptions = {
                //    login: "yourlogin",
                //    password: "yourpassword"
                //};

                //            const rapidgator = new Rapidgator(loginOptions);

                //            rapidgator.logIn().then(() => {
                //                const uploadOptionsFilepath = {
                //        name: "filename.txt",
                //        filePath: "/tmp/somefile.txt"
                //                };

                //            const uploadOptionsBuffer = {
                //        name: "filename.txt",
                //        buffer: fs.readFileSync("/tmp/somefile.txt")
                //    };

                //            rapidgator.upload(uploadOptionsFilepath).then((url) => console.log(`Your file has been uploaded.URL: ${ url}`)).catch(console.error);
                //}).catch((err) => console.error("Failed on logging in. Error: ", err));



            }



            // === WBB Login Attempt with Cookies --- Try Again When Trusted Site Working ---

            public static CookieContainer wbbLogin(string url, string username, string password)
            {
                if (url.Length == 0 || username.Length == 0 || password.Length == 0)
                {
                    MessageBox.Show("Information Missing", "Error");
                    return null;
                }

                CookieContainer myContainer = new CookieContainer();

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.CookieContainer = new CookieContainer();

                // Set type to POST

                request.Method = "POST";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; rv:2.0.1) Gecko/20100101 Firefox/4.0.1";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ProtocolVersion = HttpVersion.Version10;  // to get rid of the 417 error
                ServicePointManager.Expect100Continue = true; // to get rid of the 417 error

                // Build the new header, this isn't a multipart/form, so it's very simple
                System.Text.StringBuilder data = new System.Text.StringBuilder();
                data.Append("username=" + Uri.EscapeDataString(username));
                data.Append("&password=" + Uri.EscapeDataString(password));
                data.Append("&login=Login");

                // Create a byte array of the data we want to send
                byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

                // Set the content length in the request headers
                request.ContentLength = byteData.Length;

                System.IO.Stream postStream;
                try
                {
                    postStream = request.GetRequestStream();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Login - " + e.Message.ToString() + " (GRS)");
                    return null;
                }

                // Write data
                postStream.Write(byteData, 0, byteData.Length);

                HttpWebResponse response;
                try
                {
                    response = (HttpWebResponse)request.GetResponse();
                }
                catch (Exception e)
                {
                    MessageBox.Show("Login - " + e.Message.ToString() + " (GR)");
                    return null;
                }

                bool isLoggedIn = false;

                // Store the cookies
                foreach (Cookie c in response.Cookies)
                {
                    if (c.Name.Contains("_u"))
                    {
                        if (Convert.ToInt32(c.Value) > 1)
                        {
                            isLoggedIn = true;
                        }
                    }
                    myContainer.Add(c);
                }

                if (isLoggedIn)
                {
                    return myContainer;
                }
                else
                {
                    return null;
                }
            }

            // Submit to Poster.php from here

            public void WbbLoginTest_Click(object sender = null, EventArgs e = null)
            {

                CookieContainer boardCookies = wbbLogin("https://www.warez-bb.org/login.php", "rapidfox09", "foxrapid1");

                if (boardCookies != null)
                    MessageBox.Show("Your Login Was Successful, Welcome :}");
                else
                    MessageBox.Show("Login Failed, Recheck the Data Entered");
            }


            // v2.2

            /// <summary>
            /// Check List of RapidGator Links in Batch (up to 25 at a time), Returns RGInfo List with 
            /// </summary>
            /// <param name="Links"></param>
            /// <param name="progress"></param>
            /// <returns></returns>
            public List<RGInfo> RapidGator_BatchCheck(List<string> Links, bool ReturnOnlineOnly = true, RadProgressBar progress = null)
            {
                _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();

                string goodlinks = "";
                int linksCount = 0;
                List<string> linkList = new List<string>();
                List<RGInfo> GoodLinks = new List<RGInfo>();
                List<RGInfo> AllLinks = new List<RGInfo>();

                int i = 0;
                if (progress != null) { pro.SetupProgressBar(progress, Links.Count); }

                //========= Mulitiple Link Checkers ==============

                int n = 0;
                foreach (string link in Links)
                {
                    string Link = link;
                    if (link.Contains("|")) { Link = ahk.StringSplit(link, "|", 0); }

                    i++; linkList.Add(Link);
                    if (i == 25)
                    {
                        List<RGInfo> linkInfo = RapidGatorLinkList(linkList);

                        foreach (RGInfo rglink in linkInfo)
                        {
                            if (rglink.FileOnline) { GoodLinks.Add(rglink); }
                            AllLinks.Add(rglink);
                        }

                        linkList = new List<string>();
                        i = 0;
                    }

                    if (progress != null) { n++; pro.UpdateProgress(progress, n + "/" + Links.Count); }
                }

                // check list under 25 count added to link list
                if (linkList.Count > 0)
                {
                    List<RGInfo> linkInfo = RapidGatorLinkList(linkList);

                    foreach (RGInfo rglink in linkInfo)
                    {
                        if (rglink.FileOnline) { GoodLinks.Add(rglink); }
                        AllLinks.Add(rglink);
                    }

                    linkList = new List<string>();
                }


                if (ReturnOnlineOnly) { return GoodLinks; }
                else { return AllLinks; }
            }


            public List<RGInfo> RapidGatorLinkList(List<string> LinkURLs)
            {
                if (sessionID == null || sessionID == "") { GrabSessionID(); }  // populate session id if not found yet

                string links = "";

                foreach (string LinkURL in LinkURLs)
                {
                    string link = LinkURL;

                    if (!link.Contains(@"\/")) { link = LinkURL.Replace("/", @"\/"); }

                    link = "\"" + link + "\"";

                    if (links == "") { links = link; }
                    else { links = links + "," + link; }
                }

                string URL = "http://rapidgator.net/api/file/check_link?sid=" + sessionID + "&url=[" + links + "]";


                string html = web.DownloadHTML(URL);

                //{ "response":[{"url":"https:\/\/rapidgator.net\/file\/2eb05673cb8e6957c0319ed9e983efca\/12.Monkeys.S04E07.HDTV.x264-KILLERS.mkv.html","filename":"12.Monkeys.S04E07.HDTV.x264-KILLERS.mkv","size":"219803063","status":"ACCESS"},
                //{ "url":"https:\/\/rapidgator.net\/file\/2eb05673cb8e6957c0319ed9e983efca\/12.Monkeys.S04E07.HDTV.x264-KILLERS.mkv.html","filename":"12.Monkeys.S04E07.HDTV.x264-KILLERS.mkv","size":"219803063","status":"ACCESS"},

                List<RGInfo> RGs = new List<RGInfo>();

                List<string> ResponseSegs = ahk.StringSplit_List(html, "{"); int i = 0;
                foreach (string Seg in ResponseSegs)
                {
                    i++; if (i == 1) { continue; }

                    RGInfo rg = new RGInfo();

                    List<string> segs = ahk.StringSplit_List(Seg, "\"");
                    bool FoundURL = false; string linkURL = "";
                    bool FoundFileName = false; string fileName = "";
                    bool FoundSize = false; string fileSize = "";
                    bool FoundStatus = false; string fileStatus = "";

                    foreach (string seg in segs)
                    {
                        if (seg == ":") { continue; }

                        if (seg.Contains("url")) { FoundURL = true; continue; }
                        if (FoundURL) { linkURL = seg; FoundURL = false; rg.FileURL = linkURL; }

                        if (seg.Contains("filename")) { FoundFileName = true; continue; }
                        if (FoundFileName) { fileName = seg; FoundFileName = false; rg.FileName = fileName; }

                        if (seg.Contains("size")) { FoundSize = true; continue; }
                        if (FoundSize) { fileSize = seg; FoundSize = false; rg.FileSize = ahk.FormatBytes(fileSize); }

                        if (seg == "status") { FoundStatus = true; continue; }
                        if (FoundStatus) { fileStatus = seg; FoundStatus = false; }
                    }

                    sb.StatusBar("FileName = " + fileName + " | FileSize = " + fileSize + " | FileStatus = " + fileStatus);
                    //ahk.MsgBox("URL = " + LinkURL + "\nFileName = " + fileName + "\nFileSize = " + fileSize + "\nFileStatus = " + fileStatus); 


                    if (fileStatus.ToUpper() == "ACCESS") { rg.FileOnline = true; }
                    else { rg.FileOnline = false; }

                    RGs.Add(rg);
                }



                return RGs;
            }


            /// <summary>
            /// Download URL HTML, Extract List of RapidGator Links, Populate RadTree if Provided
            /// </summary>
            /// <param name="URL"></param>
            /// <param name="RadTree"></param>
            /// <param name="ClearTree"></param>
            /// <param name="CheckOnlineStatus"></param>
            /// <returns></returns>
            public List<string> PullRGLinks(string ShowName, string URL, bool CheckOnlineStatus = true, bool DownloadMissingImages = true, bool AllImages = false)
            {
                _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();

                string showName = ShowName;

                string html = ahk.Download_HTML(URL);

                List<string> links = Regex_RGLinks(html);

                List<string> glinks = new List<string>();
                string goodlinks = "";


                // Extract / Display Images From Page

                List<string> images = JpgImageLinks(html);  // list of jpg images found on page
                string savename = "";
                if (images != null && images.Count > 0)
                {
                    int i = 0;
                    foreach (string image in images)
                    {
                        string imagedir = ahk.AppDir() + "\\ShowImages";
                        ahk.FileCreateDir(imagedir);
                        savename = imagedir + "\\" + showName + ".jpg";

                        if (AllImages)
                        {
                            if (i > 0) { savename = imagedir + "\\" + showName + "_" + i + ".jpg"; }
                            i++;
                        }

                        if (DownloadMissingImages)
                        {
                            if (!File.Exists(savename))
                            {
                                web.DownloadFile(image, savename, true);
                            }
                        }

                        if (!AllImages) { break; }
                    }
                }

                if (CheckOnlineStatus)
                {

                    //// Batch Link Check Method
                    //_Sites.RapidGator rg = new RapidGator();
                    //List<RGInfo> checkedLinks = rg.RapidGator_BatchCheck(links, true, LinkCheckProgress);

                    //foreach(RGInfo lnk in checkedLinks)
                    //{
                    //    if (lnk.FileOnline)
                    //    {
                    //        string rgLinkInfo = lnk.FileURL + "|" + lnk.FileSize;

                    //        glinks.Add(rgLinkInfo); if (goodlinks == "") { goodlinks = rgLinkInfo; } else { goodlinks = goodlinks + "\n" + rgLinkInfo; }
                    //    }
                    //}

                    // Singe Link Check Method

                    pro.SetupProgressBar(LinkCheckProgress, links.Count); int k = 1;

                    foreach (string link in links)
                    {
                        pro.UpdateProgress(LinkCheckProgress, "Checking Link: " + k + "/" + links.Count); k++;

                        RGInfo info = RapidGatorCheckStatus(link);

                        if (info.FileOnline)
                        {
                            string rgLinkInfo = link + "|" + info.FileSize;

                            glinks.Add(rgLinkInfo); if (goodlinks == "") { goodlinks = rgLinkInfo; } else { goodlinks = goodlinks + "\n" + rgLinkInfo; }
                        }
                    }
                }
                else { glinks = links; }

                //ahk.MsgBox("Found " + links.Count + " Links");
                return glinks;
            }


        }

    }
}

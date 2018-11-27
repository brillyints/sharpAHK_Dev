using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using sharpAHK;
using sharpAHK_Dev;
using AHKExpressions;
using TelerikExpressions;
using System.Configuration;
using Telerik.WinControls.UI;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;
using static sharpAHK_Dev._Sites.EpGuides;
using System.Diagnostics;

namespace sharpAHK_Dev
{
    public partial class _Sites
    {

        // scene source v2.1
        public class SceneSource
        {
            #region === Startup ===

            //_Database.goDaddy goDad = new _Database.goDaddy();
            _AHK ahk = new _AHK();
            _Database.SQL sql = new _Database.SQL();
            _Database.SQLite sqlite = new _Database.SQLite();
            _Lists lst = new _Lists();
            _Parse prs = new _Parse();
            _StatusBar sb = new _StatusBar();
            _Web web = new _Web();
            _TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            _Images img = new _Images();

            //_Database.goDaddy goDad = new _Database.goDaddy();
            //_Database.SQL sql = new _Database.SQL();
            //_Database.SQLite sqlite = new _Database.SQLite();
            ////_GridControl grid = new _GridControl();
            //_Lists lst = new _Lists();
            //_Dict dict = new _Dict();
            //_Images img = new _Images();
            ////_WinForms frm = new _WinForms();
            ////_TreeViewControl tv = new _TreeViewControl();
            ////_TabControl tab = new _TabControl();
            ////_ScintillaControl sci = new _ScintillaControl();
            ////_Apps.Mpc mpc = new _Apps.Mpc();
            ////_Apps.Chrome cr = new _Apps.Chrome();
            ////_Code code = new _Code();
            ////_Database.Tags tags = new _Database.Tags();
            ////_Database.dir2Db dir2 = new _Database.dir2Db();
            //_Parse prs = new _Parse();
            ////_imdb im = new _imdb();
            ////_Web web = new _Web();
            ////_StatusBar sb = new _StatusBar();
            ////_RapidGator rg = new _RapidGator();
            //_Sites.EpGuides eps = new _Sites.EpGuides();
            ////FtP ftp = new FtP();
            ////_Database.dir2Db dirDb = new _Database.dir2Db();
            //_StatusBar sb = new _StatusBar();
            //_Sites.Imdb imdb = new _Sites.Imdb();
            //_TelerikLib tel = new _TelerikLib();
            //_TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            //_TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();
            //_TelerikLib._RadMenu menu = new _TelerikLib._RadMenu();
            ////_Sites.MovieParadise mp = new _Sites.MovieParadise();
            ////TVServerLocal tvServer = new TVServerLocal();
            //_Web web = new _Web();
            ////_Sites.RapidGator rg = new _Sites.RapidGator();
            _Parse.XML xml = new _Parse.XML();
            //RApidGator rg = new RApidGator();


            SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["epGuideDb"].ConnectionString);


            public static string JLRapidPass { get; set; }
            public static string LucidRapidPass { get; set; }

            public static RadTreeView DisplayTree { get; set; }

            public static PictureBox ShowImageBox { get; set; }
            public static Image ShowImage { get; set; }
            public static string ShowName { get; set; }

            // Assign RapidGator Passwords on Startup
            public void SetPasswords(string LucidPass = "", string JLPass = "")
            {
                JLRapidPass = JLPass;
                LucidRapidPass = LucidPass;
            }

            #endregion

            #region === RapidGator ===

            public static string login { get; set; }
            public static string pass { get; set; }
            public static string sessionID { get; set; }
            public static string trafficLeft { get; set; }
            public string responseStatus { get; set; }

            public static RadProgressBar LinkCheckProgress { get; set; }
            public static RadTextBox PostURL { get; set; }


            bool ShowImagesDuringDownload = false;

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
                if (Parent != null) { showName = Parent.Text; }

                string html = ahk.Download_HTML(URL);

                List<string> links = Regex_RGLinks(html);

                List<string> glinks = new List<string>();
                string goodlinks = "";

                if (!CheckOnlineStatus) { glinks = links; }

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

                if (ShowImagesDuringDownload)
                {
                    if (savename != "")
                    {
                        if (ShowImageBox != null)
                        {
                            if (ShowImageBox.InvokeRequired)
                            {
                                ShowImageBox.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    //ShowImageBox.Image = img.To_Image(savename); });
                                    ShowImageBox.Image = img.GetCopyImage(savename);
                                });
                            }
                            else
                            {
                                //ShowImageBox.Image = img.To_Image(savename);
                                ShowImageBox.Image = img.GetCopyImage(savename);
                            }
                        }
                    }
                }




                if (RadTree != null)
                {
                    // add list to treeview (without checking online status)
                    if (!CheckOnlineStatus) { tree.ListRadTree(RadTree, links); }
                }

                foreach (string link in links)
                {
                    RadTreeNode node = new RadTreeNode();
                    node.Tag = link;
                    string epFileName = "";
                    string SeasonEpNum = "";

                    if (CheckOnlineStatus)
                    {
                        bool online = CheckStatus(link);

                        if (online) { glinks.Add(link); if (goodlinks == "") { goodlinks = link; } else { goodlinks = goodlinks + "\n" + link; } }

                        RGInfo info = RapidGatorCheckStatus(link);

                        epFileName = info.FileName;

                        //SceneSource SS = new SceneSource();
                        //ssTv show = SS.SceneSource_TV_LineParse(epFileName, SSCats.TVShows, false);

                        SeasonEpNum = prs.SeasonEpNums("C:\\" + epFileName, false);

                        //ssTv show = new ssTv();

                        if (RadTree != null)
                        {
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
                        }


                    }

                    if (!CheckOnlineStatus)
                    {
                        // parse url for file name
                        List<string> parts = ahk.StringSplit_List(link, "/");
                        foreach (string part in parts)
                        {
                            epFileName = part;
                        }

                        epFileName = epFileName.Replace(".html", "");

                        node.Text = epFileName;

                        string fakePath = "C:\\" + epFileName;

                        SeasonEpNum = prs.SeasonEpNums(fakePath, false);
                        epFileName = ahk.FileNameNoExt(fakePath);
                    }


                    if (RadTree != null)
                    {
                        if (Parent == null)
                        {
                            tree.AddNode(RadTree, node);
                        }
                        else
                        {

                            string EpNodeText = showName + "_" + SeasonEpNum;
                            RadTreeNode EpNumNode = new RadTreeNode();
                            EpNumNode.Text = SeasonEpNum;
                            EpNumNode.Tag = EpNodeText;
                            tree.AddNode(RadTree, EpNumNode);
                        }
                    }


                }


                //ahk.MsgBox("Found " + links.Count + " Links");

                return glinks;
            }


            public struct RGInfo
            {
                public string FileURL { get; set; }
                public string FileName { get; set; }
                public string FileSize { get; set; }
                public bool FileOnline { get; set; }
            }


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

                    if (val.Contains(".jpg"))
                    {
                        //ahk.MsgBox(val); 
                        matches.Add(val);
                    }
                }



                return matches;
            }


            public bool GrabSessionID(string login = "lucidmethod@gmail.com")
            {
                // first call with l/p - returns session ID used in other calls
                //login = "lucidmethod@gmail.com";
                //pass = "zStj6X";

                if (login == "lucidmethod@gmail.com") { pass = LucidRapidPass; }
                if (login == "jasonlangan.mobile@gmail.com") { pass = JLRapidPass; }

                string URL = "https://rapidgator.net/api/user/login?username=" + login + "&password=" + pass;

                string html = web.DownloadHTML(URL);

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

                if (sessionID != null) { return true; }
                else { return false; }
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



            #endregion

            #region === SceneSource ===


            //https://www.scnsrc.me/category/tv/page/4/


            //_Database.goDaddy goDad = new _Database.goDaddy();
            //sharpAHK._AHK ahk = new sharpAHK._AHK();
            //_Database.SQL sql = new _Database.SQL();
            //_Database.SQLite sqlite = new _Database.SQLite();
            //_GridControl grid = new _GridControl();
            //_Lists lst = new _Lists();
            //_Dict dict = new _Dict();
            //_Images img = new _Images();
            //_WinForms frm = new _WinForms();
            //_TreeViewControl tv = new _TreeViewControl();
            //_TabControl tab = new _TabControl();
            //_ScintillaControl sci = new _ScintillaControl();
            //_Apps.Mpc mpc = new _Apps.Mpc();
            //_Apps.Chrome cr = new _Apps.Chrome();
            //_Code code = new _Code();
            //_Database.Tags tags = new _Database.Tags();
            //_Database.dir2Db dir2 = new _Database.dir2Db();
            //_Parse prs = new _Parse();
            //_imdb im = new _imdb();
            //_Web web = new _Web();
            //_StatusBar sb = new _StatusBar();
            //_RapidGator rg = new _RapidGator();
            //_Sites.EpGuides eps = new _Sites.EpGuides();
            ////FtP ftp = new FtP();
            ////_Database.dir2Db dirDb = new _Database.dir2Db();
            //_StatusBar sb = new _StatusBar();
            //_Sites.Imdb imdb = new _Sites.Imdb();
            //_TelerikLib tel = new _TelerikLib();
            //_TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();
            //_TelerikLib._RadMenu menu = new _TelerikLib._RadMenu();
            ////_Sites.MovieParadise mp = new _Sites.MovieParadise();
            ////TVServerLocal tvServer = new TVServerLocal();
            //_Web web = new _Web();
            ////_Sites.RapidGator rg = new _Sites.RapidGator();
            //_Parse.XML xm = new _Parse.XML();
            //RApidGator rg = new RApidGator();


            public static RadTextBox txtBox { get; set; }
            public static RadTextBox totalBox { get; set; }

            public static RadProgressBar dispProgress { get; set; }
            public static RadTreeView RadTree { get; set; }

            public static List<string> CapturedPostURLs { get; set; }




            //public static Image ShowImage { get; set; }
            //public static string ShowName { get; set; }


            #region === Scene Source Site ===



            public enum SiteName
            {
                SceneSource = 0,
                ReleaseBB = 1
            }


            /// <summary>
            /// 
            /// </summary>		
            public enum SSCats
            {
                BDRip,
                BluRay,
                DVDRip,
                ThreeD,
                HD,
                DVDScr,
                TVShows,
                Films 
            }




            public List<ssMovie> SceneSource_Download_MovieIndex(int PageNum = 1, int TotalPgs = 0, SSCats Cat = SSCats.Films)
            {
                bool CheckOnlineStatus = true; bool WriteSQL = true; bool DownloadImages = true; bool GrabLinks = true; bool AllImages = false;

                //CapturedPostURLs

                //string saveRoot = ahk.AppDir() + "\\SceneSource\\" + Cat;
                //ahk.FileCreateDir(saveRoot);

                string urlStart = "https://www.scnsrc.me";

                PageNum = 1;
                if (Cat == SSCats.TVShows) { urlStart = "https://www.scnsrc.me/category/tv/page/"; if (TotalPgs == 0) { TotalPgs = 182; } }
                if (Cat == SSCats.BDRip) { urlStart = "https://www.scnsrc.me/category/films/bdrip/page/"; if (TotalPgs == 0) { TotalPgs = 182; } }
                if (Cat == SSCats.BluRay) { urlStart = "https://www.scnsrc.me/category/films/bluray/page/"; if (TotalPgs == 0) { TotalPgs = 429; } }
                if (Cat == SSCats.DVDRip) { urlStart = "https://www.scnsrc.me/category/films/dvdrip/page/"; if (TotalPgs == 0) { TotalPgs = 489; } }
                if (Cat == SSCats.ThreeD) { urlStart = "https://www.scnsrc.me/category/films/3d/page/"; if (TotalPgs == 0) { TotalPgs = 9; } }
                if (Cat == SSCats.HD) { urlStart = "https://www.scnsrc.me/category/films/hd/page/"; if (TotalPgs == 0) { TotalPgs = 25; } }
                if (Cat == SSCats.DVDScr) { urlStart = "https://www.scnsrc.me/category/films/dvdscr/page/"; if (TotalPgs == 0) { TotalPgs = 24; } }
                if (Cat == SSCats.Films) { urlStart = "https://www.scnsrc.me/category/films/page/"; if (TotalPgs == 0) { TotalPgs = 400; } }

                int total = TotalPgs - PageNum;
                total++;

                if (dispProgress != null)
                {
                    pro.SetupProgressBar(dispProgress, total);
                }

                totalBox.UpdateTextbox(TotalPgs.ToString());
                List<ssMovie> pages = new List<ssMovie>();

                int i = PageNum;
                do
                {
                    if (txtBox != null) { txtBox.UpdateTextbox(i.ToString()); }

                    //txtCurrentPg.UpdateControl(i.ToString()); 
                    //txtCurrentPg.UpdateControl(i.ToString());

                    if (dispProgress != null) { pro.UpdateProgress(dispProgress, i + "/" + TotalPgs); }

                    string url = urlStart + i + "/";


                    List<ssMovie> page = Parse_SceneSourceMoviePage(url, Cat, RadTree, CheckOnlineStatus, WriteSQL, DownloadImages, GrabLinks);

                    // add page to list of pages
                    foreach (ssMovie pg in page)
                    {
                        pages.Add(pg);
                    }

                    //string saveFile = saveRoot + "\\" + i.AddLeadingZeros(4) + ".txt";
                    //ahk.FileAppend(html, saveFile);
                    //sqlite.Setting("SceneSourcePg", i.ToString());

                    i++;
                } while (i <= TotalPgs);

                //ahk.MsgBox("Finished Downloading SceneSource " + Cat + " Index HTML");

                return pages;
            }

            public List<ssMovie> Parse_SceneSourceMoviePage(string URL, SSCats Cat = SSCats.Films, RadTreeView RadTree = null, bool CheckOnlineStatus = true, bool WriteSQL = true, bool DownloadImages = true, bool GrabLinks = true, bool AllImages = false)
            {
                string xmlPath = "";

                if (Cat == SSCats.BDRip || Cat == SSCats.BluRay || Cat == SSCats.DVDRip || Cat == SSCats.ThreeD || Cat == SSCats.HD || Cat == SSCats.DVDScr || Cat == SSCats.Films) { xmlPath = "//body/div/div/div/div/h2"; }

                string html = web.DownloadHTML(URL);

                List<string> lines = xml.Parse_HTML_XML(html, xmlPath);  // extract sections of text from html xml

                List<ssTv> TVResults = new List<ssTv>();
                List<ssMovie> MovieResults = new List<ssMovie>();

                int i = 1;
                foreach (string line in lines)
                {
                    if (Cat == SSCats.BDRip || Cat == SSCats.BluRay || Cat == SSCats.DVDRip || Cat == SSCats.ThreeD || Cat == SSCats.HD || Cat == SSCats.DVDScr || Cat == SSCats.Films)
                    {
                        MovieResults.Add(SceneSource_Movie_LineParse(line, Cat));
                    }

                    sb.StatusBar(i + "/" + lines.Count); i++;
                }

                return MovieResults;

            }

            public ssMovie SceneSource_Movie_LineParse(string Line, SSCats Cat = SSCats.Films, bool WriteSQL = true)
            {
                ssMovie movie = new ssMovie();

                if (Cat == SSCats.BDRip || Cat == SSCats.BluRay || Cat == SSCats.DVDRip || Cat == SSCats.ThreeD || Cat == SSCats.HD || Cat == SSCats.DVDScr || Cat == SSCats.Films)
                {
                    movie.Cateogry = Cat.ToString();

                    string StartLine = Line.Decode();

                    string sline = StartLine.Replace("<a href=\"", "");
                    string sURL = ahk.StringSplit(sline, "\"", 0);
                    sURL = ahk.TrimEndIf(sURL, "/");

                    sline = sline.Replace(sURL, "");
                    sline = sline.Replace("\"", "");
                    sline = sline.Replace("rel=bookmark", "");
                    sline = sline.Replace("title=", "").Trim();

                    string sName = ahk.StringSplit(sline, ">", 0).Trim();
                    sName = ahk.TrimFirst(sName, 7); // trim off "  goto "

                    string PostTitle = sName.Trim();
                    string SeasonEp = prs.ExtractYear(sName);  // extract year

                    if (SeasonEp != "")
                    {
                        sName = sName.Replace(SeasonEp, "|");
                        sName = ahk.StringSplit(sName, "|", 0);
                    }

                    string ShowName = sName.Trim();

                    string returnLine = sURL + "\n" + PostTitle + "\n" + ShowName + "\n" + SeasonEp;


                    movie.MovieName = ShowName;
                    movie.PostURL = sURL;
                    movie.PostTitle = PostTitle;
                    movie.Year = SeasonEp;

                    if (WriteSQL)
                    {
                        // write to sql db
                        bool added = ssMovie_UpdateInsert(movie);
                    }

                    //sb("Added " + PostTitle + " = " + added.ToString());
                    //ahk.MsgBox(sURL + "\n" + PostTitle + "\n" + ShowName + "\n" + SeasonEp);
                }


                return movie;
            }





            public List<ssTv> SceneSource_Download_TVIndexHTML(int PageNum = 1, int TotalPgs = 0, SSCats Cat = SSCats.TVShows, bool CheckOnlineStatus = true, bool WriteSQL = true, bool DownloadImages = true, bool GrabLinks = true, bool AllImages = false)
            {
                //CapturedPostURLs

                //string saveRoot = ahk.AppDir() + "\\SceneSource\\" + Cat;
                //ahk.FileCreateDir(saveRoot);

                string urlStart = "https://www.scnsrc.me";

                PageNum = 1;
                if (Cat == SSCats.TVShows) { urlStart = "https://www.scnsrc.me/category/tv/page/"; if (TotalPgs == 0) { TotalPgs = 182; } }
                if (Cat == SSCats.BDRip) { urlStart = "https://www.scnsrc.me/category/films/bdrip/page/"; if (TotalPgs == 0) { TotalPgs = 182; } }
                if (Cat == SSCats.BluRay) { urlStart = "https://www.scnsrc.me/category/films/bluray/page/"; if (TotalPgs == 0) { TotalPgs = 429; } }
                if (Cat == SSCats.DVDRip) { urlStart = "https://www.scnsrc.me/category/films/dvdrip/page/"; if (TotalPgs == 0) { TotalPgs = 489; } }
                if (Cat == SSCats.ThreeD) { urlStart = "https://www.scnsrc.me/category/films/3d/page/"; if (TotalPgs == 0) { TotalPgs = 9; } }
                if (Cat == SSCats.HD) { urlStart = "https://www.scnsrc.me/category/films/hd/page/"; if (TotalPgs == 0) { TotalPgs = 25; } }
                if (Cat == SSCats.DVDScr) { urlStart = "https://www.scnsrc.me/category/films/dvdscr/page/"; if (TotalPgs == 0) { TotalPgs = 24; } }

                int total = TotalPgs - PageNum;
                total++;

                if (dispProgress != null)
                {
                    pro.SetupProgressBar(dispProgress, total);
                }

                totalBox.UpdateTextbox(TotalPgs.ToString());
                List<ssTv> pages = new List<ssTv>();

                int i = PageNum;
                do
                {
                    if (txtBox != null) { txtBox.UpdateTextbox(i.ToString()); }

                    //txtCurrentPg.UpdateControl(i.ToString()); 
                    //txtCurrentPg.UpdateControl(i.ToString());

                    if (dispProgress != null) { pro.UpdateProgress(dispProgress, i + "/" + TotalPgs); }

                    string url = urlStart + i + "/";


                    List<ssTv> page = Parse_SceneSourcePage(url, Cat, RadTree, CheckOnlineStatus, WriteSQL, DownloadImages, GrabLinks);

                    // add page to list of pages
                    foreach (ssTv pg in page)
                    {
                        pages.Add(pg);
                    }

                    //string saveFile = saveRoot + "\\" + i.AddLeadingZeros(4) + ".txt";
                    //ahk.FileAppend(html, saveFile);
                    //sqlite.Setting("SceneSourcePg", i.ToString());

                    i++;
                } while (i <= TotalPgs);

                //ahk.MsgBox("Finished Downloading SceneSource " + Cat + " Index HTML");

                return pages;
            }

            // check to see if post url has already been entered into db
            public bool ssTvURL_InDb(string URL)
            {
                string cmd = "select count(posttitle) FROM [lucidmedia].[lucidmethod].[SceneSourceTV] where PostURL = '" + URL + "'";
                int count = sql.Count(ssTv_Conn(), cmd);
                if (count > 0) { return true; }
                return false;
            }

            public List<ssTv> Parse_SceneSourcePage(string URL, SSCats Cat = SSCats.TVShows, RadTreeView RadTree = null, bool CheckOnlineStatus = true, bool WriteSQL = true, bool DownloadImages = true, bool GrabLinks = true, bool AllImages = false)
            {
                string xmlPath = "";

                if (Cat == SSCats.TVShows) { xmlPath = "//body/div/div/div/div/h2"; }

                if (Cat == SSCats.BDRip || Cat == SSCats.BluRay || Cat == SSCats.DVDRip || Cat == SSCats.ThreeD || Cat == SSCats.HD || Cat == SSCats.DVDScr || Cat == SSCats.Films) { xmlPath = "//body/div/div/div/div/h2"; }

                string html = web.DownloadHTML(URL);

                List<string> lines = xml.Parse_HTML_XML(html, xmlPath);  // extract sections of text from html xml

                List<ssTv> TVResults = new List<ssTv>();
                List<ssMovie> MovieResults = new List<ssMovie>();

                int i = 1;
                foreach (string line in lines)
                {
                    if (Cat == SSCats.TVShows)
                    {
                        TVResults.Add(SceneSource_TV_LineParse(line, Cat, WriteSQL, CheckOnlineStatus, DownloadImages, GrabLinks, AllImages));
                    }

                    if (Cat == SSCats.BDRip || Cat == SSCats.BluRay || Cat == SSCats.DVDRip || Cat == SSCats.ThreeD || Cat == SSCats.HD || Cat == SSCats.DVDScr || Cat == SSCats.Films)
                    {
                        MovieResults.Add(SceneSource_Movie_LineParse(line, Cat));
                    }

                    sb.StatusBar(i + "/" + lines.Count); i++;
                }

                return TVResults;

                //if (RadTree != null)
                //{
                //    if (Cat == SSCats.TVShows)
                //    {
                //        foreach (ssTv show in TVResults)
                //        {
                //            RadTreeNode node = new RadTreeNode();  // show name 
                //            node.Text = show.ShowName;
                //            node.Tag = show.PostURL;
                //            tree.AddNode(RadTree, node);


                //            //Thread NewThread = new Thread(() => rg.RGLinks(txtURL.Text, radTree, true, cbCheckOnlineStat.Checked)); // Function To Execute
                //            //NewThread.Start();

                //            string rglinks = "";
                //            List<string> links = RGLinks(show.PostURL, RadTree, false, CheckOnlineStatus, node);
                //            foreach (string lnk in links)
                //            {
                //                if (rglinks == "") { rglinks = lnk; } else { rglinks = rglinks + "\n" + lnk; }
                //            }

                //            bool updated = UpdateRGLinks(show.PostURL, rglinks);

                //        }
                //    }


                //}


                //ahk.MsgBox(TVResults.Count); 
            }


            public void ParseHTMLDir(SSCats Cat, RadProgressBar dispProgress = null)
            {
                string saveRoot = ahk.AppDir() + "\\SceneSource\\";

                List<string> files = lst.FileList(saveRoot, "*.txt", true, false, true);

                if (dispProgress != null)
                {
                    pro.SetupProgressBar(dispProgress, files.Count);
                    pro.ResetProgress(dispProgress);
                }

                string xmlPath = "";

                if (Cat == SSCats.TVShows) { xmlPath = "//body/div/div/div/div/h2"; }

                if (Cat == SSCats.BDRip || Cat == SSCats.BluRay || Cat == SSCats.DVDRip || Cat == SSCats.ThreeD || Cat == SSCats.HD || Cat == SSCats.DVDScr) { xmlPath = "//body/div/div/div/div/h2"; }

                int p = 1;
                foreach (string file in files)
                {
                    pro.UpdateProgress(dispProgress, p + "/" + files.Count); p++;

                    string html = ahk.FileRead(file);
                    //string html = web.DownloadHTML(url);

                    //string xmlPath = "//body/div/div/div/div/h2";

                    //string xmlPath = txtXMLPath.Text;
                    //sqlite.Setting("LastXmlPath", xmlPath);


                    List<string> lines = xml.Parse_HTML_XML(html, xmlPath);  // extract sections of text from html xml
                    List<string> results = new List<string>();

                    int i = 1;
                    foreach (string line in lines)
                    {

                        if (Cat == SSCats.TVShows)
                        {
                            results.Add(SceneSource_TV_LineParse(line, Cat) + "\n\n");
                        }

                        if (Cat == SSCats.BDRip || Cat == SSCats.BluRay || Cat == SSCats.DVDRip || Cat == SSCats.ThreeD || Cat == SSCats.HD || Cat == SSCats.DVDScr)
                        {
                            results.Add(SceneSource_Movie_LineParse(line, Cat) + "\n\n");
                        }

                        sb.StatusBar(i + "/" + lines.Count); i++;
                    }

                }

                pro.ResetProgress(dispProgress);
            }


            public void UpdateLinks(ssTv show)
            {
                List<string> links = RGLinks(show.PostURL);
                string goodlinks = "";
                int linksCount = 0;

                foreach (string link in links)
                {
                    RGInfo info = RapidGatorCheckStatus(link);

                    if (info.FileOnline)
                    {
                        string rgLinkInfo = info.FileURL + "|" + info.FileSize;

                        if (goodlinks == "") { goodlinks = rgLinkInfo; linksCount++; } else { goodlinks = goodlinks + "\n" + rgLinkInfo; linksCount++; }
                    }
                }

                show.LinksChecked = true;
                show.LinksOnline = linksCount;
                show.RapidGator = goodlinks;

                UpdateShow(show);
            }

            public ssTv SceneSource_TV_LineParse(string Line, SSCats Cat = SSCats.TVShows, bool WriteSQL = false, bool CheckOnlineStatus = true, bool DownloadImages = true, bool GrabLinks = true, bool AllImages = false)
            {
                RapidGator rg = new RapidGator();

                bool SkipExistingPosts = true;

                ssTv show = new ssTv();

                if (Cat == SSCats.TVShows)
                {
                    string StartLine = Line.Decode();

                    string sline = StartLine.Replace("<a href=\"", "");
                    string sURL = ahk.StringSplit(sline, "\"", 0);
                    sURL = ahk.TrimEndIf(sURL, "/");

                    sline = sline.Replace(sURL, "");
                    sline = sline.Replace("\"", "");
                    sline = sline.Replace("rel=bookmark", "");
                    sline = sline.Replace("title=", "").Trim();

                    string sName = ahk.StringSplit(sline, ">", 0).Trim();
                    sName = ahk.TrimFirst(sName, 7); // trim off "  goto "

                    string PostTitle = sName.Trim();

                    string SeasonEp = ShowEp_FromFilePath(PostTitle);
                    string ShowName = ShowName_FromFilePath(PostTitle);


                    string returnLine = sURL + "\n" + PostTitle + "\n" + ShowName + "\n" + SeasonEp;

                    show.ShowName = ShowName;
                    show.PostURL = sURL;
                    show.PostTitle = PostTitle;
                    show.SeasonEp = SeasonEp;

                    if (SkipExistingPosts)  // option to check to see if post url exists in db
                    {
                        bool exists = ssTvURL_InDb(show.PostURL);
                        if (exists) { return ShowByURL(show.PostURL); }
                    }


                    string rglinks = "";
                    List<string> links = new List<string>();

                    if (GrabLinks)  // download html from each page in index, extract links/images
                    {
                        links = rg.PullRGLinks(show.ShowName, show.PostURL, CheckOnlineStatus, DownloadImages, AllImages);
                        foreach (string lnk in links)
                        {
                            if (rglinks == "") { rglinks = lnk; } else { rglinks = rglinks + "\n" + lnk; }
                        }

                        if (CheckOnlineStatus) { show.LinksChecked = true; }
                        else { show.LinksChecked = false; }
                    }


                    show.LinksOnline = links.Count;
                    show.RapidGator = rglinks;


                    string imagedir = ahk.AppDir() + "\\ShowImages";
                    string imgPath = imagedir + "\\" + show.ShowName + ".jpg";

                    if (ShowImagesDuringDownload)
                    {
                        if (File.Exists(imgPath))
                        {
                            if (ShowImageBox != null)
                            {
                                if (ShowImageBox.InvokeRequired)
                                {
                                    ShowImageBox.BeginInvoke((MethodInvoker)delegate ()
                                    {
                                        //ShowImageBox.Image = img.To_Image(savename); });
                                        ShowImageBox.Image = img.GetCopyImage(imgPath);
                                    });
                                }
                                else
                                {
                                    //ShowImageBox.Image = img.To_Image(savename);
                                    ShowImageBox.Image = img.GetCopyImage(imgPath);
                                }
                            }
                        }
                    }


                    if (WriteSQL)
                    {
                        // write to sql db
                        bool added = UpdateShow(show);
                        //bool added = ssTv_UpdateInsert(show);
                    }

                    //sb.StatusBar("Added " + PostTitle + " = " + added.ToString());

                    return show;
                    //ahk.MsgBox(sURL + "\n" + PostTitle + "\n" + ShowName + "\n" + SeasonEp);
                }

                return show;
            }



            #region === ssTv FUNCTIONS ===

            #region ===== ssTv Object =====

            public struct ssTv
            {
                public string ShowName { get; set; }
                public string SeasonEp { get; set; }
                public string PostTitle { get; set; }
                public string PostURL { get; set; }
                public string RapidGator { get; set; }
                public bool LinksChecked { get; set; }
                public bool InCollection { get; set; }
                public DateTime DateAdded { get; set; }
                public DateTime DateChecked { get; set; }
                public int LinksOnline { get; set; }
                public bool Flagged { get; set; }
            }





            #endregion


            #region ===== ssTv SQLite : Return =====

            public ssTv Return_Object_From_SceneSourceTV(string WhereClause = "[ShowName] = ''", string DbFile = "")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\ssTv.sqlite"; }
                string SelectLine = "Select [ShowName], [SeasonEp], [PostTitle], [PostURL], [RapidGator], [LinksChecked], [InCollection], [DateAdded], [DateChecked], [LinksOnline], [Flagged] From [SceneSourceTV] ";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
                ssTv returnObject = new ssTv();
                int i = 0;
                string Value = "";
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.SeasonEp = ret["SeasonEp"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToBool();
                        returnObject.InCollection = ret["InCollection"].ToBool();
                        returnObject.DateAdded = ret["DateAdded"].ToDateTime();
                        returnObject.DateChecked = ret["DateChecked"].ToDateTime();

                        string linksonline = ret["LinksOnline"].ToString();
                        returnObject.LinksOnline = 0;
                        if (linksonline != "") { returnObject.LinksOnline = linksonline.ToInt(); }

                        returnObject.Flagged = ret["Flagged"].ToBool();
                    }
                }

                return returnObject;
            }

            public List<ssTv> Return_ssTv_List(string WhereClause = "", string DbFile = "", string TableName = "[SceneSourceTV]")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\ssTv.sqlite"; }
                string SelectLine = "Select * From " + TableName;

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

                List<ssTv> ReturnList = new List<ssTv>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        ssTv returnObject = new ssTv();

                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.SeasonEp = ret["SeasonEp"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToBool();
                        returnObject.InCollection = ret["InCollection"].ToBool();
                        returnObject.DateAdded = ret["DateAdded"].ToDateTime();
                        returnObject.DateChecked = ret["DateChecked"].ToDateTime();

                        string linksonline = ret["LinksOnline"].ToString();
                        returnObject.LinksOnline = 0;
                        if (linksonline != "") { returnObject.LinksOnline = linksonline.ToInt(); }

                        returnObject.Flagged = ret["Flagged"].ToBool();

                        ReturnList.Add(returnObject);
                    }
                }

                return ReturnList;
            }

            public DataTable Return_DataTable_From_SceneSourceTV(string DbFile)
            {
                string SelectLine = "Select [ShowName], [SeasonEp], [PostTitle], [PostURL], [RapidGator], [LinksChecked], [InCollection], [DateAdded], [DateChecked], [LinksOnline], [Flagged] From [SceneSourceTV]";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                return ReturnTable;
            }


            #endregion
            #region ===== ssTv SQLite : Update Insert =====

            public bool ssTv_Insert(ssTv inObject, string DbFile = "")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\ssTv.sqlite"; }
                string InsertLine = "Insert Into [SceneSourceTV] (ShowName, SeasonEp, PostTitle, PostURL, RapidGator, LinksChecked, InCollection, DateAdded, DateChecked, LinksOnline, Flagged) values ('" + inObject.ShowName + "', '" + inObject.SeasonEp + "', '" + inObject.PostTitle + "', '" + inObject.PostURL + "', '" + inObject.RapidGator + "', '" + inObject.LinksChecked + "', '" + inObject.InCollection + "', '" + inObject.DateAdded + "', '" + inObject.DateChecked + "', '" + inObject.LinksOnline + "', '" + inObject.Flagged + "')";
                bool Inserted = sqlite.Execute(DbFile, InsertLine);
                if (!Inserted) { ahk.MsgBox("Inserted Into [SceneSourceTV] = " + Inserted.ToString()); }
                return Inserted;
            }

            public bool ssTv_Update(ssTv inObject, string DbFile = "")
            {
                //string UpdateLine = "Update [SceneSourceTV] set ShowName = '" + inObject.ShowName + "', SeasonEp = '" + inObject.SeasonEp + "', PostTitle = '" + inObject.PostTitle + "', PostURL = '" + inObject.PostURL + "', RapidGator = '" + inObject.RapidGator + "', LinksChecked = '" + inObject.LinksChecked + "', InCollection = '" + inObject.InCollection + "', DateAdded = '" + inObject.DateAdded + "', DateChecked = '" + inObject.DateChecked + "', LinksOnline = '" + inObject.LinksOnline + "', Flagged = '" + inObject.Flagged + "' WHERE [Item] = 'Value' ";
                string UpdateLine = "Update [SceneSourceTV] set ";


                if (inObject.ShowName != null) { UpdateLine = UpdateLine + "[ShowName] = '" + inObject.ShowName + "',"; }
                if (inObject.SeasonEp != null) { UpdateLine = UpdateLine + "[SeasonEp] = '" + inObject.SeasonEp + "',"; }
                if (inObject.PostTitle != null) { UpdateLine = UpdateLine + "[PostTitle] = '" + inObject.PostTitle + "',"; }
                if (inObject.PostURL != null) { UpdateLine = UpdateLine + "[PostURL] = '" + inObject.PostURL + "',"; }
                if (inObject.RapidGator != null) { UpdateLine = UpdateLine + "[RapidGator] = '" + inObject.RapidGator + "',"; }
                UpdateLine = UpdateLine + "[LinksChecked] = '" + inObject.LinksChecked + "',";
                UpdateLine = UpdateLine + "[InCollection] = '" + inObject.InCollection + "',";
                if (inObject.DateAdded != null) { UpdateLine = UpdateLine + "[DateAdded] = '" + inObject.DateAdded + "',"; }
                if (inObject.DateChecked != null) { UpdateLine = UpdateLine + "[DateChecked] = '" + inObject.DateChecked + "',"; }
                UpdateLine = UpdateLine + "[LinksOnline] = '" + inObject.LinksOnline + "',";
                UpdateLine = UpdateLine + "[Flagged] = '" + inObject.Flagged + "',";

                UpdateLine = ahk.TrimLast(UpdateLine, 1);
                UpdateLine = UpdateLine + " WHERE [PostURL] = '" + inObject + "'"; // DEFINE CONDITION HERE !!!

                bool Updated = sqlite.Execute(DbFile, UpdateLine);
                return Updated;
            }

            public bool ssTv_UpdateInsert(ssTv obj, string DbFile = "")
            {

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\ssTv.sqlite"; }
                bool Updated = ssTv_Update(obj, DbFile);  // try to update record first
                if (!Updated) { Updated = ssTv_Insert(obj, DbFile); }  // if unable to update, insert new record
                return Updated;
            }


            #endregion
            #region ===== ssTv DataTable =====

            public DataTable Return_ssTv_DataTable(string DbFile = "", string TableName = "ssTv", string WhereClause = "", bool Debug = false)
            {

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\ssTv.sqlite"; }
                string SelectLine = "Select * From [SceneSourceTV]";

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }

                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


                DataTable table = new DataTable();
                table.Columns.Add("ShowName", typeof(string));
                table.Columns.Add("SeasonEp", typeof(string));
                table.Columns.Add("PostTitle", typeof(string));
                table.Columns.Add("PostURL", typeof(string));
                table.Columns.Add("RapidGator", typeof(string));
                table.Columns.Add("LinksChecked", typeof(string));
                table.Columns.Add("InCollection", typeof(string));
                table.Columns.Add("DateAdded", typeof(string));
                table.Columns.Add("DateChecked", typeof(string));
                table.Columns.Add("LinksOnline", typeof(string));
                table.Columns.Add("Flagged", typeof(string));

                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        ssTv returnObject = new ssTv();

                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.SeasonEp = ret["SeasonEp"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToBool();
                        returnObject.InCollection = ret["InCollection"].ToBool();
                        returnObject.DateAdded = ret["DateAdded"].ToDateTime();
                        returnObject.DateChecked = ret["DateChecked"].ToDateTime();

                        string linksonline = ret["LinksOnline"].ToString();
                        returnObject.LinksOnline = 0;
                        if (linksonline != "") { returnObject.LinksOnline = linksonline.ToInt(); }

                        returnObject.Flagged = ret["Flagged"].ToBool();

                        table.Rows.Add(returnObject.ShowName, returnObject.SeasonEp, returnObject.PostTitle, returnObject.PostURL, returnObject.RapidGator, returnObject.LinksChecked, returnObject.InCollection, returnObject.DateAdded, returnObject.DateChecked, returnObject.LinksOnline, returnObject.Flagged);
                    }
                }

                return table;
            }

            public DataTable Create_SceneSourceTV_DataTable(ssTv inObject)
            {
                DataTable table = new DataTable();
                table.Columns.Add("ShowName", typeof(string));
                table.Columns.Add("SeasonEp", typeof(string));
                table.Columns.Add("PostTitle", typeof(string));
                table.Columns.Add("PostURL", typeof(string));
                table.Columns.Add("RapidGator", typeof(string));
                table.Columns.Add("LinksChecked", typeof(string));
                table.Columns.Add("InCollection", typeof(string));
                table.Columns.Add("DateAdded", typeof(string));
                table.Columns.Add("DateChecked", typeof(string));
                table.Columns.Add("LinksOnline", typeof(string));
                table.Columns.Add("Flagged", typeof(string));

                table.Rows.Add(inObject.ShowName, inObject.SeasonEp, inObject.PostTitle, inObject.PostURL, inObject.RapidGator, inObject.LinksChecked, inObject.InCollection, inObject.DateAdded, inObject.DateChecked, inObject.LinksOnline, inObject.Flagged);
                return table;
            }


            #endregion
            #region ===== ssTv DataGridView =====

            public void HideShow_SceneSourceTV_Columns(DataGridView dv)
            {

                try { dv.Columns["ShowName"].Visible = true; } catch { }
                try { dv.Columns["SeasonEp"].Visible = true; } catch { }
                try { dv.Columns["PostTitle"].Visible = true; } catch { }
                try { dv.Columns["PostURL"].Visible = true; } catch { }
                try { dv.Columns["RapidGator"].Visible = true; } catch { }
                try { dv.Columns["LinksChecked"].Visible = true; } catch { }
                try { dv.Columns["InCollection"].Visible = true; } catch { }
                try { dv.Columns["DateAdded"].Visible = true; } catch { }
                try { dv.Columns["DateChecked"].Visible = true; } catch { }
                try { dv.Columns["LinksOnline"].Visible = true; } catch { }
                try { dv.Columns["Flagged"].Visible = true; } catch { }
            }
            public void Enable_SceneSourceTV_Columns(DataGridView dv)
            {

                try { dv.Columns["ShowName"].ReadOnly = true; } catch { }
                try { dv.Columns["SeasonEp"].ReadOnly = true; } catch { }
                try { dv.Columns["PostTitle"].ReadOnly = true; } catch { }
                try { dv.Columns["PostURL"].ReadOnly = true; } catch { }
                try { dv.Columns["RapidGator"].ReadOnly = true; } catch { }
                try { dv.Columns["LinksChecked"].ReadOnly = true; } catch { }
                try { dv.Columns["InCollection"].ReadOnly = true; } catch { }
                try { dv.Columns["DateAdded"].ReadOnly = true; } catch { }
                try { dv.Columns["DateChecked"].ReadOnly = true; } catch { }
                try { dv.Columns["LinksOnline"].ReadOnly = true; } catch { }
                try { dv.Columns["Flagged"].ReadOnly = true; } catch { }
            }

            #endregion
            #region ===== ssTv SQL Functions =====

            // Return ssTv SQL Connection String
            public SqlConnection ssTv_Conn()
            {
                // populate sql connection
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMLucidMedia"].ConnectionString);
                // SqlConnection Conn = new SqlConnection("Server=188.168.188.88;DataBase=LucidMedia;Uid=lucidm;Pwd=pass");
                return conn;
            }

            // Return ssTv TableName (Full Path)
            public string ssTv_TableName()
            {
                // populate to return full sql table name
                return "[LucidMedia].[dbo].[SceneSourceTV]";
            }

            // Generate SQL Table
            public bool ssTv_CreateSQLTable()
            {
                SqlConnection Conn = ssTv_Conn();
                string CreateTableLine = "CREATE TABLE [SceneSourceTV](";
                CreateTableLine = CreateTableLine + "[ShowName] [int] IDENTITY(1,1) NOT NULL,";
                CreateTableLine = CreateTableLine + "[SeasonEp] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[PostTitle] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[PostURL] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[RapidGator] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[LinksChecked] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[InCollection] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[DateAdded] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[DateChecked] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[LinksOnline] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Flagged] [varchar](max) NOT NULL";
                CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                return false;
            }

            public bool ssTv_InsertSQL(ssTv obj)
            {
                SqlConnection Con = ssTv_Conn();
                string SQLLine = "Insert Into " + ssTv_TableName() + " (ShowName, SeasonEp, PostTitle, PostURL, RapidGator, LinksChecked, InCollection, DateAdded, DateChecked, LinksOnline, Flagged) VALUES (@ShowName, @SeasonEp, @PostTitle, @PostURL, @RapidGator, @LinksChecked, @InCollection, @DateAdded, @DateChecked, @LinksOnline, @Flagged)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.ShowName == null) { obj.ShowName = ""; }
                if (obj.SeasonEp == null) { obj.SeasonEp = ""; }
                if (obj.PostTitle == null) { obj.PostTitle = ""; }
                if (obj.PostURL == null) { obj.PostURL = ""; }
                if (obj.RapidGator == null) { obj.RapidGator = ""; }
                //if (obj.LinksChecked == null) { obj.LinksChecked = ""; }
                //if (obj.InCollection == null) { obj.InCollection = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = DateTime.Now; }
                if (obj.DateChecked == null) { obj.DateChecked = DateTime.Now; }
                //if (obj.LinksOnline == null) { obj.LinksOnline = ""; }
                //if (obj.Flagged == null) { obj.Flagged = ""; }
                cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                cmd2.Parameters.AddWithValue(@"SeasonEp", obj.SeasonEp.ToString());
                cmd2.Parameters.AddWithValue(@"PostTitle", obj.PostTitle.ToString());
                cmd2.Parameters.AddWithValue(@"PostURL", obj.PostURL.ToString());
                cmd2.Parameters.AddWithValue(@"RapidGator", obj.RapidGator.ToString());
                cmd2.Parameters.AddWithValue(@"LinksChecked", obj.LinksChecked.ToString());
                cmd2.Parameters.AddWithValue(@"InCollection", obj.InCollection.ToString());
                cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded.ToString());
                cmd2.Parameters.AddWithValue(@"DateChecked", obj.DateChecked.ToString());
                cmd2.Parameters.AddWithValue(@"LinksOnline", obj.LinksOnline.ToString());
                cmd2.Parameters.AddWithValue(@"Flagged", obj.Flagged.ToString());
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        ssTv_InsertSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        ssTv_InsertSQL(obj);
                    }
                    else
                    {
                        ahk.MsgBox(ex.ToString());
                        return false;
                    }
                }
                Con.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool ssTv_UpdateSQL(ssTv obj)
            {
                SqlConnection Conn = ssTv_Conn();
                string SQLLine = "Update " + ssTv_TableName() + " SET ShowName = @ShowName, SeasonEp = @SeasonEp, PostTitle = @PostTitle, RapidGator = @RapidGator, LinksChecked = @LinksChecked, InCollection = @InCollection, DateChecked = @DateChecked, LinksOnline = @LinksOnline, Flagged = @Flagged WHERE PostURL = @PostURL";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.ShowName == null) { obj.ShowName = ""; }
                if (obj.SeasonEp == null) { obj.SeasonEp = ""; }
                if (obj.PostTitle == null) { obj.PostTitle = ""; }
                if (obj.PostURL == null) { obj.PostURL = ""; }
                if (obj.RapidGator == null) { obj.RapidGator = ""; }
                //if (obj.LinksChecked == null) { obj.LinksChecked = ""; }
                //if (obj.InCollection == null) { obj.InCollection = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = DateTime.Now; }
                if (obj.DateChecked == null) { obj.DateChecked = DateTime.Now; }
                //if (obj.LinksOnline == null) { obj.LinksOnline = ""; }
                //if (obj.Flagged == null) { obj.Flagged = ""; }
                cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                cmd2.Parameters.AddWithValue(@"SeasonEp", obj.SeasonEp.ToString());
                cmd2.Parameters.AddWithValue(@"PostTitle", obj.PostTitle.ToString());
                cmd2.Parameters.AddWithValue(@"PostURL", obj.PostURL.ToString());
                cmd2.Parameters.AddWithValue(@"RapidGator", obj.RapidGator.ToString());
                cmd2.Parameters.AddWithValue(@"LinksChecked", obj.LinksChecked.ToString());
                cmd2.Parameters.AddWithValue(@"InCollection", obj.InCollection.ToString());
                //cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded.ToString());
                cmd2.Parameters.AddWithValue(@"DateChecked", obj.DateChecked.ToString());
                cmd2.Parameters.AddWithValue(@"LinksOnline", obj.LinksOnline.ToString());
                cmd2.Parameters.AddWithValue(@"Flagged", obj.Flagged.ToString());
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        ssTv_UpdateSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        ssTv_UpdateSQL(obj);
                    }
                    else
                    {
                        ahk.MsgBox(ex.ToString());
                        return false;
                    }
                }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool ssTv_UpdateLinks(ssTv obj)
            {
                SqlConnection Conn = ssTv_Conn();
                string SQLLine = "Update " + ssTv_TableName() + " SET ShowName = @ShowName, SeasonEp = @SeasonEp, PostTitle = @PostTitle, RapidGator = @RapidGator, LinksChecked = @LinksChecked, InCollection = @InCollection, DateChecked = @DateChecked, LinksOnline = @LinksOnline, Flagged = @Flagged WHERE PostURL = @PostURL";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.ShowName == null) { obj.ShowName = ""; }
                if (obj.SeasonEp == null) { obj.SeasonEp = ""; }
                if (obj.PostTitle == null) { obj.PostTitle = ""; }
                if (obj.PostURL == null) { obj.PostURL = ""; }
                if (obj.RapidGator == null) { obj.RapidGator = ""; }
                //if (obj.LinksChecked == null) { obj.LinksChecked = ""; }
                //if (obj.InCollection == null) { obj.InCollection = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = DateTime.Now; }
                if (obj.DateChecked == null) { obj.DateChecked = DateTime.Now; }
                //if (obj.LinksOnline == null) { obj.LinksOnline = ""; }
                //if (obj.Flagged == null) { obj.Flagged = ""; }
                cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                cmd2.Parameters.AddWithValue(@"SeasonEp", obj.SeasonEp.ToString());
                cmd2.Parameters.AddWithValue(@"PostTitle", obj.PostTitle.ToString());
                cmd2.Parameters.AddWithValue(@"PostURL", obj.PostURL.ToString());
                cmd2.Parameters.AddWithValue(@"RapidGator", obj.RapidGator.ToString());
                cmd2.Parameters.AddWithValue(@"LinksChecked", obj.LinksChecked.ToString());
                cmd2.Parameters.AddWithValue(@"InCollection", obj.InCollection.ToString());
                //cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded.ToString());
                cmd2.Parameters.AddWithValue(@"DateChecked", obj.DateChecked.ToString());
                cmd2.Parameters.AddWithValue(@"LinksOnline", obj.LinksOnline.ToString());
                cmd2.Parameters.AddWithValue(@"Flagged", obj.Flagged.ToString());
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        ssTv_UpdateLinks(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        ssTv_UpdateLinks(obj);
                    }
                    else
                    {
                        ahk.MsgBox(ex.ToString());
                        return false;
                    }
                }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool UpdateShow(ssTv obj)
            {
                SqlConnection Conn = ssTv_Conn();

                obj.DateChecked = DateTime.Now;
                obj.DateAdded = DateTime.Now;

                bool Updated = ssTv_UpdateSQL(obj);  // try to update record first
                if (!Updated) { Updated = ssTv_InsertSQL(obj); }  // if unable to update, insert new record
                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool ssTv_UpdateIfPopulated(ssTv obj, string ID = "")
            {
                SqlConnection Conn = ssTv_Conn();
                string SQLcmd = "Update " + ssTv_TableName() + " SET ";
                if (obj.ShowName != null) { SQLcmd = SQLcmd + " ShowName = @ShowName,"; }
                if (obj.SeasonEp != null) { SQLcmd = SQLcmd + " SeasonEp = @SeasonEp,"; }
                if (obj.PostTitle != null) { SQLcmd = SQLcmd + " PostTitle = @PostTitle,"; }
                if (obj.PostURL != null) { SQLcmd = SQLcmd + " PostURL = @PostURL,"; }
                if (obj.RapidGator != null) { SQLcmd = SQLcmd + " RapidGator = @RapidGator,"; }
                if (obj.LinksChecked != null) { SQLcmd = SQLcmd + " LinksChecked = @LinksChecked,"; }
                if (obj.InCollection != null) { SQLcmd = SQLcmd + " InCollection = @InCollection,"; }
                if (obj.DateAdded != null) { SQLcmd = SQLcmd + " DateAdded = @DateAdded,"; }
                if (obj.DateChecked != null) { SQLcmd = SQLcmd + " DateChecked = @DateChecked,"; }
                if (obj.LinksOnline != null) { SQLcmd = SQLcmd + " LinksOnline = @LinksOnline,"; }
                if (obj.Flagged != null) { SQLcmd = SQLcmd + " Flagged = @Flagged,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ID = @ID";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.ShowName != null) { cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName); }
                if (obj.SeasonEp != null) { cmd2.Parameters.AddWithValue(@"SeasonEp", obj.SeasonEp); }
                if (obj.PostTitle != null) { cmd2.Parameters.AddWithValue(@"PostTitle", obj.PostTitle); }
                if (obj.PostURL != null) { cmd2.Parameters.AddWithValue(@"PostURL", obj.PostURL); }
                if (obj.RapidGator != null) { cmd2.Parameters.AddWithValue(@"RapidGator", obj.RapidGator); }
                if (obj.LinksChecked != null) { cmd2.Parameters.AddWithValue(@"LinksChecked", obj.LinksChecked); }
                if (obj.InCollection != null) { cmd2.Parameters.AddWithValue(@"InCollection", obj.InCollection); }
                if (obj.DateAdded != null) { cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded); }
                if (obj.DateChecked != null) { cmd2.Parameters.AddWithValue(@"DateChecked", obj.DateChecked); }
                if (obj.LinksOnline != null) { cmd2.Parameters.AddWithValue(@"LinksOnline", obj.LinksOnline); }
                if (obj.Flagged != null) { cmd2.Parameters.AddWithValue(@"Flagged", obj.Flagged); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }


            // returns sql command from tv server db
            public string ssTV_SQL(string Command)
            {
                SqlConnection Conn = ssTv_Conn();
                string SelectLine = Command;

                return sql.SQL_Return_Value(Conn, Command);
            }

            public ssTv ssTv_ReturnSQL(string Where = "")
            {
                SqlConnection Conn = ssTv_Conn();
                string SelectLine = "Select [ShowName],[SeasonEp],[PostTitle],[PostURL],[RapidGator],[LinksChecked],[InCollection],[DateAdded],[DateChecked],[LinksOnline],[Flagged] From " + ssTv_TableName() + " " + Where;
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                ssTv returnObject = new ssTv();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.SeasonEp = ret["SeasonEp"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToBool();
                        returnObject.InCollection = ret["InCollection"].ToBool();
                        returnObject.DateAdded = ret["DateAdded"].ToDateTime();
                        returnObject.DateChecked = ret["DateChecked"].ToDateTime();

                        string linksonline = ret["LinksOnline"].ToString();
                        returnObject.LinksOnline = 0;
                        if (linksonline != "") { returnObject.LinksOnline = linksonline.ToInt(); }

                        returnObject.Flagged = ret["Flagged"].ToBool();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public List<ssTv> ssTv_ReturnSQLList(string Command = "")
            {
                if (Command == "") { Command = "Select * From [lucidmedia].[lucidmethod].[SceneSourceTV]"; }
                SqlConnection Conn = ssTv_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<ssTv> ReturnList = new List<ssTv>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        ssTv returnObject = new ssTv();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.SeasonEp = ret["SeasonEp"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToBool();
                        returnObject.InCollection = ret["InCollection"].ToBool();
                        returnObject.DateAdded = ret["DateAdded"].ToDateTime();
                        returnObject.DateChecked = ret["DateChecked"].ToDateTime();

                        string linksonline = ret["LinksOnline"].ToString();
                        returnObject.LinksOnline = 0;
                        if (linksonline != "") { returnObject.LinksOnline = linksonline.ToInt(); }

                        returnObject.Flagged = ret["Flagged"].ToBool();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }

            public bool ssTv_SQL_to_SQLite(string SqliteDBPath = @"\Db\ssTv.sqlite")
            {
                string SaveFile = SqliteDBPath;
                if (SqliteDBPath == @"\Db\ssTv.sqlite")
                {
                    ahk.FileCreateDir(ahk.AppDir() + @"\Db");
                    SaveFile = ahk.AppDir() + @"\Db\ssTv.sqlite";
                }

                sb.StatusBar("Copying SQL Db to " + SaveFile + "...");
                sqlite.SQLTable_To_NewSQLiteTable(ssTv_Conn(), "SceneSourceTV", "SceneSourceTV", SaveFile, "", false, false, false);
                sb.StatusBar("FINISHED Copying SQL Db to " + SaveFile);

                if (File.Exists(SaveFile)) { return true; } else { return false; }
            }



            public ssTv ShowByPostText(string PostTitle)
            {
                SqlConnection Conn = ssTv_Conn();
                string SelectLine = "Select [ShowName],[SeasonEp],[PostTitle],[PostURL],[RapidGator],[LinksChecked],[InCollection],[DateAdded],[DateChecked],[LinksOnline],[Flagged] From " + ssTv_TableName() + " WHERE [PostTitle] = '" + PostTitle + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                ssTv returnObject = new ssTv();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.SeasonEp = ret["SeasonEp"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToBool();
                        returnObject.InCollection = ret["InCollection"].ToBool();
                        returnObject.DateAdded = ret["DateAdded"].ToDateTime();
                        returnObject.DateChecked = ret["DateChecked"].ToDateTime();

                        string linksonline = ret["LinksOnline"].ToString();
                        returnObject.LinksOnline = 0;
                        if (linksonline != "") { returnObject.LinksOnline = linksonline.ToInt(); }

                        returnObject.Flagged = ret["Flagged"].ToBool();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public ssTv ShowByURL(string PostURL)
            {
                SqlConnection Conn = ssTv_Conn();
                string SelectLine = "Select [ShowName],[SeasonEp],[PostTitle],[PostURL],[RapidGator],[LinksChecked],[InCollection],[DateAdded],[DateChecked],[LinksOnline],[Flagged] From " + ssTv_TableName() + " WHERE [PostURL] = '" + PostURL + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                ssTv returnObject = new ssTv();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.SeasonEp = ret["SeasonEp"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToBool();
                        returnObject.InCollection = ret["InCollection"].ToBool();
                        returnObject.DateAdded = ret["DateAdded"].ToDateTime();
                        returnObject.DateChecked = ret["DateChecked"].ToDateTime();

                        string linksonline = ret["LinksOnline"].ToString();
                        returnObject.LinksOnline = 0;
                        if (linksonline != "") { returnObject.LinksOnline = linksonline.ToInt(); }

                        returnObject.Flagged = ret["Flagged"].ToBool();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public List<ssTv> ShowList(bool UncheckedLinks = false, bool RecheckLinks = false)  // list of show entries from SceneSource Table with Links Found
            {
                string Command = "Select * From " + ssTv_TableName() + " where PostURL like '%scnsrc.me%' and LinksOnline > 0  order by DateAdded desc";

                if (UncheckedLinks) { Command = "select TOP (500) * FROM " + ssTv_TableName() + " where PostURL like '%scnsrc.me%' and LinksChecked = 0 order by DateAdded desc"; }
                if (RecheckLinks) { Command = "select * FROM " + ssTv_TableName() + " where PostURL like '%scnsrc.me%' and LinksChecked != 0 order by DateAdded desc"; }

                SqlConnection Conn = ssTv_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<ssTv> ReturnList = new List<ssTv>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        ssTv returnObject = new ssTv();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.SeasonEp = ret["SeasonEp"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToBool();
                        returnObject.InCollection = ret["InCollection"].ToBool();
                        returnObject.DateAdded = ret["DateAdded"].ToDateTime();
                        returnObject.DateChecked = ret["DateChecked"].ToDateTime();

                        string linksonline = ret["LinksOnline"].ToString();
                        returnObject.LinksOnline = 0;
                        if (linksonline != "") { returnObject.LinksOnline = linksonline.ToInt(); }

                        returnObject.Flagged = ret["Flagged"].ToBool();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }


            #endregion

            #endregion


            #region === ssMovie FUNCTIONS ===

            #region ===== ssMovie Object =====

            public struct ssMovie
            {
                public string MovieName { get; set; }
                public string IMDbID { get; set; }
                public string Year { get; set; }
                public string PostTitle { get; set; }
                public string PostURL { get; set; }
                public string ImageURL { get; set; }
                public string RapidGator { get; set; }
                public string LinksChecked { get; set; }
                public string InCollection { get; set; }
                public string DateAdded { get; set; }
                public string Cateogry { get; set; }
            }
            public ssMovie Return_ssMovie(string MovieName = "", string IMDbID = "", string Year = "", string PostTitle = "", string PostURL = "", string ImageURL = "", string RapidGator = "", string LinksChecked = "", string InCollection = "", string DateAdded = "", string Cateogry = "")
            {
                ssMovie obj = new ssMovie();
                obj.MovieName = MovieName;
                obj.IMDbID = IMDbID;
                obj.Year = Year;
                obj.PostTitle = PostTitle;
                obj.PostURL = PostURL;
                obj.ImageURL = ImageURL;
                obj.RapidGator = RapidGator;
                obj.LinksChecked = LinksChecked;
                obj.InCollection = InCollection;
                obj.DateAdded = DateAdded;
                obj.Cateogry = Cateogry;

                return obj;
            }

            //  Fix illegal characters before Sql/Sqlite Db Inserts
            public ssMovie ssMovie_FixChars(ssMovie ToFix)
            {
                ssMovie Fixed = new ssMovie();

                Fixed.MovieName = ToFix.MovieName.Replace("'", "''");
                Fixed.IMDbID = ToFix.IMDbID.Replace("'", "''");
                Fixed.Year = ToFix.Year.Replace("'", "''");
                Fixed.PostTitle = ToFix.PostTitle.Replace("'", "''");
                Fixed.PostURL = ToFix.PostURL.Replace("'", "''");
                Fixed.ImageURL = ToFix.ImageURL.Replace("'", "''");
                Fixed.RapidGator = ToFix.RapidGator.Replace("'", "''");
                Fixed.LinksChecked = ToFix.LinksChecked.Replace("'", "''");
                Fixed.InCollection = ToFix.InCollection.Replace("'", "''");
                Fixed.DateAdded = ToFix.DateAdded.Replace("'", "''");
                Fixed.Cateogry = ToFix.Cateogry.Replace("'", "''");

                return Fixed;
            }

            // Compare two objects to see if they have identical values
            public bool ssMovie_Changed(ssMovie OldVal, ssMovie NewVal)
            {
                ssMovie diff = new ssMovie();
                List<string> diffList = new List<string>();
                bool different = false;
                if (OldVal.MovieName == null) { OldVal.MovieName = ""; }
                if (NewVal.MovieName == null) { NewVal.MovieName = ""; }
                if (OldVal.MovieName != NewVal.MovieName) { different = true; }
                if (OldVal.IMDbID == null) { OldVal.IMDbID = ""; }
                if (NewVal.IMDbID == null) { NewVal.IMDbID = ""; }
                if (OldVal.IMDbID != NewVal.IMDbID) { different = true; }
                if (OldVal.Year == null) { OldVal.Year = ""; }
                if (NewVal.Year == null) { NewVal.Year = ""; }
                if (OldVal.Year != NewVal.Year) { different = true; }
                if (OldVal.PostTitle == null) { OldVal.PostTitle = ""; }
                if (NewVal.PostTitle == null) { NewVal.PostTitle = ""; }
                if (OldVal.PostTitle != NewVal.PostTitle) { different = true; }
                if (OldVal.PostURL == null) { OldVal.PostURL = ""; }
                if (NewVal.PostURL == null) { NewVal.PostURL = ""; }
                if (OldVal.PostURL != NewVal.PostURL) { different = true; }
                if (OldVal.ImageURL == null) { OldVal.ImageURL = ""; }
                if (NewVal.ImageURL == null) { NewVal.ImageURL = ""; }
                if (OldVal.ImageURL != NewVal.ImageURL) { different = true; }
                if (OldVal.RapidGator == null) { OldVal.RapidGator = ""; }
                if (NewVal.RapidGator == null) { NewVal.RapidGator = ""; }
                if (OldVal.RapidGator != NewVal.RapidGator) { different = true; }
                if (OldVal.LinksChecked == null) { OldVal.LinksChecked = ""; }
                if (NewVal.LinksChecked == null) { NewVal.LinksChecked = ""; }
                if (OldVal.LinksChecked != NewVal.LinksChecked) { different = true; }
                if (OldVal.InCollection == null) { OldVal.InCollection = ""; }
                if (NewVal.InCollection == null) { NewVal.InCollection = ""; }
                if (OldVal.InCollection != NewVal.InCollection) { different = true; }
                if (OldVal.DateAdded == null) { OldVal.DateAdded = ""; }
                if (NewVal.DateAdded == null) { NewVal.DateAdded = ""; }
                if (OldVal.DateAdded != NewVal.DateAdded) { different = true; }
                if (OldVal.Cateogry == null) { OldVal.Cateogry = ""; }
                if (NewVal.Cateogry == null) { NewVal.Cateogry = ""; }
                if (OldVal.Cateogry != NewVal.Cateogry) { different = true; }
                return different;
            }

            // Returns object containing the new values different from the old values in object comparison
            public ssMovie ssMovie_Diff(ssMovie OldVal, ssMovie NewVal)
            {
                ssMovie diff = new ssMovie();
                if (OldVal.MovieName != NewVal.MovieName) { diff.MovieName = NewVal.MovieName; }
                if (OldVal.IMDbID != NewVal.IMDbID) { diff.IMDbID = NewVal.IMDbID; }
                if (OldVal.Year != NewVal.Year) { diff.Year = NewVal.Year; }
                if (OldVal.PostTitle != NewVal.PostTitle) { diff.PostTitle = NewVal.PostTitle; }
                if (OldVal.PostURL != NewVal.PostURL) { diff.PostURL = NewVal.PostURL; }
                if (OldVal.ImageURL != NewVal.ImageURL) { diff.ImageURL = NewVal.ImageURL; }
                if (OldVal.RapidGator != NewVal.RapidGator) { diff.RapidGator = NewVal.RapidGator; }
                if (OldVal.LinksChecked != NewVal.LinksChecked) { diff.LinksChecked = NewVal.LinksChecked; }
                if (OldVal.InCollection != NewVal.InCollection) { diff.InCollection = NewVal.InCollection; }
                if (OldVal.DateAdded != NewVal.DateAdded) { diff.DateAdded = NewVal.DateAdded; }
                if (OldVal.Cateogry != NewVal.Cateogry) { diff.Cateogry = NewVal.Cateogry; }
                return diff;
            }

            // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
            public List<string> ssMovie_DiffList(ssMovie OldVal, ssMovie NewVal)
            {
                List<string> diffList = new List<string>();
                if (OldVal.MovieName != NewVal.MovieName) { diffList.Add("Changed MovieName Value From " + OldVal.MovieName + " To " + NewVal.MovieName); }
                if (OldVal.IMDbID != NewVal.IMDbID) { diffList.Add("Changed IMDbID Value From " + OldVal.IMDbID + " To " + NewVal.IMDbID); }
                if (OldVal.Year != NewVal.Year) { diffList.Add("Changed Year Value From " + OldVal.Year + " To " + NewVal.Year); }
                if (OldVal.PostTitle != NewVal.PostTitle) { diffList.Add("Changed PostTitle Value From " + OldVal.PostTitle + " To " + NewVal.PostTitle); }
                if (OldVal.PostURL != NewVal.PostURL) { diffList.Add("Changed PostURL Value From " + OldVal.PostURL + " To " + NewVal.PostURL); }
                if (OldVal.ImageURL != NewVal.ImageURL) { diffList.Add("Changed ImageURL Value From " + OldVal.ImageURL + " To " + NewVal.ImageURL); }
                if (OldVal.RapidGator != NewVal.RapidGator) { diffList.Add("Changed RapidGator Value From " + OldVal.RapidGator + " To " + NewVal.RapidGator); }
                if (OldVal.LinksChecked != NewVal.LinksChecked) { diffList.Add("Changed LinksChecked Value From " + OldVal.LinksChecked + " To " + NewVal.LinksChecked); }
                if (OldVal.InCollection != NewVal.InCollection) { diffList.Add("Changed InCollection Value From " + OldVal.InCollection + " To " + NewVal.InCollection); }
                if (OldVal.DateAdded != NewVal.DateAdded) { diffList.Add("Changed DateAdded Value From " + OldVal.DateAdded + " To " + NewVal.DateAdded); }
                if (OldVal.Cateogry != NewVal.Cateogry) { diffList.Add("Changed Cateogry Value From " + OldVal.Cateogry + " To " + NewVal.Cateogry); }
                return diffList;
            }


            #endregion
            #region ===== ssMovie SQLite : Return =====

            public ssMovie Return_Object_From_SceneSourceMovies(string DbFile, string WhereClause = "[MovieName] = ''")
            {
                string SelectLine = "Select [MovieName], [IMDbID], [Year], [PostTitle], [PostURL], [ImageURL], [RapidGator], [LinksChecked], [InCollection], [DateAdded], [Cateogry] From [SceneSourceTV] ";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
                ssMovie returnObject = new ssMovie();
                int i = 0;
                string Value = "";
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.MovieName = ret["MovieName"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.Year = ret["Year"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.ImageURL = ret["ImageURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToString();
                        returnObject.InCollection = ret["InCollection"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.Cateogry = ret["Cateogry"].ToString();
                    }
                }

                return returnObject;
            }

            public List<ssMovie> Return_ssMovie_List(string DbFile, string TableName = "[SceneSourceTV]", string WhereClause = "")
            {
                string SelectLine = "Select * From " + TableName;

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

                List<ssMovie> ReturnList = new List<ssMovie>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        ssMovie returnObject = new ssMovie();

                        returnObject.MovieName = ret["MovieName"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.Year = ret["Year"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.ImageURL = ret["ImageURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToString();
                        returnObject.InCollection = ret["InCollection"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.Cateogry = ret["Cateogry"].ToString();

                        ReturnList.Add(returnObject);
                    }
                }

                return ReturnList;
            }

            public DataTable Return_DataTable_From_SceneSourceMovies(string DbFile)
            {
                string SelectLine = "Select [MovieName], [IMDbID], [Year], [PostTitle], [PostURL], [ImageURL], [RapidGator], [LinksChecked], [InCollection], [DateAdded], [Cateogry] From [SceneSourceTV]";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                return ReturnTable;
            }


            #endregion
            #region ===== ssMovie SQLite : Update Insert =====

            public bool ssMovie_Insert(string DbFile, ssMovie inObject)
            {
                string InsertLine = "Insert Into [SceneSourceTV] (MovieName, IMDbID, Year, PostTitle, PostURL, ImageURL, RapidGator, LinksChecked, InCollection, DateAdded, Cateogry) values ('" + inObject.MovieName + "', '" + inObject.IMDbID + "', '" + inObject.Year + "', '" + inObject.PostTitle + "', '" + inObject.PostURL + "', '" + inObject.ImageURL + "', '" + inObject.RapidGator + "', '" + inObject.LinksChecked + "', '" + inObject.InCollection + "', '" + inObject.DateAdded + "', '" + inObject.Cateogry + "')";
                bool Inserted = sqlite.Execute(DbFile, InsertLine);
                if (!Inserted) { ahk.MsgBox("Inserted Into [SceneSourceTV] = " + Inserted.ToString()); }
                return Inserted;
            }

            public bool ssMovie_Update(string DbFile, ssMovie inObject, string WHERE = "[Item] = 'Value'")
            {
                //string UpdateLine = "Update [SceneSourceTV] set MovieName = '" + inObject.MovieName + "', IMDbID = '" + inObject.IMDbID + "', Year = '" + inObject.Year + "', PostTitle = '" + inObject.PostTitle + "', PostURL = '" + inObject.PostURL + "', ImageURL = '" + inObject.ImageURL + "', RapidGator = '" + inObject.RapidGator + "', LinksChecked = '" + inObject.LinksChecked + "', InCollection = '" + inObject.InCollection + "', DateAdded = '" + inObject.DateAdded + "', Cateogry = '" + inObject.Cateogry + "' WHERE [Item] = 'Value' ";
                string UpdateLine = "Update [SceneSourceTV] set ";


                if (inObject.MovieName != null) { UpdateLine = UpdateLine + "[MovieName] = '" + inObject.MovieName + "',"; }
                if (inObject.IMDbID != null) { UpdateLine = UpdateLine + "[IMDbID] = '" + inObject.IMDbID + "',"; }
                if (inObject.Year != null) { UpdateLine = UpdateLine + "[Year] = '" + inObject.Year + "',"; }
                if (inObject.PostTitle != null) { UpdateLine = UpdateLine + "[PostTitle] = '" + inObject.PostTitle + "',"; }
                if (inObject.PostURL != null) { UpdateLine = UpdateLine + "[PostURL] = '" + inObject.PostURL + "',"; }
                if (inObject.ImageURL != null) { UpdateLine = UpdateLine + "[ImageURL] = '" + inObject.ImageURL + "',"; }
                if (inObject.RapidGator != null) { UpdateLine = UpdateLine + "[RapidGator] = '" + inObject.RapidGator + "',"; }
                if (inObject.LinksChecked != null) { UpdateLine = UpdateLine + "[LinksChecked] = '" + inObject.LinksChecked + "',"; }
                if (inObject.InCollection != null) { UpdateLine = UpdateLine + "[InCollection] = '" + inObject.InCollection + "',"; }
                if (inObject.DateAdded != null) { UpdateLine = UpdateLine + "[DateAdded] = '" + inObject.DateAdded + "',"; }
                if (inObject.Cateogry != null) { UpdateLine = UpdateLine + "[Cateogry] = '" + inObject.Cateogry + "',"; }

                UpdateLine = ahk.TrimLast(UpdateLine, 1);
                UpdateLine = UpdateLine + " WHERE " + WHERE;

                bool Updated = sqlite.Execute(DbFile, UpdateLine);
                return Updated;
            }

            public bool ssMovie_UpdateInsert(string DbFile, ssMovie obj, string WhereClause = "")
            {
                bool Updated = ssMovie_Update(DbFile, obj, WhereClause);  // try to update record first
                if (!Updated) { Updated = ssMovie_Insert(DbFile, obj); }  // if unable to update, insert new record
                return Updated;
            }


            #endregion
            #region ===== ssMovie DataTable =====

            public DataTable Return_ssMovie_DataTable(string DbFile, string TableName = "ssMovie", string WhereClause = "", bool Debug = false)
            {
                string SelectLine = "Select * From [SceneSourceTV]";

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }

                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


                DataTable table = new DataTable();
                table.Columns.Add("MovieName", typeof(string));
                table.Columns.Add("IMDbID", typeof(string));
                table.Columns.Add("Year", typeof(string));
                table.Columns.Add("PostTitle", typeof(string));
                table.Columns.Add("PostURL", typeof(string));
                table.Columns.Add("ImageURL", typeof(string));
                table.Columns.Add("RapidGator", typeof(string));
                table.Columns.Add("LinksChecked", typeof(string));
                table.Columns.Add("InCollection", typeof(string));
                table.Columns.Add("DateAdded", typeof(string));
                table.Columns.Add("Cateogry", typeof(string));

                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        ssMovie returnObject = new ssMovie();

                        returnObject.MovieName = ret["MovieName"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.Year = ret["Year"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.ImageURL = ret["ImageURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToString();
                        returnObject.InCollection = ret["InCollection"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.Cateogry = ret["Cateogry"].ToString();

                        table.Rows.Add(returnObject.MovieName, returnObject.IMDbID, returnObject.Year, returnObject.PostTitle, returnObject.PostURL, returnObject.ImageURL, returnObject.RapidGator, returnObject.LinksChecked, returnObject.InCollection, returnObject.DateAdded, returnObject.Cateogry);
                    }
                }

                return table;
            }

            public DataTable Create_SceneSourceTV_DataTable(ssMovie inObject)
            {
                DataTable table = new DataTable();
                table.Columns.Add("MovieName", typeof(string));
                table.Columns.Add("IMDbID", typeof(string));
                table.Columns.Add("Year", typeof(string));
                table.Columns.Add("PostTitle", typeof(string));
                table.Columns.Add("PostURL", typeof(string));
                table.Columns.Add("ImageURL", typeof(string));
                table.Columns.Add("RapidGator", typeof(string));
                table.Columns.Add("LinksChecked", typeof(string));
                table.Columns.Add("InCollection", typeof(string));
                table.Columns.Add("DateAdded", typeof(string));
                table.Columns.Add("Cateogry", typeof(string));

                table.Rows.Add(inObject.MovieName, inObject.IMDbID, inObject.Year, inObject.PostTitle, inObject.PostURL, inObject.ImageURL, inObject.RapidGator, inObject.LinksChecked, inObject.InCollection, inObject.DateAdded, inObject.Cateogry);
                return table;
            }


            #endregion
            #region ===== ssMovie SQL Functions =====

            // Return ssMovie SQL Connection String
            public SqlConnection ssMovie_Conn()
            {
                // populate sql connection
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMLucidMedia"].ConnectionString);
                // SqlConnection Conn = new SqlConnection("Server=188.168.188.88;DataBase=LucidMedia;Uid=lucidm;Pwd=pass");
                return conn;
            }

            // Return ssMovie TableName (Full Path)
            public string ssMovie_TableName()
            {
                // populate to return full sql table name
                return "[LucidMedia].[dbo].[SceneSourceMovies]";
            }

            // Generate SQL Table
            public bool ssMovie_CreateSQLTable()
            {
                SqlConnection Conn = ssMovie_Conn();
                string CreateTableLine = "CREATE TABLE [SceneSourceTV](";
                CreateTableLine = CreateTableLine + "[MovieName] [int] IDENTITY(1,1) NOT NULL,";
                CreateTableLine = CreateTableLine + "[IMDbID] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Year] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[PostTitle] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[PostURL] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[ImageURL] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[RapidGator] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[LinksChecked] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[InCollection] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[DateAdded] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Cateogry] [varchar](max) NOT NULL";
                CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                return false;
            }

            public bool ssMovie_InsertSQL(ssMovie obj)
            {
                SqlConnection Con = ssMovie_Conn();
                string SQLLine = "Insert Into " + ssMovie_TableName() + " (MovieName, IMDbID, Year, PostTitle, PostURL, ImageURL, RapidGator, LinksChecked, InCollection, DateAdded, Cateogry) VALUES (@MovieName, @IMDbID, @Year, @PostTitle, @PostURL, @ImageURL, @RapidGator, @LinksChecked, @InCollection, @DateAdded, @Cateogry)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.MovieName == null) { obj.MovieName = ""; }
                if (obj.IMDbID == null) { obj.IMDbID = ""; }
                if (obj.Year == null) { obj.Year = ""; }
                if (obj.PostTitle == null) { obj.PostTitle = ""; }
                if (obj.PostURL == null) { obj.PostURL = ""; }
                if (obj.ImageURL == null) { obj.ImageURL = ""; }
                if (obj.RapidGator == null) { obj.RapidGator = ""; }
                if (obj.LinksChecked == null) { obj.LinksChecked = ""; }
                if (obj.InCollection == null) { obj.InCollection = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = ""; }
                if (obj.Cateogry == null) { obj.Cateogry = ""; }
                cmd2.Parameters.AddWithValue(@"MovieName", obj.MovieName.ToString());
                cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID.ToString());
                cmd2.Parameters.AddWithValue(@"Year", obj.Year.ToString());
                cmd2.Parameters.AddWithValue(@"PostTitle", obj.PostTitle.ToString());
                cmd2.Parameters.AddWithValue(@"PostURL", obj.PostURL.ToString());
                cmd2.Parameters.AddWithValue(@"ImageURL", obj.ImageURL.ToString());
                cmd2.Parameters.AddWithValue(@"RapidGator", obj.RapidGator.ToString());
                cmd2.Parameters.AddWithValue(@"LinksChecked", obj.LinksChecked.ToString());
                cmd2.Parameters.AddWithValue(@"InCollection", obj.InCollection.ToString());
                cmd2.Parameters.AddWithValue(@"DateAdded", DateTime.Now.ToString());
                cmd2.Parameters.AddWithValue(@"Cateogry", obj.Cateogry.ToString());
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        ssMovie_InsertSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        ssMovie_InsertSQL(obj);
                    }
                    else
                    {
                        ahk.MsgBox(ex.ToString());
                        return false;
                    }
                }
                Con.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool ssMovie_UpdateSQL(ssMovie obj)
            {
                SqlConnection Conn = ssMovie_Conn();
                string SQLLine = "Update " + ssMovie_TableName() + " SET MovieName = @MovieName, IMDbID = @IMDbID, Year = @Year, PostTitle = @PostTitle, ImageURL = @ImageURL, RapidGator = @RapidGator, LinksChecked = @LinksChecked, InCollection = @InCollection, Cateogry = @Cateogry WHERE PostURL = @PostURL";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.MovieName == null) { obj.MovieName = ""; }
                if (obj.IMDbID == null) { obj.IMDbID = ""; }
                if (obj.Year == null) { obj.Year = ""; }
                if (obj.PostTitle == null) { obj.PostTitle = ""; }
                if (obj.PostURL == null) { obj.PostURL = ""; }
                if (obj.ImageURL == null) { obj.ImageURL = ""; }
                if (obj.RapidGator == null) { obj.RapidGator = ""; }
                if (obj.LinksChecked == null) { obj.LinksChecked = ""; }
                if (obj.InCollection == null) { obj.InCollection = ""; }
                //if (obj.DateAdded == null) { obj.DateAdded = ""; }
                if (obj.Cateogry == null) { obj.Cateogry = ""; }
                cmd2.Parameters.AddWithValue(@"MovieName", obj.MovieName.ToString());
                cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID.ToString());
                cmd2.Parameters.AddWithValue(@"Year", obj.Year.ToString());
                cmd2.Parameters.AddWithValue(@"PostTitle", obj.PostTitle.ToString());
                cmd2.Parameters.AddWithValue(@"PostURL", obj.PostURL.ToString());
                cmd2.Parameters.AddWithValue(@"ImageURL", obj.ImageURL.ToString());
                cmd2.Parameters.AddWithValue(@"RapidGator", obj.RapidGator.ToString());
                cmd2.Parameters.AddWithValue(@"LinksChecked", obj.LinksChecked.ToString());
                cmd2.Parameters.AddWithValue(@"InCollection", obj.InCollection.ToString());
                //cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded.ToString());
                cmd2.Parameters.AddWithValue(@"Cateogry", obj.Cateogry.ToString());
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        ssMovie_UpdateSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        ssMovie_UpdateSQL(obj);
                    }
                    else
                    {
                        ahk.MsgBox(ex.ToString());
                        return false;
                    }
                }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool ssMovie_UpdateLinkStatus(ssMovie obj)
            {
                SqlConnection Conn = ssMovie_Conn();
                string SQLLine = "Update " + ssMovie_TableName() + " SET RapidGator = @RapidGator WHERE PostURL = @PostURL";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.RapidGator == null) { obj.RapidGator = ""; }
                cmd2.Parameters.AddWithValue(@"RapidGator", obj.RapidGator.ToString());
                cmd2.Parameters.AddWithValue(@"PostURL", obj.PostURL.ToString());
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        ssMovie_UpdateSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        ssMovie_UpdateSQL(obj);
                    }
                    else
                    {
                        ahk.MsgBox(ex.ToString());
                        return false;
                    }
                }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }


            public bool ssMovie_UpdateInsert(ssMovie obj)
            {
                SqlConnection Conn = ssMovie_Conn();
                bool Updated = ssMovie_UpdateSQL(obj);  // try to update record first
                if (!Updated) { Updated = ssMovie_InsertSQL(obj); }  // if unable to update, insert new record
                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool ssMovie_UpdateIfPopulated(ssMovie obj, string ID = "")
            {
                SqlConnection Conn = ssMovie_Conn();
                string SQLcmd = "Update " + ssMovie_TableName() + " SET ";
                if (obj.MovieName != null) { SQLcmd = SQLcmd + " MovieName = @MovieName,"; }
                if (obj.IMDbID != null) { SQLcmd = SQLcmd + " IMDbID = @IMDbID,"; }
                if (obj.Year != null) { SQLcmd = SQLcmd + " Year = @Year,"; }
                if (obj.PostTitle != null) { SQLcmd = SQLcmd + " PostTitle = @PostTitle,"; }
                if (obj.PostURL != null) { SQLcmd = SQLcmd + " PostURL = @PostURL,"; }
                if (obj.ImageURL != null) { SQLcmd = SQLcmd + " ImageURL = @ImageURL,"; }
                if (obj.RapidGator != null) { SQLcmd = SQLcmd + " RapidGator = @RapidGator,"; }
                if (obj.LinksChecked != null) { SQLcmd = SQLcmd + " LinksChecked = @LinksChecked,"; }
                if (obj.InCollection != null) { SQLcmd = SQLcmd + " InCollection = @InCollection,"; }
                if (obj.DateAdded != null) { SQLcmd = SQLcmd + " DateAdded = @DateAdded,"; }
                if (obj.Cateogry != null) { SQLcmd = SQLcmd + " Cateogry = @Cateogry,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ID = @ID";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.MovieName != null) { cmd2.Parameters.AddWithValue(@"MovieName", obj.MovieName); }
                if (obj.IMDbID != null) { cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID); }
                if (obj.Year != null) { cmd2.Parameters.AddWithValue(@"Year", obj.Year); }
                if (obj.PostTitle != null) { cmd2.Parameters.AddWithValue(@"PostTitle", obj.PostTitle); }
                if (obj.PostURL != null) { cmd2.Parameters.AddWithValue(@"PostURL", obj.PostURL); }
                if (obj.ImageURL != null) { cmd2.Parameters.AddWithValue(@"ImageURL", obj.ImageURL); }
                if (obj.RapidGator != null) { cmd2.Parameters.AddWithValue(@"RapidGator", obj.RapidGator); }
                if (obj.LinksChecked != null) { cmd2.Parameters.AddWithValue(@"LinksChecked", obj.LinksChecked); }
                if (obj.InCollection != null) { cmd2.Parameters.AddWithValue(@"InCollection", obj.InCollection); }
                if (obj.DateAdded != null) { cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded); }
                if (obj.Cateogry != null) { cmd2.Parameters.AddWithValue(@"Cateogry", obj.Cateogry); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public ssMovie ssMovie_ReturnSQL(string ID = "")
            {
                SqlConnection Conn = ssMovie_Conn();
                string SelectLine = "Select [MovieName],[IMDbID],[Year],[PostTitle],[PostURL],[ImageURL],[RapidGator],[LinksChecked],[InCollection],[DateAdded],[Cateogry] From " + ssMovie_TableName() + " WHERE ID = '" + ID + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                ssMovie returnObject = new ssMovie();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.MovieName = ret["MovieName"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.Year = ret["Year"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.ImageURL = ret["ImageURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToString();
                        returnObject.InCollection = ret["InCollection"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.Cateogry = ret["Cateogry"].ToString();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public List<ssMovie> ssMovie_ReturnSQLList(string Command = "")
            {
                SqlConnection Conn = ssMovie_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<ssMovie> ReturnList = new List<ssMovie>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        ssMovie returnObject = new ssMovie();
                        returnObject.MovieName = ret["MovieName"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.Year = ret["Year"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.ImageURL = ret["ImageURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToString();
                        returnObject.InCollection = ret["InCollection"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.Cateogry = ret["Cateogry"].ToString();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }




            #endregion

            #endregion


            public bool URLinIndex(string URL)
            {
                SqlConnection Conn = ssTv_Conn();
                string SelectLine = "Select [ShowName] From " + ssTv_TableName() + " WHERE PostURL = '" + URL + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                ssTv returnObject = new ssTv();
                if (ReturnTable != null)
                {
                    return true;
                }
                return false;
            }

            public bool UpdateRGLinks(string URL, string Links)
            {
                SqlConnection Conn = ssTv_Conn();
                string SQLLine = "Update " + ssTv_TableName() + " SET RapidGator = @RapidGator, LinksChecked = @LinksChecked WHERE PostURL = @PostURL";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);

                //cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                //cmd2.Parameters.AddWithValue(@"SeasonEp", obj.SeasonEp.ToString());
                //cmd2.Parameters.AddWithValue(@"PostTitle", obj.PostTitle.ToString());
                cmd2.Parameters.AddWithValue(@"PostURL", URL);
                cmd2.Parameters.AddWithValue(@"RapidGator", Links);
                cmd2.Parameters.AddWithValue(@"LinksChecked", true);
                //cmd2.Parameters.AddWithValue(@"InCollection", obj.InCollection.ToString());
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public void _Backfill_MovieLinks(string Cat)
            {
                //string Cat = "DVDRip";

                string cmd = "select * FROM [lucidmedia].[lucidmethod].[SceneSourceMovies] where Cateogry = '" + Cat + "' order by MovieName asc";
                //if (Reverse) { cmd = "select * FROM [lucidmedia].[lucidmethod].[SceneSourceMovies] where LinksChecked = '0' and Cateogry = 'DVDRip' order by MovieName desc"; }

                List<ssMovie> movies = ssMovie_ReturnSQLList(cmd);

                //List<ssMovie> updateMovies = new List<ssMovie>();  // create list of items to update in batch

                string saveDir = ahk.AppDir() + "\\SceneSource\\" + Cat + "\\PostHTML";
                ahk.FileCreateDir(saveDir);

                int i = 1;
                foreach (ssMovie movie in movies)
                {
                    ssMovie Movie = movie;
                    sb.StatusBar("Movie " + i + "/" + movies.Count, 2); i++;

                    string html = web.DownloadHTML(movie.PostURL);

                    //https://www.scnsrc.me/borg-vs-mcenroe-2017-bdrip-x264-amiable

                    string name = "";
                    List<string> parts = ahk.StringSplit_List(Movie.PostURL, "/");
                    foreach (string part in parts) { name = part; }

                    if (html.Contains("rapidgator.net") || html.Contains("rg.net"))
                    {
                        ahk.FileAppend(html, saveDir + "\\" + name + ".txt");
                    }




                    //List<string> rgLinks = web.Rapidgator_LinkList(html);

                    //pro.SetupProgressBar(dispProgress, rgLinks.Count);
                    //pro.ResetProgress(dispProgress);

                    //List<string> rgLinksChecked = new List<string>();

                    //int p = 1;
                    //foreach (string item in rgLinks)
                    //{
                    //    pro.UpdateProgress(dispProgress, p + "/" + rgLinks.Count()); p++;

                    //    _Web.RapidGator.RGInfo rapid = rg.RapidGatorCheckStatus(item);  // check if link is online

                    //    if (rapid.FileOnline) { rgLinksChecked.Add(item + " (" + rapid.FileSize + ")"); }  // if online add to list

                    //}

                    //string goodLinks = lst.List_To_String_NewLines(rgLinksChecked);

                    //Movie.RapidGator = goodLinks;
                    //Movie.LinksChecked = "1";
                    //string id = prs.Extract_IMDbID(html);


                    //Movie.IMDbID = id;


                    //// build list of movies to update in db in batch (every 10th movie update and start list over)
                    //if (updateMovies.Count < 10) { updateMovies.Add(Movie); }
                    //else
                    //{
                    //    updateMovies.Add(Movie);
                    //    foreach (ssMovie mov in updateMovies)
                    //    {
                    //        bool Updated = ssMovie_UpdateSQL(mov);
                    //    }
                    //    updateMovies = new List<ssMovie>();
                    //}

                }
            }



            #endregion



            #endregion

            #region === Parse Show Info ===

            /// <summary>
            /// Extract Show Ep Number/Season Number from FileName or URL
            /// </summary>
            /// <param name="FileName_Or_URL">FileName or URL</param>
            /// <returns></returns>
            public string ShowEp_FromFilePath(string FileName_Or_URL)
            {
                string sName = "";

                // parse file name from url if passed in
                List<string> Parts = ahk.StringSplit_List(FileName_Or_URL, "/");
                foreach (string Part in Parts) { sName = Part; }

                string SeasonEp = prs.SeasonEpNums("c:\\" + sName + ".avi", false);

                sName = sName.Replace("-", " ");

                if (SeasonEp == "")
                {
                    string badChars = sName.Encode();

                    sName = sName.Replace("×", "x");

                    SeasonEp = prs.SeasonEpNums(sName, false);

                    if (SeasonEp == "")
                    {
                        SeasonEp = prs.ExtractSeason(sName);  // ex: show s01 title
                    }

                    if (SeasonEp == "")
                    {
                        SeasonEp = prs.ExtractDate(sName); // extract date (2017 01 13)
                    }

                    if (SeasonEp == "")
                    {
                        SeasonEp = prs.ExtractYear(sName);  // extract year
                    }
                }

                if (SeasonEp != "")
                {
                    if (SeasonEp.Length > 6)
                    {
                        sName = sName.Replace(SeasonEp.ToUpper(), "|");
                        sName = sName.Replace(SeasonEp.ToLower(), "|");

                        sName = ahk.StringSplit(sName, "|", 0);
                        SeasonEp = SeasonEp.Replace(" ", ".");
                    }
                    else
                    {
                        sName = sName.Replace(SeasonEp.ToUpper(), "|");
                        sName = sName.Replace(SeasonEp.ToLower(), "|");
                        sName = ahk.StringSplit(sName, "|", 0);
                    }
                }

                string ShowName = sName.Trim().ToTitleCase();

                return SeasonEp;
            }

            /// <summary>
            /// Extract Show Name from FileName or URL
            /// </summary>
            /// <param name="FileName_Or_URL">FileName or URL</param>
            /// <returns></returns>
            public string ShowName_FromFilePath(string FileName_Or_URL)
            {
                string sName = "";

                // parse file name from url if passed in
                List<string> Parts = ahk.StringSplit_List(FileName_Or_URL, "/");
                foreach (string Part in Parts) { sName = Part; }

                string SeasonEp = prs.SeasonEpNums("c:\\" + sName + ".avi", false);

                sName = sName.Replace("-", " ");

                if (SeasonEp == "")
                {
                    string badChars = sName.Encode();

                    sName = sName.Replace("×", "x");

                    SeasonEp = prs.SeasonEpNums(sName, false);

                    if (SeasonEp == "")
                    {
                        SeasonEp = prs.ExtractSeason(sName);  // ex: show s01 title
                    }

                    if (SeasonEp == "")
                    {
                        SeasonEp = prs.ExtractDate(sName); // extract date (2017 01 13)
                    }

                    if (SeasonEp == "")
                    {
                        SeasonEp = prs.ExtractYear(sName);  // extract year
                    }
                }

                if (SeasonEp != "")
                {
                    if (SeasonEp.Length > 6)
                    {
                        sName = sName.Replace(SeasonEp.ToUpper(), "|");
                        sName = sName.Replace(SeasonEp.ToLower(), "|");

                        sName = ahk.StringSplit(sName, "|", 0);
                        SeasonEp = SeasonEp.Replace(" ", ".");
                    }
                    else
                    {
                        sName = sName.Replace(SeasonEp.ToUpper(), "|");
                        sName = sName.Replace(SeasonEp.ToLower(), "|");
                        sName = ahk.StringSplit(sName, "|", 0);
                    }
                }

                string ShowName = sName.Trim().ToTitleCase();


                return ShowName;
            }


            #endregion

        }

        // scene source v2.0
        public class SceneSourceLib
        {
            public _Database.TVShowDb.LITM_ShowIndex GetShowInfo(ssTv show, bool DownloadEpGuide = false)
            {
                _Sites.EpGuides epg = new _Sites.EpGuides();
                _Database.TVShowDb sinfo = new _Database.TVShowDb();

                _Database.TVShowDb.LITM_ShowIndex showInfo = sinfo.LITM_ReturnShow(show.ShowName); // string LocalRoot = "", string IMDbID = "", bool Local = true)

                if (DownloadEpGuide)
                {
                    if (showInfo.EpGuideURL != null && showInfo.EpGuideURL != "")
                    {
                        List<litmEpisodes> EpGuide = epg.EpGuideProcess(false, showInfo.EpGuideURL, "LITMLucidMedia");
                        showInfo.EpGuide = EpGuide;
                    }
                    else
                    {
                        ahk.MsgBox("EpGuideURL Not Found For Show");
                    }
                }

                return showInfo;
            }


            #region === Startup ===

            _AHK ahk = new _AHK();
            _Database.SQL sql = new _Database.SQL();
            _Database.SQLite sqlite = new _Database.SQLite();
            _Lists lst = new _Lists();
            _Parse prs = new _Parse();
            _StatusBar sb = new _StatusBar();
            _Web web = new _Web();
            _TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            _Images img = new _Images();
            _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();
            _Parse.XML xml = new _Parse.XML();
            _Sites.RapidGator rg = new _Sites.RapidGator();



            public static string JLRapidPass { get; set; }
            public static string LucidRapidPass { get; set; }

            public static RadTreeView DisplayTree { get; set; }

            public static PictureBox ShowImageBox { get; set; }

            public static Image ShowImage { get; set; }
            public static string ShowName { get; set; }

            public static RadTextBox txtBox { get; set; }
            public static RadTextBox totalBox { get; set; }

            public static RadProgressBar LinkCheckProgress { get; set; }
            public static RadTextBox PostURL { get; set; }

            public static RadProgressBar dispProgress { get; set; }
            public static RadTreeView RadTree { get; set; }

            public static List<string> CapturedPostURLs { get; set; }


            // Assign RapidGator Passwords on Startup
            public void SetPasswords(string LucidPass = "", string JLPass = "")
            {
                JLRapidPass = JLPass;
                LucidRapidPass = LucidPass;
            }

            #endregion

            #region === CONFIG ===

            //SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["epGuideDb"].ConnectionString);
            SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMLucidMedia"].ConnectionString);


            // Return ssTv TableName (Full Path)
            public string ssTv_TableName()
            {
                // populate to return full sql table name
                //return "[lucidmedia].[lucidmethod].[SceneSourceTV]";
                return "[LucidMedia].[dbo].[SceneSourceTV]";
            }

            // Return ssMovie TableName (Full Path)
            public string ssMovie_TableName()
            {
                // populate to return full sql table name
                //return "[lucidmedia].[lucidmethod].[SceneSourceMovies]";
                return "[LucidMedia].[dbo].[SceneSourceMovies]";
            }


            #endregion

            #region === RapidGator ===


            bool ShowImagesDuringDownload = true;
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
                if (Parent != null) { showName = Parent.Text; }

                string html = ahk.Download_HTML(URL);

                List<string> links = rg.Regex_RGLinks(html);

                List<string> glinks = new List<string>();
                string goodlinks = "";

                if (!CheckOnlineStatus) { glinks = links; }

                // Extract / Display Images From Page

                List<string> images = rg.JpgImageLinks(html);  // list of jpg images found on page
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

                if (ShowImagesDuringDownload)
                {
                    if (savename != "")
                    {
                        if (ShowImageBox != null)
                        {
                            if (ShowImageBox.InvokeRequired)
                            {
                                ShowImageBox.BeginInvoke((MethodInvoker)delegate ()
                                {
                                    //ShowImageBox.Image = img.To_Image(savename); });
                                    ShowImageBox.Image = img.GetCopyImage(savename);
                                });
                            }
                            else
                            {
                                //ShowImageBox.Image = img.To_Image(savename);
                                ShowImageBox.Image = img.GetCopyImage(savename);
                            }
                        }
                    }
                }




                if (RadTree != null)
                {
                    // add list to treeview (without checking online status)
                    if (!CheckOnlineStatus) { tree.ListRadTree(RadTree, links); }
                }

                foreach (string link in links)
                {
                    RadTreeNode node = new RadTreeNode();
                    node.Tag = link;
                    string epFileName = "";
                    string SeasonEpNum = "";

                    if (CheckOnlineStatus)
                    {
                        bool online = rg.CheckStatus(link);

                        if (online) { glinks.Add(link); if (goodlinks == "") { goodlinks = link; } else { goodlinks = goodlinks + "\n" + link; } }

                        _Sites.RapidGator.RGInfo info = RapidGatorCheckStatus(link);

                        epFileName = info.FileName;

                        //SceneSource SS = new SceneSource();
                        //ssTv show = SS.SceneSource_TV_LineParse(epFileName, SSCats.TVShows, false);

                        SeasonEpNum = prs.SeasonEpNums("C:\\" + epFileName, false);

                        //ssTv show = new ssTv();

                        if (RadTree != null)
                        {
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
                        }


                    }

                    if (!CheckOnlineStatus)
                    {
                        // parse url for file name
                        List<string> parts = ahk.StringSplit_List(link, "/");
                        foreach (string part in parts)
                        {
                            epFileName = part;
                        }

                        epFileName = epFileName.Replace(".html", "");

                        node.Text = epFileName;

                        string fakePath = "C:\\" + epFileName;

                        SeasonEpNum = prs.SeasonEpNums(fakePath, false);
                        epFileName = ahk.FileNameNoExt(fakePath);
                    }


                    if (RadTree != null)
                    {
                        if (Parent == null)
                        {
                            tree.AddNode(RadTree, node);
                        }
                        else
                        {

                            string EpNodeText = showName + "_" + SeasonEpNum;
                            RadTreeNode EpNumNode = new RadTreeNode();
                            EpNumNode.Text = SeasonEpNum;
                            EpNumNode.Tag = EpNodeText;
                            tree.AddNode(RadTree, EpNumNode);
                        }
                    }


                }


                //ahk.MsgBox("Found " + links.Count + " Links");

                return glinks;
            }


            public static string login { get; set; }
            public static string pass { get; set; }
            public static string sessionID { get; set; }
            public static string trafficLeft { get; set; }
            public string responseStatus { get; set; }




            /// <summary>
            /// check to see if RapidGator link is Online, Returns Object with Returned Info 
            /// </summary>
            /// <param name="LinkURL">Rapidgator Link to Check</param>
            /// <returns>Returns object with Rapidgator file status</returns>
            public sharpAHK_Dev._Sites.RapidGator.RGInfo RapidGatorCheckStatus(string LinkURL)
            {
                if (sessionID == null || sessionID == "") // populate session id if not found yet
                {
                    sessionID = rg.GrabSessionID();
                }

                _Sites.RapidGator.RGInfo RGInfo = new _Sites.RapidGator.RGInfo();


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
                    if (FoundURL) { linkURL = seg; FoundURL = false; RGInfo.FileURL = linkURL; }

                    if (seg.Contains("filename")) { FoundFileName = true; continue; }
                    if (FoundFileName) { fileName = seg; FoundFileName = false; RGInfo.FileName = fileName; }

                    if (seg.Contains("size")) { FoundSize = true; continue; }
                    if (FoundSize) { fileSize = seg; FoundSize = false; RGInfo.FileSize = ahk.FormatBytes(fileSize); }

                    if (seg == "status") { FoundStatus = true; continue; }
                    if (FoundStatus) { fileStatus = seg; FoundStatus = false; }
                }

                sb.StatusBar("FileName = " + fileName + " | FileSize = " + fileSize + " | FileStatus = " + fileStatus);
                //ahk.MsgBox("URL = " + LinkURL + "\nFileName = " + fileName + "\nFileSize = " + fileSize + "\nFileStatus = " + fileStatus); 


                if (fileStatus.ToUpper() == "ACCESS") { RGInfo.FileOnline = true; }
                else { RGInfo.FileOnline = false; }

                return RGInfo;
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



            #endregion

            #region === SceneSource ===

            public enum SiteName
            {
                SceneSource,
                ReleaseBB
            }


            /// <summary>
            /// 
            /// </summary>		
            public enum SSCats
            {
                BDRip,
                BluRay,
                DVDRip,
                ThreeD,
                HD,
                DVDScr,
                TVShows
            }

            public struct ParseControls
            {
                public RadProgressBar Bar1 { get; set; }
                public RadProgressBar Bar2 { get; set; }
                public RadProgressBar Bar3 { get; set; }
                public RadTextBox URLDisp { get; set; }
            }

            public struct SceneSourceOptions
            {
                public int PageNum { get; set; }
                public int TotalPgs { get; set; }
                public SSCats Cat { get; set; }
                public bool CheckOnlineStatus { get; set; }
                public bool WriteSQL { get; set; }
                public bool DownloadImages { get; set; }
                public bool GrabLinks { get; set; }
                public bool AllImages { get; set; }
            }


            public List<ssTv> SceneSource_Download_TVIndexHTML(ssTv options)
            {
                int PageNum = options.Sync_StartPageNum;
                int TotalPgs = options.Sync_TotalPageNums;
                bool CheckOnlineStatus = options.Sync_CheckLinkStatus;
                bool WriteSQL = options.Sync_WriteSQL;
                bool DownloadImages = options.Sync_DownloadMissingImages;

                SSCats Cat = SSCats.TVShows;


                //CapturedPostURLs

                bool GrabLinks = true; bool AllImages = false;

                //string saveRoot = ahk.AppDir() + "\\SceneSource\\" + Cat;
                //ahk.FileCreateDir(saveRoot);

                string urlStart = "https://www.scnsrc.me";

                PageNum = 1;
                if (Cat == SSCats.TVShows) { urlStart = "https://www.scnsrc.me/category/tv/page/"; if (TotalPgs == 0) { TotalPgs = 182; } }
                if (Cat == SSCats.BDRip) { urlStart = "https://www.scnsrc.me/category/films/bdrip/page/"; if (TotalPgs == 0) { TotalPgs = 182; } }
                if (Cat == SSCats.BluRay) { urlStart = "https://www.scnsrc.me/category/films/bluray/page/"; if (TotalPgs == 0) { TotalPgs = 429; } }
                if (Cat == SSCats.DVDRip) { urlStart = "https://www.scnsrc.me/category/films/dvdrip/page/"; if (TotalPgs == 0) { TotalPgs = 489; } }
                if (Cat == SSCats.ThreeD) { urlStart = "https://www.scnsrc.me/category/films/3d/page/"; if (TotalPgs == 0) { TotalPgs = 9; } }
                if (Cat == SSCats.HD) { urlStart = "https://www.scnsrc.me/category/films/hd/page/"; if (TotalPgs == 0) { TotalPgs = 25; } }
                if (Cat == SSCats.DVDScr) { urlStart = "https://www.scnsrc.me/category/films/dvdscr/page/"; if (TotalPgs == 0) { TotalPgs = 24; } }

                int total = TotalPgs - PageNum;
                total++;

                if (dispProgress != null)
                {
                    pro.SetupProgressBar(dispProgress, total);
                }

                totalBox.UpdateTextbox(TotalPgs.ToString());
                List<ssTv> pages = new List<ssTv>();

                int i = PageNum;
                do
                {
                    if (txtBox != null) { txtBox.UpdateTextbox(i.ToString()); }

                    //txtCurrentPg.UpdateControl(i.ToString()); 
                    //txtCurrentPg.UpdateControl(i.ToString());

                    if (dispProgress != null) { pro.UpdateProgress(dispProgress, i + "/" + TotalPgs); }

                    string url = urlStart + i + "/";


                    List<ssTv> page = Parse_SceneSourcePage(url, Cat, RadTree, CheckOnlineStatus, WriteSQL, DownloadImages, GrabLinks);

                    // add page to list of pages
                    foreach (ssTv pg in page)
                    {
                        pages.Add(pg);
                    }

                    //string saveFile = saveRoot + "\\" + i.AddLeadingZeros(4) + ".txt";
                    //ahk.FileAppend(html, saveFile);
                    //sqlite.Setting("SceneSourcePg", i.ToString());

                    i++;
                } while (i <= TotalPgs);

                //ahk.MsgBox("Finished Downloading SceneSource " + Cat + " Index HTML");

                return pages;
            }

            // check to see if post url has already been entered into db
            public bool ssTvURL_InDb(string URL)
            {
                string cmd = "select count(posttitle) FROM " + ssTv_TableName() + " where PostURL = '" + URL + "'";
                int count = sql.Count(ssTv_Conn(), cmd);
                if (count > 0) { return true; }
                return false;
            }

            public List<ssTv> Parse_SceneSourcePage(string URL, SSCats Cat = SSCats.TVShows, RadTreeView RadTree = null, bool CheckOnlineStatus = true, bool WriteSQL = true, bool DownloadImages = true, bool GrabLinks = true, bool AllImages = false)
            {
                string xmlPath = "";

                if (Cat == SSCats.TVShows) { xmlPath = "//body/div/div/div/div/h2"; }

                if (Cat == SSCats.BDRip || Cat == SSCats.BluRay || Cat == SSCats.DVDRip || Cat == SSCats.ThreeD || Cat == SSCats.HD || Cat == SSCats.DVDScr) { xmlPath = "//body/div/div/div/div/h2"; }

                string html = web.DownloadHTML(URL);

                List<string> lines = xml.Parse_HTML_XML(html, xmlPath);  // extract sections of text from html xml

                List<ssTv> TVResults = new List<ssTv>();
                List<ssMovie> MovieResults = new List<ssMovie>();

                int i = 1;
                foreach (string line in lines)
                {
                    if (Cat == SSCats.TVShows)
                    {
                        TVResults.Add(SceneSource_TV_LineParse(line, Cat, WriteSQL, CheckOnlineStatus, DownloadImages, GrabLinks, AllImages));
                    }

                    if (Cat == SSCats.BDRip || Cat == SSCats.BluRay || Cat == SSCats.DVDRip || Cat == SSCats.ThreeD || Cat == SSCats.HD || Cat == SSCats.DVDScr)
                    {
                        MovieResults.Add(SceneSource_Movie_LineParse(line, Cat));
                    }

                    sb.StatusBar(i + "/" + lines.Count); i++;
                }

                return TVResults;

                //if (RadTree != null)
                //{
                //    if (Cat == SSCats.TVShows)
                //    {
                //        foreach (ssTv show in TVResults)
                //        {
                //            RadTreeNode node = new RadTreeNode();  // show name 
                //            node.Text = show.ShowName;
                //            node.Tag = show.PostURL;
                //            tree.AddNode(RadTree, node);


                //            //Thread NewThread = new Thread(() => rg.RGLinks(txtURL.Text, radTree, true, cbCheckOnlineStat.Checked)); // Function To Execute
                //            //NewThread.Start();

                //            string rglinks = "";
                //            List<string> links = RGLinks(show.PostURL, RadTree, false, CheckOnlineStatus, node);
                //            foreach (string lnk in links)
                //            {
                //                if (rglinks == "") { rglinks = lnk; } else { rglinks = rglinks + "\n" + lnk; }
                //            }

                //            bool updated = UpdateRGLinks(show.PostURL, rglinks);

                //        }
                //    }


                //}


                //ahk.MsgBox(TVResults.Count); 
            }


            public void ParseHTMLDir(SSCats Cat, RadProgressBar dispProgress = null)
            {
                string saveRoot = ahk.AppDir() + "\\SceneSource\\";

                List<string> files = lst.FileList(saveRoot, "*.txt", true, false, true);

                if (dispProgress != null)
                {
                    pro.SetupProgressBar(dispProgress, files.Count);
                    pro.ResetProgress(dispProgress);
                }

                string xmlPath = "";

                if (Cat == SSCats.TVShows) { xmlPath = "//body/div/div/div/div/h2"; }

                if (Cat == SSCats.BDRip || Cat == SSCats.BluRay || Cat == SSCats.DVDRip || Cat == SSCats.ThreeD || Cat == SSCats.HD || Cat == SSCats.DVDScr) { xmlPath = "//body/div/div/div/div/h2"; }

                int p = 1;
                foreach (string file in files)
                {
                    pro.UpdateProgress(dispProgress, p + "/" + files.Count); p++;

                    string html = ahk.FileRead(file);
                    //string html = web.DownloadHTML(url);

                    //string xmlPath = "//body/div/div/div/div/h2";

                    //string xmlPath = txtXMLPath.Text;
                    //sqlite.Setting("LastXmlPath", xmlPath);


                    List<string> lines = xml.Parse_HTML_XML(html, xmlPath);  // extract sections of text from html xml
                    List<string> results = new List<string>();

                    int i = 1;
                    foreach (string line in lines)
                    {

                        if (Cat == SSCats.TVShows)
                        {
                            results.Add(SceneSource_TV_LineParse(line, Cat) + "\n\n");
                        }

                        if (Cat == SSCats.BDRip || Cat == SSCats.BluRay || Cat == SSCats.DVDRip || Cat == SSCats.ThreeD || Cat == SSCats.HD || Cat == SSCats.DVDScr)
                        {
                            results.Add(SceneSource_Movie_LineParse(line, Cat) + "\n\n");
                        }

                        sb.StatusBar(i + "/" + lines.Count); i++;
                    }

                }

                pro.ResetProgress(dispProgress);
            }


            // Using the Original Post URL, Returns RG Links and Updates DB
            public bool UpdateLinksFromSiteLinks(ssTv show, RadProgressBar progress = null, bool ClearTree = false, RadTreeView radTree = null)
            {
                List<string> links = RGLinks(show.PostURL, radTree, ClearTree, false);

                string ParseLinks = sql.SQL_Return_Value(ssTv_Conn(), "select [RapidGator] FROM [LucidMedia].[dbo].[SceneSourceTV] where PostURL = '" + show.PostURL + "'");



                string goodlinks = "";
                int linksCount = 0;
                List<string> linkList = new List<string>();
                List<_Sites.RapidGator.RGInfo> GoodLinks = new List<_Sites.RapidGator.RGInfo>();

                int i = 0;
                if (progress != null) { pro.SetupProgressBar(progress, links.Count); }

                //foreach (string link in links)
                //{
                //    // SINGLE LINK METHOD (WORKS)

                //    RGInfo info = RapidGatorCheckStatus(link);

                //    if (progress != null) { i++; pro.UpdateProgress(progress, i + "/" + links.Count); }

                //    if (info.FileOnline)
                //    {
                //        string rgLinkInfo = info.FileURL + "|" + info.FileSize;

                //        if (goodlinks == "") { goodlinks = rgLinkInfo; linksCount++; } else { goodlinks = goodlinks + "\n" + rgLinkInfo; linksCount++; }
                //    }
                //}

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                //========= Mulitiple Link Checkers ==============

                goodlinks = "";
                linkList = new List<string>();
                GoodLinks = new List<_Sites.RapidGator.RGInfo>(); int n = 0;
                foreach (string link in links)
                {
                    i++; linkList.Add(link);
                    if (i == 25)
                    {
                        List<_Sites.RapidGator.RGInfo> linkInfo = rg.RapidGatorLinkList(linkList);

                        foreach (_Sites.RapidGator.RGInfo rglink in linkInfo)
                        {
                            if (rglink.FileOnline) { GoodLinks.Add(rglink); }
                        }

                        linkList = new List<string>();
                        i = 0;
                    }

                    if (progress != null) { n++; pro.UpdateProgress(progress, n + "/" + links.Count); }
                }

                // check list under 25 count added to link list
                if (linkList.Count > 0)
                {
                    List<_Sites.RapidGator.RGInfo> linkInfo = rg.RapidGatorLinkList(linkList);

                    foreach (_Sites.RapidGator.RGInfo rglink in linkInfo)
                    {
                        if (rglink.FileOnline) { GoodLinks.Add(rglink); }
                    }

                    linkList = new List<string>();
                }

                // convert good links to name | filesize format for db entry
                foreach (_Sites.RapidGator.RGInfo link in GoodLinks)
                {
                    string rgLinkInfo = link.FileURL + "|" + link.FileSize;
                    if (goodlinks == "") { goodlinks = rgLinkInfo; linksCount++; } else { goodlinks = goodlinks + "\n" + rgLinkInfo; linksCount++; }
                }

                stopwatch.Stop();
                string MultipleTime = stopwatch.Elapsed.ToString();

                ahk.MsgBox("MultipleTime = " + MultipleTime + " GoodLinks = " + GoodLinks.Count + " / " + links.Count);

                show.LinksChecked = true;
                show.LinksOnline = linksCount;
                show.RapidGator = goodlinks;

                return UpdateShow(show);
            }


            // Using 
            /// <summary>
            /// Using Database URLs, Return RG Links, Batch Status Check, Update DB 
            /// </summary>
            /// <param name="show"></param>
            /// <param name="progress"></param>
            /// <param name="ClearTree"></param>
            /// <param name="radTree"></param>
            /// <returns></returns>
            public ssTv BatchUpdate_Db_Links(ssTv show, RadProgressBar progress = null, bool ClearTree = false, RadTreeView radTree = null)
            {
                //List<string> links = RGLinks(show.PostURL, radTree, ClearTree, false);
                //string ParseLinks = sql.SQL_Return_Value(ssTv_Conn(), "select [RapidGator] FROM " + ssTv_TableName() + " where [PostURL] = '" + show.PostURL + "'");

                //rg.SetPasswords("PIYi2R");

                List<string> links = lst.Text_To_List(show.RapidGator);

                string goodlinks = "";
                int linksCount = 0;
                List<string> linkList = new List<string>();
                List<_Sites.RapidGator.RGInfo> GoodLinks = new List<_Sites.RapidGator.RGInfo>();

                int i = 0;
                if (progress != null) { pro.SetupProgressBar(progress, links.Count); }

                //foreach (string link in links)
                //{
                //    // SINGLE LINK METHOD (WORKS)

                //    RGInfo info = RapidGatorCheckStatus(link);

                //    if (progress != null) { i++; pro.UpdateProgress(progress, i + "/" + links.Count); }

                //    if (info.FileOnline)
                //    {
                //        string rgLinkInfo = info.FileURL + "|" + info.FileSize;

                //        if (goodlinks == "") { goodlinks = rgLinkInfo; linksCount++; } else { goodlinks = goodlinks + "\n" + rgLinkInfo; linksCount++; }
                //    }
                //}

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                //========= Mulitiple Link Checkers ==============

                goodlinks = "";
                linkList = new List<string>();
                GoodLinks = new List<_Sites.RapidGator.RGInfo>(); int n = 0;
                foreach (string link in links)
                {
                    string Link = link;
                    if (link.Contains("|")) { Link = ahk.StringSplit(link, "|", 0); }

                    i++; linkList.Add(Link);
                    if (i == 25)
                    {
                        List<_Sites.RapidGator.RGInfo> linkInfo = rg.RapidGatorLinkList(linkList);

                        foreach (_Sites.RapidGator.RGInfo rglink in linkInfo)
                        {
                            if (rglink.FileOnline) { GoodLinks.Add(rglink); }
                        }

                        linkList = new List<string>();
                        i = 0;
                    }

                    if (progress != null) { n++; pro.UpdateProgress(progress, n + "/" + links.Count); }
                }

                // check list under 25 count added to link list
                if (linkList.Count > 0)
                {
                    List<_Sites.RapidGator.RGInfo> linkInfo = rg.RapidGatorLinkList(linkList);

                    foreach (_Sites.RapidGator.RGInfo rglink in linkInfo)
                    {
                        if (rglink.FileOnline) { GoodLinks.Add(rglink); }
                    }

                    linkList = new List<string>();
                }

                // convert good links to name | filesize format for db entry
                foreach (_Sites.RapidGator.RGInfo link in GoodLinks)
                {
                    string rgLinkInfo = link.FileURL + "|" + link.FileSize;
                    if (goodlinks == "") { goodlinks = rgLinkInfo; linksCount++; } else { goodlinks = goodlinks + "\n" + rgLinkInfo; linksCount++; }
                }

                stopwatch.Stop();
                string MultipleTime = stopwatch.Elapsed.ToString();

                //ahk.MsgBox("MultipleTime = " + MultipleTime + " GoodLinks = " + GoodLinks.Count + " / " + links.Count);

                show.LinksChecked = true;
                show.LinksOnline = linksCount;
                show.RapidGator = goodlinks;

                return show;
            }



            public ssTv SceneSource_TV_LineParse(string Line, SSCats Cat = SSCats.TVShows, bool WriteSQL = false, bool CheckOnlineStatus = true, bool DownloadImages = true, bool GrabLinks = true, bool AllImages = false)
            {
                bool SkipExistingPosts = true;

                ssTv show = new ssTv();

                if (Cat == SSCats.TVShows)
                {
                    string StartLine = Line.Decode();

                    string sline = StartLine.Replace("<a href=\"", "");
                    string sURL = ahk.StringSplit(sline, "\"", 0);
                    sURL = ahk.TrimEndIf(sURL, "/");

                    sline = sline.Replace(sURL, "");
                    sline = sline.Replace("\"", "");
                    sline = sline.Replace("rel=bookmark", "");
                    sline = sline.Replace("title=", "").Trim();

                    string sName = ahk.StringSplit(sline, ">", 0).Trim();
                    sName = ahk.TrimFirst(sName, 7); // trim off "  goto "

                    string PostTitle = sName.Trim();

                    string SeasonEp = ShowEp_FromFilePath(PostTitle);
                    string ShowName = ShowName_FromFilePath(PostTitle);


                    string returnLine = sURL + "\n" + PostTitle + "\n" + ShowName + "\n" + SeasonEp;

                    show.ShowName = ShowName;
                    show.PostURL = sURL;
                    show.PostTitle = PostTitle;
                    show.SeasonEp = SeasonEp;

                    if (SkipExistingPosts)  // option to check to see if post url exists in db
                    {
                        bool exists = ssTvURL_InDb(show.PostURL);
                        if (exists) { return ShowByURL(show.PostURL); }
                    }


                    string rglinks = "";
                    List<string> links = new List<string>();

                    if (GrabLinks)  // download html from each page in index, extract links/images
                    {
                        links = rg.PullRGLinks(show.ShowName, show.PostURL, CheckOnlineStatus, DownloadImages, AllImages);
                        foreach (string lnk in links)
                        {
                            if (rglinks == "") { rglinks = lnk; } else { rglinks = rglinks + "\n" + lnk; }
                        }

                        if (CheckOnlineStatus) { show.LinksChecked = true; }
                        else { show.LinksChecked = false; }
                    }


                    show.LinksOnline = links.Count;
                    show.RapidGator = rglinks;


                    string imagedir = ahk.AppDir() + "\\ShowImages";
                    string imgPath = imagedir + "\\" + show.ShowName + ".jpg";

                    if (ShowImagesDuringDownload)
                    {
                        if (File.Exists(imgPath))
                        {
                            if (ShowImageBox != null)
                            {
                                if (ShowImageBox.InvokeRequired)
                                {
                                    ShowImageBox.BeginInvoke((MethodInvoker)delegate ()
                                    {
                                        //ShowImageBox.Image = img.To_Image(savename); });
                                        ShowImageBox.Image = img.GetCopyImage(imgPath);
                                    });
                                }
                                else
                                {
                                    //ShowImageBox.Image = img.To_Image(savename);
                                    ShowImageBox.Image = img.GetCopyImage(imgPath);
                                }
                            }
                        }
                    }


                    if (WriteSQL)
                    {
                        // write to sql db
                        bool added = UpdateShow(show);
                        //bool added = ssTv_UpdateInsert(show);
                    }

                    //sb.StatusBar("Added " + PostTitle + " = " + added.ToString());

                    return show;
                    //ahk.MsgBox(sURL + "\n" + PostTitle + "\n" + ShowName + "\n" + SeasonEp);
                }

                return show;
            }


            public ssMovie SceneSource_Movie_LineParse(string Line, SSCats Cat = SSCats.TVShows, bool WriteSQL = false)
            {
                ssMovie movie = new ssMovie();

                if (Cat == SSCats.BDRip || Cat == SSCats.BluRay || Cat == SSCats.DVDRip || Cat == SSCats.ThreeD || Cat == SSCats.HD || Cat == SSCats.DVDScr)
                {
                    movie.Cateogry = Cat.ToString();

                    string StartLine = Line.Decode();

                    string sline = StartLine.Replace("<a href=\"", "");
                    string sURL = ahk.StringSplit(sline, "\"", 0);
                    sURL = ahk.TrimEndIf(sURL, "/");

                    sline = sline.Replace(sURL, "");
                    sline = sline.Replace("\"", "");
                    sline = sline.Replace("rel=bookmark", "");
                    sline = sline.Replace("title=", "").Trim();

                    string sName = ahk.StringSplit(sline, ">", 0).Trim();
                    sName = ahk.TrimFirst(sName, 7); // trim off "  goto "

                    string PostTitle = sName.Trim();
                    string SeasonEp = prs.ExtractYear(sName);  // extract year

                    if (SeasonEp != "")
                    {
                        sName = sName.Replace(SeasonEp, "|");
                        sName = ahk.StringSplit(sName, "|", 0);
                    }

                    string ShowName = sName.Trim();

                    string returnLine = sURL + "\n" + PostTitle + "\n" + ShowName + "\n" + SeasonEp;


                    movie.MovieName = ShowName;
                    movie.PostURL = sURL;
                    movie.PostTitle = PostTitle;
                    movie.Year = SeasonEp;

                    if (WriteSQL)
                    {
                        // write to sql db
                        bool added = ssMovie_UpdateInsert(movie);
                    }

                    //sb("Added " + PostTitle + " = " + added.ToString());
                    //ahk.MsgBox(sURL + "\n" + PostTitle + "\n" + ShowName + "\n" + SeasonEp);
                }


                return movie;
            }


            #region === ssTv FUNCTIONS ===

            #region ===== ssTv Object =====

            public struct ssTv
            {
                public string ShowName { get; set; }
                public string SeasonEp { get; set; }
                public string PostTitle { get; set; }
                public string PostURL { get; set; }
                public string RapidGator { get; set; }
                public bool LinksChecked { get; set; }
                public bool InCollection { get; set; }
                public DateTime DateAdded { get; set; }
                public DateTime DateChecked { get; set; }
                public int LinksOnline { get; set; }
                public bool Flagged { get; set; }

                /// <summary>
                /// Season EpNum + RG Links
                /// </summary>
                public Dictionary<string, string> SeasonEpLinks { get; set; }

                /// <summary>
                /// FileSize + RG Links
                /// </summary>
                public Dictionary<string, string> SeasonEpSizes { get; set; }

                /// <summary>
                /// Episdoe Number + InCollection
                /// </summary>
                public Dictionary<string, string> EpInCollection { get; set; }

                public string RapidGator_WEB { get; set; }
                public string RapidGator_HDTV { get; set; }
                public string RapidGator_720P { get; set; }
                public string RapidGator_1080P { get; set; }


                public int Sync_StartPageNum { get; set; }
                public int Sync_TotalPageNums { get; set; }
                public bool Sync_CheckLinkStatus { get; set; }
                public bool Sync_DownloadMissingImages { get; set; }
                public bool Sync_WriteSQL { get; set; }
                public bool Sync_WriteSQLite { get; set; }

                public RadTextBox GUI_PostTitle { get; set; }
                public RadTextBox GUI_ShowName { get; set; }
                public RadTextBox GUI_SeasonEp { get; set; }
                public RadTextBox GUI_URL { get; set; }

            }





            #endregion
            #region ===== ssTv SQLite : Return =====

            public ssTv Return_Object_From_SceneSourceTV(string WhereClause = "[ShowName] = ''", string DbFile = "")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\ssTv.sqlite"; }
                string SelectLine = "Select [ShowName], [SeasonEp], [PostTitle], [PostURL], [RapidGator], [LinksChecked], [InCollection], [DateAdded], [DateChecked], [LinksOnline], [Flagged] From [SceneSourceTV] ";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
                ssTv returnObject = new ssTv();
                int i = 0;
                string Value = "";
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.SeasonEp = ret["SeasonEp"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToBool();
                        returnObject.InCollection = ret["InCollection"].ToBool();
                        returnObject.DateAdded = ret["DateAdded"].ToDateTime();
                        returnObject.DateChecked = ret["DateChecked"].ToDateTime();

                        string linksonline = ret["LinksOnline"].ToString();
                        returnObject.LinksOnline = 0;
                        if (linksonline != "") { returnObject.LinksOnline = linksonline.ToInt(); }

                        returnObject.Flagged = ret["Flagged"].ToBool();
                    }
                }

                return returnObject;
            }

            public List<ssTv> Return_ssTv_List(string WhereClause = "", string DbFile = "", string TableName = "[SceneSourceTV]")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\ssTv.sqlite"; }
                string SelectLine = "Select * From " + TableName;

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

                List<ssTv> ReturnList = new List<ssTv>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        ssTv returnObject = new ssTv();

                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.SeasonEp = ret["SeasonEp"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToBool();
                        returnObject.InCollection = ret["InCollection"].ToBool();
                        returnObject.DateAdded = ret["DateAdded"].ToDateTime();
                        returnObject.DateChecked = ret["DateChecked"].ToDateTime();

                        string linksonline = ret["LinksOnline"].ToString();
                        returnObject.LinksOnline = 0;
                        if (linksonline != "") { returnObject.LinksOnline = linksonline.ToInt(); }

                        returnObject.Flagged = ret["Flagged"].ToBool();

                        ReturnList.Add(returnObject);
                    }
                }

                return ReturnList;
            }

            public DataTable Return_DataTable_From_SceneSourceTV(string DbFile)
            {
                string SelectLine = "Select [ShowName], [SeasonEp], [PostTitle], [PostURL], [RapidGator], [LinksChecked], [InCollection], [DateAdded], [DateChecked], [LinksOnline], [Flagged] From [SceneSourceTV]";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                return ReturnTable;
            }


            #endregion
            #region ===== ssTv SQLite : Update Insert =====

            public bool ssTv_Insert(ssTv inObject, string DbFile = "")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\ssTv.sqlite"; }
                string InsertLine = "Insert Into [SceneSourceTV] (ShowName, SeasonEp, PostTitle, PostURL, RapidGator, LinksChecked, InCollection, DateAdded, DateChecked, LinksOnline, Flagged) values ('" + inObject.ShowName + "', '" + inObject.SeasonEp + "', '" + inObject.PostTitle + "', '" + inObject.PostURL + "', '" + inObject.RapidGator + "', '" + inObject.LinksChecked + "', '" + inObject.InCollection + "', '" + inObject.DateAdded + "', '" + inObject.DateChecked + "', '" + inObject.LinksOnline + "', '" + inObject.Flagged + "')";
                bool Inserted = sqlite.Execute(DbFile, InsertLine);
                if (!Inserted) { ahk.MsgBox("Inserted Into [SceneSourceTV] = " + Inserted.ToString()); }
                return Inserted;
            }

            public bool ssTv_Update(ssTv inObject, string DbFile = "")
            {
                //string UpdateLine = "Update [SceneSourceTV] set ShowName = '" + inObject.ShowName + "', SeasonEp = '" + inObject.SeasonEp + "', PostTitle = '" + inObject.PostTitle + "', PostURL = '" + inObject.PostURL + "', RapidGator = '" + inObject.RapidGator + "', LinksChecked = '" + inObject.LinksChecked + "', InCollection = '" + inObject.InCollection + "', DateAdded = '" + inObject.DateAdded + "', DateChecked = '" + inObject.DateChecked + "', LinksOnline = '" + inObject.LinksOnline + "', Flagged = '" + inObject.Flagged + "' WHERE [Item] = 'Value' ";
                string UpdateLine = "Update [SceneSourceTV] set ";


                if (inObject.ShowName != null) { UpdateLine = UpdateLine + "[ShowName] = '" + inObject.ShowName + "',"; }
                if (inObject.SeasonEp != null) { UpdateLine = UpdateLine + "[SeasonEp] = '" + inObject.SeasonEp + "',"; }
                if (inObject.PostTitle != null) { UpdateLine = UpdateLine + "[PostTitle] = '" + inObject.PostTitle + "',"; }
                if (inObject.PostURL != null) { UpdateLine = UpdateLine + "[PostURL] = '" + inObject.PostURL + "',"; }
                if (inObject.RapidGator != null) { UpdateLine = UpdateLine + "[RapidGator] = '" + inObject.RapidGator + "',"; }
                UpdateLine = UpdateLine + "[LinksChecked] = '" + inObject.LinksChecked + "',";
                UpdateLine = UpdateLine + "[InCollection] = '" + inObject.InCollection + "',";
                if (inObject.DateAdded != null) { UpdateLine = UpdateLine + "[DateAdded] = '" + inObject.DateAdded + "',"; }
                if (inObject.DateChecked != null) { UpdateLine = UpdateLine + "[DateChecked] = '" + inObject.DateChecked + "',"; }
                UpdateLine = UpdateLine + "[LinksOnline] = '" + inObject.LinksOnline + "',";
                UpdateLine = UpdateLine + "[Flagged] = '" + inObject.Flagged + "',";

                UpdateLine = ahk.TrimLast(UpdateLine, 1);
                UpdateLine = UpdateLine + " WHERE [PostURL] = '" + inObject + "'"; // DEFINE CONDITION HERE !!!

                bool Updated = sqlite.Execute(DbFile, UpdateLine);
                return Updated;
            }

            public bool ssTv_UpdateInsert(ssTv obj, string DbFile = "")
            {

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\ssTv.sqlite"; }
                bool Updated = ssTv_Update(obj, DbFile);  // try to update record first
                if (!Updated) { Updated = ssTv_Insert(obj, DbFile); }  // if unable to update, insert new record
                return Updated;
            }


            #endregion
            #region ===== ssTv DataTable =====

            public DataTable Return_ssTv_DataTable(string DbFile = "", string TableName = "ssTv", string WhereClause = "", bool Debug = false)
            {

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\ssTv.sqlite"; }
                string SelectLine = "Select * From [SceneSourceTV]";

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }

                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


                DataTable table = new DataTable();
                table.Columns.Add("ShowName", typeof(string));
                table.Columns.Add("SeasonEp", typeof(string));
                table.Columns.Add("PostTitle", typeof(string));
                table.Columns.Add("PostURL", typeof(string));
                table.Columns.Add("RapidGator", typeof(string));
                table.Columns.Add("LinksChecked", typeof(string));
                table.Columns.Add("InCollection", typeof(string));
                table.Columns.Add("DateAdded", typeof(string));
                table.Columns.Add("DateChecked", typeof(string));
                table.Columns.Add("LinksOnline", typeof(string));
                table.Columns.Add("Flagged", typeof(string));

                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        ssTv returnObject = new ssTv();

                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.SeasonEp = ret["SeasonEp"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToBool();
                        returnObject.InCollection = ret["InCollection"].ToBool();
                        returnObject.DateAdded = ret["DateAdded"].ToDateTime();
                        returnObject.DateChecked = ret["DateChecked"].ToDateTime();

                        string linksonline = ret["LinksOnline"].ToString();
                        returnObject.LinksOnline = 0;
                        if (linksonline != "") { returnObject.LinksOnline = linksonline.ToInt(); }

                        returnObject.Flagged = ret["Flagged"].ToBool();

                        table.Rows.Add(returnObject.ShowName, returnObject.SeasonEp, returnObject.PostTitle, returnObject.PostURL, returnObject.RapidGator, returnObject.LinksChecked, returnObject.InCollection, returnObject.DateAdded, returnObject.DateChecked, returnObject.LinksOnline, returnObject.Flagged);
                    }
                }

                return table;
            }

            public DataTable Create_SceneSourceTV_DataTable(ssTv inObject)
            {
                DataTable table = new DataTable();
                table.Columns.Add("ShowName", typeof(string));
                table.Columns.Add("SeasonEp", typeof(string));
                table.Columns.Add("PostTitle", typeof(string));
                table.Columns.Add("PostURL", typeof(string));
                table.Columns.Add("RapidGator", typeof(string));
                table.Columns.Add("LinksChecked", typeof(string));
                table.Columns.Add("InCollection", typeof(string));
                table.Columns.Add("DateAdded", typeof(string));
                table.Columns.Add("DateChecked", typeof(string));
                table.Columns.Add("LinksOnline", typeof(string));
                table.Columns.Add("Flagged", typeof(string));

                table.Rows.Add(inObject.ShowName, inObject.SeasonEp, inObject.PostTitle, inObject.PostURL, inObject.RapidGator, inObject.LinksChecked, inObject.InCollection, inObject.DateAdded, inObject.DateChecked, inObject.LinksOnline, inObject.Flagged);
                return table;
            }


            #endregion
            #region ===== ssTv DataGridView =====

            public void HideShow_SceneSourceTV_Columns(DataGridView dv)
            {

                try { dv.Columns["ShowName"].Visible = true; } catch { }
                try { dv.Columns["SeasonEp"].Visible = true; } catch { }
                try { dv.Columns["PostTitle"].Visible = true; } catch { }
                try { dv.Columns["PostURL"].Visible = true; } catch { }
                try { dv.Columns["RapidGator"].Visible = true; } catch { }
                try { dv.Columns["LinksChecked"].Visible = true; } catch { }
                try { dv.Columns["InCollection"].Visible = true; } catch { }
                try { dv.Columns["DateAdded"].Visible = true; } catch { }
                try { dv.Columns["DateChecked"].Visible = true; } catch { }
                try { dv.Columns["LinksOnline"].Visible = true; } catch { }
                try { dv.Columns["Flagged"].Visible = true; } catch { }
            }
            public void Enable_SceneSourceTV_Columns(DataGridView dv)
            {

                try { dv.Columns["ShowName"].ReadOnly = true; } catch { }
                try { dv.Columns["SeasonEp"].ReadOnly = true; } catch { }
                try { dv.Columns["PostTitle"].ReadOnly = true; } catch { }
                try { dv.Columns["PostURL"].ReadOnly = true; } catch { }
                try { dv.Columns["RapidGator"].ReadOnly = true; } catch { }
                try { dv.Columns["LinksChecked"].ReadOnly = true; } catch { }
                try { dv.Columns["InCollection"].ReadOnly = true; } catch { }
                try { dv.Columns["DateAdded"].ReadOnly = true; } catch { }
                try { dv.Columns["DateChecked"].ReadOnly = true; } catch { }
                try { dv.Columns["LinksOnline"].ReadOnly = true; } catch { }
                try { dv.Columns["Flagged"].ReadOnly = true; } catch { }
            }

            #endregion
            #region ===== ssTv SQL Functions =====

            // Return ssTv SQL Connection String
            public SqlConnection ssTv_Conn()
            {
                // populate sql connection
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMLucidMedia"].ConnectionString);
                //SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLserver"].ConnectionString);
                // SqlConnection Conn = new SqlConnection("Server=188.168.188.88;DataBase=LucidMedia;Uid=lucidm;Pwd=pass");
                return conn;
            }

            // Generate SQL Table
            public bool ssTv_CreateSQLTable()
            {
                SqlConnection Conn = ssTv_Conn();
                string CreateTableLine = "CREATE TABLE [SceneSourceTV](";
                CreateTableLine = CreateTableLine + "[ShowName] [int] IDENTITY(1,1) NOT NULL,";
                CreateTableLine = CreateTableLine + "[SeasonEp] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[PostTitle] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[PostURL] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[RapidGator] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[LinksChecked] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[InCollection] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[DateAdded] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[DateChecked] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[LinksOnline] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Flagged] [varchar](max) NOT NULL";
                CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                return false;
            }

            public bool ssTv_InsertSQL(ssTv obj)
            {
                SqlConnection Con = ssTv_Conn();
                string SQLLine = "Insert Into " + ssTv_TableName() + " (ShowName, SeasonEp, PostTitle, PostURL, RapidGator, LinksChecked, InCollection, DateAdded, DateChecked, LinksOnline, Flagged) VALUES (@ShowName, @SeasonEp, @PostTitle, @PostURL, @RapidGator, @LinksChecked, @InCollection, @DateAdded, @DateChecked, @LinksOnline, @Flagged)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.ShowName == null) { obj.ShowName = ""; }
                if (obj.SeasonEp == null) { obj.SeasonEp = ""; }
                if (obj.PostTitle == null) { obj.PostTitle = ""; }
                if (obj.PostURL == null) { obj.PostURL = ""; }
                if (obj.RapidGator == null) { obj.RapidGator = ""; }
                //if (obj.LinksChecked == null) { obj.LinksChecked = ""; }
                //if (obj.InCollection == null) { obj.InCollection = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = DateTime.Now; }
                if (obj.DateChecked == null) { obj.DateChecked = DateTime.Now; }
                //if (obj.LinksOnline == null) { obj.LinksOnline = ""; }
                //if (obj.Flagged == null) { obj.Flagged = ""; }
                cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                cmd2.Parameters.AddWithValue(@"SeasonEp", obj.SeasonEp.ToString());
                cmd2.Parameters.AddWithValue(@"PostTitle", obj.PostTitle.ToString());
                cmd2.Parameters.AddWithValue(@"PostURL", obj.PostURL.ToString());
                cmd2.Parameters.AddWithValue(@"RapidGator", obj.RapidGator.ToString());
                cmd2.Parameters.AddWithValue(@"LinksChecked", obj.LinksChecked.ToString());
                cmd2.Parameters.AddWithValue(@"InCollection", obj.InCollection.ToString());
                cmd2.Parameters.AddWithValue(@"DateAdded", DateTime.Now.ToString());
                cmd2.Parameters.AddWithValue(@"DateChecked", DateTime.Now.ToString());
                cmd2.Parameters.AddWithValue(@"LinksOnline", obj.LinksOnline.ToString());
                cmd2.Parameters.AddWithValue(@"Flagged", obj.Flagged.ToString());
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        ssTv_InsertSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        ssTv_InsertSQL(obj);
                    }
                    else
                    {
                        ahk.MsgBox(ex.ToString());
                        return false;
                    }
                }
                Con.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool ssTv_UpdateSQL(ssTv obj)
            {
                SqlConnection Conn = ssTv_Conn();
                string SQLLine = "Update " + ssTv_TableName() + " SET ShowName = @ShowName, SeasonEp = @SeasonEp, PostTitle = @PostTitle, RapidGator = @RapidGator, LinksChecked = @LinksChecked, InCollection = @InCollection, DateChecked = @DateChecked, LinksOnline = @LinksOnline, Flagged = @Flagged WHERE PostURL = @PostURL";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.ShowName == null) { obj.ShowName = ""; }
                if (obj.SeasonEp == null) { obj.SeasonEp = ""; }
                if (obj.PostTitle == null) { obj.PostTitle = ""; }
                if (obj.PostURL == null) { obj.PostURL = ""; }
                if (obj.RapidGator == null) { obj.RapidGator = ""; }
                //if (obj.LinksChecked == null) { obj.LinksChecked = ""; }
                //if (obj.InCollection == null) { obj.InCollection = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = DateTime.Now; }
                if (obj.DateChecked == null) { obj.DateChecked = DateTime.Now; }
                //if (obj.LinksOnline == null) { obj.LinksOnline = ""; }
                //if (obj.Flagged == null) { obj.Flagged = ""; }
                cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                cmd2.Parameters.AddWithValue(@"SeasonEp", obj.SeasonEp.ToString());
                cmd2.Parameters.AddWithValue(@"PostTitle", obj.PostTitle.ToString());
                cmd2.Parameters.AddWithValue(@"PostURL", obj.PostURL.ToString());
                cmd2.Parameters.AddWithValue(@"RapidGator", obj.RapidGator.ToString());
                cmd2.Parameters.AddWithValue(@"LinksChecked", obj.LinksChecked.ToString());
                cmd2.Parameters.AddWithValue(@"InCollection", obj.InCollection.ToString());
                //cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded.ToString());
                cmd2.Parameters.AddWithValue(@"DateChecked", obj.DateChecked.ToString());
                cmd2.Parameters.AddWithValue(@"LinksOnline", obj.LinksOnline.ToString());
                cmd2.Parameters.AddWithValue(@"Flagged", obj.Flagged.ToString());
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        ssTv_UpdateSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        ssTv_UpdateSQL(obj);
                    }
                    else
                    {
                        ahk.MsgBox(ex.ToString());
                        return false;
                    }
                }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool ssTv_UpdateInCollection(ssTv obj)
            {
                SqlConnection Conn = ssTv_Conn();
                string SQLLine = "Update " + ssTv_TableName() + " SET InCollection = @InCollection, DateChecked = @DateChecked WHERE ShowName = @ShowName AND SeasonEp = @SeasonEp";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.ShowName == null) { obj.ShowName = ""; }
                if (obj.SeasonEp == null) { obj.SeasonEp = ""; }

                cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                cmd2.Parameters.AddWithValue(@"SeasonEp", obj.SeasonEp.ToString());
                cmd2.Parameters.AddWithValue(@"InCollection", obj.InCollection.ToString());
                cmd2.Parameters.AddWithValue(@"DateChecked", DateTime.Now.ToString());

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool UpdateShow(ssTv obj)
            {
                SqlConnection Conn = ssTv_Conn();

                obj.DateChecked = DateTime.Now;
                //obj.DateAdded = DateTime.Now;

                bool Updated = ssTv_UpdateSQL(obj);  // try to update record first
                if (!Updated) { Updated = ssTv_InsertSQL(obj); }  // if unable to update, insert new record
                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool ssTv_UpdateIfPopulated(ssTv obj, string ID = "")
            {
                SqlConnection Conn = ssTv_Conn();
                string SQLcmd = "Update " + ssTv_TableName() + " SET ";
                if (obj.ShowName != null) { SQLcmd = SQLcmd + " ShowName = @ShowName,"; }
                if (obj.SeasonEp != null) { SQLcmd = SQLcmd + " SeasonEp = @SeasonEp,"; }
                if (obj.PostTitle != null) { SQLcmd = SQLcmd + " PostTitle = @PostTitle,"; }
                if (obj.PostURL != null) { SQLcmd = SQLcmd + " PostURL = @PostURL,"; }
                if (obj.RapidGator != null) { SQLcmd = SQLcmd + " RapidGator = @RapidGator,"; }
                if (obj.LinksChecked != null) { SQLcmd = SQLcmd + " LinksChecked = @LinksChecked,"; }
                if (obj.InCollection != null) { SQLcmd = SQLcmd + " InCollection = @InCollection,"; }
                if (obj.DateAdded != null) { SQLcmd = SQLcmd + " DateAdded = @DateAdded,"; }
                if (obj.DateChecked != null) { SQLcmd = SQLcmd + " DateChecked = @DateChecked,"; }
                if (obj.LinksOnline != null) { SQLcmd = SQLcmd + " LinksOnline = @LinksOnline,"; }
                if (obj.Flagged != null) { SQLcmd = SQLcmd + " Flagged = @Flagged,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ID = @ID";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.ShowName != null) { cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName); }
                if (obj.SeasonEp != null) { cmd2.Parameters.AddWithValue(@"SeasonEp", obj.SeasonEp); }
                if (obj.PostTitle != null) { cmd2.Parameters.AddWithValue(@"PostTitle", obj.PostTitle); }
                if (obj.PostURL != null) { cmd2.Parameters.AddWithValue(@"PostURL", obj.PostURL); }
                if (obj.RapidGator != null) { cmd2.Parameters.AddWithValue(@"RapidGator", obj.RapidGator); }
                if (obj.LinksChecked != null) { cmd2.Parameters.AddWithValue(@"LinksChecked", obj.LinksChecked); }
                if (obj.InCollection != null) { cmd2.Parameters.AddWithValue(@"InCollection", obj.InCollection); }
                if (obj.DateAdded != null) { cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded); }
                if (obj.DateChecked != null) { cmd2.Parameters.AddWithValue(@"DateChecked", obj.DateChecked); }
                if (obj.LinksOnline != null) { cmd2.Parameters.AddWithValue(@"LinksOnline", obj.LinksOnline); }
                if (obj.Flagged != null) { cmd2.Parameters.AddWithValue(@"Flagged", obj.Flagged); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }


            // returns sql command from tv server db
            public string ssTV_SQL(string Command)
            {
                SqlConnection Conn = ssTv_Conn();
                string SelectLine = Command;

                return sql.SQL_Return_Value(Conn, Command);
            }

            public ssTv ssTv_ReturnSQL(string Where = "")
            {
                SqlConnection Conn = ssTv_Conn();
                string SelectLine = "Select [ShowName],[SeasonEp],[PostTitle],[PostURL],[RapidGator],[LinksChecked],[InCollection],[DateAdded],[DateChecked],[LinksOnline],[Flagged] From " + ssTv_TableName() + " " + Where;
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                ssTv returnObject = new ssTv();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.SeasonEp = ret["SeasonEp"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToBool();
                        returnObject.InCollection = ret["InCollection"].ToBool();
                        returnObject.DateAdded = ret["DateAdded"].ToDateTime();
                        returnObject.DateChecked = ret["DateChecked"].ToDateTime();

                        string linksonline = ret["LinksOnline"].ToString();
                        returnObject.LinksOnline = 0;
                        if (linksonline != "") { returnObject.LinksOnline = linksonline.ToInt(); }

                        returnObject.Flagged = ret["Flagged"].ToBool();
                        return returnObject;
                    }
                }
                return returnObject;
            }



            public List<ssTv> ssTv_ReturnSQLList(string Command = "")
            {
                if (Command == "") { Command = "Select * From " + ssTv_TableName(); }
                SqlConnection Conn = ssTv_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<ssTv> ReturnList = new List<ssTv>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        ssTv returnObject = new ssTv();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.SeasonEp = ret["SeasonEp"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToBool();
                        returnObject.InCollection = ret["InCollection"].ToBool();
                        returnObject.DateAdded = ret["DateAdded"].ToDateTime();
                        returnObject.DateChecked = ret["DateChecked"].ToDateTime();

                        string linksonline = ret["LinksOnline"].ToString();
                        returnObject.LinksOnline = 0;
                        if (linksonline != "") { returnObject.LinksOnline = linksonline.ToInt(); }

                        returnObject.Flagged = ret["Flagged"].ToBool();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }

            public bool ssTv_SQL_to_SQLite(string SqliteDBPath = @"\Db\ssTv.sqlite")
            {
                string SaveFile = SqliteDBPath;
                if (SqliteDBPath == @"\Db\ssTv.sqlite")
                {
                    ahk.FileCreateDir(ahk.AppDir() + @"\Db");
                    SaveFile = ahk.AppDir() + @"\Db\ssTv.sqlite";
                }

                sb.StatusBar("Copying SQL Db to " + SaveFile + "...");
                sqlite.SQLTable_To_NewSQLiteTable(ssTv_Conn(), "SceneSourceTV", "SceneSourceTV", SaveFile, "", false, false, false);
                sb.StatusBar("FINISHED Copying SQL Db to " + SaveFile);

                if (File.Exists(SaveFile)) { return true; } else { return false; }
            }



            public ssTv ShowByPostText(string PostTitle)
            {
                SqlConnection Conn = ssTv_Conn();
                string SelectLine = "Select [ShowName],[SeasonEp],[PostTitle],[PostURL],[RapidGator],[LinksChecked],[InCollection],[DateAdded],[DateChecked],[LinksOnline],[Flagged] From " + ssTv_TableName() + " WHERE [PostTitle] = '" + PostTitle + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                ssTv returnObject = new ssTv();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.SeasonEp = ret["SeasonEp"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToBool();
                        returnObject.InCollection = ret["InCollection"].ToBool();
                        returnObject.DateAdded = ret["DateAdded"].ToDateTime();
                        returnObject.DateChecked = ret["DateChecked"].ToDateTime();

                        string linksonline = ret["LinksOnline"].ToString();
                        returnObject.LinksOnline = 0;
                        if (linksonline != "") { returnObject.LinksOnline = linksonline.ToInt(); }

                        returnObject.Flagged = ret["Flagged"].ToBool();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public ssTv ShowByURL(string PostURL)
            {
                SqlConnection Conn = ssTv_Conn();
                string SelectLine = "Select [ShowName],[SeasonEp],[PostTitle],[PostURL],[RapidGator],[LinksChecked],[InCollection],[DateAdded],[DateChecked],[LinksOnline],[Flagged] From " + ssTv_TableName() + " WHERE [PostURL] = '" + PostURL + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                ssTv returnObject = new ssTv();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.SeasonEp = ret["SeasonEp"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToBool();
                        returnObject.InCollection = ret["InCollection"].ToBool();
                        returnObject.DateAdded = ret["DateAdded"].ToDateTime();
                        returnObject.DateChecked = ret["DateChecked"].ToDateTime();

                        string linksonline = ret["LinksOnline"].ToString();
                        returnObject.LinksOnline = 0;
                        if (linksonline != "") { returnObject.LinksOnline = linksonline.ToInt(); }

                        returnObject.Flagged = ret["Flagged"].ToBool();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public List<ssTv> ShowList(bool UncheckedLinks = false, bool RecheckLinks = false)  // list of show entries from SceneSource Table with Links Found
            {
                string Command = "Select * From " + ssTv_TableName() + " where PostURL like '%scnsrc.me%' and LinksOnline > 0  order by DateAdded desc";

                if (UncheckedLinks) { Command = "select TOP (500) * FROM " + ssTv_TableName() + " where PostURL like '%scnsrc.me%' and LinksChecked = 0 order by DateAdded desc"; }
                if (RecheckLinks) { Command = "select * FROM " + ssTv_TableName() + " where PostURL like '%scnsrc.me%' and LinksChecked != 0 order by DateAdded desc"; }

                SqlConnection Conn = ssTv_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<ssTv> ReturnList = new List<ssTv>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        ssTv returnObject = new ssTv();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.SeasonEp = ret["SeasonEp"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToBool();
                        returnObject.InCollection = ret["InCollection"].ToBool();
                        returnObject.DateAdded = ret["DateAdded"].ToDateTime();
                        returnObject.DateChecked = ret["DateChecked"].ToDateTime();

                        string linksonline = ret["LinksOnline"].ToString();
                        returnObject.LinksOnline = 0;
                        if (linksonline != "") { returnObject.LinksOnline = linksonline.ToInt(); }

                        returnObject.Flagged = ret["Flagged"].ToBool();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }


            #endregion

            #endregion


            #region === ssMovie FUNCTIONS ===

            #region ===== ssMovie Object =====

            public struct ssMovie
            {
                public string MovieName { get; set; }
                public string IMDbID { get; set; }
                public string Year { get; set; }
                public string PostTitle { get; set; }
                public string PostURL { get; set; }
                public string ImageURL { get; set; }
                public string RapidGator { get; set; }
                public string LinksChecked { get; set; }
                public string InCollection { get; set; }
                public string DateAdded { get; set; }
                public string Cateogry { get; set; }
            }
            public ssMovie Return_ssMovie(string MovieName = "", string IMDbID = "", string Year = "", string PostTitle = "", string PostURL = "", string ImageURL = "", string RapidGator = "", string LinksChecked = "", string InCollection = "", string DateAdded = "", string Cateogry = "")
            {
                ssMovie obj = new ssMovie();
                obj.MovieName = MovieName;
                obj.IMDbID = IMDbID;
                obj.Year = Year;
                obj.PostTitle = PostTitle;
                obj.PostURL = PostURL;
                obj.ImageURL = ImageURL;
                obj.RapidGator = RapidGator;
                obj.LinksChecked = LinksChecked;
                obj.InCollection = InCollection;
                obj.DateAdded = DateAdded;
                obj.Cateogry = Cateogry;

                return obj;
            }

            //  Fix illegal characters before Sql/Sqlite Db Inserts
            public ssMovie ssMovie_FixChars(ssMovie ToFix)
            {
                ssMovie Fixed = new ssMovie();

                Fixed.MovieName = ToFix.MovieName.Replace("'", "''");
                Fixed.IMDbID = ToFix.IMDbID.Replace("'", "''");
                Fixed.Year = ToFix.Year.Replace("'", "''");
                Fixed.PostTitle = ToFix.PostTitle.Replace("'", "''");
                Fixed.PostURL = ToFix.PostURL.Replace("'", "''");
                Fixed.ImageURL = ToFix.ImageURL.Replace("'", "''");
                Fixed.RapidGator = ToFix.RapidGator.Replace("'", "''");
                Fixed.LinksChecked = ToFix.LinksChecked.Replace("'", "''");
                Fixed.InCollection = ToFix.InCollection.Replace("'", "''");
                Fixed.DateAdded = ToFix.DateAdded.Replace("'", "''");
                Fixed.Cateogry = ToFix.Cateogry.Replace("'", "''");

                return Fixed;
            }

            // Compare two objects to see if they have identical values
            public bool ssMovie_Changed(ssMovie OldVal, ssMovie NewVal)
            {
                ssMovie diff = new ssMovie();
                List<string> diffList = new List<string>();
                bool different = false;
                if (OldVal.MovieName == null) { OldVal.MovieName = ""; }
                if (NewVal.MovieName == null) { NewVal.MovieName = ""; }
                if (OldVal.MovieName != NewVal.MovieName) { different = true; }
                if (OldVal.IMDbID == null) { OldVal.IMDbID = ""; }
                if (NewVal.IMDbID == null) { NewVal.IMDbID = ""; }
                if (OldVal.IMDbID != NewVal.IMDbID) { different = true; }
                if (OldVal.Year == null) { OldVal.Year = ""; }
                if (NewVal.Year == null) { NewVal.Year = ""; }
                if (OldVal.Year != NewVal.Year) { different = true; }
                if (OldVal.PostTitle == null) { OldVal.PostTitle = ""; }
                if (NewVal.PostTitle == null) { NewVal.PostTitle = ""; }
                if (OldVal.PostTitle != NewVal.PostTitle) { different = true; }
                if (OldVal.PostURL == null) { OldVal.PostURL = ""; }
                if (NewVal.PostURL == null) { NewVal.PostURL = ""; }
                if (OldVal.PostURL != NewVal.PostURL) { different = true; }
                if (OldVal.ImageURL == null) { OldVal.ImageURL = ""; }
                if (NewVal.ImageURL == null) { NewVal.ImageURL = ""; }
                if (OldVal.ImageURL != NewVal.ImageURL) { different = true; }
                if (OldVal.RapidGator == null) { OldVal.RapidGator = ""; }
                if (NewVal.RapidGator == null) { NewVal.RapidGator = ""; }
                if (OldVal.RapidGator != NewVal.RapidGator) { different = true; }
                if (OldVal.LinksChecked == null) { OldVal.LinksChecked = ""; }
                if (NewVal.LinksChecked == null) { NewVal.LinksChecked = ""; }
                if (OldVal.LinksChecked != NewVal.LinksChecked) { different = true; }
                if (OldVal.InCollection == null) { OldVal.InCollection = ""; }
                if (NewVal.InCollection == null) { NewVal.InCollection = ""; }
                if (OldVal.InCollection != NewVal.InCollection) { different = true; }
                if (OldVal.DateAdded == null) { OldVal.DateAdded = ""; }
                if (NewVal.DateAdded == null) { NewVal.DateAdded = ""; }
                if (OldVal.DateAdded != NewVal.DateAdded) { different = true; }
                if (OldVal.Cateogry == null) { OldVal.Cateogry = ""; }
                if (NewVal.Cateogry == null) { NewVal.Cateogry = ""; }
                if (OldVal.Cateogry != NewVal.Cateogry) { different = true; }
                return different;
            }

            // Returns object containing the new values different from the old values in object comparison
            public ssMovie ssMovie_Diff(ssMovie OldVal, ssMovie NewVal)
            {
                ssMovie diff = new ssMovie();
                if (OldVal.MovieName != NewVal.MovieName) { diff.MovieName = NewVal.MovieName; }
                if (OldVal.IMDbID != NewVal.IMDbID) { diff.IMDbID = NewVal.IMDbID; }
                if (OldVal.Year != NewVal.Year) { diff.Year = NewVal.Year; }
                if (OldVal.PostTitle != NewVal.PostTitle) { diff.PostTitle = NewVal.PostTitle; }
                if (OldVal.PostURL != NewVal.PostURL) { diff.PostURL = NewVal.PostURL; }
                if (OldVal.ImageURL != NewVal.ImageURL) { diff.ImageURL = NewVal.ImageURL; }
                if (OldVal.RapidGator != NewVal.RapidGator) { diff.RapidGator = NewVal.RapidGator; }
                if (OldVal.LinksChecked != NewVal.LinksChecked) { diff.LinksChecked = NewVal.LinksChecked; }
                if (OldVal.InCollection != NewVal.InCollection) { diff.InCollection = NewVal.InCollection; }
                if (OldVal.DateAdded != NewVal.DateAdded) { diff.DateAdded = NewVal.DateAdded; }
                if (OldVal.Cateogry != NewVal.Cateogry) { diff.Cateogry = NewVal.Cateogry; }
                return diff;
            }

            // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
            public List<string> ssMovie_DiffList(ssMovie OldVal, ssMovie NewVal)
            {
                List<string> diffList = new List<string>();
                if (OldVal.MovieName != NewVal.MovieName) { diffList.Add("Changed MovieName Value From " + OldVal.MovieName + " To " + NewVal.MovieName); }
                if (OldVal.IMDbID != NewVal.IMDbID) { diffList.Add("Changed IMDbID Value From " + OldVal.IMDbID + " To " + NewVal.IMDbID); }
                if (OldVal.Year != NewVal.Year) { diffList.Add("Changed Year Value From " + OldVal.Year + " To " + NewVal.Year); }
                if (OldVal.PostTitle != NewVal.PostTitle) { diffList.Add("Changed PostTitle Value From " + OldVal.PostTitle + " To " + NewVal.PostTitle); }
                if (OldVal.PostURL != NewVal.PostURL) { diffList.Add("Changed PostURL Value From " + OldVal.PostURL + " To " + NewVal.PostURL); }
                if (OldVal.ImageURL != NewVal.ImageURL) { diffList.Add("Changed ImageURL Value From " + OldVal.ImageURL + " To " + NewVal.ImageURL); }
                if (OldVal.RapidGator != NewVal.RapidGator) { diffList.Add("Changed RapidGator Value From " + OldVal.RapidGator + " To " + NewVal.RapidGator); }
                if (OldVal.LinksChecked != NewVal.LinksChecked) { diffList.Add("Changed LinksChecked Value From " + OldVal.LinksChecked + " To " + NewVal.LinksChecked); }
                if (OldVal.InCollection != NewVal.InCollection) { diffList.Add("Changed InCollection Value From " + OldVal.InCollection + " To " + NewVal.InCollection); }
                if (OldVal.DateAdded != NewVal.DateAdded) { diffList.Add("Changed DateAdded Value From " + OldVal.DateAdded + " To " + NewVal.DateAdded); }
                if (OldVal.Cateogry != NewVal.Cateogry) { diffList.Add("Changed Cateogry Value From " + OldVal.Cateogry + " To " + NewVal.Cateogry); }
                return diffList;
            }


            #endregion
            #region ===== ssMovie SQLite : Return =====

            public ssMovie Return_Object_From_SceneSourceMovies(string DbFile, string WhereClause = "[MovieName] = ''")
            {
                string SelectLine = "Select [MovieName], [IMDbID], [Year], [PostTitle], [PostURL], [ImageURL], [RapidGator], [LinksChecked], [InCollection], [DateAdded], [Cateogry] From [SceneSourceTV] ";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
                ssMovie returnObject = new ssMovie();
                int i = 0;
                string Value = "";
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.MovieName = ret["MovieName"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.Year = ret["Year"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.ImageURL = ret["ImageURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToString();
                        returnObject.InCollection = ret["InCollection"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.Cateogry = ret["Cateogry"].ToString();
                    }
                }

                return returnObject;
            }

            public List<ssMovie> Return_ssMovie_List(string DbFile, string TableName = "[SceneSourceTV]", string WhereClause = "")
            {
                string SelectLine = "Select * From " + TableName;

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

                List<ssMovie> ReturnList = new List<ssMovie>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        ssMovie returnObject = new ssMovie();

                        returnObject.MovieName = ret["MovieName"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.Year = ret["Year"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.ImageURL = ret["ImageURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToString();
                        returnObject.InCollection = ret["InCollection"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.Cateogry = ret["Cateogry"].ToString();

                        ReturnList.Add(returnObject);
                    }
                }

                return ReturnList;
            }

            public DataTable Return_DataTable_From_SceneSourceMovies(string DbFile)
            {
                string SelectLine = "Select [MovieName], [IMDbID], [Year], [PostTitle], [PostURL], [ImageURL], [RapidGator], [LinksChecked], [InCollection], [DateAdded], [Cateogry] From [SceneSourceTV]";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                return ReturnTable;
            }


            #endregion
            #region ===== ssMovie SQLite : Update Insert =====

            public bool ssMovie_Insert(string DbFile, ssMovie inObject)
            {
                string InsertLine = "Insert Into [SceneSourceTV] (MovieName, IMDbID, Year, PostTitle, PostURL, ImageURL, RapidGator, LinksChecked, InCollection, DateAdded, Cateogry) values ('" + inObject.MovieName + "', '" + inObject.IMDbID + "', '" + inObject.Year + "', '" + inObject.PostTitle + "', '" + inObject.PostURL + "', '" + inObject.ImageURL + "', '" + inObject.RapidGator + "', '" + inObject.LinksChecked + "', '" + inObject.InCollection + "', '" + inObject.DateAdded + "', '" + inObject.Cateogry + "')";
                bool Inserted = sqlite.Execute(DbFile, InsertLine);
                if (!Inserted) { ahk.MsgBox("Inserted Into [SceneSourceTV] = " + Inserted.ToString()); }
                return Inserted;
            }

            public bool ssMovie_Update(string DbFile, ssMovie inObject, string WHERE = "[Item] = 'Value'")
            {
                //string UpdateLine = "Update [SceneSourceTV] set MovieName = '" + inObject.MovieName + "', IMDbID = '" + inObject.IMDbID + "', Year = '" + inObject.Year + "', PostTitle = '" + inObject.PostTitle + "', PostURL = '" + inObject.PostURL + "', ImageURL = '" + inObject.ImageURL + "', RapidGator = '" + inObject.RapidGator + "', LinksChecked = '" + inObject.LinksChecked + "', InCollection = '" + inObject.InCollection + "', DateAdded = '" + inObject.DateAdded + "', Cateogry = '" + inObject.Cateogry + "' WHERE [Item] = 'Value' ";
                string UpdateLine = "Update [SceneSourceTV] set ";


                if (inObject.MovieName != null) { UpdateLine = UpdateLine + "[MovieName] = '" + inObject.MovieName + "',"; }
                if (inObject.IMDbID != null) { UpdateLine = UpdateLine + "[IMDbID] = '" + inObject.IMDbID + "',"; }
                if (inObject.Year != null) { UpdateLine = UpdateLine + "[Year] = '" + inObject.Year + "',"; }
                if (inObject.PostTitle != null) { UpdateLine = UpdateLine + "[PostTitle] = '" + inObject.PostTitle + "',"; }
                if (inObject.PostURL != null) { UpdateLine = UpdateLine + "[PostURL] = '" + inObject.PostURL + "',"; }
                if (inObject.ImageURL != null) { UpdateLine = UpdateLine + "[ImageURL] = '" + inObject.ImageURL + "',"; }
                if (inObject.RapidGator != null) { UpdateLine = UpdateLine + "[RapidGator] = '" + inObject.RapidGator + "',"; }
                if (inObject.LinksChecked != null) { UpdateLine = UpdateLine + "[LinksChecked] = '" + inObject.LinksChecked + "',"; }
                if (inObject.InCollection != null) { UpdateLine = UpdateLine + "[InCollection] = '" + inObject.InCollection + "',"; }
                if (inObject.DateAdded != null) { UpdateLine = UpdateLine + "[DateAdded] = '" + inObject.DateAdded + "',"; }
                if (inObject.Cateogry != null) { UpdateLine = UpdateLine + "[Cateogry] = '" + inObject.Cateogry + "',"; }

                UpdateLine = ahk.TrimLast(UpdateLine, 1);
                UpdateLine = UpdateLine + " WHERE " + WHERE;

                bool Updated = sqlite.Execute(DbFile, UpdateLine);
                return Updated;
            }

            public bool ssMovie_UpdateInsert(string DbFile, ssMovie obj, string WhereClause = "")
            {
                bool Updated = ssMovie_Update(DbFile, obj, WhereClause);  // try to update record first
                if (!Updated) { Updated = ssMovie_Insert(DbFile, obj); }  // if unable to update, insert new record
                return Updated;
            }


            #endregion
            #region ===== ssMovie DataTable =====

            public DataTable Return_ssMovie_DataTable(string DbFile, string TableName = "ssMovie", string WhereClause = "", bool Debug = false)
            {
                string SelectLine = "Select * From [SceneSourceTV]";

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }

                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


                DataTable table = new DataTable();
                table.Columns.Add("MovieName", typeof(string));
                table.Columns.Add("IMDbID", typeof(string));
                table.Columns.Add("Year", typeof(string));
                table.Columns.Add("PostTitle", typeof(string));
                table.Columns.Add("PostURL", typeof(string));
                table.Columns.Add("ImageURL", typeof(string));
                table.Columns.Add("RapidGator", typeof(string));
                table.Columns.Add("LinksChecked", typeof(string));
                table.Columns.Add("InCollection", typeof(string));
                table.Columns.Add("DateAdded", typeof(string));
                table.Columns.Add("Cateogry", typeof(string));

                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        ssMovie returnObject = new ssMovie();

                        returnObject.MovieName = ret["MovieName"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.Year = ret["Year"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.ImageURL = ret["ImageURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToString();
                        returnObject.InCollection = ret["InCollection"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.Cateogry = ret["Cateogry"].ToString();

                        table.Rows.Add(returnObject.MovieName, returnObject.IMDbID, returnObject.Year, returnObject.PostTitle, returnObject.PostURL, returnObject.ImageURL, returnObject.RapidGator, returnObject.LinksChecked, returnObject.InCollection, returnObject.DateAdded, returnObject.Cateogry);
                    }
                }

                return table;
            }

            public DataTable Create_SceneSourceTV_DataTable(ssMovie inObject)
            {
                DataTable table = new DataTable();
                table.Columns.Add("MovieName", typeof(string));
                table.Columns.Add("IMDbID", typeof(string));
                table.Columns.Add("Year", typeof(string));
                table.Columns.Add("PostTitle", typeof(string));
                table.Columns.Add("PostURL", typeof(string));
                table.Columns.Add("ImageURL", typeof(string));
                table.Columns.Add("RapidGator", typeof(string));
                table.Columns.Add("LinksChecked", typeof(string));
                table.Columns.Add("InCollection", typeof(string));
                table.Columns.Add("DateAdded", typeof(string));
                table.Columns.Add("Cateogry", typeof(string));

                table.Rows.Add(inObject.MovieName, inObject.IMDbID, inObject.Year, inObject.PostTitle, inObject.PostURL, inObject.ImageURL, inObject.RapidGator, inObject.LinksChecked, inObject.InCollection, inObject.DateAdded, inObject.Cateogry);
                return table;
            }


            #endregion
            #region ===== ssMovie SQL Functions =====

            // Return ssMovie SQL Connection String
            public SqlConnection ssMovie_Conn()
            {
                // populate sql connection
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMLucidMedia"].ConnectionString);
                // SqlConnection Conn = new SqlConnection("Server=188.168.188.88;DataBase=LucidMedia;Uid=lucidm;Pwd=pass");
                return conn;
            }


            // Generate SQL Table
            public bool ssMovie_CreateSQLTable()
            {
                SqlConnection Conn = ssMovie_Conn();
                string CreateTableLine = "CREATE TABLE [SceneSourceTV](";
                CreateTableLine = CreateTableLine + "[MovieName] [int] IDENTITY(1,1) NOT NULL,";
                CreateTableLine = CreateTableLine + "[IMDbID] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Year] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[PostTitle] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[PostURL] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[ImageURL] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[RapidGator] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[LinksChecked] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[InCollection] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[DateAdded] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Cateogry] [varchar](max) NOT NULL";
                CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                return false;
            }

            public bool ssMovie_InsertSQL(ssMovie obj)
            {
                SqlConnection Con = ssMovie_Conn();
                string SQLLine = "Insert Into " + ssMovie_TableName() + " (MovieName, IMDbID, Year, PostTitle, PostURL, ImageURL, RapidGator, LinksChecked, InCollection, DateAdded, Cateogry) VALUES (@MovieName, @IMDbID, @Year, @PostTitle, @PostURL, @ImageURL, @RapidGator, @LinksChecked, @InCollection, @DateAdded, @Cateogry)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.MovieName == null) { obj.MovieName = ""; }
                if (obj.IMDbID == null) { obj.IMDbID = ""; }
                if (obj.Year == null) { obj.Year = ""; }
                if (obj.PostTitle == null) { obj.PostTitle = ""; }
                if (obj.PostURL == null) { obj.PostURL = ""; }
                if (obj.ImageURL == null) { obj.ImageURL = ""; }
                if (obj.RapidGator == null) { obj.RapidGator = ""; }
                if (obj.LinksChecked == null) { obj.LinksChecked = ""; }
                if (obj.InCollection == null) { obj.InCollection = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = ""; }
                if (obj.Cateogry == null) { obj.Cateogry = ""; }
                cmd2.Parameters.AddWithValue(@"MovieName", obj.MovieName.ToString());
                cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID.ToString());
                cmd2.Parameters.AddWithValue(@"Year", obj.Year.ToString());
                cmd2.Parameters.AddWithValue(@"PostTitle", obj.PostTitle.ToString());
                cmd2.Parameters.AddWithValue(@"PostURL", obj.PostURL.ToString());
                cmd2.Parameters.AddWithValue(@"ImageURL", obj.ImageURL.ToString());
                cmd2.Parameters.AddWithValue(@"RapidGator", obj.RapidGator.ToString());
                cmd2.Parameters.AddWithValue(@"LinksChecked", obj.LinksChecked.ToString());
                cmd2.Parameters.AddWithValue(@"InCollection", obj.InCollection.ToString());
                cmd2.Parameters.AddWithValue(@"DateAdded", DateTime.Now.ToString());
                cmd2.Parameters.AddWithValue(@"Cateogry", obj.Cateogry.ToString());
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        ssMovie_InsertSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        ssMovie_InsertSQL(obj);
                    }
                    else
                    {
                        ahk.MsgBox(ex.ToString());
                        return false;
                    }
                }
                Con.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool ssMovie_UpdateSQL(ssMovie obj)
            {
                SqlConnection Conn = ssMovie_Conn();
                string SQLLine = "Update " + ssMovie_TableName() + " SET MovieName = @MovieName, IMDbID = @IMDbID, Year = @Year, PostTitle = @PostTitle, ImageURL = @ImageURL, RapidGator = @RapidGator, LinksChecked = @LinksChecked, InCollection = @InCollection, Cateogry = @Cateogry WHERE PostURL = @PostURL";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.MovieName == null) { obj.MovieName = ""; }
                if (obj.IMDbID == null) { obj.IMDbID = ""; }
                if (obj.Year == null) { obj.Year = ""; }
                if (obj.PostTitle == null) { obj.PostTitle = ""; }
                if (obj.PostURL == null) { obj.PostURL = ""; }
                if (obj.ImageURL == null) { obj.ImageURL = ""; }
                if (obj.RapidGator == null) { obj.RapidGator = ""; }
                if (obj.LinksChecked == null) { obj.LinksChecked = ""; }
                if (obj.InCollection == null) { obj.InCollection = ""; }
                //if (obj.DateAdded == null) { obj.DateAdded = ""; }
                if (obj.Cateogry == null) { obj.Cateogry = ""; }
                cmd2.Parameters.AddWithValue(@"MovieName", obj.MovieName.ToString());
                cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID.ToString());
                cmd2.Parameters.AddWithValue(@"Year", obj.Year.ToString());
                cmd2.Parameters.AddWithValue(@"PostTitle", obj.PostTitle.ToString());
                cmd2.Parameters.AddWithValue(@"PostURL", obj.PostURL.ToString());
                cmd2.Parameters.AddWithValue(@"ImageURL", obj.ImageURL.ToString());
                cmd2.Parameters.AddWithValue(@"RapidGator", obj.RapidGator.ToString());
                cmd2.Parameters.AddWithValue(@"LinksChecked", obj.LinksChecked.ToString());
                cmd2.Parameters.AddWithValue(@"InCollection", obj.InCollection.ToString());
                //cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded.ToString());
                cmd2.Parameters.AddWithValue(@"Cateogry", obj.Cateogry.ToString());
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        ssMovie_UpdateSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        ssMovie_UpdateSQL(obj);
                    }
                    else
                    {
                        ahk.MsgBox(ex.ToString());
                        return false;
                    }
                }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool ssMovie_UpdateInsert(ssMovie obj)
            {
                SqlConnection Conn = ssMovie_Conn();
                bool Updated = ssMovie_UpdateSQL(obj);  // try to update record first
                if (!Updated) { Updated = ssMovie_InsertSQL(obj); }  // if unable to update, insert new record
                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool ssMovie_UpdateIfPopulated(ssMovie obj, string ID = "")
            {
                SqlConnection Conn = ssMovie_Conn();
                string SQLcmd = "Update " + ssMovie_TableName() + " SET ";
                if (obj.MovieName != null) { SQLcmd = SQLcmd + " MovieName = @MovieName,"; }
                if (obj.IMDbID != null) { SQLcmd = SQLcmd + " IMDbID = @IMDbID,"; }
                if (obj.Year != null) { SQLcmd = SQLcmd + " Year = @Year,"; }
                if (obj.PostTitle != null) { SQLcmd = SQLcmd + " PostTitle = @PostTitle,"; }
                if (obj.PostURL != null) { SQLcmd = SQLcmd + " PostURL = @PostURL,"; }
                if (obj.ImageURL != null) { SQLcmd = SQLcmd + " ImageURL = @ImageURL,"; }
                if (obj.RapidGator != null) { SQLcmd = SQLcmd + " RapidGator = @RapidGator,"; }
                if (obj.LinksChecked != null) { SQLcmd = SQLcmd + " LinksChecked = @LinksChecked,"; }
                if (obj.InCollection != null) { SQLcmd = SQLcmd + " InCollection = @InCollection,"; }
                if (obj.DateAdded != null) { SQLcmd = SQLcmd + " DateAdded = @DateAdded,"; }
                if (obj.Cateogry != null) { SQLcmd = SQLcmd + " Cateogry = @Cateogry,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ID = @ID";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.MovieName != null) { cmd2.Parameters.AddWithValue(@"MovieName", obj.MovieName); }
                if (obj.IMDbID != null) { cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID); }
                if (obj.Year != null) { cmd2.Parameters.AddWithValue(@"Year", obj.Year); }
                if (obj.PostTitle != null) { cmd2.Parameters.AddWithValue(@"PostTitle", obj.PostTitle); }
                if (obj.PostURL != null) { cmd2.Parameters.AddWithValue(@"PostURL", obj.PostURL); }
                if (obj.ImageURL != null) { cmd2.Parameters.AddWithValue(@"ImageURL", obj.ImageURL); }
                if (obj.RapidGator != null) { cmd2.Parameters.AddWithValue(@"RapidGator", obj.RapidGator); }
                if (obj.LinksChecked != null) { cmd2.Parameters.AddWithValue(@"LinksChecked", obj.LinksChecked); }
                if (obj.InCollection != null) { cmd2.Parameters.AddWithValue(@"InCollection", obj.InCollection); }
                if (obj.DateAdded != null) { cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded); }
                if (obj.Cateogry != null) { cmd2.Parameters.AddWithValue(@"Cateogry", obj.Cateogry); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public ssMovie ssMovie_ReturnSQL(string ID = "")
            {
                SqlConnection Conn = ssMovie_Conn();
                string SelectLine = "Select [MovieName],[IMDbID],[Year],[PostTitle],[PostURL],[ImageURL],[RapidGator],[LinksChecked],[InCollection],[DateAdded],[Cateogry] From " + ssMovie_TableName() + " WHERE ID = '" + ID + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                ssMovie returnObject = new ssMovie();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.MovieName = ret["MovieName"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.Year = ret["Year"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.ImageURL = ret["ImageURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToString();
                        returnObject.InCollection = ret["InCollection"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.Cateogry = ret["Cateogry"].ToString();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public List<ssMovie> ssMovie_ReturnSQLList(string Command = "")
            {
                if (Command == "") { Command = ""; }
                SqlConnection Conn = ssMovie_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<ssMovie> ReturnList = new List<ssMovie>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        ssMovie returnObject = new ssMovie();
                        returnObject.MovieName = ret["MovieName"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.Year = ret["Year"].ToString();
                        returnObject.PostTitle = ret["PostTitle"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.ImageURL = ret["ImageURL"].ToString();
                        returnObject.RapidGator = ret["RapidGator"].ToString();
                        returnObject.LinksChecked = ret["LinksChecked"].ToString();
                        returnObject.InCollection = ret["InCollection"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.Cateogry = ret["Cateogry"].ToString();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }




            #endregion

            #endregion


            public bool URLinIndex(string URL)
            {
                SqlConnection Conn = ssTv_Conn();
                string SelectLine = "Select [ShowName] From " + ssTv_TableName() + " WHERE PostURL = '" + URL + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                if (ReturnTable != null) { return true; }
                return false;
            }

            public bool UpdateRGLinks(string URL, string Links)
            {
                SqlConnection Conn = ssTv_Conn();
                string SQLLine = "Update " + ssTv_TableName() + " SET RapidGator = @RapidGator, LinksChecked = @LinksChecked WHERE PostURL = @PostURL";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);

                //cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                //cmd2.Parameters.AddWithValue(@"SeasonEp", obj.SeasonEp.ToString());
                //cmd2.Parameters.AddWithValue(@"PostTitle", obj.PostTitle.ToString());
                cmd2.Parameters.AddWithValue(@"PostURL", URL);
                cmd2.Parameters.AddWithValue(@"RapidGator", Links);
                cmd2.Parameters.AddWithValue(@"LinksChecked", true);
                //cmd2.Parameters.AddWithValue(@"InCollection", obj.InCollection.ToString());
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public void _Backfill_MovieLinks(string Cat)
            {
                //string Cat = "DVDRip";

                string cmd = "select * FROM " + ssMovie_TableName() + " where Cateogry = '" + Cat + "' order by MovieName asc";
                //if (Reverse) { cmd = "select * FROM [lucidmedia].[lucidmethod].[SceneSourceMovies] where LinksChecked = '0' and Cateogry = 'DVDRip' order by MovieName desc"; }

                List<ssMovie> movies = ssMovie_ReturnSQLList(cmd);

                //List<ssMovie> updateMovies = new List<ssMovie>();  // create list of items to update in batch

                string saveDir = ahk.AppDir() + "\\SceneSource\\" + Cat + "\\PostHTML";
                ahk.FileCreateDir(saveDir);

                int i = 1;
                foreach (ssMovie movie in movies)
                {
                    ssMovie Movie = movie;
                    sb.StatusBar("Movie " + i + "/" + movies.Count, 2); i++;

                    string html = web.DownloadHTML(movie.PostURL);

                    //https://www.scnsrc.me/borg-vs-mcenroe-2017-bdrip-x264-amiable

                    string name = "";
                    List<string> parts = ahk.StringSplit_List(Movie.PostURL, "/");
                    foreach (string part in parts) { name = part; }

                    if (html.Contains("rapidgator.net") || html.Contains("rg.net"))
                    {
                        ahk.FileAppend(html, saveDir + "\\" + name + ".txt");
                    }




                    //List<string> rgLinks = web.Rapidgator_LinkList(html);

                    //pro.SetupProgressBar(dispProgress, rgLinks.Count);
                    //pro.ResetProgress(dispProgress);

                    //List<string> rgLinksChecked = new List<string>();

                    //int p = 1;
                    //foreach (string item in rgLinks)
                    //{
                    //    pro.UpdateProgress(dispProgress, p + "/" + rgLinks.Count()); p++;

                    //    _Web.RapidGator.RGInfo rapid = rg.RapidGatorCheckStatus(item);  // check if link is online

                    //    if (rapid.FileOnline) { rgLinksChecked.Add(item + " (" + rapid.FileSize + ")"); }  // if online add to list

                    //}

                    //string goodLinks = lst.List_To_String_NewLines(rgLinksChecked);

                    //Movie.RapidGator = goodLinks;
                    //Movie.LinksChecked = "1";
                    //string id = prs.Extract_IMDbID(html);


                    //Movie.IMDbID = id;


                    //// build list of movies to update in db in batch (every 10th movie update and start list over)
                    //if (updateMovies.Count < 10) { updateMovies.Add(Movie); }
                    //else
                    //{
                    //    updateMovies.Add(Movie);
                    //    foreach (ssMovie mov in updateMovies)
                    //    {
                    //        bool Updated = ssMovie_UpdateSQL(mov);
                    //    }
                    //    updateMovies = new List<ssMovie>();
                    //}

                }
            }



            #endregion

            #region === Convert Links ===

            /// <summary>
            /// Parses RG Link Dictionary and Populates RG Link Values by File Format
            /// </summary>
            /// <param name="epPost"></param>
            /// <param name="CheckLinks"></param>
            /// <returns></returns>
            public ssTv DesiredRGLink(ssTv epPost, bool CheckLinks = true)
            {
                if (epPost.SeasonEpSizes == null) { epPost = ParseRGLinks(epPost, true); }
                else
                {
                    if (CheckLinks) { epPost = ParseRGLinks(epPost, true); }
                }

                // Store keys in a List.
                List<string> list = new List<string>(epPost.SeasonEpSizes.Keys);
                // Loop through list.
                foreach (string k in list)
                {
                    if (epPost.SeasonEpSizes.ContainsKey("WEB"))
                    {
                        List<string> lines = ahk.StringSplit_List(epPost.SeasonEpSizes["WEB"], "\n");
                        foreach (string line in lines) { epPost.RapidGator_WEB = line; break; }
                    }
                    if (epPost.SeasonEpSizes.ContainsKey("HDTV"))
                    {
                        List<string> lines = ahk.StringSplit_List(epPost.SeasonEpSizes["HDTV"], "\n");
                        foreach (string line in lines) { epPost.RapidGator_HDTV = line; break; }
                    }
                    if (epPost.SeasonEpSizes.ContainsKey("720P"))
                    {
                        List<string> lines = ahk.StringSplit_List(epPost.SeasonEpSizes["720P"], "\n");
                        foreach (string line in lines) { epPost.RapidGator_720P = line; break; }
                    }
                    if (epPost.SeasonEpSizes.ContainsKey("1080P"))
                    {
                        List<string> lines = ahk.StringSplit_List(epPost.SeasonEpSizes["1080P"], "\n");
                        foreach (string line in lines) { epPost.RapidGator_1080P = line; break; }
                    }
                }

                return epPost;
            }

            /// <summary>
            /// Parse RapidGator Links into Dictionaries for Display
            /// </summary>
            /// <param name="show"></param>
            /// <param name="CheckOnlineStatus"></param>
            /// <returns></returns>
            public ssTv ParseRGLinks(ssTv show, bool CheckOnlineStatus = true)
            {
                ssTv showReturn = show;

                if (CheckOnlineStatus) { showReturn = BatchUpdate_Db_Links(show); }

                showReturn.SeasonEpLinks = ParsedSeasonEpLinks(showReturn);

                showReturn.SeasonEpSizes = ParsedEpLinks_SizeQuality(showReturn);

                showReturn = DesiredRGLink(showReturn, false);  // Parses RG Link Dictionary and Populates RG Link Values by File Format

                return showReturn;
            }

            /// <summary>
            /// Creates Dictionary with S00E00 as the Key, then Contains Episode RapidGator Links
            /// </summary>
            /// <param name="show">Show Object Containing RapidGator Links to Parse</param>
            /// <returns></returns>
            public Dictionary<string, string> ParsedSeasonEpLinks(ssTv show)
            {
                // extract list of rapidgator links from show object
                List<string> LINKS = lst.Text_To_List(show.RapidGator);

                Dictionary<string, string> linkDict = new Dictionary<string, string>();

                List<string> SeasonEpNums = new List<string>();

                // Extract Unique List of Episode Numbers
                foreach (string lnk in LINKS)
                {
                    string rapidLink = "";

                    if (lnk.Contains("|")) { rapidLink = ahk.StringSplit(lnk, "|", 0); }

                    string ShowName = ShowName_FromFilePath(rapidLink);
                    string SeasonEp = ShowEp_FromFilePath(rapidLink);

                    ShowName = ShowName.Replace(".", " "); ShowName = ShowName.Trim();
                    SeasonEp = SeasonEp.Trim();

                    // See whether it contains this string.
                    if (linkDict.ContainsKey(SeasonEp))
                    {
                        // grab existing dictionary value, add link
                        if (linkDict.TryGetValue(SeasonEp, out string description))
                        {
                            string links = description + "\n" + lnk;
                            linkDict[SeasonEp] = links;
                        }
                    }
                    else // otherwise create new entry for season / links
                    {
                        linkDict.Add(SeasonEp, lnk);
                    }

                }

                return linkDict;
            }

            public Dictionary<string, string> ParsedEpLinks_SizeQuality(ssTv show)
            {
                List<string> RGLinks = lst.Text_To_List(show.RapidGator);

                Dictionary<string, string> MainDict = new Dictionary<string, string>();

                List<string> FileSizes = new List<string>();

                foreach (string lnk in RGLinks)
                {
                    string rglink = ahk.StringSplit(lnk, "|", 0);
                    string size = "";
                    if (lnk.Contains("|")) { size = ahk.StringSplit(lnk, "|", 1); }

                    string sizeFixed = FileSizeFix(size);

                    // list of unique file sizes
                    if (!lst.InList(FileSizes, sizeFixed)) { FileSizes.Add(sizeFixed); }

                    string rgFileName = prs.LinkFileName(rglink);

                    string SeasonEp = ShowEp_FromFilePath(rgFileName);

                    RadTreeNode node = new RadTreeNode();
                    node.Text = rgFileName + " (" + sizeFixed + ")";
                    node.Tag = rglink; // lnk;

                    if (rgFileName.ToUpper().Contains("WEB"))
                    {
                        if (!rgFileName.ToUpper().Contains("720P") && !rgFileName.ToUpper().Contains("1080P"))
                        {
                            if (!MainDict.ContainsKey("WEB"))
                            {
                                MainDict.Add("WEB", rglink);
                            }
                            else
                            {
                                MainDict["WEB"] = MainDict["WEB"].ToString() + "\n" + rglink;
                            }
                        }
                    }
                    if (rgFileName.ToUpper().Contains("WEBRIP"))
                    {
                        if (!rgFileName.ToUpper().Contains("720P") && !rgFileName.ToUpper().Contains("1080P"))
                        {
                            if (!MainDict.ContainsKey("WEB"))
                            {
                                MainDict.Add("WEB", rglink);
                            }
                            else
                            {
                                MainDict["WEB"] = MainDict["WEB"].ToString() + "\n" + rglink;
                            }

                        }
                    }
                    if (rgFileName.ToUpper().Contains("HDTV"))
                    {
                        if (!rgFileName.ToUpper().Contains("720P") && !rgFileName.ToUpper().Contains("1080P"))
                        {
                            if (!MainDict.ContainsKey("HDTV"))
                            {
                                MainDict.Add("HDTV", rglink);
                            }
                            else
                            {
                                MainDict["HDTV"] = MainDict["HDTV"].ToString() + "\n" + rglink;
                            }
                        }
                    }
                    if (rgFileName.ToUpper().Contains("720P"))
                    {
                        if (!MainDict.ContainsKey("720P"))
                        {
                            MainDict.Add("720P", rglink);
                        }
                        else
                        {
                            MainDict["720P"] = MainDict["720P"].ToString() + "\n" + rglink;
                        }
                    }
                    if (rgFileName.ToUpper().Contains("1080P"))
                    {
                        if (!MainDict.ContainsKey("1080P"))
                        {
                            MainDict.Add("1080P", rglink);
                        }
                        else
                        {
                            MainDict["1080P"] = MainDict["1080P"].ToString() + "\n" + rglink;
                        }
                    }

                    //tree.AddNode(RadTree, node);
                }



                //========== SORTED BY FILE SIZE NODE ======================

                FileSizes = lst.FileSize_SortList(FileSizes, true);

                RadTreeNode ByFileSize = new RadTreeNode();
                ByFileSize.Text = "FileSize";
                ByFileSize.Tag = "FileSize";

                foreach (string FileSize in FileSizes)
                {
                    RadTreeNode sizeNode = new RadTreeNode();
                    sizeNode.Text = FileSize;
                    sizeNode.Tag = FileSize;
                    ByFileSize.Nodes.Add(sizeNode);

                    foreach (string lnk in RGLinks)
                    {
                        string rglink = ahk.StringSplit(lnk, "|", 0);
                        string size = "";
                        if (lnk.Contains("|")) { size = ahk.StringSplit(lnk, "|", 1); }
                        size = FileSizeFix(size);

                        if (size == FileSize)
                        {
                            if (!MainDict.ContainsKey("FileSize_" + FileSize))
                            {
                                MainDict.Add("FileSize_" + FileSize, rglink);
                            }
                            else
                            {
                                MainDict["FileSize_" + FileSize] = MainDict["FileSize_" + FileSize].ToString() + "\n" + rglink;
                            }
                        }
                    }
                }


                return MainDict;
            }

            /// <summary>
            /// Removes Decimal Values from File Size Names (ex: 3.04 MB = 3 MB)
            /// </summary>
            /// <param name="filesize"></param>
            /// <returns></returns>
            private string FileSizeFix(string filesize)
            {
                string size = "";
                if (filesize.ToUpper().Contains("KB"))
                {
                    size = filesize.ToUpper().Replace("KB", "").Trim();
                    if (size.Contains(".")) { size = ahk.StringSplit(size, ".", 0); }
                    size = size + " KB";
                }
                if (filesize.ToUpper().Contains("MB"))
                {
                    size = filesize.ToUpper().Replace("MB", "").Trim();
                    if (size.Contains(".")) { size = ahk.StringSplit(size, ".", 0); }
                    size = size + " MB";
                }
                if (filesize.ToUpper().Contains("GB"))
                {
                    size = filesize.ToUpper().Replace("GB", "").Trim();
                    if (size.Contains(".")) { size = ahk.StringSplit(size, ".", 0); }
                    size = size + " GB";
                }

                return size;
            }

            #endregion

            #region === GUI / Display ===

            /// <summary>
            /// Add Show Episode Object to RadTree, with Download Links as Tags
            /// </summary>
            /// <param name="ShowEpisode">ssTv Show Episode Object to Display in RadTree</param>
            /// <param name="RadTree">RadTree Control To Populate</param>
            /// <returns></returns>
            public RadTreeNode RadTree_AddShow(ssTv ShowEpisode, RadTreeView RadTree = null, bool ShowNameEpOneEntry = false)
            {
                RadTreeNode ShowNode = new RadTreeNode();
                ShowNode.Text = ShowEpisode.ShowName;
                ShowNode.Tag = ShowEpisode.ShowName;

                RadTreeNode SeasonEpNode = new RadTreeNode();
                SeasonEpNode.Text = ShowEpisode.SeasonEp;
                SeasonEpNode.Tag = ShowEpisode.SeasonEp;

                if (ShowEpisode.RapidGator_WEB != null && ShowEpisode.RapidGator_WEB != "")
                {
                    RadTreeNode WebEpNode = new RadTreeNode();
                    WebEpNode.Text = "WEB|" + ShowEpisode.PostTitle;
                    WebEpNode.Tag = ShowEpisode.RapidGator_WEB;

                    if (ShowNameEpOneEntry) { ShowNode.Text = ShowEpisode.SeasonEp + "WEB|" + ShowEpisode.PostTitle; ShowNode.Tag = ShowEpisode.RapidGator_WEB; }
                    else { SeasonEpNode.Nodes.Add(WebEpNode); }
                }
                if (ShowEpisode.RapidGator_HDTV != null && ShowEpisode.RapidGator_HDTV != "")
                {
                    RadTreeNode RapidGator_HDTV = new RadTreeNode();
                    RapidGator_HDTV.Text = "HDTV|" + ShowEpisode.PostTitle;
                    RapidGator_HDTV.Tag = ShowEpisode.RapidGator_HDTV;

                    if (ShowNameEpOneEntry) { ShowNode.Text = ShowEpisode.SeasonEp + "HDTV|" + ShowEpisode.PostTitle; ShowNode.Tag = ShowEpisode.RapidGator_HDTV; }
                    else { SeasonEpNode.Nodes.Add(RapidGator_HDTV); }
                }
                if (ShowEpisode.RapidGator_720P != null && ShowEpisode.RapidGator_720P != "")
                {
                    RadTreeNode RapidGator_720P = new RadTreeNode();
                    RapidGator_720P.Text = "720P|" + ShowEpisode.PostTitle;
                    RapidGator_720P.Tag = ShowEpisode.RapidGator_720P;

                    if (ShowNameEpOneEntry) { ShowNode.Text = ShowEpisode.SeasonEp + "720P|" + ShowEpisode.PostTitle; ShowNode.Tag = ShowEpisode.RapidGator_720P; }
                    else { SeasonEpNode.Nodes.Add(RapidGator_720P); }
                }
                if (ShowEpisode.RapidGator_1080P != null && ShowEpisode.RapidGator_1080P != "")
                {
                    RadTreeNode RapidGator_1080P = new RadTreeNode();
                    RapidGator_1080P.Text = "1080P|" + ShowEpisode.PostTitle;
                    RapidGator_1080P.Tag = ShowEpisode.RapidGator_1080P;

                    if (ShowNameEpOneEntry) { ShowNode.Text = ShowEpisode.SeasonEp + "1080P|" + ShowEpisode.PostTitle; ShowNode.Tag = ShowEpisode.RapidGator_1080P; }
                    else { SeasonEpNode.Nodes.Add(RapidGator_1080P); }
                }

                if (!ShowNameEpOneEntry) { ShowNode.Nodes.Add(SeasonEpNode); }

                if (RadTree != null) { tree.AddNode(RadTree, ShowNode); }

                return ShowNode;
            }


            #endregion

            #region === Parse Show Info ===

            /// <summary>
            /// Extract Show Ep Number/Season Number from FileName or URL
            /// </summary>
            /// <param name="FileName_Or_URL">FileName or URL</param>
            /// <returns></returns>
            public string ShowEp_FromFilePath(string FileName_Or_URL)
            {
                string sName = "";

                // parse file name from url if passed in
                List<string> Parts = ahk.StringSplit_List(FileName_Or_URL, "/");
                foreach (string Part in Parts) { sName = Part; }

                string SeasonEp = prs.SeasonEpNums("c:\\" + sName + ".avi", false);

                sName = sName.Replace("-", " ");

                if (SeasonEp == "" || SeasonEp == "S20E18")
                {
                    string badChars = sName.Encode();

                    sName = sName.Replace("×", "x");

                    SeasonEp = prs.SeasonEpNums(sName, false);

                    if (SeasonEp == "" || SeasonEp == "S20E18")
                    {
                        SeasonEp = prs.ExtractDate(sName); // extract date (2017 01 13)
                    }

                    if (SeasonEp == "" || SeasonEp == "S20E18")
                    {
                        SeasonEp = prs.ExtractSeason(sName);  // ex: show s01 title
                    }

                    if (SeasonEp == "" || SeasonEp == "S20E18")
                    {
                        SeasonEp = prs.ExtractYear(sName);  // extract year
                    }
                }

                if (SeasonEp != "")
                {
                    if (SeasonEp.Length > 6)
                    {
                        sName = sName.Replace(SeasonEp.ToUpper(), "|");
                        sName = sName.Replace(SeasonEp.ToLower(), "|");

                        sName = ahk.StringSplit(sName, "|", 0);
                        SeasonEp = SeasonEp.Replace(" ", ".");
                    }
                    else
                    {
                        sName = sName.Replace(SeasonEp.ToUpper(), "|");
                        sName = sName.Replace(SeasonEp.ToLower(), "|");
                        sName = ahk.StringSplit(sName, "|", 0);
                    }
                }

                string ShowName = sName.Trim().ToTitleCase();

                return SeasonEp;
            }

            /// <summary>
            /// Extract Show Name from FileName or URL
            /// </summary>
            /// <param name="FileName_Or_URL">FileName or URL</param>
            /// <returns></returns>
            public string ShowName_FromFilePath(string FileName_Or_URL)
            {
                string sName = "";

                // parse file name from url if passed in
                List<string> Parts = ahk.StringSplit_List(FileName_Or_URL, "/");
                foreach (string Part in Parts) { sName = Part; }

                string SeasonEp = prs.SeasonEpNums("c:\\" + sName + ".avi", false);

                sName = sName.Replace("-", " ");

                if (SeasonEp == "" || SeasonEp == "S20E18")
                {
                    string badChars = sName.Encode();

                    sName = sName.Replace("×", "x");

                    SeasonEp = prs.SeasonEpNums(sName, false);

                    if (SeasonEp == "" || SeasonEp == "S20E18")
                    {
                        SeasonEp = prs.ExtractDate(sName); // extract date (2017 01 13)
                    }

                    if (SeasonEp == "" || SeasonEp == "S20E18")
                    {
                        SeasonEp = prs.ExtractSeason(sName);  // ex: show s01 title
                    }

                    if (SeasonEp == "" || SeasonEp == "S20E18")
                    {
                        SeasonEp = prs.ExtractYear(sName);  // extract year
                    }
                }

                if (SeasonEp != "")
                {
                    if (SeasonEp.Length > 6)
                    {
                        sName = sName.Replace(SeasonEp.ToUpper(), "|");
                        sName = sName.Replace(SeasonEp.ToLower(), "|");

                        sName = ahk.StringSplit(sName, "|", 0);
                        SeasonEp = SeasonEp.Replace(" ", ".");
                    }
                    else
                    {
                        sName = sName.Replace(SeasonEp.ToUpper(), "|");
                        sName = sName.Replace(SeasonEp.ToLower(), "|");
                        sName = ahk.StringSplit(sName, "|", 0);
                    }
                }

                string ShowName = sName.Trim().ToTitleCase();


                return ShowName;
            }


            #endregion

            #region === SubIndex FUNCTIONS ===

            /// <summary>
            /// Take list of shows in SceneSource TV Posts and write missing ShowNames to SceneSource Show Index
            /// </summary>
            public void UpdateIndex_SceneSourceTV_Shows()
            {
                List<string> shows = lst.SQL_To_List(SubIndex_Conn(), "select distinct [ShowName] FROM " + ssTv_TableName() + " order by [ShowName]");

                List<string> indexShows = lst.SQL_To_List(SubIndex_Conn(), "select distinct [ShowName] FROM " + SubIndex_TableName() + " order by [ShowName]");

                int added = 0;
                foreach (string show in shows)
                {
                    if (!lst.InList(indexShows, show)) { added++; sql.Write_SQL(SubIndex_Conn(), "Insert Into " + SubIndex_TableName() + " (ShowName, Subscribed) VALUES ('" + show + "', '0')"); }
                }

                ahk.MsgBox("Added " + added + " Shows to Index");
            }

            #region ===== SubIndex Object =====

            public struct SubIndex
            {
                public string ShowName { get; set; }
                public string Subscribed { get; set; }
            }
            //  Fix illegal characters before Sql/Sqlite Db Inserts
            public SubIndex SubIndex_FixChars(SubIndex ToFix)
            {
                SubIndex Fixed = new SubIndex();

                Fixed.ShowName = ToFix.ShowName.Replace("'", "''");
                Fixed.Subscribed = ToFix.Subscribed.Replace("'", "''");

                return Fixed;
            }

            // Compare two objects to see if they have identical values
            public bool SubIndex_Changed(SubIndex OldVal, SubIndex NewVal)
            {
                SubIndex diff = new SubIndex();
                List<string> diffList = new List<string>();
                bool different = false;
                if (OldVal.ShowName == null) { OldVal.ShowName = ""; }
                if (NewVal.ShowName == null) { NewVal.ShowName = ""; }
                if (OldVal.ShowName != NewVal.ShowName) { different = true; }
                if (OldVal.Subscribed == null) { OldVal.Subscribed = ""; }
                if (NewVal.Subscribed == null) { NewVal.Subscribed = ""; }
                if (OldVal.Subscribed != NewVal.Subscribed) { different = true; }
                return different;
            }

            // Returns object containing the new values different from the old values in object comparison
            public SubIndex SubIndex_Diff(SubIndex OldVal, SubIndex NewVal)
            {
                SubIndex diff = new SubIndex();
                if (OldVal.ShowName != NewVal.ShowName) { diff.ShowName = NewVal.ShowName; }
                if (OldVal.Subscribed != NewVal.Subscribed) { diff.Subscribed = NewVal.Subscribed; }
                return diff;
            }

            // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
            public List<string> SubIndex_DiffList(SubIndex OldVal, SubIndex NewVal)
            {
                List<string> diffList = new List<string>();
                if (OldVal.ShowName != NewVal.ShowName) { diffList.Add("Changed ShowName Value From " + OldVal.ShowName + " To " + NewVal.ShowName); }
                if (OldVal.Subscribed != NewVal.Subscribed) { diffList.Add("Changed Subscribed Value From " + OldVal.Subscribed + " To " + NewVal.Subscribed); }
                return diffList;
            }

            // Generate XML String From Object
            public string SubIndex_ToXML(SubIndex obj)
            {
                string XMLString = "";
                XMLString = XMLString + "<ShowName>" + obj.ShowName + "</ShowName>";
                XMLString = XMLString + "<Subscribed>" + obj.Subscribed + "</Subscribed>";
                return XMLString;
            }

            // Populate Object from XML Tag String
            public SubIndex SubIndex_FromXML(string XMLString)
            {
                SubIndex obj = new SubIndex();
                obj.ShowName = prs.XML_Text(XMLString, "<ShowName>");
                obj.Subscribed = prs.XML_Text(XMLString, "<Subscribed>");
                return obj;
            }


            #endregion
            #region ===== SubIndex SQLite : Return =====

            public SubIndex Return_Object_From_SceneSourceTVIndex(string WhereClause = "[ShowName] = ''", string DbFile = "")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\SubIndex.sqlite"; }
                string SelectLine = "Select [ShowName], [Subscribed] From [SceneSourceTVIndex] ";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
                SubIndex returnObject = new SubIndex();
                int i = 0;
                string Value = "";
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.Subscribed = ret["Subscribed"].ToString();
                    }
                }

                return returnObject;
            }

            public List<SubIndex> Return_SubIndex_List(string WhereClause = "", string DbFile = "", string TableName = "[SceneSourceTVIndex]")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\SubIndex.sqlite"; }
                string SelectLine = "Select * From " + TableName;

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

                List<SubIndex> ReturnList = new List<SubIndex>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        SubIndex returnObject = new SubIndex();

                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.Subscribed = ret["Subscribed"].ToString();

                        ReturnList.Add(returnObject);
                    }
                }

                return ReturnList;
            }

            public DataTable Return_DataTable_From_SceneSourceTVIndex(string DbFile)
            {
                string SelectLine = "Select [ShowName], [Subscribed] From [SceneSourceTVIndex]";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                return ReturnTable;
            }


            #endregion
            #region ===== SubIndex SQLite : Update Insert =====

            public bool SubIndex_Insert(SubIndex inObject, string DbFile = "")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\SubIndex.sqlite"; }
                string InsertLine = "Insert Into [SceneSourceTVIndex] (ShowName, Subscribed) values ('" + inObject.ShowName + "', '" + inObject.Subscribed + "')";
                bool Inserted = sqlite.Execute(DbFile, InsertLine);
                if (!Inserted) { ahk.MsgBox("Inserted Into [SceneSourceTVIndex] = " + Inserted.ToString()); }
                return Inserted;
            }

            public bool SubIndex_Update(SubIndex inObject, string DbFile = "")
            {
                //string UpdateLine = "Update [SceneSourceTVIndex] set ShowName = '" + inObject.ShowName + "', Subscribed = '" + inObject.Subscribed + "' WHERE [Item] = 'Value' ";
                string UpdateLine = "Update [SceneSourceTVIndex] set ";


                if (inObject.ShowName != null) { UpdateLine = UpdateLine + "[ShowName] = '" + inObject.ShowName + "',"; }
                if (inObject.Subscribed != null) { UpdateLine = UpdateLine + "[Subscribed] = '" + inObject.Subscribed + "',"; }

                UpdateLine = ahk.TrimLast(UpdateLine, 1);
                UpdateLine = UpdateLine + " WHERE [ShowName] = ' '"; // DEFINE CONDITION HERE !!!

                bool Updated = sqlite.Execute(DbFile, UpdateLine);
                return Updated;
            }

            public bool SubIndex_UpdateInsert(SubIndex obj, string DbFile = "")
            {

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\SubIndex.sqlite"; }
                if (!File.Exists(DbFile)) { Create_Table_SceneSourceTVIndex(DbFile); }

                bool Updated = SubIndex_Update(obj, DbFile);  // try to update record first
                if (!Updated) { Updated = SubIndex_Insert(obj, DbFile); }  // if unable to update, insert new record
                return Updated;
            }


            #endregion
            #region ===== SubIndex SQL Functions =====

            // Return SubIndex SQL Connection String
            public SqlConnection SubIndex_Conn()
            {
                // populate sql connection
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMLucidMedia"].ConnectionString);
                // SqlConnection Conn = new SqlConnection("Server=188.168.188.88;DataBase=LucidMedia;Uid=lucidm;Pwd=pass");
                return conn;
            }

            public bool Create_Table_SceneSourceTVIndex(string DbFile)
            {
                string CreateLine = "Create Table [SceneSourceTVIndex] (ShowName VARCHAR, Subscribed VARCHAR)";
                bool TableCreated = sqlite.Table_Exists(DbFile, "SceneSourceTVIndex");
                if (!TableCreated) { TableCreated = sqlite.Table_New(DbFile, "SceneSourceTVIndex", "Create Table [SceneSourceTVIndex] (ShowName VARCHAR, Subscribed VARCHAR", false); }


                if (!TableCreated) { ahk.MsgBox("[SceneSourceTVIndex] Created = " + TableCreated.ToString()); }
                return TableCreated;
            }

            // Return SubIndex TableName (Full Path)
            public string SubIndex_TableName()
            {
                // populate to return full sql table name
                return "[LucidMedia].[dbo].[SceneSourceTVIndex]";
            }

            // Generate SQL Table
            public bool SubIndex_CreateSQLTable()
            {
                SqlConnection Conn = SubIndex_Conn();
                string CreateTableLine = "CREATE TABLE " + SubIndex_TableName() + "(";
                CreateTableLine = CreateTableLine + "[ShowName] [int] IDENTITY(1,1) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Subscribed] [varchar](max) NOT NULL";
                CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                return false;
            }



            public bool SubIndex_InsertSQL(SubIndex obj)
            {
                SqlConnection Con = SubIndex_Conn();
                string SQLLine = "Insert Into " + SubIndex_TableName() + " (ShowName, Subscribed) VALUES (@ShowName, @Subscribed)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.ShowName == null) { obj.ShowName = ""; }
                if (obj.Subscribed == null) { obj.Subscribed = ""; }
                cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                cmd2.Parameters.AddWithValue(@"Subscribed", obj.Subscribed.ToString());
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        SubIndex_InsertSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        SubIndex_InsertSQL(obj);
                    }
                    else
                    {
                        ahk.MsgBox(ex.ToString());
                        return false;
                    }
                }
                Con.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool SubIndex_UpdateSQL(SubIndex obj)
            {
                SqlConnection Conn = SubIndex_Conn();
                string SQLLine = "Update " + SubIndex_TableName() + " SET Subscribed = @Subscribed WHERE ShowName = @ShowName";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.ShowName == null) { obj.ShowName = ""; }
                if (obj.Subscribed == null) { obj.Subscribed = ""; }
                cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                cmd2.Parameters.AddWithValue(@"Subscribed", obj.Subscribed.ToString());
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        SubIndex_UpdateSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        SubIndex_UpdateSQL(obj);
                    }
                    else
                    {
                        ahk.MsgBox(ex.ToString());
                        return false;
                    }
                }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool SubIndex_UpdateInsert(SubIndex obj)
            {
                SqlConnection Conn = SubIndex_Conn();
                bool Updated = SubIndex_UpdateSQL(obj);  // try to update record first
                if (!Updated) { Updated = SubIndex_InsertSQL(obj); }  // if unable to update, insert new record
                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool SubIndex_UpdateIfPopulated(SubIndex obj, string ID = "")
            {
                SqlConnection Conn = SubIndex_Conn();
                string SQLcmd = "Update " + SubIndex_TableName() + " SET ";
                if (obj.ShowName != null) { SQLcmd = SQLcmd + " ShowName = @ShowName,"; }
                if (obj.Subscribed != null) { SQLcmd = SQLcmd + " Subscribed = @Subscribed,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ID = @ID";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.ShowName != null) { cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName); }
                if (obj.Subscribed != null) { cmd2.Parameters.AddWithValue(@"Subscribed", obj.Subscribed); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public SubIndex SubIndex_ReturnSQL(string ID = "")
            {
                SqlConnection Conn = SubIndex_Conn();
                string SelectLine = "Select [ShowName],[Subscribed] From " + SubIndex_TableName() + " WHERE ID = '" + ID + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                SubIndex returnObject = new SubIndex();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.Subscribed = ret["Subscribed"].ToString();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public List<SubIndex> SubIndex_ReturnSQLList(bool SubscriptionsOnly = false)
            {
                string Command = "Select * From " + SubIndex_TableName() + " order by ShowName";

                if (SubscriptionsOnly) { Command = "Select * From " + SubIndex_TableName() + " where [Subscribed] = '1' order by ShowName"; }

                SqlConnection Conn = SubIndex_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<SubIndex> ReturnList = new List<SubIndex>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        SubIndex returnObject = new SubIndex();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.Subscribed = ret["Subscribed"].ToString();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }

            /// <summary>
            /// Used for Testing, Returns Post Titles from DB
            /// </summary>
            /// <returns></returns>
            public List<string> PostTitles()
            {
                string Command = "Select [PostTitle] From [LucidMedia].[dbo].[SceneSourceTV] where PostURL like '%.ru/%' and PostTitle like '%WWE%'";

                Command = "select [PostTitle] FROM [LucidMedia].[dbo].[SceneSourceTV] where PostURL like '%.scnsrc.me/%'";

                Command = "select [PostTitle] FROM [LucidMedia].[dbo].[SceneSourceTV] where PostURL like '%.ru/%'";

                Command = "select [PostTitle] FROM [LucidMedia].[dbo].[SceneSourceTV] where PostTitle like '%Daily Show%'";

                Command = "select [PostTitle] FROM [LucidMedia].[dbo].[SceneSourceTV] where PostTitle like '%Daily Show%' and PostURL like '%.ru/%'";

                SqlConnection Conn = SubIndex_Conn();
                return lst.SQL_To_List(Conn, Command);
            }

            public bool SubIndex_SQL_to_SQLite(string SqliteDBPath = @"\Db\SubIndex.sqlite")
            {
                string SaveFile = SqliteDBPath;
                if (SqliteDBPath == @"\Db\SubIndex.sqlite")
                {
                    ahk.FileCreateDir(ahk.AppDir() + @"\Db");
                    SaveFile = ahk.AppDir() + @"\Db\SubIndex.sqlite";
                }

                sb.StatusBar("Copying SQL Db to " + SaveFile + "...");
                sqlite.SQLTable_To_NewSQLiteTable(SubIndex_Conn(), "SceneSourceTVIndex", "SceneSourceTVIndex", SaveFile, "", false, false, false);
                sb.StatusBar("FINISHED Copying SQL Db to " + SaveFile);

                if (File.Exists(SaveFile)) { return true; } else { return false; }
            }


            #endregion

            #endregion

        }



    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Data;
using System.Globalization;
using System.Data.SQLite;
using System.Threading;
using static sharpAHK_Dev._Sites.Imdb;
using Telerik.WinControls.UI;

namespace sharpAHK_Dev
{
    public partial class _Sites
    {

        public class EpGuides
        {
            #region === Startup ===

            _Database.goDaddy goDad = new _Database.goDaddy();
            sharpAHK._AHK ahk = new sharpAHK._AHK();
            _Database.SQL sql = new _Database.SQL();
            _Database.SQLite sqlite = new _Database.SQLite();
            _GridControl grid = new _GridControl();
            _Lists lst = new _Lists();
            _Dict dict = new _Dict();
            _Images img = new _Images();
            _WinForms frm = new _WinForms();
            _TreeViewControl tv = new _TreeViewControl();
            _TabControl tab = new _TabControl();
            _MenuControl menu = new _MenuControl();
            _ScintillaControl sci = new _ScintillaControl();
            _Apps.Mpc mpc = new _Apps.Mpc();
            _Apps.Chrome cr = new _Apps.Chrome();
            _Code code = new _Code();
            _Database.Tags tags = new _Database.Tags();
            //_Database.dir2Db dir2 = new _Database.dir2Db();
            _Parse prs = new _Parse();
            _StatusBar sb = new _StatusBar();
            _Web web = new _Web();
            _TelerikLib tel = new _TelerikLib();
            _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();
            _Database.dir2Db dirDb = new _Database.dir2Db();

            SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMLucidMedia"].ConnectionString);
            //SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["epGuideDb"].ConnectionString);
            //SqlConnection Conn = new SqlConnection("Server=184.168.194.77;DataBase=LucidMedia;Uid=lucidmethod;Pwd=dUy76k^7");

            public string userName = "LucidMethod";

            public string LITMEpisodeTable = "[lucidmedia].[dbo].[LITM_Episodes]";

            public string EpGuideShowIndexTable = "[lucidmedia].[dbo].[EpGuides_ShowIndex]";

            public string LITMShowIndexTable = "[lucidmedia].[dbo].[LITM_ShowIndex]";

            public void LITMTables(string Username = "lucidmethod")
            {
                LITMEpisodeTable = "[lucidmedia].[" + Username + "].[LITM_Episodes]";

                EpGuideShowIndexTable = "[lucidmedia].[" + Username + "].[EpGuides_ShowIndex]";

                LITMShowIndexTable = "[lucidmedia].[" + Username + "].[LITM_ShowIndex]";
            }


            ProgressBar progressbar = new ProgressBar();


            public void StatusBar(string Text, int section = 1)
            {
                //EpGuide foM = new EpGuide();
                //foM.StatusBar(Text, Pos);

                if (section == 1)
                {
                    try
                    {
                        MethodInvoker inv = delegate { if (epGuideGui.statusPanel != null) { epGuideGui.statusPanel.Text = Text; } };
                        epGuideGui.fromForm.Invoke(inv);
                    }
                    catch
                    {
                        if (epGuideGui.statusPanel != null) { epGuideGui.statusPanel.Text = Text; }
                    }
                }

                if (section == 2)
                {
                    try
                    {
                        MethodInvoker inv = delegate { if (epGuideGui.statusPanel != null) { epGuideGui.datetimePanel.Text = Text; } };
                        epGuideGui.fromForm.Invoke(inv);
                    }
                    catch
                    {
                        if (epGuideGui.statusPanel != null) { epGuideGui.datetimePanel.Text = Text; }
                    }
                }

            }


            #endregion

            #region === EpGuides Database ===

            // download entire EpGuide Index from CSV Parsed db - save each html page to dir

            public void Download_EpGuides_HTML_FullIndex()
            {
                string dbFile = ahk.AppDir() + "\\Db\\EpGuidesComIndex.sqlite";

                string htmlDir = ahk.AppDir() + "\\Db\\EpGuidesHTML";
                ahk.FileCreateDir(htmlDir);

                string cmd = "select [EpGuideURL] from [EpGuidesComIndex]";

                List<string> lines = lst.SQLite_To_List(dbFile, cmd);

                int i = 0;
                foreach (string line in lines)
                {
                    string showID = line.Replace("http://epguides.com/", "");
                    showID = showID.Replace("/", "");

                    string filePath = htmlDir + "\\" + showID + ".txt";

                    ahk.FileAppend(web.DownloadHTML(line, true), filePath);
                    i++;

                    sb.StatusBar("Downloaded EpGuides HTML " + i + "/" + lines.Count);
                }

                //http://epguides.com/AaaghItstheMrHellShow/
            }

            private void downloadEpGuideHTMLFullIndexToolStripMenuItem_Click(object sender, EventArgs e)
            {
                Download_EpGuides_HTML_FullIndex();
            }


            public void Update_IMDbLinks_EpGuideDb()
            {
                List<string> URLs = lst.SQL_To_List(Conn, "select [EpGuideURL] FROM [LucidMedia].[dbo].[ShowIndex] where IMDbURL = '' and [EpGuideURL] != ''");

                foreach (string url in URLs)
                {
                    string imdbLink = IMDbLink_FromEpGuideHTML(url);

                    if (imdbLink != "")
                    {
                        string cmd = "Update [LucidMedia].[dbo].[ShowIndex] set [IMDbURL] = '" + imdbLink + "' where [EpGuideURL] = '" + url + "'";
                        bool updated = sql.Write_SQL(Conn, cmd);

                    }
                }



            }

            // returns imdb link from html, if url provided downloading link, if skipped and html provided, parses out link
            public string IMDbLink_FromEpGuideHTML(string url = "", string html = "")
            {
                string imdbLink = "";

                if (url != "")
                {
                    html = web.DownloadHTML(url);
                }

                List<string> lines = lst.Text_To_List(html, true, true, false);
                foreach (string line in lines)
                {
                    if (line.Contains("imdb.com"))
                    {
                        List<string> segs = ahk.StringSplit_List(line, "\"");
                        foreach (string seg in segs)
                        {
                            if (seg.Contains("imdb.com"))
                            {
                                imdbLink = seg;
                                //ahk.MsgBox(seg);
                                break;
                            }
                        }
                    }
                }

                return imdbLink;
            }


            // downloads .csv file from epguide side, parses, update database
            public void updateEpGuideSiteIndex_FromCSV(ProgressBar progressBar1)
            {
                bool updateCSVIndex = false;

                //ProgressBar progressBar1 = new ProgressBar();

                if (updateCSVIndex)
                {
                    sb.StatusBar("Starting EpGuides.Com CSV Parse...");
                    Populate_EpGuidesComIndex_FromCSV(progressBar1);
                    sb.StatusBar("FINISHED EpGuides.Com CSV Parse.");
                }


                Download_EpGuidesIndex_Content(progressBar1);

            }

            public void Download_EpGuidesIndex_Content(ProgressBar pb)
            {
                List<string> pages = lst.SQL_To_List(Conn, "select EpGuideURL FROM " + EGCi_TableName() + " order by ShowTitle");

                pb.Maximum = pages.Count();

                string ImgSaveDir = ahk.AppDir() + "\\EpGuides\\Images";
                ahk.FileCreateDir(ImgSaveDir);

                string HTMLSaveDir = ahk.AppDir() + "\\EpGuides\\HTML";
                ahk.FileCreateDir(HTMLSaveDir);

                int i = 1;
                foreach (string page in pages)
                {
                    pb.Value = i;
                    string EpID = page.Replace("http://epguides.com/", "");
                    EpID = EpID.Replace("/", "");

                    // download show image
                    web.DownloadFile(page + "cast.jpg", ImgSaveDir + "\\" + EpID + ".jpg");

                    // download show html
                    string html = web.DownloadHTML(page);
                    ahk.FileAppend(html, HTMLSaveDir + "\\" + EpID + ".txt");


                    i++;
                }

                ahk.OpenDir(ImgSaveDir);

            }

            /// Download EpGuides.com Show Index CSV 
            /// ResetCache - Option to Force New Download of CSV Index
            public string EpGuides_IndexCSV(bool ResetCache = false)
            {
                string localFile = ahk.AppDir() + "\\EpGuidesCom_ShowIndexWeb.txt";
                string IndexURL = "http://epguides.com/common/allshows.txt";

                if (ResetCache) { ahk.FileDelete(localFile); }

                if (!File.Exists(localFile)) { web.DownloadFile(IndexURL, localFile, false); }

                string text = ahk.FileRead(localFile);

                return text;
            }

            public void Populate_EpGuidesComIndex_FromCSV(ProgressBar pb, bool DLIndexFromSite = true)
            {
                string localFile = @"D:\_Code\AHK_OfficeToolkit\ScriptLets\EpGuidesImportTest.txt";

                if (DLIndexFromSite)  // option to download latest csv from epguide site
                {
                    localFile = ahk.AppDir() + "\\EpGuidesCom_ShowIndexWeb.txt";
                    string IndexURL = "http://epguides.com/common/allshows.txt";

                    ahk.FileDelete(localFile);
                    web.DownloadFile(IndexURL, localFile, false);
                }

                if (File.Exists(localFile))
                {
                    List<string> lines = lst.TextFile_To_List(localFile);

                    pb.Maximum = lines.Count();

                    int i = 1;
                    foreach (string line in lines)
                    {
                        sb.StatusBar("EpGuides.Com Show Index | " + i + "/" + lines.Count());
                        //List<string> segs = ahk.CSV_ToList(line);
                        List<string> segs = ParseCSVwithCommas(line);

                        pb.Value = i;

                        //List<string> segs = ahk.StringSplit_List(line, "\"");

                        EGCi show = new EGCi();

                        int j = 1;
                        foreach (string seg in segs)
                        {
                            //title,directory,tvrage,TVmaze,start date,end date,number of episodes,run time,network,country
                            if (j == 1) { show.ShowTitle = seg; }
                            if (j == 2)
                            {
                                string see = seg.Replace("\"", "");
                                see = see.Trim();
                                show.EpGuideURL = "http://epguides.com/" + see + "/";
                            }
                            if (j == 3) { show.TVRage = seg; }
                            if (j == 4) { show.TVMaze = seg; }
                            if (j == 5) { show.StartDate = seg; }
                            if (j == 6) { show.EndDate = seg; }
                            if (j == 7) { show.EpCount = seg.Replace(" eps", ""); }
                            if (j == 8) { show.RunTime = seg; }
                            if (j == 9) { show.Network = seg; }
                            if (j == 10) { show.Country = seg; }

                            //ahk.MsgBox("Seg " + j + " | " + seg);
                            j++;
                        }

                        if (i != 1)  // write show entry to db
                        {
                            bool Added = EGCi_UpdateInsert(show);
                        }

                        i++;
                    }
                }

            }





            #region === EGCi Object ===

            public struct EGCi
            {
                public string ID { get; set; }
                public string ShowTitle { get; set; }
                public string EpGuideURL { get; set; }
                public string TVRage { get; set; }
                public string TVMaze { get; set; }
                public string IMDb { get; set; }
                public string StartDate { get; set; }
                public string EndDate { get; set; }
                public string EpCount { get; set; }
                public string RunTime { get; set; }
                public string Network { get; set; }
                public string Country { get; set; }
                public string HTML { get; set; }
                public string Links { get; set; }
                public string Tags { get; set; }
            }
            public EGCi Return_EGCi(string ID = "", string ShowTitle = "", string EpGuideURL = "", string TVRage = "", string TVMaze = "", string IMDb = "", string StartDate = "", string EndDate = "", string EpCount = "", string RunTime = "", string Network = "", string Country = "", string HTML = "", string Links = "", string Tags = "")
            {
                EGCi obj = new EGCi();
                obj.ID = ID;
                obj.ShowTitle = ShowTitle;
                obj.EpGuideURL = EpGuideURL;
                obj.TVRage = TVRage;
                obj.TVMaze = TVMaze;
                obj.IMDb = IMDb;
                obj.StartDate = StartDate;
                obj.EndDate = EndDate;
                obj.EpCount = EpCount;
                obj.RunTime = RunTime;
                obj.Network = Network;
                obj.Country = Country;
                obj.HTML = HTML;
                obj.Links = Links;
                obj.Tags = Tags;

                return obj;
            }

            //  Fix illegal characters before Sql/Sqlite Db Inserts
            public EGCi EGCi_FixChars(EGCi ToFix)
            {
                EGCi Fixed = new EGCi();

                Fixed.ID = ToFix.ID.Replace("'", "''");
                Fixed.ShowTitle = ToFix.ShowTitle.Replace("'", "''");
                Fixed.EpGuideURL = ToFix.EpGuideURL.Replace("'", "''");
                Fixed.TVRage = ToFix.TVRage.Replace("'", "''");
                Fixed.TVMaze = ToFix.TVMaze.Replace("'", "''");
                Fixed.IMDb = ToFix.IMDb.Replace("'", "''");
                Fixed.StartDate = ToFix.StartDate.Replace("'", "''");
                Fixed.EndDate = ToFix.EndDate.Replace("'", "''");
                Fixed.EpCount = ToFix.EpCount.Replace("'", "''");
                Fixed.RunTime = ToFix.RunTime.Replace("'", "''");
                Fixed.Network = ToFix.Network.Replace("'", "''");
                Fixed.Country = ToFix.Country.Replace("'", "''");
                Fixed.HTML = ToFix.HTML.Replace("'", "''");
                Fixed.Links = ToFix.Links.Replace("'", "''");
                Fixed.Tags = ToFix.Tags.Replace("'", "''");

                return Fixed;
            }

            // Compare two objects to see if they have identical values
            public bool EGCi_Changed(EGCi OldVal, EGCi NewVal)
            {
                EGCi diff = new EGCi();
                List<string> diffList = new List<string>();
                bool different = false;
                if (OldVal.ID == null) { OldVal.ID = ""; }
                if (NewVal.ID == null) { NewVal.ID = ""; }
                if (OldVal.ID != NewVal.ID) { different = true; }
                if (OldVal.ShowTitle == null) { OldVal.ShowTitle = ""; }
                if (NewVal.ShowTitle == null) { NewVal.ShowTitle = ""; }
                if (OldVal.ShowTitle != NewVal.ShowTitle) { different = true; }
                if (OldVal.EpGuideURL == null) { OldVal.EpGuideURL = ""; }
                if (NewVal.EpGuideURL == null) { NewVal.EpGuideURL = ""; }
                if (OldVal.EpGuideURL != NewVal.EpGuideURL) { different = true; }
                if (OldVal.TVRage == null) { OldVal.TVRage = ""; }
                if (NewVal.TVRage == null) { NewVal.TVRage = ""; }
                if (OldVal.TVRage != NewVal.TVRage) { different = true; }
                if (OldVal.TVMaze == null) { OldVal.TVMaze = ""; }
                if (NewVal.TVMaze == null) { NewVal.TVMaze = ""; }
                if (OldVal.TVMaze != NewVal.TVMaze) { different = true; }
                if (OldVal.IMDb == null) { OldVal.IMDb = ""; }
                if (NewVal.IMDb == null) { NewVal.IMDb = ""; }
                if (OldVal.IMDb != NewVal.IMDb) { different = true; }
                if (OldVal.StartDate == null) { OldVal.StartDate = ""; }
                if (NewVal.StartDate == null) { NewVal.StartDate = ""; }
                if (OldVal.StartDate != NewVal.StartDate) { different = true; }
                if (OldVal.EndDate == null) { OldVal.EndDate = ""; }
                if (NewVal.EndDate == null) { NewVal.EndDate = ""; }
                if (OldVal.EndDate != NewVal.EndDate) { different = true; }
                if (OldVal.EpCount == null) { OldVal.EpCount = ""; }
                if (NewVal.EpCount == null) { NewVal.EpCount = ""; }
                if (OldVal.EpCount != NewVal.EpCount) { different = true; }
                if (OldVal.RunTime == null) { OldVal.RunTime = ""; }
                if (NewVal.RunTime == null) { NewVal.RunTime = ""; }
                if (OldVal.RunTime != NewVal.RunTime) { different = true; }
                if (OldVal.Network == null) { OldVal.Network = ""; }
                if (NewVal.Network == null) { NewVal.Network = ""; }
                if (OldVal.Network != NewVal.Network) { different = true; }
                if (OldVal.Country == null) { OldVal.Country = ""; }
                if (NewVal.Country == null) { NewVal.Country = ""; }
                if (OldVal.Country != NewVal.Country) { different = true; }
                if (OldVal.HTML == null) { OldVal.HTML = ""; }
                if (NewVal.HTML == null) { NewVal.HTML = ""; }
                if (OldVal.HTML != NewVal.HTML) { different = true; }
                if (OldVal.Links == null) { OldVal.Links = ""; }
                if (NewVal.Links == null) { NewVal.Links = ""; }
                if (OldVal.Links != NewVal.Links) { different = true; }
                if (OldVal.Tags == null) { OldVal.Tags = ""; }
                if (NewVal.Tags == null) { NewVal.Tags = ""; }
                if (OldVal.Tags != NewVal.Tags) { different = true; }
                return different;
            }

            // Returns object containing the new values different from the old values in object comparison
            public EGCi EGCi_Diff(EGCi OldVal, EGCi NewVal)
            {
                EGCi diff = new EGCi();
                if (OldVal.ID != NewVal.ID) { diff.ID = NewVal.ID; }
                if (OldVal.ShowTitle != NewVal.ShowTitle) { diff.ShowTitle = NewVal.ShowTitle; }
                if (OldVal.EpGuideURL != NewVal.EpGuideURL) { diff.EpGuideURL = NewVal.EpGuideURL; }
                if (OldVal.TVRage != NewVal.TVRage) { diff.TVRage = NewVal.TVRage; }
                if (OldVal.TVMaze != NewVal.TVMaze) { diff.TVMaze = NewVal.TVMaze; }
                if (OldVal.IMDb != NewVal.IMDb) { diff.IMDb = NewVal.IMDb; }
                if (OldVal.StartDate != NewVal.StartDate) { diff.StartDate = NewVal.StartDate; }
                if (OldVal.EndDate != NewVal.EndDate) { diff.EndDate = NewVal.EndDate; }
                if (OldVal.EpCount != NewVal.EpCount) { diff.EpCount = NewVal.EpCount; }
                if (OldVal.RunTime != NewVal.RunTime) { diff.RunTime = NewVal.RunTime; }
                if (OldVal.Network != NewVal.Network) { diff.Network = NewVal.Network; }
                if (OldVal.Country != NewVal.Country) { diff.Country = NewVal.Country; }
                if (OldVal.HTML != NewVal.HTML) { diff.HTML = NewVal.HTML; }
                if (OldVal.Links != NewVal.Links) { diff.Links = NewVal.Links; }
                if (OldVal.Tags != NewVal.Tags) { diff.Tags = NewVal.Tags; }
                return diff;
            }

            // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
            public List<string> EGCi_DiffList(EGCi OldVal, EGCi NewVal)
            {
                List<string> diffList = new List<string>();
                if (OldVal.ID != NewVal.ID) { diffList.Add("Changed ID Value From " + OldVal.ID + " To " + NewVal.ID); }
                if (OldVal.ShowTitle != NewVal.ShowTitle) { diffList.Add("Changed ShowTitle Value From " + OldVal.ShowTitle + " To " + NewVal.ShowTitle); }
                if (OldVal.EpGuideURL != NewVal.EpGuideURL) { diffList.Add("Changed EpGuideURL Value From " + OldVal.EpGuideURL + " To " + NewVal.EpGuideURL); }
                if (OldVal.TVRage != NewVal.TVRage) { diffList.Add("Changed TVRage Value From " + OldVal.TVRage + " To " + NewVal.TVRage); }
                if (OldVal.TVMaze != NewVal.TVMaze) { diffList.Add("Changed TVMaze Value From " + OldVal.TVMaze + " To " + NewVal.TVMaze); }
                if (OldVal.IMDb != NewVal.IMDb) { diffList.Add("Changed IMDb Value From " + OldVal.IMDb + " To " + NewVal.IMDb); }
                if (OldVal.StartDate != NewVal.StartDate) { diffList.Add("Changed StartDate Value From " + OldVal.StartDate + " To " + NewVal.StartDate); }
                if (OldVal.EndDate != NewVal.EndDate) { diffList.Add("Changed EndDate Value From " + OldVal.EndDate + " To " + NewVal.EndDate); }
                if (OldVal.EpCount != NewVal.EpCount) { diffList.Add("Changed EpCount Value From " + OldVal.EpCount + " To " + NewVal.EpCount); }
                if (OldVal.RunTime != NewVal.RunTime) { diffList.Add("Changed RunTime Value From " + OldVal.RunTime + " To " + NewVal.RunTime); }
                if (OldVal.Network != NewVal.Network) { diffList.Add("Changed Network Value From " + OldVal.Network + " To " + NewVal.Network); }
                if (OldVal.Country != NewVal.Country) { diffList.Add("Changed Country Value From " + OldVal.Country + " To " + NewVal.Country); }
                if (OldVal.HTML != NewVal.HTML) { diffList.Add("Changed HTML Value From " + OldVal.HTML + " To " + NewVal.HTML); }
                if (OldVal.Links != NewVal.Links) { diffList.Add("Changed Links Value From " + OldVal.Links + " To " + NewVal.Links); }
                if (OldVal.Tags != NewVal.Tags) { diffList.Add("Changed Tags Value From " + OldVal.Tags + " To " + NewVal.Tags); }
                return diffList;
            }


            #endregion

            #region === EGCi SQL Functions ===

            // Return EGCi SQL Connection String
            public SqlConnection EGCi_Conn(bool Local = true)
            {

                if (Local) { return new SqlConnection(ConfigurationManager.ConnectionStrings["LITMLucidMedia"].ConnectionString); }
                // populate sql connection
                //SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["epGuideDb"].ConnectionString);
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLserver"].ConnectionString);


                return Conn;
            }

            // Return EGCi TableName (Full Path)
            public string EGCi_TableName(bool Local = true)
            {
                string EpGuideShowIndexTable = "[lucidmedia].[lucidmethod].[EpGuides_ShowIndex]";

                if (Local) { return "[lucidmedia].[dbo].[EpGuides_ShowIndex]"; }

                return EpGuideShowIndexTable;
            }

            public bool EGCi_InsertSQL(EGCi obj, bool Local = true)
            {
                SqlConnection Con = EGCi_Conn(Local);
                string SQLLine = "Insert Into " + EGCi_TableName(Local) + "(ShowTitle, EpGuideURL, TVRage, TVMaze, IMDb, StartDate, EndDate, EpCount, RunTime, Network, Country, HTML, Links, Tags) VALUES (@ShowTitle, @EpGuideURL, @TVRage, @TVMaze, @IMDb, @StartDate, @EndDate, @EpCount, @RunTime, @Network, @Country, @HTML, @Links, @Tags)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.ShowTitle == null) { obj.ShowTitle = ""; }
                if (obj.EpGuideURL == null) { obj.EpGuideURL = ""; }
                if (obj.TVRage == null) { obj.TVRage = ""; }
                if (obj.TVMaze == null) { obj.TVMaze = ""; }
                if (obj.IMDb == null) { obj.IMDb = ""; }
                if (obj.StartDate == null) { obj.StartDate = ""; }
                if (obj.EndDate == null) { obj.EndDate = ""; }
                if (obj.EpCount == null) { obj.EpCount = ""; }
                if (obj.RunTime == null) { obj.RunTime = ""; }
                if (obj.Network == null) { obj.Network = ""; }
                if (obj.Country == null) { obj.Country = ""; }
                if (obj.HTML == null) { obj.HTML = ""; }
                if (obj.Links == null) { obj.Links = ""; }
                if (obj.Tags == null) { obj.Tags = ""; }


                string sTitle = obj.ShowTitle.Replace("\"", "");
                string epCount = obj.EpCount.Replace("\"", "");
                string runTime = obj.RunTime.Replace("\"", "");
                string network = obj.Network.Replace("\"", "");
                string country = obj.Country.Replace("\"", "");

                string startDate = obj.StartDate;
                string endDate = obj.EndDate;

                if (startDate.Contains("_")) { startDate = "Dec 2022"; }
                if (startDate.Trim() == "___ ____") { startDate = "Dec 2022"; }

                if (endDate.Contains("_")) { endDate = "Dec 2022"; }
                if (endDate.Trim() == "___ ____") { endDate = "Dec 2022"; }

                endDate = endDate.Replace("?", "");
                startDate = startDate.Replace("?", "");

                cmd2.Parameters.AddWithValue(@"ShowTitle", sTitle);
                cmd2.Parameters.AddWithValue(@"EpGuideURL", obj.EpGuideURL.ToString());
                cmd2.Parameters.AddWithValue(@"TVRage", obj.TVRage.ToString());
                cmd2.Parameters.AddWithValue(@"TVMaze", obj.TVMaze.ToString());
                cmd2.Parameters.AddWithValue(@"IMDb", "");
                cmd2.Parameters.AddWithValue(@"StartDate", startDate);
                cmd2.Parameters.AddWithValue(@"EndDate", endDate);
                cmd2.Parameters.AddWithValue(@"EpCount", epCount.Trim());
                cmd2.Parameters.AddWithValue(@"RunTime", runTime);
                cmd2.Parameters.AddWithValue(@"Network", network);
                cmd2.Parameters.AddWithValue(@"Country", country);
                cmd2.Parameters.AddWithValue(@"HTML", "");
                cmd2.Parameters.AddWithValue(@"Links", "");
                cmd2.Parameters.AddWithValue(@"Tags", "");


                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    ahk.FileAppend(sTitle + "|" + ex.ToString(), ahk.AppDir() + "\\SQLErrors.txt");
                    //ahk.MsgBox(ex.ToString()); 
                    return false;
                }
                Con.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool EGCi_UpdateSQL(EGCi obj, bool Local = true)
            {
                SqlConnection Conn = EGCi_Conn(Local);
                string SQLLine = "Update " + EGCi_TableName(Local) + " SET EpGuideURL = @EpGuideURL, TVRage = @TVRage, TVMaze = @TVMaze, IMDb = @IMDb, StartDate = @StartDate, EndDate = @EndDate, EpCount = @EpCount, RunTime = @RunTime, Network = @Network, Country = @Country, HTML = @HTML, Links = @Links, Tags = @Tags WHERE ShowTitle = @ShowTitle";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);

                if (obj.ShowTitle == null) { obj.ShowTitle = ""; }
                if (obj.EpGuideURL == null) { obj.EpGuideURL = ""; }
                if (obj.TVRage == null) { obj.TVRage = ""; }
                if (obj.TVMaze == null) { obj.TVMaze = ""; }
                if (obj.IMDb == null) { obj.IMDb = ""; }
                if (obj.StartDate == null) { obj.StartDate = ""; }
                if (obj.EndDate == null) { obj.EndDate = ""; }
                if (obj.EpCount == null) { obj.EpCount = ""; }
                if (obj.RunTime == null) { obj.RunTime = ""; }
                if (obj.Network == null) { obj.Network = ""; }
                if (obj.Country == null) { obj.Country = ""; }
                if (obj.HTML == null) { obj.HTML = ""; }
                if (obj.Links == null) { obj.Links = ""; }
                if (obj.Tags == null) { obj.Tags = ""; }


                string sTitle = obj.ShowTitle.Replace("\"", "");
                string epCount = obj.EpCount.Replace("\"", "");
                string runTime = obj.RunTime.Replace("\"", "");
                string network = obj.Network.Replace("\"", "");
                string country = obj.Country.Replace("\"", "");

                string startDate = obj.StartDate;
                string endDate = obj.EndDate;

                if (startDate.Contains("_")) { startDate = "Dec 2022"; }
                if (startDate.Trim() == "___ ____") { startDate = "Dec 2022"; }

                if (endDate.Contains("_")) { endDate = "Dec 2022"; }
                if (endDate.Trim() == "___ ____") { endDate = "Dec 2022"; }

                endDate = endDate.Replace("?", "");
                startDate = startDate.Replace("?", "");

                cmd2.Parameters.AddWithValue(@"ShowTitle", sTitle);
                cmd2.Parameters.AddWithValue(@"EpGuideURL", obj.EpGuideURL.ToString());
                cmd2.Parameters.AddWithValue(@"TVRage", obj.TVRage.ToString());
                cmd2.Parameters.AddWithValue(@"TVMaze", obj.TVMaze.ToString());
                cmd2.Parameters.AddWithValue(@"IMDb", "");
                cmd2.Parameters.AddWithValue(@"StartDate", startDate);
                cmd2.Parameters.AddWithValue(@"EndDate", endDate);
                cmd2.Parameters.AddWithValue(@"EpCount", epCount.Trim());
                cmd2.Parameters.AddWithValue(@"RunTime", runTime);
                cmd2.Parameters.AddWithValue(@"Network", network);
                cmd2.Parameters.AddWithValue(@"Country", country);
                cmd2.Parameters.AddWithValue(@"HTML", "");
                cmd2.Parameters.AddWithValue(@"Links", "");
                cmd2.Parameters.AddWithValue(@"Tags", "");


                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    ahk.FileAppend(sTitle + "|" + ex.ToString(), ahk.AppDir() + "\\SQLErrors.txt");
                    //ahk.MsgBox(ex.ToString()); 
                    return false;
                }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool EGCi_UpdateInsert(EGCi obj, bool Local = true)
            {
                SqlConnection Conn = EGCi_Conn(Local);
                bool Updated = EGCi_UpdateSQL(obj, Local);  // try to update record first
                if (!Updated) { Updated = EGCi_InsertSQL(obj, Local); }  // if unable to update, insert new record
                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool EGCi_UpdateIfPopulated(EGCi obj, string ID = "")
            {
                SqlConnection Conn = EGCi_Conn();
                string SQLcmd = "Update " + EGCi_TableName() + " SET ";
                if (obj.ID != null) { SQLcmd = SQLcmd + " ID = @ID,"; }
                if (obj.ShowTitle != null) { SQLcmd = SQLcmd + " ShowTitle = @ShowTitle,"; }
                if (obj.EpGuideURL != null) { SQLcmd = SQLcmd + " EpGuideURL = @EpGuideURL,"; }
                if (obj.TVRage != null) { SQLcmd = SQLcmd + " TVRage = @TVRage,"; }
                if (obj.TVMaze != null) { SQLcmd = SQLcmd + " TVMaze = @TVMaze,"; }
                if (obj.IMDb != null) { SQLcmd = SQLcmd + " IMDb = @IMDb,"; }
                if (obj.StartDate != null) { SQLcmd = SQLcmd + " StartDate = @StartDate,"; }
                if (obj.EndDate != null) { SQLcmd = SQLcmd + " EndDate = @EndDate,"; }
                if (obj.EpCount != null) { SQLcmd = SQLcmd + " EpCount = @EpCount,"; }
                if (obj.RunTime != null) { SQLcmd = SQLcmd + " RunTime = @RunTime,"; }
                if (obj.Network != null) { SQLcmd = SQLcmd + " Network = @Network,"; }
                if (obj.Country != null) { SQLcmd = SQLcmd + " Country = @Country,"; }
                if (obj.HTML != null) { SQLcmd = SQLcmd + " HTML = @HTML,"; }
                if (obj.Links != null) { SQLcmd = SQLcmd + " Links = @Links,"; }
                if (obj.Tags != null) { SQLcmd = SQLcmd + " Tags = @Tags,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ID = @ID";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.ID != null) { cmd2.Parameters.AddWithValue(@"ID", obj.ID); }
                if (obj.ShowTitle != null) { cmd2.Parameters.AddWithValue(@"ShowTitle", obj.ShowTitle); }
                if (obj.EpGuideURL != null) { cmd2.Parameters.AddWithValue(@"EpGuideURL", obj.EpGuideURL); }
                if (obj.TVRage != null) { cmd2.Parameters.AddWithValue(@"TVRage", obj.TVRage); }
                if (obj.TVMaze != null) { cmd2.Parameters.AddWithValue(@"TVMaze", obj.TVMaze); }
                if (obj.IMDb != null) { cmd2.Parameters.AddWithValue(@"IMDb", obj.IMDb); }
                if (obj.StartDate != null) { cmd2.Parameters.AddWithValue(@"StartDate", obj.StartDate); }
                if (obj.EndDate != null) { cmd2.Parameters.AddWithValue(@"EndDate", obj.EndDate); }
                if (obj.EpCount != null) { cmd2.Parameters.AddWithValue(@"EpCount", obj.EpCount); }
                if (obj.RunTime != null) { cmd2.Parameters.AddWithValue(@"RunTime", obj.RunTime); }
                if (obj.Network != null) { cmd2.Parameters.AddWithValue(@"Network", obj.Network); }
                if (obj.Country != null) { cmd2.Parameters.AddWithValue(@"Country", obj.Country); }
                if (obj.HTML != null) { cmd2.Parameters.AddWithValue(@"HTML", obj.HTML); }
                if (obj.Links != null) { cmd2.Parameters.AddWithValue(@"Links", obj.Links); }
                if (obj.Tags != null) { cmd2.Parameters.AddWithValue(@"Tags", obj.Tags); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public EGCi EGCi_ReturnSQL(string ShowName = "", bool Local = true)
            {
                SqlConnection Conn = EGCi_Conn(Local);
                string SelectLine = "Select [ID],[ShowTitle],[EpGuideURL],[TVRage],[TVMaze],[IMDb],[StartDate],[EndDate],[EpCount],[RunTime],[Network],[Country],[HTML],[Links],[Tags] From " + EGCi_TableName(Local) + " WHERE [ShowTitle] = '" + ShowName + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                EGCi returnObject = new EGCi();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.ShowTitle = ret["ShowTitle"].ToString();
                        returnObject.EpGuideURL = ret["EpGuideURL"].ToString();
                        returnObject.TVRage = ret["TVRage"].ToString();
                        returnObject.TVMaze = ret["TVMaze"].ToString();
                        returnObject.IMDb = ret["IMDb"].ToString();
                        returnObject.StartDate = ret["StartDate"].ToString();
                        returnObject.EndDate = ret["EndDate"].ToString();
                        returnObject.EpCount = ret["EpCount"].ToString();
                        returnObject.RunTime = ret["RunTime"].ToString();
                        returnObject.Network = ret["Network"].ToString();
                        returnObject.Country = ret["Country"].ToString();
                        returnObject.HTML = ret["HTML"].ToString();
                        returnObject.Links = ret["Links"].ToString();
                        returnObject.Tags = ret["Tags"].ToString();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public List<EGCi> EGCi_ReturnSQLList(string Command)
            {
                SqlConnection Conn = EGCi_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<EGCi> ReturnList = new List<EGCi>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        EGCi returnObject = new EGCi();
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.ShowTitle = ret["ShowTitle"].ToString();
                        returnObject.EpGuideURL = ret["EpGuideURL"].ToString();
                        returnObject.TVRage = ret["TVRage"].ToString();
                        returnObject.TVMaze = ret["TVMaze"].ToString();
                        returnObject.IMDb = ret["IMDb"].ToString();
                        returnObject.StartDate = ret["StartDate"].ToString();
                        returnObject.EndDate = ret["EndDate"].ToString();
                        returnObject.EpCount = ret["EpCount"].ToString();
                        returnObject.RunTime = ret["RunTime"].ToString();
                        returnObject.Network = ret["Network"].ToString();
                        returnObject.Country = ret["Country"].ToString();
                        returnObject.HTML = ret["HTML"].ToString();
                        returnObject.Links = ret["Links"].ToString();
                        returnObject.Tags = ret["Tags"].ToString();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }



            public List<string> ParseCSVwithCommas(string csv)
            {
                Microsoft.VisualBasic.FileIO.TextFieldParser parser = new Microsoft.VisualBasic.FileIO.TextFieldParser(new StringReader(csv));

                // You can also read from a file
                // TextFieldParser parser = new TextFieldParser("mycsvfile.csv");

                List<string> csvParts = new List<string>();

                parser.HasFieldsEnclosedInQuotes = true;
                parser.SetDelimiters(",");

                string[] fields;

                while (!parser.EndOfData)
                {
                    fields = parser.ReadFields();
                    foreach (string field in fields)
                    {
                        //Console.WriteLine(field);
                        csvParts.Add(field);
                    }
                }

                parser.Close();

                return csvParts;
            }


            public void GoDaddy_To_LocalSQL()
            {
                List<EGCi> list = EGCi_ReturnSQLList("Select * From " + EGCi_TableName());

                int i = 1;
                foreach (EGCi item in list)
                {
                    sb.StatusBar("Adding " + i + " / " + list.Count.ToString());
                    NewEpGuide_InsertSQL(item);
                    i++;
                }


                //SqlConnection localConn = new SqlConnection("Server=(localdb)\\MyInstance;DataBase=EpisodeGuides;Integrated Security=true;");

                ////List<string> test = lst.SQL_To_List(localConn, "select * FROM [EpisodeGuides].[dbo].[ComIndex]");

                //string count = sql.SQL_Return_Value(localConn, "select count(Imdb) FROM [EpisodeGuides].[dbo].[ComIndex]");

                ahk.MsgBox("FINSIHED!");

            }


            public void NewEpGuide_InsertSQL(EGCi obj)
            {
                SqlConnection Conn = new SqlConnection("Server=(localdb)\\MyInstance;DataBase=EpisodeGuides;Integrated Security=true;");
                string SQLLine = "Insert Into [EpisodeGuides].[dbo].[ComIndex](ShowID, ShowTitle, EpGuideURL, TVRage, TVMaze, IMDb, StartDate, EndDate, EpCount, RunTime, Network, Country, Tags) VALUES (@ShowID, @ShowTitle, @EpGuideURL, @TVRage, @TVMaze, @IMDb, @StartDate, @EndDate, @EpCount, @RunTime, @Network, @Country, @Tags)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);

                //if (obj.ShowTitle == null) { obj.ShowTitle = ""; }
                //if (obj.EpGuideURL == null) { obj.EpGuideURL = ""; }
                //if (obj.TVRage == null) { obj.TVRage = ""; }
                //if (obj.TVMaze == null) { obj.TVMaze = ""; }
                //if (obj.IMDb == null) { obj.IMDb = ""; }
                //if (obj.StartDate == null) { obj.StartDate = ""; }
                //if (obj.EndDate == null) { obj.EndDate = ""; }
                //if (obj.EpCount == null) { obj.EpCount = ""; }
                //if (obj.RunTime == null) { obj.RunTime = ""; }
                //if (obj.Network == null) { obj.Network = ""; }
                //if (obj.Country == null) { obj.Country = ""; }
                //if (obj.Tags == null) { obj.Tags = ""; }



                //if (startDate.Contains("_")) { startDate = "Dec 2022"; }

                string showID = obj.EpGuideURL.Replace("http://epguides.com/", "");
                showID = showID.Replace("/", "");


                string imdbTitle = obj.IMDb;
                if (imdbTitle == "")
                {
                    //IMDbLink_FromEpGuideHTML(string url = "", string html = "")

                    string htmlFile = @"C:\_Code\LucidProjects\ADBindex\ADBindex\bin\Debug\EpGuides\HTML\" + showID + ".txt";

                    if (File.Exists(htmlFile))
                    {
                        string html = ahk.FileRead(htmlFile);

                        // returns imdb link from html, if url provided downloading link, if skipped and html provided, parses out link
                        imdbTitle = IMDbLink_FromEpGuideHTML("", html);




                        // extract just the imdb title id
                        List<string> Parts = ahk.StringSplit_List(imdbTitle, "/");
                        int sL = 0;
                        foreach (string Part in Parts)
                        {
                            if (Part == "http:") { continue; }
                            if (Part.Contains("tt")) { imdbTitle = Part; break; }
                        }
                    }


                }



                cmd2.Parameters.AddWithValue(@"ShowID", showID);
                cmd2.Parameters.AddWithValue(@"ShowTitle", obj.ShowTitle);
                cmd2.Parameters.AddWithValue(@"EpGuideURL", obj.EpGuideURL.ToString());
                cmd2.Parameters.AddWithValue(@"TVRage", obj.TVRage.ToString());
                cmd2.Parameters.AddWithValue(@"TVMaze", obj.TVMaze.ToString());
                cmd2.Parameters.AddWithValue(@"IMDb", imdbTitle);
                cmd2.Parameters.AddWithValue(@"StartDate", obj.StartDate);
                cmd2.Parameters.AddWithValue(@"EndDate", obj.EndDate);
                cmd2.Parameters.AddWithValue(@"EpCount", obj.EpCount);
                cmd2.Parameters.AddWithValue(@"RunTime", obj.RunTime);
                cmd2.Parameters.AddWithValue(@"Network", obj.Network);
                cmd2.Parameters.AddWithValue(@"Country", obj.Country);
                cmd2.Parameters.AddWithValue(@"Tags", "");


                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    //ahk.FileAppend(sTitle + "|" + ex.ToString(), ahk.AppDir() + "\\SQLErrors.txt");
                    ahk.MsgBox(ex.ToString());
                    //return false;
                }



                string FilePath = @"C:\_Code\LucidProjects\ADBindex\ADBindex\bin\Debug\EpGuides\Images\" + showID + ".jpg";

                if (!File.Exists(FilePath)) { FilePath = @"E:\Missing.png"; }

                if (File.Exists(FilePath))
                {

                    // write image file 

                    FileInfo info = new FileInfo(FilePath);
                    string FileName = info.Name;
                    string FileSize = info.Length.ToString();
                    string DateModified = info.LastWriteTime.ToString();

                    //SqlConnection SQLConn = new SqlConnection("Server=198.71.225.113;DataBase=MediaServer;Uid=lucid;Pwd=Go1Daddy88");

                    if (Conn.State == ConnectionState.Closed) { Conn.Open(); }

                    byte[] file;
                    using (var stream = new FileStream(FilePath, FileMode.Open, System.IO.FileAccess.Read))
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            file = reader.ReadBytes((int)stream.Length);
                        }
                    }
                    //using (var varConnection = Locale.sqlConnectOneTime(Locale.sqlDataConnectionDetails))

                    using (var sqlWrite = new SqlCommand("UPDATE [EpisodeGuides].[dbo].[ComIndex] Set [ShowImage] = @ShowImage WHERE [EpGuideURL] = @EpGuideURL", Conn))
                    {
                        sqlWrite.Parameters.AddWithValue("@EpGuideURL", obj.EpGuideURL);
                        sqlWrite.Parameters.Add("@ShowImage", SqlDbType.VarBinary, file.Length).Value = file;
                        sqlWrite.ExecuteNonQuery();
                    }

                }

                Conn.Close();
            }



            #endregion


            #endregion

            #region === Collection Management ===

            // dir to list for generating link with prefix
            public void Create_LinkList(string ShowDir, string LinkPrefix = "liveinthemedia.com/uploads/")
            {
                List<string> list = Return_LinkList(ShowDir, LinkPrefix);
                string links = lst.List_To_String_NewLines(list);

                string tmpFile = ahk.AppDir() + "\\LinkList.txt";
                ahk.FileDelete(tmpFile);
                ahk.FileAppend(links, tmpFile);
                ahk.Run(tmpFile);
            }


            // showdir to list, adding linkprefix for rapidgator uploads from webserver

            public List<string> Return_LinkList(string ShowDir, string LinkPrefix = "liveinthemedia.com/uploads/")
            {
                List<string> links = new List<string>();

                // return list of files in directory, loop through each file in list
                List<string> files = lst.FileList(ShowDir, "*.*", true, false, true);
                foreach (string file in files)
                {
                    string link = file.Replace("T:\\", "");
                    link = link.Replace("\\", "/");
                    link = LinkPrefix + link;
                    links.Add(link);
                }

                return links;
            }


            #endregion


            #region === Collection SQLITE FILES ===

            /// <summary>
            /// Looks in Show Dir Root for .URL Files, Parses, and Creates SQLite EpGuide File
            /// </summary>
            /// <param name="TVSeriesRoot"></param>
            /// <param name="NewThread"></param>
            public void CreateEpguides_SQLite(string TVSeriesRoot = @"T:\", RadProgressBar Bar = null, bool NewThread = true)
            {
                if (NewThread)
                {
                    Thread newThread = new Thread(() => CreateEpguides_SQLite(TVSeriesRoot, Bar, false)); // Function To Execute
                    newThread.Name = "CreateEpguides";
                    newThread.IsBackground = true;
                    newThread.Start();
                }
                else
                {
                    bool AddEpGuide = true;
                    bool AddIMDbGuide = true;

                    _Sites.EpGuides ep = new _Sites.EpGuides();
                    _Sites.Imdb imdb = new _Sites.Imdb();
                    _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();

                    string epGuideID = "";
                    string[] files = Directory.GetFiles(TVSeriesRoot, "*.url", System.IO.SearchOption.AllDirectories);

                    if (Bar != null) { pro.SetupProgressBar(Bar, files.Count()); }
                    int i = 0;
                    foreach (string file in files)
                    {
                        i++;
                        if (Bar != null) { pro.UpdateProgress(Bar, i + "/" + files.Count() + " | " + ahk.FileNameNoExt(file)); }
                        epGuideID = "";
                        string URLPath = ahk.IniRead(file, "InternetShortcut", "URL");

                        if (URLPath.ToUpper().Contains("EPGUIDES.COM")) { epGuideID = ep.ParseEpGuideID(URLPath); }

                        if (URLPath.ToUpper().Contains("IMDB.COM"))
                        {
                            if (URLPath.Contains("?"))  // parse out unneeded link info
                            {
                                URLPath = ahk.StringSplit(URLPath, "?", 0);
                            }

                            string IMDbID = imdb.ParseID(URLPath);

                            if (AddIMDbGuide)
                            {
                                if (Bar != null) { pro.UpdateProgress(Bar, i + "/" + files.Count() + " | IMDbID: " + IMDbID); }

                                IMDbTVSeason season = imdb.ParseIMDbShow(IMDbID, 0, false, false, true, ahk.FileDir(file));

                                int capturedSeason = season.SeasonNum;

                                do
                                {
                                    capturedSeason--;
                                    if (Bar != null) { pro.UpdateProgress(Bar, i + "/" + files.Count() + " | IMDbID: " + IMDbID + " | Season: " + capturedSeason); }
                                    season = imdb.ParseIMDbShow(IMDbID, capturedSeason, false, false, true, ahk.FileDir(file));
                                }
                                while (capturedSeason > 1);

                            }

                        }

                        if (AddEpGuide)
                        {
                            if (epGuideID != "") { ep.Create_EpGuide_SQLite(ahk.FileDir(file)); }
                        }

                    }

                    ahk.MsgBox("Finished EpGuide Create");
                }

            }




            #endregion




            // setup global static object
            public static class epGuideGui
            {
                public static StatusBar mainStatusBar { get; set; }
                public static StatusBarPanel statusPanel { get; set; }
                public static StatusBarPanel datetimePanel { get; set; }
                public static Form fromForm { get; set; }
            }



            public void Create_EpGuide_SQLite(string TVShowRoot = @"T:\Family Guy")
            {
                show tvShow = TVServer_ShowDirInfo(TVShowRoot);

                EpGuides_DownloadParseShow_SQLite(tvShow);
            }



            public void Epguides_UpdateGuides()
            {
                string cmd = "select EpguideURL FROM " + LITMShowIndexTable + " where EpGuideURl != ''";

                List<string> results = lst.SQL_To_List(show_Conn(), cmd);
                int i = 1;
                foreach (string result in results)
                {
                    StatusBar("EpGuides Import: " + result + " | " + i + " / " + results.Count());
                    //cmd = "select ShowName FROM [lucidmedia].[lucidmethod].[LITM_ShowIndex] WHERE EpGuideURL = 'StarTrekEnterprise'";

                    EpGuides_DownloadParseShow("http://epguides.com/" + result, "");
                    i++;
                }

                StatusBar("FINISHED EpGuides Import");
            }


            // search for epguide.com url files in local folders, write local root to index
            public void EpGuide_Update_LocalRoots(string SearchDir = "T:\\")
            {
                //===============================================================================
                // loop through parse directory - import all episode guides into SQLite db
                //===============================================================================

                string[] files = Directory.GetFiles(SearchDir, "*.url", System.IO.SearchOption.AllDirectories);

                int i = 1;
                foreach (string file in files)
                {
                    StatusBar("EpGuide LocalRoot Update : " + i + " / " + files.Count()); i++;


                    string URLPath = ahk.IniRead(file, "InternetShortcut", "URL");

                    string EpGuideID = ParseEpGuideID(URLPath);

                    if (EpGuideID.ToUpper().Contains("IMDB.COM")) { continue; }

                    string localRoot = ahk.FileDir(file);

                    string cmd = "update " + LITMShowIndexTable + " set LocalRoot = '" + localRoot + "'  WHERE EpGuideURL = '" + EpGuideID + "'";
                    bool updated = sql.WriteDataRecord(show_Conn(), cmd);

                    if (!updated) { ahk.MsgBox("Error Updating " + EpGuideID); }

                }


                StatusBar("FINISHED EpGuide LocalRoot Update");
                MessageBox.Show("Done Importing to DB");

            }

            /// <summary>
            /// Extract IMDb and EpGuide URLs from TV Show Dir
            /// </summary>
            /// <param name="SearchDir"></param>
            /// <returns></returns>
            public show TVServer_ShowDirInfo(string SearchDir)
            {
                string[] files = Directory.GetFiles(SearchDir, "*.url", System.IO.SearchOption.AllDirectories);

                show s = new show();

                s.ShowName = ahk.DirName(SearchDir);
                s.LocalRoot = SearchDir;
                s.DirSize = ahk.DirSize(SearchDir);

                int i = 1;
                foreach (string url in files)
                {
                    string link = ahk.IniRead(url, "InternetShortcut", "URL");  // Read INI Value
                    if (link.Contains("imdb.com"))
                    {
                        if (link.Contains("?"))  // parse out unneeded link info
                        {
                            link = ahk.StringSplit(link, "?", 0);
                        }

                        s.IMDbURL = ParseIMDbID(link);
                    }
                    if (link.Contains("epguides.com"))
                    {
                        s.EpGuideURL = ParseEpGuideID(link);
                    }
                }

                List<string> RootFiles = lst.FileList(SearchDir, "*.*", false, false, true);
                foreach (string file in RootFiles)
                {
                    if (ahk.FileExt(file).ToUpper() == ".JPG") { s.ImagePath = file; }
                    if (ahk.FileExt(file).ToUpper() == ".JPEG") { s.ImagePath = file; }
                    if (ahk.FileExt(file).ToUpper() == ".PNG") { s.ImagePath = file; }
                }

                return s;
            }


            // download html + parse epguides.com page, writing new show to sql db
            public void EpGuides_DownloadParseShow(string URL, string ShowName = "")
            {
                //StatusBar("Parsing " + URL + " | " + ShowName);

                string epGuideID = ParseEpGuideID(URL);
                string html = cr.dlHTML(URL);
                EpGuides_Html_To_SQL(html, ShowName, epGuideID);

                //StatusBar("FINISHED Parsing " + URL + " | " + ShowName);
            }

            public void EpGuides_DownloadParseShow_SQLite(show tvShow)
            {
                //StatusBar("Parsing " + URL + " | " + ShowName);

                string epGuideID = "";

                if (tvShow.EpGuideURL != null) { epGuideID = ParseEpGuideID(tvShow.EpGuideURL); }
                

                if (epGuideID == "")  // read url file in root dir
                {
                    string[] files = Directory.GetFiles(tvShow.LocalRoot, "*.url", System.IO.SearchOption.AllDirectories);
                    foreach (string file in files)
                    {
                        string URLPath = ahk.IniRead(file, "InternetShortcut", "URL");

                        if (URLPath.ToUpper().Contains("EPGUIDES.COM")) { epGuideID = ParseEpGuideID(URLPath); }

                        if (URLPath.ToUpper().Contains("imdb.com"))
                        {
                            if (URLPath.Contains("?"))  // parse out unneeded link info
                            {
                                URLPath = ahk.StringSplit(URLPath, "?", 0);
                            }

                            tvShow.IMDbURL = ParseIMDbID(URLPath);
                        }
                    }
                }


                string html = cr.dlHTML("http://www.epguides.com/" + epGuideID);
                EpGuides_Html_To_SQLite(html, tvShow.ShowName, epGuideID, tvShow.LocalRoot, tvShow.SQLiteDb);

                //StatusBar("FINISHED Parsing " + URL + " | " + ShowName);
            }



            public void EpGuides_Html_To_SQL(string InText, string ShowName = "", string EpGuideID = "")
            {
                litmEpisodes ep = new litmEpisodes();

                if (ShowName == "" && EpGuideID != "")
                {
                    ShowName = ShowNameFromEpGuideID(EpGuideID);
                }

                ep.ShowName = ShowName;

                show showInfo = ReturnShow(ShowName, EpGuideID);


                ep.LocalPath = showInfo.LocalRoot;
                ep.EpGuideID = showInfo.EpGuideURL;
                ep.ShowName = showInfo.ShowName;

                string[] words4 = InText.Split('\n');
                foreach (string line in words4)
                {
                    if (line.Contains("episode"))
                    {
                        string lineText = line.Replace("'", "");
                        if (lineText.Contains("id=\"TVHeader\"")) { continue; }
                        if (lineText.Contains("<meta")) { continue; }
                        ep = ParseEpisodeLine_SQL(lineText, ep);
                        continue;
                    }

                    if (line.Contains("show"))
                    {
                        ep.ShowName = showInfo.ShowName;

                        string lineText = line.Replace("'", "");
                        if (lineText.Contains("id=\"TVHeader\"")) { continue; }
                        if (lineText.Contains("<meta")) { continue; }
                        ep = ParseEpisodeLine_SQL(lineText, ep);
                    }
                }
            }

            public void EpGuides_Html_To_SQLite(string InText, string ShowName = "", string EpGuideID = "", string ShowRootDir = "", string DbFile = "")
            {
                litmEpisodes ep = new litmEpisodes();

                

                if (ShowName == "" && EpGuideID != "")
                {
                    ShowName = ShowNameFromEpGuideID(EpGuideID);
                }

                ep.ShowName = ShowName;

                show showInfo = ReturnShow(ShowName, EpGuideID);

                if (DbFile == "")
                {
                    if (ShowRootDir == "") { ShowRootDir = ahk.AppDir(); }
                    ep.SQLiteDbPath = ShowRootDir + "\\" + ShowName + ".sqlite";
                }
                else
                {
                    ep.SQLiteDbPath = DbFile;
                }


                //ep.LocalPath = showInfo.LocalRoot;
                ep.EpGuideID = showInfo.EpGuideURL;
                ep.ShowName = showInfo.ShowName;

                string[] words4 = InText.Split('\n');
                foreach (string line in words4)
                {
                    if (line.Contains("episode"))
                    {
                        string lineText = line.Replace("'", "");
                        if (lineText.Contains("id=\"TVHeader\"")) { continue; }
                        if (lineText.Contains("<meta")) { continue; }
                        //ep = ParseEpisodeLine_SQL(lineText, ep);
                        ep = ParseEpisodeLine_SQLite(lineText, ep, showInfo);
                        continue;
                    }

                    if (line.Contains("show"))
                    {
                        ep.ShowName = showInfo.ShowName;

                        string lineText = line.Replace("'", "");
                        if (lineText.Contains("id=\"TVHeader\"")) { continue; }
                        if (lineText.Contains("<meta")) { continue; }
                        //ep = ParseEpisodeLine_SQL(lineText, ep);

                        ep = ParseEpisodeLine_SQLite(lineText, ep, showInfo);
                    }
                }
            }



            public void Parse_HTML_TextFile_To_DB(string HtmlTextFile, string DBFile, string LocalTVShowListText = "")
            {

                //=======================================
                // Single File Parse
                //=======================================
                var InTextVar = ahk.FileRead(HtmlTextFile);

                System.IO.FileInfo fileinfo = new System.IO.FileInfo(HtmlTextFile); //retrieve info about each file
                string FileName = fileinfo.Name.ToString();
                string FileExt = fileinfo.Extension.ToString();
                string ShowName = FileName.Replace(FileExt, "");  //remove the file extension from the file name = sid 
                string SID = ShowName;

                SQLite_Delete_Show(ShowName);  // delete first to update show entry


                //string ShowName = ReturnShowNameFromSID(SID);
                //MessageBox.Show(ShowName + " SID = " + SID); 
                //CreateShowTable(DBFile, SID);   // create new table for this show in the EpGuides DB

                //   Read INI Values from Text File
                string DirPath = ahk.IniRead(HtmlTextFile, "MediaInfo", "DirPath");

                EpGuides_Html_To_Db(InTextVar.ToString(), ShowName, DBFile, DirPath, LocalTVShowListText);

            }

            public void EpGuides_Html_To_Db(string InText, string ShowName, string DBFile, string DirPath = "", string LocalTVShowListText = "")
            {
                string[] words4 = InText.Split('\n');
                foreach (string line in words4)
                {
                    if (line.Contains("episode"))
                    {

                        string lineText = line.Replace("'", "");


                        if (lineText.Contains("id=\"TVHeader\""))
                        {
                            continue;
                        }

                        if (lineText.Contains("<meta"))
                        {
                            continue;
                        }

                        ParseEpisodeLine(lineText, ShowName, DBFile, DirPath, LocalTVShowListText);

                    }

                }

            }

            public void ParseEpisodeLine(string ParseLine, string ShowName, string DBFile, string DirPath = "", string LocalTVShowListText = "")  // 2nd parse action - exports to SQLite db
            {
                Match ProductionNumber = _regexProductionNumber.Match(ParseLine);
                Match EpisodeNumber = _regexEpisodeNumber.Match(ParseLine);
                Match AirDate = _regexAirDate.Match(ParseLine);
                Match EpisodeName = _regexEpisodeTitle.Match(ParseLine);
                string LocalPath = "";

                string EpNum = Convert_EpGuideEpNum_To_S00E00_Format(EpisodeNumber.ToString());

                if (DirPath != "")
                {
                    //LocalPath = FindLocalPath_From_EpNumber(DirPath,EpNum); 
                    LocalPath = FindLocalPath_From_LocalFileList(LocalTVShowListText, EpNum, DirPath);
                    //MsgBox(LocalPath);
                }

                if (ProductionNumber.ToString() != "")
                {
                    //_Database.SQLite lite = new _Database.SQLite();
                    //SQLiteConnection dbConnection = lite._ConnectToDB(DBFile); // connect to SQLite DB file path - returns connection data
                    //lite._sqlite("INSERT into [SHOWS] (ShowName, EpName, EpNumber, ProdNumber, AirDate, LocalPath) values ('" + ShowName + "', '" + EpisodeName + "', '" + EpNum + "', '" + ProductionNumber + "', '" + AirDate + "', '" + LocalPath + "')", dbConnection);  // insert into a Table

                    string line = ShowName + "', '" + EpisodeName + "', '" + EpNum + "', '" + ProductionNumber + "', '" + AirDate + "', '" + LocalPath;
                    ahk.MsgBox(line);

                }


            }

            public litmEpisodes ParseEpisodeLine_SQL(string ParseLine, litmEpisodes episode)
            {
                episode epi = new episode();
                epi.ShowName = episode.ShowName;

                litmEpisodes eps = new litmEpisodes();
                eps.ShowName = episode.ShowName;
                eps.EpGuideID = episode.EpGuideID;
                eps.LocalPath = episode.LocalPath;


                Match ProductionNumber = _regexProductionNumber.Match(ParseLine);
                Match EpisodeNumber = _regexEpisodeNumber.Match(ParseLine);

                // try to match epnumber again if nothing returned
                if (EpisodeNumber.ToString() == "")
                {
                    string noSpaces = ahk.StringReplace(ParseLine, "- ", "-");
                    EpisodeNumber = _regexEpisodeNumber.Match(noSpaces);
                }

                Match AirDate = _regexAirDate.Match(ParseLine);

                Match EpisodeName = _regexEpisodeTitle.Match(ParseLine);
                string LocalPath = "";

                string EpNum = Convert_EpGuideEpNum_To_S00E00_Format(EpisodeNumber.ToString());

                if (epi.LocalPath != "")
                {
                    //LocalPath = FindLocalPath_From_EpNumber(DirPath,EpNum); 
                    //LocalPath = FindLocalPath_From_LocalFileList(LocalTVShowListText, EpNum, DirPath);
                    //MsgBox(LocalPath);
                }

                if (ProductionNumber.ToString() != "")
                {

                    DateTime airDate = new DateTime();

                    if (AirDate.ToString() == "")
                    {
                        string noSpaces = ahk.StringReplace(ParseLine, "- ", "-");
                        noSpaces = ahk.StringReplace(noSpaces, ProductionNumber.ToString());
                        noSpaces = ahk.StringReplace(noSpaces, EpisodeNumber.ToString());
                        noSpaces = ahk.StringSplit(noSpaces, "<", 0);
                        noSpaces = noSpaces.Trim();

                        //noSpaces = ahk.StringSplit(noSpaces, " ", 1); 
                        List<string> noSpacelist = ahk.StringSplit_List(noSpaces, " ");
                        int i = 0; int t = 0;
                        string dateout = "";
                        foreach (string it in noSpacelist)
                        {
                            if (noSpacelist.Count == 4) // line contains production # before date
                            {
                                if (i != 0)
                                {
                                    if (it.Trim() != "")
                                    {
                                        if (dateout == "") { dateout = it; }
                                        else
                                        {
                                            if (t == 0) { dateout = dateout + " " + it; }
                                            if (t == 1) { dateout = dateout + " " + it; }
                                            if (t == 2) { dateout = dateout + " 20" + it; }

                                            t++;
                                        }
                                    }
                                }
                            }

                            if (noSpacelist.Count == 3) // line doesn't contian production # before date
                            {
                                if (it.Trim() != "")
                                {
                                    if (dateout == "") { dateout = it; }
                                    else
                                    {
                                        if (t == 0) { dateout = dateout + " " + it; }
                                        if (t == 1) { dateout = dateout + " " + it; }
                                        if (t == 2) { dateout = dateout + " 20" + it; }

                                        t++;
                                    }
                                }
                            }



                            i++;
                        }

                        airDate = ahk.ToDateTime(dateout);

                        //AirDate = ahk._regexAirDate.Match(noSpaces);
                    }
                    else
                    {
                        airDate = ahk.ToDateTime(AirDate.ToString());
                    }

                    //_Database.SQLite lite = new _Database.SQLite();
                    //SQLiteConnection dbConnection = lite._ConnectToDB(DBFile); // connect to SQLite DB file path - returns connection data
                    //lite._sqlite("INSERT into [SHOWS] (ShowName, EpName, EpNumber, ProdNumber, AirDate, LocalPath) values ('" + ShowName + "', '" + EpisodeName + "', '" + EpNum + "', '" + ProductionNumber + "', '" + AirDate + "', '" + LocalPath + "')", dbConnection);  // insert into a Table
                    epi.EpisodeName = EpisodeName.ToString();


                    epi.EpisodeName = ahk.StringReplace(epi.EpisodeName, "&amp;", "&");
                    epi.EpisodeName = ahk.StringReplace(epi.EpisodeName, "&quot;", "'");
                    eps.EpisodeName = epi.EpisodeName;

                    epi.EpNum = EpNum;
                    epi.ProductionNumber = ProductionNumber.ToString();

                    eps.EpNum = EpNum;
                    eps.ProductionNum = ProductionNumber.ToString();
                    eps.EpGuideID = episode.EpGuideID;

                    epi.AirDate = airDate.ToString();
                    eps.AirDate = airDate.ToString();

                    //string line = ShowName + "', '" + EpisodeName + "', '" + EpNum + "', '" + ProductionNumber + "', '" + AirDate + "', '" + LocalPath;
                    //ahk.MsgBox(line);


                    //EpGuides_UpdateInsert(eps, string DbFile = "");

                    //litmEpisodes_UpdateInsert(eps);

                    episode_UpdateInsert(epi);
                }

                return eps;
            }

            public litmEpisodes ParseEpisodeLine_SQLite(string ParseLine, litmEpisodes episode, show showInfo)
            {
                episode epi = new episode();
                epi.ShowName = episode.ShowName;

                litmEpisodes eps = new litmEpisodes();
                eps.ShowName = episode.ShowName;
                eps.EpGuideID = episode.EpGuideID;
                eps.LocalPath = episode.LocalPath;
                eps.SQLiteDbPath = episode.SQLiteDbPath;

                Match ProductionNumber = _regexProductionNumber.Match(ParseLine);
                Match EpisodeNumber = _regexEpisodeNumber.Match(ParseLine);

                // try to match epnumber again if nothing returned
                if (EpisodeNumber.ToString() == "")
                {
                    string noSpaces = ahk.StringReplace(ParseLine, "- ", "-");
                    EpisodeNumber = _regexEpisodeNumber.Match(noSpaces);
                }

                Match AirDate = _regexAirDate.Match(ParseLine);

                Match EpisodeName = _regexEpisodeTitle.Match(ParseLine);
                string LocalPath = "";

                string EpNum = Convert_EpGuideEpNum_To_S00E00_Format(EpisodeNumber.ToString());

                if (epi.LocalPath != "")
                {
                    //LocalPath = FindLocalPath_From_EpNumber(DirPath,EpNum); 
                    //LocalPath = FindLocalPath_From_LocalFileList(LocalTVShowListText, EpNum, DirPath);
                    //MsgBox(LocalPath);
                }

                if (ProductionNumber.ToString() != "")
                {

                    DateTime airDate = new DateTime();

                    if (AirDate.ToString() == "")
                    {
                        string noSpaces = ahk.StringReplace(ParseLine, "- ", "-");
                        noSpaces = ahk.StringReplace(noSpaces, ProductionNumber.ToString());
                        noSpaces = ahk.StringReplace(noSpaces, EpisodeNumber.ToString());
                        noSpaces = ahk.StringSplit(noSpaces, "<", 0);
                        noSpaces = noSpaces.Trim();

                        //noSpaces = ahk.StringSplit(noSpaces, " ", 1); 
                        List<string> noSpacelist = ahk.StringSplit_List(noSpaces, " ");
                        int i = 0; int t = 0;
                        string dateout = "";
                        foreach (string it in noSpacelist)
                        {
                            if (noSpacelist.Count == 4) // line contains production # before date
                            {
                                if (i != 0)
                                {
                                    if (it.Trim() != "")
                                    {
                                        if (dateout == "") { dateout = it; }
                                        else
                                        {
                                            if (t == 0) { dateout = dateout + " " + it; }
                                            if (t == 1) { dateout = dateout + " " + it; }
                                            if (t == 2) { dateout = dateout + " 20" + it; }

                                            t++;
                                        }
                                    }
                                }
                            }

                            if (noSpacelist.Count == 3) // line doesn't contian production # before date
                            {
                                if (it.Trim() != "")
                                {
                                    if (dateout == "") { dateout = it; }
                                    else
                                    {
                                        if (t == 0) { dateout = dateout + " " + it; }
                                        if (t == 1) { dateout = dateout + " " + it; }
                                        if (t == 2) { dateout = dateout + " 20" + it; }

                                        t++;
                                    }
                                }
                            }



                            i++;
                        }

                        airDate = ahk.ToDateTime(dateout);

                        //AirDate = ahk._regexAirDate.Match(noSpaces);
                    }
                    else
                    {
                        airDate = ahk.ToDateTime(AirDate.ToString());
                    }

                    //_Database.SQLite lite = new _Database.SQLite();
                    //SQLiteConnection dbConnection = lite._ConnectToDB(DBFile); // connect to SQLite DB file path - returns connection data
                    //lite._sqlite("INSERT into [SHOWS] (ShowName, EpName, EpNumber, ProdNumber, AirDate, LocalPath) values ('" + ShowName + "', '" + EpisodeName + "', '" + EpNum + "', '" + ProductionNumber + "', '" + AirDate + "', '" + LocalPath + "')", dbConnection);  // insert into a Table
                    epi.EpisodeName = EpisodeName.ToString();


                    epi.EpisodeName = ahk.StringReplace(epi.EpisodeName, "&amp;", "&");
                    epi.EpisodeName = ahk.StringReplace(epi.EpisodeName, "&quot;", "'");
                    eps.EpisodeName = epi.EpisodeName;

                    epi.EpNum = EpNum;
                    epi.ProductionNumber = ProductionNumber.ToString();

                    eps.EpNum = EpNum;
                    eps.ProductionNum = ProductionNumber.ToString();
                    eps.EpGuideID = episode.EpGuideID;

                    epi.AirDate = airDate.ToString();
                    eps.AirDate = airDate.ToString();

                    //string line = ShowName + "', '" + EpisodeName + "', '" + EpNum + "', '" + ProductionNumber + "', '" + AirDate + "', '" + LocalPath;
                    //ahk.MsgBox(line);


                    EpGuides_UpdateInsert(eps, eps.SQLiteDbPath);

                    //litmEpisodes_UpdateInsert(eps);

                    //episode_UpdateInsert(epi);
                }

                return eps;
            }




            // extract info from html --- needs to update with regex 
            public void EpGuideHTML_ExtractTags(string EpGuideHTML)   // 
            {

                MessageBox.Show("Reading EpGuides HTML Dir - Updating ShowTags in DB");

                //===============================================================================
                // loop through parse directory - import all episode guides into SQLite db
                //===============================================================================

                string[] files = Directory.GetFiles(EpGuidesHTMLDir, "*.txt", System.IO.SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    //=======================================
                    // Single File Parse
                    //=======================================

                    var InTextVar = ahk.FileRead(file);

                    System.IO.FileInfo fileinfo = new System.IO.FileInfo(file); //retrieve info about each file
                    string FileName = fileinfo.Name.ToString();
                    string FileExt = fileinfo.Extension.ToString();

                    string ShowName = FileName.Replace(FileExt, "");

                    string InText = InTextVar.ToString();

                    string[] words4 = InText.Split('\n');
                    foreach (string line in words4)
                    {
                        if (line.Contains("<meta name=\"keywords\""))
                        {

                            string output = line.Replace("<meta name=\"keywords\" content=\"", "");
                            string output2 = output.Replace("\" />", "");
                            string output3 = output2.Replace(", television, series, show, episode guide", "");
                            string output4 = output3.Replace(",", "|");
                            string output5 = output4.Replace(" ", "");
                            //string output4 = output4.Replace("episode guide", "");


                            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;  // convert tag line text to CamelCase
                            string tagline = textInfo.ToTitleCase(output5); //War And Peace

                            MessageBox.Show(ShowName + " = " + tagline);

                            //return tagline; 

                            //====================================================
                            // Update TV Database TAG Entries for each Show
                            //====================================================
                            //_Database.SQLite lite = new _Database.SQLite();
                            //SQLiteConnection dbConnection = lite.ConnectToDB(txtOutFile.Text); // connect to SQLite DB file path - returns connection data
                            //lite.SQLite("Update SHOWS set ShowTags = '" + tagline + "' where ShowName = '" + ShowName + "'", dbConnection);  // insert into a Table

                        }

                    }

                    //return ""; // no tags found
                }


                MessageBox.Show("Done Importing to DB");
                //return ""; 

            }
            public void EpGuideHTML_ExtractShowDates(string EpGuidesHTMLDir)   // 
            {

                MessageBox.Show("Reading EpGuides HTML Dir - Updating ShowStillAiring in DB");


                //===============================================================================
                // loop through parse directory - import all episode guides into SQLite db
                //===============================================================================

                string[] files = Directory.GetFiles(EpGuidesHTMLDir, "*.txt", System.IO.SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    //=======================================
                    // Single File Parse
                    //=======================================

                    var InTextVar = ahk.FileRead(file);

                    System.IO.FileInfo fileinfo = new System.IO.FileInfo(file); //retrieve info about each file
                    string FileName = fileinfo.Name.ToString();
                    string FileExt = fileinfo.Extension.ToString();

                    string ShowName = FileName.Replace(FileExt, "");

                    ahk.MsgBox(ShowName);

                    string InText = InTextVar.ToString();

                    string[] words4 = InText.Split('\n');
                    foreach (string line in words4)
                    {
                        if (line.Contains("<span class='status'>"))
                        {

                            string output = line.Replace("<td>", "");
                            string output2 = output.Replace("</td>", "");
                            string output3 = output2.Replace("<em>", "");
                            string output4 = output3.Replace("</em>", "");

                            string output5 = output4.Replace("<span class='status'>", "|");
                            string output6 = output5.Replace("</span>", "");

                            string[] words = output6.Split('|');
                            string ShowStatusParse = "";
                            foreach (string word in words)
                            {
                                ShowStatusParse = word;   // show status is the 2nd in the split string
                            }


                            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;  // convert tag line text to CamelCase
                            string ShowStatus = textInfo.ToTitleCase(ShowStatusParse); //War And Peace

                            MessageBox.Show(ShowName + " = " + ShowStatus);

                            //====================================================
                            // Update TV Database TAG Entries for each Show
                            //====================================================
                            //_Database.SQLite lite = new _Database.SQLite();
                            //SQLiteConnection dbConnection = lite.ConnectToDB(txtOutFile.Text); // connect to SQLite DB file path - returns connection data
                            //lite.SQLite("Update SHOWS set ShowStillAiring = '" + ShowStatus + "' where ShowName = '" + ShowName + "'", dbConnection);  // insert into a Table

                        }

                    }


                }


                MessageBox.Show("Done Importing to DB");


            }


            public void SQLite_Delete_Show(string ShowName)  // delete first to update show entry
            {
                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection dbConnection = lite._ConnectToDB(SQLiteMediaDb); // connect to SQLite DB file path - returns connection data
                lite._sqlite("Delete from SHOWS where ShowName = '" + ShowName + "'", dbConnection);  // insert into a Table
            }

            public bool Is_Show_In_DB(string ShowName, string DB)  // returns true/false if show found in db
            {
                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite._ConnectToDB(DB); // connect to SQLite DB file path - returns connection data

                //=======================================================================================================
                // Connect to SQLite DB - Request Data from DB and Update GUI Fields
                //=======================================================================================================

                SQLiteDataReader reader = lite.ReturnSQLite("select * from SHOWS WHERE ShowName = '" + ShowName + "'", m_dbConnection);

                bool FoundShow = false;

                while (reader.Read())    // loop through each row returned from select 
                {
                    FoundShow = true;
                }

                return FoundShow;
            }


            #region === BackFill Db ===

            public void BackFill_ShowIndex_IMDbLinks()
            {
                List<string> values = lst.SQL_To_List(show_Conn(), "select EpGuideURL from " + LITMShowIndexTable + " where IMDbUrl = '' and EpGuideURL != ''");

                int i = 1;
                foreach (string value in values)
                {
                    StatusBar("Link BackFill: " + i + "/" + values.Count());

                    string imdbID = sql.SQL_Return_Value(show_Conn(), "select IMDbID FROM " + EpGuideShowIndexTable + " where EpGuideID = '" + value.Trim() + "'");

                    if (imdbID.Trim() != "")
                    {
                        bool updated = sql.WriteDataRecord(show_Conn(), "Update " + LITMShowIndexTable + " set IMDbUrl = '" + imdbID + "' WHERE EpGuideURL = '" + value.Trim() + "'");
                    }

                    i++;
                }


                values = lst.SQL_To_List(show_Conn(), "select IMDbURL from " + LITMShowIndexTable + " where IMDbUrl != '' and EpGuideURL = ''");

                i = 1;
                foreach (string value in values)
                {
                    StatusBar("Link BackFill: " + i + "/" + values.Count());

                    string epGuideID = sql.SQL_Return_Value(show_Conn(), "select EpGuideID FROM " + EpGuideShowIndexTable + " where IMDbID = '" + value + "'");

                    if (epGuideID.Trim() != "")
                    {
                        bool updated = sql.WriteDataRecord(show_Conn(), "Update " + LITMShowIndexTable + " set EpGuideURL = '" + epGuideID + "' WHERE IMDbUrl = '" + value.Trim() + "'");
                    }

                    i++;
                }


                StatusBar("FINISHED IMDb Link BackFill");
            }




            #endregion


            #region === Ep Guides ===

            string EpGuidesHTMLDir = "";
            string LocalTVShowFileList = "";
            string SQLiteMediaDb = "";

            //string SearchDir = @"\\WDMYCLOUDEX4\Public\[ TV ]\30 Rock";
            //string SearchEpNum = "S01E03";

            // Convert EpGuides Formatting to S00E00 Format

            public string Convert_EpGuideEpNum_To_S00E00_Format(string EpGuideEpNum)
            {
                //string EpGuideEpNum = "1-3";

                Match epSeasonNum = _regexSeasonNum.Match(EpGuideEpNum);
                Match epEpNum = _regexEpNum.Match(EpGuideEpNum);

                string epEpNumFixed = epEpNum.ToString();
                if (epEpNumFixed.Length == 2)
                {
                    epEpNumFixed = "E" + epEpNumFixed;
                }
                if (epEpNumFixed.Length == 1)
                {
                    epEpNumFixed = "E0" + epEpNumFixed;
                }

                string epSeasonNumFixed = epSeasonNum.ToString();
                if (epSeasonNumFixed.Length == 2)
                {
                    epSeasonNumFixed = "S" + epSeasonNumFixed;
                }
                if (epSeasonNumFixed.Length == 1)
                {
                    epSeasonNumFixed = "S0" + epSeasonNumFixed;
                }

                string ConvertedEpNum = epSeasonNumFixed + epEpNumFixed;

                //MsgBox(ConvertedEpNum);

                return ConvertedEpNum;
            }



            //================================================================================================
            // Search Show Folder for path to specific episode by S01E03 format - Returns File Path If Found
            //================================================================================================

            public string FindLocalPath_From_EpNumber(string SearchDir, string EpNumber)  // matches epsiode number to files located on network drive (slow!) 
            {
                string[] files = Directory.GetFiles(SearchDir, "*.*", System.IO.SearchOption.AllDirectories);
                string LocalPath = "";

                foreach (string file in files)
                {
                    string FilePath = file.ToString();

                    string FilePathUpper = FilePath.ToUpper();

                    if (FilePathUpper.Contains(EpNumber))
                    {
                        LocalPath = FilePath;
                        // MsgBox(FilePath);
                        break;
                    }

                }

                return LocalPath;
            }

            public string FindLocalPath_From_LocalFileList(string LocalFileListText, string EpNumber, string SearchDir)  //matches episode numbers to local text file index of files on network
            {
                string[] lines = LocalFileListText.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

                // Read in lines from file.
                foreach (string line in lines)
                {
                    string LocalFilePath = line;
                    string LocalFilePathUpper = LocalFilePath.ToUpper();
                    string SearchDirUpper = SearchDir.ToUpper();
                    string EpNumberUpper = EpNumber.ToUpper();

                    if (EpNumberUpper == "")
                    {
                        continue;
                    }

                    if (LocalFilePathUpper.Contains(SearchDirUpper))
                    {
                        if (LocalFilePathUpper.Contains(EpNumberUpper))
                        {
                            return LocalFilePath;
                        }
                    }
                }

                return "";  // no file match found in local list
            }


            //  EpGuides

            public void SQLite_New_Table(string DBFile, string TableName)
            {
                //========================================================
                // Create New SQLite DB (*Used First-Run*)
                // ========================================================
                if (!File.Exists(DBFile)) // create database file if it doen't exist
                {
                    SQLiteConnection.CreateFile(DBFile);
                }

                //===================================
                // Connect to the DB
                //===================================

                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection dbConnection = lite._ConnectToDB(DBFile); // connect to SQLite DB file path - returns connection data


                bool ReturnValue = lite._DropTable(DBFile, TableName); // drop table from DB if it exists already

                try
                {
                    lite._sqlite("CREATE TABLE [" + TableName + "] (ShowName VARCHAR, EpName VARCHAR, EpNumber VARCHAR, ProdNumber VARCHAR, AirDate VARCHAR, LocalPath VARCHAR)", dbConnection);  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


            }

            public void EpGuide_HTML_TextFileDir_to_DB(string SearchDir, string DBFile)   // take folder of EpGuide pages - import into single DB file
            {
                if (!File.Exists(DBFile)) // create database file if it doesn't exist
                {
                    SQLite_New_Table(DBFile, "Shows");
                }

                var LocalTVShowListText = ahk.FileRead(LocalTVShowFileList);
                //string[] LocalTVShowListText = File.ReadAllLines(LocalTVShowFileList);

                //===============================================================================
                // loop through parse directory - import all episode guides into SQLite db
                //===============================================================================

                string[] files = Directory.GetFiles(SearchDir, "*.txt", System.IO.SearchOption.TopDirectoryOnly);


                string ProcessedDir = SearchDir + "\\Processed";

                if (!Directory.Exists(ProcessedDir))  // create processed dir if it doesn't exist
                {
                    ahk.FileCreateDir(ProcessedDir);
                }


                foreach (string file in files)
                {
                    var filenName = Path.GetFileNameWithoutExtension(file); // parse file name 
                    Parse_HTML_TextFile_To_DB(file, DBFile, LocalTVShowListText.ToString()); //process file
                    ahk.FileMove(file, ProcessedDir + "\\" + filenName.ToString() + ".txt"); // move file to processed
                }


                //MessageBox.Show("Done Importing to DB");

            }

            public void EpGuide_URLDir_to_DB(string SearchDir, string DBFile)   // take folder of EpGuide pages - import into single DB file
            {
                //===============================================================================
                // loop through parse directory - import all episode guides into SQLite db
                //===============================================================================

                string[] files = Directory.GetFiles(SearchDir, "*.url", System.IO.SearchOption.AllDirectories);

                foreach (string file in files)
                {
                    string URLPath = ahk.IniRead(file, "InternetShortcut", "URL");
                    string ShowID = ahk.IniRead(file, "MediaInfo", "ShowID");
                    string DirPath = ahk.IniRead(file, "MediaInfo", "DirPath");
                    string ShowName = ahk.IniRead(file, "MediaInfo", "ShowName");

                    SQLite_Delete_Show(ShowName);  // delete first to update show entry

                    using (WebClient client = new WebClient()) // Download HTML from URL
                    {
                        //client.DownloadFile("http://yoursite.com/page.html", @"C:\localfile.html");

                        // Or you can get the file content without saving it:
                        string htmlCode = client.DownloadString(URLPath);

                        EpGuides_Html_To_Db(htmlCode, ShowName, SQLiteMediaDb, DirPath);

                    }


                }


                MessageBox.Show("Done Importing to DB");

            }




            #region === Show Index ===

            private void btnWriteShowIndex_Click(object sender, EventArgs e)
            {
                UpdateShowIndex_NewThread("T:\\");
            }

            public void DownloadCurrentEpGuide_UpdateSQL(string selectedShowEpGuideURL, string selectedShowName)
            {
                //StatusBar("Updating " + selectedShow.ShowName + " From " + selectedShow.EpGuideURL + "...");
                EpGuides_DownloadParseShow(selectedShowEpGuideURL, selectedShowName);
                //StatusBar("Updated " + selectedShow.ShowName);
            }


            private void btnUpdateAllShows_Click(object sender, EventArgs e)
            {
                UpdateAllShows_NewThread();
            }






            #endregion

            #region === Show RegEx ===


            ////#################################
            ////           RegEx
            ////#################################


            private Regex _regexProductionNumber = new Regex(@"\d{1,3}\b\.\s", RegexOptions.CultureInvariant | RegexOptions.Compiled);

            private Regex _regexYearEpisodeNumber = new Regex(@"\d{4}-\d{1,3}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

            private Regex _regexEpisodeNumber = new Regex(@"\d{1,2}-\d{1,2}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

            private Regex _regexAirDate = new Regex("\\d\\d\\s\\b\\w{3,4}\\b\\s\\d\\d", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

            private Regex _regexEpisodeTitle = new Regex(@"(?<=>)(.*)(?=</a>)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);


            // from 1-3 format, pull the 1 or the 3 for season/episode
            private Regex _regexSeasonNum = new Regex(@"\d{1,2}(?=-)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
            private Regex _regexEpNum = new Regex(@"(?<=-)\d{1,2}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);


            #endregion


            #endregion


            #region === Ep / Show Objects ===

            #region === show Object ===

            public struct show
            {
                public string ID { get; set; }
                public string ShowName { get; set; }
                public string EpGuideURL { get; set; }
                public string IMDbURL { get; set; }
                public string LocalRoot { get; set; }
                public string ShowAirStatus { get; set; }
                public string ShowDLStatus { get; set; }
                public string SeasonCount { get; set; }
                public string DateAdded { get; set; }
                public string DateModified { get; set; }
                public string DirSize { get; set; }
                public string Flag { get; set; }
                public string SQLiteDb { get; set; }
                public string ImagePath { get; set; }
            }
            public show Return_show(string ID = "", string ShowName = "", string EpGuideURL = "", string IMDbURL = "", string LocalRoot = "", string ShowAirStatus = "", string ShowDLStatus = "", string SeasonCount = "", string DateAdded = "", string DateModified = "", string DirSize = "", string Flag = "")
            {
                show obj = new show();
                obj.ID = ID;
                obj.ShowName = ShowName;
                obj.EpGuideURL = EpGuideURL;
                obj.IMDbURL = IMDbURL;
                obj.LocalRoot = LocalRoot;
                obj.ShowAirStatus = ShowAirStatus;
                obj.ShowDLStatus = ShowDLStatus;
                obj.SeasonCount = SeasonCount;
                obj.DateAdded = DateAdded;
                obj.DateModified = DateModified;
                obj.DirSize = DirSize;
                obj.Flag = Flag;

                return obj;
            }

            //  Fix illegal characters before Sql/Sqlite Db Inserts
            public show show_FixChars(show ToFix)
            {
                show Fixed = new show();

                Fixed.ID = ToFix.ID.Replace("'", "''");
                Fixed.ShowName = ToFix.ShowName.Replace("'", "''");
                Fixed.EpGuideURL = ToFix.EpGuideURL.Replace("'", "''");
                Fixed.IMDbURL = ToFix.IMDbURL.Replace("'", "''");
                Fixed.LocalRoot = ToFix.LocalRoot.Replace("'", "''");
                Fixed.ShowAirStatus = ToFix.ShowAirStatus.Replace("'", "''");
                Fixed.ShowDLStatus = ToFix.ShowDLStatus.Replace("'", "''");
                Fixed.SeasonCount = ToFix.SeasonCount.Replace("'", "''");
                Fixed.DateAdded = ToFix.DateAdded.Replace("'", "''");
                Fixed.DateModified = ToFix.DateModified.Replace("'", "''");
                Fixed.DirSize = ToFix.DirSize.Replace("'", "''");
                Fixed.Flag = ToFix.Flag.Replace("'", "''");

                return Fixed;
            }

            // Compare two objects to see if they have identical values
            public bool show_Changed(show OldVal, show NewVal)
            {
                show diff = new show();
                List<string> diffList = new List<string>();
                bool different = false;
                if (OldVal.ID == null) { OldVal.ID = ""; }
                if (NewVal.ID == null) { NewVal.ID = ""; }
                if (OldVal.ID != NewVal.ID) { different = true; }
                if (OldVal.ShowName == null) { OldVal.ShowName = ""; }
                if (NewVal.ShowName == null) { NewVal.ShowName = ""; }
                if (OldVal.ShowName != NewVal.ShowName) { different = true; }
                if (OldVal.EpGuideURL == null) { OldVal.EpGuideURL = ""; }
                if (NewVal.EpGuideURL == null) { NewVal.EpGuideURL = ""; }
                if (OldVal.EpGuideURL != NewVal.EpGuideURL) { different = true; }
                if (OldVal.IMDbURL == null) { OldVal.IMDbURL = ""; }
                if (NewVal.IMDbURL == null) { NewVal.IMDbURL = ""; }
                if (OldVal.IMDbURL != NewVal.IMDbURL) { different = true; }
                if (OldVal.LocalRoot == null) { OldVal.LocalRoot = ""; }
                if (NewVal.LocalRoot == null) { NewVal.LocalRoot = ""; }
                if (OldVal.LocalRoot != NewVal.LocalRoot) { different = true; }
                if (OldVal.ShowAirStatus == null) { OldVal.ShowAirStatus = ""; }
                if (NewVal.ShowAirStatus == null) { NewVal.ShowAirStatus = ""; }
                if (OldVal.ShowAirStatus != NewVal.ShowAirStatus) { different = true; }
                if (OldVal.ShowDLStatus == null) { OldVal.ShowDLStatus = ""; }
                if (NewVal.ShowDLStatus == null) { NewVal.ShowDLStatus = ""; }
                if (OldVal.ShowDLStatus != NewVal.ShowDLStatus) { different = true; }
                if (OldVal.SeasonCount == null) { OldVal.SeasonCount = ""; }
                if (NewVal.SeasonCount == null) { NewVal.SeasonCount = ""; }
                if (OldVal.SeasonCount != NewVal.SeasonCount) { different = true; }
                if (OldVal.DateAdded == null) { OldVal.DateAdded = ""; }
                if (NewVal.DateAdded == null) { NewVal.DateAdded = ""; }
                if (OldVal.DateAdded != NewVal.DateAdded) { different = true; }
                if (OldVal.DateModified == null) { OldVal.DateModified = ""; }
                if (NewVal.DateModified == null) { NewVal.DateModified = ""; }
                if (OldVal.DateModified != NewVal.DateModified) { different = true; }
                if (OldVal.DirSize == null) { OldVal.DirSize = ""; }
                if (NewVal.DirSize == null) { NewVal.DirSize = ""; }
                if (OldVal.DirSize != NewVal.DirSize) { different = true; }
                if (OldVal.Flag == null) { OldVal.Flag = ""; }
                if (NewVal.Flag == null) { NewVal.Flag = ""; }
                if (OldVal.Flag != NewVal.Flag) { different = true; }
                return different;
            }

            // Returns object containing the new values different from the old values in object comparison
            public show show_Diff(show OldVal, show NewVal)
            {
                show diff = new show();
                if (OldVal.ID != NewVal.ID) { diff.ID = NewVal.ID; }
                if (OldVal.ShowName != NewVal.ShowName) { diff.ShowName = NewVal.ShowName; }
                if (OldVal.EpGuideURL != NewVal.EpGuideURL) { diff.EpGuideURL = NewVal.EpGuideURL; }
                if (OldVal.IMDbURL != NewVal.IMDbURL) { diff.IMDbURL = NewVal.IMDbURL; }
                if (OldVal.LocalRoot != NewVal.LocalRoot) { diff.LocalRoot = NewVal.LocalRoot; }
                if (OldVal.ShowAirStatus != NewVal.ShowAirStatus) { diff.ShowAirStatus = NewVal.ShowAirStatus; }
                if (OldVal.ShowDLStatus != NewVal.ShowDLStatus) { diff.ShowDLStatus = NewVal.ShowDLStatus; }
                if (OldVal.SeasonCount != NewVal.SeasonCount) { diff.SeasonCount = NewVal.SeasonCount; }
                if (OldVal.DateAdded != NewVal.DateAdded) { diff.DateAdded = NewVal.DateAdded; }
                if (OldVal.DateModified != NewVal.DateModified) { diff.DateModified = NewVal.DateModified; }
                if (OldVal.DirSize != NewVal.DirSize) { diff.DirSize = NewVal.DirSize; }
                if (OldVal.Flag != NewVal.Flag) { diff.Flag = NewVal.Flag; }
                return diff;
            }

            // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
            public List<string> show_DiffList(show OldVal, show NewVal)
            {
                List<string> diffList = new List<string>();
                if (OldVal.ID != NewVal.ID) { diffList.Add("Changed ID Value From " + OldVal.ID + " To " + NewVal.ID); }
                if (OldVal.ShowName != NewVal.ShowName) { diffList.Add("Changed ShowName Value From " + OldVal.ShowName + " To " + NewVal.ShowName); }
                if (OldVal.EpGuideURL != NewVal.EpGuideURL) { diffList.Add("Changed EpGuideURL Value From " + OldVal.EpGuideURL + " To " + NewVal.EpGuideURL); }
                if (OldVal.IMDbURL != NewVal.IMDbURL) { diffList.Add("Changed IMDbURL Value From " + OldVal.IMDbURL + " To " + NewVal.IMDbURL); }
                if (OldVal.LocalRoot != NewVal.LocalRoot) { diffList.Add("Changed LocalRoot Value From " + OldVal.LocalRoot + " To " + NewVal.LocalRoot); }
                if (OldVal.ShowAirStatus != NewVal.ShowAirStatus) { diffList.Add("Changed ShowAirStatus Value From " + OldVal.ShowAirStatus + " To " + NewVal.ShowAirStatus); }
                if (OldVal.ShowDLStatus != NewVal.ShowDLStatus) { diffList.Add("Changed ShowDLStatus Value From " + OldVal.ShowDLStatus + " To " + NewVal.ShowDLStatus); }
                if (OldVal.SeasonCount != NewVal.SeasonCount) { diffList.Add("Changed SeasonCount Value From " + OldVal.SeasonCount + " To " + NewVal.SeasonCount); }
                if (OldVal.DateAdded != NewVal.DateAdded) { diffList.Add("Changed DateAdded Value From " + OldVal.DateAdded + " To " + NewVal.DateAdded); }
                if (OldVal.DateModified != NewVal.DateModified) { diffList.Add("Changed DateModified Value From " + OldVal.DateModified + " To " + NewVal.DateModified); }
                if (OldVal.DirSize != NewVal.DirSize) { diffList.Add("Changed DirSize Value From " + OldVal.DirSize + " To " + NewVal.DirSize); }
                if (OldVal.Flag != NewVal.Flag) { diffList.Add("Changed Flag Value From " + OldVal.Flag + " To " + NewVal.Flag); }
                return diffList;
            }


            #endregion


            #region === show SQL Functions ===

            public void UpdateAllShows_NewThread()
            {
                Thread zipthread = new Thread(() => UpdateAllShows());
                zipthread.Start();
            }


            int NewShowsAdded = 0;
            public void UpdateAllShows(bool OnlyAddNewShows = true)
            {

                // download epguide .url files from network to rebuild/update show index
                WriteShowIndex();

                //bool OnlyAddNewShows = cbNewOnly.Checked;  // option to only parse / update shows not found in database

                List<show> shows = show_ReturnSQLList("select * FROM " + show_TableName() + " where [EpGuideURL] <> ''");

                ResetProgressBar(progressbar, shows.Count);

                int i = 1;
                int added = 0;
                foreach (show show in shows)
                {
                    StatusBar("Updating " + show.ShowName + " (" + i.ToString() + "/" + shows.Count.ToString() + ")");
                    UpdateProgressBar(progressbar);

                    if (OnlyAddNewShows)
                    {
                        bool showInDb = ShowInDb(show.ShowName);  // check to see if showname found in database
                        if (showInDb)
                        {
                            StatusBar("Already in DB (SKIPPING) " + show.ShowName + " (" + i.ToString() + "/" + shows.Count.ToString() + ")");
                            i++;
                            added++;
                            continue;
                        }  // if found, continue to skip parsing 
                    }

                    StatusBar("Updating " + show.ShowName + " (" + i.ToString() + "/" + shows.Count.ToString() + ")");
                    string html = cr.dlHTML(show.EpGuideURL);  // download html
                    EpGuides_Html_To_SQL(html, show.ShowName);  // parse epguides.com html, write entries to db
                    i++;
                }

                if (OnlyAddNewShows)
                {
                    StatusBar("Finished Adding New Shows to Index\n\nAdded " + added + " Shows");
                    ahk.MsgBox("Finished Adding New Shows to Index\n\nAdded " + added + " Shows");
                }
                else
                {
                    StatusBar("Finished Rebuilding Show Index\n\nAdded " + i + " Shows");
                    ahk.MsgBox("Finished Rebuilding Show Index\n\nAdded " + i + " Shows");
                }


                ResetProgressBar(progressbar, shows.Count);
            }

            // check sql db to see if show name is found
            public bool ShowInDb(string ShowName)
            {
                SqlConnection Conn = show_Conn();
                string SelectLine = "Select top 1 [ShowName] From " + showindex_TableName() + " WHERE ShowName = '" + ShowName + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);

                if (ReturnTable == null) { return false; }
                if (ReturnTable.Rows.Count > 0) { return true; }
                return false;
            }


            public void UpdateShowIndex_NewThread(string indexRoot = "T:\\")
            {
                Thread zipthread = new Thread(() => WriteShowIndex(indexRoot));
                zipthread.Start();
            }

            /// <summary>
            ///  write initial index or update additional shows located in indexRoot Folder
            /// </summary>
            /// <param name="indexRoot"></param>
            public void WriteShowIndex(string indexRoot = "T:\\", bool InsertNewShow = false)
            {
                List<string> dirList = lst.DirList(indexRoot, "*.*", false, true);

                ResetProgressBar(progressbar, dirList.Count);

                int i = 1;
                foreach (string dir in dirList)
                {
                    StatusBar("Updating Show Index | Folder " + i + " / " + dirList.Count);
                    UpdateProgressBar(progressbar);

                    //if (!dir.Contains("8 out of 10 Cats")) { continue; }

                    show show = new show();
                    show.LocalRoot = dir;
                    show.ShowName = lst.Last_List_Item(ahk.StringSplit_List(dir, "\\"));

                    if (show.ShowName.Contains("[")) { continue; }

                    List<string> urlList = lst.FileList(dir, "*.url", false, false, true);

                    foreach (string url in urlList)
                    {
                        string link = ahk.IniRead(url, "InternetShortcut", "URL");  // Read INI Value
                        if (link.Contains("imdb.com"))
                        {
                            if (link.Contains("?"))  // parse out unneeded link info
                            {
                                link = ahk.StringSplit(link, "?", 0);
                            }

                            show.IMDbURL = ParseIMDbID(link);
                        }
                        if (link.Contains("epguides.com"))
                        {
                            show.EpGuideURL = ParseEpGuideID(link);
                        }
                    }

                    // show.DirSize = ahk.DirSize(dir).ToString();

                    bool Updated = false;

                    if (InsertNewShow) { Updated = show_InsertSQL(show); }
                    else { Updated = show_UpdateInsert(show); }

                    if (!Updated) { ahk.MsgBox("Error Update/Inserting " + show.ShowName); }

                    i++;
                }

                StatusBar("Updating Updating Show Index");
                //ahk.MsgBox("Finished Adding " + dirList.Count.ToString() + " Shows To Index");

            }



            /// <summary>
            /// Parses imdb url, extras IMDb Title ID
            /// </summary>
            /// <param name="URL"></param>
            /// <returns></returns>
            public string ParseIMDbID(string URL)
            {
                if (URL == "" || URL == null) { return "0"; }

                // nothing to do, name already matches format
                Regex regex = new Regex(@"tt\d{7}");  // /title/tt0273366/?ref_=fn_tv_tt_1

                Match match = regex.Match(URL);
                if (match.Success)
                {
                    return match.Value;
                }

                Imdb imd = new Imdb();
                return imd.ParseID(URL);
            }

            public string ParseEpGuideID(string URL)
            {
                string epguideID = URL.Replace("http://epguides.com/", "");
                epguideID = epguideID.Replace("http://www.epguides.com/", "");
                epguideID = epguideID.Replace("/", "");
                return epguideID;
            }

            /// <summary>
            /// From Existing URL or EpGuide ID, Return Show's EpGuide HomePage URL
            /// </summary>
            /// <param name="URL_or_ID"></param>
            /// <returns></returns>
            public string EpGuideURL(string URL_or_ID)
            {
                string url = ParseEpGuideID(URL_or_ID);
                return "http://epguides.com/" + url;
            }


            public string ShowNameFromEpGuideID(string EpGuideID)
            {
                string showName = sql.SQL_Return_Value(show_Conn(), "Select EpGuideID from " + show_TableName() + " WHERE [EpGuideURL] = '" + EpGuideID + "'");
                return showName;
            }

            // Return show SQL Connection String
            public SqlConnection show_Conn(string ConnName = "SQLserver|LITMLucidMedia")
            {
                return litmEpisodes_Conn(ConnName);
            }

            // Return show TableName (Full Path)
            public string show_TableName()
            {
                // populate to return full sql table name
                return LITMShowIndexTable;
            }

            // Generate MyShowIndex SQL Table
            public bool Create_MyShowIndex_SQLTable()
            {
                SqlConnection Conn = show_Conn();
                string CreateTableLine = "CREATE TABLE " + show_TableName() + " (";
                CreateTableLine = CreateTableLine + "[ID] [int] IDENTITY(1,1) NOT NULL,";
                CreateTableLine = CreateTableLine + "[ShowName] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[EpGuideURL] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[IMDbURL] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[LocalRoot] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[ShowAirStatus] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[ShowDLStatus] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[SeasonCount] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[DateAdded] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[DateModified] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[DirSize] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Flag] [varchar](max) NOT NULL";
                CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";

                bool created = sql.WriteDataRecord(Conn, CreateTableLine);

                return created;
            }

            public bool show_InsertSQL(show obj)
            {
                SqlConnection Con = show_Conn();
                string SQLLine = "Insert Into " + show_TableName() + "(ShowName, EpGuideURL, IMDbURL, LocalRoot, ShowAirStatus, ShowDLStatus, SeasonCount, DateAdded, DateModified, DirSize, Flag) VALUES (@ShowName, @EpGuideURL, @IMDbURL, @LocalRoot, @ShowAirStatus, @ShowDLStatus, @SeasonCount, @DateAdded, @DateModified, @DirSize, @Flag)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.ShowName == null) { obj.ShowName = ""; }
                if (obj.EpGuideURL == null) { obj.EpGuideURL = ""; }
                if (obj.IMDbURL == null) { obj.IMDbURL = ""; }
                if (obj.LocalRoot == null) { obj.LocalRoot = ""; }
                if (obj.ShowAirStatus == null) { obj.ShowAirStatus = ""; }
                if (obj.ShowDLStatus == null) { obj.ShowDLStatus = ""; }
                if (obj.SeasonCount == null) { obj.SeasonCount = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = ""; }
                if (obj.DateModified == null) { obj.DateModified = ""; }
                if (obj.DirSize == null) { obj.DirSize = ""; }
                if (obj.Flag == null) { obj.Flag = ""; }

                cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                cmd2.Parameters.AddWithValue(@"EpGuideURL", obj.EpGuideURL.ToString());
                cmd2.Parameters.AddWithValue(@"IMDbURL", obj.IMDbURL.ToString());
                cmd2.Parameters.AddWithValue(@"LocalRoot", obj.LocalRoot.ToString());
                cmd2.Parameters.AddWithValue(@"ShowAirStatus", obj.ShowAirStatus.ToString());
                cmd2.Parameters.AddWithValue(@"ShowDLStatus", obj.ShowDLStatus.ToString());
                cmd2.Parameters.AddWithValue(@"SeasonCount", obj.SeasonCount.ToString());
                cmd2.Parameters.AddWithValue(@"DateAdded", DateTime.Now.ToString());
                cmd2.Parameters.AddWithValue(@"DateModified", DateTime.Now.ToString());
                cmd2.Parameters.AddWithValue(@"DirSize", obj.DirSize);
                cmd2.Parameters.AddWithValue(@"Flag", obj.Flag.ToString());
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Con.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool show_UpdateSQL(show obj)
            {
                SqlConnection Conn = show_Conn();
                string SQLLine = "Update " + show_TableName() + " SET EpGuideURL = @EpGuideURL, IMDbURL = @IMDbURL, LocalRoot = @LocalRoot, ShowAirStatus = @ShowAirStatus, ShowDLStatus = @ShowDLStatus, SeasonCount = @SeasonCount, DateAdded = @DateAdded, DateModified = @DateModified, DirSize = @DirSize, Flag = @Flag WHERE ShowName = @ShowName";

                if (obj.IMDbURL == "") { SQLLine = "Update " + show_TableName() + " SET EpGuideURL = @EpGuideURL, IMDbURL = @IMDbURL, LocalRoot = @LocalRoot, ShowAirStatus = @ShowAirStatus, ShowDLStatus = @ShowDLStatus, SeasonCount = @SeasonCount, DateAdded = @DateAdded, DateModified = @DateModified, DirSize = @DirSize, Flag = @Flag WHERE ShowName = @ShowName"; }
                if (obj.EpGuideURL == "") { SQLLine = "Update " + show_TableName() + " SET EpGuideURL = @EpGuideURL, IMDbURL = @IMDbURL, LocalRoot = @LocalRoot, ShowAirStatus = @ShowAirStatus, ShowDLStatus = @ShowDLStatus, SeasonCount = @SeasonCount, DateAdded = @DateAdded, DateModified = @DateModified, DirSize = @DirSize, Flag = @Flag WHERE ShowName = @ShowName"; }

                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.ShowName == null) { obj.ShowName = ""; }
                if (obj.EpGuideURL == null) { obj.EpGuideURL = ""; }
                if (obj.IMDbURL == null) { obj.IMDbURL = ""; }
                if (obj.LocalRoot == null) { obj.LocalRoot = ""; }
                if (obj.ShowAirStatus == null) { obj.ShowAirStatus = ""; }
                if (obj.ShowDLStatus == null) { obj.ShowDLStatus = ""; }
                if (obj.SeasonCount == null) { obj.SeasonCount = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = ""; }
                if (obj.DateModified == null) { obj.DateModified = ""; }
                if (obj.DirSize == null) { obj.DirSize = ""; }
                if (obj.Flag == null) { obj.Flag = ""; }
                cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                cmd2.Parameters.AddWithValue(@"EpGuideURL", obj.EpGuideURL.ToString());
                cmd2.Parameters.AddWithValue(@"IMDbURL", obj.IMDbURL.ToString());
                cmd2.Parameters.AddWithValue(@"LocalRoot", obj.LocalRoot.ToString());
                cmd2.Parameters.AddWithValue(@"ShowAirStatus", obj.ShowAirStatus.ToString());
                cmd2.Parameters.AddWithValue(@"ShowDLStatus", obj.ShowDLStatus.ToString());
                cmd2.Parameters.AddWithValue(@"SeasonCount", obj.SeasonCount.ToString());
                cmd2.Parameters.AddWithValue(@"DateAdded", DateTime.Now.ToString());
                cmd2.Parameters.AddWithValue(@"DateModified", DateTime.Now.ToString());
                cmd2.Parameters.AddWithValue(@"DirSize", obj.DirSize.ToString());
                cmd2.Parameters.AddWithValue(@"Flag", obj.Flag.ToString());
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool show_UpdateInsert(show obj)
            {
                SqlConnection Conn = show_Conn();
                //bool Updated = show_UpdateSQL(obj);  // try to update record first
                //bool Updated = show_UpdateIfPopulated(obj);  // try to update record first

                bool Updated = false;

                if (!Updated) { Updated = show_InsertSQL(obj); }  // if unable to update, insert new record
                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool show_UpdateIfPopulated(show obj, string ID = "")
            {
                SqlConnection Conn = show_Conn();
                string SQLcmd = "Update " + show_TableName() + " SET ";
                //if (obj.ID != null) { SQLcmd = SQLcmd + " ID = @ID,"; }

                if (obj.ShowName != null && obj.ShowName != "") { SQLcmd = SQLcmd + " ShowName = @ShowName,"; }
                if (obj.EpGuideURL != null && obj.EpGuideURL != "") { SQLcmd = SQLcmd + " EpGuideURL = @EpGuideURL,"; }
                if (obj.IMDbURL != null && obj.IMDbURL != "") { SQLcmd = SQLcmd + " IMDbURL = @IMDbURL,"; }
                if (obj.LocalRoot != null && obj.LocalRoot != "") { SQLcmd = SQLcmd + " LocalRoot = @LocalRoot,"; }
                if (obj.ShowAirStatus != null && obj.ShowAirStatus != "") { SQLcmd = SQLcmd + " ShowAirStatus = @ShowAirStatus,"; }
                if (obj.ShowDLStatus != null && obj.ShowDLStatus != "") { SQLcmd = SQLcmd + " ShowDLStatus = @ShowDLStatus,"; }
                if (obj.SeasonCount != null && obj.SeasonCount != "") { SQLcmd = SQLcmd + " SeasonCount = @SeasonCount,"; }
                if (obj.DateAdded != null && obj.DateAdded != "") { SQLcmd = SQLcmd + " DateAdded = @DateAdded,"; }
                if (obj.DateModified != null && obj.DateModified != "") { SQLcmd = SQLcmd + " DateModified = @DateModified,"; }
                if (obj.DirSize != null && obj.DirSize != "") { SQLcmd = SQLcmd + " DirSize = @DirSize,"; }
                if (obj.Flag != null && obj.Flag != "") { SQLcmd = SQLcmd + " Flag = @Flag,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ShowName = @ShowName";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                //if (obj.ID != null) { cmd2.Parameters.AddWithValue(@"ID", obj.ID); }
                if (obj.ShowName != null && obj.ShowName != "") { cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName); }
                if (obj.EpGuideURL != null && obj.EpGuideURL != "") { cmd2.Parameters.AddWithValue(@"EpGuideURL", obj.EpGuideURL); }
                if (obj.IMDbURL != null && obj.IMDbURL != "") { cmd2.Parameters.AddWithValue(@"IMDbURL", obj.IMDbURL); }
                if (obj.LocalRoot != null && obj.LocalRoot != "") { cmd2.Parameters.AddWithValue(@"LocalRoot", obj.LocalRoot); }
                if (obj.ShowAirStatus != null && obj.ShowAirStatus != "") { cmd2.Parameters.AddWithValue(@"ShowAirStatus", obj.ShowAirStatus); }
                if (obj.ShowDLStatus != null && obj.ShowDLStatus != "") { cmd2.Parameters.AddWithValue(@"ShowDLStatus", obj.ShowDLStatus); }
                if (obj.SeasonCount != null && obj.SeasonCount != "") { cmd2.Parameters.AddWithValue(@"SeasonCount", obj.SeasonCount); }
                if (obj.DateAdded != null && obj.DateAdded != "") { cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded); }
                if (obj.DateModified != null && obj.DateModified != "") { cmd2.Parameters.AddWithValue(@"DateModified", obj.DateModified); }
                if (obj.DirSize != null && obj.DirSize != "") { cmd2.Parameters.AddWithValue(@"DirSize", obj.DirSize); }
                if (obj.Flag != null && obj.Flag != "") { cmd2.Parameters.AddWithValue(@"Flag", obj.Flag); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public show show_ReturnSQL(string shoWName = "", string ID = "")
            {
                SqlConnection Conn = show_Conn();
                string SelectLine = "Select [ID],[ShowName],[EpGuideURL],[IMDbURL],[LocalRoot],[ShowAirStatus],[ShowDLStatus],[SeasonCount],[DateAdded],[DateModified],[DirSize],[Flag] From " + show_TableName() + " WHERE ID = '" + ID + "'";

                if (shoWName != "") { SelectLine = "Select [ID],[ShowName],[EpGuideURL],[IMDbURL],[LocalRoot],[ShowAirStatus],[ShowDLStatus],[SeasonCount],[DateAdded],[DateModified],[DirSize],[Flag] From " + show_TableName() + " WHERE [ShowName] = '" + shoWName + "'"; }


                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                show returnObject = new show();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        try
                        {
                            returnObject.ID = ret["ID"].ToString();
                            returnObject.ShowName = ret["ShowName"].ToString();
                            returnObject.EpGuideURL = ret["EpGuideURL"].ToString();
                            returnObject.IMDbURL = ret["IMDbURL"].ToString();
                            returnObject.LocalRoot = ret["LocalRoot"].ToString();
                            returnObject.ShowAirStatus = ret["ShowAirStatus"].ToString();
                            returnObject.ShowDLStatus = ret["ShowDLStatus"].ToString();
                            returnObject.SeasonCount = ret["SeasonCount"].ToString();
                            returnObject.DateAdded = ret["DateAdded"].ToString();
                            returnObject.DateModified = ret["DateModified"].ToString();
                            returnObject.DirSize = ret["DirSize"].ToString();
                            returnObject.Flag = ret["Flag"].ToString();
                        }
                        catch
                        { }

                        return returnObject;
                    }
                }
                return returnObject;
            }

            public show ReturnShow(string showName = "", string EpGuideID = "")
            {
                SqlConnection Conn = show_Conn();
                string SelectLine = "";

                if (showName != "") { SelectLine = "Select [ID],[ShowName],[EpGuideURL],[IMDbURL],[LocalRoot],[ShowAirStatus],[ShowDLStatus],[SeasonCount],[DateAdded],[DateModified],[DirSize],[Flag] From " + show_TableName() + " WHERE ShowName = '" + showName + "'"; }
                if (EpGuideID != "") { SelectLine = "Select [ID],[ShowName],[EpGuideURL],[IMDbURL],[LocalRoot],[ShowAirStatus],[ShowDLStatus],[SeasonCount],[DateAdded],[DateModified],[DirSize],[Flag] From " + show_TableName() + " WHERE EpGuideURL = '" + EpGuideID + "'"; }


                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                show returnObject = new show();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        try
                        {
                            returnObject.ID = ret["ID"].ToString();
                            returnObject.ShowName = ret["ShowName"].ToString();
                            returnObject.EpGuideURL = ret["EpGuideURL"].ToString();
                            returnObject.IMDbURL = ret["IMDbURL"].ToString();
                            returnObject.LocalRoot = ret["LocalRoot"].ToString();
                            returnObject.ShowAirStatus = ret["ShowAirStatus"].ToString();
                            returnObject.ShowDLStatus = ret["ShowDLStatus"].ToString();
                            returnObject.SeasonCount = ret["SeasonCount"].ToString();
                            returnObject.DateAdded = ret["DateAdded"].ToString();
                            returnObject.DateModified = ret["DateModified"].ToString();
                            returnObject.DirSize = ret["DirSize"].ToString();
                            returnObject.Flag = ret["Flag"].ToString();
                        }
                        catch
                        { }

                        return returnObject;
                    }
                }
                return returnObject;
            }

            public List<show> show_ReturnSQLList(string Command)
            {
                SqlConnection Conn = show_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<show> ReturnList = new List<show>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        show returnObject = new show();
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.EpGuideURL = ret["EpGuideURL"].ToString();
                        returnObject.IMDbURL = ret["IMDbURL"].ToString();
                        returnObject.LocalRoot = ret["LocalRoot"].ToString();
                        returnObject.ShowAirStatus = ret["ShowAirStatus"].ToString();
                        returnObject.ShowDLStatus = ret["ShowDLStatus"].ToString();
                        returnObject.SeasonCount = ret["SeasonCount"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.DateModified = ret["DateModified"].ToString();
                        returnObject.DirSize = ret["DirSize"].ToString();
                        returnObject.Flag = ret["Flag"].ToString();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }

            public bool show_UpdateLocalPath_SQL(episode obj, bool NextEpSamePath = false)
            {
                SqlConnection Conn = episode_Conn();

                string SQLLine = "Update " + episode_TableName() + " SET LocalPath = @LocalPath, FileSize = @FileSize, DateAdded = @DateAdded, DateModified = @DateModified WHERE ID = @ID";

                // if set to true, will set the next ID's filepath to the same as the current episode
                if (NextEpSamePath)
                {
                    int nextId = ahk.ToInt(obj.ID); nextId++;
                    SQLLine = "Update " + episode_TableName() + " SET LocalPath = @LocalPath, FileSize = @FileSize, DateAdded = @DateAdded, DateModified = @DateModified WHERE ID = @ID OR ID = '" + nextId + "'";
                }


                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);

                if (obj.ID == null) { obj.ID = ""; }
                if (obj.LocalPath == null) { obj.LocalPath = ""; }
                if (obj.FileSize == null) { obj.FileSize = ""; }
                if (obj.RunTime == null) { obj.RunTime = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = ""; }
                if (obj.DateModified == null) { obj.DateModified = ""; }

                cmd2.Parameters.AddWithValue(@"ID", obj.ID.ToString());
                cmd2.Parameters.AddWithValue(@"LocalPath", obj.LocalPath.ToString());
                cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize.ToString());
                cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded.ToString());
                cmd2.Parameters.AddWithValue(@"DateModified", obj.DateModified.ToString());

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }


            #endregion


            #region === episode Object ===

            public struct episode
            {
                public string ID { get; set; }
                public string ShowName { get; set; }
                public string ProductionNumber { get; set; }
                public string EpNum { get; set; }
                public string EpisodeName { get; set; }
                public string AirDate { get; set; }
                public string LocalPath { get; set; }
                public string FileSize { get; set; }
                public string RunTime { get; set; }
                public string DateAdded { get; set; }
                public string DateModified { get; set; }
                public string SeasonAirStatus { get; set; }
                public string SeasonDLStatus { get; set; }
                public string EpInfoLink { get; set; }
            }
            public episode Return_episode(string ID = "", string ShowName = "", string ProductionNumber = "", string EpNum = "", string EpisodeName = "", string AirDate = "", string LocalPath = "", string FileSize = "", string RunTime = "", string DateAdded = "", string DateModified = "", string SeasonAirStatus = "", string SeasonDLStatus = "", string EpInfoLink = "")
            {
                episode obj = new episode();
                obj.ID = ID;
                obj.ShowName = ShowName;
                obj.ProductionNumber = ProductionNumber;
                obj.EpNum = EpNum;
                obj.EpisodeName = EpisodeName;
                obj.AirDate = AirDate;
                obj.LocalPath = LocalPath;
                obj.FileSize = FileSize;
                obj.RunTime = RunTime;
                obj.DateAdded = DateAdded;
                obj.DateModified = DateModified;
                obj.SeasonAirStatus = SeasonAirStatus;
                obj.SeasonDLStatus = SeasonDLStatus;
                obj.EpInfoLink = EpInfoLink;

                return obj;
            }

            //  Fix illegal characters before Sql/Sqlite Db Inserts
            public episode episode_FixChars(episode ToFix)
            {
                episode Fixed = new episode();

                Fixed.ID = ToFix.ID.Replace("'", "''");
                Fixed.ShowName = ToFix.ShowName.Replace("'", "''");
                Fixed.ProductionNumber = ToFix.ProductionNumber.Replace("'", "''");
                Fixed.EpNum = ToFix.EpNum.Replace("'", "''");
                Fixed.EpisodeName = ToFix.EpisodeName.Replace("'", "''");
                Fixed.AirDate = ToFix.AirDate.Replace("'", "''");
                Fixed.LocalPath = ToFix.LocalPath.Replace("'", "''");
                Fixed.FileSize = ToFix.FileSize.Replace("'", "''");
                Fixed.RunTime = ToFix.RunTime.Replace("'", "''");
                Fixed.DateAdded = ToFix.DateAdded.Replace("'", "''");
                Fixed.DateModified = ToFix.DateModified.Replace("'", "''");
                Fixed.SeasonAirStatus = ToFix.SeasonAirStatus.Replace("'", "''");
                Fixed.SeasonDLStatus = ToFix.SeasonDLStatus.Replace("'", "''");
                Fixed.EpInfoLink = ToFix.EpInfoLink.Replace("'", "''");

                return Fixed;
            }

            // Compare two objects to see if they have identical values
            public bool episode_Changed(episode OldVal, episode NewVal)
            {
                episode diff = new episode();
                List<string> diffList = new List<string>();
                bool different = false;
                if (OldVal.ID == null) { OldVal.ID = ""; }
                if (NewVal.ID == null) { NewVal.ID = ""; }
                if (OldVal.ID != NewVal.ID) { different = true; }
                if (OldVal.ShowName == null) { OldVal.ShowName = ""; }
                if (NewVal.ShowName == null) { NewVal.ShowName = ""; }
                if (OldVal.ShowName != NewVal.ShowName) { different = true; }
                if (OldVal.ProductionNumber == null) { OldVal.ProductionNumber = ""; }
                if (NewVal.ProductionNumber == null) { NewVal.ProductionNumber = ""; }
                if (OldVal.ProductionNumber != NewVal.ProductionNumber) { different = true; }
                if (OldVal.EpNum == null) { OldVal.EpNum = ""; }
                if (NewVal.EpNum == null) { NewVal.EpNum = ""; }
                if (OldVal.EpNum != NewVal.EpNum) { different = true; }
                if (OldVal.EpisodeName == null) { OldVal.EpisodeName = ""; }
                if (NewVal.EpisodeName == null) { NewVal.EpisodeName = ""; }
                if (OldVal.EpisodeName != NewVal.EpisodeName) { different = true; }
                if (OldVal.AirDate == null) { OldVal.AirDate = ""; }
                if (NewVal.AirDate == null) { NewVal.AirDate = ""; }
                if (OldVal.AirDate != NewVal.AirDate) { different = true; }
                if (OldVal.LocalPath == null) { OldVal.LocalPath = ""; }
                if (NewVal.LocalPath == null) { NewVal.LocalPath = ""; }
                if (OldVal.LocalPath != NewVal.LocalPath) { different = true; }
                if (OldVal.FileSize == null) { OldVal.FileSize = ""; }
                if (NewVal.FileSize == null) { NewVal.FileSize = ""; }
                if (OldVal.FileSize != NewVal.FileSize) { different = true; }
                if (OldVal.RunTime == null) { OldVal.RunTime = ""; }
                if (NewVal.RunTime == null) { NewVal.RunTime = ""; }
                if (OldVal.RunTime != NewVal.RunTime) { different = true; }
                if (OldVal.DateAdded == null) { OldVal.DateAdded = ""; }
                if (NewVal.DateAdded == null) { NewVal.DateAdded = ""; }
                if (OldVal.DateAdded != NewVal.DateAdded) { different = true; }
                if (OldVal.DateModified == null) { OldVal.DateModified = ""; }
                if (NewVal.DateModified == null) { NewVal.DateModified = ""; }
                if (OldVal.DateModified != NewVal.DateModified) { different = true; }
                if (OldVal.SeasonAirStatus == null) { OldVal.SeasonAirStatus = ""; }
                if (NewVal.SeasonAirStatus == null) { NewVal.SeasonAirStatus = ""; }
                if (OldVal.SeasonAirStatus != NewVal.SeasonAirStatus) { different = true; }
                if (OldVal.SeasonDLStatus == null) { OldVal.SeasonDLStatus = ""; }
                if (NewVal.SeasonDLStatus == null) { NewVal.SeasonDLStatus = ""; }
                if (OldVal.SeasonDLStatus != NewVal.SeasonDLStatus) { different = true; }
                if (OldVal.EpInfoLink == null) { OldVal.EpInfoLink = ""; }
                if (NewVal.EpInfoLink == null) { NewVal.EpInfoLink = ""; }
                if (OldVal.EpInfoLink != NewVal.EpInfoLink) { different = true; }
                return different;
            }

            // Returns object containing the new values different from the old values in object comparison
            public episode episode_Diff(episode OldVal, episode NewVal)
            {
                episode diff = new episode();
                if (OldVal.ID != NewVal.ID) { diff.ID = NewVal.ID; }
                if (OldVal.ShowName != NewVal.ShowName) { diff.ShowName = NewVal.ShowName; }
                if (OldVal.ProductionNumber != NewVal.ProductionNumber) { diff.ProductionNumber = NewVal.ProductionNumber; }
                if (OldVal.EpNum != NewVal.EpNum) { diff.EpNum = NewVal.EpNum; }
                if (OldVal.EpisodeName != NewVal.EpisodeName) { diff.EpisodeName = NewVal.EpisodeName; }
                if (OldVal.AirDate != NewVal.AirDate) { diff.AirDate = NewVal.AirDate; }
                if (OldVal.LocalPath != NewVal.LocalPath) { diff.LocalPath = NewVal.LocalPath; }
                if (OldVal.FileSize != NewVal.FileSize) { diff.FileSize = NewVal.FileSize; }
                if (OldVal.RunTime != NewVal.RunTime) { diff.RunTime = NewVal.RunTime; }
                if (OldVal.DateAdded != NewVal.DateAdded) { diff.DateAdded = NewVal.DateAdded; }
                if (OldVal.DateModified != NewVal.DateModified) { diff.DateModified = NewVal.DateModified; }
                if (OldVal.SeasonAirStatus != NewVal.SeasonAirStatus) { diff.SeasonAirStatus = NewVal.SeasonAirStatus; }
                if (OldVal.SeasonDLStatus != NewVal.SeasonDLStatus) { diff.SeasonDLStatus = NewVal.SeasonDLStatus; }
                if (OldVal.EpInfoLink != NewVal.EpInfoLink) { diff.EpInfoLink = NewVal.EpInfoLink; }
                return diff;
            }

            // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
            public List<string> episode_DiffList(episode OldVal, episode NewVal)
            {
                List<string> diffList = new List<string>();
                if (OldVal.ID != NewVal.ID) { diffList.Add("Changed ID Value From " + OldVal.ID + " To " + NewVal.ID); }
                if (OldVal.ShowName != NewVal.ShowName) { diffList.Add("Changed ShowName Value From " + OldVal.ShowName + " To " + NewVal.ShowName); }
                if (OldVal.ProductionNumber != NewVal.ProductionNumber) { diffList.Add("Changed ProductionNumber Value From " + OldVal.ProductionNumber + " To " + NewVal.ProductionNumber); }
                if (OldVal.EpNum != NewVal.EpNum) { diffList.Add("Changed EpNum Value From " + OldVal.EpNum + " To " + NewVal.EpNum); }
                if (OldVal.EpisodeName != NewVal.EpisodeName) { diffList.Add("Changed EpisodeName Value From " + OldVal.EpisodeName + " To " + NewVal.EpisodeName); }
                if (OldVal.AirDate != NewVal.AirDate) { diffList.Add("Changed AirDate Value From " + OldVal.AirDate + " To " + NewVal.AirDate); }
                if (OldVal.LocalPath != NewVal.LocalPath) { diffList.Add("Changed LocalPath Value From " + OldVal.LocalPath + " To " + NewVal.LocalPath); }
                if (OldVal.FileSize != NewVal.FileSize) { diffList.Add("Changed FileSize Value From " + OldVal.FileSize + " To " + NewVal.FileSize); }
                if (OldVal.RunTime != NewVal.RunTime) { diffList.Add("Changed RunTime Value From " + OldVal.RunTime + " To " + NewVal.RunTime); }
                if (OldVal.DateAdded != NewVal.DateAdded) { diffList.Add("Changed DateAdded Value From " + OldVal.DateAdded + " To " + NewVal.DateAdded); }
                if (OldVal.DateModified != NewVal.DateModified) { diffList.Add("Changed DateModified Value From " + OldVal.DateModified + " To " + NewVal.DateModified); }
                if (OldVal.SeasonAirStatus != NewVal.SeasonAirStatus) { diffList.Add("Changed SeasonAirStatus Value From " + OldVal.SeasonAirStatus + " To " + NewVal.SeasonAirStatus); }
                if (OldVal.SeasonDLStatus != NewVal.SeasonDLStatus) { diffList.Add("Changed SeasonDLStatus Value From " + OldVal.SeasonDLStatus + " To " + NewVal.SeasonDLStatus); }
                if (OldVal.EpInfoLink != NewVal.EpInfoLink) { diffList.Add("Changed EpInfoLink Value From " + OldVal.EpInfoLink + " To " + NewVal.EpInfoLink); }
                return diffList;
            }


            #endregion

            #region === episode SQL Functions ===

            // search for show, if season num provided, only returns that season
            public List<episode> returnShow(string ShowName, int SeasonNum = -1)
            {
                string Command = "Select * from " + episode_TableName() + " where [ShowName] = '" + ShowName + "'";

                if (SeasonNum != -1)
                {
                    string seasonNum = ahk.AddLeadingZeros(SeasonNum, 2);
                    Command = Command + " and [EpNum] like '%S" + seasonNum + "%' order by [EpNum]";
                }
                else
                {
                    Command = Command + " order by [EpNum]";
                }

                List<episode> returnList = episode_ReturnSQLList(Command);
                return returnList;
            }

            // Return episode SQL Connection String
            public SqlConnection episode_Conn(string ConnName = "SQLserver|LITMLucidMedia")
            {
                return litmEpisodes_Conn(ConnName);
            }


            // Return episode TableName (Full Path)
            public string episode_TableName()
            {
                // populate to return full sql table name
                return LITMEpisodeTable;
            }

            public string showindex_TableName()
            {
                return LITMShowIndexTable;
            }

            // Generate SQL Table
            public bool episode_CreateSQLTable()
            {
                SqlConnection Conn = episode_Conn();
                string CreateTableLine = "CREATE TABLE [LucidMedia].[dbo].[Episodes] (";
                CreateTableLine = CreateTableLine + "[ID] [int] IDENTITY(1,1) NOT NULL,";
                CreateTableLine = CreateTableLine + "[ShowName] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[ProductionNumber] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[EpNum] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[EpisodeName] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[AirDate] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[LocalPath] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[FileSize] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[RunTime] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[DateAdded] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[DateModified] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[SeasonAirStatus] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[SeasonDLStatus] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[EpInfoLink] [varchar](max) NOT NULL";
                CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";

                return true;
            }

            public bool episode_InsertSQL(episode obj)
            {
                SqlConnection Con = episode_Conn();
                string SQLLine = "Insert Into " + episode_TableName() + "(ShowName, ProductionNum, EpNum, EpisodeName, AirDate, LocalPath, FileSize, RunTime, DateAdded, DateModified) VALUES (@ShowName, @ProductionNum, @EpNum, @EpisodeName, @AirDate, @LocalPath, @FileSize, @RunTime, @DateAdded, @DateModified)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.ShowName == null) { obj.ShowName = ""; }
                if (obj.ProductionNumber == null) { obj.ProductionNumber = ""; }
                if (obj.EpNum == null) { obj.EpNum = ""; }
                if (obj.EpisodeName == null) { obj.EpisodeName = ""; }
                if (obj.AirDate == null) { obj.AirDate = ""; }
                if (obj.LocalPath == null) { obj.LocalPath = ""; }
                if (obj.FileSize == null) { obj.FileSize = ""; }
                if (obj.RunTime == null) { obj.RunTime = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = ""; }
                if (obj.DateModified == null) { obj.DateModified = ""; }
                if (obj.SeasonAirStatus == null) { obj.SeasonAirStatus = ""; }
                if (obj.SeasonDLStatus == null) { obj.SeasonDLStatus = ""; }
                if (obj.EpInfoLink == null) { obj.EpInfoLink = ""; }
                cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                cmd2.Parameters.AddWithValue(@"ProductionNum", obj.ProductionNumber.ToString());
                cmd2.Parameters.AddWithValue(@"EpNum", obj.EpNum.ToString());
                cmd2.Parameters.AddWithValue(@"EpisodeName", obj.EpisodeName.ToString());
                cmd2.Parameters.AddWithValue(@"AirDate", obj.AirDate.ToString());
                cmd2.Parameters.AddWithValue(@"LocalPath", obj.LocalPath.ToString());
                cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize.ToString());
                cmd2.Parameters.AddWithValue(@"RunTime", obj.RunTime.ToString());
                cmd2.Parameters.AddWithValue(@"DateAdded", DateTime.Now.ToString());
                cmd2.Parameters.AddWithValue(@"DateModified", DateTime.Now.ToString());
                //cmd2.Parameters.AddWithValue(@"SeasonAirStatus", obj.SeasonAirStatus.ToString());
                //cmd2.Parameters.AddWithValue(@"SeasonDLStatus", obj.SeasonDLStatus.ToString());
                //cmd2.Parameters.AddWithValue(@"EpInfoLink", obj.EpInfoLink.ToString());
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Con.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool episode_UpdateSQL(episode obj, bool ByID = false)
            {
                SqlConnection Conn = episode_Conn();
                string SQLLine = "Update " + episode_TableName() + " SET ShowName = @ShowName, ProductionNumber = @ProductionNumber, EpNum = @EpNum, EpisodeName = @EpisodeName, AirDate = @AirDate, LocalPath = @LocalPath, FileSize = @FileSize, RunTime = @RunTime, DateAdded = @DateAdded, DateModified = @DateModified, SeasonAirStatus = @SeasonAirStatus, SeasonDLStatus = @SeasonDLStatus, EpInfoLink = @EpInfoLink WHERE ID = @ID";

                if (obj.LocalPath == "" || obj.LocalPath == null) { SQLLine = "Update " + episode_TableName() + " SET ShowName = @ShowName, ProductionNumber = @ProductionNumber, EpNum = @EpNum, EpisodeName = @EpisodeName, AirDate = @AirDate, FileSize = @FileSize, RunTime = @RunTime, DateAdded = @DateAdded, DateModified = @DateModified, SeasonAirStatus = @SeasonAirStatus, SeasonDLStatus = @SeasonDLStatus, EpInfoLink = @EpInfoLink WHERE ID = @ID"; }

                // update based on episode #
                if (!ByID)
                {
                    SQLLine = "Update " + episode_TableName() + " SET ShowName = @ShowName, ProductionNumber = @ProductionNumber, EpNum = @EpNum, EpisodeName = @EpisodeName, AirDate = @AirDate, LocalPath = @LocalPath, FileSize = @FileSize, RunTime = @RunTime, DateAdded = @DateAdded, DateModified = @DateModified, SeasonAirStatus = @SeasonAirStatus, SeasonDLStatus = @SeasonDLStatus, EpInfoLink = @EpInfoLink WHERE EpNum = @EpNum";

                    if (obj.LocalPath == "" || obj.LocalPath == null) { SQLLine = "Update " + episode_TableName() + " SET ShowName = @ShowName, ProductionNumber = @ProductionNumber, EpNum = @EpNum, EpisodeName = @EpisodeName, AirDate = @AirDate, FileSize = @FileSize, RunTime = @RunTime, DateAdded = @DateAdded, DateModified = @DateModified, SeasonAirStatus = @SeasonAirStatus, SeasonDLStatus = @SeasonDLStatus, EpInfoLink = @EpInfoLink WHERE ID = @ID"; }
                }

                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.ID == null) { obj.ID = ""; }
                if (obj.ShowName == null) { obj.ShowName = ""; }
                if (obj.ProductionNumber == null) { obj.ProductionNumber = ""; }
                if (obj.EpNum == null) { obj.EpNum = ""; }
                if (obj.EpisodeName == null) { obj.EpisodeName = ""; }
                if (obj.AirDate == null) { obj.AirDate = ""; }
                if (obj.LocalPath == null) { obj.LocalPath = ""; }
                if (obj.FileSize == null) { obj.FileSize = ""; }
                if (obj.RunTime == null) { obj.RunTime = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = ""; }
                if (obj.DateModified == null) { obj.DateModified = ""; }
                if (obj.SeasonAirStatus == null) { obj.SeasonAirStatus = ""; }
                if (obj.SeasonDLStatus == null) { obj.SeasonDLStatus = ""; }
                if (obj.EpInfoLink == null) { obj.EpInfoLink = ""; }
                cmd2.Parameters.AddWithValue(@"ID", obj.ID.ToString());
                cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                cmd2.Parameters.AddWithValue(@"ProductionNumber", obj.ProductionNumber.ToString());
                cmd2.Parameters.AddWithValue(@"EpNum", obj.EpNum.ToString());
                cmd2.Parameters.AddWithValue(@"EpisodeName", obj.EpisodeName.ToString());
                cmd2.Parameters.AddWithValue(@"AirDate", obj.AirDate.ToString());

                if (obj.LocalPath != "" || obj.LocalPath != null) { cmd2.Parameters.AddWithValue(@"LocalPath", obj.LocalPath.ToString()); }

                cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize.ToString());
                cmd2.Parameters.AddWithValue(@"RunTime", obj.RunTime.ToString());
                cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded.ToString());
                cmd2.Parameters.AddWithValue(@"DateModified", obj.DateModified.ToString());
                cmd2.Parameters.AddWithValue(@"SeasonAirStatus", obj.SeasonAirStatus.ToString());
                cmd2.Parameters.AddWithValue(@"SeasonDLStatus", obj.SeasonDLStatus.ToString());
                cmd2.Parameters.AddWithValue(@"EpInfoLink", obj.EpInfoLink.ToString());
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }


            // update episode object (doesn't include info from local file path/size)
            public bool episode_EpGuideUpdateSQL(episode obj)
            {
                SqlConnection Conn = episode_Conn();
                string SQLLine = "Update " + episode_TableName() + " SET ShowName = @ShowName, ProductionNum = @ProductionNum, EpNum = @EpNum, EpisodeName = @EpisodeName, AirDate = @AirDate, DateModified = @DateModified WHERE EpNum = @EpNum AND ShowName = @ShowName";

                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.ID == null) { obj.ID = ""; }
                if (obj.ShowName == null) { obj.ShowName = ""; }
                if (obj.ProductionNumber == null) { obj.ProductionNumber = ""; }
                if (obj.EpNum == null) { obj.EpNum = ""; }
                if (obj.EpisodeName == null) { obj.EpisodeName = ""; }
                if (obj.AirDate == null) { obj.AirDate = ""; }
                if (obj.LocalPath == null) { obj.LocalPath = ""; }
                if (obj.FileSize == null) { obj.FileSize = ""; }
                if (obj.RunTime == null) { obj.RunTime = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = ""; }
                if (obj.DateModified == null) { obj.DateModified = ""; }
                if (obj.SeasonAirStatus == null) { obj.SeasonAirStatus = ""; }
                if (obj.SeasonDLStatus == null) { obj.SeasonDLStatus = ""; }
                if (obj.EpInfoLink == null) { obj.EpInfoLink = ""; }

                //cmd2.Parameters.AddWithValue(@"ID", obj.ID.ToString());
                cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                cmd2.Parameters.AddWithValue(@"ProductionNum", obj.ProductionNumber.ToString());
                cmd2.Parameters.AddWithValue(@"EpNum", obj.EpNum.ToString());
                cmd2.Parameters.AddWithValue(@"EpisodeName", obj.EpisodeName.ToString());
                cmd2.Parameters.AddWithValue(@"AirDate", obj.AirDate.ToString());
                cmd2.Parameters.AddWithValue(@"DateModified", DateTime.Now.ToString());

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }


            // update season status 
            public bool episode_UpdateSeasonStatus()
            {

                //string seasonStatus = ddlSeasonStatus.Text;
                //string showAirStatus = ddlAirStatus.Text;
                //string showDLStatus = ddlDLStatus.Text;


                ////// store selected items as global vars
                ////episode selectedEp = new episode();
                ////show selectedShow = new show();
                ////List<episode> selectedSeason = new List<episode>();
                ////List<string> localFiles = new List<string>();
                ////int selectedSeasonNum = 1;
                ////string selectedFolder = "";

                //SqlConnection Conn = episode_Conn();

                //string curSeason = ahk.AddLeadingZeros(selectedSeasonNum, 2);
                //string SQLLine = "Update [LucidMedia].[dbo].[Episodes] SET SeasonAirStatus = @SeasonAirStatus, SeasonDLStatus = @SeasonDLStatus, DateModified = @DateModified where [ShowName] = '" + selectedShow.ShowName + "' and EpNum like '%S" + curSeason + "%'";

                //SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                //cmd2 = new SqlCommand(SQLLine, Conn);

                //cmd2.Parameters.AddWithValue(@"DateModified", DateTime.Now.ToString());
                //cmd2.Parameters.AddWithValue(@"SeasonAirStatus", seasonStatus);
                //cmd2.Parameters.AddWithValue(@"SeasonDLStatus", ddlDLStatus.Text);

                //if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                //int recordsAffected = 0;
                //try { recordsAffected = cmd2.ExecuteNonQuery(); }
                //catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                //Conn.Close();
                //if (recordsAffected > 0) { return true; }
                //else return false;

                return false;
            }

            // update show download status
            public bool show_UpdateShowStatus()
            {
                //SqlConnection Conn = show_Conn();
                //string SQLLine = "Update [LucidMedia].[dbo].[ShowIndex] SET LocalRoot = @LocalRoot, IMDbURL = @IMDbURL, EpGuideURL = @EpGuideURL, ShowAirStatus = @ShowAirStatus, ShowDLStatus = @ShowDLStatus, DateModified = @DateModified WHERE [ShowName] = '" + selectedShow.ShowName + "'";
                //SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                //cmd2 = new SqlCommand(SQLLine, Conn);

                //cmd2.Parameters.AddWithValue(@"ShowAirStatus", ddlAirStatus.Text);
                //cmd2.Parameters.AddWithValue(@"ShowDLStatus", ddlDLStatus.Text);
                //cmd2.Parameters.AddWithValue(@"DateModified", DateTime.Now.ToString());

                //cmd2.Parameters.AddWithValue(@"LocalRoot", txtLocalRoot.Text);
                //cmd2.Parameters.AddWithValue(@"IMDbURL", txtImdbLink.Text);
                //cmd2.Parameters.AddWithValue(@"EpGuideURL", txtEpGuideURL.Text);

                //if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                //int recordsAffected = 0;
                //try { recordsAffected = cmd2.ExecuteNonQuery(); }
                //catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                //Conn.Close();
                //if (recordsAffected > 0) { return true; }
                //else return false;

                return false;
            }


            public bool episode_UpdateInsert(episode obj)
            {
                SqlConnection Conn = episode_Conn();
                //bool Updated = episode_UpdateSQL(obj);  // try to update record first

                bool Updated = episode_EpGuideUpdateSQL(obj);  // try to update record first (doesn't include local file info)

                if (!Updated) { Updated = episode_InsertSQL(obj); }  // if unable to update, insert new record
                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool episode_UpdateIfPopulated(episode obj)
            {
                SqlConnection Conn = episode_Conn();
                string SQLcmd = "Update " + episode_TableName() + " SET ";
                if (obj.ShowName != null) { SQLcmd = SQLcmd + " ShowName = @ShowName,"; }
                if (obj.ProductionNumber != null) { SQLcmd = SQLcmd + " ProductionNumber = @ProductionNumber,"; }
                if (obj.EpNum != null) { SQLcmd = SQLcmd + " EpNum = @EpNum,"; }
                if (obj.EpisodeName != null) { SQLcmd = SQLcmd + " EpisodeName = @EpisodeName,"; }
                if (obj.AirDate != null) { SQLcmd = SQLcmd + " AirDate = @AirDate,"; }
                if (obj.LocalPath != null) { SQLcmd = SQLcmd + " LocalPath = @LocalPath,"; }
                if (obj.FileSize != null) { SQLcmd = SQLcmd + " FileSize = @FileSize,"; }
                if (obj.RunTime != null) { SQLcmd = SQLcmd + " RunTime = @RunTime,"; }
                if (obj.DateAdded != null) { SQLcmd = SQLcmd + " DateAdded = @DateAdded,"; }
                if (obj.DateModified != null) { SQLcmd = SQLcmd + " DateModified = @DateModified,"; }
                if (obj.SeasonAirStatus != null) { SQLcmd = SQLcmd + " SeasonAirStatus = @SeasonAirStatus,"; }
                if (obj.SeasonDLStatus != null) { SQLcmd = SQLcmd + " SeasonDLStatus = @SeasonDLStatus,"; }
                if (obj.EpInfoLink != null) { SQLcmd = SQLcmd + " EpInfoLink = @EpInfoLink,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE EpNum = @EpNum AND ShowName = @ShowName";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.ShowName != null) { cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName); }
                if (obj.ProductionNumber != null) { cmd2.Parameters.AddWithValue(@"ProductionNumber", obj.ProductionNumber); }
                if (obj.EpNum != null) { cmd2.Parameters.AddWithValue(@"EpNum", obj.EpNum); }
                if (obj.EpisodeName != null) { cmd2.Parameters.AddWithValue(@"EpisodeName", obj.EpisodeName); }
                if (obj.AirDate != null) { cmd2.Parameters.AddWithValue(@"AirDate", obj.AirDate); }
                if (obj.LocalPath != null) { cmd2.Parameters.AddWithValue(@"LocalPath", obj.LocalPath); }
                if (obj.FileSize != null) { cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize); }
                if (obj.RunTime != null) { cmd2.Parameters.AddWithValue(@"RunTime", obj.RunTime); }
                if (obj.DateAdded != null) { cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded); }
                if (obj.DateModified != null) { cmd2.Parameters.AddWithValue(@"DateModified", obj.DateModified); }
                if (obj.SeasonAirStatus != null) { cmd2.Parameters.AddWithValue(@"SeasonAirStatus", obj.SeasonAirStatus); }
                if (obj.SeasonDLStatus != null) { cmd2.Parameters.AddWithValue(@"SeasonDLStatus", obj.SeasonDLStatus); }
                if (obj.EpInfoLink != null) { cmd2.Parameters.AddWithValue(@"EpInfoLink", obj.EpInfoLink); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public episode episode_ReturnSQL(string shoWName = "", string ID = "")
            {
                SqlConnection Conn = episode_Conn();
                string SelectLine = "Select [ID],[ShowName],[ProductionNumber],[EpNum],[EpisodeName],[AirDate],[LocalPath],[FileSize],[RunTime],[DateAdded],[DateModified],[SeasonAirStatus],[SeasonDLStatus],[EpInfoLink] From " + episode_TableName() + " WHERE ID = '" + ID + "'";

                if (shoWName != "") { SelectLine = "Select [ID],[ShowName],[ProductionNumber],[EpNum],[EpisodeName],[AirDate],[LocalPath],[FileSize],[RunTime],[DateAdded],[DateModified],[SeasonAirStatus],[SeasonDLStatus],[EpInfoLink] From " + episode_TableName() + " WHERE [ShowName] = '" + shoWName + "'"; }

                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                episode returnObject = new episode();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.ProductionNumber = ret["ProductionNumber"].ToString();
                        returnObject.EpNum = ret["EpNum"].ToString();
                        returnObject.EpisodeName = ret["EpisodeName"].ToString();
                        returnObject.AirDate = ret["AirDate"].ToString();
                        returnObject.LocalPath = ret["LocalPath"].ToString();
                        returnObject.FileSize = ret["FileSize"].ToString();
                        returnObject.RunTime = ret["RunTime"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.DateModified = ret["DateModified"].ToString();
                        returnObject.SeasonAirStatus = ret["SeasonAirStatus"].ToString();
                        returnObject.SeasonDLStatus = ret["SeasonDLStatus"].ToString();
                        returnObject.EpInfoLink = ret["EpInfoLink"].ToString();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public List<episode> episode_ReturnSQLList(string Command)
            {
                SqlConnection Conn = episode_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<episode> ReturnList = new List<episode>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        episode returnObject = new episode();

                        try
                        {
                            returnObject.ID = ret["ID"].ToString();
                            returnObject.ShowName = ret["ShowName"].ToString();
                            returnObject.ProductionNumber = ret["ProductionNumber"].ToString();
                            returnObject.EpNum = ret["EpNum"].ToString();
                            returnObject.EpisodeName = ret["EpisodeName"].ToString();
                            returnObject.AirDate = ret["AirDate"].ToString();
                            returnObject.LocalPath = ret["LocalPath"].ToString();
                            returnObject.FileSize = ret["FileSize"].ToString();
                            returnObject.RunTime = ret["RunTime"].ToString();
                            returnObject.DateAdded = ret["DateAdded"].ToString();
                            returnObject.DateModified = ret["DateModified"].ToString();
                            returnObject.SeasonAirStatus = ret["SeasonAirStatus"].ToString();
                            returnObject.SeasonDLStatus = ret["SeasonDLStatus"].ToString();
                            returnObject.EpInfoLink = ret["EpInfoLink"].ToString();
                        }
                        catch
                        { }


                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }



            #endregion

            // newest db calls 
            #region === litmEpisodes FUNCTIONS ===

            #region ===== litmEpisodes Object =====

            public struct litmEpisodes
            {
                public string ID { get; set; }
                public string EpGuideID { get; set; }
                public string IMDbID { get; set; }
                public string ShowName { get; set; }
                public string ProductionNum { get; set; }
                public string EpNum { get; set; }
                public string EpisodeName { get; set; }
                public string AirDate { get; set; }
                public string LocalPath { get; set; }
                public string FileSize { get; set; }
                public string FileSizeBytes { get; set; }
                public string RunTime { get; set; }
                public string DateAdded { get; set; }
                public string DateModified { get; set; }
                public string Links { get; set; }
                public string ShowImage { get; set; }
                public string Flag { get; set; }


                public string SQLiteDbPath { get; set; }
            }
            public litmEpisodes Return_litmEpisodes(string ID = "", string EpGuideID = "", string IMDbID = "", string ShowName = "", string ProductionNum = "", string EpNum = "", string EpisodeName = "", string AirDate = "", string LocalPath = "", string FileSize = "", string FileSizeBytes = "", string RunTime = "", string DateAdded = "", string DateModified = "", string Links = "", string ShowImage = "", string Flag = "")
            {
                litmEpisodes obj = new litmEpisodes();
                obj.ID = ID;
                obj.EpGuideID = EpGuideID;
                obj.IMDbID = IMDbID;
                obj.ShowName = ShowName;
                obj.ProductionNum = ProductionNum;
                obj.EpNum = EpNum;
                obj.EpisodeName = EpisodeName;
                obj.AirDate = AirDate;
                obj.LocalPath = LocalPath;
                obj.FileSize = FileSize;
                obj.FileSizeBytes = FileSizeBytes;
                obj.RunTime = RunTime;
                obj.DateAdded = DateAdded;
                obj.DateModified = DateModified;
                obj.Links = Links;
                obj.ShowImage = ShowImage;
                obj.Flag = Flag;

                return obj;
            }

            //  Fix illegal characters before Sql/Sqlite Db Inserts
            public litmEpisodes litmEpisodes_FixChars(litmEpisodes ToFix)
            {
                litmEpisodes Fixed = new litmEpisodes();

                Fixed.ID = ToFix.ID.Replace("'", "''");
                Fixed.EpGuideID = ToFix.EpGuideID.Replace("'", "''");
                Fixed.IMDbID = ToFix.IMDbID.Replace("'", "''");
                Fixed.ShowName = ToFix.ShowName.Replace("'", "''");
                Fixed.ProductionNum = ToFix.ProductionNum.Replace("'", "''");
                Fixed.EpNum = ToFix.EpNum.Replace("'", "''");
                Fixed.EpisodeName = ToFix.EpisodeName.Replace("'", "''");
                Fixed.AirDate = ToFix.AirDate.Replace("'", "''");
                Fixed.LocalPath = ToFix.LocalPath.Replace("'", "''");
                Fixed.FileSize = ToFix.FileSize.Replace("'", "''");
                Fixed.FileSizeBytes = ToFix.FileSizeBytes.Replace("'", "''");
                Fixed.RunTime = ToFix.RunTime.Replace("'", "''");
                Fixed.DateAdded = ToFix.DateAdded.Replace("'", "''");
                Fixed.DateModified = ToFix.DateModified.Replace("'", "''");
                Fixed.Links = ToFix.Links.Replace("'", "''");
                Fixed.ShowImage = ToFix.ShowImage.Replace("'", "''");
                Fixed.Flag = ToFix.Flag.Replace("'", "''");

                return Fixed;
            }

            // Compare two objects to see if they have identical values
            public bool litmEpisodes_Changed(litmEpisodes OldVal, litmEpisodes NewVal)
            {
                litmEpisodes diff = new litmEpisodes();
                List<string> diffList = new List<string>();
                bool different = false;
                if (OldVal.ID == null) { OldVal.ID = ""; }
                if (NewVal.ID == null) { NewVal.ID = ""; }
                if (OldVal.ID != NewVal.ID) { different = true; }
                if (OldVal.EpGuideID == null) { OldVal.EpGuideID = ""; }
                if (NewVal.EpGuideID == null) { NewVal.EpGuideID = ""; }
                if (OldVal.EpGuideID != NewVal.EpGuideID) { different = true; }
                if (OldVal.IMDbID == null) { OldVal.IMDbID = ""; }
                if (NewVal.IMDbID == null) { NewVal.IMDbID = ""; }
                if (OldVal.IMDbID != NewVal.IMDbID) { different = true; }
                if (OldVal.ShowName == null) { OldVal.ShowName = ""; }
                if (NewVal.ShowName == null) { NewVal.ShowName = ""; }
                if (OldVal.ShowName != NewVal.ShowName) { different = true; }
                if (OldVal.ProductionNum == null) { OldVal.ProductionNum = ""; }
                if (NewVal.ProductionNum == null) { NewVal.ProductionNum = ""; }
                if (OldVal.ProductionNum != NewVal.ProductionNum) { different = true; }
                if (OldVal.EpNum == null) { OldVal.EpNum = ""; }
                if (NewVal.EpNum == null) { NewVal.EpNum = ""; }
                if (OldVal.EpNum != NewVal.EpNum) { different = true; }
                if (OldVal.EpisodeName == null) { OldVal.EpisodeName = ""; }
                if (NewVal.EpisodeName == null) { NewVal.EpisodeName = ""; }
                if (OldVal.EpisodeName != NewVal.EpisodeName) { different = true; }
                if (OldVal.AirDate == null) { OldVal.AirDate = ""; }
                if (NewVal.AirDate == null) { NewVal.AirDate = ""; }
                if (OldVal.AirDate != NewVal.AirDate) { different = true; }
                if (OldVal.LocalPath == null) { OldVal.LocalPath = ""; }
                if (NewVal.LocalPath == null) { NewVal.LocalPath = ""; }
                if (OldVal.LocalPath != NewVal.LocalPath) { different = true; }
                if (OldVal.FileSize == null) { OldVal.FileSize = ""; }
                if (NewVal.FileSize == null) { NewVal.FileSize = ""; }
                if (OldVal.FileSize != NewVal.FileSize) { different = true; }
                if (OldVal.FileSizeBytes == null) { OldVal.FileSizeBytes = ""; }
                if (NewVal.FileSizeBytes == null) { NewVal.FileSizeBytes = ""; }
                if (OldVal.FileSizeBytes != NewVal.FileSizeBytes) { different = true; }
                if (OldVal.RunTime == null) { OldVal.RunTime = ""; }
                if (NewVal.RunTime == null) { NewVal.RunTime = ""; }
                if (OldVal.RunTime != NewVal.RunTime) { different = true; }
                if (OldVal.DateAdded == null) { OldVal.DateAdded = ""; }
                if (NewVal.DateAdded == null) { NewVal.DateAdded = ""; }
                if (OldVal.DateAdded != NewVal.DateAdded) { different = true; }
                if (OldVal.DateModified == null) { OldVal.DateModified = ""; }
                if (NewVal.DateModified == null) { NewVal.DateModified = ""; }
                if (OldVal.DateModified != NewVal.DateModified) { different = true; }
                if (OldVal.Links == null) { OldVal.Links = ""; }
                if (NewVal.Links == null) { NewVal.Links = ""; }
                if (OldVal.Links != NewVal.Links) { different = true; }
                if (OldVal.ShowImage == null) { OldVal.ShowImage = ""; }
                if (NewVal.ShowImage == null) { NewVal.ShowImage = ""; }
                if (OldVal.ShowImage != NewVal.ShowImage) { different = true; }
                if (OldVal.Flag == null) { OldVal.Flag = ""; }
                if (NewVal.Flag == null) { NewVal.Flag = ""; }
                if (OldVal.Flag != NewVal.Flag) { different = true; }
                return different;
            }

            // Returns object containing the new values different from the old values in object comparison
            public litmEpisodes litmEpisodes_Diff(litmEpisodes OldVal, litmEpisodes NewVal)
            {
                litmEpisodes diff = new litmEpisodes();
                if (OldVal.ID != NewVal.ID) { diff.ID = NewVal.ID; }
                if (OldVal.EpGuideID != NewVal.EpGuideID) { diff.EpGuideID = NewVal.EpGuideID; }
                if (OldVal.IMDbID != NewVal.IMDbID) { diff.IMDbID = NewVal.IMDbID; }
                if (OldVal.ShowName != NewVal.ShowName) { diff.ShowName = NewVal.ShowName; }
                if (OldVal.ProductionNum != NewVal.ProductionNum) { diff.ProductionNum = NewVal.ProductionNum; }
                if (OldVal.EpNum != NewVal.EpNum) { diff.EpNum = NewVal.EpNum; }
                if (OldVal.EpisodeName != NewVal.EpisodeName) { diff.EpisodeName = NewVal.EpisodeName; }
                if (OldVal.AirDate != NewVal.AirDate) { diff.AirDate = NewVal.AirDate; }
                if (OldVal.LocalPath != NewVal.LocalPath) { diff.LocalPath = NewVal.LocalPath; }
                if (OldVal.FileSize != NewVal.FileSize) { diff.FileSize = NewVal.FileSize; }
                if (OldVal.FileSizeBytes != NewVal.FileSizeBytes) { diff.FileSizeBytes = NewVal.FileSizeBytes; }
                if (OldVal.RunTime != NewVal.RunTime) { diff.RunTime = NewVal.RunTime; }
                if (OldVal.DateAdded != NewVal.DateAdded) { diff.DateAdded = NewVal.DateAdded; }
                if (OldVal.DateModified != NewVal.DateModified) { diff.DateModified = NewVal.DateModified; }
                if (OldVal.Links != NewVal.Links) { diff.Links = NewVal.Links; }
                if (OldVal.ShowImage != NewVal.ShowImage) { diff.ShowImage = NewVal.ShowImage; }
                if (OldVal.Flag != NewVal.Flag) { diff.Flag = NewVal.Flag; }
                return diff;
            }

            // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
            public List<string> litmEpisodes_DiffList(litmEpisodes OldVal, litmEpisodes NewVal)
            {
                List<string> diffList = new List<string>();
                if (OldVal.ID != NewVal.ID) { diffList.Add("Changed ID Value From " + OldVal.ID + " To " + NewVal.ID); }
                if (OldVal.EpGuideID != NewVal.EpGuideID) { diffList.Add("Changed EpGuideID Value From " + OldVal.EpGuideID + " To " + NewVal.EpGuideID); }
                if (OldVal.IMDbID != NewVal.IMDbID) { diffList.Add("Changed IMDbID Value From " + OldVal.IMDbID + " To " + NewVal.IMDbID); }
                if (OldVal.ShowName != NewVal.ShowName) { diffList.Add("Changed ShowName Value From " + OldVal.ShowName + " To " + NewVal.ShowName); }
                if (OldVal.ProductionNum != NewVal.ProductionNum) { diffList.Add("Changed ProductionNum Value From " + OldVal.ProductionNum + " To " + NewVal.ProductionNum); }
                if (OldVal.EpNum != NewVal.EpNum) { diffList.Add("Changed EpNum Value From " + OldVal.EpNum + " To " + NewVal.EpNum); }
                if (OldVal.EpisodeName != NewVal.EpisodeName) { diffList.Add("Changed EpisodeName Value From " + OldVal.EpisodeName + " To " + NewVal.EpisodeName); }
                if (OldVal.AirDate != NewVal.AirDate) { diffList.Add("Changed AirDate Value From " + OldVal.AirDate + " To " + NewVal.AirDate); }
                if (OldVal.LocalPath != NewVal.LocalPath) { diffList.Add("Changed LocalPath Value From " + OldVal.LocalPath + " To " + NewVal.LocalPath); }
                if (OldVal.FileSize != NewVal.FileSize) { diffList.Add("Changed FileSize Value From " + OldVal.FileSize + " To " + NewVal.FileSize); }
                if (OldVal.FileSizeBytes != NewVal.FileSizeBytes) { diffList.Add("Changed FileSizeBytes Value From " + OldVal.FileSizeBytes + " To " + NewVal.FileSizeBytes); }
                if (OldVal.RunTime != NewVal.RunTime) { diffList.Add("Changed RunTime Value From " + OldVal.RunTime + " To " + NewVal.RunTime); }
                if (OldVal.DateAdded != NewVal.DateAdded) { diffList.Add("Changed DateAdded Value From " + OldVal.DateAdded + " To " + NewVal.DateAdded); }
                if (OldVal.DateModified != NewVal.DateModified) { diffList.Add("Changed DateModified Value From " + OldVal.DateModified + " To " + NewVal.DateModified); }
                if (OldVal.Links != NewVal.Links) { diffList.Add("Changed Links Value From " + OldVal.Links + " To " + NewVal.Links); }
                if (OldVal.ShowImage != NewVal.ShowImage) { diffList.Add("Changed ShowImage Value From " + OldVal.ShowImage + " To " + NewVal.ShowImage); }
                if (OldVal.Flag != NewVal.Flag) { diffList.Add("Changed Flag Value From " + OldVal.Flag + " To " + NewVal.Flag); }
                return diffList;
            }


            #endregion
            #region ===== litmEpisodes SQLite : Return =====

            public litmEpisodes Return_Object_From_LITM_Episodes(string DbFile, string WhereClause = "[ID] = ''")
            {
                string SelectLine = "Select [ID], [EpGuideID], [IMDbID], [ShowName], [ProductionNum], [EpNum], [EpisodeName], [AirDate], [LocalPath], [FileSize], [FileSizeBytes], [RunTime], [DateAdded], [DateModified], [Links], [ShowImage], [Flag] From " + LITMEpisodeTable;
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
                litmEpisodes returnObject = new litmEpisodes();
                int i = 0;
                string Value = "";
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.EpGuideID = ret["EpGuideID"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.ProductionNum = ret["ProductionNum"].ToString();
                        returnObject.EpNum = ret["EpNum"].ToString();
                        returnObject.EpisodeName = ret["EpisodeName"].ToString();
                        returnObject.AirDate = ret["AirDate"].ToString();
                        returnObject.LocalPath = ret["LocalPath"].ToString();
                        returnObject.FileSize = ret["FileSize"].ToString();
                        returnObject.FileSizeBytes = ret["FileSizeBytes"].ToString();
                        returnObject.RunTime = ret["RunTime"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.DateModified = ret["DateModified"].ToString();
                        returnObject.Links = ret["Links"].ToString();
                        returnObject.ShowImage = ret["ShowImage"].ToString();
                        returnObject.Flag = ret["Flag"].ToString();
                    }
                }

                return returnObject;
            }

            public List<litmEpisodes> Return_litmEpisodes_List(string DbFile, string TableName = "", string WhereClause = "")
            {
                if (TableName == "") { TableName = LITMEpisodeTable; }

                string SelectLine = "Select * From " + TableName;

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

                List<litmEpisodes> ReturnList = new List<litmEpisodes>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        litmEpisodes returnObject = new litmEpisodes();

                        returnObject.ID = ret["ID"].ToString();
                        returnObject.EpGuideID = ret["EpGuideID"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.ProductionNum = ret["ProductionNum"].ToString();
                        returnObject.EpNum = ret["EpNum"].ToString();
                        returnObject.EpisodeName = ret["EpisodeName"].ToString();
                        returnObject.AirDate = ret["AirDate"].ToString();
                        returnObject.LocalPath = ret["LocalPath"].ToString();
                        returnObject.FileSize = ret["FileSize"].ToString();
                        returnObject.FileSizeBytes = ret["FileSizeBytes"].ToString();
                        returnObject.RunTime = ret["RunTime"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.DateModified = ret["DateModified"].ToString();
                        returnObject.Links = ret["Links"].ToString();
                        returnObject.ShowImage = ret["ShowImage"].ToString();
                        returnObject.Flag = ret["Flag"].ToString();

                        ReturnList.Add(returnObject);
                    }
                }

                return ReturnList;
            }

            public DataTable Return_DataTable_From_LITM_Episodes(string DbFile)
            {
                string SelectLine = "Select [ID], [EpGuideID], [IMDbID], [ShowName], [ProductionNum], [EpNum], [EpisodeName], [AirDate], [LocalPath], [FileSize], [FileSizeBytes], [RunTime], [DateAdded], [DateModified], [Links], [ShowImage], [Flag] From " + LITMEpisodeTable;
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                return ReturnTable;
            }


            #endregion
            #region ===== litmEpisodes SQLite : Update Insert =====

            public bool litmEpisodes_SQLite_Insert(litmEpisodes inObject)
            {
                if (inObject.SQLiteDbPath == "" || inObject.SQLiteDbPath == null) { inObject.SQLiteDbPath = ahk.AppDir() + "\\" + inObject.ShowName + ".sqlite"; }

                string InsertLine = "Insert Into [Episodes] (ID, EpGuideID, IMDbID, ShowName, ProductionNum, EpNum, EpisodeName, AirDate, LocalPath, FileSize, FileSizeBytes, RunTime, DateAdded, DateModified, Links, ShowImage, Flag) values ('" + inObject.ID + "', '" + inObject.EpGuideID + "', '" + inObject.IMDbID + "', '" + inObject.ShowName + "', '" + inObject.ProductionNum + "', '" + inObject.EpNum + "', '" + inObject.EpisodeName + "', '" + inObject.AirDate + "', '" + inObject.LocalPath + "', '" + inObject.FileSize + "', '" + inObject.FileSizeBytes + "', '" + inObject.RunTime + "', '" + inObject.DateAdded + "', '" + inObject.DateModified + "', '" + inObject.Links + "', '" + inObject.ShowImage + "', '" + inObject.Flag + "')";
                bool Inserted = sqlite.Execute(inObject.SQLiteDbPath, InsertLine);
                if (!Inserted) { ahk.MsgBox("Inserted Into [Episodes] = " + Inserted.ToString()); }
                return Inserted;
            }

            public bool litmEpisodes_SQLite_Update(litmEpisodes inObject)
            {
                if (inObject.SQLiteDbPath == "" || inObject.SQLiteDbPath == null) { inObject.SQLiteDbPath = ahk.AppDir() + "\\" + inObject.ShowName + ".sqlite"; }

                //string UpdateLine = "Update [LITM_Episodes] set ID = '" + inObject.ID + "', EpGuideID = '" + inObject.EpGuideID + "', IMDbID = '" + inObject.IMDbID + "', ShowName = '" + inObject.ShowName + "', ProductionNum = '" + inObject.ProductionNum + "', EpNum = '" + inObject.EpNum + "', EpisodeName = '" + inObject.EpisodeName + "', AirDate = '" + inObject.AirDate + "', LocalPath = '" + inObject.LocalPath + "', FileSize = '" + inObject.FileSize + "', FileSizeBytes = '" + inObject.FileSizeBytes + "', RunTime = '" + inObject.RunTime + "', DateAdded = '" + inObject.DateAdded + "', DateModified = '" + inObject.DateModified + "', Links = '" + inObject.Links + "', ShowImage = '" + inObject.ShowImage + "', Flag = '" + inObject.Flag + "' WHERE [Item] = 'Value' ";
                string UpdateLine = "Update [Episodes] set ";


                if (inObject.ID != null) { UpdateLine = UpdateLine + "[ID] = '" + inObject.ID + "',"; }
                if (inObject.EpGuideID != null) { UpdateLine = UpdateLine + "[EpGuideID] = '" + inObject.EpGuideID + "',"; }
                if (inObject.IMDbID != null) { UpdateLine = UpdateLine + "[IMDbID] = '" + inObject.IMDbID + "',"; }
                if (inObject.ShowName != null) { UpdateLine = UpdateLine + "[ShowName] = '" + inObject.ShowName + "',"; }
                if (inObject.ProductionNum != null) { UpdateLine = UpdateLine + "[ProductionNum] = '" + inObject.ProductionNum + "',"; }
                if (inObject.EpNum != null) { UpdateLine = UpdateLine + "[EpNum] = '" + inObject.EpNum + "',"; }
                if (inObject.EpisodeName != null) { UpdateLine = UpdateLine + "[EpisodeName] = '" + inObject.EpisodeName + "',"; }
                if (inObject.AirDate != null) { UpdateLine = UpdateLine + "[AirDate] = '" + inObject.AirDate + "',"; }
                if (inObject.LocalPath != null) { UpdateLine = UpdateLine + "[LocalPath] = '" + inObject.LocalPath + "',"; }
                if (inObject.FileSize != null) { UpdateLine = UpdateLine + "[FileSize] = '" + inObject.FileSize + "',"; }
                if (inObject.FileSizeBytes != null) { UpdateLine = UpdateLine + "[FileSizeBytes] = '" + inObject.FileSizeBytes + "',"; }
                if (inObject.RunTime != null) { UpdateLine = UpdateLine + "[RunTime] = '" + inObject.RunTime + "',"; }
                if (inObject.DateAdded != null) { UpdateLine = UpdateLine + "[DateAdded] = '" + inObject.DateAdded + "',"; }
                if (inObject.DateModified != null) { UpdateLine = UpdateLine + "[DateModified] = '" + inObject.DateModified + "',"; }
                if (inObject.Links != null) { UpdateLine = UpdateLine + "[Links] = '" + inObject.Links + "',"; }
                if (inObject.ShowImage != null) { UpdateLine = UpdateLine + "[ShowImage] = '" + inObject.ShowImage + "',"; }
                if (inObject.Flag != null) { UpdateLine = UpdateLine + "[Flag] = '" + inObject.Flag + "',"; }

                UpdateLine = ahk.TrimLast(UpdateLine, 1);
                UpdateLine = UpdateLine + " WHERE [EpGuideID] = '" + inObject.EpGuideID + "' AND [EpNum] = '" + inObject.EpNum + "'";

                bool Updated = sqlite.Execute(inObject.SQLiteDbPath, UpdateLine);
                return Updated;
            }

            public bool litmEpisodes_SQLite_UpdateInsert(litmEpisodes obj)  // sqlite insert/update 
            {
                bool Updated = litmEpisodes_SQLite_Update(obj);  // try to update record first
                if (!Updated) { Updated = litmEpisodes_SQLite_Insert(obj); }  // if unable to update, insert new record
                return Updated;
            }


            #endregion
            #region ===== litmEpisodes DataTable =====

            public DataTable Return_litmEpisodes_DataTable(string DbFile, string TableName = "litmEpisodes", string WhereClause = "", bool Debug = false)
            {
                string SelectLine = "Select * From " + LITMEpisodeTable;

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }

                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


                DataTable table = new DataTable();
                table.Columns.Add("ID", typeof(string));
                table.Columns.Add("EpGuideID", typeof(string));
                table.Columns.Add("IMDbID", typeof(string));
                table.Columns.Add("ShowName", typeof(string));
                table.Columns.Add("ProductionNum", typeof(string));
                table.Columns.Add("EpNum", typeof(string));
                table.Columns.Add("EpisodeName", typeof(string));
                table.Columns.Add("AirDate", typeof(string));
                table.Columns.Add("LocalPath", typeof(string));
                table.Columns.Add("FileSize", typeof(string));
                table.Columns.Add("FileSizeBytes", typeof(string));
                table.Columns.Add("RunTime", typeof(string));
                table.Columns.Add("DateAdded", typeof(string));
                table.Columns.Add("DateModified", typeof(string));
                table.Columns.Add("Links", typeof(string));
                table.Columns.Add("ShowImage", typeof(string));
                table.Columns.Add("Flag", typeof(string));

                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        litmEpisodes returnObject = new litmEpisodes();

                        returnObject.ID = ret["ID"].ToString();
                        returnObject.EpGuideID = ret["EpGuideID"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.ProductionNum = ret["ProductionNum"].ToString();
                        returnObject.EpNum = ret["EpNum"].ToString();
                        returnObject.EpisodeName = ret["EpisodeName"].ToString();
                        returnObject.AirDate = ret["AirDate"].ToString();
                        returnObject.LocalPath = ret["LocalPath"].ToString();
                        returnObject.FileSize = ret["FileSize"].ToString();
                        returnObject.FileSizeBytes = ret["FileSizeBytes"].ToString();
                        returnObject.RunTime = ret["RunTime"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.DateModified = ret["DateModified"].ToString();
                        returnObject.Links = ret["Links"].ToString();
                        returnObject.ShowImage = ret["ShowImage"].ToString();
                        returnObject.Flag = ret["Flag"].ToString();

                        table.Rows.Add(returnObject.ID, returnObject.EpGuideID, returnObject.IMDbID, returnObject.ShowName, returnObject.ProductionNum, returnObject.EpNum, returnObject.EpisodeName, returnObject.AirDate, returnObject.LocalPath, returnObject.FileSize, returnObject.FileSizeBytes, returnObject.RunTime, returnObject.DateAdded, returnObject.DateModified, returnObject.Links, returnObject.ShowImage, returnObject.Flag);
                    }
                }

                return table;
            }

            public DataTable Create_LITM_Episodes_DataTable(litmEpisodes inObject)
            {
                DataTable table = new DataTable();
                table.Columns.Add("ID", typeof(string));
                table.Columns.Add("EpGuideID", typeof(string));
                table.Columns.Add("IMDbID", typeof(string));
                table.Columns.Add("ShowName", typeof(string));
                table.Columns.Add("ProductionNum", typeof(string));
                table.Columns.Add("EpNum", typeof(string));
                table.Columns.Add("EpisodeName", typeof(string));
                table.Columns.Add("AirDate", typeof(string));
                table.Columns.Add("LocalPath", typeof(string));
                table.Columns.Add("FileSize", typeof(string));
                table.Columns.Add("FileSizeBytes", typeof(string));
                table.Columns.Add("RunTime", typeof(string));
                table.Columns.Add("DateAdded", typeof(string));
                table.Columns.Add("DateModified", typeof(string));
                table.Columns.Add("Links", typeof(string));
                table.Columns.Add("ShowImage", typeof(string));
                table.Columns.Add("Flag", typeof(string));

                table.Rows.Add(inObject.ID, inObject.EpGuideID, inObject.IMDbID, inObject.ShowName, inObject.ProductionNum, inObject.EpNum, inObject.EpisodeName, inObject.AirDate, inObject.LocalPath, inObject.FileSize, inObject.FileSizeBytes, inObject.RunTime, inObject.DateAdded, inObject.DateModified, inObject.Links, inObject.ShowImage, inObject.Flag);
                return table;
            }


            #endregion
            #region ===== litmEpisodes DataGridView =====

            public void HideShow_LITM_Episodes_Columns(DataGridView dv)
            {

                try { dv.Columns["ID"].Visible = true; } catch { }
                try { dv.Columns["EpGuideID"].Visible = true; } catch { }
                try { dv.Columns["IMDbID"].Visible = true; } catch { }
                try { dv.Columns["ShowName"].Visible = true; } catch { }
                try { dv.Columns["ProductionNum"].Visible = true; } catch { }
                try { dv.Columns["EpNum"].Visible = true; } catch { }
                try { dv.Columns["EpisodeName"].Visible = true; } catch { }
                try { dv.Columns["AirDate"].Visible = true; } catch { }
                try { dv.Columns["LocalPath"].Visible = true; } catch { }
                try { dv.Columns["FileSize"].Visible = true; } catch { }
                try { dv.Columns["FileSizeBytes"].Visible = true; } catch { }
                try { dv.Columns["RunTime"].Visible = true; } catch { }
                try { dv.Columns["DateAdded"].Visible = true; } catch { }
                try { dv.Columns["DateModified"].Visible = true; } catch { }
                try { dv.Columns["Links"].Visible = true; } catch { }
                try { dv.Columns["ShowImage"].Visible = true; } catch { }
                try { dv.Columns["Flag"].Visible = true; } catch { }
            }
            public void Enable_TableName_Columns(DataGridView dv)
            {

                try { dv.Columns["ID"].ReadOnly = true; } catch { }
                try { dv.Columns["EpGuideID"].ReadOnly = true; } catch { }
                try { dv.Columns["IMDbID"].ReadOnly = true; } catch { }
                try { dv.Columns["ShowName"].ReadOnly = true; } catch { }
                try { dv.Columns["ProductionNum"].ReadOnly = true; } catch { }
                try { dv.Columns["EpNum"].ReadOnly = true; } catch { }
                try { dv.Columns["EpisodeName"].ReadOnly = true; } catch { }
                try { dv.Columns["AirDate"].ReadOnly = true; } catch { }
                try { dv.Columns["LocalPath"].ReadOnly = true; } catch { }
                try { dv.Columns["FileSize"].ReadOnly = true; } catch { }
                try { dv.Columns["FileSizeBytes"].ReadOnly = true; } catch { }
                try { dv.Columns["RunTime"].ReadOnly = true; } catch { }
                try { dv.Columns["DateAdded"].ReadOnly = true; } catch { }
                try { dv.Columns["DateModified"].ReadOnly = true; } catch { }
                try { dv.Columns["Links"].ReadOnly = true; } catch { }
                try { dv.Columns["ShowImage"].ReadOnly = true; } catch { }
                try { dv.Columns["Flag"].ReadOnly = true; } catch { }
            }

            #endregion
            #region ===== litmEpisodes SQL Functions =====

            // Return litmEpisodes SQL Connection String
            public SqlConnection litmEpisodes_Conn(string ConnName = "SQLserver|LITMLucidMedia")
            {
                //if (ConnName == "SQLserver|LITMLucidMedia") { ConnName = "SQLserver"; }
                if (ConnName == "SQLserver|LITMLucidMedia") { ConnName = "LITMLucidMedia"; }

                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[ConnName].ConnectionString);

                return conn;
            }

            // Return litmEpisodes TableName (Full Path)
            public string litmEpisodes_TableName()
            {
                // populate to return full sql table name
                return LITMEpisodeTable;
            }

            // Generate SQL Table
            public bool litmEpisodes_CreateSQLTable()
            {
                SqlConnection Conn = litmEpisodes_Conn();
                string CreateTableLine = "CREATE TABLE " + litmEpisodes_TableName() + "(";
                CreateTableLine = CreateTableLine + "[ID] [int] IDENTITY(1,1) NOT NULL,";
                CreateTableLine = CreateTableLine + "[EpGuideID] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[IMDbID] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[ShowName] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[ProductionNum] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[EpNum] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[EpisodeName] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[AirDate] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[LocalPath] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[FileSize] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[FileSizeBytes] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[RunTime] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[DateAdded] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[DateModified] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Links] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[ShowImage] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Flag] [varchar](max) NOT NULL";
                CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                return false;
            }

            public bool litmEpisodes_InsertSQL(litmEpisodes obj)
            {
                SqlConnection Con = litmEpisodes_Conn();
                string SQLLine = "Insert Into " + litmEpisodes_TableName();

                if (obj.ShowImage != null) { SQLLine = SQLLine + " (EpGuideID, IMDbID, ShowName, ProductionNum, EpNum, EpisodeName, AirDate, LocalPath, FileSize, FileSizeBytes, RunTime, DateAdded, DateModified, Links, ShowImage, Flag) VALUES (@EpGuideID, @IMDbID, @ShowName, @ProductionNum, @EpNum, @EpisodeName, @AirDate, @LocalPath, @FileSize, @FileSizeBytes, @RunTime, @DateAdded, @DateModified, @Links, @ShowImage, @Flag)"; }
                if (obj.ShowImage == null) { SQLLine = SQLLine + " (EpGuideID, IMDbID, ShowName, ProductionNum, EpNum, EpisodeName, AirDate, LocalPath, FileSize, FileSizeBytes, RunTime, DateAdded, DateModified, Links, Flag) VALUES (@EpGuideID, @IMDbID, @ShowName, @ProductionNum, @EpNum, @EpisodeName, @AirDate, @LocalPath, @FileSize, @FileSizeBytes, @RunTime, @DateAdded, @DateModified, @Links, @Flag)"; }


                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.EpGuideID == null) { obj.EpGuideID = ""; }
                if (obj.IMDbID == null) { obj.IMDbID = ""; }
                if (obj.ShowName == null) { obj.ShowName = ""; }
                if (obj.ProductionNum == null) { obj.ProductionNum = ""; }
                if (obj.EpNum == null) { obj.EpNum = ""; }
                if (obj.EpisodeName == null) { obj.EpisodeName = ""; }
                if (obj.AirDate == null) { obj.AirDate = ""; }
                if (obj.LocalPath == null) { obj.LocalPath = ""; }
                if (obj.FileSize == null) { obj.FileSize = ""; }
                if (obj.FileSizeBytes == null) { obj.FileSizeBytes = ""; }
                if (obj.RunTime == null) { obj.RunTime = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = ""; }
                if (obj.DateModified == null) { obj.DateModified = ""; }
                if (obj.Links == null) { obj.Links = ""; }
                if (obj.ShowImage == null) { obj.ShowImage = ""; }
                if (obj.Flag == null) { obj.Flag = ""; }

                cmd2.Parameters.AddWithValue(@"EpGuideID", obj.EpGuideID.ToString());
                cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID.ToString());
                cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                cmd2.Parameters.AddWithValue(@"ProductionNum", obj.ProductionNum.ToString());
                cmd2.Parameters.AddWithValue(@"EpNum", obj.EpNum.ToString());
                cmd2.Parameters.AddWithValue(@"EpisodeName", obj.EpisodeName.ToString());
                cmd2.Parameters.AddWithValue(@"AirDate", obj.AirDate.ToString());
                cmd2.Parameters.AddWithValue(@"LocalPath", obj.LocalPath.ToString());
                cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize.ToString());
                cmd2.Parameters.AddWithValue(@"FileSizeBytes", obj.FileSizeBytes.ToString());
                cmd2.Parameters.AddWithValue(@"RunTime", obj.RunTime.ToString());
                cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded.ToString());
                cmd2.Parameters.AddWithValue(@"DateModified", obj.DateModified.ToString());
                cmd2.Parameters.AddWithValue(@"Links", obj.Links.ToString());

                if (obj.ShowImage != null)
                {
                    cmd2.Parameters.AddWithValue(@"ShowImage", obj.ShowImage.ToString());
                }

                cmd2.Parameters.AddWithValue(@"Flag", obj.Flag.ToString());
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox("Error 320: " + ex.ToString()); return false; }
                Con.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }


            public void BATCH_litmEpisodes_InsertSQL(List<litmEpisodes> objs, string ConnName = "SQLserver")
            {
                SqlConnection Con = litmEpisodes_Conn(ConnName);
                insertCount = 0;
                failedInsertCount = 0;

                foreach (litmEpisodes obj in objs)
                {
                    string SQLLine = "Insert Into " + litmEpisodes_TableName();

                    // don't attempt to write binary img

                    //if (obj.ShowImage != null) { SQLLine = SQLLine + " (EpGuideID, IMDbID, ShowName, ProductionNum, EpNum, EpisodeName, AirDate, LocalPath, FileSize, FileSizeBytes, RunTime, DateAdded, DateModified, Links, ShowImage, Flag) VALUES (@EpGuideID, @IMDbID, @ShowName, @ProductionNum, @EpNum, @EpisodeName, @AirDate, @LocalPath, @FileSize, @FileSizeBytes, @RunTime, @DateAdded, @DateModified, @Links, @ShowImage, @Flag)"; }
                    //else
                    //{
                    SQLLine = SQLLine + " (EpGuideID, IMDbID, ShowName, ProductionNum, EpNum, EpisodeName, AirDate, LocalPath, FileSize, FileSizeBytes, RunTime, DateAdded, DateModified, Links, Flag) VALUES (@EpGuideID, @IMDbID, @ShowName, @ProductionNum, @EpNum, @EpisodeName, @AirDate, @LocalPath, @FileSize, @FileSizeBytes, @RunTime, @DateAdded, @DateModified, @Links, @Flag)";
                    //}


                    SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                    cmd2 = new SqlCommand(SQLLine, Con);

                    string now = DateTime.Now.ToString();
                    cmd2.Parameters.AddWithValue(@"EpGuideID", obj.EpGuideID.ToString());
                    cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID.ToString());
                    cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                    cmd2.Parameters.AddWithValue(@"ProductionNum", obj.ProductionNum.ToString());
                    cmd2.Parameters.AddWithValue(@"EpNum", obj.EpNum.ToString());
                    cmd2.Parameters.AddWithValue(@"EpisodeName", obj.EpisodeName.ToString());
                    cmd2.Parameters.AddWithValue(@"AirDate", obj.AirDate.ToString());
                    cmd2.Parameters.AddWithValue(@"LocalPath", obj.LocalPath.ToString());
                    cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize.ToString());
                    cmd2.Parameters.AddWithValue(@"FileSizeBytes", obj.FileSizeBytes.ToString());
                    cmd2.Parameters.AddWithValue(@"RunTime", obj.RunTime.ToString());
                    cmd2.Parameters.AddWithValue(@"DateAdded", now);
                    cmd2.Parameters.AddWithValue(@"DateModified", now);
                    cmd2.Parameters.AddWithValue(@"Links", obj.Links.ToString());

                    //if (obj.ShowImage != null)
                    //{
                    //cmd2.Parameters.AddWithValue(@"ShowImage", "");
                    //}

                    cmd2.Parameters.AddWithValue(@"Flag", obj.Flag.ToString());
                    if (Con.State == ConnectionState.Closed) { Con.Open(); }
                    int recordsAffected = 0;

                    try { recordsAffected = cmd2.ExecuteNonQuery(); }
                    catch (SqlException ex) { ahk.MsgBox("Error 321: " + ex.ToString()); }

                    if (recordsAffected > 0) { insertCount++; }
                    else { failedInsertCount++; }

                    sb.StatusBar("Success = " + insertCount + " | Failed = " + failedInsertCount);
                }

                Con.Close();

                if (failedInsertCount != 0) { ahk.MsgBox("Success = " + insertCount + " | Failed = " + failedInsertCount); }

            }

            public int insertCount = 0;
            public int failedInsertCount = 0;


            public bool litmEpisodes_UpdateSQL(litmEpisodes obj)
            {
                SqlConnection Conn = litmEpisodes_Conn();
                string SQLLine = "Update " + litmEpisodes_TableName();


                if (obj.ShowImage != null) { SQLLine = SQLLine + " SET EpGuideID = @EpGuideID, IMDbID = @IMDbID, ShowName = @ShowName, ProductionNum = @ProductionNum, EpNum = @EpNum, EpisodeName = @EpisodeName, AirDate = @AirDate, LocalPath = @LocalPath, FileSize = @FileSize, FileSizeBytes = @FileSizeBytes, RunTime = @RunTime, DateAdded = @DateAdded, DateModified = @DateModified, Links = @Links, ShowImage = @ShowImage, Flag = @Flag WHERE EpGuideID = @EpGuideID AND EpNum = @EpNum"; }
                if (obj.ShowImage == null) { SQLLine = SQLLine + " SET EpGuideID = @EpGuideID, IMDbID = @IMDbID, ShowName = @ShowName, ProductionNum = @ProductionNum, EpNum = @EpNum, EpisodeName = @EpisodeName, AirDate = @AirDate, LocalPath = @LocalPath, FileSize = @FileSize, FileSizeBytes = @FileSizeBytes, RunTime = @RunTime, DateAdded = @DateAdded, DateModified = @DateModified, Links = @Links, Flag = @Flag WHERE EpGuideID = @EpGuideID AND EpNum = @EpNum"; }


                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.ID == null) { obj.ID = ""; }
                if (obj.EpGuideID == null) { obj.EpGuideID = ""; }
                if (obj.IMDbID == null) { obj.IMDbID = ""; }
                if (obj.ShowName == null) { obj.ShowName = ""; }
                if (obj.ProductionNum == null) { obj.ProductionNum = ""; }
                if (obj.EpNum == null) { obj.EpNum = ""; }
                if (obj.EpisodeName == null) { obj.EpisodeName = ""; }
                if (obj.AirDate == null) { obj.AirDate = ""; }
                if (obj.LocalPath == null) { obj.LocalPath = ""; }
                if (obj.FileSize == null) { obj.FileSize = ""; }
                if (obj.FileSizeBytes == null) { obj.FileSizeBytes = ""; }
                if (obj.RunTime == null) { obj.RunTime = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = ""; }
                if (obj.DateModified == null) { obj.DateModified = ""; }
                if (obj.Links == null) { obj.Links = ""; }
                if (obj.ShowImage == null) { obj.ShowImage = ""; }
                if (obj.Flag == null) { obj.Flag = ""; }
                //cmd2.Parameters.AddWithValue(@"ID", obj.ID.ToString());
                cmd2.Parameters.AddWithValue(@"EpGuideID", obj.EpGuideID.ToString());
                cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID.ToString());
                cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                cmd2.Parameters.AddWithValue(@"ProductionNum", obj.ProductionNum.ToString());
                cmd2.Parameters.AddWithValue(@"EpNum", obj.EpNum.ToString());
                cmd2.Parameters.AddWithValue(@"EpisodeName", obj.EpisodeName.ToString());
                cmd2.Parameters.AddWithValue(@"AirDate", obj.AirDate.ToString());
                cmd2.Parameters.AddWithValue(@"LocalPath", obj.LocalPath.ToString());
                cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize.ToString());
                cmd2.Parameters.AddWithValue(@"FileSizeBytes", obj.FileSizeBytes.ToString());
                cmd2.Parameters.AddWithValue(@"RunTime", obj.RunTime.ToString());
                cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded.ToString());
                cmd2.Parameters.AddWithValue(@"DateModified", obj.DateModified.ToString());
                cmd2.Parameters.AddWithValue(@"Links", obj.Links.ToString());

                if (obj.ShowImage != null)
                {
                    cmd2.Parameters.AddWithValue(@"ShowImage", obj.ShowImage.ToString());
                }
                cmd2.Parameters.AddWithValue(@"Flag", obj.Flag.ToString());
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox("Error 322: " + ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool litmEpisodes_UpdateInsert(litmEpisodes obj, bool SQL = true, bool SQLite = false)
            {
                bool Updated = false;

                SQL = false;
                SQLite = true; // write to sqlite

                if (SQLite)
                {
                    Updated = litmEpisodes_SQLite_UpdateInsert(obj);  // sqlite insert/update 
                }

                if (SQL)
                {
                    SqlConnection Conn = litmEpisodes_Conn();
                    Updated = litmEpisodes_UpdateSQL(obj);  // try to update record first
                    if (!Updated) { Updated = litmEpisodes_InsertSQL(obj); }  // if unable to update, insert new record
                }

                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool litmEpisodes_UpdateIfPopulated(litmEpisodes obj, string ID = "")
            {
                SqlConnection Conn = litmEpisodes_Conn();
                string SQLcmd = "Update " + litmEpisodes_TableName() + " SET ";
                if (obj.ID != null) { SQLcmd = SQLcmd + " ID = @ID,"; }
                if (obj.EpGuideID != null) { SQLcmd = SQLcmd + " EpGuideID = @EpGuideID,"; }
                if (obj.IMDbID != null) { SQLcmd = SQLcmd + " IMDbID = @IMDbID,"; }
                if (obj.ShowName != null) { SQLcmd = SQLcmd + " ShowName = @ShowName,"; }
                if (obj.ProductionNum != null) { SQLcmd = SQLcmd + " ProductionNum = @ProductionNum,"; }
                if (obj.EpNum != null) { SQLcmd = SQLcmd + " EpNum = @EpNum,"; }
                if (obj.EpisodeName != null) { SQLcmd = SQLcmd + " EpisodeName = @EpisodeName,"; }
                if (obj.AirDate != null) { SQLcmd = SQLcmd + " AirDate = @AirDate,"; }
                if (obj.LocalPath != null) { SQLcmd = SQLcmd + " LocalPath = @LocalPath,"; }
                if (obj.FileSize != null) { SQLcmd = SQLcmd + " FileSize = @FileSize,"; }
                if (obj.FileSizeBytes != null) { SQLcmd = SQLcmd + " FileSizeBytes = @FileSizeBytes,"; }
                if (obj.RunTime != null) { SQLcmd = SQLcmd + " RunTime = @RunTime,"; }
                if (obj.DateAdded != null) { SQLcmd = SQLcmd + " DateAdded = @DateAdded,"; }
                if (obj.DateModified != null) { SQLcmd = SQLcmd + " DateModified = @DateModified,"; }
                if (obj.Links != null) { SQLcmd = SQLcmd + " Links = @Links,"; }
                if (obj.ShowImage != null) { SQLcmd = SQLcmd + " ShowImage = @ShowImage,"; }
                if (obj.Flag != null) { SQLcmd = SQLcmd + " Flag = @Flag,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ID = @ID";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.ID != null) { cmd2.Parameters.AddWithValue(@"ID", obj.ID); }
                if (obj.EpGuideID != null) { cmd2.Parameters.AddWithValue(@"EpGuideID", obj.EpGuideID); }
                if (obj.IMDbID != null) { cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID); }
                if (obj.ShowName != null) { cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName); }
                if (obj.ProductionNum != null) { cmd2.Parameters.AddWithValue(@"ProductionNum", obj.ProductionNum); }
                if (obj.EpNum != null) { cmd2.Parameters.AddWithValue(@"EpNum", obj.EpNum); }
                if (obj.EpisodeName != null) { cmd2.Parameters.AddWithValue(@"EpisodeName", obj.EpisodeName); }
                if (obj.AirDate != null) { cmd2.Parameters.AddWithValue(@"AirDate", obj.AirDate); }
                if (obj.LocalPath != null) { cmd2.Parameters.AddWithValue(@"LocalPath", obj.LocalPath); }
                if (obj.FileSize != null) { cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize); }
                if (obj.FileSizeBytes != null) { cmd2.Parameters.AddWithValue(@"FileSizeBytes", obj.FileSizeBytes); }
                if (obj.RunTime != null) { cmd2.Parameters.AddWithValue(@"RunTime", obj.RunTime); }
                if (obj.DateAdded != null) { cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded); }
                if (obj.DateModified != null) { cmd2.Parameters.AddWithValue(@"DateModified", obj.DateModified); }
                if (obj.Links != null) { cmd2.Parameters.AddWithValue(@"Links", obj.Links); }
                if (obj.ShowImage != null) { cmd2.Parameters.AddWithValue(@"ShowImage", obj.ShowImage); }
                if (obj.Flag != null) { cmd2.Parameters.AddWithValue(@"Flag", obj.Flag); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public litmEpisodes litmEpisodes_ReturnSQL(string ID = "")
            {
                SqlConnection Conn = litmEpisodes_Conn();
                string SelectLine = "Select [ID],[EpGuideID],[IMDbID],[ShowName],[ProductionNum],[EpNum],[EpisodeName],[AirDate],[LocalPath],[FileSize],[FileSizeBytes],[RunTime],[DateAdded],[DateModified],[Links],[ShowImage],[Flag] From " + litmEpisodes_TableName() + " WHERE ID = '" + ID + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                litmEpisodes returnObject = new litmEpisodes();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.EpGuideID = ret["EpGuideID"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.ProductionNum = ret["ProductionNum"].ToString();
                        returnObject.EpNum = ret["EpNum"].ToString();
                        returnObject.EpisodeName = ret["EpisodeName"].ToString();
                        returnObject.AirDate = ret["AirDate"].ToString();
                        returnObject.LocalPath = ret["LocalPath"].ToString();
                        returnObject.FileSize = ret["FileSize"].ToString();
                        returnObject.FileSizeBytes = ret["FileSizeBytes"].ToString();
                        returnObject.RunTime = ret["RunTime"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.DateModified = ret["DateModified"].ToString();
                        returnObject.Links = ret["Links"].ToString();
                        returnObject.ShowImage = ret["ShowImage"].ToString();
                        returnObject.Flag = ret["Flag"].ToString();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public List<litmEpisodes> litmEpisodes_ReturnSQLList(string Command = "")
            {
                if (Command == "") { Command = "Select * FROM " + LITMEpisodeTable + " order by ID"; }

                SqlConnection Conn = litmEpisodes_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<litmEpisodes> ReturnList = new List<litmEpisodes>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        litmEpisodes returnObject = new litmEpisodes();
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.EpGuideID = ret["EpGuideID"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.ProductionNum = ret["ProductionNum"].ToString();
                        returnObject.EpNum = ret["EpNum"].ToString();
                        returnObject.EpisodeName = ret["EpisodeName"].ToString();
                        returnObject.AirDate = ret["AirDate"].ToString();
                        returnObject.LocalPath = ret["LocalPath"].ToString();
                        returnObject.FileSize = ret["FileSize"].ToString();
                        returnObject.FileSizeBytes = ret["FileSizeBytes"].ToString();
                        returnObject.RunTime = ret["RunTime"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.DateModified = ret["DateModified"].ToString();
                        returnObject.Links = ret["Links"].ToString();
                        returnObject.ShowImage = ret["ShowImage"].ToString();
                        returnObject.Flag = ret["Flag"].ToString();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }


            #endregion

            #endregion




            #region === EpGuides FUNCTIONS ===

            public bool Create_Table_(string DbFile)
            {
                bool TableExists = sqlite.Table_Exists(DbFile, "EpGuides_Episodes");
                if (!TableExists)
                {
                    string CreateLine = "Create Table [EpGuides_Episodes] (ID INTEGER PRIMARY KEY, EpGuideID VARCHAR, IMDbID VARCHAR, ShowName VARCHAR, ProductionNum VARCHAR, EpNum VARCHAR, EpisodeName VARCHAR, AirDate VARCHAR, LocalPath VARCHAR, FileSize VARCHAR, FileSizeBytes VARCHAR, RunTime VARCHAR, DateAdded VARCHAR, DateModified VARCHAR, Links VARCHAR, ShowImage VARCHAR, Flag VARCHAR)";

                    if (!File.Exists(DbFile))
                    {
                        // Create New SQLite DB (*Used First-Run*)
                        if (!File.Exists(DbFile)) { SQLiteConnection.CreateFile(DbFile); }
                    }

                    TableExists = sqlite.Execute(DbFile, CreateLine);
                }

                return TableExists;
            }


            public bool EpGuides_Insert(litmEpisodes inObject, string DbFile = "")
            {
                if (inObject.IMDbID == null) { inObject.IMDbID = ""; }
                if (inObject.ShowName == null) { inObject.ShowName = ""; }
                if (inObject.ProductionNum == null) { inObject.ProductionNum = ""; }
                if (inObject.EpNum == null) { inObject.EpNum = ""; }
                if (inObject.EpisodeName == null) { inObject.EpisodeName = ""; }
                if (inObject.AirDate == null) { inObject.AirDate = ""; }
                if (inObject.LocalPath == null) { inObject.LocalPath = ""; }
                if (inObject.FileSize == null) { inObject.FileSize = ""; }
                if (inObject.FileSizeBytes == null) { inObject.FileSizeBytes = ""; }
                if (inObject.RunTime == null) { inObject.RunTime = ""; }
                if (inObject.DateAdded == null) { inObject.DateAdded = ""; }
                if (inObject.DateModified == null) { inObject.DateModified = ""; }
                if (inObject.Links == null) { inObject.Links = ""; }
                if (inObject.ShowImage == null) { inObject.ShowImage = ""; }
                if (inObject.Flag == null) { inObject.Flag = ""; }


                inObject.ShowName = inObject.ShowName.Encode();
                inObject.EpisodeName = inObject.EpisodeName.Encode();
                inObject.LocalPath = inObject.LocalPath.Encode();


                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\EpGuides.sqlite"; }
                string InsertLine = "Insert Into [EpGuides_Episodes] (EpGuideID, IMDbID, ShowName, ProductionNum, EpNum, EpisodeName, AirDate, LocalPath, FileSize, FileSizeBytes, RunTime, DateAdded, DateModified, Links, ShowImage, Flag) values ('" + inObject.EpGuideID + "', '" + inObject.IMDbID + "', '" + inObject.ShowName + "', '" + inObject.ProductionNum + "', '" + inObject.EpNum + "', '" + inObject.EpisodeName + "', '" + inObject.AirDate + "', '" + inObject.LocalPath + "', '" + inObject.FileSize + "', '" + inObject.FileSizeBytes + "', '" + inObject.RunTime + "', '" + inObject.DateAdded + "', '" + inObject.DateModified + "', '" + inObject.Links + "', '" + inObject.ShowImage + "', '" + inObject.Flag + "')";
                bool Inserted = sqlite.Execute(DbFile, InsertLine);
                if (!Inserted) { ahk.MsgBox("Inserted Into [] = " + Inserted.ToString()); }
                return Inserted;
            }

            public bool EpGuides_Update(litmEpisodes inObject, string DbFile = "")
            {
                //string UpdateLine = "Update [] set ID = '" + inObject.ID + "', EpGuideID = '" + inObject.EpGuideID + "', IMDbID = '" + inObject.IMDbID + "', ShowName = '" + inObject.ShowName + "', ProductionNum = '" + inObject.ProductionNum + "', EpNum = '" + inObject.EpNum + "', EpisodeName = '" + inObject.EpisodeName + "', AirDate = '" + inObject.AirDate + "', LocalPath = '" + inObject.LocalPath + "', FileSize = '" + inObject.FileSize + "', FileSizeBytes = '" + inObject.FileSizeBytes + "', RunTime = '" + inObject.RunTime + "', DateAdded = '" + inObject.DateAdded + "', DateModified = '" + inObject.DateModified + "', Links = '" + inObject.Links + "', ShowImage = '" + inObject.ShowImage + "', Flag = '" + inObject.Flag + "' WHERE [Item] = 'Value' ";
                string UpdateLine = "Update [EpGuides_Episodes] set ";


                if (inObject.ID != null) { UpdateLine = UpdateLine + "[ID] = '" + inObject.ID + "',"; }
                if (inObject.EpGuideID != null) { UpdateLine = UpdateLine + "[EpGuideID] = '" + inObject.EpGuideID + "',"; }
                if (inObject.IMDbID != null) { UpdateLine = UpdateLine + "[IMDbID] = '" + inObject.IMDbID + "',"; }
                if (inObject.ShowName != null)
                {
                    inObject.ShowName = inObject.ShowName.Encode();
                    UpdateLine = UpdateLine + "[ShowName] = '" + inObject.ShowName + "',";
                }

                if (inObject.ProductionNum != null) { UpdateLine = UpdateLine + "[ProductionNum] = '" + inObject.ProductionNum + "',"; }
                if (inObject.EpNum != null) { UpdateLine = UpdateLine + "[EpNum] = '" + inObject.EpNum + "',"; }
                if (inObject.EpisodeName != null)
                {
                    inObject.EpisodeName = inObject.EpisodeName.Encode();
                    UpdateLine = UpdateLine + "[EpisodeName] = '" + inObject.EpisodeName + "',";
                }
                if (inObject.AirDate != null) { UpdateLine = UpdateLine + "[AirDate] = '" + inObject.AirDate + "',"; }
                if (inObject.LocalPath != null)
                {
                    inObject.LocalPath = inObject.LocalPath.Encode();
                    UpdateLine = UpdateLine + "[LocalPath] = '" + inObject.LocalPath + "',";
                }
                if (inObject.FileSize != null) { UpdateLine = UpdateLine + "[FileSize] = '" + inObject.FileSize + "',"; }
                if (inObject.FileSizeBytes != null) { UpdateLine = UpdateLine + "[FileSizeBytes] = '" + inObject.FileSizeBytes + "',"; }
                if (inObject.RunTime != null) { UpdateLine = UpdateLine + "[RunTime] = '" + inObject.RunTime + "',"; }
                if (inObject.DateAdded != null) { UpdateLine = UpdateLine + "[DateAdded] = '" + inObject.DateAdded + "',"; }
                if (inObject.DateModified != null) { UpdateLine = UpdateLine + "[DateModified] = '" + inObject.DateModified + "',"; }
                if (inObject.Links != null) { UpdateLine = UpdateLine + "[Links] = '" + inObject.Links + "',"; }
                if (inObject.ShowImage != null) { UpdateLine = UpdateLine + "[ShowImage] = '" + inObject.ShowImage + "',"; }
                if (inObject.Flag != null) { UpdateLine = UpdateLine + "[Flag] = '" + inObject.Flag + "',"; }

                UpdateLine = ahk.TrimLast(UpdateLine, 1);
                UpdateLine = UpdateLine + " WHERE [EpNum] = '" + inObject.EpNum + "'"; // DEFINE CONDITION HERE !!!

                bool Updated = sqlite.Execute(DbFile, UpdateLine);
                return Updated;
            }

            public bool EpGuides_UpdateInsert(litmEpisodes obj, string DbFile = "")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\EpGuides.sqlite"; }

                Create_Table_(DbFile);

                bool Updated = EpGuides_Update(obj, DbFile);  // try to update record first
                if (!Updated) { Updated = EpGuides_Insert(obj, DbFile); }  // if unable to update, insert new record
                return Updated;
            }




            #endregion



            #endregion


            #region === Update GUI ===

            public void Update_ListBox(ListBox lb, string NewURL)
            {
                if (lb.InvokeRequired)
                {
                    //lb.BeginInvoke((MethodInvoker)delegate() { lb.Items.Add(NewURL); });  // add to bottom of listbox
                    lb.BeginInvoke((MethodInvoker)delegate () { lb.Items.Insert(0, NewURL); });  // insert at top of listbox
                }
                else
                {
                    lb.Items.Insert(0, NewURL);  // insert at top of listbox
                                                 //lb.Items.Add(NewURL);  // add to bottom of listbox
                }
            }

            public void Update_Current_URL_Grid(DataGridView dv, string NewURL)
            {
                if (dv.InvokeRequired)
                {
                    dv.BeginInvoke((MethodInvoker)delegate () { dv.Rows.Add(NewURL); });
                }
                else
                {
                    dv.Rows.Add(NewURL);
                }
            }

            public void UpdateLabel(Label lbl, string UpdateText) //update label from any thread
            {
                if (lbl.InvokeRequired)
                {
                    lbl.BeginInvoke((MethodInvoker)delegate () { lbl.Text = UpdateText; });
                }
                else
                {
                    lbl.Text = UpdateText; ;
                }
            }

            public void ResetProgressBar(ProgressBar pr, int ProgressMax = 100) // progress bar from any thread
            {
                if (pr.InvokeRequired)
                {
                    pr.BeginInvoke((MethodInvoker)delegate () { pr.Value = 0; pr.Maximum = ProgressMax; });
                }
                else
                {
                    pr.Value = 0; pr.Maximum = ProgressMax;
                }
            }

            public void UpdateProgressBar(ProgressBar pr, int IncreaseBy = 1) // progress bar from any thread
            {
                if (pr.InvokeRequired)
                {
                    pr.BeginInvoke((MethodInvoker)delegate () { pr.Increment(IncreaseBy); });
                }
                else
                {
                    pr.Increment(IncreaseBy);
                }
            }


            #endregion



            public void SB(string Text, int Part = 1)
            {
                sb.StatusBar(Text, Part);
            }

            List<litmEpisodes> showInsert = new List<litmEpisodes>();

            /// <summary>
            /// Process EpGuides.com URL to SQL Database - Overwrites Existing Entry (Deletes First)
            /// </summary>
            /// <param name="NewThread"></param>
            /// <param name="EpGuideURL"></param>
            /// <param name="ShowName"></param>
            public List<litmEpisodes> EpGuideProcess(bool NewThread = true, string epGuideURL = "http://epguides.com/Simpsons/", string ConnName = "SQLserver", string ShowName = "")
            {
                if (NewThread)
                {
                    Thread dirToDbThread = new Thread(() => EpGuideProcess(false, epGuideURL, ConnName, ShowName));
                    dirToDbThread.Start();
                }
                else
                {
                    Added = 0;
                    showInsert = new List<litmEpisodes>();

                    // parse EpGuide ID from EpGuide URL
                    string epGuideID = ParseEpGuideID(epGuideURL);


                    // clear previous show entries to allow for batch insert
                    sql.Write_SQL(litmEpisodes_Conn(ConnName), "delete FROM " + LITMEpisodeTable + " where EpGuideID = '" + epGuideID + "'");

                    // populate ShowName from EpGuideID
                    if (ShowName == "" && epGuideID != "") { ShowName = ShowNameFromEpGuideID(epGuideID); }

                    string EPGuideURL = EpGuideURL(epGuideID);

                    // Download EpGuide Page HTML
                    string html = cr.dlHTML(EPGuideURL);


                    // create new episodes container
                    litmEpisodes ep = new litmEpisodes();

                    ep.ShowName = ShowName;

                    // populate data from db for existing shows
                    show showInfo = ReturnShow(ShowName, epGuideID);

                    ep.LocalPath = showInfo.LocalRoot;
                    ep.EpGuideID = showInfo.EpGuideURL;
                    ep.ShowName = showInfo.ShowName;

                    if (ep.ShowName == null) { ep.ShowName = ShowName; }

                    string[] words4 = html.Split('\n');
                    foreach (string line in words4)
                    {
                        if (line.Contains("episode"))
                        {
                            string lineText = line.Replace("'", "");
                            if (lineText.Contains("id=\"TVHeader\"")) { continue; }
                            if (lineText.Contains("<meta")) { continue; }
                            ep = eParseEpisodeLine_SQL(lineText, ep);
                            continue;
                        }

                        if (line.Contains("show"))
                        {
                            ep.ShowName = showInfo.ShowName;

                            string lineText = line.Replace("'", "");
                            if (lineText.Contains("id=\"TVHeader\"")) { continue; }
                            if (lineText.Contains("<meta")) { continue; }
                            ep = eParseEpisodeLine_SQL(lineText, ep);
                        }
                    }


                    // done assembling list of inserts for show, write now in single batch
                    //List<litmEpisodes>
                    BATCH_litmEpisodes_InsertSQL(showInsert, ConnName);

                    string cmd = "Update " + EpGuideShowIndexTable + " set Flag = '1' where EpGuideID = '" + epGuideID + "'";
                    sql.Write_SQL(litmEpisodes_Conn(ConnName), cmd);

                    //// start next thread
                    //string epID = sql.SQL_Return_Value(litmEpisodes_Conn(ConnName), "select top(1) EpGuideID FROM " + EpGuideShowIndexTable + " where InCollection = '1' and Flag = '0' order by EpGuideID asc");
                    //string remaining = sql.SQL_Return_Value(litmEpisodes_Conn(ConnName), "select count(EpGuideID) FROM " + EpGuideShowIndexTable + " where InCollection = '1' and Flag = '0'");
                    //completedCount++;
                    //sb.StatusBar("Completed " + completedCount + " | " + remaining + " Remaining | Now Processing " + epID, 2);
                    //EpGuideProcess(true, "http://epguides.com/" + epID);
                }

                return showInsert;
            }

            int completedCount = 0;

            public litmEpisodes eParseEpisodeLine_SQL(string ParseLine, litmEpisodes episode)
            {
                episode epi = new episode();
                epi.ShowName = episode.ShowName;

                litmEpisodes eps = new litmEpisodes();
                eps.ShowName = episode.ShowName;
                eps.EpGuideID = episode.EpGuideID;
                eps.LocalPath = episode.LocalPath;


                Match ProductionNumber = _regexProductionNumber.Match(ParseLine);
                Match EpisodeNumber = _regexEpisodeNumber.Match(ParseLine);

                Match YearEpisodeNumber = _regexYearEpisodeNumber.Match(ParseLine);


                // try to match epnumber again if nothing returned
                if (EpisodeNumber.ToString() == "")
                {
                    string noSpaces = ahk.StringReplace(ParseLine, "- ", "-");
                    EpisodeNumber = _regexEpisodeNumber.Match(noSpaces);
                }

                Match AirDate = _regexAirDate.Match(ParseLine);

                Match EpisodeName = _regexEpisodeTitle.Match(ParseLine);
                string LocalPath = "";


                string EpNum = Convert_EpGuideEpNum_To_S00E00_Format(EpisodeNumber.ToString());

                // Exception for Talk Show Formats - need to make dynamica? !!!

                //EpNum = YearEpisodeNumber.ToString();  // overwrite with year-epnum format for talk shows

                if (epi.LocalPath != "")
                {
                    //LocalPath = FindLocalPath_From_EpNumber(DirPath,EpNum); 
                    //LocalPath = FindLocalPath_From_LocalFileList(LocalTVShowListText, EpNum, DirPath);
                    //MsgBox(LocalPath);
                }

                if (ProductionNumber.ToString() != "")
                {
                    DateTime airDate = new DateTime();

                    if (AirDate.ToString() == "")
                    {
                        string noSpaces = ahk.StringReplace(ParseLine, "- ", "-");
                        noSpaces = ahk.StringReplace(noSpaces, ProductionNumber.ToString());
                        noSpaces = ahk.StringReplace(noSpaces, EpisodeNumber.ToString());
                        noSpaces = ahk.StringSplit(noSpaces, "<", 0);
                        noSpaces = noSpaces.Trim();

                        //noSpaces = ahk.StringSplit(noSpaces, " ", 1); 
                        List<string> noSpacelist = ahk.StringSplit_List(noSpaces, " ");
                        int i = 0; int t = 0;
                        string dateout = "";
                        foreach (string it in noSpacelist)
                        {
                            if (noSpacelist.Count == 4) // line contains production # before date
                            {
                                if (i != 0)
                                {
                                    if (it.Trim() != "")
                                    {
                                        if (dateout == "") { dateout = it; }
                                        else
                                        {
                                            if (t == 0) { dateout = dateout + " " + it; }
                                            if (t == 1) { dateout = dateout + " " + it; }
                                            if (t == 2) { dateout = dateout + " 20" + it; }

                                            t++;
                                        }
                                    }
                                }
                            }

                            if (noSpacelist.Count == 3) // line doesn't contian production # before date
                            {
                                if (it.Trim() != "")
                                {
                                    if (dateout == "") { dateout = it; }
                                    else
                                    {
                                        if (t == 0) { dateout = dateout + " " + it; }
                                        if (t == 1) { dateout = dateout + " " + it; }
                                        if (t == 2) { dateout = dateout + " 20" + it; }

                                        t++;
                                    }
                                }
                            }



                            i++;
                        }

                        airDate = ahk.ToDateTime(dateout);

                        //AirDate = ahk._regexAirDate.Match(noSpaces);
                    }
                    else
                    {
                        airDate = ahk.ToDateTime(AirDate.ToString());
                    }

                    //_Database.SQLite lite = new _Database.SQLite();
                    //SQLiteConnection dbConnection = lite._ConnectToDB(DBFile); // connect to SQLite DB file path - returns connection data
                    //lite._sqlite("INSERT into [SHOWS] (ShowName, EpName, EpNumber, ProdNumber, AirDate, LocalPath) values ('" + ShowName + "', '" + EpisodeName + "', '" + EpNum + "', '" + ProductionNumber + "', '" + AirDate + "', '" + LocalPath + "')", dbConnection);  // insert into a Table
                    epi.EpisodeName = EpisodeName.ToString();


                    epi.EpisodeName = ahk.StringReplace(epi.EpisodeName, "&amp;", "&");
                    epi.EpisodeName = ahk.StringReplace(epi.EpisodeName, "&quot;", "'");
                    eps.EpisodeName = epi.EpisodeName;

                    epi.EpNum = EpNum;
                    epi.ProductionNumber = ProductionNumber.ToString();

                    eps.EpNum = EpNum;
                    eps.ProductionNum = ProductionNumber.ToString();
                    eps.EpGuideID = episode.EpGuideID;

                    epi.AirDate = airDate.ToString();
                    eps.AirDate = airDate.ToString();

                    //string line = ShowName + "', '" + EpisodeName + "', '" + EpNum + "', '" + ProductionNumber + "', '" + AirDate + "', '" + LocalPath;
                    //ahk.MsgBox(line);



                    // clear out nulls before sql insert 

                    if (eps.EpGuideID == null) { eps.EpGuideID = ""; }
                    if (eps.IMDbID == null) { eps.IMDbID = ""; }
                    if (eps.ShowName == null) { eps.ShowName = ""; }
                    if (eps.ProductionNum == null) { eps.ProductionNum = ""; }
                    if (eps.EpNum == null) { eps.EpNum = ""; }
                    if (eps.EpisodeName == null) { eps.EpisodeName = ""; }
                    if (eps.AirDate == null) { eps.AirDate = ""; }
                    if (eps.LocalPath == null) { eps.LocalPath = ""; }
                    if (eps.FileSize == null) { eps.FileSize = ""; }
                    if (eps.FileSizeBytes == null) { eps.FileSizeBytes = ""; }
                    if (eps.RunTime == null) { eps.RunTime = ""; }
                    if (eps.DateAdded == null) { eps.DateAdded = ""; }
                    if (eps.DateModified == null) { eps.DateModified = ""; }
                    if (eps.Links == null) { eps.Links = ""; }
                    if (eps.ShowImage == null) { eps.ShowImage = ""; }
                    if (eps.Flag == null) { eps.Flag = ""; }

                    showInsert.Add(eps);
                    //litmEpisodes_UpdateInsert(eps);

                    Added++;
                    SB("Added " + Added.ToString());

                    //episode_UpdateInsert(epi);
                }

                return eps;
            }

            int Added = 0;


        }

    }
}

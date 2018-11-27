using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
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

namespace sharpAHK_Dev
{
    public partial class _Web
    {

        // web.wBrowser = webBrowser1; // define web browser used in winform
        // web.wBrowseURL = ddlURL;    // define dropdown list containing current url

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
        _Database.dir2Db dir2 = new _Database.dir2Db();
        _Parse prs = new _Parse();
        _StatusBar sb = new _StatusBar();

        SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["epGuideDb"].ConnectionString);

        #endregion


        #region === Parse ===

        /// <summary>
        /// Removes Decimal Values from File Size Names (ex: 3.04 MB = 3 MB)
        /// </summary>
        /// <param name="filesize"></param>
        /// <returns></returns>
        public string FileSizeFix(string filesize)
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


        public void ExtractRapidGatorLinks()
        {
            string html = CurrentHTML();
            string links = LinksFromHTML(html, "RapidGator.Net");
            //scintilla1.Text = links;

            sb.StatusBar("Extracted Links");
        }

        /// <summary>
        /// Returns list of Rapidgator links from HTML
        /// </summary>
        /// <param name="html"></param>
        public List<string> Rapidgator_LinkList(string html)
        {
            List<string> links = LinksFromHTML_List(html, "RapidGator.Net");
            List<string> links2 = LinksFromHTML_List(html, "rg.net");

            List<string> returnList = lst.MergeLists(links, links2);


            sb.StatusBar("Extracted " + returnList.Count + " Rapidgator Links");
            return returnList;
        }


        public string LinksFromHTML(string HTML, string searchText = "RapidGator.Net")  // parse current url's html, extract links
        {
            List<string> htmlLines = lst.Text_To_List(HTML, true, true, false);

            string rapidlinks = "";
            foreach (string line in htmlLines)
            {
                if (line.ToUpper().Contains(searchText.ToUpper()))
                {
                    // parse found link, extract only url

                    List<string> segs = ahk.StringSplit_List(line, ">");
                    foreach (string seg in segs)
                    {
                        if (seg.ToUpper().Contains(searchText.ToUpper()))
                        {
                            string lnk = ahk.StringSplit(seg, "<", 0);

                            if (lnk.Contains("[code]"))
                            {
                                lnk = ahk.StringSplit(seg, "[", 0);
                            }

                            lnk = lnk.Replace("&#58;", ":");

                            //bool validLink = RapidLinkValid(lnk);  // check rapidgator file link to see if it is valid
                            //lnk = lnk + " (Valid=" + validLink.ToString() + ")"; 

                            if (rapidlinks != "") { rapidlinks = rapidlinks + " | " + lnk; }
                            if (rapidlinks == "") { rapidlinks = lnk; }
                        }
                    }

                }
            }

            return rapidlinks;
        }

        public List<string> LinksFromHTML_List(string HTML, string searchText = "RapidGator.Net")  // parse current url's html, extract links
        {
            string links = LinksFromHTML(HTML, searchText);
            List<string> linkList = ahk.StringSplit_List(links, "|");

            List<string> returnLinks = new List<string>();
            foreach (string link in linkList)
            {
                string lnk = link.Replace("a href=", "");
                lnk = lnk.Replace("rel=\"nofollow\"", "");
                lnk = lnk.Replace("\"", "");

                returnLinks.Add(lnk);
            }

            return returnLinks;
        }

        /// <summary>
        /// Parse HTML Link Path for File Name for Downloads
        /// </summary>
        /// <param name="htmlLink"></param>
        /// <returns></returns>
        public string LinkFileName(string htmlLink)
        {
            List<string> parts = ahk.StringSplit_List(htmlLink, "/");

            string val = "";
            foreach (string part in parts)
            {
                val = part;
            }

            val = val.Replace(".html", "");
            return val;
        }


        //==== Extract From HTML ============================
        public string LinkFromLine(string HTMLLine, string LinkExt = ".jpg")  // extract link from html line (values separated by ")
        {
            List<string> parts = ahk.StringSplit_List(HTMLLine, "\"");

            foreach (string part in parts)
            {
                if (part.ToUpper().Contains(LinkExt.ToUpper())) { return part; }
            }


            return "";
        }


        // trim out unnecessary html / blank lines from WarezBB forum post -- also populates imdbID if found
        public string ReduceHTMLsize(string html, out string ImdbID)
        {
            //string html = ahk.FileRead("D:\\warezbbPost.txt");
            string cutHtml = StripHTML(html);


            cutHtml = cutHtml.Replace("&#58;", ":");
            cutHtml = cutHtml.Replace("Code: Select all", "");

            // remove blank lines / trim whitespace
            List<string> htmlLines = lst.Text_To_List(cutHtml, true, true, false);

            // convert back to string with line breaks
            string spaceTrimHTML = lst.List_To_String_NewLines(htmlLines);


            // extract imdb id from html
            string imdbID = "";
            foreach (string line in htmlLines)
            {
                bool titleFound = false;
                if (line.Contains("imdb.com/title"))
                {
                    List<string> segs = ahk.StringSplit_List(line, "/");

                    foreach (string seg in segs)
                    {
                        if (seg == "title") { titleFound = true; continue; }

                        if (titleFound) { imdbID = seg; break; }
                    }
                }

                if (titleFound) { break; }
            }

            ImdbID = imdbID; // assign value to output var


            //ahk.FileDelete("D:\\warezbbPost_cut.txt");
            //ahk.FileAppend(spaceTrimHTML, "D:\\warezbbPost_cut.txt");


            return spaceTrimHTML;
        }

        //Strip HTML Tags
        static string StripHTML(string inputString)
        {
            return Regex.Replace(inputString, @"<.*?>", string.Empty);
        }



        // Developed on SceneSource like sites to loop downloading and parsing forum content
        public class Forums
        {
            _Web web = new _Web();
            _Parse.XML xml = new _Parse.XML();
            _AHK ahk = new _AHK();
            _Database.SQL sql = new _Database.SQL();
            _Lists lst = new _Lists();
            _Database.SQLite sqlite = new _Database.SQLite();
            _Sites.RapidGator rg = new _Sites.RapidGator();
            _Parse prs = new _Parse();
            _Images img = new _Images();
            _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();
            _TelerikLib tel = new _TelerikLib();

            public static PictureBox ImageDisplay { get; set; }
            public static RadTextBox BaseURLDisplay { get; set; }
            public static RadTextBox CurrentURLDisplay { get; set; }
            public static RadTextBox IndexPageNumDisplay { get; set; }
            public static RadTextBox CurrentPageNumDisp { get; set; }
            public static RadTextBox GoodCountDisp { get; set; }
            public static RadTextBox GoodTotalDisp { get; set; }
            public static RadTextBox BadCountDisp { get; set; }
            public static RadProgressBar ProgressDisp { get; set; }
            public static bool ForumLoopEnabled { get; set; }
            public static int GoodTotal { get; set; }

            public string StartURLBase { get; set; }
            public string StartStartURLBase { get; set; }
            public int StartPageNum { get; set; }

            public void LoopEnabled(bool Enabled = true)
            {
                ForumLoopEnabled = Enabled;
            }

            public void SetupRapidGator()
            {
                rg.SetPasswords(); // rg passes for linkcheck
            }

            int indexPageNum = 0;
            public string IndexLoop(string BaseURL, int StartNum = 0, int Increment = 50, bool RunIndexLoop = false, string SQLiteDB = "")
            {
                indexPageNum = StartNum;
                bool RunLoop = true;
                do
                {
                    string url = BaseURL.Replace(".html", "");

                    url = url + "-" + indexPageNum + ".html";

                    SetupRapidGator();

                    string html = web.DownloadHTML(url);
                    if (html == "") { ahk.MsgBox("End of Posts Reached - No HTML Returned"); RunLoop = false; LoopEnabled(false); break; }

                    if (html.Contains("Could not obtain post")) { ahk.MsgBox("End of Posts Reached"); RunLoop = false; LoopEnabled(false); break; }

                    int HaveLinksCount = 0;

                    string xmlPath = "//body/div/div/div/div/div";

                    List<string> lines = xml.Parse_HTML_XML(html, xmlPath);  // extract sections of text from html xml

                    List<string> Posts = new List<string>();

                    pro.SetupProgressBar(ProgressDisp, Increment);
                    int k = 1;
                    foreach (string line in lines)
                    {
                        if (line.Contains("topictitle"))
                        {
                            pro.UpdateProgress(ProgressDisp, +k + "/" + Increment + " Links Checked"); k++;

                            string pagenumText = line.Replace("</a>", "|");
                            pagenumText = ahk.StringSplit(pagenumText, "|", 2);
                            pagenumText = ahk.StringSplit(pagenumText, ">", 1);

                            // parse out post name and url
                            List<string> lns = lst.Text_To_List(line, true, true, false);
                            foreach (string ln in lns)
                            {
                                if (ln.Contains("<a href="))
                                {
                                    forumIndex obj = new forumIndex();

                                    obj.PageCount = pagenumText.ToInt();

                                    string pageUrl = ln.Replace("<a href=\"", "");
                                    pageUrl = pageUrl.Replace(".html", ".html|");
                                    pageUrl = pageUrl.Replace("<h2>", "|");
                                    pageUrl = pageUrl.Replace("</h2>", "|");

                                    string PageURL = ahk.StringSplit(pageUrl, "|", 0);
                                    string PageName = ahk.StringSplit(pageUrl, "|", 2);

                                    obj.PageName = PageName.Decode();
                                    obj.URL = "https://www.pornbb.org/" + PageURL;
                                    tel.Update(CurrentURLDisplay, obj.URL);

                                    string Html = web.DownloadHTML(obj.URL);
                                    if (Html == "") { ahk.MsgBox(obj.URL + "\n\nNo HTML Returned"); continue; }
                                    bool ContainsLinks = false;
                                    if (Html.Contains("rapidgator.net") || Html.Contains("rg.to")) { ContainsLinks = true; }

                                    obj.ContainsLinks = ContainsLinks;
                                    obj.DateParsed = new DateTime(1900, 1, 1);

                                    if (obj.ContainsLinks) { HaveLinksCount++; }

                                    tel.Update(GoodCountDisp, HaveLinksCount.ToString() + " Posts With Links");

                                    forumIndex_UpdateInsertSQL(obj); // write sql
                                    //forumIndex_UpdateInsert(obj, SQLiteDB);

                                    //ahk.MsgBox(PageName + "\n\n" + "Total Pages: " + pagenumText + "\n\n" + obj.URL + "\n\nContains RGLinks = " + ContainsLinks.ToString());
                                    break;
                                }
                            }
                            //ahk.MsgBox(line);
                        }

                        RunLoop = ForumLoopEnabled;
                    }

                    indexPageNum += Increment;
                    tel.Update(IndexPageNumDisplay, indexPageNum.ToString());
                    sqlite.Setting("ForumIndexPageNum", indexPageNum.ToString());

                }
                while (RunLoop);


                return "";
            }

            int IndexPageN = 8640;
            public string ParsePBB(PictureBox ImageDisplay, string BaseURL, int StartPageNum, int Increment, string PageName, string SiteName)
            {
                PreviousGoodTotal(); // read previously stored total

                bool RunLoop = true;
                do
                {
                    string url = BaseURL + StartPageNum + ".html";

                    string SaveDir = ahk.AppDir() + "\\Posts\\" + SiteName + "\\" + PageName + "\\" + StartPageNum;


                    SetupRapidGator();// rg passes for linkcheck


                    //if (StartPageNum == 0) { url = url.Replace("-.html", ".html"); }

                    string html = web.DownloadHTML(url);
                    if (html == "") { ahk.MsgBox("No HTML Returned"); return ""; }

                    if (html.Contains("Could not obtain post")) { ahk.MsgBox("End of Posts Reached"); RunLoop = false; continue; }

                    int GoodCount = 0;
                    int BadCount = 0;

                    // parse html for list of rapidgator links
                    //List<string> links = rg.Extract_RapidGatorLinks_HTML(html, true);
                    //sb.StatusBar("Found " + links.Count().ToString() + " RapidGator Links On " + url);


                    string xmlPath = "//body/div/div/div/div/div/div/div";



                    List<string> lines = xml.Parse_HTML_XML(html, xmlPath);  // extract sections of text from html xml

                    List<string> Posts = new List<string>();

                    int PostNum = 1;
                    int i = 1; string sSaveDir = "";
                    foreach (string line in lines)
                    {
                        //ahk.MsgBox(line);
                        if (line.ToUpper().Contains("RAPIDGATOR.NET"))
                        {
                            List<string> RGLinks = rg.Extract_RapidGatorLinks_HTML(line);

                            if (RGLinks.Count == 0) { continue; }

                            pro.SetupProgressBar(ProgressDisp, RGLinks.Count);

                            string GoodLinks = "";
                            bool AllOnline = true; int l = 1;
                            foreach (string link in RGLinks)
                            {
                                pro.UpdateProgress(ProgressDisp, l + "/" + RGLinks.Count.ToString() + " Links Checked"); l++;

                                _Sites.RapidGator.RGInfo CheckedLinks = rg.RapidGatorCheckStatus(link);

                                if (!CheckedLinks.FileOnline)  // all links in posts weren't online
                                {
                                    BadCount++;
                                    tel.Update(BadCountDisp, BadCount.ToString() + " Bad Posts");
                                    AllOnline = false;
                                    break;
                                }
                                else  // links online
                                {
                                    if (GoodLinks == "") { GoodLinks = link + "|" + CheckedLinks.FileName + "|" + CheckedLinks.FileSize + "|" + DateTime.Now.ToString(); }
                                    else { GoodLinks = GoodLinks + "\n" + link + "|" + CheckedLinks.FileName + "|" + CheckedLinks.FileSize + "|" + DateTime.Now.ToString(); }

                                    Posts.Add(line); // store text from post 
                                    GoodCount++;
                                    tel.Update(GoodCountDisp, GoodCount.ToString() + " Good Posts");
                                }
                            }

                            if (AllOnline)  // good post
                            {
                                bool AllImages = true;

                                PostNum++;
                                sSaveDir = SaveDir + "\\" + ahk.AddLeadingZeros(PostNum.ToString(), 3);
                                ahk.FileCreateDir(sSaveDir);


                                // Extract / Display Images From Page

                                ahk.FileAppend(GoodLinks, sSaveDir + "\\Links.txt");

                                List<string> images = prs.JpgImageLinks(line);  // list of jpg images found on page
                                string savename = "";

                                if (images != null && images.Count > 0)  // images found on post
                                {
                                    int n = 0;
                                    foreach (string image in images)
                                    {
                                        savename = sSaveDir + "\\" + StartPageNum + "_" + PostNum + "_" + n + ".jpg";
                                        bool NameExists = true;
                                        do
                                        {
                                            savename = sSaveDir + "\\" + StartPageNum + "_" + n + ".jpg";
                                            if (!File.Exists(savename)) { NameExists = false; }
                                            n++;
                                        } while (NameExists);

                                        if (!File.Exists(savename)) { web.DownloadFile(image, savename, true); }

                                        if (!AllImages) { break; }
                                    }
                                }
                                else // no images found on post 
                                {

                                }

                                ahk.FileAppend(line, sSaveDir + "\\PostHTML.txt");


                                if (savename != "")
                                {
                                    if (ImageDisplay != null)
                                    {
                                        if (ImageDisplay.InvokeRequired)
                                        {
                                            ImageDisplay.BeginInvoke((MethodInvoker)delegate ()
                                            {
                                                ImageDisplay.Image = img.GetCopyImage(savename);
                                            });
                                        }
                                        else { ImageDisplay.Image = img.GetCopyImage(savename); }
                                    }
                                }
                            }


                        }

                        i++;
                    }

                    GoodTotal += GoodCount;
                    //ahk.MsgBox(GoodCount + " Links Found Good\n" + BadCount + " Links Found Bad");
                    StartPageNum += Increment;

                    sqlite.Setting("ForumCurrentPageNum", StartPageNum.ToString());

                    tel.Update(CurrentPageNumDisp, StartPageNum.ToString());
                    tel.Update(GoodTotalDisp, GoodTotal);

                    sqlite.Setting("ForumGoodTotal", GoodTotal.ToString());

                    RunLoop = ForumLoopEnabled;

                } while (RunLoop);



                return "";
            }

            public void PreviousGoodTotal()
            {
                GoodTotal = sqlite.Setting("ForumGoodTotal").ToInt();
            }



            #region === forumIndex FUNCTIONS ===


            public bool Create_ForumSectionsIndex(string DbFile)
            {
                // create forum sections index
                string CreateLine = "ID INTEGER PRIMARY KEY, SiteName VARCHAR, SectionURL VARCHAR, SectionName VARCHAR, CurrentPageNum VARCHAR, PageNumTotal VARCHAR, DateModified VARCHAR, ContainsLinksCount VARCHAR";
                //bool TableCreated = sqlite.Execute(DbFile, CreateLine);
                bool TableCreated = sqlite.Table_New(DbFile, "SectionIndex", CreateLine, false);
                if (!TableCreated) { ahk.MsgBox("[SectionIndex] Created = " + TableCreated.ToString()); }
                return TableCreated;
            }



            #region ===== forumIndex Object =====

            public struct forumIndex
            {
                public int ID { get; set; }
                public string URL { get; set; }
                public string PageName { get; set; }
                public int PageCount { get; set; }
                public DateTime DateAdded { get; set; }
                public DateTime DateParsed { get; set; }
                public DateTime DateModified { get; set; }
                public bool ContainsLinks { get; set; }
            }

            #endregion
            public bool Create_Table_ForumIndex(string DbFile)
            {
                string CreateLine = "ID INTEGER PRIMARY KEY, URL VARCHAR, PageName VARCHAR, PageCount VARCHAR, DateAdded VARCHAR, DateModified VARCHAR, DateParsed VARCHAR, ContainsLinks VARCHAR";
                //bool TableCreated = sqlite.Execute(DbFile, CreateLine);
                bool TableCreated = sqlite.Table_New(DbFile, "ForumIndex", CreateLine, false);
                if (!TableCreated) { ahk.MsgBox("[ForumIndex] Created = " + TableCreated.ToString()); }

                return TableCreated;
            }




            #region ===== forumIndex SQLite : Return =====

            public forumIndex Return_Object_From_ForumIndex(string WhereClause = "[ID] = ''", string DbFile = "")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\forumIndex.sqlite"; }
                string SelectLine = "Select [ID], [URL], [PageName], [PageCount], [DateAdded], [DateParsed], [ContainsLinks] From [ForumIndex] ";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
                forumIndex returnObject = new forumIndex();
                int i = 0;
                string Value = "";
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ID = ret["ID"].ToInt();
                        returnObject.URL = ret["URL"].ToString();
                        returnObject.PageName = ret["PageName"].ToString();
                        returnObject.PageCount = ret["PageCount"].ToInt();
                        returnObject.DateAdded = ret["DateAdded"].ToDateTime();
                        returnObject.DateParsed = ret["DateParsed"].ToDateTime();
                        returnObject.ContainsLinks = ret["ContainsLinks"].ToBool();
                    }
                }

                return returnObject;
            }

            public List<forumIndex> Return_forumIndex_List(string WhereClause = "", string DbFile = "", string TableName = "[ForumIndex]")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\forumIndex.sqlite"; }
                string SelectLine = "Select * From " + TableName;

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

                List<forumIndex> ReturnList = new List<forumIndex>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        forumIndex returnObject = new forumIndex();

                        returnObject.ID = ret["ID"].ToInt();
                        returnObject.URL = ret["URL"].ToString();
                        returnObject.PageName = ret["PageName"].ToString();
                        returnObject.PageCount = ret["PageCount"].ToInt();
                        returnObject.DateAdded = ret["DateAdded"].ToDateTime();
                        returnObject.DateParsed = ret["DateParsed"].ToDateTime();
                        returnObject.ContainsLinks = ret["ContainsLinks"].ToBool();

                        ReturnList.Add(returnObject);
                    }
                }

                return ReturnList;
            }

            public DataTable Return_DataTable_From_ForumIndex(string DbFile)
            {
                string SelectLine = "Select [ID], [URL], [PageName], [PageCount], [DateAdded], [DateParsed], [ContainsLinks] From [ForumIndex]";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                return ReturnTable;
            }


            #endregion
            #region ===== forumIndex SQLite : Update Insert =====

            public bool forumIndex_Insert(forumIndex inObject, string DbFile = "")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\forumIndex.sqlite"; }
                string InsertLine = "Insert Into [ForumIndex] (URL, PageName, PageCount, DateAdded, DateModified, DateParsed, ContainsLinks) values ('" + inObject.URL + "', '" + inObject.PageName + "', '" + inObject.PageCount + "', '" + DateTime.Now.ToString() + "', '" + DateTime.Now.ToString() + "', '" + inObject.DateParsed + "', '" + inObject.ContainsLinks + "')";
                bool Inserted = sqlite.Execute(DbFile, InsertLine);
                if (!Inserted) { ahk.MsgBox("Inserted Into [ForumIndex] = " + Inserted.ToString()); }
                return Inserted;
            }

            public bool forumIndex_Update(forumIndex inObject, string DbFile = "")
            {
                //string UpdateLine = "Update [ForumIndex] set ID = '" + inObject.ID + "', URL = '" + inObject.URL + "', PageName = '" + inObject.PageName + "', PageCount = '" + inObject.PageCount + "', DateAdded = '" + inObject.DateAdded + "', DateParsed = '" + inObject.DateParsed + "', ContainsLinks = '" + inObject.ContainsLinks + "' WHERE [Item] = 'Value' ";
                string UpdateLine = "Update [ForumIndex] set ";


                if (inObject.URL != null) { UpdateLine = UpdateLine + "[URL] = '" + inObject.URL + "',"; }
                if (inObject.PageName != null) { UpdateLine = UpdateLine + "[PageName] = '" + inObject.PageName + "',"; }
                UpdateLine = UpdateLine + "[PageCount] = '" + inObject.PageCount + "',";
                if (inObject.DateAdded != null) { UpdateLine = UpdateLine + "[DateModified] = '" + DateTime.Now.ToString() + "',"; }
                if (inObject.DateAdded != null) { UpdateLine = UpdateLine + "[DateAdded] = '" + inObject.DateAdded + "',"; }
                if (inObject.DateParsed != null) { UpdateLine = UpdateLine + "[DateParsed] = '" + inObject.DateParsed + "',"; }
                UpdateLine = UpdateLine + "[ContainsLinks] = '" + inObject.ContainsLinks + "',";

                UpdateLine = ahk.TrimLast(UpdateLine, 1);
                UpdateLine = UpdateLine + " WHERE [URL] = '" + inObject.URL + "'"; // DEFINE CONDITION HERE !!!

                bool Updated = sqlite.Execute(DbFile, UpdateLine);
                return Updated;
            }

            public bool forumIndex_UpdateInsert(forumIndex obj, string DbFile = "")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\forumIndex.sqlite"; }

                if (!File.Exists(DbFile)) { Create_Table_ForumIndex(DbFile); }

                obj.PageName = obj.PageName.Encode();


                bool Updated = forumIndex_Update(obj, DbFile);  // try to update record first
                if (!Updated) { Updated = forumIndex_Insert(obj, DbFile); }  // if unable to update, insert new record
                return Updated;
            }


            #endregion
            #region ===== forumIndex DataTable =====

            public DataTable Return_forumIndex_DataTable(string DbFile = "", string TableName = "forumIndex", string WhereClause = "", bool Debug = false)
            {

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\forumIndex.sqlite"; }
                string SelectLine = "Select * From [ForumIndex]";

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }

                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


                DataTable table = new DataTable();
                table.Columns.Add("ID", typeof(string));
                table.Columns.Add("URL", typeof(string));
                table.Columns.Add("PageName", typeof(string));
                table.Columns.Add("PageCount", typeof(string));
                table.Columns.Add("DateAdded", typeof(string));
                table.Columns.Add("DateParsed", typeof(string));
                table.Columns.Add("ContainsLinks", typeof(string));

                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        forumIndex returnObject = new forumIndex();

                        returnObject.ID = ret["ID"].ToInt();
                        returnObject.URL = ret["URL"].ToString();
                        returnObject.PageName = ret["PageName"].ToString();
                        returnObject.PageCount = ret["PageCount"].ToInt();
                        returnObject.DateAdded = ret["DateAdded"].ToDateTime();
                        returnObject.DateParsed = ret["DateParsed"].ToDateTime();
                        returnObject.ContainsLinks = ret["ContainsLinks"].ToBool();

                        table.Rows.Add(returnObject.ID, returnObject.URL, returnObject.PageName, returnObject.PageCount, returnObject.DateAdded, returnObject.DateParsed, returnObject.ContainsLinks);
                    }
                }

                return table;
            }

            public DataTable Create_ForumIndex_DataTable(forumIndex inObject)
            {
                DataTable table = new DataTable();
                table.Columns.Add("ID", typeof(string));
                table.Columns.Add("URL", typeof(string));
                table.Columns.Add("PageName", typeof(string));
                table.Columns.Add("PageCount", typeof(string));
                table.Columns.Add("DateAdded", typeof(string));
                table.Columns.Add("DateParsed", typeof(string));
                table.Columns.Add("ContainsLinks", typeof(string));

                table.Rows.Add(inObject.ID, inObject.URL, inObject.PageName, inObject.PageCount, inObject.DateAdded, inObject.DateParsed, inObject.ContainsLinks);
                return table;
            }


            #endregion
            #region ===== forumIndex DataGridView =====

            public void HideShow_ForumIndex_Columns(DataGridView dv)
            {

                try { dv.Columns["ID"].Visible = true; } catch { }
                try { dv.Columns["URL"].Visible = true; } catch { }
                try { dv.Columns["PageName"].Visible = true; } catch { }
                try { dv.Columns["PageCount"].Visible = true; } catch { }
                try { dv.Columns["DateAdded"].Visible = true; } catch { }
                try { dv.Columns["DateParsed"].Visible = true; } catch { }
                try { dv.Columns["ContainsLinks"].Visible = true; } catch { }
            }
            public void Enable_ForumIndex_Columns(DataGridView dv)
            {

                try { dv.Columns["ID"].ReadOnly = true; } catch { }
                try { dv.Columns["URL"].ReadOnly = true; } catch { }
                try { dv.Columns["PageName"].ReadOnly = true; } catch { }
                try { dv.Columns["PageCount"].ReadOnly = true; } catch { }
                try { dv.Columns["DateAdded"].ReadOnly = true; } catch { }
                try { dv.Columns["DateParsed"].ReadOnly = true; } catch { }
                try { dv.Columns["ContainsLinks"].ReadOnly = true; } catch { }
            }

            #endregion
            #region ===== forumIndex SQL Functions =====

            // Return forumIndex SQL Connection String
            public SqlConnection forumIndex_Conn()
            {
                // populate sql connection
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLserver"].ConnectionString);
                // SqlConnection Conn = new SqlConnection("Server=188.168.188.88;DataBase=LucidMedia;Uid=lucidm;Pwd=pass");
                return conn;
            }

            // Return forumIndex TableName (Full Path)
            public string forumIndex_TableName()
            {
                // populate to return full sql table name
                return "[lucidmedia].[lucidmethod].[ForumIndex]";
            }

            // Generate SQL Table
            public bool forumIndex_CreateSQLTable()
            {
                SqlConnection Conn = forumIndex_Conn();
                string CreateTableLine = "CREATE TABLE [ForumIndex](";
                CreateTableLine = CreateTableLine + "[ID] [int] IDENTITY(1,1) NOT NULL,";
                CreateTableLine = CreateTableLine + "[URL] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[PageName] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[PageCount] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[DateAdded] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[DateParsed] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[ContainsLinks] [varchar](max) NOT NULL";
                CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                return false;
            }

            public bool forumIndex_InsertSQL(forumIndex obj)
            {
                SqlConnection Con = forumIndex_Conn();
                string SQLLine = "Insert Into " + forumIndex_TableName() + " (URL, PageName, PageCount, DateAdded, DateParsed, ContainsLinks) VALUES (@URL, @PageName, @PageCount, @DateAdded, @DateParsed, @ContainsLinks)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.URL == null) { obj.URL = ""; }
                cmd2.Parameters.AddWithValue(@"URL", obj.URL.ToString());
                cmd2.Parameters.AddWithValue(@"PageName", obj.PageName.ToString());
                cmd2.Parameters.AddWithValue(@"PageCount", obj.PageCount.ToString());
                cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded.ToString());
                cmd2.Parameters.AddWithValue(@"DateParsed", obj.DateParsed.ToString());
                cmd2.Parameters.AddWithValue(@"ContainsLinks", obj.ContainsLinks.ToString());
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        forumIndex_InsertSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        forumIndex_InsertSQL(obj);
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

            public bool forumIndex_UpdateSQL(forumIndex obj)
            {
                SqlConnection Conn = forumIndex_Conn();
                string SQLLine = "Update " + forumIndex_TableName() + " SET PageName = @PageName, PageCount = @PageCount, DateAdded = @DateAdded, DateParsed = @DateParsed, ContainsLinks = @ContainsLinks WHERE URL = @URL";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.URL == null) { obj.URL = ""; }

                cmd2.Parameters.AddWithValue(@"URL", obj.URL.ToString());
                cmd2.Parameters.AddWithValue(@"PageName", obj.PageName.ToString());
                cmd2.Parameters.AddWithValue(@"PageCount", obj.PageCount.ToString());
                cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded.ToString());
                cmd2.Parameters.AddWithValue(@"DateParsed", obj.DateParsed.ToString());
                cmd2.Parameters.AddWithValue(@"ContainsLinks", obj.ContainsLinks.ToString());

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        forumIndex_UpdateSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        forumIndex_UpdateSQL(obj);
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

            public bool forumIndex_UpdateInsertSQL(forumIndex obj)
            {
                SqlConnection Conn = forumIndex_Conn();
                bool Updated = forumIndex_UpdateSQL(obj);  // try to update record first
                if (!Updated) { Updated = forumIndex_InsertSQL(obj); }  // if unable to update, insert new record
                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool forumIndex_UpdateIfPopulated(forumIndex obj, string ID = "")
            {
                SqlConnection Conn = forumIndex_Conn();
                string SQLcmd = "Update " + forumIndex_TableName() + " SET ";
                if (obj.ID != null) { SQLcmd = SQLcmd + " ID = @ID,"; }
                if (obj.URL != null) { SQLcmd = SQLcmd + " URL = @URL,"; }
                if (obj.PageName != null) { SQLcmd = SQLcmd + " PageName = @PageName,"; }
                if (obj.PageCount != null) { SQLcmd = SQLcmd + " PageCount = @PageCount,"; }
                if (obj.DateAdded != null) { SQLcmd = SQLcmd + " DateAdded = @DateAdded,"; }
                if (obj.DateParsed != null) { SQLcmd = SQLcmd + " DateParsed = @DateParsed,"; }
                if (obj.ContainsLinks != null) { SQLcmd = SQLcmd + " ContainsLinks = @ContainsLinks,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ID = @ID";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.ID != null) { cmd2.Parameters.AddWithValue(@"ID", obj.ID); }
                if (obj.URL != null) { cmd2.Parameters.AddWithValue(@"URL", obj.URL); }
                if (obj.PageName != null) { cmd2.Parameters.AddWithValue(@"PageName", obj.PageName); }
                if (obj.PageCount != null) { cmd2.Parameters.AddWithValue(@"PageCount", obj.PageCount); }
                if (obj.DateAdded != null) { cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded); }
                if (obj.DateParsed != null) { cmd2.Parameters.AddWithValue(@"DateParsed", obj.DateParsed); }
                if (obj.ContainsLinks != null) { cmd2.Parameters.AddWithValue(@"ContainsLinks", obj.ContainsLinks); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public forumIndex forumIndex_ReturnSQL(string ID = "")
            {
                SqlConnection Conn = forumIndex_Conn();
                string SelectLine = "Select [ID],[URL],[PageName],[PageCount],[DateAdded],[DateParsed],[ContainsLinks] From " + forumIndex_TableName() + " WHERE ID = '" + ID + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                forumIndex returnObject = new forumIndex();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ID = ret["ID"].ToInt();
                        returnObject.URL = ret["URL"].ToString();
                        returnObject.PageName = ret["PageName"].ToString();
                        returnObject.PageCount = ret["PageCount"].ToInt();
                        returnObject.DateAdded = ret["DateAdded"].ToDateTime();
                        returnObject.DateParsed = ret["DateParsed"].ToDateTime();
                        returnObject.ContainsLinks = ret["ContainsLinks"].ToBool();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public List<forumIndex> forumIndex_ReturnSQLList(string Command = "")
            {
                if (Command == "") { Command = "Select * From forumIndex_TableName()"; }
                SqlConnection Conn = forumIndex_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<forumIndex> ReturnList = new List<forumIndex>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        forumIndex returnObject = new forumIndex();
                        returnObject.ID = ret["ID"].ToInt();
                        returnObject.URL = ret["URL"].ToString();
                        returnObject.PageName = ret["PageName"].ToString();
                        returnObject.PageCount = ret["PageCount"].ToInt();
                        returnObject.DateAdded = ret["DateAdded"].ToDateTime();
                        returnObject.DateParsed = ret["DateParsed"].ToDateTime();
                        returnObject.ContainsLinks = ret["ContainsLinks"].ToBool();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }

            public bool forumIndex_SQL_to_SQLite(string SqliteDBPath = @"\Db\forumIndex.sqlite")
            {
                string SaveFile = SqliteDBPath;
                if (SqliteDBPath == @"\Db\forumIndex.sqlite")
                {
                    ahk.FileCreateDir(ahk.AppDir() + @"\Db");
                    SaveFile = ahk.AppDir() + @"\Db\forumIndex.sqlite";
                }

                //sb.StatusBar("Copying SQL Db to " + SaveFile + "...");
                sqlite.SQLTable_To_NewSQLiteTable(forumIndex_Conn(), "ForumIndex", "ForumIndex", SaveFile, "", false, false, false);
                //sb.StatusBar("FINISHED Copying SQL Db to " + SaveFile);

                if (File.Exists(SaveFile)) { return true; } else { return false; }
            }


            #endregion

            #endregion




        }




        #endregion


        #region === Download ===

        /// <summary>
        /// Download File From Website
        /// </summary>
        /// <param name="remoteFileUrl">URL to File</param>
        /// <param name="localFileName">Local Save Path</param>
        /// <param name="SkipExisting">Option to Skip Downloading if Local File Already Exists</param>
        /// <returns></returns>
        public bool DownloadFile(string remoteFileUrl, string localFileName, bool SkipExisting = true)
        {
            // if enabled, will skip downloading the same file again 

            if (SkipExisting) { if (File.Exists(localFileName)) { return true; } }


            WebClient webClient = new WebClient();
            try
            {
                webClient.DownloadFile(remoteFileUrl, localFileName);
            }
            catch (Exception ex)
            {
                //ahk.MsgBox(ex.ToString());
                return false;
            }

            // confirm file is found in local location after download, return true if found

            if (File.Exists(localFileName)) { sb.StatusBar("Saved " + localFileName); return true; }
            else { ahk.MsgBox("Error Saving " + localFileName + " ??\n\nLocal File Not Found\n\nURL: " + remoteFileUrl); return false; }
        }

        /// <summary>
        /// Download Website HTML to String (No WebBrowser Control Required)
        /// </summary>
        /// <param name="url">URL of Website To Download</param>
        /// <param name="StatusBarDisp">Option to Display Download Status in StatusBar</param>
        /// <returns></returns>
        public string DownloadHTML(string url, bool StatusBarDisp = false, bool DebugMsg = false)
        {
            string htmlCode = "";

            using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
            {
                if (StatusBarDisp) { sb.StatusBar("Downloading " + url + " HTML..."); }

                // Or you can get the file content without saving it:

                try
                {
                    htmlCode = client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    htmlCode = ahk.Download_HTML(url);

                    //using (WebClient Client = new WebClient())
                    //{
                    //    Client.Credentials = CredentialCache.DefaultCredentials;
                    //    var proxyUri = WebRequest.GetSystemWebProxy()
                    //        .GetProxy(new Uri(url));
                    //    Client.Proxy = new WebProxy(proxyUri);
                    //    Client.Proxy.Credentials = CredentialCache.DefaultCredentials;

                    //    try
                    //    {
                    //        htmlCode = Client.DownloadString(url);
                    //    }
                    //    catch (Exception Ex)
                    //    {
                    //        // The remote server returned an error: (502) Bad Gateway.
                    //        Console.WriteLine(ex.Message);
                    //    }
                    //}
                    //Console.WriteLine("Press any key for exit.");
                    //Console.ReadKey();


                    //// Create web client.
                    //WebClient clienT = new WebClient();

                    //// Set user agent and also accept-encoding headers.
                    ////clienT.Headers["User-Agent"] = "Googlebot/2.1 (+http://www.googlebot.com/bot.html)";
                    //clienT.Headers["User-Agent"] = "Mozilla/4.0 (Compatible; Windows NT 5.1; MSIE 6.0) " + "(compatible; MSIE 6.0; Windows NT 5.1; " + ".NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                    //clienT.Headers["Accept-Encoding"] = "gzip";

                    //// Download string.
                    //htmlCode = clienT.DownloadString(url);






                    //using (HttpClient Client = new HttpClient())
                    //{
                    //    using (HttpResponseMessage response = Client.GetAsync(url).Result)
                    //    {
                    //        using (HttpContent content = response.Content)
                    //        {
                    //            htmlCode = content.ReadAsStringAsync().Result;
                    //        }
                    //    }
                    //}

                    if (htmlCode != "")
                    {
                        return htmlCode;
                    }
                    else
                    {
                        if (DebugMsg) { ahk.MsgBox(ex.ToString()); }
                    }

                    //if (StatusBarDisp) { sb.StatusBar("Error Downloading HTML from " + url); }


                    return "";
                }
            }

            if (StatusBarDisp) { sb.StatusBar("Downloaded HTML From " + url); }

            return htmlCode;
        }


        #endregion


        //=== Web Browser ===

        #region === WebBrowser ===

        public RadDropDownList wBrowseURL { get; set; }
        public WebBrowser wBrowser { get; set; }

        /// <summary>
        /// Returns Current URL in webBrowser (+ Updates Statusbar with Value)
        /// </summary>
        /// <returns></returns>
        public string CurrentURL()
        {
            if (wBrowser == null) { return ""; }

            string currentURL = "";

            if (wBrowser.InvokeRequired)  // if currently on a different thread, invoke label first
            {
                wBrowser.BeginInvoke((MethodInvoker)delegate () { currentURL = wBrowser.Url.ToString(); });  // current url in browser 
            }
            else
            {
                currentURL = wBrowser.Url.ToString();  // current url in browser
            }


            if (wBrowseURL.InvokeRequired)  // if currently on a different thread, invoke label first
            {
                wBrowseURL.BeginInvoke((MethodInvoker)delegate () { wBrowseURL.Text = currentURL; });
            }
            else
            {
                wBrowseURL.Text = currentURL;
            }

            sb.StatusBar(currentURL);
            return currentURL;
        }

        /// <summary>
        /// Save Current HTML from IE Browser Control 
        /// </summary>
        /// <param name="SaveLocation">If SaveLocation Provided, Saves HTML to Local File Path</param>
        /// <returns></returns>
        public string CurrentHTML(string SaveLocation = "")
        {
            if (wBrowser == null) { return ""; }

            string html = "";

            if (wBrowser.InvokeRequired)  // if currently on a different thread, invoke label first
            {
                wBrowser.BeginInvoke((MethodInvoker)delegate () { html = wBrowser.DocumentText; });
            }
            else
            {
                html = wBrowser.DocumentText;
            }

            // if save location provided, saves html to file
            if (SaveLocation != "") { ahk.FileDelete(SaveLocation); ahk.FileAppend(html, SaveLocation); }

            return html;
        }

        /// <summary>
        /// Returns List of Lines From WebBrowser HTML
        /// </summary>
        /// <returns></returns>
        public List<string> CurrentHTML_Lines()
        {
            string html = CurrentHTML();
            List<string> lines = lst.Text_To_List(html, true, true, false);
            return lines;
        }

        /// <summary>
        /// Populates DataGridView with HTML Parsed by New Line
        /// </summary>
        /// <param name="dv"></param>
        public void CurrentHTML_Grid(DataGridView dv, string SearchTerm = "")
        {
            List<string> lines = CurrentHTML_Lines();

            //if search term provided, loop through returning html lines to display results in grid
            List<string> results = new List<string>();
            if (SearchTerm != "")
            {
                foreach (string line in lines)
                {
                    if (line.Contains(SearchTerm)) { results.Add(line); }
                }
            }
            else
                results = lines;

            lst.List_To_Grid(dv, results, "HTML");
        }

        /// <summary>
        /// Tracks when WebBrowser Control Finishes Loading
        /// </summary>
        bool pageLoaded = false;

        // Browser Actions

        /// <summary>
        /// Load URL in webBrowser Control
        /// </summary>
        /// <param name="URL">WebSite Address To Load in WebBrowser</param>
        public void LoadURL(string URL)
        {
            sb.StatusBar("Loading " + URL + "...");

            pageLoaded = false;  // not finished until document complete reached

            if (wBrowseURL.InvokeRequired)  // if currently on a different thread, invoke label first
            {
                wBrowseURL.BeginInvoke((MethodInvoker)delegate () { wBrowseURL.Text = URL; });
            }
            else
            {
                wBrowseURL.Text = URL;
            }

            if (wBrowser.InvokeRequired)  // if currently on a different thread, invoke label first
            {
                wBrowser.BeginInvoke((MethodInvoker)delegate () { wBrowser.Navigate(URL); });
            }
            else
            {
                wBrowser.Navigate(URL);
            }

        }

        public void Forward()
        {
            wBrowser.GoForward();
        }
        public void Back()
        {
            wBrowser.GoBack();
        }
        public void Home()
        {
            wBrowser.GoHome();
        }
        public void PrintPreview()
        {
            wBrowser.ShowPrintPreviewDialog();
        }
        public void BrowserRefresh()
        {
            wBrowser.Refresh();
            sb.StatusBar("(Refreshed) " + CurrentURL());
        }
        public void ClearCache()
        {
            wBrowser.Refresh(WebBrowserRefreshOption.Completely);
        }


        //=== Browser Grid ===


        /// <summary>
        /// Returns DataTable/Populates DataGridView with HTMl Elements From Current WebPage
        /// </summary>
        /// <param name="wb">WebBrowser Control</param>
        /// <param name="dv">DataGridView Control</param>
        /// <param name="SubmitOnly">Option to Only Return Submit Buttons</param>
        /// <returns></returns>
        public DataTable HtmlElementTable(WebBrowser wb, DataGridView dv, bool SubmitOnly = false)
        {
            // Here we create a DataTable with four columns.
            DataTable table = new DataTable();
            table.Columns.Add("ID", typeof(string));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Type", typeof(string));
            table.Columns.Add("TagName", typeof(string));
            table.Columns.Add("OuterHtml", typeof(string));
            table.Columns.Add("OuterText", typeof(string));
            table.Columns.Add("InnerHtml", typeof(string));
            table.Columns.Add("InnerText", typeof(string));


            HtmlElementCollection elc = wb.Document.GetElementsByTagName("input");
            foreach (HtmlElement el in elc)
            {
                if (!SubmitOnly) { table.Rows.Add(el.Id, el.Name, el.GetType(), el.TagName, el.OuterHtml, el.OuterText, el.InnerHtml, el.InnerText); }

                if (el.GetAttribute("type").Equals("submit"))
                {
                    if (SubmitOnly) { table.Rows.Add(el.Id, el.Name, el.GetType(), el.TagName, el.OuterHtml, el.OuterText, el.InnerHtml, el.InnerText); }
                }
            }

            dv.DataSource = table;

            return table;
        }



        #endregion












    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using sharpAHK;
using AHKExpressions;
using Telerik.WinControls.UI;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace sharpAHK_Dev
{
    public partial class ADBSites
    {
        public class PRNWForum
        {
            _AHK ahk = new _AHK();
            _Database.SQL sql = new _Database.SQL();
            _Database.SQLite sqlite = new _Database.SQLite();



            #region === Site Search ===

            public List<string> SiteSearch(string SearchTerm, bool TitleSearch = false, bool NewThread = true)
            {
                _Web web = new _Web();
                _Sites.RapidGator rg = new _Sites.RapidGator();
                _TelerikLib tel = new _TelerikLib();
                _Parse.XML xml = new _Parse.XML();
                _Lists lst = new _Lists();
                _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();
                _Parse prs = new _Parse();

                List<string> LinkList = new List<string>();
                int MatchCount = 0; // number of results from search

                if (NewThread)
                {
                    Thread newThread = new Thread(() => SiteSearch(SearchTerm, TitleSearch, false)); // Function To Execute
                    newThread.Name = "SiteSearch";
                    newThread.IsBackground = true;
                    newThread.Start();
                }
                else
                {

                    string terms = SearchTerm.Replace(" ", "%20").ToLower();
                    string url = "http://www.porn-w.org/search.php?st&sk=t&sd=d&sr=posts&keywords=" + terms + "&terms=exact";

                    if (TitleSearch)
                    {
                        url = url + "&sf=titleonly";
                    }

                    int currentPage = 0;


                    //int Increment = 15;
                    //bool RunIndexLoop = false; string SQLiteDB = "";

                    //int i = StartPage; int rgLinksCount = 0;
                    //int totalpages = TotalPages(section);


                    bool RunLoop = true;
                    do
                    {
                        url = url + "&start=" + currentPage;

                        string html = web.DownloadHTML(url);
                        if (html == "" || html.Contains("Could not obtain post"))
                        {
                            //ahk.MsgBox("End of Posts Reached - No HTML Returned");
                            RunLoop = false; break;
                        }


                        if (currentPage == 0)
                        {
                            string searchResults = prs.XML_Text(html, "<h2>");
                            searchResults = searchResults.Replace("Search found", "");
                            searchResults = searchResults.Replace("matches", "").Trim();

                            MatchCount = searchResults.ToInt();
                        }


                        List<string> newLinks = rg.Regex_RGLinks(html);

                        LinkList = lst.MergeLists(LinkList, newLinks);

                        // update progress bar if provide
                        //if (disp.ProgressDisp != null) { pro.SetupProgressBar(disp.ProgressDisp, Increment); }
                        //if (disp.ProgressDisp != null) { pro.UpdateProgress(disp.ProgressDisp, section.ToString() + " | Page " + i + "/" + totalpages + " | " + k + "/" + Increment + " URLs Added | " + rgLinksCount.ToString() + " Posts With RGLinks"); k++; }

                        if (currentPage > MatchCount) { RunLoop = false; }  // reached last page

                        currentPage = currentPage + 15;

                    }
                    while (RunLoop);
                }


                if (!NewThread)
                {
                    //if (disp.ProgressDisp != null) { pro.UpdateProgress(disp.ProgressDisp, section.ToString() + " | DONE"); }
                    //ahk.MsgBox("Finished " + section.ToString() + " Index Import");
                }

                return LinkList;
            }


            public List<string> SiteSearchDisplay(string SearchTerm, bool TitleSearch = false, RadLabel MatchCountDisp = null, RadTextBox SearchResults = null, bool NewThread = true, int StartPage = 0, RadTextBox CurrentPgNum = null, RadProgressBar Bar = null)
            {
                _Web web = new _Web();
                _Sites.RapidGator rg = new _Sites.RapidGator();
                _TelerikLib tel = new _TelerikLib();
                _Parse.XML xml = new _Parse.XML();
                _Lists lst = new _Lists();
                _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();
                _Parse prs = new _Parse();

                List<string> LinkList = new List<string>();
                int MatchCount = 0; // number of results from search

                if (NewThread)
                {
                    Thread newThread = new Thread(() => SiteSearchDisplay(SearchTerm, TitleSearch, MatchCountDisp, SearchResults, false, StartPage, CurrentPgNum, Bar)); // Function To Execute
                    newThread.Name = "SiteSearch";
                    newThread.IsBackground = true;
                    newThread.Start();
                }
                else
                {

                    string terms = SearchTerm.Replace(" ", "%20").ToLower();
                    string url = "http://www.porn-w.org/search.php?st&sk=t&sd=d&sr=posts&keywords=" + terms + "&terms=exact";

                    if (TitleSearch)
                    {
                        url = url + "&sf=titleonly";
                    }

                    int currentPage = StartPage;



                    if (SearchResults != null)
                    {
                        // if continued search, pull existing values from gui to list to continue populating
                        if (StartPage != 0)
                        {
                            LinkList = SearchResults.Text.ToList(true, true, false); 
                        }
                    }


                    bool RunLoop = true;
                    do
                    {
                        url = url + "&start=" + currentPage;

                        string html = web.DownloadHTML(url);
                        if (html == "" || html.Contains("Could not obtain post"))
                        {
                            //ahk.MsgBox("End of Posts Reached - No HTML Returned");
                            RunLoop = false; break;
                        }

                        if (CurrentPgNum != null) { tel.Update(CurrentPgNum, currentPage); }


                        if (currentPage == StartPage)
                        {
                            string searchResults = prs.XML_Text(html, "<h2>");
                            searchResults = searchResults.Replace("Search found", "");
                            searchResults = searchResults.Replace("matches", "").Trim();

                            MatchCount = searchResults.ToInt();
                        }

                        if (Bar != null) { pro.SetupProgressBar(Bar, MatchCount); }

                        if (Bar != null) { pro.UpdateProgress(Bar, currentPage + "/" + MatchCount); }

                        List<string> newLinks = rg.Regex_RGLinks(html);

                        LinkList = lst.MergeLists(LinkList, newLinks);

                        if (MatchCountDisp != null)
                        {
                            tel.Update(MatchCountDisp, "CurrentPage: " + currentPage + " | MatchesFound: " + MatchCount + " | LinksFound: " + LinkList.Count);
                        }

                        if (SearchResults != null)
                        {
                            // clear results field when starting new search, otherwise continue to populate
                            if (StartPage == 0) { tel.Update(SearchResults, LinkList.List_ToStringLines()); }
                        }

                        // update progress bar if provide
                        //if (disp.ProgressDisp != null) { pro.SetupProgressBar(disp.ProgressDisp, Increment); }
                        //if (disp.ProgressDisp != null) { pro.UpdateProgress(disp.ProgressDisp, section.ToString() + " | Page " + i + "/" + totalpages + " | " + k + "/" + Increment + " URLs Added | " + rgLinksCount.ToString() + " Posts With RGLinks"); k++; }

                        if (currentPage > MatchCount) { RunLoop = false; }  // reached last page

                        currentPage = currentPage + 15;

                    }
                    while (RunLoop);
                }


                if (!NewThread)
                {
                    //if (disp.ProgressDisp != null) { pro.UpdateProgress(disp.ProgressDisp, section.ToString() + " | DONE"); }
                    //ahk.MsgBox("Finished " + section.ToString() + " Index Import");
                }

                return LinkList;
            }


            #endregion


            #region === Site Sync ===

            public string NextURL(PrnWSection Section, int StartPage = 0)
            {
                int StartNum = 0;
                if (StartPage != 0) { StartNum = StartPage * 50; }  // determine starting url number

                string name = Section.ToString();
                string url = "";

                if (name == "FullMovies") { url = "http://www.porn-w.org/forum1-" + StartNum + ".html"; }
                if (name == "VideoMegaThreads") { url = "http://www.porn-w.org/forum2-" + StartNum + ".html"; }
                if (name == "ClipsShorts") { url = "http://www.porn-w.org/forum3-" + StartNum + ".html"; }
                if (name == "HighDef") { url = "http://www.porn-w.org/forum32-" + StartNum + ".html"; }
                if (name == "Vintage") { url = "http://www.porn-w.org/forum33-" + StartNum + ".html"; }
                if (name == "Celebrity") { url = "http://www.porn-w.org/forum35-" + StartNum + ".html"; }
                if (name == "Amateur") { url = "http://www.porn-w.org/forum28-" + StartNum + ".html"; }
                if (name == "Hentai") { url = "http://www.porn-w.org/forum5-" + StartNum + ".html"; }

                if (name == "ModelPics") { url = "http://www.porn-w.org/forum40-" + StartNum + ".html"; }
                if (name == "ModelVideos") { url = "http://www.porn-w.org/forum38-" + StartNum + ".html"; }
                if (name == "ModelVideosHD") { url = "http://www.porn-w.org/forum39-" + StartNum + ".html"; }

                if (name == "SoftcorePics") { url = "http://www.porn-w.org/forum6-" + StartNum + ".html"; }
                if (name == "HardcorePics") { url = "http://www.porn-w.org/forum7-" + StartNum + ".html"; }
                if (name == "AnimePics") { url = "http://www.porn-w.org/forum18-" + StartNum + ".html"; }
                if (name == "CelebPics") { url = "http://www.porn-w.org/forum8-" + StartNum + ".html"; }
                if (name == "AmateurPics") { url = "http://www.porn-w.org/forum36-" + StartNum + ".html"; }
                if (name == "PicMegaThread") { url = "http://www.porn-w.org/forum9-" + StartNum + ".html"; }

                if (name == "Games") { url = "http://www.porn-w.org/forum11-" + StartNum + ".html"; }
                if (name == "Magazines") { url = "http://www.porn-w.org/forum20-" + StartNum + ".html"; }
                if (name == "BabeID") { url = "http://www.porn-w.org/forum29-" + StartNum + ".html"; }
                if (name == "Other") { url = "http://www.porn-w.org/forum16-" + StartNum + ".html"; }
                return url;
            }

            //[DefaultValue(Cancelled)]
            public enum PrnWSection
            {
                FullMovies, VideoMegaThreads, ClipsShorts, HighDef, Vintage, Celebrity, Amateur, Hentai, ModelPics, ModelVideos, ModelVideosHD, SoftcorePics, HardcorePics, AnimePics,
                CelebPics, AmateurPics, PicMegaThread, Games, Magazines, BabeID, Other
            }

            public PrnWSection ToSection(string SectionName)
            {
                if (SectionName == "FullMovies") { return PrnWSection.FullMovies; }
                if (SectionName == "Amateur") { return PrnWSection.Amateur; }
                if (SectionName == "Celebrity") { return PrnWSection.Celebrity; }
                if (SectionName == "Games") { return PrnWSection.Games; }
                if (SectionName == "HighDef") { return PrnWSection.HighDef; }
                if (SectionName == "Hentai") { return PrnWSection.Hentai; }
                if (SectionName == "AnimePics") { return PrnWSection.AnimePics; }
                if (SectionName == "Magazines") { return PrnWSection.Magazines; }
                if (SectionName == "VideoMegaThreads") { return PrnWSection.VideoMegaThreads; }
                if (SectionName == "ModelPics") { return PrnWSection.ModelPics; }
                if (SectionName == "ModelVideos") { return PrnWSection.ModelVideos; }
                if (SectionName == "ModelVideosHD") { return PrnWSection.ModelVideosHD; }
                if (SectionName == "AmateurPics") { return PrnWSection.AmateurPics; }
                if (SectionName == "BabeID") { return PrnWSection.BabeID; }
                if (SectionName == "CelebPics") { return PrnWSection.CelebPics; }
                if (SectionName == "Other") { return PrnWSection.Other; }
                if (SectionName == "PicMegaThread") { return PrnWSection.PicMegaThread; }
                if (SectionName == "SoftcorePics") { return PrnWSection.SoftcorePics; }
                if (SectionName == "HardcorePics") { return PrnWSection.HardcorePics; }
                if (SectionName == "ClipsShorts") { return PrnWSection.ClipsShorts; }
                if (SectionName == "Vintage") { return PrnWSection.Vintage; }

                return PrnWSection.Other;
            }


            public int TotalPages(PrnWSection SectionName)
            {
                if (SectionName.ToString() == "FullMovies") { return 3406; }
                if (SectionName.ToString() == "VideoMegaThreads") { return 177; }
                if (SectionName.ToString() == "ClipsShorts") { return 15422; }
                if (SectionName.ToString() == "HighDef") { return 2467; }
                if (SectionName.ToString() == "Vintage") { return 107; }
                if (SectionName.ToString() == "Celebrity") { return 233; }
                if (SectionName.ToString() == "Amateur") { return 96; }
                if (SectionName.ToString() == "Hentai") { return 17; }
                if (SectionName.ToString() == "ModelPics") { return 1; }
                if (SectionName.ToString() == "ModelVideos") { return 8; }
                if (SectionName.ToString() == "ModelVideosHD") { return 5; }
                if (SectionName.ToString() == "SoftcorePics") { return 794; }
                if (SectionName.ToString() == "HardcorePics") { return 1067; }
                if (SectionName.ToString() == "AnimePics") { return 13; }
                if (SectionName.ToString() == "CelebPics") { return 130; }
                if (SectionName.ToString() == "AmateurPics") { return 105; }
                if (SectionName.ToString() == "PicMegaThread") { return 87; }
                if (SectionName.ToString() == "Games") { return 111; }
                if (SectionName.ToString() == "Magazines") { return 124; }
                if (SectionName.ToString() == "BabeID") { return 133; }
                if (SectionName.ToString() == "Other") { return 8; }
                return 0;
            }

            public List<string> PrnWSections()
            {
                List<string> items = new List<string> { "FullMovies","VideoMegaThreads","ClipsShorts","HighDef","Vintage","Celebrity","Amateur","Hentai","ModelPics","ModelVideos","ModelVideosHD",
                "SoftcorePics","HardcorePics","AnimePics","CelebPics","AmateurPics","PicMegaThread","Games","Magazines","BabeID","Other"  };

                _Lists lst = new _Lists();
                return lst.SortList(items);
            }


            int indexPageNum = 0;

            public string Category_IndexLoop(PrnWSection section, forumIndexDisp disp, int StartPage = 1, bool NewThread = true)
            {
                _Web web = new _Web();
                _Sites.RapidGator rg = new _Sites.RapidGator();
                _TelerikLib tel = new _TelerikLib();
                _Parse.XML xml = new _Parse.XML();
                _Lists lst = new _Lists();
                _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();
                _Parse prs = new _Parse();

                if (NewThread)
                {
                    Thread newThread = new Thread(() => Category_IndexLoop(section, disp, StartPage, false)); // Function To Execute
                    newThread.Name = "Category_IndexLoop_" + section.ToString();
                    newThread.IsBackground = true;
                    newThread.Start();
                }
                else
                {
                    //int StartNum = 0;
                    int Increment = 50;
                    //bool RunIndexLoop = false; string SQLiteDB = "";

                    int i = StartPage; int rgLinksCount = 0;

                    int totalpages = TotalPages(section);

                    bool RunLoop = true;
                    do
                    {
                        string url = NextURL(section, i);  // next url to load

                        string ProgressMsg = "Page " + i;

                        string html = web.DownloadHTML(url);
                        if (html == "" || html.Contains("Could not obtain post"))
                        {
                            //ahk.MsgBox("End of Posts Reached - No HTML Returned");
                            RunLoop = false; break;
                        }


                        // parse forum html index page
                        string xmlPath = "//*[@id=\"pagecontent\"]/table[2]";
                        List<string> Segments = xml.Parse_HTML_XML(html, xmlPath);  // extract sections of text from html xml

                        List<string> Posts = new List<string>();

                        // update progress bar if provide
                        if (disp.ProgressDisp != null) { pro.SetupProgressBar(disp.ProgressDisp, Increment); }

                        int k = 1;
                        foreach (string Seg in Segments)
                        {
                            string Title = "";
                            string TitleURL = "";
                            string PosterName = "";
                            string PosterURL = "";
                            string ImageURL = "";
                            string LastPostDate = "";
                            int PostPageCount = 0;

                            if (disp.ProgressDisp != null) { pro.UpdateProgress(disp.ProgressDisp, section.ToString() + " | Page " + i + "/" + totalpages + " | " + k + "/" + Increment + " URLs Added | " + rgLinksCount.ToString() + " Posts With RGLinks"); k++; }


                            if (!Seg.Contains("porn-w.org/topic")) { continue; }

                            List<string> htmlLines = lst.Text_To_List(Seg, true, true, false);
                            foreach (string htmlline in htmlLines)
                            {
                                if (htmlline.Contains("porn-w.org/topic") && htmlline.Contains("topictitle"))
                                {
                                    string st = htmlline.Replace("</a>", "");
                                    Title = ahk.StringSplit(st, ">", 0, true).Trim();
                                    Title = ahk.Decode(Title);

                                    List<string> pts = ahk.StringSplit_List(htmlline, "\"");
                                    foreach (string pt in pts)
                                    {
                                        if (pt.Contains("porn-w.org/topic")) { TitleURL = pt; break; }
                                    }

                                    string imlin = htmlline.Replace("&quot;", "|");
                                    List<string> ims = ahk.StringSplit_List(imlin, "|");
                                    foreach (string im in ims)
                                    {
                                        if (im.Contains(".jpg")) { ImageURL = im; break; }
                                    }
                                }

                                if (TitleURL != "")  // found url, now found page numbers
                                {
                                    string srch = TitleURL.Replace(".html", "-");

                                    if (htmlline.Contains(srch))
                                    {
                                        List<string> pts = ahk.StringSplit_List(htmlline, "\"");
                                        foreach (string pt in pts)
                                        {
                                            if (pt.Contains(srch) && !pt.Contains("#"))
                                            {
                                                string pgNm = pt.Replace(srch, "");
                                                pgNm = pgNm.Replace(".html", "");

                                                int page = pgNm.ToInt();

                                                if (page > PostPageCount) { PostPageCount = page; }
                                                //ahk.MsgBox("page num " + pgNm);
                                            }
                                        }
                                    }
                                }


                                if (htmlline.Contains("<a class=\"author_name"))
                                {
                                    string hm = htmlline.Replace("</a>&nbsp;", "");
                                    hm = hm.Replace("<a class=\"author_name\" id=\"an", "");
                                    hm = hm.Replace("\">", "|");
                                    PosterURL = ahk.StringSplit(hm, "|", 0);
                                    PosterName = ahk.StringSplit(hm, "|", 1);
                                }

                                if (htmlline.Contains("white-space"))
                                {
                                    string date = htmlline.StringSplit(">", 1);
                                    LastPostDate = date.StringSplit("<", 0).Trim();
                                }


                            }


                            forumIndex obj = new forumIndex();

                            obj.PageName = Title;
                            obj.URL = TitleURL;

                            // convert post count to page #
                            if (PostPageCount != 0) { PostPageCount = PostPageCount / 15; }
                            PostPageCount++;
                            obj.PageCount = PostPageCount;

                            // check to see if first page of post contains rapidgator links
                            bool ContainsLinks = false;
                            string Html = web.DownloadHTML(TitleURL).ToLower();
                            if (Html != "") { if (Html.Contains("rapidgator.net") || Html.Contains("rg.to")) { ContainsLinks = true; rgLinksCount++; } }
                            obj.ContainsLinks = ContainsLinks;


                            // convert last post date to datetime var
                            try { obj.LastPostDate = DateTime.Parse(LastPostDate); }
                            catch { obj.LastPostDate = new DateTime(1900, 1, 1); }

                            obj.SiteSection = section.ToString();
                            obj.ImagePath = ImageURL;
                            obj.UserName = PosterName;
                            obj.UserURL = PosterURL;

                            obj.DateParsed = new DateTime(1900, 1, 1);
                            obj.DateAdded = DateTime.Now;
                            obj.DateModified = DateTime.Now;
                            if (obj.ImagePath == null) { obj.ImagePath = ""; }


                            //ahk.MsgBox(Title + "\n\n" + TitleURL + "\n\n" + ImageURL + "\n\n" + PosterName + "\n" + PosterURL + "\n\nLast Post: " + LastPostDate + "\n\nPost Pages: " + PostPageCount);
                            forumIndex_UpdateInsertSQL(obj); // write sql
                        }

                        i++;
                        indexPageNum += Increment;

                        if (i > TotalPages(section)) { RunLoop = false; }

                        i++;
                    }
                    while (RunLoop);
                }


                if (!NewThread)
                {
                    if (disp.ProgressDisp != null) { pro.UpdateProgress(disp.ProgressDisp, section.ToString() + " | DONE"); }
                    ahk.MsgBox("Finished " + section.ToString() + " Index Import");
                }

                return "";
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

            public struct forumIndexDisp
            {
                public RadTextBox IndexPageNumDisplay { get; set; }
                public RadTextBox CurrentURLDisplay { get; set; }
                public RadTextBox RGLinksCount { get; set; }
                public RadProgressBar ProgressDisp { get; set; }
            }

            public struct forumIndex
            {
                public int ID { get; set; }
                public DateTime LastPostDate { get; set; }

                public string SiteSection { get; set; }
                public string URL { get; set; }
                public string PageName { get; set; }
                public string UserName { get; set; }
                public string UserURL { get; set; }
                public string ImagePath { get; set; }
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
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMADBIndex"].ConnectionString);
                // SqlConnection Conn = new SqlConnection("Server=188.168.188.88;DataBase=LucidMedia;Uid=lucidm;Pwd=pass");
                return conn;
            }

            // Return forumIndex TableName (Full Path)
            public string forumIndex_TableName()
            {
                // populate to return full sql table name
                return "[ADBIndex].[dbo].[PrnW_Index]";
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
                string SQLLine = "Insert Into " + forumIndex_TableName() + " (LastPostDate, ImageURL, SiteSection, URL, PageName, PageCount, DateAdded, DateParsed, ContainsLinks) VALUES (@LastPostDate, @ImagePath, @SiteSection, @URL, @PageName, @PageCount, @DateAdded, @DateParsed, @ContainsLinks)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.URL == null) { obj.URL = ""; }

                cmd2.Parameters.AddWithValue(@"URL", obj.URL.ToString());
                cmd2.Parameters.AddWithValue(@"PageName", obj.PageName.ToString());
                cmd2.Parameters.AddWithValue(@"PageCount", obj.PageCount.ToString());
                cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded.ToString());
                cmd2.Parameters.AddWithValue(@"DateParsed", obj.DateParsed.ToString());
                cmd2.Parameters.AddWithValue(@"ContainsLinks", obj.ContainsLinks.ToString());
                cmd2.Parameters.AddWithValue(@"SiteSection", obj.SiteSection.ToString());
                cmd2.Parameters.AddWithValue(@"ImagePath", obj.ImagePath.ToString());
                cmd2.Parameters.AddWithValue(@"LastPostDate", obj.LastPostDate.ToString());

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
                string SQLLine = "Update " + forumIndex_TableName() + " SET LastPostDate = @LastPostDate, ImageURL = @ImagePath, PageName = @PageName, PageCount = @PageCount, DateAdded = @DateAdded, DateParsed = @DateParsed, SiteSection = @SiteSection, ContainsLinks = @ContainsLinks WHERE URL = @URL";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.URL == null) { obj.URL = ""; }

                cmd2.Parameters.AddWithValue(@"URL", obj.URL.ToString());
                cmd2.Parameters.AddWithValue(@"PageName", obj.PageName.ToString());
                cmd2.Parameters.AddWithValue(@"PageCount", obj.PageCount.ToString());
                cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded.ToString());
                cmd2.Parameters.AddWithValue(@"DateParsed", obj.DateParsed.ToString());
                cmd2.Parameters.AddWithValue(@"SiteSection", obj.SiteSection.ToString());
                cmd2.Parameters.AddWithValue(@"ContainsLinks", obj.ContainsLinks.ToString());
                cmd2.Parameters.AddWithValue(@"ImagePath", obj.ImagePath.ToString());
                cmd2.Parameters.AddWithValue(@"LastPostDate", obj.LastPostDate.ToString());

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

            public forumIndex forumIndex_ReturnSQL(string URL)
            {
                SqlConnection Conn = forumIndex_Conn();
                string SelectLine = "Select [LastPostDate],[ID],[URL],[PageName],[PageCount],[DateAdded],[DateParsed],[ContainsLinks] From " + forumIndex_TableName() + " WHERE [URL] = '" + URL + "'";
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
                        returnObject.LastPostDate = ret["LastPostDate"].ToDateTime();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public List<forumIndex> forumIndex_ReturnSQLList(string Command = "")
            {
                if (Command == "") { Command = "Select * From " + forumIndex_TableName(); }
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
                        returnObject.LastPostDate = ret["LastPostDate"].ToDateTime();
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


            /// <summary>
            /// Number of posts found in a particular section
            /// </summary>
            /// <param name="Section"></param>
            /// <returns></returns>
            public int ReturnSectionPostCountFromDb(string Section)
            {
                string cmd = "select count(ID) FROM " + forumIndex_TableName() + " where SiteSection = '" + Section + "'";
                string count = sql.SQL_Return_Value(forumIndex_Conn(), cmd);
                return count.ToInt();
            }

            // take the number of posts divided by 50 per page = number of pages captured
            public int Return_PagesCapturedCount(string Section)
            {
                int posts = ReturnSectionPostCountFromDb(Section);

                int remainder = posts % 50; // remainder when dividing by 50
                int totalWithOutRemainder = posts - remainder;
                int pages = totalWithOutRemainder / 50;
                return pages;
            }


            #endregion

            #endregion


            #endregion


            // ==== v1 parse functions -- may be useful ? 

            #region === PW.org SiteParse ===

            public List<string> PW_IndexParse(string URL) // pulls all the ids from index page needed to load each post (50 posts per page)
            {
                _Web web = new _Web();
                string html = web.DownloadHTML(URL);
                if (html == "") { ahk.MsgBox("No HTML Returned"); return null; }

                List<string> pwlinks = Regex_PWLinks(html);


                //foreach(string line in pwlinks)
                //{
                //    LinkLines.Add("http://www.porn-w.org/topic" + line + ".html");
                //}
                //ahk.MsgBox("Found " + LinkLines.Count + " Links");

                return pwlinks;
            }


            public List<string> Regex_PWLinks(string HTML)
            {
                _Lists lst = new _Lists();

                List<string> matches = new List<string>();


                Regex ItemRegex = new Regex("rn-w.org/topic(.*?).html", RegexOptions.IgnoreCase);
                foreach (Match ItemMatch in ItemRegex.Matches(HTML))
                {

                    string val = ItemMatch.Value;
                    val = val.Replace("rn-w.org/topic", "");
                    if (val.Contains("-"))
                    {
                        val = ahk.StringSplit(val, "-", 0);
                    }

                    val = val.Replace(".html", "");

                    if (!lst.InList(matches, val))
                    {
                        matches.Add(val);
                    }

                    //ahk.MsgBox(val);
                }

                ahk.MsgBox(matches.Count + " Links Found");

                return matches;
            }



            #endregion



        }
    }

}

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
        public class PBBForum
        {
            _AHK ahk = new _AHK();
            _Database.SQL sql = new _Database.SQL();
            _Database.SQLite sqlite = new _Database.SQLite();

            public string NextPbbURL(PbbSection Section, int StartPage = 0)
            {
                int StartNum = 0;
                if (StartPage != 0) { StartNum = StartPage * 50; }  // determine starting url number

                string name = Section.ToString();
                string url = "";

                if (name == "Movies") { url = "http://www.pornbb.org/xxx-movies-f1-" + StartNum + ".html"; }
                if (name == "Videos") { url = "http://www.pornbb.org/xxx-videos-f3-" + StartNum + ".html"; }
                if (name == "MegaMovieThread") { url = "http://www.pornbb.org/xxx-movie-mega-threads-f46-" + StartNum + ".html"; }
                if (name == "MegaVideoThread") { url = "http://www.pornbb.org/xxx-video-mega-threads-f2-" + StartNum + ".html"; }
                if (name == "HighDef") { url = "http://www.pornbb.org/xxx-high-definition-videos-f42-" + StartNum + ".html"; }
                if (name == "Celebrity") { url = "http://www.pornbb.org/celebrity-xxx-videos-f29-" + StartNum + ".html"; }
                if (name == "Amateur") { url = "http://www.pornbb.org/all-other-amateur-xxx-videos-f22-" + StartNum + ".html"; }
                if (name == "Vintage") { url = "http://www.pornbb.org/vintage-videos-movies-f38-" + StartNum + ".html"; }
                if (name == "JAV") { url = "http://www.pornbb.org/jav-videos-and-movies-f47-" + StartNum + ".html"; }
                if (name == "WebCam") { url = "http://www.pornbb.org/webcam-girls-f48-" + StartNum + ".html"; }
                if (name == "VR") { url = "http://www.pornbb.org/virtual-reality-and-3d-porn-f49-" + StartNum + ".html"; }
                if (name == "ModelVideos") { url = "http://www.pornbb.org/erotic-and-porn-models-videos-f45-" + StartNum + ".html"; }
                if (name == "ModelPics") { url = "http://www.pornbb.org/erotic-and-porn-models-pictures-f51-" + StartNum + ".html"; }
                if (name == "SoftcorePics") { url = "http://www.pornbb.org/softcore-pictures-f5-" + StartNum + ".html"; }
                if (name == "HardcorePics") { url = "http://www.pornbb.org/hardcore-pictures-f4-" + StartNum + ".html"; }
                if (name == "PicMegaThread") { url = "http://www.pornbb.org/picture-mega-threads-f26-" + StartNum + ".html"; }
                if (name == "CelebPics") { url = "http://www.pornbb.org/celebrity-pictures-f15-" + StartNum + ".html"; }
                if (name == "AmateurPics") { url = "http://www.pornbb.org/all-other-amateur-pictures-f23-" + StartNum + ".html"; }
                if (name == "Stories") { url = "http://www.pornbb.org/xxx-stories-f16-" + StartNum + ".html"; }
                if (name == "Games") { url = "http://www.pornbb.org/xxx-games-f31-" + StartNum + ".html"; }
                if (name == "Magazines") { url = "http://www.pornbb.org/xxx-magazines-f39-" + StartNum + ".html"; }
                if (name == "BabeID") { url = "http://www.pornbb.org/babe-amp-video-identification-f34-" + StartNum + ".html"; }
                if (name == "Other") { url = "http://www.pornbb.org/other-downloads-f17-" + StartNum + ".html"; }
                return url;
            }

            //[DefaultValue(Cancelled)]
            public enum PbbSection
            {
                Movies, Videos, MegaMovieThread, MegaVideoThread, HighDef, Celebrity, Amateur, Vintage, JAV, WebCam, VR, ModelVideos,
                ModelPics, SoftcorePics, HardcorePics, PicMegaThread, CelebPics, AmateurPics, Stories, Games, Magazines, BabeID, Other
            }

            public PbbSection ToSection(string SectionName)
            {
                if (SectionName == "Amateur") { return PbbSection.Amateur; }
                if (SectionName == "AmateurPics") { return PbbSection.AmateurPics; }
                if (SectionName == "BabeID") { return PbbSection.BabeID; }
                if (SectionName == "CelebPics") { return PbbSection.CelebPics; }
                if (SectionName == "Celebrity") { return PbbSection.Celebrity; }
                if (SectionName == "Games") { return PbbSection.Games; }
                if (SectionName == "HardcorePics") { return PbbSection.HardcorePics; }
                if (SectionName == "HighDef") { return PbbSection.HighDef; }
                if (SectionName == "JAV") { return PbbSection.JAV; }
                if (SectionName == "Magazines") { return PbbSection.Magazines; }
                if (SectionName == "MegaMovieThread") { return PbbSection.MegaMovieThread; }
                if (SectionName == "MegaVideoThread") { return PbbSection.MegaVideoThread; }
                if (SectionName == "ModelPics") { return PbbSection.ModelPics; }
                if (SectionName == "ModelVideos") { return PbbSection.ModelVideos; }
                if (SectionName == "Movies") { return PbbSection.Movies; }
                if (SectionName == "Other") { return PbbSection.Other; }
                if (SectionName == "PicMegaThread") { return PbbSection.PicMegaThread; }
                if (SectionName == "SoftcorePics") { return PbbSection.SoftcorePics; }
                if (SectionName == "Stories") { return PbbSection.Stories; }
                if (SectionName == "Videos") { return PbbSection.Videos; }
                if (SectionName == "Vintage") { return PbbSection.Vintage; }
                if (SectionName == "VR") { return PbbSection.VR; }
                if (SectionName == "WebCam") { return PbbSection.WebCam; }

                return PbbSection.Other;
            }


            public int TotalPages(PbbSection SectionName)
            {
                if (SectionName.ToString() == "Amateur") { return 42; }
                if (SectionName.ToString() == "AmateurPics") { return 22; }
                if (SectionName.ToString() == "BabeID") { return 350; }
                if (SectionName.ToString() == "CelebPics") { return 916; }
                if (SectionName.ToString() == "Celebrity") { return 22; }
                if (SectionName.ToString() == "Games") { return 17; }
                if (SectionName.ToString() == "HardcorePics") { return 1342; }
                if (SectionName.ToString() == "HighDef") { return 53; }
                if (SectionName.ToString() == "JAV") { return 9; }
                if (SectionName.ToString() == "Magazines") { return 72; }
                if (SectionName.ToString() == "MegaMovieThread") { return 22; }
                if (SectionName.ToString() == "MegaVideoThread") { return 207; }
                if (SectionName.ToString() == "ModelPics") { return 16; }
                if (SectionName.ToString() == "ModelVideos") { return 35; }
                if (SectionName.ToString() == "Movies") { return 1843; }
                if (SectionName.ToString() == "Other") { return 1; }
                if (SectionName.ToString() == "PicMegaThread") { return 66; }
                if (SectionName.ToString() == "SoftcorePics") { return 1137; }
                if (SectionName.ToString() == "Stories") { return 3; }
                if (SectionName.ToString() == "Videos") { return 16045; }
                if (SectionName.ToString() == "Vintage") { return 11; }
                if (SectionName.ToString() == "VR") { return 3; }
                if (SectionName.ToString() == "WebCam") { return 6; }
                return 0;
            }

            public List<string> PbbSections()
            {
                List<string> items = new List<string> { "Movies","Videos","MegaMovieThread","MegaVideoThread","HighDef","Celebrity","Amateur","Vintage","JAV","WebCam","VR","ModelVideos",
                "ModelPics","SoftcorePics","HardcorePics","PicMegaThread","CelebPics","AmateurPics","Stories","Games","Magazines","BabeID","Other"  };

                _Lists lst = new _Lists();
                return lst.SortList(items);
            }


            int indexPageNum = 0;

            public string Category_IndexLoop(PbbSection section, forumIndexDisp disp, int StartPage = 1, bool NewThread = true)
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
                    int StartNum = 0; int Increment = 50; bool RunIndexLoop = false; string SQLiteDB = "";

                    int i = StartPage; int rgLinksCount = 0;

                    int totalpages = TotalPages(section);

                    //indexPageNum = StartNum;
                    bool RunLoop = true;
                    do
                    {
                        string url = NextPbbURL(section, i);  // next url to load

                        string ProgressMsg = "Page " + i;

                        //if (disp.IndexPageNumDisplay != null) { tel.Update(disp.IndexPageNumDisplay, i); }

                        string html = web.DownloadHTML(url);
                        if (html == "")
                        {
                            //ahk.MsgBox("End of Posts Reached - No HTML Returned");
                            RunLoop = false; break;
                        }

                        if (html.Contains("Could not obtain post"))
                        {
                            //ahk.MsgBox("End of Posts Reached");
                            RunLoop = false; break;
                        }

                        int HaveLinksCount = 0;

                        //string xmlPath = "//body/div/div/div/div/div";
                        string xmlPath = "/html/body/div/div/div[2]/div[7]";

                        List<string> lines = xml.Parse_HTML_XML(html, xmlPath);  // extract sections of text from html xml

                        List<string> Posts = new List<string>();

                        // update progress bar if provide
                        if (disp.ProgressDisp != null) { pro.SetupProgressBar(disp.ProgressDisp, Increment); }

                        string Title = "";
                        string TitleURL = "";
                        string PosterName = "";
                        string PosterURL = "";
                        string ImageURL = "";
                        string LastPostDate = "";

                        int k = 1;
                        foreach (string line in lines)
                        {
                            if (line.Contains("topictitle"))
                            {
                                // update progress bar if provide
                                if (disp.ProgressDisp != null) { pro.UpdateProgress(disp.ProgressDisp, section.ToString() + " | Page " + i + "/" + totalpages + " | " + k + "/" + Increment + " URLs Added | " + rgLinksCount.ToString() + " Posts With RGLinks"); k++; }

                                if (line.Contains("<h2>")) { Title = prs.XML_Text(line, "<h2>"); }

                                if (line.Contains("<a href=") && line.Contains("member") && line.Contains(".html"))
                                {
                                    string rLine = line.Replace("</a>", "\"");
                                    List<string> parts = ahk.StringSplit_List(rLine, "\""); bool MemberURLFound = false; int AfterMemberURL = 0;
                                    foreach (string part in parts)
                                    {
                                        if (part.Contains("member") && line.Contains(".html"))
                                        {
                                            PosterURL = part; MemberURLFound = true; continue;
                                        }

                                        if (MemberURLFound)
                                        {
                                            AfterMemberURL++;

                                            if (AfterMemberURL == 3)
                                            {
                                                PosterName = part; break;
                                            }
                                        }
                                    }
                                }

                                if (line.Contains("hidden-sm hidden-xs") && line.Contains("</span>"))
                                {
                                    string dateline = line.Replace(">", ">\n");

                                    List<string> sts = lst.Text_To_List(dateline);
                                    foreach (string st in sts)
                                    {
                                        if (st.Contains(":"))
                                        {
                                            if (st.Contains("display:")) { continue; }
                                            if (st.Contains("http:") || st.Contains("https:")) { continue; }
                                            if (st.Contains("By:")) { continue; }

                                            if (LastPostDate == "")
                                            {
                                                LastPostDate = st.Replace("</span>", "");
                                                //ahk.MsgBox(LastPostDate);
                                            }
                                        }
                                    }


                                }


                                if (line.Contains(".jpg"))
                                {
                                    //>
                                    string rLine = line.Replace("<", ">");
                                    List<string> parts = ahk.StringSplit_List(rLine, ">");
                                    foreach (string part in parts)
                                    {
                                        if (part.Contains(".jpg"))
                                        {
                                            ImageURL = part;
                                        }
                                    }
                                }

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

                                        obj.SiteSection = section.ToString();

                                        obj.ImagePath = ImageURL;
                                        obj.UserName = PosterName;
                                        obj.UserURL = PosterURL;

                                        obj.PageCount = pagenumText.ToInt();

                                        string pageUrl = ln.Replace("<a href=\"", "");
                                        pageUrl = pageUrl.Replace(".html", ".html|");
                                        pageUrl = pageUrl.Replace("<h2>", "|");
                                        pageUrl = pageUrl.Replace("</h2>", "|");

                                        string PageURL = ahk.StringSplit(pageUrl, "|", 0);
                                        string PageName = ahk.StringSplit(pageUrl, "|", 2);

                                        obj.PageName = PageName.Decode();
                                        obj.URL = "https://www.pornbb.org/" + PageURL;

                                        //if (disp.CurrentURLDisplay != null) { tel.Update(disp.CurrentURLDisplay, obj.URL); }

                                        string Html = web.DownloadHTML(obj.URL).ToLower();
                                        if (Html == "")
                                        {
                                            //ahk.MsgBox(obj.URL + "\n\nNo HTML Returned");
                                            continue;
                                        }
                                        bool ContainsLinks = false;
                                        if (Html.Contains("rapidgator.net") || Html.Contains("rg.to")) { ContainsLinks = true; }

                                        obj.ContainsLinks = ContainsLinks;
                                        obj.DateParsed = new DateTime(1900, 1, 1);
                                        obj.DateAdded = DateTime.Now;
                                        obj.DateModified = DateTime.Now;

                                        if (obj.ImagePath == null) { obj.ImagePath = ""; }

                                        if (obj.ContainsLinks) { rgLinksCount++; }

                                        obj.LastPostDate = LastPostDate.ToDateTime();

                                        //tel.Update(disp.RGLinksCount, rgLinksCount.ToString() + " Posts With Links");

                                        forumIndex_UpdateInsertSQL(obj); // write sql
                                                                            //forumIndex_UpdateInsert(obj, SQLiteDB);


                                        LastPostDate = "";  // reset before next loop

                                        //ahk.MsgBox(PageName + "\n\n" + "Total Pages: " + pagenumText + "\n\n" + obj.URL + "\n\nContains RGLinks = " + ContainsLinks.ToString());
                                        break;
                                    }
                                }

                                //ahk.MsgBox(line);
                            }

                            //RunLoop = ForumLoopEnabled;
                        }

                        indexPageNum += Increment;
                        //if (disp.IndexPageNumDisplay != null) { tel.Update(disp.IndexPageNumDisplay, indexPageNum.ToString()); }
                        //sqlite.Setting("ForumIndexPageNum", indexPageNum.ToString());

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
                public DateTime LastPostDate { get; set; }
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
                return "[ADBIndex].[dbo].[PBB_Index]";
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


            /// <summary>
            /// Set Flag to Indicate Page Has been Parsed for Valid Links in Post Pages
            /// </summary>
            /// <param name="URL"></param>
            /// <returns></returns>
            public bool forumIndex_UpdateAsParsed(string URL)
            {
                SqlConnection Conn = forumIndex_Conn();
                string SQLLine = "Update " + forumIndex_TableName() + " SET ID = @ID WHERE URL = @URL";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);

                cmd2.Parameters.AddWithValue(@"URL", URL);
                cmd2.Parameters.AddWithValue(@"ID", "1");


                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        forumIndex_UpdateAsParsed(URL);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        forumIndex_UpdateAsParsed(URL);
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
                string SelectLine = "Select [LastPostDate],[ID],[URL],[PageName],[PageCount],[DateAdded],[DateParsed],[ContainsLinks] From " + forumIndex_TableName() + " WHERE ID = '" + ID + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                forumIndex returnObject = new forumIndex();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.LastPostDate = ret["LastPostDate"].ToDateTime();
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

            public List<forumIndex> forumIndex_ReturnSQLList(string Command = "where ContainsLinks = '1' and PageCount != '0'")
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
                        returnObject.LastPostDate = ret["LastPostDate"].ToDateTime();
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

    }
}

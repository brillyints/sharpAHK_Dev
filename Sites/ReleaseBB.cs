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
        public class ReleaseBB
        {
            _AHK ahk = new _AHK();
            _Database.SQL sql = new _Database.SQL();
            _Sites.RapidGator rg = new RapidGator();
            _Sites.Imdb imdb = new Imdb();
            _Lists lst = new _Lists();
            _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();

            public void Download_ReleaseBB_MovieIndex(int StartPage = 1, int LastPage = 1366)
            {
                _Parse.XML xml = new _Parse.XML();
                _Lists lst = new _Lists();
                _Sites.Imdb imdb = new Imdb();

                string startURL = "http://rlsbb.ru/category/movies/page/";
                int LastPageNum = LastPage;

                int pageNum = StartPage;
                do
                {
                    string URL = startURL + pageNum + "/";
                    string HTML = ahk.Download_HTML(URL);

                    string xmlPath = "//*[@id=\"contentArea\"]";

                    List<string> segs = xml.Parse_HTML_XML(HTML, xmlPath);  // extract sections of text from html xml

                    int segNum = 1;
                    foreach (string seg in segs)
                    {
                        ReleaseBBMovies obj = new ReleaseBBMovies();

                        // skip segs on page that aren't posts
                        if (segNum == 1) { segNum++; continue; }
                        if (segNum == 2) { segNum++; continue; }
                        if (segNum == 13) { segNum++; continue; }
                        if (segNum == 14) { segNum++; continue; }

                        //ahk.MsgBox(segNum + " | " + seg);
                        segNum++;

                        string PostTitle = "";
                        string PostURL = "";
                        string IMDbID = "";

                        List<string> links = imdb.Regex_IMDbLinks(seg);

                        if (links.Count > 0)
                        {
                            IMDbID = imdb.IMDb_ID_FromURL(links[0]);
                        }

                        // parse index page segment 
                        List<string> lines = lst.Text_To_List(seg, true, true, false);
                        foreach (string line in lines)
                        {
                            if (line.Contains("postTitle"))
                            {
                                PostURL = line.Replace("<h2 class=\"postTitle\"><span></span><a href=\"", "");
                                PostTitle = ahk.StringSplit(PostURL, ">", 1);
                                PostTitle = ahk.StringSplit(PostTitle, "<", 0);
                                PostURL = ahk.StringSplit(PostURL, "\"", 0);
                                //ahk.MsgBox("IMDB: " + IMDbID + "\n\n" + PostTitle + "\n\n" + PostURL);
                            }
                        }

                        obj.IMDbID = IMDbID;
                        obj.PostTitle = PostTitle;
                        obj.PostURL = PostURL;

                        bool Updated = ReleaseBBMovies_UpdateInsert_IndexEntry(obj);

                    }

                    pageNum++;
                }
                while (pageNum <= LastPageNum);

            }


            /// <summary>
            /// Populate Missing RG Links / IMDb IDs using Post URLs
            /// </summary>
            public void Backfill_ReleaseBB_Links(RadProgressBar Bar = null, bool NewThread = true)
            {
                if (NewThread)
                {
                    Thread newThread = new Thread(() => Backfill_ReleaseBB_Links(Bar, false)); // Function To Execute
                    newThread.Name = "Populate Missing Actor Info";
                    newThread.IsBackground = true;
                    newThread.Start();
                }
                else
                {
                    List<ReleaseBBMovies> pages = ReleaseBBMovies_ReturnSQLList();

                    if (Bar != null) { pro.SetupProgressBar(Bar, pages.Count); } // setup progress bar if passed in

                    int i = 1;
                    foreach (ReleaseBBMovies page in pages)
                    {
                        string html = ahk.Download_HTML(page.PostURL);

                        List<string> links = rg.Regex_RGLinks(html);

                        string IMDbID = imdb.IMDbFromHTML(html);

                        string RGLinks = lst.List_To_String(links, "\n");

                        ReleaseBBMovies obj = new ReleaseBBMovies();
                        obj.PostURL = page.PostURL;
                        obj.IMDbID = IMDbID;
                        obj.RapidGator = RGLinks;

                        ReleaseBBMovies_UpdateSQL_Links(obj);

                        if (Bar != null) { pro.UpdateProgress(Bar, i + "/" + pages.Count); }
                        i++;
                    }
                }
            }


            #region === ReleaseBBMovies FUNCTIONS ===

            #region ===== ReleaseBBMovies Object =====

            public struct ReleaseBBMovies
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
            //  Fix illegal characters before Sql/Sqlite Db Inserts
            public ReleaseBBMovies ReleaseBBMovies_FixChars(ReleaseBBMovies ToFix)
            {
                ReleaseBBMovies Fixed = new ReleaseBBMovies();

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
            public bool ReleaseBBMovies_Changed(ReleaseBBMovies OldVal, ReleaseBBMovies NewVal)
            {
                ReleaseBBMovies diff = new ReleaseBBMovies();
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
            public ReleaseBBMovies ReleaseBBMovies_Diff(ReleaseBBMovies OldVal, ReleaseBBMovies NewVal)
            {
                ReleaseBBMovies diff = new ReleaseBBMovies();
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
            public List<string> ReleaseBBMovies_DiffList(ReleaseBBMovies OldVal, ReleaseBBMovies NewVal)
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

            // Generate XML String From Object
            public string ReleaseBBMovies_ToXML(ReleaseBBMovies obj)
            {
                string XMLString = "";
                XMLString = XMLString + "<MovieName>" + obj.MovieName + "</MovieName>";
                XMLString = XMLString + "<IMDbID>" + obj.IMDbID + "</IMDbID>";
                XMLString = XMLString + "<Year>" + obj.Year + "</Year>";
                XMLString = XMLString + "<PostTitle>" + obj.PostTitle + "</PostTitle>";
                XMLString = XMLString + "<PostURL>" + obj.PostURL + "</PostURL>";
                XMLString = XMLString + "<ImageURL>" + obj.ImageURL + "</ImageURL>";
                XMLString = XMLString + "<RapidGator>" + obj.RapidGator + "</RapidGator>";
                XMLString = XMLString + "<LinksChecked>" + obj.LinksChecked + "</LinksChecked>";
                XMLString = XMLString + "<InCollection>" + obj.InCollection + "</InCollection>";
                XMLString = XMLString + "<DateAdded>" + obj.DateAdded + "</DateAdded>";
                XMLString = XMLString + "<Cateogry>" + obj.Cateogry + "</Cateogry>";
                return XMLString;
            }




            #endregion

            #region ===== ReleaseBBMovies SQL Functions =====

            // Return ReleaseBBMovies SQL Connection String
            public SqlConnection ReleaseBBMovies_Conn()
            {
                // populate sql connection
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMLucidMedia"].ConnectionString);
                // SqlConnection Conn = new SqlConnection("Server=188.168.188.88;DataBase=LucidMedia;Uid=lucidm;Pwd=pass");
                return conn;
            }

            // Return ReleaseBBMovies TableName (Full Path)
            public string ReleaseBBMovies_TableName()
            {
                // populate to return full sql table name
                return "[LucidMedia].[dbo].[ReleaseBBMovies]";
            }

            // Generate SQL Table
            public bool ReleaseBBMovies_CreateSQLTable()
            {
                SqlConnection Conn = ReleaseBBMovies_Conn();
                string CreateTableLine = "CREATE TABLE [ReleaseBBMovies](";
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

                _Database.SQL sql = new _Database.SQL();

                return sql.WriteDataRecord(Conn, CreateTableLine);
            }

            public bool ReleaseBBMovies_InsertSQL(ReleaseBBMovies obj)
            {
                SqlConnection Con = ReleaseBBMovies_Conn();
                string SQLLine = "Insert Into " + ReleaseBBMovies_TableName() + " (MovieName, IMDbID, Year, PostTitle, PostURL, ImageURL, RapidGator, LinksChecked, InCollection, DateAdded, Cateogry) VALUES (@MovieName, @IMDbID, @Year, @PostTitle, @PostURL, @ImageURL, @RapidGator, @LinksChecked, @InCollection, @DateAdded, @Cateogry)";
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
                cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded.ToString());
                cmd2.Parameters.AddWithValue(@"Cateogry", obj.Cateogry.ToString());
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        ReleaseBBMovies_InsertSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        ReleaseBBMovies_InsertSQL(obj);
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

            public bool ReleaseBBMovies_UpdateSQL(ReleaseBBMovies obj)
            {
                SqlConnection Conn = ReleaseBBMovies_Conn();
                string SQLLine = "Update " + ReleaseBBMovies_TableName() + " SET MovieName = @MovieName, IMDbID = @IMDbID, Year = @Year, PostTitle = @PostTitle, PostURL = @PostURL, ImageURL = @ImageURL, RapidGator = @RapidGator, LinksChecked = @LinksChecked, InCollection = @InCollection, DateAdded = @DateAdded, Cateogry = @Cateogry WHERE ID = @ID";
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
                cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded.ToString());
                cmd2.Parameters.AddWithValue(@"Cateogry", obj.Cateogry.ToString());
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        ReleaseBBMovies_UpdateSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        ReleaseBBMovies_UpdateSQL(obj);
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


            public bool ReleaseBBMovies_UpdateSQL_Links(ReleaseBBMovies obj)
            {
                SqlConnection Conn = ReleaseBBMovies_Conn();
                string SQLLine = "Update " + ReleaseBBMovies_TableName() + " SET IMDbID = @IMDbID, RapidGator = @RapidGator, DateAdded = @DateAdded WHERE PostURL = @PostURL";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);

                if (obj.IMDbID == null) { obj.IMDbID = ""; }
                if (obj.PostURL == null) { obj.PostURL = ""; }
                if (obj.RapidGator == null) { obj.RapidGator = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = ""; }

                
                cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID.ToString());
                cmd2.Parameters.AddWithValue(@"PostURL", obj.PostURL.ToString());
                cmd2.Parameters.AddWithValue(@"RapidGator", obj.RapidGator.ToString());
                cmd2.Parameters.AddWithValue(@"DateAdded", DateTime.Now.ToString());
                
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        ReleaseBBMovies_UpdateSQL_Links(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        ReleaseBBMovies_UpdateSQL_Links(obj);
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


            public bool ReleaseBBMovies_UpdateInsert_IndexEntry(ReleaseBBMovies obj)
            {
                bool Updated = ReleaseBBMovies_UpdateSQL_IndexEntry(obj);  // try to update record first
                if (!Updated) { ReleaseBBMovies_InsertSQL_IndexEntry(obj); }  // if unable to update, insert new record
                return Updated;
            }
            public bool ReleaseBBMovies_InsertSQL_IndexEntry(ReleaseBBMovies obj)
            {
                SqlConnection Con = ReleaseBBMovies_Conn();
                string SQLLine = "Insert Into " + ReleaseBBMovies_TableName() + " (PostTitle, PostURL, IMDbID, DateAdded) VALUES (@PostTitle, @PostURL, @IMDbID, @DateAdded)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);

                if (obj.IMDbID == null) { obj.IMDbID = ""; }
                if (obj.PostTitle == null) { obj.PostTitle = ""; }
                if (obj.PostURL == null) { obj.PostURL = ""; }

                cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID.ToString());
                cmd2.Parameters.AddWithValue(@"PostTitle", obj.PostTitle.ToString());
                cmd2.Parameters.AddWithValue(@"PostURL", obj.PostURL.ToString());
                cmd2.Parameters.AddWithValue(@"DateAdded", DateTime.Now.ToString());

                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        ReleaseBBMovies_InsertSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        ReleaseBBMovies_InsertSQL(obj);
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
            public bool ReleaseBBMovies_UpdateSQL_IndexEntry(ReleaseBBMovies obj)
            {
                SqlConnection Conn = ReleaseBBMovies_Conn();
                string SQLLine = "Update " + ReleaseBBMovies_TableName() + " SET IMDbID = @IMDbID, PostTitle = @PostTitle, PostURL = @PostURL WHERE PostURL = @PostURL";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);

                if (obj.IMDbID == null) { obj.IMDbID = ""; }
                if (obj.PostTitle == null) { obj.PostTitle = ""; }
                if (obj.PostURL == null) { obj.PostURL = ""; }

                cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID.ToString());
                cmd2.Parameters.AddWithValue(@"PostTitle", obj.PostTitle.ToString());
                cmd2.Parameters.AddWithValue(@"PostURL", obj.PostURL.ToString());

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        ReleaseBBMovies_UpdateSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        ReleaseBBMovies_UpdateSQL(obj);
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

            public bool ReleaseBBMovies_UpdateLinks(ReleaseBBMovies obj)
            {
                SqlConnection Conn = ReleaseBBMovies_Conn();
                string SQLLine = "Update " + ReleaseBBMovies_TableName() + " SET RapidGator = @RapidGator WHERE PostURL = @PostURL";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);

                if (obj.IMDbID == null) { obj.IMDbID = ""; }
                if (obj.PostTitle == null) { obj.PostTitle = ""; }
                if (obj.PostURL == null) { obj.PostURL = ""; }

                cmd2.Parameters.AddWithValue(@"RapidGator", obj.RapidGator);
                cmd2.Parameters.AddWithValue(@"PostURL", obj.PostURL.ToString());

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        ReleaseBBMovies_UpdateSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        ReleaseBBMovies_UpdateSQL(obj);
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

            public bool ReleaseBBMovies_UpdateInsert(ReleaseBBMovies obj)
            {
                SqlConnection Conn = ReleaseBBMovies_Conn();
                bool Updated = ReleaseBBMovies_UpdateSQL(obj);  // try to update record first
                if (!Updated) { Updated = ReleaseBBMovies_InsertSQL(obj); }  // if unable to update, insert new record
                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool ReleaseBBMovies_UpdateIfPopulated(ReleaseBBMovies obj, string ID = "")
            {
                SqlConnection Conn = ReleaseBBMovies_Conn();
                string SQLcmd = "Update " + ReleaseBBMovies_TableName() + " SET ";
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

            public ReleaseBBMovies ReleaseBBMovies_ReturnSQL(string ID = "")
            {
                SqlConnection Conn = ReleaseBBMovies_Conn();
                string SelectLine = "Select [MovieName],[IMDbID],[Year],[PostTitle],[PostURL],[ImageURL],[RapidGator],[LinksChecked],[InCollection],[DateAdded],[Cateogry] From " + ReleaseBBMovies_TableName() + " WHERE ID = '" + ID + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                ReleaseBBMovies returnObject = new ReleaseBBMovies();
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

            public List<ReleaseBBMovies> ReleaseBBMovies_ReturnSQLList(string Command = "")
            {
                if (Command == "") { Command = "Select * From " + ReleaseBBMovies_TableName() + " where InCollection = '0' and RapidGator != ''"; }
                SqlConnection Conn = ReleaseBBMovies_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<ReleaseBBMovies> ReturnList = new List<ReleaseBBMovies>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        ReleaseBBMovies returnObject = new ReleaseBBMovies();
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


        }

    }
}

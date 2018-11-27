using System.Windows.Forms;
using sharpAHK;
using sharpAHK_Dev;
using AHKExpressions;
using TelerikExpressions;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading;
using Telerik.WinControls.UI;
using System.Data;
using System.Collections.Generic;
using System;
using System.IO;

namespace sharpAHK_Dev
{
    public partial class ADBSites
    {
        public class prnChill
        {
            #region === Startup ===

            //_Parse.XML xml = new _Parse.XML();
            //_AHK ahk = new _AHK();
            //_Database.SQL sql = new _Database.SQL();
            //_Lists lst = new _Lists();
            //_Parse prs = new _Parse();
            ////_Images img = new _Images();
            //_TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();
            //_TelerikLib tel = new _TelerikLib();
            ////_Apps.Chrome cr = new _Apps.Chrome();
            //sharpAHK_Dev._Threads thr = new sharpAHK_Dev._Threads();
            //_TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            //IAFD iafd = new IAFD();
            //_Web.ADBSites.PBBForum pbb = new _Web.ADBSites.PBBForum();
            //_Web.ADBSites.PRNWForum prnw = new _Web.ADBSites.PRNWForum();

            #endregion


            public void Download_Site(int startPage = 1, int LastPage = 230, bool SkipExisting = true, RadProgressBar Bar = null, RadProgressBar Bar2 = null, bool NewThread = true)
            {
                _AHK ahk = new _AHK();
                _Lists lst = new _Lists();
                _Sites.RapidGator rg = new _Sites.RapidGator();
                _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();

                if (NewThread)
                {
                    Thread newThread = new Thread(() => Download_Site(startPage, LastPage, SkipExisting, Bar, Bar2, false)); // Function To Execute
                    newThread.IsBackground = true;
                    newThread.Start();
                }
                else
                {
                    int pageNum = startPage;
                    do
                    {
                        int postNum = 0;
                        string html = ahk.Download_HTML("http://pornchil.com/page/" + pageNum + "/");
                        List<string> lines = lst.Text_To_List(html, true, true, false);

                        if (Bar != null) { pro.SetupProgressBar(Bar, 10); }  // # of posts/page

                        foreach (string line in lines)
                        {
                            if (line.Contains("<h1 class=\"entry-title\"><a href="))
                            {
                                PrnChill chill = new PrnChill();
                                //ahk.MsgBox(line);

                                string Line = line.Replace("<h1 class=\"entry-title\"><a href=\"", "");
                                chill.PostURL = ahk.StringSplit(Line, "\"", 0);
                                chill.PostName = ahk.StringSplit(Line, ">", 1);
                                chill.PostName = ahk.StringSplit(chill.PostName, "<", 0);

                                if (SkipExisting)
                                {
                                    if (AlreadyParsed(chill.PostURL)) { continue; }  // check to see if this post already has an entry, if so skip
                                }


                                if (Bar != null) { postNum++; pro.UpdateProgress(Bar, postNum + "/10"); }  // # of posts remaining

                                string postHTML = ahk.Download_HTML(chill.PostURL);
                                List<string> links = rg.Regex_RGLinks(postHTML);

                                if (links.Count > 0)
                                {
                                    if (Bar != null) { pro.ProgressText(Bar, postNum + "/10 | Verifying Links"); }  // # of posts remaining

                                    List<_Sites.RapidGator.RGInfo> checkedLinks = rg.RapidGator_BatchCheck(links, true, Bar2); // batch check list of rg links

                                    string rglinks = ""; int goodLinkCount = 0;
                                    foreach (_Sites.RapidGator.RGInfo link in checkedLinks)
                                    {
                                        if (rglinks == "") { rglinks = link.FileURL + "|" + link.FileSize; goodLinkCount++; }
                                        else { rglinks = rglinks + "\n" + link.FileURL + "|" + link.FileSize; goodLinkCount++; }
                                    }

                                    chill.Links = rglinks; // online rg links with  filepath|filesize return
                                    chill.LinkCount = goodLinkCount;

                                    chill.LinkCheckDate = DateTime.Now.ToString();
                                }

                                chill.InCollection = "false";

                                List<string> images = rg.JpgImageLinks(postHTML);

                                if (Bar != null) { pro.ProgressText(Bar, postNum + "/10 | Downloading Images (" + images.Count + ")"); }  // # of posts remaining

                                if (images.Count > 0)
                                {
                                    // create save dir
                                    string saveDir = ahk.AppDir() + "\\PrnChillPosts";
                                    ahk.FileCreateDir(saveDir);
                                    saveDir = saveDir + "\\" + chill.PostName;
                                    ahk.FileCreateDir(saveDir);

                                    chill.ImageDir = saveDir;

                                    int imgNum = 1;
                                    foreach (string image in images)
                                    {
                                        ahk.Download_File(image, saveDir + "\\" + imgNum + ".jpg", true); imgNum++;
                                    }
                                }

                                PrnChill_UpdateInsert(chill);
                            }
                        }

                        pageNum++;

                    } while (pageNum <= LastPage);
                }




            }


            #region === PrnChill FUNCTIONS ===

            #region ===== PrnChill Object =====

            public struct PrnChill
            {
                public string PostName { get; set; }
                public string PostURL { get; set; }
                public string Links { get; set; }
                public string DateAdded { get; set; }
                public string LinkCheckDate { get; set; }
                public string InCollection { get; set; }
                public string ImageDir { get; set; }
                public int LinkCount { get; set; }

            }
            //  Fix illegal characters before Sql/Sqlite Db Inserts
            public PrnChill PrnChill_FixChars(PrnChill ToFix)
            {
                PrnChill Fixed = new PrnChill();

                Fixed.PostName = ToFix.PostName.Replace("'", "''");
                Fixed.PostURL = ToFix.PostURL.Replace("'", "''");
                Fixed.Links = ToFix.Links.Replace("'", "''");
                Fixed.DateAdded = ToFix.DateAdded.Replace("'", "''");
                Fixed.LinkCheckDate = ToFix.LinkCheckDate.Replace("'", "''");
                Fixed.InCollection = ToFix.InCollection.Replace("'", "''");
                Fixed.ImageDir = ToFix.ImageDir.Replace("'", "''");

                return Fixed;
            }

            // Compare two objects to see if they have identical values
            public bool PrnChill_Changed(PrnChill OldVal, PrnChill NewVal)
            {
                PrnChill diff = new PrnChill();
                List<string> diffList = new List<string>();
                bool different = false;
                if (OldVal.PostName == null) { OldVal.PostName = ""; }
                if (NewVal.PostName == null) { NewVal.PostName = ""; }
                if (OldVal.PostName != NewVal.PostName) { different = true; }
                if (OldVal.PostURL == null) { OldVal.PostURL = ""; }
                if (NewVal.PostURL == null) { NewVal.PostURL = ""; }
                if (OldVal.PostURL != NewVal.PostURL) { different = true; }
                if (OldVal.Links == null) { OldVal.Links = ""; }
                if (NewVal.Links == null) { NewVal.Links = ""; }
                if (OldVal.Links != NewVal.Links) { different = true; }
                if (OldVal.DateAdded == null) { OldVal.DateAdded = ""; }
                if (NewVal.DateAdded == null) { NewVal.DateAdded = ""; }
                if (OldVal.DateAdded != NewVal.DateAdded) { different = true; }
                if (OldVal.LinkCheckDate == null) { OldVal.LinkCheckDate = ""; }
                if (NewVal.LinkCheckDate == null) { NewVal.LinkCheckDate = ""; }
                if (OldVal.LinkCheckDate != NewVal.LinkCheckDate) { different = true; }
                if (OldVal.InCollection == null) { OldVal.InCollection = ""; }
                if (NewVal.InCollection == null) { NewVal.InCollection = ""; }
                if (OldVal.InCollection != NewVal.InCollection) { different = true; }
                if (OldVal.ImageDir == null) { OldVal.ImageDir = ""; }
                if (NewVal.ImageDir == null) { NewVal.ImageDir = ""; }
                if (OldVal.ImageDir != NewVal.ImageDir) { different = true; }
                return different;
            }

            // Returns object containing the new values different from the old values in object comparison
            public PrnChill PrnChill_Diff(PrnChill OldVal, PrnChill NewVal)
            {
                PrnChill diff = new PrnChill();
                if (OldVal.PostName != NewVal.PostName) { diff.PostName = NewVal.PostName; }
                if (OldVal.PostURL != NewVal.PostURL) { diff.PostURL = NewVal.PostURL; }
                if (OldVal.Links != NewVal.Links) { diff.Links = NewVal.Links; }
                if (OldVal.DateAdded != NewVal.DateAdded) { diff.DateAdded = NewVal.DateAdded; }
                if (OldVal.LinkCheckDate != NewVal.LinkCheckDate) { diff.LinkCheckDate = NewVal.LinkCheckDate; }
                if (OldVal.InCollection != NewVal.InCollection) { diff.InCollection = NewVal.InCollection; }
                if (OldVal.ImageDir != NewVal.ImageDir) { diff.ImageDir = NewVal.ImageDir; }
                return diff;
            }

            // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
            public List<string> PrnChill_DiffList(PrnChill OldVal, PrnChill NewVal)
            {
                List<string> diffList = new List<string>();
                if (OldVal.PostName != NewVal.PostName) { diffList.Add("Changed PostName Value From " + OldVal.PostName + " To " + NewVal.PostName); }
                if (OldVal.PostURL != NewVal.PostURL) { diffList.Add("Changed PostURL Value From " + OldVal.PostURL + " To " + NewVal.PostURL); }
                if (OldVal.Links != NewVal.Links) { diffList.Add("Changed Links Value From " + OldVal.Links + " To " + NewVal.Links); }
                if (OldVal.DateAdded != NewVal.DateAdded) { diffList.Add("Changed DateAdded Value From " + OldVal.DateAdded + " To " + NewVal.DateAdded); }
                if (OldVal.LinkCheckDate != NewVal.LinkCheckDate) { diffList.Add("Changed LinkCheckDate Value From " + OldVal.LinkCheckDate + " To " + NewVal.LinkCheckDate); }
                if (OldVal.InCollection != NewVal.InCollection) { diffList.Add("Changed InCollection Value From " + OldVal.InCollection + " To " + NewVal.InCollection); }
                if (OldVal.ImageDir != NewVal.ImageDir) { diffList.Add("Changed ImageDir Value From " + OldVal.ImageDir + " To " + NewVal.ImageDir); }
                return diffList;
            }

            // Generate XML String From Object
            public string PrnChill_ToXML(PrnChill obj)
            {
                string XMLString = "";
                XMLString = XMLString + "<PostName>" + obj.PostName + "</PostName>";
                XMLString = XMLString + "<PostURL>" + obj.PostURL + "</PostURL>";
                XMLString = XMLString + "<Links>" + obj.Links + "</Links>";
                XMLString = XMLString + "<DateAdded>" + obj.DateAdded + "</DateAdded>";
                XMLString = XMLString + "<LinkCheckDate>" + obj.LinkCheckDate + "</LinkCheckDate>";
                XMLString = XMLString + "<InCollection>" + obj.InCollection + "</InCollection>";
                XMLString = XMLString + "<ImageDir>" + obj.ImageDir + "</ImageDir>";
                return XMLString;
            }



            #endregion
            public bool Create_Table_PrnChill(string DbFile)
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                _AHK ahk = new _AHK();
                string CreateLine = "Create Table [PrnChill] (PostName VARCHAR, PostURL VARCHAR, Links VARCHAR, DateAdded VARCHAR, LinkCheckDate VARCHAR, InCollection VARCHAR, ImageDir VARCHAR)";
                bool TableCreated = sqlite.Table_Exists(DbFile, "PrnChill");
                if (!TableCreated) { TableCreated = sqlite.Table_New(DbFile, "PrnChill", "Create Table [PrnChill] (PostName VARCHAR, PostURL VARCHAR, Links VARCHAR, DateAdded VARCHAR, LinkCheckDate VARCHAR, InCollection VARCHAR, ImageDir VARCHAR", false); }


                if (!TableCreated) { ahk.MsgBox("[PrnChill] Created = " + TableCreated.ToString()); }
                return TableCreated;
            }

            #region ===== PrnChill SQLite : Return =====

            public PrnChill Return_Object_From_PrnChill(string WhereClause = "[PostName] = ''", string DbFile = "")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                _AHK ahk = new _AHK();
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\PrnChill.sqlite"; }
                string SelectLine = "Select [PostName], [PostURL], [Links], [DateAdded], [LinkCheckDate], [InCollection], [ImageDir] From [PrnChill] ";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
                PrnChill returnObject = new PrnChill();
                int i = 0;
                string Value = "";
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.PostName = ret["PostName"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.Links = ret["Links"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.LinkCheckDate = ret["LinkCheckDate"].ToString();
                        returnObject.InCollection = ret["InCollection"].ToString();
                        returnObject.ImageDir = ret["ImageDir"].ToString();
                    }
                }

                return returnObject;
            }

            public List<PrnChill> Return_PrnChill_List(string WhereClause = "", string DbFile = "", string TableName = "[PrnChill]")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                _AHK ahk = new _AHK();
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\PrnChill.sqlite"; }
                string SelectLine = "Select * From " + TableName;

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

                List<PrnChill> ReturnList = new List<PrnChill>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        PrnChill returnObject = new PrnChill();

                        returnObject.PostName = ret["PostName"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.Links = ret["Links"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.LinkCheckDate = ret["LinkCheckDate"].ToString();
                        returnObject.InCollection = ret["InCollection"].ToString();
                        returnObject.ImageDir = ret["ImageDir"].ToString();

                        ReturnList.Add(returnObject);
                    }
                }

                return ReturnList;
            }

            public DataTable Return_DataTable_From_PrnChill(string DbFile)
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                string SelectLine = "Select [PostName], [PostURL], [Links], [DateAdded], [LinkCheckDate], [InCollection], [ImageDir] From [PrnChill]";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                return ReturnTable;
            }


            #endregion
            #region ===== PrnChill SQLite : Update Insert =====

            public bool PrnChill_Insert(PrnChill inObject, string DbFile = "")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                _AHK ahk = new _AHK();
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\PrnChill.sqlite"; }
                string InsertLine = "Insert Into [PrnChill] (PostName, PostURL, Links, DateAdded, LinkCheckDate, InCollection, ImageDir) values ('" + inObject.PostName + "', '" + inObject.PostURL + "', '" + inObject.Links + "', '" + inObject.DateAdded + "', '" + inObject.LinkCheckDate + "', '" + inObject.InCollection + "', '" + inObject.ImageDir + "')";
                bool Inserted = sqlite.Execute(DbFile, InsertLine);
                if (!Inserted) { ahk.MsgBox("Inserted Into [PrnChill] = " + Inserted.ToString()); }
                return Inserted;
            }

            public bool PrnChill_Update(PrnChill inObject, string DbFile = "")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                _AHK ahk = new _AHK();
                //string UpdateLine = "Update [PrnChill] set PostName = '" + inObject.PostName + "', PostURL = '" + inObject.PostURL + "', Links = '" + inObject.Links + "', DateAdded = '" + inObject.DateAdded + "', LinkCheckDate = '" + inObject.LinkCheckDate + "', InCollection = '" + inObject.InCollection + "', ImageDir = '" + inObject.ImageDir + "' WHERE [Item] = 'Value' ";
                string UpdateLine = "Update [PrnChill] set ";


                if (inObject.PostName != null) { UpdateLine = UpdateLine + "[PostName] = '" + inObject.PostName + "',"; }
                if (inObject.PostURL != null) { UpdateLine = UpdateLine + "[PostURL] = '" + inObject.PostURL + "',"; }
                if (inObject.Links != null) { UpdateLine = UpdateLine + "[Links] = '" + inObject.Links + "',"; }
                if (inObject.DateAdded != null) { UpdateLine = UpdateLine + "[DateAdded] = '" + inObject.DateAdded + "',"; }
                if (inObject.LinkCheckDate != null) { UpdateLine = UpdateLine + "[LinkCheckDate] = '" + inObject.LinkCheckDate + "',"; }
                if (inObject.InCollection != null) { UpdateLine = UpdateLine + "[InCollection] = '" + inObject.InCollection + "',"; }
                if (inObject.ImageDir != null) { UpdateLine = UpdateLine + "[ImageDir] = '" + inObject.ImageDir + "',"; }

                UpdateLine = ahk.TrimLast(UpdateLine, 1);
                UpdateLine = UpdateLine + " WHERE [PostName] = ' '"; // DEFINE CONDITION HERE !!!

                bool Updated = sqlite.Execute(DbFile, UpdateLine);
                return Updated;
            }

            public bool PrnChill_UpdateInsert(PrnChill obj, string DbFile = "")
            {
                _AHK ahk = new _AHK();

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\PrnChill.sqlite"; }
                if (!File.Exists(DbFile)) { Create_Table_PrnChill(DbFile); }

                bool Updated = PrnChill_Update(obj, DbFile);  // try to update record first
                if (!Updated) { Updated = PrnChill_Insert(obj, DbFile); }  // if unable to update, insert new record
                return Updated;
            }


            #endregion
            #region ===== PrnChill DataTable =====

            public DataTable Return_PrnChill_DataTable(string DbFile = "", string TableName = "PrnChill", string WhereClause = "", bool Debug = false)
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                _AHK ahk = new _AHK();

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\PrnChill.sqlite"; }
                string SelectLine = "Select * From [PrnChill]";

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }

                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


                DataTable table = new DataTable();
                table.Columns.Add("PostName", typeof(string));
                table.Columns.Add("PostURL", typeof(string));
                table.Columns.Add("Links", typeof(string));
                table.Columns.Add("DateAdded", typeof(string));
                table.Columns.Add("LinkCheckDate", typeof(string));
                table.Columns.Add("InCollection", typeof(string));
                table.Columns.Add("ImageDir", typeof(string));

                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        PrnChill returnObject = new PrnChill();

                        returnObject.PostName = ret["PostName"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.Links = ret["Links"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.LinkCheckDate = ret["LinkCheckDate"].ToString();
                        returnObject.InCollection = ret["InCollection"].ToString();
                        returnObject.ImageDir = ret["ImageDir"].ToString();

                        table.Rows.Add(returnObject.PostName, returnObject.PostURL, returnObject.Links, returnObject.DateAdded, returnObject.LinkCheckDate, returnObject.InCollection, returnObject.ImageDir);
                    }
                }

                return table;
            }

            public DataTable Create_PrnChill_DataTable(PrnChill inObject)
            {
                DataTable table = new DataTable();
                table.Columns.Add("PostName", typeof(string));
                table.Columns.Add("PostURL", typeof(string));
                table.Columns.Add("Links", typeof(string));
                table.Columns.Add("DateAdded", typeof(string));
                table.Columns.Add("LinkCheckDate", typeof(string));
                table.Columns.Add("InCollection", typeof(string));
                table.Columns.Add("ImageDir", typeof(string));

                table.Rows.Add(inObject.PostName, inObject.PostURL, inObject.Links, inObject.DateAdded, inObject.LinkCheckDate, inObject.InCollection, inObject.ImageDir);
                return table;
            }


            #endregion
            #region ===== PrnChill DataGridView =====

            public void HideShow_PrnChill_Columns(DataGridView dv)
            {

                try { dv.Columns["PostName"].Visible = true; } catch { }
                try { dv.Columns["PostURL"].Visible = true; } catch { }
                try { dv.Columns["Links"].Visible = true; } catch { }
                try { dv.Columns["DateAdded"].Visible = true; } catch { }
                try { dv.Columns["LinkCheckDate"].Visible = true; } catch { }
                try { dv.Columns["InCollection"].Visible = true; } catch { }
                try { dv.Columns["ImageDir"].Visible = true; } catch { }
            }
            public void Enable_PrnChill_Columns(DataGridView dv)
            {

                try { dv.Columns["PostName"].ReadOnly = true; } catch { }
                try { dv.Columns["PostURL"].ReadOnly = true; } catch { }
                try { dv.Columns["Links"].ReadOnly = true; } catch { }
                try { dv.Columns["DateAdded"].ReadOnly = true; } catch { }
                try { dv.Columns["LinkCheckDate"].ReadOnly = true; } catch { }
                try { dv.Columns["InCollection"].ReadOnly = true; } catch { }
                try { dv.Columns["ImageDir"].ReadOnly = true; } catch { }
            }

            #endregion
            #region ===== PrnChill SQL Functions =====

            // Return PrnChill SQL Connection String
            public SqlConnection PrnChill_Conn()
            {
                // populate sql connection
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMLucidMedia"].ConnectionString);
                // SqlConnection Conn = new SqlConnection("Server=188.168.188.88;DataBase=LucidMedia;Uid=lucidm;Pwd=pass");
                return conn;
            }

            // Return PrnChill TableName (Full Path)
            public string PrnChill_TableName()
            {
                // populate to return full sql table name
                return "[ADBIndex].[dbo].[PrnChill]";
            }

            // Generate SQL Table
            public bool PrnChill_CreateSQLTable()
            {
                SqlConnection Conn = PrnChill_Conn();
                string CreateTableLine = "CREATE TABLE [PrnChill](";
                CreateTableLine = CreateTableLine + "[PostName] [int] IDENTITY(1,1) NOT NULL,";
                CreateTableLine = CreateTableLine + "[PostURL] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Links] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[DateAdded] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[LinkCheckDate] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[InCollection] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[ImageDir] [varchar](max) NOT NULL";
                CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";

                _Database.SQL sql = new _Database.SQL();

                return sql.WriteDataRecord(Conn, CreateTableLine);
            }

            public bool PrnChill_InsertSQL(PrnChill obj)
            {
                _AHK ahk = new _AHK();
                SqlConnection Con = PrnChill_Conn();
                string SQLLine = "Insert Into " + PrnChill_TableName() + " (LinkCount, PostName, PostURL, Links, DateAdded, LinkCheckDate, InCollection, ImageDir) VALUES (@LinkCount, @PostName, @PostURL, @Links, @DateAdded, @LinkCheckDate, @InCollection, @ImageDir)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.PostName == null) { obj.PostName = ""; }
                if (obj.PostURL == null) { obj.PostURL = ""; }
                if (obj.Links == null) { obj.Links = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = ""; }
                if (obj.LinkCheckDate == null) { obj.LinkCheckDate = ""; }
                if (obj.InCollection == null) { obj.InCollection = ""; }
                if (obj.ImageDir == null) { obj.ImageDir = ""; }
                cmd2.Parameters.AddWithValue(@"PostName", obj.PostName.ToString());
                cmd2.Parameters.AddWithValue(@"PostURL", obj.PostURL.ToString());
                cmd2.Parameters.AddWithValue(@"Links", obj.Links.ToString());
                cmd2.Parameters.AddWithValue(@"DateAdded", DateTime.Now.ToString());
                cmd2.Parameters.AddWithValue(@"LinkCheckDate", obj.LinkCheckDate.ToString());
                cmd2.Parameters.AddWithValue(@"InCollection", obj.InCollection.ToString());
                cmd2.Parameters.AddWithValue(@"ImageDir", obj.ImageDir.ToString());
                cmd2.Parameters.AddWithValue(@"LinkCount", obj.LinkCount.ToString());
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        PrnChill_InsertSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        PrnChill_InsertSQL(obj);
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

            public bool PrnChill_UpdateSQL(PrnChill obj)
            {
                _AHK ahk = new _AHK();
                SqlConnection Conn = PrnChill_Conn();
                string SQLLine = "Update " + PrnChill_TableName() + " SET LinkCount = @LinkCount, PostName = @PostName, PostURL = @PostURL, Links = @Links, DateAdded = @DateAdded, LinkCheckDate = @LinkCheckDate, InCollection = @InCollection, ImageDir = @ImageDir WHERE PostURL = @PostURL";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.PostName == null) { obj.PostName = ""; }
                if (obj.PostURL == null) { obj.PostURL = ""; }
                if (obj.Links == null) { obj.Links = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = ""; }
                if (obj.LinkCheckDate == null) { obj.LinkCheckDate = ""; }
                if (obj.InCollection == null) { obj.InCollection = ""; }
                if (obj.ImageDir == null) { obj.ImageDir = ""; }
                cmd2.Parameters.AddWithValue(@"PostName", obj.PostName.ToString());
                cmd2.Parameters.AddWithValue(@"PostURL", obj.PostURL.ToString());
                cmd2.Parameters.AddWithValue(@"Links", obj.Links.ToString());
                cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded.ToString());
                cmd2.Parameters.AddWithValue(@"LinkCheckDate", obj.LinkCheckDate.ToString());
                cmd2.Parameters.AddWithValue(@"InCollection", obj.InCollection.ToString());
                cmd2.Parameters.AddWithValue(@"ImageDir", obj.ImageDir.ToString());
                cmd2.Parameters.AddWithValue(@"LinkCount", obj.LinkCount.ToString());
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        PrnChill_UpdateSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        PrnChill_UpdateSQL(obj);
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

            public bool PrnChill_UpdateInsert(PrnChill obj)
            {
                bool Updated = PrnChill_UpdateSQL(obj);  // try to update record first
                if (!Updated) { Updated = PrnChill_InsertSQL(obj); }  // if unable to update, insert new record
                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool PrnChill_UpdateIfPopulated(PrnChill obj, string ID = "")
            {
                _AHK ahk = new _AHK();
                SqlConnection Conn = PrnChill_Conn();
                string SQLcmd = "Update " + PrnChill_TableName() + " SET ";
                if (obj.PostName != null) { SQLcmd = SQLcmd + " PostName = @PostName,"; }
                if (obj.PostURL != null) { SQLcmd = SQLcmd + " PostURL = @PostURL,"; }
                if (obj.Links != null) { SQLcmd = SQLcmd + " Links = @Links,"; }
                if (obj.DateAdded != null) { SQLcmd = SQLcmd + " DateAdded = @DateAdded,"; }
                if (obj.LinkCheckDate != null) { SQLcmd = SQLcmd + " LinkCheckDate = @LinkCheckDate,"; }
                if (obj.InCollection != null) { SQLcmd = SQLcmd + " InCollection = @InCollection,"; }
                if (obj.ImageDir != null) { SQLcmd = SQLcmd + " ImageDir = @ImageDir,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ID = @ID";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.PostName != null) { cmd2.Parameters.AddWithValue(@"PostName", obj.PostName); }
                if (obj.PostURL != null) { cmd2.Parameters.AddWithValue(@"PostURL", obj.PostURL); }
                if (obj.Links != null) { cmd2.Parameters.AddWithValue(@"Links", obj.Links); }
                if (obj.DateAdded != null) { cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded); }
                if (obj.LinkCheckDate != null) { cmd2.Parameters.AddWithValue(@"LinkCheckDate", obj.LinkCheckDate); }
                if (obj.InCollection != null) { cmd2.Parameters.AddWithValue(@"InCollection", obj.InCollection); }
                if (obj.ImageDir != null) { cmd2.Parameters.AddWithValue(@"ImageDir", obj.ImageDir); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            /// <summary>
            /// What this post already added to database
            /// </summary>
            /// <param name="PostURL"></param>
            /// <returns></returns>
            public bool AlreadyParsed(string PostURL)
            {
                PrnChill item = PrnChill_ReturnSQL(PostURL);
                if (item.PostName == null) { return false; }
                return true;
            }

            public PrnChill PrnChill_ReturnSQL(string PostURL)
            {
                _Database.SQL sql = new _Database.SQL();
                SqlConnection Conn = PrnChill_Conn();
                string SelectLine = "Select [PostName],[PostURL],[Links],[DateAdded],[LinkCheckDate],[InCollection],[ImageDir] From " + PrnChill_TableName() + " WHERE PostURL = '" + PostURL + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                PrnChill returnObject = new PrnChill();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.PostName = ret["PostName"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.Links = ret["Links"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.LinkCheckDate = ret["LinkCheckDate"].ToString();
                        returnObject.InCollection = ret["InCollection"].ToString();
                        returnObject.ImageDir = ret["ImageDir"].ToString();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public List<PrnChill> PrnChill_ReturnSQLList(string Command = "")
            {
                _Database.SQL sql = new _Database.SQL();
                if (Command == "") { Command = "Select * From PrnChill_TableName()"; }
                SqlConnection Conn = PrnChill_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<PrnChill> ReturnList = new List<PrnChill>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        PrnChill returnObject = new PrnChill();
                        returnObject.PostName = ret["PostName"].ToString();
                        returnObject.PostURL = ret["PostURL"].ToString();
                        returnObject.Links = ret["Links"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.LinkCheckDate = ret["LinkCheckDate"].ToString();
                        returnObject.InCollection = ret["InCollection"].ToString();
                        returnObject.ImageDir = ret["ImageDir"].ToString();
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

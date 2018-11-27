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
        public class Novanon
        {
            _AHK ahk = new _AHK();
            _Parse prs = new _Parse();
            _Parse.XML xml = new _Parse.XML();
            _Lists lst = new _Lists();
            _Database.SQL sql = new _Database.SQL();
            _Database.SQLite sqlite = new _Database.SQLite();

            public string CartoonsAnime = "https://novanon.net/forums/cartoons-anime.57/";


            public void ParseShowIndex_HTMLDIR()
            {
                //string html = ahk.Download_HTML(CartoonsAnime);

                string path = @"D:\xvdPrs\Novanon\Cartoons&AnimeNovanon.html";

                List<string> htmlFiles = lst.FileList(@"D:\xvdPrs\Novanon", "*.html", false);

                string Section = "Cartoons";

                foreach (string file in htmlFiles)
                {
                    string cmd = "//*[@id=\"top\"]/div[2]/div[1]/div/div/div[3]/div/div/div/div[2]/div[2]/div/div";

                    string html = ahk.FileRead(file);

                    List<string> Between = xml.Parse_HTML_XML(html, cmd);  // extract sections of text from html xml

                    //if (Bar != null) { pro.SetupProgressBar(Bar, Between.Count); }

                    int p = 1; string dateline = "";
                    foreach (string seg in Between)
                    {
                        Novanan obj = new Novanan();
                        obj.Section = Section;

                        int hrefs = 0;
                        List<string> lines = lst.Text_To_List(seg, true, true, false);

                        foreach (string line in lines)
                        {
                            if (line.Contains("<a href="))
                            {
                                if (line.Contains("/latest"))
                                {
                                    dateline = line;

                                    //title="Aug 16, 2018 at 12:45 AM">Today at 12:45 AM</time></a>
                                    dateline = dateline.Replace("title=\"", " |");

                                    dateline = ahk.StringSplit(dateline, "|", 1);

                                    dateline = dateline.Replace("\">", " |");

                                    dateline = ahk.StringSplit(dateline, "|", 0);

                                    //ahk.MsgBox(dateline);

                                    obj.LastPostDate = dateline;
                                }

                                if (!line.Contains("/members/"))
                                {
                                    if (line.Contains("/unread"))
                                    {
                                        //ahk.MsgBox(line);

                                        string URL = ahk.StringSplit(line, "\"", 1);

                                        string rest = line.Replace("id=\"", "|");
                                        rest = ahk.StringSplit(rest, "|", 1);

                                        string ID = ahk.StringSplit(rest, "\"", 0);

                                        string title = ahk.StringSplit(rest, ">", 1);
                                        title = ahk.StringSplit(title, "<", 0);

                                        //ahk.MsgBox(title + "\n\n" + URL);

                                        obj.Title = title;
                                        obj.TitleURL = URL;

                                        if (obj.Title.Trim() != "") { Novanan_UpdateInsert(obj); }

                                        continue;
                                    }
                                    if (line.Contains("/threads/"))
                                    {
                                        //ahk.MsgBox(line);

                                        string URL = ahk.StringSplit(line, "\"", 1);

                                        string rest = line.Replace("id=\"", "|");
                                        rest = ahk.StringSplit(rest, "|", 1);

                                        string ID = ahk.StringSplit(rest, "\"", 0);

                                        string title = ahk.StringSplit(rest, ">", 1);
                                        title = ahk.StringSplit(title, "<", 0);

                                        //ahk.MsgBox(title + "\n\n" + URL);

                                        obj.Title = title;
                                        obj.TitleURL = URL;

                                        if (obj.Title.Trim() != "") { Novanan_UpdateInsert(obj); }

                                        continue;
                                    }
                                }


                            }
                        }

                        //ahk.MsgBox(p + " | " + seg); p++;
                    }
                }


                ahk.MsgBox("FINISHED!");


            }


            public void ParseShowIndex(string html, string Section = "Cartoons")
            {
                //string html = ahk.Download_HTML(CartoonsAnime);

                string path = @"D:\xvdPrs\Novanon\Cartoons&AnimeNovanon.html";

                //List<string> htmlFiles = lst.FileList(@"D:\xvdPrs\Novanon", "*.html", false);


                string cmd = "//*[@id=\"top\"]/div[2]/div[1]/div/div/div[3]/div/div/div/div[2]/div[2]/div/div";

                //string html = ahk.FileRead(file);

                List<string> Between = xml.Parse_HTML_XML(html, cmd);  // extract sections of text from html xml

                //if (Bar != null) { pro.SetupProgressBar(Bar, Between.Count); }

                int p = 1; string dateline = "";
                foreach (string seg in Between)
                {
                    Novanan obj = new Novanan();
                    obj.Section = Section;

                    int hrefs = 0;
                    List<string> lines = lst.Text_To_List(seg, true, true, false);

                    foreach (string line in lines)
                    {
                        if (line.Contains("<a href="))
                        {
                            if (line.Contains("/latest"))
                            {
                                dateline = line;

                                //title="Aug 16, 2018 at 12:45 AM">Today at 12:45 AM</time></a>
                                dateline = dateline.Replace("title=\"", " |");

                                dateline = ahk.StringSplit(dateline, "|", 1);

                                dateline = dateline.Replace("\">", " |");

                                dateline = ahk.StringSplit(dateline, "|", 0);

                                //ahk.MsgBox(dateline);

                                obj.LastPostDate = dateline;
                            }

                            if (!line.Contains("/members/"))
                            {
                                if (line.Contains("/unread"))
                                {
                                    //ahk.MsgBox(line);

                                    string URL = ahk.StringSplit(line, "\"", 1);

                                    string rest = line.Replace("id=\"", "|");
                                    rest = ahk.StringSplit(rest, "|", 1);

                                    string ID = ahk.StringSplit(rest, "\"", 0);

                                    string title = ahk.StringSplit(rest, ">", 1);
                                    title = ahk.StringSplit(title, "<", 0);

                                    //ahk.MsgBox(title + "\n\n" + URL);

                                    obj.Title = title;
                                    obj.TitleURL = URL;

                                    if (obj.Title.Trim() != "") { Novanan_UpdateInsert(obj); }

                                    continue;
                                }
                                if (line.Contains("/threads/"))
                                {
                                    //ahk.MsgBox(line);

                                    string URL = ahk.StringSplit(line, "\"", 1);

                                    string rest = line.Replace("id=\"", "|");
                                    rest = ahk.StringSplit(rest, "|", 1);

                                    string ID = ahk.StringSplit(rest, "\"", 0);

                                    string title = ahk.StringSplit(rest, ">", 1);
                                    title = ahk.StringSplit(title, "<", 0);

                                    //ahk.MsgBox(title + "\n\n" + URL);

                                    obj.Title = title;
                                    obj.TitleURL = URL;

                                    if (obj.Title.Trim() != "") { Novanan_UpdateInsert(obj); }

                                    continue;
                                }
                            }


                        }
                    }

                    //ahk.MsgBox(p + " | " + seg); p++;
                }

                ahk.MsgBox("FINISHED!");

            }


            #region === Novanan FUNCTIONS ===

            #region ===== Novanan Object =====

            public struct Novanan
            {
                public string ID { get; set; }
                public string IMDb { get; set; }
                public string Section { get; set; }
                public string Title { get; set; }
                public string TitleURL { get; set; }
                public string TitleLinks { get; set; }
                public string DateAdded { get; set; }
                public string DateModified { get; set; }

                public string LastPostDate { get; set; }

                public string InCollection { get; set; }
            }
            //  Fix illegal characters before Sql/Sqlite Db Inserts
            public Novanan Novanan_FixChars(Novanan ToFix)
            {
                Novanan Fixed = new Novanan();

                Fixed.ID = ToFix.ID.Replace("'", "''");
                Fixed.Section = ToFix.Section.Replace("'", "''");
                Fixed.Title = ToFix.Title.Replace("'", "''");
                Fixed.TitleURL = ToFix.TitleURL.Replace("'", "''");
                Fixed.TitleLinks = ToFix.TitleLinks.Replace("'", "''");
                Fixed.DateAdded = ToFix.DateAdded.Replace("'", "''");
                Fixed.DateModified = ToFix.DateModified.Replace("'", "''");
                Fixed.InCollection = ToFix.InCollection.Replace("'", "''");

                return Fixed;
            }

            // Compare two objects to see if they have identical values
            public bool Novanan_Changed(Novanan OldVal, Novanan NewVal)
            {
                Novanan diff = new Novanan();
                List<string> diffList = new List<string>();
                bool different = false;
                if (OldVal.ID == null) { OldVal.ID = ""; }
                if (NewVal.ID == null) { NewVal.ID = ""; }
                if (OldVal.ID != NewVal.ID) { different = true; }
                if (OldVal.Section == null) { OldVal.Section = ""; }
                if (NewVal.Section == null) { NewVal.Section = ""; }
                if (OldVal.Section != NewVal.Section) { different = true; }
                if (OldVal.Title == null) { OldVal.Title = ""; }
                if (NewVal.Title == null) { NewVal.Title = ""; }
                if (OldVal.Title != NewVal.Title) { different = true; }
                if (OldVal.TitleURL == null) { OldVal.TitleURL = ""; }
                if (NewVal.TitleURL == null) { NewVal.TitleURL = ""; }
                if (OldVal.TitleURL != NewVal.TitleURL) { different = true; }
                if (OldVal.TitleLinks == null) { OldVal.TitleLinks = ""; }
                if (NewVal.TitleLinks == null) { NewVal.TitleLinks = ""; }
                if (OldVal.TitleLinks != NewVal.TitleLinks) { different = true; }
                if (OldVal.DateAdded == null) { OldVal.DateAdded = ""; }
                if (NewVal.DateAdded == null) { NewVal.DateAdded = ""; }
                if (OldVal.DateAdded != NewVal.DateAdded) { different = true; }
                if (OldVal.DateModified == null) { OldVal.DateModified = ""; }
                if (NewVal.DateModified == null) { NewVal.DateModified = ""; }
                if (OldVal.DateModified != NewVal.DateModified) { different = true; }
                if (OldVal.InCollection == null) { OldVal.InCollection = ""; }
                if (NewVal.InCollection == null) { NewVal.InCollection = ""; }
                if (OldVal.InCollection != NewVal.InCollection) { different = true; }
                return different;
            }

            // Returns object containing the new values different from the old values in object comparison
            public Novanan Novanan_Diff(Novanan OldVal, Novanan NewVal)
            {
                Novanan diff = new Novanan();
                if (OldVal.ID != NewVal.ID) { diff.ID = NewVal.ID; }
                if (OldVal.Section != NewVal.Section) { diff.Section = NewVal.Section; }
                if (OldVal.Title != NewVal.Title) { diff.Title = NewVal.Title; }
                if (OldVal.TitleURL != NewVal.TitleURL) { diff.TitleURL = NewVal.TitleURL; }
                if (OldVal.TitleLinks != NewVal.TitleLinks) { diff.TitleLinks = NewVal.TitleLinks; }
                if (OldVal.DateAdded != NewVal.DateAdded) { diff.DateAdded = NewVal.DateAdded; }
                if (OldVal.DateModified != NewVal.DateModified) { diff.DateModified = NewVal.DateModified; }
                if (OldVal.InCollection != NewVal.InCollection) { diff.InCollection = NewVal.InCollection; }
                return diff;
            }

            // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
            public List<string> Novanan_DiffList(Novanan OldVal, Novanan NewVal)
            {
                List<string> diffList = new List<string>();
                if (OldVal.ID != NewVal.ID) { diffList.Add("Changed ID Value From " + OldVal.ID + " To " + NewVal.ID); }
                if (OldVal.Section != NewVal.Section) { diffList.Add("Changed Section Value From " + OldVal.Section + " To " + NewVal.Section); }
                if (OldVal.Title != NewVal.Title) { diffList.Add("Changed Title Value From " + OldVal.Title + " To " + NewVal.Title); }
                if (OldVal.TitleURL != NewVal.TitleURL) { diffList.Add("Changed TitleURL Value From " + OldVal.TitleURL + " To " + NewVal.TitleURL); }
                if (OldVal.TitleLinks != NewVal.TitleLinks) { diffList.Add("Changed TitleLinks Value From " + OldVal.TitleLinks + " To " + NewVal.TitleLinks); }
                if (OldVal.DateAdded != NewVal.DateAdded) { diffList.Add("Changed DateAdded Value From " + OldVal.DateAdded + " To " + NewVal.DateAdded); }
                if (OldVal.DateModified != NewVal.DateModified) { diffList.Add("Changed DateModified Value From " + OldVal.DateModified + " To " + NewVal.DateModified); }
                if (OldVal.InCollection != NewVal.InCollection) { diffList.Add("Changed InCollection Value From " + OldVal.InCollection + " To " + NewVal.InCollection); }
                return diffList;
            }

            // Generate XML String From Object
            public string Novanan_ToXML(Novanan obj)
            {
                string XMLString = "";
                XMLString = XMLString + "<ID>" + obj.ID + "</ID>";
                XMLString = XMLString + "<Section>" + obj.Section + "</Section>";
                XMLString = XMLString + "<Title>" + obj.Title + "</Title>";
                XMLString = XMLString + "<TitleURL>" + obj.TitleURL + "</TitleURL>";
                XMLString = XMLString + "<TitleLinks>" + obj.TitleLinks + "</TitleLinks>";
                XMLString = XMLString + "<DateAdded>" + obj.DateAdded + "</DateAdded>";
                XMLString = XMLString + "<DateModified>" + obj.DateModified + "</DateModified>";
                XMLString = XMLString + "<InCollection>" + obj.InCollection + "</InCollection>";
                return XMLString;
            }

            // Populate Object from XML Tag String
            public Novanan Novanan_FromXML(string XMLString)
            {
                Novanan obj = new Novanan();
                obj.ID = prs.XML_Text(XMLString, "<ID>");
                obj.Section = prs.XML_Text(XMLString, "<Section>");
                obj.Title = prs.XML_Text(XMLString, "<Title>");
                obj.TitleURL = prs.XML_Text(XMLString, "<TitleURL>");
                obj.TitleLinks = prs.XML_Text(XMLString, "<TitleLinks>");
                obj.DateAdded = prs.XML_Text(XMLString, "<DateAdded>");
                obj.DateModified = prs.XML_Text(XMLString, "<DateModified>");
                obj.InCollection = prs.XML_Text(XMLString, "<InCollection>");
                return obj;
            }


            #endregion
            public bool Create_Table_Novanan(string DbFile)
            {
                string CreateLine = "Create Table [Novanan] (ID INTEGER PRIMARY KEY, Section VARCHAR, Title VARCHAR, TitleURL VARCHAR, TitleLinks VARCHAR, DateAdded VARCHAR, DateModified VARCHAR, InCollection VARCHAR)";
                bool TableCreated = sqlite.Table_Exists(DbFile, "Novanan");
                if (!TableCreated) { TableCreated = sqlite.Table_New(DbFile, "Novanan", "Create Table [Novanan] (ID INTEGER PRIMARY KEY, Section VARCHAR, Title VARCHAR, TitleURL VARCHAR, TitleLinks VARCHAR, DateAdded VARCHAR, DateModified VARCHAR, InCollection VARCHAR", false); }


                if (!TableCreated) { ahk.MsgBox("[Novanan] Created = " + TableCreated.ToString()); }
                return TableCreated;
            }

            #region ===== Novanan SQLite : Return =====

            public Novanan Return_Object_From_Novanan(string WhereClause = "[ID] = ''", string DbFile = "")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\Novanan.sqlite"; }
                string SelectLine = "Select [ID], [Section], [Title], [TitleURL], [TitleLinks], [DateAdded], [DateModified], [InCollection] From [Novanan] ";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
                Novanan returnObject = new Novanan();
                int i = 0;
                string Value = "";
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.Section = ret["Section"].ToString();
                        returnObject.Title = ret["Title"].ToString();
                        returnObject.TitleURL = ret["TitleURL"].ToString();
                        returnObject.TitleLinks = ret["TitleLinks"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.DateModified = ret["DateModified"].ToString();
                        returnObject.InCollection = ret["InCollection"].ToString();
                    }
                }

                return returnObject;
            }

            public List<Novanan> Return_Novanan_List(string WhereClause = "", string DbFile = "", string TableName = "[Novanan]")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\Novanan.sqlite"; }
                string SelectLine = "Select * From " + TableName;

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

                List<Novanan> ReturnList = new List<Novanan>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        Novanan returnObject = new Novanan();

                        returnObject.ID = ret["ID"].ToString();
                        returnObject.Section = ret["Section"].ToString();
                        returnObject.Title = ret["Title"].ToString();
                        returnObject.TitleURL = ret["TitleURL"].ToString();
                        returnObject.TitleLinks = ret["TitleLinks"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.DateModified = ret["DateModified"].ToString();
                        returnObject.InCollection = ret["InCollection"].ToString();

                        ReturnList.Add(returnObject);
                    }
                }

                return ReturnList;
            }

            public DataTable Return_DataTable_From_Novanan(string DbFile)
            {
                string SelectLine = "Select [ID], [Section], [Title], [TitleURL], [TitleLinks], [DateAdded], [DateModified], [InCollection] From [Novanan]";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                return ReturnTable;
            }


            #endregion
            #region ===== Novanan SQLite : Update Insert =====

            public bool Novanan_Insert(Novanan inObject, string DbFile = "")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\Novanan.sqlite"; }
                string InsertLine = "Insert Into [Novanan] (ID, Section, Title, TitleURL, TitleLinks, DateAdded, DateModified, InCollection) values ('" + inObject.ID + "', '" + inObject.Section + "', '" + inObject.Title + "', '" + inObject.TitleURL + "', '" + inObject.TitleLinks + "', '" + inObject.DateAdded + "', '" + inObject.DateModified + "', '" + inObject.InCollection + "')";
                bool Inserted = sqlite.Execute(DbFile, InsertLine);
                if (!Inserted) { ahk.MsgBox("Inserted Into [Novanan] = " + Inserted.ToString()); }
                return Inserted;
            }

            public bool Novanan_Update(Novanan inObject, string DbFile = "")
            {
                //string UpdateLine = "Update [Novanan] set ID = '" + inObject.ID + "', Section = '" + inObject.Section + "', Title = '" + inObject.Title + "', TitleURL = '" + inObject.TitleURL + "', TitleLinks = '" + inObject.TitleLinks + "', DateAdded = '" + inObject.DateAdded + "', DateModified = '" + inObject.DateModified + "', InCollection = '" + inObject.InCollection + "' WHERE [Item] = 'Value' ";
                string UpdateLine = "Update [Novanan] set ";


                if (inObject.ID != null) { UpdateLine = UpdateLine + "[ID] = '" + inObject.ID + "',"; }
                if (inObject.Section != null) { UpdateLine = UpdateLine + "[Section] = '" + inObject.Section + "',"; }
                if (inObject.Title != null) { UpdateLine = UpdateLine + "[Title] = '" + inObject.Title + "',"; }
                if (inObject.TitleURL != null) { UpdateLine = UpdateLine + "[TitleURL] = '" + inObject.TitleURL + "',"; }
                if (inObject.TitleLinks != null) { UpdateLine = UpdateLine + "[TitleLinks] = '" + inObject.TitleLinks + "',"; }
                if (inObject.DateAdded != null) { UpdateLine = UpdateLine + "[DateAdded] = '" + inObject.DateAdded + "',"; }
                if (inObject.DateModified != null) { UpdateLine = UpdateLine + "[DateModified] = '" + inObject.DateModified + "',"; }
                if (inObject.InCollection != null) { UpdateLine = UpdateLine + "[InCollection] = '" + inObject.InCollection + "',"; }

                UpdateLine = ahk.TrimLast(UpdateLine, 1);
                UpdateLine = UpdateLine + " WHERE [ID] = ' '"; // DEFINE CONDITION HERE !!!

                bool Updated = sqlite.Execute(DbFile, UpdateLine);
                return Updated;
            }

            public bool Novanan_UpdateInsert(Novanan obj, string DbFile = "")
            {

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\Novanan.sqlite"; }
                if (!File.Exists(DbFile)) { Create_Table_Novanan(DbFile); }

                bool Updated = Novanan_Update(obj, DbFile);  // try to update record first
                if (!Updated) { Updated = Novanan_Insert(obj, DbFile); }  // if unable to update, insert new record
                return Updated;
            }


            #endregion
            #region ===== Novanan DataTable =====

            public DataTable Return_Novanan_DataTable(string DbFile = "", string TableName = "Novanan", string WhereClause = "", bool Debug = false)
            {

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\Novanan.sqlite"; }
                string SelectLine = "Select * From [Novanan]";

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }

                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


                DataTable table = new DataTable();
                table.Columns.Add("ID", typeof(string));
                table.Columns.Add("Section", typeof(string));
                table.Columns.Add("Title", typeof(string));
                table.Columns.Add("TitleURL", typeof(string));
                table.Columns.Add("TitleLinks", typeof(string));
                table.Columns.Add("DateAdded", typeof(string));
                table.Columns.Add("DateModified", typeof(string));
                table.Columns.Add("InCollection", typeof(string));

                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        Novanan returnObject = new Novanan();

                        returnObject.ID = ret["ID"].ToString();
                        returnObject.Section = ret["Section"].ToString();
                        returnObject.Title = ret["Title"].ToString();
                        returnObject.TitleURL = ret["TitleURL"].ToString();
                        returnObject.TitleLinks = ret["TitleLinks"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.DateModified = ret["DateModified"].ToString();
                        returnObject.InCollection = ret["InCollection"].ToString();

                        table.Rows.Add(returnObject.ID, returnObject.Section, returnObject.Title, returnObject.TitleURL, returnObject.TitleLinks, returnObject.DateAdded, returnObject.DateModified, returnObject.InCollection);
                    }
                }

                return table;
            }

            public DataTable Create_Novanan_DataTable(Novanan inObject)
            {
                DataTable table = new DataTable();
                table.Columns.Add("ID", typeof(string));
                table.Columns.Add("Section", typeof(string));
                table.Columns.Add("Title", typeof(string));
                table.Columns.Add("TitleURL", typeof(string));
                table.Columns.Add("TitleLinks", typeof(string));
                table.Columns.Add("DateAdded", typeof(string));
                table.Columns.Add("DateModified", typeof(string));
                table.Columns.Add("InCollection", typeof(string));

                table.Rows.Add(inObject.ID, inObject.Section, inObject.Title, inObject.TitleURL, inObject.TitleLinks, inObject.DateAdded, inObject.DateModified, inObject.InCollection);
                return table;
            }


            #endregion
            #region ===== Novanan DataGridView =====

            public void HideShow_Novanan_Columns(DataGridView dv)
            {

                try { dv.Columns["ID"].Visible = true; } catch { }
                try { dv.Columns["Section"].Visible = true; } catch { }
                try { dv.Columns["Title"].Visible = true; } catch { }
                try { dv.Columns["TitleURL"].Visible = true; } catch { }
                try { dv.Columns["TitleLinks"].Visible = true; } catch { }
                try { dv.Columns["DateAdded"].Visible = true; } catch { }
                try { dv.Columns["DateModified"].Visible = true; } catch { }
                try { dv.Columns["InCollection"].Visible = true; } catch { }
            }
            public void Enable_Novanan_Columns(DataGridView dv)
            {

                try { dv.Columns["ID"].ReadOnly = true; } catch { }
                try { dv.Columns["Section"].ReadOnly = true; } catch { }
                try { dv.Columns["Title"].ReadOnly = true; } catch { }
                try { dv.Columns["TitleURL"].ReadOnly = true; } catch { }
                try { dv.Columns["TitleLinks"].ReadOnly = true; } catch { }
                try { dv.Columns["DateAdded"].ReadOnly = true; } catch { }
                try { dv.Columns["DateModified"].ReadOnly = true; } catch { }
                try { dv.Columns["InCollection"].ReadOnly = true; } catch { }
            }

            #endregion
            #region ===== Novanan SQL Functions =====

            // Return Novanan SQL Connection String
            public SqlConnection Novanan_Conn()
            {
                // populate sql connection
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMLucidMedia"].ConnectionString);
                // SqlConnection Conn = new SqlConnection("Server=188.168.188.88;DataBase=LucidMedia;Uid=lucidm;Pwd=pass");
                return conn;
            }

            // Return Novanan TableName (Full Path)
            public string Novanan_TableName()
            {
                // populate to return full sql table name
                return "[LucidMedia].[dbo].[Novanan]";
            }


            public bool Novanan_InsertSQL(Novanan obj)
            {
                SqlConnection Con = Novanan_Conn();
                string SQLLine = "Insert Into " + Novanan_TableName() + " (IMDb, LastPostDate, Section, Title, TitleURL, TitleLinks, DateAdded, DateModified, InCollection) VALUES (@IMDb, @LastPostDate, @Section, @Title, @TitleURL, @TitleLinks, @DateAdded, @DateModified, @InCollection)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.Section == null) { obj.Section = ""; }
                if (obj.Title == null) { obj.Title = ""; }
                if (obj.TitleURL == null) { obj.TitleURL = ""; }
                if (obj.TitleLinks == null) { obj.TitleLinks = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = ""; }
                if (obj.DateModified == null) { obj.DateModified = ""; }
                if (obj.InCollection == null) { obj.InCollection = ""; }

                cmd2.Parameters.AddWithValue(@"Section", obj.Section.ToString());
                cmd2.Parameters.AddWithValue(@"Title", obj.Title.ToString());
                cmd2.Parameters.AddWithValue(@"TitleURL", obj.TitleURL.ToString());
                cmd2.Parameters.AddWithValue(@"TitleLinks", obj.TitleLinks.ToString());
                cmd2.Parameters.AddWithValue(@"DateAdded", DateTime.Now.ToString());
                cmd2.Parameters.AddWithValue(@"DateModified", DateTime.Now.ToString());
                cmd2.Parameters.AddWithValue(@"InCollection", obj.InCollection.ToString());
                cmd2.Parameters.AddWithValue(@"LastPostDate", obj.LastPostDate);
                cmd2.Parameters.AddWithValue(@"IMDb", obj.IMDb);

                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        Novanan_InsertSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        Novanan_InsertSQL(obj);
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

            public bool Novanan_UpdateSQL(Novanan obj)
            {
                SqlConnection Conn = Novanan_Conn();
                string SQLLine = "Update " + Novanan_TableName() + " SET IMDb = @IMDb, LastPostDate = @LastPostDate, Section = @Section, Title = @Title, TitleURL = @TitleURL, TitleLinks = @TitleLinks, DateModified = @DateModified, InCollection = @InCollection WHERE TitleURL = @TitleURL";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.ID == null) { obj.ID = ""; }
                if (obj.Section == null) { obj.Section = ""; }
                if (obj.Title == null) { obj.Title = ""; }
                if (obj.TitleURL == null) { obj.TitleURL = ""; }
                if (obj.TitleLinks == null) { obj.TitleLinks = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = ""; }
                if (obj.DateModified == null) { obj.DateModified = ""; }
                if (obj.InCollection == null) { obj.InCollection = ""; }
                if (obj.LastPostDate == null) { obj.LastPostDate = ""; }

                cmd2.Parameters.AddWithValue(@"Section", obj.Section.ToString());
                cmd2.Parameters.AddWithValue(@"Title", obj.Title.ToString());
                cmd2.Parameters.AddWithValue(@"TitleURL", obj.TitleURL.ToString());
                cmd2.Parameters.AddWithValue(@"TitleLinks", obj.TitleLinks.ToString());
                cmd2.Parameters.AddWithValue(@"DateModified", DateTime.Now.ToString());
                cmd2.Parameters.AddWithValue(@"InCollection", obj.InCollection.ToString());
                cmd2.Parameters.AddWithValue(@"LastPostDate", obj.LastPostDate);
                cmd2.Parameters.AddWithValue(@"IMDb", obj.IMDb);

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        Novanan_UpdateSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        Novanan_UpdateSQL(obj);
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


            public bool Novanan_UpdateLinks(Novanan obj)
            {
                SqlConnection Conn = Novanan_Conn();
                string SQLLine = "Update " + Novanan_TableName() + " SET TitleLinks = @TitleLinks, DateModified = @DateModified WHERE TitleURL = @TitleURL";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.ID == null) { obj.ID = ""; }
                if (obj.Section == null) { obj.Section = ""; }
                if (obj.Title == null) { obj.Title = ""; }
                if (obj.TitleURL == null) { obj.TitleURL = ""; }
                if (obj.TitleLinks == null) { obj.TitleLinks = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = ""; }
                if (obj.DateModified == null) { obj.DateModified = ""; }
                if (obj.InCollection == null) { obj.InCollection = ""; }
                if (obj.LastPostDate == null) { obj.LastPostDate = ""; }

                cmd2.Parameters.AddWithValue(@"TitleURL", obj.TitleURL.ToString());
                cmd2.Parameters.AddWithValue(@"TitleLinks", obj.TitleLinks.ToString());
                cmd2.Parameters.AddWithValue(@"DateModified", DateTime.Now.ToString());


                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        Novanan_UpdateSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        Novanan_UpdateSQL(obj);
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

            public bool Novanan_UpdateInsert(Novanan obj)
            {
                SqlConnection Conn = Novanan_Conn();
                bool Updated = Novanan_UpdateSQL(obj);  // try to update record first
                if (!Updated) { Updated = Novanan_InsertSQL(obj); }  // if unable to update, insert new record
                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool Novanan_UpdateIfPopulated(Novanan obj, string ID = "")
            {
                SqlConnection Conn = Novanan_Conn();
                string SQLcmd = "Update " + Novanan_TableName() + " SET ";
                if (obj.ID != null) { SQLcmd = SQLcmd + " ID = @ID,"; }
                if (obj.Section != null) { SQLcmd = SQLcmd + " Section = @Section,"; }
                if (obj.Title != null) { SQLcmd = SQLcmd + " Title = @Title,"; }
                if (obj.TitleURL != null) { SQLcmd = SQLcmd + " TitleURL = @TitleURL,"; }
                if (obj.TitleLinks != null) { SQLcmd = SQLcmd + " TitleLinks = @TitleLinks,"; }
                if (obj.DateAdded != null) { SQLcmd = SQLcmd + " DateAdded = @DateAdded,"; }
                if (obj.DateModified != null) { SQLcmd = SQLcmd + " DateModified = @DateModified,"; }
                if (obj.InCollection != null) { SQLcmd = SQLcmd + " InCollection = @InCollection,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ID = @ID";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.ID != null) { cmd2.Parameters.AddWithValue(@"ID", obj.ID); }
                if (obj.Section != null) { cmd2.Parameters.AddWithValue(@"Section", obj.Section); }
                if (obj.Title != null) { cmd2.Parameters.AddWithValue(@"Title", obj.Title); }
                if (obj.TitleURL != null) { cmd2.Parameters.AddWithValue(@"TitleURL", obj.TitleURL); }
                if (obj.TitleLinks != null) { cmd2.Parameters.AddWithValue(@"TitleLinks", obj.TitleLinks); }
                if (obj.DateAdded != null) { cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded); }
                if (obj.DateModified != null) { cmd2.Parameters.AddWithValue(@"DateModified", obj.DateModified); }
                if (obj.InCollection != null) { cmd2.Parameters.AddWithValue(@"InCollection", obj.InCollection); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        Novanan_UpdateIfPopulated(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        Novanan_UpdateIfPopulated(obj);
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

            public Novanan Novanan_ReturnSQL(string ID = "")
            {
                SqlConnection Conn = Novanan_Conn();
                string SelectLine = "Select [ID],[Section],[Title],[TitleURL],[TitleLinks],[DateAdded],[DateModified],[InCollection],[IMDb] From " + Novanan_TableName() + " WHERE ID = '" + ID + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                Novanan returnObject = new Novanan();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.Section = ret["Section"].ToString();
                        returnObject.Title = ret["Title"].ToString();
                        returnObject.TitleURL = ret["TitleURL"].ToString();
                        returnObject.TitleLinks = ret["TitleLinks"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.DateModified = ret["DateModified"].ToString();
                        returnObject.InCollection = ret["InCollection"].ToString();
                        returnObject.IMDb = ret["IMDb"].ToString();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public List<Novanan> Novanan_ReturnSQLList(string Command = "")
            {
                if (Command == "") { Command = "Select * From " + Novanan_TableName() + " WHERE InCollection = '0' and IMDb != '' and TitleLinks != ''"; }
                SqlConnection Conn = Novanan_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<Novanan> ReturnList = new List<Novanan>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        Novanan returnObject = new Novanan();
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.Section = ret["Section"].ToString();
                        returnObject.Title = ret["Title"].ToString();
                        returnObject.TitleURL = ret["TitleURL"].ToString();
                        returnObject.TitleLinks = ret["TitleLinks"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.DateModified = ret["DateModified"].ToString();
                        returnObject.InCollection = ret["InCollection"].ToString();
                        returnObject.IMDb = ret["IMDb"].ToString();
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

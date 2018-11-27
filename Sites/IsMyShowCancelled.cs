using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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
using System.ComponentModel;

namespace sharpAHK_Dev
{
    public partial class _Sites
    {
        public class IsMyShowCancelled
        {

            private void btnOnAirShows_Click(object sender, EventArgs e)
            {
                _AHK ahk = new _AHK();

                IsMyShowCancelled isc = new IsMyShowCancelled();

                bool WriteSQL = true;

                RadButton clicked = (RadButton)sender;

                List<ShowCancelledInfo> segs = new List<ShowCancelledInfo>();

                if (clicked.Text == "All Shows") { segs = isc.ReturnShows(IsMyShowCancelled.ShowStatus.All, WriteSQL); }
                if (clicked.Text == "Cancelled Shows") { segs = isc.ReturnShows(IsMyShowCancelled.ShowStatus.Cancelled, WriteSQL); }
                if (clicked.Text == "Coming Soon") { segs = isc.ReturnShows(IsMyShowCancelled.ShowStatus.ComingSoon, WriteSQL); }
                if (clicked.Text == "Concluded Shows") { segs = isc.ReturnShows(IsMyShowCancelled.ShowStatus.Concluded, WriteSQL); }
                if (clicked.Text == "On Air Shows") { segs = isc.ReturnShows(IsMyShowCancelled.ShowStatus.OnAir, WriteSQL); }

                //foreach(ShowCancelledInfo show in segs)
                //{
                //    ahk.MsgBox(show.ShowName + "\n\n" + show.Description + "\n\n" + show.Genre + "\n\n" + show.ImageURL + "\n\n" + show.Network + "\n\n" + show.ShowStatus + "\n\n" + show.ShowURL + "\n\n" + show.Years);
                //}

                ahk.MsgBox("Finished Returning " + segs.Count + " Shows");

            }


            //int OnAirShowsPageCount = 60;
            //int CancelledShowsPageCount = 85;
            //int ConcludedShowsPageCount = 15;
            //int NewShowsPageCount = 3;

            string OnAirShowsURL = "http://www.ismyshowcancelled.com/shows/onair/page/";
            string CancelledShowsURL = "http://www.ismyshowcancelled.com/shows/cancelled/page/";
            string ConcludedShowsURL = "http://www.ismyshowcancelled.com/shows/concluded/page/";
            string NewShowsURL = "http://www.ismyshowcancelled.com/shows/new/page/";
            string ALLShowsURL = "http://www.ismyshowcancelled.com/shows/all/page/";


            [DefaultValue(Cancelled)]
            public enum ShowStatus
            {
                All,
                OnAir,
                Cancelled,
                Concluded,
                ComingSoon
            }

            public List<ShowCancelledInfo> OnAirShows()
            {
                return ReturnShows(ShowStatus.OnAir);
            }

            public class ShowCancelledInfo
            {
                public string ShowURL { get; set; }
                public string ImageURL { get; set; }
                public string ShowStatus { get; set; }
                public string ShowName { get; set; }
                public string Years { get; set; }
                public string Network { get; set; }
                public string Genre { get; set; }
                public string Description { get; set; }
                public string IMDb { get; set; }
            }

            public List<ShowCancelledInfo> ReturnShows(ShowStatus status, bool WriteSQL = false)
            {
                _AHK ahk = new _AHK();

                string startURL = "";
                if (status == ShowStatus.OnAir) { startURL = OnAirShowsURL; }
                if (status == ShowStatus.Concluded) { startURL = ConcludedShowsURL; }
                if (status == ShowStatus.Cancelled) { startURL = CancelledShowsURL; }
                if (status == ShowStatus.ComingSoon) { startURL = NewShowsURL; }
                if (status == ShowStatus.All) { startURL = ALLShowsURL; }

                bool RunLoop = true;
                List<string> segs = new List<string>();
                List<ShowCancelledInfo> showInfos = new List<ShowCancelledInfo>();
                int i = 1;
                do
                {
                    string URL = startURL + i + "/";

                    string html = ahk.Download_HTML(URL);

                    bool capture = false; string seg = ""; int divCount = 0; int AddedCount = 0;
                    List<string> lines = html.ToList(true, true, false);
                    foreach (string line in lines)
                    {
                        if (line.Contains("<div class=\"shows-item\">")) { capture = true; divCount = 0; continue; }

                        if (capture)
                        {
                            if (line.Contains("</div>")) { divCount++; }

                            if (seg == "") { seg = line; }
                            else { seg = seg + "\n" + line; }
                        }

                        if (divCount == 7)
                        {
                            ShowCancelledInfo info = new ShowCancelledInfo();

                            //ahk.MsgBox(seg);

                            List<string> parts = ahk.StringSplit_List(seg, "\n");

                            foreach (string part in parts)
                            {
                                //ahk.MsgBox(part);
                                if (part.Contains("title="))
                                {
                                    List<string> pts = ahk.StringSplit_List(part, "\"");
                                    info.ShowURL = pts[1];
                                    info.ShowName = pts[3];
                                }
                                if (part.Contains("<img src="))
                                {
                                    List<string> pts = ahk.StringSplit_List(part, "\"");
                                    info.ImageURL = pts[1];
                                }
                                if (part.Contains("class='status"))
                                {
                                    List<string> pts = ahk.StringSplit_List(part, ">");
                                    info.ShowStatus = pts[4].Replace("</span", "");
                                }

                                if (part.Contains("Years:"))
                                {
                                    List<string> pts = ahk.StringSplit_List(part, ">");
                                    info.Years = pts[4].Replace("</span", "");
                                }
                                if (part.Contains("Network:"))
                                {
                                    List<string> pts = ahk.StringSplit_List(part, ">");
                                    info.Network = pts[4].Replace("</span", "");
                                }
                                if (part.Contains("Genre:"))
                                {
                                    List<string> pts = ahk.StringSplit_List(part, ">");
                                    info.Genre = pts[4].Replace("</span", "");
                                }

                                if (part.Contains("<div class='excerpt'>"))
                                {
                                    string before = part.Replace("<div class='excerpt'>", "");
                                    List<string> pts = ahk.StringSplit_List(before, "<");
                                    info.Description = pts[0].Replace("&nbsp;", "");
                                }
                            }

                            if (WriteSQL) { ShowCancelledInfo_UpdateInsert(info); }


                            showInfos.Add(info);
                            AddedCount++;
                            // reset capture counters, add captured segment to list of segs to parse next
                            capture = false; divCount = 0; segs.Add(seg); seg = "";
                        }
                    }


                    if (AddedCount < 10) { RunLoop = false; }  // didn't return a full page, end of loop process

                    i++;
                }
                while (RunLoop);

                //ahk.MsgBox("Status: " + status.ToString() + " | Captured " + segs.Count + " Segments");

                return showInfos;
            }


            #region === ShowCancelledInfo FUNCTIONS ===

            #region ===== ShowCancelledInfo Object =====


            //  Fix illegal characters before Sql/Sqlite Db Inserts
            public ShowCancelledInfo ShowCancelledInfo_FixChars(ShowCancelledInfo ToFix)
            {
                ShowCancelledInfo Fixed = new ShowCancelledInfo();

                Fixed.ShowName = ToFix.ShowName.Replace("'", "''");
                Fixed.ShowStatus = ToFix.ShowStatus.Replace("'", "''");
                Fixed.Genre = ToFix.Genre.Replace("'", "''");
                Fixed.Network = ToFix.Network.Replace("'", "''");
                Fixed.Years = ToFix.Years.Replace("'", "''");
                Fixed.Description = ToFix.Description.Replace("'", "''");
                Fixed.ShowURL = ToFix.ShowURL.Replace("'", "''");
                Fixed.ImageURL = ToFix.ImageURL.Replace("'", "''");
                Fixed.IMDb = ToFix.IMDb.Replace("'", "''");

                return Fixed;
            }

            // Compare two objects to see if they have identical values
            public bool ShowCancelledInfo_Changed(ShowCancelledInfo OldVal, ShowCancelledInfo NewVal)
            {
                ShowCancelledInfo diff = new ShowCancelledInfo();
                List<string> diffList = new List<string>();
                bool different = false;
                if (OldVal.ShowName == null) { OldVal.ShowName = ""; }
                if (NewVal.ShowName == null) { NewVal.ShowName = ""; }
                if (OldVal.ShowName != NewVal.ShowName) { different = true; }
                if (OldVal.ShowStatus == null) { OldVal.ShowStatus = ""; }
                if (NewVal.ShowStatus == null) { NewVal.ShowStatus = ""; }
                if (OldVal.ShowStatus != NewVal.ShowStatus) { different = true; }
                if (OldVal.Genre == null) { OldVal.Genre = ""; }
                if (NewVal.Genre == null) { NewVal.Genre = ""; }
                if (OldVal.Genre != NewVal.Genre) { different = true; }
                if (OldVal.Network == null) { OldVal.Network = ""; }
                if (NewVal.Network == null) { NewVal.Network = ""; }
                if (OldVal.Network != NewVal.Network) { different = true; }
                if (OldVal.Years == null) { OldVal.Years = ""; }
                if (NewVal.Years == null) { NewVal.Years = ""; }
                if (OldVal.Years != NewVal.Years) { different = true; }
                if (OldVal.Description == null) { OldVal.Description = ""; }
                if (NewVal.Description == null) { NewVal.Description = ""; }
                if (OldVal.Description != NewVal.Description) { different = true; }
                if (OldVal.ShowURL == null) { OldVal.ShowURL = ""; }
                if (NewVal.ShowURL == null) { NewVal.ShowURL = ""; }
                if (OldVal.ShowURL != NewVal.ShowURL) { different = true; }
                if (OldVal.ImageURL == null) { OldVal.ImageURL = ""; }
                if (NewVal.ImageURL == null) { NewVal.ImageURL = ""; }
                if (OldVal.ImageURL != NewVal.ImageURL) { different = true; }
                if (OldVal.IMDb == null) { OldVal.IMDb = ""; }
                if (NewVal.IMDb == null) { NewVal.IMDb = ""; }
                if (OldVal.IMDb != NewVal.IMDb) { different = true; }
                return different;
            }

            // Returns object containing the new values different from the old values in object comparison
            public ShowCancelledInfo ShowCancelledInfo_Diff(ShowCancelledInfo OldVal, ShowCancelledInfo NewVal)
            {
                ShowCancelledInfo diff = new ShowCancelledInfo();
                if (OldVal.ShowName != NewVal.ShowName) { diff.ShowName = NewVal.ShowName; }
                if (OldVal.ShowStatus != NewVal.ShowStatus) { diff.ShowStatus = NewVal.ShowStatus; }
                if (OldVal.Genre != NewVal.Genre) { diff.Genre = NewVal.Genre; }
                if (OldVal.Network != NewVal.Network) { diff.Network = NewVal.Network; }
                if (OldVal.Years != NewVal.Years) { diff.Years = NewVal.Years; }
                if (OldVal.Description != NewVal.Description) { diff.Description = NewVal.Description; }
                if (OldVal.ShowURL != NewVal.ShowURL) { diff.ShowURL = NewVal.ShowURL; }
                if (OldVal.ImageURL != NewVal.ImageURL) { diff.ImageURL = NewVal.ImageURL; }
                if (OldVal.IMDb != NewVal.IMDb) { diff.IMDb = NewVal.IMDb; }
                return diff;
            }

            // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
            public List<string> ShowCancelledInfo_DiffList(ShowCancelledInfo OldVal, ShowCancelledInfo NewVal)
            {
                List<string> diffList = new List<string>();
                if (OldVal.ShowName != NewVal.ShowName) { diffList.Add("Changed ShowName Value From " + OldVal.ShowName + " To " + NewVal.ShowName); }
                if (OldVal.ShowStatus != NewVal.ShowStatus) { diffList.Add("Changed ShowStatus Value From " + OldVal.ShowStatus + " To " + NewVal.ShowStatus); }
                if (OldVal.Genre != NewVal.Genre) { diffList.Add("Changed Genre Value From " + OldVal.Genre + " To " + NewVal.Genre); }
                if (OldVal.Network != NewVal.Network) { diffList.Add("Changed Network Value From " + OldVal.Network + " To " + NewVal.Network); }
                if (OldVal.Years != NewVal.Years) { diffList.Add("Changed Years Value From " + OldVal.Years + " To " + NewVal.Years); }
                if (OldVal.Description != NewVal.Description) { diffList.Add("Changed Description Value From " + OldVal.Description + " To " + NewVal.Description); }
                if (OldVal.ShowURL != NewVal.ShowURL) { diffList.Add("Changed ShowURL Value From " + OldVal.ShowURL + " To " + NewVal.ShowURL); }
                if (OldVal.ImageURL != NewVal.ImageURL) { diffList.Add("Changed ImageURL Value From " + OldVal.ImageURL + " To " + NewVal.ImageURL); }
                if (OldVal.IMDb != NewVal.IMDb) { diffList.Add("Changed IMDb Value From " + OldVal.IMDb + " To " + NewVal.IMDb); }
                return diffList;
            }

            // Generate XML String From Object
            public string ShowCancelledInfo_ToXML(ShowCancelledInfo obj)
            {
                string XMLString = "";
                XMLString = XMLString + "<ShowName>" + obj.ShowName + "</ShowName>";
                XMLString = XMLString + "<ShowStatus>" + obj.ShowStatus + "</ShowStatus>";
                XMLString = XMLString + "<Genre>" + obj.Genre + "</Genre>";
                XMLString = XMLString + "<Network>" + obj.Network + "</Network>";
                XMLString = XMLString + "<Years>" + obj.Years + "</Years>";
                XMLString = XMLString + "<Description>" + obj.Description + "</Description>";
                XMLString = XMLString + "<ShowURL>" + obj.ShowURL + "</ShowURL>";
                XMLString = XMLString + "<ImageURL>" + obj.ImageURL + "</ImageURL>";
                XMLString = XMLString + "<IMDb>" + obj.IMDb + "</IMDb>";
                return XMLString;
            }

            // Populate Object from XML Tag String
            public ShowCancelledInfo ShowCancelledInfo_FromXML(string XMLString)
            {
                _Parse prs = new _Parse();
                ShowCancelledInfo obj = new ShowCancelledInfo();
                obj.ShowName = prs.XML_Text(XMLString, "<ShowName>");
                obj.ShowStatus = prs.XML_Text(XMLString, "<ShowStatus>");
                obj.Genre = prs.XML_Text(XMLString, "<Genre>");
                obj.Network = prs.XML_Text(XMLString, "<Network>");
                obj.Years = prs.XML_Text(XMLString, "<Years>");
                obj.Description = prs.XML_Text(XMLString, "<Description>");
                obj.ShowURL = prs.XML_Text(XMLString, "<ShowURL>");
                obj.ImageURL = prs.XML_Text(XMLString, "<ImageURL>");
                obj.IMDb = prs.XML_Text(XMLString, "<IMDb>");
                return obj;
            }


            #endregion
            public bool Create_Table_IsMyShow(string DbFile)
            {
                _AHK ahk = new _AHK();
                _Database.SQLite sqlite = new _Database.SQLite();
                string CreateLine = "Create Table [IsMyShow] (ShowName VARCHAR, ShowStatus VARCHAR, Genre VARCHAR, Network VARCHAR, Years VARCHAR, Description VARCHAR, ShowURL VARCHAR, ImageURL VARCHAR, IMDb VARCHAR)";
                bool TableCreated = sqlite.Table_Exists(DbFile, "IsMyShow");
                if (!TableCreated) { TableCreated = sqlite.Table_New(DbFile, "IsMyShow", "Create Table [IsMyShow] (ShowName VARCHAR, ShowStatus VARCHAR, Genre VARCHAR, Network VARCHAR, Years VARCHAR, Description VARCHAR, ShowURL VARCHAR, ImageURL VARCHAR, IMDb VARCHAR", false); }


                if (!TableCreated) { ahk.MsgBox("[IsMyShow] Created = " + TableCreated.ToString()); }
                return TableCreated;
            }

            #region ===== ShowCancelledInfo SQLite : Return =====

            public ShowCancelledInfo Return_Object_From_IsMyShow(string WhereClause = "[ShowName] = ''", string DbFile = "")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                _AHK ahk = new _AHK();
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\ShowCancelledInfo.sqlite"; }
                string SelectLine = "Select [ShowName], [ShowStatus], [Genre], [Network], [Years], [Description], [ShowURL], [ImageURL], [IMDb] From [IsMyShow] ";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
                ShowCancelledInfo returnObject = new ShowCancelledInfo();
                int i = 0;
                string Value = "";
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.ShowStatus = ret["ShowStatus"].ToString();
                        returnObject.Genre = ret["Genre"].ToString();
                        returnObject.Network = ret["Network"].ToString();
                        returnObject.Years = ret["Years"].ToString();
                        returnObject.Description = ret["Description"].ToString();
                        returnObject.ShowURL = ret["ShowURL"].ToString();
                        returnObject.ImageURL = ret["ImageURL"].ToString();
                        returnObject.IMDb = ret["IMDb"].ToString();
                    }
                }

                return returnObject;
            }

            public List<ShowCancelledInfo> Return_ShowCancelledInfo_List(string WhereClause = "", string DbFile = "", string TableName = "[IsMyShow]")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                _AHK ahk = new _AHK();
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\ShowCancelledInfo.sqlite"; }
                string SelectLine = "Select * From " + TableName;

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

                List<ShowCancelledInfo> ReturnList = new List<ShowCancelledInfo>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        ShowCancelledInfo returnObject = new ShowCancelledInfo();

                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.ShowStatus = ret["ShowStatus"].ToString();
                        returnObject.Genre = ret["Genre"].ToString();
                        returnObject.Network = ret["Network"].ToString();
                        returnObject.Years = ret["Years"].ToString();
                        returnObject.Description = ret["Description"].ToString();
                        returnObject.ShowURL = ret["ShowURL"].ToString();
                        returnObject.ImageURL = ret["ImageURL"].ToString();
                        returnObject.IMDb = ret["IMDb"].ToString();

                        ReturnList.Add(returnObject);
                    }
                }

                return ReturnList;
            }

            public DataTable Return_DataTable_From_IsMyShow(string DbFile)
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                string SelectLine = "Select [ShowName], [ShowStatus], [Genre], [Network], [Years], [Description], [ShowURL], [ImageURL], [IMDb] From [IsMyShow]";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                return ReturnTable;
            }


            #endregion
            #region ===== ShowCancelledInfo SQLite : Update Insert =====

            public bool ShowCancelledInfo_Insert(ShowCancelledInfo inObject, string DbFile = "")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                _AHK ahk = new _AHK();
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\ShowCancelledInfo.sqlite"; }
                string InsertLine = "Insert Into [IsMyShow] (ShowName, ShowStatus, Genre, Network, Years, Description, ShowURL, ImageURL, IMDb) values ('" + inObject.ShowName + "', '" + inObject.ShowStatus + "', '" + inObject.Genre + "', '" + inObject.Network + "', '" + inObject.Years + "', '" + inObject.Description + "', '" + inObject.ShowURL + "', '" + inObject.ImageURL + "', '" + inObject.IMDb + "')";
                bool Inserted = sqlite.Execute(DbFile, InsertLine);
                if (!Inserted) { ahk.MsgBox("Inserted Into [IsMyShow] = " + Inserted.ToString()); }
                return Inserted;
            }

            public bool ShowCancelledInfo_Update(ShowCancelledInfo inObject, string DbFile = "")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                _AHK ahk = new _AHK();
                //string UpdateLine = "Update [IsMyShow] set ShowName = '" + inObject.ShowName + "', ShowStatus = '" + inObject.ShowStatus + "', Genre = '" + inObject.Genre + "', Network = '" + inObject.Network + "', Years = '" + inObject.Years + "', Description = '" + inObject.Description + "', ShowURL = '" + inObject.ShowURL + "', ImageURL = '" + inObject.ImageURL + "', IMDb = '" + inObject.IMDb + "' WHERE [Item] = 'Value' ";
                string UpdateLine = "Update [IsMyShow] set ";


                if (inObject.ShowName != null) { UpdateLine = UpdateLine + "[ShowName] = '" + inObject.ShowName + "',"; }
                if (inObject.ShowStatus != null) { UpdateLine = UpdateLine + "[ShowStatus] = '" + inObject.ShowStatus + "',"; }
                if (inObject.Genre != null) { UpdateLine = UpdateLine + "[Genre] = '" + inObject.Genre + "',"; }
                if (inObject.Network != null) { UpdateLine = UpdateLine + "[Network] = '" + inObject.Network + "',"; }
                if (inObject.Years != null) { UpdateLine = UpdateLine + "[Years] = '" + inObject.Years + "',"; }
                if (inObject.Description != null) { UpdateLine = UpdateLine + "[Description] = '" + inObject.Description + "',"; }
                if (inObject.ShowURL != null) { UpdateLine = UpdateLine + "[ShowURL] = '" + inObject.ShowURL + "',"; }
                if (inObject.ImageURL != null) { UpdateLine = UpdateLine + "[ImageURL] = '" + inObject.ImageURL + "',"; }
                if (inObject.IMDb != null) { UpdateLine = UpdateLine + "[IMDb] = '" + inObject.IMDb + "',"; }

                UpdateLine = ahk.TrimLast(UpdateLine, 1);
                UpdateLine = UpdateLine + " WHERE [ShowName] = ' '"; // DEFINE CONDITION HERE !!!

                bool Updated = sqlite.Execute(DbFile, UpdateLine);
                return Updated;
            }

            public bool ShowCancelledInfo_UpdateInsert(ShowCancelledInfo obj, string DbFile = "")
            {
                _AHK ahk = new _AHK();
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\ShowCancelledInfo.sqlite"; }
                if (!File.Exists(DbFile)) { Create_Table_IsMyShow(DbFile); }

                bool Updated = ShowCancelledInfo_Update(obj, DbFile);  // try to update record first
                if (!Updated) { Updated = ShowCancelledInfo_Insert(obj, DbFile); }  // if unable to update, insert new record
                return Updated;
            }


            #endregion
            #region ===== ShowCancelledInfo DataTable =====

            public DataTable Return_ShowCancelledInfo_DataTable(string DbFile = "", string TableName = "ShowCancelledInfo", string WhereClause = "", bool Debug = false)
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                _AHK ahk = new _AHK();
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\ShowCancelledInfo.sqlite"; }
                string SelectLine = "Select * From [IsMyShow]";

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }

                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


                DataTable table = new DataTable();
                table.Columns.Add("ShowName", typeof(string));
                table.Columns.Add("ShowStatus", typeof(string));
                table.Columns.Add("Genre", typeof(string));
                table.Columns.Add("Network", typeof(string));
                table.Columns.Add("Years", typeof(string));
                table.Columns.Add("Description", typeof(string));
                table.Columns.Add("ShowURL", typeof(string));
                table.Columns.Add("ImageURL", typeof(string));
                table.Columns.Add("IMDb", typeof(string));

                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        ShowCancelledInfo returnObject = new ShowCancelledInfo();

                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.ShowStatus = ret["ShowStatus"].ToString();
                        returnObject.Genre = ret["Genre"].ToString();
                        returnObject.Network = ret["Network"].ToString();
                        returnObject.Years = ret["Years"].ToString();
                        returnObject.Description = ret["Description"].ToString();
                        returnObject.ShowURL = ret["ShowURL"].ToString();
                        returnObject.ImageURL = ret["ImageURL"].ToString();
                        returnObject.IMDb = ret["IMDb"].ToString();

                        table.Rows.Add(returnObject.ShowName, returnObject.ShowStatus, returnObject.Genre, returnObject.Network, returnObject.Years, returnObject.Description, returnObject.ShowURL, returnObject.ImageURL, returnObject.IMDb);
                    }
                }

                return table;
            }

            public DataTable Create_IsMyShow_DataTable(ShowCancelledInfo inObject)
            {
                DataTable table = new DataTable();
                table.Columns.Add("ShowName", typeof(string));
                table.Columns.Add("ShowStatus", typeof(string));
                table.Columns.Add("Genre", typeof(string));
                table.Columns.Add("Network", typeof(string));
                table.Columns.Add("Years", typeof(string));
                table.Columns.Add("Description", typeof(string));
                table.Columns.Add("ShowURL", typeof(string));
                table.Columns.Add("ImageURL", typeof(string));
                table.Columns.Add("IMDb", typeof(string));

                table.Rows.Add(inObject.ShowName, inObject.ShowStatus, inObject.Genre, inObject.Network, inObject.Years, inObject.Description, inObject.ShowURL, inObject.ImageURL, inObject.IMDb);
                return table;
            }


            #endregion
            #region ===== ShowCancelledInfo DataGridView =====

            public void HideShow_IsMyShow_Columns(DataGridView dv)
            {

                try { dv.Columns["ShowName"].Visible = true; } catch { }
                try { dv.Columns["ShowStatus"].Visible = true; } catch { }
                try { dv.Columns["Genre"].Visible = true; } catch { }
                try { dv.Columns["Network"].Visible = true; } catch { }
                try { dv.Columns["Years"].Visible = true; } catch { }
                try { dv.Columns["Description"].Visible = true; } catch { }
                try { dv.Columns["ShowURL"].Visible = true; } catch { }
                try { dv.Columns["ImageURL"].Visible = true; } catch { }
                try { dv.Columns["IMDb"].Visible = true; } catch { }
            }
            public void Enable_IsMyShow_Columns(DataGridView dv)
            {

                try { dv.Columns["ShowName"].ReadOnly = true; } catch { }
                try { dv.Columns["ShowStatus"].ReadOnly = true; } catch { }
                try { dv.Columns["Genre"].ReadOnly = true; } catch { }
                try { dv.Columns["Network"].ReadOnly = true; } catch { }
                try { dv.Columns["Years"].ReadOnly = true; } catch { }
                try { dv.Columns["Description"].ReadOnly = true; } catch { }
                try { dv.Columns["ShowURL"].ReadOnly = true; } catch { }
                try { dv.Columns["ImageURL"].ReadOnly = true; } catch { }
                try { dv.Columns["IMDb"].ReadOnly = true; } catch { }
            }

            #endregion
            #region ===== ShowCancelledInfo SQL Functions =====

            // Return ShowCancelledInfo SQL Connection String
            public SqlConnection ShowCancelledInfo_Conn()
            {
                // populate sql connection
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMLucidMedia"].ConnectionString);
                // SqlConnection Conn = new SqlConnection("Server=188.168.188.88;DataBase=LucidMedia;Uid=lucidm;Pwd=pass");
                return conn;
            }

            // Return ShowCancelledInfo TableName (Full Path)
            public string ShowCancelledInfo_TableName()
            {
                // populate to return full sql table name
                return "[LucidMedia].[dbo].[IsMyShow]";
            }

            // Generate SQL Table
            public bool ShowCancelledInfo_CreateSQLTable()
            {
                SqlConnection Conn = ShowCancelledInfo_Conn();
                string CreateTableLine = "CREATE TABLE [IsMyShow](";
                CreateTableLine = CreateTableLine + "[ShowName] [int] IDENTITY(1,1) NOT NULL,";
                CreateTableLine = CreateTableLine + "[ShowStatus] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Genre] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Network] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Years] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Description] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[ShowURL] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[ImageURL] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[IMDb] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[TimeStamp] [datetime] NULL";
                CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                return false;
            }

            public bool ShowCancelledInfo_InsertSQL(ShowCancelledInfo obj)
            {
                _AHK ahk = new _AHK();
                SqlConnection Con = ShowCancelledInfo_Conn();
                string SQLLine = "Insert Into " + ShowCancelledInfo_TableName() + " (ShowName, ShowStatus, Genre, Network, Years, Description, ShowURL, ImageURL, IMDb, TimeStamp) VALUES (@ShowName, @ShowStatus, @Genre, @Network, @Years, @Description, @ShowURL, @ImageURL, @IMDb, @TimeStamp)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.ShowName == null) { obj.ShowName = ""; }
                if (obj.ShowStatus == null) { obj.ShowStatus = ""; }
                if (obj.Genre == null) { obj.Genre = ""; }
                if (obj.Network == null) { obj.Network = ""; }
                if (obj.Years == null) { obj.Years = ""; }
                if (obj.Description == null) { obj.Description = ""; }
                if (obj.ShowURL == null) { obj.ShowURL = ""; }
                if (obj.ImageURL == null) { obj.ImageURL = ""; }
                if (obj.IMDb == null) { obj.IMDb = ""; }
                cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                cmd2.Parameters.AddWithValue(@"ShowStatus", obj.ShowStatus.ToString());
                cmd2.Parameters.AddWithValue(@"Genre", obj.Genre.ToString());
                cmd2.Parameters.AddWithValue(@"Network", obj.Network.ToString());
                cmd2.Parameters.AddWithValue(@"Years", obj.Years.ToString());
                cmd2.Parameters.AddWithValue(@"Description", obj.Description.ToString());
                cmd2.Parameters.AddWithValue(@"ShowURL", obj.ShowURL.ToString());
                cmd2.Parameters.AddWithValue(@"ImageURL", obj.ImageURL.ToString());
                cmd2.Parameters.AddWithValue(@"IMDb", obj.IMDb.ToString());
                cmd2.Parameters.AddWithValue(@"TimeStamp", DateTime.Now.ToString());

                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        ShowCancelledInfo_InsertSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        ShowCancelledInfo_InsertSQL(obj);
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

            public bool ShowCancelledInfo_UpdateSQL(ShowCancelledInfo obj)
            {
                _AHK ahk = new _AHK();
                SqlConnection Conn = ShowCancelledInfo_Conn();
                string SQLLine = "Update " + ShowCancelledInfo_TableName() + " SET ShowName = @ShowName, ShowStatus = @ShowStatus, Genre = @Genre, Network = @Network, Years = @Years, Description = @Description, ShowURL = @ShowURL, ImageURL = @ImageURL, IMDb = @IMDb, TimeStamp = @TimeStamp WHERE ShowName = @ShowName";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.ShowName == null) { obj.ShowName = ""; }
                if (obj.ShowStatus == null) { obj.ShowStatus = ""; }
                if (obj.Genre == null) { obj.Genre = ""; }
                if (obj.Network == null) { obj.Network = ""; }
                if (obj.Years == null) { obj.Years = ""; }
                if (obj.Description == null) { obj.Description = ""; }
                if (obj.ShowURL == null) { obj.ShowURL = ""; }
                if (obj.ImageURL == null) { obj.ImageURL = ""; }
                if (obj.IMDb == null) { obj.IMDb = ""; }

                cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                cmd2.Parameters.AddWithValue(@"ShowStatus", obj.ShowStatus.ToString());
                cmd2.Parameters.AddWithValue(@"Genre", obj.Genre.ToString());
                cmd2.Parameters.AddWithValue(@"Network", obj.Network.ToString());
                cmd2.Parameters.AddWithValue(@"Years", obj.Years.ToString());
                cmd2.Parameters.AddWithValue(@"Description", obj.Description.ToString());
                cmd2.Parameters.AddWithValue(@"ShowURL", obj.ShowURL.ToString());
                cmd2.Parameters.AddWithValue(@"ImageURL", obj.ImageURL.ToString());
                cmd2.Parameters.AddWithValue(@"IMDb", obj.IMDb.ToString());
                cmd2.Parameters.AddWithValue(@"TimeStamp", DateTime.Now.ToString());

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        ShowCancelledInfo_UpdateSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        ShowCancelledInfo_UpdateSQL(obj);
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

            public bool ShowCancelledInfo_UpdateInsert(ShowCancelledInfo obj)
            {
                SqlConnection Conn = ShowCancelledInfo_Conn();
                bool Updated = ShowCancelledInfo_UpdateSQL(obj);  // try to update record first
                if (!Updated) { Updated = ShowCancelledInfo_InsertSQL(obj); }  // if unable to update, insert new record
                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool ShowCancelledInfo_UpdateIfPopulated(ShowCancelledInfo obj, string ID = "")
            {
                _AHK ahk = new _AHK();
                SqlConnection Conn = ShowCancelledInfo_Conn();
                string SQLcmd = "Update " + ShowCancelledInfo_TableName() + " SET ";
                if (obj.ShowName != null) { SQLcmd = SQLcmd + " ShowName = @ShowName,"; }
                if (obj.ShowStatus != null) { SQLcmd = SQLcmd + " ShowStatus = @ShowStatus,"; }
                if (obj.Genre != null) { SQLcmd = SQLcmd + " Genre = @Genre,"; }
                if (obj.Network != null) { SQLcmd = SQLcmd + " Network = @Network,"; }
                if (obj.Years != null) { SQLcmd = SQLcmd + " Years = @Years,"; }
                if (obj.Description != null) { SQLcmd = SQLcmd + " Description = @Description,"; }
                if (obj.ShowURL != null) { SQLcmd = SQLcmd + " ShowURL = @ShowURL,"; }
                if (obj.ImageURL != null) { SQLcmd = SQLcmd + " ImageURL = @ImageURL,"; }
                if (obj.IMDb != null) { SQLcmd = SQLcmd + " IMDb = @IMDb,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ID = @ID";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.ShowName != null) { cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName); }
                if (obj.ShowStatus != null) { cmd2.Parameters.AddWithValue(@"ShowStatus", obj.ShowStatus); }
                if (obj.Genre != null) { cmd2.Parameters.AddWithValue(@"Genre", obj.Genre); }
                if (obj.Network != null) { cmd2.Parameters.AddWithValue(@"Network", obj.Network); }
                if (obj.Years != null) { cmd2.Parameters.AddWithValue(@"Years", obj.Years); }
                if (obj.Description != null) { cmd2.Parameters.AddWithValue(@"Description", obj.Description); }
                if (obj.ShowURL != null) { cmd2.Parameters.AddWithValue(@"ShowURL", obj.ShowURL); }
                if (obj.ImageURL != null) { cmd2.Parameters.AddWithValue(@"ImageURL", obj.ImageURL); }
                if (obj.IMDb != null) { cmd2.Parameters.AddWithValue(@"IMDb", obj.IMDb); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public ShowCancelledInfo ShowCancelledInfo_ReturnSQL(string ID = "")
            {
                _Database.SQL sql = new _Database.SQL();
                SqlConnection Conn = ShowCancelledInfo_Conn();
                string SelectLine = "Select [ShowName],[ShowStatus],[Genre],[Network],[Years],[Description],[ShowURL],[ImageURL],[IMDb] From " + ShowCancelledInfo_TableName() + " WHERE ID = '" + ID + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                ShowCancelledInfo returnObject = new ShowCancelledInfo();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.ShowStatus = ret["ShowStatus"].ToString();
                        returnObject.Genre = ret["Genre"].ToString();
                        returnObject.Network = ret["Network"].ToString();
                        returnObject.Years = ret["Years"].ToString();
                        returnObject.Description = ret["Description"].ToString();
                        returnObject.ShowURL = ret["ShowURL"].ToString();
                        returnObject.ImageURL = ret["ImageURL"].ToString();
                        returnObject.IMDb = ret["IMDb"].ToString();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public List<ShowCancelledInfo> ShowCancelledInfo_ReturnSQLList(string Command = "")
            {
                _Database.SQL sql = new _Database.SQL();
                if (Command == "") { Command = "Select * From ShowCancelledInfo_TableName()"; }
                SqlConnection Conn = ShowCancelledInfo_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<ShowCancelledInfo> ReturnList = new List<ShowCancelledInfo>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        ShowCancelledInfo returnObject = new ShowCancelledInfo();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.ShowStatus = ret["ShowStatus"].ToString();
                        returnObject.Genre = ret["Genre"].ToString();
                        returnObject.Network = ret["Network"].ToString();
                        returnObject.Years = ret["Years"].ToString();
                        returnObject.Description = ret["Description"].ToString();
                        returnObject.ShowURL = ret["ShowURL"].ToString();
                        returnObject.ImageURL = ret["ImageURL"].ToString();
                        returnObject.IMDb = ret["IMDb"].ToString();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }

            public bool ShowCancelledInfo_SQL_to_SQLite(string SqliteDBPath = @"\Db\ShowCancelledInfo.sqlite")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                _AHK ahk = new _AHK();
                string SaveFile = SqliteDBPath;
                if (SqliteDBPath == @"\Db\ShowCancelledInfo.sqlite")
                {
                    ahk.FileCreateDir(ahk.AppDir() + @"\Db");
                    SaveFile = ahk.AppDir() + @"\Db\ShowCancelledInfo.sqlite";
                }

                //sb.StatusBar("Copying SQL Db to " + SaveFile + "...");
                sqlite.SQLTable_To_NewSQLiteTable(ShowCancelledInfo_Conn(), "IsMyShow", "IsMyShow", SaveFile, "", false, false, false);
                //sb.StatusBar("FINISHED Copying SQL Db to " + SaveFile);

                if (File.Exists(SaveFile)) { return true; } else { return false; }
            }


            #endregion

            #endregion

        }
    }
}

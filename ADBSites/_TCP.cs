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
        public class TCp
        {

            _AHK ahk = new _AHK();
            _Database.SQL sql = new _Database.SQL();
            _Lists lst = new _Lists();
            _Parse.XML xml = new _Parse.XML();
            _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();

            public void TCP_PageLoop(RadProgressBar Bar, bool NewThread = true)
            {
                if (NewThread)
                {
                    Thread newThread = new Thread(() => TCP_PageLoop(Bar, false)); // Function To Execute
                    newThread.Name = "Populate Missing Actor Info";
                    newThread.IsBackground = true;
                    newThread.Start();
                }
                else
                {
                    int TotalPages = 679;

                    pro.SetupProgressBar(Bar, TotalPages);

                    int page = 1;
                    do
                    {
                        pro.UpdateProgress(Bar, page + "/" + TotalPages);
                        string URL = "https://theclassicporn.com/videos/" + page + "/";
                        Parse_TCPPage(URL, @"H:\SiteParse\TheClassic");
                        page++;
                    }
                    while (page < TotalPages);

                    ahk.MsgBox("DONE!");
                }
            }


            public List<TCP> Parse_TCPPage(string URL, string imageRoot = @"H:\SiteParse\TheClassic")
            {
                List<TCP> Films = new List<TCP>();

                string xpath = "/html/body/div[3]/div/div[2]/div[4]/ul";
                string html = ahk.Download_HTML(URL);
                List<string> resultLines = xml.Parse_HTML_XML(html, xpath);  // extract sections of text from html xml

                foreach (string line in resultLines)  // pull segments of page (20 total)
                {
                    TCP Film = new TCP();

                    List<string> ImageLinks = new List<string>();

                    //string FilmURL = ""; string CoverURL = ""; string Title = ""; List<string> ImageLinks = new List<string>();
                    //string VidID = "";


                    string seg = line.Replace("\">", "\">\n");
                    List<string> newLines = lst.Text_To_List(seg); bool TitleFound = false;
                    foreach (string newline in newLines)
                    {
                        //FILM URL
                        string videoURLCheck = "thumb-video-link\" href=\"/videos";
                        if (newline.Contains(videoURLCheck))
                        {
                            string videoURL = newline.Replace("<a class=\"thumb-video-link\" href=\"", "");
                            videoURL = videoURL.Replace("/\">", "");
                            Film.FilmURL = "https://theclassicporn.com" + videoURL; // + @"\";
                                                                                    //ahk.MsgBox("FilmURL: " + FilmURL);
                        }

                        // VIDEO ID
                        if (newline.Contains("covers.jpg"))
                        {
                            List<string> items = ahk.StringSplit_List(newline, "/");
                            foreach (string item in items)
                            {
                                if (!item.Contains("covers.jpg")) { Film.VideoID = item; }
                                if (item.Contains("covers.jpg")) { break; }
                            }
                            //ahk.MsgBox(vidID);
                        }

                        // VIDEO COVER URL
                        if (newline.Contains("covers.jpg"))
                        {
                            List<string> items = ahk.StringSplit_List(newline, "\"");
                            foreach (string item in items)
                            {
                                if (item.Contains("covers.jpg")) { Film.CoverURL = item; break; }
                            }
                            //ahk.MsgBox(CoverURL);
                        }

                        // VIDEO TITLE
                        if (newline.Contains("class=\"link-blue link-no-border\"")) { TitleFound = true; continue; }
                        if (TitleFound)
                        {
                            Film.FilmName = newline.Replace("</a>", "");
                            //ahk.MsgBox(Title);
                            TitleFound = false;
                        }


                        if (newline.Contains(".jpg") && !newline.Contains("covers.jpg"))
                        {
                            string ImageLine = newline.Replace("\" alt=\"\">", "");
                            ImageLine = ImageLine.Replace("<img class=\"screen-thumb\" src=\"", "");
                            //ahk.MsgBox(ImageLine);
                            ImageLinks.Add(ImageLine);
                        }


                    }


                    string links = "";
                    if (imageRoot != "")
                    {
                        string saveDir = imageRoot + "\\" + Film.VideoID;
                        ahk.FileCreateDir(saveDir);

                        // download film images to dirs sorted by VidID
                        foreach (string image in ImageLinks)
                        {
                            string fileName = ahk.StringSplit(image, "/", 0, true);
                            ahk.Download_File(image, saveDir + "\\" + fileName);

                            if (links == "") { links = image; }
                            else { links = links + "\n" + image; }
                        }

                        // download film cover
                        if (Film.CoverURL != null)
                        {
                            string filename = ahk.StringSplit(Film.CoverURL, "/", 0, true);
                            ahk.Download_File(Film.CoverURL, saveDir + "\\" + filename);
                        }

                    }
                    else
                    {
                        // add image links to string list
                        foreach (string image in ImageLinks)
                        {
                            if (links == "") { links = image; }
                            else { links = links + "\n" + image; }
                        }
                    }



                    Film.ImageLinks = links;
                    Film.ImgLinks = ImageLinks;
                    Films.Add(Film);

                    bool added = TCP_UpdateInsert(Film);
                }


                return Films;
            }



            #region === TCP FUNCTIONS ===

            #region ===== TCP Object =====

            public struct TCP
            {
                public string VideoID { get; set; }
                public string FilmName { get; set; }
                public string FilmURL { get; set; }
                public string CoverURL { get; set; }
                public string ImageLinks { get; set; }

                public List<string> ImgLinks { get; set; }
            }

            #endregion
            #region ===== TCP SQL Functions =====

            // Return TCP SQL Connection String
            public SqlConnection TCP_Conn()
            {
                // populate sql connection
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMADBIndex"].ConnectionString);
                return conn;
            }

            // Return TCP TableName (Full Path)
            public string TCP_TableName()
            {
                // populate to return full sql table name
                return "[ADBIndex].[dbo].[TCP]";
            }

            // Generate SQL Table
            public bool TCP_CreateSQLTable()
            {
                SqlConnection Conn = TCP_Conn();
                string CreateTableLine = "CREATE TABLE [TCP](";
                CreateTableLine = CreateTableLine + "[VideoID] [int] IDENTITY(1,1) NOT NULL,";
                CreateTableLine = CreateTableLine + "[FilmName] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[FilmURL] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[CoverURL] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[ImageLinks] [varchar](max) NOT NULL";
                CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                return false;
            }

            public bool TCP_InsertSQL(TCP obj)
            {
                SqlConnection Con = TCP_Conn();
                string SQLLine = "Insert Into " + TCP_TableName() + " (VideoID, FilmName, FilmURL, CoverURL, ImageLinks) VALUES (@VideoID, @FilmName, @FilmURL, @CoverURL, @ImageLinks)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.VideoID == null) { obj.VideoID = ""; }
                if (obj.FilmName == null) { obj.FilmName = ""; }
                if (obj.FilmURL == null) { obj.FilmURL = ""; }
                if (obj.CoverURL == null) { obj.CoverURL = ""; }
                if (obj.ImageLinks == null) { obj.ImageLinks = ""; }
                cmd2.Parameters.AddWithValue(@"VideoID", obj.VideoID.ToString());
                cmd2.Parameters.AddWithValue(@"FilmName", obj.FilmName.ToString());
                cmd2.Parameters.AddWithValue(@"FilmURL", obj.FilmURL.ToString());
                cmd2.Parameters.AddWithValue(@"CoverURL", obj.CoverURL.ToString());
                cmd2.Parameters.AddWithValue(@"ImageLinks", obj.ImageLinks.ToString());
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // retry on deadlock
                    {
                        TCP_InsertSQL(obj);
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

            public bool TCP_UpdateSQL(TCP obj)
            {
                SqlConnection Conn = TCP_Conn();
                string SQLLine = "Update " + TCP_TableName() + " SET VideoID = @VideoID, FilmName = @FilmName, FilmURL = @FilmURL, CoverURL = @CoverURL, ImageLinks = @ImageLinks WHERE VideoID = @VideoID";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.VideoID == null) { obj.VideoID = ""; }
                if (obj.FilmName == null) { obj.FilmName = ""; }
                if (obj.FilmURL == null) { obj.FilmURL = ""; }
                if (obj.CoverURL == null) { obj.CoverURL = ""; }
                if (obj.ImageLinks == null) { obj.ImageLinks = ""; }
                cmd2.Parameters.AddWithValue(@"VideoID", obj.VideoID.ToString());
                cmd2.Parameters.AddWithValue(@"FilmName", obj.FilmName.ToString());
                cmd2.Parameters.AddWithValue(@"FilmURL", obj.FilmURL.ToString());
                cmd2.Parameters.AddWithValue(@"CoverURL", obj.CoverURL.ToString());
                cmd2.Parameters.AddWithValue(@"ImageLinks", obj.ImageLinks.ToString());
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // retry on deadlock
                    {
                        TCP_UpdateSQL(obj);
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

            public bool TCP_UpdateInsert(TCP obj)
            {
                SqlConnection Conn = TCP_Conn();
                bool Updated = TCP_UpdateSQL(obj);  // try to update record first
                if (!Updated) { Updated = TCP_InsertSQL(obj); }  // if unable to update, insert new record
                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool TCP_UpdateIfPopulated(TCP obj, string ID = "")
            {
                SqlConnection Conn = TCP_Conn();
                string SQLcmd = "Update " + TCP_TableName() + " SET ";
                if (obj.VideoID != null) { SQLcmd = SQLcmd + " VideoID = @VideoID,"; }
                if (obj.FilmName != null) { SQLcmd = SQLcmd + " FilmName = @FilmName,"; }
                if (obj.FilmURL != null) { SQLcmd = SQLcmd + " FilmURL = @FilmURL,"; }
                if (obj.CoverURL != null) { SQLcmd = SQLcmd + " CoverURL = @CoverURL,"; }
                if (obj.ImageLinks != null) { SQLcmd = SQLcmd + " ImageLinks = @ImageLinks,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ID = @ID";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.VideoID != null) { cmd2.Parameters.AddWithValue(@"VideoID", obj.VideoID); }
                if (obj.FilmName != null) { cmd2.Parameters.AddWithValue(@"FilmName", obj.FilmName); }
                if (obj.FilmURL != null) { cmd2.Parameters.AddWithValue(@"FilmURL", obj.FilmURL); }
                if (obj.CoverURL != null) { cmd2.Parameters.AddWithValue(@"CoverURL", obj.CoverURL); }
                if (obj.ImageLinks != null) { cmd2.Parameters.AddWithValue(@"ImageLinks", obj.ImageLinks); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public TCP TCP_ReturnSQL(string ID = "")
            {
                SqlConnection Conn = TCP_Conn();
                string SelectLine = "Select [VideoID],[FilmName],[FilmURL],[CoverURL],[ImageLinks] From " + TCP_TableName() + " WHERE ID = '" + ID + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                TCP returnObject = new TCP();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.VideoID = ret["VideoID"].ToString();
                        returnObject.FilmName = ret["FilmName"].ToString();
                        returnObject.FilmURL = ret["FilmURL"].ToString();
                        returnObject.CoverURL = ret["CoverURL"].ToString();
                        returnObject.ImageLinks = ret["ImageLinks"].ToString();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public List<TCP> TCP_ReturnSQLList(string Command = "")
            {
                if (Command == "") { Command = "Select * From TCP_TableName()"; }
                SqlConnection Conn = TCP_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<TCP> ReturnList = new List<TCP>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        TCP returnObject = new TCP();
                        returnObject.VideoID = ret["VideoID"].ToString();
                        returnObject.FilmName = ret["FilmName"].ToString();
                        returnObject.FilmURL = ret["FilmURL"].ToString();
                        returnObject.CoverURL = ret["CoverURL"].ToString();
                        returnObject.ImageLinks = ret["ImageLinks"].ToString();
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

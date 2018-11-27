using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using AHKExpressions;
using Telerik.WinControls.UI;

namespace sharpAHK_Dev
{
    public partial class _Sites
    {
        public class MovieParadise
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
            _Database.dir2Db dir2 = new _Database.dir2Db();
            _Parse prs = new _Parse();
            _StatusBar sb = new _StatusBar();
            _TelerikLib tel = new _TelerikLib();
            _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();
            _Web web = new _Web();
            _Sites.Imdb imdb = new _Sites.Imdb();
            _Parse.XML xml = new _Parse.XML();

            SqlConnection GoDadConn = new SqlConnection(ConfigurationManager.ConnectionStrings["epGuideDb"].ConnectionString);
            SqlConnection Conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLserver"].ConnectionString);



            #endregion

            #region === Unzip MovieParadise Zips ==

            public void MovieParadise_UnZip(string ZipsDir, string UnZipRoot)
            {
                //string ZipsDir = @"D:\Media\MOVIES\Queue_B";
                //string UnZipRoot = @"J:\UNZipped\Queue_B";

                ahk.FileCreateDir(UnZipRoot);

                List<string> files = lst.FileList(ZipsDir, "*.*", false, false, true);

                int i = 1;
                foreach (string file in files)
                {
                    sb.StatusBar("Unzipping " + i + " / " + files.Count + " | " + ahk.FileName(file));

                    // loopup imdb id for this file
                    string fileName = ahk.FileNameNoExt(file);
                    string id = IMDbID_From_MPZipFile(fileName);

                    // write imdb link / create new dir 
                    string NewDirWithLink = imdb.IMDb_UrlDir(UnZipRoot, id);

                    // unzip rar contents to new dir with link
                    UnRAR_Movie(file, NewDirWithLink);

                    // move unzipped file to dir after processing
                    ahk.Sleep(2000);
                    ahk.FileCreateDir(ahk.FileDir(file) + "\\UnZips");
                    ahk.FileMove(file, ahk.FileDir(file) + "\\UnZips\\" + ahk.FileName(file));
                    i++;
                }


                sb.StatusBar("FINISHED Unzipping");

            }

            public void UnRAR_Movie(string filePath, string saveDir = "", bool OverWriteExisting = false)
            {
                if (!File.Exists(filePath)) { ahk.MsgBox(filePath + " Not Found"); return; }

                string unRAR = ahk.AppDir() + "\\RAR\\UnRAR.exe"; if (!File.Exists(unRAR)) { ahk.MsgBox(unRAR + " Not Found"); return; }

                if (!File.Exists(unRAR)) { ahk.MsgBox(unRAR + " Not Found"); return; }

                if (saveDir == "") { saveDir = ahk.FileDir(filePath); }

                // e = ignore path
                string cmd = "";

                if (OverWriteExisting) { cmd = unRAR + " e -y -o+ \"" + filePath + "\" \"" + saveDir + "\""; }
                if (!OverWriteExisting) { cmd = unRAR + " e -y -o- \"" + filePath + "\" \"" + saveDir + "\""; }

                //sb.StatusBar("Runnig " + cmd);
                //sb.StatusBar("Extracting " + ahk.FileName(filePath) + "...", 2);
                ahk.RunWait(cmd, "", "Hide");
                //sb.StatusBar("Finished Extracting " + ahk.FileName(filePath)); 
            }

            // based on saved zipfile name, return imdb id from MovieParadise db 
            public string IMDbID_From_MPZipFile(string MPZipFile)
            {
                string imdbID = "";
                string cmd = "select IMDbID FROM [lucidmedia].[lucidmethod].[MovieParadise] where RapidGatorLinks like '%" + MPZipFile + "%'";
                imdbID = sql.SQL_Return_Value(mpConn(), cmd);
                return imdbID;
            }

            public SqlConnection mpConn(string ConnName = "SQLserver")
            {
                //if (GoDad) { return new SqlConnection(ConfigurationManager.ConnectionStrings["epGuideDb"].ConnectionString); }
                return new SqlConnection(ConfigurationManager.ConnectionStrings[ConnName].ConnectionString);
            }


            #endregion

            #region === MovieParadise ===

            List<string> Between = new List<string>();

            // parse downloaded index pages for links to movie paradise pages
            bool Parsing_MovieParadiseHTML = false;
            string PageFinishedParsing = "";


            int mpUpdated = 0;
            int mpInserted = 0;

            public void Parse_MovieParadise_HTML(string html = "", bool DownloadImages = false, RadProgressBar Bar = null, Telerik.WinControls.UI.RadLabel LBLUpdated = null, Telerik.WinControls.UI.RadLabel LBLInserted = null)
            {
                //if (PageFinishedParsing == web.CurrentURL())
                //{
                //    return;
                //}

                _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();

                Parsing_MovieParadiseHTML = true;

                if (DownloadImages) { ahk.FileCreateDir(ahk.AppDir() + "\\IMDb_Img"); }


                int total = 0;
                int o = 0;
                o++;
                string HTML = html;

                string xmlPath = "//*[@id=\"contenedor\"]/div[3]/div[1]/div[1]";
                List<string> Between = xml.Parse_HTML_XML(html, xmlPath);  // extract sections of text from html xml

                if (Bar != null) { pro.SetupProgressBar(Bar, Between.Count); }

                int p = 1;
                foreach(string seg in Between)
                {
                    if (Bar != null) { pro.UpdateProgress(Bar, p + "/" + Between.Count); } p++;

                    movieParadise obj = new movieParadise();

                    string newSeg = seg.Replace(">", ">\n");

                    string filmName = xml.XML_Text(seg, "<h3>");

                    
                    filmName = filmName.Replace("<a href=\"", "");
                    filmName = filmName.Replace("</a>", "");
                    filmName = filmName.Replace("\">", "|");

                    string url = ahk.StringSplit(filmName, "|", 0).Trim();
                    string title = ahk.StringSplit(filmName, "|", 1).Trim();

                    //ahk.MsgBox(title + "\n\n" + url);

                    // MovieParadise Return is Either MOVIE or TVSHOW
                    bool IsMovie = false; if (url.ToLower().Contains("/movies/")) { IsMovie = true; }
                    bool IsShow = false; if (url.ToLower().Contains("/tvshows/")) { IsShow = true; }

                    string MovieHTML = ahk.Download_HTML(url);

                    _Sites.Imdb im = new Imdb();
                    _Sites.RapidGator rg = new RapidGator();

                    List<string> links = im.Regex_IMDbLinks(MovieHTML);

                    string IMDbLink = "";
                    string imdbID = "";

                    if (links.Count > 0)
                    {
                        IMDbLink = im.Regex_IMDbLinks(MovieHTML)[0];
                        imdbID = im.IMDb_ID_FromURL(IMDbLink);
                    }

                    List<string> rglinks = rg.Regex_RGLinks(MovieHTML);

                    string Rglinks = "";
                    if (rglinks.Count > 0)
                    {
                        foreach (string link in rglinks)
                        {
                            Rglinks = Rglinks + "\n" + link;
                        }
                    }
                    
                    if (obj.ImageLink == "" || obj.ImageLink == null)
                    {
                        if (seg.ToUpper().Contains(".JPG"))
                        {
                            List<string> imdbLinkChars = ahk.StringSplit_List(seg, "\"");
                            int i = 0;
                            foreach (string imChars in imdbLinkChars)
                            {
                                i++;

                                if (imChars.ToUpper().Contains(".JPG"))
                                {
                                    obj.ImageLink = imChars;

                                    //ahk.MsgBox("MovieImage: " + movieImage);
                                    //sb.StatusBar("MovieImage: " + obj.ImageLink);
                                }
                            }
                        }
                    }


                    if (DownloadImages)
                    {
                        string localLink = ahk.AppDir() + "\\IMDb_Img\\" + obj.IMDbID + ".jpg";

                        if (!File.Exists(localLink))
                        {
                            // download movie image - save with imdb id as name
                            using (WebClient client = new WebClient())
                            {
                                // download image if image link was parced
                                if (obj.ImageLink != null) { client.DownloadFile(new Uri(obj.ImageLink), localLink); }   // parses imdb url, extras title id); 
                            }
                        }
                    }


                    obj.MovieTitle = title;
                    obj.MoviePageLink = url;
                    obj.IMDbID = imdbID;
                    obj.RapidGatorLinks = Rglinks;


                    // check to see if movie already found in collection
                    obj.FoundInCollection = InIMDbLib(obj.IMDbID);


                    // check to see if movie already in MovieParadise DB - insert if missing
                    bool inMovieParadies = InMovieParadiseDb(obj.IMDbID);
                    if (!inMovieParadies)
                    {
                        bool inserted = movieParadise_UpdateInsert(obj);
                        if (inserted)
                        {
                            mpInserted++;
                            tel.UpdateLabel(LBLInserted, "Inserted: " + mpInserted.ToString());
                        }
                    }
                    if (inMovieParadies)
                    {
                        mpUpdated++;
                        tel.UpdateLabel(LBLUpdated, "Updated: " + mpUpdated.ToString());
                    }


                    //ahk.MsgBox(title + "\n\n" + url + "\n\nIMDbID: " + imdbID + "\nimage: " + obj.ImageLink + "\n\n" + Rglinks);

                }



                ////Between = CaptureSections("", "<article id=", "</div></div></div></article>", HTML);

                //int p = 0;

                //MovieParadise_AlreadyInDbCount = 0; // counter for # of entries already located in our db
                //foreach (string section in Between)
                //{
                //    total++;
                //    p++;
                //    //sb.StatusBar("Parsing " + p + "/" + Between.Count.ToString() + " | Updated " + MovieParadise_AlreadyInDbCount + " | Page " + MovieParadisePg + "/" + totalMoviePages, 2);

                //    movieParadise obj = new movieParadise();

                //    List<string> sectionLines = lst.Text_To_List(section, true, true, false);

                //    foreach (string line in sectionLines)
                //    {
                //        if (obj.MoviePageLink == "" || obj.MoviePageLink == null)
                //        {
                //            if (line.Contains("movieparadise.org"))
                //            {
                //                obj.MoviePageLink = line;
                //                //ahk.MsgBox("MovieParadise: " + line);

                //                List<string> imdbLinkChars = ahk.StringSplit_List(line, "\"");
                //                int i = 0;
                //                foreach (string imChars in imdbLinkChars)
                //                {
                //                    i++;
                //                    if (i == 2)
                //                    {
                //                        obj.MoviePageLink = imChars;

                //                        //ahk.MsgBox("MovieParadise: " + moviePageLink);
                //                        //sb.StatusBar("MovieParadise: " + obj.MoviePageLink);
                //                    }
                //                }

                //            }
                //        }

                //        if (obj.IMDbID == "" || obj.IMDbID == null)
                //        {
                //            if (line.Contains("imdb.com"))
                //            {
                //                List<string> imdbLinkChars = ahk.StringSplit_List(line, "\"");
                //                int i = 0;
                //                foreach (string imChars in imdbLinkChars)
                //                {
                //                    i++;
                //                    if (i == 2)
                //                    {
                //                        obj.IMDbID = imChars;
                //                        //ahk.MsgBox("IMDB: " + imdbID);
                //                        //sb.StatusBar("IMDB: " + obj.IMDbID);
                //                    }
                //                }
                //            }
                //        }

                //        if (obj.MovieTitle == "" || obj.MovieTitle == null)
                //        {
                //            if (line.Contains("<h1>"))
                //            {
                //                List<string> imdbLinkChars = ahk.StringSplit_List(line, "<");
                //                int i = 0;
                //                foreach (string imChars in imdbLinkChars)
                //                {
                //                    i++;
                //                    if (i == 2)
                //                    {
                //                        obj.MovieTitle = imChars;
                //                        obj.MovieTitle = ahk.StringReplace(obj.MovieTitle, "h1>");
                //                        obj.MovieTitle = ahk.StringReplace(obj.MovieTitle, "&nbsp");

                //                        if (obj.MovieTitle.Contains(";"))
                //                        {
                //                            obj.MovieYear = ahk.StringSplit(obj.MovieTitle, ";", 1);
                //                            obj.MovieTitle = ahk.StringSplit(obj.MovieTitle, ";", 0);
                //                        }

                //                        //ahk.MsgBox("MovieTitle: " + movieTitle);
                //                        //ahk.MsgBox("MovieYear: " + movieYear);
                //                        //sb.StatusBar("MovieTitle: " + obj.MovieTitle + " | " + "MovieYear: " + obj.MovieYear);
                //                    }
                //                }
                //            }
                //        }

                //        if (obj.ImageLink == "" || obj.ImageLink == null)
                //        {
                //            if (line.ToUpper().Contains(".JPG"))
                //            {
                //                List<string> imdbLinkChars = ahk.StringSplit_List(line, "\"");
                //                int i = 0;
                //                foreach (string imChars in imdbLinkChars)
                //                {
                //                    i++;

                //                    if (imChars.ToUpper().Contains(".JPG"))
                //                    {
                //                        obj.ImageLink = imChars;

                //                        //ahk.MsgBox("MovieImage: " + movieImage);
                //                        //sb.StatusBar("MovieImage: " + obj.ImageLink);
                //                    }
                //                }
                //            }
                //        }


                //    }  // end of section loop

                //    obj.IMDbID = ParseID(obj.IMDbID);

                //    if (DownloadImages)
                //    {
                //        // download movie image - save with imdb id as name
                //        using (WebClient client = new WebClient())
                //        {
                //            //sb.StatusBar("Downloading Movie Image");

                //            // download image if image link was parced
                //            if (obj.ImageLink != null) { client.DownloadFile(new Uri(obj.ImageLink), ahk.AppDir() + "\\IMDb_Img\\" + obj.IMDbID + ".jpg"); }   // parses imdb url, extras title id); 
                //        }
                //    }


                //    // check to see if movie already found in collection
                //    obj.FoundInCollection = InIMDbLib(obj.IMDbID);


                //    // check to see if movie already in MovieParadise DB - insert if missing
                //    bool inMovieParadies = InMovieParadiseDb(obj.IMDbID);
                //    if (!inMovieParadies)
                //    {
                //        bool inserted = movieParadise_UpdateInsert(obj);
                //        if (inserted)
                //        {
                //            mpInserted++;
                //            tel.UpdateLabel(LBLInserted, "Inserted: " + mpInserted.ToString());
                //        }
                //    }
                //    if (inMovieParadies)
                //    {
                //        mpUpdated++;
                //        tel.UpdateLabel(LBLUpdated, "Updated: " + mpUpdated.ToString());
                //    }

                //    //if (MovieParadise_AlreadyInDbCount == 25) 
                //    //{
                //    //    Stop_MovieParadiseSync();
                //    //}


                //}  // end of sections loop



                //sb.StatusBar("Finished Parsing");

                //if (LoadInBrowser) { PageFinishedParsing = web.CurrentURL(); }

                Parsing_MovieParadiseHTML = false;
            }


            // loop through text, return list of strings containing text between start and end strings
            public List<string> CaptureSections(string filePath, string StartString, string EndString, string TextOnlyOverRide = "", bool IfLineMatches = true)
            {
                List<string> Lines = new List<string>();

                if (TextOnlyOverRide == "")
                {
                    Lines = lst.TextFile_To_List(filePath, true, false, false);
                }
                if (TextOnlyOverRide != "")
                {
                    Lines = lst.Text_To_List(TextOnlyOverRide, true, true, false);
                }

                List<string> inBetween = new List<string>();
                string between = "";

                // either make the start / end items match exactly or be found on the line
                bool IfLineContains = false;
                //bool IfLineMatches = true;
                if (!IfLineMatches) { IfLineContains = true; }


                bool cap = false;
                foreach (string line in Lines)
                {
                    if (IfLineContains) { if (line.Contains(StartString)) { cap = true; } }
                    if (IfLineMatches)
                    {
                        if (line.Trim() == StartString.Trim())
                        {
                            cap = true;
                        }
                    }

                    if (cap)
                    {

                        if (between != "") { between = between + Environment.NewLine + line; }
                        if (between == "") { between = line; continue; }
                        //continue; 
                    }

                    if (IfLineContains) { if (line.Contains(EndString)) { cap = false; break; } }
                    if (IfLineMatches)
                    {
                        if (line.Trim() == EndString.Trim())
                        {
                            if (between != "")
                            {
                                inBetween.Add(between);
                                cap = false;
                                between = "";
                                //break;
                            }

                        }
                    }

                }

                // add last found item to list (?)
                inBetween.Add(between);


                return inBetween;
            }


            /// <summary>
            /// Check To See if IMDb Title Found In My Collection
            /// </summary>
            /// <param name="IMDbID">IMDb Title ID or IMDb.com URL</param>
            /// <returns></returns>
            public bool InIMDbLib(string IMDbID = "tt1211837")
            {
                _Sites.Imdb imd = new _Sites.Imdb();

                return imd.InLib(IMDbID);

                //SqlConnection Conn = movieParadise_Conn();

                //List<string> recordsFound = lst.SQL_To_List(Conn, "select * FROM [LucidMedia].[dbo].[IMDb_Details] where IMDbID = '" + IMDbID + "'");

                //if (recordsFound.Count > 0)
                //{
                //    //StatusBar("IMDb Title" + IMDbID + " FOUND in Collection");
                //    return true;
                //}

                ////StatusBar("IMDb Title" + IMDbID + " [NOT] FOUND in Collection");
                //return false;

            }

            /// <summary>
            /// Parses imdb url, extras IMDb Title ID
            /// </summary>
            /// <param name="URL"></param>
            /// <returns></returns>
            public string ParseID(string URL)
            {
                if (URL == "" || URL == null) { return "0"; }

                Imdb imd = new Imdb();
                return imd.ParseID(URL);
            }


            /// <summary>
            /// Returns Total Number of Movie Pages Currently In Index (From MovieParadise Webpage)
            /// </summary>
            /// <returns></returns>
            public int MovieParadise_TotalPages(RadTextBox disp = null, bool NewThread = true)
            {
                if (NewThread)
                {
                    Thread newThread = new Thread(() => MovieParadise_TotalPages(disp, false)); // Function To Execute
                    newThread.Name = "Return MovieParadies Total Page Count";
                    newThread.IsBackground = true;
                    newThread.Start();
                }
                else
                {
                    string url = "https://movieparadise.org/trending/page/100/";
                    string html = ahk.Download_HTML(url);

                    List<string> matches = new List<string>();

                    string cmd = @"<span>Page.*of(.*)</span>";

                    int pageCount = 0;
                    Regex ItemRegex = new Regex(cmd, RegexOptions.IgnoreCase);
                    foreach (Match ItemMatch in ItemRegex.Matches(html))
                    {
                        //Console.WriteLine(ItemMatch);
                        string val = ItemMatch.Value;
                        //ahk.MsgBox(val);

                        string pgNums = prs.XML_Text(val, "<span>");

                        pgNums = pgNums.Replace(" of ", "|");

                        //ahk.MsgBox(prs.XML_Text(val, "<span>"));

                        string pg = ahk.StringSplit(pgNums, "|", 1).Trim();

                        //ahk.MsgBox(pg);

                        pageCount = pg.ToInt();

                        return pageCount;
                        //if (val.ToLower().Contains("imdb.com")) { matches.Add(val); }
                    }

                    // update control if provided
                    if (disp != null) { tel.Update(disp, pageCount.ToString()); }
                }


                return -1;
            }




            #endregion

            //url = "http://movieparadise.org/page/" + current + "/";

            public void Download_MovieParadise_Index(int TotalPages = 735, int StartNum = 1, RadProgressBar Bar = null, RadProgressBar Bar2 = null, Telerik.WinControls.UI.RadTextBox CurrentPgNum = null, Telerik.WinControls.UI.RadLabel LBLUpdated = null, Telerik.WinControls.UI.RadLabel LBLInserted = null, bool NewThread = true)
            {
                if (NewThread)
                {
                    Thread newThread = new Thread(() => Download_MovieParadise_Index(TotalPages, StartNum, Bar, Bar2, CurrentPgNum, LBLUpdated, LBLInserted, false)); // Function To Execute
                    newThread.Name = "Populate Missing Actor Info";
                    newThread.IsBackground = true;
                    newThread.Start();
                }
                else
                {
                    _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();

                    int current = 1;
                    bool started = false;
                    do
                    {
                        if (Bar != null) { pro.SetupProgressBar(Bar, TotalPages); }

                        if (current == StartNum) { started = true; }

                        if (Bar != null) { pro.UpdateProgress(Bar, current + "/" + TotalPages); }

                        if (!started)  // skip past previous pages if startnum provided
                        {
                            current++;
                            tel.UpdateText(CurrentPgNum, current.ToString());
                            continue;
                        }

                        string url = "https://movieparadise.org/trending/page/" + current + "/";
                        if (current == 1) { url = "https://movieparadise.org/trending/"; }

                        string html = ahk.Download_HTML(url);

                        //ahk.MsgBox(html);

                        Parse_MovieParadise_HTML(html, true, Bar2, LBLUpdated, LBLInserted);

                        current++;
                        tel.UpdateText(CurrentPgNum, current.ToString());

                    } while (current < TotalPages + 1);

                    ahk.MsgBox("Finished Updating MovieParadise Index");
                }
            }

            #region === movieParadise Object ===

            public struct movieParadise
            {
                public string ID { get; set; }
                public string MoviePageLink { get; set; }
                public string IMDbID { get; set; }
                public string MovieTitle { get; set; }
                public string MovieYear { get; set; }
                public string ImageLink { get; set; }
                public string RapidGatorLinks { get; set; }
                public bool FoundInCollection { get; set; }
                public bool InCollection { get; set; }
                public string DateModified { get; set; }
            }
            public movieParadise Return_movieParadise(string ID = "", string MoviePageLink = "", string IMDbID = "", string MovieTitle = "", string MovieYear = "", string ImageLink = "", string RapidGatorLinks = "")
            {
                movieParadise obj = new movieParadise();
                obj.ID = ID;
                obj.MoviePageLink = MoviePageLink;
                obj.IMDbID = IMDbID;
                obj.MovieTitle = MovieTitle;
                obj.MovieYear = MovieYear;
                obj.ImageLink = ImageLink;
                obj.RapidGatorLinks = RapidGatorLinks;

                return obj;
            }

            //  Fix illegal characters before Sql/Sqlite Db Inserts
            public movieParadise movieParadise_FixChars(movieParadise ToFix)
            {
                movieParadise Fixed = new movieParadise();

                Fixed.ID = ToFix.ID.Replace("'", "''");
                Fixed.MoviePageLink = ToFix.MoviePageLink.Replace("'", "''");
                Fixed.IMDbID = ToFix.IMDbID.Replace("'", "''");
                Fixed.MovieTitle = ToFix.MovieTitle.Replace("'", "''");
                Fixed.MovieYear = ToFix.MovieYear.Replace("'", "''");
                Fixed.ImageLink = ToFix.ImageLink.Replace("'", "''");
                Fixed.RapidGatorLinks = ToFix.RapidGatorLinks.Replace("'", "''");

                return Fixed;
            }

            // Compare two objects to see if they have identical values
            public bool movieParadise_Changed(movieParadise OldVal, movieParadise NewVal)
            {
                movieParadise diff = new movieParadise();
                List<string> diffList = new List<string>();
                bool different = false;
                if (OldVal.ID == null) { OldVal.ID = ""; }
                if (NewVal.ID == null) { NewVal.ID = ""; }
                if (OldVal.ID != NewVal.ID) { different = true; }
                if (OldVal.MoviePageLink == null) { OldVal.MoviePageLink = ""; }
                if (NewVal.MoviePageLink == null) { NewVal.MoviePageLink = ""; }
                if (OldVal.MoviePageLink != NewVal.MoviePageLink) { different = true; }
                if (OldVal.IMDbID == null) { OldVal.IMDbID = ""; }
                if (NewVal.IMDbID == null) { NewVal.IMDbID = ""; }
                if (OldVal.IMDbID != NewVal.IMDbID) { different = true; }
                if (OldVal.MovieTitle == null) { OldVal.MovieTitle = ""; }
                if (NewVal.MovieTitle == null) { NewVal.MovieTitle = ""; }
                if (OldVal.MovieTitle != NewVal.MovieTitle) { different = true; }
                if (OldVal.MovieYear == null) { OldVal.MovieYear = ""; }
                if (NewVal.MovieYear == null) { NewVal.MovieYear = ""; }
                if (OldVal.MovieYear != NewVal.MovieYear) { different = true; }
                if (OldVal.ImageLink == null) { OldVal.ImageLink = ""; }
                if (NewVal.ImageLink == null) { NewVal.ImageLink = ""; }
                if (OldVal.ImageLink != NewVal.ImageLink) { different = true; }
                if (OldVal.RapidGatorLinks == null) { OldVal.RapidGatorLinks = ""; }
                if (NewVal.RapidGatorLinks == null) { NewVal.RapidGatorLinks = ""; }
                if (OldVal.RapidGatorLinks != NewVal.RapidGatorLinks) { different = true; }
                return different;
            }

            // Returns object containing the new values different from the old values in object comparison
            public movieParadise movieParadise_Diff(movieParadise OldVal, movieParadise NewVal)
            {
                movieParadise diff = new movieParadise();
                if (OldVal.ID != NewVal.ID) { diff.ID = NewVal.ID; }
                if (OldVal.MoviePageLink != NewVal.MoviePageLink) { diff.MoviePageLink = NewVal.MoviePageLink; }
                if (OldVal.IMDbID != NewVal.IMDbID) { diff.IMDbID = NewVal.IMDbID; }
                if (OldVal.MovieTitle != NewVal.MovieTitle) { diff.MovieTitle = NewVal.MovieTitle; }
                if (OldVal.MovieYear != NewVal.MovieYear) { diff.MovieYear = NewVal.MovieYear; }
                if (OldVal.ImageLink != NewVal.ImageLink) { diff.ImageLink = NewVal.ImageLink; }
                if (OldVal.RapidGatorLinks != NewVal.RapidGatorLinks) { diff.RapidGatorLinks = NewVal.RapidGatorLinks; }
                return diff;
            }

            // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
            public List<string> movieParadise_DiffList(movieParadise OldVal, movieParadise NewVal)
            {
                List<string> diffList = new List<string>();
                if (OldVal.ID != NewVal.ID) { diffList.Add("Changed ID Value From " + OldVal.ID + " To " + NewVal.ID); }
                if (OldVal.MoviePageLink != NewVal.MoviePageLink) { diffList.Add("Changed MoviePageLink Value From " + OldVal.MoviePageLink + " To " + NewVal.MoviePageLink); }
                if (OldVal.IMDbID != NewVal.IMDbID) { diffList.Add("Changed IMDbID Value From " + OldVal.IMDbID + " To " + NewVal.IMDbID); }
                if (OldVal.MovieTitle != NewVal.MovieTitle) { diffList.Add("Changed MovieTitle Value From " + OldVal.MovieTitle + " To " + NewVal.MovieTitle); }
                if (OldVal.MovieYear != NewVal.MovieYear) { diffList.Add("Changed MovieYear Value From " + OldVal.MovieYear + " To " + NewVal.MovieYear); }
                if (OldVal.ImageLink != NewVal.ImageLink) { diffList.Add("Changed ImageLink Value From " + OldVal.ImageLink + " To " + NewVal.ImageLink); }
                if (OldVal.RapidGatorLinks != NewVal.RapidGatorLinks) { diffList.Add("Changed RapidGatorLinks Value From " + OldVal.RapidGatorLinks + " To " + NewVal.RapidGatorLinks); }
                return diffList;
            }


            #endregion
            #region === movieParadise SQL Functions ===

            // Return movieParadise SQL Connection String
            public SqlConnection movieParadise_Conn()
            {
                // populate sql connection
                //SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLserver"].ConnectionString);
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMLucidMedia"].ConnectionString);
                return conn;
            }

            // Return movieParadise TableName (Full Path)
            public string movieParadise_TableName()
            {
                // populate to return full sql table name
                return "[LucidMedia].[dbo].[MovieParadise]";
            }

            // Generate SQL Table
            public bool movieParadise_CreateSQLTable()
            {
                SqlConnection Conn = movieParadise_Conn();
                string CreateTableLine = "CREATE TABLE " + movieParadise_TableName() + " (";
                CreateTableLine = CreateTableLine + "[ID] [int] IDENTITY(1,1) NOT NULL,";
                CreateTableLine = CreateTableLine + "[MoviePageLink] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[IMDbID] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[MovieTitle] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[MovieYear] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[ImageLink] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[RapidGatorLinks] [varchar](max) NOT NULL";
                CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                return true;
            }

            public bool movieParadise_InsertSQL(movieParadise obj)
            {
                SqlConnection Con = movieParadise_Conn();
                string SQLLine = "Insert Into " + movieParadise_TableName() + " WITH (TABLOCK) (MoviePageLink, IMDbID, MovieTitle, MovieYear, ImageLink, RapidGatorLinks, InCollection, DateModified) VALUES (@MoviePageLink, @IMDbID, @MovieTitle, @MovieYear, @ImageLink, @RapidGatorLinks, @InCollection, '" + DateTime.Now + "')";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.MoviePageLink == null) { obj.MoviePageLink = ""; }
                if (obj.IMDbID == null) { obj.IMDbID = ""; }
                if (obj.MovieTitle == null) { obj.MovieTitle = ""; }
                if (obj.MovieYear == null) { obj.MovieYear = ""; }
                if (obj.ImageLink == null) { obj.ImageLink = ""; }
                if (obj.RapidGatorLinks == null) { obj.RapidGatorLinks = ""; }

                cmd2.Parameters.AddWithValue(@"MoviePageLink", obj.MoviePageLink.ToString());
                cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID.ToString());
                cmd2.Parameters.AddWithValue(@"MovieTitle", obj.MovieTitle.ToString());
                cmd2.Parameters.AddWithValue(@"MovieYear", obj.MovieYear.ToString());
                cmd2.Parameters.AddWithValue(@"ImageLink", obj.ImageLink.ToString());
                cmd2.Parameters.AddWithValue(@"RapidGatorLinks", obj.RapidGatorLinks.ToString());
                cmd2.Parameters.AddWithValue(@"InCollection", obj.FoundInCollection.ToString());

                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        movieParadise_InsertSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        movieParadise_InsertSQL(obj);
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

            public bool movieParadise_UpdateSQL(movieParadise obj)
            {
                SqlConnection Conn = movieParadise_Conn();
                string SQLLine = "";
                int recordsAffected = 0;

                string RapidLinks = "";
                try
                {
                    RapidLinks = obj.RapidGatorLinks.ToString();
                }
                catch { RapidLinks = ""; }

                if (RapidLinks == "")
                {
                    //SQLLine = "Update " + movieParadise_TableName() + " SET MoviePageLink = @MoviePageLink, MovieTitle = @MovieTitle, MovieYear = @MovieYear, ImageLink = @ImageLink, InCollection = @InCollection, DateModified = '" + DateTime.Now + "'  WHERE IMDbID = @IMDbID";
                    SQLLine = "Update " + movieParadise_TableName() + " WITH (TABLOCK) SET IMDbID = @IMDbID, MovieTitle = @MovieTitle, MovieYear = @MovieYear, ImageLink = @ImageLink, InCollection = @InCollection, DateModified = '" + DateTime.Now + "'  WHERE MoviePageLink = @MoviePageLink";


                    SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                    cmd2 = new SqlCommand(SQLLine, Conn);
                    if (obj.MoviePageLink == null) { obj.MoviePageLink = ""; }
                    if (obj.IMDbID == null) { obj.IMDbID = ""; }
                    if (obj.MovieTitle == null) { obj.MovieTitle = ""; }
                    if (obj.MovieYear == null) { obj.MovieYear = ""; }
                    if (obj.ImageLink == null) { obj.ImageLink = ""; }

                    cmd2.Parameters.AddWithValue(@"MoviePageLink", obj.MoviePageLink.ToString());
                    cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID.ToString());
                    cmd2.Parameters.AddWithValue(@"MovieTitle", obj.MovieTitle.ToString());
                    cmd2.Parameters.AddWithValue(@"MovieYear", obj.MovieYear.ToString());
                    cmd2.Parameters.AddWithValue(@"ImageLink", obj.ImageLink.ToString());
                    cmd2.Parameters.AddWithValue(@"InCollection", obj.FoundInCollection.ToString());

                    if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                    try { recordsAffected = cmd2.ExecuteNonQuery(); }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 1205) // Retry on DeadLock
                        {
                            ahk.Sleep(1000);
                            movieParadise_UpdateSQL(obj);
                        }
                        else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                        {
                            ahk.Sleep(1000);
                            movieParadise_UpdateSQL(obj);
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


                if (RapidLinks != "")
                {
                    SQLLine = "Update " + movieParadise_TableName() + " WITH (TABLOCK) SET IMDbID = @IMDbID, MovieTitle = @MovieTitle, MovieYear = @MovieYear, ImageLink = @ImageLink, RapidGatorLinks = @RapidGatorLinks, InCollection = @InCollection, DateModified = '" + DateTime.Now + "' WHERE MoviePageLink = @MoviePageLink";

                    SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                    cmd2 = new SqlCommand(SQLLine, Conn);
                    if (obj.MoviePageLink == null) { obj.MoviePageLink = ""; }
                    if (obj.IMDbID == null) { obj.IMDbID = ""; }
                    if (obj.MovieTitle == null) { obj.MovieTitle = ""; }
                    if (obj.MovieYear == null) { obj.MovieYear = ""; }
                    if (obj.ImageLink == null) { obj.ImageLink = ""; }
                    if (obj.RapidGatorLinks == null) { obj.RapidGatorLinks = ""; }

                    cmd2.Parameters.AddWithValue(@"MoviePageLink", obj.MoviePageLink.ToString());
                    cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID.ToString());
                    cmd2.Parameters.AddWithValue(@"MovieTitle", obj.MovieTitle.ToString());
                    cmd2.Parameters.AddWithValue(@"MovieYear", obj.MovieYear.ToString());
                    cmd2.Parameters.AddWithValue(@"ImageLink", obj.ImageLink.ToString());
                    cmd2.Parameters.AddWithValue(@"RapidGatorLinks", obj.RapidGatorLinks.ToString());
                    cmd2.Parameters.AddWithValue(@"InCollection", obj.FoundInCollection.ToString());

                    if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                    try { recordsAffected = cmd2.ExecuteNonQuery(); }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 1205) // Retry on DeadLock
                        {
                            ahk.Sleep(1000);
                            movieParadise_UpdateSQL(obj);
                        }
                        else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                        {
                            ahk.Sleep(1000);
                            movieParadise_UpdateSQL(obj);
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



                return false;
            }

            public bool movieParadise_UpdateRGLinks(string MoviePageLink, string rglinks)
            {
                //// SQLite Db

                //string DbFile = ahk.AppDir() + "\\Db\\MovieParadise.sqlite";
                //string SQLiteLine = "Update " + movieParadise_TableName() + " SET RapidGatorLinks = '" + rglinks + "' WHERE MoviePageLink = '" + MoviePageLink + "'";
                //bool Updated = sqlite.Execute(DbFile, SQLiteLine);
                //return Updated;

                // SQL Db 

                SqlConnection Conn = movieParadise_Conn();
                string SQLLine = "Update " + movieParadise_TableName() + " SET RapidGatorLinks = '" + rglinks + "' WHERE MoviePageLink = '" + MoviePageLink + "'";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);


                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        movieParadise_UpdateRGLinks(MoviePageLink, rglinks);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        movieParadise_UpdateRGLinks(MoviePageLink, rglinks);
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

            int MovieParadise_AlreadyInDbCount = 0;

            public bool movieParadise_UpdateInsert(movieParadise obj, bool SQLiteUpdate = false)
            {
                if (SQLiteUpdate)
                {
                    // SQLite Db
                    bool updated = movieParadise_SQLite_UpdateInsert(obj);
                    return updated;
                }

                // SQL Db

                SqlConnection Conn = movieParadise_Conn();
                bool Updated = movieParadise_UpdateSQL(obj);  // try to update record first

                if (Updated) { MovieParadise_AlreadyInDbCount++; }

                if (!Updated) { Updated = movieParadise_InsertSQL(obj); }  // if unable to update, insert new record
                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool movieParadise_UpdateIfPopulated(movieParadise obj, string ID = "")
            {
                SqlConnection Conn = movieParadise_Conn();
                string SQLcmd = "Update " + movieParadise_TableName() + " SET ";
                if (obj.ID != null) { SQLcmd = SQLcmd + " ID = @ID,"; }
                if (obj.MoviePageLink != null) { SQLcmd = SQLcmd + " MoviePageLink = @MoviePageLink,"; }
                if (obj.IMDbID != null) { SQLcmd = SQLcmd + " IMDbID = @IMDbID,"; }
                if (obj.MovieTitle != null) { SQLcmd = SQLcmd + " MovieTitle = @MovieTitle,"; }
                if (obj.MovieYear != null) { SQLcmd = SQLcmd + " MovieYear = @MovieYear,"; }
                if (obj.ImageLink != null) { SQLcmd = SQLcmd + " ImageLink = @ImageLink,"; }
                if (obj.RapidGatorLinks != null) { SQLcmd = SQLcmd + " RapidGatorLinks = @RapidGatorLinks,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ID = @ID";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.ID != null) { cmd2.Parameters.AddWithValue(@"ID", obj.ID); }
                if (obj.MoviePageLink != null) { cmd2.Parameters.AddWithValue(@"MoviePageLink", obj.MoviePageLink); }
                if (obj.IMDbID != null) { cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID); }
                if (obj.MovieTitle != null) { cmd2.Parameters.AddWithValue(@"MovieTitle", obj.MovieTitle); }
                if (obj.MovieYear != null) { cmd2.Parameters.AddWithValue(@"MovieYear", obj.MovieYear); }
                if (obj.ImageLink != null) { cmd2.Parameters.AddWithValue(@"ImageLink", obj.ImageLink); }
                if (obj.RapidGatorLinks != null) { cmd2.Parameters.AddWithValue(@"RapidGatorLinks", obj.RapidGatorLinks); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public movieParadise movieParadise_ReturnSQL(string ID = "")
            {
                SqlConnection Conn = movieParadise_Conn();
                string SelectLine = "Select [ID],[MoviePageLink],[IMDbID],[MovieTitle],[MovieYear],[ImageLink],[RapidGatorLinks] From " + movieParadise_TableName() + " WHERE ID = '" + ID + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                movieParadise returnObject = new movieParadise();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.MoviePageLink = ret["MoviePageLink"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.MovieTitle = ret["MovieTitle"].ToString();
                        returnObject.MovieYear = ret["MovieYear"].ToString();
                        returnObject.ImageLink = ret["ImageLink"].ToString();
                        returnObject.RapidGatorLinks = ret["RapidGatorLinks"].ToString();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public List<movieParadise> movieParadise_ReturnSQLList(string Command = "")
            {
                if (Command == "") { Command = "select * from " + movieParadise_TableName() + " where InCollection = '0'"; }

                SqlConnection Conn = movieParadise_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<movieParadise> ReturnList = new List<movieParadise>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        movieParadise returnObject = new movieParadise();
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.MoviePageLink = ret["MoviePageLink"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.MovieTitle = ret["MovieTitle"].ToString();
                        returnObject.MovieYear = ret["MovieYear"].ToString();
                        returnObject.ImageLink = ret["ImageLink"].ToString();
                        returnObject.RapidGatorLinks = ret["RapidGatorLinks"].ToString();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }


            /// <summary>
            /// Check MovieParadies Db to see if IMDb Id Already Exists
            /// </summary>
            /// <param name="IMDbID">IMDb ID to Check For</param>
            /// <returns>Returns True if IMDb ID Found in MovieParadise Db</returns>
            public bool InMovieParadiseDb(string IMDbID)
            {
                string returnVal = sql.SQL_Return_Value(movieParadise_Conn(), "select Count(IMDbID) FROM " + movieParadise_TableName() + " where IMDbId = '" + IMDbID + "'");
                int foundCount = ahk.ToInt(returnVal);
                if (foundCount > 0) { return true; }
                return false;
            }

            #endregion
            #region ===== movieParadise SQLite : Return =====

            public movieParadise Return_Object_From_MovieParadise(string WhereClause = "[ID] = ''")
            {
                string DbFile = ahk.AppDir() + "\\Db\\MovieParadise.sqlite";

                string SelectLine = "Select [ID], [MoviePageLink], [IMDbID], [MovieTitle], [MovieYear], [ImageLink], [RapidGatorLinks], [InCollection], [DateModified] From " + movieParadise_TableName();
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
                movieParadise returnObject = new movieParadise();
                int i = 0;
                string Value = "";
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.MoviePageLink = ret["MoviePageLink"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.MovieTitle = ret["MovieTitle"].ToString();
                        returnObject.MovieYear = ret["MovieYear"].ToString();
                        returnObject.ImageLink = ret["ImageLink"].ToString();
                        returnObject.RapidGatorLinks = ret["RapidGatorLinks"].ToString();
                        returnObject.InCollection = ahk.ToBool(ret["InCollection"].ToString());
                        returnObject.DateModified = ret["DateModified"].ToString();
                    }
                }

                return returnObject;
            }

            public List<movieParadise> Return_movieParadise_List(string TableName = "[MovieParadise]", string WhereClause = "")
            {
                string DbFile = ahk.AppDir() + "\\Db\\MovieParadise.sqlite";

                string SelectLine = "Select * From " + TableName;

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

                List<movieParadise> ReturnList = new List<movieParadise>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        movieParadise returnObject = new movieParadise();

                        returnObject.ID = ret["ID"].ToString();
                        returnObject.MoviePageLink = ret["MoviePageLink"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.MovieTitle = ret["MovieTitle"].ToString();
                        returnObject.MovieYear = ret["MovieYear"].ToString();
                        returnObject.ImageLink = ret["ImageLink"].ToString();
                        returnObject.RapidGatorLinks = ret["RapidGatorLinks"].ToString();
                        returnObject.InCollection = ahk.ToBool(ret["InCollection"].ToString());
                        returnObject.DateModified = ret["DateModified"].ToString();

                        ReturnList.Add(returnObject);
                    }
                }

                return ReturnList;
            }

            public DataTable Return_DataTable_From_MovieParadise()
            {
                string DbFile = ahk.AppDir() + "\\Db\\MovieParadise.sqlite";

                string SelectLine = "Select [ID], [MoviePageLink], [IMDbID], [MovieTitle], [MovieYear], [ImageLink], [RapidGatorLinks], [InCollection], [DateModified] From " + movieParadise_TableName();
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                return ReturnTable;
            }


            #endregion
            #region ===== movieParadise SQLite : Update Insert =====

            public bool movieParadise_Insert(movieParadise inObject)
            {
                string DbFile = ahk.AppDir() + "\\Db\\MovieParadise.sqlite";

                string InsertLine = "Insert Into " + movieParadise_TableName() + " (MoviePageLink, IMDbID, MovieTitle, MovieYear, ImageLink, RapidGatorLinks, InCollection, DateModified) values ('" + inObject.MoviePageLink + "', '" + inObject.IMDbID + "', '" + inObject.MovieTitle + "', '" + inObject.MovieYear + "', '" + inObject.ImageLink + "', '" + inObject.RapidGatorLinks + "', '" + inObject.InCollection + "', '" + DateTime.Now + "')";
                bool Inserted = sqlite.Execute(DbFile, InsertLine);
                if (!Inserted) { ahk.MsgBox("Inserted Into [MovieParadise] = " + Inserted.ToString()); }
                return Inserted;
            }

            public bool movieParadise_Update(movieParadise inObject)
            {
                string DbFile = ahk.AppDir() + "\\Db\\MovieParadise.sqlite";

                //string UpdateLine = "Update [MovieParadise] set ID = '" + inObject.ID + "', MoviePageLink = '" + inObject.MoviePageLink + "', IMDbID = '" + inObject.IMDbID + "', MovieTitle = '" + inObject.MovieTitle + "', MovieYear = '" + inObject.MovieYear + "', ImageLink = '" + inObject.ImageLink + "', RapidGatorLinks = '" + inObject.RapidGatorLinks + "', InCollection = '" + inObject.InCollection + "', DateModified = '" + inObject.DateModified + "' WHERE [Item] = 'Value' ";
                string UpdateLine = "Update " + movieParadise_TableName() + " set ";

                if (inObject.MoviePageLink != null) { UpdateLine = UpdateLine + "[MoviePageLink] = '" + inObject.MoviePageLink + "',"; }
                if (inObject.IMDbID != null) { UpdateLine = UpdateLine + "[IMDbID] = '" + inObject.IMDbID + "',"; }
                if (inObject.MovieTitle != null) { UpdateLine = UpdateLine + "[MovieTitle] = '" + inObject.MovieTitle + "',"; }
                if (inObject.MovieYear != null) { UpdateLine = UpdateLine + "[MovieYear] = '" + inObject.MovieYear + "',"; }
                if (inObject.ImageLink != null) { UpdateLine = UpdateLine + "[ImageLink] = '" + inObject.ImageLink + "',"; }
                if (inObject.RapidGatorLinks != null) { UpdateLine = UpdateLine + "[RapidGatorLinks] = '" + inObject.RapidGatorLinks + "',"; }
                if (inObject.InCollection != null) { UpdateLine = UpdateLine + "[InCollection] = '" + inObject.InCollection + "',"; }

                UpdateLine = UpdateLine + "[DateModified] = '" + DateTime.Now + "',";

                UpdateLine = ahk.TrimLast(UpdateLine, 1);
                UpdateLine = UpdateLine + " WHERE [MoviePageLink] = '" + inObject.MoviePageLink + "'";

                bool Updated = sqlite.Execute(DbFile, UpdateLine);
                return Updated;
            }

            public bool movieParadise_SQLite_UpdateInsert(movieParadise obj)
            {
                bool Updated = movieParadise_Update(obj);  // try to update record first

                if (Updated) { MovieParadise_AlreadyInDbCount++; }

                if (!Updated) { movieParadise_Insert(obj); }  // if unable to update, insert new record
                return Updated;
            }


            #endregion


            /// <summary>
            /// Return # of Entries in MovieParadise Table
            /// </summary>
            /// <returns># of Entries in MovieParadise Table</returns>
            public int MovieParadiseTotal()
            {
                string returnVal = sql.SQL_Return_Value(movieParadise_Conn(), "select Count(IMDbID) FROM " + movieParadise_TableName());
                int count = ahk.ToInt(returnVal);
                return count;
            }

            /// <summary>
            /// Returns # of MovieParadise Links Missing RapidGator Links
            /// </summary>
            /// <param name="MissingFromCollectionOnly">If True, Only Returns # of Links Missing From Movies NOT In Collection</param>
            /// <returns></returns>
            public int MovieParadise_MissingRGLinks(bool MissingFromCollectionOnly = false)
            {
                string cmd = "select Count(IMDbID) FROM " + movieParadise_TableName() + " where RapidGatorLinks = ''";

                if (MissingFromCollectionOnly) { cmd = "select Count(IMDbID) FROM " + movieParadise_TableName() + " where InCollection = '0' and RapidGatorLinks = ''"; }

                string returnVal = sql.SQL_Return_Value(movieParadise_Conn(), cmd);
                int count = ahk.ToInt(returnVal);
                return count;
            }

            // return list of movieparadise sql entries that don't have rapidgator links populated
            public void PullMissingRapidLinks(Telerik.WinControls.UI.RadTextBox Txt1 = null)
            {
                string cmd = "select [MoviePageLink] FROM " + movieParadise_TableName() + " where InCollection = '0' and RapidGatorLinks = '' order by [IMDbID] desc";

                List<string> URLlist = lst.SQL_To_List(movieParadise_Conn(), cmd);

                sb.StatusBar(URLlist.Count + " URLs to Download...");

                int i = 0;
                int remaining = URLlist.Count();
                foreach (string url in URLlist)
                {
                    i++;

                    // calculate and display remaining number of links to check/update
                    remaining -= i;
                    tel.UpdateText(Txt1, remaining.ToString());

                    sb.StatusBar("Downloading Links " + i + "/" + URLlist.Count);

                    string html = web.DownloadHTML(url);

                    List<string> htmlLines = lst.Text_To_List(html, true, true, false);

                    string rapidlinks = "";
                    foreach (string line in htmlLines)
                    {
                        if (line.ToUpper().Contains("RAPIDGATOR.NET"))
                        {
                            if (!line.ToUpper().Contains("YOUR.SUPPORT.RAR"))
                            {
                                string linkTrim = ahk.StringSplit(line, "\"", 1);
                                linkTrim = ahk.StringSplit(linkTrim, "\"", 0);

                                //ahk.MsgBox(linkTrim);

                                if (rapidlinks != "") { rapidlinks = rapidlinks + " | " + linkTrim; }
                                if (rapidlinks == "") { rapidlinks = linkTrim; }
                            }

                        }
                    }

                    // ahk.MsgBox(rapidlinks);


                    bool updated = movieParadise_UpdateRGLinks(url, rapidlinks);

                    //ahk.MsgBox(html); 

                }

                sb.StatusBar("FINISHED Downloading Links");
            }

            /// <summary>
            /// Update MovieParadise Table with IMDb IDs from COLLECTION Db (if no id provided, updates all, otherwise only 1 id)
            /// Returns # of Movies Updated Successfully
            /// </summary>
            /// <param name="IMDbID">If provided, updates MovieParadise Db Indicating ID is IN Collection</param>
            public int Update_MovieParadise_InCollection(string IMDbID = "")
            {
                int updateCount = 0;
                if (IMDbID == "")
                {
                    List<string> InCol = lst.SQL_To_List(movieParadise_Conn(), "select [IMDbID] FROM [lucidmedia].[lucidmethod].[LITM_IMDb_New]");
                    int i = 0;
                    foreach (string imdbID in InCol)
                    {
                        i++;
                        bool updated = sql.Write_SQL(movieParadise_Conn(), "Update [lucidmedia].[lucidmethod].[MovieParadise] set [InCollection] = '1' WHERE [IMDbID] = '" + imdbID + "'");
                        sb.StatusBar(i + "/" + InCol.Count, 2);
                        if (updated) { updateCount++; }
                    }

                    ahk.MsgBox("Updated " + updateCount + " MovieParadise - InCollection Entries");
                }
                else
                {
                    bool updated = sql.Write_SQL(movieParadise_Conn(), "Update [lucidmedia].[lucidmethod].[MovieParadise] set [InCollection] = '1' WHERE [IMDbID] = '" + IMDbID + "'");
                    sb.StatusBar(1 + "/" + 1, 2);
                    if (updated)
                    {
                        updateCount++;
                        //ahk.MsgBox("Updated 1 MovieParadise - InCollection Entry");
                    }
                }

                return updateCount;
            }


        }

    }
}

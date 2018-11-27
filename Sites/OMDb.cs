using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using static sharpAHK_Dev._Sites.Imdb;
using sharpAHK;
using AHKExpressions;
using System.Threading;
using System.Configuration;
using System.Windows.Forms;

namespace sharpAHK_Dev
{
    public partial class _Sites
    {
        public class OMDB
        {
            _AHK ahk = new _AHK();
            _Sites.Imdb imdb = new Imdb(); 

            #region === OMDb ===

            /// <summary>
            /// Searchg OMDb API by IMDb ID 
            /// </summary>
            /// <param name="imdbID"></param>
            /// <returns></returns>
            public imdB OMdb(string imdbID = "tt0790799")
            {
                string apiKey = "6f5fca0b";
                string url = "http://www.omdbapi.com/?apikey=" + apiKey + "&i=" + imdbID + "&plot=full&r=json";

                using (WebClient wc = new WebClient())
                {
                    try
                    {
                        var json = wc.DownloadString(url);
                        JavaScriptSerializer oJS = new JavaScriptSerializer();
                        ImdbEntity obj = new ImdbEntity();
                        obj = oJS.Deserialize<ImdbEntity>(json);
                        if (obj.Response == "True")
                        {
                            imdB Movie = new imdB();
                            //public string Metascore { get; set; }
                            //public string Type { get; set; }
                            //public string Response { get; set; }
                            Movie.Title = obj.Title;
                            Movie.Year = obj.Year;
                            Movie.Rating = obj.imdbRating;
                            Movie.ReleaseDate = obj.Released;
                            Movie.Runtime = obj.Runtime;
                            Movie.Genres = obj.Genre;
                            Movie.Directors = obj.Director;
                            Movie.Writers = obj.Writer;
                            Movie.Cast = obj.Actors;
                            Movie.Plot = obj.Plot;
                            Movie.Languages = obj.Language;
                            Movie.Countries = obj.Country;
                            Movie.Awards = obj.Awards;
                            Movie.Poster = obj.Poster;
                            Movie.MpaaRating = obj.Rated;
                            Movie.Votes = obj.imdbVotes;
                            Movie.Tagline = obj.Type;
                            Movie.IMDbID = obj.imdbID;
                            return Movie;
                        }
                        else
                        {
                            //MessageBox.Show("Movie " + imdbID + " not Found in OMDb!!!");
                            return new imdB();
                        }
                    }
                    catch
                    {
                        // Pause and Try Again
                        //ahk.Sleep(2000);
                        ahk.MsgBox("OMDb Lookup Error: " + imdbID);
                        return new imdB();
                    }
                }

                return new imdB();
            }


            public imdB Search(string SearchTerm = "tt0790799", string Year = "", bool WriteBasicLink = true, string saveDir = "")
            {
                string apiKey = "6f5fca0b";

                string searchTerm = SearchTerm.Replace(" ", "+");

                string url = "http://www.omdbapi.com/?apikey=" + apiKey + "&t=" + searchTerm + "&r=json";
                if (Year != "") { url = url = "http://www.omdbapi.com/?apikey=" + apiKey + "&t=" + searchTerm + "&y=" + Year + "&r=json"; }

                //ahk.MsgBox(url);

                using (WebClient wc = new WebClient())
                {
                    try
                    {
                        var json = wc.DownloadString(url);
                        JavaScriptSerializer oJS = new JavaScriptSerializer();
                        ImdbEntity obj = new ImdbEntity();
                        obj = oJS.Deserialize<ImdbEntity>(json);
                        if (obj.Response == "True")
                        {
                            imdB Movie = new imdB();
                            //public string Metascore { get; set; }
                            //public string Type { get; set; }
                            //public string Response { get; set; }
                            Movie.Title = obj.Title;
                            Movie.Year = obj.Year;
                            Movie.Rating = obj.imdbRating;
                            Movie.ReleaseDate = obj.Released;
                            Movie.Runtime = obj.Runtime;
                            Movie.Genres = obj.Genre;
                            Movie.Directors = obj.Director;
                            Movie.Writers = obj.Writer;
                            Movie.Cast = obj.Actors;
                            Movie.Plot = obj.Plot;
                            Movie.Languages = obj.Language;
                            Movie.Countries = obj.Country;
                            Movie.Awards = obj.Awards;
                            Movie.Poster = obj.Poster;
                            Movie.MpaaRating = obj.Rated;
                            Movie.Votes = obj.imdbVotes;
                            Movie.Tagline = obj.Type;
                            Movie.IMDbID = obj.imdbID;

                            if (WriteBasicLink) { WriteIMDbFileBasic(obj.imdbID, saveDir); }

                            return Movie;
                        }
                        else
                        {
                            //MessageBox.Show("Movie " + searchTerm + " not Found in OMDb!!!");
                            return new imdB();
                        }
                    }
                    catch
                    {
                        // Pause and Try Again
                        //ahk.Sleep(2000);
                        ahk.MsgBox("OMDb Lookup Error: " + searchTerm);
                        return new imdB();
                    }
                }

                return new imdB();
            }


            /// <summary>
            /// Creates new Dir with MovieTitle (Year) + Creates Populated .URL File
            /// </summary>
            /// <param name="IMDbID"></param>
            /// <param name="SaveDirRoot"></param>
            /// <returns></returns>
            public string OMDb_NewDirURL(string IMDbID, string SaveDirRoot)
            {
                imdB movieInfo = OMdb(IMDbID);
                movieInfo = imdb.FixIMDbTitle(movieInfo);

                string url = "http://www.imdb.com/title/" + movieInfo.IMDbID + "/";

                string newDirName = SaveDirRoot + "\\" + movieInfo.Title + " (" + movieInfo.Year + ")";
                ahk.FileCreateDir(newDirName);

                imdb.IMDb_URL_Write(movieInfo, newDirName);  // write URL file populated with IMDb Info

                return newDirName;
            }

            // Renames Dir Contianing .Zip/.RAR with Movie Title (Year) + Creates Populated IMDb.URL
            public string OMDb_RenameURLDir(string IMDbID, string SaveDirRoot)
            {
                imdB movieInfo = OMdb(IMDbID);
                movieInfo = imdb.FixIMDbTitle(movieInfo);

                string url = "http://www.imdb.com/title/" + movieInfo.IMDbID + "/";

                string newDirName = SaveDirRoot + "\\" + movieInfo.Title + " (" + movieInfo.Year + ")";

                //ahk.FileRename(ahk.FileDir(), newDirName)


                ahk.FileCreateDir(newDirName);

                imdb.IMDb_URL_Write(movieInfo, newDirName);  // write URL file populated with IMDb Info

                return newDirName;
            }

            /// <summary>
            /// Looks up Movie Title by IMDbID from OMDb
            /// </summary>
            /// <param name="IMDbID"></param>
            /// <returns></returns>
            public string OMDb_Title(string IMDbID)
            {
                _Sites.OMDB om = new _Sites.OMDB();
                imdB movie = om.OMdb(IMDbID);
                return movie.Title;
            }



            public imdB OMdbPoster(string imdbID = "tt0790799")
            {
                string apiKey = "6f5fca0b";
                string url = "http://img.omdbapi.com/?apikey=" + apiKey + "&i=" + imdbID;

                using (WebClient wc = new WebClient())
                {
                    try
                    {
                        wc.DownloadFile(url, ahk.AppDir() + "\\" + imdbID + ".jpg");
                        //var json = wc.DownloadString(url);
                        //JavaScriptSerializer oJS = new JavaScriptSerializer();
                        //ImdbEntity obj = new ImdbEntity();
                        //obj = oJS.Deserialize<ImdbEntity>(json);
                        //if (obj.Response == "True")
                        //{
                        //    imdB Movie = new imdB();
                        //    //public string Metascore { get; set; }
                        //    //public string Type { get; set; }
                        //    //public string Response { get; set; }
                        //    Movie.Title = obj.Title;
                        //    Movie.Year = obj.Year;
                        //    Movie.Rating = obj.imdbRating;
                        //    Movie.ReleaseDate = obj.Released;
                        //    Movie.Runtime = obj.Runtime;
                        //    Movie.Genres = obj.Genre;
                        //    Movie.Directors = obj.Director;
                        //    Movie.Writers = obj.Writer;
                        //    Movie.Cast = obj.Actors;
                        //    Movie.Plot = obj.Plot;
                        //    Movie.Languages = obj.Language;
                        //    Movie.Countries = obj.Country;
                        //    Movie.Awards = obj.Awards;
                        //    Movie.Poster = obj.Poster;
                        //    Movie.MpaaRating = obj.Rated;
                        //    Movie.Votes = obj.imdbVotes;
                        //    Movie.Tagline = obj.Type;
                        //    Movie.IMDbID = obj.imdbID;
                        //    return Movie;
                        //}
                        //else
                        //{
                        //    //MessageBox.Show("Movie " + imdbID + " not Found in OMDb!!!");
                        //    return new imdB();
                        //}

                        retryCount = 0;
                    }
                    catch
                    {
                        if (retryCount > 2) { retryCount = 0; return new imdB(); }

                        retryCount++;
                        // Pause and Try Again
                        ahk.Sleep(2000);
                        return OMdb(imdbID);
                    }
                }

                return new imdB();
            }

            int retryCount = 0;

            public class ImdbEntity
            {
                public string Title { get; set; }
                public string Year { get; set; }
                public string Rated { get; set; }
                public string Released { get; set; }
                public string Runtime { get; set; }
                public string Genre { get; set; }
                public string Director { get; set; }
                public string Writer { get; set; }
                public string Actors { get; set; }
                public string Plot { get; set; }
                public string Language { get; set; }
                public string Country { get; set; }
                public string Awards { get; set; }
                public string Poster { get; set; }
                public string Metascore { get; set; }
                public string imdbRating { get; set; }
                public string imdbVotes { get; set; }
                public string imdbID { get; set; }
                public string Type { get; set; }
                public string Response { get; set; }
            }


            /// <summary>
            /// Populate OMDb SQL Db from List of IMDb IDs (NEW THREAD)
            /// </summary>
            /// <param name="IMDbIDs">IMDb Title ID</param>
            /// <param name="Bar">Optional RadProgresBar Display</param>
            public void OMDb_Populate(List<string> IMDbIDs, Telerik.WinControls.UI.RadProgressBar Bar = null, bool NewThread = true)
            {
                if (NewThread)
                {
                    Thread newThread = new Thread(() => OMDb_Populate(IMDbIDs, Bar, false)); // Function To Execute
                    newThread.Name = "Populate Missing OMDb Info";
                    newThread.IsBackground = true;
                    newThread.Start();
                }
                else
                {
                    int total = IMDbIDs.Count;

                    _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();

                    if (Bar != null) { pro.SetupProgressBar(Bar, total); } // setup progress bar if passed in

                    int i = 1;
                    foreach (string id in IMDbIDs)
                    {
                        imdB movie = OMdb(id);

                        OMDb om = new OMDb();
                        om.IMDbID = id;
                        om.Awards = movie.Awards;
                        om.Cast = movie.Cast;
                        om.Countries = movie.Countries;
                        om.Directors = movie.Directors;
                        om.Genre = movie.Genres;
                        om.Languages = movie.Languages;
                        om.MpaaRating = movie.MpaaRating;
                        om.Plot = movie.Plot;
                        om.Poster = movie.Poster;
                        om.Rating = movie.Rating;
                        om.ReleaseDate = movie.ReleaseDate;
                        om.RunTime = movie.Runtime;
                        om.Title = movie.Title;
                        om.Type = movie.Tagline;
                        om.Votes = movie.Votes;
                        om.Writers = movie.Writers;
                        om.Year = movie.Year;

                        OMDb_UpdateInsert(om);

                        // update progress bar if defined
                        if (Bar != null) { pro.UpdateProgress(Bar, i + "/" + total); }
                        i++;
                    }
                }
            }



            #region === OMDb FUNCTIONS ===

            #region ===== OMDb Object =====

            public struct OMDb
            {
                public string IMDbID { get; set; }
                public string Title { get; set; }
                public string Year { get; set; }
                public string Rating { get; set; }
                public string ReleaseDate { get; set; }
                public string RunTime { get; set; }
                public string Genre { get; set; }
                public string Directors { get; set; }
                public string Writers { get; set; }
                public string Cast { get; set; }
                public string Plot { get; set; }
                public string Languages { get; set; }
                public string Countries { get; set; }
                public string Awards { get; set; }
                public string Poster { get; set; }
                public string MpaaRating { get; set; }
                public string Votes { get; set; }
                public string Type { get; set; }
            }
            public OMDb Return_OMDb(string IMDbID = "", string Title = "", string Year = "", string Rating = "", string ReleaseDate = "", string RunTime = "", string Genre = "", string Directors = "", string Writers = "", string Cast = "", string Plot = "", string Languages = "", string Countries = "", string Awards = "", string Poster = "", string MpaaRating = "", string Votes = "", string Type = "")
            {
                OMDb obj = new OMDb();
                obj.IMDbID = IMDbID;
                obj.Title = Title;
                obj.Year = Year;
                obj.Rating = Rating;
                obj.ReleaseDate = ReleaseDate;
                obj.RunTime = RunTime;
                obj.Genre = Genre;
                obj.Directors = Directors;
                obj.Writers = Writers;
                obj.Cast = Cast;
                obj.Plot = Plot;
                obj.Languages = Languages;
                obj.Countries = Countries;
                obj.Awards = Awards;
                obj.Poster = Poster;
                obj.MpaaRating = MpaaRating;
                obj.Votes = Votes;
                obj.Type = Type;

                return obj;
            }

            //  Fix illegal characters before Sql/Sqlite Db Inserts
            public OMDb OMDb_FixChars(OMDb ToFix)
            {
                OMDb Fixed = new OMDb();

                Fixed.IMDbID = ToFix.IMDbID.Replace("'", "''");
                Fixed.Title = ToFix.Title.Replace("'", "''");
                Fixed.Year = ToFix.Year.Replace("'", "''");
                Fixed.Rating = ToFix.Rating.Replace("'", "''");
                Fixed.ReleaseDate = ToFix.ReleaseDate.Replace("'", "''");
                Fixed.RunTime = ToFix.RunTime.Replace("'", "''");
                Fixed.Genre = ToFix.Genre.Replace("'", "''");
                Fixed.Directors = ToFix.Directors.Replace("'", "''");
                Fixed.Writers = ToFix.Writers.Replace("'", "''");
                Fixed.Cast = ToFix.Cast.Replace("'", "''");
                Fixed.Plot = ToFix.Plot.Replace("'", "''");
                Fixed.Languages = ToFix.Languages.Replace("'", "''");
                Fixed.Countries = ToFix.Countries.Replace("'", "''");
                Fixed.Awards = ToFix.Awards.Replace("'", "''");
                Fixed.Poster = ToFix.Poster.Replace("'", "''");
                Fixed.MpaaRating = ToFix.MpaaRating.Replace("'", "''");
                Fixed.Votes = ToFix.Votes.Replace("'", "''");
                Fixed.Type = ToFix.Type.Replace("'", "''");

                return Fixed;
            }

            // Compare two objects to see if they have identical values
            public bool OMDb_Changed(OMDb OldVal, OMDb NewVal)
            {
                OMDb diff = new OMDb();
                List<string> diffList = new List<string>();
                bool different = false;
                if (OldVal.IMDbID == null) { OldVal.IMDbID = ""; }
                if (NewVal.IMDbID == null) { NewVal.IMDbID = ""; }
                if (OldVal.IMDbID != NewVal.IMDbID) { different = true; }
                if (OldVal.Title == null) { OldVal.Title = ""; }
                if (NewVal.Title == null) { NewVal.Title = ""; }
                if (OldVal.Title != NewVal.Title) { different = true; }
                if (OldVal.Year == null) { OldVal.Year = ""; }
                if (NewVal.Year == null) { NewVal.Year = ""; }
                if (OldVal.Year != NewVal.Year) { different = true; }
                if (OldVal.Rating == null) { OldVal.Rating = ""; }
                if (NewVal.Rating == null) { NewVal.Rating = ""; }
                if (OldVal.Rating != NewVal.Rating) { different = true; }
                if (OldVal.ReleaseDate == null) { OldVal.ReleaseDate = ""; }
                if (NewVal.ReleaseDate == null) { NewVal.ReleaseDate = ""; }
                if (OldVal.ReleaseDate != NewVal.ReleaseDate) { different = true; }
                if (OldVal.RunTime == null) { OldVal.RunTime = ""; }
                if (NewVal.RunTime == null) { NewVal.RunTime = ""; }
                if (OldVal.RunTime != NewVal.RunTime) { different = true; }
                if (OldVal.Genre == null) { OldVal.Genre = ""; }
                if (NewVal.Genre == null) { NewVal.Genre = ""; }
                if (OldVal.Genre != NewVal.Genre) { different = true; }
                if (OldVal.Directors == null) { OldVal.Directors = ""; }
                if (NewVal.Directors == null) { NewVal.Directors = ""; }
                if (OldVal.Directors != NewVal.Directors) { different = true; }
                if (OldVal.Writers == null) { OldVal.Writers = ""; }
                if (NewVal.Writers == null) { NewVal.Writers = ""; }
                if (OldVal.Writers != NewVal.Writers) { different = true; }
                if (OldVal.Cast == null) { OldVal.Cast = ""; }
                if (NewVal.Cast == null) { NewVal.Cast = ""; }
                if (OldVal.Cast != NewVal.Cast) { different = true; }
                if (OldVal.Plot == null) { OldVal.Plot = ""; }
                if (NewVal.Plot == null) { NewVal.Plot = ""; }
                if (OldVal.Plot != NewVal.Plot) { different = true; }
                if (OldVal.Languages == null) { OldVal.Languages = ""; }
                if (NewVal.Languages == null) { NewVal.Languages = ""; }
                if (OldVal.Languages != NewVal.Languages) { different = true; }
                if (OldVal.Countries == null) { OldVal.Countries = ""; }
                if (NewVal.Countries == null) { NewVal.Countries = ""; }
                if (OldVal.Countries != NewVal.Countries) { different = true; }
                if (OldVal.Awards == null) { OldVal.Awards = ""; }
                if (NewVal.Awards == null) { NewVal.Awards = ""; }
                if (OldVal.Awards != NewVal.Awards) { different = true; }
                if (OldVal.Poster == null) { OldVal.Poster = ""; }
                if (NewVal.Poster == null) { NewVal.Poster = ""; }
                if (OldVal.Poster != NewVal.Poster) { different = true; }
                if (OldVal.MpaaRating == null) { OldVal.MpaaRating = ""; }
                if (NewVal.MpaaRating == null) { NewVal.MpaaRating = ""; }
                if (OldVal.MpaaRating != NewVal.MpaaRating) { different = true; }
                if (OldVal.Votes == null) { OldVal.Votes = ""; }
                if (NewVal.Votes == null) { NewVal.Votes = ""; }
                if (OldVal.Votes != NewVal.Votes) { different = true; }
                if (OldVal.Type == null) { OldVal.Type = ""; }
                if (NewVal.Type == null) { NewVal.Type = ""; }
                if (OldVal.Type != NewVal.Type) { different = true; }
                return different;
            }

            // Returns object containing the new values different from the old values in object comparison
            public OMDb OMDb_Diff(OMDb OldVal, OMDb NewVal)
            {
                OMDb diff = new OMDb();
                if (OldVal.IMDbID != NewVal.IMDbID) { diff.IMDbID = NewVal.IMDbID; }
                if (OldVal.Title != NewVal.Title) { diff.Title = NewVal.Title; }
                if (OldVal.Year != NewVal.Year) { diff.Year = NewVal.Year; }
                if (OldVal.Rating != NewVal.Rating) { diff.Rating = NewVal.Rating; }
                if (OldVal.ReleaseDate != NewVal.ReleaseDate) { diff.ReleaseDate = NewVal.ReleaseDate; }
                if (OldVal.RunTime != NewVal.RunTime) { diff.RunTime = NewVal.RunTime; }
                if (OldVal.Genre != NewVal.Genre) { diff.Genre = NewVal.Genre; }
                if (OldVal.Directors != NewVal.Directors) { diff.Directors = NewVal.Directors; }
                if (OldVal.Writers != NewVal.Writers) { diff.Writers = NewVal.Writers; }
                if (OldVal.Cast != NewVal.Cast) { diff.Cast = NewVal.Cast; }
                if (OldVal.Plot != NewVal.Plot) { diff.Plot = NewVal.Plot; }
                if (OldVal.Languages != NewVal.Languages) { diff.Languages = NewVal.Languages; }
                if (OldVal.Countries != NewVal.Countries) { diff.Countries = NewVal.Countries; }
                if (OldVal.Awards != NewVal.Awards) { diff.Awards = NewVal.Awards; }
                if (OldVal.Poster != NewVal.Poster) { diff.Poster = NewVal.Poster; }
                if (OldVal.MpaaRating != NewVal.MpaaRating) { diff.MpaaRating = NewVal.MpaaRating; }
                if (OldVal.Votes != NewVal.Votes) { diff.Votes = NewVal.Votes; }
                if (OldVal.Type != NewVal.Type) { diff.Type = NewVal.Type; }
                return diff;
            }

            // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
            public List<string> OMDb_DiffList(OMDb OldVal, OMDb NewVal)
            {
                List<string> diffList = new List<string>();
                if (OldVal.IMDbID != NewVal.IMDbID) { diffList.Add("Changed IMDbID Value From " + OldVal.IMDbID + " To " + NewVal.IMDbID); }
                if (OldVal.Title != NewVal.Title) { diffList.Add("Changed Title Value From " + OldVal.Title + " To " + NewVal.Title); }
                if (OldVal.Year != NewVal.Year) { diffList.Add("Changed Year Value From " + OldVal.Year + " To " + NewVal.Year); }
                if (OldVal.Rating != NewVal.Rating) { diffList.Add("Changed Rating Value From " + OldVal.Rating + " To " + NewVal.Rating); }
                if (OldVal.ReleaseDate != NewVal.ReleaseDate) { diffList.Add("Changed ReleaseDate Value From " + OldVal.ReleaseDate + " To " + NewVal.ReleaseDate); }
                if (OldVal.RunTime != NewVal.RunTime) { diffList.Add("Changed RunTime Value From " + OldVal.RunTime + " To " + NewVal.RunTime); }
                if (OldVal.Genre != NewVal.Genre) { diffList.Add("Changed Genre Value From " + OldVal.Genre + " To " + NewVal.Genre); }
                if (OldVal.Directors != NewVal.Directors) { diffList.Add("Changed Directors Value From " + OldVal.Directors + " To " + NewVal.Directors); }
                if (OldVal.Writers != NewVal.Writers) { diffList.Add("Changed Writers Value From " + OldVal.Writers + " To " + NewVal.Writers); }
                if (OldVal.Cast != NewVal.Cast) { diffList.Add("Changed Cast Value From " + OldVal.Cast + " To " + NewVal.Cast); }
                if (OldVal.Plot != NewVal.Plot) { diffList.Add("Changed Plot Value From " + OldVal.Plot + " To " + NewVal.Plot); }
                if (OldVal.Languages != NewVal.Languages) { diffList.Add("Changed Languages Value From " + OldVal.Languages + " To " + NewVal.Languages); }
                if (OldVal.Countries != NewVal.Countries) { diffList.Add("Changed Countries Value From " + OldVal.Countries + " To " + NewVal.Countries); }
                if (OldVal.Awards != NewVal.Awards) { diffList.Add("Changed Awards Value From " + OldVal.Awards + " To " + NewVal.Awards); }
                if (OldVal.Poster != NewVal.Poster) { diffList.Add("Changed Poster Value From " + OldVal.Poster + " To " + NewVal.Poster); }
                if (OldVal.MpaaRating != NewVal.MpaaRating) { diffList.Add("Changed MpaaRating Value From " + OldVal.MpaaRating + " To " + NewVal.MpaaRating); }
                if (OldVal.Votes != NewVal.Votes) { diffList.Add("Changed Votes Value From " + OldVal.Votes + " To " + NewVal.Votes); }
                if (OldVal.Type != NewVal.Type) { diffList.Add("Changed Type Value From " + OldVal.Type + " To " + NewVal.Type); }
                return diffList;
            }


            #endregion
            #region ===== OMDb SQLite : Return =====

            public OMDb Return_Object_From_OMDb(string DbFile, string WhereClause = "[IMDbID] = ''")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                string SelectLine = "Select [IMDbID], [Title], [Year], [Rating], [ReleaseDate], [RunTime], [Genre], [Directors], [Writers], [Cast], [Plot], [Languages], [Countries], [Awards], [Poster], [MpaaRating], [Votes], [Type] From [OMDb] ";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
                OMDb returnObject = new OMDb();
                int i = 0;
                string Value = "";
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.Title = ret["Title"].ToString();
                        returnObject.Year = ret["Year"].ToString();
                        returnObject.Rating = ret["Rating"].ToString();
                        returnObject.ReleaseDate = ret["ReleaseDate"].ToString();
                        returnObject.RunTime = ret["RunTime"].ToString();
                        returnObject.Genre = ret["Genre"].ToString();
                        returnObject.Directors = ret["Directors"].ToString();
                        returnObject.Writers = ret["Writers"].ToString();
                        returnObject.Cast = ret["Cast"].ToString();
                        returnObject.Plot = ret["Plot"].ToString();
                        returnObject.Languages = ret["Languages"].ToString();
                        returnObject.Countries = ret["Countries"].ToString();
                        returnObject.Awards = ret["Awards"].ToString();
                        returnObject.Poster = ret["Poster"].ToString();
                        returnObject.MpaaRating = ret["MpaaRating"].ToString();
                        returnObject.Votes = ret["Votes"].ToString();
                        returnObject.Type = ret["Type"].ToString();
                    }
                }

                return returnObject;
            }

            public List<OMDb> Return_OMDb_List(string DbFile, string TableName = "[OMDb]", string WhereClause = "")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                string SelectLine = "Select * From " + TableName;

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

                List<OMDb> ReturnList = new List<OMDb>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        OMDb returnObject = new OMDb();

                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.Title = ret["Title"].ToString();
                        returnObject.Year = ret["Year"].ToString();
                        returnObject.Rating = ret["Rating"].ToString();
                        returnObject.ReleaseDate = ret["ReleaseDate"].ToString();
                        returnObject.RunTime = ret["RunTime"].ToString();
                        returnObject.Genre = ret["Genre"].ToString();
                        returnObject.Directors = ret["Directors"].ToString();
                        returnObject.Writers = ret["Writers"].ToString();
                        returnObject.Cast = ret["Cast"].ToString();
                        returnObject.Plot = ret["Plot"].ToString();
                        returnObject.Languages = ret["Languages"].ToString();
                        returnObject.Countries = ret["Countries"].ToString();
                        returnObject.Awards = ret["Awards"].ToString();
                        returnObject.Poster = ret["Poster"].ToString();
                        returnObject.MpaaRating = ret["MpaaRating"].ToString();
                        returnObject.Votes = ret["Votes"].ToString();
                        returnObject.Type = ret["Type"].ToString();

                        ReturnList.Add(returnObject);
                    }
                }

                return ReturnList;
            }

            public DataTable Return_DataTable_From_OMDb(string DbFile)
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                string SelectLine = "Select [IMDbID], [Title], [Year], [Rating], [ReleaseDate], [RunTime], [Genre], [Directors], [Writers], [Cast], [Plot], [Languages], [Countries], [Awards], [Poster], [MpaaRating], [Votes], [Type] From [OMDb]";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                return ReturnTable;
            }


            #endregion
            #region ===== OMDb SQLite : Update Insert =====

            public bool OMDb_Insert(string DbFile, OMDb inObject)
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                string InsertLine = "Insert Into [OMDb] (IMDbID, Title, Year, Rating, ReleaseDate, RunTime, Genre, Directors, Writers, Cast, Plot, Languages, Countries, Awards, Poster, MpaaRating, Votes, Type) values ('" + inObject.IMDbID + "', '" + inObject.Title + "', '" + inObject.Year + "', '" + inObject.Rating + "', '" + inObject.ReleaseDate + "', '" + inObject.RunTime + "', '" + inObject.Genre + "', '" + inObject.Directors + "', '" + inObject.Writers + "', '" + inObject.Cast + "', '" + inObject.Plot + "', '" + inObject.Languages + "', '" + inObject.Countries + "', '" + inObject.Awards + "', '" + inObject.Poster + "', '" + inObject.MpaaRating + "', '" + inObject.Votes + "', '" + inObject.Type + "')";
                bool Inserted = sqlite.Execute(DbFile, InsertLine);
                if (!Inserted) { ahk.MsgBox("Inserted Into [OMDb] = " + Inserted.ToString()); }
                return Inserted;
            }

            public bool OMDb_Update(string DbFile, OMDb inObject, string WHERE = "[Item] = 'Value'")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                //string UpdateLine = "Update [OMDb] set IMDbID = '" + inObject.IMDbID + "', Title = '" + inObject.Title + "', Year = '" + inObject.Year + "', Rating = '" + inObject.Rating + "', ReleaseDate = '" + inObject.ReleaseDate + "', RunTime = '" + inObject.RunTime + "', Genre = '" + inObject.Genre + "', Directors = '" + inObject.Directors + "', Writers = '" + inObject.Writers + "', Cast = '" + inObject.Cast + "', Plot = '" + inObject.Plot + "', Languages = '" + inObject.Languages + "', Countries = '" + inObject.Countries + "', Awards = '" + inObject.Awards + "', Poster = '" + inObject.Poster + "', MpaaRating = '" + inObject.MpaaRating + "', Votes = '" + inObject.Votes + "', Type = '" + inObject.Type + "' WHERE [Item] = 'Value' ";
                string UpdateLine = "Update [OMDb] set ";


                if (inObject.IMDbID != null) { UpdateLine = UpdateLine + "[IMDbID] = '" + inObject.IMDbID + "',"; }
                if (inObject.Title != null) { UpdateLine = UpdateLine + "[Title] = '" + inObject.Title + "',"; }
                if (inObject.Year != null) { UpdateLine = UpdateLine + "[Year] = '" + inObject.Year + "',"; }
                if (inObject.Rating != null) { UpdateLine = UpdateLine + "[Rating] = '" + inObject.Rating + "',"; }
                if (inObject.ReleaseDate != null) { UpdateLine = UpdateLine + "[ReleaseDate] = '" + inObject.ReleaseDate + "',"; }
                if (inObject.RunTime != null) { UpdateLine = UpdateLine + "[RunTime] = '" + inObject.RunTime + "',"; }
                if (inObject.Genre != null) { UpdateLine = UpdateLine + "[Genre] = '" + inObject.Genre + "',"; }
                if (inObject.Directors != null) { UpdateLine = UpdateLine + "[Directors] = '" + inObject.Directors + "',"; }
                if (inObject.Writers != null) { UpdateLine = UpdateLine + "[Writers] = '" + inObject.Writers + "',"; }
                if (inObject.Cast != null) { UpdateLine = UpdateLine + "[Cast] = '" + inObject.Cast + "',"; }
                if (inObject.Plot != null) { UpdateLine = UpdateLine + "[Plot] = '" + inObject.Plot + "',"; }
                if (inObject.Languages != null) { UpdateLine = UpdateLine + "[Languages] = '" + inObject.Languages + "',"; }
                if (inObject.Countries != null) { UpdateLine = UpdateLine + "[Countries] = '" + inObject.Countries + "',"; }
                if (inObject.Awards != null) { UpdateLine = UpdateLine + "[Awards] = '" + inObject.Awards + "',"; }
                if (inObject.Poster != null) { UpdateLine = UpdateLine + "[Poster] = '" + inObject.Poster + "',"; }
                if (inObject.MpaaRating != null) { UpdateLine = UpdateLine + "[MpaaRating] = '" + inObject.MpaaRating + "',"; }
                if (inObject.Votes != null) { UpdateLine = UpdateLine + "[Votes] = '" + inObject.Votes + "',"; }
                if (inObject.Type != null) { UpdateLine = UpdateLine + "[Type] = '" + inObject.Type + "',"; }

                UpdateLine = ahk.TrimLast(UpdateLine, 1);
                UpdateLine = UpdateLine + " WHERE " + WHERE;

                bool Updated = sqlite.Execute(DbFile, UpdateLine);
                return Updated;
            }

            public bool OMDb_UpdateInsert(string DbFile, OMDb obj, string WhereClause = "")
            {
                bool Updated = OMDb_Update(DbFile, obj, WhereClause);  // try to update record first
                if (!Updated) { Updated = OMDb_Insert(DbFile, obj); }  // if unable to update, insert new record
                return Updated;
            }


            #endregion
            #region ===== OMDb DataTable =====

            public DataTable Return_OMDb_DataTable(string DbFile, string TableName = "OMDb", string WhereClause = "", bool Debug = false)
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                string SelectLine = "Select * From [OMDb]";

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }

                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


                DataTable table = new DataTable();
                table.Columns.Add("IMDbID", typeof(string));
                table.Columns.Add("Title", typeof(string));
                table.Columns.Add("Year", typeof(string));
                table.Columns.Add("Rating", typeof(string));
                table.Columns.Add("ReleaseDate", typeof(string));
                table.Columns.Add("RunTime", typeof(string));
                table.Columns.Add("Genre", typeof(string));
                table.Columns.Add("Directors", typeof(string));
                table.Columns.Add("Writers", typeof(string));
                table.Columns.Add("Cast", typeof(string));
                table.Columns.Add("Plot", typeof(string));
                table.Columns.Add("Languages", typeof(string));
                table.Columns.Add("Countries", typeof(string));
                table.Columns.Add("Awards", typeof(string));
                table.Columns.Add("Poster", typeof(string));
                table.Columns.Add("MpaaRating", typeof(string));
                table.Columns.Add("Votes", typeof(string));
                table.Columns.Add("Type", typeof(string));

                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        OMDb returnObject = new OMDb();

                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.Title = ret["Title"].ToString();
                        returnObject.Year = ret["Year"].ToString();
                        returnObject.Rating = ret["Rating"].ToString();
                        returnObject.ReleaseDate = ret["ReleaseDate"].ToString();
                        returnObject.RunTime = ret["RunTime"].ToString();
                        returnObject.Genre = ret["Genre"].ToString();
                        returnObject.Directors = ret["Directors"].ToString();
                        returnObject.Writers = ret["Writers"].ToString();
                        returnObject.Cast = ret["Cast"].ToString();
                        returnObject.Plot = ret["Plot"].ToString();
                        returnObject.Languages = ret["Languages"].ToString();
                        returnObject.Countries = ret["Countries"].ToString();
                        returnObject.Awards = ret["Awards"].ToString();
                        returnObject.Poster = ret["Poster"].ToString();
                        returnObject.MpaaRating = ret["MpaaRating"].ToString();
                        returnObject.Votes = ret["Votes"].ToString();
                        returnObject.Type = ret["Type"].ToString();

                        table.Rows.Add(returnObject.IMDbID, returnObject.Title, returnObject.Year, returnObject.Rating, returnObject.ReleaseDate, returnObject.RunTime, returnObject.Genre, returnObject.Directors, returnObject.Writers, returnObject.Cast, returnObject.Plot, returnObject.Languages, returnObject.Countries, returnObject.Awards, returnObject.Poster, returnObject.MpaaRating, returnObject.Votes, returnObject.Type);
                    }
                }

                return table;
            }

            public DataTable Create_OMDb_DataTable(OMDb inObject)
            {
                DataTable table = new DataTable();
                table.Columns.Add("IMDbID", typeof(string));
                table.Columns.Add("Title", typeof(string));
                table.Columns.Add("Year", typeof(string));
                table.Columns.Add("Rating", typeof(string));
                table.Columns.Add("ReleaseDate", typeof(string));
                table.Columns.Add("RunTime", typeof(string));
                table.Columns.Add("Genre", typeof(string));
                table.Columns.Add("Directors", typeof(string));
                table.Columns.Add("Writers", typeof(string));
                table.Columns.Add("Cast", typeof(string));
                table.Columns.Add("Plot", typeof(string));
                table.Columns.Add("Languages", typeof(string));
                table.Columns.Add("Countries", typeof(string));
                table.Columns.Add("Awards", typeof(string));
                table.Columns.Add("Poster", typeof(string));
                table.Columns.Add("MpaaRating", typeof(string));
                table.Columns.Add("Votes", typeof(string));
                table.Columns.Add("Type", typeof(string));

                table.Rows.Add(inObject.IMDbID, inObject.Title, inObject.Year, inObject.Rating, inObject.ReleaseDate, inObject.RunTime, inObject.Genre, inObject.Directors, inObject.Writers, inObject.Cast, inObject.Plot, inObject.Languages, inObject.Countries, inObject.Awards, inObject.Poster, inObject.MpaaRating, inObject.Votes, inObject.Type);
                return table;
            }


            #endregion
            #region ===== OMDb DataGridView =====

            public void HideShow_OMDb_Columns(DataGridView dv)
            {

                try { dv.Columns["IMDbID"].Visible = true; } catch { }
                try { dv.Columns["Title"].Visible = true; } catch { }
                try { dv.Columns["Year"].Visible = true; } catch { }
                try { dv.Columns["Rating"].Visible = true; } catch { }
                try { dv.Columns["ReleaseDate"].Visible = true; } catch { }
                try { dv.Columns["RunTime"].Visible = true; } catch { }
                try { dv.Columns["Genre"].Visible = true; } catch { }
                try { dv.Columns["Directors"].Visible = true; } catch { }
                try { dv.Columns["Writers"].Visible = true; } catch { }
                try { dv.Columns["Cast"].Visible = true; } catch { }
                try { dv.Columns["Plot"].Visible = true; } catch { }
                try { dv.Columns["Languages"].Visible = true; } catch { }
                try { dv.Columns["Countries"].Visible = true; } catch { }
                try { dv.Columns["Awards"].Visible = true; } catch { }
                try { dv.Columns["Poster"].Visible = true; } catch { }
                try { dv.Columns["MpaaRating"].Visible = true; } catch { }
                try { dv.Columns["Votes"].Visible = true; } catch { }
                try { dv.Columns["Type"].Visible = true; } catch { }
            }
            public void Enable_TableNamea_Columns(DataGridView dv)
            {

                try { dv.Columns["IMDbID"].ReadOnly = true; } catch { }
                try { dv.Columns["Title"].ReadOnly = true; } catch { }
                try { dv.Columns["Year"].ReadOnly = true; } catch { }
                try { dv.Columns["Rating"].ReadOnly = true; } catch { }
                try { dv.Columns["ReleaseDate"].ReadOnly = true; } catch { }
                try { dv.Columns["RunTime"].ReadOnly = true; } catch { }
                try { dv.Columns["Genre"].ReadOnly = true; } catch { }
                try { dv.Columns["Directors"].ReadOnly = true; } catch { }
                try { dv.Columns["Writers"].ReadOnly = true; } catch { }
                try { dv.Columns["Cast"].ReadOnly = true; } catch { }
                try { dv.Columns["Plot"].ReadOnly = true; } catch { }
                try { dv.Columns["Languages"].ReadOnly = true; } catch { }
                try { dv.Columns["Countries"].ReadOnly = true; } catch { }
                try { dv.Columns["Awards"].ReadOnly = true; } catch { }
                try { dv.Columns["Poster"].ReadOnly = true; } catch { }
                try { dv.Columns["MpaaRating"].ReadOnly = true; } catch { }
                try { dv.Columns["Votes"].ReadOnly = true; } catch { }
                try { dv.Columns["Type"].ReadOnly = true; } catch { }
            }

            #endregion
            #region ===== OMDb SQL Functions =====

            // Return OMDb SQL Connection String
            public SqlConnection OMDb_Conn()
            {
                // populate sql connection
                //SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLserver"].ConnectionString);
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMLucidMedia"].ConnectionString);
                return conn;
            }

            // Return OMDb TableName (Full Path)
            public string OMDb_TableName()
            {
                // populate to return full sql table name
                return "[LucidMedia].[dbo].[OMDb]";
            }

            // Generate SQL Table
            public bool OMDb_CreateSQLTable()
            {
                SqlConnection Conn = OMDb_Conn();
                string CreateTableLine = "CREATE TABLE [OMDb](";
                CreateTableLine = CreateTableLine + "[IMDbID] [int] IDENTITY(1,1) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Title] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Year] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Rating] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[ReleaseDate] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[RunTime] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Genre] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Directors] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Writers] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Cast] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Plot] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Languages] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Countries] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Awards] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Poster] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[MpaaRating] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Votes] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Type] [varchar](max) NOT NULL";
                CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                return false;
            }

            public bool OMDb_InsertSQL(OMDb obj)
            {
                //SqlConnection Con = OMDb_Conn();
                SqlConnection Con = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMLucidMedia"].ConnectionString);

                string SQLLine = "Insert Into " + OMDb_TableName() + " (IMDbID, Title, Year, Rating, ReleaseDate, RunTime, Genre, Directors, Writers, Cast, Plot, Languages, Countries, Awards, Poster, MpaaRating, Votes, Type) VALUES (@IMDbID, @Title, @Year, @Rating, @ReleaseDate, @RunTime, @Genre, @Directors, @Writers, @Cast, @Plot, @Languages, @Countries, @Awards, @Poster, @MpaaRating, @Votes, @Type)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.IMDbID == null) { obj.IMDbID = ""; }
                if (obj.Title == null) { obj.Title = ""; }
                if (obj.Year == null) { obj.Year = ""; }
                if (obj.Rating == null) { obj.Rating = ""; }
                if (obj.ReleaseDate == null) { obj.ReleaseDate = ""; }
                if (obj.RunTime == null) { obj.RunTime = ""; }
                if (obj.Genre == null) { obj.Genre = ""; }
                if (obj.Directors == null) { obj.Directors = ""; }
                if (obj.Writers == null) { obj.Writers = ""; }
                if (obj.Cast == null) { obj.Cast = ""; }
                if (obj.Plot == null) { obj.Plot = ""; }
                if (obj.Languages == null) { obj.Languages = ""; }
                if (obj.Countries == null) { obj.Countries = ""; }
                if (obj.Awards == null) { obj.Awards = ""; }
                if (obj.Poster == null) { obj.Poster = ""; }
                if (obj.MpaaRating == null) { obj.MpaaRating = ""; }
                if (obj.Votes == null) { obj.Votes = ""; }
                if (obj.Type == null) { obj.Type = ""; }
                cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID.ToString());
                cmd2.Parameters.AddWithValue(@"Title", obj.Title.ToString());
                cmd2.Parameters.AddWithValue(@"Year", obj.Year.ToString());
                cmd2.Parameters.AddWithValue(@"Rating", obj.Rating.ToString());
                cmd2.Parameters.AddWithValue(@"ReleaseDate", obj.ReleaseDate.ToString());
                cmd2.Parameters.AddWithValue(@"RunTime", obj.RunTime.ToString());
                cmd2.Parameters.AddWithValue(@"Genre", obj.Genre.ToString());
                cmd2.Parameters.AddWithValue(@"Directors", obj.Directors.ToString());
                cmd2.Parameters.AddWithValue(@"Writers", obj.Writers.ToString());
                cmd2.Parameters.AddWithValue(@"Cast", obj.Cast.ToString());
                cmd2.Parameters.AddWithValue(@"Plot", obj.Plot.ToString());
                cmd2.Parameters.AddWithValue(@"Languages", obj.Languages.ToString());
                cmd2.Parameters.AddWithValue(@"Countries", obj.Countries.ToString());
                cmd2.Parameters.AddWithValue(@"Awards", obj.Awards.ToString());
                cmd2.Parameters.AddWithValue(@"Poster", obj.Poster.ToString());
                cmd2.Parameters.AddWithValue(@"MpaaRating", obj.MpaaRating.ToString());
                cmd2.Parameters.AddWithValue(@"Votes", obj.Votes.ToString());
                cmd2.Parameters.AddWithValue(@"Type", obj.Type.ToString());

                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        OMDb_InsertSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        OMDb_InsertSQL(obj);
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

            public bool OMDb_UpdateSQL(OMDb obj)
            {
                SqlConnection Conn = OMDb_Conn();
                string SQLLine = "Update " + OMDb_TableName() + " SET IMDbID = @IMDbID, Title = @Title, Year = @Year, Rating = @Rating, ReleaseDate = @ReleaseDate, RunTime = @RunTime, Genre = @Genre, Directors = @Directors, Writers = @Writers, Cast = @Cast, Plot = @Plot, Languages = @Languages, Countries = @Countries, Awards = @Awards, Poster = @Poster, MpaaRating = @MpaaRating, Votes = @Votes, Type = @Type WHERE IMDbID = @IMDbID";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.IMDbID == null) { obj.IMDbID = ""; }
                if (obj.Title == null) { obj.Title = ""; }
                if (obj.Year == null) { obj.Year = ""; }
                if (obj.Rating == null) { obj.Rating = ""; }
                if (obj.ReleaseDate == null) { obj.ReleaseDate = ""; }
                if (obj.RunTime == null) { obj.RunTime = ""; }
                if (obj.Genre == null) { obj.Genre = ""; }
                if (obj.Directors == null) { obj.Directors = ""; }
                if (obj.Writers == null) { obj.Writers = ""; }
                if (obj.Cast == null) { obj.Cast = ""; }
                if (obj.Plot == null) { obj.Plot = ""; }
                if (obj.Languages == null) { obj.Languages = ""; }
                if (obj.Countries == null) { obj.Countries = ""; }
                if (obj.Awards == null) { obj.Awards = ""; }
                if (obj.Poster == null) { obj.Poster = ""; }
                if (obj.MpaaRating == null) { obj.MpaaRating = ""; }
                if (obj.Votes == null) { obj.Votes = ""; }
                if (obj.Type == null) { obj.Type = ""; }
                cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID.ToString());
                cmd2.Parameters.AddWithValue(@"Title", obj.Title.ToString());
                cmd2.Parameters.AddWithValue(@"Year", obj.Year.ToString());
                cmd2.Parameters.AddWithValue(@"Rating", obj.Rating.ToString());
                cmd2.Parameters.AddWithValue(@"ReleaseDate", obj.ReleaseDate.ToString());
                cmd2.Parameters.AddWithValue(@"RunTime", obj.RunTime.ToString());
                cmd2.Parameters.AddWithValue(@"Genre", obj.Genre.ToString());
                cmd2.Parameters.AddWithValue(@"Directors", obj.Directors.ToString());
                cmd2.Parameters.AddWithValue(@"Writers", obj.Writers.ToString());
                cmd2.Parameters.AddWithValue(@"Cast", obj.Cast.ToString());
                cmd2.Parameters.AddWithValue(@"Plot", obj.Plot.ToString());
                cmd2.Parameters.AddWithValue(@"Languages", obj.Languages.ToString());
                cmd2.Parameters.AddWithValue(@"Countries", obj.Countries.ToString());
                cmd2.Parameters.AddWithValue(@"Awards", obj.Awards.ToString());
                cmd2.Parameters.AddWithValue(@"Poster", obj.Poster.ToString());
                cmd2.Parameters.AddWithValue(@"MpaaRating", obj.MpaaRating.ToString());
                cmd2.Parameters.AddWithValue(@"Votes", obj.Votes.ToString());
                cmd2.Parameters.AddWithValue(@"Type", obj.Type.ToString());
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        OMDb_UpdateSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        OMDb_UpdateSQL(obj);
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

            public bool OMDb_UpdateInsert(OMDb obj)
            {
                SqlConnection Conn = OMDb_Conn();
                bool Updated = OMDb_UpdateSQL(obj);  // try to update record first
                if (!Updated) { Updated = OMDb_InsertSQL(obj); }  // if unable to update, insert new record
                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool OMDb_UpdateIfPopulated(OMDb obj, string ID = "")
            {
                SqlConnection Conn = OMDb_Conn();
                string SQLcmd = "Update " + OMDb_TableName() + " SET ";
                if (obj.IMDbID != null) { SQLcmd = SQLcmd + " IMDbID = @IMDbID,"; }
                if (obj.Title != null) { SQLcmd = SQLcmd + " Title = @Title,"; }
                if (obj.Year != null) { SQLcmd = SQLcmd + " Year = @Year,"; }
                if (obj.Rating != null) { SQLcmd = SQLcmd + " Rating = @Rating,"; }
                if (obj.ReleaseDate != null) { SQLcmd = SQLcmd + " ReleaseDate = @ReleaseDate,"; }
                if (obj.RunTime != null) { SQLcmd = SQLcmd + " RunTime = @RunTime,"; }
                if (obj.Genre != null) { SQLcmd = SQLcmd + " Genre = @Genre,"; }
                if (obj.Directors != null) { SQLcmd = SQLcmd + " Directors = @Directors,"; }
                if (obj.Writers != null) { SQLcmd = SQLcmd + " Writers = @Writers,"; }
                if (obj.Cast != null) { SQLcmd = SQLcmd + " Cast = @Cast,"; }
                if (obj.Plot != null) { SQLcmd = SQLcmd + " Plot = @Plot,"; }
                if (obj.Languages != null) { SQLcmd = SQLcmd + " Languages = @Languages,"; }
                if (obj.Countries != null) { SQLcmd = SQLcmd + " Countries = @Countries,"; }
                if (obj.Awards != null) { SQLcmd = SQLcmd + " Awards = @Awards,"; }
                if (obj.Poster != null) { SQLcmd = SQLcmd + " Poster = @Poster,"; }
                if (obj.MpaaRating != null) { SQLcmd = SQLcmd + " MpaaRating = @MpaaRating,"; }
                if (obj.Votes != null) { SQLcmd = SQLcmd + " Votes = @Votes,"; }
                if (obj.Type != null) { SQLcmd = SQLcmd + " Type = @Type,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ID = @ID";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.IMDbID != null) { cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID); }
                if (obj.Title != null) { cmd2.Parameters.AddWithValue(@"Title", obj.Title); }
                if (obj.Year != null) { cmd2.Parameters.AddWithValue(@"Year", obj.Year); }
                if (obj.Rating != null) { cmd2.Parameters.AddWithValue(@"Rating", obj.Rating); }
                if (obj.ReleaseDate != null) { cmd2.Parameters.AddWithValue(@"ReleaseDate", obj.ReleaseDate); }
                if (obj.RunTime != null) { cmd2.Parameters.AddWithValue(@"RunTime", obj.RunTime); }
                if (obj.Genre != null) { cmd2.Parameters.AddWithValue(@"Genre", obj.Genre); }
                if (obj.Directors != null) { cmd2.Parameters.AddWithValue(@"Directors", obj.Directors); }
                if (obj.Writers != null) { cmd2.Parameters.AddWithValue(@"Writers", obj.Writers); }
                if (obj.Cast != null) { cmd2.Parameters.AddWithValue(@"Cast", obj.Cast); }
                if (obj.Plot != null) { cmd2.Parameters.AddWithValue(@"Plot", obj.Plot); }
                if (obj.Languages != null) { cmd2.Parameters.AddWithValue(@"Languages", obj.Languages); }
                if (obj.Countries != null) { cmd2.Parameters.AddWithValue(@"Countries", obj.Countries); }
                if (obj.Awards != null) { cmd2.Parameters.AddWithValue(@"Awards", obj.Awards); }
                if (obj.Poster != null) { cmd2.Parameters.AddWithValue(@"Poster", obj.Poster); }
                if (obj.MpaaRating != null) { cmd2.Parameters.AddWithValue(@"MpaaRating", obj.MpaaRating); }
                if (obj.Votes != null) { cmd2.Parameters.AddWithValue(@"Votes", obj.Votes); }
                if (obj.Type != null) { cmd2.Parameters.AddWithValue(@"Type", obj.Type); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public OMDb OMDb_ReturnSQL(string ID = "")
            {
                _Database.SQL sql = new _Database.SQL();
                SqlConnection Conn = OMDb_Conn();
                string SelectLine = "Select [IMDbID],[Title],[Year],[Rating],[ReleaseDate],[RunTime],[Genre],[Directors],[Writers],[Cast],[Plot],[Languages],[Countries],[Awards],[Poster],[MpaaRating],[Votes],[Type] From " + OMDb_TableName() + " WHERE ID = '" + ID + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                OMDb returnObject = new OMDb();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.Title = ret["Title"].ToString();
                        returnObject.Year = ret["Year"].ToString();
                        returnObject.Rating = ret["Rating"].ToString();
                        returnObject.ReleaseDate = ret["ReleaseDate"].ToString();
                        returnObject.RunTime = ret["RunTime"].ToString();
                        returnObject.Genre = ret["Genre"].ToString();
                        returnObject.Directors = ret["Directors"].ToString();
                        returnObject.Writers = ret["Writers"].ToString();
                        returnObject.Cast = ret["Cast"].ToString();
                        returnObject.Plot = ret["Plot"].ToString();
                        returnObject.Languages = ret["Languages"].ToString();
                        returnObject.Countries = ret["Countries"].ToString();
                        returnObject.Awards = ret["Awards"].ToString();
                        returnObject.Poster = ret["Poster"].ToString();
                        returnObject.MpaaRating = ret["MpaaRating"].ToString();
                        returnObject.Votes = ret["Votes"].ToString();
                        returnObject.Type = ret["Type"].ToString();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public List<OMDb> OMDb_ReturnSQLList(string Command)
            {
                _Database.SQL sql = new _Database.SQL();
                SqlConnection Conn = OMDb_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<OMDb> ReturnList = new List<OMDb>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        OMDb returnObject = new OMDb();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.Title = ret["Title"].ToString();
                        returnObject.Year = ret["Year"].ToString();
                        returnObject.Rating = ret["Rating"].ToString();
                        returnObject.ReleaseDate = ret["ReleaseDate"].ToString();
                        returnObject.RunTime = ret["RunTime"].ToString();
                        returnObject.Genre = ret["Genre"].ToString();
                        returnObject.Directors = ret["Directors"].ToString();
                        returnObject.Writers = ret["Writers"].ToString();
                        returnObject.Cast = ret["Cast"].ToString();
                        returnObject.Plot = ret["Plot"].ToString();
                        returnObject.Languages = ret["Languages"].ToString();
                        returnObject.Countries = ret["Countries"].ToString();
                        returnObject.Awards = ret["Awards"].ToString();
                        returnObject.Poster = ret["Poster"].ToString();
                        returnObject.MpaaRating = ret["MpaaRating"].ToString();
                        returnObject.Votes = ret["Votes"].ToString();
                        returnObject.Type = ret["Type"].ToString();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }


            #endregion

            #endregion


            public void SQLServer_ToLocalSQL()
            {
                List<OMDb> movies = OMDb_ReturnSQLList("select * FROM [lucidmedia].[lucidmethod].[OMDb]");

                ahk.MsgBox("Starting - " + movies.Count + " movies found");

                foreach (OMDb movie in movies)
                {
                    OMDb_InsertSQL(movie);
                }

                ahk.MsgBox("DONE!");
            }



            #endregion


            public void WriteIMDbFileBasic(string imdbID, string saveDir)
            {
                string line = "[InternetShortcut]\nURL=https://www.imdb.com/title/" + imdbID + "/";
                ahk.FileAppend(line, saveDir + "\\IMDb.url");
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sharpAHK;
using sharpAHK_Dev;
using AHKExpressions;
using TelerikExpressions;
using System.Configuration;
using Telerik.WinControls.UI;
using System.Text.RegularExpressions;
using System.Drawing;
using System.IO;
using static sharpAHK_Dev._Sites.EpGuides;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;
using System.Threading;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Web.Script.Serialization;
using System.Web;
using System.Data.SQLite;
using static sharpAHK_Dev._WinServices;

namespace sharpAHK_Dev
{
    public partial class _Sites
    {

        public class Imdb
        {
            public string LITMShowIndexTable = "[lucidmedia].[dbo].[LITM_ShowIndex]";
            //public string LITMShowIndexTable = "[lucidmedia].[lucidmethod].[LITM_ShowIndex]";

            public string IMDbMovieTable = "[LucidMedia].[dbo].[LITM_IMDb]";
            //public string IMDbMovieTable = "[lucidmedia].[lucidmethod].[LITM_IMDb_New]";

            public string IMDbEpisodeTable = "[LucidMedia].[dbo].[IMDb_Episodes]";
            //public string IMDbEpisodeTable = "[LucidMedia].[lucidmethod].[IMDb_Episodes]";

            #region === StartUp ===

            
            
            
            _AHK ahk = new _AHK();
            
            
            
            

            // setup global static object
            public static class imdbGui
            {
                // defined in main form to populate statusbar here
                public static StatusBar mainStatusBar { get; set; }
                public static StatusBarPanel statusPanel { get; set; }
                public static StatusBarPanel datetimePanel { get; set; }
                public static Form fromForm { get; set; }

            }

            public void StatusBar(string Text, int section = 1)
            {
                if (section == 1)
                {
                    try
                    {
                        MethodInvoker inv = delegate { if (_StatusBar.SBGui.thisStatusPanel != null) { _StatusBar.SBGui.thisStatusPanel.Text = Text; } };
                        _StatusBar.SBGui.thisForm.Invoke(inv);
                    }
                    catch
                    {
                        _StatusBar.SBGui.thisStatusPanel.Text = Text;
                    }
                }

                if (section == 2)
                {
                    try
                    {
                        MethodInvoker inv = delegate { _StatusBar.SBGui.thisDatePanel.Text = Text; };
                        _StatusBar.SBGui.thisForm.Invoke(inv);
                    }
                    catch
                    {
                        _StatusBar.SBGui.thisDatePanel.Text = Text;
                    }
                }

            }


            #endregion


            #region === IMDb RegEx Constants (TO USE) ===

            // source: https://searchcode.com/file/901627/Classes/Scraper.vb#l-278

            //    'Public Const REGEX_TAGLINE             = ">Tagline.*?:</h4>[ \t\r\n]+(?<tagline>.*?)[ \t\r\n]+<span"
            //    'Const REGEX_MOVIE_TITLE_PATTERN        = "<h1 class=""header"" itemprop=""name"">(.*?)<span class=""nobr"">"
            //    'Const REGEX_MOVIE_YEAR_PATTERN         = "<img alt="".*?\((.*?)\).*?"" title="""
            //    Public Const REGEX_HREF_PATTERN         = "<a.*?href=[""'](?<url>.*?)[""'].*?>(?<name>.*?)</a>"
            //    Public Const REGEX_RELEASE_DATE         = ">Release Date:</h4>(?<date>.*?)<span"
            //    Public Const REGEX_STARS                = "Stars:</h4>(.*?)</div>"
            //    Public Const REGEX_TITLE_AND_YEAR       = "<title>(.*?)</title>"
            //    Public Const REGEX_TITLE                = "<title>(.*?) \("
            //    Public Const REGEX_YEAR                 = "\(.*?(\d{4}).*?\)" 
            //    Public Const REGEX_NAME                 = "itemprop=""name"">(?<name>.*?)</span>"                '"<span.*?>(?<name>.*?)</span>"
            //    Public Const REGEX_STUDIO               = "<h4 class=""inline"">Production.*?/h4>(.*?)</div>"
            //    Public Const REGEX_CREDITS              = "<h4 class=""inline"">Writers?:</h4>(.*?)</div>"
            //    Public Const REGEX_ORIGINAL_TITLE       = "<span class=""title-extra"" itemprop=""name"">(.*?)<i>\(original title\)</i>"
            //    Public Const REGEX_OUTLINE              = "itemprop=""description"">(?<outline>.*?)<div"
            //    Public Const REGEX_ACTORS_TABLE         = "<table class=""cast_list"">(.*?)</table>"
            //    Public Const REGEX_TR                   = "<tr.*?>(.*?)</tr>"
            //    Public Const REGEX_ACTOR_NO_IMAGE       = "<td.*?>.*?<a href=""/name/nm(?<actorid>.*?)/.*?_=ttfc_fc_cl_i(?<order>.*?)"".*?title=""(?<actorname>.*?)"".*?=""character"".*?<div>(?<actorrole>.*?)</div>"
            //    Public Const REGEX_ACTOR_WITH_IMAGE     = "<td.*?>.*?<a href=""/name/nm(?<actorid>.*?)/.*?_=ttfc_fc_cl_i(?<order>.*?)"".*?title=""(?<actorname>.*?)"".*?loadlate=""(?<actorthumb>.*?)"".*?=""character"".*?<div>(?<actorrole>.*?)</div>"
            //    Public Const REGEX_IMDB_KEYWORD         = "ttkw_kw_[0-9]*"">(?<keyword>.*?)<\/a"
            //    Public Const REGEX_IMDB_TRAILER         = "<h2><a href=""/video/imdb/vi(.*?)/"
            //    Public Const REGEX_TOP_250              = "=tt_awd""> Top Rated Movies #(.*?)</a>"    '"<strong>Top 250 Movies #(.*?)</strong></a>"
            //    Public Const REGEX_VOTES                = "itemprop=""ratingCount"">([\d{1,3},.?\s]*[0-9]?)</span>"
            //    Public Const REGEX_TAGLINE              = "<h4 class=""inline"">Tagline.*?:</h4>(.*?)<"
            //    Public Const REGEX_RUNTIME              = "<h4 class=""inline"">Runtime:</h4>(.*?)</div>"
            //    Public Const REGEX_DURATION             = "<time itemprop=""duration"" datetime="".*?"">(.*?) min</time>"
            //    Public Const REGEX_COUNTRYS             = "class=""inline"">Countr(.*?)</div>"
            //    Public Const REGEX_COUNTRY              = "itemprop='url'>(.*?)</a>"
            //    Public Const REGEX_RATING               = "<span itemprop=""ratingValue"">(.*?)</span>"
            //    Public Const REGEX_RUNTIMETECH          = "class=""label""> Runtime <\/td>(.*?)<\/tr>"
            //    Public Const REGEX_DURATIONTECH         = "\((.*?) min"
            //    Public Const REGEX_DURATIONTECH2        = "(\d*?) min"
            //    Public Const REGEX_ASPECTRATIO          = "class=""inline"">Aspect Ratio:</h4>(.*?)</div>"
            //    Public Const REGEX_ASPECTRATIOALL       = "class=""label""> Aspect Ratio </td>(.*?)</td>"
            //End Class

            #endregion

            //##### SHOWS  ########################

            #region === Objects / Vars / Controls ===


            public static class imdbDisp
            {
                public static TextBox txtDescription { get; set; }
                public static TextBox txtRating { get; set; }
                public static TextBox txtVotes { get; set; }
                public static TextBox txtTitle { get; set; }
                public static TextBox txtSeason { get; set; }
                public static TextBox txtCategory { get; set; }
                public static DataGridView episodeGrid { get; set; }
            }



            private String temp1 { get; set; }
            private String temp2 { get; set; }


            public struct IMDbTVShow
            {
                public string ShowName { get; set; }
                public string IMDbID { get; set; }


                public string Description { get; set; }
                public string Rating { get; set; }
                public string Votes { get; set; }
                public string Category { get; set; }

                public int SeasonsCount { get; set; }

                public List<IMDbTVSeason> Seasons { get; set; }
            }

            public struct IMDbTVSeason
            {
                public string ShowName { get; set; }
                public string IMDbID { get; set; }

                public string ShowDescription { get; set; }
                public int SeasonNum { get; set; }

                public string ShowRating { get; set; }
                public string ShowVotes { get; set; }
                public string ShowCategory { get; set; }

                public List<IMDbTVEpisode> Episodes { get; set; }
            }

            public struct IMDbTVEpisode
            {
                public string ShowName { get; set; }
                public string IMDbID { get; set; }
                public int SeasonNum { get; set; }

                public string EpNum { get; set; }
                public string EpName { get; set; }
                public string AirDate { get; set; }
                public string Images { get; set; }
                public string Description { get; set; }
            }

            //// store episodes in datatable to display after parsing
            //DataTable table = new DataTable();


            #endregion

            #region === IMDb TVShowParse ===

            public void IMDb_ShowDescription(string IMDbId)
            {
                IMDbTVShow show = new IMDbTVShow();

                show.Seasons = new List<IMDbTVSeason>();
                show.IMDbID = IMDbId;

                IMDbTVSeason season = new IMDbTVSeason();

                string description = "";
                string category = "";
                string votes = "";
                string rating = "";
                string showName = "";

                season = ParseIMDbShow(IMDbId, 0, false);  // extract all episodes from season 

                if (season.Episodes == null) { return; }

                if (season.Episodes.Count > 0)
                {
                    description = season.ShowDescription;
                    category = season.ShowCategory;
                    votes = season.ShowVotes;
                    rating = season.ShowRating;
                    showName = season.ShowName;
                }

                var encoded = HttpUtility.HtmlEncode(description);

                string cmd = "Update " + LITMShowIndexTable + " set [IMDbRating] = '" + rating + "', [Category] = '" + category + "', [Description] = '" + encoded + "', [IMDbVotes] = '" + votes + "' WHERE [IMDbURL] = '" + IMDbId + "'";

                bool updated = sql.WriteDataRecord(ImdbEps_Conn(), cmd);

                if (!updated) { ahk.MsgBox("Error Updating\n\n" + cmd); }

                //ahk.MsgBox(showName + "\n\n" + description);
            }

            // Lookup IMDb Show dEscription - write to database
            public void Write_IMDb_ShowDescriptionLoop()
            {
                string cmd = "select IMDbURL FROM " + LITMShowIndexTable + " WHERE IMDbURL != '' AND (Description = '' or Description is null)";

                _Lists lst = new _Lists();
                List<string> imdbIds = lst.SQL_To_List(ImdbEps_Conn(), cmd);

                int j = 0;
                foreach (string IMDbID in imdbIds)
                {
                    j++;

                    StatusBar(j + "/" + imdbIds.Count(), 2);

                    if (j < 45) { continue; }  // temp fix ! 


                    IMDb_ShowDescription(IMDbID);
                }

                StatusBar("FINISHED Show Description Update");

            }


            public void ParseIMDbShowLoop(string IMDbId, int seasonNum = 0, DataGridView episodeGrid = null)
            {
                if (episodeGrid != null)
                {
                    _GridControl grid = new _GridControl();
                    grid.GridAddColumn(episodeGrid, "EpName");
                    grid.GridAddColumn(episodeGrid, "EpNum");
                    grid.GridAddColumn(episodeGrid, "Description");
                    grid.GridAddColumn(episodeGrid, "AirDate");
                    grid.GridAddColumn(episodeGrid, "CoverImg");
                }

                bool addToGrid = false;
                if (episodeGrid != null) { addToGrid = true; }


                IMDbTVShow show = new IMDbTVShow();

                show.Seasons = new List<IMDbTVSeason>();
                show.IMDbID = IMDbId;

                IMDbTVSeason season = new IMDbTVSeason();

                string lastEpNum = "";
                bool done = false;
                int i = 1;
                do
                {
                    season = ParseIMDbShow(IMDbId, i, addToGrid);  // extract all episodes from season 

                    StatusBar("Parsing " + season.ShowName + " Season " + i);

                    // remove after testing !!!
                    // if (i == 2) { break; }

                    if (season.Episodes.Count > 0)
                    {
                        // add season to show object + show details
                        show.Seasons.Add(season);
                        show.Description = season.ShowDescription;
                        show.Category = season.ShowCategory;
                        show.Votes = season.ShowVotes;
                        show.Rating = season.ShowRating;
                        show.ShowName = season.ShowName;

                        // check to see if last used episode number matches current season, if so, break b/c looping on same season num
                        foreach (IMDbTVEpisode lastep in season.Episodes)
                        {
                            if (lastEpNum == lastep.EpNum)
                            {
                                done = true;
                            }
                        }

                        // store last used episode number 
                        if (!done) { foreach (IMDbTVEpisode lastep in season.Episodes) { lastEpNum = lastep.EpNum; } }

                    }
                    else
                    {
                        done = true;
                    }

                    i++;
                } while (done == false);

                show.SeasonsCount = show.Seasons.Count();

                if (addToGrid) { _GridControl grid = new _GridControl(); grid.Fill_Column_Width(episodeGrid); }

                StatusBar("Finished Parsing " + season.ShowName);
            }


            public void ParseIMDbShowLoop_AllShows(string IMDbId, int seasonNum = 0, DataGridView episodeGrid = null)
            {
                if (episodeGrid != null)
                {
                    _GridControl grid = new _GridControl();
                    grid.GridAddColumn(episodeGrid, "EpName");
                    grid.GridAddColumn(episodeGrid, "EpNum");
                    grid.GridAddColumn(episodeGrid, "Description");
                    grid.GridAddColumn(episodeGrid, "AirDate");
                    grid.GridAddColumn(episodeGrid, "CoverImg");
                }

                bool addToGrid = false;
                if (episodeGrid != null) { addToGrid = true; }


                string cmd = "select IMDbURL FROM " + LITMShowIndexTable + " WHERE IMDbURL != ''";

                //List<string> imdbIds = new List<string>();

                _Lists lst = new _Lists();
                List<string> imdbIds = lst.SQL_To_List(ImdbEps_Conn(), cmd);

                int j = 0;
                foreach (string IMDbID in imdbIds)
                {
                    j++; StatusBar(j + "/" + imdbIds.Count(), 2);

                    IMDbTVShow show = new IMDbTVShow();

                    show.Seasons = new List<IMDbTVSeason>();
                    show.IMDbID = IMDbId;

                    IMDbTVSeason season = new IMDbTVSeason();

                    string lastEpNum = "";
                    bool done = false;
                    int i = 1;
                    do
                    {
                        season = ParseIMDbShow(IMDbID, i, addToGrid);  // extract all episodes from season 

                        StatusBar("Parsing " + season.ShowName + " Season " + i);

                        // remove after testing !!!
                        // if (i == 2) { break; }

                        if (season.Episodes.Count > 0)
                        {
                            // add season to show object + show details
                            show.Seasons.Add(season);
                            show.Description = season.ShowDescription;
                            show.Category = season.ShowCategory;
                            show.Votes = season.ShowVotes;
                            show.Rating = season.ShowRating;
                            show.ShowName = season.ShowName;

                            // check to see if last used episode number matches current season, if so, break b/c looping on same season num
                            foreach (IMDbTVEpisode lastep in season.Episodes)
                            {
                                if (lastEpNum == lastep.EpNum)
                                {
                                    done = true;
                                }
                            }

                            // store last used episode number 
                            if (!done) { foreach (IMDbTVEpisode lastep in season.Episodes) { lastEpNum = lastep.EpNum; } }

                        }
                        else
                        {
                            done = true;
                        }

                        i++;
                    } while (done == false);

                    show.SeasonsCount = show.Seasons.Count();

                    if (addToGrid) { _GridControl grid = new _GridControl(); grid.Fill_Column_Width(episodeGrid); }

                    StatusBar("Finished Parsing " + season.ShowName);
                }


            }

            public IMDbTVSeason ParseIMDbShow(string IMDbID, int seasonNum, bool AddToGrid = true, bool WriteToSQL = false, bool WriteToSQLite = true, string ShowRoot = "")
            {
                IMDbTVSeason season = new IMDbTVSeason();
                season.IMDbID = IMDbID;
                season.SeasonNum = seasonNum;

                List<string> EpisodeNum = new List<string>();
                List<string> EpisodeNames = new List<string>();
                List<string> EpisodeDescriptions = new List<string>();
                List<string> EpisodeAirDates = new List<string>();
                List<string> EpisodeCovers = new List<string>();
                string Title = "";
                string Description = "";
                string Rating = "";
                string Votes = "";
                string SeasonNum = "";
                string Category = "";


                String[,]
                data = new String[7, 150];

                WebClient web = new WebClient();

                String html = null;

                try
                {
                    html = web.DownloadString("http://www.imdb.com/title/" + IMDbID);
                }
                catch (Exception ex)
                {
                    ahk.MsgBox(ex.ToString());
                    return season;
                }


                Match title = Regex.Match(html, @"originalTitle"">\s*(.*?)\s*<span", RegexOptions.Singleline);
                if (title.Length <= 4)
                {
                    title = Regex.Match(html, @"<h1 itemprop=""name"" class="""">(.*?)&nbsp;", RegexOptions.Singleline);
                }

                String TitleTXT = title.Groups[1].ToString();
                temp1 = TitleTXT;
                data[0, 0] = TitleTXT;
                Title = title.Groups[1].ToString();

                Match description = Regex.Match(html, @"description"" content=""(.*?)""", RegexOptions.Multiline);
                String DescriptionTXT = @description.Groups[1].ToString();
                data[1, 0] = DescriptionTXT;
                Description = DescriptionTXT;

                Match rate = Regex.Match(html, @"<strong title=""(.*?) based on", RegexOptions.Multiline);
                String RateTXT = rate.Groups[1].ToString();
                data[2, 0] = RateTXT;
                Rating = RateTXT;

                Match votes = Regex.Match(html, @"based on (.*?)user ratings", RegexOptions.Multiline);
                String VotesTXT = votes.Groups[1].ToString();
                data[3, 0] = VotesTXT;
                Votes = VotesTXT;


                UpdateText(_Sites.Imdb.imdbDisp.txtDescription, Description);
                UpdateText(_Sites.Imdb.imdbDisp.txtRating, Rating);
                UpdateText(_Sites.Imdb.imdbDisp.txtDescription, Description);
                UpdateText(_Sites.Imdb.imdbDisp.txtVotes, Votes);
                UpdateText(_Sites.Imdb.imdbDisp.txtTitle, Title);

                //ahk._MsgBox("Title: " + Title + "\n\nDescription = " + Description + "\n\nRating = " + Rating + "\nVotes = " + Votes); 

                //============================
                // Show Posters
                //============================

                bool downloadPosters = false;  // poster download errors

                if (downloadPosters)
                {
                    Match poster = Regex.Match(html, @"<div class=""poster"">\n<a href=""(.*?)""\n(.*?)\nsrc=""(.*?)""", RegexOptions.Multiline);
                    //Console.WriteLine(poster.Groups[1]);

                    string postergroups = poster.Groups[1].ToString();
                    string posterurl = "http://www.imdb.com" + postergroups;

                    posterurl = "http://www.imdb.com/title/tt0106179/mediaviewer/rm73865472";

                    string posterhtml2 = ParseIMDbShow_DownloadHTML(posterurl);


                    String posterhtml = web.DownloadString(posterurl);
                    //Match poster2 = Regex.Match(posterhtml, @"cursor-default(.*?)src=""(.*?)""", RegexOptions.Multiline);

                    Match poster2 = Regex.Match(html, @"cursor-default(.*?)src=""(.*?)""", RegexOptions.Multiline);
                    string postergroups2 = poster2.Groups[1].ToString();

                    //// SAVE IMAGE 
                    //string subPath = @"D:\\" + code;
                    //bool exists = System.IO.Directory.Exists(subPath);
                    //if (!exists) { System.IO.Directory.CreateDirectory(subPath); }
                    //web.DownloadFile(poster2.Groups[2].Value, @"D:\\" + code + "\\poster.jpg");
                    //ResizeImage ri = new ResizeImage();
                    //Image img = new Bitmap(@"D:\\" + code + "\\poster.jpg");

                    //Stream newstream = ri.resize(ri.ToStream(img, ImageFormat.Jpeg));
                    //Image newImg = System.Drawing.Image.FromStream(newstream);

                    //newImg.Save(@"D:\\" + code + "\\poster2.jpg", ImageFormat.Jpeg);


                    String PosterTXT = poster2.Groups[2].ToString();
                    data[4, 0] = PosterTXT;
                }


                Match cat = Regex.Match(html, @"><span class=""itemprop"" itemprop=""genre"">(.*?)</span>(.*?)\n(.*?)\n><span class=""itemprop"" itemprop=""genre"">(.*?)</span>", RegexOptions.Multiline);
                if (cat.Length <= 4)
                {
                    cat = Regex.Match(html, @"><span class=""itemprop"" itemprop=""genre"">(.*?)</span>", RegexOptions.Multiline);
                }

                String CatTXT = cat.Groups[1].ToString();
                data[5, 0] = CatTXT;

                Category = CatTXT;

                UpdateText(_Sites.Imdb.imdbDisp.txtCategory, Category); // update textbox while running on thread

                String htmlS = null;

                try
                {
                    htmlS = web.DownloadString("http://www.imdb.com/title/" + IMDbID + "/episodes?season=" + seasonNum);
                }
                catch (Exception ex)
                {
                    //ahk.MsgBox(ex.ToString());
                    return season;
                }


                Match actualSeasone = Regex.Match(htmlS, @"nbsp;<strong>Season (.*?)</strong>", RegexOptions.Multiline);
                String ActualSeasoneTXT = actualSeasone.Groups[1].ToString();
                temp2 = ActualSeasoneTXT;
                data[6, 0] = ActualSeasoneTXT;
                int t = 0;

                SeasonNum = ActualSeasoneTXT;
                UpdateText(_Sites.Imdb.imdbDisp.txtSeason, SeasonNum); // update textbox while running on thread


                MatchCollection episodesTitle = Regex.Matches(htmlS, @" itemprop=""name"">(.*?)</a></strong>", RegexOptions.Multiline);
                int espidoesTitleCount = episodesTitle.Count;
                foreach (Match m in episodesTitle)
                {
                    try
                    {
                        data[0, t] = m.Groups[1].Value;
                        EpisodeNames.Add(m.Groups[1].Value);
                    }
                    catch
                    {

                    }

                    t++;
                }

                t = 0;
                MatchCollection episodesDescriptions = Regex.Matches(htmlS, @" <div class=""item_description"" itemprop=""description"">\n(.*?)</div>", RegexOptions.Multiline);
                int episodesDescriptionsCount = episodesDescriptions.Count;
                foreach (Match m in episodesDescriptions)
                {
                    try
                    {
                        data[0, t] = m.Groups[1].Value;
                        EpisodeDescriptions.Add(m.Groups[1].Value);
                        t++;
                    }
                    catch (Exception ex)
                    {
                        ahk.MsgBox(ex.ToString());
                        //data[0, t] = m.Groups[1].Value;
                        //EpisodeDescriptions.Add(m.Groups[1].Value);
                    }

                }

                t = 0;
                MatchCollection episodesAir = Regex.Matches(htmlS, @"<div class=""airdate"">\n(.*?)\n", RegexOptions.Multiline);
                int episodesAirCount = episodesAir.Count;
                foreach (Match m in episodesAir)
                {
                    DateTime myDate = new DateTime(2016);
                    DateTime value;
                    if (DateTime.TryParse(m.Groups[1].Value, out value))
                    {
                        myDate = DateTime.Parse(value.ToString());
                    }
                    else

                        data[0, t] = m.Groups[1].Value;


                    string date = m.Groups[1].Value.Trim();
                    EpisodeAirDates.Add(date);

                    t++;
                }


                int ss = 0; t = 0;
                MatchCollection episodesCover = Regex.Matches(htmlS, @"<img width=""200"" height=""112"" class=""zero-z-index"" alt=""(.*?)"" src=""(.*?)"">\n<div>S(.*?), Ep(.*?)</div>", RegexOptions.Multiline);
                int episodesCoverCount = episodesCover.Count;
                foreach (Match m in episodesCover)
                {
                    int s = Int32.Parse(m.Groups[3].Value);
                    int e = Int32.Parse(m.Groups[4].Value);
                    string sTXT;
                    string eTXT;
                    if (s < 10)
                    {
                        sTXT = "S0" + s;
                    }
                    else
                    {
                        sTXT = "S" + s;
                    }
                    if (e < 10)
                    {
                        eTXT = "E0" + e;
                    }
                    else
                    {
                        eTXT = "E" + e;
                    }

                    //    //Console.WriteLine(sTXT+"    "+eTXT);


                    try
                    {
                        data[0, t] = m.Groups[2].Value;

                        EpisodeCovers.Add(m.Groups[2].Value);

                        web.DownloadFile(m.Groups[2].Value, @"D:\\" + IMDbID + "\\" + sTXT + "" + eTXT + ".jpg");
                    }
                    catch
                    { }


                    t++;
                    try
                    {
                        data[0, t] = sTXT + "" + eTXT;

                        string EpNum = sTXT + "" + eTXT;
                        EpisodeNum.Add(EpNum);
                    }
                    catch
                    {

                    }



                }


                // need to add new matching criteria for episode numbers when there isn't an image found for episode

                //t = 0;
                //MatchCollection epNumsMatch = Regex.Matches(htmlS, @"<div>S class=""zero-z-index"" alt=""(.*?)"" src=""(.*?)"">\n<div>S(.*?), Ep(.*?)</div>", RegexOptions.Multiline);
                //int epNumsMatchCount = epNumsMatch.Count;
                //foreach (Match m in epNumsMatch)
                //{
                //    t++;
                //    try
                //    {
                //        data[0, t] = sTXT + "" + eTXT;

                //        string EpNum = sTXT + "" + eTXT;
                //        EpisodeNum.Add(EpNum);
                //    }
                //    catch
                //    {

                //    }
                //}


                //</a> <div>S2, Ep41</div>  -- season/ep WITHOUT image for episode
                //<div>S2, Ep42</div>     -- season/ep WITH image for episode

                //==========================
                // Episode Guide Table
                //==========================

                //bool DisplayDataTable = false;



                int i = 0; t = 0;

                List<IMDbTVEpisode> eps = new List<IMDbTVEpisode>();

                string seasonnum = "0";

                foreach (string ep in EpisodeNum)
                {
                    IMDbTVEpisode episode = new IMDbTVEpisode();

                    _Lists lst = new _Lists();
                    string epname = lst.Return_List_Value(EpisodeNames, i);
                    string epdescript = lst.Return_List_Value(EpisodeDescriptions, i);
                    string epdate = lst.Return_List_Value(EpisodeAirDates, i).Trim();
                    string epcover = lst.Return_List_Value(EpisodeCovers, i);

                    if (epdate == "2018") { epdate = "01/01/2018"; }

                    string writeDate = epdate;
                    try
                    {
                        DateTime EpDate = Convert.ToDateTime(epdate);
                        episode.AirDate = EpDate.ToShortDateString();
                        writeDate = episode.AirDate;
                    }
                    catch
                    {
                        episode.AirDate = epdate;
                        writeDate = epdate;
                    }


                    episode.Description = epdescript;
                    episode.EpName = epname;
                    episode.ShowName = Title;
                    episode.IMDbID = IMDbID;
                    episode.SeasonNum = seasonNum;
                    episode.EpNum = ep;
                    eps.Add(episode); // add to episode list

                    if (seasonnum == "0")
                    {
                        seasonnum = episode.EpNum;
                        if (seasonnum.Contains("E"))
                        {
                            seasonnum = ahk.StringSplit(seasonnum, "E", 0);
                        }
                    }
                    

                    bool DownloadEpImages = true;
                    string saveFile = "";

                    if (DownloadEpImages)
                    {
                        string localDir = ahk.AppDir() + "\\EpisodeImages\\" + IMDbID;
                        ahk.FileCreateDir(localDir);

                        saveFile = localDir + "\\" + ep + ".jpg";

                        if (!File.Exists(saveFile))
                        {
                            web.DownloadFile(epcover, localDir + "\\" + ep + ".jpg");
                        }

                        if (!File.Exists(saveFile))
                        {
                            saveFile = @"C:\_Code\LucidProjects\ADBindex\IMDbDisp\blankImage.jpg";
                        }
                    }


                    ImdbEps obj = new ImdbEps();
                    obj.IMDbID = episode.IMDbID;
                    obj.AirDate = episode.AirDate;
                    obj.EpisodeName = episode.EpName.Encode();
                    obj.EpNum = episode.EpNum;
                    obj.ShowName = episode.ShowName.Encode();
                    obj.ShowImage = saveFile;
                    obj.EpDescription = episode.Description.Encode();

                    if (WriteToSQL)
                    {
                        //ImdbEps_UpdateInsert(obj);

                        bool inserted = ImdbEps_InsertSQL(obj);
                    }

                    if (WriteToSQLite)
                    {
                        string saveDir = "";
                        if (ShowRoot != "") { saveDir = ShowRoot; }
                        else { saveDir = ahk.AppDir() + "\\Db"; ahk.FileCreateDir(saveDir); }
                        
                        obj.ShowName = obj.ShowName.Decode();
                        obj.ShowName = obj.ShowName.Replace(":", "-");
                        obj.ShowName = obj.ShowName.Replace("'", "");

                        //if (obj.ShowName == "")
                        //{
                        //    string prop = "";
                        //}

                        ImdbEps_UpdateInsert(saveDir + "\\" + obj.ShowName + ".sqlite", obj);
                    }


                    //table.Rows.Add(ep, epname, epdescript, EpDate.ToShortDateString(), epcover);

                    if (AddToGrid)
                    {
                        // Add row to DataGridView (from any thread)
                        if (_Sites.Imdb.imdbDisp.episodeGrid.InvokeRequired) { _Sites.Imdb.imdbDisp.episodeGrid.BeginInvoke((MethodInvoker)delegate () { _Sites.Imdb.imdbDisp.episodeGrid.Rows.Add(ep, epname, epdescript, writeDate, epcover); }); }
                        else { _Sites.Imdb.imdbDisp.episodeGrid.Rows.Add(ep, epname, epdescript, writeDate, epcover); }

                        _GridControl grid = new _GridControl();
                        grid.Grid_Refresh(_Sites.Imdb.imdbDisp.episodeGrid);

                    }

                    i++;
                }

                season.ShowCategory = Category;
                season.ShowRating = Rating;
                season.ShowVotes = Votes;
                season.Episodes = eps;
                season.ShowName = Title;
                season.ShowDescription = Description;
                
                if (season.SeasonNum == 0) { season.SeasonNum = seasonnum.ToInt(); }

                return season;
            }

            public string ParseIMDbShow_DownloadHTML(string url, bool StatusBarDisp = true)
            {
                string htmlCode = "";

                using (WebClient client = new WebClient()) // WebClient class inherits IDisposable
                {
                    if (StatusBarDisp)
                    {
                        _StatusBar sb = new _StatusBar();
                        sb.StatusBar("Downloading " + url + " HTML...");
                    }

                    // Or you can get the file content without saving it:

                    try
                    {
                        htmlCode = client.DownloadString(url);
                    }
                    catch
                    {
                        if (StatusBarDisp) { _StatusBar sb = new _StatusBar(); sb.StatusBar("Error Downloading HTML from " + url); }
                        return "";
                    }
                }

                if (StatusBarDisp) { _StatusBar sb = new _StatusBar(); sb.StatusBar("Downloaded HTML From " + url); }

                return htmlCode;
            }

            private void UpdateText(TextBox tp, string text)
            {
                if (tp != null)
                {
                    // return text from control (from any thread)
                    if (tp.InvokeRequired) { tp.BeginInvoke((MethodInvoker)delegate () { tp.Text = text; }); }
                    else { tp.Text = text; }
                }
            }

            #endregion

            #region === IMDb Images ===


            /// <summary>
            /// Returns Path to Local IMDb Image FilePath from Title ID 
            /// </summary>
            /// <param name="TitleID">IMDb TitleID</param>
            /// <returns>Returns Local Path to IMDb Image by TitleID</returns>
            public string IMDb_ImageLookup(string TitleID)
            {
                // Root Folder with IMDb Title ID Subfolders Containg Images from IMDb.com
                string IMDbImgRoot = @"D:\_Images\_Imdb";

                string imgRoot = IMDbImgRoot + "\\" + TitleID;

                string defaultImg = imgRoot + "\\" + TitleID + ".jpg";
                if (File.Exists(defaultImg)) { return defaultImg; }

                string imgFull = imgRoot + "\\" + TitleID + "_Full.jpg";
                if (File.Exists(imgFull)) { return imgFull; }

                string imgLarge = imgRoot + "\\" + TitleID + "_Full.jpg";
                if (File.Exists(imgLarge)) { return imgLarge; }

                return "";
            }


            /// <summary>
            /// Search IMDb for Movie Poster, Download IMDb Poster to SaveDir
            /// </summary>
            /// <param name="ImdbID">IMDb Id to Lookup</param>
            /// <param name="saveDir">Location to save imaves (default = appdir\_Imdb)</param>
            /// <returns>Returns True if Poster(s) Downlaoded</returns>
            public bool IMDb_ImageDL(string ImdbID, string saveDir = "")
            {
                imdB movieInfo = parse_IMDb_Page(ImdbID, "", true, false);

                _Web web = new _Web();

                string posterLink = movieInfo.Poster;
                string posterLinkLarge = movieInfo.PosterLarge;
                string posterLinkFull = movieInfo.PosterFull;

                if (saveDir == "") { saveDir = ahk.AppDir() + "\\_Imdb"; }
                ahk.FileCreateDir(saveDir);

                bool downloaded = false;

                if (posterLink != "")
                {
                    string poster = saveDir + "\\" + movieInfo.IMDbID + ".jpg";
                    downloaded = web.DownloadFile(posterLink, poster, true);
                }

                if (posterLinkLarge != "")
                {
                    string poster = saveDir + "\\" + movieInfo.IMDbID + "_Large.jpg";
                    downloaded = web.DownloadFile(posterLinkLarge, poster, true);
                }

                if (posterLinkFull != "")
                {
                    string poster = saveDir + "\\" + movieInfo.IMDbID + "_Full.jpg";
                    downloaded = web.DownloadFile(posterLinkFull, poster, true);
                }

                return downloaded;
            }




            /// <summary>
            /// Return Path to Local IMDb Image Directory
            /// </summary>
            /// <param name="OpenDir">Option to Open IMDb Image Directory (Default = False)</param>
            /// <returns>Returns path to IMDb Image Dir</returns>
            public string IMDb_ImageDir(bool OpenDir = false)
            {
                string imgDir = @"D:\_Images\IMDb_Img";

                if (!Directory.Exists(imgDir)) { ahk.MsgBox(imgDir + " NOT FOUND"); return ""; }

                if (OpenDir) { ahk.OpenDir(imgDir); }
                return imgDir;
            }

            /// <summary>
            /// Returns FilePath to Highest Quality IMDb Image Saved in Local IMDb ImageDir
            /// </summary>
            /// <param name="IMDbID">IMDb Title ID to Return Saved Image Path</param>
            /// <returns></returns>
            public string IMDb_ImagePath_BestQuality(string IMDbID)
            {
                string ImgDirRoot = IMDb_ImageDir();

                string imgPathSmall = ImgDirRoot + "\\" + IMDbID + ".jpg";
                string imgPathMedium = ImgDirRoot + "\\" + IMDbID + "_Large.jpg";
                string imgPathLarge = ImgDirRoot + "\\" + IMDbID + "_Full.jpg";

                if (File.Exists(imgPathLarge)) { return imgPathLarge; }
                if (File.Exists(imgPathMedium)) { return imgPathMedium; }
                if (File.Exists(imgPathSmall)) { return imgPathSmall; }

                return "";
            }

            /// <summary>
            /// Returns FilePath to Lowest Quality / Smallest FileSize From Local IMDb Image Directory
            /// </summary>
            /// <param name="IMDbID">IMDb Title ID to Return Saved Image Path</param>
            /// <returns></returns>
            public string IMDb_ImagePath_Smallest(string IMDbID)
            {
                string ImgDirRoot = IMDb_ImageDir();

                string imgPathSmall = ImgDirRoot + "\\" + IMDbID + ".jpg";
                string imgPathMedium = ImgDirRoot + "\\" + IMDbID + "_Large.jpg";
                string imgPathLarge = ImgDirRoot + "\\" + IMDbID + "_Full.jpg";

                if (File.Exists(imgPathSmall)) { return imgPathSmall; }
                if (File.Exists(imgPathMedium)) { return imgPathMedium; }
                if (File.Exists(imgPathLarge)) { return imgPathLarge; }

                return "";
            }

            /// <summary>
            /// Checks Local IMDb ImageDir by IMDb ID To See if Image Saved For That ID
            /// </summary>
            /// <param name="IMDbID">IMDb ID To Check For</param>
            /// <returns></returns>
            public bool ImageSaved(string IMDbID)
            {
                string ImgDirRoot = IMDb_ImageDir();

                string imgPathSmall = ImgDirRoot + "\\" + IMDbID + ".jpg";
                string imgPathMedium = ImgDirRoot + "\\" + IMDbID + "_Large.jpg";
                string imgPathLarge = ImgDirRoot + "\\" + IMDbID + "_Full.jpg";

                if (File.Exists(imgPathSmall)) { return true; }
                if (File.Exists(imgPathMedium)) { return true; }
                if (File.Exists(imgPathLarge)) { return true; }

                return false;
            }



            #endregion

            #region === ImdbEps FUNCTIONS ===

            _Database.SQL sql = new _Database.SQL();
            _Database.SQLite sqlite = new _Database.SQLite();

            #region ===== ImdbEps Object =====

            public struct ImdbEps
            {
                public string ID { get; set; }
                public string EpGuideID { get; set; }
                public string IMDbID { get; set; }
                public string ShowName { get; set; }
                public string EpNum { get; set; }
                public string EpisodeName { get; set; }
                public string EpDescription { get; set; }
                public string LocalPath { get; set; }
                public string AirDate { get; set; }
                public string ShowImage { get; set; }
                public string Flag { get; set; }
            }
            public ImdbEps Return_ImdbEps(string ID = "", string EpGuideID = "", string IMDbID = "", string ShowName = "", string EpNum = "", string EpisodeName = "", string EpDescription = "", string LocalPath = "", string AirDate = "", string ShowImage = "", string Flag = "")
            {
                ImdbEps obj = new ImdbEps();
                obj.ID = ID;
                obj.EpGuideID = EpGuideID;
                obj.IMDbID = IMDbID;
                obj.ShowName = ShowName;
                obj.EpNum = EpNum;
                obj.EpisodeName = EpisodeName;
                obj.EpDescription = EpDescription;
                obj.LocalPath = LocalPath;
                obj.AirDate = AirDate;
                obj.ShowImage = ShowImage;
                obj.Flag = Flag;

                return obj;
            }

            //  Fix illegal characters before Sql/Sqlite Db Inserts
            public ImdbEps ImdbEps_FixChars(ImdbEps ToFix)
            {
                ImdbEps Fixed = new ImdbEps();

                Fixed.ID = ToFix.ID.Replace("'", "''");
                Fixed.EpGuideID = ToFix.EpGuideID.Replace("'", "''");
                Fixed.IMDbID = ToFix.IMDbID.Replace("'", "''");
                Fixed.ShowName = ToFix.ShowName.Replace("'", "''");
                Fixed.EpNum = ToFix.EpNum.Replace("'", "''");
                Fixed.EpisodeName = ToFix.EpisodeName.Replace("'", "''");
                Fixed.EpDescription = ToFix.EpDescription.Replace("'", "''");
                Fixed.LocalPath = ToFix.LocalPath.Replace("'", "''");
                Fixed.AirDate = ToFix.AirDate.Replace("'", "''");
                Fixed.ShowImage = ToFix.ShowImage.Replace("'", "''");
                Fixed.Flag = ToFix.Flag.Replace("'", "''");

                return Fixed;
            }

            // Compare two objects to see if they have identical values
            public bool ImdbEps_Changed(ImdbEps OldVal, ImdbEps NewVal)
            {
                ImdbEps diff = new ImdbEps();
                List<string> diffList = new List<string>();
                bool different = false;
                if (OldVal.ID == null) { OldVal.ID = ""; }
                if (NewVal.ID == null) { NewVal.ID = ""; }
                if (OldVal.ID != NewVal.ID) { different = true; }
                if (OldVal.EpGuideID == null) { OldVal.EpGuideID = ""; }
                if (NewVal.EpGuideID == null) { NewVal.EpGuideID = ""; }
                if (OldVal.EpGuideID != NewVal.EpGuideID) { different = true; }
                if (OldVal.IMDbID == null) { OldVal.IMDbID = ""; }
                if (NewVal.IMDbID == null) { NewVal.IMDbID = ""; }
                if (OldVal.IMDbID != NewVal.IMDbID) { different = true; }
                if (OldVal.ShowName == null) { OldVal.ShowName = ""; }
                if (NewVal.ShowName == null) { NewVal.ShowName = ""; }
                if (OldVal.ShowName != NewVal.ShowName) { different = true; }
                if (OldVal.EpNum == null) { OldVal.EpNum = ""; }
                if (NewVal.EpNum == null) { NewVal.EpNum = ""; }
                if (OldVal.EpNum != NewVal.EpNum) { different = true; }
                if (OldVal.EpisodeName == null) { OldVal.EpisodeName = ""; }
                if (NewVal.EpisodeName == null) { NewVal.EpisodeName = ""; }
                if (OldVal.EpisodeName != NewVal.EpisodeName) { different = true; }
                if (OldVal.EpDescription == null) { OldVal.EpDescription = ""; }
                if (NewVal.EpDescription == null) { NewVal.EpDescription = ""; }
                if (OldVal.EpDescription != NewVal.EpDescription) { different = true; }
                if (OldVal.LocalPath == null) { OldVal.LocalPath = ""; }
                if (NewVal.LocalPath == null) { NewVal.LocalPath = ""; }
                if (OldVal.LocalPath != NewVal.LocalPath) { different = true; }
                if (OldVal.AirDate == null) { OldVal.AirDate = ""; }
                if (NewVal.AirDate == null) { NewVal.AirDate = ""; }
                if (OldVal.AirDate != NewVal.AirDate) { different = true; }
                if (OldVal.ShowImage == null) { OldVal.ShowImage = ""; }
                if (NewVal.ShowImage == null) { NewVal.ShowImage = ""; }
                if (OldVal.ShowImage != NewVal.ShowImage) { different = true; }
                if (OldVal.Flag == null) { OldVal.Flag = ""; }
                if (NewVal.Flag == null) { NewVal.Flag = ""; }
                if (OldVal.Flag != NewVal.Flag) { different = true; }
                return different;
            }

            // Returns object containing the new values different from the old values in object comparison
            public ImdbEps ImdbEps_Diff(ImdbEps OldVal, ImdbEps NewVal)
            {
                ImdbEps diff = new ImdbEps();
                if (OldVal.ID != NewVal.ID) { diff.ID = NewVal.ID; }
                if (OldVal.EpGuideID != NewVal.EpGuideID) { diff.EpGuideID = NewVal.EpGuideID; }
                if (OldVal.IMDbID != NewVal.IMDbID) { diff.IMDbID = NewVal.IMDbID; }
                if (OldVal.ShowName != NewVal.ShowName) { diff.ShowName = NewVal.ShowName; }
                if (OldVal.EpNum != NewVal.EpNum) { diff.EpNum = NewVal.EpNum; }
                if (OldVal.EpisodeName != NewVal.EpisodeName) { diff.EpisodeName = NewVal.EpisodeName; }
                if (OldVal.EpDescription != NewVal.EpDescription) { diff.EpDescription = NewVal.EpDescription; }
                if (OldVal.LocalPath != NewVal.LocalPath) { diff.LocalPath = NewVal.LocalPath; }
                if (OldVal.AirDate != NewVal.AirDate) { diff.AirDate = NewVal.AirDate; }
                if (OldVal.ShowImage != NewVal.ShowImage) { diff.ShowImage = NewVal.ShowImage; }
                if (OldVal.Flag != NewVal.Flag) { diff.Flag = NewVal.Flag; }
                return diff;
            }

            // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
            public List<string> ImdbEps_DiffList(ImdbEps OldVal, ImdbEps NewVal)
            {
                List<string> diffList = new List<string>();
                if (OldVal.ID != NewVal.ID) { diffList.Add("Changed ID Value From " + OldVal.ID + " To " + NewVal.ID); }
                if (OldVal.EpGuideID != NewVal.EpGuideID) { diffList.Add("Changed EpGuideID Value From " + OldVal.EpGuideID + " To " + NewVal.EpGuideID); }
                if (OldVal.IMDbID != NewVal.IMDbID) { diffList.Add("Changed IMDbID Value From " + OldVal.IMDbID + " To " + NewVal.IMDbID); }
                if (OldVal.ShowName != NewVal.ShowName) { diffList.Add("Changed ShowName Value From " + OldVal.ShowName + " To " + NewVal.ShowName); }
                if (OldVal.EpNum != NewVal.EpNum) { diffList.Add("Changed EpNum Value From " + OldVal.EpNum + " To " + NewVal.EpNum); }
                if (OldVal.EpisodeName != NewVal.EpisodeName) { diffList.Add("Changed EpisodeName Value From " + OldVal.EpisodeName + " To " + NewVal.EpisodeName); }
                if (OldVal.EpDescription != NewVal.EpDescription) { diffList.Add("Changed EpDescription Value From " + OldVal.EpDescription + " To " + NewVal.EpDescription); }
                if (OldVal.LocalPath != NewVal.LocalPath) { diffList.Add("Changed LocalPath Value From " + OldVal.LocalPath + " To " + NewVal.LocalPath); }
                if (OldVal.AirDate != NewVal.AirDate) { diffList.Add("Changed AirDate Value From " + OldVal.AirDate + " To " + NewVal.AirDate); }
                if (OldVal.ShowImage != NewVal.ShowImage) { diffList.Add("Changed ShowImage Value From " + OldVal.ShowImage + " To " + NewVal.ShowImage); }
                if (OldVal.Flag != NewVal.Flag) { diffList.Add("Changed Flag Value From " + OldVal.Flag + " To " + NewVal.Flag); }
                return diffList;
            }


            #endregion
            #region ===== ImdbEps SQLite : Return =====

            public ImdbEps Return_Object_From_IMDb_Episodes(string DbFile, string WhereClause = "[ID] = ''")
            {
                string SelectLine = "Select [ID], [EpGuideID], [IMDbID], [ShowName], [EpNum], [EpisodeName], [EpDescription], [LocalPath], [AirDate], [ShowImage], [Flag] From [IMDb_Episodes]";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
                ImdbEps returnObject = new ImdbEps();
                int i = 0;
                string Value = "";
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.EpGuideID = ret["EpGuideID"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.EpNum = ret["EpNum"].ToString();
                        returnObject.EpisodeName = ret["EpisodeName"].ToString();
                        returnObject.EpDescription = ret["EpDescription"].ToString();
                        returnObject.LocalPath = ret["LocalPath"].ToString();
                        returnObject.AirDate = ret["AirDate"].ToString();
                        returnObject.ShowImage = ret["ShowImage"].ToString();
                        returnObject.Flag = ret["Flag"].ToString();
                    }
                }

                return returnObject;
            }

            public List<ImdbEps> Return_ImdbEps_List(string DbFile, string TableName = "[IMDb_Episodes]", string WhereClause = "")
            {
                string SelectLine = "Select * From " + TableName;

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

                List<ImdbEps> ReturnList = new List<ImdbEps>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        ImdbEps returnObject = new ImdbEps();

                        returnObject.ID = ret["ID"].ToString();
                        returnObject.EpGuideID = ret["EpGuideID"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.EpNum = ret["EpNum"].ToString();
                        returnObject.EpisodeName = ret["EpisodeName"].ToString();
                        returnObject.EpDescription = ret["EpDescription"].ToString();
                        returnObject.LocalPath = ret["LocalPath"].ToString();
                        returnObject.AirDate = ret["AirDate"].ToString();
                        returnObject.ShowImage = ret["ShowImage"].ToString();
                        returnObject.Flag = ret["Flag"].ToString();

                        ReturnList.Add(returnObject);
                    }
                }

                return ReturnList;
            }

            public DataTable Return_DataTable_From_IMDb_Episodes(string DbFile)
            {
                string SelectLine = "Select [ID], [EpGuideID], [IMDbID], [ShowName], [EpNum], [EpisodeName], [EpDescription], [LocalPath], [AirDate], [ShowImage], [Flag] From [IMDb_Episodes]";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                return ReturnTable;
            }


            #endregion
            #region ===== ImdbEps SQLite : Update Insert =====

            public bool ImdbEps_Insert(string DbFile, ImdbEps inObject)
            {
                if (inObject.EpGuideID == null) { inObject.EpGuideID = ""; }
                if (inObject.IMDbID == null) { inObject.IMDbID = ""; }
                if (inObject.ShowName == null) { inObject.ShowName = ""; }
                if (inObject.EpNum == null) { inObject.EpNum = ""; }
                if (inObject.EpisodeName == null) { inObject.EpisodeName = ""; }
                if (inObject.EpDescription == null) { inObject.EpDescription = ""; }
                if (inObject.LocalPath == null) { inObject.LocalPath = ""; }
                if (inObject.AirDate == null) { inObject.AirDate = ""; }
                if (inObject.Flag == null) { inObject.Flag = ""; }

                inObject.EpisodeName = inObject.EpisodeName.Encode();
                inObject.ShowName = inObject.ShowName.Encode();
                inObject.EpDescription = inObject.EpDescription.Encode();
                inObject.LocalPath = inObject.LocalPath.Encode();

                //string InsertLine = "Insert Into [IMDb_Episodes] (EpGuideID, IMDbID, ShowName, EpNum, EpisodeName, EpDescription, LocalPath, AirDate, ShowImage, Flag) values ('" + inObject.EpGuideID + "', '" + inObject.IMDbID + "', '" + inObject.ShowName + "', '" + inObject.EpNum + "', '" + inObject.EpisodeName + "', '" + inObject.EpDescription + "', '" + inObject.LocalPath + "', '" + inObject.AirDate + "', '" + inObject.ShowImage + "', '" + inObject.Flag + "')";
                string InsertLine = "Insert Into [IMDb_Episodes] (EpGuideID, IMDbID, ShowName, EpNum, EpisodeName, EpDescription, LocalPath, AirDate, Flag) values ('" + inObject.EpGuideID + "', '" + inObject.IMDbID + "', '" + inObject.ShowName + "', '" + inObject.EpNum + "', '" + inObject.EpisodeName + "', '" + inObject.EpDescription + "', '" + inObject.LocalPath + "', '" + inObject.AirDate + "', '" + inObject.Flag + "')";
                bool Inserted = sqlite.Execute(DbFile, InsertLine);
                if (!Inserted) { ahk.MsgBox("Inserted Into [IMDb_Episodes] = " + Inserted.ToString()); }
                return Inserted;
            }

            public bool ImdbEps_Update(string DbFile, ImdbEps inObject)
            {
                //string UpdateLine = "Update [IMDb_Episodes] set ID = '" + inObject.ID + "', EpGuideID = '" + inObject.EpGuideID + "', IMDbID = '" + inObject.IMDbID + "', ShowName = '" + inObject.ShowName + "', EpNum = '" + inObject.EpNum + "', EpisodeName = '" + inObject.EpisodeName + "', EpDescription = '" + inObject.EpDescription + "', LocalPath = '" + inObject.LocalPath + "', AirDate = '" + inObject.AirDate + "', ShowImage = '" + inObject.ShowImage + "', Flag = '" + inObject.Flag + "' WHERE [Item] = 'Value' ";
                string UpdateLine = "Update [IMDb_Episodes] set ";

                if (inObject.EpisodeName != null)
                {
                    inObject.EpisodeName = inObject.EpisodeName.Encode();
                }

                if (inObject.EpGuideID != null) { UpdateLine = UpdateLine + "[EpGuideID] = '" + inObject.EpGuideID + "',"; }
                if (inObject.IMDbID != null) { UpdateLine = UpdateLine + "[IMDbID] = '" + inObject.IMDbID + "',"; }
                if (inObject.ShowName != null)
                {
                    inObject.ShowName = inObject.ShowName.Encode();
                    UpdateLine = UpdateLine + "[ShowName] = '" + inObject.ShowName + "',";
                }
                if (inObject.EpNum != null) { UpdateLine = UpdateLine + "[EpNum] = '" + inObject.EpNum + "',"; }
                if (inObject.EpisodeName != null) { UpdateLine = UpdateLine + "[EpisodeName] = '" + inObject.EpisodeName + "',"; }
                if (inObject.EpDescription != null)
                {
                    inObject.EpDescription = inObject.EpDescription.Encode();
                    UpdateLine = UpdateLine + "[EpDescription] = '" + inObject.EpDescription + "',";
                }
                if (inObject.LocalPath != null)
                {
                    inObject.LocalPath = inObject.LocalPath.Encode();
                    UpdateLine = UpdateLine + "[LocalPath] = '" + inObject.LocalPath + "',";
                }
                if (inObject.AirDate != null) { UpdateLine = UpdateLine + "[AirDate] = '" + inObject.AirDate + "',"; }
                //if (inObject.ShowImage != null) { UpdateLine = UpdateLine + "[ShowImage] = '" + inObject.ShowImage + "',"; }
                if (inObject.Flag != null) { UpdateLine = UpdateLine + "[Flag] = '" + inObject.Flag + "',"; }

                UpdateLine = ahk.TrimLast(UpdateLine, 1);
                UpdateLine = UpdateLine + " WHERE [IMDbID] = '" + inObject.IMDbID + "' AND [EpNum] = '" + inObject.EpNum + "'";

                bool Updated = sqlite.Execute(DbFile, UpdateLine);
                return Updated;
            }

            public bool ImdbEps_UpdateInsert(string DbFile, ImdbEps obj)
            {
                //string name = ahk.FileName(DbFile);
                //name = name.Decode();
                //name = name.Replace(":", "-");

                //DbFile = ahk.FileDir(DbFile) + "\\" + name;

                //if (DbFile == "\\")
                //{
                //    string problem = "this";
                //}

                ImdbEps_NewDb(DbFile, "IMDb_Episodes");  // make new db file if not created yet

                bool Updated = ImdbEps_Update(DbFile, obj);  // try to update record first
                if (!Updated) { Updated = ImdbEps_Insert(DbFile, obj); }  // if unable to update, insert new record
                return Updated;
            }


            /// <summary>Creates IMDb Show Table If it Doesn't Exist</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To AppName.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            /// <param name="OverWriteExisting">Option to Overwrite Previous Setting Table If Found</param>
            public bool ImdbEps_NewDb(string DbFile = "IMDbEpisodes.sqlite", string TableName = "IMDb_Episodes", bool OverWriteExisting = false)
            {
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                if (OverWriteExisting)  // option to clear out previous Db setttings
                {
                    sqlite.Table_Clear(DbFile, TableName, true);
                }

                // Create New SQLite DB (*Used First-Run*)
                if (!File.Exists(DbFile)) // create database file if it doen't exist
                {
                    SQLiteConnection.CreateFile(DbFile);
                }

                // Create New Table If It Does NOT Exist Yet
                bool TableExist = sqlite.Table_Exists(DbFile, TableName);  //See if selected Table Exists in SQLite DB file

                if (!TableExist)  // Table DOES NOT exist in SQLite DB
                {
                    string NewTableLine = "ID INTEGER PRIMARY KEY, EpGuideID VARCHAR, IMDbID VARCHAR, ShowName VARCHAR, EpNum VARCHAR, EpisodeName VARCHAR, EpDescription VARCHAR, LocalPath VARCHAR, AirDate VARCHAR, ShowImage VARCHAR, Flag VARCHAR";

                    //ahk.MsgBox(NewTableLine); 

                    bool ReturnValue = sqlite.Execute(DbFile, "CREATE TABLE [" + TableName + "] (" + NewTableLine + ")");  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
                }


                if (File.Exists(DbFile)) { return true; }
                return false;
            }


            #endregion
            #region ===== ImdbEps DataTable =====

            public DataTable Return_ImdbEps_DataTable(string DbFile, string TableName = "ImdbEps", string WhereClause = "", bool Debug = false)
            {
                string SelectLine = "Select * From " + IMDbEpisodeTable;

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }

                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


                DataTable table = new DataTable();
                table.Columns.Add("ID", typeof(string));
                table.Columns.Add("EpGuideID", typeof(string));
                table.Columns.Add("IMDbID", typeof(string));
                table.Columns.Add("ShowName", typeof(string));
                table.Columns.Add("EpNum", typeof(string));
                table.Columns.Add("EpisodeName", typeof(string));
                table.Columns.Add("EpDescription", typeof(string));
                table.Columns.Add("LocalPath", typeof(string));
                table.Columns.Add("AirDate", typeof(string));
                table.Columns.Add("ShowImage", typeof(string));
                table.Columns.Add("Flag", typeof(string));

                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        ImdbEps returnObject = new ImdbEps();

                        returnObject.ID = ret["ID"].ToString();
                        returnObject.EpGuideID = ret["EpGuideID"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.EpNum = ret["EpNum"].ToString();
                        returnObject.EpisodeName = ret["EpisodeName"].ToString();
                        returnObject.EpDescription = ret["EpDescription"].ToString();
                        returnObject.LocalPath = ret["LocalPath"].ToString();
                        returnObject.AirDate = ret["AirDate"].ToString();
                        returnObject.ShowImage = ret["ShowImage"].ToString();
                        returnObject.Flag = ret["Flag"].ToString();

                        table.Rows.Add(returnObject.ID, returnObject.EpGuideID, returnObject.IMDbID, returnObject.ShowName, returnObject.EpNum, returnObject.EpisodeName, returnObject.EpDescription, returnObject.LocalPath, returnObject.AirDate, returnObject.ShowImage, returnObject.Flag);
                    }
                }

                return table;
            }

            public DataTable Create_IMDb_Episodes_DataTable(ImdbEps inObject)
            {
                DataTable table = new DataTable();
                table.Columns.Add("ID", typeof(string));
                table.Columns.Add("EpGuideID", typeof(string));
                table.Columns.Add("IMDbID", typeof(string));
                table.Columns.Add("ShowName", typeof(string));
                table.Columns.Add("EpNum", typeof(string));
                table.Columns.Add("EpisodeName", typeof(string));
                table.Columns.Add("EpDescription", typeof(string));
                table.Columns.Add("LocalPath", typeof(string));
                table.Columns.Add("AirDate", typeof(string));
                table.Columns.Add("ShowImage", typeof(string));
                table.Columns.Add("Flag", typeof(string));

                table.Rows.Add(inObject.ID, inObject.EpGuideID, inObject.IMDbID, inObject.ShowName, inObject.EpNum, inObject.EpisodeName, inObject.EpDescription, inObject.LocalPath, inObject.AirDate, inObject.ShowImage, inObject.Flag);
                return table;
            }


            #endregion
            #region ===== ImdbEps DataGridView =====

            public void HideShow_IMDb_Episodes_Columns(DataGridView dv)
            {

                try { dv.Columns["ID"].Visible = true; } catch { }
                try { dv.Columns["EpGuideID"].Visible = true; } catch { }
                try { dv.Columns["IMDbID"].Visible = true; } catch { }
                try { dv.Columns["ShowName"].Visible = true; } catch { }
                try { dv.Columns["EpNum"].Visible = true; } catch { }
                try { dv.Columns["EpisodeName"].Visible = true; } catch { }
                try { dv.Columns["EpDescription"].Visible = true; } catch { }
                try { dv.Columns["LocalPath"].Visible = true; } catch { }
                try { dv.Columns["AirDate"].Visible = true; } catch { }
                try { dv.Columns["ShowImage"].Visible = true; } catch { }
                try { dv.Columns["Flag"].Visible = true; } catch { }
            }
            public void Enable_TableName_Columns(DataGridView dv)
            {

                try { dv.Columns["ID"].ReadOnly = true; } catch { }
                try { dv.Columns["EpGuideID"].ReadOnly = true; } catch { }
                try { dv.Columns["IMDbID"].ReadOnly = true; } catch { }
                try { dv.Columns["ShowName"].ReadOnly = true; } catch { }
                try { dv.Columns["EpNum"].ReadOnly = true; } catch { }
                try { dv.Columns["EpisodeName"].ReadOnly = true; } catch { }
                try { dv.Columns["EpDescription"].ReadOnly = true; } catch { }
                try { dv.Columns["LocalPath"].ReadOnly = true; } catch { }
                try { dv.Columns["AirDate"].ReadOnly = true; } catch { }
                try { dv.Columns["ShowImage"].ReadOnly = true; } catch { }
                try { dv.Columns["Flag"].ReadOnly = true; } catch { }
            }

            #endregion
            #region ===== ImdbEps SQL Functions =====

            // Return ImdbEps SQL Connection String
            public SqlConnection ImdbEps_Conn(string ConnName = "SQLserver|LITMLucidMedia")
            {
                if (ConnName == "SQLserver|LITMLucidMedia") { ConnName = "LITMLucidMedia"; }
                // populate sql connection
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[ConnName].ConnectionString);
                // SqlConnection Conn = new SqlConnection("Server=188.168.188.88;DataBase=LucidMedia;Uid=lucidm;Pwd=pass");
                return conn;
            }

            // Return ImdbEps TableName (Full Path)
            public string ImdbEps_TableName()
            {
                // populate to return full sql table name
                return IMDbEpisodeTable;
            }

            // Generate SQL Table
            public bool ImdbEps_CreateSQLTable()
            {
                SqlConnection Conn = ImdbEps_Conn();
                string CreateTableLine = "CREATE TABLE " + ImdbEps_TableName() + "(";
                CreateTableLine = CreateTableLine + "[ID] [int] IDENTITY(1,1) NOT NULL,";
                CreateTableLine = CreateTableLine + "[EpGuideID] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[IMDbID] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[ShowName] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[EpNum] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[EpisodeName] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[EpDescription] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[LocalPath] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[AirDate] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[ShowImage] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Flag] [varchar](max) NOT NULL";
                CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                return false;
            }

            public bool ImdbEps_InsertSQL(ImdbEps obj)
            {
                SqlConnection Con = ImdbEps_Conn();
                string SQLLine = "Insert Into " + ImdbEps_TableName() + "(EpGuideID, IMDbID, ShowName, EpNum, EpisodeName, EpDescription, LocalPath, AirDate, ShowImage, Flag) VALUES (@EpGuideID, @IMDbID, @ShowName, @EpNum, @EpisodeName, @EpDescription, @LocalPath, @AirDate, @ShowImage, @Flag)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.EpGuideID == null) { obj.EpGuideID = ""; }
                if (obj.IMDbID == null) { obj.IMDbID = ""; }
                if (obj.ShowName == null) { obj.ShowName = ""; }
                if (obj.EpNum == null) { obj.EpNum = ""; }
                if (obj.EpisodeName == null) { obj.EpisodeName = ""; }
                if (obj.EpDescription == null) { obj.EpDescription = ""; }
                if (obj.LocalPath == null) { obj.LocalPath = ""; }
                if (obj.AirDate == null) { obj.AirDate = ""; }
                if (obj.ShowImage == null) { obj.ShowImage = ""; }
                if (obj.Flag == null) { obj.Flag = ""; }
                cmd2.Parameters.AddWithValue(@"EpGuideID", obj.EpGuideID.ToString());
                cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID.ToString());
                cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                cmd2.Parameters.AddWithValue(@"EpNum", obj.EpNum.ToString());
                cmd2.Parameters.AddWithValue(@"EpisodeName", obj.EpisodeName.ToString());
                cmd2.Parameters.AddWithValue(@"EpDescription", obj.EpDescription.ToString());
                cmd2.Parameters.AddWithValue(@"LocalPath", obj.LocalPath.ToString());
                cmd2.Parameters.AddWithValue(@"AirDate", obj.AirDate.ToString());

                // convert file to binary to store in sql db
                byte[] file;
                using (var stream = new FileStream(obj.ShowImage, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        file = reader.ReadBytes((int)stream.Length);
                    }
                }
                cmd2.Parameters.AddWithValue(@"ShowImage", file);

                cmd2.Parameters.AddWithValue(@"Flag", obj.Flag.ToString());
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Con.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool ImdbEps_UpdateSQL(ImdbEps obj)
            {
                SqlConnection Conn = ImdbEps_Conn();
                string SQLLine = "Update " + ImdbEps_TableName() + " SET EpGuideID = @EpGuideID, IMDbID = @IMDbID, ShowName = @ShowName, EpNum = @EpNum, EpisodeName = @EpisodeName, EpDescription = @EpDescription, LocalPath = @LocalPath, AirDate = @AirDate, ShowImage = @ShowImage, Flag = @Flag WHERE IMDbID = @IMDbID AND EpNum = @EpNum";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);

                if (obj.EpGuideID == null) { obj.EpGuideID = ""; }
                if (obj.IMDbID == null) { obj.IMDbID = ""; }
                if (obj.ShowName == null) { obj.ShowName = ""; }
                if (obj.EpNum == null) { obj.EpNum = ""; }
                if (obj.EpisodeName == null) { obj.EpisodeName = ""; }
                if (obj.EpDescription == null) { obj.EpDescription = ""; }
                if (obj.LocalPath == null) { obj.LocalPath = ""; }
                if (obj.AirDate == null) { obj.AirDate = ""; }
                if (obj.ShowImage == null) { obj.ShowImage = ""; }
                if (obj.Flag == null) { obj.Flag = ""; }

                cmd2.Parameters.AddWithValue(@"EpGuideID", obj.EpGuideID.ToString());
                cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID.ToString());
                cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                cmd2.Parameters.AddWithValue(@"EpNum", obj.EpNum.ToString());
                cmd2.Parameters.AddWithValue(@"EpisodeName", obj.EpisodeName.ToString());
                cmd2.Parameters.AddWithValue(@"EpDescription", obj.EpDescription.ToString());
                cmd2.Parameters.AddWithValue(@"LocalPath", obj.LocalPath.ToString());
                cmd2.Parameters.AddWithValue(@"AirDate", obj.AirDate.ToString());


                // convert file to binary to store in sql db
                byte[] file;
                using (var stream = new FileStream(obj.ShowImage, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        file = reader.ReadBytes((int)stream.Length);
                    }
                }
                cmd2.Parameters.AddWithValue(@"ShowImage", file);


                cmd2.Parameters.AddWithValue(@"Flag", obj.Flag.ToString());
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool ImdbEps_UpdateInsert(ImdbEps obj)
            {
                SqlConnection Conn = ImdbEps_Conn();
                bool Updated = ImdbEps_UpdateSQL(obj);  // try to update record first
                if (!Updated) { Updated = ImdbEps_InsertSQL(obj); }  // if unable to update, insert new record
                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool ImdbEps_UpdateIfPopulated(ImdbEps obj, string ID = "")
            {
                SqlConnection Conn = ImdbEps_Conn();
                string SQLcmd = "Update IMDb_Episodes SET ";
                if (obj.ID != null) { SQLcmd = SQLcmd + " ID = @ID,"; }
                if (obj.EpGuideID != null) { SQLcmd = SQLcmd + " EpGuideID = @EpGuideID,"; }
                if (obj.IMDbID != null) { SQLcmd = SQLcmd + " IMDbID = @IMDbID,"; }
                if (obj.ShowName != null) { SQLcmd = SQLcmd + " ShowName = @ShowName,"; }
                if (obj.EpNum != null) { SQLcmd = SQLcmd + " EpNum = @EpNum,"; }
                if (obj.EpisodeName != null) { SQLcmd = SQLcmd + " EpisodeName = @EpisodeName,"; }
                if (obj.EpDescription != null) { SQLcmd = SQLcmd + " EpDescription = @EpDescription,"; }
                if (obj.LocalPath != null) { SQLcmd = SQLcmd + " LocalPath = @LocalPath,"; }
                if (obj.AirDate != null) { SQLcmd = SQLcmd + " AirDate = @AirDate,"; }
                if (obj.ShowImage != null) { SQLcmd = SQLcmd + " ShowImage = @ShowImage,"; }
                if (obj.Flag != null) { SQLcmd = SQLcmd + " Flag = @Flag,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ID = @ID";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.ID != null) { cmd2.Parameters.AddWithValue(@"ID", obj.ID); }
                if (obj.EpGuideID != null) { cmd2.Parameters.AddWithValue(@"EpGuideID", obj.EpGuideID); }
                if (obj.IMDbID != null) { cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID); }
                if (obj.ShowName != null) { cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName); }
                if (obj.EpNum != null) { cmd2.Parameters.AddWithValue(@"EpNum", obj.EpNum); }
                if (obj.EpisodeName != null) { cmd2.Parameters.AddWithValue(@"EpisodeName", obj.EpisodeName); }
                if (obj.EpDescription != null) { cmd2.Parameters.AddWithValue(@"EpDescription", obj.EpDescription); }
                if (obj.LocalPath != null) { cmd2.Parameters.AddWithValue(@"LocalPath", obj.LocalPath); }
                if (obj.AirDate != null) { cmd2.Parameters.AddWithValue(@"AirDate", obj.AirDate); }
                if (obj.ShowImage != null) { cmd2.Parameters.AddWithValue(@"ShowImage", obj.ShowImage); }
                if (obj.Flag != null) { cmd2.Parameters.AddWithValue(@"Flag", obj.Flag); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public ImdbEps ImdbEps_ReturnSQL(string ID = "")
            {
                SqlConnection Conn = ImdbEps_Conn();
                string SelectLine = "Select [ID],[EpGuideID],[IMDbID],[ShowName],[EpNum],[EpisodeName],[EpDescription],[LocalPath],[AirDate],[ShowImage],[Flag] From " + ImdbEps_TableName() + " WHERE ID = '" + ID + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                ImdbEps returnObject = new ImdbEps();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.EpGuideID = ret["EpGuideID"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.EpNum = ret["EpNum"].ToString();
                        returnObject.EpisodeName = ret["EpisodeName"].ToString();
                        returnObject.EpDescription = ret["EpDescription"].ToString();
                        returnObject.LocalPath = ret["LocalPath"].ToString();
                        returnObject.AirDate = ret["AirDate"].ToString();
                        returnObject.ShowImage = ret["ShowImage"].ToString();
                        returnObject.Flag = ret["Flag"].ToString();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public List<ImdbEps> ImdbEps_ReturnSQLList(string Command)
            {
                SqlConnection Conn = ImdbEps_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<ImdbEps> ReturnList = new List<ImdbEps>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        ImdbEps returnObject = new ImdbEps();
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.EpGuideID = ret["EpGuideID"].ToString();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.EpNum = ret["EpNum"].ToString();
                        returnObject.EpisodeName = ret["EpisodeName"].ToString();
                        returnObject.EpDescription = ret["EpDescription"].ToString();
                        returnObject.LocalPath = ret["LocalPath"].ToString();
                        returnObject.AirDate = ret["AirDate"].ToString();
                        returnObject.ShowImage = ret["ShowImage"].ToString();
                        returnObject.Flag = ret["Flag"].ToString();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }


            #endregion

            #endregion


            //### MOVIES ############################


            #region === Main Actions ===


            /// <summary>
            /// Check To See if IMDb Title Found In My Collection
            /// </summary>
            /// <param name="IMDbID">IMDb Title ID or IMDb.com URL</param>
            /// <returns></returns>
            public bool InLib(string IMDbID = "tt1211837")
            {
                SqlConnection Conn = imdB_Conn("LITMLucidMedia");

                _Lists lst = new _Lists();
                List<string> recordsFound = lst.SQL_To_List(Conn, "select * FROM " + IMDbMovieTable + " where IMDbID = '" + IMDbID + "'");

                if (recordsFound.Count > 0)
                {
                    //StatusBar("IMDb Title" + IMDbID + " FOUND in Collection");
                    return true;
                }

                //StatusBar("IMDb Title" + IMDbID + " [NOT] FOUND in Collection");
                return false;

            }

            /// <summary>
            /// Prompt to enter IMDb ID, check against library to see if found 
            /// </summary>
            /// <returns></returns>
            public bool IMDb_InLibPrompt()
            {
                string value = "";

                // input box not setup yet
                //if (ahk.InputBox("IMDb.ID", "IMDb TitleID: ", ref value) == DialogResult.OK)
                //{
                //    bool inLib = InLib(value);
                //    ahk.MsgBox("In Library = " + inLib);
                //    return true;
                //}

                return false;
            }


            /// <summary>
            /// Parses IMDb URL for IMDb Title ID
            /// </summary>
            /// <param name="URL">IMDb.com URL to Parse</param>
            /// <returns>Return IMDb ID</returns>
            public string IMDb_ID_FromURL(string URL)
            {
                Imdb imd = new Imdb();
                return imd.ParseID(URL);
            }



            // pass in IMDbID (AFTER Lookup / Movie Loaded to GUI), Creates Local Movie Dir with IMDb URL 
            // Returns Path to New Directory
            public string IMDb_NewDir_NoLookup(imdB movieInfo, string SaveDirRoot)
            {
                movieInfo = FixIMDbTitle(movieInfo);

                string url = "http://www.imdb.com/title/" + movieInfo.IMDbID + "/";

                string newDirName = SaveDirRoot + "\\" + movieInfo.Title + " (" + movieInfo.Year + ")";

                ahk.FileCreateDir(newDirName);

                bool WroteURL = ahk.IniWrite(url, newDirName + "\\" + movieInfo.Title + " (" + movieInfo.Year + ") - IMDb.url", "InternetShortcut", "URL");

                return newDirName;

            }



            // update IMDb Library with Latest Additions
            public void UpdateAllImdb(string SearchDir = "", bool SkipExisting = false, bool WriteMissingToDb = true, Telerik.WinControls.UI.RadProgressBar progress1 = null, Telerik.WinControls.UI.RadProgressBar progress2 = null)
            {
                Thread zipthread = new Thread(() => _UpdateAllImdb(SearchDir, SkipExisting, WriteMissingToDb));
                zipthread.Start();
            }

            private void _UpdateAllImdb(string SearchDir = "", bool SkipExisting = false, bool WriteMissingToDb = true, Telerik.WinControls.UI.RadProgressBar progress1 = null, Telerik.WinControls.UI.RadProgressBar progress2 = null)
            {
                _StatusBar sb = new _StatusBar();

                List<string> SearchDirs = new List<string>();

                //SearchDir = @"E:\_Movies_3"; 

                // reset global counters
                imdbUpdateCount = 0;
                imdbInsertCount = 0;

                if (SearchDir != "")
                {
                    SearchDirs.Add(SearchDir);
                }
                if (SearchDir == "")
                {
                    //string MovieDir = @"M:";
                    ////MovieDir = @"F:\[ Movies ]";
                    //SearchDirs.Add(MovieDir + @"\[ Movies A-K ]");
                    //SearchDirs.Add(MovieDir + @"\[ Movies L-S ]");
                    //SearchDirs.Add(MovieDir + @"\[ Movies T-Z ]");
                    ////SearchDirs.Add(@"M:\[ Theater Rips ]");


                    TVServerLocal tvs = new TVServerLocal();

                    // list of local tv folders to search for folder names
                    SearchDirs = tvs.LocalMovieDirs();

                }


                //pro.ResetProgress(progress1);
                //pro.SetupProgressBar1(progress1, SearchDirs.Count());
                //pro.UpdateProgress(progress1);

                int i = 1;
                foreach (string dirPath in SearchDirs)
                {
                    _Sites.OMDB omdb = new OMDB();
                    _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();

                    if (!Directory.Exists(dirPath)) { continue; }

                    sb.StatusBar("IMDb Index Update : Collecting IMDb Links... [ " + i + " / " + SearchDirs.Count + " ]");

                    string[] files = Directory.GetFiles(dirPath, "*.url", System.IO.SearchOption.AllDirectories);

                    pro.ResetProgress(progress1);
                    pro.SetupProgressBar(progress1, files.Count());

                    int count = 1;
                    foreach (string file in files)
                    {
                        pro.UpdateProgress(progress1);

                        sb.StatusBar("Updating IMDb Movie Index : " + count.ToString() + " / " + files.Count().ToString() + " | Updated " + imdbUpdateCount + " | NEW " + imdbInsertCount);

                        //if (file.Contains("[ Theater Rips ]")) { count++; continue; }
                        //if (file.Contains("[ INCOMING ]")) { count++; continue; }


                        string URLPath = ahk.IniRead(file, "InternetShortcut", "URL");
                        if (URLPath.ToUpper().Contains("IMDB.COM"))
                        {
                            // read imdb id from url, lookup movie info from OMDb API
                            imdB Movie = omdb.OMdb(ParseID(URLPath));

                            string fileDir = ahk.FileDir(file);
                            Movie.RootDir = fileDir;
                            //long dirsize = _AHK.DirSize(fileDir);
                            //Movie.DirSize = ahk.FormatBytes(dirsize);

                            Movie.DirSize = ahk.DirSize(fileDir);


                            // PREVIOUS VERSION - NOT AS ACCURATE FROM IMDB PARSE
                            //imdB Movie = parse_IMDb_Page(URLPath, file, SkipExisting, WriteMissingToDb);

                            if (Movie.Title == "" || Movie.Title == null) { continue; }

                            imdB_UpdateInsert(Movie);
                        }

                        count++;
                    }

                    i++;
                }


                sb.StatusBar("Finished Updating IMDb Library | Updated " + imdbUpdateCount + " | NEW " + imdbInsertCount);
            }





            /// <summary>
            /// Parses imdb url, extras IMDb Title ID
            /// </summary>
            /// <param name="URL">Parse IMDb.Com URL for TitleID</param>
            /// <returns>Returns IMDb TitleID from URL</returns>
            public string ParseID(string URL)
            {
                if (URL == "" || URL == null) { return "0"; }

                string ID = URL;
                ID = ahk.StringSplit(ID, "?", 0);
                ID = ahk.StringReplace(ID, "http://www.imdb.com/title/");
                ID = ahk.StringReplace(ID, "http://imdb.com/title/");
                ID = ahk.StringReplace(ID, "https://www.imdb.com/title/");
                ID = ahk.StringReplace(ID, "https://imdb.com/title/");
                ID = ahk.StringReplace(ID, "/episodes");
                ID = ahk.StringReplace(ID, "/awards");
                ID = ahk.StringReplace(ID, "/");
                return ID;
            }

            /// <summary>
            /// Extract IMDb ID from HTML
            /// </summary>
            /// <param name="HTML"></param>
            /// <returns></returns>
            public string IMDbFromHTML(string HTML)
            {
                List<string> imdbLinks = Regex_IMDbLinks(HTML);
                string IMDbID = "";
                if (imdbLinks.Count > 0) { IMDbID = imdbLinks[0]; }
                if (imdbLinks.Count > 1)
                {
                    if (IMDbID.Contains("/chart")) { IMDbID = imdbLinks[1]; }
                }

                return ParseID(IMDbID);
            }


            //Parse IMDb page data
            // SkipExisting = false;  // skips updating exsting sql db entry for this film
            public _Sites.Imdb.imdB parse_IMDb_Page(string imdbUrl, string imdbLinkFile = "", bool SkipExisting = false, bool WriteMissingToDb = true)
            {
                imdB mov = new imdB();
                _AHK ahk = new _AHK();

                mov.RootDir = "";
                mov.DirSize = "";
                Id = ParseID(imdbUrl);   // parses imdb url, extras title id

                string fixedURL = "http://www.imdb.com/title/" + Id + "/";

                // option to not search for / load imdb content from web 
                if (SkipExisting)
                {
                    // check to see if title already found in sql db
                    bool AlreadyInDb = inDb(Id);
                    if (AlreadyInDb)
                    {
                        mov = imdB_ReturnSQL(Id); // return sql entry for this film
                                                  //StatusBar(Id + " Already Found In Db - Skipping...");
                        return mov;
                    }
                }

                if (imdbLinkFile != "")
                {
                    mov.RootDir = ahk.FileDir(imdbLinkFile);
                    //mov.RootDir = ahk.StringReplace(mov.RootDir, "'", "`'", "ALL"); 

                    //mov.DirSize = ahk.DirSize(mov.RootDir).ToString();

                    //mov.DirSize = ahk.DirSize(mov.RootDir);

                    ////string dirBytes = _AHK.DirSize(mov.RootDir).ToString();
                    //Int64 num = 0;
                    //if (Int64.TryParse(DirSizeBytes(mov.RootDir), out num))
                    //{
                    //    //mov.DirSize = num.ToString(); 
                    //}

                    //mov.DirSize = ahk.FormatBytes(num);
                }

                bool GetExtraInfo = true;



                //imdbUrl = ahk.StringSplit(imdbUrl, "?", 0);  // trim down url to just required path

                string html = getUrlData(fixedURL + "combined");

                //ahk.MsgBox(html);

                //Id = match(@"<link rel=""canonical"" href=""http://www.imdb.com/title/(tt\d{7})/combined"" />", html);
                //if (Id == "") { Id = match(@"<link rel=""canonical"" href=""http://www.imdb.com/title/(tt\d{7})/"" />", html); }
                //if (Id == "") { Id = match(@"<link rel=""canonical"" href=""http://www.imdb.com/title/(tt\d{7})"" />", html); }

                mov.IMDbID = Id;
                if (!string.IsNullOrEmpty(Id))
                {
                    status = true;
                    mov.Title = match(@"<title>(IMDb \- )*(.*?) \(.*?</title>", html, 2);
                    mov.Title = ahk.StringReplace(mov.Title, "'", "''", "ALL");

                    mov.OriginalTitle = match(@"title-extra"">(.*?)<", html);
                    mov.OriginalTitle = ahk.StringReplace(mov.OriginalTitle, "'", "''", "ALL");

                    mov.Year = match(@"<title>.*?\(.*?(\d{4}).*?\).*?</title>", html);
                    mov.Rating = match(@"<b>(\d.\d)/10</b>", html);

                    Genres = matchAll(@"<a.*?>(.*?)</a>", match(@"Genre.?:(.*?)(</div>|See more)", html));
                    mov.Genres = ArrayList_ToString(Genres);

                    Directors = matchAll(@"<td valign=""top""><a.*?href=""/name/.*?/"">(.*?)</a>", match(@"Directed by</a></h5>(.*?)</table>", html));
                    mov.Directors = ArrayList_ToString(Directors);

                    Writers = matchAll(@"<td valign=""top""><a.*?href=""/name/.*?/"">(.*?)</a>", match(@"Writing credits</a></h5>(.*?)</table>", html));
                    mov.Writers = ArrayList_ToString(Writers);

                    Producers = matchAll(@"<td valign=""top""><a.*?href=""/name/.*?/"">(.*?)</a>", match(@"Produced by</a></h5>(.*?)</table>", html));
                    mov.Producers = ArrayList_ToString(Producers);

                    Musicians = matchAll(@"<td valign=""top""><a.*?href=""/name/.*?/"">(.*?)</a>", match(@"Original Music by</a></h5>(.*?)</table>", html));
                    mov.Musicians = ArrayList_ToString(Musicians);

                    Cinematographers = matchAll(@"<td valign=""top""><a.*?href=""/name/.*?/"">(.*?)</a>", match(@"Cinematography by</a></h5>(.*?)</table>", html));
                    mov.Cinematographers = ArrayList_ToString(Cinematographers);

                    Editors = matchAll(@"<td valign=""top""><a.*?href=""/name/.*?/"">(.*?)</a>", match(@"Film Editing by</a></h5>(.*?)</table>", html));
                    mov.Editors = ArrayList_ToString(Editors);

                    Cast = matchAll(@"<td class=""nm""><a.*?href=""/name/.*?/"".*?>(.*?)</a>", match(@"<h3>Cast</h3>(.*?)</table>", html));
                    mov.Cast = ArrayList_ToString(Cast);

                    mov.Plot = match(@"Plot:</h5>.*?<div class=""info-content"">(.*?)(<a|</div)", html);

                    PlotKeywords = matchAll(@"<a.*?>(.*?)</a>", match(@"Plot Keywords:</h5>.*?<div class=""info-content"">(.*?)</div", html));
                    mov.PlotKeywords = ArrayList_ToString(PlotKeywords);

                    mov.ReleaseDate = match(@"Release Date:</h5>.*?<div class=""info-content"">.*?(\d{1,2} (January|February|March|April|May|June|July|August|September|October|November|December) (19|20)\d{2})", html);
                    mov.Runtime = match(@"Runtime:</h5><div class=""info-content"">(\d{1,4}) min[\s]*.*?</div>", html);
                    mov.Top250 = match(@"Top 250: #(\d{1,3})<", html);
                    mov.Oscars = match(@"Won (\d+) Oscars?\.", html);
                    if (string.IsNullOrEmpty(Oscars) && "Won Oscar.".Equals(match(@"(Won Oscar\.)", html))) mov.Oscars = "1";
                    mov.Awards = match(@"(\d{1,4}) wins", html);
                    mov.Nominations = match(@"(\d{1,4}) nominations", html);
                    mov.Tagline = match(@"Tagline:</h5>.*?<div class=""info-content"">(.*?)(<a|</div)", html);
                    mov.MpaaRating = match(@"MPAA</a>:</h5><div class=""info-content"">Rated (G|PG|PG-13|PG-14|R|NC-17|X) ", html);
                    mov.Votes = match(@">(\d+,?\d*) votes<", html);

                    Languages = matchAll(@"<a.*?>(.*?)</a>", match(@"Language.?:(.*?)(</div>|>.?and )", html));
                    mov.Languages = ArrayList_ToString(Languages);

                    Countries = matchAll(@"<a.*?>(.*?)</a>", match(@"Country:(.*?)(</div>|>.?and )", html));
                    mov.Countries = ArrayList_ToString(Countries);

                    mov.Poster = match(@"<div class=""photo"">.*?<a name=""poster"".*?><img.*?src=""(.*?)"".*?</div>", html);
                    //if (!string.IsNullOrEmpty(Poster) && Poster.IndexOf("media-imdb.com") > 0)
                    if (mov.Poster.Contains("images-amazon.com"))
                    {
                        Poster = mov.Poster;

                        Poster = Regex.Replace(Poster, @"_V1.*?.jpg", "_V1._SY200.jpg");
                        if (Poster != "") { mov.Poster = Poster; }

                        PosterLarge = Regex.Replace(Poster, @"_V1.*?.jpg", "_V1._SY500.jpg");
                        if (PosterLarge != "") { mov.PosterLarge = PosterLarge; }

                        PosterFull = Regex.Replace(Poster, @"_V1.*?.jpg", "_V1._SY0.jpg");
                        if (PosterFull != "") { mov.PosterFull = PosterFull; }
                    }

                    ImdbURL = "http://www.imdb.com/title/" + Id + "/";

                    if (GetExtraInfo)
                    {
                        string plotHtml = getUrlData(imdbUrl + "plotsummary");
                        Storyline = match(@"<p class=""plotpar"">(.*?)(<i>|</p>)", plotHtml);
                        ReleaseDates = getReleaseDates();
                        MediaImages = getMediaImages();
                        RecommendedTitles = getRecommendedTitles();
                    }



                }


                if (mov.Title.Trim() == "") { mov.Title = mov.OriginalTitle; }

                mov = FixIMDbTitle(mov);

                // write movie to collection db 
                if (WriteMissingToDb) { imdB_UpdateInsert(mov); }

                return mov;
            }




            #endregion


            #region === IMDb Service ===

            _WinServices ws = new _WinServices();

            int serviceInsertCount = 0;
            int serviceUpdateCount = 0;
            int serviceErrorCount = 0;


            public void UpdateLocalDb_IMDbService(RadProgressBar Bar = null, RadProgressBar Bar2 = null, bool NewThread = true)
            {
                if (NewThread)
                {
                    Thread newThread = new Thread(() => UpdateLocalDb_IMDbService(Bar, Bar2, false)); // Function To Execute
                    newThread.Name = "Populate Missing Actor Info";
                    newThread.IsBackground = true;
                    newThread.Start();
                }
                else
                {
                    List<string> SearchDirs = new List<string>();

                    _Sites.OMDB omdb = new OMDB();

                    string SearchDir = "";
                    //string SearchDir = @"E:\[ Movies ]";

                    // reset global counters
                    imdbUpdateCount = 0;
                    imdbInsertCount = 0;

                    if (SearchDir != "")
                    {
                        SearchDirs.Add(SearchDir);
                    }
                    if (SearchDir == "")
                    {
                        SearchDirs.Add(@"E:\[ Movies ]\[ Movies A-K ]");
                        SearchDirs.Add(@"E:\[ Movies ]\[ Movies L-S ]");
                        SearchDirs.Add(@"E:\[ Movies ]\[ Movies T-Z ]");
                        SearchDirs.Add(@"I:\Movies_4");
                        SearchDirs.Add(@"N:\Movies_9");
                        SearchDirs.Add(@"R:\Movies_INCOMING");
                        SearchDirs.Add(@"Z:\[ Standup ]");
                    }

                    _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();

                    if (Bar != null)
                    {
                        pro.SetupProgressBar(Bar, SearchDirs.Count());
                        pro.UpdateProgress(Bar);
                    }

                    int i = 1;
                    foreach (string dirPath in SearchDirs)
                    {
                        if (!Directory.Exists(dirPath))
                        {
                            ServiceLog(dirPath + " Is INVALID DirPath", 0, 0, 1);
                            continue;
                        }

                        if (Bar != null)
                        {
                            pro.UpdateProgress(Bar, "IMDb Index Update : Collecting IMDb Links... [ " + i + " / " + SearchDirs.Count + " ]");
                        }

                        //sb.StatusBar("IMDb Index Update : Collecting IMDb Links... [ " + i + " / " + SearchDirs.Count + " ]");

                        string[] files = Directory.GetFiles(dirPath, "*.url", System.IO.SearchOption.AllDirectories);


                        ServiceLog(files.Count() + " URLs Found In " + dirPath, 0, 0, 0);

                        

                        if (Bar2 != null)
                        {
                            pro.ResetProgress(Bar2);
                            pro.SetupProgressBar(Bar2, files.Count());
                        }
                        

                        int count = 1;
                        foreach (string file in files)
                        {
                            if (Bar2 != null) { pro.UpdateProgress(Bar2, "Updating IMDb Movie Index: " + count.ToString() + " / " + files.Count().ToString() + " | Updated " + serviceUpdateCount + " | NEW " + serviceInsertCount); }

                            //sb.StatusBar("Updating IMDb Movie Index : " + count.ToString() + " / " + files.Count().ToString() + " | Updated " + imdbUpdateCount + " | NEW " + imdbInsertCount);

                            //if (file.Contains("[ Theater Rips ]")) { count++; continue; }
                            //if (file.Contains("[ INCOMING ]")) { count++; continue; }


                            string URLPath = ahk.IniRead(file, "InternetShortcut", "URL");
                            if (URLPath.ToUpper().Contains("IMDB.COM"))
                            {
                                // read imdb id from url, lookup movie info from OMDb API
                                imdB Movie = omdb.OMdb(ParseID(URLPath));

                                string fileDir = ahk.FileDir(file);
                                Movie.RootDir = fileDir;
                                //long dirsize = _AHK.DirSize(fileDir);
                                //Movie.DirSize = ahk.FormatBytes(dirsize);

                                Movie.DirSize = ahk.DirSize(fileDir);


                                // PREVIOUS VERSION - NOT AS ACCURATE FROM IMDB PARSE
                                //imdB Movie = parse_IMDb_Page(URLPath, file, SkipExisting, WriteMissingToDb);

                                if (Movie.Title == "" || Movie.Title == null) { continue; }

                                //imdB_UpdateInsert(Movie);

                                bool Updated = imdB_UpdateSQL(Movie);  // try to update record first
                                if (Updated) { serviceUpdateCount++; }

                                if (!Updated) // if unable to update, insert new record
                                {
                                    Updated = imdB_InsertSQL(Movie);

                                    if (Updated) { serviceInsertCount++; }
                                    else { serviceErrorCount++; }
                                }

                            }

                            count++;
                        }

                        i++;
                    }

                    ServiceLog("Finshed Processing IMDb Links", serviceInsertCount, serviceUpdateCount, serviceErrorCount);

                    //sb.StatusBar("Finished Updating IMDb Library | Updated " + imdbUpdateCount + " | NEW " + imdbInsertCount);
                }

            }

            public void ServiceLog(string Text, int Added = 0, int Updated = 0, int Errors = 0)
            {
                ServiceLog obj = new ServiceLog();
                obj.ServiceName = "IMDb_Service";
                obj.LogType = "LogEntry";
                obj.LogMessage = Text;
                obj.InsertCount = Added;
                obj.ErrorCount = Errors;
                obj.UpdateCount = Updated;

                ws.ServiceLog_InsertSQL(obj);

                //ws.ServiceConfig_CheckIn(serviceConfig);  // write update timestamp for service in db
            }





            /// <summary>
            /// Loop through root of dir containing movies, find list of folders without IMDb.url File
            /// </summary>
            /// <param name="RootDir"></param>
            public void FoldersWithoutIMDb(RadProgressBar Bar = null, RadProgressBar Bar2 = null, bool NewThread = true)
            {
                if (NewThread)
                {
                    Thread newThread = new Thread(() => FoldersWithoutIMDb(Bar, Bar2, false)); // Function To Execute
                    newThread.Name = "Looking for Missing IMDb";
                    newThread.IsBackground = true;
                    newThread.Start();
                }
                else
                {
                    // reset global counters
                    imdbUpdateCount = 0;
                    imdbInsertCount = 0;

                    List<string> SearchDirs = new List<string>();
                    SearchDirs.Add(@"E:\[ Movies ]\[ Movies A-K ]");
                    SearchDirs.Add(@"E:\[ Movies ]\[ Movies L-S ]");
                    SearchDirs.Add(@"E:\[ Movies ]\[ Movies T-Z ]");
                    SearchDirs.Add(@"I:\Movies_4");
                    SearchDirs.Add(@"I:\Movies_Incoming");
                    SearchDirs.Add(@"N:\Movies_9");
                    SearchDirs.Add(@"N:\Movies_Incoming");
                    SearchDirs.Add(@"R:\Movies_INCOMING");
                    //SearchDirs.Add(@"Z:\[ Standup ]");

                    _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();
                    if (Bar != null) { pro.SetupProgressBar(Bar, SearchDirs.Count()); pro.UpdateProgress(Bar); }

                    int i = 1;
                    foreach (string RootDir in SearchDirs)
                    {
                        if (Bar != null) { pro.UpdateProgress(Bar, "Looking for Missing IMDb URL Folders... [ " + i + " / " + SearchDirs.Count + " ]"); i++; }

                        List<string> MovieDirs = ahk.DirList(RootDir, "*.*", false, true);

                        if (Bar2 != null)
                        {
                            pro.ResetProgress(Bar2);
                            pro.SetupProgressBar(Bar2, MovieDirs.Count());
                        }

                        int count = 1;
                        foreach (string MovieDir in MovieDirs)
                        {
                            if (Bar2 != null) { pro.UpdateProgress(Bar2, "Updating IMDb Movie Index: " + count.ToString() + " / " + MovieDirs.Count().ToString()); count++; }

                            MissingURL obj = new MissingURL();
                            obj.Found = false;
                            obj.FolderPath = MovieDir;

                            List<string> files = Directory.GetFiles(MovieDir, "*.url", System.IO.SearchOption.AllDirectories).ToList();
                            foreach (string url in files)
                            {
                                string URLPath = ahk.IniRead(url, "InternetShortcut", "URL");
                                if (URLPath.ToUpper().Contains("IMDB.COM")) { obj.Found = true; }
                            }

                            if (!obj.Found)
                            {
                                MissingURL_UpdateInsert(obj);
                            }
                            
                        }
                    }
                }

            }

            public struct MissingURL
            {
                public string FolderPath { get; set; }
                public bool Found { get; set; }
                public DateTime Modified { get; set; }
                public string MovieName { get; set; }
                public string IMDb { get; set; }
            }


            public SqlConnection Conn(string ConnName = "LITM")
            {
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[ConnName].ConnectionString);
                return conn;
            }

            public bool MissingURL_InsertSQL(MissingURL obj)
            {
                SqlConnection Con = Conn();
                string SQLLine = "Insert Into [LucidMedia].[dbo].[IMDb_MissingURLs] (IMDb, MovieName, FolderPath, Found, Modified) VALUES (@IMDb, @MovieName, @FolderPath, @Found, @Modified)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);

                cmd2.Parameters.AddWithValue(@"FolderPath", obj.FolderPath);
                cmd2.Parameters.AddWithValue(@"Found", obj.Found);
                cmd2.Parameters.AddWithValue(@"IMDb", obj.IMDb);
                cmd2.Parameters.AddWithValue(@"MovieName", obj.MovieName);
                cmd2.Parameters.AddWithValue(@"Modified", DateTime.Now.ToString());

                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Con.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool MissingURL_UpdateSQL(MissingURL obj)
            {
                SqlConnection Con = Conn();

                string SQLLine = "Update [LucidMedia].[dbo].[IMDb_MissingURLs] SET [IMDb] = @IMDb, MovieName = @MovieName, [FolderPath] = @FolderPath, [Found] = @Found, [Modified] = @Modified WHERE [FolderPath] = @FolderPath";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);

                cmd2.Parameters.AddWithValue(@"FolderPath", obj.FolderPath);
                cmd2.Parameters.AddWithValue(@"Found", obj.Found);
                cmd2.Parameters.AddWithValue(@"IMDb", obj.IMDb);
                cmd2.Parameters.AddWithValue(@"MovieName", obj.MovieName);
                cmd2.Parameters.AddWithValue(@"Modified", DateTime.Now.ToString());

                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Con.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool MissingURL_UpdateInsert(MissingURL obj)
            {
                bool Updated = MissingURL_UpdateSQL(obj);  // try to update record first
                if (!Updated) { Updated = MissingURL_InsertSQL(obj); }  // if unable to update, insert new record
                return Updated;
            }

            public List<MissingURL> ReturnMissingIMDb(string Command = "select * FROM [LucidMedia].[dbo].[IMDb_MissingURLs] where Found = '0'")
            {
                SqlConnection Conn = ImdbEps_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<MissingURL> ReturnList = new List<MissingURL>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        MissingURL returnObject = new MissingURL();
                        returnObject.FolderPath = ret["FolderPath"].ToString();
                        returnObject.Found = ret["Found"].ToBool();
                        returnObject.MovieName = ret["MovieName"].ToString();
                        returnObject.IMDb = ret["IMDb"].ToString();
                        returnObject.Modified = ret["Modified"].ToDateTime();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }


            public void ProcessMovieNames()
            {
                List<MissingURL> missing = ReturnMissingIMDb();
                foreach(MissingURL miss in missing)
                {
                    MissingURL update = miss;

                    string name = ahk.DirName(miss.FolderPath);
                    name = name.Replace("(", "|");
                    name = name.Replace("[", "|");
                    name = ahk.StringSplit(name, "|", 0);

                    update.MovieName = name.Trim();

                    MissingURL_UpdateSQL(update);
                }
            }
            


            #endregion



            #region === IMDb Read / Write .URL File INI ===

            /// <summary>
            /// Search Directory for IMDb .URL Files - Lookup IMDb Info - Write Info to URL INI
            /// </summary>
            /// <param name="SearchDir"></param>
            /// <param name="progress1"></param>
            /// <param name="progress2"></param>
            public void IMDb_URL_SearchWrite(string SearchDir = "", Telerik.WinControls.UI.RadProgressBar progress1 = null, Telerik.WinControls.UI.RadProgressBar progress2 = null)
            {
                List<string> SearchDirs = new List<string>();

                SearchDir = @"E:\_Movies_3";

                // reset global counters
                imdbUpdateCount = 0;
                imdbInsertCount = 0;

                if (SearchDir != "")
                {
                    SearchDirs.Add(SearchDir);
                }
                if (SearchDir == "")
                {
                    //string MovieDir = @"M:";
                    ////MovieDir = @"F:\[ Movies ]";
                    //SearchDirs.Add(MovieDir + @"\[ Movies A-K ]");
                    //SearchDirs.Add(MovieDir + @"\[ Movies L-S ]");
                    //SearchDirs.Add(MovieDir + @"\[ Movies T-Z ]");
                    ////SearchDirs.Add(@"M:\[ Theater Rips ]");


                    TVServerLocal tvs = new TVServerLocal();

                    // list of local tv folders to search for folder names
                    SearchDirs = tvs.LocalMovieDirs();

                }


                //pro.ResetProgress(progress1);
                //pro.SetupProgressBar1(progress1, SearchDirs.Count());
                //pro.UpdateProgress(progress1);
                _StatusBar sb = new _StatusBar();
                int i = 1;
                foreach (string dirPath in SearchDirs)
                {
                    if (!Directory.Exists(dirPath)) { continue; }

                    _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();
                    _Sites.OMDB omdb = new OMDB();

                    sb.StatusBar("IMDb Index Update : Collecting IMDb Links... [ " + i + " / " + SearchDirs.Count + " ]");

                    string[] files = Directory.GetFiles(dirPath, "*.url", System.IO.SearchOption.AllDirectories);

                    pro.ResetProgress(progress1);
                    pro.SetupProgressBar(progress1, files.Count());

                    int count = 1;
                    foreach (string file in files)
                    {
                        pro.UpdateProgress(progress1);

                        sb.StatusBar("Updating IMDb Movie Index : " + count.ToString() + " / " + files.Count().ToString() + " | Updated " + imdbUpdateCount + " | NEW " + imdbInsertCount);

                        //if (file.Contains("[ Theater Rips ]")) { count++; continue; }
                        //if (file.Contains("[ INCOMING ]")) { count++; continue; }

                        // lookup imdb info - write as ini to .url file
                        IMDb_URL_Populate(file);

                        string URLPath = ahk.IniRead(file, "InternetShortcut", "URL");
                        if (URLPath.ToUpper().Contains("IMDB.COM"))
                        {
                            // read imdb id from url, lookup movie info from OMDb API
                            imdB Movie = omdb.OMdb(ParseID(URLPath));

                            string fileDir = ahk.FileDir(file);
                            Movie.RootDir = fileDir;
                            //long dirsize = _AHK.DirSize(fileDir);
                            //Movie.DirSize = ahk.FormatBytes(dirsize);
                            Movie.DirSize = ahk.DirSize(fileDir);

                            // PREVIOUS VERSION - NOT AS ACCURATE FROM IMDB PARSE
                            //imdB Movie = parse_IMDb_Page(URLPath, file, SkipExisting, WriteMissingToDb);

                            if (Movie.Title == "" || Movie.Title == null) { continue; }

                            if (Movie.IMDbID == null) { Movie.IMDbID = ""; }
                            if (Movie.Title == null) { Movie.Title = "_"; }
                            if (Movie.OriginalTitle == null) { Movie.OriginalTitle = "_"; }
                            if (Movie.Year == null) { Movie.Year = "_"; }
                            if (Movie.DirSize == null) { Movie.DirSize = "_"; }
                            if (Movie.Awards == null) { Movie.Awards = "_"; }
                            if (Movie.Cast == null) { Movie.Cast = "_"; }
                            if (Movie.Cinematographers == null) { Movie.Cinematographers = "_"; }
                            if (Movie.Countries == null) { Movie.Countries = "_"; }
                            if (Movie.Directors == null) { Movie.Directors = "_"; }
                            if (Movie.Editors == null) { Movie.Editors = "_"; }
                            if (Movie.Genres == null) { Movie.Genres = "_"; }
                            if (Movie.Languages == null) { Movie.Languages = "_"; }
                            if (Movie.MpaaRating == null) { Movie.MpaaRating = "_"; }
                            if (Movie.Musicians == null) { Movie.Musicians = "_"; }
                            if (Movie.Nominations == null) { Movie.Nominations = "_"; }
                            if (Movie.Oscars == null) { Movie.Oscars = "_"; }
                            if (Movie.Plot == null) { Movie.Plot = "_"; }
                            if (Movie.PlotKeywords == null) { Movie.PlotKeywords = "_"; }
                            if (Movie.Poster == null) { Movie.Poster = "_"; }
                            if (Movie.PosterFull == null) { Movie.PosterFull = "_"; }
                            if (Movie.Producers == null) { Movie.Producers = "_"; }
                            if (Movie.Rating == null) { Movie.Rating = "_"; }
                            if (Movie.ReleaseDate == null) { Movie.ReleaseDate = "_"; }
                            if (Movie.RootDir == null) { Movie.RootDir = "_"; }
                            if (Movie.Runtime == null) { Movie.Runtime = "_"; }
                            if (Movie.Tagline == null) { Movie.Tagline = "_"; }
                            if (Movie.Top250 == null) { Movie.Top250 = "_"; }
                            if (Movie.Votes == null) { Movie.Votes = "_"; }
                            if (Movie.Writers == null) { Movie.Writers = "_"; }

                            ahk.IniWrite(Movie.IMDbID, file, "IMDb", "ID");
                            ahk.IniWrite(Movie.Title, file, "IMDb", "Title");
                            ahk.IniWrite(Movie.OriginalTitle, file, "IMDb", "Original_Title");
                            ahk.IniWrite(Movie.Year, file, "IMDb", "Year");
                            ahk.IniWrite(Movie.DirSize, file, "IMDb", "DirSize");
                            ahk.IniWrite(Movie.Awards, file, "IMDb", "Awards");
                            ahk.IniWrite(Movie.Cast, file, "IMDb", "Cast");
                            ahk.IniWrite(Movie.Cinematographers, file, "IMDb", "Cinematographers");
                            ahk.IniWrite(Movie.Countries, file, "IMDb", "Countries");
                            ahk.IniWrite(Movie.Directors, file, "IMDb", "Directors");
                            ahk.IniWrite(Movie.Editors, file, "IMDb", "Editors");
                            ahk.IniWrite(Movie.Genres, file, "IMDb", "Genres");
                            ahk.IniWrite(Movie.Languages, file, "IMDb", "Languages");
                            ahk.IniWrite(Movie.MpaaRating, file, "IMDb", "MPAARating");
                            ahk.IniWrite(Movie.Musicians, file, "IMDb", "Musicians");
                            ahk.IniWrite(Movie.Nominations, file, "IMDb", "Nominations");
                            ahk.IniWrite(Movie.Oscars, file, "IMDb", "Oscars");
                            ahk.IniWrite(Movie.Plot, file, "IMDb", "Plot");
                            ahk.IniWrite(Movie.PlotKeywords, file, "IMDb", "PlotKeywords");
                            ahk.IniWrite(Movie.Poster, file, "IMDb", "Poster");
                            ahk.IniWrite(Movie.PosterFull, file, "IMDb", "PosterFull");
                            ahk.IniWrite(Movie.Producers, file, "IMDb", "Producers");
                            ahk.IniWrite(Movie.Rating, file, "IMDb", "Rating");
                            ahk.IniWrite(Movie.ReleaseDate, file, "IMDb", "ReleaseDate");
                            ahk.IniWrite(Movie.RootDir, file, "IMDb", "RootDir");
                            ahk.IniWrite(Movie.Runtime, file, "IMDb", "RunTime");
                            ahk.IniWrite(Movie.Tagline, file, "IMDb", "TagLine");
                            ahk.IniWrite(Movie.Top250, file, "IMDb", "Top250");
                            ahk.IniWrite(Movie.Votes, file, "IMDb", "Votes");
                            ahk.IniWrite(Movie.Writers, file, "IMDb", "Writers");


                            //imdB_UpdateInsert(Movie);
                        }

                        count++;
                    }

                    i++;
                }


                 sb.StatusBar("Finished Updating IMDb Library | Updated " + imdbUpdateCount + " | NEW " + imdbInsertCount);
            }


            /// <summary>
            /// Lookup IMDb Title using .URL File with IMDB.COM Link - Writes IMDb Info to URL File
            /// </summary>
            /// <param name="IMDbURLPath">Path to IMDb .URL File</param>
            public void IMDb_URL_Populate(string IMDbURLPath)
            {
                _Sites.OMDB omdb = new OMDB();
                string file = IMDbURLPath;

                string URLPath = ahk.IniRead(file, "InternetShortcut", "URL");
                if (URLPath.ToUpper().Contains("IMDB.COM"))
                {
                    // read imdb id from url, lookup movie info from OMDb API
                    imdB Movie = omdb.OMdb(ParseID(URLPath));

                    string fileDir = ahk.FileDir(file);
                    Movie.RootDir = fileDir;

                    //long dirsize = _AHK.DirSize(fileDir);
                    //Movie.DirSize = ahk.FormatBytes(dirsize);
                    Movie.DirSize = ahk.DirSize(fileDir);

                    // PREVIOUS VERSION - NOT AS ACCURATE FROM IMDB PARSE
                    //imdB Movie = parse_IMDb_Page(URLPath, file, SkipExisting, WriteMissingToDb);


                    if (Movie.Title == null) { Movie.Title = ""; }
                    if (Movie.IMDbID == null) { Movie.IMDbID = ""; }
                    if (Movie.Title == null) { Movie.Title = "_"; }
                    if (Movie.OriginalTitle == null) { Movie.OriginalTitle = "_"; }
                    if (Movie.Year == null) { Movie.Year = "_"; }
                    if (Movie.DirSize == null) { Movie.DirSize = "_"; }
                    if (Movie.Awards == null) { Movie.Awards = "_"; }
                    if (Movie.Cast == null) { Movie.Cast = "_"; }
                    if (Movie.Cinematographers == null) { Movie.Cinematographers = "_"; }
                    if (Movie.Countries == null) { Movie.Countries = "_"; }
                    if (Movie.Directors == null) { Movie.Directors = "_"; }
                    if (Movie.Editors == null) { Movie.Editors = "_"; }
                    if (Movie.Genres == null) { Movie.Genres = "_"; }
                    if (Movie.Languages == null) { Movie.Languages = "_"; }
                    if (Movie.MpaaRating == null) { Movie.MpaaRating = "_"; }
                    if (Movie.Musicians == null) { Movie.Musicians = "_"; }
                    if (Movie.Nominations == null) { Movie.Nominations = "_"; }
                    if (Movie.Oscars == null) { Movie.Oscars = "_"; }
                    if (Movie.Plot == null) { Movie.Plot = "_"; }
                    if (Movie.PlotKeywords == null) { Movie.PlotKeywords = "_"; }
                    if (Movie.Poster == null) { Movie.Poster = "_"; }
                    if (Movie.PosterFull == null) { Movie.PosterFull = "_"; }
                    if (Movie.Producers == null) { Movie.Producers = "_"; }
                    if (Movie.Rating == null) { Movie.Rating = "_"; }
                    if (Movie.ReleaseDate == null) { Movie.ReleaseDate = "_"; }
                    if (Movie.RootDir == null) { Movie.RootDir = "_"; }
                    if (Movie.Runtime == null) { Movie.Runtime = "_"; }
                    if (Movie.Tagline == null) { Movie.Tagline = "_"; }
                    if (Movie.Top250 == null) { Movie.Top250 = "_"; }
                    if (Movie.Votes == null) { Movie.Votes = "_"; }
                    if (Movie.Writers == null) { Movie.Writers = "_"; }

                    ahk.IniWrite(Movie.IMDbID, file, "IMDb", "ID");
                    ahk.IniWrite(Movie.Title, file, "IMDb", "Title");
                    ahk.IniWrite(Movie.OriginalTitle, file, "IMDb", "Original_Title");
                    ahk.IniWrite(Movie.Year, file, "IMDb", "Year");
                    ahk.IniWrite(Movie.DirSize, file, "IMDb", "DirSize");
                    ahk.IniWrite(Movie.Awards, file, "IMDb", "Awards");
                    ahk.IniWrite(Movie.Cast, file, "IMDb", "Cast");
                    ahk.IniWrite(Movie.Cinematographers, file, "IMDb", "Cinematographers");
                    ahk.IniWrite(Movie.Countries, file, "IMDb", "Countries");
                    ahk.IniWrite(Movie.Directors, file, "IMDb", "Directors");
                    ahk.IniWrite(Movie.Editors, file, "IMDb", "Editors");
                    ahk.IniWrite(Movie.Genres, file, "IMDb", "Genres");
                    ahk.IniWrite(Movie.Languages, file, "IMDb", "Languages");
                    ahk.IniWrite(Movie.MpaaRating, file, "IMDb", "MPAARating");
                    ahk.IniWrite(Movie.Musicians, file, "IMDb", "Musicians");
                    ahk.IniWrite(Movie.Nominations, file, "IMDb", "Nominations");
                    ahk.IniWrite(Movie.Oscars, file, "IMDb", "Oscars");
                    ahk.IniWrite(Movie.Plot, file, "IMDb", "Plot");
                    ahk.IniWrite(Movie.PlotKeywords, file, "IMDb", "PlotKeywords");
                    ahk.IniWrite(Movie.Poster, file, "IMDb", "Poster");
                    ahk.IniWrite(Movie.PosterFull, file, "IMDb", "PosterFull");
                    ahk.IniWrite(Movie.Producers, file, "IMDb", "Producers");
                    ahk.IniWrite(Movie.Rating, file, "IMDb", "Rating");
                    ahk.IniWrite(Movie.ReleaseDate, file, "IMDb", "ReleaseDate");
                    ahk.IniWrite(Movie.RootDir, file, "IMDb", "RootDir");
                    ahk.IniWrite(Movie.Runtime, file, "IMDb", "RunTime");
                    ahk.IniWrite(Movie.Tagline, file, "IMDb", "TagLine");
                    ahk.IniWrite(Movie.Top250, file, "IMDb", "Top250");
                    ahk.IniWrite(Movie.Votes, file, "IMDb", "Votes");
                    ahk.IniWrite(Movie.Writers, file, "IMDb", "Writers");


                    //imdB_UpdateInsert(Movie);
                }



            }

            /// <summary>
            /// Read Local IMDb URL Info to IMDb Object
            /// </summary>
            /// <param name="FilePath">Path to Local .URL File Containing IMDb INI</param>
            /// <returns>Returns IMDb Object Populated from URL File</returns>
            public imdB IMDb_URL_Read(string FilePath)
            {
                imdB Movie = new imdB();

                Movie.IMDbID = ahk.IniRead(FilePath, "IMDb", "ID");
                Movie.Title = ahk.IniRead(FilePath, "IMDb", "Title");
                Movie.OriginalTitle = ahk.IniRead(FilePath, "IMDb", "Original_Title");
                Movie.Year = ahk.IniRead(FilePath, "IMDb", "Year");
                Movie.DirSize = ahk.IniRead(FilePath, "IMDb", "DirSize");
                Movie.Awards = ahk.IniRead(FilePath, "IMDb", "Awards");
                Movie.Cast = ahk.IniRead(FilePath, "IMDb", "Cast");
                Movie.Cinematographers = ahk.IniRead(FilePath, "IMDb", "Cinematographers");
                Movie.Countries = ahk.IniRead(FilePath, "IMDb", "Countries");
                Movie.Directors = ahk.IniRead(FilePath, "IMDb", "Directors");
                Movie.Editors = ahk.IniRead(FilePath, "IMDb", "Editors");
                Movie.Genres = ahk.IniRead(FilePath, "IMDb", "Genres");
                Movie.Languages = ahk.IniRead(FilePath, "IMDb", "Languages");
                Movie.MpaaRating = ahk.IniRead(FilePath, "IMDb", "MPAARating");
                Movie.Musicians = ahk.IniRead(FilePath, "IMDb", "Musicians");
                Movie.Nominations = ahk.IniRead(FilePath, "IMDb", "Nominations");
                Movie.Oscars = ahk.IniRead(FilePath, "IMDb", "Oscars");
                Movie.Plot = ahk.IniRead(FilePath, "IMDb", "Plot");
                Movie.PlotKeywords = ahk.IniRead(FilePath, "IMDb", "PlotKeywords");
                Movie.Poster = ahk.IniRead(FilePath, "IMDb", "Poster");
                Movie.PosterFull = ahk.IniRead(FilePath, "IMDb", "PosterFull");
                Movie.Producers = ahk.IniRead(FilePath, "IMDb", "Producers");
                Movie.Rating = ahk.IniRead(FilePath, "IMDb", "Rating");
                Movie.ReleaseDate = ahk.IniRead(FilePath, "IMDb", "ReleaseDate");
                Movie.RootDir = ahk.IniRead(FilePath, "IMDb", "RootDir");
                Movie.Runtime = ahk.IniRead(FilePath, "IMDb", "RunTime");
                Movie.Tagline = ahk.IniRead(FilePath, "IMDb", "TagLine");
                Movie.Top250 = ahk.IniRead(FilePath, "IMDb", "Top250");
                Movie.Votes = ahk.IniRead(FilePath, "IMDb", "Votes");
                Movie.Writers = ahk.IniRead(FilePath, "IMDb", "Writers");

                return Movie;
            }

            /// <summary>
            /// Writes / Updates IMDb URL File with IMDb Object
            /// </summary>
            /// <param name="Movie">Populated IMDb Object to Write/Update</param>
            /// <param name="FilePath">Path to .URL File or Directory Path, If Directory Creates New IMDb.Com URL File</param>
            public bool IMDb_URL_Write(imdB Movie, string Dir_or_FilePath = "")
            {
                if (Movie.Title == null) { Movie.Title = ""; }
                if (Movie.IMDbID == null) { Movie.IMDbID = ""; }
                if (Movie.Title == null) { Movie.Title = "_"; }
                if (Movie.OriginalTitle == null) { Movie.OriginalTitle = "_"; }
                if (Movie.Year == null) { Movie.Year = "_"; }
                if (Movie.DirSize == null) { Movie.DirSize = "_"; }
                if (Movie.Awards == null) { Movie.Awards = "_"; }
                if (Movie.Cast == null) { Movie.Cast = "_"; }
                if (Movie.Cinematographers == null) { Movie.Cinematographers = "_"; }
                if (Movie.Countries == null) { Movie.Countries = "_"; }
                if (Movie.Directors == null) { Movie.Directors = "_"; }
                if (Movie.Editors == null) { Movie.Editors = "_"; }
                if (Movie.Genres == null) { Movie.Genres = "_"; }
                if (Movie.Languages == null) { Movie.Languages = "_"; }
                if (Movie.MpaaRating == null) { Movie.MpaaRating = "_"; }
                if (Movie.Musicians == null) { Movie.Musicians = "_"; }
                if (Movie.Nominations == null) { Movie.Nominations = "_"; }
                if (Movie.Oscars == null) { Movie.Oscars = "_"; }
                if (Movie.Plot == null) { Movie.Plot = "_"; }
                if (Movie.PlotKeywords == null) { Movie.PlotKeywords = "_"; }
                if (Movie.Poster == null) { Movie.Poster = "_"; }
                if (Movie.PosterFull == null) { Movie.PosterFull = "_"; }
                if (Movie.Producers == null) { Movie.Producers = "_"; }
                if (Movie.Rating == null) { Movie.Rating = "_"; }
                if (Movie.ReleaseDate == null) { Movie.ReleaseDate = "_"; }
                if (Movie.RootDir == null) { Movie.RootDir = "_"; }
                if (Movie.Runtime == null) { Movie.Runtime = "_"; }
                if (Movie.Tagline == null) { Movie.Tagline = "_"; }
                if (Movie.Top250 == null) { Movie.Top250 = "_"; }
                if (Movie.Votes == null) { Movie.Votes = "_"; }
                if (Movie.Writers == null) { Movie.Writers = "_"; }


                string file = Dir_or_FilePath;
                if (Dir_or_FilePath.IsDir()) // If Directory Passed in, Create new URL File with IMDb.Com URL Entry
                {
                    file = file + "\\" + Movie.Title + " (" + Movie.Year + ").url";
                    bool WroteURL = ahk.IniWrite("http://www.imdb.com/title/" + Movie.IMDbID + "/", file, "InternetShortcut", "URL");
                }


                ahk.IniWrite(Movie.IMDbID, file, "IMDb", "ID");
                ahk.IniWrite(Movie.Title, file, "IMDb", "Title");
                ahk.IniWrite(Movie.OriginalTitle, file, "IMDb", "Original_Title");
                ahk.IniWrite(Movie.Year, file, "IMDb", "Year");
                ahk.IniWrite(Movie.DirSize, file, "IMDb", "DirSize");
                ahk.IniWrite(Movie.Awards, file, "IMDb", "Awards");
                ahk.IniWrite(Movie.Cast, file, "IMDb", "Cast");
                ahk.IniWrite(Movie.Cinematographers, file, "IMDb", "Cinematographers");
                ahk.IniWrite(Movie.Countries, file, "IMDb", "Countries");
                ahk.IniWrite(Movie.Directors, file, "IMDb", "Directors");
                ahk.IniWrite(Movie.Editors, file, "IMDb", "Editors");
                ahk.IniWrite(Movie.Genres, file, "IMDb", "Genres");
                ahk.IniWrite(Movie.Languages, file, "IMDb", "Languages");
                ahk.IniWrite(Movie.MpaaRating, file, "IMDb", "MPAARating");
                ahk.IniWrite(Movie.Musicians, file, "IMDb", "Musicians");
                ahk.IniWrite(Movie.Nominations, file, "IMDb", "Nominations");
                ahk.IniWrite(Movie.Oscars, file, "IMDb", "Oscars");
                ahk.IniWrite(Movie.Plot, file, "IMDb", "Plot");
                ahk.IniWrite(Movie.PlotKeywords, file, "IMDb", "PlotKeywords");
                ahk.IniWrite(Movie.Poster, file, "IMDb", "Poster");
                ahk.IniWrite(Movie.PosterFull, file, "IMDb", "PosterFull");
                ahk.IniWrite(Movie.Producers, file, "IMDb", "Producers");
                ahk.IniWrite(Movie.Rating, file, "IMDb", "Rating");
                ahk.IniWrite(Movie.ReleaseDate, file, "IMDb", "ReleaseDate");
                ahk.IniWrite(Movie.RootDir, file, "IMDb", "RootDir");
                ahk.IniWrite(Movie.Runtime, file, "IMDb", "RunTime");
                ahk.IniWrite(Movie.Tagline, file, "IMDb", "TagLine");
                ahk.IniWrite(Movie.Top250, file, "IMDb", "Top250");
                ahk.IniWrite(Movie.Votes, file, "IMDb", "Votes");
                ahk.IniWrite(Movie.Writers, file, "IMDb", "Writers");

                if (File.Exists(file)) { return true; }
                return false;
            }


            /// <summary>
            /// Create .URL Link File for IMDb.com Page
            /// </summary>
            /// <param name="SaveDir"></param>
            /// <param name="ImdbID"></param>
            /// <returns></returns>
            public bool Write_IMDb_URL(string SaveDir, string ImdbID, string Title, string Year)
            {
                string urlFilePath = SaveDir + "\\" + Title + " (" + Year + ").url";
                bool WroteURL = ahk.IniWrite("http://www.imdb.com/title/" + ImdbID + "/", urlFilePath, "InternetShortcut", "URL");
                return WroteURL;
            }

            public void Create_IMDb_Link(string IMDbID, string SaveDir = "", string selectedMovieFilePath = "", bool SingleImport = false)
            {
                string url = "http://www.imdb.com/title/" + IMDbID + "/";
                imdB movieInfo = parse_IMDb_Page(url, "", false);

                if (SaveDir == "") { SaveDir = @"D:\_Incoming\_Movies\_ToFix"; }

                string newDirName = SaveDir + "\\" + movieInfo.Title + " (" + movieInfo.Year + ")";
                string pathFixedN = newDirName.Replace("&#x27;", "'");
                pathFixedN = pathFixedN.Replace("?", "");
                pathFixedN = pathFixedN.Replace(",", " ");
                ahk.FileCreateDir(pathFixedN);

                //    [InternetShortcut]
                //    URL=http://www.imdb.com/title/tt0117918/?ref_=fn_al_tt_1

                ahk.IniWrite(url, newDirName + "\\" + movieInfo.Title + " (" + movieInfo.Year + ") - IMDb.url", "InternetShortcut", "URL");

                //string selectedFile = txtLocalFileSelect.Text;

                string pathFixed = selectedMovieFilePath.Replace("&#x27;", "'");
                pathFixed = pathFixed.Replace("?", "");
                pathFixed = pathFixed.Replace(",", " ");

                ahk.FileMove(selectedMovieFilePath, pathFixedN + "\\" + ahk.FileName(pathFixed));

                if (SingleImport)
                {
                    ahk.OpenDir(newDirName);
                    //btnLoadLocalFiles_Click(); // reload local filelist grid after file move
                }
            }

            /// <summary>
            /// Read .URL File, Check to see if IMDb Site Link
            /// </summary>
            /// <param name="URLFilePath"></param>
            /// <returns></returns>
            public bool IsIMDbURL(string URLFilePath)
            {
                string URLPath = ahk.IniRead(URLFilePath, "InternetShortcut", "URL");
                if (URLPath.ToUpper().Contains("IMDB.COM"))
                {
                    return true;
                }
                return false;
            }



            #endregion


            #region === IMDb Misc ===

            // fix hex characters returned from IMDb Search
            public imdB FixIMDbTitle(imdB Movie)
            {
                //string fixedTitle = MovieTitle.Replace("&#xE9;", "é");
                //fixedTitle = fixedTitle.Replace("&#xF6;", "ö");
                //fixedTitle = fixedTitle.Replace("&#xE4;", "ä");
                //fixedTitle = fixedTitle.Replace("&#x22;", "\"");
                //fixedTitle = fixedTitle.Replace("&#x27;", "'");
                //fixedTitle = fixedTitle.Replace("?", "");
                //fixedTitle = fixedTitle.Replace(":", "-");
                //fixedTitle = fixedTitle.Replace("&#xE8;", "è");

                string fixedTitle = "";
                bool swapped = false;
                StringWriter myWriter = new StringWriter();

                if (Movie.Title != null)
                {
                    fixedTitle = Movie.Title.Replace(",", "");
                    fixedTitle = fixedTitle.Replace("?", "");
                    fixedTitle = fixedTitle.Replace(":", "-");

                    
                    // Decode the encoded string.
                    HttpUtility.HtmlDecode(fixedTitle, myWriter);
                    Movie.Title = myWriter.ToString();


                    // swap the original title with the title entry in imdb object
                    if (Movie.Title == "Alexandre Ajas Maniac") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Bébé part en vadrouille") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Daunbailó") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title.Trim() == "") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Der Frosch mit der Maske") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Die Todesreise") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Eine Armee Gretchen") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "El superviviente") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Fidelio l'odyssée d'Alice") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Fuse- teppô musume no torimonochô") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Ji-geum-eun-mat-go-geu-ddae-neun-teul-li-da") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "La leyenda de Nicholas Nickleby") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Las garras de Lorelei") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Minä olen tuomari ") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Nyû shinema paradaisu") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Olsen-bandens store kup") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Ondas da Vida") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Paiement cash") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Sasuperia Teruza saigo no majo") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Scintilla") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Shadô") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Soonjung") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Szökevények nyomában") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Tyttö nimeltä Varpu") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Was bleibt") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Вторжение- Битва за рай") { Movie = swapTitles(Movie); swapped = true; }
                    if (Movie.Title == "Оно живёт") { Movie = swapTitles(Movie); swapped = true; }

                    if (Movie.Title != null)
                    {
                        // if movie title was swapped with original title, fix new title before returning
                        if (swapped)
                        {
                            fixedTitle = Movie.Title.Replace(",", "");
                            fixedTitle = fixedTitle.Replace("?", "");
                            fixedTitle = fixedTitle.Replace(":", "-");

                            myWriter = new StringWriter();
                            // Decode the encoded string.
                            HttpUtility.HtmlDecode(fixedTitle, myWriter);
                            Movie.Title = myWriter.ToString();
                        }
                    }
                }


                return Movie;
            }


            #endregion


            #region === IMDb ===


            /// <summary>
            /// Extracts list of IMDb Links From Page HTML
            /// </summary>
            /// <param name="HTML"></param>
            /// <returns></returns>
            public List<string> Regex_IMDbLinks(string HTML)
            {
                List<string> matches = new List<string>();

                string cmd = @"(http|ftp|https)://([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?";

                Regex ItemRegex = new Regex(cmd, RegexOptions.IgnoreCase);
                foreach (Match ItemMatch in ItemRegex.Matches(HTML))
                {
                    //Console.WriteLine(ItemMatch);
                    string val = ItemMatch.Value;
                    if (val.ToLower().Contains("imdb.com")) { matches.Add(val); }
                }


                return matches;
            }


            // === Parse Assist ===
            private string ArrayList_ToString(ArrayList array)
            {
                int i = 0;
                string outTxt = "";
                foreach (string ar in array)
                {
                    if (i == 0) { outTxt = ar; }
                    if (i != 0) { outTxt = outTxt + "|" + ar; }
                    i++;
                }

                return outTxt;
            }


            // returns IMDb Entry by IMDb ID - tries sql db first, then looks up on imdb.com and writes to sql if missing from sql db
            public imdB IMDb_Title(string IMDbID)
            {
                return parse_IMDb_Page("http://www.imdb.com/title/" + IMDbID + "/", "", true, false);
            }


            // Parse IMDb Page from ID - Returns Movie Object Parsed from IMDB.com
            public _Sites.Imdb.imdB IMDb_Page(string imdbID)
            {
                return parse_IMDb_Page("http://www.imdb.com/title/" + imdbID, "", false, false);
            }


            //Parse IMDb page data
            // option to not update existing imdb entries - faster
            public _Sites.Imdb.imdB Parse_IMDb_Page(string imdbUrl, string imdbLinkFile = "", bool WriteToDb = true, bool skipExisting = false)
            {
                _Sites.Imdb.imdB mov = new _Sites.Imdb.imdB();


                mov.RootDir = "";
                mov.DirSize = "";
                Id = IMDb_ID_FromURL(imdbUrl);   // parses imdb url, extras title id

                if (skipExisting)
                {
                    // check to see if title already found in sql db
                    bool AlreadyInDb = inDb(Id);
                    if (AlreadyInDb)
                    {
                        _StatusBar sb = new _StatusBar();
                        sb.StatusBar(Id + " Already Found In Db - Skipping...");
                        return mov;
                    }
                }

                if (imdbLinkFile != "")
                {
                    mov.RootDir = ahk.FileDir(imdbLinkFile);
                    //mov.RootDir = ahk.StringReplace(mov.RootDir, "'", "`'", "ALL"); 

                    //mov.DirSize = ahk.DirSize(mov.RootDir).ToString();


                    string dirBytes = ahk.DirSize(mov.RootDir);
                    //Int64 num = 0;
                    //if (Int64.TryParse(dirBytes, out num))
                    //{
                    //    //mov.DirSize = num.ToString(); 
                    //}

                    //mov.DirSize = ahk.FormatBytes(num);

                    mov.DirSize = dirBytes;
                }

                bool GetExtraInfo = true;



                imdbUrl = ahk.StringSplit(imdbUrl, "?", 0);  // trim down url to just required path

                string html = getUrlData(imdbUrl + "combined");

                if (html == "")
                {
                    html = getUrlData(imdbUrl + "/combined");  // add missing slash if needed
                }

                //ahk.MsgBox(html);

                //Id = match(@"<link rel=""canonical"" href=""http://www.imdb.com/title/(tt\d{7})/combined"" />", html);
                //if (Id == "") { Id = match(@"<link rel=""canonical"" href=""http://www.imdb.com/title/(tt\d{7})/"" />", html); }
                //if (Id == "") { Id = match(@"<link rel=""canonical"" href=""http://www.imdb.com/title/(tt\d{7})"" />", html); }

                mov.IMDbID = Id;
                if (!string.IsNullOrEmpty(Id))
                {
                    status = true;
                    mov.Title = match(@"<title>(IMDb \- )*(.*?) \(.*?</title>", html, 2);
                    mov.Title = ahk.StringReplace(mov.Title, "'", "''", "ALL");

                    mov.OriginalTitle = match(@"title-extra"">(.*?)<", html);
                    mov.OriginalTitle = ahk.StringReplace(mov.OriginalTitle, "'", "''", "ALL");

                    mov.Year = match(@"<title>.*?\(.*?(\d{4}).*?\).*?</title>", html);
                    mov.Rating = match(@"<b>(\d.\d)/10</b>", html);

                    Genres = matchAll(@"<a.*?>(.*?)</a>", match(@"Genre.?:(.*?)(</div>|See more)", html));
                    mov.Genres = ArrayList_ToString(Genres);

                    Directors = matchAll(@"<td valign=""top""><a.*?href=""/name/.*?/"">(.*?)</a>", match(@"Directed by</a></h5>(.*?)</table>", html));
                    mov.Directors = ArrayList_ToString(Directors);

                    Writers = matchAll(@"<td valign=""top""><a.*?href=""/name/.*?/"">(.*?)</a>", match(@"Writing credits</a></h5>(.*?)</table>", html));
                    mov.Writers = ArrayList_ToString(Writers);

                    Producers = matchAll(@"<td valign=""top""><a.*?href=""/name/.*?/"">(.*?)</a>", match(@"Produced by</a></h5>(.*?)</table>", html));
                    mov.Producers = ArrayList_ToString(Producers);

                    Musicians = matchAll(@"<td valign=""top""><a.*?href=""/name/.*?/"">(.*?)</a>", match(@"Original Music by</a></h5>(.*?)</table>", html));
                    mov.Musicians = ArrayList_ToString(Musicians);

                    Cinematographers = matchAll(@"<td valign=""top""><a.*?href=""/name/.*?/"">(.*?)</a>", match(@"Cinematography by</a></h5>(.*?)</table>", html));
                    mov.Cinematographers = ArrayList_ToString(Cinematographers);

                    Editors = matchAll(@"<td valign=""top""><a.*?href=""/name/.*?/"">(.*?)</a>", match(@"Film Editing by</a></h5>(.*?)</table>", html));
                    mov.Editors = ArrayList_ToString(Editors);

                    Cast = matchAll(@"<td class=""nm""><a.*?href=""/name/.*?/"".*?>(.*?)</a>", match(@"<h3>Cast</h3>(.*?)</table>", html));
                    mov.Cast = ArrayList_ToString(Cast);

                    mov.Plot = match(@"Plot:</h5>.*?<div class=""info-content"">(.*?)(<a|</div)", html);

                    PlotKeywords = matchAll(@"<a.*?>(.*?)</a>", match(@"Plot Keywords:</h5>.*?<div class=""info-content"">(.*?)</div", html));
                    mov.PlotKeywords = ArrayList_ToString(PlotKeywords);

                    mov.ReleaseDate = match(@"Release Date:</h5>.*?<div class=""info-content"">.*?(\d{1,2} (January|February|March|April|May|June|July|August|September|October|November|December) (19|20)\d{2})", html);
                    mov.Runtime = match(@"Runtime:</h5><div class=""info-content"">(\d{1,4}) min[\s]*.*?</div>", html);
                    mov.Top250 = match(@"Top 250: #(\d{1,3})<", html);
                    mov.Oscars = match(@"Won (\d+) Oscars?\.", html);
                    if (string.IsNullOrEmpty(Oscars) && "Won Oscar.".Equals(match(@"(Won Oscar\.)", html))) mov.Oscars = "1";
                    mov.Awards = match(@"(\d{1,4}) wins", html);
                    mov.Nominations = match(@"(\d{1,4}) nominations", html);
                    mov.Tagline = match(@"Tagline:</h5>.*?<div class=""info-content"">(.*?)(<a|</div)", html);
                    mov.MpaaRating = match(@"MPAA</a>:</h5><div class=""info-content"">Rated (G|PG|PG-13|PG-14|R|NC-17|X) ", html);
                    mov.Votes = match(@">(\d+,?\d*) votes<", html);

                    Languages = matchAll(@"<a.*?>(.*?)</a>", match(@"Language.?:(.*?)(</div>|>.?and )", html));
                    mov.Languages = ArrayList_ToString(Languages);

                    Countries = matchAll(@"<a.*?>(.*?)</a>", match(@"Country:(.*?)(</div>|>.?and )", html));
                    mov.Countries = ArrayList_ToString(Countries);

                    mov.Poster = match(@"<div class=""photo"">.*?<a name=""poster"".*?><img.*?src=""(.*?)"".*?</div>", html);
                    //if (!string.IsNullOrEmpty(Poster) && Poster.IndexOf("media-imdb.com") > 0)
                    if (mov.Poster.Contains("images-amazon.com"))
                    {
                        Poster = mov.Poster;

                        Poster = Regex.Replace(Poster, @"_V1.*?.jpg", "_V1._SY200.jpg");
                        if (Poster != "") { mov.Poster = Poster; }

                        PosterLarge = Regex.Replace(Poster, @"_V1.*?.jpg", "_V1._SY500.jpg");
                        if (PosterLarge != "") { mov.PosterLarge = PosterLarge; }

                        PosterFull = Regex.Replace(Poster, @"_V1.*?.jpg", "_V1._SY0.jpg");
                        if (PosterFull != "") { mov.Poster = PosterFull; }
                    }
                    else
                    {
                        string oldPosterRef = mov.Poster;

                        Poster = string.Empty;
                        PosterLarge = string.Empty;
                        PosterFull = string.Empty;
                        mov.Poster = "";
                    }
                    ImdbURL = "http://www.imdb.com/title/" + Id + "/";

                    if (GetExtraInfo)
                    {
                        string plotHtml = getUrlData(imdbUrl + "plotsummary");
                        if (plotHtml == "")
                        {
                            plotHtml = getUrlData(imdbUrl + "/plotsummary");
                        }

                        Storyline = match(@"<p class=""plotpar"">(.*?)(<i>|</p>)", plotHtml);
                        ReleaseDates = getReleaseDates();
                        MediaImages = getMediaImages();
                        RecommendedTitles = getRecommendedTitles();
                    }



                }


                if (WriteToDb) { imdB_UpdateInsert(mov); }

                return mov;
            }


            // swap the original title with the title entry in imdb object
            public imdB swapTitles(imdB Movie)
            {
                string tmp = Movie.OriginalTitle;
                string tmp2 = Movie.Title;
                Movie.Title = tmp;
                Movie.OriginalTitle = tmp2;
                return Movie;
            }


            public string FormatDBTextIn(string InText)
            {
                if (InText == null) { return ""; }

                string OutText = InText.Replace(",", "");  //format text to write to datatbase 
                string OutText2 = OutText.Replace("`", "");  //format text to write to datatbase 
                OutText = OutText2.Replace("\"", "");  //format text to write to datatbase 

                return OutText;
            }


            #region === IMDb Scraper ===

            /*******************************************************************************
             * Free ASP.net IMDb Scraper API for the new IMDb Template.
             * Author: Abhinay Rathore
             * Website: http://www.AbhinayRathore.com
             * Blog: http://web3o.blogspot.com
             * More Info: http://web3o.blogspot.com/2010/11/aspnetc-imdb-scraping-api.html
             * Last Updated: Feb 20, 2013
             *******************************************************************************/

            //Match single instance
            private string match(string regex, string html, int i = 1)
            {
                return new Regex(regex, RegexOptions.Multiline).Match(html).Groups[i].Value.Trim();
            }

            //Get URL Data
            private string getUrlData(string url)
            {
                if (url == "http://www.imdb.com/findcombined") { return ""; }
                if (url == "http://www.imdb.com/findplotsummary") { return ""; }
                if (url == "http://www.imdb.com/title/http:www.imdb.comfind/releaseinfo") { return ""; }



                //string HTML = Download_HTML(url);
                //return HTML; 

                WebClient client = new WebClient();
                Random r = new Random();
                //Random IP Address
                client.Headers["X-Forwarded-For"] = r.Next(0, 255) + "." + r.Next(0, 255) + "." + r.Next(0, 255) + "." + r.Next(0, 255);
                //Random User-Agent
                client.Headers["User-Agent"] = "Mozilla/" + r.Next(3, 5) + ".0 (Windows NT " + r.Next(3, 5) + "." + r.Next(0, 2) + "; rv:2.0.1) Gecko/20100101 Firefox/" + r.Next(3, 5) + "." + r.Next(0, 5) + "." + r.Next(0, 5);
                try
                {
                    System.IO.Stream datastream = client.OpenRead(url);
                    StreamReader reader = new StreamReader(datastream);
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    while (!reader.EndOfStream)
                        sb.Append(reader.ReadLine());
                    return sb.ToString();
                }
                catch
                {
                    return "";
                }

            }

            //Get IMDb URL from search results
            private string getIMDbUrl(string MovieName, string searchEngine = "google")
            {
                string url = GoogleSearch + MovieName; //default to Google search
                if (searchEngine.ToLower().Equals("bing")) url = BingSearch + MovieName;
                if (searchEngine.ToLower().Equals("ask")) url = AskSearch + MovieName;
                string html = getUrlData(url);
                ArrayList imdbUrls = matchAll(@"<a href=""(http://www.imdb.com/title/tt\d{7}/)"".*?>.*?</a>", html);
                if (imdbUrls.Count > 0)
                    return (string)imdbUrls[0]; //return first IMDb result
                else if (searchEngine.ToLower().Equals("google")) //if Google search fails
                    return getIMDbUrl(MovieName, "bing"); //search using Bing
                else if (searchEngine.ToLower().Equals("bing")) //if Bing search fails
                    return getIMDbUrl(MovieName, "ask"); //search using Ask
                else //search fails
                    return string.Empty;
            }

            //Get all release dates
            private ArrayList getReleaseDates()
            {
                ArrayList list = new ArrayList();
                string releasehtml = getUrlData("http://www.imdb.com/title/" + Id + "/releaseinfo");
                foreach (string r in matchAll(@"<tr>(.*?)</tr>", match(@"Date</th></tr>\n*?(.*?)</table>", releasehtml)))
                {
                    Match rd = new Regex(@"<td>(.*?)</td>\n*?.*?<td align=""right"">(.*?)</td>", RegexOptions.Multiline).Match(r);
                    list.Add(StripHTML(rd.Groups[1].Value.Trim()) + " = " + StripHTML(rd.Groups[2].Value.Trim()));
                }
                return list;
            }

            //Get all media images
            private ArrayList getMediaImages()
            {
                ArrayList list = new ArrayList();
                string mediaurl = "http://www.imdb.com/title/" + Id + "/mediaindex";
                string mediahtml = getUrlData(mediaurl);
                int pagecount = matchAll(@"<a href=""\?page=(.*?)"">", match(@"<span style=""padding: 0 1em;"">(.*?)</span>", mediahtml)).Count;
                for (int p = 1; p <= pagecount + 1; p++)
                {
                    mediahtml = getUrlData(mediaurl + "?page=" + p);
                    foreach (Match m in new Regex(@"src=""(.*?)""", RegexOptions.Multiline).Matches(match(@"<div class=""thumb_list"" style=""font-size: 0px;"">(.*?)</div>", mediahtml)))
                    {
                        String image = m.Groups[1].Value;
                        list.Add(Regex.Replace(image, @"_V1\..*?.jpg", "_V1._SY0.jpg"));
                    }
                }
                return list;
            }

            //Get Recommended Titles
            private ArrayList getRecommendedTitles()
            {
                ArrayList list = new ArrayList();
                string recUrl = "http://www.imdb.com/widget/recommendations/_ajax/get_more_recs?specs=p13nsims%3A" + Id;
                string json = getUrlData(recUrl);
                list = matchAll(@"title=\\""(.*?)\\""", json);
                HashSet<String> set = new HashSet<string>();
                foreach (String rec in list) set.Add(rec);
                return new ArrayList(set.ToList());
            }

            /*******************************[ Helper Methods ]********************************/

            //Match all instances and return as ArrayList
            private ArrayList matchAll(string regex, string html, int i = 1)
            {
                ArrayList list = new ArrayList();
                foreach (Match m in new Regex(regex, RegexOptions.Multiline).Matches(html))
                    list.Add(m.Groups[i].Value.Trim());
                return list;
            }

            //Strip HTML Tags
            static string StripHTML(string inputString)
            {
                return Regex.Replace(inputString, @"<.*?>", string.Empty);
            }


            // IMDB Setup

            public bool status { get; set; }
            public string Id { get; set; }
            public string Title { get; set; }
            public string OriginalTitle { get; set; }
            public string Year { get; set; }
            public string Rating { get; set; }
            public ArrayList Genres { get; set; }
            public ArrayList Directors { get; set; }
            public ArrayList Writers { get; set; }
            public ArrayList Cast { get; set; }
            public ArrayList Producers { get; set; }
            public ArrayList Musicians { get; set; }
            public ArrayList Cinematographers { get; set; }
            public ArrayList Editors { get; set; }
            public string MpaaRating { get; set; }
            public string ReleaseDate { get; set; }
            public string Plot { get; set; }
            public ArrayList PlotKeywords { get; set; }
            public string Poster { get; set; }
            public string PosterLarge { get; set; }
            public string PosterFull { get; set; }
            public string Runtime { get; set; }
            public string Top250 { get; set; }
            public string Oscars { get; set; }
            public string Awards { get; set; }
            public string Nominations { get; set; }
            public string Storyline { get; set; }
            public string Tagline { get; set; }
            public string Votes { get; set; }
            public ArrayList Languages { get; set; }
            public ArrayList Countries { get; set; }
            public ArrayList ReleaseDates { get; set; }
            public ArrayList MediaImages { get; set; }
            public ArrayList RecommendedTitles { get; set; }
            public string ImdbURL { get; set; }

            //Search Engine URLs
            private string GoogleSearch = "http://www.google.com/search?q=imdb+";
            private string BingSearch = "http://www.bing.com/search?q=imdb+";
            private string AskSearch = "http://www.ask.com/web?q=imdb+";



            #region === imdB Object ===

            public struct imdB
            {
                public string IMDbID { get; set; }
                public string Title { get; set; }
                public string OriginalTitle { get; set; }
                public string Year { get; set; }
                public string Rating { get; set; }
                public string Genres { get; set; }
                public string Directors { get; set; }
                public string Writers { get; set; }
                public string Producers { get; set; }
                public string Musicians { get; set; }
                public string Cinematographers { get; set; }
                public string Editors { get; set; }
                public string Cast { get; set; }
                public string Plot { get; set; }
                public string PlotKeywords { get; set; }
                public string ReleaseDate { get; set; }
                public string Runtime { get; set; }
                public string Top250 { get; set; }
                public string Oscars { get; set; }
                public string Awards { get; set; }
                public string Nominations { get; set; }
                public string Tagline { get; set; }
                public string MpaaRating { get; set; }
                public string Votes { get; set; }
                public string Languages { get; set; }
                public string Countries { get; set; }
                public string Poster { get; set; }
                public string PosterFull { get; set; }
                public string PosterLarge { get; set; }
                public string RootDir { get; set; }
                public string DirSize { get; set; }
                public string PosterImage { get; set; }
            }


            /// <summary>
            /// Encodes Title Text into Database Compatible Storage
            /// </summary>
            /// <param name="TitleText"></param>
            /// <returns></returns>
            public string Encode(string TitleText)
            {
                if (TitleText == null) { return ""; }
                var encoded = HttpUtility.HtmlEncode(TitleText);
                return encoded.ToString();
            }

            /// <summary>
            /// Decodes Encoded Text into Display Text
            /// </summary>
            /// <param name="TitleText"></param>
            /// <returns></returns>
            public string Decode(string TitleText)
            {
                if (TitleText == null) { return ""; }
                StringWriter myWriter = new StringWriter();
                HttpUtility.HtmlDecode(TitleText, myWriter);  // Decode the encoded string.
                return myWriter.ToString();
            }


            //  Fix illegal characters before Sql/Sqlite Db Inserts
            public imdB imdB_FixChars(imdB ToFix)
            {
                imdB Fixed = new imdB();

                Fixed.IMDbID = ToFix.IMDbID.Replace("'", "''");
                Fixed.Title = ToFix.Title.Replace("'", "''");
                Fixed.OriginalTitle = ToFix.OriginalTitle.Replace("'", "''");
                Fixed.Year = ToFix.Year.Replace("'", "''");
                Fixed.Rating = ToFix.Rating.Replace("'", "''");
                Fixed.Genres = ToFix.Genres.Replace("'", "''");
                Fixed.Directors = ToFix.Directors.Replace("'", "''");
                Fixed.Writers = ToFix.Writers.Replace("'", "''");
                Fixed.Producers = ToFix.Producers.Replace("'", "''");
                Fixed.Musicians = ToFix.Musicians.Replace("'", "''");
                Fixed.Cinematographers = ToFix.Cinematographers.Replace("'", "''");
                Fixed.Editors = ToFix.Editors.Replace("'", "''");
                Fixed.Cast = ToFix.Cast.Replace("'", "''");
                Fixed.Plot = ToFix.Plot.Replace("'", "''");
                Fixed.PlotKeywords = ToFix.PlotKeywords.Replace("'", "''");
                Fixed.ReleaseDate = ToFix.ReleaseDate.Replace("'", "''");
                Fixed.Runtime = ToFix.Runtime.Replace("'", "''");
                Fixed.Top250 = ToFix.Top250.Replace("'", "''");
                Fixed.Oscars = ToFix.Oscars.Replace("'", "''");
                Fixed.Awards = ToFix.Awards.Replace("'", "''");
                Fixed.Nominations = ToFix.Nominations.Replace("'", "''");
                Fixed.Tagline = ToFix.Tagline.Replace("'", "''");
                Fixed.MpaaRating = ToFix.MpaaRating.Replace("'", "''");
                Fixed.Votes = ToFix.Votes.Replace("'", "''");
                Fixed.Languages = ToFix.Languages.Replace("'", "''");
                Fixed.Countries = ToFix.Countries.Replace("'", "''");
                Fixed.Poster = ToFix.Poster.Replace("'", "''");

                return Fixed;
            }

            // Compare two objects to see if they have identical values
            public bool imdB_Changed(imdB OldVal, imdB NewVal)
            {
                imdB diff = new imdB();
                List<string> diffList = new List<string>();
                bool different = false;
                if (OldVal.IMDbID == null) { OldVal.IMDbID = ""; }
                if (NewVal.IMDbID == null) { NewVal.IMDbID = ""; }
                if (OldVal.IMDbID != NewVal.IMDbID) { different = true; }
                if (OldVal.Title == null) { OldVal.Title = ""; }
                if (NewVal.Title == null) { NewVal.Title = ""; }
                if (OldVal.Title != NewVal.Title) { different = true; }
                if (OldVal.OriginalTitle == null) { OldVal.OriginalTitle = ""; }
                if (NewVal.OriginalTitle == null) { NewVal.OriginalTitle = ""; }
                if (OldVal.OriginalTitle != NewVal.OriginalTitle) { different = true; }
                if (OldVal.Year == null) { OldVal.Year = ""; }
                if (NewVal.Year == null) { NewVal.Year = ""; }
                if (OldVal.Year != NewVal.Year) { different = true; }
                if (OldVal.Rating == null) { OldVal.Rating = ""; }
                if (NewVal.Rating == null) { NewVal.Rating = ""; }
                if (OldVal.Rating != NewVal.Rating) { different = true; }
                if (OldVal.Genres == null) { OldVal.Genres = ""; }
                if (NewVal.Genres == null) { NewVal.Genres = ""; }
                if (OldVal.Genres != NewVal.Genres) { different = true; }
                if (OldVal.Directors == null) { OldVal.Directors = ""; }
                if (NewVal.Directors == null) { NewVal.Directors = ""; }
                if (OldVal.Directors != NewVal.Directors) { different = true; }
                if (OldVal.Writers == null) { OldVal.Writers = ""; }
                if (NewVal.Writers == null) { NewVal.Writers = ""; }
                if (OldVal.Writers != NewVal.Writers) { different = true; }
                if (OldVal.Producers == null) { OldVal.Producers = ""; }
                if (NewVal.Producers == null) { NewVal.Producers = ""; }
                if (OldVal.Producers != NewVal.Producers) { different = true; }
                if (OldVal.Musicians == null) { OldVal.Musicians = ""; }
                if (NewVal.Musicians == null) { NewVal.Musicians = ""; }
                if (OldVal.Musicians != NewVal.Musicians) { different = true; }
                if (OldVal.Cinematographers == null) { OldVal.Cinematographers = ""; }
                if (NewVal.Cinematographers == null) { NewVal.Cinematographers = ""; }
                if (OldVal.Cinematographers != NewVal.Cinematographers) { different = true; }
                if (OldVal.Editors == null) { OldVal.Editors = ""; }
                if (NewVal.Editors == null) { NewVal.Editors = ""; }
                if (OldVal.Editors != NewVal.Editors) { different = true; }
                if (OldVal.Cast == null) { OldVal.Cast = ""; }
                if (NewVal.Cast == null) { NewVal.Cast = ""; }
                if (OldVal.Cast != NewVal.Cast) { different = true; }
                if (OldVal.Plot == null) { OldVal.Plot = ""; }
                if (NewVal.Plot == null) { NewVal.Plot = ""; }
                if (OldVal.Plot != NewVal.Plot) { different = true; }
                if (OldVal.PlotKeywords == null) { OldVal.PlotKeywords = ""; }
                if (NewVal.PlotKeywords == null) { NewVal.PlotKeywords = ""; }
                if (OldVal.PlotKeywords != NewVal.PlotKeywords) { different = true; }
                if (OldVal.ReleaseDate == null) { OldVal.ReleaseDate = ""; }
                if (NewVal.ReleaseDate == null) { NewVal.ReleaseDate = ""; }
                if (OldVal.ReleaseDate != NewVal.ReleaseDate) { different = true; }
                if (OldVal.Runtime == null) { OldVal.Runtime = ""; }
                if (NewVal.Runtime == null) { NewVal.Runtime = ""; }
                if (OldVal.Runtime != NewVal.Runtime) { different = true; }
                if (OldVal.Top250 == null) { OldVal.Top250 = ""; }
                if (NewVal.Top250 == null) { NewVal.Top250 = ""; }
                if (OldVal.Top250 != NewVal.Top250) { different = true; }
                if (OldVal.Oscars == null) { OldVal.Oscars = ""; }
                if (NewVal.Oscars == null) { NewVal.Oscars = ""; }
                if (OldVal.Oscars != NewVal.Oscars) { different = true; }
                if (OldVal.Awards == null) { OldVal.Awards = ""; }
                if (NewVal.Awards == null) { NewVal.Awards = ""; }
                if (OldVal.Awards != NewVal.Awards) { different = true; }
                if (OldVal.Nominations == null) { OldVal.Nominations = ""; }
                if (NewVal.Nominations == null) { NewVal.Nominations = ""; }
                if (OldVal.Nominations != NewVal.Nominations) { different = true; }
                if (OldVal.Tagline == null) { OldVal.Tagline = ""; }
                if (NewVal.Tagline == null) { NewVal.Tagline = ""; }
                if (OldVal.Tagline != NewVal.Tagline) { different = true; }
                if (OldVal.MpaaRating == null) { OldVal.MpaaRating = ""; }
                if (NewVal.MpaaRating == null) { NewVal.MpaaRating = ""; }
                if (OldVal.MpaaRating != NewVal.MpaaRating) { different = true; }
                if (OldVal.Votes == null) { OldVal.Votes = ""; }
                if (NewVal.Votes == null) { NewVal.Votes = ""; }
                if (OldVal.Votes != NewVal.Votes) { different = true; }
                if (OldVal.Languages == null) { OldVal.Languages = ""; }
                if (NewVal.Languages == null) { NewVal.Languages = ""; }
                if (OldVal.Languages != NewVal.Languages) { different = true; }
                if (OldVal.Countries == null) { OldVal.Countries = ""; }
                if (NewVal.Countries == null) { NewVal.Countries = ""; }
                if (OldVal.Countries != NewVal.Countries) { different = true; }
                if (OldVal.Poster == null) { OldVal.Poster = ""; }
                if (NewVal.Poster == null) { NewVal.Poster = ""; }
                if (OldVal.Poster != NewVal.Poster) { different = true; }
                return different;
            }

            // Returns object containing the new values different from the old values in object comparison
            public imdB imdB_Diff(imdB OldVal, imdB NewVal)
            {
                imdB diff = new imdB();
                if (OldVal.IMDbID != NewVal.IMDbID) { diff.IMDbID = NewVal.IMDbID; }
                if (OldVal.Title != NewVal.Title) { diff.Title = NewVal.Title; }
                if (OldVal.OriginalTitle != NewVal.OriginalTitle) { diff.OriginalTitle = NewVal.OriginalTitle; }
                if (OldVal.Year != NewVal.Year) { diff.Year = NewVal.Year; }
                if (OldVal.Rating != NewVal.Rating) { diff.Rating = NewVal.Rating; }
                if (OldVal.Genres != NewVal.Genres) { diff.Genres = NewVal.Genres; }
                if (OldVal.Directors != NewVal.Directors) { diff.Directors = NewVal.Directors; }
                if (OldVal.Writers != NewVal.Writers) { diff.Writers = NewVal.Writers; }
                if (OldVal.Producers != NewVal.Producers) { diff.Producers = NewVal.Producers; }
                if (OldVal.Musicians != NewVal.Musicians) { diff.Musicians = NewVal.Musicians; }
                if (OldVal.Cinematographers != NewVal.Cinematographers) { diff.Cinematographers = NewVal.Cinematographers; }
                if (OldVal.Editors != NewVal.Editors) { diff.Editors = NewVal.Editors; }
                if (OldVal.Cast != NewVal.Cast) { diff.Cast = NewVal.Cast; }
                if (OldVal.Plot != NewVal.Plot) { diff.Plot = NewVal.Plot; }
                if (OldVal.PlotKeywords != NewVal.PlotKeywords) { diff.PlotKeywords = NewVal.PlotKeywords; }
                if (OldVal.ReleaseDate != NewVal.ReleaseDate) { diff.ReleaseDate = NewVal.ReleaseDate; }
                if (OldVal.Runtime != NewVal.Runtime) { diff.Runtime = NewVal.Runtime; }
                if (OldVal.Top250 != NewVal.Top250) { diff.Top250 = NewVal.Top250; }
                if (OldVal.Oscars != NewVal.Oscars) { diff.Oscars = NewVal.Oscars; }
                if (OldVal.Awards != NewVal.Awards) { diff.Awards = NewVal.Awards; }
                if (OldVal.Nominations != NewVal.Nominations) { diff.Nominations = NewVal.Nominations; }
                if (OldVal.Tagline != NewVal.Tagline) { diff.Tagline = NewVal.Tagline; }
                if (OldVal.MpaaRating != NewVal.MpaaRating) { diff.MpaaRating = NewVal.MpaaRating; }
                if (OldVal.Votes != NewVal.Votes) { diff.Votes = NewVal.Votes; }
                if (OldVal.Languages != NewVal.Languages) { diff.Languages = NewVal.Languages; }
                if (OldVal.Countries != NewVal.Countries) { diff.Countries = NewVal.Countries; }
                if (OldVal.Poster != NewVal.Poster) { diff.Poster = NewVal.Poster; }
                return diff;
            }

            // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
            public List<string> imdB_DiffList(imdB OldVal, imdB NewVal)
            {
                List<string> diffList = new List<string>();
                if (OldVal.IMDbID != NewVal.IMDbID) { diffList.Add("Changed IMDbID Value From " + OldVal.IMDbID + " To " + NewVal.IMDbID); }
                if (OldVal.Title != NewVal.Title) { diffList.Add("Changed Title Value From " + OldVal.Title + " To " + NewVal.Title); }
                if (OldVal.OriginalTitle != NewVal.OriginalTitle) { diffList.Add("Changed OriginalTitle Value From " + OldVal.OriginalTitle + " To " + NewVal.OriginalTitle); }
                if (OldVal.Year != NewVal.Year) { diffList.Add("Changed Year Value From " + OldVal.Year + " To " + NewVal.Year); }
                if (OldVal.Rating != NewVal.Rating) { diffList.Add("Changed Rating Value From " + OldVal.Rating + " To " + NewVal.Rating); }
                if (OldVal.Genres != NewVal.Genres) { diffList.Add("Changed Genres Value From " + OldVal.Genres + " To " + NewVal.Genres); }
                if (OldVal.Directors != NewVal.Directors) { diffList.Add("Changed Directors Value From " + OldVal.Directors + " To " + NewVal.Directors); }
                if (OldVal.Writers != NewVal.Writers) { diffList.Add("Changed Writers Value From " + OldVal.Writers + " To " + NewVal.Writers); }
                if (OldVal.Producers != NewVal.Producers) { diffList.Add("Changed Producers Value From " + OldVal.Producers + " To " + NewVal.Producers); }
                if (OldVal.Musicians != NewVal.Musicians) { diffList.Add("Changed Musicians Value From " + OldVal.Musicians + " To " + NewVal.Musicians); }
                if (OldVal.Cinematographers != NewVal.Cinematographers) { diffList.Add("Changed Cinematographers Value From " + OldVal.Cinematographers + " To " + NewVal.Cinematographers); }
                if (OldVal.Editors != NewVal.Editors) { diffList.Add("Changed Editors Value From " + OldVal.Editors + " To " + NewVal.Editors); }
                if (OldVal.Cast != NewVal.Cast) { diffList.Add("Changed Cast Value From " + OldVal.Cast + " To " + NewVal.Cast); }
                if (OldVal.Plot != NewVal.Plot) { diffList.Add("Changed Plot Value From " + OldVal.Plot + " To " + NewVal.Plot); }
                if (OldVal.PlotKeywords != NewVal.PlotKeywords) { diffList.Add("Changed PlotKeywords Value From " + OldVal.PlotKeywords + " To " + NewVal.PlotKeywords); }
                if (OldVal.ReleaseDate != NewVal.ReleaseDate) { diffList.Add("Changed ReleaseDate Value From " + OldVal.ReleaseDate + " To " + NewVal.ReleaseDate); }
                if (OldVal.Runtime != NewVal.Runtime) { diffList.Add("Changed Runtime Value From " + OldVal.Runtime + " To " + NewVal.Runtime); }
                if (OldVal.Top250 != NewVal.Top250) { diffList.Add("Changed Top250 Value From " + OldVal.Top250 + " To " + NewVal.Top250); }
                if (OldVal.Oscars != NewVal.Oscars) { diffList.Add("Changed Oscars Value From " + OldVal.Oscars + " To " + NewVal.Oscars); }
                if (OldVal.Awards != NewVal.Awards) { diffList.Add("Changed Awards Value From " + OldVal.Awards + " To " + NewVal.Awards); }
                if (OldVal.Nominations != NewVal.Nominations) { diffList.Add("Changed Nominations Value From " + OldVal.Nominations + " To " + NewVal.Nominations); }
                if (OldVal.Tagline != NewVal.Tagline) { diffList.Add("Changed Tagline Value From " + OldVal.Tagline + " To " + NewVal.Tagline); }
                if (OldVal.MpaaRating != NewVal.MpaaRating) { diffList.Add("Changed MpaaRating Value From " + OldVal.MpaaRating + " To " + NewVal.MpaaRating); }
                if (OldVal.Votes != NewVal.Votes) { diffList.Add("Changed Votes Value From " + OldVal.Votes + " To " + NewVal.Votes); }
                if (OldVal.Languages != NewVal.Languages) { diffList.Add("Changed Languages Value From " + OldVal.Languages + " To " + NewVal.Languages); }
                if (OldVal.Countries != NewVal.Countries) { diffList.Add("Changed Countries Value From " + OldVal.Countries + " To " + NewVal.Countries); }
                if (OldVal.Poster != NewVal.Poster) { diffList.Add("Changed Poster Value From " + OldVal.Poster + " To " + NewVal.Poster); }
                return diffList;
            }


            #endregion

            #region === imdB SQL Functions ===


            /// <summary>
            /// Return SQL Connection From Config File 
            /// </summary>
            /// <param name="GoDad">Default = False - Returns SQLserver Connection</param>
            /// <returns></returns>
            public SqlConnection imdB_Conn(string ConnName = "SQLserver|epGuideDb|LITMLucidMedia")
            {
                //if (ConnName == "SQLserver|epGuideDb|LITMLucidMedia") { ConnName = "LITMLucidMedia"; }

                return new SqlConnection(ConfigurationManager.ConnectionStrings["LITMLucidMedia"].ConnectionString);

                //if (GoDad) { return new SqlConnection(ConfigurationManager.ConnectionStrings["epGuideDb"].ConnectionString); }
                //return new SqlConnection(ConfigurationManager.ConnectionStrings["SQLserver"].ConnectionString);
            }

            // Return imdB TableName (Full Path)
            public string imdB_TableName()
            {
                // populate to return full sql table name
                return "[LucidMedia].[dbo].[LITM_IMDb]";
            }


            public bool imdB_InsertSQL(imdB obj)
            {
                SqlConnection Con = imdB_Conn();

                string SQLLine = "Insert Into " + imdB_TableName() + "(IMDbID, Title, OriginalTitle, Year, Rating, Genres, Directors, Writers, Producers, Musicians, Cinematographers, Editors, Cast, Plot, PlotKeywords, ReleaseDate, Runtime, Top250, Oscars, Awards, Nominations, Tagline, MpaaRating, Votes, Languages, Countries, Poster, RootDir, DirSize) VALUES (@IMDbID, @Title, @OriginalTitle, @Year, @Rating, @Genres, @Directors, @Writers, @Producers, @Musicians, @Cinematographers, @Editors, @Cast, @Plot, @PlotKeywords, @ReleaseDate, @Runtime, @Top250, @Oscars, @Awards, @Nominations, @Tagline, @MpaaRating, @Votes, @Languages, @Countries, @Poster, @RootDir, @DirSize)";


                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.IMDbID == null) { obj.IMDbID = ""; }
                if (obj.Title == null) { obj.Title = ""; }
                if (obj.OriginalTitle == null) { obj.OriginalTitle = ""; }
                if (obj.Year == null) { obj.Year = ""; }
                if (obj.Rating == null) { obj.Rating = ""; }
                if (obj.Genres == null) { obj.Genres = ""; }
                if (obj.Directors == null) { obj.Directors = ""; }
                if (obj.Writers == null) { obj.Writers = ""; }
                if (obj.Producers == null) { obj.Producers = ""; }
                if (obj.Musicians == null) { obj.Musicians = ""; }
                if (obj.Cinematographers == null) { obj.Cinematographers = ""; }
                if (obj.Editors == null) { obj.Editors = ""; }
                if (obj.Cast == null) { obj.Cast = ""; }
                if (obj.Plot == null) { obj.Plot = ""; }
                if (obj.PlotKeywords == null) { obj.PlotKeywords = ""; }
                if (obj.ReleaseDate == null) { obj.ReleaseDate = ""; }
                if (obj.Runtime == null) { obj.Runtime = ""; }
                if (obj.Top250 == null) { obj.Top250 = ""; }
                if (obj.Oscars == null) { obj.Oscars = ""; }
                if (obj.Awards == null) { obj.Awards = ""; }
                if (obj.Nominations == null) { obj.Nominations = ""; }
                if (obj.Tagline == null) { obj.Tagline = ""; }
                if (obj.MpaaRating == null) { obj.MpaaRating = ""; }
                if (obj.Votes == null) { obj.Votes = ""; }
                if (obj.Languages == null) { obj.Languages = ""; }
                if (obj.Countries == null) { obj.Countries = ""; }
                if (obj.Poster == null) { obj.Poster = ""; }

                cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID.ToString());
                cmd2.Parameters.AddWithValue(@"Title", obj.Title.ToString());
                cmd2.Parameters.AddWithValue(@"OriginalTitle", obj.OriginalTitle.ToString());
                cmd2.Parameters.AddWithValue(@"Year", obj.Year.ToString());
                cmd2.Parameters.AddWithValue(@"Rating", obj.Rating.ToString());
                cmd2.Parameters.AddWithValue(@"Genres", obj.Genres.ToString());
                cmd2.Parameters.AddWithValue(@"Directors", obj.Directors.ToString());
                cmd2.Parameters.AddWithValue(@"Writers", obj.Writers.ToString());
                cmd2.Parameters.AddWithValue(@"Producers", obj.Producers.ToString());
                cmd2.Parameters.AddWithValue(@"Musicians", obj.Musicians.ToString());
                cmd2.Parameters.AddWithValue(@"Cinematographers", obj.Cinematographers.ToString());
                cmd2.Parameters.AddWithValue(@"Editors", obj.Editors.ToString());
                cmd2.Parameters.AddWithValue(@"Cast", obj.Cast.ToString());
                cmd2.Parameters.AddWithValue(@"Plot", obj.Plot.ToString());
                cmd2.Parameters.AddWithValue(@"PlotKeywords", obj.PlotKeywords.ToString());
                cmd2.Parameters.AddWithValue(@"ReleaseDate", obj.ReleaseDate.ToString());
                cmd2.Parameters.AddWithValue(@"Runtime", obj.Runtime.ToString());
                cmd2.Parameters.AddWithValue(@"Top250", obj.Top250.ToString());
                cmd2.Parameters.AddWithValue(@"Oscars", obj.Oscars.ToString());
                cmd2.Parameters.AddWithValue(@"Awards", obj.Awards.ToString());
                cmd2.Parameters.AddWithValue(@"Nominations", obj.Nominations.ToString());
                cmd2.Parameters.AddWithValue(@"Tagline", obj.Tagline.ToString());
                cmd2.Parameters.AddWithValue(@"MpaaRating", obj.MpaaRating.ToString());
                cmd2.Parameters.AddWithValue(@"Votes", obj.Votes.ToString());
                cmd2.Parameters.AddWithValue(@"Languages", obj.Languages.ToString());
                cmd2.Parameters.AddWithValue(@"Countries", obj.Countries.ToString());
                cmd2.Parameters.AddWithValue(@"Poster", obj.Poster.ToString());
                cmd2.Parameters.AddWithValue(@"RootDir", obj.RootDir.ToString());
                cmd2.Parameters.AddWithValue(@"DirSize", obj.DirSize.ToString());

                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        imdB_InsertSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        imdB_InsertSQL(obj);
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

            public bool imdB_UpdateSQL(imdB obj)
            {
                SqlConnection Conn = imdB_Conn();


                string SQLLine = "Update " + imdB_TableName() + " SET Title = @Title, OriginalTitle = @OriginalTitle, Year = @Year, Rating = @Rating, Genres = @Genres, Directors = @Directors, Writers = @Writers, Producers = @Producers, Musicians = @Musicians, Cinematographers = @Cinematographers, Editors = @Editors, Cast = @Cast, Plot = @Plot, PlotKeywords = @PlotKeywords, ReleaseDate = @ReleaseDate, Runtime = @Runtime, Top250 = @Top250, Oscars = @Oscars, Awards = @Awards, Nominations = @Nominations, Tagline = @Tagline, MpaaRating = @MpaaRating, Votes = @Votes, Languages = @Languages, Countries = @Countries, Poster = @Poster, RootDir = @RootDir, DirSize = @DirSize WHERE IMDbID = @IMDbID";

                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);

                if (obj.IMDbID == null) { obj.IMDbID = ""; }
                if (obj.Title == null) { obj.Title = ""; }
                if (obj.OriginalTitle == null) { obj.OriginalTitle = ""; }
                if (obj.Year == null) { obj.Year = ""; }
                if (obj.Rating == null) { obj.Rating = ""; }
                if (obj.Genres == null) { obj.Genres = ""; }
                if (obj.Directors == null) { obj.Directors = ""; }
                if (obj.Writers == null) { obj.Writers = ""; }
                if (obj.Producers == null) { obj.Producers = ""; }
                if (obj.Musicians == null) { obj.Musicians = ""; }
                if (obj.Cinematographers == null) { obj.Cinematographers = ""; }
                if (obj.Editors == null) { obj.Editors = ""; }
                if (obj.Cast == null) { obj.Cast = ""; }
                if (obj.Plot == null) { obj.Plot = ""; }
                if (obj.PlotKeywords == null) { obj.PlotKeywords = ""; }
                if (obj.ReleaseDate == null) { obj.ReleaseDate = ""; }
                if (obj.Runtime == null) { obj.Runtime = ""; }
                if (obj.Top250 == null) { obj.Top250 = ""; }
                if (obj.Oscars == null) { obj.Oscars = ""; }
                if (obj.Awards == null) { obj.Awards = ""; }
                if (obj.Nominations == null) { obj.Nominations = ""; }
                if (obj.Tagline == null) { obj.Tagline = ""; }
                if (obj.MpaaRating == null) { obj.MpaaRating = ""; }
                if (obj.Votes == null) { obj.Votes = ""; }
                if (obj.Languages == null) { obj.Languages = ""; }
                if (obj.Countries == null) { obj.Countries = ""; }
                if (obj.Poster == null) { obj.Poster = ""; }

                string poster = obj.Poster + "|" + obj.PosterFull + "|" + obj.PosterLarge;

                cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID.ToString());
                cmd2.Parameters.AddWithValue(@"Title", obj.Title.ToString());
                cmd2.Parameters.AddWithValue(@"OriginalTitle", obj.OriginalTitle.ToString());
                cmd2.Parameters.AddWithValue(@"Year", obj.Year.ToString());
                cmd2.Parameters.AddWithValue(@"Rating", obj.Rating.ToString());
                cmd2.Parameters.AddWithValue(@"Genres", obj.Genres.ToString());
                cmd2.Parameters.AddWithValue(@"Directors", obj.Directors.ToString());
                cmd2.Parameters.AddWithValue(@"Writers", obj.Writers.ToString());
                cmd2.Parameters.AddWithValue(@"Producers", obj.Producers.ToString());
                cmd2.Parameters.AddWithValue(@"Musicians", obj.Musicians.ToString());
                cmd2.Parameters.AddWithValue(@"Cinematographers", obj.Cinematographers.ToString());
                cmd2.Parameters.AddWithValue(@"Editors", obj.Editors.ToString());
                cmd2.Parameters.AddWithValue(@"Cast", obj.Cast.ToString());
                cmd2.Parameters.AddWithValue(@"Plot", obj.Plot.ToString());
                cmd2.Parameters.AddWithValue(@"PlotKeywords", obj.PlotKeywords.ToString());
                cmd2.Parameters.AddWithValue(@"ReleaseDate", obj.ReleaseDate.ToString());
                cmd2.Parameters.AddWithValue(@"Runtime", obj.Runtime.ToString());
                cmd2.Parameters.AddWithValue(@"Top250", obj.Top250.ToString());
                cmd2.Parameters.AddWithValue(@"Oscars", obj.Oscars.ToString());
                cmd2.Parameters.AddWithValue(@"Awards", obj.Awards.ToString());
                cmd2.Parameters.AddWithValue(@"Nominations", obj.Nominations.ToString());
                cmd2.Parameters.AddWithValue(@"Tagline", obj.Tagline.ToString());
                cmd2.Parameters.AddWithValue(@"MpaaRating", obj.MpaaRating.ToString());
                cmd2.Parameters.AddWithValue(@"Votes", obj.Votes.ToString());
                cmd2.Parameters.AddWithValue(@"Languages", obj.Languages.ToString());
                cmd2.Parameters.AddWithValue(@"Countries", obj.Countries.ToString());
                cmd2.Parameters.AddWithValue(@"Poster", poster);
                cmd2.Parameters.AddWithValue(@"RootDir", obj.RootDir.ToString());
                cmd2.Parameters.AddWithValue(@"DirSize", obj.DirSize.ToString());

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        imdB_UpdateSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        imdB_UpdateSQL(obj);
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

            int imdbUpdateCount = 0;
            int imdbInsertCount = 0;

            public bool imdB_UpdateInsert(imdB obj)
            {
                SqlConnection Conn = imdB_Conn();

                bool Updated = imdB_UpdateSQL(obj);  // try to update record first

                //bool Updated = imdB_UpdateIfPopulated(obj);  // only write populated fields


                if (Updated) { imdbUpdateCount++; }
                if (!Updated) { Updated = imdB_InsertSQL(obj); imdbInsertCount++; }  // if unable to update, insert new record

                return Updated;
            }


            //string imdbTable = "[lucidmedia].[lucidmethod].[LITM_IMDb]";

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool imdB_UpdateIfPopulated(imdB obj)
            {
                SqlConnection Conn = imdB_Conn();
                string TableName = "[lucidmedia].[lucidmethod].[LITM_IMDb]";

                TableName = IMDbMovieTable;

                string SQLcmd = "Update " + TableName + " SET ";
                if (obj.IMDbID != null) { SQLcmd = SQLcmd + " IMDbID = @IMDbID,"; }
                if (obj.Title != null) { SQLcmd = SQLcmd + " Title = @Title,"; }
                if (obj.OriginalTitle != null) { SQLcmd = SQLcmd + " OriginalTitle = @OriginalTitle,"; }
                if (obj.Year != null) { SQLcmd = SQLcmd + " Year = @Year,"; }
                if (obj.Rating != null) { SQLcmd = SQLcmd + " Rating = @Rating,"; }
                if (obj.Genres != null) { SQLcmd = SQLcmd + " Genres = @Genres,"; }
                if (obj.Directors != null) { SQLcmd = SQLcmd + " Directors = @Directors,"; }
                if (obj.Writers != null) { SQLcmd = SQLcmd + " Writers = @Writers,"; }
                if (obj.Producers != null) { SQLcmd = SQLcmd + " Producers = @Producers,"; }
                if (obj.Musicians != null) { SQLcmd = SQLcmd + " Musicians = @Musicians,"; }
                if (obj.Cinematographers != null) { SQLcmd = SQLcmd + " Cinematographers = @Cinematographers,"; }
                if (obj.Editors != null) { SQLcmd = SQLcmd + " Editors = @Editors,"; }
                if (obj.Cast != null) { SQLcmd = SQLcmd + " Cast = @Cast,"; }
                if (obj.Plot != null) { SQLcmd = SQLcmd + " Plot = @Plot,"; }
                if (obj.PlotKeywords != null) { SQLcmd = SQLcmd + " PlotKeywords = @PlotKeywords,"; }
                if (obj.ReleaseDate != null) { SQLcmd = SQLcmd + " ReleaseDate = @ReleaseDate,"; }
                if (obj.Runtime != null) { SQLcmd = SQLcmd + " Runtime = @Runtime,"; }
                if (obj.Top250 != null) { SQLcmd = SQLcmd + " Top250 = @Top250,"; }
                if (obj.Oscars != null) { SQLcmd = SQLcmd + " Oscars = @Oscars,"; }
                if (obj.Awards != null) { SQLcmd = SQLcmd + " Awards = @Awards,"; }
                if (obj.Nominations != null) { SQLcmd = SQLcmd + " Nominations = @Nominations,"; }
                if (obj.Tagline != null) { SQLcmd = SQLcmd + " Tagline = @Tagline,"; }
                if (obj.MpaaRating != null) { SQLcmd = SQLcmd + " MpaaRating = @MpaaRating,"; }
                if (obj.Votes != null) { SQLcmd = SQLcmd + " Votes = @Votes,"; }
                if (obj.Languages != null) { SQLcmd = SQLcmd + " Languages = @Languages,"; }
                if (obj.Countries != null) { SQLcmd = SQLcmd + " Countries = @Countries,"; }
                if (obj.Poster != null) { SQLcmd = SQLcmd + " Poster = @Poster,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ID = @ID";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.IMDbID != null) { cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID); }
                if (obj.Title != null) { cmd2.Parameters.AddWithValue(@"Title", obj.Title); }
                if (obj.OriginalTitle != null) { cmd2.Parameters.AddWithValue(@"OriginalTitle", obj.OriginalTitle); }
                if (obj.Year != null) { cmd2.Parameters.AddWithValue(@"Year", obj.Year); }
                if (obj.Rating != null) { cmd2.Parameters.AddWithValue(@"Rating", obj.Rating); }
                if (obj.Genres != null) { cmd2.Parameters.AddWithValue(@"Genres", obj.Genres); }
                if (obj.Directors != null) { cmd2.Parameters.AddWithValue(@"Directors", obj.Directors); }
                if (obj.Writers != null) { cmd2.Parameters.AddWithValue(@"Writers", obj.Writers); }
                if (obj.Producers != null) { cmd2.Parameters.AddWithValue(@"Producers", obj.Producers); }
                if (obj.Musicians != null) { cmd2.Parameters.AddWithValue(@"Musicians", obj.Musicians); }
                if (obj.Cinematographers != null) { cmd2.Parameters.AddWithValue(@"Cinematographers", obj.Cinematographers); }
                if (obj.Editors != null) { cmd2.Parameters.AddWithValue(@"Editors", obj.Editors); }
                if (obj.Cast != null) { cmd2.Parameters.AddWithValue(@"Cast", obj.Cast); }
                if (obj.Plot != null) { cmd2.Parameters.AddWithValue(@"Plot", obj.Plot); }
                if (obj.PlotKeywords != null) { cmd2.Parameters.AddWithValue(@"PlotKeywords", obj.PlotKeywords); }
                if (obj.ReleaseDate != null) { cmd2.Parameters.AddWithValue(@"ReleaseDate", obj.ReleaseDate); }
                if (obj.Runtime != null) { cmd2.Parameters.AddWithValue(@"Runtime", obj.Runtime); }
                if (obj.Top250 != null) { cmd2.Parameters.AddWithValue(@"Top250", obj.Top250); }
                if (obj.Oscars != null) { cmd2.Parameters.AddWithValue(@"Oscars", obj.Oscars); }
                if (obj.Awards != null) { cmd2.Parameters.AddWithValue(@"Awards", obj.Awards); }
                if (obj.Nominations != null) { cmd2.Parameters.AddWithValue(@"Nominations", obj.Nominations); }
                if (obj.Tagline != null) { cmd2.Parameters.AddWithValue(@"Tagline", obj.Tagline); }
                if (obj.MpaaRating != null) { cmd2.Parameters.AddWithValue(@"MpaaRating", obj.MpaaRating); }
                if (obj.Votes != null) { cmd2.Parameters.AddWithValue(@"Votes", obj.Votes); }
                if (obj.Languages != null) { cmd2.Parameters.AddWithValue(@"Languages", obj.Languages); }
                if (obj.Countries != null) { cmd2.Parameters.AddWithValue(@"Countries", obj.Countries); }
                if (obj.Poster != null) { cmd2.Parameters.AddWithValue(@"Poster", obj.Poster); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            /// <summary>
            /// Returns IMDb Info From SQL Server
            /// </summary>
            /// <param name="IMDbID"></param>
            /// <returns></returns>
            public imdB imdB_ReturnSQL(string IMDbID = "")
            {
                SqlConnection Conn = imdB_Conn();
                string TableName = "[lucidmedia].[lucidmethod].[LITM_IMDb]";

                TableName = IMDbMovieTable;

                string SelectLine = "Select [IMDbID],[Title],[OriginalTitle],[Year],[Rating],[Genres],[Directors],[Writers],[Producers],[Musicians],[Cinematographers],[Editors],[Cast],[Plot],[PlotKeywords],[ReleaseDate],[Runtime],[Top250],[Oscars],[Awards],[Nominations],[Tagline],[MpaaRating],[Votes],[Languages],[Countries],[Poster],[RootDir],[DirSize] From " + TableName + " WHERE IMDbID = '" + IMDbID + "'";

                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                imdB returnObject = new imdB();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.Title = ret["Title"].ToString();
                        returnObject.OriginalTitle = ret["OriginalTitle"].ToString();
                        returnObject.Year = ret["Year"].ToString();
                        returnObject.Rating = ret["Rating"].ToString();
                        returnObject.Genres = ret["Genres"].ToString();
                        returnObject.Directors = ret["Directors"].ToString();
                        returnObject.Writers = ret["Writers"].ToString();
                        returnObject.Producers = ret["Producers"].ToString();
                        returnObject.Musicians = ret["Musicians"].ToString();
                        returnObject.Cinematographers = ret["Cinematographers"].ToString();
                        returnObject.Editors = ret["Editors"].ToString();
                        returnObject.Cast = ret["Cast"].ToString();
                        returnObject.Plot = ret["Plot"].ToString();
                        returnObject.PlotKeywords = ret["PlotKeywords"].ToString();
                        returnObject.ReleaseDate = ret["ReleaseDate"].ToString();
                        returnObject.Runtime = ret["Runtime"].ToString();
                        returnObject.Top250 = ret["Top250"].ToString();
                        returnObject.Oscars = ret["Oscars"].ToString();
                        returnObject.Awards = ret["Awards"].ToString();
                        returnObject.Nominations = ret["Nominations"].ToString();
                        returnObject.Tagline = ret["Tagline"].ToString();
                        returnObject.MpaaRating = ret["MpaaRating"].ToString();
                        returnObject.Votes = ret["Votes"].ToString();
                        returnObject.Languages = ret["Languages"].ToString();
                        returnObject.Countries = ret["Countries"].ToString();
                        returnObject.Poster = ret["Poster"].ToString();
                        returnObject.RootDir = ret["RootDir"].ToString();
                        returnObject.DirSize = ret["DirSize"].ToString();

                        return returnObject;
                    }
                }
                return returnObject;
            }

            public bool inDb(string IMDbID) // check to see if IMDb ID already exists in sql table
            {
                string TableName = IMDbMovieTable; //"[lucidmedia].[lucidmethod].[LITM_IMDb]";
                SqlConnection Conn = imdB_Conn();
                int found = sql.Count(Conn, "Select count(IMDbID) FROM " + TableName + " Where IMDbID = '" + IMDbID + "'");
                if (found > 0) { return true; }
                return false;
            }

            public List<imdB> imdB_ReturnSQLList(string Command = "")
            {
                string cmd = Command;
                if (Command == "") { cmd = "select * from " + IMDbMovieTable; }

                SqlConnection Conn = imdB_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<imdB> ReturnList = new List<imdB>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        imdB returnObject = new imdB();
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.Title = ret["Title"].ToString();
                        returnObject.OriginalTitle = ret["OriginalTitle"].ToString();
                        returnObject.Year = ret["Year"].ToString();
                        returnObject.Rating = ret["Rating"].ToString();
                        returnObject.Genres = ret["Genres"].ToString();
                        returnObject.Directors = ret["Directors"].ToString();
                        returnObject.Writers = ret["Writers"].ToString();
                        returnObject.Producers = ret["Producers"].ToString();
                        returnObject.Musicians = ret["Musicians"].ToString();
                        returnObject.Cinematographers = ret["Cinematographers"].ToString();
                        returnObject.Editors = ret["Editors"].ToString();
                        returnObject.Cast = ret["Cast"].ToString();
                        returnObject.Plot = ret["Plot"].ToString();
                        returnObject.PlotKeywords = ret["PlotKeywords"].ToString();
                        returnObject.ReleaseDate = ret["ReleaseDate"].ToString();
                        returnObject.Runtime = ret["Runtime"].ToString();
                        returnObject.Top250 = ret["Top250"].ToString();
                        returnObject.Oscars = ret["Oscars"].ToString();
                        returnObject.Awards = ret["Awards"].ToString();
                        returnObject.Nominations = ret["Nominations"].ToString();
                        returnObject.Tagline = ret["Tagline"].ToString();
                        returnObject.MpaaRating = ret["MpaaRating"].ToString();
                        returnObject.Votes = ret["Votes"].ToString();
                        returnObject.Languages = ret["Languages"].ToString();
                        returnObject.Countries = ret["Countries"].ToString();
                        returnObject.Poster = ret["Poster"].ToString();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }

            public bool imdB_UpdatePosterPath(string IMDbID, string PosterPath)
            {
                SqlConnection Conn = imdB_Conn();
                string TableName = "[lucidmedia].[lucidmethod].[LITM_IMDb]";
                string SQLcmd = "Update " + TableName + " SET Poster = '" + PosterPath + "' WHERE [IMDbID] = '" + IMDbID + "'";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }


            #endregion

            #endregion

            #region === IMDb Scraper ===

            /*******************************************************************************
             * Free ASP.net IMDb Scraper API for the new IMDb Template.
             * Author: Abhinay Rathore
             * Website: http://www.AbhinayRathore.com
             * Blog: http://web3o.blogspot.com
             * More Info: http://web3o.blogspot.com/2010/11/aspnetc-imdb-scraping-api.html
             * Last Updated: Feb 20, 2013
             *******************************************************************************/


            //public void SQLite_New_IMDB_Table(string DBFile, string TableName)
            //{
            //    //========================================================
            //    // Create New SQLite DB (*Used First-Run*)
            //    // ========================================================
            //    if (!File.Exists(DBFile)) // create database file if it doen't exist
            //    {
            //        SQLiteConnection.CreateFile(DBFile);
            //    }

            //    //===================================
            //    // Connect to the DB
            //    //===================================

            //    _Database.SQLite lite = new _Database.SQLite();
            //    SQLiteConnection dbConnection = lite._ConnectToDB(DBFile); // connect to SQLite DB file path - returns connection data


            //    bool ReturnValue = lite._DropTable(DBFile, TableName); // drop table from DB if it exists already

            //    try
            //    {
            //        lite._sqlite("CREATE TABLE [" + TableName + "] ((Title VARCHAR, OriginalTitle VARCHAR, Year VARCHAR, Rating VARCHAR, Id VARCHAR, MpaaRating VARCHAR, ReleaseDate VARCHAR, Plot VARCHAR, Poster VARCHAR, PosterLarge VARCHAR, PosterFull VARCHAR, Runtime VARCHAR, Top250 VARCHAR, Oscars VARCHAR, Awards VARCHAR, Nominations VARCHAR, Storyline VARCHAR, Tagline VARCHAR, Votes VARCHAR, ImdbURL VARCHAR)", dbConnection);  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]

            //    }
            //    catch (Exception ex)
            //    {
            //        MessageBox.Show(ex.Message);
            //    }


            //}




            public void GetMedia()
            {
                //string id = IMDb_ID_FromURL(txtIMDbURL.Text);
                getIMDbMedia("tt5462602");
            }

            //Get all media images
            public List<string> getIMDbMedia(string imdbID)
            {
                List<string> list = new List<string>();
                string mediaurl = "http://www.imdb.com/title/" + imdbID + "/mediaindex";
                string mediahtml = getUrlData(mediaurl);

                //int pagecount = matchAll(@"<a href=""\?page=(.*?)"">", match(@"<span style=""padding: 0 1em;"">(.*?)</span>", mediahtml)).Count;
                //for (int p = 1; p <= pagecount + 1; p++)
                //{
                //mediahtml = getUrlData(mediaurl + "?page=" + p);
                foreach (Match m in new Regex(@"src=""(.*?)""", RegexOptions.Multiline).Matches(match(@"<div class=""thumb_list"" style=""font-size: 0px;"">(.*?)</div>", mediahtml)))
                {
                    String image = m.Groups[1].Value;
                    //string path = Regex.Replace(image, @"_V1\..*?.jpg", "_V1._SY0.jpg"); 
                    list.Add(Regex.Replace(image, @"_V1\..*?.jpg", "_V1._SY0.jpg"));
                }
                //}

                return list;
            }



            //public string FormatDBTextIn(string InText)
            //{
            //    if (InText == null) { return ""; }

            //    string OutText = InText.Replace(",", "");  //format text to write to datatbase 
            //    string OutText2 = OutText.Replace("`", "");  //format text to write to datatbase 
            //    OutText = OutText2.Replace("\"", "");  //format text to write to datatbase 

            //    return OutText;
            //}


            ////Match single instance
            //private string match(string regex, string html, int i = 1)
            //{
            //    return new Regex(regex, RegexOptions.Multiline).Match(html).Groups[i].Value.Trim();
            //}

            ////Get URL Data
            //private string getUrlData(string url)
            //{
            //    if (url == "http://www.imdb.com/findcombined") { return ""; }
            //    if (url == "http://www.imdb.com/findplotsummary") { return ""; }
            //    if (url == "http://www.imdb.com/title/http:www.imdb.comfind/releaseinfo") { return ""; }



            //    //string HTML = Download_HTML(url);
            //    //return HTML; 

            //    WebClient client = new WebClient();
            //    Random r = new Random();
            //    //Random IP Address
            //    client.Headers["X-Forwarded-For"] = r.Next(0, 255) + "." + r.Next(0, 255) + "." + r.Next(0, 255) + "." + r.Next(0, 255);
            //    //Random User-Agent
            //    client.Headers["User-Agent"] = "Mozilla/" + r.Next(3, 5) + ".0 (Windows NT " + r.Next(3, 5) + "." + r.Next(0, 2) + "; rv:2.0.1) Gecko/20100101 Firefox/" + r.Next(3, 5) + "." + r.Next(0, 5) + "." + r.Next(0, 5);
            //    try
            //    {
            //        System.IO.Stream datastream = client.OpenRead(url);
            //        StreamReader reader = new StreamReader(datastream);
            //        System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //        while (!reader.EndOfStream)
            //            sb.Append(reader.ReadLine());
            //        return sb.ToString();
            //    }
            //    catch
            //    {
            //        return "";
            //    }

            //}

            ////Get IMDb URL from search results
            //private string getIMDbUrl(string MovieName, string searchEngine = "google")
            //{
            //    string url = GoogleSearch + MovieName; //default to Google search
            //    if (searchEngine.ToLower().Equals("bing")) url = BingSearch + MovieName;
            //    if (searchEngine.ToLower().Equals("ask")) url = AskSearch + MovieName;
            //    string html = getUrlData(url);
            //    ArrayList imdbUrls = matchAll(@"<a href=""(http://www.imdb.com/title/tt\d{7}/)"".*?>.*?</a>", html);
            //    if (imdbUrls.Count > 0)
            //        return (string)imdbUrls[0]; //return first IMDb result
            //    else if (searchEngine.ToLower().Equals("google")) //if Google search fails
            //        return getIMDbUrl(MovieName, "bing"); //search using Bing
            //    else if (searchEngine.ToLower().Equals("bing")) //if Bing search fails
            //        return getIMDbUrl(MovieName, "ask"); //search using Ask
            //    else //search fails
            //        return string.Empty;
            //}

            ////Get all release dates
            //private ArrayList getReleaseDates()
            //{
            //    ArrayList list = new ArrayList();
            //    string releasehtml = getUrlData("http://www.imdb.com/title/" + Id + "/releaseinfo");
            //    foreach (string r in matchAll(@"<tr>(.*?)</tr>", match(@"Date</th></tr>\n*?(.*?)</table>", releasehtml)))
            //    {
            //        Match rd = new Regex(@"<td>(.*?)</td>\n*?.*?<td align=""right"">(.*?)</td>", RegexOptions.Multiline).Match(r);
            //        list.Add(StripHTML(rd.Groups[1].Value.Trim()) + " = " + StripHTML(rd.Groups[2].Value.Trim()));
            //    }
            //    return list;
            //}

            ////Get all media images (doesn't seem to work)
            //private ArrayList getMediaImages()
            //{
            //    ArrayList list = new ArrayList();
            //    string mediaurl = "http://www.imdb.com/title/" + Id + "/mediaindex";
            //    string mediahtml = getUrlData(mediaurl);
            //    int pagecount = matchAll(@"<a href=""\?page=(.*?)"">", match(@"<span style=""padding: 0 1em;"">(.*?)</span>", mediahtml)).Count;
            //    for (int p = 1; p <= pagecount + 1; p++)
            //    {
            //        mediahtml = getUrlData(mediaurl + "?page=" + p);
            //        foreach (Match m in new Regex(@"src=""(.*?)""", RegexOptions.Multiline).Matches(match(@"<div class=""thumb_list"" style=""font-size: 0px;"">(.*?)</div>", mediahtml)))
            //        {
            //            String image = m.Groups[1].Value;
            //            list.Add(Regex.Replace(image, @"_V1\..*?.jpg", "_V1._SY0.jpg"));
            //        }
            //    }
            //    return list;
            //}

            ////Get Recommended Titles
            //private ArrayList getRecommendedTitles()
            //{
            //    ArrayList list = new ArrayList();
            //    string recUrl = "http://www.imdb.com/widget/recommendations/_ajax/get_more_recs?specs=p13nsims%3A" + Id;
            //    string json = getUrlData(recUrl);
            //    list = matchAll(@"title=\\""(.*?)\\""", json);
            //    HashSet<String> set = new HashSet<string>();
            //    foreach (String rec in list) set.Add(rec);
            //    return new ArrayList(set.ToList());
            //}

            ///*******************************[ Helper Methods ]********************************/

            ////Match all instances and return as ArrayList
            //private ArrayList matchAll(string regex, string html, int i = 1)
            //{
            //    ArrayList list = new ArrayList();
            //    foreach (Match m in new Regex(regex, RegexOptions.Multiline).Matches(html))
            //        list.Add(m.Groups[i].Value.Trim());
            //    return list;
            //}

            ////Strip HTML Tags
            //static string StripHTML(string inputString)
            //{
            //    return Regex.Replace(inputString, @"<.*?>", string.Empty);
            //}


            //// IMDB Setup


            ////Search Engine URLs
            //private string GoogleSearch = "http://www.google.com/search?q=imdb+";
            //private string BingSearch = "http://www.bing.com/search?q=imdb+";
            //private string AskSearch = "http://www.ask.com/web?q=imdb+";



            ////#region === imdB Object ===

            ////public struct imdB
            ////{
            ////    public string IMDbID { get; set; }
            ////    public string Title { get; set; }
            ////    public string OriginalTitle { get; set; }
            ////    public string Year { get; set; }
            ////    public string Rating { get; set; }
            ////    public string Genres { get; set; }
            ////    public string Directors { get; set; }
            ////    public string Writers { get; set; }
            ////    public string Producers { get; set; }
            ////    public string Musicians { get; set; }
            ////    public string Cinematographers { get; set; }
            ////    public string Editors { get; set; }
            ////    public string Cast { get; set; }
            ////    public string Plot { get; set; }
            ////    public string PlotKeywords { get; set; }
            ////    public string ReleaseDate { get; set; }
            ////    public string Runtime { get; set; }
            ////    public string Top250 { get; set; }
            ////    public string Oscars { get; set; }
            ////    public string Awards { get; set; }
            ////    public string Nominations { get; set; }
            ////    public string Tagline { get; set; }
            ////    public string MpaaRating { get; set; }
            ////    public string Votes { get; set; }
            ////    public string Languages { get; set; }
            ////    public string Countries { get; set; }
            ////    public string Poster { get; set; }
            ////    public string PosterLarge { get; set; }
            ////    public string PosterFull { get; set; }
            ////    public string RootDir { get; set; }
            ////    public string DirSize { get; set; }
            ////}
            ////public imdB Return_imdB(string IMDbID = "", string Title = "", string OriginalTitle = "", string Year = "", string Rating = "", string Genres = "", string Directors = "", string Writers = "", string Producers = "", string Musicians = "", string Cinematographers = "", string Editors = "", string Cast = "", string Plot = "", string PlotKeywords = "", string ReleaseDate = "", string Runtime = "", string Top250 = "", string Oscars = "", string Awards = "", string Nominations = "", string Tagline = "", string MpaaRating = "", string Votes = "", string Languages = "", string Countries = "", string Poster = "")
            ////{
            ////    imdB obj = new imdB();
            ////    obj.IMDbID = IMDbID;
            ////    obj.Title = Title;
            ////    obj.OriginalTitle = OriginalTitle;
            ////    obj.Year = Year;
            ////    obj.Rating = Rating;
            ////    obj.Genres = Genres;
            ////    obj.Directors = Directors;
            ////    obj.Writers = Writers;
            ////    obj.Producers = Producers;
            ////    obj.Musicians = Musicians;
            ////    obj.Cinematographers = Cinematographers;
            ////    obj.Editors = Editors;
            ////    obj.Cast = Cast;
            ////    obj.Plot = Plot;
            ////    obj.PlotKeywords = PlotKeywords;
            ////    obj.ReleaseDate = ReleaseDate;
            ////    obj.Runtime = Runtime;
            ////    obj.Top250 = Top250;
            ////    obj.Oscars = Oscars;
            ////    obj.Awards = Awards;
            ////    obj.Nominations = Nominations;
            ////    obj.Tagline = Tagline;
            ////    obj.MpaaRating = MpaaRating;
            ////    obj.Votes = Votes;
            ////    obj.Languages = Languages;
            ////    obj.Countries = Countries;
            ////    obj.Poster = Poster;

            ////    return obj;
            ////}

            //////  Fix illegal characters before Sql/Sqlite Db Inserts
            ////public imdB imdB_FixChars(imdB ToFix)
            ////{
            ////    imdB Fixed = new imdB();

            ////    Fixed.IMDbID = ToFix.IMDbID.Replace("'", "''");
            ////    Fixed.Title = ToFix.Title.Replace("'", "''");
            ////    Fixed.OriginalTitle = ToFix.OriginalTitle.Replace("'", "''");
            ////    Fixed.Year = ToFix.Year.Replace("'", "''");
            ////    Fixed.Rating = ToFix.Rating.Replace("'", "''");
            ////    Fixed.Genres = ToFix.Genres.Replace("'", "''");
            ////    Fixed.Directors = ToFix.Directors.Replace("'", "''");
            ////    Fixed.Writers = ToFix.Writers.Replace("'", "''");
            ////    Fixed.Producers = ToFix.Producers.Replace("'", "''");
            ////    Fixed.Musicians = ToFix.Musicians.Replace("'", "''");
            ////    Fixed.Cinematographers = ToFix.Cinematographers.Replace("'", "''");
            ////    Fixed.Editors = ToFix.Editors.Replace("'", "''");
            ////    Fixed.Cast = ToFix.Cast.Replace("'", "''");
            ////    Fixed.Plot = ToFix.Plot.Replace("'", "''");
            ////    Fixed.PlotKeywords = ToFix.PlotKeywords.Replace("'", "''");
            ////    Fixed.ReleaseDate = ToFix.ReleaseDate.Replace("'", "''");
            ////    Fixed.Runtime = ToFix.Runtime.Replace("'", "''");
            ////    Fixed.Top250 = ToFix.Top250.Replace("'", "''");
            ////    Fixed.Oscars = ToFix.Oscars.Replace("'", "''");
            ////    Fixed.Awards = ToFix.Awards.Replace("'", "''");
            ////    Fixed.Nominations = ToFix.Nominations.Replace("'", "''");
            ////    Fixed.Tagline = ToFix.Tagline.Replace("'", "''");
            ////    Fixed.MpaaRating = ToFix.MpaaRating.Replace("'", "''");
            ////    Fixed.Votes = ToFix.Votes.Replace("'", "''");
            ////    Fixed.Languages = ToFix.Languages.Replace("'", "''");
            ////    Fixed.Countries = ToFix.Countries.Replace("'", "''");
            ////    Fixed.Poster = ToFix.Poster.Replace("'", "''");

            ////    return Fixed;
            ////}

            ////// Compare two objects to see if they have identical values
            ////public bool imdB_Changed(imdB OldVal, imdB NewVal)
            ////{
            ////    imdB diff = new imdB();
            ////    List<string> diffList = new List<string>();
            ////    bool different = false;
            ////    if (OldVal.IMDbID == null) { OldVal.IMDbID = ""; }
            ////    if (NewVal.IMDbID == null) { NewVal.IMDbID = ""; }
            ////    if (OldVal.IMDbID != NewVal.IMDbID) { different = true; }
            ////    if (OldVal.Title == null) { OldVal.Title = ""; }
            ////    if (NewVal.Title == null) { NewVal.Title = ""; }
            ////    if (OldVal.Title != NewVal.Title) { different = true; }
            ////    if (OldVal.OriginalTitle == null) { OldVal.OriginalTitle = ""; }
            ////    if (NewVal.OriginalTitle == null) { NewVal.OriginalTitle = ""; }
            ////    if (OldVal.OriginalTitle != NewVal.OriginalTitle) { different = true; }
            ////    if (OldVal.Year == null) { OldVal.Year = ""; }
            ////    if (NewVal.Year == null) { NewVal.Year = ""; }
            ////    if (OldVal.Year != NewVal.Year) { different = true; }
            ////    if (OldVal.Rating == null) { OldVal.Rating = ""; }
            ////    if (NewVal.Rating == null) { NewVal.Rating = ""; }
            ////    if (OldVal.Rating != NewVal.Rating) { different = true; }
            ////    if (OldVal.Genres == null) { OldVal.Genres = ""; }
            ////    if (NewVal.Genres == null) { NewVal.Genres = ""; }
            ////    if (OldVal.Genres != NewVal.Genres) { different = true; }
            ////    if (OldVal.Directors == null) { OldVal.Directors = ""; }
            ////    if (NewVal.Directors == null) { NewVal.Directors = ""; }
            ////    if (OldVal.Directors != NewVal.Directors) { different = true; }
            ////    if (OldVal.Writers == null) { OldVal.Writers = ""; }
            ////    if (NewVal.Writers == null) { NewVal.Writers = ""; }
            ////    if (OldVal.Writers != NewVal.Writers) { different = true; }
            ////    if (OldVal.Producers == null) { OldVal.Producers = ""; }
            ////    if (NewVal.Producers == null) { NewVal.Producers = ""; }
            ////    if (OldVal.Producers != NewVal.Producers) { different = true; }
            ////    if (OldVal.Musicians == null) { OldVal.Musicians = ""; }
            ////    if (NewVal.Musicians == null) { NewVal.Musicians = ""; }
            ////    if (OldVal.Musicians != NewVal.Musicians) { different = true; }
            ////    if (OldVal.Cinematographers == null) { OldVal.Cinematographers = ""; }
            ////    if (NewVal.Cinematographers == null) { NewVal.Cinematographers = ""; }
            ////    if (OldVal.Cinematographers != NewVal.Cinematographers) { different = true; }
            ////    if (OldVal.Editors == null) { OldVal.Editors = ""; }
            ////    if (NewVal.Editors == null) { NewVal.Editors = ""; }
            ////    if (OldVal.Editors != NewVal.Editors) { different = true; }
            ////    if (OldVal.Cast == null) { OldVal.Cast = ""; }
            ////    if (NewVal.Cast == null) { NewVal.Cast = ""; }
            ////    if (OldVal.Cast != NewVal.Cast) { different = true; }
            ////    if (OldVal.Plot == null) { OldVal.Plot = ""; }
            ////    if (NewVal.Plot == null) { NewVal.Plot = ""; }
            ////    if (OldVal.Plot != NewVal.Plot) { different = true; }
            ////    if (OldVal.PlotKeywords == null) { OldVal.PlotKeywords = ""; }
            ////    if (NewVal.PlotKeywords == null) { NewVal.PlotKeywords = ""; }
            ////    if (OldVal.PlotKeywords != NewVal.PlotKeywords) { different = true; }
            ////    if (OldVal.ReleaseDate == null) { OldVal.ReleaseDate = ""; }
            ////    if (NewVal.ReleaseDate == null) { NewVal.ReleaseDate = ""; }
            ////    if (OldVal.ReleaseDate != NewVal.ReleaseDate) { different = true; }
            ////    if (OldVal.Runtime == null) { OldVal.Runtime = ""; }
            ////    if (NewVal.Runtime == null) { NewVal.Runtime = ""; }
            ////    if (OldVal.Runtime != NewVal.Runtime) { different = true; }
            ////    if (OldVal.Top250 == null) { OldVal.Top250 = ""; }
            ////    if (NewVal.Top250 == null) { NewVal.Top250 = ""; }
            ////    if (OldVal.Top250 != NewVal.Top250) { different = true; }
            ////    if (OldVal.Oscars == null) { OldVal.Oscars = ""; }
            ////    if (NewVal.Oscars == null) { NewVal.Oscars = ""; }
            ////    if (OldVal.Oscars != NewVal.Oscars) { different = true; }
            ////    if (OldVal.Awards == null) { OldVal.Awards = ""; }
            ////    if (NewVal.Awards == null) { NewVal.Awards = ""; }
            ////    if (OldVal.Awards != NewVal.Awards) { different = true; }
            ////    if (OldVal.Nominations == null) { OldVal.Nominations = ""; }
            ////    if (NewVal.Nominations == null) { NewVal.Nominations = ""; }
            ////    if (OldVal.Nominations != NewVal.Nominations) { different = true; }
            ////    if (OldVal.Tagline == null) { OldVal.Tagline = ""; }
            ////    if (NewVal.Tagline == null) { NewVal.Tagline = ""; }
            ////    if (OldVal.Tagline != NewVal.Tagline) { different = true; }
            ////    if (OldVal.MpaaRating == null) { OldVal.MpaaRating = ""; }
            ////    if (NewVal.MpaaRating == null) { NewVal.MpaaRating = ""; }
            ////    if (OldVal.MpaaRating != NewVal.MpaaRating) { different = true; }
            ////    if (OldVal.Votes == null) { OldVal.Votes = ""; }
            ////    if (NewVal.Votes == null) { NewVal.Votes = ""; }
            ////    if (OldVal.Votes != NewVal.Votes) { different = true; }
            ////    if (OldVal.Languages == null) { OldVal.Languages = ""; }
            ////    if (NewVal.Languages == null) { NewVal.Languages = ""; }
            ////    if (OldVal.Languages != NewVal.Languages) { different = true; }
            ////    if (OldVal.Countries == null) { OldVal.Countries = ""; }
            ////    if (NewVal.Countries == null) { NewVal.Countries = ""; }
            ////    if (OldVal.Countries != NewVal.Countries) { different = true; }
            ////    if (OldVal.Poster == null) { OldVal.Poster = ""; }
            ////    if (NewVal.Poster == null) { NewVal.Poster = ""; }
            ////    if (OldVal.Poster != NewVal.Poster) { different = true; }
            ////    return different;
            ////}

            ////// Returns object containing the new values different from the old values in object comparison
            ////public imdB imdB_Diff(imdB OldVal, imdB NewVal)
            ////{
            ////    imdB diff = new imdB();
            ////    if (OldVal.IMDbID != NewVal.IMDbID) { diff.IMDbID = NewVal.IMDbID; }
            ////    if (OldVal.Title != NewVal.Title) { diff.Title = NewVal.Title; }
            ////    if (OldVal.OriginalTitle != NewVal.OriginalTitle) { diff.OriginalTitle = NewVal.OriginalTitle; }
            ////    if (OldVal.Year != NewVal.Year) { diff.Year = NewVal.Year; }
            ////    if (OldVal.Rating != NewVal.Rating) { diff.Rating = NewVal.Rating; }
            ////    if (OldVal.Genres != NewVal.Genres) { diff.Genres = NewVal.Genres; }
            ////    if (OldVal.Directors != NewVal.Directors) { diff.Directors = NewVal.Directors; }
            ////    if (OldVal.Writers != NewVal.Writers) { diff.Writers = NewVal.Writers; }
            ////    if (OldVal.Producers != NewVal.Producers) { diff.Producers = NewVal.Producers; }
            ////    if (OldVal.Musicians != NewVal.Musicians) { diff.Musicians = NewVal.Musicians; }
            ////    if (OldVal.Cinematographers != NewVal.Cinematographers) { diff.Cinematographers = NewVal.Cinematographers; }
            ////    if (OldVal.Editors != NewVal.Editors) { diff.Editors = NewVal.Editors; }
            ////    if (OldVal.Cast != NewVal.Cast) { diff.Cast = NewVal.Cast; }
            ////    if (OldVal.Plot != NewVal.Plot) { diff.Plot = NewVal.Plot; }
            ////    if (OldVal.PlotKeywords != NewVal.PlotKeywords) { diff.PlotKeywords = NewVal.PlotKeywords; }
            ////    if (OldVal.ReleaseDate != NewVal.ReleaseDate) { diff.ReleaseDate = NewVal.ReleaseDate; }
            ////    if (OldVal.Runtime != NewVal.Runtime) { diff.Runtime = NewVal.Runtime; }
            ////    if (OldVal.Top250 != NewVal.Top250) { diff.Top250 = NewVal.Top250; }
            ////    if (OldVal.Oscars != NewVal.Oscars) { diff.Oscars = NewVal.Oscars; }
            ////    if (OldVal.Awards != NewVal.Awards) { diff.Awards = NewVal.Awards; }
            ////    if (OldVal.Nominations != NewVal.Nominations) { diff.Nominations = NewVal.Nominations; }
            ////    if (OldVal.Tagline != NewVal.Tagline) { diff.Tagline = NewVal.Tagline; }
            ////    if (OldVal.MpaaRating != NewVal.MpaaRating) { diff.MpaaRating = NewVal.MpaaRating; }
            ////    if (OldVal.Votes != NewVal.Votes) { diff.Votes = NewVal.Votes; }
            ////    if (OldVal.Languages != NewVal.Languages) { diff.Languages = NewVal.Languages; }
            ////    if (OldVal.Countries != NewVal.Countries) { diff.Countries = NewVal.Countries; }
            ////    if (OldVal.Poster != NewVal.Poster) { diff.Poster = NewVal.Poster; }
            ////    return diff;
            ////}

            ////// Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
            ////public List<string> imdB_DiffList(imdB OldVal, imdB NewVal)
            ////{
            ////    List<string> diffList = new List<string>();
            ////    if (OldVal.IMDbID != NewVal.IMDbID) { diffList.Add("Changed IMDbID Value From " + OldVal.IMDbID + " To " + NewVal.IMDbID); }
            ////    if (OldVal.Title != NewVal.Title) { diffList.Add("Changed Title Value From " + OldVal.Title + " To " + NewVal.Title); }
            ////    if (OldVal.OriginalTitle != NewVal.OriginalTitle) { diffList.Add("Changed OriginalTitle Value From " + OldVal.OriginalTitle + " To " + NewVal.OriginalTitle); }
            ////    if (OldVal.Year != NewVal.Year) { diffList.Add("Changed Year Value From " + OldVal.Year + " To " + NewVal.Year); }
            ////    if (OldVal.Rating != NewVal.Rating) { diffList.Add("Changed Rating Value From " + OldVal.Rating + " To " + NewVal.Rating); }
            ////    if (OldVal.Genres != NewVal.Genres) { diffList.Add("Changed Genres Value From " + OldVal.Genres + " To " + NewVal.Genres); }
            ////    if (OldVal.Directors != NewVal.Directors) { diffList.Add("Changed Directors Value From " + OldVal.Directors + " To " + NewVal.Directors); }
            ////    if (OldVal.Writers != NewVal.Writers) { diffList.Add("Changed Writers Value From " + OldVal.Writers + " To " + NewVal.Writers); }
            ////    if (OldVal.Producers != NewVal.Producers) { diffList.Add("Changed Producers Value From " + OldVal.Producers + " To " + NewVal.Producers); }
            ////    if (OldVal.Musicians != NewVal.Musicians) { diffList.Add("Changed Musicians Value From " + OldVal.Musicians + " To " + NewVal.Musicians); }
            ////    if (OldVal.Cinematographers != NewVal.Cinematographers) { diffList.Add("Changed Cinematographers Value From " + OldVal.Cinematographers + " To " + NewVal.Cinematographers); }
            ////    if (OldVal.Editors != NewVal.Editors) { diffList.Add("Changed Editors Value From " + OldVal.Editors + " To " + NewVal.Editors); }
            ////    if (OldVal.Cast != NewVal.Cast) { diffList.Add("Changed Cast Value From " + OldVal.Cast + " To " + NewVal.Cast); }
            ////    if (OldVal.Plot != NewVal.Plot) { diffList.Add("Changed Plot Value From " + OldVal.Plot + " To " + NewVal.Plot); }
            ////    if (OldVal.PlotKeywords != NewVal.PlotKeywords) { diffList.Add("Changed PlotKeywords Value From " + OldVal.PlotKeywords + " To " + NewVal.PlotKeywords); }
            ////    if (OldVal.ReleaseDate != NewVal.ReleaseDate) { diffList.Add("Changed ReleaseDate Value From " + OldVal.ReleaseDate + " To " + NewVal.ReleaseDate); }
            ////    if (OldVal.Runtime != NewVal.Runtime) { diffList.Add("Changed Runtime Value From " + OldVal.Runtime + " To " + NewVal.Runtime); }
            ////    if (OldVal.Top250 != NewVal.Top250) { diffList.Add("Changed Top250 Value From " + OldVal.Top250 + " To " + NewVal.Top250); }
            ////    if (OldVal.Oscars != NewVal.Oscars) { diffList.Add("Changed Oscars Value From " + OldVal.Oscars + " To " + NewVal.Oscars); }
            ////    if (OldVal.Awards != NewVal.Awards) { diffList.Add("Changed Awards Value From " + OldVal.Awards + " To " + NewVal.Awards); }
            ////    if (OldVal.Nominations != NewVal.Nominations) { diffList.Add("Changed Nominations Value From " + OldVal.Nominations + " To " + NewVal.Nominations); }
            ////    if (OldVal.Tagline != NewVal.Tagline) { diffList.Add("Changed Tagline Value From " + OldVal.Tagline + " To " + NewVal.Tagline); }
            ////    if (OldVal.MpaaRating != NewVal.MpaaRating) { diffList.Add("Changed MpaaRating Value From " + OldVal.MpaaRating + " To " + NewVal.MpaaRating); }
            ////    if (OldVal.Votes != NewVal.Votes) { diffList.Add("Changed Votes Value From " + OldVal.Votes + " To " + NewVal.Votes); }
            ////    if (OldVal.Languages != NewVal.Languages) { diffList.Add("Changed Languages Value From " + OldVal.Languages + " To " + NewVal.Languages); }
            ////    if (OldVal.Countries != NewVal.Countries) { diffList.Add("Changed Countries Value From " + OldVal.Countries + " To " + NewVal.Countries); }
            ////    if (OldVal.Poster != NewVal.Poster) { diffList.Add("Changed Poster Value From " + OldVal.Poster + " To " + NewVal.Poster); }
            ////    return diffList;
            ////}


            ////#endregion

            ////#region === imdB SQL Functions ===

            ////// Return imdB SQL Connection String
            ////public SqlConnection imdB_Conn()
            ////{
            ////    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["epGuideDb"].ConnectionString);
            ////    // populate sql connection
            ////    return conn;
            ////}

            ////// Return imdB TableName (Full Path)
            ////public string imdB_TableName()
            ////{
            ////    // populate to return full sql table name
            ////    return "[LucidMedia].[dbo].[IMDb_Details]";
            ////}

            ////// returns true/false if found in existing imdb library
            ////public bool IMDb_InLib(string IMDbID = "tt1211837")
            ////{
            ////    SqlConnection Conn = movieParadise_Conn();

            ////    List<string> recordsFound = lst.SQL_To_List(Conn, "select * FROM [LucidMedia].[dbo].[IMDb_Details] where IMDbID = '" + IMDbID + "'");

            ////    if (recordsFound.Count > 0)
            ////    {
            ////        StatusBar("IMDb Title" + IMDbID + " FOUND in Collection");
            ////        return true;
            ////    }

            ////    StatusBar("IMDb Title" + IMDbID + " [NOT] FOUND in Collection");
            ////    return false;

            ////}


            ////public bool imdB_InsertSQL(imdB obj)
            ////{
            ////    SqlConnection Con = imdB_Conn();
            ////    string SQLLine = "Insert Into [LucidMedia].[dbo].[IMDb_Details](IMDbID, Title, OriginalTitle, Year, Rating, Genres, Directors, Writers, Producers, Musicians, Cinematographers, Editors, Cast, Plot, PlotKeywords, ReleaseDate, Runtime, Top250, Oscars, Awards, Nominations, Tagline, MpaaRating, Votes, Languages, Countries, Poster, RootDir, DirSize) VALUES (@IMDbID, @Title, @OriginalTitle, @Year, @Rating, @Genres, @Directors, @Writers, @Producers, @Musicians, @Cinematographers, @Editors, @Cast, @Plot, @PlotKeywords, @ReleaseDate, @Runtime, @Top250, @Oscars, @Awards, @Nominations, @Tagline, @MpaaRating, @Votes, @Languages, @Countries, @Poster, @RootDir, @DirSize)";
            ////    SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
            ////    cmd2 = new SqlCommand(SQLLine, Con);
            ////    if (obj.IMDbID == null) { obj.IMDbID = ""; }
            ////    if (obj.Title == null) { obj.Title = ""; }
            ////    if (obj.OriginalTitle == null) { obj.OriginalTitle = ""; }
            ////    if (obj.Year == null) { obj.Year = ""; }
            ////    if (obj.Rating == null) { obj.Rating = ""; }
            ////    if (obj.Genres == null) { obj.Genres = ""; }
            ////    if (obj.Directors == null) { obj.Directors = ""; }
            ////    if (obj.Writers == null) { obj.Writers = ""; }
            ////    if (obj.Producers == null) { obj.Producers = ""; }
            ////    if (obj.Musicians == null) { obj.Musicians = ""; }
            ////    if (obj.Cinematographers == null) { obj.Cinematographers = ""; }
            ////    if (obj.Editors == null) { obj.Editors = ""; }
            ////    if (obj.Cast == null) { obj.Cast = ""; }
            ////    if (obj.Plot == null) { obj.Plot = ""; }
            ////    if (obj.PlotKeywords == null) { obj.PlotKeywords = ""; }
            ////    if (obj.ReleaseDate == null) { obj.ReleaseDate = ""; }
            ////    if (obj.Runtime == null) { obj.Runtime = ""; }
            ////    if (obj.Top250 == null) { obj.Top250 = ""; }
            ////    if (obj.Oscars == null) { obj.Oscars = ""; }
            ////    if (obj.Awards == null) { obj.Awards = ""; }
            ////    if (obj.Nominations == null) { obj.Nominations = ""; }
            ////    if (obj.Tagline == null) { obj.Tagline = ""; }
            ////    if (obj.MpaaRating == null) { obj.MpaaRating = ""; }
            ////    if (obj.Votes == null) { obj.Votes = ""; }
            ////    if (obj.Languages == null) { obj.Languages = ""; }
            ////    if (obj.Countries == null) { obj.Countries = ""; }
            ////    if (obj.Poster == null) { obj.Poster = ""; }


            ////    string tagline = obj.Tagline.Replace("&#x27;", "");  // was a ' in the tagline
            ////    string plot = obj.Plot.Replace("&#x27;", "");  // was a ' in the tagline

            ////    cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Title", obj.Title.ToString());
            ////    cmd2.Parameters.AddWithValue(@"OriginalTitle", obj.OriginalTitle.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Year", obj.Year.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Rating", obj.Rating.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Genres", obj.Genres.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Directors", obj.Directors.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Writers", obj.Writers.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Producers", obj.Producers.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Musicians", obj.Musicians.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Cinematographers", obj.Cinematographers.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Editors", obj.Editors.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Cast", obj.Cast.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Plot", plot);
            ////    cmd2.Parameters.AddWithValue(@"PlotKeywords", obj.PlotKeywords.ToString());
            ////    cmd2.Parameters.AddWithValue(@"ReleaseDate", obj.ReleaseDate.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Runtime", obj.Runtime.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Top250", obj.Top250.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Oscars", obj.Oscars.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Awards", obj.Awards.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Nominations", obj.Nominations.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Tagline", tagline);
            ////    cmd2.Parameters.AddWithValue(@"MpaaRating", obj.MpaaRating.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Votes", obj.Votes.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Languages", obj.Languages.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Countries", obj.Countries.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Poster", obj.Poster.ToString());
            ////    cmd2.Parameters.AddWithValue(@"RootDir", obj.RootDir.ToString());
            ////    cmd2.Parameters.AddWithValue(@"DirSize", obj.DirSize.ToString());

            ////    if (Con.State == ConnectionState.Closed) { Con.Open(); }
            ////    int recordsAffected = 0;
            ////    try { recordsAffected = cmd2.ExecuteNonQuery(); }
            ////    catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
            ////    Con.Close();
            ////    if (recordsAffected > 0) { return true; }
            ////    else return false;
            ////}

            ////public bool imdB_UpdateSQL(imdB obj)
            ////{
            ////    SqlConnection Conn = imdB_Conn();
            ////    string SQLLine = "Update [LucidMedia].[dbo].[IMDb_Details] SET Title = @Title, OriginalTitle = @OriginalTitle, Year = @Year, Rating = @Rating, Genres = @Genres, Directors = @Directors, Writers = @Writers, Producers = @Producers, Musicians = @Musicians, Cinematographers = @Cinematographers, Editors = @Editors, Cast = @Cast, Plot = @Plot, PlotKeywords = @PlotKeywords, ReleaseDate = @ReleaseDate, Runtime = @Runtime, Top250 = @Top250, Oscars = @Oscars, Awards = @Awards, Nominations = @Nominations, Tagline = @Tagline, MpaaRating = @MpaaRating, Votes = @Votes, Languages = @Languages, Countries = @Countries, Poster = @Poster, RootDir = @RootDir, DirSize = @DirSize WHERE IMDbID = @IMDbID";
            ////    SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
            ////    cmd2 = new SqlCommand(SQLLine, Conn);
            ////    if (obj.IMDbID == null) { obj.IMDbID = ""; }
            ////    if (obj.Title == null) { obj.Title = ""; }
            ////    if (obj.OriginalTitle == null) { obj.OriginalTitle = ""; }
            ////    if (obj.Year == null) { obj.Year = ""; }
            ////    if (obj.Rating == null) { obj.Rating = ""; }
            ////    if (obj.Genres == null) { obj.Genres = ""; }
            ////    if (obj.Directors == null) { obj.Directors = ""; }
            ////    if (obj.Writers == null) { obj.Writers = ""; }
            ////    if (obj.Producers == null) { obj.Producers = ""; }
            ////    if (obj.Musicians == null) { obj.Musicians = ""; }
            ////    if (obj.Cinematographers == null) { obj.Cinematographers = ""; }
            ////    if (obj.Editors == null) { obj.Editors = ""; }
            ////    if (obj.Cast == null) { obj.Cast = ""; }
            ////    if (obj.Plot == null) { obj.Plot = ""; }
            ////    if (obj.PlotKeywords == null) { obj.PlotKeywords = ""; }
            ////    if (obj.ReleaseDate == null) { obj.ReleaseDate = ""; }
            ////    if (obj.Runtime == null) { obj.Runtime = ""; }
            ////    if (obj.Top250 == null) { obj.Top250 = ""; }
            ////    if (obj.Oscars == null) { obj.Oscars = ""; }
            ////    if (obj.Awards == null) { obj.Awards = ""; }
            ////    if (obj.Nominations == null) { obj.Nominations = ""; }
            ////    if (obj.Tagline == null) { obj.Tagline = ""; }
            ////    if (obj.MpaaRating == null) { obj.MpaaRating = ""; }
            ////    if (obj.Votes == null) { obj.Votes = ""; }
            ////    if (obj.Languages == null) { obj.Languages = ""; }
            ////    if (obj.Countries == null) { obj.Countries = ""; }
            ////    if (obj.Poster == null) { obj.Poster = ""; }

            ////    string tagline = obj.Tagline.Replace("&#x27;", "");  // was a ' in the tagline
            ////    string plot = obj.Plot.Replace("&#x27;", "");  // was a ' in the tagline

            ////    cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Title", obj.Title.ToString());
            ////    cmd2.Parameters.AddWithValue(@"OriginalTitle", obj.OriginalTitle.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Year", obj.Year.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Rating", obj.Rating.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Genres", obj.Genres.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Directors", obj.Directors.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Writers", obj.Writers.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Producers", obj.Producers.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Musicians", obj.Musicians.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Cinematographers", obj.Cinematographers.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Editors", obj.Editors.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Cast", obj.Cast.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Plot", plot);
            ////    cmd2.Parameters.AddWithValue(@"PlotKeywords", obj.PlotKeywords.ToString());
            ////    cmd2.Parameters.AddWithValue(@"ReleaseDate", obj.ReleaseDate.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Runtime", obj.Runtime.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Top250", obj.Top250.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Oscars", obj.Oscars.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Awards", obj.Awards.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Nominations", obj.Nominations.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Tagline", tagline);
            ////    cmd2.Parameters.AddWithValue(@"MpaaRating", obj.MpaaRating.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Votes", obj.Votes.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Languages", obj.Languages.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Countries", obj.Countries.ToString());
            ////    cmd2.Parameters.AddWithValue(@"Poster", obj.Poster.ToString());
            ////    cmd2.Parameters.AddWithValue(@"RootDir", obj.RootDir.ToString());
            ////    cmd2.Parameters.AddWithValue(@"DirSize", obj.DirSize.ToString());


            ////    if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
            ////    int recordsAffected = 0;
            ////    try { recordsAffected = cmd2.ExecuteNonQuery(); }
            ////    catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
            ////    Conn.Close();
            ////    if (recordsAffected > 0) { return true; }
            ////    else return false;
            ////}

            ////int imdbUpdateCount = 0;
            ////int imdbInsertCount = 0;

            ////public bool imdB_UpdateInsert(imdB obj)
            ////{
            ////    SqlConnection Conn = imdB_Conn();
            ////    bool Updated = imdB_UpdateSQL(obj);  // try to update record first

            ////    if (Updated) { imdbUpdateCount++; }
            ////    if (!Updated) { Updated = imdB_InsertSQL(obj); imdbInsertCount++; }  // if unable to update, insert new record

            ////    return Updated;
            ////}

            ////// Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            ////public bool imdB_UpdateIfPopulated(imdB obj, string ID = "")
            ////{
            ////    SqlConnection Conn = imdB_Conn();
            ////    string SQLcmd = "Update [LucidMedia].[dbo].[IMDb_Details] SET ";
            ////    if (obj.IMDbID != null) { SQLcmd = SQLcmd + " IMDbID = @IMDbID,"; }
            ////    if (obj.Title != null) { SQLcmd = SQLcmd + " Title = @Title,"; }
            ////    if (obj.OriginalTitle != null) { SQLcmd = SQLcmd + " OriginalTitle = @OriginalTitle,"; }
            ////    if (obj.Year != null) { SQLcmd = SQLcmd + " Year = @Year,"; }
            ////    if (obj.Rating != null) { SQLcmd = SQLcmd + " Rating = @Rating,"; }
            ////    if (obj.Genres != null) { SQLcmd = SQLcmd + " Genres = @Genres,"; }
            ////    if (obj.Directors != null) { SQLcmd = SQLcmd + " Directors = @Directors,"; }
            ////    if (obj.Writers != null) { SQLcmd = SQLcmd + " Writers = @Writers,"; }
            ////    if (obj.Producers != null) { SQLcmd = SQLcmd + " Producers = @Producers,"; }
            ////    if (obj.Musicians != null) { SQLcmd = SQLcmd + " Musicians = @Musicians,"; }
            ////    if (obj.Cinematographers != null) { SQLcmd = SQLcmd + " Cinematographers = @Cinematographers,"; }
            ////    if (obj.Editors != null) { SQLcmd = SQLcmd + " Editors = @Editors,"; }
            ////    if (obj.Cast != null) { SQLcmd = SQLcmd + " Cast = @Cast,"; }
            ////    if (obj.Plot != null) { SQLcmd = SQLcmd + " Plot = @Plot,"; }
            ////    if (obj.PlotKeywords != null) { SQLcmd = SQLcmd + " PlotKeywords = @PlotKeywords,"; }
            ////    if (obj.ReleaseDate != null) { SQLcmd = SQLcmd + " ReleaseDate = @ReleaseDate,"; }
            ////    if (obj.Runtime != null) { SQLcmd = SQLcmd + " Runtime = @Runtime,"; }
            ////    if (obj.Top250 != null) { SQLcmd = SQLcmd + " Top250 = @Top250,"; }
            ////    if (obj.Oscars != null) { SQLcmd = SQLcmd + " Oscars = @Oscars,"; }
            ////    if (obj.Awards != null) { SQLcmd = SQLcmd + " Awards = @Awards,"; }
            ////    if (obj.Nominations != null) { SQLcmd = SQLcmd + " Nominations = @Nominations,"; }
            ////    if (obj.Tagline != null) { SQLcmd = SQLcmd + " Tagline = @Tagline,"; }
            ////    if (obj.MpaaRating != null) { SQLcmd = SQLcmd + " MpaaRating = @MpaaRating,"; }
            ////    if (obj.Votes != null) { SQLcmd = SQLcmd + " Votes = @Votes,"; }
            ////    if (obj.Languages != null) { SQLcmd = SQLcmd + " Languages = @Languages,"; }
            ////    if (obj.Countries != null) { SQLcmd = SQLcmd + " Countries = @Countries,"; }
            ////    if (obj.Poster != null) { SQLcmd = SQLcmd + " Poster = @Poster,"; }
            ////    SQLcmd = ahk.TrimLast(SQLcmd, 1);
            ////    SQLcmd = SQLcmd + " WHERE ID = @ID";

            ////    SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

            ////    if (obj.IMDbID != null) { cmd2.Parameters.AddWithValue(@"IMDbID", obj.IMDbID); }
            ////    if (obj.Title != null) { cmd2.Parameters.AddWithValue(@"Title", obj.Title); }
            ////    if (obj.OriginalTitle != null) { cmd2.Parameters.AddWithValue(@"OriginalTitle", obj.OriginalTitle); }
            ////    if (obj.Year != null) { cmd2.Parameters.AddWithValue(@"Year", obj.Year); }
            ////    if (obj.Rating != null) { cmd2.Parameters.AddWithValue(@"Rating", obj.Rating); }
            ////    if (obj.Genres != null) { cmd2.Parameters.AddWithValue(@"Genres", obj.Genres); }
            ////    if (obj.Directors != null) { cmd2.Parameters.AddWithValue(@"Directors", obj.Directors); }
            ////    if (obj.Writers != null) { cmd2.Parameters.AddWithValue(@"Writers", obj.Writers); }
            ////    if (obj.Producers != null) { cmd2.Parameters.AddWithValue(@"Producers", obj.Producers); }
            ////    if (obj.Musicians != null) { cmd2.Parameters.AddWithValue(@"Musicians", obj.Musicians); }
            ////    if (obj.Cinematographers != null) { cmd2.Parameters.AddWithValue(@"Cinematographers", obj.Cinematographers); }
            ////    if (obj.Editors != null) { cmd2.Parameters.AddWithValue(@"Editors", obj.Editors); }
            ////    if (obj.Cast != null) { cmd2.Parameters.AddWithValue(@"Cast", obj.Cast); }
            ////    if (obj.Plot != null) { cmd2.Parameters.AddWithValue(@"Plot", obj.Plot); }
            ////    if (obj.PlotKeywords != null) { cmd2.Parameters.AddWithValue(@"PlotKeywords", obj.PlotKeywords); }
            ////    if (obj.ReleaseDate != null) { cmd2.Parameters.AddWithValue(@"ReleaseDate", obj.ReleaseDate); }
            ////    if (obj.Runtime != null) { cmd2.Parameters.AddWithValue(@"Runtime", obj.Runtime); }
            ////    if (obj.Top250 != null) { cmd2.Parameters.AddWithValue(@"Top250", obj.Top250); }
            ////    if (obj.Oscars != null) { cmd2.Parameters.AddWithValue(@"Oscars", obj.Oscars); }
            ////    if (obj.Awards != null) { cmd2.Parameters.AddWithValue(@"Awards", obj.Awards); }
            ////    if (obj.Nominations != null) { cmd2.Parameters.AddWithValue(@"Nominations", obj.Nominations); }
            ////    if (obj.Tagline != null) { cmd2.Parameters.AddWithValue(@"Tagline", obj.Tagline); }
            ////    if (obj.MpaaRating != null) { cmd2.Parameters.AddWithValue(@"MpaaRating", obj.MpaaRating); }
            ////    if (obj.Votes != null) { cmd2.Parameters.AddWithValue(@"Votes", obj.Votes); }
            ////    if (obj.Languages != null) { cmd2.Parameters.AddWithValue(@"Languages", obj.Languages); }
            ////    if (obj.Countries != null) { cmd2.Parameters.AddWithValue(@"Countries", obj.Countries); }
            ////    if (obj.Poster != null) { cmd2.Parameters.AddWithValue(@"Poster", obj.Poster); }

            ////    if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
            ////    int recordsAffected = 0;
            ////    try { recordsAffected = cmd2.ExecuteNonQuery(); }
            ////    catch (SqlException ex) { return false; }
            ////    Conn.Close();
            ////    if (recordsAffected > 0) { return true; }
            ////    else return false;
            ////}

            ////public imdB imdB_ReturnSQL(string ID = "")
            ////{
            ////    SqlConnection Conn = imdB_Conn();
            ////    string SelectLine = "Select [IMDbID],[Title],[OriginalTitle],[Year],[Rating],[Genres],[Directors],[Writers],[Producers],[Musicians],[Cinematographers],[Editors],[Cast],[Plot],[PlotKeywords],[ReleaseDate],[Runtime],[Top250],[Oscars],[Awards],[Nominations],[Tagline],[MpaaRating],[Votes],[Languages],[Countries],[Poster] From [LucidMedia].[dbo].[IMDb_Details] WHERE ID = '" + ID + "'";
            ////    DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
            ////    imdB returnObject = new imdB();
            ////    if (ReturnTable != null)
            ////    {
            ////        foreach (DataRow ret in ReturnTable.Rows)
            ////        {
            ////            returnObject.IMDbID = ret["IMDbID"].ToString();
            ////            returnObject.Title = ret["Title"].ToString();
            ////            returnObject.OriginalTitle = ret["OriginalTitle"].ToString();
            ////            returnObject.Year = ret["Year"].ToString();
            ////            returnObject.Rating = ret["Rating"].ToString();
            ////            returnObject.Genres = ret["Genres"].ToString();
            ////            returnObject.Directors = ret["Directors"].ToString();
            ////            returnObject.Writers = ret["Writers"].ToString();
            ////            returnObject.Producers = ret["Producers"].ToString();
            ////            returnObject.Musicians = ret["Musicians"].ToString();
            ////            returnObject.Cinematographers = ret["Cinematographers"].ToString();
            ////            returnObject.Editors = ret["Editors"].ToString();
            ////            returnObject.Cast = ret["Cast"].ToString();
            ////            returnObject.Plot = ret["Plot"].ToString();
            ////            returnObject.PlotKeywords = ret["PlotKeywords"].ToString();
            ////            returnObject.ReleaseDate = ret["ReleaseDate"].ToString();
            ////            returnObject.Runtime = ret["Runtime"].ToString();
            ////            returnObject.Top250 = ret["Top250"].ToString();
            ////            returnObject.Oscars = ret["Oscars"].ToString();
            ////            returnObject.Awards = ret["Awards"].ToString();
            ////            returnObject.Nominations = ret["Nominations"].ToString();
            ////            returnObject.Tagline = ret["Tagline"].ToString();
            ////            returnObject.MpaaRating = ret["MpaaRating"].ToString();
            ////            returnObject.Votes = ret["Votes"].ToString();
            ////            returnObject.Languages = ret["Languages"].ToString();
            ////            returnObject.Countries = ret["Countries"].ToString();
            ////            returnObject.Poster = ret["Poster"].ToString();
            ////            return returnObject;
            ////        }
            ////    }
            ////    return returnObject;
            ////}

            ////public bool inDb(string IMDbID) // check to see if IMDb ID already exists in sql table
            ////{
            ////    List<imdB> found = imdB_ReturnSQLList("Select * FROM [LucidMedia].[dbo].[IMDb_Details] Where IMDbID = '" + IMDbID + "'");
            ////    if (found.Count > 0) { return true; }
            ////    return false;
            ////}

            ////public List<imdB> imdB_ReturnSQLList(string Command)
            ////{
            ////    SqlConnection Conn = imdB_Conn();
            ////    DataTable ReturnTable = sql.GetDataTable(Conn, Command);
            ////    List<imdB> ReturnList = new List<imdB>();
            ////    if (ReturnTable != null)
            ////    {
            ////        foreach (DataRow ret in ReturnTable.Rows)
            ////        {
            ////            imdB returnObject = new imdB();
            ////            returnObject.IMDbID = ret["IMDbID"].ToString();
            ////            returnObject.Title = ret["Title"].ToString();
            ////            returnObject.OriginalTitle = ret["OriginalTitle"].ToString();
            ////            returnObject.Year = ret["Year"].ToString();
            ////            returnObject.Rating = ret["Rating"].ToString();
            ////            returnObject.Genres = ret["Genres"].ToString();
            ////            returnObject.Directors = ret["Directors"].ToString();
            ////            returnObject.Writers = ret["Writers"].ToString();
            ////            returnObject.Producers = ret["Producers"].ToString();
            ////            returnObject.Musicians = ret["Musicians"].ToString();
            ////            returnObject.Cinematographers = ret["Cinematographers"].ToString();
            ////            returnObject.Editors = ret["Editors"].ToString();
            ////            returnObject.Cast = ret["Cast"].ToString();
            ////            returnObject.Plot = ret["Plot"].ToString();
            ////            returnObject.PlotKeywords = ret["PlotKeywords"].ToString();
            ////            returnObject.ReleaseDate = ret["ReleaseDate"].ToString();
            ////            returnObject.Runtime = ret["Runtime"].ToString();
            ////            returnObject.Top250 = ret["Top250"].ToString();
            ////            returnObject.Oscars = ret["Oscars"].ToString();
            ////            returnObject.Awards = ret["Awards"].ToString();
            ////            returnObject.Nominations = ret["Nominations"].ToString();
            ////            returnObject.Tagline = ret["Tagline"].ToString();
            ////            returnObject.MpaaRating = ret["MpaaRating"].ToString();
            ////            returnObject.Votes = ret["Votes"].ToString();
            ////            returnObject.Languages = ret["Languages"].ToString();
            ////            returnObject.Countries = ret["Countries"].ToString();
            ////            returnObject.Poster = ret["Poster"].ToString();
            ////            ReturnList.Add(returnObject);
            ////        }
            ////    }
            ////    return ReturnList;
            ////}


            ////public bool imdB_UpdatePosterPath(string IMDbID, string PosterPath)
            ////{
            ////    SqlConnection Conn = imdB_Conn();
            ////    string SQLcmd = "Update [LucidMedia].[dbo].[IMDb_Details] SET Poster = '" + PosterPath + "' WHERE [IMDbID] = '" + IMDbID + "'";

            ////    SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

            ////    if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
            ////    int recordsAffected = 0;
            ////    try { recordsAffected = cmd2.ExecuteNonQuery(); }
            ////    catch (SqlException ex) { return false; }
            ////    Conn.Close();
            ////    if (recordsAffected > 0) { return true; }
            ////    else return false;
            ////}





            ////#endregion

            ////#endregion


            //#region === IMDB Search (testing) ===





            int _state = 0;
            ////Creating a new instance of WinForms WebBrowser class
            //System.Windows.Forms.WebBrowser _ieBrowser = new System.Windows.Forms.WebBrowser();

            List<string> results = new List<string>();
            List<string> errors = new List<string>();

            public List<string> ImdbSearch(WebBrowser browser, string searchTerm = "Lie to Me")
            {
                //System.Windows.Forms.WebBrowser _ieBrowser = webBrowser1;

                browser.ScriptErrorsSuppressed = true;
                //webBrowser1.DocumentCompleted += (send, ev) =>
                //{
                //    //button1.IsEnabled = false;
                //    if (_state == 0)
                //    {
                //        webBrowser1.Document.GetElementById("navbar-query").SetAttribute("value", searchTerm);
                //        //Choosing the existing value for quicksearch element in HTML document
                //        webBrowser1.Document.GetElementById("quicksearch").SetAttribute("value", "tt");
                //        //Click the button to accept the search query
                //        HtmlElement acceptButton = webBrowser1.Document.GetElementById("navbar-submit-button");
                //        if (acceptButton != null)
                //        {
                //            acceptButton.InvokeMember("click");
                //        }
                //        _state = 1;

                //        txtWarezBB.Text = webBrowser1.Url.ToString(); 
                //    }
                //    else if (_state == 1)
                //    {
                //        HtmlElementCollection tables = webBrowser1.Document.GetElementsByTagName("table");
                //        try
                //        {
                //            if (tables.Count <= 0) return;
                //            HtmlElementCollection rows = tables[0].GetElementsByTagName("tr");
                //            foreach (HtmlElement row in rows)
                //            {
                //                HtmlElementCollection cells = row.GetElementsByTagName("td");
                //                foreach (HtmlElement cell in cells)
                //                {
                //                    String text = cell.InnerText;
                //                    if (!String.IsNullOrEmpty(text) && !String.IsNullOrWhiteSpace(text))
                //                    {
                //                        results.Add(text);
                //                    }
                //                }
                //            }
                //            _state = 2;

                //            txtWarezBB.Text = webBrowser1.Url.ToString();
                //        }
                //        catch (ArgumentOutOfRangeException exc)
                //        {
                //            errors.Add(exc.Message);
                //        }
                //    }
                //};


                browser.DocumentCompleted += (send, ev) =>
                {
                    ////button1.IsEnabled = false;
                    //if (_state == 0)
                    //{
                    //    webBrowser1.Document.GetElementById("navbar-query").SetAttribute("value", searchTerm);
                    //    //Choosing the existing value for quicksearch element in HTML document
                    //    webBrowser1.Document.GetElementById("quicksearch").SetAttribute("value", "tt");
                    //    //Click the button to accept the search query
                    //    HtmlElement acceptButton = webBrowser1.Document.GetElementById("navbar-submit-button");
                    //    if (acceptButton != null)
                    //    {
                    //        acceptButton.InvokeMember("click");
                    //    }
                    //    _state = 1;

                    //    txtWarezBB.Text = webBrowser1.Url.ToString();
                    //}

                    if (_state == 1)
                    {
                        HtmlElementCollection tables = browser.Document.GetElementsByTagName("table");
                        try
                        {
                            if (tables.Count <= 0) return;
                            HtmlElementCollection rows = tables[0].GetElementsByTagName("tr");
                            foreach (HtmlElement row in rows)
                            {
                                HtmlElementCollection cells = row.GetElementsByTagName("td");
                                foreach (HtmlElement cell in cells)
                                {
                                    String text = cell.InnerText;
                                    if (!String.IsNullOrEmpty(text) && !String.IsNullOrWhiteSpace(text))
                                    {
                                        results.Add(text);
                                    }
                                }
                            }
                            _state = 2;


                            ahk.MsgBox(browser.Url.ToString() + "\n" + results.Count + " Results");
                            //txtURLBar.Text = browser.Url.ToString();
                            //btnImdbSearch.Text = results.Count + " Results";
                        }
                        catch (ArgumentOutOfRangeException exc)
                        {
                            errors.Add(exc.Message.ToString());
                        }
                    }
                };


                return results;
            }

            public void imdbSearchAction(WebBrowser browser, string searchTerm = "Lie to Me")
            {
                browser.Document.GetElementById("navbar-query").SetAttribute("value", searchTerm);
                //Choosing the existing value for quicksearch element in HTML document
                browser.Document.GetElementById("quicksearch").SetAttribute("value", "tt");
                //Click the button to accept the search query
                HtmlElement acceptButton = browser.Document.GetElementById("navbar-submit-button");
                if (acceptButton != null)
                {
                    acceptButton.InvokeMember("click");
                }
                _state = 1;

                _StatusBar sb = new _StatusBar(); sb.StatusBar("Completed Search For " + searchTerm);

            }




            //#endregion

            #endregion

            #region === IMDB Search (testing) ===



            //int _state = 0;
            //////Creating a new instance of WinForms WebBrowser class
            ////System.Windows.Forms.WebBrowser _ieBrowser = new System.Windows.Forms.WebBrowser();

            //List<string> results = new List<string>();
            //List<string> errors = new List<string>();

            //public List<string> ImdbSearch(string searchTerm = "Lie to Me")
            //{
            //    //System.Windows.Forms.WebBrowser _ieBrowser = webBrowser1;

            //    webBrowser1.ScriptErrorsSuppressed = true;
            //    //webBrowser1.DocumentCompleted += (send, ev) =>
            //    //{
            //    //    //button1.IsEnabled = false;
            //    //    if (_state == 0)
            //    //    {
            //    //        webBrowser1.Document.GetElementById("navbar-query").SetAttribute("value", searchTerm);
            //    //        //Choosing the existing value for quicksearch element in HTML document
            //    //        webBrowser1.Document.GetElementById("quicksearch").SetAttribute("value", "tt");
            //    //        //Click the button to accept the search query
            //    //        HtmlElement acceptButton = webBrowser1.Document.GetElementById("navbar-submit-button");
            //    //        if (acceptButton != null)
            //    //        {
            //    //            acceptButton.InvokeMember("click");
            //    //        }
            //    //        _state = 1;

            //    //        txtWarezBB.Text = webBrowser1.Url.ToString(); 
            //    //    }
            //    //    else if (_state == 1)
            //    //    {
            //    //        HtmlElementCollection tables = webBrowser1.Document.GetElementsByTagName("table");
            //    //        try
            //    //        {
            //    //            if (tables.Count <= 0) return;
            //    //            HtmlElementCollection rows = tables[0].GetElementsByTagName("tr");
            //    //            foreach (HtmlElement row in rows)
            //    //            {
            //    //                HtmlElementCollection cells = row.GetElementsByTagName("td");
            //    //                foreach (HtmlElement cell in cells)
            //    //                {
            //    //                    String text = cell.InnerText;
            //    //                    if (!String.IsNullOrEmpty(text) && !String.IsNullOrWhiteSpace(text))
            //    //                    {
            //    //                        results.Add(text);
            //    //                    }
            //    //                }
            //    //            }
            //    //            _state = 2;

            //    //            txtWarezBB.Text = webBrowser1.Url.ToString();
            //    //        }
            //    //        catch (ArgumentOutOfRangeException exc)
            //    //        {
            //    //            errors.Add(exc.Message);
            //    //        }
            //    //    }
            //    //};


            //    webBrowser1.DocumentCompleted += (send, ev) =>
            //    {
            //        ////button1.IsEnabled = false;
            //        //if (_state == 0)
            //        //{
            //        //    webBrowser1.Document.GetElementById("navbar-query").SetAttribute("value", searchTerm);
            //        //    //Choosing the existing value for quicksearch element in HTML document
            //        //    webBrowser1.Document.GetElementById("quicksearch").SetAttribute("value", "tt");
            //        //    //Click the button to accept the search query
            //        //    HtmlElement acceptButton = webBrowser1.Document.GetElementById("navbar-submit-button");
            //        //    if (acceptButton != null)
            //        //    {
            //        //        acceptButton.InvokeMember("click");
            //        //    }
            //        //    _state = 1;

            //        //    txtWarezBB.Text = webBrowser1.Url.ToString();
            //        //}

            //        if (_state == 1)
            //        {
            //            HtmlElementCollection tables = webBrowser1.Document.GetElementsByTagName("table");
            //            try
            //            {
            //                if (tables.Count <= 0) return;
            //                HtmlElementCollection rows = tables[0].GetElementsByTagName("tr");
            //                foreach (HtmlElement row in rows)
            //                {
            //                    HtmlElementCollection cells = row.GetElementsByTagName("td");
            //                    foreach (HtmlElement cell in cells)
            //                    {
            //                        String text = cell.InnerText;
            //                        if (!String.IsNullOrEmpty(text) && !String.IsNullOrWhiteSpace(text))
            //                        {
            //                            results.Add(text);
            //                        }
            //                    }
            //                }
            //                _state = 2;

            //                txtURLBar.Text = webBrowser1.Url.ToString();
            //                btnImdbSearch.Text = results.Count + " Results";
            //            }
            //            catch (ArgumentOutOfRangeException exc)
            //            {
            //                errors.Add(exc.Message.ToString());
            //            }
            //        }
            //    };


            //    return results;
            //}

            public void imdbSearchAction(string searchTerm = "Lie to Me")
            {
                //webBrowser1.Document.GetElementById("navbar-query").SetAttribute("value", searchTerm);
                ////Choosing the existing value for quicksearch element in HTML document
                //webBrowser1.Document.GetElementById("quicksearch").SetAttribute("value", "tt");
                ////Click the button to accept the search query
                //HtmlElement acceptButton = webBrowser1.Document.GetElementById("navbar-submit-button");
                //if (acceptButton != null)
                //{
                //    acceptButton.InvokeMember("click");
                //}
                //_state = 1;

                //StatusBar("Completed Search For " + searchTerm);

            }




            #endregion


            #endregion



            #region === IMDb GUI ===

            public void MovieTreeView(imdB movie, TreeView TV, bool ClearTV = true)
            {
                if (TV == null) { return; }
                if (ClearTV) { _TreeViewControl tv = new _TreeViewControl(); tv.ClearTV(TV); }

                TreeNode MovieNode = new TreeNode();  // level 1
                MovieNode.Text = movie.Title + " (" + movie.Year + ")";
                MovieNode.Tag = movie.IMDbID + "|" + movie.Title + "|" + movie.OriginalTitle;

                //=== Populate IMDb TreeView ===========================
                TreeNode About = new TreeNode();  // level 1
                About.Text = "About";

                TreeNode Asection = new TreeNode();  // level 2
                Asection.Text = "MPAA Rating = " + movie.MpaaRating;
                Asection.Tag = movie.IMDbID + "|About";
                About.Nodes.Add(Asection);

                Asection = new TreeNode();  // level 2
                Asection.Text = "DirSize = " + movie.DirSize;
                Asection.Tag = movie.IMDbID + "|About";
                About.Nodes.Add(Asection);

                Asection = new TreeNode();  // level 2
                Asection.Text = "Release Date = " + movie.ReleaseDate;
                Asection.Tag = movie.IMDbID + "|About";
                About.Nodes.Add(Asection);

                Asection = new TreeNode();  // level 2
                Asection.Text = "Dir = " + movie.RootDir;
                Asection.Tag = movie.IMDbID + "|About";
                About.Nodes.Add(Asection);

                Asection = new TreeNode();  // level 2
                Asection.Text = "RunTime = " + movie.Runtime;
                Asection.Tag = movie.IMDbID + "|About";
                About.Nodes.Add(Asection);

                MovieNode.Nodes.Add(About);  // populate tree


                //=== Populate IMDb TreeView ===========================
                TreeNode Awards = new TreeNode();  // level 1
                Awards.Text = "Awards";
                Asection = new TreeNode();  // level 2
                Asection.Text = "Awards = " + movie.Awards;
                Asection.Tag = movie.IMDbID + "|Awards";
                Awards.Nodes.Add(Asection);

                Asection = new TreeNode();  // level 2
                Asection.Text = "Nominations = " + movie.Nominations;
                Asection.Tag = movie.IMDbID + "|Awards";
                Awards.Nodes.Add(Asection);

                Asection = new TreeNode();  // level 2
                Asection.Text = "Oscars = " + movie.Oscars;
                Asection.Tag = movie.IMDbID + "|Awards";
                Awards.Nodes.Add(Asection);

                Asection = new TreeNode();  // level 2
                Asection.Text = "Top250 = " + movie.Top250;
                Asection.Tag = movie.IMDbID + "|Awards";
                Awards.Nodes.Add(Asection);

                Asection = new TreeNode();  // level 2
                Asection.Text = "Rating = " + movie.Rating + " (" + movie.Votes + ")";
                Asection.Tag = movie.IMDbID + "|Awards";
                Awards.Nodes.Add(Asection);

                MovieNode.Nodes.Add(Awards);  // populate tree


                //=== Populate IMDb TreeView ===========================
                List<string> castList = ahk.StringSplit_List(movie.Cast, "|");
                TreeNode Cast = new TreeNode();  // level 1
                Cast.Text = "Cast";
                foreach (string actor in castList)
                {
                    TreeNode section = new TreeNode();  // level 2
                    section.Text = actor;
                    section.Tag = movie.IMDbID + "|Cast|" + actor;
                    Cast.Nodes.Add(section);
                }
                MovieNode.Nodes.Add(Cast);  // populate tree


                //=== Populate IMDb TreeView ===========================
                List<string> directorList = ahk.StringSplit_List(movie.Directors, "|");
                TreeNode Directors = new TreeNode();  // level 1
                Directors.Text = "Directors";
                foreach (string actor in directorList)
                {
                    TreeNode section = new TreeNode();  // level 2
                    section.Text = actor;
                    section.Tag = movie.IMDbID + "|Directors|" + actor;
                    Directors.Nodes.Add(section);
                }
                MovieNode.Nodes.Add(Directors);  // populate tree

                //=== Populate IMDb TreeView ===========================
                List<string> CinematographersList = ahk.StringSplit_List(movie.Cinematographers, "|");
                TreeNode Cinematographers = new TreeNode();  // level 1
                Cinematographers.Text = "Cinematographers";
                foreach (string actor in CinematographersList)
                {
                    TreeNode section = new TreeNode();  // level 2
                    section.Text = actor;
                    section.Tag = movie.IMDbID + "|Cinematographers|" + actor;
                    Cinematographers.Nodes.Add(section);
                }
                MovieNode.Nodes.Add(Cinematographers);  // populate tree

                //=== Populate IMDb TreeView ===========================
                List<string> CountriesList = ahk.StringSplit_List(movie.Countries, "|");
                TreeNode Countries = new TreeNode();  // level 1
                Countries.Text = "Countries";
                foreach (string actor in CountriesList)
                {
                    TreeNode section = new TreeNode();  // level 2
                    section.Text = actor;
                    section.Tag = movie.IMDbID + "|Countries|" + actor;
                    Countries.Nodes.Add(section);
                }
                MovieNode.Nodes.Add(Countries);  // populate tree

                //=== Populate IMDb TreeView ===========================
                List<string> EditorsList = ahk.StringSplit_List(movie.Editors, "|");
                TreeNode Editors = new TreeNode();  // level 1
                Editors.Text = "Editors";
                foreach (string actor in EditorsList)
                {
                    TreeNode section = new TreeNode();  // level 2
                    section.Text = actor;
                    section.Tag = movie.IMDbID + "|Editors|" + actor;
                    Editors.Nodes.Add(section);
                }
                MovieNode.Nodes.Add(Editors);  // populate tree

                //=== Populate IMDb TreeView ===========================
                List<string> GenresList = ahk.StringSplit_List(movie.Genres, "|");
                TreeNode Genres = new TreeNode();  // level 1
                Genres.Text = "Genres";
                foreach (string actor in GenresList)
                {
                    TreeNode section = new TreeNode();  // level 2
                    section.Text = actor;
                    section.Tag = movie.IMDbID + "|Genres|" + actor;
                    Genres.Nodes.Add(section);
                }
                MovieNode.Nodes.Add(Genres);  // populate tree

                //=== Populate IMDb TreeView ===========================
                List<string> LanguagesList = ahk.StringSplit_List(movie.Languages, "|");
                TreeNode Languages = new TreeNode();  // level 1
                Languages.Text = "Languages";
                foreach (string actor in LanguagesList)
                {
                    TreeNode section = new TreeNode();  // level 2
                    section.Text = actor;
                    section.Tag = movie.IMDbID + "|Languages|" + actor;
                    Languages.Nodes.Add(section);
                }
                MovieNode.Nodes.Add(Languages);  // populate tree


                //=== Populate IMDb TreeView ===========================
                List<string> MusiciansList = ahk.StringSplit_List(movie.Musicians, "|");
                TreeNode Musicians = new TreeNode();  // level 1
                Musicians.Text = "Musicians";
                foreach (string actor in MusiciansList)
                {
                    TreeNode section = new TreeNode();  // level 2
                    section.Text = actor;
                    section.Tag = movie.IMDbID + "|Musicians|" + actor;
                    Musicians.Nodes.Add(section);
                }
                MovieNode.Nodes.Add(Musicians);  // populate tree

                //=== Populate IMDb TreeView ===========================
                List<string> ProducersList = ahk.StringSplit_List(movie.Producers, "|");
                TreeNode Producers = new TreeNode();  // level 1
                Producers.Text = "Producers";
                foreach (string actor in ProducersList)
                {
                    TreeNode section = new TreeNode();  // level 2
                    section.Text = actor;
                    section.Tag = movie.IMDbID + "|Producers|" + actor;
                    Producers.Nodes.Add(section);
                }
                MovieNode.Nodes.Add(Producers);  // populate tree

                //=== Populate IMDb TreeView ===========================
                List<string> WritersList = ahk.StringSplit_List(movie.Writers, "|");
                TreeNode Writers = new TreeNode();  // level 1
                Writers.Text = "Writers";
                foreach (string actor in WritersList)
                {
                    TreeNode section = new TreeNode();  // level 2
                    section.Text = actor;
                    section.Tag = movie.IMDbID + "|Writers|" + actor;
                    Writers.Nodes.Add(section);
                }
                MovieNode.Nodes.Add(Writers);  // populate tree

                //=== Populate IMDb TreeView ===========================
                List<string> PlotKeywordsList = ahk.StringSplit_List(movie.PlotKeywords, "|");
                TreeNode PlotKeywords = new TreeNode();  // level 1
                PlotKeywords.Text = "PlotKeywords";
                foreach (string actor in PlotKeywordsList)
                {
                    TreeNode section = new TreeNode();  // level 2
                    section.Text = actor;
                    section.Tag = movie.IMDbID + "|PlotKeywords|" + actor;
                    PlotKeywords.Nodes.Add(section);
                }
                MovieNode.Nodes.Add(PlotKeywords);  // populate tree


                TV.Nodes.Add(MovieNode);

            }

            public void LoadMoviePoster(imdB movie, PictureBox PicBox)
            {
                if (PicBox == null) { return; }
                // find movie poster from local imdb cache
                string image = ahk.AppDir() + "\\_Imdb\\" + movie.IMDbID + ".jpg";
                if (File.Exists(ahk.AppDir() + "\\_Imdb\\" + movie.IMDbID + "_Large.jpg")) { image = ahk.AppDir() + "\\_Imdb\\" + movie.IMDbID + "_Large.jpg"; }
                if (File.Exists(ahk.AppDir() + "\\_Imdb\\" + movie.IMDbID + "_Full.jpg")) { image = ahk.AppDir() + "\\_Imdb\\" + movie.IMDbID + "_Full.jpg"; }

                _Images img = new _Images();
                if (File.Exists(image))
                {
                    img.Load_PictureBox(PicBox, image, true);
                }
            }


            #region #### IMDb RadTreeView ####

            /// <summary>
            /// Display IMDb Object in RadTreeView
            /// </summary>
            /// <param name="radTree">RadTreeView Control</param>
            /// <param name="movie">Populated IMDb Object</param>
            public void IMDb_TreeView(RadTreeView radTree, _Sites.Imdb.imdB movie)
            {
                if (radTree == null) { return; }
                //RadTreeView radTree = radTreeView1;

                RadTreeNode castNode = new RadTreeNode("Cast");
                RadTreeNode directorsNode = new RadTreeNode("Directors");
                RadTreeNode editorsNode = new RadTreeNode("Editors");
                RadTreeNode musiciansNode = new RadTreeNode("Musicians");
                RadTreeNode writersNode = new RadTreeNode("Writers");
                RadTreeNode producersNode = new RadTreeNode("Producers");
                RadTreeNode languagesNode = new RadTreeNode("Languages");
                RadTreeNode genresNode = new RadTreeNode("Genres");
                RadTreeNode countriesNode = new RadTreeNode("Countries");
                RadTreeNode cinematographersNode = new RadTreeNode("Cinematographers");
                RadTreeNode keywordsNode = new RadTreeNode("PlotKeywords");


                DataTable castTable = new DataTable();
                castTable.Columns.Add("Cast", typeof(string));
                List<string> castList = ahk.StringSplit_List(movie.Cast, "|");
                foreach (string person in castList) { castNode.Nodes.Add(person); castTable.Rows.Add(person); }

                DataTable directorTable = new DataTable();
                directorTable.Columns.Add("Directors", typeof(string));
                List<string> directorList = ahk.StringSplit_List(movie.Directors, "|");
                foreach (string person in directorList) { directorTable.Rows.Add(person); directorsNode.Nodes.Add(person); }

                DataTable editorsTable = new DataTable();
                editorsTable.Columns.Add("editors", typeof(string));
                List<string> editorsList = ahk.StringSplit_List(movie.Editors, "|");
                foreach (string person in editorsList) { editorsTable.Rows.Add(person); editorsNode.Nodes.Add(person); }

                DataTable musiciansTable = new DataTable();
                musiciansTable.Columns.Add("musicians", typeof(string));
                List<string> musiciansList = ahk.StringSplit_List(movie.Musicians, "|");
                foreach (string person in musiciansList) { musiciansTable.Rows.Add(person); musiciansNode.Nodes.Add(person); }

                DataTable writersTable = new DataTable();
                writersTable.Columns.Add("writers", typeof(string));
                List<string> writersList = ahk.StringSplit_List(movie.Writers, "|");
                foreach (string person in writersList) { writersTable.Rows.Add(person); writersNode.Nodes.Add(person); }

                DataTable producersTable = new DataTable();
                producersTable.Columns.Add("producers", typeof(string));
                List<string> producersList = ahk.StringSplit_List(movie.Producers, "|");
                foreach (string person in producersList) { producersTable.Rows.Add(person); producersNode.Nodes.Add(person); }

                DataTable languagesTable = new DataTable();
                languagesTable.Columns.Add("languages", typeof(string));
                List<string> languagesList = ahk.StringSplit_List(movie.Languages, "|");
                foreach (string person in languagesList) { languagesTable.Rows.Add(person); languagesNode.Nodes.Add(person); }

                DataTable genresTable = new DataTable();
                genresTable.Columns.Add("genres", typeof(string));
                List<string> genresList = ahk.StringSplit_List(movie.Genres, "|");
                foreach (string person in genresList) { genresTable.Rows.Add(person); genresNode.Nodes.Add(person); }

                DataTable countriesTable = new DataTable();
                countriesTable.Columns.Add("countries", typeof(string));
                List<string> countriesList = ahk.StringSplit_List(movie.Countries, "|");
                foreach (string person in countriesList) { countriesTable.Rows.Add(person); countriesNode.Nodes.Add(person); }

                DataTable cinematographersTable = new DataTable();
                cinematographersTable.Columns.Add("cinematographers", typeof(string));
                List<string> cinematographersList = ahk.StringSplit_List(movie.Cinematographers, "|");
                foreach (string person in cinematographersList) { cinematographersTable.Rows.Add(person); cinematographersNode.Nodes.Add(person); }

                DataTable plotsTable = new DataTable();
                plotsTable.Columns.Add("Plot Keywords", typeof(string));
                List<string> plotsList = ahk.StringSplit_List(movie.PlotKeywords, "|");
                foreach (string person in plotsList) { plotsTable.Rows.Add(person); keywordsNode.Nodes.Add(person); }

                _TelerikLib.RadTree tree = new _TelerikLib.RadTree();

                // Assemble TreeView
                tree.AddNode(radTree, castNode);
                tree.AddNode(radTree, directorsNode);
                tree.AddNode(radTree, editorsNode);
                tree.AddNode(radTree, musiciansNode);
                tree.AddNode(radTree, writersNode);
                tree.AddNode(radTree, producersNode);
                tree.AddNode(radTree, languagesNode);
                tree.AddNode(radTree, genresNode);
                tree.AddNode(radTree, countriesNode);
                tree.AddNode(radTree, cinematographersNode);
                tree.AddNode(radTree, keywordsNode);
            }


            /// <summary>
            /// Search Dir for IMDb .URL Files, Parse IMDb Info, Display in RadTreeView 
            /// </summary>
            /// <param name="radTree">RadTreeView Control to Populate</param>
            /// <param name="DirPath">Directory containing Subfolders with .URL Files</param>
            /// <param name="DisplayName">First Node Text</param>
            /// <param name="progress1">RadProgressBar To Display Progress</param>
            public void IMDb_URL_Tree(RadTreeView radTree, string DirPath, string DisplayName = "IMDb_Index", Telerik.WinControls.UI.RadProgressBar progress1 = null)
            {
                if (radTree == null) { return; }
                Thread addThread = new Thread(() => _IMDb_URL_Tree(radTree, DirPath, DisplayName, progress1));
                addThread.Start();
            }

            private void _IMDb_URL_Tree(RadTreeView radTree, string DirPath, string DisplayName = "IMDb_Index", Telerik.WinControls.UI.RadProgressBar progress1 = null, bool WriteOMDbSQLite = true)
            {
                if (radTree == null) { return; }

                _Sites.OMDB omdb = new OMDB();
                _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();
                _AHK ahk = new _AHK();
                _Lists lst = new _Lists();
                _Sites.Imdb imdb = new _Sites.Imdb();
                _TelerikLib.RadProgress rad = new _TelerikLib.RadProgress();
                _TelerikLib tel = new _TelerikLib();
                _TelerikLib.RadTree tree = new _TelerikLib.RadTree();

                string listName = "";
                if (DisplayName == "DirName") { listName = ahk.DirName(DirPath); }
                else { listName = DisplayName; }

                //if (!Directory.Exists(DirPath)) { continue; }
                //sb.StatusBar("IMDb Index Update : Collecting IMDb Links... [ " + i + " / " + SearchDirs.Count + " ]");

                string[] files = Directory.GetFiles(DirPath, "*.url", System.IO.SearchOption.AllDirectories);

                pro.ResetProgress(progress1);
                pro.SetupProgressBar(progress1, files.Count());


                RadTreeNode Node1 = new RadTreeNode(ahk.DirName(DirPath));

                tree.AddNode(radTree, Node1);

                int count = 1;
                foreach (string file in files)
                {
                    pro.UpdateProgress(progress1, count + "/" + files.Count());
                    //sb.StatusBar("Updating IMDb Movie Index : " + count.ToString() + " / " + files.Count().ToString() + " | Updated " + imdbUpdateCount + " | NEW " + imdbInsertCount);

                    string URLPath = ahk.IniRead(file, "InternetShortcut", "URL");
                    if (URLPath.ToUpper().Contains("IMDB.COM"))
                    {
                        // read imdb id from url, lookup movie info from OMDb API
                        imdB Movie = omdb.OMdb(ParseID(URLPath));

                        // Option to Write Write OMDb Movie Data to Local SQLite Db
                        if (WriteOMDbSQLite) { bool added = imdb.imdB_UpdateInsert(ahk.AppDir() + "\\IMDb.sqlite", Movie); }


                        string fileDir = ahk.FileDir(file);
                        //Movie.RootDir = fileDir;
                        //long dirsize = _AHK.DirSize(fileDir);
                        //Movie.DirSize = ahk.FormatBytes(dirsize);
                        Movie.DirSize = ahk.DirSize(fileDir);
                        Movie.RootDir = fileDir;

                        string localImgPath = @"D:\_Images\_Imdb\" + Movie.IMDbID + "\\" + Movie.IMDbID + ".jpg";

                        if (File.Exists(localImgPath)) { Movie.PosterImage = localImgPath; }
                        else { Movie.PosterImage = ""; }


                        // PREVIOUS VERSION - NOT AS ACCURATE FROM IMDB PARSE
                        //imdB Movie = parse_IMDb_Page(URLPath, file, SkipExisting, WriteMissingToDb);

                        //if (Movie.Title == "" || Movie.Title == null) { continue; }

                        RadTreeNode Node4 = new RadTreeNode();
                        Node4.Text = ahk.DirName(file);

                        Node4.Tag = fileDir;

                        //Node4.Tag = DirPath + "\\" + file;
                        Node4.Name = Movie.IMDbID;

                        tree.AddSubNode(Node1, Node4);
                    }

                    count++;

                    //if (count == 20) { break; }
                }

            }




            #endregion


            #endregion


    
            #region === IMDb SQLite ===

            #region ===== imdB SQLite : Return =====

            public imdB Return_IMDbLocal(string IMDbID = "", string DbFile = "")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + "\\IMDb.sqlite"; }

                string SelectLine = "Select [IMDbID], [Title], [OriginalTitle], [Year], [Rating], [Genres], [Directors], [Writers], [Producers], [Musicians], [Cinematographers], [Editors], [Cast], [Plot], [PlotKeywords], [ReleaseDate], [Runtime], [Top250], [Oscars], [Awards], [Nominations], [Tagline], [MpaaRating], [Votes], [Languages], [Countries], [Poster], [RootDir], [DirSize], [PosterImage] From [IMDbLocal] WHERE [IMDbID] = '" + IMDbID + "'";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

                imdB returnObject = new imdB();
                int i = 0;
                string Value = "";
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.Title = ret["Title"].ToString();
                        returnObject.OriginalTitle = ret["OriginalTitle"].ToString();
                        returnObject.Year = ret["Year"].ToString();
                        returnObject.Rating = ret["Rating"].ToString();
                        returnObject.Genres = ret["Genres"].ToString();
                        returnObject.Directors = ret["Directors"].ToString();
                        returnObject.Writers = ret["Writers"].ToString();
                        returnObject.Producers = ret["Producers"].ToString();
                        returnObject.Musicians = ret["Musicians"].ToString();
                        returnObject.Cinematographers = ret["Cinematographers"].ToString();
                        returnObject.Editors = ret["Editors"].ToString();
                        returnObject.Cast = ret["Cast"].ToString();
                        returnObject.Plot = ret["Plot"].ToString();
                        returnObject.PlotKeywords = ret["PlotKeywords"].ToString();
                        returnObject.ReleaseDate = ret["ReleaseDate"].ToString();
                        returnObject.Runtime = ret["Runtime"].ToString();
                        returnObject.Top250 = ret["Top250"].ToString();
                        returnObject.Oscars = ret["Oscars"].ToString();
                        returnObject.Awards = ret["Awards"].ToString();
                        returnObject.Nominations = ret["Nominations"].ToString();
                        returnObject.Tagline = ret["Tagline"].ToString();
                        returnObject.MpaaRating = ret["MpaaRating"].ToString();
                        returnObject.Votes = ret["Votes"].ToString();
                        returnObject.Languages = ret["Languages"].ToString();
                        returnObject.Countries = ret["Countries"].ToString();
                        returnObject.Poster = ret["Poster"].ToString();
                        returnObject.RootDir = ret["RootDir"].ToString();
                        returnObject.DirSize = ret["DirSize"].ToString();
                        returnObject.PosterImage = ret["PosterImage"].ToString();
                    }
                }

                returnObject = DecodeObj(returnObject);

                return returnObject;
            }

            public List<imdB> Return_imdB_List(string DbFile, string TableName = "[IMDbLocal]", string TitleID = "")
            {
                string SelectLine = "Select * From " + TableName + " WHERE [IMDbID] = '" + TitleID + "'";

                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

                List<imdB> ReturnList = new List<imdB>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        imdB returnObject = new imdB();

                        returnObject.IMDbID = Decode(returnObject.IMDbID);

                        returnObject.IMDbID = ret["IMDbID"].ToString();
                        returnObject.Title = ret["Title"].ToString();
                        returnObject.OriginalTitle = ret["OriginalTitle"].ToString();
                        returnObject.Year = ret["Year"].ToString();
                        returnObject.Rating = ret["Rating"].ToString();
                        returnObject.Genres = ret["Genres"].ToString();
                        returnObject.Directors = ret["Directors"].ToString();
                        returnObject.Writers = ret["Writers"].ToString();
                        returnObject.Producers = ret["Producers"].ToString();
                        returnObject.Musicians = ret["Musicians"].ToString();
                        returnObject.Cinematographers = ret["Cinematographers"].ToString();
                        returnObject.Editors = ret["Editors"].ToString();
                        returnObject.Cast = ret["Cast"].ToString();
                        returnObject.Plot = ret["Plot"].ToString();
                        returnObject.PlotKeywords = ret["PlotKeywords"].ToString();
                        returnObject.ReleaseDate = ret["ReleaseDate"].ToString();
                        returnObject.Runtime = ret["Runtime"].ToString();
                        returnObject.Top250 = ret["Top250"].ToString();
                        returnObject.Oscars = ret["Oscars"].ToString();
                        returnObject.Awards = ret["Awards"].ToString();
                        returnObject.Nominations = ret["Nominations"].ToString();
                        returnObject.Tagline = ret["Tagline"].ToString();
                        returnObject.MpaaRating = ret["MpaaRating"].ToString();
                        returnObject.Votes = ret["Votes"].ToString();
                        returnObject.Languages = ret["Languages"].ToString();
                        returnObject.Countries = ret["Countries"].ToString();
                        returnObject.Poster = ret["Poster"].ToString();
                        returnObject.RootDir = ret["RootDir"].ToString();
                        returnObject.DirSize = ret["DirSize"].ToString();
                        returnObject.PosterImage = ret["PosterImage"].ToString();

                        returnObject = DecodeObj(returnObject);

                        ReturnList.Add(returnObject);
                    }
                }

                return ReturnList;
            }

            /// <summary>
            /// Decodes Encoded Text (Encoded When Writing To DB To Fix Illegal Chars)
            /// </summary>
            /// <param name="returnObject"></param>
            /// <returns></returns>
            public imdB DecodeObj(imdB returnObject)
            {
                imdB eturnObject = new imdB();

                eturnObject.IMDbID = Decode(returnObject.IMDbID);
                eturnObject.Title = Decode(returnObject.Title);
                eturnObject.OriginalTitle = Decode(returnObject.OriginalTitle);
                eturnObject.Year = Decode(returnObject.Year);
                eturnObject.Rating = Decode(returnObject.Rating);
                eturnObject.Genres = Decode(returnObject.Genres);
                eturnObject.Directors = Decode(returnObject.Directors);
                eturnObject.Writers = Decode(returnObject.Writers);
                eturnObject.Producers = Decode(returnObject.Producers);
                eturnObject.Musicians = Decode(returnObject.Musicians);
                eturnObject.Cinematographers = Decode(returnObject.Cinematographers);
                eturnObject.Editors = Decode(returnObject.Editors);
                eturnObject.Cast = Decode(returnObject.Cast);
                eturnObject.Plot = Decode(returnObject.Plot);
                eturnObject.PlotKeywords = Decode(returnObject.PlotKeywords);
                eturnObject.ReleaseDate = Decode(returnObject.ReleaseDate);
                eturnObject.Runtime = Decode(returnObject.Runtime);
                eturnObject.Top250 = Decode(returnObject.Top250);
                eturnObject.Oscars = Decode(returnObject.Oscars);
                eturnObject.Awards = Decode(returnObject.Awards);
                eturnObject.Nominations = Decode(returnObject.Nominations);
                eturnObject.Tagline = Decode(returnObject.Tagline);
                eturnObject.MpaaRating = Decode(returnObject.MpaaRating);
                eturnObject.Votes = Decode(returnObject.Votes);
                eturnObject.Languages = Decode(returnObject.Languages);
                eturnObject.Countries = Decode(returnObject.Countries);
                eturnObject.Poster = Decode(returnObject.Poster);
                eturnObject.RootDir = Decode(returnObject.RootDir);
                eturnObject.DirSize = Decode(returnObject.DirSize);
                eturnObject.PosterImage = Decode(returnObject.PosterImage);
                return eturnObject;
            }

            /// <summary>
            /// Encodes Encoded Text (Encoded When Writing To DB To Fix Illegal Chars)
            /// </summary>
            /// <param name="returnObject"></param>
            /// <returns></returns>
            public imdB EncodeObj(imdB returnObject)
            {
                imdB eturnObject = new imdB();

                eturnObject.IMDbID = Encode(returnObject.IMDbID);
                eturnObject.Title = Encode(returnObject.Title);
                eturnObject.OriginalTitle = Encode(returnObject.OriginalTitle);
                eturnObject.Year = Encode(returnObject.Year);
                eturnObject.Rating = Encode(returnObject.Rating);
                eturnObject.Genres = Encode(returnObject.Genres);
                eturnObject.Directors = Encode(returnObject.Directors);
                eturnObject.Writers = Encode(returnObject.Writers);
                eturnObject.Producers = Encode(returnObject.Producers);
                eturnObject.Musicians = Encode(returnObject.Musicians);
                eturnObject.Cinematographers = Encode(returnObject.Cinematographers);
                eturnObject.Editors = Encode(returnObject.Editors);
                eturnObject.Cast = Encode(returnObject.Cast);
                eturnObject.Plot = Encode(returnObject.Plot);
                eturnObject.PlotKeywords = Encode(returnObject.PlotKeywords);
                eturnObject.ReleaseDate = Encode(returnObject.ReleaseDate);
                eturnObject.Runtime = Encode(returnObject.Runtime);
                eturnObject.Top250 = Encode(returnObject.Top250);
                eturnObject.Oscars = Encode(returnObject.Oscars);
                eturnObject.Awards = Encode(returnObject.Awards);
                eturnObject.Nominations = Encode(returnObject.Nominations);
                eturnObject.Tagline = Encode(returnObject.Tagline);
                eturnObject.MpaaRating = Encode(returnObject.MpaaRating);
                eturnObject.Votes = Encode(returnObject.Votes);
                eturnObject.Languages = Encode(returnObject.Languages);
                eturnObject.Countries = Encode(returnObject.Countries);
                eturnObject.Poster = Encode(returnObject.Poster);
                eturnObject.RootDir = Encode(returnObject.RootDir);
                eturnObject.DirSize = Encode(returnObject.DirSize);
                eturnObject.PosterImage = Encode(returnObject.PosterImage);
                return eturnObject;
            }


            public DataTable Return_DataTable_From_LITM_IMDb_New(string DbFile)
            {
                string SelectLine = "Select [IMDbID], [Title], [OriginalTitle], [Year], [Rating], [Genres], [Directors], [Writers], [Producers], [Musicians], [Cinematographers], [Editors], [Cast], [Plot], [PlotKeywords], [ReleaseDate], [Runtime], [Top250], [Oscars], [Awards], [Nominations], [Tagline], [MpaaRating], [Votes], [Languages], [Countries], [Poster], [RootDir], [DirSize], [PosterImage] From [LITM_IMDb_New]";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                return ReturnTable;
            }


            #endregion
            #region ===== imdB SQLite : Update Insert =====

            public bool imdB_Insert(string DbFile, imdB inObject)
            {
                string InsertLine = "Insert Into [IMDbLocal] (IMDbID, Title, OriginalTitle, Year, Rating, Genres, Directors, Writers, Producers, Musicians, Cinematographers, Editors, Cast, Plot, PlotKeywords, ReleaseDate, Runtime, Top250, Oscars, Awards, Nominations, Tagline, MpaaRating, Votes, Languages, Countries, Poster, RootDir, DirSize, PosterImage) values ('" + inObject.IMDbID + "', '" + inObject.Title + "', '" + inObject.OriginalTitle + "', '" + inObject.Year + "', '" + inObject.Rating + "', '" + inObject.Genres + "', '" + inObject.Directors + "', '" + inObject.Writers + "', '" + inObject.Producers + "', '" + inObject.Musicians + "', '" + inObject.Cinematographers + "', '" + inObject.Editors + "', '" + inObject.Cast + "', '" + inObject.Plot + "', '" + inObject.PlotKeywords + "', '" + inObject.ReleaseDate + "', '" + inObject.Runtime + "', '" + inObject.Top250 + "', '" + inObject.Oscars + "', '" + inObject.Awards + "', '" + inObject.Nominations + "', '" + inObject.Tagline + "', '" + inObject.MpaaRating + "', '" + inObject.Votes + "', '" + inObject.Languages + "', '" + inObject.Countries + "', '" + inObject.Poster + "', '" + inObject.RootDir + "', '" + inObject.DirSize + "', '" + inObject.PosterImage + "')";
                bool Inserted = sqlite.Execute(DbFile, InsertLine);
                if (!Inserted) { ahk.MsgBox("Inserted Into [IMDbLocal] = " + Inserted.ToString()); }
                return Inserted;
            }

            public bool imdB_Update(string DbFile, imdB inObject)
            {
                //string UpdateLine = "Update [LITM_IMDb_New] set IMDbID = '" + inObject.IMDbID + "', Title = '" + inObject.Title + "', OriginalTitle = '" + inObject.OriginalTitle + "', Year = '" + inObject.Year + "', Rating = '" + inObject.Rating + "', Genres = '" + inObject.Genres + "', Directors = '" + inObject.Directors + "', Writers = '" + inObject.Writers + "', Producers = '" + inObject.Producers + "', Musicians = '" + inObject.Musicians + "', Cinematographers = '" + inObject.Cinematographers + "', Editors = '" + inObject.Editors + "', Cast = '" + inObject.Cast + "', Plot = '" + inObject.Plot + "', PlotKeywords = '" + inObject.PlotKeywords + "', ReleaseDate = '" + inObject.ReleaseDate + "', Runtime = '" + inObject.Runtime + "', Top250 = '" + inObject.Top250 + "', Oscars = '" + inObject.Oscars + "', Awards = '" + inObject.Awards + "', Nominations = '" + inObject.Nominations + "', Tagline = '" + inObject.Tagline + "', MpaaRating = '" + inObject.MpaaRating + "', Votes = '" + inObject.Votes + "', Languages = '" + inObject.Languages + "', Countries = '" + inObject.Countries + "', Poster = '" + inObject.Poster + "', RootDir = '" + inObject.RootDir + "', DirSize = '" + inObject.DirSize + "', PosterImage = '" + inObject.PosterImage + "' WHERE [Item] = 'Value' ";
                string UpdateLine = "Update [IMDbLocal] set ";


                if (inObject.IMDbID != null) { UpdateLine = UpdateLine + "[IMDbID] = '" + inObject.IMDbID + "',"; }
                if (inObject.Title != null) { UpdateLine = UpdateLine + "[Title] = '" + inObject.Title + "',"; }
                if (inObject.OriginalTitle != null) { UpdateLine = UpdateLine + "[OriginalTitle] = '" + inObject.OriginalTitle + "',"; }
                if (inObject.Year != null) { UpdateLine = UpdateLine + "[Year] = '" + inObject.Year + "',"; }
                if (inObject.Rating != null) { UpdateLine = UpdateLine + "[Rating] = '" + inObject.Rating + "',"; }
                if (inObject.Genres != null) { UpdateLine = UpdateLine + "[Genres] = '" + inObject.Genres + "',"; }
                if (inObject.Directors != null) { UpdateLine = UpdateLine + "[Directors] = '" + inObject.Directors + "',"; }
                if (inObject.Writers != null) { UpdateLine = UpdateLine + "[Writers] = '" + inObject.Writers + "',"; }
                if (inObject.Producers != null) { UpdateLine = UpdateLine + "[Producers] = '" + inObject.Producers + "',"; }
                if (inObject.Musicians != null) { UpdateLine = UpdateLine + "[Musicians] = '" + inObject.Musicians + "',"; }
                if (inObject.Cinematographers != null) { UpdateLine = UpdateLine + "[Cinematographers] = '" + inObject.Cinematographers + "',"; }
                if (inObject.Editors != null) { UpdateLine = UpdateLine + "[Editors] = '" + inObject.Editors + "',"; }
                if (inObject.Cast != null) { UpdateLine = UpdateLine + "[Cast] = '" + inObject.Cast + "',"; }
                if (inObject.Plot != null) { UpdateLine = UpdateLine + "[Plot] = '" + inObject.Plot + "',"; }
                if (inObject.PlotKeywords != null) { UpdateLine = UpdateLine + "[PlotKeywords] = '" + inObject.PlotKeywords + "',"; }
                if (inObject.ReleaseDate != null) { UpdateLine = UpdateLine + "[ReleaseDate] = '" + inObject.ReleaseDate + "',"; }
                if (inObject.Runtime != null) { UpdateLine = UpdateLine + "[Runtime] = '" + inObject.Runtime + "',"; }
                if (inObject.Top250 != null) { UpdateLine = UpdateLine + "[Top250] = '" + inObject.Top250 + "',"; }
                if (inObject.Oscars != null) { UpdateLine = UpdateLine + "[Oscars] = '" + inObject.Oscars + "',"; }
                if (inObject.Awards != null) { UpdateLine = UpdateLine + "[Awards] = '" + inObject.Awards + "',"; }
                if (inObject.Nominations != null) { UpdateLine = UpdateLine + "[Nominations] = '" + inObject.Nominations + "',"; }
                if (inObject.Tagline != null) { UpdateLine = UpdateLine + "[Tagline] = '" + inObject.Tagline + "',"; }
                if (inObject.MpaaRating != null) { UpdateLine = UpdateLine + "[MpaaRating] = '" + inObject.MpaaRating + "',"; }
                if (inObject.Votes != null) { UpdateLine = UpdateLine + "[Votes] = '" + inObject.Votes + "',"; }
                if (inObject.Languages != null) { UpdateLine = UpdateLine + "[Languages] = '" + inObject.Languages + "',"; }
                if (inObject.Countries != null) { UpdateLine = UpdateLine + "[Countries] = '" + inObject.Countries + "',"; }
                if (inObject.Poster != null) { UpdateLine = UpdateLine + "[Poster] = '" + inObject.Poster + "',"; }
                if (inObject.RootDir != null) { UpdateLine = UpdateLine + "[RootDir] = '" + inObject.RootDir + "',"; }
                if (inObject.DirSize != null) { UpdateLine = UpdateLine + "[DirSize] = '" + inObject.DirSize + "',"; }
                //if (inObject.PosterImage != null) { UpdateLine = UpdateLine + "[PosterImage] = '" + inObject.PosterImage + "',"; }

                UpdateLine = ahk.TrimLast(UpdateLine, 1);
                UpdateLine = UpdateLine + " WHERE [IMDbID] = '" + inObject.IMDbID + "'";

                bool Updated = sqlite.Execute(DbFile, UpdateLine);
                return Updated;
            }

            public bool imdB_UpdateInsert(string DbFile, imdB obj)
            {
                obj = EncodeObj(obj);  // fix illegal charas for Db

                // create table if it doesn't exist yet
                bool Table = imdB_Table(DbFile, "IMDbLocal");

                bool Updated = imdB_Update(DbFile, obj);  // try to update record first
                if (!Updated) { Updated = imdB_Insert(DbFile, obj); }  // if unable to update, insert new record
                return Updated;
            }

            // create table if it doesn't exist yet
            public bool imdB_Table(string DbFile, string TableName = "IMDbLocal")
            {
                if (!sqlite.Table_Exists(DbFile, TableName))
                {

                    string NewTableLine = "IMDbID VARCHAR, Title VARCHAR, OriginalTitle VARCHAR, [Year] VARCHAR, Rating VARCHAR, Genres VARCHAR, Directors VARCHAR, Writers VARCHAR, Producers VARCHAR, Musicians VARCHAR, Cinematographers VARCHAR, Editors VARCHAR, [Cast] VARCHAR, Plot VARCHAR, PlotKeywords VARCHAR, ReleaseDate VARCHAR, Runtime VARCHAR, Top250 VARCHAR, Oscars VARCHAR, Awards VARCHAR, Nominations VARCHAR, Tagline VARCHAR, MpaaRating VARCHAR, Votes VARCHAR, Languages VARCHAR, Countries VARCHAR, Poster VARCHAR, RootDir VARCHAR, DirSize VARCHAR, PosterImage VARCHAR";
                    bool Table = sqlite.Table_New(DbFile, TableName, NewTableLine, false);

                    return Table;
                }

                return true;
            }

            //ahk.MsgBox(NewTableLine); 

            #endregion


            public void SQLite_New_IMDB_Table(string DBFile, string TableName)
            {
                //========================================================
                // Create New SQLite DB (*Used First-Run*)
                // ========================================================
                if (!File.Exists(DBFile)) // create database file if it doen't exist
                {
                    SQLiteConnection.CreateFile(DBFile);
                }

                //===================================
                // Connect to the DB
                //===================================

                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection dbConnection = lite._ConnectToDB(DBFile); // connect to SQLite DB file path - returns connection data


                bool ReturnValue = lite._DropTable(DBFile, TableName); // drop table from DB if it exists already

                try
                {
                    lite._sqlite("CREATE TABLE [" + TableName + "] ((Title VARCHAR, OriginalTitle VARCHAR, Year VARCHAR, Rating VARCHAR, Id VARCHAR, MpaaRating VARCHAR, ReleaseDate VARCHAR, Plot VARCHAR, Poster VARCHAR, PosterLarge VARCHAR, PosterFull VARCHAR, Runtime VARCHAR, Top250 VARCHAR, Oscars VARCHAR, Awards VARCHAR, Nominations VARCHAR, Storyline VARCHAR, Tagline VARCHAR, Votes VARCHAR, ImdbURL VARCHAR)", dbConnection);  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


            }


            /// <summary>
            /// Lookup OMDb Info By Title ID, Write Entry to IMDb.sqlite
            /// </summary>
            /// <param name="IMDbID">IMDb TitleID to Lookup</param>
            public bool IMDb_SQLite(string IMDbID)
            {
                _Sites.OMDB omdb = new OMDB();
                
                // read imdb id from url, lookup movie info from OMDb API
                imdB Movie = omdb.OMdb(IMDbID);

                // Option to Write Write OMDb Movie Data to Local SQLite Db
                bool added = imdB_UpdateInsert(ahk.AppDir() + "\\IMDb.sqlite", Movie);

                return added;
            }


            #endregion


            /// <summary>
            /// Looks up Movie Info by IMDbID, Creates Movie Dir + Movie URL
            /// </summary>
            /// <param name="ImdbID">IMDb Title ID</param>
            /// <param name="SaveDirRoot">Root Dir to Create New Dir with URL</param> 
            /// <returns>Returns Path to New IMDb URL Dir</returns>
            public string IMDb_UrlDir(string ImdbID, string SaveDirRoot)
            {
                imdB movieInfo = parse_IMDb_Page(ImdbID, "", true, false);

                movieInfo = FixIMDbTitle(movieInfo);

                string newDirName = IMDb_NewDir_NoLookup(movieInfo, SaveDirRoot);

                return newDirName;
            }


        }


    }
}

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
            public class IAFD
            {
                _AHK ahk = new _AHK();
                _Database.SQL sql = new _Database.SQL();
                _Lists lst = new _Lists();
                _Parse prs = new _Parse();



                public void Start4MovieThreads(RadProgressBar bar1, RadProgressBar bar2, RadProgressBar bar3, RadProgressBar bar4)
                {
                    List<string> urls = Return_Flagged_MovieURLs();

                    // divide with no remainder, then add remainder value to last list 
                    int remainder = urls.Count % 4;
                    int totalWithOutRemainder = urls.Count - remainder;
                    int quarter = totalWithOutRemainder / 4;

                    List<string> urls1 = new List<string>();
                    List<string> urls2 = new List<string>();
                    List<string> urls3 = new List<string>();
                    List<string> urls4 = new List<string>();

                    int counter = 0;
                    foreach (string url in urls)
                    {
                        if (counter < quarter) { urls1.Add(url); counter++; continue; }
                        if (counter < (quarter * 2)) { urls2.Add(url); counter++; continue; }
                        if (counter < (quarter * 3)) { urls3.Add(url); counter++; continue; }
                        if (counter < (quarter * 4 + remainder)) { urls4.Add(url); counter++; continue; }
                    }

                    PopulateMissingMovieInfoThreads(urls1, 1, null, bar1, true);
                    PopulateMissingMovieInfoThreads(urls2, 2, null, bar2, true);
                    PopulateMissingMovieInfoThreads(urls3, 3, null, bar3, true);
                    PopulateMissingMovieInfoThreads(urls4, 4, null, bar4, true);
                }



                public void PopulateMissingMovieInfoThreads(List<string> urls, int ThreadNum = 1, RadTextBox TextBox = null, RadProgressBar Progress = null, bool NewThread = true)
                {
                    if (NewThread)
                    {
                        Thread newThread = new Thread(() => PopulateMissingMovieInfoThreads(urls, ThreadNum, TextBox, Progress, false)); // Function To Execute
                        newThread.Name = "Populate Movie Info " + ThreadNum;
                        newThread.IsBackground = true;
                        newThread.Start();
                    }
                    else
                    {
                        _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();
                        _TelerikLib tel = new _TelerikLib();

                        if (Progress != null) { pro.SetupProgressBar(Progress, urls.Count); }

                        int i = 0; Stopwatch stopwatch = new Stopwatch();
                        foreach (string url in urls)
                        {
                            stopwatch.Reset(); stopwatch.Start();
                            PopulateMovieInfo(url, false);
                            stopwatch.Stop();

                            i++;
                            if (Progress != null) { pro.UpdateProgress(Progress, "Thread" + ThreadNum + " | " + ProgressRate(stopwatch.Elapsed, i, urls.Count)); }
                            if (TextBox != null) { tel.Update(TextBox, url); }
                        }
                    }



                }


                #region === Parse Actions ===

                public bool PopulateMovieInfo(string URL, bool NewThread = true)
                {
                    if (NewThread)
                    {
                        Thread newThread = new Thread(() => PopulateMovieInfo(URL, false)); // Function To Execute
                        newThread.Name = "Populate Movie Info";
                        newThread.IsBackground = true;
                        newThread.Start();
                    }
                    else
                    {
                        IAFD.FilmParse film = new IAFD.FilmParse();
                        film = ParseFilmPage(URL);
                        bool wrote = WriteFilm(film);
                        return wrote;
                    }

                    return false;
                }

                public bool PopulateActorInfo(string URL, bool NewThread = true)
                {
                    if (NewThread)
                    {
                        Thread newThread = new Thread(() => PopulateActorInfo(URL, false)); // Function To Execute
                        newThread.Name = "Populate Actor Info";
                        newThread.IsBackground = true;
                        newThread.Start();
                    }
                    else
                    {
                        IAFD.Actor actor = ParseActorPage(URL);
                        bool wrote = WriteActor(actor);
                        return wrote;
                    }

                    return false;
                }

                public void PopulateMissingMovieInfo(RadTextBox TextBox = null, RadProgressBar Progress = null, bool NewThread = true)
                {
                    if (NewThread)
                    {
                        Thread newThread = new Thread(() => PopulateMissingMovieInfo(TextBox, Progress, false)); // Function To Execute
                        newThread.Name = "Populate Missing Movie Info";
                        newThread.IsBackground = true;
                        newThread.Start();
                    }
                    else
                    {
                        _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();
                        _TelerikLib tel = new _TelerikLib();
                        List<string> urls = Return_Flagged_MovieURLs();

                        if (Progress != null) { pro.SetupProgressBar(Progress, urls.Count); }

                        int i = 0; Stopwatch stopwatch = new Stopwatch();
                        foreach (string url in urls)
                        {
                            stopwatch.Reset(); stopwatch.Start();
                            PopulateMovieInfo(url, false);
                            stopwatch.Stop();

                            i++;
                            if (Progress != null) { pro.UpdateProgress(Progress, ProgressRate(stopwatch.Elapsed, i, urls.Count)); }
                            if (TextBox != null) { tel.Update(TextBox, url); }
                        }

                    }
                }
                public void PopulateMissingActorInfo(RadTextBox TextBox = null, RadProgressBar Progress = null, bool NewThread = true)
                {
                    if (NewThread)
                    {
                        Thread newThread = new Thread(() => PopulateMissingActorInfo(TextBox, Progress, false)); // Function To Execute
                        newThread.Name = "Populate Missing Actor Info";
                        newThread.IsBackground = true;
                        newThread.Start();
                    }
                    else
                    {
                        _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();
                        _TelerikLib tel = new _TelerikLib();
                        List<string> urls = Return_Flagged_ActorURLs();

                        if (Progress != null) { pro.SetupProgressBar(Progress, urls.Count); }

                        int i = 0; Stopwatch stopwatch = new Stopwatch();
                        foreach (string url in urls)
                        {
                            stopwatch.Reset(); stopwatch.Start();
                            PopulateActorInfo(url, false);
                            stopwatch.Stop();

                            i++;
                            if (Progress != null) { pro.UpdateProgress(Progress, ProgressRate(stopwatch.Elapsed, i, urls.Count)); }
                            if (TextBox != null) { tel.Update(TextBox, url); }
                        }
                    }
                }

                public string ProgressRate(TimeSpan ActionTime, int Current, int Total, bool ShowPerMin = true, bool ShowPerHour = true)
                {

                    double ms = ActionTime.TotalMilliseconds;
                    ms = ms / 1000;
                    int PerUnit = 60;
                    double rate = (double)PerUnit / (double)ms;

                    // actions per min / hour
                    int PerMin = Convert.ToDouble(rate.ToString("N0")).ToInt();
                    int PerHour = PerMin * 60;
                    if (PerMin == 0) { PerMin = 1; }

                    // time remaining
                    int ActionsRemaining = Total - Current;  // # of items remaining
                    int RemainingMin = ActionsRemaining / PerMin;


                    string Rate = "";
                    //if (ShowPerMin) { Rate = PerMin.ToString() + "/Min"; }
                    //if (ShowPerHour)
                    //{
                    //    if (Rate == "") { Rate = PerHour.ToString() + "/Hour"; }
                    //    else { Rate = Rate + " | " + PerHour.ToString() + "/Hour"; }
                    //}

                    Rate = Current + "/" + Total + " | " + PerMin.ToString() + "/Min" + " | " + PerHour.ToString() + "/Hour" + " | " + ActionsRemaining + " Remaining | " + RemainingMin + " MinToGo";


                    return Rate;
                }


                public FilmParse ParseFilmPage(string URL = "", string HTML = "")
                {
                    FilmParse film = new FilmParse();

                    if (URL != "")
                    {
                        HTML = ahk.Download_HTML(URL);
                        film.FilmURL = URL;
                    }

                    List<string> Segs = new List<string>();
                    List<string> lines = lst.Text_To_List(HTML, true, true, false);
                    foreach (string line in lines)
                    {
                        if (line.Contains("<div class=\"castbox"))
                        {
                            string newLine = line.Replace("<div class=", "\n<div class=");
                            newLine = newLine.Replace("<p>", "\n");
                            newLine = newLine.Replace("<img", "\n<img");
                            newLine = newLine.Replace("<br>&nbsp;<br>&nbsp;<br></div>", "");
                            newLine = newLine.Replace("<img class=\"headshot\" src=\"", "");
                            newLine = newLine.Replace("</a><br> ", "|");
                            newLine = newLine.Replace(".jpg\"><br>", ".jpg|");
                            newLine = newLine.Replace(".png\"><br>", ".png|");
                            newLine = newLine.Replace("<a href=\"/person", "/person");
                            newLine = newLine.Replace(".htm\">", ".htm");
                            newLine = newLine.Replace("</a><br></div></div></div></div>", "\n");
                            newLine = newLine.Replace("</a><br>", "");
                            newLine = newLine.Replace("<div class=\"padded-panel\">", "");
                            newLine = newLine.Replace("<div class=\"row\">", "");
                            newLine = newLine.Replace("<div class=\"col-sm-12\">", "");
                            newLine = newLine.Replace("</a><br>", "");
                            newLine = newLine.Replace("</a><br>", "");
                            newLine = newLine.Replace("</a><br>", "");
                            newLine = newLine.Replace("<img class=\"headshot\" src=\"", "");

                            Segs.Add(newLine);
                        }

                        if (line.Contains("<h1>"))
                        {
                            string title = line.Replace("<h1>", "");
                            title = title.Replace("</h1>", "");
                            film.FilmTitle = title;

                            if (title.Contains("("))
                            {
                                string yr = ahk.StringSplit(title, "(", 1);
                                yr = yr.Replace(")", "").Trim();
                                film.Year = yr;
                            }
                        }

                        if (line.Contains("<dd>"))
                        {
                            string title = line.Replace("<dd>", "");
                            title = title.Replace("</dd>", "");
                            title = title.Replace("<dl><b>Also Known As</b>", "");
                            title = title.Replace("</dl>", "");
                            film.AltTitle = title;
                        }
                    }

                    if (Segs.Count > 0)
                    {
                        List<string> Lines = lst.Text_To_List(Segs[0], true, true, false);

                        CreditParse person = new CreditParse();
                        List<CreditParse> credits = new List<CreditParse>();
                        foreach (string Line in Lines)
                        {
                            if (Line.Contains(".htm"))
                            {
                                person = new CreditParse();
                                person.NameLink = "http://www.iafd.com" + Line;
                            }
                            if (Line.Contains(".jpg") || Line.Contains(".png"))
                            {
                                List<string> split = ahk.StringSplit_List(Line, "|"); int i = 0;
                                foreach (string se in split)
                                {
                                    if (i == 0)
                                    {
                                        string il = se;
                                        il = il.Replace("<img class=\"headshot\" src=\"", "");
                                        person.ImageLink = il;

                                        if (il.Contains("src="))
                                        {
                                            il = il.Replace("src=", "|");
                                            person.ImageLink = ahk.StringSplit(il, "|", 1);
                                        }

                                        person.ImageLink = person.ImageLink.Replace("\"", "");
                                        person.ImageLink = person.ImageLink.Replace("\"", "");
                                    }
                                    if (i == 1)
                                    {
                                        string nm = se;
                                        if (nm.Contains("<br>"))
                                        {
                                            nm = nm.Replace("<br>", "|");

                                            if (nm.Contains("<i>"))
                                            {
                                                string nameTwo = ahk.StringSplit(nm, "|", 0);
                                                nameTwo = nameTwo.Replace("<i>", "|");
                                                person.Name = ahk.StringSplit(nameTwo, "|", 0);
                                            }
                                            else
                                            {
                                                person.Name = ahk.StringSplit(nm, "|", 0);
                                            }

                                            person.NameNote = ahk.StringSplit(nm, "|", 1);
                                            person.NameNote = person.NameNote.Replace("</div>", "");
                                            person.NameNote = person.NameNote.Trim();
                                        }
                                        else
                                        {
                                            person.Name = se;
                                        }
                                    }
                                    if (i == 2)
                                    {
                                        if (person.NameNote == null)
                                        {
                                            person.NameNote = se;
                                        }
                                    }
                                    i++;
                                }
                                if (person.NameNote == null) { person.NameNote = ""; }
                                credits.Add(person);
                            }
                        }

                        film.Actors = credits;
                    }


                    int p = 0; string Min = ""; string Director = ""; string Distributor = ""; string AllGirl = ""; string AllMale = ""; string Compilation = ""; string ReleaseDate = "";
                    foreach (string line in lines)
                    {
                        if (line.Contains("<p class=\"bioheading\">Minutes</p>"))
                        {
                            Min = lines[p];
                            Min = Min.Replace("<p class=\"bioheading\">Minutes</p>", "");
                            Min = Min.Replace("<p class=\"biodata\">", "");
                            Min = Min.Replace("</p>", "");
                        }
                        if (line.Contains("<p class=\"bioheading\">All-Girl</p>"))
                        {
                            AllGirl = lines[p];
                            AllGirl = AllGirl.Replace("<p class=\"bioheading\">All-Girl</p>", "");
                            AllGirl = AllGirl.Replace("<p class=\"biodata\">", "");
                            AllGirl = AllGirl.Replace("</p>", "");
                            if (!AllGirl.Contains("No")) { film.AllGirl = true; } else { film.AllGirl = false; }
                        }
                        if (line.Contains("<p class=\"bioheading\">All-Male</p>"))
                        {
                            AllMale = lines[p];
                            AllMale = AllMale.Replace("<p class=\"bioheading\">All-Male</p>", "");
                            AllMale = AllMale.Replace("<p class=\"biodata\">", "");
                            AllMale = AllMale.Replace("</p>", "");
                            if (!AllMale.Contains("No")) { film.AllMale = true; } else { film.AllMale = false; }
                        }
                        if (line.Contains("<p class=\"bioheading\">Compilation</p>"))
                        {
                            Compilation = lines[p];
                            Compilation = Compilation.Replace("<p class=\"bioheading\">Compilation</p>", "");
                            Compilation = Compilation.Replace("<p class=\"biodata\">", "");
                            Compilation = Compilation.Replace("</p>", "");
                            if (!Compilation.Contains("No")) { film.Compilation = true; } else { film.Compilation = false; }
                        }
                        if (line.Contains("<p class=\"bioheading\">Release Date</p>"))
                        {
                            ReleaseDate = lines[p];
                            ReleaseDate = ReleaseDate.Replace("<p class=\"bioheading\">Release Date</p>", "");
                            ReleaseDate = ReleaseDate.Replace("<p class=\"biodata\">", "");
                            ReleaseDate = ReleaseDate.Replace("</p>", "");
                            film.ReleaseDate = ReleaseDate;
                        }
                        if (line.Contains("<p class=\"bioheading\">Distributor</p>"))
                        {
                            Distributor = lines[p];
                            Distributor = Distributor.Replace("<p class=\"bioheading\">Distributor</p>", "");
                            Distributor = Distributor.Replace("<p class=\"biodata\">", "");
                            Distributor = Distributor.Replace("<a href=\"", "");
                            Distributor = Distributor.Replace(".htm\">", ".htm|");
                            Distributor = Distributor.Replace("</a>", "");
                            Distributor = Distributor.Replace("</p>", "");

                            DistributorParse dist = new DistributorParse();
                            dist.Name = ahk.StringSplit(Distributor, "|", 1).Trim();

                            string dirLink = ahk.StringSplit(Distributor, "|", 0).Trim();
                            if (!dirLink.Contains("iafd.com")) { dirLink = "http://www.iafd.com" + dirLink; }
                            dist.NameLink = dirLink;

                            film.Distributor = dist;
                        }
                        if (line.Contains("<p class=\"bioheading\">Directors</p>"))
                        {
                            Director = lines[p];
                            Director = Director.Replace("<p class=\"bioheading\">Directors</p>", "");
                            Director = Director.Replace("<p class=\"biodata\">", "");
                            Director = Director.Replace("<a href=\"", "");
                            Director = Director.Replace(".htm\">", ".htm|");
                            Director = Director.Replace("</a>", "");
                            Director = Director.Replace("</p>", "");

                            DirectorParse dir = new DirectorParse();
                            dir.Name = ahk.StringSplit(Director, "|", 1).Trim();

                            string dirLink = ahk.StringSplit(Director, "|", 0).Trim();
                            if (!dirLink.Contains("iafd.com")) { dirLink = "http://www.iafd.com" + dirLink; }

                            dir.NameLink = dirLink;
                            film.Director = dir;
                        }

                        p++;
                    }

                    film.Minutes = Min;
                    return film;
                }

                public Actor ParseActorPage(string URL = "", string HTML = "")
                {
                    Actor actor = new Actor();
                    actor.URL = URL;
                    if (HTML == "") { HTML = ahk.Download_HTML(URL); }

                    HTML = HTML.Replace("<p class=\"bioheading\">", "\n<p class=\"bioheading\">");
                    HTML = HTML.Replace("<div class=\"text-left\"", "\n<div class=\"text-left\"");

                    //ahk.ClipWrite(HTML);

                    List<ActorFilms> ActorsFilms = new List<ActorFilms>();

                    List<string> Segs = new List<string>(); bool performerCap = false; string performerCAP = "";
                    List<string> lines = lst.Text_To_List(HTML, true, true, false); int p = 0;
                    foreach (string line in lines)
                    {

                        // extract paths for links to title actor played in
                        if (line.Contains("href=\"/title.rme"))
                        {
                            List<string> parts = ahk.StringSplit_List(line, "\"");
                            foreach (string part in parts)
                            {
                                if (part.Contains("title.rme"))
                                {
                                    ActorFilms film = new ActorFilms();
                                    film.FilmLink = "http://www.iafd.com" + part;
                                    ActorsFilms.Add(film);
                                }
                            }
                        }
                        actor.Films = ActorsFilms;

                        if (line.Contains("<h1>"))
                        {
                            string name = line.Replace("<h1>", "");
                            name = name.Replace("</h1>", "");
                            actor.Name = name;
                        }

                        if (line.Contains("<p class=\"bioheading\">Ethnicity</p>"))
                        {
                            string Min = lines[p];
                            Min = Min.Replace("<p class=\"bioheading\">Ethnicity</p>", "");
                            Min = Min.Replace("<p class=\"biodata\">", "");
                            Min = Min.Replace("</p>", "");
                            Min = Min.Replace("&nbsp;", "");
                            actor.Ethnicity = Min;
                        }
                        if (line.Contains("<p class=\"bioheading\">Nationality</p>"))
                        {
                            string Min = lines[p];
                            Min = Min.Replace("<p class=\"bioheading\">Nationality</p>", "");
                            Min = Min.Replace("<p class=\"biodata\">", "");
                            Min = Min.Replace("</p>", "");
                            Min = Min.Replace("&nbsp;", "");
                            actor.Nationality = Min;
                        }
                        if (line.Contains("<p class=\"bioheading\">Hair Color</p>"))
                        {
                            string Min = lines[p];
                            Min = Min.Replace("<p class=\"bioheading\">Hair Color</p>", "");
                            Min = Min.Replace("<p class=\"biodata\">", "");
                            Min = Min.Replace("</p>", "");
                            Min = Min.Replace("&nbsp;", "");
                            actor.HairColor = Min;
                        }
                        if (line.Contains("<p class=\"bioheading\">Height</p>"))
                        {
                            string Min = lines[p];
                            Min = Min.Replace("<p class=\"bioheading\">Height</p>", "");
                            Min = Min.Replace("<p class=\"biodata\">", "");
                            Min = Min.Replace("</p>", "");
                            Min = Min.Replace("&nbsp;", "");
                            actor.Height = Min;
                        }
                        if (line.Contains("<p class=\"bioheading\">Weight</p>"))
                        {
                            string Min = lines[p];
                            Min = Min.Replace("<p class=\"bioheading\">Weight</p>", "");
                            Min = Min.Replace("<p class=\"biodata\">", "");
                            Min = Min.Replace("</p>", "");
                            Min = Min.Replace("&nbsp;", "");
                            actor.Weight = Min;
                        }
                        if (line.Contains("<p class=\"bioheading\">Measurements</p>"))
                        {
                            string Min = lines[p];
                            Min = Min.Replace("<p class=\"bioheading\">Measurements</p>", "");
                            Min = Min.Replace("<p class=\"biodata\">", "");
                            Min = Min.Replace("</p>", "");
                            Min = Min.Replace("&nbsp;", "");
                            actor.Measurements = Min;
                        }
                        if (line.Contains("<p class=\"bioheading\">Tattoos</p>"))
                        {
                            string Min = lines[p];
                            Min = Min.Replace("<p class=\"bioheading\">Tattoos</p>", "");
                            Min = Min.Replace("<p class=\"biodata\">", "");
                            Min = Min.Replace("</p>", "");
                            Min = Min.Replace("&nbsp;", "");
                            actor.Tattoos = Min;
                        }
                        if (line.Contains("<p class=\"bioheading\">Piercings</p>"))
                        {
                            string Min = lines[p];
                            Min = Min.Replace("<p class=\"bioheading\">Piercings</p>", "");
                            Min = Min.Replace("<p class=\"biodata\">", "");
                            Min = Min.Replace("</p>", "");
                            Min = Min.Replace("&nbsp;", "");
                            actor.Piercings = Min;
                        }
                        if (line.Trim() == "Performer")
                        {
                            performerCap = true;
                        }
                        if (performerCap)
                        {
                            if (performerCAP == "") { performerCAP = line; }
                            else
                            {
                                performerCAP = performerCAP + "|" + line;

                                performerCAP = performerCAP.Replace("</p><div class=\"biodata\">", "|");

                                if (line.Contains("</div>"))
                                {
                                    performerCAP = performerCAP.Replace("</div>", "");

                                    string names = "";
                                    if (performerCAP.Contains("|"))
                                    {
                                        names = ahk.StringSplit(performerCAP, "|", 2);
                                        names = names.Replace(",", "|");
                                        names = names.Replace(" |", "|");
                                        names = names.Replace("| ", "|");
                                    }

                                    performerCap = false; actor.PerformerAKA = names;
                                }
                            }
                        }
                        if (line.Contains("<p class=\"bioheading\">Birthday</p>"))
                        {
                            string Min = lines[p];
                            Min = Min.Replace("<p class=\"bioheading\">Birthday</p>", "");
                            Min = Min.Replace("<p class=\"biodata\">", "");
                            Min = Min.Replace("</p>", "");
                            Min = Min.Replace("&nbsp;", "");

                            if (Min.Contains(">"))
                            {
                                string part = ahk.StringSplit(Min, ">", 1);
                                string par = ahk.StringSplit(part, "<", 0);
                                Min = par;
                            }

                            actor.Birthday = Min;
                        }
                        if (line.Contains("<p class=\"bioheading\">Astrology</p>"))
                        {
                            string Min = lines[p];
                            Min = Min.Replace("<p class=\"bioheading\">Astrology</p>", "");
                            Min = Min.Replace("<p class=\"biodata\">", "");
                            Min = Min.Replace("</p>", "");
                            Min = Min.Replace("&nbsp;", "");

                            if (Min.Contains(">"))
                            {
                                string part = ahk.StringSplit(Min, ">", 1);
                                string par = ahk.StringSplit(part, "<", 0);
                                Min = par;
                            }

                            actor.Astrology = Min;
                        }
                        if (line.Contains("<p class=\"bioheading\">Birthplace</p>"))
                        {
                            string Min = lines[p];
                            Min = Min.Replace("<p class=\"bioheading\">Birthplace</p>", "");
                            Min = Min.Replace("<p class=\"biodata\">", "");
                            Min = Min.Replace("</p>", "");
                            Min = Min.Replace("&nbsp;", "");
                            actor.BirthPlace = Min;
                        }
                        if (line.Contains("<p class=\"bioheading\">Years Active</p>"))
                        {
                            string Min = lines[p];
                            Min = Min.Replace("<p class=\"bioheading\">Years Active</p>", "");
                            Min = Min.Replace("<p class=\"biodata\">", "");
                            Min = Min.Replace("</p>", "");
                            actor.YearsActive = Min;
                        }
                        if (line.Contains("<p class=\"bioheading\">Years Active as Performer </p>"))
                        {
                            string Min = lines[p];
                            Min = Min.Replace("<p class=\"bioheading\">Years Active as Performer </p>", "");
                            Min = Min.Replace("<p class=\"biodata\">", "");
                            Min = Min.Replace("</p>", "");
                            Min = Min.Replace("&nbsp;", "");
                            actor.YearsActiveAsPerformer = Min;
                        }
                        if (line.Contains("<p class=\"bioheading\">Year Active as Director</p>"))
                        {
                            string Min = lines[p];
                            Min = Min.Replace("<p class=\"bioheading\">Year Active as Director</p>", "");
                            Min = Min.Replace("<p class=\"biodata\">", "");
                            Min = Min.Replace("</p>", "");
                            Min = Min.Replace("&nbsp;", "");
                            actor.YearsActiveAsDirector = Min;
                        }
                        if (line.Contains("<div id=\"headshot\"><img"))
                        {
                            string Min = lines[p];

                            List<string> segs = ahk.StringSplit_List(Min, "\"");
                            foreach (string seg in segs)
                            {
                                if (seg.Contains(".jpg") || seg.Contains(".png"))
                                {
                                    actor.HeadShotURL = seg;
                                }
                            }
                        }


                        p++;
                    }


                    return actor;
                }


                public bool WriteActor(Actor actor)
                {
                    IAFD_Actors act = new IAFD_Actors();
                    if (actor.Astrology != null) { act.Astrology = actor.Astrology.Replace("No data", ""); }

                    string aka = actor.PerformerAKA;
                    if (aka != "" && actor.DirectorAKA != "") { aka = actor.PerformerAKA + "|" + actor.DirectorAKA; }
                    if (aka == "" && actor.DirectorAKA != "") { aka = actor.DirectorAKA; }
                    if (aka != null) { act.AKA = aka.Replace("No data", ""); }


                    if (actor.Birthday != null) { act.Birthday = actor.Birthday.Replace("No data", ""); }
                    if (actor.BirthPlace != null) { act.BirthPlace = actor.BirthPlace.Replace("No data", ""); }
                    if (actor.Ethnicity != null) { act.Ethnicity = actor.Ethnicity.Replace("No data", ""); }

                    string films = "";
                    if (actor.Films != null)
                    {
                        foreach (ActorFilms film in actor.Films)
                        {
                            if (films == "") { films = film.FilmLink; }
                            else { films = films + "\n" + film.FilmLink; }

                            bool populated = IAFD_Movies_FilmURLOnly_UpdateInsert(film.FilmLink);
                            //if (!populated) { WriteFilmNoInfo(film.FilmLink); }
                        }
                    }

                    act.Films = films;

                    act.Flag = false;


                    if (actor.HairColor != null) { act.HairColor = actor.HairColor.Replace("No data", ""); }
                    if (actor.Height != null) { act.Height = actor.Height.Replace("No data", ""); }
                    if (actor.Measurements != null) { act.Measurements = actor.Measurements.Replace("No data", ""); }
                    if (actor.Name != null) { act.Name = actor.Name; }
                    if (actor.Nationality != null) { act.Nationality = actor.Nationality.Replace("No data", ""); }
                    if (actor.Piercings != null) { act.Piercings = actor.Piercings.Replace("No data", ""); }
                    if (actor.Tattoos != null) { act.Tattoos = actor.Tattoos.Replace("No data", ""); }

                    if (actor.Tattoos != null)
                    {
                        if (actor.Tattoos == "") { act.Tattoos = "False"; }
                        else if (actor.Tattoos == "None") { act.Tattoos = "False"; }
                        else { act.Tattoos = "True"; }
                    }

                    act.URL = actor.URL;
                    if (actor.Weight != null) { act.Weight = actor.Weight.Replace("No data", ""); }
                    if (actor.YearsActive != null) { act.YearsActive = actor.YearsActive.Replace("No data", ""); }
                    if (actor.HeadShotURL != null) { act.HeadShotURL = actor.HeadShotURL; }

                    return IAFD_Actors_UpdateInsert(act);
                }

                public bool WriteActorNoInfo(string URL)
                {
                    IAFD_Actors act = new IAFD_Actors();
                    act.Astrology = "";
                    act.AKA = "";
                    act.Birthday = "";
                    act.BirthPlace = "";
                    act.Ethnicity = "";
                    act.Films = "";
                    act.Flag = true;
                    act.HairColor = "";
                    act.Height = "";
                    act.Measurements = "";
                    act.Name = "";
                    act.Nationality = "";
                    act.Piercings = "";
                    act.Tattoos = "";
                    act.URL = URL;
                    act.Weight = "";
                    act.YearsActive = "";
                    act.HeadShotURL = "";
                    act.HeadShotLocalImg = "";
                    act.HeadShotURL = "";
                    return IAFD_Actors_UpdateInsert(act);
                }


                public bool WriteFilm(FilmParse Film)
                {
                    IAFD_Movies film = new IAFD_Movies();

                    string actors = "";
                    if (Film.Actors != null)
                    {
                        foreach (CreditParse actor in Film.Actors)
                        {
                            if (actors == "") { actors = actor.Name + "|" + actor.NameLink + "|" + actor.NameNote + "|" + actor.ImageLink; }
                            else { actors = actors + "\n" + actor.Name + "|" + actor.NameLink + "|" + actor.NameNote + "|" + actor.ImageLink; }

                            // for each actor in film, write the actor's url with flag indicating the actor info needs to be populated
                            bool ActorInfoInDb = ActorInfoPopulated(actor.NameLink);
                            if (!ActorInfoInDb) { WriteActorNoInfo(actor.NameLink); }
                        }
                    }

                    film.Actors = actors;
                    film.Flag = false;
                    film.AllGirl = Film.AllGirl.ToString();
                    film.AllMale = Film.AllMale.ToString();
                    film.AltTitle = Film.AltTitle;
                    film.Compilation = Film.Compilation.ToString();
                    film.Director = Film.Director.Name + "|" + Film.Director.NameLink;
                    film.Minutes = Film.Minutes;
                    film.ReleaseDate = Film.ReleaseDate;
                    film.Studio = Film.Distributor.Name + Film.Distributor.NameLink;
                    film.Title = Film.FilmTitle;
                    film.URL = Film.FilmURL;
                    film.Year = Film.Year;

                    film.FilePath = "";
                    film.FileSize = "";
                    film.Quality = "";
                    film.Rating = "";

                    return IAFD_Movies_UpdateInsert(film);
                }

                public bool WriteFilmNoInfo(string URL)
                {
                    IAFD_Movies film = new IAFD_Movies();
                    film.Actors = "";
                    film.Flag = true;
                    film.AllGirl = "False";
                    film.AllMale = "False";
                    film.AltTitle = "";
                    film.Compilation = "False";
                    film.Director = "";
                    film.Minutes = "";
                    film.ReleaseDate = "";
                    film.Studio = "";
                    film.Title = "";
                    film.URL = URL;
                    film.Year = "";
                    film.FilePath = "";
                    film.FileSize = "";
                    film.Quality = "";
                    film.Rating = "";

                    return IAFD_Movies_UpdateInsert(film);
                }

                #endregion



                #region === Parse Objects ===

                // Parse Film Pages

                public struct FilmParse
                {
                    public string FilmTitle { get; set; }
                    public string Year { get; set; }
                    public string AltTitle { get; set; }
                    public string FilmURL { get; set; }
                    public string Minutes { get; set; }
                    public string ReleaseDate { get; set; }
                    public bool AllGirl { get; set; }
                    public bool AllMale { get; set; }
                    public bool Compilation { get; set; }
                    public bool Flag { get; set; }
                    public DistributorParse Distributor { get; set; }
                    public DirectorParse Director { get; set; }
                    public List<CreditParse> Actors { get; set; }

                }
                public struct CreditParse
                {
                    public string Name { get; set; }
                    public string NameLink { get; set; }
                    public string ImageLink { get; set; }
                    public string NameNote { get; set; }
                }
                public struct DirectorParse
                {
                    public string Name { get; set; }
                    public string NameLink { get; set; }
                }
                public struct DistributorParse
                {
                    public string Name { get; set; }
                    public string NameLink { get; set; }
                }


                // Parse Actor Pages

                public struct Actor
                {
                    public string URL { get; set; }
                    public bool Flag { get; set; }
                    public string Name { get; set; }
                    public string Ethnicity { get; set; }
                    public string Nationality { get; set; }
                    public string HairColor { get; set; }
                    public string Height { get; set; }
                    public string Weight { get; set; }
                    public string Measurements { get; set; }
                    public string Tattoos { get; set; }
                    public string Piercings { get; set; }
                    public List<ActorFilms> Films { get; set; }
                    public string PerformerAKA { get; set; }
                    public string DirectorAKA { get; set; }
                    public string Birthday { get; set; }
                    public string Astrology { get; set; }
                    public string BirthPlace { get; set; }
                    public string YearsActive { get; set; }
                    public string YearsActiveAsPerformer { get; set; }
                    public string YearsActiveAsDirector { get; set; }
                    public string HeadShotURL { get; set; }

                    public string HeadShotLocalImg { get; set; }
                }

                public struct ActorFilms
                {
                    public string FilmTitle { get; set; }
                    public string FilmLink { get; set; }
                    public string StudioName { get; set; }
                    public string StudioLink { get; set; }
                    public string Year { get; set; }
                    public string Notes { get; set; }

                }


                #endregion


                public int IAFD_FlagFix()
                {
                    SqlConnection Conn = IAFD_Movies_Conn();
                    string SQLLine = "Update " + IAFD_Movies_TableName() + "  set [Flag] = '1' where Title = '' and Flag = '0'";
                    SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                    cmd2 = new SqlCommand(SQLLine, Conn);

                    if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                    int recordsAffected = 0;
                    try { recordsAffected = cmd2.ExecuteNonQuery(); }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 1205) // retry on deadlock
                        {
                            ahk.Sleep(1000);
                            IAFD_FlagFix();
                        }
                        else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                        {
                            ahk.Sleep(1000);
                            IAFD_FlagFix();
                        }
                        else
                        {
                            ahk.MsgBox(ex.ToString());
                            return 0;
                        }
                    }
                    Conn.Close();
                    return recordsAffected;
                }



                #region === IAFD_Actors FUNCTIONS ===

                #region ===== IAFD_Actors Object =====

                public struct IAFD_Actors
                {
                    public string ID { get; set; }
                    public bool Flag { get; set; }
                    public string Name { get; set; }
                    public string AKA { get; set; }
                    public string Ethnicity { get; set; }
                    public string Nationality { get; set; }
                    public string HairColor { get; set; }
                    public string Height { get; set; }
                    public string Weight { get; set; }
                    public string Measurements { get; set; }
                    public string Tattoos { get; set; }
                    public string Piercings { get; set; }
                    public string Films { get; set; }
                    public string Birthday { get; set; }
                    public string Astrology { get; set; }
                    public string BirthPlace { get; set; }
                    public string YearsActive { get; set; }
                    public string URL { get; set; }
                    public string HeadShotURL { get; set; }
                    public string HeadShotLocalImg { get; set; }
                    public string Rating { get; set; }
                }
                //  Fix illegal characters before Sql/Sqlite Db Inserts
                public IAFD_Actors IAFD_Actors_FixChars(IAFD_Actors ToFix)
                {
                    IAFD_Actors Fixed = new IAFD_Actors();

                    Fixed.ID = ToFix.ID.Replace("'", "''");
                    Fixed.Name = ToFix.Name.Replace("'", "''");
                    Fixed.AKA = ToFix.AKA.Replace("'", "''");
                    Fixed.Ethnicity = ToFix.Ethnicity.Replace("'", "''");
                    Fixed.Nationality = ToFix.Nationality.Replace("'", "''");
                    Fixed.HairColor = ToFix.HairColor.Replace("'", "''");
                    Fixed.Height = ToFix.Height.Replace("'", "''");
                    Fixed.Weight = ToFix.Weight.Replace("'", "''");
                    Fixed.Measurements = ToFix.Measurements.Replace("'", "''");
                    Fixed.Tattoos = ToFix.Tattoos.Replace("'", "''");
                    Fixed.Piercings = ToFix.Piercings.Replace("'", "''");
                    Fixed.Films = ToFix.Films.Replace("'", "''");
                    Fixed.Birthday = ToFix.Birthday.Replace("'", "''");
                    Fixed.Astrology = ToFix.Astrology.Replace("'", "''");
                    Fixed.BirthPlace = ToFix.BirthPlace.Replace("'", "''");
                    Fixed.YearsActive = ToFix.YearsActive.Replace("'", "''");
                    Fixed.URL = ToFix.URL.Replace("'", "''");
                    Fixed.Rating = ToFix.Rating.Replace("'", "''");

                    return Fixed;
                }

                // Compare two objects to see if they have identical values
                public bool IAFD_Actors_Changed(IAFD_Actors OldVal, IAFD_Actors NewVal)
                {
                    IAFD_Actors diff = new IAFD_Actors();
                    List<string> diffList = new List<string>();
                    bool different = false;
                    if (OldVal.ID == null) { OldVal.ID = ""; }
                    if (NewVal.ID == null) { NewVal.ID = ""; }
                    if (OldVal.ID != NewVal.ID) { different = true; }
                    if (OldVal.Name == null) { OldVal.Name = ""; }
                    if (NewVal.Name == null) { NewVal.Name = ""; }
                    if (OldVal.Name != NewVal.Name) { different = true; }
                    if (OldVal.AKA == null) { OldVal.AKA = ""; }
                    if (NewVal.AKA == null) { NewVal.AKA = ""; }
                    if (OldVal.AKA != NewVal.AKA) { different = true; }
                    if (OldVal.Ethnicity == null) { OldVal.Ethnicity = ""; }
                    if (NewVal.Ethnicity == null) { NewVal.Ethnicity = ""; }
                    if (OldVal.Ethnicity != NewVal.Ethnicity) { different = true; }
                    if (OldVal.Nationality == null) { OldVal.Nationality = ""; }
                    if (NewVal.Nationality == null) { NewVal.Nationality = ""; }
                    if (OldVal.Nationality != NewVal.Nationality) { different = true; }
                    if (OldVal.HairColor == null) { OldVal.HairColor = ""; }
                    if (NewVal.HairColor == null) { NewVal.HairColor = ""; }
                    if (OldVal.HairColor != NewVal.HairColor) { different = true; }
                    if (OldVal.Height == null) { OldVal.Height = ""; }
                    if (NewVal.Height == null) { NewVal.Height = ""; }
                    if (OldVal.Height != NewVal.Height) { different = true; }
                    if (OldVal.Weight == null) { OldVal.Weight = ""; }
                    if (NewVal.Weight == null) { NewVal.Weight = ""; }
                    if (OldVal.Weight != NewVal.Weight) { different = true; }
                    if (OldVal.Measurements == null) { OldVal.Measurements = ""; }
                    if (NewVal.Measurements == null) { NewVal.Measurements = ""; }
                    if (OldVal.Measurements != NewVal.Measurements) { different = true; }
                    if (OldVal.Tattoos == null) { OldVal.Tattoos = ""; }
                    if (NewVal.Tattoos == null) { NewVal.Tattoos = ""; }
                    if (OldVal.Tattoos != NewVal.Tattoos) { different = true; }
                    if (OldVal.Piercings == null) { OldVal.Piercings = ""; }
                    if (NewVal.Piercings == null) { NewVal.Piercings = ""; }
                    if (OldVal.Piercings != NewVal.Piercings) { different = true; }
                    if (OldVal.Films == null) { OldVal.Films = ""; }
                    if (NewVal.Films == null) { NewVal.Films = ""; }
                    if (OldVal.Films != NewVal.Films) { different = true; }
                    if (OldVal.Birthday == null) { OldVal.Birthday = ""; }
                    if (NewVal.Birthday == null) { NewVal.Birthday = ""; }
                    if (OldVal.Birthday != NewVal.Birthday) { different = true; }
                    if (OldVal.Astrology == null) { OldVal.Astrology = ""; }
                    if (NewVal.Astrology == null) { NewVal.Astrology = ""; }
                    if (OldVal.Astrology != NewVal.Astrology) { different = true; }
                    if (OldVal.BirthPlace == null) { OldVal.BirthPlace = ""; }
                    if (NewVal.BirthPlace == null) { NewVal.BirthPlace = ""; }
                    if (OldVal.BirthPlace != NewVal.BirthPlace) { different = true; }
                    if (OldVal.YearsActive == null) { OldVal.YearsActive = ""; }
                    if (NewVal.YearsActive == null) { NewVal.YearsActive = ""; }
                    if (OldVal.YearsActive != NewVal.YearsActive) { different = true; }
                    if (OldVal.URL == null) { OldVal.URL = ""; }
                    if (NewVal.URL == null) { NewVal.URL = ""; }
                    if (OldVal.URL != NewVal.URL) { different = true; }
                    if (OldVal.Rating == null) { OldVal.Rating = ""; }
                    if (NewVal.Rating == null) { NewVal.Rating = ""; }
                    if (OldVal.Rating != NewVal.Rating) { different = true; }
                    return different;
                }

                // Returns object containing the new values different from the old values in object comparison
                public IAFD_Actors IAFD_Actors_Diff(IAFD_Actors OldVal, IAFD_Actors NewVal)
                {
                    IAFD_Actors diff = new IAFD_Actors();
                    if (OldVal.ID != NewVal.ID) { diff.ID = NewVal.ID; }
                    if (OldVal.Name != NewVal.Name) { diff.Name = NewVal.Name; }
                    if (OldVal.AKA != NewVal.AKA) { diff.AKA = NewVal.AKA; }
                    if (OldVal.Ethnicity != NewVal.Ethnicity) { diff.Ethnicity = NewVal.Ethnicity; }
                    if (OldVal.Nationality != NewVal.Nationality) { diff.Nationality = NewVal.Nationality; }
                    if (OldVal.HairColor != NewVal.HairColor) { diff.HairColor = NewVal.HairColor; }
                    if (OldVal.Height != NewVal.Height) { diff.Height = NewVal.Height; }
                    if (OldVal.Weight != NewVal.Weight) { diff.Weight = NewVal.Weight; }
                    if (OldVal.Measurements != NewVal.Measurements) { diff.Measurements = NewVal.Measurements; }
                    if (OldVal.Tattoos != NewVal.Tattoos) { diff.Tattoos = NewVal.Tattoos; }
                    if (OldVal.Piercings != NewVal.Piercings) { diff.Piercings = NewVal.Piercings; }
                    if (OldVal.Films != NewVal.Films) { diff.Films = NewVal.Films; }
                    if (OldVal.Birthday != NewVal.Birthday) { diff.Birthday = NewVal.Birthday; }
                    if (OldVal.Astrology != NewVal.Astrology) { diff.Astrology = NewVal.Astrology; }
                    if (OldVal.BirthPlace != NewVal.BirthPlace) { diff.BirthPlace = NewVal.BirthPlace; }
                    if (OldVal.YearsActive != NewVal.YearsActive) { diff.YearsActive = NewVal.YearsActive; }
                    if (OldVal.URL != NewVal.URL) { diff.URL = NewVal.URL; }
                    if (OldVal.Rating != NewVal.Rating) { diff.Rating = NewVal.Rating; }
                    return diff;
                }

                // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
                public List<string> IAFD_Actors_DiffList(IAFD_Actors OldVal, IAFD_Actors NewVal)
                {
                    List<string> diffList = new List<string>();
                    if (OldVal.ID != NewVal.ID) { diffList.Add("Changed ID Value From " + OldVal.ID + " To " + NewVal.ID); }
                    if (OldVal.Name != NewVal.Name) { diffList.Add("Changed Name Value From " + OldVal.Name + " To " + NewVal.Name); }
                    if (OldVal.AKA != NewVal.AKA) { diffList.Add("Changed AKA Value From " + OldVal.AKA + " To " + NewVal.AKA); }
                    if (OldVal.Ethnicity != NewVal.Ethnicity) { diffList.Add("Changed Ethnicity Value From " + OldVal.Ethnicity + " To " + NewVal.Ethnicity); }
                    if (OldVal.Nationality != NewVal.Nationality) { diffList.Add("Changed Nationality Value From " + OldVal.Nationality + " To " + NewVal.Nationality); }
                    if (OldVal.HairColor != NewVal.HairColor) { diffList.Add("Changed HairColor Value From " + OldVal.HairColor + " To " + NewVal.HairColor); }
                    if (OldVal.Height != NewVal.Height) { diffList.Add("Changed Height Value From " + OldVal.Height + " To " + NewVal.Height); }
                    if (OldVal.Weight != NewVal.Weight) { diffList.Add("Changed Weight Value From " + OldVal.Weight + " To " + NewVal.Weight); }
                    if (OldVal.Measurements != NewVal.Measurements) { diffList.Add("Changed Measurements Value From " + OldVal.Measurements + " To " + NewVal.Measurements); }
                    if (OldVal.Tattoos != NewVal.Tattoos) { diffList.Add("Changed Tattoos Value From " + OldVal.Tattoos + " To " + NewVal.Tattoos); }
                    if (OldVal.Piercings != NewVal.Piercings) { diffList.Add("Changed Piercings Value From " + OldVal.Piercings + " To " + NewVal.Piercings); }
                    if (OldVal.Films != NewVal.Films) { diffList.Add("Changed Films Value From " + OldVal.Films + " To " + NewVal.Films); }
                    if (OldVal.Birthday != NewVal.Birthday) { diffList.Add("Changed Birthday Value From " + OldVal.Birthday + " To " + NewVal.Birthday); }
                    if (OldVal.Astrology != NewVal.Astrology) { diffList.Add("Changed Astrology Value From " + OldVal.Astrology + " To " + NewVal.Astrology); }
                    if (OldVal.BirthPlace != NewVal.BirthPlace) { diffList.Add("Changed BirthPlace Value From " + OldVal.BirthPlace + " To " + NewVal.BirthPlace); }
                    if (OldVal.YearsActive != NewVal.YearsActive) { diffList.Add("Changed YearsActive Value From " + OldVal.YearsActive + " To " + NewVal.YearsActive); }
                    if (OldVal.URL != NewVal.URL) { diffList.Add("Changed URL Value From " + OldVal.URL + " To " + NewVal.URL); }
                    if (OldVal.Rating != NewVal.Rating) { diffList.Add("Changed Rating Value From " + OldVal.Rating + " To " + NewVal.Rating); }
                    return diffList;
                }

                // Generate XML String From Object
                public string IAFD_Actors_ToXML(IAFD_Actors obj)
                {
                    string XMLString = "";
                    XMLString = XMLString + "<ID>" + obj.ID + "</ID>";
                    XMLString = XMLString + "<Name>" + obj.Name + "</Name>";
                    XMLString = XMLString + "<AKA>" + obj.AKA + "</AKA>";
                    XMLString = XMLString + "<Ethnicity>" + obj.Ethnicity + "</Ethnicity>";
                    XMLString = XMLString + "<Nationality>" + obj.Nationality + "</Nationality>";
                    XMLString = XMLString + "<HairColor>" + obj.HairColor + "</HairColor>";
                    XMLString = XMLString + "<Height>" + obj.Height + "</Height>";
                    XMLString = XMLString + "<Weight>" + obj.Weight + "</Weight>";
                    XMLString = XMLString + "<Measurements>" + obj.Measurements + "</Measurements>";
                    XMLString = XMLString + "<Tattoos>" + obj.Tattoos + "</Tattoos>";
                    XMLString = XMLString + "<Piercings>" + obj.Piercings + "</Piercings>";
                    XMLString = XMLString + "<Films>" + obj.Films + "</Films>";
                    XMLString = XMLString + "<Birthday>" + obj.Birthday + "</Birthday>";
                    XMLString = XMLString + "<Astrology>" + obj.Astrology + "</Astrology>";
                    XMLString = XMLString + "<BirthPlace>" + obj.BirthPlace + "</BirthPlace>";
                    XMLString = XMLString + "<YearsActive>" + obj.YearsActive + "</YearsActive>";
                    XMLString = XMLString + "<URL>" + obj.URL + "</URL>";
                    XMLString = XMLString + "<Rating>" + obj.Rating + "</Rating>";
                    return XMLString;
                }

                // Populate Object from XML Tag String
                public IAFD_Actors IAFD_Actors_FromXML(string XMLString)
                {
                    IAFD_Actors obj = new IAFD_Actors();
                    obj.ID = prs.XML_Text(XMLString, "<ID>");
                    obj.Name = prs.XML_Text(XMLString, "<Name>");
                    obj.AKA = prs.XML_Text(XMLString, "<AKA>");
                    obj.Ethnicity = prs.XML_Text(XMLString, "<Ethnicity>");
                    obj.Nationality = prs.XML_Text(XMLString, "<Nationality>");
                    obj.HairColor = prs.XML_Text(XMLString, "<HairColor>");
                    obj.Height = prs.XML_Text(XMLString, "<Height>");
                    obj.Weight = prs.XML_Text(XMLString, "<Weight>");
                    obj.Measurements = prs.XML_Text(XMLString, "<Measurements>");
                    obj.Tattoos = prs.XML_Text(XMLString, "<Tattoos>");
                    obj.Piercings = prs.XML_Text(XMLString, "<Piercings>");
                    obj.Films = prs.XML_Text(XMLString, "<Films>");
                    obj.Birthday = prs.XML_Text(XMLString, "<Birthday>");
                    obj.Astrology = prs.XML_Text(XMLString, "<Astrology>");
                    obj.BirthPlace = prs.XML_Text(XMLString, "<BirthPlace>");
                    obj.YearsActive = prs.XML_Text(XMLString, "<YearsActive>");
                    obj.URL = prs.XML_Text(XMLString, "<URL>");
                    obj.Rating = prs.XML_Text(XMLString, "<Rating>");
                    return obj;
                }


                #endregion
                #region ===== IAFD_Actors SQL Functions =====

                // Return IAFD_Actors SQL Connection String
                public SqlConnection IAFD_Actors_Conn()
                {
                    // populate sql connection
                    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMADBIndex"].ConnectionString);
                    // SqlConnection Conn = new SqlConnection("Server=188.168.188.88;DataBase=LucidMedia;Uid=lucidm;Pwd=pass");
                    return conn;
                }

                // Return IAFD_Actors TableName (Full Path)
                public string IAFD_Actors_TableName()
                {
                    // populate to return full sql table name
                    return "[ADBIndex].[dbo].[IAFD_Actors]";
                }

                // Generate SQL Table

                public bool IAFD_Actors_InsertSQL(IAFD_Actors obj)
                {
                    SqlConnection Con = IAFD_Actors_Conn();
                    string SQLLine = "Insert Into " + IAFD_Actors_TableName() + " (Flag, Name, AKA, Ethnicity, Nationality, HairColor, Height, Weight, Measurements, Tattoos, Piercings, Films, Birthday, Astrology, BirthPlace, YearsActive, URL, Rating, HeadShotURL, HeadShotLocalImg) VALUES (@Flag, @Name, @AKA, @Ethnicity, @Nationality, @HairColor, @Height, @Weight, @Measurements, @Tattoos, @Piercings, @Films, @Birthday, @Astrology, @BirthPlace, @YearsActive, @URL, @Rating, @HeadShotURL, @HeadShotLocalImg)";
                    SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                    cmd2 = new SqlCommand(SQLLine, Con);

                    if (obj.Name == null) { obj.Name = ""; }
                    if (obj.AKA == null) { obj.AKA = ""; }
                    if (obj.Ethnicity == null) { obj.Ethnicity = ""; }
                    if (obj.Nationality == null) { obj.Nationality = ""; }
                    if (obj.HairColor == null) { obj.HairColor = ""; }
                    if (obj.Height == null) { obj.Height = ""; }
                    if (obj.Weight == null) { obj.Weight = ""; }
                    if (obj.Measurements == null) { obj.Measurements = ""; }
                    if (obj.Tattoos == null) { obj.Tattoos = ""; }
                    if (obj.Piercings == null) { obj.Piercings = ""; }
                    if (obj.Films == null) { obj.Films = ""; }
                    if (obj.Birthday == null) { obj.Birthday = ""; }
                    if (obj.Astrology == null) { obj.Astrology = ""; }
                    if (obj.BirthPlace == null) { obj.BirthPlace = ""; }
                    if (obj.YearsActive == null) { obj.YearsActive = ""; }
                    if (obj.URL == null) { obj.URL = ""; }
                    if (obj.Rating == null) { obj.Rating = ""; }
                    if (obj.HeadShotURL == null) { obj.HeadShotURL = ""; }
                    if (obj.HeadShotLocalImg == null) { obj.HeadShotLocalImg = ""; }

                    cmd2.Parameters.AddWithValue(@"Flag", obj.Flag.ToString());
                    cmd2.Parameters.AddWithValue(@"Name", obj.Name.ToString());
                    cmd2.Parameters.AddWithValue(@"AKA", obj.AKA.ToString());
                    cmd2.Parameters.AddWithValue(@"Ethnicity", obj.Ethnicity.ToString());
                    cmd2.Parameters.AddWithValue(@"Nationality", obj.Nationality.ToString());
                    cmd2.Parameters.AddWithValue(@"HairColor", obj.HairColor.ToString());
                    cmd2.Parameters.AddWithValue(@"Height", obj.Height.ToString());
                    cmd2.Parameters.AddWithValue(@"Weight", obj.Weight.ToString());
                    cmd2.Parameters.AddWithValue(@"Measurements", obj.Measurements.ToString());
                    cmd2.Parameters.AddWithValue(@"Tattoos", obj.Tattoos.ToString());
                    cmd2.Parameters.AddWithValue(@"Piercings", obj.Piercings.ToString());
                    cmd2.Parameters.AddWithValue(@"Films", obj.Films.ToString());
                    cmd2.Parameters.AddWithValue(@"Birthday", obj.Birthday.ToString());
                    cmd2.Parameters.AddWithValue(@"Astrology", obj.Astrology.ToString());
                    cmd2.Parameters.AddWithValue(@"BirthPlace", obj.BirthPlace.ToString());
                    cmd2.Parameters.AddWithValue(@"YearsActive", obj.YearsActive.ToString());
                    cmd2.Parameters.AddWithValue(@"URL", obj.URL.ToString());
                    cmd2.Parameters.AddWithValue(@"Rating", obj.Rating.ToString());
                    cmd2.Parameters.AddWithValue(@"HeadShotURL", obj.HeadShotURL.ToString());
                    cmd2.Parameters.AddWithValue(@"HeadShotLocalImg", obj.HeadShotLocalImg.ToString());

                    if (Con.State == ConnectionState.Closed) { Con.Open(); }
                    int recordsAffected = 0;
                    try { recordsAffected = cmd2.ExecuteNonQuery(); }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 1205) // Retry on DeadLock
                        {
                            ahk.Sleep(1000);
                            IAFD_Actors_InsertSQL(obj);
                        }
                        else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                        {
                            ahk.Sleep(1000);
                            IAFD_Actors_InsertSQL(obj);
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

                public bool IAFD_Actors_UpdateSQL(IAFD_Actors obj)
                {
                    SqlConnection Conn = IAFD_Actors_Conn();
                    string SQLLine = "Update " + IAFD_Actors_TableName() + " SET Flag = @Flag, Name = @Name, AKA = @AKA, Ethnicity = @Ethnicity, Nationality = @Nationality, HairColor = @HairColor, Height = @Height, Weight = @Weight, Measurements = @Measurements, Tattoos = @Tattoos, Piercings = @Piercings, Films = @Films, Birthday = @Birthday, Astrology = @Astrology, BirthPlace = @BirthPlace, YearsActive = @YearsActive, Rating = @Rating, HeadShotURL = @HeadShotURL, HeadShotLocalImg = @HeadShotLocalImg WHERE URL = @URL";
                    SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                    cmd2 = new SqlCommand(SQLLine, Conn);

                    if (obj.Name == null) { obj.Name = ""; }
                    if (obj.AKA == null) { obj.AKA = ""; }
                    if (obj.Ethnicity == null) { obj.Ethnicity = ""; }
                    if (obj.Nationality == null) { obj.Nationality = ""; }
                    if (obj.HairColor == null) { obj.HairColor = ""; }
                    if (obj.Height == null) { obj.Height = ""; }
                    if (obj.Weight == null) { obj.Weight = ""; }
                    if (obj.Measurements == null) { obj.Measurements = ""; }
                    if (obj.Tattoos == null) { obj.Tattoos = ""; }
                    if (obj.Piercings == null) { obj.Piercings = ""; }
                    if (obj.Films == null) { obj.Films = ""; }
                    if (obj.Birthday == null) { obj.Birthday = ""; }
                    if (obj.Astrology == null) { obj.Astrology = ""; }
                    if (obj.BirthPlace == null) { obj.BirthPlace = ""; }
                    if (obj.YearsActive == null) { obj.YearsActive = ""; }
                    if (obj.URL == null) { obj.URL = ""; }
                    if (obj.Rating == null) { obj.Rating = ""; }
                    if (obj.HeadShotURL == null) { obj.HeadShotURL = ""; }
                    if (obj.HeadShotLocalImg == null) { obj.HeadShotLocalImg = ""; }

                    cmd2.Parameters.AddWithValue(@"Flag", obj.Flag.ToString());
                    cmd2.Parameters.AddWithValue(@"Name", obj.Name.ToString());
                    cmd2.Parameters.AddWithValue(@"AKA", obj.AKA.ToString());
                    cmd2.Parameters.AddWithValue(@"Ethnicity", obj.Ethnicity.ToString());
                    cmd2.Parameters.AddWithValue(@"Nationality", obj.Nationality.ToString());
                    cmd2.Parameters.AddWithValue(@"HairColor", obj.HairColor.ToString());
                    cmd2.Parameters.AddWithValue(@"Height", obj.Height.ToString());
                    cmd2.Parameters.AddWithValue(@"Weight", obj.Weight.ToString());
                    cmd2.Parameters.AddWithValue(@"Measurements", obj.Measurements.ToString());
                    cmd2.Parameters.AddWithValue(@"Tattoos", obj.Tattoos.ToString());
                    cmd2.Parameters.AddWithValue(@"Piercings", obj.Piercings.ToString());
                    cmd2.Parameters.AddWithValue(@"Films", obj.Films.ToString());
                    cmd2.Parameters.AddWithValue(@"Birthday", obj.Birthday.ToString());
                    cmd2.Parameters.AddWithValue(@"Astrology", obj.Astrology.ToString());
                    cmd2.Parameters.AddWithValue(@"BirthPlace", obj.BirthPlace.ToString());
                    cmd2.Parameters.AddWithValue(@"YearsActive", obj.YearsActive.ToString());
                    cmd2.Parameters.AddWithValue(@"URL", obj.URL.ToString());
                    cmd2.Parameters.AddWithValue(@"Rating", obj.Rating.ToString());
                    cmd2.Parameters.AddWithValue(@"HeadShotURL", obj.HeadShotURL.ToString());
                    cmd2.Parameters.AddWithValue(@"HeadShotLocalImg", obj.HeadShotLocalImg.ToString());

                    if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                    int recordsAffected = 0;
                    try { recordsAffected = cmd2.ExecuteNonQuery(); }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 1205) // Retry on DeadLock
                        {
                            ahk.Sleep(1000);
                            IAFD_Actors_UpdateSQL(obj);
                        }
                        else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                        {
                            ahk.Sleep(1000);
                            IAFD_Actors_UpdateSQL(obj);
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

                public bool IAFD_Actors_UpdateInsert(IAFD_Actors obj)
                {
                    SqlConnection Conn = IAFD_Actors_Conn();
                    bool Updated = IAFD_Actors_UpdateSQL(obj);  // try to update record first
                    if (!Updated) { Updated = IAFD_Actors_InsertSQL(obj); }  // if unable to update, insert new record
                    return Updated;
                }

                // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
                public bool IAFD_Actors_UpdateIfPopulated(IAFD_Actors obj, string ID = "")
                {
                    SqlConnection Conn = IAFD_Actors_Conn();
                    string SQLcmd = "Update " + IAFD_Actors_TableName() + " SET ";

                    if (obj.Name != null) { SQLcmd = SQLcmd + " Name = @Name,"; }
                    if (obj.AKA != null) { SQLcmd = SQLcmd + " AKA = @AKA,"; }
                    if (obj.Ethnicity != null) { SQLcmd = SQLcmd + " Ethnicity = @Ethnicity,"; }
                    if (obj.Nationality != null) { SQLcmd = SQLcmd + " Nationality = @Nationality,"; }
                    if (obj.HairColor != null) { SQLcmd = SQLcmd + " HairColor = @HairColor,"; }
                    if (obj.Height != null) { SQLcmd = SQLcmd + " Height = @Height,"; }
                    if (obj.Weight != null) { SQLcmd = SQLcmd + " Weight = @Weight,"; }
                    if (obj.Measurements != null) { SQLcmd = SQLcmd + " Measurements = @Measurements,"; }
                    if (obj.Tattoos != null) { SQLcmd = SQLcmd + " Tattoos = @Tattoos,"; }
                    if (obj.Piercings != null) { SQLcmd = SQLcmd + " Piercings = @Piercings,"; }
                    if (obj.Films != null) { SQLcmd = SQLcmd + " Films = @Films,"; }
                    if (obj.Birthday != null) { SQLcmd = SQLcmd + " Birthday = @Birthday,"; }
                    if (obj.Astrology != null) { SQLcmd = SQLcmd + " Astrology = @Astrology,"; }
                    if (obj.BirthPlace != null) { SQLcmd = SQLcmd + " BirthPlace = @BirthPlace,"; }
                    if (obj.YearsActive != null) { SQLcmd = SQLcmd + " YearsActive = @YearsActive,"; }
                    if (obj.URL != null) { SQLcmd = SQLcmd + " URL = @URL,"; }
                    if (obj.Rating != null) { SQLcmd = SQLcmd + " Rating = @Rating,"; }
                    SQLcmd = ahk.TrimLast(SQLcmd, 1);
                    SQLcmd = SQLcmd + " WHERE ID = @ID";

                    SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                    if (obj.Name != null) { cmd2.Parameters.AddWithValue(@"Name", obj.Name); }
                    if (obj.AKA != null) { cmd2.Parameters.AddWithValue(@"AKA", obj.AKA); }
                    if (obj.Ethnicity != null) { cmd2.Parameters.AddWithValue(@"Ethnicity", obj.Ethnicity); }
                    if (obj.Nationality != null) { cmd2.Parameters.AddWithValue(@"Nationality", obj.Nationality); }
                    if (obj.HairColor != null) { cmd2.Parameters.AddWithValue(@"HairColor", obj.HairColor); }
                    if (obj.Height != null) { cmd2.Parameters.AddWithValue(@"Height", obj.Height); }
                    if (obj.Weight != null) { cmd2.Parameters.AddWithValue(@"Weight", obj.Weight); }
                    if (obj.Measurements != null) { cmd2.Parameters.AddWithValue(@"Measurements", obj.Measurements); }
                    if (obj.Tattoos != null) { cmd2.Parameters.AddWithValue(@"Tattoos", obj.Tattoos); }
                    if (obj.Piercings != null) { cmd2.Parameters.AddWithValue(@"Piercings", obj.Piercings); }
                    if (obj.Films != null) { cmd2.Parameters.AddWithValue(@"Films", obj.Films); }
                    if (obj.Birthday != null) { cmd2.Parameters.AddWithValue(@"Birthday", obj.Birthday); }
                    if (obj.Astrology != null) { cmd2.Parameters.AddWithValue(@"Astrology", obj.Astrology); }
                    if (obj.BirthPlace != null) { cmd2.Parameters.AddWithValue(@"BirthPlace", obj.BirthPlace); }
                    if (obj.YearsActive != null) { cmd2.Parameters.AddWithValue(@"YearsActive", obj.YearsActive); }
                    if (obj.URL != null) { cmd2.Parameters.AddWithValue(@"URL", obj.URL); }
                    if (obj.Rating != null) { cmd2.Parameters.AddWithValue(@"Rating", obj.Rating); }

                    if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                    int recordsAffected = 0;
                    try { recordsAffected = cmd2.ExecuteNonQuery(); }
                    catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                    Conn.Close();
                    if (recordsAffected > 0) { return true; }
                    else return false;
                }

                /// <summary>
                /// Check Local DB to See if Actor Already Has Populated Info 
                /// </summary>
                /// <param name="URL"></param>
                /// <returns></returns>
                public bool ActorInfoPopulated(string URL)
                {
                    IAFD_Actors info = IAFD_Actors_ReturnSQL(URL);
                    bool InfoFound = false;
                    if (info.Name != "" && info.Name != null) { InfoFound = true; }
                    return InfoFound;
                }

                public IAFD_Actors IAFD_Actors_ReturnSQL(string URL)
                {
                    SqlConnection Conn = IAFD_Actors_Conn();
                    string SelectLine = "Select [ID],[Name],[AKA],[Ethnicity],[Nationality],[HairColor],[Height],[Weight],[Measurements],[Tattoos],[Piercings],[Films],[Birthday],[Astrology],[BirthPlace],[YearsActive],[URL],[Rating],[Flag],[HeadShotURL],[HeadShotLocalImg] From " + IAFD_Actors_TableName() + " WHERE [URL] = '" + URL + "'";
                    DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                    IAFD_Actors returnObject = new IAFD_Actors();
                    if (ReturnTable != null)
                    {
                        foreach (DataRow ret in ReturnTable.Rows)
                        {
                            returnObject.ID = ret["ID"].ToString();
                            returnObject.Name = ret["Name"].ToString();
                            returnObject.AKA = ret["AKA"].ToString();
                            returnObject.Ethnicity = ret["Ethnicity"].ToString();
                            returnObject.Nationality = ret["Nationality"].ToString();
                            returnObject.HairColor = ret["HairColor"].ToString();
                            returnObject.Height = ret["Height"].ToString();
                            returnObject.Weight = ret["Weight"].ToString();
                            returnObject.Measurements = ret["Measurements"].ToString();
                            returnObject.Tattoos = ret["Tattoos"].ToString();
                            returnObject.Piercings = ret["Piercings"].ToString();
                            returnObject.Films = ret["Films"].ToString();
                            returnObject.Birthday = ret["Birthday"].ToString();
                            returnObject.Astrology = ret["Astrology"].ToString();
                            returnObject.BirthPlace = ret["BirthPlace"].ToString();
                            returnObject.YearsActive = ret["YearsActive"].ToString();
                            returnObject.URL = ret["URL"].ToString();
                            returnObject.Rating = ret["Rating"].ToString();
                            returnObject.Flag = ret["Flag"].ToBool();
                            returnObject.HeadShotLocalImg = ret["HeadShotLocalImg"].ToString();
                            returnObject.HeadShotURL = ret["HeadShotURL"].ToString();

                            return returnObject;
                        }
                    }
                    return returnObject;
                }

                public List<IAFD_Actors> IAFD_Actors_ReturnSQLList(string Command = "")
                {
                    if (Command == "") { Command = "Select * From " + IAFD_Actors_TableName(); }
                    SqlConnection Conn = IAFD_Actors_Conn();
                    DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                    List<IAFD_Actors> ReturnList = new List<IAFD_Actors>();
                    if (ReturnTable != null)
                    {
                        foreach (DataRow ret in ReturnTable.Rows)
                        {
                            IAFD_Actors returnObject = new IAFD_Actors();
                            returnObject.ID = ret["ID"].ToString();
                            returnObject.Name = ret["Name"].ToString();
                            returnObject.AKA = ret["AKA"].ToString();
                            returnObject.Ethnicity = ret["Ethnicity"].ToString();
                            returnObject.Nationality = ret["Nationality"].ToString();
                            returnObject.HairColor = ret["HairColor"].ToString();
                            returnObject.Height = ret["Height"].ToString();
                            returnObject.Weight = ret["Weight"].ToString();
                            returnObject.Measurements = ret["Measurements"].ToString();
                            returnObject.Tattoos = ret["Tattoos"].ToString();
                            returnObject.Piercings = ret["Piercings"].ToString();
                            returnObject.Films = ret["Films"].ToString();
                            returnObject.Birthday = ret["Birthday"].ToString();
                            returnObject.Astrology = ret["Astrology"].ToString();
                            returnObject.BirthPlace = ret["BirthPlace"].ToString();
                            returnObject.YearsActive = ret["YearsActive"].ToString();
                            returnObject.URL = ret["URL"].ToString();
                            returnObject.Rating = ret["Rating"].ToString();
                            returnObject.Flag = ret["Flag"].ToBool();
                            returnObject.HeadShotLocalImg = ret["HeadShotLocalImg"].ToString();
                            returnObject.HeadShotURL = ret["HeadShotURL"].ToString();
                            ReturnList.Add(returnObject);
                        }
                    }
                    return ReturnList;
                }

                /// <summary>
                /// Returns List of Actor URLS from Db that are Flagged to Download More Info
                /// </summary>
                /// <returns></returns>
                public List<string> Return_Flagged_ActorURLs()
                {
                    string Command = "Select [URL] From " + IAFD_Actors_TableName() + " WHERE [Flag] = '1'";
                    SqlConnection Conn = IAFD_Actors_Conn();
                    DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                    List<string> ReturnList = new List<string>();
                    if (ReturnTable != null)
                    {
                        foreach (DataRow ret in ReturnTable.Rows)
                        {
                            ReturnList.Add(ret["URL"].ToString());
                        }
                    }
                    return ReturnList;
                }

                public bool IAFD_Actors_SQL_to_SQLite(string SqliteDBPath = @"\Db\IAFD_Actors.sqlite")
                {
                    _Database.SQLite sqlite = new _Database.SQLite();

                    string SaveFile = SqliteDBPath;
                    if (SqliteDBPath == @"\Db\IAFD_Actors.sqlite")
                    {
                        ahk.FileCreateDir(ahk.AppDir() + @"\Db");
                        SaveFile = ahk.AppDir() + @"\Db\IAFD_Actors.sqlite";
                    }

                    //sb.StatusBar("Copying SQL Db to " + SaveFile + "...");
                    sqlite.SQLTable_To_NewSQLiteTable(IAFD_Actors_Conn(), "IAFD_Actors", "IAFD_Actors", SaveFile, "", false, false, false);
                    //sb.StatusBar("FINISHED Copying SQL Db to " + SaveFile);

                    if (File.Exists(SaveFile)) { return true; } else { return false; }
                }


                #endregion

                #endregion


                #region === IAFD_Movies FUNCTIONS ===

                #region ===== IAFD_Movies Object =====

                public struct IAFD_Movies
                {
                    public string ID { get; set; }
                    public bool Flag { get; set; }
                    public string Title { get; set; }
                    public string AltTitle { get; set; }
                    public string Year { get; set; }
                    public string AllGirl { get; set; }
                    public string AllMale { get; set; }
                    public string Compilation { get; set; }
                    public string Studio { get; set; }
                    public string Director { get; set; }
                    public string Actors { get; set; }
                    public string Minutes { get; set; }
                    public string ReleaseDate { get; set; }
                    public string URL { get; set; }
                    public string FilePath { get; set; }
                    public string FileSize { get; set; }
                    public string Rating { get; set; }
                    public string Quality { get; set; }
                    public string FilmImageURL { get; set; }
                    public string FilmImageLocal { get; set; }
                }
                //  Fix illegal characters before Sql/Sqlite Db Inserts
                public IAFD_Movies IAFD_Movies_FixChars(IAFD_Movies ToFix)
                {
                    IAFD_Movies Fixed = new IAFD_Movies();

                    Fixed.ID = ToFix.ID.Replace("'", "''");
                    Fixed.Title = ToFix.Title.Replace("'", "''");
                    Fixed.AltTitle = ToFix.AltTitle.Replace("'", "''");
                    Fixed.Year = ToFix.Year.Replace("'", "''");
                    Fixed.AllGirl = ToFix.AllGirl.Replace("'", "''");
                    Fixed.AllMale = ToFix.AllMale.Replace("'", "''");
                    Fixed.Compilation = ToFix.Compilation.Replace("'", "''");
                    Fixed.Studio = ToFix.Studio.Replace("'", "''");
                    Fixed.Director = ToFix.Director.Replace("'", "''");
                    Fixed.Actors = ToFix.Actors.Replace("'", "''");
                    Fixed.Minutes = ToFix.Minutes.Replace("'", "''");
                    Fixed.ReleaseDate = ToFix.ReleaseDate.Replace("'", "''");
                    Fixed.URL = ToFix.URL.Replace("'", "''");
                    Fixed.FilePath = ToFix.FilePath.Replace("'", "''");
                    Fixed.FileSize = ToFix.FileSize.Replace("'", "''");
                    Fixed.Rating = ToFix.Rating.Replace("'", "''");
                    Fixed.Quality = ToFix.Quality.Replace("'", "''");

                    return Fixed;
                }

                // Compare two objects to see if they have identical values
                public bool IAFD_Movies_Changed(IAFD_Movies OldVal, IAFD_Movies NewVal)
                {
                    IAFD_Movies diff = new IAFD_Movies();
                    List<string> diffList = new List<string>();
                    bool different = false;
                    if (OldVal.ID == null) { OldVal.ID = ""; }
                    if (NewVal.ID == null) { NewVal.ID = ""; }
                    if (OldVal.ID != NewVal.ID) { different = true; }
                    if (OldVal.Title == null) { OldVal.Title = ""; }
                    if (NewVal.Title == null) { NewVal.Title = ""; }
                    if (OldVal.Title != NewVal.Title) { different = true; }
                    if (OldVal.AltTitle == null) { OldVal.AltTitle = ""; }
                    if (NewVal.AltTitle == null) { NewVal.AltTitle = ""; }
                    if (OldVal.AltTitle != NewVal.AltTitle) { different = true; }
                    if (OldVal.Year == null) { OldVal.Year = ""; }
                    if (NewVal.Year == null) { NewVal.Year = ""; }
                    if (OldVal.Year != NewVal.Year) { different = true; }
                    if (OldVal.AllGirl == null) { OldVal.AllGirl = ""; }
                    if (NewVal.AllGirl == null) { NewVal.AllGirl = ""; }
                    if (OldVal.AllGirl != NewVal.AllGirl) { different = true; }
                    if (OldVal.AllMale == null) { OldVal.AllMale = ""; }
                    if (NewVal.AllMale == null) { NewVal.AllMale = ""; }
                    if (OldVal.AllMale != NewVal.AllMale) { different = true; }
                    if (OldVal.Compilation == null) { OldVal.Compilation = ""; }
                    if (NewVal.Compilation == null) { NewVal.Compilation = ""; }
                    if (OldVal.Compilation != NewVal.Compilation) { different = true; }
                    if (OldVal.Studio == null) { OldVal.Studio = ""; }
                    if (NewVal.Studio == null) { NewVal.Studio = ""; }
                    if (OldVal.Studio != NewVal.Studio) { different = true; }
                    if (OldVal.Director == null) { OldVal.Director = ""; }
                    if (NewVal.Director == null) { NewVal.Director = ""; }
                    if (OldVal.Director != NewVal.Director) { different = true; }
                    if (OldVal.Actors == null) { OldVal.Actors = ""; }
                    if (NewVal.Actors == null) { NewVal.Actors = ""; }
                    if (OldVal.Actors != NewVal.Actors) { different = true; }
                    if (OldVal.Minutes == null) { OldVal.Minutes = ""; }
                    if (NewVal.Minutes == null) { NewVal.Minutes = ""; }
                    if (OldVal.Minutes != NewVal.Minutes) { different = true; }
                    if (OldVal.ReleaseDate == null) { OldVal.ReleaseDate = ""; }
                    if (NewVal.ReleaseDate == null) { NewVal.ReleaseDate = ""; }
                    if (OldVal.ReleaseDate != NewVal.ReleaseDate) { different = true; }
                    if (OldVal.URL == null) { OldVal.URL = ""; }
                    if (NewVal.URL == null) { NewVal.URL = ""; }
                    if (OldVal.URL != NewVal.URL) { different = true; }
                    if (OldVal.FilePath == null) { OldVal.FilePath = ""; }
                    if (NewVal.FilePath == null) { NewVal.FilePath = ""; }
                    if (OldVal.FilePath != NewVal.FilePath) { different = true; }
                    if (OldVal.FileSize == null) { OldVal.FileSize = ""; }
                    if (NewVal.FileSize == null) { NewVal.FileSize = ""; }
                    if (OldVal.FileSize != NewVal.FileSize) { different = true; }
                    if (OldVal.Rating == null) { OldVal.Rating = ""; }
                    if (NewVal.Rating == null) { NewVal.Rating = ""; }
                    if (OldVal.Rating != NewVal.Rating) { different = true; }
                    if (OldVal.Quality == null) { OldVal.Quality = ""; }
                    if (NewVal.Quality == null) { NewVal.Quality = ""; }
                    if (OldVal.Quality != NewVal.Quality) { different = true; }
                    return different;
                }

                // Returns object containing the new values different from the old values in object comparison
                public IAFD_Movies IAFD_Movies_Diff(IAFD_Movies OldVal, IAFD_Movies NewVal)
                {
                    IAFD_Movies diff = new IAFD_Movies();
                    if (OldVal.ID != NewVal.ID) { diff.ID = NewVal.ID; }
                    if (OldVal.Title != NewVal.Title) { diff.Title = NewVal.Title; }
                    if (OldVal.AltTitle != NewVal.AltTitle) { diff.AltTitle = NewVal.AltTitle; }
                    if (OldVal.Year != NewVal.Year) { diff.Year = NewVal.Year; }
                    if (OldVal.AllGirl != NewVal.AllGirl) { diff.AllGirl = NewVal.AllGirl; }
                    if (OldVal.AllMale != NewVal.AllMale) { diff.AllMale = NewVal.AllMale; }
                    if (OldVal.Compilation != NewVal.Compilation) { diff.Compilation = NewVal.Compilation; }
                    if (OldVal.Studio != NewVal.Studio) { diff.Studio = NewVal.Studio; }
                    if (OldVal.Director != NewVal.Director) { diff.Director = NewVal.Director; }
                    if (OldVal.Actors != NewVal.Actors) { diff.Actors = NewVal.Actors; }
                    if (OldVal.Minutes != NewVal.Minutes) { diff.Minutes = NewVal.Minutes; }
                    if (OldVal.ReleaseDate != NewVal.ReleaseDate) { diff.ReleaseDate = NewVal.ReleaseDate; }
                    if (OldVal.URL != NewVal.URL) { diff.URL = NewVal.URL; }
                    if (OldVal.FilePath != NewVal.FilePath) { diff.FilePath = NewVal.FilePath; }
                    if (OldVal.FileSize != NewVal.FileSize) { diff.FileSize = NewVal.FileSize; }
                    if (OldVal.Rating != NewVal.Rating) { diff.Rating = NewVal.Rating; }
                    if (OldVal.Quality != NewVal.Quality) { diff.Quality = NewVal.Quality; }
                    return diff;
                }

                // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
                public List<string> IAFD_Movies_DiffList(IAFD_Movies OldVal, IAFD_Movies NewVal)
                {
                    List<string> diffList = new List<string>();
                    if (OldVal.ID != NewVal.ID) { diffList.Add("Changed ID Value From " + OldVal.ID + " To " + NewVal.ID); }
                    if (OldVal.Title != NewVal.Title) { diffList.Add("Changed Title Value From " + OldVal.Title + " To " + NewVal.Title); }
                    if (OldVal.AltTitle != NewVal.AltTitle) { diffList.Add("Changed AltTitle Value From " + OldVal.AltTitle + " To " + NewVal.AltTitle); }
                    if (OldVal.Year != NewVal.Year) { diffList.Add("Changed Year Value From " + OldVal.Year + " To " + NewVal.Year); }
                    if (OldVal.AllGirl != NewVal.AllGirl) { diffList.Add("Changed AllGirl Value From " + OldVal.AllGirl + " To " + NewVal.AllGirl); }
                    if (OldVal.AllMale != NewVal.AllMale) { diffList.Add("Changed AllMale Value From " + OldVal.AllMale + " To " + NewVal.AllMale); }
                    if (OldVal.Compilation != NewVal.Compilation) { diffList.Add("Changed Compilation Value From " + OldVal.Compilation + " To " + NewVal.Compilation); }
                    if (OldVal.Studio != NewVal.Studio) { diffList.Add("Changed Studio Value From " + OldVal.Studio + " To " + NewVal.Studio); }
                    if (OldVal.Director != NewVal.Director) { diffList.Add("Changed Director Value From " + OldVal.Director + " To " + NewVal.Director); }
                    if (OldVal.Actors != NewVal.Actors) { diffList.Add("Changed Actors Value From " + OldVal.Actors + " To " + NewVal.Actors); }
                    if (OldVal.Minutes != NewVal.Minutes) { diffList.Add("Changed Minutes Value From " + OldVal.Minutes + " To " + NewVal.Minutes); }
                    if (OldVal.ReleaseDate != NewVal.ReleaseDate) { diffList.Add("Changed ReleaseDate Value From " + OldVal.ReleaseDate + " To " + NewVal.ReleaseDate); }
                    if (OldVal.URL != NewVal.URL) { diffList.Add("Changed URL Value From " + OldVal.URL + " To " + NewVal.URL); }
                    if (OldVal.FilePath != NewVal.FilePath) { diffList.Add("Changed FilePath Value From " + OldVal.FilePath + " To " + NewVal.FilePath); }
                    if (OldVal.FileSize != NewVal.FileSize) { diffList.Add("Changed FileSize Value From " + OldVal.FileSize + " To " + NewVal.FileSize); }
                    if (OldVal.Rating != NewVal.Rating) { diffList.Add("Changed Rating Value From " + OldVal.Rating + " To " + NewVal.Rating); }
                    if (OldVal.Quality != NewVal.Quality) { diffList.Add("Changed Quality Value From " + OldVal.Quality + " To " + NewVal.Quality); }
                    return diffList;
                }

                // Generate XML String From Object
                public string IAFD_Movies_ToXML(IAFD_Movies obj)
                {
                    string XMLString = "";
                    XMLString = XMLString + "<ID>" + obj.ID + "</ID>";
                    XMLString = XMLString + "<Title>" + obj.Title + "</Title>";
                    XMLString = XMLString + "<AltTitle>" + obj.AltTitle + "</AltTitle>";
                    XMLString = XMLString + "<Year>" + obj.Year + "</Year>";
                    XMLString = XMLString + "<AllGirl>" + obj.AllGirl + "</AllGirl>";
                    XMLString = XMLString + "<AllMale>" + obj.AllMale + "</AllMale>";
                    XMLString = XMLString + "<Compilation>" + obj.Compilation + "</Compilation>";
                    XMLString = XMLString + "<Studio>" + obj.Studio + "</Studio>";
                    XMLString = XMLString + "<Director>" + obj.Director + "</Director>";
                    XMLString = XMLString + "<Actors>" + obj.Actors + "</Actors>";
                    XMLString = XMLString + "<Minutes>" + obj.Minutes + "</Minutes>";
                    XMLString = XMLString + "<ReleaseDate>" + obj.ReleaseDate + "</ReleaseDate>";
                    XMLString = XMLString + "<URL>" + obj.URL + "</URL>";
                    XMLString = XMLString + "<FilePath>" + obj.FilePath + "</FilePath>";
                    XMLString = XMLString + "<FileSize>" + obj.FileSize + "</FileSize>";
                    XMLString = XMLString + "<Rating>" + obj.Rating + "</Rating>";
                    XMLString = XMLString + "<Quality>" + obj.Quality + "</Quality>";
                    return XMLString;
                }

                // Populate Object from XML Tag String
                public IAFD_Movies IAFD_Movies_FromXML(string XMLString)
                {
                    IAFD_Movies obj = new IAFD_Movies();
                    obj.ID = prs.XML_Text(XMLString, "<ID>");
                    obj.Title = prs.XML_Text(XMLString, "<Title>");
                    obj.AltTitle = prs.XML_Text(XMLString, "<AltTitle>");
                    obj.Year = prs.XML_Text(XMLString, "<Year>");
                    obj.AllGirl = prs.XML_Text(XMLString, "<AllGirl>");
                    obj.AllMale = prs.XML_Text(XMLString, "<AllMale>");
                    obj.Compilation = prs.XML_Text(XMLString, "<Compilation>");
                    obj.Studio = prs.XML_Text(XMLString, "<Studio>");
                    obj.Director = prs.XML_Text(XMLString, "<Director>");
                    obj.Actors = prs.XML_Text(XMLString, "<Actors>");
                    obj.Minutes = prs.XML_Text(XMLString, "<Minutes>");
                    obj.ReleaseDate = prs.XML_Text(XMLString, "<ReleaseDate>");
                    obj.URL = prs.XML_Text(XMLString, "<URL>");
                    obj.FilePath = prs.XML_Text(XMLString, "<FilePath>");
                    obj.FileSize = prs.XML_Text(XMLString, "<FileSize>");
                    obj.Rating = prs.XML_Text(XMLString, "<Rating>");
                    obj.Quality = prs.XML_Text(XMLString, "<Quality>");
                    return obj;
                }


                #endregion
                #region ===== IAFD_Movies SQL Functions =====


                /// <summary>
                /// Searches Movie DB for Duplicate URLs and writes flag to DB
                /// </summary>
                /// <returns></returns>
                public int Flag_Dupe_URLs(RadButton button = null, bool NewThread = true)
                {
                    if (NewThread)
                    {
                        Thread newThread = new Thread(() => Flag_Dupe_URLs(button, false)); // Function To Execute
                        newThread.Name = "Flag Dupe Entries";
                        newThread.IsBackground = true;
                        newThread.Start();
                    }
                    else
                    {
                        _TelerikLib tel = new _TelerikLib();
                        SqlConnection Con = IAFD_Movies_Conn();
                        int flagged = 0;
                        List<string> commands = new List<string>();
                        List<string> UpdatedURLs = new List<string>();
                        List<string> urls = Movie_URLs();
                        int i = 0;
                        foreach (string url in urls)
                        {
                            i++;

                            //if (lst.InList(UpdatedURLs, url))
                            //{
                            //    if (button != null) { tel.Update(button, i + "/" + urls.Count); }
                            //    continue;
                            //}

                            int count = Movie_URL_Count(urls, url);
                            if (count > 1)
                            {

                                /// delete duplicate entries by id
                                List<string> ids = DupeMovieIDs(url);
                                foreach (string id in ids)
                                {
                                    bool deleted = DeleteByID(id);
                                    tel.Update(button, "Deleted " + id + " | " + i + "/" + urls.Count);
                                    break;
                                }

                                ////string cmd = "Update " + IAFD_Movies_TableName() + " Set [Quality] = 'DUPE' WHERE [URL] = '" + url + "'";
                                //bool updated = Flag_Dupe_URL(url);
                                //if (!updated) { ahk.MsgBox("Error Updating " + url); }
                                //else
                                //{
                                //    UpdatedURLs.Add(url);
                                //    flagged++;
                                //    if (button != null) { tel.Update(button, i + "/" + urls.Count + " | Flagged = " + flagged); }
                                //}

                                //commands.Add(cmd);

                                //if (commands.Count > 200)  // write to db when 200 entries collected
                                //{
                                //    if (button != null) { tel.Update(button, i + "/" + urls.Count + " | Writing SQL..."); }
                                //    int updated = sql.WriteSQL_List(Con, commands);
                                //    commands = new List<string>();
                                //    if (button != null) { tel.Update(button, i + "/" + urls.Count + " | Wrote : " + updated); }
                                //}
                                //else
                                //{
                                //    if (button != null) { tel.Update(button, i + "/" + urls.Count + " | ToUpdate: " + commands.Count); }
                                //}
                            }
                            else
                            {
                                if (button != null) { tel.Update(button, i + "/" + urls.Count + " | ToUpdate: " + commands.Count); }
                            }
                        }

                        return flagged;
                    }


                    return 0;
                }


                public int Deleted_Flagged_Dupe_URLs(RadButton button = null, bool NewThread = true)
                {
                    if (NewThread)
                    {
                        Thread newThread = new Thread(() => Flag_Dupe_URLs(button, false)); // Function To Execute
                        newThread.Name = "Flag Dupe Entries";
                        newThread.IsBackground = true;
                        newThread.Start();
                    }
                    else
                    {
                        _TelerikLib tel = new _TelerikLib();
                        SqlConnection Con = IAFD_Movies_Conn();
                        int flagged = 0;
                        List<string> commands = new List<string>();
                        List<string> UpdatedURLs = new List<string>();
                        List<string> urls = Movie_URLs();
                        int i = 0;
                        foreach (string url in urls)
                        {
                            i++;

                            int count = Movie_URL_Count(urls, url);
                            if (count > 1)
                            {

                                /// delete duplicate entries by id
                                List<string> ids = DupeMovieIDs(url);
                                foreach (string id in ids)
                                {
                                    bool deleted = DeleteByID(id);
                                    tel.Update(button, "Deleted " + id + " | " + i + "/" + urls.Count);
                                    break;
                                }

                                ////string cmd = "Update " + IAFD_Movies_TableName() + " Set [Quality] = 'DUPE' WHERE [URL] = '" + url + "'";
                                //bool updated = Flag_Dupe_URL(url);
                                //if (!updated) { ahk.MsgBox("Error Updating " + url); }
                                //else
                                //{
                                //    UpdatedURLs.Add(url);
                                //    flagged++;
                                //    if (button != null) { tel.Update(button, i + "/" + urls.Count + " | Flagged = " + flagged); }
                                //}

                                //commands.Add(cmd);

                                //if (commands.Count > 200)  // write to db when 200 entries collected
                                //{
                                //    if (button != null) { tel.Update(button, i + "/" + urls.Count + " | Writing SQL..."); }
                                //    int updated = sql.WriteSQL_List(Con, commands);
                                //    commands = new List<string>();
                                //    if (button != null) { tel.Update(button, i + "/" + urls.Count + " | Wrote : " + updated); }
                                //}
                                //else
                                //{
                                //    if (button != null) { tel.Update(button, i + "/" + urls.Count + " | ToUpdate: " + commands.Count); }
                                //}
                            }
                            else
                            {
                                if (button != null) { tel.Update(button, i + "/" + urls.Count + " | ToUpdate: " + commands.Count); }
                            }
                        }

                        return flagged;
                    }


                    return 0;
                }

                /// <summary>
                /// Returns list of all URLS from Db
                /// </summary>
                /// <param name="URL"></param>
                /// <returns></returns>
                public List<string> Movie_URLs(string URL = "")
                {
                    SqlConnection Con = IAFD_Movies_Conn();
                    //string cmd = "select [URL] FROM " + IAFD_Movies_TableName() + " WHERE [Quality] = ''";
                    string cmd = "select [URL] FROM " + IAFD_Movies_TableName();
                    if (URL != "") { cmd = "select [URL] FROM " + IAFD_Movies_TableName() + " WHERE URL = '" + URL + "'"; }
                    List<string> URLS = lst.SQL_To_List(Con, cmd);
                    return URLS;
                }


                public List<string> DupeMovieIDs(string URL)
                {
                    SqlConnection Con = IAFD_Movies_Conn();
                    string cmd = "select ID FROM " + IAFD_Movies_TableName() + " where [URL] = '" + URL + "'";
                    List<string> URLS = lst.SQL_To_List(Con, cmd);
                    return URLS;
                }

                public bool DeleteByID(string ID)
                {
                    SqlConnection Con = IAFD_Movies_Conn();
                    string cmd = "Delete FROM " + IAFD_Movies_TableName() + " WHERE [ID] = '" + ID + "'";
                    return sql.WriteDataRecord(Con, cmd);
                }

                public bool Flag_Dupe_URL(string URL)
                {
                    SqlConnection Con = IAFD_Movies_Conn();
                    string cmd = "Update " + IAFD_Movies_TableName() + " Set [Quality] = 'DUPE' WHERE [URL] = '" + URL + "'";
                    return sql.WriteDataRecord(Con, cmd);
                }

                /// <summary>
                /// Returns # of URLs Found in dB for URL
                /// </summary>
                /// <param name="URL"></param>
                /// <returns></returns>
                public int Movie_URL_Count(List<string> urls, string URL)
                {
                    int count = 0;
                    foreach (string url in urls)
                    {
                        if (url == URL) { count++; }
                    }

                    return count;
                }


                // Return IAFD_Movies SQL Connection String
                public SqlConnection IAFD_Movies_Conn()
                {
                    // populate sql connection
                    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMADBIndex"].ConnectionString);
                    return conn;
                }

                // Return IAFD_Movies TableName (Full Path)
                public string IAFD_Movies_TableName()
                {
                    // populate to return full sql table name
                    return "[ADBIndex].[dbo].[IAFD_Movies]";
                }

                // Generate SQL Table
                public bool IAFD_Movies_CreateSQLTable()
                {
                    SqlConnection Conn = IAFD_Movies_Conn();
                    string CreateTableLine = "CREATE TABLE [IAFD_Movies](";
                    CreateTableLine = CreateTableLine + "[ID] [int] IDENTITY(1,1) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[Title] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[AltTitle] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[Year] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[AllGirl] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[AllMale] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[Compilation] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[Studio] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[Director] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[Actors] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[Minutes] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[ReleaseDate] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[URL] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[FilePath] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[FileSize] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[Rating] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[Quality] [varchar](max) NOT NULL";
                    CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                    return false;
                }

                public bool IAFD_Movies_InsertSQL(IAFD_Movies obj)
                {
                    SqlConnection Con = IAFD_Movies_Conn();
                    string SQLLine = "Insert Into " + IAFD_Movies_TableName() + " (Flag, Title, AltTitle, Year, AllGirl, AllMale, Compilation, Studio, Director, Actors, Minutes, ReleaseDate, URL, FilePath, FileSize, Rating, Quality, FilmImageURL, FilmImageLocal) VALUES (@Flag, @Title, @AltTitle, @Year, @AllGirl, @AllMale, @Compilation, @Studio, @Director, @Actors, @Minutes, @ReleaseDate, @URL, @FilePath, @FileSize, @Rating, @Quality, @FilmImageURL, @FilmImageLocal)";
                    SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                    cmd2 = new SqlCommand(SQLLine, Con);

                    if (obj.Title == null) { obj.Title = ""; }
                    if (obj.AltTitle == null) { obj.AltTitle = ""; }
                    if (obj.Year == null) { obj.Year = ""; }
                    if (obj.AllGirl == null) { obj.AllGirl = ""; }
                    if (obj.AllMale == null) { obj.AllMale = ""; }
                    if (obj.Compilation == null) { obj.Compilation = ""; }
                    if (obj.Studio == null) { obj.Studio = ""; }
                    if (obj.Director == null) { obj.Director = ""; }
                    if (obj.Actors == null) { obj.Actors = ""; }
                    if (obj.Minutes == null) { obj.Minutes = ""; }
                    if (obj.ReleaseDate == null) { obj.ReleaseDate = ""; }
                    if (obj.URL == null) { obj.URL = ""; }
                    if (obj.FilePath == null) { obj.FilePath = ""; }
                    if (obj.FileSize == null) { obj.FileSize = ""; }
                    if (obj.Rating == null) { obj.Rating = ""; }
                    if (obj.Quality == null) { obj.Quality = ""; }
                    if (obj.FilmImageURL == null) { obj.FilmImageURL = ""; }
                    if (obj.FilmImageLocal == null) { obj.FilmImageLocal = ""; }

                    string year = ahk.Remove_Letters(obj.Year.ToString());
                    if (year.Trim() == "") { year = "0"; }

                    cmd2.Parameters.AddWithValue(@"Flag", obj.Flag.ToString());
                    cmd2.Parameters.AddWithValue(@"Title", obj.Title.ToString());
                    cmd2.Parameters.AddWithValue(@"AltTitle", obj.AltTitle.ToString());
                    cmd2.Parameters.AddWithValue(@"Year", year);
                    cmd2.Parameters.AddWithValue(@"AllGirl", obj.AllGirl.ToString());
                    cmd2.Parameters.AddWithValue(@"AllMale", obj.AllMale.ToString());
                    cmd2.Parameters.AddWithValue(@"Compilation", obj.Compilation.ToString());
                    cmd2.Parameters.AddWithValue(@"Studio", obj.Studio.ToString());
                    cmd2.Parameters.AddWithValue(@"Director", obj.Director.ToString());
                    cmd2.Parameters.AddWithValue(@"Actors", obj.Actors.ToString());
                    cmd2.Parameters.AddWithValue(@"Minutes", obj.Minutes.ToString());
                    cmd2.Parameters.AddWithValue(@"ReleaseDate", obj.ReleaseDate.ToString());
                    cmd2.Parameters.AddWithValue(@"URL", obj.URL.ToString());
                    cmd2.Parameters.AddWithValue(@"FilePath", obj.FilePath.ToString());
                    cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize.ToString());
                    cmd2.Parameters.AddWithValue(@"Rating", obj.Rating.ToString());
                    cmd2.Parameters.AddWithValue(@"Quality", obj.Quality.ToString());
                    cmd2.Parameters.AddWithValue(@"FilmImageURL", obj.FilmImageURL.ToString());
                    cmd2.Parameters.AddWithValue(@"FilmImageLocal", obj.FilmImageLocal.ToString());

                    if (Con.State == ConnectionState.Closed) { Con.Open(); }
                    int recordsAffected = 0;
                    try { recordsAffected = cmd2.ExecuteNonQuery(); }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 1205) // retry on deadlock
                        {
                            ahk.Sleep(1000);
                            IAFD_Movies_InsertSQL(obj);
                        }
                        else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                        {
                            ahk.Sleep(1000);
                            IAFD_Movies_InsertSQL(obj);
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

                public bool IAFD_Movies_UpdateSQL(IAFD_Movies obj)
                {
                    SqlConnection Conn = IAFD_Movies_Conn();
                    string SQLLine = "Update " + IAFD_Movies_TableName() + " SET Flag = @Flag, Title = @Title, AltTitle = @AltTitle, Year = @Year, AllGirl = @AllGirl, AllMale = @AllMale, Compilation = @Compilation, Studio = @Studio, Director = @Director, Actors = @Actors, Minutes = @Minutes, ReleaseDate = @ReleaseDate, URL = @URL, FilePath = @FilePath, FileSize = @FileSize, Rating = @Rating, Quality = @Quality, FilmImageURL = @FilmImageURL, FilmImageLocal = @FilmImageLocal WHERE URL = @URL";
                    SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                    cmd2 = new SqlCommand(SQLLine, Conn);

                    if (obj.Title == null) { obj.Title = ""; }
                    if (obj.AltTitle == null) { obj.AltTitle = ""; }
                    if (obj.Year == null) { obj.Year = ""; }
                    if (obj.AllGirl == null) { obj.AllGirl = ""; }
                    if (obj.AllMale == null) { obj.AllMale = ""; }
                    if (obj.Compilation == null) { obj.Compilation = ""; }
                    if (obj.Studio == null) { obj.Studio = ""; }
                    if (obj.Director == null) { obj.Director = ""; }
                    if (obj.Actors == null) { obj.Actors = ""; }
                    if (obj.Minutes == null) { obj.Minutes = ""; }
                    if (obj.ReleaseDate == null) { obj.ReleaseDate = ""; }
                    if (obj.URL == null) { obj.URL = ""; }
                    if (obj.FilePath == null) { obj.FilePath = ""; }
                    if (obj.FileSize == null) { obj.FileSize = ""; }
                    if (obj.Rating == null) { obj.Rating = ""; }
                    if (obj.Quality == null) { obj.Quality = ""; }
                    if (obj.FilmImageURL == null) { obj.FilmImageURL = ""; }
                    if (obj.FilmImageLocal == null) { obj.FilmImageLocal = ""; }

                    string year = ahk.Remove_Letters(obj.Year.ToString());
                    if (year.Trim() == "") { year = "0"; }

                    cmd2.Parameters.AddWithValue(@"Flag", obj.Flag.ToString());
                    cmd2.Parameters.AddWithValue(@"Title", obj.Title.ToString());
                    cmd2.Parameters.AddWithValue(@"AltTitle", obj.AltTitle.ToString());
                    cmd2.Parameters.AddWithValue(@"Year", year);
                    cmd2.Parameters.AddWithValue(@"AllGirl", obj.AllGirl.ToString());
                    cmd2.Parameters.AddWithValue(@"AllMale", obj.AllMale.ToString());
                    cmd2.Parameters.AddWithValue(@"Compilation", obj.Compilation.ToString());
                    cmd2.Parameters.AddWithValue(@"Studio", obj.Studio.ToString());
                    cmd2.Parameters.AddWithValue(@"Director", obj.Director.ToString());
                    cmd2.Parameters.AddWithValue(@"Actors", obj.Actors.ToString());
                    cmd2.Parameters.AddWithValue(@"Minutes", obj.Minutes.ToString());
                    cmd2.Parameters.AddWithValue(@"ReleaseDate", obj.ReleaseDate.ToString());
                    cmd2.Parameters.AddWithValue(@"URL", obj.URL.ToString());
                    cmd2.Parameters.AddWithValue(@"FilePath", obj.FilePath.ToString());
                    cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize.ToString());
                    cmd2.Parameters.AddWithValue(@"Rating", obj.Rating.ToString());
                    cmd2.Parameters.AddWithValue(@"Quality", obj.Quality.ToString());
                    cmd2.Parameters.AddWithValue(@"FilmImageURL", obj.FilmImageURL.ToString());
                    cmd2.Parameters.AddWithValue(@"FilmImageLocal", obj.FilmImageLocal.ToString());


                    if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                    int recordsAffected = 0;
                    try { recordsAffected = cmd2.ExecuteNonQuery(); }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 1205) // retry on deadlock
                        {
                            ahk.Sleep(1000);
                            IAFD_Movies_UpdateSQL(obj);
                        }
                        else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                        {
                            ahk.Sleep(1000);
                            IAFD_Movies_UpdateSQL(obj);
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

                public bool IAFD_Movies_UpdateInsert(IAFD_Movies obj)
                {
                    SqlConnection Conn = IAFD_Movies_Conn();
                    bool Updated = IAFD_Movies_UpdateSQL(obj);  // try to update record first
                    if (!Updated) { Updated = IAFD_Movies_InsertSQL(obj); }  // if unable to update, insert new record
                    return Updated;
                }




                public bool IAFD_Movies_FilmURLOnly_InsertSQL(string URL)
                {
                    SqlConnection Con = IAFD_Movies_Conn();
                    string SQLLine = "Insert Into " + IAFD_Movies_TableName() + " (URL) VALUES (@URL)";
                    SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                    cmd2 = new SqlCommand(SQLLine, Con);


                    cmd2.Parameters.AddWithValue(@"URL", URL);

                    if (Con.State == ConnectionState.Closed) { Con.Open(); }
                    int recordsAffected = 0;
                    try { recordsAffected = cmd2.ExecuteNonQuery(); }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 1205) // retry on deadlock
                        {
                            ahk.Sleep(1000);
                            IAFD_Movies_FilmURLOnly_InsertSQL(URL);
                        }
                        else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                        {
                            ahk.Sleep(1000);
                            IAFD_Movies_FilmURLOnly_InsertSQL(URL);
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

                public bool IAFD_Movies_FilmURLOnly_UpdateSQL(string URL)
                {
                    SqlConnection Conn = IAFD_Movies_Conn();
                    string SQLLine = "Update " + IAFD_Movies_TableName() + " SET URL = @URL WHERE URL = @URL";
                    SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                    cmd2 = new SqlCommand(SQLLine, Conn);

                    cmd2.Parameters.AddWithValue(@"URL", URL);

                    if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                    int recordsAffected = 0;
                    try { recordsAffected = cmd2.ExecuteNonQuery(); }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 1205) // retry on deadlock
                        {
                            ahk.Sleep(1000);
                            IAFD_Movies_FilmURLOnly_UpdateSQL(URL);
                        }
                        else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                        {
                            ahk.Sleep(1000);
                            IAFD_Movies_FilmURLOnly_UpdateSQL(URL);
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

                public bool IAFD_Movies_FilmURLOnly_UpdateInsert(string URL)
                {
                    SqlConnection Conn = IAFD_Movies_Conn();
                    bool Updated = IAFD_Movies_FilmURLOnly_UpdateSQL(URL);  // try to update record first
                    if (!Updated) { Updated = IAFD_Movies_FilmURLOnly_InsertSQL(URL); }  // if unable to update, insert new record
                    return Updated;
                }


                // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
                public bool IAFD_Movies_UpdateIfPopulated(IAFD_Movies obj, string ID = "")
                {
                    SqlConnection Conn = IAFD_Movies_Conn();
                    string SQLcmd = "Update " + IAFD_Movies_TableName() + " SET ";
                    if (obj.ID != null) { SQLcmd = SQLcmd + " ID = @ID,"; }
                    if (obj.Title != null) { SQLcmd = SQLcmd + " Title = @Title,"; }
                    if (obj.AltTitle != null) { SQLcmd = SQLcmd + " AltTitle = @AltTitle,"; }
                    if (obj.Year != null) { SQLcmd = SQLcmd + " Year = @Year,"; }
                    if (obj.AllGirl != null) { SQLcmd = SQLcmd + " AllGirl = @AllGirl,"; }
                    if (obj.AllMale != null) { SQLcmd = SQLcmd + " AllMale = @AllMale,"; }
                    if (obj.Compilation != null) { SQLcmd = SQLcmd + " Compilation = @Compilation,"; }
                    if (obj.Studio != null) { SQLcmd = SQLcmd + " Studio = @Studio,"; }
                    if (obj.Director != null) { SQLcmd = SQLcmd + " Director = @Director,"; }
                    if (obj.Actors != null) { SQLcmd = SQLcmd + " Actors = @Actors,"; }
                    if (obj.Minutes != null) { SQLcmd = SQLcmd + " Minutes = @Minutes,"; }
                    if (obj.ReleaseDate != null) { SQLcmd = SQLcmd + " ReleaseDate = @ReleaseDate,"; }
                    if (obj.URL != null) { SQLcmd = SQLcmd + " URL = @URL,"; }
                    if (obj.FilePath != null) { SQLcmd = SQLcmd + " FilePath = @FilePath,"; }
                    if (obj.FileSize != null) { SQLcmd = SQLcmd + " FileSize = @FileSize,"; }
                    if (obj.Rating != null) { SQLcmd = SQLcmd + " Rating = @Rating,"; }
                    if (obj.Quality != null) { SQLcmd = SQLcmd + " Quality = @Quality,"; }
                    SQLcmd = ahk.TrimLast(SQLcmd, 1);
                    SQLcmd = SQLcmd + " WHERE ID = @ID";

                    SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                    if (obj.ID != null) { cmd2.Parameters.AddWithValue(@"ID", obj.ID); }
                    if (obj.Title != null) { cmd2.Parameters.AddWithValue(@"Title", obj.Title); }
                    if (obj.AltTitle != null) { cmd2.Parameters.AddWithValue(@"AltTitle", obj.AltTitle); }
                    if (obj.Year != null) { cmd2.Parameters.AddWithValue(@"Year", obj.Year); }
                    if (obj.AllGirl != null) { cmd2.Parameters.AddWithValue(@"AllGirl", obj.AllGirl); }
                    if (obj.AllMale != null) { cmd2.Parameters.AddWithValue(@"AllMale", obj.AllMale); }
                    if (obj.Compilation != null) { cmd2.Parameters.AddWithValue(@"Compilation", obj.Compilation); }
                    if (obj.Studio != null) { cmd2.Parameters.AddWithValue(@"Studio", obj.Studio); }
                    if (obj.Director != null) { cmd2.Parameters.AddWithValue(@"Director", obj.Director); }
                    if (obj.Actors != null) { cmd2.Parameters.AddWithValue(@"Actors", obj.Actors); }
                    if (obj.Minutes != null) { cmd2.Parameters.AddWithValue(@"Minutes", obj.Minutes); }
                    if (obj.ReleaseDate != null) { cmd2.Parameters.AddWithValue(@"ReleaseDate", obj.ReleaseDate); }
                    if (obj.URL != null) { cmd2.Parameters.AddWithValue(@"URL", obj.URL); }
                    if (obj.FilePath != null) { cmd2.Parameters.AddWithValue(@"FilePath", obj.FilePath); }
                    if (obj.FileSize != null) { cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize); }
                    if (obj.Rating != null) { cmd2.Parameters.AddWithValue(@"Rating", obj.Rating); }
                    if (obj.Quality != null) { cmd2.Parameters.AddWithValue(@"Quality", obj.Quality); }

                    if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                    int recordsAffected = 0;
                    try { recordsAffected = cmd2.ExecuteNonQuery(); }
                    catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                    Conn.Close();
                    if (recordsAffected > 0) { return true; }
                    else return false;
                }

                /// <summary>
                /// Check to see if Movie Info Is Populated based on URL
                /// </summary>
                /// <param name="URL"></param>
                /// <returns></returns>
                public bool MovieInfoPopulated(string URL)
                {
                    bool InfoPopulated = false;
                    IAFD_Movies movie = IAFD_Movies_ReturnSQL(URL);
                    if (movie.Title != "" && movie.Title != null) { InfoPopulated = true; }
                    return InfoPopulated;
                }

                public IAFD_Movies IAFD_Movies_ReturnSQL(string URL)
                {
                    SqlConnection Conn = IAFD_Movies_Conn();
                    string SelectLine = "Select [ID],[Flag],[FilmImageURL],[FilmImageLocal],[Title],[AltTitle],[Year],[AllGirl],[AllMale],[Compilation],[Studio],[Director],[Actors],[Minutes],[ReleaseDate],[URL],[FilePath],[FileSize],[Rating],[Quality] From " + IAFD_Movies_TableName() + " WHERE [URL] = '" + URL + "'";
                    DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                    IAFD_Movies returnObject = new IAFD_Movies();
                    if (ReturnTable != null)
                    {
                        foreach (DataRow ret in ReturnTable.Rows)
                        {
                            returnObject.ID = ret["ID"].ToString();
                            returnObject.Flag = ret["Flag"].ToBool();
                            returnObject.Title = ret["Title"].ToString();
                            returnObject.AltTitle = ret["AltTitle"].ToString();
                            returnObject.Year = ret["Year"].ToString();
                            returnObject.AllGirl = ret["AllGirl"].ToString();
                            returnObject.AllMale = ret["AllMale"].ToString();
                            returnObject.Compilation = ret["Compilation"].ToString();
                            returnObject.Studio = ret["Studio"].ToString();
                            returnObject.Director = ret["Director"].ToString();
                            returnObject.Actors = ret["Actors"].ToString();
                            returnObject.Minutes = ret["Minutes"].ToString();
                            returnObject.ReleaseDate = ret["ReleaseDate"].ToString();
                            returnObject.URL = ret["URL"].ToString();
                            returnObject.FilePath = ret["FilePath"].ToString();
                            returnObject.FileSize = ret["FileSize"].ToString();
                            returnObject.Rating = ret["Rating"].ToString();
                            returnObject.Quality = ret["Quality"].ToString();
                            returnObject.FilmImageURL = ret["FilmImageURL"].ToString();
                            returnObject.FilmImageLocal = ret["FilmImageLocal"].ToString();

                            return returnObject;
                        }
                    }
                    return returnObject;
                }

                public List<IAFD_Movies> IAFD_Movies_ReturnSQLList(string Command = "")
                {
                    if (Command == "") { Command = "Select * From " + IAFD_Movies_TableName(); }
                    SqlConnection Conn = IAFD_Movies_Conn();
                    DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                    List<IAFD_Movies> ReturnList = new List<IAFD_Movies>();
                    if (ReturnTable != null)
                    {
                        foreach (DataRow ret in ReturnTable.Rows)
                        {
                            IAFD_Movies returnObject = new IAFD_Movies();
                            returnObject.ID = ret["ID"].ToString();
                            returnObject.Flag = ret["Flag"].ToBool();
                            returnObject.Title = ret["Title"].ToString();
                            returnObject.AltTitle = ret["AltTitle"].ToString();
                            returnObject.Year = ret["Year"].ToString();
                            returnObject.AllGirl = ret["AllGirl"].ToString();
                            returnObject.AllMale = ret["AllMale"].ToString();
                            returnObject.Compilation = ret["Compilation"].ToString();
                            returnObject.Studio = ret["Studio"].ToString();
                            returnObject.Director = ret["Director"].ToString();
                            returnObject.Actors = ret["Actors"].ToString();
                            returnObject.Minutes = ret["Minutes"].ToString();
                            returnObject.ReleaseDate = ret["ReleaseDate"].ToString();
                            returnObject.URL = ret["URL"].ToString();
                            returnObject.FilePath = ret["FilePath"].ToString();
                            returnObject.FileSize = ret["FileSize"].ToString();
                            returnObject.Rating = ret["Rating"].ToString();
                            returnObject.Quality = ret["Quality"].ToString();
                            returnObject.FilmImageURL = ret["FilmImageURL"].ToString();
                            returnObject.FilmImageLocal = ret["FilmImageLocal"].ToString();
                            ReturnList.Add(returnObject);
                        }
                    }
                    return ReturnList;
                }

                /// <summary>
                /// Returns List of Movie URLS from Db that are Flagged to Download More Info
                /// </summary>
                /// <returns></returns>
                public List<string> Return_Flagged_MovieURLs()
                {
                    string Command = "Select [URL] From " + IAFD_Movies_TableName() + " WHERE [Flag] = '1'";
                    SqlConnection Conn = IAFD_Movies_Conn();
                    DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                    List<string> ReturnList = new List<string>();
                    if (ReturnTable != null)
                    {
                        foreach (DataRow ret in ReturnTable.Rows)
                        {
                            ReturnList.Add(ret["URL"].ToString());
                        }
                    }
                    return ReturnList;
                }

                public bool IAFD_Movies_SQL_to_SQLite(string SqliteDBPath = @"\Db\IAFD_Movies.sqlite")
                {
                    string SaveFile = SqliteDBPath;
                    if (SqliteDBPath == @"\Db\IAFD_Movies.sqlite")
                    {
                        ahk.FileCreateDir(ahk.AppDir() + @"\Db");
                        SaveFile = ahk.AppDir() + @"\Db\IAFD_Movies.sqlite";
                    }

                    //sb.StatusBar("Copying SQL Db to " + SaveFile + "...");
                    //sqlite.SQLTable_To_NewSQLiteTable(IAFD_Movies_Conn(), "IAFD_Movies", "IAFD_Movies", SaveFile, "", false, false, false);
                    //sb.StatusBar("FINISHED Copying SQL Db to " + SaveFile);

                    if (File.Exists(SaveFile)) { return true; } else { return false; }
                }



                public List<IAFD_Movies> Duplicate_URLS(RadProgressBar Bar = null)
                {
                    string Command = "select distinct [URL],[ID] FROM " + IAFD_Movies_TableName() + " order by [URL]";
                    SqlConnection Conn = IAFD_Movies_Conn();
                    DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                    List<IAFD_Movies> ReturnList = new List<IAFD_Movies>();
                    if (ReturnTable != null)
                    {
                        foreach (DataRow ret in ReturnTable.Rows)
                        {
                            IAFD_Movies returnObject = new IAFD_Movies();
                            returnObject.ID = ret["ID"].ToString();
                            returnObject.URL = ret["URL"].ToString();
                            ReturnList.Add(returnObject);
                        }
                    }

                    List<string> URLS = new List<string>();
                    List<string> DeleteIDs = new List<string>();

                    _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();

                    if (Bar != null) { pro.SetupProgressBar(Bar, ReturnList.Count); }

                    int i = 0; int DeleteListCount = 0;
                    foreach (IAFD_Movies movie in ReturnList)
                    {
                        if (Bar != null) { i++; DeleteListCount = DeleteIDs.Count; pro.UpdateProgress(Bar, i + "/" + ReturnList.Count + " | DeleteCount: " + DeleteListCount); }

                        bool AlreadyInList = lst.InList(URLS, movie.URL);
                        if (!AlreadyInList) { URLS.Add(movie.URL); }
                        else
                        {
                            DeleteIDs.Add(movie.ID);
                        }
                    }


                    // loop through list of ids to delete from database, setup progress bar again, then delete dupes from DB
                    if (Bar != null) { pro.SetupProgressBar(Bar, DeleteIDs.Count); }
                    i = 0;

                    foreach (string id in DeleteIDs)
                    {
                        if (Bar != null) { i++; pro.UpdateProgress(Bar, "Deleted " + i + "/" + DeleteIDs.Count); }
                        bool deleted = DeleteByID(id);
                    }

                    return ReturnList;
                }



                #endregion

                #endregion






            }
        }
}

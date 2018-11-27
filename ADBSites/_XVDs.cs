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
        public class XVDs
        {
            _AHK ahk = new _AHK();
            _Lists lst = new _Lists();
            _Database.SQLite sqlite = new _Database.SQLite();
            _Database.SQL sql = new _Database.SQL();
            _Apps.Chrome cr = new _Apps.Chrome();

            #region === xVd ===

            // *** Returns # of Times Videos is found in Localdb using URL lookup ***
            public int xVdLocalCheck(string URL = "")
            {
                //// populate list of local file names to global var
                //Populate_DirToDb_FileList(false);

                // if user doesn't provide URL, grab current Chrome browser URL, add "http://" prefix if missing
                if (URL == "") { URL = cr.CurrentURL(@"http://"); }

                // download and parse xVd page, return videoName.format 
                string vidName = xVdPage(URL);

                // returns # of times filename found in local dir2db index
                return localVideoCount(vidName);
            }

            // extract name of video file from HTML line, return filename.format [xVd]
            public string ExtractVidName(string InText)
            {
                int pos = 0;
                string name = "";

                //ahk.MsgBox(InText); 

                do
                {
                    string check = ahk.StringSplit(InText, "/", pos);
                    if (check.Contains(".mp4")) { name = check; }
                    if (check.Contains(".flv")) { name = check; }
                    if (check.Contains(".mpg")) { name = check; }
                    if (check.Contains(".mpeg")) { name = check; }
                    if (check.Contains(".mkv")) { name = check; }
                    if (check.Contains(".avi")) { name = check; }
                    pos++;
                } while (name == "");

                name = ahk.StringSplit(name, "?", 0);

                //ahk.MsgBox(name);

                return name;
            }

            // take video name from website (already extracted), compare file name to local file index, see if file has been downloaded, return # of instances found in db
            public int localVideoCount(string vidName)
            {
                int foundCount = 0;

                // // populate list of local file names to global var
                // Populate_DirToDb_FileList(false);

                //// loop through each file in db and compare to vidName, count # of times file is located in db
                // foreach (_Database.dir2Db.FileIndex checkFile in driveCapture)
                // {
                //     if (checkFile.FileName.Trim() == vidName.Trim())
                //     {
                //         //ahk.MsgBox("Match Found for " + checkFile.FilePath);
                //         foundCount++;
                //     }
                // }

                return foundCount;
            }

            // download URL HTML to string, apply login credentials if applicable [xVd]
            public string xVdPage(string URL)
            {
                WebClient webClient = cr.loginInfo(URL);

                string pageHTML = webClient.DownloadString(URL);

                List<string> list = lst.Text_To_List(pageHTML, true, true, false);

                //foreach (string item in list)
                //{
                //    if (item.Contains(".mp4"))
                //    {
                //        //string update = txtDisplay.Text; update = update + Environment.NewLine + item; txtDisplay.Text = update; 
                //        txtDisplay.Text = txtDisplay.Text + Environment.NewLine + ExtractVidName(item);

                //        return ExtractVidName(item);
                //    }
                //}

                //ahk.MsgBox("done"); 
                //txtDisplay.Text = pageHTML; 
                return pageHTML;
            }

            // download URL HTML to string, extract link to page's hq mp4 download path
            public string xVdPage_Mp4Link_FromURL(string URL)
            {
                WebClient webClient = cr.loginInfo(URL);

                string pageHTML = webClient.DownloadString(URL);

                List<string> list = lst.Text_To_List(pageHTML, true, true, false);

                foreach (string item in list)
                {
                    if (item.Contains(".mp4"))
                    {
                        //string update = txtDisplay.Text; update = update + Environment.NewLine + item; txtDisplay.Text = update; 
                        //txtDisplay.Text = txtDisplay.Text + Environment.NewLine + ExtractVidName(item);

                        return ExtractVidName(item);
                    }
                }

                return "";
            }

            // parse xvids page html for mp4 link
            public string xVdPage_Mp4Link_FromHTML(string pageHTML)
            {
                List<string> list = lst.Text_To_List(pageHTML, true, true, false);

                foreach (string item in list)
                {
                    if (item.Contains(".mp4"))
                    {
                        //string update = txtDisplay.Text; update = update + Environment.NewLine + item; txtDisplay.Text = update; 
                        //txtDisplay.Text = txtDisplay.Text + Environment.NewLine + ExtractVidName(item);

                        //return ExtractVidName(item);
                        return item;
                    }
                }

                return "";
            }

            // parse xvids page html for mp4 link, returns name of xvideo file 
            public string xVdPage_VideoName_FromHTML(string pageHTML)
            {
                List<string> list = lst.Text_To_List(pageHTML, true, true, false);

                foreach (string item in list)
                {
                    if (item.Contains(".mp4"))
                    {
                        //string update = txtDisplay.Text; update = update + Environment.NewLine + item; txtDisplay.Text = update; 
                        //txtDisplay.Text = txtDisplay.Text + Environment.NewLine + ExtractVidName(item);

                        return ExtractVidName(item);
                    }
                }

                return "";
            }



            // download tag page to html and parse into tags
            public string xVd_TagPage(string URL = @"http://www.xvideos.com/tags")
            {
                WebClient webClient = cr.loginInfo(URL);

                string pageHTML = webClient.DownloadString(URL);

                List<string> list = lst.Text_To_List(pageHTML, true, true, false);

                foreach (string item in list)
                {
                    if (item.Contains("<a href=\"/tags"))
                    {
                        //string update = txtDisplay.Text; update = update + Environment.NewLine + item; txtDisplay.Text = update; 
                        //txtDisplay.Text = txtDisplay.Text + Environment.NewLine + ExtractVidName(item);

                        ahk.MsgBox(item);

                        //return ExtractVidName(item);
                    }
                }

                //ahk.MsgBox("done"); 
                //txtDisplay.Text = pageHTML; 
                return pageHTML;
            }


            // extract xvid keywords / description from downloaded html
            public string xVd_Keywords(string HTML)
            {
                //<meta name="keywords" content="xvideos,xvideos.com, x videos,x video,porn,video,videos,anal,teen,hardcore,blonde,creampie,handjob,young,gangbang,cute,bukkake,bukakke,gang-bang" />
                //<meta name="description" content="XVIDEOS Two young cute girls and 80 men free" />

                List<string> list = lst.Text_To_List(HTML, true, true, false);

                string KeyWordLine = "";
                foreach (string line in list) { if (line.Contains("<meta name=\"keywords\"")) { KeyWordLine = line; } }

                string DescriptionLine = "";
                foreach (string line in list) { if (line.Contains("<meta name=\"description\"")) { DescriptionLine = line; } }

                string keys = "";
                if (KeyWordLine != "")
                {
                    keys = ahk.StringSplit(KeyWordLine, "=", 2);
                    keys = ahk.StringSplit(keys, "/", 0);
                    keys = ahk.StringReplace(keys, "\"", "", "ALL");
                    //ahk.MsgBox(keys); 
                }

                return keys;
            }

            public string xVd_Description(string HTML)  // parses the description field from xvid page
            {
                //<meta name="keywords" content="xvideos,xvideos.com, x videos,x video,porn,video,videos,anal,teen,hardcore,blonde,creampie,handjob,young,gangbang,cute,bukkake,bukakke,gang-bang" />
                //<meta name="description" content="XVIDEOS Two young cute girls and 80 men free" />

                List<string> list = lst.Text_To_List(HTML, true, true, false);

                string KeyWordLine = "";
                foreach (string line in list) { if (line.Contains("<meta name=\"keywords\"")) { KeyWordLine = line; } }

                string DescriptionLine = "";
                foreach (string line in list) { if (line.Contains("<meta name=\"description\"")) { DescriptionLine = line; } }

                string descript = "";

                if (DescriptionLine != "")
                {
                    descript = ahk.StringSplit(DescriptionLine, "=", 2);
                    descript = ahk.StringSplit(descript, "/", 0);
                    descript = ahk.StringReplace(descript, "\"", "", "ALL");
                    //ahk.MsgBox(descript);
                }

                return descript;
            }

            public string xVd_Models(string HTML)  // parses the model names from xvid page
            {
                List<string> list = lst.Text_To_List(HTML, true, true, false);
                string KeyWordLine = "";
                foreach (string line in list) { if (line.Contains("Models in this video:")) { KeyWordLine = line; } }
                return KeyWordLine;
            }

            public string xVd_Duration(string HTML)  // parses the duration field from xvid page
            {
                List<string> list = lst.Text_To_List(HTML, true, true, false);
                string KeyWordLine = "";
                foreach (string line in list) { if (line.Contains("class=\"duration\">")) { KeyWordLine = line; } }
                return KeyWordLine;
            }


            //<!-- dispo - Wed, 25 Jan 17 02:50:43 +0000 Exist (set in array). Loaded ! Video exists and loaded. Video exists and OK. -->
            //<h2>Busty Shione Cooper fucking a rock hard Cock <span class="duration">- 10 min</span></h2>
            //<p class="video-metadata"><span class="uploader">Uploader: <a href="/profiles/cumlouder" class="hg">Cumlouder</a> .
            //<a id="video-subscribe" class="video-subscribe" style="display:none;"><span class="sub-btn"><span class="icon subscribe-feed"></span></span><span class="label">Subscribe</span><span class="nb-subscribers">44354</span></a></span><a class="mobile-show" id="show-all-tags">+</a><span class="video-tags"><em>Tagged:</em>&nbsp;<a href="/tags/hardcore" class="nu">hardcore</a>, <a href="/tags/european" class="nu">european</a>, <a href="/tags/brunette" class="nu">brunette</a>, <a href="/tags/busty" class="nu">busty</a>, <a href="/tags/beach" class="nu">beach</a>, <a href="/tags/huge-boobs" class="nu">huge-boobs</a>, <a href="/tags/big-dick" class="nu">big-dick</a>, <a href="/tags/oral-sex" class="nu">oral-sex</a>, <a href="/tags/natural-tits" class="nu">natural-tits</a>, <a href="/tags/vaginal-sex" class="nu">vaginal-sex</a>, <a href="/tags/" class="nu"><strong>more tags</strong></a>.</span></p><p class="video-metadata">
            //Models in this video: <a href="/profiles/shione-cooper" class="hg">Shione Cooper</a>, find more in our <a href="/pornstars">Pornstars index</a>.
            //</p>	


            private void btnParseIndexPage_Click(object sender, EventArgs e)
            {
                ////string URL = cr.CurrentURL(@"http://");

                //string tagBase = txtTagPage.Text;
                //int tagPageLast = ahk.ToInt(txtTagPageLastNum.Text);
                //int tagStartNum = ahk.ToInt(txtStartNum.Text);

                //ahk.FileCreateDir(ahk.AppDir() + @"\HTML\" + txtTagName.Text);

                //string LastHTML = "";

                //do
                //{
                //    string nextURL = tagBase + txtTagName.Text + "/" + tagStartNum + @"/";

                //    //ahk.Run();
                //    string HTML = xVdPage(nextURL);

                //    if (HTML == LastHTML) { break; }

                //    ahk.FileAppend(HTML, ahk.AppDir() + "\\HTML\\" + txtTagName.Text + "\\" + txtTagName.Text + "_" + tagStartNum.ToString() + ".txt", 2);

                //    LastHTML = HTML;

                //    ahk.Sleep(ahk.ToInt(txtDelay.Text));

                //    tagStartNum++;
                //} while (tagStartNum < tagPageLast);


                //// //List<string> LastNum = ahk.StringSplit_List(URL, @"/");
                ////foreach (string section in LastNum)
                ////{
                ////    ahk.MsgBox(section); 
                ////    string 
                ////}


                ////Parse_XVid_IndexPage(URL);
            }

            // Use either site URL or local file path with html, extract list of video page links from index page
            public List<string> Parse_XVid_IndexPage(string URL = "", string FilePath = "")
            {
                List<string> lines = new List<string>();

                if (URL != "") { lines = HTML_List(URL); }

                if (FilePath != "") { lines = lst.TextFile_To_List(FilePath, true, true, false); }

                List<string> parsedInfo = new List<string>();

                foreach (string line in lines)
                {
                    if (line.Contains("<div id=\"video_"))
                    {
                        List<string> items = ahk.StringSplit_List(line, "<");
                        foreach (string item in items)
                        {
                            if (item.Contains("a href=") && item.Contains("title="))
                            {
                                string mItems = ahk.StringSplit(item, ">", 0);
                                //ahk.MsgBox(mItems);
                                mItems = ahk.StringReplace(mItems, "a href=");
                                mItems = ahk.StringReplace(mItems, "title=");

                                List<string> itemS = ahk.StringSplit_List(mItems, "\"");
                                int i = 1;
                                string Link = "";
                                string Title = "";
                                foreach (string IteM in itemS)
                                {
                                    if (i == 1) { Link = @"http://xvideos.com" + IteM; }
                                    if (i == 2) { Title = IteM; }
                                    i++;
                                }

                                parsedInfo.Add(Link + "|" + Title);

                                //ahk.MsgBox("Link = " + Link + "\n\rTitle = " + Title);
                                //ahk.Run(Link); 
                            }
                        }
                    }

                }

                //<div id="video_9489" class="thumb-block "><div class="thumb-inside"><div class="thumb"><script>document.write(xv.thumbs.replaceThumbUrl('<a href="/video9489/anal_pov"><img src="http://img-hw.xvideos.com/videos/thumbs169/83/69/5a/83695a3cff257d9e9b5144f7a6a544c3/83695a3cff257d9e9b5144f7a6a544c3.15.jpg" id="pic_9489" onload="xv.thumbs.mgr.load(9489)" onerror="xv.thumbs.mgr.fail(9489)" /></a>'));</script></div></div><p><a href="/video9489/anal_pov" title="anal POV">anal POV</a></p><p class="metadata"><span class="bg"><strong>35 sec</strong> - 100% - <a href="/profiles/sexy-employee">Sexy Employee</a></span></p></div>	
                return parsedInfo;
            }


            // xvid site search button
            private void btnSearch_Click(object sender, EventArgs e)
            {
                string SearchTerm = "testing";

                XVidSiteSearch(SearchTerm);
            }

            // search xvid site for search term(s) 
            public void XVidSiteSearch(string SearchTerm)
            {
                SearchTerm = SearchTerm.Replace(" ", "+");
                ahk.Run("http://www.xvideos.com/?k=" + SearchTerm);
            }


            // download html from URL, return as line list
            private List<string> HTML_List(string URL)
            {
                WebClient webClient = cr.loginInfo(URL);

                string pageHTML = webClient.DownloadString(URL);

                List<string> list = lst.Text_To_List(pageHTML, true, true, false);

                return list;
            }

            public void grabCurrent_XVidPage()  // reads current chrome page, parses elements, updates gui
            {
                string HTML = Grab_Chrome_HTML();
                string mp4HTML = xVdPage_Mp4Link_FromHTML(HTML);
                string keyWords = xVd_Keywords(HTML);
                string description = xVd_Description(HTML);
                string vidName = ExtractVidName(mp4HTML);
                string models = xVd_Models(HTML);  // parses the model names from xvid page
                string duration = xVd_Duration(HTML);  // parses the duration field from xvid page

                // update gui with parsed values

                string mp4Link = MP4_Link_From_HTML(HTML);


                //// update display text field with additional content if already populated
                //string displayText = mp4Link;
                //if (txtDisplay.Text.Trim() != "") { txtDisplay.Text = txtDisplay.Text + Environment.NewLine + displayText; }
                //if (txtDisplay.Text.Trim() == "") { txtDisplay.Text = displayText; }

                //txtDuration.Text = duration;
                //txtModels.Text = models;
                //txtKeywords.Text = keyWords;
                //txtDescription.Text = description;
                //txtMp4Link.Text = vidName;


                //// check local sqlite db to see if file already downloaded

                ////int inCollectionCount = localVideoCount(vidName);
                ////txtFoundInDb.Text = inCollectionCount.ToString(); 

                ////// if this file isn't in the collection
                ////// save link to clipboard, JDownloader detects to add to queue
                ////bool ToDownload = false;
                ////if (inCollectionCount < 1) { ToDownload = true; ahk.ClipWrite(Links); }

                //// extract description/keywords for xvd page html
                ////xVd_Keywords(HTML);
            }

            private void btnGrabHTML_Click(object sender, EventArgs e)
            {
                grabCurrent_XVidPage();  // reads current chrome page, parses elements, updates gui
            }

            private string MP4_Link_From_HTML(string HTML)
            {
                List<string> links = HTML_LinkList(HTML, ".mp4");
                string mp4Link = "";
                foreach (string lnk in links) { mp4Link = lnk; } // the last mp4 link is the HQ link to mp4
                return mp4Link;
            }

            // extract list of links from html, option to only return links containing specific text
            private List<string> HTML_LinkList(string HTML, string LinkContains = "")
            {
                List<string> links = new List<string>();

                var linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                foreach (Match m in linkParser.Matches(HTML))
                {
                    if (LinkContains == "") { links.Add(m.Value); }  // add all links to list

                    if (LinkContains != "") { if (m.Value.Contains(LinkContains)) { links.Add(m.Value); } } // only add links with LinkContains text 
                }

                return links;
            }

            private string Grab_Chrome_HTML(string urL = "")  // returns HTML from current chrome browser tab
            {
                // grab current url from chrome if not provided
                if (urL == "") { urL = cr.CurrentURL(@"http://"); }

                WebClient webClient = cr.loginInfo(urL);

                string HTML = webClient.DownloadString(urL);
                //string HTML = webClient.DownloadString("http://www.warez-bb.org/viewforum.php?f=57&topicdays=0&start=600urL"); 
                //txtDisplay.Text = HTML;

                // extract list of links from HTML

                //string Links = "";
                //var linkParser = new Regex(@"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                ////var rawString = "house home go www.monstermmorpg.com nice hospital http://www.monstermmorpg.com this is incorrect url http://www.monstermmorpg.commerged continue";
                //foreach (Match m in linkParser.Matches(HTML))
                //    Links = Links + "\n\r" + m.Value; // extract list of all links

                //    if (m.Value.Contains(".mp4")) { Links = m.Value; }  // only extract last link on page with .mp4 in path 
                //if (m.Value.Contains(".mp4")) { Links = Links + "\n\r" + m.Value; }  // only add links with .mp4 in path 
                //Links = Links + "\n\r" + m.Value; // extract list of all links
                //MessageBox.Show(m.Value);

                //string vidName = ExtractVidName(Links);

                //string mp4 = xVdPage_Mp4Link_FromHTML(HTML);
                //txtDisplay.Text = mp4; 
                //return mp4; 

                //txtDisplay.Text = Links + " |  To Download = " + ToDownload.ToString();

                //txtDisplay.Text = ahk.UnHtml(HTML); 

                return HTML;
            }

            #endregion

        }
    }
}

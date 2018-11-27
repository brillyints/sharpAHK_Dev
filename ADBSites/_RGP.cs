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
using static sharpAHK_Dev._Sites.RapidGator;

namespace sharpAHK_Dev
{
    public partial class ADBSites
    {
        public class Rgp
        {
            _AHK ahk = new _AHK();
            _Database.SQL sql = new _Database.SQL();
            _Lists lst = new _Lists();
            _Parse prs = new _Parse();
            _TelerikLib tel = new _TelerikLib();
            _Web web = new _Web();
            _Sites.RapidGator rg = new _Sites.RapidGator();
            _Parse.XML xml = new _Parse.XML();

            #region === RGP SiteParse ===

            // http://rgporn.com/index.php?newsid=65366

            public void RPG_Loop(int StartNum = 1, int EndNum = 10)
            {
                bool online = false;

                do
                {
                    online = RGP(StartNum);

                    //if (online) { ahk.MsgBox(StartNum + " Online"); }

                    StartNum++;
                } while (StartNum < EndNum);

            }


            List<string> resultLines = new List<string>();
            public static int ResultNum { get; set; }

            public bool RGP(int PageNum = 1)
            {
                string url = "http://rgporn.com/index.php?newsid=" + PageNum;
                //tel.Update(txtURL, url);

                string html = web.DownloadHTML(url);
                if (html == "") { return false; }

                resultLines = xml.Parse_HTML_XML(html, "//body/div/div/div/div/table/tbody/tr/td/div/div/div/p");  // extract sections of text from html xml

                // extract tags
                string display = "TAGS: ";
                List<string> tags = new List<string>();
                string Tags = "";
                foreach (string line in resultLines)
                {
                    if (line.Contains("=tags"))
                    {
                        string tag = line.Replace("<a", "");
                        tag = tag.Replace("</a>", "");
                        tag = ahk.StringSplit(tag, ">", 1);
                        tag = tag.Trim();
                        tags.Add(tag);

                        display = display + "\n" + tag;

                        if (Tags != "") { Tags = Tags + "|" + tag; }
                        if (Tags == "") { Tags = tag; }
                    }
                }

                // extract title
                List<string> titleLines = xml.Parse_HTML_XML(html, "//body/div/div/div/div/table/tbody/tr/td/div/div/div");  // extract sections of text from html xml
                string title = lst.Return_List_Value(titleLines, 2);
                title = title.Replace("<h1 class=\"shead\">", "");
                title = title.Replace("</h1>", "");

                string imagehtml = "";
                foreach (string ima in titleLines)
                {
                    if (ima.Contains(".jpg")) { imagehtml = imagehtml + "\n" + ima; }
                }

                //ahk.MsgBox(title);

                List<string> links = rg.Regex_RGLinks(html);

                bool AllOnline = true;
                int onlineLinkCount = 0;
                string linkText = "";
                foreach (string link in links)
                {
                    RGInfo info = rg.RapidGatorCheckStatus(link);

                    if (!info.FileOnline) { AllOnline = false; }
                    else { onlineLinkCount++; }

                    string wlink = link + "|" + info.FileSize;

                    if (linkText != "") { linkText = linkText + "\n" + wlink; }
                    if (linkText == "") { linkText = wlink; }
                }

                string linksMsg = onlineLinkCount + "/" + links.Count + " Links Online";

                // Extract / Display Images From Page
                int downloaded = 0;
                if (AllOnline)
                {
                    string SaveDir = ahk.AppDir() + "\\Posts\\RGP\\" + ahk.AddLeadingZeros(PageNum, 5).ToString();
                    ahk.FileCreateDir(SaveDir);
                    ahk.Sleep(500);

                    ahk.FileAppend(Tags, SaveDir + "\\Tags.txt");
                    ahk.FileAppend(title, SaveDir + "\\Title.txt");
                    ahk.FileAppend(linkText, SaveDir + "\\Links.txt");

                    downloaded = DownloadImages(imagehtml, SaveDir, ahk.AddLeadingZeros(PageNum, 5).ToString());

                    //tel.Update(txtResults, "Title: " + title + "\n\nTags: " + Tags + "\n\n" + linksMsg);
                }


                //ahk.MsgBox("Found " + links.Count + " Links\nDownloaded " + downloaded.ToString() + " + Images");


                return AllOnline;
            }


            #endregion



            /// <summary>
            /// Download JPG Images From Parsed HTML Links
            /// </summary>
            /// <param name="html">HTML Containing URL Links to JPG Images</param>
            /// <param name="SaveRoot">Dir To Save Images To</param>
            /// <param name="BaseName">Base Save Name for New Images</param>
            /// <returns></returns>
            public int DownloadImages(string html, string SaveRoot = "\\Images", string BaseName = "Image")
            {
                if (SaveRoot == "\\Images") { SaveRoot = ahk.AppDir() + "\\Images"; }

                List<string> images = rg.JpgImageLinks(html);  // list of jpg images found on page
                string savename = "";

                int downloaded = 0;
                if (images != null && images.Count > 0)
                {
                    ahk.FileCreateDir(SaveRoot);

                    int i = 0;
                    foreach (string image in images)
                    {
                        savename = SaveRoot + "\\" + BaseName + ".jpg";

                        if (i > 0) { savename = SaveRoot + "\\" + BaseName + "_" + i.AddLeadingZeros(3) + ".jpg"; }
                        i++;

                        if (!File.Exists(savename)) { web.DownloadFile(image, savename, true); downloaded++; }
                    }
                }

                //ahk.MsgBox("Downloaded " + downloaded.ToString() + " + Images");
                return downloaded;
            }




        }
    }

}

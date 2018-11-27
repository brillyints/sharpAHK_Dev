using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sharpAHK;
using sharpAHK_Dev;
using AHKExpressions;
using TelerikExpressions;
using HtmlAgilityPack;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;

namespace sharpAHK_Dev
{
    public class _Parse
    {
        /// <summary>
        /// Loop through text, return list of strings containing text between start and end strings 
        /// </summary>
        /// <param name="filePath">FilePath containing text to parse</param>
        /// <param name="StartString">String indicating start of section to capture</param>
        /// <param name="EndString">String indicating end of section to capture</param>
        /// <returns>Returns list of sections captured in between start and end sections</returns>
        public List<string> CaptureSections(string FilePath_Or_String, string StartString, string EndString)
        {
            _Lists lst = new _Lists();

            List<string> Lines = new List<string>();

            if (File.Exists(FilePath_Or_String)) { Lines = lst.TextFile_To_List(FilePath_Or_String, true, false, false); }
            else
            {
                Lines = lst.Text_To_List(FilePath_Or_String, true, true, false);
            }

            List<string> inBetween = new List<string>();
            string between = "";

            // either make the start / end items match exactly or be found on the line
            bool IfLineContains = false;
            bool IfLineMatches = true;


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

            return inBetween;
        }


        /// <summary>
        /// Split string into multiple lines, with max # of characters per line, split by spaces. 
        /// </summary>
        /// <param name="InText">Text to parse into line segments</param>
        /// <param name="LineCharMax">Max number of characters per returned line</param>
        /// <param name="MaxLinesOut">Max number of lines to return. 0 = All lines</param>
        /// <param name="MsgBoxDisp">Optional message box to display output while testing. Default = False</param>
        /// <returns></returns>
        public List<string> ParseLongStringIntoSections(string InText, int LineCharMax = 66, int MaxLinesOut = 0, bool MsgBoxDisp = false)
        {
            _AHK ahk = new _AHK();

            // split InText into list, separated by spaces
            List<string> Parts = ahk.StringSplit_List(InText, " ");

            List<string> OutLines = new List<string>();

            string NextLine = "";
            string NextLinePreview = "";
            foreach (string part in Parts)
            {
                if (NextLine == "") { NextLine = part; NextLinePreview = part; continue; }

                if (NextLine.Length < LineCharMax)
                {
                    NextLinePreview = NextLinePreview + " " + part;

                    if (NextLinePreview.Length <= LineCharMax) { NextLine = NextLine + " " + part; }

                    if (NextLinePreview.Length > LineCharMax)
                    {
                        OutLines.Add(NextLine);
                        NextLine = part;
                        NextLinePreview = part;

                        // If value provided, only return X lines from InText Total 
                        if (MaxLinesOut != 0) { if (OutLines.Count == MaxLinesOut) { break; } }
                    }
                }
            }

            //===== Display Output Lines ======
            if (MsgBoxDisp)
            {
                int i = 1; foreach (string line in OutLines) { ahk.MsgBox(i + " - " + line); i++; }
            }

            return OutLines;
        }


        /// <summary>
        /// Extract Season/EpNum Format from FilePath (or string) and Return S00E00 Format
        /// </summary>
        /// <param name="FilePath">FilePath or String Containing Season/EpNum Info to Parse</param>
        /// <param name="RenameFile">Option to Replace Unformatted Season/EpNum with Standard S00E00 Naming Convention</param>
        /// <returns></returns>
        public string SeasonEpNums(string FilePath, bool RenameFile = false)
        {
            _AHK ahk = new _AHK();

            bool msgDisp = false;
            string FileNameNoExt = "";
            bool FileFound = false;
            string dir = "";
            string ext = "";


            if (RenameFile)
            {
                if (File.Exists(FilePath))
                {
                    FileNameNoExt = ahk.FileNameNoExt(FilePath);
                    ext = ahk.FileExt(FilePath);
                    dir = ahk.FileDir(FilePath);
                    FileFound = true;
                }
            }
            else
            {
                if (FilePath.Contains(":"))
                {
                    FilePath = FilePath.Replace(":", "-");
                    FileNameNoExt = ahk.FileNameNoExt(FilePath);
                }
                else { FileNameNoExt = FilePath; }
            }

            FileNameNoExt = FileNameNoExt.Replace("x264", "");
            FileNameNoExt = FileNameNoExt.Replace("x265", "");
            FileNameNoExt = FileNameNoExt.Replace("720p", "");
            FileNameNoExt = FileNameNoExt.Replace("1080p", "");
            FileNameNoExt = FileNameNoExt.ToUpper();

            // nothing to do, name already matches format
            Regex regex = new Regex(@"S\d{2}E\d{2}");  // hawaii.five-0.2010.S08E01
            Match match = regex.Match(FileNameNoExt);
            if (match.Success)
            {
                //if (msgDisp) { ahk.MsgBox(match.Value); }
                return match.Value;
            }


            // find 3 digit season/ep format, rename file to S00E00 Format
            regex = new Regex(@"(\.|_)\d{3}(\.|_)");   // "hawaii.five-0.2010.805.hdtv-lol"
            match = regex.Match(FileNameNoExt);
            if (match.Success)
            {
                string MatchText = match.Value;
                string seasonEp = match.Value.Replace(".", "");
                seasonEp = match.Value.Replace("_", "");

                string season = ahk.FirstCharacters(seasonEp, 1);
                string ep = ahk.LastCharacters(seasonEp, 2);
                seasonEp = "S0" + season + "E" + ep;

                if (FileFound && RenameFile)
                {
                    FileNameNoExt = FileNameNoExt.Replace(MatchText, "." + seasonEp + ".");
                    string newName = dir + "\\" + FileNameNoExt + ext;

                    bool renamed = ahk.FileRename(FilePath, newName);

                    if (msgDisp) { ahk.MsgBox("Renamed = " + renamed.ToString() + "\n\n" + FilePath + "\n\n" + newName); }
                }

                return seasonEp;
            }


            // find 3 digit season/ep format, rename file to S00E00 Format
            regex = new Regex(@"\.\d{1,2}X\d{2}(\.|_)");  // .4x23. OR .4x23_
            match = regex.Match(FileNameNoExt);
            if (match.Success)
            {
                string MatchText = match.Value;
                string seasonEp = match.Value.Replace(".", "");
                seasonEp = match.Value.Replace("_", "");

                string season = ahk.FirstCharacters(seasonEp, 1);
                string ep = ahk.LastCharacters(seasonEp, 2);
                seasonEp = "S0" + season + "E" + ep;

                if (FileFound && RenameFile)
                {
                    FileNameNoExt = FileNameNoExt.Replace(MatchText, "." + seasonEp + ".");
                    string newName = dir + "\\" + FileNameNoExt + ext;

                    bool renamed = false;

                    renamed = ahk.FileRename(FilePath, newName);

                    if (msgDisp) { ahk.MsgBox("Renamed = " + renamed.ToString() + "\n\n" + FilePath + "\n\n" + newName); }
                }

                return seasonEp;
            }


            regex = new Regex(@"\.\d{1-2}X\d{1-2}\.");   // 7th_heaven.6x02.teased.dvdrip_xvid-fov
            match = regex.Match(FileNameNoExt);
            if (match.Success)
            {
                string seasonEp = match.Value.Replace(".", "");
                string season = ahk.FirstCharacters(seasonEp, 1);
                string ep = ahk.LastCharacters(seasonEp, 2);
                seasonEp = "S0" + season + "E" + ep.AddLeadingZeros(2);
                if (msgDisp) { ahk.MsgBox(seasonEp); }
                return seasonEp;
            }

            regex = new Regex(@"\d{4}.\d{2}.d{2}");   // conan.2018.01.18.gerard.butler.720p.web.x264 - tbs
            match = regex.Match(FileNameNoExt);
            if (match.Success)
            {
                if (msgDisp) { ahk.MsgBox(match.Value); }

                //string seasonEp = match.Value.Replace(".", "");
                string seasonEp = match.Value;
                return seasonEp;
            }

            regex = new Regex(@"\d{4}\s\d{2}\s\d{2}");   // conan 2018 01 18 gerard butler 720p web x264 - tbs
            match = regex.Match(FileNameNoExt);
            if (match.Success)
            {
                if (msgDisp) { ahk.MsgBox(match.Value); }
                return match.Value;
            }


            regex = new Regex(@"\d{4}");   // hap.and.leonard.0304-yestv
            match = regex.Match(FileNameNoExt);
            if (match.Success)
            {
                string seasonEp = match.Value.Replace(".", "");
                string season = ahk.FirstCharacters(seasonEp, 2);
                string ep = ahk.LastCharacters(seasonEp, 2);
                seasonEp = "S" + season + "E" + ep.AddLeadingZeros(2);
                if (msgDisp) { ahk.MsgBox(seasonEp); }
                return seasonEp;
            }



            return "";
        }

        /// <summary>
        /// Extracts IMDb ID from HTML Page or from URL Provided
        /// </summary>
        /// <param name="Text">HTML/Text Containing IMDb URL</param>
        /// <returns></returns>
        public string Extract_IMDbID(string Text)  // ex: show s01 title
        {
            Regex regex = new Regex(@"imdb.com/title/tt\d{7}");   // href="http://www.imdb.com/title/tt5727282/"
            Match match = regex.Match(Text);
            if (match.Success)
            {
                string id = match.Value.Trim();
                id = id.Replace("imdb.com/title/", "");
                return id;
            }

            regex = new Regex(@"tt\d{7}");   // href="http://www.imdb.com/title/tt5727282/"
            match = regex.Match(Text);
            if (match.Success)
            {
                string id = match.Value.Trim();
                id = id.Replace("imdb.com/title/", "");
                return id;
            }

            return "";
        }

        public string ExtractSeason(string Text)  // ex: show s01 title
        {
            _AHK ahk = new _AHK();

            // nothing to do, name already matches format
            Regex regex = new Regex(@"S\d{2} ");  // 4 digit year
            Match match = regex.Match(Text);
            if (match.Success)
            {
                //if (msgDisp) { ahk.MsgBox(match.Value); }
                return match.Value;
            }


            regex = new Regex(@"\d{1-2}x\d{1-2}");   // 7th_heaven 6x02 teased.dvdrip_xvid-fov
            match = regex.Match(Text);
            if (match.Success)
            {
                string seasonEp = match.Value.Trim();
                string season = ahk.FirstCharacters(seasonEp, 1);
                string ep = ahk.LastCharacters(seasonEp, 2);
                seasonEp = "S0" + season + "E" + ep.AddLeadingZeros(2);
                return seasonEp;
            }

            return "";
        }

        public string ExtractYear(string Text)
        {
            // nothing to do, name already matches format
            Regex regex = new Regex(@"\d{4}");  // 4 digit year
            Match match = regex.Match(Text);
            if (match.Success)
            {
                //if (msgDisp) { ahk.MsgBox(match.Value); }
                return match.Value;
            }

            return "";
        }

        /// <summary>
        /// Extract Date From File Name - Accounts for Multiple Date Formats
        /// </summary>
        /// <param name="Text"></param>
        /// <returns></returns>
        public string ExtractDate(string Text)
        {
            // regex search for "year month day" date convention
            Regex regex = new Regex(@"\d{4}.\d{2}.\d{2}");  // 2018 01 17
            Match match = regex.Match(Text);
            if (match.Success)
            {
                return match.Value;
            }

            // if no match, try "month day year" format next
            regex = new Regex(@"\d{2}.\d{2}.\d{4}");  // 01 17 2018
            match = regex.Match(Text);
            if (match.Success)
            {
                return match.Value;
            }


            return "";
        }


        /// <summary>
        /// Returns Text Between Two <XML>Tags</XML>
        /// </summary>
        /// <param name="Text">String Containg XML Content with Tag</param>
        /// <param name="Tag">XML Tag to Return Text From</param>
        /// <returns></returns>
        public string XML_Text(string Text, string Tag = "<h5>")
        {
            if (!Tag.Contains("<") && !Tag.Contains(">")) { Tag = "<" + Tag + ">"; }
            string end = Tag.Replace("<", "</");

            int anstart = (Text.ToString().IndexOf(Tag) + Tag.Length);
            int anend = (Text.ToString().IndexOf(end));
            int anlen = 0;
            if (anstart > 0 && anend > 0)
            {
                anlen = anend - anstart;
            }

            string Asset = Text.ToString().Substring(anstart, anlen);
            return Asset;
        }



        /// <summary>
        /// Regex Jpg Image Links from HTML to List
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public List<string> JpgImageLinks(string HTML)
        {
            List<string> matches = new List<string>();

            string cmd = @"(http|ftp|https)://([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?";

            Regex ItemRegex = new Regex(cmd, RegexOptions.IgnoreCase);
            foreach (Match ItemMatch in ItemRegex.Matches(HTML))
            {
                //Console.WriteLine(ItemMatch);
                string val = ItemMatch.Value;

                if (val.ToUpper().Contains(".JPG") || val.ToUpper().Contains(".JPEG"))
                {
                    //ahk.MsgBox(val); 
                    matches.Add(val);
                }
            }
            return matches;
        }

        /// <summary>
        /// Parse Standard URL For FileName at End of Path
        /// </summary>
        /// <param name="link">URL to Parse</param>
        /// <returns>Returns Last Section of URL - Usually File Name</returns>
        public string LinkFileName(string link)
        {
            _AHK ahk = new _AHK();
            return ahk.StringSplit(link, "/", 0, true);
        }

        /// <summary>
        /// Parse HTML for List of RapidGator Links
        /// </summary>
        /// <param name="HTML">Text/HTML to Parse for Links</param>
        /// <returns>Returns List of RapidGator Links</returns>
        public List<string> Regex_RGLinks(string HTML)
        {
            List<string> matches = new List<string>();

            string cmd = @"(http|ftp|https)://([\w_-]+(?:(?:\.[\w_-]+)+))([\w.,@?^=%&:/~+#-]*[\w@?^=%&/~+#-])?";

            Regex ItemRegex = new Regex(cmd, RegexOptions.IgnoreCase);
            foreach (Match ItemMatch in ItemRegex.Matches(HTML))
            {
                //Console.WriteLine(ItemMatch);
                string val = ItemMatch.Value;

                if (val.ToUpper().Contains("RAPIDGATOR.NET")) { matches.Add(val); }
                if (val.ToUpper().Contains("RG.NET")) { matches.Add(val); }
                if (val.ToUpper().Contains("SAFELINKING.NET")) { matches.Add(val); }
            }


            return matches;
        }



        #region === XML Lib ===

        // RadCodeLib.cs  has XML Parse with paths example

        public class XML
        {

            // Helper method for getting inner text of named XML element (From File or XML String)
            public string XML_String_GetValue(string XML_String, string name, string defaultValue = "")
            {
                try
                {
                    XmlDocument docXML = new XmlDocument();
                    docXML.Load(XML_String);
                    XmlElement elValue = docXML.DocumentElement.SelectSingleNode(name) as XmlElement;
                    return (elValue == null) ? defaultValue : elValue.InnerText;
                }
                catch
                {
                    return defaultValue;
                }
            }

            // Helper method for getting inner text of named XML element (From File or XML String)
            public string XML_File_GetValue(string XMLPath, string name, string defaultValue = "")
            {
                if (!File.Exists(XMLPath))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(XMLPath);

                    XmlElement elValue = doc.DocumentElement.SelectSingleNode(name) as XmlElement;
                    return (elValue == null) ? defaultValue : elValue.InnerText;
                    //return defaultValue;
                }

                return "";
            }

            // Helper method to set inner text of named element.  Creates document if it doesn't exist
            static public void XMLSetValue(string XML_FilePath, string name, string stringValue)
            {
                XmlDocument docXML = new XmlDocument();
                XmlElement elRoot = null;
                if (!File.Exists(XML_FilePath))
                {
                    elRoot = docXML.CreateElement("root");
                    docXML.AppendChild(elRoot);
                }
                else
                {
                    docXML.Load(XML_FilePath);
                    elRoot = docXML.DocumentElement;
                }
                XmlElement value = docXML.DocumentElement.SelectSingleNode(name) as XmlElement;
                if (value == null)
                {
                    value = docXML.CreateElement(name);
                    elRoot.AppendChild(value);
                }
                value.InnerText = stringValue;
                docXML.Save(XML_FilePath);
            }


            // WORKS

            /// <summary>
            /// Returns Text Between Two <XML>Tags</XML>
            /// </summary>
            /// <param name="Text">String Containg XML Content with Tag</param>
            /// <param name="Tag">XML Tag to Return Text From</param>
            /// <returns></returns>
            public string XML_Text(string Text, string Tag = "<h5>")
            {
                if (!Tag.Contains("<") && !Tag.Contains(">")) { Tag = "<" + Tag + ">"; }
                string end = Tag.Replace("<", "</");

                int anstart = (Text.ToString().IndexOf(Tag) + Tag.Length);
                int anend = (Text.ToString().IndexOf(end));
                int anlen = 0;
                if (anstart > 0 && anend > 0)
                {
                    anlen = anend - anstart;
                }

                string Asset = Text.ToString().Substring(anstart, anlen);
                return Asset;
            }



            // provide either downloaded HTML or URL to download html, plus XML path to extract, and return list of found entries for xml path
            // xmlPath = "//body/div/div/div/div/h2"
            public List<string> Parse_HTML_XML(string html = "", string xmlPath = "//body/div/div/div/div/h2", string url = "")
            {
                _AHK ahk = new _AHK();

                string HTML = html;

                if (url != "") { HTML = ahk.Download_HTML(url); }

                //string xmlPath = "//body/div/div/div/div/h2";
                List<string> lines = new List<string>();

                // Parse HTML by XML Path, Create List of content inside Path
                var htmlDoc = new HtmlAgilityPack.HtmlDocument();
                htmlDoc.LoadHtml(HTML);

                try
                {
                    var htmlNodes = htmlDoc.DocumentNode.SelectNodes(xmlPath);
                    if (htmlNodes != null)
                    {
                        foreach (HtmlNode node in htmlNodes)
                        {
                            HtmlNodeCollection childNodes = node.ChildNodes;

                            foreach (var nd in childNodes)
                            {
                                if (nd.NodeType == HtmlNodeType.Element)
                                {
                                    lines.Add(nd.OuterHtml);
                                    Console.WriteLine(nd.OuterHtml);
                                }
                            }
                        }
                    }
                }
                catch { }



                return lines;
            }




        }

        #endregion


    }
}

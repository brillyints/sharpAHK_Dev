using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;
using sharpAHK;

namespace sharpAHK_Dev
{
    public partial class _Apps
    {

        /// <summary>
        /// Collection of ShortCuts / Interactions with Google Chrome WebBrowser
        /// </summary>
        public class Chrome
        {
            private static _AHK ahk = new sharpAHK._AHK();
            private static _Lists lst = new _Lists();

            #region === Downloads ===

            /// <summary> returns # of files currently downloading in Chrome</summary>
            /// <param name="DownloadDir"> </param>
            public int Current_Download_Count(string DownloadDir = "")
            {
                if (DownloadDir == "") { DownloadDir = @"C:\Users\" + ahk.UserName() + "\\Downloads"; }
                List<string> DLFiles = lst.FileList(DownloadDir, "*.crdownload", false, false, false);
                return DLFiles.Count;
            }

            /// <summary> returns # of files currently downloading in Chrome that appear to have already downloaded</summary>
            /// <param name="DownloadDir"> </param>
            public int Current_Download_DUPLICATE_Count(string DownloadDir = "")
            {
                if (DownloadDir == "") { DownloadDir = @"C:\Users\" + ahk.UserName() + "\\Downloads"; }
                List<string> DLFiles = lst.FileList(DownloadDir, "*(1)*.crdownload", false, false, false);
                return DLFiles.Count;
            }

            #endregion

            #region === Actions ===

            /// <summary>
            /// Returns the URL From the Current / Most Recent Chrome Browser's Active Display Tab 
            /// </summary>
            /// <returns>Returns string with Chrome's Current URL</returns>
            public string ActiveTabURL()  // 
            {
                // v1 - works but slowish

                Process[] procsChrome = Process.GetProcessesByName("chrome");

                if (procsChrome.Length <= 0)
                    return null;

                foreach (Process proc in procsChrome)
                {
                    // the chrome process must have a window 
                    if (proc.MainWindowHandle == IntPtr.Zero)
                        continue;

                    // to find the tabs we first need to locate something reliable - the 'New Tab' button 
                    AutomationElement root = AutomationElement.FromHandle(proc.MainWindowHandle);
                    var SearchBar = root.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Address and search bar"));
                    if (SearchBar != null)
                        return (string)SearchBar.GetCurrentPropertyValue(ValuePatternIdentifiers.ValueProperty);
                }


                // v2 - 2nd attempt if first fails to return URL

                foreach (Process chrome in procsChrome)
                {
                    if (chrome.MainWindowHandle == IntPtr.Zero)
                        continue;

                    AutomationElement element = AutomationElement.FromHandle(chrome.MainWindowHandle);
                    if (element == null)
                        return null;
                    System.Windows.Automation.Condition conditions = new AndCondition(
                        new PropertyCondition(AutomationElement.ProcessIdProperty, chrome.Id),
                        new PropertyCondition(AutomationElement.IsControlElementProperty, true),
                        new PropertyCondition(AutomationElement.IsContentElementProperty, true),
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Edit));

                    AutomationElement elementx = element.FindFirst(TreeScope.Descendants, conditions);
                    return ((ValuePattern)elementx.GetCurrentPattern(ValuePattern.Pattern)).Current.Value as string;
                }

                return null;
            }

            /// <summary>
            /// Returns the URL From the Current / Most Recent Chrome Browser's Active Display Tab 
            /// </summary>
            /// <param name="returnPrefix">Option to add 'Http://' prefix to WWW links when Missing/Needed</param>
            /// <returns></returns>
            public string CurrentURL(string returnPrefix = @"http://")
            {
                string URL = ActiveTabURL();

                if (returnPrefix != "") { if (!URL.Contains(returnPrefix)) { URL = returnPrefix + URL; } }

                return URL;
            }

            // closes current tab in most recently touched chrome window
            public void CloseTab(string winTitle = "ahk_class Chrome_WidgetWin_1")
            {
                Activate(winTitle); // unminimize and activate window
                ahk.SendInput("^w");  // shortcut to close current tab
            }

            // unminimizes (if minimized) + activate chrome window
            public void Activate(string winTitle = "ahk_class Chrome_WidgetWin_1") // unminimize and activate window
            {
                bool Minimized = ahk.IsMinimized(winTitle);
                if (Minimized) { ahk.WinRestore(winTitle); }

                ahk.WinActivate(winTitle);
            }

            // opens download tab in chrome
            public void DownloadTab(string winTitle = "ahk_class Chrome_WidgetWin_1")
            {
                Activate(winTitle); // unminimize and activate window
                ahk.WinWaitActive(winTitle);
                ahk.SendInput("^j");  // shortcut to open download tab
            }

            // aka "Back" button on browser, go to current tab's previous page
            public void PreviousPage(string winTitle = "ahk_class Chrome_WidgetWin_1")
            {
                Activate(winTitle); // unminimize and activate window
                ahk.WinWaitActive(winTitle);
                ahk.SendInput("!{Left}");  // go back a page 
            }

            // aka "Forward" button on browser, go to current tab's next page
            public void NextPage(string winTitle = "ahk_class Chrome_WidgetWin_1")
            {
                Activate(winTitle); // unminimize and activate window
                ahk.WinWaitActive(winTitle);
                ahk.SendInput("!{Right}");  // next web page
            }

            // open new tab in current chrome browser
            public void NewTab(string winTitle = "ahk_class Chrome_WidgetWin_1")
            {
                Activate(winTitle); // unminimize and activate window
                ahk.WinWaitActive(winTitle);
                ahk.SendInput("^t");  // open new tab
            }

            // reloads current page on most recent chrome browser
            public void ReloadTab(string winTitle = "ahk_class Chrome_WidgetWin_1")
            {
                Activate(winTitle); // unminimize and activate window
                ahk.WinWaitActive(winTitle);
                ahk.SendInput("^r");  // reload current tab
            }

            // opens 'save page as' prompt in chrome 
            public void SavePagePrompt(string winTitle = "ahk_class Chrome_WidgetWin_1")
            {
                Activate(winTitle); // unminimize and activate window
                ahk.WinWaitActive(winTitle);
                ahk.SendInput("^s");  // save current page (prompt)
            }

            /// <summary>
            /// Open Chrome Save As Prompt + Enter SavePath
            /// </summary>
            /// <param name="SavePath">Enters SavePath - If Not Provided Will Use Default Save Name in Chrome and Press Enter</param>
            /// <param name="winTitle"></param>
            public void SavePageHTML(string SavePath = "", string winTitle = "ahk_class Chrome_WidgetWin_1")
            {
                SavePagePrompt(winTitle);

                ahk.WinWait("Save As");
                ahk.WinActivate("Save As");
                ahk.WinWaitActive("Save As");

                ahk.Sleep(1000);
                //ahk.Send("NewPage");

                if (SavePath != "") { ahk.SendInput(SavePath); ahk.Sleep(200); }

                ahk.SendInput("{Enter}");
            }


            public void NextTab(string winTitle = "ahk_class Chrome_WidgetWin_1")
            {
                Activate(winTitle); // unminimize and activate window
                ahk.WinWaitActive(winTitle);
                ahk.SendInput("^{Tab}");
            }
            public void PreviousTab(string winTitle = "ahk_class Chrome_WidgetWin_1")
            {
                Activate(winTitle); // unminimize and activate window
                ahk.WinWaitActive(winTitle);
                ahk.SendInput("^{PgUp}");
            }
            public void TabNum(int TabNum, string winTitle = "ahk_class Chrome_WidgetWin_1")
            {
                Activate(winTitle); // unminimize and activate window
                ahk.WinWaitActive(winTitle);
                ahk.SendInput("^" + TabNum);
            }
            public void NewWindow(string winTitle = "ahk_class Chrome_WidgetWin_1")
            {
                Activate(winTitle); // unminimize and activate window
                ahk.WinWaitActive(winTitle);
                ahk.SendInput("^N");
            }
            public void Home(string winTitle = "ahk_class Chrome_WidgetWin_1")
            {
                Activate(winTitle); // unminimize and activate window
                ahk.WinWaitActive(winTitle);
                ahk.SendInput("{Alt Down}{Home}{Alt Up}");
            }

            #endregion

            #region === Download HTML ===

            // download URL HTML, parse into List by new line, return HTML string list to parse
            public List<string> List_dlHTML(string URL)
            {
                // based on URL, return l/p to allow downloading on password protected sites
                WebClient wbClient = loginInfo(URL);

                string pageHTML = wbClient.DownloadString(URL);

                List<string> list = lst.Text_To_List(pageHTML, true, true, false);

                return list;
            }

            // download URL HTML, return string with contents of webpage
            public string dlHTML(string URL)
            {
                // based on URL, return l/p to allow downloading on password protected sites
                WebClient wbClient = loginInfo(URL);

                string pageHTML = "";

                try
                {
                    pageHTML = wbClient.DownloadString(URL);
                }
                catch
                { }

                //List<string> list = lst.Text_To_List(pageHTML, true, true, false);

                return pageHTML;
            }

            // based on URL, return l/p to allow downloading on password protected sites
            public WebClient loginInfo(string URL)
            {
                WebClient webClient = new WebClient();

                bool found = false;

                if (URL.Contains("SiteNameHERE.com")) { found = true; webClient.Credentials = new System.Net.NetworkCredential("UserNameHERE", "PassWordHere", "www.SiteNameHERE.com"); return webClient; }

                if (!found) { webClient.Credentials = new System.Net.NetworkCredential("", "", ""); }

                return webClient;
            }

            #endregion

        }


    }
}

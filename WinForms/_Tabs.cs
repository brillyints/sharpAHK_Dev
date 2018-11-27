using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sharpAHK;
using sharpAHK_Dev;
using AHKExpressions;
using TelerikExpressions;
using System.Windows.Forms;
using System.Threading;
using ScintillaNET;

namespace sharpAHK_Dev
{
    public class _TabControl
    {
        #region === Instructions ===

        //### New Tab Control Setup Instructions ############
        //####### Save and Load Previous Tab on Startup #####

        //  -Set each TabControl SelectedIndexChanged action to:
        //       tab.tabControl_SelectedIndexChanged(sender, e);

        //  -In Load Function, have each tab control execute:
        //      tab.Load_Last_Tab(tabControl1;



        #endregion

        #region === Event Handlers ===

        // selected index changed on tab control - save value to load on next startup
        public void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl TC = (TabControl)sender;
            Save_Last_Tab(TC);
        }

        #endregion

        #region === Tab Actions ===

        /// <summary>Select TabPage by Index Number</summary>
        /// <param name="tc">TabControl Name</param>
        /// <param name="IndexNum">Index (starting with 0) Position</param>
        public void Select_Tab(TabControl tc, int IndexNum)
        {
            // select tab (from any thread)
            if (tc.InvokeRequired) { tc.BeginInvoke((MethodInvoker)delegate () { tc.SelectedIndex = IndexNum; }); }
            else { tc.SelectedIndex = IndexNum; }
        }

        /// <summary>Rename TabPage Text</summary>
        /// <param name="tp">TabPage Name</param>
        /// <param name="NewName"> </param>
        public void Rename_Tab(TabPage tp, string NewName)
        {
            tp.Text = NewName;
        }

        /// <summary>Hide/Remove Tab from TabControl</summary>
        /// <param name="tc">TabControl Name</param>
        /// <param name="tp">TabPage Name</param>
        public void Hide_Tab(TabControl tc, TabPage tp)
        {
            tp.Hide();
            tc.TabPages.Remove(tp);
        }

        /// <summary>Close/Remove Current TabPage</summary>
        /// <param name="tc">TabControl Name</param>
        public void Close_Current(TabControl tc)
        {
            int CurrentTabIndex = tc.SelectedIndex; // determine previous tab index number to jump to after closing current
            CurrentTabIndex--;

            // Close Selected Tab
            TabPage ThisTab = tc.SelectedTab;
            ThisTab.Hide();
            tc.TabPages.Remove(ThisTab);

            tc.SelectedIndex = CurrentTabIndex; // select the previous tab after closing current
        }

        /// <summary>Clears All TabPages from a TabControl</summary>
        /// <param name="tc">TabControl Name</param>
        public void Clear_Tabs(TabControl tc)
        {
            tc.TabPages.Clear();
        }

        /// <summary>Creates New TabPage During Run Time, Returns New TabPage Info</summary>
        /// <param name="tc">TabControl Name</param>
        /// <param name="NewTabName"> </param>
        public TabPage Create_New_Dynamic_TabPage(TabControl tc, string NewTabName = "NewTab")
        {
            // opens newly created tab page generated during runtime (works)

            var page = new TabPage(NewTabName);
            var browser = new Scintilla();
            browser.Dock = DockStyle.Fill;
            page.Controls.Add(browser);
            tc.TabPages.Add(page);
            tc.SelectedTab = page;
            return page;
        }

        /// <summary>Opens/Shows/Attaches TabPage Generated at Startup. Uses List of TabPage Names (TabPageList() funtion)</summary>
        /// <param name="tc">TabControl Name</param>
        /// <param name="ExistingTab">Existing TabPage To Display on TabControl</param>
        public TabPage Open_Existing_TabPage(TabControl tc, TabPage ExistingTab)
        {
            // opens tab from preset collection of tabs (works)

            ////TabControl tc = tabControl1;
            //TabPage NextTab = Next_Available_TabPage(tc, ProjectFile);
            //if (NextTab == null) { ahk.MsgBox("Reached Max # of New Tab Pages (" + tabCount.ToString() + ")"); return null; }

            int tabCount = TabCount(tc);
            tc.TabPages.Insert(tabCount, ExistingTab);
            ExistingTab.Show();
            tc.SelectedTab = ExistingTab;

            return ExistingTab;
        }


        #endregion

        #region === Tab Info ===

        /// <summary>Returns Current Tab Count</summary>
        /// <param name="tc">TabControl Name</param>
        public int TabCount(TabControl tc)
        {
            return tc.TabPages.Count;
        }

        /// <summary>Checks If TabPage Created</summary>
        /// <param name="tc">TabControl Name</param>
        /// <param name="tp">TabPage Name</param>
        public bool TabCreated(TabControl tc, TabPage tp)
        {
            try
            {
                int IndexOfTabPage = tc.TabPages.IndexOf(tp);
                if (IndexOfTabPage == -1) { return false; }
                return true;
            }
            catch { return false; }
        }

        /// <summary>Returns Int for TagPage Index in TabControl (-1 If Not Created Yet)</summary>
        /// <param name="tc">TabControl Name</param>
        /// <param name="tp">TabPage Name</param>
        public int TabPageIndex(TabControl tc, TabPage tp)
        {
            int IndexOfTabPage = tc.TabPages.IndexOf(tp);
            return IndexOfTabPage;
        }

        /// <summary>Returns TabPage Display Text</summary>
        /// <param name="tp">TabPage Name</param>
        public string TabPageText(TabPage tp)
        {
            string text = "";

            // return text from control (from any thread)
            if (tp.InvokeRequired) { tp.BeginInvoke((MethodInvoker)delegate () { text = tp.Text; }); }
            else { text = tp.Text; }

            return text;
        }

        #endregion

        #region === Find Controls on Tab ===

        /// <summary>Returns List of Control Types on Selected TabPage</summary>
        /// <param name="tc">TabControl Name</param>
        public List<string> List_Of_Controls_On_CurrentTab(TabControl tc)
        {
            TabPage page = tc.SelectedTab;
            List<string> Controls = new List<string>();

            var controls = page.Controls;  // list of controls on current page

            foreach (var control in controls)
            {
                Controls.Add(control.ToString());
            }

            return Controls;
        }

        /// <summary>Returns the Name of a Scintilla Control on Selected TabPage</summary>
        /// <param name="tc">TabControl Name</param>
        public string Scintilla_Name_on_CurrentTab(TabControl tc)
        {
            TabPage page = tc.SelectedTab;
            string ScintillaName = "";

            if (page.Controls != null)
            {
                var controls = page.Controls;  // list of controls on current page

                foreach (var control in controls)
                {
                    if (control.ToString() == "ScintillaNET.Scintilla")
                    {
                        Scintilla sc = (Scintilla)control;
                        //ahk.MsgBox(sc.Name);
                        ScintillaName = sc.Name;
                    }
                }
            }

            return ScintillaName;
        }

        /// <summary>Returns Name of DataGridView Control on Selected TabPage</summary>
        /// <param name="tc">TabControl Name</param>
        public string DataGridView_Name_CurrentTab(TabControl tc)
        {
            TabPage page = tc.SelectedTab;
            string DataGridViewName = "";

            if (page.Controls != null)
            {
                var controls = page.Controls;  // list of controls on current page

                foreach (var control in controls)
                {
                    if (control.ToString() == "System.Windows.Forms.DataGridView")
                    {
                        DataGridView sc = (DataGridView)control;
                        //ahk.MsgBox(sc.Name);
                        DataGridViewName = sc.Name;
                    }
                }
            }

            return DataGridViewName;
        }

        /// <summary>Returns DataGridView Control on Selected TabPage</summary>
        /// <param name="tc">TabControl Name</param>
        public DataGridView DataGridView_CurrentTab(TabControl tc)
        {
            TabPage page = tc.SelectedTab;
            DataGridView sc = null;

            if (page.Controls != null)
            {
                var controls = page.Controls;  // list of controls on current page

                foreach (var control in controls)
                {
                    if (control.ToString() == "System.Windows.Forms.DataGridView")
                    {
                        sc = (DataGridView)control;
                        //ahk.MsgBox(sc.Name);
                    }
                }
            }

            return sc;
        }

        /// <summary>Returns TreeView Control Name on Selected TabPage</summary>
        /// <param name="tc">TabControl Name</param>
        public string TreeView_Name_CurrentTab(TabControl tc)
        {
            TabPage page = tc.SelectedTab;
            string TreeViewName = "";

            if (page.Controls != null)
            {
                var controls = page.Controls;  // list of controls on current page

                foreach (var control in controls)
                {
                    if (control.ToString().Contains("TreeView")) // == "System.Windows.Forms.TreeView")
                    {
                        TreeView sc = (TreeView)control;
                        //ahk.MsgBox(sc.Name);
                        TreeViewName = sc.Name;
                    }
                }
            }

            return TreeViewName;
        }

        /// <summary>Returns TreeView Control on Selected TabPage</summary>
        /// <param name="tc">TabControl Name</param>
        public TreeView TreeView_CurrentTab(TabControl tc)
        {
            TabPage page = tc.SelectedTab;

            TreeView sc = null;

            if (page.Controls != null)
            {
                var controls = page.Controls;  // list of controls on current page

                foreach (var control in controls)
                {
                    if (control.ToString().Contains("TreeView")) // == "System.Windows.Forms.TreeView")
                    {
                        sc = (TreeView)control;
                        //ahk.MsgBox(sc.Name);
                    }
                }
            }

            return sc;
        }


        /// <summary>Returns Splitter Control Name on Selected TabPage</summary>
        /// <param name="tc">TabControl Name</param>
        public string Splitter_Name_CurrentTab(TabControl tc)
        {
            TabPage page = tc.SelectedTab;
            string SplitterName = "";

            if (page.Controls != null)
            {
                var controls = page.Controls;  // list of controls on current page

                foreach (var control in controls)
                {
                    //if (control.ToString() == "System.Windows.Forms.Splitter")
                    if (control.ToString().Contains("Splitter"))
                    {
                        Splitter sc = (Splitter)control;
                        //ahk.MsgBox(sc.Name);
                        SplitterName = sc.Name;
                    }
                }
            }

            return SplitterName;
        }

        /// <summary>Returns Splitter Control on Selected TabPage</summary>
        /// <param name="tc">TabControl Name</param>
        public Splitter Splitter_CurrentTab(TabControl tc)
        {
            TabPage page = tc.SelectedTab;
            Splitter sc = null;


            if (page.Controls != null)
            {
                var controls = page.Controls;  // list of controls on current page

                foreach (var control in controls)
                {
                    //if (control.ToString() == "System.Windows.Forms.Splitter")
                    if (control.ToString().Contains("Splitter"))
                    {
                        sc = (Splitter)control;
                        //ahk.MsgBox(sc.Name);
                    }
                }
            }

            return sc;
        }



        #endregion

        #region === TESTING ===

        //    public string Generate_TabPage_Code_List(string ProjectFile = "")  // CodeGen: Create TabPage List Function (Populated) To Insert Into Project
        //{
        //    if (ProjectFile == "") { ProjectFile = @"C:\Users\jason\Google Drive\IMDB\SQLiter\NPAD\NPAD.cs"; }
        //    List<string> TabPages = code.ProjectFile_ControlList(ProjectFile, "TABPAGE");

        //    string Code = "public List<TabPage> TabPageList()  // returns list of TabPages to reference throughout project" + Environment.NewLine + "{" + Environment.NewLine;
        //    Code = Code + "\t\t" + "List<TabPage> TabList = new List<TabPage>();" + Environment.NewLine;

        //    foreach (string Tab in TabPages)
        //    {
        //        Code = Code + "\t\t" + "TabList.Add(" + Tab + ");" + Environment.NewLine;
        //    }

        //    Code = Code + "\t\treturn TabList;" + Environment.NewLine + "\t}";

        //    return Code; 
        //}

        //    public TabPage Next_Available_TabPage(TabControl tc, string ProjectFile = "", List<TabPage> TabList = null)  // use list of tabs (auto generated code from dev dll) to find next available tab page control
        //    {
        //        if (TabList == null && ProjectFile != "")
        //        {
        //            TabList = TabPageList();
        //        }

        //        foreach (TabPage tp in TabList)
        //        {
        //            int IndexOfTabPage = tc.TabPages.IndexOf(tp);
        //            if (IndexOfTabPage == -1) { return tp; }  // -1 means not created yet - free to use
        //        }

        //        return null;
        //    }

        //    public TabPage TabPage_From_IndexId(TabControl tc, int IndexId, List<TabPage> TabList = null)  // use list of tabs (auto generated code from dev dll) to find next available tab page control
        //    {
        //        if (TabList == null)
        //        {
        //            TabList = TabPageList();  // return list of tab pages from local function
        //        }

        //        if (TabList != null)
        //        {
        //            foreach (TabPage Tab in TabList)
        //            {
        //                int IndexOfTabPage = tc.TabPages.IndexOf(Tab);
        //                if (IndexOfTabPage == IndexId)
        //                { return Tab; }
        //            }
        //        }

        //        return null;
        //    }

        //    public List<TabPage> TabPageList()  // PLACE HOLDER FOR CODEGEN CODE
        //    {
        //        ahk.MsgBox("Need to Populate First With CodeGen.");
        //        return null;
        //    }

        #endregion

        #region === Save / Load Tab Index ===

        /// <summary>Store the selected index for the tab control to SettingsDb</summary>
        /// <param name="tc">TabControl Name</param>
        public void Save_Last_Tab(TabControl tc, bool LoadOnStartupEnabled = true)
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            _AHK ahk = new _AHK();
            sqlite.Setting_Save(tc.Name + "_LastTab", tc.SelectedIndex.ToString(), LoadOnStartupEnabled.ToString(), ahk.AppDir() + "\\Settings.sqlite");
        }

        /// <summary>Loads last selected tab # when starting app (from Settings.sqlite table)</summary>
        /// <param name="tc">TabControl Name</param>
        public void Load_Last_Tab(TabControl tc)
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            _AHK ahk = new _AHK();

            string LastTabIndex = sqlite.Setting(tc.Name + "_LastTab");

            // read same setting value again, this time for the option field, convert to bool response 
            bool LoadLastTab = ahk.ToBool(sqlite.Setting_Value(tc.Name + "_LastTab", ahk.AppDir() + "\\Settings.sqlite", "Settings", "", true));  // check user setting to see if this option is enabled

            if (LoadLastTab)  // if option enabled, read the last index position and select that tab
            {
                int lTab = ahk.ToInt(LastTabIndex);
                try
                {
                    tc.SelectedIndex = lTab;
                }
                catch { }
            }
        }

        #endregion

    }
}

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
using sharpAHK;
using sharpAHK_Dev;
using AHKExpressions;
using TelerikExpressions;
using System.Windows.Forms;
using System.Threading;
using Telerik.WinControls.UI;
using System.Data.SQLite;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;

namespace sharpAHK_Dev
{
    public class _TreeViewControl
    {
        private static _AHK ahk = new _AHK();
        private static _Database.SQLite sqlite = new _Database.SQLite();

        //sharpAHK._AHK ahk = new sharpAHK._AHK();
        //_Database.SQL sql = new _Database.SQL();
        //_Database.SQLite sqlite = new _Database.SQLite();
        //_GridControl grid = new _GridControl();
        //_Lists lst = new _Lists();
        //sharpAHK._Dict dict = new sharpAHK._Dict();
        //_Images img = new _Images();


        #region === TreeView: Populate ===

        // build treeview example:

        //=== Populate TreeView Example

        // TreeNode parent = new TreeNode();  // level 1
        // parent.Text = "Project";

        //   TreeNode section = new TreeNode();  // level 2
        //   section.Text = "Using";
        //   parent.Nodes.Add(section);

        //         TreeNode entry = new TreeNode();  // level 3
        //         entry.Text = FunctionName;
        //         section.Nodes.Add(entry);

        //   treeView1.Nodes.Add(parent);  // populate tree


        //=========================================
        // Populate TreeView Contents
        //=========================================

        // loop through folder, add sqlite files to treeview under 1 node
        // works BUT messes up tv icon setup

        /// <summary>Search DbDir for *.sqlite files, then load projects in TreeView display</summary>
        /// <param name="TV"> </param>
        /// <param name="DbDir"> </param>
        /// <param name="DbNameAsRoot"> </param>
        /// <param name="ClearTree"> </param>
        public void SQLite_DbDir_ToNodes(TreeView TV, string DbDir, bool ClearTree = false)
        {
            bool DbNameAsRoot = true;

            //if (ParentText != "") { DbNameAsRoot = false; }
            string[] files = Directory.GetFiles(DbDir, "*.sqlite", SearchOption.AllDirectories);

            if (ClearTree) { TV.Nodes.Clear(); } // option to clear out existing grid values before populating

            foreach (string file in files)  // loop through list of files and write file details to sqlite db
            {
                string DBFile = file;

                Dictionary<string, string> TableDict = sqlite.Table_Dict(DBFile); // extract list of table names from sqlite db
                List<string> list = new List<string>(TableDict.Keys);  // Store keys in a List.

                TreeNode dbname = new TreeNode();
                if (DbNameAsRoot)
                {
                    // populate top node with the db file name 
                    string dbfilename = Path.GetFileName(DBFile);
                    dbname.Text = dbfilename;
                    dbname.Tag = "File|" + DBFile;
                }

                foreach (string Table in list)
                {
                    //==== Create Parent Node =============
                    //ahk.MsgBox(Table);
                    TreeNode parent = new TreeNode();
                    parent.Text = Table;
                    parent.Tag = "SQLiteTable|" + DBFile + "|" + Table;

                    if (DbNameAsRoot) { dbname.Nodes.Add(parent); }


                    //=== Create Children Nodes with Column Names ====
                    Dictionary<string, string> ColumnDict = sqlite.Column_Dict(DBFile, Table); // extract list of columns from sqlite db table
                    List<string> ColList = new List<string>(ColumnDict.Keys);  // Store keys in a List.

                    foreach (string Column in ColList)
                    {
                        TreeNode child2 = new TreeNode();
                        child2.Text = Column;
                        child2.Tag = "SQLiteColumn|" + DBFile + "|" + Table + "|" + Column;
                        parent.Nodes.Add(child2);
                    }

                }

                if (DbNameAsRoot) { TV.Nodes.Add(dbname); }
                //if (!DbNameAsRoot) { TV.Nodes.Add(parent); }

            }	// end file loop

        }

        /// <summary>Populate TreeView with SQLite Table Names with Columns Below</summary>
        /// <param name="TV"> </param>
        /// <param name="DbFile"> </param>
        /// <param name="DbNameAsRoot"> </param>
        /// <param name="ClearTree"> </param>
        public void SQLite_Tables_with_Columns(TreeView TV, string DbFile, bool DbNameAsRoot = true, bool ClearTree = false)
        {
            Dictionary<string, string> TableDict = sqlite.Table_Dict(DbFile); // extract list of table names from sqlite db
            List<string> list = new List<string>(TableDict.Keys);  // Store keys in a List.

            if (ClearTree) { TV.Nodes.Clear(); } // option to clear out existing grid values before populating

            TreeNode dbname = new TreeNode();
            if (DbNameAsRoot)
            {
                // populate top node with the db file name 
                string dbfilename = Path.GetFileName(DbFile);
                dbname.Text = dbfilename;
                dbname.Tag = "File|" + DbFile;
            }


            TreeNode parent = new TreeNode();

            foreach (string Table in list)
            {
                //==== Create Parent Node =============
                //ahk.MsgBox(Table);
                parent = new TreeNode();
                parent.Text = Table;
                parent.Tag = "SQLiteTable|" + DbFile + "|" + Table;

                if (DbNameAsRoot) { dbname.Nodes.Add(parent); }

                //=== Create Children Nodes with Column Names ====
                Dictionary<string, string> ColumnDict = sqlite.Column_Dict(DbFile, Table); // extract list of columns from sqlite db table
                List<string> ColList = new List<string>(ColumnDict.Keys);  // Store keys in a List.

                foreach (string Column in ColList)
                {
                    TreeNode child2 = new TreeNode();
                    child2.Text = Column;
                    child2.Tag = "SQLiteColumn|" + DbFile + "|" + Table + "|" + Column;
                    parent.Nodes.Add(child2);
                }
            }

            if (DbNameAsRoot) { TV.Nodes.Add(dbname); }
            if (!DbNameAsRoot) { TV.Nodes.Add(parent); }
        }

        /// <summary>Reads sqlite db file, displays tables and column names in TreeView control</summary>
        /// <param name="TV"> </param>
        /// <param name="DbFile"> </param>
        /// <param name="DbNameAsRoot"> </param>
        /// <param name="ClearTree"> </param>
        public void SQLite_Tables(TreeView TV, string DbFile, bool DbNameAsRoot = true, bool ClearTree = false)
        {
            Dictionary<string, string> TableDict = sqlite.Table_Dict(DbFile); // extract list of table names from sqlite db
            List<string> list = new List<string>(TableDict.Keys);  // Store keys in a List.

            if (ClearTree) { ClearTV(TV); } // option to clear out existing grid values before populating

            //TreeNode dbname = new TreeNode();

            TreeNode parent = new TreeNode();

            //if (DbNameAsRoot)
            //{
            // populate top node with the db file name 
            string dbfilename = Path.GetFileName(DbFile);
            //dbname.Text = dbfilename;

            //==== Create Parent Node =============
            //ahk.MsgBox(Table);
            parent.Text = dbfilename;
            parent.Tag = "File|" + DbFile;
            //dbname.Nodes.Add(parent);
            //}


            int ParentCount = 0;
            foreach (string Table in list)
            {

                TreeNode child2 = new TreeNode();
                child2.Text = Table;
                child2.Tag = "SQLiteTable|" + DbFile + "|" + Table;
                parent.Nodes.Add(child2);


                //==== Create Parent Node =============
                //ahk.MsgBox(Table);

                //if (!DbNameAsRoot)
                //{
                //    TreeNode parent = new TreeNode();
                //    parent.Text = Table;
                //    dbname.Nodes.Add(parent);
                //}


                //if (DbNameAsRoot) { dbname.Nodes.Add(parent); }


                ////=== Create Children Nodes with Column Names ====
                //Dictionary<string, string> ColumnDict = sqlite.ExtractColumnNames(DBFile, Table); // extract list of columns from sqlite db table
                //List<string> ColList = new List<string>(ColumnDict.Keys);  // Store keys in a List.

                //foreach (string Column in ColList)
                //{
                //    TreeNode child2 = new TreeNode();
                //    child2.Text = Column;
                //    parent.Nodes.Add(child2);
                //}

                //if (DbNameAsRoot) 
                //{
                //    //if (ParentCount > 0) { dbname = dbname + " (" + ParentCount + ")"; }
                //    TV.Nodes.Add(dbname); 
                //}
                //if (!DbNameAsRoot) { TV.Nodes.Add(parent); }

                //ParentCount++; 
            }


            // update control (from any thread) -- [ works in dll ]
            if (TV.InvokeRequired) { TV.BeginInvoke((MethodInvoker)delegate () { TV.Nodes.Add(parent); }); }
            else { TV.Nodes.Add(parent); }
        }

        /// <summary>SQLite search results to treeview</summary>
        /// <param name="TV"> </param>
        /// <param name="Db"> </param>
        /// <param name="SQLITE"> </param>
        public void SQLite(TreeView TV, string Db, string SQLITE)
        {
            //string Db = @"C:\Google Drive\IMDB\SQLiter\Db\UserDb.sqlite";
            //Dev.TreeViewControl.TreeView_SQLite(treeView1, Db, "select [HotStringValue] from [HotStrings]"); 

            SQLiteConnection m_dbConnection = sqlite.Connect(Db); // connect to SQLite DB file path - returns connection data

            //=======================================================================================================
            // Connect to SQLite DB - Request Data from DB and Update GUI Fields
            //=======================================================================================================

            SQLiteDataReader reader = sqlite.ReturnSQLite(SQLITE, m_dbConnection);


            //List<string> list = new List<string>(TableDict.Keys);  // Store keys in a List.


            //==== Create Parent Node =============
            //ahk.MsgBox(Table);
            TreeNode parent = new TreeNode();
            parent.Text = "Code Lib";


            while (reader.Read())    // loop through each row returned from select 
            {
                string ReturnVal = reader[0].ToString();
                //ahk.MsgBox(ReturnVal); 


                TreeNode child2 = new TreeNode();
                child2.Text = ReturnVal;
                parent.Nodes.Add(child2);



                ////=== Create Children Nodes with Column Names ====
                //Dictionary<string, string> ColumnDict = sqlite.ExtractColumnNames(DBFile, Table); // extract list of columns from sqlite db table
                //List<string> ColList = new List<string>(ColumnDict.Keys);  // Store keys in a List.

                //foreach (string Column in ColList)
                //{
                //    TreeNode child2 = new TreeNode();
                //    child2.Text = Column;
                //    parent.Nodes.Add(child2);
                //}

                //TV.Nodes.Add(parent);


            }

            TV.Nodes.Add(parent);

        }

        /// <summary>Populates treeview with list of files/folders found under path</summary>
        /// <param name="TV"> </param>
        /// <param name="path"> </param>
        /// <param name="ClearGrid"> </param>
        public void List_Directory(TreeView TV, string path, bool ClearGrid = true)
        {
            if (ClearGrid) { ClearTV(TV); }  // option to clear out existing treeview values before populating

            var stack = new Stack<TreeNode>();
            var rootDirectory = new DirectoryInfo(path);
            var node = new TreeNode(rootDirectory.Name) { Tag = rootDirectory };
            //node.ImageIndex = 5;  // sets icon for root node
            //node.SelectedImageIndex = 5;  // icon for when node is selected
            stack.Push(node);

            while (stack.Count > 0)
            {
                var currentNode = stack.Pop();

                string dirPath = currentNode.Tag.ToString();

                if (dirPath.Contains("|")) // if this came from a tag that contains and ID parameter, parse to 2nd value for dir path
                {
                    dirPath = ahk.StringSplit(dirPath, "|", 1);
                }


                DirectoryInfo directoryInfo = new DirectoryInfo(dirPath);


                //var directoryInfo = (DirectoryInfo)currentNode.Tag;

                if (directoryInfo.Exists)
                {
                    foreach (var directory in directoryInfo.GetDirectories())
                    {
                        //string DirName = directory.ToString();
                        //string DirPath = directory.Name; 

                        TreeNode childDirectoryNode = new TreeNode(directory.Name); //{ Tag = directory };

                        childDirectoryNode.Tag = "Directory|" + path + "\\" + directory;

                        //childDirectoryNode.ImageIndex = 6;  // sets icon for node (FOLDER ICO)
                        //childDirectoryNode.SelectedImageIndex = 6;  // icon for when node is selected (FOLDER ICO)

                        currentNode.Nodes.Add(childDirectoryNode);
                        stack.Push(childDirectoryNode);
                    }
                    foreach (var file in directoryInfo.GetFiles())
                    {
                        string FileName = file.ToString();
                        string FilePath = directoryInfo.FullName.ToString() + "\\" + FileName;

                        //string FilePath = path + "\\" + DirName + "\\" + FileName;

                        if (ahk.IfNotExist(FilePath)) { ahk.MsgBox(FilePath + " Not Found"); }

                        TreeNode addNode = new TreeNode();
                        addNode.Text = file.Name;
                        addNode.Tag = "File|" + FilePath;

                        currentNode.Nodes.Add(addNode);
                    }
                }

            }


            // update control (from any thread) -- [ works in dll ]
            if (TV.InvokeRequired) { TV.BeginInvoke((MethodInvoker)delegate () { TV.Nodes.Add(node); }); }
            else { TV.Nodes.Add(node); }
        }

        /// <summary>Populates treeview with list of files/folders found under path, putting files at top/root node in TV</summary>
        /// <param name="TV"> </param>
        /// <param name="path"> </param>
        /// <param name="ClearGrid"> </param>
        public void List_Directory_NoParent(TreeView TV, string path, bool ClearGrid = true)
        {
            _Lists lst = new _Lists();

            if (ClearGrid) { ClearTV(TV); }  // option to clear out existing treeview values before populating

            List<string> FileList = lst.FileList(path, "*.*", false, true, true);

            Load_List(TV, FileList);


            //// update control (from any thread) -- [ works in dll ]
            //if (TV.InvokeRequired) { TV.BeginInvoke((MethodInvoker)delegate() { TV.Nodes.Add(node); }); }
            //else { TV.Nodes.Add(node); }
        }

        /// <summary>List folders under dirPath to nodes</summary>
        /// <param name="TV"> </param>
        /// <param name="DirPath"> </param>
        /// <param name="ClearGrid"> </param>
        /// <param name="NoParent"> </param>
        public void Directory_List(TreeView TV, string DirPath, bool ClearGrid = true, bool NoParent = false)
        {
            if (ClearGrid) { ClearTV(TV); }  // option to clear out existing treeview values before populating

            _Lists lst = new _Lists();
            List<string> DirList = lst.DirList(DirPath, "*.*", false, true);



            // ### Load List in TreeView - UnderNeath "ParentName" ===
            if (!NoParent)
            {
                //==== Create Parent Node =============
                TreeNode parent = new TreeNode();
                parent.Text = ahk.DirName(DirPath);
                parent.Tag = "Directory|" + DirPath;

                //==== Loop To Create Children Nodes =======
                foreach (string ListItem in DirList)
                {
                    TreeNode child2 = new TreeNode();

                    string dirName = ahk.FileName(ListItem);

                    child2.Text = dirName;
                    child2.Tag = "Directory|" + ListItem;

                    parent.Nodes.Add(child2);
                }

                TV.Nodes.Add(parent);  // add to treeview
            }


            // ### Load List In TreeView With No Parent ===
            if (NoParent)
            {
                //==== Loop To Create Parent Nodes =======
                foreach (string ListItem in DirList)
                {
                    //==== Create Parent Node =============
                    TreeNode parent = new TreeNode();
                    string dirName = ahk.FileName(ListItem);
                    parent.Text = dirName;
                    parent.Tag = "Directory|" + ListItem;
                    TV.Nodes.Add(parent);  // add to treeview    
                }
            }

        }

        /// <summary>//SQL Search results to Treeview</summary>
        public void SQL(TreeView TV, SqlConnection Conn, string Command, string ParentText = "")
        {
            _Database.SQL sql = new _Database.SQL();

            List<string> sqlReturn = sql.SQL_To_List(Conn, Command);

            //==== ParentNodeText Provided - Create Top Node with SQL Search Results Below ========

            if (ParentText != "")
            {
                TreeNode parent = new TreeNode();
                parent.Text = ParentText;

                foreach (string item in sqlReturn)
                {
                    TreeNode child2 = new TreeNode();
                    child2.Text = item;
                    parent.Nodes.Add(child2);
                }

                TV.Nodes.Add(parent);
            }

            //==== No Parent Text Provided = All Parents / No Children ========

            if (ParentText == "")
            {
                foreach (string item in sqlReturn)
                {
                    TreeNode parent = new TreeNode();
                    parent.Text = item;
                    TV.Nodes.Add(parent);
                }
            }

        }


        /// <summary>Load List<string> into TreeView (option to display under parent node)</summary>
        /// <param name="TV"> </param>
        /// <param name="LoadList"> </param>
        /// <param name="ClearTV"> Option to clear out existing treeview contents before loading drives</param>
        /// <param name="NodeParentName"> </param>
        public void Load_File_List(TreeView TV, List<string> LoadList, bool ClearTV = true, string NodeParentName = "")
        {
            if (ClearTV) { TV.Nodes.Clear(); }

            // if node parent name provided in parameters, use that as Node header. otherwise no header
            bool NoParent = true; if (NodeParentName != "") { NoParent = false; }

            // ### Load List in TreeView - UnderNeath "ParentName" ===
            if (!NoParent)
            {
                //==== Create Parent Node =============
                TreeNode parent = new TreeNode();
                parent.Text = NodeParentName;

                //==== Loop To Create Children Nodes =======
                foreach (string ListItem in LoadList)
                {
                    string FilePath = ListItem;

                    string tag = "File|" + FilePath;
                    // check to see if the path provided is a directory - update tag if so
                    bool IsDIR = ahk.IsDir(FilePath);
                    if (IsDIR) { tag = "Directory|" + FilePath; }

                    TreeNode child2 = new TreeNode();
                    child2.Text = ahk.FileName(FilePath);
                    child2.Tag = tag;
                    parent.Nodes.Add(child2);
                }

                TV.Nodes.Add(parent);  // add to treeview

            }


            // ### Load List In TreeView With No Parent ===
            if (NoParent)
            {
                //==== Loop To Create Parent Nodes =======
                foreach (string ListItem in LoadList)
                {
                    string FilePath = ListItem;

                    //==== Create Parent Node =============
                    TreeNode parent = new TreeNode();
                    //parent.Text = ListItem;

                    string tag = "File|" + FilePath;
                    // check to see if the path provided is a directory - update tag if so
                    bool IsDIR = ahk.IsDir(FilePath);
                    if (IsDIR) { tag = "Directory|" + FilePath; }

                    parent.Text = ahk.FileName(FilePath);
                    parent.Tag = tag;
                    TV.Nodes.Add(parent);  // add to treeview    
                }
            }
        }

        /// <summary>Load list of drives on pc in treeview</summary>
        /// <param name="TV"> </param>
        /// <param name="ClearTV"> Option to clear out existing treeview contents before loading drives</param>
        /// <param name="ParentName"> </param>
        public void Load_Drives(TreeView TV, bool ClearTV = true, string ParentName = "")
        {
            if (ClearTV) { TV.Nodes.Clear(); }

            // if node parent name provided in parameters, use that as Node header. otherwise no header
            bool NoParent = true; if (ParentName != "") { NoParent = false; }

            TV.ShowNodeToolTips = true;

            DataTable driveInfo = DriveInfo_Table();

            // ### Load List in TreeView - UnderNeath "ParentName" ===
            if (!NoParent)
            {
                //==== Create Parent Node =============
                TreeNode parent = new TreeNode();
                parent.Text = ParentName;

                List<string> DriveListAdds = new List<string>();
                if (driveInfo != null)
                {
                    foreach (DataRow row in driveInfo.Rows)
                    {
                        string Name = row["Name"].ToString();
                        string Label = row["VolumeLabel"].ToString();
                        string FreeSpace = row["Free Space"].ToString();
                        string TotalSpace = row["Total Space"].ToString();
                        string RootDir = row["RootDir"].ToString();


                        TreeNode child2 = new TreeNode();
                        child2.Text = RootDir + " - " + Label;
                        child2.Tag = "DriveInfo|" + Name + "|" + Label + "|" + FreeSpace + "|" + TotalSpace + "|" + RootDir;
                        child2.ToolTipText = FreeSpace + " / " + TotalSpace;
                        parent.Nodes.Add(child2);

                        //// create string with drive info to match with other returned values - sort out duplicate network references
                        //string ID = Label + "|" + FreeSpace + "|" + TotalSpace;
                        //bool INList = lst.InList(DriveListAdds, ID);
                        //if (!INList) // add drive to list if not already added
                        //{
                        //    DriveListAdds.Add(ID);
                        //    DriveLetters.Add(RootDir);
                        //}
                    }
                }


                TV.Nodes.Add(parent);  // add to treeview
            }


            // ### Load List In TreeView With No Parent ===
            if (NoParent)
            {
                if (driveInfo != null)
                {
                    foreach (DataRow row in driveInfo.Rows)
                    {
                        string Name = row["Name"].ToString();
                        string Label = row["VolumeLabel"].ToString();
                        string FreeSpace = row["Free Space"].ToString();
                        string TotalSpace = row["Total Space"].ToString();
                        string RootDir = row["RootDir"].ToString();


                        TreeNode parent = new TreeNode();
                        parent.Text = RootDir + " - " + Label;
                        parent.Tag = RootDir;
                        parent.ToolTipText = FreeSpace + " / " + TotalSpace;
                        TV.Nodes.Add(parent);  // add to treeview

                        //// create string with drive info to match with other returned values - sort out duplicate network references
                        //string ID = Label + "|" + FreeSpace + "|" + TotalSpace;
                        //bool INList = lst.InList(DriveListAdds, ID);
                        //if (!INList) // add drive to list if not already added
                        //{
                        //    DriveListAdds.Add(ID);
                        //    DriveLetters.Add(RootDir);
                        //}
                    }
                }

            }
        }

        /// <summary>Returns datatable with drive information</summary>
        public DataTable DriveInfo_Table()
        {
            // create new datatable structure
            DataTable table = new DataTable();
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("VolumeLabel", typeof(string));
            table.Columns.Add("Free Space", typeof(string));
            table.Columns.Add("Percent Free", typeof(string));
            table.Columns.Add("Total Space", typeof(string));
            table.Columns.Add("Type", typeof(string));
            table.Columns.Add("Format", typeof(string));
            table.Columns.Add("RootDir", typeof(string));

            // loop through visible / ready to access drives and populate table
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.DriveType.ToString().ToUpper() == "CDROM") { continue; }

                if (drive.IsReady)  // excludes removeable drives that aren't currently connected 
                {
                    double freeSpace = drive.TotalFreeSpace;
                    double totalSpace = drive.TotalSize;
                    double percentFree = (freeSpace / totalSpace) * 100;
                    float num = (float)percentFree;

                    string label = drive.VolumeLabel.ToString();
                    if (label == "") { label = "[ No Label ]"; }

                    string TotalSpace = ahk.FormatBytes(drive.TotalSize);
                    string FreeSpace = ahk.FormatBytes(drive.AvailableFreeSpace);

                    table.Rows.Add(drive.Name.ToString(), label, FreeSpace, num.ToString(), TotalSpace, drive.DriveType.ToString(), drive.DriveFormat.ToString(), drive.RootDirectory.ToString());
                }
            }

            return table;
        }

        /// <summary>Load List<string> into TreeView (option to display under parent node) - If List Item contains "|" it splits by NodeText|NodeTag</summary>
        /// <param name="TV"> </param>
        /// <param name="LoadList"> </param>
        /// <param name="NodeParentName"> </param>
        public void Load_List(TreeView TV, List<string> LoadList, string NodeParentName = "")
        {
            if (LoadList == null) { return; }

            // if node parent name provided in parameters, use that as Node header. otherwise no header
            bool NoParent = true; if (NodeParentName != "") { NoParent = false; }

            // ### Load List in TreeView - UnderNeath "ParentName" ===
            if (!NoParent)
            {
                //==== Create Parent Node =============
                TreeNode parent = new TreeNode();
                parent.Text = NodeParentName;

                if (NodeParentName.Contains("|"))
                {
                    string[] words = NodeParentName.Split('|');
                    int i = 0;
                    foreach (string word in words)
                    {
                        if (i == 0) { parent.Text = word; }
                        if (i == 1) { parent.Tag = word; }
                        i++;
                    }

                    if (parent.Text.Trim() == "") { parent.Text = parent.Tag.ToString(); }
                }


                //==== Loop To Create Children Nodes =======
                foreach (string ListItem in LoadList)
                {
                    TreeNode child2 = new TreeNode();
                    child2.Text = ListItem;

                    if (ListItem.Contains("|"))
                    {
                        string[] words = ListItem.Split('|');
                        int i = 0;
                        foreach (string word in words)
                        {
                            if (i == 0) { child2.Text = word; }
                            if (i == 1) { child2.Tag = word; }
                            i++;
                        }

                        if (child2.Text.Trim() == "") { child2.Text = child2.Tag.ToString(); }
                    }

                    parent.Nodes.Add(child2);
                }

                TV.Nodes.Add(parent);  // add to treeview

            }


            // ### Load List In TreeView With No Parent ===
            if (NoParent)
            {
                //==== Loop To Create Parent Nodes =======
                foreach (string ListItem in LoadList)
                {
                    //==== Create Parent Node =============
                    TreeNode parent = new TreeNode();
                    parent.Text = ListItem;

                    if (ListItem.Contains("|"))
                    {
                        string[] words = ListItem.Split('|');
                        int i = 0;
                        foreach (string word in words)
                        {
                            if (i == 0) { parent.Text = word; }
                            if (i == 1) { parent.Tag = word; }
                            i++;
                        }
                    }


                    TV.Nodes.Add(parent);  // add to treeview    
                }
            }
        }

        /// <summary>Load List<int> into TreeView (option to display under parent node)</summary>
        /// <param name="TV"> </param>
        /// <param name="LoadList"> </param>
        /// <param name="NodeParentName"> </param>
        public void Load_List_Int(TreeView TV, List<int> LoadList, string NodeParentName = "")
        {
            // if node parent name provided in parameters, use that as Node header. otherwise no header
            bool NoParent = true; if (NodeParentName != "") { NoParent = false; }

            // ### Load Lis in TreeView - UnderNeath "ParentName" ===
            if (!NoParent)
            {
                //==== Create Parent Node =============
                TreeNode parent = new TreeNode();
                parent.Text = NodeParentName;

                //==== Loop To Create Children Nodes =======
                foreach (int ListItem in LoadList)
                {
                    TreeNode child2 = new TreeNode();
                    child2.Text = ListItem.ToString();
                    parent.Nodes.Add(child2);
                }

                TV.Nodes.Add(parent);  // add to treeview

            }


            // ### Load List In TreeView With No Parent ===
            if (NoParent)
            {
                //==== Loop To Create Parent Nodes =======
                foreach (int ListItem in LoadList)
                {
                    //==== Create Parent Node =============
                    TreeNode parent = new TreeNode();
                    parent.Text = ListItem.ToString();
                    TV.Nodes.Add(parent);  // add to treeview    
                }
            }
        }



        /// <summary>Create Node in TreeView with List of DataGridView Columns</summary>
        /// <param name="TV"> </param>
        /// <param name="dv"> </param>
        /// <param name="ParentName"> </param>
        /// <param name="ClearTree"> </param>
        /// <param name="CheckBoxes"> </param>
        /// <param name="ExpandOnLoad"> </param>
        /// <param name="ColumnTag"> </param>
        public void Load_DataGridColumns(TreeView TV, DataGridView dv, string ParentName = "DataGridView1", bool ClearTree = true, bool CheckBoxes = true, bool ExpandOnLoad = true, string ColumnTag = "")
        {
            if (ClearTree) { ClearTV(TV); } // option to clear treeview before populating

            //==== Create Parent Node =============
            TreeNode parent = new TreeNode();
            parent.Text = ParentName;
            parent.Tag = "DataGridColumns|" + dv.Name;

            try
            {
                int columnCount = dv.Rows[0].Cells.Count;
                for (int i = 0; i < columnCount; i++)
                {
                    TreeNode child2 = new TreeNode();
                    child2.Text = dv.Columns[i].HeaderText;
                    child2.Checked = dv.Columns[i].Visible;
                    child2.Tag = "DataGridColumns|" + dv.Name + "|" + dv.Columns[i].Name;
                    parent.Nodes.Add(child2);

                }

                TV.Nodes.Add(parent);  // add to treeview
            }
            catch { }

            if (ExpandOnLoad) { TV.ExpandAll(); } // option to expand results in treeview

            if (CheckBoxes) { TV.CheckBoxes = true; }

        }

        /// <summary>Start at root of folder, take each subfolder + .tag files underneath, populate treeview control</summary>
        /// <param name="TagDirRoot"> </param>
        /// <param name="TV"> </param>
        /// <param name="CheckBoxes"> </param>
        /// <param name="Expand"> </param>
        /// <param name="clearTV"> </param>
        public void TagDir_To_Tree(string TagDirRoot, TreeView TV, bool CheckBoxes = true, bool Expand = true, bool clearTV = true)
        {
            _Lists lst = new _Lists();

            if (clearTV) { ClearTV(TV); }

            // TagDir To Tree
            //string TagDirRoot = @"C:\Users\Jason\Google Drive\IMDB\SQLiter\Dynamic_Coder\bin\Debug\Project_Files\Tags\[CodeTags]";
            List<string> tagDirs = lst.DirList(TagDirRoot, "*.*", false);

            foreach (string tagDir in tagDirs)
            {
                string TagDir = TagDirRoot + "\\" + tagDir;

                string DirName = ahk.DirName(TagDir);
                List<string> tags = lst.FileList(TagDir, "*.tag", false, true, false);

                //=== Populate TreeView

                TreeNode parent = new TreeNode();  // level 1
                parent.Text = tagDir;

                foreach (string tag in tags)
                {
                    TreeNode section = new TreeNode();  // level 2
                    section.Text = tag;
                    parent.Nodes.Add(section);
                }

                TV.Nodes.Add(parent);  // populate tree
            }

            if (CheckBoxes) { TV.CheckBoxes = true; }
            if (Expand) { TV.ExpandAll(); }
        }

        /// <summary>Populate treenode with list of tabpages from Function_Dev project</summary>
        /// <param name="TV"> </param>
        /// <param name="ParentName"> </param>
        public TreeNode Function_Dev_TabPage_TreeNode(TreeView TV = null, string ParentName = "TabPages")
        {
            TreeNode topleve = new TreeNode(); topleve.Text = ParentName;


            //==== Create Parent Node =============
            TreeNode parent = new TreeNode(); parent.Text = "Main_Menu"; parent.Tag = "tabMainMenu"; topleve.Nodes.Add(parent);


            TreeNode child1 = new TreeNode(); child1.Text = "Testing"; child1.Tag = "tabTesting"; parent.Nodes.Add(child1);
            TreeNode child2 = new TreeNode(); child2.Text = "tabInsertCode"; parent.Nodes.Add(child2);
            TreeNode child3 = new TreeNode(); child3.Text = "tabDuplicate"; parent.Nodes.Add(child3);
            TreeNode child4 = new TreeNode(); child4.Text = "tabProjectGrid"; parent.Nodes.Add(child4);
            TreeNode child5 = new TreeNode(); child5.Text = "tabControls"; parent.Nodes.Add(child5);

            TreeNode child6 = new TreeNode(); child6.Text = "tabDataGridViewCode"; child5.Nodes.Add(child6);
            TreeNode child7 = new TreeNode(); child7.Text = "tabDataGridViewClickEvents"; child6.Nodes.Add(child7);
            TreeNode child8 = new TreeNode(); child8.Text = "tabDataGridViewReturnFrom"; child6.Nodes.Add(child8);
            TreeNode child9 = new TreeNode(); child9.Text = "tabDataGridViewModify"; child6.Nodes.Add(child9);
            TreeNode child10 = new TreeNode(); child10.Text = "dataGridViewDataGridView"; child6.Nodes.Add(child10);

            TreeNode child11 = new TreeNode(); child11.Text = "tabTreeViewCode"; child5.Nodes.Add(child11);
            TreeNode child12 = new TreeNode(); child12.Text = "tabTreeViewClickEvents"; child11.Nodes.Add(child12);
            TreeNode child13 = new TreeNode(); child13.Text = "tabTreeViewPopulate"; child11.Nodes.Add(child13);
            TreeNode child14 = new TreeNode(); child14.Text = "tabTreeViewReturnFrom"; child11.Nodes.Add(child14);
            TreeNode child15 = new TreeNode(); child15.Text = "tabTreeViewModify"; child11.Nodes.Add(child15);
            TreeNode child16 = new TreeNode(); child16.Text = "tabTreeViewFunctions"; child11.Nodes.Add(child16);

            TreeNode child17 = new TreeNode(); child17.Text = "tabScintillaCode"; child5.Nodes.Add(child17);
            TreeNode child18 = new TreeNode(); child18.Text = "tabScintillaConfigure"; child17.Nodes.Add(child18);
            TreeNode child19 = new TreeNode(); child19.Text = "tabScintillaFunctions"; child17.Nodes.Add(child19);
            TreeNode child20 = new TreeNode(); child20.Text = "tabComboBox"; child5.Nodes.Add(child20);

            TreeNode child21 = new TreeNode(); child21.Text = "tabListBox"; child5.Nodes.Add(child21);
            TreeNode child22 = new TreeNode(); child22.Text = "tabTabControl"; child5.Nodes.Add(child22);
            TreeNode child23 = new TreeNode(); child23.Text = "tabStatusBar"; child5.Nodes.Add(child23);
            TreeNode child24 = new TreeNode(); child24.Text = "tabOpenFileDialog"; child6.Nodes.Add(child24);
            TreeNode child25 = new TreeNode(); child25.Text = "tabMenu"; child5.Nodes.Add(child25);
            TreeNode child26 = new TreeNode(); child26.Text = "tabSaveFileDialog"; child5.Nodes.Add(child26);
            TreeNode child27 = new TreeNode(); child27.Text = "tabFileSystemWatcher"; child5.Nodes.Add(child27);
            TreeNode child28 = new TreeNode(); child28.Text = "tabButtons"; child5.Nodes.Add(child28);
            TreeNode child29 = new TreeNode(); child29.Text = "tabRegions"; parent.Nodes.Add(child29);
            TreeNode child30 = new TreeNode(); child30.Text = "tabDatabase"; parent.Nodes.Add(child30);
            TreeNode child39 = new TreeNode(); child39.Text = "tabFunctions"; parent.Nodes.Add(child39);

            //TV.Nodes.Add(parent);  // add to treeview

            // tab side tree 

            TreeNode parent2 = new TreeNode(); parent2.Text = "tabSide"; topleve.Nodes.Add(parent2);
            TreeNode child31 = new TreeNode(); child31.Text = "tabProjectTree"; parent2.Nodes.Add(child31);
            TreeNode child32 = new TreeNode(); child32.Text = "tabControlList"; parent2.Nodes.Add(child32);
            TreeNode child33 = new TreeNode(); child33.Text = "tabGridOptions"; parent2.Nodes.Add(child33);

            //TV.Nodes.Add(parent2);  // add to treeview


            TreeNode parent3 = new TreeNode(); parent3.Text = "tabBottom"; topleve.Nodes.Add(parent3);
            TreeNode child34 = new TreeNode(); child34.Text = "tabNPad"; parent3.Nodes.Add(child34);
            TreeNode child35 = new TreeNode(); child35.Text = "tabBottomGrid"; parent3.Nodes.Add(child35);
            TreeNode child36 = new TreeNode(); child36.Text = "tabControlNpad"; parent3.Nodes.Add(child36);

            TreeNode parent4 = new TreeNode(); parent4.Text = "tabControlBottomRight"; topleve.Nodes.Add(parent4);
            TreeNode child37 = new TreeNode(); child37.Text = "tabControlInfo"; parent4.Nodes.Add(child37);
            TreeNode child38 = new TreeNode(); child38.Text = "tabList"; parent4.Nodes.Add(child38);

            //TV.Nodes.Add(topleve);  // add to treeview


            return topleve;



        }

        /// <summary>Populate treeview with list of color options</summary>
        /// <param name="TV"> </param>
        /// <param name="ShowColors"> </param>
        /// <param name="ParentName"> </param>
        /// <param name="NoParent"> </param>
        /// <param name="cLearTV"> </param>
        public void ColorListTree(TreeView TV, bool ShowColors = true, string ParentName = "Colors", bool NoParent = true, bool cLearTV = true)
        {
            _Code code = new _Code();

            List<string> LoadList = code.ColorList();
            //lst.List_To_TreeView(TV, colors, ParentName, NoParent, cLearTV);

            // ### Load List in TreeView - UnderNeath "ParentName" ===
            if (!NoParent)
            {
                //==== Create Parent Node =============
                TreeNode parent = new TreeNode();
                parent.Text = ParentName;

                //==== Loop To Create Children Nodes =======
                foreach (string ListItem in LoadList)
                {
                    TreeNode child2 = new TreeNode();
                    child2.Text = ListItem;
                    child2.Tag = "Colors|" + ListItem;

                    if (ShowColors) { Color NodeColor = code.Return_Color(child2.Text); child2.BackColor = NodeColor; }

                    parent.Nodes.Add(child2);
                }

                TV.Nodes.Add(parent);  // add to treeview

            }


            // ### Load List In TreeView With No Parent ===
            if (NoParent)
            {
                //==== Loop To Create Parent Nodes =======
                foreach (string ListItem in LoadList)
                {
                    //==== Create Parent Node =============
                    TreeNode parent = new TreeNode();
                    parent.Text = ListItem;

                    if (ShowColors) { Color NodeColor = code.Return_Color(parent.Text); parent.BackColor = NodeColor; }

                    TV.Nodes.Add(parent);  // add to treeview    
                }
            }
        }


        /// <summary>Display all controls from Form in TreeView</summary>
        /// <param name="TV"> </param>
        /// <param name="FormName"> </param>
        /// <param name="ExpandTree"> </param>
        public void Form_Controls(TreeView TV, Control FormName, bool ExpandTree = true)
        {
            _Lists lst = new _Lists();

            // Display Controls from Form in TreeView
            List<Control> controlList = lst.Control_List(FormName);
            List<string> controls = new List<string>();
            foreach (Control control in controlList)
            {
                //ahk.MsgBox(control.GetType().ToString());

                string ListItem = control.Name.ToString();
                if (ListItem == "") { ListItem = control.GetType().ToString(); }
                controls.Add(ListItem);
            }

            Form formname = (Form)FormName;
            string name = formname.Name.ToString();

            lst.List_To_TreeView(TV, controls, name + " Controls");

            if (ExpandTree) { Expand(TV); }
        }

        /// <summary>Display all controls from Form in TreeView - Sorted By Control Type</summary>
        /// <param name="TV"> </param>
        /// <param name="FormName"> </param>
        /// <param name="ExpandTree"> </param>
        public void Form_Controls_ByType(TreeView TV, Control FormName, bool ExpandTree = true)
        {
            _Lists lst = new _Lists();

            // Display Controls from Form in TreeView
            List<Control> controlList = lst.Control_List(FormName);
            List<string> controls = new List<string>();

            List<string> controlTypes = new List<string>();

            foreach (Control control in controlList)
            {
                // the type of control found
                string cType = control.GetType().ToString();
                //ahk.MsgBox(cType);

                // add control type to list if type not already in list
                if (!lst.InList(controlTypes, cType))
                    controlTypes.Add(cType);

                //string ListItem = control.Name.ToString();
                //if (ListItem == "") { ListItem = control.GetType().ToString(); }
                //controls.Add(ListItem);
            }

            Form formname = (Form)FormName;
            string name = formname.Name.ToString();

            TreeNode parent = new TreeNode();  // level 1
            parent.Text = name;

            foreach (string Type in controlTypes)
            {
                TreeNode section = new TreeNode();  // level 2
                string type = ahk.StringReplace(Type, "System.Windows.Forms.");
                type = ahk.StringReplace(type, "ScintillaNET.");
                section.Text = type;
                section.Tag = "Control|" + type;
                parent.Nodes.Add(section);

                foreach (Control control in controlList)
                {
                    string cType = control.GetType().ToString();

                    if (cType == Type)
                    {
                        // use the control name as the display name - otherwise default to control type
                        string ListItem = control.Name.ToString();
                        if (ListItem == "") { ListItem = control.GetType().ToString(); }

                        controls.Add(ListItem);

                        TreeNode entry = new TreeNode();  // level 3
                        entry.Tag = "Control|" + type + "|" + ListItem;
                        entry.Text = ListItem;
                        section.Nodes.Add(entry);
                    }
                }
            }

            TV.Nodes.Add(parent);  // populate treeview

            if (ExpandTree) { Expand(TV); }  // option to expand on load
        }

        /// <summary>Display List of Running Processes in TreeView</summary>
        /// <param name="TV"> </param>
        /// <param name="ClearTree"> </param>
        /// <param name="ExpandTV"> </param>
        public void Process_Tree(TreeView TV, bool ClearTree = true, bool ExpandTV = false)
        {
            // displays list of user's current processes

            if (ClearTree) { ClearTV(TV); }


            Process[] processlist = Process.GetProcesses();

            //List<Process> pList = processlist.ToList();


            TreeNode parent = new TreeNode();  // level 1
            foreach (Process theprocess in processlist)
            {
                if (!String.IsNullOrEmpty(theprocess.MainWindowTitle))
                {

                    try
                    {
                        sharpAHK.winInfo WinPositions = new sharpAHK.winInfo();
                        WinPositions = ahk.WinGetPos("ahk_PID " + theprocess.MainWindowHandle);

                        string cLass = ahk.WinGetClass("ahk_PID " + theprocess.MainWindowHandle);
                        bool IsMin = ahk.IsMinimized("ahk_PID " + theprocess.MainWindowHandle);
                        bool IsMax = ahk.IsMaximized("ahk_PID " + theprocess.MainWindowHandle);

                        //string ProcessInfo = "ProcessInfo|" + "ProcessName" + "|" + theprocess.ProcessName + "WinTitle|" + theprocess.MainWindowTitle + "IsMinimized|" + IsMin.ToString() + "IsMaximized|" + IsMax.ToString() + "Handle|" + theprocess.Handle + "MainWinHandle|" + theprocess.MainWindowHandle + "WinX|" + WinPositions.WinX + "WinY|" + WinPositions.WinY + "WinW|" + WinPositions.WinW + "WinH|" + WinPositions.WinH + "WinClass|" + cLass;

                        parent = new TreeNode();  // level 1
                        parent.Text = "Process Name: " + theprocess.ProcessName;
                        parent.Tag = "ProcessName" + "|" + theprocess.ProcessName;
                        //parent.Tag = ProcessInfo;

                        TreeNode section = new TreeNode();  // level 2
                        section.Text = "WinTitle: " + theprocess.MainWindowTitle;
                        section.Tag = "WinTitle|" + theprocess.MainWindowTitle;
                        //section.Tag = ProcessInfo; 
                        parent.Nodes.Add(section);

                        section = new TreeNode();  // level 2
                        section.Text = "Minimized: " + IsMin.ToString();
                        section.Tag = "IsMinimized|" + IsMin.ToString();
                        parent.Nodes.Add(section);

                        section = new TreeNode();  // level 2
                        section.Text = "Maximized: " + IsMax.ToString();
                        section.Tag = "IsMaximized|" + IsMax.ToString();
                        parent.Nodes.Add(section);

                        section = new TreeNode();  // level 2
                        section.Text = "Handle: " + theprocess.Handle;
                        section.Tag = "Handle|" + theprocess.Handle;
                        parent.Nodes.Add(section);

                        section = new TreeNode();  // level 2
                        section.Text = "MainWin Handle: " + theprocess.MainWindowHandle;
                        section.Tag = "MainWinHandle|" + theprocess.MainWindowHandle;
                        parent.Nodes.Add(section);

                        section = new TreeNode();  // level 2
                        section.Text = "WinX: " + WinPositions.WinX;
                        section.Tag = "WinX|" + WinPositions.WinX;
                        parent.Nodes.Add(section);

                        section = new TreeNode();  // level 2
                        section.Text = "WinY: " + WinPositions.WinY;
                        section.Tag = "WinY|" + WinPositions.WinY;
                        parent.Nodes.Add(section);

                        section = new TreeNode();  // level 2
                        section.Text = "WinW: " + WinPositions.WinW;
                        section.Tag = "WinW|" + WinPositions.WinW;
                        parent.Nodes.Add(section);

                        section = new TreeNode();  // level 2
                        section.Text = "WinH: " + WinPositions.WinH;
                        section.Tag = "WinH|" + WinPositions.WinH;
                        parent.Nodes.Add(section);

                        section = new TreeNode();  // level 2
                        section.Text = "WinClass: " + cLass;
                        section.Tag = "WinClass|" + cLass;
                        parent.Nodes.Add(section);


                        TV.Nodes.Add(parent);  // populate tree

                        //if (MinimalView == true)
                        //    dt.Rows.Add(new object[] { false, theprocess.ProcessName, theprocess.MainWindowTitle, WinPositions.WinX, WinPositions.WinY, WinPositions.WinWidth, WinPositions.WinHeight });

                        //dt.Rows.Add(new object[] { false, theprocess.ProcessName, theprocess.MainWindowTitle, theprocess.Handle, theprocess.MainWindowHandle, WinPositions.WinX, WinPositions.WinY, WinPositions.WinW, WinPositions.WinH });
                    }
                    catch
                    {
                    }

                }
            }

            TV.Sort();
            if (ExpandTV) { Expand(TV); }

        }

        /// <summary>Add new node to TreeView</summary>
        /// <param name="TV"> </param>
        /// <param name="NodeName"> </param>
        /// <param name="NodeTag"> </param>
        /// <param name="NodeImage"> </param>
        /// <param name="BackColor"> </param>
        public void Node_Add(TreeView TV, string NodeName, string NodeTag = "", string NodeImage = "", string BackColor = "")
        {
            TreeNode parent = new TreeNode();  // level 1
            parent.Text = NodeName;
            parent.Tag = NodeTag;
            if (BackColor != "") { Color red = Color.FromName(BackColor); parent.BackColor = red; }
            //parent.ForeColor = ForeColor;

            TV.Nodes.Add(parent);  // populate tree
        }



        //public TabPage Find_Control(int i)
        //{
        //    var tabPage = FindControl("tabPage" + i);
        //    //tabControl.TabPages.Remove(tabPage); 
        //    return tabPage;
        //}

        //public static void SetEnabled(Control control, bool enabled)
        //{

        //    foreach (Control child in control.Controls)
        //    {
        //        SetEnabled(child, enabled);
        //    }
        //}     

        #endregion


        #region === TreeView: Return From Tree ===

        // Return Data From TreeView

        /// <summary>Returns list of nodes in TreeView (Option to Return Checked Only) - option to return nodes on certain level</summary>
        /// <param name="TV"> </param>
        /// <param name="CheckedOnly"> </param>
        /// <param name="NodeLevel"> </param>
        public List<TreeNode> NodeList(TreeView TV, bool CheckedOnly = false, int NodeLevel = -1)
        {
            ////Ex: 

            //List<TreeNode> results = tv.NodeList(treeView1, false); // return list of nodes (option for checked only)

            //foreach (TreeNode node in results)
            //{
            //    ahk.MsgBox(node.Text); 
            //}


            List<TreeNode> result = new List<TreeNode>();


            // create list of nodes to loop through
            List<TreeNode> nodeList = new List<TreeNode>();  // create list of all nodes in treeview to check
            foreach (TreeNode node in TV.Nodes) { nodeList.Add(node); }


            foreach (TreeNode node in nodeList)
            {

                if (NodeLevel != -1)
                {
                    if (node.Level == NodeLevel)
                    {
                        if (CheckedOnly)
                        {
                            if (node.Checked) { result.Add(node); }
                        }

                        if (!CheckedOnly) { result.Add(node); }
                    }
                    continue;
                }



                // only return values that are checked
                if (CheckedOnly)
                {
                    if (node.Checked)
                    {
                        result.Add(node);
                        List<TreeNode> kids = Nodes_Children(TV, node, CheckedOnly);
                        foreach (TreeNode kid in kids)
                        {
                            result.Add(kid);
                        }
                    }

                    if (!node.Checked)
                    {
                        List<TreeNode> kids = Nodes_Children(TV, node, CheckedOnly);
                        foreach (TreeNode kid in kids)
                        {
                            result.Add(kid);
                        }
                    }

                }

                // return all entries, checked + unchecked
                if (!CheckedOnly)
                {
                    //MessageBox.Show(node.Text);
                    result.Add(node);
                    List<TreeNode> kids = Nodes_Children(TV, node, CheckedOnly);
                    foreach (TreeNode kid in kids)
                    {
                        result.Add(kid);
                    }
                }

            }


            return result;
        }

        /// <summary>Returns list of nodes in TreeView (just the names of the nodes)</summary>
        /// <param name="TV"> </param>
        /// <param name="CheckedOnly"> </param>
        public List<string> Node_Names_List(TreeView TV, bool CheckedOnly = false)
        {
            ////Ex: 

            //List<TreeNode> results = tv.NodeList(treeView1, false); // return list of nodes (option for checked only)

            //foreach (TreeNode node in results)
            //{
            //    ahk.MsgBox(node.Text); 
            //}


            List<string> result = new List<string>();


            // create list of nodes to loop through
            List<TreeNode> nodeList = new List<TreeNode>();  // create list of all nodes in treeview to check
            foreach (TreeNode node in TV.Nodes) { nodeList.Add(node); }


            foreach (TreeNode node in nodeList)
            {
                // only return values that are checked
                if (CheckedOnly)
                {
                    if (node.Checked)
                    {
                        result.Add(node.Name);
                        List<TreeNode> kids = Nodes_Children(TV, node, CheckedOnly);
                        foreach (TreeNode kid in kids)
                        {
                            result.Add(kid.Name);
                        }
                    }

                    if (!node.Checked)
                    {
                        List<TreeNode> kids = Nodes_Children(TV, node, CheckedOnly);
                        foreach (TreeNode kid in kids)
                        {
                            result.Add(kid.Name);
                        }
                    }

                }

                // return all entries, checked + unchecked
                if (!CheckedOnly)
                {
                    //MessageBox.Show(node.Text);
                    result.Add(node.Name);
                    List<TreeNode> kids = Nodes_Children(TV, node, CheckedOnly);
                    foreach (TreeNode kid in kids)
                    {
                        result.Add(kid.Name);
                    }
                }

            }


            return result;
        }

        /// <summary>Returns the # of checked nodes in a TreeView</summary>
        /// <param name="TV"> </param>
        public int Checked_Node_Count(TreeView TV)
        {
            List<TreeNode> nodes = NodeList(TV, true);
            return nodes.Count();
        }

        /// <summary>Recurse through treeview nodes, return list of child nodes</summary>
        /// <param name="TV"> </param>
        /// <param name="treeNode"> </param>
        /// <param name="CheckedOnly"> </param>
        public List<TreeNode> Nodes_Children(TreeView TV, TreeNode treeNode, bool CheckedOnly = false)
        {
            List<TreeNode> kids = new List<TreeNode>();

            if (treeNode == null) { return null; }  //nothing to do if null value passed while user is clicking


            // update control text (from any thread) -- [ works in dll ]
            List<TreeNode> nodeList = new List<TreeNode>();  // create list of all nodes in treeview to check
            foreach (TreeNode tnz in treeNode.Nodes) { nodeList.Add(tnz); }


            // Print each child node recursively.
            foreach (TreeNode tn in nodeList)
            {
                // only return values that are checked
                if (CheckedOnly)
                {
                    if (tn.Checked)
                    {
                        kids.Add(tn);
                        List<TreeNode> subkids = Nodes_Children(TV, tn, CheckedOnly);
                        foreach (TreeNode kid in subkids)
                        {
                            kids.Add(kid);
                        }
                    }
                    if (!tn.Checked)
                    {
                        List<TreeNode> subkids = Nodes_Children(TV, tn, CheckedOnly);
                        foreach (TreeNode kid in subkids)
                        {
                            kids.Add(kid);
                        }
                    }

                }

                // return all entries, checked + unchecked
                if (!CheckedOnly)
                {
                    // Print the node.
                    //MessageBox.Show(tn.Text);
                    kids.Add(tn);
                    List<TreeNode> subkids = Nodes_Children(TV, tn, CheckedOnly);
                    foreach (TreeNode kid in subkids)
                    {
                        kids.Add(kid);
                    }

                }
            }



            return kids;
        }

        /// <summary> </summary>
        /// <param name="TV"> </param>
        /// <param name="NodeTag"> </param>
        /// <param name="Level"> </param>
        public List<string> Checked_Node_Text(TreeView TV, bool NodeTag = false, int Level = -1)  // return list of checked nodes in treeview (node text or optional tag), provide a level # to only pull checked from that level
        {
            List<string> checkedNodes = new List<string>();

            List<TreeNode> nodes = NodeList(TV, true);
            foreach (TreeNode node in nodes)
            {
                if (Level != -1)
                {
                    if (node.Level == Level)  // user provided a level # - only return checked from this level
                    {
                        string addText = node.Text;
                        if (NodeTag) { addText = node.Tag.ToString(); }
                        checkedNodes.Add(addText);
                    }
                }

                if (Level == -1)  // ignore level - just return all checked 
                {
                    string addText = node.Text;
                    if (NodeTag) { addText = node.Tag.ToString(); }
                    checkedNodes.Add(addText);
                }


            }

            return checkedNodes;
        }

        // Node Search

        /// <summary>Search For Node by Name, Returns Associated Node</summary>
        /// <param name="TV"> </param>
        /// <param name="SearchNode"> </param>
        public TreeNode ReturnNode(TreeView TV, string SearchNode)
        {
            TreeNode ReturnNode = null;
            List<TreeNode> results = NodeList(TV, false); // return list of nodes (option for checked only)

            foreach (TreeNode node in results)
            {
                if (SearchNode.ToUpper() == node.Text.ToUpper())
                {
                    TV.SelectedNode = node;
                    return node;
                }
            }

            return ReturnNode;
        }

        /// <summary>Search for Node by Name, includes parent search option</summary>
        /// <param name="TV"> </param>
        /// <param name="NodeText"> </param>
        /// <param name="ParentText"> </param>
        public TreeNode Find_Node(TreeView TV, string NodeText = "Using", string ParentText = "")
        {
            // search for the Using Node under Parent Text (loaded file name)
            TreeNode usingnode = null;
            List<TreeNode> ChildNodes = ReturnChildren(TV, ParentText);

            if (ChildNodes == null) { return null; }

            foreach (TreeNode noDE in ChildNodes)
            {
                if (noDE == null) { continue; }

                if (noDE.Text == NodeText)
                {
                    usingnode = noDE;
                    break;
                }
            }

            return usingnode;
        }

        /// <summary>Search all nodes in tree for node text, return matching node</summary>
        /// <param name="TV"> </param>
        /// <param name="NodeTag"> </param>
        /// <param name="ParentText"> </param>
        public TreeNode Node_Tag_Seach(TreeView TV, string NodeTag = "Tagged", string ParentText = "")
        {
            List<TreeNode> nodes = NodeList(TV, false);

            foreach (TreeNode node in nodes)
            {
                if (node.Tag == NodeTag)
                {
                    return node;
                }
            }

            return null;
        }

        /// <summary>Search For Node by Name, Returns True if Node Name Already Exists</summary>
        /// <param name="TV"> </param>
        /// <param name="SearchNode"> </param>
        public bool NodeExist(TreeView TV, string SearchNode)
        {
            List<TreeNode> results = NodeList(TV, false); // return list of nodes (option for checked only)

            foreach (TreeNode node in results)
            {
                if (SearchNode.ToUpper() == node.Text.ToUpper())
                {
                    TV.SelectedNode = node;
                    return true;
                }
            }

            return false;
        }

        /// <summary>Takes TreeNode and Returns Children Names</summary>
        /// <param name="TV"> </param>
        /// <param name="SearchNode"> </param>
        public List<TreeNode> ReturnChildren(TreeView TV, string SearchNode)
        {
            // ReturnChildren(treeView1, "Code Notes");  // takes TreeNode and Returns Children Names

            TreeNode node = ReturnNode(TV, SearchNode);  // search tree for node containing SearchNode text

            //List<TreeNode> returnlist = null;
            List<TreeNode> results = Nodes_Children(TV, node);  // return list of children under SearchNode

            //foreach (TreeNode nod in results)
            //{
            //    //ahk.MsgBox(nod.Text); 
            //    returnlist.Add(nod);
            //}


            //return returnlist;

            return results;
        }

        /// <summary>Returns # of Children Under Node</summary>
        /// <param name="TV"> </param>
        /// <param name="SearchNode"> </param>
        public int ChildrenCount(TreeView TV, TreeNode SearchNode)
        {
            List<TreeNode> results = Nodes_Children(TV, SearchNode);  // return list of children under SearchNode

            int ChildCount = 0;
            foreach (TreeNode nod in results)
            {
                ChildCount++;
            }

            return ChildCount;
        }

        /// <summary>Returns the Root Parent of the selected node</summary>
        /// <param name="node"> </param>
        public string Root_Node(TreeNode node)
        {
            string SelectedValue = node.Text;        // value of selected node
            string FullPath = node.FullPath.ToString();   // full path with all nodes + file name

            // === Extract the top/root node name from the full path === 
            string RootParent = "";
            string[] words = FullPath.Split('\\');
            int Index = 0;
            foreach (string word in words)
            {
                if (Index == 0)
                {
                    RootParent = word;
                    return RootParent;
                }

            }

            return "";
        }

        /// <summary>Returns true/false if SearchNode is found in TreeView</summary>
        /// <param name="TV"> </param>
        /// <param name="SearchNode"> </param>
        public bool NodeInTree(TreeView TV, string SearchNode)
        {
            //string SearchNode = "File.cs";

            List<TreeNode> results = NodeList(TV, false); // return list of nodes (option for checked only)

            foreach (TreeNode node in results)
            {
                if (SearchNode.ToUpper() == node.Text.ToUpper())
                {
                    //ahk.MsgBox("Found " + node.Text);
                    return true;
                }
            }

            return false;
        }


        // Remove Node

        /// <summary>Remove Node from TreeView based on Node Text</summary>
        /// <param name="TV">TreeView control to remove node from</param>
        /// <param name="NodeText">Either search by the Node Text or Node Tag - Display text on the node to search from and remove node</param>
        /// <param name="NodeTag">Either search by the Node Text or Node Tag - Node tag to search for and remove node. </param>
        public void Remove_Node(TreeView TV, string NodeText = "", string NodeTag = "")
        {
            List<TreeNode> results = NodeList(TV, false); // return list of nodes (option for checked only)

            foreach (TreeNode node in results)
            {
                if (NodeText != "")
                {
                    if (NodeText.ToUpper() == node.Text.ToUpper())
                    {
                        node.Remove();
                    }
                }

                if (NodeTag != "")
                {
                    if (NodeTag.ToUpper() == node.Tag.ToString().ToUpper())
                    {
                        node.Remove();
                    }
                }

            }
        }

        // Select / View

        /// <summary>Return the node user currently has selected/focused</summary>
        /// <param name="TV"> </param>
        public TreeNode Selected_Node(TreeView TV)
        {
            List<TreeNode> results = NodeList(TV, false); // return list of nodes (option for checked only)

            foreach (TreeNode node in results)
            {
                if (node.IsSelected) { return node; }  // return node that is selected
            }

            return null;
        }

        /// <summary>Select node in treeView programatically</summary>
        /// <param name="TV"> </param>
        /// <param name="SearchNode"> </param>
        public void SelectNode(TreeView TV, string SearchNode)
        {
            List<TreeNode> results = NodeList(TV, false); // return list of nodes (option for checked only)

            foreach (TreeNode node in results)
            {
                if (SearchNode.ToUpper() == node.Text.ToUpper())
                {
                    TV.SelectedNode = node;
                }
            }
        }

        /// <summary>Select # in treeView programatically</summary>
        /// <param name="TV"> </param>
        /// <param name="NodeNum"> </param>
        public void Select_First_Node(TreeView TV, int NodeNum = 1)
        {
            List<TreeNode> results = NodeList(TV, false); // return list of nodes (option for checked only)

            int i = 0;
            foreach (TreeNode node in results)
            {
                i++;
                if (i == NodeNum)
                {
                    TV.SelectedNode = node;
                }
            }
        }

        /// <summary>Select next visible node programatically</summary>
        /// <param name="TV"> </param>
        public void Select_Next_Node(TreeView TV)
        {
            _Lists lst = new _Lists();

            TreeNode node = TV.SelectedNode;

            ////string options = "Next";
            string options = "NextVisible";

            // Determine which TreeNode to select.
            switch (options)
            {
                case "Previous":
                    node.TreeView.SelectedNode = node.PrevNode;
                    break;
                case "PreviousVisible":
                    node.TreeView.SelectedNode = node.PrevVisibleNode;
                    break;
                case "Next":
                    node.TreeView.SelectedNode = node.NextNode;
                    break;
                case "NextVisible":
                    node.TreeView.SelectedNode = node.NextVisibleNode;
                    break;
                case "First":
                    node.TreeView.SelectedNode = node.FirstNode;
                    break;
                case "Last":
                    node.TreeView.SelectedNode = node.LastNode;
                    break;
            }

            node.TreeView.Focus();
        }

        /// <summary>Select previous visible node programatically</summary>
        /// <param name="TV"> </param>
        public void Select_Previous_Node(TreeView TV)
        {
            _Lists lst = new _Lists();

            TreeNode node = TV.SelectedNode;

            ////string options = "Previous";
            string options = "PreviousVisible";

            // Determine which TreeNode to select.
            switch (options)
            {
                case "Previous":
                    node.TreeView.SelectedNode = node.PrevNode;
                    break;
                case "PreviousVisible":
                    node.TreeView.SelectedNode = node.PrevVisibleNode;
                    break;
                case "Next":
                    node.TreeView.SelectedNode = node.NextNode;
                    break;
                case "NextVisible":
                    node.TreeView.SelectedNode = node.NextVisibleNode;
                    break;
                case "First":
                    node.TreeView.SelectedNode = node.FirstNode;
                    break;
                case "Last":
                    node.TreeView.SelectedNode = node.LastNode;
                    break;
            }

            node.TreeView.Focus();
        }

        /// <summary>Remove nodes without children (setting the level of nodes deep to start before removing children)</summary>
        /// <param name="TV"> </param>
        /// <param name="SearchNode"> </param>
        /// <param name="Level"> </param>
        public void Remove_Empty_Children_Nodes(TreeView TV, string SearchNode, int Level)
        {
            List<TreeNode> kids = ReturnChildren(TV, SearchNode);   // takes TreeNode and Returns Children Names

            if (kids != null)  // make sure the node exists
            {
                foreach (TreeNode node in kids)
                {
                    if (node.Level == Level)
                    {
                        int ChildCount = ChildrenCount(TV, node);
                        if (ChildCount == 0)
                        {
                            node.Remove();
                        }
                    }
                }
            }
        }

        #endregion


        #region === TreeView: Display ===

        /// <summary>Clear all nodes on TreeView</summary>
        /// <param name="TV"> </param>
        public void ClearTV(TreeView TV)
        {
            // update control text (from any thread)
            if (TV.InvokeRequired) { TV.BeginInvoke((MethodInvoker)delegate () { TV.Nodes.Clear(); ; }); }
            else { TV.Nodes.Clear(); }
        }

        /// <summary>Toggle viewing treeview control</summary>
        /// <param name="TV"> </param>
        public void ToggleView(TreeView TV)
        {
            bool Visible = false;

            // update control text (from any thread)
            if (TV.InvokeRequired) { TV.BeginInvoke((MethodInvoker)delegate () { Visible = TV.Visible; }); }
            else { Visible = TV.Visible; }

            // update control text (from any thread)
            if (TV.InvokeRequired) { TV.BeginInvoke((MethodInvoker)delegate () { if (Visible) { TV.Visible = false; return; } if (!Visible) { TV.Visible = true; return; } }); }
            else { if (Visible) { TV.Visible = false; return; } if (!Visible) { TV.Visible = true; return; } }
        }

        /// <summary>Expand All Nodes in TreeView</summary>
        /// <param name="TV"> </param>
        public void Expand(TreeView TV)
        {
            // expand search results in tree
            if (TV.InvokeRequired)
            {
                TV.BeginInvoke((MethodInvoker)delegate () { TV.ExpandAll(); });
            }
            else
            {
                TV.ExpandAll();
            }
        }

        /// <summary>
        /// Expand TreeNodes Below NodeLevel 
        /// </summary>
        /// <param name="TV">TreeView Control</param>
        /// <param name="NodeLevel">Level of Nodes to Expand</param>
        public void Expand_Level(TreeView TV, int NodeLevel = 0)
        {
            List<TreeNode> results = NodeList(TV, false); // return list of nodes (option for checked only)

            foreach (TreeNode node in results)
            {
                if (node.Level <= NodeLevel)
                {
                    node.Expand();
                }
                if (node.Level > NodeLevel)
                {
                    node.Collapse();
                }
            }
        }

        /// <summary>Collapse All Nodes in TreeView</summary>
        /// <param name="TV"> </param>
        public void Collapse(TreeView tv)
        {
            tv.CollapseAll();
        }

        /// <summary>Expand Child Nodes under SearchNode Name</summary>
        /// <param name="TV"> </param>
        /// <param name="SearchNode"> </param>
        public void Expand_Children(TreeView TV, string SearchNode)
        {
            TreeNode node = ReturnNode(TV, SearchNode);  // search tree for node containing SearchNode text

            List<TreeNode> results = Nodes_Children(TV, node);  // return list of children under SearchNode

            foreach (TreeNode nod in results)
            {
                if (node.IsExpanded) { node.Collapse(); return; }
                if (!node.IsExpanded) { TV.CollapseAll(); node.Expand(); return; }
            }

        }

        /// <summary>Populate TreeView Icons from ImageList</summary>
        /// <param name="TV"> </param>
        /// <param name="imageList"> </param>
        public void Populate_TV_Images(TreeView TV, ImageList imageList = null)
        {
            //// populate imagelist to assign to treeview
            //string ICODir = ahk.DevRoot() + "\\ICO_Lib";
            //ImageList imgList = img.ImageList(ICODir);  // populate imagelist


            if (imageList == null)  // populate imagelist with default if no list provided
            {
                _Images img = new _Images();
                imageList = img.ImageList(ahk.AppDir() + "\\ICO_Lib\\FlatIcon");
            }

            // Assign the ImageList to the TreeView.
            TV.ImageList = imageList;

            //int ImageCount = imageList.Images.Count;  // # of images in imagelist


            List<TreeNode> results = NodeList(TV, false); // return list of nodes (option for checked only)

            foreach (TreeNode node in results)
            {
                //===========================
                // Set Default Node Images
                //===========================

                //==== Change Parent Node Icon =============

                if (node.Level == 0)
                {
                    node.StateImageIndex = imageList.Images.IndexOfKey("folder1");
                    node.ImageIndex = imageList.Images.IndexOfKey("folder1");
                    node.SelectedImageIndex = imageList.Images.IndexOfKey("folder1");
                }

                if (node.Level > 0)
                {
                    node.ImageIndex = imageList.Images.IndexOfKey("folder10");
                    node.SelectedImageIndex = imageList.Images.IndexOfKey("folder10");

                }

                //=====================================================
                // Set TreeNode Images Based on Selected FileType
                //=====================================================


                if (node.Text.ToUpper().Contains(".SQLITE"))
                {
                    node.ImageIndex = imageList.Images.IndexOfKey("SQL");
                    node.SelectedImageIndex = imageList.Images.IndexOfKey("SQL");
                }
                if (node.Text.ToUpper().Contains(".AHK"))
                {
                    node.ImageIndex = imageList.Images.IndexOfKey("AHK");
                    node.SelectedImageIndex = imageList.Images.IndexOfKey("AHK");
                }

                if (node.Text.ToUpper().Contains(".CS"))
                {
                    //==== Change Node Icon =============
                    node.ImageIndex = imageList.Images.IndexOfKey("CS");
                    node.SelectedImageIndex = imageList.Images.IndexOfKey("CS");
                }
                if (node.Text.ToUpper().Contains(".TXT"))
                {
                    //==== Change Node Icon =============
                    node.ImageIndex = imageList.Images.IndexOfKey("TXT");
                    node.SelectedImageIndex = imageList.Images.IndexOfKey("TXT");
                }


            }

        }

        /// <summary>Reset the color back to black text / white bg for all nodes in tree</summary>
        /// <param name="TV"> </param>
        /// <param name="font"> </param>
        /// <param name="back"> </param>
        public void Node_Colors(TreeView TV, Color font, Color back)
        {
            List<TreeNode> results = NodeList(TV, false); // return list of nodes (option for checked only)

            foreach (TreeNode node in results)
            {
                node.ForeColor = font;
                node.BackColor = back;
                //TV.BackColor = Color.PeachPuff;
            }
        }

        /// <summary>Reset the color back to black text / white bg for all nodes in tree</summary>
        /// <param name="TV"> </param>
        public void Reset_Node_Colors(TreeView TV)
        {
            List<TreeNode> results = NodeList(TV, false); // return list of nodes (option for checked only)

            foreach (TreeNode node in results)
            {
                node.ForeColor = Color.Black;
                node.BackColor = Color.White;
            }
        }

        // CheckBoxes

        /// <summary>Check node based on NodeName Search</summary>
        /// <param name="TV"> </param>
        /// <param name="SearchNode"> </param>
        public void Check_Node(TreeView TV, string SearchNode)
        {
            List<TreeNode> results = NodeList(TV, false); // return list of nodes (option for checked only)

            foreach (TreeNode node in results)
            {
                if (SearchNode.ToUpper() == node.Text.ToUpper())
                {
                    node.Checked = true;
                    node.ForeColor = Color.Orange;
                }
            }
        }

        /// <summary>User provides list of nodes to search for and tag if found in TreeView (used for Tag Management)</summary>
        /// <param name="TV"> </param>
        /// <param name="CheckList"> </param>
        /// <param name="UnCheckAllFirst"> </param>
        public void Check_Nodes_From_List(TreeView TV, List<string> CheckList, bool UnCheckAllFirst = true)
        {
            if (UnCheckAllFirst) { UnCheck_All(TV); }  // option to uncheck all items in tree before comparing to check list


            List<TreeNode> results = NodeList(TV, false); // return list of nodes (option for checked only)

            foreach (string item in CheckList)
            {
                foreach (TreeNode node in results)
                {
                    if (item.Trim().ToUpper() == node.Text.Trim().ToUpper())
                    {
                        node.Checked = true;
                    }
                }

            }


        }

        /// <summary>Toggle viewing checkboxes in treeview</summary>
        /// <param name="TV"> </param>
        public void Toggle_CheckBoxes(TreeView TV)
        {
            if (TV.CheckBoxes) { TV.CheckBoxes = false; return; }
            if (!TV.CheckBoxes) { TV.CheckBoxes = true; return; }
        }

        /// <summary>Check all nodes in treeview</summary>
        /// <param name="TV"> </param>
        public void Check_All(TreeView TV)
        {
            List<TreeNode> results = NodeList(TV, false); // return list of nodes (option for checked only)

            foreach (TreeNode node in results)
            {
                node.Checked = true;
            }
        }

        /// <summary>Uncheck all nodes in treeview</summary>
        /// <param name="TV"> </param>
        public void UnCheck_All(TreeView TV)
        {
            List<TreeNode> results = NodeList(TV, false); // return list of nodes (option for checked only)

            foreach (TreeNode node in results)
            {
                node.Checked = false;
                node.ForeColor = Color.Black;
            }
        }

        /// <summary>Checks All Children Under SearchNode</summary>
        /// <param name="TV"> </param>
        /// <param name="SearchNode"> </param>
        public void Check_Children(TreeView TV, string SearchNode)
        {
            TreeNode node = ReturnNode(TV, SearchNode);  // search tree for node containing SearchNode text

            List<TreeNode> results = Nodes_Children(TV, node);  // return list of children under SearchNode

            foreach (TreeNode nod in results)
            {
                nod.Checked = true;
            }
        }

        /// <summary>Checks All Children Under SearchNode (using Node ref instead of Searching For Node)</summary>
        /// <param name="TV"> </param>
        /// <param name="node"> </param>
        public void Check_Children_UnderNode(TreeView TV, TreeNode node)
        {
            List<TreeNode> results = Nodes_Children(TV, node);  // return list of children under SearchNode

            foreach (TreeNode nod in results)
            {
                nod.Checked = true;
            }
        }

        /// <summary>UnChecks All Children Under SearchNode</summary>
        /// <param name="TV"> </param>
        /// <param name="SearchNode"> </param>
        public void UnCheck_Children(TreeView TV, string SearchNode)
        {
            TreeNode node = ReturnNode(TV, SearchNode);  // search tree for node containing SearchNode text

            try
            {
                List<TreeNode> results = Nodes_Children(TV, node);  // return list of children under SearchNode

                foreach (TreeNode nod in results)
                {
                    try { nod.Checked = false; }
                    catch { }
                }

                ahk.Sleep("200");
            }

            catch
            { }
        }

        /// <summary>UnChecks All Children Under SearchNode (using Node ref instead of Searching For Node)</summary>
        /// <param name="TV"> </param>
        /// <param name="node"> </param>
        public void UnCheck_Children_UnderNode(TreeView TV, TreeNode node)
        {
            try
            {
                List<TreeNode> results = Nodes_Children(TV, node);  // return list of children under SearchNode

                foreach (TreeNode nod in results)
                {
                    try { nod.Checked = false; }
                    catch { }
                }

                ahk.Sleep("200");
            }

            catch
            { }
        }

        /// <summary>Checks the first node under each parent in treeview control</summary>
        /// <param name="TV"> </param>
        public void Check_First_Child(TreeView TV)
        {
            // loop through the control type options available for this project, check the last used control name by type or the first control under the node

            foreach (TreeNode nodeSearch in TV.Nodes)
            {
                bool LastUsedFound = false;

                if (!LastUsedFound)  // otherwise, check the first control under each node
                {
                    int childnum = 0;
                    foreach (TreeNode tn in nodeSearch.Nodes)   // Print each child node recursively.
                    {
                        if (childnum == 0) { tn.Checked = true; TV.SelectedNode = tn; }
                        childnum++;
                    }
                }
            }
        }

        /// <summary>Checks the first node in a treeview control</summary>
        /// <param name="TV"> </param>
        public void Check_First_Node(TreeView TV)
        {
            // loop through the control type options available for this project, check the last used control name by type or the first control under the node

            int childnum = 0;
            foreach (TreeNode nodeSearch in TV.Nodes)
            {
                if (childnum == 0) { nodeSearch.Checked = true; TV.SelectedNode = nodeSearch; }
                childnum++;
            }

        }

        /// <summary>Scroll to top node in tree</summary>
        /// <param name="TV"> </param>
        public void Scroll_To_Top_Node(TreeView TV)
        {
            TV.Nodes[TV.Nodes.Count - 1].EnsureVisible();
        }

        /// <summary>Scroll to bottom node in tree  (ToDo: Not Working Yet)</summary>
        /// <param name="TV"> </param>
        public void Scroll_To_Bottom_Node(TreeView TV)
        {
            TV.Nodes[TV.Nodes.Count].EnsureVisible();
        }


        #endregion


        #region === TreeView: Search ===

        // Search TreeView   -- ResultsHaveFileExt = requires the results to have a "." in the name to stay - otherwise a folder to remove

        /// <summary>Removes all nodes that don't contain search text (returns # of children nodes left)</summary>
        /// <param name="TV"> </param>
        /// <param name="SearchText"> </param>
        /// <param name="ResultsHaveFileExt"> </param>
        /// <param name="RemoveUnderLevel"> </param>
        public int TreeSearch(TreeView TV, string SearchText, bool ResultsHaveFileExt = false, int RemoveUnderLevel = -1)
        {
            // loop through nodes, remove nodes that don't have search text

            List<TreeNode> results = NodeList(TV, false); // return list of nodes

            foreach (TreeNode node in results)
            {
                int ChildCount = ChildrenCount(TV, node);
                if (ChildCount > 0) { continue; }

                string NodeText = node.Text.ToUpper();

                if (!NodeText.Contains(SearchText.ToUpper())) { node.Remove(); }  // remove node if search text not found
            }


            if (ResultsHaveFileExt)
            {
                // loop through nodes one more time, remove the parent nodes that no longer have children

                List<TreeNode> results2 = NodeList(TV, false); // return list of nodes
                foreach (TreeNode node in results2)
                {
                    //if (node.Level == 0)
                    int ChildCount = ChildrenCount(TV, node);
                    if (ChildCount == 0)
                    {
                        if (!node.Text.Contains('.')) { node.Remove(); }  // if this isn't a file format, remove
                    }
                }
            }

            // remove nodes if there are no childen under level #
            if (RemoveUnderLevel != -1)
            {
                // loop through nodes one more time, remove the parent nodes that no longer have children

                List<TreeNode> results3 = NodeList(TV, false); // return list of nodes
                foreach (TreeNode node in results3)
                {
                    if (node.Level == RemoveUnderLevel)
                    {
                        int ChildCount = ChildrenCount(TV, node);
                        if (ChildCount == 0)
                        {
                            node.Remove();
                        }
                    }
                }
            }



            List<TreeNode> nodeCount = NodeList(TV, false); // return list of nodes
            int count = 0;
            foreach (TreeNode node in nodeCount)
            {
                //if (node.Level == 0)
                int ChildCount = ChildrenCount(TV, node);
                count += ChildCount;
            }


            TV.ExpandAll(); // expand search results in tree


            return count;
        }

        /// <summary>Returns list of checked nodes (string list) - Node Text (or Node Tag)</summary>
        /// <param name="TV"> </param>
        /// <param name="ReturnTag"> </param>
        public List<string> Return_Checked(TreeView TV, bool ReturnTag = false)
        {
            List<string> Checked_Nodes = new List<string>();
            List<TreeNode> results = NodeList(TV, true); // return list of checked nodes
            foreach (TreeNode node in results)
            {
                if (!ReturnTag) { Checked_Nodes.Add(node.Text); }
                if (ReturnTag) { Checked_Nodes.Add(node.Tag.ToString()); }
            }

            return Checked_Nodes;
        }

        /// <summary>Return the text from the first checked item in a treeview</summary>
        /// <param name="TV"> </param>
        public string Return_First_CheckedText(TreeView TV)
        {
            string ReturnNode = "";

            List<TreeNode> results = NodeList(TV, true); // return list of checked nodes
            foreach (TreeNode node in results)
            {
                return node.Text;
            }

            return ReturnNode;
        }

        /// <summary>Return the text from the last checked item in a treeview</summary>
        /// <param name="TV"> </param>
        public string Return_Last_CheckedText(TreeView TV)
        {
            string ReturnNode = "";

            List<TreeNode> results = NodeList(TV, true); // return list of checked nodes
            foreach (TreeNode node in results)
            {
                ReturnNode = node.Text;
            }

            return ReturnNode;
        }

        #endregion


        #region === TreeView: Close / Copy / Backup Nodes ====

        /// <summary>Copy nodes from one TreeView to another</summary>
        /// <param name="Source"> </param>
        /// <param name="Destination"> </param>
        public void Copy_TreeNodes(TreeView Source, TreeView Destination)
        {
            Destination.Nodes.Clear();

            foreach (TreeNode tn in Source.Nodes)
            {
                TreeNode newNode = new TreeNode(tn.Text, tn.ImageIndex, tn.SelectedImageIndex);
                newNode.Name = tn.Name;
                newNode.Tag = tn.Tag;
                newNode.Checked = tn.Checked;

                CopyChildren(newNode, tn);
                Destination.Nodes.Add(newNode);
            }
        }

        /// <summary>Used to Copy nodes from one TreeView to another </summary>
        /// <param name="parent"> </param>
        /// <param name="original"> </param>
        private void CopyChildren(TreeNode parent, TreeNode original)
        {
            foreach (TreeNode tn in original.Nodes)
            {
                TreeNode newNode = new TreeNode(tn.Text, tn.ImageIndex, tn.SelectedImageIndex);
                newNode.Name = tn.Name;
                newNode.Tag = tn.Tag;
                newNode.Checked = tn.Checked;

                parent.Nodes.Add(newNode);
                CopyChildren(newNode, tn);
            }
        }

        #endregion



        #region === TreeView Behavior: Check Children Under Parent Node ===

        /// <summary>Add the event handler to check children nodes if parent node is checked (parameter to set the Level considered Parent)</summary>
        /// <param name="TV"> </param>
        /// <param name="ParentLevelToCheckUnder"> </param>
        /// <param name="EnableCheckBoxes"> </param>
        public void Enable_Check_Under_ParentNode(TreeView TV, int ParentLevelToCheckUnder = 1, bool EnableCheckBoxes = true)
        {
            if (EnableCheckBoxes) { TV.CheckBoxes = true; }

            TV.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(treeView_AfterCheck_Children);
            //TV.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);

            Set_ParentNodeLevel(ParentLevelToCheckUnder);
        }

        /// <summary>Add the event handler to check children nodes if parent node is checked (parameter to set the Level considered Parent)</summary>
        /// <param name="TV"> </param>
        /// <param name="ParentLevelToCheckUnder"> </param>
        /// <param name="EnableCheckBoxes"> </param>
        public void Disable_Check_Under_ParentNode(TreeView TV, int ParentLevelToCheckUnder = 1, bool EnableCheckBoxes = true)
        {
            if (EnableCheckBoxes) { TV.CheckBoxes = true; }

            TV.AfterCheck -= new System.Windows.Forms.TreeViewEventHandler(treeView_AfterCheck_Children);
            //TV.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_NodeMouseDoubleClick);

            //Set_ParentNodeLevel(ParentLevelToCheckUnder);
        }


        //==== After TV Item Checked ====

        /// <summary>Parameter used by treeView_AfterCheck_Children - determines the level # to apply action to</summary>
        /// <param name="ParentLevelToCheckUnder"> </param>
        public void Set_ParentNodeLevel(int ParentLevelToCheckUnder)
        {
            _ParentNodeLevel = ParentLevelToCheckUnder;
        }

        public int _ParentNodeLevel = 0;

        /// <summary>Event Handler Action to check children nodes under set parentlevel node</summary>
        /// <param name="sender"> </param>
        /// <param name=" TreeViewEventArgs e"> </param>
        private void treeView_AfterCheck_Children(object sender, TreeViewEventArgs e)
        {
            TreeView TV = (TreeView)sender;

            // === Node Checked / Unchecked
            TreeNode node = e.Node;
            if (node == null) { return; }  //nothing to do if null value passed while user is clicking

            string CheckedText = e.Node.Text;
            string RootNode = Root_Node(node);
            //if (RootNode.ToUpper() == "DATAGRIDVIEW1" || node.Parent.Text.ToUpper() == "DATAGRIDVIEW1")  // toggle displaying columns in gridview based on treeview selections

            if (node.Level <= _ParentNodeLevel)  // check children under node when checked
            {
                if (node.Checked) { Check_Children_UnderNode(TV, node); return; }
                if (!node.Checked) { UnCheck_Children_UnderNode(TV, node); return; }
            }
        }


        #endregion

        #region === TreeView Behavior: Hide CheckBox on Node Level # ===

        // #### Hide CheckBox On Node Level X #####

        // Add To Starup: 
        //      tv.Setup_Hide_NodeLevel_CheckBox(treeView1, 1, true);  // enable 
        //      
        //      Elsewhere: 
        //          tv.Set_Hide_NodeLevel(0);  // node level # to hide on next redraw event 


        // constants used to hide a checkbox
        public const int TVIF_STATE = 0x8;
        public const int TVIS_STATEIMAGEMASK = 0xF000;
        public const int TV_FIRST = 0x1100;
        public const int TVM_SETITEM = TV_FIRST + 63;

        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam,
        IntPtr lParam);

        // struct used to set node properties
        public struct TVITEM
        {
            public int mask;
            public IntPtr hItem;
            public int state;
            public int stateMask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public String lpszText;
            public int cchTextMax;
            public int iImage;
            public int iSelectedImage;
            public int cChildren;
            public IntPtr lParam;

        }


        public int _Hide_CheckBox_Level;

        /// <summary> </summary>
        /// <param name="TV"> </param>
        /// <param name="HideCheckBoxLevel"> </param>
        /// <param name="EnableCheckBoxes"> </param>
        public void Setup_Hide_NodeLevel_CheckBox(TreeView TV, int HideCheckBoxLevel = 1, bool EnableCheckBoxes = true)  // add the event handler to allow for removing the CheckBox from certain Level #
        {
            if (EnableCheckBoxes) { TV.CheckBoxes = true; }

            TV.DrawMode = TreeViewDrawMode.OwnerDrawText;
            TV.DrawNode += new DrawTreeNodeEventHandler(tree_DrawNode);

            Set_Hide_NodeLevel(HideCheckBoxLevel);
        }

        /// <summary> </summary>
        /// <param name="NodeLevel"> </param>
        public void Set_Hide_NodeLevel(int NodeLevel = 1)  // stores the node level # to hide on next redraw event 
        {
            _Hide_CheckBox_Level = NodeLevel;
        }

        /// <summary> </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        void tree_DrawNode(object sender, DrawTreeNodeEventArgs e)  // ReDraw Tree Without CheckBoxes at Level #
        {
            TreeView TV = (TreeView)sender;

            if (e.Node.Level == _Hide_CheckBox_Level)
            {
                Hide_NodeLevel_CheckBox(e.Node);
                e.DrawDefault = true;
            }
            else
            {
                e.Graphics.DrawString(e.Node.Text, e.Node.TreeView.Font,
                   Brushes.Black, e.Node.Bounds.X, e.Node.Bounds.Y);
            }

        }

        /// <summary> </summary>
        /// <param name="node"> </param>
        private void Hide_NodeLevel_CheckBox(TreeNode node)
        {
            TVITEM tvi = new TVITEM();
            tvi.hItem = node.Handle;
            tvi.mask = TVIF_STATE;
            tvi.stateMask = TVIS_STATEIMAGEMASK;
            tvi.state = 0;
            IntPtr lparam = Marshal.AllocHGlobal(Marshal.SizeOf(tvi));
            Marshal.StructureToPtr(tvi, lparam, false);
            SendMessage(node.TreeView.Handle, TVM_SETITEM, IntPtr.Zero, lparam);
        }


        #endregion

        #region === TreeView Behavior: Show Tag On Node MouseOver ===

        // Kinda Works - tooltip shows too close to the node (ToDo: improve)

        /// <summary> </summary>
        /// <param name="TV"> </param>
        public void Enable_Node_ToolTips(TreeView TV)  // add the event handler to check children nodes if parent node is checked (parameter to set the Level considered Parent)
        {

            TV.MouseMove += new System.Windows.Forms.MouseEventHandler(treeView_MouseMove_ShowToolTipOnNodes);


            ToolTip ttip = new ToolTip();
            ttip.IsBalloon = true;
            ttip.UseFading = true;
            ttip.UseAnimation = true;

            _ttip = ttip;
        }

        /// <summary> </summary>
        /// <param name="TV"> </param>
        public void Disable_Node_ToolTips(TreeView TV)  // add the event handler to check children nodes if parent node is checked (parameter to set the Level considered Parent)
        {

            TV.MouseMove -= new System.Windows.Forms.MouseEventHandler(treeView_MouseMove_ShowToolTipOnNodes);
        }


        ToolTip _ttip;

        /// <summary> </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        public void treeView_MouseMove_ShowToolTipOnNodes(object sender, MouseEventArgs e)
        {
            TreeView TV = (TreeView)sender;

            // Get the node at the current mouse pointer location.
            TreeNode theNode = TV.GetNodeAt(e.X, e.Y);

            // Set a ToolTip only if the mouse pointer is actually paused on a node.
            if ((theNode != null))
            {
                // Verify that the tag property is not "null".
                if (theNode.Tag != null)
                {
                    // Change the ToolTip only if the pointer moved to a new node.
                    if (theNode.Tag.ToString() != _ttip.GetToolTip(TV))
                    {
                        _ttip.SetToolTip(TV, theNode.Tag.ToString());
                    }
                }
                else
                {
                    _ttip.SetToolTip(TV, "");
                }
            }
            else     // Pointer is not over a node so clear the ToolTip.
            {
                _ttip.SetToolTip(TV, "");
            }
        }

        // mouse move function that grabs Node under Mouse + Action

        //ToolTip tt = new ToolTip();

        //private void toolStripStatusLabel1_MouseHover(object sender, EventArgs e) 
        //{
        //          tt.Show("This is my tool tip", statusStrip1, new Point(toolStripStatusLabel1.Bounds.Right, toolStripStatusLabel1.Bounds.Top - 10));
        //}

        //private void toolStripStatusLabel1_MouseLeave(object sender, EventArgs e) { tt.Hide(statusStrip1); }

        #endregion

        /// <summary> </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        public void TESTING_treeView_MouseMove_ShowToolTipOnNodes(object sender, MouseEventArgs e)
        {
            TreeView TV = (TreeView)sender;

            Color NewColor = Color.CornflowerBlue;


            // Get the node at the current mouse pointer location.
            TreeNode theNode = TV.GetNodeAt(e.X, e.Y);


            // Set a ToolTip only if the mouse pointer is actually paused on a node.
            if ((theNode != null))
            {
                if (theNode.BackColor != NewColor)
                {
                    Reset_Node_Colors(TV);
                    theNode.BackColor = NewColor;
                }
            }
        }


        // add tags with full file paths when loading directories
        class NodeTag
        {
            public NodeTag(string FilePath)  // not in use
            {
                NodePath = FilePath;
                //Direction = direction;
            }
            //public string Direction { get; set; }
            public string NodePath { get; set; }
        }

        //// ex:
        //TreeNode node = new TreeNode();
        //node.Tag = new NodeTag(FilePath);
        //treeView1.Nodes.Add(node);

    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace sharpAHK_Dev
{

    public partial class _Code  // parse code functions
    {

        #region === Project Settings - Parse/Display ===

        /// <summary> List of Settings Names From Project</summary>
        /// <param name="Settings"> </param>
        public List<string> Project_Settings_List(string Settings = "")
        {
            Settings = @"C:\Users\jason\Google Drive\IMDB\SQLiter\Dynamic_Coder\Properties\Settings.settings";

            if (File.Exists(Settings)) { Settings = ahk.FileRead(Settings); }

            List<string> Lines = lst.TextFile_To_List(Settings);

            List<string> SettingList = new List<string>();

            //<Settings>
            //  <Setting Name="Load_Code_Bin" Type="System.Boolean" Scope="User">
            //  <Value Profile="(Default)">True</Value>
            //</Setting>

            string SettingName = "";
            string Type = "";
            string Scope = "";
            string DefaultValue = "";

            int counter = 0;
            foreach (string line in Lines)
            {
                if (line.Contains("<Setting Name="))
                {

                    SettingName = ahk.StringSplit(line, "=", 1);
                    SettingName = ahk.StringReplace(SettingName, "Type");
                    SettingName = ahk.StringReplace(SettingName, "\"");
                    SettingName = SettingName.Trim();

                    Type = ahk.StringSplit(line, "=", 2);
                    Type = ahk.StringReplace(Type, "Scope");
                    Type = ahk.StringReplace(Type, "\"");
                    Type = Type.Trim();

                    Scope = ahk.StringSplit(line, "=", 3);
                    Scope = ahk.StringReplace(Scope, ">");
                    Scope = ahk.StringReplace(Scope, "\"");
                    Scope = Scope.Trim();

                    SettingList.Add(SettingName);

                    //ahk.MsgBox("Setting: " + SettingName + Environment.NewLine + "Type: " + Type + Environment.NewLine + "Scope: " + Scope);
                }

                if (line.Contains("<Value Profile="))
                {
                    DefaultValue = ahk.StringSplit(line, ">", 1);
                    DefaultValue = ahk.StringSplit(DefaultValue, "<", 0);
                    DefaultValue = DefaultValue.Trim();
                    if (DefaultValue == "") { DefaultValue = " "; }
                    //ahk.MsgBox("Default Value: " + DefaultValue); 
                }

                if (SettingName != "" && DefaultValue != "")
                {
                    counter++;

                    ahk.MsgBox("Setting (" + counter.ToString() + ") : " + SettingName + Environment.NewLine + "Type: " + Type + Environment.NewLine + "Scope: " + Scope + Environment.NewLine + "Default Value: " + DefaultValue);

                    SettingName = "";
                    Type = "";
                    Scope = "";
                    DefaultValue = "";
                }

            }


            return SettingList;
        }

        /// <summary> Parse Properties\Settings.settings" File from Visual Studio Project - Defines Default Values</summary>
        /// <param name="DataGridView DV"> </param>
        /// <param name="Settings"> </param>
        public DataTable Settings_Config_Table(DataGridView DV, string Settings = "")
        {
            if (Settings == "") { Settings = @"C:\Users\jason\Google Drive\IMDB\SQLiter\Dynamic_Coder\Properties\Settings.settings"; }

            if (File.Exists(Settings)) { Settings = ahk.FileRead(Settings); }

            List<string> Lines = lst.TextFile_To_List(Settings);

            //<Settings>
            //  <Setting Name="Load_Code_Bin" Type="System.Boolean" Scope="User">
            //  <Value Profile="(Default)">True</Value>
            //</Setting>

            // Here we create a DataTable with four columns.
            DataTable ConfigTable = new DataTable();
            ConfigTable.Columns.Add("SettingName", typeof(string));
            ConfigTable.Columns.Add("Type", typeof(string));
            ConfigTable.Columns.Add("Scope", typeof(string));
            ConfigTable.Columns.Add("DefaultValue", typeof(string));


            string SettingName = "";
            string Type = "";
            string Scope = "";
            string DefaultValue = "";

            int counter = 0;
            foreach (string line in Lines)
            {
                if (line.Contains("<Setting Name="))
                {

                    SettingName = ahk.StringSplit(line, "=", 1);
                    SettingName = ahk.StringReplace(SettingName, "Type");
                    SettingName = ahk.StringReplace(SettingName, "\"");
                    SettingName = SettingName.Trim();

                    Type = ahk.StringSplit(line, "=", 2);
                    Type = ahk.StringReplace(Type, "Scope");
                    Type = ahk.StringReplace(Type, "\"");
                    Type = Type.Trim();

                    Scope = ahk.StringSplit(line, "=", 3);
                    Scope = ahk.StringReplace(Scope, ">");
                    Scope = ahk.StringReplace(Scope, "\"");
                    Scope = Scope.Trim();


                    //ahk.MsgBox("Setting: " + SettingName + Environment.NewLine + "Type: " + Type + Environment.NewLine + "Scope: " + Scope);
                }

                if (line.Contains("<Value Profile="))
                {
                    DefaultValue = ahk.StringSplit(line, ">", 1);
                    DefaultValue = ahk.StringSplit(DefaultValue, "<", 0);
                    DefaultValue = DefaultValue.Trim();
                    if (DefaultValue == "") { DefaultValue = " "; }
                    //ahk.MsgBox("Default Value: " + DefaultValue); 
                }

                if (SettingName != "" && DefaultValue != "")
                {
                    counter++;

                    //ahk.MsgBox("Setting (" + counter.ToString() + ") : " + SettingName + Environment.NewLine + "Type: " + Type + Environment.NewLine + "Scope: " + Scope + Environment.NewLine + "Default Value: " + DefaultValue);

                    ConfigTable.Rows.Add(SettingName, Type, Scope, DefaultValue);

                    SettingName = "";
                    Type = "";
                    Scope = "";
                    DefaultValue = "";
                }

            }

            DV.DataSource = ConfigTable;

            return ConfigTable;
        }

        /// <summary>  Enter the main project.cs file - loads  app.config</summary>
        /// <param name="DataGridView DV"> </param>
        /// <param name=" ProjectFile"> </param>
        public DataTable Saved_Settings_Table(DataGridView DV, string ProjectFile)
        {
            //if (AppConfig == "") { AppConfig = @"C:\Users\jason\Google Drive\IMDB\SQLiter\Dynamic_Coder\app.config"; }

            string AppConfig = ahk.FileDir(ProjectFile) + "\\app.config";

            DataTable SettingsTable = new DataTable();
            SettingsTable.Columns.Add("SettingName", typeof(string));
            SettingsTable.Columns.Add("serializeAs", typeof(string));
            SettingsTable.Columns.Add("Value", typeof(string));

            if (File.Exists(AppConfig))
            {
                AppConfig = ahk.FileRead(AppConfig);

                List<string> Lines = lst.Text_To_List(AppConfig);

                //<setting name="Load_Code_Bin" serializeAs="String">
                //  <value>True</value>
                //</setting>

                string SettingName = "";
                string serializeAs = "";
                string Value = "";

                int counter = 0;
                foreach (string line in Lines)
                {
                    if (line.Contains("<setting name="))
                    {

                        SettingName = ahk.StringSplit(line, "=", 1);
                        SettingName = ahk.StringReplace(SettingName, "serializeAs");
                        SettingName = ahk.StringReplace(SettingName, "\"");
                        SettingName = SettingName.Trim();

                        serializeAs = ahk.StringSplit(line, "=", 2);
                        serializeAs = ahk.StringReplace(serializeAs, ">");
                        serializeAs = ahk.StringReplace(serializeAs, "\"");
                        serializeAs = serializeAs.Trim();

                        //ahk.MsgBox("Setting: " + SettingName + Environment.NewLine + "serializeAs: " + serializeAs);
                    }

                    if (line.Contains("<value>"))
                    {
                        Value = ahk.StringReplace(line, "<value>");
                        Value = ahk.StringReplace(Value, "</value>");
                        Value = Value.Trim();
                        if (Value == "") { Value = " "; }
                        //ahk.MsgBox("Saved Value: " + Value);
                    }

                    if (SettingName != "" && Value != "")
                    {
                        counter++;
                        //ahk.MsgBox("Setting (" + counter.ToString() + ") : " + SettingName + Environment.NewLine + "serializeAs: " + serializeAs + Environment.NewLine + "Value: " + Value);

                        SettingsTable.Rows.Add(SettingName, serializeAs, Value);

                        SettingName = "";
                        serializeAs = "";
                        Value = "";
                    }

                }


                DV.DataSource = SettingsTable;
            }

            return SettingsTable;
        }

        /// <summary> example table - creating datatable to display in a grid</summary>
        static DataTable Example_Settings_Table()
        {
            // Here we create a DataTable with four columns.
            DataTable table = new DataTable();
            table.Columns.Add("Dosage", typeof(int));
            table.Columns.Add("Drug", typeof(string));
            table.Columns.Add("Date", typeof(DateTime));

            // Here we add five DataRows.
            table.Rows.Add(25, "Indocin", DateTime.Now);
            table.Rows.Add(50, "Enebrel", DateTime.Now);
            return table;
        }

        #endregion

        #region === Code Parse TreeView ===

        //// Parse code file - display Regions + Functions in TreeView
        //        string codeFile = @"C:\Users\Jason\Google Drive\IMDB\sharp_AHK\AutoHotkey.Interop\sharpAHK.cs";
        //        TreeNode regions = code.Region_Node_FromFile(codeFile);
        //        Left_Tree.Nodes.Add(regions);  // populate tree


        /// <summary> use this function to load project/dll function lists into treeview</summary>
        /// <param name="TreeView TV"> </param>
        /// <param name="All"> </param>
        public void CodeParse_TreeView(TreeView TV, bool All = false)
        {
            List<string> DLLCodeFileList = DLL_Code_FileList(All);

            //List<string> ProjectFileList = Project_Code_FileList(All);

            Setup_CodeParse_TV(TV);

            // ddl code files
            foreach (string item in DLLCodeFileList) { Load_CodeParseFile(TV, item); }

            // project code files
            //foreach (string item in ProjectFileList) { Load_CodeParseFile(TV, item); }
        }

        /// <summary> loading new file - add structure to treeview for nodes to attach during parse</summary>
        /// <param name="TreeView TV"> </param>
        /// <param name="Load_UsingNodes"> </param>
        /// <param name="AllProjects"> </param>
        public void Setup_CodeParse_TV(TreeView TV, bool Load_UsingNodes = false, bool AllProjects = false)
        {
            List<string> DLL_FileList = DLL_Code_FileList(AllProjects);
            List<string> ProjectFileList = Project_Code_FileList(AllProjects);

            // setup treeview structure

            //string Option = "DLL";
            //Option = "Projects";


            TreeNode rootNodeDLL = new TreeNode();
            TreeNode rootNodeTools = new TreeNode();


            foreach (string item in DLL_FileList)
            {
                rootNodeDLL.Text = "DLL Projects";

                TreeNode parent = new TreeNode();  // level 1
                TreeNode usingnode = new TreeNode();  // level 2
                TreeNode regionSnode = new TreeNode();  // level 2
                TreeNode functionSnode = new TreeNode();  // level 2

                FileInfo info2 = new FileInfo(item);
                string LoadFileName = info2.Name;


                parent.Text = LoadFileName;
                rootNodeDLL.Nodes.Add(parent);

                if (Load_UsingNodes)
                {
                    usingnode.Text = "Using";
                    parent.Nodes.Add(usingnode);
                }

                // regionSnode.Text = "Regions";
                // parent.Nodes.Add(regionSnode);

                functionSnode.Text = "Functions";
                parent.Nodes.Add(functionSnode);

            }


            foreach (string item in ProjectFileList)
            {
                rootNodeTools.Text = "Tool Projects";

                TreeNode parent = new TreeNode();  // level 1
                TreeNode usingnode = new TreeNode();  // level 2
                TreeNode regionSnode = new TreeNode();  // level 2
                TreeNode functionSnode = new TreeNode();  // level 2
                TreeNode controlSnode = new TreeNode();  // level 2

                FileInfo info2 = new FileInfo(item);
                string LoadFileName = info2.Name;


                parent.Text = LoadFileName;
                rootNodeTools.Nodes.Add(parent);

                if (Load_UsingNodes)
                {
                    usingnode.Text = "Using";
                    parent.Nodes.Add(usingnode);
                }

                //regionSnode.Text = "Regions";
                //parent.Nodes.Add(regionSnode);

                functionSnode.Text = "Functions";
                parent.Nodes.Add(functionSnode);

                //controlSnode.Text = "Controls";
                //parent.Nodes.Add(controlSnode);


            }

            TV.Nodes.Add(rootNodeDLL);
            TV.Nodes.Add(rootNodeTools);
            TV.Refresh();


        }

        // parse .cs file into elements (function list etc), display each cs file in new node on treeview 
        public void Load_CodeParseList(TreeView TV, List<string> CSFiles, bool Load_UsingNodes = false)
        {
            TV.Enabled = false;
            // setup nodes for each file in list before populating
            foreach (string LoadFile in CSFiles)
            {
                TreeNode rootNodeDLL = new TreeNode();

                rootNodeDLL.Text = ahk.FileName(LoadFile);
                rootNodeDLL.Tag = LoadFile;


                TreeNode parent = new TreeNode();  // level 1
                TreeNode usingnode = new TreeNode();  // level 2
                TreeNode regionSnode = new TreeNode();  // level 2
                TreeNode functionSnode = new TreeNode();  // level 2

                FileInfo info2 = new FileInfo(LoadFile);
                string LoadFileName = info2.Name;


                parent.Text = LoadFileName;
                rootNodeDLL.Nodes.Add(parent);

                if (Load_UsingNodes)
                {
                    usingnode.Text = "Using";
                    rootNodeDLL.Nodes.Add(usingnode);
                }

                // regionSnode.Text = "Regions";
                // parent.Nodes.Add(regionSnode);

                functionSnode.Text = "Functions";
                rootNodeDLL.Nodes.Add(functionSnode);
                TV.Nodes.Add(rootNodeDLL);

                Load_CodeParseFile(TV, LoadFile, Load_UsingNodes);
            }



            TV.Enabled = true;
            TV.Refresh();

        }


        /// <summary> load .cs file in treeview</summary>
        /// <param name="TreeView TV"> </param>
        /// <param name="LoadFilen"> </param>
        /// <param name="Load_UsingNodes"> </param>
        public string Load_CodeParseFile(TreeView TV, string LoadFilen = @"c:\users\jason\google drive\imdb\sqliter\ahk_dll\autohotkey.interop\ahk.cs", bool Load_UsingNodes = false)
        {
            FileInfo finfo = new FileInfo(LoadFilen);

            // set global variables 
            string LoadFile = LoadFilen;
            string LoadFileName = finfo.Name;


            // read loadfile text, display on gui
            string Code = ahk.FileRead(LoadFilen);


            if (Load_UsingNodes) { Populate_Using_Node(TV, LoadFilen, "Using", LoadFileName); }  // option to load Using Lines in Node View


            //List<string> functionslisted = Populate_Function_Dictionary(LoadFilen);   // populate dictionary with lists of functions and their file paths when loading each file
            Populate_FunctionList_Tv(TV, LoadFilen, "Functions", LoadFileName);

            //Populate_RegionList_Tv(TV, LoadFilen, "Regions", LoadFileName);

            Populate_Controls_Tv(TV, LoadFilen, "Controls", LoadFileName);


            TV.CollapseAll();

            return Code;
        }




        // todo: finish this

        /// <summary> parse .cs file, populate treeview control with "Regions" node with list of functions by Region underneath</summary>
        /// <param name="TreeView TV"> </param>
        /// <param name=" ParseFile"> </param>
        /// <param name="NodeName"> </param>
        /// <param name="ParentName"> </param>
        public void Populate_RegionList_Tv(TreeView TV, string ParseFile, string NodeName = "Regions", string ParentName = "")
        {
            string loadFileName = ahk.FileName(ParseFile);

            TreeNode parent = tv.ReturnNode(TV, loadFileName);  // return the parent node to add new control node under

            TreeNode regionsNode = Generate_RegionList_Tv(TV, ParseFile, "Regions", loadFileName);

            parent.Nodes.Add(regionsNode);  // add control list node to treeview

            return;

            string Code = ahk.FileRead(ParseFile);
            string FileTag = ParseFile;



            List<string> LineList = ahk.LineList(Code);

            string RegionCodeSection = "";
            bool RegionStart = false;
            bool RegionCapture = false;
            string ThisRegionName = "";
            int RegionCounter = 0;
            bool InFunction = false;
            string ThisFunctionName = "";
            int BracketCount = 0;
            bool BracketStart = false;

            foreach (string Line in LineList)
            {
                if (Line == "") { continue; }


                //### Check To See If We're Inside a function before attempting to grab the region start/end  #####

                bool FunctionCheck = IsFunction(Line);

                if (FunctionCheck)
                {
                    InFunction = true;
                    ThisFunctionName = FunctionName(Line);

                    //if (ThisFunctionName == "}") { InFunction = false; }  // fixes get-set lines
                }

                if (InFunction)
                {
                    if (Line.Contains("{"))
                    {
                        int bcount = Line.Count(f => f == '{');  // count number of brackets in line
                        BracketStart = true;
                        BracketCount += bcount;
                    }

                    if (BracketStart)
                    {
                        if (Line.Contains("}"))
                        {
                            int bcount = Line.Count(f => f == '}');  // count number of brackets in line
                            BracketCount -= bcount;
                        }

                        if (BracketCount == 0)
                        {
                            InFunction = false;
                            ThisFunctionName = "";
                            BracketStart = false;
                        }
                    }
                }


                if (IsCommentLine(Line)) { continue; }

                if (!InFunction) { RegionStart = IsRegion(Line); }

                if (!InFunction)
                {
                    if (RegionStart)  // beginning of region
                    {
                        if (RegionCounter == 0)
                        {
                            RegionCounter++;
                            RegionCapture = true;
                            ThisRegionName = RegionName(Line);
                        }

                    }

                    bool EndofRegion = false;
                    if (!InFunction) { EndofRegion = IsEndRegion(Line); }

                    if (IsEndRegion(Line))  // end of region
                    {
                        if (ThisRegionName == "") { continue; }

                        RegionCounter--;

                        if (RegionCounter == 0)
                        {
                            RegionCapture = false;

                            //ahk.MsgBox(RegionCodeSection); 

                            // search for the Regions Node under LoadFileName
                            TreeNode regionSnode = tv.Find_Node(TV, NodeName, ParentName);

                            if (regionSnode == null)
                            {
                                regionSnode = new TreeNode();
                                regionSnode.Text = "Regions";
                            }

                            bool NodeExists = tv.NodeExist(TV, ThisRegionName);

                            TV.Refresh();
                            if (!NodeExists)
                            {
                                TreeNode regionnode = new TreeNode();  // level 2
                                regionnode.Text = ThisRegionName;
                                regionnode.Tag = FileTag;
                                regionSnode.Nodes.Add(regionnode);
                            }


                            // loop through region section and display functions in region section

                            List<string> RegionLines = ahk.LineList(RegionCodeSection);
                            List<string> RegionFunctions = new List<string>();
                            foreach (string reg in RegionLines)
                            {
                                bool FunctionLine = IsFunction(reg);
                                if (FunctionLine)
                                {
                                    string Function = FunctionName(reg);
                                    string Params = FunctionParams(reg);
                                    string Comments = FunctionComments(reg);

                                    RegionFunctions.Add(Function);

                                    bool NodeFount = tv.NodeExist(TV, ThisRegionName);

                                    if (tv.NodeExist(TV, ThisRegionName))
                                    {
                                        TreeNode regionnode = tv.ReturnNode(TV, ThisRegionName);

                                        TreeNode region = new TreeNode();
                                        region.Text = Function;
                                        region.Tag = FileTag;
                                        regionnode.Nodes.Add(region);
                                    }



                                    //ahk.MsgBox(Function + Environment.NewLine + Params + Environment.NewLine + Comments);
                                }

                            }


                            //ahk.MsgBox("Region: " + ThisRegionName + " Has " + RegionFunctions.Count.ToString() + " Functions");

                            RegionCodeSection = "";
                            ThisRegionName = "";

                        }


                    }

                }

                if (RegionCapture)
                {
                    if (RegionCodeSection != "") { RegionCodeSection = RegionCodeSection + Environment.NewLine + Line; }
                    if (RegionCodeSection == "") { RegionCodeSection = Line; }
                }

            }


        }


        // !!! messed up sqlite values on function changes

        /// <summary> generates region nodes from .cs file, can attach to another parent node</summary>
        /// <param name="TreeView TV"> </param>
        /// <param name=" ParseFile"> </param>
        /// <param name="NodeName"> </param>
        /// <param name="ParentName"> </param>
        public TreeNode Generate_RegionList_Tv(TreeView TV, string ParseFile, string NodeName = "Regions", string ParentName = "")
        {
            string Code = ahk.FileRead(ParseFile);
            string FileTag = ParseFile;

            NodeInfo node = new NodeInfo();
            node.FilePath = ParseFile;

            Dictionary<string, TreeNode> NodeDict = new Dictionary<string, TreeNode>();

            bool WriteSQL = false;
            string SQLiteDb = @"C:\Users\jason\Google Drive\IMDB\SQLiter\Db\FunctionExport.sqlite";
            //if (WriteSQL)
            //{
            //    sqlite.Create_SettingsDb(SQLiteDb, "Function_Lib", true);
            //}


            List<string> LineList = ahk.LineList(Code);

            string RegionCodeSection = "";
            bool RegionStart = false;
            bool RegionCapture = false;
            string ThisRegionName = "";
            int RegionCounter = 0;
            bool InFunction = false;
            string ThisFunctionName = "";
            int BracketCount = 0;
            bool BracketStart = false;

            // search for the Regions Node under LoadFileName
            TreeNode regionSnode = tv.Find_Node(TV, NodeName, ParentName);

            // create new node if it doesn't already exist
            if (regionSnode == null)
            {
                regionSnode = new TreeNode();
                regionSnode.Text = NodeName;
            }


            NodeDict.Add(ParentName, regionSnode);


            foreach (string Line in LineList)
            {
                if (Line == "") { continue; }


                //### Check To See If We're Inside a function before attempting to grab the region start/end  #####

                bool FunctionCheck = IsFunction(Line);

                if (FunctionCheck)
                {
                    InFunction = true;
                    ThisFunctionName = FunctionName(Line);

                    //if (ThisFunctionName == "}") { InFunction = false; }  // fixes get-set lines
                }

                if (InFunction)
                {
                    if (Line.Contains("{"))
                    {
                        int bcount = Line.Count(f => f == '{');  // count number of brackets in line
                        BracketStart = true;
                        BracketCount += bcount;
                    }

                    if (BracketStart)
                    {
                        if (Line.Contains("}"))
                        {
                            int bcount = Line.Count(f => f == '}');  // count number of brackets in line
                            BracketCount -= bcount;
                        }

                        if (BracketCount == 0)
                        {
                            InFunction = false;
                            ThisFunctionName = "";
                            BracketStart = false;
                        }
                    }
                }


                if (IsCommentLine(Line)) { continue; }

                if (!InFunction) { RegionStart = IsRegion(Line); }

                if (!InFunction)
                {
                    if (RegionStart)  // beginning of region
                    {
                        if (RegionCounter == 0)
                        {
                            RegionCounter++;
                            RegionCapture = true;
                            ThisRegionName = RegionName(Line);
                        }

                    }

                    bool EndofRegion = false;
                    if (!InFunction) { EndofRegion = IsEndRegion(Line); }

                    if (IsEndRegion(Line))  // end of region
                    {
                        if (ThisRegionName == "") { continue; }

                        RegionCounter--;

                        if (RegionCounter == 0)
                        {
                            RegionCapture = false;

                            //ahk.MsgBox(RegionCodeSection); 

                            if (!NodeDict.ContainsKey(ThisRegionName))
                            {
                                TreeNode regionnode = new TreeNode();  // level 2
                                regionnode.Text = ThisRegionName;
                                regionnode.Tag = FileTag;
                                regionSnode.Nodes.Add(regionnode);

                                NodeDict.Add(ThisRegionName, regionnode);

                                if (WriteSQL) { sqlite.Setting_Save("Function_Lib", NodeName, ThisRegionName, SQLiteDb); }
                            }


                            //bool NodeExists = tv.NodeExist(TV, ThisRegionName);

                            ////TV.Refresh();
                            //if (!NodeExists)
                            //{

                            //}


                            // loop through region section and display functions in region section

                            List<string> RegionLines = ahk.LineList(RegionCodeSection);
                            List<string> RegionFunctions = new List<string>();
                            foreach (string reg in RegionLines)
                            {
                                bool FunctionLine = IsFunction(reg);
                                if (FunctionLine)
                                {
                                    string Function = FunctionName(reg);
                                    string Params = FunctionParams(reg);
                                    string Comments = FunctionComments(reg);

                                    RegionFunctions.Add(Function);


                                    bool NodeFount = tv.NodeExist(TV, ThisRegionName);

                                    if (NodeDict.ContainsKey(ThisRegionName))
                                    {
                                        //tv.ReturnNode(TV, ThisRegionName);

                                        TreeNode regionnode = null;
                                        TreeNode regionval = null;

                                        if (NodeDict.TryGetValue(ThisRegionName, out regionval)) // Returns false.
                                        {
                                            regionnode = regionval;
                                        }

                                        TreeNode region = new TreeNode();
                                        region.Text = Function;
                                        region.Tag = FileTag;
                                        regionnode.Nodes.Add(region);

                                        if (!NodeDict.ContainsKey(Function)) { NodeDict.Add(Function, region); }

                                        if (WriteSQL) { sqlite.Setting_Save(SQLiteDb, "Function_Lib", NodeName, ThisRegionName); }

                                    }


                                    //ahk.MsgBox(Function + Environment.NewLine + Params + Environment.NewLine + Comments);
                                }

                            }


                            //ahk.MsgBox("Region: " + ThisRegionName + " Has " + RegionFunctions.Count.ToString() + " Functions");

                            RegionCodeSection = "";
                            ThisRegionName = "";

                        }


                    }

                }

                if (RegionCapture)
                {
                    if (RegionCodeSection != "") { RegionCodeSection = RegionCodeSection + Environment.NewLine + Line; }
                    if (RegionCodeSection == "") { RegionCodeSection = Line; }
                }

            }


            //foreach (var item in NodeDict)
            //{
            //    ahk.MsgBox(item.Key);
            //}

            TV.Nodes.Add(regionSnode); // populate treeview

            return regionSnode;

        }

        /// <summary> generates region nodes from .cs file, can attach to another parent node</summary>
        /// <param name="TreeView TV"> </param>
        /// <param name=" ParseFile"> </param>
        /// <param name="ParentName"> </param>
        public void Display_Regions_Only_TreeNodes(TreeView TV, string ParseFile, string ParentName = "Regions")
        {
            List<string> RegionList = RegionList_FromFile(ParseFile);


            if (ParentName != "")
            {
                TreeNode regions = new TreeNode();  // level 2
                regions.Text = ParentName;

                foreach (string region in RegionList)
                {
                    TreeNode regionnode = new TreeNode();  // level 2
                    regionnode.Text = region;
                    regionnode.Tag = ParseFile;

                    regions.Nodes.Add(regionnode);
                }

                TV.Nodes.Add(regions);
            }

            if (ParentName == "")
            {
                foreach (string region in RegionList)
                {
                    TreeNode regionnode = new TreeNode();  // level 2
                    regionnode.Text = region;
                    regionnode.Tag = ParseFile;
                    TV.Nodes.Add(regionnode);
                }

            }


        }

        /// <summary> populate treeview with "Functions" node and list of functions underneath. can attach to another parent node name</summary>
        /// <param name="TreeView TV"> </param>
        /// <param name=" ProjectFile"> </param>
        /// <param name="NodeName"> </param>
        /// <param name="ParentName"> </param>
        public void Populate_FunctionList_Tv(TreeView TV, string ProjectFile, string NodeName = "Functions", string ParentName = "")
        {
            string Code = ahk.FileRead(ProjectFile);
            List<string> functionList = FunctionList_FromCode(Code);

            functionList.Sort();  // show function list sorted alpha

            // search for the Functions Node under LoadFileName
            TreeNode functionSnode = tv.Find_Node(TV, NodeName, ParentName);

            foreach (string item in functionList)
            {
                TreeNode entry = new TreeNode();  // level 3
                entry.Text = item;
                entry.Tag = ProjectFile;
                functionSnode.Nodes.Add(entry);
            }

        }

        /// <summary> returns a treenode with list of functions from the .cs parse file</summary>
        /// <param name="TreeView TV"> </param>
        /// <param name=" ParseFile"> </param>
        /// <param name="NodeName"> </param>
        public TreeNode Return_FunctionList_Node(TreeView TV, string ParseFile, string NodeName = "Functions")
        {
            string Code = ahk.FileRead(ParseFile);
            List<string> functionList = FunctionList_FromCode(Code);

            functionList.Sort();  // show function list sorted alpha

            TreeNode functionSnode = new TreeNode();
            functionSnode.Text = NodeName;
            functionSnode.Tag = ParseFile;

            foreach (string item in functionList)
            {
                TreeNode entry = new TreeNode();  // level 3
                entry.Text = item;
                entry.Tag = ParseFile;
                functionSnode.Nodes.Add(entry);
            }

            return functionSnode;
        }



        /// <summary> populate treeview with controls from .cs project</summary>
        /// <param name="TreeView TV"> </param>
        /// <param name=" ParseFile"> </param>
        /// <param name="NodeName"> </param>
        /// <param name="ParentName"> </param>
        public void Populate_Controls_Tv(TreeView TV, string ParseFile, string NodeName = "Controls", string ParentName = "")
        {
            string designerFile = DesignerPath(ParseFile);  // take project .cs path and add .designer.cs 

            if (ahk.IfExist(designerFile))
            {
                string loadFileName = "";

                if (ParentName != "") { loadFileName = ParentName; }
                if (ParentName == "") { loadFileName = ahk.FileName(ParseFile); } // name of the .cs file loading (name of parent node)

                TreeNode parent = tv.ReturnNode(TV, loadFileName);  // return the parent node to add new control node under

                TreeNode controlsNode = DesignerFile_TVNode(designerFile, NodeName);

                parent.Nodes.Add(controlsNode);  // add control list node to treeview
            }
        }

        /// <summary> adds "using" node to treeview from parsed .cs file</summary>
        /// <param name="TreeView TV"> </param>
        /// <param name=" ParseFile"> </param>
        /// <param name="NodeName"> </param>
        /// <param name="ParentName"> </param>
        public void Populate_Using_Node(TreeView TV, string ParseFile, string NodeName = "Using", string ParentName = "")
        {
            string Code = ahk.FileRead(ParseFile);
            List<string> LineList = ahk.LineList(Code);


            // search for the Using Node under LoadFileName
            TreeNode usingnode = tv.Find_Node(TV, NodeName, ParentName);

            if (usingnode == null)
            {
                TreeNode parent = tv.ReturnNode(TV, ParentName);   // create using node if it doesn't exist yet

                usingnode = new TreeNode();  // level 2
                usingnode.Text = NodeName;
                parent.Nodes.Add(usingnode);
            }


            // Using Node
            foreach (string Line in LineList)
            {
                if (Line == "") { continue; }


                bool iUsingLine = IsUsing(Line);
                if (iUsingLine)
                {
                    string UsingNamed = UsingName(Line);

                    TreeNode entry = new TreeNode();  // level 3
                    entry.Text = UsingNamed;
                    entry.Tag = ParseFile;
                    usingnode.Nodes.Add(entry);
                }
            }

        }

        public struct NodeInfo  // used to return window position
        {
            public TreeView TV { get; set; }
            public string FilePath { get; set; }
            public string FileName { get; set; }
            public string NodeName { get; set; }
            public TreeNode parentNode { get; set; }
            public TreeNode thisNode { get; set; }

            //Dictionary<string, TreeNode> NodeDictEntry { get; set; }
        }


        /// <summary> parse project Designer.cs file - display controls in TreeView</summary>
        /// <param name="TreeView TV"> </param>
        /// <param name=" DesignerFilePath"> </param>
        /// <param name="NodeName"> </param>
        /// <param name="ClearTV"> </param>
        public void DesignerFile_TV(TreeView TV, string DesignerFilePath, string NodeName = "Controls", bool ClearTV = false)
        {
            if (ClearTV) { TV.Nodes.Clear(); }

            TreeNode Controls = DesignerFile_TVNode(DesignerFilePath, NodeName);

            TV.Nodes.Add(Controls); // populate treeview

        }

        /// <summary> parse project Designer.cs file - returns TreeNode populated with control names</summary>
        /// <param name="DesignerFilePath"> </param>
        /// <param name="NodeName"> </param>
        public TreeNode DesignerFile_TVNode(string DesignerFilePath, string NodeName = "Controls")
        {
            if (!File.Exists(DesignerFilePath)) { return null; }

            bool ParentBuilt = false;

            TreeNode Controls = new TreeNode();


            if (NodeName.ToUpper() == "DATAGRIDVIEW")
            {
                Controls.Text = NodeName.ToUpper();
                Controls.Tag = DesignerFilePath;

                List<string> Lines = lst.TextFile_To_List(DesignerFilePath);

                foreach (string Line in Lines)
                {
                    string FirstWord = ahk.FirstWord(Line);

                    if (FirstWord == "private")
                    {
                        if (Line.Contains("=")) { continue; }
                        if (Line.Contains("(")) { continue; }

                        bool Added = false;

                        string controlName = ahk.WordNum(Line, 3);  //third word in line is the name of the control
                        controlName = ahk.StringReplace(controlName, ";");
                        controlName = controlName.Trim();

                        string controlType = ahk.WordNum(Line, 2);


                        if (controlType == "System.Windows.Forms.DataGridView") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; Controls.Nodes.Add(newnode); }
                    }
                }

                return Controls;
            }
            if (NodeName.ToUpper() == "TREEVIEW")
            {
                Controls.Text = NodeName.ToUpper();
                Controls.Tag = DesignerFilePath;

                List<string> Lines = lst.TextFile_To_List(DesignerFilePath);

                foreach (string Line in Lines)
                {
                    string FirstWord = ahk.FirstWord(Line);

                    if (FirstWord == "private")
                    {
                        if (Line.Contains("=")) { continue; }
                        if (Line.Contains("(")) { continue; }

                        bool Added = false;

                        string controlName = ahk.WordNum(Line, 3);  //third word in line is the name of the control
                        controlName = ahk.StringReplace(controlName, ";");
                        controlName = controlName.Trim();

                        string controlType = ahk.WordNum(Line, 2);


                        if (controlType == "System.Windows.Forms.TreeView") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; Controls.Nodes.Add(newnode); }
                    }
                }

                return Controls;
            }
            if (NodeName.ToUpper() == "SCINTILLA")
            {
                Controls.Text = NodeName.ToUpper();
                Controls.Tag = DesignerFilePath;

                List<string> Lines = lst.TextFile_To_List(DesignerFilePath);

                foreach (string Line in Lines)
                {
                    string FirstWord = ahk.FirstWord(Line);

                    if (FirstWord == "private")
                    {
                        if (Line.Contains("=")) { continue; }
                        if (Line.Contains("(")) { continue; }

                        bool Added = false;

                        string controlName = ahk.WordNum(Line, 3);  //third word in line is the name of the control
                        controlName = ahk.StringReplace(controlName, ";");
                        controlName = controlName.Trim();

                        string controlType = ahk.WordNum(Line, 2);


                        if (controlType == "ScintillaNET.Scintilla") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; Controls.Nodes.Add(newnode); }
                    }
                }

                return Controls;
            }
            if (NodeName.ToUpper() == "LISTBOX")
            {
                Controls.Text = NodeName.ToUpper();
                Controls.Tag = DesignerFilePath;

                List<string> Lines = lst.TextFile_To_List(DesignerFilePath);

                foreach (string Line in Lines)
                {
                    string FirstWord = ahk.FirstWord(Line);

                    if (FirstWord == "private")
                    {
                        if (Line.Contains("=")) { continue; }
                        if (Line.Contains("(")) { continue; }

                        bool Added = false;

                        string controlName = ahk.WordNum(Line, 3);  //third word in line is the name of the control
                        controlName = ahk.StringReplace(controlName, ";");
                        controlName = controlName.Trim();

                        string controlType = ahk.WordNum(Line, 2);


                        if (controlType == "System.Windows.Forms.ListBox") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; Controls.Nodes.Add(newnode); }
                    }
                }

                return Controls;
            }
            if (NodeName.ToUpper() == "COMBOBOX")
            {
                Controls.Text = NodeName.ToUpper();
                Controls.Tag = DesignerFilePath;

                List<string> Lines = lst.TextFile_To_List(DesignerFilePath);

                foreach (string Line in Lines)
                {
                    string FirstWord = ahk.FirstWord(Line);

                    if (FirstWord == "private")
                    {
                        if (Line.Contains("=")) { continue; }
                        if (Line.Contains("(")) { continue; }

                        bool Added = false;

                        string controlName = ahk.WordNum(Line, 3);  //third word in line is the name of the control
                        controlName = ahk.StringReplace(controlName, ";");
                        controlName = controlName.Trim();

                        string controlType = ahk.WordNum(Line, 2);


                        if (controlType == "System.Windows.Forms.ComboBox") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; Controls.Nodes.Add(newnode); }
                    }
                }

                return Controls;
            }



            // Add ALL Controls To Single Node

            if (!ParentBuilt)
            {
                //setup treeview
                //TreeNode Controls = new TreeNode();
                Controls.Text = NodeName;

                TreeNode BindingNavigatorNode = new TreeNode();
                BindingNavigatorNode.Text = "BindingNavigators";
                Controls.Nodes.Add(BindingNavigatorNode);

                TreeNode ButtonNode = new TreeNode();
                ButtonNode.Text = "Buttons";
                Controls.Nodes.Add(ButtonNode);

                TreeNode CheckBoxNode = new TreeNode();
                CheckBoxNode.Text = "CheckBoxes";
                Controls.Nodes.Add(CheckBoxNode);

                TreeNode CheckedListBoxNode = new TreeNode();
                CheckedListBoxNode.Text = "CheckedListBoxes";
                Controls.Nodes.Add(CheckedListBoxNode);

                TreeNode ColorDialogNode = new TreeNode();
                ColorDialogNode.Text = "ColorDialog";
                Controls.Nodes.Add(ColorDialogNode);

                TreeNode ComboBoxNode = new TreeNode();
                ComboBoxNode.Text = "ComboBoxs";
                Controls.Nodes.Add(ComboBoxNode);

                TreeNode ContextMenuStripNode = new TreeNode();
                ContextMenuStripNode.Text = "ContextMenuStrips";
                Controls.Nodes.Add(ContextMenuStripNode);

                TreeNode DataGridViewNode = new TreeNode();
                DataGridViewNode.Text = "DataGridViews";
                Controls.Nodes.Add(DataGridViewNode);

                TreeNode DateTimePickerNode = new TreeNode();
                DateTimePickerNode.Text = "DateTimePicker";
                Controls.Nodes.Add(DateTimePickerNode);

                TreeNode FileSystemWatcherNode = new TreeNode();
                FileSystemWatcherNode.Text = "FileSystemWatchers";
                Controls.Nodes.Add(FileSystemWatcherNode);

                TreeNode FontDialogNode = new TreeNode();
                FontDialogNode.Text = "FontDialog";
                Controls.Nodes.Add(FontDialogNode);

                TreeNode LabelNode = new TreeNode();
                LabelNode.Text = "Labels";
                Controls.Nodes.Add(LabelNode);

                TreeNode ListBoxNode = new TreeNode();
                ListBoxNode.Text = "ListBoxes";
                Controls.Nodes.Add(ListBoxNode);

                TreeNode ListViewNode = new TreeNode();
                ListViewNode.Text = "ListView";
                Controls.Nodes.Add(ListViewNode);

                TreeNode MenuStripNode = new TreeNode();
                MenuStripNode.Text = "MenuStrips";
                Controls.Nodes.Add(MenuStripNode);

                TreeNode MonthCalendarNode = new TreeNode();
                MonthCalendarNode.Text = "MonthCalendar";
                Controls.Nodes.Add(MonthCalendarNode);

                TreeNode NotifyIconNode = new TreeNode();
                NotifyIconNode.Text = "NotifyIcon";
                Controls.Nodes.Add(NotifyIconNode);

                TreeNode OpenFileDialogNode = new TreeNode();
                OpenFileDialogNode.Text = "OpenFileDialogs";
                Controls.Nodes.Add(OpenFileDialogNode);

                TreeNode PanelNode = new TreeNode();
                PanelNode.Text = "Panels";
                Controls.Nodes.Add(PanelNode);

                TreeNode PictureBoxNode = new TreeNode();
                PictureBoxNode.Text = "PictureBoxes";
                Controls.Nodes.Add(PictureBoxNode);

                TreeNode ProgressBarNode = new TreeNode();
                ProgressBarNode.Text = "ProgressBars";
                Controls.Nodes.Add(ProgressBarNode);

                TreeNode PropertyGridNode = new TreeNode();
                PropertyGridNode.Text = "PropertyGrid";
                Controls.Nodes.Add(PropertyGridNode);

                TreeNode RadioButtonNode = new TreeNode();
                RadioButtonNode.Text = "RadioButton";
                Controls.Nodes.Add(RadioButtonNode);

                TreeNode RichTextBoxNode = new TreeNode();
                RichTextBoxNode.Text = "RichTextBoxs";
                Controls.Nodes.Add(RichTextBoxNode);

                TreeNode SaveFileDialogNode = new TreeNode();
                SaveFileDialogNode.Text = "SaveFileDialogs";
                Controls.Nodes.Add(SaveFileDialogNode);

                TreeNode ScintillaNode = new TreeNode();
                ScintillaNode.Text = "Scintillas";
                Controls.Nodes.Add(ScintillaNode);

                TreeNode SplitterNode = new TreeNode();
                SplitterNode.Text = "Splitters";
                Controls.Nodes.Add(SplitterNode);

                TreeNode TabControlNode = new TreeNode();
                TabControlNode.Text = "TabControls";
                Controls.Nodes.Add(TabControlNode);

                TreeNode TabPageNode = new TreeNode();
                TabPageNode.Text = "TabPages";
                Controls.Nodes.Add(TabPageNode);

                TreeNode TableLayoutPanelNode = new TreeNode();
                TableLayoutPanelNode.Text = "TableLayoutPanels";
                Controls.Nodes.Add(TableLayoutPanelNode);

                TreeNode TextBoxNode = new TreeNode();
                TextBoxNode.Text = "TextBoxes";
                Controls.Nodes.Add(TextBoxNode);

                TreeNode ToolStripButtonNode = new TreeNode();
                ToolStripButtonNode.Text = "ToolStripButtons";
                Controls.Nodes.Add(ToolStripButtonNode);

                TreeNode ToolStripComboBoxNode = new TreeNode();
                ToolStripComboBoxNode.Text = "ToolStripComboBoxs";
                Controls.Nodes.Add(ToolStripComboBoxNode);

                TreeNode ToolStripLabelNode = new TreeNode();
                ToolStripLabelNode.Text = "ToolStripLabels";
                Controls.Nodes.Add(ToolStripLabelNode);

                TreeNode ToolStripMenuItemNode = new TreeNode();
                ToolStripMenuItemNode.Text = "ToolStripMenuItems";
                Controls.Nodes.Add(ToolStripMenuItemNode);

                TreeNode ToolStripTextBoxNode = new TreeNode();
                ToolStripTextBoxNode.Text = "ToolStripTextBoxs";
                Controls.Nodes.Add(ToolStripTextBoxNode);

                TreeNode ToolStripSeparatorNode = new TreeNode();
                ToolStripSeparatorNode.Text = "ToolStripSeparators";
                Controls.Nodes.Add(ToolStripSeparatorNode);

                TreeNode TreeViewNode = new TreeNode();
                TreeViewNode.Text = "TreeViews";
                Controls.Nodes.Add(TreeViewNode);

                TreeNode TreeViewFastNode = new TreeNode();
                TreeViewFastNode.Text = "TreeViewFast";
                Controls.Nodes.Add(TreeViewFastNode);

                TreeNode WebBrowserNode = new TreeNode();
                WebBrowserNode.Text = "WebBrowsers";
                Controls.Nodes.Add(WebBrowserNode);


                List<string> Lines = lst.TextFile_To_List(DesignerFilePath);

                List<string> controls = new List<string>();

                foreach (string Line in Lines)
                {
                    string FirstWord = ahk.FirstWord(Line);

                    if (FirstWord == "private")
                    {
                        if (Line.Contains("=")) { continue; }
                        if (Line.Contains("(")) { continue; }

                        bool Added = false;

                        string controlName = ahk.WordNum(Line, 3);  //third word in line is the name of the control
                        controlName = ahk.StringReplace(controlName, ";");
                        controlName = controlName.Trim();

                        string controlType = ahk.WordNum(Line, 2);


                        if (controlType == "System.Windows.Forms.DataGridView") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; DataGridViewNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.Button") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ButtonNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.TreeView") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; TreeViewNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.TextBox") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; TextBoxNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.TabControl") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; TabControlNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.TabPage") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; TabPageNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.ListBox") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ListBoxNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.MenuStrip") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; MenuStripNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.ToolStripMenuItem") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ToolStripMenuItemNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.TableLayoutPanel") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; TableLayoutPanelNode.Nodes.Add(newnode); }
                        if (controlType == "TreeViewFast.Controls.TreeViewFast") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; TreeViewFastNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.CheckBox") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; CheckBoxNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.Label") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; LabelNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.PictureBox") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; PictureBoxNode.Nodes.Add(newnode); }
                        if (controlType == "ScintillaNET.Scintilla") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ScintillaNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.Splitter") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; SplitterNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.Panel") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; PanelNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.ToolStripSeparator") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ToolStripSeparatorNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.SaveFileDialog") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; SaveFileDialogNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.BindingNavigator") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; BindingNavigatorNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.OpenFileDialog") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; OpenFileDialogNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.WebBrowser") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; WebBrowserNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.ToolStripComboBox") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ToolStripComboBoxNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.ToolStripButton") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ToolStripButtonNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.ToolStripLabel") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ToolStripLabelNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.ToolStripTextBox") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ToolStripTextBoxNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.RichTextBox") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; RichTextBoxNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.ComboBox") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ComboBoxNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.ContextMenuStrip") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ContextMenuStripNode.Nodes.Add(newnode); }
                        if (controlType == "System.IO.FileSystemWatcher") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; FileSystemWatcherNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.CheckedListBox") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; CheckedListBoxNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.DateTimePicker") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; DateTimePickerNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.MonthCalendar") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; MonthCalendarNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.FontDialog") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; FontDialogNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.PropertyGrid") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; PropertyGridNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.ListView") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ListViewNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.ColorDialog") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ColorDialogNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.NotifyIcon") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; NotifyIconNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.RadioButton") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; RadioButtonNode.Nodes.Add(newnode); }
                        if (controlType == "System.Windows.Forms.ProgressBar") { Added = true; TreeNode newnode = new TreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ProgressBarNode.Nodes.Add(newnode); }


                        if (!Added) { ahk.MsgBox("Didn't Add: " + controlType); }


                        //controls.Add(controlType);
                        //controls.Add(controlName);
                        //controls.Add(Line);
                    }
                }


            }


            //listBox1.DataSource = controls;
            //tv.Nodes.Add(Controls); // populate treeview

            return Controls;
        }


        /// <summary> creates a parent node in treeview to populate with project sections</summary>
        /// <param name="TreeView TV"> </param>
        /// <param name="ParentText"> </param>
        public void Populate_Parent_Project_Tree(TreeView TV, List<string> CSFileList, string ParentText = "-=Projects=-")
        {
            //=== Populate TreeView Structure ===

            TreeNode parent = new TreeNode();  // level 1
            parent.Text = ParentText;

            //List<string> ProjectList = Project_Code_FileList(true);

            foreach (string project in CSFileList)
            {
                string FileName = ahk.FileName(project);

                string ProjectName = ahk.StringReplace(FileName, ".cs");
                ProjectName = "[ " + ProjectName + " ]";
                TreeNode section = new TreeNode();  // level 2
                section.Text = ProjectName;
                section.Tag = project;
                parent.Nodes.Add(section);

                //TreeNode controls = new TreeNode();  // file folder
                //controls.Text = "Controls";
                //controls.Tag = project;
                //section.Nodes.Add(controls);

                TreeNode files = new TreeNode();  // file folder
                files.Text = "Files";
                files.Tag = project;
                section.Nodes.Add(files);

                TreeNode folders = new TreeNode();  // file folder
                folders.Text = "Folders";
                folders.Tag = project;
                section.Nodes.Add(folders);

                TreeNode functions = new TreeNode();  // file folder
                functions.Text = "Functions";
                functions.Tag = project;
                section.Nodes.Add(functions);


                //==== Files ====

                TreeNode file1 = new TreeNode();  // project cs file
                file1.Text = FileName;
                file1.Tag = project;
                files.Nodes.Add(file1);

                TreeNode file2 = new TreeNode();  // designer file
                file2.Text = ahk.FileName(DesignerPath(project));
                file2.Tag = DesignerPath(project);
                files.Nodes.Add(file2);

                TreeNode file3 = new TreeNode();  // app.config
                string appconfig = ahk.FileDir(project) + "\\app.config";
                file3.Text = ahk.FileName(appconfig);
                file3.Tag = appconfig;
                files.Nodes.Add(file3);

                TreeNode file4 = new TreeNode();  // C:\Users\jason\Google Drive\IMDB\SQLiter\Dynamic_Coder\Properties\Settings.Designer.cs
                string ssettingsd = ahk.FileDir(project) + "\\Properties\\Settings.Designer.cs";
                file4.Text = ahk.FileName(ssettingsd);
                file4.Tag = ssettingsd;
                files.Nodes.Add(file4);

                TreeNode file7 = new TreeNode();  // C:\Users\jason\Google Drive\IMDB\SQLiter\Dynamic_Coder\Properties\Settings.Designer.cs
                string ssettin = ahk.FileDir(project) + "\\Properties\\Settings.settings";
                file7.Text = ahk.FileName(ssettin);
                file7.Tag = ssettin;
                files.Nodes.Add(file7);

                TreeNode file8 = new TreeNode();  // C:\Users\jason\Google Drive\IMDB\SQLiter\Dynamic_Coder\Properties\Settings.Designer.cs
                string ssett = ahk.FileDir(project) + "\\Properties\\Resources.Designer.cs";
                file8.Text = ahk.FileName(ssett);
                file8.Tag = ssett;
                files.Nodes.Add(file8);



                TreeNode file5 = new TreeNode();   //C:\Users\jason\Google Drive\IMDB\SQLiter\Dynamic_Coder\Dynamic_Coder.csproj
                string csproj = ahk.FileDir(project) + "\\" + ahk.FileNameNoExt(project) + ".csproj";
                file5.Text = ahk.FileName(csproj);
                file5.Tag = csproj;
                files.Nodes.Add(file5);

                TreeNode file6 = new TreeNode();    //C:\Users\jason\Google Drive\IMDB\SQLiter\Dynamic_Coder\Properties\AssemblyInfo.cs
                string assembly = ahk.FileDir(project) + "\\Properties\\AssemblyInfo.cs";
                file6.Text = ahk.FileName(assembly);
                file6.Tag = assembly;
                files.Nodes.Add(file6);


                // === folders ===

                TreeNode folder1 = new TreeNode();    //C:\Users\jason\Google Drive\IMDB\SQLiter\Dynamic_Coder\Properties\AssemblyInfo.cs
                folder1.Text = ahk.DirName(project);
                folder1.Tag = ahk.FileDir(project);
                folders.Nodes.Add(folder1);

                TreeNode folder2 = new TreeNode();    //C:\Users\jason\Google Drive\IMDB\SQLiter\Dynamic_Coder\Properties\AssemblyInfo.cs
                folder2.Text = "Debug";
                folder2.Tag = ahk.FileDir(project) + "\\bin\\Debug";
                folders.Nodes.Add(folder2);

                TreeNode folder3 = new TreeNode();    //C:\Users\jason\Google Drive\IMDB\SQLiter\Dynamic_Coder\Properties\AssemblyInfo.cs
                folder3.Text = "Release";
                folder3.Tag = ahk.FileDir(project) + "\\bin\\Release";
                folders.Nodes.Add(folder3);

            }


            //TreeNode parent = Build_Parent_Project_Node(TV, "", true, ParentText); 
            TV.Nodes.Add(parent);  // populate tree
        }

        /// <summary> returns a parent node for projects - populates from list of project files inside function</summary>
        /// <param name="TreeView TV"> </param>
        /// <param name=" ProjectFile"> </param>
        /// <param name="ParentText"> </param>
        public TreeNode Build_Parent_Project_Node(TreeView TV, string ProjectFile, string ParentText = "-=Projects=-")
        {
            //=== Populate TreeView Structure ===

            TreeNode parent = new TreeNode();  // level 1
            parent.Text = ParentText;


            List<string> Project_File_List = new List<string>(); // list of files linked to main project file
            List<string> Project_Folder_List = new List<string>(); // list of files linked to main project file
            List<string> ProjectList = new List<string>();  // list of projects (or single project) to add to import to treeview


            ProjectList.Add(ProjectFile);

            Project_File_List.Add(ProjectFile);  // project file
            Project_File_List.Add(ahk.FileName(DesignerPath(ProjectFile)));  // designer file

            if (File.Exists(ahk.FileDir(ProjectFile) + "\\app.config")) { ProjectList.Add(ahk.FileDir(ProjectFile) + "\\app.config"); }
            if (File.Exists(ahk.FileDir(ProjectFile) + "\\Properties\\Settings.Designer.cs")) { ProjectList.Add(ahk.FileDir(ProjectFile) + "\\Properties\\Settings.Designer.cs"); }
            if (File.Exists(ahk.FileDir(ProjectFile) + "\\Properties\\Settings.settings")) { ProjectList.Add(ahk.FileDir(ProjectFile) + "\\Properties\\Settings.settings"); }

            Project_File_List.Add(ahk.FileDir(ProjectFile) + "\\app.config");  // app.config
            Project_File_List.Add(ahk.FileDir(ProjectFile) + "\\Properties\\Settings.Designer.cs"); // Properties\Settings.Designer.cs
            Project_File_List.Add(ahk.FileDir(ProjectFile) + "\\Properties\\Settings.settings");  //\Properties\Settings.settings
            Project_File_List.Add(ahk.FileDir(ProjectFile) + "\\Properties\\Resources.Designer.cs");  // \Properties\Settings.Designer.cs
            Project_File_List.Add(ahk.FileDir(ProjectFile) + "\\" + ahk.FileNameNoExt(ProjectFile) + ".csproj"); // \Dynamic_Coder.csproj
            Project_File_List.Add(ahk.FileDir(ProjectFile) + "\\Properties\\AssemblyInfo.cs"); // \Properties\AssemblyInfo.cs

            //== folders 
            Project_Folder_List.Add(ahk.DirName(ProjectFile)); // project dir
            Project_Folder_List.Add(ahk.FileDir(ProjectFile) + "\\bin\\Debug"); // debug dir
            Project_Folder_List.Add(ahk.FileDir(ProjectFile) + "\\bin\\Release"); // release dir


            foreach (string project in ProjectList)
            {
                string FileName = ahk.FileName(project);

                string ProjectName = ahk.StringReplace(FileName, ".cs");
                ProjectName = "[ " + ProjectName + " ]";

                parent.Text = ProjectName;
                parent.Tag = project;

                //TreeNode controls = new TreeNode();  // file folder
                //controls.Text = "Controls";
                //controls.Tag = project;
                //section.Nodes.Add(controls);

                TreeNode files = new TreeNode();  // file folder
                files.Text = "Files";
                files.Tag = project;
                parent.Nodes.Add(files);

                TreeNode folders = new TreeNode();  // file folder
                folders.Text = "Folders";
                folders.Tag = project;
                parent.Nodes.Add(folders);

                //TreeNode functions = new TreeNode();  // file folder
                //functions.Text = "Functions";
                //functions.Tag = project;
                //section.Nodes.Add(functions);


                // parse file list to create nodes for each file
                foreach (string filepath in Project_File_List)
                {
                    TreeNode file1 = new TreeNode();
                    //file1.Text = ahk.FileName(filepath);
                    file1.Text = project;
                    file1.Tag = project;
                    files.Nodes.Add(file1);
                }


                // parse folder list to create nodes for each file
                foreach (string folderpath in Project_Folder_List)
                {
                    TreeNode folder1 = new TreeNode();
                    folder1.Text = ahk.DirName(folderpath);
                    folder1.Tag = project;
                    folders.Nodes.Add(folder1);
                }


            }


            //TV.Nodes.Add(parent);  // populate tree

            return parent;
        }


        /// <summary> populates a treeview with list of controls</summary>
        /// <param name="TreeView TV"> </param>
        /// <param name=" NodeName"> </param>
        /// <param name=" ParentName"> </param>
        /// <param name=" ProjectFile"> </param>
        public void Populate_Project_ControlsTree(TreeView TV, string NodeName, string ParentName, string ProjectFile)
        {
            string designerPath = DesignerPath(ProjectFile);

            //TreeNode controlParent = tv.Find_Node(TV, NodeName, ParentName);

            TreeNode parent = tv.ReturnNode(TV, ParentName);

            TreeNode controlsNode = DesignerFile_TVNode(designerPath, NodeName);

            if (controlsNode != null)
            {
                parent.Nodes.Add(controlsNode);  // add control list node to treeview
            }

        }

        /// <summary> menu action to load dll code + project functions/folders into project</summary>
        /// <param name="TreeView TV"> </param>
        public void Populate_MyCode_TreeView(TreeView TV, List<string> CSFiles)
        {
            Populate_Parent_Project_Tree(TV, CSFiles);

            //List<string> projects = Project_Code_FileList(true);
            foreach (string project in CSFiles)
            {
                Load_ProjectFile_TV(TV, project, false, ahk.FileName(project));

                // v2 - works as well
                /*
                                string Parent = "[ " + ahk.FileNameNoExt(project) + " ]";
                                Populate_Project_ControlsTree(TV, "Controls", Parent, project);
                                Populate_FunctionList_Tv(TV, project, "Functions", Parent);
                 */
            }
        }

        /// <summary>Load project file, parses functions / control references to a single node</summary>
        /// <param name="TV">TreeView to Populate </param>
        /// <param name="ProjectFile">Path to CS File to Parse</param>
        /// <param name="ClearTV">Option to Clear TreeView before adding to TreeView</param>
        /// <param name="ParentText">Header Text for Project Display (Default/Blank = Name of CS File)</param>
        /// <param name="Controls">If Project has Designer File, Option to Parse Out Control Names</param>
        /// <param name="Functions">Node with all Functions from CS file</param>
        /// <param name="Regions">Nodes with Each Region and Functions Below</param>
        public void Load_ProjectFile_TV(TreeView TV, string ProjectFile, bool ClearTV = true, string ParentText = "-=Projects=-", bool Controls = true, bool Functions = true, bool Regions = true)
        {
            if (ClearTV) { TV.Nodes.Clear(); }

            TreeNode parent = Build_Parent_Project_Node(TV, ProjectFile, ahk.FileName(ProjectFile));

            if (Controls)
            {
                string designerPath = DesignerPath(ProjectFile);

                if (File.Exists(designerPath))
                {
                    TreeNode controlsNode = DesignerFile_TVNode(designerPath, "Controls");
                    parent.Nodes.Add(controlsNode);
                }
            }

            if (Functions)
            {
                TreeNode functionNode = Return_FunctionList_Node(TV, ProjectFile, "Functions");
                parent.Nodes.Add(functionNode);
            }

            if (Regions)
            {
                TreeNode regionsNode = Region_Node_FromFile(ProjectFile, "==Regions==");
                parent.Nodes.Add(regionsNode);
            }

            TV.Nodes.Add(parent);  // add assembled node to treeview
        }


        /// <summary> parses project file down into region nodes with functions under neath</summary>
        /// <param name="ProjectFile"> </param>
        public TreeNode Region_Node_FromFile(string ProjectFile, string ParentText = "==Regions==")
        {
            TreeNode parent = new TreeNode();  // level 1

            //parent.Text = ahk.FileName(ProjectFile);
            parent.Text = ParentText;
            parent.Tag = ProjectFile;

            TreeNode section = new TreeNode();
            TreeNode subsection = new TreeNode();
            TreeNode subsectionsub = new TreeNode();
            TreeNode CurrentParent = new TreeNode();


            string CodeString = ahk.FileRead(ProjectFile);

            List<string> LineList = lst.Text_To_List(CodeString, false, false, false);
            int RegionCount = 0;
            foreach (string Line in LineList)
            {
                if (IsRegion(Line))
                {
                    RegionCount++;

                    codeNode seg = new codeNode();

                    string RegionNamed = RegionName(Line);

                    if (RegionCount == 1)
                    {
                        section = new TreeNode();  // level 1
                        section.Text = RegionNamed;

                        seg.regionName = RegionNamed;
                        seg.filePath = ProjectFile;
                        seg.nodeType = "Region";


                        section.Tag = "CsRegion|" + ProjectFile + "|" + RegionNamed;
                        parent.Nodes.Add(section);
                        CurrentParent = parent;
                    }

                    if (RegionCount == 2)
                    {
                        subsection = new TreeNode();  // level 2
                        subsection.Text = RegionNamed;
                        subsection.Tag = "CsRegion|" + ProjectFile + "|" + RegionNamed;
                        section.Nodes.Add(subsection);
                        CurrentParent = section;
                    }

                    if (RegionCount == 3)
                    {
                        subsectionsub = new TreeNode();  // level 3
                        subsectionsub.Text = RegionNamed;
                        subsectionsub.Tag = "CsRegion|" + ProjectFile + "|" + RegionNamed;
                        subsection.Nodes.Add(subsectionsub);
                        CurrentParent = subsection;
                    }

                }

                if (IsEndRegion(Line))
                {
                    RegionCount--;
                }

                if (IsFunction(Line))
                {
                    string FunctionNamed = FunctionName(Line);

                    if (RegionCount == 0)
                    {
                        TreeNode nFunction = new TreeNode();  // level 2
                        nFunction.Text = FunctionNamed;
                        nFunction.Tag = "CsFunction|" + ProjectFile + "|" + FunctionNamed;
                        parent.Nodes.Add(nFunction);
                    }

                    if (RegionCount == 1)
                    {
                        TreeNode nFunction = new TreeNode();  // level 2
                        nFunction.Text = FunctionNamed;
                        nFunction.Tag = "CsFunction|" + ProjectFile + "|" + FunctionNamed;
                        section.Nodes.Add(nFunction);
                    }

                    if (RegionCount == 2)
                    {
                        TreeNode nFunction = new TreeNode();  // level 2
                        nFunction.Text = FunctionNamed;
                        nFunction.Tag = "CsFunction|" + ProjectFile + "|" + FunctionNamed;
                        subsection.Nodes.Add(nFunction);
                    }

                    if (RegionCount == 3)
                    {
                        TreeNode nFunction = new TreeNode();  // level 2
                        nFunction.Text = FunctionNamed;
                        nFunction.Tag = "CsFunction|" + ProjectFile + "|" + FunctionNamed;
                        subsectionsub.Nodes.Add(nFunction);
                    }

                }

            }


            //treeView3.Nodes.Add(parent);  // populate tree

            return parent;
        }



        public class codeNode
        {
            // ex:
            // mousePos mp = ahk.MouseGetPos("3");  // populate object with values

            public string functionName { get; set; }
            public string functionCode { get; set; }
            public string documentation { get; set; }
            public string regionName { get; set; }
            public string filePath { get; set; }

            public string nodeType { get; set; }   // Region/Function/Class
            public TreeNode NodeName { get; set; }
            public bool Public { get; set; }
            public string Tags { get; set; }
            public string Class { get; set; }
        }


        /// <summary> parse project list, create file / region / function sub folders</summary>
        /// <param name="List<string> ProjectList"> </param>
        public TreeNode Project_Region_Display(List<string> ProjectList)
        {
            TreeNode parent = new TreeNode();
            parent.Text = "Project Regions";
            //parent.Tag = ProjectFile;

            ProjectList = Project_Code_FileList(true);
            foreach (string Project in ProjectList)
            {
                TreeNode child = Region_Node_FromFile(Project);
                child.Text = ahk.FileName(Project);
                child.Tag = Project;
                parent.Nodes.Add(child);
            }

            return parent;
        }



        #endregion

        #region === Code Parse RadTreeView ===

        public string RadTree_CodeParseFile(RadTreeView RadTree, string LoadFilen = @"c:\users\jason\google drive\imdb\sqliter\ahk_dll\autohotkey.interop\ahk.cs", bool LoadFunctions = true, bool LoadControls = true, bool LoadRegions = true, bool Load_UsingNodes = false, bool ClearTree = true, bool Collapse = true)
        {
            if (RadTree == null) { return ""; }

            if (ClearTree) { tree.ClearTree(RadTree); }
            

            FileInfo finfo = new FileInfo(LoadFilen);

            // set global variables 
            string LoadFile = LoadFilen;
            string LoadFileName = finfo.Name;


            // read loadfile text, display on gui
            string Code = ahk.FileRead(LoadFilen);


            //if (Load_UsingNodes) { Populate_Using_Node(RadTree, LoadFilen, "Using", LoadFileName); }  // option to load Using Lines in Node View
            //List<string> functionslisted = Populate_Function_Dictionary(LoadFilen);   // populate dictionary with lists of functions and their file paths when loading each file


            if (LoadFunctions) { Populate_FunctionList_RadTv(RadTree, LoadFilen, "Functions", LoadFileName); }

            if (LoadRegions) { Populate_RegionList_RadTv(RadTree, LoadFilen, "Regions", LoadFileName); }

            if (LoadControls) { Populate_Controls_RadTv(RadTree, LoadFilen, "Controls", LoadFileName); }


            if (Collapse) { RadTree.CollapseAll(); }
            else { RadTree.ExpandAll(); }

            return Code;
        }

        /// <summary> populate treeview with "Functions" node and list of functions underneath. can attach to another parent node name</summary>
        /// <param name="TreeView TV"> </param>
        /// <param name=" ProjectFile"> </param>
        /// <param name="NodeName"> </param>
        /// <param name="ParentName"> </param>
        private void Populate_FunctionList_RadTv(RadTreeView TV, string ProjectFile, string NodeName = "Functions", string ParentName = "")
        {

            string Code = ahk.FileRead(ProjectFile);
            List<string> functionList = FunctionList_FromCode(Code);

            functionList.Sort();  // show function list sorted alpha

            // search for the Functions Node under LoadFileName
            //TreeNode functionSnode = tv.Find_Node(TV, NodeName, ParentName);

            RadTreeNode functionSnode = TV.Find(NodeName);

            if (functionSnode == null)
            {
                functionSnode = new RadTreeNode();
                functionSnode.Text = NodeName;
                functionSnode.Tag = ProjectFile;
                //functionSnode.Nodes.Add(entry);
                tree.AddNode(TV, functionSnode);
            }

            foreach (string item in functionList)
            {
                RadTreeNode entry = new RadTreeNode();
                entry.Text = item;
                entry.Tag = ProjectFile;
                //functionSnode.Nodes.Add(entry);
                tree.AddSubNode(functionSnode, entry, TV);
            }

        }

        /// <summary> populate treeview with controls from .cs project</summary>
        /// <param name="TreeView TV"> </param>
        /// <param name=" ParseFile"> </param>
        /// <param name="NodeName"> </param>
        /// <param name="ParentName"> </param>
        private void Populate_Controls_RadTv(RadTreeView TV, string ParseFile, string NodeName = "Controls", string ParentName = "")
        {
            string designerFile = DesignerPath(ParseFile);  // take project .cs path and add .designer.cs 

            if (ahk.IfExist(designerFile))
            {
                string loadFileName = "";

                if (ParentName != "") { loadFileName = ParentName; }
                if (ParentName == "") { loadFileName = ahk.FileName(ParseFile); } // name of the .cs file loading (name of parent node)

                //TreeNode parent = tv.ReturnNode(TV, loadFileName);  // return the parent node to add new control node under

                RadTreeNode parent = TV.Find(loadFileName);

                RadTreeNode controlsNode = DesignerFile_RadTVNode(designerFile, NodeName);

                parent.Nodes.Add(controlsNode);  // add control list node to treeview

                //tree.AddSubNode(parent, controlsNode, TV);
            }
        }

        /// <summary> parse project Designer.cs file - returns TreeNode populated with control names</summary>
        /// <param name="DesignerFilePath"> </param>
        /// <param name="NodeName"> </param>
        private RadTreeNode DesignerFile_RadTVNode(string DesignerFilePath, string NodeName = "Controls")
        {
            if (!File.Exists(DesignerFilePath)) { return null; }

            bool ParentBuilt = false;

            RadTreeNode Controls = new RadTreeNode();


            if (NodeName.ToUpper() == "DATAGRIDVIEW")
            {
                Controls.Text = NodeName.ToUpper();
                Controls.Tag = DesignerFilePath;

                List<string> Lines = lst.TextFile_To_List(DesignerFilePath);

                foreach (string Line in Lines)
                {
                    string FirstWord = ahk.FirstWord(Line);

                    if (FirstWord == "private")
                    {
                        if (Line.Contains("=")) { continue; }
                        if (Line.Contains("(")) { continue; }

                        bool Added = false;

                        string controlName = ahk.WordNum(Line, 3);  //third word in line is the name of the control
                        controlName = ahk.StringReplace(controlName, ";");
                        controlName = controlName.Trim();

                        string controlType = ahk.WordNum(Line, 2);


                        if (controlType == "System.Windows.Forms.DataGridView") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; Controls.Nodes.Add(newnode); }
                    }
                }

                return Controls;
            }
            if (NodeName.ToUpper() == "TREEVIEW")
            {
                Controls.Text = NodeName.ToUpper();
                Controls.Tag = DesignerFilePath;

                List<string> Lines = lst.TextFile_To_List(DesignerFilePath);

                foreach (string Line in Lines)
                {
                    string FirstWord = ahk.FirstWord(Line);

                    if (FirstWord == "private")
                    {
                        if (Line.Contains("=")) { continue; }
                        if (Line.Contains("(")) { continue; }

                        bool Added = false;

                        string controlName = ahk.WordNum(Line, 3);  //third word in line is the name of the control
                        controlName = ahk.StringReplace(controlName, ";");
                        controlName = controlName.Trim();

                        string controlType = ahk.WordNum(Line, 2);


                        if (controlType == "System.Windows.Forms.TreeView") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; Controls.Nodes.Add(newnode); }
                    }
                }

                return Controls;
            }
            if (NodeName.ToUpper() == "SCINTILLA")
            {
                Controls.Text = NodeName.ToUpper();
                Controls.Tag = DesignerFilePath;

                List<string> Lines = lst.TextFile_To_List(DesignerFilePath);

                foreach (string Line in Lines)
                {
                    string FirstWord = ahk.FirstWord(Line);

                    if (FirstWord == "private")
                    {
                        if (Line.Contains("=")) { continue; }
                        if (Line.Contains("(")) { continue; }

                        bool Added = false;

                        string controlName = ahk.WordNum(Line, 3);  //third word in line is the name of the control
                        controlName = ahk.StringReplace(controlName, ";");
                        controlName = controlName.Trim();

                        string controlType = ahk.WordNum(Line, 2);


                        if (controlType == "ScintillaNET.Scintilla") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; Controls.Nodes.Add(newnode); }
                    }
                }

                return Controls;
            }
            if (NodeName.ToUpper() == "LISTBOX")
            {
                Controls.Text = NodeName.ToUpper();
                Controls.Tag = DesignerFilePath;

                List<string> Lines = lst.TextFile_To_List(DesignerFilePath);

                foreach (string Line in Lines)
                {
                    string FirstWord = ahk.FirstWord(Line);

                    if (FirstWord == "private")
                    {
                        if (Line.Contains("=")) { continue; }
                        if (Line.Contains("(")) { continue; }

                        bool Added = false;

                        string controlName = ahk.WordNum(Line, 3);  //third word in line is the name of the control
                        controlName = ahk.StringReplace(controlName, ";");
                        controlName = controlName.Trim();

                        string controlType = ahk.WordNum(Line, 2);


                        if (controlType == "System.Windows.Forms.ListBox") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; Controls.Nodes.Add(newnode); }
                    }
                }

                return Controls;
            }
            if (NodeName.ToUpper() == "COMBOBOX")
            {
                Controls.Text = NodeName.ToUpper();
                Controls.Tag = DesignerFilePath;

                List<string> Lines = lst.TextFile_To_List(DesignerFilePath);

                foreach (string Line in Lines)
                {
                    string FirstWord = ahk.FirstWord(Line);

                    if (FirstWord == "private")
                    {
                        if (Line.Contains("=")) { continue; }
                        if (Line.Contains("(")) { continue; }

                        bool Added = false;

                        string controlName = ahk.WordNum(Line, 3);  //third word in line is the name of the control
                        controlName = ahk.StringReplace(controlName, ";");
                        controlName = controlName.Trim();

                        string controlType = ahk.WordNum(Line, 2);


                        if (controlType == "System.Windows.Forms.ComboBox") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; Controls.Nodes.Add(newnode); }
                    }
                }

                return Controls;
            }



            // Add ALL Controls To Single Node

            if (!ParentBuilt)
            {
                //setup treeview
                //TreeNode Controls = new RadTreeNode();
                Controls.Text = NodeName;

                //RadTreeNode BindingNavigatorNode = new RadTreeNode();
                //BindingNavigatorNode.Text = "BindingNavigators";
                //BindingNavigatorNode.Tag = "BindingNavigators";
                //Controls.Nodes.Add(BindingNavigatorNode);

                //RadTreeNode ButtonNode = new RadTreeNode();
                //ButtonNode.Text = "Buttons";
                //ButtonNode.Tag = "Buttons";
                //Controls.Nodes.Add(ButtonNode);

                //RadTreeNode CheckBoxNode = new RadTreeNode();
                //CheckBoxNode.Text = "CheckBoxes";
                //CheckBoxNode.Tag = "CheckBoxes";
                //Controls.Nodes.Add(CheckBoxNode);

                //RadTreeNode CheckedListBoxNode = new RadTreeNode();
                //CheckedListBoxNode.Text = "CheckedListBoxes";
                //CheckedListBoxNode.Tag = "CheckedListBoxes";
                //Controls.Nodes.Add(CheckedListBoxNode);

                //RadTreeNode ColorDialogNode = new RadTreeNode();
                //ColorDialogNode.Text = "ColorDialog";
                //ColorDialogNode.Tag = "ColorDialog";
                //Controls.Nodes.Add(ColorDialogNode);

                //RadTreeNode ComboBoxNode = new RadTreeNode();
                //ComboBoxNode.Text = "ComboBoxs";
                //ComboBoxNode.Tag = "ComboBoxs";
                //Controls.Nodes.Add(ComboBoxNode);

                //RadTreeNode ContextMenuStripNode = new RadTreeNode();
                //ContextMenuStripNode.Text = "ContextMenuStrips";
                //ContextMenuStripNode.Tag = "ContextMenuStrips";
                //Controls.Nodes.Add(ContextMenuStripNode);

                //RadTreeNode DataGridViewNode = new RadTreeNode();
                //DataGridViewNode.Text = "DataGridViews";
                //DataGridViewNode.Tag = "DataGridViews";
                //Controls.Nodes.Add(DataGridViewNode);

                //RadTreeNode DateTimePickerNode = new RadTreeNode();
                //DateTimePickerNode.Text = "DateTimePicker";
                //DateTimePickerNode.Tag = "DateTimePicker";
                //Controls.Nodes.Add(DateTimePickerNode);

                //RadTreeNode FileSystemWatcherNode = new RadTreeNode();
                //FileSystemWatcherNode.Text = "FileSystemWatchers";
                //FileSystemWatcherNode.Tag = "FileSystemWatchers";
                //Controls.Nodes.Add(FileSystemWatcherNode);

                //RadTreeNode FontDialogNode = new RadTreeNode();
                //FontDialogNode.Text = "FontDialog";
                //FontDialogNode.Tag = "FontDialog";
                //Controls.Nodes.Add(FontDialogNode);

                //RadTreeNode LabelNode = new RadTreeNode();
                //LabelNode.Text = "Labels";
                //LabelNode.Tag = "Labels";
                //Controls.Nodes.Add(LabelNode);

                //RadTreeNode ListBoxNode = new RadTreeNode();
                //ListBoxNode.Text = "ListBoxes";
                //ListBoxNode.Tag = "ListBoxes";
                //Controls.Nodes.Add(ListBoxNode);

                //RadTreeNode ListViewNode = new RadTreeNode();
                //ListViewNode.Text = "ListView";
                //ListViewNode.Tag = "ListView";
                //Controls.Nodes.Add(ListViewNode);

                //RadTreeNode MenuStripNode = new RadTreeNode();
                //MenuStripNode.Text = "MenuStrips";
                //MenuStripNode.Tag = "MenuStrips";
                //Controls.Nodes.Add(MenuStripNode);

                //RadTreeNode MonthCalendarNode = new RadTreeNode();
                //MonthCalendarNode.Text = "MonthCalendar";
                //MonthCalendarNode.Tag = "MonthCalendar";
                //Controls.Nodes.Add(MonthCalendarNode);

                //RadTreeNode NotifyIconNode = new RadTreeNode();
                //NotifyIconNode.Text = "NotifyIcon";
                //NotifyIconNode.Tag = "NotifyIcon";
                //Controls.Nodes.Add(NotifyIconNode);

                //RadTreeNode OpenFileDialogNode = new RadTreeNode();
                //OpenFileDialogNode.Text = "OpenFileDialogs";
                //OpenFileDialogNode.Tag = "OpenFileDialogs";
                //Controls.Nodes.Add(OpenFileDialogNode);

                //RadTreeNode PanelNode = new RadTreeNode();
                //PanelNode.Text = "Panels";
                //PanelNode.Tag = "Panels";
                //Controls.Nodes.Add(PanelNode);

                //RadTreeNode PictureBoxNode = new RadTreeNode();
                //PictureBoxNode.Text = "PictureBoxes";
                //Controls.Nodes.Add(PictureBoxNode);

                //RadTreeNode ProgressBarNode = new RadTreeNode();
                //ProgressBarNode.Text = "ProgressBars";
                //Controls.Nodes.Add(ProgressBarNode);

                //RadTreeNode PropertyGridNode = new RadTreeNode();
                //PropertyGridNode.Text = "PropertyGrid";
                //Controls.Nodes.Add(PropertyGridNode);

                //RadTreeNode RadioButtonNode = new RadTreeNode();
                //RadioButtonNode.Text = "RadioButton";
                //Controls.Nodes.Add(RadioButtonNode);

                //RadTreeNode RichTextBoxNode = new RadTreeNode();
                //RichTextBoxNode.Text = "RichTextBoxs";
                //Controls.Nodes.Add(RichTextBoxNode);

                //RadTreeNode SaveFileDialogNode = new RadTreeNode();
                //SaveFileDialogNode.Text = "SaveFileDialogs";
                //Controls.Nodes.Add(SaveFileDialogNode);

                //RadTreeNode ScintillaNode = new RadTreeNode();
                //ScintillaNode.Text = "Scintillas";
                //Controls.Nodes.Add(ScintillaNode);

                //RadTreeNode SplitterNode = new RadTreeNode();
                //SplitterNode.Text = "Splitters";
                //Controls.Nodes.Add(SplitterNode);

                //RadTreeNode TabControlNode = new RadTreeNode();
                //TabControlNode.Text = "TabControls";
                //Controls.Nodes.Add(TabControlNode);

                //RadTreeNode TabPageNode = new RadTreeNode();
                //TabPageNode.Text = "TabPages";
                //Controls.Nodes.Add(TabPageNode);

                //RadTreeNode TableLayoutPanelNode = new RadTreeNode();
                //TableLayoutPanelNode.Text = "TableLayoutPanels";
                //Controls.Nodes.Add(TableLayoutPanelNode);

                //RadTreeNode TextBoxNode = new RadTreeNode();
                //TextBoxNode.Text = "TextBoxes";
                //Controls.Nodes.Add(TextBoxNode);

                //RadTreeNode ToolStripButtonNode = new RadTreeNode();
                //ToolStripButtonNode.Text = "ToolStripButtons";
                //Controls.Nodes.Add(ToolStripButtonNode);

                //RadTreeNode ToolStripComboBoxNode = new RadTreeNode();
                //ToolStripComboBoxNode.Text = "ToolStripComboBoxs";
                //Controls.Nodes.Add(ToolStripComboBoxNode);

                //RadTreeNode ToolStripLabelNode = new RadTreeNode();
                //ToolStripLabelNode.Text = "ToolStripLabels";
                //Controls.Nodes.Add(ToolStripLabelNode);

                //RadTreeNode ToolStripMenuItemNode = new RadTreeNode();
                //ToolStripMenuItemNode.Text = "ToolStripMenuItems";
                //Controls.Nodes.Add(ToolStripMenuItemNode);

                //RadTreeNode ToolStripTextBoxNode = new RadTreeNode();
                //ToolStripTextBoxNode.Text = "ToolStripTextBoxs";
                //Controls.Nodes.Add(ToolStripTextBoxNode);

                //RadTreeNode ToolStripSeparatorNode = new RadTreeNode();
                //ToolStripSeparatorNode.Text = "ToolStripSeparators";
                //Controls.Nodes.Add(ToolStripSeparatorNode);

                //RadTreeNode TreeViewNode = new RadTreeNode();
                //TreeViewNode.Text = "TreeViews";
                //Controls.Nodes.Add(TreeViewNode);

                //RadTreeNode TreeViewFastNode = new RadTreeNode();
                //TreeViewFastNode.Text = "TreeViewFast";
                //Controls.Nodes.Add(TreeViewFastNode);

                //RadTreeNode WebBrowserNode = new RadTreeNode();
                //WebBrowserNode.Text = "WebBrowsers";
                //Controls.Nodes.Add(WebBrowserNode);


                //// Telerik Controls ===

                //RadTreeNode RadTextBoxNode = new RadTreeNode();
                //RadTextBoxNode.Text = "RadTextBox";
                //RadTextBoxNode.Tag = "RadTextBox";
                //Controls.Nodes.Add(RadTextBoxNode);

                //RadTreeNode RadLabelElementNode = new RadTreeNode();
                //RadLabelElementNode.Text = "RadLabelElement";
                //RadLabelElementNode.Tag = "RadLabelElement";
                //Controls.Nodes.Add(RadLabelElementNode);

                //RadTreeNode RadProgressBarNode = new RadTreeNode();
                //RadProgressBarNode.Text = "RadProgressBar";
                //RadProgressBarNode.Tag = "RadProgressBar";
                //Controls.Nodes.Add(RadProgressBarNode);

                //RadTreeNode RadTreeViewNode = new RadTreeNode();
                //RadTreeViewNode.Text = "RadTreeView";
                //RadTreeViewNode.Tag = "RadTreeView";
                //Controls.Nodes.Add(RadTreeViewNode);

                //RadTreeNode RadStatusStripNode = new RadTreeNode();
                //RadStatusStripNode.Text = "RadStatusStrip";
                //RadStatusStripNode.Tag = "RadStatusStrip";
                //Controls.Nodes.Add(RadStatusStripNode);

                //RadTreeNode RadButtonNode = new RadTreeNode();
                //RadButtonNode.Text = "RadButton";
                //RadButtonNode.Tag = "RadButton";
                //Controls.Nodes.Add(RadButtonNode);

                //RadTreeNode RadSplitContainerNode = new RadTreeNode();
                //RadSplitContainerNode.Text = "RadSplitContainer";
                //RadSplitContainerNode.Tag = "RadSplitContainer";
                //Controls.Nodes.Add(RadSplitContainerNode);

                //RadTreeNode SplitPanelNode = new RadTreeNode();
                //SplitPanelNode.Text = "SplitPanel";
                //SplitPanelNode.Tag = "SplitPanel";
                //Controls.Nodes.Add(SplitPanelNode);

                //RadTreeNode RadMenuNode = new RadTreeNode();
                //RadMenuNode.Text = "RadMenu";
                //RadMenuNode.Tag = "RadMenu";
                //Controls.Nodes.Add(RadMenuNode);

                //RadTreeNode RadPageViewNode = new RadTreeNode();
                //RadPageViewNode.Text = "RadPageView";
                //RadPageViewNode.Tag = "RadPageView";
                //Controls.Nodes.Add(RadPageViewNode);

                //RadTreeNode RadPageViewPageNode = new RadTreeNode();
                //RadPageViewPageNode.Text = "RadPageViewPage";
                //RadPageViewPageNode.Tag = "RadPageViewPage";
                //Controls.Nodes.Add(RadPageViewPageNode);

                //RadTreeNode RadLabelNode = new RadTreeNode();
                //RadLabelNode.Text = "RadLabel";
                //RadLabelNode.Tag = "RadLabel";
                //Controls.Nodes.Add(RadLabelNode);





                List<string> Lines = lst.TextFile_To_List(DesignerFilePath);

                List<string> controls = new List<string>();

                foreach (string Line in Lines)
                {
                    string FirstWord = ahk.FirstWord(Line);

                    if (FirstWord == "private")
                    {
                        if (Line.Contains("=")) { continue; }
                        if (Line.Contains("(")) { continue; }

                        bool Added = false;

                        string controlName = ahk.WordNum(Line, 3);  //third word in line is the name of the control
                        controlName = ahk.StringReplace(controlName, ";");
                        controlName = controlName.Trim();

                        string controlType = ahk.WordNum(Line, 2);



                        string controlTypeTrim = controlType.Replace("System.Windows.Forms.", "");
                        controlTypeTrim = controlTypeTrim.Replace("Telerik.WinControls.UI.", "");

                        //AddNodeIfMissing(RadTreeView RadTree, RadTreeNode NewNode, bool MatchByTagText = false, RadTreeNode ParentNode = null)


                        RadTreeNode ControlNameNode = new RadTreeNode();

                        // loop through currently building node to see if node already exists
                        bool found = false;
                        foreach (RadTreeNode cNode in Controls.Nodes)
                        {
                            if (cNode.Text == controlTypeTrim) { found = true; ControlNameNode = cNode; }
                        }
                        if (!found)  // if not found, create new node to add to 
                        {
                            ControlNameNode.Text = controlTypeTrim;
                            ControlNameNode.Tag = controlTypeTrim;
                            Controls.Nodes.Add(ControlNameNode);
                        }


                        Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ControlNameNode.Nodes.Add(newnode);



                        //if (controlType == "System.Windows.Forms.DataGridView") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; DataGridViewNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.Button") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ButtonNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.TreeView") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; TreeViewNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.TextBox") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; TextBoxNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.TabControl") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; TabControlNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.TabPage") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; TabPageNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.ListBox") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ListBoxNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.MenuStrip") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; MenuStripNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.ToolStripMenuItem") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ToolStripMenuItemNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.TableLayoutPanel") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; TableLayoutPanelNode.Nodes.Add(newnode); }
                        //if (controlType == "TreeViewFast.Controls.TreeViewFast") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; TreeViewFastNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.CheckBox") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; CheckBoxNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.Label") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; LabelNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.PictureBox") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; PictureBoxNode.Nodes.Add(newnode); }
                        //if (controlType == "ScintillaNET.Scintilla") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ScintillaNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.Splitter") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; SplitterNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.Panel") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; PanelNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.ToolStripSeparator") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ToolStripSeparatorNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.SaveFileDialog") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; SaveFileDialogNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.BindingNavigator") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; BindingNavigatorNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.OpenFileDialog") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; OpenFileDialogNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.WebBrowser") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; WebBrowserNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.ToolStripComboBox") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ToolStripComboBoxNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.ToolStripButton") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ToolStripButtonNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.ToolStripLabel") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ToolStripLabelNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.ToolStripTextBox") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ToolStripTextBoxNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.RichTextBox") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; RichTextBoxNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.ComboBox") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ComboBoxNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.ContextMenuStrip") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ContextMenuStripNode.Nodes.Add(newnode); }
                        //if (controlType == "System.IO.FileSystemWatcher") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; FileSystemWatcherNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.CheckedListBox") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; CheckedListBoxNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.DateTimePicker") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; DateTimePickerNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.MonthCalendar") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; MonthCalendarNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.FontDialog") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; FontDialogNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.PropertyGrid") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; PropertyGridNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.ListView") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ListViewNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.ColorDialog") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ColorDialogNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.NotifyIcon") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; NotifyIconNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.RadioButton") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; RadioButtonNode.Nodes.Add(newnode); }
                        //if (controlType == "System.Windows.Forms.ProgressBar") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; ProgressBarNode.Nodes.Add(newnode); }


                        //if (controlType == "Telerik.WinControls.UI.RadTextBox") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; RadTextBoxNode.Nodes.Add(newnode); }
                        //if (controlType == "Telerik.WinControls.UI.RadLabelElement") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; RadLabelElementNode.Nodes.Add(newnode); }
                        //if (controlType == "Telerik.WinControls.UI.RadProgressBar") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; RadProgressBarNode.Nodes.Add(newnode); }
                        //if (controlType == "Telerik.WinControls.UI.RadTreeView") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; RadTreeViewNode.Nodes.Add(newnode); }
                        //if (controlType == "Telerik.WinControls.UI.RadStatusStrip") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; RadStatusStripNode.Nodes.Add(newnode); }
                        //if (controlType == "Telerik.WinControls.UI.RadButton") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; RadButtonNode.Nodes.Add(newnode); }
                        //if (controlType == "Telerik.WinControls.UI.RadSplitContainer") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; RadSplitContainerNode.Nodes.Add(newnode); }
                        //if (controlType == "Telerik.WinControls.UI.SplitPanel") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; SplitPanelNode.Nodes.Add(newnode); }
                        //if (controlType == "Telerik.WinControls.UI.RadMenu") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; RadMenuNode.Nodes.Add(newnode); }
                        //if (controlType == "Telerik.WinControls.UI.RadPageView") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; RadPageViewNode.Nodes.Add(newnode); }
                        //if (controlType == "Telerik.WinControls.UI.RadPageViewPage") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; RadPageViewPageNode.Nodes.Add(newnode); }
                        //if (controlType == "Telerik.WinControls.UI.RadLabel") { Added = true; RadTreeNode newnode = new RadTreeNode(); newnode.Tag = DesignerFilePath; newnode.Text = controlName; RadLabelNode.Nodes.Add(newnode); }


                        if (!Added) { ahk.MsgBox("Didn't Add: " + controlType); }


                        //controls.Add(controlType);
                        //controls.Add(controlName);
                        //controls.Add(Line);
                    }
                }


            }


            //listBox1.DataSource = controls;
            //tv.Nodes.Add(Controls); // populate treeview

            return Controls;
        }


        public void Populate_RegionList_RadTv(RadTreeView TV, string ParseFile, string NodeName = "Regions", string ParentName = "", bool NoParent = false)
        {
            string loadFileName = ahk.FileName(ParseFile);

            RadTreeNode parent = TV.Find(loadFileName); // tv.ReturnNode(TV, loadFileName);  // return the parent node to add new control node under

            if (parent == null)
            {
                parent = new RadTreeNode(loadFileName);
                parent.Tag = ParseFile;
                tree.AddNode(TV, parent);
            }

            if (!NoParent)
            {
                RadTreeNode regionsNode = Generate_RegionList_RadTv(TV, ParseFile, "Regions", loadFileName);
                parent.Nodes.Add(regionsNode);  // add control list node to treeview
            }
            else
            {
                RadTreeNode regionsNode = Generate_RegionList_RadTv(TV, ParseFile, "", "");
                parent.Nodes.Add(regionsNode);  // add control list node to treeview
            }





            // list of functions not in regions
            List<string> noRegionList = FunctionList_Not_In_Regions_FromFile(ParseFile);

            if (noRegionList.Count > 0)  // if any functions are found outside of regions, add to noregion node
            {
                RadTreeNode NoRegionsNode = new RadTreeNode();
                NoRegionsNode.Text = "No Region";
                NoRegionsNode.Tag = ParseFile;
                //tree.AddNode(TV, NoRegionsNode);
                parent.Nodes.Add(NoRegionsNode);  // add control list node to treeview

                foreach (string noReg in noRegionList)
                {
                    RadTreeNode noRegionsNode = new RadTreeNode();
                    noRegionsNode.Text = noReg;
                    noRegionsNode.Tag = ParseFile;
                    tree.AddSubNode(NoRegionsNode, noRegionsNode, TV);
                }
            }


            return;

            string Code = ahk.FileRead(ParseFile);
            string FileTag = ParseFile;





            List<string> LineList = ahk.LineList(Code);

            string RegionCodeSection = "";
            bool RegionStart = false;
            bool RegionCapture = false;
            string ThisRegionName = "";
            int RegionCounter = 0;
            bool InFunction = false;
            string ThisFunctionName = "";
            int BracketCount = 0;
            bool BracketStart = false;

            foreach (string Line in LineList)
            {
                if (Line == "") { continue; }


                //### Check To See If We're Inside a function before attempting to grab the region start/end  #####

                bool FunctionCheck = IsFunction(Line);

                if (FunctionCheck)
                {
                    InFunction = true;
                    ThisFunctionName = FunctionName(Line);

                    //if (ThisFunctionName == "}") { InFunction = false; }  // fixes get-set lines
                }

                if (InFunction)
                {
                    if (Line.Contains("{"))
                    {
                        int bcount = Line.Count(f => f == '{');  // count number of brackets in line
                        BracketStart = true;
                        BracketCount += bcount;
                    }

                    if (BracketStart)
                    {
                        if (Line.Contains("}"))
                        {
                            int bcount = Line.Count(f => f == '}');  // count number of brackets in line
                            BracketCount -= bcount;
                        }

                        if (BracketCount == 0)
                        {
                            InFunction = false;
                            ThisFunctionName = "";
                            BracketStart = false;
                        }
                    }
                }


                if (IsCommentLine(Line)) { continue; }

                if (!InFunction) { RegionStart = IsRegion(Line); }

                if (!InFunction)
                {
                    if (RegionStart)  // beginning of region
                    {
                        if (RegionCounter == 0)
                        {
                            RegionCounter++;
                            RegionCapture = true;
                            ThisRegionName = RegionName(Line);
                        }

                    }

                    bool EndofRegion = false;
                    if (!InFunction) { EndofRegion = IsEndRegion(Line); }

                    if (IsEndRegion(Line))  // end of region
                    {
                        if (ThisRegionName == "") { continue; }

                        RegionCounter--;

                        if (RegionCounter == 0)
                        {
                            RegionCapture = false;

                            //ahk.MsgBox(RegionCodeSection); 

                            // search for the Regions Node under LoadFileName
                            RadTreeNode regionSnode = TV.Find(NodeName); // tv.Find_Node(TV, NodeName, ParentName);

                            if (regionSnode == null)
                            {
                                regionSnode = new RadTreeNode();
                                regionSnode.Text = "Regions";
                            }

                            bool NodeExists = false; //tv.NodeExist(TV, ThisRegionName);
                            RadTreeNode nodeCheck = TV.Find(ThisRegionName); // tv.Find_Node(TV, NodeName, ParentName);
                            if (nodeCheck != null) { NodeExists = true; }

                            TV.Refresh();
                            if (!NodeExists)
                            {
                                RadTreeNode regionnode = new RadTreeNode();  // level 2
                                regionnode.Text = ThisRegionName;
                                regionnode.Tag = FileTag;
                                regionSnode.Nodes.Add(regionnode);
                            }


                            // loop through region section and display functions in region section

                            List<string> RegionLines = ahk.LineList(RegionCodeSection);
                            List<string> RegionFunctions = new List<string>();
                            foreach (string reg in RegionLines)
                            {
                                bool FunctionLine = IsFunction(reg);
                                if (FunctionLine)
                                {
                                    string Function = FunctionName(reg);
                                    string Params = FunctionParams(reg);
                                    string Comments = FunctionComments(reg);

                                    RegionFunctions.Add(Function);

                                    //bool NodeFount = tv.NodeExist(TV, ThisRegionName);

                                    bool NodeFount = false; //tv.NodeExist(TV, ThisRegionName);
                                    RadTreeNode nodeCheck2 = TV.Find(ThisRegionName); // tv.Find_Node(TV, NodeName, ParentName);
                                    if (nodeCheck2 != null) { NodeFount = true; }

                                    if (NodeFount)
                                    {
                                        RadTreeNode regionnode = TV.Find(ThisRegionName); // tv.ReturnNode(TV, ThisRegionName);

                                        RadTreeNode region = new RadTreeNode();
                                        region.Text = Function;
                                        region.Tag = FileTag;
                                        regionnode.Nodes.Add(region);
                                    }



                                    //ahk.MsgBox(Function + Environment.NewLine + Params + Environment.NewLine + Comments);
                                }

                            }


                            //ahk.MsgBox("Region: " + ThisRegionName + " Has " + RegionFunctions.Count.ToString() + " Functions");

                            RegionCodeSection = "";
                            ThisRegionName = "";

                        }


                    }

                }

                if (RegionCapture)
                {
                    if (RegionCodeSection != "") { RegionCodeSection = RegionCodeSection + Environment.NewLine + Line; }
                    if (RegionCodeSection == "") { RegionCodeSection = Line; }
                }

            }


        }


        /// <summary> generates region nodes from .cs file, can attach to another parent node</summary>
        /// <param name="TreeView TV"> </param>
        /// <param name=" ParseFile"> </param>
        /// <param name="NodeName"> </param>
        /// <param name="ParentName"> </param>
        public RadTreeNode Generate_RegionList_RadTv(RadTreeView TV, string ParseFile, string NodeName = "Regions", string ParentName = "")
        {
            string Code = ahk.FileRead(ParseFile);
            string FileTag = ParseFile;

            NodeInfo node = new NodeInfo();
            node.FilePath = ParseFile;

            Dictionary<string, RadTreeNode> NodeDict = new Dictionary<string, RadTreeNode>();

            bool WriteSQL = false;
            string SQLiteDb = @"C:\Users\jason\Google Drive\IMDB\SQLiter\Db\FunctionExport.sqlite";
            //if (WriteSQL)
            //{
            //    sqlite.Create_SettingsDb(SQLiteDb, "Function_Lib", true);
            //}


            List<string> LineList = ahk.LineList(Code);

            string RegionCodeSection = "";
            bool RegionStart = false;
            bool RegionCapture = false;
            string ThisRegionName = "";
            int RegionCounter = 0;
            bool InFunction = false;
            string ThisFunctionName = "";
            int BracketCount = 0;
            bool BracketStart = false;

            // search for the Regions Node under LoadFileName
            RadTreeNode regionSnode = TV.Find(NodeName);   //tv.Find_Node(TV, NodeName, ParentName);

            // create new node if it doesn't already exist
            if (regionSnode == null)
            {
                regionSnode = new RadTreeNode();
                regionSnode.Text = NodeName;
            }


            NodeDict.Add(ParentName, regionSnode);


            foreach (string Line in LineList)
            {
                if (Line == "") { continue; }


                //### Check To See If We're Inside a function before attempting to grab the region start/end  #####

                bool FunctionCheck = IsFunction(Line);

                if (FunctionCheck)
                {
                    InFunction = true;
                    ThisFunctionName = FunctionName(Line);

                    //if (ThisFunctionName == "}") { InFunction = false; }  // fixes get-set lines
                }

                if (InFunction)
                {
                    if (Line.Contains("{"))
                    {
                        int bcount = Line.Count(f => f == '{');  // count number of brackets in line
                        BracketStart = true;
                        BracketCount += bcount;
                    }

                    if (BracketStart)
                    {
                        if (Line.Contains("}"))
                        {
                            int bcount = Line.Count(f => f == '}');  // count number of brackets in line
                            BracketCount -= bcount;
                        }

                        if (BracketCount == 0)
                        {
                            InFunction = false;
                            ThisFunctionName = "";
                            BracketStart = false;
                        }
                    }
                }


                if (IsCommentLine(Line)) { continue; }

                if (!InFunction) { RegionStart = IsRegion(Line); }

                if (!InFunction)
                {
                    if (RegionStart)  // beginning of region
                    {
                        if (RegionCounter == 0)
                        {
                            RegionCounter++;
                            RegionCapture = true;
                            ThisRegionName = RegionName(Line);
                        }

                    }

                    bool EndofRegion = false;
                    if (!InFunction) { EndofRegion = IsEndRegion(Line); }

                    if (IsEndRegion(Line))  // end of region
                    {
                        if (ThisRegionName == "") { continue; }

                        RegionCounter--;

                        if (RegionCounter == 0)
                        {
                            RegionCapture = false;

                            //ahk.MsgBox(RegionCodeSection); 

                            if (!NodeDict.ContainsKey(ThisRegionName))
                            {
                                RadTreeNode regionnode = new RadTreeNode();  // level 2
                                regionnode.Text = ThisRegionName;
                                regionnode.Tag = FileTag;
                                regionSnode.Nodes.Add(regionnode);

                                NodeDict.Add(ThisRegionName, regionnode);

                                if (WriteSQL) { sqlite.Setting_Save("Function_Lib", NodeName, ThisRegionName, SQLiteDb); }
                            }


                            //bool NodeExists = tv.NodeExist(TV, ThisRegionName);

                            ////TV.Refresh();
                            //if (!NodeExists)
                            //{

                            //}


                            // loop through region section and display functions in region section

                            List<string> RegionLines = ahk.LineList(RegionCodeSection);
                            List<string> RegionFunctions = new List<string>();
                            foreach (string reg in RegionLines)
                            {
                                bool FunctionLine = IsFunction(reg);
                                if (FunctionLine)
                                {
                                    string Function = FunctionName(reg);
                                    string Params = FunctionParams(reg);
                                    string Comments = FunctionComments(reg);

                                    RegionFunctions.Add(Function);

                                    //bool NodeFount = tv.NodeExist(TV, ThisRegionName);

                                    if (NodeDict.ContainsKey(ThisRegionName))
                                    {
                                        //tv.ReturnNode(TV, ThisRegionName);

                                        RadTreeNode regionnode = null;
                                        RadTreeNode regionval = null;

                                        if (NodeDict.TryGetValue(ThisRegionName, out regionval)) // Returns false.
                                        {
                                            regionnode = regionval;
                                        }

                                        RadTreeNode region = new RadTreeNode();
                                        region.Text = Function;
                                        region.Tag = FileTag;
                                        regionnode.Nodes.Add(region);

                                        if (!NodeDict.ContainsKey(Function)) { NodeDict.Add(Function, region); }

                                        if (WriteSQL) { sqlite.Setting_Save(SQLiteDb, "Function_Lib", NodeName, ThisRegionName); }

                                    }


                                    //ahk.MsgBox(Function + Environment.NewLine + Params + Environment.NewLine + Comments);
                                }

                            }


                            //ahk.MsgBox("Region: " + ThisRegionName + " Has " + RegionFunctions.Count.ToString() + " Functions");

                            RegionCodeSection = "";
                            ThisRegionName = "";

                        }


                    }

                }

                if (RegionCapture)
                {
                    if (RegionCodeSection != "") { RegionCodeSection = RegionCodeSection + Environment.NewLine + Line; }
                    if (RegionCodeSection == "") { RegionCodeSection = Line; }
                }

            }


            //foreach (var item in NodeDict)
            //{
            //    ahk.MsgBox(item.Key);
            //}

            //TV.Nodes.Add(regionSnode); // populate treeview

            return regionSnode;

        }




        #endregion

        #region === Insert / Update Code ===


        #region === Insert DLL References ===


        // check for missing dll / using references, compare with saved list (DLLReferenceList() and Custom_Using_List()) - return code to add to new project

        /// <summary> returns string with dll references that aren't in FilePath</summary>
        /// <param name="FilePath"> </param>
        public string Return_Missing_DLL_Reference_Code(string FilePath)
        {
            List<string> Return_Missing = Return_Missing_DLL_Reference_List(FilePath);

            string RefText = "";

            foreach (string refer in Return_Missing)
            {
                if (RefText != "") { RefText = RefText + Environment.NewLine + refer; }
                if (RefText == "") { RefText = refer; }
            }

            return RefText;
        }

        /// <summary> returns list with dll references that arent' in FilePath</summary>
        /// <param name="FilePath"> </param>
        public List<string> Return_Missing_DLL_Reference_List(string FilePath)
        {
            List<string> RefList = DLLReferenceList();
            List<string> AddRefList = new List<string>();

            foreach (string line in RefList)
            {
                bool Found = Line_In_CodeFile(FilePath, line);  // returns true if TextLine is found in FilePath

                if (!Found) { AddRefList.Add(line); }
            }

            return AddRefList;
        }

        /// <summary> returns list of USING references that arent' in FilePath</summary>
        /// <param name="FilePath"> </param>
        public List<string> Return_Missing_USING_Reference_List(string FilePath)
        {
            List<string> UsingList = Custom_Using_List();
            List<string> AddUsingList = new List<string>();

            foreach (string line in UsingList)
            {
                bool Found = Line_In_CodeFile(FilePath, line);  // returns true if TextLine is found in FilePath

                if (!Found) { AddUsingList.Add(line); }
            }

            return AddUsingList;
        }

        /// <summary> returns string with USING references that arent' in FilePath</summary>
        /// <param name="FilePath"> </param>
        public string Return_Missing_USING_Reference_Code(string FilePath)
        {
            List<string> Return_Missing = Return_Missing_USING_Reference_List(FilePath);

            string RefText = "";

            foreach (string refer in Return_Missing)
            {
                if (RefText != "") { RefText = RefText + Environment.NewLine + refer; }
                if (RefText == "") { RefText = refer; }
            }

            return RefText;
        }


        #endregion


        #region === Insert / Update / Remove Code Sections ===

        /// <summary>insert line() into bottom of existing function in .cs file</summary>
        /// <param name="ProjectFile"> </param>
        /// <param name=" FunctionNAME"> </param>
        /// <param name=" NewLine"> </param>
        /// <param name=" out ErrorMsg"> </param>
        public bool Insert_Line_In_Function_ToFile(string ProjectFile, string FunctionNAME, string NewLine, out string ErrorMsg)
        {
            ErrorMsg = ProjectFile + " Not Found - Unable To Insert";
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found."); return false; }
            string ParseCode = ahk.FileRead(ProjectFile);

            bool Success = false;
            string NewFunctionCode = Insert_Line_In_Function_ToCode(ParseCode, FunctionNAME, NewLine, out Success);
            if (!Success) { ErrorMsg = "Unable To Insert New Line - " + FunctionNAME + " Not Found In " + ahk.FileName(ProjectFile); return false; }

            // update function here

            bool Updated = Update_Function_Code_ToFile(ProjectFile, FunctionNAME, NewFunctionCode, out ErrorMsg);
            if (!Updated) { return false; }

            ErrorMsg = "Inserted New Line in Function: " + FunctionNAME + " Into: " + ahk.FileName(ProjectFile);
            return true;
        }

        /// <summary>insert line(s) into bottom of existing function in .cs code string</summary>
        /// <param name="CodeString"> </param>
        /// <param name=" FunctionNAME"> </param>
        /// <param name=" NewLine"> </param>
        /// <param name=" out Success"> </param>
        public string Insert_Line_In_Function_ToCode(string CodeString, string FunctionNAME, string NewLine, out bool Success)
        {
            Success = false;
            bool AlreadyInFunction = Line_In_Function_FromCode(CodeString, FunctionNAME, NewLine);
            if (AlreadyInFunction) { return CodeString; }  // already there, nothing to do, return original code

            bool FunctionExists = Function_InCode(CodeString, FunctionNAME);  // check to see if project file already has this function}
            if (!FunctionExists) { return CodeString; }

            string InsertAfterLine = Last_CodeLine_In_Function_FromCode(CodeString, FunctionNAME);

            //string UpdatedCode = Insert_After_Line_ToCode(CodeString, InsertAfterLine, NewLine);
            string FunctionCode = Return_Function_FromCode(CodeString, FunctionNAME);

            List<string> LineListF = lst.Text_To_List(FunctionCode, false, false, false);

            string UpdatedFunctionCode = "";
            string LastLine = "";

            foreach (string Line in LineListF)
            {
                if (Line.Trim() != InsertAfterLine.Trim())
                {
                    bool ISFunction = IsFunction(Line);
                    if (Line.Trim() != "" && Line.Trim() != "{" && Line.Trim() != "}" && ISFunction != true) { LastLine = Line; }
                    if (UpdatedFunctionCode != "") { UpdatedFunctionCode = UpdatedFunctionCode + Environment.NewLine + Line; }
                    if (UpdatedFunctionCode == "") { UpdatedFunctionCode = Line; }
                }

                if (Line.Trim() == InsertAfterLine.Trim())
                {
                    if (LastLine == "") { LastLine = InsertAfterLine; }

                    string NewLineWithSpacesAdded = ahk.AddLeadingSpaces(NewLine, ahk.LeadingSpaceCount(LastLine));

                    if (LastLine.Trim() == "{") { NewLineWithSpacesAdded = "\t\t\t" + NewLine; }  // add three tabs if there weren't any other lines in the function to match space count

                    string WriteLine = Line + Environment.NewLine + NewLineWithSpacesAdded;

                    if (UpdatedFunctionCode != "") { UpdatedFunctionCode = UpdatedFunctionCode + Environment.NewLine + WriteLine; }
                    if (UpdatedFunctionCode == "") { UpdatedFunctionCode = WriteLine; }
                }

            }

            Success = true;
            return UpdatedFunctionCode;
        }

        /// <summary> search code for specific line, insert new code after that line</summary>
        /// <param name="ProjectFile"> </param>
        /// <param name=" LineToInsertAfter"> </param>
        /// <param name=" NewCode"> </param>
        /// <param name=" out ErrorMsg"> </param>
        public bool Insert_After_Line_ToFile(string ProjectFile, string LineToInsertAfter, string NewCode, out string ErrorMsg)
        {
            // Read Project File Text - Return Error If Not Found
            ErrorMsg = ProjectFile + " Not Found.";
            if (!File.Exists(ProjectFile)) { if (GlobalDebug) { ahk.MsgBox(ErrorMsg); } return false; }
            string ParseCode = ahk.FileRead(ProjectFile);


            string AllText = Insert_After_Line_ToCode(ParseCode, LineToInsertAfter, NewCode);


            // Write New ProjectFile with Updated Code, Backing up the Project File First
            bool WroteNewFile = Write_ProjectFile_With_Backup(ProjectFile, AllText, out ErrorMsg);
            if (!WroteNewFile) { return false; }

            ErrorMsg = "Inserted New Line Into " + ahk.FileName(ProjectFile);
            return true;
        }

        /// <summary> search code for specific line, insert new code after that line</summary>
        /// <param name="CodeString"> </param>
        /// <param name=" LineToInsertAfter"> </param>
        /// <param name=" NewCode"> </param>
        public string Insert_After_Line_ToCode(string CodeString, string LineToInsertAfter, string NewCode)
        {
            List<string> Lines = lst.Text_To_List(CodeString, false, false, false);

            bool Inserted = false;

            string AllText = "";

            foreach (string line in Lines)
            {
                string LineCheck = line;
                string WriteText = line;

                if (!Inserted)
                {
                    if (LineCheck.Trim() == LineToInsertAfter.Trim())
                    {
                        WriteText = line + Environment.NewLine + NewCode + Environment.NewLine + Environment.NewLine;
                        Inserted = true;
                    }
                }


                if (AllText != "") { AllText = AllText + Environment.NewLine + WriteText; }
                if (AllText == "") { AllText = WriteText; }
            }

            return AllText;
        }

        /// <summary> write new code before first function in file</summary>
        /// <param name="ProjectFile"> </param>
        /// <param name=" NewCode"> </param>
        /// <param name=" out ErrorMsg"> </param>
        public bool Insert_Code_Before_First_Function_ToFile(string ProjectFile, string NewCode, out string ErrorMsg)
        {
            // Read Project File Text - Return Error If Not Found
            ErrorMsg = ProjectFile + " Not Found.";
            if (!File.Exists(ProjectFile)) { if (GlobalDebug) { ahk.MsgBox(ErrorMsg); } return false; }
            string ParseCode = ahk.FileRead(ProjectFile);


            string NewCodeAll = Insert_Code_Before_First_Function_ToCode(ParseCode, NewCode);


            // Write New ProjectFile with Updated Code, Backing up the Project File First
            bool WroteNewFile = Write_ProjectFile_With_Backup(ProjectFile, NewCodeAll, out ErrorMsg);
            if (!WroteNewFile) { return false; }

            ErrorMsg = "Inserted Code Before First Function Into " + ahk.FileName(ProjectFile);
            return true;
        }

        /// <summary> write new code before first function in file</summary>
        /// <param name="CodeString"> </param>
        /// <param name=" NewCode"> </param>
        public string Insert_Code_Before_First_Function_ToCode(string CodeString, string NewCode)
        {
            string FirstFunctionName = FirstFunction_FromCode(CodeString);
            string FirstFunctionCode = Return_Function_FromCode(CodeString, FirstFunctionName);

            FirstFunctionCode = NewCode + Environment.NewLine + Environment.NewLine + FirstFunctionCode;

            string NewCodeAll = Update_Function_Code_ToCode(CodeString, FirstFunctionName, FirstFunctionCode);

            return NewCodeAll;
        }

        /// <summary> write new code after last function in file</summary>
        /// <param name="ProjectFile"> </param>
        /// <param name=" NewCode"> </param>
        /// <param name=" out ErrorMsg"> </param>
        public bool Insert_Code_After_Last_Function_ToFile(string ProjectFile, string NewCode, out string ErrorMsg)
        {
            // Read Project File Text - Return Error If Not Found
            ErrorMsg = ProjectFile + " Not Found.";
            if (!File.Exists(ProjectFile)) { if (GlobalDebug) { ahk.MsgBox(ErrorMsg); } return false; }
            string ParseCode = ahk.FileRead(ProjectFile);

            string NewCodeAll = Insert_Code_After_Last_Function_ToCode(ParseCode, NewCode);


            // Write New ProjectFile with Updated Code, Backing up the Project File First
            bool WroteNewFile = Write_ProjectFile_With_Backup(ProjectFile, NewCodeAll, out ErrorMsg);
            if (!WroteNewFile) { return false; }

            ErrorMsg = "Inserted Code After Last Funtion Into " + ahk.FileName(ProjectFile);
            return true;
        }

        /// <summary> write new code after last function in code string</summary>
        /// <param name="CodeString"> </param>
        /// <param name=" NewCode"> </param>
        public string Insert_Code_After_Last_Function_ToCode(string CodeString, string NewCode)
        {
            string LastFunctionName = LastFunction_FromCode(CodeString);
            string LastFunctionCode = Return_Function_FromCode(CodeString, LastFunctionName);

            LastFunctionCode = LastFunctionCode + Environment.NewLine + Environment.NewLine + NewCode;

            string NewCodeAll = Update_Function_Code_ToCode(CodeString, LastFunctionName, LastFunctionCode);

            return NewCodeAll;
        }

        /// <summary> write new code after last function in file</summary>
        /// <param name="ProjectFile"> </param>
        /// <param name=" InsertAfterFunctionName"> </param>
        /// <param name=" NewCode"> </param>
        /// <param name=" out ErrorMsg"> </param>
        public bool Insert_Function_After_Function_ToFile(string ProjectFile, string InsertAfterFunctionName, string NewCode, out string ErrorMsg)
        {
            // Read Project File Text - Return Error If Not Found
            ErrorMsg = ProjectFile + " Not Found.";
            if (!File.Exists(ProjectFile)) { if (GlobalDebug) { ahk.MsgBox(ErrorMsg); } return false; }
            string ParseCode = ahk.FileRead(ProjectFile);


            string NewCodeAll = Insert_Function_After_Function_ToCode(ParseCode, InsertAfterFunctionName, NewCode);


            // Write New ProjectFile with Updated Code, Backing up the Project File First
            bool WroteNewFile = Write_ProjectFile_With_Backup(ProjectFile, NewCodeAll, out ErrorMsg);
            if (!WroteNewFile) { return false; }

            ErrorMsg = "Inserted New Function After " + InsertAfterFunctionName + " Into " + ahk.FileName(ProjectFile);
            return true;
        }

        /// <summary> write new code after last function in file</summary>
        /// <param name="CodeString"> </param>
        /// <param name=" InsertAfterFunctionName"> </param>
        /// <param name=" NewCode"> </param>
        public string Insert_Function_After_Function_ToCode(string CodeString, string InsertAfterFunctionName, string NewCode)
        {
            string ExistingFunctionCode = Return_Function_FromCode(CodeString, InsertAfterFunctionName);

            ExistingFunctionCode = ExistingFunctionCode + Environment.NewLine + Environment.NewLine + NewCode;

            string NewCodeAll = Update_Function_Code_ToCode(CodeString, InsertAfterFunctionName, ExistingFunctionCode);

            return NewCodeAll;
        }

        /// <summary> find the "InitializeComponent();" line and insert code after that line</summary>
        /// <param name="ProjectFile"> </param>
        /// <param name=" NewFunctionText"> </param>
        /// <param name=" out ErrorMsg"> </param>
        public bool Insert_Startup_Code_ToFile(string ProjectFile, string NewFunctionText, out string ErrorMsg)
        {
            // Read Project File Text - Return Error If Not Found
            ErrorMsg = ProjectFile + " Not Found.";
            if (!File.Exists(ProjectFile)) { if (GlobalDebug) { ahk.MsgBox(ErrorMsg); } return false; }
            string ParseCode = ahk.FileRead(ProjectFile);

            string AllText = Insert_Startup_Code_ToCode(ParseCode, NewFunctionText);


            // Write New ProjectFile with Updated Code, Backing up the Project File First
            bool WroteNewFile = Write_ProjectFile_With_Backup(ProjectFile, AllText, out ErrorMsg);
            if (!WroteNewFile) { return false; }

            ErrorMsg = "Inserted Startup Code Into " + ahk.FileName(ProjectFile);
            return true;
        }

        /// <summary> find the "InitializeComponent();" line and insert code after that line</summary>
        /// <param name="CodeString"> </param>
        /// <param name=" NewFunctionText"> </param>
        public string Insert_Startup_Code_ToCode(string CodeString, string NewFunctionText)
        {
            List<string> LineListF = lst.Text_To_List(CodeString, false, false, false);

            int BracketCount = 0;
            bool BracketStart = false;
            bool FunctionCapture = false;
            string FunctionCapText = "";
            string ThisFunctionName = "";
            bool InsertCode = false;
            int spacecount = 0;
            string InitLine = "";

            string AllText = "";

            foreach (string Line in LineListF)
            {
                if (InsertCode)
                {
                    List<string> AddLines = lst.Text_To_List(NewFunctionText);

                    foreach (string AddLine in AddLines)  // loop through each new line to add, add leading spaces to fit on init line
                    {
                        string LineWithSpacesAdded = ahk.AddLeadingSpaces(AddLine, ahk.LeadingSpaceCount(InitLine));
                        AllText = AllText + Environment.NewLine + LineWithSpacesAdded;
                    }


                    InsertCode = false;
                }

                string LineCheck = Line;
                if (LineCheck.Trim() == "InitializeComponent();")
                {
                    InitLine = Line;
                    InsertCode = true;
                }

                string WriteLine = Line;

                if (!FunctionCapture)
                {
                    if (AllText != "") { AllText = AllText + Environment.NewLine + WriteLine; }
                    if (AllText == "") { AllText = WriteLine; }
                }

            }


            return AllText;
        }

        /// <summary> update existing function, overwrite old function code - OLD doesn't account for documentation changes</summary>
        /// <param name="ProjectFile"> </param>
        /// <param name=" FunctionNAME"> </param>
        /// <param name=" NewFunctionText"> </param>
        /// <param name=" out ErrorMsg"> </param>
        public bool Update_Function_Code_ToFile(string ProjectFile, string FunctionNAME, string NewFunctionText, out string ErrorMsg)
        {
            // Read Project File Text - Return Error If Not Found
            ErrorMsg = ProjectFile + " Not Found.";
            if (!File.Exists(ProjectFile)) { if (GlobalDebug) { ahk.MsgBox(ErrorMsg); } return false; }
            string ParseCode = ahk.FileRead(ProjectFile);


            string AllText = Update_Function_Code_ToCode(ParseCode, FunctionNAME, NewFunctionText);


            // Write New ProjectFile with Updated Code, Backing up the Project File First
            bool WroteNewFile = Write_ProjectFile_With_Backup(ProjectFile, AllText, out ErrorMsg);
            if (!WroteNewFile) { return false; }

            ErrorMsg = "Updated Function " + FunctionNAME + " In " + ahk.FileName(ProjectFile);
            return true;
        }

        /// <summary> update existing function, overwrite old function code OLD - doesn't account for documentation updates</summary>
        /// <param name="CodeString"> </param>
        /// <param name=" FunctionNAME"> </param>
        /// <param name=" NewFunctionText"> </param>
        public string Update_Function_Code_ToCode(string CodeString, string FunctionNAME, string NewFunctionText)
        {
            List<string> LineListF = lst.Text_To_List(CodeString, false, false, false);

            int BracketCount = 0;
            bool BracketStart = false;
            bool FunctionCapture = false;
            string FunctionCapText = "";
            string ThisFunctionName = "";
            //int LineNum = 1;

            string AllText = "";

            foreach (string Line in LineListF)
            {
                //string LineCheck = Line;
                //if (LineCheck.Trim() == "") { LineNum++;  continue; }

                string WriteLine = Line;

                bool FunctionLine = IsFunction(Line);

                if (FunctionLine)
                {
                    ThisFunctionName = FunctionName(Line);

                    if (ThisFunctionName == FunctionNAME) { FunctionCapture = true; }
                }

                if (FunctionCapture)
                {
                    if (FunctionCapText != "") { FunctionCapText = FunctionCapText + Environment.NewLine + Line; }
                    if (FunctionCapText == "") { FunctionCapText = Line; }

                    if (Line.Contains("{"))
                    {
                        int bCount = Line.Count(f => f == '{');  // count number of brackets to account for in this line
                        BracketStart = true;
                        BracketCount += bCount;
                    }

                    if (BracketStart)
                    {
                        if (Line.Contains("}"))
                        {
                            int bCount = Line.Count(f => f == '}');
                            BracketCount -= bCount;
                        }

                        if (BracketCount == 0)
                        {
                            FunctionCapture = false;
                            BracketStart = false;
                            WriteLine = NewFunctionText;
                        }
                    }
                }


                if (!FunctionCapture)
                {
                    if (AllText != "") { AllText = AllText + Environment.NewLine + WriteLine; }
                    if (AllText == "") { AllText = WriteLine; }
                }


            }


            return AllText;
        }

        /// <summary> function to remove specific function from a project file</summary>
        /// <param name="ProjectFile"> </param>
        /// <param name=" RemoveFunctionNAME"> </param>
        /// <param name=" out ErrorMsg"> </param>
        public bool Remove_Function_FromFile(string ProjectFile, string RemoveFunctionNAME, out string ErrorMsg)
        {
            // Read Project File Text - Return Error If Not Found
            ErrorMsg = ProjectFile + " Not Found.";
            if (!File.Exists(ProjectFile)) { if (GlobalDebug) { ahk.MsgBox(ErrorMsg); } return false; }
            string CodeString = ahk.FileRead(ProjectFile);

            bool FoundFunction = Function_InCode(CodeString, RemoveFunctionNAME);
            if (!FoundFunction)
            {
                ErrorMsg = RemoveFunctionNAME + " Not Found In " + ahk.FileName(ProjectFile);
                if (GlobalDebug) { ahk.MsgBox(ErrorMsg); }
                return false;
            }

            string NewProjectCode = Remove_Function_FromCode(CodeString, RemoveFunctionNAME);


            // Write New ProjectFile with Updated Code, Backing up the Project File First
            bool WroteNewFile = Write_ProjectFile_With_Backup(ProjectFile, NewProjectCode, out ErrorMsg);
            if (!WroteNewFile) { return false; }

            ErrorMsg = "Removed Function: " + RemoveFunctionNAME + " From: " + ahk.FileName(ProjectFile);
            return true;
        }

        /// <summary> remove function from file, returns project code without that function</summary>
        /// <param name="CodeString"> </param>
        /// <param name=" RemoveFunctionNAME"> </param>
        public string Remove_Function_FromCode(string CodeString, string RemoveFunctionNAME)
        {
            List<string> LineListF = lst.Text_To_List(CodeString, false, false, false);

            //back.Backup_File(ProjectFile);  // backup design file before changing
            //ahk.FileDelete(ProjectFile);    // remove old code file


            int BracketCount = 0;
            bool BracketStart = false;
            bool FunctionCapture = true;
            string FunctionCapText = "";
            string ThisFunctionName = "";
            //int LineNum = 1;

            string AllText = "";

            foreach (string Line in LineListF)
            {
                //string LineCheck = Line;
                //if (LineCheck.Trim() == "") { LineNum++;  continue; }

                string WriteLine = Line;

                bool FunctionLine = IsFunction(Line);

                if (FunctionLine)
                {
                    ThisFunctionName = FunctionName(Line);

                    if (ThisFunctionName == RemoveFunctionNAME) { FunctionCapture = false; }
                }

                if (!FunctionCapture)
                {
                    if (FunctionCapText != "") { FunctionCapText = FunctionCapText + Environment.NewLine + Line; }
                    if (FunctionCapText == "") { FunctionCapText = Line; }

                    if (Line.Contains("{") && (!Line.Contains("\"{\"")))
                    {
                        int bCount = Line.Count(f => f == '{');  // count number of brackets to account for in this line
                        BracketStart = true;
                        BracketCount += bCount;
                    }

                    if (BracketStart)
                    {
                        if (Line.Contains("}") && (!Line.Contains("\"}\"")))
                        {
                            int bCount = Line.Count(f => f == '}');
                            BracketCount -= bCount;
                        }

                        if (BracketCount == 0)  // end of function (last bracket)
                        {
                            FunctionCapture = true;
                            BracketStart = false;
                            //WriteLine = NewFunctionText;
                            continue;
                        }
                    }
                }


                if (FunctionCapture)
                {
                    if (AllText != "") { AllText = AllText + Environment.NewLine + WriteLine; }
                    if (AllText == "") { AllText = WriteLine; }
                }


            }

            //ahk.FileAppend(AllText, ProjectFile, false, true);

            return AllText;
        }

        // loops through c# file, adds structure for documentation to each function if missing
        public string Add_Documentation_FromFile(string ProjectFile, bool Backup = true)
        {
            // Read Project File Text - Return Error If Not Found
            string ErrorMsg = ProjectFile + " Not Found.";
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ErrorMsg); return ""; }
            string CodeString = ahk.FileRead(ProjectFile);

            CodeString = CodeString.Replace("%", "`%");

            // add documentation to each function in string
            string writeCode = Add_Documentation_FromCode(CodeString);

            if (Backup) // option to create back of original code file before overwriting 
            {
                ahk.Backup_File(ProjectFile, true);
                ahk.FileAppend(writeCode, ProjectFile, 1);
            }

            return writeCode;
        }

        // loops through c# file, adds structure for documentation to each function if missing
        public string Add_Documentation_FromCode(string CodeString)
        {
            // add documentation to each function in string
            return Update_Function_FromCode(CodeString, "", "", true);
        }

        /// <summary> update function in cs file (includes documentation)</summary>
        /// <param name="CodeString"> </param>
        /// <param name=" UpdateFunctionNAME"> </param>
        public string Update_Function_FromCode(string CodeString, string UpdateFunctionNAME, string UpdateFunctionCODE, bool WriteMissingDocumentation = true)
        {
            List<string> LineListF = lst.Text_To_List(CodeString, false, false, false);

            //back.Backup_File(ProjectFile);  // backup design file before changing
            //ahk.FileDelete(ProjectFile);    // remove old code file


            int BracketCount = 0;
            bool BracketStart = false;
            bool FunctionCapture = true;
            string FunctionCapText = "";
            string ThisFunctionName = "";
            //int LineNum = 1;
            string docReturn = "";

            string AllText = "";

            foreach (string Line in LineListF)
            {
                //string LineCheck = Line;
                //if (LineCheck.Trim() == "") { LineNum++;  continue; }

                string WriteLine = Line;

                bool FunctionLine = IsFunction(Line);
                bool DocLine = DocumentationLine(Line);

                if (FunctionLine)
                {
                    ThisFunctionName = FunctionName(Line);

                    if (ThisFunctionName == UpdateFunctionNAME) { FunctionCapture = false; }

                    //if (FunctionCapture)
                    //{
                    //    docReturn = ""; // reset documentation capture 
                    //}
                }

                if (DocLine)
                {
                    if (docReturn != "") { docReturn = docReturn + "\n" + Line; }
                    if (docReturn == "") { docReturn = Line; }
                }

                if (!FunctionCapture)
                {
                    if (FunctionCapText != "") { FunctionCapText = FunctionCapText + Environment.NewLine + Line; }
                    if (FunctionCapText == "") { FunctionCapText = Line; }

                    if (Line.Contains("{") && (!Line.Contains("\"{\"")))
                    {
                        int bCount = Line.Count(f => f == '{');  // count number of brackets to account for in this line
                        BracketStart = true;
                        BracketCount += bCount;
                    }

                    if (BracketStart)
                    {
                        if (Line.Contains("}") && (!Line.Contains("\"}\"")))
                        {
                            int bCount = Line.Count(f => f == '}');
                            BracketCount -= bCount;
                        }

                        if (BracketCount == 0)  // end of function (last bracket)
                        {
                            FunctionCapture = true;
                            BracketStart = false;
                            //WriteLine = NewFunctionText;

                            //FunctionCapText = FunctionCapText + Environment.NewLine + UpdateFunctionCODE;

                            // add new function code into cs file after removing old
                            AllText = AllText + Environment.NewLine + UpdateFunctionCODE;
                            //AllText = AllText + UpdateFunctionCODE;

                            docReturn = "";

                            continue;
                        }
                    }
                }


                if (FunctionCapture)
                {
                    //if (DocLine)
                    //{
                    //    if (AllText != "") { AllText = AllText + Environment.NewLine + WriteLine; }
                    //    if (AllText == "") { AllText = WriteLine; }
                    //}

                    if (!DocLine && !FunctionLine)
                    {
                        if (AllText != "") { AllText = AllText + Environment.NewLine + WriteLine; }
                        if (AllText == "") { AllText = WriteLine; }
                    }

                    // write previously captured documentation lines above current function
                    if (FunctionLine)
                    {
                        if (docReturn == "")  // no documentation found for this function
                        {
                            if (!WriteMissingDocumentation) { AllText = AllText + Environment.NewLine + WriteLine; }
                            if (WriteMissingDocumentation)
                            {
                                string doc = ParseCSFunctionLine_Return_Documentation(WriteLine);

                                // match leading spaces with function line spacing
                                int spaces = ahk.LeadingSpaceCount(Line);
                                List<string> doclines = lst.Text_To_List(doc, true, true, false);
                                foreach (string d in doclines)
                                {
                                    string newDocLine = ahk.AddLeadingSpaces(d, spaces);

                                    AllText = AllText + Environment.NewLine + newDocLine;
                                }

                                WriteLine = RemoveCommentsFromLine(WriteLine);
                                WriteLine = ahk.AddLeadingSpaces(WriteLine, spaces);

                                AllText = AllText + Environment.NewLine + WriteLine;
                            }

                        }
                        if (docReturn != "") { AllText = AllText + Environment.NewLine + docReturn + Environment.NewLine + WriteLine; }

                        docReturn = ""; // reset documentation capture 
                    }
                }


            }

            //ahk.FileAppend(AllText, ProjectFile, false, true);

            return AllText;
        }


        // untested 

        /// <summary> extracts all functions from code file, returns code minus the functions</summary>
        /// <param name="ProjectFile"> </param>
        public string Return_Code_Without_Functions_FromFile(string ProjectFile)
        {
            // Read Project File Text - Return Error If Not Found
            string ErrorMsg = ProjectFile + " Not Found.";
            if (!File.Exists(ProjectFile)) { if (GlobalDebug) { ahk.MsgBox(ErrorMsg); } return ""; }
            string CodeString = ahk.FileRead(ProjectFile);

            string CodeNoFunctions = Return_Code_Without_Functions_FromCode(CodeString);

            ErrorMsg = "Removed Functions From : " + ahk.FileName(ProjectFile);
            return CodeNoFunctions;
        }

        /// <summary> extracts all functions from code, returns code minus the functions</summary>
        /// <param name="CodeString"> </param>
        public string Return_Code_Without_Functions_FromCode(string CodeString)
        {
            string CodeNoFunctions = CodeString;

            List<string> functions = FunctionList_FromCode(CodeString);
            foreach (string function in functions)
            {
                CodeNoFunctions = Remove_Function_FromCode(CodeNoFunctions, function);
            }

            return CodeNoFunctions;
        }


        /// <summary> insert new function into existing region(backs up original file then adds new function in region in .cs file)</summary>
        /// <param name="ProjectFile"> </param>
        /// <param name=" RegionName"> </param>
        /// <param name=" NewFunctionCode"> </param>
        /// <param name=" out ErrorMsg"> </param>
        public bool Insert_New_Function_Into_Region_ToFile(string ProjectFile, string RegionName, string NewFunctionCode, out string ErrorMsg)
        {
            ErrorMsg = ahk.FileName(ProjectFile) + " Not Found - Unable To Insert";
            if (!File.Exists(ProjectFile)) { if (GlobalDebug) { ahk.MsgBox(ErrorMsg); } return false; }
            string CodeString = ahk.FileRead(ProjectFile);


            // reformat the line lines with tab spaces in front 

            string NewFunctionFormatted = "";
            List<string> NewFunctionLines = lst.Text_To_List(NewFunctionCode, false, false, false);
            foreach (string nLine in NewFunctionLines)
            {
                string wLine = "";
                bool Formated = false;

                if (IsFunction(nLine)) { wLine = "\t\t" + nLine.Trim(); Formated = true; }
                if (nLine.Trim() == "{") { wLine = "\t\t" + nLine.Trim(); Formated = true; }
                if (nLine.Trim() == "}") { wLine = "\t\t" + nLine.Trim(); Formated = true; }

                if (!Formated) { wLine = "\t\t\t" + nLine.Trim(); Formated = true; }

                if (NewFunctionFormatted != "") { NewFunctionFormatted = NewFunctionFormatted + Environment.NewLine + wLine; }
                if (NewFunctionFormatted == "") { NewFunctionFormatted = wLine; }
            }


            string WriteCode = Insert_New_Function_Into_Region_ToCode(CodeString, RegionName, NewFunctionFormatted, out ErrorMsg);
            if (WriteCode == "") { return false; }


            // Write New ProjectFile with Updated Code, Backing up the Project File First
            bool WroteNewFile = Write_ProjectFile_With_Backup(ProjectFile, WriteCode, out ErrorMsg);
            if (!WroteNewFile) { return false; }

            ErrorMsg = "Inserted New Function In Region: " + RegionName + " Into " + ahk.FileName(ProjectFile);
            return true;
        }

        /// <summary> insert new function into existing region - returns updated .cs code</summary>
        /// <param name="CodeString"> </param>
        /// <param name=" RegionName"> </param>
        /// <param name=" NewFunctionCode"> </param>
        /// <param name=" out ErrorMsg"> </param>
        public string Insert_New_Function_Into_Region_ToCode(string CodeString, string RegionName, string NewFunctionCode, out string ErrorMsg)
        {
            string LastFunctionName = Last_FunctionName_In_Region_FromCode(CodeString, RegionName, out ErrorMsg);  // return the last functionname found in the region
            if (LastFunctionName == "")  // no functions in the region yet 
            {
                // todo: need to be able to insert new function in empty region 
                return "";
            }

            string WriteCode = Insert_Function_After_Function_ToCode(CodeString, LastFunctionName, NewFunctionCode);  // write new code after last function in file

            ErrorMsg = "No Errors.";
            return WriteCode;
        }


        /// <summary> insert new 'using' lines under last using line, writes to file and backs up original project</summary>
        /// <param name="ProjectFile"> </param>
        /// <param name="List<string> NewUsingLines"> </param>
        public bool Insert_Using_Lines_ToFile(string ProjectFile, List<string> NewUsingLines)
        {
            List<string> ExistingUsingLines = Using_List_FromFile(ProjectFile);
            string LastUsingLine = "";
            foreach (string usingline in ExistingUsingLines) { LastUsingLine = usingline; } // store the last using statement

            string InsertUsingLines = "";
            int NewCount = 0;
            foreach (string custom in NewUsingLines)
            {
                bool AlreadyUsed = lst.InList(ExistingUsingLines, custom);
                if (!AlreadyUsed)
                {
                    NewCount++;
                    if (InsertUsingLines != "") { InsertUsingLines = InsertUsingLines + "\n" + custom; }
                    if (InsertUsingLines == "") { InsertUsingLines = custom; }
                }
            }

            bool Inserted = false;
            if (NewCount > 0)
            {
                return true; // nothing to insert
            }

            string ErrorMsg = "";
            Inserted = Insert_After_Line_ToFile(ProjectFile, LastUsingLine, InsertUsingLines, out ErrorMsg);
            return Inserted;
        }

        /// <summary> Write Project File Code To File, Backing Up Original File First, Overwriting Existing Project File</summary>
        /// <param name="ProjectFile"> </param>
        /// <param name=" CodeToWrite"> </param>
        /// <param name=" out ErrorMsg"> </param>
        public bool Write_ProjectFile_With_Backup(string ProjectFile, string CodeToWrite, out string ErrorMsg)
        {
            bool BackedUp = ahk.Backup_File(ProjectFile);  // backup design file before changing

            if (BackedUp)
            {
                bool Deleted = ahk.FileDelete(ProjectFile);    // remove old code file
                if (!Deleted) { ErrorMsg = "Error Deleting " + ProjectFile + " - Unable To Write New Project File"; return false; }

                bool FileAppended = ahk.FileAppend(CodeToWrite, ProjectFile); // write new code to file
                if (!FileAppended) { ErrorMsg = "Error Writing New Project File (" + ahk.FileName(ProjectFile) + ") - May Need To Restore Backup"; return false; }
            }

            if (!BackedUp)
            {
                ErrorMsg = "Error Backing Up " + ahk.FileName(ProjectFile) + " - Did Not Write New File." + Environment.NewLine + "No Changes Made";
                if (GlobalDebug) { ahk.MsgBox(ErrorMsg); }
                return false;
            }

            ErrorMsg = "";
            return true;
        }


        // parse cs funcion line, return documentation template
        public string ParseCSFunctionLine_Return_Documentation(string Line)
        {
            ahk.SetVar("Line", Line);  // assign input variable value
            ahk.SetVar("LineComments", "");  // assign input variable value
            ahk.SetVar("out", "");  // assign input variable value

            string codeMem = @"
; take function declaration line, parse parameters, and return C# documentation lines

StringSplit, WordArray, Line, (
StringSplit, WordArrayCom, Line, /

LineEnd = %WordArray2%
Comments = %WordArrayCom3%

StringSplit, WordArray, LineEnd, )
LineEnd = %WordArray1%

StringReplace, LineEnd, LineEnd, string%A_Space%,,ALL
StringReplace, LineEnd, LineEnd, int%A_Space%,,ALL
StringReplace, LineEnd, LineEnd, bool%A_Space%,,ALL

AutoTrim, On

StringReplace, LineComments, WordArray2,//,|,ALL

StringSplit, CArray, LineEnd, `,
count = 0
Loop, %CArray0%
{
    this_param := CArray%a_index%
	
	ifInString, this_param, =
		{
		StringSplit, WordArray, this_param, =
		this_param = %WordArray1%
		}
    
	if count = 0
		Out = /// <param name=""%this_param%""> </param>


    if count > 0
        Out = %Out%`n/// <param name=""%this_param%""> </param>		

      count++
  
      ; MsgBox, %a_index% is %this_param%
}

;Msgbox %LineComments%
IfInString, LineComments, |
	{
		StringSplit, CoArray, LineComments, |,
		
        writeComment = %CoArray2%
		comments = <summary>%writeComment%</summary>

        comments = /// %comments%

        if out != 
		    out = %comments%`n%out%	
        if out = 
            out = %comments%
	}

    IfNotInString, LineComments, |
	    out = /// <summary> </summary>`n%out%	

    out = StringReplace, out, out, <param name=%A_Space%, <param name=

    ;msgbox %out%

";

            // store function in memory before executing
            string returnVal = ahk.Execute(codeMem, "out");

            //if (!ahkLoaded)
            //{
            //    string tmpFile = ahk.AppDir() + "\\tmp.txt";
            //    ahk.FileDelete(tmpFile);
            //    ahk.FileAppend(code, tmpFile, 1);
            //    ahk.Load_ahkFile(tmpFile);
            //    ahkLoaded = true;
            //}

            //string returnVal = ahk.Function("ParseCSFunctionLine_Return_Documentation", Line);

            return returnVal;
        }
        bool ahkLoaded = false;

        #region === Insert Into Specific Sections ===

        /// <summary> adds a line to existing Control_Setup() function (or any function)</summary>
        /// <param name="ProjectFile"> </param>
        /// <param name=" NewLine"> </param>
        /// <param name=" out ErrorMsg"> </param>
        /// <param name="FunctionNAME"> </param>
        public bool Add_Control_Setup_Line(string ProjectFile, string NewLine, out string ErrorMsg, string FunctionNAME = "Control_Setup")
        {
            //bool Inserted = Add_Function_Line(ProjectFile, NewLine, "Control_Setup");
            bool Inserted = Insert_Line_In_Function_ToFile(ProjectFile, FunctionNAME, NewLine, out ErrorMsg);
            return Inserted;
        }

        /// <summary> adds a line to existing Define_EventHandlers() function (or any function)</summary>
        /// <param name="ProjectFile"> </param>
        /// <param name=" NewLine"> </param>
        /// <param name=" out ErrorMsg"> </param>
        /// <param name="FunctionNAME"> </param>
        public bool Add_Define_EventHandlers_Line(string ProjectFile, string NewLine, out string ErrorMsg, string FunctionNAME = "Define_EventHandlers")
        {
            bool Inserted = Insert_Line_In_Function_ToFile(ProjectFile, FunctionNAME, NewLine, out ErrorMsg);
            return Inserted;
        }


        #endregion

        #endregion


        #endregion

        #region === ReFormat Project Code ===


        // remove extra blank lines from project (default = more than 2 consecutive lines) 
        public bool Remove_Extra_Blank_Lines_FromFile(string ProjectFile, out string ErrorMsg, int BlankLinesAllowed = 2)  // remove more than BlankLinesAllowed Count of blanks (trims down empty space in project file) - backs up then overwrites existing .cs file
        {
            // read project file to code string
            ErrorMsg = ahk.FileName(ProjectFile) + " Not Found - Unable To Reformat";
            if (!File.Exists(ProjectFile)) { if (GlobalDebug) { ahk.MsgBox(ErrorMsg); } return false; }
            string CodeString = ahk.FileRead(ProjectFile);

            string NewCode = Remove_Extra_Blank_Lines_FromCode(CodeString, BlankLinesAllowed);


            // Write New ProjectFile with Updated Code, Backing up the Project File First
            bool WroteNewFile = Write_ProjectFile_With_Backup(ProjectFile, NewCode, out ErrorMsg);
            if (!WroteNewFile) { return false; }

            // return success
            ErrorMsg = "Removed Extra Blank Lines From " + ahk.FileName(ProjectFile);
            return true;
        }
        public string Remove_Extra_Blank_Lines_FromCode(string CodeString, int BlankLinesAllowed = 2)  // remove more than BlankLinesAllowed Count of blanks (trims down empty space in project file)
        {
            List<string> LineList = lst.Text_To_List(CodeString, false, false, false);

            string AllText = "";

            int BlankLineCounter = 0;
            foreach (string Line in LineList)
            {
                string WriteLine = Line;

                if (Line.Trim() == "") { BlankLineCounter++; }

                if (Line.Trim() == "" && BlankLineCounter > BlankLinesAllowed) { continue; }

                if (Line.Trim() != "") { BlankLineCounter = 0; }

                if (AllText != "") { AllText = AllText + Environment.NewLine + WriteLine; }
                if (AllText == "") { AllText = WriteLine; }
            }

            return AllText;
        }  // insert code underneath the form declaration (startup code region) to Code String


        // move menu item functions into the === Menu === region

        public bool Move_Menu_Items_To_MenuRegion(string ProjectFile)  // if there are any menu item functions outside of the === Menu === region - move into region
        {
            string CodeString = ahk.FileRead(ProjectFile);
            List<string> MenuFunctions = ProjectFile_ControlList(ProjectFile, "ToolStripMenuItem", false);  // parse project Designer.cs file - returns list of controls (either all or by control type) - ReturnControlTypes option to just return list of types of control in project

            string MenuRegion = "=== Menu ===";

            //foreach (string menu in MenuFunctions)
            //{
            //   List<string> menuHandlers = Designer_ControlSetup_List(ProjectFile, menu, true);  // extract Control setup info from Designer.cs file based on control name

            //   foreach (string handle in menuHandlers)
            //   {
            //       string MenuFunction = DesignerFile_HandlerAction(handle);

            //       ahk.MsgBox("Menu: " + menu + "\nFunction: " + MenuFunction);
            //   }

            //}

            List<string> menuNotInRegions = Menu_Functions_NotInRegions_FromCode(CodeString);

            if (menuNotInRegions.Count > 0)  // if there are any menu items outside of regions (to move list)
            {
                string ErrorMsg = "";

                // does menu region exist already? 
                bool MenuRegionExists = Region_In_Code(CodeString, MenuRegion);  // is there already a menu function?
                ahk.MsgBox("Menu Region Found = " + MenuRegionExists.ToString() + "\nMenu Functions To Move: " + menuNotInRegions.Count.ToString());

                // create Menu Region First loop if needed
                if (!MenuRegionExists) { ahk.MsgBox("Need To Run New Project StartUp First - Menu Region MISSING"); return false; }

                // move each function into the === Menu === Region
                foreach (string freeMenu in menuNotInRegions)
                {
                    bool Moved = Move_Function_Into_Region_ToFile(ProjectFile, MenuRegion, freeMenu, out ErrorMsg);
                    //ahk.MsgBox("Not In Region: " + freeMenu + "\nMoved to Region = " + Moved);
                }

                Remove_Extra_Blank_Lines_FromFile(ProjectFile, out ErrorMsg, 2);
            }



            //public string DesignerFile_HandlerAction(string EventLine)


            return true;
        }



        #endregion

        #region === Parse Code ===

        #region === Form Name ===

        public bool IsFormNameLine(string Line)  // checks to see if line passed in is the beginning of a new winform (public partial class ___)
        {
            //public partial class NPAD : Form
            //string FirstWord = ahk.WordNum(Line, 1);  // return specific word # from string
            //bool MaybeClass = false;
            //if (FirstWord == "public" || FirstWord == "private") { MaybeClass = true; }

            bool FormLine = ahk.IfInString("public partial class", Line);

            //if (FormLine) { ahk.MsgBox(Line); }

            return FormLine;
        }

        public string Return_FormName_Line_FromFile(string ProjectFile)  // returns the first line of a project declairing the a new WinForm from ProjectFile
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found."); return ""; }

            string CodeString = ahk.FileRead(ProjectFile);

            string FormLine = Return_FormName_Line_FromCode(CodeString);

            return FormLine;
        }
        public string Return_FormName_Line_FromCode(string CodeString)  // returns the first line of a project declairing the a new WinForm from Code String
        {
            List<string> LineList = ahk.LineList(CodeString, false);

            foreach (string Line in LineList)
            {
                bool FormLine = IsFormNameLine(Line);
                if (FormLine) { return Line; }
            }

            return "";
        }



        public string Return_FormCode_FromFile(string ProjectFile)  // returns all the code inside the main class in startup file
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found."); return ""; }
            string CodeString = ahk.FileRead(ProjectFile);
            //string FormLine = Return_FormName_Line_FromCode(CodeString);
            string FormCode = Return_FormCode_FromCode(CodeString);


            return FormCode;
        }
        public string Return_FormCode_FromCode(string CodeString)  // returns all the code inside the main class in startup code
        {
            List<string> LineList = lst.Text_To_List(CodeString, false, false, false);
            string AllCode = "";

            bool CApture = false;
            int CapCount = 0;
            bool BracketStart = false;
            int BracketCount = 0;
            foreach (string Line in LineList)
            {
                bool FormLine = IsFormNameLine(Line);
                if (FormLine) { CApture = true; }

                if (CApture)
                {
                    CapCount++;

                    if (Line.Contains("{"))
                    {
                        int bCount = Line.Count(f => f == '{');  // count number of brackets to account for in this line
                        BracketStart = true;
                        BracketCount += bCount;
                        //ahk.MsgBox("Bracket Start: " + Line); 
                    }

                    if (CapCount < 3) { continue; } // skip name line and first bracket

                    if (BracketStart)
                    {
                        if (Line.Contains("}"))
                        {
                            int bCount = Line.Count(f => f == '}');
                            BracketCount -= bCount;
                        }

                        if (BracketCount == 0)
                        {
                            CApture = false;
                            //ahk.MsgBox(ThisFunctionName + " : " + FunctionCapText);
                            //FunctionCapText = "";
                            //ThisFunctionName = "";
                            BracketStart = false;

                            return AllCode;
                        }
                    }


                    if (AllCode != "") { AllCode = AllCode + Environment.NewLine + Line; }
                    if (AllCode == "") { AllCode = Line; }
                }

            }

            return AllCode;
        }


        // insert new region code into bottom of current project
        public string New_Region_OnBottom_ToFile(string ProjectFile, string NewRegionCode, out string ErrorMsg)  // insert new region code into buttom of main class
        {
            // Read Project File Text - Return Error If Not Found
            ErrorMsg = ProjectFile + " Not Found.";
            if (!File.Exists(ProjectFile)) { if (GlobalDebug) { ahk.MsgBox(ErrorMsg); } return ""; }
            string CodeString = ahk.FileRead(ProjectFile);

            string NewCode = New_Region_OnBottom_ToCode(CodeString, NewRegionCode, out ErrorMsg);

            return NewCode;
        }
        public string New_Region_OnBottom_ToCode(string CodeString, string NewRegionCode, out string ErrorMsg)  // insert new region code into buttom of main class
        {
            string FormCode = Return_FormCode_FromCode(CodeString);
            List<string> LineList = lst.Text_To_List(CodeString, false, false, false);
            ErrorMsg = "None";

            string TopCode = "";
            foreach (string Line in LineList)
            {
                if (!IsFormNameLine(Line))
                {
                    if (TopCode != "") { TopCode = TopCode + Environment.NewLine + Line; }
                    if (TopCode == "") { TopCode = Line; }
                }

                if (IsFormNameLine(Line))
                {
                    TopCode = TopCode + Environment.NewLine + Line + Environment.NewLine + "{" + Environment.NewLine + FormCode + Environment.NewLine + NewRegionCode + Environment.NewLine + Environment.NewLine + Environment.NewLine + "}" + Environment.NewLine + "}" + Environment.NewLine;
                    return TopCode;
                }

            }
            return "";

        }


        #endregion

        #region === Parse: Designer CS ===

        public string DesignerPath(string CSFilePath) //return the path to the visual studio control designer file
        {
            if (CSFilePath.Contains(".Designer.cs"))
            {
                return CSFilePath; //file is the desinger file path
            }

            string FileDir = ahk.FileDir(CSFilePath);

            string FileName = ahk.FileNameNoExt(CSFilePath).Replace(".", "");
            FileName = FileDir + "\\" + FileName + ".Designer.cs";
            return FileName;
        }

        // Designer Control Config - export existing startup settings / options / event handlers for each control
        public List<string> Designer_ControlSetup_List(string DesignerFilePath, string controlName, bool EventHandlersOnly = false)  // extract Control setup info from Designer.cs file based on control name
        {
            DesignerFilePath = DesignerPath(DesignerFilePath); // if project file is passed in, convert to designer file path

            List<string> Lines = lst.TextFile_To_List(DesignerFilePath);

            List<string> controlInfo = new List<string>();
            string controlInfoString = "";

            if (Lines != null)
            {
                foreach (string Line in Lines)
                {
                    if (Line.Contains("this." + controlName + "."))
                    {
                        if (EventHandlersOnly)
                        {
                            if (Line.Contains("+=") && Line.Contains("EventHandler"))  // extrac the event handler lines for this control
                            {
                                controlInfoString = controlInfoString + Environment.NewLine + Line;
                                controlInfo.Add(Line);
                            }

                            continue;
                        }

                        controlInfo.Add(Line);
                        controlInfoString = controlInfoString + Environment.NewLine + Line;
                    }
                }
            }


            return controlInfo;
        }
        public string Extract_Control_Setup(string DesignerFilePath, string controlName, bool EventHandlersOnly = false)  // extract Control setup info from Designer.cs file based on control name
        {
            DesignerFilePath = DesignerPath(DesignerFilePath); // if project file is passed in, convert to designer file path

            List<string> Lines = lst.TextFile_To_List(DesignerFilePath);

            List<string> controlInfo = new List<string>();
            string controlInfoString = "";

            foreach (string Line in Lines)
            {
                if (Line.Contains("this." + controlName + "."))
                {
                    if (EventHandlersOnly)
                    {
                        if (Line.Contains("+=") && Line.Contains("EventHandler"))  // extrac the event handler lines for this control
                        {
                            controlInfoString = controlInfoString + Environment.NewLine + Line;
                            controlInfo.Add(Line);
                        }

                        continue;
                    }

                    controlInfo.Add(Line);
                    controlInfoString = controlInfoString + Environment.NewLine + Line;
                }
            }

            return controlInfoString;
        }

        // list of functions referenced in designer as function associated with controls (ex: menu and button click events)
        public List<string> Designer_Control_Event_Function_List(string ProjectFile, string controlType = "ToolStripMenuItem")  // list of functions referenced in designer as function associated with controls (ex: menu and button click events)
        {
            //List<string> functionList = FunctionList_FromFile(ProjectFile);
            List<string> MenuEventFunctions = new List<string>();
            List<string> MenuFunctions = ProjectFile_ControlList(ProjectFile, controlType, false);  // parse project Designer.cs file - returns list of controls (either all or by control type) - ReturnControlTypes option to just return list of types of control in project

            foreach (string menu in MenuFunctions)
            {
                List<string> menuHandlers = Designer_ControlSetup_List(ProjectFile, menu, true);  // extract Control setup info from Designer.cs file based on control name

                foreach (string handle in menuHandlers)
                {
                    string MenuFunction = DesignerFile_HandlerAction(handle);
                    //ahk.MsgBox("Menu: " + menu + "\nFunction: " + MenuFunction);
                    MenuEventFunctions.Add(MenuFunction);
                }
            }


            return MenuEventFunctions;
        }


        // List of Controls - for each ControlType or All Controls, Option to Return List of Distinct Control Types
        public List<string> ProjectFile_ControlList(string ProjectFile, string ControlType = "ALL", bool ReturnControlTypes = false)  // parse project Designer.cs file - returns list of controls (either all or by control type) - ReturnControlTypes option to just return list of types of control in project
        {
            string DesignerFilePath = DesignerPath(ProjectFile);

            if (!File.Exists(DesignerFilePath)) { return null; }

            List<string> Lines = lst.TextFile_To_List(DesignerFilePath);

            List<string> controls = new List<string>();
            List<string> controlTypeList = new List<string>();

            foreach (string Line in Lines)
            {
                string FirstWord = ahk.FirstWord(Line);

                if (FirstWord == "private")
                {
                    if (Line.Contains("=")) { continue; }
                    if (Line.Contains("(")) { continue; }

                    bool Added = false;

                    string controlName = ahk.WordNum(Line, 3);  //third word in line is the name of the control
                    controlName = ahk.StringReplace(controlName, ";");
                    controlName = controlName.Trim();

                    string controlType = ahk.WordNum(Line, 2);

                    // create distinct list of control types to return (option)
                    if (!lst.InList(controlTypeList, controlType)) { controlTypeList.Add(controlType); }


                    if (controlType == "System.Windows.Forms.DataGridView") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "DATAGRIDVIEW") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.Button") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "BUTTON") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.TreeView") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "TREEVIEW") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.TextBox") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "TEXTBOX") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.TabControl") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "TABCONTROL") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.TabPage") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "TABPAGE") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.ListBox") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "LISTBOX") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.MenuStrip") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "MENUSTRIP") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.ToolStripMenuItem") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "TOOLSTRIPMENUITEM") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.TableLayoutPanel") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "TABLELAYOUTPANEL") { controls.Add(controlName); } }
                    if (controlType == "TreeViewFast.Controls.TreeViewFast") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "TREEVIEWFAST") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.CheckBox") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "CHECKBOX") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.Label") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "LABEL") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.PictureBox") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "PICTUREBOX") { controls.Add(controlName); } }
                    if (controlType == "ScintillaNET.Scintilla") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "SCINTILLA") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.Splitter") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "SPLITTER") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.Panel") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "PANEL") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.ToolStripSeparator") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "TOOLSTRIPSEPERATOR") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.SaveFileDialog") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "SAVEFILEDIALOG") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.BindingNavigator") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "BINDINGNAVIGATOR") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.OpenFileDialog") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "OPENFILEDIALOG") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.WebBrowser") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "WEBBROWSER") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.ToolStripComboBox") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "TOOLSTRIPCOMBOBOX") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.ToolStripButton") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "TOOLSTRIPBUTTON") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.ToolStripLabel") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "TOOLSTRIPLABEL") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.ToolStripTextBox") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "TOOLSTRIPTEXTBOX") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.RichTextBox") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "RICHTEXTBOX") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.ComboBox") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "COMBOBOX") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.ContextMenuStrip") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "CONTEXTMENUSTRIP") { controls.Add(controlName); } }
                    if (controlType == "System.IO.FileSystemWatcher") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "FILESYSTEMWATCHER") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.CheckedListBox") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "CHECKEDLISTBOX") { controls.Add(controlName); } }
                    if (controlType == "System.Windows.Forms.RadioButton") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "RADIOBUTTON") { controls.Add(controlName); } }

                    if (controlType == "Telerik.WinControls.UI.RadLabel") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "RADLABEL") { controls.Add(controlName); } }
                    if (controlType == "Telerik.WinControls.UI.RadTextBox") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "RADTEXTBOX") { controls.Add(controlName); } }
                    if (controlType == "Telerik.WinControls.UI.RadButton") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "RADBUTTON") { controls.Add(controlName); } }
                    if (controlType == "Telerik.WinControls.UI.RadStatusStrip") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "RADSTATUSSTRIP") { controls.Add(controlName); } }
                    if (controlType == "Telerik.WinControls.UI.RadLabelElement") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "RADLABELELEMENT") { controls.Add(controlName); } }
                    if (controlType == "Telerik.WinControls.UI.RadMenu") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "RADMENU") { controls.Add(controlName); } }
                    if (controlType == "Telerik.WinControls.UI.RadMenuItem") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "RADMENUITEM") { controls.Add(controlName); } }
                    if (controlType == "Telerik.WinControls.UI.RadPageView") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "RADPAGEVIEW") { controls.Add(controlName); } }
                    if (controlType == "Telerik.WinControls.UI.RadPageViewPage") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "RADPAGEVIEWPAGE") { controls.Add(controlName); } }
                    //if (controlType == "") { Added = true; if (ControlType.ToUpper() == "ALL" || ControlType.ToUpper() == "RADIOBUTTON") { controls.Add(controlName); } }


                    if (GlobalDebug) { if (!Added) { ahk.MsgBox("Control Type Not Setup Yet: " + controlType); } }

                }
            }

            if (ReturnControlTypes) { return controlTypeList; }  // return just the distinct list of control types in project

            return controls;
        }

        // Designer Project Event Handler References -- Add / Remove / Define
        public bool Has_Handler(string ProjectFile, string ControlName, string ControlEvent)  // check to see if a control already has an event assigned for a click event (ex: MouseClick)
        {
            ProjectFile = DesignerPath(ProjectFile);

            List<string> Lines = Designer_ControlSetup_List(ProjectFile, ControlName);
            string SearchLine = "this." + ControlName + "." + ControlEvent;
            foreach (string line in Lines)
            {
                if (line.Contains(SearchLine)) return true;
            }
            return false;

            //this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            //this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            //this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            //this.dataGridView1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyUp);
            //this.dataGridView1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseClick);


        }

        // write the same function (ProjectControls_MouseClick()) to every control in project - for global right click options
        public void DesignerFile_MouseClick_AllFunctions(string ProjectFile, bool OverWriteExisting = true)  // write the same function (ProjectControls_MouseClick()) to every control in project - for global right click options
        {
            List<string> controlList = ProjectFile_ControlList(ProjectFile, "ALL", false);  // parse project Designer.cs file - returns list of controls (either all or by control type) - ReturnControlTypes option to just return list of types of control in project
            foreach (string control in controlList)
            {
                DesignerFile_Insert_Handler(ProjectFile, "MouseClick", control, "MouseClick", OverWriteExisting); // insert new control handler line in Designer file
            }

        }

        public bool DesignerFile_Insert_Handler(string ProjectFile, string ControlType, string ControlName, string ControlEvent, bool OverWriteOld = false) // insert new control handler line in Designer file
        {
            ProjectFile = DesignerPath(ProjectFile);

            string InsertLine = "";

            if (ControlType.ToUpper() == "MOUSECLICK")
            {
                if (ControlEvent == "MouseClick") { InsertLine = "this." + ControlName + ".MouseClick += new System.Windows.Forms.MouseEventHandler(this.ProjectControls_MouseClick);"; }
            }

            if (ControlType.ToUpper() == "DATAGRIDVIEW")
            {
                if (ControlEvent == "CellClick") { InsertLine = "this." + ControlName + ".CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this." + ControlName + "_CellClick);"; }
                if (ControlEvent == "CellDoubleClick") { InsertLine = "this." + ControlName + ".CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this." + ControlName + "_CellDoubleClick);"; }
                if (ControlEvent == "CellValueChanged") { InsertLine = "this." + ControlName + ".CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this." + ControlName + "_CellValueChanged);"; }
                if (ControlEvent == "KeyUp") { InsertLine = "this." + ControlName + ".KeyUp += new System.Windows.Forms.KeyEventHandler(this." + ControlName + "_KeyUp);"; }
                if (ControlEvent == "MouseClick") { InsertLine = "this." + ControlName + ".MouseClick += new System.Windows.Forms.MouseEventHandler(this." + ControlName + "_MouseClick);"; }
            }
            if (ControlType.ToUpper() == "TABCONTROL")
            {
                if (ControlEvent == "SelectedIndexChanged") { InsertLine = "this." + ControlName + ".SelectedIndexChanged += new System.EventHandler(this." + ControlName + "_SelectedIndexChanged);"; }
            }
            if (ControlType.ToUpper() == "LISTBOX")
            {
                if (ControlEvent == "SelectedIndexChanged") { InsertLine = "this." + ControlName + ".SelectedIndexChanged += new System.EventHandler(this." + ControlName + "_SelectedIndexChanged);"; }
            }
            if (ControlType.ToUpper() == "TREEVIEW")
            {
                if (ControlEvent == "ItemDrag") { InsertLine = "this." + ControlName + ".ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this." + ControlName + "_ItemDrag);"; }
                if (ControlEvent == "DragEnter") { InsertLine = "this." + ControlName + ".DragEnter += new System.Windows.Forms.DragEventHandler(this." + ControlName + "_DragEnter);"; }
                if (ControlEvent == "DragDrop") { InsertLine = "this." + ControlName + ".DragDrop += new System.Windows.Forms.DragEventHandler(this." + ControlName + "_DragDrop);"; }
                if (ControlEvent == "DragOver") { InsertLine = "this." + ControlName + ".DragOver += new System.Windows.Forms.DragEventHandler(this." + ControlName + "_DragOver);"; }
                if (ControlEvent == "AfterCheck") { InsertLine = "this." + ControlName + ".AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this." + ControlName + "_AfterCheck);"; }
                if (ControlEvent == "AfterSelect") { InsertLine = "this." + ControlName + ".AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this." + ControlName + "_AfterSelect);"; }
                if (ControlEvent == "NodeMouseClick") { InsertLine = "this." + ControlName + ".NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this." + ControlName + "_NodeMouseClick);"; }
                if (ControlEvent == "NodeMouseDoubleClick") { InsertLine = "this." + ControlName + ".NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this." + ControlName + "_NodeMouseDoubleClick);"; }
            }
            if (ControlType.ToUpper() == "TEXTBOX")
            {
                if (ControlEvent == "TextChanged") { InsertLine = "this." + ControlName + ".TextChanged += new System.EventHandler(this." + ControlName + "_TextChanged);"; }
                if (ControlEvent == "KeyDown") { InsertLine = "this." + ControlName + ".KeyDown += new System.Windows.Forms.KeyEventHandler(this." + ControlName + "_KeyDown);"; }
            }
            if (ControlType.ToUpper() == "CHECKBOX" || ControlType.ToUpper() == "RADIOBUTTON")
            {
                if (ControlEvent == "CheckedChanged") { InsertLine = "this." + ControlName + ".CheckedChanged += new System.EventHandler(this." + ControlName + "_CheckedChanged);"; }
            }
            if (ControlType.ToUpper() == "COMBOBOX")
            {
                if (ControlEvent == "SelectionChangeCommitted") { InsertLine = "this." + ControlName + ".SelectionChangeCommitted += new System.EventHandler(this." + ControlName + "_SelectionChangeCommitted);"; }
            }

            string ToolStripMenuAction = ControlEvent; // "MenuItem_Click";
            if (ControlType.ToUpper() == "TOOLSTRIPMENUITEM") { InsertLine = "this." + ControlName + ".Click += new System.EventHandler(" + ToolStripMenuAction + ");"; }



            if (InsertLine == "") { ahk.MsgBox("Control Event Type: " + ControlEvent + " Not Found For " + ControlName + "\nNo Action Taken."); return false; }

            // check to see if there is already an entry in the designer file for this event - do nothing if so
            bool AlreadyExists = Has_Handler(ProjectFile, ControlName, ControlEvent);
            if (AlreadyExists)
            {
                if (!OverWriteOld)
                {
                    //ahk.MsgBox("Event Already Exists For " + ControlEvent); 
                    return true;
                }
                if (OverWriteOld)
                {
                    DesignerFile_Remove_Handler(ProjectFile, ControlName, ControlEvent);  // remove existing handler line for a control in Designer file
                }
            }

            // find last line in the section for this control (to insert under)
            List<string> Lines = Designer_ControlSetup_List(ProjectFile, ControlName);
            string LastLine = "";
            foreach (string line in Lines)   // return the last control setup line for this control name
            {
                // ignore these lines - fall outside of the control definition block
                if (line.Contains(".ResumeLayout(")) { continue; }
                if (line.Contains(".SuspendLayout(")) { continue; }
                LastLine = line;
            }

            string NewCode = "";
            List<string> DesignerLines = lst.TextFile_To_List(ProjectFile, false, false, false);
            bool InsertedNewLine = false;
            foreach (string dLine in DesignerLines)
            {
                if (dLine.Trim() != LastLine.Trim())
                {
                    if (NewCode != "") { NewCode = NewCode + "\n" + dLine; continue; }
                    if (NewCode == "") { NewCode = dLine; continue; }
                }

                if (dLine.Trim() == LastLine.Trim())
                {
                    NewCode = NewCode + "\n" + dLine + "\n\t\t\t" + InsertLine;
                    InsertedNewLine = true;
                }
            }

            if (!InsertedNewLine) ahk.MsgBox("Didn't Insert New Line - No Match To LastLine??");

            // overwrite previous designer file after writing code with new insert line
            string ErrorMsg = "";
            bool BackedUp = Write_ProjectFile_With_Backup(ProjectFile, NewCode, out ErrorMsg);

            if (ErrorMsg != "") { ahk.MsgBox("Backup Error: " + ErrorMsg); return false; }

            return true;
        }
        public void DesignerFile_Insert_Handler_List(string ProjectFile, string ControlType, string ControlName, List<string> HandlerEvents)  // insert new control handler line in Designer file (from list of handler events to insert)
        {
            int Count = 0;
            foreach (string HandlerType in HandlerEvents)
            {
                Count++;
                bool Inserted = DesignerFile_Insert_Handler(ProjectFile, ControlType, ControlName, HandlerType, false);
                if (!Inserted) ahk.MsgBox("Inserted Line = " + Inserted.ToString() + "\nHandler Event= " + HandlerType);
            }
        }
        public bool DesignerFile_Remove_Handler(string ProjectFile, string ControlName, string ControlEvent)  // remove existing handler line for a control in Designer file
        {
            ProjectFile = DesignerPath(ProjectFile);

            string NewCode = "";
            List<string> DesignerLines = lst.TextFile_To_List(ProjectFile, false, false, false);

            foreach (string dLine in DesignerLines)
            {
                if (dLine.Contains("this." + ControlName + "." + ControlEvent + " +="))
                {
                    continue; // skip writing this line back to file
                }
                if (!dLine.Contains("this." + ControlName + "." + ControlEvent + " +="))
                {
                    if (NewCode != "") { NewCode = NewCode + "\n" + dLine; continue; }
                    if (NewCode == "") { NewCode = dLine; continue; }
                }

            }

            // overwrite previous designer file after writing code with new insert line
            string ErrorMsg = "";
            bool BackedUp = Write_ProjectFile_With_Backup(ProjectFile, NewCode, out ErrorMsg);
            return BackedUp;
        }
        public string DesignerFile_Update_Handler(string CodePath, string ControlName, string NewAction)  // update event handler line in the designer.cs file  (older - unsure if best method - can delete and insert new)
        {
            //==================================================
            // change existing event handler in Designer.CS 
            //==================================================

            //string codeFile = @"C:\Users\jason\Google Drive\IMDB\SQLiter\GUI_Dev\Gui_Dev.cs";
            string DesignerFilePath = DesignerPath(CodePath);

            ahk.Backup_File(DesignerFilePath);  // backup design file before changing

            List<string> DesignerLines = lst.Text_To_List(DesignerFilePath, false, false, false);

            ahk.FileDelete(DesignerFilePath);

            //string ToReplace = "this.button1.Click += new System.EventHandler(this.button1_Click);";
            //string NewLine = "this.button1.Click += new System.EventHandler(this.button1_NEWClick);";

            //string control = "button1";
            //string OldAction = "button1_Click";
            //string NewAction = "button1_Click";  //"button1_NEWClick"; 

            string NewText = "";

            foreach (string Line in DesignerLines)
            {
                string WriteLine = Line;

                //ahk.MsgBox(WriteLine); 

                if (Line.Contains(ControlName) && Line.Contains("new System.EventHandler"))
                {
                    string OldAction = DesignerFile_HandlerAction(Line); //extract old action handler action to replace

                    if (ahk.IfInString("this.", NewAction))
                    { WriteLine = Line.Replace(OldAction, NewAction); }

                    if (!ahk.IfInString("this.", NewAction))
                    { WriteLine = Line.Replace(OldAction, "this." + NewAction); }


                    WriteLine = Line.Replace(OldAction, NewAction);

                    //ahk.MsgBox("Found " + Line);
                    //WriteLine = NewLine; 
                }

                if (NewText != "") { NewText = NewText + Environment.NewLine + WriteLine; }
                if (NewText == "") { NewText = WriteLine; }
            }


            ahk.FileAppend(NewText, DesignerFilePath);

            return NewText;
        }
        public string DesignerFile_HandlerAction(string EventLine)  // extract / return event handler action from event handler code line
        {
            //string st = "this.button1.Click += new System.EventHandler(this.button1_Click);";
            string str = ahk.StringSplit(EventLine, "(", 1);
            string stri = ahk.StringSplit(str, ")", 0);
            stri = ahk.StringReplace(stri, "this.");
            //ahk.MsgBox(stri);
            return stri;
        }

        // Populate Event Handlers by Control Type - Used along side Insert Code to define control actions 
        public List<string> DataGridView_Handlers()  // Populate DataGridView Event Handlers - Used along side Insert Code to define control actions 
        {
            List<string> dataGridView_Handlers = new List<string>();
            dataGridView_Handlers.Add("CellClick");
            dataGridView_Handlers.Add("CellDoubleClick");
            dataGridView_Handlers.Add("CellValueChanged");
            dataGridView_Handlers.Add("KeyUp");
            dataGridView_Handlers.Add("MouseClick");
            return dataGridView_Handlers;
        }
        public List<string> TabControl_Handlers()  // Populate TabControl Event Handlers - Used along side Insert Code to define control actions 
        {
            List<string> tabControl_Handlers = new List<string>();
            tabControl_Handlers.Add("SelectedIndexChanged");
            return tabControl_Handlers;
        }
        public List<string> ListBox_Handlers()  // Populate ListBox Event Handlers - Used along side Insert Code to define control actions 
        {
            List<string> listBox_Handlers = new List<string>();
            listBox_Handlers.Add("SelectedIndexChanged");
            return listBox_Handlers;
        }
        public List<string> TreeView_Handlers(bool IncludeDragDrop = false, bool DragDropOnly = false)  // Populate TreeView Event Handlers - Used along side Insert Code to define control actions 
        {
            List<string> treeView_Handlers = new List<string>();

            if (IncludeDragDrop || DragDropOnly)
            {
                treeView_Handlers.Add("ItemDrag");
                treeView_Handlers.Add("DragEnter");
                treeView_Handlers.Add("DragDrop");
                treeView_Handlers.Add("DragOver");
            }

            if (DragDropOnly) { return treeView_Handlers; }

            treeView_Handlers.Add("AfterCheck");
            treeView_Handlers.Add("AfterSelect");
            treeView_Handlers.Add("NodeMouseClick");
            treeView_Handlers.Add("NodeMouseDoubleClick");
            return treeView_Handlers;
        }
        public List<string> TextBox_Handlers()  // Populate TextBox Event Handlers - Used along side Insert Code to define control actions 
        {
            List<string> textBox_Handlers = new List<string>();
            textBox_Handlers.Add("TextChanged");
            textBox_Handlers.Add("KeyDown");
            return textBox_Handlers;
        }
        public List<string> CheckBox_Handlers()  // Populate CheckBox Event Handlers - Used along side Insert Code to define control actions 
        {
            List<string> cb_Handlers = new List<string>();
            cb_Handlers.Add("CheckedChanged");
            return cb_Handlers;
        }
        public List<string> RadioButton_Handlers()  // Populate RadioButton Event Handlers - Used along side Insert Code to define control actions 
        {
            List<string> cb_Handlers = new List<string>();
            cb_Handlers.Add("CheckedChanged");
            return cb_Handlers;
        }
        public List<string> ComboBox_Handlers()  // Populate ComboBox Event Handlers - Used along side Insert Code to define control actions 
        {
            List<string> cb_Handlers = new List<string>();
            cb_Handlers.Add("SelectionChangeCommitted");
            return cb_Handlers;
        }


        #endregion

        #region === Parse: Functions ===

        // return # of functions found in code
        public int Function_Count_FromFile(string ProjectFile)  //returns # of functions found in file
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return 0; }
            string ParseText = ahk.FileRead(ProjectFile); //read file if text file passed in 

            int count = Function_Count_FromCode(ParseText);

            return count;
        }
        public int Function_Count_FromCode(string CodeString)  //returns # of functions found in code
        {
            List<string> LineListF = FunctionList_FromCode(CodeString);

            return LineListF.Count();
        }


        // list of all functions in project
        public List<string> FunctionList_FromFile(string ProjectFile)  // returns list of functions found in ParseText
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return null; }

            string ParseText = ahk.FileRead(ProjectFile); //read file if text file passed in 

            List<string> FuncList = FunctionList_FromCode(ParseText);

            return FuncList;
        }
        public List<string> FunctionList_FromCode(string CodeString)  // returns list of functions found in ParseText
        {
            List<string> FuncList = new List<string>();
            List<string> LineList = ahk.LineList(CodeString, false, false);

            foreach (string Line in LineList)
            {
                if (Line == "") { continue; }

                bool FunctionLine = IsFunction(Line);

                if (FunctionLine)
                {
                    FuncList.Add(FunctionName(Line));

                    //if (ThisFunctionName == "}") { FunctionCapture = false; }  // fixes get-set lines
                }

            }

            return FuncList;
        }

        // list of functions in a specific function
        public List<string> FunctionList_From_RegionFile(string ProjectFile, string RegionName)  // extract list of functions inside a region
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return null; }

            string ParseText = ahk.FileRead(ProjectFile); //read file if text file passed in 

            List<string> RegFunctions = FunctionList_From_RegionCode(ParseText, RegionName);

            return RegFunctions;
        }
        public List<string> FunctionList_From_RegionCode(string CodeString, string RegionName)  // extract list of functions inside a region
        {
            string RegionC = Extract_Region_FromCode(CodeString, RegionName);
            List<string> RegFunctions = FunctionList_FromCode(RegionC);
            return RegFunctions;
        }

        // list of functions NOT in regions
        public List<string> FunctionList_Not_In_Regions_FromFile(string ProjectFile)  // returns list of functions not inside of a region (from file)
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return null; }
            string ParseText = ahk.FileRead(ProjectFile); //read file if text file passed in 

            List<string> FuncList = FunctionList_Not_In_Regions_FromCode(ParseText);

            return FuncList;
        }
        public List<string> FunctionList_Not_In_Regions_FromCode(string CodeString)  // returns list of functions not inside of a region (from code)
        {
            CodeString = Remove_All_Regions_FromCode(CodeString, true);

            List<string> FuncList = FunctionList_FromCode(CodeString);

            return FuncList;
        }


        // Menu Function Lists

        /// <summary>
        /// returns list of tool menu functions from file
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <returns></returns>
        public List<string> Menu_Functions_FromFile(string ProjectFile) 
        {
            List<string> MenuFunctions = Menu_Functions_FromCode(ahk.FileRead(ProjectFile));
            return MenuFunctions;
        }

        /// <summary>
        /// returns list of tool menu functions from code string
        /// </summary>
        /// <param name="CodeString"></param>
        /// <returns></returns>
        public List<string> Menu_Functions_FromCode(string CodeString)
        {
            List<string> AllFunctions = FunctionList_FromCode(CodeString);
            List<string> MenuFunctions = new List<string>();

            foreach (string function in AllFunctions)
            {
                if (function.Contains("ToolStripMenuItem_Click"))
                {
                    MenuFunctions.Add(function);
                }
            }

            return MenuFunctions;
        }

        /// <summary>
        /// returns list of tool menu functions (not inside functions) from file
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <returns></returns>
        public List<string> Menu_Functions_NotInRegions_FromFile(string ProjectFile)  
        {
            List<string> MenuFunctions = Menu_Functions_NotInRegions_FromCode(ahk.FileRead(ProjectFile));
            return MenuFunctions;
        }

        /// <summary>
        /// returns list of tool menu functions (not inside functions) from code
        /// </summary>
        /// <param name="CodeString"></param>
        /// <returns></returns>
        public List<string> Menu_Functions_NotInRegions_FromCode(string CodeString) 
        {
            List<string> FunctionsNotInRegions = FunctionList_Not_In_Regions_FromCode(CodeString);

            List<string> MenuFunctions = new List<string>();

            foreach (string function in FunctionsNotInRegions)
            {
                if (function.Contains("ToolStripMenuItem_Click"))
                {
                    MenuFunctions.Add(function);
                }
            }

            return MenuFunctions;
        }


        /// <summary>
        /// move function in project into a different region 
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <param name="RegionName"></param>
        /// <param name="FunctionName"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        public bool Move_Function_Into_Region_ToFile(string ProjectFile, string RegionName, string FunctionName, out string ErrorMsg) 
        {
            // read project file to code string
            ErrorMsg = ahk.FileName(ProjectFile) + " Not Found - Unable To Insert";
            if (!File.Exists(ProjectFile)) { if (GlobalDebug) { ahk.MsgBox(ErrorMsg); } return false; }
            string CodeString = ahk.FileRead(ProjectFile);

            // move function in the code to desired region
            string NewProjectCode = Move_Function_Into_Region_ToCode(CodeString, RegionName, FunctionName, out ErrorMsg);
            if (NewProjectCode == "") { return false; }


            // Write New ProjectFile with Updated Code, Backing up the Project File First
            bool WroteNewFile = Write_ProjectFile_With_Backup(ProjectFile, NewProjectCode, out ErrorMsg);
            if (!WroteNewFile) { return false; }

            // return success
            ErrorMsg = "Moved Function: " + FunctionName + " Into Region: " + RegionName + " in " + ahk.FileName(ProjectFile);
            return true;
        }

        /// <summary>
        /// moves function in project code to a different region, returns updated code
        /// </summary>
        /// <param name="CodeString"></param>
        /// <param name="RegionName"></param>
        /// <param name="FunctionName"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        public string Move_Function_Into_Region_ToCode(string CodeString, string RegionName, string FunctionName, out string ErrorMsg) 
        {
            // confirm region name is in the code first
            ErrorMsg = RegionName + " Not Found In Code";
            bool RegionFound = Region_In_Code(CodeString, RegionName);  // returns True if RegionName is found in CodeString
            if (!RegionFound) { if (GlobalDebug) { ahk.MsgBox(ErrorMsg); } return ""; }


            // confirm function is in the code 
            ErrorMsg = FunctionName + " Not Found In Code";
            bool FunctionFound = Function_InCode(CodeString, FunctionName);  // returns True if function name found in code string
            if (!FunctionFound) { if (GlobalDebug) { ahk.MsgBox(ErrorMsg); } return ""; }


            // extract function text, remove that function from the code, then insert function into desired region
            string FunctionText = Return_Function_FromCode(CodeString, FunctionName);  // find function by name in code string,  returns code in that function
            string NewProjectCode = Remove_Function_FromCode(CodeString, FunctionName);  // remove function from file, returns project code without that function
            NewProjectCode = Insert_New_Function_Into_Region_ToCode(NewProjectCode, RegionName, FunctionText, out ErrorMsg);

            return NewProjectCode;
        }



        /// <summary>
        /// return function code by function name 
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <param name="FunctionName"></param>
        /// <returns></returns>
        public string Return_Function_FromFile(string ProjectFile, string FunctionName)  // find function by name in code file, returns code in that function
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return ""; }

            string ParseCode = ahk.FileRead(ProjectFile);

            string ReturnCode = Return_Function_FromCode(ParseCode, FunctionName);

            return ReturnCode;
        }

        /// <summary>
        /// find function by name in code string,  returns code in that function
        /// </summary>
        /// <param name="CodeString"></param>
        /// <param name="functionName"></param>
        /// <returns></returns>
        public string Return_Function_FromCode(string CodeString, string functionName)  
        {
            List<string> LineListF = lst.Text_To_List(CodeString, false, false, false);

            //string cLine = ahk.RemoveComments(Line);

            int BracketCount = 0;
            bool BracketStart = false;
            bool FunctionCapture = false;
            string FunctionCapText = "";
            string ThisFunctionName = "";
            string docReturn = "";

            foreach (string Line in LineListF)
            {
                // make sure line isn't blank first
                string LineCheck = Line;
                LineCheck = LineCheck.Trim();
                //if (LineCheck == "") { continue; }

                bool FunctionLine = IsFunction(Line);
                bool DocLine = DocumentationLine(Line);

                if (DocLine)
                {
                    if (docReturn != "") { docReturn = docReturn + "\n" + Line; }
                    if (docReturn == "") { docReturn = Line; }
                }

                if (FunctionLine)
                {
                    ThisFunctionName = FunctionName(Line);

                    if (ThisFunctionName == functionName) { FunctionCapture = true; }

                    docReturn = "";   // reset documentation capture 
                }

                if (FunctionCapture)
                {
                    if (FunctionCapText != "") { FunctionCapText = FunctionCapText + Environment.NewLine + Line; }
                    if (FunctionCapText == "") { FunctionCapText = Line; }

                    if (Line.Contains("{"))
                    {
                        int bCount = Line.Count(f => f == '{');  // count number of brackets to account for in this line
                        BracketStart = true;
                        BracketCount += bCount;
                        //ahk.MsgBox("Bracket Start: " + Line); 
                    }

                    if (BracketStart)
                    {
                        if (Line.Contains("}"))
                        {
                            int bCount = Line.Count(f => f == '}');
                            BracketCount -= bCount;
                        }

                        if (BracketCount == 0)
                        {
                            FunctionCapture = false;
                            //ahk.MsgBox(ThisFunctionName + " : " + FunctionCapText);
                            //FunctionCapText = "";
                            //ThisFunctionName = "";
                            BracketStart = false;

                            return FunctionCapText;
                        }
                    }
                }

            }

            return "";
        }


        /// <summary>
        /// returns true if line passed in is a documentation line with "///" 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public bool DocumentationLine(string line)
        {
            string lineText = line.Trim();

            string firstThree = ahk.StringLeft(lineText, "3");
            if (firstThree == "///")
            {
                string firstFour = ahk.StringLeft(lineText, "4");
                if (firstFour == "////")
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// returns function's documentation from cs file 
        /// </summary>
        /// <param name="CodeString"></param>
        /// <param name="functionName"></param>
        /// <returns></returns>
        public string Return_FunctionDocumentation_FromCode(string CodeString, string functionName)
        {
            List<string> LineListF = lst.Text_To_List(CodeString, false, false, false);

            string FunctionCapText = "";
            string ThisFunctionName = "";
            string docReturn = "";

            foreach (string Line in LineListF)
            {
                // make sure line isn't blank first
                string LineCheck = Line.Trim();
                if (LineCheck == "") { continue; }

                bool DocLine = DocumentationLine(Line);
                if (DocLine)
                {
                    if (docReturn != "") { docReturn = docReturn + "\n" + Line; }
                    if (docReturn == "") { docReturn = Line; }
                    continue;
                }

                bool FunctionLine = IsFunction(Line);
                if (FunctionLine)
                {
                    ThisFunctionName = FunctionName(Line);

                    if (ThisFunctionName == functionName) { return docReturn; }

                    docReturn = ""; // reset documentation capture 
                }


            }

            return "";
        }


        /// <summary>
        /// returns function's documentation from cs file - works but slow 
        /// </summary>
        /// <param name="CodeString"></param>
        /// <param name="functionName"></param>
        /// <returns></returns>
        public string slowReturn_FunctionDocumentation_FromCode(string CodeString, string functionName)
        {
            List<string> LineListF = lst.Text_To_List(CodeString, false, false, false);

            //string cLine = ahk.RemoveComments(Line);

            int BracketCount = 0;
            bool BracketStart = false;
            bool FunctionCapture = false;
            string FunctionCapText = "";
            string ThisFunctionName = "";
            string docReturn = "";

            foreach (string Line in LineListF)
            {
                // make sure line isn't blank first
                string LineCheck = Line;
                LineCheck = LineCheck.Trim();
                //if (LineCheck == "") { continue; }

                bool FunctionLine = IsFunction(Line);
                bool DocLine = DocumentationLine(Line);

                if (DocLine)
                {
                    if (docReturn != "") { docReturn = docReturn + "\n" + Line; }
                    if (docReturn == "") { docReturn = Line; }
                }

                if (FunctionLine)
                {
                    ThisFunctionName = FunctionName(Line);

                    if (ThisFunctionName == functionName) { FunctionCapture = true; }

                    if (!FunctionCapture)
                    {
                        docReturn = ""; // reset documentation capture 
                    }

                }

                if (FunctionCapture)
                {
                    if (FunctionCapText != "") { FunctionCapText = FunctionCapText + Environment.NewLine + Line; }
                    if (FunctionCapText == "") { FunctionCapText = Line; }

                    if (Line.Contains("{"))
                    {
                        int bCount = Line.Count(f => f == '{');  // count number of brackets to account for in this line
                        BracketStart = true;
                        BracketCount += bCount;
                        //ahk.MsgBox("Bracket Start: " + Line); 
                    }

                    if (BracketStart)
                    {
                        if (Line.Contains("}"))
                        {
                            int bCount = Line.Count(f => f == '}');
                            BracketCount -= bCount;
                        }

                        if (BracketCount == 0)
                        {
                            FunctionCapture = false;
                            //ahk.MsgBox(ThisFunctionName + " : " + FunctionCapText);
                            //FunctionCapText = "";
                            //ThisFunctionName = "";
                            BracketStart = false;

                            return docReturn;
                        }
                    }
                }

            }

            return "";
        }


        /// <summary>
        /// Return the last Function Name found in the Region 
        /// </summary>
        /// <param name="CodeString"></param>
        /// <param name="RegionName"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        public string Last_FunctionName_In_Region_FromCode(string CodeString, string RegionName, out string ErrorMsg)  
        {
            ErrorMsg = RegionName + " Not Found In Code";
            bool RegionFound = Region_In_Code(CodeString, RegionName);  // returns True if RegionName is found in CodeString
            if (!RegionFound) { if (GlobalDebug) { ahk.MsgBox(ErrorMsg); } return ""; }

            List<string> RegionFunctions = FunctionList_From_RegionCode(CodeString, RegionName);  // extract list of functions inside a region

            string LastFunction = "";
            foreach (string function in RegionFunctions)
            {
                LastFunction = function;
            }

            return LastFunction;
        }



        /// <summary>
        /// pass in project.cs file, checks function text to see if line exists
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <param name="FunctionName"></param>
        /// <param name="FunctionLine"></param>
        /// <returns></returns>
        public bool Line_In_Function_FromFile(string ProjectFile, string FunctionName, string FunctionLine)  
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return false; }

            string CodeString = ahk.FileRead(ProjectFile);

            bool Found = Line_In_Function_FromCode(CodeString, FunctionName, FunctionLine);

            return Found;
        }

        /// <summary>
        /// pass in project code, checks function text to see if line exists
        /// </summary>
        /// <param name="CodeString"></param>
        /// <param name="FunctionName"></param>
        /// <param name="FunctionLine"></param>
        /// <returns></returns>
        public bool Line_In_Function_FromCode(string CodeString, string FunctionName, string FunctionLine)  
        {
            string FunctionCode = Return_Function_FromCode(CodeString, FunctionName);

            List<string> LineList = ahk.LineList(FunctionCode, true, true);

            foreach (string Line in LineList)
            {
                if (Line == FunctionLine.Trim()) { return true; }
            }

            return false;
        }


        /// <summary>
        /// pass in project code, find function, return first line of code in that function
        /// </summary>
        /// <param name="CodeString"></param>
        /// <param name="FunctionName"></param>
        /// <returns></returns>
        public string First_CodeLine_In_Function_FromCode(string CodeString, string FunctionName)  
        {
            string FunctionCode = Return_Function_FromCode(CodeString, FunctionName);

            List<string> LineList = ahk.LineList(FunctionCode, true, true);

            foreach (string Line in LineList)
            {
                return Line; // return first line in the function
            }

            return "";
        }

        /// <summary>
        /// pass in project code, find function, return last line of code in that function
        /// </summary>
        /// <param name="CodeString"></param>
        /// <param name="FunctionName"></param>
        /// <returns></returns>
        public string Last_CodeLine_In_Function_FromCode(string CodeString, string FunctionName)  
        {
            string FunctionCode = Return_Function_FromCode(CodeString, FunctionName);

            List<string> LineList = ahk.LineList(FunctionCode, false, true);

            string LastLine = "";
            foreach (string Line in LineList)
            {
                string LineTrim = Line.Trim();
                if (LineTrim != "}") { LastLine = Line; } // stores the last line in the function
            }

            return LastLine;
        }


        /// <summary>
        /// checks to see if line passed in is a function declaration
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public bool IsFunction(string Line)  
        {
            string FirstWord = ahk.FirstWord(Line);

            if (Line.Contains("get;") && Line.Contains("set;")) { return false; }


            bool MaybeFunc = false;

            if (FirstWord == "public" || FirstWord == "private") { MaybeFunc = true; }

            if (!Line.Contains("(")) { MaybeFunc = false; }

            if (MaybeFunc)
            {
                List<string> WordList = ahk.WordList(Line);
                foreach (string Word in WordList)
                {
                    if (Word == "class") { MaybeFunc = false; }
                }

            } // if class return false

            return MaybeFunc;

        }
        
        /// <summary>
        /// return Function name from Function Line
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public string FunctionName(string Line)  
        {
            string FunctionNameLine = ahk.StringSplit(Line, "(", 0);  // text before split character

            return ahk.LastWord(FunctionNameLine);
        }

        /// <summary>
        /// return Function paramaters from Function Line
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public string FunctionParams(string Line)  
        {
            string FunctionName = ahk.StringSplit(Line, "(", 0);  // text before split character
            string FunctionParams = ahk.StringSplit(Line, "(", 1);  // text after split character

            string Params = ahk.StringSplit(FunctionParams, ")", 0);  // text after split character

            return Params;
        }

        /// <summary>
        /// return comments on function line
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public string FunctionComments(string Line) 
        {
            string FunctionName = ahk.StringSplit(Line, "(", 0);  // text before split character
            string FunctionParams = ahk.StringSplit(Line, "(", 1);  // text after split character

            string Params = ahk.StringSplit(FunctionParams, ")", 0);  // text after split character

            return Params;
        }

        /// <summary>
        /// returns single function name from code string
        /// </summary>
        /// <param name="CodeString"></param>
        /// <returns></returns>
        public string FunctionName_FromCode(string CodeString)  
        {
            List<string> LineList = ahk.LineList(CodeString, false, true);
            foreach (string line in LineList)
            {
                bool funcLine = IsFunction(line);
                if (funcLine)
                {
                    return FunctionName(line);
                }
            }

            return "";
        }


        /// <summary>
        /// return list of functions containing SearchTerm
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <param name="SearchTerm"></param>
        /// <returns></returns>
        public List<string> Return_FunctionText_Matches_FromFile(string ProjectFile, string SearchTerm)  
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return null; }

            string CodeString = ahk.FileRead(ProjectFile);

            List<string> FoundInFunctions = Return_FunctionText_Matches_FromCode(CodeString, SearchTerm);

            return FoundInFunctions;
        }

        /// <summary>
        /// return list of functions containing SearchTerm
        /// </summary>
        /// <param name="CodeString"></param>
        /// <param name="SearchTerm"></param>
        /// <returns></returns>
        public List<string> Return_FunctionText_Matches_FromCode(string CodeString, string SearchTerm)  
        {
            List<string> Functions = FunctionList_FromCode(CodeString);

            List<string> FoundInFunctions = new List<string>();

            foreach (string func in Functions)
            {
                string funcCode = Return_Function_FromCode(CodeString, func);

                if (funcCode.Contains(SearchTerm)) { FoundInFunctions.Add(func); }
            }

            return FoundInFunctions;
        }

        /// <summary>
        /// returns true if TextLine is found in FilePath
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <param name="TextLine"></param>
        /// <returns></returns>
        public bool Line_In_CodeFile(string ProjectFile, string TextLine) 
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return false; }

            string CodeString = ahk.FileRead(ProjectFile);

            bool Found = Line_In_Code(CodeString, TextLine);

            return Found;
        }

        /// <summary>
        /// returns true if TextLine is found in FilePath
        /// </summary>
        /// <param name="CodeString"></param>
        /// <param name="TextLine"></param>
        /// <returns></returns>
        public bool Line_In_Code(string CodeString, string TextLine)  
        {
            List<string> Lines = lst.Text_To_List(CodeString);

            foreach (string line in Lines)
            {
                string CheckLine = line.Trim();

                if (CheckLine == TextLine.Trim()) { return true; }
            }

            return false;
        }


        /// <summary>
        /// returns name of function in FilePath containing "InitializeComponent();"
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <returns></returns>
        public string Find_Initialize_Function_FromFile(string ProjectFile)  
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return null; }
            string CodeString = ahk.FileRead(ProjectFile);

            string InitializeFunction = Find_Initialize_Function_FromCode(CodeString);

            return InitializeFunction;
        }

        /// <summary>
        /// returns name of function in FilePath containing "InitializeComponent();"
        /// </summary>
        /// <param name="CodeString"></param>
        /// <returns></returns>
        public string Find_Initialize_Function_FromCode(string CodeString)  
        {
            string SearchTerm = "InitializeComponent();";
            List<string> matches = Return_FunctionText_Matches_FromCode(CodeString, SearchTerm);

            string InitializeFunction = "";

            foreach (string match in matches)
            {
                InitializeFunction = match;
            }

            return InitializeFunction;
        }


        /// <summary>
        /// return first (or only) function in code file
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <returns></returns>
        public string FirstFunction_FromFile(string ProjectFile)  
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return null; }

            string CodeString = ahk.FileRead(ProjectFile);
            string FunctionName = FirstFunction_FromCode(CodeString);
            return FunctionName;
        }

        /// <summary>
        /// return first (or only) function in code file
        /// </summary>
        /// <param name="CodeString"></param>
        /// <returns></returns>
        public string FirstFunction_FromCode(string CodeString)  
        {
            List<string> Functions = FunctionList_FromCode(CodeString);

            string FunctionName = "";

            foreach (string funct in Functions)
            {
                FunctionName = funct;
                break;
            }

            return FunctionName;
        }

        /// <summary>
        /// return last function listed in code file
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <returns></returns>
        public string LastFunction_FromFile(string ProjectFile)  
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return null; }

            string CodeString = ahk.FileRead(ProjectFile);

            string FunctionName = LastFunction_FromCode(CodeString);

            return FunctionName;
        }

        /// <summary>
        /// return last function listed in code string
        /// </summary>
        /// <param name="CodeString"></param>
        /// <returns></returns>
        public string LastFunction_FromCode(string CodeString)  
        {
            List<string> Functions = FunctionList_FromCode(CodeString);

            string FunctionName = "";

            foreach (string funct in Functions)
            {
                FunctionName = funct;
            }

            return FunctionName;
        }


        /// <summary>
        /// returns True if function name found in file
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <param name="FunctionName"></param>
        /// <returns></returns>
        public bool Function_InFile(string ProjectFile, string FunctionName) 
        {
            // Read Project File Text - Return Error If Not Found
            string ErrorMsg = ProjectFile + " Not Found.";
            if (!File.Exists(ProjectFile)) { if (GlobalDebug) { ahk.MsgBox(ErrorMsg); } return false; }
            string ParseCode = ahk.FileRead(ProjectFile);

            bool FoundFunction = Function_InCode(ParseCode, FunctionName);
            if (!FoundFunction) { ErrorMsg = FunctionName + " Not Found In " + ahk.FileName(ProjectFile); return FoundFunction; } // didn't find function

            // found function
            ErrorMsg = FunctionName + " Found In " + ahk.FileName(ProjectFile);
            return FoundFunction;
        }

        /// <summary>
        /// returns True if function name found in code string
        /// </summary>
        /// <param name="CodeString"></param>
        /// <param name="FunctionName"></param>
        /// <returns></returns>
        public bool Function_InCode(string CodeString, string FunctionName)  
        {
            bool FoundFunction = false;

            List<string> Functions = FunctionList_FromCode(CodeString);

            foreach (string Funct in Functions)
            {
                if (Funct == FunctionName) { return true; }
            }

            return FoundFunction;
        }



        //public void Extract_Function_Paramaters(string ProjectFile)
        //{
        //    string ErrorMsg = ProjectFile + " Not Found.";
        //    if (!File.Exists(ProjectFile)) { ahk.MsgBox(ErrorMsg); return; }
        //    string CodeString = ahk.FileRead(ProjectFile);

        //    List<string> LineList = lst.Text_To_List(CodeString, true, true, true);

        //    foreach(string Line in LineList)
        //    {
        //        if (IsFunction(Line))
        //        {
        //            //ahk.MsgBox(Line);
        //            string SendLine = ahk.RemoveComments(Line);
        //            string ReturnVal = Parse_Function_Parameters_Line(SendLine);  // parse function line, return parameters parced
        //        }

        //    }

        //    return;
        //}

        //public string Parse_Function_Parameters_Line(string line = "", string ControlName1 = "", string ControlName2 = "", string ControlName3 = "")  // parse function line, return parameters parced
        //{
        //    //line = "public bool FileAppend(string WriteText, string FileName, string Debug= \"false\", bool value=false, bool value2 = true)  //note about function";

        //    string Comments = "";
        //    string LineType = "";
        //    line = ParseCodeLine(line, out Comments, out LineType);  //separate comments and code

        //    int Counter = 0;
        //    string StartOfLine = "";
        //    string FunctionParameters = "";

        //    // extract text before and after parenthesis to parse
        //    string[] words = line.Split('(');
        //    foreach (string word in words)
        //    {
        //        //ahk.MsgBox(word);

        //        if (Counter == 0) { StartOfLine = word; }
        //        if (Counter == 1) { FunctionParameters = word; }

        //        Counter++;
        //    }

        //    //ahk.MsgBox("Start: " + StartOfLine);
        //    //ahk.MsgBox("Parameters: " + FunctionParameters);


        //    //========================================================================
        //    // Parse first part of the line for the function name / output type
        //    //========================================================================

        //    string FunctionName = "";
        //    string FunctionOutput = "";
        //    string FunctionVisibility = "";
        //    bool Static = false;
        //    words = StartOfLine.Split(' ');
        //    Counter = 0;

        //    // count total number of words in string
        //    int WordCountTotal = 0;
        //    foreach (string wordcount in words) { WordCountTotal++; }
        //    //ahk.MsgBox("WordCountTotal: " + WordCountTotal.ToString()); 

        //    foreach (string word in words)
        //    {
        //        Counter++;

        //        if (Counter == 1) { FunctionVisibility = word; }

        //        if (word == "static") { Static = true; continue; }

        //        if (Counter > 1 && Counter < WordCountTotal) { FunctionOutput = word; }

        //        FunctionName = word;  //function will be the last word in the loop
        //    }

        //    //ahk.MsgBox("Function: " + FunctionName + Environment.NewLine + "Output: " + FunctionOutput + Environment.NewLine + "Static: " + Static.ToString() + Environment.NewLine + "Comments: " + Comments);

        //    FunctionParameters = ahk.TrimLastCharacter(FunctionParameters); //trim off trailing )
        //    //ahk.MsgBox(FunctionParameters);


        //    string Return_Function = "";

        //    //=================================
        //    // parse function parameters

        //    //string WriteText, string FileName, bool Debug = false

        //    Counter = 0;
        //    string[] paramaterarray = FunctionParameters.Split(',');
        //    foreach (string parameter in paramaterarray)
        //    {
        //        //ahk.MsgBox(parameter);  //bool Debug = false

        //        string[] paramaters = parameter.Split(' ');
        //        Counter = 0;
        //        string ParamType = "";
        //        string ParamName = "";
        //        string ParamDefaultVal = "";
        //        bool DefaultValFound = false;
        //        foreach (string par in paramaters)
        //        {
        //            string ParameterWord = par;
        //            ParameterWord = par.Trim();
        //            if (ParameterWord == "") { continue; }


        //            if (parameter.Contains("=")) { DefaultValFound = true; }  //if there is a default value in the function declaration

        //            Counter++;
        //            if (Counter == 1) { ParamType = par; }
        //            if (Counter == 2) { ParamName = par; }


        //            if (ParamName.Contains("="))  //if there is a default value in the function declaration
        //            {
        //                int ValCounter = 0;
        //                string[] paramatervalue = ParamName.Split('=');
        //                foreach (string parval in paramatervalue)
        //                {
        //                    string ValLine = parval;
        //                    ValLine = parval.Trim();
        //                    if (ValLine == "") { continue; }

        //                    if (ValCounter == 0) { ParamName = ValLine; }
        //                    if (ValCounter == 1) { ParamDefaultVal = ValLine; }

        //                    ValCounter++;
        //                }

        //                DefaultValFound = false;
        //            }


        //            if (DefaultValFound)  // if the default value is provided in the parameter line
        //            {
        //                int ValCounter = 0;
        //                string[] paramatervalue = parameter.Split('=');
        //                foreach (string parval in paramatervalue)
        //                {
        //                    string ValLine = parval;
        //                    ValLine = parval.Trim();
        //                    if (ValLine == "") { continue; }

        //                    if (ValCounter == 1) { ParamDefaultVal = ValLine; }

        //                    ValCounter++;
        //                }
        //            }

        //            DefaultValFound = false;

        //        }

        //        ahk.MsgBox("Line: " + line + Environment.NewLine + "Counter = " + Counter + Environment.NewLine + "ParamType = " + ParamType + Environment.NewLine + "ParamName = " + ParamName + Environment.NewLine + "DefaultValue = " + ParamDefaultVal);
        //        //if (Counter == 0) { StartOfLine = word; }
        //        //if (Counter == 1) { FunctionParameters = word; }

        //        // recognized param types to assign values 
        //        //if (ParamType == "TreeView" || ParamType == "DataGridView" || ParamType == "string" || ParamType == "bool" || ParamType == "int" || ParamType == "List<string>" || ParamType == "List<int>" || ParamType == "List<TreeNode>" || ParamType == "TreeNode")

        //        bool Added = false;

        //        if (ParamDefaultVal == "")
        //        {

        //            string pt = ParamType.ToUpper();

        //            if (pt == "TREEVIEW" || pt == "DATAGRIDVIEW" || pt == "COMBOBOX" || pt == "SCINTILLA" || pt == "LISTBOX")
        //            {
        //                if (ControlName1.Contains(pt))
        //                {
        //                    ControlName1 = ahk.StringReplace(ControlName1, pt).Trim();

        //                    if (Return_Function != "") { Return_Function = Return_Function + ", " + ControlName1; }
        //                    if (Return_Function == "") { Return_Function = ControlName1; }
        //                    //ahk.MsgBox(Return_Function);
        //                    Added = true;
        //                }

        //                if (ControlName2.Contains(pt))
        //                {
        //                    ControlName2 = ahk.StringReplace(ControlName2, pt).Trim();

        //                    if (Return_Function != "") { Return_Function = Return_Function + ", " + ControlName2; }
        //                    if (Return_Function == "") { Return_Function = ControlName2; }
        //                    //ahk.MsgBox(Return_Function);
        //                    Added = true;
        //                }

        //                if (ControlName3.Contains(pt))
        //                {
        //                    ControlName3 = ahk.StringReplace(ControlName3, pt).Trim();

        //                    if (Return_Function != "") { Return_Function = Return_Function + ", " + ControlName3; }
        //                    if (Return_Function == "") { Return_Function = ControlName3; }
        //                    //ahk.MsgBox(Return_Function);
        //                    Added = true;
        //                }
        //            }


        //        }


        //        if (ParamDefaultVal != "")
        //        {
        //            if (Return_Function != "") { Return_Function = Return_Function + ", " + ParamDefaultVal; }
        //            if (Return_Function == "") { Return_Function = ParamDefaultVal; }
        //            //ahk.MsgBox(Return_Function);
        //            Added = true;
        //        }



        //        if (!Added)
        //        {
        //            if (ParamType == "string" || ParamType == "bool" || ParamType == "int" || ParamType == "List<string>" || ParamType == "List<int>" || ParamType == "List<TreeNode>" || ParamType == "TreeNode")
        //            {
        //                if (Return_Function != "") { Return_Function = Return_Function + ", " + ParamName; }
        //                if (Return_Function == "") { Return_Function = ParamName; }
        //                //ahk.MsgBox(Return_Function);
        //                Added = true;
        //            }

        //        }


        //        //if (!Added)
        //        //{
        //        //    ahk.MsgBox("DIDN'T ADD ::: " + Counter + Environment.NewLine + "ParamType = " + ParamType + Environment.NewLine + "ParamName = " + ParamName + Environment.NewLine + "DefaultValue = " + ParamDefaultVal);
        //        //}


        //        Counter++;
        //    }

        //    return Return_Function;

        //}


        #endregion

        #region === Parse: Regions ===

        //// unfinished
        //public TreeNode RegionNodes_FromFile(string ProjectFile)  // r
        //{
        //    if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return null; }
        //    string ParseCode = ahk.FileRead(ProjectFile);  // read file if path is passed in 

        //    return null;
        //}
        //public TreeNode RegionNodes_FromCode(string CodeString)
        //{
        //    List<string> RegionList = RegionList_FromCode(CodeString);
        //    List<string> HasChildrenList = new List<string>(); 

        //    foreach(string region in RegionList)
        //    {
        //        int SubCount = SubRegionCount_FromCode(CodeString, region);

        //        if (SubCount > 0) { HasChildrenList.Add(region);  }


        //    }


        //    //TreeNode parent = new TreeNode();
        //    //parent.Text = "Regions";


        //    //    TreeNode child2 = new TreeNode();
        //    //    child2.Text = "";
        //    //    parent.Nodes.Add(child2);



        //    //SubRegionCount_FromCode(string CodeString, string RegionName)


        //    return null;
        //}



        // return list of all regions in code

        /// <summary>
        /// return list of regions in File
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <returns></returns>
        public List<string> RegionList_FromFile(string ProjectFile)  
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return null; }

            string ParseCode = ahk.FileRead(ProjectFile);  // read file if path is passed in 

            List<string> RegList = RegionList_FromCode(ParseCode);

            return RegList;
        }

        /// <summary>
        /// return list of regions in Code
        /// </summary>
        /// <param name="CodeString"></param>
        /// <returns></returns>
        public List<string> RegionList_FromCode(string CodeString) 
        {
            List<string> RegList = new List<string>();
            List<string> LineList = ahk.LineList(CodeString);

            foreach (string Line in LineList)
            {
                if (IsRegion(Line)) { RegList.Add(RegionName(Line)); }
            }

            return RegList;
        }


        /// <summary>
        /// return list of regions inside another region
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <param name="RegionName"></param>
        /// <returns></returns>
        public List<string> SubRegionList_FromFile(string ProjectFile, string RegionName) 
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return null; }
            string CodeString = ahk.FileRead(ProjectFile);  // read file if path is passed in 

            List<string> SubRegionList = SubRegionList_FromCode(CodeString, RegionName);

            return SubRegionList;
        }

        /// <summary>
        /// return list of regions inside another region
        /// </summary>
        /// <param name="CodeString"></param>
        /// <param name="RegionName"></param>
        /// <returns></returns>
        public List<string> SubRegionList_FromCode(string CodeString, string RegionName) 
        {
            string MainRegionCode = Extract_Region_FromCode(CodeString, RegionName);  // all code from region with sub regions

            List<string> SubRegionList = RegionList_FromCode(MainRegionCode);

            //Extract_Single_Region_FromCode(CodeString, string ExtractRegionName)
            //List<string> AllRegionList = RegionList_FromCode();

            return SubRegionList;
        }



        //### Extract Region Code ###

        // returns the code inside a region (all regions in section)
        /// <summary>
        /// returns the code inside a region (all regions in section) - from a .cs file
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <param name="ExtractRegionName"></param>
        /// <returns></returns>
        public string Extract_Region_FromFile(string ProjectFile, string ExtractRegionName)
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return ""; }

            string ParseCode = ahk.FileRead(ProjectFile);   // read file if path is passed in 

            string RegionCodeSection = Extract_Region_FromCode(ParseCode, ExtractRegionName);

            return RegionCodeSection;

        }

        /// <summary>
        /// returns the code inside a region (all regions in section) - from .cs code
        /// </summary>
        /// <param name="CodeString"></param>
        /// <param name="ExtractRegionName"></param>
        /// <returns></returns>
        public string Extract_Region_FromCode(string CodeString, string ExtractRegionName)
        {
            List<string> LineList = lst.Text_To_List(CodeString, false, false, false);

            string RegionCodeSection = "";
            bool RegionStart = false;
            bool RegionCapture = false;
            string ThisRegionName = "";
            int RegionCounter = 0;
            string ThisFunctionName = "";
            bool InFunction = false;
            bool BracketStart = false;
            int BracketCount = 0;

            foreach (string Line in LineList)
            {
                //if (Line.Trim() == "#region === Startup ===")
                //{
                //    ahk.MsgBox("Pause");
                //}

                //### Check To See If We're Inside a function before attempting to grab the region start/end  #####

                bool FunctionLine = IsFunction(Line);

                if (FunctionLine)
                {
                    InFunction = true;
                    ThisFunctionName = FunctionName(Line);
                }

                if (InFunction)
                {
                    if (Line.Contains("{"))
                    {
                        BracketStart = true;
                        int bCount = Line.Count(f => f == '{');
                        BracketCount += bCount;
                    }

                    if (BracketStart)
                    {
                        if (Line.Contains("}"))
                        {
                            int bCount = Line.Count(f => f == '}');
                            BracketCount -= bCount;
                        }

                        if (BracketCount == 0)
                        {
                            InFunction = false;
                            ThisFunctionName = "";
                            BracketStart = false;
                        }
                    }
                }


                if (!InFunction)
                {
                    if (IsRegion(Line))  // beginning of region
                    {
                        if (RegionCounter < 1) { ThisRegionName = RegionName(Line); }

                        //ahk.MsgBox("[" + ExtractRegionName + "] -- [" + ThisRegionName + "]"); 

                        //if (ExtractRegionName == ThisRegionName)
                        if (ExtractRegionName == ThisRegionName || RegionCounter > 0)
                        {
                            RegionStart = true;
                            RegionCounter++;
                            RegionCapture = true;
                        }

                        //if (ExtractRegionName != ThisRegionName)
                        //{
                        //    RegionCounter++;
                        //    RegionCapture = false;
                        //}
                    }
                }


                bool EndRegionLine = false;

                if (!InFunction) { EndRegionLine = IsEndRegion(Line); }  // check for end of region if not inside function


                if (RegionCapture)
                {
                    if (EndRegionLine)  // end of region
                    {
                        //if (ExtractRegionName != ThisRegionName) { RegionCounter--; }

                        //if (RegionCapture) { RegionCounter--; }

                        RegionCounter--;

                        //if (RegionCounter == 1)
                        if (RegionCounter > 0)
                        {
                            RegionCapture = true;
                        }


                        if (RegionCounter == 0)
                        {
                            RegionCapture = false;

                            //if (ExtractRegionName == ThisRegionName)
                            //{
                            return RegionCodeSection;
                            //}
                        }
                    }

                }


                if (RegionCapture)
                {
                    if (RegionCodeSection != "")
                    {
                        if (RegionCounter == 0) { if (IsEndRegion(Line)) { continue; } }

                        if (RegionCounter > 0)
                        {
                            RegionCodeSection = RegionCodeSection + Environment.NewLine + Line;
                        }
                    }

                    if (RegionCodeSection == "")
                    {
                        //if (RegionCounter == 1)  // skip writing the first region text to output
                        if (RegionCounter > 0)
                        {
                            if (IsRegion(Line))
                            {
                                continue;
                            }

                            RegionCodeSection = Line;
                        }
                    }
                }
            }

            return RegionCodeSection;
        }


        /// <summary>
        /// returns the code inside a single region (excludes subregion code) - from a .cs file
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <param name="ExtractRegionName"></param>
        /// <returns></returns>
        public string Extract_Single_Region_FromFile(string ProjectFile, string ExtractRegionName) 
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return ""; }

            string ParseCode = ahk.FileRead(ProjectFile);   // read file if path is passed in 

            string RegionCodeSection = Extract_Single_Region_FromCode(ParseCode, ExtractRegionName);

            return RegionCodeSection;

        }

        /// <summary>
        /// returns the code inside a single region (excludes subregion code) - from .cs code
        /// </summary>
        /// <param name="CodeString"></param>
        /// <param name="ExtractRegionName"></param>
        /// <returns></returns>
        public string Extract_Single_Region_FromCode(string CodeString, string ExtractRegionName) 
        {
            List<string> LineList = lst.Text_To_List(CodeString, false, false, false);

            string RegionCodeSection = "";
            bool RegionStart = false;
            bool RegionCapture = false;
            string ThisRegionName = "";
            int RegionCounter = 0;
            string ThisFunctionName = "";
            bool InFunction = false;
            bool BracketStart = false;
            int BracketCount = 0;

            //int RegionCountTotal = RegionCount(ParseText); 

            foreach (string Line in LineList)
            {
                if (Line == "") { continue; }

                //### Check To See If We're Inside a function before attempting to grab the region start/end  #####

                bool FunctionLine = IsFunction(Line);

                if (FunctionLine)
                {
                    InFunction = true;
                    ThisFunctionName = FunctionName(Line);
                }

                if (InFunction)
                {
                    if (Line.Contains("{"))
                    {
                        BracketStart = true;
                        int bCount = Line.Count(f => f == '{');
                        BracketCount += bCount;
                    }

                    if (BracketStart)
                    {
                        if (Line.Contains("}"))
                        {
                            int bCount = Line.Count(f => f == '}');
                            BracketCount -= bCount;
                        }

                        if (BracketCount == 0)
                        {
                            InFunction = false;
                            ThisFunctionName = "";
                            BracketStart = false;
                        }
                    }
                }



                //if (!InFunction)
                //{
                //    RegionStart = IsRegion(Line);
                //}

                if (!InFunction)
                {
                    if (IsRegion(Line))  // beginning of region
                    {
                        ThisRegionName = RegionName(Line);

                        //ahk.MsgBox("[" + ExtractRegionName + "] -- [" + ThisRegionName + "]"); 

                        if (ExtractRegionName == ThisRegionName)
                        {
                            RegionStart = true;
                            RegionCounter++;
                            RegionCapture = true;
                            //RegList.Add(ThisRegionName);
                        }

                        if (ExtractRegionName != ThisRegionName)
                        {
                            RegionCounter++;
                            RegionCapture = false;
                        }

                        //ahk.FileAppend("[" + ThisRegionName + "]", OutFile); 
                    }
                }


                bool EndRegionLine = false;

                if (!InFunction) { EndRegionLine = IsEndRegion(Line); }  // check for end of region if not inside function


                if (EndRegionLine)  // end of region
                {
                    //if (ThisRegionName == "") { continue; }


                    if (ExtractRegionName != ThisRegionName) { RegionCounter--; }

                    if (RegionCapture) { RegionCounter--; }

                    if (RegionCounter == 1)
                    {
                        RegionCapture = true;
                    }


                    if (RegionCounter == 0)
                    {
                        RegionCapture = false;

                        //List<string> Functions = FunctionList(RegionCodeSection);
                        //foreach(string func in Functions)
                        //{
                        //    ahk.FileAppend("---" + func, OutFile); 
                        //}

                        //ahk.MsgBox(RegionCodeSection);
                        //ahk.MsgBox("Region: " + ThisRegionName + " Has " + RegList.Count.ToString() + " Functions");


                        if (ExtractRegionName == ThisRegionName)
                        {
                            return RegionCodeSection;
                        }



                        //RegionCodeSection = "";
                        //ThisRegionName = "";

                    }


                }

                if (RegionCapture)
                {
                    if (RegionCodeSection != "")
                    {
                        //if (RegionCounter > 1)
                        //{
                        if (IsEndRegion(Line))
                        {
                            continue;
                        }

                        //}

                        if (RegionCounter == 1)
                        {
                            RegionCodeSection = RegionCodeSection + Environment.NewLine + Line;
                        }

                        //RegionCodeSection = RegionCodeSection + Environment.NewLine + Line; 
                    }

                    if (RegionCodeSection == "")
                    {
                        if (RegionCounter == 1)  // skip writing the first region text to output
                        {
                            if (IsRegion(Line))
                            {
                                continue;
                            }

                            RegionCodeSection = Line;
                        }
                    }
                }
            }

            //ahk.Run(OutFile);

            return RegionCodeSection;

        }


        /// <summary>
        /// returns project code without a region (includes subregion code) - from .cs file
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <param name="ExtractRegionName"></param>
        /// <returns></returns>
        public string Remove_Region_FromFile(string ProjectFile, string ExtractRegionName)  
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return ""; }

            string ParseCode = ahk.FileRead(ProjectFile);   // read file if path is passed in 

            string RegionCodeSection = Remove_Region_FromCode(ParseCode, ExtractRegionName);

            return RegionCodeSection;

        }

        /// <summary>
        /// returns project code without a region (includes subregion code) - from .cs code
        /// </summary>
        /// <param name="CodeString"></param>
        /// <param name="ExtractRegionName"></param>
        /// <returns></returns>
        public string Remove_Region_FromCode(string CodeString, string ExtractRegionName) 
        {
            List<string> LineList = lst.Text_To_List(CodeString, false, false, false);

            string RegionCodeSection = "";
            bool RegionStart = false;
            bool InsideRegion = false;
            string ThisRegionName = "";
            int RegionCounter = 0;
            string ThisFunctionName = "";
            bool InFunction = false;
            bool BracketStart = false;
            int BracketCount = 0;
            string CodeWithOutRegion = "";

            foreach (string Line in LineList)
            {
                //### Check To See If We're Inside a function before attempting to grab the region start/end  #####

                bool FunctionLine = IsFunction(Line);

                if (FunctionLine)
                {
                    InFunction = true;
                    ThisFunctionName = FunctionName(Line);
                }

                if (InFunction)
                {
                    if (Line.Contains("{"))
                    {
                        BracketStart = true;
                        int bCount = Line.Count(f => f == '{');
                        BracketCount += bCount;
                    }

                    if (BracketStart)
                    {
                        if (Line.Contains("}"))
                        {
                            int bCount = Line.Count(f => f == '}');
                            BracketCount -= bCount;
                        }

                        if (BracketCount == 0)
                        {
                            InFunction = false;
                            ThisFunctionName = "";
                            BracketStart = false;
                        }
                    }
                }


                //if (!InFunction)
                //{
                //    if (IsRegion(Line))  // beginning of region
                //    {
                //        if (RegionCounter < 1) { ThisRegionName = RegionName(Line); }

                //        if (ExtractRegionName == ThisRegionName || RegionCounter > 0)
                //        {
                //            RegionStart = true;
                //            RegionCounter++;
                //            InsideRegion = true;
                //        }
                //    }
                //}


                if (IsRegion(Line))  // beginning of region
                {
                    if (RegionCounter < 1) { ThisRegionName = RegionName(Line); }

                    if (ExtractRegionName == ThisRegionName || RegionCounter > 0)
                    {
                        RegionStart = true;
                        RegionCounter++;
                        InsideRegion = true;
                    }
                }



                bool EndRegionLine = false;

                if (!InFunction) { EndRegionLine = IsEndRegion(Line); }  // check for end of region if not inside function


                if (InsideRegion)
                {
                    if (EndRegionLine)  // end of region
                    {
                        RegionCounter--;

                        ////if (RegionCounter == 1)
                        //if (RegionCounter > 0)
                        //{
                        //    InsideRegion = true;
                        //}


                        if (RegionCounter == 0)
                        {
                            InsideRegion = false;
                            continue;
                            //if (ExtractRegionName == ThisRegionName)
                            //{
                            //return RegionCodeSection;
                            //}
                        }
                    }

                }


                if (InsideRegion)
                {
                    //if (RegionCodeSection != "")
                    //{
                    //    if (RegionCounter == 0) { if (IsEndRegion(Line)) { continue; } }

                    //    //if (RegionCounter > 0)
                    //    //{
                    //    //    RegionCodeSection = RegionCodeSection + Environment.NewLine + Line;
                    //    //}
                    //}

                    if (RegionCodeSection == "")
                    {
                        //if (RegionCounter == 1)  // skip writing the first region text to output
                        if (RegionCounter > 0)
                        {
                            if (IsRegion(Line))
                            {
                                continue;
                            }

                            RegionCodeSection = Line;
                        }
                    }
                }


                if (!InsideRegion)
                {
                    if (CodeWithOutRegion != "") { CodeWithOutRegion = CodeWithOutRegion + Environment.NewLine + Line; }
                    if (CodeWithOutRegion == "") { CodeWithOutRegion = Line; }
                }


            }


            return CodeWithOutRegion;
        }


        /// <summary>
        /// returns project code without ANY regions (from .cs file)
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <param name="RemoveExtraBlanks"></param>
        /// <returns></returns>
        public string Remove_All_Regions_FromFile(string ProjectFile, bool RemoveExtraBlanks = true) 
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return ""; }
            string CodeString = ahk.FileRead(ProjectFile);

            string WithOutRegions = Remove_All_Regions_FromCode(CodeString, RemoveExtraBlanks);

            return WithOutRegions;
        }

        /// <summary>
        /// returns project code without ANY regions (from .cs code)
        /// </summary>
        /// <param name="CodeString"></param>
        /// <param name="RemoveExtraBlanks"></param>
        /// <returns></returns>
        public string Remove_All_Regions_FromCode(string CodeString, bool RemoveExtraBlanks = true)  
        {
            List<string> RegionList = RegionList_FromCode(CodeString);

            string WithOutRegions = CodeString;

            foreach (string region in RegionList)
            {
                WithOutRegions = Remove_Region_FromCode(WithOutRegions, region);
            }

            if (RemoveExtraBlanks)
            {
                WithOutRegions = Remove_Extra_Blank_Lines_FromCode(WithOutRegions, 2);  // remove extra lines
            }

            return WithOutRegions;
        }



        //### Region Info ###

        public bool IsRegion(string Line)  // check to see if this is the beginning of a Region section 
        {
            // extract first word from string
            string FirstWord = "";
            string[] words = Line.Split(' ');
            foreach (string word in words)
            {
                FirstWord = word.Trim();
                if (FirstWord != "") { break; }
            }

            if (Line.Contains("region"))
            {
                ahk.Sleep(1);
            }

            if (FirstWord.Length > 6)
            {
                string FirstChars = ahk.FirstCharacters(FirstWord, 7);
                if (FirstChars == "#region")
                {
                    if (Line.Contains("=== Error Handling ===")) { return false; }  // skip error handling regions

                    return true;  // is a region line
                }
            }

            return false;
        }

        /// <summary>
        /// check to see if this is the beginning of a Region section
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public bool IsEndRegion(string Line)   
        {
            string FirstWord = ahk.WordNum(Line, 1);  // return specific word # from string

            if (FirstWord.Length > 9)
            {
                string FirstChars = ahk.FirstCharacters(FirstWord, 10);
                if (FirstChars == "#endregion") { return true; }
            }

            return false;
        }

        /// <summary>
        /// returns name of function from (IsRegion = true) line
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public string RegionName(string Line)  
        {
            string ReturnText = ahk.StringReplace(Line, "#region");
            ReturnText = ReturnText.Trim();
            return ReturnText;
        }

        // check if region name found in code / file

        /// <summary>
        /// returns True if RegionName is found in ProjectFile
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <param name="RegionName"></param>
        /// <returns></returns>
        public bool Region_In_File(string ProjectFile, string RegionName)  
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return false; }

            string ParseCode = ahk.FileRead(ProjectFile);   // read file if path is passed in 

            return Region_In_Code(ParseCode, RegionName);
        }

        /// <summary>
        /// returns True if RegionName is found in CodeString
        /// </summary>
        /// <param name="CodeString"></param>
        /// <param name="RegionName"></param>
        /// <returns></returns>
        public bool Region_In_Code(string CodeString, string RegionName)  
        {
            List<string> regions = RegionList_FromCode(CodeString);

            RegionName = ahk.StringReplace(RegionName, "#region");  //remove #region from user input if passed in, list already has this removed

            foreach (string region in regions)
            {
                if (region.Trim() == RegionName.Trim()) { return true; }
            }

            return false;
        }



        //### Number of Regions ###

        /// <summary>
        /// Returns # of Regions Found in Text/File
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <returns></returns>
        public int RegionCount_FromFile(string ProjectFile) 
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return 0; }

            string ParseCode = ahk.FileRead(ProjectFile);   // read file if path is passed in 

            int RegionCount = RegionCount_FromCode(ParseCode);

            return RegionCount;
        }

        /// <summary>
        /// Returns # of Regions Found in Text/File
        /// </summary>
        /// <param name="CodeString"></param>
        /// <returns></returns>
        public int RegionCount_FromCode(string CodeString)  
        {
            List<string> LineList = ahk.LineList(CodeString);

            int RegionCount = 0;

            foreach (string Line in LineList)
            {
                bool Found = IsRegion(Line);
                if (Found) { RegionCount++; }
            }

            return RegionCount;
        }

        /// <summary>
        /// Returns # of Regions Found inside another region (from file)
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <param name="RegionName"></param>
        /// <returns></returns>
        public int SubRegionCount_FromFile(string ProjectFile, string RegionName)  
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return 0; }
            string ParseCode = ahk.FileRead(ProjectFile);   // read file if path is passed in 


            int RegionCount = SubRegionCount_FromCode(ParseCode, RegionName);
            return RegionCount;
        }

        /// <summary>
        /// Returns # of Regions Found inside another region (from code)
        /// </summary>
        /// <param name="CodeString"></param>
        /// <param name="RegionName"></param>
        /// <returns></returns>
        public int SubRegionCount_FromCode(string CodeString, string RegionName)  
        {
            // extract code from specific region
            string RegionCode = Extract_Region_FromCode(CodeString, RegionName);

            List<string> LineList = lst.Text_To_List(RegionCode, true, false, true);

            int RegionCount = 0;

            // count number of sub regions
            foreach (string Line in LineList)
            {
                bool Found = IsRegion(Line);
                if (Found) { RegionCount++; }
            }

            return RegionCount;
        }





        #endregion

        #region === Parse: Constants ===

        /// <summary>
        /// checks to see if line of code is a constant declaration
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public bool IsConstant(string Line)  
        {
            //private const uint FILE_ATTRIBUTE_NORMAL = 0x80;

            string Const = ahk.WordNum(Line, 1);
            if (Const == "const") { return true; }

            Const = ahk.WordNum(Line, 2);
            if (Const == "const") { return true; }

            Const = ahk.WordNum(Line, 3);
            if (Const == "const") { return true; }

            return false;
        }


        #endregion

        #region === Parse: Documentation ===

        //string code = @"/// <summary>Makes a variety of changes to a control.</summary>" + Environment.NewLine + "/// <param name=\"Options\">Check|Uncheck|Enable|Disable|Show|Hide|Style,N|ExStyle,N|ShowDropDown|HideDropDown|TabLeft,Count|TabRight,Count|Add,String|Delete,N|Choose,N|EditPaste,String</param>";

        /// <summary>
        /// checks to see if line of code is a documentation declaration (starts with ///)
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public bool IsDoc(string Line)  
        {
            string FirstWord = ahk.FirstWord(Line);

            if (FirstWord.Contains("///")) { return true; }
            //if (FirstWord == "///") { return true; } // v1  works mostly ? 
            return false;
        }

        /// <summary>
        /// checks to see if line of code is a <Param> declaration
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public bool IsParam(string Line) 
        {
            if (Line.Contains("<param name="))
                return true;

            return false;
        }
        public string Extract_ParamName(string Code)
        {
            string between = ahk.StringSplit(Code, "\"", 1);
            between = ahk.StringSplit(between, "\"", 0);
            return between;
        }
        public string Extract_ParamText(string Code, string paramName)
        {
            string paramStart = "/// <param name=\"" + paramName + "\">";
            string paramText = ahk.StringReplace(Code, paramStart);
            paramText = ahk.StringReplace(paramText, "</param>");
            return paramText;
        }

        /// <summary>
        /// checks to see if line of code is a <summary> declaration
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public bool IsSummary(string Line) 
        {
            if (Line.Contains("<summary>"))
                return true;

            return false;
        }
        public string Extract_Summary(string Code)
        {
            string Summary = Extract_Tag(Code, "summary");
            return Summary;
        }

        /// <summary>
        /// checks to see if line of code is a <return> declaration
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public bool IsReturn(string Line) 
        {
            if (Line.Contains("<return>"))
                return true;

            return false;
        }
        public string Extract_Return(string Code)
        {
            string Summary = Extract_Tag(Code, "return");
            return Summary;
        }


        string Extract_Tag(string s, string tag)
        {
            // You should check for errors in real-world code, omitted for brevity
            var startTag = "<" + tag + ">";
            int startIndex = s.IndexOf(startTag) + startTag.Length;
            int endIndex = s.IndexOf("</" + tag + ">", startIndex);

            if (endIndex <= 1) { return ""; }

            return s.Substring(startIndex, endIndex - startIndex);
        }

        #endregion

        #region === Parse: Classes ===

        /// <summary>
        /// checks to see if line of code is a class declaration
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public bool IsClass(string Line) 
        {
            string FirstWord = ahk.WordNum(Line, 1);  // return specific word # from string

            bool MaybeClass = false;

            //if (FirstWord == "public" || FirstWord == "private" || FirstWord == "class") { MaybeClass = true; }

            List<string> WordList = ahk.WordList(Line);
            foreach (string Word in WordList)
            {
                if (Word == "class") { MaybeClass = true; }
            }


            return MaybeClass;

        }

        /// <summary>
        /// returns the class name from a (IsClass = true) line of code
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public string ClassName(string Line)  
        {
            string ReturnText = ahk.StringReplace(Line, "class");
            ReturnText = ahk.StringReplace(ReturnText, "public");
            ReturnText = ahk.StringReplace(ReturnText, "private");
            ReturnText = ReturnText.Trim();
            return ReturnText;
        }

        #endregion

        #region === Parse: NameSpaces ===

        /// <summary>
        /// returns NameSpace Name from IsNameSpace line
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public string NameSpaceName(string Line)  
        {
            string NS = ahk.StringReplace(Line, "namespace");
            NS = NS.Trim();
            return NS;
        }

        /// <summary>
        /// check to see if this is a "using" declaration line
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public bool IsNameSpace(string Line)  
        {
            string FirstWord = ahk.WordNum(Line, 1);  // return specific word # from string
            if (FirstWord == "namespace") { return true; }
            return false;
        }


        #endregion

        #region === Parse: Using ===

        /// <summary>
        /// check to see if this is a "using" declaration line
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public bool IsUsing(string Line)  
        {
            Line = Line.Trim();

            string SecondWord = ahk.WordNum(Line, 2);  // return specific word # from string

            if (ahk.FirstCharacters(SecondWord, 1) == "(")
            {
                return false;
            }

            string FirstWord = ahk.WordNum(Line, 1);  // return specific word # from string
            if (FirstWord == "using") { return true; }
            return false;
        }

        /// <summary>
        /// returns the "using" name from a Using Line (minus the word using and ;)
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public string UsingName(string Line)  
        {
            Line = ahk.RemoveComments(Line);
            Line = ahk.StringReplace(Line, "using");
            Line = ahk.StringReplace(Line, ";");
            Line = Line.Trim();

            return Line;
        }

        /// <summary>
        /// return comments on using declaration line
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public string UsingComments(string Line)  
        {
            string comments = ahk.ReturnComments(Line);

            return comments;
        }


        /// <summary>
        /// return list of Using declarations from code file/text
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <returns></returns>
        public List<string> Using_List_FromFile(string ProjectFile) 
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return null; }
            string ParseCode = ahk.FileRead(ProjectFile);   // read file if path is passed in 

            List<string> UsingLines = Using_List_FromCode(ParseCode);

            return UsingLines;
        }

        /// <summary>
        /// return list of Using declarations from code file/text
        /// </summary>
        /// <param name="CodeString"></param>
        /// <returns></returns>
        public List<string> Using_List_FromCode(string CodeString)  
        {
            List<string> Lines = lst.Text_To_List(CodeString);
            List<string> UsingLines = new List<string>();

            foreach (string line in Lines)
            {
                bool UsingLine = IsUsing(line);

                if (UsingLine) { UsingLines.Add(line); }
            }

            return UsingLines;
        }


        /// <summary>
        /// return last using reference line at top of code file
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <returns></returns>
        public string Last_Using_Reference_FromFile(string ProjectFile)  
        {
            if (!File.Exists(ProjectFile)) { ahk.MsgBox(ProjectFile + " Not Found"); return ""; }
            string ParseCode = ahk.FileRead(ProjectFile);   // read file if path is passed in 

            string LastUsingLine = Last_Using_Reference_FromCode(ParseCode);

            return LastUsingLine;
        }

        /// <summary>
        /// return last using reference line at top of code file
        /// </summary>
        /// <param name="CodeString"></param>
        /// <returns></returns>
        public string Last_Using_Reference_FromCode(string CodeString)  
        {
            List<string> Lines = lst.Text_To_List(CodeString);
            string LastUsingLine = "";

            foreach (string line in Lines)
            {
                bool UsingLine = IsUsing(line);

                if (UsingLine) { LastUsingLine = line; }
            }

            return LastUsingLine;
        }


        #endregion

        #region === Parse: Comments ===

        /// <summary>
        /// check to see if line start with // for comment only line
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public bool IsCommentLine(string Line) 
        {
            string FirstWord = ahk.WordNum(Line, 1);  // return specific word # from string

            string FirstTwo = ahk.FirstCharacters(FirstWord, 2);

            if (FirstTwo == "//")
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// return line of code with the comments parsed out
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public string WithoutComments(string Line)  
        {
            string comments = ahk.RemoveComments(Line);

            return comments;
        }

        /// <summary>
        /// return line of code with the comments parsed out
        /// </summary>
        /// <param name="Line"></param>
        /// <returns></returns>
        public string OnlyComments(string Line) 
        {
            string comments = ahk.ReturnComments(Line);

            return comments;
        }

        #endregion

        #region === Parce: C# To SQLite ===


        // parse / write SQL / SQLite --> (v3)

        public bool CSharpList_To_SQL(List<string> CodeFilePathList, SqlConnection Conn = null, string TableName = "[codeserver].[lucidmethod].[FunctionLib]")
        {
            if (Conn == null) { Conn = sql.GetConn("SQLserver"); }  // return connection from app.config

            foreach (string file in CodeFilePathList)
            {
                List<sharpFunctions> functions = ParseFunctionLib(file);

            }


            return false;
        }

        public bool CSharp_To_SQL(string CodeFilePath, SqlConnection Conn = null, string TableName = "[codeserver].[lucidmethod].[FunctionLib]")
        {
            if (Conn == null) { Conn = sql.GetConn("SQLserver"); }  // return connection from app.config

            List<sharpFunctions> functions = ParseFunctionLib(CodeFilePath);

            ahk.MsgBox(functions.Count() + " Functions Found");

            return false;
        }

        public bool CSharp_To_SQLite(List<string> CodeFilePathList, string DbFile = "", string TableName = "FunctionLib")
        {

            //string DbFile = "";
            //bool ReplaceOld = true;


            ////sqlite.CreateUserDb(DbFile, TableName);
            //Create_Parse_To_SQLite_Tables(DbFile, TableName);
            //if (ReplaceOld) { sqlite.Execute(DbFile, "Delete From " + TableName); } // clear out existing entries (used while testing)


            //string InsertLine = "INSERT into " + TableName + " (FileName, NameSpace, Class, Function, Region, LineNum, Tags, Comments, TimeStamp, FunctionLine, FunctionText, FilePath, DLLVer, DateAdded, UsesErrorLevel, Tested, ToDo, Documentation, Example) VALUES ('" + func.FileName + "', '" + func.NameSpace + "', '" + func.Class + "', '" + func.Function + "', '" + func.Region + "', '" + func.LineNum + "', '" + func.Tags + "', '" + func.Comments + "', '" + func.TimeStamp + "', '" + func.FunctionLine + "', '" + func.FunctionText + "', '" + func.FilePath + "', '" + func.DLLVer + "', '" + func.DateAdded + "', '" + func.UsesErrorLevel + "', '" + func.Tested + "', '" + func.ToDo + "','" + func.Documentation + "', '" + func.Example + "')";

            //// update by region and function name
            //string UpdateLine = "UPDATE " + TableName + " set LineNum = '" + LineCount.ToString() + "', Comments = '" + Summary + "', TimeStamp = '" + DateTime.Now + "', FunctionLine = '" + CodeReturn + "', FunctionText = '" + FunctionText + "', FilePath = '" + FilePath + "', DLLVer = '" + DLLVer + "', Example = '" + Example + "' WHERE Function = '" + FunctionName + "' AND Region = '" + REgionName + "'";
            ////string UpdateLine = "UPDATE " + TableName + " set LineNum = '" + LineCount.ToString() + "', Comments = '" + Summary + "', TimeStamp = '" + DateTime.Now + "', Region = '" + REgionName + "', FunctionLine = '" + CodeReturn + "', FunctionText = '" + FunctionText + "', FilePath = '" + FilePath + "', DLlVer = '" + DLLVer + "', Example = '" + Example + "' WHERE Function = '" + FunctionName + "' AND Region = '" + REgionName + "'";


            //if (Write_To_GoDaddy_SQL)
            //{
            //    _Database.goDaddy goDad = new _Database.goDaddy();
            //    SqlConnection goDadCon = sqlite.GoDaddyConnection();
            //    goDad.Update_Insert_SQLFunctionLib(goDadCon, func);
            //}

            //// add to list of sqlite command lines to execute in batch 
            //InsertList.Add(InsertLine);



            //// Replace to insert SQLITE !!!       
            //sqlite.Batch_Insert(DbFile, InsertList);



            return false;
        }


        //public List<sharpFunctions> currentFunctionLib;

        public sharpFunctions ReturnFunction_FromMemory(List<sharpFunctions> functionLib, string FunctionName)
        {
            foreach (sharpFunctions func in functionLib)
            {
                if (func.FunctionName == FunctionName) { return func; }
            }

            return new sharpFunctions();
        }


        public List<sharpFunctions> ParseFunctionLib(string CodeFilePath)
        {
            List<sharpFunctions> functionList = new List<sharpFunctions>();

            string FileName = Path.GetFileName(CodeFilePath);

            string Code = ahk.FileRead(CodeFilePath);  // code to parse


            int LineCount = 0;
            bool NameSpaceFound = false;
            string NameSpaceCode = "";

            //=================================================
            // Extract NameSpace Name and Code UnderNeath
            //=================================================

            string ClassName = "";
            string FunctionName = "";
            string NameSpaceName = "";
            string[] lines = Code.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            string REgionName = "";

            //SQLiteConnection conn = sqlite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

            int FID = 0;
            //if (BulkInsert)
            //{
            //    if (conn.State == ConnectionState.Closed) { conn.Open(); } //open SQLite connection
            //}

            string Summary = "";
            string ParamName = "";
            string ParamText = "";
            string ReturnTag = "";
            string Documentation = "";

            List<string> InsertList = new List<string>();

            foreach (string line in lines)
            {
                LineCount++;

                //=== parse line of C# code, returns Code as output and returns comment in the out field ===
                string CommentOut = "";
                string LineType = "";
                string CodeReturn = ParseCodeLine(line, out CommentOut, out LineType);
                //ahk.MsgBox(CodeReturn);
                //ahk.MsgBox(CommentOut);

                bool IsDocLine = IsDoc(line);
                if (IsDocLine)
                {
                    if (Documentation != "") { Documentation = Documentation + Environment.NewLine + line; }
                    if (Documentation == "") { Documentation = line; }

                    bool ISSummary = IsSummary(line);
                    if (ISSummary)
                    {
                        Summary = Extract_Summary(line);
                    }

                    if (IsParam(line))
                    {
                        ParamName = Extract_ParamName(line);
                        ParamText = Extract_ParamText(line, ParamName);
                    }

                    if (IsReturn(line))
                    {
                        ReturnTag = Extract_Return(line);
                    }
                }


                if (LineType == "dll call") { continue; }

                if (LineType == "constant") { continue; }

                if (LineType == "namespace")
                {
                    NameSpaceName = CodeReturn.Trim();
                    //ahk.MsgBox("NameSpace: " + CodeReturn + " (" + LineCount.ToString() + ")");
                    NameSpaceFound = true;
                }

                if (LineType == "class")
                {
                    ClassName = CodeReturn;
                    string LineTypeOut;
                    ClassName = ParseCodeLine(ClassName, out CommentOut, out LineTypeOut);  //separate the comments out
                    //ahk.MsgBox("Class: " + ClassName + " (" + LineCount.ToString() + ")");
                }

                if (IsRegion(line)) { REgionName = RegionName(line); }

                if (LineType != "class" && LineType != "namespace" && LineType != "")
                {
                    string FunctionLine = CodeReturn;
                    FunctionName = ParseFunctionLine(FunctionLine);
                    //ahk.MsgBox("NameSpace: " + NameSpaceName + Environment.NewLine + "Class: " + ClassName + Environment.NewLine + "Function: " + FunctionName + " (" + LineCount.ToString() + ")");

                    ClassName = ClassName.Trim();

                    //=============================================================
                    // UPDATE or INSERT 
                    //=============================================================
                    string Comments = ahk.FixSpecialChars(CommentOut);
                    string Tags = "";
                    bool UsesEL = false;
                    string functionTEXT = Return_Function_FromCode(Code, FunctionName);

                    //functionTEXT = ahk.StringReplace(functionTEXT, "',", "'',", "ALL");
                    //functionTEXT = ahk.StringReplace(functionTEXT, "%", "'%", "ALL");

                    if (functionTEXT.Contains("ErrorLog_Setup("))
                    {
                        //if (ahk.IfInString("ErrorLog_Setup(false)", functionTEXT))
                        if (functionTEXT.Contains("ErrorLog_Setup(false)"))
                            UsesEL = false;

                        if (functionTEXT.Contains("ErrorLog_Setup(true)"))
                            UsesEL = true;
                    }

                    Tags = REgionName.Replace("=", "");
                    Tags = Tags.Trim();

                    if (Tags.Contains("##"))
                    {
                        Tags = Tags.Replace("#", "");
                        Tags = Tags.Trim();
                        Tags = "Variables: " + Tags;
                    }

                    //Tags = Tags + ", " + TagClassName;

                    CodeReturn = RemoveCommentsFromLine(CodeReturn);

                    string FilePath = CodeFilePath;
                    string Parent = "";
                    string DLLVer = "1.0";

                    string Example = Return_Example_Code(FunctionName.Trim());

                    FID++;
                    Documentation = FixInsertString(Documentation);
                    functionTEXT = FixInsertString(functionTEXT);
                    FunctionLine = FixInsertString(FunctionLine);
                    Summary = FixInsertString(Summary);
                    Tags = Tags.Replace("=", "");
                    Tags = Tags.Trim();

                    if (Tags.Contains("###"))
                    {
                        Tags = Tags.Replace("#", "");
                        Tags = Tags.Replace(" and", "");
                        Tags = Tags.Trim();
                        Tags = "Variables, " + Tags;
                    }

                    if (REgionName.Contains("###"))
                    {
                        REgionName = REgionName.Replace("#", "");
                        REgionName = REgionName.Trim();
                        REgionName = "Variables : " + REgionName;
                    }

                    REgionName = ahk.StringReplace(REgionName, "=", "", "ALL");
                    REgionName = REgionName.Trim();

                    Tags = Tags.Replace(":", ",");
                    Tags = Tags.Trim();

                    sharpFunctions func = new sharpFunctions();

                    func.FileName = FileName;
                    func.FunctionName = FunctionName;
                    func.NameSpace = NameSpaceName;
                    func.Class = ClassName;
                    func.RegionName = REgionName;
                    func.LineNum = LineCount.ToString();
                    func.Tags = Tags;
                    func.Comments = Summary;
                    func.FunctionLine = CodeReturn;
                    func.FunctionCode = functionTEXT;
                    func.FilePath = FilePath;
                    func.DLLVer = DLLVer;
                    func.Documentation = Documentation;
                    func.Examples = Example;
                    func.UsesErrorLevel = UsesEL;
                    func.Tested = false;
                    func.ToDo = "";

                    functionList.Add(func);   // add parsed function to return list

                    Summary = "";
                    Documentation = "";
                }
            }


            return functionList;
        }


        public struct sharpFunctions
        {
            public string ID { get; set; }
            public string Flagged { get; set; }
            public string FileName { get; set; }
            public string NameSpace { get; set; }
            public string Class { get; set; }
            public string FunctionName { get; set; }
            public string RegionName { get; set; }
            public string LineNum { get; set; }
            public string Tags { get; set; }
            public string Comments { get; set; }
            public string FunctionLine { get; set; }
            public string FunctionCode { get; set; }
            public string FilePath { get; set; }
            public string DLLVer { get; set; }
            public string Examples { get; set; }
            public string Documentation { get; set; }
            public DateTime DateAdded { get; set; }
            public DateTime DateModified { get; set; }
            public bool UsesErrorLevel { get; set; }
            public bool Tested { get; set; }
            public string ToDo { get; set; }
        }


        /// <summary> </summary>
        /// <param name="FunctionLib obj"> </param>
        /// <param name="OnlyInsert"> </param>
        public void Update_Insert_FunctionLib(sharpFunctions obj, SqlConnection Conn = null, string TableName = "[codeserver].[lucidmethod].[FunctionLib]", bool OnlyInsert = false)
        {
            // initialize var names as blank, if not null in object, assign that value
            string ID = ""; if (obj.ID != null) { ID = obj.ID.ToString(); }
            string FunctionName = ""; if (obj.FunctionName != null) { FunctionName = obj.FunctionName.ToString(); }
            string FunctionLine = ""; if (obj.FunctionLine != null) { FunctionLine = obj.FunctionLine.ToString(); }
            string FunctionCode = ""; if (obj.FunctionCode != null) { FunctionCode = obj.FunctionCode.ToString(); }
            string RegionName = ""; if (obj.RegionName != null) { RegionName = obj.RegionName.ToString(); }
            string FileName = ""; if (obj.FileName != null) { FileName = obj.FileName.ToString(); }
            string Documentation = ""; if (obj.Documentation != null) { Documentation = obj.Documentation.ToString(); }
            string Examples = ""; if (obj.Examples != null) { Examples = obj.Examples.ToString(); }
            string Tags = ""; if (obj.Tags != null) { Tags = obj.Tags.ToString(); }
            //string Links = ""; if (obj.Links != null) { Links = obj.Links.ToString(); }
            //string ICO_Name = ""; if (obj.ICO_Name != null) { ICO_Name = obj.ICO_Name.ToString(); }
            //string ICO_Img = ""; if (obj.ICO_Img != null) { ICO_Img = obj.ICO_Img.ToString(); }
            obj.DateAdded = DateTime.Now;
            obj.DateModified = DateTime.Now;

            string Flagged = ""; if (obj.Flagged != null) { Flagged = obj.Flagged.ToString(); }

            // Attempt to UPDATE Values into SQL Table (by FID)

            int recordsAffected = 0;

            if (!OnlyInsert)
            {
                string SQLLine = ""; // FIX LATER !!!

                //string SQLLine = "UPDATE " + TableName + " SET FunctionName= '" + FunctionName + "', FunctionLine= '" + FunctionLine + "', FunctionCode= '" + FunctionCode + "', RegionName= '" + RegionName + "', FileName= '" + FileName + "', Documentation= '" + Documentation + "', Examples= '" + Examples + "', Tags= '" + Tags + "', Links= '" + Links + "', ICO_Name= '" + ICO_Name + "', ICO_Img= '" + ICO_Img + "', DateAdded= '" + DateAdded + "', DateModified= '" + DateModified + "', Flagged= '" + Flagged + "', Released= '" + Released + "' WHERE ID = '" + FID + "'";

                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }

                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { MessageBox.Show(ex.ToString()); }

                Conn.Close();
            }


            // if the Update doesn't change any rows, need to insert new entry
            if (recordsAffected == 0)
            {
                // INSERT Values into SQL Table
                string SQLcommand = "INSERT INTO " + TableName + " (FunctionName,FunctionLine,FunctionCode,RegionName,FileName,Documentation,Examples,Tags,Links,ICO_Name,ICO_Img,DateAdded,DateModified,Flagged,Released) VALUES(@FunctionName, @FunctionLine, @FunctionCode, @RegionName, @FileName, @Documentation, @Examples, @Tags, @Links, @ICO_Name, @ICO_Img, @DateAdded, @DateModified, @Flagged, @Released)";
                SqlCommand cmd1 = new SqlCommand(SQLcommand, Conn);

                cmd1.Parameters.AddWithValue(@"FunctionName", obj.FunctionName);
                cmd1.Parameters.AddWithValue(@"FunctionLine", obj.FunctionLine);
                cmd1.Parameters.AddWithValue(@"FunctionCode", obj.FunctionCode);
                cmd1.Parameters.AddWithValue(@"RegionName", obj.RegionName);
                cmd1.Parameters.AddWithValue(@"FileName", obj.FileName);
                cmd1.Parameters.AddWithValue(@"Documentation", obj.Documentation);
                cmd1.Parameters.AddWithValue(@"Examples", Examples);
                cmd1.Parameters.AddWithValue(@"Tags", Tags);
                //cmd1.Parameters.AddWithValue(@"Links", Links);
                //cmd1.Parameters.AddWithValue(@"ICO_Name", ICO_Name);
                //cmd1.Parameters.AddWithValue(@"ICO_Img", ICO_Img);
                cmd1.Parameters.AddWithValue(@"DateAdded", DateTime.Now);
                cmd1.Parameters.AddWithValue(@"DateModified", DateTime.Now);
                cmd1.Parameters.AddWithValue(@"Flagged", Flagged);
                //cmd1.Parameters.AddWithValue(@"Released", Released);

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }

                try { cmd1.ExecuteNonQuery(); }
                catch (SqlException ex) { MessageBox.Show(ex.ToString()); }
            }
        }




        //#########################################################################

        // parse / write sqlite - new documentation parse (v2)

        public string Parse_To_SQLite(string DbFile, string CodePath, string TableName = "FunctionLib", bool BulkInsert = true, bool ReplaceOld = false, bool Write_To_GoDaddy_SQL = false)  // Parses C# File, Loading Function Names / Info into SQLite Table
        {
            // development option to write to godaddy sql database while parsing/updating

            //string CodeFile = @"C:\Users\jason\Google Drive\AHK\IMDB\SQLiter\AHK\AHK.cs";

            int ResumeLineCount = 3693;  // enter last captured line number to restart process there  -- 0 otherwise

            BulkInsert = false;

            string FileName = Path.GetFileName(CodePath);

            //sqlite.CreateUserDb(DbFile, TableName);
            Create_Parse_To_SQLite_Tables(DbFile, TableName);

            if (ReplaceOld) { sqlite.Execute(DbFile, "Delete From " + TableName); } // clear out existing entries (used while testing)

            string Code = ahk.FileRead(CodePath);  // code to parse


            int LineCount = 0;
            bool NameSpaceFound = false;
            string NameSpaceCode = "";

            //=================================================
            // Extract NameSpace Name and Code UnderNeath
            //=================================================

            string ClassName = "";
            string FunctionName = "";
            string NameSpaceName = "";
            string[] lines = Code.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            string REgionName = "";

            //SQLiteConnection conn = sqlite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

            int FID = 0;
            //if (BulkInsert)
            //{
            //    if (conn.State == ConnectionState.Closed) { conn.Open(); } //open SQLite connection
            //}

            string Summary = "";
            string ParamName = "";
            string ParamText = "";
            string ReturnTag = "";
            string Documentation = "";

            List<string> InsertList = new List<string>();

            int i = 0; int k = 0;
            foreach (string line in lines)
            {
                LineCount++;

                if (LineCount < ResumeLineCount) { continue; }

                //=== parse line of C# code, returns Code as output and returns comment in the out field ===
                string CommentOut = "";
                string LineType = "";
                string CodeReturn = ParseCodeLine(line, out CommentOut, out LineType);
                //ahk.MsgBox(CodeReturn);
                //ahk.MsgBox(CommentOut);

                bool IsDocLine = IsDoc(line);
                if (IsDocLine)
                {
                    if (Documentation != "") { Documentation = Documentation + Environment.NewLine + line; }
                    if (Documentation == "") { Documentation = line; }

                    bool ISSummary = IsSummary(line);
                    if (ISSummary)
                    {
                        Summary = Extract_Summary(line);
                    }

                    if (IsParam(line))
                    {
                        ParamName = Extract_ParamName(line);
                        ParamText = Extract_ParamText(line, ParamName);
                    }

                    if (IsReturn(line))
                    {
                        ReturnTag = Extract_Return(line);
                    }
                }


                if (LineType == "dll call") { continue; }

                if (LineType == "constant") { continue; }

                if (LineType == "namespace")
                {
                    NameSpaceName = CodeReturn.Trim();
                    //ahk.MsgBox("NameSpace: " + CodeReturn + " (" + LineCount.ToString() + ")");
                    NameSpaceFound = true;
                }

                if (LineType == "class")
                {
                    ClassName = CodeReturn;
                    string LineTypeOut;
                    ClassName = ParseCodeLine(ClassName, out CommentOut, out LineTypeOut);  //separate the comments out
                    //ahk.MsgBox("Class: " + ClassName + " (" + LineCount.ToString() + ")");
                }

                if (IsRegion(line)) { REgionName = RegionName(line); }

                if (LineType != "class" && LineType != "namespace" && LineType != "")
                {
                    string FunctionLine = CodeReturn;
                    FunctionName = ParseFunctionLine(FunctionLine);
                    //ahk.MsgBox("NameSpace: " + NameSpaceName + Environment.NewLine + "Class: " + ClassName + Environment.NewLine + "Function: " + FunctionName + " (" + LineCount.ToString() + ")");

                    ClassName = ClassName.Trim();

                    //=============================================================
                    // UPDATE or INSERT 
                    //=============================================================
                    string Comments = ahk.FixSpecialChars(CommentOut);
                    string Tags = "";

                    if (FileName == "sharpAHK.cs" || FileName == "sharpDev.cs")
                    {
                        Tags = FileName.Replace(".cs", ".dll");
                    }

                    //string TagClassName = ClassName;
                    //if (TagClassName == "SQLITE") { TagClassName = "SQLite"; }
                    //if (TagClassName == "ftp") { TagClassName = "FTP"; }
                    //if (TagClassName == "TreeViewControl") { TagClassName = "TreeView"; }
                    //if (TagClassName == "Grid") { TagClassName = "DataGridview"; }
                    //if (TagClassName == "IniFile") { TagClassName = "INI"; }

                    string UsesEL = "-1";

                    string functionTEXT = Return_Function_FromCode(Code, FunctionName);

                    //functionTEXT = ahk.StringReplace(functionTEXT, "',", "'',", "ALL");
                    //functionTEXT = ahk.StringReplace(functionTEXT, "%", "'%", "ALL");

                    if (functionTEXT.Contains("ErrorLog_Setup("))
                    {
                        //if (ahk.IfInString("ErrorLog_Setup(false)", functionTEXT))
                        if (functionTEXT.Contains("ErrorLog_Setup(false)"))
                            UsesEL = "0";

                        if (functionTEXT.Contains("ErrorLog_Setup(true)"))
                            UsesEL = "1";
                    }

                    Tags = REgionName.Replace("=", "");
                    Tags = Tags.Trim();

                    if (Tags.Contains("##"))
                    {
                        Tags = Tags.Replace("#", "");
                        Tags = Tags.Trim();
                        Tags = "Variables: " + Tags;
                    }

                    //Tags = Tags + ", " + TagClassName;

                    CodeReturn = RemoveCommentsFromLine(CodeReturn);

                    string FunctionText = Return_Function_FromCode(Code, FunctionName);
                    string FilePath = CodePath;
                    string Parent = "";
                    string DLLVer = "1.0";


                    // removed for speed ! 
                    //string Example = Return_Example_Code(FunctionName.Trim());
                    string Example = "";

                    FID++;
                    //FunctionText = ahk.StringReplace(FunctionText, @"'", @"''");
                    //FunctionText = "";
                    //Documentation = "";
                    //Summary = ahk.StringReplace(Summary, @"'", "\'");
                    Documentation = FixInsertString(Documentation);
                    FunctionText = FixInsertString(FunctionText);
                    FunctionLine = FixInsertString(FunctionLine);
                    Summary = FixInsertString(Summary);
                    Tags = Tags.Replace("=", "");
                    Tags = Tags.Trim();

                    if (Tags.Contains("###"))
                    {
                        Tags = Tags.Replace("#", "");
                        Tags = Tags.Replace(" and", "");
                        Tags = Tags.Trim();
                        Tags = "Variables, " + Tags;
                    }

                    if (REgionName.Contains("###"))
                    {
                        REgionName = REgionName.Replace("#", "");
                        REgionName = REgionName.Trim();
                        REgionName = "Variables : " + REgionName;
                    }

                    REgionName = REgionName.Replace("=", "");
                    REgionName = REgionName.Trim();

                    Tags = Tags.Replace(":", ",");
                    Tags = Tags.Trim();

                    _Database.goDaddy.functionLib func = new _Database.goDaddy.functionLib();

                    func.FileName = FileName;
                    func.Function = FunctionName;
                    func.NameSpace = NameSpaceName;
                    func.Class = ClassName;
                    func.Region = REgionName;
                    func.LineNum = LineCount.ToString();
                    func.Tags = Tags;
                    func.Comments = Summary;
                    func.TimeStamp = DateTime.Now.ToString();
                    func.FunctionLine = CodeReturn;
                    func.FunctionText = FunctionText;
                    func.FilePath = FilePath;
                    func.DLLVer = DLLVer;
                    func.Documentation = Documentation;
                    func.Example = Example;
                    func.UsesErrorLevel = UsesEL;
                    func.Tested = "0";
                    func.ToDo = "";

                    string InsertLine = "INSERT into " + TableName + " (FileName, NameSpace, Class, Function, Region, LineNum, Tags, Comments, TimeStamp, FunctionLine, FunctionText, FilePath, DLLVer, DateAdded, UsesErrorLevel, Tested, ToDo, Documentation, Example) VALUES ('" + func.FileName + "', '" + func.NameSpace + "', '" + func.Class + "', '" + func.Function + "', '" + func.Region + "', '" + func.LineNum + "', '" + func.Tags + "', '" + func.Comments + "', '" + func.TimeStamp + "', '" + func.FunctionLine + "', '" + func.FunctionText + "', '" + func.FilePath + "', '" + func.DLLVer + "', '" + func.DateAdded + "', '" + func.UsesErrorLevel + "', '" + func.Tested + "', '" + func.ToDo + "','" + func.Documentation + "', '" + func.Example + "')";

                    // update by region and function name
                    string UpdateLine = "UPDATE " + TableName + " set LineNum = '" + LineCount.ToString() + "', Comments = '" + Summary + "', TimeStamp = '" + DateTime.Now + "', FunctionLine = '" + CodeReturn + "', FunctionText = '" + FunctionText + "', FilePath = '" + FilePath + "', DLLVer = '" + DLLVer + "', Example = '" + Example + "' WHERE Function = '" + FunctionName + "' AND Region = '" + REgionName + "'";
                    //string UpdateLine = "UPDATE " + TableName + " set LineNum = '" + LineCount.ToString() + "', Comments = '" + Summary + "', TimeStamp = '" + DateTime.Now + "', Region = '" + REgionName + "', FunctionLine = '" + CodeReturn + "', FunctionText = '" + FunctionText + "', FilePath = '" + FilePath + "', DLlVer = '" + DLLVer + "', Example = '" + Example + "' WHERE Function = '" + FunctionName + "' AND Region = '" + REgionName + "'";


                    if (Write_To_GoDaddy_SQL)
                    {
                        _Database.goDaddy goDad = new _Database.goDaddy();
                        SqlConnection goDadCon = sqlite.GoDaddyConnection();
                        goDad.Update_Insert_SQLFunctionLib(goDadCon, func);
                    }

                    // add to list of sqlite command lines to execute in batch 
                    InsertList.Add(InsertLine);

                    Summary = "";
                    Documentation = "";
                }
            }

            // Replace to insert SQLITE !!!       
            sqlite.Batch_Insert(DbFile, InsertList);

            return "";
        }


        // fix escape characters in SQLite Line before inserting/updating table
        public string FixInsertString(string SQLiteLine)
        {
            string line = SQLiteLine.Replace("'", "''");
            //line = line.Replace("%", "'%");
            return line;
        }

        public void Create_Parse_To_SQLite_Tables(string DbFile, string FunctionTableName = "FunctionLib") // create tables for cs to sqlite parse contents
        {
            // create database file if it doen't exist
            if (!File.Exists(DbFile)) { SQLiteConnection.CreateFile(DbFile); }

            // Connect to the DB
            SQLiteConnection m_dbConnection = sqlite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

            // Create New Table If It Does NOT Exist Yet
            bool TableExist = sqlite.Table_Exists(DbFile, FunctionTableName);  //See if selected Table Exists in SQLite DB file

            if (!TableExist)  // Table DOES NOT exist in SQLite DB
            {
                string NewTableLine = "";
                NewTableLine = "ID INTEGER PRIMARY KEY, Flagged BOOL, FileName VARCHAR, NameSpace VARCHAR, Class VARCHAR, Function VARCHAR, Region VARCHAR, LineNum VARCHAR, Tags VARCHAR, Comments VARCHAR, TimeStamp VARCHAR, FunctionLine VARCHAR, FunctionText VARCHAR, FilePath VARCHAR, DLLVer VARCHAR, Example VARCHAR, Documentation VARCHAR, UsedInProjects VARCHAR, DateAdded VARCHAR, UsesErrorLevel VARCHAR, Tested VARCHAR, ToDo VARCHAR";
                //ahk.MsgBox(NewTableLine); 
                bool Created = sqlite.Execute(DbFile, "CREATE TABLE [" + FunctionTableName + "] (" + NewTableLine + ")");  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
                //if (!Created) { ahk.MsgBox("Error Creating SQLite Table + " + FunctionTableName); }
            }


            // Create New Parmater Table If It Does NOT Exist Yet
            TableExist = sqlite.Table_Exists(DbFile, FunctionTableName.Trim() + "_Params");  //See if selected Table Exists in SQLite DB file

            if (!TableExist)  // Table DOES NOT exist in SQLite DB
            {
                string NewTableLine = "";
                NewTableLine = "ID INTEGER PRIMARY KEY, Function VARCHAR, Region VARCHAR, Param1Name VARCHAR, Param1Text VARCHAR, Param2Name VARCHAR, Param2Text VARCHAR, Param3Name VARCHAR, Param3Text VARCHAR, Param4Name VARCHAR, Param4Text VARCHAR, Param5Name VARCHAR, Param5Text VARCHAR, Param6Name VARCHAR, Param6Text VARCHAR, Param7Name VARCHAR, Param7Text VARCHAR, Param8Name VARCHAR, Param8Text VARCHAR, Param9Name VARCHAR, Param9Text VARCHAR, Param10Name VARCHAR, Param10Text VARCHAR";
                //ahk.MsgBox(NewTableLine); 
                bool Created = sqlite.Execute(DbFile, "CREATE TABLE [" + FunctionTableName.Trim() + "_Params] (" + NewTableLine + ")");  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
                //if (!Created) { ahk.MsgBox("Error Creating SQLite Table + " + FunctionTableName.Trim() + "_Params"); }
            }

        }

        // returns examples from string or file path (one must be provided)
        public string Return_Example_Code(string SearchFunction, string SearchFile = @"C:\Users\Jason\Google Drive\IMDB\Examples.cs", string SearchText = "")
        {
            string ExampleReturn = "";

            // code from string or file path
            string CodeString = "";
            if (SearchFile != "") { CodeString = ahk.FileRead(SearchFile); }
            else { CodeString = SearchText; }

            if (CodeString.Contains("." + SearchFunction))
            {
                List<string> functionList = FunctionList_FromCode(CodeString);  // returns list of functions found in ParseText

                foreach (string function in functionList)
                {
                    string returnCode = Return_Function_FromCode(CodeString, function);
                    if (returnCode.Contains("." + SearchFunction))
                    {
                        if (ExampleReturn != "") { ExampleReturn = ExampleReturn + Environment.NewLine + Environment.NewLine + returnCode; }
                        if (ExampleReturn == "") { ExampleReturn = returnCode; }
                    }
                }
            }

            return ExampleReturn;
        }




        //=== Parse C# Code (v1) ======

        public void ParseCode(string CodeFile)  // parse code (testing function)
        {
            string Code = ahk.FileRead(CodeFile);

            // for each line of text in code file
            using (StringReader reader = new StringReader(Code))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    bool CommentLine = false;
                    line = line.Trim(); //remove leading spaces

                    if (line.Length > 0) // make sure line has text
                    {
                        string FirstCharacter = line[0].ToString(); // return the first chracter of the string
                        if (FirstCharacter == "/") { CommentLine = true; }
                        if (!CommentLine)
                        {

                            // parse line by spaces
                            string[] words = line.Split(' ');
                            int index = 0;
                            string VarType = "";
                            string FunctionName = "";
                            foreach (string word in words)
                            {
                                if (index == 0) //variable type
                                {
                                    VarType = word;
                                    if (VarType.Contains("Dictionary")) { VarType = "Dictionary"; }
                                    if (VarType.Contains("List")) { VarType = "List"; }
                                    //ahk.MsgBox(VarType);
                                }

                                if (index == 1) // function name
                                {
                                    string[] funct = word.Split('(');
                                    int index2 = 0;
                                    foreach (string funcname in funct)
                                    {
                                        if (index2 == 0) { FunctionName = funcname; }
                                        index2++;
                                    }
                                }

                                if (index == 2) // function name
                                {
                                    ahk.MsgBox("VarType = " + VarType + " | Function = " + FunctionName);
                                }

                                Console.WriteLine(word);
                                index++;
                            }

                            //ahk.MsgBox(line); 

                        }
                    }


                }
            }

        }

        public string Parse_Lines(string Code, string FunctionLib)  // function used by other parse functions to extract info about .cs code
        {
            int LineCount = 0;
            bool NameSpaceFound = false;
            string NameSpaceCode = "";

            //=================================================
            // Extract NameSpace Name and Code UnderNeath
            //=================================================

            string ClassName = "";
            string FunctionName = "";
            string NameSpaceName = "";
            string[] lines = Code.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (string line in lines)
            {
                LineCount++;

                //=== parse line of C# code, returns Code as output and returns comment in the out field ===
                string CommentOut = "";
                string LineType = "";
                string CodeReturn = ParseCodeLine(line, out CommentOut, out LineType);
                //ahk.MsgBox(CodeReturn);
                //ahk.MsgBox(CommentOut);


                if (LineType == "namespace")
                {
                    NameSpaceName = CodeReturn.Trim();
                    //ahk.MsgBox("NameSpace: " + CodeReturn + " (" + LineCount.ToString() + ")");
                    NameSpaceFound = true;
                }

                if (LineType == "class")
                {
                    ClassName = CodeReturn;
                    //ahk.MsgBox("Class: " + ClassName + " (" + LineCount.ToString() + ")");
                }

                if (LineType == "function")
                {
                    string FunctionLine = CodeReturn;
                    FunctionName = ParseFunctionLine(FunctionLine);
                    //ahk.MsgBox("NameSpace: " + NameSpaceName + Environment.NewLine + "Class: " + ClassName + Environment.NewLine + "Function: " + FunctionName + " (" + LineCount.ToString() + ")");


                    //=============================================================
                    // UPDATE or INSERT Example
                    //=============================================================
                    string Comments = "";
                    string Tags = "";
                    bool Updated = sqlite.Execute(FunctionLib, "UPDATE FunctionLib set LineNum = '" + LineCount.ToString() + "', Tags = '" + Tags + "', Comments = '" + Comments + "' WHERE Class = '" + ClassName + "' AND Function = '" + FunctionName + "'");  // Update Table
                    if (!Updated) { Updated = sqlite.Execute(FunctionLib, "INSERT into FunctionLib (NameSpace, Class, Function, LineNum, Tags, Comments, TimeStamp) VALUES ('" + NameSpaceName + "', '" + ClassName + "', '" + FunctionName + "', '" + LineCount.ToString() + "', '" + Tags + "', '" + Comments + "', '" + DateTime.Now + "')"); }  // insert into a Table

                }


            }



            return "";
        }

        public string ParseFunctionLine(string line = "")   // function used by other parse functions to extract info about .cs code
        {
            //line = "public bool FileAppend(string WriteText, string FileName, string Debug= \"false\", bool value=false, bool value2 = true)  //note about function";

            string Comments = "";
            string LineType = "";
            line = ParseCodeLine(line, out Comments, out LineType);  //separate comments and code

            int Counter = 0;
            string StartOfLine = "";
            string FunctionParameters = "";

            // extract text before and after parenthesis to parse
            string[] words = line.Split('(');
            foreach (string word in words)
            {
                //ahk.MsgBox(word);

                if (Counter == 0) { StartOfLine = word; }
                if (Counter == 1) { FunctionParameters = word; }

                Counter++;
            }

            //ahk.MsgBox(StartOfLine);
            //ahk.MsgBox(FunctionParameters);


            //========================================================================
            // Parse first part of the line for the function name / output type
            //========================================================================

            string FunctionName = "";
            string FunctionOutput = "";
            string FunctionVisibility = "";
            bool Static = false;
            words = StartOfLine.Split(' ');
            Counter = 0;

            // count total number of words in string
            int WordCountTotal = words.Count();
            //foreach (string wordcount in words) { WordCountTotal++; }



            //ahk.MsgBox("WordCountTotal: " + WordCountTotal.ToString()); 

            foreach (string word in words)
            {
                Counter++;

                if (Counter == 1) { FunctionVisibility = word; }

                if (word == "static") { Static = true; continue; }

                if (Counter > 1 && Counter < WordCountTotal) { FunctionOutput = word; }

                FunctionName = word;  //function will be the last word in the loop
            }

            //ahk.MsgBox("Function: " + FunctionName + Environment.NewLine + "Output: " + FunctionOutput + Environment.NewLine + "Static: " + Static.ToString() + Environment.NewLine + "Comments: " + Comments);

            FunctionParameters = ahk.TrimLast(FunctionParameters); //trim off trailing )
            //ahk.MsgBox(FunctionParameters);


            //=================================
            // parse function parameters

            //string WriteText, string FileName, bool Debug = false

            Counter = 0;
            string[] paramaterarray = FunctionParameters.Split(',');
            foreach (string parameter in paramaterarray)
            {
                //ahk.MsgBox(parameter);  //bool Debug = false

                string[] paramaters = parameter.Split(' ');
                Counter = 0;
                string ParamType = "";
                string ParamName = "";
                string ParamDefaultVal = "";
                bool DefaultValFound = false;
                foreach (string par in paramaters)
                {
                    string ParameterWord = par;
                    ParameterWord = par.Trim();
                    if (ParameterWord == "") { continue; }


                    if (parameter.Contains("=")) { DefaultValFound = true; }  //if there is a default value in the function declaration

                    Counter++;
                    if (Counter == 1) { ParamType = par; }
                    if (Counter == 2) { ParamName = par; }


                    if (ParamName.Contains("="))  //if there is a default value in the function declaration
                    {
                        int ValCounter = 0;
                        string[] paramatervalue = ParamName.Split('=');
                        foreach (string parval in paramatervalue)
                        {
                            string ValLine = parval;
                            ValLine = parval.Trim();
                            if (ValLine == "") { continue; }

                            if (ValCounter == 0) { ParamName = ValLine; }
                            if (ValCounter == 1) { ParamDefaultVal = ValLine; }

                            ValCounter++;
                        }

                        DefaultValFound = false;
                    }


                    if (DefaultValFound)  // if the default value is provided in the parameter line
                    {
                        int ValCounter = 0;
                        string[] paramatervalue = parameter.Split('=');
                        foreach (string parval in paramatervalue)
                        {
                            string ValLine = parval;
                            ValLine = parval.Trim();
                            if (ValLine == "") { continue; }

                            if (ValCounter == 1) { ParamDefaultVal = ValLine; }

                            ValCounter++;
                        }
                    }

                    DefaultValFound = false;
                }

                //ahk.MsgBox("ParamType = " + ParamType + Environment.NewLine + "ParamName = " + ParamName + Environment.NewLine + "DefaultValue = " + ParamDefaultVal); 
                //if (Counter == 0) { StartOfLine = word; }
                //if (Counter == 1) { FunctionParameters = word; }

                Counter++;
            }

            return FunctionName;

        }

        public string ParseCodeLine(string line, out string CommentOut, out string LineTypeOut)   // function used by other parse functions to extract info about .cs code
        {
            LineTypeOut = "";
            CommentOut = "";
            if (line == null) { return ""; }

            //=== parse line of C# code, returns Code as output and returns comment in the out field ===

            //string line = "public class SQLite  // JDL SQLite Collection";
            //string CommentOut = "";
            //string Code = ParseCodeLine(line, out CommentOut); 
            //ahk.MsgBox(Code);
            //ahk.MsgBox(CommentOut);

            string LineType = "";
            string lineCode = line.Trim();  //trim extra spaces from line
            string FirstChar = "";
            string First2Chars = "";
            string Code = "";
            string Comment = "";
            bool CodeLine = false;
            bool CommentLine = false;
            string FirstWord = "";
            bool NameSpaceFound = false;
            string NameSpaceName = "";
            bool FunctionOrClass = false;
            bool FunctionFound = false;
            bool ClassFound = false;
            string ClassOrFunctionText = "";

            int WordNumber = 0;

            //================================
            // Parse String by Space
            //================================
            string[] words = lineCode.Split(' ');
            foreach (string word in words)
            {
                if (word == "") { continue; } //skip blank lines
                WordNumber++;
                //ahk.MsgBox(word); 
                if (WordNumber == 1) { FirstWord = word; }

                FirstChar = word.Substring(0, 1); //returns the first character of the string
                if (word.Length >= 2) { First2Chars = word.Substring(0, 2); } //returns the first two characters of the word (to see if a comment is starting)


                if (First2Chars == "//")
                {
                    CodeLine = false;
                    CommentLine = true;
                }

                if (!CommentLine)
                {
                    Code = Code + " " + word;
                }

                if (CommentLine)
                {
                    Comment = Comment + " " + word;
                }

                if (FirstWord == "namespace")
                {
                    FirstWord = "";
                    NameSpaceFound = true;
                    LineType = "namespace";
                    continue;
                }

                if (NameSpaceFound)
                {
                    NameSpaceName = word;
                    NameSpaceFound = false;
                }

                if (lineCode.ToUpper().Contains("USER32.DLL"))
                {
                    LineType = "dll call";
                }


                if (FirstWord == "public" || FirstWord == "private")
                {
                    FirstWord = "";
                    FunctionOrClass = true;
                    LineType = "function";
                    continue;
                }

                if (LineType == "function")
                {
                    if (word == "class")
                    {
                        LineType = "class";
                        continue;
                    }
                }

                //if (word == "string")
                //{
                //    LineType = "string";
                //}

                if (LineType == "class")
                {
                    ClassOrFunctionText = ClassOrFunctionText + " " + word;
                }

                if (LineType == "function")
                {
                    if (!lineCode.Contains("("))
                    {
                        LineType = "constant";
                    }
                }


                if (LineType == "function")
                {
                    ClassOrFunctionText = ClassOrFunctionText + " " + word;
                }

                if (LineType == "constant")
                {
                    ClassOrFunctionText = ClassOrFunctionText + " " + word;
                }


            }


            //if (NameSpaceName != "") { ahk.MsgBox("NameSpace: " + NameSpaceName); }
            //ahk.MsgBox(Code);
            //ahk.MsgBox(Comment);

            if (NameSpaceName != "")
            {
                Code = NameSpaceName;
            }

            //if (LineType == "class")
            //{
            //    ClassOrFunctionText = ahk.StringReplace(ClassOrFunctionText, "class"); 
            //    ahk.MsgBox("Class: " + ClassOrFunctionText); 
            //}

            if (LineType == "function")
            {
                if (Code.Contains("{ get; set; }"))
                {
                    LineType = "object";
                    Code = Code.Replace("public", "");
                    Code = Code.Replace("private", "");
                    Code = Code.Replace("static", "");
                    Code = Code.Replace("{ get; set; }", "");
                    Code = Code.Replace("string", "");
                    Code = Code.Trim();
                }

            }

            if (LineType == "class" || LineType == "function")
            {
                Code = ClassOrFunctionText;
                Code = Code.Replace("static", "");
                Code = Code.Trim();
            }

            if (LineType == "constant")
            {
                //public int Top { get; set; }
                Code = ClassOrFunctionText;

                Code = Code.Replace("{ get; set; }", "");
                Code = Code.Replace("public", "");
                Code = Code.Replace("private", "");
                Code = Code.Replace("static", "");
                Code = Code.Replace("string", "");
                Code = Code.Replace("int", "");
                Code = Code.Replace("const", "");
                Code = Code.Replace("UInt32", "");
                Code = Code.Trim();
            }


            LineTypeOut = LineType;
            CommentOut = Comment;
            return Code;
        }

        public string old_Parse_All_Lines(string DbFile, string CodePath, string TableName = "FunctionLib", bool BulkInsert = true, bool ReplaceOld = false)  // Parses C# File, Loading Function Names / Info into SQLite Table
        {
            //string CodeFile = @"C:\Users\jason\Google Drive\AHK\IMDB\SQLiter\AHK\AHK.cs";

            BulkInsert = false;


            string FileName = Path.GetFileName(CodePath);

            //sqlite.CreateUserDb(DbFile, TableName);

            if (ReplaceOld) { sqlite.Execute(DbFile, "Delete From " + TableName); } // clear out existing entries (used while testing)

            string Code = ahk.FileRead(CodePath);  // code to parse


            int LineCount = 0;
            bool NameSpaceFound = false;
            string NameSpaceCode = "";

            //=================================================
            // Extract NameSpace Name and Code UnderNeath
            //=================================================

            string ClassName = "";
            string FunctionName = "";
            string NameSpaceName = "";
            string[] lines = Code.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            string REgionName = "";

            SQLiteConnection conn = sqlite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

            int FID = 0;
            if (BulkInsert)
            {
                if (conn.State == ConnectionState.Closed) { conn.Open(); } //open SQLite connection
            }

            using (var cmd = new SQLiteCommand(conn))
            {
                using (var transaction = conn.BeginTransaction())
                {
                    foreach (string line in lines)
                    {
                        LineCount++;

                        //=== parse line of C# code, returns Code as output and returns comment in the out field ===
                        string CommentOut = "";
                        string LineType = "";
                        string CodeReturn = ParseCodeLine(line, out CommentOut, out LineType);
                        //ahk.MsgBox(CodeReturn);
                        //ahk.MsgBox(CommentOut);
                        string Summary = "";
                        string ParamName = "";
                        string ParamText = "";
                        string ReturnTag = "";

                        bool IsDocLine = IsDoc(line);
                        if (IsDocLine)
                        {
                            bool ISSummary = IsSummary(line);
                            if (ISSummary)
                            {
                                Summary = Extract_Summary(line);
                            }

                            if (IsParam(line))
                            {
                                ParamName = Extract_ParamName(line);
                                ParamText = Extract_ParamText(line, ParamName);
                            }

                            if (IsReturn(line))
                            {
                                ReturnTag = Extract_Return(line);
                            }
                        }


                        if (LineType == "dll call") { continue; }

                        if (LineType == "constant") { continue; }

                        if (LineType == "namespace")
                        {
                            NameSpaceName = CodeReturn.Trim();
                            //ahk.MsgBox("NameSpace: " + CodeReturn + " (" + LineCount.ToString() + ")");
                            NameSpaceFound = true;
                        }

                        if (LineType == "class")
                        {
                            ClassName = CodeReturn;
                            string LineTypeOut;
                            ClassName = ParseCodeLine(ClassName, out CommentOut, out LineTypeOut);  //separate the comments out
                            //ahk.MsgBox("Class: " + ClassName + " (" + LineCount.ToString() + ")");
                        }

                        if (IsRegion(line)) { REgionName = RegionName(line); }

                        if (LineType != "class" && LineType != "namespace" && LineType != "")
                        {
                            string FunctionLine = CodeReturn;
                            FunctionName = ParseFunctionLine(FunctionLine);
                            //ahk.MsgBox("NameSpace: " + NameSpaceName + Environment.NewLine + "Class: " + ClassName + Environment.NewLine + "Function: " + FunctionName + " (" + LineCount.ToString() + ")");

                            ClassName = ClassName.Trim();

                            //=============================================================
                            // UPDATE or INSERT 
                            //=============================================================
                            string Comments = ahk.FixSpecialChars(CommentOut);
                            string Tags = "";

                            if (FileName == "AHK.cs" || FileName == "Dev.cs" || FileName == "SSI.dll.cs" || FileName == "ZIP.cs" || FileName == "jdlCRMOD.cs")
                            {
                                Tags = ahk.StringReplace(FileName, ".cs", ".dll");
                            }

                            if (FileName == "ahk.cs") { FileName = "AHK.cs"; }

                            if (CodePath.Contains("SQLiter_DLL")) { Tags = ahk.StringReplace(FileName, ".cs", ".dll"); }

                            //string TagClassName = ClassName;

                            //if (TagClassName == "SQLITE") { TagClassName = "SQLite"; }
                            //if (TagClassName == "ftp") { TagClassName = "FTP"; }
                            //if (TagClassName == "TreeViewControl") { TagClassName = "TreeView"; }
                            //if (TagClassName == "Grid") { TagClassName = "DataGridview"; }
                            //if (TagClassName == "IniFile") { TagClassName = "INI"; }

                            //Tags = Tags + ", " + TagClassName;

                            CodeReturn = RemoveCommentsFromLine(CodeReturn);

                            string FunctionText = Return_Function_FromCode(Code, FunctionName);
                            string FilePath = CodePath;
                            string Parent = "";

                            FID++;
                            FunctionText = ahk.StringReplace(FunctionText, @"'", @"''");
                            string InsertLine = "INSERT into " + TableName + " (FileName, NameSpace, Class, Function, Type, LineNum, Tags, Comments, TimeStamp, FunctionLine, FunctionText, FilePath, Parent, DateAdded, FID) VALUES ('" + FileName + "', '" + NameSpaceName + "', '" + ClassName + "', '" + FunctionName + "', '" + REgionName + "', '" + LineCount.ToString() + "', '" + Tags + "', '" + Comments + "', '" + DateTime.Now + "', '" + CodeReturn + "', '" + FunctionText + "', '" + FilePath + "', '" + Parent + "', '" + DateTime.Now + "', '" + FID.ToString() + "')";

                            // setup the function params table by writing the function and file name fields first
                            string InsertLine2 = "INSERT into [FunctionParams] (FileName, Function) VALUES ('" + FileName + "', '" + FunctionName + "')";
                            string UpdateLine2 = "Update [FunctionParams] set FileName = '" + FileName + "' where Function = '" + FunctionName + "'";



                            if (!BulkInsert)
                            {
                                if (conn.State == ConnectionState.Open) { conn.Close(); } //close existing connection if needed
                                bool Insert2 = sqlite.Update_Insert(DbFile, UpdateLine2, InsertLine2);  // Update Table

                                string UpdateLine = "UPDATE " + TableName + " set LineNum = '" + LineCount.ToString() + "', Comments = '" + Comments + "', TimeStamp = '" + DateTime.Now + "', Type = '" + REgionName + "', FunctionLine = '" + CodeReturn + "', FunctionText = '" + FunctionText + "', FilePath = '" + FilePath + "', Parent = '" + Parent + "', FID = '" + FID.ToString() + "' WHERE Function = '" + FunctionName + "' AND FileName = '" + FileName + "'";
                                bool Updated = sqlite.Update_Insert(DbFile, UpdateLine, InsertLine);  // Update/Insert to Table

                                //if (conn.State == ConnectionState.Open) { conn.Close(); } //close existing connection if needed
                                //bool Updated = sqlite.Execute(DbFile, UpdateLine);  // Update Table
                                //if (!Updated) { Updated = sqlite.Execute(DbFile, InsertLine); }  // insert into a Table
                            }

                            if (BulkInsert)
                            {
                                cmd.CommandText = InsertLine;
                                cmd.ExecuteNonQuery();
                            }


                        }


                    }

                    if (BulkInsert)
                    {
                        transaction.Commit();  // write insert into SQLite 
                        conn.Close();
                    }
                }
            }





            return "";
        }

        public string RemoveCommentsFromLine(string line) // Returns code line without comments
        {
            //=== parse line of C# code, returns Code as output and returns comment in the out field ===

            //string line = "public class SQLite  // JDL SQLite Collection";

            string lineCode = line.Trim();  //trim extra spaces from line
            string First2Chars = "";
            string Code = "";

            //================================
            // Parse String by Space
            //================================
            string[] words = lineCode.Split(' ');
            foreach (string word in words)
            {
                if (word == "") { continue; } //skip blank lines
                //ahk.MsgBox(word); 

                if (word.Length >= 2) { First2Chars = word.Substring(0, 2); } //returns the first two characters of the word (to see if a comment is starting)

                if (First2Chars == "//")
                {
                    return Code;
                }

                if (Code == "") { Code = word; continue; }
                if (Code != "") { Code = Code + " " + word; continue; }
            }
            return Code;
        }

        public List<string> Functions_No_Longer_In_CodeFile(string DbFile, string CodeFilePath, string TableName = "FunctionLib")  // compares list of functions in dll vs functions in sqlite FunctionLib, returns list of functions no longer in sqlite FunctionLib 
        {
            List<string> ProjectFunctions = FunctionList_FromFile(CodeFilePath);

            string FileName = ahk.FileName(CodeFilePath);

            List<string> DbFileFunctions = lst.SQLite_To_List(DbFile, "select [Function] from [FunctionLib] where FileName = '" + FileName + "'");

            List<string> ToRemove = new List<string>();

            foreach (string dbFunction in DbFileFunctions)
            {
                if (!lst.InList(ProjectFunctions, dbFunction))
                {
                    ToRemove.Add(dbFunction);
                }
            }

            return ToRemove;
        }

        // parse function into parameters to write / update [FunctionParam] database  (latest - works)
        public void ParseFunction_WriteDb(string DbFile, string CodeFile)
        {
            //CodeFile = @"C:\Users\jason\Google Drive\IMDB\SQLiter\Dev_DLL\Dev.cs";

            string FileName = ahk.FileName(CodeFile);

            List<string> CodeLines = lst.TextFile_To_List(CodeFile);

            string OutLine = "";
            int i = 0;
            string functionName = "";
            string FunctionParams = "";
            string Comments = "";

            string Param1Name;
            string Param1Type;
            string Param1Default;
            string Param2Name;
            string Param2Type;
            string Param2Default;
            string Param3Name;
            string Param3Type;
            string Param3Default;
            string Param4Name;
            string Param4Type;
            string Param4Default;
            string Param5Name;
            string Param5Type;
            string Param5Default;
            string Param6Name;
            string Param6Type;
            string Param6Default;
            string Param7Name;
            string Param7Type;
            string Param7Default;
            string Param8Name;
            string Param8Type;
            string Param8Default;
            string Param9Name;
            string Param9Type;
            string Param9Default;


            foreach (string Line in CodeLines)
            {
                if (IsFunction(Line))
                {
                    functionName = FunctionName(Line);

                    //Comments = ahk.StringSplit(Line, @"//", 1);
                    string BeforeParenths = ahk.StringSplit(Line, "(", 0);
                    List<string> words = ahk.WordList(BeforeParenths);
                    foreach (string word in words)
                    {
                        if (word == "public") { BeforeParenths = ahk.StringReplace(BeforeParenths, "public"); }
                        if (word == "private") { BeforeParenths = ahk.StringReplace(BeforeParenths, "private"); }
                        if (word == "static") { BeforeParenths = ahk.StringReplace(BeforeParenths, "static"); }
                    }
                    BeforeParenths = ahk.StringReplace(BeforeParenths, functionName);
                    BeforeParenths = BeforeParenths.Trim();

                    string AfterParenths = ahk.StringSplit(Line, "(", 1);
                    AfterParenths = ahk.StringSplit(AfterParenths, ")", 0);

                    string OutPutVarType = BeforeParenths;

                    //AfterParenths = code.RemoveCommentsFromLine(AfterParenths);
                    int CommaCount = ahk.CharCount(AfterParenths, ",");
                    int ParamCount = 0;
                    if (CommaCount > 0 || AfterParenths.Trim() != "")
                    {
                        CommaCount++;
                        ParamCount = CommaCount;

                        string ParamEntry = AfterParenths;
                        int t = 0;
                        string LastParam = "";
                        string paramType = "";
                        string paramName = "";
                        do
                        {
                            ParamEntry = ahk.StringSplit(AfterParenths, ",", t);

                            paramType = "";
                            paramName = "";

                            if ((ParamEntry.Contains("<")) && (!ParamEntry.Contains(">")))
                            {
                                LastParam = ParamEntry;
                                t++;
                                continue;
                            }
                            if (LastParam != "")
                            {
                                paramName = ahk.StringSplit(ParamEntry, ">", 1);
                                ParamEntry = LastParam + ", " + ParamEntry;
                                paramType = ahk.StringSplit(ParamEntry, ">", 0);
                                paramType = paramType + ">";
                                LastParam = "";
                                t--;
                            }


                            //if ((ParamEntry.Contains(">")) && (!ParamEntry.Contains("<")))
                            //{
                            //    continue;
                            //}


                            //ahk.MsgBox(ParamEntry);

                            string SplitLine = ParamEntry.Trim();

                            if (t == 0)
                            {
                                if (!SplitLine.Contains("="))
                                {
                                    Param1Type = ahk.StringSplit(SplitLine, " ", 0); Param1Name = ahk.StringSplit(SplitLine, " ", 1);
                                    if (paramName != "") { Param1Name = paramName; Param1Type = paramType; }
                                    string InsertLine = "INSERT into [FunctionParams] (FileName, Function, OutVarType, ParamCount, Param1Type, Param1Name) VALUES ('" + FileName + "', '" + functionName + "', '" + OutPutVarType + "', '" + ParamCount + "', '" + Param1Type + "', '" + Param1Name + "')";
                                    string UpdateLine = "Update [FunctionParams] set OutVarType = '" + OutPutVarType + "', ParamCount = '" + ParamCount + "', Param1Type = '" + Param1Type + "', Param1Name = '" + Param1Name + "' where Function = '" + functionName + "' and FileName = '" + FileName + "'";
                                    bool Inserted = sqlite.Update_Insert(DbFile, UpdateLine, InsertLine);  // Update Table
                                    if (!Inserted) { ahk.MsgBox("Error Inserting: " + InsertLine); }
                                }
                                if (SplitLine.Contains("="))
                                {
                                    Param1Default = ahk.StringSplit(SplitLine, "=", 1); Param1Default = Param1Default.Trim(); string before = ahk.StringSplit(SplitLine, "=", 0); before = before.Trim(); Param1Name = ahk.StringSplit(before, " ", 1); Param1Type = ahk.StringSplit(before, " ", 0);
                                    if (paramName != "") { Param1Name = paramName; Param1Type = paramType; }
                                    string InsertLine = "INSERT into [FunctionParams] (FileName, Function, OutVarType, ParamCount, Param1Type, Param1Name, Param1Default) VALUES ('" + FileName + "', '" + functionName + "', '" + OutPutVarType + "', '" + ParamCount + "', '" + Param1Type + "', '" + Param1Name + "', '" + Param1Default + "')";
                                    string UpdateLine = "Update [FunctionParams] set OutVarType = '" + OutPutVarType + "', ParamCount = '" + ParamCount + "', Param1Type = '" + Param1Type + "', Param1Name = '" + Param1Name + "', Param1Default = '" + Param1Default + "' where Function = '" + functionName + "' and FileName = '" + FileName + "'";
                                    bool Inserted = sqlite.Update_Insert(DbFile, UpdateLine, InsertLine);  // Update Table
                                    if (!Inserted) { ahk.MsgBox("Error Inserting: " + InsertLine); }
                                }

                            }
                            if (t == 1)
                            {
                                if (!SplitLine.Contains("="))
                                {
                                    Param2Type = ahk.StringSplit(SplitLine, " ", 0); Param2Name = ahk.StringSplit(SplitLine, " ", 1);
                                    if (paramName != "") { Param2Name = paramName; Param2Type = paramType; }
                                    string InsertLine = "INSERT into [FunctionParams] (FileName, Function, OutVarType, ParamCount, Param2Type, Param2Name) VALUES ('" + FileName + "', '" + functionName + "', '" + OutPutVarType + "', '" + ParamCount + "', '" + Param2Type + "', '" + Param2Name + "')";
                                    string UpdateLine = "Update [FunctionParams] set OutVarType = '" + OutPutVarType + "', ParamCount = '" + ParamCount + "', Param2Type = '" + Param2Type + "', Param2Name = '" + Param2Name + "' where Function = '" + functionName + "' and FileName = '" + FileName + "'";
                                    bool Inserted = sqlite.Update_Insert(DbFile, UpdateLine, InsertLine);  // Update Table
                                    if (!Inserted) { ahk.MsgBox("Error Inserting: " + InsertLine); }
                                }
                                if (SplitLine.Contains("="))
                                {
                                    Param2Default = ahk.StringSplit(SplitLine, "=", 1); Param2Default = Param2Default.Trim(); string before = ahk.StringSplit(SplitLine, "=", 0); before = before.Trim(); Param2Name = ahk.StringSplit(before, " ", 1); Param2Type = ahk.StringSplit(before, " ", 0);
                                    if (paramName != "") { Param2Name = paramName; Param2Type = paramType; }
                                    string InsertLine = "INSERT into [FunctionParams] (FileName, Function, OutVarType, ParamCount, Param2Type, Param2Name, Param2Default) VALUES ('" + FileName + "', '" + functionName + "', '" + OutPutVarType + "', '" + ParamCount + "', '" + Param2Type + "', '" + Param2Name + "', '" + Param2Default + "')";
                                    string UpdateLine = "Update [FunctionParams] set OutVarType = '" + OutPutVarType + "', ParamCount = '" + ParamCount + "', Param2Type = '" + Param2Type + "', Param2Name = '" + Param2Name + "', Param2Default = '" + Param2Default + "' where Function = '" + functionName + "' and FileName = '" + FileName + "'";
                                    bool Inserted = sqlite.Update_Insert(DbFile, UpdateLine, InsertLine);  // Update Table
                                    if (!Inserted) { ahk.MsgBox("Error Inserting: " + InsertLine); }
                                }
                            }
                            if (t == 2)
                            {
                                if (!SplitLine.Contains("="))
                                {
                                    Param3Type = ahk.StringSplit(SplitLine, " ", 0); Param3Name = ahk.StringSplit(SplitLine, " ", 1);
                                    if (paramName != "") { Param3Name = paramName; Param3Type = paramType; }
                                    string InsertLine = "INSERT into [FunctionParams] (FileName, Function, OutVarType, ParamCount, Param3Type, Param3Name) VALUES ('" + FileName + "', '" + functionName + "', '" + OutPutVarType + "', '" + ParamCount + "', '" + Param3Type + "', '" + Param3Name + "')";
                                    string UpdateLine = "Update [FunctionParams] set OutVarType = '" + OutPutVarType + "', ParamCount = '" + ParamCount + "', Param3Type = '" + Param3Type + "', Param3Name = '" + Param3Name + "' where Function = '" + functionName + "' and FileName = '" + FileName + "'";
                                    bool Inserted = sqlite.Update_Insert(DbFile, UpdateLine, InsertLine);  // Update Table
                                    if (!Inserted) { ahk.MsgBox("Error Inserting: " + InsertLine); }
                                }
                                if (SplitLine.Contains("="))
                                {
                                    Param3Default = ahk.StringSplit(SplitLine, "=", 1); Param3Default = Param3Default.Trim(); string before = ahk.StringSplit(SplitLine, "=", 0); before = before.Trim(); Param3Name = ahk.StringSplit(before, " ", 1); Param3Type = ahk.StringSplit(before, " ", 0);
                                    if (paramName != "") { Param3Name = paramName; Param3Type = paramType; }
                                    string InsertLine = "INSERT into [FunctionParams] (FileName, Function, OutVarType, ParamCount, Param3Type, Param3Name, Param3Default) VALUES ('" + FileName + "', '" + functionName + "', '" + OutPutVarType + "', '" + ParamCount + "', '" + Param3Type + "', '" + Param3Name + "', '" + Param3Default + "')";
                                    string UpdateLine = "Update [FunctionParams] set OutVarType = '" + OutPutVarType + "', ParamCount = '" + ParamCount + "', Param3Type = '" + Param3Type + "', Param3Name = '" + Param3Name + "', Param3Default = '" + Param3Default + "' where Function = '" + functionName + "' and FileName = '" + FileName + "'";
                                    bool Inserted = sqlite.Update_Insert(DbFile, UpdateLine, InsertLine);  // Update Table
                                    if (!Inserted) { ahk.MsgBox("Error Inserting: " + InsertLine); }
                                }
                            }
                            if (t == 3)
                            {
                                if (!SplitLine.Contains("="))
                                {
                                    Param4Type = ahk.StringSplit(SplitLine, " ", 0); Param4Name = ahk.StringSplit(SplitLine, " ", 1);
                                    if (paramName != "") { Param4Name = paramName; Param4Type = paramType; }
                                    string InsertLine = "INSERT into [FunctionParams] (FileName, Function, OutVarType, ParamCount, Param4Type, Param4Name) VALUES ('" + FileName + "', '" + functionName + "', '" + OutPutVarType + "', '" + ParamCount + "', '" + Param4Type + "', '" + Param4Name + "')";
                                    string UpdateLine = "Update [FunctionParams] set OutVarType = '" + OutPutVarType + "', ParamCount = '" + ParamCount + "', Param4Type = '" + Param4Type + "', Param4Name = '" + Param4Name + "' where Function = '" + functionName + "' and FileName = '" + FileName + "'";
                                    bool Inserted = sqlite.Update_Insert(DbFile, UpdateLine, InsertLine);  // Update Table
                                    if (!Inserted) { ahk.MsgBox("Error Inserting: " + InsertLine); }
                                }
                                if (SplitLine.Contains("="))
                                {
                                    Param4Default = ahk.StringSplit(SplitLine, "=", 1); Param4Default = Param4Default.Trim(); string before = ahk.StringSplit(SplitLine, "=", 0); before = before.Trim(); Param4Name = ahk.StringSplit(before, " ", 1); Param4Type = ahk.StringSplit(before, " ", 0);
                                    if (paramName != "") { Param4Name = paramName; Param4Type = paramType; }
                                    string InsertLine = "INSERT into [FunctionParams] (FileName, Function, OutVarType, ParamCount, Param4Type, Param4Name, Param4Default) VALUES ('" + FileName + "', '" + functionName + "', '" + OutPutVarType + "', '" + ParamCount + "', '" + Param4Type + "', '" + Param4Name + "', '" + Param4Default + "')";
                                    string UpdateLine = "Update [FunctionParams] set OutVarType = '" + OutPutVarType + "', ParamCount = '" + ParamCount + "', Param4Type = '" + Param4Type + "', Param4Name = '" + Param4Name + "', Param4Default = '" + Param4Default + "' where Function = '" + functionName + "' and FileName = '" + FileName + "'";
                                    bool Inserted = sqlite.Update_Insert(DbFile, UpdateLine, InsertLine);  // Update Table
                                    if (!Inserted) { ahk.MsgBox("Error Inserting: " + InsertLine); }
                                }
                            }
                            if (t == 4)
                            {
                                if (!SplitLine.Contains("="))
                                {
                                    Param5Type = ahk.StringSplit(SplitLine, " ", 0); Param5Name = ahk.StringSplit(SplitLine, " ", 1);
                                    if (paramName != "") { Param5Name = paramName; Param5Type = paramType; }
                                    string InsertLine = "INSERT into [FunctionParams] (FileName, Function, OutVarType, ParamCount, Param5Type, Param5Name) VALUES ('" + FileName + "', '" + functionName + "', '" + OutPutVarType + "', '" + ParamCount + "', '" + Param5Type + "', '" + Param5Name + "')";
                                    string UpdateLine = "Update [FunctionParams] set OutVarType = '" + OutPutVarType + "', ParamCount = '" + ParamCount + "', Param5Type = '" + Param5Type + "', Param5Name = '" + Param5Name + "' where Function = '" + functionName + "' and FileName = '" + FileName + "'";
                                    bool Inserted = sqlite.Update_Insert(DbFile, UpdateLine, InsertLine);  // Update Table
                                    if (!Inserted) { ahk.MsgBox("Error Inserting: " + InsertLine); }
                                }
                                if (SplitLine.Contains("="))
                                {
                                    Param5Default = ahk.StringSplit(SplitLine, "=", 1); Param5Default = Param5Default.Trim(); string before = ahk.StringSplit(SplitLine, "=", 0); before = before.Trim(); Param5Name = ahk.StringSplit(before, " ", 1); Param5Type = ahk.StringSplit(before, " ", 0);
                                    if (paramName != "") { Param5Name = paramName; Param5Type = paramType; }
                                    string InsertLine = "INSERT into [FunctionParams] (FileName, Function, OutVarType, ParamCount, Param5Type, Param5Name, Param5Default) VALUES ('" + FileName + "', '" + functionName + "', '" + OutPutVarType + "', '" + ParamCount + "', '" + Param5Type + "', '" + Param5Name + "', '" + Param5Default + "')";
                                    string UpdateLine = "Update [FunctionParams] set OutVarType = '" + OutPutVarType + "', ParamCount = '" + ParamCount + "', Param5Type = '" + Param5Type + "', Param5Name = '" + Param5Name + "', Param5Default = '" + Param5Default + "' where Function = '" + functionName + "' and FileName = '" + FileName + "'";
                                    bool Inserted = sqlite.Update_Insert(DbFile, UpdateLine, InsertLine);  // Update Table
                                    if (!Inserted) { ahk.MsgBox("Error Inserting: " + InsertLine); }
                                }
                            }
                            if (t == 5)
                            {
                                if (!SplitLine.Contains("="))
                                {
                                    Param6Type = ahk.StringSplit(SplitLine, " ", 0); Param6Name = ahk.StringSplit(SplitLine, " ", 1);
                                    if (paramName != "") { Param6Name = paramName; Param6Type = paramType; }
                                    string InsertLine = "INSERT into [FunctionParams] (FileName, Function, OutVarType, ParamCount, Param6Type, Param6Name) VALUES ('" + FileName + "', '" + functionName + "', '" + OutPutVarType + "', '" + ParamCount + "', '" + Param6Type + "', '" + Param6Name + "')";
                                    string UpdateLine = "Update [FunctionParams] set OutVarType = '" + OutPutVarType + "', ParamCount = '" + ParamCount + "', Param6Type = '" + Param6Type + "', Param6Name = '" + Param6Name + "' where Function = '" + functionName + "' and FileName = '" + FileName + "'";
                                    bool Inserted = sqlite.Update_Insert(DbFile, UpdateLine, InsertLine);  // Update Table
                                    if (!Inserted) { ahk.MsgBox("Error Inserting: " + InsertLine); }
                                }
                                if (SplitLine.Contains("="))
                                {
                                    Param6Default = ahk.StringSplit(SplitLine, "=", 1); Param6Default = Param6Default.Trim(); string before = ahk.StringSplit(SplitLine, "=", 0); before = before.Trim(); Param6Name = ahk.StringSplit(before, " ", 1); Param6Type = ahk.StringSplit(before, " ", 0);
                                    if (paramName != "") { Param6Name = paramName; Param6Type = paramType; }
                                    string InsertLine = "INSERT into [FunctionParams] (FileName, Function, OutVarType, ParamCount, Param6Type, Param6Name, Param6Default) VALUES ('" + FileName + "', '" + functionName + "', '" + OutPutVarType + "', '" + ParamCount + "', '" + Param6Type + "', '" + Param6Name + "', '" + Param6Default + "')";
                                    string UpdateLine = "Update [FunctionParams] set OutVarType = '" + OutPutVarType + "', ParamCount = '" + ParamCount + "', Param6Type = '" + Param6Type + "', Param6Name = '" + Param6Name + "', Param6Default = '" + Param6Default + "' where Function = '" + functionName + "' and FileName = '" + FileName + "'";
                                    bool Inserted = sqlite.Update_Insert(DbFile, UpdateLine, InsertLine);  // Update Table
                                    if (!Inserted) { ahk.MsgBox("Error Inserting: " + InsertLine); }
                                }
                            }
                            if (t == 6)
                            {
                                if (!SplitLine.Contains("="))
                                {
                                    Param7Type = ahk.StringSplit(SplitLine, " ", 0); Param7Name = ahk.StringSplit(SplitLine, " ", 1);
                                    if (paramName != "") { Param7Name = paramName; Param7Type = paramType; }
                                    string InsertLine = "INSERT into [FunctionParams] (FileName, Function, OutVarType, ParamCount, Param7Type, Param7Name) VALUES ('" + FileName + "', '" + functionName + "', '" + OutPutVarType + "', '" + ParamCount + "', '" + Param7Type + "', '" + Param7Name + "')";
                                    string UpdateLine = "Update [FunctionParams] set OutVarType = '" + OutPutVarType + "', ParamCount = '" + ParamCount + "', Param7Type = '" + Param7Type + "', Param7Name = '" + Param7Name + "' where Function = '" + functionName + "' and FileName = '" + FileName + "'";
                                    bool Inserted = sqlite.Update_Insert(DbFile, UpdateLine, InsertLine);  // Update Table
                                    if (!Inserted) { ahk.MsgBox("Error Inserting: " + InsertLine); }
                                }
                                if (SplitLine.Contains("="))
                                {
                                    Param7Default = ahk.StringSplit(SplitLine, "=", 1); Param7Default = Param7Default.Trim(); string before = ahk.StringSplit(SplitLine, "=", 0); before = before.Trim(); Param7Name = ahk.StringSplit(before, " ", 1); Param7Type = ahk.StringSplit(before, " ", 0);
                                    if (paramName != "") { Param7Name = paramName; Param7Type = paramType; }
                                    string InsertLine = "INSERT into [FunctionParams] (FileName, Function, OutVarType, ParamCount, Param7Type, Param7Name, Param7Default) VALUES ('" + FileName + "', '" + functionName + "', '" + OutPutVarType + "', '" + ParamCount + "', '" + Param7Type + "', '" + Param7Name + "', '" + Param7Default + "')";
                                    string UpdateLine = "Update [FunctionParams] set OutVarType = '" + OutPutVarType + "', ParamCount = '" + ParamCount + "', Param7Type = '" + Param7Type + "', Param7Name = '" + Param7Name + "', Param7Default = '" + Param7Default + "' where Function = '" + functionName + "' and FileName = '" + FileName + "'";
                                    bool Inserted = sqlite.Update_Insert(DbFile, UpdateLine, InsertLine);  // Update Table
                                    if (!Inserted) { ahk.MsgBox("Error Inserting: " + InsertLine); }
                                }
                            }
                            if (t == 7)
                            {
                                if (!SplitLine.Contains("="))
                                {
                                    Param8Type = ahk.StringSplit(SplitLine, " ", 0); Param8Name = ahk.StringSplit(SplitLine, " ", 1);
                                    if (paramName != "") { Param8Name = paramName; Param8Type = paramType; }
                                    string InsertLine = "INSERT into [FunctionParams] (FileName, Function, OutVarType, ParamCount, Param8Type, Param8Name) VALUES ('" + FileName + "', '" + functionName + "', '" + OutPutVarType + "', '" + ParamCount + "', '" + Param8Type + "', '" + Param8Name + "')";
                                    string UpdateLine = "Update [FunctionParams] set OutVarType = '" + OutPutVarType + "', ParamCount = '" + ParamCount + "', Param8Type = '" + Param8Type + "', Param8Name = '" + Param8Name + "' where Function = '" + functionName + "' and FileName = '" + FileName + "'";
                                    bool Inserted = sqlite.Update_Insert(DbFile, UpdateLine, InsertLine);  // Update Table
                                    if (!Inserted) { ahk.MsgBox("Error Inserting: " + InsertLine); }
                                }
                                if (SplitLine.Contains("="))
                                {
                                    Param8Default = ahk.StringSplit(SplitLine, "=", 1); Param8Default = Param8Default.Trim(); string before = ahk.StringSplit(SplitLine, "=", 0); before = before.Trim(); Param8Name = ahk.StringSplit(before, " ", 1); Param8Type = ahk.StringSplit(before, " ", 0);
                                    if (paramName != "") { Param8Name = paramName; Param8Type = paramType; }
                                    string InsertLine = "INSERT into [FunctionParams] (FileName, Function, OutVarType, ParamCount, Param8Type, Param8Name, Param8Default) VALUES ('" + FileName + "', '" + functionName + "', '" + OutPutVarType + "', '" + ParamCount + "', '" + Param8Type + "', '" + Param8Name + "', '" + Param8Default + "')";
                                    string UpdateLine = "Update [FunctionParams] set OutVarType = '" + OutPutVarType + "', ParamCount = '" + ParamCount + "', Param8Type = '" + Param8Type + "', Param8Name = '" + Param8Name + "', Param8Default = '" + Param8Default + "' where Function = '" + functionName + "' and FileName = '" + FileName + "'";
                                    bool Inserted = sqlite.Update_Insert(DbFile, UpdateLine, InsertLine);  // Update Table
                                    if (!Inserted) { ahk.MsgBox("Error Inserting: " + InsertLine); }
                                }
                            }
                            if (t == 8)
                            {
                                if (!SplitLine.Contains("="))
                                {
                                    Param9Type = ahk.StringSplit(SplitLine, " ", 0); Param9Name = ahk.StringSplit(SplitLine, " ", 1);
                                    if (paramName != "") { Param9Name = paramName; Param9Type = paramType; }
                                    string InsertLine = "INSERT into [FunctionParams] (FileName, Function, OutVarType, ParamCount, Param9Type, Param9Name) VALUES ('" + FileName + "', '" + functionName + "', '" + OutPutVarType + "', '" + ParamCount + "', '" + Param9Type + "', '" + Param9Name + "')";
                                    string UpdateLine = "Update [FunctionParams] set OutVarType = '" + OutPutVarType + "', ParamCount = '" + ParamCount + "', Param9Type = '" + Param9Type + "', Param9Name = '" + Param9Name + "' where Function = '" + functionName + "' and FileName = '" + FileName + "'";
                                    bool Inserted = sqlite.Update_Insert(DbFile, UpdateLine, InsertLine);  // Update Table
                                    if (!Inserted) { ahk.MsgBox("Error Inserting: " + InsertLine); }
                                }
                                if (SplitLine.Contains("="))
                                {
                                    Param9Default = ahk.StringSplit(SplitLine, "=", 1); Param9Default = Param9Default.Trim(); string before = ahk.StringSplit(SplitLine, "=", 0); before = before.Trim(); Param9Name = ahk.StringSplit(before, " ", 1); Param9Type = ahk.StringSplit(before, " ", 0);
                                    if (paramName != "") { Param9Name = paramName; Param9Type = paramType; }
                                    string InsertLine = "INSERT into [FunctionParams] (FileName, Function, OutVarType, ParamCount, Param9Type, Param9Name, Param9Default) VALUES ('" + FileName + "', '" + functionName + "', '" + OutPutVarType + "', '" + ParamCount + "', '" + Param9Type + "', '" + Param9Name + "', '" + Param9Default + "')";
                                    string UpdateLine = "Update [FunctionParams] set OutVarType = '" + OutPutVarType + "', ParamCount = '" + ParamCount + "', Param9Type = '" + Param9Type + "', Param9Name = '" + Param9Name + "', Param9Default = '" + Param9Default + "' where Function = '" + functionName + "' and FileName = '" + FileName + "'";
                                    bool Inserted = sqlite.Update_Insert(DbFile, UpdateLine, InsertLine);  // Update Table
                                    if (!Inserted) { ahk.MsgBox("Error Inserting: " + InsertLine); }
                                }
                            }


                            if (paramType != "")
                            {
                                paramType = "";
                                paramName = "";
                            }


                            t++;
                        } while (t < ParamCount);

                    }

                }
            }


        }


        // create and write parsed functions from .cs file to sqlite db (creates new table if doesn't exist)
        public void old_Write_Function_Db(string DbFile, string CodePath, string TableName = "FunctionLib", bool Insert = true)  // Insert False = Update (line by line update instead of batch insert)
        {
            old_Create_FunctionList_Table(DbFile, TableName);
            if (Insert)
            {
                old_Parse_All_Lines(DbFile, CodePath, TableName, true, true);  // false for bulk insert = UPDATE 
            }

            if (!Insert) // Update 
            {
                old_Parse_All_Lines(DbFile, CodePath, TableName, false, false);  // false for bulk insert = UPDATE 
            }

        }

        public void old_Create_FunctionList_Table(string DbFile, string FunctionTableName = "FunctionLib")
        {

            //========================================================
            // Create New SQLite DB (*Used First-Run*)
            //========================================================
            if (!File.Exists(DbFile)) // create database file if it doen't exist
            {
                SQLiteConnection.CreateFile(DbFile);
            }

            //=====================================================
            // Create New Table If It Does NOT Exist Yet
            //=====================================================

            bool TableExist = sqlite.Table_Exists(DbFile, FunctionTableName);  //See if selected Table Exists in SQLite DB file

            if (!TableExist)  // Table DOES NOT exist in SQLite DB
            {
                string NewTableLine = "";
                NewTableLine = "Flagged BOOL, FileName VARCHAR, NameSpace VARCHAR, Class VARCHAR, Function VARCHAR, Type VARCHAR, LineNum VARCHAR, Tags VARCHAR, Comments VARCHAR, TimeStamp VARCHAR, FunctionLine VARCHAR, FunctionText VARCHAR, FilePath VARCHAR, Parent VARCHAR, Example VARCHAR, Documentation VARCHAR, UsedInProjects VARCHAR, DateAdded VARCHAR, FID INTEGER";
                //ahk.MsgBox(NewTableLine); 
                bool ReturnValue = sqlite.Execute(DbFile, "CREATE TABLE [" + FunctionTableName + "] (" + NewTableLine + ")");  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
            }

        }


        #endregion


        #endregion

        #region === Function Lib ===

        #region === Function Objects ===

        // public var to store current loaded function

        // reference as "_Code.fUNCTION" in other projects

        /// <summary>
        /// container for function info
        /// </summary>
        public struct fUNCTION
        {
            public string Flagged { get; set; }
            public string FileName { get; set; }
            public string NameSpace { get; set; }
            public string Class { get; set; }
            public string Function { get; set; }
            public string Type { get; set; }
            public string LineNum { get; set; }
            public string Tags { get; set; }
            public string Comments { get; set; }
            public string TimeStamp { get; set; }
            public string FunctionLine { get; set; }
            public string FunctionText { get; set; }
            public string FilePath { get; set; }
            public string Parent { get; set; }
            public string Example { get; set; }
            public string Documentation { get; set; }
            public string UsedInProjects { get; set; }
            public string DateAdded { get; set; }
            public string FID { get; set; }

        }   

        /// <summary>
        /// object for hiding/showing grid columns and storing values to reload
        /// </summary>
        public struct fUNCTIONgrid 
        {
            public bool Flagged { get; set; }
            public bool FileName { get; set; }
            public bool NameSpace { get; set; }
            public bool Class { get; set; }
            public bool Function { get; set; }
            public bool Type { get; set; }
            public bool LineNum { get; set; }
            public bool Tags { get; set; }
            public bool Comments { get; set; }
            public bool TimeStamp { get; set; }
            public bool FunctionLine { get; set; }
            public bool FunctionText { get; set; }
            public bool FilePath { get; set; }
            public bool Parent { get; set; }
            public bool Example { get; set; }
            public bool Documentation { get; set; }
            public bool UsedInProjects { get; set; }
            public bool DateAdded { get; set; }
            public bool FID { get; set; }
        }


        /// <summary>
        /// populate function object from datagridview
        /// </summary>
        /// <param name="dv"></param>
        /// <param name="RowNum"></param>
        /// <returns></returns>
        public _Code.fUNCTION FunctionData_From_Grid(DataGridView dv, int RowNum)  
        {
            _Code.fUNCTION fn = new _Code.fUNCTION();

            fn.Flagged = dv.Rows[RowNum].Cells["Flagged"].Value.ToString();
            fn.FileName = dv.Rows[RowNum].Cells["FileName"].Value.ToString();
            fn.NameSpace = dv.Rows[RowNum].Cells["NameSpace"].Value.ToString();
            fn.Class = dv.Rows[RowNum].Cells["Class"].Value.ToString();
            fn.Function = dv.Rows[RowNum].Cells["Function"].Value.ToString();
            fn.Type = dv.Rows[RowNum].Cells["Type"].Value.ToString();
            fn.LineNum = dv.Rows[RowNum].Cells["LineNum"].Value.ToString();
            fn.Tags = dv.Rows[RowNum].Cells["Tags"].Value.ToString();
            fn.Comments = dv.Rows[RowNum].Cells["Comments"].Value.ToString();
            fn.TimeStamp = dv.Rows[RowNum].Cells["TimeStamp"].Value.ToString();
            fn.FunctionLine = dv.Rows[RowNum].Cells["FunctionLine"].Value.ToString();
            fn.FunctionText = dv.Rows[RowNum].Cells["FunctionText"].Value.ToString();
            fn.FilePath = dv.Rows[RowNum].Cells["FilePath"].Value.ToString();
            fn.Parent = dv.Rows[RowNum].Cells["Parent"].Value.ToString();
            fn.Example = dv.Rows[RowNum].Cells["Example"].Value.ToString();
            fn.Documentation = dv.Rows[RowNum].Cells["Documentation"].Value.ToString();
            fn.UsedInProjects = dv.Rows[RowNum].Cells["UsedInProjects"].Value.ToString();
            fn.DateAdded = dv.Rows[RowNum].Cells["DateAdded"].Value.ToString();
            fn.FID = dv.Rows[RowNum].Cells["FID"].Value.ToString();

            //LoadedFunction = fn;

            return fn;
        }


        /// <summary>
        /// takes user edits from function grid and saves/updates sqlite database
        /// </summary>
        /// <param name="dv"></param>
        /// <param name="DbFile"></param>
        public void Save_FunctionLib_Grid(DataGridView dv, string DbFile)  
        {
            using (var conn = new SQLiteConnection(@"Data Source=" + DbFile))
            {
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach (DataGridViewRow row in dv.Rows)
                        {
                            if (row.Cells[1].Value == null) { continue; }

                            //string Flagged = row.Cells[0].Value.ToString();
                            //string FileName = row.Cells[1].Value.ToString();
                            //string NameSpace = row.Cells[2].Value.ToString();
                            //string Class = row.Cells[3].Value.ToString();
                            //string Function = row.Cells[4].Value.ToString();
                            //string Type = row.Cells[5].Value.ToString();
                            //string LineNum = row.Cells[6].Value.ToString();
                            //string Tags = row.Cells[7].Value.ToString();
                            //string Comments = row.Cells[8].Value.ToString();
                            //string TimeStamp = row.Cells[9].Value.ToString();
                            //string FunctionLine = row.Cells[10].Value.ToString();
                            //ahk.MsgBox(Function);

                            _Code.fUNCTION fn = new _Code.fUNCTION();

                            fn.Flagged = row.Cells["Flagged"].Value.ToString();
                            fn.FileName = row.Cells["FileName"].Value.ToString();
                            fn.NameSpace = row.Cells["NameSpace"].Value.ToString();
                            fn.Class = row.Cells["Class"].Value.ToString();
                            fn.Function = row.Cells["Function"].Value.ToString();
                            fn.Type = row.Cells["Type"].Value.ToString();
                            fn.LineNum = row.Cells["LineNum"].Value.ToString();
                            fn.Tags = row.Cells["Tags"].Value.ToString();
                            fn.Comments = row.Cells["Comments"].Value.ToString();
                            fn.TimeStamp = row.Cells["TimeStamp"].Value.ToString();
                            fn.FunctionLine = row.Cells["FunctionLine"].Value.ToString();
                            fn.FunctionText = row.Cells["FunctionText"].Value.ToString();
                            fn.FilePath = row.Cells["FilePath"].Value.ToString();
                            fn.Parent = row.Cells["Parent"].Value.ToString();
                            fn.Example = row.Cells["Example"].Value.ToString();
                            fn.Documentation = row.Cells["Documentation"].Value.ToString();
                            fn.UsedInProjects = row.Cells["UsedInProjects"].Value.ToString();
                            fn.DateAdded = row.Cells["DateAdded"].Value.ToString();
                            fn.FID = row.Cells["FID"].Value.ToString();


                            fn.Comments = ahk.StringReplace(fn.Comments, "'", "''");

                            string UpdateLine = "UPDATE [FunctionLib] set Flagged = '" + fn.Flagged + "', Tags = '" + fn.Tags + "', Comments = '" + fn.Comments + "' WHERE Function = '" + fn.Function + "' and FileName = '" + fn.FileName + "'";

                            //UpdateLine = ahk.StringReplace(UpdateLine, "'", "''"); 
                            cmd.CommandText = UpdateLine + ";";
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }

                conn.Close();
            }

        }


        #endregion


        //public static class GlobalVars
        //{
        //    // ex:
        //    // GlobalVars global = new GlobalVars();  // allow access to set of global settings used across interfaces
        //    // if (Play == true) { global.LoadedFunction = true; }

        //    public fUNCTION LoadedFunction { get; set; }

        //    public bool ErrorLevel { get; set; }
        //    public bool Debug { get; set; }

        //}


        #endregion

        #region === Insert Code Lib ===

        /// <summary>
        /// insert startup function references under the Initialize() function
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <param name="Prompt"></param>
        /// <returns></returns>
        public List<string> Setup_New_Project(string ProjectFile, bool Prompt = true) 
        {
            bool DebugExport = true;
            string ErrorMsg;
            bool Inserted = false;
            string InsertDir = @"C:\Users\Jason\Google Drive\IMDB\SQLiter\Dynamic_Coder\bin\Debug\Project_Files\Code_Bin\Code Insert";

            List<string> UpdateList = new List<string>();

            // build dictionary list of function startup lines to insert under the Initialize() function
            Dictionary<string, string> InsertStartLineDict = new Dictionary<string, string>();
            InsertStartLineDict.Add("Define_EventHandlers", "Define_EventHandlers();");
            InsertStartLineDict.Add("Control_Setup", "Control_Setup();");


            // build dictionary list of function names + file path to read and insert into project
            Dictionary<string, string> InsertDict = new Dictionary<string, string>();
            InsertDict.Add("Define_EventHandlers", "\t\tpublic void Define_EventHandlers()\n\t\t{\n\t\t}\n");
            InsertDict.Add("Control_Setup", "\t\tpublic void Control_Setup()\n\t\t{\n\t\t}\n");



            if (Prompt)  // confirm user wants to insert code into current project
            {
                var ResultValue = ahk.YesNoBox("Add Startup Lines To Project?", "Add Startup Line to " + ahk.FileName(ProjectFile) + "?");
                if (ResultValue.ToString() != "Yes")
                {
                    return null;
                }
            }



            // create the === Startup === region, move the init/load functions into startup, add dll references

            string StartupRegionCode = ""; // PLACEHOLDER FOR INSERT CODE
            Inserted = Insert_Startup_Region(ProjectFile, out ErrorMsg, StartupRegionCode, "        #region ===[ Startup ]===");
            if (!Inserted) { UpdateList.Add(ErrorMsg); }
            if (Inserted) { UpdateList.Add("Inserted ===Startup=== Region Code"); }


            // loop through dictionary and insert startup function lines not already in the project
            foreach (var pair in InsertStartLineDict)
            {
                string functionName = pair.Key;
                string CSFile = pair.Value;

                bool Code_Already_Inserted = Function_InFile(ProjectFile, functionName);  // check to see if project file already has this function
                if (Code_Already_Inserted) { UpdateList.Add(functionName + " Already In Project"); }

                if (!Code_Already_Inserted)
                {
                    Inserted = Insert_Startup_Code_ToFile(ProjectFile, CSFile, out ErrorMsg);  // write startup code under the Initialize() function
                    if (!Inserted) { UpdateList.Add("ERROR Inserting " + functionName + " : " + ErrorMsg); }
                    if (Inserted) { UpdateList.Add("Inserted " + functionName + " Startup Reference"); }
                }
            }



            string InitializeFunctionName = Find_Initialize_Function_FromFile(ProjectFile);  // name of function to write startup functions underneath

            // loop through dictionary and insert startup functions not already in the project
            foreach (var pair in InsertDict)
            {
                string functionName = pair.Key;

                bool Code_Already_Inserted = Function_InFile(ProjectFile, functionName);  // check to see if project file already has this function
                if (Code_Already_Inserted) { UpdateList.Add(functionName + " Already In Project"); }

                if (!Code_Already_Inserted)
                {
                    string insertFunction = pair.Value.ToString();
                    Inserted = Insert_Function_After_Function_ToFile(ProjectFile, InitializeFunctionName, insertFunction, out ErrorMsg);
                    if (!Inserted) { UpdateList.Add("ERROR Inserting " + functionName + " : " + ErrorMsg); }
                    if (Inserted) { UpdateList.Add("Inserted " + functionName + " Function Code"); }
                }
            }



            //      Control Code Assemble 
            // =========================================
            // loop through controls in project and add in control setup basic interaction functionality

            bool AddControlCode = true;
            string ControlRegion = "#region ===[ Controls ]===\n";
            List<string> ControlSetup = new List<string>();  // list of functions to load on startup to configure controls


            // options for insert code not directly tied to existing controls
            bool CreateStatusBar = true;
            bool FileDialogs = true;
            bool FileSystemWatcher = true;


            // ######## StatusBar #########
            if (CreateStatusBar)
            {
                ControlSetup.Add("CreateStatusBar();"); // function to add to project startup
            }

            // read new function to insert from seperate code file
            string NewRegion = ahk.FileRead(InsertDir + "\\StatusBar.cs");
            ControlRegion = ControlRegion + NewRegion + "\n\n";


            if (AddControlCode)
            {
                // ####### DataGridView #######
                List<string> Controls = ProjectFile_ControlList(ProjectFile, "DataGridView", false);
                if (Controls.Count > 0)
                {
                    ControlRegion = ControlRegion + "\t#region === DataGridView ===\n";
                    string InsertCodeFile = InsertDir + "\\DataGridView.cs";

                    string InsertCode = "";
                    foreach (string control in Controls)
                    {
                        InsertCode = ahk.FileRead(InsertCodeFile);
                        InsertCode = ahk.StringReplace(InsertCode, "[GridName]", control);    // replace the Macro text in insert code with user's grid name
                        InsertCode = ahk.StringReplace(InsertCode, "[ControlName]", control);  // replace the Macro text in insert code with user's control name
                        //ControlRegion = ControlRegion + "\t\t#region === " + control + " ===\n\n";
                        ControlRegion = ControlRegion + InsertCode;
                        //ControlRegion = ControlRegion + "\n\n\t\t#endregion\n\n";
                    }

                    ControlRegion = ControlRegion + "\n\t#endregion " + @"// End of DataGridView" + "\n";
                }

                // ####### TreeView #######
                Controls = ProjectFile_ControlList(ProjectFile, "TreeView", false);
                if (Controls.Count > 0)
                {
                    ControlRegion = ControlRegion + "\n\t#region === TreeView ===\n";
                    string InsertCodeFile = InsertDir + "\\TreeView.cs";

                    string InsertCode = "";
                    foreach (string control in Controls)
                    {
                        InsertCode = ahk.FileRead(InsertCodeFile);
                        InsertCode = ahk.StringReplace(InsertCode, "[GridName]", control);    // replace the Macro text in insert code with user's grid name
                        InsertCode = ahk.StringReplace(InsertCode, "[ControlName]", control);  // replace the Macro text in insert code with user's control name
                        //ControlRegion = ControlRegion + "\t\t#region === " + control + " ===\n\n";
                        ControlRegion = ControlRegion + InsertCode + "\n";
                        //ControlRegion = ControlRegion + "\n\n\t\t#endregion\n\n";
                    }

                    ControlRegion = ControlRegion + "\n\t#endregion " + @"// End of TreeView" + "\n";
                }

                // ####### TabControl #######
                Controls = ProjectFile_ControlList(ProjectFile, "TabControl", false);
                if (Controls.Count > 0)
                {
                    ControlRegion = ControlRegion + "\n\t#region === TabControl ===\n";
                    string InsertCodeFile = InsertDir + "\\TabControl.cs";

                    string InsertCode = "";
                    foreach (string control in Controls)
                    {
                        ControlSetup.Add(control + "_Load_Last_Tab();"); // function to add to project startup

                        InsertCode = ahk.FileRead(InsertCodeFile);
                        InsertCode = ahk.StringReplace(InsertCode, "[GridName]", control);    // replace the Macro text in insert code with user's grid name
                        InsertCode = ahk.StringReplace(InsertCode, "[ControlName]", control);  // replace the Macro text in insert code with user's control name
                        //ControlRegion = ControlRegion + "\t\t#region === " + control + " ===\n\n";
                        ControlRegion = ControlRegion + InsertCode + "\n";
                        //ControlRegion = ControlRegion + "\n\n\t\t#endregion\n\n";
                    }

                    ControlRegion = ControlRegion + "\n\t#endregion " + @"// End of TabControl" + "\n";
                }

                // ####### Scintilla #######
                Controls = ProjectFile_ControlList(ProjectFile, "Scintilla", false);
                if (Controls.Count > 0)
                {
                    ControlRegion = ControlRegion + "\n\t#region === Scintilla ===\n";
                    string InsertCodeFile = InsertDir + "\\Scintilla.cs";

                    string InsertCode = "";
                    foreach (string control in Controls)
                    {
                        ControlSetup.Add(control + "_Setup();"); // function to add to project startup

                        InsertCode = ahk.FileRead(InsertCodeFile);
                        InsertCode = ahk.StringReplace(InsertCode, "[GridName]", control);    // replace the Macro text in insert code with user's grid name
                        InsertCode = ahk.StringReplace(InsertCode, "[ControlName]", control);  // replace the Macro text in insert code with user's control name
                        //ControlRegion = ControlRegion + "\t\t#region === " + control + " ===\n\n";
                        ControlRegion = ControlRegion + InsertCode + "\n";
                        //ControlRegion = ControlRegion + "\n\n\t\t#endregion\n\n";
                    }

                    ControlRegion = ControlRegion + "\n\t#endregion " + @"// End of Scintilla" + "\n";
                }


                // ######## File Dialogs (Open/Save) #########
                if (FileDialogs)
                {
                    NewRegion = ahk.FileRead(InsertDir + "\\File_Dialogs.cs");
                    ControlRegion = ControlRegion + NewRegion + "\n";
                }

                // ######## FileSystemWatcher (DirWatch) #########
                if (FileSystemWatcher)
                {
                    NewRegion = ahk.FileRead(InsertDir + "\\FileSystemWatcher.cs");
                    ControlRegion = ControlRegion + NewRegion + "\n";
                }

                // ######## Menu #########
                Controls = ProjectFile_ControlList(ProjectFile, "ToolStripMenuItem", false);
                NewRegion = ahk.FileRead(InsertDir + "\\Menu.cs");
                ControlRegion = ControlRegion + NewRegion + "\n";

                foreach (string control in Controls)
                {
                    ControlSetup.Add("menu.MenuItem_Checked_Last(" + control + ");");  // function to add to project startup
                }


                // ######## Buttons #########
                Controls = ProjectFile_ControlList(ProjectFile, "Button", false);

                foreach (string control in Controls)
                {
                    //ControlSetup.Add("menu.MenuItem_Checked_Last(" + control + ");");  // function to add to project startup
                }



                ControlRegion = ControlRegion + "\n#endregion " + @"// End of CONTROLS";
            }


            // ### Write Control Setup Functions to Top (TODO)




            //ControlRegion = ControlSetupLines + "n\n\n" + ControlRegion;


            // insert === CONTROLS === under === StartUp ===
            bool inserted = Insert_After_Line_ToFile(ProjectFile, "#endregion", "\n\t\t" + ControlRegion, out ErrorMsg);

            // move menu functions under === Menu ===
            inserted = Move_Menu_Items_To_MenuRegion(ProjectFile);  // move menu functions under === Menu ===

            // event handlers in designer for control events
            Insert_Control_Event_Handlers(ProjectFile);  // loop through available controls and assign event handlers to functions already created here


            // Add line to "Control_Setup()" startup function to configure the controls 
            foreach (string line in ControlSetup)
            {
                Inserted = Add_Control_Setup_Line(ProjectFile, line, out ErrorMsg, "Control_Setup");
            }


            //if (!Inserted) { UpdateList.Add("ERROR Inserting SetupScintilla(" + control + " : " + ErrorMsg); }
            //if (Inserted) { UpdateList.Add("Inserted SetupScintilla(" + control + ""); }

            //bool AlreadyThere = Function_InFile(ProjectFile, "CreateStatusBar", out ErrorMsg, DebugExport);  // check to see if project file already has this function
            //if (AlreadyThere) { UpdateList.Add("CreateStatusBar() Already In Project - Skipping"); }

            //// display temp file with export code
            //string OutFile = @"C:\Users\jason\Google Drive\IMDB\SQLiter\Dynamic_Coder\bin\Debug\Project_Files\Code_Bin\Code Notes\Control_Region.cs";
            //ahk.FileDelete(OutFile);
            //ahk.FileAppend(ControlRegion, OutFile); 


            // insert custom using commands to project file (if they don't already exist)
            List<string> NewUsing = Custom_Using_List();
            Inserted = Insert_Using_Lines_ToFile(ProjectFile, NewUsing);
            if (!Inserted) { UpdateList.Add("ERROR Removing Extra Blank Lines : " + ErrorMsg); }
            if (Inserted) { UpdateList.Add("Removed Extra (More than 2) Blank Lines"); }


            // Remove Extra (More Than 2 Consecutive) Blank Lines From Project Code (trim blank spaces) 
            Inserted = Remove_Extra_Blank_Lines_FromFile(ProjectFile, out ErrorMsg, 2);
            if (!Inserted) { UpdateList.Add("ERROR Removing Extra Blank Lines : " + ErrorMsg); }
            if (Inserted) { UpdateList.Add("Removed Extra (More than 2) Blank Lines"); }


            return UpdateList;
        }


        /// <summary>
        /// insert ===StartUp=== Region underneath the form declaration (startup code region) to File
        /// </summary>
        /// <param name="ProjectFile"></param>
        /// <param name="ErrorMsg"></param>
        /// <param name="CodeUnderNewRegion"></param>
        /// <param name="RegionName"></param>
        /// <returns></returns>
        public bool Insert_Startup_Region(string ProjectFile, out string ErrorMsg, string CodeUnderNewRegion = "", string RegionName = "        #region ===[ Startup ]===")  
        {
            ErrorMsg = ahk.FileName(ProjectFile) + " Not Found - Unable To Insert";
            if (!File.Exists(ProjectFile)) { if (GlobalDebug) { ahk.MsgBox(ErrorMsg); } return false; }
            string CodeString = ahk.FileRead(ProjectFile);

            ErrorMsg = RegionName.Trim() + " Already Exists In " + ahk.FileName(ProjectFile);
            bool RegionAlreadyExists = Region_In_Code(CodeString, RegionName);
            if (RegionAlreadyExists) { return false; }


            // find and remove Initialize Component function to write into Startup Region
            string InitFunctionName = Find_Initialize_Function_FromCode(CodeString);  // returns name of function in FilePath containing "InitializeComponent();"
            string InitFunctionCode = Return_Function_FromCode(CodeString, InitFunctionName);  // find function by name in code string,  returns code in that function
            string CodeWithInitRemoved = Remove_Function_FromCode(CodeString, InitFunctionName);  // remove function from file, returns project code without that function

            // check to see if there is a Form_Load function, move to startup region if so
            string FormLoadName = InitFunctionName + "_Load";
            bool FormLoadExists = Function_InCode(CodeString, FormLoadName);
            string FormLoadCode = "";
            if (FormLoadExists)
            {
                FormLoadCode = Return_Function_FromCode(CodeString, FormLoadName);  // find function by name in code string,  returns code in that function
                CodeWithInitRemoved = Remove_Function_FromCode(CodeWithInitRemoved, FormLoadName);  // remove function from file, returns project code without that function
            }


            // check to see if any custom dll calls need to be added inside === Startup ===
            List<string> DllRefList = DLLReferenceList();
            string DLLRefs = "";
            foreach (string DLL in DllRefList)
            {
                if (!CodeWithInitRemoved.Contains(DLL))
                {
                    if (DLLRefs != "") { DLLRefs = DLLRefs + "\n\t\t" + DLL; }
                    if (DLLRefs == "") { DLLRefs = "\t\t" + DLL; }
                }
            }


            // assemble new code to write to StartUp Region
            string NewWriteCode = RegionName + "\n\n" + DLLRefs + "\n\n" + InitFunctionCode + "\n" + FormLoadCode + "\n" + CodeUnderNewRegion + "\n\n" + "        #endregion\n";



            // return all project code with new code inserted
            string NewProjectCode = Insert_Code_After_FormStart_ToCode(CodeWithInitRemoved, NewWriteCode);


            // Write New ProjectFile with Updated Code, Backing up the Project File First
            bool WroteNewFile = Write_ProjectFile_With_Backup(ProjectFile, NewProjectCode, out ErrorMsg);
            if (!WroteNewFile) { return false; }

            ErrorMsg = "Inserted Startup Region Into " + ahk.FileName(ProjectFile);
            return true;
        }

        /// <summary>
        /// insert code underneath the form declaration (startup code region) to Code String
        /// </summary>
        /// <param name="CodeString"></param>
        /// <param name="NewCode"></param>
        /// <returns></returns>
        public string Insert_Code_After_FormStart_ToCode(string CodeString, string NewCode)  
        {
            List<string> LineList = lst.Text_To_List(CodeString, false, false, false);

            string FormLine = Return_FormName_Line_FromCode(CodeString);
            string AllText = "";
            bool FormLineFound = false;

            foreach (string Line in LineList)
            {
                string WriteLine = Line;

                if (Line.Trim() == FormLine.Trim()) { FormLineFound = true; AllText = AllText + Environment.NewLine + WriteLine; continue; }

                if (FormLineFound)
                {
                    if (Line.Trim() != "{")
                    {
                        WriteLine = WriteLine + Environment.NewLine + Environment.NewLine + NewCode;
                        FormLineFound = false;
                    }
                }

                if (AllText != "") { AllText = AllText + Environment.NewLine + WriteLine; }
                if (AllText == "") { AllText = WriteLine; }
            }

            return AllText;
        }

        /// <summary>
        /// insert event handler events in projectfile for each control type
        /// </summary>
        /// <param name="ProjectFile"></param>
        public void Insert_Control_Event_Handlers(string ProjectFile)  
        {
            // ### DataGridView ###
            List<string> dataGridView_Handlers = DataGridView_Handlers();
            DesignerFile_Insert_Handler_List(ProjectFile, "DataGridView", "dataGridView1", dataGridView_Handlers);

            // ### TabControl ###
            List<string> tabControl_Handlers = TabControl_Handlers();
            DesignerFile_Insert_Handler_List(ProjectFile, "TabControl", "tabControl1", tabControl_Handlers);

            //// ### ListBox ###
            List<string> listBox_Handlers = ListBox_Handlers();
            DesignerFile_Insert_Handler_List(ProjectFile, "ListBox", "listBox1", listBox_Handlers);

            //// ### TreeView ###
            List<string> treeView_Handlers = TreeView_Handlers(true);
            DesignerFile_Insert_Handler_List(ProjectFile, "TreeView", "treeView1", treeView_Handlers);

            //// ### TextBox ###
            List<string> textBox_Handlers = TextBox_Handlers();
            DesignerFile_Insert_Handler_List(ProjectFile, "TextBox", "textBox1", textBox_Handlers);

            //// ### CheckBox ###
            List<string> checkBox_Handlers = CheckBox_Handlers();
            DesignerFile_Insert_Handler_List(ProjectFile, "CheckBox", "checkBox1", checkBox_Handlers);

            //// ### RadioButton ###
            List<string> radioButton_Handlers = RadioButton_Handlers();
            DesignerFile_Insert_Handler_List(ProjectFile, "RadioButton", "radioButton1", radioButton_Handlers);

            //// ### ComboBox ###
            List<string> comboBox_Handlers = ComboBox_Handlers();
            DesignerFile_Insert_Handler_List(ProjectFile, "ComboBox", "comboBox1", comboBox_Handlers);


            //ahk.MsgBox("Inserted Handler Events in Designer"); 
        }


        #endregion

        #region === Parse AppConfig ===

        /// <summary>
        /// Returns Path to App.Config File (either original project config or current exe's app config)
        /// </summary>
        /// <param name="ProjectConfig">If True, Returns Path to Current EXE's Project App.Config, Otherwise Current EXE's Config File</param>
        /// <returns></returns>
        public string AppConfig(bool ProjectConfig = false)
        {
            string path = "";

            // project's app config
            if (ProjectConfig)
            {
                string projectDir = Assembly.GetEntryAssembly().Location.Replace("\\bin\\Debug\\" + ahk.FileName(Assembly.GetEntryAssembly().Location), "");
                path = projectDir + "\\App.config";

                if (!File.Exists(path))
                {
                    // current exe app config
                    path = ahk.AppDir() + "\\" + ahk.FileName(Assembly.GetEntryAssembly().Location) + ".config";
                }
            }
            else
            {
                // current exe app config
                path = ahk.AppDir() + "\\" + ahk.FileName(Assembly.GetEntryAssembly().Location) + ".config";
            }

            return path;
        }

        /// <summary>
        /// Returns List of ConnNames from .AppConfig
        /// </summary>
        /// <param name="ProjectConfig">If True, Returns Path to Current EXE's Project App.Config, Otherwise Current EXE's Config File</param>
        /// <returns></returns>
        public List<string> ConnList(bool ProjectConfig = false)
        {
            Dictionary<string, string> ConnDict = AppConfig_ConnString(ProjectConfig);
            _Dict dict = new _Dict();
            return dict.KeyList(ConnDict);
        }

        /// <summary>
        /// Returns Dictionary of ConnName / ConnString from .AppConfig 
        /// </summary>
        /// <param name="ProjectConfig">If True, Returns Path to Current EXE's Project App.Config, Otherwise Current EXE's Config File</param>
        /// <returns></returns>
        public Dictionary<string, string> AppConfig_ConnString(bool ProjectConfig = false)
        {
            _Parse prs = new _Parse();
            Dictionary<string, string> Dict = new Dictionary<string, string>();

            string appconfig = AppConfig(ProjectConfig);

            string section = prs.XML_Text(ahk.FileRead(appconfig), "<connectionStrings>");
            List<string> lines = lst.Text_To_List(section);

            foreach(string line in lines)
            {
                string name = ahk.StringSplit(line, "\"", 1);
                string connstring = ahk.StringSplit(line, "\"", 3);
                Dict.Add(name, connstring);
                //ahk.MsgBox(name + "\n\n" + connstring);
            }

            return Dict;
        }


        /// <summary>
        /// Checks To See if Connection Name Found in App.Config
        /// </summary>
        /// <param name="ConnName">Connection Name to Check For</param>
        /// <param name="ProjectConfig">If True, Checks Current EXE's Project App.Config, Otherwise Current EXE's Config File</param>
        /// <returns></returns>
        public bool ConnExists(string ConnName, bool ProjectConfig = false)
        {
            List<string> list = ConnList(ProjectConfig);
            return lst.InList(list, ConnName);
        }



        /// <summary>
        /// Returns List of Setting Names from .AppConfig
        /// </summary>
        /// <param name="ProjectConfig">If True, Returns Path to Current EXE's Project App.Config, Otherwise Current EXE's Config File</param>
        /// <returns></returns>
        public List<string> SettingList(bool ProjectConfig = false)
        {
            Dictionary<string, string> ConnDict = AppConfig_Settings(ProjectConfig);
            _Dict dict = new _Dict();
            return dict.KeyList(ConnDict);
        }

        /// <summary>
        /// Returns Dictionary of App Setting Names / Values from .AppConfig
        /// </summary>
        /// <param name="ProjectConfig">If True, Returns Path to Current EXE's Project App.Config, Otherwise Current EXE's Config File</param>
        /// <returns></returns>
        public Dictionary<string, string> AppConfig_Settings(bool ProjectConfig = false)
        {
            _Parse prs = new _Parse();
            Dictionary<string, string> Dict = new Dictionary<string, string>();

            string appconfig = AppConfig(ProjectConfig);

            string section = prs.XML_Text(ahk.FileRead(appconfig), "<appSettings>");
            List<string> lines = lst.Text_To_List(section);

            foreach (string line in lines)
            {
                string name = ahk.StringSplit(line, "\"", 1);
                string settingValue = ahk.StringSplit(line, "\"", 3);
                Dict.Add(name, settingValue);
                ahk.MsgBox(name + "\n\n" + settingValue);
            }

            return Dict;
        }


        /// <summary>
        /// Checks To See if Setting Name Found in App.Config
        /// </summary>
        /// <param name="ConnName">Setting Name to Check For</param>
        /// <param name="ProjectConfig">If True, Checks Current EXE's Project App.Config, Otherwise Current EXE's Config File</param>
        /// <returns></returns>
        public bool SettingExists(string SettingName, bool ProjectConfig = false)
        {
            List<string> list = SettingList(ProjectConfig);
            return lst.InList(list, SettingName);
        }


        #endregion


        #region === Parse CSProj ===

        /// <summary>
        /// Returns List of .CS File References Included in ProjectFile
        /// </summary>
        /// <param name="CSProjPath"></param>
        /// <returns></returns>
        public List<string> IncludedCSFiles(string CSProjPath)
        {
            List<string> lines = lst.TextFile_To_List(CSProjPath, true, true, true);

            List<string> csFiles = new List<string>();

            foreach(string line in lines)
            {
                string name = "";
                if (line.Contains("<Compile Include="))
                {
                    name = line.Replace("<Compile Include=", "");
                    name = name.Replace("\"", "");
                    name = name.Replace(">", "").Trim();
                    csFiles.Add(name);
                }
            }

            return csFiles;
        }




        #endregion

    }

}

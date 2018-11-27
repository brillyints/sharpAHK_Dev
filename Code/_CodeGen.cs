using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sharpAHK;
using sharpAHK_Dev;
using AHKExpressions;
using TelerikExpressions;
using System.Diagnostics;
using Telerik.WinControls.UI;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;

// TODO: 

// Fix TableName when parsing from CreateSQL lines
// Account for binary column types
// remove ID from UPDATE SQL statement - cmd2.Parameters.AddWithValue(@"ID", obj.ID.ToString());
// add DateTime.Now.ToString() to update/insert with defined names (ex: currenttime)

namespace sharpAHK_Dev
{
    public partial class _Code  //code gen functions
    {

        #region === Populate: Code File Lists ===

        /// <summary> returns list of dll code projects</summary>
        /// <param name="All"> </param>
        public List<string> DLL_Code_FileList(bool All = false)
        {
            // ex:
            //      List<string> DllCodeList = code.DLL_Code_FileList(true);

            List<string> DLLCodeFileList = new List<string>();

            // DLL Code Project Files
            //DLLCodeFileList.Add(@"C:\Users\jason\Google Drive\Temp.cs"); 
            if (All)
            {
                /*
                                DLLCodeFileList.Add(@"c:\users\jason\google drive\imdb\sqliter\ahk_dll\autohotkey.interop\AHK.cs");
                                DLLCodeFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Dev_DLL\Dev.cs");
                                DLLCodeFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\jdlCRMOD\jdlCRMOD.cs");
                                DLLCodeFileList.Add(@"c:\users\jason\google drive\imdb\sqliter\ssi\SSI.dll.cs");
                                DLLCodeFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\SQLiter_DLL\SQLiter.cs");
                                DLLCodeFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\ZIP\ZIP.cs");
                */

                DLLCodeFileList.Add(@"C:\_Code\LucidProjects\ADBindex\sharpAHK\sharpAHK_Dll\AutoHotkey.Interop\_sharpAHK\AHK.cs");
                DLLCodeFileList.Add(@"C:\_Code\LucidProjects\ADBindex\sharpAHK_Dev\sharpAHK_Dev.cs");
            }
            else
            {
                DLLCodeFileList.Add(@"C:\_Code\LucidProjects\ADBindex\sharpAHK\sharpAHK_Dll\AutoHotkey.Interop\_sharpAHK\AHK.cs");
                DLLCodeFileList.Add(@"C:\_Code\LucidProjects\ADBindex\sharpAHK_Dev\sharpAHK_Dev.cs");
                //DLLCodeFileList.Add(@"c:\users\jason\google drive\imdb\sqliter\ssi\SSI.dll.cs");
                //DLLCodeFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\SQLiter_DLL\SQLiter.cs");
                //DLLCodeFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\ZIP\ZIP.cs");
            }


            return DLLCodeFileList;
        }

        /// <summary> returns list of project code files</summary>
        /// <param name="All"> </param>
        public List<string> Project_Code_FileList(bool All = false)
        {
            // ex:
            //      List<string> ProjectCodeList = code.Project_Code_FileList(true);
            /*
                        // list of all CS files in project dir
                        List<string> ProjectFileListSlow = lst.FileList(@"C:\Users\jason\Google Drive\IMDB", "*.cs", true, false, true);

                        List<string> ExcludeList = DLL_Code_FileList(true);
                        ProjectFileListSlow = lst.Remove_From_List(ProjectFileListSlow, ExcludeList);

                        return ProjectFileListSlow;
            */
            List<string> ProjectFileList = new List<string>();

            //Project / Tool Project Files 
            if (All)
            {
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Dynamic_Coder\Dynamic_Coder.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\SQLiter\SQLiter.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\SSIGrab\GrabIt\ScreenClip.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\SQLiter\TV_Db.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Parse_Code\Parse_Code.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Control\Control.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Code_Lib\Code_Lib.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\SSR_Manager\SSR_Manager.cs");
                //ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\dynamicCode\DynamicCode.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Email\Email.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\TextEdit\Text.Edit.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Support.Desk_Lessons\Lesson_Builder.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\SupportDesk\Support.Desk.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\SupportDesk\SSR.Viewer.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Updater\Updater.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Image_Sort\Image_Sort.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Movies\Movies.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\MPC\MPC.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\GoDaddyMedia\AHK.Test.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Control_Dev\Control_Dev.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\SQL_View\SQL_View.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\NPAD\NPAD.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Function_Dev\Function_Dev.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Function_Dev\DateTime.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Function_Dev\GridMenu.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Function_Dev\ListView.cs");
                ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Function_Dev\TreeMenu.cs");
            }
            else
            {
                ProjectFileList.Add(@"C:\_Code\LucidProjects\ADBindex\sharpAHK\sharpAHK_Dll\AutoHotkey.Interop\_sharpAHK\AHK.cs");
                ProjectFileList.Add(@"C:\_Code\LucidProjects\ADBindex\sharpAHK_Dev\sharpAHK_Dev.cs");

                //ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Dynamic_Coder\Dynamic_Coder.cs");
                //ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\SQLiter\SQLiter.cs");
                //ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\SSIGrab\GrabIt\ScreenClip.cs");
                //ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\SQLiter\TV_Db.cs");
                //ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Parse_Code\Parse_Code.cs");
                //ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Control\Control.cs");
                //ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Code_Lib\Code_Lib.cs");
                //ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\SSR_Manager\SSR_Manager.cs");
                //ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\dynamicCode\DynamicCode.cs");
                //ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Email\Email.cs");
                //ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\TextEdit\Text.Edit.cs");
                //ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Support.Desk_Lessons\Lesson_Builder.cs");
                //ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\SupportDesk\Support.Desk.cs");
                //ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\SupportDesk\SSR.Viewer.cs");
                //ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Updater\Updater.cs");
                //ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Image_Sort\Image_Sort.cs");
                //ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\Movies\Movies.cs");
                //ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\MPC\MPC.cs");
                //ProjectFileList.Add(@"C:\Users\jason\Google Drive\IMDB\SQLiter\GoDaddyMedia\AHK.Test.cs");
            }


            ProjectFileList.Sort();

            return ProjectFileList;
        }

        /// <summary> Populate DLL Code File List in TreeView control</summary>
        /// <param name="TreeView TV"> </param>
        /// <param name="CheckBoxes"> </param>
        /// <param name="Clear"> </param>
        public void DLL_Code_FileList_TV(TreeView TV, bool CheckBoxes = true, bool Clear = false)
        {
            if (Clear) { TV.Nodes.Clear(); }
            if (CheckBoxes) { TV.CheckBoxes = true; }

            List<string> FileList = DLL_Code_FileList(true);
            tv.Load_File_List(TV, FileList, Clear, "DLL Code");
            tv.Expand(TV);
        }

        /// <summary> Populate Project Code File List in TreeView control</summary>
        /// <param name="TreeView TV"> </param>
        /// <param name="CheckBoxes"> </param>
        /// <param name="Clear"> </param>
        public void Project_Code_FileList_TV(TreeView TV, bool CheckBoxes = true, bool Clear = false)
        {
            if (Clear) { TV.Nodes.Clear(); }
            if (CheckBoxes) { TV.CheckBoxes = true; }

            List<string> FileList = Project_Code_FileList(false);
            tv.Load_File_List(TV, FileList, Clear, "Project Code");
            tv.Expand(TV);
        }

        /// <summary> populate datatable with project file names / file paths to populate datagrid</summary>
        /// <param name="DataGridView dv"> </param>
        /// <param name="DllOnly"> </param>
        /// <param name="ProjectsOnly"> </param>
        public DataTable Project_File_Grid(DataGridView dv, bool DllOnly = false, bool ProjectsOnly = false)
        {
            // Here we create a DataTable with four columns.
            DataTable table = new DataTable();
            table.Columns.Add("Selected", typeof(bool));
            table.Columns.Add("FileName", typeof(string));
            table.Columns.Add("FilePath", typeof(string));

            List<string> dllList = DLL_Code_FileList(true);
            List<string> projectList = Project_Code_FileList(true);


            if (DllOnly)
            {
                // loop through dll file list - populate table rows
                foreach (string dll in dllList) { table.Rows.Add(false, ahk.FileName(dll), dll); }
            }

            if (ProjectsOnly)
            {
                // loop through project file list - populate table rows
                foreach (string project in projectList) { table.Rows.Add(false, ahk.FileName(project), project); }
            }


            if (DllOnly != true && ProjectsOnly != true)
            {
                // loop through dll file list - populate table rows
                foreach (string dll in dllList) { table.Rows.Add(false, ahk.FileName(dll), dll); }

                table.Rows.Add(false, "", ""); // add blank seperator line

                // loop through project file list - populate table rows
                foreach (string project in projectList) { table.Rows.Add(false, ahk.FileName(project), project); }
            }


            dv.DataSource = table;  // populate datagrid with values
            return table;
        }

        /// <summary> returns list of known/supported control types</summary>
        public List<string> Known_Control_Types()
        {
            List<string> knownControls = new List<string>();
            knownControls.Add("DataGridView");
            knownControls.Add("Button");
            knownControls.Add("TreeView");
            knownControls.Add("TextBox");
            knownControls.Add("TabControl");
            knownControls.Add("TabPage");
            knownControls.Add("ListBox");
            knownControls.Add("MenuStrip");
            knownControls.Add("ToolStripMenuItem");
            knownControls.Add("TableLayoutPanel");
            knownControls.Add("CheckBox");
            knownControls.Add("Label");
            knownControls.Add("PictureBox");
            knownControls.Add("Splitter");
            knownControls.Add("Panel");
            knownControls.Add("ToolStripSeparator");
            knownControls.Add("SaveFileDialog");
            knownControls.Add("BindingNavigator");
            knownControls.Add("OpenFileDialog");
            knownControls.Add("WebBrowser");
            knownControls.Add("ToolStripComboBox");
            knownControls.Add("ToolStripButton");
            knownControls.Add("ToolStripLabel");
            knownControls.Add("ToolStripTextBox");
            knownControls.Add("RichTextBox");
            knownControls.Add("ComboBox");
            knownControls.Add("ContextMenuStrip");
            knownControls.Add("CheckedListBox");
            knownControls.Add("DateTimePicker");
            knownControls.Add("MonthCalendar");
            knownControls.Add("FontDialog");
            knownControls.Add("PropertyGrid");
            knownControls.Add("ListView");
            knownControls.Add("ColorDialog");
            knownControls.Add("NotifyIcon");
            knownControls.Add("RadioButton");
            knownControls.Add("Scintilla");
            knownControls.Add("TreeViewFast");
            knownControls.Add("ProgressBar");
            return knownControls;
        }

        // populate lists to use to compare / insert missing dll references (initial project setup)


        /// <summary> returns list of custom dll references to insert into new projects</summary>
        public List<string> DLLReferenceList()
        {
            // ex:
            //      List<string> DllRefList = code.DLLReferenceList();

            List<string> DLLRefList = new List<string>();
            DLLRefList.Add("AHK.AHK ahk = new AHK.AHK();");

            DLLRefList.Add("Backup back = new Backup(); ");
            DLLRefList.Add("sharpAHK._Code code = new sharpAHK._Code();");
            DLLRefList.Add("Dev.Dict dict = new Dev.Dict(); ");
            DLLRefList.Add("ControlS.Grid grid = new ControlS.Grid();");
            DLLRefList.Add("_Images img = new _Images(); ");
            DLLRefList.Add("_Lists lst = new _Lists();");
            DLLRefList.Add("ControlS.mEnu menu = new ControlS.mEnu();");
            DLLRefList.Add("Dev.MPC mpc = new Dev.MPC();");
            DLLRefList.Add("Dev.Scintillaz scin = new Dev.Scintillaz();");
            DLLRefList.Add("Dev.Tabs tab = new Dev.Tabs(); ");
            DLLRefList.Add("Tags tags = new Tags();");
            DLLRefList.Add("ControlS.TreeViewControl tv = new ControlS.TreeViewControl();");

            DLLRefList.Add("_Database.SQL sql = new _Database.SQL();");
            DLLRefList.Add("_Database.SQLite sqlite = new _Database.SQLite();");
            DLLRefList.Add("SSI.SSI.CRM ssi = new SSI.SSI.CRM();");
            DLLRefList.Add("ZIP.ZIP zip = new ZIP.ZIP(); ");

            return DLLRefList;
        }

        /// <summary> returns list of custom using statements to insert into new project</summary>
        public List<string> Custom_Using_List()
        {
            // ex:
            //      List<string> MyUsingList = code.Custom_Using_List();

            List<string> UsingList = new List<string>();
            UsingList.Add("using AHK;");
            UsingList.Add("using Dev_DLL;");
            UsingList.Add("using SQLiter_DLL;");
            UsingList.Add("using SSI;");
            UsingList.Add("using ZIP;");

            return UsingList;
        }

        /// <summary> list of commands to show in treeview</summary>
        public List<string> MPC_CommandList()
        {
            List<string> lst = new List<string>();
            lst.Add("Launch");
            lst.Add("Play");
            lst.Add("Stop");
            lst.Add("Next");
            lst.Add("Previous");
            lst.Add("Close");

            return lst;
        }

        #endregion

        #region === CodeGen ===

        #region === CodeGen: SQL Columns ===

        //// Enter SQL Connection String, Database Name, and Table Name
        //// Generates TXT Files to SaveDir and Launches the TXT File(s)
        ////   -Code to Return Data from SQL Table with ColumnNames as Variable Names
        ////   -Code to Update a SQL Table with ColumnNames as Variable Names
        ////   -Code to Insert into SQL Table with ColumnNames as Variable Names

        //  AHK.CodeGen codegen = new AHK.CodeGen();
        //  codegen.SQLColumns_To_Code(ahkGlobal.UserTempDir, T2SS, "T2SSPortal", "Integration_Sync_Log", true, true, true); 


        /// <summary>codegen function to generate sql return/insert/update commands from sql table column names</summary>
        /// <param name="ConnectString"> </param>
        /// <param name="ConnName"> </param>
        /// <param name="LoadDbName"> </param>
        /// <param name="LoadTableName"> </param>
        /// <param name="WriteToTextFile"> </param>
        /// <param name="OpenAfterWrite"> </param>
        /// <param name="SaveDir"> </param>
        public string SQLColumns_To_Code(SqlConnection ConnectString, string ConnName = "GoDad", string LoadDbName = "sharpAHK", string LoadTableName = "FunctionLib", bool WriteToTextFile = false, bool OpenAfterWrite = true, string SaveDir = "\\TempDir")
        {
            bool ReturnCode = true;
            bool InsertCode = true;
            bool UpdateCode = true;

            DataTable ColumnNames = sql.Return_ColumnNamesDT(ConnectString, LoadDbName, LoadTableName);

            string codeOut = "";

            //=== Output Options ====
            if (ReturnCode) { codeOut = ColumnNames_To_Code(ColumnNames, LoadDbName, LoadTableName, "ReturnSQLValues", ConnName, "dbo", false, false, SaveDir); } // code to return sql values from db
            if (InsertCode) { codeOut = codeOut + Environment.NewLine + Environment.NewLine + ColumnNames_To_Code(ColumnNames, LoadDbName, LoadTableName, "InsertIntoSQL", ConnName, "dbo", false, false, SaveDir); }  // insert statement from column names
            if (UpdateCode) { codeOut = codeOut + Environment.NewLine + Environment.NewLine + ColumnNames_To_Code(ColumnNames, LoadDbName, LoadTableName, "UpdateSQL", ConnName, "dbo", false, false, SaveDir); } // update statements code gen
            if (UpdateCode) { codeOut = codeOut + Environment.NewLine + Environment.NewLine + ColumnNames_To_Code(ColumnNames, LoadDbName, LoadTableName, "UpdateInsert", ConnName, "dbo", false, false, SaveDir); } // update statements code gen

            return codeOut;
        }


        /// <summary> function used by SQLColumns_To_Code that generates the sql commands based on parameters passed in</summary>
        /// <param name="DataTable ColumnNames"> </param>
        /// <param name=" LoadDbName"> </param>
        /// <param name=" LoadTableName"> </param>
        /// <param name="CodeType"> </param>
        /// <param name="ConnName"> </param>
        /// <param name="DBO"> </param>
        /// <param name="WriteToTextFile"> </param>
        /// <param name="OpenAfterWrite"> </param>
        /// <param name="SaveDir"> </param>
        public string ColumnNames_To_Code(DataTable ColumnNames, string LoadDbName, string LoadTableName, string CodeType = "ReturnSQLValues", string ConnName = "T2SS", string DBO = "dbo", bool WriteToTextFile = false, bool OpenAfterWrite = true, string SaveDir = "\\TempDir")
        {
            bool WriteObjectCode = true;

            string ReturnSQLText = "";
            string InsertSQLText = "";
            string UpdateSQLText = "";
            string UpdateInsertText = "";

            bool WriteAsFunction = true;  //option to generate text for new function in c#

            //remove characters to create standard object name for class
            string ObjectName = ahk.StringReplace(LoadTableName, " ");
            ObjectName = ahk.StringReplace(ObjectName, ".");
            ObjectName = ahk.StringReplace(ObjectName, "_");


            //public static string UserDb { get; set; }
            // parse list of column names - create list of variable names to declare
            string VarDeclare = "";
            string ObjectDeclare = "";

            if (ColumnNames != null)
            {
                foreach (DataRow row in ColumnNames.Rows) // Loop over the rows.
                {
                    string DataSource = row["DataSource"].ToString();  // DataSource
                    string DbName = row["DbName"].ToString();  // DbName 
                    string TableName = row["TableName"].ToString();  // TableName 
                    string ColumnName = row["ColumnName"].ToString();  // Column Name 

                    if (!WriteObjectCode)
                    {
                        if (VarDeclare != "") { VarDeclare = VarDeclare + Environment.NewLine + "string " + ColumnName + " = \"\";"; }
                        if (VarDeclare == "") { VarDeclare = "string " + ColumnName + " = \"\";"; }
                    }

                    if (WriteObjectCode)
                    {
                        if (VarDeclare != "") { VarDeclare = VarDeclare + Environment.NewLine + "string " + ColumnName + " = \"\"; if (obj." + ColumnName + " != null) {" + ColumnName + " = obj." + ColumnName + ".ToString(); }"; }
                        if (VarDeclare == "") { VarDeclare = "string " + ColumnName + " = \"\"; if (obj." + ColumnName + " != null) {" + ColumnName + " = obj." + ColumnName + ".ToString(); }"; }
                    }

                    //string ReviewedBy = ""; if (obj.ReviewedBy.ToString() != null) { ReviewedBy = obj.ReviewedBy.ToString(); }

                    if (ObjectDeclare != "") { ObjectDeclare = ObjectDeclare + Environment.NewLine + "public string " + ColumnName + " { get; set; }"; }
                    if (ObjectDeclare == "") { ObjectDeclare = "public string " + ColumnName + " { get; set; }"; }
                }
            }


            ObjectDeclare = "public class " + ObjectName + Environment.NewLine + "{" + Environment.NewLine + ObjectDeclare + Environment.NewLine + "}";

            string FunctionHeader = "public void Update_Insert_" + ObjectName + "(" + ObjectName + " obj)" + Environment.NewLine + "{" + Environment.NewLine;


            // setup temp file to export code to 
            // if user doesn't provide directory, defaults to and creates temp dir under application
            string TempFile = "";
            if (SaveDir == "\\TempDir") { SaveDir = ahk.AppDir() + "\\ExportTemp"; TempFile = SaveDir + "\\Return_SQL_Values_" + LoadTableName + ".txt"; }
            if (SaveDir != "\\TempDir") { TempFile = SaveDir + "\\CodeExport_ReturnSQLValues_" + LoadTableName + ".txt"; }

            ahk.FileCreateDir(SaveDir);  // create export directory if folder doesn't already exist

            // Convert SQL Table Names to SQL Return Function Code
            if (CodeType == "ReturnSQLValues")
            {
                string HeaderText = "";
                if (WriteAsFunction)
                {
                    HeaderText = "public void ReturnSQLValues_" + LoadTableName + "()" + Environment.NewLine + "{" + Environment.NewLine;
                }

                string ReturnResultsCode = "";
                HeaderText = HeaderText + "//=======================================" + Environment.NewLine + "// Search SQL Table - Parse Results" + Environment.NewLine + "//=======================================";
                string SQLcommand = "string SQLcommand = \"Select * From [" + LoadDbName + "].[" + DBO + "].[" + LoadTableName + "]\";" + Environment.NewLine + "SqlDataAdapter SQLconnect = new SqlDataAdapter(SQLcommand, " + ConnName + ");" + Environment.NewLine + "DataTable SQLresults = new DataTable();";
                string WriteCode = HeaderText + Environment.NewLine + SQLcommand + Environment.NewLine + Environment.NewLine;

                // parse list of column names
                if (ColumnNames != null)
                {
                    foreach (DataRow row in ColumnNames.Rows) // Loop over the rows.
                    {
                        string DataSource = row["DataSource"].ToString();  // DataSource
                        string DbName = row["DbName"].ToString();  // DbName 
                        string TableName = row["TableName"].ToString();  // TableName 
                        string ColumnName = row["ColumnName"].ToString();  // Column Name 


                        //ReturnSSR.Type = SQLresults.Rows[0]["SSRType"].ToString();  //returns a string result
                        string ReturnCodeVarPrefix = "string "; // text before return variable name 
                        string ReturnLine = ReturnCodeVarPrefix + ColumnName + " = SQLresults.Rows[iIndex][\"" + ColumnName + "\"].ToString();";
                        ReturnResultsCode = ReturnResultsCode + Environment.NewLine + ReturnLine;
                    }
                }


                string ForEachLine = "int iIndex = 0;" + Environment.NewLine + "foreach (DataRow row in SQLresults.Rows)" + Environment.NewLine + "{" + Environment.NewLine + ReturnResultsCode + Environment.NewLine + Environment.NewLine + "iIndex++;" + Environment.NewLine + "}";
                string TryCatch = "try" + Environment.NewLine + "{" + Environment.NewLine + "SQLconnect.Fill(SQLresults);" + Environment.NewLine + "if (SQLresults.Rows.Count < 1)" + Environment.NewLine + "{" + Environment.NewLine + "// no results returned" + Environment.NewLine + "}" + Environment.NewLine + "else" + Environment.NewLine + "{" + Environment.NewLine + ForEachLine + Environment.NewLine + "}" + Environment.NewLine + "}" + Environment.NewLine + "catch (SqlException ex)" + Environment.NewLine + "{" + Environment.NewLine + "// MessageBox.Show(ex.ToString());" + Environment.NewLine + "}" + Environment.NewLine + Environment.NewLine + ConnName + ".Close();";
                WriteCode = WriteCode + TryCatch + Environment.NewLine;

                if (WriteAsFunction) { WriteCode = WriteCode + "}"; }


                if (WriteToTextFile)  // option to write code expor to temp text file that launches after generating
                {
                    ahk.FileDelete(TempFile);
                    ahk.FileAppend(WriteCode, TempFile, 2);
                    if (OpenAfterWrite) { ahk.Run(TempFile); }
                }

                return WriteCode;  // return code as string
            }

            // Convert SQL Table Names to SQL Insert Function
            if (CodeType == "InsertIntoSQL" || CodeType == "UpdateInsert")
            {
                // setup temp file to export code to 
                TempFile = SaveDir + "\\Insert_Into_SQL_" + LoadTableName + ".txt";

                string HeaderText = "";
                if (WriteAsFunction)
                {
                    HeaderText = "public void InsertSQLValues_" + LoadTableName + "()" + Environment.NewLine + "{" + Environment.NewLine;
                }
                HeaderText = HeaderText + "//=======================================" + Environment.NewLine + "// INSERT Values into SQL Table" + Environment.NewLine + "//=======================================";

                string WriteCode = HeaderText + Environment.NewLine;

                string VarList = "";
                string VarListValues = "";
                string InsertLines = "";

                if (ColumnNames != null)
                {
                    foreach (DataRow row in ColumnNames.Rows) // Loop over the rows.
                    {
                        string ColumnName = row["ColumnName"].ToString();  // Column Name 

                        if (ColumnName == "") { continue; }

                        if (VarList != "") { VarList = VarList + "," + ColumnName; }
                        if (VarList == "") { VarList = ColumnName; }

                        if (VarListValues != "") { VarListValues = VarListValues + ", @" + ColumnName; }
                        if (VarListValues == "") { VarListValues = "@" + ColumnName; }

                        if (InsertLines != "") { InsertLines = InsertLines + "cmd1.Parameters.AddWithValue(@\"" + ColumnName + "\", " + ColumnName + ");" + Environment.NewLine; }
                        if (InsertLines == "") { InsertLines = "cmd1.Parameters.AddWithValue(@\"" + ColumnName + "\", " + ColumnName + ");" + Environment.NewLine; }
                    }
                }


                //cmd2.Parameters.AddWithValue("@SSRNumber", SSR.SSRNumber.ToString());
                //VarListValues = "\"" + VarListValues + "\"";

                string SQLcommand = "string SQLcommand = \"INSERT INTO [" + LoadDbName + "].[" + DBO + "].[" + LoadTableName + "] (" + VarList + ") VALUES(" + VarListValues + ")\";";


                WriteCode = WriteCode + SQLcommand + Environment.NewLine + "SqlCommand cmd1 = new SqlCommand(SQLcommand, " + ConnName + ");" + Environment.NewLine + Environment.NewLine + InsertLines;

                string ExecuteLine = "if (" + ConnName + ".State == ConnectionState.Closed) { " + ConnName + ".Open(); }" + Environment.NewLine + Environment.NewLine + "try" + Environment.NewLine + "{" + Environment.NewLine + "cmd1.ExecuteNonQuery();" + Environment.NewLine + "}" + Environment.NewLine + "catch (SqlException ex)" + Environment.NewLine + "{" + Environment.NewLine + "// MessageBox.Show(ex.ToString());" + Environment.NewLine + "}";

                WriteCode = WriteCode + Environment.NewLine + ExecuteLine + Environment.NewLine;

                if (WriteAsFunction) { WriteCode = WriteCode + "}"; }

                InsertSQLText = WriteCode;

                if (CodeType != "UpdateInsert")
                {
                    if (WriteToTextFile)  // option to write code expor to temp text file that launches after generating
                    {
                        ahk.FileDelete(TempFile);
                        ahk.FileAppend(WriteCode, TempFile, 2);
                        if (OpenAfterWrite) { ahk.Run(TempFile); }
                    }

                    return WriteCode;  // return code as string
                }

            }

            // Convert SQL Table Names to SQL UPDATE Function
            if (CodeType == "UpdateSQL" || CodeType == "UpdateInsert")
            {
                // setup temp file to export code to 
                TempFile = SaveDir + "\\Update_SQL_" + LoadTableName + ".txt";


                string HeaderText = "";
                if (WriteAsFunction)
                {
                    HeaderText = "public void UpdateSQLValues_" + LoadTableName + "()" + Environment.NewLine + "{" + Environment.NewLine;
                }
                HeaderText = HeaderText + "//=======================================" + Environment.NewLine + "// UPDATE Values into SQL Table" + Environment.NewLine + "//=======================================";


                string WriteCode = HeaderText + Environment.NewLine;

                string VarList = "";
                if (ColumnNames != null)
                {
                    foreach (DataRow row in ColumnNames.Rows) // Loop over the rows.
                    {
                        string ColumnName = row["ColumnName"].ToString();  // Column Name 

                        if (ColumnName == "") { continue; }

                        if (VarList != "") { VarList = VarList + ", " + ColumnName + "= \'\" + " + ColumnName + " + \"'"; }
                        if (VarList == "") { VarList = "SET " + ColumnName + "= \'\" + " + ColumnName + " + \"'"; }
                    }
                }


                string SQLLine = "string SQLLine = \"UPDATE [" + LoadDbName + "].[" + DBO + "].[" + LoadTableName + "] " + VarList + " WHERE [CONDITION HERE]\";";
                string ExecuteUpdate = "SqlCommand cmd2 = new SqlCommand(SQLLine, " + ConnName + ");" + Environment.NewLine + "if (" + ConnName + ".State == ConnectionState.Closed) { " + ConnName + ".Open(); }" + Environment.NewLine + "int recordsAffected = 0;" + Environment.NewLine + "try" + Environment.NewLine + "{" + Environment.NewLine + "recordsAffected = cmd2.ExecuteNonQuery();" + Environment.NewLine + "}" + Environment.NewLine + "catch (SqlException ex)" + Environment.NewLine + "{" + Environment.NewLine + "// MessageBox.Show(ex.ToString());" + Environment.NewLine + "}" + Environment.NewLine + "" + ConnName + ".Close();";
                WriteCode = WriteCode + Environment.NewLine + SQLLine + Environment.NewLine + Environment.NewLine + ExecuteUpdate + Environment.NewLine;
                if (WriteAsFunction) { WriteCode = WriteCode + "}"; }

                UpdateSQLText = WriteCode;

                if (CodeType != "UpdateInsert")
                {
                    if (WriteToTextFile)  // option to write code expor to temp text file that launches after generating
                    {
                        ahk.FileDelete(TempFile);
                        ahk.FileAppend(WriteCode, TempFile, 2);
                        if (OpenAfterWrite) { ahk.Run(TempFile); }
                    }
                }
            }


            if (CodeType == "UpdateInsert")  //update or insert into table if update fails
            {
                TempFile = SaveDir + "\\Update_Insert_SQL_" + LoadTableName + ".txt"; ahk.FileDelete(TempFile);

                if (WriteObjectCode)
                {
                    UpdateInsertText = ObjectDeclare + Environment.NewLine + Environment.NewLine + FunctionHeader + VarDeclare + Environment.NewLine + Environment.NewLine + UpdateSQLText + Environment.NewLine + Environment.NewLine + "if (recordsAffected == 0)" + Environment.NewLine + "{" + Environment.NewLine + InsertSQLText + Environment.NewLine + "}" + Environment.NewLine + "}";
                }

                if (!WriteObjectCode)
                {
                    UpdateInsertText = VarDeclare + Environment.NewLine + Environment.NewLine + UpdateSQLText + Environment.NewLine + Environment.NewLine + "if (recordsAffected == 0)" + Environment.NewLine + "{" + Environment.NewLine + InsertSQLText + Environment.NewLine + "}";
                }

                if (WriteToTextFile)  // option to write code expor to temp text file that launches after generating
                {
                    ahk.FileDelete(TempFile);
                    ahk.FileAppend(UpdateInsertText, TempFile, 2);
                    if (OpenAfterWrite) { ahk.Run(TempFile); }
                }

                return UpdateInsertText;  // return code as string
            }





            return "Parameter Not Found In Function";
        }


        #endregion

        #region === CodeGen: DataGridView ===
        // GridView CodeGen

        /// <summary> codegen: using DataGridView Columns, generate SQLite Insert Statement Using Column Names as Variables</summary>
        /// <param name="DataGridView dv"> </param>
        /// <param name="TableName"> </param>
        /// <param name="ShowAfter"> </param>
        public string GridView_CodeGen_InsertLine(DataGridView dv, string TableName = "TableName", bool ShowAfter = true)
        {
            // string InsertLine = "INSERT into " + TableName + " (FileName, NameSpace, Class, Function, Type, LineNum, Tags, Comments, TimeStamp, FunctionLine, FunctionText, FilePath, Parent, DateAdded, FID) VALUES ('" + FileName + "', '" + NameSpaceName + "', '" + ClassName + "', '" + FunctionName + "', '" + REgionName + "', '" + LineCount.ToString() + "', '" + Tags + "', '" + Comments + "', '" + DateTime.Now + "', '" + CodeReturn + "', '" + FunctionText + "', '" + FilePath + "', '" + Parent + "', '" + DateTime.Now + "', '" + FID.ToString() + "')";
            //('" + FileName + "', '" + NameSpaceName + "', '" +
            //INSERT into [FunctionLib] (Flagged, FileName, NameSpace) VALUES (" false ", "true", "name");

            string OutString = "INSERT into [" + TableName + "] (";
            string Varlist = "(";

            List<string> ColumnList = grid.Column_Names(dv);
            int i = 0;
            foreach (string Col in ColumnList)
            {
                i++;

                if (i > 3) { break; }

                OutString = OutString + Col + ", ";

                if (i == 1) { Varlist = Varlist + "'" + Col + "', '"; }
                if (i != 1) { Varlist = Varlist + Col + "', '"; }
            }

            Varlist = Varlist.Trim();
            Varlist = ahk.TrimLast(Varlist); // trim extra comma
            Varlist = Varlist.Trim();
            Varlist = ahk.TrimLast(Varlist); // trim extra comma
            OutString = OutString.Trim();
            OutString = ahk.TrimLast(OutString); // trim extra comma
            OutString = OutString + ") VALUES " + Varlist + ");";


            if (ShowAfter) { ahk.MsgBox(OutString); }

            return OutString;


            //SQLiteConnection conn = sqlite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

            //int FID = 0;
            //if (BulkInsert)
            //{
            //    if (conn.State == ConnectionState.Closed) { conn.Open(); } //open SQLite connection
            //}

            //using (var cmd = new SQLiteCommand(conn))
            //{
            //    using (var transaction = conn.BeginTransaction())
            //    {
            //        foreach (string line in lines)
            //        {
            //            LineCount++;

            //            FunctionText = ahk.StringReplace(FunctionText, @"'", @"''");
            //            string InsertLine = "INSERT into " + TableName + " (FileName, NameSpace, Class, Function, Type, LineNum, Tags, Comments, TimeStamp, FunctionLine, FunctionText, FilePath, Parent, DateAdded, FID) VALUES ('" + FileName + "', '" + NameSpaceName + "', '" + ClassName + "', '" + FunctionName + "', '" + REgionName + "', '" + LineCount.ToString() + "', '" + Tags + "', '" + Comments + "', '" + DateTime.Now + "', '" + CodeReturn + "', '" + FunctionText + "', '" + FilePath + "', '" + Parent + "', '" + DateTime.Now + "', '" + FID.ToString() + "')";


            //                if (!BulkInsert)
            //                {
            //                    string UpdateLine = "UPDATE " + TableName + " set LineNum = '" + LineCount.ToString() + "', Comments = '" + Comments + "', TimeStamp = '" + DateTime.Now + "', Type = '" + REgionName + "', FunctionLine = '" + CodeReturn + "', FunctionText = '" + FunctionText + "', FilePath = '" + FilePath + "', Parent = '" + Parent + "', FID = '" + FID.ToString() + "' WHERE Function = '" + FunctionName + "' AND FileName = '" + FileName + "'";
            //                    if (conn.State == ConnectionState.Open) { conn.Close(); } //close existing connection if needed
            //                    bool Updated = sqlite.Execute(DbFile, UpdateLine);  // Update Table
            //                    if (!Updated) { Updated = sqlite.Execute(DbFile, InsertLine); }  // insert into a Table
            //                }

            //                if (BulkInsert)
            //                {
            //                    cmd.CommandText = InsertLine;
            //                    cmd.ExecuteNonQuery();
            //                }

            //        }

            //        if (BulkInsert)
            //        {
            //            transaction.Commit();  // write insert into SQLite 
            //            conn.Close();
            //        }
            //    }
            //}


        }


        /// <summary>codegen: use datagridview columns to generate HideColumn functions</summary>
        /// <param name="DataGridView dv"> </param>
        /// <param name="ShowAfter"> </param>
        public string Generate_HideColumn_Function_From_DataGridColumns(DataGridView dv, bool ShowAfter = false)
        {
            string OutString = "public void Hide_Columns(DataGridView dv)" + Environment.NewLine + "{" + Environment.NewLine;

            List<string> ColumnList = grid.Column_Names(dv);

            foreach (string Col in ColumnList)
            {
                OutString = OutString + "dv.Columns[\"" + Col + "\"].Visible = false;" + Environment.NewLine;
            }

            OutString = OutString + "}";

            if (ShowAfter) { ahk.MsgBox(OutString); }

            return OutString;
        }

        /// <summary> codegen: use datagridview columns to generate ShowColumn functions</summary>
        /// <param name="DataGridView dv"> </param>
        /// <param name="ShowAfter"> </param>
        public string Generate_ShowColumn_Function_From_DataGridColumns(DataGridView dv, bool ShowAfter = false)
        {
            string OutString = "public void Show_Columns(DataGridView dv)" + Environment.NewLine + "{" + Environment.NewLine;

            List<string> ColumnList = grid.Column_Names(dv);

            foreach (string Col in ColumnList)
            {
                OutString = OutString + "dv.Columns[\"" + Col + "\"].Visible = true;" + Environment.NewLine;
            }

            OutString = OutString + "}";

            if (ShowAfter) { ahk.MsgBox(OutString); }

            return OutString;
        }

        /// <summary> codegen: generate sql select statements from datagridview column names</summary>
        /// <param name="DataGridView dv"> </param>
        /// <param name="ShowAfter"> </param>
        public string SQL_SelectStatement_From_DataGridColumns(DataGridView dv, bool ShowAfter = false)
        {
            string OutString = "SELECT ";

            List<string> ColumnList = grid.Column_Names(dv);

            foreach (string Col in ColumnList)
            {
                OutString = OutString + Col + ", ";
            }

            OutString = OutString.Trim();
            OutString = ahk.TrimLast(OutString); // trim extra comma

            OutString = OutString + " FROM [DATABASE_TABLE]";

            if (ShowAfter) { ahk.MsgBox(OutString); }

            return OutString;
        }

        #endregion

        #region === Duplicate Code ===

        /// <summary> take user code section and replace [NUM] with incrementing #</summary>
        /// <param name="CodeToRepeat"> </param>
        /// <param name="StartNum"> </param>
        /// <param name="LoopCount"> </param>
        public string Duplicate_Code(string CodeToRepeat, int StartNum = 1, int LoopCount = 5)
        {
            // macros; 
            // 
            // "[NUM]" = StartNumber (increases by 1 each loop)
            // "[NUM+1]" = StartNumber + 1 (one more than the start number each loop)

            if (!CodeToRepeat.Contains("[NUM]") || !CodeToRepeat.Contains("[NUM"))
            { ahk.MsgBox("[NUM]" + " Not In CodeToRepeat"); return ""; }

            string OutCode = "";

            string NewLine = CodeToRepeat;
            int counter2 = StartNum;
            int i = 0;
            do
            {
                NewLine = CodeToRepeat;  // reset var with start text to replace

                //TODO: Expand this function with RegEx statements to populate all numbers
                if (NewLine.Contains("[NUM+1]"))  // macro for startnumber + 1 
                {
                    counter2 = StartNum; // add one to existing index count
                    counter2++;
                    NewLine = ahk.StringReplace(NewLine, "[NUM+1]", counter2.ToString());  // repace this value in code
                }
                if (NewLine.Contains("[NUM-1]"))  // macro for startnumber - 1 
                {
                    counter2 = StartNum; // add one to existing index count
                    counter2--;
                    NewLine = ahk.StringReplace(NewLine, "[NUM-1]", counter2.ToString());  // repace this value in code
                }

                NewLine = ahk.StringReplace(NewLine, "[NUM]", StartNum.ToString());


                StartNum++;

                if (OutCode != "") { OutCode = OutCode + Environment.NewLine + Environment.NewLine + NewLine; }
                if (OutCode == "") { OutCode = NewLine; }

                i++;
            } while (i < LoopCount);

            //if (CreateFunction)
            //{
            //    OutCode = "private void " + ahk.StringReplace(lblFunctionName.Text, "FUNCTION NAME:").Trim() + Environment.NewLine + "{" + Environment.NewLine + ReturnCode + Environment.NewLine + "}"; 
            //}


            return OutCode;
        }


        #endregion


        #endregion




    }
}

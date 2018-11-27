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
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Data.SqlClient;
using System.Data.SQLite;

namespace sharpAHK_Dev
{
    /// <summary>
    /// List Management Library | LucidMedod 
    /// </summary>
    public class _Lists
    {
        #region === Lists: Populate ===

        /// <summary>Converts text from a text file to list <string>
        /// <param name="TextString">Text String To Parse By New Line Into List Return</param>
        /// <param name="SkipBlankLines">Option To Skip Blank Lines in List Return</param>
        /// <param name="Trim">Option To Trim Each Line</param>
        /// <param name="SkipCommentLines">Skip Lines Starting with '//' (For Excluding C# Comments)</param>
        public List<string> Text_To_List(string TextString, bool SkipBlankLines = true, bool Trim = true, bool SkipCommentLines = true)
        {
            _AHK ahk = new _AHK();
            List<string> list = new List<string>();

            if (TextString == null) { return list; }

            // parse by new line
            {
                // Creates new StringReader instance from System.IO
                using (StringReader reader = new StringReader(TextString))
                {
                    // Loop over the lines in the string.
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (SkipCommentLines)
                        {
                            string First2 = ahk.FirstCharacters(line, 2); // skip over lines if they are comments
                            if (First2 == @"//") { continue; }
                        }

                        string writeline = line;

                        if (Trim) { writeline = line.Trim(); } // trim leading spaces

                        if (SkipBlankLines) { if (writeline == "") { continue; } }

                        list.Add(writeline);
                    }
                }

            }
            return list;
        }

        /// <summary>Converts String of Numbers Separted by New Lines to List INT</summary>
        /// <param name="TextString">String of numbers separated by New Lines</param>
        /// <param name="SkipBlankLines">Option to Skip Blank Lines/Entries in List</param>
        /// <param name="SkipCommentLines">Option to Skip Lines Starting with '//' for Excluding C# Comments</param>
        public List<int> Text_To_ListInt(string TextString, bool SkipBlankLines = true, bool SkipCommentLines = true)
        {
            _AHK ahk = new _AHK();
            List<int> list = new List<int>();
            bool Trim = true;

            // parse by new line
            {
                // Creates new StringReader instance from System.IO
                using (StringReader reader = new StringReader(TextString))
                {
                    // Loop over the lines in the string.
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (SkipCommentLines)
                        {
                            string First2 = ahk.FirstCharacters(line, 2); // skip over lines if they are comments
                            if (First2 == @"//") { continue; }
                        }

                        string writeline = line;

                        if (Trim) { writeline = line.Trim(); } // trim leading spaces

                        if (SkipBlankLines) { if (writeline == "") { continue; } }

                        int WriteInt = ahk.ToInt(writeline);  // convert string from text to int

                        list.Add(WriteInt);
                    }
                }

            }
            return list;
        }

        /// <summary>Read text file, return list <string></summary>
        /// <param name="FilePath"> </param>
        /// <param name="SkipBlankLines"> </param>
        /// <param name="Trim"> </param>
        /// <param name="SkipCommentLines"> </param>
        public List<string> TextFile_To_List(string FilePath, bool SkipBlankLines = true, bool Trim = true, bool SkipCommentLines = true)
        {
            _AHK ahk = new _AHK();

            if (File.Exists(FilePath))
            {
                string ParseCode = ahk.FileRead(FilePath);

                List<string> list = Text_To_List(ParseCode, SkipBlankLines, Trim, SkipCommentLines);
                return list;
            }
            else
            {
                return null;
            }
        }

        /// <summary>Read Text File, Return List INT</summary>
        /// <param name="FilePath">Path to File to Read and Convert to List</param>
        public List<int> TextFile_To_ListInt(string FilePath)
        {
            _AHK ahk = new _AHK();
            string ListTxt = ahk.FileRead(FilePath);
            List<int> list = new List<int>();

            // parse by new line
            {
                string[] lines = ListTxt.Split(Environment.NewLine.ToCharArray());
                foreach (string line in lines)
                {
                    string First2 = ahk.FirstCharacters(line, 2); // skip over lines if they are comments
                    if (First2 == @"//") { continue; }
                    string writeline = line.Trim();  // trim leading spaces
                    if (writeline == "") { continue; }
                    int lineInt = ahk.ToInt(writeline);  // convert line to integer
                    list.Add(lineInt);
                }
            }
            return list;
        }

        /// <summary>Convert Array[] to List</summary>
        /// <param name="string[] arr"> </param>
        public List<string> Array_To_List(string[] arr)
        {
            List<string> list = new List<string>(arr); // Copy array values to List.
            return list;
        }

        /// <summary>Convert List to Array[]</summary>
        /// <param name="List<string> list"> </param>
        public string[] List_To_Array(List<string> list)
        {
            string[] s = list.ToArray();
            return s;
        }

        /// <summary>SQLite search to list <string></summary>
        /// <param name="DbFile"> </param>
        /// <param name=" SearchLine"> </param>
        public List<string> SQLite_To_List(string DbFile, string SearchLine)
        {
            _Database.SQLite sqlite = new _Database.SQLite();

            SQLiteConnection m_dbConnection = sqlite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

            //string SearchLine = "Select Distinct [Tag_Group] from [" + TableName + "]";
            SQLiteDataReader reader = sqlite.ReturnSQLite(SearchLine, m_dbConnection);  // request data from DB
            List<string> TagGroups = new List<string>();

            if (reader == null) { return TagGroups; }

            while (reader.Read())    // loop through each row returned from select 
            {
                string Group = reader[0].ToString();
                TagGroups.Add(Group);
            }

            sqlite.Disconnect(m_dbConnection);  // free up db for other use

            return TagGroups;
        }

        /// <summary>SQLite search to list <int></summary>
        /// <param name="DbFile"> </param>
        /// <param name=" SearchLine"> </param>
        public List<int> SQLite_To_ListInt(string DbFile, string SearchLine)
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            _AHK ahk = new _AHK();

            SQLiteConnection m_dbConnection = sqlite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

            //string SearchLine = "Select Distinct [Tag_Group] from [" + TableName + "]";
            SQLiteDataReader reader = sqlite.ReturnSQLite(SearchLine, m_dbConnection);  // request data from DB
            List<int> TagGroups = new List<int>();

            while (reader.Read())    // loop through each row returned from select 
            {
                string Group = reader[0].ToString();
                int groupInt = ahk.ToInt(Group);
                TagGroups.Add(groupInt);
            }

            sqlite.Disconnect(m_dbConnection);  // free up db for other use

            return TagGroups;
        }

        /// <summary>Returns List<string> of .SQLITE Files in DirPath</summary>
        /// <param name="DirPath"> </param>
        /// <param name="Recurse"> </param>
        public List<string> SQLite_DbFile_List(string DirPath, bool Recurse = true)
        {
            List<string> myImageList = new List<string>();

            if (Recurse)  // search all directories under DirPath
            {
                string[] files = Directory.GetFiles(DirPath, "*.sqlite", SearchOption.AllDirectories);

                foreach (string file in files)  // loop through list of files and write file details to sqlite db
                {
                    myImageList.Add(file);
                }
            }


            if (!Recurse)  // only search the directory path, no sub directories
            {
                string[] files = Directory.GetFiles(DirPath, "*.sqlite", SearchOption.TopDirectoryOnly);

                foreach (string file in files)  // loop through list of files and write file details to sqlite db
                {
                    myImageList.Add(file);
                }
            }


            return myImageList;
        }

        /// <summary>SQLite Table Columns Names to List <string></summary>
        /// <param name="SQLiteFile"> </param>
        /// <param name=" TableName"> </param>
        public List<string> SQLite_ColumnList(string SQLiteFile, string TableName)
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            List<string> ColumnList = sqlite.Column_List(SQLiteFile, TableName);
            return ColumnList;
        }

        /// <summary>SQLite Table Names List <string></summary>
        /// <param name="SQLiteFile"> </param>
        public List<string> SQLite_TableList(string SQLiteFile)
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            List<string> TableList = sqlite.Table_List(SQLiteFile);
            return TableList;
        }

        /// <summary>SQL Search Resutls to List <string></summary>
        /// <param name="Connection"> </param>
        /// <param name=" SQLSearch"> </param>
        public List<string> SQL_To_List(SqlConnection Connection, string SQLSearch)
        {
            _Database.SQL sql = new _Database.SQL();
            DataTable dt = sql.GetDataTable(Connection, SQLSearch);

            List<string> ReturnValues = new List<string>();

            if (dt != null)
            {
                foreach (DataRow datarow in dt.Rows)
                {
                    ReturnValues.Add(datarow[0].ToString()); //save row text to list
                }
            }

            return ReturnValues;
        }

        /// <summary>SQL Search Resutls to List <int></summary>
        /// <param name="Connection"> </param>
        /// <param name=" SQLSearch"> </param>
        public List<int> SQL_To_ListInt(SqlConnection Connection, string SQLSearch)
        {
            _Database.SQL sql = new _Database.SQL();
            _AHK ahk = new _AHK();

            DataTable dt = sql.GetDataTable(Connection, SQLSearch);

            List<int> ReturnValues = new List<int>();

            if (dt != null)
            {
                foreach (DataRow datarow in dt.Rows)
                {
                    int AddValue = ahk.ToInt(datarow[0].ToString());  // convert return value to int from string
                    ReturnValues.Add(AddValue); //save row text to list
                }
            }

            Connection.Close();

            return ReturnValues;
        }


        /// <summary>Returns list of Dictionary Keys <string></summary>
        /// <param name="Dictionary<string"> </param>
        /// <param name=" string> dictionary"> </param>
        public List<string> Dict_KeyList(Dictionary<string, string> dictionary)
        {
            _Dict dict = new _Dict();
            List<string> returnlist = dict.KeyList(dictionary);
            return returnlist;
        }

        /// <summary>Returns list of Dictionary Keys <int></summary>
        /// <param name="Dictionary<int"> </param>
        /// <param name=" string> dictionary"> </param>
        public List<int> Dict_KeyListInt(Dictionary<int, string> dictionary)
        {
            _Dict dict = new _Dict();
            List<int> returnlist = dict.KeyListInt(dictionary);
            return returnlist;
        }


        /// <summary>Returns list of Dictionary Values <string></summary>
        /// <param name="Dictionary<string"> </param>
        /// <param name=" string> dictionary"> </param>
        public List<string> Dict_ValueList(Dictionary<string, string> dictionary)
        {
            _Dict dict = new _Dict();
            List<string> returnlist = dict.ValueList(dictionary);
            return returnlist;
        }

        /// <summary>Returns list of Dictionary Values <string></summary>
        /// <param name="Dictionary<string"> </param>
        /// <param name=" int> dictionary"> </param>
        public List<int> Dict_ValueListInt(Dictionary<string, int> dictionary)
        {
            _Dict dict = new _Dict();
            List<int> returnlist = dict.ValueListInt(dictionary);
            return returnlist;
        }


        //=== File Path Lists ===

        // ex: List<string> FileList = lst.DirList(file, "*.sqlite", true); 

        /// <summary>Returns List<string> of folders in directory path</summary>
        /// <param name="DirPath"> </param>
        /// <param name="SearchPattern"> </param>
        /// <param name="Recurse"> </param>
        /// <param name="FullPathReturn"> </param>
        public List<string> DirList(string DirPath, string SearchPattern = "*.*", bool Recurse = true, bool FullPathReturn = false)
        {
            _AHK ahk = new _AHK();
            List<string> FileList = ahk.DirList(DirPath, SearchPattern, Recurse, FullPathReturn);

            return FileList;
        }

        /// <summary>Returns List<string> of files in directory path</summary>
        /// <param name="DirPath"> </param>
        /// <param name="SearchPattern"> </param>
        /// <param name="Recurse"> </param>
        /// <param name="FileNameOnly"> </param>
        /// <param name="IncludeExt"> </param>
        public List<string> FileList(string DirPath, string SearchPattern = "*.*", bool Recurse = true, bool FileNameOnly = false, bool IncludeExt = true)
        {
            _AHK ahk = new _AHK();
            List<string> FileList = ahk.FileList(DirPath, SearchPattern, Recurse, FileNameOnly, IncludeExt);

            //    if (Recurse)
            //        {
            //        string[] array2 = Directory.GetFiles(DirPath, SearchPattern, SearchOption.AllDirectories);
            //        }

            //if (!Recurse)
            //{
            //    string[] array2 = Directory.GetFiles(DirPath, SearchPattern, SearchOption.TopDirectoryOnly);
            //}





            return FileList;
        }

        /// <summary>Returns List of Image File Paths in Directory Path (JPG, JPEG, GIF, ICO, PNG)</summary>
        /// <param name="DirPath">Directory Path to Search For Image Files</param>
        /// <param name="Recurse">Default (True) option to Search SubDirectories For Image Files</param>
        public List<string> FileList_Images(string DirPath, bool Recurse = true)
        {
            _AHK ahk = new _AHK();
            List<string> myImageList = new List<string>();

            if (!Directory.Exists(DirPath)) { return myImageList; } // return blank list if dir not found

            SearchOption recurse = SearchOption.AllDirectories;
            if (!Recurse) { recurse = SearchOption.TopDirectoryOnly; }

            string[] files = Directory.GetFiles(DirPath, "*.*", recurse);

            foreach (string file in files)  // loop through list of files and write file details to sqlite db
            {
                bool AddToList = ahk.isImage(file);

                // add image to list if valid image format
                if (AddToList) { myImageList.Add(file); }
            }

            return myImageList;
        }


        /// <summary>Returns List of Video File Paths in Directory Path (AVI, MKV, MPG, etc)</summary>
        /// <param name="DirPath">Directory Path to Search For Image Files</param>
        /// <param name="Recurse">Default (True) option to Search SubDirectories For Image Files</param>
        public List<string> FileList_Videos(string DirPath, bool Recurse = true)
        {
            _AHK ahk = new _AHK();
            List<string> myImageList = new List<string>();

            if (!Directory.Exists(DirPath)) { return myImageList; } // return blank list if dir not found

            SearchOption recurse = SearchOption.AllDirectories;
            if (!Recurse) { recurse = SearchOption.TopDirectoryOnly; }

            string[] files = Directory.GetFiles(DirPath, "*.*", recurse);

            foreach (string file in files)  // loop through list of files and write file details to sqlite db
            {
                bool AddToList = ahk.isVideo(file);

                // add image to list if valid image format
                if (AddToList) { myImageList.Add(file); }
            }

            return myImageList;
        }


        /// <summary>loop through multiple folders, return files meeting search criteria, sort files by FileName regardless of directory path, return sorted list of full file paths as list</summary>
        /// <param name="DirPath"> </param>
        /// <param name="ExtTypes"> </param>
        /// <param name="Recurse"> </param>
        public List<string> FileList_SortedAlpha_ByFileName(string DirPath, string ExtTypes = "*.*", bool Recurse = true)
        {
            _AHK ahk = new _AHK();
            if (!Directory.Exists(DirPath)) { return null; }

            string[] files = Directory.GetFiles(DirPath, ExtTypes, SearchOption.AllDirectories);

            if (!Recurse) { files = Directory.GetFiles(DirPath, ExtTypes, SearchOption.TopDirectoryOnly); }

            // sort list alpha first
            List<string> filelist = new List<string>(files);
            filelist.Sort();

            List<string> filelistSort = new List<string>();
            foreach (string fil in filelist)  // loop through list items
            {
                filelistSort.Add(ahk.FileName(fil) + "|" + fil);
            }
            filelistSort.Sort();

            List<string> filelistSorted = new List<string>();
            foreach (string fi in filelistSort)  // loop through listitems
            {
                string[] words = fi.Split('|');
                string fullPath = "";
                foreach (string word in words)
                {
                    fullPath = word;
                }
                filelistSorted.Add(fullPath);
            }

            return filelistSorted;
        }

        // TODO: Finish 

        /// <summary>returns List<string> of files in directory path modified today</summary>
        /// <param name="DirPath"> </param>
        /// <param name="SearchPattern"> </param>
        /// <param name="Recurse"> </param>
        /// <param name="FileNameOnly"> </param>
        /// <param name="IncludeExt"> </param>
        public List<string> FileList_Modified_Today(string DirPath, string SearchPattern = "*.*", bool Recurse = true, bool FileNameOnly = false, bool IncludeExt = true)
        {
            _AHK ahk = new _AHK();
            List<string> FileList = new List<string>();

            if (!Directory.Exists(DirPath)) { return null; }

            string[] files = null;
            if (Recurse) { files = Directory.GetFiles(DirPath, SearchPattern, SearchOption.AllDirectories); }
            if (!Recurse) { files = Directory.GetFiles(DirPath, SearchPattern, SearchOption.TopDirectoryOnly); }

            foreach (string file in files)  // loop through list of files and write file details to sqlite db
            {
                FileInfo info = new FileInfo(file);

                if (info.LastWriteTime.Date == DateTime.Today)  // if file modified today - add to list
                {
                    string addFile = file;
                    if (FileNameOnly) { addFile = ahk.FileName(file); }
                    if ((FileNameOnly) && (!IncludeExt)) { addFile = ahk.FileNameNoExt(file); }
                    FileList.Add(addFile);
                }

                //ahk.MsgBox(info.LastWriteTime.Date.ToString());

                //DateTime value = new DateTime(2010, 1, 18);
                //string dateCompare = DateTimeCompare(info.LastWriteTime.Date, DateTime.Today);
                //ahk.MsgBox(dateCompare); 

                //// file size
                //FileInfo info = new FileInfo("C:\\a");
                //long value = info.Length;
                //Console.WriteLine(value);

                // add file to list to return
                //FileList.Add(addFile);
            }

            return FileList;
        }

        // TODO: Finish 

        /// <summary>returns List<string> of files in directory path modified today</summary>
        /// <param name="DirPath"> </param>
        /// <param name=" DateTime Since"> </param>
        /// <param name="SearchPattern"> </param>
        /// <param name="Recurse"> </param>
        /// <param name="FileNameOnly"> </param>
        /// <param name="IncludeExt"> </param>
        public List<string> FileList_Modified_Since(string DirPath, DateTime Since, string SearchPattern = "*.*", bool Recurse = true, bool FileNameOnly = false, bool IncludeExt = true)
        {
            _AHK ahk = new _AHK();
            List<string> FileList = new List<string>();

            if (!Directory.Exists(DirPath)) { return null; }

            string[] files = null;
            if (Recurse) { files = Directory.GetFiles(DirPath, SearchPattern, SearchOption.AllDirectories); }
            if (!Recurse) { files = Directory.GetFiles(DirPath, SearchPattern, SearchOption.TopDirectoryOnly); }

            foreach (string file in files)  // loop through list of files and write file details to sqlite db
            {
                FileInfo info = new FileInfo(file);


                DateTime fileDate = info.LastWriteTime;

                if (fileDate > Since)
                {
                    string addFile = file;
                    if (FileNameOnly) { addFile = ahk.FileName(file); }
                    if ((FileNameOnly) && (!IncludeExt)) { addFile = ahk.FileNameNoExt(file); }
                    FileList.Add(addFile);
                }

                //string diff = DateTimeCompare(info.LastWriteTime.Date, Since.Date);

                ////if (diff == "Later" || diff == "Same")  // date is later or the same day
                //if (diff == "Later")  // date is later or the same day
                //{
                //    string addFile = file;
                //    if (FileNameOnly) { addFile = ahk.FileName(file); }
                //    if ((FileNameOnly) && (!IncludeExt)) { addFile = ahk.FileNameNoExt(file); }
                //    FileList.Add(addFile);
                //}

                //if (info.LastWriteTime.Date == Since.Date)  // if file modified today - add to list
                //{
                //    string addFile = file;
                //    if (FileNameOnly) { addFile = ahk.FileName(file); }
                //    if ((FileNameOnly) && (!IncludeExt)) { addFile = ahk.FileNameNoExt(file); }
                //    FileList.Add(addFile);
                //}

                //ahk.MsgBox(info.LastWriteTime.Date.ToString());

                //DateTime value = new DateTime(2010, 1, 18);
                //string dateCompare = DateTimeCompare(info.LastWriteTime.Date, DateTime.Today);
                //ahk.MsgBox(dateCompare); 

                //// file size
                //FileInfo info = new FileInfo("C:\\a");
                //long value = info.Length;
                //Console.WriteLine(value);

                // add file to list to return
                //FileList.Add(addFile);
            }

            return FileList;
        }


        // Drives

        /// <summary>returns list of drive letters (C:\ etc) visible on this pc </summary>
        public List<string> Drive_List()
        {
            List<string> Drives = new List<string>();

            foreach (System.IO.DriveInfo di in System.IO.DriveInfo.GetDrives())
                Drives.Add(di.Name);

            return Drives;
        }


        // CSV

        /// <summary>convert list to csv formatted string</summary>
        /// <param name="List<string> list"> </param>
        public string List_To_CSV(List<string> list)
        {
            string returnText = "";
            foreach (string item in list)
            {
                if (returnText != "") { returnText = returnText + ", " + item; }
                if (returnText == "") { returnText = item; }
            }

            return returnText;
        }

        /// <summary>convert csv to list<string></summary>
        /// <param name="CSV"> </param>
        /// <param name="ParseBy"> </param>
        public List<string> CSV_To_List(string CSV, string ParseBy = ",")
        {
            char myChar = ParseBy[0];
            List<string> CSV_List = new List<string>();
            string[] words = CSV.Split(myChar);
            foreach (string word in words)
            {
                CSV_List.Add(word);
            }

            return CSV_List;
        }

        // Controls

        /// <summary>Returns list of controls on a form as a Control list</summary>
        /// <param name="form"> </param>
        public List<Control> Control_List(Control form)
        {
            var controlList = new List<Control>();

            foreach (Control childControl in form.Controls)
            {
                // Recurse child controls.
                controlList.AddRange(Control_List(childControl));
                controlList.Add(childControl);
            }
            return controlList;
        }


        /// <summary>Returns List of ALL Processes Running on PC - If ProcessName provided then return all processes with that name</summary>
        /// <param name="ProcessName">Optional parameter to return all processes with this process name</param>
        public List<Process> Process_List(string ProcessName = "")
        {
            List<Process> ProcessList = new List<Process>();

            Process[] processlist = Process.GetProcesses();

            foreach (Process theprocess in processlist)
            {
                if (!String.IsNullOrEmpty(theprocess.MainWindowTitle))
                {
                    try
                    {
                        //sharpAHK.winInfo WinPositions = new sharpAHK.winInfo();
                        //WinPositions = ahk.WinGetPos("ahk_PID " + theprocess.MainWindowHandle);

                        if (ProcessName == "") { ProcessList.Add(theprocess); }  // return all processes if Process Name not provided

                        if (ProcessName != "") { if (theprocess.ProcessName == ProcessName) { ProcessList.Add(theprocess); } } // return all processes if Process Name not provided
                    }
                    catch
                    {
                    }
                }
            }

            return ProcessList;
        }

        /// <summary>Returns List of all WinTitles with ProcessName</summary>
        /// <param name="ProcessName">Name of process to seach for - Blank for ALL</param>
        public List<string> All_WinTitles_By_ProcessName(string processName = "mpc-hc64")
        {
            List<string> MPC_WinTitles = new List<string>();

            Process[] processlist = Process.GetProcesses();

            foreach (Process process in processlist)
            {
                if (!String.IsNullOrEmpty(process.MainWindowTitle))
                {
                    if (processName != "")
                    {
                        if (process.ProcessName == processName) { MPC_WinTitles.Add(process.MainWindowTitle); }
                    }
                    if (processName == "" || processName == null) // no process provided - return ALL
                    {
                        MPC_WinTitles.Add(process.MainWindowTitle);
                    }

                    //Console.WriteLine("Process: {0} ID: {1} Window title: {2}", process.ProcessName, process.Id, process.MainWindowTitle);
                }
            }

            return MPC_WinTitles;
        }

        /// <summary> Return list of dummy data to populate controls while testing, ex: UserName1 UserName2 UserName3 as List</summary>
        /// <param name="ExampleName">The leading text before a unique number is added </param>
        /// <param name="ListSize">Number of List values to return</param>
        /// <param name="spaceChar">Determines the space/character between ExampleName and #</param>
        public List<string> ReturnExamples(string ExampleName = "UserName", int ListSize = 10, string spaceChar = "_")
        {
            List<string> ex = new List<string>();
            int i = 0;
            do
            {
                ex.Add(ExampleName + spaceChar + i.ToString());
                i++;
            }
            while (i < ListSize);

            return ex;
        }


        #endregion

        #region === Lists: Display  ===

        //  Display Lists

        // List of Tables in SQLite File to GridView Display
        // Ex: lst.List_To_Grid(dataGridView1, sqlite.TableList(DbFile), "TABLES"); 

        /// <summary>
        /// Populate DataGridView from List
        /// </summary>
        /// <param name="dv"> </param>
        /// <param name="list"> </param>
        /// <param name="ListName"> </param>
        /// <param name="AddCheckBox"> </param>
        /// <returns>Returns Grid Row Count</returns>
        public int List_To_Grid(DataGridView dv, List<string> list, string ListName = "List_View", bool AddCheckBox = false)
        {
            //======= Create DataTable and Assign to DataGrid  =======
            DataTable dt = new DataTable();

            int i = 0;
            if (!AddCheckBox)
            {
                dt.Columns.Add(ListName, typeof(String));                     // Create Columns

                foreach (string item in list)
                {
                    dt.Rows.Add(new object[] { item });
                }
            }

            if (AddCheckBox)  // option to add checkboxes to first column of datagridview
            {
                dt.Columns.Add("Selected", typeof(bool));
                dt.Columns.Add(ListName, typeof(String));

                foreach (string item in list)
                {
                    dt.Rows.Add(new object[] { false, item });
                }
            }


            // Assign to Grid / Display (from any thread)
            if (dv.InvokeRequired) { dv.BeginInvoke((MethodInvoker)delegate () { dv.DataSource = dt; }); }
            else { dv.DataSource = dt; }

            return list.Count();
        }

        /// <summary>populate DataGridView from List <int></summary>
        /// <param name="dv"> </param>
        /// <param name=" List<int> list"> </param>
        /// <param name="ListName"> </param>
        public void List_To_GridInt(DataGridView dv, List<int> list, string ListName = "List_View")
        {
            //======= Create DataTable and Assign to DataGrid  =======
            DataTable dt = new DataTable();

            // Create Columns
            dt.Columns.Add(ListName, typeof(String));

            foreach (int item in list)
            {
                dt.Rows.Add(new object[] { item });
            }

            // Assign to Grid / Display
            dv.DataSource = dt;
        }

        /// <summary>populate TreeView from List <string></summary>
        /// <param name="TV"> </param>
        /// <param name="List<string> LoadList"> </param>
        /// <param name="ParentName"> </param>
        /// <param name="NoParent"> </param>
        /// <param name="cLearTV"> </param>
        public void List_To_TreeView(TreeView TV, List<string> LoadList, string ParentName = "List", bool NoParent = false, bool cLearTV = true)
        {
            _TreeViewControl tv = new _TreeViewControl();
            if (cLearTV) { TV.Nodes.Clear(); }

            if (NoParent) { tv.Load_List(TV, LoadList); }
            if (!NoParent) { tv.Load_List(TV, LoadList, ParentName); }
        }

        /// <summary>populate TreeView from List <int></summary>
        /// <param name="TV"> </param>
        /// <param name=" List<string> LoadList"> </param>
        /// <param name="ParentName"> </param>
        /// <param name="NoParent"> </param>
        /// <param name="cLearTV"> </param>
        public void List_To_TreeViewInt(TreeView TV, List<int> LoadList, string ParentName = "List", bool NoParent = false, bool cLearTV = true)
        {
            _TreeViewControl tv = new _TreeViewControl();
            if (cLearTV) { TV.Nodes.Clear(); }

            if (!NoParent) { tv.Load_List_Int(TV, LoadList, ParentName); }
            if (NoParent) { tv.Load_List_Int(TV, LoadList); }
        }


        /// <summary>Populate ComboBox from String List</summary>
        /// <param name="cb">ComboBox Control To Populate </param>
        /// <param name="LoadList">List to load in Combobox</param>
        public void List_To_ComboBox(ComboBox cb, List<string> LoadList, string TopEntry = "")
        {
            // option to add top entry in combobox display list
            if (TopEntry != "") { LoadList = AddToTop_List(LoadList, TopEntry); }

            //Setup data binding
            cb.DataSource = LoadList;
            cb.DisplayMember = "Name";
            //cb.ValueMember = "Value";
        }

        /// <summary>populate ComboBox from List <int></summary>
        /// <param name="ComboBox cb"> </param>
        /// <param name=" List<string> LoadList"> </param>
        public void List_To_ComboBoxInt(ComboBox cb, List<int> LoadList)
        {
            //Setup data binding
            cb.DataSource = LoadList;
            cb.DisplayMember = "Name";
            //cb.ValueMember = "Value";
        }

        /// <summary>populate ListBox from List <string> (from any thread)</summary>
        /// <param name="ListBox listbox"> </param>
        /// <param name=" List<string> LoadList"> </param>
        /// <param name="Clear"> </param>
        public void List_To_ListBox(ListBox listbox, List<string> LoadList, bool Clear = true)
        {
            if (Clear)  // option to clear out previous listbox values before adding
            {
                if (listbox.InvokeRequired) { listbox.BeginInvoke((MethodInvoker)delegate () { listbox.Items.Clear(); }); }
                else { listbox.Items.Clear(); }
            }

            //*/


            // update listbox values (from any thread) 
            if (listbox.InvokeRequired) { listbox.BeginInvoke((MethodInvoker)delegate () { listbox.Items.AddRange(LoadList.ToArray()); }); }
            else { listbox.Items.AddRange(LoadList.ToArray()); }

        }

        /// <summary>populate ListBox from List <int></summary>
        /// <param name="ListBox listbox"> </param>
        /// <param name=" List<int> LoadList"> </param>
        public void List_To_ListBoxInt(ListBox listbox, List<int> LoadList)
        {
            //Setup data binding
            listbox.DataSource = LoadList;
            listbox.DisplayMember = "Name";
            //cb.ValueMember = "Value";
        }


        /// <summary>populate ListBox from List <string> (from any thread)</summary>
        /// <param name="ListBox listbox"> </param>
        /// <param name=" List<string> LoadList"> </param>
        /// <param name="Clear"> </param>
        public void List_To_CheckedListBox(CheckedListBox listbox, List<string> LoadList, bool Clear = true)
        {
            if (Clear)  // option to clear out previous listbox values before adding
            {
                if (listbox.InvokeRequired) { listbox.BeginInvoke((MethodInvoker)delegate () { listbox.Items.Clear(); }); }
                else { listbox.Items.Clear(); }
            }

            //*/


            // update listbox values (from any thread) 
            if (listbox.InvokeRequired) { listbox.BeginInvoke((MethodInvoker)delegate () { listbox.Items.AddRange(LoadList.ToArray()); }); }
            else { listbox.Items.AddRange(LoadList.ToArray()); }

        }

        public void List_To_TextFile(List<string> list, string SaveFile)
        {
            _AHK ahk = new _AHK();

            foreach (string item in list)
            {
                ahk.FileAppend(item, SaveFile);
            }
        }

        #endregion

        #region === Lists: Return From ===

        /// <summary>return position of search item in List<string></summary>
        /// <param name="List<string> list"> </param>
        /// <param name=" SearchTerm"> </param>
        public int Return_List_Position(List<string> list, string SearchTerm)
        {
            int iIndex = 0;
            foreach (string line in list)
            {
                // compare line text to search term
                if (line.ToUpper().Trim() == SearchTerm.ToUpper().Trim())
                {
                    return iIndex; // if found, return current index number
                }

                iIndex++;
            }

            return iIndex;
        }

        /// <summary>return position of search item in list<int></summary>
        /// <param name="List<int> list"> </param>
        /// <param name=" SearchInt"> </param>
        public int Return_List_PositionInt(List<int> list, int SearchInt)
        {
            int iIndex = 0;
            foreach (int line in list)
            {
                // compare line text to search term
                if (line == SearchInt)
                {
                    return iIndex; // if found, return current index number
                }

                iIndex++;
            }

            return iIndex;
        }

        /// <summary>Return value of list<string> item by position in list</summary>
        /// <param name="list">List to Use for Value Lookup</param>
        /// <param name="ListPosition">Position of Item in List, Starts with 1</param>
        public string Return_List_Value(List<string> list, int ListPosition = 1)
        {
            if (ListPosition == 0) { ListPosition = 1; }
            int iIndex = 1;
            string ReturnText = "";
            foreach (string line in list)
            {
                ReturnText = line;

                // compare line text to search term
                if (iIndex == ListPosition)
                {
                    return ReturnText; // if found, return current index number
                }

                iIndex++;
            }

            return ReturnText;  // return last value in the list if position out of range
        }

        /// <summary>return value of list<int> item by position in list</summary>
        /// <param name="List<int> list"> </param>
        /// <param name=" ListPosition"> </param>
        public int Return_List_ValueInt(List<int> list, int ListPosition)
        {
            int iIndex = 0;
            int ReturnInt = 0;
            foreach (int line in list)
            {
                ReturnInt = line;

                // compare line text to search term
                if (iIndex == ListPosition)
                {
                    return ReturnInt; // if found, return current index number
                }

                iIndex++;
            }

            return ReturnInt;  // return last value in the list if position out of range
        }

        /// <summary>returns first item added to list</summary>
        /// <param name="List<string> list"> </param>
        public string First_List_Item(List<string> list)
        {
            string firstItem = "";
            foreach (string item in list)
            {
                return item;
            }
            return firstItem;
        }

        /// <summary>returns last item added to list</summary>
        /// <param name="List<string> list"> </param>
        public string Last_List_Item(List<string> list)
        {
            string lastItem = "";
            foreach (string item in list)
            {
                lastItem = item;
            }
            return lastItem;
        }

        /// <summary>return list split by splitchar (ex: ",") as new string</summary>
        /// <param name="List<string> list"> </param>
        /// <param name="SplitChar"> </param>
        public string List_To_String(List<string> list, string SplitChar = ",")
        {
            // Join strings into one CSV line.
            string line = string.Join(SplitChar, list.ToArray());

            return line;
        }

        /// <summary>return list to string, each item on new line</summary>
        /// <param name="List<string> list"> </param>
        public string List_To_String_NewLines(List<string> list)
        {
            // Join strings into one CSV line.
            string LineCode = List_To_String(list, Environment.NewLine);

            return LineCode;
        }

        #endregion

        #region === Lists: Management ===

        /// <summary>Checks if value in is List<string>. Returns true if found.</summary>
        /// <param name="List<string> list"> </param>
        /// <param name=" value"> </param>
        public bool InList(List<string> list, string value)
        {
            if (list == null) { return false; }

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Trim() == value.Trim())
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>Returns List with Items Containing SearchText</summary>
        /// <param name="List<string> list"> </param>
        /// <param name=" SearchText"> </param>
        public List<string> In_List_Search(List<string> list, string SearchText)
        {
            List<string> returnList = new List<string>();

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ToUpper().Contains(SearchText.ToUpper()))
                {
                    returnList.Add(list[i]);
                }
            }

            return returnList;
        }

        /// <summary>Add new list item to top of existing list</summary>
        /// <param name="List<string> OriginalList">List to add an entry to</param>
        /// <param name="NewListItem">New item to add to top of list</param>
        public List<string> AddToTop_List(List<string> OriginalList, string NewListItem)
        {
            List<string> returnList = new List<string>();

            returnList.Add(NewListItem); // add new item to list

            foreach (string Item in OriginalList)
            {
                returnList.Add(Item);  // add previous list entries
            }

            return returnList;
        }


        /// <summary>removes list of exclude items from the original list - subtracting unwanted items</summary>
        /// <param name="List<string> OriginalList"> </param>
        /// <param name=" List<string> ExcludeItems"> </param>
        public List<string> Remove_From_List(List<string> OriginalList, List<string> ExcludeItems)
        {
            if (OriginalList == null || OriginalList.Count < 1) { return OriginalList; } // nothing to do if blank value passed in
            if (ExcludeItems == null || ExcludeItems.Count < 1) { return OriginalList; } // nothing to do if blank value passed in

            List<string> returnList = new List<string>();

            if (OriginalList == null) { return null; }

            foreach (string Item in OriginalList)
            {
                bool Add = true;
                foreach (string eItem in ExcludeItems)
                {
                    if (Item.Trim().ToUpper() == eItem.Trim().ToUpper())
                    {
                        Add = false;
                    }
                }

                if (Add) { returnList.Add(Item); }
            }

            return returnList;
        }

        /// <summary>removes misc .cs files from return list of project cs files</summary>
        /// <param name="List<string> OriginalList"> </param>
        /// <param name=" List<string> ExcludeItems"> </param>
        public List<string> Remove_From_CodeList(List<string> OriginalList, List<string> ExcludeItems)
        {
            if (OriginalList == null || OriginalList.Count < 1) { return OriginalList; } // nothing to do if blank value passed in
            if (ExcludeItems == null || ExcludeItems.Count < 1) { return OriginalList; } // nothing to do if blank value passed in

            List<string> returnList = new List<string>();

            foreach (string Item in OriginalList)
            {
                bool Add = true;

                if (Item.Contains(".Designer.")) { Add = false; }


                foreach (string eItem in ExcludeItems)
                {
                    if (Item.Trim().ToUpper() == eItem.Trim().ToUpper())
                    {
                        Add = false;
                    }
                }

                if (Add) { returnList.Add(Item); }
            }

            return returnList;
        }

        /// <summary>take list and return list that doesn't contain TextToExclude</summary>
        /// <param name="List<string> OriginalList"> </param>
        /// <param name=" TextToExclude"> </param>
        public List<string> Remove_From_List_Containing_Text(List<string> OriginalList, string TextToExclude)
        {
            if (OriginalList == null || OriginalList.Count < 1) { return OriginalList; } // nothing to do if blank value passed in

            List<string> NewList = new List<string>();

            foreach (string project in OriginalList)
            {
                if (!project.ToUpper().Contains(TextToExclude.ToUpper()))
                {
                    NewList.Add(project);
                }
            }

            return NewList;
        }

        /// <summary>Append AddList to MainList</summary>
        /// <param name="List<string> MainList"> </param>
        /// <param name=" List<string> AddList"> </param>
        public List<string> MergeLists(List<string> MainList, List<string> AddList)
        {
            if (MainList == null && AddList != null) { return AddList; } // nothing to merge if one list is blank
            if (AddList == null && MainList != null) { return MainList; } // nothing to merge if one list is blank
            if (AddList == null && MainList == null) { return new List<string>(); } // nothing to merge if both lists blank

            MainList.AddRange(AddList);  // Merge two lists
            return MainList;
        }

        /// <summary>Returns list from existing list, removing duplicate entries</summary>
        /// <param name="ListWithDuplicates">List potentially containing duplicate entries to remove</param>
        public List<string> Distinct(List<string> ListWithDuplicates)
        {
            if (ListWithDuplicates == null) { return new List<string>(); } // nothing to sort, return blank list

            List<string> unique = new List<string>();

            foreach (string line in ListWithDuplicates)
            {
                string newLine = line;
                if (newLine.Trim() == "") { continue; }

                bool inList = InList(unique, newLine);
                if (!inList) { unique.Add(newLine); }
            }

            return unique;
        }

        /// <summary>Returns list sorted alphabetically</summary>
        /// <param name="list">List to sort alphabetically</param>
        public List<string> SortList(List<string> list)
        {
            if (list == null) { return new List<string>(); } // nothing to sort, return blank list

            string[] s = list.ToArray();
            Array.Sort(s);
            List<string> OutList = new List<string>();
            foreach (string t in s)
            {
                OutList.Add(t);
            }

            return OutList;
        }

        /// <summary>
        /// Sort List of File Sizes (Containing KB/MB/GB) By Size Order
        /// </summary>
        /// <param name="FileSizes">List of File Sizes</param>
        /// <returns>Returns List Sorted by FileSize</returns>
        public List<string> FileSize_SortList(List<string> FileSizes, bool Ascending = true)
        {
            List<string> ReturnList = new List<string>();
            List<int> MBList = new List<int>();
            List<int> GBList = new List<int>();
            List<int> KBList = new List<int>();

            _AHK ahk = new _AHK();

            foreach (string size in FileSizes)
            {
                string SIze = "";

                if (size.ToUpper().Contains("KB"))
                {
                    SIze = size.ToUpper().Replace("KB", "").Trim();
                    if (SIze.Contains(".")) { SIze = ahk.StringSplit(SIze, ".", 0); }
                    KBList.Add(SIze.ToInt());
                    //double vOut = Convert.ToDouble(SIze.ToInt());
                    //KBList.Add(vOut);
                }
                if (size.ToUpper().Contains("MB"))
                {
                    SIze = size.ToUpper().Replace("MB", "").Trim();
                    if (SIze.Contains(".")) { SIze = ahk.StringSplit(SIze, ".", 0); }
                    MBList.Add(SIze.ToInt());
                    //double vOut = Convert.ToDouble(SIze.ToInt());
                    //MBList.Add(vOut);
                }
                if (size.ToUpper().Contains("GB"))
                {
                    SIze = size.ToUpper().Replace("GB", "").Trim();
                    if (SIze.Contains(".")) { SIze = ahk.StringSplit(SIze, ".", 0); }
                    GBList.Add(SIze.ToInt());
                    //double vOut = Convert.ToDouble(SIze.ToInt());
                    //GBList.Add(vOut);
                }


                //if (size.ToUpper().Contains("KB")) { SIze = size.ToUpper().Replace("KB", "").Trim(); double vOut = Convert.ToDouble(SIze.ToInt()); KBList.Add(vOut); }
                //if (size.ToUpper().Contains("MB")) { SIze = size.ToUpper().Replace("MB", "").Trim(); MBList.Add(SIze.ToInt()); }
                //if (size.ToUpper().Contains("GB")) { SIze = size.ToUpper().Replace("GB", "").Trim(); GBList.Add(SIze.ToInt()); }
            }

            if (Ascending)
            {
                var ascendingKB = KBList.OrderBy(i => i);  // sort list ascending
                var ascendingMB = MBList.OrderBy(i => i);
                var ascendingGB = GBList.OrderBy(i => i);

                foreach (int A in ascendingKB) { ReturnList.Add(A + " KB"); }
                foreach (int A in ascendingMB) { ReturnList.Add(A + " MB"); }
                foreach (int A in ascendingGB) { ReturnList.Add(A + " GB"); }
            }
            else
            {
                var descendingKB = KBList.OrderByDescending(i => i); // sort list descending
                var descendingMB = MBList.OrderByDescending(i => i);
                var descendingGB = GBList.OrderByDescending(i => i);

                foreach (int A in descendingGB) { ReturnList.Add(A + " GB"); }
                foreach (int A in descendingMB) { ReturnList.Add(A + " MB"); }
                foreach (int A in descendingKB) { ReturnList.Add(A + " KB"); }
            }

            return ReturnList;
        }


        /// <summary>
        /// Splits One List into 4 Equal Parts
        /// </summary>
        /// <param name="urls">List to Split into 4 Equal Parts (remainder go on last list)</param>
        /// <param name="list1"></param>
        /// <param name="list2"></param>
        /// <param name="list3"></param>
        /// <param name="list4"></param>
        public void FourLists(List<string> urls, out List<string> list1, out List<string> list2, out List<string> list3, out List<string> list4)
        {
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

            list1 = urls1;
            list2 = urls2;
            list3 = urls3;
            list4 = urls4;
        }


        #endregion


    }
}

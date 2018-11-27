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
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;
using Telerik.WinControls.UI;
using System.Data.SQLite;
using System.Threading;

namespace sharpAHK_Dev
{

    public partial class _Database
    {
        public class dir2Db
        {

            private static _AHK ahk = new _AHK();
            private static _Database.SQLite sqlite = new _Database.SQLite();
            private static _Database.SQL sql = new _Database.SQL();
            private static _Lists lst = new _Lists();
            private static _WinForms frm = new _WinForms();
            private static _TelerikLib tel = new _TelerikLib();
            private static _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();


            #region === SQLite: DirToDb ===

            //### DirToDb Setup ###

            //// define display control to write update progress while writing dir2db entries
            // globalControls.DirToDbDisplay = txtStatus; 


            /// <summary>Updates Text on Control - Must define dbDisplay with Text/Label Name to Populate. Used as Optional Parameter for Updates While Functions Execute</summary>
            /// <param name="Text">Text to populate user's defined dbDisplay (TextBox/Label/Etc)</param>
            public void DirToDb_DisplayText(string Text = "")
            {
                if (globalControls.DirToDbDisplay != null)
                {
                    try  // try to update control from another thread
                    {
                        MethodInvoker inv = delegate { globalControls.DirToDbDisplay.Text = Text; };
                        Form formName = globalControls.DirToDbDisplay.FindForm();  // determine which form this control belongs to 
                        formName.Invoke(inv);
                    }
                    catch // otherwise update control from current thread
                    {
                        globalControls.DirToDbDisplay.Text = Text;
                    }
                }
            }



            /// <summary>Save Contents of Directory to .sqlite Table</summary>
            /// <param name="DirPath">Directory Path / Drive to Index</param>
            /// <param name="FilePattern">Option to Only Add Specific File Pattern. Default = All Files (*.*)</param>
            /// <param name="Recurse">Option to Search Folders Underneath DirPath (Default = True)</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To Settings.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To. Defaults To 'FileIndex'</param>
            /// <param name="OverWriteTable">Option to OverWrite Existing FileIndex Table (Default = True)</param>
            /// <control>Assign Label/TextBox to displayControl before running to view current status while executing. EX: "sqlite.displayControl = TextBox1;"</control>
            public bool DirToDb(string DirPath, string FilePattern = "*.*", bool Recurse = true, string DbFile = "Settings.sqlite", string TableName = "FileIndex", bool OverWriteTable = true, bool CreateIndexTable = true, Control Progressbar = null)
            {
                bool WriteFileHash = false;  // option to hash and store hash in sqlite 
                bool WriteAttributes = false;

                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }
                if (TableName == "") { TableName = "FileIndex"; }
                if (FilePattern == "") { FilePattern = "*.*"; }

                // Toggle Between Using WinForms and Telerik ProgressBar based on Control Provided
                ProgressBar WinProgressBar = new ProgressBar();
                RadProgressBar TelProgressBar = new RadProgressBar();
                bool Winforms = false; bool Telerik = false;
                if (Progressbar != null)
                {
                    string VarType = Progressbar.GetType().ToString();  //determine what kind of variable was passed into function

                    if (VarType == "Telerik.WinControls.UI.RadProgressBar") { TelProgressBar = (RadProgressBar)Progressbar; Telerik = true; }
                    if (VarType == "System.Windows.Forms.ProgressBar") { WinProgressBar = (ProgressBar)Progressbar; Winforms = true; }

                    //ahk.MsgBox(VarType);
                }




                string IndexTableName = "_Index";

                //bool WriteLastModifiedOnly = false;

                // Create New SQLite DB (*Used First-Run*)
                if (!File.Exists(DbFile)) // create database file if it doen't exist
                {
                    SQLiteConnection.CreateFile(DbFile);
                }

                if (!Directory.Exists(DirPath))  // confirm import directory exists - otherwise return error message to user
                {
                    MessageBox.Show("Unable to Import!\n" + DirPath + " Not Found");
                    return false;
                }


                // Create New Table If It Does NOT Exist Yet
                bool TableExist = sqlite.Table_Exists(DbFile, TableName);  //See if selected Table Exists in SQLite DB file

                if (TableExist)
                {
                    if (OverWriteTable)  // option to clear table contents before writing file index
                    {
                        sqlite.Table_Clear(DbFile, TableName, true);  // clear out existing sqlite table contents
                    }
                }

                if (!TableExist)  // Table DOES NOT exist in SQLite DB
                {
                    string NewTableLine = "FID INTEGER PRIMARY KEY, FileFlag BOOL, FilePath VARCHAR, FileName VARCHAR, DirName VARCHAR, DirPath VARCHAR, FileExt VARCHAR, FileSize VARCHAR, FileSizeBytes VARCHAR, CreationTime VARCHAR, LastAccessTime VARCHAR, LastWriteTime VARCHAR, IsReadOnly BOOL, Attributes VARCHAR, FileExists BOOL, FileAction VARCHAR, Tags VARCHAR, Notes VARCHAR";
                    bool Created = sqlite.Execute(DbFile, "CREATE TABLE [" + TableName + "] (" + NewTableLine + ")");  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
                    if (!sqlite.Table_Exists(DbFile, TableName)) { ahk.MsgBox("Error Creating [" + TableName + "] in\n" + DbFile); return false; } // if the table didn't create - stop with error
                }


                if (CreateIndexTable)
                {
                    //== create directory index for storing multiple drives / folders -- tracks last modified etc ===
                    TableExist = sqlite.Table_Exists(DbFile, IndexTableName);  //See if selected Table Exists in SQLite DB file
                    if (!TableExist)  // Table DOES NOT exist in SQLite DB
                    {
                        string NewTableLine = "FID INTEGER PRIMARY KEY, Enabled BOOL, TableName VARCHAR, Label VARCHAR, DirPath VARCHAR, FileCount VARCHAR, CaptureTime VARCHAR, LastUpdated VARCHAR";
                        sqlite.Execute(DbFile, "CREATE TABLE " + IndexTableName + " (" + NewTableLine + ")");  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
                    }
                }


                //=============================================================================================
                // Get list of files in the specific directory - Recursive File Search - Fill Data Table
                //=============================================================================================

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                // search all directories unless user selects Recurse = False
                SearchOption recurse = SearchOption.AllDirectories;
                if (!Recurse) { recurse = SearchOption.TopDirectoryOnly; }

                //string[] files = Directory.GetFiles(DirPath, FilePattern, recurse);

                var files = GetAllAccessibleFiles(DirPath);

                int totalFileCount = files.Count;

                // setup progress bar if progress bar provided
                if (Progressbar != null)
                {
                    if (Winforms) { frm.SetupProgressBar(WinProgressBar, totalFileCount); }
                    if (Telerik) { pro.SetupProgressBar(TelProgressBar, totalFileCount); }
                }

                // update displayControl Text with progress (if user defined this control name)
                if (!Loading_DirToDb_LIST) { DirToDb_DisplayText("Total Files To Index = " + totalFileCount.ToString()); }

                //List<string> SkipFormats = new List<string> { "JPG", "JPEG", "PNG" };


                int FileCount = 0;

                try
                {
                    using (var conn = new SQLiteConnection(@"Data Source=" + DbFile))
                    {
                        conn.Open();

                        using (var cmd = new SQLiteCommand(conn))
                        {
                            using (var transaction = conn.BeginTransaction())
                            {
                                int k = 1;
                                foreach (string file in files)  // loop through list of files and write file details to SQLite db
                                {

                                    if (Progressbar != null)
                                    {
                                        if (Winforms) { frm.UpdateProgressBar(WinProgressBar); }
                                        if (Telerik) { pro.UpdateProgress(TelProgressBar, k + "/" + files.Count); k++; }
                                    }

                                    System.IO.FileInfo fileinfo = new System.IO.FileInfo(file); //retrieve info about each file

                                    string FileExt = fileinfo.Extension.ToString();
                                    FileExt = ahk.StringReplace(FileExt, "."); // remove period from file ext

                                    //// if user provides any file formats to skip, check here and avoid writing to db 
                                    //    if (SkipFormats != null && SkipFormats.Count > 0)
                                    //{
                                    //    bool SkipFile = lst.InList(SkipFormats, FileExt.ToUpper());
                                    //    if (SkipFile) { continue; }
                                    //}


                                    //string FileName = fileinfo.Name.ToString();

                                    string FileSizeBytes = fileinfo.Length.ToString();  // # of bytes as an int
                                    string FileSize = ahk.FormatBytes(fileinfo.Length);  // convert bytes to Text representation (adds kb/mb/tb to return)

                                    string LastWriteTime = "";
                                    try { LastWriteTime = fileinfo.LastWriteTime.ToString(); }
                                    catch { }

                                    string LastAccessTime = "";
                                    try { LastAccessTime = fileinfo.LastAccessTime.ToString(); }
                                    catch { }

                                    string CreationTime = "";
                                    try { CreationTime = fileinfo.CreationTime.ToString(); }
                                    catch { }

                                    bool IsReadOnly = fileinfo.IsReadOnly;

                                    bool FileExists = fileinfo.Exists;
                                    //string DirName = fileinfo.DirectoryName.ToString();
                                    //string dirPath = fileinfo.Directory.ToString();

                                    string Attributes = fileinfo.Attributes.ToString();

                                    string FileAction = ""; // variable used to indicate a file is queued to be copied to another location 
                                    string FilePath = file;
                                    bool FileFlag = false;

                                    //DirName = ahk.FixSpecialChars(DirName); //remove invalid characters before writing
                                    //DirPath = ahk.FixSpecialChars(DirPath); //remove invalid characters before writing
                                    //FileName = ahk.FixSpecialChars(FileName); //remove invalid characters before writing
                                    FilePath = ahk.FixSpecialChars(FilePath);//remove invalid characters before writing
                                    string DirName = ahk.FixSpecialChars(ahk.DirName(file));
                                    string FileName = ahk.FixSpecialChars(ahk.FileName(file));
                                    string dirPath = ahk.FixSpecialChars(ahk.FileDir(file));

                                    string fileHash = "";
                                    if (WriteFileHash) { fileHash = ahk.FileHash(file); }  // returns hash for file 

                                    string InsertLine = "INSERT into [" + TableName + "] (FileFlag, FilePath, FileName, DirName, DirPath, FileExt, FileSize, FileSizeBytes, CreationTime, LastAccessTime, LastWriteTime, IsReadOnly, Attributes, FileExists, FileAction, Tags, Notes) values ('" + FileFlag + "', '" + FilePath + "', '" + FileName + "', '" + DirName + "', '" + dirPath + "', '" + FileExt + "', '" + FileSize + "', '" + FileSizeBytes + "', '" + CreationTime + "', '" + LastAccessTime + "', '" + LastWriteTime + "', '" + IsReadOnly + "', '" + Attributes + "', '" + FileExists + "', '" + FileAction + "', '" + DirName + "', '" + fileHash + "');";
                                    // if (InsertLine.Contains("zix")) { ahk.MsgBox(InsertLine); }

                                    cmd.CommandText = InsertLine;
                                    try
                                    {
                                        cmd.ExecuteNonQuery();
                                    }
                                    catch (Exception ex)
                                    {
                                        string Ex = ex.ToString();
                                    }

                                    FileCount++;

                                    // loading single directory 
                                    if (!Loading_DirToDb_LIST) { DirToDb_DisplayText("Adding File " + FileCount.ToString() + " / " + totalFileCount.ToString()); }

                                    // loading multiple directories in list
                                    if (Loading_DirToDb_LIST) { DirToDb_DisplayText("Adding File " + FileCount.ToString() + " / " + totalFileCount.ToString() + "  |  Dir " + Loading_DirToDb_LIST_CurrentCount.ToString() + " / " + Loading_DirToDb_LIST_TotalCount.ToString()); }


                                    //    DateTime LastSyncDate = new DateTime(2016, 06, 25);

                                    //    if (fileinfo.LastWriteTime > LastSyncDate)
                                    //    {
                                    //        //ahk.MsgBox("Need to Add - " + fileinfo.LastWriteTime.ToString());
                                    //        string InsertLine = "INSERT into " + TableName + " (FileFlag, FilePath, FileName, DirName, DirPath, FileExt, FileSize, FileSizeBytes, CreationTime, LastAccessTime, LastWriteTime, IsReadOnly, Attributes, FileExists, FileAction, Tags) values ('" + FileFlag + "', '" + FilePath + "', '" + FileName + "', '" + DirName + "', '" + DirPath + "', '" + FileExt + "', '" + FileSize + "', '" + FileSizeBytes + "', '" + CreationTime + "', '" + LastAccessTime + "', '" + LastWriteTime + "', '" + IsReadOnly + "', '" + Attributes + "', '" + FileExists + "', '" + FileAction + "', '" + DirName + "')";
                                    //        cmd.CommandText = InsertLine + ";";
                                    //        cmd.ExecuteNonQuery();
                                    //    }



                                }  // end foreach file loop

                                transaction.Commit();  //write collection of insert statements
                            }
                        }

                        conn.Close();

                        DirToDb_DisplayText("Finished Adding Dir to Db!");
                    }
                }
                catch(Exception ex)
                {
                    string Ex = ex.ToString();
                }


                if (CreateIndexTable)
                {
                    string IndexTimeSeconds = stopwatch.Elapsed.TotalSeconds.ToString();

                    // create index entry / update existing entry if found
                    string Label = ahk.DirName(DirPath);
                    string Index_InsertLine = "Insert Into [" + IndexTableName + "] (Enabled, TableName, Label, DirPath, FileCount, CaptureTime, LastUpdated) VALUES ('true', '" + TableName + "', '" + Label + "', '" + DirPath + "', '" + FileCount + "', '" + IndexTimeSeconds + "', '" + DateTime.Now + "')";
                    string Index_UpdateLine = "Update [" + IndexTableName + "] Set Enabled = 'true', TableName = '" + TableName + "', Label = '" + Label + "', DirPath = '" + DirPath + "', FileCount = '" + FileCount + "', CaptureTime = '" + IndexTimeSeconds + "', LastUpdated = '" + DateTime.Now + "' where DirPath = '" + DirPath + "'";
                    sqlite.Update_Insert(DbFile, Index_UpdateLine, Index_InsertLine);
                }

                // reset progress bar if provided
                if (Progressbar != null)
                {
                    if (Winforms) { frm.SetupProgressBar(WinProgressBar, 1); }
                    if (Telerik) { }
                }

                //Console.WriteLine("{0} seconds with one transaction.", stopwatch.Elapsed.TotalSeconds);
                return true;
            }

            private bool Loading_DirToDb_LIST = false; // internal param that tells DirToDb a list is being loaded, changes behavior of display text to include dir counts
            private int Loading_DirToDb_LIST_TotalCount = 0; // internal param that tells DirToDb the total number of dirs being processed
            private int Loading_DirToDb_LIST_CurrentCount = 0; // internal param that tracks current progress of DirToDb list processing

            // execute dirtoDb function in batch, loading list of directories to add to same index
            public void DirToDb_LoadDirList(List<string> DirList, string FilePattern = "*.*", bool Recurse = true, string DbFile = "Settings.sqlite", string TableName = "FileIndex", bool OverWriteTable = true, bool CreateIndexTable = true)
            {
                Loading_DirToDb_LIST = true;
                Loading_DirToDb_LIST_TotalCount = DirList.Count;
                Loading_DirToDb_LIST_CurrentCount = 1;

                foreach (string dir in DirList)
                {
                    // only overwrite table on first directory load if option is enabled
                    if (OverWriteTable)
                    {
                        if (Loading_DirToDb_LIST_CurrentCount == 1) { DirToDb(dir, FilePattern, Recurse, DbFile, TableName, true, CreateIndexTable); }
                        if (Loading_DirToDb_LIST_CurrentCount != 1) { DirToDb(dir, FilePattern, Recurse, DbFile, TableName, false, CreateIndexTable); }
                    }
                    if (!OverWriteTable) { DirToDb(dir, FilePattern, Recurse, DbFile, TableName, OverWriteTable, CreateIndexTable); }

                    Loading_DirToDb_LIST_CurrentCount++;
                }

                Loading_DirToDb_LIST = false;
            }

            // load list of directories to Db in batch - ON NEW THREAD
            public void DirToDb_LoadDirList_(List<string> DirList, string FilePattern = "*.*", bool Recurse = true, string DbFile = "Settings.sqlite", string TableName = "FileIndex", bool OverWriteTable = true, bool CreateIndexTable = true)
            {
                Thread dirToDbThread = new Thread(() => DirToDb_LoadDirList(DirList, FilePattern, Recurse, DbFile, TableName, OverWriteTable, CreateIndexTable));
                dirToDbThread.Start();
            }


            /// <summary>Save Contents of Directory to .sqlite Table [CREATES NEW THREAD]</summary>
            /// <param name="DirPath">Directory Path / Drive to Index</param>
            /// <param name="FilePattern">Option to Only Add Specific File Pattern. Default = All Files (*.*)</param>
            /// <param name="Recurse">Option to Search Folders Underneath DirPath (Default = True)</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To Settings.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To. Defaults To 'FileIndex'</param>
            /// <param name="OverWriteTable">Option to OverWrite Existing FileIndex Table (Default = True)</param>
            public void DirToDb_(string DirPath, string FilePattern = "*.*", bool Recurse = true, string DbFile = "Settings.sqlite", string TableName = "FileIndex", bool OverWriteTable = true, bool CreateIndexTable = true)
            {
                Thread dirToDbThread = new Thread(() => DirToDb(DirPath, FilePattern, Recurse, DbFile, TableName, OverWriteTable, CreateIndexTable));
                dirToDbThread.Start();
            }


            // dir2db - lighter on extra details, includes file hash for image comparison jobs 
            public bool imgDirToDb(string DirPath, string FilePattern = "*.*", bool Recurse = true, string DbFile = "imgDirToDb.sqlite", string TableName = "imgDirToDb", bool OverWriteTable = true)
            {
                if (DbFile == "imgDirToDb.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\imgDirToDb.sqlite"; }
                if (TableName == "") { TableName = "FileIndex"; }
                if (FilePattern == "") { FilePattern = "*.*"; }

                // create database file if it doen't exist
                if (!File.Exists(DbFile)) { SQLiteConnection.CreateFile(DbFile); }

                if (!Directory.Exists(DirPath))  // confirm import directory exists - otherwise return error message to user
                {
                    MessageBox.Show("Unable to Import!\n" + DirPath + " Not Found");
                    return false;
                }

                // Create New Table If It Does NOT Exist Yet
                bool TableExist = sqlite.Table_Exists(DbFile, TableName);  //See if selected Table Exists in SQLite DB file

                if (TableExist)  // option to overwrite table if it already exists
                {
                    // option to clear table contents before writing file index
                    if (OverWriteTable) { sqlite.Table_Clear(DbFile, TableName, true); } // clear out existing sqlite table contents
                }

                //FileName, FilePath, FileSize, FileHash, FileContents, FileVersion, DateModified, FileFlag
                if (!TableExist)  // Table DOES NOT exist in SQLite DB
                {
                    string NewTableLine = "FID INTEGER PRIMARY KEY, FileName VARCHAR, FilePath VARCHAR, FileSize VARCHAR, FileHash VARCHAR, FileContents VARCHAR, FileVersion VARCHAR, DateModified VARCHAR, FileFlag VARCHAR";
                    bool Created = sqlite.Execute(DbFile, "CREATE TABLE [" + TableName + "] (" + NewTableLine + ")");  // Create a Table in Sqlite Db
                    if (!sqlite.Table_Exists(DbFile, TableName)) { ahk.MsgBox("Error Creating [" + TableName + "] in\n" + DbFile); return false; } // if the table didn't create - stop with error
                }


                // Get list of files in the specific directory - Recursive File Search - Fill Data Table

                //Search all directories unless user selects Recurse = False
                //SearchOption recurse = SearchOption.AllDirectories;
                //if (!Recurse) { recurse = SearchOption.TopDirectoryOnly; }

                var files = GetAllAccessibleFiles(DirPath);

                int totalFileCount = files.Count;

                int FileCount = 0;

                //string mode = "BATCH INSERT";
                string mode = "INSERT UPDATE";

                if (mode.ToUpper() == "INSERT UPDATE")
                {
                    foreach (string file in files)  // loop through list of files and write file details to SQLite db
                    {
                        System.IO.FileInfo fileinfo = new System.IO.FileInfo(file); //retrieve info about each file

                        if (fileinfo.Exists)
                        {
                            string FileName = fileinfo.Name.ToString().Replace("'", "''");
                            string FileSizeBytes = fileinfo.Length.ToString();  // # of bytes as an int
                                                                                //string FileSize = ahk.FormatBytes(fileinfo.Length);  // convert bytes to Text representation (adds kb/mb/tb to return)
                            string LastWriteTime = fileinfo.LastWriteTime.ToString();
                            //string LastAccessTime = fileinfo.LastAccessTime.ToString();
                            //string CreationTime = fileinfo.CreationTime.ToString();
                            //bool IsReadOnly = fileinfo.IsReadOnly;
                            string FileExt = fileinfo.Extension.ToString();

                            bool image = false;
                            if (FileExt.ToUpper() == ".JPG") { image = true; }
                            if (FileExt.ToUpper() == ".JPEG") { image = true; }
                            if (FileExt.ToUpper() == ".PNG") { image = true; }

                            if (!image) { continue; }

                            //FileExt = ahk.StringReplace(FileExt, "."); // remove period from file ext
                            //bool FileExists = fileinfo.Exists;
                            //string DirName = fileinfo.DirectoryName.ToString();
                            //string dirPath = fileinfo.Directory.ToString();
                            //string Attributes = fileinfo.Attributes.ToString();
                            //string FileAction = ""; // variable used to indicate a file is queued to be copied to another location 
                            //string FileFlag = "";
                            //DirName = ahk.FixSpecialChars(DirName); //remove invalid characters before writing
                            //DirPath = ahk.FixSpecialChars(DirPath); //remove invalid characters before writing
                            //FileName = ahk.FixSpecialChars(FileName); //remove invalid characters before writing
                            string FilePath = ahk.FixSpecialChars(file);//remove invalid characters before writing
                            string DirName = ahk.FixSpecialChars(ahk.DirName(file));
                            //string FileName = ahk.FixSpecialChars(ahk.FileName(file));
                            //string dirPath = ahk.FixSpecialChars(ahk.FileDir(file));
                            string fileHash = ahk.FileHash(file);  // returns hash for file 

                            string fileContents = "";
                            string fileVersion = "1";
                            //FileName, FilePath, FileSize, FileHash, FileContents, FileVersion, DateModified
                            string InsertLine = "INSERT into [" + TableName + "] (FileName, FilePath, FileSize, FileHash, FileContents, FileVersion, DateModified, FileFlag) values ('" + FileName + "', '" + FilePath + "', '" + FileSizeBytes + "', '" + fileHash + "', '" + fileContents + "', '" + fileVersion + "', '" + LastWriteTime + "', '0');";
                            string UpdateLine = "UPDATE [" + TableName + "] Set FileFlag = '1' WHERE FileHash = '" + fileHash + "' AND FileSize = '" + FileSizeBytes + "'";

                            bool Updated = sqlite.Execute(DbFile, UpdateLine);

                            // file was found in database (hash matched)
                            if (Updated)
                            {
                                string existingFilePath = sqlite.ReturnString(DbFile, "Select FilePath from [" + TableName + "] where FileHash = '" + fileHash + "'");
                                string existingFileName = sqlite.ReturnString(DbFile, "Select FileName from [" + TableName + "] where FileHash = '" + fileHash + "'");
                                string existingFileSize = sqlite.ReturnString(DbFile, "Select FileSize from [" + TableName + "] where FileHash = '" + fileHash + "'");

                                ahk.MsgBox(FileSizeBytes + " | " + existingFileSize + Environment.NewLine + "Existing FilePath: " + existingFilePath + Environment.NewLine + "Adding File: " + FilePath);
                            }

                            // file isn't found, insert into database
                            if (!Updated)
                            {
                                Updated = sqlite.Execute(DbFile, InsertLine);
                            }

                            FileCount++;

                        } // if file exists on drive
                    }

                }



                if (mode.ToUpper() == "BATCH INSERT")
                {
                    using (var conn = new SQLiteConnection(@"Data Source=" + DbFile))
                    {
                        conn.Open();

                        using (var cmd = new SQLiteCommand(conn))
                        {
                            using (var transaction = conn.BeginTransaction())
                            {
                                foreach (string file in files)  // loop through list of files and write file details to SQLite db
                                {
                                    System.IO.FileInfo fileinfo = new System.IO.FileInfo(file); //retrieve info about each file

                                    if (fileinfo.Exists)
                                    {
                                        string FileName = fileinfo.Name.ToString().Replace("'", "''");
                                        string FileSizeBytes = fileinfo.Length.ToString();  // # of bytes as an int
                                                                                            //string FileSize = ahk.FormatBytes(fileinfo.Length);  // convert bytes to Text representation (adds kb/mb/tb to return)
                                                                                            //string LastWriteTime = fileinfo.LastWriteTime.ToString();
                                                                                            //string LastAccessTime = fileinfo.LastAccessTime.ToString();
                                                                                            //string CreationTime = fileinfo.CreationTime.ToString();
                                                                                            //bool IsReadOnly = fileinfo.IsReadOnly;
                                                                                            //string FileExt = fileinfo.Extension.ToString();
                                                                                            //FileExt = ahk.StringReplace(FileExt, "."); // remove period from file ext
                                                                                            //bool FileExists = fileinfo.Exists;
                                                                                            //string DirName = fileinfo.DirectoryName.ToString();
                                                                                            //string dirPath = fileinfo.Directory.ToString();
                                                                                            //string Attributes = fileinfo.Attributes.ToString();
                                                                                            //string FileAction = ""; // variable used to indicate a file is queued to be copied to another location 
                                        string FileFlag = "";
                                        //DirName = ahk.FixSpecialChars(DirName); //remove invalid characters before writing
                                        //DirPath = ahk.FixSpecialChars(DirPath); //remove invalid characters before writing
                                        //FileName = ahk.FixSpecialChars(FileName); //remove invalid characters before writing
                                        //string FilePath = ahk.FixSpecialChars(file);//remove invalid characters before writing
                                        string DirName = ahk.FixSpecialChars(ahk.DirName(file));
                                        //string FileName = ahk.FixSpecialChars(ahk.FileName(file));
                                        //string dirPath = ahk.FixSpecialChars(ahk.FileDir(file));
                                        string fileHash = ahk.FileHash(file);  // returns hash for file 

                                        string InsertLine = "INSERT into [" + TableName + "] (FileName, DirName, FileSizeBytes, FileHash, FileFlag) values ('" + FileName + "', '" + DirName + "', '" + FileSizeBytes + "', '" + fileHash + "', '" + FileFlag + "');";
                                        // if (InsertLine.Contains("zix")) { ahk.MsgBox(InsertLine); }

                                        cmd.CommandText = InsertLine;
                                        cmd.ExecuteNonQuery();
                                        FileCount++;

                                        //// loading single directory 
                                        //if (!Loading_DirToDb_LIST) { DirToDb_DisplayText("Adding File " + FileCount.ToString() + " / " + totalFileCount.ToString()); }

                                        //// loading multiple directories in list
                                        //if (Loading_DirToDb_LIST) { DirToDb_DisplayText("Adding File " + FileCount.ToString() + " / " + totalFileCount.ToString() + "  |  Dir " + Loading_DirToDb_LIST_CurrentCount.ToString() + " / " + Loading_DirToDb_LIST_TotalCount.ToString()); }


                                    } // if file exists on drive
                                }

                                transaction.Commit();  //write collection of insert statements
                            }
                        }

                        conn.Close();

                        DirToDb_DisplayText("Finished Adding Dir to Db!");
                    }
                }


                //Console.WriteLine("{0} seconds with one transaction.", stopwatch.Elapsed.TotalSeconds);
                return true;
            }


            /// <summary> </summary>
            /// <param name="rootPath"> </param>
            /// <param name="List<string> alreadyFound"> </param>
            public static List<string> GetAllAccessibleFiles(string rootPath, List<string> alreadyFound = null)
            {
                if (alreadyFound == null)
                    alreadyFound = new List<string>();
                DirectoryInfo di = new DirectoryInfo(rootPath);
                try
                {
                    var dirs = di.EnumerateDirectories();
                    foreach (DirectoryInfo dir in dirs)
                    {
                        if (!((dir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden))
                        {
                            alreadyFound = GetAllAccessibleFiles(dir.FullName, alreadyFound);
                        }
                    }

                    var files = Directory.GetFiles(rootPath);
                    foreach (string s in files)
                    {
                        alreadyFound.Add(s);
                    }
                }
                catch
                { }

                return alreadyFound;
            }


            /// <summary>Replace invalid characters with empty strings.</summary>
            /// <param name="fileName"> </param>
            public string CleanFileName(string fileName)
            {

                return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));

                //try
                //{
                //    return Regex.Replace(strIn, @"[^\w\.@-]", "",
                //                         RegexOptions.None, TimeSpan.FromSeconds(1.5));
                //}
                //// If we timeout when replacing invalid characters, 
                //// we should return Empty.
                //catch (RegexMatchTimeoutException)
                //{
                //    return String.Empty;
                //}
            }

            /// <summary>Load FileIndex Table from sqlite Database and Populate DataGridView Control</summary>
            /// <param name="dv">DataGridView Control To Populate</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table To Return All Contents From to Grid</param>
            /// <param name="Or_SQL_Command">SQLite Command That OverRides Previous TableName Search</param>
            /// <param name="AutoSizeColumns">Option To Resize DataGridView Column Width After Populating</param>
            public int DirToDb_Load(DataGridView dv, string DbFile = "Settings.sqlite", string TableName = "FileIndex", string Or_SQL_Command = "", bool AutoSizeColumns = true)
            {
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }
                if (TableName == "") { TableName = "FileIndex"; }

                int RowCount = sqlite.Grid_Load(dv, DbFile, TableName, Or_SQL_Command, AutoSizeColumns);

                return RowCount;
            }


            #region === DirToDb Grid Events ===

            // action when cell is clicked to return selected FileIndex entry from DataGridView
            public FileIndex DirToDb_CellClick(object sender, DataGridViewCellEventArgs e)
            {
                DataGridView dv = (DataGridView)sender;

                FileIndex selectedFile = new FileIndex();
                selectedFile = Return_Object_From_FunctionGrid(dv, e.RowIndex);
                //ahk.MsgBox(selectedFile.FilePath);

                return selectedFile;
            }


            #endregion


            #region === DirToDb Database Functions ===


            public struct FileIndex
            {
                public string FID { get; set; }
                public string FileFlag { get; set; }
                public string FilePath { get; set; }
                public string FileName { get; set; }
                public string DirName { get; set; }
                public string DirPath { get; set; }
                public string FileExt { get; set; }
                public string FileSize { get; set; }
                public string FileSizeBytes { get; set; }
                public string CreationTime { get; set; }
                public string LastAccessTime { get; set; }
                public string LastWriteTime { get; set; }
                public string IsReadOnly { get; set; }
                public string Attributes { get; set; }
                public string FileExists { get; set; }
                public string FileAction { get; set; }
                public string Tags { get; set; }
                public string Notes { get; set; }
            }

            #region === NOT USING ===

            /// <summary></summary>
            /// <param name="FileIndex inObject"> </param>
            /// <param name="DbFile"> </param>
            public bool FileIndex_Update(FileIndex inObject, string DbFile = "Settings.sqlite")
            {
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                string UpdateLine = "Update [FileIndex] set ";
                if (inObject.FID != "") { UpdateLine = UpdateLine + "[FID] = '" + inObject.FID + "',"; }
                if (inObject.FileFlag != "") { UpdateLine = UpdateLine + "[FileFlag] = '" + inObject.FileFlag + "',"; }
                if (inObject.FilePath != "") { UpdateLine = UpdateLine + "[FilePath] = '" + inObject.FilePath + "',"; }
                if (inObject.FileName != "") { UpdateLine = UpdateLine + "[FileName] = '" + inObject.FileName + "',"; }
                if (inObject.DirName != "") { UpdateLine = UpdateLine + "[DirName] = '" + inObject.DirName + "',"; }
                if (inObject.DirPath != "") { UpdateLine = UpdateLine + "[DirPath] = '" + inObject.DirPath + "',"; }
                if (inObject.FileExt != "") { UpdateLine = UpdateLine + "[FileExt] = '" + inObject.FileExt + "',"; }
                if (inObject.FileSize != "") { UpdateLine = UpdateLine + "[FileSize] = '" + inObject.FileSize + "',"; }
                if (inObject.FileSizeBytes != "") { UpdateLine = UpdateLine + "[FileSizeBytes] = '" + inObject.FileSizeBytes + "',"; }
                if (inObject.CreationTime != "") { UpdateLine = UpdateLine + "[CreationTime] = '" + inObject.CreationTime + "',"; }
                if (inObject.LastAccessTime != "") { UpdateLine = UpdateLine + "[LastAccessTime] = '" + inObject.LastAccessTime + "',"; }
                if (inObject.LastWriteTime != "") { UpdateLine = UpdateLine + "[LastWriteTime] = '" + inObject.LastWriteTime + "',"; }
                if (inObject.IsReadOnly != "") { UpdateLine = UpdateLine + "[IsReadOnly] = '" + inObject.IsReadOnly + "',"; }
                if (inObject.Attributes != "") { UpdateLine = UpdateLine + "[Attributes] = '" + inObject.Attributes + "',"; }
                if (inObject.FileExists != "") { UpdateLine = UpdateLine + "[FileExists] = '" + inObject.FileExists + "',"; }
                if (inObject.FileAction != "") { UpdateLine = UpdateLine + "[FileAction] = '" + inObject.FileAction + "',"; }
                if (inObject.Tags != "") { UpdateLine = UpdateLine + "[Tags] = '" + inObject.Tags + "',"; }
                if (inObject.Notes != "") { UpdateLine = UpdateLine + "[Notes] = '" + inObject.Notes + "',"; }

                UpdateLine = ahk.TrimLast(UpdateLine, 1);

                bool Updated = sqlite.Execute(DbFile, UpdateLine);
                return Updated;
            }

            /// <summary></summary>
            /// <param name="DataGridView dv"> </param>
            /// <param name="RowNum"> </param>
            public FileIndex Return_Object_From_FunctionGrid(DataGridView dv, int RowNum = -1)
            {
                _GridControl grid = new _GridControl();

                FileIndex returnObject = new FileIndex();
                if (RowNum < 0) { return returnObject; }
                List<string> colNames = grid.Column_Names(dv);

                returnObject.FID = dv.Rows[RowNum].Cells["FID"].Value.ToString();
                returnObject.FileFlag = dv.Rows[RowNum].Cells["FileFlag"].Value.ToString();
                returnObject.FilePath = dv.Rows[RowNum].Cells["FilePath"].Value.ToString();
                returnObject.FileName = dv.Rows[RowNum].Cells["FileName"].Value.ToString();
                returnObject.DirName = dv.Rows[RowNum].Cells["DirName"].Value.ToString();
                returnObject.DirPath = dv.Rows[RowNum].Cells["DirPath"].Value.ToString();
                returnObject.FileExt = dv.Rows[RowNum].Cells["FileExt"].Value.ToString();
                returnObject.FileSize = dv.Rows[RowNum].Cells["FileSize"].Value.ToString();
                returnObject.FileSizeBytes = dv.Rows[RowNum].Cells["FileSizeBytes"].Value.ToString();
                returnObject.CreationTime = dv.Rows[RowNum].Cells["CreationTime"].Value.ToString();
                returnObject.LastAccessTime = dv.Rows[RowNum].Cells["LastAccessTime"].Value.ToString();
                returnObject.LastWriteTime = dv.Rows[RowNum].Cells["LastWriteTime"].Value.ToString();
                returnObject.IsReadOnly = dv.Rows[RowNum].Cells["IsReadOnly"].Value.ToString();
                returnObject.Attributes = dv.Rows[RowNum].Cells["Attributes"].Value.ToString();
                returnObject.FileExists = dv.Rows[RowNum].Cells["FileExists"].Value.ToString();
                returnObject.FileAction = dv.Rows[RowNum].Cells["FileAction"].Value.ToString();
                returnObject.Tags = dv.Rows[RowNum].Cells["Tags"].Value.ToString();
                returnObject.Notes = dv.Rows[RowNum].Cells["Notes"].Value.ToString();

                return returnObject;
            }

            /// <summary></summary>
            /// <param name="DbFile"> </param>
            /// <param name="TableName"> </param>
            /// <param name="WhereClause"> </param>
            public DataTable Return_FileIndex_DataTable(string DbFile = "Settings.sqlite", string TableName = "FileIndex", string WhereClause = "")
            {
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }
                if (TableName == "") { TableName = "FileIndex"; }

                string SelectLine = "Select * From [" + TableName + "]";

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }

                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


                DataTable table = new DataTable();
                table.Columns.Add("FID", typeof(string));
                table.Columns.Add("FileFlag", typeof(string));
                table.Columns.Add("FilePath", typeof(string));
                table.Columns.Add("FileName", typeof(string));
                table.Columns.Add("DirName", typeof(string));
                table.Columns.Add("DirPath", typeof(string));
                table.Columns.Add("FileExt", typeof(string));
                table.Columns.Add("FileSize", typeof(string));
                table.Columns.Add("FileSizeBytes", typeof(string));
                table.Columns.Add("CreationTime", typeof(string));
                table.Columns.Add("LastAccessTime", typeof(string));
                table.Columns.Add("LastWriteTime", typeof(string));
                table.Columns.Add("IsReadOnly", typeof(string));
                table.Columns.Add("Attributes", typeof(string));
                table.Columns.Add("FileExists", typeof(string));
                table.Columns.Add("FileAction", typeof(string));
                table.Columns.Add("Tags", typeof(string));
                table.Columns.Add("Notes", typeof(string));

                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        FileIndex returnObject = new FileIndex();

                        returnObject.FID = ret["FID"].ToString();
                        returnObject.FileFlag = ret["FileFlag"].ToString();
                        returnObject.FilePath = ret["FilePath"].ToString();
                        returnObject.FileName = ret["FileName"].ToString();
                        returnObject.DirName = ret["DirName"].ToString();
                        returnObject.DirPath = ret["DirPath"].ToString();
                        returnObject.FileExt = ret["FileExt"].ToString();
                        returnObject.FileSize = ret["FileSize"].ToString();
                        returnObject.FileSizeBytes = ret["FileSizeBytes"].ToString();
                        returnObject.CreationTime = ret["CreationTime"].ToString();
                        returnObject.LastAccessTime = ret["LastAccessTime"].ToString();
                        returnObject.LastWriteTime = ret["LastWriteTime"].ToString();
                        returnObject.IsReadOnly = ret["IsReadOnly"].ToString();
                        returnObject.Attributes = ret["Attributes"].ToString();
                        returnObject.FileExists = ret["FileExists"].ToString();
                        returnObject.FileAction = ret["FileAction"].ToString();
                        returnObject.Tags = ret["Tags"].ToString();
                        returnObject.Notes = ret["Notes"].ToString();

                        table.Rows.Add(returnObject.FID, returnObject.FileFlag, returnObject.FilePath, returnObject.FileName, returnObject.DirName, returnObject.DirPath, returnObject.FileExt, returnObject.FileSize, returnObject.FileSizeBytes, returnObject.CreationTime, returnObject.LastAccessTime, returnObject.LastWriteTime, returnObject.IsReadOnly, returnObject.Attributes, returnObject.FileExists, returnObject.FileAction, returnObject.Tags, returnObject.Notes);
                    }
                }

                return table;
            }


            /// <summary></summary>
            /// <param name="DbFile"> </param>
            /// <param name="FileFlag"> </param>
            /// <param name="FilePath"> </param>
            /// <param name="FileName"> </param>
            /// <param name="DirName"> </param>
            /// <param name="DirPath"> </param>
            /// <param name="FileExt"> </param>
            /// <param name="FileSize"> </param>
            /// <param name="FileSizeBytes"> </param>
            /// <param name="CreationTime"> </param>
            /// <param name="LastAccessTime"> </param>
            /// <param name="LastWriteTime"> </param>
            /// <param name="IsReadOnly"> </param>
            /// <param name="Attributes"> </param>
            /// <param name="FileExists"> </param>
            /// <param name="FileAction"> </param>
            /// <param name="Tags"> </param>
            /// <param name="Notes"> </param>
            public bool Update_FileIndex(string DbFile = "Settings.sqlite", string FileFlag = "", string FilePath = "", string FileName = "", string DirName = "", string DirPath = "", string FileExt = "", string FileSize = "", string FileSizeBytes = "", string CreationTime = "", string LastAccessTime = "", string LastWriteTime = "", string IsReadOnly = "", string Attributes = "", string FileExists = "", string FileAction = "", string Tags = "", string Notes = "")
            {
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                string UpdateLine = "Update [FileIndex] set ";
                if (FileFlag != "") { UpdateLine = UpdateLine + "FileFlag = '" + FileFlag + "',"; }
                if (FileFlag != "") { UpdateLine = UpdateLine + "FilePath = '" + FilePath + "',"; }
                if (FileFlag != "") { UpdateLine = UpdateLine + "FileName = '" + FileName + "',"; }

                //List<string> Options = new List<string> { "FileFlag","FileName","explorer"	};

                //FileFlag = '" + FileFlag + "', FilePath = '" + FilePath + "', FileName = '" + FileName + "', DirName = '" + DirName + "', DirPath = '" + DirPath + "', FileExt = '" + FileExt + "', FileSize = '" + FileSize + "', FileSizeBytes = '" + FileSizeBytes + "', CreationTime = '" + CreationTime + "', LastAccessTime = '" + LastAccessTime + "', LastWriteTime = '" + LastWriteTime + "', IsReadOnly = '" + IsReadOnly + "', Attributes = '" + Attributes + "', FileExists = '" + FileExists + "', FileAction = '" + FileAction + "', Tags = '" + Tags + "', Notes = '" + Notes + "' WHERE FilePath = '" + FilePath + "'";



                bool Updated = sqlite.Execute(DbFile, UpdateLine);
                return Updated;
            }

            /// <summary></summary>
            /// <param name="FileIndex inObject"> </param>
            public DataTable Create_FileIndex_DataTable(FileIndex inObject)
            {
                DataTable table = new DataTable();
                table.Columns.Add("FID", typeof(string));
                table.Columns.Add("FileFlag", typeof(string));
                table.Columns.Add("FilePath", typeof(string));
                table.Columns.Add("FileName", typeof(string));
                table.Columns.Add("DirName", typeof(string));
                table.Columns.Add("DirPath", typeof(string));
                table.Columns.Add("FileExt", typeof(string));
                table.Columns.Add("FileSize", typeof(string));
                table.Columns.Add("FileSizeBytes", typeof(string));
                table.Columns.Add("CreationTime", typeof(string));
                table.Columns.Add("LastAccessTime", typeof(string));
                table.Columns.Add("LastWriteTime", typeof(string));
                table.Columns.Add("IsReadOnly", typeof(string));
                table.Columns.Add("Attributes", typeof(string));
                table.Columns.Add("FileExists", typeof(string));
                table.Columns.Add("FileAction", typeof(string));
                table.Columns.Add("Tags", typeof(string));
                table.Columns.Add("Notes", typeof(string));

                table.Rows.Add(inObject.FID, inObject.FileFlag, inObject.FilePath, inObject.FileName, inObject.DirName, inObject.DirPath, inObject.FileExt, inObject.FileSize, inObject.FileSizeBytes, inObject.CreationTime, inObject.LastAccessTime, inObject.LastWriteTime, inObject.IsReadOnly, inObject.Attributes, inObject.FileExists, inObject.FileAction, inObject.Tags, inObject.Notes);
                return table;
            }

            /// <summary></summary>
            /// <param name="DataGridView dv"> </param>
            public void Enable_FileIndex_Columns(DataGridView dv)
            {

                try { dv.Columns["FID"].ReadOnly = true; }
                catch { }
                try { dv.Columns["FileFlag"].ReadOnly = true; }
                catch { }
                try { dv.Columns["FilePath"].ReadOnly = true; }
                catch { }
                try { dv.Columns["FileName"].ReadOnly = true; }
                catch { }
                try { dv.Columns["DirName"].ReadOnly = true; }
                catch { }
                try { dv.Columns["DirPath"].ReadOnly = true; }
                catch { }
                try { dv.Columns["FileExt"].ReadOnly = true; }
                catch { }
                try { dv.Columns["FileSize"].ReadOnly = true; }
                catch { }
                try { dv.Columns["FileSizeBytes"].ReadOnly = true; }
                catch { }
                try { dv.Columns["CreationTime"].ReadOnly = true; }
                catch { }
                try { dv.Columns["LastAccessTime"].ReadOnly = true; }
                catch { }
                try { dv.Columns["LastWriteTime"].ReadOnly = true; }
                catch { }
                try { dv.Columns["IsReadOnly"].ReadOnly = true; }
                catch { }
                try { dv.Columns["Attributes"].ReadOnly = true; }
                catch { }
                try { dv.Columns["FileExists"].ReadOnly = true; }
                catch { }
                try { dv.Columns["FileAction"].ReadOnly = true; }
                catch { }
                try { dv.Columns["Tags"].ReadOnly = true; }
                catch { }
                try { dv.Columns["Notes"].ReadOnly = true; }
                catch { }
            }

            /// <summary></summary>
            /// <param name="DataGridView dv"> </param>
            public void HideShow_FileIndex_Columns(DataGridView dv)
            {

                try { dv.Columns["FID"].Visible = true; }
                catch { }
                try { dv.Columns["FileFlag"].Visible = true; }
                catch { }
                try { dv.Columns["FilePath"].Visible = true; }
                catch { }
                try { dv.Columns["FileName"].Visible = true; }
                catch { }
                try { dv.Columns["DirName"].Visible = true; }
                catch { }
                try { dv.Columns["DirPath"].Visible = true; }
                catch { }
                try { dv.Columns["FileExt"].Visible = true; }
                catch { }
                try { dv.Columns["FileSize"].Visible = true; }
                catch { }
                try { dv.Columns["FileSizeBytes"].Visible = true; }
                catch { }
                try { dv.Columns["CreationTime"].Visible = true; }
                catch { }
                try { dv.Columns["LastAccessTime"].Visible = true; }
                catch { }
                try { dv.Columns["LastWriteTime"].Visible = true; }
                catch { }
                try { dv.Columns["IsReadOnly"].Visible = true; }
                catch { }
                try { dv.Columns["Attributes"].Visible = true; }
                catch { }
                try { dv.Columns["FileExists"].Visible = true; }
                catch { }
                try { dv.Columns["FileAction"].Visible = true; }
                catch { }
                try { dv.Columns["Tags"].Visible = true; }
                catch { }
                try { dv.Columns["Notes"].Visible = true; }
                catch { }
            }


            #endregion

            #region === Update/Insert FileIndex  ===

            /// <summary>Return list of DirToDb Files</summary>
            /// <param name="DbFile"> </param>
            /// <param name="TableName"> </param>
            /// <param name="Or_SQL_Command"> </param>
            public List<FileIndex> Dir2Db_List(string DbFile = "Settings.sqlite", string TableName = "FileIndex", string Or_SQL_Command = "")
            {
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }
                if (TableName == "") { TableName = "FileIndex"; }

                string SelectLine = "Select * From [" + TableName + "]";

                if (Or_SQL_Command != "") { SelectLine = Or_SQL_Command; }

                //if (WhereClause != "")
                //{
                //    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                //    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                //}
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                List<FileIndex> ReturnList = new List<FileIndex>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        FileIndex returnObject = new FileIndex();

                        returnObject.FID = ret["FID"].ToString();
                        returnObject.FileFlag = ret["FileFlag"].ToString();
                        returnObject.FilePath = ret["FilePath"].ToString();
                        returnObject.FileName = ret["FileName"].ToString();
                        returnObject.DirName = ret["DirName"].ToString();
                        returnObject.DirPath = ret["DirPath"].ToString();
                        returnObject.FileExt = ret["FileExt"].ToString();
                        returnObject.FileSize = ret["FileSize"].ToString();
                        returnObject.FileSizeBytes = ret["FileSizeBytes"].ToString();
                        returnObject.CreationTime = ret["CreationTime"].ToString();
                        returnObject.LastAccessTime = ret["LastAccessTime"].ToString();
                        returnObject.LastWriteTime = ret["LastWriteTime"].ToString();
                        returnObject.IsReadOnly = ret["IsReadOnly"].ToString();
                        returnObject.Attributes = ret["Attributes"].ToString();
                        returnObject.FileExists = ret["FileExists"].ToString();
                        returnObject.FileAction = ret["FileAction"].ToString();
                        returnObject.Tags = ret["Tags"].ToString();
                        returnObject.Notes = ret["Notes"].ToString();

                        ReturnList.Add(returnObject);
                    }
                }

                return ReturnList;
            }

            /// <summary>Return File from DirToDb Database</summary>
            /// <param name="Where"> </param>
            /// <param name="DbFile"> </param>
            public FileIndex Dir2Db_Return(string Where = "[FilePath] = ", string DbFile = "Settings.sqlite", string TableName = "FileIndex")
            {
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                string SelectLine = "Select [FID], [FileFlag], [FilePath], [FileName], [DirName], [DirPath], [FileExt], [FileSize], [FileSizeBytes], [CreationTime], [LastAccessTime], [LastWriteTime], [IsReadOnly], [Attributes], [FileExists], [FileAction], [Tags], [Notes] From [" + TableName + "] WHERE " + Where;
                FileIndex returnObject = new FileIndex();

                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                int i = 0;
                string Value = "";
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.FID = ret["FID"].ToString();
                        returnObject.FileFlag = ret["FileFlag"].ToString();
                        returnObject.FilePath = ret["FilePath"].ToString();
                        returnObject.FileName = ret["FileName"].ToString();
                        returnObject.DirName = ret["DirName"].ToString();
                        returnObject.DirPath = ret["DirPath"].ToString();
                        returnObject.FileExt = ret["FileExt"].ToString();
                        returnObject.FileSize = ret["FileSize"].ToString();
                        returnObject.FileSizeBytes = ret["FileSizeBytes"].ToString();
                        returnObject.CreationTime = ret["CreationTime"].ToString();
                        returnObject.LastAccessTime = ret["LastAccessTime"].ToString();
                        returnObject.LastWriteTime = ret["LastWriteTime"].ToString();
                        returnObject.IsReadOnly = ret["IsReadOnly"].ToString();
                        returnObject.Attributes = ret["Attributes"].ToString();
                        returnObject.FileExists = ret["FileExists"].ToString();
                        returnObject.FileAction = ret["FileAction"].ToString();
                        returnObject.Tags = ret["Tags"].ToString();
                        returnObject.Notes = ret["Notes"].ToString();
                    }
                }

                return returnObject;
            }


            // check to see if FilePath exists, if found return FileIndex object, otherwise returns null object
            public FileIndex Dir2Db_File(string FieldName = "FilePath", string FieldValue = "", string DbFile = "Settings.sqlite", string TableName = "FileIndex")
            {
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                FileIndex fi = Dir2Db_Return("[" + FieldName + "] = '" + FieldValue.Trim() + "'", DbFile, TableName);

                string FID = fi.FID;
                if (FID == "" || FID == null) { return fi; }

                return fi;
            }

            /// <summary>Update File Entry in Dir2db Index based on FID value</summary>
            /// <param name="FileIndex inObject">Object containing file details written to Dir2Db database </param>
            /// <param name="DbFile">Path to sqlite db file</param>
            /// <param name="TableName">File Index table name to update</param>
            public bool Dir2Db_Update(FileIndex inObject, string DbFile = "Settings.sqlite", string TableName = "FileIndex")
            {
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                string UpdateLine = Dir2Db_UpdateLINE(inObject, DbFile, TableName);
                bool Updated = sqlite.Execute(DbFile, UpdateLine);

                return Updated;
            }

            // returns sqlite Update command to update project in batch command 
            public string Dir2Db_UpdateLINE(FileIndex inObject, string DbFile = "Settings.sqlite", string TableName = "FileIndex")
            {
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                string FileName = ahk.StringReplace(inObject.FileName, "'", "''", "ALL");
                string FilePath = ahk.StringReplace(inObject.FilePath, "'", "''", "ALL");
                string DirName = ahk.StringReplace(inObject.DirName, "'", "''", "ALL");
                string DirPath = ahk.StringReplace(inObject.DirPath, "'", "''", "ALL");
                string Tags = ahk.StringReplace(inObject.Tags, "'", "''", "ALL");
                string Notes = ahk.StringReplace(inObject.Notes, "'", "''", "ALL");
                string FileAction = ahk.StringReplace(inObject.FileAction, "'", "''", "ALL");

                string UpdateLine = "Update [" + TableName + "] set FilePath = '" + FilePath + "', FileFlag = '" + inObject.FileFlag + "', FilePath = '" + FilePath + "', FileName = '" + FileName + "', DirName = '" + DirName + "', DirPath = '" + DirPath + "', FileExt = '" + inObject.FileExt + "', FileSize = '" + inObject.FileSize + "', FileSizeBytes = '" + inObject.FileSizeBytes + "', CreationTime = '" + inObject.CreationTime + "', LastAccessTime = '" + inObject.LastAccessTime + "', LastWriteTime = '" + inObject.LastWriteTime + "', IsReadOnly = '" + inObject.IsReadOnly + "', Attributes = '" + inObject.Attributes + "', FileExists = '" + inObject.FileExists + "', FileAction = '" + FileAction + "', Tags = '" + Tags + "', Notes = '" + Notes + "' WHERE FID = '" + inObject.FID + "'";

                return UpdateLine;
            }



            /// <summary></summary>
            /// <param name="FileIndex inObject"> </param>
            /// <param name="DbFile"> </param>
            public bool Dir2Db_Insert(FileIndex inObject, string DbFile = "Settings.sqlite", string TableName = "FileIndex")
            {
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                string InsertLine = "Insert Into [" + TableName + "] (FID, FileFlag, FilePath, FileName, DirName, DirPath, FileExt, FileSize, FileSizeBytes, CreationTime, LastAccessTime, LastWriteTime, IsReadOnly, Attributes, FileExists, FileAction, Tags, Notes) values ('" + inObject.FID + "', '" + inObject.FileFlag + "', '" + inObject.FilePath + "', '" + inObject.FileName + "', '" + inObject.DirName + "', '" + inObject.DirPath + "', '" + inObject.FileExt + "', '" + inObject.FileSize + "', '" + inObject.FileSizeBytes + "', '" + inObject.CreationTime + "', '" + inObject.LastAccessTime + "', '" + inObject.LastWriteTime + "', '" + inObject.IsReadOnly + "', '" + inObject.Attributes + "', '" + inObject.FileExists + "', '" + inObject.FileAction + "', '" + inObject.Tags + "', '" + inObject.Notes + "')";
                bool Inserted = sqlite.Execute(DbFile, InsertLine);
                return Inserted;
            }

            /// <summary>Clear contents of FileIndex Table</summary>
            /// <param name="FileIndex inObject"> </param>
            /// <param name="DbFile"> </param>
            public bool Dir2Db_Clear(string TableName = "FileIndex", string DbFile = "Settings.sqlite", string WHERE = "")
            {
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }
                string cmd = "Delete from [" + TableName + "] ";
                if (WHERE != "") { cmd = cmd + WHERE; }
                bool Cleared = sqlite.Execute(DbFile, cmd);

                //if (GlobalDebug) { ahk.MsgBox("Inserted Into [FileIndex] = " + Inserted.ToString()); }
                return Cleared;
            }


            #endregion


            public List<FileIndex> ReturnDrive(string DriveLetter)
            {
                return Dir2Db_List(ahk.AppDir() + "\\Settings.sqlite", "FileIndex", "Select * From FileIndex WHERE FilePath LIKE '%" + DriveLetter + "%'");
            }

            #endregion


            #region === Table Compare To Files ===

            /// <summary>returns datatable used to populate initial datagridview when running a file check against a FileIndex db</summary>
            public DataTable Return_FileIndexCheck_BlankTable()
            {
                // Here we create a DataTable with three columns.
                DataTable table = new DataTable();
                table.Columns.Add("FilePath", typeof(string));
                table.Columns.Add("Found", typeof(string));
                table.Columns.Add("Changed", typeof(string));

                return table;
            }


            #endregion







            #endregion


            #region === SQL: DirToDb ===

            /// <summary>Save Contents of Directory to .sqlite Table</summary>
            /// <param name="DirPath">Directory Path / Drive to Index</param>
            /// <param name="FilePattern">Option to Only Add Specific File Pattern. Default = All Files (*.*)</param>
            /// <param name="Recurse">Option to Search Folders Underneath DirPath (Default = True)</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To Settings.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To. Defaults To 'FileIndex'</param>
            /// <param name="OverWriteTable">Option to OverWrite Existing FileIndex Table (Default = True)</param>
            /// <control>Assign Label/TextBox to displayControl before running to view current status while executing. EX: "sqlite.displayControl = TextBox1;"</control>
            public bool DirToDbSQL(string DirPath, string FilePattern = "*.*", bool Recurse = true, SqlConnection Conn = null, string TableName = "[ADBIndex].[dbo].[FileTable]", bool OverWriteTable = true, RadProgressBar pr = null)
            {
                _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();

                // Create New Table If It Does NOT Exist Yet
                Create_FileTable(Conn, TableName);

                //=============================================================================================
                // Get list of files in the specific directory - Recursive File Search - Fill Data Table
                //=============================================================================================

                var stopwatch = new Stopwatch(); stopwatch.Start();


                // search all directories unless user selects Recurse = False
                SearchOption recurse = SearchOption.AllDirectories;
                if (!Recurse) { recurse = SearchOption.TopDirectoryOnly; }

                //string[] files = Directory.GetFiles(DirPath, FilePattern, recurse);

                var files = GetAllAccessibleFiles(DirPath);

                int totalFileCount = files.Count;

                // setup progress bar if progress bar provided
                if (pr != null)
                {
                    //frm.SetupProgressBar(pr, totalFileCount);  // OLD PROGRESS BAR CONFIG
                    pro.SetupProgressBar(pr, files.Count);
                }

                // update displayControl Text with progress (if user defined this control name)
                if (!Loading_DirToDb_LIST) { DirToDb_DisplayText("Total Files To Index = " + totalFileCount.ToString()); }

                int FileCount = 0;

                // Here we create a DataTable with four columns.
                DataTable table = new DataTable();
                table.Columns.Add("FileFlag", typeof(string));
                table.Columns.Add("FilePath", typeof(string));
                table.Columns.Add("FileName", typeof(string));
                table.Columns.Add("DirPath", typeof(string));
                table.Columns.Add("FileExt", typeof(string));
                table.Columns.Add("FileSize", typeof(string));
                table.Columns.Add("CreationTime", typeof(string));
                table.Columns.Add("LastAccessTime", typeof(string));
                table.Columns.Add("LastWriteTime", typeof(string));
                table.Columns.Add("IsReadOnly", typeof(bool));
                table.Columns.Add("Attributes", typeof(string));
                table.Columns.Add("FileExists", typeof(string));
                table.Columns.Add("FileAction", typeof(string));
                table.Columns.Add("Tags", typeof(string));
                table.Columns.Add("Notes", typeof(string));
                //table.Columns.Add("", typeof(byte[]));
                //table.Columns.Add("CreationTime", typeof(string));


                int i = 1;
                foreach (string file in files)  // loop through list of files and write file details to SQLite db
                {
                    if (pr != null)
                    {
                        //frm.UpdateProgressBar(pr); // update progressbar if setup  OLD PROGRESS BAR
                        pro.UpdateProgress(pr, i + "/" + files.Count); i++;
                    }

                    System.IO.FileInfo fileinfo = new System.IO.FileInfo(file); //retrieve info about each file

                    FileTable obj = new FileTable();

                    if (fileinfo.Exists)
                    {
                        //string FileName = fileinfo.Name.ToString();

                        string FileSizeBytes = fileinfo.Length.ToString();  // # of bytes as an int
                        string FileSize = ahk.FormatBytes(fileinfo.Length);  // convert bytes to Text representation (adds kb/mb/tb to return)

                        //string LastWriteTime = "";
                        //try { fileinfo.LastWriteTime.ToString(); }
                        //catch { }

                        //string LastAccessTime = "";
                        //try { fileinfo.LastAccessTime.ToString(); }
                        //catch { }

                        //string CreationTime = "";
                        //try { fileinfo.CreationTime.ToString(); }
                        //catch { }

                        //bool IsReadOnly = fileinfo.IsReadOnly;
                        //string FileExt = fileinfo.Extension.ToString();
                        //FileExt = ahk.StringReplace(FileExt, "."); // remove period from file ext

                        //bool FileExists = fileinfo.Exists;
                        //string DirName = fileinfo.DirectoryName.ToString();
                        //string dirPath = fileinfo.Directory.ToString();

                        //string Attributes = fileinfo.Attributes.ToString();
                        //string FileAction = ""; // variable used to indicate a file is queued to be copied to another location 
                        //string FilePath = file;
                        //bool FileFlag = false;


                        obj.FilePath = file;
                        obj.FileFound = fileinfo.Exists;
                        obj.FileFlag = false;
                        obj.FileDate = fileinfo.LastWriteTime;
                        obj.DirName = ahk.DirName(file);
                        obj.FileName = ahk.FileName(file);
                        obj.FileSize = FileSize;
                        obj.FileTags = ahk.DirName(file);


                        //DirName = ahk.FixSpecialChars(DirName); //remove invalid characters before writing
                        //DirPath = ahk.FixSpecialChars(DirPath); //remove invalid characters before writing
                        //FileName = ahk.FixSpecialChars(FileName); //remove invalid characters before writing
                        //FilePath = ahk.FixSpecialChars(FilePath);//remove invalid characters before writing
                        //string DirName = ahk.FixSpecialChars(ahk.DirName(file));
                        //string FileName = ahk.FixSpecialChars(ahk.FileName(file));
                        //string dirPath = ahk.FixSpecialChars(ahk.FileDir(file));

                        //string fileHash = ahk.FileHash(file);  // returns hash for file 

                        //string[] paths = DirName.Split('\\'); // split the directory path to get the name
                        //DirName = paths[paths.Length - 1]; //returns last folder name in string

                        //FilePath = FilePath.Replace("'", "''");
                        //DirPath = DirPath.Replace("'", "''");
                        //FileName = FileName.Replace("'", "''");
                        //DirName = DirName.Replace("'", "''");

                        //FilePath = ahk.StringReplace(FilePath, "'", "''");
                        //DirPath = ahk.StringReplace(DirPath, "'", "''");
                        //DirName = ahk.StringReplace(DirName, "'", "''");
                        //FileName = ahk.StringReplace(FileName, "'", "''");

                        //FilePath = CleanInput(FilePath);
                        //dirPath = CleanInput(dirPath);
                        //FileName = CleanInput(FileName);
                        //DirName = CleanInput(DirName);
                        //string InsertLine = "INSERT into [" + TableName + "] (FileFlag, FilePath, FileName, DirName, DirPath, FileExt, FileSize, FileSizeBytes, CreationTime, LastAccessTime, LastWriteTime, IsReadOnly, Attributes, FileExists, FileAction, Tags, Notes) values ('" + FileFlag + "', '" + FilePath + "', '" + FileName + "', '" + DirName + "', '" + dirPath + "', '" + FileExt + "', '" + FileSize + "', '" + FileSizeBytes + "', '" + CreationTime + "', '" + LastAccessTime + "', '" + LastWriteTime + "', '" + IsReadOnly + "', '" + Attributes + "', '" + FileExists + "', '" + FileAction + "', '" + DirName + "', '" + fileHash + "');";

                        //table.Rows.Add(FileFlag,FilePath,FileName,DirName,FileExt, FileSizeBytes, ahk.ToDateTime(CreationTime), ahk.ToDateTime(LastAccessTime), ahk.ToDateTime(LastWriteTime),IsReadOnly,Attributes,FileExists,FileAction,DirName,fileHash);
                        //table.Rows.Add(FileFlag, FilePath, FileName, DirName, FileExt, FileSizeBytes, IsReadOnly, Attributes, FileExists, FileAction, DirName, fileHash);


                        // if (InsertLine.Contains("zix")) { ahk.MsgBox(InsertLine); }

                        //cmd.CommandText = InsertLine;
                        //cmd.ExecuteNonQuery();
                        FileCount++;

                        // loading single directory 
                        //if (!Loading_DirToDb_LIST) { DirToDb_DisplayText("Adding File " + FileCount.ToString() + " / " + totalFileCount.ToString()); }

                        // loading multiple directories in list
                        //if (Loading_DirToDb_LIST) { DirToDb_DisplayText("Adding File " + FileCount.ToString() + " / " + totalFileCount.ToString() + "  |  Dir " + Loading_DirToDb_LIST_CurrentCount.ToString() + " / " + Loading_DirToDb_LIST_TotalCount.ToString()); }


                        FileTable_UpdateInsert(obj, false, false);


                        //    DateTime LastSyncDate = new DateTime(2016, 06, 25);

                        //    if (fileinfo.LastWriteTime > LastSyncDate)
                        //    {
                        //        //ahk.MsgBox("Need to Add - " + fileinfo.LastWriteTime.ToString());
                        //        string InsertLine = "INSERT into " + TableName + " (FileFlag, FilePath, FileName, DirName, DirPath, FileExt, FileSize, FileSizeBytes, CreationTime, LastAccessTime, LastWriteTime, IsReadOnly, Attributes, FileExists, FileAction, Tags) values ('" + FileFlag + "', '" + FilePath + "', '" + FileName + "', '" + DirName + "', '" + DirPath + "', '" + FileExt + "', '" + FileSize + "', '" + FileSizeBytes + "', '" + CreationTime + "', '" + LastAccessTime + "', '" + LastWriteTime + "', '" + IsReadOnly + "', '" + Attributes + "', '" + FileExists + "', '" + FileAction + "', '" + DirName + "')";
                        //        cmd.CommandText = InsertLine + ";";
                        //        cmd.ExecuteNonQuery();
                        //    }

                    } // if file exists on drive
                }


                //sb("Clearing Previous CodeTable");
                //sql.WriteDataRecord(Conn, "Delete From " + TableName + " Where FileName = '" + ahk.FileName(filePath) + "'");



                // BATCH INSERT OPTION
                //sql.SQL_BatchInsert(Conn, TableName, table);




                //sb("Finished CodeTable Insert : " + ahk.FileName(filePath));



                //if (CreateIndexTable)
                //{
                //    string IndexTimeSeconds = stopwatch.Elapsed.TotalSeconds.ToString();

                //    // create index entry / update existing entry if found
                //    string Label = ahk.DirName(DirPath);
                //    string Index_InsertLine = "Insert Into [" + IndexTableName + "] (Enabled, TableName, Label, DirPath, FileCount, CaptureTime, LastUpdated) VALUES ('true', '" + TableName + "', '" + Label + "', '" + DirPath + "', '" + FileCount + "', '" + IndexTimeSeconds + "', '" + DateTime.Now + "')";
                //    string Index_UpdateLine = "Update [" + IndexTableName + "] Set Enabled = 'true', TableName = '" + TableName + "', Label = '" + Label + "', DirPath = '" + DirPath + "', FileCount = '" + FileCount + "', CaptureTime = '" + IndexTimeSeconds + "', LastUpdated = '" + DateTime.Now + "' where DirPath = '" + DirPath + "'";
                //    sqlite.Update_Insert(DbFile, Index_UpdateLine, Index_InsertLine);
                //}

                // reset progress bar if provided
                //if (pr != null) { frm.SetupProgressBar(pr, 1); }

                //Console.WriteLine("{0} seconds with one transaction.", stopwatch.Elapsed.TotalSeconds);
                return true;
            }


            // Create SQL Table for DirToDB SQL (if it doesn't already exist)
            public bool Create_FileTable(SqlConnection Conn, string TablePath = "[ADBIndex].[dbo].[FileTable]")
            {
                string createLine = "CREATE TABLE " + TablePath + " ([ID] [bigint] IDENTITY(1,1) NOT NULL,[FilePath] [nvarchar](max) NULL,[FileName] [nvarchar](max) NULL,[DirName] [nvarchar](max) NULL,[FileSize] [nvarchar](max) NULL,[FileDate] [datetime] NULL,[FileTags] [nvarchar](max) NULL,[FileAction] [nvarchar](max) NULL,[FileFound] [bit] NULL,[FileFlag] [bit] NULL) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";

                bool AlreadyExists = sql.TableExist(Conn, TablePath);

                if (AlreadyExists) { return true; }
                else
                {
                    sql.Write_SQL(Conn, createLine);

                    ahk.Sleep(500);

                    return sql.TableExist(Conn, TablePath);
                }
            }




            #region === FileTable FUNCTIONS ===

            #region ===== FileTable Object =====

            public struct FileTable
            {
                public string ID { get; set; }
                public string FilePath { get; set; }
                public string FileName { get; set; }
                public string DirName { get; set; }
                public string FileSize { get; set; }
                public DateTime FileDate { get; set; }
                public string FileTags { get; set; }
                public string FileAction { get; set; }
                public bool FileFound { get; set; }
                public bool FileFlag { get; set; }
            }

            // Compare two objects to see if they have identical values
            public bool FileTable_Changed(FileTable OldVal, FileTable NewVal)
            {
                FileTable diff = new FileTable();
                List<string> diffList = new List<string>();
                bool different = false;
                if (OldVal.ID == null) { OldVal.ID = ""; }
                if (NewVal.ID == null) { NewVal.ID = ""; }
                if (OldVal.ID != NewVal.ID) { different = true; }
                if (OldVal.FilePath == null) { OldVal.FilePath = ""; }
                if (NewVal.FilePath == null) { NewVal.FilePath = ""; }
                if (OldVal.FilePath != NewVal.FilePath) { different = true; }
                if (OldVal.FileName == null) { OldVal.FileName = ""; }
                if (NewVal.FileName == null) { NewVal.FileName = ""; }
                if (OldVal.FileName != NewVal.FileName) { different = true; }
                if (OldVal.DirName == null) { OldVal.DirName = ""; }
                if (NewVal.DirName == null) { NewVal.DirName = ""; }
                if (OldVal.DirName != NewVal.DirName) { different = true; }
                if (OldVal.FileSize == null) { OldVal.FileSize = ""; }
                if (NewVal.FileSize == null) { NewVal.FileSize = ""; }
                if (OldVal.FileSize != NewVal.FileSize) { different = true; }
                //if (OldVal.FileDate == null) { OldVal.FileDate = ""; }
                //if (NewVal.FileDate == null) { NewVal.FileDate = ""; }
                if (OldVal.FileDate != NewVal.FileDate) { different = true; }
                if (OldVal.FileTags == null) { OldVal.FileTags = ""; }
                if (NewVal.FileTags == null) { NewVal.FileTags = ""; }
                if (OldVal.FileTags != NewVal.FileTags) { different = true; }
                if (OldVal.FileAction == null) { OldVal.FileAction = ""; }
                if (NewVal.FileAction == null) { NewVal.FileAction = ""; }
                if (OldVal.FileAction != NewVal.FileAction) { different = true; }
                //if (OldVal.FileFound == null) { OldVal.FileFound = ""; }
                //if (NewVal.FileFound == null) { NewVal.FileFound = ""; }
                if (OldVal.FileFound != NewVal.FileFound) { different = true; }
                //if (OldVal.FileFlag == null) { OldVal.FileFlag = ""; }
                //if (NewVal.FileFlag == null) { NewVal.FileFlag = ""; }
                if (OldVal.FileFlag != NewVal.FileFlag) { different = true; }
                return different;
            }

            // Returns object containing the new values different from the old values in object comparison
            public FileTable FileTable_Diff(FileTable OldVal, FileTable NewVal)
            {
                FileTable diff = new FileTable();
                if (OldVal.ID != NewVal.ID) { diff.ID = NewVal.ID; }
                if (OldVal.FilePath != NewVal.FilePath) { diff.FilePath = NewVal.FilePath; }
                if (OldVal.FileName != NewVal.FileName) { diff.FileName = NewVal.FileName; }
                if (OldVal.DirName != NewVal.DirName) { diff.DirName = NewVal.DirName; }
                if (OldVal.FileSize != NewVal.FileSize) { diff.FileSize = NewVal.FileSize; }
                if (OldVal.FileDate != NewVal.FileDate) { diff.FileDate = NewVal.FileDate; }
                if (OldVal.FileTags != NewVal.FileTags) { diff.FileTags = NewVal.FileTags; }
                if (OldVal.FileAction != NewVal.FileAction) { diff.FileAction = NewVal.FileAction; }
                if (OldVal.FileFound != NewVal.FileFound) { diff.FileFound = NewVal.FileFound; }
                if (OldVal.FileFlag != NewVal.FileFlag) { diff.FileFlag = NewVal.FileFlag; }
                return diff;
            }

            // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
            public List<string> FileTable_DiffList(FileTable OldVal, FileTable NewVal)
            {
                List<string> diffList = new List<string>();
                if (OldVal.ID != NewVal.ID) { diffList.Add("Changed ID Value From " + OldVal.ID + " To " + NewVal.ID); }
                if (OldVal.FilePath != NewVal.FilePath) { diffList.Add("Changed FilePath Value From " + OldVal.FilePath + " To " + NewVal.FilePath); }
                if (OldVal.FileName != NewVal.FileName) { diffList.Add("Changed FileName Value From " + OldVal.FileName + " To " + NewVal.FileName); }
                if (OldVal.DirName != NewVal.DirName) { diffList.Add("Changed DirName Value From " + OldVal.DirName + " To " + NewVal.DirName); }
                if (OldVal.FileSize != NewVal.FileSize) { diffList.Add("Changed FileSize Value From " + OldVal.FileSize + " To " + NewVal.FileSize); }
                if (OldVal.FileDate != NewVal.FileDate) { diffList.Add("Changed FileDate Value From " + OldVal.FileDate + " To " + NewVal.FileDate); }
                if (OldVal.FileTags != NewVal.FileTags) { diffList.Add("Changed FileTags Value From " + OldVal.FileTags + " To " + NewVal.FileTags); }
                if (OldVal.FileAction != NewVal.FileAction) { diffList.Add("Changed FileAction Value From " + OldVal.FileAction + " To " + NewVal.FileAction); }
                if (OldVal.FileFound != NewVal.FileFound) { diffList.Add("Changed FileFound Value From " + OldVal.FileFound + " To " + NewVal.FileFound); }
                if (OldVal.FileFlag != NewVal.FileFlag) { diffList.Add("Changed FileFlag Value From " + OldVal.FileFlag + " To " + NewVal.FileFlag); }
                return diffList;
            }

            // Generate XML String From Object
            public string FileTable_ToXML(FileTable obj)
            {
                string XMLString = "";
                XMLString = XMLString + "<ID>" + obj.ID + "</ID>";
                XMLString = XMLString + "<FilePath>" + obj.FilePath + "</FilePath>";
                XMLString = XMLString + "<FileName>" + obj.FileName + "</FileName>";
                XMLString = XMLString + "<DirName>" + obj.DirName + "</DirName>";
                XMLString = XMLString + "<FileSize>" + obj.FileSize + "</FileSize>";
                XMLString = XMLString + "<FileDate>" + obj.FileDate + "</FileDate>";
                XMLString = XMLString + "<FileTags>" + obj.FileTags + "</FileTags>";
                XMLString = XMLString + "<FileAction>" + obj.FileAction + "</FileAction>";
                XMLString = XMLString + "<FileFound>" + obj.FileFound + "</FileFound>";
                XMLString = XMLString + "<FileFlag>" + obj.FileFlag + "</FileFlag>";
                return XMLString;
            }

            // Populate Object from XML Tag String
            public FileTable FileTable_FromXML(string XMLString)
            {
                _Parse prs = new _Parse();
                FileTable obj = new FileTable();
                //obj.ID = prs.XML_Text(XMLString, "<ID>");
                //obj.FilePath = prs.XML_Text(XMLString, "<FilePath>");
                //obj.FileName = prs.XML_Text(XMLString, "<FileName>");
                //obj.DirName = prs.XML_Text(XMLString, "<DirName>");
                //obj.FileSize = prs.XML_Text(XMLString, "<FileSize>");
                //obj.FileDate = prs.XML_Text(XMLString, "<FileDate>");
                //obj.FileTags = prs.XML_Text(XMLString, "<FileTags>");
                //obj.FileAction = prs.XML_Text(XMLString, "<FileAction>");
                //obj.FileFound = prs.XML_Text(XMLString, "<FileFound>");
                //obj.FileFlag = prs.XML_Text(XMLString, "<FileFlag>");
                return obj;
            }


            #endregion
            public bool Create_Table_FileTable(string DbFile)
            {
                string CreateLine = "Create Table [FileTable] (ID INTEGER PRIMARY KEY, FilePath VARCHAR, FileName VARCHAR, DirName VARCHAR, FileSize VARCHAR, FileDate VARCHAR, FileTags VARCHAR, FileAction VARCHAR, FileFound VARCHAR, FileFlag VARCHAR)";
                bool TableCreated = sqlite.Table_Exists(DbFile, "FileTable");
                if (!TableCreated) { TableCreated = sqlite.Table_New(DbFile, "FileTable", "Create Table [FileTable] (ID INTEGER PRIMARY KEY, FilePath VARCHAR, FileName VARCHAR, DirName VARCHAR, FileSize VARCHAR, FileDate VARCHAR, FileTags VARCHAR, FileAction VARCHAR, FileFound VARCHAR, FileFlag VARCHAR", false); }


                if (!TableCreated) { ahk.MsgBox("[FileTable] Created = " + TableCreated.ToString()); }
                return TableCreated;
            }

            #region ===== FileTable SQLite : Return =====

            public FileTable Return_Object_From_FileTable(string WhereClause = "[ID] = ''", string DbFile = "")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\FileTable.sqlite"; }
                string SelectLine = "Select [ID], [FilePath], [FileName], [DirName], [FileSize], [FileDate], [FileTags], [FileAction], [FileFound], [FileFlag] From [FileTable] ";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
                FileTable returnObject = new FileTable();
                int i = 0;
                string Value = "";
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.FilePath = ret["FilePath"].ToString();
                        returnObject.FileName = ret["FileName"].ToString();
                        returnObject.DirName = ret["DirName"].ToString();
                        returnObject.FileSize = ret["FileSize"].ToString();
                        returnObject.FileDate = ret["FileDate"].ToDateTime();
                        returnObject.FileTags = ret["FileTags"].ToString();
                        returnObject.FileAction = ret["FileAction"].ToString();
                        returnObject.FileFound = ret["FileFound"].ToBool();
                        returnObject.FileFlag = ret["FileFlag"].ToBool();
                    }
                }

                return returnObject;
            }

            public List<FileTable> Return_FileTable_List(string WhereClause = "", string DbFile = "", string TableName = "[FileTable]")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\FileTable.sqlite"; }
                string SelectLine = "Select * From " + TableName;

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

                List<FileTable> ReturnList = new List<FileTable>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        FileTable returnObject = new FileTable();

                        returnObject.ID = ret["ID"].ToString();
                        returnObject.FilePath = ret["FilePath"].ToString();
                        returnObject.FileName = ret["FileName"].ToString();
                        returnObject.DirName = ret["DirName"].ToString();
                        returnObject.FileSize = ret["FileSize"].ToString();
                        returnObject.FileDate = ret["FileDate"].ToDateTime();
                        returnObject.FileTags = ret["FileTags"].ToString();
                        returnObject.FileAction = ret["FileAction"].ToString();
                        returnObject.FileFound = ret["FileFound"].ToBool();
                        returnObject.FileFlag = ret["FileFlag"].ToBool();

                        ReturnList.Add(returnObject);
                    }
                }

                return ReturnList;
            }

            public DataTable Return_DataTable_From_FileTable(string DbFile)
            {
                string SelectLine = "Select [ID], [FilePath], [FileName], [DirName], [FileSize], [FileDate], [FileTags], [FileAction], [FileFound], [FileFlag] From [FileTable]";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                return ReturnTable;
            }


            #endregion
            #region ===== FileTable SQLite : Update Insert =====

            public bool FileTable_Insert(FileTable inObject, string DbFile = "")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\FileTable.sqlite"; }
                string InsertLine = "Insert Into [FileTable] (ID, FilePath, FileName, DirName, FileSize, FileDate, FileTags, FileAction, FileFound, FileFlag) values ('" + inObject.ID + "', '" + inObject.FilePath + "', '" + inObject.FileName + "', '" + inObject.DirName + "', '" + inObject.FileSize + "', '" + inObject.FileDate + "', '" + inObject.FileTags + "', '" + inObject.FileAction + "', '" + inObject.FileFound + "', '" + inObject.FileFlag + "')";
                bool Inserted = sqlite.Execute(DbFile, InsertLine);
                if (!Inserted) { ahk.MsgBox("Inserted Into [FileTable] = " + Inserted.ToString()); }
                return Inserted;
            }

            public bool FileTable_Update(FileTable inObject, string DbFile = "")
            {
                //string UpdateLine = "Update [FileTable] set ID = '" + inObject.ID + "', FilePath = '" + inObject.FilePath + "', FileName = '" + inObject.FileName + "', DirName = '" + inObject.DirName + "', FileSize = '" + inObject.FileSize + "', FileDate = '" + inObject.FileDate + "', FileTags = '" + inObject.FileTags + "', FileAction = '" + inObject.FileAction + "', FileFound = '" + inObject.FileFound + "', FileFlag = '" + inObject.FileFlag + "' WHERE [Item] = 'Value' ";
                string UpdateLine = "Update [FileTable] set ";


                if (inObject.ID != null) { UpdateLine = UpdateLine + "[ID] = '" + inObject.ID + "',"; }
                if (inObject.FilePath != null) { UpdateLine = UpdateLine + "[FilePath] = '" + inObject.FilePath + "',"; }
                if (inObject.FileName != null) { UpdateLine = UpdateLine + "[FileName] = '" + inObject.FileName + "',"; }
                if (inObject.DirName != null) { UpdateLine = UpdateLine + "[DirName] = '" + inObject.DirName + "',"; }
                if (inObject.FileSize != null) { UpdateLine = UpdateLine + "[FileSize] = '" + inObject.FileSize + "',"; }
                if (inObject.FileDate != null) { UpdateLine = UpdateLine + "[FileDate] = '" + inObject.FileDate + "',"; }
                if (inObject.FileTags != null) { UpdateLine = UpdateLine + "[FileTags] = '" + inObject.FileTags + "',"; }
                if (inObject.FileAction != null) { UpdateLine = UpdateLine + "[FileAction] = '" + inObject.FileAction + "',"; }
                if (inObject.FileFound != null) { UpdateLine = UpdateLine + "[FileFound] = '" + inObject.FileFound + "',"; }
                if (inObject.FileFlag != null) { UpdateLine = UpdateLine + "[FileFlag] = '" + inObject.FileFlag + "',"; }

                UpdateLine = ahk.TrimLast(UpdateLine, 1);
                UpdateLine = UpdateLine + " WHERE [ID] = ' '"; // DEFINE CONDITION HERE !!!

                bool Updated = sqlite.Execute(DbFile, UpdateLine);
                return Updated;
            }

            public bool FileTable_UpdateInsert(FileTable obj, string DbFile = "")
            {

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\FileTable.sqlite"; }
                if (!File.Exists(DbFile)) { Create_Table_FileTable(DbFile); }

                bool Updated = FileTable_Update(obj, DbFile);  // try to update record first
                if (!Updated) { Updated = FileTable_Insert(obj, DbFile); }  // if unable to update, insert new record
                return Updated;
            }


            #endregion
            #region ===== FileTable DataTable =====

            public DataTable Return_FileTable_DataTable(string DbFile = "", string TableName = "FileTable", string WhereClause = "", bool Debug = false)
            {

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\FileTable.sqlite"; }
                string SelectLine = "Select * From [FileTable]";

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }

                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


                DataTable table = new DataTable();
                table.Columns.Add("ID", typeof(string));
                table.Columns.Add("FilePath", typeof(string));
                table.Columns.Add("FileName", typeof(string));
                table.Columns.Add("DirName", typeof(string));
                table.Columns.Add("FileSize", typeof(string));
                table.Columns.Add("FileDate", typeof(string));
                table.Columns.Add("FileTags", typeof(string));
                table.Columns.Add("FileAction", typeof(string));
                table.Columns.Add("FileFound", typeof(string));
                table.Columns.Add("FileFlag", typeof(string));

                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        FileTable returnObject = new FileTable();

                        returnObject.ID = ret["ID"].ToString();
                        returnObject.FilePath = ret["FilePath"].ToString();
                        returnObject.FileName = ret["FileName"].ToString();
                        returnObject.DirName = ret["DirName"].ToString();
                        returnObject.FileSize = ret["FileSize"].ToString();
                        returnObject.FileDate = ret["FileDate"].ToDateTime();
                        returnObject.FileTags = ret["FileTags"].ToString();
                        returnObject.FileAction = ret["FileAction"].ToString();
                        returnObject.FileFound = ret["FileFound"].ToBool();
                        returnObject.FileFlag = ret["FileFlag"].ToBool();

                        table.Rows.Add(returnObject.ID, returnObject.FilePath, returnObject.FileName, returnObject.DirName, returnObject.FileSize, returnObject.FileDate, returnObject.FileTags, returnObject.FileAction, returnObject.FileFound, returnObject.FileFlag);
                    }
                }

                return table;
            }

            public DataTable Create_FileTable_DataTable(FileTable inObject)
            {
                DataTable table = new DataTable();
                table.Columns.Add("ID", typeof(string));
                table.Columns.Add("FilePath", typeof(string));
                table.Columns.Add("FileName", typeof(string));
                table.Columns.Add("DirName", typeof(string));
                table.Columns.Add("FileSize", typeof(string));
                table.Columns.Add("FileDate", typeof(string));
                table.Columns.Add("FileTags", typeof(string));
                table.Columns.Add("FileAction", typeof(string));
                table.Columns.Add("FileFound", typeof(string));
                table.Columns.Add("FileFlag", typeof(string));

                table.Rows.Add(inObject.ID, inObject.FilePath, inObject.FileName, inObject.DirName, inObject.FileSize, inObject.FileDate, inObject.FileTags, inObject.FileAction, inObject.FileFound, inObject.FileFlag);
                return table;
            }


            #endregion
            #region ===== FileTable DataGridView =====

            public void HideShow_FileTable_Columns(DataGridView dv)
            {

                try { dv.Columns["ID"].Visible = true; } catch { }
                try { dv.Columns["FilePath"].Visible = true; } catch { }
                try { dv.Columns["FileName"].Visible = true; } catch { }
                try { dv.Columns["DirName"].Visible = true; } catch { }
                try { dv.Columns["FileSize"].Visible = true; } catch { }
                try { dv.Columns["FileDate"].Visible = true; } catch { }
                try { dv.Columns["FileTags"].Visible = true; } catch { }
                try { dv.Columns["FileAction"].Visible = true; } catch { }
                try { dv.Columns["FileFound"].Visible = true; } catch { }
                try { dv.Columns["FileFlag"].Visible = true; } catch { }
            }
            public void Enable_FileTable_Columns(DataGridView dv)
            {

                try { dv.Columns["ID"].ReadOnly = true; } catch { }
                try { dv.Columns["FilePath"].ReadOnly = true; } catch { }
                try { dv.Columns["FileName"].ReadOnly = true; } catch { }
                try { dv.Columns["DirName"].ReadOnly = true; } catch { }
                try { dv.Columns["FileSize"].ReadOnly = true; } catch { }
                try { dv.Columns["FileDate"].ReadOnly = true; } catch { }
                try { dv.Columns["FileTags"].ReadOnly = true; } catch { }
                try { dv.Columns["FileAction"].ReadOnly = true; } catch { }
                try { dv.Columns["FileFound"].ReadOnly = true; } catch { }
                try { dv.Columns["FileFlag"].ReadOnly = true; } catch { }
            }

            #endregion
            #region ===== FileTable SQL Functions =====

            // Return FileTable SQL Connection String
            public SqlConnection FileTable_Conn()
            {
                // populate sql connection
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMADBIndex"].ConnectionString);
                // SqlConnection Conn = new SqlConnection("Server=188.168.188.88;DataBase=LucidMedia;Uid=lucidm;Pwd=pass");
                return conn;
            }

            // Return FileTable TableName (Full Path)
            public string FileTable_TableName()
            {
                // populate to return full sql table name
                return "[ADBIndex].[dbo].[FileTable]";
            }

            // Generate SQL Table
            public bool FileTable_CreateSQLTable()
            {
                SqlConnection Conn = FileTable_Conn();
                string CreateTableLine = "CREATE TABLE [FileTable](";
                CreateTableLine = CreateTableLine + "[ID] [int] IDENTITY(1,1) NOT NULL,";
                CreateTableLine = CreateTableLine + "[FilePath] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[FileName] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[DirName] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[FileSize] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[FileDate] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[FileTags] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[FileAction] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[FileFound] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[FileFlag] [varchar](max) NOT NULL";
                CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                return false;
            }

            public bool FileTable_InsertSQL(FileTable obj)
            {
                SqlConnection Con = FileTable_Conn();
                string SQLLine = "Insert Into " + FileTable_TableName() + " (FilePath, FileName, DirName, FileSize, FileDate, FileTags, FileAction, FileFound, FileFlag) VALUES (@FilePath, @FileName, @DirName, @FileSize, @FileDate, @FileTags, @FileAction, @FileFound, @FileFlag)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.FilePath == null) { obj.FilePath = ""; }
                if (obj.FileName == null) { obj.FileName = ""; }
                if (obj.DirName == null) { obj.DirName = ""; }
                if (obj.FileSize == null) { obj.FileSize = ""; }
                //if (obj.FileDate == null) { obj.FileDate = ""; }
                if (obj.FileTags == null) { obj.FileTags = ""; }
                if (obj.FileAction == null) { obj.FileAction = ""; }
                //if (obj.FileFound == null) { obj.FileFound = ""; }
                //if (obj.FileFlag == null) { obj.FileFlag = ""; }
                cmd2.Parameters.AddWithValue(@"FilePath", obj.FilePath.ToString());
                cmd2.Parameters.AddWithValue(@"FileName", obj.FileName.ToString());
                cmd2.Parameters.AddWithValue(@"DirName", obj.DirName.ToString());
                cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize.ToString());
                cmd2.Parameters.AddWithValue(@"FileDate", obj.FileDate.ToString());
                cmd2.Parameters.AddWithValue(@"FileTags", obj.FileTags.ToString());
                cmd2.Parameters.AddWithValue(@"FileAction", obj.FileAction.ToString());
                cmd2.Parameters.AddWithValue(@"FileFound", obj.FileFound.ToString());
                cmd2.Parameters.AddWithValue(@"FileFlag", obj.FileFlag.ToString());
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Con.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool FileTable_UpdateSQL(FileTable obj, bool ByID = true, bool UpdateTags = true)
            {
                SqlConnection Conn = FileTable_Conn();
                string SQLLine = "Update " + FileTable_TableName() + " SET FilePath = @FilePath, FileName = @FileName, DirName = @DirName, FileSize = @FileSize, FileDate = @FileDate, FileTags = @FileTags, FileAction = @FileAction, FileFound = @FileFound, FileFlag = @FileFlag WHERE ID = @ID";
                if (!UpdateTags) { SQLLine = "Update " + FileTable_TableName() + " SET FilePath = @FilePath, FileName = @FileName, DirName = @DirName, FileSize = @FileSize, FileDate = @FileDate, FileAction = @FileAction, FileFound = @FileFound, FileFlag = @FileFlag WHERE ID = @ID"; }

                if (!ByID)
                {
                    SQLLine = "Update " + FileTable_TableName() + " SET FileName = @FileName, DirName = @DirName, FileSize = @FileSize, FileDate = @FileDate, FileTags = @FileTags, FileAction = @FileAction, FileFound = @FileFound, FileFlag = @FileFlag WHERE FilePath = @FilePath";

                    if (!UpdateTags) { SQLLine = "Update " + FileTable_TableName() + " SET FilePath = @FilePath, FileName = @FileName, DirName = @DirName, FileSize = @FileSize, FileDate = @FileDate, FileAction = @FileAction, FileFound = @FileFound, FileFlag = @FileFlag WHERE FilePath = @FilePath"; }
                }

                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.ID == null) { obj.ID = ""; }
                if (obj.FilePath == null) { obj.FilePath = ""; }
                if (obj.FileName == null) { obj.FileName = ""; }
                if (obj.DirName == null) { obj.DirName = ""; }
                if (obj.FileSize == null) { obj.FileSize = ""; }
                //if (obj.FileDate == null) { obj.FileDate = ""; }
                if (obj.FileTags == null) { obj.FileTags = ""; }
                if (obj.FileAction == null) { obj.FileAction = ""; }
                //if (obj.FileFound == null) { obj.FileFound = ""; }
                //if (obj.FileFlag == null) { obj.FileFlag = ""; }

                if (ByID) { cmd2.Parameters.AddWithValue(@"ID", obj.ID.ToString()); }

                cmd2.Parameters.AddWithValue(@"FilePath", obj.FilePath.ToString());
                cmd2.Parameters.AddWithValue(@"FileName", obj.FileName.ToString());
                cmd2.Parameters.AddWithValue(@"DirName", obj.DirName.ToString());
                cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize.ToString());
                cmd2.Parameters.AddWithValue(@"FileDate", obj.FileDate.ToString());
                if (UpdateTags) { cmd2.Parameters.AddWithValue(@"FileTags", obj.FileTags.ToString()); }
                cmd2.Parameters.AddWithValue(@"FileAction", obj.FileAction.ToString());
                cmd2.Parameters.AddWithValue(@"FileFound", obj.FileFound.ToString());
                cmd2.Parameters.AddWithValue(@"FileFlag", obj.FileFlag.ToString());
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool FileTable_UpdateInsert(FileTable obj, bool ByIDField = true, bool UpdateTags = false)
            {
                SqlConnection Conn = FileTable_Conn();

                bool Updated = false;

                if (ByIDField)
                {
                    if (obj.ID != null && obj.ID != "")
                    {
                        Updated = FileTable_UpdateSQL(obj, ByIDField, UpdateTags);  // try to update record first
                        if (!Updated) { Updated = FileTable_InsertSQL(obj); }  // if unable to update, insert new record
                    }
                    else
                    {
                        Updated = FileTable_InsertSQL(obj);
                    }
                }
                else  // updates using filepath
                {
                    Updated = FileTable_UpdateSQL(obj, ByIDField, UpdateTags);  // try to update record first
                    if (!Updated) { Updated = FileTable_InsertSQL(obj); }  // if unable to update, insert new record
                }

                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool FileTable_UpdateIfPopulated(FileTable obj, string ID = "")
            {
                SqlConnection Conn = FileTable_Conn();
                string SQLcmd = "Update " + FileTable_TableName() + " SET ";
                if (obj.ID != null) { SQLcmd = SQLcmd + " ID = @ID,"; }
                if (obj.FilePath != null) { SQLcmd = SQLcmd + " FilePath = @FilePath,"; }
                if (obj.FileName != null) { SQLcmd = SQLcmd + " FileName = @FileName,"; }
                if (obj.DirName != null) { SQLcmd = SQLcmd + " DirName = @DirName,"; }
                if (obj.FileSize != null) { SQLcmd = SQLcmd + " FileSize = @FileSize,"; }
                if (obj.FileDate != null) { SQLcmd = SQLcmd + " FileDate = @FileDate,"; }
                if (obj.FileTags != null) { SQLcmd = SQLcmd + " FileTags = @FileTags,"; }
                if (obj.FileAction != null) { SQLcmd = SQLcmd + " FileAction = @FileAction,"; }
                if (obj.FileFound != null) { SQLcmd = SQLcmd + " FileFound = @FileFound,"; }
                if (obj.FileFlag != null) { SQLcmd = SQLcmd + " FileFlag = @FileFlag,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ID = @ID";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.ID != null) { cmd2.Parameters.AddWithValue(@"ID", obj.ID); }
                if (obj.FilePath != null) { cmd2.Parameters.AddWithValue(@"FilePath", obj.FilePath); }
                if (obj.FileName != null) { cmd2.Parameters.AddWithValue(@"FileName", obj.FileName); }
                if (obj.DirName != null) { cmd2.Parameters.AddWithValue(@"DirName", obj.DirName); }
                if (obj.FileSize != null) { cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize); }
                if (obj.FileDate != null) { cmd2.Parameters.AddWithValue(@"FileDate", obj.FileDate); }
                if (obj.FileTags != null) { cmd2.Parameters.AddWithValue(@"FileTags", obj.FileTags); }
                if (obj.FileAction != null) { cmd2.Parameters.AddWithValue(@"FileAction", obj.FileAction); }
                if (obj.FileFound != null) { cmd2.Parameters.AddWithValue(@"FileFound", obj.FileFound); }
                if (obj.FileFlag != null) { cmd2.Parameters.AddWithValue(@"FileFlag", obj.FileFlag); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public FileTable FileTable_ReturnSQL(string ID = "")
            {
                SqlConnection Conn = FileTable_Conn();
                string SelectLine = "Select [ID],[FilePath],[FileName],[DirName],[FileSize],[FileDate],[FileTags],[FileAction],[FileFound],[FileFlag] From " + FileTable_TableName() + " WHERE ID = '" + ID + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                FileTable returnObject = new FileTable();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.FilePath = ret["FilePath"].ToString();
                        returnObject.FileName = ret["FileName"].ToString();
                        returnObject.DirName = ret["DirName"].ToString();
                        returnObject.FileSize = ret["FileSize"].ToString();
                        returnObject.FileDate = ret["FileDate"].ToDateTime();
                        returnObject.FileTags = ret["FileTags"].ToString();
                        returnObject.FileAction = ret["FileAction"].ToString();
                        returnObject.FileFound = ret["FileFound"].ToBool();
                        returnObject.FileFlag = ret["FileFlag"].ToBool();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            /// <summary>
            /// Return List of Files from SQL FileTable (All if no Where clause provided)
            /// </summary>
            /// <param name="Where">Do Not Need to Include the Word WHERE</param>
            /// <returns></returns>
            public List<FileTable> FileTable_ReturnSQLList(string Where = "")
            {
                string Command = "Select * From " + FileTable_TableName();
                if (Where != "") { Command = Command + " Where " + Where; }
                SqlConnection Conn = FileTable_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<FileTable> ReturnList = new List<FileTable>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        FileTable returnObject = new FileTable();
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.FilePath = ret["FilePath"].ToString();
                        returnObject.FileName = ret["FileName"].ToString();
                        returnObject.DirName = ret["DirName"].ToString();
                        returnObject.FileSize = ret["FileSize"].ToString();
                        returnObject.FileDate = ret["FileDate"].ToDateTime();
                        returnObject.FileTags = ret["FileTags"].ToString();
                        returnObject.FileAction = ret["FileAction"].ToString();
                        returnObject.FileFound = ret["FileFound"].ToBool();
                        returnObject.FileFlag = ret["FileFlag"].ToBool();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }

            public bool FileTable_SQL_to_SQLite(string SqliteDBPath = @"\Db\FileTable.sqlite")
            {
                string SaveFile = SqliteDBPath;
                if (SqliteDBPath == @"\Db\FileTable.sqlite")
                {
                    ahk.FileCreateDir(ahk.AppDir() + @"\Db");
                    SaveFile = ahk.AppDir() + @"\Db\FileTable.sqlite";
                }

                //sb.StatusBar("Copying SQL Db to " + SaveFile + "...");
                sqlite.SQLTable_To_NewSQLiteTable(FileTable_Conn(), "FileTable", "FileTable", SaveFile, "", false, false, false);
                //sb.StatusBar("FINISHED Copying SQL Db to " + SaveFile);

                if (File.Exists(SaveFile)) { return true; } else { return false; }
            }


            #endregion

            #endregion



            #endregion


        }
    }

    
}

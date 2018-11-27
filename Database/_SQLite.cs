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
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Drawing;
using Telerik.WinControls.Primitives;
using Telerik.WinControls.Enumerations;
using Telerik.WinControls.UI;
using Telerik.WinControls.Zip.Extensions;
using Telerik.WinControls.Zip;

namespace sharpAHK_Dev
{

    public partial class _Database
    {
        public class SQLite  // SQLite Function Lib
        {
            //public _AHK ahk { get; set; }
            //private static _AHK ahk = new _AHK();


            private static bool GlobalDebug = false;  // option to enable/disable MessageBox error messages while code is executing


            #region === SQLite: Connection ===

            SQLiteConnection m_dbConnection;

            /// <summary>Creates Sqlite Connection to .sqlite Database File</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            public SQLiteConnection Connect(string DbFile)
            {
                //// Connect to the DB
                //====== EXAMPLE USE:
                //SQLiteConnection m_dbConnection = Connect(NewDBFile); // connect to SQLite DB file path - returns connection data

                m_dbConnection = new SQLiteConnection("Data Source=" + DbFile + ";Version=3;");
                m_dbConnection.Open();
                return m_dbConnection;
            }

            /// <summary>Disconnects Sqlite Session To Free File For Other Use</summary>
            /// <param name="db">SQLiteConnection To Disconnect</param>
            public void Disconnect(SQLiteConnection db = null)
            {
                SQLiteConnection.ClearAllPools();

                if (db == null) { db = m_dbConnection; }

                if (db != null)
                {
                    db.Close(); //disconnect from database file
                }

                GC.Collect(); //empty garbage files
                GC.WaitForPendingFinalizers(); //wait for those garbage files to finish before proceeding
            }


            #endregion

            #region === SQLite: Write Commands ===

            /// <summary>Execute a SQLite Command on a .sqlite Database File. Returns True If > 1 Rows Changed</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="SQLiteCommand">SQLite Command To Execute</param>
            public bool Execute(string DbFile, string SQLiteCommand)
            {
                _AHK ahk = new _AHK();

                if (!File.Exists(DbFile))  // ensure file path exists
                {
                    if (ahk.IfInString(SQLiteCommand, "Create"))
                    {
                        if (!File.Exists(DbFile)) // create database file if it doesn't exist
                        {
                            SQLiteConnection.CreateFile(DbFile);
                        }
                    }
                    else
                        ahk.MsgBox(DbFile + " Not Found | .sqlite File Must Be Defined | Unable To Execute Command"); return false;
                }

                // Connect to the SQLite DB File
                SQLiteConnection m_dbConnection = Connect(DbFile); // connect to SQLite DB file path - returns connection data

                SQLiteCommand command = new SQLiteCommand(SQLiteCommand, m_dbConnection); // create command

                int RowsChanged = 0;
                try
                {
                    RowsChanged = command.ExecuteNonQuery();             // execute command
                }
                catch (Exception ex)
                {
                    string error = ex.Message;

                    if (error.Contains("already exists"))  // error msg saying it couldn't create table bc it exists - ignore
                    {
                        return true;
                    }

                    GlobalDebug = true;

                    if (GlobalDebug == true) { MessageBox.Show(SQLiteCommand + Environment.NewLine + Environment.NewLine + Environment.NewLine + error); }

                    if (error.Contains("database is locked"))
                    {
                        MessageBox.Show("DB File Currently LOCKED : Unable To Write To " + ahk.FileName(DbFile));
                    }

                    return false;
                }

                Disconnect(m_dbConnection);  // free up db for other use

                if (RowsChanged >= 1) { return true; }   // row updated successfully
                if (RowsChanged == 0) { return false; }  // no rows changed
                return false;
            }

            /// <summary>Attempt to UPDATE Existing Database Entry. If Update Fails, Insert New Entry</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="UpdateLine">SQLite Command To Update Existing Table Entry</param>
            /// <param name="InsertLine">If Update Fails, Line to Insert New SQLite Db Entry</param>
            public bool Update_Insert(string DbFile, string UpdateLine, string InsertLine)
            {
                //setup the function params table by writing the function and file name fields first
                //string UpdateLine = "UPDATE [FunctionParams] set LineNum = '" + LineCount.ToString() + "', Comments = '" + Comments + "', TimeStamp = '" + DateTime.Now + "', Type = '" + REgionName + "', FunctionLine = '" + CodeReturn + "', FunctionText = '" + FunctionText + "', FilePath = '" + FilePath + "', Parent = '" + Parent + "', FID = '" + FID.ToString() + "' WHERE Function = '" + FunctionName + "' AND FileName = '" + FileName + "'";
                //string InsertLine = "INSERT into [FunctionParams] (FileName, Function) VALUES ('" + FileName + "', '" + FunctionName + "')";

                //if (conn.State == ConnectionState.Open) { conn.Close(); } //close existing connection if needed
                bool Updated = Execute(DbFile, UpdateLine);  // Update Table
                if (!Updated) { Updated = Execute(DbFile, InsertLine); }  // insert into a Table

                return Updated;
            }

            /// <summary>Write a Collection of SQLite Insert Commands In Batch - Faster Than Individual Writes to Database</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="Insert_Lines">List of INSERT Commands To Write In Batch</param>
            public bool Batch_Insert(string DbFile, List<string> Insert_Lines)
            {
                _AHK ahk = new _AHK();
                if (!File.Exists(DbFile))  // ensure file path exists
                {
                    ahk.MsgBox(DbFile + " Not Found - Unable to Batch Insert"); return false;
                }

                SQLiteConnection conn = Connect(DbFile); // connect to SQLite DB file path - returns connection data
                                                         //conn.Open();

                List<string> FailedLines = new List<string>();
                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach (string line in Insert_Lines)
                        {
                            cmd.CommandText = line;
                            try
                            {
                                cmd.ExecuteNonQuery();
                            }
                            catch
                            {
                                FailedLines.Add(line);
                            }
                        }

                        transaction.Commit();  // write all commands to table
                    }
                }

                conn.Close();

                if (FailedLines.Count > 0) { ahk.MsgBox("DbFile: " + DbFile + "\n\rFailed Lines Count: " + FailedLines.Count.ToString()); return false; }

                return true;  // true if there were no failed lines during insert process
            }

            /// <summary>Search and Replace Text In SQLite Column Contents</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Read</param>
            /// <param name="ColNameOrPos">Column Name or Column Postion #</param>
            /// <param name="OldText">Text in Column To Replace</param>
            /// <param name="NewText">New Text To Replace In Column</param>
            public bool Replace_Text(string DbFile, string TableName, object ColNameOrPos, string OldText, string NewText)
            {
                _AHK ahk = new _AHK();
                string ColumnName = "";
                if (ColNameOrPos.GetType().ToString() == "System.String")  // user passed in Column Name
                {
                    ColumnName = ColNameOrPos.ToString();
                }

                if (ColNameOrPos.GetType().ToString() == "System.Int32")  // user passed in Column #
                {
                    ColumnName = Column_Name(DbFile, TableName, ahk.ToInt(ColNameOrPos));  // look up column name by the column #
                }

                // remove brackets if passed in
                string Col = ahk.StringReplace(ColumnName, "[");
                Col = ahk.StringReplace(Col, "]");

                // remove brackets if passed in
                string tableName = ahk.StringReplace(TableName, "[");
                tableName = ahk.StringReplace(tableName, "]");

                // assemble sqlite update string 
                string UpdateLine = @"UPDATE [" + TableName + "] SET [" + Col + "] = REPLACE([" + Col + "], '" + OldText + "', '" + NewText + "')";

                bool Updated = Execute(DbFile, UpdateLine);  // Update Table

                return Updated;
            }


            #endregion

            #region === SQLite: Return ===

            // Return info from SQLite

            /// <summary> returns datatable from sqlite search command in sqlite db file (same as SQLiteSearch)</summary>
            /// <param name="DbFile"> </param>
            /// <param name=" SQLiteCommand"> </param>
            public DataTable ReturnDataTable(string DbFile, string SQLiteCommand)
            {
                SQLiteConnection con = new SQLiteConnection("data source=" + DbFile);
                string SearchCommand = SQLiteCommand;
                //if (WhereClause != "") { SearchCommand = SearchCommand + " " + WhereClause; } //add where clause if provided
                SQLiteCommand cmd = new SQLiteCommand(SearchCommand, con);
                con.Open();
                DataTable DT = new DataTable();
                cmd = con.CreateCommand();
                //cmd.CommandText = string.Format("SELECT * FROM {0}", TableName);
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                try
                {
                    adapter.Fill(DT);
                }
                catch
                {
                    con.Close();
                    return null;
                }
                con.Close();
                //DT.TableName = TableName;
                return DT;
            }

            /// <summary> returns datatable from sqlite search command in sqlite db file (same as ReturnDataTable)</summary>
            /// <param name="DbFile"> </param>
            /// <param name=" SQLiteCommand"> </param>
            public DataTable SQLiteSearch(string DbFile, string SQLiteCommand)
            {
                _AHK ahk = new _AHK();
                SQLiteConnection con = new SQLiteConnection("data source=" + DbFile + "; datetimeformat=ISO8601");
                //datetimeformat=CurrentCulture or Ticks or ISO8601

                //string SearchCommand = "SELECT * FROM " + TableName;
                SQLiteCommand cmd = new SQLiteCommand(SQLiteCommand, con);
                con.Open();
                DataTable DT = new DataTable();
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                try
                {
                    adapter.Fill(DT);
                }
                catch (SQLiteException ex)
                {
                    if (GlobalDebug) { ahk.MsgBox(ex.ToString()); }
                    con.Close();
                    return null;
                }

                con.Close();
                return DT;
            }

            /// <summary> returns datatable from sqlite search command in sqlite db file (same as ReturnDataTable)</summary>
            /// <param name="DbFile"> </param>
            /// <param name=" SQLiteCommand"> </param>
            public DataTable GetDataTable(string DbFile, string SQLiteCommand)
            {
                _AHK ahk = new _AHK();
                SQLiteConnection con = new SQLiteConnection("data source=" + DbFile + "; datetimeformat=ISO8601");
                //datetimeformat=CurrentCulture or Ticks or ISO8601

                //string SearchCommand = "SELECT * FROM " + TableName;
                SQLiteCommand cmd = new SQLiteCommand(SQLiteCommand, con);
                con.Open();
                DataTable DT = new DataTable();
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                try
                {
                    adapter.Fill(DT);
                }
                catch (SQLiteException ex)
                {
                    if (GlobalDebug) { ahk.MsgBox(ex.ToString()); }
                    con.Close();
                    return null;
                }

                con.Close();
                return DT;
            }

            /// <summary>more dynamic verion of getdatatable for tags</summary>
            /// <param name="DbFile"> </param>
            /// <param name=" SQLiteQuery"> </param>
            public DataTable ReturnTags(string DbFile, string SQLiteQuery)
            {
                SQLiteConnection con = new SQLiteConnection("data source=" + DbFile);
                SQLiteCommand cmd = new SQLiteCommand(SQLiteQuery, con);
                con.Open();
                DataTable DT = new DataTable();
                cmd = con.CreateCommand();
                cmd.CommandText = SQLiteQuery;
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                adapter.Fill(DT);
                con.Close();
                DT.TableName = "Tags";
                return DT;
            }

            /// <summary>returns data from sqlite db to parse return values</summary>
            /// <param name="SQLLine"> </param>
            /// <param name=" SQLiteConnection db"> </param>
            public SQLiteDataReader ReturnSQLite(string SQLLine, SQLiteConnection db)
            {
                ////// EXAMPLE USE: 
                //////=======================================================================================================
                ////// Connect to SQLite DB - Request Data from DB and Return It - Loop through Each Row then ____
                //////=======================================================================================================

                ////SQLiteConnection m_dbConnection = Connect(NewDBFile); // connect to SQLite DB file path - returns connection data

                ////SQLiteDataReader reader = ReturnSQLite("select * from filelist order by score desc", m_dbConnection);  // request data from DB

                ////while (reader.Read())                                                                      // loop through each row returned from select 
                ////    Console.WriteLine("File: " + reader["filepath"] + "\tScore: " + reader["score"]);            



                //=====================================================
                // read from the data table - return data to reader 
                //=====================================================
                //string SQLLine = "select * from filelist order by score desc";
                SQLiteCommand command = new SQLiteCommand(SQLLine, db);
                SQLiteDataReader reader = null;

                try
                {
                    reader = command.ExecuteReader();
                }
                catch { }

                //while (reader.Read()) // loop through each row returned from select 
                //    Console.WriteLine("File: " + reader["filepath"] + "\tScore: " + reader["score"]);

                //Disconnect(db);  // free up db for other use

                return reader;
            }

            /// <summary> returns last int value from a field in sqlite table</summary>
            /// <param name="TableName"> </param>
            /// <param name=" FieldName"> </param>
            /// <param name=" DbFile"> </param>
            public int ReturnLastINT(string TableName, string FieldName, string DbFile)
            {
                SQLiteConnection m_dbConnection = Connect(DbFile); // connect to SQLite DB file path - returns connection data

                SQLiteDataReader reader = ReturnSQLite("select [" + FieldName + "] from " + TableName + " order by " + FieldName + " desc limit 1", m_dbConnection);  // request data from DB

                string LastFIDString = "";

                while (reader.Read())                                                                      // loop through each row returned from select 
                    LastFIDString = reader[FieldName].ToString();

                int LastFID = Int32.Parse(LastFIDString); //convert string to int

                return LastFID;
            }

            /// <summary> return a string value from a sqlite search (first column / first row returned)</summary>
            /// <param name="DbFile"> </param>
            /// <param name="SQLiteCommand"> </param>
            public string ReturnString(string DbFile, string SQLiteCommand)
            {
                //SQLiteCommand = "select wCount from WordCount where word = 'INI'"; 
                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

                // Connect to SQLite DB - Request Data from DB and Return It - Loop through Each Row then ____

                SQLiteDataReader reader = lite.ReturnSQLite(SQLiteCommand, m_dbConnection);  // request data from DB

                string value = "";

                if (reader == null) { return ""; }

                while (reader.Read())    // loop through each row returned from select 
                {
                    value = reader[0].ToString();
                    break;
                }

                return value;
            }

            /// <summary>Returns # of rows SQLite Command returns (check to see if item is in a database)</summary>
            /// <param name="DbFile"> </param>
            /// <param name="SQLiteCommand"> </param>
            public int FoundCount(string DbFile, string SQLiteCommand)
            {
                //SQLiteCommand = "select wCount from WordCount where word = 'INI'"; 

                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

                // Connect to SQLite DB - Request Data from DB and Return It - Loop through Each Row then ____

                SQLiteDataReader reader = lite.ReturnSQLite(SQLiteCommand, m_dbConnection);  // request data from DB

                string value = "";

                int found = 0;
                while (reader.Read())    // loop through each row returned from select 
                {
                    value = reader[0].ToString();
                    found++;
                }

                return found;
            }

            // returns number of records in a sqlite table (newer)
            public int Count(string DbFile, string TableName, string WHERE = "")
            {
                string where = WHERE.ToUpper().Replace("WHERE ", ""); // remove where from clause if already there
                int Count = 0;
                if (WHERE != "") { Count = ReturnInt(DbFile, "select count(*) from " + TableName + " where " + where); }
                if (WHERE == "") { Count = ReturnInt(DbFile, "select count(*) from " + TableName); }
                return Count;
            }


            /// <summary> return a int value from a sqlite search (first column / first row returned)</summary>
            /// <param name="DbFile"> </param>
            /// <param name=" SQLiteCommand"> </param>
            public int ReturnInt(string DbFile, string SQLiteCommand)
            {
                _AHK ahk = new _AHK();
                //int Value = sqlite.ReturnInt(DbFile, "select wCount from WordCount where word = 'INI'"); 

                //SQLiteCommand = "select wCount from WordCount where word = 'INI'"; 

                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

                // Connect to SQLite DB - Request Data from DB and Return It - Loop through Each Row then ____

                SQLiteDataReader reader = lite.ReturnSQLite(SQLiteCommand, m_dbConnection);  // request data from DB

                string value = "";
                int iValue = 0;

                if (reader != null)
                {
                    while (reader.Read())    // loop through each row returned from select 
                    {
                        value = reader[0].ToString();
                    }

                    iValue = ahk.ToInt(value);
                }

                return iValue;
            }


            #endregion

            #region === SQLite: DataGrid ===

            /// <summary>Load Values from sqlite Database and Populate DataGridView Control. Pass just the TableName to load all contents of Table, or populate Or_SQL_Command for specific search criteria</summary>
            /// <param name="dv">DataGridView Control To Populate</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table To Return All Contents From to Grid</param>
            /// <param name="Or_SQL_Command">SQLite Command That OverRides Previous TableName Search</param>
            /// <param name="AutoSizeColumns">Option To Resize DataGridView Column Width After Populating</param>
            public int Grid_Load(DataGridView dv, string DbFile = "Settings.sqlite", string TableName = "Settings", string Or_SQL_Command = "", bool AutoSizeColumns = true)
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                if (!File.Exists(DbFile)) { ahk.MsgBox(DbFile + " Not Found"); return -1; } // make sure database file exists


                string command = "Select * From [" + TableName + "];";  // selects all contents from sqlite table

                if (Or_SQL_Command != "")  // option for user to pass in full sqlite command instead of returning all of one table
                {
                    command = Or_SQL_Command;
                }

                SQLiteConnection db = Connect(DbFile); // connect to SQLite DB file path - returns connection data

                try
                {
                    DataSet ds = new DataSet();
                    var da = new SQLiteDataAdapter(command, db);  // search SQLite DB
                    da.Fill(ds);

                    // assign the DataGridView Name to Populate
                    dv.DataSource = ds.Tables[0].DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("SQL Exception Catch Here: " + ex.ToString());
                }

                dv.AutoGenerateColumns = true;


                if (AutoSizeColumns)
                {
                    // Resize the master DataGridView columns to fit the newly loaded data.
                    dv.AutoResizeColumns();

                    // Configure the details DataGridView so that its columns automatically 
                    // adjust their widths when the data changes.
                    dv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                }

                Disconnect(db);  // free up db for other use

                // return grid row count
                return dv.RowCount;
            }

            /// <summary>Save Values in DataGridView Control to .sqlite Database File. Creates new Sqlite Table with using Column Names if Table does not already exist</summary>
            /// <param name="dv">DataGridView Control To Save</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table To Save Contents of Grid to</param>
            /// <param name="OverWrite">Option To Clear Previous Values in Table Before Saving</param>
            /// <param name="SkipColumnNum">Column # in Grid To Skip Writing To Database (usually the Index ID Field)</param>
            public void Grid_Save(DataGridView dv, string DbFile = "Settings.sqlite", string TableName = "GridValues", bool OverWrite = true, int SkipColumnNum = -1)
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                bool SkipColumn = false;
                if (SkipColumnNum != -1) { SkipColumn = true; }

                // check to see if there are any columns to populate list first
                int colCount = 0;
                try { colCount = dv.Rows[0].Cells.Count; }
                catch
                {
                    ahk.MsgBox("No Columns Populated In DataGridView To Save");
                    return;
                }

                // Create List of Grid Column Names
                List<string> Grid_Columns = new List<string>();
                string ColumnName = "";
                int columnCount = dv.Rows[0].Cells.Count;
                for (int i = 0; i < columnCount; i++)
                {
                    ColumnName = dv.Columns[i].HeaderText;
                    Grid_Columns.Add(ColumnName);
                }

                // check to see if this table already exists in sqlite db
                bool TableFound = Table_Exists(DbFile, TableName);

                if (OverWrite) // option to overwrite existing table if it exists already
                {
                    if (TableFound)
                    {
                        bool Cleared = Table_Clear(DbFile, TableName, true);
                    }
                }

                int ColNum = 0;

                if (!TableFound)  // Create new SQLITE Table using DataGridView Column Names if Table Does Not Exist
                {
                    string NewTableLine = "ID INTEGER PRIMARY KEY";

                    foreach (string col in Grid_Columns)
                    {
                        if (SkipColumn) { if (ColNum == SkipColumnNum) { ColNum++; continue; } } // option to skip writing specific column (usually id field) into database 
                        NewTableLine = NewTableLine + ", " + col + " VARCHAR";
                    }

                    bool Created = Table_New(DbFile, TableName, NewTableLine);
                }


                // Create Insert Command (Grid Column Names = Sqlite Table Column Names)
                string Command = "Insert into [" + TableName + "] (";
                string VarList = "";
                ColNum = 0;
                foreach (string col in Grid_Columns)
                {
                    if (SkipColumn) { if (ColNum == SkipColumnNum) { ColNum++; continue; } } // option to skip writing specific column (usually id field) into database 
                    if (VarList != "") { VarList = VarList + "," + col; }
                    if (VarList == "") { VarList = col; }
                    ColNum++;
                }
                Command = Command + VarList + ") Values (";


                // Populate Insert Command Values
                columnCount = dv.Rows[0].Cells.Count;
                List<string> InsertList = new List<string>();
                string RowValues = "";
                for (int i = 0; i < dv.Rows.Count - 1; i++) // loop through each row
                {
                    RowValues = "";

                    for (int c = 0; c < columnCount; c++)  // loop through each column
                    {
                        if (SkipColumn) { if (c == SkipColumnNum) { ColNum++; continue; } } // option to skip writing specific column (usually id field) into database 

                        //bool ColVisible = dataGridView1.Rows[i].Cells[c].Visible;
                        //ahk.MsgBox("Col " + c + " = " + ColVisible.ToString()); 

                        string value = dv.Rows[i].Cells[c].Value.ToString();
                        if (RowValues != "") { RowValues = RowValues + ", '" + value + "'"; }
                        if (RowValues == "") { RowValues = "'" + value + "'"; continue; }
                    }

                    string SQLCommand = Command + RowValues + ");";
                    InsertList.Add(SQLCommand);
                }

                Batch_Insert(DbFile, InsertList);

                ahk.MsgBox("Grid Saved");
            }

            #endregion

            #region === SQLite: From SQL ===

            // SQL to SQLite
            // -needs to be update statement or insert

            /// <summary> Create SQLite Table From Existing SQL Table Structure</summary>
            /// <param name="Connection"> </param>
            /// <param name=" DataBaseName"> </param>
            /// <param name=" TableName"> </param>
            /// <param name=" DbFile"> </param>
            /// <param name="NewTableName"> </param>
            /// <param name="CreateTableOnly"> </param>
            /// <param name="SingleInsert"> </param>
            /// <param name="CreateIDField"> </param>
            public void SQLTable_To_NewSQLiteTable(SqlConnection Connection, string DataBaseName, string TableName, string DbFile, string NewTableName = "", bool CreateTableOnly = false, bool SingleInsert = false, bool CreateIDField = true)
            {
                _AHK ahk = new _AHK();
                TableName = TableName.Replace("[", "");
                TableName = TableName.Replace("]", "");

                DataBaseName = DataBaseName.Replace("[", "");
                DataBaseName = DataBaseName.Replace("]", "");

                //=================================================================
                // Create SQLite Table From Existing SQL Table Structure
                //=================================================================

                // SingleInsert = Method of Writing to SQLite Table 1 record at a time --- MUCH Slower but Used to Debug


                //SqlConnection T2SS = new SqlConnection("Server=192.168.30.178\\MSSQLSRVR2008;DataBase=T2SSPortal;Uid=QAADMIN;Pwd=P@ssword178");
                //string DataBaseName = "IMCRMData";
                //string TableName = "CRMPicklist";

                // connect to SQL database and return the column structure for a table
                // SELECT * FROM IMCRMData.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'CRMPicklist'     /* Returns info about a SQL Table Structure */
                _Database.SQL sql = new _Database.SQL();
                string cm = "SELECT * FROM " + DataBaseName + ".INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + TableName + "'";
                DataTable dt = sql.GetDataTable(Connection, cm);

                string NewTableLine = "";
                //string NewTableLine = "ID INTEGER PRIMARY KEY";

                if (!CreateIDField) // option to not create an ID Primary Key
                {
                    NewTableLine = "";
                }

                List<string> ColumnNames = new List<string>();
                string ColumnNameString = "";

                //// skip writing id field if there is already an id column
                //foreach (DataRow datarow in dt.Rows)
                //{
                //    string ColumnName = datarow["COLUMN_NAME"].ToString();
                //    string Position = datarow["ORDINAL_POSITION"].ToString();
                //    string DataType = datarow["DATA_TYPE"].ToString();

                //    if (DataType == "bit") { DataType = "BOOL"; }

                //    if (ColumnName == "ID") { NewTableLine = ""; }
                //}

                if (dt != null)
                {
                    foreach (DataRow datarow in dt.Rows)
                    {
                        string ColumnName = datarow["COLUMN_NAME"].ToString();
                        string Position = datarow["ORDINAL_POSITION"].ToString();
                        string DataType = datarow["DATA_TYPE"].ToString();

                        if (DataType == "bit") { DataType = "BOOL"; }

                        ColumnNames.Add(ColumnName); //save column name to list

                        //NewTableLine = "ID INTEGER PRIMARY KEY, Public BOOL, ClientNum VARCHAR, NoteName VARCHAR, Note VARCHAR"; }

                        if (NewTableLine != "") { NewTableLine = NewTableLine + ", '" + ColumnName + "' " + DataType; }
                        if (NewTableLine == "")
                        {
                            NewTableLine = "'" + ColumnName + "' " + DataType;
                        }


                        ColumnNameString = ColumnNameString + "'" + ColumnName + "',";
                    }
                }

                ColumnNameString = ahk.TrimLast(ColumnNameString);  //trim last comma
                ColumnNameString = "(" + ColumnNameString + ")";

                //MessageBox.Show(NewTableLine);

                if (NewTableName != "") // if user provided different name for new SQLite table name, use that 
                {
                    TableName = NewTableName;
                }

                bool Created = Table_New(DbFile, TableName, NewTableLine, false); //create SQLite table if it doesn't exist yet
                                                                                  //MessageBox.Show(TableName + " Created.");// = " + Created);
                Table_Clear(DbFile, TableName); //remove existing data from this table before inserting


                // option to just create the table structure
                if (CreateTableOnly == true) { return; }


                //==================================================================================
                // Populate SQLite Table From Existing SQL Table - BATCH Import Method (Fastest)
                //==================================================================================

                string SQLSearch = "SELECT * FROM [" + DataBaseName + "].[dbo].[" + TableName + "]";
                DataTable dt2 = new DataTable();

                dt2 = sql.GetDataTable(Connection, SQLSearch);


                // account for using GoDaddy SQL calls if NULL returned

                if (dt2 == null)
                {
                    //ahk.MsgBox("null");
                    SQLSearch = "SELECT * FROM [" + DataBaseName + "].[lucid].[" + TableName + "]";
                    dt2 = sql.GetDataTable(Connection, SQLSearch);
                }
                if (dt2 == null)
                {
                    //ahk.MsgBox("null");
                    SQLSearch = "SELECT * FROM [" + DataBaseName + "].[lucidmethod].[" + TableName + "]";
                    dt2 = sql.GetDataTable(Connection, SQLSearch);
                }


                // write the sql table return values to SQLite table with bulk insert statement (MUCH FASTER THAN SINGLE INSERT)

                if (!SingleInsert)
                {
                    using (var conn = new SQLiteConnection(@"Data Source=" + DbFile))
                    {
                        conn.Open();

                        //var stopwatch = new Stopwatch();
                        //stopwatch.Start();

                        using (var cmd = new SQLiteCommand(conn))
                        {
                            using (var transaction = conn.BeginTransaction())
                            {
                                int RowCounter = 0;

                                if (dt2 != null)
                                {
                                    foreach (DataRow sqldatarow in dt2.Rows)
                                    {
                                        int ColumnCounter = 0;
                                        string ValueString = "";
                                        foreach (string Column in ColumnNames)
                                        {
                                            string ColumnValue = dt2.Rows[RowCounter][Column].ToString();
                                            ColumnCounter++;

                                            ColumnValue = ahk.FixSpecialChars(ColumnValue); //remove invalid characters before writing

                                            ValueString = ValueString + "'" + ColumnValue + "',";
                                        }

                                        ValueString = ahk.TrimLast(ValueString);  //trim last comma

                                        string InsertLine = "INSERT into " + TableName + " " + ColumnNameString + " values (" + ValueString + ")";

                                        //if (SingleInsert) { Execute_SQLite(SQLiteDb, InsertLine, true); } //write entry to SQLite table (single entry method)

                                        cmd.CommandText = InsertLine + ";";
                                        cmd.ExecuteNonQuery();


                                        RowCounter++;
                                    }
                                }

                                if (!SingleInsert) { transaction.Commit(); }
                            }
                        }

                        //Console.WriteLine("{0} seconds with one transaction.", stopwatch.Elapsed.TotalSeconds);

                        conn.Close();
                    }
                }




                //=====================================================================================
                // Populate SQLite Table From Existing SQL Table - Single Insert Mode - DEBUG
                //=====================================================================================

                if (SingleInsert)
                {
                    int RowCounter = 0;
                    SQLiteConnection conn = Connect(DbFile); // connect to SQLite DB file path - returns connection data
                    if (dt2 != null)
                    {
                        foreach (DataRow sqldatarow in dt2.Rows)
                        {
                            int ColumnCounter = 0;
                            string ValueString = "";
                            foreach (string Column in ColumnNames)
                            {
                                string ColumnValue = dt2.Rows[RowCounter][Column].ToString();
                                ColumnCounter++;

                                ColumnValue = ahk.FixSpecialChars(ColumnValue); //remove invalid characters before writing

                                ValueString = ValueString + "'" + ColumnValue + "',";
                            }

                            ValueString = ahk.TrimLast(ValueString);  //trim last comma

                            string InsertLine = "INSERT into " + TableName + " " + ColumnNameString + " values (" + ValueString + ")";

                            Execute(DbFile, InsertLine); //write entry to SQLite table

                            RowCounter++;
                        }
                    }


                    conn.Close();
                }


                //MessageBox.Show("Finished Updating SQLite Db"); 

            }

            /// <summary> copy tablestructure and database contents from sql table to local sqlite db file</summary>
            /// <param name="Connection"> </param>
            /// <param name=" DataBaseName"> </param>
            /// <param name=" DbFile"> </param>
            /// <param name="CreateTableOnly"> </param>
            /// <param name="SingleInsert"> </param>
            /// <param name="CreateIDField"> </param>
            public void SQLDb_To_SQLiteDb(SqlConnection Connection, string DataBaseName, string DbFile, bool CreateTableOnly = false, bool SingleInsert = false, bool CreateIDField = true)
            {
                //======================================
                // Return List of SQL Table Names
                //======================================
                // EX: 
                _Database.SQL sql = new _Database.SQL();
                List<string> SQLTableNames = sql.Return_TableList(Connection);
                foreach (string TableName in SQLTableNames)
                {
                    //ahk.MsgBox(TableName);
                    SQLTable_To_NewSQLiteTable(Connection, DataBaseName, TableName, DbFile, "", CreateTableOnly, SingleInsert, CreateIDField);
                }

            }


            #endregion

            #region === SQLite: Tables ===

            //string NewTableLine = "ID INTEGER PRIMARY KEY, SettingName VARCHAR, SettingValue VARCHAR, Flag BOOL";
            //sqlite.Table_New = (DbFile, "Settings", NewTableLine);

            /// <summary>Create New SQLite Table</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of New Table in .sqlite File Create</param>
            /// <param name="NewTableLine">Define Table Columns</param>
            /// <param name="OverWrite">Option to Drop And Create New Table If TableName Already Exists</param>
            public bool Table_New(string DbFile, string TableName, string NewTableLine, bool OverWrite = false)
            {
                // Create New SQLite DB (*Used First-Run*)
                if (!File.Exists(DbFile)) // create database file if it doen't exist
                {
                    SQLiteConnection.CreateFile(DbFile);
                }

                // Check to see if TableName already exists in SQLite Db
                bool TableExist = Table_Exists(DbFile, TableName);

                // option to overwrite existing table if it exists
                if (OverWrite)
                {
                    if (TableExist) { Table_Drop(DbFile, TableName); }
                }

                if (!TableExist)  // Table DOES NOT exist in SQLite DB - Create Now
                {
                    Execute(DbFile, "CREATE TABLE [" + TableName + "] (" + NewTableLine + ")");  // Create new Table
                                                                                                 // confirm table exists and return true/false
                    return Table_Exists(DbFile, TableName);
                }

                return true;  // Table Already Exists - Return True
            }

            public bool Table_NewFromList(string DbFile, string TableName, List<string> ColumnNameList, bool IDField = true, bool OverWrite = false)
            {
                _AHK ahk = new _AHK();
                // Create New SQLite DB (*Used First-Run*)
                if (!File.Exists(DbFile)) // create database file if it doen't exist
                {
                    SQLiteConnection.CreateFile(DbFile);
                }

                // Check to see if TableName already exists in SQLite Db
                bool TableExist = Table_Exists(DbFile, TableName);

                // option to overwrite existing table if it exists
                if (OverWrite)
                {
                    if (TableExist) { Table_Drop(DbFile, TableName); }
                }

                if (!TableExist)  // Table DOES NOT exist in SQLite DB - Create Now
                {
                    TableName = TableName.Replace("[", "");
                    TableName = TableName.Replace("]", "");

                    string NewTableLine = "CREATE TABLE [" + TableName + "] (";
                    if (IDField) { NewTableLine = NewTableLine + "ID INTEGER PRIMARY KEY, "; }

                    foreach (string line in ColumnNameList)
                    {
                        // add each new line, if space found, assumes field type was defined in list, otherwias add varchar
                        if (line.Trim().Contains(" ")) { NewTableLine = NewTableLine + line + ", "; }
                        else { NewTableLine = NewTableLine + line + " VARCHAR, "; }
                    }

                    NewTableLine = NewTableLine.Trim();

                    NewTableLine = ahk.TrimLast(NewTableLine, 1);  // trim last comma 

                    NewTableLine = NewTableLine + ")";

                    Execute(DbFile, NewTableLine);  // Create new Table

                    // confirm table exists and return true/false
                    return Table_Exists(DbFile, TableName);
                }

                return true;  // Table Already Exists - Return True
            }


            /// <summary>Clears Contents of SQLite Table</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Clear</param>
            /// <param name="RestoreSpace">Option To Vacuum Database File To Restore Unused Space In The .sqlite File</param>
            public bool Table_Clear(string DbFile, string TableName, bool RestoreSpace = true)
            {
                _AHK ahk = new _AHK();
                if (GlobalDebug) { if (ahk.IfNotExist(DbFile)) { ahk.MsgBox(" DbFile Not Found - Possibly have Function Backwards in Code."); return false; } }

                //=== Drop Table from DB File IF It Exists =========================
                bool Return = Execute(DbFile, "Delete From [" + TableName + "]");

                if (RestoreSpace)
                {
                    Table_Shrink(DbFile);  //restores space after deleting from tables
                }

                return Return;
            }

            /// <summary>Drops Table from SQLite Table (Aka Delete)</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Remove</param>
            /// <param name="RestoreSpace">Option To Vacuum Database File To Restore Unused Space In The .sqlite File</param>
            public bool Table_Drop(string DbFile, string TableName, bool RestoreSpace = true)
            {
                //=== Drop Table from DB File IF It Exists =========================
                bool Return = Execute(DbFile, "Drop Table IF EXISTS [" + TableName + "]");

                if (RestoreSpace)
                {
                    Table_Shrink(DbFile);  //restores space after deleting from tables
                }

                return Return;  // otherwise TRUE, dropped table
            }

            /// <summary>Restores Previously Used Space in Database File (Vacuum)</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            public bool Table_Shrink(string DbFile)
            {
                bool Return = Execute(DbFile, "VACUUM;");
                return Return;
            }

            /// <summary>Returns List of TableNames in .sqlite Database File</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            public List<string> Table_List(string DbFile)
            {
                _AHK ahk = new _AHK();
                if (!File.Exists(DbFile)) { ahk.MsgBox(ahk.FileName(DbFile) + " Not Found"); return null; } // database file doesn't exist - table not found

                List<string> TableListOut = new List<string>();

                // Extract TABLE Names from SQLite DB

                // Initialize the connection
                SQLiteConnection conn = new SQLiteConnection("data source=" + DbFile);

                // These is how you list the schema of an SQLite database
                SQLiteCommand comm = new SQLiteCommand("SELECT * FROM SQLite_master WHERE type = 'table' ORDER BY 1", conn);
                conn.Open();

                // Populate the reader
                SQLiteDataReader reader = comm.ExecuteReader();


                // Parse Elements from SQLite DB File - Step through each row 
                while (reader.Read())
                {
                    for (int a = 0; a < reader.FieldCount; a++)
                    {
                        string columnName = reader.GetName(a);  // This will give you the name of the current row's column
                        string columnValue = reader[a].ToString();  // This will give you the value of the current row's column
                                                                    //MessageBox.Show(columnName + " = " + columnValue); 

                        if (columnName == "tbl_name")
                        {
                            TableListOut.Add(columnValue);
                        }
                    }
                }

                Disconnect(conn);
                return TableListOut;
            }

            /// <summary> Returns list of TABLE NAMES</summary>
            /// <param name="DbFile"> </param>
            public Dictionary<string, string> Table_Dict(string DbFile = "")
            {
                _AHK ahk = new _AHK();
                //// ex: return table names and bind to combobox ddl
                //Dictionary<string, string> TableDict = sqlite.ExtractTableNameList(SQLiteDbDir + "\\" + ddlDbName + ".SQLite");
                //ddlTableName.DataSource = new BindingSource(TableDict, null);
                //ddlTableName.DisplayMember = "Key";
                //ddlTableName.ValueMember = "Value";



                Dictionary<string, string> TableNamesDict = new Dictionary<string, string>();

                //=============================================
                // Extract TABLE Names from SQLite DB
                //=============================================

                if (DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                // Initialize the connection
                SQLiteConnection conn = new SQLiteConnection("data source=" + DbFile);

                // These is how you list the schema of an SQLite database
                SQLiteCommand comm = new SQLiteCommand("SELECT * FROM SQLite_master WHERE type = 'table' ORDER BY 1", conn);
                conn.Open();
                // Populate the reader
                SQLiteDataReader reader = comm.ExecuteReader();


                //=================================================
                // Parse Elements from SQLite DB File
                //=================================================

                string TableName = "";
                string DataName = "";
                string DataType = "";
                string RootPageValue = "";
                string CreateTableCode = "";
                string TableList = "";

                // Step through each row
                while (reader.Read())
                {
                    for (int a = 0; a < reader.FieldCount; a++)
                    {

                        string columnName = reader.GetName(a);  // This will give you the name of the current row's column
                        string columnValue = reader[a].ToString();  // This will give you the value of the current row's column
                                                                    //MessageBox.Show(columnName + " = " + columnValue); 


                        if (columnName == "type")
                        {
                            DataType = columnValue;  // = table
                        }

                        if (columnName == "name")
                        {
                            DataName = columnValue;  // aka the tbl_name in this case (?)
                        }

                        if (columnName == "tbl_name")
                        {
                            TableName = columnValue;
                            // TableList = TableList + Environment.NewLine + TableName;

                            if (!TableNamesDict.ContainsKey(TableName))
                            {
                                TableNamesDict.Add(TableName, TableName);
                            }
                        }

                        if (columnName == "rootpage")
                        {
                            RootPageValue = columnValue;
                        }

                        if (columnName == "sql")
                        {
                            CreateTableCode = columnValue;
                        }
                    }

                    //char[] c = { '|' };  // trim off leading | if it exists 
                    //TableList = TableList.TrimStart(c);
                }


                //MessageBox.Show("[" + TableList + "]"); 
                Disconnect(conn);

                return TableNamesDict;
            }

            /// <summary>Checks To See If Table Name Exists in SQLite Db - Returns True/False</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="SearchTableName">Name of the Table to Check For</param>
            public bool Table_Exists(string DbFile, string SearchTableName)
            {
                if (!File.Exists(DbFile)) { return false; } // database file doesn't exist - table not found

                List<string> TableNames = Table_List(DbFile);

                foreach (string table in TableNames)
                {
                    if (table.ToUpper().Trim() == SearchTableName.ToUpper().Trim()) { return true; }
                }

                return false;
            }

            // rename existing table in sqlite db
            public bool Table_Rename(string DbFile, string OldTableName, string NewTableName)
            {
                bool renamed = Execute(DbFile, "ALTER TABLE " + OldTableName + " RENAME TO " + NewTableName);
                return renamed;
            }

            #endregion

            #region === SQLite: Columns ===

            /// <summary>Returns # of Columns In TableName</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Search</param>
            public int Column_Count(string DbFile, string TableName)
            {
                _AHK ahk = new _AHK();
                if (!File.Exists(DbFile)) // confirm database file exists
                {
                    ahk.MsgBox(DbFile + " Not Found - Unable to Load Column Names");
                    return -1;
                }

                // Initialize the connection
                SQLiteConnection conn = new SQLiteConnection("data source=" + DbFile);

                // These is how you list the schema of an SQLite database
                SQLiteCommand comm = new SQLiteCommand("SELECT * FROM SQLite_master WHERE type = 'table' ORDER BY 1", conn);
                conn.Open();
                // Populate the reader
                SQLiteDataReader reader = comm.ExecuteReader();

                var cmd = new SQLiteCommand("select * from '" + TableName + "'", conn);
                var dr = cmd.ExecuteReader();

                int ColumnCount = dr.FieldCount;

                //// alternative method to count columns
                //for (var i = 0; i < dr.FieldCount; i++) { ColumnCount++; }

                Disconnect(conn);  // free up db for other use

                return ColumnCount;
            }

            /// <summary>Returns list of Column Names in .sqlite Table</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Search</param>
            public List<string> Column_List(string DbFile, string TableName)
            {
                _AHK ahk = new _AHK();
                if (!File.Exists(DbFile)) // confirm database file exists
                {
                    ahk.MsgBox(DbFile + " Not Found - Unable to Load Column Names");
                    return null;
                }

                List<string> ColumnNames = new List<string>();

                // Initialize the connection
                SQLiteConnection conn = new SQLiteConnection("data source=" + DbFile);

                // These is how you list the schema of an SQLite database
                SQLiteCommand comm = new SQLiteCommand("SELECT * FROM SQLite_master WHERE type = 'table' ORDER BY 1", conn);
                conn.Open();
                // Populate the reader
                SQLiteDataReader reader = comm.ExecuteReader();

                // Loop through TABLE name to extract column names 

                var cmd = new SQLiteCommand("select * from '" + TableName + "'", conn);
                var dr = cmd.ExecuteReader();

                for (var i = 0; i < dr.FieldCount; i++)
                {
                    string Column = dr.GetName(i);
                    ColumnNames.Add(Column);
                }

                Disconnect(conn);  // free up db for other use

                return ColumnNames;
            }

            // check to see if column name exists in table
            public bool IsColumn(string DbFile, string TableName, string ColNameToCheck)
            {
                _Lists lst = new _Lists();
                List<string> cols = Column_List(DbFile, TableName);
                if (lst.InList(cols, ColNameToCheck)) { return true; }
                return false;
            }

            /// <summary>Returns Dictionary Var With Column Name + Column Data Type</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Search</param>
            public Dictionary<string, string> Column_Dict(string DbFile, string TableName)
            {
                Dictionary<string, string> ColumnNamesDict = new Dictionary<string, string>();

                // Initialize the connection
                SQLiteConnection conn = new SQLiteConnection("data source=" + DbFile);

                // These is how you list the schema of an SQLite database
                SQLiteCommand comm = new SQLiteCommand("SELECT * FROM SQLite_master WHERE type = 'table' ORDER BY 1", conn);
                conn.Open();
                // Populate the reader
                SQLiteDataReader reader = comm.ExecuteReader();

                // Loop through TABLE name to extract COLUMN names / column COUNT

                var cmd = new SQLiteCommand("select * from '" + TableName + "'", conn);
                var dr = cmd.ExecuteReader();

                for (var i = 0; i < dr.FieldCount; i++)
                {
                    string Column = dr.GetName(i);

                    if (!ColumnNamesDict.ContainsKey(Column))
                    {
                        ColumnNamesDict.Add(Column, dr.GetDataTypeName(i).ToString());
                    }

                }

                Disconnect(conn);  // free up db for other use

                return ColumnNamesDict;
            }

            /// <summary>Return Column Type [BOOL/VARCHAR/INTEGER] Search by Column Name or Column Position #</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Read</param>
            /// <param name="ColNameOrPos">Column Name or Column Postion #</param>
            public string Column_Type(string DbFile, string TableName, object ColNameOrPos)
            {
                _AHK ahk = new _AHK();
                if (!File.Exists(DbFile))
                {
                    ahk.MsgBox(DbFile + " Not Found - Unable to Load Column Type");
                    return "";
                }

                // Initialize the connection
                SQLiteConnection conn = new SQLiteConnection("data source=" + DbFile);

                // These is how you list the schema of an SQLite database
                SQLiteCommand comm = new SQLiteCommand("SELECT * FROM SQLite_master WHERE type = 'table' ORDER BY 1", conn);
                conn.Open();
                // Populate the reader
                SQLiteDataReader reader = comm.ExecuteReader();

                //========================================================================
                // Loop through TABLE Column Types
                //========================================================================

                // Sqlite Column Field Types (dr.GetFieldType(i).ToString())
                //          System.String
                //          System.Boolean
                //          System.Int64

                // Sqlite Column Data Types (dr.GetDataTypeName(i).ToString())
                //          BOOL
                //          VARCHAR
                //          INTEGER

                var cmd = new SQLiteCommand("select * from '" + TableName + "'", conn);
                var dr = cmd.ExecuteReader();

                for (var i = 0; i < dr.FieldCount; i++)
                {
                    string Column = dr.GetName(i);
                    string colFieldType = dr.GetFieldType(i).ToString();
                    string colType = dr.GetDataTypeName(i).ToString();

                    //ahk.MsgBox(Column + " - " + colType + " - " + dr.GetDataTypeName(i).ToString() + " - " + dr.GetType().ToString());

                    if (ColNameOrPos.GetType().ToString() == "System.String")  // user passed in Column Name
                    {
                        if (ColNameOrPos.ToString().ToUpper() == Column.ToUpper())   // match column by current column name 
                        {
                            Disconnect(conn);  // free up db for other use
                            return colType;
                        }
                    }

                    if (ColNameOrPos.GetType().ToString() == "System.Int32")  // user passed in Column #
                    {
                        if (ColNameOrPos.ToString() == i.ToString())   // match column by current col # int
                        {
                            Disconnect(conn);  // free up db for other use
                            return colType;
                        }
                    }
                }

                Disconnect(conn);  // free up db for other use
                return ""; // no match found to col name / col number
            }

            /// <summary>Returns DataTable with SQLite Table Schema</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Read</param>
            public DataTable Table_SchemaTable(string DbFile, string TableName)
            {
                _AHK ahk = new _AHK();
                if (!File.Exists(DbFile))
                {
                    ahk.MsgBox(DbFile + " Not Found - Unable to Load DataBase Schema Table");
                    //return "";
                    return null;
                }

                // Initialize the connection
                SQLiteConnection conn = new SQLiteConnection("data source=" + DbFile);

                // These is how you list the schema of an SQLite database
                SQLiteCommand comm = new SQLiteCommand("SELECT * FROM SQLite_master WHERE type = 'table' ORDER BY 1", conn);
                conn.Open();
                // Populate the reader
                SQLiteDataReader reader = comm.ExecuteReader();

                var cmd = new SQLiteCommand("select * from '" + TableName + "'", conn);
                var dr = cmd.ExecuteReader();

                DataTable sqliteTable = dr.GetSchemaTable();

                Disconnect(conn);  // free up db for other use

                return sqliteTable;
            }

            /// <summary>Return SQLite Column Info From Table Schema</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Read</param>
            /// <param name="ColNameOrPos">Column Name or Column Postion #</param>
            /// <param name="ReturnType">OPTIONS: IsKey / IsUnique / AllowDbNull / ColumnName / ColumnNum / ColumnSize / DataType / DataTypeName</param>
            public string Column_Info(string DbFile, string TableName, object ColNameOrPos, string ReturnType = "IsKey")
            {
                DataTable table = Table_SchemaTable(DbFile, TableName);

                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        string AllowDbNull = row["AllowDBNull"].ToString();
                        string IsUnique = row["IsUnique"].ToString();
                        string IsKey = row["IsKey"].ToString();
                        string ColumnName = row["ColumnName"].ToString();
                        string ColumnNum = row["ColumnOrdinal"].ToString();
                        string ColumnSize = row["ColumnSize"].ToString();
                        string DataType = row["DataType"].ToString();
                        string DataTypeName = row["DataTypeName"].ToString();

                        string ReturnVal = "";
                        if (ReturnType.ToUpper() == "ALLOWDBNULL") { ReturnVal = AllowDbNull; }
                        if (ReturnType.ToUpper() == "ISUNIQUE") { ReturnVal = IsUnique; }
                        if (ReturnType.ToUpper() == "ISKEY") { ReturnVal = IsKey; }
                        if (ReturnType.ToUpper() == "COLUMNNAME") { ReturnVal = ColumnName; }
                        if (ReturnType.ToUpper() == "COLUMNNUM") { ReturnVal = ColumnNum; }
                        if (ReturnType.ToUpper() == "COLUMNSIZE") { ReturnVal = ColumnSize; }
                        if (ReturnType.ToUpper() == "DATATYPE") { ReturnVal = DataType; }
                        if (ReturnType.ToUpper() == "DATATYPENAME") { ReturnVal = DataTypeName; }


                        if (ColNameOrPos.GetType().ToString() == "System.String")  // user passed in Column Name
                        {
                            if (ColNameOrPos.ToString().ToUpper() == ColumnName.ToUpper())   // match column by current column name 
                            {
                                return ReturnVal;
                            }
                        }

                        if (ColNameOrPos.GetType().ToString() == "System.Int32")  // user passed in Column #
                        {
                            if (ColNameOrPos.ToString() == ColumnNum.ToString())   // match column by current col # int
                            {

                                return ReturnVal;
                            }
                        }

                    }
                }


                return "";
            }

            /// <summary>Is SQLite Column a Key Column in Table (Returns True/False)</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Read</param>
            /// <param name="ColNameOrPos">Column Name or Column Postion #</param>
            public bool Column_IsKey(string DbFile, string TableName, object ColNameOrPos)
            {
                DataTable table = Table_SchemaTable(DbFile, TableName);

                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        string IsKey = row["IsKey"].ToString();
                        string ColumnName = row["ColumnName"].ToString();
                        string ColumnNum = row["ColumnOrdinal"].ToString();

                        if (ColNameOrPos.GetType().ToString() == "System.String")  // user passed in Column Name
                        {
                            if (ColNameOrPos.ToString().ToUpper() == ColumnName.ToUpper())   // match column by current column name 
                            {
                                if (IsKey.ToUpper() == "TRUE") { return true; }
                                if (IsKey.ToUpper() == "FALSE") { return false; }
                            }
                        }

                        if (ColNameOrPos.GetType().ToString() == "System.Int32")  // user passed in Column #
                        {
                            if (ColNameOrPos.ToString() == ColumnNum.ToString())   // match column by current col # int
                            {
                                if (IsKey.ToUpper() == "TRUE") { return true; }
                                if (IsKey.ToUpper() == "FALSE") { return false; }
                            }
                        }
                    }
                }

                return false;
            }

            /// <summary>Does SQLite Column Require Unique Value in Column (Returns True/False)</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Read</param>
            /// <param name="ColNameOrPos">Column Name or Column Postion #</param>
            public bool Column_IsUnique(string DbFile, string TableName, object ColNameOrPos)
            {
                DataTable table = Table_SchemaTable(DbFile, TableName);

                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        string IsUnique = row["IsUnique"].ToString();
                        string ColumnName = row["ColumnName"].ToString();
                        string ColumnNum = row["ColumnOrdinal"].ToString();

                        if (ColNameOrPos.GetType().ToString() == "System.String")  // user passed in Column Name
                        {
                            if (ColNameOrPos.ToString().ToUpper() == ColumnName.ToUpper())   // match column by current column name 
                            {
                                if (IsUnique.ToUpper() == "TRUE") { return true; }
                                if (IsUnique.ToUpper() == "FALSE") { return false; }
                            }
                        }

                        if (ColNameOrPos.GetType().ToString() == "System.Int32")  // user passed in Column #
                        {
                            if (ColNameOrPos.ToString() == ColumnNum.ToString())   // match column by current col # int
                            {
                                if (IsUnique.ToUpper() == "TRUE") { return true; }
                                if (IsUnique.ToUpper() == "FALSE") { return false; }
                            }
                        }
                    }
                }

                return false;
            }

            /// <summary>Does SQLite Column Allow Null Values Inserted Into Column (Returns True/False)</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Read</param>
            /// <param name="ColNameOrPos">Column Name or Column Postion #</param>
            public bool Column_AllowNull(string DbFile, string TableName, object ColNameOrPos)
            {
                DataTable table = Table_SchemaTable(DbFile, TableName);

                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        string AllowDbNull = row["AllowDBNull"].ToString();
                        string ColumnName = row["ColumnName"].ToString();
                        string ColumnNum = row["ColumnOrdinal"].ToString();

                        if (ColNameOrPos.GetType().ToString() == "System.String")  // user passed in Column Name
                        {
                            if (ColNameOrPos.ToString().ToUpper() == ColumnName.ToUpper())   // match column by current column name 
                            {
                                if (AllowDbNull.ToUpper() == "TRUE") { return true; }
                                if (AllowDbNull.ToUpper() == "FALSE") { return false; }
                            }
                        }

                        if (ColNameOrPos.GetType().ToString() == "System.Int32")  // user passed in Column #
                        {
                            if (ColNameOrPos.ToString() == ColumnNum.ToString())   // match column by current col # int
                            {
                                if (AllowDbNull.ToUpper() == "TRUE") { return true; }
                                if (AllowDbNull.ToUpper() == "FALSE") { return false; }
                            }
                        }
                    }
                }

                return false;
            }

            /// <summary>Returns Max Size For SQLite Column (int)</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Read</param>
            /// <param name="ColNameOrPos">Column Name or Column Postion #</param>
            public int Column_Size(string DbFile, string TableName, object ColNameOrPos)
            {
                _AHK ahk = new _AHK();
                DataTable table = Table_SchemaTable(DbFile, TableName);

                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        string ColumnSize = row["ColumnSize"].ToString();
                        string ColumnName = row["ColumnName"].ToString();
                        string ColumnNum = row["ColumnOrdinal"].ToString();

                        if (ColNameOrPos.GetType().ToString() == "System.String")  // user passed in Column Name
                        {
                            if (ColNameOrPos.ToString().ToUpper() == ColumnName.ToUpper())   // match column by current column name 
                            {
                                return ahk.ToInt(ColumnSize);
                            }
                        }

                        if (ColNameOrPos.GetType().ToString() == "System.Int32")  // user passed in Column #
                        {
                            if (ColNameOrPos.ToString() == ColumnNum.ToString())   // match column by current col # int
                            {
                                return ahk.ToInt(ColumnSize);
                            }
                        }
                    }
                }

                return -1;
            }

            /// <summary>Returns Column Name From the Column Position #</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Read</param>
            /// <param name="ColumnPos">Column Postion #</param>
            public string Column_Name(string DbFile, string TableName, int ColumnPos)
            {
                DataTable table = Table_SchemaTable(DbFile, TableName);

                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        string ColumnSize = row["ColumnSize"].ToString();
                        string ColumnName = row["ColumnName"].ToString();
                        string ColumnNum = row["ColumnOrdinal"].ToString();

                        if (ColumnPos.ToString() == ColumnNum.ToString())   // match column by current col # int
                        {
                            return ColumnName;
                        }

                    }
                }

                return "";
            }

            // add new column to existing sqlite table (checks to see if column already exists before attempting to add)
            public bool Column_Add(string DbFile, string TableName, string NewColumnName)
            {
                // check to see if column name exists in table
                bool colExists = IsColumn(DbFile, TableName, NewColumnName);

                if (!colExists)
                {
                    bool added = Execute(DbFile, "Alter table " + TableName + " Add Column " + NewColumnName + " VARCHAR");
                    return added;
                }

                return true;
            }

            #endregion

            #region === SQLite: Settings Db ===


            // Save Setting / Return Setting Value from Settings.sqlite

            public string Setting(string SettingName, string SettingValue = "", string Option = "", string DbFile = "Settings.sqlite")
            {
                _AHK ahk = new _AHK();

                if (DbFile.Trim() == "" || DbFile.ToUpper() == "SETTINGS.SQLITE") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                if (SettingValue != "") { Setting_Save(SettingName, SettingValue, Option, DbFile); return SettingValue; }

                return Setting_Value(SettingName, DbFile);
            }


            /// <summary>Saves Setting to Project SQLite Database File</summary>
            /// <param name="SettingName">Unique Setting Name To Store User Value</param>
            /// <param name="Value">Value For Setting To Save And Return Later</param>
            /// <param name="Option">Optional Parameter For Enabling/Disabling/Sorting Settings</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To Settings.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            public bool Setting_Save(string SettingName, string Value, string Option = "", string DbFile = "Settings.sqlite", string TableName = "Settings")
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                // Create New Table If It Does NOT Exist Yet
                bool TableExist = Table_Exists(DbFile, TableName);  //See if selected Table Exists in SQLite DB file
                if (!TableExist) { Settings_NewDb(DbFile, TableName); }  // Table DOES NOT exist in SQLite DB - Create Now

                // UPDATE or INSERT Server Files
                string UpdateLine = "UPDATE " + TableName + " set Value = '" + Value + "', Option = '" + Option + "',TimeStamp = '" + DateTime.Now.ToString() + "' WHERE Setting = '" + SettingName + "'";
                if (Option != "") { UpdateLine = "UPDATE " + TableName + " set Value = '" + Value + "', Option = '" + Option + "',TimeStamp = '" + DateTime.Now.ToString() + "' WHERE Setting = '" + SettingName + "' AND Option = '" + Option + "'"; }

                bool Updated = Execute(DbFile, UpdateLine);  // Update Table

                if (!Updated) { Updated = Execute(DbFile, "INSERT into " + TableName + " (Setting, Value, Option, TimeStamp) values ('" + SettingName + "','" + Value + "','" + Option + "','" + DateTime.Now.ToString() + "')"); }  // insert into a Table
                if (!Updated) { MessageBox.Show("Failed to Insert: " + SettingName + "' | '" + Value + " | " + Option + "' | '" + DateTime.Now.ToString()); }
                return Updated;
            }

            /// <summary>Set Enabled Value to FALSE For SettingName</summary>
            /// <param name="SettingName">Unique Setting Name To Store User Value</param>
            /// <param name="Value">Value For Setting To Save And Return Later</param>
            /// <param name="Option">Optional Parameter For Enabling/Disabling/Sorting Settings</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To Settings.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            public bool Setting_Disable(string SettingName, string Value, string Option = "", string DbFile = "Settings.sqlite", string TableName = "Settings")
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                // Create New Table If It Does NOT Exist Yet
                bool TableExist = Table_Exists(DbFile, TableName);  //See if selected Table Exists in SQLite DB file
                if (!TableExist) { Settings_NewDb(DbFile, TableName); }  // Table DOES NOT exist in SQLite DB - Create Now

                // UPDATE or INSERT Server Files
                string UpdateLine = "UPDATE " + TableName + " set Value = '" + Value + "', Option = '" + Option + "',TimeStamp = '" + DateTime.Now.ToString() + "' WHERE Setting = '" + SettingName + "'";
                if (Option != "") { UpdateLine = "UPDATE " + TableName + " set Value = '" + Value + "', Option = '" + Option + "',TimeStamp = '" + DateTime.Now.ToString() + "' WHERE Setting = '" + SettingName + "' AND Option = '" + Option + "'"; }

                bool Updated = Execute(DbFile, UpdateLine);  // Update Table
                return Updated;
            }

            /// <summary>Return Setting From Project SQLite DataBase File</summary>
            /// <param name="SettingName">Unique Setting Name To Store User Value</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To AppName.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            /// <param name="OptionValue">If provided, this field serves as another search condition when returning the setting value</param>
            /// <param name="ReturnOptionValue">If true, returns value in Option field instead of default Setting Value</param>
            public string Setting_Value(string SettingName, string DbFile = "Settings.sqlite", string TableName = "Settings", string OptionValue = "", bool ReturnOptionValue = false)
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                if (!File.Exists(DbFile)) { return ""; } // db not created yet

                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

                string SearchLine = "Select * from " + TableName + " WHERE Setting = '" + SettingName + "'";
                if (OptionValue != "") { SearchLine = "Select * from " + TableName + " WHERE Setting = '" + SettingName + "' AND Option = '" + OptionValue + "'"; }

                SQLiteDataReader reader = lite.ReturnSQLite(SearchLine, m_dbConnection);  // request data from DB

                if (reader == null) { return ""; }

                string Value = "";
                string Option = "";
                string TimeStamp = "";
                while (reader.Read())    // loop through each row returned from select 
                {
                    TimeStamp = reader["TimeStamp"].ToString();
                    Value = reader["Value"].ToString();
                    Option = reader["Option"].ToString();
                }

                lite.Disconnect(m_dbConnection);  // free up db for other use

                if (ReturnOptionValue) { return Option; }

                return Value;
            }

            /// <summary>Returns Setting Option Value From Project SQLite Database File</summary>
            /// <param name="SettingName">Unique Setting Name To Store User Value</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To AppName.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            public string Setting_Option(string SettingName, string DbFile = "Settings.sqlite", string TableName = "Settings")
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

                string SearchLine = "Select * from " + TableName + " WHERE Setting = '" + SettingName + "'";
                SQLiteDataReader reader = lite.ReturnSQLite(SearchLine, m_dbConnection);  // request data from DB

                string Value = "";
                string Option = "";
                string TimeStamp = "";
                string Enabled = "";
                while (reader.Read())    // loop through each row returned from select 
                {
                    TimeStamp = reader["TimeStamp"].ToString();
                    Value = reader["Value"].ToString();
                    Option = reader["Option"].ToString();
                    Enabled = reader["Enabled"].ToString();
                }

                lite.Disconnect(m_dbConnection);  // free up db for other use
                return Option;
            }

            /// <summary>Returns Setting Option Value From Project SQLite Database File</summary>
            /// <param name="SettingName">Unique Setting Name To Store User Value</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To AppName.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            public bool Setting_Enabled(string SettingName, string DbFile = "Settings.sqlite", string TableName = "Settings", string OptionValue = "")
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

                string SearchLine = "Select * from " + TableName + " WHERE Setting = '" + SettingName + "'";
                if (OptionValue != "") { SearchLine = "Select * from " + TableName + " WHERE Setting = '" + SettingName + "' AND Option = '" + OptionValue + "'"; }

                SQLiteDataReader reader = lite.ReturnSQLite(SearchLine, m_dbConnection);  // request data from DB

                string Value = "";
                string Option = "";
                string TimeStamp = "";
                string Enabled = "";
                while (reader.Read())    // loop through each row returned from select 
                {
                    TimeStamp = reader["TimeStamp"].ToString();
                    Value = reader["Value"].ToString();
                    Option = reader["Option"].ToString();
                    Enabled = reader["Enabled"].ToString();
                }

                lite.Disconnect(m_dbConnection);  // free up db for other use

                if (Enabled.ToUpper().Trim() == "TRUE" || Enabled.Trim() == "1") { return true; }
                return false;
            }

            /// <summary>Returns List of All Setting Names Stored in Project SQLite Database File</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To Settings.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            public List<string> Setting_List(string DbFile = "Settings.sqlite", string TableName = "Settings")
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

                string SearchLine = "Select * from " + TableName;
                SQLiteDataReader reader = lite.ReturnSQLite(SearchLine, m_dbConnection);  // request data from DB

                List<string> Settings = new List<string>();

                while (reader.Read())    // loop through each row returned from select 
                {
                    string Setting = reader["Setting"].ToString();
                    Settings.Add(Setting);
                }

                lite.Disconnect(m_dbConnection);  // free up db for other use
                return Settings;
            }

            /// <summary>Creates New Project SQLite Settings Database + Table. Writes Automatically On First Save If Not Executed Elsewhere First</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To AppName.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            /// <param name="OverWriteExisting">Option to Overwrite Previous Setting Table If Found</param>
            public bool Settings_NewDb(string DbFile = "Settings.sqlite", string TableName = "Settings", bool OverWriteExisting = false)
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                if (OverWriteExisting)  // option to clear out previous Db setttings
                {
                    Table_Clear(DbFile, TableName, true);
                }

                // Create New SQLite DB (*Used First-Run*)
                if (!File.Exists(DbFile)) // create database file if it doen't exist
                {
                    SQLiteConnection.CreateFile(DbFile);
                }

                // Create New Table If It Does NOT Exist Yet
                bool TableExist = Table_Exists(DbFile, TableName);  //See if selected Table Exists in SQLite DB file

                if (!TableExist)  // Table DOES NOT exist in SQLite DB
                {

                    string NewTableLine = "ID INTEGER PRIMARY KEY, Setting VARCHAR, Value VARCHAR, Option VARCHAR, TimeStamp VARCHAR, Enabled VARCHAR";

                    //ahk.MsgBox(NewTableLine); 

                    bool ReturnValue = Execute(DbFile, "CREATE TABLE [" + TableName + "] (" + NewTableLine + ")");  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
                }


                if (File.Exists(DbFile)) { return true; }
                return false;
            }

            /// <summary>Creates Database Index Table - Stores list of projected .sqlite files</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To Settings.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            /// <param name="OverWriteExisting">Option to Overwrite Previous Setting Table If Found</param>
            public bool Create_DbIndex_Table(string DbFile = "Settings.sqlite", string TableName = "DbIndex", bool OverWriteExisting = false)
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                if (OverWriteExisting)  // option to clear out previous Db setttings
                {
                    Table_Clear(DbFile, TableName, true);
                }

                // Create New SQLite DB (*Used First-Run*)
                if (!File.Exists(DbFile)) // create database file if it doen't exist
                {
                    SQLiteConnection.CreateFile(DbFile);
                }

                // Create New Table If It Does NOT Exist Yet
                bool TableExist = Table_Exists(DbFile, TableName);  //See if selected Table Exists in SQLite DB file

                if (!TableExist)  // Table DOES NOT exist in SQLite DB
                {

                    string NewTableLine = "ID INTEGER PRIMARY KEY, FilePath VARCHAR, DbLoaded VARCHAR, DisplayDb VARCHAR, DateModified VARCHAR, ComputerName VARCHAR, Enabled VARCHAR";

                    //ahk.MsgBox(NewTableLine); 

                    bool ReturnValue = Execute(DbFile, "CREATE TABLE [" + TableName + "] (" + NewTableLine + ")");  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
                }


                if (File.Exists(DbFile)) { return true; }
                return false;
            }


            #endregion

            #region === SQLite: PRIVATE Settings Db ===

            // reads connection string values from private.db and returns connection string for db use
            public SqlConnection GoDaddyConnection()
            {
                string goDaddyConnectString = pSetting_Value("GoDaddyConnection");
                SqlConnection GoDad = new SqlConnection(goDaddyConnectString);
                return GoDad;
            }


            /// <summary>Saves Setting to Project SQLite Database File</summary>
            /// <param name="SettingName">Unique Setting Name To Store User Value</param>
            /// <param name="Value">Value For Setting To Save And Return Later</param>
            /// <param name="Option">Optional Parameter For Enabling/Disabling/Sorting Settings</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To Settings.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            public bool pSetting_Save(string SettingName, string Value, string Option = "", string DbFile = "Settings.sqlite", string TableName = "Settings")
            {
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = pSettingPath(); }

                // Create New Table If It Does NOT Exist Yet
                bool TableExist = Table_Exists(DbFile, TableName);  //See if selected Table Exists in SQLite DB file
                if (!TableExist) { pSettings_NewDb(DbFile, TableName); }  // Table DOES NOT exist in SQLite DB - Create Now

                // UPDATE or INSERT Server Files
                string UpdateLine = "UPDATE " + TableName + " set Value = '" + Value + "', Option = '" + Option + "',TimeStamp = '" + DateTime.Now.ToString() + "' WHERE Setting = '" + SettingName + "'";
                if (Option != "") { UpdateLine = "UPDATE " + TableName + " set Value = '" + Value + "', Option = '" + Option + "',TimeStamp = '" + DateTime.Now.ToString() + "' WHERE Setting = '" + SettingName + "' AND Option = '" + Option + "'"; }

                bool Updated = Execute(DbFile, UpdateLine);  // Update Table

                if (!Updated) { Updated = Execute(DbFile, "INSERT into " + TableName + " (Setting, Value, Option, TimeStamp) values ('" + SettingName + "','" + Value + "','" + Option + "','" + DateTime.Now.ToString() + "')"); }  // insert into a Table
                if (!Updated) { MessageBox.Show("Failed to Insert: " + SettingName + "' | '" + Value + " | " + Option + "' | '" + DateTime.Now.ToString()); }
                return Updated;
            }

            /// <summary>Set Enabled Value to FALSE For SettingName</summary>
            /// <param name="SettingName">Unique Setting Name To Store User Value</param>
            /// <param name="Value">Value For Setting To Save And Return Later</param>
            /// <param name="Option">Optional Parameter For Enabling/Disabling/Sorting Settings</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To Settings.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            public bool pSetting_Disable(string SettingName, string Value, string Option = "", string DbFile = "Settings.sqlite", string TableName = "Settings")
            {
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = pSettingPath(); }

                // Create New Table If It Does NOT Exist Yet
                bool TableExist = Table_Exists(DbFile, TableName);  //See if selected Table Exists in SQLite DB file
                if (!TableExist) { pSettings_NewDb(DbFile, TableName); }  // Table DOES NOT exist in SQLite DB - Create Now

                // UPDATE or INSERT Server Files
                string UpdateLine = "UPDATE " + TableName + " set Value = '" + Value + "', Option = '" + Option + "',TimeStamp = '" + DateTime.Now.ToString() + "' WHERE Setting = '" + SettingName + "'";
                if (Option != "") { UpdateLine = "UPDATE " + TableName + " set Value = '" + Value + "', Option = '" + Option + "',TimeStamp = '" + DateTime.Now.ToString() + "' WHERE Setting = '" + SettingName + "' AND Option = '" + Option + "'"; }

                bool Updated = Execute(DbFile, UpdateLine);  // Update Table
                return Updated;
            }

            /// <summary>Return Setting From Project SQLite DataBase File</summary>
            /// <param name="SettingName">Unique Setting Name To Store User Value</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To AppName.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            /// <param name="OptionValue">If provided, this field serves as another search condition when returning the setting value</param>
            /// <param name="ReturnOptionValue">If true, returns value in Option field instead of default Setting Value</param>
            public string pSetting_Value(string SettingName, string DbFile = "Settings.sqlite", string TableName = "Settings", string OptionValue = "", bool ReturnOptionValue = false)
            {
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = pSettingPath(); }

                if (!File.Exists(DbFile)) { return ""; } // db not created yet

                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

                string SearchLine = "Select * from " + TableName + " WHERE Setting = '" + SettingName + "'";
                if (OptionValue != "") { SearchLine = "Select * from " + TableName + " WHERE Setting = '" + SettingName + "' AND Option = '" + OptionValue + "'"; }

                SQLiteDataReader reader = lite.ReturnSQLite(SearchLine, m_dbConnection);  // request data from DB

                if (reader == null) { return ""; }

                string Value = "";
                string Option = "";
                string TimeStamp = "";
                while (reader.Read())    // loop through each row returned from select 
                {
                    TimeStamp = reader["TimeStamp"].ToString();
                    Value = reader["Value"].ToString();
                    Option = reader["Option"].ToString();
                }

                lite.Disconnect(m_dbConnection);  // free up db for other use

                if (ReturnOptionValue) { return Option; }

                return Value;
            }

            /// <summary>Returns Setting Option Value From Project SQLite Database File</summary>
            /// <param name="SettingName">Unique Setting Name To Store User Value</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To AppName.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            public string pSetting_Option(string SettingName, string DbFile = "Settings.sqlite", string TableName = "Settings")
            {
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = pSettingPath(); }

                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

                string SearchLine = "Select * from " + TableName + " WHERE Setting = '" + SettingName + "'";
                SQLiteDataReader reader = lite.ReturnSQLite(SearchLine, m_dbConnection);  // request data from DB

                string Value = "";
                string Option = "";
                string TimeStamp = "";
                string Enabled = "";
                while (reader.Read())    // loop through each row returned from select 
                {
                    TimeStamp = reader["TimeStamp"].ToString();
                    Value = reader["Value"].ToString();
                    Option = reader["Option"].ToString();
                    Enabled = reader["Enabled"].ToString();
                }

                lite.Disconnect(m_dbConnection);  // free up db for other use
                return Option;
            }

            /// <summary>Returns Setting Option Value From Project SQLite Database File</summary>
            /// <param name="SettingName">Unique Setting Name To Store User Value</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To AppName.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            public bool pSetting_Enabled(string SettingName, string DbFile = "Settings.sqlite", string TableName = "Settings", string OptionValue = "")
            {
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = pSettingPath(); }

                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

                string SearchLine = "Select * from " + TableName + " WHERE Setting = '" + SettingName + "'";
                if (OptionValue != "") { SearchLine = "Select * from " + TableName + " WHERE Setting = '" + SettingName + "' AND Option = '" + OptionValue + "'"; }

                SQLiteDataReader reader = lite.ReturnSQLite(SearchLine, m_dbConnection);  // request data from DB

                string Value = "";
                string Option = "";
                string TimeStamp = "";
                string Enabled = "";
                while (reader.Read())    // loop through each row returned from select 
                {
                    TimeStamp = reader["TimeStamp"].ToString();
                    Value = reader["Value"].ToString();
                    Option = reader["Option"].ToString();
                    Enabled = reader["Enabled"].ToString();
                }

                lite.Disconnect(m_dbConnection);  // free up db for other use

                if (Enabled.ToUpper().Trim() == "TRUE" || Enabled.Trim() == "1") { return true; }
                return false;
            }

            /// <summary>Returns List of All Setting Names Stored in Project SQLite Database File</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To Settings.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            public List<string> pSetting_List(string DbFile = "Settings.sqlite", string TableName = "Settings")
            {
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = pSettingPath(); }

                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

                string SearchLine = "Select * from " + TableName;
                SQLiteDataReader reader = lite.ReturnSQLite(SearchLine, m_dbConnection);  // request data from DB

                List<string> Settings = new List<string>();

                while (reader.Read())    // loop through each row returned from select 
                {
                    string Setting = reader["Setting"].ToString();
                    Settings.Add(Setting);
                }

                lite.Disconnect(m_dbConnection);  // free up db for other use
                return Settings;
            }

            /// <summary>Creates New Project SQLite Settings Database + Table. Writes Automatically On First Save If Not Executed Elsewhere First</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To AppName.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            /// <param name="OverWriteExisting">Option to Overwrite Previous Setting Table If Found</param>
            public bool pSettings_NewDb(string DbFile = "Settings.sqlite", string TableName = "Settings", bool OverWriteExisting = false)
            {
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = pSettingPath(); }

                if (OverWriteExisting)  // option to clear out previous Db setttings
                {
                    Table_Clear(DbFile, TableName, true);
                }

                // Create New SQLite DB (*Used First-Run*)
                if (!File.Exists(DbFile)) // create database file if it doen't exist
                {
                    SQLiteConnection.CreateFile(DbFile);
                }

                // Create New Table If It Does NOT Exist Yet
                bool TableExist = Table_Exists(DbFile, TableName);  //See if selected Table Exists in SQLite DB file

                if (!TableExist)  // Table DOES NOT exist in SQLite DB
                {

                    string NewTableLine = "ID INTEGER PRIMARY KEY, Setting VARCHAR, Value VARCHAR, Option VARCHAR, TimeStamp VARCHAR, Enabled VARCHAR";

                    //ahk.MsgBox(NewTableLine); 

                    bool ReturnValue = Execute(DbFile, "CREATE TABLE [" + TableName + "] (" + NewTableLine + ")");  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
                }


                if (File.Exists(DbFile)) { return true; }
                return false;
            }


            /// <summary>Creates Database Index Table - Stores list of projected .sqlite files</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To Settings.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            /// <param name="OverWriteExisting">Option to Overwrite Previous Setting Table If Found</param>
            public bool pCreate_DbIndex_Table(string DbFile = "Settings.sqlite", string TableName = "DbIndex", bool OverWriteExisting = false)
            {
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = pSettingPath(); }

                if (OverWriteExisting)  // option to clear out previous Db setttings
                {
                    Table_Clear(DbFile, TableName, true);
                }

                // Create New SQLite DB (*Used First-Run*)
                if (!File.Exists(DbFile)) // create database file if it doen't exist
                {
                    SQLiteConnection.CreateFile(DbFile);
                }

                // Create New Table If It Does NOT Exist Yet
                bool TableExist = Table_Exists(DbFile, TableName);  //See if selected Table Exists in SQLite DB file

                if (!TableExist)  // Table DOES NOT exist in SQLite DB
                {

                    string NewTableLine = "ID INTEGER PRIMARY KEY, FilePath VARCHAR, DbLoaded VARCHAR, DisplayDb VARCHAR, DateModified VARCHAR, ComputerName VARCHAR, Enabled VARCHAR";

                    //ahk.MsgBox(NewTableLine); 

                    bool ReturnValue = Execute(DbFile, "CREATE TABLE [" + TableName + "] (" + NewTableLine + ")");  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
                }


                if (File.Exists(DbFile)) { return true; }
                return false;
            }

            /// <summary>Returns the path to user's private settings file</summary>
            public string pSettingPath()
            {
                _AHK ahk = new _AHK();
                string appDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                ahk.FileCreateDir(appDir + "\\sharpAHK");
                return appDir + "\\sharpAHK\\sharpAHK_Private.sqlite";
            }

            #endregion

            #region === SQLite: History Db ===

            /// <summary>Creates New Project SQLite Settings Database + Table For Loaded File/Value History. Writes Automatically On First Save If Not Executed Elsewhere First</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To AppName.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'History'</param>
            /// <param name="OverWriteOld">Option to Overwrite Previous Setting Table If Found</param>
            public bool History_NewDb(string DbFile = "Settings.sqlite", string TableName = "History", bool OverWriteExisting = false)
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                if (OverWriteExisting)  // option to clear out previous Db setttings
                {
                    Table_Clear(DbFile, TableName, true);
                }

                // Create New SQLite DB (*Used First-Run*)
                if (!File.Exists(DbFile)) // create database file if it doen't exist
                {
                    SQLiteConnection.CreateFile(DbFile);
                }

                // Create New Table If It Does NOT Exist Yet
                bool TableExist = Table_Exists(DbFile, TableName);  //See if selected Table Exists in SQLite DB file

                if (!TableExist)  // Table DOES NOT exist in SQLite DB
                {

                    string NewTableLine = "ID INTEGER PRIMARY KEY, FieldName VARCHAR, FieldValue VARCHAR, Option VARCHAR, TimeStamp VARCHAR";

                    //ahk.MsgBox(NewTableLine); 

                    bool ReturnValue = Execute(DbFile, "CREATE TABLE [" + TableName + "] (" + NewTableLine + ")");  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
                }


                if (File.Exists(DbFile)) { return true; }
                return false;
            }

            /// <summary>Saves History / Previously Used Values to Project SQLite Database File</summary>
            /// <param name="FieldName">Field name to group stored history values as (name of control or type of history logged</param>
            /// <param name="FieldValue">Value For Field to Store</param>
            /// <param name="Option">Optional Parameter For Enabling/Disabling/Sorting History Values</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To Settings.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'History'</param>
            public bool History_Save(string FieldName, string FieldValue, string Option = "", string DbFile = "Settings.sqlite", string TableName = "History")
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                // Create New Table If It Does NOT Exist Yet
                bool TableExist = Table_Exists(DbFile, TableName);  //See if selected Table Exists in SQLite DB file
                if (!TableExist) { History_NewDb(DbFile, TableName); }  // Table DOES NOT exist in SQLite DB - Create Now

                bool ReturnBool = Execute(DbFile, "INSERT into " + TableName + " (FieldName, FieldValue, Option, TimeStamp) values ('" + FieldName + "','" + FieldValue + "','" + Option + "','" + DateTime.Now.ToString() + "')");  // insert into a Table
                if (ReturnBool == false) { ahk.MsgBox("Failed to Insert: " + FieldName + "' | '" + FieldValue + " | " + Option + "' | '" + DateTime.Now.ToString()); }
                return ReturnBool;
            }

            /// <summary>Returns List of All Setting Names Stored in Project SQLite Database File</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To Settings.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'History'</param>
            public List<string> History_List(string FieldName, int HistoryCount = 10, string DbFile = "Settings.sqlite", string TableName = "History")
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

                //Select * from History WHERE FieldName = 'LoadedDir' Order by ID Desc LIMIT 3
                string SearchLine = "Select Distinct FieldValue from " + TableName + " WHERE FieldName = '" + FieldName + "' Order by ID Desc LIMIT " + HistoryCount.ToString();
                SQLiteDataReader reader = lite.ReturnSQLite(SearchLine, m_dbConnection);  // request data from DB

                List<string> Settings = new List<string>();

                while (reader.Read())    // loop through each row returned from select 
                {
                    string Setting = reader["FieldValue"].ToString();
                    Settings.Add(Setting);
                }

                lite.Disconnect(m_dbConnection);  // free up db for other use
                return Settings;
            }

            /// <summary>Load previously used values from SQLite History Log To Control</summary>
            /// <param name="controlName">Currently works with ToolStripComboBox Controls</param>
            /// <param name="FieldName">This can be any unique name to store different history lists. Ex: comboBox1List</param>
            /// <param name="HistoryCount">Number of records to return (most recent to oldest)</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To Settings.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            public void History_Load(object controlName, bool ShowLastValue = false, string FieldName = "Loaded", int HistoryCount = 10, string DbFile = "Settings.sqlite", string TableName = "History")
            {
                _GridControl grid = new _GridControl();
                _Lists lst = new _Lists();

                // load list of history events from sqlite db to list
                List<string> LoadHist = History_List(FieldName, HistoryCount, DbFile, TableName);

                // if toolstrip combobox passed in - populate
                if (controlName.GetType().ToString() == "System.Windows.Forms.ToolStripComboBox")
                {
                    ToolStripComboBox combo = (ToolStripComboBox)controlName;

                    combo.ComboBox.DataSource = null;
                    //combo.ComboBox.DisplayMember = "DisplayName";
                    //combo.ComboBox.ValueMember = "Value";
                    combo.ComboBox.DataSource = LoadHist;

                    if (!ShowLastValue) { combo.Text = ""; }
                    if (ShowLastValue) { combo.SelectedIndex = 0; } // select first item in combobox to display
                }


                // if toolstrip combobox passed in - populate
                if (controlName.GetType().ToString() == "System.Windows.Forms.DataGridView")
                {
                    DataGridView dv = (DataGridView)controlName;
                    grid.List_To_Grid(dv, LoadHist, FieldName);
                }

            }

            /// <summary>Clears History Table / Specific Field from SQLite History Log</summary>
            /// <param name="FieldName">Name of a specific field in the History table to clear, or Default = ALL fields in table</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To Settings.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'History'</param>
            public bool History_Clear(string FieldName = "ALL", string DbFile = "Settings.sqlite", string TableName = "History")
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                // Create New Table If It Does NOT Exist Yet
                bool TableExist = Table_Exists(DbFile, TableName);  //See if selected Table Exists in SQLite DB file
                if (!TableExist) { History_NewDb(DbFile, TableName); }  // Table DOES NOT exist in SQLite DB - Create Now

                // assemble delete statement - if user passed in a field name to remove, add condition, otherwise delete all from History table
                string Command = "DELETE FROM " + TableName;
                if (FieldName.ToUpper() != "ALL") { Command = Command + " WHERE FieldName = '" + FieldName + "'"; }

                bool ReturnBool = Execute(DbFile, Command);  // delete items in table
                return ReturnBool;
            }


            #endregion

            #region === SQLite: UserLists ===


            /// <summary>
            /// Launches New List Editor Dialog with Ability to Edit / Save User Lists to SQLite Db
            /// </summary>
            /// <param name="ListName">Optional Name of List to Populate Editor on Startup</param>
            /// <example>
            ///             _Database.SQLite sqlite = new _Database.SQLite();
            ///             sqlite.ListEditor();
            /// </example>
            public void ListEditor(string ListName = "")
            {
                sharpAHK_Dev.Controls.listEditor form = new sharpAHK_Dev.Controls.listEditor(ListName);
                form.Show();
            }


            /// <summary> create a tag table in Sqlite Db File</summary>
            /// <param name="NewDBFile"> </param>
            /// <param name="TableName"> </param>
            public bool Create_UserListsDb(string DbFile, string TableName = "UserLists")
            {
                // create database file if it doen't exist
                if (!File.Exists(DbFile)) { SQLiteConnection.CreateFile(DbFile); }

                // Create New Table If It Does NOT Exist Yet
                bool TableExist = Table_Exists(DbFile, TableName);  //See if selected Table Exists in SQLite DB file

                if (!TableExist)  // Table DOES NOT exist in SQLite DB
                {
                    string NewTableLine = "";

                    //NewTableLine = "ID INTEGER PRIMARY KEY, ListName VARCHAR, ListAction VARCHAR, ListValue VARCHAR, ListOption VARCHAR, UserNotes VARCHAR, DateAdded VARCHAR, DateModified VARCHAR, Enabled VARCHAR";
                    NewTableLine = "ID INTEGER PRIMARY KEY, ListName VARCHAR, Setting VARCHAR, Value VARCHAR, Option VARCHAR, TimeStamp VARCHAR, Enabled VARCHAR";

                    //ahk.MsgBox(NewTableLine); 

                    bool ReturnValue = Execute(DbFile, "CREATE TABLE " + TableName + " (" + NewTableLine + ")");

                    return ReturnValue;
                }

                return TableExist;
            }

            public bool UserLists_Save(string ListName, string SettingName, string Value, string Option = "", string DbFile = "Settings.sqlite", string TableName = "UserLists")
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                // Create New Table If It Does NOT Exist Yet
                bool TableExist = Table_Exists(DbFile, TableName);  //See if selected Table Exists in SQLite DB file
                if (!TableExist) { Settings_NewDb(DbFile, TableName); }  // Table DOES NOT exist in SQLite DB - Create Now

                // UPDATE or INSERT Server Files
                string UpdateLine = "UPDATE " + TableName + " set Setting = '" + SettingName + "', Value = '" + Value + "', Option = '" + Option + "',TimeStamp = '" + DateTime.Now.ToString() + "' WHERE ListName = '" + ListName + "' AND Setting = '" + SettingName + "'";
                if (Option != "") { UpdateLine = "UPDATE " + TableName + " set Value = '" + Value + "', Option = '" + Option + "',TimeStamp = '" + DateTime.Now.ToString() + "' WHERE Setting = '" + SettingName + "' AND Option = '" + Option + "'"; }

                bool Updated = Execute(DbFile, UpdateLine);  // Update Table
                bool ReturnBool = true;
                if (!Updated) { ReturnBool = Execute(DbFile, "INSERT into " + TableName + " (Setting, Value, Option, TimeStamp) values ('" + SettingName + "','" + Value + "','" + Option + "','" + DateTime.Now.ToString() + "')"); }  // insert into a Table
                if (ReturnBool == false) { MessageBox.Show("Failed to Insert: " + SettingName + "' | '" + Value + " | " + Option + "' | '" + DateTime.Now.ToString()); }
                return ReturnBool;
            }

            public List<string> UserList_Names(string DbFile = "Settings.sqlite", string TableName = "UserLists")
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

                string SearchLine = "Select * from " + TableName;
                SQLiteDataReader reader = lite.ReturnSQLite(SearchLine, m_dbConnection);  // request data from DB

                List<string> Settings = new List<string>();

                while (reader.Read())    // loop through each row returned from select 
                {
                    string Setting = reader["Setting"].ToString();
                    Settings.Add(Setting);
                }

                lite.Disconnect(m_dbConnection);  // free up db for other use
                return Settings;
            }


            #endregion

            #region === SQLite: Save/Return Lists ===


            /// <summary>
            /// Return List of values from SQLite Table
            /// </summary>
            /// <param name="SQLiteCommand">SQLite Command Returning Column of Values</param>
            /// <param name="DbFile">SQLite Database FilePath</param>
            /// <returns></returns>
            public List<string> SQLite_To_List(string SQLiteCommand, string DbFile = "Settings.sqlite")
            {
                DataTable dt = GetDataTable(DbFile, SQLiteCommand);

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

            /// <summary>
            /// Returns List Values if ListValues is Null, Adds to Existing List if OverWriteList = False, Updates Existing List When values Provided and OverWriteList = True
            /// </summary>
            /// <param name="ListName">Name of List to Return or Update</param>
            /// <param name="ListValues">Add List<string> of Values, Single string Value, or leave Blank to Return Value From Exsting List</param>
            /// <param name="OverWriteList">Enable to Update Existing List Contents, Overwriting All Previous Values</param>
            /// <param name="DbFile">SQLite Database FilePath</param>
            /// <param name="ListTableName">Name of Table Storing Lists</param>
            /// <returns></returns>
            public List<string> List(string ListName, object listValues = null, bool OverWriteList = true, string DbFile = "Settings.sqlite", string ListTableName = "Lists")
            {
                _AHK ahk = new _AHK();
                if (DbFile.Trim() == "" || DbFile.ToUpper() == "SETTINGS.SQLITE") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                List<string> lstValues = new List<string>();

                if (listValues != null)
                {
                    // Determine if User Passed in 'string' value or 'List<string>'
                    string VarType = listValues.GetType().ToString();  //determine what kind of variable was passed into function
                    if (VarType == "System.String") { lstValues.Add(listValues.ToString()); }
                    else if (VarType == "System.Collections.Generic.List`1[System.String]") { lstValues = (List<string>)listValues; }
                }



                Lists_NewDb(DbFile, ListTableName, false);  // creates new sqlite file / adds list table to existing sqlite file if doesn't exist already

                List<string> returnList = new List<string>();

                if (lstValues != null && lstValues.Count > 0)  // Write / Update List
                {
                    if (OverWriteList)  // option to clear out existing list items, otherwise adds to existing list 
                    {
                        string cmd = "Delete from '" + ListTableName + "' WHERE ListName = '" + ListName + "'";
                        Execute(DbFile, cmd);
                    }

                    string Option = "";

                    List<string> commands = new List<string>();
                    foreach (string item in lstValues)
                    {
                        string cmd = "INSERT into " + ListTableName + " (ListName, ListValue, Option, TimeStamp) values ('" + ListName + "','" + item + "','" + Option + "','" + DateTime.Now.ToString() + "')";
                        commands.Add(cmd);
                    }

                    Batch_Insert(DbFile, commands);

                }
                else  // Otherwise Return Existing List
                {
                    returnList = ListValues(ListName, ListOrder.ID, DbFile, ListTableName);
                }


                //if (SettingValue != "") { Setting_Save(SettingName, SettingValue, Option, DbFile); return SettingValue; }

                return returnList;
            }

            /// <summary>
            /// Returns List of Values from SQLite Db
            /// </summary>
            /// <param name="ListName">Name of List to Return Values For</param>
            /// <param name="SortAlpha">Option to Sort List Results Alpha, Default Sorts by Original Entry Order (ID Field)</param>
            /// <param name="DbFile">SQLite Database FilePath</param>
            /// <param name="ListTableName">Name of Table Storing Lists</param>
            /// <returns></returns>
            public List<string> ListValues(string ListName, ListOrder SortBy = ListOrder.ListValue, string DbFile = "Settings.sqlite", string ListTableName = "Lists")
            {
                _AHK ahk = new _AHK();
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                string orderBy = " order by ID";
                if (SortBy == ListOrder.ListValue) { orderBy = " order by ListValue"; }
                if (SortBy == ListOrder.TimeStamp) { orderBy = " order by TimeStamp"; }
                if (SortBy == ListOrder.ID) { orderBy = " order by ID"; }
                if (SortBy == ListOrder.Option) { orderBy = " order by Option"; }

                string cmd = "Select ListValue from " + ListTableName + " Where ListName = '" + ListName + "'" + orderBy;

                return SQLite_To_List(cmd, DbFile);
            }

            /// <summary>
            /// Used to Determine List Sort Order 
            /// </summary>
            public enum ListOrder
            {
                ID = 3,
                ListValue = 0,
                TimeStamp = 1,
                Option = 2
            }


            /// <summary>
            /// Adds New Entry To Existing List
            /// </summary>
            /// <param name="DbFile">SQLite Database FilePath</param>
            /// <param name="ListTableName">Name of Table Storing Lists</param>
            /// <returns></returns>
            public bool AddToList(string ListName, string NewListValue, bool AddIfUnique = false, string Option = "", string DbFile = "Settings.sqlite", string ListTableName = "Lists")
            {
                _AHK ahk = new _AHK();
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                // Option to Only Add Unique Entries to Existing List
                if (AddIfUnique)
                {
                    List<string> currentValues = ListValues(ListName, ListOrder.ID, DbFile, ListTableName);

                    if (currentValues.Contains(NewListValue))  // value already exists in list --- just update timestamp for existing item 
                    {
                        string cmdd = "Update " + ListTableName + " set TimeStamp = '" + DateTime.Now.ToString() + "' WHERE ListName = '" + ListName + "' and ListValue = '" + NewListValue + "'";
                        return Execute(DbFile, cmdd);
                    }
                }

                Lists_NewDb(DbFile, ListTableName, false);  // creates new sqlite file / adds list table to existing sqlite file if doesn't exist already

                string cmd = "INSERT into " + ListTableName + " (ListName, ListValue, Option, TimeStamp) values ('" + ListName + "','" + NewListValue + "','" + Option + "','" + DateTime.Now.ToString() + "')";
                return Execute(DbFile, cmd);
            }

            /// <summary>
            /// Returns Number of Values Found in ListName
            /// </summary>
            /// <param name="ListName"></param>
            /// <param name="NewListValue"></param>
            /// <param name="DbFile"></param>
            /// <param name="ListTableName"></param>
            /// <returns></returns>
            public int ListValueCount(string ListName, string NewListValue, string DbFile = "Settings.sqlite", string ListTableName = "Lists")
            {
                _AHK ahk = new _AHK();
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                return ReturnInt(DbFile, "select count(ListValue) from " + ListTableName + " where ListName = '" + ListName + "'");
            }

            /// <summary>
            /// Removes List Contents from List Table
            /// </summary>
            /// <param name="DbFile">SQLite Database FilePath</param>
            /// <param name="ListTableName">Name of Table Storing Lists</param>
            /// <returns></returns>
            public bool RemoveList(string ListName, string DbFile = "Settings.sqlite", string ListTableName = "Lists")
            {
                _AHK ahk = new _AHK();
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                string cmd = "Delete from " + ListTableName + " where ListName = '" + ListName + "'";

                return Execute(DbFile, cmd);
            }

            /// <summary>
            /// Returns List of ListNames in Users ListTable
            /// </summary>
            /// <param name="DbFile">SQLite Database FilePath</param>
            /// <param name="ListTableName">Name of Table Storing Lists</param>
            /// <returns></returns>
            public List<string> ListNames(string DbFile = "Settings.sqlite", string ListTableName = "Lists")
            {
                _AHK ahk = new _AHK();
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                string cmd = "Select distinct ListName from " + ListTableName + " order by ListName";

                return SQLite_To_List(cmd, DbFile);
            }

            /// <summary>
            /// Checks to See if ListName Already Exists in List Table
            /// </summary>
            /// <param name="ListName"></param>
            /// <param name="DbFile"></param>
            /// <param name="ListTableName"></param>
            /// <returns></returns>
            public bool ListExists(string ListName, string DbFile = "Settings.sqlite", string ListTableName = "Lists")
            {
                _AHK ahk = new _AHK();
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                List<string> lists = ListNames(DbFile, ListTableName);
                foreach (string list in lists)
                {
                    if (ListName.ToUpper() == list.ToUpper()) { return true; }
                }
                return false;
            }

            /// <summary>Creates New Project SQLite Settings Database + Table. Writes Automatically On First Save If Not Executed Elsewhere First</summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To AppName.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            /// <param name="OverWriteExisting">Option to Overwrite Previous Setting Table If Found</param>
            private bool Lists_NewDb(string DbFile = "Settings.sqlite", string TableName = "Lists", bool OverWriteExisting = false)
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                if (OverWriteExisting)  // option to clear out previous Db setttings
                {
                    Table_Clear(DbFile, TableName, true);
                }

                // Create New SQLite DB (*Used First-Run*)
                if (!File.Exists(DbFile)) // create database file if it doen't exist
                {
                    SQLiteConnection.CreateFile(DbFile);
                }

                // Create New Table If It Does NOT Exist Yet
                bool TableExist = Table_Exists(DbFile, TableName);  //See if selected Table Exists in SQLite DB file

                if (!TableExist)  // Table DOES NOT exist in SQLite DB
                {

                    string NewTableLine = "ID INTEGER PRIMARY KEY, ListName VARCHAR, ListValue VARCHAR, Option VARCHAR, TimeStamp VARCHAR";

                    //ahk.MsgBox(NewTableLine); 

                    bool ReturnValue = Execute(DbFile, "CREATE TABLE [" + TableName + "] (" + NewTableLine + ")");  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
                }


                if (File.Exists(DbFile)) { return true; }
                return false;
            }



            #endregion



            #region === SQLite ===


            //=== Read / Write SQLite Images ===

            _AHK ahk = new _AHK(); 

            private void btnWriteFileSQLite_Click(object sender, EventArgs e)
            {
                string filePath = @"D:\_Images\_Imdb\tt0003805\tt0003805.jpg";

                WriteFileSQLite(filePath, false, true);
            }

            public void WriteFileSQLite(string FilePath, bool StoreAsZip = true, bool OverWrite = true, string DbFilePath = "")
            {
                string saveFile = DbFilePath;

                if (saveFile.Trim() == "")
                {
                    string saveDir = ahk.AppDir() + "\\Db";
                    ahk.FileCreateDir(saveDir);
                    saveFile = saveDir + "\\SQLiteFile.sqlite";
                    if (OverWrite) { ahk.FileDelete(saveFile); }
                }
                else
                {
                    if (OverWrite) { ahk.FileDelete(saveFile); }
                }


                if (!Table_Exists(saveFile, "Photos"))
                {
                    using (var connection = new SQLiteConnection("Data Source=" + saveFile + ";Version=3"))
                    using (var command = new SQLiteCommand("CREATE TABLE PHOTOS(ID INTEGER PRIMARY KEY AUTOINCREMENT, PHOTO BLOB, ImageSize VARCHAR, ImageName VARCHAR)", connection))
                    {
                        byte[] photo = ReadImageFile(FilePath);

                        connection.Open();
                        command.ExecuteNonQuery();

                        //command.Parameters.Add("@photo", DbType.Binary, 20).Value = photo;
                        //command.Parameters.Add(new SQLiteParameter("@ImageSize", photo.Length));
                        //command.Parameters.Add(new SQLiteParameter("@ImageName", ahk.FileName(FilePath)));
                        //command.ExecuteNonQuery();

                        command.CommandText = "INSERT INTO PHOTOS (PHOTO, ImageSize, ImageName) VALUES (@photo, @ImageSize, @ImageName)";
                        command.Parameters.Add("@photo", DbType.Binary, 20).Value = photo;
                        command.Parameters.Add(new SQLiteParameter("@ImageSize", photo.Length));
                        command.Parameters.Add(new SQLiteParameter("@ImageName", ahk.FileName(FilePath)));
                        command.ExecuteNonQuery();
                    }

                }
                else
                {
                    bool ConnOpen = false;
                    using (var connection = new SQLiteConnection("Data Source=" + saveFile + ";Version=3"))
                    using (var command = new SQLiteCommand())
                    {
                        if (!Table_Exists(saveFile, "Photos"))
                        {
                            command.CommandText = "CREATE TABLE PHOTOS(ID INTEGER PRIMARY KEY AUTOINCREMENT, PHOTO BLOB, ImageSize VARCHAR, ImageName VARCHAR)";
                            connection.Open(); ConnOpen = true;
                            command.ExecuteNonQuery();
                        }

                        if (StoreAsZip)  // option to compress file to zip format before storing in sqlite
                        {
                            using (Stream stream = File.Open(ahk.AppDir() + "\\Temp.zip", FileMode.Create))
                            {
                                using (ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Create, false, null))
                                {
                                    using (ZipArchiveEntry entry = archive.CreateEntry(FilePath))
                                    {
                                        StreamWriter writer = new StreamWriter(entry.Open());
                                        writer.WriteLine("Zipping " + FilePath.FileName());
                                        writer.Flush();
                                    }
                                }
                            }


                            byte[] photo = ReadImageFile(ahk.AppDir() + "\\Temp.zip");

                            command.CommandText = "INSERT INTO PHOTOS (PHOTO, ImageName) VALUES (@photo, @ImageName)";
                            command.Parameters.Add("@photo", DbType.Binary, 20).Value = photo;
                            command.Parameters.Add(new SQLiteParameter("@ImageName", ahk.FileName(FilePath)));
                            command.ExecuteNonQuery();

                            ahk.FileDelete(ahk.AppDir() + "\\Temp.zip");  // clean up after use
                        }


                        if (!ConnOpen)
                        {
                            InsertImage(saveFile, FilePath);
                        }
                        else
                        {
                            byte[] photo = ReadImageFile(FilePath);

                            command.CommandText = "INSERT INTO PHOTOS (PHOTO, ImageSize, ImageName) VALUES (@photo, @ImageSize, @ImageName)";
                            command.Parameters.Add("@photo", DbType.Binary, 20).Value = photo;
                            command.Parameters.Add(new SQLiteParameter("@ImageSize", photo.Length));
                            command.Parameters.Add(new SQLiteParameter("@ImageName", ahk.FileName(FilePath)));
                            command.ExecuteNonQuery();
                        }

                    }
                }




            }


            public void InsertImage(string DbFile, string ImagePath)
            {
                byte[] photo = ReadImageFile(ImagePath);

                using (var connection = new SQLiteConnection("Data Source=" + DbFile + ";Version=3"))
                using (var command = new SQLiteCommand("INSERT INTO PHOTOS (PHOTO, ImageSize, ImageName) VALUES (@photo, @ImageSize, @ImageName)", connection))
                {
                        connection.Open();
                        command.Parameters.Add("@photo", DbType.Binary, 20).Value = photo;
                        command.Parameters.Add(new SQLiteParameter("@ImageSize", photo.Length));
                        command.Parameters.Add(new SQLiteParameter("@ImageName", ahk.FileName(ImagePath)));
                        command.ExecuteNonQuery();
                }
            }


            public void SQLite_LinkIndex(string DbFile, string Name, string DirPath)
            {
                _Database.SQLite sqlite = new SQLite();

                string epGuideID = "";
                string imdbID = "";
                string mediaType = "TV Show";
                string forumLinks = "";
                string otherGuide = "";

                //string dirSize = ahk.DirSize(DirPath, true);
                string dirSize = ""; // ahk.FileGetSize(DirPath);


                _Sites.EpGuides ep = new _Sites.EpGuides();

                string[] files = Directory.GetFiles(DirPath, "*.url", System.IO.SearchOption.TopDirectoryOnly);
                foreach (string file in files)
                {
                    string URLPath = ahk.IniRead(file, "InternetShortcut", "URL");

                    if (URLPath.ToUpper().Contains("EPGUIDES.COM")) { epGuideID = ep.ParseEpGuideID(URLPath); }

                    if (URLPath.ToUpper().Contains("IMDB.COM"))
                    {
                        if (URLPath.Contains("?"))  // parse out unneeded link info
                        {
                            URLPath = ahk.StringSplit(URLPath, "?", 0);
                        }

                        imdbID = ep.ParseIMDbID(URLPath);
                    }
                }



                if (!sqlite.Table_Exists(DbFile, "MediaIndex"))
                {
                    using (var connection = new SQLiteConnection("Data Source=" + DbFile + ";Version=3"))
                    using (var command = new SQLiteCommand("CREATE TABLE MediaIndex(ID INTEGER PRIMARY KEY AUTOINCREMENT, [Name] VARCHAR, RootPath VARCHAR, EpGuideID VARCHAR, IMDbID VARCHAR, OtherGuide VARCHAR, ForumLinks VARCHAR, MediaType VARCHAR, DirSize VARCHAR)", connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();

                        command.CommandText = "INSERT INTO MediaIndex ([Name], RootPath, EpGuideID, IMDbID, OtherGuide, ForumLinks, MediaType, DirSize) VALUES (@Name, @RootPath, @EpGuideID, @IMDbID, @OtherGuide, @ForumLinks, @MediaType, @DirSize)";
                        command.Parameters.Add(new SQLiteParameter("@Name", Name));
                        command.Parameters.Add(new SQLiteParameter("@RootPath", DirPath));
                        command.Parameters.Add(new SQLiteParameter("@EpGuideID", epGuideID));
                        command.Parameters.Add(new SQLiteParameter("@IMDbID", imdbID));
                        command.Parameters.Add(new SQLiteParameter("@OtherGuide", otherGuide));
                        command.Parameters.Add(new SQLiteParameter("@ForumLinks", forumLinks));
                        command.Parameters.Add(new SQLiteParameter("@MediaType", mediaType));
                        command.Parameters.Add(new SQLiteParameter("@DirSize", dirSize));
                        command.ExecuteNonQuery();
                    }

                }
            }



            public Image SQLite_Image(string DbFile, string ImageName)
            {
                using (var connection = new SQLiteConnection("Data Source=" + DbFile + ";Version=3"))
                using (var command = new SQLiteCommand("SELECT PHOTO FROM PHOTOS WHERE ImageName = '" + ImageName + "'", connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();

                    //command.CommandText = "SELECT PHOTO FROM PHOTOS WHERE ID = 1";
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            byte[] buffer = GetBytes(reader, false);

                            Image LoadImage = GetImageFromBytes(buffer);


                            ImagePrimitive imagePrimitive = new ImagePrimitive();
                            imagePrimitive.Image = LoadImage;
                            imagePrimitive.MinSize = new Size(100, 85);
                            imagePrimitive.ImageScaling = ImageScaling.SizeToFit;

                            //if (ImageDisp != null)
                            //{
                            //    ImageDisp.ImageAlignment = ContentAlignment.MiddleCenter;
                            //    ImageDisp.Image = imagePrimitive.Image;
                            //}

                            return LoadImage;
                        }
                    }
                }

                return null;
            }




            public static byte[] ReadImageFile(string imageLocation)
            {
                byte[] imageData = null;
                FileInfo fileInfo = new FileInfo(imageLocation);
                long imageFileLength = fileInfo.Length;
                FileStream fs = new FileStream(imageLocation, FileMode.Open, FileAccess.Read);
                BinaryReader br = new BinaryReader(fs);
                imageData = br.ReadBytes((int)imageFileLength);
                return imageData;
            }

            static byte[] GetBytes(SQLiteDataReader reader, bool StoredAsZip = true)
            {
                const int CHUNK_SIZE = 2 * 1024;
                byte[] buffer = new byte[CHUNK_SIZE];
                long bytesRead;
                long fieldOffset = 0;
                using (MemoryStream stream = new MemoryStream())
                {
                    while ((bytesRead = reader.GetBytes(0, fieldOffset, buffer, 0, buffer.Length)) > 0)
                    {
                        stream.Write(buffer, 0, (int)bytesRead);
                        fieldOffset += bytesRead;
                    }

                    if (StoredAsZip)
                    {
                        ZipArchive z = new ZipArchive(stream);
                        List<ZipArchiveEntry> allEntries = z.Entries.ToList();
                        foreach (ZipArchiveEntry f in allEntries)
                        {
                            var file = f.FullName;
                        }
                    }


                    return stream.ToArray();
                }
            }

            private void btnReadFileSQLite_Click(object sender, EventArgs e)
            {
                ReadFileSQLite("", true);
            }

            public void ReadFileSQLite(string DbFilePath = "", bool StoredAsZip = true, RadButton ImageDisp = null)
            {
                string dbPath = DbFilePath;
                if (dbPath.Trim() == "")
                {
                    dbPath = ahk.AppDir() + "\\Db\\SQLiteFile.sqlite";
                }

                //string saveFile = ahk.AppDir() + "\\Db\\SQLiteFile.sqlite";

                if (!StoredAsZip) // return original file stored as bytes in sqlite
                {
                    using (var connection = new SQLiteConnection("Data Source=" + dbPath + ";Version=3"))
                    using (var command = new SQLiteCommand("SELECT PHOTO FROM PHOTOS WHERE ID = 1", connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();

                        command.CommandText = "SELECT PHOTO FROM PHOTOS WHERE ID = 1";
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                byte[] buffer = GetBytes(reader, StoredAsZip);

                                Image LoadImage = GetImageFromBytes(buffer);


                                ImagePrimitive imagePrimitive = new ImagePrimitive();
                                imagePrimitive.Image = LoadImage;
                                imagePrimitive.MinSize = new Size(100, 85);
                                imagePrimitive.ImageScaling = ImageScaling.SizeToFit;

                                if (ImageDisp != null)
                                {
                                    ImageDisp.ImageAlignment = ContentAlignment.MiddleCenter;
                                    ImageDisp.Image = imagePrimitive.Image;
                                }
                            }
                        }
                    }
                }
                else  // return zip file from sqlite db
                {
                    using (var connection = new SQLiteConnection("Data Source=" + dbPath + ";Version=3"))
                    using (var command = new SQLiteCommand("SELECT PHOTO FROM PHOTOS WHERE ID = 1", connection))
                    {
                        connection.Open();
                        command.ExecuteNonQuery();

                        command.CommandText = "SELECT PHOTO FROM PHOTOS WHERE ID = 1";
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                byte[] buffer = GetBytes(reader, StoredAsZip);

                                //string LoadZipPath = GetZipFromBytes(buffer, true);

                                Image savedImg = GetImageFromBytes(buffer);

                                if (ImageDisp != null)
                                {
                                    ImageDisp.Image = savedImg;
                                    ImageDisp.ImageAlignment = ContentAlignment.MiddleCenter;
                                }
                            }
                        }
                    }
                }

            }

            public class FileHelper
            {
                public int ID { get; set; }
                public int CompressedSize { get; set; }
                public int UncompressedSize { get; set; }
                public string FileNameInZip { get; set; }
                public string IconPath { get; set; }
                public byte[] Data { get; set; }
            }

            private Image GetImageFromBytes(object data)
            {
                Image result = null;
                MemoryStream stream = null;

                try
                {
                    if (data != null &&
                        typeof(byte[]).IsAssignableFrom(data.GetType()))
                    {
                        byte[] bytes = null;
                        bytes = (byte[])data;
                        if (bytes.Length > 0)
                        {
                            int count;
                            int start;
                            count = bytes.Length;
                            start = 0;


                            stream = new MemoryStream(bytes, start, count);
                            result = Image.FromStream(stream);
                        }
                    }
                }
                catch
                {
                    result = null;
                }
                finally
                {
                    if (stream != null)
                        stream.Close();
                }
                return result;
            }


            #endregion






            #region === v1 Connect / Disconnect === Done

            /// <summary>
            /// Creates Sqlite Connection to .sqlite Database File
            /// </summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            public SQLiteConnection _Connect(string DbFile) // connect to SQLite DB file path - returns connection data
            {
                //// EXAMPLE USE:
                ////===================================
                //// Connect to the DB
                ////===================================
                ////SQLiteConnection m_dbConnection = ConnectToDB(NewDBFile); // connect to SQLite DB file path - returns connection data

                SQLiteConnection m_dbConnection;
                m_dbConnection = new SQLiteConnection("Data Source=" + DbFile + ";Version=3;");
                m_dbConnection.Open();

                return m_dbConnection;
            }

            /// <summary>
            /// Disconnects Sqlite Session To Free File For Other Use
            /// </summary>
            /// <param name="db">SQLiteConnection To Disconnect</param>
            public void _Disconnect(SQLiteConnection db = null) // disconnects from local DB connection (opens file up to edit in other locations)
            {
                SQLiteConnection.ClearAllPools();

                if (db != null)
                {
                    db.Close(); //disconnect from database file
                }

                GC.Collect(); //empty garbage files
                GC.WaitForPendingFinalizers(); //wait for those garbage files to finish before proceeding
            }


            #endregion

            #region === v1 Write Commands ===  Done

            /// <summary>
            /// Execute a SQLite Command on a .sqlite Database File. Returns True If > 1 Rows Changed
            /// </summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="SQLiteCommand">SQLite Command To Execute</param>
            /// <param name="Debug">Enables Additional MessageBox For TroubleShooting</param>
            public bool _Execute(string DbFile, string SQLiteCommand, bool Debug = false)  // executes sqlite command on sqlite db file
            {
                _AHK ahk = new _AHK();
                if (!File.Exists(DbFile))  // ensure file path exists
                {
                    ahk.MsgBox(DbFile + " Not Found | .sqlite File Must Be Defined | Unable To Execute Command"); return false;
                }

                // Connect to the SQLite DB File
                SQLiteConnection m_dbConnection = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data

                SQLiteCommand command = new SQLiteCommand(SQLiteCommand, m_dbConnection); // create command

                int RowsChanged = 0;
                try
                {
                    RowsChanged = command.ExecuteNonQuery();             // execute command
                }
                catch (Exception ex)
                {
                    string error = ex.Message;

                    if (error.Contains("already exists"))  // error msg saying it couldn't create table bc it exists - ignore
                    {
                        return true;
                    }

                    if (Debug == true) { MessageBox.Show(SQLiteCommand + Environment.NewLine + Environment.NewLine + Environment.NewLine + error); }

                    if (error.Contains("database is locked"))
                    {
                        MessageBox.Show("DB File Currently LOCKED : Unable To Write To " + ahk.FileName(DbFile));
                    }

                    return false;
                }

                _DisconnectFromDB(m_dbConnection);  // free up db for other use

                if (RowsChanged >= 1) { return true; }   // row updated successfully
                if (RowsChanged == 0) { return false; }  // no rows changed
                return false;
            }

            /// <summary>
            /// Attempt to UPDATE Existing Database Entry. If Update Fails, Insert New Entry
            /// </summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="UpdateLine">SQLite Command To Update Existing Table Entry</param>
            /// <param name="InsertLine">If Update Fails, Line to Insert New SQLite Db Entry</param>
            /// <param name="Debug">Enables Additional MessageBox For TroubleShooting</param>
            public bool _Update_Insert(string DbFile, string UpdateLine, string InsertLine, bool Debug = false)  // attempts update line first, then inserts if update fails
            {
                //setup the function params table by writing the function and file name fields first
                //string UpdateLine = "UPDATE [FunctionParams] set LineNum = '" + LineCount.ToString() + "', Comments = '" + Comments + "', TimeStamp = '" + DateTime.Now + "', Type = '" + REgionName + "', FunctionLine = '" + CodeReturn + "', FunctionText = '" + FunctionText + "', FilePath = '" + FilePath + "', Parent = '" + Parent + "', FID = '" + FID.ToString() + "' WHERE Function = '" + FunctionName + "' AND FileName = '" + FileName + "'";
                //string InsertLine = "INSERT into [FunctionParams] (FileName, Function) VALUES ('" + FileName + "', '" + FunctionName + "')";

                //if (conn.State == ConnectionState.Open) { conn.Close(); } //close existing connection if needed
                bool Updated = _Execute(DbFile, UpdateLine, Debug);  // Update Table
                if (!Updated) { Updated = _Execute(DbFile, InsertLine, Debug); }  // insert into a Table

                return Updated;
            }


            /// <summary>
            /// Write a Collection of SQLite Insert Commands In Batch - Faster Than Individual Writes to Database
            /// </summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="Insert_Lines">List of INSERT Commands To Write In Batch</param>
            public void _Batch_Insert(string DbFile, List<string> Insert_Lines)
            {
                _AHK ahk = new _AHK();
                if (!File.Exists(DbFile))  // ensure file path exists
                {
                    ahk.MsgBox(DbFile + " Not Found - Unable to Batch Insert"); return;
                }

                SQLiteConnection conn = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data
                                                              //conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        foreach (string line in Insert_Lines)
                        {
                            cmd.CommandText = line;
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();  // write all commands to table
                    }
                }

                conn.Close();
            }


            /// <summary>
            /// Search and Replace Text In SQLite Column Contents
            /// </summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Read</param>
            /// <param name="ColNameOrPos">Column Name or Column Postion #</param>
            /// <param name="OldText">Text in Column To Replace</param>
            /// <param name="NewText">New Text To Replace In Column</param>
            public bool _Replace_Text(string DbFile, string TableName, object ColNameOrPos, string OldText, string NewText)
            {
                _AHK ahk = new _AHK();
                string ColumnName = "";
                if (ColNameOrPos.GetType().ToString() == "System.String")  // user passed in Column Name
                {
                    ColumnName = ColNameOrPos.ToString();
                }

                if (ColNameOrPos.GetType().ToString() == "System.Int32")  // user passed in Column #
                {
                    ColumnName = _Column_Name(DbFile, TableName, ahk.ToInt(ColNameOrPos));  // look up column name by the column #
                }

                // remove brackets if passed in
                string Col = ahk.StringReplace(ColumnName, "[");
                Col = ahk.StringReplace(Col, "]");

                // remove brackets if passed in
                string tableName = ahk.StringReplace(TableName, "[");
                tableName = ahk.StringReplace(tableName, "]");

                // assemble sqlite update string 
                string UpdateLine = @"UPDATE [" + TableName + "] SET [" + Col + "] = REPLACE([" + Col + "], '" + OldText + "', '" + NewText + "')";

                bool Updated = _Execute(DbFile, UpdateLine);  // Update Table

                return Updated;
            }


            #endregion

            #region === v1 Return from SQLite ===

            // Return info from SQLite
            public DataTable _ReturnDataTable(string DbFile, string SQLiteCommand)  // returns datatable from sqlite search command in sqlite db file (same as SQLiteSearch)
            {
                SQLiteConnection con = new SQLiteConnection("data source=" + DbFile);
                string SearchCommand = SQLiteCommand;
                //if (WhereClause != "") { SearchCommand = SearchCommand + " " + WhereClause; } //add where clause if provided
                SQLiteCommand cmd = new SQLiteCommand(SearchCommand, con);
                con.Open();
                DataTable DT = new DataTable();
                cmd = con.CreateCommand();
                //cmd.CommandText = string.Format("SELECT * FROM {0}", TableName);
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                try
                {
                    adapter.Fill(DT);
                }
                catch
                {
                    con.Close();
                    return null;
                }
                con.Close();
                //DT.TableName = TableName;
                return DT;
            }
            public DataTable _SQLiteSearch(string DbFile, string SQLiteCommand, bool Debug = false)  // returns datatable from sqlite search command in sqlite db file (same as ReturnDataTable)
            {
                _AHK ahk = new _AHK();
                SQLiteConnection con = new SQLiteConnection("data source=" + DbFile + "; datetimeformat=ISO8601");
                //datetimeformat=CurrentCulture or Ticks or ISO8601

                //string SearchCommand = "SELECT * FROM " + TableName;
                SQLiteCommand cmd = new SQLiteCommand(SQLiteCommand, con);
                con.Open();
                DataTable DT = new DataTable();
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                try
                {
                    adapter.Fill(DT);
                }
                catch (SQLiteException ex)
                {
                    if (Debug) { ahk.MsgBox(ex.ToString()); }
                    con.Close();
                    return null;
                }

                con.Close();
                return DT;
            }
            public DataTable _GetDataTable(string DbFile, string SQLiteCommand, bool Debug = false)  // returns datatable from sqlite search command in sqlite db file (same as ReturnDataTable)
            {
                _AHK ahk = new _AHK();
                SQLiteConnection con = new SQLiteConnection("data source=" + DbFile + "; datetimeformat=ISO8601");
                //datetimeformat=CurrentCulture or Ticks or ISO8601

                //string SearchCommand = "SELECT * FROM " + TableName;
                SQLiteCommand cmd = new SQLiteCommand(SQLiteCommand, con);
                con.Open();
                DataTable DT = new DataTable();
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                try
                {
                    adapter.Fill(DT);
                }
                catch (SQLiteException ex)
                {
                    if (Debug) { ahk.MsgBox(ex.ToString()); }
                    con.Close();
                    return null;
                }

                con.Close();
                return DT;
            }

            public DataTable _ReturnTags(string DbFile, string SQLiteQuery) //more dynamic verion of getdatatable for tags (not in use?)
            {
                SQLiteConnection con = new SQLiteConnection("data source=" + DbFile);
                SQLiteCommand cmd = new SQLiteCommand(SQLiteQuery, con);
                con.Open();
                DataTable DT = new DataTable();
                cmd = con.CreateCommand();
                cmd.CommandText = SQLiteQuery;
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                adapter.Fill(DT);
                con.Close();
                DT.TableName = "Tags";
                return DT;
            }

            public SQLiteDataReader _ReturnSQLite(string SQLLine, SQLiteConnection db)  //returns data from sqlite db to parse return values
            {
                ////// EXAMPLE USE: 
                //////=======================================================================================================
                ////// Connect to SQLite DB - Request Data from DB and Return It - Loop through Each Row then ____
                //////=======================================================================================================

                ////SQLiteConnection m_dbConnection = ConnectToDB(NewDBFile); // connect to SQLite DB file path - returns connection data

                ////SQLiteDataReader reader = ReturnSQLite("select * from filelist order by score desc", m_dbConnection);  // request data from DB

                ////while (reader.Read())                                                                      // loop through each row returned from select 
                ////    Console.WriteLine("File: " + reader["filepath"] + "\tScore: " + reader["score"]);            



                //=====================================================
                // read from the data table - return data to reader 
                //=====================================================
                //string SQLLine = "select * from filelist order by score desc";
                SQLiteCommand command = new SQLiteCommand(SQLLine, db);
                SQLiteDataReader reader = null;

                try
                {
                    reader = command.ExecuteReader();
                }
                catch { }

                //while (reader.Read()) // loop through each row returned from select 
                //    Console.WriteLine("File: " + reader["filepath"] + "\tScore: " + reader["score"]);

                //DisconnectFromDB(db);  // free up db for other use

                return reader;
            }
            public int _ReturnLastINT(string TableName, string FieldName, string DataBaseFile)  // returns last int value from a field in sqlite table
            {
                SQLiteConnection m_dbConnection = _ConnectToDB(DataBaseFile); // connect to SQLite DB file path - returns connection data

                SQLiteDataReader reader = _ReturnSQLite("select [" + FieldName + "] from " + TableName + " order by " + FieldName + " desc limit 1", m_dbConnection);  // request data from DB

                string LastFIDString = "";

                while (reader.Read())                                                                      // loop through each row returned from select 
                    LastFIDString = reader[FieldName].ToString();

                int LastFID = Int32.Parse(LastFIDString); //convert string to int

                return LastFID;
            }


            public string _ReturnString(string DbFile, string SQLiteCommand)  // return a string value from a sqlite search (first column / first row returned)
            {
                //SQLiteCommand = "select wCount from WordCount where word = 'INI'"; 

                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite._ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data

                // Connect to SQLite DB - Request Data from DB and Return It - Loop through Each Row then ____

                SQLiteDataReader reader = lite._ReturnSQLite(SQLiteCommand, m_dbConnection);  // request data from DB

                string value = "";

                while (reader.Read())    // loop through each row returned from select 
                {
                    value = reader[0].ToString();
                }

                return value;
            }

            public int _ReturnInt(string DbFile, string SQLiteCommand)  // return a int value from a sqlite search (first column / first row returned)
            {
                _AHK ahk = new _AHK();
                //int Value = sqlite.ReturnInt(DbFile, "select wCount from WordCount where word = 'INI'"); 

                //SQLiteCommand = "select wCount from WordCount where word = 'INI'"; 

                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite._ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data

                // Connect to SQLite DB - Request Data from DB and Return It - Loop through Each Row then ____

                SQLiteDataReader reader = lite._ReturnSQLite(SQLiteCommand, m_dbConnection);  // request data from DB

                string value = "";
                int iValue = 0;

                if (reader != null)
                {
                    while (reader.Read())    // loop through each row returned from select 
                    {
                        value = reader[0].ToString();
                    }

                    iValue = ahk.ToInt(value);
                }

                return iValue;
            }


            #endregion

            #region === v1 DataGrid === Done


            /// <summary>
            /// Load Values from sqlite Database and Populate DataGridView Control. Pass just the TableName to load all contents of Table, or populate Or_SQL_Command for specific search criteria
            /// </summary>
            /// <param name="dv">DataGridView Control To Populate</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table To Return All Contents From to Grid</param>
            /// <param name="Or_SQL_Command">SQLite Command That OverRides Previous TableName Search</param>
            /// <param name="AutoSizeColumns">Option To Resize DataGridView Column Width After Populating</param>
            public int _Grid_Load(DataGridView dv, string DbFile = "Settings.sqlite", string TableName = "Settings", string Or_SQL_Command = "", bool AutoSizeColumns = true)  // pass in table name and db connection, autopopulates dataGridView1
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                if (!File.Exists(DbFile)) { ahk.MsgBox(DbFile + " Not Found"); return -1; } // make sure database file exists


                string command = "Select * From [" + TableName + "];";  // selects all contents from sqlite table

                if (Or_SQL_Command != "")  // option for user to pass in full sqlite command instead of returning all of one table
                {
                    command = Or_SQL_Command;
                }

                SQLiteConnection db = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data

                try
                {
                    DataSet ds = new DataSet();
                    var da = new SQLiteDataAdapter(command, db);  // search SQLite DB
                    da.Fill(ds);

                    // assign the DataGridView Name to Populate
                    dv.DataSource = ds.Tables[0].DefaultView;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("SQL Exception Catch Here: " + ex.ToString());
                }

                dv.AutoGenerateColumns = true;


                if (AutoSizeColumns)
                {
                    // Resize the master DataGridView columns to fit the newly loaded data.
                    dv.AutoResizeColumns();

                    // Configure the details DataGridView so that its columns automatically 
                    // adjust their widths when the data changes.
                    dv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                }

                _DisconnectFromDB(db);  // free up db for other use

                // return grid row count
                return dv.RowCount;
            }


            /// <summary>
            /// Save Values in DataGridView Control to .sqlite Database File. Creates new Sqlite Table with using Column Names if Table does not already exist
            /// </summary>
            /// <param name="dv">DataGridView Control To Save</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table To Save Contents of Grid to</param>
            /// <param name="OverWrite">Option To Clear Previous Values in Table Before Saving</param>
            /// <param name="SkipColumnNum">Column # in Grid To Skip Writing To Database (usually the Index ID Field)</param>
            public void _Grid_Save(DataGridView dv, string DbFile = "Settings.sqlite", string TableName = "GridValues", bool OverWrite = true, int SkipColumnNum = -1)
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                bool SkipColumn = false;
                if (SkipColumnNum != -1) { SkipColumn = true; }

                // check to see if there are any columns to populate list first
                int colCount = 0;
                try { colCount = dv.Rows[0].Cells.Count; }
                catch
                {
                    ahk.MsgBox("No Columns Populated In DataGridView To Save");
                    return;
                }

                // Create List of Grid Column Names
                List<string> Grid_Columns = new List<string>();
                string ColumnName = "";
                int columnCount = dv.Rows[0].Cells.Count;
                for (int i = 0; i < columnCount; i++)
                {
                    ColumnName = dv.Columns[i].HeaderText;
                    Grid_Columns.Add(ColumnName);
                }

                // check to see if this table already exists in sqlite db
                bool TableFound = _Table_Exists(DbFile, TableName);

                if (OverWrite) // option to overwrite existing table if it exists already
                {
                    if (TableFound)
                    {
                        bool Cleared = _Table_Clear(DbFile, TableName, true);
                    }
                }

                int ColNum = 0;

                if (!TableFound)  // Create new SQLITE Table using DataGridView Column Names if Table Does Not Exist
                {
                    string NewTableLine = "ID INTEGER PRIMARY KEY";

                    foreach (string col in Grid_Columns)
                    {
                        if (SkipColumn) { if (ColNum == SkipColumnNum) { ColNum++; continue; } } // option to skip writing specific column (usually id field) into database 
                        NewTableLine = NewTableLine + ", " + col + " VARCHAR";
                    }

                    bool Created = _Table_New(DbFile, TableName, NewTableLine);
                }


                // Create Insert Command (Grid Column Names = Sqlite Table Column Names)
                string Command = "Insert into [" + TableName + "] (";
                string VarList = "";
                ColNum = 0;
                foreach (string col in Grid_Columns)
                {
                    if (SkipColumn) { if (ColNum == SkipColumnNum) { ColNum++; continue; } } // option to skip writing specific column (usually id field) into database 
                    if (VarList != "") { VarList = VarList + "," + col; }
                    if (VarList == "") { VarList = col; }
                    ColNum++;
                }
                Command = Command + VarList + ") Values (";


                // Populate Insert Command Values
                columnCount = dv.Rows[0].Cells.Count;
                List<string> InsertList = new List<string>();
                string RowValues = "";
                for (int i = 0; i < dv.Rows.Count - 1; i++) // loop through each row
                {
                    RowValues = "";

                    for (int c = 0; c < columnCount; c++)  // loop through each column
                    {
                        if (SkipColumn) { if (c == SkipColumnNum) { ColNum++; continue; } } // option to skip writing specific column (usually id field) into database 

                        //bool ColVisible = dataGridView1.Rows[i].Cells[c].Visible;
                        //ahk.MsgBox("Col " + c + " = " + ColVisible.ToString()); 

                        string value = dv.Rows[i].Cells[c].Value.ToString();
                        if (RowValues != "") { RowValues = RowValues + ", '" + value + "'"; }
                        if (RowValues == "") { RowValues = "'" + value + "'"; continue; }
                    }

                    string SQLCommand = Command + RowValues + ");";
                    InsertList.Add(SQLCommand);
                }

                _Batch_Insert(DbFile, InsertList);

                ahk.MsgBox("Grid Saved");
            }


            #endregion

            #region === v1 FileIndex (Dir2Db) ===

            /// <summary>
            /// Save Contents of Directory to .sqlite Table
            /// </summary>
            /// <param name="DirPath">Directory Path / Drive to Index</param>
            /// <param name="FilePattern">Option to Only Add Specific File Pattern. Default = All Files (*.*)</param>
            /// <param name="Recurse">Option to Search Folders Underneath DirPath (Default = True)</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To Settings.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To. Defaults To 'FileIndex'</param>
            /// <param name="OverWriteTable">Option to OverWrite Existing FileIndex Table (Default = True)</param>
            public bool _FileIndex_Create(string DirPath, string FilePattern = "*.*", bool Recurse = true, string DbFile = "Settings.sqlite", string TableName = "FileIndex", bool OverWriteTable = true, bool CreateIndexTable = true)  // creates a "File Index" of a directory, storing file info to sqlite database table (v2)
            {
                _AHK ahk = new _AHK();
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }
                if (TableName == "") { TableName = "FileIndex"; }
                if (FilePattern == "") { FilePattern = "*.*"; }

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
                bool TableExist = _Table_Exists(DbFile, TableName);  //See if selected Table Exists in SQLite DB file

                if (TableExist)
                {
                    if (OverWriteTable)  // option to clear table contents before writing file index
                    {
                        _Table_Clear(DbFile, TableName, true);  // clear out existing sqlite table contents
                    }
                }

                if (!TableExist)  // Table DOES NOT exist in SQLite DB
                {
                    string NewTableLine = "FID INTEGER PRIMARY KEY, FileFlag BOOL, FilePath VARCHAR, FileName VARCHAR, DirName VARCHAR, DirPath VARCHAR, FileExt VARCHAR, FileSize VARCHAR, FileSizeBytes VARCHAR, CreationTime VARCHAR, LastAccessTime VARCHAR, LastWriteTime VARCHAR, IsReadOnly BOOL, Attributes VARCHAR, FileExists BOOL, FileAction VARCHAR, Tags VARCHAR, Notes VARCHAR";
                    bool Created = _Execute(DbFile, "CREATE TABLE [" + TableName + "] (" + NewTableLine + ")");  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
                    if (!_Table_Exists(DbFile, TableName)) { ahk.MsgBox("Error Creating [" + TableName + "] in\n" + DbFile); return false; } // if the table didn't create - stop with error
                }


                if (CreateIndexTable)
                {
                    //== create directory index for storing multiple drives / folders -- tracks last modified etc ===
                    TableExist = _DoesTableExist(DbFile, IndexTableName);  //See if selected Table Exists in SQLite DB file
                    if (!TableExist)  // Table DOES NOT exist in SQLite DB
                    {
                        string NewTableLine = "FID INTEGER PRIMARY KEY, Enabled BOOL, TableName VARCHAR, Label VARCHAR, DirPath VARCHAR, FileCount VARCHAR, CaptureTime VARCHAR, LastUpdated VARCHAR";
                        _Execute(DbFile, "CREATE TABLE " + IndexTableName + " (" + NewTableLine + ")");  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
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

                var files = _GetAllAccessibleFiles(DirPath);

                //var files = new List<string>();

                //try
                //{
                //    files.AddRange(Directory.GetFiles(DirPath, FilePattern, recurse));
                //    foreach (var directory in Directory.GetDirectories(DirPath))
                //        files.AddRange(Directory.GetFiles(directory, FilePattern));
                //}
                //catch (UnauthorizedAccessException) { }


                int FileCount = 0;

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
                                    string FileName = fileinfo.Name.ToString();

                                    string FileSizeBytes = fileinfo.Length.ToString();
                                    string FileSize = ahk.FormatBytes(fileinfo.Length);  // convert bytes to Text representation (adds kb/mb/tb to return)

                                    string LastWriteTime = fileinfo.LastWriteTime.ToString();
                                    string LastAccessTime = fileinfo.LastAccessTime.ToString();
                                    string CreationTime = fileinfo.CreationTime.ToString();

                                    bool IsReadOnly = fileinfo.IsReadOnly;
                                    string FileExt = fileinfo.Extension.ToString();
                                    FileExt = ahk.StringReplace(FileExt, "."); // remove period from file ext

                                    bool FileExists = fileinfo.Exists;
                                    string DirName = fileinfo.DirectoryName.ToString();
                                    string dirPath = fileinfo.Directory.ToString();

                                    string Attributes = fileinfo.Attributes.ToString();
                                    string FileAction = ""; // variable used to indicate a file is queued to be copied to another location 
                                    string FilePath = file;
                                    bool FileFlag = false;

                                    //DirName = ahk.FixSpecialChars(DirName); //remove invalid characters before writing
                                    //DirPath = ahk.FixSpecialChars(DirPath); //remove invalid characters before writing
                                    //FileName = ahk.FixSpecialChars(FileName); //remove invalid characters before writing
                                    //FilePath = ahk.FixSpecialChars(FilePath); //remove invalid characters before writing

                                    string[] paths = DirName.Split('\\'); // split the directory path to get the name
                                    DirName = paths[paths.Length - 1]; //returns last folder name in string

                                    //FilePath = FilePath.Replace("'", "''");
                                    //DirPath = DirPath.Replace("'", "''");
                                    //FileName = FileName.Replace("'", "''");
                                    //DirName = DirName.Replace("'", "''");

                                    //FilePath = ahk.StringReplace(FilePath, "'", "''");
                                    //DirPath = ahk.StringReplace(DirPath, "'", "''");
                                    //DirName = ahk.StringReplace(DirName, "'", "''");
                                    //FileName = ahk.StringReplace(FileName, "'", "''");

                                    FilePath = _CleanInput(FilePath);
                                    dirPath = _CleanInput(dirPath);
                                    FileName = _CleanInput(FileName);
                                    DirName = _CleanInput(DirName);

                                    cmd.CommandText = "INSERT into [" + TableName + "] (FileFlag, FilePath, FileName, DirName, DirPath, FileExt, FileSize, FileSizeBytes, CreationTime, LastAccessTime, LastWriteTime, IsReadOnly, Attributes, FileExists, FileAction, Tags, Notes) values ('" + FileFlag + "', '" + FilePath + "', '" + FileName + "', '" + DirName + "', '" + dirPath + "', '" + FileExt + "', '" + FileSize + "', '" + FileSizeBytes + "', '" + CreationTime + "', '" + LastAccessTime + "', '" + LastWriteTime + "', '" + IsReadOnly + "', '" + Attributes + "', '" + FileExists + "', '" + FileAction + "', '" + DirName + "', '" + "" + "');";
                                    cmd.ExecuteNonQuery();
                                    FileCount++;

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

                            transaction.Commit();  //write collection of insert statements
                        }
                    }

                    conn.Close();
                }


                if (CreateIndexTable)
                {
                    // create index entry / update existing entry if found
                    string Label = ahk.DirName(DirPath);
                    string Index_InsertLine = "Insert Into [" + IndexTableName + "] (Enabled, TableName, Label, DirPath, FileCount, CaptureTime, LastUpdated) VALUES ('true', '" + TableName + "', '" + Label + "', '" + DirPath + "', '" + FileCount + "', '" + stopwatch.Elapsed.TotalSeconds + "', '" + DateTime.Now + "')";
                    string Index_UpdateLine = "Update [" + IndexTableName + "] Set Enabled = 'true', TableName = '" + TableName + "', Label = '" + Label + "', DirPath = '" + DirPath + "', FileCount = '" + FileCount + "', CaptureTime = '" + stopwatch.Elapsed.TotalSeconds + "', LastUpdated = '" + DateTime.Now + "' where DirPath = '" + DirPath + "'";
                    _Update_Insert(DbFile, Index_UpdateLine, Index_InsertLine, false);
                }


                //Console.WriteLine("{0} seconds with one transaction.", stopwatch.Elapsed.TotalSeconds);
                return true;
            }

            public static List<string> _GetAllAccessibleFiles(string rootPath, List<string> alreadyFound = null)
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
                            alreadyFound = _GetAllAccessibleFiles(dir.FullName, alreadyFound);
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


            public string _CleanInput(string strIn)
            {
                // Replace invalid characters with empty strings.
                try
                {
                    return Regex.Replace(strIn, @"[^\w\.@-]", "",
                                         RegexOptions.None, TimeSpan.FromSeconds(1.5));
                }
                // If we timeout when replacing invalid characters, 
                // we should return Empty.
                catch (RegexMatchTimeoutException)
                {
                    return String.Empty;
                }
            }

            /// <summary>
            /// Load FileIndex Table from sqlite Database and Populate DataGridView Control
            /// </summary>
            /// <param name="dv">DataGridView Control To Populate</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table To Return All Contents From to Grid</param>
            /// <param name="Or_SQL_Command">SQLite Command That OverRides Previous TableName Search</param>
            /// <param name="AutoSizeColumns">Option To Resize DataGridView Column Width After Populating</param>
            public int _FileIndex_Load(DataGridView dv, string DbFile = "Settings.sqlite", string TableName = "FileIndex", string Or_SQL_Command = "", bool AutoSizeColumns = true)  // creates a "File Index" of a directory, storing file info to sqlite database table (v2)
            {
                _AHK ahk = new _AHK();
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }
                if (TableName == "") { TableName = "FileIndex"; }

                int RowCount = _Grid_Load(dv, DbFile, TableName, Or_SQL_Command, AutoSizeColumns);

                return RowCount;
            }




            #region === v1 Db Access ===


            public struct fileIndex
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


            public List<fileIndex> _Return_FileIndex_List(string DbFile = "Settings.sqlite", string TableName = "FileIndex", string Or_SQL_Command = "", bool Debug = false)
            {
                _AHK ahk = new _AHK();
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }
                if (TableName == "") { TableName = "FileIndex"; }

                string SelectLine = "Select * From [" + TableName + "]";

                if (Or_SQL_Command != "") { SelectLine = Or_SQL_Command; }

                //if (WhereClause != "")
                //{
                //    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                //    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                //}
                DataTable ReturnTable = _GetDataTable(DbFile, SelectLine, Debug);
                List<fileIndex> ReturnList = new List<fileIndex>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        fileIndex returnObject = new fileIndex();

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
            public fileIndex _FileIndex_Return(string Where = "[File] = ", string DbFile = "Settings.sqlite", bool Debug = false)
            {
                _AHK ahk = new _AHK();
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                string SelectLine = "Select [FID], [FileFlag], [FilePath], [FileName], [DirName], [DirPath], [FileExt], [FileSize], [FileSizeBytes], [CreationTime], [LastAccessTime], [LastWriteTime], [IsReadOnly], [Attributes], [FileExists], [FileAction], [Tags], [Notes] From [FileIndex] WHERE " + Where;
                fileIndex returnObject = new fileIndex();

                DataTable ReturnTable = _GetDataTable(DbFile, SelectLine, Debug);
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
            public bool _FileIndex_Insert(fileIndex inObject, string DbFile = "Settings.sqlite", bool Debug = false)
            {
                _AHK ahk = new _AHK();
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                string InsertLine = "Insert Into [FileIndex] (FID, FileFlag, FilePath, FileName, DirName, DirPath, FileExt, FileSize, FileSizeBytes, CreationTime, LastAccessTime, LastWriteTime, IsReadOnly, Attributes, FileExists, FileAction, Tags, Notes) values ('" + inObject.FID + "', '" + inObject.FileFlag + "', '" + inObject.FilePath + "', '" + inObject.FileName + "', '" + inObject.DirName + "', '" + inObject.DirPath + "', '" + inObject.FileExt + "', '" + inObject.FileSize + "', '" + inObject.FileSizeBytes + "', '" + inObject.CreationTime + "', '" + inObject.LastAccessTime + "', '" + inObject.LastWriteTime + "', '" + inObject.IsReadOnly + "', '" + inObject.Attributes + "', '" + inObject.FileExists + "', '" + inObject.FileAction + "', '" + inObject.Tags + "', '" + inObject.Notes + "')";
                bool Inserted = _Execute(DbFile, InsertLine, Debug);
                if (Debug) { ahk.MsgBox("Inserted Into [FileIndex] = " + Inserted.ToString()); }
                return Inserted;
            }
            public bool _FileIndex_Update(fileIndex inObject, string DbFile = "Settings.sqlite", bool Debug = false)
            {
                _AHK ahk = new _AHK();
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

                bool Updated = _Execute(DbFile, UpdateLine, Debug);
                if (Debug) { ahk.MsgBox("Updated [FileIndex] = " + Updated.ToString()); }
                return Updated;
            }



            public fileIndex _Return_Object_From_FunctionGrid(DataGridView dv, int RowNum = -1)  // from grid
            {
                _GridControl grid = new _GridControl();

                fileIndex returnObject = new fileIndex();
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


            public DataTable _Return_FileIndex_DataTable(string DbFile = "Settings.sqlite", string TableName = "FileIndex", string WhereClause = "", bool Debug = false)
            {
                _AHK ahk = new _AHK();
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }
                if (TableName == "") { TableName = "FileIndex"; }

                string SelectLine = "Select * From [" + TableName + "]";

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }

                DataTable ReturnTable = _GetDataTable(DbFile, SelectLine, Debug);


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
                        fileIndex returnObject = new fileIndex();

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



            public bool _Update_FileIndex(string DbFile = "Settings.sqlite", bool Debug = false, string FileFlag = "", string FilePath = "", string FileName = "", string DirName = "", string DirPath = "", string FileExt = "", string FileSize = "", string FileSizeBytes = "", string CreationTime = "", string LastAccessTime = "", string LastWriteTime = "", string IsReadOnly = "", string Attributes = "", string FileExists = "", string FileAction = "", string Tags = "", string Notes = "")
            {
                _AHK ahk = new _AHK();
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                string UpdateLine = "Update [FileIndex] set ";
                if (FileFlag != "") { UpdateLine = UpdateLine + "FileFlag = '" + FileFlag + "',"; }
                if (FileFlag != "") { UpdateLine = UpdateLine + "FilePath = '" + FilePath + "',"; }
                if (FileFlag != "") { UpdateLine = UpdateLine + "FileName = '" + FileName + "',"; }

                //List<string> Options = new List<string> { "FileFlag","FileName","explorer"	};

                //FileFlag = '" + FileFlag + "', FilePath = '" + FilePath + "', FileName = '" + FileName + "', DirName = '" + DirName + "', DirPath = '" + DirPath + "', FileExt = '" + FileExt + "', FileSize = '" + FileSize + "', FileSizeBytes = '" + FileSizeBytes + "', CreationTime = '" + CreationTime + "', LastAccessTime = '" + LastAccessTime + "', LastWriteTime = '" + LastWriteTime + "', IsReadOnly = '" + IsReadOnly + "', Attributes = '" + Attributes + "', FileExists = '" + FileExists + "', FileAction = '" + FileAction + "', Tags = '" + Tags + "', Notes = '" + Notes + "' WHERE FilePath = '" + FilePath + "'";



                bool Updated = _Execute(DbFile, UpdateLine, Debug);
                if (Debug) { ahk.MsgBox("Updated [FileIndex] = " + Updated.ToString()); }
                return Updated;
            }
            public bool _Update_FileIndex_FromObject(fileIndex inObject, string DbFile = "Settings.sqlite", bool Debug = false)
            {
                _AHK ahk = new _AHK();
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                string UpdateLine = "Update [FileIndex] set FID = '" + inObject.FID + "', FileFlag = '" + inObject.FileFlag + "', FilePath = '" + inObject.FilePath + "', FileName = '" + inObject.FileName + "', DirName = '" + inObject.DirName + "', DirPath = '" + inObject.DirPath + "', FileExt = '" + inObject.FileExt + "', FileSize = '" + inObject.FileSize + "', FileSizeBytes = '" + inObject.FileSizeBytes + "', CreationTime = '" + inObject.CreationTime + "', LastAccessTime = '" + inObject.LastAccessTime + "', LastWriteTime = '" + inObject.LastWriteTime + "', IsReadOnly = '" + inObject.IsReadOnly + "', Attributes = '" + inObject.Attributes + "', FileExists = '" + inObject.FileExists + "', FileAction = '" + inObject.FileAction + "', Tags = '" + inObject.Tags + "', Notes = '" + inObject.Notes + "' WHERE FilePath = '" + inObject.FilePath + "'";
                bool Updated = _Execute(DbFile, UpdateLine, Debug);
                if (Debug) { ahk.MsgBox("Updated [FileIndex] = " + Updated.ToString()); }
                return Updated;
            }





            public DataTable _Create_FileIndex_DataTable(fileIndex inObject)
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

            public void _Enable_FileIndex_Columns(DataGridView dv)
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
            public void _HideShow_FileIndex_Columns(DataGridView dv)
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




            public struct _FileIndex
            {
                public string FID { get; set; }
                public string FileFlag { get; set; }
                public string FilePath { get; set; }
                public string FileName { get; set; }
                public string FileSize { get; set; }
                public string LastAccessTime { get; set; }
                public string LastWriteTime { get; set; }
                public string IsReadOnly { get; set; }
                public string FileExt { get; set; }
                public string FileExists { get; set; }
                public string DirName { get; set; }
                public string FileDirectory { get; set; }
                public string CreationTime { get; set; }
                public string Attributes { get; set; }
                public string FileAction { get; set; }
                public string Tags { get; set; }
            }





            #endregion

            #region === v1 SQL to SQLite ===

            // SQL to SQLite
            // -needs to be update statement or insert
            public void _SQLTable_To_NewSQLiteTable(SqlConnection Connection, string DataBaseName, string TableName, string SQLiteDb, string NewTableName = "", bool CreateTableOnly = false, bool SingleInsert = false, bool CreateIDField = true)  // Create SQLite Table From Existing SQL Table Structure
            {
                _AHK ahk = new _AHK();
                SqlConnection Conn = Connection;
                string connString = Conn.ConnectionString;

                _Database.SQL sql = new _Database.SQL();
                //=================================================================
                // Create SQLite Table From Existing SQL Table Structure
                //=================================================================

                // SingleInsert = Method of Writing to SQLite Table 1 record at a time --- MUCH Slower but Used to Debug


                //SqlConnection T2SS = new SqlConnection("Server=192.168.30.178\\MSSQLSRVR2008;DataBase=T2SSPortal;Uid=QAADMIN;Pwd=P@ssword178");
                //string DataBaseName = "IMCRMData";
                //string TableName = "CRMPicklist";

                // connect to SQL database and return the column structure for a table
                // SELECT * FROM IMCRMData.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'CRMPicklist'     /* Returns info about a SQL Table Structure */
                DataTable dt = sql.GetDataTable(Connection, "SELECT * FROM " + DataBaseName + ".INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + TableName + "'");
                string NewTableLine = "ID INTEGER PRIMARY KEY";

                if (!CreateIDField) // option to not create an ID Primary Key
                {
                    NewTableLine = "";
                }

                List<string> ColumnNames = new List<string>();
                string ColumnNameString = "";

                int i = 1;
                if (dt != null)
                {
                    foreach (DataRow datarow in dt.Rows)
                    {
                        string ColumnName = datarow["COLUMN_NAME"].ToString();
                        string Position = datarow["ORDINAL_POSITION"].ToString();
                        string DataType = datarow["DATA_TYPE"].ToString();

                        if (DataType.ToUpper() == "BIT") { DataType = "BOOL"; }

                        ColumnNames.Add(ColumnName); //save column name to list

                        //NewTableLine = "ID INTEGER PRIMARY KEY, Public BOOL, ClientNum VARCHAR, NoteName VARCHAR, Note VARCHAR"; }

                        if (i == 1) { if (ColumnName.Contains("ID")) { NewTableLine = ColumnName + " INTEGER PRIMARY KEY"; } else { NewTableLine = "'" + ColumnName + "' " + DataType; } }

                        if (i != 1)
                        {
                            if (NewTableLine != "") { NewTableLine = NewTableLine + ", '" + ColumnName + "' " + DataType; }
                            if (NewTableLine == "") { NewTableLine = "'" + ColumnName + "' " + DataType; }
                        }

                        ColumnNameString = ColumnNameString + "'" + ColumnName + "',";
                        i++;
                    }
                }

                ColumnNameString = ahk.TrimLast(ColumnNameString);  //trim last comma
                ColumnNameString = "(" + ColumnNameString + ")";

                //MessageBox.Show(NewTableLine);

                if (NewTableName != "") // if user provided different name for new SQLite table name, use that 
                {
                    TableName = NewTableName;
                }

                bool Created = _CreateNewTable(SQLiteDb, TableName, NewTableLine, false); //create SQLite table if it doesn't exist yet
                                                                                          //MessageBox.Show(TableName + " Created.");// = " + Created);
                _ClearTable(SQLiteDb, TableName); //remove existing data from this table before inserting


                // option to just create the table structure
                if (CreateTableOnly == true) { return; }


                //==================================================================================
                // Populate SQLite Table From Existing SQL Table - BATCH Import Method (Fastest)
                //==================================================================================

                SqlConnection sqlcon = new SqlConnection(connString);
                //ahk.MsgBox(connString.ToString());

                string SQLSearch = "SELECT * FROM [" + DataBaseName + "].[dbo].[" + TableName + "]";
                //ahk.MsgBox(SQLSearch);
                DataTable dt2 = null;
                try
                {
                    dt2 = sql.GetDataTable(sqlcon, SQLSearch);
                }
                catch
                {
                }
                //if (dt2 == null) { ahk.MsgBox(SQLSearch + " = NULL"); }

                // account for using GoDaddy SQL calls if NULL returned
                if (dt2 == null)
                {
                    SQLSearch = "SELECT * FROM [" + DataBaseName + "].[lucid].[" + TableName + "]";
                    dt2 = sql.GetDataTable(sqlcon, SQLSearch);
                    if (dt2 == null)
                    {
                        //ahk.MsgBox(SQLSearch + " = NULL");
                    }
                }
                if (dt2 == null)
                {
                    SQLSearch = "SELECT * FROM [" + DataBaseName + "].[lucidmethod].[" + TableName + "]";
                    dt2 = sql.GetDataTable(sqlcon, SQLSearch);
                    if (dt2 == null)
                    {
                        ahk.MsgBox(SQLSearch + " = NULL");
                    }
                }

                // write the sql table return values to SQLite table with bulk insert statement (MUCH FASTER THAN SINGLE INSERT)

                if (!SingleInsert)
                {
                    using (var conn = new SQLiteConnection(@"Data Source=" + SQLiteDb))
                    {
                        conn.Open();

                        var stopwatch = new Stopwatch();
                        stopwatch.Start();

                        using (var cmd = new SQLiteCommand(conn))
                        {
                            using (var transaction = conn.BeginTransaction())
                            {
                                int RowCounter = 0;

                                if (dt2 != null)
                                {
                                    foreach (DataRow sqldatarow in dt2.Rows)
                                    {
                                        int ColumnCounter = 0;
                                        string ValueString = "";
                                        foreach (string Column in ColumnNames)
                                        {
                                            string ColumnValue = dt2.Rows[RowCounter][Column].ToString();
                                            ColumnCounter++;

                                            ColumnValue = ahk.FixSpecialChars(ColumnValue); //remove invalid characters before writing

                                            ValueString = ValueString + "'" + ColumnValue + "',";
                                        }

                                        ValueString = ahk.TrimLast(ValueString);  //trim last comma

                                        string InsertLine = "INSERT into " + TableName + " " + ColumnNameString + " values (" + ValueString + ")";

                                        //if (SingleInsert) { Execute_SQLite(SQLiteDb, InsertLine, true); } //write entry to SQLite table (single entry method)

                                        cmd.CommandText = InsertLine + ";";
                                        cmd.ExecuteNonQuery();

                                        RowCounter++;
                                    }
                                }



                                if (!SingleInsert) { transaction.Commit(); }
                            }
                        }

                        Console.WriteLine("{0} seconds with one transaction.", stopwatch.Elapsed.TotalSeconds);

                        conn.Close();
                    }
                }




                //=====================================================================================
                // Populate SQLite Table From Existing SQL Table - Single Insert Mode - DEBUG
                //=====================================================================================

                if (SingleInsert)
                {
                    int RowCounter = 0;
                    SQLiteConnection conn = _ConnectToDB(SQLiteDb); // connect to SQLite DB file path - returns connection data
                    if (dt2 != null)
                    {
                        foreach (DataRow sqldatarow in dt2.Rows)
                        {
                            int ColumnCounter = 0;
                            string ValueString = "";
                            foreach (string Column in ColumnNames)
                            {
                                string ColumnValue = dt2.Rows[RowCounter][Column].ToString();
                                ColumnCounter++;

                                ColumnValue = ahk.FixSpecialChars(ColumnValue); //remove invalid characters before writing

                                ValueString = ValueString + "'" + ColumnValue + "',";
                            }

                            ValueString = ahk.TrimLast(ValueString);  //trim last comma

                            string InsertLine = "INSERT into " + TableName + " " + ColumnNameString + " values (" + ValueString + ")";

                            _sqlite(InsertLine, conn, true); //write entry to SQLite table

                            RowCounter++;
                        }
                    }


                    conn.Close();
                }


                //MessageBox.Show("Finished Updating SQLite Db"); 

            }

            public void _SQLDb_To_SQLiteDb(SqlConnection Connection, string DataBaseName, string SQLiteDb, bool CreateTableOnly = false, bool SingleInsert = false, bool CreateIDField = true) // copy tablestructure and database contents from sql table to local sqlite db file
            {
                _Database.SQL sql = new _Database.SQL();

                //======================================
                // Return List of SQL Table Names
                //======================================
                // EX: 
                List<string> SQLTableNames = sql.Return_TableList(Connection);
                foreach (string TableName in SQLTableNames)
                {
                    //ahk.MsgBox(TableName);
                    _SQLTable_To_NewSQLiteTable(Connection, DataBaseName, TableName, SQLiteDb, "", CreateTableOnly, SingleInsert, CreateIDField);
                }

            }


            public void _Backup_Work_Dbs()  // example function used to backup a sql table to sqlite file
            {
                SqlConnection T2SS = new SqlConnection("Server=192.168.30.178\\MSSQLSRVR2008;DataBase=T2SSPortal;Uid=QAADMIN;Pwd=P@ssword178");
                string DataBaseName = "IMCRMData";
                string TableName = "CRMPicklist";

                string DbFile = @"S:\Db\CRM.sqlite";

                _SQLTable_To_NewSQLiteTable(T2SS, "IMCRMData", "CRMPicklist", DbFile);

                //SQLDb_To_SQLiteDb(T2SS, "IMCRMData", DbFile); 
            }


            #endregion

            #region === v1 Table Functions ===  Done

            /// <summary>
            /// Create New SQLite Table 
            /// </summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of New Table in .sqlite File Create</param>
            /// <param name="NewTableLine">Define Table Columns</param>
            /// <param name="OverWrite">Option to Drop And Create New Table If TableName Already Exists</param>
            /// 
            ///string NewTableLine = "ID INTEGER PRIMARY KEY, SettingName VARCHAR, SettingValue VARCHAR, Flag BOOL";
            ///sqlite.Table_New = (DbFile, "Settings", NewTableLine);
            public bool _Table_New(string DbFile, string TableName, string NewTableLine, bool OverWrite = false)  // create new sqlite db table
            {
                // Create New SQLite DB (*Used First-Run*)
                if (!File.Exists(DbFile)) // create database file if it doen't exist
                {
                    SQLiteConnection.CreateFile(DbFile);
                }

                // Check to see if TableName already exists in SQLite Db
                bool TableExist = _Table_Exists(DbFile, TableName);

                // option to overwrite existing table if it exists
                if (OverWrite)
                {
                    if (TableExist) { _DropTable(DbFile, TableName); }
                }

                if (!TableExist)  // Table DOES NOT exist in SQLite DB - Create Now
                {
                    _Execute(DbFile, "CREATE TABLE " + TableName + " (" + NewTableLine + ")");  // Create new Table
                                                                                                // confirm table exists and return true/false
                    return _Table_Exists(DbFile, TableName);
                }

                return true;  // Table Already Exists - Return True
            }

            /// <summary>
            /// Clears Contents of SQLite Table
            /// </summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Clear</param>
            /// <param name="RestoreSpace">Option To Vacuum Database File To Restore Unused Space In The .sqlite File</param>
            /// <param name="Debug">Enables Additional MessageBox For TroubleShooting</param>
            public bool _Table_Clear(string DbFile, string TableName, bool RestoreSpace = true, bool Debug = false)  // clear out existing sqlite table contents
            {
                _AHK ahk = new _AHK();
                if (Debug) { if (ahk.IfNotExist(DbFile)) { ahk.MsgBox(" DbFile Not Found - Possibly have Function Backwards in Code."); return false; } }

                SQLiteConnection db = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data

                //=== Drop Table from DB File IF It Exists =========================
                bool Return = _sqlite("Delete From [" + TableName + "]", db);

                if (RestoreSpace)
                {
                    _Table_Shrink(DbFile);  //restores space after deleting from tables
                }

                return Return;
            }

            /// <summary>
            /// Drops Table from SQLite Table (Aka Delete)
            /// </summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Remove</param>
            /// <param name="RestoreSpace">Option To Vacuum Database File To Restore Unused Space In The .sqlite File</param>
            public bool _Table_Drop(string DbFile, string TableName, bool RestoreSpace = true)  // drop / remove table + table contents from sqlite db file
            {
                //=== Drop Table from DB File IF It Exists =========================
                bool Return = _Execute(DbFile, "Drop Table IF EXISTS [" + TableName + "]");

                if (RestoreSpace)
                {
                    _Table_Shrink(DbFile);  //restores space after deleting from tables
                }

                return Return;  // otherwise TRUE, dropped table
            }

            /// <summary>
            /// Restores Previously Used Space in Database File (Vacuum)
            /// </summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location</param>
            public bool _Table_Shrink(string DbFile)  //restores space after deleting from tables
            {
                bool Return = _Execute(DbFile, "VACUUM;");
                return Return;
            }

            /// <summary>
            /// Returns List of TableNames in .sqlite Database File
            /// </summary>
            /// <param name="SQLiteFile">Path to .sqlite DataBase File Location</param>
            public List<string> _Table_List(string SQLiteFile)
            {
                _AHK ahk = new _AHK();
                if (!File.Exists(SQLiteFile)) { ahk.MsgBox(ahk.FileName(SQLiteFile) + " Not Found"); return null; } // database file doesn't exist - table not found

                List<string> TableListOut = new List<string>();

                // Extract TABLE Names from SQLite DB

                // Initialize the connection
                SQLiteConnection conn = new SQLiteConnection("data source=" + SQLiteFile);

                // These is how you list the schema of an SQLite database
                SQLiteCommand comm = new SQLiteCommand("SELECT * FROM SQLite_master WHERE type = 'table' ORDER BY 1", conn);
                conn.Open();

                // Populate the reader
                SQLiteDataReader reader = comm.ExecuteReader();


                // Parse Elements from SQLite DB File - Step through each row 
                while (reader.Read())
                {
                    for (int a = 0; a < reader.FieldCount; a++)
                    {
                        string columnName = reader.GetName(a);  // This will give you the name of the current row's column
                        string columnValue = reader[a].ToString();  // This will give you the value of the current row's column
                                                                    //MessageBox.Show(columnName + " = " + columnValue); 

                        if (columnName == "tbl_name")
                        {
                            TableListOut.Add(columnValue);
                        }
                    }
                }

                _DisconnectFromDB(conn);
                return TableListOut;
            }

            /// <summary>
            /// Checks To See If Table Name Exists in SQLite Db - Returns True/False
            /// </summary>
            /// <param name="SQLiteFile">Path to .sqlite DataBase File Location</param>
            /// <param name="SearchTableName">Name of the Table to Check For</param>
            public bool _Table_Exists(string SQLiteFile, string SearchTableName)
            {
                if (!File.Exists(SQLiteFile)) { return false; } // database file doesn't exist - table not found

                Dictionary<string, string> TableNamesDict = _ExtractTableNameList(SQLiteFile);

                if (TableNamesDict.ContainsKey(SearchTableName))
                {
                    return true;
                }

                return false;
            }


            #endregion

            #region === v1 Columns === Done

            /// <summary>
            /// Returns # of Columns In TableName
            /// </summary>
            /// <param name="SQLiteFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Search</param>
            public int _Column_Count(string SQLiteFile, string TableName)
            {
                _AHK ahk = new _AHK();
                if (!File.Exists(SQLiteFile)) // confirm database file exists
                {
                    ahk.MsgBox(SQLiteFile + " Not Found - Unable to Load Column Names");
                    return -1;
                }

                // Initialize the connection
                SQLiteConnection conn = new SQLiteConnection("data source=" + SQLiteFile);

                // These is how you list the schema of an SQLite database
                SQLiteCommand comm = new SQLiteCommand("SELECT * FROM SQLite_master WHERE type = 'table' ORDER BY 1", conn);
                conn.Open();
                // Populate the reader
                SQLiteDataReader reader = comm.ExecuteReader();

                var cmd = new SQLiteCommand("select * from '" + TableName + "'", conn);
                var dr = cmd.ExecuteReader();

                int ColumnCount = dr.FieldCount;

                //// alternative method to count columns
                //for (var i = 0; i < dr.FieldCount; i++) { ColumnCount++; }

                _DisconnectFromDB(conn);  // free up db for other use

                return ColumnCount;
            }

            /// <summary>
            /// Returns list of Column Names in .sqlite Table
            /// </summary>
            /// <param name="SQLiteFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Search</param>
            public List<string> _Column_List(string SQLiteFile, string TableName)
            {
                _AHK ahk = new _AHK();
                if (!File.Exists(SQLiteFile)) // confirm database file exists
                {
                    ahk.MsgBox(SQLiteFile + " Not Found - Unable to Load Column Names");
                    return null;
                }

                List<string> ColumnNames = new List<string>();

                // Initialize the connection
                SQLiteConnection conn = new SQLiteConnection("data source=" + SQLiteFile);

                // These is how you list the schema of an SQLite database
                SQLiteCommand comm = new SQLiteCommand("SELECT * FROM SQLite_master WHERE type = 'table' ORDER BY 1", conn);
                conn.Open();
                // Populate the reader
                SQLiteDataReader reader = comm.ExecuteReader();

                // Loop through TABLE name to extract column names 

                var cmd = new SQLiteCommand("select * from '" + TableName + "'", conn);
                var dr = cmd.ExecuteReader();

                for (var i = 0; i < dr.FieldCount; i++)
                {
                    string Column = dr.GetName(i);
                    ColumnNames.Add(Column);
                }

                _DisconnectFromDB(conn);  // free up db for other use

                return ColumnNames;
            }

            /// <summary>
            /// Returns Dictionary Var With Column Name + Column Data Type 
            /// </summary>
            /// <param name="SQLiteFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Search</param>
            public Dictionary<string, string> _Column_Dict(string SQLiteFile, string TableName)
            {
                Dictionary<string, string> ColumnNamesDict = new Dictionary<string, string>();

                // Initialize the connection
                SQLiteConnection conn = new SQLiteConnection("data source=" + SQLiteFile);

                // These is how you list the schema of an SQLite database
                SQLiteCommand comm = new SQLiteCommand("SELECT * FROM SQLite_master WHERE type = 'table' ORDER BY 1", conn);
                conn.Open();
                // Populate the reader
                SQLiteDataReader reader = comm.ExecuteReader();

                // Loop through TABLE name to extract COLUMN names / column COUNT

                var cmd = new SQLiteCommand("select * from '" + TableName + "'", conn);
                var dr = cmd.ExecuteReader();

                for (var i = 0; i < dr.FieldCount; i++)
                {
                    string Column = dr.GetName(i);

                    if (!ColumnNamesDict.ContainsKey(Column))
                    {
                        ColumnNamesDict.Add(Column, dr.GetDataTypeName(i).ToString());
                    }

                }

                _DisconnectFromDB(conn);  // free up db for other use

                return ColumnNamesDict;
            }

            /// <summary>
            /// Return Column Type [BOOL/VARCHAR/INTEGER] 
            /// Search by Column Name or Column Position #
            /// </summary>
            /// <param name="SQLiteFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Read</param>
            /// <param name="ColNameOrPos">Column Name or Column Postion #</param>
            public string _Column_Type(string SQLiteFile, string TableName, object ColNameOrPos)
            {
                _AHK ahk = new _AHK();
                if (!File.Exists(SQLiteFile))
                {
                    ahk.MsgBox(SQLiteFile + " Not Found - Unable to Load Column Type");
                    return "";
                }

                // Initialize the connection
                SQLiteConnection conn = new SQLiteConnection("data source=" + SQLiteFile);

                // These is how you list the schema of an SQLite database
                SQLiteCommand comm = new SQLiteCommand("SELECT * FROM SQLite_master WHERE type = 'table' ORDER BY 1", conn);
                conn.Open();
                // Populate the reader
                SQLiteDataReader reader = comm.ExecuteReader();

                //========================================================================
                // Loop through TABLE Column Types
                //========================================================================

                // Sqlite Column Field Types (dr.GetFieldType(i).ToString())
                //          System.String
                //          System.Boolean
                //          System.Int64

                // Sqlite Column Data Types (dr.GetDataTypeName(i).ToString())
                //          BOOL
                //          VARCHAR
                //          INTEGER

                var cmd = new SQLiteCommand("select * from '" + TableName + "'", conn);
                var dr = cmd.ExecuteReader();

                for (var i = 0; i < dr.FieldCount; i++)
                {
                    string Column = dr.GetName(i);
                    string colFieldType = dr.GetFieldType(i).ToString();
                    string colType = dr.GetDataTypeName(i).ToString();

                    //ahk.MsgBox(Column + " - " + colType + " - " + dr.GetDataTypeName(i).ToString() + " - " + dr.GetType().ToString());

                    if (ColNameOrPos.GetType().ToString() == "System.String")  // user passed in Column Name
                    {
                        if (ColNameOrPos.ToString().ToUpper() == Column.ToUpper())   // match column by current column name 
                        {
                            _DisconnectFromDB(conn);  // free up db for other use
                            return colType;
                        }
                    }

                    if (ColNameOrPos.GetType().ToString() == "System.Int32")  // user passed in Column #
                    {
                        if (ColNameOrPos.ToString() == i.ToString())   // match column by current col # int
                        {
                            _DisconnectFromDB(conn);  // free up db for other use
                            return colType;
                        }
                    }
                }

                _DisconnectFromDB(conn);  // free up db for other use
                return ""; // no match found to col name / col number
            }

            /// <summary>
            /// Returns DataTable with SQLite Table Schema
            /// </summary>
            /// <param name="SQLiteFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Read</param>
            public DataTable _Table_SchemaTable(string SQLiteFile, string TableName)
            {
                _AHK ahk = new _AHK();
                if (!File.Exists(SQLiteFile))
                {
                    ahk.MsgBox(SQLiteFile + " Not Found - Unable to Load DataBase Schema Table");
                    //return "";
                    return null;
                }

                // Initialize the connection
                SQLiteConnection conn = new SQLiteConnection("data source=" + SQLiteFile);

                // These is how you list the schema of an SQLite database
                SQLiteCommand comm = new SQLiteCommand("SELECT * FROM SQLite_master WHERE type = 'table' ORDER BY 1", conn);
                conn.Open();
                // Populate the reader
                SQLiteDataReader reader = comm.ExecuteReader();

                var cmd = new SQLiteCommand("select * from '" + TableName + "'", conn);
                var dr = cmd.ExecuteReader();

                DataTable sqliteTable = dr.GetSchemaTable();

                _DisconnectFromDB(conn);  // free up db for other use

                return sqliteTable;
            }

            /// <summary>
            /// Return SQLite Column Info From Table Schema 
            /// </summary>
            /// <param name="SQLiteFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Read</param>
            /// <param name="ColNameOrPos">Column Name or Column Postion #</param>
            /// <param name="ReturnType">OPTIONS: IsKey / IsUnique / AllowDbNull / ColumnName / ColumnNum / ColumnSize / DataType / DataTypeName</param>
            public string _Column_Info(string SQLiteFile, string TableName, object ColNameOrPos, string ReturnType = "IsKey")
            {
                DataTable table = _Table_SchemaTable(SQLiteFile, TableName);

                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        string AllowDbNull = row["AllowDBNull"].ToString();
                        string IsUnique = row["IsUnique"].ToString();
                        string IsKey = row["IsKey"].ToString();
                        string ColumnName = row["ColumnName"].ToString();
                        string ColumnNum = row["ColumnOrdinal"].ToString();
                        string ColumnSize = row["ColumnSize"].ToString();
                        string DataType = row["DataType"].ToString();
                        string DataTypeName = row["DataTypeName"].ToString();

                        string ReturnVal = "";
                        if (ReturnType.ToUpper() == "ALLOWDBNULL") { ReturnVal = AllowDbNull; }
                        if (ReturnType.ToUpper() == "ISUNIQUE") { ReturnVal = IsUnique; }
                        if (ReturnType.ToUpper() == "ISKEY") { ReturnVal = IsKey; }
                        if (ReturnType.ToUpper() == "COLUMNNAME") { ReturnVal = ColumnName; }
                        if (ReturnType.ToUpper() == "COLUMNNUM") { ReturnVal = ColumnNum; }
                        if (ReturnType.ToUpper() == "COLUMNSIZE") { ReturnVal = ColumnSize; }
                        if (ReturnType.ToUpper() == "DATATYPE") { ReturnVal = DataType; }
                        if (ReturnType.ToUpper() == "DATATYPENAME") { ReturnVal = DataTypeName; }


                        if (ColNameOrPos.GetType().ToString() == "System.String")  // user passed in Column Name
                        {
                            if (ColNameOrPos.ToString().ToUpper() == ColumnName.ToUpper())   // match column by current column name 
                            {
                                return ReturnVal;
                            }
                        }

                        if (ColNameOrPos.GetType().ToString() == "System.Int32")  // user passed in Column #
                        {
                            if (ColNameOrPos.ToString() == ColumnNum.ToString())   // match column by current col # int
                            {

                                return ReturnVal;
                            }
                        }

                    }
                }


                return "";
            }

            /// <summary>
            /// Is SQLite Column a Key Column in Table (Returns True/False)
            /// </summary>
            /// <param name="SQLiteFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Read</param>
            /// <param name="ColNameOrPos">Column Name or Column Postion #</param>
            public bool _Column_IsKey(string SQLiteFile, string TableName, object ColNameOrPos)
            {
                DataTable table = _Table_SchemaTable(SQLiteFile, TableName);

                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        string IsKey = row["IsKey"].ToString();
                        string ColumnName = row["ColumnName"].ToString();
                        string ColumnNum = row["ColumnOrdinal"].ToString();

                        if (ColNameOrPos.GetType().ToString() == "System.String")  // user passed in Column Name
                        {
                            if (ColNameOrPos.ToString().ToUpper() == ColumnName.ToUpper())   // match column by current column name 
                            {
                                if (IsKey.ToUpper() == "TRUE") { return true; }
                                if (IsKey.ToUpper() == "FALSE") { return false; }
                            }
                        }

                        if (ColNameOrPos.GetType().ToString() == "System.Int32")  // user passed in Column #
                        {
                            if (ColNameOrPos.ToString() == ColumnNum.ToString())   // match column by current col # int
                            {
                                if (IsKey.ToUpper() == "TRUE") { return true; }
                                if (IsKey.ToUpper() == "FALSE") { return false; }
                            }
                        }
                    }
                }

                return false;
            }

            /// <summary>
            /// Does SQLite Column Require Unique Value in Column (Returns True/False)
            /// </summary>
            /// <param name="SQLiteFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Read</param>
            /// <param name="ColNameOrPos">Column Name or Column Postion #</param>
            public bool _Column_IsUnique(string SQLiteFile, string TableName, object ColNameOrPos)
            {
                DataTable table = _Table_SchemaTable(SQLiteFile, TableName);

                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        string IsUnique = row["IsUnique"].ToString();
                        string ColumnName = row["ColumnName"].ToString();
                        string ColumnNum = row["ColumnOrdinal"].ToString();

                        if (ColNameOrPos.GetType().ToString() == "System.String")  // user passed in Column Name
                        {
                            if (ColNameOrPos.ToString().ToUpper() == ColumnName.ToUpper())   // match column by current column name 
                            {
                                if (IsUnique.ToUpper() == "TRUE") { return true; }
                                if (IsUnique.ToUpper() == "FALSE") { return false; }
                            }
                        }

                        if (ColNameOrPos.GetType().ToString() == "System.Int32")  // user passed in Column #
                        {
                            if (ColNameOrPos.ToString() == ColumnNum.ToString())   // match column by current col # int
                            {
                                if (IsUnique.ToUpper() == "TRUE") { return true; }
                                if (IsUnique.ToUpper() == "FALSE") { return false; }
                            }
                        }
                    }
                }

                return false;
            }

            /// <summary>
            /// Does SQLite Column Allow Null Values Inserted Into Column (Returns True/False)
            /// </summary>
            /// <param name="SQLiteFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Read</param>
            /// <param name="ColNameOrPos">Column Name or Column Postion #</param>
            public bool _Column_AllowNull(string SQLiteFile, string TableName, object ColNameOrPos)
            {
                DataTable table = _Table_SchemaTable(SQLiteFile, TableName);

                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        string AllowDbNull = row["AllowDBNull"].ToString();
                        string ColumnName = row["ColumnName"].ToString();
                        string ColumnNum = row["ColumnOrdinal"].ToString();

                        if (ColNameOrPos.GetType().ToString() == "System.String")  // user passed in Column Name
                        {
                            if (ColNameOrPos.ToString().ToUpper() == ColumnName.ToUpper())   // match column by current column name 
                            {
                                if (AllowDbNull.ToUpper() == "TRUE") { return true; }
                                if (AllowDbNull.ToUpper() == "FALSE") { return false; }
                            }
                        }

                        if (ColNameOrPos.GetType().ToString() == "System.Int32")  // user passed in Column #
                        {
                            if (ColNameOrPos.ToString() == ColumnNum.ToString())   // match column by current col # int
                            {
                                if (AllowDbNull.ToUpper() == "TRUE") { return true; }
                                if (AllowDbNull.ToUpper() == "FALSE") { return false; }
                            }
                        }
                    }
                }

                return false;
            }

            /// <summary>
            /// Returns Max Size For SQLite Column (int)
            /// </summary>
            /// <param name="SQLiteFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Read</param>
            /// <param name="ColNameOrPos">Column Name or Column Postion #</param>
            public int _Column_Size(string SQLiteFile, string TableName, object ColNameOrPos)
            {
                _AHK ahk = new _AHK();
                DataTable table = _Table_SchemaTable(SQLiteFile, TableName);

                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        string ColumnSize = row["ColumnSize"].ToString();
                        string ColumnName = row["ColumnName"].ToString();
                        string ColumnNum = row["ColumnOrdinal"].ToString();

                        if (ColNameOrPos.GetType().ToString() == "System.String")  // user passed in Column Name
                        {
                            if (ColNameOrPos.ToString().ToUpper() == ColumnName.ToUpper())   // match column by current column name 
                            {
                                return ahk.ToInt(ColumnSize);
                            }
                        }

                        if (ColNameOrPos.GetType().ToString() == "System.Int32")  // user passed in Column #
                        {
                            if (ColNameOrPos.ToString() == ColumnNum.ToString())   // match column by current col # int
                            {
                                return ahk.ToInt(ColumnSize);
                            }
                        }
                    }
                }

                return -1;
            }

            /// <summary>
            /// Returns Column Name From the Column Position #
            /// </summary>
            /// <param name="SQLiteFile">Path to .sqlite DataBase File Location</param>
            /// <param name="TableName">Name of Table in .sqlite File To Read</param>
            /// <param name="ColumnPos">Column Postion #</param>
            public string _Column_Name(string SQLiteFile, string TableName, int ColumnPos)
            {
                DataTable table = _Table_SchemaTable(SQLiteFile, TableName);

                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        string ColumnSize = row["ColumnSize"].ToString();
                        string ColumnName = row["ColumnName"].ToString();
                        string ColumnNum = row["ColumnOrdinal"].ToString();

                        if (ColumnPos.ToString() == ColumnNum.ToString())   // match column by current col # int
                        {
                            return ColumnName;
                        }

                    }
                }

                return "";
            }


            #endregion

            #region === v1 Settings ===  Done

            /// <summary>
            /// Saves Setting to Project SQLite Database File
            /// </summary>
            /// <param name="SettingName">Unique Setting Name To Store User Value</param>
            /// <param name="Value">Value For Setting To Save And Return Later</param>
            /// <param name="Option">Optional Parameter For Enabling/Disabling/Sorting Settings</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To Settings.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            public bool _Setting_Save(string SettingName, string Value, string Option = "", string DbFile = "Settings.sqlite", string TableName = "Settings")  //Support.Desk - writes setting to sqlite setting table
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                // Create New Table If It Does NOT Exist Yet
                bool TableExist = _Table_Exists(DbFile, TableName);  //See if selected Table Exists in SQLite DB file
                if (!TableExist) { _Settings_NewDb(DbFile, TableName); }  // Table DOES NOT exist in SQLite DB - Create Now

                SQLiteConnection m_dbConnection = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data

                // UPDATE or INSERT Server Files
                bool Updated = _sqlite("UPDATE " + TableName + " set Value = '" + Value + "', Option = '" + Option + "',TimeStamp = '" + DateTime.Now.ToString() + "' WHERE Setting = '" + SettingName + "'", m_dbConnection);  // Update Table
                bool ReturnBool = true;
                if (!Updated) { ReturnBool = _sqlite("INSERT into " + TableName + " (Setting, Value, Option, TimeStamp) values ('" + SettingName + "','" + Value + "','" + Option + "','" + DateTime.Now.ToString() + "')", m_dbConnection); }  // insert into a Table
                if (ReturnBool == false) { MessageBox.Show("Failed to Insert: " + SettingName + "' | '" + Value + " | " + Option + "' | '" + DateTime.Now.ToString()); }
                return ReturnBool;
            }

            /// <summary>
            /// Return Setting From Project SQLite DataBase File
            /// </summary>
            /// <param name="SettingName">Unique Setting Name To Store User Value</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To AppName.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            public string _Setting_Value(string SettingName, string DbFile = "Settings.sqlite", string TableName = "Settings")
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite._ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data

                string SearchLine = "Select * from " + TableName + " WHERE Setting = '" + SettingName + "'";
                SQLiteDataReader reader = lite._ReturnSQLite(SearchLine, m_dbConnection);  // request data from DB

                string Value = "";
                string Option = "";
                string TimeStamp = "";
                while (reader.Read())    // loop through each row returned from select 
                {
                    TimeStamp = reader["TimeStamp"].ToString();
                    Value = reader["Value"].ToString();
                    Option = reader["Option"].ToString();
                }

                lite._DisconnectFromDB(m_dbConnection);  // free up db for other use
                return Value;
            }

            /// <summary>
            /// Returns Setting Option Value From Project SQLite Database File
            /// </summary>
            /// <param name="SettingName">Unique Setting Name To Store User Value</param>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To AppName.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            public string _Setting_Option(string SettingName, string DbFile = "Settings.sqlite", string TableName = "Settings")
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite._ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data

                string SearchLine = "Select * from " + TableName + " WHERE Setting = '" + SettingName + "'";
                SQLiteDataReader reader = lite._ReturnSQLite(SearchLine, m_dbConnection);  // request data from DB

                string Value = "";
                string Option = "";
                string TimeStamp = "";
                while (reader.Read())    // loop through each row returned from select 
                {
                    TimeStamp = reader["TimeStamp"].ToString();
                    Value = reader["Value"].ToString();
                    Option = reader["Option"].ToString();
                }

                lite._DisconnectFromDB(m_dbConnection);  // free up db for other use
                return Option;
            }

            /// <summary>
            /// Returns List of All Setting Names Stored in Project SQLite Database File
            /// </summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To AppName.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            public List<string> _Setting_List(string DbFile = "Settings.sqlite", string TableName = "Settings")
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite._ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data

                string SearchLine = "Select * from " + TableName;
                SQLiteDataReader reader = lite._ReturnSQLite(SearchLine, m_dbConnection);  // request data from DB

                List<string> Settings = new List<string>();

                while (reader.Read())    // loop through each row returned from select 
                {
                    string Setting = reader["Setting"].ToString();
                    Settings.Add(Setting);
                }

                lite._DisconnectFromDB(m_dbConnection);  // free up db for other use
                return Settings;
            }

            /// <summary>
            /// Creates New Project SQLite Settings Database + Table. Writes Automatically On First Save If Not Executed Elsewhere First
            /// </summary>
            /// <param name="DbFile">Path to .sqlite DataBase File Location - Defaults To AppName.sqlite In Application Directory</param>
            /// <param name="TableName">Name of Table in .sqlite File To Save To - Defaults To 'Settings'</param>
            /// <param name="OverWriteOld">Option to Overwrite Previous Setting Table If Found</param>
            public bool _Settings_NewDb(string DbFile = "Settings.sqlite", string TableName = "Settings", bool OverWriteExisting = false)
            {
                _AHK ahk = new _AHK();
                // if no database name is provided - default saved to Settings.sqlite in application directory
                if (DbFile == "Settings.sqlite" || DbFile == "") { DbFile = ahk.AppDir() + "\\Settings.sqlite"; }

                if (OverWriteExisting)  // option to clear out previous Db setttings
                {
                    _Table_Clear(DbFile, TableName, true);
                }

                // Create New SQLite DB (*Used First-Run*)
                if (!File.Exists(DbFile)) // create database file if it doen't exist
                {
                    SQLiteConnection.CreateFile(DbFile);
                }

                // Connect to the DB
                SQLiteConnection m_dbConnection = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data


                // Create New Table If It Does NOT Exist Yet
                bool TableExist = _Table_Exists(DbFile, TableName);  //See if selected Table Exists in SQLite DB file

                if (!TableExist)  // Table DOES NOT exist in SQLite DB
                {

                    string NewTableLine = "ID INTEGER PRIMARY KEY, Setting VARCHAR UNIQUE, Value VARCHAR, Option VARCHAR, TimeStamp VARCHAR";

                    //ahk.MsgBox(NewTableLine); 

                    bool ReturnValue = _sqlite("CREATE TABLE [" + TableName + "] (" + NewTableLine + ")", m_dbConnection);  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
                }


                _DisconnectFromDB(m_dbConnection);  // free up db for other use

                if (File.Exists(DbFile)) { return true; }
                return false;
            }

            #endregion

            #region === v1 OLD ===

            #region === Support Desk ===

            //==== used in Support.Desk ====================================
            public void SetupDatabases(string DbFile)  //Support.Desk - Setup databases function
            {
                CreateUserDb(DbFile, "HotStrings");
                CreateUserDb(DbFile, "Notes");
                CreateUserDb(DbFile, "MouseMacros");
                CreateUserDb(DbFile, "ActiveTitleLog");
                CreateUserDb(DbFile, "UserLists");
            }
            public void CreateUserDb(string NewDBFile, string TableName = "HotStrings", string Options = "") //Support.Desk - create user database tables
            {
                _AHK ahk = new _AHK();
                if (Options.Contains("NewDBFile=True"))
                {
                    _DisconnectFromDB();
                    ahk.FileDelete(NewDBFile);
                }

                //========================================================
                // Create New SQLite DB (*Used First-Run*)
                //========================================================
                if (!File.Exists(NewDBFile)) // create database file if it doen't exist
                {
                    SQLiteConnection.CreateFile(NewDBFile);
                }

                //===================================
                // Connect to the DB
                //===================================

                SQLiteConnection m_dbConnection = _ConnectToDB(NewDBFile); // connect to SQLite DB file path - returns connection data


                //=====================================================
                // Create New Table If It Does NOT Exist Yet
                //=====================================================

                bool TableExist = _DoesTableExist(NewDBFile, TableName);  //See if selected Table Exists in SQLite DB file

                if (!TableExist)  // Table DOES NOT exist in SQLite DB
                {
                    string NewTableLine = "";

                    if (TableName == "HotStrings") { NewTableLine = "FID INTEGER PRIMARY KEY, Enabled BOOL, HotString VARCHAR, HotStringValue VARCHAR"; }

                    if (TableName == "Notes") { NewTableLine = "ID INTEGER PRIMARY KEY, Public BOOL, ClientNum VARCHAR, NoteName VARCHAR, Note VARCHAR"; }

                    if (TableName == "MouseMacros") { NewTableLine = "ID INTEGER PRIMARY KEY, MacroName VARCHAR, MouseAction VARCHAR, MouseX VARCHAR, MouseY VARCHAR, ActiveWindow VARCHAR, Other VARCHAR, CurrentTime VARCHAR"; }

                    if (TableName == "ActiveTitleLog") { NewTableLine = "ID INTEGER PRIMARY KEY, ActiveTitle VARCHAR, CurrentTime VARCHAR"; }

                    if (TableName == "SupportDeskServer") { NewTableLine = "ID INTEGER PRIMARY KEY, Setting VARCHAR, Value VARCHAR, ReturnValue VARCHAR, TimeStamp VARCHAR"; }

                    if (TableName == "UserSettings") { NewTableLine = "ID INTEGER PRIMARY KEY, Setting VARCHAR, Value VARCHAR, ReturnValue VARCHAR, TimeStamp VARCHAR"; }

                    if (TableName == "FunctionLib") { NewTableLine = "Flagged BOOL, FileName VARCHAR, NameSpace VARCHAR, Class VARCHAR, Function VARCHAR, Type VARCHAR, LineNum VARCHAR, Tags VARCHAR, Comments VARCHAR, TimeStamp VARCHAR, FunctionLine VARCHAR, FunctionText VARCHAR, FilePath VARCHAR, Parent VARCHAR, Example VARCHAR, Documentation VARCHAR, UsedInProjects VARCHAR, DateAdded VARCHAR, FID INTEGER"; }

                    if (TableName == "ProjectFunctions") { NewTableLine = "Flagged BOOL, FileName VARCHAR, NameSpace VARCHAR, Class VARCHAR, Function VARCHAR, Type VARCHAR, LineNum VARCHAR, Tags VARCHAR, Comments VARCHAR, TimeStamp VARCHAR, FunctionLine VARCHAR, FunctionText VARCHAR, FilePath VARCHAR, Parent VARCHAR, Example VARCHAR, Documentation VARCHAR, UsedInProjects VARCHAR, DateAdded VARCHAR, FID INTEGER"; }

                    //if (TableName == "ApplicationLog") { NewTableLine = "ID INTEGER PRIMARY KEY, ApplicationAction VARCHAR, Value VARCHAR, ReturnValue VARCHAR, TimeStamp VARCHAR"; }

                    if (TableName == "UserLists") { NewTableLine = "ID INTEGER PRIMARY KEY, ListName VARCHAR, ListAction VARCHAR, ListValue VARCHAR, ListOption VARCHAR, DateNeeded VARCHAR, UpdateTimeStamp VARCHAR, ActionCompleted BOOL, ServerResponse VARCHAR"; }

                    if (TableName == "SupportDeskFileServer") { NewTableLine = "ID INTEGER PRIMARY KEY, FullPath VARCHAR, Directory VARCHAR, FileName VARCHAR, FileSize VARCHAR, DateModified VARCHAR, FileTag VARCHAR, FileFlag VARCHAR"; }

                    if (TableName == "SupportDeskUserFiles") { NewTableLine = "ID INTEGER PRIMARY KEY, FullPath VARCHAR, Directory VARCHAR, FileName VARCHAR, FileSize VARCHAR, DateModified VARCHAR, FileTag VARCHAR, FileFlag VARCHAR"; }

                    if (TableName == "SSR") { NewTableLine = "`ID` INTEGER, `EntryDate` datetime, `LastUpdate` datetime, `IsParent` BOOL, `SSR` varchar, `SSRType` varchar, `SSRSource` varchar, 	`SSRSeverity` varchar, `SSRStatus` varchar, `SSRSubStatus` varchar, `PossibleHippaIncident` BOOL, `ClientName` varchar, `ClientNumber` varchar,	`SupportGroupRegion` varchar, `ClientState`	varchar, `MainFrame` varchar, `CSC`	varchar, `CSCEmail`	varchar, `AssetNumber` varchar, 	`AssetName`	varchar, `ProductName` varchar, `LegacyNumber` varchar, `SSRFunction` varchar, `ProblemCategory` varchar, `SSROwner` varchar, 	`Billable` BOOL, `BillableHours` varchar, `RDPriority` varchar, `RDStatus` varchar, `RDSustainmentLane` varchar, `BoardType` varchar, 	`CCBSource` varchar, `CCBSourceDetail` varchar, `CCBType` varchar, `CCBReason` varchar, `CCBSize` varchar, `SustainmentWeek` varchar, `RowID`	varchar, `CreatedBy` varchar, `ModifiedBy` varchar, `PrimarySSR` varchar, `SSROwnerID` varchar, `ProblemDescription` varchar, `Resolution`	varchar, `SSRGroup`	varchar, `Contact` varchar, `ContactEmail` varchar, `AssetVersion` varchar, `ClientPhone` varchar, `CreatedDate` datetime, 	`ClosedDate` datetime, `EncryptedPD` varbinary, `RoutingFamily` varchar, PRIMARY KEY(ID)"; }

                    //ahk.MsgBox(NewTableLine); 


                    bool ReturnValue = _sqlite("CREATE TABLE " + TableName + " (" + NewTableLine + ")", m_dbConnection);  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]

                    //if (TableName == "HotStrings")
                    //{
                    //    // Create Example HotString Entries
                    //    InsertHotString(".wyd", "What are you doing?", NewDBFile);
                    //    InsertHotString(".ttyl", "Talk to you later.", NewDBFile);
                    //    InsertHotString(".sys", "See you soon.", NewDBFile);
                    //}
                    //if (TableName == "Notes")
                    //{
                    //    InsertNote(true, "00000", "First Note", "Users Can Save Client Specific Notes Here.\n\nSave Notes For Personal or Group Use.", NewDBFile); 
                    //}


                }


                _DisconnectFromDB(m_dbConnection);  // free up db for other use
            }



            public bool InsertHotString(string HotString, string HotStringValue, string HotStringDb)  //Support.Desk - insert hotstring into sqlite table
            {
                bool ReturnValue = InsertListItem(GlobalVars.UserDb, "UserLists", "HotStrings", HotString, HotStringValue);
                return ReturnValue;
            }
            public bool UpdateHotString(string HotStringDb, string HotString, string HotStringValue, string Enabled, string ListItemID) //Support.Desk - update hotstring into sqlite table
            {
                Enabled = Enabled.ToUpper();
                if (Enabled == "TRUE") { Enabled = "1"; }
                if (Enabled == "FALSE") { Enabled = "0"; }

                bool ReturnValue = UpdateListItem(HotStringDb, "UserLists", "HotStrings", ListItemID, HotString, HotStringValue, "", Enabled);
                return ReturnValue;
            }
            public void ClearHotStringList() //Support.Desk - clear out hotstring table (with prompt)
            {
                _AHK ahk = new _AHK();
                var ResultValue = ahk.YesNoBox("Clear Out HotStrings?", "Are You Sure?");

                if (ResultValue.ToString() == "Yes")
                {
                    ClearUserList(GlobalVars.UserDb, "UserLists", "HotStrings");
                }
            }


            // tv db
            public string ReturnTVInfo(string SQLiteFile, string TableName, string ReturnField, string SID)  //Media.Db - Returns info from tv database field
            {
                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite._ConnectToDB(SQLiteFile); // connect to SQLite DB file path - returns connection data
                string ReturnText = "";

                //=======================================================================================================
                // Connect to SQLite DB - Request Data from DB and Return It - Loop through Each Row then ____
                //=======================================================================================================

                SQLiteDataReader reader = lite._ReturnSQLite("Select * from [" + TableName + "] WHERE SID = '" + SID + "'", m_dbConnection);  // request data from DB

                //string SID = "";
                string ShowName = "";
                string ShowType = "";
                string ShowTags = "";
                string ShowStillAiring = "";
                string ShowComplete = "";
                string ShowNamingFixed = "";
                string ShowRootPath = "";
                string EpGuidesLink = "";



                while (reader.Read())    // loop through each row returned from select 
                {
                    SID = reader["SID"].ToString();
                    ShowName = reader["ShowName"].ToString();
                    ShowType = reader["ShowType"].ToString();
                    ShowTags = reader["ShowTags"].ToString();
                    ShowStillAiring = reader["ShowStillAiring"].ToString();
                    ShowComplete = reader["ShowComplete"].ToString();
                    ShowNamingFixed = reader["ShowNamingFixed"].ToString();
                    ShowRootPath = reader["ShowRootPath"].ToString();
                    EpGuidesLink = reader["EpGuidesLink"].ToString();
                }

                lite._DisconnectFromDB(m_dbConnection);  // free up db for other use

                switch (ReturnField)
                {
                    case "SID":
                        ReturnText = SID;
                        break;
                    case "ShowName":
                        ReturnText = ShowName;
                        break;
                    case "ShowType":
                        ReturnText = ShowType;
                        break;
                    case "ShowTags":
                        ReturnText = ShowTags;
                        break;
                    case "ShowStillAiring":
                        ReturnText = ShowStillAiring;
                        break;
                    case "ShowComplete":
                        ReturnText = ShowComplete;
                        break;
                    case "ShowNamingFixed":
                        ReturnText = ShowNamingFixed;
                        break;
                    case "ShowRootPath":
                        ReturnText = ShowRootPath;
                        break;
                    case "EpGuidesLink":
                        ReturnText = EpGuidesLink;
                        break;
                }



                return ReturnText;
            }
            public void UpdateTVInfo(string SQLiteFile, string TableName, string UpdateFieldName, string UpdateValue, string SID)  //Media.Db - Updates TV db entry
            {
                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection dbConnection = lite._ConnectToDB(SQLiteFile); // connect to SQLite DB file path - returns connection data

                string SQL = "UPDATE " + TableName + " SET [" + UpdateFieldName + "] = '" + UpdateValue + "' WHERE SID = '" + SID + "';";

                lite._sqlite(SQL, dbConnection);  // 

                lite._DisconnectFromDB(dbConnection);

            }


            // macros
            public bool Insert_MouseMacro(string DbFile, bool Debug = true, string ID = "", string Flagged = "", string MacroName = "", string MouseAction = "", string MouseX = "", string MouseY = "", string ActiveWindow = "", string SearchImage = "", string CurrentTime = "", string PlayBack = "", string ActionNum = "", string Description = "", string Code = "")
            {
                _AHK ahk = new _AHK();
                string InsertLine = "Insert Into [MouseMacros] (Flagged, MacroName, MouseAction, MouseX, MouseY, ActiveWindow, SearchImage, CurrentTime, PlayBack, ActionNum, Description, Code) values ('" + Flagged + "', '" + MacroName + "', '" + MouseAction + "', '" + MouseX + "', '" + MouseY + "', '" + ActiveWindow + "', '" + SearchImage + "', '" + CurrentTime + "', '" + PlayBack + "', '" + ActionNum + "', '" + Description + "', '" + Code + "')";
                bool Inserted = _Execute_SQLite(DbFile, InsertLine, Debug);
                if (!Inserted) { ahk.MsgBox("Inserted Into [MouseMacros] = " + Inserted.ToString()); }
                return Inserted;
            }
            public bool Insert_Object_MouseMacro(string DbFile, MouseMacro inObject, bool Debug = true)
            {
                _AHK ahk = new _AHK();
                string InsertLine = "Insert Into [MouseMacros] (Flagged, MacroName, MouseAction, MouseX, MouseY, ActiveWindow, SearchImage, CurrentTime, PlayBack, ActionNum, Description, Code) values ('" + inObject.Flagged + "', '" + inObject.MacroName + "', '" + inObject.MouseAction + "', '" + inObject.MouseX + "', '" + inObject.MouseY + "', '" + inObject.ActiveWindow + "', '" + inObject.SearchImage + "', '" + inObject.CurrentTime + "', '" + inObject.PlayBack + "', '" + inObject.ActionNum + "', '" + inObject.Description + "', '" + inObject.Code + "')";
                bool Inserted = _Execute_SQLite(DbFile, InsertLine, Debug);
                if (!Inserted) { ahk.MsgBox("Inserted Into [MouseMacros] = " + Inserted.ToString()); }
                return Inserted;
            }


            public bool Flag_MacroAction(string DbFile, string ID)  // set checkbox flag to ignore this action in datagrid display
            {
                _AHK ahk = new _AHK();
                string UpdateLine = "Update [MouseMacros] set Flagged = '1' WHERE ID = '" + ID + "'";
                bool Updated = _Execute_SQLite(DbFile, UpdateLine, false);
                if (!Updated) { ahk.MsgBox("Updated [MouseMacros] = " + Updated.ToString()); }
                return Updated;
            }

            public bool Write_PlayBack_Note(string DbFile, string ID, string PlayBack)  // after executing the macro command, return feedback / show it ran
            {
                _AHK ahk = new _AHK();
                string UpdateLine = "Update [MouseMacros] set PlayBack = '" + PlayBack + "' WHERE ID = '" + ID + "'";
                bool Updated = _Execute_SQLite(DbFile, UpdateLine, false);
                if (!Updated) { ahk.MsgBox("Updated [MouseMacros] = " + Updated.ToString()); }
                return Updated;
            }


            public bool Update_MouseMacro(string DbFile, bool Debug = true, string ID = "", string Flagged = "", string MacroName = "", string MouseAction = "", string MouseX = "", string MouseY = "", string ActiveWindow = "", string SearchImage = "", string CurrentTime = "", string PlayBack = "", string ActionNum = "", string Description = "", string Code = "")
            {
                _AHK ahk = new _AHK();
                string UpdateLine = "Update [MouseMacros] set Flagged = '" + Flagged + "', MacroName = '" + MacroName + "', MouseAction = '" + MouseAction + "', MouseX = '" + MouseX + "', MouseY = '" + MouseY + "', ActiveWindow = '" + ActiveWindow + "', SearchImage = '" + SearchImage + "', CurrentTime = '" + CurrentTime + "', PlayBack = '" + PlayBack + "', ActionNum = '" + ActionNum + "', Description = '" + Description + "', Code = '" + Code + "' WHERE ID = '" + ID + "'";
                bool Updated = _Execute_SQLite(DbFile, UpdateLine, Debug);
                if (!Updated) { ahk.MsgBox("Updated [MouseMacros] = " + Updated.ToString()); }
                return Updated;
            }
            public bool Update_MouseMacro_FromObject(string DbFile, MouseMacro inObject, bool Debug = true)
            {
                _AHK ahk = new _AHK();
                string UpdateLine = "Update [MouseMacros] set Flagged = '" + inObject.Flagged + "', MacroName = '" + inObject.MacroName + "', MouseAction = '" + inObject.MouseAction + "', MouseX = '" + inObject.MouseX + "', MouseY = '" + inObject.MouseY + "', ActiveWindow = '" + inObject.ActiveWindow + "', SearchImage = '" + inObject.SearchImage + "', CurrentTime = '" + inObject.CurrentTime + "', PlayBack = '" + inObject.PlayBack + "', ActionNum = '" + inObject.ActionNum + "', Description = '" + inObject.Description + "', Code = '" + inObject.Code + "' WHERE ID = '" + inObject.ID + "'";
                bool Updated = _Execute_SQLite(DbFile, UpdateLine, Debug);
                if (!Updated) { ahk.MsgBox("Updated [MouseMacros] = " + Updated.ToString()); }
                return Updated;
            }

            public bool DeleteMouseMacro(string MacroName, string DbFile)  //Support.Desk - delete mouse macro by macro name from sqlite db file
            {
                SQLiteConnection m_dbConnection = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data
                bool ReturnValue = _sqlite("Delete FROM MouseMacros where MacroName = '" + MacroName + "'", m_dbConnection);  // insert into a Table
                _DisconnectFromDB(m_dbConnection);  // free up db for other use
                return ReturnValue;
            }
            public bool Delete_All_Mouse_Macros(string DbFile)  //Support.Desk - delete mouse macro by macro name from sqlite db file
            {
                SQLiteConnection m_dbConnection = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data
                bool ReturnValue = _sqlite("Delete FROM [MouseMacros]", m_dbConnection);  // insert into a Table
                _DisconnectFromDB(m_dbConnection);  // free up db for other use
                return ReturnValue;
            }

            public SQLiteDataReader ReturnMouseMacro(string MacroName, string DbFile)  //Support.Desk - returns mouse macro by macro name from sqlite db file
            {
                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite._ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data
                SQLiteDataReader reader = lite._ReturnSQLite("Select * from [MouseMacros] WHERE MacroName = '" + MacroName + "'", m_dbConnection);  // request data from DB
                lite._DisconnectFromDB(m_dbConnection);  // free up db for other use
                return reader;
            }
            public void PlayMouseMacro(string MacroName, string DbFile, bool Play)  //Support.Desk - play macro by name back from macro sqlite db table
            {
                _AHK ahk = new _AHK();
                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite._ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data

                string SearchLine = "Select * from [MouseMacros] WHERE MacroName = 'Mouse Recording'";
                SQLiteDataReader reader = lite._ReturnSQLite(SearchLine, m_dbConnection);  // request data from DB

                MouseMacro ms = new MouseMacro();

                if (Play == true) { GlobalVars.MacroPlaying = true; }
                if (Play == false) { GlobalVars.MacroPlaying = false; }

                while (reader.Read())    // loop through each row returned from select 
                {
                    string Ctime = reader["CurrentTime"].ToString();
                    ms.CurrentTime = DateTime.Parse(Ctime);
                    ms.ActiveWindow = reader["ActiveWindow"].ToString();
                    ms.MouseAction = reader["MouseAction"].ToString();
                    ms.MouseX = reader["MouseX"].ToString();
                    ms.MouseY = reader["MouseY"].ToString();
                    ms.SearchImage = reader["SearchImage"].ToString();
                    ms.PlayBack = reader["PlayBack"].ToString();
                    ms.ActionNum = reader["ActionNum"].ToString();
                    ms.Flagged = reader["Flagged"].ToString();
                    ms.Description = reader["Description"].ToString();
                    ms.Code = reader["Code"].ToString();
                    ms.ID = reader["ID"].ToString();

                    if (GlobalVars.MacroPlaying == false) { return; }

                    if (ms.MouseAction == "LeftButtonDown")
                    {
                        int x = Int32.Parse(ms.MouseX);
                        int y = Int32.Parse(ms.MouseY);
                        ahk.MouseClick(_AHK.MouseButton.Left, x, y);

                        ahk.Sleep(1000);
                    }
                }

                lite._DisconnectFromDB(m_dbConnection);  // free up db for other use


            }
            public bool InsertActiveWinTitle(string DbFile)  //Support.Desk - insert active win title into sqlite macro db file
            {
                _AHK ahk = new _AHK();
                string ActiveWinTitle = ahk.WinGetActiveTitle();
                SQLiteConnection m_dbConnection = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data
                string InsertString = "insert into ActiveTitleLog (ActiveTitle, CurrentTime) values ('" + ActiveWinTitle + "','" + DateTime.Now.ToString() + "')";
                bool ReturnValue = _sqlite(InsertString, m_dbConnection);  // insert into a Table
                if (ReturnValue == false) { MessageBox.Show("Failed to Insert: " + InsertString); }
                _DisconnectFromDB(m_dbConnection);  // free up db for other use
                return ReturnValue;
            }
            public void CreateMacroDb(string NewDBFile, string TableName = "MouseMacros", string Options = "") //Support.Desk - create user database tables
            {
                _AHK ahk = new _AHK();
                if (Options.Contains("NewDBFile=True"))
                {
                    _DisconnectFromDB();
                    ahk.FileDelete(NewDBFile);
                }

                //========================================================
                // Create New SQLite DB (*Used First-Run*)
                //========================================================
                if (!File.Exists(NewDBFile)) // create database file if it doen't exist
                {
                    SQLiteConnection.CreateFile(NewDBFile);
                }

                //===================================
                // Connect to the DB
                //===================================

                SQLiteConnection m_dbConnection = _ConnectToDB(NewDBFile); // connect to SQLite DB file path - returns connection data


                //=====================================================
                // Create New Table If It Does NOT Exist Yet
                //=====================================================

                bool TableExist = _DoesTableExist(NewDBFile, TableName);  //See if selected Table Exists in SQLite DB file

                if (!TableExist)  // Table DOES NOT exist in SQLite DB
                {
                    string NewTableLine = "";

                    if (TableName == "HotStrings") { NewTableLine = "FID INTEGER PRIMARY KEY, Enabled BOOL, HotString VARCHAR, HotStringValue VARCHAR"; }

                    if (TableName == "Notes") { NewTableLine = "ID INTEGER PRIMARY KEY, Public BOOL, ClientNum VARCHAR, NoteName VARCHAR, Note VARCHAR"; }

                    if (TableName == "MouseMacros") { NewTableLine = "ID INTEGER PRIMARY KEY, Flagged BOOL, MacroName VARCHAR, MouseAction VARCHAR, MouseX VARCHAR, MouseY VARCHAR, ActiveWindow VARCHAR, SearchImage VARCHAR, CurrentTime VARCHAR, PlayBack VARCHAR, ActionNum VARCHAR, Description VARCHAR, Code VARCHAR"; }

                    if (TableName == "ActiveTitleLog") { NewTableLine = "ID INTEGER PRIMARY KEY, ActiveTitle VARCHAR, CurrentTime VARCHAR"; }

                    //ahk.MsgBox(NewTableLine); 

                    bool ReturnValue = _sqlite("CREATE TABLE " + TableName + " (" + NewTableLine + ")", m_dbConnection);  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]

                    //if (TableName == "HotStrings")
                    //{
                    //    // Create Example HotString Entries
                    //    InsertHotString(".wyd", "What are you doing?", NewDBFile);
                    //    InsertHotString(".ttyl", "Talk to you later.", NewDBFile);
                    //    InsertHotString(".sys", "See you soon.", NewDBFile);
                    //}
                    //if (TableName == "Notes")
                    //{
                    //    InsertNote(true, "00000", "First Note", "Users Can Save Client Specific Notes Here.\n\nSave Notes For Personal or Group Use.", NewDBFile); 
                    //}


                }


                _DisconnectFromDB(m_dbConnection);  // free up db for other use
            }


            // Notes
            public NoteClass ReturnNoteFromID(string NoteID, string DbFile)  //Support.Desk - returns node from id number
            {
                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite._ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data

                string SearchLine = "Select * from [MouseMacros] WHERE MacroName = 'Mouse Recording'";
                //string SearchLine = "Select * from [Notes] WHERE ID = '" + NoteID + "'";
                SQLiteDataReader reader = lite._ReturnSQLite(SearchLine, m_dbConnection);  // request data from DB

                NoteClass note = new NoteClass();
                MouseMacro ms = new MouseMacro();

                while (reader.Read())    // loop through each row returned from select 
                {
                    ms.MouseAction = reader["MouseAction"].ToString();
                    //note.Public = reader["Public"].ToString();
                    //note.ClientNum = reader["ClientNum"].ToString();
                    //note.NoteName = reader["NoteName"].ToString();
                    //note.Note = reader["Note"].ToString();
                }

                lite._DisconnectFromDB(m_dbConnection);  // free up db for other use

                return note;
            }
            public bool InsertNote(bool Public, string ClientNum, string NoteName, string Note, string DbFile) //Support.Desk - inserts new note to sqlite notes table
            {
                SQLiteConnection m_dbConnection = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data
                string InsertString = "insert into Notes (Public, ClientNum, NoteName, Note) values ('" + Public + "','" + ClientNum + "','" + NoteName + "','" + Note + "')";
                bool ReturnValue = _sqlite(InsertString, m_dbConnection);  // insert into a Table
                if (ReturnValue == false) { MessageBox.Show("Failed to Insert: " + InsertString); }
                _DisconnectFromDB(m_dbConnection);  // free up db for other use
                return ReturnValue;
            }


            // List Management
            public bool InsertListItem(string DbFile, string TableName, string ListName, string ListAction, string ListValue = "", string ListOption = "", string ActionCompleted = "0", string DateNeeded = "", string ServerResponse = "")  //Support.Desk - inserts new list item into sqlite db table
            {
                SQLiteConnection m_dbConnection = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data
                                                                        //string InsertString = "INSERT into " + TableName + " (ListName, ListAction, TimeStamp, ListOption, ActionCompleted) values ('" + ListName + "','" + ListAction + "','" + DateTime.Now.ToString() + "','" + ListOption + "','" + ActionCompleted + "')";
                string InsertString = "INSERT into " + TableName + " (ListName, ListAction, ListValue, ListOption, DateNeeded, UpdateTimeStamp,ActionCompleted,ServerResponse) values ('" + ListName + "','" + ListAction + "','" + ListValue + "','" + ListOption + "','" + DateNeeded + "','" + DateTime.Now.ToString() + "','" + ActionCompleted + "','" + ServerResponse + "')";
                bool ReturnValue = _sqlite(InsertString, m_dbConnection);  // insert into a Table
                if (ReturnValue == false) { MessageBox.Show("Failed to Insert: " + InsertString); }
                _DisconnectFromDB(m_dbConnection);  // free up db for other use
                return ReturnValue;
            }
            public bool UpdateListItem(string DbFile, string TableName, string ListName, string ListItemID, string ListAction, string ListValue = "", string ListOption = "", string ActionCompleted = "0", string DateNeeded = "", string ServerResponse = "")  //Support.Desk - updates list item in sqlite db table
            {
                SQLiteConnection m_dbConnection = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data
                string UpdateString = "UPDATE " + TableName + " set ListAction = '" + ListAction + "', ListValue = '" + ListValue + "',ListOption = '" + ListOption + "', ActionCompleted = '" + ActionCompleted + "', DateNeeded = '" + DateNeeded + "', ServerResponse = '" + ServerResponse + "', UpdateTimeStamp = '" + DateTime.Now.ToString() + "' WHERE ID = '" + ListItemID + "'";
                bool ReturnValue = _sqlite(UpdateString, m_dbConnection);  // insert into a Table
                if (ReturnValue == false) { MessageBox.Show("Failed to Update: " + UpdateString); }
                _DisconnectFromDB(m_dbConnection);  // free up db for other use
                return ReturnValue;
            }
            public bool UpdateListOption(string DbFile, string TableName, string ListItemID, string ListOption)  //Support.Desk - updates list option in sqlite db table
            {
                SQLiteConnection m_dbConnection = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data
                string UpdateString = "UPDATE " + TableName + " set ListOption = '" + ListOption + "', UpdateTimeStamp = '" + DateTime.Now.ToString() + "' WHERE ID = '" + ListItemID + "'";
                bool ReturnValue = _sqlite(UpdateString, m_dbConnection);  // insert into a Table
                if (ReturnValue == false) { MessageBox.Show("Failed to Update: " + UpdateString); }
                _DisconnectFromDB(m_dbConnection);  // free up db for other use
                return ReturnValue;
            }


            public bool ClearUserList(string DbFile, string TableName, string ListName)  //Support.Desk - clears out user's list in sqlite db table
            {
                SQLiteConnection m_dbConnection = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data
                bool ReturnValue = _sqlite("Delete FROM " + TableName + " where ListName = '" + ListName + "'", m_dbConnection);  // insert into a Table
                _DisconnectFromDB(m_dbConnection);  // free up db for other use
                return ReturnValue;
            }
            public DataTable ReturnUserLists(string DbFile, string TableName = "UserLists", string DistinctField = "ListName", string WhereClause = "order by ListName asc")  //Support.Desk - returns datatable with user's list from sqlite db table
            {
                bool TableExist = _DoesTableExist(DbFile, TableName);
                if (!TableExist)
                {
                    return null;
                }

                DataTable FileTable = new DataTable();
                SQLiteConnection con = new SQLiteConnection("data source=" + DbFile);
                string SearchLine = "SELECT Distinct " + DistinctField + " FROM " + TableName;
                if (WhereClause != "") { SearchLine = SearchLine + " " + WhereClause; }

                SQLiteCommand cmd = new SQLiteCommand(SearchLine, con);
                con.Open();
                cmd = con.CreateCommand();
                cmd.CommandText = SearchLine;
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);

                adapter.Fill(FileTable);
                con.Close();
                FileTable.TableName = TableName;


                return FileTable;
            }



            #endregion

            // Populate Datagrid 
            // ### Phasing Out ###
            public int _FillDataGrid(DataGridView dv, string TableName, string DbFile, string sql = "All")  // pass in table name and db connection, autopopulates dataGridView1
            {
                SQLiteConnection db = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data

                if (sql == "All")  // default option to load ALL fields in Table
                {
                    sql = "select * from '" + TableName + "';";
                }

                try
                {
                    //dv.DataSource = null;
                    //dv.Columns.Clear();

                    DataSet ds = new DataSet();
                    var da = new SQLiteDataAdapter(sql, db);  // search SQLite DB
                    da.Fill(ds);

                    // assign the DataGridView Name to Populate
                    dv.DataSource = ds.Tables[0].DefaultView;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("SQL Exception Catch Here: " + ex.ToString());
                }


                //=== update gui with grid row count ====================================
                int GridRowCount = dv.RowCount;
                //GridRowCount = GridRowCount - 1;  // adjust for additional row counted
                //lblGridCount.Text = GridRowCount.ToString() + " Shows"; //update gui with Row Count in Grid

                dv.AutoGenerateColumns = true;

                //ColumnView(1);  // Set the GridView Column Layout (all fields fields)
                //ColumnView(2);  // Set the GridView Column Layout (specific fields)

                // Resize the master DataGridView columns to fit the newly loaded data.
                dv.AutoResizeColumns();

                // Configure the details DataGridView so that its columns automatically 
                // adjust their widths when the data changes.
                dv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                //SQLITE lite = new SQLITE();
                _DisconnectFromDB(db);  // free up db for other use

                return GridRowCount;
            }
            // table actions --- PHASING OUT
            public bool _ClearTable(string DbFile, string TableName, bool Debug = true)  // clear out existing sqlite table contents
            {
                _AHK ahk = new _AHK();
                if (Debug) { if (ahk.IfNotExist(DbFile)) { ahk.MsgBox(" DbFile Not Found - Possibly have Function Backwards in Code."); return false; } }

                SQLiteConnection db = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data

                //=== Drop Table from DB File IF It Exists =========================
                bool Return = _sqlite("Delete From [" + TableName + "]", db);

                _FixDbSize(DbFile);  //restores space after deleting from tables

                return Return;
            }
            public bool _CreateNewTable(string DbFile, string TableName = "HotStrings", string NewTableLine = "", bool DropPreviousTable = false, bool CreateNEWDbFile = false)  // create new sqlite db table
            {
                _AHK ahk = new _AHK();
                if (CreateNEWDbFile == true)
                {
                    _DisconnectFromDB();
                    ahk.FileDelete(DbFile);
                }

                //========================================================
                // Create New SQLite DB (*Used First-Run*)
                //========================================================
                if (!File.Exists(DbFile)) // create database file if it doen't exist
                {
                    SQLiteConnection.CreateFile(DbFile);
                }

                // Connect to the DB
                SQLiteConnection m_dbConnection = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data


                //=====================================================
                // Create New Table If It Does NOT Exist Yet
                //=====================================================

                bool TableExist = _DoesTableExist(DbFile, TableName);  //See if selected Table Exists in SQLite DB file

                if (DropPreviousTable)  // drop existing table if found and user enables option
                {
                    if (TableExist) { _DropTable(DbFile, TableName); }
                }

                if (!TableExist)  // Table DOES NOT exist in SQLite DB
                {
                    m_dbConnection = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data

                    _sqlite("CREATE TABLE " + TableName + " (" + NewTableLine + ")", m_dbConnection);  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
                }


                // check to see if the table we want to create is there
                bool ReturnValue = _DoesTableExist(DbFile, TableName);  //See if selected Table Exists in SQLite DB file

                _DisconnectFromDB(m_dbConnection);  // free up db for other use
                return ReturnValue;
            }
            public bool _DropTable(string FileToRead, string TableToDrop)  // drop / remove table + table contents from sqlite db file
            {
                FileToRead = FileToRead.Replace("\\\"", "\\\\\\"); // read user input - add slashes for proper formmatting

                SQLiteConnection db = _ConnectToDB(FileToRead); // connect to SQLite DB file path - returns connection data


                //=== Drop Table from DB File IF It Exists =========================
                bool Return = _sqlite("Drop Table IF EXISTS [" + TableToDrop + "]", db);
                //MessageBox.Show(Return); 

                //=== Confirm Table was removed from DB File ================

                bool TableFound = _DoesTableExist(FileToRead, TableToDrop);
                if (TableFound == true)
                {
                    _DisconnectFromDB(db);  // free up db for other use
                    return false;  // false meaning the table did NOT drop, still able to locate
                }


                _DisconnectFromDB(db);  // free up db for other use

                _FixDbSize(FileToRead);  //restores space after deleting from tables

                return true;  // otherwise TRUE, dropped table
            }
            public bool _FixDbSize(string DbFile)  //restores space after deleting from tables
            {
                SQLiteConnection db = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data

                //=== Drop Table from DB File IF It Exists =========================
                bool Return = _sqlite("VACUUM;", db);

                db.Close();   // close connection to db
                GC.Collect(); // force db to release connection
                return Return;
            }
            //string NewTableLine = "ID INTEGER PRIMARY KEY, SettingName VARCHAR, SettingValue VARCHAR, Flag BOOL";
            //sqlite.CreateTable = (DbFile, "Settings", NewTableLine);
            public bool _CreateTable(string DbFile, string TableName, string NewTableLine, bool NewDbFile = false)  // create new sqlite table
            {
                _AHK ahk = new _AHK();
                if (NewDbFile) // create new database file before writing new table
                {
                    _DisconnectFromDB();
                    ahk.FileDelete(DbFile);
                }

                //========================================================
                // Create New SQLite DB (*Used First-Run*)
                //========================================================
                if (!File.Exists(DbFile)) // create database file if it doen't exist
                {
                    SQLiteConnection.CreateFile(DbFile);
                }

                //===================================
                // Connect to the DB
                //===================================

                SQLiteConnection m_dbConnection = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data


                //=====================================================
                // Create New Table If It Does NOT Exist Yet
                //=====================================================

                bool TableExist = _DoesTableExist(DbFile, TableName);  //See if selected Table Exists in SQLite DB file

                bool ReturnValue = TableExist;

                if (!TableExist)  // Table DOES NOT exist in SQLite DB
                {
                    ReturnValue = _sqlite("CREATE TABLE " + TableName + " (" + NewTableLine + ")", m_dbConnection);  // Create a Table
                }

                _DisconnectFromDB(m_dbConnection);  // free up db connection

                return ReturnValue;
            }
            // table info  *phasing out*
            public bool _DoesTableExist(string SQLiteFile, string SearchTableName)  // returns true/false if Table already exists in SQLite File
            {
                Dictionary<string, string> TableNamesDict = _ExtractTableNameList(SQLiteFile);

                if (TableNamesDict.ContainsKey(SearchTableName))
                {
                    return true;
                }

                return false;
            }
            public Dictionary<string, string> _ExtractTableNameList(string SQLiteFile)   // Returns list of TABLE NAMES
            {
                //// ex: return table names and bind to combobox ddl
                //Dictionary<string, string> TableDict = sqlite.ExtractTableNameList(SQLiteDbDir + "\\" + ddlDbName + ".SQLite");
                //ddlTableName.DataSource = new BindingSource(TableDict, null);
                //ddlTableName.DisplayMember = "Key";
                //ddlTableName.ValueMember = "Value";



                Dictionary<string, string> TableNamesDict = new Dictionary<string, string>();

                //=============================================
                // Extract TABLE Names from SQLite DB
                //=============================================

                //SQLiteFile = @"C:\Users\Jason\Google Drive\MDb\SQLiter20\SQLiter\bin\Debug\TV_Db.SQLite"; 
                //SQLiteFile = IMDbSQLite; 
                //SQLiteFile = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\IMDb_New.SQLite";
                // Initialize the connection
                SQLiteConnection conn = new SQLiteConnection("data source=" + SQLiteFile);

                // These is how you list the schema of an SQLite database
                SQLiteCommand comm = new SQLiteCommand("SELECT * FROM SQLite_master WHERE type = 'table' ORDER BY 1", conn);
                conn.Open();
                // Populate the reader
                SQLiteDataReader reader = comm.ExecuteReader();


                //=================================================
                // Parse Elements from SQLite DB File
                //=================================================

                string TableName = "";
                string DataName = "";
                string DataType = "";
                string RootPageValue = "";
                string CreateTableCode = "";
                string TableList = "";

                // Step through each row
                while (reader.Read())
                {
                    for (int a = 0; a < reader.FieldCount; a++)
                    {

                        string columnName = reader.GetName(a);  // This will give you the name of the current row's column
                        string columnValue = reader[a].ToString();  // This will give you the value of the current row's column
                                                                    //MessageBox.Show(columnName + " = " + columnValue); 


                        if (columnName == "type")
                        {
                            DataType = columnValue;  // = table
                        }

                        if (columnName == "name")
                        {
                            DataName = columnValue;  // aka the tbl_name in this case (?)
                        }

                        if (columnName == "tbl_name")
                        {
                            TableName = columnValue;
                            // TableList = TableList + Environment.NewLine + TableName;

                            if (!TableNamesDict.ContainsKey(TableName))
                            {
                                TableNamesDict.Add(TableName, TableName);
                            }
                        }

                        if (columnName == "rootpage")
                        {
                            RootPageValue = columnValue;
                        }

                        if (columnName == "sql")
                        {
                            CreateTableCode = columnValue;
                        }
                    }

                    //char[] c = { '|' };  // trim off leading | if it exists 
                    //TableList = TableList.TrimStart(c);
                }


                //MessageBox.Show("[" + TableList + "]"); 
                _DisconnectFromDB(conn);

                return TableNamesDict;
            }
            public List<string> _TableList(string SQLiteFile)   // Returns list of TABLE NAMES
            {
                List<string> TableListOut = new List<string>();

                //=============================================
                // Extract TABLE Names from SQLite DB
                //=============================================

                //SQLiteFile = @"C:\Users\Jason\Google Drive\MDb\SQLiter20\SQLiter\bin\Debug\TV_Db.SQLite"; 
                //SQLiteFile = IMDbSQLite; 
                //SQLiteFile = System.IO.Path.GetDirectoryName(Application.ExecutablePath) + "\\IMDb_New.SQLite";
                // Initialize the connection
                SQLiteConnection conn = new SQLiteConnection("data source=" + SQLiteFile);

                // These is how you list the schema of an SQLite database
                SQLiteCommand comm = new SQLiteCommand("SELECT * FROM SQLite_master WHERE type = 'table' ORDER BY 1", conn);
                conn.Open();
                // Populate the reader
                SQLiteDataReader reader = comm.ExecuteReader();


                //=================================================
                // Parse Elements from SQLite DB File
                //=================================================

                string TableName = "";
                string DataName = "";
                string DataType = "";
                string RootPageValue = "";
                string CreateTableCode = "";
                string TableList = "";

                // Step through each row
                while (reader.Read())
                {
                    for (int a = 0; a < reader.FieldCount; a++)
                    {

                        string columnName = reader.GetName(a);  // This will give you the name of the current row's column
                        string columnValue = reader[a].ToString();  // This will give you the value of the current row's column
                                                                    //MessageBox.Show(columnName + " = " + columnValue); 


                        if (columnName == "type")
                        {
                            DataType = columnValue;  // = table
                        }

                        if (columnName == "name")
                        {
                            DataName = columnValue;  // aka the tbl_name in this case (?)
                        }

                        if (columnName == "tbl_name")
                        {
                            TableName = columnValue;
                            // TableList = TableList + Environment.NewLine + TableName;

                            TableListOut.Add(TableName);

                            //if (!TableNamesDict.ContainsKey(TableName))
                            //{
                            //    TableNamesDict.Add(TableName, TableName);
                            //}
                        }

                        if (columnName == "rootpage")
                        {
                            RootPageValue = columnValue;
                        }

                        if (columnName == "sql")
                        {
                            CreateTableCode = columnValue;
                        }
                    }

                    //char[] c = { '|' };  // trim off leading | if it exists 
                    //TableList = TableList.TrimStart(c);
                }


                //MessageBox.Show("[" + TableList + "]"); 
                _DisconnectFromDB(conn);

                return TableListOut;
            }
            public int _ColumnCount(string SQLiteFile, string TableName)  //Returns COLUMN COUNT in a Table
            {
                // Initialize the connection
                SQLiteConnection conn = new SQLiteConnection("data source=" + SQLiteFile);

                // These is how you list the schema of an SQLite database
                SQLiteCommand comm = new SQLiteCommand("SELECT * FROM SQLite_master WHERE type = 'table' ORDER BY 1", conn);
                conn.Open();
                // Populate the reader
                SQLiteDataReader reader = comm.ExecuteReader();

                //========================================================================
                // Loop through TABLE name to extract COLUMN names / column COUNT
                //========================================================================

                var cmd = new SQLiteCommand("select * from '" + TableName + "'", conn);
                var dr = cmd.ExecuteReader();
                int ColumnCount = 0;

                for (var i = 0; i < dr.FieldCount; i++)
                {
                    //MessageBox.Show(dr.GetName(i));
                    ColumnCount++;
                }


                //MessageBox.Show("ColumnCount = " + ColumnCount);

                _DisconnectFromDB(conn);  // free up db for other use

                return ColumnCount;
            }
            public Dictionary<string, string> _ExtractColumnNames(string SQLiteFile, string TableName)  //Returns COLUMN NAMES in a Table
            {
                Dictionary<string, string> ColumnNamesDict = new Dictionary<string, string>();

                // Initialize the connection
                SQLiteConnection conn = new SQLiteConnection("data source=" + SQLiteFile);

                // These is how you list the schema of an SQLite database
                SQLiteCommand comm = new SQLiteCommand("SELECT * FROM SQLite_master WHERE type = 'table' ORDER BY 1", conn);
                conn.Open();
                // Populate the reader
                SQLiteDataReader reader = comm.ExecuteReader();

                // Loop through TABLE name to extract COLUMN names / column COUNT

                var cmd = new SQLiteCommand("select * from '" + TableName + "'", conn);
                var dr = cmd.ExecuteReader();

                for (var i = 0; i < dr.FieldCount; i++)
                {
                    string Column = dr.GetName(i);

                    if (!ColumnNamesDict.ContainsKey(Column))
                    {
                        ColumnNamesDict.Add(Column, dr.GetDataTypeName(i).ToString());
                    }

                }

                _DisconnectFromDB(conn);  // free up db for other use

                return ColumnNamesDict;
            }
            public List<string> _ColumnList(string SQLiteFile, string TableName)
            {
                List<string> ColumnNames = new List<string>();

                // Initialize the connection
                SQLiteConnection conn = new SQLiteConnection("data source=" + SQLiteFile);

                // These is how you list the schema of an SQLite database
                SQLiteCommand comm = new SQLiteCommand("SELECT * FROM SQLite_master WHERE type = 'table' ORDER BY 1", conn);
                conn.Open();
                // Populate the reader
                SQLiteDataReader reader = comm.ExecuteReader();

                //========================================================================
                // Loop through TABLE name to extract COLUMN names / column COUNT
                //========================================================================

                var cmd = new SQLiteCommand("select * from '" + TableName + "'", conn);
                var dr = cmd.ExecuteReader();

                for (var i = 0; i < dr.FieldCount; i++)
                {
                    string Column = dr.GetName(i);
                    //MessageBox.Show(dr.GetName(i));
                    //ColumnCount++;
                    //ColumnNames = ColumnNames + Environment.NewLine + Column;

                    ColumnNames.Add(Column);
                }

                _DisconnectFromDB(conn);  // free up db for other use

                return ColumnNames;
            }  //Returns COLUMN NAMES in a List
               //ex: 
               //CreateUserDb(ServerDb, "SupportDeskServer");  CreateUserDb(ServerDb, "SupportDeskFileServer");
               //string NewServerVersion = "4"; 
               //WriteSetting(ServerDb, "SupportDeskServer", "ServerVersionNum", NewServerVersion);  //write new server version number
               //WriteSetting(UserDb, "UserSettings", "CurrentVersion", CurrentVersion);  //write current user version number
               //string CurrentServerVersion = ReturnSetting(ServerDb, "TableName", "ServerVersionNum", "ServerVersionNum"); 

            //=== phasing out
            // Write Setting (Update or Insert if New)
            public bool _WriteSetting(string DbFile, string TableName, string Setting, string Value, string ReturnValue = "")  //Support.Desk - writes setting to sqlite setting table
            {
                //string TableName = "SupportDeskServer"; 

                // Create New Table If It Does NOT Exist Yet
                bool TableExist = _DoesTableExist(DbFile, TableName);  //See if selected Table Exists in SQLite DB file
                if (!TableExist) { _Create_SettingsDb(DbFile, TableName); }  // Table DOES NOT exist in SQLite DB - Create Now

                SQLiteConnection m_dbConnection = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data

                // UPDATE or INSERT Server Files
                bool Updated = _sqlite("UPDATE " + TableName + " set Value = '" + Value + "', ReturnValue = '" + ReturnValue + "',TimeStamp = '" + DateTime.Now.ToString() + "' WHERE Setting = '" + Setting + "'", m_dbConnection);  // Update Table
                bool ReturnBool = true;
                if (!Updated) { ReturnBool = _sqlite("INSERT into " + TableName + " (Setting, Value, ReturnValue, TimeStamp) values ('" + Setting + "','" + Value + "','" + ReturnValue + "','" + DateTime.Now.ToString() + "')", m_dbConnection); }  // insert into a Table
                if (ReturnBool == false) { MessageBox.Show("Failed to Insert: " + Setting + "' | '" + Value + " | " + ReturnValue + "' | '" + DateTime.Now.ToString()); }
                return ReturnBool;
            }
            //=== phasing out
            // Return Setting Value from SQLite Table
            public string _ReturnSetting(string DbFile, string TableName, string Setting, bool UseReturnValue = false)  //Support.Desk - returns setting from sqlite Settings table
            {
                //string TableName = "SupportDeskServer";

                _Database.SQLite lite = new _Database.SQLite();
                SQLiteConnection m_dbConnection = lite._ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data

                string SearchLine = "Select * from " + TableName + " WHERE Setting = '" + Setting + "'";
                SQLiteDataReader reader = lite._ReturnSQLite(SearchLine, m_dbConnection);  // request data from DB

                string Value = "";
                string ReturnValue = "";
                while (reader.Read())    // loop through each row returned from select 
                {
                    string TimeStampString = reader["TimeStamp"].ToString();
                    //TimeStamp = DateTime.Parse(TimeStampString);
                    Value = reader["Value"].ToString();
                    ReturnValue = reader["ReturnValue"].ToString();
                }

                lite._DisconnectFromDB(m_dbConnection);  // free up db for other use

                string ReturnText = Value.ToString();
                if (UseReturnValue == true) { ReturnText = ReturnValue.ToString(); }

                return ReturnText;
            }
            //=== phasing out
            public void _Create_SettingsDb(string NewDBFile, string TableName = "Settings", bool OverWriteOld = false)  //Support.Desk - creates UserSettings sqlite db table structure
            {
                _AHK ahk = new _AHK();
                if (OverWriteOld)
                {
                    _DisconnectFromDB();
                    ahk.FileDelete(NewDBFile);
                    ahk.Sleep(250);
                }

                // Create New SQLite DB (*Used First-Run*)
                if (!File.Exists(NewDBFile)) // create database file if it doen't exist
                {
                    SQLiteConnection.CreateFile(NewDBFile);
                }

                // Connect to the DB
                SQLiteConnection m_dbConnection = _ConnectToDB(NewDBFile); // connect to SQLite DB file path - returns connection data


                // Create New Table If It Does NOT Exist Yet
                bool TableExist = _DoesTableExist(NewDBFile, TableName);  //See if selected Table Exists in SQLite DB file

                if (!TableExist)  // Table DOES NOT exist in SQLite DB
                {

                    string NewTableLine = "ID INTEGER PRIMARY KEY, Setting VARCHAR UNIQUE, Value VARCHAR, Option VARCHAR, TimeStamp VARCHAR";

                    //ahk.MsgBox(NewTableLine); 

                    bool ReturnValue = _sqlite("CREATE TABLE [" + TableName + "] (" + NewTableLine + ")", m_dbConnection);  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
                }


                _DisconnectFromDB(m_dbConnection);  // free up db for other use
            }
            public SQLiteConnection _ConnectToDB(string NewDBFile) // connect to SQLite DB file path - returns connection data
            {
                //// EXAMPLE USE:
                ////===================================
                //// Connect to the DB
                ////===================================
                ////SQLiteConnection m_dbConnection = ConnectToDB(NewDBFile); // connect to SQLite DB file path - returns connection data

                SQLiteConnection m_dbConnection;
                m_dbConnection = new SQLiteConnection("Data Source=" + NewDBFile + ";Version=3;");
                m_dbConnection.Open();

                return m_dbConnection;
            }
            public void _DisconnectFromDB(SQLiteConnection db = null) // disconnects from local DB connection (opens file up to edit in other locations)
            {
                SQLiteConnection.ClearAllPools();

                if (db != null)
                {
                    db.Close(); //disconnect from database file
                }

                GC.Collect(); //empty garbage files
                GC.WaitForPendingFinalizers(); //wait for those garbage files to finish before proceeding
            }
            // Write to SQLite --- REPLACING WITH 'EXECUTE'
            public bool _sqlite(string SQLLine, SQLiteConnection db, bool DebugMode = true)  //returns true/false with success, used to execute sqlite commands
            {
                //// EXAMPLE USE:
                ////=============================================
                //// SQLite("insert into filelist (filepath, score) values ('" + file + "', 9001)", db);  // insert into a Table
                //// SQLite("CREATE TABLE filelist (filepath VARCHAR(20), score INT)", db);  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
                ////=============================================

                ////=============================================================
                //// UPDATE or INSERT Example
                ////=============================================================
                //bool Updated = SQLite("UPDATE HotStrings set Enabled = '" + Enabled + "', HotString = '" + HotString + "', HotStringValue = '" + HotStringValue + "' WHERE FID = '" + FID + "'", db);  // Update Table
                //if (!Updated) { Updated = SQLite("INSERT into HotStrings (Enabled,HotString,HotStringValue) VALUES ('" + Enabled + "', '" + HotString + "', '" + HotStringValue + "')", db); }  // insert into a Table

                SQLiteCommand command = new SQLiteCommand(SQLLine, db); // create command

                int RowsChanged = 0;

                try
                {
                    RowsChanged = command.ExecuteNonQuery();             // execute command
                }
                catch (Exception ex)
                {
                    string error = ex.Message;

                    if (error.Contains("already exists"))  // error msg saying it couldn't create table bc it exists - ignore
                    {
                        return true;
                    }

                    if (DebugMode == true) { MessageBox.Show(SQLLine + Environment.NewLine + Environment.NewLine + Environment.NewLine + error); }

                    if (error.Contains("database is locked"))
                    {
                        MessageBox.Show("DB File Currently LOCKED - Unable To Write To Db.");
                    }

                    return false;
                }


                //db.Close();   // close connection to db
                //GC.Collect(); // force db to release connection

                if (RowsChanged >= 1) { return true; }   // row updated successfully
                if (RowsChanged == 0) { return false; }  // no rows changed
                return false;
            }
            public bool _Execute_SQLite(string DbFile, string SQLiteCommand, bool DebugMode = false)  // executes sqlite command on sqlite db file
            {
                // Connect to the DB
                SQLiteConnection m_dbConnection = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data

                bool ReturnBool = _sqlite(SQLiteCommand, m_dbConnection, DebugMode);  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
                                                                                      //SQLite("CREATE TABLE " + TableName + " (" + NewTableLine + ")", m_dbConnection);  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
                                                                                      //SQLite("INSERT into " + TableName + " (FileFlag, FilePath, FileName, FileSize, LastAccessTime, LastWriteTime, IsReadOnly, FileExt, FileExists, DirName, FileDirectory, CreationTime, Attributes, CopyFile) values ('" + FileFlag + "', '" + FilePath + "', '" + FileName + "', '" + FileSize + "', '" + LastAccessTime + "', '" + LastWriteTime + "', '" + IsReadOnly + "', '" + FileExt + "', '" + FileExists + "', '" + DirName + "', '" + FileDirectory + "', '" + CreationTime + "', '" + Attributes + "', '" + CopyFile + "')", m_dbConnection);  // insert into a Table
                _DisconnectFromDB(m_dbConnection);  // free up db for other use
                return ReturnBool;
            }
            private void _Bulk_Insert_Example(string DbFile)  // example of bulk insert process into SQLite
            {
                SQLiteConnection conn = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data
                conn.Open();

                using (var cmd = new SQLiteCommand(conn))
                {
                    using (var transaction = conn.BeginTransaction())
                    {
                        // 100,000 inserts
                        for (var i = 0; i < 1000000; i++)
                        {
                            cmd.CommandText = "INSERT INTO Person (FirstName, LastName) VALUES ('John', 'Doe');";
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                }

                conn.Close();
            }
            public void _SQLiteBrowser(string InFile)  //function to open SQLite DB file in External Viewer
            {
                _AHK ahk = new _AHK();
                string SQLiteBrowserPath = @"C:\Users\jason\Google Drive\AHK\Lib\Tool_Library\Code_Tools\SQLiteDB_Portable\SQLiteDatabaseBrowserPortable.exe";
                if (File.Exists(SQLiteBrowserPath))
                {
                    ahk.Run(SQLiteBrowserPath, InFile);
                }
                else
                {
                    ahk.MsgBox("SQLite Browser Not Found @ " + SQLiteBrowserPath);
                }
            }
            //ex: 
            //WriteFileDb(ServerDb,"SupportDeskFileServer", @"C:\Google Drive\IMDB\SQLiter\SupportDesk", "Support Desk", "1"); 
            //WriteFileDb(UserDb,"SupportDeskUserFiles", @"C:\Google Drive\IMDB\SQLiter\SupportDesk\bin\Debug", "Support Desk", "1"); 

            //DataTable ServerFiles = ReturnFileTable(ServerDb, "SupportDeskFileServer"); 
            //DataTable SupportDeskUserFiles = ReturnFileTables(UserDb, "SupportDeskUserFiles"); 
            //string ServerVersion = sqlite.ReturnSetting(ServerDb, "SupportDeskServer", "ServerVersionNum");
            //string CurrentVersion = sqlite.ReturnSetting(UserDb, "UserSettings", "CurrentVersion");
            public void _WriteFileDb(string DbFile, string Dir, string TableName = "SupportDeskFileServer", string FileTag = "", string FileFlag = "")  // writes file database to SupportDesk sqlite db file
            {
                // confirm import directory exists - otherwise return error message to user
                if (!Directory.Exists(Dir)) { MessageBox.Show("Directory Not Found" + "\n" + "Unable to Import: " + Dir); return; }

                // Create New Table If It Does NOT Exist Yet
                bool TableExist = _DoesTableExist(DbFile, TableName);  //See if selected Table Exists in SQLite DB file
                if (!TableExist) { CreateUserDb(DbFile, TableName); }  // Table DOES NOT exist in SQLite DB - Create Now

                // Get list of files in the specific directory - Recursive File Search - Fill Data Table
                string[] files = Directory.GetFiles(Dir, "*.*", SearchOption.AllDirectories);
                foreach (string file in files)  // loop through list of files and write file details to SQLite db
                {
                    _InsertFile(DbFile, TableName, file, FileTag, FileFlag);
                }
            }
            public bool _InsertFile(string DbFile, string FullFilePath, string TableName = "SupportDeskFileServer", string FileTag = "", string FileFlag = "")  // inserts new file into SupportDesk sqlite db file
            {
                _AHK ahk = new _AHK();
                SQLiteConnection m_dbConnection = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data
                System.IO.FileInfo fileinfo = new System.IO.FileInfo(FullFilePath); //retrieve info about each file

                string FileName = fileinfo.Name.ToString();
                string FileSize = fileinfo.Length.ToString();
                string LastWriteTime = fileinfo.LastWriteTime.ToString();
                string LastAccessTime = fileinfo.LastAccessTime.ToString();
                bool IsReadOnly = fileinfo.IsReadOnly;

                string FileExt = fileinfo.Extension.ToString();
                bool FileExists = fileinfo.Exists;
                string DirName = fileinfo.DirectoryName.ToString();
                string FileDirectory = fileinfo.Directory.ToString();
                string CreationTime = fileinfo.CreationTime.ToString();
                string Attributes = fileinfo.Attributes.ToString();
                string FilePath = FullFilePath;

                DirName = ahk.FixSpecialChars(DirName);
                FileDirectory = ahk.FixSpecialChars(FileDirectory);
                FileName = ahk.FixSpecialChars(FileName);
                FilePath = ahk.FixSpecialChars(FilePath);

                string[] paths = DirName.Split('\\'); // split the directory path to get the name
                DirName = paths[paths.Length - 1]; //returns last folder name in string


                // UPDATE or INSERT Server Files
                bool Updated = _sqlite("UPDATE SupportDeskFileServer set FullPath = '" + FilePath + "', Directory = '" + FileDirectory + "',FileName = '" + FileName + "', FileSize = '" + FileSize + "', DateModified = '" + LastWriteTime + "', FileTag ='" + FileTag + "', FileFlag = '" + FileFlag + "' WHERE FullPath = '" + FilePath + "'", m_dbConnection);  // Update Table
                bool ReturnBool = true;
                if (!Updated) { ReturnBool = _sqlite("INSERT into SupportDeskFileServer (FullPath, Directory, FileName, FileSize, DateModified, FileTag, FileFlag) values ('" + FilePath + "','" + FileDirectory + "','" + FileName + "','" + FileSize + "','" + LastWriteTime + "','" + FileTag + "','" + FileFlag + "')", m_dbConnection); }  // insert into a Table
                if (ReturnBool == false) { MessageBox.Show("Failed to Insert: " + FilePath + "' | '" + FileDirectory + " | " + FileName + "' | '" + FileSize + "' | '" + LastWriteTime + "' | '" + FileTag); }
                return ReturnBool;
            }
            public DataTable _ReturnFileTable(string DbFile, string TableName = "SupportDeskFileServer", string WhereClause = "")  // returns datatable from Support Desk file table
            {
                DataTable FileTable = new DataTable();
                SQLiteConnection con = new SQLiteConnection("data source=" + DbFile);
                string SearchLine = "SELECT * FROM " + TableName;
                if (WhereClause != "") { SearchLine = SearchLine + " " + WhereClause; }

                SQLiteCommand cmd = new SQLiteCommand(SearchLine, con);
                con.Open();
                cmd = con.CreateCommand();
                cmd.CommandText = SearchLine;
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd);
                adapter.Fill(FileTable);
                con.Close();
                FileTable.TableName = TableName;


                return FileTable;
            }
            // Dir 2 Db  - v1 
            public void _DirToSQLiteDB(string Dir, string NewDBFile, string Options = "", string TableName = "FileIndex")  // creates a "File Index" of a directory, storing file info to sqlite database table
            {
                _AHK ahk = new _AHK();
                //string Dir = "C:\\DMS";
                //NewDBFile = "c:\\DMS.SQLite";

                bool WriteLastModifiedOnly = false;

                if (Options.Contains("LastModOnly")) { WriteLastModifiedOnly = true; } //option to only write files modified today

                if (Options.Contains("NewDBFile=True"))
                {
                    _DisconnectFromDB();
                    ahk.FileDelete(NewDBFile);
                }

                //========================================================
                // Create New SQLite DB (*Used First-Run*)
                //========================================================
                if (!File.Exists(NewDBFile)) // create database file if it doen't exist
                {
                    SQLiteConnection.CreateFile(NewDBFile);
                }

                if (!Directory.Exists(Dir))  // confirm import directory exists - otherwise return error message to user
                {
                    MessageBox.Show("Directory Not Found" + "\n" + "Unable to Import: " + Dir);
                    return;
                }


                //===================================
                // Connect to the DB
                //===================================

                SQLiteConnection m_dbConnection = _ConnectToDB(NewDBFile); // connect to SQLite DB file path - returns connection data


                //=====================================================
                // Create New Table If It Does NOT Exist Yet
                //=====================================================

                bool TableExist = _DoesTableExist(NewDBFile, TableName);  //See if selected Table Exists in SQLite DB file

                if (!TableExist)  // Table DOES NOT exist in SQLite DB
                {
                    string NewTableLine = "FID INTEGER PRIMARY KEY, FileFlag BOOL, FilePath VARCHAR(100), FileName VARCHAR, FileSize VARCHAR, LastAccessTime VARCHAR, LastWriteTime VARCHAR, IsReadOnly BOOL, FileExt VARCHAR, FileExists BOOL, DirName VARCHAR, FileDirectory VARCHAR, CreationTime VARCHAR, Attributes VARCHAR, CopyFile VARCHAR";
                    _sqlite("CREATE TABLE " + TableName + " (" + NewTableLine + ")", m_dbConnection);  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
                                                                                                       //MessageBox.Show(SelectedListBoxText + " NOT IN DB");
                }

                _DisconnectFromDB(m_dbConnection);  // free up db for other use


                //=============================================================================================
                // Get list of files in the specific directory - Recursive File Search - Fill Data Table
                //=============================================================================================

                var stopwatch = new Stopwatch();
                stopwatch.Start();


                string[] files = null;

                try
                {
                    files = Directory.GetFiles(Dir, "*.*", SearchOption.AllDirectories);
                }
                catch (UnauthorizedAccessException) { }

                if (files != null)
                {
                    using (var conn = new SQLiteConnection(@"Data Source=" + NewDBFile))
                    {
                        conn.Open();

                        using (var cmd = new SQLiteCommand(conn))
                        {
                            using (var transaction = conn.BeginTransaction())
                            {

                                foreach (string file in files)  // loop through list of files and write file details to SQLite db
                                {
                                    System.IO.FileInfo fileinfo = new System.IO.FileInfo(file); //retrieve info about each file

                                    string FileName = fileinfo.Name.ToString();
                                    string FileSize = fileinfo.Length.ToString();
                                    string LastWriteTime = fileinfo.LastWriteTime.ToString();
                                    string LastAccessTime = fileinfo.LastAccessTime.ToString();
                                    bool IsReadOnly = fileinfo.IsReadOnly;

                                    string FileExt = fileinfo.Extension.ToString();
                                    bool FileExists = fileinfo.Exists;
                                    string DirName = fileinfo.DirectoryName.ToString();
                                    string FileDirectory = fileinfo.Directory.ToString();
                                    string CreationTime = fileinfo.CreationTime.ToString();
                                    string Attributes = fileinfo.Attributes.ToString();
                                    string CopyFile = "false"; //variable used to indicate a file is queued to be copied to another location 
                                    string FilePath = file;
                                    bool FileFlag = false;

                                    DirName = ahk.FixSpecialChars(DirName); //remove invalid characters before writing
                                    FileDirectory = ahk.FixSpecialChars(FileDirectory); //remove invalid characters before writing
                                    FileName = ahk.FixSpecialChars(FileName); //remove invalid characters before writing
                                    FilePath = ahk.FixSpecialChars(FilePath); //remove invalid characters before writing

                                    string[] paths = DirName.Split('\\'); // split the directory path to get the name
                                    DirName = paths[paths.Length - 1]; //returns last folder name in string

                                    // Normal Write Mode - Write All Files
                                    if (WriteLastModifiedOnly != true)
                                    {
                                        // build collection of insert statements
                                        string InsertLine = "INSERT into " + TableName + " (FileFlag, FilePath, FileName, FileSize, LastAccessTime, LastWriteTime, IsReadOnly, FileExt, FileExists, DirName, FileDirectory, CreationTime, Attributes, CopyFile) values ('" + FileFlag + "', '" + FilePath + "', '" + FileName + "', '" + FileSize + "', '" + LastAccessTime + "', '" + LastWriteTime + "', '" + IsReadOnly + "', '" + FileExt + "', '" + FileExists + "', '" + DirName + "', '" + FileDirectory + "', '" + CreationTime + "', '" + Attributes + "', '" + CopyFile + "')";
                                        cmd.CommandText = InsertLine + ";";
                                        cmd.ExecuteNonQuery();
                                    }

                                    // Option to ONLY Write Recently Modified Files
                                    if (WriteLastModifiedOnly == true)
                                    {
                                        DateTime dateTime = DateTime.UtcNow.Date;
                                        int day = System.Convert.ToInt32(dateTime.ToString("dd"));
                                        int month = System.Convert.ToInt32(dateTime.ToString("MM"));
                                        int year = System.Convert.ToInt32(dateTime.ToString("yyyy"));

                                        //DateTime LastSyncDate = new DateTime(year, 06, day);
                                        DateTime LastSyncDate = new DateTime(2016, 06, 25);

                                        if (fileinfo.LastWriteTime > LastSyncDate)
                                        {
                                            //ahk.MsgBox("Need to Add - " + fileinfo.LastWriteTime.ToString());
                                            string InsertLine = "INSERT into " + TableName + " (FileFlag, FilePath, FileName, FileSize, LastAccessTime, LastWriteTime, IsReadOnly, FileExt, FileExists, DirName, FileDirectory, CreationTime, Attributes, CopyFile) values ('" + FileFlag + "', '" + FilePath + "', '" + FileName + "', '" + FileSize + "', '" + LastAccessTime + "', '" + LastWriteTime + "', '" + IsReadOnly + "', '" + FileExt + "', '" + FileExists + "', '" + DirName + "', '" + FileDirectory + "', '" + CreationTime + "', '" + Attributes + "', '" + CopyFile + "')";
                                            cmd.CommandText = InsertLine + ";";
                                            cmd.ExecuteNonQuery();
                                        }
                                    }

                                }

                                transaction.Commit();  //write collection of insert statements
                            }
                        }

                        conn.Close();
                    }
                }



                Console.WriteLine("{0} seconds with one transaction.", stopwatch.Elapsed.TotalSeconds);
            }
            public void _Dir_To_Db(string DbFile, string Dir, string TableName = "FileIndex", bool OverWriteTable = false)  // creates a "File Index" of a directory, storing file info to sqlite database table (v2)
            {
                _AHK ahk = new _AHK();
                //string Dir = "C:\\DMS";
                //NewDBFile = "c:\\DMS.SQLite";

                bool WriteLastModifiedOnly = false;

                // Create New SQLite DB (*Used First-Run*)
                if (!File.Exists(DbFile)) // create database file if it doen't exist
                {
                    SQLiteConnection.CreateFile(DbFile);
                }

                if (!Directory.Exists(Dir))  // confirm import directory exists - otherwise return error message to user
                {
                    MessageBox.Show("Directory Not Found" + "\n" + "Unable to Import: " + Dir);
                    return;
                }


                // Connect to the DB
                SQLiteConnection m_dbConnection = _ConnectToDB(DbFile); // connect to SQLite DB file path - returns connection data


                // Create New Table If It Does NOT Exist Yet

                bool TableExist = _DoesTableExist(DbFile, TableName);  //See if selected Table Exists in SQLite DB file

                if (TableExist)
                {
                    if (OverWriteTable)  // option to clear table contents before writing file index
                    {
                        _ClearTable(DbFile, TableName, true);  // clear out existing sqlite table contents
                    }
                }

                if (!TableExist)  // Table DOES NOT exist in SQLite DB
                {
                    string NewTableLine = "FID INTEGER PRIMARY KEY, FileFlag BOOL, FilePath VARCHAR, FileName VARCHAR, DirName VARCHAR, DirPath VARCHAR, FileExt VARCHAR, FileSize VARCHAR, FileSizeBytes VARCHAR, CreationTime VARCHAR, LastAccessTime VARCHAR, LastWriteTime VARCHAR, IsReadOnly BOOL, Attributes VARCHAR, FileExists BOOL, FileAction VARCHAR, Tags VARCHAR";
                    _sqlite("CREATE TABLE " + TableName + " (" + NewTableLine + ")", m_dbConnection);  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
                                                                                                       //MessageBox.Show(SelectedListBoxText + " NOT IN DB");
                }


                //== create directory index for storing multiple drives / folders -- tracks last modified etc ===
                TableExist = _DoesTableExist(DbFile, "Dir2Db_Index");  //See if selected Table Exists in SQLite DB file
                if (!TableExist)  // Table DOES NOT exist in SQLite DB
                {
                    string NewTableLine = "FID INTEGER PRIMARY KEY, Enabled BOOL, DirPath VARCHAR, FileCount VARCHAR, LastUpdated VARCHAR";
                    _sqlite("CREATE TABLE " + TableName + " (" + NewTableLine + ")", m_dbConnection);  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]
                }


                _DisconnectFromDB(m_dbConnection);  // free up db for other use


                //=============================================================================================
                // Get list of files in the specific directory - Recursive File Search - Fill Data Table
                //=============================================================================================

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                string[] files = Directory.GetFiles(Dir, "*.*", SearchOption.AllDirectories);
                int FileCount = 0;
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

                                string FileName = fileinfo.Name.ToString();

                                string FileSizeBytes = fileinfo.Length.ToString();
                                string FileSize = ahk.FormatBytes(fileinfo.Length);  // convert bytes to Text representation (adds kb/mb/tb to return)

                                string LastWriteTime = fileinfo.LastWriteTime.ToString();
                                string LastAccessTime = fileinfo.LastAccessTime.ToString();
                                string CreationTime = fileinfo.CreationTime.ToString();

                                bool IsReadOnly = fileinfo.IsReadOnly;
                                string FileExt = fileinfo.Extension.ToString();
                                bool FileExists = fileinfo.Exists;
                                string DirName = fileinfo.DirectoryName.ToString();
                                string DirPath = fileinfo.Directory.ToString();

                                string Attributes = fileinfo.Attributes.ToString();
                                string FileAction = ""; // variable used to indicate a file is queued to be copied to another location 
                                string FilePath = file;
                                bool FileFlag = false;

                                DirName = ahk.FixSpecialChars(DirName); //remove invalid characters before writing
                                DirPath = ahk.FixSpecialChars(DirPath); //remove invalid characters before writing
                                FileName = ahk.FixSpecialChars(FileName); //remove invalid characters before writing
                                FilePath = ahk.FixSpecialChars(FilePath); //remove invalid characters before writing

                                string[] paths = DirName.Split('\\'); // split the directory path to get the name
                                DirName = paths[paths.Length - 1]; //returns last folder name in string

                                // Normal Write Mode - Write All Files
                                if (WriteLastModifiedOnly != true)
                                {
                                    // build collection of insert statements
                                    string InsertLine = "INSERT into " + TableName + " (FileFlag, FilePath, FileName, DirName, DirPath, FileExt, FileSize, FileSizeBytes, CreationTime, LastAccessTime, LastWriteTime, IsReadOnly, Attributes, FileExists, FileAction, Tags) values ('" + FileFlag + "', '" + FilePath + "', '" + FileName + "', '" + DirName + "', '" + DirPath + "', '" + FileExt + "', '" + FileSize + "', '" + FileSizeBytes + "', '" + CreationTime + "', '" + LastAccessTime + "', '" + LastWriteTime + "', '" + IsReadOnly + "', '" + Attributes + "', '" + FileExists + "', '" + FileAction + "', '" + DirName + "')";
                                    cmd.CommandText = InsertLine + ";";
                                    cmd.ExecuteNonQuery();
                                    FileCount++;
                                }

                                // Option to ONLY Write Recently Modified Files
                                if (WriteLastModifiedOnly == true)
                                {
                                    DateTime dateTime = DateTime.UtcNow.Date;
                                    int day = System.Convert.ToInt32(dateTime.ToString("dd"));
                                    int month = System.Convert.ToInt32(dateTime.ToString("MM"));
                                    int year = System.Convert.ToInt32(dateTime.ToString("yyyy"));

                                    //DateTime LastSyncDate = new DateTime(year, 06, day);
                                    DateTime LastSyncDate = new DateTime(2016, 06, 25);

                                    if (fileinfo.LastWriteTime > LastSyncDate)
                                    {
                                        //ahk.MsgBox("Need to Add - " + fileinfo.LastWriteTime.ToString());
                                        string InsertLine = "INSERT into " + TableName + " (FileFlag, FilePath, FileName, DirName, DirPath, FileExt, FileSize, FileSizeBytes, CreationTime, LastAccessTime, LastWriteTime, IsReadOnly, Attributes, FileExists, FileAction, Tags) values ('" + FileFlag + "', '" + FilePath + "', '" + FileName + "', '" + DirName + "', '" + DirPath + "', '" + FileExt + "', '" + FileSize + "', '" + FileSizeBytes + "', '" + CreationTime + "', '" + LastAccessTime + "', '" + LastWriteTime + "', '" + IsReadOnly + "', '" + Attributes + "', '" + FileExists + "', '" + FileAction + "', '" + DirName + "')";
                                        cmd.CommandText = InsertLine + ";";
                                        cmd.ExecuteNonQuery();
                                        FileCount++;
                                    }
                                }

                            }

                            transaction.Commit();  //write collection of insert statements
                        }
                    }

                    conn.Close();
                }

                Console.WriteLine("{0} seconds with one transaction.", stopwatch.Elapsed.TotalSeconds);


                // create index entry / update existing entry if found
                string Index_InsertLine = "Insert Into [Dir2Db_Index] (Enabled, DirPath, FileCount, LastUpdated) VALUES ('true', '" + Dir + "', '" + FileCount + "', '" + DateTime.Now + "')";
                string Index_UpdateLine = "Update [Dir2Db_Index] Set Enabled = 'true', DirPath = '" + Dir + "', FileCount = '" + FileCount + "', LastUpdated = '" + DateTime.Now + "' where DirPath = '" + Dir + "'";
                _Update_Insert(DbFile, Index_UpdateLine, Index_InsertLine, false);
            }


            #endregion

        }
    }

}

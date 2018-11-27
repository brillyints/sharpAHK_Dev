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
using System.Data;
using System.IO;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;
using System.Drawing;

namespace sharpAHK_Dev
{
    public partial class _Database
    {
        /// <summary>
        /// SQL Database Function Library | LucidMedod 
        /// </summary>
        public class SQL
        {
            // sql update or insert (single command for batch processing 
            //update test set name = 'john' where id = 3012 IF @@ROWCOUNT=0 insert into test(name) values('john');


            private static bool GlobalDebug = false;

            #region === SQL Connection ===

            // Connection String in Web.Config/App.Config Example
            //<?xml version = "1.0" encoding="utf-8" ?>
            //<configuration>
            //    <startup> 
            //        <supportedRuntime version = "v4.0" sku=".NETFramework,Version=v4.5" />
            //    </startup>

            //  <connectionStrings>
            //    <add name = "T2Connect" connectionString="Server=192.xxx.xxx.xxx.;DataBase=Portal;Uid=USERNAME;Pwd=PASS;" />
            //  </connectionStrings>  



            /// <summary>
            /// Retrieves SQLConnection By Name From WebConfig/AppConfig 
            /// </summary>
            /// <param name="ConnectionName">Name of Connection in WebConfig/AppConfig</param>
            /// <returns>Returns SQLConnection, Returns NULL if the Connection Name Not Found.</returns>
            public SqlConnection GetConn(string ConnectionName)
            {
                // Assume failure.
                string returnValue = null;
                SqlConnection returnConn = null;

                // Look for the name in the connectionStrings section.
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[ConnectionName];

                // If found, return the connection string.
                if (settings != null)
                {
                    returnValue = settings.ConnectionString;
                    returnConn = new SqlConnection(returnValue);
                }

                //return returnValue;
                return returnConn;
            }


            /// <summary>
            /// Retrieves SQLConnection String By Name From WebConfig/AppConfig 
            /// </summary>
            /// <param name="ConnectionName">Name of Connection in WebConfig/AppConfig</param>
            /// <returns>Returns SQL Connection String, Returns Blank if the Connection Name Not Found.</returns>
            public string GetConnString(string ConnectionName)
            {
                // Assume failure.
                string returnValue = "";

                // Look for the name in the connectionStrings section.
                ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[ConnectionName];

                // If found, return the connection string.
                if (settings != null) { returnValue = settings.ConnectionString; }

                return returnValue;
            }



            #endregion

            #region === SQL: Return ===

            // === Return Database Values EXAMPLE ===

            // Example Looping through Database Names / Tables for Column Info
            private void btnGetSQL_Click(object sender = null, EventArgs e = null)
            {
                _AHK ahk = new _AHK();

                SqlConnection conn = GetConn("LITM");

                List<string> dbnames = Return_DbNames(conn);

                ahk.MsgBox("Found " + dbnames.Count + " Databases");

                int i = 1;
                foreach (string name in dbnames)
                {
                    //if (name.ToUpper() != "ICONLIB") { continue; }

                    string dbConnLine = @"Server=(localdb)\MyInstance;DataBase=" + name + ";Uid=namtrak;Pwd=packman;";
                    SqlConnection newConn = new SqlConnection(dbConnLine);

                    List<string> tables = Return_TableList(newConn, name, true);
                    //ahk.MsgBox("( " + i + "/" + dbnames.Count + " ) - DbName: " + name + " | " + tables.Count + " Tables"); i++;

                    int j = 1;
                    foreach (string table in tables)
                    {
                        Dictionary<string, string> cols = Return_ColumnDict(newConn, name, table);

                        //List<string> cols = sql.Return_ColumnNames(newConn, name, table);
                        //ahk.MsgBox("( " + j + "/" + tables.Count + " ) - " + table + " | " + cols.Count + " Columns"); j++;
                        //ahk.MsgBox("(" + j + "/" + tables.Count + " Tables in " + name + ") - TableName: " + table); j++;
                        //ahk.MsgBox("( " + i + "/" + dbnames.Count + " ) - DbName: " + name + " | " + tables.Count + " Tables | TableName: " + table + " | " + cols.Count + " Columns"); j++;

                        foreach (KeyValuePair<string, string> pair in cols)
                        {
                            bool known = KnownFieldType(pair.Value);

                            if (!known)
                            {
                                ahk.MsgBox("DbName: " + name + "\nTableName: " + table + "\n" + pair.Key + " = " + pair.Value);
                            }
                        }
                    }
                }

                ahk.MsgBox("DONE");
            }


            /// <summary>
            /// Check to see if database field type is recognized 
            /// </summary>
            /// <param name="FieldType"></param>
            /// <returns></returns>
            public bool KnownFieldType(string FieldType)
            {
                bool known = false;

                if (FieldType.ToUpper() == "BIT") { known = true; }
                if (FieldType.ToUpper() == "BIGINT") { known = true; }
                if (FieldType.ToUpper() == "NVARCHAR") { known = true; }
                if (FieldType.ToUpper() == "DATETIME") { known = true; }
                if (FieldType.ToUpper() == "INT") { known = true; }
                if (FieldType.ToUpper() == "VARCHAR") { known = true; }
                if (FieldType.ToUpper() == "VARBINARY") { known = true; }
                if (FieldType.ToUpper() == "NCHAR") { known = true; }
                //if (FieldType.ToUpper() == "") { known = true; }
                //if (FieldType.ToUpper() == "") { known = true; }
                //if (FieldType.ToUpper() == "") { known = true; }
                return known;
            }



            //=== Get DataTable from SQL Query

            /// <summary> Get DataTable from SQL Query</summary>
            /// <param name="conn"> </param>
            /// <param name=" query"> </param>
            public DataTable GetDataTable(SqlConnection conn, string query)
            {
                _AHK ahk = new _AHK();
                DataTable dt = new DataTable();
                DataTable table = new DataTable();

                //ahk.MsgBox(query + "\n\n" + conn.ConnectionString);

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    try
                    {
                        adapter.Fill(dt);
                    }
                    catch (Exception ex)
                    {
                        //ahk.MsgBox(ex.ToString());
                        return null;
                    }
                }

                conn.Close();
                return dt;
            }


            //== Return Database Names ===

            /// <summary> returns list of db names returned from sql connection</summary>
            /// <param name="Connection">SQL Connection to search for Database Names</param>
            /// <param name="ExcludeDefaults">Excludes Master, TempDb, Model, and MSDb from Return List</param>
            public List<string> Return_DbNames(SqlConnection Connection, bool ExcludeDefaults = true)
            {
                var DataSource = Connection.DataSource;

                // Create DataTable instance.
                DataTable ColumnDT = new DataTable("DbNames");
                //tableDT.Columns.Add("ID");
                ColumnDT.Columns.Add("DataSource");
                ColumnDT.Columns.Add("DbName");


                // connect to SQL database and return the database names from a sql connection
                DataTable dt = GetDataTable(Connection, "SELECT name from sys.databases");

                List<string> DbNames = new List<string>();

                if (dt != null)
                {
                    foreach (DataRow datarow in dt.Rows)
                    {
                        string DbName = datarow["name"].ToString();

                        if (ExcludeDefaults)
                        {
                            if (DbName.ToUpper() == "MASTER" || DbName.ToUpper() == "TEMPDB" || DbName.ToUpper() == "MODEL" || DbName.ToUpper() == "MSDB") { continue; }
                        }

                        DbNames.Add(DbName);
                    }
                }


                return DbNames;
            }

            /// <summary> returns datatable of db names from sql connection</summary>
            /// <param name="Connection"> </param>
            public DataTable Return_DbNamesDT(SqlConnection Connection)
            {
                var DataSource = Connection.DataSource;

                // Create DataTable instance.
                DataTable ColumnDT = new DataTable("DbNames");
                //tableDT.Columns.Add("ID");
                ColumnDT.Columns.Add("DataSource");
                ColumnDT.Columns.Add("DbName");


                // connect to SQL database and return the database names from a sql connection
                DataTable dt = GetDataTable(Connection, "SELECT name from sys.databases");

                List<string> ColumnNames = new List<string>();

                if (dt != null)
                {
                    foreach (DataRow datarow in dt.Rows)
                    {
                        string DbName = datarow["name"].ToString();

                        ColumnDT.Rows.Add(DataSource.ToString(), DbName);
                    }
                }

                return ColumnDT;
            }


            //=== Return Column Names ===

            /// <summary> returns list of column names from sql table</summary>
            /// <param name="Connection"> </param>
            /// <param name=" DataBaseName">If blank, defaults to database in connection string </param>
            /// <param name=" TableName"> </param>
            public List<string> Return_ColumnNames(SqlConnection Connection, string DataBaseName = "", string TableName = "")
            {
                if (DataBaseName == "") { DataBaseName = Connection.Database; }

                if (TableName.Contains("[")) // parse out table name if full path provided
                {
                    TableName = TableName_From_FullDbPath(TableName); 
                }

                ////======================================
                //// Return List of SQL Column Names
                ////======================================
                // Ex: 
                //  List<string> SQLColumnNames = sql.Return_ColumnNames(T2SS, "IMCRMData", "CRMPicklist");
                //  foreach (string Column in SQLColumnNames)
                //  {
                //      ahk.MsgBox(Column);
                //  }
                // ddlTableName.DataSource = SQLTableNames;  // Bind List to ComboBox

                // connect to SQL database and return the column structure for a table
                // SELECT * FROM IMCRMData.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'CRMPicklist'     /* Returns info about a SQL Table Structure */
                DataTable dt = GetDataTable(Connection, "SELECT * FROM " + DataBaseName + ".INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + TableName + "'");

                List<string> ColumnNames = new List<string>();

                if (dt != null)
                {
                    foreach (DataRow datarow in dt.Rows)
                    {
                        string ColumnName = datarow["COLUMN_NAME"].ToString();
                        string Position = datarow["ORDINAL_POSITION"].ToString();
                        string DataType = datarow["DATA_TYPE"].ToString();

                        //if (DataType == "bit") { DataType = "BOOL"; }

                        ColumnNames.Add(ColumnName); //save column name to list
                    }
                }

                return ColumnNames;
            }


            /// <summary>
            /// Convert SQL Column Type to C# Variable Type Needed in Code
            /// </summary>
            /// <param name="varType"></param>
            /// <returns></returns>
            public string SQLVarType(string varType)
            {
                string retType = "";

                if (varType.ToLower() == "nvarchar") { retType = "string"; }
                if (varType.ToLower() == "varchar") { retType = "string"; }
                if (varType.ToLower() == "bool") { retType = "bool"; }
                if (varType.ToLower() == "datetime") { retType = "DateTime"; }
                if (varType.ToLower() == "int") { retType = "int"; }
                if (varType.ToLower() == "varbinary") { retType = "binary"; }
                //if (varType.ToLower() == "") { retType = ""; }
                //if (varType.ToLower() == "") { retType = ""; }

                return retType;
            }

            /// <summary>
            /// Returns Dictionary of Columns with ColumnName / ColumnType Format
            /// </summary>
            /// <param name="Connection"></param>
            /// <param name="DataBaseName"></param>
            /// <param name="TableName"></param>
            /// <returns></returns>
            public Dictionary<string, string> Return_ColumnDict(SqlConnection Connection, string DataBaseName = "", string TableName = "")
            {
                if (DataBaseName == "") { DataBaseName = Connection.Database; }

                if (TableName.Contains("[")) // parse out table name if full path provided
                {
                    TableName = TableName_From_FullDbPath(TableName);
                }


                Dictionary<string, string> Dict = new Dictionary<string, string>();

                ////======================================
                //// Return List of SQL Column Names
                ////======================================
                // Ex: 
                //  List<string> SQLColumnNames = sql.Return_ColumnNames(T2SS, "IMCRMData", "CRMPicklist");
                //  foreach (string Column in SQLColumnNames)
                //  {
                //      ahk.MsgBox(Column);
                //  }
                // ddlTableName.DataSource = SQLTableNames;  // Bind List to ComboBox

                // connect to SQL database and return the column structure for a table
                // SELECT * FROM IMCRMData.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'CRMPicklist'     /* Returns info about a SQL Table Structure */
                DataTable dt = GetDataTable(Connection, "SELECT * FROM " + DataBaseName + ".INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + TableName + "'");

                List<string> ColumnNames = new List<string>();

                if (dt != null)
                {
                    foreach (DataRow datarow in dt.Rows)
                    {
                        string ColumnName = datarow["COLUMN_NAME"].ToString();
                        string Position = datarow["ORDINAL_POSITION"].ToString();
                        string DataType = datarow["DATA_TYPE"].ToString();

                        //if (DataType == "bit") { DataType = "BOOL"; }

                        Dict.Add(ColumnName, DataType);
                        ColumnNames.Add(ColumnName); //save column name to list
                    }
                }

                return Dict;
            }

            /// <summary> returns datatable with list of column names in sql table</summary>
            /// <param name="Connection"> </param>
            /// <param name=" DataBaseName"> </param>
            /// <param name=" TableName"> </param>
            public DataTable Return_ColumnNamesDT(SqlConnection Connection, string DataBaseName, string TableName)
            {
                ////=========================================================
                //// Return List of SQL Column Names - Populate DataGrid
                ////=========================================================
                //// Ex: 
                //DataTable ColumnList = sql.Return_ColumnNamesDT(T2SS, "CRMReporting", "WorkTable");
                //dataGridView1.DataSource = ColumnList;

                var dbName = Connection.Database; // pull DB Name from Connection String
                var DataSource = Connection.DataSource; // pull Connection IP


                string ReturnType = "ColumnInfo";
                // (ReturnType == "ColumnPath") =  datasource / dbname / tablename / columnname 


                if (ReturnType == "ColumnInfo")
                {
                    // column name / type

                    // Create DataTable instance.
                    DataTable ColumnDT = new DataTable("Columns");
                    ColumnDT.Columns.Add("ColumnName");
                    ColumnDT.Columns.Add("ColumnType");


                    // connect to SQL database and return the column structure for a table
                    // SELECT * FROM IMCRMData.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'CRMPicklist'     /* Returns info about a SQL Table Structure */
                    DataTable dt = GetDataTable(Connection, "SELECT * FROM " + DataBaseName + ".INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + TableName + "'");

                    List<string> ColumnNames = new List<string>();

                    if (dt != null)
                    {
                        foreach (DataRow datarow in dt.Rows)
                        {
                            string ColumnName = datarow["COLUMN_NAME"].ToString();
                            string Position = datarow["ORDINAL_POSITION"].ToString();
                            string DataType = datarow["DATA_TYPE"].ToString();

                            //if (DataType == "bit") { DataType = "BOOL"; }

                            ColumnDT.Rows.Add(ColumnName, DataType);
                        }
                    }

                    return ColumnDT;
                }


                if (ReturnType == "ColumnPath")
                {
                    // datasource / dbname / tablename / columnname 

                    // Create DataTable instance.
                    DataTable ColumnDT = new DataTable("Columns");
                    //tableDT.Columns.Add("ID");
                    ColumnDT.Columns.Add("DataSource");
                    ColumnDT.Columns.Add("DbName");
                    ColumnDT.Columns.Add("TableName");
                    ColumnDT.Columns.Add("ColumnName");


                    // connect to SQL database and return the column structure for a table
                    // SELECT * FROM IMCRMData.INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'CRMPicklist'     /* Returns info about a SQL Table Structure */
                    DataTable dt = GetDataTable(Connection, "SELECT * FROM " + DataBaseName + ".INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = N'" + TableName + "'");

                    List<string> ColumnNames = new List<string>();

                    if (dt != null)
                    {
                        foreach (DataRow datarow in dt.Rows)
                        {
                            string ColumnName = datarow["COLUMN_NAME"].ToString();
                            string Position = datarow["ORDINAL_POSITION"].ToString();
                            string DataType = datarow["DATA_TYPE"].ToString();

                            //if (DataType == "bit") { DataType = "BOOL"; }

                            //ColumnNames.Add(ColumnName); //save column name to list
                            ColumnDT.Rows.Add(DataSource.ToString(), dbName.ToString(), TableName, ColumnName);
                        }
                    }

                    return ColumnDT;
                }


                return null;
            }


            // === Search SQL Database, Return List of Values From Search Results ===

            /// <summary>Search SQL Database, Return List of Values From Search Results</summary>
            /// <param name="Connection"> </param>
            /// <param name=" SQLSearch"> </param>
            public List<string> SQL_To_List(SqlConnection Connection, string SQLSearch)
            {
                DataTable dt = GetDataTable(Connection, SQLSearch);

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

            // === Search SQL Database, Return First Value From Search Results ===

            /// <summary>Search SQL Database, Return First Value From Search Results</summary>
            /// <param name="Connection"> </param>
            /// <param name=" SQLSearch"> </param>
            public string SQL_Return_Value(SqlConnection Connection, string SQLSearch)
            {
                DataTable dt = GetDataTable(Connection, SQLSearch);
                string ReturnValue = "";

                if (dt != null)
                {
                    foreach (DataRow datarow in dt.Rows)
                    {
                        ReturnValue = datarow[0].ToString();
                        break;
                    }
                }

                return ReturnValue;
            }

            /// <summary>Search SQL Database - Return Count as Int</summary>
            /// <param name="SQL Connection"> </param>
            /// <param name="SQLSearch">SQL Command Returning Count</param>
            public int Count(SqlConnection Connection, string SQLSearch)
            {
                DataTable dt = GetDataTable(Connection, SQLSearch);
                string ReturnValue = "";

                if (dt != null)
                {
                    foreach (DataRow datarow in dt.Rows)
                    {
                        ReturnValue = datarow[0].ToString();
                        break;
                    }
                }

                _AHK ahk = new _AHK();
                return ahk.ToInt(ReturnValue);
            }


            //=== Return Table Names ===

            /// <summary> returns list of tables from sql database</summary>
            /// <param name="Connection"> </param>
            /// <param name="DbName"> </param>
            /// <param name="FullPath">Option to return entire Database Name + Table Name</param>
            public List<string> Return_TableList(SqlConnection Connection, string DbName = "", bool FullPath = false)
            {
                ////======================================
                //// Return List of SQL Table Names
                ////======================================
                //// EX: 
                //List<string> SQLTableNames = sql.Return_TableList(T2SS);
                //foreach (string TableName in SQLTableNames)
                //{
                //    ahk.MsgBox(TableName);
                //}
                // ddlTableName.DataSource = SQLTableNames;  // Bind List to ComboBox

                if (Connection.State == ConnectionState.Closed) { Connection.Open(); }

                //Connection.

                //if (DbName != "") { Connection.co = DbName; }

                DataTable schema = Connection.GetSchema("Tables");
                List<string> TableNames = new List<string>();

                if (schema != null)
                {
                    foreach (DataRow row in schema.Rows)
                    {
                        string dbName = row[0].ToString();
                        string userName = row[1].ToString();
                        string tablename = row[2].ToString();

                        //MessageBox.Show(row[0].ToString() + "\n" + row[1].ToString() + "\n" + row[2].ToString() + "\n" + row[3].ToString());
                        if (FullPath)
                        {
                            string fullPath = "[" + dbName + "].[" + userName + "].[" + tablename + "]";

                            if (DbName != "")
                            {
                                if (DbName.Trim().ToUpper() == dbName.Trim().ToUpper())
                                {
                                    TableNames.Add(fullPath);
                                }
                            }
                            else
                            {
                                TableNames.Add(fullPath);
                            }

                        }

                        if (!FullPath)
                        {
                            if (DbName != "")
                            {
                                if (DbName.Trim().ToUpper() == dbName.Trim().ToUpper())
                                {
                                    TableNames.Add(tablename);
                                }
                            }
                            else
                            {
                                TableNames.Add(tablename);
                            }
                        }
                    }
                }

                Connection.Close();
                return TableNames;
            }

            /// <summary> returns datatable with sql table names</summary>
            /// <param name="Connection"> </param>
            public DataTable Return_TableListDT(SqlConnection Connection)
            {
                var dbName = Connection.Database; // pull DB Name from Connection String
                var DataSource = Connection.DataSource; // pull Connection IP

                ////=========================================================
                //// Return List of SQL Table Names - Populate DataGrid
                ////=========================================================
                //// Ex: 
                //      DataTable TableList = sql.Return_TableListDT(T2SS);
                //      dataGridView1.DataSource = TableList; 


                // Create DataTable instance.
                DataTable tableDT = new DataTable("Tables");
                tableDT.Columns.Add("DataSource");
                tableDT.Columns.Add("DB");
                tableDT.Columns.Add("TableName");

                if (Connection.State == ConnectionState.Closed) { Connection.Open(); }
                DataTable schema = Connection.GetSchema("Tables");
                List<string> TableNames = new List<string>();
                int RowCount = 1;
                if (schema != null)
                {
                    foreach (DataRow row in schema.Rows)
                    {
                        //TableNames.Add(row[2].ToString());
                        //tableDT.Rows.Add(RowCount, row[2].ToString());
                        tableDT.Rows.Add(DataSource.ToString(), dbName.ToString(), row[2].ToString());
                        RowCount++;
                    }
                }

                Connection.Close();
                return tableDT;
            }


            /// <summary>Populate combobox with sql table names</summary>
            /// <param name="Connection">SQL Connection</param>
            /// <param name="cb">Combobox control to populate </param>
            /// <param name="DbName">Database name used by SQL connection (optional)</param>
            public void SQL_TableList_ToComboBox(SqlConnection Connection, ComboBox cb, string DbName = "")
            {
                List<string> SQLTableNames = Return_TableList(Connection, DbName);
                cb.DataSource = SQLTableNames;  // Bind List to ComboBox
            }


            //#### Parse SQL #####


            // takes text copied from SQL Management Studio and extracts the column names (Create New SQL Table Text)
            public Dictionary<string, string> parseColumnsFromSQLCreateTable(string CreateTableTxt)
            {
                _Lists lst = new _Lists();
                _AHK ahk = new _AHK();

                List<string> lines = lst.Text_To_List(CreateTableTxt, true, true, false);

                Dictionary<string, string> columns = new Dictionary<string, string>();

                foreach (string line in lines)
                {
                    if (line.Contains("/***")) { continue; } // skip comment lines

                    if (line.ToUpper().Contains("CREATE TABLE"))
                    {
                        string tableName = ahk.StringSplit(line, "(", 0);
                        List<string> segs = ahk.StringSplit_List(line, "[");
                        string tableNAME = "";
                        foreach (string seg in segs) { tableNAME = seg; }  // get last section in loop
                        tableNAME = tableNAME.Replace("]", "");
                        tableNAME = tableNAME.Replace("(", "");
                        // columns.Add(tableNAME);
                        //ahk.MsgBox(tableNAME);
                    }
                    else
                    {
                        string colName = ahk.StringSplit(line, "]", 0);
                        colName = colName.Replace("[", "");


                        //[AccountName] [varchar] (max) NOT NULL,

                        string colType = ahk.StringSplit(line, "]", 1);
                        colType = colType.Replace("[", "");


                        if (colName.ToUpper().Contains("ON PRIMARY")) { return columns; }

                        columns.Add(colName, colType);

                        //ahk.MsgBox(colName);
                    }

                }

                return columns;
            }

            // takes text copied from SQL Management Studio and extracts the column names (Select Top 1000 SQL Text)
            public Dictionary<string, string> parseColumnsFromSQLSelect(string CreateTableTxt)
            {
                _Lists lst = new _Lists();
                _AHK ahk = new _AHK();

                List<string> lines = lst.Text_To_List(CreateTableTxt, true, true, false);

                Dictionary<string, string> columns = new Dictionary<string, string>();

                foreach (string line in lines)
                {
                    if (line.Contains("/***")) { continue; } // skip comment lines

                    string colName = line.Replace("]", "");
                    colName = colName.Replace(",[", "");

                    if (line.ToUpper().Contains("SELECT TOP "))
                    {
                        colName = ahk.StringSplit(colName, "[", 1);
                    }

                    if (colName.ToUpper().Contains("FROM [")) { return columns; }

                    columns.Add(colName, "string");
                    //ahk.MsgBox(tableNAME);
                }

                return columns;
            }



            /// <summary>
            /// Extract table name from full db table path (ex: [LucidMedia].[lucidmethod].[IndexTable] = IndexTable) 
            /// </summary>
            /// <param name="FullDbPath">ex: [LucidMedia].[lucidmethod].[IndexTable]</param>
            /// <returns></returns>
            public string TableName_From_FullDbPath(string FullDbPath)
            {
                _AHK ahk = new _AHK();

                // Split string into new list, separated by split character
                List<string> Parts = ahk.StringSplit_List(FullDbPath, "[");
                int sL = 0;
                string tableName = "";
                foreach (string Part in Parts)
                {
                    if (sL == 2) { tableName = Part.Replace("]", ""); }
                    sL++; // loop counter
                }

                return tableName;
            }

            /// <summary>
            /// Extract user name from full db table path (ex: [LucidMedia].[lucidmethod].[IndexTable] = lucidmethod) 
            /// </summary>
            /// <param name="FullDbPath">ex: [LucidMedia].[lucidmethod].[IndexTable]</param>
            /// <returns></returns>
            public string UserName_From_FullDbPath(string FullDbPath)
            {
                _AHK ahk = new _AHK();

                // Split string into new list, separated by split character
                List<string> Parts = ahk.StringSplit_List(FullDbPath, "[");
                int sL = 0;
                string userName = "";
                foreach (string Part in Parts)
                {
                    if (sL == 1) { userName = Part.Replace("]", ""); }
                    sL++; // loop counter
                }

                return userName;
            }

            /// <summary>
            /// Extract database name from full db table path (ex: [LucidMedia].[lucidmethod].[IndexTable] = LucidMedia) 
            /// </summary>
            /// <param name="FullDbPath">ex: [LucidMedia].[lucidmethod].[IndexTable]</param>
            /// <returns></returns>
            public string Database_From_FullDbPath(string FullDbPath)
            {
                _AHK ahk = new _AHK();

                // Split string into new list, separated by split character
                List<string> Parts = ahk.StringSplit_List(FullDbPath, "[");
                int sL = 0;
                string dbName = "";
                foreach (string Part in Parts)
                {
                    if (sL == 0) { dbName = Part.Replace("]", ""); }
                    sL++; // loop counter
                }

                return dbName;
            }




            //=== Return File from SQL Field ========

            /// <summary> read file contents saved in sql table containing file</summary>
            /// <param name="ServerFileName"> </param>
            /// <param name=" NewFilePath"> </param>
            public void databaseFileRead(SqlConnection SQLConn, string ServerFileName, string NewFilePath)
            {
                if (SQLConn.State == ConnectionState.Closed) { SQLConn.Open(); }
                //using (var varConnection = Locale.sqlConnectOneTime(Locale.sqlDataConnectionDetails))
                using (var sqlQuery = new SqlCommand(@"SELECT [FileContents] FROM [MediaServer].[lucid].[FileBin] WHERE [FileName] = @FileName", SQLConn))
                {
                    sqlQuery.Parameters.AddWithValue("@FileName", ServerFileName);
                    using (var sqlQueryResult = sqlQuery.ExecuteReader())
                        if (sqlQueryResult != null)
                        {
                            sqlQueryResult.Read();
                            var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
                            sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);
                            using (var fs = new FileStream(NewFilePath, FileMode.Create, FileAccess.Write))
                                fs.Write(blob, 0, blob.Length);
                        }
                }

                SQLConn.Close();
            }

            /// <summary> returns file from sql database, saves to local file path</summary>
            /// <param name="SQLConn"> </param>
            /// <param name="SQLSearchLine"> </param>
            /// <param name="LocalFilePath"> </param>
            public void ReturnFile(SqlConnection SQLConn, string SQLSearchLine, string LocalFilePath)
            {
                //string SQLSearchLine = "SELECT [FileContents] FROM [MediaServer].[lucid].[FileBin] WHERE [FileName] = 'TestFile.zip'";

                if (SQLConn.State == ConnectionState.Closed) { SQLConn.Open(); }
                using (var sqlQuery = new SqlCommand(SQLSearchLine, SQLConn))
                {
                    //sqlQuery.Parameters.AddWithValue("@FileName", ServerFileName);
                    using (var sqlQueryResult = sqlQuery.ExecuteReader())
                        if (sqlQueryResult != null)
                        {
                            sqlQueryResult.Read();
                            var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
                            sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);
                            using (var fs = new FileStream(LocalFilePath, FileMode.Create, FileAccess.Write))
                                fs.Write(blob, 0, blob.Length);
                        }
                }

                SQLConn.Close();
            }

            /// <summary>returns file from sql database, saves to local file path</summary>
            /// <param name="SQLConn"> </param>
            /// <param name=" SQLSearchLine"> </param>
            /// <param name="LocalFilePath"> </param>
            public Image ReturnImage(SqlConnection SQLConn, string SQLSearchLine, string LocalFilePath = "")
            {
                //string SQLSearchLine = "SELECT [FileContents] FROM [MediaServer].[lucid].[FileBin] WHERE [FileName] = 'TestFile.zip'";
                _AHK ahk = new _AHK();

                bool DeleteTmpFile = false;
                if (LocalFilePath == "") { DeleteTmpFile = true; LocalFilePath = ahk.AppDir() + "\\IconTemp.png"; }

                if (SQLConn.State == ConnectionState.Closed) { SQLConn.Open(); }
                using (var sqlQuery = new SqlCommand(SQLSearchLine, SQLConn))
                {
                    //sqlQuery.Parameters.AddWithValue("@FileName", ServerFileName);
                    using (var sqlQueryResult = sqlQuery.ExecuteReader())
                        if (sqlQueryResult != null)
                        {
                            sqlQueryResult.Read();
                            var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
                            sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);
                            using (var fs = new FileStream(LocalFilePath, FileMode.Create, FileAccess.Write))
                                fs.Write(blob, 0, blob.Length);
                        }
                }

                SQLConn.Close();

                //Image returnImg = img.GetCopyImage(LocalFilePath);  // convert local return img to Image format

                Image returnImg = Image.FromFile(LocalFilePath);

                //if (DeleteTmpFile) { ahk.FileDelete(LocalFilePath); }  // delete local temp image

                return returnImg;
            }



            #endregion

            #region === SQL: Write ===

            //=== Write Data to SQL Db

            /// <summary> write data to sql table</summary>
            /// <param name="conn"> </param>
            /// <param name=" query"> </param>
            public bool WriteDataRecord(SqlConnection conn, string query)
            {
                try
                {
                    SqlCommand cmd = new SqlCommand(query, conn);
                    if (conn.State == ConnectionState.Closed) { conn.Open(); }
                    cmd.ExecuteNonQuery();
                    conn.Close();
                    return true;
                }
                catch (SqlException SqlEx)
                {
                    sharpAHK._AHK ahk = new sharpAHK._AHK();
                    //if (GlobalDebug == true) { ahk.MsgBox(SqlEx.ToString()); }
                    conn.Close();
                    return false;
                }
                catch (Exception Ex)
                {
                    sharpAHK._AHK ahk = new sharpAHK._AHK();
                    //if (GlobalDebug == true) { ahk.MsgBox(Ex.ToString()); }
                    conn.Close();
                    return false;
                }

            }


            //=== Write File to SQL Field ========

            /// <summary> writes local file to sql table (file contents stored)</summary>
            /// <param name="SQLConn"> </param>
            /// <param name=" TableName"> </param>
            /// <param name=" FilePath"> </param>
            public void databaseFilePut(SqlConnection SQLConn, string TableName, string FilePath)
            {
                //You have to create table with VARBINARY(MAX) as one of the columns

                //TableName = "[MediaServer].[lucid].[FileBin]";

                FileInfo info = new FileInfo(FilePath);
                string FileName = info.Name;
                string FileSize = info.Length.ToString();
                string DateModified = info.LastWriteTime.ToString();

                if (SQLConn.State == ConnectionState.Closed) { SQLConn.Open(); }

                byte[] file;
                using (var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        file = reader.ReadBytes((int)stream.Length);
                    }
                }
                //using (var varConnection = Locale.sqlConnectOneTime(Locale.sqlDataConnectionDetails))

                using (var sqlWrite = new SqlCommand("INSERT INTO " + TableName + " (DateModified, FileName, FileSize, FileContents) Values(@DateModified, @FileName, @FileSize, @FileContents)", SQLConn))
                {
                    sqlWrite.Parameters.AddWithValue("@FileName", FileName);
                    sqlWrite.Parameters.AddWithValue("@FileSize", FileSize);
                    sqlWrite.Parameters.AddWithValue("@DateModified", DateModified);
                    sqlWrite.Parameters.Add("@FileContents", SqlDbType.VarBinary, file.Length).Value = file;
                    sqlWrite.ExecuteNonQuery();
                }

                SQLConn.Close();
            }

            /// <summary> writes local file path to sql table (file contents stored)</summary>
            /// <param name="SQLConn"> </param>
            /// <param name=" TableName"> </param>
            /// <param name=" FilePath"> </param>
            public void WriteFile(SqlConnection SQLConn, string TableName, string FilePath)
            {
                //You have to create table with VARBINARY(MAX) as one of the columns

                //TableName = "[MediaServer].[lucid].[FileBin]";

                FileInfo info = new FileInfo(FilePath);
                string FileName = info.Name;
                string FileSize = info.Length.ToString();
                string DateModified = info.LastWriteTime.ToString();

                if (SQLConn.State == ConnectionState.Closed) { SQLConn.Open(); }

                // convert file to binary to store in sql db
                byte[] file;
                using (var stream = new FileStream(FilePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        file = reader.ReadBytes((int)stream.Length);
                    }
                }

                using (var sqlWrite = new SqlCommand("INSERT INTO " + TableName + " (DateModified, FileName, FileSize, FileContents) Values(@DateModified, @FileName, @FileSize, @FileContents)", SQLConn))
                {
                    sqlWrite.Parameters.AddWithValue("@FileName", FileName);
                    sqlWrite.Parameters.AddWithValue("@FileSize", FileSize);
                    sqlWrite.Parameters.AddWithValue("@DateModified", DateModified);
                    sqlWrite.Parameters.AddWithValue("@FileContents", file); //write file binary to sql table
                                                                             //sqlWrite.Parameters.Add("@FileContents", file);
                                                                             //sqlWrite.Parameters.Add("@FileContents", SqlDbType.VarBinary, file.Length).Value = file;
                    sqlWrite.ExecuteNonQuery();
                }

                SQLConn.Close();
            }


            // !!! NOT FINISHED --- MORE DETAILED WriteFile Action (?) 
            private void FileWrite(string filePath)
            {
                System.IO.FileInfo fileinfo = new System.IO.FileInfo(filePath); //retrieve info about each file
                _AHK ahk = new _AHK();

                if (fileinfo.Exists)
                {
                    //string FileName = fileinfo.Name.ToString();

                    string FileSizeBytes = fileinfo.Length.ToString();  // # of bytes as an int
                    string FileSize = ahk.FormatBytes(fileinfo.Length);  // convert bytes to Text representation (adds kb/mb/tb to return)

                    string LastWriteTime = fileinfo.LastWriteTime.ToString();
                    string LastAccessTime = fileinfo.LastAccessTime.ToString();
                    string CreationTime = fileinfo.CreationTime.ToString();

                    bool IsReadOnly = fileinfo.IsReadOnly;
                    string FileExt = fileinfo.Extension.ToString();
                    FileExt = ahk.StringReplace(FileExt, "."); // remove period from file ext

                    bool FileExists = fileinfo.Exists;
                    //string DirName = fileinfo.DirectoryName.ToString();
                    //string dirPath = fileinfo.Directory.ToString();

                    string Attributes = fileinfo.Attributes.ToString();
                    string FileAction = ""; // variable used to indicate a file is queued to be copied to another location 
                    string FilePath = filePath;
                    bool FileFlag = false;

                    //DirName = ahk.FixSpecialChars(DirName); //remove invalid characters before writing
                    //DirPath = ahk.FixSpecialChars(DirPath); //remove invalid characters before writing
                    //FileName = ahk.FixSpecialChars(FileName); //remove invalid characters before writing
                    FilePath = ahk.FixSpecialChars(FilePath);//remove invalid characters before writing
                    string DirName = ahk.FixSpecialChars(ahk.DirName(filePath));
                    string FileName = ahk.FixSpecialChars(ahk.FileName(filePath));
                    string dirPath = ahk.FixSpecialChars(ahk.FileDir(filePath));

                    string fileHash = ahk.FileHash(filePath);  // returns hash for file 

                }
            }

            /// <summary> </summary>
            /// <param name="Conn"> </param>
            /// <param name=" SQLLine"> </param>
            public bool Write_SQL(SqlConnection Conn, string SQLLine)
            {
                //string SQLLine = "Update [CZData].[dbo].[CZProjects] SET LastUpdated = @LastUpdated, AdditionalComments = @AdditionalComments, Alignment = @Alignment, Benefit = @Benefit, BudgetSummary = @BudgetSummary, BudgetVariance = @BudgetVariance, BudgetVariancePercent = @BudgetVariancePercent, BusinessAlignment = @BusinessAlignment, BusinessImpact = @BusinessImpact, ClosingNotes = @ClosingNotes, ExpectedBusinessValue = @ExpectedBusinessValue, ExpectedROI = @ExpectedROI, GoalsAchieved = @GoalsAchieved, HoldingNotes = @HoldingNotes, InvestmentType = @InvestmentType, Justification = @Justification, Mitigation = @Mitigation, OverallSummary = @OverallSummary, PlannedSavings = @PlannedSavings, isPortfolio = @isPortfolio, Risks = @Risks, ProjectGoals = @ProjectGoals, ProjectManager = @ProjectManager, ProjectSize = @ProjectSize, ProjectSponsor = @ProjectSponsor, ProjectTagging = @ProjectTagging, ProjectType = @ProjectType, RemainingBudget = @RemainingBudget, Required = @Required, RisksImpact = @RisksImpact, RisksRate = @RisksRate, RisksTotalScore = @RisksTotalScore, RollupFinancialAndEffortDataFromShortcut = @RollupFinancialAndEffortDataFromShortcut, RollupProgressAndDatesFromShortcut = @RollupProgressAndDatesFromShortcut, ScheduleSummary = @ScheduleSummary, Score = @Score, C_AnnualSupport = @C_AnnualSupport, C_AnnualSupportRemaining = @C_AnnualSupportRemaining, C_BillableEvent = @C_BillableEvent, C_BillableType = @C_BillableType, C_BilledDate = @C_BilledDate, C_BlockedWaitingReason = @C_BlockedWaitingReason, C_CancellationRisk = @C_CancellationRisk, C_ClientNumber = @C_ClientNumber, C_CompletedByUser = @C_CompletedByUser, C_ContractDate = @C_ContractDate, C_CustomerContact = @C_CustomerContact, C_CustomerName1 = @C_CustomerName1, C_DocumentID = @C_DocumentID, C_EnrollmentTimeAddedToProject = @C_EnrollmentTimeAddedToProject, C_ERAFrequency = @C_ERAFrequency, C_Exclude = @C_Exclude, C_FrontEndInstalled = @C_FrontEndInstalled, C_hardwareallocated = @C_hardwareallocated, C_HMSSessionandHMSLoginCredentials = @C_HMSSessionandHMSLoginCredentials, C_IDSUBID = @C_IDSUBID, C_InstallProvisioningRemaining = @C_InstallProvisioningRemaining, C_IsBillable = @C_IsBillable, C_ItemsCompleted = @C_ItemsCompleted, C_LicenseFee = @C_LicenseFee, C_LOB = @C_LOB, C_MasterDataCustomerID = @C_MasterDataCustomerID, C_MonthlyAlreadyBilled = @C_MonthlyAlreadyBilled, C_MonthlyServiceAccessFee = @C_MonthlyServiceAccessFee, C_MonthlyServiceAccessRemaining = @C_MonthlyServiceAccessRemaining, C_Transactio = @C_Transactio, C_MonthlyTransactionVolume = @C_MonthlyTransactionVolume, C_MovingtoSSCDate = @C_MovingtoSSCDate, C_NetworkPaths = @C_NetworkPaths, C_NonCustomerProjectTime = @C_NonCustomerProjectTime, C_NotonBacklog = @C_NotonBacklog, C_NumberOfUsers = @C_NumberOfUsers, C_NumberofVirtualMachines = @C_NumberofVirtualMachines, C_OnCIP = @C_OnCIP, C_OpportunityType = @C_OpportunityType, C_PostingStatus = @C_PostingStatus, C_PrerequisitesMet = @C_PrerequisitesMet, C_ProductCode = @C_ProductCode, C_ProductFamily = @C_ProductFamily, C_ProductionPostDate = @C_ProductionPostDate, C_ProgrammerName = @C_ProgrammerName, C_ProgrammerNotes = @C_ProgrammerNotes, C_ProjectGoLiveDateNew = @C_ProjectGoLiveDateNew, C_ProjectNotes = @C_ProjectNotes, C_ProjectedGoLiveDate = @C_ProjectedGoLiveDate, C_AlreadyBilling = @C_AlreadyBilling, C_ReceivingRemits = @C_ReceivingRemits, C_RegistrationSSR = @C_RegistrationSSR, C_RelatedChanges = @C_RelatedChanges, C_RelatedIssues = @C_RelatedIssues, C_RelatedRisks = @C_RelatedRisks, C_ReleasedDate = @C_ReleasedDate, C_AddedX = @C_AddedX, C_ReviewedInclude = @C_ReviewedInclude, C_SalesRep = @C_SalesRep, C_ScriptTestDates = @C_ScriptTestDates, C_SEDISCurrentMonthlyFee = @C_SEDISCurrentMonthlyFee, C_SEDISProvisioningAlreadyBilled = @C_SEDISProvisioningAlreadyBilled, C_SFTPConfiguration = @C_SFTPConfiguration, C_SignoffReceivedDate = @C_SignoffReceivedDate, C_SignoffSentDate = @C_SignoffSentDate, C_SSCCBOName = @C_SSCCBOName, C_SSR = @C_SSR, C_TM = @C_TM, C_ToleranceSituation = @C_ToleranceSituation, C_TotalServiceAccessFeeValue = @C_TotalServiceAccessFeeValue, C_TotalTransactionFeeValue = @C_TotalTransactionFeeValue, C_TransactionCodes = @C_TransactionCodes, C_UndevelopedPayers = @C_UndevelopedPayers, CreatedOn = @CreatedOn WHERE CreatedOn = '" + createdOn + "'";

                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);

                cmd2 = new SqlCommand(SQLLine, Conn);

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }

                int recordsAffected = 0;

                try { recordsAffected = cmd2.ExecuteNonQuery(); }

                catch (SqlException ex) { MessageBox.Show(ex.ToString()); }

                Conn.Close();

                if (recordsAffected > 0) { return true; }

                else return false;
            }


            // write list of sql commands in batch - returns # of updated rows (untested)
            public int WriteSQL_List(SqlConnection Conn, List<string> SQLLines)
            {

                //string SQLLine = "Update [CZData].[dbo].[CZProjects] SET LastUpdated = @LastUpdated, AdditionalComments = @AdditionalComments, Alignment = @Alignment, Benefit = @Benefit, BudgetSummary = @BudgetSummary, BudgetVariance = @BudgetVariance, BudgetVariancePercent = @BudgetVariancePercent, BusinessAlignment = @BusinessAlignment, BusinessImpact = @BusinessImpact, ClosingNotes = @ClosingNotes, ExpectedBusinessValue = @ExpectedBusinessValue, ExpectedROI = @ExpectedROI, GoalsAchieved = @GoalsAchieved, HoldingNotes = @HoldingNotes, InvestmentType = @InvestmentType, Justification = @Justification, Mitigation = @Mitigation, OverallSummary = @OverallSummary, PlannedSavings = @PlannedSavings, isPortfolio = @isPortfolio, Risks = @Risks, ProjectGoals = @ProjectGoals, ProjectManager = @ProjectManager, ProjectSize = @ProjectSize, ProjectSponsor = @ProjectSponsor, ProjectTagging = @ProjectTagging, ProjectType = @ProjectType, RemainingBudget = @RemainingBudget, Required = @Required, RisksImpact = @RisksImpact, RisksRate = @RisksRate, RisksTotalScore = @RisksTotalScore, RollupFinancialAndEffortDataFromShortcut = @RollupFinancialAndEffortDataFromShortcut, RollupProgressAndDatesFromShortcut = @RollupProgressAndDatesFromShortcut, ScheduleSummary = @ScheduleSummary, Score = @Score, C_AnnualSupport = @C_AnnualSupport, C_AnnualSupportRemaining = @C_AnnualSupportRemaining, C_BillableEvent = @C_BillableEvent, C_BillableType = @C_BillableType, C_BilledDate = @C_BilledDate, C_BlockedWaitingReason = @C_BlockedWaitingReason, C_CancellationRisk = @C_CancellationRisk, C_ClientNumber = @C_ClientNumber, C_CompletedByUser = @C_CompletedByUser, C_ContractDate = @C_ContractDate, C_CustomerContact = @C_CustomerContact, C_CustomerName1 = @C_CustomerName1, C_DocumentID = @C_DocumentID, C_EnrollmentTimeAddedToProject = @C_EnrollmentTimeAddedToProject, C_ERAFrequency = @C_ERAFrequency, C_Exclude = @C_Exclude, C_FrontEndInstalled = @C_FrontEndInstalled, C_hardwareallocated = @C_hardwareallocated, C_HMSSessionandHMSLoginCredentials = @C_HMSSessionandHMSLoginCredentials, C_IDSUBID = @C_IDSUBID, C_InstallProvisioningRemaining = @C_InstallProvisioningRemaining, C_IsBillable = @C_IsBillable, C_ItemsCompleted = @C_ItemsCompleted, C_LicenseFee = @C_LicenseFee, C_LOB = @C_LOB, C_MasterDataCustomerID = @C_MasterDataCustomerID, C_MonthlyAlreadyBilled = @C_MonthlyAlreadyBilled, C_MonthlyServiceAccessFee = @C_MonthlyServiceAccessFee, C_MonthlyServiceAccessRemaining = @C_MonthlyServiceAccessRemaining, C_Transactio = @C_Transactio, C_MonthlyTransactionVolume = @C_MonthlyTransactionVolume, C_MovingtoSSCDate = @C_MovingtoSSCDate, C_NetworkPaths = @C_NetworkPaths, C_NonCustomerProjectTime = @C_NonCustomerProjectTime, C_NotonBacklog = @C_NotonBacklog, C_NumberOfUsers = @C_NumberOfUsers, C_NumberofVirtualMachines = @C_NumberofVirtualMachines, C_OnCIP = @C_OnCIP, C_OpportunityType = @C_OpportunityType, C_PostingStatus = @C_PostingStatus, C_PrerequisitesMet = @C_PrerequisitesMet, C_ProductCode = @C_ProductCode, C_ProductFamily = @C_ProductFamily, C_ProductionPostDate = @C_ProductionPostDate, C_ProgrammerName = @C_ProgrammerName, C_ProgrammerNotes = @C_ProgrammerNotes, C_ProjectGoLiveDateNew = @C_ProjectGoLiveDateNew, C_ProjectNotes = @C_ProjectNotes, C_ProjectedGoLiveDate = @C_ProjectedGoLiveDate, C_AlreadyBilling = @C_AlreadyBilling, C_ReceivingRemits = @C_ReceivingRemits, C_RegistrationSSR = @C_RegistrationSSR, C_RelatedChanges = @C_RelatedChanges, C_RelatedIssues = @C_RelatedIssues, C_RelatedRisks = @C_RelatedRisks, C_ReleasedDate = @C_ReleasedDate, C_AddedX = @C_AddedX, C_ReviewedInclude = @C_ReviewedInclude, C_SalesRep = @C_SalesRep, C_ScriptTestDates = @C_ScriptTestDates, C_SEDISCurrentMonthlyFee = @C_SEDISCurrentMonthlyFee, C_SEDISProvisioningAlreadyBilled = @C_SEDISProvisioningAlreadyBilled, C_SFTPConfiguration = @C_SFTPConfiguration, C_SignoffReceivedDate = @C_SignoffReceivedDate, C_SignoffSentDate = @C_SignoffSentDate, C_SSCCBOName = @C_SSCCBOName, C_SSR = @C_SSR, C_TM = @C_TM, C_ToleranceSituation = @C_ToleranceSituation, C_TotalServiceAccessFeeValue = @C_TotalServiceAccessFeeValue, C_TotalTransactionFeeValue = @C_TotalTransactionFeeValue, C_TransactionCodes = @C_TransactionCodes, C_UndevelopedPayers = @C_UndevelopedPayers, CreatedOn = @CreatedOn WHERE CreatedOn = '" + createdOn + "'";

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }

                int TotalRecordsAffected = 0;
                foreach (string SQLLine in SQLLines)
                {

                    SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);

                    cmd2 = new SqlCommand(SQLLine, Conn);

                    int recordsAffected = 0;

                    try
                    {
                        recordsAffected = cmd2.ExecuteNonQuery();
                    }
                    catch (SqlException ex) { MessageBox.Show(ex.ToString()); }

                    TotalRecordsAffected += recordsAffected;
                }

                Conn.Close();

                return TotalRecordsAffected;
            }


            // Write DataTable to SQL Db (Much Faster Than Single Inserts)
            public void SQL_BatchInsert(SqlConnection Conn, string TableName = "[codeserver].[lucidmethod].[CodeTable]", DataTable tableInsert = null)
            {
                // connect to SQL
                using (SqlConnection connection = Conn)
                {
                    // make sure to enable triggers
                    // more on triggers in next post
                    SqlBulkCopy bulkCopy =
                        new SqlBulkCopy
                        (
                        connection,
                        SqlBulkCopyOptions.TableLock |
                        SqlBulkCopyOptions.FireTriggers |
                        SqlBulkCopyOptions.UseInternalTransaction,
                        null
                        );

                    // set the destination table name
                    bulkCopy.DestinationTableName = TableName;
                    connection.Open();

                    // write the data in the "dataTable"
                    bulkCopy.WriteToServer(tableInsert);
                    connection.Close();
                }
                // reset
                tableInsert.Clear();

            }


            #endregion

            #region === SQL: Tables ===

            //== Check to see if Table Exist

            /// <summary>Checks to See if SQL Table Name Exists in Db</summary>
            /// <param name="Connection"> </param>
            /// <param name=" TableName"> </param>
            public bool TableExist(SqlConnection Connection, string TableName)
            {
                // check to see if a sql table already exists

                string tableName = TableName;

                if (TableName.Contains("["))  // parse out just the database name if more than that is present
                {
                    string[] words = TableName.Split('[');
                    foreach (string word in words)
                    {
                        tableName = word;
                    }

                    sharpAHK._AHK ahk = new sharpAHK._AHK();
                    tableName = ahk.StringReplace(tableName, "]");
                }

                List<string> SQLTableNames = Return_TableList(Connection);  // return list of sql tables
                foreach (string TableNameFound in SQLTableNames)  // check each table name to see if it mataches user input
                {
                    if (tableName.ToUpper().Trim() == TableNameFound.ToUpper().Trim()) { return true; }
                }

                return false;
            }

            //== Create Table


            // UNNECESSARY FUNCTION -- WRITE COMMAND DOES THE SAME THING - To Remove !!!

            /// <summary> create new SQL table</summary>
            /// <param name="conn"> </param>
            /// <param name=" NewTableName"> </param>
            /// <param name=" NewTableLine"> </param>
            public bool CreateNewTable(SqlConnection conn, string NewTableName, string NewTableLine)
            {
                //// create new SQL Table
                //string NewTableName = "[MediaServer].[lucid].[EpGuides3]";
                //string NewTableLine = "([ShowName] [nvarchar](max) NULL,[EpName] [nvarchar](max) NULL, [SyncDate] [datetime] NULL, [Flagged] [bit] NULL, [FileContents] [varbinary](max) NULL ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                //bool Created = CreateNewTable(conn, NewTableName, NewTableLine);

                string CreateTableLine = "CREATE TABLE " + NewTableName + NewTableLine;
                bool ReturnVal = WriteDataRecord(conn, CreateTableLine);
                return ReturnVal;
            }

            #endregion

            #region === SQL: FileIndex Db ===

            //        public void LucidMedia_CreateFileIndex(string TableName = "FileIndex")  // creates sql table if it doesn't exist
            //        {
            //            bool TableExists = TableExist(GoDad, TableName);

            //            if (!TableExists)
            //            {
            //                bool Created = Create_LucidMedia_DirToDb(null, "[LucidMedia].[dbo].[" + TableName + "]");
            //                //ahk.MsgBox("Created = " + Created.ToString());
            //            }
            //            else
            //                ahk.MsgBox("Table Exists = " + TableExists.ToString());
            //        }

            //        //== Create Dir-To-Db SQL Table
            //        public bool Create_LucidMedia_DirToDb(SqlConnection conn = null, string NewTableName = "[LucidMedia].[dbo].[FileIndex]")  // create new SQL table
            //        {
            //            if (conn == null) { conn = GoDad; }

            //            //// create new SQL Table
            //            string NewTableSetup = @"CREATE TABLE " + NewTableName + @"(
            //	[FID] [int] IDENTITY(1,1) NOT NULL,
            //	[FileFlag] [bit] NULL,
            //	[FilePath] [varchar](MAX) NOT NULL,
            //	[FileName] [varchar](300) NULL,
            //	[FileSize] [varchar](100) NULL,
            //	[LastAccessTime] [datetime] NULL,
            //	[LastWriteTime] [datetime] NULL,
            //	[IsReadOnly] [bit] NULL,
            //	[FileExt] [varchar](50) NULL,
            //	[FileExists] [bit] NULL,
            //	[DirName] [varchar](MAX) NULL,
            //	[FileDirectory] [varchar](MAX) NULL,
            //	[CreationTime] [datetime] NULL,
            //	[Attributes] [varchar](MAX) NULL,
            //	[FileAction] [varchar](MAX) NULL,
            //	[Tags] [varchar](MAX) NULL,
            //) ON [PRIMARY]";

            //            bool ReturnVal = WriteDataRecord(conn, NewTableSetup);
            //            return ReturnVal;
            //        }

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

            /// <summary> </summary>
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
                try { dv.Columns["FileSize"].Visible = true; }
                catch { }
                try { dv.Columns["LastAccessTime"].Visible = true; }
                catch { }
                try { dv.Columns["LastWriteTime"].Visible = true; }
                catch { }
                try { dv.Columns["IsReadOnly"].Visible = true; }
                catch { }
                try { dv.Columns["FileExt"].Visible = true; }
                catch { }
                try { dv.Columns["FileExists"].Visible = true; }
                catch { }
                try { dv.Columns["DirName"].Visible = true; }
                catch { }
                try { dv.Columns["FileDirectory"].Visible = true; }
                catch { }
                try { dv.Columns["CreationTime"].Visible = true; }
                catch { }
                try { dv.Columns["Attributes"].Visible = true; }
                catch { }
                try { dv.Columns["FileAction"].Visible = true; }
                catch { }
                try { dv.Columns["Tags"].Visible = true; }
                catch { }
            }

            /// <summary> </summary>
            /// <param name="Conn"> </param>
            /// <param name=" TableName"> </param>
            /// <param name=" _FileIndex obj"> </param>
            public bool Insert_SQL_FileIndex(SqlConnection Conn, string TableName, _FileIndex obj)
            {
                string SQLLine = "Insert Into " + TableName + " (FileFlag, FilePath, FileName, FileSize, LastAccessTime, LastWriteTime, IsReadOnly, FileExt, FileExists, DirName, FileDirectory, CreationTime, Attributes, FileAction, Tags) VALUES (@FileFlag, @FilePath, @FileName, @FileSize, @LastAccessTime, @LastWriteTime, @IsReadOnly, @FileExt, @FileExists, @DirName, @FileDirectory, @CreationTime, @Attributes, @FileAction, @Tags)";

                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);

                cmd2 = new SqlCommand(SQLLine, Conn);

                if (obj.FileFlag == null) { obj.FileFlag = ""; }
                if (obj.FilePath == null) { obj.FilePath = ""; }
                if (obj.FileName == null) { obj.FileName = ""; }
                if (obj.FileSize == null) { obj.FileSize = ""; }
                if (obj.LastAccessTime == null) { obj.LastAccessTime = ""; }
                if (obj.LastWriteTime == null) { obj.LastWriteTime = ""; }
                if (obj.IsReadOnly == null) { obj.IsReadOnly = ""; }
                if (obj.FileExt == null) { obj.FileExt = ""; }
                if (obj.FileExists == null) { obj.FileExists = ""; }
                if (obj.DirName == null) { obj.DirName = ""; }
                if (obj.FileDirectory == null) { obj.FileDirectory = ""; }
                if (obj.CreationTime == null) { obj.CreationTime = ""; }
                if (obj.Attributes == null) { obj.Attributes = ""; }
                if (obj.FileAction == null) { obj.FileAction = ""; }
                if (obj.Tags == null) { obj.Tags = ""; }


                cmd2.Parameters.AddWithValue(@"FileFlag", obj.FileFlag.ToString());
                cmd2.Parameters.AddWithValue(@"FilePath", obj.FilePath.ToString());
                cmd2.Parameters.AddWithValue(@"FileName", obj.FileName.ToString());
                cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize.ToString());
                cmd2.Parameters.AddWithValue(@"LastAccessTime", obj.LastAccessTime.ToString());
                cmd2.Parameters.AddWithValue(@"LastWriteTime", obj.LastWriteTime.ToString());
                cmd2.Parameters.AddWithValue(@"IsReadOnly", obj.IsReadOnly.ToString());
                cmd2.Parameters.AddWithValue(@"FileExt", obj.FileExt.ToString());
                cmd2.Parameters.AddWithValue(@"FileExists", obj.FileExists.ToString());
                cmd2.Parameters.AddWithValue(@"DirName", obj.DirName.ToString());
                cmd2.Parameters.AddWithValue(@"FileDirectory", obj.FileDirectory.ToString());
                cmd2.Parameters.AddWithValue(@"CreationTime", obj.CreationTime.ToString());
                cmd2.Parameters.AddWithValue(@"Attributes", obj.Attributes.ToString());
                cmd2.Parameters.AddWithValue(@"FileAction", obj.FileAction.ToString());
                cmd2.Parameters.AddWithValue(@"Tags", obj.Tags.ToString());

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }

                int recordsAffected = 0;

                try { recordsAffected = cmd2.ExecuteNonQuery(); }

                catch (SqlException ex) { MessageBox.Show(ex.ToString()); }

                Conn.Close();

                if (recordsAffected > 0) { return true; }

                else return false;
            }

            /// <summary> </summary>
            /// <param name="Conn"> </param>
            /// <param name=" TableName"> </param>
            /// <param name=" _FileIndex obj"> </param>
            /// <param name=" WhereClause"> </param>
            public bool Update_SQL_FileIndex(SqlConnection Conn, string TableName, _FileIndex obj, string WhereClause)
            {
                sharpAHK._AHK ahk = new sharpAHK._AHK();
                if (WhereClause.ToUpper().Contains("WHERE")) { WhereClause = WhereClause.ToUpper(); WhereClause = ahk.StringReplace(WhereClause, "WHERE"); }  // remove the word where if passed by user

                //[LucidMedia].[dbo].[FileIndex2]
                string SQLLine = "Update " + TableName + " SET FileFlag = @FileFlag, FilePath = @FilePath, FileName = @FileName, FileSize = @FileSize, LastAccessTime = @LastAccessTime, LastWriteTime = @LastWriteTime, IsReadOnly = @IsReadOnly, FileExt = @FileExt, FileExists = @FileExists, DirName = @DirName, FileDirectory = @FileDirectory, CreationTime = @CreationTime, Attributes = @Attributes, FileAction = @FileAction, Tags = @Tags WHERE " + WhereClause + "";

                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);

                cmd2 = new SqlCommand(SQLLine, Conn);

                if (obj.FileFlag == null) { obj.FileFlag = ""; }
                if (obj.FilePath == null) { obj.FilePath = ""; }
                if (obj.FileName == null) { obj.FileName = ""; }
                if (obj.FileSize == null) { obj.FileSize = ""; }
                if (obj.LastAccessTime == null) { obj.LastAccessTime = ""; }
                if (obj.LastWriteTime == null) { obj.LastWriteTime = ""; }
                if (obj.IsReadOnly == null) { obj.IsReadOnly = ""; }
                if (obj.FileExt == null) { obj.FileExt = ""; }
                if (obj.FileExists == null) { obj.FileExists = ""; }
                if (obj.DirName == null) { obj.DirName = ""; }
                if (obj.FileDirectory == null) { obj.FileDirectory = ""; }
                if (obj.CreationTime == null) { obj.CreationTime = ""; }
                if (obj.Attributes == null) { obj.Attributes = ""; }
                if (obj.FileAction == null) { obj.FileAction = ""; }
                if (obj.Tags == null) { obj.Tags = ""; }

                cmd2.Parameters.AddWithValue(@"FileFlag", obj.FileFlag.ToString());
                cmd2.Parameters.AddWithValue(@"FilePath", obj.FilePath.ToString());
                cmd2.Parameters.AddWithValue(@"FileName", obj.FileName.ToString());
                cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize.ToString());
                cmd2.Parameters.AddWithValue(@"LastAccessTime", obj.LastAccessTime.ToString());
                cmd2.Parameters.AddWithValue(@"LastWriteTime", obj.LastWriteTime.ToString());
                cmd2.Parameters.AddWithValue(@"IsReadOnly", obj.IsReadOnly.ToString());
                cmd2.Parameters.AddWithValue(@"FileExt", obj.FileExt.ToString());
                cmd2.Parameters.AddWithValue(@"FileExists", obj.FileExists.ToString());
                cmd2.Parameters.AddWithValue(@"DirName", obj.DirName.ToString());
                cmd2.Parameters.AddWithValue(@"FileDirectory", obj.FileDirectory.ToString());
                cmd2.Parameters.AddWithValue(@"CreationTime", obj.CreationTime.ToString());
                cmd2.Parameters.AddWithValue(@"Attributes", obj.Attributes.ToString());
                cmd2.Parameters.AddWithValue(@"FileAction", obj.FileAction.ToString());
                cmd2.Parameters.AddWithValue(@"Tags", obj.Tags.ToString());

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }

                int recordsAffected = 0;

                try { recordsAffected = cmd2.ExecuteNonQuery(); }

                catch (SqlException ex) { MessageBox.Show(ex.ToString()); }

                Conn.Close();

                if (recordsAffected > 0) { return true; }

                else return false;
            }


            #endregion

            #region === SQL: Populate Controls ===

            // Populate DataGridView with SQL DataTable (returns grid rowcount)

            /// <summary> Populate DataGridView with SQL DataTable (returns grid rowcount)</summary>
            /// <param name="dv"> </param>
            /// <param name="conn"> </param>
            /// <param name="SQLQuery"> </param>
            /// <param name="ClearGrid"> </param>
            /// <param name="AddCheckBoxColumn"> </param>
            public int Populate_DataGrid(DataGridView dv, SqlConnection conn, string SQLQuery, bool ClearGrid = true, bool AddCheckBoxColumn = true)
            {
                if (ClearGrid)
                {
                    // clear out datagrid contents
                    dv.DataSource = null;
                    dv.Rows.Clear();
                    dv.Columns.Clear();
                    dv.AutoGenerateColumns = true;
                }

                DataTable SQL = new DataTable();
                SQL = GetDataTable(conn, SQLQuery);

                if (AddCheckBoxColumn)  // option to add new column with checkboxes to SQL search results
                {
                    SQL.Columns.Add(new DataColumn("Selected", typeof(bool))); //this will show checkboxes in Selected Column
                }


                dv.DataSource = SQL;
                return dv.RowCount;
            }


            #endregion

            #region === DateTime ===

            /// <summary>
            /// Verify / Format Dates Before Inserting into SQL Db 
            /// </summary>
            /// <param name="InTime">Time String to Validate / Convert to DateTime</param>
            /// <returns>Returns DateTime Variable from InTime String</returns>
            public DateTime ValidTime(string InTime)
            {
                bool Valid = IsValidSqlDatetime(InTime);

                if (Valid)
                {
                    DateTime Converted = Convert.ToDateTime(InTime);
                    return Converted;
                }

                //if (!Valid)
                DateTime PlaceHolderTime = new DateTime(1900, 1, 1);
                return PlaceHolderTime;
            }
            static bool IsValidSqlDatetime(string someval)
            {
                bool valid = false;
                DateTime testDate = DateTime.MinValue;
                DateTime minDateTime = DateTime.MaxValue;
                DateTime maxDateTime = DateTime.MinValue;

                minDateTime = new DateTime(1753, 1, 1);
                maxDateTime = new DateTime(9999, 12, 31, 23, 59, 59, 997);

                if (DateTime.TryParse(someval, out testDate))
                {
                    if (testDate >= minDateTime && testDate <= maxDateTime)
                    {
                        valid = true;
                    }
                }

                return valid;
            }

            public DateTime GetSundaysDate(DateTime CurrDate)
            {
                int day = ((int)CurrDate.DayOfWeek);
                int subtract = 0;
                switch (day)
                {
                    case 0:
                        return CurrDate.Date;
                    case 1:
                        subtract = -1;
                        CurrDate = CurrDate.AddDays(subtract).Date;
                        return CurrDate;
                    case 2:
                        subtract = -2;
                        CurrDate = CurrDate.AddDays(subtract).Date;
                        return CurrDate;
                    case 3:
                        subtract = -3;
                        CurrDate = CurrDate.AddDays(subtract).Date;
                        return CurrDate;
                    case 4:
                        subtract = -4;
                        CurrDate = CurrDate.AddDays(subtract).Date;
                        return CurrDate;
                    case 5:
                        subtract = -5;
                        CurrDate = CurrDate.AddDays(subtract).Date;
                        return CurrDate;
                    case 6:
                        subtract = -6;
                        CurrDate = CurrDate.AddDays(subtract).Date;
                        return CurrDate;
                    default:
                        return CurrDate;

                }
                //return null;
            }

            #endregion

            #region === Show Images ===

            public void WriteShowImage(string filepath, SqlConnection Conn, string TableName, string ShowName)
            {
                if (File.Exists(filepath))
                {

                    // write image file 

                    FileInfo info = new FileInfo(filepath);
                    string FileName = info.Name;
                    string FileSize = info.Length.ToString();
                    string DateModified = info.LastWriteTime.ToString();

                    //SqlConnection SQLConn = new SqlConnection("Server=198.71.225.113;DataBase=MediaServer;Uid=lucid;Pwd=Go1Daddy88");

                    if (Conn.State == ConnectionState.Closed) { Conn.Open(); }

                    byte[] file;
                    using (var stream = new FileStream(filepath, FileMode.Open, System.IO.FileAccess.Read))
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            file = reader.ReadBytes((int)stream.Length);
                        }
                    }
                    //using (var varConnection = Locale.sqlConnectOneTime(Locale.sqlDataConnectionDetails))

                    using (var sqlWrite = new SqlCommand("UPDATE " + TableName + " Set [ShowImage] = @ShowImage WHERE [ShowName] = @ShowName", Conn))
                    {
                        sqlWrite.Parameters.AddWithValue("@ShowName", ShowName);
                        sqlWrite.Parameters.Add("@ShowImage", SqlDbType.VarBinary, file.Length).Value = file;
                        sqlWrite.ExecuteNonQuery();
                    }
                }
            }

            public Image ReturnShowImage(SqlConnection SQLConn, string SQLSearchLine, string LocalFilePath = "")
            {
                //string SQLSearchLine = "SELECT [FileContents] FROM [MediaServer].[lucid].[FileBin] WHERE [FileName] = 'TestFile.zip'";
                _AHK ahk = new _AHK();

                bool DeleteTmpFile = false;
                if (LocalFilePath == "") { DeleteTmpFile = true; LocalFilePath = ahk.AppDir() + "\\IconTemp.png"; }

                if (SQLConn.State == ConnectionState.Closed) { SQLConn.Open(); }
                using (var sqlQuery = new SqlCommand(SQLSearchLine, SQLConn))
                {
                    //sqlQuery.Parameters.AddWithValue("@FileName", ServerFileName);
                    using (var sqlQueryResult = sqlQuery.ExecuteReader())
                        if (sqlQueryResult != null)
                        {
                            sqlQueryResult.Read();
                            var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
                            sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);
                            using (var fs = new FileStream(LocalFilePath, FileMode.Create, FileAccess.Write))
                                fs.Write(blob, 0, blob.Length);
                        }
                }

                SQLConn.Close();

                //Image returnImg = img.GetCopyImage(LocalFilePath);  // convert local return img to Image format

                Image returnImg = Image.FromFile(LocalFilePath);

                //if (DeleteTmpFile) { ahk.FileDelete(LocalFilePath); }  // delete local temp image

                return returnImg;
            }


            #endregion

            #region === DDLists FUNCTIONS ===

            #region ===== DDLists Object =====

            public struct DDLists
            {
                public string ID { get; set; }
                public string FieldName { get; set; }
                public string FieldValue { get; set; }
                public string FieldCateogry { get; set; }
                public string FieldEnabled { get; set; }
                public string DateModified { get; set; }
            }
            //  Fix illegal characters before Sql/Sqlite Db Inserts
            public DDLists DDLists_FixChars(DDLists ToFix)
            {
                DDLists Fixed = new DDLists();

                Fixed.ID = ToFix.ID.Replace("'", "''");
                Fixed.FieldName = ToFix.FieldName.Replace("'", "''");
                Fixed.FieldValue = ToFix.FieldValue.Replace("'", "''");
                Fixed.FieldCateogry = ToFix.FieldCateogry.Replace("'", "''");
                Fixed.FieldEnabled = ToFix.FieldEnabled.Replace("'", "''");
                Fixed.DateModified = ToFix.DateModified.Replace("'", "''");

                return Fixed;
            }

            // Compare two objects to see if they have identical values
            public bool DDLists_Changed(DDLists OldVal, DDLists NewVal)
            {
                DDLists diff = new DDLists();
                List<string> diffList = new List<string>();
                bool different = false;
                if (OldVal.ID == null) { OldVal.ID = ""; }
                if (NewVal.ID == null) { NewVal.ID = ""; }
                if (OldVal.ID != NewVal.ID) { different = true; }
                if (OldVal.FieldName == null) { OldVal.FieldName = ""; }
                if (NewVal.FieldName == null) { NewVal.FieldName = ""; }
                if (OldVal.FieldName != NewVal.FieldName) { different = true; }
                if (OldVal.FieldValue == null) { OldVal.FieldValue = ""; }
                if (NewVal.FieldValue == null) { NewVal.FieldValue = ""; }
                if (OldVal.FieldValue != NewVal.FieldValue) { different = true; }
                if (OldVal.FieldCateogry == null) { OldVal.FieldCateogry = ""; }
                if (NewVal.FieldCateogry == null) { NewVal.FieldCateogry = ""; }
                if (OldVal.FieldCateogry != NewVal.FieldCateogry) { different = true; }
                if (OldVal.FieldEnabled == null) { OldVal.FieldEnabled = ""; }
                if (NewVal.FieldEnabled == null) { NewVal.FieldEnabled = ""; }
                if (OldVal.FieldEnabled != NewVal.FieldEnabled) { different = true; }
                if (OldVal.DateModified == null) { OldVal.DateModified = ""; }
                if (NewVal.DateModified == null) { NewVal.DateModified = ""; }
                if (OldVal.DateModified != NewVal.DateModified) { different = true; }
                return different;
            }

            // Returns object containing the new values different from the old values in object comparison
            public DDLists DDLists_Diff(DDLists OldVal, DDLists NewVal)
            {
                DDLists diff = new DDLists();
                if (OldVal.ID != NewVal.ID) { diff.ID = NewVal.ID; }
                if (OldVal.FieldName != NewVal.FieldName) { diff.FieldName = NewVal.FieldName; }
                if (OldVal.FieldValue != NewVal.FieldValue) { diff.FieldValue = NewVal.FieldValue; }
                if (OldVal.FieldCateogry != NewVal.FieldCateogry) { diff.FieldCateogry = NewVal.FieldCateogry; }
                if (OldVal.FieldEnabled != NewVal.FieldEnabled) { diff.FieldEnabled = NewVal.FieldEnabled; }
                if (OldVal.DateModified != NewVal.DateModified) { diff.DateModified = NewVal.DateModified; }
                return diff;
            }

            // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
            public List<string> DDLists_DiffList(DDLists OldVal, DDLists NewVal)
            {
                List<string> diffList = new List<string>();
                if (OldVal.ID != NewVal.ID) { diffList.Add("Changed ID Value From " + OldVal.ID + " To " + NewVal.ID); }
                if (OldVal.FieldName != NewVal.FieldName) { diffList.Add("Changed FieldName Value From " + OldVal.FieldName + " To " + NewVal.FieldName); }
                if (OldVal.FieldValue != NewVal.FieldValue) { diffList.Add("Changed FieldValue Value From " + OldVal.FieldValue + " To " + NewVal.FieldValue); }
                if (OldVal.FieldCateogry != NewVal.FieldCateogry) { diffList.Add("Changed FieldCateogry Value From " + OldVal.FieldCateogry + " To " + NewVal.FieldCateogry); }
                if (OldVal.FieldEnabled != NewVal.FieldEnabled) { diffList.Add("Changed FieldEnabled Value From " + OldVal.FieldEnabled + " To " + NewVal.FieldEnabled); }
                if (OldVal.DateModified != NewVal.DateModified) { diffList.Add("Changed DateModified Value From " + OldVal.DateModified + " To " + NewVal.DateModified); }
                return diffList;
            }

            // Generate XML String From Object
            public string DDLists_ToXML(DDLists obj)
            {
                string XMLString = "";
                XMLString = XMLString + "<ID>" + obj.ID + "</ID>";
                XMLString = XMLString + "<FieldName>" + obj.FieldName + "</FieldName>";
                XMLString = XMLString + "<FieldValue>" + obj.FieldValue + "</FieldValue>";
                XMLString = XMLString + "<FieldCateogry>" + obj.FieldCateogry + "</FieldCateogry>";
                XMLString = XMLString + "<FieldEnabled>" + obj.FieldEnabled + "</FieldEnabled>";
                XMLString = XMLString + "<DateModified>" + obj.DateModified + "</DateModified>";
                return XMLString;
            }

            // Populate Object from XML Tag String
            public DDLists DDLists_FromXML(string XMLString)
            {
                _Parse prs = new _Parse();
                DDLists obj = new DDLists();
                obj.ID = prs.XML_Text(XMLString, "<ID>");
                obj.FieldName = prs.XML_Text(XMLString, "<FieldName>");
                obj.FieldValue = prs.XML_Text(XMLString, "<FieldValue>");
                obj.FieldCateogry = prs.XML_Text(XMLString, "<FieldCateogry>");
                obj.FieldEnabled = prs.XML_Text(XMLString, "<FieldEnabled>");
                obj.DateModified = prs.XML_Text(XMLString, "<DateModified>");
                return obj;
            }


            #endregion
            public bool Create_Table_DropDownLists(string DbFile)
            {
                _AHK ahk = new _AHK();
                _Database.SQLite sqlite = new _Database.SQLite();
                string CreateLine = "Create Table [DropDownLists] (ID INTEGER PRIMARY KEY, FieldName VARCHAR, FieldValue VARCHAR, FieldCateogry VARCHAR, FieldEnabled VARCHAR, DateModified VARCHAR)";
                bool TableCreated = sqlite.Table_Exists(DbFile, "DropDownLists");
                if (!TableCreated) { TableCreated = sqlite.Table_New(DbFile, "DropDownLists", "Create Table [DropDownLists] (ID INTEGER PRIMARY KEY, FieldName VARCHAR, FieldValue VARCHAR, FieldCateogry VARCHAR, FieldEnabled VARCHAR, DateModified VARCHAR", false); }


                if (!TableCreated) { ahk.MsgBox("[DropDownLists] Created = " + TableCreated.ToString()); }
                return TableCreated;
            }

            #region ===== DDLists SQLite : Return =====

            public DDLists Return_Object_From_DropDownLists(string WhereClause = "[ID] = ''", string DbFile = "")
            {
                _AHK ahk = new _AHK();
                _Database.SQLite sqlite = new _Database.SQLite();

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\DDLists.sqlite"; }
                string SelectLine = "Select [ID], [FieldName], [FieldValue], [FieldCateogry], [FieldEnabled], [DateModified] From [DropDownLists] ";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
                DDLists returnObject = new DDLists();
                int i = 0;
                string Value = "";
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.FieldName = ret["FieldName"].ToString();
                        returnObject.FieldValue = ret["FieldValue"].ToString();
                        returnObject.FieldCateogry = ret["FieldCateogry"].ToString();
                        returnObject.FieldEnabled = ret["FieldEnabled"].ToString();
                        returnObject.DateModified = ret["DateModified"].ToString();
                    }
                }

                return returnObject;
            }

            public List<DDLists> Return_DDLists_List(string WhereClause = "", string DbFile = "", string TableName = "[DropDownLists]")
            {
                _AHK ahk = new _AHK();
                _Database.SQLite sqlite = new _Database.SQLite();
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\DDLists.sqlite"; }
                string SelectLine = "Select * From " + TableName;

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

                List<DDLists> ReturnList = new List<DDLists>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        DDLists returnObject = new DDLists();

                        returnObject.ID = ret["ID"].ToString();
                        returnObject.FieldName = ret["FieldName"].ToString();
                        returnObject.FieldValue = ret["FieldValue"].ToString();
                        returnObject.FieldCateogry = ret["FieldCateogry"].ToString();
                        returnObject.FieldEnabled = ret["FieldEnabled"].ToString();
                        returnObject.DateModified = ret["DateModified"].ToString();

                        ReturnList.Add(returnObject);
                    }
                }

                return ReturnList;
            }

            public DataTable Return_DataTable_From_DropDownLists(string DbFile)
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                string SelectLine = "Select [ID], [FieldName], [FieldValue], [FieldCateogry], [FieldEnabled], [DateModified] From [DropDownLists]";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                return ReturnTable;
            }


            #endregion
            #region ===== DDLists SQLite : Update Insert =====

            public bool DDLists_Insert(DDLists inObject, string DbFile = "")
            {
                _AHK ahk = new _AHK();
                _Database.SQLite sqlite = new _Database.SQLite();

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\DDLists.sqlite"; }
                string InsertLine = "Insert Into [DropDownLists] (ID, FieldName, FieldValue, FieldCateogry, FieldEnabled, DateModified) values ('" + inObject.ID + "', '" + inObject.FieldName + "', '" + inObject.FieldValue + "', '" + inObject.FieldCateogry + "', '" + inObject.FieldEnabled + "', '" + inObject.DateModified + "')";
                bool Inserted = sqlite.Execute(DbFile, InsertLine);
                if (!Inserted) { ahk.MsgBox("Inserted Into [DropDownLists] = " + Inserted.ToString()); }
                return Inserted;
            }

            public bool DDLists_Update(DDLists inObject, string DbFile = "")
            {
                _AHK ahk = new _AHK();
                _Database.SQLite sqlite = new _Database.SQLite();

                //string UpdateLine = "Update [DropDownLists] set ID = '" + inObject.ID + "', FieldName = '" + inObject.FieldName + "', FieldValue = '" + inObject.FieldValue + "', FieldCateogry = '" + inObject.FieldCateogry + "', FieldEnabled = '" + inObject.FieldEnabled + "', DateModified = '" + inObject.DateModified + "' WHERE [Item] = 'Value' ";
                string UpdateLine = "Update [DropDownLists] set ";


                if (inObject.ID != null) { UpdateLine = UpdateLine + "[ID] = '" + inObject.ID + "',"; }
                if (inObject.FieldName != null) { UpdateLine = UpdateLine + "[FieldName] = '" + inObject.FieldName + "',"; }
                if (inObject.FieldValue != null) { UpdateLine = UpdateLine + "[FieldValue] = '" + inObject.FieldValue + "',"; }
                if (inObject.FieldCateogry != null) { UpdateLine = UpdateLine + "[FieldCateogry] = '" + inObject.FieldCateogry + "',"; }
                if (inObject.FieldEnabled != null) { UpdateLine = UpdateLine + "[FieldEnabled] = '" + inObject.FieldEnabled + "',"; }
                if (inObject.DateModified != null) { UpdateLine = UpdateLine + "[DateModified] = '" + inObject.DateModified + "',"; }

                UpdateLine = ahk.TrimLast(UpdateLine, 1);
                UpdateLine = UpdateLine + " WHERE [ID] = ' '"; // DEFINE CONDITION HERE !!!

                bool Updated = sqlite.Execute(DbFile, UpdateLine);
                return Updated;
            }

            public bool DDLists_UpdateInsert(DDLists obj, string DbFile = "")
            {
                _AHK ahk = new _AHK();
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\DDLists.sqlite"; }
                if (!File.Exists(DbFile)) { Create_Table_DropDownLists(DbFile); }

                bool Updated = DDLists_Update(obj, DbFile);  // try to update record first
                if (!Updated) { Updated = DDLists_Insert(obj, DbFile); }  // if unable to update, insert new record
                return Updated;
            }


            #endregion
            #region ===== DDLists DataTable =====

            public DataTable Return_DDLists_DataTable(string DbFile = "", string TableName = "DDLists", string WhereClause = "", bool Debug = false)
            {
                _AHK ahk = new _AHK();
                _Database.SQLite sqlite = new _Database.SQLite();

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\DDLists.sqlite"; }
                string SelectLine = "Select * From [DropDownLists]";

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }

                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


                DataTable table = new DataTable();
                table.Columns.Add("ID", typeof(string));
                table.Columns.Add("FieldName", typeof(string));
                table.Columns.Add("FieldValue", typeof(string));
                table.Columns.Add("FieldCateogry", typeof(string));
                table.Columns.Add("FieldEnabled", typeof(string));
                table.Columns.Add("DateModified", typeof(string));

                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        DDLists returnObject = new DDLists();

                        returnObject.ID = ret["ID"].ToString();
                        returnObject.FieldName = ret["FieldName"].ToString();
                        returnObject.FieldValue = ret["FieldValue"].ToString();
                        returnObject.FieldCateogry = ret["FieldCateogry"].ToString();
                        returnObject.FieldEnabled = ret["FieldEnabled"].ToString();
                        returnObject.DateModified = ret["DateModified"].ToString();

                        table.Rows.Add(returnObject.ID, returnObject.FieldName, returnObject.FieldValue, returnObject.FieldCateogry, returnObject.FieldEnabled, returnObject.DateModified);
                    }
                }

                return table;
            }

            public DataTable Create_DropDownLists_DataTable(DDLists inObject)
            {
                DataTable table = new DataTable();
                table.Columns.Add("ID", typeof(string));
                table.Columns.Add("FieldName", typeof(string));
                table.Columns.Add("FieldValue", typeof(string));
                table.Columns.Add("FieldCateogry", typeof(string));
                table.Columns.Add("FieldEnabled", typeof(string));
                table.Columns.Add("DateModified", typeof(string));

                table.Rows.Add(inObject.ID, inObject.FieldName, inObject.FieldValue, inObject.FieldCateogry, inObject.FieldEnabled, inObject.DateModified);
                return table;
            }


            #endregion
            #region ===== DDLists DataGridView =====

            public void HideShow_DropDownLists_Columns(DataGridView dv)
            {

                try { dv.Columns["ID"].Visible = true; } catch { }
                try { dv.Columns["FieldName"].Visible = true; } catch { }
                try { dv.Columns["FieldValue"].Visible = true; } catch { }
                try { dv.Columns["FieldCateogry"].Visible = true; } catch { }
                try { dv.Columns["FieldEnabled"].Visible = true; } catch { }
                try { dv.Columns["DateModified"].Visible = true; } catch { }
            }
            public void Enable_DropDownLists_Columns(DataGridView dv)
            {

                try { dv.Columns["ID"].ReadOnly = true; } catch { }
                try { dv.Columns["FieldName"].ReadOnly = true; } catch { }
                try { dv.Columns["FieldValue"].ReadOnly = true; } catch { }
                try { dv.Columns["FieldCateogry"].ReadOnly = true; } catch { }
                try { dv.Columns["FieldEnabled"].ReadOnly = true; } catch { }
                try { dv.Columns["DateModified"].ReadOnly = true; } catch { }
            }

            #endregion
            #region ===== DDLists SQL Functions =====

            // Return DDLists SQL Connection String
            public SqlConnection DDLists_Conn()
            {
                // populate sql connection
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMLucidMedia"].ConnectionString);
                // SqlConnection Conn = new SqlConnection("Server=188.168.188.88;DataBase=LucidMedia;Uid=lucidm;Pwd=pass");
                return conn;
            }

            // Return DDLists TableName (Full Path)
            public string DDLists_TableName()
            {
                // populate to return full sql table name
                return "[ADBIndex].[dbo].[DropDownLists]";
            }

            // Generate SQL Table
            public bool DDLists_CreateSQLTable()
            {
                SqlConnection Conn = DDLists_Conn();
                string CreateTableLine = "CREATE TABLE [DropDownLists](";
                CreateTableLine = CreateTableLine + "[ID] [int] IDENTITY(1,1) NOT NULL,";
                CreateTableLine = CreateTableLine + "[FieldName] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[FieldValue] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[FieldCateogry] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[FieldEnabled] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[DateModified] [varchar](max) NOT NULL";
                CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";



                return false;
            }



            /// <summary>
            /// Deletes DDList Object based on ID Field
            /// </summary>
            /// <param name="obj">DDList Object Populated with List Item ID</param>
            /// <returns>Returns True if Successful</returns>
            public bool Delete_DDList(DDLists obj)
            {
                string cmd = "Delete FROM [ADBIndex].[dbo].[DropDownLists] where [ID] = '" + obj.ID + "'";
                _Database.SQL sql = new SQL();
                return sql.Write_SQL(DDLists_Conn(), cmd);
            }


            public bool DDLists_InsertSQL(DDLists obj)
            {
                _AHK ahk = new _AHK();
                SqlConnection Con = DDLists_Conn();
                string SQLLine = "Insert Into " + DDLists_TableName() + " (FieldName, FieldValue, FieldCateogry, FieldEnabled, DateModified) VALUES (@FieldName, @FieldValue, @FieldCateogry, @FieldEnabled, @DateModified)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.FieldName == null) { obj.FieldName = ""; }
                if (obj.FieldValue == null) { obj.FieldValue = ""; }
                if (obj.FieldCateogry == null) { obj.FieldCateogry = ""; }
                if (obj.FieldEnabled == null) { obj.FieldEnabled = ""; }
                if (obj.DateModified == null) { obj.DateModified = ""; }
                cmd2.Parameters.AddWithValue(@"FieldName", obj.FieldName.ToString());
                cmd2.Parameters.AddWithValue(@"FieldValue", obj.FieldValue.ToString());
                cmd2.Parameters.AddWithValue(@"FieldCateogry", obj.FieldCateogry.ToString());
                cmd2.Parameters.AddWithValue(@"FieldEnabled", obj.FieldEnabled.ToString());
                cmd2.Parameters.AddWithValue(@"DateModified", DateTime.Now.ToString());
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Con.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool DDLists_UpdateSQL(DDLists obj)
            {
                if (obj.ID == null) { return false; }

                _AHK ahk = new _AHK();
                SqlConnection Conn = DDLists_Conn();
                string SQLLine = "Update " + DDLists_TableName() + " SET FieldName = @FieldName, FieldValue = @FieldValue, FieldCateogry = @FieldCateogry, FieldEnabled = @FieldEnabled, DateModified = @DateModified WHERE ID = @ID";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.ID == null) { obj.ID = ""; }
                if (obj.FieldName == null) { obj.FieldName = ""; }
                if (obj.FieldValue == null) { obj.FieldValue = ""; }
                if (obj.FieldCateogry == null) { obj.FieldCateogry = ""; }
                if (obj.FieldEnabled == null) { obj.FieldEnabled = ""; }
                if (obj.DateModified == null) { obj.DateModified = ""; }
                cmd2.Parameters.AddWithValue(@"ID", obj.ID.ToString());
                cmd2.Parameters.AddWithValue(@"FieldName", obj.FieldName.ToString());
                cmd2.Parameters.AddWithValue(@"FieldValue", obj.FieldValue.ToString());
                cmd2.Parameters.AddWithValue(@"FieldCateogry", obj.FieldCateogry.ToString());
                cmd2.Parameters.AddWithValue(@"FieldEnabled", obj.FieldEnabled.ToString());
                cmd2.Parameters.AddWithValue(@"DateModified", DateTime.Now.ToString());
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool DDLists_UpdateInsert(DDLists obj)
            {
                SqlConnection Conn = DDLists_Conn();
                bool Updated = DDLists_UpdateSQL(obj);  // try to update record first
                if (!Updated) { Updated = DDLists_InsertSQL(obj); }  // if unable to update, insert new record
                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool DDLists_UpdateIfPopulated(DDLists obj, string ID = "")
            {
                _AHK ahk = new _AHK();
                SqlConnection Conn = DDLists_Conn();
                string SQLcmd = "Update " + DDLists_TableName() + " SET ";
                if (obj.ID != null) { SQLcmd = SQLcmd + " ID = @ID,"; }
                if (obj.FieldName != null) { SQLcmd = SQLcmd + " FieldName = @FieldName,"; }
                if (obj.FieldValue != null) { SQLcmd = SQLcmd + " FieldValue = @FieldValue,"; }
                if (obj.FieldCateogry != null) { SQLcmd = SQLcmd + " FieldCateogry = @FieldCateogry,"; }
                if (obj.FieldEnabled != null) { SQLcmd = SQLcmd + " FieldEnabled = @FieldEnabled,"; }
                if (obj.DateModified != null) { SQLcmd = SQLcmd + " DateModified = @DateModified,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ID = @ID";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.ID != null) { cmd2.Parameters.AddWithValue(@"ID", obj.ID); }
                if (obj.FieldName != null) { cmd2.Parameters.AddWithValue(@"FieldName", obj.FieldName); }
                if (obj.FieldValue != null) { cmd2.Parameters.AddWithValue(@"FieldValue", obj.FieldValue); }
                if (obj.FieldCateogry != null) { cmd2.Parameters.AddWithValue(@"FieldCateogry", obj.FieldCateogry); }
                if (obj.FieldEnabled != null) { cmd2.Parameters.AddWithValue(@"FieldEnabled", obj.FieldEnabled); }
                if (obj.DateModified != null) { cmd2.Parameters.AddWithValue(@"DateModified", obj.DateModified); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public DDLists DDLists_ReturnSQL(string ID = "")
            {
                _Database.SQL sql = new _Database.SQL();
                _AHK ahk = new _AHK();
                SqlConnection Conn = DDLists_Conn();
                string SelectLine = "Select [ID],[FieldName],[FieldValue],[FieldCateogry],[FieldEnabled],[DateModified] From " + DDLists_TableName() + " WHERE ID = '" + ID + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                DDLists returnObject = new DDLists();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.FieldName = ret["FieldName"].ToString();
                        returnObject.FieldValue = ret["FieldValue"].ToString();
                        returnObject.FieldCateogry = ret["FieldCateogry"].ToString();
                        returnObject.FieldEnabled = ret["FieldEnabled"].ToString();
                        returnObject.DateModified = ret["DateModified"].ToString();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public List<DDLists> DDLists_ReturnSQLList(string Command = "")
            {
                _Database.SQL sql = new _Database.SQL();
                if (Command == "") { Command = "Select * From DDLists_TableName()"; }
                SqlConnection Conn = DDLists_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<DDLists> ReturnList = new List<DDLists>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        DDLists returnObject = new DDLists();
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.FieldName = ret["FieldName"].ToString();
                        returnObject.FieldValue = ret["FieldValue"].ToString();
                        returnObject.FieldCateogry = ret["FieldCateogry"].ToString();
                        returnObject.FieldEnabled = ret["FieldEnabled"].ToString();
                        returnObject.DateModified = ret["DateModified"].ToString();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }

            public bool DDLists_SQL_to_SQLite(string SqliteDBPath = @"\Db\DDLists.sqlite")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                _AHK ahk = new _AHK();
                string SaveFile = SqliteDBPath;
                if (SqliteDBPath == @"\Db\DDLists.sqlite")
                {
                    ahk.FileCreateDir(ahk.AppDir() + @"\Db");
                    SaveFile = ahk.AppDir() + @"\Db\DDLists.sqlite";
                }

                //sb.StatusBar("Copying SQL Db to " + SaveFile + "...");
                sqlite.SQLTable_To_NewSQLiteTable(DDLists_Conn(), "DropDownLists", "DropDownLists", SaveFile, "", false, false, false);
                //sb.StatusBar("FINISHED Copying SQL Db to " + SaveFile);

                if (File.Exists(SaveFile)) { return true; } else { return false; }
            }


            #endregion

            #endregion

            #region === CodeGen Functions ===

            // example use, check to see if sql column is one of these types
            public void ColCheck(string fieldType)
            {
                bool isTextCol = IsTextCol(fieldType);
                bool isIntCol = IsIntCol(fieldType);
                bool isImageCol = IsImageCol(fieldType);
                bool isDateTimeCol = IsDateTimeCol(fieldType);
                bool isCbCol = IsCbCol(fieldType);
            }

            public bool IsTextCol(string fieldType)
            {
                bool IS = false;

                if (fieldType.Trim().ToLower() == "bigint") { IS = true; }
                if (fieldType.Trim().ToLower() == "nvarchar") { IS = true; }
                //if (fieldType.Trim().ToLower() == "int") { IS = true; }
                if (fieldType.Trim().ToLower() == "varchar") { IS = true; }
                if (fieldType.Trim().ToLower() == "nchar") { IS = true; }

                return IS;
            }

            public bool IsIntCol(string fieldType)
            {
                bool IS = false;

                if (fieldType.Trim().ToLower() == "bigint") { IS = true; }
                if (fieldType.Trim().ToLower() == "int") { IS = true; }

                return IS;
            }

            public bool IsDateTimeCol(string fieldType)
            {
                bool IS = false;

                if (fieldType.Trim().ToLower() == "datetime") { IS = true; }

                return IS;
            }

            public bool IsImageCol(string fieldType)
            {
                bool IS = false;

                if (fieldType.Trim().ToLower() == "varbinary") { IS = true; }

                return IS;
            }

            public bool IsCbCol(string fieldType)
            {
                bool IS = false;

                if (fieldType.Trim().ToLower() == "bit") { IS = true; }

                return IS;
            }


            #endregion

        }
    }


}

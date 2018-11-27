using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using sharpAHK;
using sharpAHK_Dev;
using AHKExpressions;
using TelerikExpressions;
using static sharpAHK_Dev._Sites.EpGuides;
using System.Configuration;
using System.Web;
using System.Collections;
using System.Net;
using System.ComponentModel;
using System.Web.Script.Serialization;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using static sharpAHK_Dev._Web.DL_Accounts;

namespace sharpAHK_Dev
{
    public partial class _Web
    {

        /// <summary>
        /// Class for Account L/P Pulled from Database
        /// </summary>
        public class DL_Accounts
        {
            _AHK ahk = new _AHK();

            #region === Accounts FUNCTIONS ===

            /// <summary>
            /// Updates RapidGator Account Password (Verifies Account Status While Updating)
            /// </summary>
            /// <param name="Login"></param>
            /// <param name="Pass"></param>
            /// <param name="AccountOrder"></param>
            /// <param name="Enabled"></param>
            /// <returns></returns>
            public bool RapidGatorAccount_Update(string Login, string Pass, int AccountOrder = 1, bool Enabled = true)
            {
                _Sites.RapidGator rg = new _Sites.RapidGator();

                // Check to see if Account is Active/Valid
                string SessionID = "";
                string TrafficLeft = "";
                string ResponseStatus = "";
                bool Online = rg.RapidLoginCheck(Login, Pass, out SessionID, out TrafficLeft, out ResponseStatus);

                Accounts Account = new Accounts();
                Account.SiteName = Sites.RapidGator;
                Account.AccountPass = EncryptPass(Account.SiteName, Pass);
                Account.LastChecked = DateTime.Now;
                Account.AccountOrder = AccountOrder;
                Account.AccountUserName = Login;
                Account.AccountEnabled = Enabled;

                Account.AccountVerified = false;
                if (SessionID != "") { Account.AccountVerified = true; }

                bool updated = Accounts_UpdateInsert(Account);

                return updated;
            }


            /// <summary>
            /// Return RapidGator Password for UserName
            /// </summary>
            /// <param name="UserName"></param>
            /// <returns></returns>
            public string RapidGatorPass(string UserName)
            {
                Accounts account = ReturnAccount(Sites.RapidGator, UserName);
                return account.AccountPass;
            }


            public List<Accounts> RapidGatorAccounts()
            {
                List<Accounts> accounts = Return_Accounts(Sites.RapidGator);
                return accounts;
            }



            // Encrypt/Decrypt Db Passes based on SiteName
            public string EncryptPass(Sites SiteName, string encryptWord)
            {
                string Encrypted = encryptWord;

                if (SiteName.ToString().ToUpper() == "RAPIDGATOR") { Encrypted = ahk.Encrypt(encryptWord, "rgDbStore"); }

                return Encrypted;
            }
            public string DecryptPass(Sites SiteName, string decryptWord)
            {
                string decrypted = decryptWord;

                if (SiteName.ToString().ToUpper() == "RAPIDGATOR") { decrypted = ahk.Decrypt(decryptWord, "rgDbStore"); }

                return decrypted;
            }




            #region ===== Accounts Object =====

            public enum Sites
            {
                All,
                None,
                RapidGator,
                LocalDb
            }

            public Sites ToSite(string SiteName)
            {
                if (SiteName.ToUpper() == "RAPIDGATOR") { return Sites.RapidGator; }

                return Sites.None;
            }

            public struct Accounts
            {
                public Sites SiteName { get; set; }
                public int AccountOrder { get; set; }
                public string AccountUserName { get; set; }
                public string AccountPass { get; set; }
                public bool AccountVerified { get; set; }
                public bool AccountEnabled { get; set; }
                public DateTime LastChecked { get; set; }
            }


            #endregion
            #region === SQLite ===

            public bool Create_Table_Accounts(string DbFile)
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                string CreateLine = "Create Table [Accounts] (SiteName VARCHAR, AccountOrder VARCHAR, AccountUserName VARCHAR, AccountPass VARCHAR, AccountVerified VARCHAR, AccountEnabled VARCHAR, LastChecked VARCHAR)";
                bool TableCreated = sqlite.Table_Exists(DbFile, "Accounts");
                if (!TableCreated) { TableCreated = sqlite.Table_New(DbFile, "Accounts", "Create Table [Accounts] (SiteName VARCHAR, AccountOrder VARCHAR, AccountUserName VARCHAR, AccountPass VARCHAR, AccountVerified VARCHAR, AccountEnabled VARCHAR, LastChecked VARCHAR", false); }


                if (!TableCreated) { ahk.MsgBox("[Accounts] Created = " + TableCreated.ToString()); }
                return TableCreated;
            }

            #region ===== Accounts SQLite : Return =====

            public Accounts Return_Object_From_Accounts(string WhereClause = "[SiteName] = ''", string DbFile = "")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\Accounts.sqlite"; }
                string SelectLine = "Select [SiteName], [AccountOrder], [AccountUserName], [AccountPass], [AccountVerified], [AccountEnabled], [LastChecked] From [Accounts] ";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
                Accounts returnObject = new Accounts();
                int i = 0;
                string Value = "";
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.SiteName = ToSite(ret["SiteName"].ToString());
                        returnObject.AccountOrder = ret["AccountOrder"].ToInt();
                        returnObject.AccountUserName = ret["AccountUserName"].ToString();
                        returnObject.AccountPass = ret["AccountPass"].ToString();
                        returnObject.AccountVerified = ret["AccountVerified"].ToBool();
                        returnObject.AccountEnabled = ret["AccountEnabled"].ToBool();
                        returnObject.LastChecked = ret["LastChecked"].ToDateTime();
                    }
                }

                return returnObject;
            }

            public List<Accounts> Return_Accounts_List(string WhereClause = "", string DbFile = "", string TableName = "[Accounts]")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\Accounts.sqlite"; }
                string SelectLine = "Select * From " + TableName;

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

                List<Accounts> ReturnList = new List<Accounts>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        Accounts returnObject = new Accounts();

                        returnObject.SiteName = ToSite(ret["SiteName"].ToString());
                        returnObject.AccountOrder = ret["AccountOrder"].ToInt();
                        returnObject.AccountUserName = ret["AccountUserName"].ToString();
                        returnObject.AccountPass = ret["AccountPass"].ToString();
                        returnObject.AccountVerified = ret["AccountVerified"].ToBool();
                        returnObject.AccountEnabled = ret["AccountEnabled"].ToBool();
                        returnObject.LastChecked = ret["LastChecked"].ToDateTime();

                        ReturnList.Add(returnObject);
                    }
                }

                return ReturnList;
            }

            public DataTable Return_DataTable_From_Accounts(string DbFile)
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                string SelectLine = "Select [SiteName], [AccountOrder], [AccountUserName], [AccountPass], [AccountVerified], [AccountEnabled], [LastChecked] From [Accounts]";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                return ReturnTable;
            }


            #endregion
            #region ===== Accounts SQLite : Update Insert =====

            public bool Accounts_Insert(Accounts inObject, string DbFile = "")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\Accounts.sqlite"; }
                string InsertLine = "Insert Into [Accounts] (SiteName, AccountOrder, AccountUserName, AccountPass, AccountVerified, AccountEnabled, LastChecked) values ('" + inObject.SiteName + "', '" + inObject.AccountOrder + "', '" + inObject.AccountUserName + "', '" + inObject.AccountPass + "', '" + inObject.AccountVerified + "', '" + inObject.AccountEnabled + "', '" + inObject.LastChecked + "')";
                bool Inserted = sqlite.Execute(DbFile, InsertLine);
                if (!Inserted) { ahk.MsgBox("Inserted Into [Accounts] = " + Inserted.ToString()); }
                return Inserted;
            }

            public bool Accounts_Update(Accounts inObject, string DbFile = "")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                //string UpdateLine = "Update [Accounts] set SiteName = '" + inObject.SiteName + "', AccountOrder = '" + inObject.AccountOrder + "', AccountUserName = '" + inObject.AccountUserName + "', AccountPass = '" + inObject.AccountPass + "', AccountVerified = '" + inObject.AccountVerified + "', AccountEnabled = '" + inObject.AccountEnabled + "', LastChecked = '" + inObject.LastChecked + "' WHERE [Item] = 'Value' ";
                string UpdateLine = "Update [Accounts] set ";


                if (inObject.SiteName.ToString() != "") { UpdateLine = UpdateLine + "[SiteName] = '" + inObject.SiteName + "',"; }
                if (inObject.AccountOrder.ToString() != "") { UpdateLine = UpdateLine + "[AccountOrder] = '" + inObject.AccountOrder + "',"; }
                if (inObject.AccountUserName != null) { UpdateLine = UpdateLine + "[AccountUserName] = '" + inObject.AccountUserName + "',"; }
                if (inObject.AccountPass != null) { UpdateLine = UpdateLine + "[AccountPass] = '" + inObject.AccountPass + "',"; }
                UpdateLine = UpdateLine + "[AccountVerified] = '" + inObject.AccountVerified + "',";
                UpdateLine = UpdateLine + "[AccountEnabled] = '" + inObject.AccountEnabled + "',";
                if (inObject.LastChecked != null) { UpdateLine = UpdateLine + "[LastChecked] = '" + inObject.LastChecked + "',"; }

                UpdateLine = ahk.TrimLast(UpdateLine, 1);
                UpdateLine = UpdateLine + " WHERE [SiteName] = ' '"; // DEFINE CONDITION HERE !!!

                bool Updated = sqlite.Execute(DbFile, UpdateLine);
                return Updated;
            }

            public bool Accounts_UpdateInsert(Accounts obj, string DbFile = "")
            {

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\Accounts.sqlite"; }
                if (!File.Exists(DbFile)) { Create_Table_Accounts(DbFile); }

                bool Updated = Accounts_Update(obj, DbFile);  // try to update record first
                if (!Updated) { Updated = Accounts_Insert(obj, DbFile); }  // if unable to update, insert new record
                return Updated;
            }


            #endregion

            #endregion
            #region ===== Accounts DataTable =====

            public DataTable Return_Accounts_DataTable(string DbFile = "", string TableName = "Accounts", string WhereClause = "", bool Debug = false)
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\Accounts.sqlite"; }
                string SelectLine = "Select * From [Accounts]";

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }

                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


                DataTable table = new DataTable();
                table.Columns.Add("SiteName", typeof(string));
                table.Columns.Add("AccountOrder", typeof(string));
                table.Columns.Add("AccountUserName", typeof(string));
                table.Columns.Add("AccountPass", typeof(string));
                table.Columns.Add("AccountVerified", typeof(string));
                table.Columns.Add("AccountEnabled", typeof(string));
                table.Columns.Add("LastChecked", typeof(string));

                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        Accounts returnObject = new Accounts();

                        returnObject.SiteName = ToSite(ret["SiteName"].ToString());
                        returnObject.AccountOrder = ret["AccountOrder"].ToInt();
                        returnObject.AccountUserName = ret["AccountUserName"].ToString();
                        returnObject.AccountPass = ret["AccountPass"].ToString();
                        returnObject.AccountVerified = ret["AccountVerified"].ToBool();
                        returnObject.AccountEnabled = ret["AccountEnabled"].ToBool();
                        returnObject.LastChecked = ret["LastChecked"].ToDateTime();

                        table.Rows.Add(returnObject.SiteName, returnObject.AccountOrder, returnObject.AccountUserName, returnObject.AccountPass, returnObject.AccountVerified, returnObject.AccountEnabled, returnObject.LastChecked);
                    }
                }

                return table;
            }

            public DataTable Create_Accounts_DataTable(Accounts inObject)
            {
                DataTable table = new DataTable();
                table.Columns.Add("SiteName", typeof(string));
                table.Columns.Add("AccountOrder", typeof(string));
                table.Columns.Add("AccountUserName", typeof(string));
                table.Columns.Add("AccountPass", typeof(string));
                table.Columns.Add("AccountVerified", typeof(string));
                table.Columns.Add("AccountEnabled", typeof(string));
                table.Columns.Add("LastChecked", typeof(string));

                table.Rows.Add(inObject.SiteName, inObject.AccountOrder, inObject.AccountUserName, inObject.AccountPass, inObject.AccountVerified, inObject.AccountEnabled, inObject.LastChecked);
                return table;
            }


            #endregion
            #region ===== Accounts DataGridView =====

            public void HideShow_Accounts_Columns(DataGridView dv)
            {

                try { dv.Columns["SiteName"].Visible = true; } catch { }
                try { dv.Columns["AccountOrder"].Visible = true; } catch { }
                try { dv.Columns["AccountUserName"].Visible = true; } catch { }
                try { dv.Columns["AccountPass"].Visible = true; } catch { }
                try { dv.Columns["AccountVerified"].Visible = true; } catch { }
                try { dv.Columns["AccountEnabled"].Visible = true; } catch { }
                try { dv.Columns["LastChecked"].Visible = true; } catch { }
            }
            public void Enable_Accounts_Columns(DataGridView dv)
            {

                try { dv.Columns["SiteName"].ReadOnly = true; } catch { }
                try { dv.Columns["AccountOrder"].ReadOnly = true; } catch { }
                try { dv.Columns["AccountUserName"].ReadOnly = true; } catch { }
                try { dv.Columns["AccountPass"].ReadOnly = true; } catch { }
                try { dv.Columns["AccountVerified"].ReadOnly = true; } catch { }
                try { dv.Columns["AccountEnabled"].ReadOnly = true; } catch { }
                try { dv.Columns["LastChecked"].ReadOnly = true; } catch { }
            }

            #endregion
            #region ===== Accounts SQL Functions =====

            // Return Accounts SQL Connection String
            public SqlConnection Accounts_Conn()
            {
                // populate sql connection
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMLucidMedia"].ConnectionString);
                return conn;
            }

            // Return Accounts TableName (Full Path)
            public string Accounts_TableName()
            {
                // populate to return full sql table name
                return "[LucidMedia].[dbo].[Accounts]";
            }

            // Generate SQL Table
            public bool Accounts_CreateSQLTable()
            {
                SqlConnection Conn = Accounts_Conn();
                string CreateTableLine = "CREATE TABLE [Accounts](";
                CreateTableLine = CreateTableLine + "[SiteName] [int] IDENTITY(1,1) NOT NULL,";
                CreateTableLine = CreateTableLine + "[AccountOrder] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[AccountUserName] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[AccountPass] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[AccountVerified] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[AccountEnabled] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[LastChecked] [varchar](max) NOT NULL";
                CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                return false;
            }

            public bool Accounts_InsertSQL(Accounts obj)
            {
                SqlConnection Con = Accounts_Conn();
                string SQLLine = "Insert Into " + Accounts_TableName() + " (SiteName, AccountOrder, AccountUserName, AccountPass, AccountVerified, AccountEnabled, LastChecked) VALUES (@SiteName, @AccountOrder, @AccountUserName, @AccountPass, @AccountVerified, @AccountEnabled, @LastChecked)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                //if (obj.SiteName == null) { obj.SiteName = ""; }
                //if (obj.AccountOrder == null) { obj.AccountOrder = ""; }
                //if (obj.AccountUserName == null) { obj.AccountUserName = ""; }
                //if (obj.AccountPass == null) { obj.AccountPass = ""; }
                //if (obj.AccountVerified == null) { obj.AccountVerified = ""; }
                //if (obj.AccountEnabled == null) { obj.AccountEnabled = ""; }
                //if (obj.LastChecked == null) { obj.LastChecked = ""; }

                cmd2.Parameters.AddWithValue(@"SiteName", obj.SiteName.ToString());
                cmd2.Parameters.AddWithValue(@"AccountOrder", obj.AccountOrder.ToString());
                cmd2.Parameters.AddWithValue(@"AccountUserName", obj.AccountUserName.ToString());
                cmd2.Parameters.AddWithValue(@"AccountPass", obj.AccountPass.ToString());
                cmd2.Parameters.AddWithValue(@"AccountVerified", obj.AccountVerified.ToString());
                cmd2.Parameters.AddWithValue(@"AccountEnabled", obj.AccountEnabled.ToString());
                cmd2.Parameters.AddWithValue(@"LastChecked", obj.LastChecked.ToString());
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        Accounts_InsertSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        Accounts_InsertSQL(obj);
                    }
                    else
                    {
                        ahk.MsgBox(ex.ToString());
                        return false;
                    }
                }
                Con.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool Accounts_UpdateSQL(Accounts obj)
            {
                SqlConnection Conn = Accounts_Conn();
                string SQLLine = "Update " + Accounts_TableName() + " SET AccountOrder = @AccountOrder, AccountPass = @AccountPass, AccountVerified = @AccountVerified, AccountEnabled = @AccountEnabled, LastChecked = @LastChecked WHERE SiteName = @SiteName AND AccountUserName = @AccountUserName";

                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                //if (obj.SiteName == null) { obj.SiteName = ""; }
                //if (obj.AccountOrder == null) { obj.AccountOrder = ""; }
                //if (obj.AccountUserName == null) { obj.AccountUserName = ""; }
                //if (obj.AccountPass == null) { obj.AccountPass = ""; }
                //if (obj.AccountVerified == null) { obj.AccountVerified = ""; }
                //if (obj.AccountEnabled == null) { obj.AccountEnabled = ""; }
                //if (obj.LastChecked == null) { obj.LastChecked = ""; }

                cmd2.Parameters.AddWithValue(@"SiteName", obj.SiteName.ToString());
                cmd2.Parameters.AddWithValue(@"AccountOrder", obj.AccountOrder.ToString());
                cmd2.Parameters.AddWithValue(@"AccountUserName", obj.AccountUserName.ToString());
                cmd2.Parameters.AddWithValue(@"AccountPass", obj.AccountPass.ToString());
                cmd2.Parameters.AddWithValue(@"AccountVerified", obj.AccountVerified.ToString());
                cmd2.Parameters.AddWithValue(@"AccountEnabled", obj.AccountEnabled.ToString());
                cmd2.Parameters.AddWithValue(@"LastChecked", obj.LastChecked.ToString());

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        Accounts_UpdateSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        Accounts_UpdateSQL(obj);
                    }
                    else
                    {
                        ahk.MsgBox(ex.ToString());
                        return false;
                    }
                }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool Accounts_UpdateInsert(Accounts obj)
            {
                SqlConnection Conn = Accounts_Conn();
                bool Updated = Accounts_UpdateSQL(obj);  // try to update record first
                if (!Updated) { Updated = Accounts_InsertSQL(obj); }  // if unable to update, insert new record
                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool Accounts_UpdateIfPopulated(Accounts obj, string ID = "")
            {
                SqlConnection Conn = Accounts_Conn();
                string SQLcmd = "Update " + Accounts_TableName() + " SET ";
                if (obj.SiteName != null) { SQLcmd = SQLcmd + " SiteName = @SiteName,"; }
                if (obj.AccountOrder != null) { SQLcmd = SQLcmd + " AccountOrder = @AccountOrder,"; }
                if (obj.AccountUserName != null) { SQLcmd = SQLcmd + " AccountUserName = @AccountUserName,"; }
                if (obj.AccountPass != null) { SQLcmd = SQLcmd + " AccountPass = @AccountPass,"; }
                if (obj.AccountVerified != null) { SQLcmd = SQLcmd + " AccountVerified = @AccountVerified,"; }
                if (obj.AccountEnabled != null) { SQLcmd = SQLcmd + " AccountEnabled = @AccountEnabled,"; }
                if (obj.LastChecked != null) { SQLcmd = SQLcmd + " LastChecked = @LastChecked,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ID = @ID";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.SiteName != null) { cmd2.Parameters.AddWithValue(@"SiteName", obj.SiteName); }
                if (obj.AccountOrder != null) { cmd2.Parameters.AddWithValue(@"AccountOrder", obj.AccountOrder); }
                if (obj.AccountUserName != null) { cmd2.Parameters.AddWithValue(@"AccountUserName", obj.AccountUserName); }
                if (obj.AccountPass != null) { cmd2.Parameters.AddWithValue(@"AccountPass", obj.AccountPass); }
                if (obj.AccountVerified != null) { cmd2.Parameters.AddWithValue(@"AccountVerified", obj.AccountVerified); }
                if (obj.AccountEnabled != null) { cmd2.Parameters.AddWithValue(@"AccountEnabled", obj.AccountEnabled); }
                if (obj.LastChecked != null) { cmd2.Parameters.AddWithValue(@"LastChecked", obj.LastChecked); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public Accounts ReturnAccount(Sites SiteName, string AccountUserName)
            {
                _Database.SQL sql = new _Database.SQL();
                SqlConnection Conn = Accounts_Conn();
                string SelectLine = "Select [SiteName],[AccountOrder],[AccountUserName],[AccountPass],[AccountVerified],[AccountEnabled],[LastChecked] From " + Accounts_TableName() + " WHERE [SiteName] = '" + SiteName.ToString() + "' AND [AccountUserName] = '" + AccountUserName + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                Accounts returnObject = new Accounts();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.SiteName = ToSite(ret["SiteName"].ToString());
                        returnObject.AccountOrder = ret["AccountOrder"].ToInt();
                        returnObject.AccountUserName = ret["AccountUserName"].ToString();
                        returnObject.AccountPass = DecryptPass(returnObject.SiteName, ret["AccountPass"].ToString());
                        returnObject.AccountVerified = ret["AccountVerified"].ToBool();
                        returnObject.AccountEnabled = ret["AccountEnabled"].ToBool();
                        returnObject.LastChecked = ret["LastChecked"].ToDateTime();

                        return returnObject;
                    }
                }

                return returnObject;
            }

            public List<Accounts> Return_Accounts(Sites sites = Sites.All)
            {
                _Database.SQL sql = new _Database.SQL();
                string Command = "Select * From " + Accounts_TableName();
                if (sites != Sites.All)
                {
                    Command = "Select * From " + Accounts_TableName() + " WHERE SiteName = '" + sites.ToString() + "'";
                }

                SqlConnection Conn = Accounts_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<Accounts> ReturnList = new List<Accounts>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        Accounts returnObject = new Accounts();
                        returnObject.SiteName = ToSite(ret["SiteName"].ToString());
                        returnObject.AccountOrder = ret["AccountOrder"].ToInt();
                        returnObject.AccountUserName = ret["AccountUserName"].ToString();
                        returnObject.AccountPass = DecryptPass(returnObject.SiteName, ret["AccountPass"].ToString());
                        returnObject.AccountVerified = ret["AccountVerified"].ToBool();
                        returnObject.AccountEnabled = ret["AccountEnabled"].ToBool();
                        returnObject.LastChecked = ret["LastChecked"].ToDateTime();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }

            public bool Accounts_SQL_to_SQLite(string SqliteDBPath = @"\Db\Accounts.sqlite")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                string SaveFile = SqliteDBPath;
                if (SqliteDBPath == @"\Db\Accounts.sqlite")
                {
                    ahk.FileCreateDir(ahk.AppDir() + @"\Db");
                    SaveFile = ahk.AppDir() + @"\Db\Accounts.sqlite";
                }

                //sb.StatusBar("Copying SQL Db to " + SaveFile + "...");
                sqlite.SQLTable_To_NewSQLiteTable(Accounts_Conn(), "Accounts", "Accounts", SaveFile, "", false, false, false);
                //sb.StatusBar("FINISHED Copying SQL Db to " + SaveFile);

                if (File.Exists(SaveFile)) { return true; } else { return false; }
            }


            #endregion

            #endregion

        }


   }

    public partial class _Sites
    {
        /// <summary>
        /// Class for Account L/P Pulled from Database
        /// </summary>
        public class DL_Accounts
        {
            _AHK ahk = new _AHK();

            #region === Accounts FUNCTIONS ===

            /// <summary>
            /// Updates RapidGator Account Password (Verifies Account Status While Updating)
            /// </summary>
            /// <param name="Login"></param>
            /// <param name="Pass"></param>
            /// <param name="AccountOrder"></param>
            /// <param name="Enabled"></param>
            /// <returns></returns>
            public bool RapidGatorAccount_Update(string Login, string Pass, int AccountOrder = 1, bool Enabled = true)
            {
                _Sites.RapidGator rg = new _Sites.RapidGator();

                // Check to see if Account is Active/Valid
                string SessionID = "";
                string TrafficLeft = "";
                string ResponseStatus = "";
                bool Online = rg.RapidLoginCheck(Login, Pass, out SessionID, out TrafficLeft, out ResponseStatus);

                Accounts Account = new Accounts();
                Account.SiteName = Sites.RapidGator;
                Account.AccountPass = EncryptPass(Account.SiteName, Pass);
                Account.LastChecked = DateTime.Now;
                Account.AccountOrder = AccountOrder;
                Account.AccountUserName = Login;
                Account.AccountEnabled = Enabled;

                Account.AccountVerified = false;
                if (SessionID != "") { Account.AccountVerified = true; }

                bool updated = Accounts_UpdateInsert(Account);

                return updated;
            }


            /// <summary>
            /// Return RapidGator Password for UserName
            /// </summary>
            /// <param name="UserName"></param>
            /// <returns></returns>
            public string RapidGatorPass(string UserName)
            {
                Accounts account = ReturnAccount(Sites.RapidGator, UserName);
                return account.AccountPass;
            }


            public List<Accounts> RapidGatorAccounts()
            {
                List<Accounts> accounts = Return_Accounts(Sites.RapidGator);
                return accounts;
            }



            // Encrypt/Decrypt Db Passes based on SiteName
            public string EncryptPass(Sites SiteName, string encryptWord)
            {
                string Encrypted = encryptWord;

                if (SiteName.ToString().ToUpper() == "RAPIDGATOR") { Encrypted = ahk.Encrypt(encryptWord, "rgDbStore"); }

                return Encrypted;
            }
            public string DecryptPass(Sites SiteName, string decryptWord)
            {
                string decrypted = decryptWord;

                if (SiteName.ToString().ToUpper() == "RAPIDGATOR") { decrypted = ahk.Decrypt(decryptWord, "rgDbStore"); }

                return decrypted;
            }




            #region ===== Accounts Object =====

            public enum Sites
            {
                All,
                None,
                RapidGator,
                LocalDb
            }

            public Sites ToSite(string SiteName)
            {
                if (SiteName.ToUpper() == "RAPIDGATOR") { return Sites.RapidGator; }

                return Sites.None;
            }

            public struct Accounts
            {
                public Sites SiteName { get; set; }
                public int AccountOrder { get; set; }
                public string AccountUserName { get; set; }
                public string AccountPass { get; set; }
                public bool AccountVerified { get; set; }
                public bool AccountEnabled { get; set; }
                public DateTime LastChecked { get; set; }
            }


            #endregion
            #region === SQLite ===

            public bool Create_Table_Accounts(string DbFile)
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                string CreateLine = "Create Table [Accounts] (SiteName VARCHAR, AccountOrder VARCHAR, AccountUserName VARCHAR, AccountPass VARCHAR, AccountVerified VARCHAR, AccountEnabled VARCHAR, LastChecked VARCHAR)";
                bool TableCreated = sqlite.Table_Exists(DbFile, "Accounts");
                if (!TableCreated) { TableCreated = sqlite.Table_New(DbFile, "Accounts", "Create Table [Accounts] (SiteName VARCHAR, AccountOrder VARCHAR, AccountUserName VARCHAR, AccountPass VARCHAR, AccountVerified VARCHAR, AccountEnabled VARCHAR, LastChecked VARCHAR", false); }


                if (!TableCreated) { ahk.MsgBox("[Accounts] Created = " + TableCreated.ToString()); }
                return TableCreated;
            }

            #region ===== Accounts SQLite : Return =====

            public Accounts Return_Object_From_Accounts(string WhereClause = "[SiteName] = ''", string DbFile = "")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\Accounts.sqlite"; }
                string SelectLine = "Select [SiteName], [AccountOrder], [AccountUserName], [AccountPass], [AccountVerified], [AccountEnabled], [LastChecked] From [Accounts] ";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
                Accounts returnObject = new Accounts();
                int i = 0;
                string Value = "";
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.SiteName = ToSite(ret["SiteName"].ToString());
                        returnObject.AccountOrder = ret["AccountOrder"].ToInt();
                        returnObject.AccountUserName = ret["AccountUserName"].ToString();
                        returnObject.AccountPass = ret["AccountPass"].ToString();
                        returnObject.AccountVerified = ret["AccountVerified"].ToBool();
                        returnObject.AccountEnabled = ret["AccountEnabled"].ToBool();
                        returnObject.LastChecked = ret["LastChecked"].ToDateTime();
                    }
                }

                return returnObject;
            }

            public List<Accounts> Return_Accounts_List(string WhereClause = "", string DbFile = "", string TableName = "[Accounts]")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\Accounts.sqlite"; }
                string SelectLine = "Select * From " + TableName;

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

                List<Accounts> ReturnList = new List<Accounts>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        Accounts returnObject = new Accounts();

                        returnObject.SiteName = ToSite(ret["SiteName"].ToString());
                        returnObject.AccountOrder = ret["AccountOrder"].ToInt();
                        returnObject.AccountUserName = ret["AccountUserName"].ToString();
                        returnObject.AccountPass = ret["AccountPass"].ToString();
                        returnObject.AccountVerified = ret["AccountVerified"].ToBool();
                        returnObject.AccountEnabled = ret["AccountEnabled"].ToBool();
                        returnObject.LastChecked = ret["LastChecked"].ToDateTime();

                        ReturnList.Add(returnObject);
                    }
                }

                return ReturnList;
            }

            public DataTable Return_DataTable_From_Accounts(string DbFile)
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                string SelectLine = "Select [SiteName], [AccountOrder], [AccountUserName], [AccountPass], [AccountVerified], [AccountEnabled], [LastChecked] From [Accounts]";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                return ReturnTable;
            }


            #endregion
            #region ===== Accounts SQLite : Update Insert =====

            public bool Accounts_Insert(Accounts inObject, string DbFile = "")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\Accounts.sqlite"; }
                string InsertLine = "Insert Into [Accounts] (SiteName, AccountOrder, AccountUserName, AccountPass, AccountVerified, AccountEnabled, LastChecked) values ('" + inObject.SiteName + "', '" + inObject.AccountOrder + "', '" + inObject.AccountUserName + "', '" + inObject.AccountPass + "', '" + inObject.AccountVerified + "', '" + inObject.AccountEnabled + "', '" + inObject.LastChecked + "')";
                bool Inserted = sqlite.Execute(DbFile, InsertLine);
                if (!Inserted) { ahk.MsgBox("Inserted Into [Accounts] = " + Inserted.ToString()); }
                return Inserted;
            }

            public bool Accounts_Update(Accounts inObject, string DbFile = "")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                //string UpdateLine = "Update [Accounts] set SiteName = '" + inObject.SiteName + "', AccountOrder = '" + inObject.AccountOrder + "', AccountUserName = '" + inObject.AccountUserName + "', AccountPass = '" + inObject.AccountPass + "', AccountVerified = '" + inObject.AccountVerified + "', AccountEnabled = '" + inObject.AccountEnabled + "', LastChecked = '" + inObject.LastChecked + "' WHERE [Item] = 'Value' ";
                string UpdateLine = "Update [Accounts] set ";


                if (inObject.SiteName.ToString() != "") { UpdateLine = UpdateLine + "[SiteName] = '" + inObject.SiteName + "',"; }
                if (inObject.AccountOrder.ToString() != "") { UpdateLine = UpdateLine + "[AccountOrder] = '" + inObject.AccountOrder + "',"; }
                if (inObject.AccountUserName != null) { UpdateLine = UpdateLine + "[AccountUserName] = '" + inObject.AccountUserName + "',"; }
                if (inObject.AccountPass != null) { UpdateLine = UpdateLine + "[AccountPass] = '" + inObject.AccountPass + "',"; }
                UpdateLine = UpdateLine + "[AccountVerified] = '" + inObject.AccountVerified + "',";
                UpdateLine = UpdateLine + "[AccountEnabled] = '" + inObject.AccountEnabled + "',";
                if (inObject.LastChecked != null) { UpdateLine = UpdateLine + "[LastChecked] = '" + inObject.LastChecked + "',"; }

                UpdateLine = ahk.TrimLast(UpdateLine, 1);
                UpdateLine = UpdateLine + " WHERE [SiteName] = ' '"; // DEFINE CONDITION HERE !!!

                bool Updated = sqlite.Execute(DbFile, UpdateLine);
                return Updated;
            }

            public bool Accounts_UpdateInsert(Accounts obj, string DbFile = "")
            {

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\Accounts.sqlite"; }
                if (!File.Exists(DbFile)) { Create_Table_Accounts(DbFile); }

                bool Updated = Accounts_Update(obj, DbFile);  // try to update record first
                if (!Updated) { Updated = Accounts_Insert(obj, DbFile); }  // if unable to update, insert new record
                return Updated;
            }


            #endregion

            #endregion
            #region ===== Accounts DataTable =====

            public DataTable Return_Accounts_DataTable(string DbFile = "", string TableName = "Accounts", string WhereClause = "", bool Debug = false)
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\Accounts.sqlite"; }
                string SelectLine = "Select * From [Accounts]";

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }

                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


                DataTable table = new DataTable();
                table.Columns.Add("SiteName", typeof(string));
                table.Columns.Add("AccountOrder", typeof(string));
                table.Columns.Add("AccountUserName", typeof(string));
                table.Columns.Add("AccountPass", typeof(string));
                table.Columns.Add("AccountVerified", typeof(string));
                table.Columns.Add("AccountEnabled", typeof(string));
                table.Columns.Add("LastChecked", typeof(string));

                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        Accounts returnObject = new Accounts();

                        returnObject.SiteName = ToSite(ret["SiteName"].ToString());
                        returnObject.AccountOrder = ret["AccountOrder"].ToInt();
                        returnObject.AccountUserName = ret["AccountUserName"].ToString();
                        returnObject.AccountPass = ret["AccountPass"].ToString();
                        returnObject.AccountVerified = ret["AccountVerified"].ToBool();
                        returnObject.AccountEnabled = ret["AccountEnabled"].ToBool();
                        returnObject.LastChecked = ret["LastChecked"].ToDateTime();

                        table.Rows.Add(returnObject.SiteName, returnObject.AccountOrder, returnObject.AccountUserName, returnObject.AccountPass, returnObject.AccountVerified, returnObject.AccountEnabled, returnObject.LastChecked);
                    }
                }

                return table;
            }

            public DataTable Create_Accounts_DataTable(Accounts inObject)
            {
                DataTable table = new DataTable();
                table.Columns.Add("SiteName", typeof(string));
                table.Columns.Add("AccountOrder", typeof(string));
                table.Columns.Add("AccountUserName", typeof(string));
                table.Columns.Add("AccountPass", typeof(string));
                table.Columns.Add("AccountVerified", typeof(string));
                table.Columns.Add("AccountEnabled", typeof(string));
                table.Columns.Add("LastChecked", typeof(string));

                table.Rows.Add(inObject.SiteName, inObject.AccountOrder, inObject.AccountUserName, inObject.AccountPass, inObject.AccountVerified, inObject.AccountEnabled, inObject.LastChecked);
                return table;
            }


            #endregion
            #region ===== Accounts DataGridView =====

            public void HideShow_Accounts_Columns(DataGridView dv)
            {

                try { dv.Columns["SiteName"].Visible = true; } catch { }
                try { dv.Columns["AccountOrder"].Visible = true; } catch { }
                try { dv.Columns["AccountUserName"].Visible = true; } catch { }
                try { dv.Columns["AccountPass"].Visible = true; } catch { }
                try { dv.Columns["AccountVerified"].Visible = true; } catch { }
                try { dv.Columns["AccountEnabled"].Visible = true; } catch { }
                try { dv.Columns["LastChecked"].Visible = true; } catch { }
            }
            public void Enable_Accounts_Columns(DataGridView dv)
            {

                try { dv.Columns["SiteName"].ReadOnly = true; } catch { }
                try { dv.Columns["AccountOrder"].ReadOnly = true; } catch { }
                try { dv.Columns["AccountUserName"].ReadOnly = true; } catch { }
                try { dv.Columns["AccountPass"].ReadOnly = true; } catch { }
                try { dv.Columns["AccountVerified"].ReadOnly = true; } catch { }
                try { dv.Columns["AccountEnabled"].ReadOnly = true; } catch { }
                try { dv.Columns["LastChecked"].ReadOnly = true; } catch { }
            }

            #endregion
            #region ===== Accounts SQL Functions =====

            // Return Accounts SQL Connection String
            public SqlConnection Accounts_Conn()
            {
                // populate sql connection
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMLucidMedia"].ConnectionString);
                return conn;
            }

            // Return Accounts TableName (Full Path)
            public string Accounts_TableName()
            {
                // populate to return full sql table name
                return "[LucidMedia].[dbo].[Accounts]";
            }

            // Generate SQL Table
            public bool Accounts_CreateSQLTable()
            {
                SqlConnection Conn = Accounts_Conn();
                string CreateTableLine = "CREATE TABLE [Accounts](";
                CreateTableLine = CreateTableLine + "[SiteName] [int] IDENTITY(1,1) NOT NULL,";
                CreateTableLine = CreateTableLine + "[AccountOrder] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[AccountUserName] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[AccountPass] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[AccountVerified] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[AccountEnabled] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[LastChecked] [varchar](max) NOT NULL";
                CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                return false;
            }

            public bool Accounts_InsertSQL(Accounts obj)
            {
                SqlConnection Con = Accounts_Conn();
                string SQLLine = "Insert Into " + Accounts_TableName() + " (SiteName, AccountOrder, AccountUserName, AccountPass, AccountVerified, AccountEnabled, LastChecked) VALUES (@SiteName, @AccountOrder, @AccountUserName, @AccountPass, @AccountVerified, @AccountEnabled, @LastChecked)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                //if (obj.SiteName == null) { obj.SiteName = ""; }
                //if (obj.AccountOrder == null) { obj.AccountOrder = ""; }
                //if (obj.AccountUserName == null) { obj.AccountUserName = ""; }
                //if (obj.AccountPass == null) { obj.AccountPass = ""; }
                //if (obj.AccountVerified == null) { obj.AccountVerified = ""; }
                //if (obj.AccountEnabled == null) { obj.AccountEnabled = ""; }
                //if (obj.LastChecked == null) { obj.LastChecked = ""; }

                cmd2.Parameters.AddWithValue(@"SiteName", obj.SiteName.ToString());
                cmd2.Parameters.AddWithValue(@"AccountOrder", obj.AccountOrder.ToString());
                cmd2.Parameters.AddWithValue(@"AccountUserName", obj.AccountUserName.ToString());
                cmd2.Parameters.AddWithValue(@"AccountPass", obj.AccountPass.ToString());
                cmd2.Parameters.AddWithValue(@"AccountVerified", obj.AccountVerified.ToString());
                cmd2.Parameters.AddWithValue(@"AccountEnabled", obj.AccountEnabled.ToString());
                cmd2.Parameters.AddWithValue(@"LastChecked", obj.LastChecked.ToString());
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        Accounts_InsertSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        Accounts_InsertSQL(obj);
                    }
                    else
                    {
                        ahk.MsgBox(ex.ToString());
                        return false;
                    }
                }
                Con.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool Accounts_UpdateSQL(Accounts obj)
            {
                SqlConnection Conn = Accounts_Conn();
                string SQLLine = "Update " + Accounts_TableName() + " SET AccountOrder = @AccountOrder, AccountPass = @AccountPass, AccountVerified = @AccountVerified, AccountEnabled = @AccountEnabled, LastChecked = @LastChecked WHERE SiteName = @SiteName AND AccountUserName = @AccountUserName";

                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                //if (obj.SiteName == null) { obj.SiteName = ""; }
                //if (obj.AccountOrder == null) { obj.AccountOrder = ""; }
                //if (obj.AccountUserName == null) { obj.AccountUserName = ""; }
                //if (obj.AccountPass == null) { obj.AccountPass = ""; }
                //if (obj.AccountVerified == null) { obj.AccountVerified = ""; }
                //if (obj.AccountEnabled == null) { obj.AccountEnabled = ""; }
                //if (obj.LastChecked == null) { obj.LastChecked = ""; }

                cmd2.Parameters.AddWithValue(@"SiteName", obj.SiteName.ToString());
                cmd2.Parameters.AddWithValue(@"AccountOrder", obj.AccountOrder.ToString());
                cmd2.Parameters.AddWithValue(@"AccountUserName", obj.AccountUserName.ToString());
                cmd2.Parameters.AddWithValue(@"AccountPass", obj.AccountPass.ToString());
                cmd2.Parameters.AddWithValue(@"AccountVerified", obj.AccountVerified.ToString());
                cmd2.Parameters.AddWithValue(@"AccountEnabled", obj.AccountEnabled.ToString());
                cmd2.Parameters.AddWithValue(@"LastChecked", obj.LastChecked.ToString());

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex)
                {
                    if (ex.Number == 1205) // Retry on DeadLock
                    {
                        ahk.Sleep(1000);
                        Accounts_UpdateSQL(obj);
                    }
                    else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                    {
                        ahk.Sleep(1000);
                        Accounts_UpdateSQL(obj);
                    }
                    else
                    {
                        ahk.MsgBox(ex.ToString());
                        return false;
                    }
                }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool Accounts_UpdateInsert(Accounts obj)
            {
                SqlConnection Conn = Accounts_Conn();
                bool Updated = Accounts_UpdateSQL(obj);  // try to update record first
                if (!Updated) { Updated = Accounts_InsertSQL(obj); }  // if unable to update, insert new record
                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool Accounts_UpdateIfPopulated(Accounts obj, string ID = "")
            {
                SqlConnection Conn = Accounts_Conn();
                string SQLcmd = "Update " + Accounts_TableName() + " SET ";
                if (obj.SiteName != null) { SQLcmd = SQLcmd + " SiteName = @SiteName,"; }
                if (obj.AccountOrder != null) { SQLcmd = SQLcmd + " AccountOrder = @AccountOrder,"; }
                if (obj.AccountUserName != null) { SQLcmd = SQLcmd + " AccountUserName = @AccountUserName,"; }
                if (obj.AccountPass != null) { SQLcmd = SQLcmd + " AccountPass = @AccountPass,"; }
                if (obj.AccountVerified != null) { SQLcmd = SQLcmd + " AccountVerified = @AccountVerified,"; }
                if (obj.AccountEnabled != null) { SQLcmd = SQLcmd + " AccountEnabled = @AccountEnabled,"; }
                if (obj.LastChecked != null) { SQLcmd = SQLcmd + " LastChecked = @LastChecked,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ID = @ID";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.SiteName != null) { cmd2.Parameters.AddWithValue(@"SiteName", obj.SiteName); }
                if (obj.AccountOrder != null) { cmd2.Parameters.AddWithValue(@"AccountOrder", obj.AccountOrder); }
                if (obj.AccountUserName != null) { cmd2.Parameters.AddWithValue(@"AccountUserName", obj.AccountUserName); }
                if (obj.AccountPass != null) { cmd2.Parameters.AddWithValue(@"AccountPass", obj.AccountPass); }
                if (obj.AccountVerified != null) { cmd2.Parameters.AddWithValue(@"AccountVerified", obj.AccountVerified); }
                if (obj.AccountEnabled != null) { cmd2.Parameters.AddWithValue(@"AccountEnabled", obj.AccountEnabled); }
                if (obj.LastChecked != null) { cmd2.Parameters.AddWithValue(@"LastChecked", obj.LastChecked); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public Accounts ReturnAccount(Sites SiteName, string AccountUserName)
            {
                _Database.SQL sql = new _Database.SQL();
                SqlConnection Conn = Accounts_Conn();
                string SelectLine = "Select [SiteName],[AccountOrder],[AccountUserName],[AccountPass],[AccountVerified],[AccountEnabled],[LastChecked] From " + Accounts_TableName() + " WHERE [SiteName] = '" + SiteName.ToString() + "' AND [AccountUserName] = '" + AccountUserName + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                Accounts returnObject = new Accounts();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.SiteName = ToSite(ret["SiteName"].ToString());
                        returnObject.AccountOrder = ret["AccountOrder"].ToInt();
                        returnObject.AccountUserName = ret["AccountUserName"].ToString();
                        returnObject.AccountPass = DecryptPass(returnObject.SiteName, ret["AccountPass"].ToString());
                        returnObject.AccountVerified = ret["AccountVerified"].ToBool();
                        returnObject.AccountEnabled = ret["AccountEnabled"].ToBool();
                        returnObject.LastChecked = ret["LastChecked"].ToDateTime();

                        return returnObject;
                    }
                }

                return returnObject;
            }

            public List<Accounts> Return_Accounts(Sites sites = Sites.All)
            {
                _Database.SQL sql = new _Database.SQL();
                string Command = "Select * From " + Accounts_TableName();
                if (sites != Sites.All)
                {
                    Command = "Select * From " + Accounts_TableName() + " WHERE SiteName = '" + sites.ToString() + "'";
                }

                SqlConnection Conn = Accounts_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<Accounts> ReturnList = new List<Accounts>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        Accounts returnObject = new Accounts();
                        returnObject.SiteName = ToSite(ret["SiteName"].ToString());
                        returnObject.AccountOrder = ret["AccountOrder"].ToInt();
                        returnObject.AccountUserName = ret["AccountUserName"].ToString();
                        returnObject.AccountPass = DecryptPass(returnObject.SiteName, ret["AccountPass"].ToString());
                        returnObject.AccountVerified = ret["AccountVerified"].ToBool();
                        returnObject.AccountEnabled = ret["AccountEnabled"].ToBool();
                        returnObject.LastChecked = ret["LastChecked"].ToDateTime();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }

            public bool Accounts_SQL_to_SQLite(string SqliteDBPath = @"\Db\Accounts.sqlite")
            {
                _Database.SQLite sqlite = new _Database.SQLite();
                string SaveFile = SqliteDBPath;
                if (SqliteDBPath == @"\Db\Accounts.sqlite")
                {
                    ahk.FileCreateDir(ahk.AppDir() + @"\Db");
                    SaveFile = ahk.AppDir() + @"\Db\Accounts.sqlite";
                }

                //sb.StatusBar("Copying SQL Db to " + SaveFile + "...");
                sqlite.SQLTable_To_NewSQLiteTable(Accounts_Conn(), "Accounts", "Accounts", SaveFile, "", false, false, false);
                //sb.StatusBar("FINISHED Copying SQL Db to " + SaveFile);

                if (File.Exists(SaveFile)) { return true; } else { return false; }
            }


            #endregion

            #endregion

        }























    }



}

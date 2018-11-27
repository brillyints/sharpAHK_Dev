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
using System.Security.Principal;
using System.ServiceProcess;
using System.Data;
using System.IO;
using Telerik.WinControls.UI;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Configuration;
using System.Management;
using System.Threading;

namespace sharpAHK_Dev
{
    public class _WinServices
    {
        #region === Install / Uninstall Services ===


        /// <summary>
        /// Define InstallUtil Path
        /// </summary>
        public static string util32 = @"C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe";
        public static string util64 = @"C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe";


        /// <summary>
        /// Confirm User Has Admin Access to Install/Uninstall/Manage Services
        /// </summary>
        /// <returns></returns>
        public bool IsUserAdministrator()
        {
            bool isAdmin;
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }

        /// <summary>
        /// Returns Either 32bit or 64bit InstallUtil based on whether using 64bit machine
        /// </summary>
        /// <returns></returns>
        public string InstallUtilPath()
        {
            string path = "";

            if (Is64Bit()) { path = Util64(); }
            else { path = Util32(); }

            if (path == "")
            {
                _AHK ahk = new _AHK();
                ahk.MsgBox("Unable to Locate InstallUtil.exe | Unable to Install/Uninstall");
                return "";
            }

            return path;
        }

        public bool Is64Bit()
        {
            return Environment.Is64BitOperatingSystem;
        }

        public string Util32()
        {
            string returnPath = "";
            if (File.Exists(util32)) { returnPath = util32; }
            return returnPath;
        }
        public string Util64()
        {
            string returnPath = "";
            if (File.Exists(util64)) { returnPath = util64; }
            return returnPath;
        }



        public void InstallService(string ServiceEXEPath)
        {
            _AHK ahk = new _AHK();
            _Database.SQLite sqlite = new _Database.SQLite();

            //string InstallUtil = Util64();
            string InstallUtil = InstallUtilPath();

            string runCmd = InstallUtil + " \"" + ServiceEXEPath + "\"";

            if (!File.Exists(InstallUtil)) { ahk.MsgBox(InstallUtil + " NOT FOUND"); }
            else
            {
                //sqlite.Setting("ServiceInstallPath", ServiceEXEPath);
                //ahk.Run(runCmd, "", "Hide");
                ahk.Run(runCmd);
            }


            string login = "litm\\jason";
            string pass = "packman";

            ahk.WinWait("Set Service Login");
            ahk.WinActivate("Set Service Login");
            ahk.Send(login + "{tab}"); ahk.Sleep(100);
            ahk.Send(pass + "{tab}"); ahk.Sleep(100);
            ahk.Send(pass + "{tab}"); ahk.Sleep(100);
            //ahk.Send("{enter}");
        }

        public string ServiceInstaller(string ServiceEXEPath, bool Hidden = true, bool Bit64 = true, int TimeOut = 10000, string WinUser = "litm\\jason", string WinPass = "pass")
        {
            _AHK ahk = new _AHK();
            _Database.SQLite sqlite = new _Database.SQLite();

            //string InstallUtil = Util64();
            string InstallUtil = InstallUtilPath();

            string runCmd = "\"" + ServiceEXEPath + "\"";
            string exepath = "";


            if (!File.Exists(InstallUtil)) { ahk.MsgBox(InstallUtil + " NOT FOUND"); return ""; }

            sqlite.Setting("ServiceInstallPath", ServiceEXEPath);


            //create a process info
            //ProcessStartInfo oInfo = new ProcessStartInfo(this.FFmpegPath, Parameters);
            ProcessStartInfo oInfo = new ProcessStartInfo(InstallUtil, runCmd);
            oInfo.UseShellExecute = false;

            oInfo.RedirectStandardOutput = true;
            oInfo.RedirectStandardError = true;

            oInfo.UseShellExecute = false;  //testing
            oInfo.CreateNoWindow = Hidden;  //testing


            //Create the output and streamreader to get the output
            string output = null; StreamReader srOutput = null;

            //try the process
            try
            {
                //run the process
                Process proc = System.Diagnostics.Process.Start(oInfo);

                //proc.WaitForExit(TimeOut); // 10 seconds timeout

                string login = WinUser;
                string pass = WinPass;

                ahk.WinWait("Set Service Login");
                ahk.WinActivate("Set Service Login");
                ahk.Send(login + "{tab}"); ahk.Sleep(100);
                ahk.Send(pass + "{tab}"); ahk.Sleep(100);
                ahk.Send(pass + "{tab}"); ahk.Sleep(100);
                //ahk.Send("{enter}");

                proc.WaitForExit(TimeOut); // 10 seconds timeout

                //get the output
                srOutput = proc.StandardError;

                //now put it in a string
                output = srOutput.ReadToEnd();

                //proc.Close();
            }
            catch (Exception ex)
            {
                output = string.Empty;
                ahk.MsgBox("Exception Caught: " + ex.ToString());
            }
            finally
            {
                //now, if we succeded, close out the streamreader
                if (srOutput != null)
                {
                    srOutput.Close();
                    srOutput.Dispose();
                }
            }

            ahk.MsgBox(output);

            return output;
        }

        public void UnInstallService(string ServiceEXEPath, bool Bit64 = true)
        {
            _AHK ahk = new _AHK();

            //string InstallUtil = Util64();
            string InstallUtil = InstallUtilPath();

            string runCmd = InstallUtil + " /u \"" + ServiceEXEPath + "\"";
            if (!File.Exists(InstallUtil)) { ahk.MsgBox(InstallUtil + " NOT FOUND"); }
            else { ahk.Run(runCmd); }

            //ahk.Run(runCmd);
        }



        #endregion

        #region === Service Actions ===

        public void StartService(ServiceController service)
        {
            if (service.CanStop)
            {
                if (service.Status.ToString() == "Stopped") { service.Start(); }
            }
        }
        public void StartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch
            {
                // ...
            }
        }


        public void StopService(ServiceController service)
        {
            if (service.CanStop)
            {
                if (service.Status.ToString() == "Running") { service.Stop(); }
            }
        }
        public void StopService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch
            {
                // ...
            }
        }



        public void RestartService(string serviceName, int timeoutMilliseconds = 5000)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                int millisec1 = Environment.TickCount;
                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                // count the rest of the timeout
                int millisec2 = Environment.TickCount;
                timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds - (millisec2 - millisec1));

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch
            {
                // ...
            }
        }



        #endregion

        #region === Service INFO ===

        /// <summary>
        /// Returns Windows Service Control For Service By ServiceName
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <returns></returns>
        public ServiceController ReturnService(string ServiceName)
        {
            ServiceController service = new ServiceController();

            // return windows service
            List<ServiceController> services = Services(ServiceName);
            if (services.Count > 0)
            {
                try
                {
                    service = services[0];
                }
                catch { }
            }

            return service;
        }


        /// <summary>
        /// Check Windows Services to See if Service Is Currently Running (Status = Running)
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <returns></returns>
        public bool ServiceRunning(string ServiceName)
        {
            // get list of Windows services
            ServiceController[] services = ServiceController.GetServices();

            // try to find service name
            foreach (ServiceController service in services)
            {
                if (service.ServiceName == ServiceName)
                {
                    if (service.Status.ToString() == "Running") { return true; }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns list of Windows Service Names Installed
        /// </summary>
        /// <returns></returns>
        public List<string> ServiceList()
        {
            // get list of Windows services
            ServiceController[] services = ServiceController.GetServices();

            List<string> Services = new List<string>();

            // try to find service name
            foreach (ServiceController service in services)
            {
                Services.Add(service.ServiceName);

                //if (service.ServiceName == serviceName)
                //    return true;
            }

            return Services;
        }


        /// <summary>
        /// Returns List of Windows Services
        /// </summary>
        /// <param name="InName">If Populated, Limits Services Returned to Items Containing InName Text</param>
        /// <returns></returns>
        public List<ServiceController> Services(string InName = "")
        {
            // get list of Windows services
            ServiceController[] services = ServiceController.GetServices();

            List<ServiceController> SERvices = new List<ServiceController>();

            // try to find service name
            foreach (ServiceController service in services)
            {
                if (InName != "")
                {
                    if (service.ServiceName.ToUpper().Contains(InName.ToUpper())) { SERvices.Add(service); }
                }
                else { SERvices.Add(service); }
            }

            return SERvices;
        }

        /// <summary>
        /// Returns True if Service Name Found Installed on PC
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <returns></returns>
        public bool ServiceInstalled(string ServiceName)
        {
            // get list of Windows services
            ServiceController[] services = ServiceController.GetServices();

            // try to find service name
            foreach (ServiceController service in services)
            {
                if (service.ServiceName == ServiceName)
                    return true;
            }

            return false;
        }


        /// <summary>
        /// Return Service Name from Installed EXE Service Path
        /// </summary>
        /// <param name="ServiceEXE"></param>
        /// <returns></returns>
        public string ServiceNameFromEXE(string ServiceEXE)
        {
            List<string> services = ServiceList();
            foreach(string service in services)
            {
                string path = ServicePath(service);

                if (ServiceEXE.ToUpper() == path.ToUpper()) { return service; }
            }

            return "";
        }


        /// <summary>
        /// Returns EXE Path for Service based on ServiceName
        /// </summary>
        /// <param name="ServiceName">Name of Service to Return Local EXE Path</param>
        /// <returns></returns>
        public string ServicePath(string ServiceName)
        {
            string servicePath = "";
            using (ManagementObject wmiService = new ManagementObject("Win32_Service.Name='" + ServiceName + "'"))
            {
                wmiService.Get();
                try
                {
                    string currentserviceExePath = wmiService["PathName"].ToString();
                    servicePath = wmiService["PathName"].ToString();
                }
                catch
                {
                    return "";
                }
            }

            servicePath = servicePath.Replace("\"", "");

            return servicePath;
        }

        /// <summary>
        /// Based on Service Path, Checks to See if Service Installed On This PC
        /// </summary>
        /// <param name="servicePath"></param>
        /// <returns></returns>
        public bool ServicePath_Installed(string servicePath)
        {
            List<ServiceController> services = Services(); bool ServiceInstalled = false;
            foreach (ServiceController service in services)
            {
                string serPath = ServicePath(service.ServiceName).Replace("\"", ""); // update display with path to loaded service (removes quotes)
                if (serPath == servicePath.Replace("\"", "")) { ServiceInstalled = true; break; }
            }

            return ServiceInstalled;
        }


        #endregion

        #region === Service Display ===

        /// <summary>
        /// Populate RadTree with List of Stored SQL Commands Related to ServiceName
        /// </summary>
        /// <param name="RadTree"></param>
        /// <param name="ServiceName"></param>
        public void LoadServiceTables(RadTreeView RadTree, string ServiceName, bool NewThread = true)
        {
            if (NewThread)
            {
                Thread newThread = new Thread(() => LoadServiceTables(RadTree, ServiceName, false)); // Function To Execute
                newThread.Name = "LoadServiceTables";
                newThread.IsBackground = true;
                newThread.Start();
            }
            else
            {
                _TelerikLib.RadTree tree = new _TelerikLib.RadTree();
                RadTree.ClearTree();

                List<ServiceTables> tables = ServiceTableList(ServiceName);

                foreach (ServiceTables table in tables)
                {
                    RadTreeNode node = new RadTreeNode();
                    node.Text = table.TableDisplayName;
                    node.Tag = table.TableConnName + "|" + table.TableSQL;
                    //RadTree.Nodes.Add(node);
                    tree.AddNode(RadTree, node);
                }
            }
        }

        /// <summary>
        /// Display Details about specific Windows Service
        /// </summary>
        /// <param name="service"></param>
        public void ServiceInfoDisp(ServiceController service)
        {
            _AHK ahk = new _AHK();
            string servicePath = ServicePath(service.ServiceName);
            ahk.MsgBox("ServiceName: " + service.ServiceName + "\nStatus: " + service.Status + "\nStartType: " + service.ServiceType + "\nCanStop: " + service.CanStop + "\nCanPause: " + service.CanPauseAndContinue + "\nDisplayName: " + service.DisplayName + "\nMachineName: " + service.MachineName + "\nServiceType: " + service.ServiceType + "\nSite: " + service.Site + "\nServicePath: " + servicePath);
        }

        public void ServiceTreeDisp(RadTreeView RadTree)
        {
            _TelerikLib.RadTree tree = new _TelerikLib.RadTree();


        }


        #endregion

        #region === Service Database Calls ===


        #region === Initial Setup ===

        /// <summary>
        /// Write Initial Service Tables to SQL Db
        /// </summary>
        public void CreateServiceLogTables()
        {
            _AHK ahk = new _AHK();

            int createdCount = 0;

            bool created = ServiceLog_CreateSQLTable(ServiceLog_TableName());
            if (created) { createdCount++; }

            created = ServiceConfig_CreateSQLTable(ServiceConfig_TableName());
            if (created) { createdCount++; }

            created = ServiceTables_CreateSQLTable(ServiceTables_TableName());
            if (created) { createdCount++; }

            ahk.MsgBox("Created " + createdCount + " / 3 Service Tables");
        }


        #endregion


        #region === Service Log ===

        #region ===== ServiceLog Object =====

        public struct ServiceLog
        {
            public int ID { get; set; }
            public string ServiceName { get; set; }
            public string LogType { get; set; }
            public string LogMessage { get; set; }
            public DateTime LogTime { get; set; }
            public int InsertCount { get; set; }
            public int UpdateCount { get; set; }
            public int DeleteCount { get; set; }
            public int ErrorCount { get; set; }
        }


        #endregion
        #region ===== ServiceLog SQLite : Return =====

        public ServiceLog Return_Object_From_ServiceLog(string DbFile, string WhereClause = "[ID] = ''")
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            string SelectLine = "Select [ID], [ServiceName], [LogType], [LogMessage], [LogTime], [InsertCount], [UpdateCount], [DeleteCount], [ErrorCount] From [ServiceLog] ";
            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
            if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
            if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
            ServiceLog returnObject = new ServiceLog();
            int i = 0;
            string Value = "";
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    returnObject.ID = ret["ID"].ToInt();
                    returnObject.ServiceName = ret["ServiceName"].ToString();
                    returnObject.LogType = ret["LogType"].ToString();
                    returnObject.LogMessage = ret["LogMessage"].ToString();
                    returnObject.LogTime = ret["LogTime"].ToDateTime();
                    returnObject.InsertCount = ret["InsertCount"].ToInt();
                    returnObject.UpdateCount = ret["UpdateCount"].ToInt();
                    returnObject.DeleteCount = ret["DeleteCount"].ToInt();
                    returnObject.ErrorCount = ret["ErrorCount"].ToInt();
                }
            }

            return returnObject;
        }

        public List<ServiceLog> Return_ServiceLog_List(string DbFile, string TableName = "[ServiceLog]", string WhereClause = "")
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            string SelectLine = "Select * From " + TableName;

            if (WhereClause != "")
            {
                if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
            }
            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

            List<ServiceLog> ReturnList = new List<ServiceLog>();
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    ServiceLog returnObject = new ServiceLog();

                    returnObject.ID = ret["ID"].ToInt();
                    returnObject.ServiceName = ret["ServiceName"].ToString();
                    returnObject.LogType = ret["LogType"].ToString();
                    returnObject.LogMessage = ret["LogMessage"].ToString();
                    returnObject.LogTime = ret["LogTime"].ToDateTime();
                    returnObject.InsertCount = ret["InsertCount"].ToInt();
                    returnObject.UpdateCount = ret["UpdateCount"].ToInt();
                    returnObject.DeleteCount = ret["DeleteCount"].ToInt();
                    returnObject.ErrorCount = ret["ErrorCount"].ToInt();

                    ReturnList.Add(returnObject);
                }
            }

            return ReturnList;
        }

        public DataTable Return_DataTable_From_ServiceLog(string DbFile)
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            string SelectLine = "Select [ID], [ServiceName], [LogType], [LogMessage], [LogTime], [InsertCount], [UpdateCount], [DeleteCount], [ErrorCount] From [ServiceLog]";
            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
            return ReturnTable;
        }


        #endregion
        #region ===== ServiceLog SQLite : Update Insert =====

        public bool ServiceLog_Insert(string DbFile, ServiceLog inObject)
        {
            _AHK ahk = new _AHK();
            _Database.SQLite sqlite = new _Database.SQLite();
            string InsertLine = "Insert Into [ServiceLog] (ID, ServiceName, LogType, LogMessage, LogTime, InsertCount, UpdateCount, DeleteCount, ErrorCount) values ('" + inObject.ID + "', '" + inObject.ServiceName + "', '" + inObject.LogType + "', '" + inObject.LogMessage + "', '" + inObject.LogTime + "', '" + inObject.InsertCount + "', '" + inObject.UpdateCount + "', '" + inObject.DeleteCount + "', '" + inObject.ErrorCount + "')";
            bool Inserted = sqlite.Execute(DbFile, InsertLine);
            if (!Inserted) { ahk.MsgBox("Inserted Into [ServiceLog] = " + Inserted.ToString()); }
            return Inserted;
        }

        public bool ServiceLog_Update(string DbFile, ServiceLog inObject, string WHERE = "[Item] = 'Value'")
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            _AHK ahk = new _AHK();

            //string UpdateLine = "Update [ServiceLog] set ID = '" + inObject.ID + "', ServiceName = '" + inObject.ServiceName + "', LogType = '" + inObject.LogType + "', LogMessage = '" + inObject.LogMessage + "', LogTime = '" + inObject.LogTime + "', InsertCount = '" + inObject.InsertCount + "', UpdateCount = '" + inObject.UpdateCount + "', DeleteCount = '" + inObject.DeleteCount + "', ErrorCount = '" + inObject.ErrorCount + "' WHERE [Item] = 'Value' ";
            string UpdateLine = "Update [ServiceLog] set ";


            //if (inObject.ID != null) { UpdateLine = UpdateLine + "[ID] = '" + inObject.ID + "',"; }
            //if (inObject.ServiceName != null) { UpdateLine = UpdateLine + "[ServiceName] = '" + inObject.ServiceName + "',"; }
            //if (inObject.LogType != null) { UpdateLine = UpdateLine + "[LogType] = '" + inObject.LogType + "',"; }
            //if (inObject.LogMessage != null) { UpdateLine = UpdateLine + "[LogMessage] = '" + inObject.LogMessage + "',"; }
            //if (inObject.LogTime != null) { UpdateLine = UpdateLine + "[LogTime] = '" + inObject.LogTime + "',"; }
            //if (inObject.InsertCount != null) { UpdateLine = UpdateLine + "[InsertCount] = '" + inObject.InsertCount + "',"; }
            //if (inObject.UpdateCount != null) { UpdateLine = UpdateLine + "[UpdateCount] = '" + inObject.UpdateCount + "',"; }
            //if (inObject.DeleteCount != null) { UpdateLine = UpdateLine + "[DeleteCount] = '" + inObject.DeleteCount + "',"; }
            //if (inObject.ErrorCount != null) { UpdateLine = UpdateLine + "[ErrorCount] = '" + inObject.ErrorCount + "',"; }

            UpdateLine = ahk.TrimLast(UpdateLine, 1);
            UpdateLine = UpdateLine + " WHERE " + WHERE;

            bool Updated = sqlite.Execute(DbFile, UpdateLine);
            return Updated;
        }

        public bool ServiceLog_UpdateInsert(string DbFile, ServiceLog obj, string WhereClause = "")
        {
            bool Updated = ServiceLog_Update(DbFile, obj, WhereClause);  // try to update record first
            if (!Updated) { Updated = ServiceLog_Insert(DbFile, obj); }  // if unable to update, insert new record
            return Updated;
        }


        #endregion
        #region ===== ServiceLog DataTable =====

        public DataTable Return_ServiceLog_DataTable(string DbFile, string TableName = "ServiceLog", string WhereClause = "", bool Debug = false)
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            string SelectLine = "Select * From [ServiceLog]";

            if (WhereClause != "")
            {
                if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
            }

            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


            DataTable table = new DataTable();
            table.Columns.Add("ID", typeof(string));
            table.Columns.Add("ServiceName", typeof(string));
            table.Columns.Add("LogType", typeof(string));
            table.Columns.Add("LogMessage", typeof(string));
            table.Columns.Add("LogTime", typeof(string));
            table.Columns.Add("InsertCount", typeof(string));
            table.Columns.Add("UpdateCount", typeof(string));
            table.Columns.Add("DeleteCount", typeof(string));
            table.Columns.Add("ErrorCount", typeof(string));

            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    ServiceLog returnObject = new ServiceLog();

                    returnObject.ID = ret["ID"].ToInt();
                    returnObject.ServiceName = ret["ServiceName"].ToString();
                    returnObject.LogType = ret["LogType"].ToString();
                    returnObject.LogMessage = ret["LogMessage"].ToString();
                    returnObject.LogTime = ret["LogTime"].ToDateTime();
                    returnObject.InsertCount = ret["InsertCount"].ToInt();
                    returnObject.UpdateCount = ret["UpdateCount"].ToInt();
                    returnObject.DeleteCount = ret["DeleteCount"].ToInt();
                    returnObject.ErrorCount = ret["ErrorCount"].ToInt();

                    table.Rows.Add(returnObject.ID, returnObject.ServiceName, returnObject.LogType, returnObject.LogMessage, returnObject.LogTime, returnObject.InsertCount, returnObject.UpdateCount, returnObject.DeleteCount, returnObject.ErrorCount);
                }
            }

            return table;
        }

        public DataTable Create_ServiceLog_DataTable(ServiceLog inObject)
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", typeof(string));
            table.Columns.Add("ServiceName", typeof(string));
            table.Columns.Add("LogType", typeof(string));
            table.Columns.Add("LogMessage", typeof(string));
            table.Columns.Add("LogTime", typeof(string));
            table.Columns.Add("InsertCount", typeof(string));
            table.Columns.Add("UpdateCount", typeof(string));
            table.Columns.Add("DeleteCount", typeof(string));
            table.Columns.Add("ErrorCount", typeof(string));

            table.Rows.Add(inObject.ID, inObject.ServiceName, inObject.LogType, inObject.LogMessage, inObject.LogTime, inObject.InsertCount, inObject.UpdateCount, inObject.DeleteCount, inObject.ErrorCount);
            return table;
        }


        #endregion
        #region ===== ServiceLog DataGridView =====

        public void HideShow_ServiceLog_Columns(DataGridView dv)
        {

            try { dv.Columns["ID"].Visible = true; } catch { }
            try { dv.Columns["ServiceName"].Visible = true; } catch { }
            try { dv.Columns["LogType"].Visible = true; } catch { }
            try { dv.Columns["LogMessage"].Visible = true; } catch { }
            try { dv.Columns["LogTime"].Visible = true; } catch { }
            try { dv.Columns["InsertCount"].Visible = true; } catch { }
            try { dv.Columns["UpdateCount"].Visible = true; } catch { }
            try { dv.Columns["DeleteCount"].Visible = true; } catch { }
            try { dv.Columns["ErrorCount"].Visible = true; } catch { }
        }
        public void Enable_TableName_Columns(DataGridView dv)
        {

            try { dv.Columns["ID"].ReadOnly = true; } catch { }
            try { dv.Columns["ServiceName"].ReadOnly = true; } catch { }
            try { dv.Columns["LogType"].ReadOnly = true; } catch { }
            try { dv.Columns["LogMessage"].ReadOnly = true; } catch { }
            try { dv.Columns["LogTime"].ReadOnly = true; } catch { }
            try { dv.Columns["InsertCount"].ReadOnly = true; } catch { }
            try { dv.Columns["UpdateCount"].ReadOnly = true; } catch { }
            try { dv.Columns["DeleteCount"].ReadOnly = true; } catch { }
            try { dv.Columns["ErrorCount"].ReadOnly = true; } catch { }
        }

        #endregion
        #region ===== ServiceLog SQL Functions =====

        // Return ServiceLog SQL Connection String
        public SqlConnection ServiceLog_Conn()
        {
            // populate sql connection
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ServiceDb"].ConnectionString);
            if (conn == null)
            {
                _AHK ahk = new _AHK();
                ahk.FileAppend("ServiceDb Returned Null", ahk.AppDir() + "\\Service_ErrorLog.txt");
            }
            return conn;
        }

        // Return ServiceLog TableName (Full Path)
        public string ServiceLog_TableName()
        {
            string TableNameBase = ConfigurationManager.ConnectionStrings["ServiceTableRoot"].ConnectionString;

            if (TableNameBase == "")
            {
                _AHK ahk = new _AHK();
                ahk.FileAppend("ServiceTableRoot Not Populated in AppConfig", ahk.AppDir() + "\\Service_ErrorLog.txt");
            }

            // populate to return full sql table name
            return TableNameBase + ".[ServiceLog]";
        }

        // Generate SQL Table
        public bool ServiceLog_CreateSQLTable(string NewTablePath = "")
        {
            if (NewTablePath == "") { NewTablePath = ServiceLog_TableName(); }

            _Database.SQL sql = new _Database.SQL();
            SqlConnection Conn = ServiceLog_Conn();
            string CreateTableLine = "CREATE TABLE " + NewTablePath + "(";
            CreateTableLine = CreateTableLine + "[ID] [bigint] IDENTITY(1,1) NOT NULL,";
            CreateTableLine = CreateTableLine + "[ServiceName] [varchar](max) NOT NULL,";
            CreateTableLine = CreateTableLine + "[LogType] [varchar](max) NOT NULL,";
            CreateTableLine = CreateTableLine + "[LogMessage] [varchar](max) NOT NULL,";
            CreateTableLine = CreateTableLine + "[LogTime] [datetime] NOT NULL,";
            CreateTableLine = CreateTableLine + "[InsertCount] [int] NULL,";
            CreateTableLine = CreateTableLine + "[UpdateCount] [int] NULL,";
            CreateTableLine = CreateTableLine + "[DeleteCount] [int] NULL,";
            CreateTableLine = CreateTableLine + "[ErrorCount] [int] NULL";
            CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";

            return sql.WriteDataRecord(Conn, CreateTableLine);
            //return false;
        }

        public bool ServiceLog_InsertSQL(ServiceLog obj)
        {
            _AHK ahk = new _AHK();
            SqlConnection Con = ServiceLog_Conn();
            string SQLLine = "Insert Into " + ServiceLog_TableName() + "(ServiceName, LogType, LogMessage, LogTime, InsertCount, UpdateCount, DeleteCount, ErrorCount) VALUES (@ServiceName, @LogType, @LogMessage, @LogTime, @InsertCount, @UpdateCount, @DeleteCount, @ErrorCount)";
            SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
            cmd2 = new SqlCommand(SQLLine, Con);

            cmd2.Parameters.AddWithValue(@"ServiceName", obj.ServiceName.ToString());
            cmd2.Parameters.AddWithValue(@"LogType", obj.LogType.ToString());
            cmd2.Parameters.AddWithValue(@"LogMessage", obj.LogMessage.ToString());
            cmd2.Parameters.AddWithValue(@"LogTime", DateTime.Now.ToString());
            cmd2.Parameters.AddWithValue(@"InsertCount", obj.InsertCount.ToString());
            cmd2.Parameters.AddWithValue(@"UpdateCount", obj.UpdateCount.ToString());
            cmd2.Parameters.AddWithValue(@"DeleteCount", obj.DeleteCount.ToString());
            cmd2.Parameters.AddWithValue(@"ErrorCount", obj.ErrorCount.ToString());

            if (Con.State == ConnectionState.Closed) { Con.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex)
            {
                if (ex.Number == 1205) // Retry on DeadLock
                {
                    ahk.Sleep(1000);
                    ServiceLog_InsertSQL(obj);
                }
                else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                {
                    ahk.Sleep(1000);
                    ServiceLog_InsertSQL(obj);
                }
                else
                {
                    //ahk.MsgBox(ex.ToString());
                    return false;
                }
            }
            Con.Close();
            if (recordsAffected > 0) { return true; }
            else return false;
        }

        public bool ServiceLog_UpdateSQL(ServiceLog obj)
        {
            _AHK ahk = new _AHK();

            SqlConnection Conn = ServiceLog_Conn();
            string SQLLine = "Update " + ServiceLog_TableName() + " SET LogType = @LogType, LogMessage = @LogMessage, LogTime = @LogTime, InsertCount = @InsertCount, UpdateCount = @UpdateCount, DeleteCount = @DeleteCount, ErrorCount = @ErrorCount WHERE ServiceName = @ServiceName";
            SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
            cmd2 = new SqlCommand(SQLLine, Conn);

            cmd2.Parameters.AddWithValue(@"ServiceName", obj.ServiceName.ToString());
            cmd2.Parameters.AddWithValue(@"LogType", obj.LogType.ToString());
            cmd2.Parameters.AddWithValue(@"LogMessage", obj.LogMessage.ToString());
            cmd2.Parameters.AddWithValue(@"LogTime", obj.LogTime.ToString());
            cmd2.Parameters.AddWithValue(@"InsertCount", obj.InsertCount.ToString());
            cmd2.Parameters.AddWithValue(@"UpdateCount", obj.UpdateCount.ToString());
            cmd2.Parameters.AddWithValue(@"DeleteCount", obj.DeleteCount.ToString());
            cmd2.Parameters.AddWithValue(@"ErrorCount", obj.ErrorCount.ToString());

            if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex)
            {
                if (ex.Number == 1205) // Retry on DeadLock
                {
                    ahk.Sleep(1000);
                    ServiceLog_UpdateSQL(obj);
                }
                else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                {
                    ahk.Sleep(1000);
                    ServiceLog_UpdateSQL(obj);
                }
                else
                {
                    //ahk.MsgBox(ex.ToString());
                    return false;
                }
            }
            Conn.Close();
            if (recordsAffected > 0) { return true; }
            else return false;
        }

        public bool ServiceLog_UpdateInsert(ServiceLog obj)
        {
            SqlConnection Conn = ServiceLog_Conn();
            bool Updated = ServiceLog_UpdateSQL(obj);  // try to update record first
            if (!Updated) { Updated = ServiceLog_InsertSQL(obj); }  // if unable to update, insert new record
            return Updated;
        }

        // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
        public bool ServiceLog_UpdateIfPopulated(ServiceLog obj, string ID = "")
        {
            SqlConnection Conn = ServiceLog_Conn();
            string SQLcmd = "Update ServiceLog SET ";
            if (obj.ID != null) { SQLcmd = SQLcmd + " ID = @ID,"; }
            if (obj.ServiceName != null) { SQLcmd = SQLcmd + " ServiceName = @ServiceName,"; }
            if (obj.LogType != null) { SQLcmd = SQLcmd + " LogType = @LogType,"; }
            if (obj.LogMessage != null) { SQLcmd = SQLcmd + " LogMessage = @LogMessage,"; }
            if (obj.LogTime != null) { SQLcmd = SQLcmd + " LogTime = @LogTime,"; }
            if (obj.InsertCount != null) { SQLcmd = SQLcmd + " InsertCount = @InsertCount,"; }
            if (obj.UpdateCount != null) { SQLcmd = SQLcmd + " UpdateCount = @UpdateCount,"; }
            if (obj.DeleteCount != null) { SQLcmd = SQLcmd + " DeleteCount = @DeleteCount,"; }
            if (obj.ErrorCount != null) { SQLcmd = SQLcmd + " ErrorCount = @ErrorCount,"; }

            _AHK ahk = new _AHK();
            SQLcmd = ahk.TrimLast(SQLcmd, 1);
            SQLcmd = SQLcmd + " WHERE ID = @ID";

            SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

            if (obj.ID != null) { cmd2.Parameters.AddWithValue(@"ID", obj.ID); }
            if (obj.ServiceName != null) { cmd2.Parameters.AddWithValue(@"ServiceName", obj.ServiceName); }
            if (obj.LogType != null) { cmd2.Parameters.AddWithValue(@"LogType", obj.LogType); }
            if (obj.LogMessage != null) { cmd2.Parameters.AddWithValue(@"LogMessage", obj.LogMessage); }
            if (obj.LogTime != null) { cmd2.Parameters.AddWithValue(@"LogTime", obj.LogTime); }
            if (obj.InsertCount != null) { cmd2.Parameters.AddWithValue(@"InsertCount", obj.InsertCount); }
            if (obj.UpdateCount != null) { cmd2.Parameters.AddWithValue(@"UpdateCount", obj.UpdateCount); }
            if (obj.DeleteCount != null) { cmd2.Parameters.AddWithValue(@"DeleteCount", obj.DeleteCount); }
            if (obj.ErrorCount != null) { cmd2.Parameters.AddWithValue(@"ErrorCount", obj.ErrorCount); }

            if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
            Conn.Close();
            if (recordsAffected > 0) { return true; }
            else return false;
        }

        public ServiceLog ServiceLog_ReturnSQL(string ID = "")
        {
            _Database.SQL sql = new _Database.SQL();
            SqlConnection Conn = ServiceLog_Conn();
            string SelectLine = "Select [ID],[ServiceName],[LogType],[LogMessage],[LogTime],[InsertCount],[UpdateCount],[DeleteCount],[ErrorCount] From ServiceLog WHERE ID = '" + ID + "'";
            DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
            ServiceLog returnObject = new ServiceLog();
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    returnObject.ID = ret["ID"].ToInt();
                    returnObject.ServiceName = ret["ServiceName"].ToString();
                    returnObject.LogType = ret["LogType"].ToString();
                    returnObject.LogMessage = ret["LogMessage"].ToString();
                    returnObject.LogTime = ret["LogTime"].ToDateTime();
                    returnObject.InsertCount = ret["InsertCount"].ToInt();
                    returnObject.UpdateCount = ret["UpdateCount"].ToInt();
                    returnObject.DeleteCount = ret["DeleteCount"].ToInt();
                    returnObject.ErrorCount = ret["ErrorCount"].ToInt();
                    return returnObject;
                }
            }
            return returnObject;
        }

        public List<ServiceLog> ServiceLog_ReturnSQLList(string Command)
        {
            _Database.SQL sql = new _Database.SQL();
            SqlConnection Conn = ServiceLog_Conn();
            DataTable ReturnTable = sql.GetDataTable(Conn, Command);
            List<ServiceLog> ReturnList = new List<ServiceLog>();
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    ServiceLog returnObject = new ServiceLog();
                    returnObject.ID = ret["ID"].ToInt();
                    returnObject.ServiceName = ret["ServiceName"].ToString();
                    returnObject.LogType = ret["LogType"].ToString();
                    returnObject.LogMessage = ret["LogMessage"].ToString();
                    returnObject.LogTime = ret["LogTime"].ToDateTime();
                    returnObject.InsertCount = ret["InsertCount"].ToInt();
                    returnObject.UpdateCount = ret["UpdateCount"].ToInt();
                    returnObject.DeleteCount = ret["DeleteCount"].ToInt();
                    returnObject.ErrorCount = ret["ErrorCount"].ToInt();
                    ReturnList.Add(returnObject);
                }
            }
            return ReturnList;
        }


        #endregion

        #endregion


        #region === Service Config ===

        #region ===== ServiceConfig Object =====

        public struct ServiceConfig
        {
            public string ID { get; set; }
            public string ServiceName { get; set; }
            public string ServicePath { get; set; }
            public bool ServiceEnabled { get; set; }
            public int ServiceCheckInMin { get; set; }
            public int ServiceEventTimerMin { get; set; }
            public DateTime SettingDateModified { get; set; }
            public DateTime LastStartTime { get; set; }
            public DateTime LastStopTime { get; set; }
            public string ServiceRunStatus { get; set; }
            public DateTime ServiceStatusCheckTime { get; set; }
            public DateTime LastCheckInTime { get; set; }
            public bool ServiceError { get; set; }
        }


        #endregion
        #region ===== ServiceConfig SQLite : Return =====

        public ServiceConfig Return_Object_From_ServiceConfig(string DbFile, string WhereClause = "[ID] = ''")
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            string SelectLine = "Select [ID], [ServiceName], [ServicePath], [ServiceEnabled], [ServiceCheckInMin], [ServiceEventTimerMin], [SettingDateModified], [LastStartTime], [LastStopTime], [ServiceRunStatus], [ServiceStatusCheckTime], [LastCheckInTime], [ServiceError] From [ServiceConfig] ";
            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
            if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
            if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
            ServiceConfig returnObject = new ServiceConfig();
            int i = 0;
            string Value = "";
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    returnObject.ID = ret["ID"].ToString();
                    returnObject.ServiceName = ret["ServiceName"].ToString();
                    returnObject.ServicePath = ret["ServicePath"].ToString();
                    returnObject.ServiceEnabled = ret["ServiceEnabled"].ToBool();
                    returnObject.ServiceCheckInMin = ret["ServiceCheckInMin"].ToInt();
                    returnObject.ServiceEventTimerMin = ret["ServiceEventTimerMin"].ToInt();
                    returnObject.SettingDateModified = ret["SettingDateModified"].ToDateTime();
                    returnObject.LastStartTime = ret["LastStartTime"].ToDateTime();
                    returnObject.LastStopTime = ret["LastStopTime"].ToDateTime();
                    returnObject.ServiceRunStatus = ret["ServiceRunStatus"].ToString();
                    returnObject.ServiceStatusCheckTime = ret["ServiceStatusCheckTime"].ToDateTime();
                    returnObject.LastCheckInTime = ret["LastCheckInTime"].ToDateTime();
                    returnObject.ServiceError = ret["ServiceError"].ToBool();
                }
            }

            return returnObject;
        }

        public List<ServiceConfig> Return_ServiceConfig_List(string DbFile, string TableName = "[ServiceConfig]", string WhereClause = "")
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            string SelectLine = "Select * From " + TableName;

            if (WhereClause != "")
            {
                if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
            }
            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

            List<ServiceConfig> ReturnList = new List<ServiceConfig>();
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    ServiceConfig returnObject = new ServiceConfig();

                    returnObject.ID = ret["ID"].ToString();
                    returnObject.ServiceName = ret["ServiceName"].ToString();
                    returnObject.ServicePath = ret["ServicePath"].ToString();
                    returnObject.ServiceEnabled = ret["ServiceEnabled"].ToBool();
                    returnObject.ServiceCheckInMin = ret["ServiceCheckInMin"].ToInt();
                    returnObject.ServiceEventTimerMin = ret["ServiceEventTimerMin"].ToInt();
                    returnObject.SettingDateModified = ret["SettingDateModified"].ToDateTime();
                    returnObject.LastStartTime = ret["LastStartTime"].ToDateTime();
                    returnObject.LastStopTime = ret["LastStopTime"].ToDateTime();
                    returnObject.ServiceRunStatus = ret["ServiceRunStatus"].ToString();
                    returnObject.ServiceStatusCheckTime = ret["ServiceStatusCheckTime"].ToDateTime();
                    returnObject.LastCheckInTime = ret["LastCheckInTime"].ToDateTime();
                    returnObject.ServiceError = ret["ServiceError"].ToBool();

                    ReturnList.Add(returnObject);
                }
            }

            return ReturnList;
        }

        public DataTable Return_DataTable_From_ServiceConfig(string DbFile)
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            string SelectLine = "Select [ID], [ServiceName], [ServicePath], [ServiceEnabled], [ServiceCheckInMin], [ServiceEventTimerMin], [SettingDateModified], [LastStartTime], [LastStopTime], [ServiceRunStatus], [ServiceStatusCheckTime], [LastCheckInTime], [ServiceError] From [ServiceConfig]";
            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
            return ReturnTable;
        }


        #endregion
        #region ===== ServiceConfig SQLite : Update Insert =====

        public bool ServiceConfig_Insert(string DbFile, ServiceConfig inObject)
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            string InsertLine = "Insert Into [ServiceConfig] (ID, ServiceName, ServicePath, ServiceEnabled, ServiceCheckInMin, ServiceEventTimerMin, SettingDateModified, LastStartTime, LastStopTime, ServiceRunStatus, ServiceStatusCheckTime, LastCheckInTime, ServiceError) values ('" + inObject.ID + "', '" + inObject.ServiceName + "', '" + inObject.ServicePath + "', '" + inObject.ServiceEnabled + "', '" + inObject.ServiceCheckInMin + "', '" + inObject.ServiceEventTimerMin + "', '" + inObject.SettingDateModified + "', '" + inObject.LastStartTime + "', '" + inObject.LastStopTime + "', '" + inObject.ServiceRunStatus + "', '" + inObject.ServiceStatusCheckTime + "', '" + inObject.LastCheckInTime + "', '" + inObject.ServiceError + "')";
            bool Inserted = sqlite.Execute(DbFile, InsertLine);
            if (!Inserted)
            {
                _AHK ahk = new _AHK();
                ahk.MsgBox("Inserted Into [ServiceConfig] = " + Inserted.ToString());
            }
            return Inserted;
        }

        public bool ServiceConfig_Update(string DbFile, ServiceConfig inObject, string WHERE = "[Item] = 'Value'")
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            //string UpdateLine = "Update [ServiceConfig] set ID = '" + inObject.ID + "', ServiceName = '" + inObject.ServiceName + "', ServicePath = '" + inObject.ServicePath + "', ServiceEnabled = '" + inObject.ServiceEnabled + "', ServiceCheckInMin = '" + inObject.ServiceCheckInMin + "', ServiceEventTimerMin = '" + inObject.ServiceEventTimerMin + "', SettingDateModified = '" + inObject.SettingDateModified + "', LastStartTime = '" + inObject.LastStartTime + "', LastStopTime = '" + inObject.LastStopTime + "', ServiceRunStatus = '" + inObject.ServiceRunStatus + "', ServiceStatusCheckTime = '" + inObject.ServiceStatusCheckTime + "', LastCheckInTime = '" + inObject.LastCheckInTime + "', ServiceError = '" + inObject.ServiceError + "' WHERE [Item] = 'Value' ";
            string UpdateLine = "Update [ServiceConfig] set ";


            if (inObject.ServiceName != null) { UpdateLine = UpdateLine + "[ServiceName] = '" + inObject.ServiceName + "',"; }
            if (inObject.ServicePath != null) { UpdateLine = UpdateLine + "[ServicePath] = '" + inObject.ServicePath + "',"; }
            if (inObject.SettingDateModified != null) { UpdateLine = UpdateLine + "[SettingDateModified] = '" + inObject.SettingDateModified + "',"; }
            if (inObject.LastStartTime != null) { UpdateLine = UpdateLine + "[LastStartTime] = '" + inObject.LastStartTime + "',"; }
            if (inObject.LastStopTime != null) { UpdateLine = UpdateLine + "[LastStopTime] = '" + inObject.LastStopTime + "',"; }
            if (inObject.ServiceRunStatus != null) { UpdateLine = UpdateLine + "[ServiceRunStatus] = '" + inObject.ServiceRunStatus + "',"; }
            if (inObject.ServiceStatusCheckTime != null) { UpdateLine = UpdateLine + "[ServiceStatusCheckTime] = '" + inObject.ServiceStatusCheckTime + "',"; }
            if (inObject.LastCheckInTime != null) { UpdateLine = UpdateLine + "[LastCheckInTime] = '" + inObject.LastCheckInTime + "',"; }

            _AHK ahk = new _AHK();
            UpdateLine = ahk.TrimLast(UpdateLine, 1);
            UpdateLine = UpdateLine + " WHERE " + WHERE;

            bool Updated = sqlite.Execute(DbFile, UpdateLine);
            return Updated;
        }

        public bool ServiceConfig_sqliteUpdateInsert(string DbFile, ServiceConfig obj, string WhereClause = "")
        {
            bool Updated = ServiceConfig_Update(DbFile, obj, WhereClause);  // try to update record first
            if (!Updated) { Updated = ServiceConfig_Insert(DbFile, obj); }  // if unable to update, insert new record
            return Updated;
        }


        #endregion
        #region ===== ServiceConfig DataTable =====

        public DataTable Return_ServiceConfig_DataTable(string DbFile, string TableName = "ServiceConfig", string WhereClause = "", bool Debug = false)
        {
            _Database.SQLite sqlite = new _Database.SQLite();

            string SelectLine = "Select * From [ServiceConfig]";

            if (WhereClause != "")
            {
                if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
            }

            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


            DataTable table = new DataTable();
            table.Columns.Add("ID", typeof(string));
            table.Columns.Add("ServiceName", typeof(string));
            table.Columns.Add("ServicePath", typeof(string));
            table.Columns.Add("ServiceEnabled", typeof(string));
            table.Columns.Add("ServiceCheckInMin", typeof(string));
            table.Columns.Add("ServiceEventTimerMin", typeof(string));
            table.Columns.Add("SettingDateModified", typeof(string));
            table.Columns.Add("LastStartTime", typeof(string));
            table.Columns.Add("LastStopTime", typeof(string));
            table.Columns.Add("ServiceRunStatus", typeof(string));
            table.Columns.Add("ServiceStatusCheckTime", typeof(string));
            table.Columns.Add("LastCheckInTime", typeof(string));
            table.Columns.Add("ServiceError", typeof(string));

            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    ServiceConfig returnObject = new ServiceConfig();

                    returnObject.ID = ret["ID"].ToString();
                    returnObject.ServiceName = ret["ServiceName"].ToString();
                    returnObject.ServicePath = ret["ServicePath"].ToString();
                    returnObject.ServiceEnabled = ret["ServiceEnabled"].ToBool();
                    returnObject.ServiceCheckInMin = ret["ServiceCheckInMin"].ToInt();
                    returnObject.ServiceEventTimerMin = ret["ServiceEventTimerMin"].ToInt();
                    returnObject.SettingDateModified = ret["SettingDateModified"].ToDateTime();
                    returnObject.LastStartTime = ret["LastStartTime"].ToDateTime();
                    returnObject.LastStopTime = ret["LastStopTime"].ToDateTime();
                    returnObject.ServiceRunStatus = ret["ServiceRunStatus"].ToString();
                    returnObject.ServiceStatusCheckTime = ret["ServiceStatusCheckTime"].ToDateTime();
                    returnObject.LastCheckInTime = ret["LastCheckInTime"].ToDateTime();
                    returnObject.ServiceError = ret["ServiceError"].ToBool();


                    table.Rows.Add(returnObject.ID, returnObject.ServiceName, returnObject.ServicePath, returnObject.ServiceEnabled, returnObject.ServiceCheckInMin, returnObject.ServiceEventTimerMin, returnObject.SettingDateModified, returnObject.LastStartTime, returnObject.LastStopTime, returnObject.ServiceRunStatus, returnObject.ServiceStatusCheckTime, returnObject.LastCheckInTime, returnObject.ServiceError);
                }
            }

            return table;
        }

        public DataTable Create_ServiceConfig_DataTable(ServiceConfig inObject)
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", typeof(string));
            table.Columns.Add("ServiceName", typeof(string));
            table.Columns.Add("ServicePath", typeof(string));
            table.Columns.Add("ServiceEnabled", typeof(string));
            table.Columns.Add("ServiceCheckInMin", typeof(string));
            table.Columns.Add("ServiceEventTimerMin", typeof(string));
            table.Columns.Add("SettingDateModified", typeof(string));
            table.Columns.Add("LastStartTime", typeof(string));
            table.Columns.Add("LastStopTime", typeof(string));
            table.Columns.Add("ServiceRunStatus", typeof(string));
            table.Columns.Add("ServiceStatusCheckTime", typeof(string));
            table.Columns.Add("LastCheckInTime", typeof(string));
            table.Columns.Add("ServiceError", typeof(string));

            table.Rows.Add(inObject.ID, inObject.ServiceName, inObject.ServicePath, inObject.ServiceEnabled, inObject.ServiceCheckInMin, inObject.ServiceEventTimerMin, inObject.SettingDateModified, inObject.LastStartTime, inObject.LastStopTime, inObject.ServiceRunStatus, inObject.ServiceStatusCheckTime, inObject.LastCheckInTime, inObject.ServiceError);
            return table;
        }


        #endregion
        #region ===== ServiceConfig SQL Functions =====


        /// <summary>
        /// Write Initial Service Config Entry + SQL Job Entries
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <param name="connName"></param>
        public bool Write_ServiceSQLJobs(string ServiceExePath, string connName = "ServiceDb", int ServiceEventTimer = 30, int ServiceCheckIn = 60)
        {
            string ServiceTableName = ServiceLog_TableName();

            string ServiceName = ServiceNameFromEXE(ServiceExePath);

            string sqlLine = "Select * FROM " + ServiceTableName + " where [ServiceName] = '" + ServiceName.Trim() + "' AND [LogType] = 'LogEntry' order by [ID] desc";
            string JobName = "Event Log"; 
            bool Inserted = SaveSQLCommand(ServiceName.Trim(), connName, JobName, sqlLine);

            sqlLine = "select * FROM " + ServiceTableName + " where [ServiceName] = '" + ServiceName.Trim() + "' AND [LogType] = 'StartStop' order by [ID] desc";
            JobName = "Start Stop Log";
            Inserted = SaveSQLCommand(ServiceName.Trim(), connName, JobName, sqlLine);

            sqlLine = "select * FROM " + ServiceTableName + " where [ServiceName] = " + ServiceName.Trim() + "' AND [LogType] = 'LogEntry' and [LogMessage] like '%Finished%' order by [ID] desc";
            JobName = "Finished Log";
            Inserted = SaveSQLCommand(ServiceName.Trim(), connName, JobName, sqlLine);



            ServiceConfig job = new ServiceConfig();
            job.ServiceName = ServiceName;
            job.ServiceEnabled = true;
            job.ServiceError = false;
            job.ServiceCheckInMin = ServiceCheckIn.ToInt();
            job.ServiceEventTimerMin = ServiceEventTimer.ToInt();
            job.ServicePath = ServiceExePath;
            job.SettingDateModified = DateTime.Now;
            bool updated = ServiceConfig_UpdateInsert(job);
            return updated;
        }


        /// <summary>
        /// Save/Update SQL Job Commands by ServiceName 
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <param name="ConnName"></param>
        /// <param name="JobName"></param>
        /// <param name="SQLCmd"></param>
        /// <returns></returns>
        public bool SaveSQLCommand(string ServiceName, string ConnName, string JobName, string SQLCmd)
        {
            ServiceTables table = new ServiceTables();
            table.ServiceName = ServiceName;
            table.Enabled = true;
            table.TableConnName = ConnName;
            table.TableSQL = SQLCmd;
            table.TableDisplayName = JobName;
            return ServiceTables_UpdateInsert(table);
        }



        // Return ServiceConfig SQL Connection String
        public SqlConnection ServiceConfig_Conn()
        {
            // populate sql connection
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ServiceDb"].ConnectionString);
            if (conn == null)
            {
                _AHK ahk = new _AHK();
                //ahk.MsgBox("ServiceLog_Conn() Returned Null for 'ServiceDb' AppConfig Entry");
                ahk.FileAppend("ServiceDb Returned Null", ahk.DesktopDir() + "\\CRM_TFS_Service_ErrorLog.txt");
            }
            return conn;
        }

        // Return ServiceConfig TableName (Full Path)
        public string ServiceConfig_TableName()
        {
            string TableNameBase = ConfigurationManager.ConnectionStrings["ServiceTableRoot"].ConnectionString;

            // populate to return full sql table name
            return TableNameBase + ".[ServiceConfig]";
        }

        // Generate SQL Table
        public bool ServiceConfig_CreateSQLTable(string NewTablePath = "")
        {
            if (NewTablePath == "") { NewTablePath = ServiceConfig_TableName(); }

            _Database.SQL sql = new _Database.SQL();
            SqlConnection Conn = ServiceConfig_Conn();
            string CreateTableLine = "CREATE TABLE " + NewTablePath + "(";
            CreateTableLine = CreateTableLine + "[ID] [bigint] IDENTITY(1,1) NOT NULL,";
            CreateTableLine = CreateTableLine + "[ServiceName] [varchar](max) NOT NULL,";
            CreateTableLine = CreateTableLine + "[ServicePath] [varchar](max) NOT NULL,";
            CreateTableLine = CreateTableLine + "[ServiceEnabled] [bit] NULL,";
            CreateTableLine = CreateTableLine + "[ServiceCheckInMin] [int] NOT NULL,";
            CreateTableLine = CreateTableLine + "[ServiceEventTimerMin] [int] NOT NULL,";
            CreateTableLine = CreateTableLine + "[SettingDateModified] [datetime] NULL,";
            CreateTableLine = CreateTableLine + "[LastStartTime] [datetime] NULL,";
            CreateTableLine = CreateTableLine + "[LastStopTime] [datetime] NULL,";
            CreateTableLine = CreateTableLine + "[ServiceRunStatus] [varchar](max) NULL,";
            CreateTableLine = CreateTableLine + "[ServiceStatusCheckTime] [datetime] NULL,";
            CreateTableLine = CreateTableLine + "[LastCheckInTime] [datetime] NULL,";
            CreateTableLine = CreateTableLine + "[ServiceError] [bit] NULL";
            CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";

            return sql.WriteDataRecord(Conn, CreateTableLine);
        }

        public bool ServiceConfig_InsertSQL(ServiceConfig obj)
        {
            _AHK ahk = new _AHK();
            SqlConnection Con = ServiceConfig_Conn();
            string SQLLine = "Insert Into " + ServiceConfig_TableName() + " (ServiceName, ServicePath, ServiceEnabled, ServiceCheckInMin, ServiceEventTimerMin, SettingDateModified, LastStartTime, LastStopTime, ServiceRunStatus, ServiceStatusCheckTime, LastCheckInTime, ServiceError) VALUES (@ServiceName, @ServicePath, @ServiceEnabled, @ServiceCheckInMin, @ServiceEventTimerMin, @SettingDateModified, @LastStartTime, @LastStopTime, @ServiceRunStatus, @ServiceStatusCheckTime, @LastCheckInTime, @ServiceError)";
            SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
            cmd2 = new SqlCommand(SQLLine, Con);

            if (obj.ServicePath == null) { obj.ServicePath = ""; }
            if (obj.ServiceRunStatus == null) { obj.ServiceRunStatus = ""; }
            obj.SettingDateModified = DateTime.Now;
            if (obj.LastStartTime.ToString() == "1/1/0001 12:00:00 AM") { obj.LastStartTime = new DateTime(1900, 1, 1); }
            if (obj.LastStopTime.ToString() == "1/1/0001 12:00:00 AM") { obj.LastStopTime = new DateTime(1900, 1, 1); }
            if (obj.ServiceStatusCheckTime.ToString() == "1/1/0001 12:00:00 AM") { obj.ServiceStatusCheckTime = new DateTime(1900, 1, 1); }
            if (obj.LastCheckInTime.ToString() == "1/1/0001 12:00:00 AM") { obj.LastCheckInTime = new DateTime(1900, 1, 1); }

            cmd2.Parameters.AddWithValue(@"ServiceName", obj.ServiceName.ToString());
            cmd2.Parameters.AddWithValue(@"ServicePath", obj.ServicePath.ToString());
            cmd2.Parameters.AddWithValue(@"ServiceEnabled", obj.ServiceEnabled.ToString());
            cmd2.Parameters.AddWithValue(@"ServiceCheckInMin", obj.ServiceCheckInMin.ToString());
            cmd2.Parameters.AddWithValue(@"ServiceEventTimerMin", obj.ServiceEventTimerMin.ToString());
            cmd2.Parameters.AddWithValue(@"SettingDateModified", obj.SettingDateModified.ToString());
            cmd2.Parameters.AddWithValue(@"LastStartTime", obj.LastStartTime.ToString());
            cmd2.Parameters.AddWithValue(@"LastStopTime", obj.LastStopTime.ToString());
            cmd2.Parameters.AddWithValue(@"ServiceRunStatus", obj.ServiceRunStatus.ToString());
            cmd2.Parameters.AddWithValue(@"ServiceStatusCheckTime", obj.ServiceStatusCheckTime.ToString());
            cmd2.Parameters.AddWithValue(@"LastCheckInTime", obj.LastCheckInTime.ToString());
            cmd2.Parameters.AddWithValue(@"ServiceError", obj.ServiceError.ToString());

            if (Con.State == ConnectionState.Closed) { Con.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex)
            {
                if (ex.Number == 1205) // Retry on DeadLock
                {
                    ahk.Sleep(1000);
                    ServiceConfig_InsertSQL(obj);
                }
                else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                {
                    ahk.Sleep(1000);
                    ServiceConfig_InsertSQL(obj);
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

        public bool ServiceConfig_DeleteService(ServiceConfig obj)
        {
            _AHK ahk = new _AHK();
            SqlConnection Conn = ServiceConfig_Conn();
            string SQLLine = "Delete From " + ServiceConfig_TableName() + " WHERE ServiceName = @ServiceName";
            SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
            cmd2 = new SqlCommand(SQLLine, Conn);

            cmd2.Parameters.AddWithValue(@"ServiceName", obj.ServiceName.ToString());

            if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex)
            {
                if (ex.Number == 1205) // Retry on DeadLock
                {
                    ahk.Sleep(1000);
                    ServiceConfig_DeleteService(obj);
                }
                else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                {
                    ahk.Sleep(1000);
                    ServiceConfig_DeleteService(obj);
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

        public bool ServiceConfig_UpdateSQL(ServiceConfig obj)
        {
            SqlConnection Conn = ServiceConfig_Conn();
            string SQLLine = "Update " + ServiceConfig_TableName() + " SET ServicePath = @ServicePath, ServiceEnabled = @ServiceEnabled, ServiceCheckInMin = @ServiceCheckInMin, ServiceEventTimerMin = @ServiceEventTimerMin, SettingDateModified = @SettingDateModified, LastStartTime = @LastStartTime, LastStopTime = @LastStopTime, ServiceRunStatus = @ServiceRunStatus, ServiceStatusCheckTime = @ServiceStatusCheckTime, LastCheckInTime = @LastCheckInTime, ServiceError = @ServiceError WHERE ServiceName = @ServiceName";
            SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
            cmd2 = new SqlCommand(SQLLine, Conn);


            if (obj.ServicePath == null) { obj.ServicePath = ""; }
            if (obj.ServiceRunStatus == null) { obj.ServiceRunStatus = ""; }
            obj.SettingDateModified = DateTime.Now;
            if (obj.LastStartTime.ToString() == "1/1/0001 12:00:00 AM") { obj.LastStartTime = new DateTime(1900, 1, 1); }
            if (obj.LastStopTime.ToString() == "1/1/0001 12:00:00 AM") { obj.LastStopTime = new DateTime(1900, 1, 1); }
            if (obj.ServiceStatusCheckTime.ToString() == "1/1/0001 12:00:00 AM") { obj.ServiceStatusCheckTime = new DateTime(1900, 1, 1); }
            if (obj.LastCheckInTime.ToString() == "1/1/0001 12:00:00 AM") { obj.LastCheckInTime = new DateTime(1900, 1, 1); }

            cmd2.Parameters.AddWithValue(@"ServiceName", obj.ServiceName.ToString());
            cmd2.Parameters.AddWithValue(@"ServicePath", obj.ServicePath.ToString());
            cmd2.Parameters.AddWithValue(@"ServiceEnabled", obj.ServiceEnabled.ToString());
            cmd2.Parameters.AddWithValue(@"ServiceCheckInMin", obj.ServiceCheckInMin.ToString());
            cmd2.Parameters.AddWithValue(@"ServiceEventTimerMin", obj.ServiceEventTimerMin.ToString());
            cmd2.Parameters.AddWithValue(@"SettingDateModified", obj.SettingDateModified.ToString());
            cmd2.Parameters.AddWithValue(@"LastStartTime", obj.LastStartTime.ToString());
            cmd2.Parameters.AddWithValue(@"LastStopTime", obj.LastStopTime.ToString());
            cmd2.Parameters.AddWithValue(@"ServiceRunStatus", obj.ServiceRunStatus.ToString());
            cmd2.Parameters.AddWithValue(@"ServiceStatusCheckTime", obj.ServiceStatusCheckTime.ToString());
            cmd2.Parameters.AddWithValue(@"LastCheckInTime", obj.LastCheckInTime.ToString());
            cmd2.Parameters.AddWithValue(@"ServiceError", obj.ServiceError.ToString());

            if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex)
            {
                _AHK ahk = new _AHK();
                ahk.MsgBox(ex.ToString());
                return false;
            }
            Conn.Close();
            if (recordsAffected > 0) { return true; }
            else return false;
        }

        public bool ServiceConfig_UpdateServiceStatus(ServiceConfig obj)
        {
            _AHK ahk = new _AHK();
            SqlConnection Conn = ServiceConfig_Conn();
            string SQLLine = "Update " + ServiceConfig_TableName() + " SET ServiceRunStatus = @ServiceRunStatus, ServiceStatusCheckTime = @ServiceStatusCheckTime, ServiceError = @ServiceError WHERE ServiceName = @ServiceName";
            SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
            cmd2 = new SqlCommand(SQLLine, Conn);


            if (obj.ServicePath == null) { obj.ServicePath = ""; }
            if (obj.ServiceRunStatus == null) { obj.ServiceRunStatus = ""; }
            obj.SettingDateModified = DateTime.Now;
            if (obj.LastStartTime.ToString() == "1/1/0001 12:00:00 AM") { obj.LastStartTime = new DateTime(1900, 1, 1); }
            if (obj.LastStopTime.ToString() == "1/1/0001 12:00:00 AM") { obj.LastStopTime = new DateTime(1900, 1, 1); }
            if (obj.ServiceStatusCheckTime.ToString() == "1/1/0001 12:00:00 AM") { obj.ServiceStatusCheckTime = new DateTime(1900, 1, 1); }
            if (obj.LastCheckInTime.ToString() == "1/1/0001 12:00:00 AM") { obj.LastCheckInTime = new DateTime(1900, 1, 1); }

            cmd2.Parameters.AddWithValue(@"ServiceName", obj.ServiceName.ToString());
            cmd2.Parameters.AddWithValue(@"ServiceRunStatus", obj.ServiceRunStatus.ToString());
            cmd2.Parameters.AddWithValue(@"ServiceStatusCheckTime", obj.ServiceStatusCheckTime.ToString());
            cmd2.Parameters.AddWithValue(@"ServiceError", obj.ServiceError.ToString());

            if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex)
            {
                if (ex.Number == 1205) // Retry on DeadLock
                {
                    ahk.Sleep(1000);
                    ServiceConfig_UpdateServiceStatus(obj);
                }
                else if (ex.Message.ToUpper().Contains("TIMEOUT EXPIRED")) // Retry on Standard TimeOut
                {
                    ahk.Sleep(1000);
                    ServiceConfig_UpdateServiceStatus(obj);
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

        public bool ServiceConfig_UpdateInsert(ServiceConfig obj)
        {
            SqlConnection Conn = ServiceConfig_Conn();
            bool Updated = ServiceConfig_UpdateSQL(obj);  // try to update record first
            if (!Updated) { Updated = ServiceConfig_InsertSQL(obj); }  // if unable to update, insert new record
            return Updated;
        }

        // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
        public bool ServiceConfig_UpdateIfPopulated(ServiceConfig obj, string ID = "")
        {
            SqlConnection Conn = ServiceConfig_Conn();
            string SQLcmd = "Update ServiceConfig SET ";
            if (obj.ID != null) { SQLcmd = SQLcmd + " ID = @ID,"; }
            if (obj.ServiceName != null) { SQLcmd = SQLcmd + " ServiceName = @ServiceName,"; }
            if (obj.ServicePath != null) { SQLcmd = SQLcmd + " ServicePath = @ServicePath,"; }
            if (obj.ServiceEnabled != null) { SQLcmd = SQLcmd + " ServiceEnabled = @ServiceEnabled,"; }
            if (obj.ServiceCheckInMin != null) { SQLcmd = SQLcmd + " ServiceCheckInMin = @ServiceCheckInMin,"; }
            if (obj.ServiceEventTimerMin != null) { SQLcmd = SQLcmd + " ServiceEventTimerMin = @ServiceEventTimerMin,"; }
            if (obj.SettingDateModified != null) { SQLcmd = SQLcmd + " SettingDateModified = @SettingDateModified,"; }
            if (obj.LastStartTime != null) { SQLcmd = SQLcmd + " LastStartTime = @LastStartTime,"; }
            if (obj.LastStopTime != null) { SQLcmd = SQLcmd + " LastStopTime = @LastStopTime,"; }
            if (obj.ServiceRunStatus != null) { SQLcmd = SQLcmd + " ServiceRunStatus = @ServiceRunStatus,"; }
            if (obj.ServiceStatusCheckTime != null) { SQLcmd = SQLcmd + " ServiceStatusCheckTime = @ServiceStatusCheckTime,"; }
            if (obj.LastCheckInTime != null) { SQLcmd = SQLcmd + " LastCheckInTime = @LastCheckInTime,"; }
            if (obj.ServiceError != null) { SQLcmd = SQLcmd + " ServiceError = @ServiceError,"; }

            _AHK ahk = new _AHK();
            SQLcmd = ahk.TrimLast(SQLcmd, 1);
            SQLcmd = SQLcmd + " WHERE ID = @ID";

            SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

            if (obj.ID != null) { cmd2.Parameters.AddWithValue(@"ID", obj.ID); }
            if (obj.ServiceName != null) { cmd2.Parameters.AddWithValue(@"ServiceName", obj.ServiceName); }
            if (obj.ServicePath != null) { cmd2.Parameters.AddWithValue(@"ServicePath", obj.ServicePath); }
            if (obj.ServiceEnabled != null) { cmd2.Parameters.AddWithValue(@"ServiceEnabled", obj.ServiceEnabled); }
            if (obj.ServiceCheckInMin != null) { cmd2.Parameters.AddWithValue(@"ServiceCheckInMin", obj.ServiceCheckInMin); }
            if (obj.ServiceEventTimerMin != null) { cmd2.Parameters.AddWithValue(@"ServiceEventTimerMin", obj.ServiceEventTimerMin); }
            if (obj.SettingDateModified != null) { cmd2.Parameters.AddWithValue(@"SettingDateModified", obj.SettingDateModified); }
            if (obj.LastStartTime != null) { cmd2.Parameters.AddWithValue(@"LastStartTime", obj.LastStartTime); }
            if (obj.LastStopTime != null) { cmd2.Parameters.AddWithValue(@"LastStopTime", obj.LastStopTime); }
            if (obj.ServiceRunStatus != null) { cmd2.Parameters.AddWithValue(@"ServiceRunStatus", obj.ServiceRunStatus); }
            if (obj.ServiceStatusCheckTime != null) { cmd2.Parameters.AddWithValue(@"ServiceStatusCheckTime", obj.ServiceStatusCheckTime); }
            if (obj.LastCheckInTime != null) { cmd2.Parameters.AddWithValue(@"LastCheckInTime", obj.LastCheckInTime); }
            if (obj.ServiceError != null) { cmd2.Parameters.AddWithValue(@"ServiceError", obj.ServiceError); }

            if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex)
            {
                ahk.MsgBox(ex.ToString());
                return false;
            }
            Conn.Close();
            if (recordsAffected > 0) { return true; }
            else return false;
        }

        public ServiceConfig ServiceConfigJob(string ServiceName)
        {
            _Database.SQL sql = new _Database.SQL();
            SqlConnection Conn = ServiceConfig_Conn();
            string SelectLine = "Select [ID],[ServiceName],[ServicePath],[ServiceEnabled],[ServiceCheckInMin],[ServiceEventTimerMin],[SettingDateModified],[LastStartTime],[LastStopTime],[ServiceRunStatus],[ServiceStatusCheckTime],[LastCheckInTime],[ServiceError] From " + ServiceConfig_TableName() + " WHERE ServiceName = '" + ServiceName + "'";
            DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
            ServiceConfig returnObject = new ServiceConfig();
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    returnObject.ID = ret["ID"].ToString();
                    returnObject.ServiceName = ret["ServiceName"].ToString();
                    returnObject.ServicePath = ret["ServicePath"].ToString();
                    returnObject.ServiceEnabled = ret["ServiceEnabled"].ToBool();
                    returnObject.ServiceCheckInMin = ret["ServiceCheckInMin"].ToInt();
                    returnObject.ServiceEventTimerMin = ret["ServiceEventTimerMin"].ToInt();
                    returnObject.SettingDateModified = ret["SettingDateModified"].ToDateTime();
                    returnObject.LastStartTime = ret["LastStartTime"].ToDateTime();
                    returnObject.LastStopTime = ret["LastStopTime"].ToDateTime();
                    returnObject.ServiceRunStatus = ret["ServiceRunStatus"].ToString();
                    returnObject.ServiceStatusCheckTime = ret["ServiceStatusCheckTime"].ToDateTime();
                    returnObject.LastCheckInTime = ret["LastCheckInTime"].ToDateTime();
                    returnObject.ServiceError = ret["ServiceError"].ToBool();

                    if (returnObject.ID == null) { returnObject.ID = "0"; }
                    if (returnObject.ServiceName == null) { returnObject.ServiceName = ""; }
                    if (returnObject.ServicePath == null) { returnObject.ServicePath = ""; }
                    if (returnObject.ServiceRunStatus == null) { returnObject.ServiceRunStatus = ""; }
                    if (returnObject.SettingDateModified.ToString() == "1/1/0001 12:00:00 AM") { returnObject.SettingDateModified = new DateTime(1900, 1, 1); }
                    if (returnObject.LastStartTime.ToString() == "1/1/0001 12:00:00 AM") { returnObject.LastStartTime = new DateTime(1900, 1, 1); }
                    if (returnObject.LastStopTime.ToString() == "1/1/0001 12:00:00 AM") { returnObject.LastStopTime = new DateTime(1900, 1, 1); }
                    if (returnObject.ServiceStatusCheckTime.ToString() == "1/1/0001 12:00:00 AM") { returnObject.ServiceStatusCheckTime = new DateTime(1900, 1, 1); }
                    if (returnObject.LastCheckInTime.ToString() == "1/1/0001 12:00:00 AM") { returnObject.LastCheckInTime = new DateTime(1900, 1, 1); }

                    if (returnObject.ServiceCheckInMin == 0) { returnObject.ServiceCheckInMin = 10; }
                    if (returnObject.ServiceEventTimerMin == 0) { returnObject.ServiceEventTimerMin = 10; }

                    return returnObject;
                }
            }

            if (returnObject.ID == null) { returnObject.ID = "0"; }
            if (returnObject.ServiceName == null) { returnObject.ServiceName = ""; }
            if (returnObject.ServicePath == null) { returnObject.ServicePath = ""; }
            if (returnObject.ServiceRunStatus == null) { returnObject.ServiceRunStatus = ""; }
            if (returnObject.SettingDateModified.ToString() == "1/1/0001 12:00:00 AM") { returnObject.SettingDateModified = new DateTime(1900, 1, 1); }
            if (returnObject.LastStartTime.ToString() == "1/1/0001 12:00:00 AM") { returnObject.LastStartTime = new DateTime(1900, 1, 1); }
            if (returnObject.LastStopTime.ToString() == "1/1/0001 12:00:00 AM") { returnObject.LastStopTime = new DateTime(1900, 1, 1); }
            if (returnObject.ServiceStatusCheckTime.ToString() == "1/1/0001 12:00:00 AM") { returnObject.ServiceStatusCheckTime = new DateTime(1900, 1, 1); }
            if (returnObject.LastCheckInTime.ToString() == "1/1/0001 12:00:00 AM") { returnObject.LastCheckInTime = new DateTime(1900, 1, 1); }


            return returnObject;
        }

        public List<ServiceConfig> ServiceConfigJobs()
        {
            string Command = "select * FROM " + ServiceConfig_TableName();

            _Database.SQL sql = new _Database.SQL();
            SqlConnection Conn = ServiceConfig_Conn();
            DataTable ReturnTable = sql.GetDataTable(Conn, Command);
            List<ServiceConfig> ReturnList = new List<ServiceConfig>();
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    ServiceConfig returnObject = new ServiceConfig();

                    returnObject.ID = ret["ID"].ToString();
                    returnObject.ServiceName = ret["ServiceName"].ToString();
                    returnObject.ServicePath = ret["ServicePath"].ToString();
                    returnObject.ServiceEnabled = ret["ServiceEnabled"].ToBool();
                    returnObject.ServiceCheckInMin = ret["ServiceCheckInMin"].ToInt();
                    returnObject.ServiceEventTimerMin = ret["ServiceEventTimerMin"].ToInt();
                    returnObject.SettingDateModified = ret["SettingDateModified"].ToDateTime();
                    returnObject.LastStartTime = ret["LastStartTime"].ToDateTime();
                    returnObject.LastStopTime = ret["LastStopTime"].ToDateTime();
                    returnObject.ServiceRunStatus = ret["ServiceRunStatus"].ToString();
                    returnObject.ServiceStatusCheckTime = ret["ServiceStatusCheckTime"].ToDateTime();
                    returnObject.LastCheckInTime = ret["LastCheckInTime"].ToDateTime();
                    returnObject.ServiceError = ret["ServiceError"].ToBool();

                    ReturnList.Add(returnObject);
                }
            }
            return ReturnList;
        }


        /// <summary>
        /// From Service, Checks In With Database To Ensure Service Is Still Running
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool ServiceConfig_CheckIn(ServiceConfig obj)
        {
            SqlConnection Conn = ServiceConfig_Conn();
            string SQLLine = "Update " + ServiceConfig_TableName() + " SET LastCheckInTime = @LastCheckInTime WHERE ServiceName = @ServiceName";
            SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
            cmd2 = new SqlCommand(SQLLine, Conn);

            cmd2.Parameters.AddWithValue(@"ServiceName", obj.ServiceName.ToString());
            cmd2.Parameters.AddWithValue(@"LastCheckInTime", DateTime.Now.ToString());

            if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex)
            {
                //_AHK ahk = new _AHK();
                //ahk.MsgBox(ex.ToString());
                return false;
            }
            Conn.Close();
            if (recordsAffected > 0) { return true; }
            else return false;
        }

        /// <summary>
        /// Logs Service Start Time to Service Db (updates using ServiceName)
        /// </summary>
        /// <param name="obj">ServiceConfig Object for Service to Update</param>
        /// <returns>Returns True with Successful Db Write</returns>
        public bool ServiceConfig_Start(ServiceConfig obj)
        {
            SqlConnection Conn = ServiceConfig_Conn();
            string SQLLine = "Update " + ServiceConfig_TableName() + " SET LastStartTime = @LastStartTime, ServiceRunStatus = @ServiceRunStatus, LastCheckInTime = @LastCheckInTime WHERE ServiceName = @ServiceName";
            SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
            cmd2 = new SqlCommand(SQLLine, Conn);

            cmd2.Parameters.AddWithValue(@"ServiceName", obj.ServiceName.ToString());
            cmd2.Parameters.AddWithValue(@"LastStartTime", DateTime.Now.ToString());
            cmd2.Parameters.AddWithValue(@"ServiceRunStatus", "Running");
            cmd2.Parameters.AddWithValue(@"LastCheckInTime", DateTime.Now.ToString());

            if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex)
            {
                //_AHK ahk = new _AHK();
                //ahk.MsgBox(ex.ToString());
                return false;
            }
            Conn.Close();
            if (recordsAffected > 0) { return true; }
            else return false;
        }

        /// <summary>
        /// Logs Service Stop Time to Service Db (updates using ServiceName)
        /// </summary>
        /// <param name="obj">ServiceConfig Object for Service to Update</param>
        /// <returns>Returns True with Successful Db Write</returns>
        public bool ServiceConfig_Stop(ServiceConfig obj)
        {
            SqlConnection Conn = ServiceConfig_Conn();
            string SQLLine = "Update " + ServiceConfig_TableName() + " SET LastStopTime = @LastStopTime, ServiceRunStatus = @ServiceRunStatus, LastCheckInTime = @LastCheckInTime WHERE ServiceName = @ServiceName";
            SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
            cmd2 = new SqlCommand(SQLLine, Conn);

            cmd2.Parameters.AddWithValue(@"ServiceName", obj.ServiceName.ToString());
            cmd2.Parameters.AddWithValue(@"LastStopTime", DateTime.Now.ToString());
            cmd2.Parameters.AddWithValue(@"ServiceRunStatus", "Stopped");
            cmd2.Parameters.AddWithValue(@"LastCheckInTime", DateTime.Now.ToString());

            if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex)
            {
                //_AHK ahk = new _AHK();
                //ahk.MsgBox(ex.ToString());
                return false;
            }
            Conn.Close();
            if (recordsAffected > 0) { return true; }
            else return false;
        }

        /// <summary>
        /// Logs Paused Service Status When Service is Paused (updates using ServiceName)
        /// </summary>
        /// <param name="obj">ServiceConfig Object for Service to Update</param>
        /// <returns>Returns True with Successful Db Write</returns>
        public bool ServiceConfig_Paused(ServiceConfig obj)
        {
            SqlConnection Conn = ServiceConfig_Conn();
            string SQLLine = "Update " + ServiceConfig_TableName() + " SET ServiceRunStatus = @ServiceRunStatus, LastCheckInTime = @LastCheckInTime WHERE ServiceName = @ServiceName";
            SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
            cmd2 = new SqlCommand(SQLLine, Conn);

            cmd2.Parameters.AddWithValue(@"ServiceName", obj.ServiceName.ToString());
            cmd2.Parameters.AddWithValue(@"ServiceRunStatus", "Paused");
            cmd2.Parameters.AddWithValue(@"LastCheckInTime", DateTime.Now.ToString());

            if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex)
            {
                //_AHK ahk = new _AHK();
                //ahk.MsgBox(ex.ToString());
                return false;
            }
            Conn.Close();
            if (recordsAffected > 0) { return true; }
            else return false;
        }


        #endregion

        #endregion


        #region === Service Tables ===

        #region ===== ServiceTables Object =====

        public struct ServiceTables
        {
            public int ID { get; set; }
            public string ServiceName { get; set; }
            public string TableDisplayName { get; set; }
            public string TableConnName { get; set; }
            public string TableSQL { get; set; }
            public bool Enabled { get; set; }
        }


        #endregion
        #region ===== ServiceTables SQLite : Return =====

        public ServiceTables Return_Object_From_ServiceTables(string DbFile, string WhereClause = "[ID] = ''")
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            string SelectLine = "Select [ID], [ServiceName], [TableDisplayName], [TableConnName], [TableSQL], [Enabled] From [ServiceTables] ";
            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
            if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
            if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
            ServiceTables returnObject = new ServiceTables();
            int i = 0;
            string Value = "";
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    returnObject.ID = ret["ID"].ToInt();
                    returnObject.ServiceName = ret["ServiceName"].ToString();
                    returnObject.TableDisplayName = ret["TableDisplayName"].ToString();
                    returnObject.TableConnName = ret["TableConnName"].ToString();
                    returnObject.TableSQL = ret["TableSQL"].ToString();
                    returnObject.Enabled = ret["Enabled"].ToBool();
                }
            }

            return returnObject;
        }

        public List<ServiceTables> Return_ServiceTables_List(string DbFile, string TableName = "[ServiceTables]", string WhereClause = "")
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            string SelectLine = "Select * From " + TableName;

            if (WhereClause != "")
            {
                if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
            }
            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

            List<ServiceTables> ReturnList = new List<ServiceTables>();
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    ServiceTables returnObject = new ServiceTables();

                    returnObject.ID = ret["ID"].ToInt();
                    returnObject.ServiceName = ret["ServiceName"].ToString();
                    returnObject.TableDisplayName = ret["TableDisplayName"].ToString();
                    returnObject.TableConnName = ret["TableConnName"].ToString();
                    returnObject.TableSQL = ret["TableSQL"].ToString();
                    returnObject.Enabled = ret["Enabled"].ToBool();

                    ReturnList.Add(returnObject);
                }
            }

            return ReturnList;
        }

        public DataTable Return_DataTable_From_ServiceTables(string DbFile)
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            string SelectLine = "Select [ID], [ServiceName], [TableDisplayName], [TableConnName], [TableSQL], [Enabled] From [ServiceTables]";
            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
            return ReturnTable;
        }


        #endregion
        #region ===== ServiceTables SQLite : Update Insert =====

        public bool ServiceTables_Insert(string DbFile, ServiceTables inObject)
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            string InsertLine = "Insert Into [ServiceTables] (ID, ServiceName, TableDisplayName, TableConnName, TableSQL, Enabled) values ('" + inObject.ID + "', '" + inObject.ServiceName + "', '" + inObject.TableDisplayName + "', '" + inObject.TableConnName + "', '" + inObject.TableSQL + "', '" + inObject.Enabled + "')";
            bool Inserted = sqlite.Execute(DbFile, InsertLine);
            if (!Inserted)
            {
                _AHK ahk = new _AHK();
                ahk.MsgBox("Inserted Into [ServiceTables] = " + Inserted.ToString());
            }
            return Inserted;
        }

        public bool ServiceTables_Update(string DbFile, ServiceTables inObject, string WHERE = "[Item] = 'Value'")
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            //string UpdateLine = "Update [ServiceTables] set ID = '" + inObject.ID + "', ServiceName = '" + inObject.ServiceName + "', TableDisplayName = '" + inObject.TableDisplayName + "', TableConnName = '" + inObject.TableConnName + "', TableSQL = '" + inObject.TableSQL + "', Enabled = '" + inObject.Enabled + "' WHERE [Item] = 'Value' ";
            string UpdateLine = "Update [ServiceTables] set ";


            if (inObject.ID != null) { UpdateLine = UpdateLine + "[ID] = '" + inObject.ID + "',"; }
            if (inObject.ServiceName != null) { UpdateLine = UpdateLine + "[ServiceName] = '" + inObject.ServiceName + "',"; }
            if (inObject.TableDisplayName != null) { UpdateLine = UpdateLine + "[TableDisplayName] = '" + inObject.TableDisplayName + "',"; }
            if (inObject.TableConnName != null) { UpdateLine = UpdateLine + "[TableConnName] = '" + inObject.TableConnName + "',"; }
            if (inObject.TableSQL != null) { UpdateLine = UpdateLine + "[TableSQL] = '" + inObject.TableSQL + "',"; }
            if (inObject.Enabled != null) { UpdateLine = UpdateLine + "[Enabled] = '" + inObject.Enabled + "',"; }

            _AHK ahk = new _AHK();
            UpdateLine = ahk.TrimLast(UpdateLine, 1);
            UpdateLine = UpdateLine + " WHERE " + WHERE;

            bool Updated = sqlite.Execute(DbFile, UpdateLine);
            return Updated;
        }

        public bool ServiceTables_UpdateInsert(string DbFile, ServiceTables obj, string WhereClause = "")
        {
            bool Updated = ServiceTables_Update(DbFile, obj, WhereClause);  // try to update record first
            if (!Updated) { Updated = ServiceTables_Insert(DbFile, obj); }  // if unable to update, insert new record
            return Updated;
        }


        #endregion
        #region ===== ServiceTables DataTable =====

        public DataTable Return_ServiceTables_DataTable(string DbFile, string TableName = "ServiceTables", string WhereClause = "", bool Debug = false)
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            string SelectLine = "Select * From [ServiceTables]";

            if (WhereClause != "")
            {
                if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
            }

            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


            DataTable table = new DataTable();
            table.Columns.Add("ID", typeof(string));
            table.Columns.Add("ServiceName", typeof(string));
            table.Columns.Add("TableDisplayName", typeof(string));
            table.Columns.Add("TableConnName", typeof(string));
            table.Columns.Add("TableSQL", typeof(string));
            table.Columns.Add("Enabled", typeof(string));

            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    ServiceTables returnObject = new ServiceTables();

                    returnObject.ID = ret["ID"].ToInt();
                    returnObject.ServiceName = ret["ServiceName"].ToString();
                    returnObject.TableDisplayName = ret["TableDisplayName"].ToString();
                    returnObject.TableConnName = ret["TableConnName"].ToString();
                    returnObject.TableSQL = ret["TableSQL"].ToString();
                    returnObject.Enabled = ret["Enabled"].ToBool();

                    table.Rows.Add(returnObject.ID, returnObject.ServiceName, returnObject.TableDisplayName, returnObject.TableConnName, returnObject.TableSQL, returnObject.Enabled);
                }
            }

            return table;
        }

        public DataTable Create_ServiceTables_DataTable(ServiceTables inObject)
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", typeof(string));
            table.Columns.Add("ServiceName", typeof(string));
            table.Columns.Add("TableDisplayName", typeof(string));
            table.Columns.Add("TableConnName", typeof(string));
            table.Columns.Add("TableSQL", typeof(string));
            table.Columns.Add("Enabled", typeof(string));

            table.Rows.Add(inObject.ID, inObject.ServiceName, inObject.TableDisplayName, inObject.TableConnName, inObject.TableSQL, inObject.Enabled);
            return table;
        }


        #endregion
        #region ===== ServiceTables DataGridView =====



        #endregion
        #region ===== ServiceTables SQL Functions =====

        // Return ServiceTables SQL Connection String
        public SqlConnection ServiceTables_Conn()
        {
            // populate sql connection
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["ServiceDb"].ConnectionString);
            if (conn == null)
            {
                _AHK ahk = new _AHK();
                ahk.FileAppend("ServiceDb Returned Null", ahk.DesktopDir() + "\\CRM_TFS_Service_ErrorLog.txt");
            }
            return conn;
        }

        // Return ServiceTables TableName (Full Path)
        public string ServiceTables_TableName()
        {
            string TableNameBase = ConfigurationManager.ConnectionStrings["ServiceTableRoot"].ConnectionString;

            // populate to return full sql table name
            return TableNameBase + ".[ServiceTables]";
        }

        // Generate SQL Table
        public bool ServiceTables_CreateSQLTable(string NewTablePath = "")
        {
            if (NewTablePath == "") { NewTablePath = ServiceTables_TableName(); }

            _Database.SQL sql = new _Database.SQL();
            SqlConnection Conn = ServiceTables_Conn();
            string CreateTableLine = "CREATE TABLE " + NewTablePath + "(";
            CreateTableLine = CreateTableLine + "[ID] [bigint] IDENTITY(1,1) NOT NULL,";
            CreateTableLine = CreateTableLine + "[ServiceName] [varchar](max) NOT NULL,";
            CreateTableLine = CreateTableLine + "[TableDisplayName] [varchar](max) NOT NULL,";
            CreateTableLine = CreateTableLine + "[TableConnName] [varchar](max) NOT NULL,";
            CreateTableLine = CreateTableLine + "[TableSQL] [varchar](max) NOT NULL,";
            CreateTableLine = CreateTableLine + "[Enabled] [bit] NULL";
            CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";

            return sql.WriteDataRecord(Conn, CreateTableLine);
        }

        public bool ServiceTables_InsertSQL(ServiceTables obj)
        {
            SqlConnection Con = ServiceTables_Conn();
            string SQLLine = "Insert Into " + ServiceTables_TableName() + "(ServiceName, TableDisplayName, TableConnName, TableSQL, Enabled) VALUES (@ServiceName, @TableDisplayName, @TableConnName, @TableSQL, @Enabled)";
            SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
            cmd2 = new SqlCommand(SQLLine, Con);
            //if (obj.ServiceName == null) { obj.ServiceName = ""; }
            //if (obj.TableDisplayName == null) { obj.TableDisplayName = ""; }
            //if (obj.TableConnName == null) { obj.TableConnName = ""; }
            //if (obj.TableSQL == null) { obj.TableSQL = ""; }
            //if (obj.Enabled == null) { obj.Enabled = ""; }
            cmd2.Parameters.AddWithValue(@"ServiceName", obj.ServiceName.ToString());
            cmd2.Parameters.AddWithValue(@"TableDisplayName", obj.TableDisplayName.ToString());
            cmd2.Parameters.AddWithValue(@"TableConnName", obj.TableConnName.ToString());
            cmd2.Parameters.AddWithValue(@"TableSQL", obj.TableSQL.ToString());
            cmd2.Parameters.AddWithValue(@"Enabled", obj.Enabled.ToString());
            if (Con.State == ConnectionState.Closed) { Con.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex)
            {
                //_AHK ahk = new _AHK();
                //ahk.MsgBox(ex.ToString());
                return false;
            }
            Con.Close();
            if (recordsAffected > 0) { return true; }
            else return false;
        }

        public bool ServiceTables_UpdateSQL(ServiceTables obj)
        {
            SqlConnection Conn = ServiceTables_Conn();
            string SQLLine = "Update " + ServiceTables_TableName() + " SET TableDisplayName = @TableDisplayName, TableConnName = @TableConnName, TableSQL = @TableSQL, Enabled = @Enabled WHERE ServiceName = @ServiceName and TableDisplayName = @TableDisplayName";
            SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
            cmd2 = new SqlCommand(SQLLine, Conn);

            //if (obj.ID == null) { obj.ID = ""; }
            //if (obj.ServiceName == null) { obj.ServiceName = ""; }
            //if (obj.TableDisplayName == null) { obj.TableDisplayName = ""; }
            //if (obj.TableConnName == null) { obj.TableConnName = ""; }
            //if (obj.TableSQL == null) { obj.TableSQL = ""; }
            //if (obj.Enabled == null) { obj.Enabled = ""; }

            cmd2.Parameters.AddWithValue(@"ServiceName", obj.ServiceName.ToString());
            cmd2.Parameters.AddWithValue(@"TableDisplayName", obj.TableDisplayName.ToString());
            cmd2.Parameters.AddWithValue(@"TableConnName", obj.TableConnName.ToString());
            cmd2.Parameters.AddWithValue(@"TableSQL", obj.TableSQL.ToString());
            cmd2.Parameters.AddWithValue(@"Enabled", obj.Enabled.ToString());

            if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex)
            {
                //_AHK ahk = new _AHK();
                //ahk.MsgBox(ex.ToString());
                return false;
            }
            Conn.Close();
            if (recordsAffected > 0) { return true; }
            else return false;
        }

        public bool ServiceTables_UpdateInsert(ServiceTables obj)
        {
            SqlConnection Conn = ServiceTables_Conn();
            bool Updated = ServiceTables_UpdateSQL(obj);  // try to update record first
            if (!Updated) { Updated = ServiceTables_InsertSQL(obj); }  // if unable to update, insert new record
            return Updated;
        }

        // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
        public bool ServiceTables_UpdateIfPopulated(ServiceTables obj, string ID = "")
        {
            SqlConnection Conn = ServiceTables_Conn();
            string SQLcmd = "Update ServiceTables SET ";
            if (obj.ID != null) { SQLcmd = SQLcmd + " ID = @ID,"; }
            if (obj.ServiceName != null) { SQLcmd = SQLcmd + " ServiceName = @ServiceName,"; }
            if (obj.TableDisplayName != null) { SQLcmd = SQLcmd + " TableDisplayName = @TableDisplayName,"; }
            if (obj.TableConnName != null) { SQLcmd = SQLcmd + " TableConnName = @TableConnName,"; }
            if (obj.TableSQL != null) { SQLcmd = SQLcmd + " TableSQL = @TableSQL,"; }
            if (obj.Enabled != null) { SQLcmd = SQLcmd + " Enabled = @Enabled,"; }

            _AHK ahk = new _AHK();
            SQLcmd = ahk.TrimLast(SQLcmd, 1);
            SQLcmd = SQLcmd + " WHERE ID = @ID";

            SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

            if (obj.ID != null) { cmd2.Parameters.AddWithValue(@"ID", obj.ID); }
            if (obj.ServiceName != null) { cmd2.Parameters.AddWithValue(@"ServiceName", obj.ServiceName); }
            if (obj.TableDisplayName != null) { cmd2.Parameters.AddWithValue(@"TableDisplayName", obj.TableDisplayName); }
            if (obj.TableConnName != null) { cmd2.Parameters.AddWithValue(@"TableConnName", obj.TableConnName); }
            if (obj.TableSQL != null) { cmd2.Parameters.AddWithValue(@"TableSQL", obj.TableSQL); }
            if (obj.Enabled != null) { cmd2.Parameters.AddWithValue(@"Enabled", obj.Enabled); }

            if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex)
            {
                //ahk.MsgBox(ex.ToString());
                return false;
            }
            Conn.Close();
            if (recordsAffected > 0) { return true; }
            else return false;
        }

        public ServiceTables ServiceTables_ReturnSQL(string ServiceName)
        {
            _Database.SQL sql = new _Database.SQL();
            SqlConnection Conn = ServiceTables_Conn();
            string SelectLine = "Select [ID],[ServiceName],[TableDisplayName],[TableConnName],[TableSQL],[Enabled] From " + ServiceTables_TableName() + " WHERE ServiceName = '" + ServiceName + "'";
            DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
            ServiceTables returnObject = new ServiceTables();
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    returnObject.ID = ret["ID"].ToInt();
                    returnObject.ServiceName = ret["ServiceName"].ToString();
                    returnObject.TableDisplayName = ret["TableDisplayName"].ToString();
                    returnObject.TableConnName = ret["TableConnName"].ToString();
                    returnObject.TableSQL = ret["TableSQL"].ToString();
                    returnObject.Enabled = ret["Enabled"].ToBool();
                    return returnObject;
                }
            }
            return returnObject;
        }


        /// <summary>
        /// Returns List of Stored Tables for Service
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <returns></returns>
        public List<ServiceTables> ServiceTableList(string ServiceName)
        {
            string Command = "Select * From " + ServiceTables_TableName() + " WHERE [ServiceName] = '" + ServiceName + "'";

            _Database.SQL sql = new _Database.SQL();
            SqlConnection Conn = ServiceTables_Conn();
            DataTable ReturnTable = sql.GetDataTable(Conn, Command);
            List<ServiceTables> ReturnList = new List<ServiceTables>();
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    ServiceTables returnObject = new ServiceTables();
                    returnObject.ID = ret["ID"].ToInt();
                    returnObject.ServiceName = ret["ServiceName"].ToString();
                    returnObject.TableDisplayName = ret["TableDisplayName"].ToString();
                    returnObject.TableConnName = ret["TableConnName"].ToString();
                    returnObject.TableSQL = ret["TableSQL"].ToString();
                    returnObject.Enabled = ret["Enabled"].ToBool();
                    ReturnList.Add(returnObject);
                }
            }
            return ReturnList;
        }



        #endregion

        #endregion



        #endregion


    }
}

using sharpAHK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
//using System.Web;
//using System.Configuration;
using sharpAHK_Dev;
using System.Net.Mail;
using Telerik.WinControls.UI;
using System.Collections;
using Telerik.WinControls;
using TelerikExpressions;
using AHKExpressions;
using Telerik.WinControls.UI.Docking;
using System.Reflection;

namespace sharpAHK_Dev
{
    public partial class _TelerikLib
    {
        public class RadTree
        {
            #region === Startup ===

            sharpAHK._AHK ahk = new sharpAHK._AHK();
            //_Database.SQL sql = new _Database.SQL();
            //_Database.SQLite sqlite = new _Database.SQLite();
            //_Lists lst = new _Lists();
            //_Images img = new _Images();
            //_Code code = new _Code();
            //_TelerikLib.RadProgress pro = new RadProgress();


            #endregion


            //===== POPULATE =================

            #region === POPULATE RADTREE ===


            #region === Save/Load XML Contents ===

            string xmlDir = @"C:\_Code\LucidProjects\ADBindex\IMDbDisp\bin\Debug\RadTreeXML";


            /// <summary>
            /// Create New XML File and Load In Tree For Editing 
            /// </summary>
            /// <param name="RadTree"></param>
            /// <param name="NewName"></param>
            public void New_TreeXML(RadTreeView RadTree, string NewName = "RadTreeXML")
            {
                if (RadTree == null) { return; }

                StringBuilder _builder = new StringBuilder();
                _builder.Append("<?xml version=\"1.0\" encoding=\"utf-16\"?>");
                _builder.Append("<TreeView xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" MultiSelect=\"true\" SpacingBetweenNodes=\"2\" AllowDragDrop=\"true\" TriStateMode=\"true\" CheckBoxes=\"true\" LabelEdit=\"true\" AllowDrop=\"true\" BackColor=\"Transparent\">");
                _builder.Append("</TreeView>");

                string xmlDir = ahk.AppDir() + "\\RadTreeXML";
                ahk.FileCreateDir(xmlDir);

                string file = xmlDir + "\\" + NewName + ".xml";
                ahk.FileDelete(file);

                ahk.FileAppend(_builder.ToString(), file);

                LoadTree(RadTree, NewName);
            }

            //===== LOAD ===================================

            /// <summary>
            /// Clear Nodes from RadTreeView
            /// </summary>
            /// <param name="RadTree"></param>
            public void ClearTree(RadTreeView RadTree)
            {
                if (RadTree == null) { return; }

                if (RadTree.InvokeRequired)  // if currently on a different thread, invoke 
                {
                    RadTree.BeginInvoke((MethodInvoker)delegate () { RadTree.Nodes.Clear(); RadTree.Controls.Clear(); RadTree.Invalidate(); });
                }
                else
                {
                    RadTree.Nodes.Clear(); RadTree.Controls.Clear(); RadTree.Invalidate();
                }
            }

            /// <summary>
            /// Save RadTreeView Contents to SQLite Entry 
            /// </summary>
            /// <param name="RadTree"></param>
            /// <param name="SaveName"></param>
            /// <param name="SaveDir"></param>
            public void SaveTree(RadTreeView RadTree, string SaveName = "RadTreeXML", string SaveDir = "\\RadTreeXML")
            {
                if (RadTree == null) { return; }

                //sqlite.Setting(SaveName, RadTree.TreeViewXml.ToString());

                var xml = NodeXML(RadTree);

                string xmlDir = ahk.AppDir() + "\\RadTreeXML";

                if (SaveDir != "\\RadTreeXML") { xmlDir = SaveDir; }

                //string xmlDir = @"C:\_Code\LucidProjects\ADBindex\IMDbDisp\bin\Debug\RadTreeXML";
                ahk.FileCreateDir(xmlDir);

                // Store SaveName as Tree Tag 
                string tagText = "";

                if (SaveName == "")
                {
                    if (RadTree.Tag != null)
                    {
                        tagText = RadTree.Tag.ToString();
                        if (tagText.Contains("XML:"))
                        {
                            SaveName = tagText.Replace("XML:", "");
                            RadTree.Tag = SaveName;
                        }
                    }
                }
                else
                {
                    RadTree.Tag = "XML:" + SaveName;
                }


                string path = xmlDir + "\\" + SaveName + ".xml";
                ahk.FileDelete(path);
                ahk.FileAppend(xml.ToString(), path);
            }


            /// <summary>
            /// Restore RadTreeView Contents from SQLite Entry 
            /// </summary>
            /// <param name="RadTree"></param>
            /// <param name="SaveName"></param>
            /// <param name="SaveDir"></param>
            public void LoadTree(RadTreeView RadTree, string SaveName = "RadTreeXML", string SaveDir = "\\RadTreeXML")
            {
                if (RadTree == null) { return; }

                //string xmlDir = ahk.AppDir() + "\\RadTreeXML";
                RadTree.Tag = "XML:" + SaveName;

                if (SaveDir == "\\RadTreeXML") { SaveDir = ahk.AppDir() + "\\RadTreeXML"; }

                xmlDir = SaveDir;

                string xmlFile = xmlDir + "\\" + SaveName + ".xml";
                string xml = ahk.FileRead(xmlFile);

                if (xml != "" && xml != "\r\n")
                {
                    RadTree.LoadNodeXML(xml);
                }
            }

            /// <summary>
            /// Returns List of XML Files in XML TreeLoadDir
            /// </summary>
            /// <returns></returns>
            public List<string> XMLDir_List()
            {
                _Lists lst = new _Lists();

                xmlDir = ahk.AppDir() + "\\RadTreeXML";
                List<string> files = lst.FileList(xmlDir, "*.xml", false, false, true);
                return files;
            }


            #endregion


            #region === Code RadTree ===

            /// <summary>
            /// Populate RadTreeView with Parsed C# Function Display
            /// </summary>
            /// <param name="RadTree"></param>
            /// <param name="file"></param>
            /// <param name="codeProgress"></param>
            public void RadCodeTree(RadTreeView RadTree, string file, RadProgressBar codeProgress = null)
            {
                if (RadTree == null) { return; }
                RadTreeNode Name = new RadTreeNode(file.FileName());
                //DbName.Image = img.resizeImage(20, 20, @"D:\_Images\icoBin_v2\database\Database-48.png");
                //DbName.ItemHeight = 40;
                //DbName.CheckType = CheckType.None;
                Name.Tag = file;
                RadTree.AddNode(Name);

                _Code code = new _Code();
                _TelerikLib.RadProgress pro = new RadProgress();

                string allCode = ahk.FileRead(file);
                List<string> functions = code.FunctionList_FromCode(allCode);

                if (codeProgress != null) { pro.SetupProgressBar(codeProgress, functions.Count); }

                int i = 0;
                if (functions != null)
                {
                    foreach (string function in functions)
                    {
                        i++;
                        if (codeProgress != null) { pro.UpdateProgress(codeProgress, i + "/" + functions.Count()); }
                        string Code = code.Return_Function_FromCode(allCode, function);
                        RadTreeNode Func = new RadTreeNode(function);
                        //DbName.Image = img.resizeImage(20, 20, @"D:\_Images\icoBin_v2\database\Database-48.png");
                        //DbName.ItemHeight = 40;
                        //DbName.CheckType = CheckType.None;
                        Func.Tag = Code;
                        Name.AddSubNode(Func);
                    }
                }

            }


            #endregion


            #region === SQL/SQLite RadTree ===

            public void SQL_RadTree(RadTreeView RadTree)
            {
                _Database.SQL sql = new _Database.SQL();
                _Database.SQLite sqlite = new _Database.SQLite();

                // LucidMedia Db
                string SQLPath = "[lucidmedia].[lucidmethod]";
                string ConnName = "SQLserver";

                RadTreeNode DbName = new RadTreeNode("LucidMedia");
                //DbName.Image = img.resizeImage(20, 20, @"D:\_Images\icoBin_v2\database\Database-48.png");
                DbName.ItemHeight = 40;
                DbName.CheckType = CheckType.None;
                RadTree.Nodes.Add(DbName);

                List<string> SQLTableNames = sql.Return_TableList(sql.GetConn(ConnName), "LucidMedia");

                foreach (string TableName in SQLTableNames)
                {
                    RadTreeNode dirNode = new RadTreeNode(TableName);
                    dirNode.Tag = SQLPath + ".[" + TableName + "]";
                    DbName.Nodes.Add(dirNode);

                    //List<string> Columns = sql.Return_ColumnNames(sql.GetConn(ConnName), string DataBaseName = "", string TableName = "")
                }


                // FileServer Db
                SQLPath = "[fileserver].[lucidmethod]";
                ConnName = "fileserver";

                RadTreeNode aDbName = new RadTreeNode("FileServer");
                //DbName.Image = img.resizeImage(20, 20, @"D:\_Images\icoBin_v2\database\Database-48.png");
                aDbName.ItemHeight = 40;
                aDbName.CheckType = CheckType.None;
                RadTree.Nodes.Add(aDbName);

                List<string> fsSQLTableNames = sql.Return_TableList(sql.GetConn(ConnName), "FileServer");

                foreach (string TableName in fsSQLTableNames)
                {
                    RadTreeNode dirNode = new RadTreeNode(TableName);
                    dirNode.Tag = SQLPath + ".[" + TableName + "]";
                    aDbName.Nodes.Add(dirNode);

                    //List<string> Columns = sql.Return_ColumnNames(sql.GetConn(ConnName), string DataBaseName = "", string TableName = "")
                }



                // CodeServer Db
                SQLPath = "[codeserver].[lucidmethod]";
                ConnName = "codeserver";

                RadTreeNode bDbName = new RadTreeNode("CodeServer");
                //DbName.Image = img.resizeImage(20, 20, @"D:\_Images\icoBin_v2\database\Database-48.png");
                bDbName.ItemHeight = 40;
                bDbName.CheckType = CheckType.None;
                RadTree.Nodes.Add(bDbName);

                List<string> fsaSQLTableNames = sql.Return_TableList(sql.GetConn(ConnName), "CodeServer");

                foreach (string TableName in fsaSQLTableNames)
                {
                    RadTreeNode dirNode = new RadTreeNode(TableName);
                    dirNode.Tag = SQLPath + ".[" + TableName + "]";
                    bDbName.Nodes.Add(dirNode);

                    //List<string> Columns = sql.Return_ColumnNames(sql.GetConn(ConnName), string DataBaseName = "", string TableName = "")
                }



                // adbindex Db
                SQLPath = "[adbindex].[lucidmethod]";
                ConnName = "adbindex";

                RadTreeNode cDbName = new RadTreeNode("ADBIndex");
                //DbName.Image = img.resizeImage(20, 20, @"D:\_Images\icoBin_v2\database\Database-48.png");
                cDbName.ItemHeight = 40;
                cDbName.CheckType = CheckType.None;
                RadTree.Nodes.Add(cDbName);

                List<string> fszSQLTableNames = sql.Return_TableList(sql.GetConn(ConnName), "ADBIndex");

                foreach (string TableName in fszSQLTableNames)
                {
                    RadTreeNode dirNode = new RadTreeNode(TableName);
                    dirNode.Tag = SQLPath + ".[" + TableName + "]";
                    cDbName.Nodes.Add(dirNode);

                    //List<string> Columns = sql.Return_ColumnNames(sql.GetConn(ConnName), string DataBaseName = "", string TableName = "")
                }



                // sqlite db import

                string sqliteDir = ahk.AppDir() + "\\Db";
                List<string> files = ahk.FileList(sqliteDir, "*.sqlite", false);
                foreach (string file in files)
                {
                    RadTreeNode radName = new RadTreeNode(ahk.FileName(file));
                    //DbName.Image = img.resizeImage(20, 20, @"D:\_Images\icoBin_v2\database\Database-48.png");
                    radName.ItemHeight = 40;
                    radName.CheckType = CheckType.None;
                    RadTree.Nodes.Add(radName);

                    List<string> TableNames = sqlite.Table_List(file);

                    foreach (string TableName in TableNames)
                    {
                        RadTreeNode dirNode = new RadTreeNode(TableName);
                        //dirNode.Tag = "[" + TableName + "]";
                        dirNode.Tag = file;
                        radName.Nodes.Add(dirNode);
                        //List<string> Columns = sql.Return_ColumnNames(sql.GetConn(ConnName), string DataBaseName = "", string TableName = "")
                    }

                }


                //RadTreeActions(radTreeView1, "Sort Nodes Asc");

            }

            public void sql_RadTree(RadTreeView RadTree)
            {
                if (RadTree == null) { return; }

                _Database.SQL sql = new _Database.SQL();
                _Images img = new _Images();

                string SQLPath = "[lucidmedia].[lucidmethod]";
                string ConnName = "SQLserver";

                RadTreeNode DbName = new RadTreeNode("LucidMedia");
                DbName.Image = img.resizeImage(25, 25, @"D:\_Images\icoBin_v2\database\Database-48.png");
                RadTree.Nodes.Add(DbName);

                List<string> SQLTableNames = sql.Return_TableList(sql.GetConn(ConnName));

                if (SQLTableNames != null)
                {
                    foreach (string TableName in SQLTableNames)
                    {
                        RadTreeNode dirNode = new RadTreeNode(TableName);
                        dirNode.Tag = SQLPath + ".[" + TableName + "]";
                        DbName.Nodes.Add(dirNode);
                        //List<string> Columns = sql.Return_ColumnNames(sql.GetConn(ConnName), string DataBaseName = "", string TableName = "")
                    }
                }

            }

            #endregion


            #region === UnRAR RadTree ===

            // Display All RAR Files Under UnRAR Dir
            public void UnRarTree(RadTreeView RadTree, string RARFileDir)
            {
                _Lists lst = new _Lists();

                string dir = RARFileDir;

                RadTree.Nodes.Clear();

                List<string> files = lst.FileList(dir, "*.rar", true, false, true);
                foreach (string file in files)
                {
                    if (file.Contains(".part2.")) { continue; }
                    if (file.Contains(".part3.")) { continue; }

                    RadTreeNode dirNode = new RadTreeNode(file.DirName());
                    dirNode.Tag = file;
                    RadTree.Nodes.Add(dirNode);
                }

                //sb("Added " + files.Count + " RAR Files From " + dir);
                //sqlite.Setting("LastUnRARFolderPath", txtUnRARDir.Text);
            }

            // Loop Through Nodes with RAR Paths, UnRAR Paths 
            public void UnRAR_TreeNodes(RadTreeView RadTree, RadProgressBar RadProg)
            {
                _TelerikLib.RadProgress pro = new RadProgress();
                _TelerikLib.RadTree tree = new RadTree();

                List<RadTreeNode> Nodes = tree.AllNodes(RadTree);

                pro.SetupProgressBar(RadProg, Nodes.Count());
                pro.ResetProgress(RadProg);

                int i = 1;
                foreach (RadTreeNode node in Nodes)
                {
                    //bool RunJob = IsChecked(cbUnRARjob);
                    bool RunJob = true;

                    if (RunJob)
                    {
                        string path = node.Tag.ToString();
                        pro.UpdateProgress(RadProg, path + " (" + i + "/" + Nodes.Count() + ")");
                        // CheckboxText(cbUnRARjob, "Running...");
                        NodeColor(node, Color.GreenYellow, Color.Black);

                        //UnRAR_Movie(path);
                        NodeColor(node, Color.Red, Color.White);
                    }
                    else
                    {
                        pro.ResetProgress(RadProg);
                        //CheckboxText(cbUnRARjob, "Stopped By User");
                        break;
                    }
                }

                ahk.MsgBox("Done UnRARing");
            }


            #endregion



            #endregion


            //===== NODES =================

            #region === RADTREE NODES ===


            #region === Add Nodes ===

            /// <summary>
            /// Add New Node to TreeView Using Text (From Any Thread) 
            /// </summary>
            /// <param name="RadTree"></param>
            /// <param name="Text"></param>
            /// <param name="Tag"></param>
            /// <param name="Checked"></param>
            /// <returns></returns>
            public RadTreeNode NewNode(RadTreeView RadTree, string Text = "", string Tag = "", bool Checked = false)
            {
                if (RadTree == null) { return new RadTreeNode(); }

                RadTreeNode castNode = new RadTreeNode();
                castNode.Text = Text;
                castNode.Checked = Checked;

                if (Tag == "") { Tag = Text; }
                castNode.Tag = Tag;
                //castNode = RadTree.Nodes.Add(Text);

                AddNode(RadTree, castNode);

                return castNode;
            }

            /// <summary>
            /// Add TreeNode to TreeView (From Any Thread) 
            /// </summary>
            /// <param name="RadTree"></param>
            /// <param name="node"></param>
            /// <param name="Checked"></param>
            public void AddNode(RadTreeView RadTree, RadTreeNode node, bool Checked = false)
            {
                if (RadTree == null) { return; }

                if (RadTree.InvokeRequired)  // if currently on a different thread, invoke 
                {
                    RadTree.BeginInvoke((MethodInvoker)delegate () { RadTree.Nodes.Add(node); });
                }
                else
                {
                    RadTree.Nodes.Add(node);
                }
            }

            /// <summary>
            /// Add SubNode to Existing RadTreeNode on RadTreeView (From Any Thread)
            /// </summary>
            /// <param name="existingNode"></param>
            /// <param name="newNode"></param>
            /// <param name="RadTree"></param>
            public void AddSubNode(RadTreeNode existingNode, RadTreeNode newNode, RadTreeView RadTree = null)
            {
                if (RadTree == null) { RadTree = existingNode.TreeView; }
                if (RadTree == null) { return; }

                if (existingNode == null) { return; }
                if (newNode == null) { return; }

                if (RadTree.InvokeRequired)  // if currently on a different thread, invoke 
                {
                    RadTree.BeginInvoke((MethodInvoker)delegate () { existingNode.Nodes.Add(newNode); });
                }
                else
                {
                    existingNode.Nodes.Add(newNode);
                }
            }

            /// <summary>
            /// Checks Tree For Node with Same Text (optionally same tag text), Adds Node To RadTree If Missing From RadTree Display (From Any Thread)
            /// </summary>
            /// <param name="RadTree"></param>
            /// <param name="NewNode"></param>
            /// <param name="MatchByTagText"></param>
            /// <param name="ParentNode"></param>
            public void AddNodeIfMissing(RadTreeView RadTree, RadTreeNode NewNode, bool MatchByTagText = false, RadTreeNode ParentNode = null)
            {
                if (RadTree == null) { return; }

                bool inTree = false;

                if (MatchByTagText)
                {
                    inTree = NodeExist(RadTree, NewNode.Tag.ToString(), MatchByTagText);
                }
                else
                {
                    inTree = NodeExist(RadTree, NewNode.Text, MatchByTagText);
                }

                if (!inTree)  // add to tree if match not found
                {
                    if (ParentNode != null) // if parent provided, add as subnode
                    {
                        AddSubNode(ParentNode, NewNode, RadTree);
                    }
                    else  // otherwise add to tree root
                    {
                        AddNode(RadTree, NewNode);
                    }
                }
            }

            #endregion

            #region === FIND Nodes ===

            /// <summary>
            /// Create / Return Existing Node : Searches RadGrid for Node by Text / TagText if Provided, Returns Populated Node if Not Located. 
            /// </summary>
            /// <param name="RadTree">RadTree To Search</param>
            /// <param name="NodeText"></param>
            /// <param name="TagText"></param>
            /// <returns></returns>
            public RadTreeNode ReturnNode(RadTreeView RadTree, string NodeText = "", string TagText = "", bool MatchByTagText = false)
            {
                RadTreeNode returnNode = new RadTreeNode();

                if (RadTree == null) { return returnNode; }

                // Search RadTree For Node By Text/TagText - Return Matching Node if Possible
                List<RadTreeNode> nodes = AllNodes(RadTree);
                if (nodes != null)
                {
                    foreach (RadTreeNode node in nodes)
                    {
                        if (MatchByTagText) // search by tag text, return 
                        {
                            if (node.Tag != null)
                            {
                                if (node.Tag.ToString().ToUpper() == NodeText.ToUpper()) { return node; }
                            }
                        }
                        else // search by text
                        {
                            if (node.Text.ToUpper() == NodeText.ToUpper()) { return node; }
                        }
                    }
                }


                // OtherWise Populate the Node with 
                returnNode.Text = NodeText;
                returnNode.Tag = TagText;

                return returnNode;
            }

            /// <summary>
            /// Check RadTree for Node Searching by  Text or Tag Text 
            /// </summary>
            /// <param name="radTree">RadTree To Search</param>
            /// <param name="SearchText">Text to match with existing node text in RadTree</param>
            /// <param name="SearchTags">Option to Search RadTree by Node Tag Text instead of Node Text (default = False)</param>
            /// <returns></returns>
            public bool NodeExist(RadTreeView RadTree, string SearchText, bool SearchTags = false)
            {
                if (RadTree == null) { return false; }
                bool found = false;
                List<RadTreeNode> nodes = AllNodes(RadTree);
                if (nodes != null)
                {
                    foreach (RadTreeNode node in nodes)
                    {
                        if (SearchTags) // search by tag text
                        {
                            if (node.Tag != null)
                            {
                                if (node.Tag.ToString().ToUpper() == SearchText.ToUpper()) { return true; }
                            }
                        }
                        else // search by text
                        {
                            if (node.Text.ToUpper() == SearchText.ToUpper()) { return true; }
                        }
                    }
                }

                return found;
            }


            #endregion

            #region === Nodes + Lists ===

            /// <summary>
            /// Add List of Items to RadTreeView Control
            /// </summary>
            /// <param name="Items"></param>
            /// <param name="parent"></param>
            /// <param name="RadTree"></param>
            /// <param name="Tag"></param>
            public void ListRadTree(Telerik.WinControls.UI.RadTreeView RadTree, List<string> Items, string Tag = "", string ParentNodeText = "")
            {
                if (RadTree == null) { return; }
                if (Items == null) { return; }

                _TelerikLib.RadTree tv = new _TelerikLib.RadTree();

                RadTreeNode parent = new RadTreeNode(ParentNodeText);

                if (ParentNodeText != "") { parent.Text = ParentNodeText; tv.AddNode(RadTree, parent); }

                int i = 1;
                if (Items != null)
                {
                    foreach (string item in Items)
                    {
                        Telerik.WinControls.UI.RadTreeNode child = new Telerik.WinControls.UI.RadTreeNode(item);
                        if (Tag != "") { child.Tag = Tag; } // add tag value if user passed value
                        else { child.Tag = item; }  // otherwise assign same value as display text

                        if (ParentNodeText != "") { tv.AddSubNode(parent, child, RadTree); }
                        if (ParentNodeText == "") { tv.AddNode(RadTree, child); }

                        i++;
                    }
                }

            }

            public void SQLRadTree(Telerik.WinControls.UI.RadTreeView RadTree, Dictionary<string, string> Items, string Tag = "", string ParentNodeText = "")
            {
                if (RadTree == null) { return; }
                if (Items == null) { return; }

                _TelerikLib.RadTree tv = new _TelerikLib.RadTree();

                RadTreeNode parent = new RadTreeNode(ParentNodeText);

                if (ParentNodeText != "") { parent.Text = ParentNodeText; tv.AddNode(RadTree, parent); }

                int i = 1;
                if (Items != null)
                {
                    foreach (string item in Items.Keys)
                    {
                        Telerik.WinControls.UI.RadTreeNode child = new Telerik.WinControls.UI.RadTreeNode(item);
                        if (Tag != "") { child.Tag = Tag; } // add tag value if user passed value
                        else { child.Tag = item; }  // otherwise assign same value as display text

                        if (ParentNodeText != "") { tv.AddSubNode(parent, child, RadTree); }
                        if (ParentNodeText == "") { tv.AddNode(RadTree, child); }

                        i++;
                    }
                }

            }


            /// <summary>
            /// Adds List of FilePaths to RadTreeView. Displays as: FileName.Ext, with Full FilePath as the Node Tag
            /// </summary>
            /// <param name="RadTree"></param>
            /// <param name="FilePaths"></param>
            /// <param name="ParentNodeText"></param>
            public void FileList_RadTree(Telerik.WinControls.UI.RadTreeView RadTree, List<string> FilePaths, string ParentNodeText = "", bool ShowExt = true)
            {
                if (RadTree == null) { return; }
                if (FilePaths == null) { return; }

                _TelerikLib.RadTree tv = new _TelerikLib.RadTree();

                RadTreeNode parent = new RadTreeNode(ParentNodeText);

                if (ParentNodeText != "") { parent.Text = ParentNodeText; tv.AddNode(RadTree, parent); }

                int i = 1;
                if (FilePaths != null)
                {
                    foreach (string item in FilePaths)
                    {
                        Telerik.WinControls.UI.RadTreeNode child = new Telerik.WinControls.UI.RadTreeNode();

                        if (ShowExt) { child.Text = ahk.FileName(item); }  // display includes .format 
                        else { child.Text = ahk.FileNameNoExt(item); }  // no file extention displayed

                        child.Tag = item.ToString();  // add full filepath as node tag

                        if (ParentNodeText != "") { tv.AddSubNode(parent, child, RadTree); } // add as subnode to parent
                        if (ParentNodeText == "") { tv.AddNode(RadTree, child); }  // no parent text provided, adds to root of radtreeview

                        i++;
                    }
                }

            }


            /// <summary>
            /// Add List of Items to Existing RadTreeNode as SubNodes
            /// </summary>
            /// <param name="Items"></param>
            /// <param name="parent"></param>
            /// <param name="TV"></param>
            /// <param name="Tag"></param>
            public void RadTree_SubNodeList(List<string> Items, Telerik.WinControls.UI.RadTreeNode parent, Telerik.WinControls.UI.RadTreeView RadTree, string Tag = "")
            {
                if (RadTree == null) { return; }
                if (Items == null) { return; }

                _TelerikLib.RadTree tv = new _TelerikLib.RadTree();

                foreach (string item in Items)
                {
                    Telerik.WinControls.UI.RadTreeNode child = new Telerik.WinControls.UI.RadTreeNode(item);
                    if (Tag != "") { child.Tag = Tag; } else { Tag = item; }  // populate tag value with provided value or item text
                    tv.AddSubNode(parent, child, RadTree); // add subnode to parent node and attach to tree (from any thread)
                }
            }



            /// <summary>
            /// Bring Last Node Into View 
            /// </summary>
            /// <param name="RadTree"></param>
            public void ViewLastNode(RadTreeView RadTree)
            {
                if (RadTree == null) { return; }

                if (RadTree.Nodes.Count > 0)
                {
                    int Last = RadTree.Nodes.Count;
                    RadTreeNode lastRootNode = RadTree.Nodes[Last - 1];

                    if (Last > 0)
                    {
                        try
                        {
                            if (RadTree.InvokeRequired)  // if currently on a different thread, invoke 
                            {
                                RadTree.BeginInvoke((MethodInvoker)delegate () { RadTree.BringIntoView(lastRootNode); });
                            }
                            else
                            {
                                RadTree.BringIntoView(lastRootNode);
                            }
                        }
                        catch
                        { }
                    }
                }
            }


            #endregion

            #region  === Return ===

            /// <summary>
            /// Loop Through Tree Nodes, Returns String with NODE TAG Text on All Checked Items
            /// </summary>
            /// <param name="RadTree"></param>
            /// <returns></returns>
            public string Checked_NodeTagText(RadTreeView RadTree)
            {
                if (RadTree == null) { return ""; }

                string nodeTags = "";

                foreach (RadTreeNode node in RadTree.CheckedNodes)
                {
                    if (node.Tag == null) { continue; }

                    string tagtext = node.Tag.ToString();
                    bool checkedNode = node.Checked;

                    if (node.Checked == true)
                    {
                        if (nodeTags == "") { nodeTags = node.Tag.ToString(); }
                        else { nodeTags = nodeTags + "\n\n" + node.Tag.ToString(); }
                    }
                }

                return nodeTags;
            }

            /// <summary>
            /// Loop Through Tree Nodes, Returns List of NODE TAGS Text on All Checked Items
            /// </summary>
            /// <param name="radTree">RadTreeView Control to Read</param>
            /// <returns>Returns list of TreeNode Tags</returns>
            public List<string> Checked_NodeTagList(RadTreeView RadTree)
            {
                List<string> tags = new List<string>();

                if (RadTree == null) { return tags; }

                foreach (RadTreeNode node in RadTree.CheckedNodes)
                {
                    if (node.Tag == null) { continue; }

                    string tagtext = node.Tag.ToString();
                    bool checkedNode = node.Checked;

                    if (node.Checked == true)
                    {
                        tags.Add(node.Tag.ToString());
                    }
                }

                return tags;
            }

            /// <summary>
            /// Return Text From Checked Nodes
            /// </summary>
            /// <param name="RadTree"></param>
            /// <returns></returns>
            public List<string> Checked_TextList(RadTreeView RadTree)
            {
                List<string> tags = new List<string>();

                if (RadTree == null) { return tags; }

                foreach (RadTreeNode node in RadTree.CheckedNodes)
                {
                    tags.Add(node.Text);
                }

                return tags;
            }


            /// <summary>
            /// Returns List of All Nodes in RadTree
            /// </summary>
            /// <param name="RadTree">RadTreeView Control To Use</param>
            /// <returns></returns>
            public List<RadTreeNode> AllNodes(RadTreeView RadTree)
            {
                if (RadTree == null) { return new List<RadTreeNode>(); }
                if (RadTree.Nodes.Count == 0) { return new List<RadTreeNode>(); }

                // Define the True/False Criteria for Your Node Search (ex: only level 1 nodes returend)
                Predicate<RadTreeNode> match = new Predicate<RadTreeNode>(delegate (RadTreeNode node)
                {
                    if (node.Level >= 0) { return true; }
                    else
                    {
                        return false;
                    }
                });

                List<RadTreeNode> results = new List<RadTreeNode>();

                if (match != null)
                {
                    try
                    {
                        // Return list of nodes to loop based on Node Match Criteria
                        RadTreeNode[] result = RadTree.FindNodes(match);

                        //ahk.MsgBox("matches found = " + result.Count());    // # of matches found

                        // Loop through matching nodes
                        if (result != null)
                        {
                            foreach (RadTreeNode no in result)
                            {
                                results.Add(no);
                            }
                        }

                    }
                    catch
                    {
                        return results;
                    }
                }

                return results;
            }

            /// <summary>
            /// Return List of Nodes From Specific Level # 
            /// </summary>
            /// <param name="radTree">RadTreeView Control To Use</param>
            /// <param name="Level">Node Level Number (Starting with 0)</param>
            /// <returns></returns>
            public List<RadTreeNode> NodesByLevel(RadTreeView RadTree, int Level = 0)
            {
                if (RadTree == null) { return new List<RadTreeNode>(); }

                // Define the True/False Criteria for Your Node Search (ex: only level 1 nodes returend)
                Predicate<RadTreeNode> match = new Predicate<RadTreeNode>(delegate (RadTreeNode node)
                {
                    if (node.Level == Level) { return true; }
                    else
                    {
                        return false;
                    }
                });

                // Return list of nodes to loop based on Node Match Criteria
                RadTreeNode[] result = RadTree.FindNodes(match);

                //ahk.MsgBox("matches found = " + result.Count());    // # of matches found

                List<RadTreeNode> results = new List<RadTreeNode>();

                // Loop through matching nodes

                if (result != null)
                {
                    foreach (RadTreeNode no in result)
                    {
                        results.Add(no);
                    }
                }


                return results;
            }








            /// <summary>
            /// Returns True if All Nodes at Level (Level 0 by Default) are Expanded
            /// </summary>
            /// <param name="RadTree">RadTreeView containing Nodes to Check</param>
            /// <returns></returns>
            public bool AllExpanded(RadTreeView RadTree, int Level = 0)
            {
                foreach (RadTreeNode node in RadTree.Nodes)
                {
                    if (node.Level <= Level) { if (!node.Expanded) { return false; } }
                }

                return true;
            }

            /// <summary>
            /// Returns True if All Nodes at Level (Level 0 by Default) are Expanded
            /// </summary>
            /// <param name="RadTree">RadTreeView containing Nodes to Check</param>
            /// <returns></returns>
            public bool AllCollapsed(RadTreeView RadTree, int Level = 0)
            {
                foreach (RadTreeNode node in RadTree.Nodes)
                {
                    if (node.Level <= Level) { if (!node.Expanded) { return false; } }
                }

                return true;
            }



            #endregion


            #region === Populate ===

            /// <summary>
            /// Loop Through RootDir, Add Folder Names Under Root Path as Nodes
            /// </summary>
            /// <param name="RadTree">RadTreeView to Populate</param>
            /// <param name="DirPath">Root Dir To Search For Folders</param>
            public void RadTree_Folders(RadTreeView RadTree, string DirPath)
            {
                if (RadTree == null) { return; }

                _Lists lst = new _Lists();

                // root node
                RadTreeNode folder = new RadTreeNode(ahk.DirName(DirPath));
                string icoPath = ahk.AppDir() + "\\ICO\\Using\\TreeFolder.ico";
                if (File.Exists(icoPath)) { folder.Image = icoPath.ToImg(); }
                RadTree.Nodes.Add(folder);

                List<string> DirList = lst.DirList(DirPath, "*.*", false, true);
                if (DirList != null)
                {
                    foreach (string Dir in DirList)
                    {
                        RadTreeNode dirNode = new RadTreeNode(ahk.DirName(Dir));
                        dirNode.Tag = Dir;

                        // add icon 
                        icoPath = ahk.AppDir() + "\\ICO\\Using\\TreeFolder.ico";
                        if (File.Exists(icoPath)) { dirNode.Image = icoPath.ToImg(); }

                        folder.Nodes.Add(dirNode);
                    }
                }

            }

            /// <summary>
            /// Loop Through RootDir, Add File Names Under Root Path as Nodes
            /// </summary>
            /// <param name="RadTree">RadTreeView to Populate</param>
            /// <param name="DirPath">Root Dir To Search For Files</param>
            public void RadTree_Files(RadTreeView RadTree, string DirPath)
            {
                if (RadTree == null) { return; }

                _Lists lst = new _Lists();

                RadTreeNode folder = new RadTreeNode(ahk.DirName(DirPath));
                folder.Tag = DirPath;

                string icoPath = ahk.AppDir() + "\\ICO\\Using\\TreeFolder.ico";
                if (File.Exists(icoPath)) { folder.Image = icoPath.ToImg(); }
                //RadTree.Nodes.Add(folder);

                AddNode(RadTree, folder);


                List<string> FList = lst.FileList(DirPath, "*.*", false, false);
                if (FList != null)
                {
                    foreach (string file in FList)
                    {
                        RadTreeNode dirNode = new RadTreeNode(file.FileName());
                        dirNode.Tag = file;

                        // add file format
                        string ext = file.FileExt().Replace(".", "");
                        icoPath = ahk.AppDir() + "\\ICO\\FileFormats\\" + ext + ".ico";
                        if (File.Exists(icoPath)) { dirNode.Image = icoPath.ToImg(); }

                        AddSubNode(folder, dirNode, RadTree);
                        //folder.Nodes.Add(dirNode);

                        //AddNode(RadTree, dirN);
                    }
                }

            }



            #endregion


            #region === RadTree XML ===

            /// <summary>
            /// Returns XML From RadTree (From Any Thread)
            /// </summary>
            /// <param name="RadTree"></param>
            /// <returns></returns>
            public string NodeXML(RadTreeView RadTree)
            {
                if (RadTree == null) { return ""; }

                string xml = "";

                if (RadTree.InvokeRequired)  // if currently on a different thread, invoke 
                {
                    RadTree.BeginInvoke((MethodInvoker)delegate () { xml = RadTree.TreeViewXml.ToString(); });
                }
                else
                {
                    xml = RadTree.TreeViewXml.ToString();
                }

                return xml;
            }

            /// <summary>
            /// Load Previously Saved XML into RadTreeView
            /// </summary>
            /// <param name="RadTree"></param>
            /// <param name="xml"></param>
            public void LoadNodeXML(RadTreeView RadTree, string xml)
            {
                if (RadTree == null) { return; }

                if (RadTree.InvokeRequired)  // if currently on a different thread, invoke 
                {
                    RadTree.BeginInvoke((MethodInvoker)delegate () { RadTree.TreeViewXml = xml; });
                }
                else
                {
                    RadTree.TreeViewXml = xml;
                }
            }

            //string xml = RadTree.TreeViewXml.ToString();


            #endregion

            #region === Node Modify ===

            /// <summary>
            /// Update Node Background Color (All 4 BackColor Fields) + Node Text Color
            /// </summary>
            /// <param name="node">RadTreeNode To Update</param>
            /// <param name="BackColor">Node Background Color</param>
            /// <param name="TextColor">Node Text Color</param>
            public RadTreeNode NodeColor(RadTreeNode node, System.Drawing.Color BackColor, System.Drawing.Color TextColor)
            {
                if (node.TreeView == null) { return node; }

                if (node.TreeView.InvokeRequired)  // if currently on a different thread, invoke 
                {
                    node.TreeView.BeginInvoke((MethodInvoker)delegate ()
                    {
                        node.BackColor = BackColor;
                        node.BackColor2 = BackColor;
                        node.BackColor3 = BackColor;
                        node.BackColor4 = BackColor;

                        node.ForeColor = TextColor;
                    });

                }
                else
                {
                    node.BackColor = BackColor;
                    node.BackColor2 = BackColor;
                    node.BackColor3 = BackColor;
                    node.BackColor4 = BackColor;

                    node.ForeColor = TextColor;
                }

                return node;
            }

            /// <summary>
            /// Loop through all nodes in tree and reset back color to blank
            /// </summary>
            /// <param name="RadTree">RadTreeView Control To Use</param>
            public void ResetAllNodeColor(RadTreeView RadTree)
            {
                if (RadTree == null) { return; }

                if (RadTree.InvokeRequired)  // if currently on a different thread, invoke 
                {
                    RadTree.BeginInvoke((MethodInvoker)delegate ()
                    {
                        // loop through all nodes in tree and reset back color to blank
                        List<RadTreeNode> nodes = AllNodes(RadTree);
                        if (nodes != null)
                        {
                            foreach (RadTreeNode node in nodes)
                            {
                                node.BackColor = Color.Empty;
                            }
                        }

                    });

                }
                else
                {
                    // loop through all nodes in tree and reset back color to blank
                    List<RadTreeNode> nodes = AllNodes(RadTree);
                    if (nodes != null)
                    {
                        foreach (RadTreeNode node in nodes)
                        {
                            node.BackColor = Color.Empty;
                        }
                    }
                }
            }


            /// <summary>
            /// Check All Nodes in RadTreeView
            /// </summary>
            /// <param name="RadTree"></param>
            public void CheckAll(RadTreeView RadTree)
            {
                if (RadTree == null) { return; }
                foreach (RadTreeNode node in RadTree.Nodes) { node.Checked = true; }
            }

            /// <summary>
            /// UnCheck All Nodes in RadTreeView
            /// </summary>
            /// <param name="RadTree"></param>
            public void UnCheckAll(RadTreeView RadTree)
            {
                if (RadTree == null) { return; }
                foreach (RadTreeNode node in RadTree.Nodes) { node.Checked = false; }
            }


            #endregion


            #endregion

        }

    }
}

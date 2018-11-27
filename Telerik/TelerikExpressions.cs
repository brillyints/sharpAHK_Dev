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
using System.Web;
using System.Configuration;
using System.Net.Mail;
using Telerik.WinControls.UI;
using Telerik.WinForms.Documents.Model.Code;
using System.Collections;
using ScintillaNET;
using sharpAHK_Dev;
using TelerikExpressions;
using Telerik.WinControls;



namespace TelerikExpressions
{
    public static class ScintillaExp
    {

        /// <summary>Enable syntax highlighting and default start values</summary>
        /// <param name="sci">Scintilla Control Name</param>
        /// <param name="Ver">Configuration Version</param>
        public static void Setup(this Scintilla Sci, int Ver = 1, string KeyWords = "")
        {
            _ScintillaControl sci = new _ScintillaControl();
            sci.SetupScintilla(Sci, Ver, KeyWords);
        }

        /// <summary>Populate Scintilla Control from List, From Any Thread</summary>
        /// <param name="sci">Scintilla Control Name</param>
        /// <param name="list">List To Populate Scintilla Control</param>
        public static void LoadList(this Scintilla Sci, List<string> ListToLoad)
        {
            _ScintillaControl sci = new _ScintillaControl();
            sci.LoadList(Sci, ListToLoad); 
        }

        /// <summary>Update Scintilla Control (From Any Thread)</summary>
        /// <param name="sci">Scintilla Control Name</param>
        /// <param name="Text"> </param>
        public static void UpdateText(this Scintilla Sci, string Text = "")
        {
            _ScintillaControl sci = new _ScintillaControl();
            sci.UpdateScintilla(Sci, Text);
        }


        /// <summary>Scintilla - Read File and Populate Control</summary>
        /// <param name="sci">Scintilla Control Name</param>
        /// <param name="FileName"> </param>
        public static void OpenFile(this Scintilla Sci, string FileName)
        {
            _AHK ahk = new _AHK();
            _ScintillaControl sci = new _ScintillaControl();

            if (File.Exists(FileName))
            {
                string Code = ahk.FileRead(FileName);
                sci.UpdateScintilla(Sci, Code);
            }
        }

        /// <summary>
        /// Windows File Prompt To Select File to Load in NotePad
        /// </summary>
        /// <param name="sci"></param>
        public static void OpenFilePrompt(this Scintilla Sci)
        {
            _ScintillaControl sci = new _ScintillaControl();
            sci.OpenFilePrompt(Sci);
        }

        /// <summary>Highlight a specific search word in the scintilla control</summary>
        /// <param name="sci">Scintilla Control Name</param>
        /// <param name="text">Word To Highlight in Scintilla Control</param>
        /// <param name="color">Color To Change Word To</param>
        /// <param name="ClearHighlights">Option To Clear Previous Highlighting Before Adding New</param>
        public static void HighlightWord(this Scintilla Sci, string Word, Color color, bool ClearHighlights = true)
        {
            _ScintillaControl sci = new _ScintillaControl();
            sci.HighlightWord(Sci, Word, color, ClearHighlights);
        }

        /// <summary>Highlight List of Words in Scintilla Control</summary>
        /// <param name="sci">Scintilla Control Name</param>
        /// <param name="WordList">List of Words To Highlight</param>
        /// <param name="color">Color To Change List Words To</param>
        /// <param name="ClearHighlights">Option To Clear Previous Highlighting Before Adding New</param>
        public static void HighlightWordList(this Scintilla Sci, List<string> WordList, Color color, bool ClearHighlights = true)
        {
            _ScintillaControl sci = new _ScintillaControl();
            sci.HighlightWordList(Sci, WordList, color, ClearHighlights);
        }

        /// <summary>Clear Highlights From Scintilla Control</summary>
        /// <param name="sci">Scintilla Control Name</param>
        public static void ClearHighlights(this Scintilla Sci)
        {
            _ScintillaControl sci = new _ScintillaControl();
            // Remove all uses of our indicator
            sci.ClearHighlights(Sci);
        }


        /// <summary>Fold Text Brackets</summary>
        /// <param name="sci">Scintilla Control Name</param>
        public static void Fold(this Scintilla Sci)
        {
            _ScintillaControl sci = new _ScintillaControl();
            sci.FoldALL(Sci); 
        }

        /// <summary>Unfold All Bracked Code</summary>
        /// <param name="sci">Scintilla Control Name</param>
        public static void UnFold(this Scintilla Sci)
        {
            _ScintillaControl sci = new _ScintillaControl();
            sci.UnFold(Sci);
        }

        /// <summary>Returns current line number in control</summary>
        /// <param name="sci">Scintilla Control Name</param>
        public static int CurrentLinNum(this Scintilla Sci)
        {
            _ScintillaControl sci = new _ScintillaControl();
            return sci.CurrentLine(Sci);
        }

        /// <summary>Returns selected text in control</summary>
        /// <param name="sci">Scintilla Control Name</param>
        public static string SelectedText(this Scintilla Sci)
        {
            _ScintillaControl sci = new _ScintillaControl();
            return sci.SelectedText(Sci);
        }


        /// <summary>Enable WordWrap mode on Scintilla control</summary>
        /// <param name="sci">Scintilla Control Name</param>
        /// <param name="On">Enable WordWrap Mode</param>
        /// <param name="Opt">Opt = WhiteSpace | Char</param>
        public static void WordWrap(this Scintilla Sci, bool On = true, string Opt = "")
        {
            _ScintillaControl sci = new _ScintillaControl();
            sci.WordWrap(Sci, On, Opt);
        }


    }

    public static class RadExp
    {

        public static void UpdateGrid(this RadGridView radGrid, DataTable datasource)
        {
            if (radGrid == null) { return; }

            if (radGrid.InvokeRequired)  // if currently on a different thread, invoke 
            {
                radGrid.BeginInvoke((MethodInvoker)delegate () { radGrid.DataSource = datasource; });
            }
            else
            {
                radGrid.DataSource = datasource;
            }

            //tel.Fill_Column_Width(radGrid);
        }
        public static void UpdateLabel(this RadLabel lablel, string Text)
        {
            if (lablel == null) { return; }

            if (lablel.InvokeRequired)  // if currently on a different thread, invoke 
            {
                lablel.BeginInvoke((MethodInvoker)delegate () { lablel.Text = Text; });
            }
            else
            {
                lablel.Text = Text;
            }
        }
        public static void UpdateRTF(this RadRichTextEditor la, string Text)
        {
            if (la == null) { return; }

            if (la.InvokeRequired)  // if currently on a different thread, invoke 
            {
                la.BeginInvoke((MethodInvoker)delegate () { la.Text = Text; });
            }
            else
            {
                la.Text = Text;
            }
        }
        public static void UpdateButton(this RadButton la, string Text)
        {
            if (la == null) { return; }

            if (la.InvokeRequired)  // if currently on a different thread, invoke 
            {
                la.BeginInvoke((MethodInvoker)delegate () { la.Text = Text; });
            }
            else
            {
                la.Text = Text;
            }
        }
        public static void UpdatePanel(this RadCollapsiblePanel la, string Text)
        {
            if (la == null) { return; }

            if (la.InvokeRequired)  // if currently on a different thread, invoke 
            {
                la.BeginInvoke((MethodInvoker)delegate () { la.HeaderText = Text; });
            }
            else
            {
                la.HeaderText = Text;
            }
        }
        public static void UpdateTextbox(this Telerik.WinControls.UI.RadTextBox lablel, string Text)
        {
            if (lablel == null) { return; }

            if (lablel.InvokeRequired)  // if currently on a different thread, invoke 
            {
                lablel.BeginInvoke((MethodInvoker)delegate () { lablel.Text = Text; });
            }
            else
            {
                lablel.Text = Text;
            }
        }

        // Update Controls (Any Thread)

        public static void UpdateControl(this RadGridView radGrid, DataTable datasource)
        {
            if (radGrid.InvokeRequired)  // if currently on a different thread, invoke 
            {
                radGrid.BeginInvoke((MethodInvoker)delegate () { radGrid.DataSource = datasource; });
            }
            else
            {
                radGrid.DataSource = datasource;
            }

            //tel.Fill_Column_Width(radGrid);
        }
        public static void UpdateControl(this RadLabel lablel, string Text)
        {
            if (lablel.InvokeRequired)  // if currently on a different thread, invoke 
            {
                lablel.BeginInvoke((MethodInvoker)delegate () { lablel.Text = Text; });
            }
            else
            {
                lablel.Text = Text;
            }
        }
        public static void UpdateControl(this Telerik.WinControls.UI.RadTextBox lablel, string Text)
        {
            if (lablel.InvokeRequired)  // if currently on a different thread, invoke 
            {
                lablel.BeginInvoke((MethodInvoker)delegate () { lablel.Text = Text; });
            }
            else
            {
                lablel.Text = Text;
            }
        }

        public static void UpdateControl(this RadRichTextEditor la, string Text)
        {
            if (la.InvokeRequired)  // if currently on a different thread, invoke 
            {
                la.BeginInvoke((MethodInvoker)delegate () { la.Text = Text; });
            }
            else
            {
                la.Text = Text;
            }
        }
        public static void UpdateControl(this RadButton la, string Text)
        {
            if (la.InvokeRequired)  // if currently on a different thread, invoke 
            {
                la.BeginInvoke((MethodInvoker)delegate () { la.Text = Text; });
            }
            else
            {
                la.Text = Text;
            }
        }
        public static void UpdateControl(this RadCollapsiblePanel la, string Text)
        {
            if (la.InvokeRequired)  // if currently on a different thread, invoke 
            {
                la.BeginInvoke((MethodInvoker)delegate () { la.HeaderText = Text; });
            }
            else
            {
                la.HeaderText = Text;
            }
        }



        #region === RadChat ===

        /// <summary>
        /// Clears RadChat Conversation
        /// </summary>
        /// <param name="radchat"></param>
        public static void Clear(this RadChat radchat)
        {
            _TelerikLib._RadChat ct = new _TelerikLib._RadChat();
            ct.Clear(radchat);
        }

        /// <summary>
        /// Scroll to Bottom of RadChat Conversation
        /// </summary>
        /// <param name="radchat">RadChat Control</param>
        public static void ScrollToBottom(this RadChat radchat)
        {
            _TelerikLib._RadChat ct = new _TelerikLib._RadChat();
            ct.ScrollToBottom(radchat);
        }


        /// <summary>
        /// Scroll to Top of RadChat Conversation
        /// </summary>
        /// <param name="radchat">RadChat Control</param>
        public static void ScrollToTop(this RadChat radchat)
        {
            _TelerikLib._RadChat ct = new _TelerikLib._RadChat();
            ct.ScrollToTop(radchat);
        }


        #endregion


    }
    public static class RadTreeExp
    {
        /// <summary>
        /// Clear Contents of TreeView (From Any Thread)
        /// </summary>
        /// <param name="RadTree">RadTreeView Control To Clear</param>
        public static void ClearTree(this RadTreeView RadTree)
        {
            sharpAHK_Dev._TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            tree.ClearTree(RadTree);
        }

        /// <summary>
        /// Save RadTreeView Contents to Local XML File 
        /// </summary>
        /// <param name="RadTree"></param>
        /// <param name="SaveName"></param>
        public static void SaveTree(this RadTreeView RadTree, string SaveName = "RadTreeXML")
        {
            sharpAHK_Dev._TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            tree.SaveTree(RadTree, SaveName);
        }

        /// <summary>
        /// Restore RadTreeView Contents from Local XML File 
        /// </summary>
        /// <param name="RadTree"></param>
        /// <param name="SaveName"></param>
        public static void LoadTree(this RadTreeView RadTree, string SaveName = "RadTreeXML")
        {
            sharpAHK_Dev._TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            tree.LoadTree(RadTree, SaveName);
        }

        // Create New XML File and Load In Tree For Editing
        public static void New_TreeXML(this RadTreeView RadTree, string NewName = "RadTreeXML")
        {
            sharpAHK_Dev._TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            tree.New_TreeXML(RadTree, NewName);
        }


        /// <summary>
        /// Check All Nodes in RadTreeView
        /// </summary>
        /// <param name="RadTree"></param>
        public static void CheckAll(this RadTreeView RadTree)
        {
            if (RadTree == null) { return; }
            foreach (RadTreeNode node in RadTree.Nodes) { node.Checked = true; }
        }

        /// <summary>
        /// UnCheck All Nodes in RadTreeView
        /// </summary>
        /// <param name="RadTree"></param>
        public static void UnCheckAll(this RadTreeView RadTree)
        {
            if (RadTree == null) { return; }
            foreach (RadTreeNode node in RadTree.Nodes) { node.Checked = false; }
        }


        //===== NODES =================

        // Add New Node to TreeView Using Text (From Any Thread)
        public static RadTreeNode NewNode(this RadTreeView radTree, string NodeText = "", string NodeTag = "")
        {
            sharpAHK_Dev._TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            return tree.NewNode(radTree, NodeText, NodeTag);
        }

        /// <summary>
        /// Add TreeNode to TreeView (From Any Thread) 
        /// </summary>
        /// <param name="radTree"></param>
        /// <param name="node"></param>
        public static void AddNode(this RadTreeView radTree, RadTreeNode node)
        {
            sharpAHK_Dev._TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            tree.AddNode(radTree, node);
        }

        public static void AddSubNode(this RadTreeNode existingNode, RadTreeNode newNode)
        {
            sharpAHK_Dev._TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            tree.AddSubNode(existingNode, newNode);
        }

        /// <summary>
        /// Search RadTreeView Nodes for Matching Text, Returns Node if Found
        /// </summary>
        /// <param name="radTree">RadTreeView To Search Nodes</param>
        /// <param name="SearchText">Text Found in Node Tag to Search For</param>
        /// <returns></returns>
        public static RadTreeNode FindNode(this RadTreeView radTree, string SearchText)
        {
            sharpAHK_Dev._TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            return tree.ReturnNode(radTree, SearchText);
        }


        /// <summary>
        /// Search RadTreeView Nodes for Matching Node Tag Text, Returns Node if Found. Option to Find Nodes Only Containing SearchText
        /// </summary>
        /// <param name="radTree">RadTreeView To Search Nodes</param>
        /// <param name="SearchText">Text Found in Node Tag to Search For</param>
        /// <param name="ExactMatch">If False, will return nodes containing SearchText</param>
        /// <returns></returns>
        public static RadTreeNode FindNodeByTag(this RadTreeView radTree, string SearchText, bool ExactMatch = true)
        {
            sharpAHK_Dev._TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            return tree.ReturnNode(radTree, "", SearchText, true);
        }


        /// <summary>
        /// Returns XML From RadTree (From Any Thread)
        /// </summary>
        /// <param name="radTree"></param>
        /// <returns></returns>
        public static object NodeXML(this RadTreeView radTree)
        {
            sharpAHK_Dev._TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            return NodeXML(radTree);
        }


        /// <summary>
        /// Load Previously Saved XML into RadTreeView
        /// </summary>
        /// <param name="radTree"></param>
        /// <param name="xml"></param>
        public static void LoadNodeXML(this RadTreeView radTree, string xml)
        {
            sharpAHK_Dev._TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            tree.LoadNodeXML(radTree, xml);
        }


        public static void FileTree(this RadTreeView radTree, string DirPath, string DisplayName = "DirName")
        {
            sharpAHK_Dev._TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            tree.RadTree_Files(radTree, DirPath);
        }



        /// <summary>
        /// Loop Through Tree Nodes, Returns String with NODE TAG Text on All Checked Items
        /// </summary>
        /// <param name="radTree"></param>
        /// <returns></returns>
        public static string Checked_NodeTagText(this RadTreeView radTree)
        {
            sharpAHK_Dev._TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            return tree.Checked_NodeTagText(radTree);
        }



        /// <summary>
        /// Return List of Nodes From Specific Level # 
        /// </summary>
        /// <param name="radTree">RadTreeView Control To Use</param>
        /// <param name="Level">Node Level Number (Starting with 0)</param>
        /// <returns></returns>
        public static List<RadTreeNode> NodesByLevel(this RadTreeView radTree, int Level = 0)
        {
            sharpAHK_Dev._TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            return tree.NodesByLevel(radTree, Level);
        }

        /// <summary>
        /// Returns List of All Nodes in RadTree
        /// </summary>
        /// <param name="radTree">RadTreeView Control To Use</param>
        /// <returns></returns>
        public static List<RadTreeNode> AllNodes(this RadTreeView radTree)
        {
            sharpAHK_Dev._TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            return tree.AllNodes(radTree);
        }

        /// <summary>
        /// Loop through all nodes in tree and reset back color to blank
        /// </summary>
        /// <param name="radTree">RadTreeView Control To Use</param>
        public static void ResetAllNodeColor(this RadTreeView radTree)
        {
            sharpAHK_Dev._TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            tree.ResetAllNodeColor(radTree);
        }


        /// <summary>
        /// Add List of Items to Existing Node
        /// </summary>
        /// <param name="Items"></param>
        /// <param name="parent"></param>
        /// <param name="RadTree"></param>
        /// <param name="Tag"></param>
        public static void ToRadTree(this List<string> Items, Telerik.WinControls.UI.RadTreeView RadTree, string Tag = "", string ParentNodeText = "")
        {
            sharpAHK_Dev._TelerikLib.RadTree tree = new _TelerikLib.RadTree();
            tree.ListRadTree(RadTree, Items, Tag, ParentNodeText);
        }



    }




}

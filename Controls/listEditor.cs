using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using AHKExpressions;
using sharpAHK;
using sharpAHK_Dev;
using TelerikExpressions;
using Telerik.WinControls.UI;

namespace sharpAHK_Dev.Controls
{
    public partial class listEditor : Telerik.WinControls.UI.RadForm
    {
        #region === Startup ===

        public listEditor(string ListName = "")
        {
            InitializeComponent();
            txtListName.Text = ListName;
        }

        public void sb(string Text = "")
        {
            radLabelElement1.Text = Text;
        }

        #endregion

        private void btnAddListItem_Click(object sender, EventArgs e)
        {
            _AHK ahk = new _AHK();
            _Database.SQLite sqlite = new _Database.SQLite();
            _Lists lst = new _Lists();

            sqlite.List(txtListName.Text, txtListItem.Text, false);
            sb("Added Item to " + txtListName.Text + " List");
        }

        private void btnUpdateListItem_Click(object sender, EventArgs e)
        {
            _AHK ahk = new _AHK();
            _Database.SQLite sqlite = new _Database.SQLite();
            _Lists lst = new _Lists();
        }

        private void btnDeleteList_Click(object sender, EventArgs e)
        {
            _AHK ahk = new _AHK();
            _Database.SQLite sqlite = new _Database.SQLite();
            _Lists lst = new _Lists();



            sqlite.RemoveList(txtListName.Text);
        }

        private void btnDisplayLists_Click(object sender, EventArgs e)
        {
            _AHK ahk = new _AHK();
            _Database.SQLite sqlite = new _Database.SQLite();
            _Lists lst = new _Lists();

            List<string> ListNames = sqlite.ListNames();

            radTreeView1.ClearTree();

            if (ListNames != null && ListNames.Count > 0)
            {
                foreach (string listName in ListNames)
                {
                    RadTreeNode node = new RadTreeNode();
                    node.Text = listName; node.Tag = "ListName|" + listName;

                    List<string> listItems = sqlite.ListValues(listName, _Database.SQLite.ListOrder.ListValue);
                    foreach (string item in listItems)
                    {
                        RadTreeNode snode = new RadTreeNode();
                        snode.Text = item; node.Tag = "ListName|" + listName + "|" + item;
                        node.Nodes.Add(snode);
                    }

                    radTreeView1.Nodes.Add(node);
                }
            }


            sb("Loaded " + ListNames.Count + " Lists");
        }

        private void radTreeView1_NodeMouseClick(object sender, RadTreeViewEventArgs e)
        {
            RadTreeNode clicked = e.Node;
            if (clicked == null) { return; }  // do nothing if node value blank

            string ListName = "";

            // grab node text and tag (if populated)
            string text = ""; string tag = "";
            try { text = clicked.Text; } catch { }
            if (clicked.Tag != null) { try { tag = clicked.Tag.ToString(); } catch { } }


            if (clicked.Level == 0) { ListName = Text; }
            if (clicked.Level > 0) { ListName = clicked.Parent.Text; }

            txtListName.Text = ListName;
            txtListItem.Text = text;

        }

        private void radTreeView1_NodeMouseDoubleClick(object sender, RadTreeViewEventArgs e)
        {
            RadTreeNode clicked = e.Node;
            if (clicked == null) { return; }  // do nothing if node value blank

            // grab node text and tag (if populated)
            string text = ""; string tag = "";
            try { text = clicked.Text; } catch { }
            if (clicked.Tag != null) { try { tag = clicked.Tag.ToString(); } catch { } }


        }

        private void btnNewList_Click(object sender, EventArgs e)
        {
            txtListName.Text = "";
            txtListItem.Text = "";
            sb("New List");
        }
    }
}

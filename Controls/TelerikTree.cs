using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Telerik.WinControls.UI;
using Telerik.WinControls;
using AHKExpressions;
using sharpAHK;
using sharpAHK_Dev;
using Telerik;
using TelerikExpressions;
using System.Data.SqlClient;
using System.Threading;
using System.Text.RegularExpressions;

namespace sharpAHK_Dev
{
    public partial class TelerikTree : UserControl
    {
        _Database.SQLite sqlite = new _Database.SQLite();
        _AHK ahk = new _AHK();
        _TelerikLib._RadMenu menu = new _TelerikLib._RadMenu();
        _TelerikLib.RadTree tree = new _TelerikLib.RadTree();

        public TelerikTree()
        {
            InitializeComponent();
            sb("Started");

            SetupRadTree(radTreeView1);
            AddTreeMenu();

            Create_RadPage_ContextMenu();

            // enable text filtering for nodes
            radTreeView1.TreeViewElement.FilterPredicate = FilterNode;

            _RadTREE = radTreeView1;

            if (!_ShowStatusBar) { radStatusStrip1.Visible = false; }
            if (!_ShowProgressBar) { radProgressBar1.Visible = false; }
        }

            public RadTreeView _RadTREE { get; set; }
            public string _FilePath { get; set; }
            public string _FileName { get; set; }
            public string _NodeName { get; set; }
            public RadTreeNode _parentNode { get; set; }
            public RadTreeNode _thisNode { get; set; }

            public static RadPageView _RadPage { get; set; }    
            public static RadContextMenu _RadPageContextMenu { get; set; }

            public bool _ShowStatusBar { get; set; }
            public bool _ShowProgressBar { get; set; }


        #region === GUI ===


        #region === RadStatusBar ===

        /// <summary>
        /// Populate StatusBar with Text
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Text2"></param>
        public void sb(string Text, string Text2 = "")
        {
            if (radStatusStrip1 != null)
            {
                if (radStatusStrip1.Items.Count > 0)
                {
                    try { (radStatusStrip1.Items[0] as RadLabelElement).Text = Text; } catch { }
                }
                if (radStatusStrip1.Items.Count > 1)
                {
                    try { (radStatusStrip1.Items[1] as RadLabelElement).Text = Text2; } catch { }
                }
            }
        }

        #endregion


        #region === Rad Pages ===

        private void radPageView1_SelectedPageChanged(object sender, EventArgs e)
        {
            sb("Page Clicked");
        }


        public void AddPage(RadPageView RadPageContainer, string HeaderText = "NewPage")
        {
            RadPageViewPage pageOne = new RadPageViewPage();
            pageOne.Text = HeaderText;
            RadPageContainer.Pages.Add(pageOne);
        }

        public void AddButton(RadPageViewPage RadPage, string ButtonText = "New Button")
        {
            RadButton button = new RadButton();
            button.Text = ButtonText;
            RadPage.Controls.Add(button);
        }

        public void SelectPage(int PageNum = 1)
        {
            radPageView1.SelectedPage = radPageView1.Pages[PageNum];
        }





        public void Create_RadPage_ContextMenu()
        {
            _RadPage = radPageView1;
            RadContextMenu contextMenu = new RadContextMenu();

            _RadPageContextMenu = contextMenu;

            //contextMenu = new RadContextMenu();

            RadMenuItem addNewTabMenuItem = new RadMenuItem();
            addNewTabMenuItem.Text = "Add New Tab";
            addNewTabMenuItem.Click += new EventHandler(addNewTabMenuItem_Click);
            _RadPageContextMenu.Items.Add(addNewTabMenuItem);
            RadMenuSeparatorItem separator = new RadMenuSeparatorItem();
            _RadPageContextMenu.Items.Add(separator);
            RadMenuItem closeTabMenuItem = new RadMenuItem();
            closeTabMenuItem.Text = "Close Tab";
            closeTabMenuItem.Click += new EventHandler(closeTabMenuItem_Click);
            _RadPageContextMenu.Items.Add(closeTabMenuItem);
            RadMenuItem closeAllButThisMenuItem = new RadMenuItem();
            closeAllButThisMenuItem.Text = "Close All But This";
            closeAllButThisMenuItem.Click += new EventHandler(closeAllButThisMenuItem_Click);
            _RadPageContextMenu.Items.Add(closeAllButThisMenuItem);
            RadMenuItem closeAllTabsMenuItem = new RadMenuItem();
            closeAllTabsMenuItem.Text = "Close All Tabs";
            closeAllTabsMenuItem.Click += new EventHandler(closeAllTabsMenuItem_Click);
            _RadPageContextMenu.Items.Add(closeAllTabsMenuItem);

            _RadPage.MouseClick += new MouseEventHandler(radPageView1_MouseClick);
            //_RadPage.MouseDoubleClick += new MouseEventHandler(radPageViewPage1_MouseDoubleClick);
        }

        public void addNewTabMenuItem_Click(object sender, EventArgs e)
        {
            RadPageViewPage newPage = new RadPageViewPage();
            newPage.Text = "My new tab text";
            _RadPage.Pages.Add(newPage);
        }
        public void closeTabMenuItem_Click(object sender, EventArgs e)
        {
            _RadPage.Pages.Remove(_RadPage.SelectedPage);
        }
        public void closeAllButThisMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = _RadPage.Pages.Count - 1; i >= 0; i--)
            {
                if (_RadPage.Pages[i] != _RadPage.SelectedPage)
                {
                    _RadPage.Pages.RemoveAt(i);
                }
            }
        }
        public void closeAllTabsMenuItem_Click(object sender, EventArgs e)
        {
            _RadPage.Pages.Clear();
        }

        public void radPageView1_MouseClick(object sender, MouseEventArgs e)
        {
            RadPageViewItem hitItem = _RadPage.ViewElement.ItemFromPoint(e.Location);
            if (e.Button == MouseButtons.Right && hitItem != null)
            {
                _RadPageContextMenu.Show(_RadPage.PointToScreen(e.Location));
            }
        }




        #endregion


        #region === RadTreeView ===

        // Build Menu with RadTree Basic Actions
        public void AddTreeMenu()
        {
            _TelerikLib._RadMenu menu = new _TelerikLib._RadMenu(); 
            menu.RadTreeMenu_Basics(RadTreeMenu_Basics_Click, radMenu1);
        }

        // Configure RadTree Options - Use in Form Startup!
        public void SetupRadTree(RadTreeView RadTree, bool AddCheckboxes = false)
        {
            RadTree.SelectedNodeChanged += new Telerik.WinControls.UI.RadTreeView.RadTreeViewEventHandler(RadTree_SelectedNodeChanged);

            RadTree.NodeAdded += new Telerik.WinControls.UI.RadTreeView.RadTreeViewEventHandler(RadTree_NodeAdded);
            RadTree.NodeAdding += new Telerik.WinControls.UI.RadTreeView.RadTreeViewCancelEventHandler(RadTree_NodeAdding);

            RadTree.NodeRemoved += new Telerik.WinControls.UI.RadTreeView.RadTreeViewEventHandler(RadTree_NodeRemoved);
            RadTree.NodeRemoving += new Telerik.WinControls.UI.RadTreeView.RadTreeViewCancelEventHandler(RadTree_NodeRemoving);


            // used to set initial text when going into edit mode (starts blank otherwise)
            RadTree.EditorInitialized += new Telerik.WinControls.UI.TreeNodeEditorInitializedEventHandler(RadTree_EditorInitialized);

            // events that fire after node text edited
            RadTree.ValueChanged += new Telerik.WinControls.UI.TreeNodeValueChangedEventHandler(RadTree_ValueChanged);
            RadTree.ValueValidating += new Telerik.WinControls.UI.TreeNodeValidatingEventHandler(RadTree_ValueValidating);

            if (AddCheckboxes)
            {
                // When TriStateMode is set to true, the CheckBoxes property is also set to true automatically.
                RadTree.TriStateMode = true;

                //To programmatically set the state when TriStateMode is true assign the CheckState property one of the ToggleState enumeration values.
                if (RadTree.GetNodeCount(true) > 0)
                {
                    RadTree.SelectedNode.CheckState = Telerik.WinControls.Enumerations.ToggleState.Indeterminate;
                }
            }

            // Display TreeView Properties in Property Viewer Control
            //radPropertyGrid1.ToolbarVisible = true;  // allows user to filter display options by typing top bar
            //radPropertyGrid1.SelectedObject = RadTree;

            // assigns node display format options while loading new content
            RadTree.NodeFormatting += new Telerik.WinControls.UI.TreeNodeFormattingEventHandler(RadTree_NodeFormatting);

            //======== Drag Drop ===================
            RadTree.AllowDragDrop = true;
            RadTree.MultiSelect = true;
            RadTree.AllowDrop = true;

            RadTree.SpacingBetweenNodes = 2;  // default = -1

            // Enable and Define Drag / Drop behavior
            EnableDragDrop(RadTree);

            //====== Context Menu ==========================

            //// Create new Context menu
            //RadContextMenu radContextMenu1 = new Telerik.WinControls.UI.RadContextMenu(this.components);

            // Assign Custom Context Menu 
            //RadTree.RadContextMenu = radContextMenu1;

            // Enable Default Context Menu Options
            RadTree.AllowDefaultContextMenu = true;

            // Available Context Actions
            RadTree.AllowAdd = true;
            RadTree.AllowEdit = true;
            RadTree.AllowRemove = true;


            // assign action to fire when context menu is opening (option to filter available menu opts)
            RadTree.ContextMenuOpening += new Telerik.WinControls.UI.TreeViewContextMenuOpeningEventHandler(RadTree_ContextMenuOpening);


            // TOOLTIPS: Enable Tooltips over Nodes
            RadTree.ScreenTipNeeded += new Telerik.WinControls.ScreenTipNeededEventHandler(RadTree_ScreenTipNeeded);


            //===== Keyboard ====================
            RadTree.KeyboardSearchEnabled = true;  // allows user to jump to node starting with typed text
            RadTree.TreeViewElement.KeyboardSearchResetInterval = 500;  // used to determin what time between keystrokes will be considered as typing


            RadTree.EnableKineticScrolling = false;  // allows user to scroll while holding a node down  (doesn't work well when dragging is enabled)


            // enable RadTreeView Node Filter
            //radTreeOtherFilms.TreeViewElement.FilterPredicate = FilterNode;

            //RadTree.Font = new System.Drawing.Font("Segoe UI Black", 11F);

            sb("RadTree Configured");

            AddTreeMenu();
        }


        public void RadTreeMenu_Basics_Click(object sender, EventArgs e)
        {
            RadMenuItem item = (sender as RadMenuItem);

            RadTreeActions(radTreeView1, item.Text);

            sb("RadTree: " + item.Text);
        }


        public void RadTreeActions(RadTreeView radTree, string Action)
        {


            string clicked = Action;

            if (clicked == "Load Tree" || clicked == "RadCode")
            {
                sqlite.Setting("LastXML", clicked);
                radTree.LoadTree("RadTreeXML");
            }
            if (clicked == "Snippets") { radTree.LoadTree("Snippets"); sqlite.Setting("LastXML", clicked); }
            if (clicked == "Movies") { radTree.LoadTree("Movies"); sqlite.Setting("LastXML", clicked); }
            if (clicked == "TV Drives") { radTree.LoadTree("TVDrives"); sqlite.Setting("LastXML", clicked); }
            if (clicked == "ADB") { radTree.LoadTree("ADB"); sqlite.Setting("LastXML", clicked); }
            if (clicked == "Rad_ASPX") { radTree.LoadTree("Rad_ASPX"); sqlite.Setting("LastXML", clicked); }
            if (clicked == "Rad_WinForms") { radTree.LoadTree("Rad_WinForms"); sqlite.Setting("LastXML", clicked); }
            if (clicked == "SharpAHK_Dev") { radTree.LoadTree("SharpAHK_Dev"); sqlite.Setting("LastXML", clicked); }
            if (clicked == "SharpAHK") { radTree.LoadTree("SharpAHK"); sqlite.Setting("LastXML", clicked); }
            if (clicked == "LikelyLad") { radTree.LoadTree("LikelyLad"); sqlite.Setting("LastXML", clicked); }

            if (clicked == "Current TreeTag")
            {
                if (radTreeView1.Tag != null)
                {
                    string tag = radTreeView1.Tag.ToString();
                    ahk.MsgBox("Current XML = " + tag);
                }
            }

            if (clicked == "Open XML Dir") { ahk.OpenDir(ahk.AppDir() + "\\RadTreeXML"); }

            if (clicked == "New Tree") { tree.New_TreeXML(radTree); }

            if (clicked == "Save As")
            {
                string NewName = ahk.InputBox("Save Tree As", "Save Tree XML Name");
                if (NewName != "")
                {
                    tree.SaveTree(radTree, NewName);

                    // Repopulate Menu with XML Files after Saving New
                    menu.RadMenu_FolderFiles(ahk.AppDir() + "\\RadTreeXML", RadMenu_XMLDir_Click, "*.xml", radMenu1);

                    sqlite.Setting("LastXML", NewName);
                    sb("Saved Tree XML: " + NewName);
                    return;
                }

            }

            if (clicked == "Sort Nodes Asc")
            {
                //radMenuNoSort.IsChecked = false;
                //radMenuSortAsc.IsChecked = true;
                //radMenuSortDesc.IsChecked = false;
                radTree.SortOrder = System.Windows.Forms.SortOrder.Ascending;
                sb("Sorting Nodes");
            }
            if (clicked == "Sort Nodes Desc")
            {
                //radMenuNoSort.IsChecked = false;
                //radMenuSortAsc.IsChecked = false;
                //radMenuSortDesc.IsChecked = true;
                radTree.SortOrder = System.Windows.Forms.SortOrder.Descending;
                sb("Sorting Nodes");
            }
            if (clicked == "No Sort")
            {
                //radMenuNoSort.IsChecked = true;
                //radMenuSortAsc.IsChecked = false;
                //radMenuSortDesc.IsChecked = false;
                radTree.SortOrder = System.Windows.Forms.SortOrder.None;
                sb("Sorting Nodes");
            }

            if (clicked == "Save Tree")
            {
                // pull name of current xml file from tree
                string tag = radTreeView1.Tag.ToString();
                tag = tag.Replace("XML:", "");

                // if no tag has been added to tree, use 'save as' prompt for new name
                if (tag.Trim() == "")
                {
                    RadTreeActions(radTree, "Save As");
                    return;
                }

                //radTree.SaveTree();
                tree.SaveTree(radTree, tag);
            }
            if (clicked == "Clear Tree") { radTree.ClearTree(); }
            if (clicked == "Add Node")
            {
                RadTreeNode Node1 = new RadTreeNode("NewNode");
                Node1.Tag = "";
                //Node1.BackColor = Color.Blue;
                radTree.Nodes.Add(Node1);
            }

            if (clicked == "Expand Nodes") { radTree.ExpandAll(); }
            if (clicked == "Collapse Nodes") { radTree.CollapseAll(); }

            if (clicked == "Checked Node Tags") { ahk.MsgBox(radTree.Checked_NodeTagText()); }

        }

        // Load Saved XML Tree Content into RadTree
        void RadMenu_XMLDir_Click(object sender, EventArgs e)
        {
            RadMenuItem item = (sender as RadMenuItem);

            radTreeView1.LoadTree(item.Text); sqlite.Setting("LastXML", item.Text);

            sb("Loaded XML: " + item.Text);
        }




        #region === RadTreeView Events ===

        //  === Tree Events ===

        private void radTreeView1_NodeMouseClick(object sender, Telerik.WinControls.UI.RadTreeViewEventArgs e)
        {
            RadTreeView tree = e.TreeView;
            if (tree.SelectedNode == null) { return; }

            string Text = ""; string Tag = "";
            try { Text = tree.SelectedNode.Text; } catch { }
            try { Tag = tree.SelectedNode.Tag.ToString(); } catch { }

            try
            {
                //txtNodeLevel.Text = e.Node.Level.ToString();
                //txtNodeName.Text = e.Node.Name;
                //txtNodeTag.Text = Tag;
                //txtNodeText.Text = Text;
                //txtNodeParentText.Text = e.Node.Parent.Text;
            }
            catch
            {

            }

            sb("Clicked: " + Text + " | " + Tag);
        }

        private void radTreeView1_NodeMouseDoubleClick(object sender, Telerik.WinControls.UI.RadTreeViewEventArgs e)
        {
            RadTreeView tree = e.TreeView;
            if (tree.SelectedNode == null) { return; }

            string Text = ""; string Tag = "";
            try { Text = tree.SelectedNode.Text; } catch { }
            try { Tag = tree.SelectedNode.Tag.ToString(); } catch { }

            sb("DoubleClicked: " + Text + " | " + Tag);
        }




        public RadTreeNode selectedNode = new RadTreeNode();

        private void RadTree_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {
            if (e.Node == null) { return; }

            RadTreeView RadTree = e.Node.TreeView;
            selectedNode = e.Node;
            string nodeDisplay = "";

        }

        string RadTreeMode = "SQLTree";

        void RadTree_NodeAdded(object sender, RadTreeViewEventArgs e)
        {
            if (e.Node.Text != null)
            {
                sb("Node Added: " + e.Node.Text);
            }
        }

        void RadTree_NodeAdding(object sender, RadTreeViewCancelEventArgs e)
        {
            if (e.Node.Text != null)
            {
                if (e.Node.Text.Contains("Non-insertable"))
                {
                    e.Cancel = true;
                }
            }
        }


        private void RadTree_EditorInitialized(object sender, TreeNodeEditorInitializedEventArgs e)
        {
            // Populate Previous Value in Edit Field Upon Entering Edit Mode
            e.Editor.Value = e.Node.Text;
        }

        public void RadTree_NodeRemoved(object sender, RadTreeViewEventArgs e)
        {
            sb("Removed Node: " + e.Node.Text);
        }

        public void RadTree_NodeRemoving(object sender, RadTreeViewCancelEventArgs e)
        {
            //if (e.Node.Level == 0)  // Disable Removing Root Node
            //{
            //    e.Cancel = true;
            //}
        }

        private void RadTree_ValueChanged(object sender, TreeNodeValueChangedEventArgs e)
        {
            sb("Changed: " + e.Node.Text);
        }

        private void RadTree_ValueValidating(object sender, TreeNodeValidatingEventArgs e)
        {
            RadTreeViewElement parent = (RadTreeViewElement)sender;

            parent.ActiveEditor.Value = "Validating";
        }

        // Setup ToolTips for Tree
        RadOffice2007ScreenTipElement screenTip = new RadOffice2007ScreenTipElement();
        Size size = new Size(120, 70);
        Padding pad = new Padding(2);

        // ToolTip for Tree Action
        private void RadTree_ScreenTipNeeded(object sender, Telerik.WinControls.ScreenTipNeededEventArgs e)
        {
            TreeNodeElement node = e.Item as TreeNodeElement;
            if (node != null)
            {
                screenTip.MainTextLabel.Image = node.ImageElement.Image;
                screenTip.MainTextLabel.TextImageRelation = TextImageRelation.ImageBeforeText;
                screenTip.MainTextLabel.Padding = pad;
                screenTip.MainTextLabel.Text = "This is " + node.ContentElement.Text;
                screenTip.MainTextLabel.Margin = new System.Windows.Forms.Padding(10);
                screenTip.CaptionLabel.Padding = pad;
                screenTip.CaptionLabel.Text = node.ContentElement.Text;
                screenTip.EnableCustomSize = true;
                screenTip.AutoSize = false;
                screenTip.Size = size;
                node.ScreenTip = this.screenTip;
            }
        }


        private void radTreeView1_ToolTipTextNeeded(object sender, Telerik.WinControls.ToolTipTextNeededEventArgs e)
        {
            
        }


        private void RadTree_NodeFormatting(object sender, TreeNodeFormattingEventArgs e)
        {
            if (e.Node.Level == 0)
            {
                //Image IconImage = img.To_Image(@"C:\_Code\LucidProjects\IcoLib\folders.png");
                //e.NodeElement.ImageElement.Image = IconImage;
            }
            if (e.Node.Level > 0)
            {
                //e.NodeElement.BorderColor = Color.Blue;
                //e.NodeElement.BorderBoxStyle = Telerik.WinControls.BorderBoxStyle.SingleBorder;
                //e.NodeElement.BorderGradientStyle = Telerik.WinControls.GradientStyles.Solid;
                //e.NodeElement.BackColor = Color.LightBlue;
                //e.NodeElement.ContentElement.ForeColor = Color.Black;

                // add image
                //Image IconImage = img.To_Image(@"C:\_Code\LucidProjects\IcoLib\tumblr.ico");
                //e.NodeElement.ImageElement.Image = IconImage; 
            }
            else
            {
                //e.NodeElement.ResetValue(LightVisualElement.BorderColorProperty, ValueResetFlags.Local);
                //e.NodeElement.ResetValue(LightVisualElement.BorderBoxStyleProperty, ValueResetFlags.Local);
                //e.NodeElement.ResetValue(LightVisualElement.BorderGradientStyleProperty, ValueResetFlags.Local);
                //e.NodeElement.ResetValue(LightVisualElement.BackColorProperty, ValueResetFlags.Local);
                //e.NodeElement.ContentElement.ResetValue(LightVisualElement.ForeColorProperty, ValueResetFlags.Local);
                //e.NodeElement.ImageElement.ResetValue(LightVisualElement.ImageProperty, ValueResetFlags.Local);
            }
        }


        // Tree Context Menu Options + Public Selected Node Info

        private void RadTree_ContextMenuOpening(object sender, TreeViewContextMenuOpeningEventArgs e)
        {
            // Remove Unwanted Options from Built in Context Menu Options
            List<string> ExcludeList = new List<string> { "New", "Delete", "Cut", "Copy" };

            for (int i = e.Menu.Items.Count - 1; i >= 0; i--)
            {
                if (ExcludeList.Contains(e.Menu.Items[i].Name)) { e.Menu.Items.Remove(e.Menu.Items[i]); }
            }


            // Add New Context Menu Options
            RadMenuItem newItem = new RadMenuItem();
            newItem.Text = "Open";
            e.Menu.Items.Add(newItem);
            newItem.Click += new EventHandler(Context_Options);

            // Save Node as Public Var
            selectedNode = e.Node;
        }

        private void Context_Options(object sender, EventArgs e)
        {
            string Selected = "";

            if (selectedNode.Tag != null)
            {
                try
                {
                    Selected = selectedNode.Tag.ToString();
                }
                catch { }
            }

            if (Selected != "") { Context_Actions(Selected); }
        }

        public void Context_Actions(string Action)
        {
            _AHK ahk = new _AHK();
            
            if (Action.IsDir()) { ahk.OpenDir(Action); return; }

            if (File.Exists(Action)) { ahk.Run(Action); return; }

            // IMDb Title Tag Node
            string firstTwo = ahk.FirstCharacters(Action, 2);
            if (firstTwo.ToUpper() == "TT") { ahk.Run("http://www.imdb.com/title/tt1213404/"); return; }
        }


        public void EnableDragDrop(RadTreeView radTree)
        {
            // enabled actions and define handler events
            radTree.DragEnter += new DragEventHandler(RadTree_DragEnter);
            radTree.DragDrop += new DragEventHandler(RadTree_DragDrop);
            radTree.DragEnding += new RadTreeView.DragEndingHandler(RadTree_DragEnding);
            radTree.DragEnded += new RadTreeView.DragEndedHandler(RadTree_DragEnded);


            radTree.AllowDrop = true;
            radTree.AllowDragDrop = true;

            //radTree.AllowPlusMinusAnimation = true;

            // enable visual assistants for drag/drop operation on tree
            radTree.DropHintColor = Color.Yellow; // color line that indicates where drop will be placed
            radTree.ShowDragHint = true;
            radTree.ShowDropHint = true;
        }

        private void RadTree_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        // action after item dragged to tree
        private void RadTree_DragDrop(object sender, DragEventArgs e)
        {
            string dropText = e.Data.GetData(DataFormats.Text).ToString();  // text of item dropped on tree

            Point p = radTreeView1.PointToClient(new Point(e.X, e.Y));
            RadTreeNode hoverNode = radTreeView1.GetNodeAt(p.X, p.Y);
            if (hoverNode == null)
            {
                radTreeView1.Nodes.Add(dropText);
                return;
            }

            // add new node as item dropped by user
            hoverNode.Nodes.Add(dropText);
        }

        // before drag event ends, option to CANCEL action or restrict placement
        private void RadTree_DragEnding(object sender, RadTreeViewDragCancelEventArgs e)
        {
            if (e.TargetNode.Level != e.Node.Level)
            {
                e.Cancel = true;
                RadMessageBox.Show("Only nodes from the same level can be dropped here.");
            }
        }

        private void RadTree_DragEnded(object sender, RadTreeViewDragEventArgs e)
        {

        }



        #endregion


        #region === Populate Tree ===

        private void menuFileTree_Click(object sender, EventArgs e)
        {
            _TelerikLib.RadTree tree = new _TelerikLib.RadTree();

            // Add 1 Level Deep Dirs Only 
            tree.RadTree_Files(radTreeView1, @"C:\Users\jason\source\Workspaces\sharpAHK_Development\ADBindex");
        }



        #endregion


        #region === RadTree Node Filter ===

        //==== TreeView Node Filter ===

        private void txtFilterText_TextChanged(object sender, EventArgs e)
        {
            radTreeView1.Filter = txtFilterText.Text.ToUpper();

            HideNoChildrenNodes();
        }


        private bool FilterNode(RadTreeNode node)
        {
            if (node.Level > 0)
            {
                if (node.Text.ToUpper().Contains(txtFilterText.Text.ToUpper()))
                {
                    return true;
                }
                return false;
            }
            else
            {
                //int kids = node.GetNodeCount(true);
                return true;
            }

        }


        public void HideNoChildrenNodes()
        {
            _TelerikLib.RadTree tree = new _TelerikLib.RadTree();

            List<RadTreeNode> nodes = tree.NodesByLevel(radTreeView1, 0);

            foreach(RadTreeNode node in nodes)
            {
                int kids = node.GetNodeCount(true);

                if (kids > 0)
                {
                    node.Visible = true;
                }
                else
                {
                    node.Visible = false; 
                }

            }
        }



        void radTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {

                radTreeView1.Filter = txtFilterText.Text.ToUpper();


                //// enable RadTreeView Node Filter (goes in tree config)
                ////radTreeView1.TreeViewElement.FilterPredicate = FilterNode;

                //// text user has typed
                //string filterText = txtSameGenreFilter.Text;

                //// filter the tree based on text provided
                //radTreeOtherFilms.Filter = filterText;

                string filterTxt = txtFilterText.Text.ToUpper();

                foreach (RadTreeNode node in radTreeView1.Nodes)
                {
                    if (node.Text.ToUpper().Contains(filterTxt))
                    {
                        node.Visible = true;
                    }
                    else
                    {
                        node.Visible = false;
                    }
                }


                //RadTreeView clickedTree = node.RootNode.TreeView.Name; 


                //Predicate<RadTreeNode> match = new Predicate<RadTreeNode>(delegate (RadTreeNode node)
                //{

                //    //return node.Tag != null ? true : false;
                //    return node.Tag.ToString().Contains(txtSameGenreFilter.Text) ? true : false;
                //});
                //RadTreeNode[] result = radTreeOtherFilms.FindNodes(match);


            }
        }


        #endregion



        #endregion



        #region === Grid ContextMenu ===

        private void myGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1) { return; }  // do nothing if row isn't within range
            DataGridView dv = (DataGridView)sender;

            int RowNum = e.RowIndex;
            int ColNum = e.ColumnIndex;

            string ValueClicked = dv.Rows[RowNum].Cells[ColNum].Value.ToString();

            MessageBox.Show("User Clicked Row " + RowNum + " | Col " + ColNum + "\nValue: " + ValueClicked);
        }

        private void myGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1) { return; }  // do nothing if row isn't within range
            DataGridView dv = (DataGridView)sender;

            int RowNum = e.RowIndex;
            int ColNum = e.ColumnIndex;

            string ValueClicked = dv.Rows[RowNum].Cells[ColNum].Value.ToString();

            MessageBox.Show("User DOUBLE Clicked Row " + RowNum + " | Col " + ColNum + "\nValue: " + ValueClicked);
        }

        private void myGrid_MouseClick(object sender, MouseEventArgs e)
        {
            DataGridView dv = (DataGridView)sender;

            if (e.Button == MouseButtons.Right)
            {
                ContextMenu m = new ContextMenu();
                m.MenuItems.Add(new MenuItem("Cut", myGrid_MouseClickAction));
                m.MenuItems.Add(new MenuItem("Copy", myGrid_MouseClickAction));
                m.MenuItems.Add(new MenuItem("Paste", myGrid_MouseClickAction));
                m.MenuItems.Add(new MenuItem("LoadGrid", myGrid_MouseClickAction));


                int currentMouseOverRow = dv.HitTest(e.X, e.Y).RowIndex;

                if (currentMouseOverRow >= 0)
                {
                    m.MenuItems.Add(new MenuItem(string.Format("Do something to row {0}", currentMouseOverRow.ToString())));
                }

                m.Show(dv, new Point(e.X, e.Y));

            }
        }

        private void myGrid_MouseClickAction(Object sender, System.EventArgs e)
        {
            MenuItem clicked = (MenuItem)sender;


        }

        // when user edits grid, set value to true to save with next save action
        private void myGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)  // FunctionGrid Actions - Cell Value Changed
        {
            if (e.RowIndex < 0) { return; }  // invalid row number passed into function

            //if (e.RowIndex != -1)  // Return Row # in Grid
            //{
            //    int RowNum = (e.RowIndex); // returns selected row # in grid

            //    string FileName = ((DataGridView)sender).Rows[RowNum].Cells["FileName"].Value.ToString();  // return value from datagridview
            //    string TableName = ((DataGridView)sender).Rows[RowNum].Cells[0].Value.ToString();          // return value from datagridview
            //}
        }

        // Delete selected rows on DELETE Key Press on DataGrid
        private void myGrid_KeyUp(object sender, KeyEventArgs e) // FunctionGrid Actions - Delete selected rows on DELETE Key Press on DataGrid
        {
            //DataGridView dv = this;  // this grid

            if (e.KeyCode == Keys.Delete)
            {
                foreach (DataGridViewRow row in ((DataGridView)sender).SelectedRows)
                {
                    ((DataGridView)sender).Rows.Remove(row);
                }
            }

            // [ Space Bar ]

            if (e.KeyCode == Keys.Space)  // space key to toggle check box fields with the keyboard
            {
                //foreach (DataGridViewRow r in dv.SelectedRows)
                //{
                //    // toggle between checked and unchecked when highlighted and space bar pressed
                //    if (r.Cells["Setting_Bool"].Value.ToString().ToUpper() == "FALSE") { r.Cells["Setting_Bool"].Value = true; continue; }
                //    if (r.Cells["Setting_Bool"].Value.ToString().ToUpper() == "TRUE") { r.Cells["Setting_Bool"].Value = false; continue; }
                //}

                e.Handled = true; //this is necessary because otherwise when the checkbox cell is selected, it will apply this keyup and also apply the default behavior for the checkbox
            }


        }


        #endregion


        #region === Control Under Mouse ===

        // Ex: 
        //      frm.Add_MouseOverControl(this.Controls);  // add mouse-over-control event to every control - updates statusbar with current control


        // returns the name of the control currently under mouse

        public Control LastControlUnderMouse; // stores last control detected under mouse (for dynamic context menu)

        public void Add_MouseOverControl(System.Collections.IEnumerable controls)  // create an action that occurs when every control is moved over with mouse (add to startup)
        {
            // Ex: (Add To Startup)
            // Add_MouseOverControl(this.Controls);  // loop through all controls and add mouse over control action

            foreach (Control control in controls)
            {
                control.MouseEnter += MouseOverControl;

                if (control.HasChildren)
                    Add_MouseOverControl(control.Controls);
            }
        }

        // enable status bar to display results 
        public void MouseOverControl(object sender, EventArgs e)  // Event Handler for Mouse over Control to Update Screen/StatusBar with Control Name
        {

        }

        public string Control_Under_Mouse(Control control)  // returns the name of the control on a form currently under the cursor
        {
            string ControlName = "";
            try
            {
                ControlName = ControlNavigationHelper.getYoungestChildUnderMouse(control).Name.ToString();
                LastControlUnderMouse = ControlNavigationHelper.getYoungestChildUnderMouse(control);
            }
            catch { }
            return ControlName;
        }









        #endregion



        #endregion


        #region === Show Date Index (Date Based Show Reference Lib)


        #region === SDI FUNCTIONS ===

        #region ===== SDI Object =====

        public struct SDI
        {
            public string ShowName { get; set; }
            public string Year { get; set; }
            public string Month { get; set; }
            public string Day { get; set; }
            public string EpTitle { get; set; }
            public string FileName { get; set; }
            public string FileSize { get; set; }
            public string sCount { get; set; }
        }

        #endregion
        public bool Create_Table_ShowDateIndex(string DbFile)
        {
            _AHK ahk = new _AHK();
            _Database.SQLite sqlite = new _Database.SQLite();

            string CreateLine = "Create Table [ShowDateIndex] (ShowName VARCHAR, Year VARCHAR, Month VARCHAR, Day VARCHAR, EpTitle VARCHAR, FileName VARCHAR, FileSize VARCHAR, sCount VARCHAR)";
            bool TableCreated = sqlite.Table_Exists(DbFile, "ShowDateIndex");
            if (!TableCreated) { TableCreated = sqlite.Table_New(DbFile, "ShowDateIndex", "Create Table [ShowDateIndex] (ShowName VARCHAR, Year VARCHAR, Month VARCHAR, Day VARCHAR, EpTitle VARCHAR, FileName VARCHAR, FileSize VARCHAR, sCount VARCHAR", false); }


            if (!TableCreated) { ahk.MsgBox("[ShowDateIndex] Created = " + TableCreated.ToString()); }
            return TableCreated;
        }

        #region ===== SDI SQLite : Return =====

        public SDI Return_Object_From_ShowDateIndex(string WhereClause = "[ShowName] = ''", string DbFile = "")
        {
            _AHK ahk = new _AHK();
            _Database.SQLite sqlite = new _Database.SQLite();

            if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\SDI.sqlite"; }
            string SelectLine = "Select [ShowName], [Year], [Month], [Day], [EpTitle], [FileName], [FileSize], [sCount] From [ShowDateIndex] ";
            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
            if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
            if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
            SDI returnObject = new SDI();
            int i = 0;
            string Value = "";
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    returnObject.ShowName = ret["ShowName"].ToString();
                    returnObject.Year = ret["Year"].ToString();
                    returnObject.Month = ret["Month"].ToString();
                    returnObject.Day = ret["Day"].ToString();
                    returnObject.EpTitle = ret["EpTitle"].ToString();
                    returnObject.FileName = ret["FileName"].ToString();
                    returnObject.FileSize = ret["FileSize"].ToString();
                    returnObject.sCount = ret["sCount"].ToString();
                }
            }

            return returnObject;
        }

        public List<SDI> Return_SDI_List(string WhereClause = "", string DbFile = "", string TableName = "[ShowDateIndex]")
        {
            _AHK ahk = new _AHK();
            _Database.SQLite sqlite = new _Database.SQLite();

            if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\SDI.sqlite"; }
            string SelectLine = "Select * From " + TableName;

            if (WhereClause != "")
            {
                if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
            }
            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

            List<SDI> ReturnList = new List<SDI>();
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    SDI returnObject = new SDI();

                    returnObject.ShowName = ret["ShowName"].ToString();
                    returnObject.Year = ret["Year"].ToString();
                    returnObject.Month = ret["Month"].ToString();
                    returnObject.Day = ret["Day"].ToString();
                    returnObject.EpTitle = ret["EpTitle"].ToString();
                    returnObject.FileName = ret["FileName"].ToString();
                    returnObject.FileSize = ret["FileSize"].ToString();
                    returnObject.sCount = ret["sCount"].ToString();

                    ReturnList.Add(returnObject);
                }
            }

            return ReturnList;
        }

        public DataTable Return_DataTable_From_ShowDateIndex(string DbFile)
        {
            _AHK ahk = new _AHK();
            _Database.SQLite sqlite = new _Database.SQLite();

            string SelectLine = "Select [ShowName], [Year], [Month], [Day], [EpTitle], [FileName], [FileSize], [sCount] From [ShowDateIndex]";
            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
            return ReturnTable;
        }


        #endregion

        #region ===== SDI SQL Functions =====

        // Return SDI SQL Connection String
        public SqlConnection SDI_Conn()
        {
            // SQL Connection with LOCAL Server:  
            SqlConnection Conn = new SqlConnection("Server=(localdb)\\MyInstance;DataBase=Movies;Integrated Security=true;");

            // populate sql connection
            //SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLserver"].ConnectionString);
            // SqlConnection Conn = new SqlConnection("Server=188.168.188.88;DataBase=LucidMedia;Uid=lucidm;Pwd=pass");
            return Conn;
        }

        // Return SDI TableName (Full Path)
        public string SDI_TableName()
        {
            // populate to return full sql table name
            return "[Movies].[dbo].[ShowDateIndex]";
        }


        public bool SDI_InsertSQL(SDI obj)
        {
            _AHK ahk = new _AHK();

            SqlConnection Con = SDI_Conn();
            string SQLLine = "Insert Into " + SDI_TableName() + " (ShowName, Year, Month, Day, EpTitle, FileName, FileSize, sCount) VALUES (@ShowName, @Year, @Month, @Day, @EpTitle, @FileName, @FileSize, @sCount)";
            SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
            cmd2 = new SqlCommand(SQLLine, Con);
            if (obj.ShowName == null) { obj.ShowName = ""; }
            if (obj.Year == null) { obj.Year = ""; }
            if (obj.Month == null) { obj.Month = ""; }
            if (obj.Day == null) { obj.Day = ""; }
            if (obj.EpTitle == null) { obj.EpTitle = ""; }
            if (obj.FileName == null) { obj.FileName = ""; }
            if (obj.FileSize == null) { obj.FileSize = ""; }
            if (obj.sCount == null) { obj.sCount = ""; }
            cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
            cmd2.Parameters.AddWithValue(@"Year", obj.Year.ToString());
            cmd2.Parameters.AddWithValue(@"Month", obj.Month.ToString());
            cmd2.Parameters.AddWithValue(@"Day", obj.Day.ToString());
            cmd2.Parameters.AddWithValue(@"EpTitle", obj.EpTitle.ToString());
            cmd2.Parameters.AddWithValue(@"FileName", obj.FileName.ToString());
            cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize.ToString());
            cmd2.Parameters.AddWithValue(@"sCount", obj.sCount.ToString());
            if (Con.State == ConnectionState.Closed) { Con.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
            Con.Close();
            if (recordsAffected > 0) { return true; }
            else return false;
        }

        public bool SDI_UpdateSQL(SDI obj)
        {
            _AHK ahk = new _AHK();

            SqlConnection Conn = SDI_Conn();
            string SQLLine = "Update " + SDI_TableName() + " SET ShowName = @ShowName, Year = @Year, Month = @Month, Day = @Day, EpTitle = @EpTitle, FileName = @FileName, FileSize = @FileSize, sCount = @sCount WHERE ID = @ID";
            SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
            cmd2 = new SqlCommand(SQLLine, Conn);
            if (obj.ShowName == null) { obj.ShowName = ""; }
            if (obj.Year == null) { obj.Year = ""; }
            if (obj.Month == null) { obj.Month = ""; }
            if (obj.Day == null) { obj.Day = ""; }
            if (obj.EpTitle == null) { obj.EpTitle = ""; }
            if (obj.FileName == null) { obj.FileName = ""; }
            if (obj.FileSize == null) { obj.FileSize = ""; }
            if (obj.sCount == null) { obj.sCount = ""; }
            cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
            cmd2.Parameters.AddWithValue(@"Year", obj.Year.ToString());
            cmd2.Parameters.AddWithValue(@"Month", obj.Month.ToString());
            cmd2.Parameters.AddWithValue(@"Day", obj.Day.ToString());
            cmd2.Parameters.AddWithValue(@"EpTitle", obj.EpTitle.ToString());
            cmd2.Parameters.AddWithValue(@"FileName", obj.FileName.ToString());
            cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize.ToString());
            cmd2.Parameters.AddWithValue(@"sCount", obj.sCount.ToString());
            if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
            Conn.Close();
            if (recordsAffected > 0) { return true; }
            else return false;
        }

        public bool SDI_UpdateInsert(SDI obj)
        {
            SqlConnection Conn = SDI_Conn();
            bool Updated = SDI_UpdateSQL(obj);  // try to update record first
            if (!Updated) { Updated = SDI_InsertSQL(obj); }  // if unable to update, insert new record
            return Updated;
        }

        // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
        public bool SDI_UpdateIfPopulated(SDI obj, string ID = "")
        {
            _AHK ahk = new _AHK();

            SqlConnection Conn = SDI_Conn();
            string SQLcmd = "Update " + SDI_TableName() + " SET ";
            if (obj.ShowName != null) { SQLcmd = SQLcmd + " ShowName = @ShowName,"; }
            if (obj.Year != null) { SQLcmd = SQLcmd + " Year = @Year,"; }
            if (obj.Month != null) { SQLcmd = SQLcmd + " Month = @Month,"; }
            if (obj.Day != null) { SQLcmd = SQLcmd + " Day = @Day,"; }
            if (obj.EpTitle != null) { SQLcmd = SQLcmd + " EpTitle = @EpTitle,"; }
            if (obj.FileName != null) { SQLcmd = SQLcmd + " FileName = @FileName,"; }
            if (obj.FileSize != null) { SQLcmd = SQLcmd + " FileSize = @FileSize,"; }
            if (obj.sCount != null) { SQLcmd = SQLcmd + " sCount = @sCount,"; }
            SQLcmd = ahk.TrimLast(SQLcmd, 1);
            SQLcmd = SQLcmd + " WHERE ID = @ID";

            SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

            if (obj.ShowName != null) { cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName); }
            if (obj.Year != null) { cmd2.Parameters.AddWithValue(@"Year", obj.Year); }
            if (obj.Month != null) { cmd2.Parameters.AddWithValue(@"Month", obj.Month); }
            if (obj.Day != null) { cmd2.Parameters.AddWithValue(@"Day", obj.Day); }
            if (obj.EpTitle != null) { cmd2.Parameters.AddWithValue(@"EpTitle", obj.EpTitle); }
            if (obj.FileName != null) { cmd2.Parameters.AddWithValue(@"FileName", obj.FileName); }
            if (obj.FileSize != null) { cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize); }
            if (obj.sCount != null) { cmd2.Parameters.AddWithValue(@"sCount", obj.sCount); }

            if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
            Conn.Close();
            if (recordsAffected > 0) { return true; }
            else return false;
        }

        public SDI SDI_ReturnSQL(SDI obj)
        {
            _Database.SQL sql = new _Database.SQL();

            SqlConnection Conn = SDI_Conn();
            string SelectLine = "Select [sCount] From " + SDI_TableName() + " WHERE  [ShowName] = '" + obj.ShowName + "' AND [Year] = '" + obj.Year + "' AND [Month] = '" + obj.Month + "' AND [Day] = '" + obj.Day + "'";
            DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
            SDI returnObject = new SDI();
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    returnObject.ShowName = ret["ShowName"].ToString();
                    returnObject.Year = ret["Year"].ToString();
                    returnObject.Month = ret["Month"].ToString();
                    returnObject.Day = ret["Day"].ToString();
                    returnObject.EpTitle = ret["EpTitle"].ToString();
                    returnObject.FileName = ret["FileName"].ToString();
                    returnObject.FileSize = ret["FileSize"].ToString();
                    returnObject.sCount = ret["sCount"].ToString();
                    return returnObject;
                }
            }
            return returnObject;
        }

        public List<SDI> SDI_ReturnSQLList(string Command = "")
        {
            _Database.SQL sql = new _Database.SQL();

            if (Command == "") { Command = "Select * From " + SDI_TableName(); }
            SqlConnection Conn = SDI_Conn();
            DataTable ReturnTable = sql.GetDataTable(Conn, Command);
            List<SDI> ReturnList = new List<SDI>();
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    SDI returnObject = new SDI();
                    returnObject.ShowName = ret["ShowName"].ToString();
                    returnObject.Year = ret["Year"].ToString();
                    returnObject.Month = ret["Month"].ToString();
                    returnObject.Day = ret["Day"].ToString();
                    returnObject.EpTitle = ret["EpTitle"].ToString();
                    returnObject.FileName = ret["FileName"].ToString();
                    returnObject.FileSize = ret["FileSize"].ToString();
                    returnObject.sCount = ret["sCount"].ToString();
                    ReturnList.Add(returnObject);
                }
            }
            return ReturnList;
        }

        public bool SDI_SQL_to_SQLite(string SqliteDBPath = @"\Db\SDI.sqlite")
        {
            _AHK ahk = new _AHK();
            _Database.SQLite sqlite = new _Database.SQLite();


            string SaveFile = SqliteDBPath;
            if (SqliteDBPath == @"\Db\SDI.sqlite")
            {
                ahk.FileCreateDir(ahk.AppDir() + @"\Db");
                SaveFile = ahk.AppDir() + @"\Db\SDI.sqlite";
            }

            sb("Copying SQL Db to " + SaveFile + "...");
            sqlite.SQLTable_To_NewSQLiteTable(SDI_Conn(), "ShowDateIndex", "ShowDateIndex", SaveFile, "", false, false, false);
            sb("FINISHED Copying SQL Db to " + SaveFile);

            if (File.Exists(SaveFile)) { return true; } else { return false; }
        }


        #endregion

        #endregion


        public void ShowDateIndex(string ShowName = "The Daily Show", string ShowRootDir = @"O:\Talk Shows\The Daily Show\2008")
        {
            _Lists lst = new _Lists();
            _AHK ahk = new _AHK();
            _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();
            _Parse prs = new _Parse();

            List<string> files = lst.FileList(ShowRootDir, "*.*", true, false, true);

            pro.SetupProgressBar(radProgressBar1, files.Count);

            int i = 0;
            foreach(string file in files)
            {
                i++; 
                pro.UpdateProgress(radProgressBar1, i.ToString() + "/" + files.Count);

                SDI obj = new SDI();

                obj.ShowName = ShowName;

                obj.EpTitle = file.FileName();
                obj.FileSize = file.FileSize();
                obj.FileName = file;

                // extract season/ep number from file name
                //string epNum = prs.SeasonEpNums(file);

                string showDate = ""; // prs.ExtractDate(file.FileNameNoExt());



                //public string ExtractDate(string Text)

                bool YearFirst = true;

                // regex search for "year month day" date convention
                Regex regex = new Regex(@"\d{4}.\d{2}.\d{2}");  // 2018 01 17
                Match match = regex.Match(obj.FileName);
                if (match.Success)
                {
                    showDate = match.Value;
                }
                else
                {
                    // if no match, try "month day year" format next
                    regex = new Regex(@"\d{2}.\d{2}.\d{4}");  // 01 17 2018
                    match = regex.Match(obj.FileName);
                    if (match.Success)
                    {
                        showDate = match.Value;
                        YearFirst = false;
                    }
                    else
                    {
                        // if no match, try "month day year" short format next
                        regex = new Regex(@"\d{2}.\d{2}.\d{2}");  // 01 17 18
                        match = regex.Match(obj.FileName);
                        if (match.Success)
                        {
                            showDate = match.Value;
                            YearFirst = false;
                        }
                    }
                }


                showDate = showDate.Replace(" ", "-");
                showDate = showDate.Replace(".", "-");
                showDate = showDate.Replace("_", "-");

                List<string> dateSegs = ahk.StringSplit_List(showDate, "-");

                // match the year to month date fields based on whether the date came back with year first or last in naming sequence

                int k = 1;
                foreach (string seg in dateSegs) 
                {
                    if (k == 1)
                    {
                        if (!YearFirst) { obj.Month = seg; }
                        else { obj.Year = seg; }
                    }
                    if (k == 2)
                    {
                        if (!YearFirst) { obj.Day = seg; }
                        else { obj.Month = seg; }
                    }
                    if (k == 3)
                    {
                        if (!YearFirst)
                        {
                            // if only 2 characters for year, add leading date values for 4 digit year
                            if (seg.CharCount(seg.Trim()) == 2) { obj.Year = "20" + seg.Trim(); }
                            else if (seg.CharCount(seg.Trim()) == 1) { obj.Year = "200" + seg.Trim(); }
                            else { obj.Year = seg; }
                        }
                        else { obj.Day = seg; }
                    }
                    k++;
                }


                if (obj.Year == null || obj.Year == "")
                {
                    obj.Year = prs.ExtractYear(file.FileName());
                }



                SDI_InsertSQL(obj);
            }





        }

        private void menuShowDateIndex_Click(object sender, EventArgs e)
        {
            ShowDateIndex("The Daily Show", @"O:\Talk Shows\The Daily Show");
        }


        private void btnPopulateComedyTree_Click(object sender, EventArgs e)
        {
            ComedianTree(radTreeView1, "S:\\", true);
        }


        #endregion


        #region === Populate RadTree ===


        // Standup Comedians IndexTree
        public void ComedianTree(RadTreeView radtree, string rootDir = "S:\\", bool NewThread = true)
        {
            _AHK ahk = new _AHK();
            _Lists lst = new _Lists();
            _TelerikLib tel = new _TelerikLib();
            _TelerikLib.RadTree tree = new _TelerikLib.RadTree();

            if (NewThread)
            {
                Thread imdbTVParseThread = new Thread(() => ComedianTree(radtree, rootDir, false));
                imdbTVParseThread.Start();
            }
            else
            {
                //string rootDir = "S:\\";

                List<string> Comedians = lst.DirList(rootDir, "*.*", false, false);

                Comedians = lst.SortList(Comedians); // alpha sort list

                foreach (string com in Comedians)
                {
                    string first = ahk.FirstCharacters(com, 1); if (first == "_") { continue; }   // skip folders starting with "_"

                    // add node for comedian
                    RadTreeNode comNode = new RadTreeNode();
                    comNode.Text = com; comNode.Tag = rootDir + "\\" + com;
                    //radtree.Nodes.Add(comNode);
                    tree.AddNode(radtree, comNode);

                    // list of shows under comedian dir
                    List<string> shows = lst.DirList(rootDir + "\\" + com, "*.*", false, false);

                    shows = lst.SortList(shows); // alpha sort list

                    foreach (string show in shows)
                    {
                        RadTreeNode showNode = new RadTreeNode();
                        showNode.Text = show; showNode.Tag = rootDir + "\\" + com + "\\" + show;
                        //comNode.Nodes.Add(showNode);
                        tree.AddSubNode(comNode, showNode, radtree);
                    }
                }
            }


        }


        // File TreeView 
        private void btnFileTree_Click(object sender, EventArgs e)
        {
            _TelerikLib.RadTree tree = new _TelerikLib.RadTree();

            tree.RadTree_Files(radTreeView1, txtPath.Text);
        }

        private void btnShowDateIndex_Click(object sender, EventArgs e)
        {
            _Lists lst = new _Lists();
            string rootDir = @"O:\Talk Shows";

            List<string> dirs = lst.DirList(rootDir, "*.*", false, false);

            int t = 0;
            foreach(string dir in dirs)
            {
                string path = rootDir + "\\" + dir;
                ShowDateIndex(dir, path);
                t++;
            }

            _AHK ahk = new _AHK();
            ahk.MsgBox("Show Date Index : Updated " + t.ToString() + " Shows");

            //string showName = "The Daily Show";
            //ShowDateIndex(txtShowName.Text, txtPath.Text);
        }


        #endregion

        private void radMenuThreadsPage_Click(object sender, EventArgs e)
        {
            RadMenuItem clicked = (RadMenuItem)sender;

            // When User Clicks Top Drop Down Option, Set menu checkbox values based on pages currently added to pageview
            if (clicked.Text == "Options")
            {
                if (radPageView1.Contains(radPageSharpTree)) { radMenuSharpTree.IsChecked = true; } else { radMenuSharpTree.IsChecked = false; }
                if (radPageView1.Contains(radPageConfig)) { radMenuConfig.IsChecked = true; } else { radMenuConfig.IsChecked = false; }
                if (radPageView1.Contains(radPageThreads)) { radMenuThreads.IsChecked = true; } else { radMenuThreads.IsChecked = false; }
            }


            // When user selects page option, if checked used as toggle and remove page/checkbox, otherwise launch page and activate while checking option
            if (clicked.Text == "SharpTree")
            {
                if (radMenuSharpTree.IsChecked) { _RadPage.Pages.Remove(radPageSharpTree); }
                else
                {
                    if (!radPageView1.Contains(radPageSharpTree)) { radPageView1.Pages.Add(radPageSharpTree); }  // create page if closed
                    radPageView1.SelectedPage = radPageSharpTree; // select page
                    radMenuSharpTree.IsChecked = true;
                }
            }
            if (clicked.Text == "Config")
            {
                if (radMenuConfig.IsChecked) { _RadPage.Pages.Remove(radPageConfig); }
                else
                {
                    if (!radPageView1.Contains(radPageConfig)) { radPageView1.Pages.Add(radPageConfig); }  // create page if closed
                    radPageView1.SelectedPage = radPageConfig; // select page
                    radMenuConfig.IsChecked = true;
                }
            }
            if (clicked.Text == "Threads")
            {
                if (radMenuThreads.IsChecked) { _RadPage.Pages.Remove(radPageThreads); }
                else
                {
                    if (!radPageView1.Contains(radPageThreads)) { radPageView1.Pages.Add(radPageThreads); }  // create page if closed
                    radPageView1.SelectedPage = radPageThreads; // select page
                    radMenuThreads.IsChecked = true;
                }
            }

        }


    }
}

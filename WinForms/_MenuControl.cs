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
using ScintillaNET;
using System.Collections;
using System.Threading;
using System.Drawing;

namespace sharpAHK_Dev
{
    public class _MenuControl
    {
        #region === Startup ===

        private static _Code code = new _Code();
        private static _Images img = new _Images();
        private static _Database.Tags tags = new _Database.Tags();
        private static _WinForms cv = new _WinForms();
        private static _AHK ahk = new _AHK();
        private static _Lists lst = new _Lists();


        #endregion

        #region === Populate Menu ===

        // ### Populate Menu From Dir / List ###

        /// <summary>Create New Menu Item (Top Level) or Attach Under Existing ToolStripMenuItem (Leave NewMenuName Blank to not create subgroup)</summary>
        /// <param name="list"> </param>
        /// <param name="MenuOrItem"> </param>
        /// <param name="ClickEventFunction"> </param>
        /// <param name="NewMenuName"> </param>
        /// <param name="RemovePrevious"> </param>
        public object List_To_Menu(List<string> list, object MenuOrItem, EventHandler ClickEventFunction, string NewMenuName = "", bool RemovePrevious = false)
        {
            ToolStripMenuItem MenuItem = new ToolStripMenuItem();

            if (MenuOrItem is ToolStripMenuItem)  // menu item provided to attach to 
            {
                MenuItem = MenuOrItem as ToolStripMenuItem;  // cast object as MenuStrip item

                ToolStripMenuItem parentMenuItem = new ToolStripMenuItem(NewMenuName);  // new top level menu item

                if (list != null)
                {
                    foreach (string text in list)  // create new submenu items and attach to parent Menu item
                    {
                        ToolStripMenuItem foo = new ToolStripMenuItem(text);
                        foo.Tag = parentMenuItem.Text;
                        foo.Click += new EventHandler(ClickEventFunction);

                        if (NewMenuName == "")  // attach directly under parent if no name provided
                        {
                            MenuItem.DropDownItems.Add(foo);
                        }

                        if (NewMenuName != "")
                        {
                            parentMenuItem.DropDownItems.Add(foo);
                        }

                    }
                }

                if (NewMenuName != "")
                {
                    MenuItem.DropDownItems.Add(parentMenuItem);
                }

                return MenuItem;
            }

            if (MenuOrItem is MenuStrip)  // create new menu item on top level of menu strip
            {
                MenuStrip AttachMenu = MenuOrItem as MenuStrip;  // cast object as MenuStrip item

                if (RemovePrevious)
                {
                    Menu_Remove(AttachMenu, NewMenuName);
                }


                ToolStripMenuItem parentMenuItem = new ToolStripMenuItem(NewMenuName);  // new top level menu item

                foreach (string text in list)  // create new submenu items and attach to parent Menu item
                {
                    ToolStripMenuItem foo = new ToolStripMenuItem(text);
                    foo.Tag = parentMenuItem.Text;
                    foo.Click += new EventHandler(ClickEventFunction);
                    parentMenuItem.DropDownItems.Add(foo);
                }

                AttachMenu.Items.Add(parentMenuItem);

                return parentMenuItem;
            }

            return MenuItem;
        }

        /// <summary>Create menu from list of directories, populate submenus with directory file names, attach to menu item, assign click event</summary>
        /// <param name="DirPath"> </param>
        /// <param name="MenuOrItem"> </param>
        /// <param name="ClickEventFunction"> </param>
        /// <param name="NewMenuName"> </param>
        /// <param name="RemoveFileExt"> </param>
        /// <param name="RemovePreviousMenu"> </param>
        public void DirList_To_Menu(string DirPath, object MenuOrItem, EventHandler ClickEventFunction, string NewMenuName = "Dir_Menu", bool RemoveFileExt = false, bool RemovePreviousMenu = false)
        {
            List<string> DirList = lst.DirList(DirPath, "*.*", false);

            if (MenuOrItem is MenuStrip)
            {
                MenuStrip AttachMenu = MenuOrItem as MenuStrip;  // cast object as MenuStrip item

                if (RemovePreviousMenu)
                {
                    Menu_Remove(AttachMenu, NewMenuName);
                }

                ToolStripMenuItem parentMenuItem = new ToolStripMenuItem(NewMenuName);  // new top level menu item

                foreach (string dirName in DirList)  // create new submenu items and attach to parent Menu item
                {
                    ToolStripMenuItem foo = new ToolStripMenuItem(dirName);
                    parentMenuItem.DropDownItems.Add(foo);

                    List<string> fileMenuItems = lst.FileList(DirPath + "\\" + dirName, "*.*", false);

                    foreach (string fileName in fileMenuItems)  // create new submenu items and attach to parent Menu item
                    {
                        string FileName = "";

                        if (RemoveFileExt) { FileName = ahk.FileNameNoExt(fileName); }
                        if (!RemoveFileExt) { FileName = ahk.FileName(fileName); }


                        ToolStripMenuItem fileM = new ToolStripMenuItem(FileName);
                        fileM.Click += new EventHandler(ClickEventFunction);
                        foo.DropDownItems.Add(fileM);
                    }
                }

                AttachMenu.Items.Add(parentMenuItem);
            }


            // user passed in an existing ToolStrip Item - build and Populate on that ToolStrip
            if (MenuOrItem is ToolStripMenuItem)
            {
                ToolStripMenuItem parentMenuItem = MenuOrItem as ToolStripMenuItem;

                foreach (string dirName in DirList)  // create new submenu items and attach to parent Menu item
                {
                    ToolStripMenuItem foo = new ToolStripMenuItem(dirName);
                    parentMenuItem.DropDownItems.Add(foo);

                    List<string> fileMenuItems = lst.FileList(DirPath + "\\" + dirName, "*.*", false);

                    foreach (string fileName in fileMenuItems)  // create new submenu items and attach to parent Menu item
                    {
                        string FileName = "";

                        if (RemoveFileExt) { FileName = ahk.FileNameNoExt(fileName); }
                        if (!RemoveFileExt) { FileName = ahk.FileName(fileName); }


                        ToolStripMenuItem fileM = new ToolStripMenuItem(FileName);
                        fileM.Click += new EventHandler(ClickEventFunction);
                        foo.DropDownItems.Add(fileM);
                    }
                }
            }


        }

        /// <summary>create list of files under 1 directory to create menu items (option to recurse)</summary>
        /// <param name="DirPath"> </param>
        /// <param name="parentMenu"> </param>
        /// <param name="ClickEventFunction"> </param>
        /// <param name="NewMenuName"> </param>
        /// <param name="SearchPattern"> </param>
        /// <param name="Recurse"> </param>
        /// <param name="FileNameOnly"> </param>
        /// <param name="RemovePreviousMenu"> </param>
        public void Dir_To_Menu(string DirPath, object parentMenu, EventHandler ClickEventFunction, string NewMenuName = "Dir_Menu", string SearchPattern = "*.*", bool Recurse = false, bool FileNameOnly = true, bool RemovePreviousMenu = false)
        {
            List<string> FileList = lst.FileList(DirPath, SearchPattern, Recurse, FileNameOnly, false);

            List_To_Menu(FileList, parentMenu, ClickEventFunction, NewMenuName, RemovePreviousMenu);
        }

        /// <summary>Creates menu from .cs project file, just the list of control types found in the project</summary>
        /// <param name="menu"> </param>
        /// <param name="ProjectFile"> </param>
        /// <param name="ControlNameSub"> </param>
        /// <param name="MenuName"> </param>
        public void Menu_Project_Control_Types(MenuStrip menu, string ProjectFile, bool ControlNameSub = true, string MenuName = "Controls")
        {
            List<string> ControlTypes = code.ProjectFile_ControlList(ProjectFile, "ALL", true);  // parse project Designer.cs file - returns list of controls (either all or by control type) - ReturnControlTypes option to just return list of types of control in project

            var item = new System.Windows.Forms.ToolStripMenuItem()  // top menu option
            {
                Name = MenuName,
                Text = MenuName
            };

            foreach (string type in ControlTypes)
            {
                string cType = ahk.StringReplace(type, "System.Windows.Forms.");
                cType = ahk.StringReplace(cType, "ScintillaNET.");
                cType = ahk.StringReplace(cType, "TreeViewFast.Controls.");
                cType = ahk.StringReplace(cType, "System.IO.");

                var item2 = new System.Windows.Forms.ToolStripMenuItem()  // sub menu options
                {
                    Name = cType,
                    Text = cType,
                    Tag = type
                };

                item.DropDownItems.Add(item2);


                if (ControlNameSub)  // option to add the control names under the control types
                {
                    List<string> controlNames = code.ProjectFile_ControlList(ProjectFile, cType, false);  // return list of the control names for this control type in project
                    foreach (string name in controlNames)
                    {
                        var item3 = new System.Windows.Forms.ToolStripMenuItem()  // sub-sub menu options
                        {
                            Name = name,
                            Text = name,
                            Tag = type
                        };

                        item2.DropDownItems.Add(item3);
                    }
                }

            }

            menu.Items.Add(item);

        }

        /// <summary>Create menu from datagridview column names to hide/show on click</summary>
        /// <param name="dv"> </param>
        /// <param name="MenuOrItem"> </param>
        /// <param name="ClickEventFunction"> </param>
        /// <param name="NewMenuName"> </param>
        public object Grid_ColumnNames_To_Menu(DataGridView dv, object MenuOrItem, EventHandler ClickEventFunction, string NewMenuName = "Grid Columns")
        {
            _GridControl grid = new _GridControl();

            List<string> GridColNames = grid.Column_Names(dv);


            ToolStripMenuItem MenuItem = new ToolStripMenuItem();

            if (MenuOrItem is ToolStripMenuItem)  // menu item provided to attach to 
            {
                MenuItem = MenuOrItem as ToolStripMenuItem;  // cast object as MenuStrip item

                ToolStripMenuItem ColMenu = new ToolStripMenuItem(NewMenuName);

                foreach (string colname in GridColNames)  // create new submenu items and attach to parent Menu item
                {
                    //ToolStripMenuItem foo = new ToolStripMenuItem(text);
                    //foo.Click += new EventHandler(ClickEventFunction);
                    //MenuItem.DropDownItems.Add(foo);


                    ToolStripMenuItem foo = new ToolStripMenuItem(colname);

                    bool ColumnVisible = grid.Column_Visible(dv, colname);  // check to see if this column is currently visible on the grid
                    foo.Checked = ColumnVisible;   // check menu item if column is visible

                    //foo.Checked = 
                    foo.Click += new EventHandler(ClickEventFunction);

                    ColMenu.DropDownItems.Add(foo);

                }


                MenuItem.DropDownItems.Add(ColMenu);

                return MenuItem;
            }



            if (MenuOrItem is MenuStrip)  // create new menu item on top level of menu strip
            {
                MenuStrip AttachMenu = MenuOrItem as MenuStrip;  // cast object as MenuStrip item

                //Menu_Remove(AttachMenu, NewMenuName);

                ToolStripMenuItem parentMenuItem = new ToolStripMenuItem(NewMenuName);  // new top level menu item

                foreach (string colname in GridColNames)  // create new submenu items and attach to parent Menu item
                {
                    ToolStripMenuItem foo = new ToolStripMenuItem(colname);

                    bool ColumnVisible = grid.Column_Visible(dv, colname);  // check to see if this column is currently visible on the grid
                    foo.Checked = ColumnVisible;   // check menu item if column is visible

                    //foo.Checked = 
                    foo.Click += new EventHandler(ClickEventFunction);
                    parentMenuItem.DropDownItems.Add(foo);
                }

                AttachMenu.Items.Add(parentMenuItem);

                return parentMenuItem;
            }


            //if (ParentName == "") { parentMenuItem.Text = dv.Name.ToString(); }  // rename the menu header the name of the datagridview control
            //if (ParentName != "") { parentMenuItem.Text = ParentName; }  // rename the menu header the name of the datagridview control

            return MenuItem;

        }

        /// <summary>Create menu from datagridview to toggle various grid actions/behaviors/bools</summary>
        /// <param name="dv"> </param>
        /// <param name="MenuOrItem"> </param>
        /// <param name="ClickEventFunction"> </param>
        /// <param name="NewMenuName"> </param>
        public object Grid_Add_Config_Menu(DataGridView dv, object MenuOrItem, EventHandler ClickEventFunction, string NewMenuName = "Grid Config")
        {
            _GridControl grid = new _GridControl();

            List<string> GridColNames = grid.Column_Names(dv);


            ToolStripMenuItem MenuItem = new ToolStripMenuItem();

            if (MenuOrItem is ToolStripMenuItem)  // menu item provided to attach to 
            {
                MenuItem = MenuOrItem as ToolStripMenuItem;  // cast object as MenuStrip item

                ToolStripMenuItem ColMenu = new ToolStripMenuItem(NewMenuName);


                ToolStripMenuItem foo = new ToolStripMenuItem(dv.Name);  // grid name
                foo.Checked = false;
                foo.Click += new EventHandler(ClickEventFunction);
                ColMenu.DropDownItems.Add(foo);


                foo = new ToolStripMenuItem("Allow Drop");
                foo.Checked = dv.AllowDrop;
                foo.Click += new EventHandler(ClickEventFunction);
                ColMenu.DropDownItems.Add(foo);


                foo = new ToolStripMenuItem("Allow User Add Rows");
                foo.Checked = dv.AllowUserToAddRows;
                foo.Click += new EventHandler(ClickEventFunction);
                ColMenu.DropDownItems.Add(foo);

                foo = new ToolStripMenuItem("Allow User Delete Rows");
                foo.Checked = dv.AllowUserToDeleteRows;
                foo.Click += new EventHandler(ClickEventFunction);
                ColMenu.DropDownItems.Add(foo);

                foo = new ToolStripMenuItem("Allow User Order Columns");
                foo.Checked = dv.AllowUserToOrderColumns;
                foo.Click += new EventHandler(ClickEventFunction);
                ColMenu.DropDownItems.Add(foo);

                foo = new ToolStripMenuItem("Allow User Resize Columns");
                foo.Checked = dv.AllowUserToResizeColumns;
                foo.Click += new EventHandler(ClickEventFunction);
                ColMenu.DropDownItems.Add(foo);

                foo = new ToolStripMenuItem("Allow User Resize Rows");
                foo.Checked = dv.AllowUserToResizeRows;
                foo.Click += new EventHandler(ClickEventFunction);
                ColMenu.DropDownItems.Add(foo);

                foo = new ToolStripMenuItem("Causes Validation");
                foo.Checked = dv.CausesValidation;
                foo.Click += new EventHandler(ClickEventFunction);
                ColMenu.DropDownItems.Add(foo);

                foo = new ToolStripMenuItem("Column Headers Visible");
                foo.Checked = dv.ColumnHeadersVisible;
                foo.Click += new EventHandler(ClickEventFunction);
                ColMenu.DropDownItems.Add(foo);

                foo = new ToolStripMenuItem("Enabled");
                foo.Checked = dv.Enabled;
                foo.Click += new EventHandler(ClickEventFunction);
                ColMenu.DropDownItems.Add(foo);

                foo = new ToolStripMenuItem("Enable Header Visual Styles");
                foo.Checked = dv.EnableHeadersVisualStyles;
                foo.Click += new EventHandler(ClickEventFunction);
                ColMenu.DropDownItems.Add(foo);

                foo = new ToolStripMenuItem("Multi Select");
                foo.Checked = dv.MultiSelect;
                foo.Click += new EventHandler(ClickEventFunction);
                ColMenu.DropDownItems.Add(foo);

                foo = new ToolStripMenuItem("Read Only");
                foo.Checked = dv.ReadOnly;
                foo.Click += new EventHandler(ClickEventFunction);
                ColMenu.DropDownItems.Add(foo);

                foo = new ToolStripMenuItem("Row Headers Visible");
                foo.Checked = dv.RowHeadersVisible;
                foo.Click += new EventHandler(ClickEventFunction);
                ColMenu.DropDownItems.Add(foo);

                foo = new ToolStripMenuItem("Show Cell Errors");
                foo.Checked = dv.ShowCellErrors;
                foo.Click += new EventHandler(ClickEventFunction);
                ColMenu.DropDownItems.Add(foo);

                foo = new ToolStripMenuItem("Show Cell ToolTips");
                foo.Checked = dv.ShowCellToolTips;
                foo.Click += new EventHandler(ClickEventFunction);
                ColMenu.DropDownItems.Add(foo);

                foo = new ToolStripMenuItem("Show Editing Icon");
                foo.Checked = dv.ShowEditingIcon;
                foo.Click += new EventHandler(ClickEventFunction);
                ColMenu.DropDownItems.Add(foo);

                foo = new ToolStripMenuItem("Show Row Errors");
                foo.Checked = dv.ShowRowErrors;
                foo.Click += new EventHandler(ClickEventFunction);
                ColMenu.DropDownItems.Add(foo);

                foo = new ToolStripMenuItem("Tab Stop");
                foo.Checked = dv.TabStop;
                foo.Click += new EventHandler(ClickEventFunction);
                ColMenu.DropDownItems.Add(foo);

                foo = new ToolStripMenuItem("Use Wait Cursor");
                foo.Checked = dv.UseWaitCursor;
                foo.Click += new EventHandler(ClickEventFunction);
                ColMenu.DropDownItems.Add(foo);

                foo = new ToolStripMenuItem("Visible");
                foo.Checked = dv.Visible;
                foo.Click += new EventHandler(ClickEventFunction);
                ColMenu.DropDownItems.Add(foo);



                MenuItem.DropDownItems.Add(ColMenu);

                return MenuItem;
            }



            if (MenuOrItem is MenuStrip)  // create new menu item on top level of menu strip
            {
                MenuStrip AttachMenu = MenuOrItem as MenuStrip;  // cast object as MenuStrip item

                //Menu_Remove(AttachMenu, NewMenuName);

                ToolStripMenuItem parentMenuItem = new ToolStripMenuItem(NewMenuName);  // new top level menu item

                foreach (string colname in GridColNames)  // create new submenu items and attach to parent Menu item
                {
                    ToolStripMenuItem foo = new ToolStripMenuItem(colname);

                    bool ColumnVisible = grid.Column_Visible(dv, colname);  // check to see if this column is currently visible on the grid
                    foo.Checked = ColumnVisible;   // check menu item if column is visible

                    //foo.Checked = 
                    foo.Click += new EventHandler(ClickEventFunction);
                    parentMenuItem.DropDownItems.Add(foo);
                }

                AttachMenu.Items.Add(parentMenuItem);

                return parentMenuItem;
            }


            //if (ParentName == "") { parentMenuItem.Text = dv.Name.ToString(); }  // rename the menu header the name of the datagridview control
            //if (ParentName != "") { parentMenuItem.Text = ParentName; }  // rename the menu header the name of the datagridview control

            return MenuItem;

        }


        #endregion

        #region === Modify Menu ===

        /// <summary>Add top level menu item to MenuStrip - returns ToolStripMenu name, option to overwrite existing dynamic menu</summary>
        /// <param name="mEnu"> </param>
        /// <param name="NewMenuItem"> </param>
        /// <param name="ReplaceOld"> </param>
        /// <param name="ClickEventFunction"> </param>
        public ToolStripMenuItem Add_Menu_Item(MenuStrip mEnu, string NewMenuItem, bool ReplaceOld = true, EventHandler ClickEventFunction = null)
        {
            if (ReplaceOld) { Menu_Remove(mEnu, NewMenuItem); } // remove a menu item/group

            ToolStripMenuItem foo = new ToolStripMenuItem(NewMenuItem);

            if (ClickEventFunction != null)
            {
                foo.Click += new EventHandler(ClickEventFunction);
            }

            mEnu.Items.Add(foo);
            return foo;
        }

        /// <summary>Add top level menu item to MenuStrip - returns ToolStripMenu name, option to overwrite existing dynamic menu</summary>
        /// <param name="parentMenuItem"> </param>
        /// <param name="NewMenuItemText"> </param>
        /// <param name="ClickEventFunction"> </param>
        public ToolStripMenuItem Add_Sub_Menu_Item(ToolStripMenuItem parentMenuItem, string NewMenuItemText, EventHandler ClickEventFunction = null)
        {
            //if (ReplaceOld) { Menu_Remove(mEnu, NewMenuItem); } // remove a menu item/group

            ToolStripMenuItem foo = new ToolStripMenuItem(NewMenuItemText);

            foo.Text = NewMenuItemText;

            if (ClickEventFunction != null)
            {
                foo.Click += new EventHandler(ClickEventFunction);
            }

            parentMenuItem.DropDownItems.Add(foo);
            return foo;
        }

        /// <summary>Remove a menu item/group</summary>
        /// <param name="menu"> </param>
        /// <param name="RemoveItemText"> </param>
        public void Menu_Remove(MenuStrip menu, string RemoveItemText)
        {
            foreach (ToolStripMenuItem child in menu.Items)
            {
                //ahk.MsgBox(child.Text); 

                if (child.Text == RemoveItemText)
                {
                    menu.Items.Remove(child);
                    break;
                }

                if (child.HasDropDownItems)
                {
                    foreach (object item in child.DropDownItems)
                    {
                        ToolStripMenuItem menuItem = item as ToolStripMenuItem;
                        if (menuItem == null)
                            continue;

                        if (menuItem.Text == RemoveItemText)
                        {
                            menu.Items.Remove(menuItem);
                            break;
                        }

                    }

                    //foreach (ToolStripMenuItem subchild in child.DropDownItems)
                    //{
                    //    if (subchild.Text == RemoveItemText)
                    //    {
                    //        menu.Items.Remove(subchild);
                    //        break;
                    //    }
                    //}
                }

            }

        }

        /// <summary>Toggle checked state of menu item and save checked state to ini</summary>
        /// <param name="menuItem"> </param>
        public void MenuItem_ToggleChecked_Save(ToolStripMenuItem menuItem)
        {
            // toggle checked state of menu item & save checked state to ini
            if (menuItem.Checked) { menuItem.Checked = false; }
            else { menuItem.Checked = true; }
            cv.MenuItem_Checked_Save(menuItem);  // saved checked state of menu item
        }

        /// <summary>Load previous checked state from ini</summary>
        /// <param name="menuItem"> </param>
        public void MenuItem_Checked_Last(ToolStripMenuItem menuItem)
        {
            cv.MenuItem_Checked_Last(menuItem);
        }


        #endregion

        #region === Menu Info ===

        /// <summary>Check to see if menu item exsts on MenuStrip</summary>
        /// <param name="menu"> </param>
        /// <param name="ItemName"> </param>
        public bool Menu_Exists(MenuStrip menu, string ItemName)
        {
            List<string> Items = Menu_Item_List(menu, false);
            foreach (string Item in Items)
            {
                if (Item == ItemName) { return true; }
            }
            return false;
        }

        /// <summary>List of the items in menu strip (option to only return top level items)</summary>
        /// <param name="menu">MenuStrip Name to Read List of Items From</param>
        /// <param name="TopLevelOnly">If True, Only Returns Top Visible Links (No Recurse)</param>
        /// <param name="MenuChildrenSearch">If Populated, Only Returns Children Under MenuText Search</param>
        public List<string> Menu_Item_List(MenuStrip menu, bool TopLevelOnly = false, string MenuChildrenSearch = "")
        {
            //// ex:
            //List<string> Menu = Menu_Top_Items_List(menuStrip1);
            //foreach (string item in Menu)
            //{ ahk.MsgBox(item); }

            List<string> menuItems = new List<string>();

            foreach (ToolStripMenuItem child in menu.Items)
            {
                //ahk.MsgBox(child.Text);

                if (MenuChildrenSearch != "") // searching for children items under menu name
                {
                    if (child.Text == MenuChildrenSearch)
                    {
                        if (child.HasDropDownItems)
                        {
                            foreach (ToolStripMenuItem subchild in child.DropDownItems)
                            {
                                menuItems.Add(subchild.Text);
                            }
                        }
                    }
                }
                else  // otherwise searching for all top level or all menu items 
                {
                    if (!TopLevelOnly) { menuItems.Add("| " + child.Text); }
                    else { menuItems.Add(child.Text); }

                    if (!TopLevelOnly)
                    {
                        if (child.HasDropDownItems)
                        {
                            foreach (ToolStripMenuItem subchild in child.DropDownItems)
                            {
                                menuItems.Add(subchild.Text);
                            }
                        }
                    }
                }
            }

            return menuItems;
        }


        //=== Populate TreeView with Menu Items

        /// <summary>Populate TreeView with Menu Item Listing</summary>
        /// <param name="menu"> </param>
        /// <param name="TV"> </param>
        /// <param name="Expand"> </param>
        /// <param name="clearTV"> </param>
        public void MenuItems_TreeView(MenuStrip menu, TreeView TV, bool Expand = true, bool clearTV = true)
        {
            if (clearTV) { TV.Nodes.Clear(); }

            TreeNode parent = new TreeNode();  // level 1
            parent.Text = menu.Name;

            List<ToolStripMenuItem> Parents = MenuStrip_Parent_Items(menu);

            foreach (ToolStripMenuItem Item in Parents)
            {
                TreeNode section = new TreeNode();  // level 2
                section.Text = Item.Text;
                section.Tag = Item.Name;
                parent.Nodes.Add(section);

                List<ToolStripMenuItem> Children = Menu_ToolStrip_Items(Item);

                foreach (ToolStripMenuItem subItem in Item.DropDownItems)
                {
                    TreeNode entry = new TreeNode();  // level 3
                    entry.Text = subItem.Text;
                    entry.Tag = subItem.Name;
                    section.Nodes.Add(entry);
                }

            }

            TV.Nodes.Add(parent);  // populate tree

            if (Expand) { TV.ExpandAll(); }
        }

        /// <summary>Returns MenuStrip control after locating on form</summary>
        /// <param name="theForm"> </param>
        /// <param name="MenuStripName"> </param>
        public MenuStrip Return_MenuStrip(Form theForm, string MenuStripName)
        {
            MenuStrip tbx = theForm.Controls.Find(MenuStripName, true).FirstOrDefault() as MenuStrip;   // find control from a winform saved in Dll
            return tbx;
        }


        /// <summary>Search menu items by display text, return toolstrip item with that text</summary>
        /// <param name="menu"> </param>
        /// <param name=" MenuItemText"> </param>
        public ToolStripMenuItem Return_ToolStripItem(MenuStrip menu, string MenuItemText)
        {
            foreach (ToolStripMenuItem child in menu.Items)
            {
                if (child.Text == MenuItemText) { return child; }

                foreach (ToolStripMenuItem kid in child.DropDownItems)
                {
                    if (kid.Text == MenuItemText) { return kid; }

                    if (kid.HasDropDownItems)
                    {
                        foreach (ToolStripMenuItem subchild in kid.DropDownItems)
                        {
                            if (subchild.Text == MenuItemText) { return subchild; }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>List of the ToolStrip items in Menu Strip (top level/parent items only)</summary>
        /// <param name="menu"> </param>
        public List<ToolStripMenuItem> MenuStrip_Parent_Items(MenuStrip menu)
        {
            List<ToolStripMenuItem> menuItems = new List<ToolStripMenuItem>();

            foreach (ToolStripMenuItem child in menu.Items)
            {
                //ahk.MsgBox(child.Text);
                menuItems.Add(child);
            }

            return menuItems;
        }

        /// <summary>List of the ToolStrip items in menu strip (under neath parent menu items)</summary>
        /// <param name="parentItem"> </param>
        public List<ToolStripMenuItem> Menu_ToolStrip_Items(ToolStripMenuItem parentItem)
        {
            List<ToolStripMenuItem> menuItems = new List<ToolStripMenuItem>();

            foreach (ToolStripMenuItem child in parentItem.DropDownItems)
            {
                //ahk.MsgBox(child.Text);
                menuItems.Add(child);

                if (child.HasDropDownItems)
                {
                    foreach (ToolStripMenuItem subchild in child.DropDownItems)
                    {
                        menuItems.Add(subchild);
                    }
                }
            }

            return menuItems;
        }

        #endregion

        #region === ToolStripMenu ====


        /// <summary>build new toolstrip menu (attaches to ToolStrip) - returns image list created/used</summary>
        /// <param name="Control form"> </param>
        /// <param name=" ToolStrip StripParent"> </param>
        /// <param name=" menuName"> </param>
        /// <param name=" List<string> menuItems"> </param>
        /// <param name=" EventHandler OnClick"> </param>
        /// <param name="iconDirPath"> </param>
        /// <param name="ImageList ImgList"> </param>
        public ImageList Build_ToolStripMenu(Control form, ToolStrip StripParent, string menuName, List<string> menuItems, EventHandler OnClick, string iconDirPath = "", ImageList ImgList = null)
        {
            if (form == null)
            {
                Form fromForm = ((Control)StripParent).FindForm();   // the Form the Control is located on 
                form = fromForm;
            }



            // populate image list

            ImageList imgList = null;

            if (iconDirPath != "")  // populate imagelist if icon dir path provided
            {
                imgList = img.ImageList(iconDirPath, true);
            }
            if (ImgList != null)   // otherwise if user provides an existing ImageList, use that
            {
                imgList = ImgList;
            }

            // return list of file names for image list to match with menu items
            List<string> imgNames = img.ImageList_FileNames(imgList, true, true);


            // extract menu name / text / icon name from input
            string MenuText = menuName; string MenuName = menuName; string MenuIcon = "";
            string MenuShortCut = "";

            if (menuName.Contains("|"))  // if menuName provided contains an icon name 
            {
                // split the menu item between the MenuName and the IconName
                MenuText = ahk.StringSplit(menuName, "|", 0);
                MenuIcon = ahk.StringSplit(menuName, "|", 1);


                MenuText = MenuText.Trim();
                MenuName = MenuText;
                MenuIcon = MenuIcon.Trim();
            }

            MenuName = ahk.StringReplace(MenuText, " "); // remove spaces for control name

            if (!menuName.Contains("|"))  // if icon file name isn't provided - find a close match
            {
                MenuIcon = ahk.Closest_FileName(MenuName, imgNames, false);  // match new menu item name with image list items, find closest filename match
            }



            // create menu item
            ToolStripSplitButton btnCreate = new ToolStripSplitButton(MenuText, img.From_ImageList(imgList, MenuIcon), ToolStripSplitButton_Click, MenuName);
            btnCreate.ButtonClick += ToolStripSplitButton_Click;  // assign event handlers that make clicking on the icon or text = show drop down menu options
            btnCreate.Click += ToolStripSplitButton_Click;
            btnCreate.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            StripParent.Items.Add(btnCreate);

            // create drop down menu items (from list)
            foreach (string item in menuItems)
            {
                string subText = item;
                string subName = item;
                string subIcon = "";
                string subShortcut = "";

                if (item.Contains("|"))
                {
                    // parse out potential paramenters pass in via list (Menu Text - Menu Icon - Menu Keyboard Shortcut)
                    string[] words = item.Split('|');
                    int i = 0;
                    foreach (string word in words)
                    {
                        if (i == 0) { subText = word; }
                        if (i == 1) { subName = word; }
                        if (i == 2) { subShortcut = word; }
                        i++;
                    }


                    //// split the menu item between the MenuName and the IconName
                    //subText = ahk.StringSplit(item, "|", 0);
                    //subIcon = ahk.StringSplit(item, "|", 1);

                    subText = subText.Trim();
                    subName = subText;
                    subIcon = subIcon.Trim();
                    subShortcut = subShortcut.Trim();
                }

                subName = ahk.StringReplace(subText, " "); // remove spaces for control name

                if (!item.Contains("|"))  // if icon file name isn't provided - find a close match
                {
                    subIcon = ahk.Closest_FileName(subName, imgNames, false);  // match new menu item name with image list items, find closest filename match
                }



                ToolStripMenuItem mEnu = new ToolStripMenuItem(subText, img.From_ImageList(imgList, subIcon), OnClick, subName); // create new drop down menu item



                if (subShortcut != "")  // user provided a shortcut for this menu item
                {

                    // parse the shortcut input text - assign control/alt/shift as needed, remove reference to those modifiers to reduce to single hotkey to assign to menu shortcut

                    bool Alt = false; bool Control = false; bool Shift = false;

                    string ShortKey = subShortcut.ToUpper();
                    ShortKey = ahk.StringReplace(ShortKey, "^");
                    ShortKey = ahk.StringReplace(ShortKey, "CONTROL");

                    ShortKey = ahk.StringReplace(ShortKey, "!");
                    ShortKey = ahk.StringReplace(ShortKey, "ALT");

                    ShortKey = ahk.StringReplace(ShortKey, "+");
                    ShortKey = ahk.StringReplace(ShortKey, "SHIFT");

                    if (subShortcut.Contains("^")) { Control = true; }  // control
                    if (subShortcut.Contains("!")) { Alt = true; }  // alt
                    if (subShortcut.Contains("+")) { Shift = true; }  // shift

                    if (subShortcut.ToUpper().Contains("CONTROL")) { Control = true; }  // control
                    if (subShortcut.ToUpper().Contains("ALT")) { Alt = true; }  // alt
                    if (subShortcut.ToUpper().Contains("SHIFT")) { Shift = true; }  // shift

                    ShortKey = ShortKey.Trim();

                    Keys key = (Keys)Enum.Parse(typeof(Keys), ShortKey.ToString(), ignoreCase: true);

                    mEnu.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | key)));
                }


                if (subText.Trim() != "-")
                {
                    btnCreate.DropDownItems.Add(mEnu);  // attach to parent ToolStripSplitButton
                }
                else
                {
                    ToolStripSeparator line = new ToolStripSeparator();
                    btnCreate.DropDownItems.Add(line);  // attach to parent ToolStripSplitButton
                }



            }

            // attach to toolstrip
            form.Controls.Add(StripParent);


            return imgList;
        }

        /// <summary>ToolString Button Click Event To Show Menu DropDown Contents On Click</summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        public void ToolStripSplitButton_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripSplitButton senderButton = (ToolStripSplitButton)sender;
                senderButton.ShowDropDown();
                //ahk.MsgBox(senderButton.Text);
            }
            catch { }
        }


        //###  Build ToolStrip - Add To Form or Control ###

        ToolStrip _sharpAHK_toolStrip;
        ImageList _toolStrip_ImageLisT;

        /// <summary>Creates new toolstrip + attaches to Form / Control</summary>
        /// <param name="formName"> </param>
        public ToolStrip Build_ToolStrip(Control formName)
        {
            // configure new ToolStripMenu with DropDownMenu Items (+ icons)

            Form stripForm = formName.FindForm();

            if (_sharpAHK_toolStrip == null)
            {
                ToolStrip sharpAHK_toolStrip = new ToolStrip();  // Assign ToolStrip To Attach To 

                // Create Menu Drop Down #1
                string NewMenuName = "File";
                List<string> menuItems = new List<string>() { "New", "Open|upload.png", "Save", "SaveAs", "Print", "Close", "Exit" };   //  "MenuName | IconName.png" format for Drop Down Items
                string iconDirPath = @"C:\Users\jason\Google Drive\ICO_Lib\FlatIcon\126759-linear-color-web-interface-elements";
                ImageList ImageLisT = Build_ToolStripMenu(formName, sharpAHK_toolStrip, NewMenuName, menuItems, ToolStripSplitButton_Click, iconDirPath);  // create menu - return new imagelist
                _toolStrip_ImageLisT = ImageLisT;

                // Create Menu Drop Down #2
                NewMenuName = "Options";
                menuItems = new List<string>() { "Select", "Add", "Remove", "Settings", "NPad|paper.png" };
                Build_ToolStripMenu(formName, sharpAHK_toolStrip, NewMenuName, menuItems, ToolStripSplitButton_Click, "", ImageLisT);  // create menu (blank icondir path - image list already created to reuse)

                //// Create Menu Drop Down #3
                //NewMenuName = "Add Controls";
                //menuItems = new List<string>() { "Button", "CheckBox", "TextBox", "DataGridView", "TreeView" };
                //Build_ToolStripMenu(formName, sharpAHK_toolStrip, NewMenuName, menuItems, ToolStripSplitButton_Click, "", ImageLisT);  // create menu (blank icondir path - image list already created to reuse)


                // configure toolstrip options
                sharpAHK_toolStrip.ShowItemToolTips = false;
                //sharpAHK_toolStrip.Dock = DockStyle.Top;
                sharpAHK_toolStrip.Stretch = true;


                _sharpAHK_toolStrip = sharpAHK_toolStrip;

                return _sharpAHK_toolStrip;
            }

            return null;
        }

        /// <summary>Creates new toolstrip + attaches to Form / Control (No Menu Items Added Yet)</summary>
        /// <param name="formName"> </param>
        public ToolStrip NewToolStrip(Control formName)
        {
            // configure new ToolStripMenu with DropDownMenu Items (+ icons)

            Form stripForm = formName.FindForm();

            if (_sharpAHK_toolStrip == null)
            {
                ToolStrip sharpAHK_toolStrip = new ToolStrip();  // Assign ToolStrip To Attach To 

                //// Create Menu Drop Down #1
                //string NewMenuName = "File";
                //List<string> menuItems = new List<string>() { "New", "Open|upload.png", "Save", "SaveAs", "Print", "Close", "Exit" };   //  "MenuName | IconName.png" format for Drop Down Items
                //string iconDirPath = @"C:\Users\jason\Google Drive\ICO_Lib\126759-linear-color-web-interface-elements";
                //ImageList ImageLisT = Build_ToolStripMenu(formName, sharpAHK_toolStrip, NewMenuName, menuItems, ToolStripSplitButton_Click, iconDirPath);  // create menu - return new imagelist
                //_toolStrip_ImageLisT = ImageLisT;

                //// Create Menu Drop Down #2
                //NewMenuName = "Options";
                //menuItems = new List<string>() { "Select", "Add", "Remove", "Settings", "NPad|paper.png" };
                //Build_ToolStripMenu(formName, sharpAHK_toolStrip, NewMenuName, menuItems, ToolStripSplitButton_Click, "", ImageLisT);  // create menu (blank icondir path - image list already created to reuse)

                //// Create Menu Drop Down #3
                //NewMenuName = "Add Controls";
                //menuItems = new List<string>() { "Button", "CheckBox", "TextBox", "DataGridView", "TreeView" };
                //Build_ToolStripMenu(formName, sharpAHK_toolStrip, NewMenuName, menuItems, ToolStripSplitButton_Click, "", ImageLisT);  // create menu (blank icondir path - image list already created to reuse)


                // configure toolstrip options
                sharpAHK_toolStrip.ShowItemToolTips = false;
                //sharpAHK_toolStrip.Dock = DockStyle.Top;
                sharpAHK_toolStrip.Stretch = true;


                _sharpAHK_toolStrip = sharpAHK_toolStrip;

                return _sharpAHK_toolStrip;
            }

            return null;
        }


        /// <summary>Add ToolStrip Item To Existing Toolstrip</summary>
        /// <param name="built_toolStrip"> </param>
        public void Add_To_ToolStrip(ToolStrip built_toolStrip)
        {
            Control stripParent = built_toolStrip.Parent;
            ToolStrip sharpAHK_toolStrip = built_toolStrip;

            // Create Menu Drop Down #1
            string NewMenuName = "Add Controls";
            List<string> menuItems = new List<string>() { "Button", "CheckBox", "TextBox", "DataGridView", "TreeView" };
            string iconDirPath = @"C:\Users\jason\Google Drive\ICO_Lib\FlatIcon\126759-linear-color-web-interface-elements";
            ImageList ImageLisT = Build_ToolStripMenu(stripParent, sharpAHK_toolStrip, NewMenuName, menuItems, ToolStripSplitButton_Click, iconDirPath);  // create menu - return new imagelist
            _toolStrip_ImageLisT = ImageLisT;

            _sharpAHK_toolStrip = sharpAHK_toolStrip;
        }

        /// <summary> </summary>
        /// <param name="built_toolStrip"> </param>
        /// <param name="ClickEvent"> </param>
        public void ToolStripAdd_Launch_Menu(ToolStrip built_toolStrip, EventHandler ClickEvent)
        {
            Control stripParent = built_toolStrip.Parent;
            Form stripForm = built_toolStrip.FindForm();
            ToolStrip sharpAHK_toolStrip = built_toolStrip;

            // Create Menu Drop Down #1
            string NewMenuName = "Launch";
            List<string> menuItems = new List<string>() { "AHK_Gui", "CSharp_CHM", "Dialog_Dev", "DirToDb", "Everything", "Function_Dev", "FunctionTags" };
            menuItems.Add("Grid_Dev");
            menuItems.Add("GridMenu");
            menuItems.Add("GUI_Edits");
            menuItems.Add("ImageListGen");
            menuItems.Add("ListView");
            menuItems.Add("MacroMaker");
            menuItems.Add("MenuDev");
            menuItems.Add("NPAD");
            menuItems.Add("SQLiter");
            menuItems.Add("SQLiter_NewTable");
            menuItems.Add("SQLiterGen");
            menuItems.Add("toolBar");
            menuItems.Add("TreeMenu");
            menuItems.Add("UserInput");
            menuItems.Add("WinControl");

            string iconDirPath = @"C:\Users\jason\Google Drive\ICO_Lib\FlatIcon\126759-linear-color-web-interface-elements";
            ImageList ImageLisT = Build_ToolStripMenu(stripParent, sharpAHK_toolStrip, NewMenuName, menuItems, ClickEvent, iconDirPath);  // create menu - return new imagelist
            _toolStrip_ImageLisT = ImageLisT;

            _sharpAHK_toolStrip = sharpAHK_toolStrip;
        }

        /// <summary> </summary>
        /// <param name="built_toolStrip"> </param>
        /// <param name="ClickEvent"> </param>
        /// <param name="MenuItems"> </param>
        /// <param name="ICODir"> </param>
        public void ToolStripAdd_List_Menu(ToolStrip built_toolStrip, EventHandler ClickEvent, List<string> MenuItems, string ICODir = "")
        {
            Control stripParent = built_toolStrip.Parent;
            Form stripForm = built_toolStrip.FindForm();
            ToolStrip sharpAHK_toolStrip = built_toolStrip;

            string NewMenuName = "ListMenu";

            if (ICODir == "")
            {
                ICODir = @"C:\Users\jason\Google Drive\ICO_Lib\FlatIcon\126759-linear-color-web-interface-elements";
            }

            ImageList ImageLisT = Build_ToolStripMenu(stripForm, sharpAHK_toolStrip, NewMenuName, MenuItems, ClickEvent, ICODir);  // create menu - return new imagelist
            _toolStrip_ImageLisT = ImageLisT;

            _sharpAHK_toolStrip = sharpAHK_toolStrip;
        }

        /// <summary>creates new toolstrip + attaches to Form / Control</summary>
        /// <param name="formName"> </param>
        /// <param name="ClickEvent"> </param>
        /// <param name="MenuItems"> </param>
        /// <param name="ICODir"> </param>
        public ToolStrip ListMenu_ToolStrip(Control formName, EventHandler ClickEvent, List<string> MenuItems, string ICODir = "")
        {
            // configure new ToolStripMenu with DropDownMenu Items (+ icons)

            Form stripForm = formName.FindForm();

            ToolStrip sharpAHK_toolStrip = new ToolStrip();  // Assign ToolStrip To Attach To 



            string NewMenuName = "ListMenu";

            if (ICODir == "")
            {
                ICODir = @"C:\Users\jason\Google Drive\ICO_Lib\FlatIcon\126759-linear-color-web-interface-elements";
            }

            ImageList ImageLisT = Build_ToolStripMenu(stripForm, sharpAHK_toolStrip, NewMenuName, MenuItems, ClickEvent, ICODir);  // create menu - return new imagelist
            _toolStrip_ImageLisT = ImageLisT;

            _sharpAHK_toolStrip = sharpAHK_toolStrip;



            // configure toolstrip options
            sharpAHK_toolStrip.ShowItemToolTips = false;
            //sharpAHK_toolStrip.Dock = DockStyle.Top;
            sharpAHK_toolStrip.Stretch = true;


            _sharpAHK_toolStrip = sharpAHK_toolStrip;

            return _sharpAHK_toolStrip;

        }


        #endregion

        #region === Control Menus ===
        //
        //
        //  collection of menu options that plug into any WinForm application to add additional options/ability to code on the fly
        //
        //

        // remove menu example
        //menu.Menu_Remove(menuStrip1, "Controls");

        // conditional create menu
        //if (!menu.Menu_Exists(menuStrip1, "Controls")) menu.Menu_Project_Control_Types(menuStrip1, ThisProject, true, "Controls");


        /// <summary>used for right-click menu</summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void ProjectControls_MouseClick(object sender, MouseEventArgs e)  // 
        {
            if (e.Button == MouseButtons.Right)  // right click detected
            {
                if (sender is DataGridView) { ahk.MsgBox("DataGridView Options"); }
                if (sender is Button) { ahk.MsgBox("Button Options"); }
                if (sender is TreeView) { ahk.MsgBox("TreeView Options"); }
                if (sender is TextBox) { ahk.MsgBox("TextBox Options"); }
                if (sender is TabControl) { ahk.MsgBox("TabControl Options"); }
                if (sender is TabPage) { ahk.MsgBox("TabPage Options"); }
                if (sender is ListBox) { ahk.MsgBox("ListBox Options"); }
                if (sender is MenuStrip) { ahk.MsgBox("MenuStrip Options"); }
                if (sender is TableLayoutPanel) { ahk.MsgBox("TableLayoutPanel Options"); }
                if (sender is CheckBox) { ahk.MsgBox("CheckBox Options"); }
                if (sender is Label) { ahk.MsgBox("Label Options"); }
                if (sender is PictureBox) { ahk.MsgBox("PictureBox Options"); }
                if (sender is Splitter) { ahk.MsgBox("Splitter Options"); }
                if (sender is Panel) { ahk.MsgBox("Panel Options"); }
                if (sender is ToolStripSeparator) { ahk.MsgBox("ToolStripSeparator Options"); }
                if (sender is SaveFileDialog) { ahk.MsgBox("SaveFileDialog Options"); }
                if (sender is BindingNavigator) { ahk.MsgBox("BindingNavigator Options"); }
                if (sender is OpenFileDialog) { ahk.MsgBox("OpenFileDialog Options"); }
                if (sender is WebBrowser) { ahk.MsgBox("WebBrowser Options"); }
                if (sender is ToolStripComboBox) { ahk.MsgBox("ToolStripComboBox Options"); }
                if (sender is ToolStripButton) { ahk.MsgBox("ToolStripButton Options"); }
                if (sender is ToolStripLabel) { ahk.MsgBox("ToolStripLabel Options"); }
                if (sender is ToolStripTextBox) { ahk.MsgBox("ToolStripTextBox Options"); }
                if (sender is RichTextBox) { ahk.MsgBox("RichTextBox Options"); }
                if (sender is ComboBox) { ahk.MsgBox("ComboBox Options"); }
                if (sender is ContextMenuStrip) { ahk.MsgBox("ContextMenuStrip Options"); }
                if (sender is CheckedListBox) { ahk.MsgBox("CheckedListBox Options"); }
                if (sender is DateTimePicker) { ahk.MsgBox("DateTimePicker Options"); }
                if (sender is MonthCalendar) { ahk.MsgBox("MonthCalendar Options"); }
                if (sender is FontDialog) { ahk.MsgBox("FontDialog Options"); }
                if (sender is PropertyGrid) { ahk.MsgBox("PropertyGrid Options"); }
                if (sender is ListView) { ahk.MsgBox("ListView Options"); }
                if (sender is ColorDialog) { ahk.MsgBox("ColorDialog Options"); }
                if (sender is NotifyIcon) { ahk.MsgBox("NotifyIcon Options"); }
                if (sender is RadioButton) { ahk.MsgBox("RadioButton Options"); }
                if (sender is Scintilla) { ahk.MsgBox("Scintilla Options"); }

                //if (sender is TreeViewFast) { ahk.MsgBox("TreeViewFast Options"); }
                //if (sender is ToolStripMenuItem) { ahk.MsgBox("ToolStripMenuItem Options"); }	  // No MouseClick Event For Menu Items
            }
        }

        /// <summary>
        /// Class for control under mouse detect
        /// </summary>
        public class MyClass : UserControl
        {
            /// <summary>
            /// Class for control under mouse detect
            /// </summary>
            public MyClass()
            {
                //InitializeComponent();

                MouseClick += ControlOnMouseClick;
                if (HasChildren)
                    AddOnMouseClickHandlerRecursive(Controls);
            }

            private void AddOnMouseClickHandlerRecursive(IEnumerable controls)
            {
                foreach (Control control in controls)
                {
                    control.MouseClick += ControlOnMouseClick;

                    if (control.HasChildren)
                        AddOnMouseClickHandlerRecursive(control.Controls);
                }
            }

            private void ControlOnMouseClick(object sender, MouseEventArgs args)
            {
                if (args.Button != MouseButtons.Right)
                    return;

                var contextMenu = new ContextMenu(new[] { new MenuItem("Copy", OnCopyClick) });
                contextMenu.Show((Control)sender, new Point(args.X, args.Y));
            }

            private void OnCopyClick(object sender, EventArgs eventArgs)
            {
                MessageBox.Show("Copy menu item was clicked.");
            }
        }


        #endregion

    }
}

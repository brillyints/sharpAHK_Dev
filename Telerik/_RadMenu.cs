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
        public class _RadMenu
        {
            _AHK ahk = new _AHK();
            _Lists lst = new _Lists();
            _Database.SQLite sqlite = new _Database.SQLite();

            RadMenu CurrentMenu;

            // Event Handler Action
            public void RadTreeMenu_Click(object sender, EventArgs e)
            {

            }


            #region === From Menu ===

            /// <summary>
            /// Return List of MenuItems from RadMenu
            /// </summary>
            /// <param name="radmenu"></param>
            /// <returns></returns>
            public List<RadMenuItem> MenuItems(RadMenu radmenu)
            {
                List<RadMenuItem> menuItems = new List<RadMenuItem>();

                foreach (RadMenuItem item in radmenu.Items)
                {
                    menuItems.Add(item);
                    if (item.Items.Count > 0)
                    {
                        foreach (RadMenuItem subItem in item.Items)
                        {
                            menuItems.Add(subItem);
                            if (subItem.Items.Count > 0)
                            {
                                foreach (RadMenuItem subSubItem in subItem.Items) { menuItems.Add(subSubItem); }
                            }
                        }
                    }
                }
                return menuItems;
            }

            /// <summary>
            /// Search RadMenu for RadMenuItem by MenuText
            /// </summary>
            /// <param name="radmenu">RadMenu to Search</param>
            /// <param name="MenuText">Menu Item Text to Find</param>
            /// <returns></returns>
            public RadMenuItem FindMenuItem(RadMenu RadMenu = null, string FindMenuText = "")
            {
                if (RadMenu != null)
                {
                    List<RadMenuItem> items = MenuItems(RadMenu);

                    foreach (RadMenuItem item in items)
                    {
                        if (item.Text == FindMenuText) { return item; }
                    }
                }
                return new RadMenuItem();
            }


            #endregion

            #region === Populate Menu ===

            public void Add_RadMenu_XMLDir(RadMenu RadMen, EventHandler ev, string xmlDir = "\\RadTreeXML")
            {
                if (RadMen == null) { return; }

                CurrentMenu = RadMen;

                string dir = xmlDir;

                if (dir.Trim() == "\\RadTreeXML") { dir = ahk.AppDir() + "\\RadTreeXML"; }

                // Populate RadMenu with list of XML Files Available to Load in RadTree
                RadMenu_FolderFiles(dir, ev, "*.xml", RadMen);
            }

            public RadMenuItem RadMenu_FolderFiles(string DirPath, EventHandler ev, string FileType = "*.*", RadMenu RadMenu = null)
            {
                RadMenuItem Main = new RadMenuItem();
                if (RadMenu == null) { return Main; }

                // attach to previously used 
                RadMenuItem ExistingCheck = FindMenuItem(RadMenu, DirPath.DirName());
                if (ExistingCheck != null)
                {
                    ExistingCheck.Items.Clear();
                    Main = ExistingCheck;
                }

                Main.Text = DirPath.DirName();

                List<string> files = lst.FileList(DirPath, FileType, true, true, false);
                if (files != null)
                {
                    if (RadMenu != null)
                    {
                        foreach (string file in files) { RadMenu_AddSubMenu(file, Main, ev); }
                        RadMenu.Items.Add(Main);
                    }
                }

                return Main;
            }

            public RadMenuItem RadMenu_FromList(List<string> Items, string MenuText, EventHandler ev, RadMenu RadMenu = null)
            {
                RadMenuItem Main = new RadMenuItem();
                if (RadMenu == null) { return Main; }

                // attach to previously used 
                RadMenuItem ExistingCheck = FindMenuItem(RadMenu, MenuText);
                if (ExistingCheck != null)
                {
                    ExistingCheck.Items.Clear();
                    Main = ExistingCheck;
                }

                Main.Text = MenuText;

                //List<string> files = lst.FileList(DirPath, FileType, true, true, false);
                if (Items != null)
                {
                    if (RadMenu != null)
                    {
                        foreach (string file in Items)
                        {
                            RadMenu_AddSubMenu(file, Main, ev);
                        }

                        RadMenu.Items.Add(Main);
                    }
                }

                return Main;
            }

            public void RadMenu_AddSubMenu(string SubMenuName, RadMenuItem RadMenuItem, EventHandler ev)
            {
                if (RadMenuItem == null) { return; }
                RadMenuItem Sub = new RadMenuItem(SubMenuName);
                Sub.Click += new EventHandler(ev);
                RadMenuItem.Items.Add(Sub);
            }

            #endregion 

            #region === Menu ComboBoxDDL ===

            public void RadMenu_ComboBox(RadMenu radMenu, List<string> Items, int selectedIndex = -1)
            {
                RadMenuItem bargainItem = new RadMenuItem("LITM");
                RadMenuComboItem comboItem = new RadMenuComboItem();

                foreach (string item in Items)
                {
                    RadListDataItem DockLaunch = new RadListDataItem();
                    //DockLaunch.ForeColor = Color.Green;
                    DockLaunch.Text = item;
                    comboItem.ComboBoxElement.Items.Add(DockLaunch);
                }

                if (selectedIndex != -1) { comboItem.ComboBoxElement.SelectedIndex = selectedIndex; }


                comboItem.ComboBoxElement.SelectedIndexChanged += new Telerik.WinControls.UI.Data.PositionChangedEventHandler(ComboBoxElement_SelectedIndexChanged);

                bargainItem.Items.Add(comboItem);
                radMenu.Items.Add(bargainItem);
            }

            public void ComboBoxElement_SelectedIndexChanged(object sender, EventArgs e)
            {
                RadListDataItem item = (sender as RadDropDownListElement).SelectedItem as RadListDataItem;
                //MessageBox.Show(item.Text);

                bool dockLaunch = false;
                bool newFormLaunch = false;
                bool newAppLaunch = false;

                if (item.Text == "Dock Launch") { dockLaunch = true; }
                if (item.Text == "New Form Launch") { newFormLaunch = true; }
                if (item.Text == "New App Launch") { newAppLaunch = true; }
            }


            #endregion


            // Search RadMenu for Menu Item by Text, Returns Menu Item if Found (null otherwise)
            public bool DeleteMenuItem(RadMenu RadMenu = null, string FindMenuText = "")
            {
                RadMenuItem foundMenu = new RadMenuItem();

                if (RadMenu != null)
                {
                    foreach (RadMenuItem men in RadMenu.Items)
                    {
                        if (men.Text == FindMenuText)
                        {
                            men.Items.Clear();
                            return true;
                        }
                    }
                }

                return false;
            }

            public RadMenuItem RadTreeMenu_Basics(EventHandler ev, RadMenu RadMenu)
            {
                RadMenuItem Main = new RadMenuItem();

                if (RadMenu == null) { return Main; }

                // attach to previously used 
                RadMenuItem ExistingCheck = FindMenuItem(RadMenu, "RadTree");
                if (ExistingCheck != null)
                {
                    ExistingCheck.Items.Clear();
                    Main = ExistingCheck;
                }

                Main.Text = "RadTree";

                List<string> items = new List<string> { "New Tree", "Open XML Dir", "Current TreeTag", "Save Tree", "Save As" };

                if (items != null)
                {
                    foreach (string item in items)
                    {
                        RadMenu_AddSubMenu(item, Main, ev);
                    }

                    if (RadMenu != null) { RadMenu.Items.Add(Main); }
                }

                return Main;
            }



            #region === Control Menus ===


            // not finished 
            public RadMenuItem Build_ScintillaMenu(EventHandler ev, RadMenu RadMenu, string MenuText = "NotePad")
            {
                RadMenuItem Main = new RadMenuItem();
                if (RadMenu == null) { return Main; }

                // attempt to attach to previously used 
                RadMenuItem ExistingCheck = FindMenuItem(RadMenu, MenuText);
                if (ExistingCheck != null)
                {
                    ExistingCheck.Items.Clear();
                    Main = ExistingCheck;
                }

                Main.Text = MenuText;

                List<string> items = new List<string> { "New", "Open", "Save", "Save As", "", "" };
                if (items != null)
                {
                    foreach (string item in items) { RadMenu_AddSubMenu(item, Main, ev); }
                    if (RadMenu != null) { RadMenu.Items.Add(Main); }
                }



                RadMenuItem View = new RadMenuItem();

                string NextMenuHeader = "View";
                // attempt to attach to previously used 
                ExistingCheck = FindMenuItem(RadMenu, NextMenuHeader);
                if (ExistingCheck != null)
                {
                    ExistingCheck.Items.Clear();
                    View = ExistingCheck;
                }

                View.Text = NextMenuHeader;

                items = new List<string> { "Fold", "UnFold", "", "Mode 1", "Mode 2", " Mode 3", "", "Show Tree", "Hide Tree" };
                if (items != null)
                {
                    foreach (string item in items) { RadMenu_AddSubMenu(item, View, ev); }
                    if (RadMenu != null) { RadMenu.Items.Add(View); }
                }




                return Main;
            }




            #endregion




        }

    }
}



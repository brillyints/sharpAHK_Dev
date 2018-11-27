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
        public class RadPages
        {

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

            public void SelectPage(RadPageView RadPageContainer, int PageNum = 1)
            {
                RadPageContainer.SelectedPage = RadPageContainer.Pages[PageNum];
            }


            /// <summary>
            /// Clear All Pages from RadPageView Container
            /// </summary>
            /// <param name="RadPageContainer"></param>
            public void Clear(RadPageView RadPageContainer)
            {
                RadPageContainer.Pages.Clear();
            }


            /// <summary>
            /// Removes Current Page from RadPageView Container
            /// </summary>
            /// <param name="RadPageContainer"></param>
            public void RemoveSelectedPage(RadPageView RadPageContainer)
            {
                RadPageContainer.Pages.Remove(RadPageContainer.SelectedPage);
            }



            #region === RadPages Code ===

            public static RadPageView radPage { get; set; }
            public static RadContextMenu contextMenu { get; set; }


            public void CreateContextMenu(RadPageView RadPage)
            {
                radPage = RadPage;

                //contextMenu = new RadContextMenu();

                RadMenuItem addNewTabMenuItem = new RadMenuItem();
                addNewTabMenuItem.Text = "Add New Tab";
                addNewTabMenuItem.Click += new EventHandler(addNewTabMenuItem_Click);
                contextMenu.Items.Add(addNewTabMenuItem);
                RadMenuSeparatorItem separator = new RadMenuSeparatorItem();
                contextMenu.Items.Add(separator);
                RadMenuItem closeTabMenuItem = new RadMenuItem();
                closeTabMenuItem.Text = "Close Tab";
                closeTabMenuItem.Click += new EventHandler(closeTabMenuItem_Click);
                contextMenu.Items.Add(closeTabMenuItem);
                RadMenuItem closeAllButThisMenuItem = new RadMenuItem();
                closeAllButThisMenuItem.Text = "Close All But This";
                closeAllButThisMenuItem.Click += new EventHandler(closeAllButThisMenuItem_Click);
                contextMenu.Items.Add(closeAllButThisMenuItem);
                RadMenuItem closeAllTabsMenuItem = new RadMenuItem();
                closeAllTabsMenuItem.Text = "Close All Tabs";
                closeAllTabsMenuItem.Click += new EventHandler(closeAllTabsMenuItem_Click);
                contextMenu.Items.Add(closeAllTabsMenuItem);
            }

            public void addNewTabMenuItem_Click(object sender, EventArgs e)
            {
                RadPageViewPage newPage = new RadPageViewPage();
                newPage.Text = "My new tab text";
                radPage.Pages.Add(newPage);
            }
            public void closeTabMenuItem_Click(object sender, EventArgs e)
            {
                radPage.Pages.Remove(radPage.SelectedPage);
            }
            public void closeAllButThisMenuItem_Click(object sender, EventArgs e)
            {
                for (int i = radPage.Pages.Count - 1; i >= 0; i--)
                {
                    if (radPage.Pages[i] != radPage.SelectedPage)
                    {
                        radPage.Pages.RemoveAt(i);
                    }
                }
            }
            public void closeAllTabsMenuItem_Click(object sender, EventArgs e)
            {
                radPage.Pages.Clear();
            }

            public void radPageView1_MouseClick(object sender, MouseEventArgs e)
            {
                RadPageViewItem hitItem = radPage.ViewElement.ItemFromPoint(e.Location);
                if (e.Button == MouseButtons.Right && hitItem != null)
                {
                    contextMenu.Show(radPage.PointToScreen(e.Location));
                }
            }


            #endregion

        }

    }
}



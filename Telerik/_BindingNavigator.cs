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
        public class _BindingNavigator
        {
            //SetupRadNav(radBindingNavigator1);
            private Telerik.WinControls.UI.CommandBarTextBox radBindingNavigator1PositionItem;
            private Telerik.WinControls.UI.RadBindingNavigator radBindingNavigator1;
            private Telerik.WinControls.UI.CommandBarRowElement radBindingNavigator1RowElement;
            private Telerik.WinControls.UI.CommandBarStripElement radBindingNavigator1FirstStrip;
            private Telerik.WinControls.UI.CommandBarButton radBindingNavigator1MoveFirstItem;
            private Telerik.WinControls.UI.CommandBarButton radBindingNavigator1MovePreviousItem;
            private Telerik.WinControls.UI.CommandBarLabel radBindingNavigator1CountItem;
            private Telerik.WinControls.UI.CommandBarButton radBindingNavigator1MoveNextItem;
            private Telerik.WinControls.UI.CommandBarButton radBindingNavigator1MoveLastItem;
            private Telerik.WinControls.UI.CommandBarButton radBindingNavigator1AddNewItem;
            private Telerik.WinControls.UI.CommandBarButton radBindingNavigator1DeleteItem;


            #region === Binding Navigator ===

            List<string> resultLines = new List<string>();
            public static int ResultNum { get; set; }



        // Event Handler Action
        public void LoadItem_Click(object sender, EventArgs e)
            {
                LoadNavAction(GetCurrentPos()); // Load Current Item Action
            }

            // Action to Perform Loading Media when Nav Item Changes
            public void LoadNavAction(int ItemNum = 1)
            {
                //string updateText = lst.Return_List_Value(resultLines, ItemNum);
                //txtResults.Text = updateText;
                //btnNextResult.Text = "Next (" + ItemNum + "/" + resultLines.Count + ")";

                // parse lines into radtree display
                //radTreeParsed.ClearTree();
                //List<string> lines = lst.Text_To_List(updateText, true, true, false);
                //tree.ListRadTree(radTreeParsed, lines);
            }



            public void SetupRadNav(RadBindingNavigator RadNavBar)
            {
                RadNav = RadNavBar;

                string ItemName = "Item";

                RadNext.NextButton = radBindingNavigator1MoveNextItem;
                RadNext.NextButton.ToolTipText = "Next " + ItemName;
                RadNext.NextButton.Click += new System.EventHandler(RadBindingNext_Click);

                RadNext.PreviousButton = radBindingNavigator1MovePreviousItem;
                RadNext.PreviousButton.ToolTipText = "Previous " + ItemName;
                RadNext.PreviousButton.Click += new System.EventHandler(RadBindingPrevious_Click);

                RadNext.FirstButton = radBindingNavigator1MoveFirstItem;
                RadNext.FirstButton.ToolTipText = "First " + ItemName;
                RadNext.FirstButton.Click += new System.EventHandler(RadBindingFirst_Click);

                RadNext.LastButton = radBindingNavigator1MoveLastItem; ;  //radBindingNavigator1MoveLastItem;
                RadNext.LastButton.ToolTipText = "Last " + ItemName;
                RadNext.LastButton.Click += new System.EventHandler(RadBindingLast_Click);
            }

            RadBindingNavigatorElement RadNext = new RadBindingNavigatorElement();
            RadBindingNavigator RadNav = new RadBindingNavigator();

            public static EventHandler ItemAction_Event { get; set; }

            // Next
            public void RadBindingNext_Click(object sender, EventArgs e)
            {
                ResultNum = GetCurrentPos(); ResultNum++;

                if (ResultNum > resultLines.Count) { ResultNum = 1; }  // if at last, loop back to first

                LoadNavAction(ResultNum); // load results

                CurrentPos(ResultNum); // set current number in navigator bar
            }

            // Previous
            public void RadBindingPrevious_Click(object sender, EventArgs e)
            {
                ResultNum = GetCurrentPos(); ResultNum--;

                if (ResultNum < 1) { ResultNum = resultLines.Count; }  // if at zero, set current to last item

                LoadNavAction(ResultNum); // load results

                CurrentPos(ResultNum); // set current number in navigator bar
            }

            // First Item in Nav
            private void RadBindingFirst_Click(object sender, EventArgs e)
            {
                ResultNum = 1; // current number = first

                LoadNavAction(ResultNum); // load results

                CurrentPos(ResultNum); // set current number in navigator bar
            }


            // Last Item in Nav
            public void RadBindingLast_Click(object sender, EventArgs e)
            {
                ResultNum = resultLines.Count;  // current number = last

                LoadNavAction(ResultNum); // load results

                CurrentPos(ResultNum); // set current number in navigator bar
            }


            // Set Current Number Display
            public void CurrentPos(int CurrentNum)
            {
                radBindingNavigator1PositionItem.Text = CurrentNum.ToString();
            }

            /// <summary>
            /// Returns Value From Current Number Display 
            /// </summary>
            /// <returns></returns>
            public int GetCurrentPos()
            {
                string current = radBindingNavigator1PositionItem.Text.Trim();

                if (current == "") { return -1; }

                return radBindingNavigator1PositionItem.Text.Trim().ToInt();
            }


            // Set Total Number Display
            public void SetNavTotal(int TotalNum = 10)
            {
                radBindingNavigator1CountItem.Text = TotalNum.ToString();
            }



            // User Value in FreeText Field
            private void radBindingNavigator1PositionItem_TextChanged(object sender, EventArgs e)
            {
                ResultNum = GetCurrentPos();  // get user value

                if (ResultNum != -1) // invalid value in text field
                {
                    LoadNavAction(ResultNum); // load results

                    radBindingNavigator1PositionItem.Text = ResultNum.ToString();  // put back value without spaces
                }
            }



#endregion


        }

    }
}





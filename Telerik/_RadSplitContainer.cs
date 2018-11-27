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
        public class _RadSplitContainer
        {
            //// ex: 

            //// Enable Collapsing + Hide SplitPanel
            //  radSplitContainer1.UseSplitterButtons = true;
            //  radSplitContainer1.EnableCollapsing = true;
            //  splitPanel1.Collapsed = true;

             // if (txt == "Show Tree") { splitPanel1.Collapsed = false; }
             // if (txt == "Hide Tree") { splitPanel1.Collapsed = true; }


            /// <summary>
            ///  You can programmatically build a layout of panels using RadSplitContainer. To do so, refer to the code snippet below:
            /// </summary>
            /// <param name="RForm">RadForm to Add Controls To</param>
            public void BuildExample(RadForm RForm)
                    {
                        RadSplitContainer container = new RadSplitContainer();
                        container.Dock = DockStyle.Fill;
                        //left panel, sized absolutely   
                        RadSplitContainer leftContainer = new RadSplitContainer();
                        leftContainer.Orientation = Orientation.Horizontal;
                        leftContainer.SizeInfo.SizeMode = Telerik.WinControls.UI.Docking.SplitPanelSizeMode.Absolute;
                        leftContainer.SizeInfo.AbsoluteSize = new Size(150, 150);
                        container.SplitPanels.Add(leftContainer);
                        //middle panel, auto-sized   
                        SplitPanel middlePanel = new SplitPanel();
                        middlePanel.SplitPanelElement.Fill.BackColor = Color.Pink;
                        container.SplitPanels.Add(middlePanel);
                        //left panel, sized absolutely   
                        RadSplitContainer rightContainer = new RadSplitContainer();
                        rightContainer.Orientation = Orientation.Horizontal;
                        rightContainer.SizeInfo.SizeMode = Telerik.WinControls.UI.Docking.SplitPanelSizeMode.Absolute;
                        rightContainer.SizeInfo.AbsoluteSize = new Size(150, 150);
                        container.SplitPanels.Add(rightContainer);
                        //add panels 4 & 5   
                        SplitPanel leftTopPanel = new SplitPanel();
                        leftContainer.SplitPanelElement.Fill.BackColor = Color.Yellow;
                        leftTopPanel.SizeInfo.SizeMode = Telerik.WinControls.UI.Docking.SplitPanelSizeMode.Absolute;
                        leftTopPanel.SizeInfo.AbsoluteSize = new Size(150, 150);
                        leftContainer.SplitPanels.Add(leftTopPanel);
                        SplitPanel leftBottomPanel = new SplitPanel();
                        leftBottomPanel.SplitPanelElement.Fill.BackColor = Color.Green;
                        leftContainer.SplitPanels.Add(leftBottomPanel);
                        //add panels 6 & 7   
                        SplitPanel rightTopPanel = new SplitPanel();
                        rightTopPanel.SplitPanelElement.Fill.BackColor = Color.Red;
                        rightTopPanel.SizeInfo.SizeMode = Telerik.WinControls.UI.Docking.SplitPanelSizeMode.Absolute;
                        rightTopPanel.SizeInfo.AbsoluteSize = new Size(150, 150);
                        rightContainer.SplitPanels.Add(rightTopPanel);
                        SplitPanel rightBottomPanel = new SplitPanel();
                        rightBottomPanel.SplitPanelElement.Fill.BackColor = Color.Lime;
                        rightContainer.SplitPanels.Add(rightBottomPanel);
                        RForm.Controls.Add(container);
                    }


        }

    }
}




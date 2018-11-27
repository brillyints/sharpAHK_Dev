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
        public class RadProgress
        {
            /// <summary>
            /// Update ProgressBar Text (From any Thread) 
            /// </summary>
            /// <param name="PBar"></param>
            /// <param name="Text"></param>
            public void ProgressText(RadProgressBar PBar, string Text = "")
            {
                if (PBar == null) { return; }

                if (PBar.InvokeRequired)  // if currently on a different thread, invoke 
                {
                    PBar.BeginInvoke((MethodInvoker)delegate ()
                    {
                        if (Text != "") { PBar.Text = Text; }
                    });
                }
                else
                {
                    if (Text != "") { PBar.Text = Text; }
                }
            }

            /// <summary>
            /// Update Progress Bar By 1 Unit (Option to Update Text)
            /// </summary>
            /// <param name="PBar">RadProgressBar Control</param>
            /// <param name="Text">Optional Display Text</param>
            public void UpdateProgress(RadProgressBar PBar, string Text = "")
            {
                if (PBar == null) { return; }

                if (PBar.InvokeRequired)  // if currently on a different thread, invoke 
                {
                    PBar.BeginInvoke((MethodInvoker)delegate ()
                    {
                        if (Text != "") { PBar.Text = Text; }

                        try { PBar.Value1++; }
                        catch { }
                    });
                }
                else
                {
                    if (Text != "") { PBar.Text = Text; }

                    try { PBar.Value1++; }
                    catch { }
                }
            }

            /// <summary>
            /// Reset Progress Bar Status to Blank
            /// </summary>
            /// <param name="PBar">RadProgressBar Control</param>
            public void ResetProgress(RadProgressBar PBar)
            {
                if (PBar == null) { return; }

                if (PBar.InvokeRequired)  // if currently on a different thread, invoke 
                {
                    PBar.BeginInvoke((MethodInvoker)delegate ()
                    {
                        try { PBar.Value1 = 0; }
                        catch { }
                    });
                }
                else
                {
                    try { PBar.Value1 = 1; }
                    catch { }

                }
            }

            /// <summary>
            /// Configure Progress Bar Values (Resets Existing Values to Blank)
            /// </summary>
            /// <param name="PBar">RadProgressBar Control</param>
            /// <param name="Max">Number of Units ProgressBar will Display</param>
            public void SetupProgressBar(RadProgressBar PBar, int Max = 100)
            {
                if (PBar == null) { return; }

                PBar.ForeColor = Color.Red;
                PBar.ProgressBarElement.IndicatorElement1.BackColor = Color.LightBlue;
                PBar.ProgressBarElement.IndicatorElement1.DrawBorder = true;
                PBar.ProgressBarElement.IndicatorElement1.BorderColor = Color.Green;
                PBar.ProgressBarElement.IndicatorElement1.BorderGradientStyle = Telerik.WinControls.GradientStyles.Solid;

                PBar.Maximum = Max;

                ResetProgress(PBar);
            }


        }

    }
}





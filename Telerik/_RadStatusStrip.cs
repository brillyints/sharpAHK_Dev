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

        public static RadStatusStrip StatStrip { get; set; }
        public static RadLabelElement StripElement { get; set; }

        public void sb(string Text = "", RadStatusStrip radstatstrip = null, RadLabelElement radlabelelement = null)
        {
            if (radstatstrip != null) { StatStrip = radstatstrip; }
            if (radlabelelement != null) { StripElement = radlabelelement; }

            if (StatStrip != null && StripElement != null)
            {
                if (StatStrip.InvokeRequired)  // if currently on a different thread, invoke 
                {
                    StatStrip.BeginInvoke((MethodInvoker)delegate () { StripElement.Text = Text; });
                }
                else
                {
                    StripElement.Text = Text;
                }
            }
        }


        public class RadStatusBar
        {
        }

    }
}




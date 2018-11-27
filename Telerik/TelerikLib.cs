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
        #region === Startup ===

        _Database.goDaddy goDad = new _Database.goDaddy();
        sharpAHK._AHK ahk = new sharpAHK._AHK();
        _Database.SQL sql = new _Database.SQL();
        _Database.SQLite sqlite = new _Database.SQLite();
        _Lists lst = new _Lists();
        //_Dict dict = new _Dict();

        #endregion

        #region === Form Update ===

        public void Update(Control Control, object update = null)
        {
            if (Control.GetType().ToString() == "Telerik.WinControls.UI.RadRichTextEditor")
            {
                RadRichTextEditor txt = (RadRichTextEditor)Control; // editor to update passed by user

                string Text = ""; if (update != null) { Text = update.ToString(); }  // optional field used to pass text to populate

                if (txt.InvokeRequired)  // if currently on a different thread, invoke 
                {
                    txt.BeginInvoke((MethodInvoker)delegate () { txt.Text = Text; });
                }
                else
                {
                    txt.Text = Text; // Update Control Text
                }

                return;
            }

            if (Control.GetType().ToString() == "Telerik.WinControls.UI.RadLabel")
            {
                RadLabel txt = (RadLabel)Control; // editor to update passed by user

                string Text = ""; if (update != null) { Text = update.ToString(); }  // optional field used to pass text to populate

                if (txt.InvokeRequired)  // if currently on a different thread, invoke 
                {
                    txt.BeginInvoke((MethodInvoker)delegate () { txt.Text = Text; });
                }
                else
                {
                    txt.Text = Text; // Update Control Text
                }

                return;
            }

            if (Control.GetType().ToString() == "Telerik.WinControls.UI.RadButton")
            {
                RadButton txt = (RadButton)Control; // editor to update passed by user

                string Text = ""; if (update != null) { Text = update.ToString(); }  // optional field used to pass text to populate

                if (txt.InvokeRequired)  // if currently on a different thread, invoke 
                {
                    txt.BeginInvoke((MethodInvoker)delegate () { txt.Text = Text; });
                }
                else
                {
                    txt.Text = Text; // Update Control Text
                }

                return;
            }

            if (Control.GetType().ToString() == "Telerik.WinControls.UI.RadTextBox")
            {
                RadTextBox txt = (RadTextBox)Control; // editor to update passed by user

                string Text = ""; if (update != null) { Text = update.ToString(); }  // optional field used to pass text to populate

                if (txt.InvokeRequired)  // if currently on a different thread, invoke 
                {
                    txt.BeginInvoke((MethodInvoker)delegate () { txt.Text = Text; });
                }
                else
                {
                    txt.Text = Text; // Update Control Text
                }

                return;
            }

            if (Control.GetType().ToString() == "Telerik.WinControls.UI.RadCheckBox")
            {
                RadCheckBox txt = (RadCheckBox)Control; // editor to update passed by user

                bool CheckVal = (bool)update;

                if (txt.InvokeRequired)  // if currently on a different thread, invoke 
                {
                    txt.BeginInvoke((MethodInvoker)delegate () { txt.Checked = CheckVal; });
                }
                else
                {
                    txt.Checked = CheckVal; // Update Control Text
                }

                return;
            }

            if (Control.GetType().ToString() == "IntegrationDataUpdate.IntegrationApp")
            {
                Form txt = (Form)Control; // editor to update passed by user

                string Text = ""; if (update != null) { Text = update.ToString(); }  // optional field used to pass text to populate

                if (txt.InvokeRequired)  // if currently on a different thread, invoke 
                {
                    txt.BeginInvoke((MethodInvoker)delegate () { txt.Text = Text; });
                }
                else
                {
                    txt.Text = Text; // Update Control Text
                }

                return;
            }

            if (Control.GetType().ToString() == "Telerik.WinControls.UI.RadBindingNavigator")
            {
                RadBindingNavigator txt = (RadBindingNavigator)Control; // editor to update passed by user

                string Text = ""; if (update != null) { Text = update.ToString(); }  // optional field used to pass text to populate

                if (txt.InvokeRequired)  // if currently on a different thread, invoke 
                {
                    txt.BeginInvoke((MethodInvoker)delegate () { txt.Text = Text; });
                }
                else
                {
                    txt.Text = Text; // Update Control Text
                }

                return;
            }

            //if (Control.GetType().ToString() == "Telerik.WinControls.UI.RadLabelElement")
            //{
            //    RadLabelElement txt = (RadLabelElement)Control; // editor to update passed by user

            //    string Text = ""; if (update != null) { Text = update.ToString(); }  // optional field used to pass text to populate

            //    if (txt.InvokeRequired)  // if currently on a different thread, invoke 
            //    {
            //        txt.BeginInvoke((MethodInvoker)delegate () { txt.Text = Text; });
            //    }
            //    else
            //    {
            //        txt.Text = Text; // Update Control Text
            //    }

            //    return;
            //}

            

            ahk.MsgBox("Control Type: " + Control.GetType().ToString() + " Not Defined To Update YET");

        }

        public void UpdateGrid(RadGridView RadGrid, DataTable datasource)
        {
            if (RadGrid == null) { return; }

            if (RadGrid.InvokeRequired)  // if currently on a different thread, invoke 
            {
                RadGrid.BeginInvoke((MethodInvoker)delegate () { RadGrid.DataSource = datasource; });
            }
            else
            {
                RadGrid.DataSource = datasource;
            }

            RadGrid rgrd = new RadGrid();
            rgrd.Fill_Column_Width(RadGrid);
        }
        public void UpdateLabel(RadLabel lb, string Text)
        {
            if (lb == null) { return; }

            if (lb.InvokeRequired)  // if currently on a different thread, invoke 
            {
                lb.BeginInvoke((MethodInvoker)delegate () { lb.Text = Text; });
            }
            else
            {
                lb.Text = Text;
            }
        }


        /// <summary>
        /// Update RadTextBox Text From Any Thread
        /// </summary>
        /// <param name="lb"></param>
        /// <param name="Text"></param>
        public void UpdateText(RadTextBox lb, string Text = "")
        {
            if (lb == null) { return; }

            if (lb.InvokeRequired)  // if currently on a different thread, invoke 
            {
                lb.BeginInvoke((MethodInvoker)delegate () { lb.Text = Text; });
            }
            else
            {
                lb.Text = Text;
            }
        }

        /// <summary>
        /// Return Text from RadTextBox from Any Thread
        /// </summary>
        /// <param name="lb">Name of RadTextBox to Read</param>
        /// <returns></returns>
        public string ReturnText(RadTextBox lb)
        {
            string returnText = "";

            if (lb == null) { return ""; }

            if (lb.InvokeRequired)  // if currently on a different thread, invoke 
            {
                lb.BeginInvoke((MethodInvoker)delegate () { returnText = lb.Text; });
            }
            else
            {
                returnText = lb.Text;
            }

            return returnText;
        }

        public void UpdateRTF(RadRichTextEditor la, string Text)
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
        public void UpdateButton(RadButton la, string Text)
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
        public void UpdatePanel(RadCollapsiblePanel la, string Text)
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

        #endregion


     }


    public class DirObj
    {
        public DirObj(string DirName, string DirPath)
        {
            _DirName = DirName;
            _DirPath = DirPath;
        }

        private string _DirName;
        public string DirName
        {
            get { return _DirName; }
            set { _DirName = value; }
        }

        private string _DirPath;
        public string DirPath
        {
            get { return _DirPath; }
            set { _DirPath = value; }
        }
    }




}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sharpAHK;
using System.Drawing;
using ScintillaNET;
using System.ComponentModel;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Drawing.Imaging;
using System.Xml.Serialization;
using System.Threading;
using System.Reflection;
using System.Net;
using System.Windows.Automation;
using System.Timers;
using System.Configuration;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Globalization;
using Telerik.WinForms;
using Telerik.WinControls.UI;
using TelerikExpressions;
using System.Net.Mail;
using Telerik.WinForms.Documents.Model.Code;
using System.Web.Script.Serialization;
using AHKExpressions;
using System.Xml;
using HtmlAgilityPack;
using System.Web;
using sharpAHK_Dev;

namespace sharpAHK_Dev
{
    public partial class _Prompts
    {

        /// <summary>
        /// Launches New List Editor Dialog with Ability to Edit / Save User Lists to SQLite Db
        /// </summary>
        /// <param name="ListName">Optional Name of List to Populate Editor on Startup</param>
        /// <example>
        ///             _Database.SQLite sqlite = new _Database.SQLite();
        ///             sqlite.ListEditor();
        /// </example>
        public void ListEditor(string ListName = "")
        {
            sharpAHK_Dev.Controls.listEditor form = new sharpAHK_Dev.Controls.listEditor(ListName);
            form.Show();
        }


        /// <summary>
        /// Yes / No Dialog
        /// </summary>
        /// <param name="PromptText"></param>
        /// <param name="TitleText"></param>
        /// <example>
        ///             _Prompts prompt = new _Prompts();
        ///             bool Yes = prompt.YesNoPrompt("Play a Game?", "Yes/No");
        ///             if (Yes) { }
        ///             else { }
        /// </example>
        /// <returns></returns>
        public bool YesNoPrompt(string PromptText = "", string TitleText = "")
        {
            sharpAHK_Dev.Controls.userPrompt form = new Controls.userPrompt(PromptText, TitleText);

            DialogResult dialogResult = form.ShowDialog();

            //_AHK ahk = new _AHK();
            //ahk.MsgBox("Clicked " + dialogResult.ToString());

            if (dialogResult.ToString().ToUpper() == "OK") { return true; }
            return false;
        }


        /// <summary>
        /// Launches New Scintilla Dialog with Ability to Edit / Save Wordlists
        /// </summary>
        /// <param name="sci">Scintilla Control</param>
        /// <param name="Text">Optional Text to Populate Control</param>
        public void WordListEditor(Scintilla sci, string Text = "")
        {
            sharpAHK_Dev.Controls.scintillaWords form = new sharpAHK_Dev.Controls.scintillaWords(sci, Text);
            form.Show();
        }



    }
}

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
using System.Web;
using System.Configuration;
using System.Net.Mail;
using Telerik.WinControls.UI;
using Telerik.WinForms.Documents.Model.Code;
using System.Collections;
using ScintillaNET;
using sharpAHK_Dev;
using TelerikExpressions;
using Telerik.WinControls;
using static sharpAHK_Dev._Apps.Mpc;

namespace DevExpressions
{
    public static class DevExpressions
    {

        #region === MediaPlayerClassic ===

        public enum Actions
        {
            PlayPause,
            Next,
            Previous,
            Forward,
            Backward,
            Close,
            VolumeUp,
            VolumeDown,
            Mute,
            Zoom100,
            Zoom200,
            FullScreen,
            Q1,
            Q2,
            Q3,
            Q4,
            GoTo15
        }

        /// <summary>
        /// Clear Contents of TreeView (From Any Thread)
        /// </summary>
        /// <param name="RadTree">RadTreeView Control To Clear</param>
        public static void PlayPause(this mpcWin Player)
        {
            _AHK ahk = new _AHK();
            _Apps.Mpc mpc = new _Apps.Mpc();
            ahk.MsgBox(Player.WinHwnd.ToString());
            mpc.MPC_Actions(Player.WinHwnd, _Apps.Mpc.Actions.PlayPause);
        }



        #endregion

    }
}

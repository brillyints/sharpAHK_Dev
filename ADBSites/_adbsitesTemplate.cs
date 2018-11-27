using System.Windows.Forms;
using sharpAHK;
using sharpAHK_Dev;
using AHKExpressions;
using TelerikExpressions;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading;
using Telerik.WinControls.UI;
using System.Data;
using System.Collections.Generic;
using System;
using System.IO;


namespace sharpAHK_Dev
{
    public partial class ADBSites
    {
        public class _adbsitesTemplate
        {
            _AHK ahk = new _AHK();
            _Lists lst = new _Lists();
            _Database.SQL sql = new _Database.SQL();
            _Database.SQLite sqlite = new _Database.SQLite();
            _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();
            _TelerikLib tel = new _TelerikLib();



        }
    }

}



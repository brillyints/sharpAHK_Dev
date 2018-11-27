using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.WinControls.UI;
using sharpAHK;
using AHKExpressions;
using static sharpAHK_Dev._Sites.SceneSource;
using System.IO;
using System.Diagnostics;
using TelerikExpressions;
using System.Timers;
using System.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Text.RegularExpressions;
using static sharpAHK_Dev._Web.DL_Accounts;
using System.Windows.Forms;
using System.Net;


namespace sharpAHK_Dev
{
    public partial class _Sites
    {

        public class Facebook
        {
            private static _AHK ahk = new sharpAHK._AHK();
            //_Database.SQL sql = new _Database.SQL();
            //_Database.SQLite sqlite = new _Database.SQLite();

            public void graphAPI()
            {
                ahk.Run(@"https://developers.facebook.com/tools/explorer/145634995501895/?method=GET&path=1559171587653986%2Ffeed&version=v2.3&");
            }


        }


    }
}

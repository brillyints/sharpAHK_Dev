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
using sharpAHK_Dev;
using System.Net.Mail;
using Telerik.WinControls.UI;
using System.Collections;
using Telerik.WinControls;
using TelerikExpressions;
using AHKExpressions;

namespace sharpAHK_Dev
{
    class TelerikWeb
    {
        #region === Startup ===

        
        _Database.goDaddy goDad = new _Database.goDaddy();
        sharpAHK._AHK ahk = new sharpAHK._AHK();
        _Database.SQL sql = new _Database.SQL();
        _Database.SQLite sqlite = new _Database.SQLite();
        _GridControl grid = new _GridControl();
        _Lists lst = new _Lists();
        _Dict dict = new _Dict();
        _Images img = new _Images();
        _WinForms frm = new _WinForms();
        _TreeViewControl tv = new _TreeViewControl();
        _TabControl tab = new _TabControl();
        _MenuControl menu = new _MenuControl();
        _ScintillaControl sci = new _ScintillaControl();
        _Apps.Mpc mpc = new _Apps.Mpc();
        _Apps.Chrome cr = new _Apps.Chrome();
        _Code code = new _Code();
        _Database.Tags tags = new _Database.Tags();
        _Database.dir2Db dir2 = new _Database.dir2Db();
        _Parse prs = new _Parse();
        //_imdb im = new _imdb();
        //_Web web = new _Web();
        //_StatusBar sb = new _StatusBar();
        //_RapidGator rg = new _RapidGator();
        //_EpGuides ep = new _EpGuides();
        //FtP ftp = new FtP();
        _Database.dir2Db dirDb = new _Database.dir2Db();
        _StatusBar sb = new _StatusBar();
        _Sites.Imdb imdb = new _Sites.Imdb();
        _TelerikLib tel = new _TelerikLib();
        _TelerikLib.RadTree tree = new _TelerikLib.RadTree();
        _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();
        _Sites.MovieParadise mp = new _Sites.MovieParadise();


        #endregion



    }
}

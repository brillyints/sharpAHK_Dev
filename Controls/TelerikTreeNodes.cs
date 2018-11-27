using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using sharpAHK;
using sharpAHK_Dev;
using System.Data.SqlClient;
using System.Configuration;
using System.Threading;

namespace sharpAHK_Dev
{
    public partial class TelerikTreeNodes : UserControl
    {

        #region === Startup ===

        _AHK ahk = new _AHK();
        _TelerikLib._RadMenu menu = new _TelerikLib._RadMenu();
        _TelerikLib.RadTree tree = new _TelerikLib.RadTree();


        public TelerikTreeNodes()
        {
            InitializeComponent();

//            RadTree = radTreeControl.RadTREE; // assign radtree control from TelerikTree Control
        }

        RadTreeView RadTree = new RadTreeView();

        public TelerikTree RadTreeControl
        {
            get { return radTreeControl; }
            set
            {
                radTreeControl = value;
                RadTree = radTreeControl._RadTREE;
                Invalidate();
            }
        }

        #endregion

        #region === Control Settings ===


        // RadTree Assigned to Interact with 
        private TelerikTree radTreeControl = new TelerikTree(); 
         

        private SqlConnection Conn = new SqlConnection();
        private string SQLConName = "SQLserver";
        private string LoadDirPath = "DirPath";
        private string LoadDirPathFilePattern = "*.*";
        private bool LoadDirPathOnStart = true;
        private bool LoadDirPathRecurse = true;

        private string sqliteDbPath = "Settings.sqlite";
        private Form ThisForm = new Form();

        //ThisForm = this.FindForm();
        //Add_MouseOverControl(ThisForm.Controls);

        public string SQL_ConName
        {
            get { return SQLConName; }
            set { SQLConName = value; Invalidate(); }
        }

        private SqlConnection SQL_Con
        {
            get
            {
                string connName = this.SQL_ConName;
                return new SqlConnection(ConfigurationManager.ConnectionStrings[connName].ConnectionString);
                //return Conn; 
            }
            set { Conn = value; Invalidate(); }
        }

        public string LoadDir_Path
        {
            get { return LoadDirPath; }
            set { LoadDirPath = value; Invalidate(); }
        }
        public bool LoadDir_OnStartup
        {
            get { return LoadDirPathOnStart; }
            set { LoadDirPathOnStart = value; Invalidate(); }
        }
        public bool LoadDir_Recurse
        {
            get { return LoadDirPathRecurse; }
            set { LoadDirPathRecurse = value; Invalidate(); }
        }
        public string LoadDir_FilePattern
        {
            get { return LoadDirPathFilePattern; }
            set { LoadDirPathFilePattern = value; Invalidate(); }
        }
        public string Sqlite_Db
        {
            get { return sqliteDbPath; }
            set { sqliteDbPath = value; Invalidate(); }
        }



        #endregion


        #region === RadStatusBar ===

        /// <summary>
        /// Populate StatusBar with Text
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Text2"></param>
        public void sb(string Text, string Text2 = "")
        {
            if (radStatusStrip1 != null)
            {
                if (radStatusStrip1.Items.Count > 0)
                {
                    try { (radStatusStrip1.Items[0] as RadLabelElement).Text = Text; } catch { }
                }
                if (radStatusStrip1.Items.Count > 1)
                {
                    try { (radStatusStrip1.Items[1] as RadLabelElement).Text = Text2; } catch { }
                }
            }
        }




        #endregion

        private void btnFileTree_Click(object sender, EventArgs e)
        {
            _TelerikLib.RadTree tree = new _TelerikLib.RadTree();

            tree.RadTree_Files(RadTree, txtPath.Text);
        }

        private void btnPopulateComedyTree_Click(object sender, EventArgs e)
        {
            ComedianTree(RadTree, "S:\\", true);
        }


        public void ComedianTree(RadTreeView radtree, string rootDir = "S:\\", bool NewThread = true)
        {
            _AHK ahk = new _AHK();
            _Lists lst = new _Lists();
            _TelerikLib tel = new _TelerikLib();
            _TelerikLib.RadTree tree = new _TelerikLib.RadTree(); 

            if (NewThread)
            {
                Thread imdbTVParseThread = new Thread(() => ComedianTree(radtree, rootDir, false));
                imdbTVParseThread.Start();
            }
            else
            {
                //string rootDir = "S:\\";

                List<string> Comedians = lst.DirList(rootDir, "*.*", false, false);

                Comedians = lst.SortList(Comedians); // alpha sort list

                foreach (string com in Comedians)
                {
                    string first = ahk.FirstCharacters(com, 1); if (first == "_") { continue; }   // skip folders starting with "_"

                    // add node for comedian
                    RadTreeNode comNode = new RadTreeNode();
                    comNode.Text = com; comNode.Tag = rootDir + "\\" + com;
                    //radtree.Nodes.Add(comNode);
                    tree.AddNode(radtree, comNode);

                    // list of shows under comedian dir
                    List<string> shows = lst.DirList(rootDir + "\\" + com, "*.*", false, false);

                    shows = lst.SortList(shows); // alpha sort list

                    foreach (string show in shows)
                    {
                        RadTreeNode showNode = new RadTreeNode();
                        showNode.Text = show; showNode.Tag = rootDir + "\\" + com + "\\" + show;
                        //comNode.Nodes.Add(showNode);
                        tree.AddSubNode(comNode, showNode, radtree);
                    }
                }
            }


        }





    }
}

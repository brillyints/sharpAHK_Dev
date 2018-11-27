using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sharpAHK;
using sharpAHK_Dev;
using AHKExpressions;
using TelerikExpressions;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Timers;
using System.Drawing;
using Telerik.WinControls.UI;
using System.ComponentModel;
using System.Data;
using ScintillaNET;

namespace sharpAHK_Dev
{
    /// <summary>
    /// WinForm Controls Library | LucidMethod
    /// </summary>
    public class _WinForms
    {
        #region === Startup ===

        private static _AHK ahk = new _AHK();
        private static _Database.SQLite sqlite = new _Database.SQLite();
        private static _Lists lst = new _Lists();
        private static _GridControl grid = new _GridControl();

        //public _SQLite  { get; set; }
        //public _GridControl grid { get; set; }

        //sharpAHK._AHK ahk = new sharpAHK._AHK();
        //_Database.SQLite sqlite = new _Database.SQLite();
        //_GridControl grid = new _GridControl();

        private static bool GlobalDebug = false;

        #endregion

        #region === Extract Embeded Resources ====

        /// <summary>
        /// List Of Embeded Applications to Extract/Launch
        /// </summary>		
        public enum AppList
        {
            AutoHotkey = 0,
            MPC = 1,
            WinMerge = 2,
            ScreenClip = 4,
            NotepadPlus = 3
        }

        public string ExtractApp(AppList AppName, bool Launch = false, string NameSpace = "sharpAHK_Examples")
        {
            string path = "";
            if (AppName == AppList.AutoHotkey)
            {
                Extract_ResourceDir("AutoHotkey", NameSpace, "AppDir()\\Apps\\AppName", "Apps.AppName");
                path = ahk.AppDir() + "\\Apps\\AutoHotkey\\AutoHotkey.exe";
                if (File.Exists(path)) { if (Launch) { ahk.Run(path); } } else { ahk.MsgBox(path + " NOT FOUND"); }
            }
            if (AppName == AppList.MPC)
            {
                Extract_ResourceDir("MPC", NameSpace, "AppDir()\\Apps\\AppName", "Apps.AppName");
                path = ahk.AppDir() + "\\Apps\\MPC\\mpc-hc.exe";
                if (File.Exists(path)) { if (Launch) { ahk.Run(path); } } else { ahk.MsgBox(path + " NOT FOUND"); }
            }
            if (AppName == AppList.WinMerge)
            {
                //Extract_WinMerge();
                Extract_ResourceDir("WinMerge", NameSpace, "AppDir()\\Apps\\AppName", "Apps.AppName");
                path = ahk.AppDir() + "\\Apps\\WinMerge\\WinMergePortable.exe";
                if (File.Exists(path)) { if (Launch) { ahk.Run(path); } } else { ahk.MsgBox(path + " NOT FOUND"); }
            }
            if (AppName == AppList.ScreenClip)
            {
                //Extract_ScreenClip();
                Extract_ResourceDir("ScreenClip", NameSpace, "AppDir()\\Apps\\AppName", "Apps.AppName");
                path = ahk.AppDir() + "\\Apps\\ScreenClip\\ScreenClip.exe";
                if (File.Exists(path)) { if (Launch) { ahk.Run(path); } } else { ahk.MsgBox(path + " NOT FOUND"); }
            }
            if (AppName == AppList.NotepadPlus)
            {
                // Extract_NotepadPlus();
                Extract_ResourceDir("Notepad++", NameSpace, "AppDir()\\Apps\\AppName", "Apps.Notepad__");
                path = ahk.AppDir() + "\\Apps\\Notepad++\\Notepad++ (Portable).exe";
                if (File.Exists(path)) { if (Launch) { ahk.Run(path); } } else { ahk.MsgBox(path + " NOT FOUND"); }
            }


            return path;
        }


        /// <summary>
        /// Extract Embeded Resource from Application to Folder
        /// </summary>
        /// <param name="nameSpace">the namespace of your project, located right above your class' name;</param>
        /// <param name="outDirectory">where the file will be extracted to;</param>
        /// <param name="internalFilePath">the name of the folder inside visual studio which the files are in;  (TopFolder.SubFolder.SubSubDir Format)</param>
        /// <param name="resourceName">the name of the file;</param>
        /// <param name="RunAfterExtract">Option to Launch Exported File in Default Application</param>
        /// <param name="OverWrite">OverWrites Previous Extracted File (If Found)</param>
        /// <returns>Returns File Path To Extract File Path</returns>
        /// <example>
        ///                                   NAMESPACE           OUTDIR (Creates)            Path To Embeded File        Embeded FileName
        ///   string LocalFilePath = Extract("sharpAHK_Examples", ahk.AppDir() + "\\AHKexe", "Apps.AutoHotkey.Compiler", "Ahk2Exe.exe");
        /// </example>
        public string Extract(string nameSpace, string outDirectory, string internalFilePath, string resourceName, bool RunAfterExtract = false, bool OverWrite = false)
        {
            // *** make sure the file you are trying to extract is an 'embeded resource' --- NOT just included in the project folders as a reference  ****

            _AHK ahk = new _AHK();
            ahk.FileCreateDir(outDirectory);

            //Assembly assembly = Assembly.GetCallingAssembly();
            Assembly assembly = Assembly.GetEntryAssembly();

            string NewPath = outDirectory + "\\" + resourceName;

            //Dir = Dir.Replace("+", "_");

            if (OverWrite) { ahk.FileDelete(NewPath); }  // option to always use embeded by removing previous copy first

            if (!File.Exists(NewPath))  // extract file if it doesn't exist yet
            {
                using (Stream s = assembly.GetManifestResourceStream(nameSpace + "." + (internalFilePath == "" ? "" : internalFilePath + ".") + resourceName))

                    if (s != null)
                    {
                        using (BinaryReader r = new BinaryReader(s))
                        using (FileStream fs = new FileStream(NewPath, FileMode.OpenOrCreate))
                        using (BinaryWriter w = new BinaryWriter(fs))
                        {
                            w.Write(r.ReadBytes((int)s.Length));
                        }
                    }
            }

            if (RunAfterExtract) { ahk.Run(outDirectory + "\\" + resourceName); }

            return NewPath;
        }


        /// <summary>
        /// Returns List of All Include Resource Files Available To Extract
        /// </summary>
        /// <param name="Format">File Format To Add To List. Default = * (ALL)</param>
        /// <returns>Returns List of Files in Project Resource Dirs</returns>
        /// <example>
        /// List<string> Resources = ResourceFiles("*");
        /// foreach (string File in Resources)
        /// {
        ///     ahk.MsgBox(File);
        /// }
        /// </example>
        public List<string> ResourceFiles(string Format = "*")
        {
            Assembly _assembly;
            //_assembly = Assembly.GetExecutingAssembly();  // returns sharpAHK_Dev as NameSpace
            _assembly = Assembly.GetEntryAssembly();

            List<string> filenames = new List<string>();
            filenames = _assembly.GetManifestResourceNames().ToList<string>();
            List<string> allFiles = new List<string>();
            for (int i = 0; i < filenames.Count(); i++)
            {
                string[] items = filenames.ToArray();
                string name = items[i].ToString();

                if (Format == "*") { allFiles.Add(name); continue; }

                string fileformat = ahk.StringSplit(name, ".", 0, true, true);
                string nameSpace = ahk.StringSplit(name, ".", 0);

                string nameWithDir = name.Replace(nameSpace + ".", "");

                ahk.MsgBox(nameWithDir);

                // if user provided file format, match on file name
                if (fileformat.ToUpper().Trim() == Format.ToUpper().Trim()) { allFiles.Add(name); }

                //ahk.MsgBox(name);
            }

            return allFiles;
        }


        /// <summary>
        /// Returns List of All Included Resource Files In Specific Dir Path
        /// </summary>
        /// <param name="DirPath">Path To Project Resource Dir (Uses "." instead of "/")</param>
        /// <returns>Returns List of All Files in Resource Dir Path</returns>
        /// <example>
        /// List<string> Resources = ResourceDir("AutoHotkey.Compiler");
        /// foreach (string File in Resources)
        /// {
        ///     ahk.MsgBox(File);
        /// }
        /// </example>
        public List<string> ResourceDir(string DirPath = "AutoHotkey.Compiler")
        {
            Assembly _assembly;
            //_assembly = Assembly.GetExecutingAssembly();  // returns sharpAHK_Dev as NameSpace
            _assembly = Assembly.GetEntryAssembly();

            List<string> filenames = new List<string>();
            filenames = _assembly.GetManifestResourceNames().ToList<string>();
            List<string> allFiles = new List<string>();
            for (int i = 0; i < filenames.Count(); i++)
            {
                string[] items = filenames.ToArray();
                string name = items[i].ToString();

                string fileformat = ahk.StringSplit(name, ".", 0, true, true);
                string nameSpace = ahk.StringSplit(name, ".", 0);
                string nameWithDir = name.Replace(nameSpace + ".", "");

                //if (name.ToUpper().Contains("NOTEPAD++")) { ahk.MsgBox("notepad"); }

                if (name.ToUpper().Contains(DirPath.ToUpper()))
                {
                    string fname = name.Replace(nameSpace + "." + DirPath + ".", "");
                    allFiles.Add(fname);
                    continue;
                }

            }

            return allFiles;
        }

        // 
        /// <summary>
        /// From Resource Path, Return Name of Project NameSpace  
        /// </summary>
        /// <param name="DirPath">Path To Project Resource Dir (Uses "." instead of "/")</param>
        /// <returns>Return Name of Project NameSpace  </returns>
        public string NameSpace(string DirPath = "AutoHotkey.Compiler")
        {
            Assembly _assembly;
            //_assembly = Assembly.GetExecutingAssembly();  // returns sharpAHK_Dev as NameSpace
            _assembly = Assembly.GetEntryAssembly();


            List<string> filenames = new List<string>();
            filenames = _assembly.GetManifestResourceNames().ToList<string>();
            List<string> allFiles = new List<string>();
            for (int i = 0; i < filenames.Count(); i++)
            {
                string[] items = filenames.ToArray();
                string name = items[i].ToString();

                string fileformat = ahk.StringSplit(name, ".", 0, true, true);
                string nameSpace = ahk.StringSplit(name, ".", 0);
                string nameWithDir = name.Replace(nameSpace + ".", "");

                return nameSpace;

                //if (name.ToUpper().Contains("NOTEPAD++")) { ahk.MsgBox("notepad"); }

                if (name.ToUpper().Contains(DirPath.ToUpper()))
                {
                    string fname = name.Replace(nameSpace + "." + DirPath + ".", "");
                    allFiles.Add(fname);
                    continue;
                }

            }

            return "";
        }


        /// <summary>
        /// Extract Contents of Embeded Resource Dir to LocalSaveDir
        /// </summary>
        /// <param name="AppName">Application Name</param>
        /// <param name="nameSpace">Project NameSpace</param>
        /// <param name="LocalExtractRoot">Dir To Extract Embeded Files To</param>
        /// <param name="ResourceDirPath">Path</param>
        public void Extract_ResourceDir(string AppName = "WinMerge", string nameSpace = "sharpAHK_Examples", string LocalExtractRoot = "AppDir()\\Apps\\AppName", string ResourceDirPath = "Apps.AppName")
        {
            if (LocalExtractRoot == "AppDir()\\Apps\\AppName") { LocalExtractRoot = ahk.AppDir() + "\\Apps\\" + AppName; }


            if (ResourceDirPath == "Apps.AppName") { ResourceDirPath = "Apps." + AppName; }


            // if default name used, loopup namespace 
            if (nameSpace == "sharpAHK_Examples") { nameSpace = NameSpace(ResourceDirPath); }

            //string AppPath = LocalAppDir + "\\WinMergePortable.exe";

            ahk.FileCreateDir(LocalExtractRoot);

            List<string> files = ResourceDir(ResourceDirPath);

            //ahk.MsgBox("Extracting " + files.Count + " Files From: " + AppName);

            foreach (string file in files)
            {
                List<string> Path = ahk.StringSplit_List(file, ".");

                int nm = Path.Count; nm--; nm--;
                string fileName = lst.Return_List_Value(Path, nm);
                string fileExt = lst.Return_List_Value(Path, Path.Count);
                string File = fileName + "." + fileExt;

                string Dir = file.Replace("." + fileName + "." + fileExt, "");
                Dir = "Apps." + AppName + "." + Dir;
                Dir = Dir.Replace("+", "_");

                string FullDir = ahk.AppDir() + "\\" + Dir.Replace(".", "\\");

                // if only 2 results = file.ext --- don't create dir 
                if (Path.Count > 2) { ahk.FileCreateDir(FullDir); }
                if (Path.Count == 2)
                {
                    string Ffile = File.Replace("+", "_");
                    Dir = Dir.Replace("." + Ffile, "");
                    FullDir = LocalExtractRoot;
                }

                // fix for ++ to __ conversion done by VS
                if (FullDir.Contains("Notepad__")) { FullDir = FullDir.Replace("Notepad__", "Notepad++"); }

                // Extract AHK Compiler To Temp Dir (If Not Previously Exported)

                if (File == "Notepad++ (Portable).exe")
                {
                    ahk.MsgBox(File);
                }

                Extract(nameSpace, FullDir, Dir, File);
                //ahk.MsgBox(FullDir + "\n\n" + Dir + "\n\n" + File);
            }

            //ahk.MsgBox("Extracted " + files.Count + " Files");
            //ahk.Run(AppPath);
            //return AppPath;
        }



        #endregion

        //#region === Embeded Resources ===

        //// Extract Embeded Resources  (works)





        //public static void ExtractFile()
        //{
        //    String local = Environment.CurrentDirectory; //gets current path to extract the files

        //    // internalFilePath =  TopFolder.SubFolder  Format

        //    //Extract("sharpAHK_Examples", local, "Images.Icons", "GridIco01.ico");

        //    _AHK ahk = new _AHK();


        //    //ahk.FileCreateDir(ahk.AppDir() + "\\AHKexe");

        //    //Extract("sharpAHK_Examples", ahk.AppDir() + "\\AHKexe", "Apps.AutoHotkey", "AU3_Spy.exe");
        //    Extract("sharpAHK_Examples", ahk.AppDir() + "\\AHKexe", "Apps.AutoHotkey", "AutoHotkey.chm", true);

        //    //Extract("sharpAHK_Examples", local, "dlls", "System.Data.SQLite.Linq.dll");
        //    //Extract("sharpAHK_Examples", local, "dlls", "AU3_Spy.exe");

        //    //ahk.Run(ahk.AppDir() + "\\AHKexe\\AU3_Spy.exe");  // run file after extracting to temp dir
        //}

        //public static void Extract(string nameSpace, string outDirectory, string internalFilePath, string resourceName, bool RunAfterExtract = false)
        //{
        //    // *** make sure the file you are trying to extract is an 'embeded resource' --- NOT just included in the project folders as a reference  ****

        //    _AHK ahk = new _AHK();
        //    ahk.FileCreateDir(outDirectory);

        //    //nameSpace = the namespace of your project, located right above your class' name;
        //    //outDirectory = where the file will be extracted to;
        //    //internalFilePath = the name of the folder inside visual studio which the files are in;  (TopFolder.SubFolder.SubSubDir Format)
        //    //resourceName = the name of the file;
        //    Assembly assembly = Assembly.GetCallingAssembly();

        //    using (Stream s = assembly.GetManifestResourceStream(nameSpace + "." + (internalFilePath == "" ? "" : internalFilePath + ".") + resourceName))
        //    using (BinaryReader r = new BinaryReader(s))
        //    using (FileStream fs = new FileStream(outDirectory + "\\" + resourceName, FileMode.OpenOrCreate))
        //    using (BinaryWriter w = new BinaryWriter(fs))
        //    {
        //        w.Write(r.ReadBytes((int)s.Length));
        //    }

        //    if (RunAfterExtract) { ahk.Run(outDirectory + "\\" + resourceName); }
        //}


        //#endregion

        #region === Save / Restore Window Position / AOT ===

        public void SaveWinPos(Form formName)
        {
            int winX = 0;
            int winY = 0;
            int winW = 0;
            int winH = 0;
            bool isMaximized = false;
            bool isMinimized = false;

            Point winRec = formName.RestoreBounds.Location;  //Location; // 
            winX = winRec.X; winY = winRec.Y;

            Size recSize = formName.ClientSize;
            winW = recSize.Width; winH = recSize.Height;

            if (formName.WindowState == FormWindowState.Maximized)
            {
                isMaximized = true;
                isMinimized = false;
            }
            else if (formName.WindowState == FormWindowState.Normal)
            {
                isMaximized = false;
                isMinimized = false;
            }
            else
            {
                isMaximized = false;
                isMinimized = true;
            }

            sqlite.Setting("WinX", winX.ToString());
            sqlite.Setting("WinY", winY.ToString());
            sqlite.Setting("WinW", winW.ToString());
            sqlite.Setting("WinH", winH.ToString());
            sqlite.Setting("IsMaximized", isMaximized.ToString());
            sqlite.Setting("IsMinimized", isMinimized.ToString());
            //sqlite.Setting("LoadPreviousWinPos", "True");
        }
        public void RestoreWinPos(Form formName)
        {
            bool isMin = false;
            bool isMax = false;

            winInfo winn = new winInfo();
            winn.WinX = ahk.ToInt(sqlite.Setting("WinX"));
            winn.WinY = ahk.ToInt(sqlite.Setting("WinY"));
            winn.WinW = ahk.ToInt(sqlite.Setting("WinW"));
            winn.WinH = ahk.ToInt(sqlite.Setting("WinH"));
            isMax = ahk.ToBool(sqlite.Setting("IsMaximized"));
            isMin = ahk.ToBool(sqlite.Setting("IsMinimized"));

            //saveWinPosToolStripMenuItem.Checked = true;
            //ahk.WinMove(this.Text, "", winn.WinX, winn.WinY, winn.WinH, winn.WinW);

            Point winRec = new Point();
            winRec.X = winn.WinX; winRec.Y = winn.WinY;

            Size recSize = new Size();
            recSize.Width = winn.WinW; recSize.Height = winn.WinH;

            if (isMax)
            {
                formName.WindowState = FormWindowState.Maximized;
                //formName.Location = winRec;
                //formName.Size = recSize;
            }
            else if (isMin)
            {
                formName.Location = winRec;
                formName.Size = recSize;
                formName.WindowState = FormWindowState.Minimized;
            }
            else
            {
                formName.Location = winRec;
                formName.Size = recSize;
            }
        }


        #endregion

        // find form

        //`foreach (Form form in Application.OpenForms)
        // {
        //     if (form.GetType() == typeof(myMainform))
        //     {
        //         form.Activate();
        //         form.Show();
        //         this.Close();
        //         return;
        //     }
        // }

        // myMainform m = new myMainform();
        // m.Show();`

        // find control by name
        //TextBox tbx = this.Controls.Find("textBox1", true).FirstOrDefault() as TextBox;

        // (Button)sender) - cast sender as control

        #region === #### Move Controls On GUI at RunTime #### ====


        #region === Control Move ===

        // Enable Control Move From Menu Option (Toggles Move Mode)
        // Move Single Control - Works

        private int _xPos;
        private int _yPos;
        private bool _dragging;
        private bool _moveEnbled = true;
        private bool _moveAssigned = false;

        // Move Control On WinForm With MouseDrag Once Enabled
        public void Control_MovesOnDrag(object FormControl, bool Enable = true)
        {
            string ControlType = FormControl.GetType().ToString();  //determine what kind of variable was passed into function

            if (Enable)
            {
                if (!_moveAssigned)
                {
                    Control pb = null;
                    if (IsPictureBox(FormControl)) { pb = (PictureBox)FormControl; }

                    // Register mouse events
                    pb.MouseUp += (sender, args) =>
                    {
                        var c = FormControl;
                        if (IsPictureBox(FormControl)) { c = sender as PictureBox; }

                        if (null == c) return;
                        _dragging = false;
                    };

                    pb.MouseDown += (sender, args) =>
                    {
                        if (args.Button != MouseButtons.Left) return;

                        if (_moveEnbled) { _dragging = true; }
                        //_dragging = true;
                        _xPos = args.X;
                        _yPos = args.Y;
                    };

                    pb.MouseMove += (sender, args) =>
                    {
                        if (_moveEnbled)
                        {
                            var c = FormControl;
                            if (IsPictureBox(FormControl)) { c = sender as PictureBox; }

                            //var c = sender as PictureBox;
                            if (!_dragging || null == pb) return;
                            pb.Top = args.Y + pb.Top - _yPos;
                            pb.Left = args.X + pb.Left - _xPos;
                        }
                    };

                    _moveAssigned = true;
                }

                _moveEnbled = true;
            }
            else // Disable Action
            {
                _moveEnbled = false;
            }

        }

        // ToolStripMenu Option to Toggle Enabling/Disabling Moving Control on WinForm
        private void controlMove_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem clicked = (ToolStripMenuItem)sender;
            //StatusBar("MenuClick: " + clicked.Text);

            if (clicked.Text == "Enable PictureBox Move")
            {
                // Define PictureBox Control on WinForm
                PictureBox moveControl = new PictureBox();
                //moveControl = pictureBox1;

                if (clicked.Checked == false) { clicked.Checked = true; Control_MovesOnDrag(moveControl, true); return; }
                if (clicked.Checked == true) { clicked.Checked = false; Control_MovesOnDrag(moveControl, false); return; }
            }

        }


        #endregion




        public void Enable_MoveControl(Control control)  // enable ability to move a single control (CONTROL button + MOUSE DRAG)
        {
            control.MouseDown += (sender, e) => { MoveControl_MouseDown(sender, e); };  // assign event action
            control.MouseMove += (sender, e) => { MoveControl_MouseMove(sender, e); };  // assign event action
            control.MouseUp += (sender, e) => { MoveControl_MouseUp(sender, e); };  // assign event action
        }

        public void Enable_MoveControl_AllControls(Control form) // enable ability to move all controls on the Form (CONTROL button + MOUSE DRAG)
        {
            _Lists lst = new _Lists();
            List<Control> controls = lst.Control_List(form);  // returns list of controls on a form

            foreach (Control control in controls)
            {
                //if (control.GetType().ToString() == "System.Windows.Forms.TreeView") { continue; }

                control.MouseDown += (sender, e) => { MoveControl_MouseDown(sender, e); };  // assign event action
                control.MouseMove += (sender, e) => { MoveControl_MouseMove(sender, e); };  // assign event action
                control.MouseUp += (sender, e) => { MoveControl_MouseUp(sender, e); };  // assign event action
            }
        }
        public void Disable_MoveControl_AllControls(Control form) // enable ability to move all controls on the Form (CONTROL button + MOUSE DRAG)
        {
            _Lists lst = new _Lists();
            List<Control> controls = lst.Control_List(form);  // returns list of controls on a form

            foreach (Control control in controls)
            {
                //if (control.GetType().ToString() == "System.Windows.Forms.TreeView") { continue; }

                control.MouseDown -= (sender, e) => { MoveControl_MouseDown(sender, e); };  // assign event action
                control.MouseMove -= (sender, e) => { MoveControl_MouseMove(sender, e); };  // assign event action
                control.MouseUp -= (sender, e) => { MoveControl_MouseUp(sender, e); };  // assign event action
            }
        }

        private Point MoveControl_MouseDownLocation;
        private string MouseMoveMod = "LButton"; // combination of keys to enable control move
        private bool MoveControl_On = false;  // tracks whether control is in "move-mode"

        public void Set_MouseMoveMod(string MouseMoveModKeys)
        {
            MouseMoveModKeys = MouseMoveMod;
        }

        private void MoveControl_MouseUp(object sender, MouseEventArgs e)
        {
            MoveControl_On = false;
            Enable_All_Controls(((Control)sender).FindForm()); // enable all controls on sending form
            ((Control)sender).FindForm().Enabled = true;
        }

        private void MoveControl_MouseDown(object sender, MouseEventArgs e)
        {
            //Control sendControl = ((Control)sender);        // the Control that sent this command
            //Form fromForm = ((Control)sender).FindForm();   // the Form the Control is located on 

            if (MouseMoveMod == "Control, LButton")
            {
                if ((Control.ModifierKeys) != 0)
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        //((Control)sender).Enabled = false;
                        Disable_All_Controls(((Control)sender).FindForm()); // disable all controls on sending form
                        MoveControl_MouseDownLocation = e.Location;
                    }
                }

                //((Control)sender).Enabled = true;
                Enable_All_Controls(((Control)sender).FindForm()); // enable all controls on sending form
            }

            if (MouseMoveMod == "LButton")
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    ((Control)sender).FindForm().Enabled = false;
                    //((Control)sender).Enabled = false;
                    Disable_All_Controls(((Control)sender).FindForm()); // disable all controls on sending form
                    MoveControl_MouseDownLocation = e.Location;
                    MoveControl_On = true;
                }

                //((Control)sender).Enabled = true;
                Enable_All_Controls(((Control)sender).FindForm()); // enable all controls on sending form
                ((Control)sender).FindForm().Enabled = true;
            }


        }

        private void MoveControl_MouseMove(object sender, MouseEventArgs e)
        {
            ////if (MouseMoveMod == "Control, LButton")
            ////{
            ////    if ((Control.ModifierKeys) != 0)
            ////    {
            ////        if (e.Button == System.Windows.Forms.MouseButtons.Left)
            ////        {
            ////            //((Control)sender).Enabled = false;
            ////            Disable_All_Controls(((Control)sender).FindForm()); // disable all controls on sending form
            ////            ((Control)sender).Left = e.X + ((Control)sender).Left - MoveControl_MouseDownLocation.X;
            ////            ((Control)sender).Top = e.Y + ((Control)sender).Top - MoveControl_MouseDownLocation.Y;
            ////        }
            ////    }

            ////    //((Control)sender).Enabled = true;
            ////    Enable_All_Controls(((Control)sender).FindForm()); // enable all controls on sending form
            ////}

            if (MoveControl_On)
            {
                //((Control)sender).FindForm().Enabled = false; 
                //Disable_All_Controls(((Control)sender).FindForm()); // disable all controls on sending form
                //((Control)sender).Enabled = false;
                ((Control)sender).Left = e.X + ((Control)sender).Left - MoveControl_MouseDownLocation.X;
                ((Control)sender).Top = e.Y + ((Control)sender).Top - MoveControl_MouseDownLocation.Y;
                //MoveControl_On = false;
                //((Control)sender).Enabled = true;
                //Enable_All_Controls(((Control)sender).FindForm()); // enable all controls on sending form
                //((Control)sender).FindForm().Enabled = true; 
            }

        }

        public void Disable_All_Controls(Form FormName)
        {
            //Disable_All_Controls(((Control)sender).FindForm()); // disable all controls on sending form

            foreach (Control c in FormName.Controls)
            {
                c.Enabled = false;
            }
        }
        public void Enable_All_Controls(Form FormName)
        {
            //Enable_All_Controls(((Control)sender).FindForm()); // enable all controls on sending form

            foreach (Control c in FormName.Controls)
            {
                c.Enabled = true;
            }
        }

        #endregion

        #region === #### Auto-Scale Gui Controls On Form Resize #### ===

        // Auto-Scale Gui Public Variables
        double __rW = 0; double __rH = 0; int __fH = 0; int __fW = 0;

        private void AutoScale_Form_Resize(object sender, EventArgs e)
        {
            AutoScale_rResize((Form)sender, true);
        }

        public void AutoScale_Controls_On_Resize(Form formName)
        {
            formName.Resize += AutoScale_Form_Resize; // handles resize routine

            // Put values in the variables

            __rW = formName.Width;
            __rH = formName.Height;

            __fW = formName.Width;
            __fH = formName.Height;


            // Loop through the controls inside the  form i.e. Tabcontrol Container
            foreach (Control c in formName.Controls)
            {
                c.Tag = c.Name + "/" + c.Left + "/" + c.Top + "/" + c.Width + "/" + c.Height;

                // c.Anchor = (AnchorStyles.Right |  AnchorStyles.Left ); 

                if (c.GetType() == typeof(TabControl))
                {

                    foreach (Control f in c.Controls)
                    {

                        foreach (Control j in f.Controls) //tabpage
                        {
                            j.Tag = j.Name + "/" + j.Left + "/" + j.Top + "/" + j.Width + "/" + j.Height;
                        }
                    }
                }
            }
        }

        private void AutoScale_rResize(Control t, bool hasTabs) // Routine to Auto resize the control
        {

            Form sendingForm = t.FindForm();

            // this will return to normal default size when 1 of the conditions is met

            string[] s = null;

            if (sendingForm.Width < __fW || sendingForm.Height < __fH)
            {

                sendingForm.Width = (int)__fW;
                sendingForm.Height = (int)__fH;

                return;
            }

            foreach (Control c in t.Controls)
            {
                // Option 1:
                double rRW = (t.Width > __rW ? t.Width / (__rW) : __rW / t.Width);
                double rRH = (t.Height > __rH ? t.Height / (__rH) : __rH / t.Height);

                // Option 2:
                //  double rRW = t.Width / rW;
                //  double rRH = t.Height / rH;

                s = c.Tag.ToString().Split('/');
                if (c.Name == s[0].ToString())
                {
                    //Use integer casting
                    c.Width = (int)(Convert.ToInt32(s[3]) * rRW);
                    c.Height = (int)(Convert.ToInt32(s[4]) * rRH);
                    c.Left = (int)(Convert.ToInt32(s[1]) * rRW);
                    c.Top = (int)(Convert.ToInt32(s[2]) * rRH);
                }
                if (hasTabs)
                {
                    if (c.GetType() == typeof(TabControl))
                    {

                        foreach (Control f in c.Controls)
                        {
                            foreach (Control j in f.Controls) //tabpage
                            {
                                s = j.Tag.ToString().Split('/');

                                if (j.Name == s[0].ToString())
                                {

                                    j.Width = (int)(Convert.ToInt32(s[3]) * rRW);
                                    j.Height = (int)(Convert.ToInt32(s[4]) * rRH);
                                    j.Left = (int)(Convert.ToInt32(s[1]) * rRW);
                                    j.Top = (int)(Convert.ToInt32(s[2]) * rRH);
                                }
                            }
                        }
                    }
                }

            }
        }

        #endregion

        #region === Event Handler Lib ===

        // ### Event Handlers ####

        public List<string> Return_Event_Handlers(Control control)  // returns the Event Type (ex: Click) + Event Function Name (button1_Click()) 
        {
            List<string> eventList = new List<string>();

            ArrayList eventData = new ArrayList();
            EventDescriptorCollection events = TypeDescriptor.GetEvents(control);
            foreach (System.ComponentModel.EventDescriptor myEvent in events)
            {
                //Unwire the events
                EventDatum ed = EventDatum.Create(control, myEvent);
                if (ed == null) continue;
                eventData.Add(ed);
                //ed.Unwire(myComponent);
            }

            foreach (EventDatum ed in eventData)
            {
                string a1 = ed._event.Method.Name.ToString();


                string edu = ed._eventDesc.Name.ToString();
                string edu1 = ed._eventDesc.EventType.ToString();
                string edu2 = ed._eventDesc.DisplayName.ToString();
                string edu3 = ed._eventDesc.Description.ToString();
                string edu4 = ed._eventDesc.ComponentType.ToString();
                string edu5 = ed._eventDesc.Category.ToString();
                string edu6 = ed._eventDesc.Attributes.ToString();
                string edun = ed._event.ToString();

                //ahk.MsgBox(edu + " - " + a1);

                eventList.Add(edu + " - " + a1);
            }

            return eventList;
        }

        public void Remove_Click_Event(Control b)  // remove the "Click" Event Handler for a Control 
        {
            FieldInfo f1 = typeof(Control).GetField("EventClick", BindingFlags.Static | BindingFlags.NonPublic);
            object obj = f1.GetValue(b);
            PropertyInfo pi = b.GetType().GetProperty("Events", BindingFlags.NonPublic | BindingFlags.Instance);
            EventHandlerList list = (EventHandlerList)pi.GetValue(b, null);
            list.RemoveHandler(obj, list[obj]);
        }


        #endregion

        #region === Control Under Mouse ===

        // Ex: 
        //      frm.Add_MouseOverControl(this.Controls);  // add mouse-over-control event to every control - updates statusbar with current control


        // returns the name of the control currently under mouse

        public Control LastControlUnderMouse { get; set; } // stores last control detected under mouse (for dynamic context menu)

        public void Add_MouseOverControl(IEnumerable controls)  // create an action that occurs when every control is moved over with mouse (add to startup)
        {
            // Ex: (Add To Startup)
            // Add_MouseOverControl(this.Controls);  // loop through all controls and add mouse over control action

            foreach (Control control in controls)
            {
                control.MouseEnter += MouseOverControl;

                if (control.HasChildren)
                    Add_MouseOverControl(control.Controls);
            }
        }

        // enable status bar to display results 
        public void MouseOverControl(object sender, EventArgs e)  // Event Handler for Mouse over Control to Update Screen/StatusBar with Control Name
        {
            LastControlUnderMouse = ((Control)sender);

            string controlType = sender.GetType().ToString();
            string controlName = LastControlUnderMouse.Name;
            string controlSize = LastControlUnderMouse.Size.ToString();
            string controlParent = LastControlUnderMouse.Parent.Name.ToString();
            //int PaddingLeft = LastControlUnderMouse.Padding.Left;
            //int PaddingRight = LastControlUnderMouse.Padding.Right;
            //int PaddingTop = LastControlUnderMouse.Padding.Top;
            //int PaddingBottom = LastControlUnderMouse.Padding.Bottom;
            string controlTag = "";
            try { controlTag = LastControlUnderMouse.Tag.ToString(); }
            catch { }

            Form fromForm = ((Control)sender).FindForm();   // the Form the Control is located on 
            //ahk.MsgBox(sender.ToString());
            StatusBar(fromForm, "Mouse Over " + controlName);
        }

        public string Control_Under_Mouse(Control control)  // returns the name of the control on a form currently under the cursor
        {
            string ControlName = "";
            try
            {
                ControlName = ControlNavigationHelper.getYoungestChildUnderMouse(control).Name.ToString();
                LastControlUnderMouse = ControlNavigationHelper.getYoungestChildUnderMouse(control);
            }
            catch { }
            return ControlName;
        }

        #endregion

        // new project setup

        //frm.Create_StatusBar(this);  // Create StatusBar 
        //frm.Add_MouseOverControl(this.Controls);  // add mouse-over-control event to every control - updates statusbar with current control
        //frm.Setup_Global_Context_Menu(this);    // setup form-wide context menu (right click options) - frm.Add_MouseOverControl(this.Controls); REQUIRED 

        #region === Global Context Menu ===

        // global control context menu

        ContextMenuStrip globalMenuStrip;

        public void Setup_Global_Context_Menu(Form formName)
        {
            ContextMenuStrip globalMenuStrip1 = new ContextMenuStrip();

            ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem();
            ToolStripComboBox toolStripComboBox3 = new ToolStripComboBox();
            ToolStripSeparator toolStripSeparator3 = new ToolStripSeparator();
            ToolStripTextBox toolStripTextBox3 = new ToolStripTextBox();
            ToolStripMenuItem fileToolStripMenuItem = new ToolStripMenuItem();
            ToolStripMenuItem closeToolStripMenuItem = new ToolStripMenuItem();
            ToolStripMenuItem exitToolStripMenuItem = new ToolStripMenuItem();

            globalMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripMenuItem3,
            toolStripComboBox3,
            toolStripSeparator3,
            toolStripTextBox3});
            globalMenuStrip1.Name = "globalMenuStrip";
            globalMenuStrip1.Size = new System.Drawing.Size(271, 123);
            globalMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(Global_ContextMenu_Opening);

            globalMenuStrip = globalMenuStrip1; // store in public var for reference

            formName.ContextMenuStrip = globalMenuStrip;
        }

        public void Add_Global_ContextMenu(IEnumerable controls)  // create an action that occurs when every control is moved over with mouse (add to startup)
        {
            // Ex: (Add To Startup)
            // Add_MouseOverControl(this.Controls);  // loop through all controls and add mouse over control action

            foreach (Control control in controls)
            {
                //control.ContextMenu. += Global_ContextMenu_Opening;

                if (control.HasChildren)
                    Add_Global_ContextMenu(control.Controls);
            }
        }

        public void Global_ContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (LastControlUnderMouse == null) { return; }

            Control overControl = LastControlUnderMouse;

            // Acquire references to the owning control and item.
            Control c = globalMenuStrip.SourceControl as Control;
            ToolStripDropDownItem tsi = globalMenuStrip.OwnerItem as ToolStripDropDownItem;

            // Clear the ContextMenuStrip control's Items collection.
            globalMenuStrip.Items.Clear();



            string controlType = overControl.GetType().ToString();
            string controlName = overControl.Name;
            string controlText = overControl.Text;
            string controlTag = "";
            if (overControl.Tag != null) { controlTag = overControl.Tag.ToString(); }

            string controlSize = overControl.Size.ToString();
            //string controlParent = overControl.Parent.Name.ToString();
            //int PaddingLeft = overControl.Padding.Left;
            //int PaddingRight = overControl.Padding.Right;
            //int PaddingTop = overControl.Padding.Top;
            //int PaddingBottom = overControl.Padding.Bottom;

            //overControl.Location
            //overControl.Height
            //overControl.HasChildren
            //overControl.Handle
            //overControl.ForeColor
            //overControl.Font
            //overControl.Enabled
            //overControl.Dock
            //overControl.Controls.Count
            //overControl.BackgroundImage
            //overControl.BackColor
            //overControl.AllowDrop


            // Check the source control first.
            if (c != null)
            {
                // Add custom item (Form)

                controlType = ahk.StringReplace(controlType, "System.Windows.Forms.");

                //contextMenuStrip1.Items.Add("Control Type: " + controlType);
                //contextMenuStrip1.Items.Add("Control Name: " + controlName);
                //contextMenuStrip1.Items.Add("Control Text: " + controlText);
                //contextMenuStrip1.Items.Add("Control Tags: " + controlTag);


                ToolStripMenuItem item, submenu;

                submenu = new ToolStripMenuItem();
                submenu.Text = controlName;

                ToolStripMenuItem Opt1 = new ToolStripMenuItem("Name: " + controlName);
                Opt1.Click += new EventHandler(Global_Context_Menu_Actions);
                submenu.DropDownItems.Add(Opt1);

                ToolStripMenuItem Opt2 = new ToolStripMenuItem("Type: " + controlType);
                Opt2.Click += new EventHandler(Global_Context_Menu_Actions);
                submenu.DropDownItems.Add(Opt2);

                ToolStripMenuItem Opt3 = new ToolStripMenuItem("Text: " + controlText);
                Opt3.Click += new EventHandler(Global_Context_Menu_Actions);
                submenu.DropDownItems.Add(Opt3);

                ToolStripMenuItem Opt4 = new ToolStripMenuItem("Tags: " + controlTag);
                Opt4.Click += new EventHandler(Global_Context_Menu_Actions);
                submenu.DropDownItems.Add(Opt4);

                ToolStripMenuItem Opt5 = new ToolStripMenuItem("Rename Control");
                Opt5.Click += new EventHandler(Global_Context_Menu_Actions);
                submenu.DropDownItems.Add(Opt5);

                Opt5 = new ToolStripMenuItem("Back Color");
                Opt5.Click += new EventHandler(Global_Context_Menu_Actions);
                submenu.DropDownItems.Add(Opt5);

                Opt5 = new ToolStripMenuItem("Fore Color");
                Opt5.Click += new EventHandler(Global_Context_Menu_Actions);
                submenu.DropDownItems.Add(Opt5);

                Opt5 = new ToolStripMenuItem("Background Image");
                Opt5.Click += new EventHandler(Global_Context_Menu_Actions);
                submenu.DropDownItems.Add(Opt5);

                globalMenuStrip.Items.Add(submenu);

                //contextMenuStrip1.Items.Add("Source: " + c.GetType().ToString());
            }
            else if (tsi != null)
            {
                // Add custom item (ToolStripDropDownButton or ToolStripMenuItem)
                globalMenuStrip.Items.Add("Source: " + tsi.GetType().ToString());
            }

            // Set Cancel to false.  It is optimized to true based on empty entry.
            e.Cancel = false;

            globalMenuStrip.Show();
            globalMenuStrip.Visible = true;
        }

        public void Global_Context_Menu_Actions(object sender, EventArgs e)  // actions to perform from context menu options
        {
            //Control SendControl = (Control)sender;

            string Opt = sender.ToString();

            //ahk.MsgBox(OptionSelected + " Clicked"); 

            if (Opt == "Rename Control")
            {
                //ahk.MsgBox("Renaming " + LastControlUnderMouse.Text); 
                string value = LastControlUnderMouse.Text;

                string Inpt = ahk.InputBox("New Control Text", "New Control Text: ", false, "", "", "", "", "", "", value);

                if (Inpt != "")
                {
                    string UserEntry = value;
                    //ahk.MsgBox("Renaming to " + UserEntry); 
                    LastControlUnderMouse.Text = UserEntry;
                }
            }

            if (Opt == "Back Color")
            {
                Color SelectedColor = ahk.Color_Dialog(); // select color dialog
                // update control if value returned
                if (SelectedColor != Color.Empty) { LastControlUnderMouse.BackColor = SelectedColor; }
            }

            if (Opt == "Fore Color")
            {
                Color SelectedColor = ahk.Color_Dialog(); // select color dialog
                // update control if value returned
                if (SelectedColor != Color.Empty) { LastControlUnderMouse.ForeColor = SelectedColor; }
            }

            if (Opt == "Background Image")
            {
                //Color SelectedColor = ahk.Color_Dialog(); // select color dialog
                // update control if value returned
                //if (SelectedColor != Color.Empty) { LastControlUnderMouse.BackgroundImage = SelectedColor; }

                //ListView_Popup(LastControlUnderMouse);
                //Control_Info_Tree(treeView1, LastControlUnderMouse);
                Return_Event_Handlers(LastControlUnderMouse);
            }




            if (Opt == "Exit") { ahk.MsgBox("Option Clicked"); }
            if (Opt == "DirWatch Viewer") { ahk.MsgBox("Option Clicked"); }

        }

        #endregion

        #region === Control Info ===

        /// <summary>
        /// Display Info Control in TreeView Display 
        /// </summary>
        /// <param name="TV">TreeView Control Name To Display Control Details</param>
        /// <param name="control">ControlName To Return Info</param>
        /// <param name="ExpandTV">Option To Expand TreeView Display After Populating (Default = True)</param>
        /// <param name="ClearTV">Option To Clear TreeNodes Before Populating ControlInfo</param>
        public void ControlInfo_Tree(TreeView TV, Control control, bool ExpandTV = true, bool ClearTV = false)
        {
            _TreeViewControl tv = new _TreeViewControl();
            //Control overControl = LastControlUnderMouse;

            if (ClearTV) { tv.ClearTV(TV); }

            TreeNode parent = new TreeNode();  // level 1
            parent.Text = control.Name;
            parent.Tag = control.FindForm();

            TreeNode section = new TreeNode();  // level 2
            section.Text = "Text: " + control.Text;
            parent.Nodes.Add(section);

            section = new TreeNode();  // level 2
            section.Text = "Type: " + control.GetType().ToString();
            parent.Nodes.Add(section);

            string Tag = ""; try { Tag = control.Tag.ToString(); }
            catch { }  // grab control tag if populated
            section = new TreeNode();  // level 2
            section.Text = "Tag: " + Tag;
            parent.Nodes.Add(section);

            section = new TreeNode();  // level 2
            section.Text = "Position";
            parent.Nodes.Add(section);

            TreeNode entry = new TreeNode();  // level 3
            entry.Text = "X: " + control.Location.X.ToString();
            entry.Tag = "XPos";
            section.Nodes.Add(entry);

            entry = new TreeNode();  // level 3
            entry.Text = "Y: " + control.Location.Y.ToString();
            entry.Tag = "YPos";
            section.Nodes.Add(entry);

            entry = new TreeNode();  // level 3
            entry.Text = "W: " + control.Size.Width.ToString();
            //entry.Tag = "YPos";
            section.Nodes.Add(entry);

            entry = new TreeNode();  // level 3
            entry.Text = "H: " + control.Size.Height.ToString();
            //entry.Tag = "YPos";
            section.Nodes.Add(entry);


            //string X = control.Location.X.ToString();
            //string Y = control.Location.Y.ToString();
            //string H = control.Size.Height.ToString();
            //string W = control.Size.Width.ToString();

            section = new TreeNode();  // level 2
            section.Text = "Padding";
            parent.Nodes.Add(section);

            entry = new TreeNode();  // level 3
            entry.Text = "Left: " + control.Padding.Left;
            //entry.Tag = "YPos";
            section.Nodes.Add(entry);

            entry = new TreeNode();  // level 3
            entry.Text = "Right: " + control.Padding.Right;
            //entry.Tag = "YPos";
            section.Nodes.Add(entry);

            entry = new TreeNode();  // level 3
            entry.Text = "Top: " + control.Padding.Top;
            //entry.Tag = "YPos";
            section.Nodes.Add(entry);

            entry = new TreeNode();  // level 3
            entry.Text = "Bottom: " + control.Padding.Bottom;
            //entry.Tag = "YPos";
            section.Nodes.Add(entry);

            //int controlPadLeft = control.Padding.Left;
            //int controlPadRight = control.Padding.Right;
            //int controlPadTop = control.Padding.Top;
            //int controlPadBottom = control.Padding.Bottom;


            section = new TreeNode();  // level 2
            section.Text = "Graphics";
            parent.Nodes.Add(section);

            entry = new TreeNode();  // level 3
            entry.Text = "Text Color: " + control.ForeColor.ToString();
            //entry.Tag = "YPos";
            section.Nodes.Add(entry);

            entry = new TreeNode();  // level 3
            entry.Text = "Back Color: " + control.BackColor.ToString();
            //entry.Tag = "YPos";
            section.Nodes.Add(entry);

            Image backImage = control.BackgroundImage; string ImageTag = "";
            if (backImage != null) { try { ImageTag = backImage.Tag.ToString(); } catch { } }
            entry = new TreeNode();  // level 3
            entry.Text = "Image: " + ImageTag;
            //entry.Tag = "YPos";
            section.Nodes.Add(entry);

            //Color textColor = control.ForeColor;
            //Color backColor = control.BackColor;
            //Image backImage = control.BackgroundImage;

            section = new TreeNode();  // level 2
            section.Text = "Control Info";
            parent.Nodes.Add(section);

            entry = new TreeNode();  // level 3
            entry.Text = "Has Children: " + control.HasChildren.ToString();
            //entry.Tag = "YPos";
            section.Nodes.Add(entry);

            entry = new TreeNode();  // level 3
            entry.Text = "Enabled: " + control.Enabled.ToString();
            //entry.Tag = "YPos";
            section.Nodes.Add(entry);

            entry = new TreeNode();  // level 3
            entry.Text = "Allow Drop: " + control.AllowDrop.ToString();
            //entry.Tag = "YPos";
            section.Nodes.Add(entry);

            entry = new TreeNode();  // level 3
            entry.Text = "Control Form: " + control.FindForm().Name.ToString();
            //entry.Tag = "YPos";
            section.Nodes.Add(entry);

            entry = new TreeNode();  // level 3
            entry.Text = "Control Parent: " + control.Parent.Name.ToString();
            //entry.Tag = "YPos";
            section.Nodes.Add(entry);

            entry = new TreeNode();  // level 3
            entry.Text = "Control Handle: " + control.Handle.ToString();
            //entry.Tag = "YPos";
            section.Nodes.Add(entry);

            entry = new TreeNode();  // level 3
            entry.Text = "Control Count: " + control.Controls.Count.ToString();
            //entry.Tag = "YPos";
            section.Nodes.Add(entry);

            entry = new TreeNode();  // level 3
            entry.Text = "Dock: " + control.Dock.ToString();
            //entry.Tag = "YPos";
            section.Nodes.Add(entry);

            // Add List of Events + Event Handler Functions for Control
            List<string> handlers = Return_Event_Handlers(control);

            section = new TreeNode();  // level 2
            section.Text = "Event Handlers";
            parent.Nodes.Add(section);

            foreach (string handle in handlers)
            {
                entry = new TreeNode();  // level 3
                entry.Text = handle;
                //entry.Tag = "YPos";
                section.Nodes.Add(entry);
            }


            //string HasChildren = control.HasChildren.ToString();
            //string Enabled = control.Enabled.ToString();
            //string AllowDrop = control.AllowDrop.ToString(); 
            //Form controlForm = control.FindForm();
            //Control controlParent = control.Parent;
            //IntPtr controlHwnd = control.Handle;
            //DockStyle dock = control.Dock;
            //int controlCount = control.Controls.Count;


            TV.Nodes.Add(parent);  // populate tree

            if (ExpandTV) { tv.Expand(TV); }
        }


        /// <summary>
        /// Check To See if Object Type = Button
        /// </summary>
        /// <param name="Control">ContrlName To Check</param>
        /// <returns>Returns True If Object Is Button</returns>
        public bool IsButton(object Control)
        {
            string ControlType = Control.GetType().ToString();  //determine what kind of variable was passed into function
            if (ControlType == "System.Windows.Forms.Button") { return true; }
            return false;
        }
        /// <summary>
        /// Check To See if Object Type = CheckBox
        /// </summary>
        /// <param name="Control">ContrlName To Check</param>
        /// <returns>Returns True If Object Is CheckBox</returns>
        public bool IsCheckBox(object Control)
        {
            string ControlType = Control.GetType().ToString();  //determine what kind of variable was passed into function
            if (ControlType == "System.Windows.Forms.CheckBox") { return true; }
            return false;
        }
        /// <summary>
        /// Check To See if Object Type = DataGridView
        /// </summary>
        /// <param name="Control">ContrlName To Check</param>
        /// <returns>Returns True If Object Is DataGridView</returns>
        public bool IsDataGridView(object Control)
        {
            string ControlType = Control.GetType().ToString();  //determine what kind of variable was passed into function
            if (ControlType == "System.Windows.Forms.DataGridView") { return true; }
            return false;
        }
        /// <summary>
        /// Check To See if Object Type = ListBox 
        /// </summary>
        /// <param name="Control">ContrlName To Check</param>
        /// <returns>Returns True If Object Is ListBox</returns>
        public bool IsListBox(object Control)
        {
            string ControlType = Control.GetType().ToString();  //determine what kind of variable was passed into function
            if (ControlType == "System.Windows.Forms.ListBox") { return true; }
            return false;
        }
        /// <summary>
        /// Check To See if Object Type = PictureBox
        /// </summary>
        /// <param name="Control">ContrlName To Check</param>
        /// <returns>Returns True If Object Is PictureBox</returns>
        public bool IsPictureBox(object Control)
        {
            string ControlType = Control.GetType().ToString();  //determine what kind of variable was passed into function
            if (ControlType == "System.Windows.Forms.PictureBox") { return true; }
            return false;
        }
        /// <summary>
        /// Check To See if Object Type = TabControl
        /// </summary>
        /// <param name="Control">ContrlName To Check</param>
        /// <returns>Returns True If Object Is TabControl</returns>
        public bool IsTabControl(object Control)
        {
            string ControlType = Control.GetType().ToString();  //determine what kind of variable was passed into function
            if (ControlType == "System.Windows.Forms.TabControl") { return true; }
            return false;
        }
        /// <summary>
        /// Check To See if Object Type = TabPage
        /// </summary>
        /// <param name="Control">ContrlName To Check</param>
        /// <returns>Returns True If Object Is TabPage</returns>
        public bool IsTabPage(object Control)
        {
            string ControlType = Control.GetType().ToString();  //determine what kind of variable was passed into function
            if (ControlType == "System.Windows.Forms.TabPage") { return true; }
            return false;
        }
        /// <summary>
        /// Check To See if Object Type = Panel
        /// </summary>
        /// <param name="Control">ContrlName To Check</param>
        /// <returns>Returns True If Object Is Panel</returns>
        public bool IsPanel(object Control)
        {
            string ControlType = Control.GetType().ToString();  //determine what kind of variable was passed into function
            if (ControlType == "System.Windows.Forms.TableLayoutPanel") { return true; }
            return false;
        }
        /// <summary>
        /// Check To See if Object Type = TextBox
        /// </summary>
        /// <param name="Control">ContrlName To Check</param>
        /// <returns>Returns True If Object Is TextBox</returns>
        public bool IsTextBox(object Control)
        {
            string ControlType = Control.GetType().ToString();  //determine what kind of variable was passed into function
            if (ControlType == "System.Windows.Forms.TextBox") { return true; }
            return false;
        }
        /// <summary>
        /// Check To See if Object Type = ToolStripMenuItem
        /// </summary>
        /// <param name="Control">ContrlName To Check</param>
        /// <returns>Returns True If Object Is ToolStripMenuItem</returns>
        public bool IsToolStripMenuItem(object Control)
        {
            string ControlType = Control.GetType().ToString();  //determine what kind of variable was passed into function
            if (ControlType == "System.Windows.Forms.ToolStripMenuItem") { return true; }
            return false;
        }
        /// <summary>
        /// Check To See if Object Type = TreeView
        /// </summary>
        /// <param name="Control">ContrlName To Check</param>
        /// <returns>Returns True If Object Is TreeView</returns>
        public bool IsTreeView(object Control)
        {
            string ControlType = Control.GetType().ToString();  //determine what kind of variable was passed into function
            if (ControlType == "System.Windows.Forms.TreeView") { return true; }
            return false;
        }




        #endregion

        #region === Control PopUp Options ===

        // popup control over current control with options

        public void Popup_ListBox(object sender = null, EventArgs e = null)  // build / display popup listView
        {
            // ex button click:
            //      Popup_ListBox((Button)sender);  // build / display popup listView

            Form fromForm = ((Control)sender).FindForm();   // the Form the Control is located on 

            Control btn = LastControlUnderMouse;

            // create fake items list
            List<string> strings = new List<string>();
            for (int i = 0; i < 36; i++)
                strings.Add("ITEM " + (i + 1));
            var listViewItems = strings.Select(x => new ListViewItem(x, 0)).ToArray();

            // create a new list view
            ListView listView = new ListView();
            listView.View = View.SmallIcon;
            //listView.SmallImageList = imageList1;
            listView.MultiSelect = false;

            // add items to listview
            listView.Items.AddRange(listViewItems);

            // calculate size of list from the listViewItems' height
            int itemToShow = 18;
            var lastItemToShow = listViewItems.Take(itemToShow).Last();
            int height = lastItemToShow.Bounds.Bottom + listView.Margin.Top;
            listView.Height = height;

            // create a new popup and add the list view to it
            var popup = new ToolStripDropDown();
            popup.AutoSize = false;
            popup.Margin = Padding.Empty;
            popup.Padding = Padding.Empty;
            ToolStripControlHost host = new ToolStripControlHost(listView);
            host.Margin = Padding.Empty;
            host.Padding = Padding.Empty;
            host.AutoSize = false;
            host.Size = listView.Size;
            popup.Size = listView.Size;
            popup.Items.Add(host);


            // change some properties (for selection) and subscribe the ItemActivate 
            // event of the listView
            listView.HotTracking = true;
            listView.Activation = ItemActivation.OneClick;
            listView.ItemActivate += new EventHandler(ListBox_Popup_Action);


            // show the popup
            popup.Show(fromForm, btn.Right, btn.Top);
        }

        public void ListBox_Popup_Action(object sender, EventArgs e)  // the click on the item invokes this method
        {
            var listview = sender as ListView;
            var item = listview.SelectedItems[0].ToString();
            var dropdown = listview.Parent as ToolStripDropDown;
            // unsubscribe the event (to avoid memory leaks)
            listview.SelectedIndexChanged -= ListBox_Popup_Action;
            // close the dropdown (if you want)
            dropdown.Close();

            // do whatever you want with the item
            MessageBox.Show("Selected item is: " + item);
        }


        public void TreeView_Popup(Control control)
        {
            //Button btn = ((Button)sender);

            TreeView treeView = new TreeView();
            treeView.Height = 200;

            // create a new popup and add the list view to it
            var popup = new ToolStripDropDown();
            popup.AutoSize = false;
            popup.Margin = Padding.Empty;
            popup.Padding = Padding.Empty;
            ToolStripControlHost host = new ToolStripControlHost(treeView);
            host.Margin = Padding.Empty;
            host.Padding = Padding.Empty;
            host.AutoSize = false;
            host.Size = treeView.Size;
            popup.Size = treeView.Size;
            popup.Items.Add(host);

            // show the popup
            //popup.Show(btn, Cursor.Position.X, btn.Top - 50);
            popup.Show(control, control.Left, control.Top - 50);
            //popup.Show(btn, btn.Right, btn.Top);
        }

        public void ListView_Popup(Control control)
        {
            //Button btn = ((Button)sender);

            // create fake items list
            List<string> strings = new List<string>();
            for (int i = 0; i < 36; i++)
                strings.Add("ITEM " + (i + 1));
            var listViewItems = strings.Select(x => new ListViewItem(x, 0)).ToArray();

            // create a new list view
            ListView listView = new ListView();
            listView.View = View.SmallIcon;
            //listView.SmallImageList = imageList1;
            listView.MultiSelect = false;

            // add items to listview
            listView.Items.AddRange(listViewItems);

            listView.SelectedIndexChanged += new System.EventHandler(ListView_SelectedIndexChanged);

            // calculate size of list from the listViewItems' height
            int itemToShow = 18;
            var lastItemToShow = listViewItems.Take(itemToShow).Last();
            int height = lastItemToShow.Bounds.Bottom + listView.Margin.Top;
            listView.Height = height;

            // create a new popup and add the list view to it
            var popup = new ToolStripDropDown();
            popup.AutoSize = false;
            popup.Margin = Padding.Empty;
            popup.Padding = Padding.Empty;
            ToolStripControlHost host = new ToolStripControlHost(listView);
            host.Margin = Padding.Empty;
            host.Padding = Padding.Empty;
            host.AutoSize = false;
            host.Size = listView.Size;
            popup.Size = listView.Size;
            popup.Items.Add(host);

            // show the popup
            //popup.Show(btn, Cursor.Position.X, btn.Top - 50);
            popup.Show(control, control.Left, control.Top - 50);
            //popup.Show(btn, btn.Right, btn.Top);
        }

        private void ListView_SelectedIndexChanged(object sender, EventArgs e)
        {

            ListView LV = (ListView)sender;
            string SelectedText = "";
            string SelectedTag = "";

            if (LV.SelectedItems.Count > 0)
            {
                //SelectedTag = listView1.SelectedItems[0].Tag.ToString();  // return icon's tag / filepath
                SelectedText = LV.SelectedItems[0].Text.ToString(); // return text from icon
                ahk.MsgBox(SelectedText);

                var dropdown = LV.Parent as ToolStripDropDown;  // close menu after selection
            }

        }


        #endregion

        #region === Form Controls - List / Write To Db ===

        // reads controls off of Form - broken down by control type + contents of Menus / TreeView Nodes / Tabs
        public void Form_Controls_Tree(Form formNAME, TreeView TV, bool ClearTV = true, bool ExpandTV = false, bool NodesAndMenuItems = true)
        {
            _TreeViewControl tv = new _TreeViewControl();
            _Lists lst = new _Lists();
            _MenuControl menu = new _MenuControl();

            if (ClearTV) { tv.ClearTV(TV); }

            //ParseForm = (Form)formNAME;  // form name passed in as parameter
            List<Control> controls = lst.Control_List(formNAME);  // returns list of controls on a form

            List<string> controlTypeList = new List<string>();
            List<string> controlList = new List<string>();

            // create uniqute list of control types found on the Form
            foreach (Control control in controls)
            {
                string type = control.GetType().ToString();

                // create list of unique control types on Form
                if (!lst.InList(controlTypeList, type))
                {
                    controlTypeList.Add(type);
                }
            }


            //List<List<string>> listList = new List<List<string>>();

            foreach (string cType in controlTypeList)
            {
                string thisType = cType;
                List<string> thisTypeList = new List<string>();

                foreach (Control control in controls)
                {
                    string type = control.GetType().ToString();

                    if (type == thisType)
                    {
                        string tag = ""; try { tag = control.Tag.ToString(); }
                        catch { }

                        thisTypeList.Add(control.Text + "|" + control.Name);
                    }
                }

                thisType = ahk.StringReplace(thisType, "System.Windows.Forms.");
                //listList.Add(thisTypeList);
                lst.List_To_TreeView(TV, thisTypeList, thisType, false, false);
            }


            //### Parse Menus / TreeViews For Items ###

            if (NodesAndMenuItems)
            {
                List<string> nodeList = new List<string>();
                List<string> menuItems = new List<string>();
                List<string> buttonItems = new List<string>();
                List<string> pictureBoxItems = new List<string>();
                List<string> tabPageItems = new List<string>();

                foreach (Control control in controls)
                {
                    //if (control.GetType().ToString() == "System.Windows.Forms.TreeView") { continue; }
                    string tag = ""; try { tag = control.Tag.ToString(); }
                    catch { }

                    string ControlType = control.GetType().ToString();


                    // Add Nodes from each TreeView Control to List

                    if (ControlType == "System.Windows.Forms.TreeView")
                    {
                        TreeView ReadTree = (TreeView)control;
                        List<TreeNode> Nodes = tv.NodeList(ReadTree, false);
                        foreach (TreeNode node in Nodes)
                        {
                            tag = ""; try { tag = node.Tag.ToString(); }
                            catch { }
                            nodeList.Add(node.Text + "|" + tag);
                        }
                    }
                    if (ControlType == "System.Windows.Forms.MenuStrip")
                    {
                        MenuStrip mnu = (MenuStrip)control;
                        menuItems = menu.Menu_Item_List(mnu, false);
                    }
                    if (ControlType == "System.Windows.Forms.Button")
                    {
                        Button mnu = (Button)control;
                        tag = ""; try { tag = mnu.Tag.ToString(); }
                        catch { }
                        buttonItems.Add(mnu.Name + "|" + tag);
                    }
                    if (ControlType == "System.Windows.Forms.TabPage")
                    {
                        TabPage mnu = (TabPage)control;
                        tag = ""; try { tag = mnu.Tag.ToString(); }
                        catch { }
                        tabPageItems.Add(mnu.Name + "|" + tag);
                    }
                    if (ControlType == "System.Windows.Forms.PictureBox")
                    {
                        PictureBox mnu = (PictureBox)control;
                        tag = ""; try { tag = mnu.Tag.ToString(); }
                        catch { }
                        pictureBoxItems.Add(mnu.Name + "|" + tag);
                    }


                }

                if (nodeList.Count > 0) { lst.List_To_TreeView(TV, nodeList, "TreeView Nodes", false, false); }
                if (menuItems.Count > 0) { lst.List_To_TreeView(TV, menuItems, "Menu Items", false, false); }
                //if (buttonItems.Count > 0) { lst.List_To_TreeView(TV, buttonItems, "Buttons", false, false); }
                if (tabPageItems.Count > 0) { lst.List_To_TreeView(TV, tabPageItems, "TabPage", false, false); }
                //if (pictureBoxItems.Count > 0) { lst.List_To_TreeView(TV, pictureBoxItems, "PictureBox", false, false); }
            }

            if (ExpandTV) { tv.Expand(TV); }

        }

        // writes SQLite table with every control on the Form + Menu + TreeNode Items (Controls That can be populated with Icons)
        public void Form_Controls_Db_Write(string DbFile, Form formNAME)
        {
            _Lists lst = new _Lists();
            _TreeViewControl tv = new _TreeViewControl();
            _MenuControl menu = new _MenuControl();

            //ParseForm = (Form)formNAME;  // form name passed in as parameter
            List<Control> controls = lst.Control_List(formNAME);  // returns list of controls on a form

            bool created = Create_Table_ProjectICO(DbFile);  // create sqlite table structure if it doesn't already exist

            List<string> controlTypeList = new List<string>();
            List<string> controlList = new List<string>();

            // create uniqute list of control types found on the Form
            foreach (Control control in controls)
            {
                string type = control.GetType().ToString();

                // create list of unique control types on Form
                if (!lst.InList(controlTypeList, type))
                {
                    controlTypeList.Add(type);
                }
            }


            //List<List<string>> listList = new List<List<string>>();
            int added = 0;
            foreach (string cType in controlTypeList)
            {
                string thisType = cType;

                foreach (Control control in controls)
                {
                    string type = control.GetType().ToString();

                    if (type == thisType)
                    {
                        string tag = ""; try { tag = control.Tag.ToString(); }
                        catch { }

                        _ProjectICO trol = new _ProjectICO();
                        trol.ProjectName = formNAME.Name.ToString();
                        trol.ControlType = type;
                        trol.ControlName = control.Name;
                        trol.ControlText = control.Text;
                        trol.ControlTag = tag;
                        trol.Exclude = "false";
                        bool inserted = Insert_Object_Into_ProjectICO(DbFile, trol);  // write values to database
                        if (inserted) { added++; }
                        if (!inserted) { ahk.MsgBox("Error While Inserting : " + trol.ControlName); }
                    }
                }
            }


            //### Parse Menus / TreeViews For Items ###

            List<string> nodeList = new List<string>();
            List<string> menuItems = new List<string>();
            List<string> tabPageItems = new List<string>();
            //List<string> buttonItems = new List<string>();
            //List<string> pictureBoxItems = new List<string>();

            foreach (Control control in controls)
            {
                string tag = ""; try { tag = control.Tag.ToString(); }
                catch { }

                string ControlType = control.GetType().ToString();


                // Add Nodes from each TreeView Control to List

                if (ControlType == "System.Windows.Forms.TreeView")
                {
                    TreeView ReadTree = (TreeView)control;
                    List<TreeNode> Nodes = tv.NodeList(ReadTree, false);
                    foreach (TreeNode node in Nodes)
                    {
                        tag = ""; try { tag = node.Tag.ToString(); }
                        catch { }

                        _ProjectICO trol = new _ProjectICO();
                        trol.ProjectName = formNAME.Name.ToString();
                        trol.ControlType = ControlType;
                        trol.ControlName = node.Name;
                        trol.ControlText = node.Text;
                        trol.ControlTag = tag;
                        trol.Exclude = "false";
                        bool inserted = Insert_Object_Into_ProjectICO(DbFile, trol);  // write values to database
                        if (inserted) { added++; }
                        if (!inserted) { ahk.MsgBox("Error While Inserting : " + trol.ControlName); }
                    }
                }
                if (ControlType == "System.Windows.Forms.MenuStrip")
                {
                    MenuStrip mnu = (MenuStrip)control;
                    menuItems = menu.Menu_Item_List(mnu, false);

                    foreach (string mItem in menuItems)
                    {
                        tag = ""; try { tag = mnu.Tag.ToString(); }
                        catch { }

                        _ProjectICO trol = new _ProjectICO();
                        trol.ProjectName = formNAME.Name.ToString();
                        trol.ControlType = ControlType;
                        trol.ControlName = mItem;
                        trol.ControlText = mItem;
                        trol.ControlTag = tag;
                        trol.Exclude = "false";
                        bool inserted = Insert_Object_Into_ProjectICO(DbFile, trol);  // write values to database
                        if (inserted) { added++; }
                        if (!inserted) { ahk.MsgBox("Error While Inserting : " + trol.ControlName); }
                    }

                }
                if (ControlType == "System.Windows.Forms.TabPage")
                {
                    TabPage mnu = (TabPage)control;
                    tag = ""; try { tag = mnu.Tag.ToString(); }
                    catch { }

                    _ProjectICO trol = new _ProjectICO();
                    trol.ProjectName = formNAME.Name.ToString();
                    trol.ControlType = ControlType;
                    trol.ControlName = mnu.Name;
                    trol.ControlText = mnu.Text;
                    trol.ControlTag = tag;
                    trol.Exclude = "false";
                    bool inserted = Insert_Object_Into_ProjectICO(DbFile, trol);  // write values to database
                    if (inserted) { added++; }
                    if (!inserted) { ahk.MsgBox("Error While Inserting : " + trol.ControlName); }
                }

            }

            ahk.MsgBox("Finished Writing Control Info To DataBase\nAdded: " + added.ToString());
        }


        #endregion

        #region === ProjectICO Db ===

        public struct _ProjectICO
        {
            public string ID { get; set; }
            public string ProjectPath { get; set; }
            public string ProjectName { get; set; }
            public string ControlType { get; set; }
            public string ControlName { get; set; }
            public string ControlText { get; set; }
            public string ControlTag { get; set; }
            public string ImagePath { get; set; }
            public string KeyName { get; set; }
            public string Flag { get; set; }
            public string Exclude { get; set; }
            public string DateModified { get; set; }
        }


        public _ProjectICO Return_Object_From_FunctionGrid(DataGridView dv, int RowNum = -1)
        {
            _ProjectICO returnObject = new _ProjectICO();
            if (RowNum < 0) { return returnObject; }
            List<string> colNames = grid.Column_Names(dv);

            returnObject.ID = dv.Rows[RowNum].Cells["ID"].Value.ToString();
            returnObject.ProjectPath = dv.Rows[RowNum].Cells["ProjectPath"].Value.ToString();
            returnObject.ProjectName = dv.Rows[RowNum].Cells["ProjectName"].Value.ToString();
            returnObject.ControlType = dv.Rows[RowNum].Cells["ControlType"].Value.ToString();
            returnObject.ControlName = dv.Rows[RowNum].Cells["ControlName"].Value.ToString();
            returnObject.ControlText = dv.Rows[RowNum].Cells["ControlText"].Value.ToString();
            returnObject.ControlTag = dv.Rows[RowNum].Cells["ControlTag"].Value.ToString();
            returnObject.ImagePath = dv.Rows[RowNum].Cells["ImagePath"].Value.ToString();
            returnObject.KeyName = dv.Rows[RowNum].Cells["KeyName"].Value.ToString();
            returnObject.Flag = dv.Rows[RowNum].Cells["Flag"].Value.ToString();
            returnObject.Exclude = dv.Rows[RowNum].Cells["Exclude"].Value.ToString();
            returnObject.DateModified = dv.Rows[RowNum].Cells["DateModified"].Value.ToString();

            return returnObject;
        }


        string DbFile = @"C:\Users\Jason\Google Drive\IMDB\SQLiter\Db\ProjectICO.sqlite";

        public _ProjectICO Return_Object_From_ProjectICO(string DbFile, string ProjectName = "")
        {
            string SelectLine = "Select [ID], [ProjectPath], [ProjectName], [ControlType], [ControlName], [ControlText], [ControlTag], [ImagePath], [KeyName], [Flag], [Exclude], [DateModified] From [ProjectICO] WHERE ProjectName = '" + ProjectName + "'";
            _ProjectICO returnObject = new _ProjectICO();

            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
            int i = 0;
            string Value = "";
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    returnObject.ID = ret["ID"].ToString();
                    returnObject.ProjectPath = ret["ProjectPath"].ToString();
                    returnObject.ProjectName = ret["ProjectName"].ToString();
                    returnObject.ControlType = ret["ControlType"].ToString();
                    returnObject.ControlName = ret["ControlName"].ToString();
                    returnObject.ControlText = ret["ControlText"].ToString();
                    returnObject.ControlTag = ret["ControlTag"].ToString();
                    returnObject.ImagePath = ret["ImagePath"].ToString();
                    returnObject.KeyName = ret["KeyName"].ToString();
                    returnObject.Flag = ret["Flag"].ToString();
                    returnObject.Exclude = ret["Exclude"].ToString();
                    returnObject.DateModified = ret["DateModified"].ToString();
                }
            }

            return returnObject;
        }

        public bool Insert_Into_ProjectICO(string DbFile, string ID = "", string ProjectPath = "", string ProjectName = "", string ControlType = "", string ControlName = "", string ControlText = "", string ControlTag = "", string ImagePath = "", string KeyName = "", string Flag = "", string Exclude = "", string DateModified = "")
        {
            string InsertLine = "Insert Into [ProjectICO] (ID, ProjectPath, ProjectName, ControlType, ControlName, ControlText, ControlTag, ImagePath, KeyName, Flag, Exclude, DateModified) values ('" + ID + "', '" + ProjectPath + "', '" + ProjectName + "', '" + ControlType + "', '" + ControlName + "', '" + ControlText + "', '" + ControlTag + "', '" + ImagePath + "', '" + KeyName + "', '" + Flag + "', '" + Exclude + "', '" + DateModified + "') WHERE ProjectName = '" + ProjectName + "'";
            bool Inserted = sqlite.Execute(DbFile, InsertLine);
            if (GlobalDebug) { ahk.MsgBox("Inserted Into [ProjectICO] = " + Inserted.ToString()); }
            return Inserted;
        }

        public bool Insert_Object_Into_ProjectICO(string DbFile, _ProjectICO inObject)
        {
            string InsertLine = "Insert Into [ProjectICO] (ID, ProjectPath, ProjectName, ControlType, ControlName, ControlText, ControlTag, ImagePath, KeyName, Flag, Exclude, DateModified) values ('" + inObject.ID + "', '" + inObject.ProjectPath + "', '" + inObject.ProjectName + "', '" + inObject.ControlType + "', '" + inObject.ControlName + "', '" + inObject.ControlText + "', '" + inObject.ControlTag + "', '" + inObject.ImagePath + "', '" + inObject.KeyName + "', '" + inObject.Flag + "', '" + inObject.Exclude + "', '" + inObject.DateModified + "') WHERE ProjectName = '" + inObject.ProjectName + "'";
            bool Inserted = sqlite.Execute(DbFile, InsertLine);
            if (GlobalDebug) { ahk.MsgBox("Inserted Into [ProjectICO] = " + Inserted.ToString()); }
            return Inserted;
        }



        public bool Update_ProjectICO(string DbFile, string ID = "", string ProjectPath = "", string ProjectName = "", string ControlType = "", string ControlName = "", string ControlText = "", string ControlTag = "", string ImagePath = "", string KeyName = "", string Flag = "", string Exclude = "", string DateModified = "")
        {
            string UpdateLine = "Update [ProjectICO] set ID = '" + ID + "', ProjectPath = '" + ProjectPath + "', ProjectName = '" + ProjectName + "', ControlType = '" + ControlType + "', ControlName = '" + ControlName + "', ControlText = '" + ControlText + "', ControlTag = '" + ControlTag + "', ImagePath = '" + ImagePath + "', KeyName = '" + KeyName + "', Flag = '" + Flag + "', Exclude = '" + Exclude + "', DateModified = '" + DateModified + "' WHERE ProjectName = '" + ProjectName + "'";
            bool Updated = sqlite.Execute(DbFile, UpdateLine);
            if (GlobalDebug) { ahk.MsgBox("Updated [ProjectICO] = " + Updated.ToString()); }
            return Updated;
        }



        public bool Update_ProjectICO_FromObject(string DbFile, _ProjectICO inObject)
        {
            string UpdateLine = "Update [ProjectICO] set ID = '" + inObject.ID + "', ProjectPath = '" + inObject.ProjectPath + "', ProjectName = '" + inObject.ProjectName + "', ControlType = '" + inObject.ControlType + "', ControlName = '" + inObject.ControlName + "', ControlText = '" + inObject.ControlText + "', ControlTag = '" + inObject.ControlTag + "', ImagePath = '" + inObject.ImagePath + "', KeyName = '" + inObject.KeyName + "', Flag = '" + inObject.Flag + "', Exclude = '" + inObject.Exclude + "', DateModified = '" + inObject.DateModified + "' WHERE ProjectName = '" + inObject.ProjectName + "'";
            bool Updated = sqlite.Execute(DbFile, UpdateLine);
            if (GlobalDebug) { ahk.MsgBox("Updated [ProjectICO] = " + Updated.ToString()); }
            return Updated;
        }



        public bool Create_Table_ProjectICO(string DbFile)
        {
            string NewTableName = "[ProjectICO]";
            string dBFile = @"C:\Users\Jason\Google Drive\IMDB\SQLiter\Db\ProjectICO.sqlite";
            string NewTableLine = "[ID] INTEGER PRIMARY KEY, [ProjectPath] VARCHAR, [ProjectName] VARCHAR, [ControlType] VARCHAR, [ControlName] VARCHAR, [ControlText] VARCHAR, [ControlTag] VARCHAR, [ImagePath] VARCHAR, [KeyName] VARCHAR, [Flag] VARCHAR, [Exclude] VARCHAR, [DateModified] VARCHAR";
            bool Created = sqlite.Table_New(dBFile, NewTableName, NewTableLine, false);
            return Created;
        }



        public DataTable Create_ProjectICO_DataTable(_ProjectICO inObject)
        {
            DataTable table = new DataTable();
            table.Columns.Add("ID", typeof(string));
            table.Columns.Add("ProjectPath", typeof(string));
            table.Columns.Add("ProjectName", typeof(string));
            table.Columns.Add("ControlType", typeof(string));
            table.Columns.Add("ControlName", typeof(string));
            table.Columns.Add("ControlText", typeof(string));
            table.Columns.Add("ControlTag", typeof(string));
            table.Columns.Add("ImagePath", typeof(string));
            table.Columns.Add("KeyName", typeof(string));
            table.Columns.Add("Flag", typeof(string));
            table.Columns.Add("Exclude", typeof(string));
            table.Columns.Add("DateModified", typeof(string));

            table.Rows.Add(inObject.ID, inObject.ProjectPath, inObject.ProjectName, inObject.ControlType, inObject.ControlName, inObject.ControlText, inObject.ControlTag, inObject.ImagePath, inObject.KeyName, inObject.Flag, inObject.Exclude, inObject.DateModified);
            return table;
        }



        public void HideShow_ProjectICO_Columns(DataGridView dv)
        {

            try { dv.Columns["ID"].Visible = true; }
            catch { }
            try { dv.Columns["ProjectPath"].Visible = true; }
            catch { }
            try { dv.Columns["ProjectName"].Visible = true; }
            catch { }
            try { dv.Columns["ControlType"].Visible = true; }
            catch { }
            try { dv.Columns["ControlName"].Visible = true; }
            catch { }
            try { dv.Columns["ControlText"].Visible = true; }
            catch { }
            try { dv.Columns["ControlTag"].Visible = true; }
            catch { }
            try { dv.Columns["ImagePath"].Visible = true; }
            catch { }
            try { dv.Columns["KeyName"].Visible = true; }
            catch { }
            try { dv.Columns["Flag"].Visible = true; }
            catch { }
            try { dv.Columns["Exclude"].Visible = true; }
            catch { }
            try { dv.Columns["DateModified"].Visible = true; }
            catch { }
        }


        #endregion

        #region === User Input ===

        public DialogResult Grid_InputBox(List<string> DVList, string title, string promptText, ref string value, string OKButton = "OK", string CancelButton = "Cancel")  // user input box that promps user for input
        {
            _GridControl grid = new _GridControl();
            _Lists lst = new _Lists();

            //EX: 
            //string value = "New List Name";
            //if (ahk.InputBox("Enter New List Name: ", "", ref value) == DialogResult.OK)
            //{
            //    string UserEntry = value;
            //    bool Inserted = sqlite.InsertListItem(ahkGlobal.UserDb, "UserLists", UserEntry, "", "", "0");
            //    Load_UserList_InGrid(ahkGlobal.UserDb, UserEntry);
            //    PopulateListDDL();
            //}


            Form DialogForm = new Form();
            DataGridView dvGrid = new DataGridView();
            Label label = new Label();
            //TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();


            DialogForm.Text = title;
            label.Text = promptText;
            //textBox.Text = value;

            buttonOk.Text = OKButton;
            buttonCancel.Text = CancelButton;
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;



            label.SetBounds(9, 20, 372, 13);
            //textBox.SetBounds(12, 36, 372, 20);

            dvGrid.SetBounds(12, 36, 372, 220);

            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            dvGrid.Anchor = dvGrid.Anchor | AnchorStyles.Right;
            //textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            DialogForm.ClientSize = new Size(396, 107);
            DialogForm.Controls.AddRange(new Control[] { label, dvGrid, buttonOk, buttonCancel });
            DialogForm.ClientSize = new Size(Math.Max(400, label.Right + 10), DialogForm.ClientSize.Height + 200);
            DialogForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            DialogForm.StartPosition = FormStartPosition.CenterScreen;
            DialogForm.TopMost = true;
            DialogForm.MinimizeBox = false;
            DialogForm.MaximizeBox = false;
            DialogForm.AcceptButton = buttonOk;
            DialogForm.CancelButton = buttonCancel;


            lst.List_To_Grid(dvGrid, DVList, "Select List Item", true);

            DialogResult dialogResult = DialogForm.ShowDialog();
            //value = textBox.Text;
            return dialogResult;
        }

        public DialogResult SelectIcon(string IconDir, ref string value, string title = "", string promptText = "", string OKButton = "OK", string CancelButton = "Cancel")  // user input box that promps user for input
        {
            // ex:
            //// prompt user to select icon from IconLib

            //string SelectedICO = "";
            //control.SelectIcon(@"C:\Users\jason\Google Drive\IMDB.v111\SQLiter\Launch\bin\Debug\ICO_Temp", ref SelectedICO);
            //if (SelectedICO != "")
            //{
            //    //ahk.MsgBox(SelectedICO);
            //    //Image il.Return_Image_From_ImageList(ImageList IL, string KeyName = "png_arrow-down-icon.png") 

            //    //button9.Image = Image.FromFile(SelectedICO);
            //    button9.BackgroundImage = Image.FromFile(SelectedICO);
            //    button9.ImageAlign = ContentAlignment.MiddleRight;
            //    button9.TextAlign = ContentAlignment.MiddleLeft;
            //}




            _Lists lst = new _Lists();
            _Images img = new _Images();


            Form DialogForm = new Form();
            ListView LV = new ListView();
            Label label = new Label();
            //TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();


            DialogForm.Text = title;
            label.Text = promptText;
            //textBox.Text = value;

            buttonOk.Text = OKButton;
            buttonCancel.Text = CancelButton;
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;



            label.SetBounds(9, 20, 372, 13);
            //textBox.SetBounds(12, 36, 372, 20);

            LV.SetBounds(12, 36, 372, 220);

            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            LV.Anchor = LV.Anchor | AnchorStyles.Right;
            //textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            DialogForm.ClientSize = new Size(396, 107);
            DialogForm.Controls.AddRange(new Control[] { label, LV, buttonOk, buttonCancel });
            DialogForm.ClientSize = new Size(Math.Max(400, label.Right + 10), DialogForm.ClientSize.Height + 200);
            DialogForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            DialogForm.StartPosition = FormStartPosition.CenterScreen;
            DialogForm.TopMost = true;
            DialogForm.MinimizeBox = false;
            DialogForm.MaximizeBox = false;
            DialogForm.AcceptButton = buttonOk;
            DialogForm.CancelButton = buttonCancel;


            img.ListView_Icons(LV, IconDir);   // create imagelist and populate ListView with icons in folder path

            //lst.List_To_Grid(LV, DVList, "Select List Item", true);

            //if (saveFileDialog1.ShowDialog() == DialogResult.OK)

            //DialogResult dialogResult = DialogForm; 

            string SelectedImagePath = "";

            //DialogResult dialogResult = DialogForm.

            //if (DialogForm.ShowDialog() == DialogResult.OK)
            //{
            //    dialogResult = DialogResult.OK;

            //    if (LV.SelectedItems.Count > 0)
            //    {
            //        if (LV.SelectedItems[0].Tag != null)
            //        {
            //            SelectedImagePath = LV.SelectedItems[0].Tag.ToString();
            //        }
            //    }
            //}

            DialogResult dialogResult = DialogForm.ShowDialog();

            if (dialogResult != DialogResult.OK) { return dialogResult; }  // user cancel - no image to return

            bool ReturnTag = true;

            if (LV.SelectedItems.Count > 0)
            {
                if (ReturnTag) // return icon's tag / filepath
                {
                    if (LV.SelectedItems[0].Tag != null)
                    {
                        SelectedImagePath = LV.SelectedItems[0].Tag.ToString();
                    }
                }

                if (!ReturnTag) // return text from icon
                {
                    if (LV.SelectedItems[0].Text != null)
                    {
                        SelectedImagePath = LV.SelectedItems[0].Text.ToString();
                    }
                }
            }


            value = SelectedImagePath;
            return dialogResult;
        }

        public Image SelectIcon_From_ImageList(ImageList IconList, ref string value, string title = "Select Icon From ImageList", string promptText = "Select Icon", string OKButton = "OK", string CancelButton = "Cancel")  // user input box that promps user for input
        {
            _Lists lst = new _Lists();
            _Images img = new _Images();
            //Dev.cOntrols controls = new Dev.cOntrols();


            Form DialogForm = new Form();
            ListView LV = new ListView();
            Label label = new Label();
            //TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();


            DialogForm.Text = title;
            label.Text = promptText;
            //textBox.Text = value;

            buttonOk.Text = OKButton;
            buttonCancel.Text = CancelButton;
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;



            label.SetBounds(9, 20, 372, 13);
            //textBox.SetBounds(12, 36, 372, 20);

            LV.SetBounds(12, 36, 372, 220);

            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            LV.Anchor = LV.Anchor | AnchorStyles.Right;
            //textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            DialogForm.ClientSize = new Size(396, 107);
            DialogForm.Controls.AddRange(new Control[] { label, LV, buttonOk, buttonCancel });
            DialogForm.ClientSize = new Size(Math.Max(400, label.Right + 10), DialogForm.ClientSize.Height + 200);
            DialogForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            DialogForm.StartPosition = FormStartPosition.CenterScreen;
            DialogForm.TopMost = true;
            DialogForm.MinimizeBox = false;
            DialogForm.MaximizeBox = false;
            DialogForm.AcceptButton = buttonOk;
            DialogForm.CancelButton = buttonCancel;

            img.ImageList_ListView_Display(LV, IconList);  // take imagelist and populate ListView with icons

            string SelectedImagePath = "";
            DialogResult dialogResult = DialogForm.ShowDialog();
            if (dialogResult != DialogResult.OK) { return null; }  // user canceled - no image to return

            bool ReturnTag = true; // toggle between tag value and display text

            if (LV.SelectedItems.Count > 0)
            {
                if (ReturnTag) // return icon's tag / filepath
                {
                    if (LV.SelectedItems[0].Tag != null)
                    {
                        SelectedImagePath = LV.SelectedItems[0].Tag.ToString();
                    }
                }

                if (!ReturnTag) // return text from icon
                {
                    if (LV.SelectedItems[0].Text != null)
                    {
                        SelectedImagePath = LV.SelectedItems[0].Text.ToString();
                    }
                }

            }

            //IconList.ImageSize = new Size(4, 4);
            //IconList.ColorDepth = ColorDepth.Depth32Bit;

            Image returnImage = img.From_ImageList(IconList, SelectedImagePath);  // search imagelist for key (file name) - returns image 


            value = SelectedImagePath;
            return returnImage;
        }

        #endregion

        #region === Get Controls On Gui ===

        public IEnumerable<Control> GetAllControlsByType(Control formName, Type type)  // returns list of controls by control type
        {
            //var c = GetAll(this, typeof(TextBox));
            //MessageBox.Show("Total Controls: " + c.Count());  // foreach loop works to parse control info

            var controls = formName.Controls.Cast<Control>();

            //return controls.SelectMany(ctrl => GetAllControlsByType(ctrl, type))  // returns all controls of that control type
            //                          .Concat(controls)
            //                          .Where(c => c.GetType() == type);

            return controls.SelectMany(ctrl => GetAllControlsByType(ctrl, type))
                                      .Concat(controls);
        }

        public IEnumerable<Control> GetAllControls(Control formName, Type type)  // returns list of controls on a WinForm
        {
            //var c = GetAll(this, typeof(TextBox));
            //MessageBox.Show("Total Controls: " + c.Count());  // foreach loop works to parse control info

            //type = typeof(TextBox);
            type = typeof(Control);

            var controls = formName.Controls.Cast<Control>();

            //return controls.SelectMany(ctrl => GetAllControlsByType(ctrl, type))  // returns all controls of that control type
            //                          .Concat(controls)
            //                          .Where(c => c.GetType() == type);

            return controls.SelectMany(ctrl => GetAllControlsByType(ctrl, type))
                                      .Concat(controls);
        }

        public Button GetButton(Control formName, string ButtonName = "button1")
        {
            Button btn = (Button)GetControl(formName, ButtonName);
            return btn;
        }
        public IEnumerable<Control> GetButtons(Control formName)  // returns list of Button controls on a WinForm
        {
            Type type = typeof(Button);

            var controls = formName.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAllControlsByType(ctrl, type))  // returns all controls of that control type
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        public TextBox GetTextBox(Control formName, string TextBoxName = "textBox1")
        {
            TextBox btn = (TextBox)GetControl(formName, TextBoxName);
            return btn;
        }
        public IEnumerable<Control> GetTextBoxes(Control formName)  // returns list of TextBox controls on a WinForm
        {
            Type type = typeof(TextBox);

            var controls = formName.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAllControlsByType(ctrl, type))  // returns all controls of that control type
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        public DataGridView GetDataGridView(Control formName, string DataGridViewName = "dataGridView1")
        {
            DataGridView btn = (DataGridView)GetControl(formName, DataGridViewName);
            return btn;
        }
        public IEnumerable<Control> GetDataGridViews(Control formName)  // returns list of DataGridView controls on a WinForm
        {
            Type type = typeof(DataGridView);

            var controls = formName.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAllControlsByType(ctrl, type))  // returns all controls of that control type
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        public TreeView GetTreeView(Control formName, string TreeViewName = "treeView1")
        {
            TreeView btn = (TreeView)GetControl(formName, TreeViewName);
            return btn;
        }
        public IEnumerable<Control> GetTreeViews(Control formName)  // returns list of TreeView controls on a WinForm
        {
            Type type = typeof(TreeView);

            var controls = formName.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAllControlsByType(ctrl, type))  // returns all controls of that control type
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        public ListView GetListView(Control formName, string ListViewName = "listView1")
        {
            ListView btn = (ListView)GetControl(formName, ListViewName);
            return btn;
        }
        public IEnumerable<Control> GetListViews(Control formName)  // returns list of ListView controls on a WinForm
        {
            Type type = typeof(ListView);

            var controls = formName.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAllControlsByType(ctrl, type))  // returns all controls of that control type
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        public Scintilla GetScintilla(Control formName, string ScintillaName = "scintilla1")
        {
            Scintilla btn = (Scintilla)GetControl(formName, ScintillaName);
            return btn;
        }
        public IEnumerable<Control> GetScintillas(Control formName)  // returns list of Scintilla controls on a WinForm
        {
            Type type = typeof(Scintilla);

            var controls = formName.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAllControlsByType(ctrl, type))  // returns all controls of that control type
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        public Label GetLabel(Control formName, string LabelName = "label1")
        {
            Label btn = (Label)GetControl(formName, LabelName);
            return btn;
        }
        public IEnumerable<Control> GetLabels(Control formName)  // returns list of Label controls on a WinForm
        {
            Type type = typeof(Label);

            var controls = formName.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAllControlsByType(ctrl, type))  // returns all controls of that control type
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        public TabControl GetTabControl(Control formName, string TabControlName = "tabControl1")
        {
            TabControl btn = (TabControl)GetControl(formName, TabControlName);
            return btn;
        }
        public IEnumerable<Control> GetTabControls(Control formName)  // returns list of TabControl controls on a WinForm
        {
            Type type = typeof(TabControl);

            var controls = formName.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAllControlsByType(ctrl, type))  // returns all controls of that control type
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        public PictureBox GetPictureBox(Control formName, string PictureBoxName = "pictureBox1")
        {
            PictureBox btn = (PictureBox)GetControl(formName, PictureBoxName);
            return btn;
        }
        public IEnumerable<Control> GetPictureBoxes(Control formName)  // returns list of PictureBox controls on a WinForm
        {
            Type type = typeof(PictureBox);

            var controls = formName.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAllControlsByType(ctrl, type))  // returns all controls of that control type
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        public ToolStrip GetToolStrip(Control formName, string ToolStripName = "toolStrip1")
        {
            ToolStrip btn = (ToolStrip)GetControl(formName, ToolStripName);
            return btn;
        }
        public IEnumerable<Control> GetToolStrips(Control formName)  // returns list of ToolStrip controls on a WinForm
        {
            Type type = typeof(ToolStrip);

            var controls = formName.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAllControlsByType(ctrl, type))  // returns all controls of that control type
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        //public ToolStripMenuItem GetToolStripMenuItem(Control formName, string ToolStripMenuItemName = "toolStripMenuItem1")
        //{
        //    ToolStripMenuItem btn = (ToolStripMenuItem)GetControl(formName, ToolStripMenuItemName);
        //    return btn;
        //}
        public IEnumerable<Control> GetToolStripMenuItems(Control formName)  // returns list of ToolStripMenuItem controls on a WinForm
        {
            Type type = typeof(ToolStripMenuItem);

            var controls = formName.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAllControlsByType(ctrl, type))  // returns all controls of that control type
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        public MenuStrip GetMenuStrip(Control formName, string MenuStripName = "menuStrip1")
        {
            MenuStrip btn = (MenuStrip)GetControl(formName, MenuStripName);
            return btn;
        }
        public IEnumerable<Control> GetMenuStrips(Control formName)  // returns list of MenuStrip controls on a WinForm
        {
            Type type = typeof(MenuStrip);

            var controls = formName.Controls.Cast<Control>();

            return controls.SelectMany(ctrl => GetAllControlsByType(ctrl, type))  // returns all controls of that control type
                                      .Concat(controls)
                                      .Where(c => c.GetType() == type);
        }

        public Control GetControl(Control formName, string ControlName)
        {
            Control tbx = formName.Controls.Find(ControlName, true).FirstOrDefault() as Control;
            return tbx;
        }

        public DataGridView Return_FirstGridView(Control formName)  // Returns the First DataGridView Control Found on Form
        {
            var c = GetAllControlsByType(formName, typeof(DataGridView));
            MessageBox.Show("Total DataGridView Controls: " + c.Count());

            DataGridView returnGrid;

            foreach (var b in c)
            {
                //ahk.MsgBox(b.Name.ToString());
                returnGrid = (DataGridView)b;
                return returnGrid;
            }

            return null;
        }

        #endregion

        #region === Stretch / Fit Control Sizes ====


        // stretch the width of a control to fill out a space up towards another control to its right
        public void Control_Stretch_Width_Right(Control ToStretch, Control RightSide, int OffSetDistance = 10)
        {
            int leftToStretch = ToStretch.Bounds.Left;
            int originalWidth = ToStretch.Bounds.Width;
            int left = RightSide.Bounds.Left;

            int NewWidth = left - leftToStretch - OffSetDistance;
            ToStretch.Size = new Size(NewWidth, ToStretch.Bounds.Height);
        }

        // stretch out a toolstrip combobox to fill up the top space up until the toolstrip button to its right
        public void ToolStripMenuItem_Stretch_Width_Right(ToolStripComboBox ToStretch, ToolStripButton RightSide, int OffSetDistance = 10)
        {
            int leftToStretch = ToStretch.Bounds.Left;
            int originalWidth = ToStretch.Bounds.Width;
            int left = RightSide.Bounds.Left;

            int NewWidth = left - leftToStretch - OffSetDistance;
            ToStretch.Size = new Size(NewWidth, ToStretch.Bounds.Height);
        }



        #endregion

        #region === Control Values ===

        // === TextBox ===
        public bool TextBox_Save(TextBox txt)  // save user input from textbox to ini file
        {
            bool Saved = ahk.IniWrite(txt.Text, ahk.AppDir() + "\\AppSettings.ini", txt.Name, "Last_Value");
            return Saved;
        }
        public void TextBox_Load_Last(TextBox txt)  // loads last selected tab # when starting app (using ini setting)
        {
            // check to see if the "load on startup" is entered in the ini, if not found sets value to true 
            string LastTextOption = ahk.IniRead(ahk.AppDir() + "\\AppSettings.ini", txt.Name, "Load_On_Startup");  // check user setting to see if this option is enabled
            if (LastTextOption == "") { ahk.IniWrite("true", ahk.AppDir() + "\\AppSettings.ini", txt.Name, "Load_On_Startup"); }

            bool LoadLastText = ahk.ToBool(ahk.IniRead(ahk.AppDir() + "\\AppSettings.ini", txt.Name, "Load_On_Startup"));  // check user setting to see if this option is enabled

            if (LoadLastText)  // if option enabled, read the last index position and select that tab
            {
                string LastText = ahk.IniRead(ahk.AppDir() + "\\AppSettings.ini", txt.Name, "Last_Value");
                txt.Text = LastText;
            }
        }

        // === CheckBox ===
        public bool CheckBox_Save_Value(CheckBox cb)  // save checkbox value on change
        {
            bool Saved = ahk.IniWrite(cb.Checked.ToString(), ahk.AppDir() + "\\AppSettings.ini", cb.Name, "Checked");
            return Saved;
        }
        public void CheckBox_Load_Last(CheckBox cb)  // loads last selected tab # when starting app (using ini setting)
        {
            // check to see if the "load on startup" is entered in the ini, if not found sets value to true 
            string LastCheckBoxOption = ahk.IniRead(ahk.AppDir() + "\\AppSettings.ini", cb.Name, "Load_On_Startup");  // check user setting to see if this option is enabled
            if (LastCheckBoxOption == "") { ahk.IniWrite("true", ahk.AppDir() + "\\AppSettings.ini", cb.Name, "Load_On_Startup"); }

            bool LoadLastValue = ahk.ToBool(ahk.IniRead(ahk.AppDir() + "\\AppSettings.ini", cb.Name, "Load_On_Startup"));  // check user setting to see if this option is enabled

            if (LoadLastValue)  // if option enabled, read the last index position and select that tab
            {
                bool LastChecked = ahk.ToBool(ahk.IniRead(ahk.AppDir() + "\\AppSettings.ini", cb.Name, "Checked"));
                cb.Checked = LastChecked;
            }
        }

        // === RadioButton ===
        public bool RadioButton_Save_Value(RadioButton cb)  // save checkbox value on change
        {
            bool Saved = ahk.IniWrite(cb.Checked.ToString(), ahk.AppDir() + "\\AppSettings.ini", cb.Name, "Checked");
            return Saved;
        }
        public void RadioButton_Load_Last(RadioButton cb)  // loads last selected tab # when starting app (using ini setting)
        {
            // check to see if the "load on startup" is entered in the ini, if not found sets value to true 
            string LastCheckBoxOption = ahk.IniRead(ahk.AppDir() + "\\AppSettings.ini", cb.Name, "Load_On_Startup");  // check user setting to see if this option is enabled
            if (LastCheckBoxOption == "") { ahk.IniWrite("true", ahk.AppDir() + "\\AppSettings.ini", cb.Name, "Load_On_Startup"); }

            bool LoadLastValue = ahk.ToBool(ahk.IniRead(ahk.AppDir() + "\\AppSettings.ini", cb.Name, "Load_On_Startup"));  // check user setting to see if this option is enabled

            if (LoadLastValue)  // if option enabled, read the last index position and select that tab
            {
                bool LastChecked = ahk.ToBool(ahk.IniRead(ahk.AppDir() + "\\AppSettings.ini", cb.Name, "Checked"));
                cb.Checked = LastChecked;
            }
        }

        // === ComboBox ===
        public bool ComboBox_Save(ComboBox cb)  // save user input from textbox to ini file
        {
            //bool Saved = ahk.IniWrite(cb.SelectedIndex.ToString(), ahk.AppDir() + "\\AppSettings.ini", cb.Name, "Last_Value");
            bool Saved = ahk.IniWrite(cb.SelectedValue.ToString(), ahk.AppDir() + "\\AppSettings.ini", cb.Name, "Last_Value");
            return Saved;
        }
        public void ComboBox_Load_Last(ComboBox cb)  // loads last selected tab # when starting app (using ini setting)
        {
            // check to see if the "load on startup" is entered in the ini, if not found sets value to true 
            string LastTextOption = ahk.IniRead(ahk.AppDir() + "\\AppSettings.ini", cb.Name, "Load_On_Startup");  // check user setting to see if this option is enabled
            if (LastTextOption == "") { ahk.IniWrite("true", ahk.AppDir() + "\\AppSettings.ini", cb.Name, "Load_On_Startup"); }

            bool LoadLastText = ahk.ToBool(ahk.IniRead(ahk.AppDir() + "\\AppSettings.ini", cb.Name, "Load_On_Startup"));  // check user setting to see if this option is enabled

            if (LoadLastText)  // if option enabled, read the last index position and select that tab
            {
                string LastText = ahk.IniRead(ahk.AppDir() + "\\AppSettings.ini", cb.Name, "Last_Value");
                //cb.SelectedIndex = ahk.ToInt(LastText); // select drop down value
                cb.SelectedIndex = cb.FindString(LastText); // select drop down value
            }
        }

        // === ToolStripMenu ===
        public bool MenuItem_Checked_Save(ToolStripMenuItem cb)  // saves the checked state of a menu item
        {
            bool Saved = ahk.IniWrite(cb.Checked.ToString(), ahk.AppDir() + "\\AppSettings.ini", cb.Name, "Last_Value");
            return Saved;
        }
        public void MenuItem_Checked_Last(ToolStripMenuItem cb)  // checks the menu item if previously checked
        {
            // check to see if the "load on startup" is entered in the ini, if not found sets value to true 
            string LastTextOption = ahk.IniRead(ahk.AppDir() + "\\AppSettings.ini", cb.Name, "Load_On_Startup");  // check user setting to see if this option is enabled
            if (LastTextOption == "") { ahk.IniWrite("true", ahk.AppDir() + "\\AppSettings.ini", cb.Name, "Load_On_Startup"); }

            bool LoadLastText = ahk.ToBool(ahk.IniRead(ahk.AppDir() + "\\AppSettings.ini", cb.Name, "Load_On_Startup"));  // check user setting to see if this option is enabled

            if (LoadLastText)  // if option enabled, read the last index position and select that tab
            {
                string LastText = ahk.IniRead(ahk.AppDir() + "\\AppSettings.ini", cb.Name, "Last_Value");
                cb.Checked = ahk.ToBool(LastText); // check menu item if previously checked
            }
        }


        #endregion

        #region === StopWatch ===

        static System.Timers.Timer _timer; // using System.Timers
        Stopwatch stopwatch = new Stopwatch();
        int stopWatchTime = 250;  // ms to wait in between stopwatch gui updates
        Control stopwatchDisp;  // define control to update with timer times ( must populate from winform )

        public void StopWatchThread(bool Start = true)
        {
            if (Start)
            {
                stopwatch.Restart();
                stopwatch.Start();

                // start timer to run every 3 seconds to log active window title
                _timer = new System.Timers.Timer(250); // Set up the timer for 3 seconds
                _timer.Elapsed += new ElapsedEventHandler(StopWatchDisplay);
                _timer.Enabled = true; // Enable it
            }

            if (!Start)
            {
                stopwatch.Stop();
                _timer.Enabled = false; // disable timer to update gui
            }
        }

        public void StopWatchDisplay(object sender, ElapsedEventArgs e)
        {
            if (stopwatchDisp.InvokeRequired) { stopwatchDisp.BeginInvoke((MethodInvoker)delegate () { stopwatchDisp.Text = stopwatch.ElapsedMilliseconds.ToString(); }); }
            else { stopwatchDisp.Text = stopwatch.ElapsedMilliseconds.ToString(); }
        }


        #endregion

        #region === ListView ==
        //### Drag-Drop Items in ListView ###

        private void Enable_ListView_DragDrop(ListView LV)
        {
            LV.AllowDrop = true;
            LV.DragDrop += new DragEventHandler(ListView_DragDrop);
            LV.DragEnter += new DragEventHandler(ListView_DragEnter);
            LV.DragOver += new DragEventHandler(ListView_DragOver);
            LV.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(ListView_ItemDrag);
            //LV.SelectedIndexChanged += new System.EventHandler(ListView_SelectedIndexChanged);
        }

        private void listView_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListView LV = (ListView)sender;
            string SelectedText = "";
            string SelectedTag = "";

            if (LV.SelectedItems.Count > 0)
            {
                //SelectedTag = LV.SelectedItems[0].Tag.ToString();  // return icon's tag / filepath
                //SelectedText = LV.SelectedItems[0].Text.ToString(); // return text from icon
                ahk.MsgBox(SelectedText);
            }

        }

        private void ListView_DragEnter(object sender, DragEventArgs e)
        {
            ListView LV = (ListView)sender;
            //e.Effect = DragDropEffects.Copy;
            //e.Effect = e.AllowedEffect;
            if (privateDrag) e.Effect = e.AllowedEffect;
        }

        private void ListView_DragDrop(object sender, DragEventArgs e)
        {
            ListView LV = (ListView)sender;
            string Moved = e.Data.ToString();
            LV.Items.Add(e.Data.ToString());
        }

        private void ListView_DragOver(object sender, DragEventArgs e)
        {
            ListView LV = (ListView)sender;
            var pos = LV.PointToClient(new Point(e.X, e.Y));
            var hit = LV.HitTest(pos);
            if (hit.Item != null && hit.Item.Tag != null)
            {
                var dragItem = (ListViewItem)e.Data.GetData(typeof(ListViewItem));
                //ahk.MsgBox((string)hit.Item.Tag); 
                //copy(dragItem, (string)hit.Item.Tag);
            }
        }

        bool privateDrag; // private var used for ListView drag/drop

        private void ListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            privateDrag = true;
            ListView LV = (ListView)sender;
            LV.DoDragDrop(e.Item, DragDropEffects.Copy);
            privateDrag = false;
        }

        public void ListView_Clear(ListView LV)
        {
            // Clear listbox
            LV.Clear();
        }

        #endregion

        #region === StatusBar ===

        public static StatusBarPanel statusPanel { get; set; }
        public static StatusBarPanel datetimePanel { get; set; }
        public static RadStatusStrip radStatusStrip { get; set; }


        //====== Status Bar =======
        public void Create_StatusBar(Form formName)
        {
            // Create New StatusBar
            StatusBar mainStatusBar = new StatusBar();
            StatusBarPanel statusPanel1 = new StatusBarPanel();
            StatusBarPanel datetimePanel1 = new StatusBarPanel();

            // Set first panel properties and add to StatusBar
            statusPanel1.BorderStyle = StatusBarPanelBorderStyle.Sunken;
            statusPanel1.Text = formName.Text;
            statusPanel1.Tag = "StatusBar1";
            statusPanel1.ToolTipText = formName.Text;
            statusPanel1.AutoSize = StatusBarPanelAutoSize.Spring;
            mainStatusBar.Panels.Add(statusPanel1);

            // Set second panel properties and add to StatusBar
            datetimePanel1.BorderStyle = StatusBarPanelBorderStyle.Raised;
            datetimePanel1.ToolTipText = "DateTime: " + System.DateTime.Today.ToString();
            datetimePanel1.Text = System.DateTime.Today.ToLongDateString();
            datetimePanel1.Tag = "StatusBar2";
            datetimePanel1.AutoSize = StatusBarPanelAutoSize.Contents;
            mainStatusBar.Panels.Add(datetimePanel1);

            mainStatusBar.ShowPanels = true;
            // Add StatusBar to Form controls
            formName.Controls.Add(mainStatusBar);

            // store values in public var to reference when updating
            statusPanel = statusPanel1;
            datetimePanel = datetimePanel1;
        }

        Form statusBarForm; // form containing statusbar - holds definition to update elsewhere (stopwatch)
        public void StatusBar(Form formName, string UpdateText, int section = 1, string Timer = "")   //update statusbar text
        {
            // updates status bar text (from any thread)

            if (Timer.ToUpper() == "START") { statusBarTimerThread("Start"); }
            if (Timer.ToUpper() == "STOP") { statusBarTimerThread("Stop"); }

            statusBarForm = formName;

            // WinForms StatusBarPanel 
            if (statusPanel != null)
            {
                if (section == 1)
                {
                    try
                    {
                        MethodInvoker inv = delegate { if (statusPanel != null) { statusPanel.Text = UpdateText; } };
                        formName.Invoke(inv);
                    }
                    catch
                    {
                        statusPanel.Text = UpdateText;
                    }
                }

                if (section == 2)
                {
                    try
                    {
                        MethodInvoker inv = delegate { datetimePanel.Text = UpdateText; };
                        formName.Invoke(inv);
                    }
                    catch
                    {
                        datetimePanel.Text = UpdateText;
                    }
                }
            }

            // Telerik RadStatusStrip
            if (radStatusStrip != null)
            {
                if (radStatusStrip.Items.Count > 0)
                {
                    (radStatusStrip.Items[0] as RadLabelElement).Text = UpdateText;
                }
                if (radStatusStrip.Items.Count > 1)
                {
                    (radStatusStrip.Items[1] as RadLabelElement).Text = UpdateText;
                }
            }

        }

        public void StatusBar_Icon(string ICOPath, int section = 1)
        {
            _Images img = new _Images();

            if (section == 1)
            {
                // show icon in bottom left statusbar (works)
                try { statusPanel.Icon = img.ImagePath_to_Icon(ICOPath, 32); }
                catch (Exception ex) { ahk.MsgBox(ex.Message); }
            }

            if (section == 2)
            {
                // show icon in bottom right statusbar (works)
                try { datetimePanel.Icon = img.ImagePath_to_Icon(ICOPath, 32); }
                catch (Exception ex) { MessageBox.Show(ex.Message); }
            }


        }

        public void statusBarTimer_()
        {
            Thread dirToDbThread = new Thread(() => statusBarTimerThread("Start"));
            dirToDbThread.Start();
        }

        public void statusBarUpdate(object sender, ElapsedEventArgs e)
        {
            StatusBar(statusBarForm, sbStopwatch.Elapsed.Seconds.ToString(), 2);
        }

        Stopwatch sbStopwatch = new Stopwatch();

        public void statusBarTimerThread(string StopStart = "Start")
        {
            if (StopStart.ToUpper() == "START")  // timer not created yet
            {
                _sBtimer = new System.Timers.Timer(1000); // Set up the timer for 3 seconds
                _sBtimer.Elapsed += new ElapsedEventHandler(statusBarUpdate);
                _sBtimer.Enabled = true; // Enable it
                sbStopwatch.Start();
                return;
            }

            if (StopStart.ToUpper() == "STOP")
            {
                _sBtimer.Enabled = false;
                sbStopwatch.Stop();

                StatusBar(statusBarForm, sbStopwatch.Elapsed.Seconds.ToString() + " (DONE)", 2);
                return;
            }

            if (StopStart.ToUpper() == "RESUME")
            {
                _sBtimer.Enabled = true;
                sbStopwatch.Start();
                return;
            }

            if (StopStart.ToUpper() == "RESET")
            {
                _sBtimer.Enabled = true;
                sbStopwatch.Reset();
                return;
            }
        }

        static System.Timers.Timer _sBtimer; // using System.Timers


        #endregion

        #region === Labels ===

        // update Label Text (from any thread)
        public void LabelText(Label label, string Text)
        {
            if (label.InvokeRequired)  // if currently on a different thread, invoke label first
            {
                label.BeginInvoke((MethodInvoker)delegate () { label.Text = Text; });
            }
            else
            {
                label.Text = Text;
            }
        }

        #endregion

        #region === TextBox ===

        // update text from any thread
        public void UpdateText(Control textBox, string text)
        {
            string type = textBox.GetType().ToString();

            if (textBox.GetType().ToString() == "Windows.Forms.TextBox")
            {
                if (textBox.InvokeRequired)
                {
                    textBox.FindForm().Invoke(new Action<Control, string>(UpdateText), new object[] { textBox, text });
                    return;
                }
                textBox.Text += text;
                return;
            }


            if (textBox.GetType().ToString() == "Windows.Forms.Label")
            {

                //MethodInvoker inv = delegate
                //{
                //    this.lblCounter.Text = this.index.ToString();
                //}

                //this.Invoke(inv);

                if (textBox.InvokeRequired)
                {
                    textBox.FindForm().Invoke(new Action<Control, string>(UpdateText), new object[] { textBox, text });
                    return;
                }
                textBox.Text += text;
                return;
            }




            if (textBox.InvokeRequired)
            {
                textBox.BeginInvoke((MethodInvoker)delegate () { textBox.Text = text; });
            }
            else
            {
                textBox.Text = text;
            }
        }

        // return text from control on any thread
        public string ReturnText(Control textBox)
        {
            string returnval = "";

            if (textBox.InvokeRequired)
            {
                textBox.BeginInvoke((MethodInvoker)delegate () { returnval = textBox.Text; });
            }
            else
            {
                returnval = textBox.Text;
            }

            return returnval;
        }

        #endregion

        #region === ProgressBar ===

        // setup progress bar (total progress count) / resets existing progress 
        public void SetupProgressBar(ProgressBar pr, int ProgressMax = 100) // progress bar from any thread
        {
            if (pr.InvokeRequired)
            {
                pr.BeginInvoke((MethodInvoker)delegate () { pr.Value = 0; pr.Maximum = ProgressMax; });
            }
            else
            {
                pr.Value = 0; pr.Maximum = ProgressMax;
            }
        }

        public void UpdateProgressBar(ProgressBar pr, int IncreaseBy = 1) // progress bar from any thread
        {
            if (pr.InvokeRequired)
            {
                pr.BeginInvoke((MethodInvoker)delegate () { pr.Increment(IncreaseBy); });
            }
            else
            {
                pr.Increment(IncreaseBy);
            }
        }


        #endregion

    }
}

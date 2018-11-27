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
using System.Drawing;
using System.Drawing.Imaging;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace sharpAHK_Dev
{
    /// <summary>
    /// Image Library
    /// </summary>
    public class _Images
    {
        private _AHK ahk = new _AHK();
        //public _SQL sql { get; set; }
        //public _SQLite sqlite { get; set; }


        private bool GlobalDebug = false;


        /// <summary>
        /// Stores Location of Icon Files Based on Icon Name Lookup Return for Menu/Tree Icons
        /// </summary>
        /// <param name="IconName"></param>
        /// <param name="W"></param>
        /// <param name="H"></param>
        /// <returns></returns>
        public Image ReturnIcon(string IconName, int W, int H)
        {
            string path = "";

            if (IconName == "Database_Server") { path = @"C:\_Code\LucidProjects\ADBindex\LiveIntheMedia\LiveInTheMedia\Icons\Database_Server.png"; }
            if (IconName == "Database_Table") { path = @"C:\_Code\LucidProjects\ADBindex\LiveIntheMedia\LiveInTheMedia\Icons\Database_Table.png"; }
            if (IconName == "Database_Column") { path = @"C:\_Code\LucidProjects\ADBindex\LiveIntheMedia\LiveInTheMedia\Icons\Database_Column.png"; }

            if (File.Exists(path)) { return resizeImage(W, H, path); }
            return null;
        }


        #region === ImageLists ===

        /// <summary>returns bool if file path is an image (jpg/png/gif)</summary>
        /// <param name="FilePath"> </param>
        public bool IsImage(string FilePath)
        {
            string extension = Path.GetExtension(FilePath);
            extension = extension.ToUpper();

            // make sure file format is valid image, otherwise go to next item in list
            bool FormatAccepted = false;
            if (extension == ".PNG") { FormatAccepted = true; }
            if (extension == ".GIF") { FormatAccepted = true; }
            if (extension == ".ICO") { FormatAccepted = true; }
            if (extension == ".JPG") { FormatAccepted = true; }
            if (extension == ".JPEG") { FormatAccepted = true; }

            return FormatAccepted;
        }

        // TO ImageList

        /// <summary>Populate ImageList from Directory (Valid Image Formats Only - No Recurse!) FullPathKeyName toggles between FileNameNoExt as Key or Full File Path to Image</summary>
        /// <param name="ICODir"> </param>
        /// <param name="FullPathKeyName"> </param>
        /// <param name="SearchPattern"> </param>
        /// <param name="ImageSize"> </param>
        public ImageList ImageList(string ICODir, bool FullPathKeyName = true, string SearchPattern = "*.*", int ImageSize = 16)
        {
            //======================================
            // Populate ImageList From Directory
            //======================================

            //string ICODir = ahk.DevRoot() + "\\ICO_Lib\\Folders";

            // Load the images in an ImageList.
            ImageList myImageList = new ImageList();

            myImageList.Tag = ICODir;


            // Set the ImageSize property to a larger size 
            // (the default is 16 x 16).
            myImageList.ImageSize = new Size(ImageSize, ImageSize);


            _Lists lst = new _Lists();

            // loop through multiple folders, return files meeting search criteria, sort files by FileName regardless of directory path, return sorted list of full file paths as list
            List<string> filelistSorted = lst.FileList_SortedAlpha_ByFileName(ICODir, SearchPattern, false);


            foreach (string file in filelistSorted)  // loop through list of files and write file details to sqlite db
            //foreach (string file in files)  // loop through list of files and write file details to sqlite db
            {
                System.IO.FileInfo fileinfo = new System.IO.FileInfo(file); //retrieve info about each file

                string FileName = fileinfo.Name.ToString();
                string FileExt = fileinfo.Extension.ToString();
                string FileNameNoExt = ahk.StringReplace(FileName, FileExt);

                bool AddToList = false;

                if (FileExt.ToUpper() == ".JPG") { AddToList = true; }
                if (FileExt.ToUpper() == ".JPEG") { AddToList = true; }
                if (FileExt.ToUpper() == ".GIF") { AddToList = true; }
                if (FileExt.ToUpper() == ".PNG") { AddToList = true; }
                if (FileExt.ToUpper() == ".ICO") { AddToList = true; }


                string KeyName = FileNameNoExt;

                if (FullPathKeyName) { KeyName = file; }

                // add image to list if valid image format
                if (AddToList) { myImageList.Images.Add(KeyName, Image.FromFile(file)); }
            }

            if (GlobalDebug)
            {
                if (myImageList.Images.Count == 0)
                { ahk.MsgBox("Zero Images Imported to ImageList:" + Environment.NewLine + ICODir); }

                //ahk.MsgBox(myImageList.Images.Count.ToString() + " Images Added To ImageList");
            }

            return myImageList;
        }

        /// <summary>Populate ImageList from List of Images Provided - FullPathKeyName toggles between FileNameNoExt as Key or Full File Path to Image</summary>
        /// <param name="List<string> FileList"> </param>
        /// <param name="FullPathKeyName"> </param>
        /// <param name="SearchPattern"> </param>
        public ImageList ImageList_From_FileList(List<string> FileList, bool FullPathKeyName = true, string SearchPattern = "*.*")
        {
            ImageList il = new ImageList();
            foreach (string img in FileList)
            {
                bool IsImg = IsImage(img); // make sure an image file is being loaded
                if (!IsImg) { continue; }   // otherwise skip

                System.Net.WebRequest request = System.Net.WebRequest.Create(img);
                System.Net.WebResponse resp = request.GetResponse();
                System.IO.Stream respStream = resp.GetResponseStream();
                Bitmap bmp = new Bitmap(respStream);
                respStream.Dispose();

                string KeyName = img;  // imagelist key = file path
                if (!FullPathKeyName) { KeyName = ahk.FileNameNoExt(img); }

                il.Images.Add(KeyName, bmp);  // add image to list with key name

                GC.Collect();  // free up space??
                //il.Images.Add(bmp);
            }
            il.ImageSize = new Size(32, 32);

            return il;
        }


        // FROM ImageList

        /// <summary>return list of key names (also the filename without ext) in ImageList</summary>
        /// <param name="ImageList IL"> </param>
        /// <param name="FileNameOnly"> </param>
        /// <param name="FileExt"> </param>
        public List<string> ImageList_FileNames(ImageList IL, bool FileNameOnly = false, bool FileExt = false)
        {
            List<string> ImageListKeys = new List<string>();

            foreach (string img in IL.Images.Keys)
            {
                if (FileNameOnly)
                {
                    if (!FileExt)
                    {
                        string fileName = ahk.FileNameNoExt(img);  // file name with extension
                        ImageListKeys.Add(fileName);
                    }
                    if (FileExt)
                    {
                        string fileName = ahk.FileName(img);  // file name with extension
                        ImageListKeys.Add(fileName);
                    }

                }
                else
                {
                    ImageListKeys.Add(img);  // full file path 
                }


            }

            return ImageListKeys;
        }

        /// <summary>returns # of images in ImageList</summary>
        /// <param name="ImageList IL"> </param>
        public int ImageList_Count(ImageList IL)
        {
            return IL.Images.Count;
        }

        /// <summary>search imagelist for key (file name) - returns image</summary>
        /// <param name="ImageList IL"> </param>
        /// <param name="KeyName"> </param>
        public Image From_ImageList(ImageList IL, string KeyName = "png_arrow-down-icon.png")
        {
            List<string> imageList = ImageList_FileNames(IL);

            foreach (string image in imageList)
            {
                string img = ahk.FileName(image);
                if (KeyName == img)
                {
                    return GetCopyImage(image);
                }

                img = ahk.FileNameNoExt(image);
                if (KeyName == img)
                {
                    return GetCopyImage(image);
                }

                if (image.Contains(KeyName))
                {
                    return GetCopyImage(image);
                }
            }

            return null;
        }

        /// <summary>return image from ImageList by KeyName and Populate Picturebox</summary>
        /// <param name="PictureBox box"> </param>
        /// <param name=" ImageList IL"> </param>
        /// <param name="KeyName"> </param>
        public void Display_Image_From_ImageList(PictureBox box, ImageList IL, string KeyName = "png_arrow-down-icon.png")
        {
            box.Image = From_ImageList(IL, KeyName);  // return image from ImageList by KeyName and Populate Picturebox
        }


        // FIND ICONS ON PC

        public void FindSave_Icons(string SearchDir = @"C:\", string SaveDir = "ICO_Collect", bool ExtractEXEIcons = true)
        {
            if (SaveDir == "ICO_Collect") { SaveDir = ahk.AppDir() + "\\ICO_Collect"; }
            SaveDir.FileCreateDir();

            _Lists lst = new _Lists();

            List<string> IconFiles = new List<string>();
            List<string> EXEFiles = new List<string>();

            // Create List of Icon Files and EXE Files To Extract Icons From
            List<string> Files = lst.FileList(SearchDir, "*.*", true, false, true);
            foreach (string file in Files)
            {
                if (file.FileExt().ToUpper() == "ICO" || file.FileExt().ToUpper() == "ICON") { IconFiles.Add(file); }
                if (file.FileExt().ToUpper() == "EXE") { EXEFiles.Add(file); }
            }

            if (ExtractEXEIcons)
            {
                string exeSaveDir = SaveDir + "\\EXE_Icons";
                exeSaveDir.FileCreateDir();

                foreach (string exefile in EXEFiles)
                {
                    // Extract Icon from EXE and Save
                    Icon ico = To_Icon(exefile);
                    bool saved = SaveICO(ico, exeSaveDir + "\\" + exefile.FileNameNoExt() + ".ico");
                }
            }
        }


        #endregion


        #region === Images in Controls ===

        // ### TREEVIEW ###################

        /// <summary>populate TreeView with ImageList contents</summary>
        /// <param name="TV"> </param>
        /// <param name="IL"> </param>
        /// <param name="clearTV"> </param>
        /// <param name="eXpand"> </param>
        /// <param name="ParentText"> </param>
        public void TreeView_ImageList_Display(TreeView TV, ImageList IL, bool clearTV = true, bool eXpand = true, string ParentText = "Image List Contents")
        {
            //ImageList imgList = il.ImageList(ImageDirPath, true, "*.png");  // populate imagelist
            //ahk.MsgBox("Image Count = " + imgList.Images.Count);

            if (clearTV) { TV.Nodes.Clear(); }

            // Assign the ImageList to the TreeView. (from any thread)
            if (TV.InvokeRequired) { TV.BeginInvoke((MethodInvoker)delegate () { TV.ImageList = IL; }); }
            else { TV.ImageList = IL; }

            //=== Populate TreeView Example
            TreeNode parent = new TreeNode();  // level 1
            parent.Text = ParentText;

            List<string> ImageListKeys = new List<string>();

            foreach (string img in IL.Images.Keys)
            {
                TreeNode section = new TreeNode();  // level 2

                section.Tag = img;  // set tag on node with filepath to imagemlist image
                section.Text = ahk.FileNameNoExt(img);  // just show the file name on the treeView

                // update treeview node (from any thread)
                if (TV.InvokeRequired) { TV.BeginInvoke((MethodInvoker)delegate () { section.ImageIndex = IL.Images.IndexOfKey(img); section.SelectedImageIndex = IL.Images.IndexOfKey(img); }); }
                else { section.ImageIndex = IL.Images.IndexOfKey(img); section.SelectedImageIndex = IL.Images.IndexOfKey(img); }


                parent.Nodes.Add(section);
            }

            // update treeview node (from any thread) // populate tree
            if (TV.InvokeRequired) { TV.BeginInvoke((MethodInvoker)delegate () { TV.Nodes.Add(parent); }); }
            else { TV.Nodes.Add(parent); }

            if (TV.InvokeRequired) { TV.BeginInvoke((MethodInvoker)delegate () { TV.ExpandAll(); }); }
            else { TV.ExpandAll(); }
        }


        // ### BUTTONS ###################

        /// <summary>Add image to button control</summary>
        /// <param name="btn"> </param>
        /// <param name="IL"> </param>
        /// <param name="KeyName"> </param>
        public void Button_Icon(Button btn, ImageList IL, string KeyName = "arrow1.png")
        {
            Image iMage = From_ImageList(IL, KeyName);
            if (iMage == null) { ahk.MsgBox(KeyName + " Not Found in ImageList"); }
            //btn.Image = iMage;
            //btn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btn.BackgroundImage = iMage;

            var pic = new Bitmap(btn.BackgroundImage, new Size(btn.Width, btn.Height));
            btn.BackgroundImage = pic;

        }

        /// <summary>Add image to button control (Image Only)</summary>
        /// <param name="button"> </param>
        /// <param name="Image"> </param>
        public void Button_Image_Only(Button button, object Image)
        {
            Image image = To_Image(Image); // convert input to Image format

            //=== Button Image: just button with icon in middle (removes text) ===
            button.BackgroundImageLayout = ImageLayout.Zoom; // adjust zoom to make button icon fit (works)
            button.BackgroundImage = image;
            button.ImageAlign = ContentAlignment.MiddleCenter;
            //button9.TextAlign = ContentAlignment.MiddleRight;
            button.Text = "";
        }

        /// <summary>Add image to button control</summary>
        /// <param name="button"> </param>
        /// <param name="Image"> </param>
        /// <param name="ImageLeft"> </param>
        public void Button_Image(Button button, object Image, bool ImageLeft = true)
        {
            Image image = To_Image(Image); // convert input to Image format

            //=== Button Image: text far left - icon middle ====  (works)
            button.TextImageRelation = TextImageRelation.TextBeforeImage;
            button.BackgroundImageLayout = ImageLayout.Zoom; // adjust zoom to make button icon fit (works)
            button.BackgroundImage = image;

            if (ImageLeft)  // image on left side of button
            {
                button.ImageAlign = ContentAlignment.MiddleRight;
                button.TextAlign = ContentAlignment.MiddleLeft;
            }
            if (!ImageLeft)  // image on right side of button
            {
                button.ImageAlign = ContentAlignment.MiddleLeft;
                button.TextAlign = ContentAlignment.MiddleRight;
            }

        }


        // #### PictureBox ################

        /// <summary>load image into picture box using string file path</summary>
        /// <param name="box"> </param>
        /// <param name="Image"> </param>
        /// <param name="ZoomImage"> </param>
        public Image Load_PictureBox(PictureBox box, object Image, bool ZoomImage = true)
        {
            if (ZoomImage) { box.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom; } // option to ensure image fits in picturebox 
            if (box.Image != null) { box.Image.Dispose(); } // clear previous image from memory
            Image image = To_Image(Image); // convert input to Image format
            box.Image = image; // display image in control
            return image;
        }


        // #### LISTVIEW ################

        /// <summary>create imagelist and populate ListView with icons in folder path</summary>
        /// <param name="lv"> </param>
        /// <param name="IconDir"> </param>
        /// <param name="ICOSize"> </param>
        public int ListView_Icons(ListView lv, string IconDir, int ICOSize = 32)
        {
            _Lists lst = new _Lists();

            lv.Clear();

            if (!Directory.Exists(IconDir)) { return 0; }  // confirm directory exists before attempting to load

            List<string> IconList = new List<string>();
            IconList = lst.FileList_Images(IconDir);  // list of images in directory

            ImageList il = new ImageList();
            foreach (string img in IconList)
            {
                System.Net.WebRequest request = System.Net.WebRequest.Create(img);
                System.Net.WebResponse resp = request.GetResponse();
                System.IO.Stream respStream = resp.GetResponseStream();
                Bitmap bmp = new Bitmap(respStream);
                respStream.Dispose();

                il.Images.Add(bmp);
                //bmp.Dispose();  // testing
            }
            //il.ImageSize = new Size(32, 32);
            il.ImageSize = new Size(ICOSize, ICOSize);
            int count = 0;
            lv.LargeImageList = il;

            foreach (string file in IconList)
            {
                ListViewItem listItem = new ListViewItem();
                listItem.Text = Path.GetFileNameWithoutExtension(file);
                listItem.Tag = file;
                listItem.ImageIndex = count++;
                lv.Items.Add(listItem);
            }

            return IconList.Count;
        }

        /// <summary>take imagelist and populate ListView with icons</summary>
        /// <param name="lv"> </param>
        /// <param name="IL"> </param>
        public void ImageList_ListView_Display(ListView lv, ImageList IL)
        {
            _Lists lst = new _Lists();

            lv.Clear();

            int count = 0;

            foreach (string img in IL.Images.Keys)
            {
                ListViewItem listItem = new ListViewItem();
                listItem.Text = Path.GetFileNameWithoutExtension(img);
                listItem.Tag = img;
                listItem.ImageIndex = count++;
                lv.Items.Add(listItem);
            }

            IL.ImageSize = new Size(64, 64);
            IL.ColorDepth = ColorDepth.Depth32Bit;
            lv.LargeImageList = IL;
        }


        // #### TABS ################

        /// <summary>
        /// Add image to Tab Page
        /// </summary>
        /// <param name="control"></param>
        /// <param name="tab"></param>
        /// <param name="ILL"></param>
        /// <param name="ImageKey"></param>
        /// <param name="Text"></param>
        /// <param name="ImgSize"></param>
        public void TabPage_Image(TabControl control, TabPage tab, ImageList ILL, string ImageKey, string Text = "", int ImgSize = 32)
        {
            // populate icon in TabPage TabControl - must be 64x64 (?) (kinda works)
            //ILL.ImageSize = new Size(64, 64);
            ILL.ImageSize = new Size(ImgSize, ImgSize);
            ILL.ColorDepth = ColorDepth.Depth32Bit;
            control.ImageList = ILL;
            if (Text != "") { tab.Text = Text; }

            string ImgDir = ILL.Tag.ToString();

            tab.ImageKey = ImgDir + "\\" + ImageKey;
        }


        // #### FORM ################

        /// <summary>update the form / application icon</summary>
        /// <param name="FormName"> </param>
        /// <param name="Image"> </param>
        public void Form_Icon(Control FormName, object Image)
        {
            Icon ico = To_Icon(Image);  // convert input format to ICO

            //// change application icon (works)
            Form form = FormName.FindForm();
            form.Icon = ico;
        }


        // #### GRIDVIEW ################

        /// <summary>create list of images that can display in DataGridView Cells</summary>
        /// <param name="ImageDir"> </param>
        /// <param name="SearchPattern"> </param>
        /// <param name="Recurse"> </param>
        public List<Image> Grid_Image_List(string ImageDir, string SearchPattern = "*.*", bool Recurse = true)
        {
            List<System.Drawing.Image> gridImageList = new List<System.Drawing.Image>();

            SearchOption recurseOption = SearchOption.AllDirectories;
            if (!Recurse) { recurseOption = SearchOption.TopDirectoryOnly; }

            string[] files = Directory.GetFiles(ImageDir, SearchPattern, recurseOption);

            foreach (string file in files)
            {
                gridImageList.Add(Image.FromFile(file));
            }


            return gridImageList;
        }

        // #### MENUSTRIP ################

        /// <summary>adds icon to menu item (toolstrip menu item or toolstrip split button)</summary>
        /// <param name="menuItem"> </param>
        /// <param name="Image"> </param>
        public void Menu_Image(object menuItem, object Image)
        {
            Image image = To_Image(Image); // convert input to Image format

            //toolStripSplitButton1

            if (menuItem is ToolStripMenuItem)
            {
                ToolStripMenuItem item = (ToolStripMenuItem)menuItem;
                item.Image = image;  // adds icon to menu item 
                return;
            }
            if (menuItem is ToolStripSplitButton)
            {
                ToolStripSplitButton item = (ToolStripSplitButton)menuItem;
                item.Image = image;  // adds icon to menu item    
                return;
            }
            if (menuItem is ToolStripButton)
            {
                ToolStripButton item = (ToolStripButton)menuItem;
                item.Image = image;  // adds icon to menu item    
                return;
            }

            ahk.MsgBox("Control Type Not Configured in Menu_Image Function");
        }



        #endregion


        #region === Convert Images ===

        // Save Icon to File
        public bool SaveICO(Icon IconToSave, string SavePath)
        {
            using (FileStream fs = new FileStream(SavePath, FileMode.Create))
                IconToSave.Save(fs);

            if (File.Exists(SavePath)) { return true; }
            else { return false; }
        }


        /// <summary>Convert Image Path (png, ico, exe) / Icon / ImageList (By Key) Item / or Returns Image if Provided</summary>
        /// <param name="Image"> </param>
        /// <param name="KeyName"> </param>
        public Image To_Image(object Image, int width = 0, int height = 0, string KeyName = "")
        {
            string VarType = Image.GetType().ToString();  //determine what kind of variable was passed into function

            if (VarType == "System.String")  // Image File Path
            {
                if (!File.Exists((string)Image)) { return null; }

                // check the file extension of the file passed in 
                string fileExt = ahk.FileExt((string)Image, true);

                if (fileExt.ToUpper() == ".EXE") // extract default image from exe 
                {
                    Icon a = Icon.ExtractAssociatedIcon((string)Image);
                    Image returnImage = To_Image(a);  // convert icon to image
                    return returnImage;  // return image
                }

                // otherwise read image file path and convert to image and return
                Image fileImage = GetCopyImage((string)Image);

                // resize image if user provided w/h
                if (width != 0 && height != 0)
                {
                    fileImage = resizeImage(width, height, (string)Image);
                }

                return fileImage;
            }
            if (VarType == "System.Drawing.Icon")  // Icon
            {
                Icon ico = (Icon)Image;
                return ico.ToBitmap();
            }
            if (VarType == "System.Drawing.Bitmap")  // Image (returns same image unchanged)
            {
                return (Image)Image;
            }
            if (VarType == "System.Windows.Forms.ImageList")  // ImageList
            {
                return From_ImageList((ImageList)Image, KeyName);  // return image from image list by key
            }

            return null;
        }

        /// <summary>Convert Image Path (png, ico, exe) / Icon / ImageList (By Key) Item / or Returns Image if Provided</summary>
        /// <param name="Image"> </param>
        /// <param name="Option"> </param>
        public Icon To_Icon(object Image, object Option = null)
        {
            string VarType = Image.GetType().ToString();  //determine what kind of variable was passed into function

            if (VarType == "System.String")  // Image File Path
            {
                // check the file extension of the file passed in 
                string fileExt = ahk.FileExt((string)Image, true);

                if (fileExt.ToUpper() == ".EXE") // extract default icon from exe 
                {
                    return Icon.ExtractAssociatedIcon((string)Image);
                }

                // set icon size - use optional parameter
                int ICOSize = 64;
                if (Option != null) { ICOSize = (int)Option; }

                // otherwise read image file path and convert to image and return
                Icon ico = ImagePath_to_Icon((string)Image, ICOSize);

                return ico;
            }
            if (VarType == "System.Drawing.Icon")  // Icon
            {
                return (Icon)Image;
            }
            if (VarType == "System.Drawing.Bitmap")  // Image 
            {
                // set icon size - use optional parameter
                int ICOSize = 64;
                if (Option != null) { ICOSize = (int)Option; }

                // Image to Icon
                Bitmap bitmap = new Bitmap((Image)Image);
                bitmap.SetResolution(ICOSize, ICOSize);
                Icon icon = System.Drawing.Icon.FromHandle(bitmap.GetHicon());
                return icon;
            }
            if (VarType == "System.Windows.Forms.ImageList")  // ImageList
            {
                // default icon size
                int ICOSize = 64;

                // return image from image list by key
                Image ILImage = From_ImageList((ImageList)Image, (string)Option);

                // convert image to Icon
                var thumb = (Bitmap)ILImage.GetThumbnailImage(ICOSize, ICOSize, null, IntPtr.Zero);
                thumb.MakeTransparent();
                return Icon.FromHandle(thumb.GetHicon());
            }

            return null;
        }


        /// <summary>copy image before loading, frees file to delete</summary>
        /// <param name="path"> </param>
        public Image GetCopyImage(string path)
        {
            if (File.Exists(path))
            {
                if (IsImage(path))
                {
                    try
                    {
                        using (Image im = Image.FromFile(path))
                        {
                            Bitmap bm = new Bitmap(im);
                            return bm;
                        }
                    }
                    catch { }

                }
            }

            //ahk.MsgBox(path + "\r\nNot Found - Unable to Load Image");

            return null;
        }

        /// <summary>convert Image to Icon</summary>
        /// <param name="ImagePath"> </param>
        /// <param name="Size"> </param>
        public Icon ImagePath_to_Icon(string ImagePath, int Size = 64)
        {
            if (!File.Exists(ImagePath)) { return null; }

            var bmp = Bitmap.FromFile(ImagePath);
            var thumb = (Bitmap)bmp.GetThumbnailImage(Size, Size, null, IntPtr.Zero);
            thumb.MakeTransparent();
            return Icon.FromHandle(thumb.GetHicon());
        }


        /// <summary>convert image to ICON format (2nd method)</summary>
        /// <param name="ImagePath"> </param>
        /// <param name="W"> </param>
        /// <param name="H"> </param>
        public Icon Create_Icon(string ImagePath, int W = 72, int H = 72)
        {
            //string ImagePath = @"C:\Users\jason\Google Drive\ICO_Lib\FlatIcon\126759-linear-color-web-interface-elements\planet-earth.png";
            Image image = Image.FromFile(ImagePath);
            Bitmap bitmap = new Bitmap(image);
            bitmap.SetResolution(W, H);
            Icon icon = System.Drawing.Icon.FromHandle(bitmap.GetHicon());
            return icon;
        }




        #endregion

        #region === Combine Images ===

        // untested image combine 
        public Image CombineImages(FileInfo[] files, string OutImagePath = "")
        {
            //source: https://www.codeproject.com/Articles/502249/Combineplusseveralplusimagesplustoplusformplusaplu

            //change the location to store the final image.
            string finalImage = @"C:\\MyImages\\FinalImage.jpg";
            List<int> imageHeights = new List<int>();
            int nIndex = 0;
            int width = 0;
            foreach (FileInfo file in files)
            {
                Image img = Image.FromFile(file.FullName);
                imageHeights.Add(img.Height);
                width += img.Width;
                img.Dispose();
            }
            imageHeights.Sort();
            int height = imageHeights[imageHeights.Count - 1];
            Bitmap img3 = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(img3);
            g.Clear(SystemColors.AppWorkspace);
            foreach (FileInfo file in files)
            {
                Image img = Image.FromFile(file.FullName);
                if (nIndex == 0)
                {
                    g.DrawImage(img, new Point(0, 0));
                    nIndex++;
                    width = img.Width;
                }
                else
                {
                    g.DrawImage(img, new Point(width, 0));
                    width += img.Width;
                }
                img.Dispose();
            }
            g.Dispose();

            if (OutImagePath != "")
            {
                img3.Save(OutImagePath, System.Drawing.Imaging.ImageFormat.Png);
            }

            //img3.Dispose();
            return img3;
        }



        #endregion


        #region === Image Compare ===
        // image compare

        public int ImageCompare(string Image1, string Image2)
        {
            List<bool> iHash1 = GetImage_Hash(new Bitmap(Image1));
            List<bool> iHash2 = GetImage_Hash(new Bitmap(Image2));

            //determine the number of equal pixel (x of 256)
            int equalElements = iHash1.Zip(iHash2, (i, j) => i == j).Count(eq => eq);

            //ahk.MsgBox(equalElements.ToString()); 

            return equalElements;
        }

        public List<bool> GetImage_Hash(Bitmap bmpSource)
        {
            // source http://stackoverflow.com/questions/35151067/algorithm-to-compare-two-images-in-c-sharp
            // credit fubo

            List<bool> lResult = new List<bool>();
            //create new image with 16x16 pixel
            Bitmap bmpMin = new Bitmap(bmpSource, new Size(16, 16));
            for (int j = 0; j < bmpMin.Height; j++)
            {
                for (int i = 0; i < bmpMin.Width; i++)
                {
                    //reduce colors to true / false                
                    lResult.Add(bmpMin.GetPixel(i, j).GetBrightness() < 0.5f);
                }
            }
            return lResult;
        }



        #endregion


        #region === Image Edit ===

        //http://imageprocessor.org/getting-started/  -- image editing lib on nuget

        // UNTESTED 

        /// <summary>crop image size</summary>
        /// <param name="img"> </param>
        /// <param name="cropArea"> </param>
        public Image cropImage(Bitmap img, Rectangle cropArea)  // 
        {
            Bitmap bmp = new Bitmap(cropArea.Width, cropArea.Height);
            using (Graphics gph = Graphics.FromImage(bmp))
            {
                gph.DrawImage(img, new Rectangle(0, 0, bmp.Width, bmp.Height), cropArea, GraphicsUnit.Pixel);
            }
            return bmp;
        }

        /// <summary>resize image (untested)</summary>
        /// <param name="newWidth"> </param>
        /// <param name=" newHeight"> </param>
        /// <param name=" stPhotoPath"> </param>
        public Image resizeImage(int newWidth, int newHeight, string stPhotoPath)
        {
            Image imgPhoto = Image.FromFile(stPhotoPath);

            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;

            //Consider vertical pics
            if (sourceWidth < sourceHeight)
            {
                int buff = newWidth;

                newWidth = newHeight;
                newHeight = buff;
            }

            int sourceX = 0, sourceY = 0, destX = 0, destY = 0;
            float nPercent = 0, nPercentW = 0, nPercentH = 0;

            nPercentW = ((float)newWidth / (float)sourceWidth);
            nPercentH = ((float)newHeight / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((newWidth -
                          (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((newHeight -
                          (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);


            Bitmap bmPhoto = new Bitmap(newWidth, newHeight,
                          PixelFormat.Format24bppRgb);

            bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
                         imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Black);
            grPhoto.InterpolationMode =
                System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            imgPhoto.Dispose();
            return bmPhoto;
        }

        /// <summary> </summary>
        /// <param name="OriginalFileLocation"> </param>
        /// <param name=" heigth"> </param>
        /// <param name=" width"> </param>
        /// <param name=" Boolean keepAspectRatio"> </param>
        /// <param name=" Boolean getCenter"> </param>
        public Image resizeImageFromFile(String OriginalFileLocation, int heigth, int width, Boolean keepAspectRatio, Boolean getCenter)
        {
            /// <summary>
            /// Resize image with a directory as source
            /// </summary>
            /// <param name="OriginalFileLocation">Image location</param>
            /// <param name="heigth">new height</param>
            /// <param name="width">new width</param>
            /// <param name="keepAspectRatio">keep the aspect ratio</param>
            /// <param name="getCenter">return the center bit of the image</param>
            /// <returns>image with new dimentions</returns>

            int newheigth = heigth;
            System.Drawing.Image FullsizeImage = System.Drawing.Image.FromFile(OriginalFileLocation);

            // Prevent using images internal thumbnail
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

            if (keepAspectRatio || getCenter)
            {
                int bmpY = 0;
                double resize = (double)FullsizeImage.Width / (double)width;//get the resize vector
                if (getCenter)
                {
                    bmpY = (int)((FullsizeImage.Height - (heigth * resize)) / 2);// gives the Y value of the part that will be cut off, to show only the part in the center
                    Rectangle section = new Rectangle(new Point(0, bmpY), new Size(FullsizeImage.Width, (int)(heigth * resize)));// create the section to cut of the original image
                    //System.Console.WriteLine("the section that will be cut off: " + section.Size.ToString() + " the Y value is minimized by: " + bmpY);
                    Bitmap orImg = new Bitmap((Bitmap)FullsizeImage);//for the correct effect convert image to bitmap.
                    FullsizeImage.Dispose();//clear the original image
                    using (Bitmap tempImg = new Bitmap(section.Width, section.Height))
                    {
                        Graphics cutImg = Graphics.FromImage(tempImg);//              set the file to save the new image to.
                        cutImg.DrawImage(orImg, 0, 0, section, GraphicsUnit.Pixel);// cut the image and save it to tempImg
                        FullsizeImage = tempImg;//save the tempImg as FullsizeImage for resizing later
                        orImg.Dispose();
                        cutImg.Dispose();
                        return FullsizeImage.GetThumbnailImage(width, heigth, null, IntPtr.Zero);
                    }
                }
                else newheigth = (int)(FullsizeImage.Height / resize);//  set the new heigth of the current image
            }//return the image resized to the given heigth and width
            return FullsizeImage.GetThumbnailImage(width, newheigth, null, IntPtr.Zero);
        }

        /// <summary>Resize image (works)</summary>
        /// <param name="Image source"> </param>
        /// <param name="W"> </param>
        /// <param name="H"> </param>
        public Image ResizeImage(Image source, int W = 25, int H = 50)
        {
            // Create a rectangle.
            Rectangle rectangle1 = new Rectangle(0, 0, W, H);

            // Convert it to a RectangleF.
            RectangleF destinationBounds = rectangle1;


            RectangleF sourceBounds = new RectangleF(0.0f, 0.0f, (float)source.Width, (float)source.Height);
            RectangleF scaleBounds = new RectangleF();

            Image destinationImage = new Bitmap((int)destinationBounds.Width, (int)destinationBounds.Height);
            Graphics graph = Graphics.FromImage(destinationImage);
            graph.InterpolationMode =
                System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            // Fill with background color
            graph.FillRectangle(new SolidBrush(System.Drawing.Color.White), destinationBounds);

            float resizeRatio, sourceRatio;
            float scaleWidth, scaleHeight;

            sourceRatio = (float)source.Width / (float)source.Height;

            if (sourceRatio >= 1.0f)
            {
                //landscape
                resizeRatio = destinationBounds.Width / sourceBounds.Width;
                scaleWidth = destinationBounds.Width;
                scaleHeight = sourceBounds.Height * resizeRatio;
                float trimValue = destinationBounds.Height - scaleHeight;
                graph.DrawImage(source, 0, (trimValue / 2), destinationBounds.Width, scaleHeight);
            }
            else
            {
                //portrait
                resizeRatio = destinationBounds.Height / sourceBounds.Height;
                scaleWidth = sourceBounds.Width * resizeRatio;
                scaleHeight = destinationBounds.Height;
                float trimValue = destinationBounds.Width - scaleWidth;
                graph.DrawImage(source, (trimValue / 2), 0, scaleWidth, destinationBounds.Height);
            }

            return destinationImage;

        }

        /// <summary>Resize Image from File Path (works)</summary>
        /// <param name="ImagePath"> </param>
        /// <param name="W"> </param>
        /// <param name="H"> </param>
        public Image Resize_Image_FromFile(string ImagePath, int W = 25, int H = 50)
        {
            Image source = Image.FromFile(ImagePath);

            // Create a rectangle.
            Rectangle rectangle1 = new Rectangle(0, 0, W, H);

            // Convert it to a RectangleF.
            RectangleF destinationBounds = rectangle1;


            RectangleF sourceBounds = new RectangleF(0.0f, 0.0f, (float)source.Width, (float)source.Height);
            RectangleF scaleBounds = new RectangleF();

            Image destinationImage = new Bitmap((int)destinationBounds.Width, (int)destinationBounds.Height);
            Graphics graph = Graphics.FromImage(destinationImage);
            graph.InterpolationMode =
                System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            // Fill with background color
            graph.FillRectangle(new SolidBrush(System.Drawing.Color.White), destinationBounds);

            float resizeRatio, sourceRatio;
            float scaleWidth, scaleHeight;

            sourceRatio = (float)source.Width / (float)source.Height;

            if (sourceRatio >= 1.0f)
            {
                //landscape
                resizeRatio = destinationBounds.Width / sourceBounds.Width;
                scaleWidth = destinationBounds.Width;
                scaleHeight = sourceBounds.Height * resizeRatio;
                float trimValue = destinationBounds.Height - scaleHeight;
                graph.DrawImage(source, 0, (trimValue / 2), destinationBounds.Width, scaleHeight);
            }
            else
            {
                //portrait
                resizeRatio = destinationBounds.Height / sourceBounds.Height;
                scaleWidth = sourceBounds.Width * resizeRatio;
                scaleHeight = destinationBounds.Height;
                float trimValue = destinationBounds.Width - scaleWidth;
                graph.DrawImage(source, (trimValue / 2), 0, scaleWidth, destinationBounds.Height);
            }

            return destinationImage;

        }


        #endregion


        #region === Image MetaData ===

        /// <summary> </summary>
        /// <param name="InFile"> </param>
        /// <param name=" OutFile"> </param>
        public void Write_MetaData(string InFile, string OutFile)
        {
            Image image = Image.FromFile(InFile);

            PropertyItem pi = image.GetPropertyItem(20752);

            pi.Id = 20753;

            pi.Type = 1;
            pi.Value = Encoding.UTF8.GetBytes("MyImageInfo");
            pi.Len = pi.Value.Length;
            image.SetPropertyItem(pi);

            image.Save(OutFile);
        }

        /// <summary> </summary>
        /// <param name="FilePath"> </param>
        public string Read_MetaData(string FilePath)
        {
            Image image = Image.FromFile(FilePath);

            PropertyItem propItem = image.GetPropertyItem(20753);

            string Value = propItem.Value.ToString();

            ahk.MsgBox(Value);

            return Value;
        }


        public int ImageWidth(string FilePath)
        {
            Image Bmap = To_Image(FilePath);

            //Bitmap Bmap = Image.FromFile(FilePath);
            //int BmH = Bmap.Height;
            int BmW = Bmap.Width;
            Bmap.Dispose(); // Avoid Out of Memory errors

            return BmW;
        }

        public int ImageHeight(string FilePath)
        {
            Image Bmap = To_Image(FilePath);

            //Bitmap Bmap = Image.FromFile(FilePath);
            int BmH = Bmap.Height;
            //int BmW = Bmap.Width;
            Bmap.Dispose(); // Avoid Out of Memory errors

            return BmH;
        }


        #endregion


        #region === Color ===

        /// <summary> </summary>
        /// <param name="Color value"> </param>
        public Color Get_Contrasting_Color(Color value)
        {
            var d = 0;

            // Counting the perceptive luminance - human eye favors green color... 
            double a = 1 - (0.299 * value.R + 0.587 * value.G + 0.114 * value.B) / 255;

            if (a < 0.5)
                d = 0; // bright colors - black font
            else
                d = 255; // dark colors - white font

            return Color.FromArgb(d, d, d);
        }


        // source: https://stackoverflow.com/questions/9871262/replace-color-in-an-image-in-c-sharp   - Untested
        public Image ColorReplace(Image inputImage, int tolerance, Color oldColor, Color NewColor)
        {
            Bitmap outputImage = new Bitmap(inputImage.Width, inputImage.Height);
            Graphics G = Graphics.FromImage(outputImage);
            G.DrawImage(inputImage, 0, 0);
            for (Int32 y = 0; y < outputImage.Height; y++)
                for (Int32 x = 0; x < outputImage.Width; x++)
                {
                    Color PixelColor = outputImage.GetPixel(x, y);
                    if (PixelColor.R > oldColor.R - tolerance && PixelColor.R < oldColor.R + tolerance && PixelColor.G > oldColor.G - tolerance && PixelColor.G < oldColor.G + tolerance && PixelColor.B > oldColor.B - tolerance && PixelColor.B < oldColor.B + tolerance)
                    {
                        int RColorDiff = oldColor.R - PixelColor.R;
                        int GColorDiff = oldColor.G - PixelColor.G;
                        int BColorDiff = oldColor.B - PixelColor.B;

                        if (PixelColor.R > oldColor.R) RColorDiff = NewColor.R + RColorDiff;
                        else RColorDiff = NewColor.R - RColorDiff;
                        if (RColorDiff > 255) RColorDiff = 255;
                        if (RColorDiff < 0) RColorDiff = 0;
                        if (PixelColor.G > oldColor.G) GColorDiff = NewColor.G + GColorDiff;
                        else GColorDiff = NewColor.G - GColorDiff;
                        if (GColorDiff > 255) GColorDiff = 255;
                        if (GColorDiff < 0) GColorDiff = 0;
                        if (PixelColor.B > oldColor.B) BColorDiff = NewColor.B + BColorDiff;
                        else BColorDiff = NewColor.B - BColorDiff;
                        if (BColorDiff > 255) BColorDiff = 255;
                        if (BColorDiff < 0) BColorDiff = 0;

                        outputImage.SetPixel(x, y, Color.FromArgb(RColorDiff, GColorDiff, BColorDiff));
                    }
                }
            return outputImage;
        }


        /// <summary>
        /// Returns List of Unique Argb Colors Found In Image
        /// </summary>
        /// <param name="ImagePath"></param>
        /// <returns></returns>
        public List<Color> ImageColors(string ImagePath)
        {
            List<Color> colors = new List<Color>();

            //Converting loaded image into bitmap
            Bitmap bmp = new Bitmap(ImagePath);

            //Iterate whole bitmap to findout the picked color
            for (int i = 0; i < bmp.Height; i++)
            {
                for (int j = 0; j < bmp.Width; j++)
                {
                    //Get the color at each pixel
                    Color now_color = bmp.GetPixel(j, i);

                    // add color to list if color isn't already in list
                    bool NewColor = true;
                    foreach (Color col in colors)
                    {
                        if (now_color == col) { NewColor = false; break; }
                    }

                    if (NewColor) { colors.Add(now_color); }


                    ////Compare Pixel's Color ARGB property with the picked color's ARGB property 
                    //if (now_color.ToArgb() == colorDialog1.Color.ToArgb())
                    //{
                    //    IsColorFound = true;
                    //    MessageBox.Show("Color Found!");
                    //    break;
                    //}
                }
                //if (IsColorFound == true)
                //{
                //    break;
                //}
            }

            return colors;
        }


        #endregion


        #region === Image from DataBase ===

        // return Image stored in SQL database - convert back to Image format to display
        //Image returnImg = Return_Db_Icon(GoDad, "SELECT [ICO_Img] FROM [sharpAHK].[dbo].[FunctionLib] where FunctionName = 'FileAppend'");

        /// <summary>returns file from sql database, saves to local file path</summary>
        /// <param name="SQLConn"> </param>
        /// <param name=" SQLSearchLine"> </param>
        /// <param name="LocalFilePath"> </param>
        public Image Return_Db_Icon(SqlConnection SQLConn, string SQLSearchLine, string LocalFilePath = "")
        {
            //string SQLSearchLine = "SELECT [FileContents] FROM [MediaServer].[lucid].[FileBin] WHERE [FileName] = 'TestFile.zip'";

            bool DeleteTmpFile = false;
            if (LocalFilePath == "") { DeleteTmpFile = true; LocalFilePath = ahk.AppDir() + "\\IconTemp.png"; }

            if (SQLConn.State == ConnectionState.Closed) { SQLConn.Open(); }
            using (var sqlQuery = new SqlCommand(SQLSearchLine, SQLConn))
            {
                //sqlQuery.Parameters.AddWithValue("@FileName", ServerFileName);
                using (var sqlQueryResult = sqlQuery.ExecuteReader())
                    if (sqlQueryResult != null)
                    {
                        sqlQueryResult.Read();
                        var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
                        sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);
                        using (var fs = new FileStream(LocalFilePath, FileMode.Create, FileAccess.Write))
                            fs.Write(blob, 0, blob.Length);
                    }
            }

            SQLConn.Close();

            //Image returnImg = img.GetCopyImage(LocalFilePath);  // convert local return img to Image format

            Image returnImg = Image.FromFile(LocalFilePath);

            //if (DeleteTmpFile) { ahk.FileDelete(LocalFilePath); }  // delete local temp image

            return returnImg;
        }



        #endregion


        #region === Screen Capture ===

        // Use this version to capture the full extended desktop (i.e. multiple screens)

        /// <summary> </summary>
        /// <param name="SaveFile"> </param>
        public void CaptureScreenShot(string SaveFile)
        {
            //==============================================================
            // capture entire screen (multiple monitors included)
            //==============================================================
            Bitmap screenshot = new Bitmap(SystemInformation.VirtualScreen.Width,
                                           SystemInformation.VirtualScreen.Height,
                                           PixelFormat.Format32bppArgb);
            Graphics screenGraph = Graphics.FromImage(screenshot);
            screenGraph.CopyFromScreen(SystemInformation.VirtualScreen.X,
                                       SystemInformation.VirtualScreen.Y,
                                       0,
                                       0,
                                       SystemInformation.VirtualScreen.Size,
                                       CopyPixelOperation.SourceCopy);

            screenshot.Save(SaveFile, System.Drawing.Imaging.ImageFormat.Png);




            //==============================================================
            //Create Screen Shot of PRIMARY Monitor
            //==============================================================
            var bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                           Screen.PrimaryScreen.Bounds.Height,
                                           PixelFormat.Format32bppArgb);

            // Create a graphics object from the bitmap.
            var gfxScreenshot = Graphics.FromImage(bmpScreenshot);

            // Take the screenshot from the upper left corner to the right bottom corner.
            gfxScreenshot.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                        Screen.PrimaryScreen.Bounds.Y,
                                        0,
                                        0,
                                        Screen.PrimaryScreen.Bounds.Size,
                                        CopyPixelOperation.SourceCopy);

            // Save the screenshot to the specified path that the user has chosen.
            bmpScreenshot.Save(SaveFile, ImageFormat.Png);



        }


        /// <summary> </summary>
        /// <param name="showCursor"> </param>
        /// <param name=" Size curSize"> </param>
        /// <param name=" PocurPos"> </param>
        /// <param name=" PoSourcePoint"> </param>
        /// <param name=" PoDestinationPoint"> </param>
        /// <param name=" Rectangle SelectionRectangle"> </param>
        /// <param name=" FilePath"> </param>
        /// <param name=" extension"> </param>
        public void CaptureImage(bool showCursor, Size curSize, Point curPos, Point SourcePoint, Point DestinationPoint, Rectangle SelectionRectangle, string FilePath, string extension)
        {
            bool saveToClipboard = false;  //optional

            using (Bitmap bitmap = new Bitmap(SelectionRectangle.Width, SelectionRectangle.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(SourcePoint, DestinationPoint, SelectionRectangle.Size);

                    if (showCursor)
                    {
                        Rectangle cursorBounds = new Rectangle(curPos, curSize);
                        Cursors.Default.Draw(g, cursorBounds);
                    }
                }

                if (saveToClipboard)
                {
                    Image img = (Image)bitmap;
                    Clipboard.SetImage(img);
                }
                else
                {
                    switch (extension)
                    {
                        case ".bmp":
                            bitmap.Save(FilePath, ImageFormat.Bmp);
                            break;
                        case ".jpg":
                            bitmap.Save(FilePath, ImageFormat.Jpeg);
                            break;
                        case ".gif":
                            bitmap.Save(FilePath, ImageFormat.Gif);
                            break;
                        case ".tiff":
                            bitmap.Save(FilePath, ImageFormat.Tiff);
                            break;
                        case ".png":
                            bitmap.Save(FilePath, ImageFormat.Png);
                            break;
                        default:
                            bitmap.Save(FilePath, ImageFormat.Jpeg);
                            break;
                    }
                }
            }

        }



        #region === Screen Capture ===

        public void ScreenCap_Capture_MouseClick(bool RightClick = false, bool DoubleClick = false)
        {
            //// show both pictureboxes to populate during capture
            //try
            //{
            //    MethodInvoker inv = delegate { pictureBox1.Visible = true; pictureBox2.Visible = true; };
            //    this.Invoke(inv);
            //}
            //catch
            //{
            //    pictureBox1.Visible = true; pictureBox2.Visible = true;
            //}

            //ahk.FileCreateDir(ImageDir);

            //// take base name and find next available name with a digit added
            //string NextCaptureImage = ahk.NextFileName(ImageDir + "\\MouseCap.png");

            //// capture image around current mouse position
            //ScreenCap_Mouse(NextCaptureImage, ImageFormat.Png, 20, 20, true, RightClick, DoubleClick);

            //// store the current mouse position / window title to assist image search
            //CaptureX = Cursor.Position.X;
            //CaptureY = Cursor.Position.Y;
            //CaptureWinTitle = ahk.WinGetActiveTitle();


            //try
            //{
            //    MethodInvoker inv = delegate { txtXPos.Text = CaptureX.ToString(); txtYPos.Text = CaptureY.ToString(); txtWinTitle.Text = CaptureWinTitle; };
            //    this.Invoke(inv);
            //}
            //catch
            //{
            //    txtXPos.Text = CaptureX.ToString(); txtYPos.Text = CaptureY.ToString(); txtWinTitle.Text = CaptureWinTitle;
            //}


            //Populate_Grid(); // update grid with macro additions

            ////ahk.Run(@"C:\Users\Jason\Google Drive\IMDB\MouseCap.png");
            //StatusBar("Image Around Mouse Captured");
        }


        public void ScreenCap(string SavePath, ImageFormat format, bool PrimaryOnly = true)
        {
            Bitmap screenBitmap = new Bitmap(1024, 768, PixelFormat.Format32bppArgb);

            Rectangle screenRegion = Screen.PrimaryScreen.Bounds;
            if (!PrimaryOnly) { screenRegion = Screen.AllScreens[0].Bounds; }

            // Copy the current screep image to the bitmap image
            Graphics screenGraphics = Graphics.FromImage(screenBitmap);
            screenGraphics.CopyFromScreen(screenRegion.Left, screenRegion.Top, 0, 0, screenRegion.Size);

            screenBitmap.Save(SavePath, format);
        }

        // take screenshot around coordinates on screen 
        public void ScreenCap_Region(string SavePath, ImageFormat format, int X, int Y, int W, int H)
        {
            Bitmap screenBitmap = new Bitmap(W, H, PixelFormat.Format32bppArgb);
            Rectangle screenRegion = Screen.AllScreens[0].Bounds;

            // Copy the current screep image to the bitmap image
            Graphics screenGraphics = Graphics.FromImage(screenBitmap);
            screenGraphics.CopyFromScreen(X, Y, 0, 0, screenRegion.Size);

            screenBitmap.Save(SavePath, format);
        }

        // takes screenshot around current mouse position
        public void ScreenCap_Mouse(string SavePath, ImageFormat format, int W, int H, bool TwoShots = true, bool RightClick = false, bool DoubleClick = false)
        {
            // take screen shot around current mouse position

            Bitmap screenBitmap = new Bitmap(W, H, PixelFormat.Format32bppArgb);
            Rectangle screenRegion = Screen.AllScreens[0].Bounds;

            int CurrentX = Cursor.Position.X;
            int CurrentY = Cursor.Position.Y;
            string CaptureWinTitle = ahk.WinGetActiveTitle();

            // It will copy the current screep image to the bitmap image.
            Graphics screenGraphics = Graphics.FromImage(screenBitmap);
            screenGraphics.CopyFromScreen(Cursor.Position.X - 5, Cursor.Position.Y - 5, 0, 0, screenRegion.Size);

            screenBitmap.Save(SavePath, format);

            //il.Load_PictureBox(pictureBox1, SavePath);  // display captured image

            // populate / write mouse macro entry in database
            MouseMacro ClickInfo = new MouseMacro();
            ClickInfo.MouseX = CurrentX.ToString();
            ClickInfo.MouseY = CurrentY.ToString();
            ClickInfo.ActiveWindow = CaptureWinTitle;
            //ClickInfo.MacroName = currentMacroName;
            ClickInfo.SearchImage = SavePath;

            ClickInfo.MouseAction = "Find Click"; // default = left click
            ClickInfo.Description = "IMAGE SEARCH : Find " + ahk.FileName(SavePath) + " On Screen and Click";
            ClickInfo.Code = "bool FoundImage = ImgSearch_Find_Click(\"" + SavePath + "\", 10);";

            if (RightClick) { ClickInfo.MouseAction = "Find And RightClick"; ClickInfo.Description = "IMAGE SEARCH : Find " + ahk.FileName(SavePath) + " On Screen and Right Click"; ClickInfo.Code = "bool FoundImage = ImgSearch_Find_RightClick(\"" + SavePath + "\", 10);"; }
            if (DoubleClick) { ClickInfo.MouseAction = "Find And DoubleClick"; ClickInfo.Description = "IMAGE SEARCH : Find " + ahk.FileName(SavePath) + " On Screen and Double Click"; ClickInfo.Code = "bool FoundImage = ImgSearch_Find_DoubleClick(\"" + SavePath + "\", 10);"; }

            //sqlite.Insert_Object_MouseMacro(MacroDb, ClickInfo);

            // option to take 2 shots, one with mouse there and one moved away (helps with icons that change with mouse over)
            if (TwoShots)
            {
                ahk.MouseMove(CurrentX + 200, CurrentY + 200);  // move mouse away from capture area first (changes the look of mouse-over buttons etc)
                ahk.Sleep(500);

                // take 2nd screen shot around previous mouse position

                Bitmap screenBitmap2 = new Bitmap(W, H, PixelFormat.Format32bppArgb);
                Rectangle screenRegion2 = Screen.AllScreens[0].Bounds;

                // It will copy the current screep image to the bitmap image.
                Graphics screenGraphics2 = Graphics.FromImage(screenBitmap2);
                screenGraphics2.CopyFromScreen(CurrentX - 5, CurrentY - 5, 0, 0, screenRegion.Size);

                string SecondFile = ahk.FileDir(SavePath) + "\\" + ahk.FileNameNoExt(SavePath) + ".2" + ahk.FileExt(SavePath);

                //ahk.MsgBox(SecondFile);

                screenBitmap2.Save(SecondFile, format);

                ahk.MouseMove(CurrentX, CurrentY);  // move back to original mouse position

                //il.Load_PictureBox(pictureBox2, SecondFile); // display captured image

                //// populate / write mouse macro entry in database
                //ClickInfo = new MouseMacro();
                //ClickInfo.MouseX = CurrentX.ToString();
                //ClickInfo.MouseY = CurrentY.ToString();
                //ClickInfo.ActiveWindow = CaptureWinTitle;
                //ClickInfo.MacroName = "Image Capture";
                //ClickInfo.MouseAction = "Find Click 2";
                //ClickInfo.SearchImage = SecondFile;
                //sqlite.InsertMouseMacro(ClickInfo, MacroDb); 
            }


        }


        #endregion


        #endregion

    }
}

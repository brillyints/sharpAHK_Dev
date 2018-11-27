namespace sharpAHK_Dev
{
    partial class TelerikTree
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.radMenu1 = new Telerik.WinControls.UI.RadMenu();
            this.radMenuItem1 = new Telerik.WinControls.UI.RadMenuItem();
            this.menuFileTree = new Telerik.WinControls.UI.RadMenuItem();
            this.menuShowDateIndex = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenuItem2 = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenuItem3 = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenuThreads = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenuSharpTree = new Telerik.WinControls.UI.RadMenuItem();
            this.radMenuConfig = new Telerik.WinControls.UI.RadMenuItem();
            this.radStatusStrip1 = new Telerik.WinControls.UI.RadStatusStrip();
            this.radLabelElement1 = new Telerik.WinControls.UI.RadLabelElement();
            this.radPageView1 = new Telerik.WinControls.UI.RadPageView();
            this.radPageSharpTree = new Telerik.WinControls.UI.RadPageViewPage();
            this.radTreeView1 = new Telerik.WinControls.UI.RadTreeView();
            this.radProgressBar1 = new Telerik.WinControls.UI.RadProgressBar();
            this.txtFilterText = new Telerik.WinControls.UI.RadTextBox();
            this.radPageConfig = new Telerik.WinControls.UI.RadPageViewPage();
            this.txtShowName = new Telerik.WinControls.UI.RadTextBox();
            this.btnShowDateIndex = new Telerik.WinControls.UI.RadButton();
            this.btnFileTree = new Telerik.WinControls.UI.RadButton();
            this.radLabel6 = new Telerik.WinControls.UI.RadLabel();
            this.txtPath = new Telerik.WinControls.UI.RadTextBox();
            this.radPageThreads = new Telerik.WinControls.UI.RadPageViewPage();
            ((System.ComponentModel.ISupportInitialize)(this.radMenu1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radStatusStrip1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPageView1)).BeginInit();
            this.radPageView1.SuspendLayout();
            this.radPageSharpTree.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radTreeView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radProgressBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFilterText)).BeginInit();
            this.radPageConfig.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtShowName)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnShowDateIndex)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFileTree)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPath)).BeginInit();
            this.SuspendLayout();
            // 
            // radMenu1
            // 
            this.radMenu1.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.radMenuItem1,
            this.radMenuItem2,
            this.radMenuItem3});
            this.radMenu1.Location = new System.Drawing.Point(0, 0);
            this.radMenu1.Name = "radMenu1";
            this.radMenu1.Size = new System.Drawing.Size(336, 20);
            this.radMenu1.TabIndex = 0;
            // 
            // radMenuItem1
            // 
            this.radMenuItem1.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.menuFileTree,
            this.menuShowDateIndex});
            this.radMenuItem1.Name = "radMenuItem1";
            this.radMenuItem1.Text = "Load Tree";
            // 
            // menuFileTree
            // 
            this.menuFileTree.Name = "menuFileTree";
            this.menuFileTree.Text = "FileTree";
            this.menuFileTree.Click += new System.EventHandler(this.menuFileTree_Click);
            // 
            // menuShowDateIndex
            // 
            this.menuShowDateIndex.Name = "menuShowDateIndex";
            this.menuShowDateIndex.Text = "ShowDateIndex";
            this.menuShowDateIndex.Click += new System.EventHandler(this.menuShowDateIndex_Click);
            // 
            // radMenuItem2
            // 
            this.radMenuItem2.Name = "radMenuItem2";
            this.radMenuItem2.Text = "Save Tree";
            // 
            // radMenuItem3
            // 
            this.radMenuItem3.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.radMenuThreads,
            this.radMenuSharpTree,
            this.radMenuConfig});
            this.radMenuItem3.Name = "radMenuItem3";
            this.radMenuItem3.Text = "Options";
            this.radMenuItem3.Click += new System.EventHandler(this.radMenuThreadsPage_Click);
            // 
            // radMenuThreads
            // 
            this.radMenuThreads.Name = "radMenuThreads";
            this.radMenuThreads.Text = "Threads";
            this.radMenuThreads.Click += new System.EventHandler(this.radMenuThreadsPage_Click);
            // 
            // radMenuSharpTree
            // 
            this.radMenuSharpTree.Name = "radMenuSharpTree";
            this.radMenuSharpTree.Text = "SharpTree";
            this.radMenuSharpTree.Click += new System.EventHandler(this.radMenuThreadsPage_Click);
            // 
            // radMenuConfig
            // 
            this.radMenuConfig.Name = "radMenuConfig";
            this.radMenuConfig.Text = "Config";
            this.radMenuConfig.Click += new System.EventHandler(this.radMenuThreadsPage_Click);
            // 
            // radStatusStrip1
            // 
            this.radStatusStrip1.Items.AddRange(new Telerik.WinControls.RadItem[] {
            this.radLabelElement1});
            this.radStatusStrip1.Location = new System.Drawing.Point(0, 449);
            this.radStatusStrip1.Name = "radStatusStrip1";
            this.radStatusStrip1.Size = new System.Drawing.Size(336, 26);
            this.radStatusStrip1.TabIndex = 1;
            // 
            // radLabelElement1
            // 
            this.radLabelElement1.MinSize = new System.Drawing.Size(150, 0);
            this.radLabelElement1.Name = "radLabelElement1";
            this.radStatusStrip1.SetSpring(this.radLabelElement1, false);
            this.radLabelElement1.Text = "radLabelElement1";
            this.radLabelElement1.TextWrap = true;
            // 
            // radPageView1
            // 
            this.radPageView1.Controls.Add(this.radPageSharpTree);
            this.radPageView1.Controls.Add(this.radPageConfig);
            this.radPageView1.Controls.Add(this.radPageThreads);
            this.radPageView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radPageView1.Location = new System.Drawing.Point(0, 20);
            this.radPageView1.Name = "radPageView1";
            this.radPageView1.SelectedPage = this.radPageSharpTree;
            this.radPageView1.Size = new System.Drawing.Size(336, 429);
            this.radPageView1.TabIndex = 2;
            this.radPageView1.SelectedPageChanged += new System.EventHandler(this.radPageView1_SelectedPageChanged);
            // 
            // radPageSharpTree
            // 
            this.radPageSharpTree.Controls.Add(this.radTreeView1);
            this.radPageSharpTree.Controls.Add(this.radProgressBar1);
            this.radPageSharpTree.Controls.Add(this.txtFilterText);
            this.radPageSharpTree.ItemSize = new System.Drawing.SizeF(67F, 28F);
            this.radPageSharpTree.Location = new System.Drawing.Point(10, 37);
            this.radPageSharpTree.Name = "radPageSharpTree";
            this.radPageSharpTree.Size = new System.Drawing.Size(315, 381);
            this.radPageSharpTree.Text = "SharpTree";
            // 
            // radTreeView1
            // 
            this.radTreeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.radTreeView1.KeyboardSearchEnabled = true;
            this.radTreeView1.Location = new System.Drawing.Point(0, 20);
            this.radTreeView1.Name = "radTreeView1";
            this.radTreeView1.Size = new System.Drawing.Size(315, 337);
            this.radTreeView1.SpacingBetweenNodes = -1;
            this.radTreeView1.TabIndex = 0;
            this.radTreeView1.NodeMouseClick += new Telerik.WinControls.UI.RadTreeView.TreeViewEventHandler(this.radTreeView1_NodeMouseClick);
            this.radTreeView1.NodeMouseDoubleClick += new Telerik.WinControls.UI.RadTreeView.TreeViewEventHandler(this.radTreeView1_NodeMouseDoubleClick);
            this.radTreeView1.ToolTipTextNeeded += new Telerik.WinControls.ToolTipTextNeededEventHandler(this.radTreeView1_ToolTipTextNeeded);
            // 
            // radProgressBar1
            // 
            this.radProgressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.radProgressBar1.Location = new System.Drawing.Point(0, 357);
            this.radProgressBar1.Name = "radProgressBar1";
            this.radProgressBar1.Size = new System.Drawing.Size(315, 24);
            this.radProgressBar1.TabIndex = 2;
            this.radProgressBar1.Text = "radProgressBar1";
            // 
            // txtFilterText
            // 
            this.txtFilterText.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtFilterText.Location = new System.Drawing.Point(0, 0);
            this.txtFilterText.Name = "txtFilterText";
            this.txtFilterText.Size = new System.Drawing.Size(315, 20);
            this.txtFilterText.TabIndex = 1;
            this.txtFilterText.TextChanged += new System.EventHandler(this.txtFilterText_TextChanged);
            this.txtFilterText.KeyDown += new System.Windows.Forms.KeyEventHandler(this.radTextBox1_KeyDown);
            // 
            // radPageConfig
            // 
            this.radPageConfig.Controls.Add(this.txtShowName);
            this.radPageConfig.Controls.Add(this.btnShowDateIndex);
            this.radPageConfig.Controls.Add(this.btnFileTree);
            this.radPageConfig.Controls.Add(this.radLabel6);
            this.radPageConfig.Controls.Add(this.txtPath);
            this.radPageConfig.ItemSize = new System.Drawing.SizeF(49F, 28F);
            this.radPageConfig.Location = new System.Drawing.Point(10, 37);
            this.radPageConfig.Name = "radPageConfig";
            this.radPageConfig.Size = new System.Drawing.Size(315, 381);
            this.radPageConfig.Text = "Config";
            // 
            // txtShowName
            // 
            this.txtShowName.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtShowName.Location = new System.Drawing.Point(0, 295);
            this.txtShowName.Name = "txtShowName";
            this.txtShowName.Size = new System.Drawing.Size(315, 20);
            this.txtShowName.TabIndex = 23;
            this.txtShowName.Text = "The Daily Show";
            // 
            // btnShowDateIndex
            // 
            this.btnShowDateIndex.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnShowDateIndex.Location = new System.Drawing.Point(0, 315);
            this.btnShowDateIndex.Name = "btnShowDateIndex";
            this.btnShowDateIndex.Size = new System.Drawing.Size(315, 23);
            this.btnShowDateIndex.TabIndex = 22;
            this.btnShowDateIndex.Text = "ShowDate Index";
            this.btnShowDateIndex.Click += new System.EventHandler(this.btnShowDateIndex_Click);
            // 
            // btnFileTree
            // 
            this.btnFileTree.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnFileTree.Location = new System.Drawing.Point(0, 338);
            this.btnFileTree.Name = "btnFileTree";
            this.btnFileTree.Size = new System.Drawing.Size(315, 23);
            this.btnFileTree.TabIndex = 21;
            this.btnFileTree.Text = "FileTree";
            this.btnFileTree.Click += new System.EventHandler(this.btnFileTree_Click);
            // 
            // radLabel6
            // 
            this.radLabel6.Location = new System.Drawing.Point(-116, 129);
            this.radLabel6.Name = "radLabel6";
            this.radLabel6.Size = new System.Drawing.Size(29, 18);
            this.radLabel6.TabIndex = 20;
            this.radLabel6.Text = "Path";
            // 
            // txtPath
            // 
            this.txtPath.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtPath.Location = new System.Drawing.Point(0, 361);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(315, 20);
            this.txtPath.TabIndex = 19;
            this.txtPath.Text = "D:\\WinFiles\\Google Drive\\CodeBooK";
            // 
            // radPageThreads
            // 
            this.radPageThreads.ItemSize = new System.Drawing.SizeF(56F, 28F);
            this.radPageThreads.Location = new System.Drawing.Point(10, 37);
            this.radPageThreads.Name = "radPageThreads";
            this.radPageThreads.Size = new System.Drawing.Size(315, 381);
            this.radPageThreads.Text = "Threads";
            // 
            // TelerikTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radPageView1);
            this.Controls.Add(this.radStatusStrip1);
            this.Controls.Add(this.radMenu1);
            this.Name = "TelerikTree";
            this.Size = new System.Drawing.Size(336, 475);
            ((System.ComponentModel.ISupportInitialize)(this.radMenu1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radStatusStrip1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radPageView1)).EndInit();
            this.radPageView1.ResumeLayout(false);
            this.radPageSharpTree.ResumeLayout(false);
            this.radPageSharpTree.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.radTreeView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radProgressBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtFilterText)).EndInit();
            this.radPageConfig.ResumeLayout(false);
            this.radPageConfig.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtShowName)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnShowDateIndex)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.btnFileTree)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.radLabel6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.txtPath)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Telerik.WinControls.UI.RadMenu radMenu1;
        private Telerik.WinControls.UI.RadMenuItem radMenuItem1;
        private Telerik.WinControls.UI.RadMenuItem radMenuItem2;
        private Telerik.WinControls.UI.RadMenuItem radMenuItem3;
        private Telerik.WinControls.UI.RadStatusStrip radStatusStrip1;
        private Telerik.WinControls.UI.RadLabelElement radLabelElement1;
        private Telerik.WinControls.UI.RadPageView radPageView1;
        private Telerik.WinControls.UI.RadPageViewPage radPageSharpTree;
        private Telerik.WinControls.UI.RadTreeView radTreeView1;
        private Telerik.WinControls.UI.RadPageViewPage radPageConfig;
        private Telerik.WinControls.UI.RadMenuItem menuFileTree;
        private Telerik.WinControls.UI.RadTextBox txtFilterText;
        private Telerik.WinControls.UI.RadMenuItem menuShowDateIndex;
        private Telerik.WinControls.UI.RadProgressBar radProgressBar1;
        private Telerik.WinControls.UI.RadButton btnFileTree;
        private Telerik.WinControls.UI.RadLabel radLabel6;
        private Telerik.WinControls.UI.RadTextBox txtPath;
        private Telerik.WinControls.UI.RadButton btnShowDateIndex;
        private Telerik.WinControls.UI.RadTextBox txtShowName;
        private Telerik.WinControls.UI.RadPageViewPage radPageThreads;
        private Telerik.WinControls.UI.RadMenuItem radMenuThreads;
        private Telerik.WinControls.UI.RadMenuItem radMenuSharpTree;
        private Telerik.WinControls.UI.RadMenuItem radMenuConfig;
    }
}

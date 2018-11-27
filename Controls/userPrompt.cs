using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;

namespace sharpAHK_Dev.Controls
{
    public partial class userPrompt : Telerik.WinControls.UI.RadForm
    {
        public userPrompt(string PromptText = "", string Title = "Yes/No Prompt")
        {
            InitializeComponent();

            this.Text = Title;
            lblDisp.Text = PromptText;

            radButton2.Text = "Yes";
            radButton2.DialogResult = DialogResult.OK;

            radButton3.Text = "No";
            radButton3.DialogResult = DialogResult.Cancel;
        }

        private void btnButton_Click(object sender, EventArgs e)
        {
            RadButton clicked = (RadButton)sender;

            Response = clicked.Text;

            if (clicked.Text == "Cancel") { this.Close(); }
            if (clicked.Text == "No") { this.Close(); }
            if (clicked.Text == "") { }
            if (clicked.Text == "") { }
            if (clicked.Text == "") { }
            if (clicked.Text == "") { }
            if (clicked.Text == "") { }
            if (clicked.Text == "") { }
        }

        public static string Response { get; set; }

    }
}

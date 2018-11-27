using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sharpAHK;
using sharpAHK_Dev;
using AHKExpressions;
using TelerikExpressions;
using System.Diagnostics;
using Telerik.WinControls.UI;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;


namespace sharpAHK_Dev
{
    public partial class _Code
    {

        // === CodeGen SQLite ===

        private string CodeGen_SQLite_CreateTable(Dictionary<string, string> ColNames, string tableName = "tableNAME")
        {
            // SQLite("CREATE TABLE filelist (filepath VARCHAR(20), score INT)", db);  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]

            string CreateLine = "Create Table [" + tableName + "] (";
            string CreateCols = "";

            foreach (string col in ColNames.Keys)
            {
                if (CreateCols != "") { CreateCols = CreateCols + ", " + col + " VARCHAR"; }
                if (CreateCols == "")
                {
                    if (col.ToUpper() == "ID") { CreateCols = col + " INTEGER PRIMARY KEY"; }
                    else { CreateCols = col + " VARCHAR"; }
                }
            }

            string newcolline = CreateLine + CreateCols;
            CreateLine = CreateLine + CreateCols + ")";
            CreateLine = "\t\tstring CreateLine = \"" + CreateLine + "\";";

            // add code to execute this sqlite command - generate new function code
            string Function = "\tpublic bool Create_Table_" + tableName + "(string DbFile)" + "\n\t{\n";
            //if (!DisplayText.Contains("string DbFile =")) { Function = DbFileLine + "\n\n" + Function; }
            string CreateLineCommand = "bool TableCreated = sqlite.Table_Exists(DbFile, \"" + tableName + "\");\nif (!TableCreated) { TableCreated = sqlite.Table_New(DbFile, \"" + tableName + "\", \"" + newcolline + "\", false); }\n";
            CreateLine = CreateLine + "\n\t\t" + CreateLineCommand;
            CreateLine = CreateLine + "\n\r\t\t" + "if(!TableCreated) { ahk.MsgBox(\"[" + tableName + "] Created = \" + TableCreated.ToString()); }";
            Function = Function + CreateLine + "\n\t\treturn TableCreated;\n\t}\n";

            return Function;
        }

        private string CodeGen_Return_Variables(Dictionary<string, string> ColNames)
        {
            string Vars = "";
            int i = 0;
            foreach (string col in ColNames.Keys)
            {
                //if (i == 0) { i++; continue; } // skip the parent node
                if (Vars != "") { Vars = Vars + "\n\t\tstring " + col + " = \"\";"; }
                if (Vars == "") { Vars = "\t\tstring " + col + " = \"\";"; }
            }

            return Vars;
        }

        // returns new object definition
        private string CodeGen_Return_ObjectDef(Dictionary<string, string> ColNames, string ObjectName = "objectName")
        {
            StringBuilder NewString = new StringBuilder();

            NewString.AppendLine("\tpublic struct " + ObjectName);
            NewString.AppendLine("{");

            StringBuilder controlObjects = new StringBuilder();

            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value;
                bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                // based on field type, add line
                if (isTextCol) { NewString.AppendLine("\tpublic string " + colName + " { get; set; }"); controlObjects.AppendLine("\tpublic RadTextBox txt" + colName + " { get; set; }"); }
                if (isIntCol) { NewString.AppendLine("\tpublic int " + colName + " { get; set; }"); controlObjects.AppendLine("\tpublic RadTextBox txt" + colName + " { get; set; }"); }
                if (isImageCol) { NewString.AppendLine("\tpublic string " + colName + " { get; set; }"); controlObjects.AppendLine("\tpublic RadTextBox txt" + colName + " { get; set; }"); }
                if (isDateTimeCol) { NewString.AppendLine("\tpublic DateTime " + colName + " { get; set; }"); controlObjects.AppendLine("\tpublic RadTextBox txt" + colName + " { get; set; }"); }
                if (isCbCol) { NewString.AppendLine("\tpublic bool " + colName + " { get; set; }"); controlObjects.AppendLine("\tpublic RadCheckBox cb" + colName + " { get; set; }"); }
            }

            NewString.AppendLine("");
            NewString.AppendLine(controlObjects.ToString());
            NewString.AppendLine("");
            NewString.AppendLine("}");

            return NewString.ToString();
        }


        private string CodeGen_Return_PopulateControls(Dictionary<string, string> ColNames, string ObjectName = "objectName", string WhereField = "ID")
        {
            StringBuilder NewString = new StringBuilder();
            StringBuilder controlObjects = new StringBuilder();
            StringBuilder ReturnGUI = new StringBuilder();
            StringBuilder SaveGUI = new StringBuilder();

            NewString.AppendLine("\tpublic void PopulateGUI(" + ObjectName + " obj)");
            NewString.AppendLine("{");

            NewString.AppendLine("\t//" + ObjectName + " obj = " + ObjectName + "_ReturnSQL(" + WhereField + ");");


            controlObjects.AppendLine("\tpublic static RadButton btnSave = new RadButton();");
            controlObjects.AppendLine("\tpublic static RadButton btnDelete = new RadButton();");
            controlObjects.AppendLine("\tpublic static RadButton btnPopulate = new RadButton();");
            controlObjects.AppendLine("\tpublic static RadButton btnNew = new RadButton();");

            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value;
                bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                // based on field type, add line
                if (isTextCol) { NewString.AppendLine("\tif (txt" + colName + " != null) { txt" + colName + ".Text = obj." + colName + "; }"); controlObjects.AppendLine("\tpublic static RadTextBox txt" + colName + " = new RadTextBox();"); controlObjects.AppendLine("\tpublic static RadLabel lbl" + colName + " = new RadLabel();"); }
                if (isIntCol) { NewString.AppendLine("\tif (txt" + colName + " != null) { txt" + colName + ".Text = obj." + colName + "; }"); controlObjects.AppendLine("\tpublic static RadTextBox txt" + colName + " = new RadTextBox();"); controlObjects.AppendLine("\tpublic static RadLabel lbl" + colName + " = new RadLabel();"); }
                if (isImageCol) { NewString.AppendLine("\tif (txt" + colName + " != null) { txt" + colName + ".Text = obj." + colName + "; }"); controlObjects.AppendLine("\tpublic static RadTextBox txt" + colName + " = new RadTextBox();"); controlObjects.AppendLine("\tpublic static RadLabel lbl" + colName + " = new RadLabel();"); }
                if (isDateTimeCol) { NewString.AppendLine("\tif (txt" + colName + " != null) { txt" + colName + ".Text = obj." + colName + "; }"); controlObjects.AppendLine("\tpublic static RadTextBox txt" + colName + " = new RadTextBox();"); controlObjects.AppendLine("\tpublic static RadLabel lbl" + colName + " = new RadLabel();"); }
                if (isCbCol) { NewString.AppendLine("\tif (cb" + colName + " != null) { cb" + colName + ".Checked = obj." + colName + "; }"); controlObjects.AppendLine("\tpublic static RadCheckBox cb" + colName + " = new RadCheckBox();"); controlObjects.AppendLine("\tpublic static RadLabel lbl" + colName + " = new RadLabel();"); }
            }

            NewString.AppendLine("");
            NewString.AppendLine("}");
            NewString.AppendLine("");


            ReturnGUI.AppendLine("/// Assign Control Values from Another Form to Populate/Return Values");
            ReturnGUI.AppendLine("\tpublic void AssignGUIControls(" + ObjectName + " obj)");
            ReturnGUI.AppendLine("{");
            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value;
                bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                // based on field type, add line
                if (isTextCol) { ReturnGUI.AppendLine("\ttxt" + colName + " = obj.txt" + colName + ";"); }
                if (isIntCol) { ReturnGUI.AppendLine("\ttxt" + colName + " = obj.txt" + colName + ";"); }
                if (isImageCol) { ReturnGUI.AppendLine("\ttxt" + colName + " = obj.txt" + colName + ";"); }
                if (isDateTimeCol) { ReturnGUI.AppendLine("\ttxt" + colName + " = obj.txt" + colName + ";"); }
                if (isCbCol) { ReturnGUI.AppendLine("\tcb" + colName + " = obj.cb" + colName + ";"); }
            }

            ReturnGUI.AppendLine("\treturn;");
            ReturnGUI.AppendLine("}");
            ReturnGUI.AppendLine("");

            SaveGUI.AppendLine("\tpublic bool SaveGUI()");
            SaveGUI.AppendLine("{");
            SaveGUI.AppendLine(ObjectName + " obj = new " + ObjectName + "();");

            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value;
                bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                // based on field type, add line
                if (isTextCol) { SaveGUI.AppendLine("\tobj." + colName + " = txt" + colName + ".Text.Trim();"); }
                if (isIntCol) { SaveGUI.AppendLine("\tobj." + colName + " = txt" + colName + ".Text.Trim();"); }
                if (isImageCol) { SaveGUI.AppendLine("\tobj." + colName + " = txt" + colName + ".Text.Trim();"); }
                if (isDateTimeCol) { SaveGUI.AppendLine("\tobj." + colName + " = txt" + colName + ".Text.Trim();"); }
                if (isCbCol) { SaveGUI.AppendLine("\tobj." + colName + " = cb" + colName + ".Checked;"); }
            }

            SaveGUI.AppendLine("bool added = " + ObjectName + "_UpdateInsert(obj);");


            SaveGUI.AppendLine("\treturn added;");
            SaveGUI.AppendLine("}");
            SaveGUI.AppendLine("");


            // assemble output lines
            string saveGUI = SaveGUI.ToString();
            string PopulateGUI = NewString.ToString() + "\n" + ReturnGUI.ToString();
            string DefineControlsSection = controlObjects.ToString();

            string returnLines = DefineControlsSection + "\n" + PopulateGUI + "\n" + saveGUI;
            return returnLines;
        }

        private string CodeGen_Return_ButtonActions(Dictionary<string, string> ColNames, string ObjectName = "objectName", string WhereField = "ID")
        {
            StringBuilder NewButton = new StringBuilder();
            StringBuilder controlObjects = new StringBuilder();
            StringBuilder ReturnGUI = new StringBuilder();
            StringBuilder SaveGUI = new StringBuilder();
            StringBuilder Populate = new StringBuilder();

            #region === New Button ===

            NewButton.AppendLine("\tprivate void btnNew_Click(object sender = null, EventArgs e = null)");
            NewButton.AppendLine("{");

            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value;
                bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                // based on field type, add line
                if (isTextCol) { NewButton.AppendLine("\tif (txt" + colName + " != null) { txt" + colName + ".Text = \"\"; } "); }
                if (isIntCol) { NewButton.AppendLine("\tif (txt" + colName + " != null) { txt" + colName + ".Text = \"\"; } "); }
                if (isImageCol) { NewButton.AppendLine("\tif (txt" + colName + " != null) { txt" + colName + ".Text = \"\"; } "); }
                if (isDateTimeCol) { NewButton.AppendLine("\tif (txt" + colName + " != null) { txt" + colName + ".Text = \"\"; } "); }
                if (isCbCol) { NewButton.AppendLine("\tif (cb" + colName + " != null) { cb" + colName + ".Checked = false; } "); }
            }

            NewButton.AppendLine("");
            NewButton.AppendLine("}");
            NewButton.AppendLine("");

            #endregion

            #region === Delete Button ===

            ReturnGUI.AppendLine("\tprivate void btnDelete_Click(object sender = null, EventArgs e = null)");
            ReturnGUI.AppendLine("{");

            ReturnGUI.AppendLine("\tstring " + WhereField + " = txt" + WhereField + ".Text;");
            ReturnGUI.AppendLine("\tbool ConfirmDelete = true;");
            ReturnGUI.AppendLine("");
            ReturnGUI.AppendLine("            bool deleted = false;");
            ReturnGUI.AppendLine("            if (ConfirmDelete)");
            ReturnGUI.AppendLine("            {");
            ReturnGUI.AppendLine("                _AHK ahk = new _AHK();");
            ReturnGUI.AppendLine("                DialogResult result = ahk.YesNoBox(\"Delete Where " + WhereField + " = \" + " + WhereField + " + \"?\", \"Delete Record?\");");
            ReturnGUI.AppendLine("                if (result.ToString().ToUpper() == \"YES\")");
            ReturnGUI.AppendLine("                {");
            ReturnGUI.AppendLine("                    deleted = " + ObjectName.ToUpper() + "." + ObjectName + "_DeleteSQL(" + WhereField + ");");
            ReturnGUI.AppendLine("                    ahk.MsgBox(\"Deleted Record = \" + deleted.ToString());");
            ReturnGUI.AppendLine("                }");
            ReturnGUI.AppendLine("            }");
            ReturnGUI.AppendLine("            else");
            ReturnGUI.AppendLine("            {");
            ReturnGUI.AppendLine("                deleted = " + ObjectName.ToUpper() + "." + ObjectName + "_DeleteSQL(" + WhereField + ");");
            ReturnGUI.AppendLine("            }");
            ReturnGUI.AppendLine("}");
            ReturnGUI.AppendLine("");

            #endregion

            #region === Save Button ===

            SaveGUI.AppendLine("\tprivate void btnSave_Click(object sender = null, EventArgs e = null)");
            SaveGUI.AppendLine("{");
            SaveGUI.AppendLine(ObjectName + " " + ObjectName.ToUpper() + " = new " + ObjectName + "();");
            SaveGUI.AppendLine("_" + ObjectName + "." + ObjectName + " obj = new _" + ObjectName + "." + ObjectName + "();");

            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value;
                bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                // based on field type, add line
                if (isTextCol) { SaveGUI.AppendLine("\tobj." + colName + " = txt" + colName + ".Text.Trim();"); }
                if (isIntCol) { SaveGUI.AppendLine("\tobj." + colName + " = txt" + colName + ".Text.Trim();"); }
                if (isImageCol) { SaveGUI.AppendLine("\tobj." + colName + " = txt" + colName + ".Text.Trim();"); }
                if (isDateTimeCol) { SaveGUI.AppendLine("\tobj." + colName + " = txt" + colName + ".Text.Trim();"); }
                if (isCbCol) { SaveGUI.AppendLine("\tobj." + colName + " = cb" + colName + ".Checked;"); }
            }

            SaveGUI.AppendLine("bool updated = " + ObjectName + "_UpdateInsert(obj);");
            SaveGUI.AppendLine("_AHK ahk = new _AHK();");
            SaveGUI.AppendLine("ahk.MsgBox(\"Updated = \" + updated.ToString());");

            SaveGUI.AppendLine("}");
            SaveGUI.AppendLine("");


            #endregion


            #region === Populate Button ===

            Populate.AppendLine("_" + ObjectName + " " + ObjectName.ToUpper() + " = new _" + ObjectName + "();");
            Populate.AppendLine("public int DisplayNum = -1;");
            Populate.AppendLine("List<_" + ObjectName + "." + ObjectName + "> List = new List<_" + ObjectName + "." + ObjectName + ">(); // stores returned list values");
            Populate.AppendLine("");
            Populate.AppendLine("\tprivate void btnPopulate_Click(object sender = null, EventArgs e = null)");
            Populate.AppendLine("{");
            Populate.AppendLine("   List = " + ObjectName.ToUpper() + "." + ObjectName + "_ReturnSQLList(\"select top(100) * from \" + " + ObjectName.ToUpper() + "." + ObjectName + "_TableName());");
            Populate.AppendLine("   if (List.Count > 0)");
            Populate.AppendLine("   {");
            Populate.AppendLine("       DisplayNum++;");
            Populate.AppendLine("       if (DisplayNum == List.Count) { DisplayNum = 0; } // result counter if max reached");
            Populate.AppendLine("       " + ObjectName.ToUpper() + ".PopulateGUI(List[DisplayNum]); // populate gui with object values");
            Populate.AppendLine("   }");
            Populate.AppendLine("");
            Populate.AppendLine("}");
            Populate.AppendLine("");


            #endregion


            // assemble output lines
            string saveGUI = SaveGUI.ToString();
            string PopulateGUI = Populate.ToString() + "\n" + ReturnGUI.ToString();

            string returnLines = NewButton + "\n" + PopulateGUI + "\n" + saveGUI;
            return returnLines;
        }

        public string CodeGen_NewForm(Dictionary<string, string> ColNames, string ObjectName = "objectName", string WhereField = "ID")
        {
            StringBuilder NewForm = new StringBuilder();
            NewForm.AppendLine("        public void DisplayForm(nuspecs obj)");
            NewForm.AppendLine("        {");
            NewForm.AppendLine("            Form aForm = new Form();");
            NewForm.AppendLine("");
            NewForm.AppendLine("            //Set up the form.");
            NewForm.AppendLine("            aForm.MaximizeBox = false;");
            NewForm.AppendLine("            aForm.MinimizeBox = false;");
            NewForm.AppendLine("            aForm.BackColor = Color.White;");
            NewForm.AppendLine("            aForm.ForeColor = Color.Black;");
            NewForm.AppendLine("            aForm.Size = new System.Drawing.Size(400, 400);");
            NewForm.AppendLine("            aForm.Text = \"Run-time Controls\";");
            NewForm.AppendLine("            aForm.FormBorderStyle = FormBorderStyle.FixedDialog;");
            NewForm.AppendLine("            aForm.StartPosition = FormStartPosition.CenterScreen;");
            NewForm.AppendLine("");
            NewForm.AppendLine("            //Format controls. Note: Controls inherit color from parent form.");
            NewForm.AppendLine("");
            NewForm.AppendLine("            //=== Populate ===");
            NewForm.AppendLine("            //btnPopulate.BackColor = Color.Gray;");
            NewForm.AppendLine("            btnPopulate.Text = \"Populate\";");
            NewForm.AppendLine("            btnPopulate.Dock = DockStyle.Bottom;");
            NewForm.AppendLine("            btnPopulate.Click += new System.EventHandler(btnPopulate_Click);");
            NewForm.AppendLine("            btnPopulate.Location = new System.Drawing.Point(25, 100);");
            NewForm.AppendLine("            btnPopulate.Size = new System.Drawing.Size(50, 25);");
            NewForm.AppendLine("");
            NewForm.AppendLine("            //=== Save ===");
            NewForm.AppendLine("            //btnSave.BackColor = Color.Gray;");
            NewForm.AppendLine("            btnSave.Text = \"Save\";");
            NewForm.AppendLine("            btnSave.Dock = DockStyle.Bottom;");
            NewForm.AppendLine("            btnSave.Click += new System.EventHandler(btnSave_Click);");
            NewForm.AppendLine("            btnSave.Location = new System.Drawing.Point(25, 100);");
            NewForm.AppendLine("            btnSave.Size = new System.Drawing.Size(50, 25);");
            NewForm.AppendLine("");
            NewForm.AppendLine("            //=== Delete ===");
            NewForm.AppendLine("            //btnDelete.BackColor = Color.Gray;");
            NewForm.AppendLine("            btnDelete.Text = \"Delete\";");
            NewForm.AppendLine("            btnDelete.Dock = DockStyle.Bottom;");
            NewForm.AppendLine("            btnDelete.Click += new System.EventHandler(btnDelete_Click);");
            NewForm.AppendLine("            btnDelete.Location = new System.Drawing.Point(25, 100);");
            NewForm.AppendLine("            btnDelete.Size = new System.Drawing.Size(50, 25);");
            NewForm.AppendLine("");
            NewForm.AppendLine("            //=== New ===");
            NewForm.AppendLine("            //btnNew.BackColor = Color.Gray;");
            NewForm.AppendLine("            btnNew.Text = \"New\";");
            NewForm.AppendLine("            btnNew.Dock = DockStyle.Bottom;");
            NewForm.AppendLine("            btnNew.Click += new System.EventHandler(btnNew_Click);");
            NewForm.AppendLine("            btnNew.Location = new System.Drawing.Point(25, 100);");
            NewForm.AppendLine("            btnNew.Size = new System.Drawing.Size(50, 25);");
            NewForm.AppendLine("");
            NewForm.AppendLine("");
            NewForm.AppendLine("");

            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value;
                bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                // based on field type, add line
                if (isTextCol || isIntCol || isImageCol || isDateTimeCol)
                {
                    NewForm.AppendLine("            //Format controls. Note: Controls inherit color from parent form.");
                    NewForm.AppendLine("            //lbldescription.BackColor = Color.Gray;");
                    NewForm.AppendLine("            lbl" + colName + ".Text = \"" + colName + "\";");
                    NewForm.AppendLine("            lbl" + colName + ".Dock = DockStyle.Top;");
                    NewForm.AppendLine("            lbl" + colName + ".Location = new System.Drawing.Point(25, 25);");
                    NewForm.AppendLine("            lbl" + colName + ".Size = new System.Drawing.Size(50, 25);");
                    NewForm.AppendLine("");
                    NewForm.AppendLine("            //Format controls. Note: Controls inherit color from parent form.");
                    NewForm.AppendLine("            //txt" + colName + ".BackColor = Color.Gray;");
                    NewForm.AppendLine("            txt" + colName + ".Text = obj." + colName + "; ");
                    NewForm.AppendLine("            txt" + colName + ".Dock = DockStyle.Top;");
                    NewForm.AppendLine("            txt" + colName + ".Location = new System.Drawing.Point(25, 50);");
                    NewForm.AppendLine("            txt" + colName + ".Size = new System.Drawing.Size(50, 25);");
                    NewForm.AppendLine("");
                }

                //if (isIntCol) { NewButton.AppendLine("\tif (txt" + colName + " != null) { txt" + colName + ".Text = \"\"; } "); }
                //if (isImageCol) { NewButton.AppendLine("\tif (txt" + colName + " != null) { txt" + colName + ".Text = \"\"; } "); }
                //if (isDateTimeCol) { NewButton.AppendLine("\tif (txt" + colName + " != null) { txt" + colName + ".Text = \"\"; } "); }
                if (isCbCol)
                {
                    NewForm.AppendLine("            //Format controls. Note: Controls inherit color from parent form.");
                    NewForm.AppendLine("            //lbldescription.BackColor = Color.Gray;");
                    NewForm.AppendLine("            lbl" + colName + ".Text = \"" + colName + "\";");
                    NewForm.AppendLine("            lbl" + colName + ".Dock = DockStyle.Top;");
                    NewForm.AppendLine("            lbl" + colName + ".Location = new System.Drawing.Point(25, 25);");
                    NewForm.AppendLine("            lbl" + colName + ".Size = new System.Drawing.Size(50, 25);");
                    NewForm.AppendLine("");
                    NewForm.AppendLine("            //Format controls. Note: Controls inherit color from parent form.");
                    NewForm.AppendLine("            //txt" + colName + ".BackColor = Color.Gray;");
                    NewForm.AppendLine("            cb" + colName + ".Checked = obj." + colName + "; ");
                    NewForm.AppendLine("            cb" + colName + ".Dock = DockStyle.Top;");
                    NewForm.AppendLine("            cb" + colName + ".Location = new System.Drawing.Point(25, 50);");
                    NewForm.AppendLine("            cb" + colName + ".Size = new System.Drawing.Size(50, 25);");
                    NewForm.AppendLine("");
                }
            }

            NewForm.AppendLine("");
            NewForm.AppendLine("            aForm.Controls.Add(btnNew);");
            NewForm.AppendLine("            aForm.Controls.Add(btnPopulate);");
            NewForm.AppendLine("            aForm.Controls.Add(btnSave);");
            NewForm.AppendLine("            aForm.Controls.Add(btnDelete);");
            NewForm.AppendLine("");
            NewForm.AppendLine("");


            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value;
                bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                // based on field type, add line
                if (isTextCol || isIntCol || isImageCol || isDateTimeCol)
                {
                    NewForm.AppendLine("            aForm.Controls.Add(txt" + colName + ");");
                    NewForm.AppendLine("            aForm.Controls.Add(lbl" + colName + ");");
                }

                //if (isIntCol) { NewButton.AppendLine("\tif (txt" + colName + " != null) { txt" + colName + ".Text = \"\"; } "); }
                //if (isImageCol) { NewButton.AppendLine("\tif (txt" + colName + " != null) { txt" + colName + ".Text = \"\"; } "); }
                //if (isDateTimeCol) { NewButton.AppendLine("\tif (txt" + colName + " != null) { txt" + colName + ".Text = \"\"; } "); }
                if (isCbCol)
                {
                    NewForm.AppendLine("            aForm.Controls.Add(cb" + colName + ");");
                    NewForm.AppendLine("            aForm.Controls.Add(lbl" + colName + ");");
                }
            }



            NewForm.AppendLine("            aForm.Controls.Add(new RadLabel() { Text = \"Version 5.0\" });");
            NewForm.AppendLine("            aForm.ShowDialog();  // Or just use Show(); if you don't want it to be modal.");
            NewForm.AppendLine("        }");


            return NewForm.ToString();
        }


        // returns new object from user values
        private string CodeGen_Return_ObjectPopulate(Dictionary<string, string> ColNames, string ObjectName = "objectName")
        {
            string ObjectFunction = "\tpublic " + ObjectName + " Return_" + ObjectName + "(";

            string objectInput = "";
            foreach (string col in ColNames.Keys)
            {
                if (objectInput != "") { objectInput = objectInput + ", " + "string " + col + " = \"\""; }
                if (objectInput == "") { objectInput = "string " + col + " = \"\""; }
            }


            ObjectFunction = ObjectFunction + objectInput + ")\n\t{\t\t";

            // assign object values
            string robject = "\n" + ObjectName + " obj = new " + ObjectName + "();";
            foreach (string coll in ColNames.Keys) { robject = robject + "\n\t\tobj." + coll + " = " + coll + ";"; }

            ObjectFunction = ObjectFunction + robject + "\n\n\t\treturn obj;\n\t}\n";

            return ObjectFunction;
        }

        // returns a function to fix illegal characters before inserting into sql/sqlite
        private string CodeGen_ObjectFixChars(Dictionary<string, string> ColNames, string ObjectName = "objectName")
        {
            StringBuilder NewString = new StringBuilder();

            NewString.AppendLine("//  Fix illegal characters before Sql/Sqlite Db Inserts");
            NewString.AppendLine("\tpublic " + ObjectName + " " + ObjectName + "_FixChars(" + ObjectName + " ToFix)");
            NewString.AppendLine("{");

            NewString.AppendLine("\t" + ObjectName + " Fixed = new " + ObjectName + "();");


            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value; bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                // based on field type, add line
                if (isTextCol) { NewString.AppendLine("\tFixed." + colName + " = ToFix." + colName + ".Replace(\"'\", \"''\");"); }
                //if (isIntCol) { NewString.AppendLine("\t\t\tpublic int " + colName + " { get; set; }"); }
                //if (isImageCol) { NewString.AppendLine("\t\t\tpublic string " + colName + " { get; set; }"); }
                //if (isDateTimeCol) { NewString.AppendLine("\t\t\tpublic DateTime " + colName + " { get; set; }"); }
                //if (isCbCol) { NewString.AppendLine("\t\t\tpublic bool " + colName + " { get; set; }"); }
            }


            NewString.AppendLine("");
            NewString.AppendLine("\treturn Fixed;");
            NewString.AppendLine("}");
            NewString.AppendLine("");

            return NewString.ToString();
        }

        // returns list of column names with brackets and commas in list
        private string CodeGen_ColNames(Dictionary<string, string> ColNames)
        {
            string colnames = "";
            int i = 0;
            foreach (string col in ColNames.Keys)
            {
                //if (i == 0) { i++; continue; } // skip the parent node
                if (colnames != "") { colnames = colnames + ", [" + col + "]"; }
                if (colnames == "") { colnames = "[" + col + "]"; }
            }
            return colnames;
        }

        private string CodeGen_Select(Dictionary<string, string> ColNames, string objectName = "objectName", string tableName = "TableName", string WHEREClause = "")
        {
            string colNames = CodeGen_ColNames(ColNames);

            string SelectLine = "string SelectLine = \"Select " + colNames + " From [" + tableName + "]";
            string WhereClause = "";

            //if (WhereClause != "") { WhereClause = WhereClause + " AND " + node.Text + " = '\" + " + node.Text + " + \"'"; }
            //if (WhereClause == "") { WhereClause = " WHERE " + node.Text + " = '\" + " + node.Text + " + \"'"; }

            if (WhereClause != "") { SelectLine = SelectLine + WhereClause; }
            SelectLine = SelectLine + "\";";

            string Function = "";

            string WhereVarIn = "";
            if (WhereVarIn == "") { Function = "\tpublic DataTable Return_DataTable_From_" + tableName + "(string DbFile)" + "\n\t{\n"; }
            if (WhereVarIn != "") { Function = "\tpublic DataTable Return_DataTable_From_" + tableName + "(string DbFile, " + WhereVarIn + ")" + "\n\t{\n"; }


            string LineCommand = "\t\tDataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);";
            SelectLine = "\t\t" + SelectLine + "\n\t\t" + LineCommand;
            Function = Function + SelectLine + "\n\t\treturn ReturnTable;\n\t}\n";

            return Function;
        }

        // return single object from db search using where param
        private string CodeGen_Object(Dictionary<string, string> ColNames, string objectName = "objectName", string tableName = "TableName", string WHEREClause = "")
        {
            string Function = "";

            // pull first key from dictionary
            string firstObj = "";
            foreach (KeyValuePair<string, string> pair in ColNames) { firstObj = pair.Key; break; }

            //string firstObj = lst.First_List_Item(ColNames.Values);
            string SelectLine = "\t\tstring SelectLine = \"Select " + CodeGen_ColNames(ColNames) + " From [" + tableName + "] \";\n\t\tDataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);";

            string WhereAdd = "\tif (WhereClause.ToUpper().Contains(\"WHERE \")) { SelectLine = SelectLine + \" \" + WhereClause; }";
            WhereAdd = WhereAdd + "\n\tif (!WhereClause.ToUpper().Contains(\"WHERE \")) { SelectLine = SelectLine + \"WHERE \" + WhereClause; }";

            Function = "\tpublic " + objectName + " Return_Object_From_" + tableName + "(string WhereClause = \"[" + firstObj + "] = ''\", string DbFile=\"\")" + "\n\t{\nif (DbFile == \"\") { DbFile = ahk.AppDir() + @\"\\Db\\" + objectName + ".sqlite\"; }\n\t";
            //if (WhereVarIn != "") { Function = "\tpublic " + objectName + " Return_Object_From_" + tableName + "(string DbFile, string WhereClause, " + WhereVarIn + ")" + "\n\t{\n"; }

            //LineCommand = "DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine, Debug);";
            //SelectLine = "\t\t" + SelectLine + "\n\t\t" + LineCommand;
            Function = Function + SelectLine + "\n" + WhereAdd;

            // gather all the selected column names to use as object names to populate
            string ParseReturn = "\t\t" + objectName + " returnObject = new " + objectName + "();\n\t\tint i = 0;\n\t\tstring Value = \"\";\n\t\tif (ReturnTable != null)\n\t\t\t{\n\t\t\tforeach (DataRow ret in ReturnTable.Rows)\n\t\t\t\t{\n";
            string Objects = "";  // list of vars used as input in functions below

            //int i = 0;
            //foreach (string col in ColNames.Keys)
            //{
            //    //if (i == 0) { i++; continue; } // skip the parent node
            //    if (Objects != "") { Objects = Objects + "\n\t\t\t\treturnObject." + col + " = ret[\"" + col + "\"].ToString();"; } if (Objects == "") { Objects = "\t\t\treturnObject." + col + " = ret[\"" + col + "\"].ToString();"; }
            //}

            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value; bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                // based on field type, add line
                if (isTextCol) { if (Objects != "") { Objects = Objects + "\n\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToString();"; } if (Objects == "") { Objects = "\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToString();"; } }
                if (isIntCol) { if (Objects != "") { Objects = Objects + "\n\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToInt();"; } if (Objects == "") { Objects = "\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToInt();"; } }
                if (isImageCol) { if (Objects != "") { Objects = Objects + "\n\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToString();"; } if (Objects == "") { Objects = "\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToString();"; } }
                if (isDateTimeCol) { if (Objects != "") { Objects = Objects + "\n\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToDateTime();"; } if (Objects == "") { Objects = "\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToDateTime();"; } }
                if (isCbCol) { if (Objects != "") { Objects = Objects + "\n\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToBool();"; } if (Objects == "") { Objects = "\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToBool();"; } }
            }


            ParseReturn = ParseReturn + Objects + "\n\t\t\t\t}\n\t\t}\n";
            Function = Function + ParseReturn + "\n\t\treturn returnObject;\n\t}\n";

            return Function;
        }

        // sql search - returns list of objects as results
        private string CodeGen_ObjectList(Dictionary<string, string> ColNames, string tableName = "[TableName]", string objectName = "objectName")
        {
            string Function = "";

            string TableNAME = tableName;
            if (!TableNAME.Contains("[")) { TableNAME = "[" + tableName.Trim() + "]"; }

            string SelectLine = "\t\tstring SelectLine = \"Select * From \" + TableName;\n";
            //string SelectLine = "\t\tstring SelectLine = \"Select * From " + TableNAME + "\";\n"; // +"\t\tDataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);";

            string WhereClause = "\n\t\tif (WhereClause != \"\")\n\t\t{\n\t\t\tif (WhereClause.ToUpper().Contains(\"WHERE\")) { SelectLine = SelectLine + \" \" + WhereClause; }";
            WhereClause = WhereClause + "\n\t\t\tif (!WhereClause.ToUpper().Contains(\"WHERE\")) { SelectLine = SelectLine + \" WHERE \" + WhereClause; }\n\t\t}\n";
            WhereClause = WhereClause + "\t\t\tDataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);";

            Function = "\tpublic List<" + objectName + "> Return_" + objectName + "_List(string WhereClause = \"\", string DbFile=\"\", string TableName = \"" + TableNAME + "\")" + "\n\t{\n\tif (DbFile == \"\") { DbFile = ahk.AppDir() + @\"\\Db\\" + objectName + ".sqlite\"; }\n\t";
            Function = Function + SelectLine + WhereClause + "\n\n\t\t\tList<" + objectName + "> ReturnList = new List<" + objectName + ">();\n";

            // gather all the selected column names to use as object names to populate
            string ParseReturn = "\t\tif (ReturnTable != null)\n\t\t\t{\n\t\t\tforeach (DataRow ret in ReturnTable.Rows)\n\t\t\t\t{\n\t\t\t\t" + objectName + " returnObject = new " + objectName + "();\n\n";
            string Objects = "";  // list of vars used as input in functions below

            //int i = 0;
            //foreach (string col in ColNames.Keys)
            //{
            //    //if (i == 0) { i++; continue; } // skip the parent node
            //    if (Objects != "") { Objects = Objects + "\n\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToString();"; } if (Objects == "") { Objects = "\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToString();"; }
            //}

            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value; bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                // based on field type, add line
                if (isTextCol) { if (Objects != "") { Objects = Objects + "\n\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToString();"; } if (Objects == "") { Objects = "\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToString();"; } }
                if (isIntCol) { if (Objects != "") { Objects = Objects + "\n\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToInt();"; } if (Objects == "") { Objects = "\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToInt();"; } }
                if (isImageCol) { if (Objects != "") { Objects = Objects + "\n\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToString();"; } if (Objects == "") { Objects = "\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToString();"; } }
                if (isDateTimeCol) { if (Objects != "") { Objects = Objects + "\n\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToDateTime();"; } if (Objects == "") { Objects = "\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToDateTime();"; } }
                if (isCbCol) { if (Objects != "") { Objects = Objects + "\n\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToBool();"; } if (Objects == "") { Objects = "\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToBool();"; } }
            }



            ParseReturn = ParseReturn + Objects + "\n\n\t\t\t\tReturnList.Add(returnObject);" + "\n\t\t\t}\n\t\t}\n";
            Function = Function + ParseReturn + "\n\t\treturn ReturnList;\n\t}\n";

            return Function;
        }

        private string CodeGen_Insert(Dictionary<string, string> ColNames, string tableName = "TableName", string objectName = "oBject")
        {
            //"insert into filelist (filepath, score) values ('" + file + "', 9001)", db);  // insert into a Table
            string InsertLine = "Insert Into [" + tableName + "] ";
            string InsertLineObj = "Insert Into [" + tableName + "] ";

            string Cols = "";
            int i = 0;
            foreach (string col in ColNames.Keys)
            {
                //if (i == 0) { i++; continue; } // skip the parent node
                if (Cols != "") { Cols = Cols + ", " + col; }
                if (Cols == "") { Cols = col; }
            }

            InsertLine = InsertLine + "(" + Cols + ") values (";
            InsertLineObj = InsertLineObj + "(" + Cols + ") values (";

            string VarNames = "";
            string VarNamesObj = "";
            i = 0;
            foreach (string col in ColNames.Keys)
            {
                //if (i == 0) { i++; continue; } // skip the parent node
                if (VarNames != "") { VarNames = VarNames + ", " + "'\" + " + col + " + \"'"; }
                if (VarNames == "") { VarNames = "'\" + " + col + " + \"'"; }

                if (VarNamesObj != "") { VarNamesObj = VarNamesObj + ", " + "'\" + inObject." + col + " + \"'"; }
                if (VarNamesObj == "") { VarNamesObj = "'\" + inObject." + col + " + \"'"; }
            }

            InsertLine = InsertLine + VarNames + ")";
            InsertLineObj = InsertLineObj + VarNamesObj + ")";

            string WhereClause = "";
            string WhereClauseObj = "";

            // add where clause condition

            i = 0;
            foreach (string col in ColNames.Keys)
            {

                if (i == 0)
                {
                    if (WhereClause == "") { WhereClause = " WHERE " + col + " = '\" + " + col + " + \"'"; }
                    if (WhereClauseObj == "") { WhereClauseObj = " WHERE " + col + " = '\" + inObject." + col + " + \"'"; }
                    break;
                }

                i++;
            }

            InsertLine = "string InsertLine = \"" + InsertLine + "\";";
            InsertLineObj = "string InsertLine = \"" + InsertLineObj + "\";";


            string Function = "\tpublic bool Insert_Into_" + tableName + "(string DbFile, "; // +FunctionVarIn + ")" + "\n\t{\n";
            string FunctionObj = "\tpublic bool " + objectName + "_Insert(" + objectName + " inObject, string DbFile=\"\")" + "\n\t{\n\tif (DbFile == \"\") { DbFile = ahk.AppDir() + @\"\\Db\\" + objectName + ".sqlite\"; }\n\t";
            //if (!DisplayText.Contains("string DbFile =")) { Function = DbFileLine + "\n\n" + Function; }

            string LineCommand = "bool Inserted = sqlite.Execute(DbFile, InsertLine);";

            InsertLine = "\t\t" + InsertLine + "\n\t\t" + LineCommand;
            InsertLine = InsertLine + "\n\t\t" + "if(!Inserted) { ahk.MsgBox(\"Inserted Into [" + tableName + "] = \" + Inserted.ToString()); }";
            Function = Function + InsertLine + "\n\t\treturn Inserted;\n\t}\n";

            InsertLineObj = "\t\t" + InsertLineObj + "\n\t\t" + LineCommand;
            InsertLineObj = InsertLineObj + "\n\t\t" + "if(!Inserted) { ahk.MsgBox(\"Inserted Into [" + tableName + "] = \" + Inserted.ToString()); }";
            FunctionObj = FunctionObj + InsertLineObj + "\n\t\treturn Inserted;\n\t}\n";

            return FunctionObj;
        }

        private string CodeGen_Update(Dictionary<string, string> ColNames, string tableName = "TableName", string objectName = "oBject")
        {
            // add code to execute this sqlite command - generate new function code
            //string Function = "\t\tpublic bool Update_" + tableName + "(string DbFile, string Var)" + "\n\t{\n";
            //string UpdateFromObject = "\tpublic bool " + objectName + "_Update(string DbFile, " + objectName + " inObject, string WHERE = \"[Item] = 'Value'\")" + "\n\t{\n";
            string UpdateFromObject = "\tpublic bool " + objectName + "_Update(" + objectName + " inObject, string DbFile = \"\")" + "\n{\n\t";

            //string UpdateLine = "Update [" + tableName + "] set ";
            //string Up = "";
            string UpOb = "";
            //string UpLine = "Update [" + tableName + "] set ";
            //string UpCondits = "";
            string UpConditsOb = "\n";
            string First = "";

            //string UpdateFields = ""; 
            int i = 0;
            foreach (string col in ColNames.Keys)
            {
                //if (i == 0) { i++; continue; } // skip the parent node

                if (First == "") { First = col; }
                //if (Up != "") { Up = Up + ", " + col + " = '\" + " + col + " + \"'"; }
                //if (Up == "") { Up = col + " = '\" + " + col + " + \"'"; }

                if (UpOb != "") { UpOb = UpOb + ", " + col + " = '\" + inObject." + col + " + \"'"; }
                if (UpOb == "") { UpOb = col + " = '\" + inObject." + col + " + \"'"; }

                UpConditsOb = UpConditsOb + "\n\t\t\tif ( inObject." + col + " != null) { UpdateLine = UpdateLine + \"[" + col + "] = '\" + inObject." + col + " + \"',\"; }";
                //UpCondits = UpCondits + "\n\t\t\tif (" + col + " != \"\") { UpdateLine = UpdateLine + \"[" + col + "] = '\" + " + col + " + \"',\"; }";
            }


            string UpdateFieldLine = "";
            i = 0;
            string firstCol = "";
            foreach (string col in ColNames.Keys)
            {
                if (i == 0) { firstCol = col; i++; }
                if (UpdateFieldLine != "") { UpdateFieldLine = UpdateFieldLine + ", " + col + " = '\" + inObject." + col + " + \"'"; }
                if (UpdateFieldLine == "") { UpdateFieldLine = col + " = '\" + inObject." + col + " + \"'"; }
            }


            string UpdateLineObject = "Update [" + tableName + "] set " + UpdateFieldLine + " WHERE [Item] = 'Value' ";
            //UpdateLine = UpdateLine + Up;


            //UpCondits = "string UpdateLine = \"Update [" + tableName + "] set \";\n\n" + UpCondits;
            UpConditsOb = "string UpdateLine = \"Update [" + tableName + "] set \";\n" + UpConditsOb;

            //string WhereClause = "";
            //string WhereClauseObj = "";

            //i = 0;
            //foreach (string col in ColNames)
            //{
            //    //if (i == 0) { i++; continue; } // skip the parent node

            //    if (WhereClause != "") { WhereClause = WhereClause + " AND " + col + " = '\" + " + col + " + \"'"; }
            //    if (WhereClause == "") { WhereClause = " WHERE " + col + " = '\" + " + col + " + \"'"; }

            //    if (WhereClauseObj != "") { WhereClauseObj = WhereClauseObj + " AND " + col + " = '\" + inObject." + col + " + \"'"; }
            //    if (WhereClauseObj == "") { WhereClauseObj = " WHERE " + col + " = '\" + inObject." + col + " + \"'"; }
            //}


            //if (WhereClause != "") { UpdateLine = UpdateLine + WhereClause; }
            //if (WhereClauseObj != "") { UpdateLineObject = UpdateLineObject + WhereClauseObj; }

            //UpdateLine = "string UpdateLine = \"" + UpdateLine + "\";";
            UpdateLineObject = "string UpdateLine = \"" + UpdateLineObject + "\";";

            //UpdateLine = UpCondits;
            UpdateLineObject = "\t\t//" + UpdateLineObject + "\n\t\t" + UpConditsOb + "\n\n\t\t\tUpdateLine = ahk.TrimLast(UpdateLine, 1);\n";
            UpdateLineObject = UpdateLineObject + "\t\tUpdateLine = UpdateLine + \" WHERE [" + firstCol + "] = ' '\"; // DEFINE CONDITION HERE !!!\n";


            //if (!DisplayText.Contains("string DbFile =")) { Function = DbFileLine + "\n\n" + Function; }
            string LineCommand = "bool Updated = sqlite.Execute(DbFile, UpdateLine);";

            //UpdateLine = "\t\t" + UpdateLine + "\n\t\t" + LineCommand;
            //UpdateLine = UpdateLine + "\n\t\t" + "if(!Updated) { ahk.MsgBox(\"Updated [" + tableName + "] = \" + Updated.ToString()); }";
            //Function = Function + UpdateLine + "\n\t\treturn Updated;\n\t}\n";

            UpdateFromObject = UpdateFromObject + "\t" + UpdateLineObject + "\n\t\t" + LineCommand + "\n\t\treturn Updated;\n\t}\n";
            //UpdateLineObject = UpdateLineObject + "\n\t\t" + "if(!Updated) { ahk.MsgBox(\"Updated [" + tableName + "] = \" + Updated.ToString()); }";
            //UpdateFromObject = UpdateFromObject + UpCondits + "\n\t\treturn Updated;\n\t}\n";

            return UpdateFromObject;
        }

        // update or insert new record to sqlite
        private string CodeGen_UpdateInsert(Dictionary<string, string> ColNames, string tableName = "TableName", string objectName = "oBject")
        {
            //string ReturnString = InsertCode + "\n\r\n\r" + UpdateCode;

            //string UpdateInsertLine = "\tpublic bool " + objectName + "_UpdateInsert(string DbFile, " + objectName + " obj, string WhereClause = \"\")\r\n";
            string UpdateInsertLine = "\tpublic bool " + objectName + "_UpdateInsert(" + objectName + " obj, string DbFile=\"\")\n{\n";
            //UpdateInsertLine = UpdateInsertLine + "{\n\tbool Updated = " + objectName + "_Update(DbFile, obj, WhereClause);  // try to update record first\r\n";
            UpdateInsertLine = UpdateInsertLine + "\nif (DbFile == \"\") { DbFile = ahk.AppDir() + @\"\\Db\\" + objectName + ".sqlite\"; }";
            UpdateInsertLine = UpdateInsertLine + "\nif (!File.Exists(DbFile)) { Create_Table_" + tableName + "(DbFile); }\n";

            UpdateInsertLine = UpdateInsertLine + "\n\tbool Updated = " + objectName + "_Update(obj, DbFile);  // try to update record first\r\n";
            UpdateInsertLine = UpdateInsertLine + "\tif (!Updated) { Updated = " + objectName + "_Insert(obj, DbFile); }  // if unable to update, insert new record\r\n";
            UpdateInsertLine = UpdateInsertLine + "\treturn Updated;\n}\n\r";

            return UpdateInsertLine;
        }







        // GUI : Save / Display / Update

        private string CodeGen_GUISaveUpdate(Dictionary<string, string> ColNames, string tableName = "TableName", string objectName = "obj")
        {
            string UpdateFromObject = "\tpublic bool Display_" + objectName + "(" + objectName + " inObject)" + "\n{\n\t";

            string UpOb = "";
            string First = "";

            int i = 0;
            foreach (string col in ColNames.Keys)
            {
                if (First == "") { First = col; }
                if (UpOb != "") { UpOb = UpOb + "\n" + "tel.Update( txt" + col + ", " + objectName + "." + col + ");"; }
                if (UpOb == "") { UpOb = "tel.Update( txt" + col + ", " + objectName + "." + col + ");"; }
            }

            string lines = UpdateFromObject + "\n\n" + UpOb + "\n\t}\n";

            return lines;
        }

        private string CodeGen_StaticGUI(Dictionary<string, string> ColNames, string tableName = "TableName", string objectName = "obj")
        {
            string UP = "";
            string First = "_TelerikLib tel = new _TelerikLib();\n_Database.SQLite sqlite = new _Database.SQLite();\n_Database.SQLite sqlite = new _Database.SQLite();";

            int i = 0;
            foreach (string col in ColNames.Keys)
            {
                if (UP != "") { UP = UP + "public static RadTextBox " + col + " { get; set; }\n"; }
                if (UP == "") { UP = "public static RadTextBox " + col + " { get; set; }\n"; }
            }

            return First + "\n\n" + UP;
        }

        private string CodeGen_DisplayObjectGUI(Dictionary<string, string> ColNames, string tableName = "TableName", string objectName = "obj")
        {
            string UP = "\tpublic bool Display_" + objectName + "(" + objectName + " inObject)" + "\n{\n\t";
            string First = "_TelerikLib tel = new _TelerikLib();\n_Database.SQLite sqlite = new _Database.SQLite();\n_Database.SQLite sqlite = new _Database.SQLite();";

            int i = 0;
            foreach (string col in ColNames.Keys)
            {
                if (UP != "") { UP = UP + "public static RadTextBox " + col + " { get; set; }\n"; }
                if (UP == "") { UP = "public static RadTextBox " + col + " { get; set; }\n"; }
            }

            return First + "\n\n" + UP;
        }






        


    }
}
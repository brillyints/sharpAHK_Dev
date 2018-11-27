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

        // === CodeGen SQL ===

        private string CodeGen_SQL_Update(Dictionary<string, string> ColNames, string tableName = "[LucidMedia].[dbo].[FileIndex]", string objectName = "oBject", string whereField = "ID", string ConnName = "Conn")  // Update Code
        {
            StringBuilder NewString = new StringBuilder();
            NewString.AppendLine("\tpublic bool " + objectName + "_UpdateSQL(" + objectName + " obj)");
            NewString.AppendLine("{");
            NewString.AppendLine("\tSqlConnection Conn = " + objectName + "_Conn();");

            string colNames = "";
            foreach (string col in ColNames.Keys)
            {
                if (colNames != "") { colNames = colNames + ", " + col + " = @" + col; }
                if (colNames == "") { colNames = col + " = @" + col; }
            }
            NewString.AppendLine("\tstring SQLLine = \"Update \" + " + objectName + "_TableName() + \" SET " + colNames + " WHERE " + whereField + " = @" + whereField + "\";");
            NewString.AppendLine("\tSqlCommand cmd2 = new SqlCommand(SQLLine, Conn);");
            NewString.AppendLine("\tcmd2 = new SqlCommand(SQLLine, Conn);");
            NewString.AppendLine("");

            //foreach (string col in ColNames.Keys)
            //{
            //    NewString.AppendLine("\tif (obj." + col + " == null) { obj." + col + " = \"\"; }");
            //}

            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value; bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                if (colName == "ID") { continue; }

                // based on field type, add line
                if (isTextCol) { NewString.AppendLine("\tif (obj." + colName + " == null) { obj." + colName + " = \"\"; }"); }
                //if (isIntCol) { NewString.AppendLine("\tif (obj." + colName + " == null) { obj." + colName + " = \"\"; }"); }
                //if (isImageCol) { NewString.AppendLine("\tif (obj." + colName + " == null) { obj." + colName + " = \"\"; }"); }
                //if (isDateTimeCol) { NewString.AppendLine("\tif (obj." + colName + " == null) { obj." + colName + " = \"\"; }"); }
                //if (isCbCol) { NewString.AppendLine("\tif (obj." + colName + " == null) { obj." + colName + " = \"\"; }"); }
            }



            foreach (string col in ColNames.Keys)
            {
                NewString.AppendLine("\tcmd2.Parameters.AddWithValue(@\"" + col + "\", obj." + col + ".ToString());");
            }

            NewString.AppendLine("");
            NewString.AppendLine("\tif (Conn.State == ConnectionState.Closed) { Conn.Open(); }");
            NewString.AppendLine("\tint recordsAffected = 0;");
            NewString.AppendLine("\t_AHK ahk = new _AHK();");
            NewString.AppendLine("\ttry { recordsAffected = cmd2.ExecuteNonQuery(); }");

            // SQL Error Catch / Retry
            NewString.AppendLine("\tcatch (SqlException ex)");
            NewString.AppendLine("{");
            //NewString.AppendLine("\t_AHK ahk = new _AHK();");
            //NewString.AppendLine("\tahk.MsgBox(ex.ToString());");
            NewString.AppendLine("if (ex.Number == 1205) // Retry on DeadLock");
            NewString.AppendLine("{");
            NewString.AppendLine("    ahk.Sleep(1000);");
            NewString.AppendLine("    " + objectName + "_UpdateSQL(obj);");
            NewString.AppendLine("}");
            NewString.AppendLine("else if (ex.Message.ToUpper().Contains(\"TIMEOUT EXPIRED\")) // Retry on Standard TimeOut");
            NewString.AppendLine("{");
            NewString.AppendLine("    ahk.Sleep(1000);");
            NewString.AppendLine("    " + objectName + "_UpdateSQL(obj);");
            NewString.AppendLine("}");
            NewString.AppendLine("else");
            NewString.AppendLine("{");
            NewString.AppendLine("    ahk.MsgBox(ex.ToString());");
            NewString.AppendLine("    return false;");
            NewString.AppendLine("}");
            NewString.AppendLine("}");

            NewString.AppendLine("\tConn.Close();");
            NewString.AppendLine("\tif (recordsAffected > 0) { return true; }");
            NewString.AppendLine("\telse return false;");
            NewString.AppendLine("}");

            return NewString.ToString();
        }

        private string CodeGen_SQL_Insert(Dictionary<string, string> ColNames, string tableName = "[LucidMedia].[dbo].[FileIndex]", string objectName = "oBject", string ConnName = "Con")
        {
            StringBuilder NewString = new StringBuilder();
            NewString.AppendLine("\tpublic bool " + objectName + "_InsertSQL(" + objectName + " obj)");
            NewString.AppendLine("{");
            NewString.AppendLine("\tSqlConnection Con = " + objectName + "_Conn();");

            string InsertNames = "";
            string InsertNameVars = "";
            //int i = 0;
            //foreach (string col in ColNames.Keys)
            //{
            //    if (col.ToUpper() == "ID") { continue; }

            //    if (InsertNames != "") { InsertNames = InsertNames + ", " + colName; } if (InsertNames == "") { InsertNames = colName; } if (InsertNameVars != "") { InsertNameVars = InsertNameVars + ", @" + colName; } if (InsertNameVars == "") { InsertNameVars = "@" + colName; }
            //}


            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value; bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                if (colName == "ID") { continue; }

                // based on field type, add line
                if (isTextCol) { if (InsertNames != "") { InsertNames = InsertNames + ", " + colName; } if (InsertNames == "") { InsertNames = colName; } if (InsertNameVars != "") { InsertNameVars = InsertNameVars + ", @" + colName; } if (InsertNameVars == "") { InsertNameVars = "@" + colName; } }
                if (isIntCol) { if (InsertNames != "") { InsertNames = InsertNames + ", " + colName; } if (InsertNames == "") { InsertNames = colName; } if (InsertNameVars != "") { InsertNameVars = InsertNameVars + ", @" + colName; } if (InsertNameVars == "") { InsertNameVars = "@" + colName; } }
                if (isImageCol) { if (InsertNames != "") { InsertNames = InsertNames + ", " + colName; } if (InsertNames == "") { InsertNames = colName; } if (InsertNameVars != "") { InsertNameVars = InsertNameVars + ", @" + colName; } if (InsertNameVars == "") { InsertNameVars = "@" + colName; } }
                if (isDateTimeCol) { if (InsertNames != "") { InsertNames = InsertNames + ", " + colName; } if (InsertNames == "") { InsertNames = colName; } if (InsertNameVars != "") { InsertNameVars = InsertNameVars + ", @" + colName; } if (InsertNameVars == "") { InsertNameVars = "@" + colName; } }
                if (isCbCol) { if (InsertNames != "") { InsertNames = InsertNames + ", " + colName; } if (InsertNames == "") { InsertNames = colName; } if (InsertNameVars != "") { InsertNameVars = InsertNameVars + ", @" + colName; } if (InsertNameVars == "") { InsertNameVars = "@" + colName; } }
            }



            NewString.AppendLine("\tstring SQLLine = \"Insert Into \" + " + objectName + "_TableName() + \" (" + InsertNames + ") VALUES (" + InsertNameVars + ")\";");
            NewString.AppendLine("\tSqlCommand cmd2 = new SqlCommand(SQLLine, Con);");
            NewString.AppendLine("\tcmd2 = new SqlCommand(SQLLine, Con);");
            NewString.AppendLine("");

            //foreach (string col in ColNames.Keys)
            //{
            //    if (col.ToUpper() == "ID") { continue; }
            //    NewString.AppendLine("\tif (obj." + colName + " == null) { obj." + colName + " = \"\"; }");
            //}


            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value; bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                if (colName == "ID") { continue; }

                // based on field type, add line
                if (isTextCol) { NewString.AppendLine("\tif (obj." + colName + " == null) { obj." + colName + " = \"\"; }"); }
                //if (isIntCol) { NewString.AppendLine("\tif (obj." + colName + " == null) { obj." + colName + " = \"\"; }"); }
                //if (isImageCol) { NewString.AppendLine("\tif (obj." + colName + " == null) { obj." + colName + " = \"\"; }"); }
                //if (isDateTimeCol) { NewString.AppendLine("\tif (obj." + colName + " == null) { obj." + colName + " = \"\"; }"); }
                //if (isCbCol) { NewString.AppendLine("\tif (obj." + colName + " == null) { obj." + colName + " = \"\"; }"); }
            }




            //foreach (string col in ColNames.Keys)
            //{
            //    if (col.ToUpper() == "ID") { continue; }
            //    NewString.AppendLine("\tcmd2.Parameters.AddWithValue(@\"" + colName + "\", obj." + colName + ".ToString());");
            //}


            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value; bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                if (colName == "ID") { continue; }

                // based on field type, add line
                if (isTextCol) { NewString.AppendLine("\tcmd2.Parameters.AddWithValue(@\"" + colName + "\", obj." + colName + ".ToString());"); }
                if (isIntCol) { NewString.AppendLine("\tcmd2.Parameters.AddWithValue(@\"" + colName + "\", obj." + colName + ".ToString());"); }
                if (isImageCol) { NewString.AppendLine("\tcmd2.Parameters.AddWithValue(@\"" + colName + "\", obj." + colName + ".ToString());"); }
                if (isDateTimeCol) { NewString.AppendLine("\tcmd2.Parameters.AddWithValue(@\"" + colName + "\", obj." + colName + ".ToString());"); }
                if (isCbCol) { NewString.AppendLine("\tcmd2.Parameters.AddWithValue(@\"" + colName + "\", obj." + colName + ".ToString());"); }
            }


            NewString.AppendLine("");
            NewString.AppendLine("\tif (Con.State == ConnectionState.Closed) { Con.Open(); }");
            NewString.AppendLine("\tint recordsAffected = 0;");
            NewString.AppendLine("\t_AHK ahk = new _AHK();");
            NewString.AppendLine("\ttry { recordsAffected = cmd2.ExecuteNonQuery(); }");

            // SQL Error Catch / Retry
            NewString.AppendLine("\tcatch (SqlException ex)");
            NewString.AppendLine("{");
            NewString.AppendLine("if (ex.Number == 1205) // Retry on DeadLock");
            NewString.AppendLine("{");
            NewString.AppendLine("    ahk.Sleep(1000);");
            NewString.AppendLine("    " + objectName + "_InsertSQL(obj);");
            NewString.AppendLine("}");
            NewString.AppendLine("else if (ex.Message.ToUpper().Contains(\"TIMEOUT EXPIRED\")) // Retry on Standard TimeOut");
            NewString.AppendLine("{");
            NewString.AppendLine("    ahk.Sleep(1000);");
            NewString.AppendLine("    " + objectName + "_InsertSQL(obj);");
            NewString.AppendLine("}");
            NewString.AppendLine("else");
            NewString.AppendLine("{");
            NewString.AppendLine("\tahk.MsgBox(ex.ToString());");
            NewString.AppendLine("    return false;");
            NewString.AppendLine("}");
            NewString.AppendLine("}");

            NewString.AppendLine("\tCon.Close();");
            NewString.AppendLine("\tif (recordsAffected > 0) { return true; }");
            NewString.AppendLine("\telse return false;");
            NewString.AppendLine("}");

            return NewString.ToString();
        }

        private string CodeGen_SQL_UpdateInsert(Dictionary<string, string> ColNames, string tableName = "[LucidMedia].[dbo].[FileIndex]", string objectName = "oBject")
        {
            StringBuilder NewString = new StringBuilder();

            NewString.AppendLine("\tpublic bool " + objectName + "_UpdateInsert(" + objectName + " obj)");
            NewString.AppendLine("{");
            //NewString.AppendLine("\tSqlConnection Conn = " + objectName + "_Conn();");
            NewString.AppendLine("\tbool Updated = " + objectName + "_UpdateSQL(obj);  // try to update record first");
            NewString.AppendLine("\tif (!Updated) { Updated = " + objectName + "_InsertSQL(obj); }  // if unable to update, insert new record");
            NewString.AppendLine("\treturn Updated;");
            NewString.AppendLine("}");

            return NewString.ToString();
        }

        private string CodeGen_SQL_Delete(Dictionary<string, string> ColNames, string tableName = "[LucidMedia].[dbo].[FileIndex]", string objectName = "oBject", string whereField = "ID", string ConnName = "Conn")  // Update Code
        {
            StringBuilder NewString = new StringBuilder();
            NewString.AppendLine("\tpublic bool " + objectName + "_DeleteSQL(string " + whereField + ")");
            NewString.AppendLine("{");
            NewString.AppendLine("\tSqlConnection Conn = " + objectName + "_Conn();");

            string colNames = "";
            foreach (string col in ColNames.Keys)
            {
                if (colNames != "") { colNames = colNames + ", " + col + " = @" + col; }
                if (colNames == "") { colNames = col + " = @" + col; }
            }
            NewString.AppendLine("\tstring SQLLine = \"DELETE From \" + " + objectName + "_TableName() + \" SET " + colNames + " WHERE " + whereField + " = @" + whereField + "\";");
            NewString.AppendLine("\tSqlCommand cmd2 = new SqlCommand(SQLLine, Conn);");
            NewString.AppendLine("\tcmd2 = new SqlCommand(SQLLine, Conn);");
            NewString.AppendLine("");

            NewString.AppendLine("\tcmd2.Parameters.AddWithValue(@\"" + whereField + "\", " + whereField + ".ToString());");

            NewString.AppendLine("");
            NewString.AppendLine("\tif (Conn.State == ConnectionState.Closed) { Conn.Open(); }");
            NewString.AppendLine("\tint recordsAffected = 0;");
            NewString.AppendLine("\t_AHK ahk = new _AHK();");
            NewString.AppendLine("\ttry { recordsAffected = cmd2.ExecuteNonQuery(); }");

            // SQL Error Catch / Retry
            NewString.AppendLine("\tcatch (SqlException ex)");
            NewString.AppendLine("{");
            NewString.AppendLine("if (ex.Number == 1205) // Retry on DeadLock");
            NewString.AppendLine("{");
            NewString.AppendLine("    ahk.Sleep(1000);");
            NewString.AppendLine("    " + objectName + "_DeleteSQL(" + whereField + ");");
            NewString.AppendLine("}");
            NewString.AppendLine("else if (ex.Message.ToUpper().Contains(\"TIMEOUT EXPIRED\")) // Retry on Standard TimeOut");
            NewString.AppendLine("{");
            NewString.AppendLine("    ahk.Sleep(1000);");
            NewString.AppendLine("    " + objectName + "_DeleteSQL(" + whereField + ");");
            NewString.AppendLine("}");
            NewString.AppendLine("else");
            NewString.AppendLine("{");
            NewString.AppendLine("    ahk.MsgBox(ex.ToString());");
            NewString.AppendLine("    return false;");
            NewString.AppendLine("}");
            NewString.AppendLine("}");

            NewString.AppendLine("\tConn.Close();");
            NewString.AppendLine("\tif (recordsAffected > 0) { return true; }");
            NewString.AppendLine("\telse return false;");
            NewString.AppendLine("}");

            return NewString.ToString();
        }



        // === NEW GUI Generate ====

        public string CodeGen_SQL_DeclareGUI(Dictionary<string, string> ColNames, string tableName = "[LucidMedia].[dbo].[FileIndex]", string objectName = "oBject")
        {
            StringBuilder NewString = new StringBuilder();

            NewString.AppendLine("#region === SQL GUI Calls ===");
            NewString.AppendLine("");

            foreach (KeyValuePair<string, string> col in ColNames)
            {
                string colName = col.Key;
                string fieldType = col.Value;

                bool isTextCol = sql.IsTextCol(fieldType);
                bool isIntCol = sql.IsIntCol(fieldType);
                bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType);
                bool isCbCol = sql.IsCbCol(fieldType);

                if (isTextCol || isIntCol) { NewString.AppendLine("RadTextBox txt" + colName); }
                if (isCbCol) { NewString.AppendLine("RadCheckBox cb" + colName); }
                if (isImageCol) { NewString.AppendLine("PictureBox pic" + colName); }
                if (isDateTimeCol) { NewString.AppendLine("RadDateTimePicker date" + colName); }
            }

            NewString.AppendLine("");
            NewString.AppendLine("");

            NewString.AppendLine(CodeGen_SQL_SaveGUI(ColNames, tableName, objectName));  // save gui codegen
            NewString.AppendLine("");

            NewString.AppendLine(CodeGen_SQL_LoadGUI(ColNames, tableName, objectName));  // load gui codegen

            NewString.AppendLine("");
            NewString.AppendLine("");
            NewString.AppendLine("#endregion");
            NewString.AppendLine("");

            return NewString.ToString();
        }

        public string CodeGen_SQL_SaveGUI(Dictionary<string, string> ColNames, string tableName = "[LucidMedia].[dbo].[FileIndex]", string objectName = "oBject")
        {
            StringBuilder NewString = new StringBuilder();

            NewString.AppendLine("\tpublic bool " + objectName + "_SaveGUI()");
            NewString.AppendLine("{");
            NewString.AppendLine("");

            NewString.AppendLine("\t" + objectName + " obj = new " + objectName + "();");

            foreach (KeyValuePair<string, string> col in ColNames)
            {
                string colName = col.Key;
                string fieldType = col.Value;

                bool isTextCol = sql.IsTextCol(fieldType);
                bool isIntCol = sql.IsIntCol(fieldType);
                bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType);
                bool isCbCol = sql.IsCbCol(fieldType);

                if (isTextCol || isIntCol) { NewString.AppendLine("\tobj." + colName + " = txt" + colName + ".Text;"); }
                if (isCbCol) { NewString.AppendLine("\tobj." + colName + " = cb" + colName + ".Text;"); }
                if (isImageCol) { NewString.AppendLine("\tobj." + colName + " = pic" + colName + ".Text;"); }
                if (isDateTimeCol) { NewString.AppendLine("\tobj." + colName + " = date" + colName + ".Text;"); }
            }

            NewString.AppendLine("");
            NewString.AppendLine("\tbool Updated = " + objectName + "_UpdateInsert(" + objectName + " obj);");
            NewString.AppendLine("\treturn Updated;");
            NewString.AppendLine("}");

            return NewString.ToString();
        }

        public string CodeGen_SQL_LoadGUI(Dictionary<string, string> ColNames, string tableName = "[LucidMedia].[dbo].[FileIndex]", string objectName = "oBject")
        {
            StringBuilder NewString = new StringBuilder();

            NewString.AppendLine("\tpublic bool " + objectName + "_LoadGUI(" + objectName + " obj)");
            NewString.AppendLine("{");
            NewString.AppendLine("");

            foreach (KeyValuePair<string, string> col in ColNames)
            {
                string colName = col.Key;
                string fieldType = col.Value;

                bool isTextCol = sql.IsTextCol(fieldType);
                bool isIntCol = sql.IsIntCol(fieldType);
                bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType);
                bool isCbCol = sql.IsCbCol(fieldType);

                if (isTextCol || isIntCol) { NewString.AppendLine("\tif (obj." + colName + " != null) { txt" + colName + ".Text = obj." + colName + "; }"); }
                if (isImageCol) { NewString.AppendLine("\tif (obj." + colName + " != null) { pic" + colName + ".Text = obj." + colName + "; }"); }
                if (isDateTimeCol) { NewString.AppendLine("\tif (obj." + colName + " != null) { date" + colName + ".Text = obj." + colName + "; }"); }
                if (isCbCol) { NewString.AppendLine("\tif (obj." + colName + " != null) { cb" + colName + ".Checked = obj." + colName + "; }"); }
            }

            NewString.AppendLine("");
            NewString.AppendLine("}");

            return NewString.ToString();
        }



        private string CodeGen_SQL_UpdateIfPopulated(Dictionary<string, string> ColNames, string tableName = "[LucidMedia].[dbo].[FileIndex]", string objectName = "oBject", string whereField = "ID")
        {
            StringBuilder NewString = new StringBuilder();

            NewString.Append(@"// Updates fields provided in object if values are populated. used for updating 1 or more fields at a time");
            NewString.Append("\n");
            NewString.Append("\tpublic bool " + objectName + "_UpdateIfPopulated(" + objectName + " obj, string " + whereField + " = \"\")\n");
            NewString.Append("{\n");
            NewString.Append("\t_AHK ahk = new _AHK();\n");
            NewString.Append("\tSqlConnection Conn = " + objectName + "_Conn();\n");
            NewString.Append("\tstring SQLcmd = \"Update \" + " + objectName + "_TableName() + \" SET \";\n");

            //foreach (string col in ColNames.Keys)
            //{
            //    NewString.Append("\tif (obj." + col + " != null) {SQLcmd = SQLcmd + \" " + col + " = @" + col + ",\"; }\n");
            //}


            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value; bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                if (colName == "ID") { continue; }

                // based on field type, add line
                if (isTextCol) { NewString.Append("\tif (obj." + colName + " != null) {SQLcmd = SQLcmd + \" " + colName + " = @" + colName + ",\"; }\n"); }
                if (isIntCol) { NewString.Append("\tSQLcmd = SQLcmd + \" " + colName + " = @" + colName + ",\";\n"); }
                if (isImageCol) { NewString.Append("\tSQLcmd = SQLcmd + \" " + colName + " = @" + colName + ",\";\n"); }
                if (isDateTimeCol) { NewString.Append("\tSQLcmd = SQLcmd + \" " + colName + " = @" + colName + ",\";\n"); }
                if (isCbCol) { NewString.Append("\tSQLcmd = SQLcmd + \" " + colName + " = @" + colName + ",\";\n"); }
            }



            NewString.Append("\tSQLcmd = ahk.TrimLast(SQLcmd, 1);\n");
            NewString.Append("\tSQLcmd = SQLcmd + \" WHERE " + whereField + " = @" + whereField + "\";\n");
            NewString.Append("\n");
            NewString.Append("\tSqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);\n");
            NewString.Append("\n");

            //foreach (string col in ColNames.Keys)
            //{
            //    NewString.Append("\tif (obj." + colName + " != null) { cmd2.Parameters.AddWithValue(@\"" + colName + "\", obj." + colName + "); }\n");
            //}

            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value; bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                // based on field type, add line
                if (isTextCol) { NewString.Append("\tif (obj." + colName + " != null) { cmd2.Parameters.AddWithValue(@\"" + colName + "\", obj." + colName + "); }\n"); }
                if (isIntCol) { NewString.Append("\tcmd2.Parameters.AddWithValue(@\"" + colName + "\", obj." + colName + ");\n"); }
                if (isImageCol) { NewString.Append("\tcmd2.Parameters.AddWithValue(@\"" + colName + "\", obj." + colName + ");\n"); }
                if (isDateTimeCol) { NewString.Append("\tcmd2.Parameters.AddWithValue(@\"" + colName + "\", obj." + colName + ");\n"); }
                if (isCbCol) { NewString.Append("\tcmd2.Parameters.AddWithValue(@\"" + colName + "\", obj." + colName + ");\n"); }
            }


            NewString.Append("\n");
            NewString.Append("\tif (Conn.State == ConnectionState.Closed) { Conn.Open(); }\n");
            NewString.Append("\tint recordsAffected = 0;\n");
            NewString.Append("\ttry { recordsAffected = cmd2.ExecuteNonQuery(); }\n");
            NewString.Append("\tcatch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }\n");
            NewString.Append("\tConn.Close();\n");
            NewString.Append("\tif (recordsAffected > 0) { return true; }\n");
            NewString.Append("\telse return false;\n");

            NewString.Append("}\n");

            return NewString.ToString();
        }

        private string CodeGen_SQL_ObjectList(Dictionary<string, string> ColNames, string tableName = "TableName", string objectName = "objectName")
        {
            StringBuilder NewString = new StringBuilder();

            NewString.Append("\tpublic List<" + objectName + "> " + objectName + "_ReturnSQLList(string Command = \"\")\n");
            NewString.Append("{\n");
            NewString.Append("\t_Database.SQL sql = new _Database.SQL();\n");
            NewString.Append("\tif (Command == \"\") { Command = \"Select * From \" + " + objectName + "_TableName(); }\n");
            NewString.Append("\tSqlConnection Conn = " + objectName + "_Conn();\n");
            NewString.Append("\tDataTable ReturnTable = sql.GetDataTable(Conn, Command);\n");
            NewString.Append("\tList<" + objectName + "> ReturnList = new List<" + objectName + ">();\n");
            NewString.Append("\tif (ReturnTable != null)\n");
            NewString.Append("\t{\n");
            NewString.Append("\t\tforeach (DataRow ret in ReturnTable.Rows)\n");
            NewString.Append("\t\t{\n");
            NewString.Append("\t\t\t" + objectName + " returnObject = new " + objectName + "();\n");

            //foreach (string col in ColNames.Keys)
            //{
            //    NewString.Append("\t\t\t" + "returnObject." + colName + " = ret[\"" + colName + "\"].ToString();\n");
            //}

            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value; bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                // based on field type, add line
                if (isTextCol) { NewString.Append("\t\t\t" + "returnObject." + colName + " = ret[\"" + colName + "\"].ToString();\n"); }
                if (isIntCol) { NewString.Append("\t\t\t" + "returnObject." + colName + " = ret[\"" + colName + "\"].ToInt();\n"); }
                if (isImageCol) { NewString.Append("\t\t\t" + "returnObject." + colName + " = ret[\"" + colName + "\"].ToString();\n"); }
                if (isDateTimeCol) { NewString.Append("\t\t\t" + "returnObject." + colName + " = ret[\"" + colName + "\"].ToDateTime();\n"); }
                if (isCbCol) { NewString.Append("\t\t\t" + "returnObject." + colName + " = ret[\"" + colName + "\"].ToBool();\n"); }
            }


            NewString.Append("\t\t\tReturnList.Add(returnObject);\n");
            NewString.Append("\t\t}\n");
            NewString.Append("\t}\n");
            NewString.Append("\treturn ReturnList;\n");
            NewString.Append("}\n");

            return NewString.ToString();
        }

        private string CodeGen_SQL_Object(Dictionary<string, string> ColNames, string tableName = "TableName", string objectName = "objectName", string whereField = "ID")
        {
            StringBuilder NewString = new StringBuilder();

            NewString.AppendLine("\tpublic " + objectName + " " + objectName + "_ReturnSQL(string " + whereField + ")");
            NewString.AppendLine("{");
            NewString.AppendLine("\t_Database.SQL sql = new _Database.SQL();");
            NewString.AppendLine("\tSqlConnection Conn = " + objectName + "_Conn();");

            string colNames = "";
            foreach (string col in ColNames.Keys)
            {
                if (colNames != "") { colNames = colNames + ",[" + col + "]"; }
                if (colNames == "") { colNames = "[" + col + "]"; }
            }


            NewString.AppendLine("\tstring SelectLine = \"Select " + colNames + " From \" + " + objectName + "_TableName() + \" WHERE " + whereField + " = '\" + " + whereField + " + \"'\";");
            //NewString.Append("\tstring SelectLine = \"Select " + colNames + " From " + tableName + " WHERE " + whereField + " = '\" + " + whereField + " + \"'\";\n");
            NewString.AppendLine("\tDataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);");
            NewString.AppendLine("\t" + objectName + " returnObject = new " + objectName + "();");
            NewString.AppendLine("\tif (ReturnTable != null)");
            NewString.AppendLine("\t{");
            NewString.AppendLine("\tforeach (DataRow ret in ReturnTable.Rows)");
            NewString.AppendLine("\t\t{");

            //foreach (string col in ColNames.Keys)
            //{
            //    NewString.AppendLine("\t\t\treturnObject." + col + " = ret[\"" + col + "\"].ToString();");
            //}

            foreach (KeyValuePair<string, string> col in ColNames)
            {
                string colName = col.Key;
                string fieldType = col.Value;

                bool isTextCol = sql.IsTextCol(fieldType);
                bool isIntCol = sql.IsIntCol(fieldType);
                bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType);
                bool isCbCol = sql.IsCbCol(fieldType);

                if (isTextCol) { NewString.AppendLine("\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToString();"); }
                if (isIntCol) { NewString.AppendLine("\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToInt();"); }
                if (isImageCol) { NewString.AppendLine("\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToString();"); }
                if (isDateTimeCol) { NewString.AppendLine("\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToDateTime();"); }
                if (isCbCol) { NewString.AppendLine("\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToBool();"); }
            }


            NewString.AppendLine("\t\treturn returnObject;");
            NewString.AppendLine("\t\t}");
            NewString.AppendLine("\t}");
            NewString.AppendLine("\treturn returnObject;");
            NewString.AppendLine("}");

            return NewString.ToString();
        }

        private string CodeGen_SQL_Connection(string objectName = "objectName", string TableName = "TableName", string ConnName = "SQLserver")
        {
            StringBuilder NewString = new StringBuilder();
            NewString.Append("// Return " + objectName + " SQL Connection String\n");
            NewString.Append("\tpublic SqlConnection " + objectName + "_Conn()\n");
            NewString.Append("{\n");
            NewString.Append("// populate sql connection\n");
            NewString.Append("SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[\"" + ConnName + "\"].ConnectionString);\n");
            NewString.Append("// SqlConnection Conn = new SqlConnection(\"Server=188.168.188.88;DataBase=LucidMedia;Uid=lucidm;Pwd=pass\");\n");
            NewString.Append("\treturn conn;\n");
            NewString.Append("}\n");
            NewString.Append("\n");
            NewString.Append("// Return " + objectName + " TableName (Full Path)\n");
            NewString.Append("\tpublic string " + objectName + "_TableName()\n");
            NewString.Append("{\n");
            NewString.Append("// populate to return full sql table name\n");
            NewString.Append("\treturn \"" + TableName + "\";\n");
            NewString.Append("}\n");

            return NewString.ToString();
        }

        private string CodeGen_SQL_CreateTable(Dictionary<string, string> ColNames, string tableName = "TableName", string objectName = "objectName")
        {
            StringBuilder NewString = new StringBuilder();
            NewString.AppendLine("// Generate SQL Table");
            NewString.AppendLine("\tpublic bool " + objectName + "_CreateSQLTable()");
            NewString.AppendLine("{");
            NewString.AppendLine("\tSqlConnection Conn = " + objectName + "_Conn();");

            if (tableName.Contains("[")) { NewString.AppendLine("\tstring CreateTableLine = \"CREATE TABLE " + tableName + " (\";"); }
            if (!tableName.Contains("[")) { NewString.AppendLine("\tstring CreateTableLine = \"CREATE TABLE [" + tableName + "](\";"); }

            int i = 0;
            foreach (string col in ColNames.Keys)
            {
                i++;
                if (i == 1) { NewString.AppendLine("\tCreateTableLine = CreateTableLine + \"[" + col + "] [int] IDENTITY(1,1) NOT NULL,\";"); continue; }
                if (i != ColNames.Count) { NewString.AppendLine("\tCreateTableLine = CreateTableLine + \"[" + col + "] [varchar](max) NOT NULL,\";"); }
                if (i == ColNames.Count) { NewString.AppendLine("\tCreateTableLine = CreateTableLine + \"[" + col + "] [varchar](max) NOT NULL\";"); }
            }

            NewString.AppendLine("\tCreateTableLine = CreateTableLine + \") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]\";");
            NewString.AppendLine("");
            NewString.AppendLine("\t_Database.SQL sql = new _Database.SQL();");
            NewString.AppendLine("\treturn sql.WriteDataRecord(Conn, CreateTableLine);");
            NewString.AppendLine("}");
            return NewString.ToString();
        }

        private string CodeGen_SQL_To_SQLite(Dictionary<string, string> ColNames, string tableName = "[LucidMedia].[dbo].[FileIndex]", string objectName = "oBject", string ConnName = "Con")
        {
            StringBuilder NewString = new StringBuilder();
            NewString.Append("\tpublic bool " + objectName + "_SQL_to_SQLite(string SqliteDBPath = @\"\\Db\\" + objectName + ".sqlite\")\n");
            NewString.Append("{\n");
            NewString.Append("\t_AHK ahk = new _AHK();\n");
            NewString.Append("\t_Database.SQLite sqlite = new _Database.SQLite();\n");
            NewString.Append("\tstring SaveFile = SqliteDBPath;\n");

            NewString.Append("\tif (SqliteDBPath == @\"\\Db\\" + objectName + ".sqlite\")\n\t{\n\t\tahk.FileCreateDir(ahk.AppDir() + @\"\\Db\");\n\t\tSaveFile = ahk.AppDir() + @\"\\Db\\" + objectName + ".sqlite\"; }\n\n");

            //string tablenm = sql.TableName_From_FullDbPath(tableName);

            string table = ahk.StringSplit(tableName, ".", 2, true, true);
            string dbName = ahk.StringSplit(tableName, ".", 0);

            NewString.Append("\t//sb(\"Copying SQL Db to \" + SaveFile + \"...\");\n");
            NewString.Append("\tsqlite.SQLTable_To_NewSQLiteTable(" + objectName + "_Conn(), \"" + dbName + "\", \"" + table + "\", SaveFile, \"\", false, false, false);\n");
            NewString.Append("\t//sb(\"FINISHED Copying SQL Db to \" + SaveFile);\n\n");

            NewString.Append("if (File.Exists(SaveFile)) { return true; } else { return false; }\n}\n");

            return NewString.ToString();
        }



        


    }
}
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

        #region === CodGen Objects ===

        public string Objects_Changed(Dictionary<string, string> colNames, string objectName = "oBjectName", string tableName = "tableNAME")
        {
            StringBuilder NewString = new StringBuilder();
            NewString.AppendLine("// Compare two objects to see if they have identical values");
            NewString.AppendLine("\tpublic bool " + objectName + "_Changed(" + objectName + " OldVal, " + objectName + " NewVal)");
            NewString.AppendLine("{");
            NewString.AppendLine("\t" + objectName + " diff = new " + objectName + "();");
            NewString.AppendLine("\tList<string> diffList = new List<string>();");
            NewString.AppendLine("\tbool different = false;");

            //foreach (string col in colNames.Keys)
            //{
            //    NewString.AppendLine("if (OldVal." + col + " == null) { OldVal." + col + " = \"\"; }");
            //    NewString.AppendLine("if (NewVal." + col + " == null) { NewVal." + col + " = \"\"; }");
            //    NewString.AppendLine("if (OldVal." + col + " != NewVal." + col + ") { different = true; }");
            //}

            foreach (KeyValuePair<string, string> col in colNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value; bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                // based on field type, add line
                if (isTextCol)
                {
                    NewString.AppendLine("\tif (OldVal." + colName + " == null) { OldVal." + colName + " = \"\"; }");
                    NewString.AppendLine("\tif (NewVal." + colName + " == null) { NewVal." + colName + " = \"\"; }");
                    NewString.AppendLine("\tif (OldVal." + colName + " != NewVal." + colName + ") { different = true; }");
                }
                //if (isIntCol) { NewString.AppendLine("\t\t\tpublic int " + colName + " { get; set; }"); }
                //if (isImageCol) { NewString.AppendLine("\t\t\tpublic string " + colName + " { get; set; }"); }
                //if (isDateTimeCol) { NewString.AppendLine("\t\t\tpublic DateTime " + colName + " { get; set; }"); }
                //if (isCbCol) { NewString.AppendLine("\t\t\tpublic bool " + colName + " { get; set; }"); }
            }


            NewString.AppendLine("\treturn different;");
            NewString.AppendLine("}");

            return NewString.ToString();
        }

        // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
        public string Objects_DiffList(Dictionary<string, string> colNames, string objectName = "oBjectName", string tableName = "tableNAME")
        {
            StringBuilder NewString = new StringBuilder();

            NewString.Append("// Returns list of strings with the previous/new values after comparing 2 objects. Used for change log\n");
            NewString.Append("\tpublic List<string> " + objectName + "_DiffList(" + objectName + " OldVal, " + objectName + " NewVal)\n");
            NewString.Append("{\n");
            NewString.Append("\tList<string> diffList = new List<string>();\n");

            foreach (string col in colNames.Keys)
            {
                NewString.Append("\tif (OldVal." + col + " != NewVal." + col + ") { diffList.Add(\"Changed " + col + " Value From \" + OldVal." + col + " + \" To \" + NewVal." + col + "); }\n");
            }

            NewString.Append("\treturn diffList;\n");
            NewString.Append("}\n");

            return NewString.ToString();
        }

        public string Objects_Diff(Dictionary<string, string> colNames, string objectName = "oBjectName", string tableName = "tableNAME")
        {
            StringBuilder NewString = new StringBuilder();
            NewString.Append("// Returns object containing the new values different from the old values in object comparison\n");
            NewString.Append("\tpublic " + objectName + " " + objectName + "_Diff(" + objectName + " OldVal, " + objectName + " NewVal)\n");
            NewString.Append("{\n");
            NewString.Append("\t" + objectName + " diff = new " + objectName + "();\n");

            foreach (string col in colNames.Keys)
            {
                NewString.Append("\tif (OldVal." + col + " != NewVal." + col + ") { diff." + col + " = NewVal." + col + "; }\n");
            }

            NewString.Append("\treturn diff;\n");
            NewString.Append("}\n");

            return NewString.ToString();
        }


        #endregion


    }
}
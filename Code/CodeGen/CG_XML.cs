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

        private string CodeGen_ToXMLString(Dictionary<string, string> ColNames, string tableName = "TableName", string objectName = "objectName")
        {
            StringBuilder NewString = new StringBuilder();
            NewString.AppendLine("// Generate XML String From Object");
            NewString.AppendLine("\tpublic string " + objectName + "_ToXML(" + objectName + " obj)");
            NewString.AppendLine("{");
            NewString.AppendLine("\tstring XMLString = \"\";");

            //foreach (string col in ColNames.Keys)
            //{
            //    NewString.AppendLine("\tXMLString = XMLString + \"<" + col + ">\" + obj." + col + " + \"</" + col + ">\";");
            //}

            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value; bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                // based on field type, add line
                if (isTextCol) { NewString.AppendLine("\tXMLString = XMLString + \"<" + colName + ">\" + obj." + colName + " + \"</" + colName + ">\";"); }
                if (isIntCol) { NewString.AppendLine("\tXMLString = XMLString + \"<" + colName + ">\" + obj." + colName + " + \"</" + colName + ">\";"); }
                if (isImageCol) { NewString.AppendLine("\tXMLString = XMLString + \"<" + colName + ">\" + obj." + colName + " + \"</" + colName + ">\";"); }
                if (isDateTimeCol) { NewString.AppendLine("\tXMLString = XMLString + \"<" + colName + ">\" + obj." + colName + " + \"</" + colName + ">\";"); }
                if (isCbCol) { NewString.AppendLine("\tXMLString = XMLString + \"<" + colName + ">\" + obj." + colName + " + \"</" + colName + ">\";"); }
            }


            NewString.AppendLine("\treturn XMLString;");
            NewString.AppendLine("}");
            return NewString.ToString();
        }

        private string CodeGen_FromXMLString(Dictionary<string, string> ColNames, string tableName = "TableName", string objectName = "objectName")
        {
            StringBuilder NewString = new StringBuilder();
            NewString.AppendLine("// Populate Object from XML Tag String");
            NewString.AppendLine("\tpublic " + objectName + " " + objectName + "_FromXML(string XMLString)");
            NewString.AppendLine("{");
            NewString.AppendLine("\t_Parse prs = new _Parse();");

            NewString.AppendLine("\t" + objectName + " obj = new " + objectName + "();");

            //foreach (string col in ColNames.Keys)
            //{
            //    NewString.AppendLine("\tobj." + col + " = prs.XML_Text(XMLString, \"<" + col + ">\");");
            //}

            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value; bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                // based on field type, add line
                if (isTextCol) { NewString.AppendLine("\tobj." + colName + " = prs.XML_Text(XMLString, \"<" + colName + ">\");"); }
                if (isIntCol) { NewString.AppendLine("\tobj." + colName + " = prs.XML_Text(XMLString, \"<" + colName + ">\").ToInt();"); }
                if (isImageCol) { NewString.AppendLine("\tobj." + colName + " = prs.XML_Text(XMLString, \"<" + colName + ">\");"); }
                if (isDateTimeCol) { NewString.AppendLine("\tobj." + colName + " = prs.XML_Text(XMLString, \"<" + colName + ">\").ToDateTime();"); }
                if (isCbCol) { NewString.AppendLine("\tobj." + colName + " = prs.XML_Text(XMLString, \"<" + colName + ">\").ToBool();"); }
            }


            NewString.AppendLine("\treturn obj;");
            NewString.AppendLine("}");
            return NewString.ToString();
        }



    }
}
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

        private string CodeGen_CreateDataTable(Dictionary<string, string> ColNames, string tableName = "TableName", string objectName = "oBject")
        {
            string CreateLine = "\t\tDataTable table = new DataTable();\n";
            string CreateCols = "// Columns\n";
            string CreateRow = "";

            //int i = 0;
            //foreach (string col in ColNames.Keys)
            //{
            //    //if (i == 0) { i++; continue; } // skip the parent node

            //    if (CreateCols != "") { CreateCols = CreateCols + "\n\t\ttable.Columns.Add(\"" + col + "\", typeof(string));"; }
            //    if (CreateCols == "") { CreateCols = "\t\ttable.Columns.Add(\"" + col + "\", typeof(string));"; }

            //    if (CreateRow != "") { CreateRow = CreateRow + ", inObject." + colName; } if (CreateRow == "") { CreateRow = "inObject." + colName; }
            //}

            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value; bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                // based on field type, add line
                if (isTextCol) { CreateCols = CreateCols + "\n\t\ttable.Columns.Add(\"" + colName + "\", typeof(string));"; if (CreateRow != "") { CreateRow = CreateRow + ", inObject." + colName; } if (CreateRow == "") { CreateRow = "inObject." + colName; } }
                if (isIntCol) { CreateCols = CreateCols + "\n\t\ttable.Columns.Add(\"" + colName + "\", typeof(int));"; if (CreateRow != "") { CreateRow = CreateRow + ", inObject." + colName; } if (CreateRow == "") { CreateRow = "inObject." + colName; } }
                if (isImageCol) { CreateCols = CreateCols + "\n\t\ttable.Columns.Add(\"" + colName + "\", typeof(string));"; if (CreateRow != "") { CreateRow = CreateRow + ", inObject." + colName; } if (CreateRow == "") { CreateRow = "inObject." + colName; } }
                if (isDateTimeCol) { CreateCols = CreateCols + "\n\t\ttable.Columns.Add(\"" + colName + "\", typeof(DateTime));"; if (CreateRow != "") { CreateRow = CreateRow + ", inObject." + colName; } if (CreateRow == "") { CreateRow = "inObject." + colName; } }
                if (isCbCol) { CreateCols = CreateCols + "\n\t\ttable.Columns.Add(\"" + colName + "\", typeof(bool));"; if (CreateRow != "") { CreateRow = CreateRow + ", inObject." + colName; } if (CreateRow == "") { CreateRow = "inObject." + colName; } }
            }



            CreateLine = CreateLine + CreateCols + "\n\n\t\ttable.Rows.Add(" + CreateRow + ");";

            string Function = "\t\tpublic DataTable Create_" + tableName + "_DataTable(" + objectName + " inObject)" + "\n\t{\n";
            Function = Function + CreateLine + "\n\t\treturn table;\n\t}\n";

            return Function;
        }

        private string CodeGen_ReturnDataTable(Dictionary<string, string> ColNames, string tableName = "TableName", string objectName = "oBject")
        {

            string Function = "";
            //string tbl = "";
            //if (TablesTV.SelectedNode != null) { tbl = TablesTV.SelectedNode.Text; }
            //if (sqlTablesTree.SelectedNode != null) { tbl = sqlTablesTree.SelectedNode.Text; }
            //if (tbl == "") { tbl = TableName; }


            //tab.Select_Tab(tabControl2, 1); // where tab
            string SelectLine = "\nif (DbFile == \"\") { DbFile = ahk.AppDir() + @\"\\Db\\" + objectName + ".sqlite\"; }\n\t\tstring SelectLine = \"Select * From [" + tableName + "]\";\n";

            string WhereClause = "\t\tif (WhereClause != \"\")\n\t\t{\n\t\t\tif (WhereClause.ToUpper().Contains(\"WHERE\")) { SelectLine = SelectLine + \" \" + WhereClause; }";
            WhereClause = WhereClause + "\n\t\t\tif (!WhereClause.ToUpper().Contains(\"WHERE\")) { SelectLine = SelectLine + \" WHERE \" + WhereClause; }\n\t\t}\n";
            WhereClause = WhereClause + "\n\t\tDataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);\n";

            Function = "\t\tpublic DataTable Return_" + objectName + "_DataTable(string DbFile=\"\", string TableName = \"" + objectName + "\", string WhereClause = \"\", bool Debug = false)" + "\n\t{\n";
            //Function = DbFileLine + "\n\n" + Function;
            Function = "\n\t" + Function + SelectLine + "\n" + WhereClause + "\n\n";

            // new table setup

            string CreateLine = "\t\tDataTable table = new DataTable();\n";
            string CreateCols = "// Columns";
            string CreateRow = "";


            //int i = 0;
            //foreach (string col in ColNames.Keys)
            //{
            //    //if (i == 0) { i++; continue; } // skip the parent node

            //    if (CreateCols != "") { CreateCols = CreateCols + "\n\t\ttable.Columns.Add(\"" + col + "\", typeof(string));"; }
            //    if (CreateCols == "") { CreateCols = "\t\ttable.Columns.Add(\"" + col + "\", typeof(string));"; }

            //    if (CreateRow != "") { CreateRow = CreateRow + ", returnObject." + colName; } if (CreateRow == "") { CreateRow = "returnObject." + colName; }
            //}


            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value; bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                // based on field type, add line
                if (isTextCol) { CreateCols = CreateCols + "\n\t\ttable.Columns.Add(\"" + colName + "\", typeof(string));"; if (CreateRow != "") { CreateRow = CreateRow + ", returnObject." + colName; } if (CreateRow == "") { CreateRow = "returnObject." + colName; } }
                if (isIntCol) { CreateCols = CreateCols + "\n\t\ttable.Columns.Add(\"" + colName + "\", typeof(int));"; if (CreateRow != "") { CreateRow = CreateRow + ", returnObject." + colName; } if (CreateRow == "") { CreateRow = "returnObject." + colName; } }
                if (isImageCol) { CreateCols = CreateCols + "\n\t\ttable.Columns.Add(\"" + colName + "\", typeof(string));"; if (CreateRow != "") { CreateRow = CreateRow + ", returnObject." + colName; } if (CreateRow == "") { CreateRow = "returnObject." + colName; } }
                if (isDateTimeCol) { CreateCols = CreateCols + "\n\t\ttable.Columns.Add(\"" + colName + "\", typeof(DateTime));"; if (CreateRow != "") { CreateRow = CreateRow + ", returnObject." + colName; } if (CreateRow == "") { CreateRow = "returnObject." + colName; } }
                if (isCbCol) { CreateCols = CreateCols + "\n\t\ttable.Columns.Add(\"" + colName + "\", typeof(bool));"; if (CreateRow != "") { CreateRow = CreateRow + ", returnObject." + colName; } if (CreateRow == "") { CreateRow = "returnObject." + colName; } }
            }




            CreateLine = CreateLine + CreateCols + "\n\n";
            string createRow = "table.Rows.Add(" + CreateRow + ");";

            Function = Function + CreateLine;


            // gather all the selected column names to use as object names to populate
            string ParseReturn = "\t\tif (ReturnTable != null)\n\t\t\t{\n\t\t\tforeach (DataRow ret in ReturnTable.Rows)\n\t\t\t\t{\n\t\t\t\t" + objectName + " returnObject = new " + objectName + "();\n\n";
            string Objects = "// Populate Objects";  // list of vars used as input in functions below

            //i = 0;
            //foreach (string col in ColNames.Keys)
            //{
            //    //if (i == 0) { i++; continue; } // skip the parent node

            //    if (Objects != "") { Objects = Objects + "\n\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToString();"; }
            //    if (Objects == "") { Objects = "\t\t\t\treturnObject." + col + " = ret[\"" + col + "\"].ToString();"; }
            //}


            foreach (KeyValuePair<string, string> col in ColNames)
            {
                // determine what kind of sql field is being used
                string colName = col.Key; string fieldType = col.Value; bool isTextCol = sql.IsTextCol(fieldType); bool isIntCol = sql.IsIntCol(fieldType); bool isImageCol = sql.IsImageCol(fieldType);
                bool isDateTimeCol = sql.IsDateTimeCol(fieldType); bool isCbCol = sql.IsCbCol(fieldType);

                // based on field type, add line
                if (isTextCol) { Objects = Objects + "\n\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToString();"; }
                if (isIntCol) { Objects = Objects + "\n\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToInt();"; }
                if (isImageCol) { Objects = Objects + "\n\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToString();"; }
                if (isDateTimeCol) { Objects = Objects + "\n\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToDateTime();"; }
                if (isCbCol) { Objects = Objects + "\n\t\t\t\treturnObject." + colName + " = ret[\"" + colName + "\"].ToBool();"; }
            }



            ParseReturn = ParseReturn + Objects + "\n\n\t\t\t\t" + createRow + "\n\t\t\t\t}\n\t\t\t}\n";
            Function = Function + ParseReturn + "\n\t\treturn table;\n\t}\n";

            return Function;
        }


    }
}
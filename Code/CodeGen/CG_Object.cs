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
        public class codeGn
        {
            public string AllCodeReturn { get; set; }
            public string SQL_PopulateRadControls { get; set; }
            public string SQL_Insert { get; set; }
            public string SQL_Update { get; set; }
            public string SQL_UpdateInsert { get; set; }
            public string SQL_Delete { get; set; }
            public string SQL_Objects { get; set; }
            public string SQL_ObjectDef { get; set; }
            public string SQL_ObjectList { get; set; }
            public string SQL_To_SQLite { get; set; }
            public string SQLite_CreateTable { get; set; }
            public string SQLite_ReturnVars { get; set; }
            public string SQLite_ObjectDef { get; set; }
            public string SQLite_ObjectPopulate { get; set; }
            public string SQLite_ObjectFixChars { get; set; }
            public string SQLite_Select { get; set; }
            public string SQLite_Object { get; set; }
            public string SQLite_ObjectList { get; set; }
            public string SQLite_Insert { get; set; }
            public string SQLite_Update { get; set; }
            public string SQLite_UpdateInsert { get; set; }
            public string SQLite_ReturnDatatable { get; set; }
            public string SQLite_CreateDatatable { get; set; }
            public string SQLite_GridColsVisible { get; set; }
            public string SQLite_GridColsEnabled { get; set; }


            public string ConnectionName { get; set; }
            public string NewFilePath { get; set; }
            public string ClassName { get; set; }

            public string NewForm { get; set; }

            public string ToXMLString { get; set; }
            public string FromXMLString { get; set; }

            // input vars

            public string whereColumn { get; set; }

            public string ObjectName { get; set; }
            public string TableNameShort { get; set; }
            public string TableNameFull { get; set; }
            public Dictionary<string, string> ColumnNames { get; set; }

            public RadTreeNode CodeNodes { get; set; }

            // return regions

            public string Region_ButtonActions { get; set; }

            public string Region_Object { get; set; }
            public string Region_SQL { get; set; }
            public string Region_SQLite_Return { get; set; }
            public string Region_SQLite_Update { get; set; }
            public string Region_DataTable { get; set; }
            public string Region_DataGridView { get; set; }
        }



    }
}
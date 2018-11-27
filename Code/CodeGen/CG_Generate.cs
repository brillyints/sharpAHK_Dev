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
        // === SQL/SQLITE Function CodeGen [Latest v3 ]===

        //=== Code Gen ====
        int SectionsAdded = 0;

        public codeGn codeGenDbCode(codeGn CodeGen)
        {
            codeGn cd = CodeGen;


            string whereField = cd.whereColumn;

            // parse table name from full table path if table name not provided
            if (cd.TableNameShort.Trim() == "" && cd.TableNameFull != "")
            {
                cd.TableNameShort = sql.TableName_From_FullDbPath(cd.TableNameFull);
            }

            string TableName = cd.TableNameShort;
            string ObjectName = cd.ObjectName.Replace(" ", "_");

            Dictionary<string, string> colNames = cd.ColumnNames;

            //#### Generate Regions + Functions Underneath ####

            // object region
            // no use ? CodeGen_Return_Variables(colNames) + "\n\r"


            RadTreeNode CodeNodes = new RadTreeNode();
            CodeNodes.Text = "CodeGen"; CodeNodes.Tag = "CodeGen";


            //cd.SQLite_ObjectPopulate = CodeGen_Return_ObjectPopulate(colNames, ObjectName);
            cd.SQLite_ObjectFixChars = CodeGen_ObjectFixChars(colNames, ObjectName);
            cd.SQLite_ObjectDef = CodeGen_Return_ObjectDef(colNames, ObjectName);
            string objChanged = Objects_Changed(colNames, ObjectName, TableName);
            string objDiff = Objects_Diff(colNames, ObjectName, TableName);

            string objDiffList = Objects_DiffList(colNames, ObjectName, TableName);
            cd.ToXMLString = CodeGen_ToXMLString(colNames, TableName, ObjectName);

            cd.FromXMLString = CodeGen_FromXMLString(colNames, TableName, ObjectName);

            string populateRadControls = CodeGen_Return_PopulateControls(colNames, ObjectName);
            cd.SQL_PopulateRadControls = populateRadControls;

            cd.Region_ButtonActions = CodeGen_Return_ButtonActions(colNames, ObjectName, cd.whereColumn);

            //cd.Region_Object = "\t#region ===== " + ObjectName + " Object =====\n\r" + cd.SQLite_ObjectDef + "\n" + cd.SQLite_ObjectFixChars + "\n" + objChanged + "\n" + objDiff + "\n" + objDiffList + "\n" + cd.ToXMLString + "\n" + cd.FromXMLString + "\n\r\t#endregion";
            cd.Region_Object = "\t#region ===== " + ObjectName + " Object =====\n\r" + cd.SQLite_ObjectDef + "\n" + "\n" + cd.FromXMLString + "\n" + populateRadControls + "\n\r\t#endregion";



            RadTreeNode SubNode = new RadTreeNode();
            SubNode.Text = "Object"; SubNode.Tag = cd.Region_Object;

            RadTreeNode subSubNode = new RadTreeNode();
            subSubNode.Text = "Object"; subSubNode.Tag = cd.SQLite_ObjectDef;
            SubNode.Nodes.Add(subSubNode);

            subSubNode = new RadTreeNode();
            subSubNode.Text = "FromXMLString"; subSubNode.Tag = cd.ToXMLString;
            SubNode.Nodes.Add(subSubNode);

            subSubNode = new RadTreeNode();
            subSubNode.Text = "ToXMLString"; subSubNode.Tag = cd.FromXMLString;
            SubNode.Nodes.Add(subSubNode);

            CodeNodes.Nodes.Add(SubNode);


            RadTreeNode sqlNode = new RadTreeNode();
            sqlNode.Text = "SQL"; sqlNode.Tag = "SQL CodeGen";

            RadTreeNode sqlSubNode = new RadTreeNode();
            sqlSubNode.Text = "SQL_Insert"; sqlSubNode.Tag = cd.SQL_Insert;
            sqlNode.Nodes.Add(sqlSubNode);

            sqlSubNode = new RadTreeNode();
            sqlSubNode.Text = "SQL_Update"; sqlSubNode.Tag = cd.SQL_Update;
            sqlNode.Nodes.Add(sqlSubNode);

            sqlSubNode = new RadTreeNode();
            sqlSubNode.Text = "SQL_UpdateInsert"; sqlSubNode.Tag = cd.SQL_UpdateInsert;
            sqlNode.Nodes.Add(sqlSubNode);

            sqlSubNode = new RadTreeNode();
            sqlSubNode.Text = "SQL_Objects"; sqlSubNode.Tag = cd.SQL_Objects;
            sqlNode.Nodes.Add(sqlSubNode);

            sqlSubNode = new RadTreeNode();
            sqlSubNode.Text = "SQL_ObjectDef"; sqlSubNode.Tag = cd.SQL_ObjectDef;
            sqlNode.Nodes.Add(sqlSubNode);

            sqlSubNode = new RadTreeNode();
            sqlSubNode.Text = "SQL_ObjectList"; sqlSubNode.Tag = cd.SQL_ObjectList;
            sqlNode.Nodes.Add(sqlSubNode);

            sqlSubNode = new RadTreeNode();
            sqlSubNode.Text = "SQL_To_SQLite"; sqlSubNode.Tag = cd.SQL_To_SQLite;
            sqlNode.Nodes.Add(sqlSubNode);

            CodeNodes.Nodes.Add(sqlNode);


            RadTreeNode sqliteNode = new RadTreeNode();
            sqliteNode.Text = "SQLite"; sqliteNode.Tag = "SQLite CodeGen";

            RadTreeNode sqliteSubNode = new RadTreeNode();
            sqliteSubNode.Text = "SQLite_CreateTable"; sqliteSubNode.Tag = cd.SQLite_CreateTable;
            sqliteNode.Nodes.Add(sqliteSubNode);

            sqliteSubNode = new RadTreeNode();
            sqliteSubNode.Text = "SQLite_ReturnVars"; sqliteSubNode.Tag = cd.SQLite_ReturnVars;
            sqliteNode.Nodes.Add(sqliteSubNode);

            sqliteSubNode = new RadTreeNode();
            sqliteSubNode.Text = "SQLite_ObjectDef"; sqliteSubNode.Tag = cd.SQLite_ObjectDef;
            sqliteNode.Nodes.Add(sqliteSubNode);

            sqliteSubNode = new RadTreeNode();
            sqliteSubNode.Text = "SQLite_ObjectPopulate"; sqliteSubNode.Tag = cd.SQLite_ObjectPopulate;
            sqliteNode.Nodes.Add(sqliteSubNode);

            sqliteSubNode = new RadTreeNode();
            sqliteSubNode.Text = "SQLite_ObjectFixChars"; sqliteSubNode.Tag = cd.SQLite_ObjectFixChars;
            sqliteNode.Nodes.Add(sqliteSubNode);

            sqliteSubNode = new RadTreeNode();
            sqliteSubNode.Text = "SQLite_Select"; sqliteSubNode.Tag = cd.SQLite_Select;
            sqliteNode.Nodes.Add(sqliteSubNode);

            sqliteSubNode = new RadTreeNode();
            sqliteSubNode.Text = "SQLite_Object"; sqliteSubNode.Tag = cd.SQLite_Object;
            sqliteNode.Nodes.Add(sqliteSubNode);

            sqliteSubNode = new RadTreeNode();
            sqliteSubNode.Text = "SQLite_ObjectList"; sqliteSubNode.Tag = cd.SQLite_ObjectList;
            sqliteNode.Nodes.Add(sqliteSubNode);

            sqliteSubNode = new RadTreeNode();
            sqliteSubNode.Text = "SQLite_Insert"; sqliteSubNode.Tag = cd.SQLite_Insert;
            sqliteNode.Nodes.Add(sqliteSubNode);

            sqliteSubNode = new RadTreeNode();
            sqliteSubNode.Text = "SQLite_Update"; sqliteSubNode.Tag = cd.SQLite_Update;
            sqliteNode.Nodes.Add(sqliteSubNode);

            sqliteSubNode = new RadTreeNode();
            sqliteSubNode.Text = "SQLite_UpdateInsert"; sqliteSubNode.Tag = cd.SQLite_UpdateInsert;
            sqliteNode.Nodes.Add(sqliteSubNode);

            sqliteSubNode = new RadTreeNode();
            sqliteSubNode.Text = "SQLite_ReturnDatatable"; sqliteSubNode.Tag = cd.SQLite_ReturnDatatable;
            sqliteNode.Nodes.Add(sqliteSubNode);

            sqliteSubNode = new RadTreeNode();
            sqliteSubNode.Text = "SQLite_CreateDatatable"; sqliteSubNode.Tag = cd.SQLite_CreateDatatable;
            sqliteNode.Nodes.Add(sqliteSubNode);

            sqliteSubNode = new RadTreeNode();
            sqliteSubNode.Text = "SQLite_GridColsVisible"; sqliteSubNode.Tag = cd.SQLite_GridColsVisible;
            sqliteNode.Nodes.Add(sqliteSubNode);

            sqliteSubNode = new RadTreeNode();
            sqliteSubNode.Text = "SQLite_GridColsEnabled"; sqliteSubNode.Tag = cd.SQLite_GridColsEnabled;
            sqliteNode.Nodes.Add(sqliteSubNode);

            CodeNodes.Nodes.Add(sqliteNode);




            // return region
            cd.SQLite_Select = CodeGen_Select(colNames, ObjectName, TableName, "");
            cd.SQLite_Object = CodeGen_Object(colNames, ObjectName, TableName, "");
            cd.SQLite_ObjectList = CodeGen_ObjectList(colNames, TableName, ObjectName);
            cd.Region_SQLite_Return = "\t#region ===== " + ObjectName + " SQLite : Return =====\n\r" + cd.SQLite_Object + "\n" + cd.SQLite_ObjectList + "\n" + cd.SQLite_Select + "\n\r\t#endregion";

            // insert/update region
            cd.SQLite_CreateTable = CodeGen_SQLite_CreateTable(colNames, TableName);
            cd.SQLite_Insert = CodeGen_Insert(colNames, TableName, ObjectName);
            cd.SQLite_Update = CodeGen_Update(colNames, TableName, ObjectName);
            cd.SQLite_UpdateInsert = CodeGen_UpdateInsert(colNames, TableName, ObjectName);
            cd.Region_SQLite_Update = "\t#region ===== " + ObjectName + " SQLite : Update Insert =====\n\r" + cd.SQLite_Insert + "\n" + cd.SQLite_Update + "\n" + cd.SQLite_UpdateInsert + "\n\r\t#endregion";

            cd.SQLite_ReturnDatatable = CodeGen_ReturnDataTable(colNames, TableName, ObjectName);
            cd.SQLite_CreateDatatable = CodeGen_CreateDataTable(colNames, TableName, ObjectName);
            cd.Region_DataTable = "\t#region ===== " + ObjectName + " DataTable =====\n\r" + cd.SQLite_ReturnDatatable + "\n" + cd.SQLite_CreateDatatable + "\n\r\t#endregion";

            cd.SQLite_GridColsVisible = CodeGen_ColumnsVisible(colNames, TableName);
            cd.SQLite_GridColsEnabled = CodeGen_ColumnsEnabled(colNames, TableName);
            cd.Region_DataGridView = "\t#region ===== " + ObjectName + " DataGridView =====\n\r" + cd.SQLite_GridColsVisible + "\n" + cd.SQLite_GridColsEnabled + "\n\r\t#endregion";


            cd.SQL_Insert = CodeGen_SQL_Insert(colNames, TableName, ObjectName);
            cd.SQL_Update = CodeGen_SQL_Update(colNames, TableName, ObjectName, whereField);
            cd.SQL_UpdateInsert = CodeGen_SQL_UpdateInsert(colNames, TableName, ObjectName);
            cd.SQL_ObjectList = CodeGen_SQL_ObjectList(colNames, TableName, ObjectName);
            string UpdatePopulated = CodeGen_SQL_UpdateIfPopulated(colNames, TableName, ObjectName, whereField);
            string createSQLTable = CodeGen_SQL_CreateTable(colNames, TableName, ObjectName);
            string SQLConnection = CodeGen_SQL_Connection(ObjectName, cd.TableNameFull, cd.ConnectionName);
            string returnSQLObj = CodeGen_SQL_Object(colNames, TableName, ObjectName);
            cd.SQL_Delete = CodeGen_SQL_Delete(colNames, TableName, ObjectName, whereField, cd.ConnectionName);

            cd.NewForm = CodeGen_NewForm(colNames, ObjectName, whereField);

            cd.SQL_To_SQLite = CodeGen_SQL_To_SQLite(colNames, TableName, ObjectName);

            cd.Region_SQL = cd.NewForm + "\n\n" + cd.Region_ButtonActions + "\n\n" + "\t#region ===== " + ObjectName + " SQL Functions =====\n\n" + SQLConnection + "\n" + createSQLTable + "\n" + cd.SQL_Insert + "\n" + cd.SQL_Update + "\n" + cd.SQL_UpdateInsert + "\n" + UpdatePopulated + "\n" + returnSQLObj + "\n" + cd.SQL_ObjectList + "\n" + cd.SQL_Delete + "\n" + cd.SQL_To_SQLite + "\n\r\t#endregion";



            // all regions together
            cd.AllCodeReturn = "#region === " + ObjectName + " FUNCTIONS ===\n\n" + cd.Region_Object + "\n" + cd.SQLite_CreateTable + "\n" + cd.Region_SQLite_Return + "\n" + cd.Region_SQLite_Update + "\n" + cd.Region_DataTable + "\n" + cd.Region_DataGridView + "\n" + cd.Region_SQL + "\n\r\t#endregion";


            cd.CodeNodes = CodeNodes; // assign code nodes to out var after assembly

            return cd;
        }

        public codeGn ReturnCodeGenDbCode(Dictionary<string, string> colNames, string objectName = "oBjectName", string tableName = "tableNAME", string whereColumn = "")
        {
            // populate variables to use in codegen
            codeGn gen = new codeGn();

            gen.ObjectName = objectName;
            gen.TableNameShort = tableName;
            gen.ColumnNames = colNames;
            gen.whereColumn = whereColumn;

            // populate code gen object
            gen = codeGenDbCode(gen);

            return gen;
        }


        // generate sql + sqlite db code
        public string Generate_ALL_Db_Code(Dictionary<string, string> colNames, string objectName = "oBjectName", string tableName = "tableNAME", string WHEREclause = "")
        {

            Dictionary<string, string> columns = new Dictionary<string, string>();

            // if no table name is provided, take the first item from the column list as the table name
            if (tableName == "")
            {
                int i = 1;
                foreach (string col in colNames.Keys)
                {
                    string val = ""; colNames.TryGetValue(col, out val);
                    if (i == 1) { tableName = col; }
                    if (i != 1) { columns.Add(col, val); }
                    i++;
                }
            }
            else
            {
                columns = colNames;
            }

            string sqlite = Generate_SQLite_Code(columns, objectName, tableName, WHEREclause);
            string sql = Generate_SQL_Code(columns, objectName, tableName);
            return sqlite + Environment.NewLine + sql;
        }

        public string Generate_SQLite_Code(Dictionary<string, string> colNames, string objectName = "oBjectName", string tableName = "tableNAME", string WHEREclause = "", string Section = "ALL")
        {
            string TableName = tableName;
            string ObjectName = objectName.Trim();
            ObjectName = ahk.StringReplace(ObjectName, " ", "_");

            //#### Generate Regions + Functions Underneath ####

            string regionSQLiteTable = "// ===== " + ObjectName + " SQLite : Table =====\n\r" + "\n" + CodeGen_SQLite_CreateTable(colNames, TableName); // +"\n\r\t#endregion";
            string fixObject = CodeGen_ObjectFixChars(colNames, ObjectName);
            string regionObject = "#region === " + ObjectName + " Object ===\n\r" + CodeGen_Return_ObjectDef(colNames, ObjectName) + "\n\n" + CodeGen_Return_ObjectPopulate(colNames, ObjectName) + "\n" + fixObject + "\n\r\t#endregion";
            // no use ? CodeGen_Return_Variables(colNames) + "\n\r"

            string regionSQLiteReturn = "// ===== " + ObjectName + " SQLite : Return =====\n\r";
            string selectCode = CodeGen_Select(colNames, ObjectName, TableName, "");
            string selectObject = CodeGen_Object(colNames, ObjectName, TableName, "");
            string selectObjectList = CodeGen_ObjectList(colNames, tableName, ObjectName);
            regionSQLiteReturn = regionSQLiteReturn + selectCode + "\n" + selectObject + "\n" + selectObjectList; // +"\n\r\t#endregion";


            string regionSQLiteUpdateInsert = "// ===== " + ObjectName + " SQLite : Update Insert ===\n";
            string insertCode = CodeGen_Insert(colNames, TableName, ObjectName);
            string updateCode = CodeGen_Update(colNames, TableName, ObjectName);
            string updateInsert = CodeGen_UpdateInsert(colNames, TableName, ObjectName);
            regionSQLiteUpdateInsert = regionSQLiteUpdateInsert + "\n\r" + updateInsert + "\n" + insertCode + "\n" + updateCode; // +"\n\r\t#endregion";

            //string regionCode = "#region === " + ObjectName + " Object\n\n" + regionObject + "#endregion\n\n\r";
            string regionCode = "#region === " + objectName + " SQLite ===\n\n" + regionSQLiteReturn + "\n\r" + regionSQLiteUpdateInsert + "\n\r" + regionSQLiteTable + "\n\n#endregion";


            string regionDataTable = "// ===== " + ObjectName + " DataTable =====\n";
            string returnDataTable = CodeGen_ReturnDataTable(colNames, TableName, ObjectName);
            string createDataTable = CodeGen_CreateDataTable(colNames, TableName, ObjectName);
            regionDataTable = regionDataTable + "\n" + createDataTable + "\n" + returnDataTable; // +"\n\r#endregion";


            string regionDataGridView = "// ===== " + ObjectName + " DataGridView =====\n";
            string columnsVisible = CodeGen_ColumnsVisible(colNames, TableName);
            string columnsEnabled = CodeGen_ColumnsEnabled(colNames, TableName);
            regionDataGridView = regionDataGridView + "\n" + columnsVisible + "\n\r" + columnsEnabled; // +"\n\r#endregion";

            string DataGridREGION = "\n#region === " + ObjectName + " DataGrid ===\n\n" + regionDataTable + "\n\n" + regionDataGridView + "\n\n#endregion\n";


            regionCode = regionObject + "\n\n" + regionCode; // +"\n\n" + regionDataGridView + "\n\r" + regionDataTable;

            //// return with SQL code as well
            //string regionSQL = Generate_SQL_Code(colNames, objectName, tableName);
            //regionCode = "#region === " + objectName + " ===\n\n" + regionCode + regionSQL + "\n" + DataGridREGION + "\n\n#endregion\n";

            regionCode = "#region === " + objectName + " ===\n\n" + regionCode + "\n" + DataGridREGION + "\n\n#endregion\n";


            return regionCode;
        }

        // generate SQL db functions - returns Region of code with functions
        public string Generate_SQL_Code(Dictionary<string, string> colNames, string objectName = "oBjectName", string tableName = "tableNAME")
        {
            string regionSQL = "\t\n\n#region === " + objectName + " SQL Functions ===\n\n";
            regionSQL = regionSQL + CodeGen_SQL_Insert(colNames, tableName, objectName) + "\n" + CodeGen_SQL_Update(colNames, tableName, objectName, "");
            regionSQL = regionSQL + "\n" + CodeGen_SQL_UpdateInsert(colNames, tableName, objectName) + "\n" + CodeGen_SQL_ObjectList(colNames, tableName, objectName);

            regionSQL = regionSQL + "\n" + CodeGen_SQL_Object(colNames, tableName, objectName);
            regionSQL = regionSQL + "\n" + CodeGen_SQL_DeclareGUI(colNames, tableName, objectName);

            regionSQL = regionSQL + "\t\n#endregion";
            return regionSQL;
        }



    }
}
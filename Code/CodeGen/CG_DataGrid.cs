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

        private string CodeGen_ColumnsEnabled(Dictionary<string, string> ColNames, string tableName = "TableName")
        {
            string ShowColumns = "\t\tpublic void Enable_" + tableName + "_Columns(DataGridView dv)\n{\n";

            int i = 0;
            foreach (string col in ColNames.Keys)
            {
                //if (i == 0) { i++; continue; } // skip the parent node
                ShowColumns = ShowColumns + "\n\t\ttry { dv.Columns[\"" + col + "\"].ReadOnly = true; } catch { }";
            }

            ShowColumns = ShowColumns + "\n}";

            return ShowColumns;
        }

        private string CodeGen_ColumnsVisible(Dictionary<string, string> ColNames, string tableName = "TableName")
        {
            string ShowColumns = "\t\tpublic void HideShow_" + tableName + "_Columns(DataGridView dv)\n{\n";

            int i = 0;
            foreach (string col in ColNames.Keys)
            {
                //if (i == 0) { i++; continue; } // skip the parent node
                ShowColumns = ShowColumns + "\n\t\ttry { dv.Columns[\"" + col + "\"].Visible = true; } catch { }";
            }

            ShowColumns = ShowColumns + "\n}";
            return ShowColumns;
        }


    }
}
using sharpAHK;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
//using System.Web;
//using System.Configuration;
using sharpAHK_Dev;
using System.Net.Mail;
using Telerik.WinControls.UI;
using System.Collections;
using Telerik.WinControls;
using TelerikExpressions;
using AHKExpressions;
using Telerik.WinControls.UI.Docking;
using System.Reflection;

namespace sharpAHK_Dev
{
    public partial class _TelerikLib
    {
        public class RadGrid
        {
            #region === Startup ===

            _Database.goDaddy goDad = new _Database.goDaddy();
            sharpAHK._AHK ahk = new sharpAHK._AHK();
            _Database.SQL sql = new _Database.SQL();
            _Database.SQLite sqlite = new _Database.SQLite();
            _GridControl grid = new _GridControl();
            _Lists lst = new _Lists();
            _Dict dict = new _Dict();
            _Images img = new _Images();
            _WinForms frm = new _WinForms();
            _TreeViewControl tv = new _TreeViewControl();
            _TabControl tab = new _TabControl();
            _MenuControl menu = new _MenuControl();
            _ScintillaControl sci = new _ScintillaControl();
            _Apps.Mpc mpc = new _Apps.Mpc();
            _Apps.Chrome cr = new _Apps.Chrome();
            _Code code = new _Code();
            _Database.Tags tags = new _Database.Tags();
            _Database.dir2Db dir2 = new _Database.dir2Db();
            _Parse prs = new _Parse();
            _Database.dir2Db dirDb = new _Database.dir2Db();
            _StatusBar sb = new _StatusBar();


            #endregion

            #region === ROWS ===

            // when row is added/removed, fires to update gui with grid row count
            public void OnRowsChanged(RadGridView RadGrid, RadLabel gridRowCountDisp)
            {
                if (RadGrid == null) { return; }
                if (gridRowCountDisp == null) { return; }

                //RadGridView clicked = (RadGridView)sender;
                //RadLabel gridRowCountDisp = lblGrid;  // Define Label 

                int rowcount = 0;

                // return grid row count
                if (RadGrid.InvokeRequired) { RadGrid.BeginInvoke((MethodInvoker)delegate () { rowcount = RadGrid.Rows.Count(); }); }
                else { rowcount = RadGrid.Rows.Count(); }


                // update label with grid row count
                if (gridRowCountDisp.InvokeRequired) { gridRowCountDisp.BeginInvoke((MethodInvoker)delegate () { gridRowCountDisp.Text = rowcount + " Rows"; }); }
                else { gridRowCountDisp.Text = rowcount + " Rows"; }
            }


            // disable edit mode before reading user's input from grid
            public void EndEdit(RadGridView RadGrid)
            {
                if (RadGrid == null) { return; }

                RadGrid.EndEdit();
            }

            // select last row in radGridView
            public void Select_LastRow(RadGridView RadGrid)
            {
                if (RadGrid == null) { return; }
                GridViewRowInfo lastRow1 = RadGrid.Rows[RadGrid.Rows.Count - 1];
                lastRow1.IsSelected = true;
            }

            // select specific row number in radGridView
            public void Select_Row(RadGridView RadGrid, int RowNum = 0)
            {
                if (RadGrid == null) { return; }
                GridViewRowInfo lastRow1 = RadGrid.Rows[RowNum];
                lastRow1.IsSelected = true;
            }

            public void AddRow(RadGridView RadGrid)
            {
                if (RadGrid == null) { return; }
                RadGrid.Rows.Add("Adding New Row", 12.5, DateTime.Now, true);
            }

            // start loop on new thread to add new rows to grid without locking the GUI
            public void AddRowsThread(RadGridView RadGrid, int AddCount = 10)
            {
                if (RadGrid == null) { return; }
                Thread addThread = new Thread(() => _AddRowsThread(RadGrid, AddCount));
                addThread.Start();
            }

            public void _AddRowsThread(RadGridView RadGrid, int AddCount = 10)
            {
                if (RadGrid == null) { return; }
                int i = 0;
                do
                {
                    if (RadGrid.InvokeRequired)  // if currently on a different thread, invoke 
                    {
                        RadGrid.BeginInvoke((MethodInvoker)delegate () { RadGrid.Rows.Add("Adding New Row " + i, 12.5, DateTime.Now, true); });
                    }
                    else
                    {
                        RadGrid.Rows.Add("Adding New Row " + i, 12.5, DateTime.Now, true);
                    }

                    ahk.Sleep(200);
                    i++;
                } while (i < AddCount);

            }


            /// <summary>
            /// Extend Height of Rows, When No Height Provided it Fills Entire Visible Grid
            /// </summary>
            /// <param name="RadGrid"></param>
            /// <param name="RowHeight"></param>
            public void Fill_Row_Height(RadGridView RadGrid, int RowHeight = -1)
            {
                int gridH = RadGrid.Height;
                int gridW = RadGrid.Width;

                int rowCount = RadGrid.Rows.Count;
                int colCount = RadGrid.Columns.Count;

                int remainder = ahk.Remainder(gridH, rowCount);  // remaining value not divisible in whole numbers

                int cellHeight = (gridH - remainder) / rowCount;  // height of each cell to populate entire display


                int i = 0;
                foreach (GridViewRowInfo rowInfo in RadGrid.Rows)
                {
                    if (RowHeight != -1) { rowInfo.Height = RowHeight; }
                    else { rowInfo.Height = cellHeight; }
                    i++;
                }
            }


            #endregion

            #region === COLUMNS ===

            // Loop through each column in grid
            public void Column_Loop(RadGridView RadGrid, int ColNum = 0)
            {
                if (RadGrid == null) { return; }
                int i = 0;
                foreach (GridViewColumn column in RadGrid.Columns)
                {
                    if (column is GridViewDataColumn)
                    {
                        GridViewDataColumn col = column as GridViewDataColumn;
                        if (col != null && ColNum == i)
                        {
                            col.IsVisible = false;
                        }

                        i++;
                    }
                }
            }

            public void Column_WidthLoop(RadGridView RadGrid)
            {
                if (RadGrid == null) { return; }
                int i = 0;
                foreach (GridViewColumn column in RadGrid.Columns)
                {
                    if (column is GridViewDataColumn)
                    {
                        GridViewDataColumn col = column as GridViewDataColumn;
                        int Width = col.Width;
                        ahk.MsgBox("Column " + i + " = " + Width);
                    }

                    i++;
                }
            }



            public void Fill_Column_Width(RadGridView RadGrid, bool AutoSizeRows = false)
            {
                if (RadGrid == null) { return; }

                RadGrid.AutoSizeRows = AutoSizeRows;

                if (RadGrid.InvokeRequired) { RadGrid.BeginInvoke((MethodInvoker)delegate () { RadGrid.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill; }); }
                else { RadGrid.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill; }

                if (RadGrid.InvokeRequired)
                {
                    RadGrid.BeginInvoke((MethodInvoker)delegate ()
                    {
                        RadGrid.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill; ;
                    });
                }
                else
                {
                    RadGrid.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill; ;
                }

                try
                {
                    DataGridViewTextBoxColumn subTitleColumn = new DataGridViewTextBoxColumn();
                    subTitleColumn.MinimumWidth = 50;
                    subTitleColumn.FillWeight = 100;

                    DataGridViewTextBoxColumn summaryColumn = new DataGridViewTextBoxColumn();
                    summaryColumn.MinimumWidth = 50;
                    summaryColumn.FillWeight = 200;

                    DataGridViewTextBoxColumn contentColumn = new DataGridViewTextBoxColumn();
                    contentColumn.MinimumWidth = 50;
                    contentColumn.FillWeight = 300;
                }
                catch
                {
                    ahk.MsgBox("Fill Column Width Catch Msg");
                }
            }


            /// <summary>Hide (by Column Number) in DataGridView</summary>
            /// <param name="RadGrid"> </param>
            /// <param name="ColNumber"> </param>
            public void HideColumn(RadGridView RadGrid, int ColNumber)
            {
                if (RadGrid == null) { return; }

                // update control text (from any thread) -- [ works in dll ]
                if (RadGrid.InvokeRequired) { RadGrid.BeginInvoke((MethodInvoker)delegate () { try { RadGrid.Columns[ColNumber].IsVisible = false; } catch { } }); }
                else { try { RadGrid.Columns[ColNumber].IsVisible = false; } catch { } }
            }

            /// <summary>Show (by Column Number) in DataGridView</summary>
            /// <param name="dv"> </param>
            /// <param name="ColNumber"> </param>
            public void ShowColumn(RadGridView RadGrid, int ColNumber)
            {
                if (RadGrid == null) { return; }

                // update control text (from any thread) -- [ works in dll ]
                if (RadGrid.InvokeRequired) { RadGrid.BeginInvoke((MethodInvoker)delegate () { try { RadGrid.Columns[ColNumber].IsVisible = true; } catch { } }); }
                else { try { RadGrid.Columns[ColNumber].IsVisible = true; } catch { } }
            }


            /// <summary>
            /// Save Width of Each Column in RadGrid to SQLite Setting to Restore Later
            /// </summary>
            /// <param name="RadGrid"></param>
            /// <param name="GridName"></param>
            public void Save_RadGridColumnWidth(RadGridView RadGrid, string GridName = "RadGrid")
            {
                string Widths = "";
                int i = 0;
                foreach (GridViewColumn column in RadGrid.Columns)
                {
                    if (column is GridViewDataColumn)
                    {
                        GridViewDataColumn col = column as GridViewDataColumn;
                        int Width = col.Width;
                        if (Widths == "") { Widths = col.Name + "|" + Width; }
                        else { Widths = Widths + "\n" + col.Name + "|" + Width; }
                    }
                    i++;
                }

                sqlite.Setting(GridName + "_Widths", Widths);
            }

            /// <summary>
            /// Read Saved RadGrid Column Widths and Restore Widths in RadGridView
            /// </summary>
            /// <param name="RadGrid"></param>
            /// <param name="GridName"></param>
            public void Restore_RadGridColumnWidth(RadGridView RadGrid, string GridName = "RadGrid")
            {
                Fill_Column_Width(RadGrid);

                string Widths = sqlite.Setting(GridName + "_Widths");  // return list of column widths saved to sqlite settings

                if (Widths.Trim() != "")
                {
                    List<string> widths = ahk.StringSplit_List(Widths, "\n");

                    if (widths.Count > 0)
                    {
                        int i = 0;
                        foreach (GridViewColumn column in RadGrid.Columns)
                        {
                            if (column is GridViewDataColumn)
                            {
                                string width = widths[i];
                                int w = ahk.StringSplit(width, "|", 1).ToInt();
                                column.Width = w; i++;
                            }
                        }
                    }
                }
            }



            #endregion

            #region === RadGridView: Populate ===

            // clear out previous gridview content
            public void Clear(RadGridView RadGrid)
            {
                if (RadGrid == null) { return; }

                if (RadGrid.InvokeRequired)  // if currently on a different thread, invoke 
                {
                    RadGrid.BeginInvoke((MethodInvoker)delegate () { RadGrid.DataSource = null; });
                }
                else
                {
                    RadGrid.DataSource = null;
                }
            }


            /// <summary>Populate DataGrid with SQL Search Results (returns Row Count) (option to add column with checkboxes to search result grid)</summary>
            /// <param name="RadGrid"> </param>
            /// <param name="conn"> </param>
            /// <param name="SQLQuery"> </param>
            /// <param name="AddCheckBoxColumn"> </param>
            public int SQL(RadGridView RadGrid, SqlConnection conn, string SQLQuery, bool AddCheckBoxColumn = false)
            {
                if (RadGrid == null) { return 0; }

                bool ClearGrid = true;

                if (ClearGrid)
                {
                    // clear out datagrid contents
                    RadGrid.DataSource = null;
                    RadGrid.Rows.Clear();
                    RadGrid.Columns.Clear();
                    RadGrid.AutoGenerateColumns = true;
                }

                DataTable SQL = new DataTable();
                SQL = sql.GetDataTable(conn, SQLQuery);

                if (AddCheckBoxColumn)  // option to add new column with checkboxes to SQL search results
                {
                    SQL.Columns.Add(new DataColumn("Selected", typeof(bool))); //this will show checkboxes in Selected Column
                }


                RadGrid.DataSource = SQL;
                return RadGrid.RowCount;
            }

            /// <summary>Populate DataGriRadGridiew with SQLite Search Results (returns Row Count) (option to add column with checkboxes to search result grid)</summary>
            /// <param name="RadGrid"> </param>
            /// <param name="DbFile"> </param>
            /// <param name="SQLiteCommand"> </param>
            /// <param name="AddCheckBoxColumn"> </param>
            /// <param name="CheckBoxColText"> </param>
            public int SQLite(RadGridView RadGrid, string DbFile, string SQLiteCommand, bool AddCheckBoxColumn = false, string CheckBoxColText = "Selected")
            {
                if (RadGrid == null) { return 0; }

                SQLiteConnection db = sqlite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

                try
                {
                    DataTable dt = new DataTable();
                    var da = new SQLiteDataAdapter(SQLiteCommand, db);  // search SQLite DB
                    da.Fill(dt);

                    if (AddCheckBoxColumn)  // option to add additional column with check boxes to sqlite search results being displayed
                    {
                        dt.Columns.Add(new DataColumn(CheckBoxColText, typeof(bool))); //this will show new checkbox column in grid
                    }


                    // assign the DataGriRadGridiew Name to Populate
                    //RadGrid.DataSource = dt; 

                    PopulateGrid(RadGrid, dt); // populate grid from any thread
                }
                catch (Exception ex)
                {
                    MessageBox.Show("SQLite Exception Catch Here:\n\r" + ex.ToString());
                }


                if (AddCheckBoxColumn)
                {
                    //Change_Column_Position(RadGrid, CheckBoxColText, 0);  // move this column to the first position in the column
                }

                //=== update gui with grid row count ====================================

                int GridRowCount = 0;

                if (RadGrid.InvokeRequired)  // if currently on a different thread, invoke datagrid first
                {
                    RadGrid.BeginInvoke((MethodInvoker)delegate () { GridRowCount = RadGrid.RowCount; });
                    RadGrid.BeginInvoke((MethodInvoker)delegate () { RadGrid.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill; });
                    //RadGrid.BeginInvoke((MethodInvoker)delegate () { RadGrid.AutoSizeColumnsMode = DataGriRadGridiewAutoSizeColumnsMode.AllCells; });

                }
                else
                {
                    GridRowCount = RadGrid.RowCount;
                    RadGrid.AutoSizeColumnsMode = GridViewAutoSizeColumnsMode.Fill;
                }


                //GridRowCount = GridRowCount - 1;  // adjust for additional row counted
                //lblGridCount.Text = GridRowCount.ToString() + " Shows"; //update gui with Row Count in Grid


                //ColumnView(1);  // Set the GriRadGridiew Column Layout (all fields fields)
                //ColumnView(2);  // Set the GriRadGridiew Column Layout (specific fields)

                // Resize the master DataGriRadGridiew columns to fit the newly loaded data.



                // Configure the details DataGriRadGridiew so that its columns automatically 
                // adjust their widths when the data changes.


                //_Database.SQLite lite = new _Database.SQLite();
                sqlite.Disconnect(db);  // free up db for other use

                return GridRowCount;
            }

            /// <summary>Update DataGriRadGridiew from DataTable on Any/Current Thread</summary>
            /// <param name="RadGrid"> </param>
            /// <param name="dt"> </param>
            public void PopulateGrid(RadGridView RadGrid, DataTable dt)
            {
                if (RadGrid == null) { return; }

                if (RadGrid.InvokeRequired)  // if currently on a different thread, invoke 
                {
                    RadGrid.BeginInvoke((MethodInvoker)delegate () { RadGrid.DataSource = dt; });
                }
                else  // otherwise populate the grid
                {
                    RadGrid.DataSource = dt;
                }
            }


            // Populate Grid with Folder Paths
            public void DirList(string DirPath, RadGridView RadGrid, bool Recurse = false)
            {
                if (RadGrid == null) { return; }

                List<string> DirList = lst.DirList(DirPath, "*.*", Recurse, true);

                List<DirObj> list = new List<DirObj>();

                if (DirList != null)
                {
                    foreach (string dir in DirList)
                    {
                        DirObj dirP = new DirObj(ahk.DirName(dir), dir);
                        list.Add(dirP);
                    }
                }


                RadGrid.DataSource = list;

                Fill_Column_Width(RadGrid);
            }



            #endregion

        }

    }
}



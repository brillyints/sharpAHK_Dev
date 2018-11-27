using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Data.SQLite;
using System.Data.SqlClient;
using sharpAHK;
using System.Drawing;
using ScintillaNET;
using System.ComponentModel;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Drawing.Imaging;
using System.Xml.Serialization;
using System.Threading;
using System.Reflection;
using System.Net;
using System.Windows.Automation;
using System.Timers;
using System.Configuration;

namespace sharpAHK_Dev
{

        // Background Image in Grid Class (works)

        public class MyGrid : DataGridView
        {
            // build MyGrid example
            public void BuildGrid()
            {
                //Dev.Grid grd = new Dev.Grid(); 

                //Image returnImg;   // populate image

                //// create new datagrid 
                //MyGrid grid = new MyGrid();
                //this.Controls.Add(grid);

                //DataTable dt = new DataTable();
                //dt.Columns.Add("id");
                //dt.Columns.Add("name");
                //dt.Rows.Add("01", "Jim");
                //dt.Rows.Add("02", "Peter");
                //grid.DataSource = dt;

                //grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
                //grid.Location = new System.Drawing.Point(152, 24);
                ////grid.Name = "dataGridView2";
                //grid.Size = new System.Drawing.Size(521, 277);
                ////grid.TabIndex = 1;


                //grid.BackgroundImage = returnImg;
                //grid.SetCellsTransparent();

                //grid.BringToFront();

                //grd.Row_Color(grid, Color.White, Color.Transparent, -1);
                //grd.Fill_Column_Width(grid); 

            }

            private Image _backgroundPic;

            [Browsable(true)]
            public override Image BackgroundImage
            {
                get { return _backgroundPic; }
                set { _backgroundPic = value; }
            }

            protected override void PaintBackground(System.Drawing.Graphics graphics, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle gridBounds)
            {
                base.PaintBackground(graphics, clipBounds, gridBounds);

                if (((this.BackgroundImage != null)))
                {
                    graphics.FillRectangle(Brushes.Black, gridBounds);
                    graphics.DrawImage(this.BackgroundImage, gridBounds);
                }
            }

            //Make BackgroundImage can be seen in all cells
            public void SetCellsTransparent()
            {
                this.EnableHeadersVisualStyles = false;

                this.ColumnHeadersDefaultCellStyle.BackColor = Color.Transparent;

                this.RowHeadersDefaultCellStyle.BackColor = Color.Transparent;

                foreach (DataGridViewColumn col in this.Columns)
                {
                    col.DefaultCellStyle.BackColor = Color.Transparent;
                }
            }
        }
        namespace DataGridViewExtensions    // save / load grid config (example)
        {
            [Serializable]
            public sealed class ColumnInfo
            {
                public string Name { get; set; }
                public int DisplayIndex { get; set; }
                public int Width { get; set; }
                public bool Visible { get; set; }
            }

            public static class DataGridViewExtenstions
            {
                /// <summary>
                /// Loads columns information from the specified XML file
                /// </summary>
                /// <param name="dgv">DataGridView control instance</param>
                /// <param name="fileName">XML configuration file</param>
                public static void LoadConfiguration(this DataGridView dgv, string fileName)
                {
                    List<ColumnInfo> columns;
                    using (var streamReader = new StreamReader(fileName))
                    {
                        var xmlSerializer = new XmlSerializer(typeof(List<ColumnInfo>));
                        columns = (List<ColumnInfo>)xmlSerializer.Deserialize(streamReader);
                    }
                    foreach (var column in columns)
                    {
                        dgv.Columns[column.Name].DisplayIndex = column.DisplayIndex;
                        dgv.Columns[column.Name].Width = column.Width;
                        dgv.Columns[column.Name].Visible = column.Visible;
                    }
                }

                /// <summary>
                /// Saves columns information to the specified XML file
                /// </summary>
                /// <param name="dgv">DataGridView control instance</param>
                /// <param name="fileName">XML configuration file</param>
                public static void SaveConfiguration(this DataGridView dgv, string fileName)
                {
                    var columns = new List<ColumnInfo>();
                    for (int i = 0; i < dgv.Columns.Count; i++)
                    {
                        var column = new ColumnInfo();
                        column.Name = dgv.Columns[i].Name;
                        column.DisplayIndex = dgv.Columns[i].DisplayIndex;
                        column.Width = dgv.Columns[i].Width;
                        column.Visible = dgv.Columns[i].Visible;
                        columns.Add(column);
                    }
                    using (var streamWriter = new StreamWriter(fileName))
                    {
                        var xmlSerializer = new XmlSerializer(typeof(List<ColumnInfo>));
                        xmlSerializer.Serialize(streamWriter, columns);
                    }
                }
            }
        }
        public class ControlNavigationHelper  // used to find control under mouse position
        {
            public static Control getYoungestChildUnderMouse(Control topControl)
            {
                return ControlNavigationHelper.getYoungestChildAtDesktopPoint(topControl, System.Windows.Forms.Cursor.Position);
            }

            private static Control getYoungestChildAtDesktopPoint(Control topControl, System.Drawing.Point desktopPoint)
            {
                Control foundControl = topControl.GetChildAtPoint(topControl.PointToClient(desktopPoint));

                if ((foundControl != null) && (foundControl.HasChildren) && (foundControl.Visible))
                {
                    Control con = getYoungestChildAtDesktopPoint(foundControl, desktopPoint);
                    IContainerControl foundControler = topControl.GetContainerControl();
                    return con;
                }
                else
                {
                    return foundControl;
                }


            }
        }
        public class EventDatum  // Library to Extract Event Handlers From Controls 
        {
            public EventDescriptor _eventDesc;
            public Delegate _event;
            private static MethodInfo GetEventsMethod(Type objType)
            {
                MethodInfo mi = objType.GetMethod("get_Events", All);
                if ((mi == null) & (objType.BaseType != null))
                    mi = GetEventsMethod(objType.BaseType);
                return mi;
            }

            private static EventHandlerList GetEvents(object obj)
            {
                MethodInfo mi = GetEventsMethod(obj.GetType());
                if (mi == null) return null;
                return (EventHandlerList)mi.Invoke(obj, new object[] { });
            }

            private static FieldInfo GetEventIDField(Type objType, string eventName)
            {
                FieldInfo fi = objType.GetField("Event" + eventName, All);
                if ((fi == null) & (objType.BaseType != null))
                    fi = GetEventIDField(objType.BaseType, eventName);
                return fi;
            }

            private static object GetEventID(object obj, string eventName)
            {
                FieldInfo fi = GetEventIDField(obj.GetType(), eventName);
                if (fi == null) return null;
                return fi.GetValue(obj);
            }

            private static BindingFlags All
            {
                get
                {
                    return
                        BindingFlags.Public | BindingFlags.NonPublic |
                        BindingFlags.Instance | BindingFlags.IgnoreCase |
                        BindingFlags.Static;
                }
            }

            internal EventDatum(EventDescriptor desc, Delegate aEvent)
            {
                _eventDesc = desc;
                _event = aEvent;
            }

            public static EventDatum Create(object obj, EventDescriptor desc)
            {
                EventHandlerList list = GetEvents(obj);
                if (list == null) return null;
                object key = GetEventID(obj, desc.Name);
                if (key == null) return null;
                Delegate evnt = list[key];
                if (evnt == null) return null;
                return new EventDatum(desc, evnt);
            }

            public void Wire(object obj)
            {
                _eventDesc.AddEventHandler(obj, _event);
            }

            public void Unwire(object obj)
            {
                _eventDesc.RemoveEventHandler(obj, _event);
            }
        }
        public static class GRID_Disp
        {
            public static void Stretch_Width_Full(DataGridView dataGridView)  //extends column width to the edge of the control
            {
                if (dataGridView.Columns.Count > 0)
                {
                    var lastColIndex = dataGridView.Columns.Count - 1;
                    var lastCol = dataGridView.Columns[lastColIndex];
                    lastCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }

        }


}

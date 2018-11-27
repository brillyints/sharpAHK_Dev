using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Globalization;
using Telerik.WinForms;
using Telerik.WinControls.UI;
using TelerikExpressions;
using System.Net.Mail;
using Telerik.WinForms.Documents.Model.Code;
using System.Web.Script.Serialization;
using AHKExpressions;
using System.Xml;
using HtmlAgilityPack;
using System.Web;
using sharpAHK_Dev;
using static sharpAHK_Dev._WinForms;
using static sharpAHK_Dev._Sites.SceneSource;
using static sharpAHK_Dev._Apps.Mpc;
using static sharpAHK_Dev._Apps;
using System.ServiceProcess;
using System.Security.Principal;
using System.Management;
using static sharpAHK_Dev._Sites.EpGuides;


// ######################################
//      AHS = AutoHotkey.#Sharp
// ######################################

namespace sharpAHK_Dev
{
    
    #region === CODE ===

    public partial class _Code  //code gen functions
    {
        private static bool GlobalDebug = false;

        #region === Setup ===

        private static _AHK ahk = new _AHK();
        private static _Database.SQL sql = new _Database.SQL();
        private static _Database.SQLite sqlite = new _Database.SQLite();
        private static _GridControl grid = new _GridControl();
        private static _Lists lst = new _Lists();
        private static _TreeViewControl tv = new _TreeViewControl();
        private static _TelerikLib.RadTree tree = new _TelerikLib.RadTree();

        #endregion

        #region === Colors ===

        /// <summary> returns list of available application/control colors</summary>
        public List<string> ColorList()
        {
            List<string> Colors = new List<string>();
            Colors.Add("AliceBlue");
            Colors.Add("AntiqueWhite");
            Colors.Add("Aqua");
            Colors.Add("Aquamarine");
            Colors.Add("Azure");
            Colors.Add("Beige");
            Colors.Add("Bisque");
            Colors.Add("Black");
            Colors.Add("BlanchedAlmond");
            Colors.Add("Blue");
            Colors.Add("BlueViolet");
            Colors.Add("Brown");
            Colors.Add("BurlyWood");
            Colors.Add("CadetBlue");
            Colors.Add("Chartreuse");
            Colors.Add("Chocolate");
            Colors.Add("Coral");
            Colors.Add("CornflowerBlue");
            Colors.Add("Cornsilk");
            Colors.Add("Crimson");
            Colors.Add("Cyan");
            Colors.Add("DarkBlue");
            Colors.Add("DarkCyan");
            Colors.Add("DarkGoldenrod");
            Colors.Add("DarkGray");
            Colors.Add("DarkGreen");
            Colors.Add("DarkKhaki");
            Colors.Add("DarkMagenta");
            Colors.Add("DarkOliveGreen");
            Colors.Add("DarkOrange");
            Colors.Add("DarkOrchid");
            Colors.Add("DarkRed");
            Colors.Add("DarkSalmon");
            Colors.Add("DarkSeaGreen");
            Colors.Add("DarkSlateBlue");
            Colors.Add("DarkSlateGray");
            Colors.Add("DarkTurquoise");
            Colors.Add("DarkViolet");
            Colors.Add("DeepPink");
            Colors.Add("DeepSkyBlue");
            Colors.Add("DimGray");
            Colors.Add("DodgerBlue");
            Colors.Add("Firebrick");
            Colors.Add("FloralWhite");
            Colors.Add("ForestGreen");
            Colors.Add("Fuchsia");
            Colors.Add("Gainsboro");
            Colors.Add("GhostWhite");
            Colors.Add("Gold");
            Colors.Add("Goldenrod");
            Colors.Add("Gray");
            Colors.Add("Green");
            Colors.Add("GreenYellow");
            Colors.Add("Honeydew");
            Colors.Add("HotPink");
            Colors.Add("IndianRed");
            Colors.Add("Indigo");
            Colors.Add("Ivory");
            Colors.Add("Khaki");
            Colors.Add("Lavender");
            Colors.Add("LavenderBlush");
            Colors.Add("LawnGreen");
            Colors.Add("LemonChiffon");
            Colors.Add("LightBlue");
            Colors.Add("LightCoral");
            Colors.Add("LightCyan");
            Colors.Add("LightGoldenrodYellow");
            Colors.Add("LightGray");
            Colors.Add("LightGreen");
            Colors.Add("LightPink");
            Colors.Add("LightSalmon");
            Colors.Add("LightSeaGreen");
            Colors.Add("LightSkyBlue");
            Colors.Add("LightSlateGray");
            Colors.Add("LightSteelBlue");
            Colors.Add("LightYellow");
            Colors.Add("Lime");
            Colors.Add("LimeGreen");
            Colors.Add("Linen");
            Colors.Add("Magenta");
            Colors.Add("Maroon");
            Colors.Add("MediumAquamarine");
            Colors.Add("MediumBlue");
            Colors.Add("MediumOrchid");
            Colors.Add("MediumPurple");
            Colors.Add("MediumSeaGreen");
            Colors.Add("MediumSlateBlue");
            Colors.Add("MediumSpringGreen");
            Colors.Add("MediumTurquoise");
            Colors.Add("MediumVioletRed");
            Colors.Add("MidnightBlue");
            Colors.Add("MintCream");
            Colors.Add("MistyRose");
            Colors.Add("Moccasin");
            Colors.Add("NavajoWhite");
            Colors.Add("Navy");
            Colors.Add("OldLace");
            Colors.Add("Olive");
            Colors.Add("OliveDrab");
            Colors.Add("Orange");
            Colors.Add("OrangeRed");
            Colors.Add("Orchid");
            Colors.Add("PaleGoldenrod");
            Colors.Add("PaleGreen");
            Colors.Add("PaleTurquoise");
            Colors.Add("PaleVioletRed");
            Colors.Add("PapayaWhip");
            Colors.Add("PeachPuff");
            Colors.Add("Peru");
            Colors.Add("Pink");
            Colors.Add("Plum");
            Colors.Add("PowderBlue");
            Colors.Add("Purple");
            Colors.Add("Red");
            Colors.Add("RosyBrown");
            Colors.Add("RoyalBlue");
            Colors.Add("SaddleBrown");
            Colors.Add("Salmon");
            Colors.Add("SandyBrown");
            Colors.Add("SeaGreen");
            Colors.Add("SeaShell");
            Colors.Add("Sienna");
            Colors.Add("Silver");
            Colors.Add("SkyBlue");
            Colors.Add("SlateBlue");
            Colors.Add("SlateGray");
            Colors.Add("Snow");
            Colors.Add("SpringGreen");
            Colors.Add("SteelBlue");
            Colors.Add("Tan");
            Colors.Add("Teal");
            Colors.Add("Thistle");
            Colors.Add("Tomato");
            Colors.Add("Transparent");
            Colors.Add("Turquoise");
            Colors.Add("Violet");
            Colors.Add("Wheat");
            Colors.Add("White");
            Colors.Add("WhiteSmoke");
            Colors.Add("Yellow");
            Colors.Add("YellowGreen");

            return Colors;
        }

        /// <summary> return System Color from its Color Name</summary>
        /// <param name="ColorName"> </param>
        public Color Return_Color(string ColorName)
        {
            if (ColorName == "AliceBlue") { return Color.AliceBlue; }
            if (ColorName == "AntiqueWhite") { return Color.AntiqueWhite; }
            if (ColorName == "Aqua") { return Color.Aqua; }
            if (ColorName == "Aquamarine") { return Color.Aquamarine; }
            if (ColorName == "Azure") { return Color.Azure; }
            if (ColorName == "Beige") { return Color.Beige; }
            if (ColorName == "Bisque") { return Color.Bisque; }
            if (ColorName == "Black") { return Color.Black; }
            if (ColorName == "BlanchedAlmond") { return Color.BlanchedAlmond; }
            if (ColorName == "Blue") { return Color.Blue; }
            if (ColorName == "BlueViolet") { return Color.BlueViolet; }
            if (ColorName == "Brown") { return Color.Brown; }
            if (ColorName == "BurlyWood") { return Color.BurlyWood; }
            if (ColorName == "CadetBlue") { return Color.CadetBlue; }
            if (ColorName == "Chartreuse") { return Color.Chartreuse; }
            if (ColorName == "Chocolate") { return Color.Chocolate; }
            if (ColorName == "Coral") { return Color.Coral; }
            if (ColorName == "CornflowerBlue") { return Color.CornflowerBlue; }
            if (ColorName == "Cornsilk") { return Color.Cornsilk; }
            if (ColorName == "Crimson") { return Color.Crimson; }
            if (ColorName == "Cyan") { return Color.Cyan; }
            if (ColorName == "DarkBlue") { return Color.DarkBlue; }
            if (ColorName == "DarkCyan") { return Color.DarkCyan; }
            if (ColorName == "DarkGoldenrod") { return Color.DarkGoldenrod; }
            if (ColorName == "DarkGray") { return Color.DarkGray; }
            if (ColorName == "DarkGreen") { return Color.DarkGreen; }
            if (ColorName == "DarkKhaki") { return Color.DarkKhaki; }
            if (ColorName == "DarkMagenta") { return Color.DarkMagenta; }
            if (ColorName == "DarkOliveGreen") { return Color.DarkOliveGreen; }
            if (ColorName == "DarkOrange") { return Color.DarkOrange; }
            if (ColorName == "DarkOrchid") { return Color.DarkOrchid; }
            if (ColorName == "DarkRed") { return Color.DarkRed; }
            if (ColorName == "DarkSalmon") { return Color.DarkSalmon; }
            if (ColorName == "DarkSeaGreen") { return Color.DarkSeaGreen; }
            if (ColorName == "DarkSlateBlue") { return Color.DarkSlateBlue; }
            if (ColorName == "DarkSlateGray") { return Color.DarkSlateGray; }
            if (ColorName == "DarkTurquoise") { return Color.DarkTurquoise; }
            if (ColorName == "DarkViolet") { return Color.DarkViolet; }
            if (ColorName == "DeepPink") { return Color.DeepPink; }
            if (ColorName == "DeepSkyBlue") { return Color.DeepSkyBlue; }
            if (ColorName == "DimGray") { return Color.DimGray; }
            if (ColorName == "DodgerBlue") { return Color.DodgerBlue; }
            if (ColorName == "Firebrick") { return Color.Firebrick; }
            if (ColorName == "FloralWhite") { return Color.FloralWhite; }
            if (ColorName == "ForestGreen") { return Color.ForestGreen; }
            if (ColorName == "Fuchsia") { return Color.Fuchsia; }
            if (ColorName == "Gainsboro") { return Color.Gainsboro; }
            if (ColorName == "GhostWhite") { return Color.GhostWhite; }
            if (ColorName == "Gold") { return Color.Gold; }
            if (ColorName == "Goldenrod") { return Color.Goldenrod; }
            if (ColorName == "Gray") { return Color.Gray; }
            if (ColorName == "Green") { return Color.Green; }
            if (ColorName == "GreenYellow") { return Color.GreenYellow; }
            if (ColorName == "Honeydew") { return Color.Honeydew; }
            if (ColorName == "HotPink") { return Color.HotPink; }
            if (ColorName == "IndianRed") { return Color.IndianRed; }
            if (ColorName == "Indigo") { return Color.Indigo; }
            if (ColorName == "Ivory") { return Color.Ivory; }
            if (ColorName == "Khaki") { return Color.Khaki; }
            if (ColorName == "Lavender") { return Color.Lavender; }
            if (ColorName == "LavenderBlush") { return Color.LavenderBlush; }
            if (ColorName == "LawnGreen") { return Color.LawnGreen; }
            if (ColorName == "LemonChiffon") { return Color.LemonChiffon; }
            if (ColorName == "LightBlue") { return Color.LightBlue; }
            if (ColorName == "LightCoral") { return Color.LightCoral; }
            if (ColorName == "LightCyan") { return Color.LightCyan; }
            if (ColorName == "LightGoldenrodYellow") { return Color.LightGoldenrodYellow; }
            if (ColorName == "LightGray") { return Color.LightGray; }
            if (ColorName == "LightGreen") { return Color.LightGreen; }
            if (ColorName == "LightPink") { return Color.LightPink; }
            if (ColorName == "LightSalmon") { return Color.LightSalmon; }
            if (ColorName == "LightSeaGreen") { return Color.LightSeaGreen; }
            if (ColorName == "LightSkyBlue") { return Color.LightSkyBlue; }
            if (ColorName == "LightSlateGray") { return Color.LightSlateGray; }
            if (ColorName == "LightSteelBlue") { return Color.LightSteelBlue; }
            if (ColorName == "LightYellow") { return Color.LightYellow; }
            if (ColorName == "Lime") { return Color.Lime; }
            if (ColorName == "LimeGreen") { return Color.LimeGreen; }
            if (ColorName == "Linen") { return Color.Linen; }
            if (ColorName == "Magenta") { return Color.Magenta; }
            if (ColorName == "Maroon") { return Color.Maroon; }
            if (ColorName == "MediumAquamarine") { return Color.MediumAquamarine; }
            if (ColorName == "MediumBlue") { return Color.MediumBlue; }
            if (ColorName == "MediumOrchid") { return Color.MediumOrchid; }
            if (ColorName == "MediumPurple") { return Color.MediumPurple; }
            if (ColorName == "MediumSeaGreen") { return Color.MediumSeaGreen; }
            if (ColorName == "MediumSlateBlue") { return Color.MediumSlateBlue; }
            if (ColorName == "MediumSpringGreen") { return Color.MediumSpringGreen; }
            if (ColorName == "MediumTurquoise") { return Color.MediumTurquoise; }
            if (ColorName == "MediumVioletRed") { return Color.MediumVioletRed; }
            if (ColorName == "MidnightBlue") { return Color.MidnightBlue; }
            if (ColorName == "MintCream") { return Color.MintCream; }
            if (ColorName == "MistyRose") { return Color.MistyRose; }
            if (ColorName == "Moccasin") { return Color.Moccasin; }
            if (ColorName == "NavajoWhite") { return Color.NavajoWhite; }
            if (ColorName == "Navy") { return Color.Navy; }
            if (ColorName == "OldLace") { return Color.OldLace; }
            if (ColorName == "Olive") { return Color.Olive; }
            if (ColorName == "OliveDrab") { return Color.OliveDrab; }
            if (ColorName == "Orange") { return Color.Orange; }
            if (ColorName == "OrangeRed") { return Color.OrangeRed; }
            if (ColorName == "Orchid") { return Color.Orchid; }
            if (ColorName == "PaleGoldenrod") { return Color.PaleGoldenrod; }
            if (ColorName == "PaleGreen") { return Color.PaleGreen; }
            if (ColorName == "PaleTurquoise") { return Color.PaleTurquoise; }
            if (ColorName == "PaleVioletRed") { return Color.PaleVioletRed; }
            if (ColorName == "PapayaWhip") { return Color.PapayaWhip; }
            if (ColorName == "PeachPuff") { return Color.PeachPuff; }
            if (ColorName == "Peru") { return Color.Peru; }
            if (ColorName == "Pink") { return Color.Pink; }
            if (ColorName == "Plum") { return Color.Plum; }
            if (ColorName == "PowderBlue") { return Color.PowderBlue; }
            if (ColorName == "Purple") { return Color.Purple; }
            if (ColorName == "Red") { return Color.Red; }
            if (ColorName == "RosyBrown") { return Color.RosyBrown; }
            if (ColorName == "RoyalBlue") { return Color.RoyalBlue; }
            if (ColorName == "SaddleBrown") { return Color.SaddleBrown; }
            if (ColorName == "Salmon") { return Color.Salmon; }
            if (ColorName == "SandyBrown") { return Color.SandyBrown; }
            if (ColorName == "SeaGreen") { return Color.SeaGreen; }
            if (ColorName == "SeaShell") { return Color.SeaShell; }
            if (ColorName == "Sienna") { return Color.Sienna; }
            if (ColorName == "Silver") { return Color.Silver; }
            if (ColorName == "SkyBlue") { return Color.SkyBlue; }
            if (ColorName == "SlateBlue") { return Color.SlateBlue; }
            if (ColorName == "SlateGray") { return Color.SlateGray; }
            if (ColorName == "Snow") { return Color.Snow; }
            if (ColorName == "SpringGreen") { return Color.SpringGreen; }
            if (ColorName == "SteelBlue") { return Color.SteelBlue; }
            if (ColorName == "Tan") { return Color.Tan; }
            if (ColorName == "Teal") { return Color.Teal; }
            if (ColorName == "Thistle") { return Color.Thistle; }
            if (ColorName == "Tomato") { return Color.Tomato; }
            if (ColorName == "Transparent") { return Color.Transparent; }
            if (ColorName == "Turquoise") { return Color.Turquoise; }
            if (ColorName == "Violet") { return Color.Violet; }
            if (ColorName == "Wheat") { return Color.Wheat; }
            if (ColorName == "White") { return Color.White; }
            if (ColorName == "WhiteSmoke") { return Color.WhiteSmoke; }
            if (ColorName == "Yellow") { return Color.Yellow; }
            if (ColorName == "YellowGreen") { return Color.YellowGreen; }

            return Color.Black;
        }


        #endregion

        

 


        /// <summary>
        /// Populate TreeView with SharpAHK/SharpAHK_Dev Functions
        /// </summary>
        public class CodeTreeDisp
        {
            _AHK ahk = new _AHK();

            public funcParse ParseCSFunctionLine(string FunctionLine = "public string A_DesktopCommon(bool openDir = false)")
            {
                funcParse func = new funcParse();
                func.FunctionLine = FunctionLine;

                List<string> Segs = ahk.StringSplit_List(FunctionLine, " "); int i = 1;
                string firstWord = "";
                string secondWord = "";
                string thirdword = "";
                string functionName = ""; bool ParamsReached = false;

                string ExampleUse = "";
                string NextVarName = "";

                foreach (string seg in Segs)
                {
                    if (i == 1) { firstWord = seg; }
                    if (i == 2) { secondWord = seg; }
                    if (i == 3) { thirdword = seg; }

                    if (seg.Contains("("))
                    {
                        functionName = ahk.StringSplit(seg, "(", 0); ParamsReached = true; i++; continue;
                    }


                    if (ParamsReached)
                    {
                        string secondHalf = ahk.StringSplit(FunctionLine, "(", 1);
                        List<string> Params = ahk.StringSplit_List(secondHalf, " ");
                        foreach (string par in Params)
                        {
                            //ahk.MsgBox(par);
                        }
                    }

                    i++;
                }


                bool IsPublic = false;
                if (firstWord == "public") { IsPublic = true; }

                bool IsStatic = false;
                if (firstWord == "static" || secondWord == "static" || thirdword == "static") { IsStatic = true; }

                string OutputVarType = "";
                if (secondWord == "string" || thirdword == "string") { OutputVarType = "string"; }
                if (firstWord == "void" || secondWord == "void" || thirdword == "void") { OutputVarType = "Void"; }
                if (firstWord == "bool" || secondWord == "bool" || thirdword == "bool") { OutputVarType = "bool"; }
                if (firstWord == "List<string>" || secondWord == "List<string>" || thirdword == "List<string>") { OutputVarType = "List<string>"; }


                string DefaultVal = "";
                List<string> List = ahk.StringSplit_List(FunctionLine, "("); i = 0; bool DefaultFound = false;
                foreach (string seg in List)
                {
                    if (seg.Contains("="))
                    {
                        DefaultVal = ahk.StringSplit(seg, "=", 0).Trim();
                        DefaultFound = true; continue;
                    }

                }


                if (functionName.Contains("A_")) { NextVarName = functionName.Replace("A_", ""); }

                string SetupFunction = "_AHK ahk = new _AHK();";
                string SetupRef = ahk.StringSplit(SetupFunction, " ", 1);  // = ahk

                // dynamically generated example ready to test
                if (OutputVarType == "string")
                {
                    ExampleUse = SetupFunction + "\n" + OutputVarType + " " + NextVarName + " = " + SetupRef + functionName + "(" + DefaultVal + ");\nahk.MsgBox(\"Output = " + ahk.A_DesktopCommon() + "\");";
                }

                func.ClassRef = SetupRef;
                func.NewClassLine = SetupFunction;
                func.IsPublic = IsPublic;
                func.IsStatic = IsStatic;
                func.FunctionName = functionName;
                func.ReturnType = OutputVarType;
                func.ExampleUse = ExampleUse;
                //ahk.MsgBox(ExampleUse + "\n\nPublic = " + IsPublic + "\nStatic = " + IsStatic + "\nFunctionName = " + functionName + "\nOutVarType = " + OutputVarType);

                return func;
            }

            public struct funcParse
            {
                public string FunctionLine { get; set; }
                public string NewClassLine { get; set; }
                public string ClassRef { get; set; }
                public string FunctionName { get; set; }
                public string ReturnType { get; set; }
                public string ExampleUse { get; set; }
                public bool IsPublic { get; set; }
                public bool IsStatic { get; set; }
            }

            /// <summary>
            /// Populate Treeview with SharpAHK Expressions
            /// </summary>
            /// <param name="TreeView">WinFroms.TreeView Control</param>
            /// <returns>Returns List of Items Added to TreeView</returns>
            public List<string> sharpAhkExpressions(TreeView TreeView)
            {
                _Database.SQL sql = new _Database.SQL();
                _Lists lst = new _Lists();

                // Return FunctionList From DB
                string cmd = @"select Distinct [Method] FROM [codeserver].[lucidmethod].[CodeTable_SharpAHKdll] where [FileName] = 'AHK_Expressions.cs' and [Method] != '' Order by [Method]";
                List<string> Values = lst.SQL_To_List(sql.GetConn("SQLserver"), cmd);

                // Populates WinForms.TreeView
                if (TreeView.GetType().ToString() == "System.Windows.Forms.TreeView") { _TreeViewControl tv = new _TreeViewControl(); tv.Load_List(TreeView, Values, "sharpAHK Expressions"); }
                return Values;
            }

            /// <summary>
            /// Populate Treeview with SharpDLL Names
            /// </summary>
            /// <param name="TreeView">WinFroms.TreeView Control</param>
            /// <returns>Returns List of Items Added to TreeView</returns>
            public List<string> sharpDLLs(TreeView TreeView)
            {
                _Database.SQL sql = new _Database.SQL();
                _Lists lst = new _Lists();

                // Return FunctionList From DB
                string cmd = @"select distinct Filename FROM [codeserver].[lucidmethod].[CodeTable_SharpAHKdll]";
                List<string> Values = lst.SQL_To_List(sql.GetConn("SQLserver"), cmd);

                // Populates WinForms.TreeView
                if (TreeView.GetType().ToString() == "System.Windows.Forms.TreeView") { _TreeViewControl tv = new _TreeViewControl(); tv.Load_List(TreeView, Values, "Dll.Files"); }
                return Values;
            }

            /// <summary>
            /// Populate Treeview with SharpAHK_DEV Functions
            /// </summary>
            /// <param name="TreeView">WinFroms.TreeView Control</param>
            /// <returns>Returns List of Items Added to TreeView</returns>
            public List<string> sharpDevTree(object TreeView)
            {
                _Database.SQL sql = new _Database.SQL();
                _Lists lst = new _Lists();

                // Return FunctionList From DB
                string cmd = @"select Distinct [Method] FROM [codeserver].[lucidmethod].[CodeTable_SharpAHKdll] where [FileName] = 'sharpAHK_Dev.cs' and [Method] != '' Order by [Method]";
                List<string> Values = lst.SQL_To_List(sql.GetConn("SQLserver"), cmd);

                // Populates WinForms.TreeView
                if (TreeView.GetType().ToString() == "System.Windows.Forms.TreeView") { _TreeViewControl tv = new _TreeViewControl(); tv.Load_List((TreeView)TreeView, Values, "sharpAHK_DEV"); }

                // Populates Telerik.RadTreeView
                if (TreeView.GetType().ToString() == "Telerik.WinControls.UI.RadTreeView") { _TelerikLib.RadTree tv = new _TelerikLib.RadTree(); tv.ListRadTree((RadTreeView)TreeView, Values, "sharpAHK_DEV", "sharpAHK_DEV"); }

                return Values;
            }

            /// <summary>
            /// Populate Treeview with SharpAHK Functions
            /// </summary>
            /// <param name="TreeView">WinFroms.TreeView Control</param>
            /// <returns>Returns List of Items Added to TreeView</returns>
            public List<string> sharpAHKTree(object TreeView)
            {
                _Database.SQL sql = new _Database.SQL();
                _Lists lst = new _Lists();

                // Return FunctionList From DB
                string cmd = @"select Distinct [Method] FROM [codeserver].[lucidmethod].[CodeTable_SharpAHKdll] where [FileName] = 'AHK.cs' and [Method] != '' Order by [Method]";
                List<string> Values = lst.SQL_To_List(sql.GetConn("SQLserver"), cmd);

                // Populates WinForms.TreeView
                if (TreeView.GetType().ToString() == "System.Windows.Forms.TreeView") { _TreeViewControl tv = new _TreeViewControl(); tv.Load_List((TreeView)TreeView, Values, "sharpAHK"); }

                // Populates Telerik.RadTreeView
                if (TreeView.GetType().ToString() == "Telerik.WinControls.UI.RadTreeView") { _TelerikLib.RadTree tv = new _TelerikLib.RadTree(); tv.ListRadTree((RadTreeView)TreeView, Values, "sharpAHK", "sharpAHK"); }

                return Values;
            }

        }

    }



    public class MovedFromAHK
    {
        private static _AHK ahk = new _AHK();

        /// <summary>Returns path to the code / control example library</summary>
        public string CodeDir()
        {
            string DirPath = DevRoot() + @"\SQLiter\Code_Lib\bin\Debug\Code";
            return DirPath;
        }

        /// <summary>Returns path to the shared icon library</summary>
        public string ICOLib()
        {
            string ICO_LibDir = ahk.AppDir() + "\\Graphics";

            if (Directory.Exists(ICO_LibDir)) { ICO_LibDir = ahk.AppDir() + "\\ICO"; }
            return ICO_LibDir;
        }

        /// <summary>Returns path to the shared icon library database index</summary>
        public string ICOLib_Db()
        {
            string ICOLib_Db = DevRoot() + @"\SQLiter\Db\ICO_Lib.sqlite";
            return ICOLib_Db;
        }

        /// <summary>Returns the root path to dev projects based on pc being used</summary>
        public string DevRoot()
        {
            string DevRoot = @"C:\Users\LucidEdit\Dropbox\IMDB";
            if (Directory.Exists(DevRoot)) { DevRoot = @"C:\Users\LucidTop\Dropbox\IMDB"; }

            return DevRoot;
        }

        /// <summary>Returns the root AHK path based on pc being used</summary>
        public string AHKRoot()
        {
            string AHKDir = @"C:\Users\LucidTop\Google Drive\AHK";
            if (Directory.Exists(AHKDir)) { AHKDir = @"D:\Google Drive\AHK"; }

            return AHKDir;
        }


        //public static byte[] ToByteArray(this System.IO.Stream stream)
        //{
        //    // ex: 
        //    //FileStream f = File.OpenRead(@"c:\testfile.txt");
        //    //byte[] b = f.ConvertToByteArray();
        //    //Console.WriteLine(b.Length);            


        //    var streamLength = Convert.ToInt32(stream.Length);
        //    byte[] data = new byte[streamLength + 1];

        //    //convert to to a byte array
        //    stream.Read(data, 0, streamLength);
        //    stream.Close();

        //    return data;
        //}

/*
        /// <summary>Renames one or more files (same as FileMove)</summary>
        /// <param name="SourcePattern">The name of a single file or a wildcard pattern such as C:\Temp\*.tmp. SourcePattern is assumed to be in %A_WorkingDir% if an absolute path isn't specified.</param>
        /// <param name="DestPattern">The name or pattern of the destination, which is assumed to be in %A_WorkingDir% if an absolute path isn't specified. To perform a simple move -- retaining the existing file name(s) -- specify only the folder name as shown in these functionally identical examples: FileMove, C:\*.txt, C:\My Folder</param>
        /// <param name="OverWrite">Determines whether to overwrite files if they already exist</param>
        public bool FileRename_AddPrefix(string SourcePattern, string DestPattern, bool OverWrite = false, string FileNamePrefix = "")
        {
            string NewFileName = DestPattern;

            if (FileNamePrefix != "") { NewFileName = FileDir(SourcePattern); }

            return FileMove(SourcePattern, DestPattern, OverWrite);
        }


        /// <summary>Returns path to application's default INI file location</summary>
        public string INIFile()
        {
            string INIFilePath = AppDir() + "\\Settings.ini";
            return INIFilePath;
        }

 */

        // timer displaying to control doesn't work b/c the function firing has to be static..

        //static System.Timers.Timer _timer; // From System.Timers
        //static List<DateTime> _l; // Stores timer results
        //public static List<DateTime> DateList // Gets the results
        //{
        //    get
        //    {
        //        if (_l == null) // Lazily initialize the timer
        //        {
        //            Start(); // Start the timer
        //        }
        //        return _l; // Return the list of dates
        //    }
        //}
        //static void StartTimerDisp()
        //{
        //    _l = new List<DateTime>(); // Allocate the list
        //    _timer = new System.Timers.Timer(3000); // Set up the timer for 3 seconds
        //                              //
        //                              // Type "_timer.Elapsed += " and press tab twice.
        //                              //
        //    _timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);
        //    _timer.Enabled = true; // Enable it
        //}
        //static void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        //{
        //    //_l.Add(DateTime.Now); // Add date on each timer event

        //    DispText(DateTime.Now, timerDisp); 
        //}

        //// optional control parameter used to display values as functions execute - not necessary for function to perform, used to display progress
        //public Control timerDisp;



        #region === RegEx ===


        ////#################################
        ////           RegEx
        ////#################################


        //public Regex _regexProductionNumber = new Regex(@"\d{1,3}\b\.\s", RegexOptions.CultureInvariant | RegexOptions.Compiled);

        //public Regex _regexEpisodeNumber = new Regex(@"\d{1,2}-\d{1,2}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        //public Regex _regexAirDate = new Regex("\\d\\d\\s\\b\\w{3,4}\\b\\s\\d\\d", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

        //public Regex _regexEpisodeTitle = new Regex(@"(?<=>)(.*)(?=</a>)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);


        //// from 1-3 format, pull the 1 or the 3 for season/episode
        //public Regex _regexSeasonNum = new Regex(@"\d{1,2}(?=-)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
        //public Regex _regexEpNum = new Regex(@"(?<=-)\d{1,2}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);


        //\d{3}\b\.\s  = Episode Number (101)
        //\b\d{1,2}\b-\d\d  episode number (5-13)
        //\d\d\s\b\w{3,4}\b\s\d\d  air date
        //>(.*)</a>  episode title


        ///// <summary>
        /////  Regular expression built for C# on: Wed, Apr 13, 2016, 03:14:33 PM
        /////  Using Expresso Version: 3.0.5854, http://www.ultrapico.com
        /////  
        /////  A description of the regular expression:
        /////  
        /////  <h4>
        /////      <h4>
        /////  [1]: A numbered capture group. [.|\n], any number of repetitions, as few as possible
        /////      Select from 2 alternatives
        /////          Any character
        /////          New line
        /////  <\/h4>
        /////      <
        /////      Literal /
        /////      h4>
        /////  
        /////
        ///// </summary>
        //public Regex _MyRegex = new Regex(
        //      "<h4>(.|\\n)*?<\\/h4>",
        //    RegexOptions.IgnoreCase
        //    | RegexOptions.IgnorePatternWhitespace
        //    );


        //// This is the replacement string
        //public string _MyRegexReplace =
        //      "$& [${Day}-${Month}-${Year}]";



        //public void _MatchRegex()
        //{
        //    string InputText = FileRead(@"C:\Users\jason\Google Drive\AHK\MDb\IMDB\RegExHTML\AHKPage.html").ToString();

        //    //// Capture all Matches in the InputText
        //    MatchCollection matches = _MyRegex.Matches(InputText.ToString());


        //    string ResultLine = "";
        //    foreach (Match match in matches)
        //    {
        //        try
        //        {
        //            ResultLine = match.Value.ToString();
        //            FileAppend(ResultLine, @"C:\Users\jason\Google Drive\AHK\MDb\IMDB\RegExHTML\AHKPage_Parse1.txt");
        //            //MessageBox.Show(match.Value.ToString());
        //        }
        //        catch { }
        //    }

        //}

        //// Replace the matched text in the InputText using the replacement pattern
        // string result = MyRegex.Replace(InputText,MyRegexReplace);

        //// Split the InputText wherever the regex matches
        // string[] results = MyRegex.Split(InputText);

        //// Capture the first Match, if any, in the InputText
        // Match m = MyRegex.Match(InputText);

        //// Test to see if there is a match in the InputText
        // bool IsMatch = MyRegex.IsMatch(InputText);

        //// Get the names of all the named and numbered capture groups
        // string[] GroupNames = MyRegex.GetGroupNames();

        //// Get the numbers of all the named and numbered capture groups
        // int[] GroupNumbers = MyRegex.GetGroupNumbers();


        #endregion


        /// <summary>
        /// Contains approximate string matching (used by closest word/filename)
        /// </summary>
        static class LevenshteinDistance
        {

            // ex: 
            //int last = -1; int val = -1; 
            //foreach(string imaGe in imgNames)
            //{
            //    val = LevenshteinDistance.Compute(ItemName, imaGe);

            //    if (last == -1) { last = val; close = imaGe; }
            //    if (val < last) { last = val; close = imaGe; }
            //}

            //ahk.MsgBox(ItemName + " : " + close + " (" + last.ToString() + ")");


            /// <summary>
            /// Compute the distance between two strings.
            /// </summary>
            public static int ComputeStringDistance(string s, string t)
            {
                int n = s.Length;
                int m = t.Length;
                int[,] d = new int[n + 1, m + 1];

                // Step 1
                if (n == 0)
                {
                    return m;
                }

                if (m == 0)
                {
                    return n;
                }

                // Step 2
                for (int i = 0; i <= n; d[i, 0] = i++)
                {
                }

                for (int j = 0; j <= m; d[0, j] = j++)
                {
                }

                // Step 3
                for (int i = 1; i <= n; i++)
                {
                    //Step 4
                    for (int j = 1; j <= m; j++)
                    {
                        // Step 5
                        int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;

                        // Step 6
                        d[i, j] = Math.Min(
                            Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                            d[i - 1, j - 1] + cost);
                    }
                }
                // Step 7
                return d[n, m];
            }
        }



    }


    #endregion


    #region === OBJECTS ===

    ///// <summary>Stores Global Variables / Session Info / Error Level / Logging Values</summary>
    //private static class ahkGlobal
    //{
    //    public static AutoHotkey.Interop.AutoHotkeyEngine ahkdll { get; set; }  // stores current AHK session

    //    public static bool ErrorLevel { get; set; }         // true if error level detected in ahk command
    //    public static bool ErrorLevelEnabled { get; set; }  // true if error level information is available for this command
    //    public static string ErrorLevelValue { get; set; } // ErrorLevel variable value returned from AHK session
    //    public static string ErrorLevelMsg { get; set; }   // assembled error level message to display
    //    public static string ErrorLevelCustom { get; set; }  // custom error level text for a command

    //    public static string LastLine { get; set; }     // last line executed by ahk execute function
    //    public static string LastAction { get; set; }  // last function/ahk command used by ahk execute function
    //    public static bool Debug { get; set; }

    //    public static bool MacroPlaying { get; set; }
    //}

    /// <summary>Stores Mouse Coordinates (Relative to Screen or Window) and Info Gathered Under Mouse</summary>

    /*
        private class mousePos
        {
            // ex:
            // mousePos mp = ahk.MouseGetPos("3");  // populate object with values

            public int X_Window { get; set; } // stores x mouse coordinate relative to the active window  
            public int X_Screen { get; set; } // stores y mouse coordinate relative to the screen

            public int Y_Window { get; set; } // stores x mouse coordinate relative to the active window  
            public int Y_Screen { get; set; } // stores y mouse coordinate relative to the screen



            public string WinHwnd { get; set; }
            public string ControlClassNN { get; set; }
            public string ControlHwnd { get; set; }
        }
     */

    ///// <summary>Stores Window Coordinates and Additional Details Returned from AHK Functions</summary>
    //public class winInfo
    //{
    //    // ex:
    //    // winInfo win = new winInfo();   // define object reference

    //    public string WinTitle { get; set; }
    //    public string WinText { get; set; }
    //    public string Hwnd { get; set; }
    //    public string Class { get; set; }
    //    public string PID { get; set; }
    //    public string ProcessName { get; set; }
    //    public string MinMax { get; set; }
    //    public string Count { get; set; }

    //    public int WinX { get; set; }
    //    public int WinY { get; set; }
    //    public int WinW { get; set; }
    //    public int WinH { get; set; }

    //}

    /// <summary>Stores Rectangle Coordinates</summary>
    public struct Rect
    {
        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
    }

    /// <summary>Control Info</summary>
    public struct conInfo
    {
        public string ControlName { get; set; }
        public string ControlText { get; set; }
        public IntPtr ControlHwnd { get; set; }
        public string ControlClass { get; set; }
        public string ControlValue { get; set; }

        public int ControlX { get; set; }
        public int ControlY { get; set; }
        public int ControlW { get; set; }
        public int ControlH { get; set; }

        public bool ControlChecked { get; set; }
        public bool ControlEnabled { get; set; }
        public bool ControlVisible { get; set; }
    }


    public static class globalControls
    {
        public static DataGridView dvName { get; set; }
        public static Scintilla codeDisplay { get; set; }
        public static Control DirToDbDisplay { get; set; }
    }

    public static class GlobalVars
    {
        // ex:
        // GlobalVars global = new GlobalVars();  // allow access to set of global settings used across interfaces
        // if (Play == true) { global.LoadedFunction = true; }

        public static bool MacroPlaying { get; set; }
        public static bool Debug { get; set; }

        public static string UserDb { get; set; }

    }

    public static class grID
    {
        public static DataGridView dvName { get; set; }
        public static int ColCount { get; set; }
        public static int RowCount { get; set; }
        public static string sqliteCmd { get; set; }
        public static string dbFILE { get; set; }
    }

    public class NoteClass
    {
        public string Public { get; set; }
        public string ClientNum { get; set; }
        public string NoteName { get; set; }
        public string Note { get; set; }
    }
    public class MouseMacro
    {
        public string ID { get; set; }
        public string MacroName { get; set; }
        public string MouseAction { get; set; }
        public string MouseX { get; set; }
        public string MouseY { get; set; }
        public string ActiveWindow { get; set; }
        public DateTime CurrentTime { get; set; }
        public string SearchImage { get; set; }
        public string PlayBack { get; set; }
        public string ActionNum { get; set; }
        public string Flagged { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
    }

    // setup global static object
    public static class NEW
    {
        public static _Database.goDaddy goDad { get; set; }
        public static _Database.SQL sql { get; set; }
        public static _Database.SQLite sqlite { get; set; }
        public static _Lists lst { get; set; }
        public static _Dict dict { get; set; }
        public static _Images img { get; set; }
        public static _StatusBar sbb { get; set; }
        public static _AHK ahk { get; set; }
        public static _GridControl grid { get; set; }
        public static _TreeViewControl tv { get; set; }
        //public static _Launch run = new _Launch();

        //public static void SetRefs()
        //{
        //    ahk = new _AHK(); 


        //}
    }


    #endregion


    #region === Applications ===

    /// <summary>
    /// Collection of Applications To Launch / Interact With
    /// </summary>
    public partial class _Apps
    {
        public class Dock
        {
            

            public void FormvsDock(string winTitle = "vsDock")
            {
                //Dynamic_Coder.Forms.vsDock frm = new Dynamic_Coder.Forms.vsDock(winTitle); // Create a new instance of the GridMenu form
                //frm.Show(); // Show the settings form
            }

            public void vsDock(string MainWindow = "Untitled - Notepad", string dockTitle = "vsDock")
            {
                FormvsDock(dockTitle); // build / launch form
                dock(MainWindow, dockTitle); // dock form to another window
            }

            public void dock(string MainWindow = "Untitled - Notepad", string DockWindow = "codeDock")
            {
                _AHK ahk = new _AHK();

                string launch = "D:\\Dropbox\\IMDB\\sharp_AHK\\sharpDock\\DockWindows.exe \"" + MainWindow + "\" " + DockWindow + "\"";
                //string launch = "D:\\Dropbox\\IMDB\\sharp_AHK\\Dynamic_Coder\\bin\\Debug\\Project_Files\\Scripts\\DockWindows.exe " + MainWindow + " " + DockWindow + "";
                //string launch = "D:\\Dropbox\\IMDB\\sharp_AHK\\sharpDock\\DockWindows.exe ";
                ahk.Run(launch);
            }

            public void DockClose()
            {
                _AHK ahk = new _AHK();

                ahk.process("close", "DockWindows.exe");
            }

        }

     }


    public static class MpcExtentions
    {
        public static void MpcAction(this mpcWin Win, mpcActions action)
        {
            string hWnd = "";

            Mpc mpc = new Mpc();
            _AHK ahk = new _AHK();
            
            if (action == mpcActions.WinMoveQ1) { mpc.WinMovePID_Q1(Win.PID.ToInt()); }
            if (action == mpcActions.WinMoveQ2) { mpc.WinMovePID_Q2(Win.PID.ToInt()); }
            if (action == mpcActions.WinMoveQ3) { mpc.WinMovePID_Q3(Win.PID.ToInt()); }
            if (action == mpcActions.WinMoveQ4) { mpc.WinMovePID_Q4(Win.PID.ToInt()); }
            if (action == mpcActions.Pause) { mpc.Pause(hWnd); }
            if (action == mpcActions.Open) { mpc.Open(hWnd); }
            if (action == mpcActions.Mute) { mpc.Mute(hWnd); }
            if (action == mpcActions.Play) { mpc.Play(hWnd); }
            if (action == mpcActions.Stop) { mpc.Stop(hWnd); }
            if (action == mpcActions.Close) { mpc.Close(hWnd); }
            if (action == mpcActions.FullScreen) { mpc.FullScreen(hWnd); }
            if (action == mpcActions.Zoom200) { mpc.Zoom200(hWnd); }
            if (action == mpcActions.IncreaseRate) { mpc.IncreaseRate(hWnd); }
            if (action == mpcActions.DecreaseRate) { mpc.DecreaseRate(hWnd); }
            if (action == mpcActions.ResetRate) { mpc.ResetRate(hWnd); }
            if (action == mpcActions.JumpForwardLarge) { mpc.JumpForwardLarge(hWnd); }
            if (action == mpcActions.JumpForwardMedium) { mpc.JumpForwardMedium(hWnd); }
            if (action == mpcActions.JumpForwardSmall) { mpc.JumpForwardSmall(hWnd); }
            if (action == mpcActions.NextPlaylistItem) { mpc.NextPlaylistItem(hWnd); }
            if (action == mpcActions.ViewNormal) { mpc.ViewNormal(hWnd); }
            if (action == mpcActions.AlwaysOnTop) { mpc.AlwaysOnTop(hWnd); }
            if (action == mpcActions.QuickOpenFile) { mpc.QuickOpenFile(hWnd); }
            if (action == mpcActions.OpenDVD) { mpc.OpenDVD(hWnd); }
            if (action == mpcActions.OpenDevice) { mpc.OpenDevice(hWnd); }
            if (action == mpcActions.SaveAs) { mpc.SaveAs(hWnd); }
            if (action == mpcActions.SaveImage) { mpc.SaveImage(hWnd); }
            if (action == mpcActions.SaveImageAuto) { mpc.SaveImageAuto(hWnd); }
            if (action == mpcActions.LoadSubtitle) { mpc.LoadSubtitle(hWnd); }
            if (action == mpcActions.SaveSubtitle) { mpc.SaveSubtitle(hWnd); }
            if (action == mpcActions.Properties) { mpc.Properties(hWnd); }
            if (action == mpcActions.Exit) { mpc.Exit(hWnd); }
            if (action == mpcActions.TogglePlayPause) { mpc.TogglePlayPause(hWnd); }
            if (action == mpcActions.Framestep) { mpc.Framestep(hWnd); }
            if (action == mpcActions.FramestepBack) { mpc.FramestepBack(hWnd); }
            if (action == mpcActions.GoTo) { mpc.GoTo(hWnd); }
            if (action == mpcActions.AudioDelayPlus10ms) { mpc.AudioDelayPlus10ms(hWnd); }
            if (action == mpcActions.AudioDelayMinus10ms) { mpc.AudioDelayMinus10ms(hWnd); }
            if (action == mpcActions.JumpBackwardSmall) { mpc.JumpBackwardSmall(hWnd); }
            if (action == mpcActions.JumpBackwardMedium) { mpc.JumpBackwardMedium(hWnd); }
            if (action == mpcActions.JumpBackwardLarge) { mpc.JumpBackwardLarge(hWnd); }
            if (action == mpcActions.JumpForwardKeyframe) { mpc.JumpForwardKeyframe(hWnd); }
            if (action == mpcActions.JumpBackwardKeyframe) { mpc.JumpBackwardKeyframe(hWnd); }
            if (action == mpcActions.Next) { mpc.Next(hWnd); }
            if (action == mpcActions.Previous) { mpc.Previous(hWnd); }
            if (action == mpcActions.PreviousPlaylistItem) { mpc.PreviousPlaylistItem(hWnd); }
            if (action == mpcActions.ToggleCaptionMenu) { mpc.ToggleCaptionMenu(hWnd); }
            if (action == mpcActions.ToggleSeeker) { mpc.ToggleSeeker(hWnd); }
            if (action == mpcActions.ToggleControls) { mpc.ToggleControls(hWnd); }
            if (action == mpcActions.ToggleInformation) { mpc.ToggleInformation(hWnd); }
            if (action == mpcActions.ToggleStatistics) { mpc.ToggleStatistics(hWnd); }
            if (action == mpcActions.ToggleStatus) { mpc.ToggleStatus(hWnd); }
            if (action == mpcActions.ToggleSubresyncBar) { mpc.ToggleSubresyncBar(hWnd); }
            if (action == mpcActions.TogglePlaylistBar) { mpc.TogglePlaylistBar(hWnd); }
            if (action == mpcActions.ToggleCaptureBar) { mpc.ToggleCaptureBar(hWnd); }
            if (action == mpcActions.ToggleShaderEditorBar) { mpc.ToggleShaderEditorBar(hWnd); }
            if (action == mpcActions.ViewMinimal) { mpc.ViewMinimal(hWnd); }
            if (action == mpcActions.ViewCompact) { mpc.ViewCompact(hWnd); }
            if (action == mpcActions.FullscreenNoResChange) { mpc.FullscreenNoResChange(hWnd); }
            if (action == mpcActions.Zoom50) { mpc.Zoom50(hWnd); }
            if (action == mpcActions.Zoom100) { mpc.Zoom100(hWnd); }
            if (action == mpcActions.ZoomAutoFit) { mpc.ZoomAutoFit(hWnd); }
            if (action == mpcActions.NextARPreset) { mpc.NextARPreset(hWnd); }
            if (action == mpcActions.VidFrmHalf) { mpc.VidFrmHalf(hWnd); }
            if (action == mpcActions.VidFrmNormal) { mpc.VidFrmNormal(hWnd); }
            if (action == mpcActions.VidFrmDouble) { mpc.VidFrmDouble(hWnd); }
            if (action == mpcActions.VidFrmStretch) { mpc.VidFrmStretch(hWnd); }
            if (action == mpcActions.VidFrmInside) { mpc.VidFrmInside(hWnd); }
            if (action == mpcActions.VidFrmOutside) { mpc.VidFrmOutside(hWnd); }
            if (action == mpcActions.PnSReset) { mpc.PnSReset(hWnd); }
            if (action == mpcActions.PnSIncSize) { mpc.PnSIncSize(hWnd); }
            if (action == mpcActions.PnSIncWidth) { mpc.PnSIncWidth(hWnd); }
            if (action == mpcActions.PnSIncHeigh) { mpc.PnSIncHeigh(hWnd); }
            if (action == mpcActions.PnSDecSize) { mpc.PnSDecSize(hWnd); }
            if (action == mpcActions.PnSDecWidth) { mpc.PnSDecWidth(hWnd); }
            if (action == mpcActions.PnSDecHeight) { mpc.PnSDecHeight(hWnd); }
            if (action == mpcActions.PnSCenter) { mpc.PnSCenter(hWnd); }
            if (action == mpcActions.PnSLeft) { mpc.PnSLeft(hWnd); }
            if (action == mpcActions.PnSRight) { mpc.PnSRight(hWnd); }
            if (action == mpcActions.PnSUp) { mpc.PnSUp(hWnd); }
            if (action == mpcActions.PnSDown) { mpc.PnSDown(hWnd); }
            if (action == mpcActions.PnSUpLeft) { mpc.PnSUpLeft(hWnd); }
            if (action == mpcActions.PnSUpRight) { mpc.PnSUpRight(hWnd); }
            if (action == mpcActions.PnSDownLeft) { mpc.PnSDownLeft(hWnd); }
            if (action == mpcActions.PnSDownRight) { mpc.PnSDownRight(hWnd); }
            if (action == mpcActions.PnSRotateXPlus) { mpc.PnSRotateXPlus(hWnd); }
            if (action == mpcActions.PnSRotateXMinus) { mpc.PnSRotateXMinus(hWnd); }
            if (action == mpcActions.PnSRotateYPlus) { mpc.PnSRotateYPlus(hWnd); }
            if (action == mpcActions.PnSRotateYMinus) { mpc.PnSRotateYMinus(hWnd); }
            if (action == mpcActions.PnSRotateZPlus) { mpc.PnSRotateZPlus(hWnd); }
            if (action == mpcActions.PnSRotateZMinus) { mpc.PnSRotateZMinus(hWnd); }
            if (action == mpcActions.VolumeUp) { mpc.VolumeUp(hWnd); }
            if (action == mpcActions.VolumeDown) { mpc.VolumeDown(hWnd); }
            if (action == mpcActions.VolumeBoostIncrease) { mpc.VolumeBoostIncrease(hWnd); }
            if (action == mpcActions.VolumeBoostDecrease) { mpc.VolumeBoostDecrease(hWnd); }
            if (action == mpcActions.VolumeBoostMin) { mpc.VolumeBoostMin(hWnd); }
            if (action == mpcActions.VolumeBoostMax) { mpc.VolumeBoostMax(hWnd); }
            if (action == mpcActions.DVDTitleMenu) { mpc.DVDTitleMenu(hWnd); }
            if (action == mpcActions.DVDRootMenu) { mpc.DVDRootMenu(hWnd); }
            if (action == mpcActions.DVDSubtitleMenu) { mpc.DVDSubtitleMenu(hWnd); }
            if (action == mpcActions.DVDAudioMenu) { mpc.DVDAudioMenu(hWnd); }
            if (action == mpcActions.DVDAngleMenu) { mpc.DVDAngleMenu(hWnd); }
            if (action == mpcActions.DVDChapterMenu) { mpc.DVDChapterMenu(hWnd); }
            if (action == mpcActions.DVDMenuLeft) { mpc.DVDMenuLeft(hWnd); }
            if (action == mpcActions.DVDMenuRight) { mpc.DVDMenuRight(hWnd); }
            if (action == mpcActions.DVDMenuUp) { mpc.DVDMenuUp(hWnd); }
            if (action == mpcActions.DVDMenuDown) { mpc.DVDMenuDown(hWnd); }
            if (action == mpcActions.DVDMenuActivate) { mpc.DVDMenuActivate(hWnd); }
            if (action == mpcActions.DVDMenuBack) { mpc.DVDMenuBack(hWnd); }
            if (action == mpcActions.DVDMenuLeave) { mpc.DVDMenuLeave(hWnd); }
            if (action == mpcActions.BossKey) { mpc.BossKey(hWnd); }
            if (action == mpcActions.PlayerMenuShort) { mpc.PlayerMenuShort(hWnd); }
            if (action == mpcActions.PlayerMenuLong) { mpc.PlayerMenuLong(hWnd); }
            if (action == mpcActions.FiltersMenu) { mpc.FiltersMenu(hWnd); }
            if (action == mpcActions.Options) { mpc.Options(hWnd); }
            if (action == mpcActions.NextAudio) { mpc.NextAudio(hWnd); }
            if (action == mpcActions.PrevAudio) { mpc.PrevAudio(hWnd); }
            if (action == mpcActions.NextSubtitle) { mpc.NextSubtitle(hWnd); }
            if (action == mpcActions.PrevSubtitle) { mpc.PrevSubtitle(hWnd); }
            if (action == mpcActions.ToggleSubtitle) { mpc.ToggleSubtitle(hWnd); }
            if (action == mpcActions.NextAudioOGM) { mpc.NextAudioOGM(hWnd); }
            if (action == mpcActions.PrevAudioOGM) { mpc.PrevAudioOGM(hWnd); }
            if (action == mpcActions.NextSubtitleOGM) { mpc.NextSubtitleOGM(hWnd); }
            if (action == mpcActions.PrevSubtitleOGM) { mpc.PrevSubtitleOGM(hWnd); }
            if (action == mpcActions.NextAngleOGM) { mpc.NextAngleOGM(hWnd); }
            if (action == mpcActions.PrevAngleOGM) { mpc.PrevAngleOGM(hWnd); }
            if (action == mpcActions.NextAudioDVD) { mpc.NextAudioDVD(hWnd); }
            if (action == mpcActions.PrevAudioDVD) { mpc.PrevAudioDVD(hWnd); }
            if (action == mpcActions.NextSubtitleDVD) { mpc.NextSubtitleDVD(hWnd); }
            if (action == mpcActions.PrevSubtitleDVD) { mpc.PrevSubtitleDVD(hWnd); }
            if (action == mpcActions.OnOffSubtitleDVD) { mpc.OnOffSubtitleDVD(hWnd); }
            if (action == mpcActions.SaveThumbnails) { mpc.SaveThumbnails(hWnd); }
            if (action == mpcActions.ReloadSubtitles) { mpc.ReloadSubtitles(hWnd); }
            if (action == mpcActions.TearingTest) { mpc.TearingTest(hWnd); }
            if (action == mpcActions.RemainingTime) { mpc.RemainingTime(hWnd); }
            if (action == mpcActions.TogglePixelShader) { mpc.TogglePixelShader(hWnd); }
            if (action == mpcActions.ToggleDirect3DFullscreen) { mpc.ToggleDirect3DFullscreen(hWnd); }
            if (action == mpcActions.GotoPrevSubtitle) { mpc.GotoPrevSubtitle(hWnd); }
            if (action == mpcActions.GotoNextSubtitle) { mpc.GotoNextSubtitle(hWnd); }
            if (action == mpcActions.ShiftSubtitleLeft) { mpc.ShiftSubtitleLeft(hWnd); }
            if (action == mpcActions.ShiftSubtitleRight) { mpc.ShiftSubtitleRight(hWnd); }
            if (action == mpcActions.DisplayStats) { mpc.DisplayStats(hWnd); }
            if (action == mpcActions.SubtitleDelayMinus) { mpc.SubtitleDelayMinus(hWnd); }
            if (action == mpcActions.SubtitleDelayPlus) { mpc.SubtitleDelayPlus(hWnd); }
        }

    }


    #endregion


    #region === MEDIA ====



    public class TVServerLocal
    {
        _AHK ahk = new _AHK();
        _Database.SQL sql = new _Database.SQL();
        _Database.SQLite sqlite = new _Database.SQLite();
        _Lists lst = new _Lists();
        _TelerikLib tel = new _TelerikLib();
        _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();

        public void Flag_ShowName_Dupes(Telerik.WinControls.UI.RadProgressBar PBar)
        {
            string cmd = "select Distinct DirName FROM [fileserver].[lucidmethod].[TVServerShows] order by DirName";
            List<string> shows = lst.SQL_To_List(TVServerDirs_Conn(), cmd);

            string outFile = ahk.AppDir() + "\\Dupes.txt";
            ahk.FileDelete(outFile);

            pro.ResetProgress(PBar);
            pro.SetupProgressBar(PBar, shows.Count());

            int i = 1;
            foreach (string show in shows)
            {
                pro.UpdateProgress(PBar, i + "/" + shows.Count()); i++;

                cmd = "select count(DirName) FROM [fileserver].[lucidmethod].[TVServerShows] where DirName = '" + show + "'";
                int foundCount = ahk.ToInt(sql.SQL_Return_Value(TVServerDirs_Conn(), cmd));

                if (foundCount > 1) { ahk.FileAppend(show, outFile); }
            }

            ahk.Run(outFile);

        }


        public void UpdateLabel(Telerik.WinControls.UI.RadLabel lablel, string Text)
        {
            if (lablel.InvokeRequired)  // if currently on a different thread, invoke 
            {
                lablel.BeginInvoke((MethodInvoker)delegate () { lablel.Text = Text; });
            }
            else
            {
                lablel.Text = Text;
            }
        }

        public void UpdateGrid(RadGridView radGrid, DataTable datasource)
        {
            if (radGrid.InvokeRequired)  // if currently on a different thread, invoke 
            {
                radGrid.BeginInvoke((MethodInvoker)delegate () { radGrid.DataSource = datasource; });
            }
            else
            {
                radGrid.DataSource = datasource;
            }
        }

        public void Update_LocalTVServerListing(Telerik.WinControls.UI.RadLabel lablel, Telerik.WinControls.UI.RadProgressBar progress1, Telerik.WinControls.UI.RadProgressBar progress2)
        {

            List<string> dirs = LocalTVDirs();
            pro.ResetProgress(progress1);
            pro.SetupProgressBar(progress1, dirs.Count());
            pro.UpdateProgress(progress1);

            int added = 0;
            int i = 1;

            foreach (string dir in dirs)
            {
                // do nothing if directory not found
                if (!Directory.Exists(dir)) { continue; }

                // clear out existing shows for root dir
                string cmd = "delete from [fileserver].[lucidmethod].[TVServerShows] where RootDir = '" + dir + "'";
                bool deleted = sql.WriteDataRecord(TVServerDirs_Conn(), cmd);

                TVServerShows show = new TVServerShows();
                show.RootDir = dir;

                // list of shows in root dir
                List<string> showDirs = lst.DirList(dir, "*.*", false, true);

                pro.SetupProgressBar(progress2, showDirs.Count());

                // add each show to index
                int k = 1;
                foreach (string showDir in showDirs)
                {
                    UpdateLabel(lablel, i + "/" + dirs.Count + " | " + k + "/" + showDirs.Count()); k++;
                    pro.UpdateProgress(progress2, k + "/" + showDirs.Count());

                    show.DirPath = showDir;
                    show.DirName = ahk.DirName(showDir);
                    bool updated = TVServerShows_UpdateInsert(show);
                    if (updated) { added++; }
                }

                pro.UpdateProgress(progress1, i + "/" + dirs.Count + " | " + dir);
                pro.ResetProgress(progress2);
                i++;
            }

            ahk.MsgBox("updated/added " + added.ToString() + " Shows to index");

        }


        // list of local tv folders to search for folder names
        public List<string> LocalTVDirs(bool FromSQLDb = false)
        {
            List<string> dirs = new List<string>();

            if (FromSQLDb)
            {
                string cmd = "select DirPath FROM [fileserver].[lucidmethod].[TVServerDirs] where Enabled = '1' and [Cat] = 'TVSeries'";
                dirs = lst.SQL_To_List(TVServerDirs_Conn(), cmd);
            }
            else
            {
                dirs.Add(@"T:\");
                dirs.Add(@"Q:\TV_Server_12");
                dirs.Add(@"J:\TV_Server_5");
                dirs.Add(@"K:\TV_Server_6");
                dirs.Add(@"N:\TV_Server_8");
                dirs.Add(@"L:\TV_Server_7");
            }

            return dirs;
        }

        // list of local tv folders to search for folder names
        public List<string> LocalMovieDirs(bool FromSQLDb = false)
        {
            List<string> dirs = new List<string>();

            if (FromSQLDb)
            {
                string cmd = "select DirPath FROM [fileserver].[lucidmethod].[TVServerDirs] where Enabled = '1' and [Cat] = 'Movies'";
                dirs = lst.SQL_To_List(TVServerDirs_Conn(), cmd);
            }
            else
            {
                dirs.Add(@"E:\[ Movies ]\[ INCOMING ]");
                dirs.Add(@"E:\[ Movies ]\[ Movies A-K ]");
                dirs.Add(@"E:\[ Movies ]\[ Movies L-S ]");
                dirs.Add(@"E:\[ Movies ]\[ Movies T-Z ]");
                dirs.Add(@"E:\[ Movies ]\[ Theater Rips ]");
                dirs.Add(@"E:\[ Movies ]\[ Training ]");
                dirs.Add(@"E:\Movies_0");
                dirs.Add(@"I:\Movies_4");
                dirs.Add(@"M:\Movies_M\[ INCOMING ]");
                dirs.Add(@"M:\Movies_M\[ Movies A-K ]");
                dirs.Add(@"M:\Movies_M\[ Movies L-S ]");
                dirs.Add(@"M:\Movies_M\[ Movies T-Z ]");
                dirs.Add(@"N:\Movies_8");
                dirs.Add(@"O:\Movies_9");
                dirs.Add(@"Z:\[ Standup ]");
            }

            return dirs;
        }


        public enum DirType
        {
            Movies = 0,
            TV = 1,
            ADB = 2,
            Roms = 3,
            Code = 4,
            Production = 5,
            Backup = 6, 
            Software = 7,
            Drives = 8
        }


        public List<string> LocalDirList(DirType dirType)
        {
            List<string> dirs = new List<string>();

            if (dirType.ToString().ToUpper() == "MOVIES")
            {
                dirs.Add(@"E:\[ Movies ]\[ INCOMING ]");
                dirs.Add(@"E:\[ Movies ]\[ Movies A-K ]");
                dirs.Add(@"E:\[ Movies ]\[ Movies L-S ]");
                dirs.Add(@"E:\[ Movies ]\[ Movies T-Z ]");
                //dirs.Add(@"E:\[ Movies ]\[ Theater Rips ]");
                //dirs.Add(@"E:\[ Movies ]\[ Training ]");
                //dirs.Add(@"E:\Movies_0");
                //dirs.Add(@"I:\Movies_4");
                //dirs.Add(@"M:\Movies_M\[ INCOMING ]");
                //dirs.Add(@"M:\Movies_M\[ Movies A-K ]");
                //dirs.Add(@"M:\Movies_M\[ Movies L-S ]");
                //dirs.Add(@"M:\Movies_M\[ Movies T-Z ]");
                //dirs.Add(@"N:\Movies_8");
                //dirs.Add(@"O:\Movies_9");
                //dirs.Add(@"Z:\[ Standup ]");
            }

            if (dirType.ToString().ToUpper() == "TV")
            {
                dirs.Add(@"J:\TV_Server_5");
                dirs.Add(@"K:\TV_Server_6");
                dirs.Add(@"L:\TV_Server_7");
                dirs.Add(@"M:\TV_Server_M");
                dirs.Add(@"N:\TV_Server_8");
                dirs.Add(@"R:\TV_Server_R");
                dirs.Add(@"Z:\[ TV ]");
            }
            if (dirType.ToString().ToUpper() == "ADB")
            {
                dirs.Add(@"F:\ADB_1");
                dirs.Add(@"G:\ADB_2");
                dirs.Add(@"H:\_Foreign");
            }
            if (dirType.ToString().ToUpper() == "CODE")
            {
                dirs.Add(@"C:\_Code");
                dirs.Add(@"D:\_Code");
                dirs.Add(@"H:\Code_Backups");
                dirs.Add(@"Z:\[ Code ]");
                dirs.Add(@"Z:\_Code");
                dirs.Add(@"U:\_Code");
                dirs.Add(@"V:\_CodeDRIVE");
            }
            if (dirType.ToString().ToUpper() == "PRODUCTION")
            {
                dirs.Add(@"Q:\bAck\[ Production ]");
                dirs.Add(@"Q:\Clients");
                dirs.Add(@"V:\[ Production ]");
            }
            if (dirType.ToString().ToUpper() == "BACKUP")
            {
                dirs.Add(@"Q:\bAck\[ FAMILY ]");
                dirs.Add(@"Q:\Backup");
            }
            if (dirType.ToString().ToUpper() == "SOFTWARE")
            {
                dirs.Add(@"Z:\[ Apps ]");
                dirs.Add(@"Q:\bAck\[ Software ]");
            }
            if (dirType.ToString().ToUpper() == "ROMS")
            {
                dirs.Add(@"Z:\[ Roms ]");
                dirs.Add(@"Q:\bAck\[ Roms ]");
            }
            if (dirType.ToString().ToUpper() == "MUSIC")
            {
                dirs.Add(@"Z:\[ Music ]");
                dirs.Add(@"J:\[ Media ]\[ Audiobooks ]");
            }
            if (dirType.ToString().ToUpper() == "DRIVES")
            {
                dirs.Add(@"B:\");
                dirs.Add(@"D:\Drives");
                dirs.Add(@"I:\Drives");
                dirs.Add(@"Q:\Drives");
                dirs.Add(@"V:\Drives");
            }


            // make sure each dir is valid
            List<string> confirmed = new List<string>();
            foreach(string dir in dirs)
            {
                if (Directory.Exists(dir)) { confirmed.Add(dir); }
            }

            return confirmed;
        }



        // list of local tv folders to search for folder names
        public List<string> LocalADBDirs(bool FromSQLDb = false)
        {
            List<string> dirs = new List<string>();

            if (FromSQLDb)
            {
                string cmd = "select DirPath FROM [fileserver].[lucidmethod].[TVServerDirs] where Enabled = '1' and [Cat] = 'ADB'";
                dirs = lst.SQL_To_List(TVServerDirs_Conn(), cmd);
            }
            else
            {
                dirs.Add(@"M:\ADB_1");
                dirs.Add(@"Q:\ADB_2");
                dirs.Add(@"O:\_Foreign");
            }

            return dirs;
        }


        // look up list of local directories (by category) and populate treeview
        public void LocalDirsTree(RadTreeView RadTree)
        {
            _TelerikLib.RadTree tree = new _TelerikLib.RadTree();

            RadTree.ClearTree();

            // return list of folder categories
            string cmd = "select distinct Cat FROM [fileserver].[lucidmethod].[TVServerDirs] order by Cat";
            List<string> cats = lst.SQL_To_List(TVServerDirs_Conn(), cmd);

            // foreach category, return folders for that category and add to treeview node and attach
            foreach (string cat in cats)
            {
                cmd = "select DirPath FROM [fileserver].[lucidmethod].[TVServerDirs] where Enabled = '1' and [Cat] = '" + cat + "' order by DirPath";
                List<string> dirs = lst.SQL_To_List(TVServerDirs_Conn(), cmd);
                if (dirs.Count > 0)
                {
                    RadTreeNode catNode = new RadTreeNode(cat);  // new code with category
                    catNode.Tag = "Category";

                    // loop through list of directories returned from db 
                    foreach (string dir in dirs)
                    {

                        RadTreeNode folderNode = new RadTreeNode(dir);  // new code with category
                        folderNode.Tag = dir;
                        //folderNode.Nodes.Add(dir);
                        //catNode.Nodes.Add(folderNode);


                        List<string> dirContents = lst.DirList(dir, "*.*", false, false);

                        if (dirContents != null && dirContents.Count > 0)
                        {
                            foreach (string dirContent in dirContents)
                            {
                                RadTreeNode subfolderNode = new RadTreeNode(dirContent);  // new code with category

                                subfolderNode.Tag = dir + "\\" + dirContent;

                                tree.AddSubNode(folderNode, subfolderNode);
                                //subfolderNode.Nodes.Add(dirContent); ;

                                //titleNode.Nodes.Add(dirContent);
                                //folderNode.Nodes.Add(subfolderNode);
                            }

                            //catNode.Nodes.Add(folderNode);
                        }


                        tree.AddSubNode(catNode, folderNode);
                        //tree.AddNode(RadTree, folderNode);  // attach node to tree

                    }  // add each dir in category to node

                    tree.AddNode(RadTree, catNode);  // attach Category Node
                }

            }

        }



        #region === TVServerDirs FUNCTIONS ===

        #region ===== TVServerDirs Object =====

        public struct TVServerDirs
        {
            public string DirPath { get; set; }
            public string Enabled { get; set; }
            public string Flag { get; set; }
            public string Cat { get; set; }
        }
        public TVServerDirs Return_TVServerDirs(string DirPath = "", string Enabled = "", string Flag = "")
        {
            TVServerDirs obj = new TVServerDirs();
            obj.DirPath = DirPath;
            obj.Enabled = Enabled;
            obj.Flag = Flag;

            return obj;
        }

        //  Fix illegal characters before Sql/Sqlite Db Inserts
        public TVServerDirs TVServerDirs_FixChars(TVServerDirs ToFix)
        {
            TVServerDirs Fixed = new TVServerDirs();

            Fixed.DirPath = ToFix.DirPath.Replace("'", "''");
            Fixed.Enabled = ToFix.Enabled.Replace("'", "''");
            Fixed.Flag = ToFix.Flag.Replace("'", "''");

            return Fixed;
        }

        // Compare two objects to see if they have identical values
        public bool TVServerDirs_Changed(TVServerDirs OldVal, TVServerDirs NewVal)
        {
            TVServerDirs diff = new TVServerDirs();
            List<string> diffList = new List<string>();
            bool different = false;
            if (OldVal.DirPath == null) { OldVal.DirPath = ""; }
            if (NewVal.DirPath == null) { NewVal.DirPath = ""; }
            if (OldVal.DirPath != NewVal.DirPath) { different = true; }
            if (OldVal.Enabled == null) { OldVal.Enabled = ""; }
            if (NewVal.Enabled == null) { NewVal.Enabled = ""; }
            if (OldVal.Enabled != NewVal.Enabled) { different = true; }
            if (OldVal.Flag == null) { OldVal.Flag = ""; }
            if (NewVal.Flag == null) { NewVal.Flag = ""; }
            if (OldVal.Flag != NewVal.Flag) { different = true; }
            return different;
        }

        // Returns object containing the new values different from the old values in object comparison
        public TVServerDirs TVServerDirs_Diff(TVServerDirs OldVal, TVServerDirs NewVal)
        {
            TVServerDirs diff = new TVServerDirs();
            if (OldVal.DirPath != NewVal.DirPath) { diff.DirPath = NewVal.DirPath; }
            if (OldVal.Enabled != NewVal.Enabled) { diff.Enabled = NewVal.Enabled; }
            if (OldVal.Flag != NewVal.Flag) { diff.Flag = NewVal.Flag; }
            return diff;
        }

        // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
        public List<string> TVServerDirs_DiffList(TVServerDirs OldVal, TVServerDirs NewVal)
        {
            List<string> diffList = new List<string>();
            if (OldVal.DirPath != NewVal.DirPath) { diffList.Add("Changed DirPath Value From " + OldVal.DirPath + " To " + NewVal.DirPath); }
            if (OldVal.Enabled != NewVal.Enabled) { diffList.Add("Changed Enabled Value From " + OldVal.Enabled + " To " + NewVal.Enabled); }
            if (OldVal.Flag != NewVal.Flag) { diffList.Add("Changed Flag Value From " + OldVal.Flag + " To " + NewVal.Flag); }
            return diffList;
        }


        #endregion
        #region ===== TVServerDirs SQLite : Return =====

        public TVServerDirs Return_Object_From_TVServerDirs(string DbFile, string WhereClause = "[DirPath] = ''")
        {
            string SelectLine = "Select [DirPath], [Enabled], [Flag] From [TVServerDirs] ";
            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
            if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
            if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
            TVServerDirs returnObject = new TVServerDirs();
            int i = 0;
            string Value = "";
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    returnObject.DirPath = ret["DirPath"].ToString();
                    returnObject.Enabled = ret["Enabled"].ToString();
                    returnObject.Flag = ret["Flag"].ToString();
                }
            }

            return returnObject;
        }

        public List<TVServerDirs> Return_TVServerDirs_List(string DbFile, string TableName = "[TVServerDirs]", string WhereClause = "")
        {
            string SelectLine = "Select * From " + TableName;

            if (WhereClause != "")
            {
                if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
            }
            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

            List<TVServerDirs> ReturnList = new List<TVServerDirs>();
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    TVServerDirs returnObject = new TVServerDirs();

                    returnObject.DirPath = ret["DirPath"].ToString();
                    returnObject.Enabled = ret["Enabled"].ToString();
                    returnObject.Flag = ret["Flag"].ToString();

                    ReturnList.Add(returnObject);
                }
            }

            return ReturnList;
        }

        public DataTable Return_DataTable_From_TVServerDirs(string DbFile)
        {
            string SelectLine = "Select [DirPath], [Enabled], [Flag] From [TVServerDirs]";
            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
            return ReturnTable;
        }


        #endregion
        #region ===== TVServerDirs SQLite : Update Insert =====

        public bool TVServerDirs_Insert(string DbFile, TVServerDirs inObject)
        {
            string InsertLine = "Insert Into [TVServerDirs] (DirPath, Enabled, Flag) values ('" + inObject.DirPath + "', '" + inObject.Enabled + "', '" + inObject.Flag + "')";
            bool Inserted = sqlite.Execute(DbFile, InsertLine);
            if (!Inserted) { ahk.MsgBox("Inserted Into [TVServerDirs] = " + Inserted.ToString()); }
            return Inserted;
        }

        public bool TVServerDirs_Update(string DbFile, TVServerDirs inObject, string WHERE = "[Item] = 'Value'")
        {
            //string UpdateLine = "Update [TVServerDirs] set DirPath = '" + inObject.DirPath + "', Enabled = '" + inObject.Enabled + "', Flag = '" + inObject.Flag + "' WHERE [Item] = 'Value' ";
            string UpdateLine = "Update [TVServerDirs] set ";


            if (inObject.DirPath != null) { UpdateLine = UpdateLine + "[DirPath] = '" + inObject.DirPath + "',"; }
            if (inObject.Enabled != null) { UpdateLine = UpdateLine + "[Enabled] = '" + inObject.Enabled + "',"; }
            if (inObject.Flag != null) { UpdateLine = UpdateLine + "[Flag] = '" + inObject.Flag + "',"; }

            UpdateLine = ahk.TrimLast(UpdateLine, 1);
            UpdateLine = UpdateLine + " WHERE " + WHERE;

            bool Updated = sqlite.Execute(DbFile, UpdateLine);
            return Updated;
        }

        public bool TVServerDirs_UpdateInsert(string DbFile, TVServerDirs obj, string WhereClause = "")
        {
            bool Updated = TVServerDirs_Update(DbFile, obj, WhereClause);  // try to update record first
            if (!Updated) { Updated = TVServerDirs_Insert(DbFile, obj); }  // if unable to update, insert new record
            return Updated;
        }


        #endregion
        #region ===== TVServerDirs DataTable =====

        public DataTable Return_TVServerDirs_DataTable(string DbFile, string TableName = "TVServerDirs", string WhereClause = "", bool Debug = false)
        {
            string SelectLine = "Select * From [TVServerDirs]";

            if (WhereClause != "")
            {
                if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
            }

            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


            DataTable table = new DataTable();
            table.Columns.Add("DirPath", typeof(string));
            table.Columns.Add("Enabled", typeof(string));
            table.Columns.Add("Flag", typeof(string));

            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    TVServerDirs returnObject = new TVServerDirs();

                    returnObject.DirPath = ret["DirPath"].ToString();
                    returnObject.Enabled = ret["Enabled"].ToString();
                    returnObject.Flag = ret["Flag"].ToString();

                    table.Rows.Add(returnObject.DirPath, returnObject.Enabled, returnObject.Flag);
                }
            }

            return table;
        }

        public DataTable Create_TVServerDirs_DataTable(TVServerDirs inObject)
        {
            DataTable table = new DataTable();
            table.Columns.Add("DirPath", typeof(string));
            table.Columns.Add("Enabled", typeof(string));
            table.Columns.Add("Flag", typeof(string));

            table.Rows.Add(inObject.DirPath, inObject.Enabled, inObject.Flag);
            return table;
        }


        #endregion
        #region ===== TVServerDirs DataGridView =====

        public void HideShow_TVServerDirs_Columns(DataGridView dv)
        {

            try { dv.Columns["DirPath"].Visible = true; } catch { }
            try { dv.Columns["Enabled"].Visible = true; } catch { }
            try { dv.Columns["Flag"].Visible = true; } catch { }
        }
        public void Enable_TableName_Columns(DataGridView dv)
        {

            try { dv.Columns["DirPath"].ReadOnly = true; } catch { }
            try { dv.Columns["Enabled"].ReadOnly = true; } catch { }
            try { dv.Columns["Flag"].ReadOnly = true; } catch { }
        }

        #endregion
        #region ===== TVServerDirs SQL Functions =====

        // Return TVServerDirs SQL Connection String
        public SqlConnection TVServerDirs_Conn()
        {
            // populate sql connection
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLserver"].ConnectionString);
            // SqlConnection Conn = new SqlConnection("Server=188.168.188.88;DataBase=LucidMedia;Uid=lucidm;Pwd=pass");
            return conn;
        }

        // Return TVServerDirs TableName (Full Path)
        public string TVServerDirs_TableName()
        {
            // populate to return full sql table name
            return "[fileserver].[lucidmethod].[TVServerDirs]";
        }

        // Generate SQL Table
        public bool TVServerDirs_CreateSQLTable()
        {
            SqlConnection Conn = TVServerDirs_Conn();
            string CreateTableLine = "CREATE TABLE [TVServerDirs](";
            CreateTableLine = CreateTableLine + "[DirPath] [int] IDENTITY(1,1) NOT NULL,";
            CreateTableLine = CreateTableLine + "[Enabled] [varchar](max) NOT NULL,";
            CreateTableLine = CreateTableLine + "[Flag] [varchar](max) NOT NULL";
            CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
            return false;
        }

        public bool TVServerDirs_InsertSQL(TVServerDirs obj)
        {
            SqlConnection Con = TVServerDirs_Conn();
            string SQLLine = "Insert Into " + TVServerDirs_TableName() + " (DirPath, Enabled, Flag, Cat) VALUES (@DirPath, @Enabled, @Flag, @Cat)";
            SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
            cmd2 = new SqlCommand(SQLLine, Con);
            if (obj.DirPath == null) { obj.DirPath = ""; }
            if (obj.Enabled == null) { obj.Enabled = ""; }
            if (obj.Flag == null) { obj.Flag = ""; }
            cmd2.Parameters.AddWithValue(@"DirPath", obj.DirPath.ToString());
            cmd2.Parameters.AddWithValue(@"Enabled", obj.Enabled.ToString());
            cmd2.Parameters.AddWithValue(@"Flag", obj.Flag.ToString());
            cmd2.Parameters.AddWithValue(@"Cat", obj.Cat.ToString());
            if (Con.State == ConnectionState.Closed) { Con.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
            Con.Close();
            if (recordsAffected > 0) { return true; }
            else return false;
        }

        public bool TVServerDirs_UpdateSQL(TVServerDirs obj)
        {
            SqlConnection Conn = TVServerDirs_Conn();
            string SQLLine = "Update " + TVServerDirs_TableName() + " SET DirPath = @DirPath, Enabled = @Enabled, Flag = @Flag, Cat = @Cat WHERE DirPath = @DirPath";
            SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
            cmd2 = new SqlCommand(SQLLine, Conn);
            if (obj.DirPath == null) { obj.DirPath = ""; }
            if (obj.Enabled == null) { obj.Enabled = ""; }
            if (obj.Flag == null) { obj.Flag = ""; }
            cmd2.Parameters.AddWithValue(@"DirPath", obj.DirPath.ToString());
            cmd2.Parameters.AddWithValue(@"Enabled", obj.Enabled.ToString());
            cmd2.Parameters.AddWithValue(@"Flag", obj.Flag.ToString());
            cmd2.Parameters.AddWithValue(@"Cat", obj.Cat.ToString());
            if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
            Conn.Close();
            if (recordsAffected > 0) { return true; }
            else return false;
        }

        public bool TVServerDirs_UpdateInsert(TVServerDirs obj)
        {
            SqlConnection Conn = TVServerDirs_Conn();
            bool Updated = TVServerDirs_UpdateSQL(obj);  // try to update record first
            if (!Updated) { Updated = TVServerDirs_InsertSQL(obj); }  // if unable to update, insert new record
            return Updated;
        }

        // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
        public bool TVServerDirs_UpdateIfPopulated(TVServerDirs obj, string ID = "")
        {
            SqlConnection Conn = TVServerDirs_Conn();
            string SQLcmd = "Update " + TVServerDirs_TableName() + " SET ";
            if (obj.DirPath != null) { SQLcmd = SQLcmd + " DirPath = @DirPath,"; }
            if (obj.Enabled != null) { SQLcmd = SQLcmd + " Enabled = @Enabled,"; }
            if (obj.Flag != null) { SQLcmd = SQLcmd + " Flag = @Flag,"; }
            SQLcmd = ahk.TrimLast(SQLcmd, 1);
            SQLcmd = SQLcmd + " WHERE ID = @ID";

            SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

            if (obj.DirPath != null) { cmd2.Parameters.AddWithValue(@"DirPath", obj.DirPath); }
            if (obj.Enabled != null) { cmd2.Parameters.AddWithValue(@"Enabled", obj.Enabled); }
            if (obj.Flag != null) { cmd2.Parameters.AddWithValue(@"Flag", obj.Flag); }

            if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
            Conn.Close();
            if (recordsAffected > 0) { return true; }
            else return false;
        }

        public TVServerDirs TVServerDirs_ReturnSQL(string ID = "")
        {
            SqlConnection Conn = TVServerDirs_Conn();
            string SelectLine = "Select [DirPath],[Enabled],[Flag] From " + TVServerDirs_TableName() + " WHERE ID = '" + ID + "'";
            DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
            TVServerDirs returnObject = new TVServerDirs();
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    returnObject.DirPath = ret["DirPath"].ToString();
                    returnObject.Enabled = ret["Enabled"].ToString();
                    returnObject.Flag = ret["Flag"].ToString();
                    return returnObject;
                }
            }
            return returnObject;
        }

        public List<TVServerDirs> TVServerDirs_ReturnSQLList(string Command)
        {
            SqlConnection Conn = TVServerDirs_Conn();
            DataTable ReturnTable = sql.GetDataTable(Conn, Command);
            List<TVServerDirs> ReturnList = new List<TVServerDirs>();
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    TVServerDirs returnObject = new TVServerDirs();
                    returnObject.DirPath = ret["DirPath"].ToString();
                    returnObject.Enabled = ret["Enabled"].ToString();
                    returnObject.Flag = ret["Flag"].ToString();
                    ReturnList.Add(returnObject);
                }
            }
            return ReturnList;
        }


        #endregion

        #endregion


        #region === TVServerShows FUNCTIONS ===

        #region ===== TVServerShows Object =====

        public struct TVServerShows
        {
            public string RootDir { get; set; }
            public string DirPath { get; set; }
            public string DirName { get; set; }
            public string EpGuideURL { get; set; }
            public string IMDbURL { get; set; }
            public string Flag { get; set; }
        }
        public TVServerShows Return_TVServerShows(string RootDir = "", string DirPath = "", string DirName = "", string EpGuideURL = "", string IMDbURL = "", string Flag = "")
        {
            TVServerShows obj = new TVServerShows();
            obj.RootDir = RootDir;
            obj.DirPath = DirPath;
            obj.DirName = DirName;
            obj.EpGuideURL = EpGuideURL;
            obj.IMDbURL = IMDbURL;
            obj.Flag = Flag;

            return obj;
        }

        //  Fix illegal characters before Sql/Sqlite Db Inserts
        public TVServerShows TVServerShows_FixChars(TVServerShows ToFix)
        {
            TVServerShows Fixed = new TVServerShows();

            Fixed.RootDir = ToFix.RootDir.Replace("'", "''");
            Fixed.DirPath = ToFix.DirPath.Replace("'", "''");
            Fixed.DirName = ToFix.DirName.Replace("'", "''");
            Fixed.EpGuideURL = ToFix.EpGuideURL.Replace("'", "''");
            Fixed.IMDbURL = ToFix.IMDbURL.Replace("'", "''");
            Fixed.Flag = ToFix.Flag.Replace("'", "''");

            return Fixed;
        }

        // Compare two objects to see if they have identical values
        public bool TVServerShows_Changed(TVServerShows OldVal, TVServerShows NewVal)
        {
            TVServerShows diff = new TVServerShows();
            List<string> diffList = new List<string>();
            bool different = false;
            if (OldVal.RootDir == null) { OldVal.RootDir = ""; }
            if (NewVal.RootDir == null) { NewVal.RootDir = ""; }
            if (OldVal.RootDir != NewVal.RootDir) { different = true; }
            if (OldVal.DirPath == null) { OldVal.DirPath = ""; }
            if (NewVal.DirPath == null) { NewVal.DirPath = ""; }
            if (OldVal.DirPath != NewVal.DirPath) { different = true; }
            if (OldVal.DirName == null) { OldVal.DirName = ""; }
            if (NewVal.DirName == null) { NewVal.DirName = ""; }
            if (OldVal.DirName != NewVal.DirName) { different = true; }
            if (OldVal.EpGuideURL == null) { OldVal.EpGuideURL = ""; }
            if (NewVal.EpGuideURL == null) { NewVal.EpGuideURL = ""; }
            if (OldVal.EpGuideURL != NewVal.EpGuideURL) { different = true; }
            if (OldVal.IMDbURL == null) { OldVal.IMDbURL = ""; }
            if (NewVal.IMDbURL == null) { NewVal.IMDbURL = ""; }
            if (OldVal.IMDbURL != NewVal.IMDbURL) { different = true; }
            if (OldVal.Flag == null) { OldVal.Flag = ""; }
            if (NewVal.Flag == null) { NewVal.Flag = ""; }
            if (OldVal.Flag != NewVal.Flag) { different = true; }
            return different;
        }

        // Returns object containing the new values different from the old values in object comparison
        public TVServerShows TVServerShows_Diff(TVServerShows OldVal, TVServerShows NewVal)
        {
            TVServerShows diff = new TVServerShows();
            if (OldVal.RootDir != NewVal.RootDir) { diff.RootDir = NewVal.RootDir; }
            if (OldVal.DirPath != NewVal.DirPath) { diff.DirPath = NewVal.DirPath; }
            if (OldVal.DirName != NewVal.DirName) { diff.DirName = NewVal.DirName; }
            if (OldVal.EpGuideURL != NewVal.EpGuideURL) { diff.EpGuideURL = NewVal.EpGuideURL; }
            if (OldVal.IMDbURL != NewVal.IMDbURL) { diff.IMDbURL = NewVal.IMDbURL; }
            if (OldVal.Flag != NewVal.Flag) { diff.Flag = NewVal.Flag; }
            return diff;
        }

        // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
        public List<string> TVServerShows_DiffList(TVServerShows OldVal, TVServerShows NewVal)
        {
            List<string> diffList = new List<string>();
            if (OldVal.RootDir != NewVal.RootDir) { diffList.Add("Changed RootDir Value From " + OldVal.RootDir + " To " + NewVal.RootDir); }
            if (OldVal.DirPath != NewVal.DirPath) { diffList.Add("Changed DirPath Value From " + OldVal.DirPath + " To " + NewVal.DirPath); }
            if (OldVal.DirName != NewVal.DirName) { diffList.Add("Changed DirName Value From " + OldVal.DirName + " To " + NewVal.DirName); }
            if (OldVal.EpGuideURL != NewVal.EpGuideURL) { diffList.Add("Changed EpGuideURL Value From " + OldVal.EpGuideURL + " To " + NewVal.EpGuideURL); }
            if (OldVal.IMDbURL != NewVal.IMDbURL) { diffList.Add("Changed IMDbURL Value From " + OldVal.IMDbURL + " To " + NewVal.IMDbURL); }
            if (OldVal.Flag != NewVal.Flag) { diffList.Add("Changed Flag Value From " + OldVal.Flag + " To " + NewVal.Flag); }
            return diffList;
        }


        #endregion
        #region ===== TVServerShows SQLite : Return =====

        public TVServerShows Return_Object_From_TVServerShows(string DbFile, string WhereClause = "[RootDir] = ''")
        {
            string SelectLine = "Select [RootDir], [DirPath], [DirName], [EpGuideURL], [IMDbURL], [Flag] From [TVServerShows] ";
            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
            if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
            if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
            TVServerShows returnObject = new TVServerShows();
            int i = 0;
            string Value = "";
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    returnObject.RootDir = ret["RootDir"].ToString();
                    returnObject.DirPath = ret["DirPath"].ToString();
                    returnObject.DirName = ret["DirName"].ToString();
                    returnObject.EpGuideURL = ret["EpGuideURL"].ToString();
                    returnObject.IMDbURL = ret["IMDbURL"].ToString();
                    returnObject.Flag = ret["Flag"].ToString();
                }
            }

            return returnObject;
        }

        public List<TVServerShows> Return_TVServerShows_List(string DbFile, string TableName = "[TVServerShows]", string WhereClause = "")
        {
            string SelectLine = "Select * From " + TableName;

            if (WhereClause != "")
            {
                if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
            }
            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

            List<TVServerShows> ReturnList = new List<TVServerShows>();
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    TVServerShows returnObject = new TVServerShows();

                    returnObject.RootDir = ret["RootDir"].ToString();
                    returnObject.DirPath = ret["DirPath"].ToString();
                    returnObject.DirName = ret["DirName"].ToString();
                    returnObject.EpGuideURL = ret["EpGuideURL"].ToString();
                    returnObject.IMDbURL = ret["IMDbURL"].ToString();
                    returnObject.Flag = ret["Flag"].ToString();

                    ReturnList.Add(returnObject);
                }
            }

            return ReturnList;
        }

        public DataTable Return_DataTable_From_TVServerShows(string DbFile)
        {
            string SelectLine = "Select [RootDir], [DirPath], [DirName], [EpGuideURL], [IMDbURL], [Flag] From [TVServerShows]";
            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
            return ReturnTable;
        }


        #endregion
        #region ===== TVServerShows SQLite : Update Insert =====

        public bool TVServerShows_Insert(string DbFile, TVServerShows inObject)
        {
            string InsertLine = "Insert Into [TVServerShows] (RootDir, DirPath, DirName, EpGuideURL, IMDbURL, Flag) values ('" + inObject.RootDir + "', '" + inObject.DirPath + "', '" + inObject.DirName + "', '" + inObject.EpGuideURL + "', '" + inObject.IMDbURL + "', '" + inObject.Flag + "')";
            bool Inserted = sqlite.Execute(DbFile, InsertLine);
            if (!Inserted) { ahk.MsgBox("Inserted Into [TVServerShows] = " + Inserted.ToString()); }
            return Inserted;
        }

        public bool TVServerShows_Update(string DbFile, TVServerShows inObject, string WHERE = "[Item] = 'Value'")
        {
            //string UpdateLine = "Update [TVServerShows] set RootDir = '" + inObject.RootDir + "', DirPath = '" + inObject.DirPath + "', DirName = '" + inObject.DirName + "', EpGuideURL = '" + inObject.EpGuideURL + "', IMDbURL = '" + inObject.IMDbURL + "', Flag = '" + inObject.Flag + "' WHERE [Item] = 'Value' ";
            string UpdateLine = "Update [TVServerShows] set ";


            if (inObject.RootDir != null) { UpdateLine = UpdateLine + "[RootDir] = '" + inObject.RootDir + "',"; }
            if (inObject.DirPath != null) { UpdateLine = UpdateLine + "[DirPath] = '" + inObject.DirPath + "',"; }
            if (inObject.DirName != null) { UpdateLine = UpdateLine + "[DirName] = '" + inObject.DirName + "',"; }
            if (inObject.EpGuideURL != null) { UpdateLine = UpdateLine + "[EpGuideURL] = '" + inObject.EpGuideURL + "',"; }
            if (inObject.IMDbURL != null) { UpdateLine = UpdateLine + "[IMDbURL] = '" + inObject.IMDbURL + "',"; }
            if (inObject.Flag != null) { UpdateLine = UpdateLine + "[Flag] = '" + inObject.Flag + "',"; }

            UpdateLine = ahk.TrimLast(UpdateLine, 1);
            UpdateLine = UpdateLine + " WHERE " + WHERE;

            bool Updated = sqlite.Execute(DbFile, UpdateLine);
            return Updated;
        }

        public bool TVServerShows_UpdateInsert(string DbFile, TVServerShows obj, string WhereClause = "")
        {
            bool Updated = TVServerShows_Update(DbFile, obj, WhereClause);  // try to update record first
            if (!Updated) { Updated = TVServerShows_Insert(DbFile, obj); }  // if unable to update, insert new record
            return Updated;
        }


        #endregion
        #region ===== TVServerShows DataTable =====

        public DataTable Return_TVServerShows_DataTable(string DbFile, string TableName = "TVServerShows", string WhereClause = "", bool Debug = false)
        {
            string SelectLine = "Select * From [TVServerShows]";

            if (WhereClause != "")
            {
                if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
            }

            DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


            DataTable table = new DataTable();
            table.Columns.Add("RootDir", typeof(string));
            table.Columns.Add("DirPath", typeof(string));
            table.Columns.Add("DirName", typeof(string));
            table.Columns.Add("EpGuideURL", typeof(string));
            table.Columns.Add("IMDbURL", typeof(string));
            table.Columns.Add("Flag", typeof(string));

            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    TVServerShows returnObject = new TVServerShows();

                    returnObject.RootDir = ret["RootDir"].ToString();
                    returnObject.DirPath = ret["DirPath"].ToString();
                    returnObject.DirName = ret["DirName"].ToString();
                    returnObject.EpGuideURL = ret["EpGuideURL"].ToString();
                    returnObject.IMDbURL = ret["IMDbURL"].ToString();
                    returnObject.Flag = ret["Flag"].ToString();

                    table.Rows.Add(returnObject.RootDir, returnObject.DirPath, returnObject.DirName, returnObject.EpGuideURL, returnObject.IMDbURL, returnObject.Flag);
                }
            }

            return table;
        }

        public DataTable Create_TVServerShows_DataTable(TVServerShows inObject)
        {
            DataTable table = new DataTable();
            table.Columns.Add("RootDir", typeof(string));
            table.Columns.Add("DirPath", typeof(string));
            table.Columns.Add("DirName", typeof(string));
            table.Columns.Add("EpGuideURL", typeof(string));
            table.Columns.Add("IMDbURL", typeof(string));
            table.Columns.Add("Flag", typeof(string));

            table.Rows.Add(inObject.RootDir, inObject.DirPath, inObject.DirName, inObject.EpGuideURL, inObject.IMDbURL, inObject.Flag);
            return table;
        }


        #endregion
        #region ===== TVServerShows DataGridView =====

        public void HideShow_TVServerShows_Columns(DataGridView dv)
        {

            try { dv.Columns["RootDir"].Visible = true; } catch { }
            try { dv.Columns["DirPath"].Visible = true; } catch { }
            try { dv.Columns["DirName"].Visible = true; } catch { }
            try { dv.Columns["EpGuideURL"].Visible = true; } catch { }
            try { dv.Columns["IMDbURL"].Visible = true; } catch { }
            try { dv.Columns["Flag"].Visible = true; } catch { }
        }
        public void Enable_TableName_ColumnsA(DataGridView dv)
        {

            try { dv.Columns["RootDir"].ReadOnly = true; } catch { }
            try { dv.Columns["DirPath"].ReadOnly = true; } catch { }
            try { dv.Columns["DirName"].ReadOnly = true; } catch { }
            try { dv.Columns["EpGuideURL"].ReadOnly = true; } catch { }
            try { dv.Columns["IMDbURL"].ReadOnly = true; } catch { }
            try { dv.Columns["Flag"].ReadOnly = true; } catch { }
        }

        #endregion
        #region ===== TVServerShows SQL Functions =====

        // Return TVServerShows SQL Connection String
        public SqlConnection TVServerShows_Conn()
        {
            // populate sql connection
            SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLserver"].ConnectionString);
            // SqlConnection Conn = new SqlConnection("Server=188.168.188.88;DataBase=LucidMedia;Uid=lucidm;Pwd=pass");
            return conn;
        }

        // Return TVServerShows TableName (Full Path)
        public string TVServerShows_TableName()
        {
            // populate to return full sql table name
            return "[fileserver].[lucidmethod].[TVServerShows]";
        }

        // Generate SQL Table
        public bool TVServerShows_CreateSQLTable()
        {
            SqlConnection Conn = TVServerShows_Conn();
            string CreateTableLine = "CREATE TABLE [TVServerShows](";
            CreateTableLine = CreateTableLine + "[RootDir] [int] IDENTITY(1,1) NOT NULL,";
            CreateTableLine = CreateTableLine + "[DirPath] [varchar](max) NOT NULL,";
            CreateTableLine = CreateTableLine + "[DirName] [varchar](max) NOT NULL,";
            CreateTableLine = CreateTableLine + "[EpGuideURL] [varchar](max) NOT NULL,";
            CreateTableLine = CreateTableLine + "[IMDbURL] [varchar](max) NOT NULL,";
            CreateTableLine = CreateTableLine + "[Flag] [varchar](max) NOT NULL";
            CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
            return false;
        }

        public bool TVServerShows_InsertSQL(TVServerShows obj)
        {
            SqlConnection Con = TVServerShows_Conn();
            string SQLLine = "Insert Into " + TVServerShows_TableName() + " (RootDir, DirPath, DirName, EpGuideURL, IMDbURL, Flag) VALUES (@RootDir, @DirPath, @DirName, @EpGuideURL, @IMDbURL, @Flag)";
            SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
            cmd2 = new SqlCommand(SQLLine, Con);
            if (obj.RootDir == null) { obj.RootDir = ""; }
            if (obj.DirPath == null) { obj.DirPath = ""; }
            if (obj.DirName == null) { obj.DirName = ""; }
            if (obj.EpGuideURL == null) { obj.EpGuideURL = ""; }
            if (obj.IMDbURL == null) { obj.IMDbURL = ""; }
            if (obj.Flag == null) { obj.Flag = ""; }
            cmd2.Parameters.AddWithValue(@"RootDir", obj.RootDir.ToString());
            cmd2.Parameters.AddWithValue(@"DirPath", obj.DirPath.ToString());
            cmd2.Parameters.AddWithValue(@"DirName", obj.DirName.ToString());
            cmd2.Parameters.AddWithValue(@"EpGuideURL", obj.EpGuideURL.ToString());
            cmd2.Parameters.AddWithValue(@"IMDbURL", obj.IMDbURL.ToString());
            cmd2.Parameters.AddWithValue(@"Flag", obj.Flag.ToString());
            if (Con.State == ConnectionState.Closed) { Con.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
            Con.Close();
            if (recordsAffected > 0) { return true; }
            else return false;
        }

        public bool TVServerShows_UpdateSQL(TVServerShows obj)
        {
            SqlConnection Conn = TVServerShows_Conn();
            string SQLLine = "Update " + TVServerShows_TableName() + " SET RootDir = @RootDir, DirName = @DirName, EpGuideURL = @EpGuideURL, IMDbURL = @IMDbURL, Flag = @Flag WHERE DirPath = @DirPath";
            SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
            cmd2 = new SqlCommand(SQLLine, Conn);
            if (obj.RootDir == null) { obj.RootDir = ""; }
            if (obj.DirPath == null) { obj.DirPath = ""; }
            if (obj.DirName == null) { obj.DirName = ""; }
            if (obj.EpGuideURL == null) { obj.EpGuideURL = ""; }
            if (obj.IMDbURL == null) { obj.IMDbURL = ""; }
            if (obj.Flag == null) { obj.Flag = ""; }
            cmd2.Parameters.AddWithValue(@"RootDir", obj.RootDir.ToString());
            cmd2.Parameters.AddWithValue(@"DirPath", obj.DirPath.ToString());
            cmd2.Parameters.AddWithValue(@"DirName", obj.DirName.ToString());
            cmd2.Parameters.AddWithValue(@"EpGuideURL", obj.EpGuideURL.ToString());
            cmd2.Parameters.AddWithValue(@"IMDbURL", obj.IMDbURL.ToString());
            cmd2.Parameters.AddWithValue(@"Flag", obj.Flag.ToString());
            if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
            Conn.Close();
            if (recordsAffected > 0) { return true; }
            else return false;
        }

        public bool TVServerShows_UpdateInsert(TVServerShows obj)
        {
            SqlConnection Conn = TVServerShows_Conn();
            bool Updated = TVServerShows_UpdateSQL(obj);  // try to update record first
            if (!Updated) { Updated = TVServerShows_InsertSQL(obj); }  // if unable to update, insert new record
            return Updated;
        }

        // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
        public bool TVServerShows_UpdateIfPopulated(TVServerShows obj, string ID = "")
        {
            SqlConnection Conn = TVServerShows_Conn();
            string SQLcmd = "Update " + TVServerShows_TableName() + " SET ";
            if (obj.RootDir != null) { SQLcmd = SQLcmd + " RootDir = @RootDir,"; }
            if (obj.DirPath != null) { SQLcmd = SQLcmd + " DirPath = @DirPath,"; }
            if (obj.DirName != null) { SQLcmd = SQLcmd + " DirName = @DirName,"; }
            if (obj.EpGuideURL != null) { SQLcmd = SQLcmd + " EpGuideURL = @EpGuideURL,"; }
            if (obj.IMDbURL != null) { SQLcmd = SQLcmd + " IMDbURL = @IMDbURL,"; }
            if (obj.Flag != null) { SQLcmd = SQLcmd + " Flag = @Flag,"; }
            SQLcmd = ahk.TrimLast(SQLcmd, 1);
            SQLcmd = SQLcmd + " WHERE ID = @ID";

            SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

            if (obj.RootDir != null) { cmd2.Parameters.AddWithValue(@"RootDir", obj.RootDir); }
            if (obj.DirPath != null) { cmd2.Parameters.AddWithValue(@"DirPath", obj.DirPath); }
            if (obj.DirName != null) { cmd2.Parameters.AddWithValue(@"DirName", obj.DirName); }
            if (obj.EpGuideURL != null) { cmd2.Parameters.AddWithValue(@"EpGuideURL", obj.EpGuideURL); }
            if (obj.IMDbURL != null) { cmd2.Parameters.AddWithValue(@"IMDbURL", obj.IMDbURL); }
            if (obj.Flag != null) { cmd2.Parameters.AddWithValue(@"Flag", obj.Flag); }

            if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
            int recordsAffected = 0;
            try { recordsAffected = cmd2.ExecuteNonQuery(); }
            catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
            Conn.Close();
            if (recordsAffected > 0) { return true; }
            else return false;
        }

        public TVServerShows TVServerShows_ReturnSQL(string ID = "")
        {
            SqlConnection Conn = TVServerShows_Conn();
            string SelectLine = "Select [RootDir],[DirPath],[DirName],[EpGuideURL],[IMDbURL],[Flag] From " + TVServerShows_TableName() + " WHERE ID = '" + ID + "'";
            DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
            TVServerShows returnObject = new TVServerShows();
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    returnObject.RootDir = ret["RootDir"].ToString();
                    returnObject.DirPath = ret["DirPath"].ToString();
                    returnObject.DirName = ret["DirName"].ToString();
                    returnObject.EpGuideURL = ret["EpGuideURL"].ToString();
                    returnObject.IMDbURL = ret["IMDbURL"].ToString();
                    returnObject.Flag = ret["Flag"].ToString();
                    return returnObject;
                }
            }
            return returnObject;
        }

        public List<TVServerShows> TVServerShows_ReturnSQLList(string Command)
        {
            SqlConnection Conn = TVServerShows_Conn();
            DataTable ReturnTable = sql.GetDataTable(Conn, Command);
            List<TVServerShows> ReturnList = new List<TVServerShows>();
            if (ReturnTable != null)
            {
                foreach (DataRow ret in ReturnTable.Rows)
                {
                    TVServerShows returnObject = new TVServerShows();
                    returnObject.RootDir = ret["RootDir"].ToString();
                    returnObject.DirPath = ret["DirPath"].ToString();
                    returnObject.DirName = ret["DirName"].ToString();
                    returnObject.EpGuideURL = ret["EpGuideURL"].ToString();
                    returnObject.IMDbURL = ret["IMDbURL"].ToString();
                    returnObject.Flag = ret["Flag"].ToString();
                    ReturnList.Add(returnObject);
                }
            }
            return ReturnList;
        }


        #endregion

        #endregion


    }


    #endregion


    #region === DATABASES ===

    public partial class _Database
    { 
    public class Tags
    {
        private static _AHK ahk = new _AHK();
        private static _Database.SQLite sqlite = new _Database.SQLite();
        private static _Database.SQL sql = new _Database.SQL();
        private static _GridControl grid = new _GridControl();


        #region === Tags: Create ===

        /// <summary> create a tag table in Sqlite Db File</summary>
        /// <param name="DbFile"> </param>
        /// <param name="TableName"> </param>
        public bool Create_Tag_Db(string DbFile, string TableName = "Tags")
        {
            // create database file if it doen't exist
            if (!File.Exists(DbFile)) { SQLiteConnection.CreateFile(DbFile); }

            // Connect to the DB
            SQLiteConnection m_dbConnection = sqlite.Connect(DbFile); // connect to SQLite DB file path - returns connection data


            // Create New Table If It Does NOT Exist Yet
            bool TableExist = sqlite.Table_Exists(DbFile, TableName);  //See if selected Table Exists in SQLite DB file

            if (!TableExist)  // Table DOES NOT exist in SQLite DB
            {
                string NewTableLine = "";

                if (TableName == "Tags") { NewTableLine = "ID INTEGER PRIMARY KEY, Tag_Group VARCHAR, Tag_Category VARCHAR, Tag VARCHAR"; }

                //ahk.MsgBox(NewTableLine); 

                bool ReturnValue = sqlite.Execute(DbFile, "CREATE TABLE " + TableName + " (" + NewTableLine + ")");  // Create a Table [Only execute once - Will Error 2nd Attempt]
                return ReturnValue;
            }

            return true;
        }

        /// <summary> insert a tag into the sqlite tag db file</summary>
        /// <param name="DbFile"> </param>
        /// <param name=" Tag_Group"> </param>
        /// <param name=" Tag_Category"> </param>
        /// <param name=" Tag"> </param>
        /// <param name="TableName"> </param>
        public bool InsertUpdate_Tag(string DbFile, string Tag_Group, string Tag_Category, string Tag, string WhereClause = "", string TableName = "Tags")
        {
            bool Updated = false;

            if (WhereClause != "") // try to update before inserting if where clause is provided
            {
                sqlite.Execute(DbFile, "UPDATE " + TableName + " set Tag_Group = '" + Tag_Group + "', Tag_Category = '" + Tag_Category + "', Tag = '" + Tag + "' WHERE " + WhereClause);  // Update Table
                if (!Updated) { Updated = sqlite.Execute(DbFile, "INSERT into " + TableName + " (Tag_Group, Tag_Category, Tag) values ('" + Tag_Group + "','" + Tag_Category + "','" + Tag + "')"); }  // insert into a Table
            }
            else
            {
                Updated = sqlite.Execute(DbFile, "INSERT into " + TableName + " (Tag_Group, Tag_Category, Tag) values ('" + Tag_Group + "','" + Tag_Category + "','" + Tag + "')");
            }

            if (!Updated) { ahk.MsgBox("Failed to Insert: Tag " + Tag); }
            return Updated;
        }

        /// <summary>dlete tag from tagdb index</summary>
        /// <param name="DbFile"> </param>
        /// <param name=" Tag_Group"> </param>
        /// <param name=" Tag_Category"> </param>
        /// <param name=" Tag"> </param>
        /// <param name="TableName"> </param>
        public bool Delete_Tag(string DbFile, string Tag_Group, string Tag_Category, string Tag, string TableName = "Tags")
        {
            bool Updated = sqlite.Execute(DbFile, "Delete From " + TableName + " WHERE Tag_Group = '" + Tag_Group + "' and Tag_Category = '" + Tag_Category + "' and Tag = '" + Tag + "'");

            if (!Updated) { ahk.MsgBox("Failed to Delete: Tag " + Tag); }
            return Updated;
        }

        #endregion

        #region === Tag TreeView ===

        //======== After Select Action

        //// example use: just place after select reference in your project's treeview AfterSelect action
        //private void tvTags_AfterSelect(object sender, TreeViewEventArgs e)
        //{
        //    tg.TagTreeView_AfterSelect(sender, e);
        //}

        // optional controls to assign for after select display

        public Control TagDisplay = new Control();
        public Control TagGroupDisplay = new Control();
        public Control TagCategoryDisplay = new Control();

        public void TagTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            //=== Grab Selected Value and Node's Parent Name ===
            TreeNode node = e.Node;
            if (node == null) { return; }  //nothing to do if null value passed while user is clicking


            string SelectedValue = node.Text;        // value of selected node
            string SelectedParent = "";
            string FullPath = node.FullPath.ToString();   // full path with all nodes + file name
            string FilePath = "";

            string tagCategory = ""; string tagGroup = ""; string tag = ""; string TagOption = "";

            // if there is a tag provided, parse out parameters
            if (node.Tag != null)
            {
                string TagLine = node.Tag.ToString();

                // parse out tagline for tag parameters
                if (TagLine.Contains("|"))
                {
                    string[] tags = TagLine.Split('|');
                    int i = 0;

                    bool isGroup = false; bool isCat = false; bool isTag = false;
                    foreach (string word in tags)
                    {
                        if (i == 0)
                        {
                            if (word == "Group") { isGroup = true; }
                            if (word == "Category") { isCat = true; }
                            if (word == "Tag") { isTag = true; }
                        }

                        if (i == 1)
                        {
                            if (isGroup) { tagGroup = word; }
                            if (isCat) { tagCategory = word; }
                            if (isTag) { tag = word; }
                        }

                        if (i == 3)
                        {
                            if (isCat) { tagGroup = word; }
                            if (isTag) { tagCategory = word; }
                        }

                        if (i == 5)
                        {
                            if (isTag) { tagGroup = word; }
                        }

                        i++;
                    }
                }
            }


            Control tTag = TagDisplay;
            Control tTagCategory = TagCategoryDisplay;
            Control tTagGroup = TagGroupDisplay;

            if (tTag.Handle != null) { tTag.Text = tag; }
            if (tTagCategory.Handle != null) { tTagCategory.Text = tagCategory; }
            if (tTagGroup.Handle != null) { tTagGroup.Text = tagGroup; }
        }




        #endregion

        #region === Tags: Return ===

        // Return Tags As List

        /// <summary> returns list of tag groups from tag sqlite db file</summary>
        /// <param name="DbFile"> </param>
        /// <param name="TableName"> </param>
        public List<string> Return_Tag_Groups(string DbFile, string TableName = "Tags")
        {
            SQLiteConnection m_dbConnection = sqlite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

            string SearchLine = "Select Distinct [Tag_Group] from [" + TableName + "]";
            SQLiteDataReader reader = sqlite.ReturnSQLite(SearchLine, m_dbConnection);  // request data from DB
            List<string> TagGroups = new List<string>();

            while (reader.Read())    // loop through each row returned from select 
            {
                string Group = reader["Tag_Group"].ToString();
                TagGroups.Add(Group);
            }

            sqlite.Disconnect(m_dbConnection);  // free up db for other use

            return TagGroups;
        }

        /// <summary> returns list of tag categories from tag sqlite db file</summary>
        /// <param name="DbFile"> </param>
        /// <param name=" Tag_Group"> </param>
        /// <param name="TableName"> </param>
        public List<string> Return_Tag_Category(string DbFile, string Tag_Group, string TableName = "Tags")
        {
            SQLiteConnection m_dbConnection = sqlite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

            string SearchLine = "Select Distinct [Tag_Category] from [" + TableName + "] WHERE Tag_Group = '" + Tag_Group + "'";
            SQLiteDataReader reader = sqlite.ReturnSQLite(SearchLine, m_dbConnection);  // request data from DB
            List<string> TagCategories = new List<string>();

            while (reader.Read())    // loop through each row returned from select 
            {
                string Group = reader["Tag_Category"].ToString();
                TagCategories.Add(Group);
            }

            sqlite.Disconnect(m_dbConnection);  // free up db for other use

            return TagCategories;
        }

        /// <summary> returns list of tags from tag sqlite db file</summary>
        /// <param name="DbFile"> </param>
        /// <param name=" Tag_Group"> </param>
        /// <param name=" Tag_Category"> </param>
        /// <param name="TableName"> </param>
        public List<string> Return_Tags(string DbFile, string Tag_Group, string Tag_Category, string TableName = "Tags")
        {
            SQLiteConnection m_dbConnection = sqlite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

            string SearchLine = "Select Distinct [Tag] from [" + TableName + "] WHERE Tag_Group = '" + Tag_Group + "' AND Tag_Category = '" + Tag_Category + "'";
            SQLiteDataReader reader = sqlite.ReturnSQLite(SearchLine, m_dbConnection);  // request data from DB
            List<string> Tags = new List<string>();

            while (reader.Read())    // loop through each row returned from select 
            {
                string Group = reader["Tag"].ToString();
                Tags.Add(Group);
            }

            sqlite.Disconnect(m_dbConnection);  // free up db for other use

            return Tags;
        }

        #endregion

        #region === Tags: Display ===

        /// <summary>Populates Tag TreeView with Contents of Tab Table</summary>
        /// <param name="TreeView tv">TreeView control to populate</param>
        /// <param name=" DbFile">SQLite database file path</param>
        /// <param name="TableName">Table name to read Tags from. Default = "Tags"</param>
        public void Populate_TagTree(TreeView tv, string DbFile, bool ClearTV = true, string TableName = "Tags")
        {
            if (ClearTV) { tv.Nodes.Clear(); }

            // pull list of categories
            List<string> TagGroups = Return_Tag_Groups(DbFile, TableName);

            TreeNode parent = new TreeNode();  // level 1
            foreach (string Group in TagGroups)
            {
                parent = new TreeNode();  // level 1
                parent.Text = Group;
                parent.Tag = "Group|" + Group;

                // pull list of groups for each category
                List<string> TagCategories = Return_Tag_Category(DbFile, Group, TableName);
                foreach (string Category in TagCategories)
                {
                    TreeNode section = new TreeNode();  // level 2
                    section.Text = Category;
                    section.Tag = "Category|" + Category + "|Group|" + Group;
                    parent.Nodes.Add(section);

                    // pull each tag for each group + category
                    List<string> Tags = Return_Tags(DbFile, Group, Category, TableName);
                    foreach (string Tag in Tags)
                    {
                        TreeNode entry = new TreeNode();  // level 3
                        entry.Text = Tag;
                        entry.Tag = "Tag|" + Tag + "|Category|" + Category + "|Group|" + Group;
                        section.Nodes.Add(entry);
                    }
                }
            }

            tv.Nodes.Add(parent);  // populate tree
        }



        /// <summary> populate datagridview with tag list (all)</summary>
        /// <param name="DataGridView dv"> </param>
        /// <param name=" DbFile"> </param>
        /// <param name="SQLite"> </param>
        public int Populate_TagGrid(DataGridView dv, string DbFile, string SQLite = "Select * From [Tags]")
        {
            int RowCount = grid.SQLite(dv, DbFile, SQLite);
            return RowCount;
        }


        /// <summary> display tagged functions from [FunctionLib] in datagridview</summary>
        /// <param name="DataGridView dv"> </param>
        /// <param name=" DbFile"> </param>
        /// <param name="TagName"> </param>
        /// <param name="HideFunctionFields"> </param>
        public void Display_Function_Tags(DataGridView dv, string DbFile, string TagName = "DataGridView", bool HideFunctionFields = true)
        {
            grid.SQLite(dv, DbFile, "Select * From [FunctionLib] WHERE Tags LIKE '%" + TagName + "%' order by Function", false);

            if (HideFunctionFields) { Hide_Extra_Function_Fields(dv); }
        }

        /// <summary> hide function fields in gridview when displaying functionlib search results</summary>
        /// <param name="DataGridView dv"> </param>
        public void Hide_Extra_Function_Fields(DataGridView dv)
        {
            grid.HideColumn(dv, 0);
            grid.HideColumn(dv, 1);
            grid.HideColumn(dv, 2);
            grid.HideColumn(dv, 3);
            grid.HideColumn(dv, 5);
            grid.HideColumn(dv, 6);
            grid.HideColumn(dv, 7);
            grid.HideColumn(dv, 9);
            grid.HideColumn(dv, 10);
            grid.HideColumn(dv, 11);
            grid.HideColumn(dv, 12);
            grid.HideColumn(dv, 13);
            grid.HideColumn(dv, 14);
            grid.HideColumn(dv, 15);
            grid.HideColumn(dv, 16);
            grid.HideColumn(dv, 17);
            grid.HideColumn(dv, 18);
        }



        #endregion


        public void Tag_Search(TreeView TV, string SearchTag)
        {
            //string Search = "select FilePath from [FileIndex] WHERE [Tags] LIKE '" + SearchTag + "'";

            //List<string> searchResults = lst.SQLite_To_List(ICOLib_Db, Search);

            ////loadToolStripMenuItem.Text = nodeCount + " Results Found";

            //SearchResultsDisplayed = true;

            ////ImageList searchLisg = img.ImageList_From_FileList(searchResults, true);

            ////img.ImageList_ListView_Display(listView1, searchLisg);

            //tv.Load_File_List(TV, searchResults, true, "Search Results");
        }

        /* join two sqlite tables together, merging based on the Function name */
        // SELECT * from FunctionLib LEFT JOIN FunctionParams ON FunctionLib.Function = FunctionParams.Function

    }

    public class Macros
    {
        private static _AHK ahk = new _AHK();
        private static _Database.SQLite sqlite = new _Database.SQLite();
        private static _Database.SQL sql = new _Database.SQL();

        #region === Macro Db Functions ===

        /// <summary> </summary>
        /// <param name="DbFile"> </param>
        /// <param name="ID"> </param>
        /// <param name="Flagged"> </param>
        /// <param name="MacroName"> </param>
        /// <param name="MouseAction"> </param>
        /// <param name="MouseX"> </param>
        /// <param name="MouseY"> </param>
        /// <param name="ActiveWindow"> </param>
        /// <param name="SearchImage"> </param>
        /// <param name="CurrentTime"> </param>
        /// <param name="PlayBack"> </param>
        /// <param name="ActionNum"> </param>
        /// <param name="Description"> </param>
        /// <param name="Code"> </param>
        public bool Insert_MouseMacro(string DbFile, string ID = "", string Flagged = "", string MacroName = "", string MouseAction = "", string MouseX = "", string MouseY = "", string ActiveWindow = "", string SearchImage = "", string CurrentTime = "", string PlayBack = "", string ActionNum = "", string Description = "", string Code = "")
        {
            string InsertLine = "Insert Into [MouseMacros] (Flagged, MacroName, MouseAction, MouseX, MouseY, ActiveWindow, SearchImage, CurrentTime, PlayBack, ActionNum, Description, Code) values ('" + Flagged + "', '" + MacroName + "', '" + MouseAction + "', '" + MouseX + "', '" + MouseY + "', '" + ActiveWindow + "', '" + SearchImage + "', '" + CurrentTime + "', '" + PlayBack + "', '" + ActionNum + "', '" + Description + "', '" + Code + "')";
            bool Inserted = sqlite.Execute(DbFile, InsertLine);
            if (!Inserted) { ahk.MsgBox("Inserted Into [MouseMacros] = " + Inserted.ToString()); }
            return Inserted;
        }

        /// <summary> </summary>
        /// <param name="DbFile"> </param>
        /// <param name=" MouseMacro inObject"> </param>
        public bool Insert_Object_MouseMacro(string DbFile, MouseMacro inObject)
        {
            string InsertLine = "Insert Into [MouseMacros] (Flagged, MacroName, MouseAction, MouseX, MouseY, ActiveWindow, SearchImage, CurrentTime, PlayBack, ActionNum, Description, Code) values ('" + inObject.Flagged + "', '" + inObject.MacroName + "', '" + inObject.MouseAction + "', '" + inObject.MouseX + "', '" + inObject.MouseY + "', '" + inObject.ActiveWindow + "', '" + inObject.SearchImage + "', '" + inObject.CurrentTime + "', '" + inObject.PlayBack + "', '" + inObject.ActionNum + "', '" + inObject.Description + "', '" + inObject.Code + "')";
            bool Inserted = sqlite.Execute(DbFile, InsertLine);
            if (!Inserted) { ahk.MsgBox("Inserted Into [MouseMacros] = " + Inserted.ToString()); }
            return Inserted;
        }

        /// <summary> set checkbox flag to ignore this action in datagrid display</summary>
        /// <param name="DbFile"> </param>
        /// <param name=" ID"> </param>
        public bool Flag_MacroAction(string DbFile, string ID)
        {
            string UpdateLine = "Update [MouseMacros] set Flagged = '1' WHERE ID = '" + ID + "'";
            bool Updated = sqlite.Execute(DbFile, UpdateLine);
            if (!Updated) { ahk.MsgBox("Updated [MouseMacros] = " + Updated.ToString()); }
            return Updated;
        }

        /// <summary> after executing the macro command, return feedback / show it ran</summary>
        /// <param name="DbFile"> </param>
        /// <param name=" ID"> </param>
        /// <param name=" PlayBack"> </param>
        public bool Write_PlayBack_Note(string DbFile, string ID, string PlayBack)  // after executing the macro command, return feedback / show it ran
        {
            string UpdateLine = "Update [MouseMacros] set PlayBack = '" + PlayBack + "' WHERE ID = '" + ID + "'";
            bool Updated = sqlite.Execute(DbFile, UpdateLine);
            if (!Updated) { ahk.MsgBox("Updated [MouseMacros] = " + Updated.ToString()); }
            return Updated;
        }

        /// <summary> </summary>
        /// <param name="DbFile"> </param>
        /// <param name="Debug"> </param>
        /// <param name="ID"> </param>
        /// <param name="Flagged"> </param>
        /// <param name="MacroName"> </param>
        /// <param name="MouseAction"> </param>
        /// <param name="MouseX"> </param>
        /// <param name="MouseY"> </param>
        /// <param name="ActiveWindow"> </param>
        /// <param name="SearchImage"> </param>
        /// <param name="CurrentTime"> </param>
        /// <param name="PlayBack"> </param>
        /// <param name="ActionNum"> </param>
        /// <param name="Description"> </param>
        /// <param name="Code"> </param>
        public bool Update_MouseMacro(string DbFile, bool Debug = true, string ID = "", string Flagged = "", string MacroName = "", string MouseAction = "", string MouseX = "", string MouseY = "", string ActiveWindow = "", string SearchImage = "", string CurrentTime = "", string PlayBack = "", string ActionNum = "", string Description = "", string Code = "")
        {
            string UpdateLine = "Update [MouseMacros] set Flagged = '" + Flagged + "', MacroName = '" + MacroName + "', MouseAction = '" + MouseAction + "', MouseX = '" + MouseX + "', MouseY = '" + MouseY + "', ActiveWindow = '" + ActiveWindow + "', SearchImage = '" + SearchImage + "', CurrentTime = '" + CurrentTime + "', PlayBack = '" + PlayBack + "', ActionNum = '" + ActionNum + "', Description = '" + Description + "', Code = '" + Code + "' WHERE ID = '" + ID + "'";
            bool Updated = sqlite.Execute(DbFile, UpdateLine);
            if (!Updated) { ahk.MsgBox("Updated [MouseMacros] = " + Updated.ToString()); }
            return Updated;
        }

        /// <summary> </summary>
        /// <param name="DbFile"> </param>
        /// <param name=" MouseMacro inObject"> </param>
        /// <param name="Debug"> </param>
        public bool Update_MouseMacro_FromObject(string DbFile, MouseMacro inObject, bool Debug = true)
        {
            string UpdateLine = "Update [MouseMacros] set Flagged = '" + inObject.Flagged + "', MacroName = '" + inObject.MacroName + "', MouseAction = '" + inObject.MouseAction + "', MouseX = '" + inObject.MouseX + "', MouseY = '" + inObject.MouseY + "', ActiveWindow = '" + inObject.ActiveWindow + "', SearchImage = '" + inObject.SearchImage + "', CurrentTime = '" + inObject.CurrentTime + "', PlayBack = '" + inObject.PlayBack + "', ActionNum = '" + inObject.ActionNum + "', Description = '" + inObject.Description + "', Code = '" + inObject.Code + "' WHERE ID = '" + inObject.ID + "'";
            bool Updated = sqlite.Execute(DbFile, UpdateLine);
            if (!Updated) { ahk.MsgBox("Updated [MouseMacros] = " + Updated.ToString()); }
            return Updated;
        }

        /// <summary>Support.Desk - delete mouse macro by macro name from sqlite db file</summary>
        /// <param name="MacroName"> </param>
        /// <param name=" DbFile"> </param>
        public bool DeleteMouseMacro(string MacroName, string DbFile)
        {
            bool ReturnValue = sqlite.Execute(DbFile, "Delete FROM MouseMacros where MacroName = '" + MacroName + "'");  // delete from table
            return ReturnValue;
        }

        /// <summary>Support.Desk - delete mouse macro by macro name from sqlite db file</summary>
        /// <param name="DbFile"> </param>
        public bool Delete_All_Mouse_Macros(string DbFile)
        {
            bool ReturnValue = sqlite.Execute(DbFile, "Delete FROM [MouseMacros]");
            return ReturnValue;
        }

        /// <summary>Support.Desk - returns mouse macro by macro name from sqlite db file</summary>
        /// <param name="MacroName"> </param>
        /// <param name=" DbFile"> </param>
        public SQLiteDataReader ReturnMouseMacro(string MacroName, string DbFile)
        {
            _Database.SQLite lite = new _Database.SQLite();
            SQLiteConnection m_dbConnection = sqlite.Connect(DbFile); // connect to SQLite DB file path - returns connection data
            SQLiteDataReader reader = lite.ReturnSQLite("Select * from [MouseMacros] WHERE MacroName = '" + MacroName + "'", m_dbConnection);  // request data from DB
            sqlite.Disconnect(m_dbConnection);  // free up db for other use
            return reader;
        }

        bool macroPlaying = false;

        /// <summary>Support.Desk - play macro by name back from macro sqlite db table</summary>
        /// <param name="MacroName"> </param>
        /// <param name=" DbFile"> </param>
        /// <param name=" Play"> </param>
        public void PlayMouseMacro(string MacroName, string DbFile, bool Play)
        {
            _Database.SQLite lite = new _Database.SQLite();
            SQLiteConnection m_dbConnection = sqlite.Connect(DbFile); // connect to SQLite DB file path - returns connection data

            string SearchLine = "Select * from [MouseMacros] WHERE MacroName = 'Mouse Recording'";
            SQLiteDataReader reader = lite.ReturnSQLite(SearchLine, m_dbConnection);  // request data from DB

            MouseMacro ms = new MouseMacro();

            if (Play == true) { macroPlaying = true; }
            if (Play == false) { macroPlaying = false; }

            while (reader.Read())    // loop through each row returned from select 
            {
                string Ctime = reader["CurrentTime"].ToString();
                ms.CurrentTime = DateTime.Parse(Ctime);
                ms.ActiveWindow = reader["ActiveWindow"].ToString();
                ms.MouseAction = reader["MouseAction"].ToString();
                ms.MouseX = reader["MouseX"].ToString();
                ms.MouseY = reader["MouseY"].ToString();
                ms.SearchImage = reader["SearchImage"].ToString();
                ms.PlayBack = reader["PlayBack"].ToString();
                ms.ActionNum = reader["ActionNum"].ToString();
                ms.Flagged = reader["Flagged"].ToString();
                ms.Description = reader["Description"].ToString();
                ms.Code = reader["Code"].ToString();
                ms.ID = reader["ID"].ToString();

                if (macroPlaying == false) { return; }

                if (ms.MouseAction == "LeftButtonDown")
                {
                    int x = Int32.Parse(ms.MouseX);
                    int y = Int32.Parse(ms.MouseY);
                    ahk.MouseClick(_AHK.MouseButton.Left, x, y);

                    ahk.Sleep(1000);
                }
            }

            sqlite.Disconnect(m_dbConnection);  // free up db for other use
        }

        /// <summary>Support.Desk - insert active win title into sqlite macro db file</summary>
        /// <param name="DbFile"> </param>
        public bool InsertActiveWinTitle(string DbFile)
        {
            string ActiveWinTitle = ahk.WinGetActiveTitle();
            string InsertString = "insert into ActiveTitleLog (ActiveTitle, CurrentTime) values ('" + ActiveWinTitle + "','" + DateTime.Now.ToString() + "')";
            bool ReturnValue = sqlite.Execute(DbFile, InsertString);  // insert into a Table
            if (ReturnValue == false) { ahk.MsgBox("Failed to Insert: " + InsertString); }
            return ReturnValue;
        }

        /// <summary>Support.Desk - create user database tables</summary>
        /// <param name="DbFile"> </param>
        /// <param name="TableName"> </param>
        /// <param name="Options"> </param>
        public void CreateMacroDb(string DbFile, string TableName = "MouseMacros", string Options = "")
        {
            if (Options.Contains("NewDBFile=True"))
            {
                sqlite.Disconnect();
                ahk.FileDelete(DbFile);
            }

            // create sqlite database file if it doen't exist yet
            if (!File.Exists(DbFile)) { SQLiteConnection.CreateFile(DbFile); }

            // Create New Table If It Does NOT Exist Yet
            bool TableExist = sqlite.Table_Exists(DbFile, TableName);  //See if selected Table Exists in SQLite DB file

            if (!TableExist)  // Table DOES NOT exist in SQLite DB
            {
                string NewTableLine = "";

                if (TableName == "HotStrings") { NewTableLine = "FID INTEGER PRIMARY KEY, Enabled BOOL, HotString VARCHAR, HotStringValue VARCHAR"; }

                if (TableName == "Notes") { NewTableLine = "ID INTEGER PRIMARY KEY, Public BOOL, ClientNum VARCHAR, NoteName VARCHAR, Note VARCHAR"; }

                if (TableName == "MouseMacros") { NewTableLine = "ID INTEGER PRIMARY KEY, Flagged BOOL, MacroName VARCHAR, MouseAction VARCHAR, MouseX VARCHAR, MouseY VARCHAR, ActiveWindow VARCHAR, SearchImage VARCHAR, CurrentTime VARCHAR, PlayBack VARCHAR, ActionNum VARCHAR, Description VARCHAR, Code VARCHAR"; }

                if (TableName == "ActiveTitleLog") { NewTableLine = "ID INTEGER PRIMARY KEY, ActiveTitle VARCHAR, CurrentTime VARCHAR"; }

                //ahk.MsgBox(NewTableLine); 

                bool ReturnValue = sqlite.Execute(DbFile, "CREATE TABLE " + TableName + " (" + NewTableLine + ")");  // Create a Table [ONLY EXECUTE ONCE! WILL ERROR 2ND TIME]

            }

        }



        #endregion

    }

    public class goDaddy
    {
        private static _AHK ahk = new _AHK();

        //_AHK ahk = new _AHK();
        //_Database.SQL sql = new _Database.SQL();
        //_Database.SQLite sqlite = new _Database.SQLite();
        //_Code code = new _Code();

        SqlConnection GoDad;


        #region === GoDaddy : FileIndex ===

        /// <summary> creates sql table if it doesn't exist</summary>
        /// <param name="TableName"> </param>
        public void LucidMedia_CreateFileIndex(string TableName = "FileIndex")
        {
            _AHK ahk = new _AHK();
            _Database.SQL sql = new _Database.SQL();

            bool TableExists = sql.TableExist(GoDad, TableName);

            if (!TableExists)
            {
                bool Created = Create_LucidMedia_DirToDb(null, "[LucidMedia].[dbo].[" + TableName + "]");
                //ahk.MsgBox("Created = " + Created.ToString());
            }
            else
                ahk.MsgBox("Table Exists = " + TableExists.ToString());
        }

        //== Create Dir-To-Db SQL Table

        /// <summary> create new DirToDb SQL table</summary>
        /// <param name="conn"> </param>
        /// <param name="NewTableName"> </param>
        public bool Create_LucidMedia_DirToDb(SqlConnection conn = null, string NewTableName = "[LucidMedia].[dbo].[FileIndex]")
        {
            _Database.SQL sql = new _Database.SQL();
            if (conn == null) { MessageBox.Show("SQL Connection = NULL"); return false; }

            //// create new SQL Table
            string NewTableSetup = @"CREATE TABLE " + NewTableName + @"(
	[FID] [int] IDENTITY(1,1) NOT NULL,
	[FileFlag] [bit] NULL,
	[FilePath] [varchar](MAX) NOT NULL,
	[FileName] [varchar](300) NULL,
	[FileSize] [varchar](100) NULL,
	[LastAccessTime] [datetime] NULL,
	[LastWriteTime] [datetime] NULL,
	[IsReadOnly] [bit] NULL,
	[FileExt] [varchar](50) NULL,
	[FileExists] [bit] NULL,
	[DirName] [varchar](MAX) NULL,
	[FileDirectory] [varchar](MAX) NULL,
	[CreationTime] [datetime] NULL,
	[Attributes] [varchar](MAX) NULL,
	[FileAction] [varchar](MAX) NULL,
	[Tags] [varchar](MAX) NULL,
) ON [PRIMARY]";

            bool ReturnVal = sql.WriteDataRecord(conn, NewTableSetup);
            return ReturnVal;
        }

        /// <summary> create new SQL table</summary>
        /// <param name="CONN"> </param>
        /// <param name="TableName"> </param>
        /// <param name="Dir"> </param>
        /// <param name="FilePattern"> </param>
        /// <param name="Recurse"> </param>
        public int SQL_DirToDb(SqlConnection CONN, string TableName = "[LucidMedia].[dbo].[FileIndex]", string Dir = "", string FilePattern = "*.*", bool Recurse = true)
        {
            _AHK ahk = new _AHK();
            _Database.SQL sql = new _Database.SQL();

            // check to see if directory exists already - create if not
            bool Found = sql.TableExist(CONN, TableName);
            if (!Found)
            {
                bool Created = SQL_DirToDb_CreateDb(CONN, TableName);
                ahk.MsgBox(TableName + " Created = " + Created.ToString());
                if (!Created) { return -1; }
            }

            int InsertUpdateCount = SQL_DirToDb_Populate(CONN, TableName, Dir, FilePattern, Recurse);

            if (InsertUpdateCount > 0) { ahk.MsgBox("Updated or Inserted " + InsertUpdateCount.ToString() + " Files Into " + TableName); }
            if (InsertUpdateCount == 0) { ahk.MsgBox("ZERO Files Added/Updated in " + TableName); }
            //ahk.MsgBox(NewTableName + " Already Exists = " + Found.ToString());

            return InsertUpdateCount;
        }

        /// <summary> create new SQL DirToDb table</summary>
        /// <param name="conn"> </param>
        /// <param name="NewTableName"> </param>
        public bool SQL_DirToDb_CreateDb(SqlConnection conn = null, string NewTableName = "[LucidMedia].[dbo].[FileIndex]")
        {
            _Database.SQL sql = new _Database.SQL();
            if (conn == null) { conn = GoDad; }

            //// create new SQL Table
            string NewTableSetup = @"CREATE TABLE " + NewTableName + @"(
	[FID] [int] IDENTITY(1,1) NOT NULL,
	[FileFlag] [bit] NULL,
	[FilePath] [varchar](MAX) NOT NULL,
	[FileName] [varchar](300) NULL,
	[FileSize] [varchar](100) NULL,
	[LastAccessTime] [datetime] NULL,
	[LastWriteTime] [datetime] NULL,
	[IsReadOnly] [bit] NULL,
	[FileExt] [varchar](50) NULL,
	[FileExists] [bit] NULL,
	[DirName] [varchar](MAX) NULL,
	[FileDirectory] [varchar](MAX) NULL,
	[CreationTime] [datetime] NULL,
	[Attributes] [varchar](MAX) NULL,
	[FileAction] [varchar](MAX) NULL,
	[Tags] [varchar](MAX) NULL,
) ON [PRIMARY]";

            bool ReturnVal = sql.WriteDataRecord(conn, NewTableSetup);
            return ReturnVal;
        }

        /// <summary> </summary>
        /// <param name="CONN"> </param>
        /// <param name=" TableName"> </param>
        /// <param name=" Dir"> </param>
        /// <param name="FilePattern"> </param>
        /// <param name="Recurse"> </param>
        public int SQL_DirToDb_Populate(SqlConnection CONN, string TableName, string Dir, string FilePattern = "*.*", bool Recurse = true)
        {
            _AHK ahk = new _AHK();

            //=============================================================================================
            // Get list of files in the specific directory - Recursive File Search - Fill Data Table
            //=============================================================================================

            //var stopwatch = new Stopwatch();
            //stopwatch.Start();

            // option to toggle between recurse directory search and only top folder
            SearchOption opt = SearchOption.AllDirectories;
            if (!Recurse) { opt = SearchOption.TopDirectoryOnly; }

            string[] files = Directory.GetFiles(Dir, FilePattern, opt);

            int i = 0;

            foreach (string file in files)  // loop through list of files and write file details to SQLite db
            {
                _FileIndex fle = new _FileIndex();

                System.IO.FileInfo fileinfo = new System.IO.FileInfo(file); //retrieve info about each file

                fle.FileName = fileinfo.Name.ToString();

                //string FileSize = fileinfo.Length.ToString();
                fle.FileSize = ahk.FormatBytes(fileinfo.Length);  // convert bytes to Text representation (adds kb/mb/tb to return)

                fle.LastWriteTime = fileinfo.LastWriteTime.ToString();
                fle.LastAccessTime = fileinfo.LastAccessTime.ToString();
                fle.IsReadOnly = fileinfo.IsReadOnly.ToString();

                fle.FileExt = fileinfo.Extension.ToString();
                fle.FileExists = fileinfo.Exists.ToString();
                fle.DirName = fileinfo.DirectoryName.ToString();
                fle.FileDirectory = fileinfo.Directory.ToString();
                fle.CreationTime = fileinfo.CreationTime.ToString();
                fle.Attributes = fileinfo.Attributes.ToString();
                fle.FileAction = ""; // variable used to indicate a file is queued to be copied to another location 
                fle.FilePath = file;
                fle.FileFlag = "false";

                fle.DirName = ahk.FixSpecialChars(fle.DirName); //remove invalid characters before writing
                fle.FileDirectory = ahk.FixSpecialChars(fle.FileDirectory); //remove invalid characters before writing
                fle.FileName = ahk.FixSpecialChars(fle.FileName); //remove invalid characters before writing
                fle.FilePath = ahk.FixSpecialChars(fle.FilePath); //remove invalid characters before writing

                string[] paths = fle.DirName.Split('\\'); // split the directory path to get the name
                fle.DirName = paths[paths.Length - 1]; //returns last folder name in string

                bool Updated = Update_SQL_FileIndex(CONN, TableName, fle, "Where [FilePath] = '" + fle.FilePath + "'");
                if (Updated) { i++; }
                if (!Updated)
                {
                    bool Inserted = Insert_SQL_FileIndex(CONN, TableName, fle);
                    if (!Inserted) { ahk.MsgBox("Error Inserting " + fle.FilePath); }
                    if (Inserted) { i++; }
                }

            }  // end file loop


            return i;
        }

        public struct _FileIndex
        {
            public string FID { get; set; }
            public string FileFlag { get; set; }
            public string FilePath { get; set; }
            public string FileName { get; set; }
            public string FileSize { get; set; }
            public string LastAccessTime { get; set; }
            public string LastWriteTime { get; set; }
            public string IsReadOnly { get; set; }
            public string FileExt { get; set; }
            public string FileExists { get; set; }
            public string DirName { get; set; }
            public string FileDirectory { get; set; }
            public string CreationTime { get; set; }
            public string Attributes { get; set; }
            public string FileAction { get; set; }
            public string Tags { get; set; }
        }

        /// <summary> </summary>
        /// <param name="DataGridView dv"> </param>
        public void HideShow_FileIndex_Columns(DataGridView dv)
        {

            try { dv.Columns["FID"].Visible = true; }
            catch { }
            try { dv.Columns["FileFlag"].Visible = true; }
            catch { }
            try { dv.Columns["FilePath"].Visible = true; }
            catch { }
            try { dv.Columns["FileName"].Visible = true; }
            catch { }
            try { dv.Columns["FileSize"].Visible = true; }
            catch { }
            try { dv.Columns["LastAccessTime"].Visible = true; }
            catch { }
            try { dv.Columns["LastWriteTime"].Visible = true; }
            catch { }
            try { dv.Columns["IsReadOnly"].Visible = true; }
            catch { }
            try { dv.Columns["FileExt"].Visible = true; }
            catch { }
            try { dv.Columns["FileExists"].Visible = true; }
            catch { }
            try { dv.Columns["DirName"].Visible = true; }
            catch { }
            try { dv.Columns["FileDirectory"].Visible = true; }
            catch { }
            try { dv.Columns["CreationTime"].Visible = true; }
            catch { }
            try { dv.Columns["Attributes"].Visible = true; }
            catch { }
            try { dv.Columns["FileAction"].Visible = true; }
            catch { }
            try { dv.Columns["Tags"].Visible = true; }
            catch { }
        }

        /// <summary> </summary>
        /// <param name="Conn"> </param>
        /// <param name=" TableName"> </param>
        /// <param name=" _FileIndex obj"> </param>
        public bool Insert_SQL_FileIndex(SqlConnection Conn, string TableName, _FileIndex obj)
        {
            string SQLLine = "Insert Into " + TableName + " (FileFlag, FilePath, FileName, FileSize, LastAccessTime, LastWriteTime, IsReadOnly, FileExt, FileExists, DirName, FileDirectory, CreationTime, Attributes, FileAction, Tags) VALUES (@FileFlag, @FilePath, @FileName, @FileSize, @LastAccessTime, @LastWriteTime, @IsReadOnly, @FileExt, @FileExists, @DirName, @FileDirectory, @CreationTime, @Attributes, @FileAction, @Tags)";

            SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);

            cmd2 = new SqlCommand(SQLLine, Conn);

            if (obj.FileFlag == null) { obj.FileFlag = ""; }
            if (obj.FilePath == null) { obj.FilePath = ""; }
            if (obj.FileName == null) { obj.FileName = ""; }
            if (obj.FileSize == null) { obj.FileSize = ""; }
            if (obj.LastAccessTime == null) { obj.LastAccessTime = ""; }
            if (obj.LastWriteTime == null) { obj.LastWriteTime = ""; }
            if (obj.IsReadOnly == null) { obj.IsReadOnly = ""; }
            if (obj.FileExt == null) { obj.FileExt = ""; }
            if (obj.FileExists == null) { obj.FileExists = ""; }
            if (obj.DirName == null) { obj.DirName = ""; }
            if (obj.FileDirectory == null) { obj.FileDirectory = ""; }
            if (obj.CreationTime == null) { obj.CreationTime = ""; }
            if (obj.Attributes == null) { obj.Attributes = ""; }
            if (obj.FileAction == null) { obj.FileAction = ""; }
            if (obj.Tags == null) { obj.Tags = ""; }


            cmd2.Parameters.AddWithValue(@"FileFlag", obj.FileFlag.ToString());
            cmd2.Parameters.AddWithValue(@"FilePath", obj.FilePath.ToString());
            cmd2.Parameters.AddWithValue(@"FileName", obj.FileName.ToString());
            cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize.ToString());
            cmd2.Parameters.AddWithValue(@"LastAccessTime", obj.LastAccessTime.ToString());
            cmd2.Parameters.AddWithValue(@"LastWriteTime", obj.LastWriteTime.ToString());
            cmd2.Parameters.AddWithValue(@"IsReadOnly", obj.IsReadOnly.ToString());
            cmd2.Parameters.AddWithValue(@"FileExt", obj.FileExt.ToString());
            cmd2.Parameters.AddWithValue(@"FileExists", obj.FileExists.ToString());
            cmd2.Parameters.AddWithValue(@"DirName", obj.DirName.ToString());
            cmd2.Parameters.AddWithValue(@"FileDirectory", obj.FileDirectory.ToString());
            cmd2.Parameters.AddWithValue(@"CreationTime", obj.CreationTime.ToString());
            cmd2.Parameters.AddWithValue(@"Attributes", obj.Attributes.ToString());
            cmd2.Parameters.AddWithValue(@"FileAction", obj.FileAction.ToString());
            cmd2.Parameters.AddWithValue(@"Tags", obj.Tags.ToString());

            if (Conn.State == ConnectionState.Closed) { Conn.Open(); }

            int recordsAffected = 0;

            try { recordsAffected = cmd2.ExecuteNonQuery(); }

            catch (SqlException ex) { MessageBox.Show(ex.ToString()); }

            Conn.Close();

            if (recordsAffected > 0) { return true; }

            else return false;
        }

        /// <summary> </summary>
        /// <param name="Conn"> </param>
        /// <param name=" TableName"> </param>
        /// <param name=" _FileIndex obj"> </param>
        /// <param name=" WhereClause"> </param>
        public bool Update_SQL_FileIndex(SqlConnection Conn, string TableName, _FileIndex obj, string WhereClause)
        {
            if (WhereClause.ToUpper().Contains("WHERE")) { WhereClause = WhereClause.ToUpper(); WhereClause = WhereClause.Replace("WHERE", ""); }  // remove the word where if passed by user

            //[LucidMedia].[dbo].[FileIndex2]
            string SQLLine = "Update " + TableName + " SET FileFlag = @FileFlag, FilePath = @FilePath, FileName = @FileName, FileSize = @FileSize, LastAccessTime = @LastAccessTime, LastWriteTime = @LastWriteTime, IsReadOnly = @IsReadOnly, FileExt = @FileExt, FileExists = @FileExists, DirName = @DirName, FileDirectory = @FileDirectory, CreationTime = @CreationTime, Attributes = @Attributes, FileAction = @FileAction, Tags = @Tags WHERE " + WhereClause + "";

            SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);

            cmd2 = new SqlCommand(SQLLine, Conn);

            if (obj.FileFlag == null) { obj.FileFlag = ""; }
            if (obj.FilePath == null) { obj.FilePath = ""; }
            if (obj.FileName == null) { obj.FileName = ""; }
            if (obj.FileSize == null) { obj.FileSize = ""; }
            if (obj.LastAccessTime == null) { obj.LastAccessTime = ""; }
            if (obj.LastWriteTime == null) { obj.LastWriteTime = ""; }
            if (obj.IsReadOnly == null) { obj.IsReadOnly = ""; }
            if (obj.FileExt == null) { obj.FileExt = ""; }
            if (obj.FileExists == null) { obj.FileExists = ""; }
            if (obj.DirName == null) { obj.DirName = ""; }
            if (obj.FileDirectory == null) { obj.FileDirectory = ""; }
            if (obj.CreationTime == null) { obj.CreationTime = ""; }
            if (obj.Attributes == null) { obj.Attributes = ""; }
            if (obj.FileAction == null) { obj.FileAction = ""; }
            if (obj.Tags == null) { obj.Tags = ""; }

            cmd2.Parameters.AddWithValue(@"FileFlag", obj.FileFlag.ToString());
            cmd2.Parameters.AddWithValue(@"FilePath", obj.FilePath.ToString());
            cmd2.Parameters.AddWithValue(@"FileName", obj.FileName.ToString());
            cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize.ToString());
            cmd2.Parameters.AddWithValue(@"LastAccessTime", obj.LastAccessTime.ToString());
            cmd2.Parameters.AddWithValue(@"LastWriteTime", obj.LastWriteTime.ToString());
            cmd2.Parameters.AddWithValue(@"IsReadOnly", obj.IsReadOnly.ToString());
            cmd2.Parameters.AddWithValue(@"FileExt", obj.FileExt.ToString());
            cmd2.Parameters.AddWithValue(@"FileExists", obj.FileExists.ToString());
            cmd2.Parameters.AddWithValue(@"DirName", obj.DirName.ToString());
            cmd2.Parameters.AddWithValue(@"FileDirectory", obj.FileDirectory.ToString());
            cmd2.Parameters.AddWithValue(@"CreationTime", obj.CreationTime.ToString());
            cmd2.Parameters.AddWithValue(@"Attributes", obj.Attributes.ToString());
            cmd2.Parameters.AddWithValue(@"FileAction", obj.FileAction.ToString());
            cmd2.Parameters.AddWithValue(@"Tags", obj.Tags.ToString());

            if (Conn.State == ConnectionState.Closed) { Conn.Open(); }

            int recordsAffected = 0;

            try { recordsAffected = cmd2.ExecuteNonQuery(); }

            catch (SqlException ex) { MessageBox.Show(ex.ToString()); }

            Conn.Close();

            if (recordsAffected > 0) { return true; }

            else return false;
        }


        #endregion

        #region === GoDaddy : FunctionLib ===


        // creates FunctionLib Table if it doesn't exist already

        /// <summary> create new FunctionLib SQL table</summary>
        /// <param name="Conn"> </param>
        /// <param name="NewTableName"> </param>
        public bool Create_FunctionLib_SQL_Tables(SqlConnection Conn = null, string NewTableName = "[sharpAHK].[lucidmethod].[FunctionLib]")  // create new SQL table
        {
            _Database.SQL sql = new _Database.SQL();

            if (Conn == null) { Conn = GoDad; }

            bool TableAlreadyExists = sql.TableExist(Conn, NewTableName);

            if (TableAlreadyExists) { return true; }

            //// Create new FunctionLib SQL Table
            string NewTableSetup = @"CREATE TABLE " + NewTableName + @"(
                    [FID] [int] IDENTITY(1,1) NOT NULL,
	                [Flagged] [bit] NULL,
	                [FileName] [varchar](MAX) NULL,
	                [NameSpace] [varchar](MAX) NULL,
                    [Class] [varchar](MAX) NULL,
                    [Function] [varchar](MAX) NULL,
                    [Region] [varchar](MAX) NULL,
                    [LineNum] [int] NULL,
                    [Tags] [varchar](MAX) NULL,
                    [Comments] [varchar](MAX) NULL,
                    [FunctionLine] [varchar](MAX) NULL,
                    [FunctionText] [varchar](MAX) NULL,
                    [DLLVer] [varchar](MAX) NULL,
                    [Examples] [varchar](MAX) NULL,
                    [Documentation] [varchar](MAX) NULL,
                    [UsedInProjects] [varchar](MAX) NULL,
                    [DateAdded] [varchar](MAX) NULL,
                    [DateModified] [varchar](MAX) NULL,
                    [UsesErrorLevel] [varchar](MAX) NULL,
                    [Tested] [varchar](MAX) NULL,
                    [ToDo] [varchar](MAX) NULL,
                ) ON [PRIMARY]";
            bool ReturnVal = sql.WriteDataRecord(Conn, NewTableSetup);
            return ReturnVal;
        }

        /// <summary> </summary>
        public bool GoDaddy_Create_FunctionLib()
        {
            bool created = Create_FunctionLib_SQL_Tables(GoDad, "[sharpAHK].[lucidmethod].[FunctionLib]");
            return created;
        }

        /// <summary> </summary>
        /// <param name="SQLcommand"> </param>
        public FunctionLib Return_FunctionLib_Object(string SQLcommand = "Select * From [sharpAHK].[lucidmethod].[FunctionLib]")
        {
            //=======================================
            // Search SQL Table - Parse Results
            //=======================================
            //string SQLcommand = "Select * From [sharpAHK].[dbo].[FunctionLib]";
            SqlDataAdapter SQLconnect = new SqlDataAdapter(SQLcommand, GoDad);
            DataTable SQLresults = new DataTable();

            FunctionLib returnObj = new FunctionLib();

            try
            {
                SQLconnect.Fill(SQLresults);
                if (SQLresults.Rows.Count < 1)
                {
                    // no results returned
                }
                else
                {
                    int iIndex = 0;
                    foreach (DataRow row in SQLresults.Rows)
                    {
                        returnObj.FID = SQLresults.Rows[iIndex]["FID"].ToString();
                        returnObj.FunctionName = SQLresults.Rows[iIndex]["FunctionName"].ToString();
                        returnObj.FunctionLine = SQLresults.Rows[iIndex]["FunctionLine"].ToString();
                        returnObj.FunctionCode = SQLresults.Rows[iIndex]["FunctionCode"].ToString();
                        returnObj.RegionName = SQLresults.Rows[iIndex]["RegionName"].ToString();
                        returnObj.FileName = SQLresults.Rows[iIndex]["FileName"].ToString();
                        returnObj.Documentation = SQLresults.Rows[iIndex]["Documentation"].ToString();
                        returnObj.Examples = SQLresults.Rows[iIndex]["Examples"].ToString();
                        returnObj.Tags = SQLresults.Rows[iIndex]["Tags"].ToString();
                        returnObj.Links = SQLresults.Rows[iIndex]["Links"].ToString();
                        returnObj.ICO_Name = SQLresults.Rows[iIndex]["ICO_Name"].ToString();
                        returnObj.ICO_Img = SQLresults.Rows[iIndex]["ICO_Img"].ToString();
                        returnObj.DateAdded = SQLresults.Rows[iIndex]["DateAdded"].ToString();
                        returnObj.DateModified = SQLresults.Rows[iIndex]["DateModified"].ToString();
                        returnObj.Flagged = SQLresults.Rows[iIndex]["Flagged"].ToString();
                        returnObj.Released = SQLresults.Rows[iIndex]["Released"].ToString();
                        iIndex++;

                        return returnObj;
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.ToString());
            }

            GoDad.Close();
            return returnObj;
        }

        //public void InsertSQLValues_FunctionLib()
        //{
        ////=======================================
        //// INSERT Values into SQL Table
        ////=======================================
        //string SQLcommand = "INSERT INTO [sharpAHK].[dbo].[FunctionLib] (FunctionName,FunctionLine,FunctionCode,RegionName,FileName,Documentation,Examples,Tags,Links,ICO_Name,ICO_Img,DateAdded,DateModified,Flagged,Released) VALUES(@FID, @FunctionName, @FunctionLine, @FunctionCode, @RegionName, @FileName, @Documentation, @Examples, @Tags, @Links, @ICO_Name, @ICO_Img, @DateAdded, @DateModified, @Flagged, @Released)";
        //SqlCommand cmd1 = new SqlCommand(SQLcommand, GoDad);

        //cmd1.Parameters.AddWithValue(@"FID", FID);
        //cmd1.Parameters.AddWithValue(@"FunctionName", FunctionName);
        //cmd1.Parameters.AddWithValue(@"FunctionLine", FunctionLine);
        //cmd1.Parameters.AddWithValue(@"FunctionCode", FunctionCode);
        //cmd1.Parameters.AddWithValue(@"RegionName", RegionName);
        //cmd1.Parameters.AddWithValue(@"FileName", FileName);
        //cmd1.Parameters.AddWithValue(@"Documentation", Documentation);
        //cmd1.Parameters.AddWithValue(@"Examples", Examples);
        //cmd1.Parameters.AddWithValue(@"Tags", Tags);
        //cmd1.Parameters.AddWithValue(@"Links", Links);
        //cmd1.Parameters.AddWithValue(@"ICO_Name", ICO_Name);
        //cmd1.Parameters.AddWithValue(@"ICO_Img", ICO_Img);
        //cmd1.Parameters.AddWithValue(@"DateAdded", DateAdded);
        //cmd1.Parameters.AddWithValue(@"DateModified", DateModified);
        //cmd1.Parameters.AddWithValue(@"Flagged", Flagged);
        //cmd1.Parameters.AddWithValue(@"Released", Released);

        //if (GoDad.State == ConnectionState.Closed) { GoDad.Open(); }

        //try
        //{
        //cmd1.ExecuteNonQuery();
        //}
        //catch (SqlException ex)
        //{
        //// MessageBox.Show(ex.ToString());
        //}
        //}

        public class FunctionLib
        {
            public string FID { get; set; }
            public string FunctionName { get; set; }
            public string FunctionLine { get; set; }
            public string FunctionCode { get; set; }
            public string RegionName { get; set; }
            public string FileName { get; set; }
            public string Documentation { get; set; }
            public string Examples { get; set; }
            public string Tags { get; set; }
            public string Links { get; set; }
            public string ICO_Name { get; set; }
            public string ICO_Img { get; set; }
            public string DateAdded { get; set; }
            public string DateModified { get; set; }
            public string Flagged { get; set; }
            public string Released { get; set; }
        }

        /// <summary> </summary>
        /// <param name="FunctionLib obj"> </param>
        /// <param name="OnlyInsert"> </param>
        public void Update_Insert_FunctionLib(FunctionLib obj, bool OnlyInsert = false)
        {
            SqlConnection conn = GoDad;

            // initialize var names as blank, if not null in object, assign that value
            string FID = ""; if (obj.FID != null) { FID = obj.FID.ToString(); }
            string FunctionName = ""; if (obj.FunctionName != null) { FunctionName = obj.FunctionName.ToString(); }
            string FunctionLine = ""; if (obj.FunctionLine != null) { FunctionLine = obj.FunctionLine.ToString(); }
            string FunctionCode = ""; if (obj.FunctionCode != null) { FunctionCode = obj.FunctionCode.ToString(); }
            string RegionName = ""; if (obj.RegionName != null) { RegionName = obj.RegionName.ToString(); }
            string FileName = ""; if (obj.FileName != null) { FileName = obj.FileName.ToString(); }
            string Documentation = ""; if (obj.Documentation != null) { Documentation = obj.Documentation.ToString(); }
            string Examples = ""; if (obj.Examples != null) { Examples = obj.Examples.ToString(); }
            string Tags = ""; if (obj.Tags != null) { Tags = obj.Tags.ToString(); }
            string Links = ""; if (obj.Links != null) { Links = obj.Links.ToString(); }
            string ICO_Name = ""; if (obj.ICO_Name != null) { ICO_Name = obj.ICO_Name.ToString(); }
            string ICO_Img = ""; if (obj.ICO_Img != null) { ICO_Img = obj.ICO_Img.ToString(); }
            string DateAdded = ""; if (obj.DateAdded != null) { DateAdded = obj.DateAdded.ToString(); }
            string DateModified = ""; if (obj.DateModified != null) { DateModified = obj.DateModified.ToString(); }
            string Flagged = ""; if (obj.Flagged != null) { Flagged = obj.Flagged.ToString(); }
            string Released = ""; if (obj.Released != null) { Released = obj.Released.ToString(); }


            // Attempt to UPDATE Values into SQL Table (by FID)

            int recordsAffected = 0;

            if (OnlyInsert)
            {
                string SQLLine = "UPDATE [sharpAHK].[dbo].[FunctionLib] SET FunctionName= '" + FunctionName + "', FunctionLine= '" + FunctionLine + "', FunctionCode= '" + FunctionCode + "', RegionName= '" + RegionName + "', FileName= '" + FileName + "', Documentation= '" + Documentation + "', Examples= '" + Examples + "', Tags= '" + Tags + "', Links= '" + Links + "', ICO_Name= '" + ICO_Name + "', ICO_Img= '" + ICO_Img + "', DateAdded= '" + DateAdded + "', DateModified= '" + DateModified + "', Flagged= '" + Flagged + "', Released= '" + Released + "' WHERE ID = '" + FID + "'";

                SqlCommand cmd2 = new SqlCommand(SQLLine, GoDad);
                if (GoDad.State == ConnectionState.Closed) { GoDad.Open(); }

                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { MessageBox.Show(ex.ToString()); }

                conn.Close();
            }


            // if the Update doesn't change any rows, need to insert new entry
            if (recordsAffected == 0)
            {
                // INSERT Values into SQL Table
                string SQLcommand = "INSERT INTO [sharpAHK].[dbo].[FunctionLib] (FunctionName,FunctionLine,FunctionCode,RegionName,FileName,Documentation,Examples,Tags,Links,ICO_Name,ICO_Img,DateAdded,DateModified,Flagged,Released) VALUES(@FunctionName, @FunctionLine, @FunctionCode, @RegionName, @FileName, @Documentation, @Examples, @Tags, @Links, @ICO_Name, @ICO_Img, @DateAdded, @DateModified, @Flagged, @Released)";
                SqlCommand cmd1 = new SqlCommand(SQLcommand, GoDad);

                cmd1.Parameters.AddWithValue(@"FID", FID);
                cmd1.Parameters.AddWithValue(@"FunctionName", FunctionName);
                cmd1.Parameters.AddWithValue(@"FunctionLine", FunctionLine);
                cmd1.Parameters.AddWithValue(@"FunctionCode", FunctionCode);
                cmd1.Parameters.AddWithValue(@"RegionName", RegionName);
                cmd1.Parameters.AddWithValue(@"FileName", FileName);
                cmd1.Parameters.AddWithValue(@"Documentation", Documentation);
                cmd1.Parameters.AddWithValue(@"Examples", Examples);
                cmd1.Parameters.AddWithValue(@"Tags", Tags);
                cmd1.Parameters.AddWithValue(@"Links", Links);
                cmd1.Parameters.AddWithValue(@"ICO_Name", ICO_Name);
                cmd1.Parameters.AddWithValue(@"ICO_Img", ICO_Img);
                cmd1.Parameters.AddWithValue(@"DateAdded", DateAdded);
                cmd1.Parameters.AddWithValue(@"DateModified", DateModified);
                cmd1.Parameters.AddWithValue(@"Flagged", Flagged);
                cmd1.Parameters.AddWithValue(@"Released", Released);

                if (conn.State == ConnectionState.Closed) { conn.Open(); }

                try { cmd1.ExecuteNonQuery(); }
                catch (SqlException ex) { MessageBox.Show(ex.ToString()); }
            }
        }

        public struct functionLib
        {
            public string ID { get; set; }
            public string Flagged { get; set; }
            public string FileName { get; set; }
            public string NameSpace { get; set; }
            public string Class { get; set; }
            public string Function { get; set; }
            public string Region { get; set; }
            public string LineNum { get; set; }
            public string Tags { get; set; }
            public string Comments { get; set; }
            public string TimeStamp { get; set; }
            public string FunctionLine { get; set; }
            public string FunctionText { get; set; }
            public string FilePath { get; set; }
            public string DLLVer { get; set; }
            public string Example { get; set; }
            public string Documentation { get; set; }
            public string UsedInProjects { get; set; }
            public string DateAdded { get; set; }
            public string UsesErrorLevel { get; set; }
            public string Tested { get; set; }
            public string ToDo { get; set; }
        }

        /// <summary> </summary>
        /// <param name="functionLib obj"> </param>
        /// <param name="OnlyInsert"> </param>
        public void Update_Insert_SQLFunctionLib(SqlConnection Conn, functionLib obj, bool OnlyInsert = false)
        {
            _AHK ahk = new _AHK();

            if (obj.ID == null) { obj.ID = ""; }
            if (obj.Flagged == null) { obj.Flagged = ""; }
            if (obj.FileName == null) { obj.FileName = ""; }
            if (obj.NameSpace == null) { obj.NameSpace = ""; }
            if (obj.Class == null) { obj.Class = ""; }
            if (obj.Function == null) { obj.Function = ""; }
            if (obj.Region == null) { obj.Region = ""; }
            if (obj.LineNum == null) { obj.LineNum = ""; }
            if (obj.Tags == null) { obj.Tags = ""; }
            if (obj.Comments == null) { obj.Comments = ""; }
            if (obj.TimeStamp == null) { obj.TimeStamp = ""; }
            if (obj.FunctionLine == null) { obj.FunctionLine = ""; }
            if (obj.FunctionText == null) { obj.FunctionText = ""; }
            if (obj.FilePath == null) { obj.FilePath = ""; }
            if (obj.DLLVer == null) { obj.DLLVer = ""; }
            if (obj.Example == null) { obj.Example = ""; }
            if (obj.Documentation == null) { obj.Documentation = ""; }
            if (obj.UsedInProjects == null) { obj.UsedInProjects = ""; }
            if (obj.DateAdded == null) { obj.DateAdded = ""; }
            if (obj.UsesErrorLevel == null) { obj.UsesErrorLevel = ""; }
            if (obj.Tested == null) { obj.Tested = ""; }
            if (obj.ToDo == null) { obj.ToDo = ""; }

            //// initialize var names as blank, if not null in object, assign that value
            //string FID = ""; if (obj.ID != null) { FID = obj.ID.ToString(); }
            //string FunctionName = ""; if (obj.Function != null) { FunctionName = obj.Function.ToString(); }
            //string FunctionLine = ""; if (obj.FunctionLine != null) { FunctionLine = obj.FunctionLine.ToString(); }
            //string FunctionCode = ""; if (obj.FunctionText != null) { FunctionCode = obj.FunctionText.ToString(); }
            //string Region = ""; if (obj.Region != null) { Region = obj.Region.ToString(); }
            //string FileName = ""; if (obj.FileName != null) { FileName = obj.FileName.ToString(); }
            //string Documentation = ""; if (obj.Documentation != null) { Documentation = obj.Documentation.ToString(); }
            //string Examples = ""; if (obj.Example != null) { Examples = obj.Example.ToString(); }
            //string Tags = ""; if (obj.Tags != null) { Tags = obj.Tags.ToString(); }
            ////string Links = ""; if (obj.Links != null) { Links = obj.Links.ToString(); }
            ////string ICO_Name = ""; if (obj.ICO_Name != null) { ICO_Name = obj.ICO_Name.ToString(); }
            ////string ICO_Img = ""; if (obj.ICO_Img != null) { ICO_Img = obj.ICO_Img.ToString(); }
            //string DateAdded = ""; if (obj.DateAdded != null) { DateAdded = obj.DateAdded.ToString(); }
            //string DateModified = ""; if (obj.TimeStamp != null) { DateModified = obj.TimeStamp.ToString(); }
            ////string DateModified = ""; if (obj.DateModified != null) { DateModified = obj.DateModified.ToString(); }
            //string Flagged = ""; if (obj.Flagged != null) { Flagged = obj.Flagged.ToString(); }
            ////string Released = ""; if (obj.Released != null) { Released = obj.Released.ToString(); }

            //bool Updated = Update_SQLFunctionLib(Conn, obj, "WHERE ID = " + obj.ID);

            bool Updated = Insert_SQLFunctionLib(Conn, obj);

            if (!Updated)
            {
                //Updated = Insert_SQLFunctionLib(Conn, obj);
                if (!Updated) { ahk.MsgBox("Error Inserting Object into GoDaddy"); }
            }

        }

        /// <summary> </summary>
        /// <param name="Conn"> </param>
        /// <param name=" functionLib obj"> </param>
        public bool Insert_SQLFunctionLib(SqlConnection Conn, functionLib obj)
        {
            string SQLLine = "Insert Into [sharpAHK].[lucidmethod].[FunctionLib] (Flagged, FileName, NameSpace, Class, [Function], [Region], LineNum, Tags, [Comments], FunctionLine, FunctionText, DLLVer, Examples, Documentation, UsedInProjects, DateAdded, [DateModified], UsesErrorLevel, Tested, ToDo) VALUES (@Flagged, @FileName, @NameSpace, @Class, @Function, @Region, @LineNum, @Tags, @Comments, @FunctionLine, @FunctionText, @DLLVer, @Examples, @Documentation, @UsedInProjects, @DateAdded, @DateModified, @UsesErrorLevel, @Tested, @ToDo)";

            //string debugSQLLine = "Insert Into [sharpAHK].[lucidmethod].[FunctionLib] (Flagged, FileName, NameSpace, Class, Function, Region, LineNum, Tags, Comments, TimeStamp, FunctionLine, FunctionText, FilePath, DLLVer, Example, Documentation, UsedInProjects, DateAdded, UsesErrorLevel, Tested, ToDo) VALUES (@Flagged, @FileName, @NameSpace, @Class, @Function, @Region, @LineNum, @Tags, @Comments, @TimeStamp, @FunctionLine, @FunctionText, @FilePath, @DLLVer, @Example, @Documentation, @UsedInProjects, @DateAdded, @UsesErrorLevel, @Tested, @ToDo)";

            SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);

            cmd2 = new SqlCommand(SQLLine, Conn);

            if (obj.Flagged == null) { obj.Flagged = ""; }
            if (obj.FileName == null) { obj.FileName = ""; }
            if (obj.NameSpace == null) { obj.NameSpace = ""; }
            if (obj.Class == null) { obj.Class = ""; }
            if (obj.Function == null) { obj.Function = ""; }
            if (obj.Region == null) { obj.Region = ""; }
            if (obj.LineNum == null) { obj.LineNum = ""; }
            if (obj.Tags == null) { obj.Tags = ""; }
            if (obj.Comments == null) { obj.Comments = ""; }
            if (obj.TimeStamp == null) { obj.TimeStamp = ""; }
            if (obj.FunctionLine == null) { obj.FunctionLine = ""; }
            if (obj.FunctionText == null) { obj.FunctionText = ""; }
            if (obj.FilePath == null) { obj.FilePath = ""; }
            if (obj.DLLVer == null) { obj.DLLVer = ""; }
            if (obj.Example == null) { obj.Example = ""; }
            if (obj.Documentation == null) { obj.Documentation = ""; }
            if (obj.UsedInProjects == null) { obj.UsedInProjects = ""; }
            if (obj.DateAdded == null) { obj.DateAdded = ""; }
            if (obj.UsesErrorLevel == null) { obj.UsesErrorLevel = ""; }
            if (obj.Tested == null) { obj.Tested = ""; }
            if (obj.ToDo == null) { obj.ToDo = ""; }


            cmd2.Parameters.AddWithValue(@"Flagged", obj.Flagged.ToString());
            cmd2.Parameters.AddWithValue(@"FileName", obj.FileName.ToString());
            cmd2.Parameters.AddWithValue(@"NameSpace", obj.NameSpace.ToString());
            cmd2.Parameters.AddWithValue(@"Class", obj.Class.ToString());
            cmd2.Parameters.AddWithValue(@"Function", obj.Function.ToString());
            cmd2.Parameters.AddWithValue(@"Region", obj.Region.ToString());
            cmd2.Parameters.AddWithValue(@"LineNum", obj.LineNum.ToString());
            cmd2.Parameters.AddWithValue(@"Tags", obj.Tags.ToString());
            cmd2.Parameters.AddWithValue(@"Comments", obj.Comments.ToString());
            cmd2.Parameters.AddWithValue(@"FunctionLine", obj.FunctionLine.ToString());
            cmd2.Parameters.AddWithValue(@"FunctionText", obj.FunctionText.ToString());
            cmd2.Parameters.AddWithValue(@"DLLVer", obj.DLLVer.ToString());
            cmd2.Parameters.AddWithValue(@"Examples", obj.Example.ToString());
            cmd2.Parameters.AddWithValue(@"Documentation", obj.Documentation.ToString());
            cmd2.Parameters.AddWithValue(@"UsedInProjects", obj.UsedInProjects.ToString());
            cmd2.Parameters.AddWithValue(@"DateAdded", DateTime.Now.ToString());
            cmd2.Parameters.AddWithValue(@"DateModified", DateTime.Now.ToString());
            cmd2.Parameters.AddWithValue(@"UsesErrorLevel", obj.UsesErrorLevel.ToString());
            cmd2.Parameters.AddWithValue(@"Tested", obj.Tested.ToString());
            cmd2.Parameters.AddWithValue(@"ToDo", obj.ToDo.ToString());

            if (Conn.State == ConnectionState.Closed) { Conn.Open(); }

            int recordsAffected = 0;

            try { recordsAffected = cmd2.ExecuteNonQuery(); }

            catch (SqlException ex) { MessageBox.Show(ex.ToString()); }

            Conn.Close();

            if (recordsAffected > 0) { return true; }

            else return false;
        }

        /// <summary> </summary>
        /// <param name="Conn"> </param>
        /// <param name=" functionLib obj"> </param>
        /// <param name="WhereClause"> </param>
        public bool Update_SQLFunctionLib(SqlConnection Conn, functionLib obj, string WhereClause = "")
        {
            string SQLLine = "Update [sharpAHK].[lucidmethod].[FunctionLib] SET Flagged = @Flagged, FileName = @FileName, NameSpace = @NameSpace, Class = @Class, Function = @Function, Region = @Region, LineNum = @LineNum, Tags = @Tags, Comments = @Comments, FunctionLine = @FunctionLine, FunctionText = @FunctionText, DLLVer = @DLLVer, Examples = @Examples, Documentation = @Documentation, UsedInProjects = @UsedInProjects, DateModified = @DateModified, UsesErrorLevel = @UsesErrorLevel, Tested = @Tested, ToDo = @ToDo " + WhereClause + "";

            SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);

            cmd2 = new SqlCommand(SQLLine, Conn);

            //if (obj.ID == null) { obj.ID = ""; }
            if (obj.Flagged == null) { obj.Flagged = ""; }
            if (obj.FileName == null) { obj.FileName = ""; }
            if (obj.NameSpace == null) { obj.NameSpace = ""; }
            if (obj.Class == null) { obj.Class = ""; }
            if (obj.Function == null) { obj.Function = ""; }
            if (obj.Region == null) { obj.Region = ""; }
            if (obj.LineNum == null) { obj.LineNum = ""; }
            if (obj.Tags == null) { obj.Tags = ""; }
            if (obj.Comments == null) { obj.Comments = ""; }
            if (obj.TimeStamp == null) { obj.TimeStamp = ""; }
            if (obj.FunctionLine == null) { obj.FunctionLine = ""; }
            if (obj.FunctionText == null) { obj.FunctionText = ""; }
            if (obj.FilePath == null) { obj.FilePath = ""; }
            if (obj.DLLVer == null) { obj.DLLVer = ""; }
            if (obj.Example == null) { obj.Example = ""; }
            if (obj.Documentation == null) { obj.Documentation = ""; }
            if (obj.UsedInProjects == null) { obj.UsedInProjects = ""; }
            if (obj.DateAdded == null) { obj.DateAdded = ""; }
            if (obj.UsesErrorLevel == null) { obj.UsesErrorLevel = ""; }
            if (obj.Tested == null) { obj.Tested = ""; }
            if (obj.ToDo == null) { obj.ToDo = ""; }

            //cmd2.Parameters.AddWithValue(@"ID", obj.ID.ToString());
            cmd2.Parameters.AddWithValue(@"Flagged", obj.Flagged.ToString());
            cmd2.Parameters.AddWithValue(@"FileName", obj.FileName.ToString());
            cmd2.Parameters.AddWithValue(@"NameSpace", obj.NameSpace.ToString());
            cmd2.Parameters.AddWithValue(@"Class", obj.Class.ToString());
            cmd2.Parameters.AddWithValue(@"Function", obj.Function.ToString());
            cmd2.Parameters.AddWithValue(@"Region", obj.Region.ToString());
            cmd2.Parameters.AddWithValue(@"LineNum", obj.LineNum.ToString());
            cmd2.Parameters.AddWithValue(@"Tags", obj.Tags.ToString());
            cmd2.Parameters.AddWithValue(@"Comments", obj.Comments.ToString());
            cmd2.Parameters.AddWithValue(@"FunctionLine", obj.FunctionLine.ToString());
            cmd2.Parameters.AddWithValue(@"FunctionText", obj.FunctionText.ToString());
            cmd2.Parameters.AddWithValue(@"DLLVer", obj.DLLVer.ToString());
            cmd2.Parameters.AddWithValue(@"Examples", obj.Example.ToString());
            cmd2.Parameters.AddWithValue(@"Documentation", obj.Documentation.ToString());
            cmd2.Parameters.AddWithValue(@"UsedInProjects", obj.UsedInProjects.ToString());
            //cmd2.Parameters.AddWithValue(@"DateAdded", DateTime.Now.ToString());
            cmd2.Parameters.AddWithValue(@"DateModified", DateTime.Now.ToString());
            cmd2.Parameters.AddWithValue(@"UsesErrorLevel", obj.UsesErrorLevel.ToString());
            cmd2.Parameters.AddWithValue(@"Tested", obj.Tested.ToString());
            cmd2.Parameters.AddWithValue(@"ToDo", obj.ToDo.ToString());

            if (Conn.State == ConnectionState.Closed) { Conn.Open(); }

            int recordsAffected = 0;

            try { recordsAffected = cmd2.ExecuteNonQuery(); }

            catch (SqlException ex) { MessageBox.Show(ex.ToString()); }

            Conn.Close();

            if (recordsAffected > 0) { return true; }

            else return false;
        }


        #endregion

        #region === GoDaddy : SQL Code-Gen ===

        /// <summary> </summary>
        /// <param name="DbName"> </param>
        /// <param name="TableName"> </param>
        public string GoDaddy_SQL_Column_CodeGen(string DbName = "sharpAHK", string TableName = "FunctionLib")
        {
            _Code code = new _Code();
            return code.SQLColumns_To_Code(GoDad, "GoDad", DbName, TableName, false, false);
        }



        #endregion


    }

    public class Sync
        {

            RadLabel ddlDllCheckDisp = new RadLabel();

            public void sb(string Text = "")
            {

            }

            public bool SyncDLL_ToServer(string dllPath)
            {
                _AHK ahk = new _AHK();

                fileDb db = new fileDb();

                sb("Checking If DLL Needs Updating...");

                bool UpdateServer = db.NeedToUpdateServer(dllPath);  // checks server and local date modifed, returns true if time to write to server

                sb("Need To Update Server = " + UpdateServer.ToString());

                bool inserted = false;

                if (UpdateServer)
                {
                    if (!File.Exists(dllPath)) { ddlDllCheckDisp.Text = dllPath + " Not Found"; return false; }

                    string dllZipPath = dllPath.Replace(ahk.FileExt(dllPath), ".zip"); // new zipfile path

                    sb("Zipping " + dllZipPath);

                    db.ZipDll(dllPath);

                    System.IO.FileInfo fileinfo = new System.IO.FileInfo(dllPath); //retrieve info about each file

                    fileDb.FileDb ahkDll = new fileDb.FileDb();

                    ahkDll.ZipFile = dllZipPath;
                    ahkDll.FileSize = fileinfo.Length.ToString();  // # of bytes as an int
                                                                   //string FileSize = ahk.FormatBytes(fileinfo.Length);  // convert bytes to Text representation (adds kb/mb/tb to return)

                    ahkDll.DateModified = fileinfo.LastWriteTime.ToString();
                    //string LastAccessTime = fileinfo.LastAccessTime.ToString();
                    //string CreationTime = fileinfo.CreationTime.ToString();

                    //bool IsReadOnly = fileinfo.IsReadOnly;
                    ahkDll.FileExt = fileinfo.Extension.ToString();
                    ahkDll.FileExt = ahkDll.FileExt.Replace(".", ""); // remove period from file ext

                    ahkDll.FileName = ahk.FileName(dllPath);
                    ahkDll.FilePath = dllPath;

                    sb("Writing Object To SQL...");

                    inserted = db.FileDb_InsertSQL(ahkDll);
                    //ahk.MsgBox("inserted dll = " + inserted.ToString());
                }

                sb("Inserted Dll = " + inserted.ToString());
                return inserted;
            }


            public class fileDb
            {
                _AHK ahk = new _AHK();
                _Database.SQL sql = new _Database.SQL();
                _Database.SQLite sqlite = new _Database.SQLite();


                public bool NeedToUpdateServer(string dllPath) // checks server and local date modifed, returns true if time to write to server
                {
                    string cmd = "select top 1 [DateModified] FROM [fileserver].[lucidmethod].[FileDb] where [FileName] = '" + ahk.FileName(dllPath) + "' order by [DateModified] desc";
                    string dateModified = sql.SQL_Return_Value(Conn(), cmd);

                    System.IO.FileInfo fileinfo = new System.IO.FileInfo(dllPath); //retrieve info about each file

                    if (dateModified == fileinfo.LastWriteTime.ToString())
                    {
                        //ahk.MsgBox(fileinfo.LastWriteTime.ToString() + Environment.NewLine + dateModified.ToDateTime() + Environment.NewLine + "Same");
                        return false;
                    }

                    return true;  // dates are different

                    int result = DateTime.Compare(fileinfo.LastWriteTime, dateModified.ToDateTime());
                    if (result < 0) { Console.WriteLine("file date is less than server date"); return false; }
                    else if (result == 0) { Console.WriteLine("Both dates are same"); return false; }
                    else if (result > 0)
                    {
                        Console.WriteLine("file date is greater than server date");
                        //ahk.MsgBox("Need To Update Server: Local File Date is Newer than Server Date");
                        ahk.MsgBox("Result = " + result + "\n\nNeed To Update Server: Local File Date is Newer than Server Date");
                        return true;
                    }

                    return false;
                }


                #region === FileDb FUNCTIONS ===

                #region ===== FileDb Object =====

                public struct FileDb
                {
                    public string ID { get; set; }
                    public string FileName { get; set; }
                    public string FilePath { get; set; }
                    public string FileExt { get; set; }
                    public string DateAdded { get; set; }
                    public string DateModified { get; set; }
                    public string FileText { get; set; }
                    public string ZipFile { get; set; }
                    public string FileVersion { get; set; }
                    public string FileImage { get; set; }
                    public string Flag { get; set; }
                    public string FileSize { get; set; }
                    public string FileNote { get; set; }
                    public string FileTags { get; set; }
                }


                // Compare two objects to see if they have identical values
                public bool FileDb_Changed(FileDb OldVal, FileDb NewVal)
                {
                    FileDb diff = new FileDb();
                    List<string> diffList = new List<string>();
                    bool different = false;
                    if (OldVal.ID == null) { OldVal.ID = ""; }
                    if (NewVal.ID == null) { NewVal.ID = ""; }
                    if (OldVal.ID != NewVal.ID) { different = true; }
                    if (OldVal.FileName == null) { OldVal.FileName = ""; }
                    if (NewVal.FileName == null) { NewVal.FileName = ""; }
                    if (OldVal.FileName != NewVal.FileName) { different = true; }
                    if (OldVal.FilePath == null) { OldVal.FilePath = ""; }
                    if (NewVal.FilePath == null) { NewVal.FilePath = ""; }
                    if (OldVal.FilePath != NewVal.FilePath) { different = true; }
                    if (OldVal.FileExt == null) { OldVal.FileExt = ""; }
                    if (NewVal.FileExt == null) { NewVal.FileExt = ""; }
                    if (OldVal.FileExt != NewVal.FileExt) { different = true; }
                    if (OldVal.DateAdded == null) { OldVal.DateAdded = ""; }
                    if (NewVal.DateAdded == null) { NewVal.DateAdded = ""; }
                    if (OldVal.DateAdded != NewVal.DateAdded) { different = true; }
                    if (OldVal.DateModified == null) { OldVal.DateModified = ""; }
                    if (NewVal.DateModified == null) { NewVal.DateModified = ""; }
                    if (OldVal.DateModified != NewVal.DateModified) { different = true; }
                    if (OldVal.FileText == null) { OldVal.FileText = ""; }
                    if (NewVal.FileText == null) { NewVal.FileText = ""; }
                    if (OldVal.FileText != NewVal.FileText) { different = true; }
                    if (OldVal.ZipFile == null) { OldVal.ZipFile = ""; }
                    if (NewVal.ZipFile == null) { NewVal.ZipFile = ""; }
                    if (OldVal.ZipFile != NewVal.ZipFile) { different = true; }
                    if (OldVal.FileVersion == null) { OldVal.FileVersion = ""; }
                    if (NewVal.FileVersion == null) { NewVal.FileVersion = ""; }
                    if (OldVal.FileVersion != NewVal.FileVersion) { different = true; }
                    if (OldVal.FileImage == null) { OldVal.FileImage = ""; }
                    if (NewVal.FileImage == null) { NewVal.FileImage = ""; }
                    if (OldVal.FileImage != NewVal.FileImage) { different = true; }
                    if (OldVal.Flag == null) { OldVal.Flag = ""; }
                    if (NewVal.Flag == null) { NewVal.Flag = ""; }
                    if (OldVal.Flag != NewVal.Flag) { different = true; }
                    if (OldVal.FileSize == null) { OldVal.FileSize = ""; }
                    if (NewVal.FileSize == null) { NewVal.FileSize = ""; }
                    if (OldVal.FileSize != NewVal.FileSize) { different = true; }
                    if (OldVal.FileNote == null) { OldVal.FileNote = ""; }
                    if (NewVal.FileNote == null) { NewVal.FileNote = ""; }
                    if (OldVal.FileNote != NewVal.FileNote) { different = true; }
                    if (OldVal.FileTags == null) { OldVal.FileTags = ""; }
                    if (NewVal.FileTags == null) { NewVal.FileTags = ""; }
                    if (OldVal.FileTags != NewVal.FileTags) { different = true; }
                    return different;
                }




                #endregion
                #region ===== FileDb SQLite : Return =====

                public FileDb Return_Object_From_FileDb(string DbFile, string WhereClause = "[ID] = ''")
                {
                    string SelectLine = "Select [ID], [FileName], [FilePath], [FileExt], [DateAdded], [DateModified], [FileText], [ZipFile], [FileVersion], [FileImage], [Flag], [FileSize], [FileNote], [FileTags] From [FileDb] ";
                    DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                    if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
                    FileDb returnObject = new FileDb();
                    int i = 0;
                    string Value = "";
                    if (ReturnTable != null)
                    {
                        foreach (DataRow ret in ReturnTable.Rows)
                        {
                            returnObject.ID = ret["ID"].ToString();
                            returnObject.FileName = ret["FileName"].ToString();
                            returnObject.FilePath = ret["FilePath"].ToString();
                            returnObject.FileExt = ret["FileExt"].ToString();
                            returnObject.DateAdded = ret["DateAdded"].ToString();
                            returnObject.DateModified = ret["DateModified"].ToString();
                            returnObject.FileText = ret["FileText"].ToString();
                            returnObject.ZipFile = ret["ZipFile"].ToString();
                            returnObject.FileVersion = ret["FileVersion"].ToString();
                            returnObject.FileImage = ret["FileImage"].ToString();
                            returnObject.Flag = ret["Flag"].ToString();
                            returnObject.FileSize = ret["FileSize"].ToString();
                            returnObject.FileNote = ret["FileNote"].ToString();
                            returnObject.FileTags = ret["FileTags"].ToString();
                        }
                    }

                    return returnObject;
                }

                public List<FileDb> Return_FileDb_List(string DbFile, string TableName = "[FileDb]", string WhereClause = "")
                {
                    string SelectLine = "Select * From " + TableName;

                    if (WhereClause != "")
                    {
                        if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                        if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                    }
                    DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

                    List<FileDb> ReturnList = new List<FileDb>();
                    if (ReturnTable != null)
                    {
                        foreach (DataRow ret in ReturnTable.Rows)
                        {
                            FileDb returnObject = new FileDb();

                            returnObject.ID = ret["ID"].ToString();
                            returnObject.FileName = ret["FileName"].ToString();
                            returnObject.FilePath = ret["FilePath"].ToString();
                            returnObject.FileExt = ret["FileExt"].ToString();
                            returnObject.DateAdded = ret["DateAdded"].ToString();
                            returnObject.DateModified = ret["DateModified"].ToString();
                            returnObject.FileText = ret["FileText"].ToString();
                            returnObject.ZipFile = ret["ZipFile"].ToString();
                            returnObject.FileVersion = ret["FileVersion"].ToString();
                            returnObject.FileImage = ret["FileImage"].ToString();
                            returnObject.Flag = ret["Flag"].ToString();
                            returnObject.FileSize = ret["FileSize"].ToString();
                            returnObject.FileNote = ret["FileNote"].ToString();
                            returnObject.FileTags = ret["FileTags"].ToString();

                            ReturnList.Add(returnObject);
                        }
                    }

                    return ReturnList;
                }

                public DataTable Return_DataTable_From_FileDb(string DbFile)
                {
                    string SelectLine = "Select [ID], [FileName], [FilePath], [FileExt], [DateAdded], [DateModified], [FileText], [ZipFile], [FileVersion], [FileImage], [Flag], [FileSize], [FileNote], [FileTags] From [FileDb]";
                    DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                    return ReturnTable;
                }


                #endregion
                #region ===== FileDb SQLite : Update Insert =====

                public bool FileDb_Insert(string DbFile, FileDb inObject)
                {
                    string InsertLine = "Insert Into [FileDb] (ID, FileName, FilePath, FileExt, DateAdded, DateModified, FileText, ZipFile, FileVersion, FileImage, Flag, FileSize, FileNote, FileTags) values ('" + inObject.ID + "', '" + inObject.FileName + "', '" + inObject.FilePath + "', '" + inObject.FileExt + "', '" + inObject.DateAdded + "', '" + inObject.DateModified + "', '" + inObject.FileText + "', '" + inObject.ZipFile + "', '" + inObject.FileVersion + "', '" + inObject.FileImage + "', '" + inObject.Flag + "', '" + inObject.FileSize + "', '" + inObject.FileNote + "', '" + inObject.FileTags + "')";
                    bool Inserted = sqlite.Execute(DbFile, InsertLine);
                    if (!Inserted) { ahk.MsgBox("Inserted Into [FileDb] = " + Inserted.ToString()); }
                    return Inserted;
                }

                public bool FileDb_Update(string DbFile, FileDb inObject, string WHERE = "[Item] = 'Value'")
                {
                    //string UpdateLine = "Update [FileDb] set ID = '" + inObject.ID + "', FileName = '" + inObject.FileName + "', FilePath = '" + inObject.FilePath + "', FileExt = '" + inObject.FileExt + "', DateAdded = '" + inObject.DateAdded + "', DateModified = '" + inObject.DateModified + "', FileText = '" + inObject.FileText + "', ZipFile = '" + inObject.ZipFile + "', FileVersion = '" + inObject.FileVersion + "', FileImage = '" + inObject.FileImage + "', Flag = '" + inObject.Flag + "', FileSize = '" + inObject.FileSize + "', FileNote = '" + inObject.FileNote + "', FileTags = '" + inObject.FileTags + "' WHERE [Item] = 'Value' ";
                    string UpdateLine = "Update [FileDb] set ";


                    if (inObject.ID != null) { UpdateLine = UpdateLine + "[ID] = '" + inObject.ID + "',"; }
                    if (inObject.FileName != null) { UpdateLine = UpdateLine + "[FileName] = '" + inObject.FileName + "',"; }
                    if (inObject.FilePath != null) { UpdateLine = UpdateLine + "[FilePath] = '" + inObject.FilePath + "',"; }
                    if (inObject.FileExt != null) { UpdateLine = UpdateLine + "[FileExt] = '" + inObject.FileExt + "',"; }
                    if (inObject.DateAdded != null) { UpdateLine = UpdateLine + "[DateAdded] = '" + inObject.DateAdded + "',"; }
                    if (inObject.DateModified != null) { UpdateLine = UpdateLine + "[DateModified] = '" + inObject.DateModified + "',"; }
                    if (inObject.FileText != null) { UpdateLine = UpdateLine + "[FileText] = '" + inObject.FileText + "',"; }
                    if (inObject.ZipFile != null) { UpdateLine = UpdateLine + "[ZipFile] = '" + inObject.ZipFile + "',"; }
                    if (inObject.FileVersion != null) { UpdateLine = UpdateLine + "[FileVersion] = '" + inObject.FileVersion + "',"; }
                    if (inObject.FileImage != null) { UpdateLine = UpdateLine + "[FileImage] = '" + inObject.FileImage + "',"; }
                    if (inObject.Flag != null) { UpdateLine = UpdateLine + "[Flag] = '" + inObject.Flag + "',"; }
                    if (inObject.FileSize != null) { UpdateLine = UpdateLine + "[FileSize] = '" + inObject.FileSize + "',"; }
                    if (inObject.FileNote != null) { UpdateLine = UpdateLine + "[FileNote] = '" + inObject.FileNote + "',"; }
                    if (inObject.FileTags != null) { UpdateLine = UpdateLine + "[FileTags] = '" + inObject.FileTags + "',"; }

                    UpdateLine = ahk.TrimLast(UpdateLine, 1);
                    UpdateLine = UpdateLine + " WHERE " + WHERE;

                    bool Updated = sqlite.Execute(DbFile, UpdateLine);
                    return Updated;
                }

                public bool FileDb_UpdateInsert(string DbFile, FileDb obj, string WhereClause = "")
                {
                    bool Updated = FileDb_Update(DbFile, obj, WhereClause);  // try to update record first
                    if (!Updated) { Updated = FileDb_Insert(DbFile, obj); }  // if unable to update, insert new record
                    return Updated;
                }


                #endregion
                #region ===== FileDb DataTable =====

                public DataTable Return_FileDb_DataTable(string DbFile, string TableName = "FileDb", string WhereClause = "", bool Debug = false)
                {
                    string SelectLine = "Select * From [FileDb]";

                    if (WhereClause != "")
                    {
                        if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                        if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                    }

                    DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


                    DataTable table = new DataTable();
                    table.Columns.Add("ID", typeof(string));
                    table.Columns.Add("FileName", typeof(string));
                    table.Columns.Add("FilePath", typeof(string));
                    table.Columns.Add("FileExt", typeof(string));
                    table.Columns.Add("DateAdded", typeof(string));
                    table.Columns.Add("DateModified", typeof(string));
                    table.Columns.Add("FileText", typeof(string));
                    table.Columns.Add("ZipFile", typeof(string));
                    table.Columns.Add("FileVersion", typeof(string));
                    table.Columns.Add("FileImage", typeof(string));
                    table.Columns.Add("Flag", typeof(string));
                    table.Columns.Add("FileSize", typeof(string));
                    table.Columns.Add("FileNote", typeof(string));
                    table.Columns.Add("FileTags", typeof(string));

                    if (ReturnTable != null)
                    {
                        foreach (DataRow ret in ReturnTable.Rows)
                        {
                            FileDb returnObject = new FileDb();

                            returnObject.ID = ret["ID"].ToString();
                            returnObject.FileName = ret["FileName"].ToString();
                            returnObject.FilePath = ret["FilePath"].ToString();
                            returnObject.FileExt = ret["FileExt"].ToString();
                            returnObject.DateAdded = ret["DateAdded"].ToString();
                            returnObject.DateModified = ret["DateModified"].ToString();
                            returnObject.FileText = ret["FileText"].ToString();
                            returnObject.ZipFile = ret["ZipFile"].ToString();
                            returnObject.FileVersion = ret["FileVersion"].ToString();
                            returnObject.FileImage = ret["FileImage"].ToString();
                            returnObject.Flag = ret["Flag"].ToString();
                            returnObject.FileSize = ret["FileSize"].ToString();
                            returnObject.FileNote = ret["FileNote"].ToString();
                            returnObject.FileTags = ret["FileTags"].ToString();

                            table.Rows.Add(returnObject.ID, returnObject.FileName, returnObject.FilePath, returnObject.FileExt, returnObject.DateAdded, returnObject.DateModified, returnObject.FileText, returnObject.ZipFile, returnObject.FileVersion, returnObject.FileImage, returnObject.Flag, returnObject.FileSize, returnObject.FileNote, returnObject.FileTags);
                        }
                    }

                    return table;
                }

                public DataTable Create_FileDb_DataTable(FileDb inObject)
                {
                    DataTable table = new DataTable();
                    table.Columns.Add("ID", typeof(string));
                    table.Columns.Add("FileName", typeof(string));
                    table.Columns.Add("FilePath", typeof(string));
                    table.Columns.Add("FileExt", typeof(string));
                    table.Columns.Add("DateAdded", typeof(string));
                    table.Columns.Add("DateModified", typeof(string));
                    table.Columns.Add("FileText", typeof(string));
                    table.Columns.Add("ZipFile", typeof(string));
                    table.Columns.Add("FileVersion", typeof(string));
                    table.Columns.Add("FileImage", typeof(string));
                    table.Columns.Add("Flag", typeof(string));
                    table.Columns.Add("FileSize", typeof(string));
                    table.Columns.Add("FileNote", typeof(string));
                    table.Columns.Add("FileTags", typeof(string));

                    table.Rows.Add(inObject.ID, inObject.FileName, inObject.FilePath, inObject.FileExt, inObject.DateAdded, inObject.DateModified, inObject.FileText, inObject.ZipFile, inObject.FileVersion, inObject.FileImage, inObject.Flag, inObject.FileSize, inObject.FileNote, inObject.FileTags);
                    return table;
                }


                #endregion
                #region ===== FileDb DataGridView =====

                public void HideShow_FileDb_Columns(DataGridView dv)
                {

                    try { dv.Columns["ID"].Visible = true; } catch { }
                    try { dv.Columns["FileName"].Visible = true; } catch { }
                    try { dv.Columns["FilePath"].Visible = true; } catch { }
                    try { dv.Columns["FileExt"].Visible = true; } catch { }
                    try { dv.Columns["DateAdded"].Visible = true; } catch { }
                    try { dv.Columns["DateModified"].Visible = true; } catch { }
                    try { dv.Columns["FileText"].Visible = true; } catch { }
                    try { dv.Columns["ZipFile"].Visible = true; } catch { }
                    try { dv.Columns["FileVersion"].Visible = true; } catch { }
                    try { dv.Columns["FileImage"].Visible = true; } catch { }
                    try { dv.Columns["Flag"].Visible = true; } catch { }
                    try { dv.Columns["FileSize"].Visible = true; } catch { }
                    try { dv.Columns["FileNote"].Visible = true; } catch { }
                    try { dv.Columns["FileTags"].Visible = true; } catch { }
                }
                public void Enable_TableName_Columns(DataGridView dv)
                {

                    try { dv.Columns["ID"].ReadOnly = true; } catch { }
                    try { dv.Columns["FileName"].ReadOnly = true; } catch { }
                    try { dv.Columns["FilePath"].ReadOnly = true; } catch { }
                    try { dv.Columns["FileExt"].ReadOnly = true; } catch { }
                    try { dv.Columns["DateAdded"].ReadOnly = true; } catch { }
                    try { dv.Columns["DateModified"].ReadOnly = true; } catch { }
                    try { dv.Columns["FileText"].ReadOnly = true; } catch { }
                    try { dv.Columns["ZipFile"].ReadOnly = true; } catch { }
                    try { dv.Columns["FileVersion"].ReadOnly = true; } catch { }
                    try { dv.Columns["FileImage"].ReadOnly = true; } catch { }
                    try { dv.Columns["Flag"].ReadOnly = true; } catch { }
                    try { dv.Columns["FileSize"].ReadOnly = true; } catch { }
                    try { dv.Columns["FileNote"].ReadOnly = true; } catch { }
                    try { dv.Columns["FileTags"].ReadOnly = true; } catch { }
                }

                #endregion
                #region ===== FileDb SQL Functions =====

                // Return FileDb SQL Connection String
                public SqlConnection Conn()
                {
                    // populate sql connection
                    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLserver"].ConnectionString);
                    // SqlConnection Conn = new SqlConnection("Server=188.168.188.88;DataBase=LucidMedia;Uid=lucidm;Pwd=pass");
                    return conn;
                }

                // Return FileDb TableName (Full Path)
                public string FileDb_TableName()
                {
                    // populate to return full sql table name
                    return "FileDb";
                }

                // Generate SQL Table
                public bool FileDb_CreateSQLTable()
                {
                    string CreateTableLine = "CREATE TABLE [FileDb](";
                    CreateTableLine = CreateTableLine + "[ID] [int] IDENTITY(1,1) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[FileName] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[FilePath] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[FileExt] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[DateAdded] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[DateModified] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[FileText] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[ZipFile] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[FileVersion] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[FileImage] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[Flag] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[FileSize] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[FileNote] [varchar](max) NOT NULL,";
                    CreateTableLine = CreateTableLine + "[FileTags] [varchar](max) NOT NULL";
                    CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                    return false;
                }

                public bool FileDb_InsertSQL(FileDb obj)
                {
                    SqlConnection con = Conn();

                    using (var sqlWrite = new SqlCommand("Insert Into [fileserver].[lucidmethod].[FileDb] (FileName, FilePath, FileExt, DateAdded, DateModified, FileText, ZipFile, FileVersion, Flag, FileSize, FileNote, FileTags) VALUES (@FileName, @FilePath, @FileExt, @DateAdded, @DateModified, @FileText, @ZipFile, @FileVersion, @Flag, @FileSize, @FileNote, @FileTags)", con))
                    //using (var sqlWrite = new SqlCommand("Insert Into [fileserver].[lucidmethod].[FileDb] (FileName, FilePath, FileExt, DateAdded, DateModified, FileText, ZipFile, FileVersion, FileImage, Flag, FileSize, FileNote, FileTags) VALUES (@FileName, @FilePath, @FileExt, @DateAdded, @DateModified, @FileText, @ZipFile, @FileVersion, @FileImage, @Flag, @FileSize, @FileNote, @FileTags)", con))
                    {
                        if (obj.FileVersion == null)
                        {
                            // checks server for current version number, adds increment, and returns next version number to write to server
                            obj.FileVersion = NextFileVersion(obj.FileName).ToString();
                        }
                        //if (obj.FileImage == null) { obj.FileVersion = "0.1"; }
                        if (obj.FileText == null) { obj.FileText = ""; }
                        if (obj.ZipFile == null) { obj.ZipFile = ""; }
                        //if (obj.FileImage == null) { obj.FileImage = ""; }
                        if (obj.Flag == null) { obj.Flag = "0"; }
                        //if (obj.FileSize == null) { obj.FileSize = ""; }
                        if (obj.FileNote == null) { obj.FileNote = ""; }
                        if (obj.FileTags == null) { obj.FileTags = ""; }


                        sqlWrite.Parameters.AddWithValue(@"FileName", obj.FileName.ToString());
                        sqlWrite.Parameters.AddWithValue(@"FilePath", obj.FilePath.ToString());
                        sqlWrite.Parameters.AddWithValue(@"FileExt", obj.FileExt.ToString());
                        sqlWrite.Parameters.AddWithValue(@"DateAdded", DateTime.Now);
                        sqlWrite.Parameters.AddWithValue(@"DateModified", obj.DateModified.ToString());
                        sqlWrite.Parameters.AddWithValue(@"FileVersion", obj.FileVersion.ToString());
                        sqlWrite.Parameters.AddWithValue(@"Flag", obj.Flag.ToString());
                        sqlWrite.Parameters.AddWithValue(@"FileSize", obj.FileSize.ToString());
                        sqlWrite.Parameters.AddWithValue(@"FileNote", obj.FileNote.ToString());
                        sqlWrite.Parameters.AddWithValue(@"FileTags", obj.FileTags.ToString());
                        sqlWrite.Parameters.AddWithValue(@"FileText", obj.FileText.ToString());

                        //// filler file to avoid errors if no img defined
                        //if (obj.FileImage == null || obj.FileImage == "") { obj.FileImage = ahk.AppDir() + "\\ICO\\IcoLibSort_Icons_by_WAWAA_22x22_actions_line_double_arrow_begin.png"; }

                        //byte[] imgfile;
                        //using (var stream = new FileStream(obj.FileImage.ToString(), FileMode.Open, FileAccess.Read))
                        //{
                        //    using (var reader = new BinaryReader(stream))
                        //    {
                        //        imgfile = reader.ReadBytes((int)stream.Length);
                        //    }
                        //}
                        //sqlWrite.Parameters.Add(@"FileImage", SqlDbType.VarBinary, imgfile.Length).Value = imgfile;



                        byte[] file;
                        using (var stream = new FileStream(obj.ZipFile.ToString(), FileMode.Open, FileAccess.Read))
                        {
                            using (var reader = new BinaryReader(stream))
                            {
                                file = reader.ReadBytes((int)stream.Length);
                            }
                        }

                        sqlWrite.Parameters.AddWithValue(@"ZipFile", file);

                        //sqlWrite.Parameters.Add(@"ZipFile", SqlDbType.VarBinary, file.Length).Value = file;


                        if (con.State == ConnectionState.Closed) { con.Open(); }
                        //sqlWrite.ExecuteNonQuery();
                        int recordsAffected = 0;
                        try { recordsAffected = sqlWrite.ExecuteNonQuery(); }
                        catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                        con.Close();
                        if (recordsAffected > 0) { return true; }
                        else return false;

                    }
                }

                // checks server for current version number, adds increment, and returns next version number to write to server
                public string NextFileVersion(string FileName)
                {
                    string cmd = "select top (1) [FileVersion] FROM [fileserver].[lucidmethod].[FileDb] where FileName = '" + FileName + "' order by DateAdded desc";
                    string CurrentVersion = sql.SQL_Return_Value(Conn(), cmd);

                    int currentVersion = 0;
                    if (CurrentVersion == "" || CurrentVersion == null) { currentVersion = 0; }
                    else { currentVersion = CurrentVersion.ToInt(); }

                    currentVersion++;
                    return currentVersion.ToString();
                }


                public bool ZipDll(string DllDebugFilePath, string NewZipPath = "")
                {
                    //List<string> files = new List<string>();

                    //// add dll + documentation from visual studio debug dir
                    //files.Add(DllDebugFilePath);
                    ////files.Add(ahk.FileDir(DllDebugFilePath) + "\\" + ahk.FileNameNoExt(DllDebugFilePath) + ".pdb");
                    ////files.Add(ahk.FileDir(DllDebugFilePath) + "\\" + ahk.FileName(DllDebugFilePath) + ".config");

                    //string path = ahk.FileDir(DllDebugFilePath) + "\\" + ahk.FileNameNoExt(DllDebugFilePath) + ".xml";
                    //if (File.Exists(path)) { files.Add(path); }

                    //try
                    //{
                    //    using (ZipFile zip = new ZipFile())
                    //    {
                    //        foreach (string file in files)
                    //        {
                    //            if (File.Exists(file))
                    //            {
                    //                ZipEntry e = zip.AddFile(file, "");
                    //                e.Comment = "Released by Lucid_Method " + DateTime.Now.ToShortDateString();
                    //            }
                    //        }

                    //        string savePath = ahk.FileDir(DllDebugFilePath) + "\\" + ahk.FileNameNoExt(DllDebugFilePath) + ".zip";
                    //        if (NewZipPath != "") { savePath = NewZipPath; }
                    //        zip.Save(savePath);
                    //    }
                    //}
                    //catch (System.Exception ex1)
                    //{
                    //    System.Console.Error.WriteLine("exception: " + ex1);
                    //    return false;
                    //}

                    return true;
                }

                public bool FileDb_UpdateSQL(FileDb obj)
                {
                    string SQLLine = "Update FileDb SET ID = @ID, FileName = @FileName, FilePath = @FilePath, FileExt = @FileExt, DateAdded = @DateAdded, DateModified = @DateModified, FileText = @FileText, ZipFile = @ZipFile, FileVersion = @FileVersion, Flag = @Flag, FileSize = @FileSize, FileNote = @FileNote, FileTags = @FileTags WHERE ID = @ID";
                    SqlCommand cmd2 = new SqlCommand(SQLLine, Conn());
                    cmd2 = new SqlCommand(SQLLine, Conn());
                    if (obj.ID == null) { obj.ID = ""; }
                    if (obj.FileName == null) { obj.FileName = ""; }
                    if (obj.FilePath == null) { obj.FilePath = ""; }
                    if (obj.FileExt == null) { obj.FileExt = ""; }
                    if (obj.DateAdded == null) { obj.DateAdded = ""; }
                    if (obj.DateModified == null) { obj.DateModified = ""; }
                    if (obj.FileText == null) { obj.FileText = ""; }
                    if (obj.ZipFile == null) { obj.ZipFile = ""; }
                    if (obj.FileVersion == null) { obj.FileVersion = ""; }
                    if (obj.Flag == null) { obj.Flag = ""; }
                    if (obj.FileSize == null) { obj.FileSize = ""; }
                    if (obj.FileNote == null) { obj.FileNote = ""; }
                    if (obj.FileTags == null) { obj.FileTags = ""; }
                    cmd2.Parameters.AddWithValue(@"ID", obj.ID.ToString());
                    cmd2.Parameters.AddWithValue(@"FileName", obj.FileName.ToString());
                    cmd2.Parameters.AddWithValue(@"FilePath", obj.FilePath.ToString());
                    cmd2.Parameters.AddWithValue(@"FileExt", obj.FileExt.ToString());
                    cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded.ToString());
                    cmd2.Parameters.AddWithValue(@"DateModified", obj.DateModified.ToString());
                    cmd2.Parameters.AddWithValue(@"FileText", obj.FileText.ToString());
                    cmd2.Parameters.AddWithValue(@"ZipFile", obj.ZipFile.ToString());
                    cmd2.Parameters.AddWithValue(@"FileVersion", obj.FileVersion.ToString());
                    cmd2.Parameters.AddWithValue(@"Flag", obj.Flag.ToString());
                    cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize.ToString());
                    cmd2.Parameters.AddWithValue(@"FileNote", obj.FileNote.ToString());
                    cmd2.Parameters.AddWithValue(@"FileTags", obj.FileTags.ToString());
                    if (Conn().State == ConnectionState.Closed) { Conn().Open(); }
                    int recordsAffected = 0;
                    try { recordsAffected = cmd2.ExecuteNonQuery(); }
                    catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                    Conn().Close();
                    if (recordsAffected > 0) { return true; }
                    else return false;
                }

                public bool FileDb_UpdateInsert(FileDb obj)
                {
                    bool Updated = FileDb_UpdateSQL(obj);  // try to update record first
                    if (!Updated) { Updated = FileDb_InsertSQL(obj); }  // if unable to update, insert new record
                    return Updated;
                }

                // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
                public bool FileDb_UpdateIfPopulated(FileDb obj, string ID = "")
                {
                    SqlConnection conn = Conn();
                    string SQLcmd = "Update FileDb SET ";
                    if (obj.ID != null) { SQLcmd = SQLcmd + " ID = @ID,"; }
                    if (obj.FileName != null) { SQLcmd = SQLcmd + " FileName = @FileName,"; }
                    if (obj.FilePath != null) { SQLcmd = SQLcmd + " FilePath = @FilePath,"; }
                    if (obj.FileExt != null) { SQLcmd = SQLcmd + " FileExt = @FileExt,"; }
                    if (obj.DateAdded != null) { SQLcmd = SQLcmd + " DateAdded = @DateAdded,"; }
                    if (obj.DateModified != null) { SQLcmd = SQLcmd + " DateModified = @DateModified,"; }
                    if (obj.FileText != null) { SQLcmd = SQLcmd + " FileText = @FileText,"; }
                    if (obj.ZipFile != null) { SQLcmd = SQLcmd + " ZipFile = @ZipFile,"; }
                    if (obj.FileVersion != null) { SQLcmd = SQLcmd + " FileVersion = @FileVersion,"; }
                    if (obj.Flag != null) { SQLcmd = SQLcmd + " Flag = @Flag,"; }
                    if (obj.FileSize != null) { SQLcmd = SQLcmd + " FileSize = @FileSize,"; }
                    if (obj.FileNote != null) { SQLcmd = SQLcmd + " FileNote = @FileNote,"; }
                    if (obj.FileTags != null) { SQLcmd = SQLcmd + " FileTags = @FileTags,"; }
                    SQLcmd = ahk.TrimLast(SQLcmd, 1);
                    SQLcmd = SQLcmd + " WHERE ID = @ID";

                    SqlCommand cmd2 = new SqlCommand(SQLcmd, conn);

                    if (obj.ID != null) { cmd2.Parameters.AddWithValue(@"ID", obj.ID); }
                    if (obj.FileName != null) { cmd2.Parameters.AddWithValue(@"FileName", obj.FileName); }
                    if (obj.FilePath != null) { cmd2.Parameters.AddWithValue(@"FilePath", obj.FilePath); }
                    if (obj.FileExt != null) { cmd2.Parameters.AddWithValue(@"FileExt", obj.FileExt); }
                    if (obj.DateAdded != null) { cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded); }
                    if (obj.DateModified != null) { cmd2.Parameters.AddWithValue(@"DateModified", obj.DateModified); }
                    if (obj.FileText != null) { cmd2.Parameters.AddWithValue(@"FileText", obj.FileText); }
                    if (obj.ZipFile != null) { cmd2.Parameters.AddWithValue(@"ZipFile", obj.ZipFile); }
                    if (obj.FileVersion != null) { cmd2.Parameters.AddWithValue(@"FileVersion", obj.FileVersion); }
                    if (obj.Flag != null) { cmd2.Parameters.AddWithValue(@"Flag", obj.Flag); }
                    if (obj.FileSize != null) { cmd2.Parameters.AddWithValue(@"FileSize", obj.FileSize); }
                    if (obj.FileNote != null) { cmd2.Parameters.AddWithValue(@"FileNote", obj.FileNote); }
                    if (obj.FileTags != null) { cmd2.Parameters.AddWithValue(@"FileTags", obj.FileTags); }

                    if (conn.State == ConnectionState.Closed) { conn.Open(); }
                    int recordsAffected = 0;
                    try { recordsAffected = cmd2.ExecuteNonQuery(); }
                    catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                    conn.Close();
                    if (recordsAffected > 0) { return true; }
                    else return false;
                }

                public FileDb FileDb_ReturnSQL(string FileName = "")
                {
                    SqlConnection conn = Conn();
                    string SelectLine = "Select top (1) [ID],[FileName],[FilePath],[FileExt],[DateAdded],[DateModified],[FileText],[ZipFile],[FileVersion],[Flag],[FileSize],[FileNote],[FileTags] From FileDb order by DateAdded desc WHERE FileName = '" + FileName + "'";
                    DataTable ReturnTable = sql.GetDataTable(conn, SelectLine);
                    FileDb returnObject = new FileDb();
                    if (ReturnTable != null)
                    {
                        foreach (DataRow ret in ReturnTable.Rows)
                        {
                            returnObject.ID = ret["ID"].ToString();
                            returnObject.FileName = ret["FileName"].ToString();
                            returnObject.FilePath = ret["FilePath"].ToString();
                            returnObject.FileExt = ret["FileExt"].ToString();
                            returnObject.DateAdded = ret["DateAdded"].ToString();
                            returnObject.DateModified = ret["DateModified"].ToString();
                            returnObject.FileText = ret["FileText"].ToString();
                            returnObject.ZipFile = ret["ZipFile"].ToString();
                            returnObject.FileVersion = ret["FileVersion"].ToString();
                            //returnObject.FileImage = ret["FileImage"].ToString();
                            returnObject.Flag = ret["Flag"].ToString();
                            returnObject.FileSize = ret["FileSize"].ToString();
                            returnObject.FileNote = ret["FileNote"].ToString();
                            returnObject.FileTags = ret["FileTags"].ToString();
                            return returnObject;
                        }
                    }
                    return returnObject;
                }

                public List<FileDb> FileDb_ReturnSQLList(string Command)
                {
                    SqlConnection conn = Conn();
                    DataTable ReturnTable = sql.GetDataTable(conn, Command);
                    List<FileDb> ReturnList = new List<FileDb>();
                    if (ReturnTable != null)
                    {
                        foreach (DataRow ret in ReturnTable.Rows)
                        {
                            FileDb returnObject = new FileDb();
                            returnObject.ID = ret["ID"].ToString();
                            returnObject.FileName = ret["FileName"].ToString();
                            returnObject.FilePath = ret["FilePath"].ToString();
                            returnObject.FileExt = ret["FileExt"].ToString();
                            returnObject.DateAdded = ret["DateAdded"].ToString();
                            returnObject.DateModified = ret["DateModified"].ToString();
                            returnObject.FileText = ret["FileText"].ToString();
                            returnObject.ZipFile = ret["ZipFile"].ToString();
                            returnObject.FileVersion = ret["FileVersion"].ToString();
                            //returnObject.FileImage = ret["FileImage"].ToString();
                            returnObject.Flag = ret["Flag"].ToString();
                            returnObject.FileSize = ret["FileSize"].ToString();
                            returnObject.FileNote = ret["FileNote"].ToString();
                            returnObject.FileTags = ret["FileTags"].ToString();
                            ReturnList.Add(returnObject);
                        }
                    }
                    return ReturnList;
                }


                /// <summary>
                /// Download File from SQL (Saved as Zip) using Original File Name
                /// </summary>
                /// <param name="FileName">Original File Name with Original Format (not zip)</param>
                /// <param name="LocalSaveDir">Directory to Save and Unzip File To</param>
                /// <returns></returns>
                public bool SQL_ZipDownload(string FileName, string LocalSaveDir, bool CreateSubDir = false, bool OpenDirAfter = false, bool DeleteZipAfterExtract = true)
                {
                    //FileDb file = FileDb_ReturnSQL(FileName);

                    //string savePath = LocalSaveDir + "\\" + FileName.FileNameNoExt() + ".zip";
                    //sql.ReturnFile(Conn(), "select top (1) [ZipFile] FROM [fileserver].[lucidmethod].[FileDb] where FileName = '" + FileName + "' order by DateAdded desc", savePath);
                    //ahk.Sleep(100);
                    //if (File.Exists(savePath))
                    //{

                    //    // option to create folder in save dir to extract new files to 
                    //    if (CreateSubDir)
                    //    {
                    //        string name = ahk.StringSplit(FileName, ".", 0); // just the file name (no ext)
                    //        string newDir = LocalSaveDir + "\\" + name;
                    //        ahk.FileCreateDir(newDir);
                    //        LocalSaveDir = newDir;
                    //    }

                    //    string extractedPath = LocalSaveDir + "\\" + FileName;  // location new file should be found after extracting

                    //    using (ZipFile zip1 = ZipFile.Read(savePath))
                    //    {
                    //        // here, we extract every entry, but we could extract conditionally
                    //        // based on entry name, size, date, checkbox status, etc.  
                    //        foreach (ZipEntry e in zip1)
                    //        {
                    //            e.Extract(LocalSaveDir, ExtractExistingFileAction.OverwriteSilently);
                    //        }
                    //    }

                    //    if (File.Exists(extractedPath))
                    //    {
                    //        if (DeleteZipAfterExtract) { ahk.FileDelete(savePath); }  // remove zip after extract
                    //        if (OpenDirAfter) { ahk.Run(LocalSaveDir); }
                    //        return true;
                    //    }
                    //    else
                    //    {
                    //        return false;  // failed to locate expected file after extracting
                    //    }

                    //}
                    return false;
                }



                #endregion

                #endregion

            }

        }

    public class LocalDB
        {
            #region === localDrives FUNCTIONS ===

            #region ===== localDrives Object =====

            public struct localDrives
            {
                public string ID { get; set; }
                public string Category { get; set; }
                public string DirPath { get; set; }
                public string Flag { get; set; }
            }
            //  Fix illegal characters before Sql/Sqlite Db Inserts
            public localDrives localDrives_FixChars(localDrives ToFix)
            {
                localDrives Fixed = new localDrives();

                Fixed.ID = ToFix.ID.Replace("'", "''");
                Fixed.Category = ToFix.Category.Replace("'", "''");
                Fixed.DirPath = ToFix.DirPath.Replace("'", "''");
                Fixed.Flag = ToFix.Flag.Replace("'", "''");

                return Fixed;
            }

            // Compare two objects to see if they have identical values
            public bool localDrives_Changed(localDrives OldVal, localDrives NewVal)
            {
                localDrives diff = new localDrives();
                List<string> diffList = new List<string>();
                bool different = false;
                if (OldVal.ID == null) { OldVal.ID = ""; }
                if (NewVal.ID == null) { NewVal.ID = ""; }
                if (OldVal.ID != NewVal.ID) { different = true; }
                if (OldVal.Category == null) { OldVal.Category = ""; }
                if (NewVal.Category == null) { NewVal.Category = ""; }
                if (OldVal.Category != NewVal.Category) { different = true; }
                if (OldVal.DirPath == null) { OldVal.DirPath = ""; }
                if (NewVal.DirPath == null) { NewVal.DirPath = ""; }
                if (OldVal.DirPath != NewVal.DirPath) { different = true; }
                if (OldVal.Flag == null) { OldVal.Flag = ""; }
                if (NewVal.Flag == null) { NewVal.Flag = ""; }
                if (OldVal.Flag != NewVal.Flag) { different = true; }
                return different;
            }

            // Returns object containing the new values different from the old values in object comparison
            public localDrives localDrives_Diff(localDrives OldVal, localDrives NewVal)
            {
                localDrives diff = new localDrives();
                if (OldVal.ID != NewVal.ID) { diff.ID = NewVal.ID; }
                if (OldVal.Category != NewVal.Category) { diff.Category = NewVal.Category; }
                if (OldVal.DirPath != NewVal.DirPath) { diff.DirPath = NewVal.DirPath; }
                if (OldVal.Flag != NewVal.Flag) { diff.Flag = NewVal.Flag; }
                return diff;
            }

            // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
            public List<string> localDrives_DiffList(localDrives OldVal, localDrives NewVal)
            {
                List<string> diffList = new List<string>();
                if (OldVal.ID != NewVal.ID) { diffList.Add("Changed ID Value From " + OldVal.ID + " To " + NewVal.ID); }
                if (OldVal.Category != NewVal.Category) { diffList.Add("Changed Category Value From " + OldVal.Category + " To " + NewVal.Category); }
                if (OldVal.DirPath != NewVal.DirPath) { diffList.Add("Changed DirPath Value From " + OldVal.DirPath + " To " + NewVal.DirPath); }
                if (OldVal.Flag != NewVal.Flag) { diffList.Add("Changed Flag Value From " + OldVal.Flag + " To " + NewVal.Flag); }
                return diffList;
            }

            // Generate XML String From Object
            public string localDrives_ToXML(localDrives obj)
            {
                string XMLString = "";
                XMLString = XMLString + "<ID>" + obj.ID + "</ID>";
                XMLString = XMLString + "<Category>" + obj.Category + "</Category>";
                XMLString = XMLString + "<DirPath>" + obj.DirPath + "</DirPath>";
                XMLString = XMLString + "<Flag>" + obj.Flag + "</Flag>";
                return XMLString;
            }

            // Populate Object from XML Tag String
            public localDrives localDrives_FromXML(string XMLString)
            {
                _Parse prs = new _Parse();
                localDrives obj = new localDrives();
                obj.ID = prs.XML_Text(XMLString, "<ID>");
                obj.Category = prs.XML_Text(XMLString, "<Category>");
                obj.DirPath = prs.XML_Text(XMLString, "<DirPath>");
                obj.Flag = prs.XML_Text(XMLString, "<Flag>");
                return obj;
            }


            #endregion
            public bool Create_Table_LocalDrives(string DbFile)
            {
                SQLite sqlite = new SQLite();
                _AHK ahk = new _AHK();

                string CreateLine = "Create Table [LocalDrives] (ID INTEGER PRIMARY KEY, Category VARCHAR, DirPath VARCHAR, Flag VARCHAR)";
                bool TableCreated = sqlite.Table_Exists(DbFile, "LocalDrives");
                if (!TableCreated) { TableCreated = sqlite.Table_New(DbFile, "LocalDrives", "Create Table [LocalDrives] (ID INTEGER PRIMARY KEY, Category VARCHAR, DirPath VARCHAR, Flag VARCHAR", false); }


                if (!TableCreated) { ahk.MsgBox("[LocalDrives] Created = " + TableCreated.ToString()); }
                return TableCreated;
            }

            #region ===== localDrives SQLite : Return =====

            public localDrives Return_Object_From_LocalDrives(string WhereClause = "[ID] = ''", string DbFile = "")
            {
                SQLite sqlite = new SQLite();
                _AHK ahk = new _AHK();

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\localDrives.sqlite"; }
                string SelectLine = "Select [ID], [Category], [DirPath], [Flag] From [LocalDrives] ";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
                localDrives returnObject = new localDrives();
                int i = 0;
                string Value = "";
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.Category = ret["Category"].ToString();
                        returnObject.DirPath = ret["DirPath"].ToString();
                        returnObject.Flag = ret["Flag"].ToString();
                    }
                }

                return returnObject;
            }

            public List<localDrives> Return_localDrives_List(string WhereClause = "", string DbFile = "", string TableName = "[LocalDrives]")
            {
                SQLite sqlite = new SQLite();
                _AHK ahk = new _AHK();

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\localDrives.sqlite"; }
                string SelectLine = "Select * From " + TableName;

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

                List<localDrives> ReturnList = new List<localDrives>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        localDrives returnObject = new localDrives();

                        returnObject.ID = ret["ID"].ToString();
                        returnObject.Category = ret["Category"].ToString();
                        returnObject.DirPath = ret["DirPath"].ToString();
                        returnObject.Flag = ret["Flag"].ToString();

                        ReturnList.Add(returnObject);
                    }
                }

                return ReturnList;
            }

            public DataTable Return_DataTable_From_LocalDrives(string DbFile)
            {
                SQLite sqlite = new SQLite();

                string SelectLine = "Select [ID], [Category], [DirPath], [Flag] From [LocalDrives]";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                return ReturnTable;
            }


            #endregion
            #region ===== localDrives SQLite : Update Insert =====

            public bool localDrives_Insert(localDrives inObject, string DbFile = "")
            {
                SQLite sqlite = new SQLite();
                _AHK ahk = new _AHK();

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\localDrives.sqlite"; }
                string InsertLine = "Insert Into [LocalDrives] (ID, Category, DirPath, Flag) values ('" + inObject.ID + "', '" + inObject.Category + "', '" + inObject.DirPath + "', '" + inObject.Flag + "')";
                bool Inserted = sqlite.Execute(DbFile, InsertLine);
                if (!Inserted) { ahk.MsgBox("Inserted Into [LocalDrives] = " + Inserted.ToString()); }
                return Inserted;
            }

            public bool localDrives_Update(localDrives inObject, string DbFile = "")
            {
                SQLite sqlite = new SQLite();
                _AHK ahk = new _AHK();

                //string UpdateLine = "Update [LocalDrives] set ID = '" + inObject.ID + "', Category = '" + inObject.Category + "', DirPath = '" + inObject.DirPath + "', Flag = '" + inObject.Flag + "' WHERE [Item] = 'Value' ";
                string UpdateLine = "Update [LocalDrives] set ";


                if (inObject.ID != null) { UpdateLine = UpdateLine + "[ID] = '" + inObject.ID + "',"; }
                if (inObject.Category != null) { UpdateLine = UpdateLine + "[Category] = '" + inObject.Category + "',"; }
                if (inObject.DirPath != null) { UpdateLine = UpdateLine + "[DirPath] = '" + inObject.DirPath + "',"; }
                if (inObject.Flag != null) { UpdateLine = UpdateLine + "[Flag] = '" + inObject.Flag + "',"; }

                UpdateLine = ahk.TrimLast(UpdateLine, 1);
                UpdateLine = UpdateLine + " WHERE [ID] = ' '"; // DEFINE CONDITION HERE !!!

                bool Updated = sqlite.Execute(DbFile, UpdateLine);
                return Updated;
            }

            public bool localDrives_UpdateInsert(localDrives obj, string DbFile = "")
            {
                _AHK ahk = new _AHK();

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\localDrives.sqlite"; }
                if (!File.Exists(DbFile)) { Create_Table_LocalDrives(DbFile); }

                bool Updated = localDrives_Update(obj, DbFile);  // try to update record first
                if (!Updated) { Updated = localDrives_Insert(obj, DbFile); }  // if unable to update, insert new record
                return Updated;
            }


            #endregion
            #region ===== localDrives DataTable =====

            public DataTable Return_localDrives_DataTable(string DbFile = "", string TableName = "localDrives", string WhereClause = "", bool Debug = false)
            {
                SQLite sqlite = new SQLite();
                _AHK ahk = new _AHK();

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\localDrives.sqlite"; }
                string SelectLine = "Select * From [LocalDrives]";

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }

                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


                DataTable table = new DataTable();
                table.Columns.Add("ID", typeof(string));
                table.Columns.Add("Category", typeof(string));
                table.Columns.Add("DirPath", typeof(string));
                table.Columns.Add("Flag", typeof(string));

                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        localDrives returnObject = new localDrives();

                        returnObject.ID = ret["ID"].ToString();
                        returnObject.Category = ret["Category"].ToString();
                        returnObject.DirPath = ret["DirPath"].ToString();
                        returnObject.Flag = ret["Flag"].ToString();

                        table.Rows.Add(returnObject.ID, returnObject.Category, returnObject.DirPath, returnObject.Flag);
                    }
                }

                return table;
            }

            public DataTable Create_LocalDrives_DataTable(localDrives inObject)
            {
                DataTable table = new DataTable();
                table.Columns.Add("ID", typeof(string));
                table.Columns.Add("Category", typeof(string));
                table.Columns.Add("DirPath", typeof(string));
                table.Columns.Add("Flag", typeof(string));

                table.Rows.Add(inObject.ID, inObject.Category, inObject.DirPath, inObject.Flag);
                return table;
            }


            #endregion
            #region ===== localDrives DataGridView =====

            public void HideShow_LocalDrives_Columns(DataGridView dv)
            {

                try { dv.Columns["ID"].Visible = true; } catch { }
                try { dv.Columns["Category"].Visible = true; } catch { }
                try { dv.Columns["DirPath"].Visible = true; } catch { }
                try { dv.Columns["Flag"].Visible = true; } catch { }
            }
            public void Enable_LocalDrives_Columns(DataGridView dv)
            {

                try { dv.Columns["ID"].ReadOnly = true; } catch { }
                try { dv.Columns["Category"].ReadOnly = true; } catch { }
                try { dv.Columns["DirPath"].ReadOnly = true; } catch { }
                try { dv.Columns["Flag"].ReadOnly = true; } catch { }
            }

            #endregion
            #region ===== localDrives SQL Functions =====

            // Return localDrives SQL Connection String
            public SqlConnection localDrives_Conn()
            {
                // populate sql connection
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLserver"].ConnectionString);
                // SqlConnection Conn = new SqlConnection("Server=188.168.188.88;DataBase=LucidMedia;Uid=lucidm;Pwd=pass");
                return conn;
            }

            // Return localDrives TableName (Full Path)
            public string localDrives_TableName()
            {
                // populate to return full sql table name
                return "[LucidMedia].[dbo].[LocalDrives]";
            }

            // Generate SQL Table
            public bool localDrives_CreateSQLTable()
            {
                SqlConnection Conn = localDrives_Conn();
                string CreateTableLine = "CREATE TABLE [LocalDrives](";
                CreateTableLine = CreateTableLine + "[ID] [int] IDENTITY(1,1) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Category] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[DirPath] [varchar](max) NOT NULL,";
                CreateTableLine = CreateTableLine + "[Flag] [varchar](max) NOT NULL";
                CreateTableLine = CreateTableLine + ") ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]";
                return false;
            }

            public bool localDrives_InsertSQL(localDrives obj)
            {
                _AHK ahk = new _AHK();

                SqlConnection Con = localDrives_Conn();
                string SQLLine = "Insert Into " + localDrives_TableName() + " (Category, DirPath, Flag) VALUES (@Category, @DirPath, @Flag)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.Category == null) { obj.Category = ""; }
                if (obj.DirPath == null) { obj.DirPath = ""; }
                if (obj.Flag == null) { obj.Flag = ""; }
                cmd2.Parameters.AddWithValue(@"Category", obj.Category.ToString());
                cmd2.Parameters.AddWithValue(@"DirPath", obj.DirPath.ToString());
                cmd2.Parameters.AddWithValue(@"Flag", obj.Flag.ToString());
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Con.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool localDrives_UpdateSQL(localDrives obj)
            {
                _AHK ahk = new _AHK();

                SqlConnection Conn = localDrives_Conn();
                string SQLLine = "Update " + localDrives_TableName() + " SET ID = @ID, Category = @Category, DirPath = @DirPath, Flag = @Flag WHERE ID = @ID";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.ID == null) { obj.ID = ""; }
                if (obj.Category == null) { obj.Category = ""; }
                if (obj.DirPath == null) { obj.DirPath = ""; }
                if (obj.Flag == null) { obj.Flag = ""; }
                cmd2.Parameters.AddWithValue(@"ID", obj.ID.ToString());
                cmd2.Parameters.AddWithValue(@"Category", obj.Category.ToString());
                cmd2.Parameters.AddWithValue(@"DirPath", obj.DirPath.ToString());
                cmd2.Parameters.AddWithValue(@"Flag", obj.Flag.ToString());
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool localDrives_UpdateInsert(localDrives obj)
            {
                SqlConnection Conn = localDrives_Conn();
                bool Updated = localDrives_UpdateSQL(obj);  // try to update record first
                if (!Updated) { Updated = localDrives_InsertSQL(obj); }  // if unable to update, insert new record
                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool localDrives_UpdateIfPopulated(localDrives obj, string ID = "")
            {
                _AHK ahk = new _AHK();

                SqlConnection Conn = localDrives_Conn();
                string SQLcmd = "Update " + localDrives_TableName() + " SET ";
                if (obj.ID != null) { SQLcmd = SQLcmd + " ID = @ID,"; }
                if (obj.Category != null) { SQLcmd = SQLcmd + " Category = @Category,"; }
                if (obj.DirPath != null) { SQLcmd = SQLcmd + " DirPath = @DirPath,"; }
                if (obj.Flag != null) { SQLcmd = SQLcmd + " Flag = @Flag,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ID = @ID";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.ID != null) { cmd2.Parameters.AddWithValue(@"ID", obj.ID); }
                if (obj.Category != null) { cmd2.Parameters.AddWithValue(@"Category", obj.Category); }
                if (obj.DirPath != null) { cmd2.Parameters.AddWithValue(@"DirPath", obj.DirPath); }
                if (obj.Flag != null) { cmd2.Parameters.AddWithValue(@"Flag", obj.Flag); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public localDrives localDrives_ReturnSQL(string ID = "")
            {
                SQL sql = new SQL();
                _AHK ahk = new _AHK();

                SqlConnection Conn = localDrives_Conn();
                string SelectLine = "Select [ID],[Category],[DirPath],[Flag] From " + localDrives_TableName() + " WHERE ID = '" + ID + "'";
                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                localDrives returnObject = new localDrives();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.Category = ret["Category"].ToString();
                        returnObject.DirPath = ret["DirPath"].ToString();
                        returnObject.Flag = ret["Flag"].ToString();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public List<localDrives> localDrives_ReturnSQLList(string Command = "")
            {
                SQL sql = new SQL();

                if (Command == "") { Command = "Select * From localDrives_TableName()"; }
                SqlConnection Conn = localDrives_Conn();
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<localDrives> ReturnList = new List<localDrives>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        localDrives returnObject = new localDrives();
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.Category = ret["Category"].ToString();
                        returnObject.DirPath = ret["DirPath"].ToString();
                        returnObject.Flag = ret["Flag"].ToString();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }

            public bool localDrives_SQL_to_SQLite(string SqliteDBPath = @"\Db\localDrives.sqlite")
            {
                SQL sql = new SQL();
                _AHK ahk = new _AHK();
                SQLite sqlite = new SQLite();

                string SaveFile = SqliteDBPath;
                if (SqliteDBPath == @"\Db\localDrives.sqlite")
                {
                    ahk.FileCreateDir(ahk.AppDir() + @"\Db");
                    SaveFile = ahk.AppDir() + @"\Db\localDrives.sqlite";
                }

                //sb.StatusBar("Copying SQL Db to " + SaveFile + "...");
                sqlite.SQLTable_To_NewSQLiteTable(localDrives_Conn(), "LocalDrives", "LocalDrives", SaveFile, "", false, false, false);
                //sb.StatusBar("FINISHED Copying SQL Db to " + SaveFile);

                if (File.Exists(SaveFile)) { return true; } else { return false; }
            }


            #endregion

            #endregion

        }

    public class TVShowDb
        {
            _AHK ahk = new _AHK();
            _Database.SQL sql = new _Database.SQL();
            _Database.SQLite sqlite = new _Database.SQLite();

            public static RadProgressBar radProgress { get; set; }

            #region === SQLServer Databases ===

            #region === LITM_ShowIndex FUNCTIONS ===

            public void LITMShowIndex_SQLServer_To_Local()
            {
                _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();

                List<LITM_ShowIndex> shows = LITM_ShowIndexList(false);

                pro.SetupProgressBar(radProgress, shows.Count);

                int i = 0;
                foreach (LITM_ShowIndex show in shows)
                {
                    i++; pro.UpdateProgress(radProgress, show.ShowName + " | " + i + "/" + shows.Count);

                    // new object to null out image - done seperately
                    LITM_ShowIndex showw = show;
                    showw.ShowImage = null;

                    LITM_ShowIndex_UpdateInsert(show, true);
                }

                ahk.MsgBox("Finished Downloading LITM_ShowIndex to Local");
            }


            #region ===== LITM_ShowIndex Object =====

            public struct LITM_ShowIndex
            {
                public string ID { get; set; }
                public string ShowName { get; set; }
                public string EpGuideURL { get; set; }
                public string IMDbURL { get; set; }
                public string LocalRoot { get; set; }
                public string ShowAirStatus { get; set; }
                public string ShowDLStatus { get; set; }
                public string SeasonCount { get; set; }
                public string DateAdded { get; set; }
                public string DateModified { get; set; }
                public string DirSize { get; set; }
                public string Flag { get; set; }
                public string ShowImage { get; set; }
                public string ShowImageWide { get; set; }
                public string IMDbRating { get; set; }
                public string Category { get; set; }
                public string Description { get; set; }
                public string IMDbVotes { get; set; }

                public List<_Sites.EpGuides.litmEpisodes> EpGuide { get; set; }
            }
            //  Fix illegal characters before Sql/Sqlite Db Inserts
            public LITM_ShowIndex LITM_ShowIndex_FixChars(LITM_ShowIndex ToFix)
            {
                LITM_ShowIndex Fixed = new LITM_ShowIndex();

                Fixed.ID = ToFix.ID.Replace("'", "''");
                Fixed.ShowName = ToFix.ShowName.Replace("'", "''");
                Fixed.EpGuideURL = ToFix.EpGuideURL.Replace("'", "''");
                Fixed.IMDbURL = ToFix.IMDbURL.Replace("'", "''");
                Fixed.LocalRoot = ToFix.LocalRoot.Replace("'", "''");
                Fixed.ShowAirStatus = ToFix.ShowAirStatus.Replace("'", "''");
                Fixed.ShowDLStatus = ToFix.ShowDLStatus.Replace("'", "''");
                Fixed.SeasonCount = ToFix.SeasonCount.Replace("'", "''");
                Fixed.DateAdded = ToFix.DateAdded.Replace("'", "''");
                Fixed.DateModified = ToFix.DateModified.Replace("'", "''");
                Fixed.DirSize = ToFix.DirSize.Replace("'", "''");
                Fixed.Flag = ToFix.Flag.Replace("'", "''");
                Fixed.ShowImage = ToFix.ShowImage.Replace("'", "''");
                Fixed.ShowImageWide = ToFix.ShowImageWide.Replace("'", "''");
                Fixed.IMDbRating = ToFix.IMDbRating.Replace("'", "''");
                Fixed.Category = ToFix.Category.Replace("'", "''");
                Fixed.Description = ToFix.Description.Replace("'", "''");
                Fixed.IMDbVotes = ToFix.IMDbVotes.Replace("'", "''");

                return Fixed;
            }

            // Compare two objects to see if they have identical values
            public bool LITM_ShowIndex_Changed(LITM_ShowIndex OldVal, LITM_ShowIndex NewVal)
            {
                LITM_ShowIndex diff = new LITM_ShowIndex();
                List<string> diffList = new List<string>();
                bool different = false;
                if (OldVal.ID == null) { OldVal.ID = ""; }
                if (NewVal.ID == null) { NewVal.ID = ""; }
                if (OldVal.ID != NewVal.ID) { different = true; }
                if (OldVal.ShowName == null) { OldVal.ShowName = ""; }
                if (NewVal.ShowName == null) { NewVal.ShowName = ""; }
                if (OldVal.ShowName != NewVal.ShowName) { different = true; }
                if (OldVal.EpGuideURL == null) { OldVal.EpGuideURL = ""; }
                if (NewVal.EpGuideURL == null) { NewVal.EpGuideURL = ""; }
                if (OldVal.EpGuideURL != NewVal.EpGuideURL) { different = true; }
                if (OldVal.IMDbURL == null) { OldVal.IMDbURL = ""; }
                if (NewVal.IMDbURL == null) { NewVal.IMDbURL = ""; }
                if (OldVal.IMDbURL != NewVal.IMDbURL) { different = true; }
                if (OldVal.LocalRoot == null) { OldVal.LocalRoot = ""; }
                if (NewVal.LocalRoot == null) { NewVal.LocalRoot = ""; }
                if (OldVal.LocalRoot != NewVal.LocalRoot) { different = true; }
                if (OldVal.ShowAirStatus == null) { OldVal.ShowAirStatus = ""; }
                if (NewVal.ShowAirStatus == null) { NewVal.ShowAirStatus = ""; }
                if (OldVal.ShowAirStatus != NewVal.ShowAirStatus) { different = true; }
                if (OldVal.ShowDLStatus == null) { OldVal.ShowDLStatus = ""; }
                if (NewVal.ShowDLStatus == null) { NewVal.ShowDLStatus = ""; }
                if (OldVal.ShowDLStatus != NewVal.ShowDLStatus) { different = true; }
                if (OldVal.SeasonCount == null) { OldVal.SeasonCount = ""; }
                if (NewVal.SeasonCount == null) { NewVal.SeasonCount = ""; }
                if (OldVal.SeasonCount != NewVal.SeasonCount) { different = true; }
                if (OldVal.DateAdded == null) { OldVal.DateAdded = ""; }
                if (NewVal.DateAdded == null) { NewVal.DateAdded = ""; }
                if (OldVal.DateAdded != NewVal.DateAdded) { different = true; }
                if (OldVal.DateModified == null) { OldVal.DateModified = ""; }
                if (NewVal.DateModified == null) { NewVal.DateModified = ""; }
                if (OldVal.DateModified != NewVal.DateModified) { different = true; }
                if (OldVal.DirSize == null) { OldVal.DirSize = ""; }
                if (NewVal.DirSize == null) { NewVal.DirSize = ""; }
                if (OldVal.DirSize != NewVal.DirSize) { different = true; }
                if (OldVal.Flag == null) { OldVal.Flag = ""; }
                if (NewVal.Flag == null) { NewVal.Flag = ""; }
                if (OldVal.Flag != NewVal.Flag) { different = true; }
                if (OldVal.ShowImage == null) { OldVal.ShowImage = ""; }
                if (NewVal.ShowImage == null) { NewVal.ShowImage = ""; }
                if (OldVal.ShowImage != NewVal.ShowImage) { different = true; }
                if (OldVal.ShowImageWide == null) { OldVal.ShowImageWide = ""; }
                if (NewVal.ShowImageWide == null) { NewVal.ShowImageWide = ""; }
                if (OldVal.ShowImageWide != NewVal.ShowImageWide) { different = true; }
                if (OldVal.IMDbRating == null) { OldVal.IMDbRating = ""; }
                if (NewVal.IMDbRating == null) { NewVal.IMDbRating = ""; }
                if (OldVal.IMDbRating != NewVal.IMDbRating) { different = true; }
                if (OldVal.Category == null) { OldVal.Category = ""; }
                if (NewVal.Category == null) { NewVal.Category = ""; }
                if (OldVal.Category != NewVal.Category) { different = true; }
                if (OldVal.Description == null) { OldVal.Description = ""; }
                if (NewVal.Description == null) { NewVal.Description = ""; }
                if (OldVal.Description != NewVal.Description) { different = true; }
                if (OldVal.IMDbVotes == null) { OldVal.IMDbVotes = ""; }
                if (NewVal.IMDbVotes == null) { NewVal.IMDbVotes = ""; }
                if (OldVal.IMDbVotes != NewVal.IMDbVotes) { different = true; }
                return different;
            }

            // Returns object containing the new values different from the old values in object comparison
            public LITM_ShowIndex LITM_ShowIndex_Diff(LITM_ShowIndex OldVal, LITM_ShowIndex NewVal)
            {
                LITM_ShowIndex diff = new LITM_ShowIndex();
                if (OldVal.ID != NewVal.ID) { diff.ID = NewVal.ID; }
                if (OldVal.ShowName != NewVal.ShowName) { diff.ShowName = NewVal.ShowName; }
                if (OldVal.EpGuideURL != NewVal.EpGuideURL) { diff.EpGuideURL = NewVal.EpGuideURL; }
                if (OldVal.IMDbURL != NewVal.IMDbURL) { diff.IMDbURL = NewVal.IMDbURL; }
                if (OldVal.LocalRoot != NewVal.LocalRoot) { diff.LocalRoot = NewVal.LocalRoot; }
                if (OldVal.ShowAirStatus != NewVal.ShowAirStatus) { diff.ShowAirStatus = NewVal.ShowAirStatus; }
                if (OldVal.ShowDLStatus != NewVal.ShowDLStatus) { diff.ShowDLStatus = NewVal.ShowDLStatus; }
                if (OldVal.SeasonCount != NewVal.SeasonCount) { diff.SeasonCount = NewVal.SeasonCount; }
                if (OldVal.DateAdded != NewVal.DateAdded) { diff.DateAdded = NewVal.DateAdded; }
                if (OldVal.DateModified != NewVal.DateModified) { diff.DateModified = NewVal.DateModified; }
                if (OldVal.DirSize != NewVal.DirSize) { diff.DirSize = NewVal.DirSize; }
                if (OldVal.Flag != NewVal.Flag) { diff.Flag = NewVal.Flag; }
                if (OldVal.ShowImage != NewVal.ShowImage) { diff.ShowImage = NewVal.ShowImage; }
                if (OldVal.ShowImageWide != NewVal.ShowImageWide) { diff.ShowImageWide = NewVal.ShowImageWide; }
                if (OldVal.IMDbRating != NewVal.IMDbRating) { diff.IMDbRating = NewVal.IMDbRating; }
                if (OldVal.Category != NewVal.Category) { diff.Category = NewVal.Category; }
                if (OldVal.Description != NewVal.Description) { diff.Description = NewVal.Description; }
                if (OldVal.IMDbVotes != NewVal.IMDbVotes) { diff.IMDbVotes = NewVal.IMDbVotes; }
                return diff;
            }

            // Returns list of strings with the previous/new values after comparing 2 objects. Used for change log
            public List<string> LITM_ShowIndex_DiffList(LITM_ShowIndex OldVal, LITM_ShowIndex NewVal)
            {
                List<string> diffList = new List<string>();
                if (OldVal.ID != NewVal.ID) { diffList.Add("Changed ID Value From " + OldVal.ID + " To " + NewVal.ID); }
                if (OldVal.ShowName != NewVal.ShowName) { diffList.Add("Changed ShowName Value From " + OldVal.ShowName + " To " + NewVal.ShowName); }
                if (OldVal.EpGuideURL != NewVal.EpGuideURL) { diffList.Add("Changed EpGuideURL Value From " + OldVal.EpGuideURL + " To " + NewVal.EpGuideURL); }
                if (OldVal.IMDbURL != NewVal.IMDbURL) { diffList.Add("Changed IMDbURL Value From " + OldVal.IMDbURL + " To " + NewVal.IMDbURL); }
                if (OldVal.LocalRoot != NewVal.LocalRoot) { diffList.Add("Changed LocalRoot Value From " + OldVal.LocalRoot + " To " + NewVal.LocalRoot); }
                if (OldVal.ShowAirStatus != NewVal.ShowAirStatus) { diffList.Add("Changed ShowAirStatus Value From " + OldVal.ShowAirStatus + " To " + NewVal.ShowAirStatus); }
                if (OldVal.ShowDLStatus != NewVal.ShowDLStatus) { diffList.Add("Changed ShowDLStatus Value From " + OldVal.ShowDLStatus + " To " + NewVal.ShowDLStatus); }
                if (OldVal.SeasonCount != NewVal.SeasonCount) { diffList.Add("Changed SeasonCount Value From " + OldVal.SeasonCount + " To " + NewVal.SeasonCount); }
                if (OldVal.DateAdded != NewVal.DateAdded) { diffList.Add("Changed DateAdded Value From " + OldVal.DateAdded + " To " + NewVal.DateAdded); }
                if (OldVal.DateModified != NewVal.DateModified) { diffList.Add("Changed DateModified Value From " + OldVal.DateModified + " To " + NewVal.DateModified); }
                if (OldVal.DirSize != NewVal.DirSize) { diffList.Add("Changed DirSize Value From " + OldVal.DirSize + " To " + NewVal.DirSize); }
                if (OldVal.Flag != NewVal.Flag) { diffList.Add("Changed Flag Value From " + OldVal.Flag + " To " + NewVal.Flag); }
                if (OldVal.ShowImage != NewVal.ShowImage) { diffList.Add("Changed ShowImage Value From " + OldVal.ShowImage + " To " + NewVal.ShowImage); }
                if (OldVal.ShowImageWide != NewVal.ShowImageWide) { diffList.Add("Changed ShowImageWide Value From " + OldVal.ShowImageWide + " To " + NewVal.ShowImageWide); }
                if (OldVal.IMDbRating != NewVal.IMDbRating) { diffList.Add("Changed IMDbRating Value From " + OldVal.IMDbRating + " To " + NewVal.IMDbRating); }
                if (OldVal.Category != NewVal.Category) { diffList.Add("Changed Category Value From " + OldVal.Category + " To " + NewVal.Category); }
                if (OldVal.Description != NewVal.Description) { diffList.Add("Changed Description Value From " + OldVal.Description + " To " + NewVal.Description); }
                if (OldVal.IMDbVotes != NewVal.IMDbVotes) { diffList.Add("Changed IMDbVotes Value From " + OldVal.IMDbVotes + " To " + NewVal.IMDbVotes); }
                return diffList;
            }

            // Generate XML String From Object
            public string LITM_ShowIndex_ToXML(LITM_ShowIndex obj)
            {
                string XMLString = "";
                XMLString = XMLString + "<ID>" + obj.ID + "</ID>";
                XMLString = XMLString + "<ShowName>" + obj.ShowName + "</ShowName>";
                XMLString = XMLString + "<EpGuideURL>" + obj.EpGuideURL + "</EpGuideURL>";
                XMLString = XMLString + "<IMDbURL>" + obj.IMDbURL + "</IMDbURL>";
                XMLString = XMLString + "<LocalRoot>" + obj.LocalRoot + "</LocalRoot>";
                XMLString = XMLString + "<ShowAirStatus>" + obj.ShowAirStatus + "</ShowAirStatus>";
                XMLString = XMLString + "<ShowDLStatus>" + obj.ShowDLStatus + "</ShowDLStatus>";
                XMLString = XMLString + "<SeasonCount>" + obj.SeasonCount + "</SeasonCount>";
                XMLString = XMLString + "<DateAdded>" + obj.DateAdded + "</DateAdded>";
                XMLString = XMLString + "<DateModified>" + obj.DateModified + "</DateModified>";
                XMLString = XMLString + "<DirSize>" + obj.DirSize + "</DirSize>";
                XMLString = XMLString + "<Flag>" + obj.Flag + "</Flag>";
                XMLString = XMLString + "<ShowImage>" + obj.ShowImage + "</ShowImage>";
                XMLString = XMLString + "<ShowImageWide>" + obj.ShowImageWide + "</ShowImageWide>";
                XMLString = XMLString + "<IMDbRating>" + obj.IMDbRating + "</IMDbRating>";
                XMLString = XMLString + "<Category>" + obj.Category + "</Category>";
                XMLString = XMLString + "<Description>" + obj.Description + "</Description>";
                XMLString = XMLString + "<IMDbVotes>" + obj.IMDbVotes + "</IMDbVotes>";
                return XMLString;
            }

            // Populate Object from XML Tag String
            public LITM_ShowIndex LITM_ShowIndex_FromXML(string XMLString)
            {
                _Parse prs = new _Parse();

                LITM_ShowIndex obj = new LITM_ShowIndex();
                obj.ID = prs.XML_Text(XMLString, "<ID>");
                obj.ShowName = prs.XML_Text(XMLString, "<ShowName>");
                obj.EpGuideURL = prs.XML_Text(XMLString, "<EpGuideURL>");
                obj.IMDbURL = prs.XML_Text(XMLString, "<IMDbURL>");
                obj.LocalRoot = prs.XML_Text(XMLString, "<LocalRoot>");
                obj.ShowAirStatus = prs.XML_Text(XMLString, "<ShowAirStatus>");
                obj.ShowDLStatus = prs.XML_Text(XMLString, "<ShowDLStatus>");
                obj.SeasonCount = prs.XML_Text(XMLString, "<SeasonCount>");
                obj.DateAdded = prs.XML_Text(XMLString, "<DateAdded>");
                obj.DateModified = prs.XML_Text(XMLString, "<DateModified>");
                obj.DirSize = prs.XML_Text(XMLString, "<DirSize>");
                obj.Flag = prs.XML_Text(XMLString, "<Flag>");
                obj.ShowImage = prs.XML_Text(XMLString, "<ShowImage>");
                obj.ShowImageWide = prs.XML_Text(XMLString, "<ShowImageWide>");
                obj.IMDbRating = prs.XML_Text(XMLString, "<IMDbRating>");
                obj.Category = prs.XML_Text(XMLString, "<Category>");
                obj.Description = prs.XML_Text(XMLString, "<Description>");
                obj.IMDbVotes = prs.XML_Text(XMLString, "<IMDbVotes>");
                return obj;
            }


            #endregion
            public bool Create_Table_LITM_ShowIndex(string DbFile)
            {
                string CreateLine = "Create Table [LITM_ShowIndex] (ID INTEGER PRIMARY KEY, ShowName VARCHAR, EpGuideURL VARCHAR, IMDbURL VARCHAR, LocalRoot VARCHAR, ShowAirStatus VARCHAR, ShowDLStatus VARCHAR, SeasonCount VARCHAR, DateAdded VARCHAR, DateModified VARCHAR, DirSize VARCHAR, Flag VARCHAR, ShowImage VARCHAR, ShowImageWide VARCHAR, IMDbRating VARCHAR, Category VARCHAR, Description VARCHAR, IMDbVotes VARCHAR)";
                bool TableCreated = sqlite.Table_Exists(DbFile, "LITM_ShowIndex");
                if (!TableCreated) { TableCreated = sqlite.Table_New(DbFile, "LITM_ShowIndex", "Create Table [LITM_ShowIndex] (ID INTEGER PRIMARY KEY, ShowName VARCHAR, EpGuideURL VARCHAR, IMDbURL VARCHAR, LocalRoot VARCHAR, ShowAirStatus VARCHAR, ShowDLStatus VARCHAR, SeasonCount VARCHAR, DateAdded VARCHAR, DateModified VARCHAR, DirSize VARCHAR, Flag VARCHAR, ShowImage VARCHAR, ShowImageWide VARCHAR, IMDbRating VARCHAR, Category VARCHAR, Description VARCHAR, IMDbVotes VARCHAR", false); }


                if (!TableCreated) { ahk.MsgBox("[LITM_ShowIndex] Created = " + TableCreated.ToString()); }
                return TableCreated;
            }

            #region ===== LITM_ShowIndex SQLite : Return =====

            public LITM_ShowIndex Return_Object_From_LITM_ShowIndex(string WhereClause = "[ID] = ''", string DbFile = "")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\LITM_ShowIndex.sqlite"; }
                string SelectLine = "Select [ID], [ShowName], [EpGuideURL], [IMDbURL], [LocalRoot], [ShowAirStatus], [ShowDLStatus], [SeasonCount], [DateAdded], [DateModified], [DirSize], [Flag], [ShowImage], [ShowImageWide], [IMDbRating], [Category], [Description], [IMDbVotes] From [LITM_ShowIndex] ";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                if (WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + " " + WhereClause; }
                if (!WhereClause.ToUpper().Contains("WHERE ")) { SelectLine = SelectLine + "WHERE " + WhereClause; }
                LITM_ShowIndex returnObject = new LITM_ShowIndex();
                int i = 0;
                string Value = "";
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.EpGuideURL = ret["EpGuideURL"].ToString();
                        returnObject.IMDbURL = ret["IMDbURL"].ToString();
                        returnObject.LocalRoot = ret["LocalRoot"].ToString();
                        returnObject.ShowAirStatus = ret["ShowAirStatus"].ToString();
                        returnObject.ShowDLStatus = ret["ShowDLStatus"].ToString();
                        returnObject.SeasonCount = ret["SeasonCount"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.DateModified = ret["DateModified"].ToString();
                        returnObject.DirSize = ret["DirSize"].ToString();
                        returnObject.Flag = ret["Flag"].ToString();
                        returnObject.ShowImage = ret["ShowImage"].ToString();
                        returnObject.ShowImageWide = ret["ShowImageWide"].ToString();
                        returnObject.IMDbRating = ret["IMDbRating"].ToString();
                        returnObject.Category = ret["Category"].ToString();
                        returnObject.Description = ret["Description"].ToString();
                        returnObject.IMDbVotes = ret["IMDbVotes"].ToString();
                    }
                }

                return returnObject;
            }

            public List<LITM_ShowIndex> Return_LITM_ShowIndex_List(string WhereClause = "", string DbFile = "", string TableName = "[LITM_ShowIndex]")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\LITM_ShowIndex.sqlite"; }
                string SelectLine = "Select * From " + TableName;

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);

                List<LITM_ShowIndex> ReturnList = new List<LITM_ShowIndex>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        LITM_ShowIndex returnObject = new LITM_ShowIndex();

                        returnObject.ID = ret["ID"].ToString();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.EpGuideURL = ret["EpGuideURL"].ToString();
                        returnObject.IMDbURL = ret["IMDbURL"].ToString();
                        returnObject.LocalRoot = ret["LocalRoot"].ToString();
                        returnObject.ShowAirStatus = ret["ShowAirStatus"].ToString();
                        returnObject.ShowDLStatus = ret["ShowDLStatus"].ToString();
                        returnObject.SeasonCount = ret["SeasonCount"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.DateModified = ret["DateModified"].ToString();
                        returnObject.DirSize = ret["DirSize"].ToString();
                        returnObject.Flag = ret["Flag"].ToString();
                        returnObject.ShowImage = ret["ShowImage"].ToString();
                        returnObject.ShowImageWide = ret["ShowImageWide"].ToString();
                        returnObject.IMDbRating = ret["IMDbRating"].ToString();
                        returnObject.Category = ret["Category"].ToString();
                        returnObject.Description = ret["Description"].ToString();
                        returnObject.IMDbVotes = ret["IMDbVotes"].ToString();

                        ReturnList.Add(returnObject);
                    }
                }

                return ReturnList;
            }

            public DataTable Return_DataTable_From_LITM_ShowIndex(string DbFile)
            {
                string SelectLine = "Select [ID], [ShowName], [EpGuideURL], [IMDbURL], [LocalRoot], [ShowAirStatus], [ShowDLStatus], [SeasonCount], [DateAdded], [DateModified], [DirSize], [Flag], [ShowImage], [ShowImageWide], [IMDbRating], [Category], [Description], [IMDbVotes] From [LITM_ShowIndex]";
                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);
                return ReturnTable;
            }


            #endregion
            #region ===== LITM_ShowIndex SQLite : Update Insert =====

            public bool LITM_ShowIndex_Insert(LITM_ShowIndex inObject, string DbFile = "")
            {
                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\LITM_ShowIndex.sqlite"; }
                string InsertLine = "Insert Into [LITM_ShowIndex] (ID, ShowName, EpGuideURL, IMDbURL, LocalRoot, ShowAirStatus, ShowDLStatus, SeasonCount, DateAdded, DateModified, DirSize, Flag, ShowImage, ShowImageWide, IMDbRating, Category, Description, IMDbVotes) values ('" + inObject.ID + "', '" + inObject.ShowName + "', '" + inObject.EpGuideURL + "', '" + inObject.IMDbURL + "', '" + inObject.LocalRoot + "', '" + inObject.ShowAirStatus + "', '" + inObject.ShowDLStatus + "', '" + inObject.SeasonCount + "', '" + inObject.DateAdded + "', '" + inObject.DateModified + "', '" + inObject.DirSize + "', '" + inObject.Flag + "', '" + inObject.ShowImage + "', '" + inObject.ShowImageWide + "', '" + inObject.IMDbRating + "', '" + inObject.Category + "', '" + inObject.Description + "', '" + inObject.IMDbVotes + "')";
                bool Inserted = sqlite.Execute(DbFile, InsertLine);
                if (!Inserted) { ahk.MsgBox("Inserted Into [LITM_ShowIndex] = " + Inserted.ToString()); }
                return Inserted;
            }

            public bool LITM_ShowIndex_Update(LITM_ShowIndex inObject, string DbFile = "")
            {
                //string UpdateLine = "Update [LITM_ShowIndex] set ID = '" + inObject.ID + "', ShowName = '" + inObject.ShowName + "', EpGuideURL = '" + inObject.EpGuideURL + "', IMDbURL = '" + inObject.IMDbURL + "', LocalRoot = '" + inObject.LocalRoot + "', ShowAirStatus = '" + inObject.ShowAirStatus + "', ShowDLStatus = '" + inObject.ShowDLStatus + "', SeasonCount = '" + inObject.SeasonCount + "', DateAdded = '" + inObject.DateAdded + "', DateModified = '" + inObject.DateModified + "', DirSize = '" + inObject.DirSize + "', Flag = '" + inObject.Flag + "', ShowImage = '" + inObject.ShowImage + "', ShowImageWide = '" + inObject.ShowImageWide + "', IMDbRating = '" + inObject.IMDbRating + "', Category = '" + inObject.Category + "', Description = '" + inObject.Description + "', IMDbVotes = '" + inObject.IMDbVotes + "' WHERE [Item] = 'Value' ";
                string UpdateLine = "Update [LITM_ShowIndex] set ";


                if (inObject.ID != null) { UpdateLine = UpdateLine + "[ID] = '" + inObject.ID + "',"; }
                if (inObject.ShowName != null) { UpdateLine = UpdateLine + "[ShowName] = '" + inObject.ShowName + "',"; }
                if (inObject.EpGuideURL != null) { UpdateLine = UpdateLine + "[EpGuideURL] = '" + inObject.EpGuideURL + "',"; }
                if (inObject.IMDbURL != null) { UpdateLine = UpdateLine + "[IMDbURL] = '" + inObject.IMDbURL + "',"; }
                if (inObject.LocalRoot != null) { UpdateLine = UpdateLine + "[LocalRoot] = '" + inObject.LocalRoot + "',"; }
                if (inObject.ShowAirStatus != null) { UpdateLine = UpdateLine + "[ShowAirStatus] = '" + inObject.ShowAirStatus + "',"; }
                if (inObject.ShowDLStatus != null) { UpdateLine = UpdateLine + "[ShowDLStatus] = '" + inObject.ShowDLStatus + "',"; }
                if (inObject.SeasonCount != null) { UpdateLine = UpdateLine + "[SeasonCount] = '" + inObject.SeasonCount + "',"; }
                if (inObject.DateAdded != null) { UpdateLine = UpdateLine + "[DateAdded] = '" + inObject.DateAdded + "',"; }
                if (inObject.DateModified != null) { UpdateLine = UpdateLine + "[DateModified] = '" + inObject.DateModified + "',"; }
                if (inObject.DirSize != null) { UpdateLine = UpdateLine + "[DirSize] = '" + inObject.DirSize + "',"; }
                if (inObject.Flag != null) { UpdateLine = UpdateLine + "[Flag] = '" + inObject.Flag + "',"; }
                if (inObject.ShowImage != null) { UpdateLine = UpdateLine + "[ShowImage] = '" + inObject.ShowImage + "',"; }
                if (inObject.ShowImageWide != null) { UpdateLine = UpdateLine + "[ShowImageWide] = '" + inObject.ShowImageWide + "',"; }
                if (inObject.IMDbRating != null) { UpdateLine = UpdateLine + "[IMDbRating] = '" + inObject.IMDbRating + "',"; }
                if (inObject.Category != null) { UpdateLine = UpdateLine + "[Category] = '" + inObject.Category + "',"; }
                if (inObject.Description != null) { UpdateLine = UpdateLine + "[Description] = '" + inObject.Description + "',"; }
                if (inObject.IMDbVotes != null) { UpdateLine = UpdateLine + "[IMDbVotes] = '" + inObject.IMDbVotes + "',"; }

                UpdateLine = ahk.TrimLast(UpdateLine, 1);
                UpdateLine = UpdateLine + " WHERE [ID] = ' '"; // DEFINE CONDITION HERE !!!

                bool Updated = sqlite.Execute(DbFile, UpdateLine);
                return Updated;
            }

            public bool LITM_ShowIndex_UpdateInsert(LITM_ShowIndex obj, string DbFile = "")
            {

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\LITM_ShowIndex.sqlite"; }
                if (!File.Exists(DbFile)) { Create_Table_LITM_ShowIndex(DbFile); }

                bool Updated = LITM_ShowIndex_Update(obj, DbFile);  // try to update record first
                if (!Updated) { Updated = LITM_ShowIndex_Insert(obj, DbFile); }  // if unable to update, insert new record
                return Updated;
            }


            #endregion
            #region ===== LITM_ShowIndex DataTable =====

            public DataTable Return_LITM_ShowIndex_DataTable(string DbFile = "", string TableName = "LITM_ShowIndex", string WhereClause = "", bool Debug = false)
            {

                if (DbFile == "") { DbFile = ahk.AppDir() + @"\Db\LITM_ShowIndex.sqlite"; }
                string SelectLine = "Select * From [LITM_ShowIndex]";

                if (WhereClause != "")
                {
                    if (WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " " + WhereClause; }
                    if (!WhereClause.ToUpper().Contains("WHERE")) { SelectLine = SelectLine + " WHERE " + WhereClause; }
                }

                DataTable ReturnTable = sqlite.GetDataTable(DbFile, SelectLine);


                DataTable table = new DataTable();
                table.Columns.Add("ID", typeof(string));
                table.Columns.Add("ShowName", typeof(string));
                table.Columns.Add("EpGuideURL", typeof(string));
                table.Columns.Add("IMDbURL", typeof(string));
                table.Columns.Add("LocalRoot", typeof(string));
                table.Columns.Add("ShowAirStatus", typeof(string));
                table.Columns.Add("ShowDLStatus", typeof(string));
                table.Columns.Add("SeasonCount", typeof(string));
                table.Columns.Add("DateAdded", typeof(string));
                table.Columns.Add("DateModified", typeof(string));
                table.Columns.Add("DirSize", typeof(string));
                table.Columns.Add("Flag", typeof(string));
                table.Columns.Add("ShowImage", typeof(string));
                table.Columns.Add("ShowImageWide", typeof(string));
                table.Columns.Add("IMDbRating", typeof(string));
                table.Columns.Add("Category", typeof(string));
                table.Columns.Add("Description", typeof(string));
                table.Columns.Add("IMDbVotes", typeof(string));

                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        LITM_ShowIndex returnObject = new LITM_ShowIndex();

                        returnObject.ID = ret["ID"].ToString();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.EpGuideURL = ret["EpGuideURL"].ToString();
                        returnObject.IMDbURL = ret["IMDbURL"].ToString();
                        returnObject.LocalRoot = ret["LocalRoot"].ToString();
                        returnObject.ShowAirStatus = ret["ShowAirStatus"].ToString();
                        returnObject.ShowDLStatus = ret["ShowDLStatus"].ToString();
                        returnObject.SeasonCount = ret["SeasonCount"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.DateModified = ret["DateModified"].ToString();
                        returnObject.DirSize = ret["DirSize"].ToString();
                        returnObject.Flag = ret["Flag"].ToString();
                        returnObject.ShowImage = ret["ShowImage"].ToString();
                        returnObject.ShowImageWide = ret["ShowImageWide"].ToString();
                        returnObject.IMDbRating = ret["IMDbRating"].ToString();
                        returnObject.Category = ret["Category"].ToString();
                        returnObject.Description = ret["Description"].ToString();
                        returnObject.IMDbVotes = ret["IMDbVotes"].ToString();

                        table.Rows.Add(returnObject.ID, returnObject.ShowName, returnObject.EpGuideURL, returnObject.IMDbURL, returnObject.LocalRoot, returnObject.ShowAirStatus, returnObject.ShowDLStatus, returnObject.SeasonCount, returnObject.DateAdded, returnObject.DateModified, returnObject.DirSize, returnObject.Flag, returnObject.ShowImage, returnObject.ShowImageWide, returnObject.IMDbRating, returnObject.Category, returnObject.Description, returnObject.IMDbVotes);
                    }
                }

                return table;
            }

            public DataTable Create_LITM_ShowIndex_DataTable(LITM_ShowIndex inObject)
            {
                DataTable table = new DataTable();
                table.Columns.Add("ID", typeof(string));
                table.Columns.Add("ShowName", typeof(string));
                table.Columns.Add("EpGuideURL", typeof(string));
                table.Columns.Add("IMDbURL", typeof(string));
                table.Columns.Add("LocalRoot", typeof(string));
                table.Columns.Add("ShowAirStatus", typeof(string));
                table.Columns.Add("ShowDLStatus", typeof(string));
                table.Columns.Add("SeasonCount", typeof(string));
                table.Columns.Add("DateAdded", typeof(string));
                table.Columns.Add("DateModified", typeof(string));
                table.Columns.Add("DirSize", typeof(string));
                table.Columns.Add("Flag", typeof(string));
                table.Columns.Add("ShowImage", typeof(string));
                table.Columns.Add("ShowImageWide", typeof(string));
                table.Columns.Add("IMDbRating", typeof(string));
                table.Columns.Add("Category", typeof(string));
                table.Columns.Add("Description", typeof(string));
                table.Columns.Add("IMDbVotes", typeof(string));

                table.Rows.Add(inObject.ID, inObject.ShowName, inObject.EpGuideURL, inObject.IMDbURL, inObject.LocalRoot, inObject.ShowAirStatus, inObject.ShowDLStatus, inObject.SeasonCount, inObject.DateAdded, inObject.DateModified, inObject.DirSize, inObject.Flag, inObject.ShowImage, inObject.ShowImageWide, inObject.IMDbRating, inObject.Category, inObject.Description, inObject.IMDbVotes);
                return table;
            }


            #endregion
            #region ===== LITM_ShowIndex DataGridView =====

            public void HideShow_LITM_ShowIndex_Columns(DataGridView dv)
            {

                try { dv.Columns["ID"].Visible = true; } catch { }
                try { dv.Columns["ShowName"].Visible = true; } catch { }
                try { dv.Columns["EpGuideURL"].Visible = true; } catch { }
                try { dv.Columns["IMDbURL"].Visible = true; } catch { }
                try { dv.Columns["LocalRoot"].Visible = true; } catch { }
                try { dv.Columns["ShowAirStatus"].Visible = true; } catch { }
                try { dv.Columns["ShowDLStatus"].Visible = true; } catch { }
                try { dv.Columns["SeasonCount"].Visible = true; } catch { }
                try { dv.Columns["DateAdded"].Visible = true; } catch { }
                try { dv.Columns["DateModified"].Visible = true; } catch { }
                try { dv.Columns["DirSize"].Visible = true; } catch { }
                try { dv.Columns["Flag"].Visible = true; } catch { }
                try { dv.Columns["ShowImage"].Visible = true; } catch { }
                try { dv.Columns["ShowImageWide"].Visible = true; } catch { }
                try { dv.Columns["IMDbRating"].Visible = true; } catch { }
                try { dv.Columns["Category"].Visible = true; } catch { }
                try { dv.Columns["Description"].Visible = true; } catch { }
                try { dv.Columns["IMDbVotes"].Visible = true; } catch { }
            }
            public void Enable_LITM_ShowIndex_Columns(DataGridView dv)
            {

                try { dv.Columns["ID"].ReadOnly = true; } catch { }
                try { dv.Columns["ShowName"].ReadOnly = true; } catch { }
                try { dv.Columns["EpGuideURL"].ReadOnly = true; } catch { }
                try { dv.Columns["IMDbURL"].ReadOnly = true; } catch { }
                try { dv.Columns["LocalRoot"].ReadOnly = true; } catch { }
                try { dv.Columns["ShowAirStatus"].ReadOnly = true; } catch { }
                try { dv.Columns["ShowDLStatus"].ReadOnly = true; } catch { }
                try { dv.Columns["SeasonCount"].ReadOnly = true; } catch { }
                try { dv.Columns["DateAdded"].ReadOnly = true; } catch { }
                try { dv.Columns["DateModified"].ReadOnly = true; } catch { }
                try { dv.Columns["DirSize"].ReadOnly = true; } catch { }
                try { dv.Columns["Flag"].ReadOnly = true; } catch { }
                try { dv.Columns["ShowImage"].ReadOnly = true; } catch { }
                try { dv.Columns["ShowImageWide"].ReadOnly = true; } catch { }
                try { dv.Columns["IMDbRating"].ReadOnly = true; } catch { }
                try { dv.Columns["Category"].ReadOnly = true; } catch { }
                try { dv.Columns["Description"].ReadOnly = true; } catch { }
                try { dv.Columns["IMDbVotes"].ReadOnly = true; } catch { }
            }

            #endregion
            #region ===== LITM_ShowIndex SQL Functions =====

            // Return LITM_ShowIndex SQL Connection String
            public SqlConnection LITM_ShowIndex_Conn(bool Local = true)
            {
                // populate sql connection
                SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["SQLserver"].ConnectionString);

                if (Local) { conn = new SqlConnection(ConfigurationManager.ConnectionStrings["LITMLucidMedia"].ConnectionString); }

                return conn;
            }

            // Return LITM_ShowIndex TableName (Full Path)
            public string LITM_ShowIndex_TableName(bool Local = true)
            {
                string table = "[lucidmedia].[lucidmethod].[LITM_ShowIndex]";

                if (Local) { table = "[LucidMedia].[dbo].[LITM_ShowIndex]"; }

                return table;
            }

            // Generate SQL Table

            public bool LITM_ShowIndex_InsertSQL(LITM_ShowIndex obj, bool Local = true)
            {
                SqlConnection Con = LITM_ShowIndex_Conn(Local);
                string SQLLine = "Insert Into " + LITM_ShowIndex_TableName(Local) + " (ShowName, EpGuideURL, IMDbURL, LocalRoot, ShowAirStatus, ShowDLStatus, SeasonCount, DateAdded, DateModified, DirSize, Flag, IMDbRating, Category, Description, IMDbVotes) VALUES (@ShowName, @EpGuideURL, @IMDbURL, @LocalRoot, @ShowAirStatus, @ShowDLStatus, @SeasonCount, @DateAdded, @DateModified, @DirSize, @Flag, @IMDbRating, @Category, @Description, @IMDbVotes)";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Con);
                cmd2 = new SqlCommand(SQLLine, Con);
                if (obj.ShowName == null) { obj.ShowName = ""; }
                if (obj.EpGuideURL == null) { obj.EpGuideURL = ""; }
                if (obj.IMDbURL == null) { obj.IMDbURL = ""; }
                if (obj.LocalRoot == null) { obj.LocalRoot = ""; }
                if (obj.ShowAirStatus == null) { obj.ShowAirStatus = ""; }
                if (obj.ShowDLStatus == null) { obj.ShowDLStatus = ""; }
                if (obj.SeasonCount == null) { obj.SeasonCount = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = ""; }
                if (obj.DateModified == null) { obj.DateModified = ""; }
                if (obj.DirSize == null) { obj.DirSize = ""; }
                if (obj.Flag == null) { obj.Flag = ""; }
                //if (obj.ShowImage == null) { obj.ShowImage = ""; }
                //if (obj.ShowImageWide == null) { obj.ShowImageWide = ""; }
                if (obj.IMDbRating == null) { obj.IMDbRating = ""; }
                if (obj.Category == null) { obj.Category = ""; }
                if (obj.Description == null) { obj.Description = ""; }
                if (obj.IMDbVotes == null) { obj.IMDbVotes = ""; }
                cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                cmd2.Parameters.AddWithValue(@"EpGuideURL", obj.EpGuideURL.ToString());
                cmd2.Parameters.AddWithValue(@"IMDbURL", obj.IMDbURL.ToString());
                cmd2.Parameters.AddWithValue(@"LocalRoot", obj.LocalRoot.ToString());
                cmd2.Parameters.AddWithValue(@"ShowAirStatus", obj.ShowAirStatus.ToString());
                cmd2.Parameters.AddWithValue(@"ShowDLStatus", obj.ShowDLStatus.ToString());
                cmd2.Parameters.AddWithValue(@"SeasonCount", obj.SeasonCount.ToString());
                cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded.ToString());
                cmd2.Parameters.AddWithValue(@"DateModified", obj.DateModified.ToString());
                cmd2.Parameters.AddWithValue(@"DirSize", obj.DirSize.ToString());
                cmd2.Parameters.AddWithValue(@"Flag", obj.Flag.ToString());
                //cmd2.Parameters.AddWithValue(@"ShowImage", obj.ShowImage.ToString());
                //cmd2.Parameters.AddWithValue(@"ShowImageWide", obj.ShowImageWide.ToString());
                cmd2.Parameters.AddWithValue(@"IMDbRating", obj.IMDbRating.ToString());
                cmd2.Parameters.AddWithValue(@"Category", obj.Category.ToString());
                cmd2.Parameters.AddWithValue(@"Description", obj.Description.ToString());
                cmd2.Parameters.AddWithValue(@"IMDbVotes", obj.IMDbVotes.ToString());
                if (Con.State == ConnectionState.Closed) { Con.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Con.Close();




                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool LITM_ShowIndex_UpdateSQL(LITM_ShowIndex obj, bool Local = true)
            {
                SqlConnection Conn = LITM_ShowIndex_Conn(Local);
                string SQLLine = "Update " + LITM_ShowIndex_TableName(Local) + " SET EpGuideURL = @EpGuideURL, IMDbURL = @IMDbURL, LocalRoot = @LocalRoot, ShowAirStatus = @ShowAirStatus, ShowDLStatus = @ShowDLStatus, SeasonCount = @SeasonCount, DateAdded = @DateAdded, DateModified = @DateModified, DirSize = @DirSize, Flag = @Flag, ShowImage = @ShowImage, ShowImageWide = @ShowImageWide, IMDbRating = @IMDbRating, Category = @Category, Description = @Description, IMDbVotes = @IMDbVotes WHERE ShowName = @ShowName";
                SqlCommand cmd2 = new SqlCommand(SQLLine, Conn);
                cmd2 = new SqlCommand(SQLLine, Conn);
                if (obj.ShowName == null) { obj.ShowName = ""; }
                if (obj.EpGuideURL == null) { obj.EpGuideURL = ""; }
                if (obj.IMDbURL == null) { obj.IMDbURL = ""; }
                if (obj.LocalRoot == null) { obj.LocalRoot = ""; }
                if (obj.ShowAirStatus == null) { obj.ShowAirStatus = ""; }
                if (obj.ShowDLStatus == null) { obj.ShowDLStatus = ""; }
                if (obj.SeasonCount == null) { obj.SeasonCount = ""; }
                if (obj.DateAdded == null) { obj.DateAdded = ""; }
                if (obj.DateModified == null) { obj.DateModified = ""; }
                if (obj.DirSize == null) { obj.DirSize = ""; }
                if (obj.Flag == null) { obj.Flag = ""; }
                if (obj.ShowImage == null) { obj.ShowImage = ""; }
                if (obj.ShowImageWide == null) { obj.ShowImageWide = ""; }
                if (obj.IMDbRating == null) { obj.IMDbRating = ""; }
                if (obj.Category == null) { obj.Category = ""; }
                if (obj.Description == null) { obj.Description = ""; }
                if (obj.IMDbVotes == null) { obj.IMDbVotes = ""; }

                cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName.ToString());
                cmd2.Parameters.AddWithValue(@"EpGuideURL", obj.EpGuideURL.ToString());
                cmd2.Parameters.AddWithValue(@"IMDbURL", obj.IMDbURL.ToString());
                cmd2.Parameters.AddWithValue(@"LocalRoot", obj.LocalRoot.ToString());
                cmd2.Parameters.AddWithValue(@"ShowAirStatus", obj.ShowAirStatus.ToString());
                cmd2.Parameters.AddWithValue(@"ShowDLStatus", obj.ShowDLStatus.ToString());
                cmd2.Parameters.AddWithValue(@"SeasonCount", obj.SeasonCount.ToString());
                cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded.ToString());
                cmd2.Parameters.AddWithValue(@"DateModified", obj.DateModified.ToString());
                cmd2.Parameters.AddWithValue(@"DirSize", obj.DirSize.ToString());
                cmd2.Parameters.AddWithValue(@"Flag", obj.Flag.ToString());
                cmd2.Parameters.AddWithValue(@"ShowImage", obj.ShowImage.ToString());
                cmd2.Parameters.AddWithValue(@"ShowImageWide", obj.ShowImageWide.ToString());
                cmd2.Parameters.AddWithValue(@"IMDbRating", obj.IMDbRating.ToString());
                cmd2.Parameters.AddWithValue(@"Category", obj.Category.ToString());
                cmd2.Parameters.AddWithValue(@"Description", obj.Description.ToString());
                cmd2.Parameters.AddWithValue(@"IMDbVotes", obj.IMDbVotes.ToString());
                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            public bool LITM_ShowIndex_UpdateInsert(LITM_ShowIndex obj, bool Local = true)
            {
                SqlConnection Conn = LITM_ShowIndex_Conn(Local);
                bool Updated = LITM_ShowIndex_UpdateIfPopulated(obj, Local);  // try to update record first
                if (!Updated) { Updated = LITM_ShowIndex_InsertSQL(obj, Local); }  // if unable to update, insert new record
                return Updated;
            }

            // Updates fields provided in object if values are populated. used for updating 1 or more fields at a time
            public bool LITM_ShowIndex_UpdateIfPopulated(LITM_ShowIndex obj, bool Local = true)
            {
                SqlConnection Conn = LITM_ShowIndex_Conn(Local);
                string SQLcmd = "Update " + LITM_ShowIndex_TableName(Local) + " SET ";

                if (obj.ShowName != null) { SQLcmd = SQLcmd + " ShowName = @ShowName,"; }
                if (obj.EpGuideURL != null) { SQLcmd = SQLcmd + " EpGuideURL = @EpGuideURL,"; }
                if (obj.IMDbURL != null) { SQLcmd = SQLcmd + " IMDbURL = @IMDbURL,"; }
                if (obj.LocalRoot != null) { SQLcmd = SQLcmd + " LocalRoot = @LocalRoot,"; }
                if (obj.ShowAirStatus != null) { SQLcmd = SQLcmd + " ShowAirStatus = @ShowAirStatus,"; }
                if (obj.ShowDLStatus != null) { SQLcmd = SQLcmd + " ShowDLStatus = @ShowDLStatus,"; }
                if (obj.SeasonCount != null) { SQLcmd = SQLcmd + " SeasonCount = @SeasonCount,"; }
                if (obj.DateAdded != null) { SQLcmd = SQLcmd + " DateAdded = @DateAdded,"; }
                if (obj.DateModified != null) { SQLcmd = SQLcmd + " DateModified = @DateModified,"; }
                if (obj.DirSize != null) { SQLcmd = SQLcmd + " DirSize = @DirSize,"; }
                if (obj.Flag != null) { SQLcmd = SQLcmd + " Flag = @Flag,"; }
                //if (obj.ShowImage != null) { SQLcmd = SQLcmd + " ShowImage = @ShowImage,"; }
                //if (obj.ShowImageWide != null) { SQLcmd = SQLcmd + " ShowImageWide = @ShowImageWide,"; }
                if (obj.IMDbRating != null) { SQLcmd = SQLcmd + " IMDbRating = @IMDbRating,"; }
                if (obj.Category != null) { SQLcmd = SQLcmd + " Category = @Category,"; }
                if (obj.Description != null) { SQLcmd = SQLcmd + " Description = @Description,"; }
                if (obj.IMDbVotes != null) { SQLcmd = SQLcmd + " IMDbVotes = @IMDbVotes,"; }
                SQLcmd = ahk.TrimLast(SQLcmd, 1);
                SQLcmd = SQLcmd + " WHERE ShowName = @ShowName";

                SqlCommand cmd2 = new SqlCommand(SQLcmd, Conn);

                if (obj.ShowName != null) { cmd2.Parameters.AddWithValue(@"ShowName", obj.ShowName); }
                if (obj.EpGuideURL != null) { cmd2.Parameters.AddWithValue(@"EpGuideURL", obj.EpGuideURL); }
                if (obj.IMDbURL != null) { cmd2.Parameters.AddWithValue(@"IMDbURL", obj.IMDbURL); }
                if (obj.LocalRoot != null) { cmd2.Parameters.AddWithValue(@"LocalRoot", obj.LocalRoot); }
                if (obj.ShowAirStatus != null) { cmd2.Parameters.AddWithValue(@"ShowAirStatus", obj.ShowAirStatus); }
                if (obj.ShowDLStatus != null) { cmd2.Parameters.AddWithValue(@"ShowDLStatus", obj.ShowDLStatus); }
                if (obj.SeasonCount != null) { cmd2.Parameters.AddWithValue(@"SeasonCount", obj.SeasonCount); }
                if (obj.DateAdded != null) { cmd2.Parameters.AddWithValue(@"DateAdded", obj.DateAdded); }
                if (obj.DateModified != null) { cmd2.Parameters.AddWithValue(@"DateModified", obj.DateModified); }
                if (obj.DirSize != null) { cmd2.Parameters.AddWithValue(@"DirSize", obj.DirSize); }
                if (obj.Flag != null) { cmd2.Parameters.AddWithValue(@"Flag", obj.Flag); }
                //if (obj.ShowImage != null) { cmd2.Parameters.AddWithValue(@"ShowImage", obj.ShowImage); }
                //if (obj.ShowImageWide != null) { cmd2.Parameters.AddWithValue(@"ShowImageWide", obj.ShowImageWide); }
                if (obj.IMDbRating != null) { cmd2.Parameters.AddWithValue(@"IMDbRating", obj.IMDbRating); }
                if (obj.Category != null) { cmd2.Parameters.AddWithValue(@"Category", obj.Category); }
                if (obj.Description != null) { cmd2.Parameters.AddWithValue(@"Description", obj.Description); }
                if (obj.IMDbVotes != null) { cmd2.Parameters.AddWithValue(@"IMDbVotes", obj.IMDbVotes); }

                if (Conn.State == ConnectionState.Closed) { Conn.Open(); }
                int recordsAffected = 0;
                try { recordsAffected = cmd2.ExecuteNonQuery(); }
                catch (SqlException ex) { ahk.MsgBox(ex.ToString()); return false; }
                Conn.Close();
                if (recordsAffected > 0) { return true; }
                else return false;
            }

            /// <summary>
            /// Return Show from LITM Db, Only Uses 1 of Available Search Fields when Populated
            /// </summary>
            /// <param name="ShowName"></param>
            /// <param name="LocalRoot"></param>
            /// <param name="IMDbID"></param>
            /// <param name="Local"></param>
            /// <returns></returns>
            public LITM_ShowIndex LITM_ReturnShow(string ShowName = "", string LocalRoot = "", string IMDbID = "", bool Local = true)
            {
                SqlConnection Conn = LITM_ShowIndex_Conn(Local);
                string SelectLine = "Select [ID],[ShowName],[EpGuideURL],[IMDbURL],[LocalRoot],[ShowAirStatus],[ShowDLStatus],[SeasonCount],[DateAdded],[DateModified],[DirSize],[Flag],[ShowImage],[ShowImageWide],[IMDbRating],[Category],[Description],[IMDbVotes] From " + LITM_ShowIndex_TableName(Local) + " WHERE ShowName = '" + ShowName + "'";

                if (LocalRoot != "") { SelectLine = "Select [ID],[ShowName],[EpGuideURL],[IMDbURL],[LocalRoot],[ShowAirStatus],[ShowDLStatus],[SeasonCount],[DateAdded],[DateModified],[DirSize],[Flag],[ShowImage],[ShowImageWide],[IMDbRating],[Category],[Description],[IMDbVotes] From " + LITM_ShowIndex_TableName(Local) + " WHERE LocalRoot = '" + LocalRoot + "'"; }

                if (LocalRoot != "") { SelectLine = "Select [ID],[ShowName],[EpGuideURL],[IMDbURL],[LocalRoot],[ShowAirStatus],[ShowDLStatus],[SeasonCount],[DateAdded],[DateModified],[DirSize],[Flag],[ShowImage],[ShowImageWide],[IMDbRating],[Category],[Description],[IMDbVotes] From " + LITM_ShowIndex_TableName(Local) + " WHERE IMDbURL = '" + IMDbID + "'"; }


                DataTable ReturnTable = sql.GetDataTable(Conn, SelectLine);
                LITM_ShowIndex returnObject = new LITM_ShowIndex();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.EpGuideURL = ret["EpGuideURL"].ToString();
                        returnObject.IMDbURL = ret["IMDbURL"].ToString();
                        returnObject.LocalRoot = ret["LocalRoot"].ToString();
                        returnObject.ShowAirStatus = ret["ShowAirStatus"].ToString();
                        returnObject.ShowDLStatus = ret["ShowDLStatus"].ToString();
                        returnObject.SeasonCount = ret["SeasonCount"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.DateModified = ret["DateModified"].ToString();
                        returnObject.DirSize = ret["DirSize"].ToString();
                        returnObject.Flag = ret["Flag"].ToString();
                        returnObject.ShowImage = ret["ShowImage"].ToString();
                        returnObject.ShowImageWide = ret["ShowImageWide"].ToString();
                        returnObject.IMDbRating = ret["IMDbRating"].ToString();
                        returnObject.Category = ret["Category"].ToString();
                        returnObject.Description = ret["Description"].ToString();
                        returnObject.IMDbVotes = ret["IMDbVotes"].ToString();
                        return returnObject;
                    }
                }
                return returnObject;
            }

            public List<LITM_ShowIndex> LITM_ShowIndexList(bool Local = true)
            {
                string Command = "Select * From " + LITM_ShowIndex_TableName(Local);
                SqlConnection Conn = LITM_ShowIndex_Conn(Local);
                DataTable ReturnTable = sql.GetDataTable(Conn, Command);
                List<LITM_ShowIndex> ReturnList = new List<LITM_ShowIndex>();
                if (ReturnTable != null)
                {
                    foreach (DataRow ret in ReturnTable.Rows)
                    {
                        LITM_ShowIndex returnObject = new LITM_ShowIndex();
                        returnObject.ID = ret["ID"].ToString();
                        returnObject.ShowName = ret["ShowName"].ToString();
                        returnObject.EpGuideURL = ret["EpGuideURL"].ToString();
                        returnObject.IMDbURL = ret["IMDbURL"].ToString();
                        returnObject.LocalRoot = ret["LocalRoot"].ToString();
                        returnObject.ShowAirStatus = ret["ShowAirStatus"].ToString();
                        returnObject.ShowDLStatus = ret["ShowDLStatus"].ToString();
                        returnObject.SeasonCount = ret["SeasonCount"].ToString();
                        returnObject.DateAdded = ret["DateAdded"].ToString();
                        returnObject.DateModified = ret["DateModified"].ToString();
                        returnObject.DirSize = ret["DirSize"].ToString();
                        returnObject.Flag = ret["Flag"].ToString();
                        returnObject.ShowImage = ret["ShowImage"].ToString();
                        returnObject.ShowImageWide = ret["ShowImageWide"].ToString();
                        returnObject.IMDbRating = ret["IMDbRating"].ToString();
                        returnObject.Category = ret["Category"].ToString();
                        returnObject.Description = ret["Description"].ToString();
                        returnObject.IMDbVotes = ret["IMDbVotes"].ToString();
                        ReturnList.Add(returnObject);
                    }
                }
                return ReturnList;
            }

            public bool LITM_ShowIndex_SQL_to_SQLite(string SqliteDBPath = @"\Db\LITM_ShowIndex.sqlite")
            {
                string SaveFile = SqliteDBPath;
                if (SqliteDBPath == @"\Db\LITM_ShowIndex.sqlite")
                {
                    ahk.FileCreateDir(ahk.AppDir() + @"\Db");
                    SaveFile = ahk.AppDir() + @"\Db\LITM_ShowIndex.sqlite";
                }

                //sb("Copying SQL Db to " + SaveFile + "...");
                sqlite.SQLTable_To_NewSQLiteTable(LITM_ShowIndex_Conn(), "LITM_ShowIndex", "LITM_ShowIndex", SaveFile, "", false, false, false);
                //sb("FINISHED Copying SQL Db to " + SaveFile);

                if (File.Exists(SaveFile)) { return true; } else { return false; }
            }







            #endregion

            #endregion

            #region === Show Images ===

            public bool WriteShowImage(string filepath, string ShowName, bool ShowImageWide = false, bool Local = true)
            {
                string TableName = LITM_ShowIndex_TableName(Local);
                SqlConnection Conn = LITM_ShowIndex_Conn(Local);
                int updateCount = 0;
                if (File.Exists(filepath))
                {
                    // write image file 

                    FileInfo info = new FileInfo(filepath);
                    string FileName = info.Name;
                    string FileSize = info.Length.ToString();
                    string DateModified = info.LastWriteTime.ToString();

                    //SqlConnection SQLConn = new SqlConnection("Server=198.71.225.113;DataBase=MediaServer;Uid=lucid;Pwd=Go1Daddy88");

                    if (Conn.State == ConnectionState.Closed) { Conn.Open(); }

                    byte[] file;
                    using (var stream = new FileStream(filepath, FileMode.Open, System.IO.FileAccess.Read))
                    {
                        using (var reader = new BinaryReader(stream))
                        {
                            file = reader.ReadBytes((int)stream.Length);
                        }
                    }
                    //using (var varConnection = Locale.sqlConnectOneTime(Locale.sqlDataConnectionDetails))

                    string cmd = "UPDATE " + TableName + " Set [ShowImage] = @ShowImage WHERE [ShowName] = @ShowName";

                    if (ShowImageWide) { cmd = "UPDATE " + TableName + " Set [ShowImageWide] = @ShowImageWide WHERE [ShowName] = @ShowName"; }

                    using (var sqlWrite = new SqlCommand(cmd, Conn))
                    {
                        sqlWrite.Parameters.AddWithValue("@ShowName", ShowName);

                        if (ShowImageWide) { sqlWrite.Parameters.Add("@ShowImageWide", SqlDbType.VarBinary, file.Length).Value = file; }
                        if (!ShowImageWide) { sqlWrite.Parameters.Add("@ShowImage", SqlDbType.VarBinary, file.Length).Value = file; }

                        updateCount = sqlWrite.ExecuteNonQuery();
                    }
                }

                if (updateCount != 0) { return true; }
                return false;
            }

            public void Download_ShowImages(bool Local = true)
            {
                _Lists lst = new _Lists();
                _TelerikLib.RadProgress pro = new _TelerikLib.RadProgress();

                SqlConnection Conn = LITM_ShowIndex_Conn(Local);
                string table = LITM_ShowIndex_TableName(Local);

                List<string> ShowsWithImages = lst.SQL_To_List(Conn, "select [ShowName] FROM [lucidmedia].[lucidmethod].[LITM_ShowIndex] where [ShowImage] is not Null");

                pro.SetupProgressBar(radProgress, ShowsWithImages.Count);

                int i = 0;
                foreach (string ShowName in ShowsWithImages)
                {
                    i++; pro.UpdateProgress(radProgress, ShowName + " | " + i + "/" + ShowsWithImages.Count);

                    //string ShowName = "Ugly Betty";
                    string cmd = "select ShowImage from " + table + " where ShowName = '" + ShowName + "'";

                    string dir = ahk.AppDir() + "\\DbSync_ShowImages";
                    ahk.FileCreateDir(dir);

                    string showImgPath = dir + "\\" + ShowName + ".jpg";
                    ahk.FileDelete(showImgPath);

                    Image showImg = ReturnShowImage(Conn, cmd, showImgPath);

                    WriteShowImage(showImgPath, ShowName, false, true);
                }
            }

            public Image ReturnShowImage(SqlConnection SQLConn, string SQLSearchLine, string LocalFilePath = "")
            {
                //string SQLSearchLine = "SELECT [FileContents] FROM [MediaServer].[lucid].[FileBin] WHERE [FileName] = 'TestFile.zip'";
                _AHK ahk = new _AHK();

                bool DeleteTmpFile = false;
                if (LocalFilePath == "") { DeleteTmpFile = true; LocalFilePath = ahk.AppDir() + "\\IconTemp.png"; }

                if (SQLConn.State == ConnectionState.Closed) { SQLConn.Open(); }
                using (var sqlQuery = new SqlCommand(SQLSearchLine, SQLConn))
                {
                    //sqlQuery.Parameters.AddWithValue("@FileName", ServerFileName);
                    using (var sqlQueryResult = sqlQuery.ExecuteReader())
                        if (sqlQueryResult != null)
                        {
                            sqlQueryResult.Read();
                            var blob = new Byte[(sqlQueryResult.GetBytes(0, 0, null, 0, int.MaxValue))];
                            sqlQueryResult.GetBytes(0, 0, blob, 0, blob.Length);
                            using (var fs = new FileStream(LocalFilePath, FileMode.Create, FileAccess.Write))
                                fs.Write(blob, 0, blob.Length);
                        }
                }

                SQLConn.Close();

                //Image returnImg = img.GetCopyImage(LocalFilePath);  // convert local return img to Image format

                Image returnImg = Image.FromFile(LocalFilePath);

                //if (DeleteTmpFile) { ahk.FileDelete(LocalFilePath); }  // delete local temp image

                return returnImg;
            }



            #endregion

            #endregion


        }


    }


    #endregion





}

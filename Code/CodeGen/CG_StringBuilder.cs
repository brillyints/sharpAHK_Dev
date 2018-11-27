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


        // convert string / text file to string builder format

        // for converting HTML pages into StringBuilder lines to add to C# ASP projects
        public string File_ToStringBuilder_HTML(string filePath = @"C:\Users\jason\Google Drive\tempHtml.txt")
        {
            _Lists lst = new _Lists();
            _AHK ahk = new _AHK();

            List<string> lines = lst.TextFile_To_List(filePath, true, true, false);

            string varName = "NewString";
            string Out = "StringBuilder " + varName + " = new StringBuilder();\n";

            foreach (string line in lines)
            {
                string writeLine = line.Replace("\"", "\\\"");
                writeLine = varName + ".Append(\"" + writeLine + "\");\n";

                Out = Out + writeLine;

                //ahk.MsgBox(Out); 
            }

            //ahk.FileAppend(Out, ahk.AppDir() + "\\TempOut.txt");
            //ahk.Run(ahk.AppDir() + "\\TempOut.txt");

            return Out;
        }

        // ToStringBuilderFormat: converts collection of strings into stringbuilder add lines, used to build html inserts
        //
        // TextLines = lines to convert from string to new string builder add lines
        // varName = the name of the variable used in the output string
        // AsFunction = bool determines whether output is just the assembled lines or as a function to paste into projects 
        public string ToStringBuilderFormat(string TextLines, string varName = "NewString", bool AsFunction = true)
        {
            _Lists lst = new _Lists();
            _AHK ahk = new _AHK();

            List<string> lines = lst.Text_To_List(TextLines, true, true, false);

            string Out = "StringBuilder _" + varName + " = new StringBuilder();\n";

            foreach (string line in lines)
            {
                string writeLine = line.Replace("\"", "\\\"");
                writeLine = "_" + varName + ".AppendLine(\"" + writeLine + "\");\n";
                Out = Out + writeLine;
                //ahk.MsgBox(Out); 
            }

            bool HTMLPlaceHolderFunction = false;

            // option to generate output as a function
            if (HTMLPlaceHolderFunction)
            {
                StringBuilder NewString = new StringBuilder();
                NewString.Append("/// <summary>\n");
                NewString.Append("/// " + varName + "_HtmlInsert : Returns HTML, Inserts into .aspx PlaceHolder.\n");
                NewString.Append("/// <example>\n");
                NewString.Append("/// Add line to aspx:   <asp:PlaceHolder ID =\"" + varName + "\" runat=\"server\" />\n");
                NewString.Append("/// Add line to c#:     " + varName + "_HtmlInsert(\"" + varName + "\");\n");
                NewString.Append("/// </summary></example>\n");
                NewString.Append("/// <param name=\"" + varName + "\">ID of the Placeholder used in aspx/html. Code generated in c# is inserted here.</param>\n");
                NewString.Append("public void " + varName + "_HtmlInsert(PlaceHolder " + varName + ")\n");
                NewString.Append("{\n");
                NewString.Append(Out);
                NewString.Append("\n");
                NewString.Append(varName + ".Controls.Add(new Literal { Text = _" + varName + ".ToString() });");
                NewString.Append("\n");
                NewString.Append("}\n");

                Out = NewString.ToString();
            }
            if (AsFunction)
            {
                StringBuilder NewString = new StringBuilder();
                NewString.Append("/// <summary>\n");
                NewString.Append("/// " + varName + "_HtmlInsert : Returns HTML, Inserts into .aspx PlaceHolder.\n");
                NewString.Append("/// <example>\n");
                NewString.Append("/// Add line to aspx:   <asp:PlaceHolder ID =\"" + varName + "\" runat=\"server\" />\n");
                NewString.Append("/// Add line to c#:     " + varName + "_HtmlInsert(\"" + varName + "\");\n");
                NewString.Append("/// </summary></example>\n");
                NewString.Append("/// <param name=\"" + varName + "\">ID of the Placeholder used in aspx/html. Code generated in c# is inserted here.</param>\n");
                NewString.Append("public void " + varName + "_HtmlInsert(PlaceHolder " + varName + ")\n");
                NewString.Append("{\n");
                NewString.Append(Out);
                NewString.Append("\n");
                NewString.Append(varName + ".Controls.Add(new Literal { Text = _" + varName + ".ToString() });");
                NewString.Append("\n");
                NewString.Append("}\n");

                Out = NewString.ToString();
            }


            //ahk.FileAppend(Out, ahk.AppDir() + "\\TempOut.txt");
            //ahk.Run(ahk.AppDir() + "\\TempOut.txt");

            return Out;
        }


    }
}

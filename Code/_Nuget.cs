using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sharpAHK;
using sharpAHK_Dev;
using AHKExpressions;
using DevExpressions;
using TelerikExpressions;
using Telerik.WinControls.UI;
using System.IO;


namespace sharpAHK_Dev
{
    public partial class _Code
    {
        public class Nugets
        {

            /// <summary>
            /// Populate Default NugetSpec Values
            /// </summary>
            /// <returns></returns>
            public nugetSpec Defaults()
            {
                nugetSpec sln = new nugetSpec();

                sln.slnPath = ""; // required
                sln.csproj = @"C:\_Code\radProjects\radDLLs\sharpAHK\sharpAHK_Dll\AutoHotkey.Interop\sharpAHK.csproj";
                sln.description = "AutoHotkey for C#";
                sln.releasenotes = "SharpAHK V1 Release";
                sln.copyright = "Copyright 2019";
                sln.tags = "sharpAHK AutoHotkey";
                sln.authors = "LucidMethods";
                sln.owners = ""; if (sln.owners.Trim() == "") { sln.owners = sln.authors; }
                sln.version = ""; if (sln.version.Trim() == "") { sln.version = "1.0.0.1"; }
                sln.iconurl = "";
                sln.LicenseURL = "";
                sln.projecturl = "";
                //sln.dependencies // list of dependencies to add to nuspec
                sln.requireLicAcceptance = false;
                return sln;
            }

            /// <summary>
            /// Update Nuget NuSpec File with New Values
            /// </summary>
            /// <param name="sln"></param>
            /// <returns></returns>
            public bool UpdateNuSpec(nugetSpec sln)
            {
                List<string> dependencies = new List<string>();
                bool requireLicAcceptance = false;

                StringBuilder builder = new StringBuilder();
                builder.AppendLine("<?xml version=\"1.0\"?>");
                builder.AppendLine("<package >");
                builder.AppendLine("  <metadata>");
                builder.AppendLine("    <id>" + sln.csproj + "</id>");
                builder.AppendLine("	<authors>" + sln.authors + "</author>");
                builder.AppendLine("    <version>" + sln.version + "</version>");
                builder.AppendLine("    <owners>" + sln.owners + "</owners>");
                builder.AppendLine("    <licenseUrl>" + sln.LicenseURL + "</licenseUrl>");
                builder.AppendLine("    <projectUrl>" + sln.projecturl + "</projectUrl>");
                builder.AppendLine("    <iconUrl>" + sln.iconurl + "</iconUrl>");
                builder.AppendLine("    <requireLicenseAcceptance>" + requireLicAcceptance.ToString() + "</requireLicenseAcceptance>");
                builder.AppendLine("    <description>" + sln.description + "</description>");
                builder.AppendLine("    <releaseNotes>" + sln.releasenotes + "</releaseNotes>");
                builder.AppendLine("    <copyright>" + sln.copyright + "</copyright>");
                builder.AppendLine("    <tags>" + sln.tags + "</tags>");
                builder.AppendLine("    <dependencies>");
                foreach (string dep in dependencies) { builder.AppendLine(dep); }
                builder.AppendLine("    </dependencies>");
                builder.AppendLine("  </metadata>");
                builder.AppendLine("</package>");

                return true;
            }

            /// <summary>
            /// Read Nuget NuSpec File to Populate nugetSpec Object
            /// </summary>
            /// <param name="NuSpecPath"></param>
            /// <returns></returns>
            public nugetSpec ReadNuSpec(string NuSpecPath)
            {
                nugetSpec sln = new nugetSpec();
                _AHK ahk = new _AHK();
                _Parse prs = new _Parse();
                string nutext = ahk.FileRead(NuSpecPath);
                if (nutext != "")
                {
                    sln.csproj = prs.XML_Text(nutext, "<id>");
                    sln.authors = prs.XML_Text(nutext, "<authors>");
                    sln.version = prs.XML_Text(nutext, "<version>");
                    sln.owners = prs.XML_Text(nutext, "<owners>");
                    sln.LicenseURL = prs.XML_Text(nutext, "<licenseUrl>");
                    sln.projecturl = prs.XML_Text(nutext, "<projectUrl>");
                    sln.iconurl = prs.XML_Text(nutext, "<iconUrl>");
                    sln.requireLicAcceptance = prs.XML_Text(nutext, "<requireLicenseAcceptance>").ToBool();
                    sln.description = prs.XML_Text(nutext, "<description>");
                    sln.releasenotes = prs.XML_Text(nutext, "<releaseNotes>");
                    sln.copyright = prs.XML_Text(nutext, "<copyright>");
                    sln.tags = prs.XML_Text(nutext, "<tags>");
                }

                return sln;
            }

            /// <summary>
            /// Object to Populate to Create/Update a NuGet .nuspec File
            /// </summary>
            public class nugetSpec
            {
                public RadTextBox txtslnPath { get; set; }
                public RadTextBox txtdescription { get; set; }
                public RadTextBox txtcsproj { get; set; }
                public RadTextBox txtreleasenotes { get; set; }
                public RadTextBox txtcopyright { get; set; }
                public RadTextBox txtauthors { get; set; }
                public RadTextBox txttags { get; set; }
                public RadTextBox txtowners { get; set; }
                public RadTextBox txtversion { get; set; }
                public RadTextBox txticonurl { get; set; }
                public RadTextBox txtLicenseURL { get; set; }
                public RadTextBox txtprojecturl { get; set; }
                public RadCheckBox cbrequireLicAcceptance { get; set; }
                //public RadTextBox txt { get; set; }


                public string slnPath { get; set; }
                public string description { get; set; }
                public string csproj { get; set; }
                public string releasenotes { get; set; }
                public string copyright { get; set; }
                public string authors { get; set; }
                public string tags { get; set; }
                public string owners { get; set; }
                public string version { get; set; }
                public string iconurl { get; set; }
                public string LicenseURL { get; set; }
                public string projecturl { get; set; }
                public List<string> dependencies { get; set; }
                public bool requireLicAcceptance { get; set; }

            }



            /// <summary>
            /// Create Spec file to Edit for Nuget Packaging (Using Nuget.exe)
            /// </summary>
            /// <param name="slnPath">Path to VisualStudio Solution File</param>
            /// <param name="nugetPath">Path to Nuget.exe</param>
            /// <returns>Returns True if Both SLN and EXE Are Found - Does not reflect status of the new exe job results</returns>
            public bool CreateSpec(string slnPath, string nugetPath = "")
            {
                _AHK ahk = new _AHK();

                if (nugetPath == "") { nugetPath = ahk.AppDir() + "\\EXEs\\NuGet.exe"; }
                if (File.Exists(nugetPath)) { return false; }
                if (File.Exists(slnPath)) { return false; }

                string line = "\"" + nugetPath + "\" spec \"" + slnPath + "\"";
                ahk.Run(line);
                return true;
            }

            /// <summary>
            /// Create Nuget File to Upload (Using Nuget.exe)
            /// </summary>
            /// <param name="slnPath">Path to VisualStudio Solution File</param>
            /// <param name="nugetPath">Path to Nuget.exe</param>
            /// <returns>Returns True if Both SLN and EXE Are Found - Does not reflect status of the new exe job results</returns>
            public bool CreateNuget(string slnPath, string nugetPath = "")
            {
                _AHK ahk = new _AHK();

                if (nugetPath == "") { nugetPath = ahk.AppDir() + "\\EXEs\\NuGet.exe"; }
                if (File.Exists(nugetPath)) { return false; }
                if (File.Exists(slnPath)) { return false; }

                string line = "\"" + nugetPath + "\" pack \"" + slnPath + "\"";
                ahk.Run(line);
                return true;
            }

            /// <summary>
            /// Returns URL to SharpAHK Nuget Page (Option to Open Page)
            /// </summary>
            /// <param name="OpenPage">If True, Launches Webpage</param>
            /// <returns>Returns Path to Page</returns>
            public string SharpAHK(bool OpenPage = false)
            {
                _AHK ahk = new _AHK();
                string url = "https://www.nuget.org/packages/sharpAHK/";
                if (OpenPage) { ahk.Run(url); }
                return url;
            }

            /// <summary>
            /// Returns URL to SharpAHK Nuget Page (Option to Open Page)
            /// </summary>
            /// <param name="OpenPage">If True, Launches Webpage</param>
            /// <returns>Returns Path to Page</returns>
            public string SharpAHKDev(bool OpenPage = false)
            {
                _AHK ahk = new _AHK();
                string url = "https://www.nuget.org/packages/sharpAHK_Dev/";
                if (OpenPage) { ahk.Run(url); }
                return url;
            }

            /// <summary>
            /// Returns URL to Nuget Upload Page (Option to Open Page)
            /// </summary>
            /// <param name="OpenPage">If True, Launches Webpage</param>
            /// <returns>Returns Path to Page</returns>
            public string UploadPage(bool OpenPage = false)
            {
                _AHK ahk = new _AHK();
                string url = "https://www.nuget.org/packages/manage/upload";
                if (OpenPage) { ahk.Run(url); }
                return url;
            }




        }

    }
}

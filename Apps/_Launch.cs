using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sharpAHK;
using sharpAHK_Dev;
using AHKExpressions;
using TelerikExpressions;
using System.ComponentModel;

namespace sharpAHK_Dev
{
    public partial class _Apps
    {
        private static _AHK ahk = new _AHK();

        [DefaultValue(AppIndex)]
        public enum AppName
        {
            ADBSiteParse,
            AppIndex,
            CodeSort,
            ComPress,
            IMDb,
            LucidGenerator,
            ProjectIndex,
            VSCodeDock
        }

        public void Launch(AppName app)
        {
            if (app == AppName.ADBSiteParse) { ahk.RunOrActivate("ADB SiteParse", @"C:\_Code\LucidProjects\ADBindex\ADBSiteParse\bin\Debug\ADBSiteParse.exe"); }
            if (app == AppName.AppIndex) { ahk.RunOrActivate("LITM AppIndex", @"C:\_Code\LucidProjects\AppIndex\bin\Debug\AppIndex.exe"); }

            if (app == AppName.CodeSort) { ahk.RunOrActivate("CodeSort", @"C:\_Code\LucidProjects\TelerikDev18\CodeSort\bin\Debug\CodeSort.exe"); }
            if (app == AppName.ComPress) { ahk.RunOrActivate("comPress", @"C:\_Code\LucidProjects\ADBindex\ComPress\bin\Debug\ComPress.exe"); }
            if (app == AppName.IMDb) { ahk.RunOrActivate("IMDb Info", @"C:\_Code\DEBUGS\Debug_IMDB\IMDB_Info.exe"); }

            if (app == AppName.LucidGenerator) { ahk.RunOrActivate("Lucid.Generator", @"C:\_Code\LucidProjects\LucidGenerator\bin\Debug\LucidGenerator.exe"); }
            if (app == AppName.ProjectIndex) { ahk.RunOrActivate("ProjectLib", @"C:\_Code\LucidProjects\AppIndex\bin\_ProjectIndex\AppIndex.exe"); }

            if (app == AppName.VSCodeDock) { ahk.RunOrActivate("VSCodeDock | SharpAHK Add-On", @"C:\_Code\radProjects\radDock\VS_CodeDock\bin\Debug\VS_CodeDock.exe"); }

        }


    }
}

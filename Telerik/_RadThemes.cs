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
        public class RadThemes
        {

            public void LoadTheme(string themeName)
            {
                ThemeResolutionService.ApplicationThemeName = themeName;
            }


            // To Test: Dynamic Theme Loading

            RadDropDownList themeDropDown = new RadDropDownList();

            public void DynamicThemeLoad()
            {
                // source: https://stackoverflow.com/questions/4393098/how-do-i-switch-themes-in-telerik-winforms/4424735

                var themefiles = Directory.GetFiles(System.Windows.Forms.Application.StartupPath, "Telerik.WinControls.Themes.*.dll");

                foreach (var theme in themefiles)
                {
                    var themeAssembly = Assembly.LoadFile(theme);
                    var themeType = themeAssembly.GetTypes().Where(t => typeof(RadThemeComponentBase).IsAssignableFrom(t)).FirstOrDefault();
                    if (themeType != null)
                    {
                        RadThemeComponentBase themeObject = (RadThemeComponentBase)Activator.CreateInstance(themeType);
                        if (themeObject != null)
                        {
                            themeObject.Load();
                        }
                    }
                }
                var themeList = ThemeRepository.AvailableThemeNames.ToList();
                themeDropDown.DataSource = themeList;
            }

            private void ThemeDropDown_SelectedIndexChanged(object sender, Telerik.WinControls.UI.Data.PositionChangedEventArgs e)
            {
                string strTheme = themeDropDown.Text;
                Theme theme = ThemeResolutionService.GetTheme(strTheme);
                if (theme != null)
                {
                    ThemeResolutionService.ApplicationThemeName = theme.Name;
                }
            }


        }

    }
}

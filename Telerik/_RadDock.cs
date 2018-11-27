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
        public class RadDockLib
        {

            #region === RadDock Controls To Define ===

            public static RadDock raddock { get; set; }
            public static RadTreeView NewNodeTree { get; set; }
            public static RadLabelElement sbRadLabelElement { get; set; }


            /// <summary>
            /// StatusBar Display Message if Control Provided
            /// </summary>
            /// <param name="Text"></param>
            private void sb(string Text = "") { if (sbRadLabelElement != null) { sbRadLabelElement.Text = Text; } }


            #endregion


            #region === Dock RadMenu ===

            /// <summary>
            /// Returns List of RadDock Options Used to Populate RadMenu
            /// </summary>
            /// <returns></returns>
            public List<string> DockMenuOptions()
            {
                _Lists lst = new _Lists();

                List<string> options = new List<string>(new string[] { "Active Document", "Active Window", "Add Form", "Restore States",
                "Document Bold On",
                "Document Bold Off",
                "Document Insert Order Front",
                "Document Insert Order Back",
                "Dock Template VS 2008",
                "Dock Template VS 2010",
                "Dock Template Office 2010",
                "Dock Template Default",
                "Dock Template Custom",
                "Enable DragDrop Services",
                "Enable Dock Manager Edges",
                "Enable Dock Shortcuts",
                "Register Dock Custom Command",
                "Toggle Command Manager",
                "Configure Context Menu",
                "Toggle SingleScreen FloadMode",
                "Toggle Window Snapping Mode",
                "New Tool Window",
                "Close All Windows"
            });

                options = options.ListSORT();

                return options;
            }

            /// <summary>
            /// RadMenu Actions for RadDock Control
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void DockMenu_Click(object sender, EventArgs e)
            {
                _AHK ahk = new _AHK();

                RadMenuItem clicked = (RadMenuItem)sender; string txt = clicked.Text;

                if (txt == "Add Form") { DockAddForm(raddock, "TreePrompt", DockPosition.Right); }
                if (txt == "Restore States")
                {
                    RedockService service = raddock.GetService<RedockService>();
                    foreach (DockWindow window in raddock.DockWindows)
                    {
                        if (window.DockState == DockState.Floating)
                        {
                            service.RestoreState(window, DockState.Docked, true);
                        }
                    }
                }
                if (txt == "Active Document")
                {
                    DockWindow activeDocument = raddock.DocumentManager.ActiveDocument;
                    string name = "";
                    try { name = activeDocument.Name; }
                    catch { }

                    ahk.MsgBox("Active Document Name: " + name);
                }
                if (txt == "Active Window")
                {
                    DockWindow activeWindow = raddock.ActiveWindow;
                    ahk.MsgBox("Active Window Name: " + activeWindow.Name);
                }

                try
                {
                    if (txt == "Document Bold On") { raddock.DocumentManager.BoldActiveDocument = true; }
                    if (txt == "Document Bold Off") { raddock.DocumentManager.BoldActiveDocument = false; }
                    if (txt == "Document Insert Order Front") { raddock.DocumentManager.DocumentInsertOrder = DockWindowInsertOrder.InFront; }
                    if (txt == "Document Insert Order Back") { raddock.DocumentManager.DocumentInsertOrder = DockWindowInsertOrder.ToBack; }
                    if (txt == "Dock Template VS 2008") { raddock.DockingGuidesTemplate = PredefinedDockingGuidesTemplate.VS2008; }
                    if (txt == "Dock Template VS 2010") { raddock.DockingGuidesTemplate = PredefinedDockingGuidesTemplate.VS2010; }
                    if (txt == "Dock Template Office 2010") { raddock.DockingGuidesTemplate = PredefinedDockingGuidesTemplate.Office2010; }
                    if (txt == "Dock Template Default") { raddock.DockingGuidesTemplate = PredefinedDockingGuidesTemplate.ControlDefault; }
                    if (txt == "Dock Template Custom") { NewDockingGuideTemplate(); }
                    if (txt == "Enable DragDrop Services") { InitDragDropEvents(); }
                    if (txt == "Enable Dock Manager Edges") { InitDragDropProperties(); }
                    if (txt == "Enable Dock Shortcuts") { EnableDockShortcuts(); }
                    if (txt == "Register Dock Custom Command") { RegisterCustomCommand(); }
                    if (txt == "Toggle Command Manager") { ToggleCommandManager(); }
                    if (txt == "Configure Context Menu") { ConfigureContextMenu(); }
                    if (txt == "Toggle SingleScreen FloadMode") { ToggleSingleScreenFloatMode(); }
                    if (txt == "Toggle Window Snapping Mode") { ToggleWindowSnapping(); }
                    if (txt == "Close All Windows") { CloseAllWindows(); }
                    if (txt == "New Tool Window") { NewToolWindow(); }
                }
                catch
                {

                    //bool launched = CommandLaunch(ToSharpLaunch(txt));

                    //LaunchForm ln = new LaunchForm();
                    //bool launched = ln.Launch(txt, false, "", CurrentForm);

                    string error = "";
                }

                //if (txt == "") { }
                //if (txt == "") { }
                //if (txt == "") { }
                //if (txt == "") { }
                //if (txt == "") { }
                //if (txt == "") { }
                //if (txt == "") { }

                sb("Clicked " + txt);
            }


            /// <summary>
            /// Attach Launch Menu to Any LITM Form to launch other forms in project
            /// </summary>
            /// <param name="radmenu">RadMenu To Attach Launch Menu</param>
            public void Build_RadDock_Menu(RadMenu radmenu)
            {
                RadMenuItem Main = new RadMenuItem();
                Main.Text = "RadDock";
                Main.Name = "menuRadDock";

                List<string> SectionNames = DockMenuOptions();

                // Add List of Section SubMenus Under Main
                foreach (string item in SectionNames)
                {
                    RadMenuItem Sub = new RadMenuItem();
                    Sub.Text = item; Sub.Tag = item;
                    Sub.Name = "menu" + item.Replace(" ", ""); // remove spaces from name
                    Sub.Click += new EventHandler(DockMenu_Click);
                    Main.Items.Add(Sub);
                }

                // Attach New Menu Items to RadMenu
                radmenu.Items.Add(Main);
            }



            #endregion


            #region === RadDock Actions ===

            /// <summary>
            /// Add New/Existing Form As Docked Window
            /// </summary>
            /// <param name="dock">RadDock Control</param>
            /// <param name="FormName">LITM FormName to Add</param>
            /// <param name="Position">Window Position to Dock New Form</param>
            public void DockAddForm(RadDock dock, string FormName, Telerik.WinControls.UI.Docking.DockPosition Position)
            {
                //launch lnc = new launch();
                //Form form = new Form();
                //bool Found = lnc.DockAddForm(dock, FormName, Position);

                //if (Found)
                //{
                //    form.BackColor = Color.DarkBlue;
                //    dock.DockControl(form, Position);
                //    form.Show();
                //}
            }

            /// <summary>
            /// Create Customized RadDock Guide Template and Apply
            /// </summary>
            DockingGuidesTemplate template;
            public void NewDockingGuideTemplate()
            {
                template = new DockingGuidesTemplate();

                template.DockingHintBackColor = Color.FromArgb(30, Color.Green);
                template.DockingHintBorderColor = Color.FromArgb(30, Color.DarkGreen);

                //template.LeftImage.Image = Properties.Resources.Left;
                //template.TopImage.Image = Properties.Resources.Top;
                //template.RightImage.Image = Properties.Resources.Right;
                //template.BottomImage.Image = Properties.Resources.Bottom;
                //template.FillImage.Image = Properties.Resources.Fill;

                //template.LeftImage.HotImage = Properties.Resources.LeftHot;
                //template.TopImage.HotImage = Properties.Resources.TopHot;
                //template.RightImage.HotImage = Properties.Resources.RightHot;
                //template.BottomImage.HotImage = Properties.Resources.BottomHot;
                //template.FillImage.HotImage = Properties.Resources.FillHot;

                //template.CenterBackgroundImage.Image = Properties.Resources.Center;

                template.LeftImage.LocationOnCenterGuide = new Point(0, 28);
                template.TopImage.LocationOnCenterGuide = new Point(28, 0);
                template.RightImage.LocationOnCenterGuide = new Point(65, 28);
                template.BottomImage.LocationOnCenterGuide = new Point(28, 65);
                template.FillImage.LocationOnCenterGuide = new Point(28, 28);

                raddock.DockingGuidesTemplate = template;
            }


            /// <summary>
            /// Dock DragDropService Svents
            /// </summary>
            private void InitDragDropEvents()
            {
                DragDropService service = raddock.GetService<DragDropService>();
                service.PreviewDockPosition += new DragDropDockPositionEventHandler(service_PreviewDockPosition);
            }
            private void service_PreviewDockPosition(object sender, DragDropDockPositionEventArgs e)
            {
                if (e.DropTarget == raddock.MainDocumentContainer)
                {
                    e.AllowedDockPosition = AllowedDockPosition.Bottom;
                }
            }


            /// <summary>
            ///     Allowed Dock Manager Edges
            ///     The service may be told which edges of the owning RadDock instance are allowed for dock operation.
            ///     The following example demonstrates how to set only left and right edges as allowed:
            /// </summary>
            private void InitDragDropProperties()
            {
                DragDropService service = raddock.GetService<DragDropService>();
                //service.AllowedDockManagerEdges = AllowedDockPosition.Left | AllowedDockPosition.Right;
            }


            /// <summary>
            /// Enable Dock Shortcuts
            /// </summary>
            private void EnableDockShortcuts()
            {
                RadDockCommand command = raddock.CommandManager.FindCommandByName(PredefinedCommandNames.NextDocument);
                command.Shortcuts.Clear();
                command.Shortcuts.Add(new RadShortcut(Keys.Shift, Keys.A, Keys.S));
            }


            /// <summary>
            /// Register RadDock Custom Command
            /// </summary>
            private void RegisterCustomCommand()
            {
                raddock.CommandManager.RegisterCommand(new FloatWindowCommand());
            }
            /// <summary>
            ///   Registering Custom Command
            ///   The completely transparent object model of the command manager allows you to create and register completely custom command and associate it with the desired key combination.
            ///   The following code demonstrates how to create custom command that floats the currently active tool window and is associated with the CTRL+F shortcut:
            /// </summary>
            public class FloatWindowCommand : RadDockCommand
            {
                public FloatWindowCommand()
                {
                    this.Name = "FloatWindow";
                    this.Shortcuts.Add(new RadShortcut(Keys.Control, Keys.F));
                }
                public override bool CanExecute(object parameter)
                {
                    RadDock dock = parameter as RadDock;
                    if (dock == null)
                    {
                        return false;
                    }
                    return dock.ActiveWindow is ToolWindow;
                }
                public override object Execute(params object[] settings)
                {
                    RadDock dock = settings[0] as RadDock;
                    Debug.Assert(dock != null, "Invalid execute parameter!");
                    ToolWindow toolWindow = dock.ActiveWindow as ToolWindow;
                    if (toolWindow != null)
                    {
                        dock.FloatWindow(toolWindow);
                    }
                    return base.Execute(settings);
                }
            }



            /// <summary>
            /// Toggle Enabling/Disabling 
            /// </summary>
            private void ToggleCommandManager()
            {
                if (raddock.CommandManager.Enabled) { raddock.CommandManager.Enabled = false; sb("Command Manager Disabled"); }
                else { raddock.CommandManager.Enabled = true; sb("Command Manager Enabled"); }
            }



            /// <summary>
            /// Enable Rules that Modify Dock Context Menus
            /// </summary>
            public void ConfigureContextMenu()
            {
                ContextMenuService menuService = raddock.GetService<ContextMenuService>();
            }
            /// <summary>
            /// Modify Dock Context Menu
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            private void menuService_ContextMenuDisplaying(object sender, ContextMenuDisplayingEventArgs e)
            {
                //the menu request is associated with a valid DockWindow instance, which resides within a DocumentTabStrip
                if (e.MenuType == ContextMenuType.DockWindow &&
                    e.DockWindow.DockTabStrip is DocumentTabStrip)
                {
                    //remove the "Close" menu items
                    for (int i = 0; i < e.MenuItems.Count; i++)
                    {
                        RadMenuItemBase menuItem = e.MenuItems[i];
                        if (menuItem.Name == "CloseWindow" ||
                            menuItem.Name == "CloseAllButThis" ||
                            menuItem.Name == "CloseAll" ||
                            menuItem is RadMenuSeparatorItem)
                        {
                            // In case you just want to disable to option you can set Enabled false
                            //menuItem.Enabled = false;
                            menuItem.Visibility = Telerik.WinControls.ElementVisibility.Collapsed;
                        }
                    }
                }

                // MENU ITEM'S NAMES

                //Text                        Name
                //---------------------------------------------------
                //Close                       CloseWindow
                //Close All But This          CloseAllButThis
                //Close All                   CloseAll
                //New Horizontal Tab Group    NewHTabGroup
                //New Vertical Tab Group      NewVTabGroup
                //Floating                    Floating
                //Dockable                    Docked
                //Tabbed Document             TabbedDocument
                //Auto Hide                   AutoHide
                //Hide                        Hidden
                //Document Name               ActivateWindow


            }


            /// <summary>
            /// Change Float Behavior by Enabling/Disabling Dock Single Screen Mode
            ///     To enable the Visual Studio 2008-like docking behavior, set the SingleScreen property to true. 
            ///     Enabling this property will prevent document windows from floating and from docking inside existing floating windows
            /// </summary>
            public void ToggleSingleScreenFloatMode()
            {
                if (raddock.SingleScreen) { raddock.SingleScreen = false; sb("Disabled Single Screen Float Behavior"); }
                else { raddock.SingleScreen = true; sb("Enabled Single Screen Float Behavior"); }
            }

            /// <summary>
            /// Toggle Window Snapping Behavior for Floating Windows
            /// </summary>
            public void ToggleWindowSnapping()
            {
                if (raddock.EnableFloatingWindowSnapping) { raddock.EnableFloatingWindowSnapping = false; sb("Disabled Floating Window Snapping"); }
                else { raddock.EnableFloatingWindowSnapping = true; sb("Enabled Floating Window Snapping"); }
            }

            /// <summary>
            /// Close All RadDock Windows
            /// </summary>
            public void CloseAllWindows()
            {
                raddock.CloseAllWindows();
            }


            /// <summary>
            /// Create New Tool Window and Attach to Dock
            /// </summary>
            public void NewToolWindow()
            {
                ToolWindow toolWindow1 = new ToolWindow();
                toolWindow1.Text = "A ToolWindow";
                raddock.DockWindow(toolWindow1, DockPosition.Right);
            }


            #endregion


            //LaunchForm ln = new LaunchForm();
            //bool launched = ln.Launch(txt, false, "", CurrentForm);


            public void ToolWindowsDock(RadDock Dock)
            {
                ToolWindow windowTop = new ToolWindow();
                windowTop.Text = "Window Top";
                Dock.DockWindow(windowTop, DockPosition.Top);
            }

            public void DocumentWindowDock(RadDock Dock)
            {
                DocumentWindow documentTop = new DocumentWindow();
                documentTop.Text = "New Document";
                Dock.AddDocument(documentTop);
            }

            public void MulitpleWindowDock(RadDock Dock)
            {
                ToolWindow windowLeft = new ToolWindow();
                windowLeft.Text = "Window Left";
                Dock.DockWindow(windowLeft, DockPosition.Left);
                ToolWindow windowBottom = new ToolWindow();
                windowBottom.Text = "Window Bottom";
                Dock.DockWindow(windowBottom, DockPosition.Bottom);
                ToolWindow windowBottomRight = new ToolWindow();
                windowBottomRight.Text = "Window Bottom Right";
                Dock.DockWindow(windowBottomRight, windowBottom, DockPosition.Right);
                DocumentWindow document1 = new DocumentWindow();
                document1.Text = "Document 1";
                Dock.AddDocument(document1);
                DocumentWindow document2 = new DocumentWindow();
                document2.Text = "Document 2";
                Dock.AddDocument(document2);
                DocumentWindow document3 = new DocumentWindow();
                document3.Text = "Document 3";
                Dock.AddDocument(document3);
            }


            //private void menuItemTeamExplorer_Click1(object sender, EventArgs e)
            //{
            //    TeamExplorerUserControl teuc = new TeamExplorerUserControl();
            //    DockPosition dockTo = DockPosition.Right;
            //    HostWindow hw = this.radDock1.DockControl(teuc, dockTo);
            //    hw.Text = "Team Explorer";
            //}
            //private void menuItemServerExplorer_Click1(object sender, EventArgs e)
            //{
            //    ServerExplorerUserControl seuc = new ServerExplorerUserControl();
            //    DockPosition dockTo = DockPosition.Right;
            //    HostWindow hw = this.radDock1.DockControl(seuc, dockTo);
            //    hw.Text = "Server Explorer";
            //}
            //private void menuItemSolutionExplorer_Click1(object sender, EventArgs e)
            //{
            //    SolutionExplorerUserControl seuc = new SolutionExplorerUserControl();
            //    DockPosition dockTo = DockPosition.Right;
            //    HostWindow hw = this.radDock1.DockControl(seuc, dockTo);
            //    hw.Text = "Solution Explorer";
            //}



            //ToolTabStrip rightHandStrip = null;
            //private void menuItemTeamExplorer_Click(object sender, EventArgs e)
            //{
            //    TeamExplorerUserControl teuc = new TeamExplorerUserControl();
            //    ToolWindow teucW = new ToolWindow();
            //    teucW.Controls.Add(teuc);
            //    teucW.Text = "Team Explorer";
            //    if (rightHandStrip == null)
            //    {
            //        this.radDock1.DockWindow(teucW, DockPosition.Right);
            //        rightHandStrip = (ToolTabStrip)teucW.Parent;
            //    }
            //    else
            //    {
            //        this.radDock1.DockWindow(teucW, rightHandStrip, DockPosition.Fill);
            //    }
            //}
            //private void menuItemServerExplorer_Click(object sender, EventArgs e)
            //{
            //    ServerExplorerUserControl seuc = new ServerExplorerUserControl();
            //    ToolWindow seucW = new ToolWindow();
            //    seucW.Controls.Add(seuc);
            //    seucW.Text = "Server Explorer";
            //    if (rightHandStrip == null)
            //    {
            //        this.radDock1.DockWindow(seucW, DockPosition.Right);
            //        rightHandStrip = (ToolTabStrip)seucW.Parent;
            //    }
            //    else
            //    {
            //        this.radDock1.DockWindow(seucW, rightHandStrip, DockPosition.Fill);
            //    }
            //}
            //private void menuItemSolutionExplorer_Click(object sender, EventArgs e)
            //{
            //    SolutionExplorerUserControl seuc = new SolutionExplorerUserControl();
            //    ToolWindow seucW = new ToolWindow();
            //    seucW.Controls.Add(seuc);
            //    seucW.Text = "Solution Explorer";
            //    if (rightHandStrip == null)
            //    {
            //        this.radDock1.DockWindow(seucW, DockPosition.Right);
            //        rightHandStrip = (ToolTabStrip)seucW.Parent;
            //    }
            //    else
            //    {
            //        this.radDock1.DockWindow(seucW, rightHandStrip, DockPosition.Fill);
            //    }
            //}




        }

    }
}


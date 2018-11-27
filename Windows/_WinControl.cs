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

namespace sharpAHK_Dev
{
    public class _WinControl
    {
        _AHK ahk = new _AHK();

        #region === WinActions ===

        // [WinActions]

        public enum Actions
        {
            AlwaysOnTop = 1,
            WinSetBottom = 2,
            WinSetTop = 3,
            WinSetEnable = 4,
            WinSetDisable = 5,
            WinSetRedraw = 6,
            StatusBarWait = 7,
            WinActivate = 8,
            WinActivateBottom = 9,
            WinClose = 10,
            WinWaitClose = 11,
            WinHide = 12,
            WinShow = 13,
            WinMaximize = 14,
            WinMinimize = 15,
            WinKill = 16,
            WinMinimizeAll = 17,
            WinMinimizeAllUndo = 18,
            WinMove = 19,
            WinRestore = 20,
            WinSet = 21,
            WinSetTitle = 22,
            WinWait = 23,
            WinWaitActive = 24,
            WinWaitNotActive = 25
        }

        public void WinAction(winInfo Win, Actions AhkAction, string Param1 = "", string Param2 = "")
        {
            string WinTitle = Win.WinTitle;
            string WinText = Win.WinText;
            string ExcludeTitle = "";
            string ExcludeText = "";

            int SecondsToWait = 10;
            int WinX = Win.X;    // for WinMove 
            int WinY = Win.Y; 	// for WinMove 
            int WinW = Win.W;     // for WinMove 
            int WinH = Win.H; 	// for WinMove 

            string BarText = Param1;  // StatusBar Wait
            int PartNum = 0; // Param2.ToInt();      // StatusBar Wait

            string attribute = Param1; // winset
            string value = Param2; // winset
            string newWinTitle = Param2;  // winset

            if (AhkAction == Actions.AlwaysOnTop)
            {
                ahk.AlwaysOnTop(WinTitle, true, false, WinText, ExcludeTitle, ExcludeText);
            }

            if (AhkAction == Actions.WinSetBottom)
            {
                ahk.WinSetBottom(WinTitle);
            }

            if (AhkAction == Actions.WinSetTop)
            {
                ahk.WinSetTop(WinTitle, WinText, ExcludeTitle, ExcludeText);
            }

            if (AhkAction == Actions.WinSetEnable)
            {
                ahk.WinSetEnable(WinTitle, WinText, ExcludeTitle, ExcludeText);
            }

            if (AhkAction == Actions.WinSetDisable)
            {
                ahk.WinSetDisable(WinTitle, WinText, ExcludeTitle, ExcludeText);
            }

            if (AhkAction == Actions.WinSetRedraw)
            {
                ahk.WinSetRedraw(WinTitle, WinText, ExcludeTitle, ExcludeText);
            }

            if (AhkAction == Actions.StatusBarWait)
            {
                ahk.StatusBarWait(BarText, SecondsToWait, PartNum.ToString(), WinTitle, WinText, ExcludeTitle, ExcludeText);
            }

            if (AhkAction == Actions.WinActivate)
            {
                ahk.WinActivate(WinTitle, WinText, ExcludeTitle, ExcludeText);
            }

            if (AhkAction == Actions.WinActivateBottom)
            {
                ahk.WinActivateBottom(WinTitle, WinText, ExcludeTitle, ExcludeText);
            }

            if (AhkAction == Actions.WinClose)
            {
                ahk.WinClose(WinTitle, WinText, SecondsToWait.ToString(), ExcludeTitle, ExcludeText);
            }

            if (AhkAction == Actions.WinWaitClose)
            {
                ahk.WinWaitClose(WinTitle, WinText, SecondsToWait.ToString(), ExcludeTitle, ExcludeText);
            }

            if (AhkAction == Actions.WinHide)
            {
                ahk.WinHide(WinTitle, WinText, ExcludeTitle, ExcludeText);
            }

            if (AhkAction == Actions.WinShow)
            {
                ahk.WinShow(WinTitle, WinText, ExcludeTitle, ExcludeText);
            }

            if (AhkAction == Actions.WinMaximize)
            {
                ahk.WinMaximize(WinTitle, WinText, ExcludeTitle, ExcludeText);
            }

            if (AhkAction == Actions.WinMinimize)
            {
                ahk.WinMinimize(WinTitle, WinText, ExcludeTitle, ExcludeText);
            }

            if (AhkAction == Actions.WinKill)
            {
                ahk.WinKill(WinTitle, WinText, SecondsToWait.ToString(), ExcludeTitle, ExcludeText);
            }

            if (AhkAction == Actions.WinMinimizeAll)
            {
                ahk.WinMinimizeAll();
            }

            if (AhkAction == Actions.WinMinimizeAllUndo)
            {
                ahk.WinMinimizeAllUndo();
            }

            if (AhkAction == Actions.WinMove)
            {
                ahk.WinMove(WinTitle, WinText, WinH, WinY, WinW, WinH, ExcludeTitle, ExcludeText);
            }

            if (AhkAction == Actions.WinRestore)
            {
                ahk.WinRestore(WinTitle, WinText, ExcludeTitle, ExcludeText);
            }

            if (AhkAction == Actions.WinSet)
            {
                ahk.WinSet(attribute, value, WinTitle, WinText, ExcludeTitle, ExcludeText);
            }

            if (AhkAction == Actions.WinSetTitle)
            {
                ahk.WinSetTitle(WinTitle, WinText, newWinTitle, ExcludeTitle, ExcludeText);
            }

            if (AhkAction == Actions.WinWait)
            {
                ahk.WinWait(WinTitle, WinText, SecondsToWait.ToString(), ExcludeTitle, ExcludeText);
            }

            if (AhkAction == Actions.WinWaitActive)
            {
                ahk.WinWaitActive(WinTitle, WinText, SecondsToWait.ToString(), ExcludeTitle, ExcludeText);
            }

            if (AhkAction == Actions.WinWaitNotActive)
            {
                ahk.WinWaitNotActive(WinTitle, WinText, SecondsToWait.ToString(), ExcludeTitle, ExcludeText);
            }

        }

        //bool PostMessage(string Msg, string wParam = "", string lParam = "", string Control = "", string WinTitle = "", string WinText = "", string ExcludeTitle = "", string ExcludeText = "")
        //bool SendMessage(string Msg, string wParam = "", string lParam = "", string Control = "", string WinTitle = "", string WinText = "", string ExcludeTitle = "", string ExcludeText = "")
        //bool WinMenuSelectItem(string WinTitle, string WinText, string Menu, string SubMenu1 = "", string SubMenu2 = "", string SubMenu3 = "", string SubMenu4 = "", string SubMenu5 = "", string SubMenu6 = "", string ExcludeTitle = "", string ExcludeText = "")

        #endregion

        #region === WinInfo ===


        // [WinInfo]
        public enum Info
        {
            WinActive = 1,
            WinNotActive = 2,
            WinExist = 3,
            StatusBarGetText = 6,
            WinGetActiveID = 5,
            WinGetActiveTitle = 6,
            WinGetClass = 7,
            WinGetText = 8,
            WinGetPos = 9,
            WinGetTitle = 10,
            IsMinimized = 11,
            IsMaximized = 12,
            NotMinOrMax = 13,
            WinList = 14,
            AllInfo = 15
        }

        /// <summary>
        /// Populate WinInfo Object 
        /// </summary>
        /// <param name="Action"></param>
        /// <returns></returns>
        public winInfo WinInfo(winInfo win, Info Action)
        {
            string WinTitle = win.WinTitle;
            string WinText = win.WinText;
            string ExcludeTitle = win.ExcludeTitle;
            string ExcludeText = win.ExcludeText;

            string PartNum = win.StatusBarPart.ToString();  //StatusBarGetText

            if (Action == Info.WinExist || Action == Info.AllInfo)
            {
                win.Exists = ahk.WinExist(WinTitle, WinText, ExcludeTitle, ExcludeText);
            }
            if (Action == Info.WinActive || Action == Info.AllInfo)
            {
                win.Active = ahk.WinActive(WinTitle, WinText, ExcludeTitle, ExcludeText);
            }
            if (Action == Info.WinNotActive || Action == Info.AllInfo)
            {
                bool notActive = ahk.WinNotActive(WinTitle, WinText, ExcludeTitle, ExcludeText);
                if (notActive) { win.Active = false; } else { win.Active = true; }
            }
            if (Action == Info.StatusBarGetText || Action == Info.AllInfo)
            {
                win.StatusBarText = ahk.StatusBarGetText(PartNum, WinTitle, WinText, ExcludeTitle, ExcludeText);
            }
            if (Action == Info.WinGetActiveID || Action == Info.AllInfo)
            {
                win.AhkHwnd = ahk.WinGetActiveID(true);  // use AHK Method = true
            }
            if (Action == Info.WinGetActiveTitle || Action == Info.AllInfo)
            {
                win.WinTitle = ahk.WinGetActiveTitle(true);  // use AHK Method = true
            }
            if (Action == Info.WinGetClass || Action == Info.AllInfo)
            {
                win.Class = ahk.WinGetClass(WinTitle, WinText, ExcludeTitle, ExcludeText);
            }
            if (Action == Info.WinGetText || Action == Info.AllInfo)
            {
                win.WinText = ahk.WinGetText(WinTitle, WinText, ExcludeTitle, ExcludeText);
            }
            if (Action == Info.WinGetPos || Action == Info.AllInfo)
            {
                sharpAHK.winInfo WinI = ahk.WinGetPos(WinTitle, WinText, ExcludeTitle, ExcludeText);
                win.W = WinI.WinW;
                win.H = WinI.WinH;
                win.X = WinI.WinX;
                win.Y = WinI.WinY;
            }
            if (Action == Info.WinGetTitle || Action == Info.AllInfo)
            {
                win.WinTitle = ahk.WinGetTitle(WinTitle, WinText, ExcludeTitle, ExcludeText);
            }
            if (Action == Info.IsMinimized || Action == Info.AllInfo)
            {
                win.IsMin = ahk.IsMinimized(WinTitle, WinText, ExcludeTitle, ExcludeText);
            }
            if (Action == Info.IsMaximized || Action == Info.AllInfo)
            {
                win.IsMax = ahk.IsMaximized(WinTitle, WinText, ExcludeTitle, ExcludeText);
            }
            if (Action == Info.NotMinOrMax || Action == Info.AllInfo)
            {
                win.NotMinMax = ahk.NotMinOrMax(WinTitle, WinText, ExcludeTitle, ExcludeText);
            }
            if (Action == Info.WinList || Action == Info.AllInfo)
            {
                win.WinList = ahk.WinList();
            }
            if (Action == Info.WinList || Action == Info.AllInfo)
            {
                win.WinList = ahk.WinList();
            }

            return win;
        }


        /// <summary>
        /// Object Returning Window Details
        /// </summary>
        public class winInfo
        {
            /// <summary>
            /// Title of Window
            /// </summary>
            public string WinTitle { get; set; }
            /// <summary>
            /// Text Found in Window 
            /// </summary>
            public string WinText { get; set; }

            /// <summary>
            /// Window Handle ID Formatted for AutoHotkey Script Commands
            /// </summary>
            public string AhkHwnd { get; set; }
            /// <summary>
            /// Window PID Formatted for AutoHotkey Script Commands
            /// </summary>
            public string AhkPID { get; set; }
            /// <summary>
            /// Windows Class Formatted for AutoHotkey Script Commands
            /// </summary>
            public string AhkClass { get; set; }


            /// <summary>
            /// Window Class
            /// </summary>
            public string Class { get; set; }
            /// <summary>
            /// Window Handle
            /// </summary>
            public string Hwnd { get; set; }

            /// <summary>
            /// True IF Window Currently Minimized
            /// </summary>
            public bool IsMin { get; set; }
            /// <summary>
            /// True IF Window Currently Maximized
            /// </summary>
            public bool IsMax { get; set; }

            public bool NotMinMax { get; set; }

            /// <summary>
            /// Window X Coordinate
            /// </summary>
            public int X { get; set; }
            /// <summary>
            /// Window Y Coordinate
            /// </summary>
            public int Y { get; set; }
            /// <summary>
            /// Window Width
            /// </summary>
            public int W { get; set; }
            /// <summary>
            /// Window Height
            /// </summary>
            public int H { get; set; }

            /// <summary>
            /// List of all Windows matching search criteria
            /// </summary>
            public List<string> WinList { get; set; }
            /// <summary>
            /// Process Matching Search Criteria
            /// </summary>
            public Process process { get; set; }
            /// <summary>
            /// List of all Processes matching search criteria
            /// </summary>
            public List<Process> processes { get; set; }
            /// <summary>
            /// Number of Windows matching search criteria
            /// </summary>
            public int Count { get; set; }
            /// <summary>
            /// Window Process ID (PID)
            /// </summary>
            public string PID { get; set; }
            /// <summary>
            /// Process Name
            /// </summary>
            public string ProcessName { get; set; }

            /// <summary>
            /// List of Controls Found in Window
            /// </summary>
            public List<controlInfo> Controls { get; set; }

            public bool Active { get; set; }

            public bool Exists { get; set; }

            public string StatusBarText { get; set; }

            public int StatusBarPart { get; set; }

            public string ExcludeTitle { get; set; }
            public string ExcludeText { get; set; }

        }



        #endregion

        #region === ControlInfo ===

        public enum control
        {
            Control = 1,
            ControlClick = 2,
            ControlSend = 3,
            ControlSendRaw = 6,
            ControlFocus = 5,
            ControlGet = 7,
            ControlName = 8,
            ControlText = 9,
            ControlGetFocus = 10,
            ControlGetHwnd = 11,
            ControlGetText = 12,
            ControlSetText = 13,
            ControlMove = 14,
            PostMessage = 15,
            SendMessage = 16,
            WinMenuSelectItem = 17
        }

        // ToDo: Finish populating control actions / return control info 
        public controlInfo ControlActions(control Action)
        {
            controlInfo controlInf = new controlInfo();

            string Control = "";
            bool ReturnBool = false;

            string WinTitle = "";
            string WinText = "";


            // Control / WinMenuSelect
            string ExcludeTitle = ""; string ExcludeText = "";
            string Cmd = ""; // Control command
            string Value = ""; // Control command parameter

            string PostMessage = "";


            // WinMenuSelect
            string Menu = "";
            string SubMenu1 = "";
            string SubMenu2 = "";
            string SubMenu3 = "";
            string SubMenu4 = "";
            string SubMenu5 = "";
            string SubMenu6 = "";


            if (Action == control.Control)
            {
                ReturnBool = ahk.Control(Cmd, Value, Control, WinTitle, WinText, ExcludeTitle, ExcludeText);
            }

            //if (Action == control.ControlClick)
            //{
            //    ControlClick(string ControlOrPos = "", string WinTitle = "", string WinText = "", string WhichButton = "", string ClickCount = "", string Options = "", string ExcludeTitle = "", string ExcludeText = "");
            //}
            //if (Action == control.ControlSend)
            //{
            //    ControlSend(string Control = "", string Keys = "", string WinTitle = "", string WinText = "", string ExcludeTitle = "", string ExcludeText = "");
            //}
            //if (Action == control.ControlSendRaw)
            //{
            //    ControlSendRaw(string Control = "", string Keys = "", string WinTitle = "", string WinText = "", string ExcludeTitle = "", string ExcludeText = "");
            //}
            //if (Action == control.ControlFocus)
            //{
            //    ReturnBool = ahk.ControlFocus(string Control = "", string WinTitle = "", string WinText = "", string ExcludeTitle = "", string ExcludeText = "");
            //}
            //if (Action == control.ControlGet)
            //{
            //    string ControlGet(string Cmd, string Value = "", string Control = "", string WinTitle = "", string WinText = "", string ExcludeTitle = "", string ExcludeText = "");
            //}
            //if (Action == control.ControlGetFocus)
            //{
            //    /// <summary>Retrieves which control of the target window has input focus, if any.</summary>
            //    controlInf.ControlName = ControlGetFocus(string WinTitle = "", string WinText = "", string ExcludeTitle = "", string ExcludeText = "");
            //}
            //if (Action == control.ControlGetHwnd)
            //{
            //    controlInf.ControlHwnd = ahk.ControlGetHwnd(object Control = null, object WinTitle = null, string WinText = "", string ExcludeTitle = "", string ExcludeText = "");
            //}
            //if (Action == control.ControlGetText)
            //{
            //    controlInf.ControlText = ahk.ControlGetText(string Control = "", string WinTitle = "", string WinText = "", string ExcludeTitle = "", string ExcludeText = "");
            //}
            //if (Action == control.ControlSetText)
            //{
            //    ReturnBool = ahk.ControlSetText(string Control = "", string NewText = "", string WinTitle = "", string WinText = "", string ExcludeTitle = "", string ExcludeText = "");
            //}
            //if (Action == control.ControlMove)
            //{
            //    ReturnBool = ahk.ControlMove(string Control, string X, string Y, string W, string H, string WinTitle = "", string WinText = "", string ExcludeTitle = "", string ExcludeText = "");
            //}
            //if (Action == control.PostMessage)
            //{
            //    ReturnBool = ahk.PostMessage(string Msg, string wParam = "", string lParam = "", string Control = "", string WinTitle = "", string WinText = "", string ExcludeTitle = "", string ExcludeText = "");
            //}
            //if (Action == control.SendMessage)
            //{
            //    ReturnBool = ahk.SendMessage(string Msg, string wParam = "", string lParam = "", string Control = "", string WinTitle = "", string WinText = "", string ExcludeTitle = "", string ExcludeText = "");
            //}
            //if (Action == control.WinMenuSelectItem)
            //{
            //    ReturnBool = ahk.WinMenuSelectItem(string WinTitle, string WinText, string Menu, string SubMenu1 = "", string SubMenu2 = "", string SubMenu3 = "", string SubMenu4 = "", string SubMenu5 = "", string SubMenu6 = "", string ExcludeTitle = "", string ExcludeText = "");
            //}

            return controlInf;
        }

        public class controlInfo
        {
            /// <summary>
            /// Contains Details About Window Containing Control
            /// </summary>
            public winInfo Window { get; set; }

            public string ControlName { get; set; }
            public string ControlText { get; set; }

            /// <summary>
            /// List of Controls Found in Window
            /// </summary>
            public List<string> ControlClasseList { get; set; }

            public List<string> ControlHwndList { get; set; }

            /// <summary>
            /// Handle for Control
            /// </summary>
            public IntPtr ControlHwnd { get; set; }

        }


        #endregion

        #region === MouseInfo / MouseActions ===

        public enum mouse
        {
            Click = 1,
            MouseClick = 2,
            MouseClickDrag = 3,
            MouseGetPos = 6,
            MouseMove = 5
        }
        
        public sharpAHK.mousePos Mouse(mouse Action, int x = 0, int y = 0, int count = 1)
        {
            sharpAHK.mousePos pos = new sharpAHK.mousePos();

            string button = "Left";
            int y2 = 0;  // MouseClickDrag
            int x2 = 0; // MouseClickDrag
            string speed = "";  // mousemove / mouseclickdrag / mouseclick
            string R = ""; // mouseclickdrag


            if (Action == mouse.Click)
            {
                string Command = "Click " + x.ToString() + ", " + y.ToString();
                ahk.Click(Command);
            }

            if (Action == mouse.MouseClick)
            {
                ahk.MouseClick(_AHK.MouseButton.Left, x, y, true, count, speed,  _AHK.MouseState.None, false);
            }

            if (Action == mouse.MouseClickDrag)
            {
                ahk.MouseClickDrag(button, x.ToString(), y.ToString(), x2.ToString(), y2.ToString(), speed, R);
            }

            if (Action == mouse.MouseGetPos)
            {
                pos = ahk.MouseGetPos("");
            }

            if (Action == mouse.MouseMove)
            {
                bool OffSetFromCurrentPos = false;
                ahk.MouseMove(x, y, true, speed.ToInt(), OffSetFromCurrentPos);
            }

            return pos;
        }

        /// <summary>
        /// Object Returning Mouse Position and Related Info
        /// </summary>
        public class mouseInfo
        {
            /// <summary>
            /// Contains Details About Window Under Mouse
            /// </summary>
            public winInfo Window { get; set; }

            /// <summary>
            /// Current Mouse X Coordinate Position, Relative to Entire Screen
            /// </summary>
            public int MouseXScreen { get; set; }
            /// <summary>
            /// Current Mouse X Coordinate Position, Relative to Current Window
            /// </summary>
            public int MouseXWindow { get; set; }
            /// <summary>
            /// Current Mouse Y Coordinate Position, Relative to Entire Screen
            /// </summary>
            public int MouseYScreen { get; set; }
            /// <summary>
            /// Current Mouse Y Coordinate Position, Relative to Current Window
            /// </summary>
            public int MouseYWindow { get; set; }
            /// <summary>
            /// Name of the Control Detected Under Mouse
            /// </summary>
            public string ControlUnderMouse { get; set; }

        }


        #endregion

    }
}

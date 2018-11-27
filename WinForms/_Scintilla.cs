using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sharpAHK;
using sharpAHK_Dev;
using AHKExpressions;
using TelerikExpressions;
using System.Windows.Forms;
using System.Threading;
using ScintillaNET;
using System.Drawing;
using System.ComponentModel;

namespace sharpAHK_Dev
{
    /// <summary>
    /// Scintilla Control Function Library | LucidMedod 
    /// </summary>
    public class _ScintillaControl
    {
        [DefaultValue(CSharp)]
        public enum CodeType
        {
            AHK,
            CSharp,
            SQL
        }
        public void Setup(Scintilla sci, CodeType type, string KeyWords = "")
        {
            if (type == CodeType.CSharp) { SetupScintilla(sci, 2, KeyWords); }
            if (type == CodeType.AHK) { SetupScintilla(sci, 4, KeyWords); }
            if (type == CodeType.SQL) { SetupScintilla(sci, 5, KeyWords); }
        }

        #region === Scintilla ===

        //private static _AHK ahk { get; set; }
        private static _AHK ahk = new _AHK();

        /// <summary>Enable syntax highlighting and default start values</summary>
        /// <param name="sci">Scintilla Control Name</param>
        /// <param name="Ver">Configuration Version</param>
        public void SetupScintilla(Scintilla sci, int Ver = 1, string KeyWords = "")
        {
            if (sci == null) { return; }

            string AHKKeywords = "a_ahkpath a_ahkversion a_appdatacommon a_appdata a_autotrim a_batchlines a_carety a_caretx a_computername a_controldelay a_dd a_cursor a_ddd a_dddd a_desktop a_defaultmousespeed a_desktopcommon a_detecthiddentext a_endchar a_detecthiddenwindows a_eventinfo a_exitreason a_formatfloat a_formatinteger a_guievent a_gui a_guicontrol a_guicontrolevent a_guiheight a_guiwidth a_guix a_hour a_guiy a_iconfile a_iconhidden a_icontip a_iconnumber a_index a_ipaddress1 a_ipaddress2 a_ipaddress3 a_isadmin a_ipaddress4 a_iscompiled a_issuspended a_language a_keydelay a_iscritical a_isunicode errorlevel a_ptrsize a_lasterror a_linefile a_loopfield a_linenumber a_loopfileattrib a_loopfiledir  a_loopfileext a_loopfileshortname a_loopfileshortpath a_loopregsubkey a_loopfiletimecreated a_loopfileshortpath a_loopfilefullpath a_loopfilename a_loopfilelongpath a_loopfilesize a_loopfilesizekb a_loopfiletimeaccessed a_loopfilesizemb a_loopfiletimemodified a_loopreadline a_loopregname a_loopregkey a_loopregtimemodified a_loopregtype a_min a_mday a_mm a_mmm a_mon a_mmmm a_mousedelay a_msec a_mydocuments a_now a_numbatchlines a_nowutc a_ostype a_osversion a_programfiles a_priorhotkey a_programs  a_programscommon a_screenwidth a_screenheight a_scriptdir a_scriptfullpath a_scriptname a_sec a_space a_startmenucommon a_startmenu a_startup a_startupcommon a_tab a_stringcasesense a_thishotkey a_thismenu a_thismenuitempos a_thismenuitem a_tickcount a_timeidle  a_timeidlephysical a_timesincepriorhotkey a_timesincethishotkey a_titlematchmodespeed a_titlematchmode a_wday a_windelay a_workingdir a_windir a_yday a_year a_yyyy a_yweek clipboard clipboardall  comspec a_ispaused a_thisfunc a_thislabel programfiles a_temp a_username FALSE TRUE";
            string AHKKeywords2 = "Object() Array() Abs() AutoTrim Asc() ASin() ACos() ATan() BlockInput Break Catch Ceil() Chr() Click ClipWait ComObjActive() ComObjArray() ComObjConnect() ComObjCreate() ComObject() ComObjEnwrap() ComObjUnwrap() ComObjError() ComObjFlags() ComObjGet() ComObjMissing() ComObjParameter() ComObjQuery() ComObjType() ComObjValue() Continue Control ControlClick ControlFocus ControlGet ControlGetFocus ControlGetPos ControlGetText ControlMove ControlSend ControlSendRaw ControlSetText CoordMode Cos() Critical DetectHiddenText DetectHiddenWindows DllCall() Drive DriveGet DriveSpaceFree Edit Else EnvAdd EnvDiv EnvGet EnvMult EnvSet EnvSub EnvUpdate Exception() Exit ExitApp Exp() FileAppend FileCopy FileCopyDir FileCreateDir FileCreateShortcut FileDelete FileEncoding FileExist() FileInstall FileGetAttrib FileGetShortcut FileGetSize FileGetTime FileGetVersion FileMove FileMoveDir FileOpen FileRead FileReadLine FileRecycle FileRecycleEmpty FileRemoveDir FileSelectFile FileSelectFolder FileSetAttrib FileSetTime Finally Floor() Format FormatTime Func() GetKeyName() GetKeyVK() GetKeySC() GetKeyState Gosub Goto GroupActivate GroupAdd GroupClose GroupDeactivate Gui GuiControl GuiControlGet Hotkey Hotstring() if IfEqual IfNotEqual IfExist IfNotExist IfGreater IfGreaterOrEqual IfInString IfNotInString InStr() IfLess IfLessOrEqual IfMsgBox IfWinActive IfWinNotActive IfWinExist IfWinNotExist IL_Create() IL_Add() IL_Destroy() ImageSearch IniDelete IniRead IniWrite Input InputBox InStr() IsByRef() IsFunc() IsLabel() IsObject() KeyHistory KeyWait ListHotkeys ListLines ListVars LoadPicture() Log() Ln() Loop LV_Add() LV_Delete() LV_DeleteCol() LV_GetCount() LV_GetNext() LV_GetText() LV_Insert() LV_InsertCol() LV_Modify() LV_ModifyCol() LV_SetImageList() Max() Menu MenuGetHandle MenuGetName Min() Mod() MouseClick MouseClickDrag MouseGetPos MouseMove MsgBox NumGet() NumPut() ObjAddRef() ObjRelease() ObjBindMethod() ObjClone() ObjCount() ObjDelete() ObjGetAddress() ObjGetCapacity() ObjHasKey() ObjInsert() ObjInsertAt() ObjLength() ObjMaxIndex() ObjMinIndex() ObjNewEnum() ObjPop() ObjPush() ObjRemove() ObjRemoveAt() ObjSetCapacity() ObjGetBase() ObjRawGet() ObjRawSet() ObjSetBase() OnClipboardChange() OnError OnExit OnMessage() Ord() OutputDebug Pause PixelGetColor PixelSearch PostMessage Process Progress Random RegExMatch() RegExReplace() RegDelete RegRead RegWrite RegisterCallback() Reload Return Round() Run RunAs RunWait SB_SetIcon() SB_SetParts() SB_SetText() Send SendRaw SendInput SendPlay SendEvent SendLevel SendMessage SendMode SetBatchLines SetCapsLockState SetControlDelay SetDefaultMouseSpeed SetEnv SetFormat SetKeyDelay SetMouseDelay SetNumLockState SetScrollLockState SetRegView SetStoreCapsLockMode SetTimer SetTitleMatchMode SetWinDelay SetWorkingDir Shutdown Sin() Sleep Sort SoundBeep SoundGet SoundGetWaveVolume SoundPlay SoundSet SoundSetWaveVolume SplashImage SplashTextOn SplashTextOff SplitPath Sqrt() StatusBarGetText StatusBarWait StrPut() StrGet() StringCaseSense StringGetPos InStr() StringLeft StringLen StrLen() StringLower StringMid SubStr() StringReplace StrReplace() StringRight StringSplit StrSplit() StringTrimLeft StringTrimRight StringUpper Suspend SysGet Tan() Thread Throw ToolTip Transform TrayTip Trim() LTrim() RTrim() Try TV_Add() TV_Delete() TV_Get() TV_GetChild() TV_GetCount() TV_GetNext() TV_GetParent() TV_GetPrev() TV_GetSelection() TV_GetText() TV_Modify() TV_SetImageList() Until UrlDownloadToFile VarSetCapacity() While WinActivate WinActivateBottom WinActive() WinClose WinExist() WinGetActiveStats WinGetActiveTitle WinGetClass WinGet WinGetPos WinGetText WinGetTitle WinHide WinKill WinMaximize WinMenuSelectItem WinMinimize WinMinimizeAll WinMinimizeAllUndo WinMove WinRestore WinSet WinSetTitle WinShow WinWait WinWaitActive WinWaitClose WinWaitNotActive #ClipboardTimeout #CommentFlag #Delimiter #DerefChar #ErrorStdOut #EscapeChar #HotkeyInterval #HotkeyModifierTimeout #Hotstring #If #IfTimeout #IfWinActive #IfWinNotActive #IfWinExist #IfWinNotExist #Include #IncludeAgain #InputLevel #InstallKeybdHook #InstallMouseHook #KeyHistory #LTrim #MaxHotkeysPerInterval #MaxMem #MaxThreads #MaxThreadsBuffer #MaxThreadsPerHotkey #MenuMaskKey #NoEnv #NoTrayIcon #Persistent #SingleInstance #UseHook #Warn #WinActivateForce";


            if (Ver == 3)  // html viewer
            {
                // Configuring the default style with properties
                // we have common to every lexer style saves time.
                //sci.StyleResetDefault();
                sci.Styles[Style.Default].Font = "Consolas";
                sci.Styles[Style.Default].Size = 12;
                sci.StyleClearAll();

                // Configure the CPP (C#) lexer styles
                sci.Styles[Style.Cpp.Default].ForeColor = Color.Silver;
                sci.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(0, 128, 0); // Green
                sci.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0); // Green
                sci.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.FromArgb(128, 128, 128); // Gray
                sci.Styles[Style.Cpp.Number].ForeColor = Color.Olive;
                sci.Styles[Style.Cpp.Word].ForeColor = Color.Blue;
                sci.Styles[Style.Cpp.Word2].ForeColor = Color.Blue;
                sci.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
                sci.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21); // Red
                sci.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(163, 21, 21); // Red
                sci.Styles[Style.Cpp.StringEol].BackColor = Color.Pink;
                sci.Styles[Style.Cpp.Operator].ForeColor = Color.Purple;
                sci.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Maroon;
                //sci.Lexer = Lexer.Cpp;

                sci.Lexer = Lexer.Html;
                //sci.Lexer = Lexer.Pascal; 


                //sci.ShowGuides = true;
                //sci.IndentWidth = 5;
                sci.TabWidth = 5;
                sci.UseTabs = true;
                //sci.tab
                //sci.TabIndents = true;
                //sci.UseTabs = false;


                // Set the keywords (display as blue text in editor)
                sci.SetKeywords(0, "abstract as base break case catch checked continue default delegate do else event explicit extern false finally fixed for foreach goto if implicit in interface internal is lock namespace new null object operator out override params private protected public readonly ref return sealed sizeof stackalloc switch this throw true try typeof unchecked unsafe using virtual while ahk");
                sci.SetKeywords(1, "bool byte char class const decimal double enum float int long sbyte short static string struct uint ulong ushort void");

                sci.Margins[0].Width = 16; // option to expand width to display line numbers

                //if (!DisplayLineNum) { sci.Margins[0].Width = 0; } // option to expand width to display line numbers


                //====== code folding =============

                // Set the lexer
                //sci.Lexer = Lexer.Cpp;

                // Instruct the lexer to calculate folding
                sci.SetProperty("fold", "1");
                sci.SetProperty("fold.compact", "1");

                // Configure a margin to display folding symbols
                sci.Margins[2].Type = MarginType.Symbol;
                sci.Margins[2].Mask = Marker.MaskFolders;
                sci.Margins[2].Sensitive = true;
                sci.Margins[2].Width = 20;

                // Set colors for all folding markers
                for (int i = 25; i <= 31; i++)
                {
                    sci.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                    sci.Markers[i].SetBackColor(SystemColors.ControlDark);
                }

                // Configure folding markers with respective symbols
                sci.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
                sci.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
                sci.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
                sci.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
                sci.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
                sci.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
                sci.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

                // Enable automatic folding
                sci.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);

            }

            if (Ver == 1)
            {
                // Configuring the default style with properties
                // we have common to every lexer style saves time.
                //sci.StyleResetDefault();
                sci.Styles[Style.Default].Font = "Consolas";
                sci.Styles[Style.Default].Size = 10;
                sci.Styles[Style.Default].BackColor = Color.Wheat;

                sci.StyleClearAll();

                // Configure the CPP (C#) lexer styles
                sci.Styles[Style.Cpp.Default].ForeColor = Color.Silver;
                sci.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(0, 128, 0); // Green
                sci.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0); // Green
                sci.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.FromArgb(128, 128, 128); // Gray
                sci.Styles[Style.Cpp.Number].ForeColor = Color.Olive;
                sci.Styles[Style.Cpp.Word].ForeColor = Color.Blue;
                sci.Styles[Style.Cpp.Word2].ForeColor = Color.Blue;
                sci.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
                sci.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21); // Red
                sci.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(163, 21, 21); // Red
                sci.Styles[Style.Cpp.StringEol].BackColor = Color.Pink;
                sci.Styles[Style.Cpp.Operator].ForeColor = Color.Purple;
                sci.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Maroon;
                sci.Lexer = Lexer.Cpp;

                //sci.Lexer = Lexer.Html;
                //sci.Lexer = Lexer.Pascal; 


                //sci.ShowGuides = true;
                //sci.IndentWidth = 5;
                sci.TabWidth = 5;
                sci.UseTabs = true;
                //sci.tab
                //sci.TabIndents = true;
                //sci.UseTabs = false;


                // Set the keywords (display as blue text in editor)
                sci.SetKeywords(0, "abstract as base break case catch checked continue default delegate do else event explicit extern false finally fixed for foreach goto if implicit in interface internal is lock namespace new null object operator out override params private protected public readonly ref return sealed sizeof stackalloc switch this throw true try typeof unchecked unsafe using virtual while ahk");
                sci.SetKeywords(1, "bool byte char class const decimal double enum float int long sbyte short static string struct uint ulong ushort void " + KeyWords);

                sci.Margins[0].Width = 16; // option to expand width to display line numbers

                //if (!DisplayLineNum) { sci.Margins[0].Width = 0; } // option to expand width to display line numbers


                //====== code folding =============

                // Set the lexer
                sci.Lexer = Lexer.Cpp;

                // Instruct the lexer to calculate folding
                sci.SetProperty("fold", "1");
                sci.SetProperty("fold.compact", "1");

                // Configure a margin to display folding symbols
                sci.Margins[2].Type = MarginType.Symbol;
                sci.Margins[2].Mask = Marker.MaskFolders;
                sci.Margins[2].Sensitive = true;
                sci.Margins[2].Width = 20;

                // Set colors for all folding markers
                for (int i = 25; i <= 31; i++)
                {
                    sci.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                    sci.Markers[i].SetBackColor(SystemColors.ControlDark);
                }

                // Configure folding markers with respective symbols
                sci.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
                sci.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
                sci.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
                sci.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
                sci.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
                sci.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
                sci.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

                // Enable automatic folding
                sci.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);

            }

            if (Ver == 2) // c#
            {
                // Scintilla 2

                sci.StyleResetDefault();
                sci.Styles[Style.Default].Font = "Consolas";
                sci.Styles[Style.Default].Size = 10;
                sci.StyleClearAll();

                // Configure the CPP (C#) lexer styles
                sci.Styles[Style.Cpp.Default].ForeColor = Color.Silver;
                sci.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(0, 128, 0); // Green
                sci.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0); // Green
                sci.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.FromArgb(128, 128, 128); // Gray
                sci.Styles[Style.Cpp.Number].ForeColor = Color.Olive;
                sci.Styles[Style.Cpp.Word].ForeColor = Color.Blue;
                sci.Styles[Style.Cpp.Word2].ForeColor = Color.Blue;
                sci.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
                sci.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21); // Red
                sci.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(163, 21, 21); // Red
                sci.Styles[Style.Cpp.StringEol].BackColor = Color.Pink;
                sci.Styles[Style.Cpp.Operator].ForeColor = Color.Purple;
                sci.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Maroon;

                sci.SetKeywords(0, "select where from delete update");
                sci.SetKeywords(1, "abstract as base break case catch checked continue default delegate do else event explicit extern false finally fixed for foreach goto if implicit in interface internal is lock namespace new null object operator out override params private protected public readonly ref return sealed sizeof stackalloc switch this throw true try typeof unchecked unsafe using virtual while ahk");
                sci.SetKeywords(2, "bool byte char class const decimal double enum float int long sbyte short static string struct uint ulong ushort void");
                sci.Lexer = Lexer.Cpp;
            }

            // AHK Version
            if (Ver == 4)
            {
                DisplayLineNum = false;


                // Configuring the default style with properties
                // we have common to every lexer style saves time.
                //sci.StyleResetDefault();
                sci.Styles[Style.Default].Font = "Consolas";
                sci.Styles[Style.Default].Size = 11;
                sci.Styles[Style.Default].BackColor = Color.White;

                sci.StyleClearAll();

                // Configure the CPP (C#) lexer styles
                sci.Styles[Style.Cpp.Default].ForeColor = Color.Silver;
                sci.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(0, 128, 0); // Green
                sci.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0); // Green
                sci.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.FromArgb(0, 128, 0); // Green
                sci.Styles[Style.Cpp.Number].ForeColor = Color.Olive;
                sci.Styles[Style.Cpp.Word].ForeColor = Color.Blue;
                sci.Styles[Style.Cpp.Word2].ForeColor = Color.Blue;
                sci.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
                sci.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21); // Red
                sci.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(163, 21, 21); // Red
                sci.Styles[Style.Cpp.StringEol].BackColor = Color.Pink;
                sci.Styles[Style.Cpp.Operator].ForeColor = Color.Purple;
                sci.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Maroon;
                sci.Lexer = Lexer.Cpp;

                //sci.Lexer = Lexer.Html;
                //sci.Lexer = Lexer.Pascal; 


                //sci.ShowGuides = true;
                //sci.IndentWidth = 5;
                sci.TabWidth = 5;
                sci.UseTabs = true;
                //sci.tab
                //sci.TabIndents = true;
                //sci.UseTabs = false;


                // Set the keywords (display as blue text in editor)
                sci.SetKeywords(0, AHKKeywords);
                sci.SetKeywords(1, AHKKeywords2);


                if (!DisplayLineNum) { sci.Margins[0].Width = 0; } // option to expand width to display line numbers
                else { sci.Margins[0].Width = 16; }


                //====== code folding =============

                // Set the lexer
                sci.Lexer = Lexer.Cpp;

                // Instruct the lexer to calculate folding
                sci.SetProperty("fold", "1");
                sci.SetProperty("fold.compact", "1");

                // Configure a margin to display folding symbols
                sci.Margins[2].Type = MarginType.Symbol;
                sci.Margins[2].Mask = Marker.MaskFolders;
                sci.Margins[2].Sensitive = true;
                sci.Margins[2].Width = 20;

                // Set colors for all folding markers
                for (int i = 25; i <= 31; i++)
                {
                    sci.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                    sci.Markers[i].SetBackColor(SystemColors.ControlDark);
                }

                // Configure folding markers with respective symbols
                sci.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
                sci.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
                sci.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
                sci.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
                sci.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
                sci.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
                sci.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

                // Enable automatic folding
                sci.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);

            }

            // SQL Version
            if (Ver == 5)
            {
                DisplayLineNum = false;


                // Configuring the default style with properties
                // we have common to every lexer style saves time.
                //sci.StyleResetDefault();
                sci.Styles[Style.Default].Font = "Consolas";
                sci.Styles[Style.Default].Size = 11;
                sci.Styles[Style.Default].BackColor = Color.White;

                sci.StyleClearAll();

                // Configure the CPP (C#) lexer styles
                sci.Styles[Style.Cpp.Default].ForeColor = Color.Silver;
                sci.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(0, 128, 0); // Green
                sci.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0); // Green
                sci.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.FromArgb(0, 128, 0); // Green
                sci.Styles[Style.Cpp.Number].ForeColor = Color.Olive;
                sci.Styles[Style.Cpp.Word].ForeColor = Color.Blue;
                sci.Styles[Style.Cpp.Word2].ForeColor = Color.Blue;
                sci.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21); // Red
                sci.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21); // Red
                sci.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(163, 21, 21); // Red
                sci.Styles[Style.Cpp.StringEol].BackColor = Color.Pink;
                sci.Styles[Style.Cpp.Operator].ForeColor = Color.Purple;
                sci.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Maroon;
                sci.Lexer = Lexer.Cpp;

                //sci.Lexer = Lexer.Html;
                //sci.Lexer = Lexer.Pascal; 


                //sci.ShowGuides = true;
                //sci.IndentWidth = 5;
                sci.TabWidth = 5;
                sci.UseTabs = true;
                //sci.tab
                //sci.TabIndents = true;
                //sci.UseTabs = false;


                // Set the keywords (display as blue text in editor)
                sci.SetKeywords(0, AHKKeywords);
                sci.SetKeywords(1, AHKKeywords2);


                if (!DisplayLineNum) { sci.Margins[0].Width = 0; } // option to expand width to display line numbers
                else { sci.Margins[0].Width = 16; }


                //====== code folding =============

                // Set the lexer
                sci.Lexer = Lexer.Sql;

                // Instruct the lexer to calculate folding
                sci.SetProperty("fold", "1");
                sci.SetProperty("fold.compact", "1");

                // Configure a margin to display folding symbols
                sci.Margins[2].Type = MarginType.Symbol;
                sci.Margins[2].Mask = Marker.MaskFolders;
                sci.Margins[2].Sensitive = true;
                sci.Margins[2].Width = 20;

                // Set colors for all folding markers
                for (int i = 25; i <= 31; i++)
                {
                    sci.Markers[i].SetForeColor(SystemColors.ControlLightLight);
                    sci.Markers[i].SetBackColor(SystemColors.ControlDark);
                }

                // Configure folding markers with respective symbols
                sci.Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
                sci.Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
                sci.Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
                sci.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
                sci.Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
                sci.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
                sci.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

                // Enable automatic folding
                sci.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);

            }

        }


        /// <summary>Populate Scintilla Control from List, From Any Thread</summary>
        /// <param name="sci">Scintilla Control Name</param>
        /// <param name="list">List To Populate Scintilla Control</param>
        public void LoadList(Scintilla sci, List<string> list)
        {
            string text = "";
            int i = 0;
            foreach (string line in list)
            {
                if (i == 0) { text = line; }
                if (i != 0) { text = text + "\n" + line; }
                i++;
            }

            UpdateScintilla(sci, text);
        }

        /// <summary>Fold Text Brackets</summary>
        /// <param name="sci">Scintilla Control Name</param>
        public void FoldALL(Scintilla sci)
        {
            // update control text (from any thread) -- [ works in dll ]
            if (sci.InvokeRequired) { sci.BeginInvoke((MethodInvoker)delegate () { sci.FoldAll(FoldAction.Contract); }); }
            else { sci.FoldAll(FoldAction.Contract); }
        }

        /// <summary>Unfold All Bracked Code</summary>
        /// <param name="sci">Scintilla Control Name</param>
        public void UnFold(Scintilla sci)
        {
            // update control text (from any thread) -- [ works in dll ]
            if (sci.InvokeRequired) { sci.BeginInvoke((MethodInvoker)delegate () { sci.FoldAll(FoldAction.Expand); }); }
            else { sci.FoldAll(FoldAction.Expand); }
        }

        /// <summary>Returns current line number in control</summary>
        /// <param name="sci">Scintilla Control Name</param>
        public int CurrentLine(Scintilla sci)
        {
            int current = -1;

            // update control text (from any thread) -- [ works in dll ]
            if (sci.InvokeRequired) { sci.BeginInvoke((MethodInvoker)delegate () { current = sci.CurrentLine; }); }
            else { current = sci.CurrentLine; }

            return current;
        }

        /// <summary>Update text from any thread</summary>
        /// <param name="sci">Scintilla Control Name</param>
        /// <param name="Text"> </param>
        public void Text(Scintilla sci, string Text)
        {
            // update control text (from any thread) -- [ works in dll ]
            if (sci.InvokeRequired) { sci.BeginInvoke((MethodInvoker)delegate () { sci.Text = Text; }); }
            else { sci.Text = Text; }
        }

        /// <summary>Update Scintilla Control (From Any Thread)</summary>
        /// <param name="sci">Scintilla Control Name</param>
        /// <param name="Text"> </param>
        public void UpdateScintilla(Scintilla sci, string Text)
        {
            // update control text (from any thread) -- [ works in dll ]
            if (sci.InvokeRequired) { sci.BeginInvoke((MethodInvoker)delegate () { sci.Text = Text; }); }
            else { sci.Text = Text; }
        }

        /// <summary>Returns selected text in control</summary>
        /// <param name="sci">Scintilla Control Name</param>
        public string SelectedText(Scintilla sci)
        {
            string Text = sci.SelectedText.ToString();
            //ahk.MsgBox(Text);
            return Text;
        }

        /// <summary>Scintilla - Read File and Populate Control</summary>
        /// <param name="sci">Scintilla Control Name</param>
        /// <param name="FileName"> </param>
        public void OpenFile(Scintilla sci, string FileName)
        {
            string Code = ahk.FileRead(FileName);
            UpdateScintilla(sci, Code);  // update control with new text
            //this.Text = AppTitle + " - " + FileName;  // update app title with filename loaded
        }

        /// <summary>
        /// Windows File Prompt To Select File to Load in NotePad
        /// </summary>
        /// <param name="sci"></param>
        public void OpenFilePrompt(Scintilla sci)
        {
            string FilePath = ahk.FileSelectFile(); 
            if (FilePath.Trim() != "")
            {
                OpenFile(sci, FilePath); 
            }
        }


        public void ScintillaActions(Scintilla sci, string Action, string Params = "")
        {
            if (Action == "Open File") { OpenFilePrompt(sci); }
            if (Action == "Fold") { FoldALL(sci); }
            if (Action == "UnFold") { UnFold(sci); }

        }



        // example to change the margin width dynamically to adjust for larger #s of line numbers being displayed
        private int maxLineNumberCharLength;

        bool DisplayLineNum = true;

        /// <summary>Scintilla Text Changed Event</summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        public void scintilla_TextChanged(object sender, EventArgs e)
        {
            Scintilla sci = ((Scintilla)sender);
            if (DisplayLineNum)
            {
                // Did the number of characters in the line number display change?
                // i.e. nnn VS nn, or nnnn VS nn, etc...
                var maxLineNumberCharLength = sci.Lines.Count.ToString().Length;
                if (maxLineNumberCharLength == this.maxLineNumberCharLength)
                    return;

                // Calculate the width required to display the last line number
                // and include some padding for good measure.
                const int padding = 2;
                sci.Margins[0].Width = sci.TextWidth(Style.LineNumber, new string('9', maxLineNumberCharLength + 1)) + padding;
                this.maxLineNumberCharLength = maxLineNumberCharLength;
            }
        }

        /// <summary>on change event to capture the current cursor position to display on gui elsewhere</summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        public void scintilla_UpdateUI(object sender, UpdateUIEventArgs e)
        {
            Scintilla sci = ((Scintilla)sender);
            if ((e.Change & UpdateChange.Selection) > 0)
            {
                // The caret/selection changed
                var currentPos = sci.CurrentPosition;
                var anchorPos = sci.AnchorPosition;
                var wrd = sci.GetWordFromPosition(currentPos);

                //toolStripStatusLabel.Text = "Ch: " + currentPos + " Sel: " + Math.Abs(anchorPos - currentPos);
                //StatusBar("Ch: " + currentPos + " Sel: " + Math.Abs(anchorPos - currentPos) + " Word = " + wrd); 
            }
        }

        /// <summary>Scintilla - toggle displaying line display</summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        public void Toggle_LineDisplay(object sender, EventArgs e)
        {
            Scintilla sci = ((Scintilla)sender);

            // toggle displaying line numbers on gui
            if (DisplayLineNum)
            {
                sci.Margins[0].Width = 0; // hide the margin
                DisplayLineNum = false;
                return;
            }
            if (!DisplayLineNum)
            {
                sci.Margins[0].Width = 30; // set the initial width value
                DisplayLineNum = true;
                scintilla_TextChanged(sender, e);
                return;
            }
        }


        /// <summary>
        /// Launches New Scinilla Dialog with Ability to Edit / Save Wordlists
        /// </summary>
        /// <param name="sci">Scintilla Control</param>
        /// <param name="Text">Optional Text to Populate Control</param>
        public void WordListEditor(Scintilla sci, string Text = "")
        {
            sharpAHK_Dev.Controls.scintillaWords form = new sharpAHK_Dev.Controls.scintillaWords(sci, Text);
            form.Show();
        }


        /// <summary>Highlight List of Words in Scintilla Control</summary>
        /// <param name="sci">Scintilla Control Name</param>
        /// <param name="WordList">List of Words To Highlight</param>
        /// <param name="color">Color To Change List Words To</param>
        /// <param name="ClearHighlights">Option To Clear Previous Highlighting Before Adding New</param>
        public void HighlightWordList(Scintilla sci, List<string> WordList, Color color, bool ClearHighlights = true)
        {
            int i = 0;
            foreach (string word in WordList)
            {
                if (i == 0) { HighlightWord(sci, word, color, ClearHighlights); }  // clear existing highlights on first highlight if enabled
                if (i != 0) { HighlightWord(sci, word, color, false); }  // add additional highlights
                i++;
            }
        }

        /// <summary>Highlight a specific search word in the scintilla control</summary>
        /// <param name="sci">Scintilla Control Name</param>
        /// <param name="text">Word To Highlight in Scintilla Control</param>
        /// <param name="color">Color To Change Word To</param>
        /// <param name="ClearHighlights">Option To Clear Previous Highlighting Before Adding New</param>
        public void HighlightWord(Scintilla sci, string text, Color color, bool ClearHighlights = true)
        {
            // Indicators 0-7 could be in use by a lexer
            // so we'll use indicator 8 to highlight words.
            const int NUM = 8;

            sci.IndicatorCurrent = NUM;

            // Remove all uses of our indicator
            if (ClearHighlights) { sci.IndicatorClearRange(0, sci.TextLength); }


            // Update indicator appearance
            sci.Indicators[NUM].Style = IndicatorStyle.StraightBox;
            sci.Indicators[NUM].Under = true;
            sci.Indicators[NUM].ForeColor = color;
            sci.Indicators[NUM].OutlineAlpha = 50;
            //sci.Indicators[NUM].Alpha = 30;
            sci.Indicators[NUM].Alpha = 60;

            // Search the document
            sci.TargetStart = 0;
            sci.TargetEnd = sci.TextLength;
            sci.SearchFlags = SearchFlags.None;
            while (sci.SearchInTarget(text) != -1)
            {
                // Mark the search results with the current indicator
                sci.IndicatorFillRange(sci.TargetStart, sci.TargetEnd - sci.TargetStart);

                // Search the remainder of the document
                sci.TargetStart = sci.TargetEnd;
                sci.TargetEnd = sci.TextLength;
            }
        }

        /// <summary>Clear Highlights From Scintilla Control</summary>
        /// <param name="sci">Scintilla Control Name</param>
        public void ClearHighlights(Scintilla sci)
        {
            // Remove all uses of our indicator
            sci.IndicatorClearRange(0, sci.TextLength);
        }



        /// <summary>Enable WordWrap mode on Scintilla control</summary>
        /// <param name="sci">Scintilla Control Name</param>
        /// <param name="On">Enable WordWrap Mode</param>
        /// <param name="Opt">Opt = WhiteSpace | Char</param>
        public void WordWrap(Scintilla sci, bool On = true, string Opt = "")
        {
            if (On) { sci.WrapMode = ScintillaNET.WrapMode.Word; }
            if (!On) { sci.WrapMode = ScintillaNET.WrapMode.None; }
            if (Opt.ToUpper() == "WHITESPACE") { sci.WrapMode = ScintillaNET.WrapMode.Whitespace; }
            if (Opt.ToUpper() == "CHAR") { sci.WrapMode = ScintillaNET.WrapMode.Char; }
        }


        /// <summary>UNTESTED - hide lines in scintilla control</summary>
        private void Hide_Lines()
        {
            //if (cShowProcess.SelectedItem != null)
            //{
            //    for (int i = 0; i < scintilla1.Lines.Count; i++)
            //    {
            //        if (!scintilla1.Lines[i].Text.Contains(cShowProcess.SelectedItem.ToString()))
            //        {
            //            scintilla1.Lines.Hide(scintilla1.Lines[i].Number, scintilla1.Lines[i].Number);
            //            //scintilla1.Lines[i].IsVisible = false;
            //            continue;
            //        }
            //    }
            //}
        }

        /// <summary>Scintilla Examples</summary>
        /// <param name="sci">Scintilla Control Name</param>
        public void Scintilla_TextModExamples(Scintilla sci)
        {
            sci.Text = "Hello";
            sci.AppendText(" World"); // 'Hello' -> 'Hello World'
            sci.DeleteRange(0, 5); // 'Hello World' -> ' World'
            sci.InsertText(0, "Goodbye"); // ' World' -> 'Goodbye World'

            // Get the first 256 characters of the document
            var text = sci.GetTextRange(0, Math.Min(256, sci.TextLength));
            Console.WriteLine(text);

            sci.HideLines(3, 12);  // hide lines

            // find and replace plugin 
            //https://github.com/Stumpii/ScintillaNET-FindReplaceDialog
        }

        /// <summary>Zoom In Scintilla Control</summary>
        /// <param name="sci">Scintilla Control Name</param>
        public void ZoomIn(Scintilla sci)
        {
            sci.ZoomIn();
        }

        /// <summary>Zoom Out Scintilla Control</summary>
        /// <param name="sci">Scintilla Control Name</param>
        public void ZoomOut(Scintilla sci)
        {
            sci.ZoomOut();
        }


        #region === Scintilla AutoComplete ===

        public ScintillaNET.Scintilla selectedScintilla;

        public List<string> autoCompleteList = new List<string>();


        public void Save_AutoCompleteList(object ListValues, string ListName = "Scintilla_AutoComplete")
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            sqlite.List(ListName, ListValues);
        }

        public List<string> Return_AutoCompleteList(string ListName = "Scintilla_AutoComplete")
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            return sqlite.List(ListName);
        }

        /// <summary>
        /// Define AutoComplete list for Scintilla Control. Provide either string or List of words
        /// </summary>
        /// <param name="scintilla"></param>
        /// <param name="words"></param>
        /// <param name="Words"></param>
        public void Setup_AutoComplete(ScintillaNET.Scintilla scintilla, string words = "", List<string> Words = null)
        {
            if (scintilla != null)
            {
                selectedScintilla = scintilla;
                selectedScintilla.CharAdded += new System.EventHandler<ScintillaNET.CharAddedEventArgs>(selectedScintilla_CharAdded);
            }

            _Lists lst = new _Lists();
            if (Words != null) { autoCompleteList = Words; }
            else if (words != "")
            {
                List<string> newWords = lst.Text_To_List(words, true, true, false);
                foreach(string w in newWords)
                {
                    autoCompleteList.Add(w);
                }
            }
            else
            {
                autoCompleteList = Return_AutoCompleteList("Scintilla_AutoComplete");
            }
        }


        /// <summary> Tested ? Working ? </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void selectedScintilla_CharAdded(object sender, CharAddedEventArgs e)
        {
            // Find the word start
            var currentPos = selectedScintilla.CurrentPosition;
            var wordStartPos = selectedScintilla.WordStartPosition(currentPos, true);
            //string AutoCompleteList = "Gen_SQLite Gen_TreeView Gen_Grid List<int> List<TreeNode>";  // list of words to offer autocomplete options

            //string AutoCompleteString = String.Join(" ", AutoCompleteList.ToArray());

            selectedScintilla.AutoCAutoHide = false;  // hide popup after text is ruled out

            string trimmedWordList = "";
            // Display the autocompletion list
            var lenEntered = currentPos - wordStartPos;
            if (lenEntered > 0)
            {
                selectedScintilla.AutoCIgnoreCase = true;
                selectedScintilla.AutoCChooseSingle = false;
                selectedScintilla.AutoCMaxHeight = 10;
                selectedScintilla.AutoCDropRestOfWord = true;

                string SoFarText = selectedScintilla.GetWordFromPosition(wordStartPos);  // word typed so far
                //ahk.MsgBox(Text); 

                //string[] words = AutoCompleteList.Split(' ');
                string[] words = autoCompleteList.ToArray();

                foreach (string word in words)
                {
                    string TextInList = ahk.FirstCharacters(word, lenEntered); // returns first X characters in string
                    if (TextInList.ToUpper() == SoFarText.ToUpper()) { trimmedWordList = trimmedWordList + " " + word; }
                    //if (TextInList.ToUpper().Contains(SoFarText.ToUpper())) { trimmedWordList = trimmedWordList + " " + word; }
                }
                selectedScintilla.AutoCShow(lenEntered, trimmedWordList.Trim());

                //if (lenEntered > 3) { scintilla1.AutoCComplete(); }  // autocomplete past X characters 

                selectedScintilla.AutoCCompleted += new EventHandler<AutoCSelectionEventArgs>(selectedScintilla_AutoCompleteAccepted);
                hit = false;

                //scintilla1.AutoCShow(lenEntered, AutoCompleteList);
                //     regex pattern matching and highlighting - untested
                //foreach (Match m in Patterns.Keyword0.Matches(Encoding.ASCII.GetString(e.RawText)))
                //   e.GetRange(m.Index, m.Index + m.Length).SetStyle(1);
            }
        }

        bool hit = false;

        /// <summary> Tested ? Working ? </summary>
        /// <param name="sender"> </param>
        /// <param name="e"> </param>
        private void selectedScintilla_AutoCompleteAccepted(object sender, AutoCSelectionEventArgs e)
        {
            if (!hit)
            {
                //ahk.MsgBox(e.Text.ToString());
                hit = true;

                var currentPos = selectedScintilla.CurrentPosition;
                var wordStartPos = selectedScintilla.WordStartPosition(currentPos, true);
                string SoFarText = selectedScintilla.GetWordFromPosition(wordStartPos);  // word typed so far

                var lenEntered = currentPos - wordStartPos;
                selectedScintilla.DeleteRange(wordStartPos, lenEntered);

                string NewPaste = SoFarText;

                //if (SoFarText == "List_String") { NewPaste = "List<string> list = new List<string>();\n"; }
                //if (SoFarText == "SQLiteGen") { ahk.MsgBox("Started Tool"); }
                selectedScintilla.InsertText(selectedScintilla.CurrentPosition, NewPaste);
                //selectedScintilla.CurrentPosition()
                //selectedScintilla.CurrentPosition(selectedScintilla.CurrentPosition + NewPaste.Length); 
                //int currentline = scintilla1.CurrentLine;
                selectedScintilla.GotoPosition(selectedScintilla.CurrentPosition + NewPaste.Length); 
                //ahk.MsgBox("Pasted: " + SoFarText); 
            }
        }


        #endregion


        #endregion




    }
}

using ScintillaNET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;
using Telerik.WinControls.UI;
using TelerikExpressions;

namespace sharpAHK_Dev.Controls
{
    public partial class scintillaWords : Telerik.WinControls.UI.RadForm
    {
        public scintillaWords(ScintillaNET.Scintilla scintilla, string words = "")
        {
            InitializeComponent();
            this.Text = "Scintilla CONTROL: " + scintilla.Name;
            if (words != "") { scintilla1.Text = words; }

            Build_Scintilla_Menu(radMenu1, scintilla);

            HighlightColor = Color.Yellow;

            btnColor.BackColor = HighlightColor;
            btnColor.Text = HighlightColor.Name.ToString();
        }

        public ScintillaNET.Scintilla selectedScintilla; 

        private void btnApply_Click(object sender, EventArgs e)
        {
            _ScintillaControl sci = new _ScintillaControl();
            _Lists lst = new _Lists();
            List<string> WordList = lst.Text_To_List(scintilla1.Text, true, true, false);
            sci.HighlightWordList(selectedScintilla, WordList, HighlightColor, cbClearExisting.Checked);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _ScintillaControl sci = new _ScintillaControl();
            sci.ClearHighlights(selectedScintilla);
            //this.Close();
        }



        #region === Scintilla Menu ===

        public void Build_Scintilla_Menu(RadMenu radMenu, Scintilla scintilla)  // build scintilla menu items from list
        {
            List<string> menuItems = new List<string> { "Clear", "Fold", "UnFold", "ZoomIn", "ZoomOut" };
            _TelerikLib._RadMenu menu = new _TelerikLib._RadMenu();
            menu.RadMenu_FromList(menuItems, "Scintilla", menuScintilla_Click, radMenu);

            List<string> menuItems2 = new List<string> { "Load AutoComplete List", "Apply AutoComplete List", "Save AutoComplete List", "Load Keyword List", "Apply Keyword List", "Save Keyword List" };
            menu.RadMenu_FromList(menuItems2, "Lists", menuScintilla_Click, radMenu);


            Current(scintilla);
        }

        private void menuScintilla_Click(object sender, EventArgs e)
        {
            RadMenuItem clicked = (RadMenuItem)sender; string txt = clicked.Text;

            if (txt == "Clear") { selectedScintilla.Text = ""; }
            if (txt == "Fold") { selectedScintilla.Fold(); }
            if (txt == "UnFold") { selectedScintilla.UnFold(); }
            if (txt == "ZoomIn") { selectedScintilla.ZoomIn(); }
            if (txt == "ZoomOut") { selectedScintilla.ZoomOut(); }

            if (txt == "Load Keyword List")
            {
                _Database.SQLite sqlite = new _Database.SQLite();

                List<string> words = sqlite.List("Scintilla_Keywords");

                if (words.Count == 0)
                {
                    string AHKKeywords = "a_ahkpath a_ahkversion a_appdatacommon a_appdata a_autotrim a_batchlines a_carety a_caretx a_computername a_controldelay a_dd a_cursor a_ddd a_dddd a_desktop a_defaultmousespeed a_desktopcommon a_detecthiddentext a_endchar a_detecthiddenwindows a_eventinfo a_exitreason a_formatfloat a_formatinteger a_guievent a_gui a_guicontrol a_guicontrolevent a_guiheight a_guiwidth a_guix a_hour a_guiy a_iconfile a_iconhidden a_icontip a_iconnumber a_index a_ipaddress1 a_ipaddress2 a_ipaddress3 a_isadmin a_ipaddress4 a_iscompiled a_issuspended a_language a_keydelay a_iscritical a_isunicode errorlevel a_ptrsize a_lasterror a_linefile a_loopfield a_linenumber a_loopfileattrib a_loopfiledir  a_loopfileext a_loopfileshortname a_loopfileshortpath a_loopregsubkey a_loopfiletimecreated a_loopfileshortpath a_loopfilefullpath a_loopfilename a_loopfilelongpath a_loopfilesize a_loopfilesizekb a_loopfiletimeaccessed a_loopfilesizemb a_loopfiletimemodified a_loopreadline a_loopregname a_loopregkey a_loopregtimemodified a_loopregtype a_min a_mday a_mm a_mmm a_mon a_mmmm a_mousedelay a_msec a_mydocuments a_now a_numbatchlines a_nowutc a_ostype a_osversion a_programfiles a_priorhotkey a_programs  a_programscommon a_screenwidth a_screenheight a_scriptdir a_scriptfullpath a_scriptname a_sec a_space a_startmenucommon a_startmenu a_startup a_startupcommon a_tab a_stringcasesense a_thishotkey a_thismenu a_thismenuitempos a_thismenuitem a_tickcount a_timeidle  a_timeidlephysical a_timesincepriorhotkey a_timesincethishotkey a_titlematchmodespeed a_titlematchmode a_wday a_windelay a_workingdir a_windir a_yday a_year a_yyyy a_yweek clipboard clipboardall  comspec a_ispaused a_thisfunc a_thislabel programfiles a_temp a_username FALSE TRUE";
                    string AHKKeywords2 = "Object() Array() Abs() AutoTrim Asc() ASin() ACos() ATan() BlockInput Break Catch Ceil() Chr() Click ClipWait ComObjActive() ComObjArray() ComObjConnect() ComObjCreate() ComObject() ComObjEnwrap() ComObjUnwrap() ComObjError() ComObjFlags() ComObjGet() ComObjMissing() ComObjParameter() ComObjQuery() ComObjType() ComObjValue() Continue Control ControlClick ControlFocus ControlGet ControlGetFocus ControlGetPos ControlGetText ControlMove ControlSend ControlSendRaw ControlSetText CoordMode Cos() Critical DetectHiddenText DetectHiddenWindows DllCall() Drive DriveGet DriveSpaceFree Edit Else EnvAdd EnvDiv EnvGet EnvMult EnvSet EnvSub EnvUpdate Exception() Exit ExitApp Exp() FileAppend FileCopy FileCopyDir FileCreateDir FileCreateShortcut FileDelete FileEncoding FileExist() FileInstall FileGetAttrib FileGetShortcut FileGetSize FileGetTime FileGetVersion FileMove FileMoveDir FileOpen FileRead FileReadLine FileRecycle FileRecycleEmpty FileRemoveDir FileSelectFile FileSelectFolder FileSetAttrib FileSetTime Finally Floor() Format FormatTime Func() GetKeyName() GetKeyVK() GetKeySC() GetKeyState Gosub Goto GroupActivate GroupAdd GroupClose GroupDeactivate Gui GuiControl GuiControlGet Hotkey Hotstring() if IfEqual IfNotEqual IfExist IfNotExist IfGreater IfGreaterOrEqual IfInString IfNotInString InStr() IfLess IfLessOrEqual IfMsgBox IfWinActive IfWinNotActive IfWinExist IfWinNotExist IL_Create() IL_Add() IL_Destroy() ImageSearch IniDelete IniRead IniWrite Input InputBox InStr() IsByRef() IsFunc() IsLabel() IsObject() KeyHistory KeyWait ListHotkeys ListLines ListVars LoadPicture() Log() Ln() Loop LV_Add() LV_Delete() LV_DeleteCol() LV_GetCount() LV_GetNext() LV_GetText() LV_Insert() LV_InsertCol() LV_Modify() LV_ModifyCol() LV_SetImageList() Max() Menu MenuGetHandle MenuGetName Min() Mod() MouseClick MouseClickDrag MouseGetPos MouseMove MsgBox NumGet() NumPut() ObjAddRef() ObjRelease() ObjBindMethod() ObjClone() ObjCount() ObjDelete() ObjGetAddress() ObjGetCapacity() ObjHasKey() ObjInsert() ObjInsertAt() ObjLength() ObjMaxIndex() ObjMinIndex() ObjNewEnum() ObjPop() ObjPush() ObjRemove() ObjRemoveAt() ObjSetCapacity() ObjGetBase() ObjRawGet() ObjRawSet() ObjSetBase() OnClipboardChange() OnError OnExit OnMessage() Ord() OutputDebug Pause PixelGetColor PixelSearch PostMessage Process Progress Random RegExMatch() RegExReplace() RegDelete RegRead RegWrite RegisterCallback() Reload Return Round() Run RunAs RunWait SB_SetIcon() SB_SetParts() SB_SetText() Send SendRaw SendInput SendPlay SendEvent SendLevel SendMessage SendMode SetBatchLines SetCapsLockState SetControlDelay SetDefaultMouseSpeed SetEnv SetFormat SetKeyDelay SetMouseDelay SetNumLockState SetScrollLockState SetRegView SetStoreCapsLockMode SetTimer SetTitleMatchMode SetWinDelay SetWorkingDir Shutdown Sin() Sleep Sort SoundBeep SoundGet SoundGetWaveVolume SoundPlay SoundSet SoundSetWaveVolume SplashImage SplashTextOn SplashTextOff SplitPath Sqrt() StatusBarGetText StatusBarWait StrPut() StrGet() StringCaseSense StringGetPos InStr() StringLeft StringLen StrLen() StringLower StringMid SubStr() StringReplace StrReplace() StringRight StringSplit StrSplit() StringTrimLeft StringTrimRight StringUpper Suspend SysGet Tan() Thread Throw ToolTip Transform TrayTip Trim() LTrim() RTrim() Try TV_Add() TV_Delete() TV_Get() TV_GetChild() TV_GetCount() TV_GetNext() TV_GetParent() TV_GetPrev() TV_GetSelection() TV_GetText() TV_Modify() TV_SetImageList() Until UrlDownloadToFile VarSetCapacity() While WinActivate WinActivateBottom WinActive() WinClose WinExist() WinGetActiveStats WinGetActiveTitle WinGetClass WinGet WinGetPos WinGetText WinGetTitle WinHide WinKill WinMaximize WinMenuSelectItem WinMinimize WinMinimizeAll WinMinimizeAllUndo WinMove WinRestore WinSet WinSetTitle WinShow WinWait WinWaitActive WinWaitClose WinWaitNotActive #ClipboardTimeout #CommentFlag #Delimiter #DerefChar #ErrorStdOut #EscapeChar #HotkeyInterval #HotkeyModifierTimeout #Hotstring #If #IfTimeout #IfWinActive #IfWinNotActive #IfWinExist #IfWinNotExist #Include #IncludeAgain #InputLevel #InstallKeybdHook #InstallMouseHook #KeyHistory #LTrim #MaxHotkeysPerInterval #MaxMem #MaxThreads #MaxThreadsBuffer #MaxThreadsPerHotkey #MenuMaskKey #NoEnv #NoTrayIcon #Persistent #SingleInstance #UseHook #Warn #WinActivateForce";
                    scintilla1.Text = AHKKeywords + " " + AHKKeywords2;
                }
                else
                {
                    _Lists lst = new _Lists();
                    scintilla1.Text = lst.List_To_String(words, " ");
                }
            }
            if (txt == "Save Keyword List")
            {
                //string AHKKeywords = "a_ahkpath a_ahkversion a_appdatacommon a_appdata a_autotrim a_batchlines a_carety a_caretx a_computername a_controldelay a_dd a_cursor a_ddd a_dddd a_desktop a_defaultmousespeed a_desktopcommon a_detecthiddentext a_endchar a_detecthiddenwindows a_eventinfo a_exitreason a_formatfloat a_formatinteger a_guievent a_gui a_guicontrol a_guicontrolevent a_guiheight a_guiwidth a_guix a_hour a_guiy a_iconfile a_iconhidden a_icontip a_iconnumber a_index a_ipaddress1 a_ipaddress2 a_ipaddress3 a_isadmin a_ipaddress4 a_iscompiled a_issuspended a_language a_keydelay a_iscritical a_isunicode errorlevel a_ptrsize a_lasterror a_linefile a_loopfield a_linenumber a_loopfileattrib a_loopfiledir  a_loopfileext a_loopfileshortname a_loopfileshortpath a_loopregsubkey a_loopfiletimecreated a_loopfileshortpath a_loopfilefullpath a_loopfilename a_loopfilelongpath a_loopfilesize a_loopfilesizekb a_loopfiletimeaccessed a_loopfilesizemb a_loopfiletimemodified a_loopreadline a_loopregname a_loopregkey a_loopregtimemodified a_loopregtype a_min a_mday a_mm a_mmm a_mon a_mmmm a_mousedelay a_msec a_mydocuments a_now a_numbatchlines a_nowutc a_ostype a_osversion a_programfiles a_priorhotkey a_programs  a_programscommon a_screenwidth a_screenheight a_scriptdir a_scriptfullpath a_scriptname a_sec a_space a_startmenucommon a_startmenu a_startup a_startupcommon a_tab a_stringcasesense a_thishotkey a_thismenu a_thismenuitempos a_thismenuitem a_tickcount a_timeidle  a_timeidlephysical a_timesincepriorhotkey a_timesincethishotkey a_titlematchmodespeed a_titlematchmode a_wday a_windelay a_workingdir a_windir a_yday a_year a_yyyy a_yweek clipboard clipboardall  comspec a_ispaused a_thisfunc a_thislabel programfiles a_temp a_username FALSE TRUE";
                //string AHKKeywords2 = "Object() Array() Abs() AutoTrim Asc() ASin() ACos() ATan() BlockInput Break Catch Ceil() Chr() Click ClipWait ComObjActive() ComObjArray() ComObjConnect() ComObjCreate() ComObject() ComObjEnwrap() ComObjUnwrap() ComObjError() ComObjFlags() ComObjGet() ComObjMissing() ComObjParameter() ComObjQuery() ComObjType() ComObjValue() Continue Control ControlClick ControlFocus ControlGet ControlGetFocus ControlGetPos ControlGetText ControlMove ControlSend ControlSendRaw ControlSetText CoordMode Cos() Critical DetectHiddenText DetectHiddenWindows DllCall() Drive DriveGet DriveSpaceFree Edit Else EnvAdd EnvDiv EnvGet EnvMult EnvSet EnvSub EnvUpdate Exception() Exit ExitApp Exp() FileAppend FileCopy FileCopyDir FileCreateDir FileCreateShortcut FileDelete FileEncoding FileExist() FileInstall FileGetAttrib FileGetShortcut FileGetSize FileGetTime FileGetVersion FileMove FileMoveDir FileOpen FileRead FileReadLine FileRecycle FileRecycleEmpty FileRemoveDir FileSelectFile FileSelectFolder FileSetAttrib FileSetTime Finally Floor() Format FormatTime Func() GetKeyName() GetKeyVK() GetKeySC() GetKeyState Gosub Goto GroupActivate GroupAdd GroupClose GroupDeactivate Gui GuiControl GuiControlGet Hotkey Hotstring() if IfEqual IfNotEqual IfExist IfNotExist IfGreater IfGreaterOrEqual IfInString IfNotInString InStr() IfLess IfLessOrEqual IfMsgBox IfWinActive IfWinNotActive IfWinExist IfWinNotExist IL_Create() IL_Add() IL_Destroy() ImageSearch IniDelete IniRead IniWrite Input InputBox InStr() IsByRef() IsFunc() IsLabel() IsObject() KeyHistory KeyWait ListHotkeys ListLines ListVars LoadPicture() Log() Ln() Loop LV_Add() LV_Delete() LV_DeleteCol() LV_GetCount() LV_GetNext() LV_GetText() LV_Insert() LV_InsertCol() LV_Modify() LV_ModifyCol() LV_SetImageList() Max() Menu MenuGetHandle MenuGetName Min() Mod() MouseClick MouseClickDrag MouseGetPos MouseMove MsgBox NumGet() NumPut() ObjAddRef() ObjRelease() ObjBindMethod() ObjClone() ObjCount() ObjDelete() ObjGetAddress() ObjGetCapacity() ObjHasKey() ObjInsert() ObjInsertAt() ObjLength() ObjMaxIndex() ObjMinIndex() ObjNewEnum() ObjPop() ObjPush() ObjRemove() ObjRemoveAt() ObjSetCapacity() ObjGetBase() ObjRawGet() ObjRawSet() ObjSetBase() OnClipboardChange() OnError OnExit OnMessage() Ord() OutputDebug Pause PixelGetColor PixelSearch PostMessage Process Progress Random RegExMatch() RegExReplace() RegDelete RegRead RegWrite RegisterCallback() Reload Return Round() Run RunAs RunWait SB_SetIcon() SB_SetParts() SB_SetText() Send SendRaw SendInput SendPlay SendEvent SendLevel SendMessage SendMode SetBatchLines SetCapsLockState SetControlDelay SetDefaultMouseSpeed SetEnv SetFormat SetKeyDelay SetMouseDelay SetNumLockState SetScrollLockState SetRegView SetStoreCapsLockMode SetTimer SetTitleMatchMode SetWinDelay SetWorkingDir Shutdown Sin() Sleep Sort SoundBeep SoundGet SoundGetWaveVolume SoundPlay SoundSet SoundSetWaveVolume SplashImage SplashTextOn SplashTextOff SplitPath Sqrt() StatusBarGetText StatusBarWait StrPut() StrGet() StringCaseSense StringGetPos InStr() StringLeft StringLen StrLen() StringLower StringMid SubStr() StringReplace StrReplace() StringRight StringSplit StrSplit() StringTrimLeft StringTrimRight StringUpper Suspend SysGet Tan() Thread Throw ToolTip Transform TrayTip Trim() LTrim() RTrim() Try TV_Add() TV_Delete() TV_Get() TV_GetChild() TV_GetCount() TV_GetNext() TV_GetParent() TV_GetPrev() TV_GetSelection() TV_GetText() TV_Modify() TV_SetImageList() Until UrlDownloadToFile VarSetCapacity() While WinActivate WinActivateBottom WinActive() WinClose WinExist() WinGetActiveStats WinGetActiveTitle WinGetClass WinGet WinGetPos WinGetText WinGetTitle WinHide WinKill WinMaximize WinMenuSelectItem WinMinimize WinMinimizeAll WinMinimizeAllUndo WinMove WinRestore WinSet WinSetTitle WinShow WinWait WinWaitActive WinWaitClose WinWaitNotActive #ClipboardTimeout #CommentFlag #Delimiter #DerefChar #ErrorStdOut #EscapeChar #HotkeyInterval #HotkeyModifierTimeout #Hotstring #If #IfTimeout #IfWinActive #IfWinNotActive #IfWinExist #IfWinNotExist #Include #IncludeAgain #InputLevel #InstallKeybdHook #InstallMouseHook #KeyHistory #LTrim #MaxHotkeysPerInterval #MaxMem #MaxThreads #MaxThreadsBuffer #MaxThreadsPerHotkey #MenuMaskKey #NoEnv #NoTrayIcon #Persistent #SingleInstance #UseHook #Warn #WinActivateForce";

                _Database.SQLite sqlite = new _Database.SQLite();
                sqlite.List("Scintilla_Keywords", scintilla1.Text);

            }
            if (txt == "Apply Keyword List")
            {
                selectedScintilla.SetKeywords(0, scintilla1.Text);
            }


            if (txt == "Load AutoComplete List")
            {
                _ScintillaControl sci = new _ScintillaControl();
                //_Database.SQLite sqlite = new _Database.SQLite();

                List<string> words = sci.Return_AutoCompleteList("Scintilla_AutoComplete");

                if (words.Count == 0)
                {
                    string AHKKeywords = "a_ahkpath a_ahkversion a_appdatacommon a_appdata a_autotrim a_batchlines a_carety a_caretx a_computername a_controldelay a_dd a_cursor a_ddd a_dddd a_desktop a_defaultmousespeed a_desktopcommon a_detecthiddentext a_endchar a_detecthiddenwindows a_eventinfo a_exitreason a_formatfloat a_formatinteger a_guievent a_gui a_guicontrol a_guicontrolevent a_guiheight a_guiwidth a_guix a_hour a_guiy a_iconfile a_iconhidden a_icontip a_iconnumber a_index a_ipaddress1 a_ipaddress2 a_ipaddress3 a_isadmin a_ipaddress4 a_iscompiled a_issuspended a_language a_keydelay a_iscritical a_isunicode errorlevel a_ptrsize a_lasterror a_linefile a_loopfield a_linenumber a_loopfileattrib a_loopfiledir  a_loopfileext a_loopfileshortname a_loopfileshortpath a_loopregsubkey a_loopfiletimecreated a_loopfileshortpath a_loopfilefullpath a_loopfilename a_loopfilelongpath a_loopfilesize a_loopfilesizekb a_loopfiletimeaccessed a_loopfilesizemb a_loopfiletimemodified a_loopreadline a_loopregname a_loopregkey a_loopregtimemodified a_loopregtype a_min a_mday a_mm a_mmm a_mon a_mmmm a_mousedelay a_msec a_mydocuments a_now a_numbatchlines a_nowutc a_ostype a_osversion a_programfiles a_priorhotkey a_programs  a_programscommon a_screenwidth a_screenheight a_scriptdir a_scriptfullpath a_scriptname a_sec a_space a_startmenucommon a_startmenu a_startup a_startupcommon a_tab a_stringcasesense a_thishotkey a_thismenu a_thismenuitempos a_thismenuitem a_tickcount a_timeidle  a_timeidlephysical a_timesincepriorhotkey a_timesincethishotkey a_titlematchmodespeed a_titlematchmode a_wday a_windelay a_workingdir a_windir a_yday a_year a_yyyy a_yweek clipboard clipboardall  comspec a_ispaused a_thisfunc a_thislabel programfiles a_temp a_username FALSE TRUE";
                    string AHKKeywords2 = "Object() Array() Abs() AutoTrim Asc() ASin() ACos() ATan() BlockInput Break Catch Ceil() Chr() Click ClipWait ComObjActive() ComObjArray() ComObjConnect() ComObjCreate() ComObject() ComObjEnwrap() ComObjUnwrap() ComObjError() ComObjFlags() ComObjGet() ComObjMissing() ComObjParameter() ComObjQuery() ComObjType() ComObjValue() Continue Control ControlClick ControlFocus ControlGet ControlGetFocus ControlGetPos ControlGetText ControlMove ControlSend ControlSendRaw ControlSetText CoordMode Cos() Critical DetectHiddenText DetectHiddenWindows DllCall() Drive DriveGet DriveSpaceFree Edit Else EnvAdd EnvDiv EnvGet EnvMult EnvSet EnvSub EnvUpdate Exception() Exit ExitApp Exp() FileAppend FileCopy FileCopyDir FileCreateDir FileCreateShortcut FileDelete FileEncoding FileExist() FileInstall FileGetAttrib FileGetShortcut FileGetSize FileGetTime FileGetVersion FileMove FileMoveDir FileOpen FileRead FileReadLine FileRecycle FileRecycleEmpty FileRemoveDir FileSelectFile FileSelectFolder FileSetAttrib FileSetTime Finally Floor() Format FormatTime Func() GetKeyName() GetKeyVK() GetKeySC() GetKeyState Gosub Goto GroupActivate GroupAdd GroupClose GroupDeactivate Gui GuiControl GuiControlGet Hotkey Hotstring() if IfEqual IfNotEqual IfExist IfNotExist IfGreater IfGreaterOrEqual IfInString IfNotInString InStr() IfLess IfLessOrEqual IfMsgBox IfWinActive IfWinNotActive IfWinExist IfWinNotExist IL_Create() IL_Add() IL_Destroy() ImageSearch IniDelete IniRead IniWrite Input InputBox InStr() IsByRef() IsFunc() IsLabel() IsObject() KeyHistory KeyWait ListHotkeys ListLines ListVars LoadPicture() Log() Ln() Loop LV_Add() LV_Delete() LV_DeleteCol() LV_GetCount() LV_GetNext() LV_GetText() LV_Insert() LV_InsertCol() LV_Modify() LV_ModifyCol() LV_SetImageList() Max() Menu MenuGetHandle MenuGetName Min() Mod() MouseClick MouseClickDrag MouseGetPos MouseMove MsgBox NumGet() NumPut() ObjAddRef() ObjRelease() ObjBindMethod() ObjClone() ObjCount() ObjDelete() ObjGetAddress() ObjGetCapacity() ObjHasKey() ObjInsert() ObjInsertAt() ObjLength() ObjMaxIndex() ObjMinIndex() ObjNewEnum() ObjPop() ObjPush() ObjRemove() ObjRemoveAt() ObjSetCapacity() ObjGetBase() ObjRawGet() ObjRawSet() ObjSetBase() OnClipboardChange() OnError OnExit OnMessage() Ord() OutputDebug Pause PixelGetColor PixelSearch PostMessage Process Progress Random RegExMatch() RegExReplace() RegDelete RegRead RegWrite RegisterCallback() Reload Return Round() Run RunAs RunWait SB_SetIcon() SB_SetParts() SB_SetText() Send SendRaw SendInput SendPlay SendEvent SendLevel SendMessage SendMode SetBatchLines SetCapsLockState SetControlDelay SetDefaultMouseSpeed SetEnv SetFormat SetKeyDelay SetMouseDelay SetNumLockState SetScrollLockState SetRegView SetStoreCapsLockMode SetTimer SetTitleMatchMode SetWinDelay SetWorkingDir Shutdown Sin() Sleep Sort SoundBeep SoundGet SoundGetWaveVolume SoundPlay SoundSet SoundSetWaveVolume SplashImage SplashTextOn SplashTextOff SplitPath Sqrt() StatusBarGetText StatusBarWait StrPut() StrGet() StringCaseSense StringGetPos InStr() StringLeft StringLen StrLen() StringLower StringMid SubStr() StringReplace StrReplace() StringRight StringSplit StrSplit() StringTrimLeft StringTrimRight StringUpper Suspend SysGet Tan() Thread Throw ToolTip Transform TrayTip Trim() LTrim() RTrim() Try TV_Add() TV_Delete() TV_Get() TV_GetChild() TV_GetCount() TV_GetNext() TV_GetParent() TV_GetPrev() TV_GetSelection() TV_GetText() TV_Modify() TV_SetImageList() Until UrlDownloadToFile VarSetCapacity() While WinActivate WinActivateBottom WinActive() WinClose WinExist() WinGetActiveStats WinGetActiveTitle WinGetClass WinGet WinGetPos WinGetText WinGetTitle WinHide WinKill WinMaximize WinMenuSelectItem WinMinimize WinMinimizeAll WinMinimizeAllUndo WinMove WinRestore WinSet WinSetTitle WinShow WinWait WinWaitActive WinWaitClose WinWaitNotActive #ClipboardTimeout #CommentFlag #Delimiter #DerefChar #ErrorStdOut #EscapeChar #HotkeyInterval #HotkeyModifierTimeout #Hotstring #If #IfTimeout #IfWinActive #IfWinNotActive #IfWinExist #IfWinNotExist #Include #IncludeAgain #InputLevel #InstallKeybdHook #InstallMouseHook #KeyHistory #LTrim #MaxHotkeysPerInterval #MaxMem #MaxThreads #MaxThreadsBuffer #MaxThreadsPerHotkey #MenuMaskKey #NoEnv #NoTrayIcon #Persistent #SingleInstance #UseHook #Warn #WinActivateForce";
                    scintilla1.Text = AHKKeywords + " " + AHKKeywords2;
                }
                else
                {
                    //_Lists lst = new _Lists();
                    scintilla1.Text = words.ToString();
                }
            }
            if (txt == "Save AutoComplete List")
            {
                _ScintillaControl sci = new _ScintillaControl();
                //string AHKKeywords = "a_ahkpath a_ahkversion a_appdatacommon a_appdata a_autotrim a_batchlines a_carety a_caretx a_computername a_controldelay a_dd a_cursor a_ddd a_dddd a_desktop a_defaultmousespeed a_desktopcommon a_detecthiddentext a_endchar a_detecthiddenwindows a_eventinfo a_exitreason a_formatfloat a_formatinteger a_guievent a_gui a_guicontrol a_guicontrolevent a_guiheight a_guiwidth a_guix a_hour a_guiy a_iconfile a_iconhidden a_icontip a_iconnumber a_index a_ipaddress1 a_ipaddress2 a_ipaddress3 a_isadmin a_ipaddress4 a_iscompiled a_issuspended a_language a_keydelay a_iscritical a_isunicode errorlevel a_ptrsize a_lasterror a_linefile a_loopfield a_linenumber a_loopfileattrib a_loopfiledir  a_loopfileext a_loopfileshortname a_loopfileshortpath a_loopregsubkey a_loopfiletimecreated a_loopfileshortpath a_loopfilefullpath a_loopfilename a_loopfilelongpath a_loopfilesize a_loopfilesizekb a_loopfiletimeaccessed a_loopfilesizemb a_loopfiletimemodified a_loopreadline a_loopregname a_loopregkey a_loopregtimemodified a_loopregtype a_min a_mday a_mm a_mmm a_mon a_mmmm a_mousedelay a_msec a_mydocuments a_now a_numbatchlines a_nowutc a_ostype a_osversion a_programfiles a_priorhotkey a_programs  a_programscommon a_screenwidth a_screenheight a_scriptdir a_scriptfullpath a_scriptname a_sec a_space a_startmenucommon a_startmenu a_startup a_startupcommon a_tab a_stringcasesense a_thishotkey a_thismenu a_thismenuitempos a_thismenuitem a_tickcount a_timeidle  a_timeidlephysical a_timesincepriorhotkey a_timesincethishotkey a_titlematchmodespeed a_titlematchmode a_wday a_windelay a_workingdir a_windir a_yday a_year a_yyyy a_yweek clipboard clipboardall  comspec a_ispaused a_thisfunc a_thislabel programfiles a_temp a_username FALSE TRUE";
                //string AHKKeywords2 = "Object() Array() Abs() AutoTrim Asc() ASin() ACos() ATan() BlockInput Break Catch Ceil() Chr() Click ClipWait ComObjActive() ComObjArray() ComObjConnect() ComObjCreate() ComObject() ComObjEnwrap() ComObjUnwrap() ComObjError() ComObjFlags() ComObjGet() ComObjMissing() ComObjParameter() ComObjQuery() ComObjType() ComObjValue() Continue Control ControlClick ControlFocus ControlGet ControlGetFocus ControlGetPos ControlGetText ControlMove ControlSend ControlSendRaw ControlSetText CoordMode Cos() Critical DetectHiddenText DetectHiddenWindows DllCall() Drive DriveGet DriveSpaceFree Edit Else EnvAdd EnvDiv EnvGet EnvMult EnvSet EnvSub EnvUpdate Exception() Exit ExitApp Exp() FileAppend FileCopy FileCopyDir FileCreateDir FileCreateShortcut FileDelete FileEncoding FileExist() FileInstall FileGetAttrib FileGetShortcut FileGetSize FileGetTime FileGetVersion FileMove FileMoveDir FileOpen FileRead FileReadLine FileRecycle FileRecycleEmpty FileRemoveDir FileSelectFile FileSelectFolder FileSetAttrib FileSetTime Finally Floor() Format FormatTime Func() GetKeyName() GetKeyVK() GetKeySC() GetKeyState Gosub Goto GroupActivate GroupAdd GroupClose GroupDeactivate Gui GuiControl GuiControlGet Hotkey Hotstring() if IfEqual IfNotEqual IfExist IfNotExist IfGreater IfGreaterOrEqual IfInString IfNotInString InStr() IfLess IfLessOrEqual IfMsgBox IfWinActive IfWinNotActive IfWinExist IfWinNotExist IL_Create() IL_Add() IL_Destroy() ImageSearch IniDelete IniRead IniWrite Input InputBox InStr() IsByRef() IsFunc() IsLabel() IsObject() KeyHistory KeyWait ListHotkeys ListLines ListVars LoadPicture() Log() Ln() Loop LV_Add() LV_Delete() LV_DeleteCol() LV_GetCount() LV_GetNext() LV_GetText() LV_Insert() LV_InsertCol() LV_Modify() LV_ModifyCol() LV_SetImageList() Max() Menu MenuGetHandle MenuGetName Min() Mod() MouseClick MouseClickDrag MouseGetPos MouseMove MsgBox NumGet() NumPut() ObjAddRef() ObjRelease() ObjBindMethod() ObjClone() ObjCount() ObjDelete() ObjGetAddress() ObjGetCapacity() ObjHasKey() ObjInsert() ObjInsertAt() ObjLength() ObjMaxIndex() ObjMinIndex() ObjNewEnum() ObjPop() ObjPush() ObjRemove() ObjRemoveAt() ObjSetCapacity() ObjGetBase() ObjRawGet() ObjRawSet() ObjSetBase() OnClipboardChange() OnError OnExit OnMessage() Ord() OutputDebug Pause PixelGetColor PixelSearch PostMessage Process Progress Random RegExMatch() RegExReplace() RegDelete RegRead RegWrite RegisterCallback() Reload Return Round() Run RunAs RunWait SB_SetIcon() SB_SetParts() SB_SetText() Send SendRaw SendInput SendPlay SendEvent SendLevel SendMessage SendMode SetBatchLines SetCapsLockState SetControlDelay SetDefaultMouseSpeed SetEnv SetFormat SetKeyDelay SetMouseDelay SetNumLockState SetScrollLockState SetRegView SetStoreCapsLockMode SetTimer SetTitleMatchMode SetWinDelay SetWorkingDir Shutdown Sin() Sleep Sort SoundBeep SoundGet SoundGetWaveVolume SoundPlay SoundSet SoundSetWaveVolume SplashImage SplashTextOn SplashTextOff SplitPath Sqrt() StatusBarGetText StatusBarWait StrPut() StrGet() StringCaseSense StringGetPos InStr() StringLeft StringLen StrLen() StringLower StringMid SubStr() StringReplace StrReplace() StringRight StringSplit StrSplit() StringTrimLeft StringTrimRight StringUpper Suspend SysGet Tan() Thread Throw ToolTip Transform TrayTip Trim() LTrim() RTrim() Try TV_Add() TV_Delete() TV_Get() TV_GetChild() TV_GetCount() TV_GetNext() TV_GetParent() TV_GetPrev() TV_GetSelection() TV_GetText() TV_Modify() TV_SetImageList() Until UrlDownloadToFile VarSetCapacity() While WinActivate WinActivateBottom WinActive() WinClose WinExist() WinGetActiveStats WinGetActiveTitle WinGetClass WinGet WinGetPos WinGetText WinGetTitle WinHide WinKill WinMaximize WinMenuSelectItem WinMinimize WinMinimizeAll WinMinimizeAllUndo WinMove WinRestore WinSet WinSetTitle WinShow WinWait WinWaitActive WinWaitClose WinWaitNotActive #ClipboardTimeout #CommentFlag #Delimiter #DerefChar #ErrorStdOut #EscapeChar #HotkeyInterval #HotkeyModifierTimeout #Hotstring #If #IfTimeout #IfWinActive #IfWinNotActive #IfWinExist #IfWinNotExist #Include #IncludeAgain #InputLevel #InstallKeybdHook #InstallMouseHook #KeyHistory #LTrim #MaxHotkeysPerInterval #MaxMem #MaxThreads #MaxThreadsBuffer #MaxThreadsPerHotkey #MenuMaskKey #NoEnv #NoTrayIcon #Persistent #SingleInstance #UseHook #Warn #WinActivateForce";

                //_Database.SQLite sqlite = new _Database.SQLite();
                //sqlite.List("Scintilla_Keywords", scintilla1.Text);

                sci.Save_AutoCompleteList(scintilla1.Text);
            }
            if (txt == "Apply AutoComplete List")
            {
                _ScintillaControl sci = new _ScintillaControl();
                sci.Save_AutoCompleteList(scintilla1.Text);
                sci.Setup_AutoComplete(selectedScintilla);
            }


        }



        public void Current(Scintilla CurrentScintilla)  // set current scintilla to act upon
        {
            selectedScintilla = CurrentScintilla;
        }



        #endregion

        public Color HighlightColor;

        private void btnColor_Click(object sender, EventArgs e)
        {
            // Show the color dialog.
            DialogResult result = colorDialog1.ShowDialog();
            // See if user pressed ok.
            if (result == DialogResult.OK)
            {
                HighlightColor = colorDialog1.Color;
                btnColor.BackColor = colorDialog1.Color;
                btnColor.Text = colorDialog1.Color.Name.ToString();
            }
        }
    }


}

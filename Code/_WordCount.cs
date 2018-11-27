using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls.UI;
using sharpAHK;
using sharpAHK_Dev;
using AHKExpressions;
using System.Threading;

namespace sharpAHK_Dev
{
    public class _WordCount
    {
        #region === WordCount ===

        _AHK ahk = new _AHK();        

        // generate wordlist

        public string WordCount_FromFile(string filePath)  // returns list of words parsed to unique words with word count (in ini format) from File
        {
            //### load AHK script into memory, execute function, return parse value ###

            // load wordcount.ahk function into AHK workspace
            string wordCountAHK = ahk.AppDir() + "\\Scripts\\WordCount.ahk";  //@"C:\Users\Jason\Google Drive\IMDB\Scripts\WordCount.ahk";

            //create an autohotkey engine (AHK DLL) or use existing instance if it hasn't be initiated
            AutoHotkey.Interop.AutoHotkeyEngine ahkddll = new AutoHotkey.Interop.AutoHotkeyEngine();

            // load the ahk function file to access in user scripts
            ahk.Load_ahkFile(wordCountAHK, false, false);

            // execute function to return ini values with counts after parsing the filePath for words
            string wordCounts = ahkddll.ExecFunction("Generate_WordCount", filePath);  // execute loaded function

            return wordCounts;
        }

        // !!! need to fix to not start new AHK Session
        public string WordCount_FromString(string Text)  // returns list of words parsed to unique words with word count (in ini format) from String
        {
            //### load AHK script into memory, execute function, return parse value ###

            // load wordcount.ahk function into AHK workspace
            string wordCountAHK = ahk.AppDir() + "\\Scripts\\WordCount.ahk";  //@"C:\Users\Jason\Google Drive\IMDB\Scripts\WordCount.ahk";

            //create an autohotkey engine (AHK DLL) or use existing instance if it hasn't be initiated
            AutoHotkey.Interop.AutoHotkeyEngine ahkddll = new AutoHotkey.Interop.AutoHotkeyEngine();

            // load the ahk function file to access in user scripts
            ahk.Load_ahkFile(wordCountAHK, false);


            // execute function to return ini values with counts after parsing the filePath for words
            string wordCounts = ahkddll.ExecFunction("Generate_WordCount_FromString", Text);  // execute loaded function

            return wordCounts;
        }

        public List<string> WordList(string filePath, bool SortAlpha = true, bool AddCountToWord = false)  // returns list of all unique words parsed from filePath (option to sorta alphabetically)
        {
            _Lists lst = new _Lists();

            string countINI = WordCount_FromFile(filePath);

            List<string> wcLines = lst.Text_To_List(countINI);
            List<string> wordList = new List<string>();

            int i = 0;
            foreach (string wcLine in wcLines)
            {
                if (i > 2) // skip first X lines before beginning parse
                {
                    string word = ahk.StringSplit(wcLine, "=", 0);
                    string wordCountS = ahk.StringSplit(wcLine, "=", 1);

                    if (!AddCountToWord) { wordList.Add(word); }
                    if (AddCountToWord) { word = word + " (" + wordCountS + ")"; wordList.Add(word); }  // add the # of times word found to end of word
                }

                i++;
            }

            if (SortAlpha) { wordList.Sort(); }

            return wordList;
        }


        // generate wordlist database

        public string Generate_WordCount_Db(string dbFile, string wCountFile, string TableName = "WordCount", bool ClearPreviousTable = false)  // parses word for # of unique words, writes output to db table
        {
            _Lists lst = new _Lists();

            //string wCountFile = @"C:\Users\Jason\Google Drive\IMDB\Scripts\WordCount.ahk";

            Create_WordCount_Table(dbFile, TableName, ClearPreviousTable);  // create table structure if it doesn't already exist

            string wCount = WordCount_FromFile(wCountFile);  // return parsed ini formatted text with word + word count
            //scintilla1.Text = wCount;

            List<string> wcLines = lst.Text_To_List(wCount);

            int i = 0;
            foreach (string wcLine in wcLines)
            {
                if (i > 2) // skip first X lines before beginning parse
                {
                    string word = ahk.StringSplit(wcLine, "=", 0);
                    string wordCountS = ahk.StringSplit(wcLine, "=", 1);
                    int wordCount = ahk.ToInt(wordCountS);

                    string PaddedValue = LeadingZeros(wordCountS, 5); // add leading zeros to wordcount value

                    int currentWordCount = Return_WordCount(dbFile, word, "WordCount");  // get existing count for this word

                    // insert new entry
                    if (currentWordCount == 0)  // no value returned - insert new word
                    {
                        bool Inserted = Insert_Into_WordCount(dbFile, wCountFile, word, PaddedValue, TableName);
                        if (!Inserted) { ahk.MsgBox("Failed To Insert:\n\nWord = " + word + "\nValue = " + wordCountS + "\nInserted = " + Inserted.ToString()); }
                    }

                    // or update existing entry
                    if (currentWordCount > 0)  // value returned - update existing value (add to current count first)
                    {
                        currentWordCount += wordCount;  // add to existing word count total 

                        PaddedValue = LeadingZeros(currentWordCount.ToString(), 5); // add leading zeros to wordcount value

                        bool Inserted = Update_WordCount(dbFile, wCountFile, word, PaddedValue, TableName);
                        if (!Inserted) { ahk.MsgBox("Failed To Update:\n\nWord = " + word + "\nValue = " + wordCountS + "\nUpdated = " + Inserted.ToString()); }
                    }

                }

                i++;
            }

            return wCount;
            //ahk.MsgBox("Finished Inserting WordCount Into Db");
        }


        public string Generate_WordCount_Db_FromString(string dbFile, string Text, string TableName = "WordCount", bool ClearPreviousTable = false)  // parses word for # of unique words, writes output to db table
        {
            _Lists lst = new _Lists();

            //string wCountFile = @"C:\Users\Jason\Google Drive\IMDB\Scripts\WordCount.ahk";

            Create_WordCount_Table(dbFile, TableName, ClearPreviousTable);  // create table structure if it doesn't already exist

            string wCount = WordCount_FromString(Text);  // return parsed ini formatted text with word + word count
            //scintilla1.Text = wCount;

            List<string> wcLines = lst.Text_To_List(wCount);

            int i = 0;
            foreach (string wcLine in wcLines)
            {
                if (i > 2) // skip first X lines before beginning parse
                {
                    string word = ahk.StringSplit(wcLine, "=", 0);
                    string wordCountS = ahk.StringSplit(wcLine, "=", 1);
                    int wordCount = ahk.ToInt(wordCountS);

                    string PaddedValue = LeadingZeros(wordCountS, 5); // add leading zeros to wordcount value

                    int currentWordCount = Return_WordCount(dbFile, word, "WordCount");  // get existing count for this word

                    // insert new entry
                    if (currentWordCount == 0)  // no value returned - insert new word
                    {
                        bool Inserted = Insert_Into_WordCount(dbFile, "", word, PaddedValue, TableName);
                        if (!Inserted) { ahk.MsgBox("Failed To Insert:\n\nWord = " + word + "\nValue = " + wordCountS + "\nInserted = " + Inserted.ToString()); }
                    }

                    // or update existing entry
                    if (currentWordCount > 0)  // value returned - update existing value (add to current count first)
                    {
                        currentWordCount += wordCount;  // add to existing word count total 

                        PaddedValue = LeadingZeros(currentWordCount.ToString(), 5); // add leading zeros to wordcount value

                        bool Inserted = Update_WordCount(dbFile, "", word, PaddedValue, TableName);
                        if (!Inserted) { ahk.MsgBox("Failed To Update:\n\nWord = " + word + "\nValue = " + wordCountS + "\nUpdated = " + Inserted.ToString()); }
                    }

                }

                i++;
            }

            return wCount;
            //ahk.MsgBox("Finished Inserting WordCount Into Db");
        }

        public void WordCount_FileList_Thread(string dbFile, List<string> fileList, string TableName = "WordCount", bool ClearPreviousTable = false)  // import filelist to wordcount db on new thread
        {
            //StatusBar("Starting WordCount Import : " + fileList.Count() + " Files To Import");

            // new thread with parameters in thread function
            Thread thread1 = new Thread(() => WordCount_FileList_Db(dbFile, fileList, TableName, ClearPreviousTable));
            thread1.Start();
        }

        public void WordCount_FileList_Db(string dbFile, List<string> fileList, string TableName = "WordCount", bool ClearPreviousTable = false)  // take list of files and convert to wordlist db entry
        {
            int i = 0;
            foreach (string project in fileList)
            {
                i++;

                //StatusBar("WordCount Db Import: " + i.ToString() + " / " + fileList.Count()); // update counter with progress of import

                if (i == 1)
                {
                    if (ClearPreviousTable)
                    {
                        Generate_WordCount_Db(dbFile, project, TableName, true);
                    }
                    else
                    {
                        Generate_WordCount_Db(dbFile, project, TableName, false);
                    }
                }

                if (i != 1)
                {
                    Generate_WordCount_Db(dbFile, project, TableName = "WordCount", false);
                }

            }


            //StatusBar("Finished WordCount Import : " + fileList.Count() + " Files Imported");
        }


        // return from wordcount db

        public int Return_WordCount(string dbFile, string word, string TableName = "WordCount")  // return db entry for word, returns # of times word logged into wordcounter
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            int Value = sqlite.ReturnInt(dbFile, "select wCount from [" + TableName + "] where word = '" + word + "'");
            //ahk.MsgBox(Value.ToString());
            return Value;
        }


        // display wordlists

        public void WordList_Tree(TreeView TV, string wCountFile, bool SortAlpha = true, bool AddWordCount = false)  // parse file into unique words, display in treeview control
        {
            _Lists lst = new _Lists();
            List<string> wList = WordList(wCountFile, SortAlpha, AddWordCount);

            lst.List_To_TreeView(TV, wList, "WordList");
        }

        public void WordList_FromDb_Tree(TreeView TV, string dbFile, string TableName = "WordCount", bool SortAlpha = true, bool AddWordCount = false)  // read sqlite db table, populate treeview
        {
            _Lists lst = new _Lists();
            List<string> wList = lst.SQLite_To_List(dbFile, "Select Word from [" + TableName + "]");

            if (SortAlpha) { wList.Sort(); }

            lst.List_To_TreeView(TV, wList, TableName);
        }

        public DataTable WordList_Grid(DataGridView dv, string wCountFile, int LeadingZeroCount = 5)
        {
            _Lists lst = new _Lists();

            // Here we create a DataTable with four columns.
            DataTable table = new DataTable();
            table.Columns.Add("Flagged", typeof(bool));
            table.Columns.Add("FilePath", typeof(string));
            table.Columns.Add("Word", typeof(string));
            table.Columns.Add("Count", typeof(string));


            string wCount = WordCount_FromFile(wCountFile);  // return parsed ini formatted text with word + word count

            List<string> wcLines = lst.Text_To_List(wCount);

            int i = 0;
            foreach (string wcLine in wcLines)
            {
                if (i > 2) // skip first X lines before beginning parse
                {
                    string word = ahk.StringSplit(wcLine, "=", 0);
                    string wordCountS = ahk.StringSplit(wcLine, "=", 1);

                    string PaddedValue = LeadingZeros(wordCountS, 5);

                    table.Rows.Add(false, wCountFile, word, PaddedValue);
                }

                i++;
            }

            dv.DataSource = table;


            try { dv.Columns["ID"].Visible = false; }
            catch { }
            try { dv.Columns["flagged"].Visible = false; }
            catch { }
            try { dv.Columns["FilePath"].Visible = false; }
            catch { }
            try { dv.Columns["Word"].Visible = true; }
            catch { }
            try { dv.Columns["wCount"].Visible = true; }
            catch { }

            return table;
        }

        public void WordList_FromDb_Grid(DataGridView dv, string dbFile, string TableName = "WordCount")
        {
            _GridControl grid = new _GridControl();
            int rowCount = grid.SQLite(dv, dbFile, "Select * from [" + TableName + "]", false);

            try { dv.Columns["ID"].Visible = false; }
            catch { }
            try { dv.Columns["flagged"].Visible = false; }
            catch { }
            try { dv.Columns["FilePath"].Visible = false; }
            catch { }
            try { dv.Columns["Word"].Visible = true; }
            catch { }
            try { dv.Columns["wCount"].Visible = true; }
            catch { }

            //StatusBar("Loaded " + rowCount + " Rows in DataGridView");
        }


        // wordlist sqlite

        public bool Update_WordCount(string DbFile, string FilePath = "", string Word = "", string wCount = "", string TableName = "WordCount")  // update a wordcount entry with new word count value
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            string UpdateLine = "Update [" + TableName + "] set FilePath = '" + FilePath + "', Word = '" + Word + "', wCount = '" + wCount + "' WHERE Word = '" + Word + "'";
            bool Updated = sqlite.Execute(DbFile, UpdateLine);
            if (GlobalDebug) { ahk.MsgBox("Updated [WordCount] = " + Updated.ToString()); }
            return Updated;
        }

        public bool Insert_Into_WordCount(string DbFile, string FilePath = "", string Word = "", string wCount = "", string TableName = "WordCount")  // insert new word into wordcount table
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            string InsertLine = "Insert Into [" + TableName + "] (FilePath, Word, wCount) values ('" + FilePath + "', '" + Word + "', '" + wCount + "')";
            bool Inserted = sqlite.Execute(DbFile, InsertLine);
            if (GlobalDebug) { ahk.MsgBox("Inserted Into [WordCount] = " + Inserted.ToString()); }
            return Inserted;
        }

        bool GlobalDebug = false;

        public void Create_WordCount_Table(string dbFile, string TableName = "WordCount", bool DropPrevious = false)  // create sqlite table with WordCount columns
        {
            _Database.SQLite sqlite = new _Database.SQLite();
            string NewTableLine = "ID INTEGER PRIMARY KEY, flagged BOOL, FilePath VARCHAR, Word VARCHAR, wCount VARCHAR";

            sqlite.Table_New(dbFile, TableName, NewTableLine, DropPrevious);
        }




        // used by wordcount

        public string LeadingZeros(string InValue, int TotalDigits = 4)  // add leading zeros to string/int value
        {
            // pad with leading zeroes
            int count = ahk.ToInt(InValue);
            string PadCount = "D" + TotalDigits.ToString(); // format string to indicate # of digits total to till with zeros 
            string PaddedValue = count.ToString(PadCount);  // up to 4 characters, add leading zeros to populate
            return PaddedValue;
        }

        #endregion

    }
}

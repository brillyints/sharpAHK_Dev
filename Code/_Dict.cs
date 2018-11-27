using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using sharpAHK;
using sharpAHK_Dev;
using AHKExpressions;
using TelerikExpressions;
using System.Data;
using System.Windows.Forms;

namespace sharpAHK_Dev
{
    public class _Dict
    {
        #region === Dictionary ===

        /// <summary> returns test dictionary values</summary>
        public Dictionary<string, string> TestDict()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            dictionary.Add("dog", "1");
            dictionary.Add("cat", "2");
            dictionary.Add("llama", "0");
            dictionary.Add("iguana", "-1");

            return dictionary;
        }


        /// <summary>
        /// Returns Dictionary Value from Key
        /// </summary>
        /// <param name="Dict"></param>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        public string Value(Dictionary<string, string> Dict, string KeyName)
        {
            string val = "";

            // See whether Dictionary contains this string.
            if (Dict.ContainsKey(KeyName)) { val = Dict[KeyName]; }

            return val;
        }


        /// <summary>returns list<string> of Keys from Dictionary</summary>
        /// <param name="Dictionary<string"> </param>
        /// <param name=" string> dictionary"> </param>
        public List<string> KeyList(Dictionary<string, string> dictionary)
        {
            // Get a List of all the Keys.
            List<string> keys = new List<string>(dictionary.Keys);
            return keys;
        }

        /// <summary>returns list<int> of Keys from Dictionary </summary>
        /// <param name="Dictionary<int"> </param>
        /// <param name=" string> dictionary"> </param>
        public List<int> KeyListInt(Dictionary<int, string> dictionary)
        {
            // Get a List of all the Keys.
            List<int> keys = new List<int>(dictionary.Keys);
            return keys;
        }

        /// <summary>returns list<string> of Values from Dictionary </summary>
        /// <param name="Dictionary<string"> </param>
        /// <param name=" string> dictionary"> </param>
        public List<string> ValueList(Dictionary<string, string> dictionary)
        {
            // Get a List of all the values
            List<string> values = new List<string>(dictionary.Values);
            return values;
        }

        /// <summary>returns list<int> of Values from Dictionary </summary>
        /// <param name="Dictionary<string"> </param>
        /// <param name=" int> dictionary"> </param>
        public List<int> ValueListInt(Dictionary<string, int> dictionary)
        {
            // Get a List of all the values
            List<int> values = new List<int>(dictionary.Values);
            return values;
        }

        /// <summary>Converts Dictionary<string,string> to DataTable</summary>
        /// <param name="Dictionary<string"> </param>
        /// <param name=" string> dictionary"> </param>
        /// <param name="KeyField"> </param>
        /// <param name="ValueField"> </param>
        public DataTable Dict_to_DataTable(Dictionary<string, string> dictionary, string KeyField = "Key", string ValueField = "Value")
        {
            // Here we create a DataTable with four columns.
            DataTable table = new DataTable();
            table.Columns.Add(KeyField, typeof(string));
            table.Columns.Add(ValueField, typeof(string));

            // Use var keyword to enumerate dictionary.
            foreach (var pair in dictionary)
            {
                table.Rows.Add(pair.Key, pair.Value);
            }

            return table;
        }

        /// <summary>Converts Dictionary<int,string> to DataTable</summary>
        /// <param name="Dictionary<string"> </param>
        /// <param name=" string> dictionary"> </param>
        /// <param name="KeyField"> </param>
        /// <param name="ValueField"> </param>            
        public DataTable DictInt_to_DataTable(Dictionary<int, string> dictionary, string KeyField = "Key", string ValueField = "Value")
        {
            // Here we create a DataTable with four columns.
            DataTable table = new DataTable();
            table.Columns.Add(KeyField, typeof(string));
            table.Columns.Add(ValueField, typeof(string));

            // Use var keyword to enumerate dictionary.
            foreach (var pair in dictionary)
            {
                table.Rows.Add(pair.Key, pair.Value);
            }

            return table;
        }

        /// <summary>Display Dictionary<string,string> in DataGridView</summary>
        /// <param name="dv"> </param>
        /// <param name=" Dictionary<string"> </param>
        /// <param name=" string> Dict"> </param>
        /// <param name="KeyField"> </param>
        /// <param name="ValueField"> </param>
        public void Dict_DataGrid(DataGridView dv, Dictionary<string, string> Dict, string KeyField = "Key", string ValueField = "Value")
        {
            DataTable dt = Dict_to_DataTable(Dict, KeyField, ValueField);
            dv.DataSource = dt;
        }

        /// <summary>Display Dictionary<int,string> in DataGridView</summary>
        /// <param name="dv"> </param>
        /// <param name=" Dictionary<int"> </param>
        /// <param name=" string> Dict"> </param>
        /// <param name="KeyField"> </param>
        /// <param name="ValueField"> </param>            
        public void DictInt_DataGrid(DataGridView dv, Dictionary<int, string> Dict, string KeyField = "Key", string ValueField = "Value")
        {
            DataTable dt = DictInt_to_DataTable(Dict, KeyField, ValueField);
            dv.DataSource = dt;
        }

        /// <summary>Sort Dictionary<string, string> by Value</summary>
        /// <param name="Dictionary<string"> </param>
        /// <param name=" string> dict"> </param>
        /// <param name="Reverse"> </param>
        public Dictionary<string, string> DictSort_ByValue(Dictionary<string, string> dict, bool Reverse = false)
        {
            Dictionary<string, string> returndict = new Dictionary<string, string>();

            // Order by values.
            // ... Use LINQ to specify sorting by value.
            var items = from pair in dict
                        orderby pair.Value ascending
                        select pair;

            if (Reverse)
            {
                // Reverse sort.
                // ... Can be looped over in the same way as above.
                items = from pair in dict
                        orderby pair.Value descending
                        select pair;
            }

            // Display results.
            foreach (KeyValuePair<string, string> pair in items)
            {
                //Console.WriteLine("{0}: {1}", pair.Key, pair.Value);

                returndict.Add(pair.Key, pair.Value);
            }

            return returndict;
        }



        #endregion
    }
}

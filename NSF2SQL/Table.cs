using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSF2SQL
{
    class Table
    {
        public string Name;
        public string LinkedTable;
        public Dictionary<string, Column> Columns = new Dictionary<string, Column>();
        private int rowCount = 0;

        public Table(string name)
        {
            Name = name;
        }

        public Table(string name, string linkedTable)
        {
            Name = name;
            LinkedTable = linkedTable;
        }

        public int AddRow()//Dictionary<string, Column> columnValues)
        {
            rowCount++;
            //foreach (KeyValuePair<string, Column> column in columnValues)
            //{
            //    if(!Columns.ContainsKey(column.Key)){
            //        Columns.Add(column.Key, new Column(column.Key, column.Value.Type));
            //    }
            //    Columns[column.Key].Values.Add(rowCount, column.Value.Values[0]);
            //}
            return rowCount;
        }

        public int RowCount
        {
            get { return rowCount; }
        }
    }

    class Column
    {
        public string Name;
        public string Type;
        public bool MultipleValues = false;
        public Dictionary<int, object> Values = new Dictionary<int, object>();

        public Column(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }
}

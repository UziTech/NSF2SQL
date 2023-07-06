using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSF2SQL
{
    class TsqlGenerator : ISqlGenerator
    {
        public string CreateDatabase(string databaseName)
        {
            return string.Format(@"DROP DATABASE IF EXISTS {0};
GO

CREATE DATABASE {0};
GO

USE [{0}];", databaseName);
        }

        public string SetVariables()
        {
            return string.Empty;
        }

        public string CreateTable(Table table)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("CREATE TABLE [{0}] (\r\n", table.Name);
            sqlBuilder.AppendLine("[LotusDocNr] INT NOT NULL,");
            foreach (Column column in table.Columns.Values)
            {
                sqlBuilder.AppendFormat("[{0}] {1} NULL,\r\n", column.Name, column.Type);
            }
            sqlBuilder.Append("PRIMARY KEY ([id])");
            if (table.LinkedTable != null)
            {
                sqlBuilder.Append(",\r\n");
                sqlBuilder.AppendFormat("CONSTRAINT[{0}_constraint] FOREIGN KEY([{1}id]) REFERENCES [{1}]([id]) ON DELETE CASCADE ON UPDATE CASCADE);\r\n", table.Name, table.LinkedTable);
                sqlBuilder.AppendFormat("CREATE INDEX[{0}_idx] ON [{0}]([{1}id]);\r\n", table.Name, table.LinkedTable);
            }
            else
            {
                sqlBuilder.Append(");\r\n");
            }

            return sqlBuilder.ToString();
        }

        public string BeginInsertTable(Table table)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            sqlBuilder.AppendFormat("INSERT INTO [{0}] ([LotusDocNr],[", table.Name);
            sqlBuilder.Append(string.Join("],\r\n[", table.Columns.Keys));
            sqlBuilder.AppendLine("]) VALUES");

            return sqlBuilder.ToString();
        }

        public List<string> InsertTableRowsToList(Table table)
        {
            List<string> rows = new List<string>(table.RowCount);
            for (int i = 0; i < table.RowCount; i++)
            {
                List<string> columnValues = new List<string>(table.Columns.Count);
                foreach (Column column in table.Columns.Values)
                {
                    if (column.Values.ContainsKey(i + 1))
                    {
                        if (column.Values[i + 1] != null)
                        {
                            string value = column.Values[i + 1].ToString();
                            switch (column.Type)
                            {
                                case "decimal(20,10)":
                                    if (value == "")
                                    {
                                        value = "NULL";
                                    }
                                    else if (value == "Infinity")
                                    {
                                        value = "9999999999.9999999999";
                                    }
                                    else
                                    {
                                        double temp;
                                        if (!double.TryParse(value, out temp))
                                        {
                                            value = "NULL";
                                        }
                                        else
                                        {
                                            value = value.ToString().Replace(",", ".");
                                        }
                                    }
                                    break;
                                case "datetime":
                                    if (value == "")
                                    {
                                        value = "NULL";
                                    }
                                    else
                                    {
                                        DateTime temp;
                                        if (DateTime.TryParse(value, out temp))
                                        {
                                            if (column.Values[i + 1] is DateTime)
                                            {
                                                value = ((DateTime)column.Values[i + 1]).ToString("yyyy-MM-dd HH:mm:ss");
                                            }
                                            else
                                            {
                                                value = temp.ToString("yyyy-MM-dd HH:mm:ss");
                                            }
                                            value = "'" + value.ToString().Replace("-", "") + "'";
                                        }
                                        else
                                        {
                                            value = "NULL";
                                        }
                                    }
                                    break;
                                default:
                                    value = "'" + value.ToString().Replace("'", "''") + "'";
                                    break;
                            }
                            columnValues.Add(value);
                        }
                        else
                        {
                            columnValues.Add("NULL");
                        }
                    }
                    else
                    {
                        columnValues.Add("NULL");
                    }
                }
                rows.Add("(" + (i + 1) + "," + String.Join(",\r\n", columnValues) + ")");
            }
            return rows;
        }

        public string InsertTableRowsToString(Table table)
        {
            StringBuilder sqlBuilder = new StringBuilder();
            List<string> rows = InsertTableRowsToList(table);
            int index = 0;
            do
            {
                sqlBuilder.Append(string.Join(",\r\n", rows.GetRange(index, Math.Min(1000, rows.Count - index))));
                sqlBuilder.AppendLine(";");
                index += 1000;

                if(index < rows.Count)
                {
                    sqlBuilder.AppendLine();
                    sqlBuilder.Append(BeginInsertTable(table));
                }
            }
            while (index < rows.Count);

            return sqlBuilder.ToString();
        }

        public string EndInsertTable(Table table)
        {
            return string.Empty;
        }

        public string RestoreVariables()
        {
            return string.Empty;
        }
    }
}

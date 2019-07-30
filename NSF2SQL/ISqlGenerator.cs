using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NSF2SQL
{
    interface ISqlGenerator
    {
        string CreateDatabase(string databaseName);

        string SetVariables();

        string CreateTable(Table table);

        string BeginInsertTable(Table table);

        List<string> InsertTableRowsToList(Table table);

        string InsertTableRowsToString(Table table);

        string EndInsertTable(Table table);

        string RestoreVariables();
    }
}

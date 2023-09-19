using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Domino;
using System.IO;
using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;

namespace NSF2SQL
{
    public partial class Form1 : Form
    {
        bool onLocalComputer = false;
        string notesServer = "";
        string notesDomain = "";
        string notesPassword = "";
        string mysqlServer = "";
        string mysqlDatabase = "";
        string mysqlUsername = "";
        string mysqlPassword = "";
        int mysqlNumRowsPerInsert = 1000;
        Regex excludeField = new Regex("^(Form)$");//new Regex("^(\\$.*|Form|Readers)$");
        private static ISqlGenerator sqlGenerator;

        public Form1()
        {
            InitializeComponent();
            saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            treeView1.TreeViewNodeSorter = new NodeSorter();
            #region Command Line Arguments
            string[] args = Environment.GetCommandLineArgs();
            string notesFile = "";
            bool showHelp = false;
            List<string> error = new List<string>();
            for (int i = 1; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-notesServer":
                        if (args.Length > i + 1)
                        {
                            notesServer = args[++i];
                        }
                        else
                        {
                            error.Add("Need argument after -notesServer");
                            showHelp = true;
                        }
                        break;
                    case "-notesDomain":
                        if (args.Length > i + 1)
                        {
                            notesDomain = args[++i];
                        }
                        else
                        {
                            error.Add("Need argument after -notesDomain");
                            showHelp = true;
                        }
                        break;
                    case "-notesPassword":
                        if (args.Length > i + 1)
                        {
                            notesPassword = args[++i];
                        }
                        else
                        {
                            error.Add("Need argument after -notesPassword");
                            showHelp = true;
                        }
                        break;
                    case "-mysqlServer":
                        if (args.Length > i + 1)
                        {
                            mysqlServer = args[++i];
                        }
                        else
                        {
                            error.Add("Need argument after -mysqlServer");
                            showHelp = true;
                        }
                        break;
                    case "-mysqlDatabase":
                        if (args.Length > i + 1)
                        {
                            mysqlDatabase = args[++i];
                        }
                        else
                        {
                            error.Add("Need argument after -mysqlDatabase");
                            showHelp = true;
                        }
                        break;
                    case "-mysqlUsername":
                        if (args.Length > i + 1)
                        {
                            mysqlUsername = args[++i];
                        }
                        else
                        {
                            error.Add("Need argument after -mysqlUsername");
                            showHelp = true;
                        }
                        break;
                    case "-mysqlPassword":
                        if (args.Length > i + 1)
                        {
                            mysqlPassword = args[++i];
                        }
                        else
                        {
                            error.Add("Need argument after -mysqlPassword");
                            showHelp = true;
                        }
                        break;
                    case "-notesFile":
                        if (args.Length > i + 1)
                        {
                            notesFile = args[++i];
                        }
                        else
                        {
                            error.Add("Need argument after -notesFile");
                            showHelp = true;
                        }
                        break;
                    case "/?":
                    case "-?":
                    case "/help":
                    case "-help":
                        showHelp = true;
                        break;
                    default:
                        error.Add(args[i] + " is not a valid argument.");
                        showHelp = true;
                        break;
                }
            }
            if (notesFile != "")
            {
                try
                {
                    NotesSession nSession = initSession(notesPassword);
                    NotesDatabase db;
                    if (File.Exists(notesFile) || (notesServer == "" && notesDomain == ""))
                    {
                        db = nSession.GetDatabase("", notesFile, false);
                        onLocalComputer = true;
                    }
                    else
                    {
                        db = nSession.GetDatabase(notesServer + "//" + notesDomain, notesFile, false);
                        onLocalComputer = false;
                    }
                    treeView1.Nodes.Add(db.FilePath, db.Title, "database", "database");
                }
                catch (Exception ex)
                {
                    error.Add("Error loading -notesFile: " + ex.Message);
                }
            }
            if (showHelp)
            {
                string argString =
                    "Arguments:\n" +
                    "\n" +
                    "-notesServer: The Domino server name\n" +
                    "-notesDomain: The Domino server domain\n" +
                    "-notesPassword: The password for Lotus Notes\n" +
                    "-notesFile: The file path to the nsf database\n" +
                    "-mysqlDatabase: The database name for the exported documents\n" +
                    "-mysqlServer: The mysql server address or IP address\n" +
                    "-mysqlUsername: The username for the mysql server\n" +
                    "-mysqlPassword: The password for the mysql server\n" +
                    "\n" +
                    "Examples:\n" +
                    "\n" +
                    "nsf2sql.exe -notesServer domino -notesDomain company -notesPassword \"your password\" -notesFile banner.nsf -mysqlDatabase banner_nsf";
                if (error.Count > 0)
                {
                    string messageText = "Errors:\n\n" + String.Join("\n\n", error);
                    messageText += "\n\n" + argString;
                    MessageBox.Show(messageText, "NSF2SQL Command Line Arguments", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show(argString, "NSF2SQL Command Line Arguments", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            #endregion
        }

        private class NodeSorter : System.Collections.IComparer
        {
            //sort by hasChildNodes first then alphabetically
            public int Compare(object a, object b)
            {
                TreeNode ta = a as TreeNode;
                TreeNode tb = b as TreeNode;

                if (ta.Nodes.Count > 0 == tb.Nodes.Count > 0)
                {
                    return string.Compare(ta.Text, tb.Text);
                }
                else
                {
                    return tb.Nodes.Count - ta.Nodes.Count;
                }
            }
        }

        private void bSearchServer_Click(object sender, EventArgs ea)
        {
            treeView1.Nodes.Clear();
            onLocalComputer = false;
            InputBox input = InputBox.Show("Domino Server", new InputBoxItem[] { new InputBoxItem("Server", notesServer), new InputBoxItem("Domain", notesDomain), new InputBoxItem("Password", notesPassword, true) }, InputBoxButtons.OKCancel);
            if (input.Result == InputBoxResult.OK)
            {
                notesServer = input.Items["Server"];
                notesDomain = input.Items["Domain"];
                notesPassword = input.Items["Password"];

                ProgressDialog pDialog = new ProgressDialog();
                pDialog.Title = "Get Databases";
                pDialog.Style = ProgressBarStyle.Marquee;
                pDialog.DoWork += delegate(object dialog, DoWorkEventArgs e)
                {
                    try
                    {
                        NotesSession nSession = initSession(notesPassword);
                        pDialog.ReportProgress(0);
                        NotesDbDirectory directory = nSession.GetDbDirectory(notesServer + "//" + notesDomain);
                        NotesDatabase db = directory.GetFirstDatabase(DB_TYPES.DATABASE);
                        int i = 0;
                        while (db != null)
                        {
                            if (pDialog.IsCancelled)
                            {
                                e.Cancel = true;
                                return;
                            }
                            string[] path = db.FilePath.Split(new char[] { '\\' });
                            treeView1.Invoke((MethodInvoker)delegate()
                            {
                                TreeNodeCollection nodes = treeView1.Nodes;
                                for (int n = 0; n < path.Length - 1; n++)
                                {
                                    string folder = path[n].ToUpper();
                                    if (!nodes.ContainsKey(folder))
                                    {
                                        nodes.Add(folder, folder, "folder", "folder");
                                    }
                                    nodes = nodes[folder].Nodes;
                                }
                                nodes.Add(db.FilePath, db.Title, "database", "database");
                            });
                            db = directory.GetNextDatabase();
                            pDialog.ReportProgress(i);
                            i++;
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                };
                pDialog.ProgressChanged += delegate(object dialog, ProgressChangedEventArgs e)
                {
                    pDialog.Message = e.ProgressPercentage + " Databases Found";
                };
                pDialog.Completed += delegate(object dialog, RunWorkerCompletedEventArgs e)
                {
                    //treeView1.Invoke((MethodInvoker)delegate()
                    //{
                    if (e.Cancelled)
                    {
                        treeView1.Nodes.Clear();
                    }
                    else
                    {
                        treeView1.Sort();
                    }
                    //});
                };
                pDialog.Run();
            }
        }

        private void bSearchComputer_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    treeView1.Nodes.Clear();
                    InputBox input = InputBox.Show("Lotus Notes Password", new InputBoxItem("Password", notesPassword, true), InputBoxButtons.OKCancel);
                    if (input.Result == InputBoxResult.OK)
                    {
                        notesPassword = input.Items["Password"];
                        NotesSession nSession = initSession(notesPassword);
                        onLocalComputer = true;
                        foreach (string file in openFileDialog1.FileNames)
                        {
                            NotesDatabase db = nSession.GetDatabase("", file, false);
                            treeView1.Nodes.Add(file, db.Title, "database", "database");
                        }
                        treeView1.Sort();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                openFileDialog1.FileName = "";
            }
        }

        private void bExportDocuments_Click(object sender, EventArgs ea)
        {
            if (treeView1.SelectedNode == null || treeView1.SelectedNode.Nodes.Count > 0)
            {
                MessageBox.Show("Select a database.");
                return;
            }
            if (string.IsNullOrEmpty(txbAttachmentsFolder.Text))
            {
                MessageBox.Show("Select a folder for attachments.");
                return;
            }
            int total = 0;
            long startTicks = 0;
            long lastTicks = 0;
            string timeLeft = "";
            string timeElapsed = "0:00:00";
            string databasePath = treeView1.SelectedNode.Name;
            ProgressDialog pDialog = new ProgressDialog();
            pDialog.Title = "Exporting Documents";
            #region Export Documents
            pDialog.DoWork += delegate(object dialog, DoWorkEventArgs e)
            {
                ExportTarget exportDialog = new ExportTarget();

                try
                {
                    //export documents
                    NotesSession nSession = initSession(notesPassword);
                    Dictionary<string, Table> tables = new Dictionary<string, Table>();

                    NotesDatabase db;
                    if (onLocalComputer)
                    {
                        db = nSession.GetDatabase("", databasePath, false);
                    }
                    else
                    {
                        db = nSession.GetDatabase(notesServer + "//" + notesDomain, databasePath, false);
                    }
                    //get all documents
                    total = db.AllDocuments.Count;
                    NotesDocumentCollection allDocuments = db.AllDocuments;
                    NotesDocument doc = allDocuments.GetFirstDocument();
                    startTicks = DateTime.Now.Ticks;
                    for (int i = 0; i < total; i++)
                    {
                        object[] items = (object[])doc.Items;

                        foreach (NotesItem nItem in items)
                        {
                            if (nItem.Name == "$FILE")
                            {
                                NotesItem file = doc.GetFirstItem("$File");

                                string fileName = ((object[])nItem.Values)[0].ToString();

                                NotesEmbeddedObject attachfile = doc.GetAttachment(fileName);

                                if (attachfile != null)
                                    attachfile.ExtractFile($@"{txbAttachmentsFolder.Text}\{fileName}");
                            }
                        }

                        //check if cancelled
                        if (pDialog.IsCancelled)
                        {
                            e.Cancel = true;
                            return;
                        }
                        if (doc.HasItem("Form") && ((string[])doc.GetItemValue("Form"))[0] != "")
                        {
                            //get form
                            string form = ((string[])doc.GetItemValue("Form"))[0];
                            
                            if (!tables.ContainsKey(form))
                            {
                                tables.Add(form, new Table(form));
                            }
                            int row = tables[form].AddRow();
                            //get fields
                            //set multiple values
                            foreach (NotesItem item in (NotesItem[])doc.Items)
                            {
                                //check if cancelled
                                if (pDialog.IsCancelled)
                                {
                                    e.Cancel = true;
                                    return;
                                }
                                string field = item.Name;
                                //exclude fields that start with $ and the Form field and Readers field
                                if (field == null || excludeField.IsMatch(field))
                                {
                                    continue;
                                }
                                string type = "";
                                switch (item.type)
                                {//TODO: get more types
                                    case IT_TYPE.NUMBERS:
                                        type = "decimal(20,10)";
                                        break;
                                    case IT_TYPE.DATETIMES:
                                        type = "datetime";
                                        break;
                                    default:
                                        type = "text";
                                        break;
                                }
                                object values = item.Values;
                                bool multiple = ((object[])item.Values).Length > 1;

                                if (!tables[form].Columns.ContainsKey(field))
                                {
                                    tables[form].Columns.Add(field, new Column(field, type));
                                }

                                if (multiple && !tables[form].Columns[field].MultipleValues)
                                {
                                    tables[form].Columns[field].MultipleValues = multiple;
                                }

                                if (!tables[form].Columns[field].Values.ContainsKey(row))
                                {
                                    tables[form].Columns[field].Values.Add(row, values);
                                }
                                else
                                {
                                    int j = 1;
                                    while (tables[form].Columns.ContainsKey(field + j) && tables[form].Columns[field + j].Values.ContainsKey(row))
                                    {
                                        j++;
                                    }

                                    field += j;

                                    if (!tables[form].Columns.ContainsKey(field))
                                    {
                                        tables[form].Columns.Add(field, new Column(field, type));
                                    }

                                    if (multiple && !tables[form].Columns[field].MultipleValues)
                                    {
                                        tables[form].Columns[field].MultipleValues = multiple;
                                    }

                                    tables[form].Columns[field].Values.Add(row, values);
                                }
                            }
                        }
                        //update progress
                        pDialog.ReportProgress(i, "Parsing Documents");
                        doc = allDocuments.GetNextDocument(doc);
                    }
                    //add tables for columns with multiple values
                    Dictionary<string, Table> newTables = new Dictionary<string, Table>(tables.Count);
                    lastTicks = 0;
                    startTicks = DateTime.Now.Ticks;
                    total = tables.Count;
                    int count = 0;
                    foreach (Table table in tables.Values)
                    {
                        //check if cancelled
                        if (pDialog.IsCancelled)
                        {
                            e.Cancel = true;
                            return;
                        }
                        pDialog.ReportProgress(++count, "Formatting Tables");
                        Dictionary<string, Column> columns = new Dictionary<string, Column>(table.Columns);
                        foreach (Column column in columns.Values)
                        {
                            if (column.MultipleValues)
                            {
                                string tableName = table.Name + "_" + column.Name;
                                Table newTable = new Table(tableName, table.Name);
                                Column values = new Column(column.Name, column.Type);
                                Column ids = new Column(table.Name + "id", "int");
                                foreach (KeyValuePair<int, object> cell in column.Values)
                                {
                                    //check if cancelled
                                    if (pDialog.IsCancelled)
                                    {
                                        e.Cancel = true;
                                        return;
                                    }
                                    int id = cell.Key;
                                    object[] valueArray;
                                    if (cell.Value.GetType().IsArray)
                                    {
                                        valueArray = (object[])cell.Value;
                                    }
                                    else
                                    {
                                        valueArray = new object[] { cell.Value };
                                    }
                                    foreach (object value in valueArray)
                                    {
                                        //check if cancelled
                                        if (pDialog.IsCancelled)
                                        {
                                            e.Cancel = true;
                                            return;
                                        }
                                        int row = newTable.AddRow();
                                        ids.Values.Add(row, id);
                                        values.Values.Add(row, value);
                                    }
                                }
                                newTable.Columns.Add(table.Name + "id", ids);
                                newTable.Columns.Add(column.Name, values);
                                newTables.Add(tableName, newTable);
                                table.Columns.Remove(column.Name);
                            }
                            else
                            {
                                Dictionary<int, object> values = new Dictionary<int, object>(column.Values);
                                foreach (KeyValuePair<int, object> cell in values)
                                {
                                    //check if cancelled
                                    if (pDialog.IsCancelled)
                                    {
                                        e.Cancel = true;
                                        return;
                                    }
                                    int id = cell.Key;
                                    object value;
                                    if (cell.Value.GetType().IsArray)
                                    {
                                        if (((object[])cell.Value).Length > 0)
                                        {
                                            value = ((object[])cell.Value)[0];
                                        }
                                        else
                                        {
                                            value = null;
                                        }
                                    }
                                    else
                                    {
                                        value = cell.Value;
                                    }
                                    column.Values[id] = value;
                                }
                            }
                        }
                        newTables.Add(table.Name, table);
                    }
                    //format to sql
                    total = newTables.Count;
                    bool complete = false;

                    lastTicks = 0;
                    count = 0;
                    DialogResult result = DialogResult.Cancel;
                    Invoke((MethodInvoker)delegate() { result = exportDialog.ShowDialog(this); });
                    if (result == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                        return;
                    }

                    if (exportDialog.SqlServerExport)
                    {
                        sqlGenerator = new TsqlGenerator();
                    }
                    else if (exportDialog.MySqlExport)
                    {
                        sqlGenerator = new MySqlGenerator();
                    }

                    do
                    {
                        if (exportDialog.ServerExport)
                        {
                            InputBox input = null;
                            Invoke((MethodInvoker)delegate() { input = InputBox.Show(pDialog.Window, "SQL server info?", new InputBoxItem[] { new InputBoxItem("Server", mysqlServer), new InputBoxItem("Database", mysqlDatabase), new InputBoxItem("Username", mysqlUsername), new InputBoxItem("Password", mysqlPassword, true), new InputBoxItem("Number of rows per INSERT", mysqlNumRowsPerInsert.ToString()) }, InputBoxButtons.OKCancel); });
                            if (input.Result == InputBoxResult.OK)
                            {
                                mysqlServer = input.Items["Server"];
                                mysqlDatabase = input.Items["Database"];
                                mysqlUsername = input.Items["Username"];
                                mysqlPassword = input.Items["Password"];
                                int.TryParse(input.Items["Number of rows per INSERT"], out mysqlNumRowsPerInsert);

                                DbConnection conn = null;
                                if (exportDialog.MySqlExport)
                                {
                                    conn = new MySqlConnection("SERVER=" + mysqlServer + ";USERNAME=" + mysqlUsername + ";PASSWORD=" + mysqlPassword + ";");
                                }
                                else if (exportDialog.SqlServerExport)
                                {
                                    conn = new SqlConnection("Server=" + mysqlServer + ";User Id=" + mysqlUsername + ";Password=" + mysqlPassword + ";");
                                }

                                try
                                {
                                    startTicks = DateTime.Now.Ticks;
                                    conn.Open();

                                    string[] tokens = null;
                                    if (exportDialog.SqlServerExport)
                                    {
                                        tokens = sqlGenerator.CreateDatabase(mysqlDatabase).Split(new string[] { "GO\r\n" }, StringSplitOptions.None);
                                    }
                                    else
                                    {
                                        tokens = new string[] { sqlGenerator.CreateDatabase(mysqlDatabase) + sqlGenerator.SetVariables() };
                                    }

                                    DbCommand command = conn.CreateCommand();
                                    foreach (string sqlString in tokens)
                                    {
                                        command.CommandText = sqlString;
                                        command.ExecuteNonQuery();
                                    }

                                    foreach (Table table in newTables.Values.Where(t => string.IsNullOrEmpty(t.LinkedTable)))
                                    {
                                        //check if cancelled
                                        if (pDialog.IsCancelled)
                                        {
                                            e.Cancel = true;
                                            return;
                                        }
                                        pDialog.ReportProgress(++count, "Inserting SQL");
                                        if (table.Columns.Count > 0)
                                        {
                                            command.CommandText = sqlGenerator.CreateTable(table);
                                            command.ExecuteNonQuery();
                                            List<string> rows = sqlGenerator.InsertTableRowsToList(table);
                                            for (int i = 0; i < rows.Count; i += mysqlNumRowsPerInsert)
                                            {
                                                command.CommandText = sqlGenerator.BeginInsertTable(table);
                                                command.CommandText += String.Join(",", rows.GetRange(i, Math.Min(rows.Count - i, mysqlNumRowsPerInsert))) + ";\n";
                                                command.CommandText += sqlGenerator.EndInsertTable(table);
                                                command.ExecuteNonQuery();
                                                pDialog.ReportProgress(count, "Inserting SQL");
                                            }
                                        }
                                    }

                                    foreach (Table table in newTables.Values.Where(t => !string.IsNullOrEmpty(t.LinkedTable)))
                                    {
                                        //check if cancelled
                                        if (pDialog.IsCancelled)
                                        {
                                            e.Cancel = true;
                                            return;
                                        }
                                        pDialog.ReportProgress(++count, "Inserting SQL");
                                        if (table.Columns.Count > 0)
                                        {
                                            command.CommandText = sqlGenerator.CreateTable(table);
                                            command.ExecuteNonQuery();
                                            List<string> rows = sqlGenerator.InsertTableRowsToList(table);
                                            for (int i = 0; i < rows.Count; i += mysqlNumRowsPerInsert)
                                            {
                                                command.CommandText = sqlGenerator.BeginInsertTable(table);
                                                command.CommandText += String.Join(",", rows.GetRange(i, Math.Min(rows.Count - i, mysqlNumRowsPerInsert))) + ";\n";
                                                command.CommandText += sqlGenerator.EndInsertTable(table);
                                                command.ExecuteNonQuery();
                                                pDialog.ReportProgress(count, "Inserting SQL");
                                            }
                                        }
                                    }

                                    command.CommandText = sqlGenerator.RestoreVariables();
                                    if (!string.IsNullOrEmpty(command.CommandText))
                                    {
                                        command.ExecuteNonQuery();
                                    }
                                    complete = true;
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show(ex.Message);
                                }
                                finally
                                {
                                    conn.Close();
                                }

                                conn.Dispose();
                            }
                        }
                        else if (exportDialog.FileExport)
                        {
                            saveFileDialog1.FileName = "export.sql";
                            result = DialogResult.Cancel;
                            Invoke((MethodInvoker)delegate() { result = saveFileDialog1.ShowDialog(pDialog.Window); });
                            if (result == DialogResult.OK)
                            {
                                InputBox input = null;
                                Invoke((MethodInvoker)delegate() { input = InputBox.Show(pDialog.Window, "Database name?", "Database Name", mysqlDatabase, InputBoxButtons.OKCancel); });
                                if (input.Result == InputBoxResult.OK)
                                {
                                    mysqlDatabase = input.Items["Database Name"];
                                    StreamWriter file = new StreamWriter(saveFileDialog1.FileName, false);
                                    try
                                    {
                                        startTicks = DateTime.Now.Ticks;
                                        file.WriteLine(sqlGenerator.CreateDatabase(mysqlDatabase));
                                        file.WriteLine(sqlGenerator.SetVariables());
                                        foreach (Table table in newTables.Values.Where(t => string.IsNullOrEmpty(t.LinkedTable)))
                                        {
                                            //check if cancelled
                                            if (pDialog.IsCancelled)
                                            {
                                                e.Cancel = true;
                                                return;
                                            }
                                            pDialog.ReportProgress(++count, "Formatting SQL");
                                            if (table.Columns.Count > 0)
                                            {
                                                file.WriteLine(sqlGenerator.CreateTable(table));
                                                file.WriteLine(sqlGenerator.BeginInsertTable(table));
                                                file.WriteLine(sqlGenerator.InsertTableRowsToString(table));
                                                file.WriteLine(sqlGenerator.EndInsertTable(table));
                                            }
                                        }
                                        foreach (Table table in newTables.Values.Where(t => !string.IsNullOrEmpty(t.LinkedTable)))
                                        {
                                            //check if cancelled
                                            if (pDialog.IsCancelled)
                                            {
                                                e.Cancel = true;
                                                return;
                                            }
                                            pDialog.ReportProgress(++count, "Formatting SQL");
                                            if (table.Columns.Count > 0)
                                            {
                                                file.WriteLine(sqlGenerator.CreateTable(table));
                                                file.WriteLine(sqlGenerator.BeginInsertTable(table));
                                                file.WriteLine(sqlGenerator.InsertTableRowsToString(table));
                                                file.WriteLine(sqlGenerator.EndInsertTable(table));
                                            }
                                        }
                                        file.WriteLine(sqlGenerator.RestoreVariables());
                                        complete = true;
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show(ex.ToString());
                                    }
                                    finally
                                    {
                                        file.Close();
                                    }
                                }
                            }
                        }
                    } while (!complete);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    e.Cancel = true;
                }

                exportDialog.Dispose();
            };
            #endregion
            pDialog.ProgressChanged += delegate(object dialog, ProgressChangedEventArgs e)
            {
                if (lastTicks == 0)
                {
                    lastTicks = DateTime.Now.Ticks;
                    timeLeft = "Calculating...";
                }
                else if (e.ProgressPercentage > 0 && DateTime.Now.Ticks > lastTicks + 10000000)
                {
                    lastTicks = DateTime.Now.Ticks;

                    long ticksPassed = lastTicks - startTicks;
                    long thingsCompleted = e.ProgressPercentage;
                    long thingsLeft = total - thingsCompleted;

                    long ticks = thingsLeft * ticksPassed / thingsCompleted;

                    timeLeft = ticksToString(ticks);
                    timeElapsed = ticksToString(ticksPassed);
                }

                pDialog.Message = e.UserState.ToString() + ": " + e.ProgressPercentage + "/" + total + " Time Remaining: " + timeLeft + " Time Elapsed: " + timeElapsed;
                if (total == 0)
                {
                    pDialog.Progress = 0;
                }
                else
                {
                    pDialog.Progress = (100 * e.ProgressPercentage / total) % 101;
                }
            };
            pDialog.Completed += delegate(object dialog, RunWorkerCompletedEventArgs e)
            {
                if (!e.Cancelled)
                {
                    MessageBox.Show("Completed Successfully");
                }
            };
            pDialog.Run();
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                return;
            }

            if (treeView1.SelectedNode.Nodes.Count == 0)
            {
                bExportDocuments_Click(sender, e);
            }
        }

        private string ticksToString(long ticks)
        {
            long seconds = ticks / 10000000;
            long hours = seconds / 3600;
            seconds %= 3600;
            long minutes = seconds / 60;
            seconds %= 60;
            return (hours > 0 ? hours + ":" : "") + (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
        }

        private NotesSession initSession(string password)
        {
            NotesSession nSession = new NotesSession();
            nSession.Initialize(password);
            return nSession;
        }

        private void btnBrowseAttachmentsFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
                txbAttachmentsFolder.Text = folderBrowserDialog1.SelectedPath;            
        }
    }
}

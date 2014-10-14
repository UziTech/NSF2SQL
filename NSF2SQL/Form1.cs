using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using Domino;
using System.IO;
using MySql.Data.MySqlClient;

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

        public Form1()
        {
            InitializeComponent();
            treeView1.TreeViewNodeSorter = new NodeSorter();
            //string[] args = Environment.GetCommandLineArgs();//TODO: cmd line args
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

        private void bGetDatabases_Click(object sender, EventArgs ea)
        {
            treeView1.Nodes.Clear();
            onLocalComputer = false;
            InputBox input = InputBox.Show("Domino Server", new InputBoxItem[] { new InputBoxItem("Server", notesServer), new InputBoxItem("Domain", notesDomain), new InputBoxItem("Password", notesPassword, true) }, InputBoxButtons.OKCancel);
            if (input.Result == InputBoxResult.OK)
            {
                notesServer = input.Values["Server"];
                notesDomain = input.Values["Domain"];
                notesPassword = input.Values["Password"];

                ProgressDialog pDialog = new ProgressDialog();
                pDialog.Title = "Get Databases";
                pDialog.Style = ProgressBarStyle.Marquee;
                pDialog.DoWork += delegate(object dialog, DoWorkEventArgs e)
                {
                    try
                    {
                        NotesSession nSession = initSession(notesPassword);
                        pDialog.ReportProgress(0);
                        if (nSession != null)
                        {
                            NotesDbDirectory directory = nSession.GetDbDirectory(notesServer + "//" + notesDomain);
                            NotesDatabase db = directory.GetFirstDatabase(DB_TYPES.DATABASE);
                            int i = 0;
                            while (db != null && !pDialog.Cancelled)
                            {
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
                    treeView1.Invoke((MethodInvoker)delegate() { treeView1.Sort(); });
                };
                pDialog.Run();
            }
        }

        private void bBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    treeView1.Nodes.Clear();
                    InputBox input = InputBox.Show("Lotus Notes Password", new InputBoxItem("Password", notesPassword, true), InputBoxButtons.OKCancel);
                    if (input.Result == InputBoxResult.OK)
                    {
                        notesPassword = input.Values["Password"];
                        NotesSession nSession = initSession(notesPassword);
                        if (nSession != null)
                        {
                            onLocalComputer = true;
                            foreach (string file in openFileDialog1.FileNames)
                            {
                                NotesDatabase db = nSession.GetDatabase("", file, false);
                                treeView1.Nodes.Add(file, db.Title, "database", "database");
                            }
                            treeView1.Sort();
                        }
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
            int total = 0;
            long startTicks = 0;
            long lastTicks = 0;
            string timeLeft = "";
            string timeElapsed = "0:00:00";
            string filePath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\export.sql";
            string databasePath = treeView1.SelectedNode.Name;
            ProgressDialog pDialog = new ProgressDialog();
            pDialog.Title = "Exporting Documents";
            pDialog.DoWork += delegate(object dialog, DoWorkEventArgs e)
            {
                try
                {
                    //export documents
                    NotesSession nSession = initSession(notesPassword);
                    if (nSession != null)
                    {
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
                            //check if cancelled
                            if (pDialog.Cancelled)
                            {
                                e.Cancel = true;
                                return;
                            }
                            if (doc.HasItem("Form") && (string)doc.GetItemValue("Form")[0] != "")
                            {
                                //get form
                                string form = (string)doc.GetItemValue("Form")[0];

                                if (!tables.ContainsKey(form))
                                {
                                    tables.Add(form, new Table(form));
                                }
                                int row = tables[form].AddRow();
                                //get fields
                                //set multiple values
                                foreach (NotesItem item in doc.Items)
                                {
                                    string field = item.Name;
                                    //exclude fields that start with $ and the Form field
                                    if (field.StartsWith("$") || field == "Form")
                                    {
                                        continue;
                                    }
                                    string type = "";
                                    switch (item.type)
                                    {//TODO: get more types
                                        case IT_TYPE.NUMBERS:
                                            type = "int";
                                            break;
                                        default:
                                            type = "text";
                                            break;
                                    }
                                    object values = item.Values;
                                    bool multiple = item.Values.Length > 1;

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
                                        int id = cell.Key;
                                        object value;
                                        if (cell.Value.GetType().IsArray)
                                        {
                                            object[] valueArray = (object[])cell.Value;
                                            value = valueArray.GetValue(0);
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
                        lastTicks = 0;
                        total = newTables.Count;
                        if (MessageBox.Show("Do you want to export to a server?", "Export to a server?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        {
                            InputBox input = InputBox.Show("SQL server info?", new InputBoxItem[] { new InputBoxItem("Server", mysqlServer), new InputBoxItem("Database", mysqlDatabase), new InputBoxItem("Username", mysqlUsername), new InputBoxItem("Password", mysqlPassword, true) }, InputBoxButtons.OK);
                            if (input.Result == InputBoxResult.OK)
                            {
                                startTicks = DateTime.Now.Ticks;
                                dump2server(newTables, input.Values["Database"], pDialog, input.Values["Server"], input.Values["Username"], input.Values["Password"]);
                            }
                        }
                        else
                        {
                            InputBox input = InputBox.Show("Database name?", "Database Name", mysqlDatabase, InputBoxButtons.OK);
                            if (input.Result == InputBoxResult.OK)
                            {
                                startTicks = DateTime.Now.Ticks;
                                dump2sql(newTables, filePath, input.Values["Database Name"], pDialog);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            };
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

        private string ticksToString(long ticks)
        {
            long seconds = ticks / 10000000;
            long hours = seconds / 3600;
            seconds %= 3600;
            long minutes = seconds / 60;
            seconds %= 60;
            return (hours > 0 ? hours + ":" : "") + (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
        }

        private void dump2sql(Dictionary<string, Table> newTables, string filePath, string databaseName, ProgressDialog pDialog)
        {
            using (StreamWriter file = new StreamWriter(filePath, false))
            {
                int count = 0;
                file.WriteLine("DROP DATABASE IF EXISTS `" + databaseName + "`;");
                file.WriteLine();
                file.WriteLine("CREATE DATABASE `" + databaseName + "`;");
                file.WriteLine("USE `" + databaseName + "`;");
                file.WriteLine();
                file.WriteLine("/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;");
                file.WriteLine("/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;");
                file.WriteLine("/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;");
                file.WriteLine("/*!40101 SET NAMES utf8 */;");
                file.WriteLine("/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;");
                file.WriteLine("/*!40103 SET TIME_ZONE='+00:00' */;");
                file.WriteLine("/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;");
                file.WriteLine("/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;");
                file.WriteLine("/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;");
                file.WriteLine("/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;");
                file.WriteLine();
                foreach (Table table in newTables.Values)
                {
                    pDialog.ReportProgress(++count, "Formatting SQL");
                    if (table.Columns.Count > 0)
                    {
                        file.WriteLine("CREATE TABLE `" + table.Name + "` (");
                        file.WriteLine("`id` INT NOT NULL,");
                        foreach (Column column in table.Columns.Values)
                        {
                            file.WriteLine("`" + column.Name + "` " + column.Type + " NULL,");
                        }
                        file.Write("PRIMARY KEY (`id`)");
                        if (table.LinkedTable != null)
                        {
                            file.WriteLine(",");
                            file.WriteLine("KEY `" + table.Name + "_idx` (`" + table.LinkedTable + "id`),");
                            file.Write("CONSTRAINT `" + table.Name + "` FOREIGN KEY (`" + table.LinkedTable + "id`) REFERENCES `" + table.LinkedTable + "` (`id`) ON DELETE CASCADE ON UPDATE CASCADE");
                        }
                        file.WriteLine(");");
                        file.WriteLine();
                        file.WriteLine("LOCK TABLES `" + table.Name + "` WRITE;");
                        file.WriteLine("/*!40000 ALTER TABLE `" + table.Name + "` DISABLE KEYS */;");
                        file.WriteLine("INSERT INTO `" + table.Name + "` (`id`,`" + String.Join("`,`", table.Columns.Keys) + "`) VALUES ");
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
                                            case "int":
                                                if (value == "")
                                                {
                                                    value = "NULL";
                                                }
                                                else if (value == "Infinity")
                                                {
                                                    value = int.MaxValue.ToString();
                                                }
                                                else
                                                {
                                                    int temp;
                                                    if (!int.TryParse(value, out temp))
                                                    {
                                                        value = "NULL";
                                                    }
                                                }
                                                columnValues.Add(value);
                                                break;
                                            default:
                                                value = value.ToString().Replace("'", "\\'");
                                                columnValues.Add("'" + value + "'");
                                                break;
                                        }
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
                            rows.Add("(" + (i + 1) + "," + String.Join(",", columnValues) + ")");
                        }
                        file.WriteLine(String.Join(",\n", rows) + ";");
                        file.WriteLine("/*!40000 ALTER TABLE `" + table.Name + "` ENABLE KEYS */;");
                        file.WriteLine("UNLOCK TABLES;");
                        file.WriteLine();
                    }
                }
                file.WriteLine("/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;");
                file.WriteLine("/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;");
                file.WriteLine("/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;");
                file.WriteLine("/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;");
                file.WriteLine("/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;");
                file.WriteLine("/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;");
                file.WriteLine("/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;");
                file.WriteLine("/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;");
                file.Close();

                System.Threading.Thread.Sleep(100);//to prevent opening the file before write is complete

                System.Diagnostics.Process.Start(filePath);
            }
        }

        private void dump2server(Dictionary<string, Table> newTables, string databaseName, ProgressDialog pDialog, string server, string username, string password)
        {
            //TODO: max_allowed_packet might cause error
            using (MySqlConnection conn = new MySqlConnection("SERVER=" + server + ";USERNAME=" + username + ";PASSWORD=" + password + ";"))
            {
                conn.Open();

                int count = 0;
                MySqlCommand command = conn.CreateCommand();
                command.CommandText = "DROP DATABASE IF EXISTS `" + databaseName + "`;";

                command.CommandText += "CREATE DATABASE `" + databaseName + "`;";
                command.CommandText += "USE `" + databaseName + "`;";

                command.CommandText += "/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;";
                command.CommandText += "/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;";
                command.CommandText += "/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;";
                command.CommandText += "/*!40101 SET NAMES utf8 */;";
                command.CommandText += "/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;";
                command.CommandText += "/*!40103 SET TIME_ZONE='+00:00' */;";
                command.CommandText += "/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;";
                command.CommandText += "/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;";
                command.CommandText += "/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;";
                command.CommandText += "/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;";

                command.ExecuteNonQuery();
                foreach (Table table in newTables.Values)
                {
                    if (table.Columns.Count > 0)
                    {
                        pDialog.ReportProgress(++count, "Inserting SQL");
                        command.CommandText = "CREATE TABLE `" + table.Name + "` (";
                        command.CommandText += "`id` INT NOT NULL,";
                        foreach (Column column in table.Columns.Values)
                        {
                            command.CommandText += "`" + column.Name + "` " + column.Type + " NULL,";
                        }
                        command.CommandText += "PRIMARY KEY (`id`)";
                        if (table.LinkedTable != null)
                        {
                            command.CommandText += ",";
                            command.CommandText += "KEY `" + table.Name + "_idx` (`" + table.LinkedTable + "id`),";
                            command.CommandText += "CONSTRAINT `" + table.Name + "` FOREIGN KEY (`" + table.LinkedTable + "id`) REFERENCES `" + table.LinkedTable + "` (`id`) ON DELETE CASCADE ON UPDATE CASCADE";
                        }
                        command.CommandText += ");";
                        command.ExecuteNonQuery();

                        command.CommandText = "LOCK TABLES `" + table.Name + "` WRITE;";
                        command.CommandText += "/*!40000 ALTER TABLE `" + table.Name + "` DISABLE KEYS */;";
                        command.CommandText += "INSERT INTO `" + table.Name + "` (`id`,`" + String.Join("`,`", table.Columns.Keys) + "`) VALUES ";
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
                                            case "int":
                                                if (value == "")
                                                {
                                                    value = "NULL";
                                                }
                                                else if (value == "Infinity")
                                                {
                                                    value = int.MaxValue.ToString();
                                                }
                                                else
                                                {
                                                    int temp;
                                                    if (!int.TryParse(value, out temp))
                                                    {
                                                        value = "NULL";
                                                    }
                                                }
                                                columnValues.Add(value);
                                                break;
                                            default:
                                                value = value.ToString().Replace("'", "\\'");
                                                columnValues.Add("'" + value + "'");
                                                break;
                                        }
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
                            rows.Add("(" + (i + 1) + "," + String.Join(",", columnValues) + ")");
                            if (i % 1000 == 999 && i != table.RowCount - 1)//TODO: maybe change this? this is to prevent max_allowed_packet error
                            {
                                command.CommandText += String.Join(",", rows) + ";";
                                command.CommandText += "/*!40000 ALTER TABLE `" + table.Name + "` ENABLE KEYS */;";
                                command.CommandText += "UNLOCK TABLES;";
                                command.ExecuteNonQuery();

                                command.CommandText = "LOCK TABLES `" + table.Name + "` WRITE;";
                                command.CommandText += "/*!40000 ALTER TABLE `" + table.Name + "` DISABLE KEYS */;";
                                command.CommandText += "INSERT INTO `" + table.Name + "` (`id`,`" + String.Join("`,`", table.Columns.Keys) + "`) VALUES ";
                                rows.Clear();
                            }
                        }
                        command.CommandText += String.Join(",", rows) + ";";
                        command.CommandText += "/*!40000 ALTER TABLE `" + table.Name + "` ENABLE KEYS */;";
                        command.CommandText += "UNLOCK TABLES;";
                        command.ExecuteNonQuery();
                    }
                }
                command.CommandText = "/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;";
                command.CommandText += "/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;";
                command.CommandText += "/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;";
                command.CommandText += "/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;";
                command.CommandText += "/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;";
                command.CommandText += "/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;";
                command.CommandText += "/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;";
                command.CommandText += "/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;";
                command.ExecuteNonQuery();
                conn.Close();
            }
        }
        private NotesSession initSession(string password)
        {
            NotesSession nSession = new NotesSession();
            try
            {
                nSession.Initialize(password);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
            return nSession;
        }

        private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (treeView1.SelectedNode.Nodes.Count == 0)
            {
                bExportDocuments_Click(sender, e);
            }
        }

        //private void listBox1_DoubleClick(object sender, EventArgs e)
        //{
        //    //NotesDatabase db1 = dbArray[listBox1.SelectedIndices[0]];
        //    //NotesDatabase db2 = dbArray[listBox1.SelectedIndices[0] + 1];
        //    //List<string> sa1 = new List<string>();
        //    //List<string> sa2 = new List<string>();
        //    //List<string> diff = new List<string>();
        //    //foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(db1))
        //    //{
        //    //    try
        //    //    {
        //    //        v1 = descriptor.GetValue(db1);
        //    //        v2 = descriptor.GetValue(db2);
        //    //        sa1.Add(descriptor.Name + ": " + v1);
        //    //        sa2.Add(descriptor.Name + ": " + v2);
        //    //        if (v1 != v2)
        //    //        {
        //    //            diff.Add(descriptor.Name + ": " + v1 + " : " + v2);
        //    //        }
        //    //    }
        //    //    catch { }
        //    //}
        //    //var temp = 0;
        //}

        ////modified from http://searchdomino.techtarget.com/tip/Get-all-field-names-of-a-Notes-form
        //List<NotesForm> getForms(NotesDatabase db)
        //{
        //    // **************************************************************************
        //    // * Given server (strServer) and path (strPath) of a database
        //    // * this routine returns a list of all forms available in this database
        //    // * The if-clause marked with ### is used to exclude the hidden
        //    // * and system forms from the choice list.
        //    // **************************************************************************
        //    List<NotesForm> forms = new List<NotesForm>();
        //    foreach (NotesForm frm in db.Forms)
        //    {
        //        //if (((frm.Name.Substring(0, 1) != "$") && (frm.Name.Substring(0, 1) != "(")))
        //        //{
        //        forms.Add(frm);
        //        //}
        //    }
        //    return forms;
        //}

        ////modified from http://searchdomino.techtarget.com/tip/Get-all-field-names-of-a-Notes-form
        //List<string> getFields(NotesForm form)
        //{
        //    // **************************************************************************
        //    // * Given server (strServer) and path (strPath) of a database and
        //    // * the name or alias (strForm) of a form in this database
        //    // * this routine returns a list of all fields available in this form
        //    // * The if-clause marked with ### is used to exclude the hidden
        //    // * and system forms from the choice list.
        //    // **************************************************************************
        //    List<string> fields = new List<string>(form.Fields);
        //    return fields;
        //}
        //string getFormName(NotesForm form)
        //{

        //    if (form.Aliases.GetType().IsArray)
        //    {
        //        if ((form.Aliases[0] == ""))
        //        {
        //            return form.Name;
        //            // no alias = take the name of the form
        //        }
        //        else
        //        {
        //            return form.Aliases[form.Aliases.Length - 1];
        //            // The last alias in the list is normally the one who is written into the form field of corresponding documents
        //        }
        //    }
        //    return form.Name;
        //}
        //List<NotesDocument> getFormDocuments(NotesDatabase db, string formName)
        //{
        //    List<NotesDocument> documents = new List<NotesDocument>();
        //    for (int i = 0; i < db.AllDocuments.Count; i++)
        //    {
        //        NotesDocument document = db.AllDocuments.GetNthDocument(i);
        //        if (document.Items.Form == formName)
        //        {
        //            documents.Add(document);
        //        }
        //    }
        //    return documents;
        //}
    }
}

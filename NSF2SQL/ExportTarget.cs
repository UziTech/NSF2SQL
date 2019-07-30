using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NSF2SQL
{
    public partial class ExportTarget : Form
    {
        public ExportTarget()
        {
            InitializeComponent();

            this.ServerTypeComboBox.SelectedIndex = 0;
        }

        public bool FileExport
        {
            get
            {
                return this.FileRadioButton.Checked;
            }
        }

        public bool ServerExport
        {
            get
            {
                return this.ServerRadioButton.Checked;
            }
        }

        public bool MySqlExport
        {
            get
            {
                return this.ServerTypeComboBox.SelectedIndex == 0;
            }
        }

        public bool SqlServerExport
        {
            get
            {
                return this.ServerTypeComboBox.SelectedIndex == 1;
            }
        }
    }
}

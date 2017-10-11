using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RealtimeAnalysis
{
    public partial class CloseForm : Form
    {
        private Form form1;
        public CloseForm(Form mainform)
        {
            InitializeComponent();
            form1 = mainform;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim()=="hisingpower")
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                //form1.Close();
                //form1.Dispose();
                //Application.Exit();
            }
            else
            {
                MessageBox.Show("密码不正确，请联系管理员！");
                //form1.Show();
                this.Close();
            }
        }
    }
}

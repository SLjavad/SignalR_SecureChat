using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class frmRegister : Form
    {
        public frmRegister(string message)
        {
            InitializeComponent();
            lblMessage.Text = message;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void frmRegister_Load(object sender, EventArgs e)
        {

        }
        public string Username { get; set; }
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtUsername.Text.Trim()))
            {
                MessageBox.Show("Enter your usename");
                return;
            }
            Username = txtUsername.Text.Trim();
        }
    }
}

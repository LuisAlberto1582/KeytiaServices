using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace KeytiaUtil
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            textBox1.GotFocus += new EventHandler(textBox1_GotFocus);
            textBox2.GotFocus += new EventHandler(textBox2_GotFocus);
        }

        void textBox1_GotFocus(object sender, EventArgs e)
        {
            textBox1.SelectAll();
        }

        void textBox2_GotFocus(object sender, EventArgs e)
        {
            textBox2.SelectAll();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Text = KeytiaUtilLib.KeytiaCrypto.Encrypt(textBox1.Text);
            textBox2.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                textBox2.Text = KeytiaUtilLib.KeytiaCrypto.Decrypt(textBox1.Text);
            }
            catch (Exception ex)
            {
                MessageBox.Show("No fue posible desencriptar.", "Keytia Util", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }

            textBox2.Focus();
        }

    }
}

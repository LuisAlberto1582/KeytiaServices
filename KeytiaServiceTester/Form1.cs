using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace KeytiaServiceTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form f = new Form2();
            f.ShowDialog(this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form f = new Form3();
            f.ShowDialog(this);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form f = new Form4();
            f.ShowDialog(this);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Form f = new Form5();
            f.ShowDialog(this);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form f = new Form6();
            f.ShowDialog(this);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Form f = new Form7();
            f.ShowDialog(this);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form f = new Form8();
            f.ShowDialog(this);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Form f = new AltaUsuarNextel();
            f.ShowDialog(this);
        }
    }
}

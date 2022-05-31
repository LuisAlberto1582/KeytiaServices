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
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //instanciar la clase y llamar el método a probar
            KeytiaServiceBL.CargaCDR.CargaCDRCisco.CargaCDRCiscoNemak CargaNemak;
            CargaNemak = new KeytiaServiceBL.CargaCDR.CargaCDRCisco.CargaCDRCiscoNemak();
            CargaNemak.CodCarga = 79675;
            CargaNemak.IniciarCarga();
            this.Close();
        }
    }
}

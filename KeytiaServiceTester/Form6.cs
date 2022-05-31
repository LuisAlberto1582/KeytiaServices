using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using KeytiaServiceBL;

namespace KeytiaServiceTester
{
    public partial class Form6 : Form
    {
        public Form6()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            FileReaderCSV frcsv = new FileReaderCSV();
            string[] valores;

            frcsv.Abrir("c:\\temp\\K5\\testcsv.txt");

            while ((valores = frcsv.SiguienteRegistro()) != null)
            {
                Console.WriteLine(valores[0]);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            KDBAccess kdb = new KDBAccess();
            Hashtable lht = new Hashtable();

            lht.Add("{Client}", 280);
            lht.Add("{Paque}", 76287);

            kdb.Insert("relaciones", "Cliente - Paquete", lht);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            KDBAccess kdb = new KDBAccess();
            DataTable ldt;

            ldt = kdb.GetHisRegByEnt("", "Entidades");
        }
    }
}

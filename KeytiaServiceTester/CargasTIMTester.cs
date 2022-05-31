using KeytiaServiceBL;
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
    public partial class CargasTIMTester : Form
    {
        public CargasTIMTester()
        {
            InitializeComponent();
        }

        private void btnIniciarCarga_Click(object sender, EventArgs e)
        {
            txtEsquema.Visible = false;
            txtClaveCarga.Visible = false;

            string esquema = "FarmaciasDelAhorro";
            string claveCarga = "ho1a_202001_spt";

            int icodCatUsuarDB;
            int icodCatCargaCDR;
            KeytiaServiceBL.DSODataContext.SetContext();
            icodCatUsuarDB =
                (int)DSODataAccess.ExecuteScalar(string.Format("select icodcatalogo from Keytia.HistUsuarDB where dtfinvigencia>=getdate() and esquema = '{0}'", esquema));
            KeytiaServiceBL.DSODataContext.SetContext(icodCatUsuarDB);


            icodCatCargaCDR =
                (int)DSODataAccess.ExecuteScalar(string.Format("select icodcatalogo from HistCargas where dtfinvigencia>=getdate() and vchcodigo = '{0}'", claveCarga));


            var Carga = new KeytiaServiceBL.CargaFacturas.CargaFacturaHO1ATIM.CargaFacturaHO1ATIM();

            Carga.CodUsuarioDB = icodCatUsuarDB;
            Carga.CodCarga = icodCatCargaCDR;
            Carga.IniciarCarga();

            MessageBox.Show("Carga finalizada");
        }

        private void CargasTIMTester_Load(object sender, EventArgs e)
        {

        }
    }
}

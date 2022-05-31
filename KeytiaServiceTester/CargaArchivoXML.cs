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
    public partial class CargaArchivoXML : Form
    {
        public CargaArchivoXML()
        {
            InitializeComponent();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            string esquema = txtEsquema.Text;
            string claveCarga = txtClaveCarga.Text;

            int icodCatUsuarDB;
            int icodCatCargaCDR;
            KeytiaServiceBL.DSODataContext.SetContext();
            icodCatUsuarDB =
                (int)DSODataAccess.ExecuteScalar(string.Format("select icodcatalogo from Keytia.[vishistoricos('UsuarDB','Usuarios DB','Español')] where dtfinvigencia>=getdate() and esquema = '{0}'", esquema));
            KeytiaServiceBL.DSODataContext.SetContext(icodCatUsuarDB);           


            icodCatCargaCDR =
                (int)DSODataAccess.ExecuteScalar(string.Format("select icodcatalogo from [vishistoricos('cargas','Cargas Factura XML TIM','español')] where dtfinvigencia>=getdate() and vchcodigo = '{0}'", claveCarga));

            var Carga = new KeytiaServiceBL.CargaFacturas.CargaFacturaXMLTIM.CargaFacturaXMLTIM();

            Carga.CodUsuarioDB = icodCatUsuarDB;
            Carga.CodCarga = icodCatCargaCDR;
            Carga.IniciarCarga();

            MessageBox.Show("Carga finalizada");
        }

       
    }

    
}

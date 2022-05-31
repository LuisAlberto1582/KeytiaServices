using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KeytiaServiceBL.CargaFacturas;
using KeytiaServiceBL.CargaRecursos;

namespace KeytiaServiceTester
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void btnCargaFctTelmex_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.DSODataContext.SetContext(77215);
            CargaFacturaTelmex lCarga = new CargaFacturaTelmex();
            lCarga.CodUsuarioDB = 77215;
            lCarga.CodCarga = 206735;
            lCarga.IniciarCarga();
        }

        private void btnCargaFctAlestra_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.DSODataContext.SetContext(77214);
            CargaFacturaAlestra lCarga = new CargaFacturaAlestra();
            lCarga.CodCarga = 72802;
            lCarga.IniciarCarga();
        }

        private void btnCargaFctTelcel_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.DSODataContext.SetContext(77214);
            CargaFacturaTelcel lCarga = new CargaFacturaTelcel();
            lCarga.CodCarga = 80725;
            lCarga.IniciarCarga();
        }

        private void btnCargaFctMovistar_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.DSODataContext.SetContext(77214);
            CargaFacturaMovistar lCarga = new CargaFacturaMovistar();
            lCarga.CodCarga = 72804;
            lCarga.IniciarCarga();
        }

        private void btnCargaFctATT_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.DSODataContext.SetContext(77214);
            CargaFacturaATT lCarga = new CargaFacturaATT();
            lCarga.CodCarga = 72805;
            lCarga.IniciarCarga();
        }

        private void btnCargaFctAxtel_Click(object sender, EventArgs e)
        {
            //KeytiaServiceBL.DSODataContext.SetContext(77214);
            //CargaFacturaAxtel lCarga = new CargaFacturaAxtel();
            //lCarga.CodCarga = 79681;
            //lCarga.IniciarCarga();

            KeytiaServiceBL.DSODataContext.SetContext(79484);
            CargaFacturaAxtelV2 lCarga = new CargaFacturaAxtelV2();
            lCarga.CodCarga = 880688; // Pendiente
            lCarga.IniciarCarga();
        }

        private void btnCargaEmpleado_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.DSODataContext.SetContext(77215);
            CargaServicioEmpleado lCarga = new CargaServicioEmpleado();
            lCarga.CodUsuarioDB = 77215;
            lCarga.CodCarga = 206755;
            lCarga.IniciarCarga();
        }

        private void btnCargaCC_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.DSODataContext.SetContext(77215);
            CargaServicioCentroCosto lCarga = new CargaServicioCentroCosto();
            lCarga.CodUsuarioDB = 77215;
            lCarga.CodCarga = 206751;
            lCarga.IniciarCarga();
        }

        private void btnCargaResponsable_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.DSODataContext.SetContext(77214);
            CargaRespEmpleado lCarga = new CargaRespEmpleado();
            lCarga.CodCarga = 89643;
            lCarga.IniciarCarga();
        }

        private void btnCargaRespCC_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.DSODataContext.SetContext(77214);
            CargaRespCentroCosto lCarga = new CargaRespCentroCosto();
            lCarga.CodCarga = 89643;
            lCarga.IniciarCarga();
        }

        private void btnCargaFctEqComputo_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.DSODataContext.SetContext(77214);
            CargaFacturaEqComputo lCarga = new CargaFacturaEqComputo();
            lCarga.CodCarga = 80724;
            lCarga.IniciarCarga();
        }

        private void btnCargaFctAudioVideo_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.DSODataContext.SetContext(77214);
            CargaFacturaAudioVideo lCarga = new CargaFacturaAudioVideo();
            lCarga.CodCarga = 80725;
            lCarga.IniciarCarga();
        }

        private void btnCargaFctVPNet_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.DSODataContext.SetContext(77214);
            CargaFacturaVPNet lCarga = new CargaFacturaVPNet();
            lCarga.CodUsuarioDB = 77214;
            lCarga.CodCarga = 203536;
            lCarga.IniciarCarga();
        }

        private void btnCargaFctNextel_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.DSODataContext.SetContext(77214);
            CargaFacturaNextel lCarga = new CargaFacturaNextel();
            lCarga.CodUsuarioDB = 77214;
            lCarga.CodCarga = 203546;
            lCarga.IniciarCarga();
        }

        private void btnCargaFctIusacell_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.DSODataContext.SetContext(77215);
            CargaFacturaIusacell lCarga = new CargaFacturaIusacell();
            lCarga.CodUsuarioDB = 77215;
            lCarga.CodCarga = 219613;
            lCarga.IniciarCarga();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.DSODataContext.SetContext(79482);
            KeytiaServiceBL.CargaFacturas.CargaFacturaTotalPlayTIM.CargaFacturaTotalPlayTIM lCarga =
                new KeytiaServiceBL.CargaFacturas.CargaFacturaTotalPlayTIM.CargaFacturaTotalPlayTIM();
            lCarga.CodUsuarioDB = 79482;
            lCarga.CodCarga = 290244;
            lCarga.IniciarCarga();

            MessageBox.Show("Carga finalizada");
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            KeytiaServiceBL.DSODataContext.SetContext(77703);
            CargaFacturaPersonal lCarga = new CargaFacturaPersonal();
            lCarga.CodUsuarioDB = 77703;
            lCarga.CodCarga = 203546;
            lCarga.IniciarCarga();
        }
    }
}

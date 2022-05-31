using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using KeytiaServiceBL;
using KeytiaServiceBL.Alarmas;
using System.Linq.Expressions;
using System.Linq;

namespace KeytiaServiceTester
{
    public partial class Form5 : Form
    {
        public Form5()
        {
            InitializeComponent();
        }


        private void button1_Click_1(object sender, EventArgs e)
        {
            int icodCatUsuarDB = 97909; //NZ AirLiquide
            int icodCatCarga = 202750;

            KeytiaServiceBL.DSODataContext.SetContext(icodCatUsuarDB);

            var CargaEmpleado = new KeytiaServiceBL.CargaRecursos.CargaServicioEmpleado();

            CargaEmpleado.CodUsuarioDB = icodCatUsuarDB;
            CargaEmpleado.CodCarga = icodCatCarga;
            CargaEmpleado.IniciarCarga();

            MessageBox.Show("Carga finalizada");
        }

        private void Form5_Load(object sender, EventArgs e)
        {

        }

        private void cmdCargaCDRSYO_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.DSODataContext.SetContext(88235);

            KeytiaServiceBL.CargaFacturas.CargaFacturaVideoConferencia CargaBanRegio;

            CargaBanRegio = new KeytiaServiceBL.CargaFacturas.CargaFacturaVideoConferencia();
            CargaBanRegio.CodUsuarioDB = 88235;

            CargaBanRegio.CodCarga = 306687;
            CargaBanRegio.IniciarCarga();

            MessageBox.Show("Carga finalizada");
        }



        private void btnCargaCDR_Click(object sender, EventArgs e)
        {
            try
            {
                Control control = ((Control)sender);
                control.BackColor = Color.Green;

                string esquema = "Laureate";
                string claveCarga = "PruebasLlamsEnt01";

                int icodCatUsuarDB;
                int icodCatCargaCDR;
                KeytiaServiceBL.DSODataContext.SetContext();
                icodCatUsuarDB =
                    (int)DSODataAccess.ExecuteScalar(string.Format("select icodcatalogo from Keytia.[vishistoricos('UsuarDB','Usuarios DB','Español')] where dtfinvigencia>=getdate() and esquema = '{0}'", esquema));
                KeytiaServiceBL.DSODataContext.SetContext(icodCatUsuarDB);


                icodCatCargaCDR =
                    (int)DSODataAccess.ExecuteScalar(string.Format("select icodcatalogo from [vishistoricos('cargas','Cargas CDRs','español')] where dtfinvigencia>=getdate() and vchcodigo = '{0}'", claveCarga));

                var Carga =
                    new KeytiaServiceBL.CargaCDR.CargaCDRCisco.CargaCDRCiscoLaureate();

                KeytiaServiceBL.DSODataContext.SetContext(icodCatUsuarDB);

                Carga.CodUsuarioDB = icodCatUsuarDB;
                Carga.CodCarga = icodCatCargaCDR;


                EliminarInfoCarga(esquema, icodCatCargaCDR);

                Carga.IniciarCarga();

                control.BackColor = Color.DimGray;

                MessageBox.Show("Carga finalizada");
            }
            catch (Exception ex)
            {
                
                MessageBox.Show(ex.Message);
            }
        }

        void EliminarInfoCarga(string esquema, int iCodCatCarga, int iCodCatEstCarga = 7287)
        {

            StringBuilder sb = new StringBuilder();

            sb.Length = 0;
            sb.AppendFormat("delete {0}.[visDetallados('detall','detallecdr','español')] where icodcatalogo={1}", esquema, iCodCatCarga.ToString());
            DSODataAccess.ExecuteNonQuery(sb.ToString());

            sb.Length = 0;
            sb.AppendFormat("delete {0}.DetalleCDRComplemento where icodcatalogo={1}", esquema, iCodCatCarga.ToString());
            DSODataAccess.ExecuteNonQuery(sb.ToString());

            sb.Length = 0;
            sb.AppendFormat("delete {0}.DetalleCDREnt where icodcatalogo={1}", esquema, iCodCatCarga.ToString());
            DSODataAccess.ExecuteNonQuery(sb.ToString());

            sb.Length = 0;
            sb.AppendFormat("delete {0}.DetalleCDREnl where icodcatalogo={1}", esquema, iCodCatCarga.ToString());
            DSODataAccess.ExecuteNonQuery(sb.ToString());

            sb.Length = 0;
            sb.AppendFormat("delete {0}.[vispendientes('detall','detallecdr','español')] where icodcatalogo={1}", esquema, iCodCatCarga.ToString());
            DSODataAccess.ExecuteNonQuery(sb.ToString());

            sb.Length = 0;
            sb.AppendFormat("update {0}.[vishistoricos('cargas','cargas cdrs','español')] ", esquema);
            sb.AppendFormat(" set EstCarga = {0}, ", iCodCatEstCarga.ToString()); //eliminada: 7288, --inicializada: 7287, --en espera 79194
            sb.AppendLine(" fechainicio=NULL, "); 
            sb.AppendLine(" fechafin=NULL, "); 
            sb.AppendLine(" initasacion=NULL, "); 
            sb.AppendLine(" fintasacion=NULL, "); 
            sb.AppendLine(" durtasacion = NULL, "); 
            sb.AppendLine(" dtfinvigencia='2079-01-01' "); 
            sb.AppendFormat(" where icodcatalogo={0}", iCodCatCarga.ToString());
            DSODataAccess.ExecuteNonQuery(sb.ToString());
        }

        private void btnCargaTelcel_Click(object sender, EventArgs e)
        {
            int icodCatUsuarDB = 97909;
            int icodCatCargaCDR = 1620173;

            KeytiaServiceBL.DSODataContext.SetContext(icodCatUsuarDB);

            var Carga = new KeytiaServiceBL.CargaGenerica.CargaEnlacesCajeros.CargaEnlacesCajeros();

            Carga.CodUsuarioDB = icodCatUsuarDB;
            Carga.CodCarga = icodCatCargaCDR;
            Carga.IniciarCarga();

            MessageBox.Show("Carga finalizada");
        }

        private void btnLanzaAlarma_Click(object sender, EventArgs e)
        {
            //int icodCatUsuarDB = 77704;
            //int icodCatAlarma = 298350;

            //KDBAccess kdb = new KDBAccess();

            //KeytiaServiceBL.DSODataContext.SetContext(icodCatUsuarDB);



            ////Obtiene el listado de alarmas activas que corresponden al maestro en curso
            //DataTable ldtAlarmas = kdb.GetHisRegByEnt("Alarm", "Alarma Diaria", "iCodCatalogo = 298350");
            ////Se asigna a la variable loClaseAlarma, el valor de la enumeración que corresponda 
            ////según el maestro en curso
            //LanzadorAlarmas lanzador = new LanzadorAlarmas();
            //LanzadorAlarmas.ClaseAlarma loClaseAlarma = lanzador.getClaseAlarma(ldtAlarmas);

            //DataRow drAlarma = ldtAlarmas.Select(" icodcatalogo = " + icodCatAlarma.ToString()).FirstOrDefault();

            //DestAlarma loDestAlarma = lanzador.getDestAlarma(drAlarma, loClaseAlarma);
            //loDestAlarma.iCodUsuarioDB = icodCatUsuarDB;
            //loDestAlarma.Main();


            ////Carga.CodUsuarioDB = icodCatUsuarDB;

            //MessageBox.Show("Carga finalizada");
        }

        private void cmdCargaCDRSYOv2_Click(object sender, EventArgs e)
        {

            KeytiaServiceBL.DSODataContext.SetContext(97977);

            KeytiaServiceBL.CargaGenerica.CargaCDRSeeYouOn.CargaCDRSeeYouOn carga;

            carga = new KeytiaServiceBL.CargaGenerica.CargaCDRSeeYouOn.CargaCDRSeeYouOn();
            carga.CodUsuarioDB = 97977;

            carga.CodCarga = 203594;
            carga.IniciarCarga();


            MessageBox.Show("Carga finalizada");
        }

        private void btnCargaUris_Click(object sender, EventArgs e)
        {
            KeytiaServiceBL.DSODataContext.SetContext(97977);



            var carga = new KeytiaServiceBL.CargaGenerica.CargaUriSYO.CargaUriSYO();
            carga.CodUsuarioDB = 97977;

            carga.CodCarga = 203594;
            carga.IniciarCarga();


            MessageBox.Show("Carga finalizada");
        }

        private void btnCargaXMLTIM_Click(object sender, EventArgs e)
        {
            string esquema = "Qualtia";
            string claveCarga = "XMLAlestraCarnesP014663519 201908";

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

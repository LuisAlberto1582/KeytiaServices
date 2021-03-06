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
    public partial class CargaMasivaArchivosDatosFactura : Form
    {
        public CargaMasivaArchivosDatosFactura()
        {
            InitializeComponent();
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            int icodCatUsuarDB;
            string esquema = "Qualtia";

            KeytiaServiceBL.DSODataContext.SetContext();
            icodCatUsuarDB =
                (int)DSODataAccess.ExecuteScalar(string.Format("select icodcatalogo from Keytia.[vishistoricos('UsuarDB','Usuarios DB','Español')] where dtfinvigencia>=getdate() and esquema = '{0}'", esquema));
            KeytiaServiceBL.DSODataContext.SetContext(icodCatUsuarDB);


            var cargas = GetCargas();

            foreach (var carga in cargas)
            {
                var Carga = new KeytiaServiceBL.CargaFacturas.CargaFacturaAlestraTIM.CargaFacturaAlestraTIM
                {
                    CodUsuarioDB = icodCatUsuarDB,
                    CodCarga = carga.ICodCatalogo
                };

                Carga.IniciarCarga();
            }

            MessageBox.Show("Se han finalizado todas las cargas");
        }


        List<HistoricoCarga> GetCargas()
        {
            var cargas = new List<HistoricoCarga>();
            var dtResult = new DataTable();

            StringBuilder query = new StringBuilder();
            query.AppendLine("select iCodCatalogo, vchcodigo ");
            query.AppendLine("from [VisHistoricos('Cargas','Cargas Factura Alestra TIM','Español')] ");
            query.AppendLine("where Empre = 200002 ");
            query.AppendLine("and dtFinVigencia>=GETDATE() ");
            query.AppendLine("and EstCarga = 7287 ");
            query.AppendLine("order by Archivo01");
            dtResult = DSODataAccess.Execute(query.ToString());

            if (dtResult != null && dtResult.Rows.Count > 0)
            {
                foreach (DataRow dr in dtResult.Rows)
                {
                    cargas.Add(new HistoricoCarga
                    {
                        ICodCatalogo = (int)dr["iCodCatalogo"],
                        VchCodigo = dr["vchCodigo"].ToString()
                    }
                    );
                }
            }

            return cargas;
        }
    }

    
}

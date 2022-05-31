using System;
using System.Web.UI.WebControls;
using System.Data;
using KeytiaServiceBL;
using System.Web;

namespace KeytiaWeb.UserInterface.RepTernium
{
    public partial class ConsultaSolicitudRepTernium : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            ObtenerCargas();
        }

        private void ObtenerCargas()
        {
            string strQuery = "select * from [Ternium].[vishistoricos('Cargas','Carga Dicomtec','Español')] order by icodregistro desc";
            DataTable res = DSODataAccess.Execute(strQuery);
            if (res.Rows.Count > 0)
            {
                rowGrv.Visible = true;
                InfoPanelSucces.Visible = false;
                grvListadoCargas.DataSource = res;
                grvListadoCargas.DataBind();
            }
            else
            {
                lblMensajeSuccess.Text = "No existen Cargas Actualmente";
                InfoPanelSucces.Visible = true;
                rowGrv.Visible = false;
            }
        }

        protected void btnConsultar_Click(object sender, EventArgs e)
        {

        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Redirect("~/UserInterface/RepTernium/ConfigSolicitudRepTernium.aspx", false);
        }

        private void EliminarCarga(int intICodCatalogo)
        {
            try
            {
                /*int icodCatUsuarDB = 77703;//icodcatalogo usardb Bat
                int icodCatCargaCDR = 488326;//icodcatalogo vistas carga

                DSODataContext.SetContext(icodCatUsuarDB);*/

                int icodCatUsuarDB = 77703;

                var Carga = new KeytiaServiceBL.CargaGenerica.CargaDicomtec.CargaDicomtec
                {
                    CodUsuarioDB = icodCatUsuarDB,
                    CodCarga = intICodCatalogo
                };

                //Carga.IniciarCarga();

                Carga.EliminarCarga(intICodCatalogo);
            }
            catch (Exception ex)
            {

            }
        }

        protected void lnkEliminar_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            ImageButton lnkbtn = sender as ImageButton;
            int rowIndex = Convert.ToInt32(lnkbtn.Attributes["RowIndex"]);
            GridViewRow selectedRow = (GridViewRow)grvListadoCargas.Rows[rowIndex];
            int intICodCatalogo = (int)grvListadoCargas.DataKeys[rowIndex].Values[0];
            EliminarCarga(intICodCatalogo);
        }
    }
}
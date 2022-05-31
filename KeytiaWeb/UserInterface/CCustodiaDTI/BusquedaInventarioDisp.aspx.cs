using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KeytiaServiceBL;
using System.Text;
using System.Data;

namespace KeytiaWeb.UserInterface.CCustodiaDTI
{
    public partial class BusquedaInventarioDisp : System.Web.UI.Page
    {
        protected DataTable dtTipoInventario;
        protected DataTable dtEstatusCCustodia;
        protected DataTable dtResultadoBusqueda;
        protected StringBuilder psbQuery = new StringBuilder();
        //RZ.20131203 Campo para almacenar el estatus de la busqueda de la carta custodia.
        private string estadoInv;

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            string lsStyleSheet = (string)HttpContext.Current.Session["StyleSheet"];

            //RZ.20131203 Leer el estatus de la pagina de busqueda 
            /*Estados para la busqueda 1:Busqueda 2:Resultado
             */
            if (string.IsNullOrEmpty(Request.QueryString["stAI"]))
            {
                estadoInv = "1";
            }
            else
            {
                estadoInv = Request.QueryString["stAI"];
            }

            /*Se agrega css desde servidor para leerlo desde la carpeta de estilos elegida por la configuracion en K5*/
            Page.Header.Controls.Add(new LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + ResolveUrl(lsStyleSheet + "/CCustodia.css") + "\" />"));

            if (!Page.IsPostBack)
            {
                FillDropDowns();

                if (Session["QueryBusqInv"] != null && estadoInv == "2")
                {
                    EjecutarQueryBusqueda(Session["QueryBusqInv"].ToString());
                    refillControlesFiltros();

                }
            }

        }

        protected void refillControlesFiltros()
        {
            if (Session["EstFiltro"] != null)
            {
                drpEstatus.SelectedValue = Session["EstFiltro"].ToString();    
            }

            if (Session["drpMarcaValue"] != null)
            {
                //drpMarca.SelectedValue = Session["drpMarcaValue"].ToString();
                ccdMarca.SelectedValue = Session["drpMarcaValue"].ToString();
            }

            if (Session["drpModeloValue"] != null)
            {
                //drpModelo.SelectedValue = Session["drpModeloValue"].ToString();
                ccdModelo.SelectedValue = Session["drpModeloValue"].ToString();
            }

            if (Session["drpTipoAparatoValue"] != null)
            {
                drpTipoAparato.SelectedValue = Session["drpTipoAparatoValue"].ToString();
            }

            if (Session["txtNoSerieValue"] != null)
            {
                txtNoSerie.Text = Session["txtNoSerieValue"].ToString();
            }

            if (Session["txtMacAddressValue"] != null)
            {
                txtMacAddress.Text = Session["txtMacAddressValue"].ToString();
            }
        }

        protected void FillDropDowns()
        {
            FillDDLEstatusCC();

            FillDDLTipoInventario();
        
        }

        protected void FillDDLTipoInventario()
        {
            StringBuilder lsbQuery = new StringBuilder();

            lsbQuery.AppendLine("SELECT vchDescripcion = Español, iCodCatalogo ");
            lsbQuery.AppendLine("FROM [VisHistoricos('TipoDispositivo','Tipos de Dispositivo','Español')] ");
            lsbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia ");
            lsbQuery.AppendLine("and dtFinVigencia >= GETDATE() ");
            lsbQuery.AppendLine("order by vchDescripcion");

            dtTipoInventario = DSODataAccess.Execute(lsbQuery.ToString());

            drpTipoAparato.DataSource = dtTipoInventario;
            drpTipoAparato.DataValueField = "iCodCatalogo";
            drpTipoAparato.DataTextField = "vchDescripcion";
            drpTipoAparato.DataBind();
        }

        protected void FillDDLEstatusCC()
        {
            psbQuery.Length = 0;

            psbQuery.AppendLine("SELECT iCodCatalogo, vchDescripcion = Español ");
            psbQuery.AppendLine("FROM [VisHistoricos('Estatus','Estatus dispositivo','Español')] ");
            psbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia ");
            psbQuery.AppendLine("and dtFinVigencia >= GETDATE()");

            dtEstatusCCustodia = DSODataAccess.Execute(psbQuery.ToString());

            drpEstatus.DataSource = dtEstatusCCustodia;
            drpEstatus.DataValueField = "iCodCatalogo";
            drpEstatus.DataTextField = "vchDescripcion";
            drpEstatus.DataBind();

            //drpEstatus.Items.Insert(0, new ListItem("TODOS", "0"));
        }

        protected void lbtnBusqueda_Click(object sender, EventArgs e)
        {
            //RZ.20130715 Antes de realizar una nueva busqueda, limpio las variables de sesion.
            removerSessionVar();
            FormarQueryBusqueda();

            if (!String.IsNullOrEmpty(txtNoSerie.Text))
            {
                psbQuery.AppendLine("and NSerie like '%" + txtNoSerie.Text + "%' ");
                Session["txtNoSerieValue"] = txtNoSerie.Text;
            }

            if (!String.IsNullOrEmpty(txtMacAddress.Text))
            {
                psbQuery.AppendLine("and MacAddress like '%" + txtMacAddress.Text + "%' ");
                Session["txtMacAddressValue"] = txtMacAddress.Text;
            }

            string opcEstatus = drpEstatus.SelectedItem.Value;
            if (opcEstatus != "0")
            {
                psbQuery.AppendLine("and Estatus = " + drpEstatus.SelectedItem.Value + " ");
                Session["EstFiltro"] = opcEstatus;
            }

            string opcMarca = drpMarca.SelectedItem.Value;
            if (opcMarca != "0" && !String.IsNullOrEmpty(opcMarca))
            {
                psbQuery.AppendLine("and MarcaDisp = " + drpMarca.SelectedItem.Value + " ");
                Session["drpMarcaValue"] = opcMarca;
            }

            string opcModelo = drpModelo.SelectedItem.Value;
            if (opcModelo != "0" && !String.IsNullOrEmpty(opcModelo))
            {
                psbQuery.AppendLine("and ModeloDisp = " + drpModelo.SelectedItem.Value + " ");
                Session["drpModeloValue"] = opcModelo;
            }

            string opcTipoDispositivo = drpTipoAparato.SelectedItem.Value;
            if (opcTipoDispositivo != "0")
            {
                psbQuery.AppendLine("and TipoDispositivo = " + drpTipoAparato.SelectedItem.Value + " ");
                Session["drpTipoAparatoValue"] = opcTipoDispositivo;
            }

            EjecutarQueryBusqueda(psbQuery.ToString());
            Session["QueryBusqInv"] = psbQuery.ToString();
            refillControlesFiltros();
        
        }

        protected void lbtnLimpiarBusqueda_Click(object sender, EventArgs e)
        {
            removerSessionVar();
            Page.Response.Redirect(Page.Request.Url.ToString(), true);
        }

        protected void removerSessionVar()
        {
            Session.Remove("QueryBusqInv");
            Session.Remove("txtNoSerieValue");
            Session.Remove("txtMacAddressValue");
            Session.Remove("EstFiltro");
            Session.Remove("drpMarcaValue");
            Session.Remove("drpModeloValue");
            Session.Remove("drpTipoAparatoValue");
        }

        protected void lbtnAgregarInventario_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Redirect("~/UserInterface/CCustodiaDTI/AdminInventario.aspx?st=ga%2FDSd8L%2BcQ=");
        }

        protected void FormarQueryBusqueda()
        {
            psbQuery.Length = 0;

            psbQuery.AppendLine("--Query busqueda de inventario ");
            psbQuery.AppendLine("SELECT Estatus = EstatusDesc, Marca = MarcaDispDesc, Modelo = ModeloDispDesc, TipoDispositivo = TipoDispositivoDesc, ");
            psbQuery.AppendLine("NSerie, MacAddress, iCodCatDisp = iCodCatalogo ");
            psbQuery.AppendLine("FROM [VisHistoricos('Dispositivo','Inventario de dispositivos','Español')] ");
            psbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia ");
            psbQuery.AppendLine("and dtFinVigencia >= GETDATE() ");
            
        }

        protected void EjecutarQueryBusqueda(string query)
        {
            dtResultadoBusqueda = DSODataAccess.Execute(query);

            //preguntar si el estatus es el de asignado para mostrar la segunda grid
            if (Session["EstFiltro"].ToString() == ConsultaEstatusAsignado()) 
            {
                grvResultBusqInventario2.DataSource = dtResultadoBusqueda;
                grvResultBusqInventario2.DataBind();

                pnlResultBusqInventarioGrid2.Visible = true;
                pnlResultBusqInventarioGrid.Visible = false;
            }
            else
            {
                grvResultBusqInventario.DataSource = dtResultadoBusqueda;
                grvResultBusqInventario.DataBind();

                pnlResultBusqInventarioGrid.Visible = true;
                pnlResultBusqInventarioGrid2.Visible = false;
            }
            

            lblEquiposEncontrados.Text = "Equipos encontrados: " + dtResultadoBusqueda.Rows.Count.ToString();            
        }

        protected string ConsultaEstatusAsignado()
        {
            StringBuilder lsbConsultaEstatus = new StringBuilder();
            lsbConsultaEstatus.AppendLine("select iCodCatalogo ");
            lsbConsultaEstatus.AppendLine("FROM [VisHistoricos('Estatus','Estatus dispositivo','Español')] ");
            lsbConsultaEstatus.AppendLine("WHERE dtIniVigencia <> dtFinVigencia ");
            lsbConsultaEstatus.AppendLine("and dtFinVigencia >= GETDATE()");
            lsbConsultaEstatus.AppendLine("and value = 2");

            string lsEstatusAsignado = Util.IsDBNull(DSODataAccess.ExecuteScalar(lsbConsultaEstatus.ToString()), "").ToString();

            return lsEstatusAsignado;
        }

        /*Este metodo lo que hae es antes de llenar los rows en la grid evalua si se trata del estatus 
         disponible o asignado para despues saber si va a mostrar o no el hyperlink, pero como es campo
         que se genera con la grid es decir es un HyperLinkField elemento de Columns falto identificar 
         ese campo en el row y ponerlo como visible o no. Metodo responde al evento OnRowDataBound*/
        /*
        protected void grvResultBusqInventario_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                DataRowView rowView = (DataRowView)e.Row.DataItem;

                // Retrieve the status value for the current row. 
                string status = rowView["Estatus"].ToString();
                //Now you have the status 
                //get a reference to view hyperlink and hide it if that's the case
                //Hyperlink hlnkView = e.Row.FindControl("hlnkView") as HyperLink;
                //example: 
                //if (int.Parse(status) > 15)
                //    status = "false";
                    //hlnkView.Visible = false;//you are done

            }
            
        }*/
    }
}

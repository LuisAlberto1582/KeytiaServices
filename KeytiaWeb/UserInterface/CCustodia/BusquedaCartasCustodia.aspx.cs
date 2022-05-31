using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using KeytiaServiceBL;

namespace KeytiaWeb.UserInterface.CCustodia
{
    public partial class BuquedaCartasCustodia : System.Web.UI.Page
    {
        protected DataTable dtSitios;
        protected DataTable dtEstatusCCustodia;
        protected DataTable dtResultadoBusqueda;
        protected StringBuilder psbQuery = new StringBuilder();
        //RZ.20131203 Campo para almacenar el estatus de la busqueda de la carta custodia.
        private string estadoCCust;

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            string lsStyleSheet = (string)HttpContext.Current.Session["StyleSheet"];

            //RZ.20131203 Leer el estatus de la pagina de busqueda 
            /*Estados para la busqueda 1:Busqueda 2:Resultado
             */
            if (string.IsNullOrEmpty(Request.QueryString["stCC"]))
            {
                estadoCCust = "1";
            }
            else
            {
                estadoCCust = Request.QueryString["stCC"];
            }

            /*Se agrega css desde servidor para leerlo desde la carpeta de estilos elegida por la configuracion en K5*/
            Page.Header.Controls.Add(new LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + ResolveUrl(lsStyleSheet + "/CCustodia.css") + "\" />"));

            if (!Page.IsPostBack)
            {
                FillDropDowns();

                //RZ.20131203 Solo ejecutar busqueda cuando se tenga el estado 2 
                if (Session["QueryBusqCCustodia"] != null && estadoCCust == "2")
                {
                    EjecutarQueryBusqueda(Session["QueryBusqCCustodia"].ToString());

                }

                string iCodCatEB = Request.QueryString["ne"];

                if (!String.IsNullOrEmpty(iCodCatEB))
                {
                    lMensajeBajaEmple.Text = "<p style=\"color: red\">El empleado <strong>" + getNomCompletoEmpleBaja(iCodCatEB) + "</strong> ha sido dado baja correctamente.</p><br />";
                }
                else
                {
                    lMensajeBajaEmple.Visible = false;
                }



            }
        }

        /*RZ.20130807 Metodo para obtener el nombre completo del Empleado*/
        protected string getNomCompletoEmpleBaja(string iCodCatEmple)
        {
            StringBuilder lsbConsulta = new StringBuilder();

            lsbConsulta.Append("SELECT TOP 1 NomCompleto \r");
            lsbConsulta.Append("FROM [VisHistoricos('Emple','Empleados','Español')] \r");
            lsbConsulta.Append("WHERE iCodCatalogo = " + iCodCatEmple + " \r");
            lsbConsulta.Append("ORDER BY iCodRegistro DESC");

            return DSODataAccess.ExecuteScalar(lsbConsulta.ToString()).ToString();
        }

        protected void FillDropDowns()
        {
            FillDDLSitios();

            FillDDLEstatusCC();
        }

        protected void FillDDLSitios()
        {
            psbQuery.Length = 0;

            psbQuery.Append("SELECT iCodCatalogo, vchDescripcion \r");
            psbQuery.Append("FROM Historicos \r");
            psbQuery.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
            psbQuery.Append("and dtFinVigencia >= GETDATE() \r");
            psbQuery.Append("and iCodMaestro in (select iCodRegistro \r");
            psbQuery.Append("\t from Maestros \r");
            psbQuery.Append("\t where iCodEntidad = 23 --Sitios \r");
            psbQuery.Append("\t )");

            dtSitios = DSODataAccess.Execute(psbQuery.ToString());

            ddlSitioEmpleado.DataSource = dtSitios;
            ddlSitioEmpleado.DataValueField = "iCodCatalogo";
            ddlSitioEmpleado.DataTextField = "vchDescripcion";
            ddlSitioEmpleado.DataBind();

            ddlSitioEmpleado.Items.Insert(0, new ListItem("TODOS", "0"));

        }

        protected void FillDDLEstatusCC()
        {
            psbQuery.Length = 0;

            psbQuery.Append("SELECT iCodCatalogo, vchDescripcion = Español \r");
            psbQuery.Append("FROM [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')] \r");
            psbQuery.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
            psbQuery.Append("and dtFinVigencia >= GETDATE()");

            dtEstatusCCustodia = DSODataAccess.Execute(psbQuery.ToString());

            ddlEstatusCCustodia.DataSource = dtEstatusCCustodia;
            ddlEstatusCCustodia.DataValueField = "iCodCatalogo";
            ddlEstatusCCustodia.DataTextField = "vchDescripcion";
            ddlEstatusCCustodia.DataBind();

            ddlEstatusCCustodia.Items.Insert(0, new ListItem("TODOS", "0"));
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            limpiarBusquedaCCustodia();
        }

        //RZ.20131203 Se agrega boton para resetear la busqueda de las cartas, misma funcion que boton "Regresar"
        protected void lbtnLimpiarBusquedaCCustodia_Click(object sender, EventArgs e)
        {
            limpiarBusquedaCCustodia();
        }

        //RZ.20131203 Se encapsula funcionalidad que resetea la busqueda
        protected void limpiarBusquedaCCustodia()
        {
            Session.Remove("QueryBusqCCustodia");
            string url;

            if (Page.Request.Url.ToString().Contains("ne") || Page.Request.Url.ToString().Contains("stCC"))
            {
                url = Request.Url.GetLeftPart(UriPartial.Path);
            }
            else
            {
                url = Page.Request.Url.ToString();
            }


            Page.Response.Redirect(url, true);
        }

        protected void btnBuscarSimpleCCustodia_Click(object sender, EventArgs e)
        {
            //HttpContext.Current.Response.Redirect("~/UserInterface/CCustodia/AppCCustodia.aspx?Opc=OpcAppCCus&iCodEmple=");

            if (!String.IsNullOrEmpty(txtFiltroBusquedaCCustodia.Text))
            {
                FormarQueryBusqueda();

                string opcFiltro = ddlFiltroCCustodia.SelectedItem.Value;
                switch (opcFiltro)
                {
                    case "0":
                        //NomEmple
                        psbQuery.Append(" WHERE Emple.NomCompleto like '%" + txtFiltroBusquedaCCustodia.Text.Trim().Replace(" ", "%") + "%'");
                        psbQuery.Append(" and CCustodia.EstCCustodia <> 205320 /*CANCELADO (SOLO REGRESARÁ LAS CARTAS QUE ESTÉN ACTIVAS)*/ ");
                        break;
                    case "1":
                        //FolioCCustodia
                        psbQuery.Append(" WHERE CCustodia.FolioCCustodia = '" + txtFiltroBusquedaCCustodia.Text.Trim() + "'");
                        psbQuery.Append(" and CCustodia.EstCCustodia <> 205320 /*CANCELADO (SOLO REGRESARÁ LAS CARTAS QUE ESTÉN ACTIVAS)*/ ");
                        break;
                    case "2":
                        //Nomina
                        psbQuery.Append(" WHERE Emple.NominaA = '" + txtFiltroBusquedaCCustodia.Text.Trim() + "'");
                        psbQuery.Append(" and CCustodia.EstCCustodia <> 205320 /*CANCELADO (SOLO REGRESARÁ LAS CARTAS QUE ESTÉN ACTIVAS)*/ ");
                        break;
                    case "3":
                        //Exten
                        psbQuery.Append(" WHERE Emple.iCodCatalogo in ( select Emple \r");
                        psbQuery.Append("\t FROM [VisRelaciones('Empleado - Extension','Español')] \r");
                        psbQuery.Append("\t WHERE dtIniVigencia <> dtFinVigencia \r");
                        psbQuery.Append("\t and dtFinVigencia >= GETDATE() \r");
                        psbQuery.Append("\t and ExtenCod = '" + txtFiltroBusquedaCCustodia.Text.Trim() + "')");
                        psbQuery.Append(" and CCustodia.EstCCustodia <> 205320 /*CANCELADO (SOLO REGRESARÁ LAS CARTAS QUE ESTÉN ACTIVAS)*/ ");
                        break;
                }


                EjecutarQueryBusqueda(psbQuery.ToString());
                Session["QueryBusqCCustodia"] = psbQuery.ToString();

            }

        }

        protected void btnBuscarDetalladaCCustodia_Click(object sender, EventArgs e)
        {
            FormarQueryBusqueda();

            psbQuery.Append(" WHERE 1 = 1 \r");

            if (!String.IsNullOrEmpty(txtFolioCCustodia.Text))
            {
                psbQuery.Append(" and CCustodia.FolioCCustodia = '" + txtFolioCCustodia.Text + "' \r");
                psbQuery.Append(" and CCustodia.EstCCustodia <> 205320 /*CANCELADO (SOLO REGRESARÁ LAS CARTAS QUE ESTÉN ACTIVAS)*/ ");
            }

            if (!String.IsNullOrEmpty(txtNombreEmpleado.Text))
            {
                psbQuery.Append(" and Emple.NomCompleto like '%" + txtNombreEmpleado.Text + "%' \r");
                psbQuery.Append(" and CCustodia.EstCCustodia <> 205320 /*CANCELADO (SOLO REGRESARÁ LAS CARTAS QUE ESTÉN ACTIVAS)*/ ");
            }

            if (!String.IsNullOrEmpty(txtNoNomina.Text))
            {
                psbQuery.Append(" and Emple.NominaA = '" + txtNoNomina.Text + "' \r");
                psbQuery.Append(" and CCustodia.EstCCustodia <> 205320 /*CANCELADO (SOLO REGRESARÁ LAS CARTAS QUE ESTÉN ACTIVAS)*/ ");
            }

            string opcEstatusCCustodia = ddlEstatusCCustodia.SelectedItem.Value;
            if (opcEstatusCCustodia != "0")
            {
                psbQuery.Append("and CCustodia.EstCCustodia = " + ddlEstatusCCustodia.SelectedItem.Value + " \r");
            }

            if (!String.IsNullOrEmpty(txtExtension.Text))
            {
                psbQuery.Append("and Emple.iCodCatalogo in ( SELECT Emple \r");
                psbQuery.Append("\t FROM [VisRelaciones('Empleado - Extension','Español')] \r");
                psbQuery.Append("\t WHERE dtIniVigencia <> dtFinVigencia \r");
                psbQuery.Append("\t and dtFinVigencia >= GETDATE() \r");
                psbQuery.Append("\t and ExtenCod = '" + txtExtension.Text + "') \r");
                psbQuery.Append(" and CCustodia.EstCCustodia <> 205320 /*CANCELADO (SOLO REGRESARÁ LAS CARTAS QUE ESTÉN ACTIVAS)*/ ");
            }

            if (!String.IsNullOrEmpty(txtCodAuto.Text))
            {
                psbQuery.Append("and Emple.iCodCatalogo in ( SELECT Emple \r");
                psbQuery.Append("\t FROM [VisRelaciones('Empleado - CodAutorizacion','Español')] \r");
                psbQuery.Append("\t WHERE dtIniVigencia <> dtFinVigencia \r");
                psbQuery.Append("\t and dtFinVigencia >= GETDATE() \r");
                psbQuery.Append("\t and CodAutoCod = '" + txtCodAuto.Text + "') \r");
                psbQuery.Append(" and CCustodia.EstCCustodia <> 205320 /*CANCELADO (SOLO REGRESARÁ LAS CARTAS QUE ESTÉN ACTIVAS)*/ ");
            }

            if (!String.IsNullOrEmpty(txtNoSerie.Text))
            {
                psbQuery.Append("and Emple.iCodCatalogo in ( SELECT Emple \r");
                psbQuery.Append("\t FROM [VisRelaciones('Dispositivo - Empleado','Español')] Rel \r");
                psbQuery.Append("\t INNER JOIN [VisHistoricos('Dispositivo','Inventario de dispositivos','Español')] Hist \r");
                psbQuery.Append("\t\t ON Rel.Dispositivo = Hist.iCodCatalogo \r");
                psbQuery.Append("\t\t and Hist.dtIniVigencia <> Hist.dtFinVigencia \r");
                psbQuery.Append("\t\t and Hist.dtFinVigencia >= GETDATE() \r");
                psbQuery.Append("\t\t and Rel.dtIniVigencia <> Rel.dtFinVigencia \r");
                psbQuery.Append("\t\t and Rel.dtFinVigencia >= GETDATE() \r");
                psbQuery.Append("\t\t where Hist.NSerie like '%" + txtNoSerie.Text + "%') \r");
                psbQuery.Append(" and CCustodia.EstCCustodia <> 205320 /*CANCELADO (SOLO REGRESARÁ LAS CARTAS QUE ESTÉN ACTIVAS)*/ ");
            }

            if (!String.IsNullOrEmpty(txtMacAddress.Text))
            {
                psbQuery.Append("and Emple.iCodCatalogo in ( SELECT Emple \r");
                psbQuery.Append("\t FROM [VisRelaciones('Dispositivo - Empleado','Español')] Rel \r");
                psbQuery.Append("\t INNER JOIN [VisHistoricos('Dispositivo','Inventario de dispositivos','Español')] Hist \r");
                psbQuery.Append("\t\t ON Rel.Dispositivo = Hist.iCodCatalogo \r");
                psbQuery.Append("\t\t and Hist.dtIniVigencia <> Hist.dtFinVigencia \r");
                psbQuery.Append("\t\t and Hist.dtFinVigencia >= GETDATE() \r");
                psbQuery.Append("\t\t and Rel.dtIniVigencia <> Rel.dtFinVigencia \r");
                psbQuery.Append("\t\t and Rel.dtFinVigencia >= GETDATE() \r");
                psbQuery.Append("\t\t where Hist.macAddress like '%" + txtMacAddress.Text + "%') \r");
                psbQuery.Append(" and CCustodia.EstCCustodia <> 205320 /*CANCELADO (SOLO REGRESARÁ LAS CARTAS QUE ESTÉN ACTIVAS)*/ ");

            }

            string opcSitioEmple = ddlSitioEmpleado.SelectedValue;
            if (opcSitioEmple != "0")
            {
                psbQuery.Append("and Emple.Ubica like '" + ddlSitioEmpleado.SelectedItem.Text + "' \r");
                psbQuery.Append(" and CCustodia.EstCCustodia <> 205320 /*CANCELADO (SOLO REGRESARÁ LAS CARTAS QUE ESTÉN ACTIVAS)*/ ");
            }

            EjecutarQueryBusqueda(psbQuery.ToString());
            Session["QueryBusqCCustodia"] = psbQuery.ToString();
        }

        protected void FormarQueryBusqueda()
        {
            psbQuery.Length = 0;

            psbQuery.Append("select Emple.iCodCatalogo as iCodCatEmple, CCustodia.FolioCCustodia as Folio, CCustodia.EstCCustodiaDesc as Estatus, \r");
            psbQuery.Append("Emple.NominaA as Nomina, Emple.NomCompleto as NombreEmple, Emple.Ubica as Sitio, CCustodia.FechaCreacion as Fecha, \r");
            psbQuery.Append("InventarioAsignado = isnull(Equipo.Cant,0), Exten = isnull(Exten.Cant,0), Cod = isnull(CodAuto.Cant,0) \r");
            psbQuery.Append("from [VisHistoricos('CCustodia','Cartas custodia','Español')] CCustodia \r");
            psbQuery.Append("INNER JOIN [VisHistoricos('Emple','Empleados','Español')] Emple \r");
            psbQuery.Append("\t on Emple.iCodCatalogo = CCustodia.Emple" + "\r");
            psbQuery.Append("\t and CCustodia.dtIniVigencia <> CCustodia.dtFinVigencia" + "\r");
            psbQuery.Append("\t and CCustodia.dtFinVigencia >= GETDATE()" + "\r");
            psbQuery.Append("\t and Emple.dtIniVigencia <> Emple.dtFinVigencia" + "\r");
            psbQuery.Append("\t and Emple.dtFinVigencia >= GETDATE()" + "\r");
            psbQuery.Append("LEFT OUTER JOIN (select Emple, Cant = isnull(Count(*),0) \r");
            psbQuery.Append("\t\t from [VisRelaciones('Empleado - Extension','Español')]" + "\r");
            psbQuery.Append("\t\t where dtIniVigencia <> dtFinVigencia" + "\r");
            psbQuery.Append("\t\t and dtFinVigencia >= GETDATE()" + "\r");
            psbQuery.Append("\t\t group by Emple) as Exten" + "\r");
            psbQuery.Append("\t ON Emple.iCodCatalogo = Exten.Emple" + "\r");
            psbQuery.Append("LEFT OUTER JOIN (select Emple, Cant = isnull(Count(*),0) \r");
            psbQuery.Append("\t\t from [VisRelaciones('Empleado - CodAutorizacion','Español')]" + "\r");
            psbQuery.Append("\t\t where dtIniVigencia <> dtFinVigencia" + "\r");
            psbQuery.Append("\t\t and dtFinVigencia >= GETDATE()" + "\r");
            psbQuery.Append("\t\t group by Emple) as CodAuto" + "\r");
            psbQuery.Append("\t ON Emple.iCodCatalogo = CodAuto.Emple" + "\r");
            psbQuery.Append("LEFT OUTER JOIN (select Emple, Cant = isnull(Count(*),0) \r");
            psbQuery.Append("\t\t from [VisRelaciones('Dispositivo - Empleado','Español')]" + "\r");
            psbQuery.Append("\t\t where dtIniVigencia <> dtFinVigencia" + "\r");
            psbQuery.Append("\t\t and dtFinVigencia >= GETDATE()" + "\r");
            psbQuery.Append("\t\t group by Emple) as Equipo" + "\r");
            psbQuery.Append("\t ON Emple.iCodCatalogo = Equipo.Emple" + "\r");
        }

        protected void EjecutarQueryBusqueda(string query)
        {
            dtResultadoBusqueda = DSODataAccess.Execute(query);
            grvResultadoBusqueda.DataSource = dtResultadoBusqueda;
            grvResultadoBusqueda.DataBind();

            lblCartasEncontradas.Text = "Cartas encontradas: " + dtResultadoBusqueda.Rows.Count.ToString();

            tblHeadResultBusq.Visible = true;

            tbcBuscaCCustodia.Visible = false;
            pnlResultadoBusquedaGrid.Visible = true;
        }
    }
}

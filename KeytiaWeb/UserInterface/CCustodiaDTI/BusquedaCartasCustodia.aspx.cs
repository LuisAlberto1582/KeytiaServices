using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using KeytiaServiceBL;

namespace KeytiaWeb.UserInterface.CCustodiaDTI
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

            // Modificacion TID para FCA
            if (DSODataContext.Schema.ToUpper() == "FCA" && !this.IsPostBack)
            {
                ddlFiltroCCustodia.Items.Add(new ListItem("TID", "5"));
            }
            // Modificacion TID para FCA

            //RZ.20131203 Leer el estatus de la pagina de busqueda 
            /*Estados para la busqueda 1:Busqueda 2:Resultado
             */
            if (string.IsNullOrEmpty(Request.QueryString["stCC"]))
            {
                estadoCCust = "1";
            }
            else { estadoCCust = Request.QueryString["stCC"]; }
        
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
                else { lMensajeBajaEmple.Visible = false; }
            }
        }

        /*RZ.20130807 Metodo para obtener el nombre completo del Empleado*/
        protected string getNomCompletoEmpleBaja(string iCodCatEmple)
        {
            StringBuilder lsbConsulta = new StringBuilder();

            lsbConsulta.AppendLine("SELECT TOP 1 NomCompleto");
            lsbConsulta.AppendLine("FROM [VisHistoricos('Emple','Empleados','Español')]");
            lsbConsulta.AppendLine("WHERE iCodCatalogo = " + iCodCatEmple);
            lsbConsulta.AppendLine("ORDER BY iCodRegistro DESC");

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

            psbQuery.AppendLine("SELECT iCodCatalogo, vchDescripcion");
            psbQuery.AppendLine("FROM Historicos");
            psbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            psbQuery.AppendLine("and dtFinVigencia >= GETDATE()");
            psbQuery.AppendLine("and iCodMaestro in (select iCodRegistro");
            psbQuery.AppendLine("   from Maestros");
            psbQuery.AppendLine("   where iCodEntidad = 23)");

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

            psbQuery.AppendLine("SELECT iCodCatalogo, vchDescripcion = Español");
            psbQuery.AppendLine("FROM [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')]");
            psbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            psbQuery.AppendLine("and dtFinVigencia >= GETDATE()");

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
            else { url = Page.Request.Url.ToString(); }

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
                        psbQuery.AppendLine(" WHERE Emple.NomCompleto like '%" + txtFiltroBusquedaCCustodia.Text.Trim().Replace(" ", "%") + "%'");
                        psbQuery.AppendLine(" and Emple.dtIniVigencia<> Emple.dtFinVigencia");
                        psbQuery.AppendLine(" and Emple.dtFinVigencia >= GETDATE()");
                        break;
                    case "1":
                        //FolioCCustodia
                        psbQuery.AppendLine(" WHERE CCustodia.FolioCCustodia = '" + txtFiltroBusquedaCCustodia.Text.Trim() + "'");
                        psbQuery.AppendLine(" and Emple.dtIniVigencia<> Emple.dtFinVigencia");
                        psbQuery.AppendLine(" and Emple.dtFinVigencia >= GETDATE()");
                        break;
                    case "2":
                        //Nomina
                        psbQuery.AppendLine(" WHERE Emple.NominaA = '" + txtFiltroBusquedaCCustodia.Text.Trim() + "'");
                        psbQuery.AppendLine(" and Emple.dtIniVigencia<> Emple.dtFinVigencia");
                        psbQuery.AppendLine(" and Emple.dtFinVigencia >= GETDATE()");
                        break;
                    case "3":
                        //Exten
                        psbQuery.AppendLine(" WHERE Emple.iCodCatalogo in ( select Emple");
                        psbQuery.AppendLine("    FROM [VisRelaciones('Empleado - Extension','Español')]");
                        psbQuery.AppendLine("    WHERE dtIniVigencia <> dtFinVigencia");
                        psbQuery.AppendLine("    and dtFinVigencia >= GETDATE()");
                        psbQuery.AppendLine("    and ExtenCod = '" + txtFiltroBusquedaCCustodia.Text.Trim() + "')");
                        psbQuery.AppendLine(" and Emple.dtIniVigencia<> Emple.dtFinVigencia");
                        psbQuery.AppendLine(" and Emple.dtFinVigencia >= GETDATE()");
                        break;
                    case "4":
                        //Linea
                        psbQuery.AppendLine(" WHERE Emple.iCodCatalogo in ( select Emple");
                        psbQuery.AppendLine("    FROM [VisRelaciones('Empleado - Linea','Español')]");
                        psbQuery.AppendLine("    WHERE dtIniVigencia <> dtFinVigencia");
                        psbQuery.AppendLine("    and dtFinVigencia >= GETDATE()");
                        psbQuery.AppendLine("    and LineaCod = '" + txtFiltroBusquedaCCustodia.Text.Trim() + "')");
                        psbQuery.AppendLine(" and Emple.dtIniVigencia<> Emple.dtFinVigencia");
                        psbQuery.AppendLine(" and Emple.dtFinVigencia >= GETDATE()");
                        break;
                    case "5":
                        // Busqueda TCA TID
                        psbQuery.AppendLine(" WHERE Emple.iCodCatalogo in ( select Emple");
                        psbQuery.AppendLine("    FROM [visHistoricos('empleFCA','empleados fca','español')] ");
                        psbQuery.AppendLine("    WHERE dtIniVigencia <> dtFinVigencia");
                        psbQuery.AppendLine("    and dtFinVigencia >= GETDATE()");
                        psbQuery.AppendLine("    and FCA_T_ID = '" + txtFiltroBusquedaCCustodia.Text.Trim() + "')");
                        psbQuery.AppendLine(" and Emple.dtIniVigencia<> Emple.dtFinVigencia");
                        psbQuery.AppendLine(" and Emple.dtFinVigencia >= GETDATE()");
                        break;
                }

                /*CANCELADO (SOLO REGRESARÁ LAS CARTAS QUE ESTÉN ACTIVAS)*/
                if(DSODataContext.Schema.ToString().ToUpper() != "BAT")
                {
                    psbQuery.AppendLine(" and CCustodia.EstCCustodia NOT IN(SELECT iCodCatalogo ");
                    psbQuery.AppendLine("                                   FROM [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')] ");
                    psbQuery.AppendLine("                                   WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
                    psbQuery.AppendLine("                                       AND vchCodigo = 'EstCCustodiaCancelada')");

                }

                EjecutarQueryBusqueda(psbQuery.ToString());
                Session["QueryBusqCCustodia"] = psbQuery.ToString();

            }
        }

        protected void btnBuscarDetalladaCCustodia_Click(object sender, EventArgs e)
        {
            FormarQueryBusqueda();

            psbQuery.AppendLine(" WHERE 1 = 1");

            string opcEstatusCCustodia = ddlEstatusCCustodia.SelectedItem.Value;
            if (opcEstatusCCustodia != "0")
            {
                psbQuery.AppendLine(" and CCustodia.EstCCustodia = " + ddlEstatusCCustodia.SelectedItem.Value + " ");
            }
            else
            {
                psbQuery.AppendLine(" and CCustodia.EstCCustodia NOT IN(SELECT iCodCatalogo ");/*CANCELADO (SOLO REGRESARÁ LAS CARTAS QUE ESTÉN ACTIVAS)*/
                psbQuery.AppendLine("                                   FROM [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')] ");
                psbQuery.AppendLine("                                   WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
                psbQuery.AppendLine("                                       AND vchCodigo = 'EstCCustodiaCancelada') /*CANCELADO (SOLO REGRESARÁ LAS CARTAS QUE ESTÉN ACTIVAS)*/");
            }

            if (!String.IsNullOrEmpty(txtFolioCCustodia.Text))
            {
                psbQuery.AppendLine(" and CCustodia.FolioCCustodia = '" + txtFolioCCustodia.Text + "'");
            }

            if (!String.IsNullOrEmpty(txtNombreEmpleado.Text))
            {
                psbQuery.AppendLine(" and Emple.NomCompleto like '%" + txtNombreEmpleado.Text + "%' ");
            }

            if (!String.IsNullOrEmpty(txtNoNomina.Text))
            {
                psbQuery.AppendLine(" and Emple.NominaA = '" + txtNoNomina.Text + "' ");
            }

            if (!String.IsNullOrEmpty(txtExtension.Text))
            {
                psbQuery.AppendLine(" and Emple.iCodCatalogo in ( SELECT Emple ");
                psbQuery.AppendLine("                             FROM [VisRelaciones('Empleado - Extension','Español')]");
                psbQuery.AppendLine("                             WHERE dtIniVigencia <> dtFinVigencia");
                psbQuery.AppendLine("                               and dtFinVigencia >= GETDATE()");
                psbQuery.AppendLine("                               and ExtenCod = '" + txtExtension.Text + "')");
            }

            if (!String.IsNullOrEmpty(txtCodAuto.Text))
            {
                psbQuery.AppendLine(" and Emple.iCodCatalogo in ( SELECT Emple");
                psbQuery.AppendLine("                             FROM [VisRelaciones('Empleado - CodAutorizacion','Español')]");
                psbQuery.AppendLine("                             WHERE dtIniVigencia <> dtFinVigencia");
                psbQuery.AppendLine("                               and dtFinVigencia >= GETDATE()");
                psbQuery.AppendLine("                               and CodAutoCod = '" + txtCodAuto.Text + "')");
            }

            if (!String.IsNullOrEmpty(txtNoSerie.Text))
            {
                psbQuery.AppendLine(" and Emple.iCodCatalogo in ( SELECT Emple");
                psbQuery.AppendLine("                             FROM [VisRelaciones('Dispositivo - Empleado','Español')] Rel");
                psbQuery.AppendLine("                             INNER JOIN [VisHistoricos('Dispositivo','Inventario de dispositivos','Español')] Hist ");
                psbQuery.AppendLine("                               ON Rel.Dispositivo = Hist.iCodCatalogo ");
                psbQuery.AppendLine("                               and Hist.dtIniVigencia <> Hist.dtFinVigencia ");
                psbQuery.AppendLine("                               and Hist.dtFinVigencia >= GETDATE() ");
                psbQuery.AppendLine("                               and Rel.dtIniVigencia <> Rel.dtFinVigencia ");
                psbQuery.AppendLine("                               and Rel.dtFinVigencia >= GETDATE() ");
                psbQuery.AppendLine("                             WHERE Hist.NSerie like '%" + txtNoSerie.Text + "%') ");
            }

            if (!String.IsNullOrEmpty(txtMacAddress.Text))
            {
                psbQuery.AppendLine(" and Emple.iCodCatalogo in ( SELECT Emple ");
                psbQuery.AppendLine("                             FROM [VisRelaciones('Dispositivo - Empleado','Español')] Rel ");
                psbQuery.AppendLine("                             INNER JOIN [VisHistoricos('Dispositivo','Inventario de dispositivos','Español')] Hist ");
                psbQuery.AppendLine("                             ON Rel.Dispositivo = Hist.iCodCatalogo ");
                psbQuery.AppendLine("                               and Hist.dtIniVigencia <> Hist.dtFinVigencia ");
                psbQuery.AppendLine("                               and Hist.dtFinVigencia >= GETDATE() ");
                psbQuery.AppendLine("                               and Rel.dtIniVigencia <> Rel.dtFinVigencia ");
                psbQuery.AppendLine("                               and Rel.dtFinVigencia >= GETDATE() ");
                psbQuery.AppendLine("                             WHERE Hist.macAddress like '%" + txtMacAddress.Text + "%') ");
            }
            if(!string.IsNullOrEmpty(txtLinea.Text))
            {
                psbQuery.AppendLine(" and Emple.iCodCatalogo in ( SELECT Emple ");
                psbQuery.AppendLine("                             FROM [VisRelaciones('Empleado - Linea','Español')]");
                psbQuery.AppendLine("                             WHERE dtIniVigencia <> dtFinVigencia");
                psbQuery.AppendLine("                               and dtFinVigencia >= GETDATE()");
                psbQuery.AppendLine("                               and LineaCod = '" + txtLinea.Text + "')");
            }
            string opcSitioEmple = ddlSitioEmpleado.SelectedValue;
            if (opcSitioEmple != "0")
            {
                psbQuery.AppendLine("and Emple.Ubica like '" + ddlSitioEmpleado.SelectedItem.Text + "' ");
            }

            EjecutarQueryBusqueda(psbQuery.ToString());
            Session["QueryBusqCCustodia"] = psbQuery.ToString();
        }

        protected void FormarQueryBusqueda()
        {
            psbQuery.Length = 0;
            psbQuery.AppendLine("select Emple.iCodCatalogo as iCodCatEmple, CCustodia.FolioCCustodia as Folio, CCustodia.EstCCustodiaDesc as Estatus, ");
            psbQuery.AppendLine("Emple.NominaA as Nomina, Emple.NomCompleto as NombreEmple, Emple.Ubica as Sitio, CCustodia.FechaCreacion as Fecha, ");
            psbQuery.AppendLine("InventarioAsignado = isnull(Equipo.Cant,0), Exten = isnull(Exten.Cant,0), Cod = isnull(CodAuto.Cant,0),Linea = ISNULL(Equipo.LineaCod,'') ");
            psbQuery.AppendLine(" from [VisHistoricos('Emple','Empleados','Español')] Emple");
            psbQuery.AppendLine(" LEFT JOIN [VisHistoricos('CCustodia','Cartas custodia','Español')] CCustodia");
            psbQuery.AppendLine(" on Emple.iCodCatalogo = CCustodia.Emple");
            psbQuery.AppendLine(" and CCustodia.dtIniVigencia <> CCustodia.dtFinVigencia");
            psbQuery.AppendLine(" and CCustodia.dtFinVigencia >= GETDATE()");
            psbQuery.AppendLine("LEFT OUTER JOIN (select Emple, Cant = isnull(Count(*),0) ");
            psbQuery.AppendLine("       from [VisRelaciones('Empleado - Extension','Español')]");
            psbQuery.AppendLine("       where dtIniVigencia <> dtFinVigencia");
            psbQuery.AppendLine("       and dtFinVigencia >= GETDATE()");
            psbQuery.AppendLine("       group by Emple) as Exten" + "");
            psbQuery.AppendLine("   ON Emple.iCodCatalogo = Exten.Emple");
            psbQuery.AppendLine("LEFT OUTER JOIN (select Emple, Cant = isnull(Count(*),0) ");
            psbQuery.AppendLine("       from [VisRelaciones('Empleado - CodAutorizacion','Español')]");
            psbQuery.AppendLine("       where dtIniVigencia <> dtFinVigencia");
            psbQuery.AppendLine("       and dtFinVigencia >= GETDATE()");
            psbQuery.AppendLine("       group by Emple) as CodAuto");
            psbQuery.AppendLine("   ON Emple.iCodCatalogo = CodAuto.Emple" + "");
            //psbQuery.AppendLine("LEFT OUTER JOIN (select Emple, Cant = isnull(Count(*),0) ");
            //psbQuery.AppendLine("       from [VisRelaciones('Dispositivo - Empleado','Español')]");
            //psbQuery.AppendLine("       where dtIniVigencia <> dtFinVigencia");
            //psbQuery.AppendLine("       and dtFinVigencia >= GETDATE()");
            //psbQuery.AppendLine("       group by Emple) as Equipo");
            psbQuery.AppendLine("LEFT OUTER JOIN (select Emple,LineaCod, Cant = isnull(Count(*),0) ");
            psbQuery.AppendLine("       from [VisRelaciones('Empleado - Linea','Español')]");
            psbQuery.AppendLine("       where dtIniVigencia <> dtFinVigencia");
            psbQuery.AppendLine("       and dtFinVigencia >= GETDATE()");
            psbQuery.AppendLine("       group by Emple,LineaCod) as Equipo");
            psbQuery.AppendLine("   ON Emple.iCodCatalogo = Equipo.Emple");
        }

        protected void EjecutarQueryBusqueda(string query)
        {
            dtResultadoBusqueda = DSODataAccess.Execute(query);
            grvResultadoBusqueda.DataSource = dtResultadoBusqueda;
            grvResultadoBusqueda.DataBind();

            lblCartasEncontradas.Text = "Cartas encontradas: " + dtResultadoBusqueda.Rows.Count.ToString();
            Rep1.Visible = true;
            Rep0.Visible = false;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using KeytiaServiceBL;

namespace KeytiaWeb.UserInterface
{
    public partial class CambioEtiqueta : System.Web.UI.Page
    {
        DataTable dtGrupoEtiqueta = new DataTable();
        List<EtiquetacionModelView> listaEtiq = new List<EtiquetacionModelView>();
        StringBuilder query = new StringBuilder();

        public void Page_PreInit(object o, EventArgs e)
        {
            EnsureChildControls();
            Page.ClientScript.GetPostBackEventReference(this, "");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
                ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
                #endregion

                if (!Page.IsPostBack)
                {
                    GetNumsEmplePorEtiquetar();
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        #region Logica Resumen

        private void GetNumsEmplePorEtiquetar()
        {
            var dtResult = DSODataAccess.Execute(ConsultaResumenEtiq());
            if (dtResult.Rows.Count > 0)
            {
                dtGrupoEtiqueta = DSODataAccess.Execute(GetGruposEtiqueta());  //Esta linea se tiene que ejecutar antes del enlace de datos con el GRID

                gridResumenNums.DataSource = dtResult;
                gridResumenNums.DataBind();

                var totales = GetTotalesResumenEtiq(dtResult);
                gridResumenNums.FooterRow.Controls[0].Controls.Add(new Label() { Text = "Total" });
                gridResumenNums.FooterRow.Controls[1].Controls.Add(new Label() { Text = string.Format("{0:#,#}", Convert.ToInt32(totales["Cantidad"])) });
                gridResumenNums.FooterRow.Controls[2].Controls.Add(new Label() { Text = string.Format("{0:#,#}", Convert.ToInt32(totales["Duracion"].ToString())) });
                gridResumenNums.FooterRow.Controls[3].Controls.Add(new Label() { Text = string.Format("$ {0:#,#.##}", Convert.ToDouble(totales["Costo"].ToString())) });
            }
            else
            {
                gridResumenNums.DataSource = dtResult;
                gridResumenNums.DataBind();                
            }
        }

        private string ConsultaResumenEtiq()
        {
            query.Length = 0;
            query.AppendLine("DECLARE @gpoEtiqNoIdent INT = (SELECT GEtiqueta FROM " + DSODataContext.Schema + ".[VisHistoricos('GpoEtiqueta','Grupo Etiquetacion','Español')]");
            query.AppendLine("								 WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()	AND vchCodigo = '0NoIdent' )");
            query.AppendLine("");
            query.AppendLine("DECLARE @emple INT = (SELECT MAX(iCodCatalogo) FROM " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')]");
            query.AppendLine("					    WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND Usuar = " + Session["iCodUsuario"].ToString() + ")");
            query.AppendLine("");
            query.AppendLine("DECLARE @llamsEntrada INT = (SELECT MAX(iCodCatalogo) FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]");
            query.AppendLine("							   WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = 'Ent')");
            query.AppendLine("");
            query.AppendLine("DECLARE @llamsEnlace INT = (SELECT MAX(iCodCatalogo) FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]");
            query.AppendLine("							  WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = 'Enl')");
            query.AppendLine("");
            query.AppendLine("SELECT ");
            query.AppendLine("       [Numero]				= TelDest");
            query.AppendLine("      ,[Cantidad]				= COUNT(*)");
            query.AppendLine("      ,[Duracion]				= SUM(DuracionMin)");
            query.AppendLine("      ,[Costo]				= '$ ' + CONVERT(VARCHAR, CONVERT(MONEY,ROUND(SUM(Costo + CostoSM),2)), 1)");
            query.AppendLine("      ,[Grupo]                = GEtiqueta");
            query.AppendLine("      ,[Etiqueta]             = Etiqueta");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisDetallados('Detall','DetalleCDR','Español')]");
            query.AppendLine("WHERE GEtiqueta <> @gpoEtiqNoIdent");
            query.AppendLine("	AND Emple = @emple");
            query.AppendLine("	AND TDest <> @llamsEntrada");
            query.AppendLine("	AND TDest <> @llamsEnlace");
            query.AppendLine("	AND ( LEN(TelDest) = 3 OR LEN(TelDest) >= 7 )");
            query.AppendLine("GROUP BY TelDest, GEtiqueta,Etiqueta");
            query.AppendLine("ORDER BY SUM(Costo + CostoSM) DESC");
            return query.ToString();
        }

        private DataRow GetTotalesResumenEtiq(DataTable dtResultResumen)
        {
            int cantidad = 0;
            int duracion = 0;
            double costo = 0;
            foreach (DataRow row in dtResultResumen.Rows)
            {
                cantidad += Convert.ToInt32(row["Cantidad"]);
                duracion += Convert.ToInt32(row["Duracion"]);
                costo += Convert.ToDouble(row["Costo"].ToString().Replace("$", "").Trim());
            }

            DataRow newRow = dtResultResumen.NewRow();
            newRow["Cantidad"] = cantidad;
            newRow["Duracion"] = duracion;
            newRow["Costo"] = costo.ToString();

            return newRow;
        }

        private string GetGruposEtiqueta()
        {
            query.Length = 0;
            query.AppendLine("SELECT vchDescripcion, GEtiqueta");
            query.AppendLine("FROM [VisHistoricos('GpoEtiqueta','Grupo Etiquetacion','Español')]");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            return query.ToString();
        }

        protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlGrupoEtiqueta = (e.Row.FindControl("ddlGrupoEtiqueta") as DropDownList);
                ddlGrupoEtiqueta.DataSource = dtGrupoEtiqueta;
                ddlGrupoEtiqueta.DataTextField = "vchDescripcion";
                ddlGrupoEtiqueta.DataValueField = "GEtiqueta";
                ddlGrupoEtiqueta.DataBind();

                string grupo = (e.Row.FindControl("lblGrupo") as Label).Text;
                ddlGrupoEtiqueta.Items.FindByValue(grupo).Selected = true;

                Image imagen = (e.Row.FindControl("imgEditar")) as Image;
                imagen.ImageUrl = "~/images/pencil.png";
            }
        }          

        private void ProcesarGridResumenNums()
        {
            listaEtiq.Clear();
            int gpoNoIdentificado = Convert.ToInt32(DSODataAccess.ExecuteScalar("SELECT GEtiqueta FROM [VisHistoricos('GpoEtiqueta','Grupo Etiquetacion','Español')] WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = '0NoIdent'"));
            string mensajeError = string.Empty;

            //NZ: Obtenemos todos los datos del GridView.
            foreach (GridViewRow row in gridResumenNums.Rows)
            {
                EtiquetacionModelView objEtiq = new EtiquetacionModelView();
                objEtiq.Numero = (row.FindControl("btnLinkNumero") as LinkButton).Text;
                objEtiq.Grupo = Convert.ToInt32((row.FindControl("ddlGrupoEtiqueta") as DropDownList).SelectedValue);
                objEtiq.Etiqueta = (row.FindControl("txtEtiqueta") as TextBox).Text.Replace("'", "").Trim();

                if (objEtiq.Grupo == gpoNoIdentificado && !string.IsNullOrEmpty(objEtiq.Etiqueta))
                {
                    mensajeError = "No se puede introducir una etiqueta para el Grupo 'No Identificada'.";
                    break;
                }
                else if (objEtiq.Grupo != gpoNoIdentificado && string.IsNullOrEmpty(objEtiq.Etiqueta))
                {
                    mensajeError = "No se puede dejar una etiqueta en blanco para un grupo diferente de 'No Identificada'.";
                    break;
                }
                else if (objEtiq.Grupo != gpoNoIdentificado && !string.IsNullOrEmpty(objEtiq.Etiqueta))
                {
                    listaEtiq.Add(objEtiq);
                }
            }

            if (!string.IsNullOrEmpty(mensajeError))
            {
                lblTituloModalMsn.Text = "Error en la etiquetación de números";
                lblBodyModalMsn.Text = mensajeError;
                mpeEtqMsn.Show();
            }
            else
            {
                EtiquetarNumeros(gpoNoIdentificado);                
            }
        }

        public void EtiquetarNumeros(int gpoNoIdentificado)
        {
            if (listaEtiq.Count > 0)
            {
                query.Length = 0;
                query.AppendLine("SELECT MAX(iCodCatalogo) FROM " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')]");
                query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND Usuar = " + Session["iCodUsuario"].ToString());
                var iCodEmple = DSODataAccess.ExecuteScalar(query.ToString()).ToString();

                string exec = "EXEC ProcesoEtiquetacionLight @esquema='{0}', @numMarcado='{1}', @etiqueta='{2}', @gpoEtiqueta={3}, @Emple={4}";

                //Solo se actualizan los registros en donde el grupo etiqueta sea diferente de 0 y la etiqueta sea diferente de vacio.
                foreach (EtiquetacionModelView item in listaEtiq.Where(x => x.Grupo != gpoNoIdentificado && !string.IsNullOrEmpty(x.Etiqueta)))
                {
                    DSODataAccess.ExecuteNonQuery(string.Format(exec, DSODataContext.Schema, item.Numero, item.Etiqueta, item.Grupo, iCodEmple));
                }
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            if (Session["iCodPerfil"].ToString() == "370")
            {
                HttpContext.Current.Response.Redirect(Session["HomePage"].ToString());
            }
            else
            {
                HttpContext.Current.Response.Redirect("~/UserInterface/DashboardFC/Dashboard.aspx?MiConsumo=1&Opc=opcMiConsumo");
            }
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Redirect("~/UserInterface/Historicos/Etiquetacion/EtiquetacionEmple.aspx");  
        }
    
        #endregion

        #region Logica Detalle

        private string ConsultaTiposGEtiquetas()
        {
            query.Length = 0;

            query.AppendLine("SELECT distinct(vchDescripcion),gEtiqueta ");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('GpoEtiqueta','Grupo Etiquetacion','Español')]");
            query.AppendLine("WHERE GEtiqueta <> 0");
            query.AppendLine("ORDER BY gEtiqueta");
            return query.ToString();
        }

        private string ConsultaConsultaCamposAReetiquetar(string telDest)
        {

            query.Length = 0;

            query.AppendLine("DECLARE @gpoEtiqNoIdent INT = (SELECT GEtiqueta FROM " + DSODataContext.Schema + ".[VisHistoricos('GpoEtiqueta','Grupo Etiquetacion','Español')]     ");
            query.AppendLine("								 WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()	AND vchCodigo = '0NoIdent' )               ");
            query.AppendLine("                                                                                                                                                 ");
            query.AppendLine("DECLARE @emple INT = (SELECT MAX(iCodCatalogo) FROM " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')]                 ");
            query.AppendLine("					    WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND Usuar = " + Session["iCodUsuario"].ToString() + ") ");
            query.AppendLine("                                                                                                                                                 ");
            query.AppendLine("DECLARE @llamsEntrada INT = (SELECT MAX(iCodCatalogo) FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]    ");
            query.AppendLine("							   WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = 'Ent')                          ");
            query.AppendLine("                                                                                                                                                 ");
            query.AppendLine("DECLARE @llamsEnlace INT = (SELECT MAX(iCodCatalogo) FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]     ");
            query.AppendLine("							  WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = 'Enl')                           ");
            query.AppendLine("                                                                                                                                                 ");
            query.AppendLine("SELECT                                                                                                                                           ");
            query.AppendLine("                                                                                                                                                 ");
            query.AppendLine("       [Etiqueta]           = Etiqueta                                                                                                           ");
            query.AppendLine("      ,[gEtiqueta]              = GEtiqueta                                                                                                      ");
            query.AppendLine("      ,[Numero]				= TelDest                                                                                                          ");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisDetallados('Detall','DetalleCDR','Español')]                                                              ");
            query.AppendLine("WHERE GEtiqueta <> @gpoEtiqNoIdent                                                                                                               ");
            query.AppendLine("	AND Emple = @emple                                                                                                                             ");
            query.AppendLine("	AND TDest <> @llamsEntrada                                                                                                                     ");
            query.AppendLine("	AND TDest <> @llamsEnlace                                                                                                                      ");
            query.AppendLine("	AND ( LEN(TelDest) = 3 OR LEN(TelDest) >= 7 )                                                                                                  ");
            query.AppendLine("	AND TelDest='" + telDest + "'                                                                                                                      ");
            query.AppendLine("GROUP BY TelDest, GEtiqueta,Etiqueta                                                                                                             ");
            query.AppendLine("ORDER BY SUM(Costo + CostoSM) DESC                                                                                                               ");

            return query.ToString();
        }

        private DataRow GetTotalesDetalleLlams(DataTable dtResultDetall)
        {
            int duracion = 0;
            double costo = 0;
            foreach (DataRow row in dtResultDetall.Rows)
            {
                duracion += Convert.ToInt32(row["Duracion"]);
                costo += Convert.ToDouble(row["Costo"].ToString().Replace("$", "").Trim());
            }

            DataRow newRow = dtResultDetall.NewRow();
            newRow["Duracion"] = duracion;
            newRow["Costo"] = costo.ToString();

            return newRow;
        }

        protected void btnLinkNumero_Click(object sender, EventArgs e)
        {
            LinkButton lnkView = sender as LinkButton;
            string numTelDest = lnkView.CommandArgument;

            ReetiquetarNumMarcado(numTelDest);
            lblTituloDetallLlams.Text = "ETIQUETACIÓN ";
            mpeEtqDetallLlams.Show();
        }

        protected void btnOK_Click(object sender, EventArgs e)
        {
            listaEtiq.Clear();
            int gpoNoIdentificado = Convert.ToInt32(DSODataAccess.ExecuteScalar("SELECT GEtiqueta FROM [VisHistoricos('GpoEtiqueta','Grupo Etiquetacion','Español')] WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo ='0NoIdent'"));
            string mensajeError = string.Empty;

            EtiquetacionModelView objEtiq = new EtiquetacionModelView();
            objEtiq.Numero = lblNumeropopup.Text;
            objEtiq.Grupo = Convert.ToInt32(ddltblGetiqueta.SelectedValue);
            objEtiq.Etiqueta = txttblEtiqueta.Text;

            if (objEtiq.Grupo == gpoNoIdentificado && !string.IsNullOrEmpty(objEtiq.Etiqueta))
            {
                mensajeError = "No se puede introducir una etiqueta para el Grupo 'No Identificada'.";

            }
            else if (objEtiq.Grupo != gpoNoIdentificado && string.IsNullOrEmpty(objEtiq.Etiqueta))
            {
                mensajeError = "No se puede dejar una etiqueta en blanco para un grupo diferente de 'No Identificada'.";
            }
            else
            {
                listaEtiq.Add(objEtiq);
            }

            if (!string.IsNullOrEmpty(mensajeError))
            {
                lblTituloModalMsn.Text = "Error en la etiquetación de números";
                lblBodyModalMsn.Text = mensajeError;
                mpeEtqMsn.Show();
            }
            else
            {
                ////valor de la categoria
                //string gEtiqueta = string.Empty;
                //string numero = string.Empty;
                //string Etiqueta = string.Empty;

                //numero = objEtiq.Numero;
                //Etiqueta = objEtiq.Etiqueta;
                //gEtiqueta = objEtiq.Grupo.ToString();
                for (int i = 0; i < gridResumenNums.Rows.Count; i++)
                {
                    LinkButton linkButton = (gridResumenNums.Rows[i].FindControl("btnLinkNumero")) as LinkButton;

                    if (linkButton.Text == objEtiq.Numero)
                    {
                        TextBox txtEtiqueta = (gridResumenNums.Rows[i].FindControl("txtEtiqueta")) as TextBox;
                        txtEtiqueta.Text = objEtiq.Etiqueta;

                        DropDownList ddlgEtiqueta = (gridResumenNums.Rows[i].FindControl("ddlGrupoEtiqueta")) as DropDownList;
                        ddlgEtiqueta.SelectedValue = objEtiq.Grupo.ToString();

                        break;
                    }
                }

                EtiquetarNumeros(gpoNoIdentificado);
            }
        }

        protected void ReetiquetarNumMarcado(string telDest)
        {
            txttblEtiqueta.Text = "";
            ddltblGetiqueta.DataSource = null;
            lblNumeropopup.Text = "";

            DataTable dtGposEtiquetas = new DataTable();

            var dtResult = DSODataAccess.Execute(ConsultaConsultaCamposAReetiquetar(telDest));
            if (dtResult.Rows.Count > 0)
            {
                dtGposEtiquetas = DSODataAccess.Execute(ConsultaTiposGEtiquetas());

                ddltblGetiqueta.DataSource = dtGposEtiquetas;
                ddltblGetiqueta.DataTextField = "vchDescripcion";
                ddltblGetiqueta.DataValueField = "gEtiqueta".ToString();
                ddltblGetiqueta.DataBind();

                txttblEtiqueta.Text = dtResult.Rows[0][0].ToString();
                ddltblGetiqueta.SelectedValue = dtResult.Rows[0][1].ToString();
                lblNumeropopup.Text = dtResult.Rows[0][2].ToString();
            }
        }

        #endregion
    }
}

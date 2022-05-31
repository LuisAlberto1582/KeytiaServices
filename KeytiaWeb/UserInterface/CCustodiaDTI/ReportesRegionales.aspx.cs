using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardFC;
using KeytiaWeb.UserInterface.DashboardLT;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.CCustodiaDTI
{
    public partial class ReportesRegionales : System.Web.UI.Page
    {
        static string esquema = DSODataContext.Schema;
        static string connStr = DSODataContext.ConnectionString;
        static string iCodUsuario = string.Empty;
        static string iCodPerfil = string.Empty;
        static string fechaInicio = "";
        static string fechaFinal = "";
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            iCodUsuario = Session["iCodUsuario"].ToString();
            iCodPerfil = Session["iCodPerfil"].ToString();

            if (!Page.IsPostBack)
            {
                IniciaProceso();
                ObtieneRegiones();
                ObtieneFechaFact();
                validaUsuario();
            }

            if (Convert.ToInt32(iCodPerfil) == 367 || Convert.ToInt32(iCodPerfil) == 369)
            {
                chkTodos.Visible = true;
            }
            else
            {
                chkTodos.Visible = false;
            }
        }
        private void validaUsuario()
        {
            if (Convert.ToInt32(iCodPerfil) == 367 || Convert.ToInt32(iCodPerfil) == 369)
            {
                cboRegion.Enabled = true;
                row1.Visible = true;
                row2.Visible = true;
                row3.Visible = true;
                rowBuscarText.Visible = true;
                ObtieneInfoDetalle();
            }
            else
            {
                cboRegion.Enabled = false;
                /*obtener la region que puede ver el usuario*/
                string region = ObtieneRegionUsuario();

                if (region == "")
                {
                    /*mostrar un mensaje que no tiene permisos*/
                    lblMensajeInfo.Text = "¡No cuenta con permisos para ver este reporte!";
                    pnlInfo.Visible = true;

                    row1.Visible = false;
                    row2.Visible = false;
                    row3.Visible = false;
                    rowBuscarText.Visible = false;
                }
                else
                {
                    /*ASIGNAR LA REGION AL COMBO DE REGIONES*/
                    cboRegion.Text = region;
                    row1.Visible = true;
                    row2.Visible = true;
                    row3.Visible = true;
                    pnlInfo.Visible = false;
                    rowBuscarText.Visible = true;

                    ObtieneInfoDetalle();
                }
            }

        }
        private void IniciaProceso()
        {
            cboAnio.DataSource = GetDataDropDownList("ANIO").DefaultView;
            cboAnio.DataBind();
            cboMes.DataSource = GetDataDropDownList("MES").DefaultView;
            cboMes.DataBind();
        }
        private DataTable GetDataDropDownList(string clave)
        {
            ObtieneAnio();
            StringBuilder query = new StringBuilder();
            bool isEstatus = false;
            query.Length = 0;
            query.AppendLine("SELECT [CAMPOS]");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[NOMVISTA]");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            query.AppendLine("	AND dtFinVigencia >= GETDATE()");
            query = query.Replace("[CAMPOS]", "CASE WHEN LEN(VCHCODIGO) = 1 THEN '0' + VCHCODIGO ELSE VCHCODIGO END AS vchCodigo, UPPER(Español) AS Descripcion");
            query = query.Replace("[NOMVISTA]", "[VisHistoricos('Mes','Meses','Español')]");
            return AddRowDefault(DSODataAccess.Execute(query.ToString()), isEstatus);
        }
        private DataTable AddRowDefault(DataTable dt, bool estatus)
        {
            if (dt.Rows.Count > 0)
            {
                DataRow rowExtra = dt.NewRow();
                rowExtra["vchCodigo"] = 0;
                rowExtra["Descripcion"] = !estatus ? "TODOS" : "Seleccionar";
                dt.Rows.InsertAt(rowExtra, 0);
            }
            return dt;
        }
        private void ObtieneFechaFact()
        {
            string query = "EXEC ObtieneFechaMaximaFactura @Schema = '" + esquema + "',@Carrier = 'TELCEL'";
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                ValidaSelectCombo(dr["Anio"].ToString().ToString(), cboAnio);
                ValidaSelectCombo(dr["Mes"].ToString(), cboMes);
                DateTime fechaAux1 = Convert.ToDateTime(dr["FechaInicio"]);
                DateTime fechaFinal1 = new DateTime(fechaAux1.Year, fechaAux1.Month, 1);
                DateTime fechaInicio1 = fechaAux1.AddMonths(11);

                DateTime fecIni = fechaAux1.AddMonths(-2);
                DateTime fecFin = fecIni.AddMonths(11);

                fechaInicio = fecIni.ToString("yyyy-MM-dd");
                fechaFinal = fecFin.ToString("yyyy-MM-dd");

            }
        }
        private void ValidaSelectCombo(string valor, DropDownList cbo)
        {
            string itemToCompare = string.Empty;
            string itemOrigin = valor.ToUpper();
            foreach (ListItem item in cbo.Items)
            {
                itemToCompare = item.Text.ToUpper();
                if (itemOrigin == itemToCompare)
                {
                    cbo.ClearSelection();
                    item.Selected = true;
                }
            }
        }
        private void ObtieneAnio()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT iCodCatalogo AS vchCodigo, vchDescripcion AS Descripcion FROM " + esquema + ".[VisHistoricos('Anio','Años','Español')] WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND vchDescripcion IN(DATEPART(YEAR, GETDATE()),DATEPART(YEAR, DATEADD(YEAR, -1, GETDATE())), DATEPART(YEAR, DATEADD(YEAR, -1, GETDATE())))");
            query.AppendLine(" ORDER BY vchDescripcion DESC");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                cboAnio.DataSource = dt;
                cboAnio.DataBind();
            }
        }
        private void ObtieneRegiones()
        {
            cboRegion.Enabled = true;
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT DISTINCT(UPPER(FILLER)) AS REGION FROM " + esquema + ".HistLinea WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia<> dtFinVigencia");
            query.AppendLine(" AND dtFinVigencia >= GETDATE()");
            //query.AppendLine(" AND FILLER NOT IN('#N/A','SIN REGION','N/A','NA','')");
            query.AppendLine(" AND FILLER <> 'SIN REGION'");
            query.AppendLine(" AND FILLER NOT LIKE '%N%A%'");
            query.AppendLine(" AND FILLER NOT LIKE '%-AREA%'");
            query.AppendLine(" AND ISNULL(FILLER,'') <> ''");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                cboRegion.DataSource = dt;
                cboRegion.DataBind();
            }

        }
        private string ObtieneRegionUsuario()
        {
            string region = "";

            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT Region FROM " + esquema + ".[VisHistoricos('UsuariosRegiones','Usuarios Regiones','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia");
            query.AppendLine(" AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND Usuar = " + Convert.ToInt32(iCodUsuario) + "");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                region = dr["Region"].ToString();
            }

            return region;
        }
        private void ObtieneInfoDetalle()
        {
            try
            {
                string region = cboRegion.SelectedValue.ToString();
                string sp = "EXEC [dbo].ObtieneResumenGeneral @Esquema = '{0}',@FecIni = '{1}',@FecFin = '{2}',@Filler = '{3}'";
                string query = string.Format(sp, esquema, fechaInicio, fechaFinal, region);
                DataTable dtRes = DSODataAccess.Execute(query, connStr);
                if (dtRes != null && dtRes.Rows.Count > 0)
                {
                    /*Resumen General*/
                    GeneraResumenGeneral(dtRes);
                    /*Telefonia Movil*/
                    GeneraTelMovil(dtRes);
                    /*Telefonia Fija*/
                    GeneraTelFija(dtRes);
                    /*Internet*/
                    GeneraTelInternet(dtRes);

                    /*Genera DetalleMovil*/
                    GeneraDetalleMovil(region);
                }
                else
                {
                    lblMensajeInfo.Text = "¡No se encontro información para mostrar!";
                    pnlInfo.Visible = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
        private void GeneraResumenGeneral(DataTable dt)
        {
            try
            {
                DataView dvldt = new DataView(dt);
                dt = dvldt.ToTable(false, new string[] { "Mes", "LineaMovil", "LineaFija", "LineaInternet" });
                dt.Columns["LineaMovil"].ColumnName = "Linea Movil";
                dt.Columns["LineaFija"].ColumnName = "Linea Fija";
                dt.Columns["LineaInternet"].ColumnName = "Internet";

                resumenGral.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                DTIChartsAndControls.GridView("RepResumenGeneralGrid", dt, true, "Totales",
                                 new string[] { "", "{0:c}", "{0:c}", "{0:c}" }), "RepResumenGeneralGrid",
                                "RESUMEN TIPO SERVICIO", "", 0, FCGpoGraf.MatricialConStack2)
                        );

                dt.Columns["Linea Movil"].ColumnName = "TIPO SERVICIO LINEA MOVIL";
                dt.Columns["Linea Fija"].ColumnName = "TIPO SERVICIO LINEA FIJA";
                dt.Columns["Internet"].ColumnName = "TIPO SERVICIO INTERNET";
                dt.AcceptChanges();


                DataTable dt1 = new DataTable();
                dt1.Columns.Add("Mes", typeof(string));
                dt1.Columns.Add("TIPO SERVICIO LINEA MOVIL", typeof(string));
                dt1.Columns.Add("TIPO SERVICIO LINEA FIJA", typeof(string));
                dt1.Columns.Add("TIPO SERVICIO INTERNET", typeof(string));

                foreach (DataRow dr in dt.Rows)
                {

                    DataRow row = dt1.NewRow();
                    row["Mes"] = dr["Mes"].ToString();
                    row["TIPO SERVICIO LINEA MOVIL"] = (dr["TIPO SERVICIO LINEA MOVIL"].ToString() == "0") ? "" : dr["TIPO SERVICIO LINEA MOVIL"].ToString();
                    row["TIPO SERVICIO LINEA FIJA"] = (dr["TIPO SERVICIO LINEA FIJA"].ToString() == "0") ? "" : dr["TIPO SERVICIO LINEA FIJA"].ToString();
                    row["TIPO SERVICIO INTERNET"] = (dr["TIPO SERVICIO INTERNET"].ToString() == "0") ? "" : dr["TIPO SERVICIO INTERNET"].ToString();
                    dt1.Rows.Add(row);
                }

                if (dt1.Rows[dt1.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    dt1.Rows[dt1.Rows.Count - 1].Delete();
                }

                string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(dt1));
                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepResumenGeneralGrid",
                    FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(dt1), "RepResumenGeneralGrid",
                    "", "", "", "", 0, FCGpoGraf.MatricialConStack2, "$", ""), false);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private void GeneraTelMovil(DataTable dt)
        {
            try
            {
                DataView dvldt = new DataView(dt);
                dt = dvldt.ToTable(false, new string[] { "Mes", "LineaMovil", "PromedioMovil" });
                dt.Columns["LineaMovil"].ColumnName = "IMPORTE MENSUAL";
                dt.Columns["PromedioMovil"].ColumnName = "PROYECCION";
                dt.AcceptChanges();

                telMovil.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                DTIChartsAndControls.GridView("RepTelMovilGrid", dt, true, "Totales",
                                 new string[] { "", "{0:c}", "{0:c}", "{0:c}" }), "RepTelMovilGrid",
                                "TELEFONÍA MOVIL", "", 2, FCGpoGraf.MatricialConStack2)
                        );

                DataTable dt2 = new DataTable();
                dt2.Columns.Add("Mes", typeof(string));
                dt2.Columns.Add("IMPORTE MENSUAL", typeof(string));
                dt2.Columns.Add("PROYECCION", typeof(string));

                foreach (DataRow dr in dt.Rows)
                {

                    DataRow row = dt2.NewRow();
                    row["Mes"] = dr["Mes"].ToString();
                    row["IMPORTE MENSUAL"] = (dr["IMPORTE MENSUAL"].ToString() == "0") ? "" : dr["IMPORTE MENSUAL"].ToString();
                    row["PROYECCION"] = Convert.ToUInt32(dr["PROYECCION"]);

                    dt2.Rows.Add(row);
                }

                if (dt2.Rows[dt2.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    dt2.Rows[dt2.Rows.Count - 1].Delete();
                }


                string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(dt2));
                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTelMovilGrid",
                    FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(dt2), "RepTelMovilGrid",
                    "", "", "", "", 2, FCGpoGraf.MatricialConStack2, "$", ""), false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void GeneraTelFija(DataTable dt)
        {
            try
            {
                DataView dvldt = new DataView(dt);
                dt = dvldt.ToTable(false, new string[] { "Mes", "LineaFija", "PromedioFija" });
                dt.Columns["LineaFija"].ColumnName = "IMPORTE MENSUAL";
                dt.Columns["PromedioFija"].ColumnName = "PROYECCION";
                dt.AcceptChanges();

                telFija.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                DTIChartsAndControls.GridView("RepTelFijaGrid", dt, true, "Totales",
                                 new string[] { "", "{0:c}", "{0:c}", "{0:c}" }), "RepTelFijaGrid",
                                "TELEFONÍA FIJA", "", 2, FCGpoGraf.MatricialConStack2)
                        );


                DataTable dt3 = new DataTable();
                dt3.Columns.Add("Mes", typeof(string));
                dt3.Columns.Add("IMPORTE MENSUAL", typeof(string));
                dt3.Columns.Add("PROYECCION", typeof(string));

                foreach (DataRow dr in dt.Rows)
                {

                    DataRow row = dt3.NewRow();
                    row["Mes"] = dr["Mes"].ToString();

                    row["IMPORTE MENSUAL"] = (dr["IMPORTE MENSUAL"].ToString() == "0") ? "" : dr["IMPORTE MENSUAL"].ToString();

                    row["PROYECCION"] = Convert.ToUInt32(dr["PROYECCION"]);

                    dt3.Rows.Add(row);
                }


                if (dt3.Rows[dt3.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    dt3.Rows[dt3.Rows.Count - 1].Delete();
                }


                string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(dt3));
                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTelFijaGrid",
                    FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(dt3), "RepTelFijaGrid",
                    "", "", "", "", 2, FCGpoGraf.MatricialConStack2, "$", ""), false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void GeneraTelInternet(DataTable dt)
        {
            try
            {
                DataView dvldt = new DataView(dt);
                dt = dvldt.ToTable(false, new string[] { "Mes", "LineaInternet", "PromedioInternet" });
                dt.Columns["LineaInternet"].ColumnName = "IMPORTE MENSUAL";
                dt.Columns["PromedioInternet"].ColumnName = "PROYECCION";
                dt.AcceptChanges();

                telInternet.Controls.Add(
                        DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                DTIChartsAndControls.GridView("RepTelInternetGrid", dt, true, "Totales",
                                 new string[] { "", "{0:c}", "{0:c}" }), "RepTelInternetGrid",
                                "INTERNET", "", 2, FCGpoGraf.MatricialConStack2)
                        );

                DataTable dt4 = new DataTable();
                dt4.Columns.Add("Mes", typeof(string));
                dt4.Columns.Add("IMPORTE MENSUAL", typeof(string));
                dt4.Columns.Add("PROYECCION", typeof(string));

                foreach (DataRow dr in dt.Rows)
                {

                    DataRow row = dt4.NewRow();
                    row["Mes"] = dr["Mes"].ToString();

                    row["IMPORTE MENSUAL"] = (dr["IMPORTE MENSUAL"].ToString() == "0") ? "" : dr["IMPORTE MENSUAL"].ToString();

                    row["PROYECCION"] = Convert.ToUInt32(dr["PROYECCION"]);

                    dt4.Rows.Add(row);
                }


                if (dt4.Rows[dt4.Rows.Count - 1]["Mes"].ToString() == "Totales")
                {
                    dt4.Rows[dt4.Rows.Count - 1].Delete();
                }


                string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(dt4));
                Page.ClientScript.RegisterStartupScript(this.GetType(), "RepTelInternetGrid",
                    FCAndControls.GraficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(dt4), "RepTelInternetGrid",
                    "", "", "", "", 2, FCGpoGraf.MatricialConStack2, "$", ""), false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void GeneraDetalleMovil(string region)
        {
            string anio = cboAnio.SelectedItem.ToString();
            string mes = cboMes.SelectedValue.ToString();

            var mes1 = (mes.Length == 1) ? "0" + mes : mes;
            string fecValida = anio + "-" + mes1 + "-" + "01";
            DataTable dt = DSODataAccess.Execute(ObtieneDetallMovil(region, fecValida), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                dt = DTIChartsAndControls.ordenaTabla(dt, "CONSUMO desc");

                row4.Visible = true;
                string[] formatos = { "", "", "", "", "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}" };
                Control Controles = DTIChartsAndControls.GridView("RepDetallLineasMovil", dt, true, "Totales", formatos);
                pnl1.Controls.Add(Controles);
            }

        }
        protected void btnAplicarFecha_Click(object sender, EventArgs e)
        {

            GeneraDatos();
        }

        private void GeneraDatos()
        {
            try
            {

                string anio = cboAnio.SelectedItem.ToString();
                string mes = cboMes.SelectedValue.ToString();

                var mes1 = (mes.Length == 1) ? "0" + mes : mes;
                string fecValida = anio + "-" + mes1 + "-" + "01";

                DateTime fechaAux1 = Convert.ToDateTime(fecValida);
                DateTime fechaIni = fechaAux1.AddMonths(-2);
                //DateTime fechaFinal1 = new DateTime(fechaAux1.Year, fechaAux1.Month, 1);
                DateTime fechaFin = fechaIni.AddMonths(11);

                fechaInicio = fechaIni.ToString("yyyy-MM-dd");
                fechaFinal = fechaFin.ToString("yyyy-MM-dd");

                validaUsuario();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        protected void btnYes_Click(object sender, EventArgs e)
        {
            /*insertar la solicitud de la generacion del reporte*/
            try
            {
                string asunto = txtAsunto.Text.Trim();
                string destinatario = txtEmail.Text.Trim();

                if (asunto != "" && destinatario != "")
                {
                    AltaSolicitud(asunto, destinatario);

                    lblTituloModalMsn.Text = "Reporte Regional";
                    lblBodyModalMsn.Text = "La solicitud del Reporte Regional, Se Registro Correctamente.";
                    mpeEtqMsn.Show();
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
                lblTituloModalMsn.Text = "Reporte Regional";
                lblBodyModalMsn.Text = "Ocurrio un error al dar de alta la solicitud.";
                mpeEtqMsn.Show();

                throw ex;
            }

            GeneraDatos();
        }

        private void AltaSolicitud(string asunto, string destinatario)
        {
            try
            {


                int todos = (chkTodos.Checked == true) ? 1 : 0;
                string region = (chkTodos.Checked == true && chkTodos.Visible == true) ? "TODOS" : cboRegion.SelectedValue.ToString();
                if (asunto != "" && destinatario != "")
                {
                    AltaSolicitud(asunto, destinatario, region);

                    txtAsunto.Text = "";
                    txtEmail.Text = "";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void AltaSolicitud(string asunto, string destinatario, string region)
        {

            int idUsuario = Convert.ToInt32(iCodUsuario);
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC dbo.AltaSolicitudRepRegional");
            query.AppendLine(" @Usuario =" + idUsuario + ",");
            query.AppendLine(" @FechaIni ='" + fechaInicio + "',");
            query.AppendLine(" @FechaFin = '" + fechaFinal + "',");
            query.AppendLine(" @Region ='" + region + "',");
            query.AppendLine(" @Asunto ='" + asunto + "',");
            query.AppendLine(" @Destinatario ='" + destinatario + "'");
            DSODataAccess.Execute(query.ToString(), connStr);
        }
        private string ObtieneDetallMovil(string region, string fecha)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" EXEC[dbo].[ObtieneDetalleTelMovil]");
            query.AppendLine(" @Esquema = 'BAT',");
            query.AppendLine(" @FecIni = '" + fecha + "',");
            query.AppendLine(" @Filler = '" + region + "'");
            return query.ToString();
        }
    }
}
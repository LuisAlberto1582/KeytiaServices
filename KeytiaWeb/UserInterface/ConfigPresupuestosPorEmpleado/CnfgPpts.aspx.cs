using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using KeytiaServiceBL;
using System.Data;
using System.Collections;

namespace KeytiaWeb.UserInterface.ConfigPresupuestosPorEmpleado
{
    public partial class CnfgPpts : System.Web.UI.Page
    {
        KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            if (!string.IsNullOrEmpty(Request.QueryString["ProcComp"]))
            {
                if (Request.QueryString["ProcComp"].ToString() == "1")
                {
                    mensajeDeAdvertencia("Se completo el proceso correctamente.");
                }
            }

            if (!string.IsNullOrEmpty(Request.QueryString["iCodCenCos"]))
            {
                string iCodCenCos = Request.QueryString["iCodCenCos"];


                lblPptoActualNum.Text = DSODataAccess.ExecuteScalar(ConsultaPptoDeUnCenCos(Request.QueryString["iCodCenCos"])).ToString();

                if (lblPptoActualNum.Text == "0")
                {
                    Response.Redirect("~/UserInterface/ConfigPresupuestosPorEmpleado/CnfgPpts.aspx?SinPresup=1");
                }

                #region Esconde controles que no se deben ver en la configuracion de empleados
                pnlCenCostos.Visible = false;
                lblCenCos.Visible = false;
                lblEmpleados.Visible = true;
                btnGuardar.Visible = true;
                btnCancelar.Visible = true;
                #endregion

                grvEmpleados.DataSource = creaDataTable(ConsultaEmpleadosDeCenCosEHijos(iCodCenCos));
                grvEmpleados.DataBind();
                grvEmpleados.Columns[0].Visible = false; //iCodRegistro de [VisHistoricos('Emple','Empleados','Español')]
                grvEmpleados.Columns[1].Visible = false; //iCodRegistro de [VisHistoricos('PrepEmple','Presupuesto Fijo','Español')]

                lblPptoActualNumDisponible.Text = (
                                                    Convert.ToInt32(DSODataAccess.ExecuteScalar(ConsultaPptoDeUnCenCos(Request.QueryString["iCodCenCos"])).ToString()) -
                                                    regresaSumaDeColumnaGridView("txtPresupuesto")
                                                ).ToString();
            }

            else
            {
                if (Request.QueryString["SinPresup"] == "1")
                {
                    mensajeDeAdvertencia("El centro de costos seleccionado no cuenta con un presupuesto configurado.");
                }

                #region Esconde controles que no se deben ver en la seleccion de centro de costos
                lblCenCos.Visible = true;
                lblEmpleados.Visible = false;
                btnGuardar.Visible = false;
                btnCancelar.Visible = false;
                lbtnRegresarABusq.Visible = false;
                #endregion

                pnlEmpleados.Visible = false;
                grvCenCos.DataSource = creaDataTable(ConsultaCenCostos());
                grvCenCos.DataBind();
                grvCenCos.Columns[0].Visible = false;
            }
        }

        /// <summary>
        /// Regresa un DataTable con los resultados de la consulta de SQL que recibe como parametro de entrada en forma de string.
        /// </summary>
        /// <param name="querySQL">Query en formato string</param>
        /// <returns></returns>
        protected DataTable creaDataTable(string querySQL)
        {
            DataTable localDT = new DataTable(null);

            try
            {
                localDT.Clear();
                localDT = DSODataAccess.Execute(querySQL);
            }

            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException(
                    "Ocurrio un error en metodo creaDataTable() en KeytiaWebUserInterface.ConfigPresupuestosPorEmpleado.CnfgPpts.aspx.cs '"
                    + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }

            return localDT;
        }

        #region ConsultasSQL

        private string ConsultaCenCostos()
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("select \r");
            lsb.Append("[iCodCenCos] = iCodCatalogo , [Clave] = vchCodigo, Descripcion, [Centro de Costos Responsable] = CenCosDesc, [CenCosResp] = CenCos \r");
            lsb.Append("from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','Español')] \r");
            lsb.Append("where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate() \r");

            return lsb.ToString();
        }

        private string ConsultaEmpleadosDeCenCosEHijos(string iCodCenCos)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append(" \r");
            lsb.Append("WITH CenCos (iCodCatalogo, Descripcion, CenCos, CenCosDesc,  Level) \r");
            lsb.Append("AS \r");
            lsb.Append("( \r");
            lsb.Append("-- Anchor member definition \r");
            lsb.Append("SELECT e.iCodCatalogo, e.Descripcion, e.CenCos, e.CenCosDesc, 0 AS Level \r");
            lsb.Append("FROM [VisHistoricos('CenCos','Centro de Costos','Español')] AS e \r");
            lsb.Append("WHERE dtIniVigencia<>dtFinVigencia and dtFinVigencia>= GETDATE() and \r");
            lsb.Append("iCodCatalogo = " + iCodCenCos + " \r");
            lsb.Append("UNION ALL \r");
            lsb.Append("-- Recursive member definition \r");
            lsb.Append("SELECT  e.iCodCatalogo, e.Descripcion, e.CenCos, e.CenCosDesc,  \r");
            lsb.Append("Level + 1 \r");
            lsb.Append("FROM  [VisHistoricos('CenCos','Centro de Costos','Español')] AS e \r");
            lsb.Append("INNER JOIN CenCos AS d \r");
            lsb.Append("ON e.CenCos = d.iCodCatalogo -- \r");
            lsb.Append("WHERE dtIniVigencia<>dtFinVigencia AND dtFinVigencia>= GETDATE() \r");
            lsb.Append(") \r");
            lsb.Append("SELECT  \r");
            lsb.Append("[iCodRegEmple] = Emple.iCodRegistro,  \r");
            lsb.Append("[iCodRegPpto] = ISNULL(pptoEmple.iCodRegistro, ''), \r");
            lsb.Append("[Nombre del empleado] = Emple.NomCompleto,  \r");
            lsb.Append("[Presupuesto] = ISNULL(pptoEmple.PresupFijo,0),  \r");
            lsb.Append("[Centro de costos] = Emple.CenCosDesc   \r");
            lsb.Append("FROM [VisHistoricos('Emple','Empleados','Español')] Emple \r");
            lsb.Append("LEFT JOIN [VisHistoricos('PrepEmple','Presupuesto Fijo','Español')] pptoEmple \r");
            lsb.Append("ON Emple.iCodCatalogo = pptoEmple.Emple \r");
            lsb.Append("AND pptoEmple.dtIniVigencia <> pptoEmple.dtFinVigencia  \r");
            lsb.Append("AND pptoEmple.dtFinVigencia >= GETDATE()  \r");
            lsb.Append("WHERE Emple.dtIniVigencia <> Emple.dtFinVigencia  \r");
            lsb.Append("AND Emple.dtFinVigencia >= GETDATE()  \r");
            lsb.Append("AND Emple.CenCos IN (SELECT iCodCatalogo FROM CenCos)  \r");

            return lsb.ToString();
        }

        private string ConsultaPptoDeUnCenCos(string iCodCenCos)
        {
            StringBuilder lsb = new StringBuilder();

            lsb.Append("DECLARE @rowcount int \r");
            lsb.Append("SELECT @rowcount = COUNT(*) \r");
            lsb.Append("FROM [VisHistoricos('PrepCenCos','Presupuesto Fijo','Español')]  \r");
            lsb.Append("WHERE dtIniVigencia <> dtFinVigencia  \r");
            lsb.Append("AND dtFinVigencia >= GETDATE()  \r");
            lsb.Append("AND CenCos = " + iCodCenCos + "  \r");
            lsb.Append("IF @rowcount > 0 \r");
            lsb.Append("BEGIN  \r");
            lsb.Append("SELECT ISNULL(PresupFijo, '') \r");
            lsb.Append("FROM [VisHistoricos('PrepCenCos','Presupuesto Fijo','Español')]  \r");
            lsb.Append("WHERE dtIniVigencia <> dtFinVigencia  \r");
            lsb.Append("AND dtFinVigencia >= GETDATE()  \r");
            lsb.Append("AND CenCos = " + iCodCenCos + "  \r");
            lsb.Append("END \r");
            lsb.Append("ELSE \r");
            lsb.Append("BEGIN \r");
            lsb.Append("SELECT '0' AS PresupFijo \r");
            lsb.Append("END \r");

            return lsb.ToString();
        }

        private string ConsultaTemplate()
        {
            StringBuilder lsb = new StringBuilder();
            lsb.Append(" \r");
            return lsb.ToString();
        }

        #endregion // ConsultasSQL

        protected void lbtnRegresarABusq_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/UserInterface/ConfigPresupuestosPorEmpleado/CnfgPpts.aspx");
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/UserInterface/ConfigPresupuestosPorEmpleado/CnfgPpts.aspx");
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                int sumaPpto = regresaSumaDeColumnaGridView("txtPresupuesto");
                int pptoActual = (Convert.ToInt32(lblPptoActualNum.Text));

                if (sumaPpto > pptoActual)
                {
                    mensajeDeAdvertencia("La suma de presupuestos de los empleados, excede el presupuesto total asignado al centro de costos, favor de corregirlo e intentarlo de nuevo.");
                }

                else
                {
                    string iCodRegistroPpts = string.Empty;
                    string iCodRegistroEmple = string.Empty;
                    int iCodRegistroPptsNuevo = 0;

                    foreach (GridViewRow gr in grvEmpleados.Rows)
                    {
                        // Tomo valor de iCodRegistro de presupuestos
                        iCodRegistroPpts = string.Empty;
                        iCodRegistroPpts = gr.Cells[1].Text;

                        // Tomo valor de iCodRegistro de Empleados
                        iCodRegistroEmple = string.Empty;
                        iCodRegistroEmple = gr.Cells[0].Text;

                        // Tomo valor de presupuesto para el empleado
                        TextBox txtPpts = (TextBox)gr.FindControl("txtPresupuesto");

                        if (iCodRegistroPpts != "0")
                        {
                            //Actualiza registro en ppts.
                            DSODataAccess.ExecuteNonQuery("UPDATE [VisHistoricos('PrepEmple','Presupuesto Fijo','Español')] " +
                                                                           "SET PresupFijo = " + txtPpts.Text + ", dtFecUltAct = GETDATE() WHERE iCodRegistro = " + iCodRegistroPpts);

                            // Actualiza registro en Empleados
                            DSODataAccess.ExecuteNonQuery("UPDATE [VisHistoricos('Emple','Empleados','Español')]" +
                                                                           "SET PresupFijo = " + txtPpts.Text + ", dtFecUltAct = GETDATE() WHERE iCodRegistro = " + iCodRegistroEmple);
                        }

                        else
                        {
                            iCodRegistroPptsNuevo = 0;

                            //Da de alta registro en ppts.
                            iCodRegistroPptsNuevo = lCargasCOM.InsertaRegistro(armaHashPptoFijoEmple(iCodRegistroEmple, txtPpts.Text),
                                                                                                    "Historicos", "PrepEmple", "Presupuesto Fijo", (int)HttpContext.Current.Session["iCodUsuarioDB"]);

                            if (iCodRegistroPptsNuevo != -1)
                            {
                                // Actualiza registro en Empleados
                                DSODataAccess.ExecuteNonQuery("UPDATE [VisHistoricos('Emple','Empleados','Español')]" +
                                                                               "SET PresupFijo = " + txtPpts.Text + ", dtFecUltAct = GETDATE() WHERE iCodRegistro = " + iCodRegistroEmple);
                            }
                        }
                    }

                    lblPptoActualNumDisponible.Text = (
                                    Convert.ToInt32(DSODataAccess.ExecuteScalar(ConsultaPptoDeUnCenCos(Request.QueryString["iCodCenCos"])).ToString()) -
                                    regresaSumaDeColumnaGridView("txtPresupuesto")
                                ).ToString();

                    mensajeDeAdvertencia("Se completo el proceso correctamente.");
                    Response.Redirect("~/UserInterface/ConfigPresupuestosPorEmpleado/CnfgPpts.aspx?ProcComp=1");
                }
            }

            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al intentar guardar el presupuesto'" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        /// <summary>
        /// Muestra un mensaje de advertencia en la pagina web.
        /// </summary>
        /// <param name="mensaje"><li>Mensaje que se desea desplegar</li></param>
        protected void mensajeDeAdvertencia(string mensaje)
        {
            string mensajeJQuery = "<p>" + mensaje + "</p>";
            string script = @"<script type='text/javascript'>alerta('" + mensajeJQuery + "');</script>";
            ScriptManager.RegisterStartupScript(this, typeof(Page), "alerta", script, false);
        }

        /// <summary>
        /// Muestra un mensaje de confirmación en la pagina web.
        /// </summary>
        /// <param name="mensaje"><li>Mensaje que se desea desplegar</li></param>
        protected void mensajeDeConfirmacion(string mensaje)
        {
            string mensajeJQuery = "<p>" + mensaje + "</p>";
            string script = @"<script type='text/javascript'>confirm('" + mensajeJQuery + "');</script>";
            ScriptManager.RegisterStartupScript(this, typeof(Page), "confirm", script, false);
        }

        /// <summary>
        /// Recorre cada fila de un gridview y suma el valor de la propiedad text de un control textBox
        /// </summary>
        /// <param name="nombreControlTxtBox">ID del control del cual se requiere hacer una suma.</param>
        /// <returns></returns>
        protected int regresaSumaDeColumnaGridView(string nombreControlTxtBox)
        {
            int suma = 0;
            int i = 0;

            foreach (GridViewRow gr in grvEmpleados.Rows)
            {
                TextBox txtPpts = (TextBox)gr.FindControl(nombreControlTxtBox);

                if (int.TryParse(txtPpts.Text, out i))
                {
                    suma += Convert.ToInt32(txtPpts.Text);
                }
            }

            return suma;
        }

        protected Hashtable armaHashPptoFijoEmple(string iCodRegEmple, string presupuesto)
        {
            Hashtable lht = new Hashtable();

            DataRow ldr = DSODataAccess.ExecuteDataRow("SELECT iCodCatalogo, vchDescripcion, NominaA FROM " +
                                                     "[VisHistoricos('Emple','Empleados','Español')] WHERE iCodRegistro = " + iCodRegEmple);

            string iCodEmple = ldr["iCodCatalogo"].ToString();
            string vchDescEmple = ldr["vchDescripcion"].ToString();
            string nomina = ldr["NominaA"].ToString();

            DateTime ldtIniPpto = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1, 0, 0, 0, 0);
            DateTime ldtIniVigencia = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 0, 0, 0);

            lht.Clear();
            lht.Add("iCodMaestro", 332);
            lht.Add("vchCodigo", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            lht.Add("vchDescripcion", vchDescEmple + " (" + nomina + ")");
            lht.Add("{Emple}", iCodEmple);
            lht.Add("{TipoPr}", 450); // Costo
            lht.Add("{PeriodoPr}", 79339); // Mensual
            lht.Add("{PresupFijo}", presupuesto);
            lht.Add("{FechaInicioPrep}", ldtIniPpto.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            lht.Add("dtIniVigencia", ldtIniVigencia);
            lht.Add("iCodUsuario", HttpContext.Current.Session["iCodUsuario"]);
            lht.Add("dtFecUltAct", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            return lht;
        }

        protected void txtFiltrar_TextChanged(object sender, EventArgs e)
        {

        }

        protected void grvCenCos_EditRow(object sender, ImageClickEventArgs e)
        {
            ImageButton ibtn1 = sender as ImageButton;
            int rowIndex = Convert.ToInt32(ibtn1.Attributes["RowIndex"]);

            GridViewRow selectedRow = (GridViewRow)grvCenCos.Rows[rowIndex];

            Response.Redirect("~/UserInterface/ConfigPresupuestosPorEmpleado/CnfgPpts.aspx?iCodCenCos=" + grvCenCos.DataKeys[rowIndex].Values[0]);
        }
    }
}

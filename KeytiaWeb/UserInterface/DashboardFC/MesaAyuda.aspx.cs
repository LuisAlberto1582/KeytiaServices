using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DSOControls2008;
using System.Data;
using KeytiaServiceBL;
using System.Text;
using System.Collections;
using KeytiaWeb.UserInterface.DashboardLT;
using KeytiaServiceBL.Reportes;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class MesaAyuda : System.Web.UI.Page
    {
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

                //Controles de la pagina actual                
                Rep1.Visible = false;
                Rep2.Visible = false;
                Rep3.Visible = false;

                #region Almacenar en variable de sesion los urls de navegacion
                List<string> list = new List<string>();
                string lsURL = HttpContext.Current.Request.Url.AbsoluteUri.ToString();

                if (Session["pltNavegacionDashFC"] != null) //Entonces ya tiene navegacion almacenada
                {
                    list = (List<string>)Session["pltNavegacionDashFC"];
                }
                if (lsURL.Contains("?Opc="))
                {
                    //Asegurarse eliminar navegacion previa
                    list.Clear();

                    //Le quita el parametro Opc=XXXX
                    lsURL = lsURL.Substring(0, lsURL.IndexOf("?Opc="));
                }

                //Si el url no contiene querystring y la lista tiene urls hay que limpiar la lista
                if (!(lsURL.Contains("?")) && list.Count > 0)
                {
                    //Asegurarse eliminar navegacion previa
                    list.Clear();
                }

                //Si no existe entonces quiere decir que estoy en un nuevo nivel de navegacion
                if (!list.Exists(element => element == lsURL))
                {
                    //Agregar el valor del url actual para almacenarlo en la lista de navegacion
                    list.Add(lsURL);
                }

                //Guardar en sesion la nueva lista
                Session["pltNavegacionDashFC"] = list;

                #endregion
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException(
                      "Ocurrio un error en " + Request.Path
                      + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtNomina.Text))
            {
                var dtResultEmple = DSODataAccess.Execute(DatosEmpleado(txtNomina.Text));
                if (dtResultEmple.Rows.Count > 0)
                {
                    lblNomina.Text = dtResultEmple.Rows[0]["NominaA"].ToString();
                    lblNombre.Text = dtResultEmple.Rows[0]["NomCompleto"].ToString();
                    lblDepartamento.Text = dtResultEmple.Rows[0]["Departamento"].ToString();
                    lblRFC.Text = (dtResultEmple.Rows[0]["RFC"] != DBNull.Value) ? dtResultEmple.Rows[0]["RFC"].ToString() : "";
                    lblPuesto.Text = (dtResultEmple.Rows[0]["Puesto"] != DBNull.Value) ? dtResultEmple.Rows[0]["Puesto"].ToString() : "";
                    lblEmail.Text = (dtResultEmple.Rows[0]["Email"] != DBNull.Value) ? dtResultEmple.Rows[0]["Email"].ToString() : "";
                    lblUbicacion.Text = (dtResultEmple.Rows[0]["Ubicacion"] != DBNull.Value) ? dtResultEmple.Rows[0]["Ubicacion"].ToString() : "";
                    lblTipoEmple.Text = (dtResultEmple.Rows[0]["TipoEmple"] != DBNull.Value) ? dtResultEmple.Rows[0]["TipoEmple"].ToString() : "";
                    lblJefe.Text = (dtResultEmple.Rows[0]["Jefe"] != DBNull.Value) ? dtResultEmple.Rows[0]["Jefe"].ToString() : "";

                    Rep2.Visible = true;
                    var dtResultRecursos = DSODataAccess.Execute(DatosRecursosEmple(Convert.ToInt32(dtResultEmple.Rows[0]["iCodCatalogo"])));
                    if (dtResultRecursos.Rows.Count > 0)
                    {
                        DataView dvdtResultRecursos = new DataView(dtResultRecursos);
                        dtResultRecursos = dvdtResultRecursos.ToTable(false, new string[] { "Recurso", "Estatus", "Código", "Sitio", "Permiso", "Comentario", "Fecha de Asignación", "Fecha Fin de Asignación" });

                        Rep3.Controls.Add(
                       DTIChartsAndControls.tituloYBordesReporte(
                                       DTIChartsAndControls.GridView("RepDetalladoGrid", dtResultRecursos, false, "", 
                                       new string[] { "", "", "", "", "", "", "", "" }, "", new string[] { }, 0, new int[] { },
                                       new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, new int[] { }), "Recursos con los que cuenta el Empleado", 0));
                    }
                    else
                    {
                        Rep3.Controls.Add(
                       DTIChartsAndControls.tituloYBordesReporte(lblReporteSinDatos(), "Recursos con los que cuenta el Empleado", 0));
                    }


                    Rep3.Visible = true;
                }
                else
                {
                    Rep1.Visible = true;
                }
            }
        }

        private string DatosEmpleado(string nomina)
        {
            nomina = nomina.ToLower().Replace("'", "").Replace("delete", "").Replace("drop", "").Replace("insert", "");

            query.Length = 0;
            query.AppendLine("SELECT");
            query.AppendLine("	 Emple.iCodCatalogo");
            query.AppendLine("	, NominaA");
            query.AppendLine("	, NomCompleto");
            query.AppendLine("	, Departamento		= CenCos.Descripcion");
            query.AppendLine("	, RFC");
            query.AppendLine("	, Puesto			= PuestoDesc");
            query.AppendLine("	, Email");
            query.AppendLine("	, Ubicacion			= Ubica");
            query.AppendLine("	, TipoEmple			= TipoEmDesc");
            query.AppendLine("	, Jefe				= RTRIM(LTRIM(SUBSTRING(Emple.EmpleDesc,1, CASE WHEN CHARINDEX('(',Emple.EmpleDesc) <> 0 THEN CHARINDEX('(',Emple.EmpleDesc)-1 ELSE 250 END)))");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')] Emple ");
            query.AppendLine("");
            query.AppendLine("  JOIN " + DSODataContext.Schema + ".[VisRelaciones('CentroCosto-Empleado','Español')] Rel");
            query.AppendLine("	ON Rel.Emple = Emple.iCodCatalogo");
            query.AppendLine("		AND Rel.dtIniVigencia <> Rel.dtFinVigencia");
            query.AppendLine("		AND Rel.dtFinVigencia >= GETDATE()");
            query.AppendLine("");
            query.AppendLine("	JOIN " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','Español')] CenCos");
            query.AppendLine("	ON CenCos.iCodCatalogo = Rel.CenCos");
            query.AppendLine("		AND CenCos.dtIniVigencia <> CenCos.dtFinVigencia");
            query.AppendLine("		AND CenCos.dtFinVigencia >= GETDATE()");
            query.AppendLine("");
            query.AppendLine("WHERE Emple.dtIniVigencia <> Emple.dtFinVigencia ");
            query.AppendLine("	AND Emple.dtFinVigencia >= GETDATE()");
            query.AppendLine("	AND Emple.NominaA = '" + nomina + "'");
            return query.ToString();
        }

        private string DatosRecursosEmple(int iCodCatEmple)
        {
            query.Length = 0;
            query.AppendLine("EXEC [MSTIGetRecursEmple] @Esquema = '" + DSODataContext.Schema + "', @iCodCatEmple = " + iCodCatEmple);
            return query.ToString();
        }

        public Control lblReporteSinDatos()
        {
            Panel lpanel = new Panel();
            lpanel.HorizontalAlign = HorizontalAlign.Center;

            Label reporteSinDatos = new Label();
            reporteSinDatos.Text = "El Empleado no cuenta con recursos asignados actualmente";

            lpanel.Controls.Add(reporteSinDatos);

            return lpanel;
        }
    }
}

using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardLT;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class AdministracionRangoIpCajeros : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            RepRangosIpCajeros();

            if (!IsPostBack)
            {
                agregadoRangoIpDiv.Attributes.CssStyle.Add("display", "none");
            }
        }

        #region Reportes 

        private void RepRangosIpCajeros()
        {

            string tituloReporte = "Direcciones de IP para cajeros";

            DataTable dt = DSODataAccess.Execute(ConsultaRangosIpCajeros());

            if (dt != null && dt.Rows.Count > 0)
            {
                int[] camposBoundField = new int[] { 0, 1, 2, 3, 4 };
                int[] camposNavegacion = new int[] { 0 };
                string[] formatoColumnas = new string[] { "", "", "", "", "" };

                GridView grid = DTIChartsAndControls.GridView("ReportePrincipal", dt, false, "",
                      formatoColumnas, "", new string[] { "" }, 1,
                      camposNavegacion, camposBoundField, new int[] { }, 2);
                dt.AcceptChanges();
                Rep1.Controls.Clear();
                Rep1.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(grid, "ReportePrincipal", tituloReporte, string.Empty));
            }
            else
            {
                Label sinInfo = new Label();
                sinInfo.Text = "No hay información por mostrar";
                Rep1.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "ReportePrincipal", tituloReporte, string.Empty));
            }
        }

        #endregion

        #region Consultas

        private string ConsultaRangosIpCajeros()
        {
            StringBuilder query = new StringBuilder();
            //query.AppendLine("SELECT ");
            //query.AppendLine("RangoIp AS [Dirección IP],");
            //query.AppendLine("UbicacionAsignacion AS [Ubicación de Asignada],");
            //query.AppendLine("TipoEnlace AS [Tipo de Enlace asignado],");
            //query.AppendLine("FechaIngresoRango AS [Fecha Ingreso de Rango],");
            //query.AppendLine("FechaAsignacionRango AS [Fecha de asignación a ubicación]");
            //query.AppendLine("FROM Keytia5.k5banorte.RangoIpDeCajeros");

            query.AppendLine("SELECT ");
            query.AppendLine("RangoIp AS [Dirección IP],");
            query.AppendLine("UbicacionAsignacion AS [Ubicación de Asignada],");
            query.AppendLine("TipoEnlace AS [Tipo de Enlace asignado],");
            query.AppendLine("convert(varchar(10), FechaIngresoRango, 120) AS [Fecha Ingreso de Rango],");
            query.AppendLine("convert(varchar(10), FechaAsignacionRango, 120) AS [Fecha de asignación a ubicación]");
            query.AppendLine("FROM Keytia5.k5banorte.RangoIpDeCajeros");

            return query.ToString();
        }

        private string InsertRangoIP(string rangoIp)
        {
            StringBuilder qry = new StringBuilder();
            qry.AppendLine("INSERT INTO Keytia5.k5banorte.RangoIpDeCajeros (");
            qry.AppendLine("RangoIP,");
            qry.AppendLine("UbicacionAsignacion,");
            qry.AppendLine("FechaIngresoRango,");
            qry.AppendLine("FechaAsignacionRango,");
            qry.AppendLine("dtIniVigencia,");
            qry.AppendLine("dtFinVigencia,");
            qry.AppendLine("dtFecUltAct");
            qry.AppendLine(")");
            qry.AppendLine("VALUES");
            qry.AppendLine("(");
            qry.AppendLine("'valor',").Replace("valor", rangoIp);
            qry.AppendLine("NULL,");
            qry.AppendLine("GETDATE(),");
            qry.AppendLine("NULL,");
            qry.AppendLine("CONVERT(VARCHAR(10),GETDATE(),121),");
            qry.AppendLine("'2079-01-01',");
            qry.AppendLine("GETDATE()");
            qry.AppendLine(");");

            return qry.ToString();
        }

        private string InsertVariosValores(List<string> listadoIps)
        {
            StringBuilder qryValue = new StringBuilder();
            StringBuilder qry = new StringBuilder();

            qry.AppendLine("INSERT INTO Keytia5.k5banorte.RangoIpDeCajeros (");
            qry.AppendLine("RangoIP,");
            qry.AppendLine("UbicacionAsignacion,");
            qry.AppendLine("FechaIngresoRango,");
            qry.AppendLine("FechaAsignacionRango,");
            qry.AppendLine("dtIniVigencia,");
            qry.AppendLine("dtFinVigencia,");
            qry.AppendLine("dtFecUltAct,");
            qry.AppendLine("TipoEnlace");
            qry.AppendLine(")");
            qry.AppendLine("VALUES");

            for (int i = 0; i < listadoIps.Count; i++)
            {
                qryValue.AppendLine("(");
                qryValue.AppendLine("'valor',").Replace("valor", listadoIps[i]);
                qryValue.AppendLine("NULL,");
                qryValue.AppendLine("GETDATE(),");
                qryValue.AppendLine("NULL,");
                qryValue.AppendLine("CONVERT(VARCHAR(10),GETDATE(),121),");
                qryValue.AppendLine("'2079-01-01',");
                qryValue.AppendLine("GETDATE(),");
                qryValue.AppendLine("'valor'").Replace("valor", cboTipoEnlace.SelectedValue);

                if (i == listadoIps.Count - 1)
                {
                    qryValue.AppendLine(");");
                }
                else
                {
                    qryValue.AppendLine("),");
                }

                qry.AppendLine(qryValue.ToString());
                qryValue.Clear();
            }


            return qry.ToString();
        }

        #endregion

        #region Botones 

        protected void AgregarRangoIp_Click(object sender, EventArgs e)
        {
            Rep1.Attributes.CssStyle.Add("display", "none");
            btnAgregarRangoIp.Attributes.CssStyle.Add("display", "none");
            agregadoRangoIpDiv.Attributes.CssStyle.Add("display", "");
        }

        protected void AgregarIp_Click(object sender, EventArgs e)
        {
            string qry;
            string ipNoValidas = "";
            List<string> ipValidas = new List<string>();

            Rep1.Attributes.CssStyle.Add("display", "");
            btnAgregarRangoIp.Attributes.CssStyle.Add("display", "");
            agregadoRangoIpDiv.Attributes.CssStyle.Add("display", "none");

            // Validamos IP validas y agregamos a lista
            if (!string.IsNullOrEmpty(txtNombre.Text))
            {
                ipValidas = ObtieneListadoIpValidas(txtNombre.Text);
                qry = InsertVariosValores(ipValidas);
                DSODataAccess.Execute(qry);
                txtNombre.Text = "";
                RepRangosIpCajeros();
            }
            else
            {
                string script = "MostrarMensajesDeError();";
                ScriptManager.RegisterStartupScript(this, GetType(),
                                      "ServerControlScript", script, true);
            }

            //if (!string.IsNullOrEmpty(txtNombre.Text))
            //{
            //    if (ValidaDireccionIp(txtNombre.Text))
            //    {
            //        qry = InsertRangoIP(txtNombre.Text);

            //        DSODataAccess.Execute(qry);

            //        txtNombre.Text = "";

            //        RepRangosIpCajeros();
            //    }
            //    else
            //    {
            //        string script = "MostrarMensajesDeError();";
            //        ScriptManager.RegisterStartupScript(this, GetType(),
            //                              "ServerControlScript", script, true);
            //    }
            //}
            //else
            //{
            //    string script = "MostrarMensajesDeError();";
            //    ScriptManager.RegisterStartupScript(this, GetType(),
            //                          "ServerControlScript", script, true);
            //}
        }

        protected void CancelarAgregadoIp_Click(object sender, EventArgs e)
        {
            Rep1.Attributes.CssStyle.Add("display", "");
            btnAgregarRangoIp.Attributes.CssStyle.Add("display", "");
            agregadoRangoIpDiv.Attributes.CssStyle.Add("display", "none");
        }

        #endregion

        #region Metodos auxiliares y estilos CSS

        protected static bool ValidaDireccionIp(string IpAddress)
        {
            bool flag;

            try
            {
                IPAddress IP;

                if (IpAddress.Count(c => c == '.') == 3)
                {
                    flag = IPAddress.TryParse(IpAddress, out IP);
                }
                else
                {
                    flag = false;
                }
            }
            catch (Exception)
            {
                flag = false;
            }

            return flag;
        }

        protected static List<string> ObtieneListadoIpValidas(string listadoIp)
        {
            List<string> container = new List<string>();
            List<string> ipNoValidas = new List<string>();

            string[] ipSeparadas = listadoIp.Split(',');

            foreach (var ipStr in ipSeparadas)
            {
                if (ValidaDireccionIp(ipStr))
                {
                    container.Add(ipStr);
                }
                else
                {
                    ipNoValidas.Add(ipStr);
                }
            }

            return container;
        }

        #endregion
    }
}
using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardLT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.RepTIM
{
    public partial class RepTIMCarga : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                StringBuilder query1 = new StringBuilder();
                query1.AppendLine("select distinct FechaFactura from " + DSODataContext.Schema + ".TIMResumenMovimientosInventario order by FechaFactura asc");
                var cargas = DSODataAccess.Execute(query1.ToString());
                DropDownList1.DataSource = cargas;
                DropDownList1.DataValueField = "FechaFactura";
                DropDownList1.DataTextField = "FechaFactura";
                DropDownList1.DataBind();
                //this.DropDownList1.SelectedIndexChanged += new System.EventHandler(DropDownList1_SelectedIndexChanged);

                ConsultaTablaResultadoCargaTIMAltas(Rep1);
                ConsultaTablaResultadoCargaTIMBajas(Rep2);
                ConsultaTablaResultadoCargaTIMUpdateAltasToBajas(Rep3);
                ConsultaTablaResultadoCargaTIMUpdateCuenta(Rep4);
                ConsultaTablaResultadoCargaTIMCobrosDobles(Rep5);

            }
        }
        private string ConsultaTablaResultadoCargaAltas(string ICodCarga)
        {

            StringBuilder query = new StringBuilder();

            query.AppendLine("SELECT ");
            query.AppendLine("TablaTIMInventario.iCodCatCarga,");
            query.AppendLine("Empresas.vchDescripcion as Empresa,");
            query.AppendLine("TablaTIMInventario.TipoMovimiento as [Tipo de Movimiento],");
            query.AppendLine("TablaTIMInventario.LadaTelefono,");
            query.AppendLine("TablaTIMInventario.ClaveCargo,");
            query.AppendLine("TablaTIMInventario.Cuenta,");
            query.AppendLine("TablaTIMInventario.Subcuenta,");
            query.AppendLine("TablaTIMInventario.FechaFactura");
            query.AppendLine("FROM " + DSODataContext.Schema + ".TIMResumenMovimientosInventario as TablaTIMInventario");
            query.AppendLine(" join " + DSODataContext.Schema + ".HistCarrier as Empresas");
            query.AppendLine("ON TablaTIMInventario.iCodCatCarrier = Empresas.iCodCatalogo");
            query.AppendLine("AND Empresas.dtIniVigencia<> Empresas.dtFinVigencia");
            query.AppendLine("AND Empresas.dtFinVigencia >= GETDATE()");
            query.AppendLine("AND TipoMovimiento= 'Alta'");

            if (ICodCarga != "0")
            {
                query.AppendLine(" WHERE FechaFactura = '" + ICodCarga + "' ");
            }
            return query.ToString();
        }

        private string ConsultaTablaResultadoCargaBajas(string ICodCarga)
        {

            StringBuilder queryBajas = new StringBuilder();

            queryBajas.AppendLine("SELECT ");
            queryBajas.AppendLine("TablaTIMInventario.iCodCatCarga,");
            queryBajas.AppendLine("Empresas.vchDescripcion as Empresa,");
            queryBajas.AppendLine("TablaTIMInventario.TipoMovimiento as [Tipo de Movimiento],");
            queryBajas.AppendLine("TablaTIMInventario.LadaTelefono,");
            queryBajas.AppendLine("TablaTIMInventario.ClaveCargo,");
            queryBajas.AppendLine("TablaTIMInventario.Cuenta,");
            queryBajas.AppendLine("TablaTIMInventario.Subcuenta,");
            queryBajas.AppendLine("TablaTIMInventario.FechaFactura");
            queryBajas.AppendLine("FROM " + DSODataContext.Schema + ".TIMResumenMovimientosInventario as TablaTIMInventario");
            queryBajas.AppendLine(" join " + DSODataContext.Schema + ".HistCarrier as Empresas");
            queryBajas.AppendLine("ON TablaTIMInventario.iCodCatCarrier = Empresas.iCodCatalogo");
            queryBajas.AppendLine("AND Empresas.dtIniVigencia<> Empresas.dtFinVigencia");
            queryBajas.AppendLine("AND Empresas.dtFinVigencia >= GETDATE()");
            queryBajas.AppendLine("AND TipoMovimiento= 'Baja'");

            if (ICodCarga != "0")
            {
                queryBajas.AppendLine(" WHERE FechaFactura = '" + ICodCarga + "' ");
            }
            return queryBajas.ToString();
        }

        private string ConsultaTablaResultadoCargaUpdateAltaToBaja(string ICodCarga)
        {

            StringBuilder queryUpdateAltaToBaja = new StringBuilder();

            queryUpdateAltaToBaja.AppendLine("SELECT ");
            queryUpdateAltaToBaja.AppendLine("TablaTIMInventario.iCodCatCarga,");
            queryUpdateAltaToBaja.AppendLine("Empresas.vchDescripcion as Empresa,");
            queryUpdateAltaToBaja.AppendLine("TablaTIMInventario.TipoMovimiento as [Tipo de Movimiento],");
            queryUpdateAltaToBaja.AppendLine("TablaTIMInventario.LadaTelefono,");
            queryUpdateAltaToBaja.AppendLine("TablaTIMInventario.ClaveCargo,");
            queryUpdateAltaToBaja.AppendLine("TablaTIMInventario.Cuenta,");
            queryUpdateAltaToBaja.AppendLine("TablaTIMInventario.Subcuenta,");
            queryUpdateAltaToBaja.AppendLine("TablaTIMInventario.FechaFactura");
            queryUpdateAltaToBaja.AppendLine("FROM " + DSODataContext.Schema + ".TIMResumenMovimientosInventario as TablaTIMInventario");
            queryUpdateAltaToBaja.AppendLine(" join " + DSODataContext.Schema + ".HistCarrier as Empresas");
            queryUpdateAltaToBaja.AppendLine("ON TablaTIMInventario.iCodCatCarrier = Empresas.iCodCatalogo");
            queryUpdateAltaToBaja.AppendLine("AND Empresas.dtIniVigencia<> Empresas.dtFinVigencia");
            queryUpdateAltaToBaja.AppendLine("AND Empresas.dtFinVigencia >= GETDATE()");
            queryUpdateAltaToBaja.AppendLine("AND TipoMovimiento= 'UpdateAltaToBaja'");

            if (ICodCarga != "0")
            {
                queryUpdateAltaToBaja.AppendLine(" WHERE FechaFactura = '" + ICodCarga + "' ");
            }
            return queryUpdateAltaToBaja.ToString();
        }

        private string ConsultaTablaResultadoCargaUpdateCuenta(string ICodCarga)
        {

            StringBuilder queryUpdateCuenta = new StringBuilder();

            queryUpdateCuenta.AppendLine("SELECT ");
            queryUpdateCuenta.AppendLine("TablaTIMInventario.iCodCatCarga,");
            queryUpdateCuenta.AppendLine("Empresas.vchDescripcion as Empresa,");
            queryUpdateCuenta.AppendLine("TablaTIMInventario.TipoMovimiento as [Tipo de Movimiento],");
            queryUpdateCuenta.AppendLine("TablaTIMInventario.LadaTelefono,");
            queryUpdateCuenta.AppendLine("TablaTIMInventario.ClaveCargo,");
            queryUpdateCuenta.AppendLine("TablaTIMInventario.Cuenta,");
            queryUpdateCuenta.AppendLine("TablaTIMInventario.Subcuenta,");
            queryUpdateCuenta.AppendLine("TablaTIMInventario.FechaFactura");
            queryUpdateCuenta.AppendLine("FROM " + DSODataContext.Schema + ".TIMResumenMovimientosInventario as TablaTIMInventario");
            queryUpdateCuenta.AppendLine(" join " + DSODataContext.Schema + ".HistCarrier as Empresas");
            queryUpdateCuenta.AppendLine("ON TablaTIMInventario.iCodCatCarrier = Empresas.iCodCatalogo");
            queryUpdateCuenta.AppendLine("AND Empresas.dtIniVigencia<> Empresas.dtFinVigencia");
            queryUpdateCuenta.AppendLine("AND Empresas.dtFinVigencia >= GETDATE()");
            queryUpdateCuenta.AppendLine("AND TipoMovimiento= 'UpdateCuenta-Subcuenta'");

            if (ICodCarga != "0")
            {
                queryUpdateCuenta.AppendLine(" WHERE FechaFactura = '" + ICodCarga + "' ");
            }
            return queryUpdateCuenta.ToString();
        }

        private string ConsultaTablaResultadoCargaCobrosDobles(string ICodCarga)
        {

            StringBuilder query = new StringBuilder();

            query.AppendLine("SELECT ");
            query.AppendLine("TablaTIMInventario.iCodCatCarga,");
            query.AppendLine("Empresas.vchDescripcion as Empresa,");
            query.AppendLine("TablaTIMInventario.TipoMovimiento as [Tipo de Movimiento],");
            query.AppendLine("TablaTIMInventario.LadaTelefono,");
            query.AppendLine("TablaTIMInventario.ClaveCargo,");
            query.AppendLine("TablaTIMInventario.Cuenta,");
            query.AppendLine("TablaTIMInventario.Subcuenta,");
            query.AppendLine("TablaTIMInventario.FechaFactura");
            query.AppendLine("FROM " + DSODataContext.Schema + ".TIMResumenMovimientosInventario as TablaTIMInventario");
            query.AppendLine(" join " + DSODataContext.Schema + ".HistCarrier as Empresas");
            query.AppendLine("ON TablaTIMInventario.iCodCatCarrier = Empresas.iCodCatalogo");
            query.AppendLine("AND Empresas.dtIniVigencia<> Empresas.dtFinVigencia");
            query.AppendLine("AND Empresas.dtFinVigencia >= GETDATE()");
            query.AppendLine("AND TipoMovimiento= 'Cobros-Dobles'");

            if (ICodCarga != "0")
            {
                query.AppendLine(" WHERE FechaFactura = '" + ICodCarga + "' ");
            }
            return query.ToString();
        }

        //private void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    ConsultaTablaResultadoCargaAltas(DropDownList1.SelectedValue);
        //    throw new NotImplementedException();
        //}

        public void ConsultaTablaResultadoCargaTIMAltas(Control contenedor, string ICodCarga = "0")
        {

            string tituloReporte = "TIM Resultado Carga Altas";

            System.Data.DataTable dt = DSODataAccess.Execute(ConsultaTablaResultadoCargaAltas(ICodCarga));


            //ALTAS
            if (dt != null && dt.Rows.Count > 0)
            {
                int[] camposBoundField = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
                int[] camposNavegacion = new int[] { 0 };
                string[] formatoColumnas = new string[] { "", "", "", "", "", "", "", "" };

                GridView grid = DTIChartsAndControls.GridView("ReportePrincipalAltas", dt, false, "",
                      formatoColumnas, "", new string[] { "" }, 1,
                      camposNavegacion, camposBoundField, new int[] { }, 2);
                dt.AcceptChanges();
                contenedor.Controls.Clear();
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(grid, "ReportePrincipalAltas", tituloReporte, string.Empty));
            }
            else
            {
                System.Web.UI.WebControls.Label sinInfo = new System.Web.UI.WebControls.Label();
                sinInfo.Text = "No hay información por mostrar";
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "ReportePrincipalAltas", tituloReporte, string.Empty));
            }
        }

        public void ConsultaTablaResultadoCargaTIMBajas(Control contenedor, string ICodCarga = "0")
        {

            string tituloReporte2 = "TIM Resultado Carga Bajas";
            System.Data.DataTable dtBajas = DSODataAccess.Execute(ConsultaTablaResultadoCargaBajas(ICodCarga));

            //BAJAS
            if (dtBajas != null && dtBajas.Rows.Count > 0)
            {
                int[] camposBoundField = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
                int[] camposNavegacion = new int[] { 0 };
                string[] formatoColumnas = new string[] { "", "", "", "", "", "", "", "" };

                GridView gridBajas = DTIChartsAndControls.GridView("ReportePrincipalBajas", dtBajas, false, "",
                      formatoColumnas, "", new string[] { "" }, 1,
                      camposNavegacion, camposBoundField, new int[] { }, 2);
                dtBajas.AcceptChanges();
                contenedor.Controls.Clear();
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(gridBajas, "ReportePrincipalBajas", tituloReporte2, string.Empty));
            }
            else
            {
                System.Web.UI.WebControls.Label sinInfo = new System.Web.UI.WebControls.Label();
                sinInfo.Text = "No hay información por mostrar";
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "ReportePrincipalBajas", tituloReporte2, string.Empty));
            }
        }

        public void ConsultaTablaResultadoCargaTIMUpdateAltasToBajas(Control contenedor, string ICodCarga = "0")
        {

            string tituloReporte3 = "TIM Resultado Carga Updates";
            System.Data.DataTable dtUpdateAltaToBaja = DSODataAccess.Execute(ConsultaTablaResultadoCargaUpdateAltaToBaja(ICodCarga));

            //UPDATEALTATOBAJA
            if (dtUpdateAltaToBaja != null && dtUpdateAltaToBaja.Rows.Count > 0)
            {
                int[] camposBoundField = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
                int[] camposNavegacion = new int[] { 0 };
                string[] formatoColumnas = new string[] { "", "", "", "", "", "", "", "" };

                GridView gridUpdateAltaToBaja = DTIChartsAndControls.GridView("ReportePrincipalUpdate01", dtUpdateAltaToBaja, false, "",
                      formatoColumnas, "", new string[] { "" }, 1,
                      camposNavegacion, camposBoundField, new int[] { }, 2);
                dtUpdateAltaToBaja.AcceptChanges();
                contenedor.Controls.Clear();
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(gridUpdateAltaToBaja, "ReportePrincipalUpdate01", tituloReporte3, string.Empty));
            }
            else
            {
                System.Web.UI.WebControls.Label sinInfo = new System.Web.UI.WebControls.Label();
                sinInfo.Text = "No hay información por mostrar";
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "ReportePrincipalUpdate01", tituloReporte3, string.Empty));
            }
        }


        public void ConsultaTablaResultadoCargaTIMUpdateCuenta(Control contenedor, string ICodCarga = "0")
        {
            string tituloReporte4 = "TIM Resultado Carga Updates Cuenta";
            System.Data.DataTable dtUpdateCuenta = DSODataAccess.Execute(ConsultaTablaResultadoCargaUpdateCuenta(ICodCarga));

            //UPDATECUENTAS
            if (dtUpdateCuenta != null && dtUpdateCuenta.Rows.Count > 0)
            {
                int[] camposBoundField = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
                int[] camposNavegacion = new int[] { 0 };
                string[] formatoColumnas = new string[] { "", "", "", "", "", "", "", "" };

                GridView gridUpdateCuenta = DTIChartsAndControls.GridView("ReportePrincipalUpdateCuenta", dtUpdateCuenta, false, "",
                      formatoColumnas, "", new string[] { "" }, 1,
                      camposNavegacion, camposBoundField, new int[] { }, 2);
                dtUpdateCuenta.AcceptChanges();
                contenedor.Controls.Clear();
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(gridUpdateCuenta, "ReportePrincipalUpdateCuenta", tituloReporte4, string.Empty));
            }
            else
            {
                System.Web.UI.WebControls.Label sinInfo = new System.Web.UI.WebControls.Label();
                sinInfo.Text = "No hay información por mostrar";
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "ReportePrincipalUpdateCuenta", tituloReporte4, string.Empty));
            }

        }

        public void ConsultaTablaResultadoCargaTIMCobrosDobles(Control contenedor, string ICodCarga = "0")
        {

            string tituloReporte5 = "TIM Resultado Carga Cobros Dobles";

            System.Data.DataTable dtCobrosDobles = DSODataAccess.Execute(ConsultaTablaResultadoCargaCobrosDobles(ICodCarga));


            //ALTAS
            if (dtCobrosDobles != null && dtCobrosDobles.Rows.Count > 0)
            {
                int[] camposBoundField = new int[] { 1, 2, 3, 4, 5, 6, 7, 8 };
                int[] camposNavegacion = new int[] { 0 };
                string[] formatoColumnas = new string[] { "", "", "", "", "", "", "", "" };

                GridView grid = DTIChartsAndControls.GridView("ReportePrincipalCobrosDobles", dtCobrosDobles, false, "",
                      formatoColumnas, "", new string[] { "" }, 1,
                      camposNavegacion, camposBoundField, new int[] { }, 2);
                dtCobrosDobles.AcceptChanges();
                contenedor.Controls.Clear();
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(grid, "ReportePrincipalCobrosDobles", tituloReporte5, string.Empty));
            }
            else
            {
                System.Web.UI.WebControls.Label sinInfo = new System.Web.UI.WebControls.Label();
                sinInfo.Text = "No hay información por mostrar";
                contenedor.Controls.Add(DTIChartsAndControls.TituloYBordeRepSencilloTabla(sinInfo, "ReportePrincipalCobrosDobles", tituloReporte5, string.Empty));
            }
        }

        protected void DropDownList1_SelectedIndexChanged1(object sender, EventArgs e)
        {
            ConsultaTablaResultadoCargaTIMAltas(Rep1, DropDownList1.SelectedValue);
            ConsultaTablaResultadoCargaTIMBajas(Rep2, DropDownList1.SelectedValue);
            ConsultaTablaResultadoCargaTIMUpdateAltasToBajas(Rep3, DropDownList1.SelectedValue);
            ConsultaTablaResultadoCargaTIMUpdateCuenta(Rep4, DropDownList1.SelectedValue);
            ConsultaTablaResultadoCargaTIMCobrosDobles(Rep5, DropDownList1.SelectedValue); 
        }
    }
}
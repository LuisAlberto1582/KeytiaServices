using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardFC;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.EtiquetaNums
{
    public partial class RevisionLlamEmple : System.Web.UI.Page
    {
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        int iCodUsuario;
        private string fecIni;
        private string fecFin;
        private string fechaint;
        private string fechaIntCel;
        private string var_cuentaconFAC;
        int numLLam = 0;
        int sumT = 0;
        decimal sumC = 0;
        decimal sumPer = 0;
        decimal sumPersonal = 0;
        decimal sumNeg = 0;
        int sumTP = 0;
        int numLLam2 = 0;
        List<SitiosEmple1> empleSitios1 = new List<SitiosEmple1>();
        List<ConsumoLlamLocales1> consumo1 = new List<ConsumoLlamLocales1>();
        List<ConsumoLlamCobrar1> consumoCobrar1 = new List<ConsumoLlamCobrar1>();
        List<ConsumoCel1> consumoCelular1 = new List<ConsumoCel1>();
        protected void Page_Load(object sender, EventArgs e)
        {
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            iCodUsuario = Convert.ToInt32(Session["iCodUsuario"]);
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            if (!Page.IsPostBack)
            {
                IniciaProceso();
            }
        }
        private void IniciaProceso()
        {
            cboAnio.DataSource = GetDataDropDownList("ANIO").DefaultView;
            cboAnio.DataBind();
            cboMes.DataSource = GetDataDropDownList("MES").DefaultView;
            cboMes.DataBind();
        }
        #region Consultas
        private DataTable GetDataDropDownList(string clave)
        {
            StringBuilder query = new StringBuilder();
           // lblMensaje.Text = "";
            bool isEstatus = false;
            query.Length = 0;
            query.AppendLine("SELECT [CAMPOS]");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[NOMVISTA]");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            query.AppendLine("	AND dtFinVigencia >= GETDATE()");

            #region Filtro
            switch (clave.ToUpper())
            {

                case "ANIO":
                    query = query.Replace("[CAMPOS]", "vchCodigo, vchDescripcion AS Descripcion");
                    query = query.Replace("[NOMVISTA]", "[VisHistoricos('Anio','Años','Español')]");
                    query.AppendLine(" AND CONVERT(INT, vchDescripcion) >= 2017 AND CONVERT(INT, vchDescripcion) <= YEAR(GETDATE())");
                    return AddRowDefault(DSODataAccess.Execute(query.ToString()), isEstatus);
                case "MES":
                    query = query.Replace("[CAMPOS]", "vchCodigo, Español AS Descripcion");
                    query = query.Replace("[NOMVISTA]", "[VisHistoricos('Mes','Meses','Español')]");
                    return AddRowDefault(DSODataAccess.Execute(query.ToString()), isEstatus);
                default:
                    return new DataTable();
            }

            #endregion
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
        public void ObtieneAnio()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT iCodCatalogo, vchDescripcion FROM "+esquema+".[VisHistoricos('Anio','Años','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND vchDescripcion IN(DATEPART(YEAR, GETDATE()),DATEPART(YEAR, DATEADD(YEAR, -1, GETDATE())), DATEPART(YEAR, DATEADD(YEAR, -2, GETDATE())))");
            query.AppendLine(" ORDER BY vchDescripcion DESC");
            DataTable dt = DSODataAccess.Execute(query.ToString(),connStr);
            if(dt != null && dt.Rows.Count > 0)
            {
                cboAnio.DataSource = dt;
                cboAnio.DataBind();
            }
        }
        [WebMethod]
        public static object GetEmploye(string texto)
        {
            DataTable Emple = new DataTable();
            string connStr = DSODataContext.ConnectionString;
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT TOP 100 iCodCatalogo,NominaA +' - '+ NomCompleto AS Nomcompleto FROM FCA.HistEmple WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND NomCompleto LIKE '%" + texto + "%' OR NominaA LIKE '%" + texto + "%'");
            Emple = DSODataAccess.Execute(query.ToString(), connStr);
            DataView dvldt = new DataView(Emple);
            Emple = dvldt.ToTable(false, new string[] { "NomCompleto", "iCodCatalogo" });
            Emple.Columns["NomCompleto"].ColumnName = "Nombre";
            Emple.Columns["iCodCatalogo"].ColumnName = "Clave";
            return FCAndControls.ConvertDataTabletoJSONString(Emple);
        }
        private void ObtieneConsumoCel1(int emid, ref List<ConsumoCel1> consumoCelular1,string fechaIntCel, int tipoBusqueda)
        {

            //string sp = "EXEC FCAObtieneDetallCelular @Esquema = '{0}',@Emple = {1},@FechaInt = {2},@Opcion={3}";
            //string query = string.Format(sp, esquema, emid, fechaIntCel, 1);
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" TelDest AS de_nummarcado,");
            query.AppendLine(" Detall.GEtiqueta AS tl_idf,");
            query.AppendLine(" Bloqueada AS de_etiqueta,");
            query.AppendLine(" TD.DescripcionGeneral AS de_tipodestino,");
            query.AppendLine(" Telefono AS de_extension,");
            query.AppendLine(" ISNULL(LocalidadUsuario,'')  AS de_localidad,");
            query.AppendLine(" SUM(CostoFac / TipoCambio) AS de_costo,");
            query.AppendLine(" SUM(DuracionMin) AS de_duracion,");
            if (tipoBusqueda == 1)
            {
                query.AppendLine(" COUNT(*) AS numero,");
            }
            else
            {
                query.AppendLine(" CONVERT(VARCHAR(11),FechaInicio,103) AS numero,");
            }
            query.AppendLine(" LocalidadKeytia AS de_localidadkeytia");
            query.AppendLine(" FROM "+ esquema + ".FCADetalleTelefoniaMovil AS Detall WITH(NOLOCK)");
            query.AppendLine(" JOIN " + esquema + ".HistTdest AS TD WITH(NOLOCK)");
            //query.AppendLine(" JOIN " + esquema + ".[vishistoricos('TDest','Tipo de destino','Español')] AS TD WITH(NOLOCK)");
            query.AppendLine(" ON Detall.TDest = TD.iCodCatalogo");
            query.AppendLine(" AND TD.dtIniVigencia <> TD.dtFinVigencia");
            query.AppendLine(" AND TD.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE Emple = "+ emid + "");
            query.AppendLine(" AND FechaInt = "+ fechaIntCel + "");
            query.AppendLine(" AND TD.vchCodigo NOT IN('001800', 'Local','EnlTie')");
            query.AppendLine(" AND LocalidadUsuario NOT LIKE '%MEN%2%VIA%'");
            query.AppendLine(" GROUP BY TelDest, Detall.GEtiqueta, Bloqueada, TD.DescripcionGeneral, Telefono, ISNULL(LocalidadUsuario,''), LocalidadKeytia");
            if (tipoBusqueda == 2)
            {
                query.AppendLine(", FechaInicio");
            }
            query.AppendLine(" UNION ALL");
            query.AppendLine(" SELECT");
            query.AppendLine(" TelDest AS de_nummarcado,");
            query.AppendLine(" Detall.GEtiqueta  AS tl_idf,");
            query.AppendLine(" Bloqueada AS de_etiqueta,");
            query.AppendLine(" TD.DescripcionGeneral AS de_tipodestino,");
            query.AppendLine(" Telefono AS de_extension,");
            query.AppendLine(" ISNULL(LocalidadUsuario,'') AS de_localidad,");
            query.AppendLine(" SUM(CostoFac / TipoCambio) AS de_costo,");
            query.AppendLine(" SUM(DuracionMin) AS de_duracion,");
            if (tipoBusqueda == 1)
            {
                query.AppendLine(" COUNT(*) AS numero,");
            }
            else
            {
                query.AppendLine(" CONVERT(VARCHAR(11),FechaInicio,103) AS numero,");
            }

            query.AppendLine(" LocalidadKeytia AS de_localidadkeytia");
            query.AppendLine(" FROM " + esquema + ".FCADetalleTelefoniaMovil AS Detall WITH(NOLOCK)");
            query.AppendLine(" JOIN " + esquema + ".HistTdest AS TD WITH(NOLOCK)");
            //query.AppendLine(" JOIN " + esquema + ".[vishistoricos('TDest','Tipo de destino','Español')] AS TD WITH(NOLOCK)");
            query.AppendLine(" ON Detall.TDest = TD.iCodCatalogo");
            query.AppendLine(" AND TD.dtIniVigencia <> TD.dtFinVigencia");
            query.AppendLine(" AND TD.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE Emple ="+ emid + "");
            query.AppendLine(" AND FechaInt = "+ fechaIntCel + "");
            query.AppendLine(" AND TD.vchCodigo NOT IN('001800','Local','EnlTie')");
            query.AppendLine(" AND LocalidadUsuario LIKE '%MEN%2%VIA%'");
            query.AppendLine(" GROUP BY TelDest,Detall.GEtiqueta,Bloqueada,TD.DescripcionGeneral,Telefono,ISNULL(LocalidadUsuario,''),LocalidadKeytia");
            if (tipoBusqueda == 2)
            {
                query.AppendLine(", FechaInicio");
            }
            query.AppendLine(" ORDER BY SUM(CostoFac/TipoCambio) DESC");
 
             DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ConsumoCel1 c1 = new ConsumoCel1();
                    c1.Tabla = "detalleempleadotelcel";
                    c1.CobroLlam = "";
                    //c.CodigoAut = dr["de_codigoaut"].ToString();/*en k5 ya no se utiliza*/
                    //c.ExtId = Convert.ToInt32(dr["de_ex_id"]);/* en K5 Ya no se utiliza*/
                    c1.NumMarcado = dr["de_nummarcado"].ToString();
                    c1.Idf = Convert.ToInt32(dr["tl_idf"]);
                    c1.Etiqueta = Convert.ToInt32(dr["de_etiqueta"]);
                    c1.TipoDestino = dr["de_tipodestino"].ToString();
                    c1.Extension = dr["de_extension"].ToString();
                    c1.Localidad = dr["de_localidad"].ToString();
                    c1.Costo = Convert.ToDecimal(dr["de_costo"]);
                    c1.Duracion = Convert.ToInt32(dr["de_duracion"]);
                    c1.Numero = dr["numero"].ToString();
                    c1.LocalidadKeytia = dr["de_localidadkeytia"].ToString();
                    consumoCelular1.Add(c1);
                }
            }

        }
        private void ObtieneConsumoCel2(int emid, ref List<ConsumoCel1> consumoCelular1,string fechaIntCel)
        {
            string sp = "EXEC FCAObtieneDetallCelular @Esquema = '{0}',@Emple = {1},@FechaInt = {2},@Opcion={3}";
            string query = string.Format(sp, esquema, emid, fechaIntCel, 2);
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ConsumoCel1 c1 = new ConsumoCel1();
                    c1.Tabla = "detalleempleadotelcel";
                    //c.CodigoAut = dr["de_codigoaut"].ToString();
                    //c.ExtId = Convert.ToInt32(dr["de_ex_id"]);
                    c1.NumMarcado = dr["de_nummarcado"].ToString();
                    c1.Idf = Convert.ToInt32(dr["tl_idf"]);
                    c1.Etiqueta = Convert.ToInt32(dr["de_etiqueta"]);
                    c1.TipoDestino = dr["de_tipodestino"].ToString();
                    c1.Extension = dr["de_extension"].ToString();
                    c1.Localidad = dr["de_localidad"].ToString();
                    c1.Costo = Convert.ToDecimal(dr["de_costo"]);
                    c1.Duracion = Convert.ToInt32(dr["de_duracion"]);
                    c1.Numero = dr["numero"].ToString();
                    c1.LocalidadKeytia = dr["de_localidadkeytia"].ToString();
                    consumoCelular1.Add(c1);
                }

            }
        }
        private DataTable OtieneDetallLlamLocales(int fecini, int fecFin, int emId,int tipoBusqueda)
        {
            DataTable dt = new DataTable();

            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" S.iCodCatalogo AS Sitio,");
            query.AppendLine(" Extension,");
            query.AppendLine(" TelDest AS NumMarcado,");
            if(tipoBusqueda == 1)
            {
                query.AppendLine(" COUNT(*) AS Cantidad,");
            }
            else
            {
                query.AppendLine(" CONVERT(VARCHAR(11),FechaInicio,103) AS Cantidad,");
            }

            query.AppendLine(" SUM(DuracionMin) AS Duracion,");
            query.AppendLine(" ISNULL(Etiqueta, '') AS Etiqueta,");
            query.AppendLine(" ISNULL(D.LocaliDesc, '') AS Localidad,");
            query.AppendLine(" SUM(Costo + CostoSM) AS Importe");
            query.AppendLine(" FROM " + esquema + ".[visDetallados('Detall','DetalleCDR','Español')] AS D WITH(NOLOCK)");
            query.AppendLine(" JOIN " + esquema + ".HistSitio AS S WITH (NOLOCK)");
            //query.AppendLine(" JOIN " + esquema + ".[VisHistoricos('Sitio','Sitio - Siemens','Español')] AS S");
            query.AppendLine(" ON D.Sitio = S.iCodCatalogo");
            query.AppendLine(" AND S.dtIniVigencia <> S.dtFinVigencia");
            query.AppendLine(" AND S.dtFinVigencia >= GETDATE()");
            query.AppendLine(" JOIN " + esquema + ".HistTdest AS T WITH(NOLOCK)");
            //query.AppendLine(" JOIN " + esquema + ".[vishistoricos('TDest','Tipo de destino','Español')] AS T");
            query.AppendLine(" ON D.TDest = T.iCodCatalogo");
            query.AppendLine(" AND T.dtIniVigencia<> T.dtFinVigencia");
            query.AppendLine(" AND T.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE CONVERT(VARCHAR(8),FechaInicio,112) >= '" + fecIni + "' AND CONVERT(VARCHAR(8),FechaInicio,112) <= '" + fecFin + "'");/*variables de fechas*/
            query.AppendLine(" AND D.Emple = " + emId + "");
            query.AppendLine(" AND(S.BanderasSitio & 64) / 64 = 0");
            query.AppendLine(" AND T.vchCodigo IN('001800', 'Local','EnlTie')");
            //query.AppendLine(" AND TDestCod IN('001800','Local')");
            query.AppendLine("  GROUP BY S.iCodCatalogo,D.Extension,D.TelDest,ISNULL(D.LocaliDesc,''), ISNULL(Etiqueta,'')");
            if(tipoBusqueda == 2)
            {
                query.AppendLine(", FechaInicio");
            }

            /*query.AppendLine(" HAVING SUM(Costo + CostoSM) > 0");*/
            return dt = DSODataAccess.Execute(query.ToString(), connStr);

        }
        private DataTable ObtieneDetallLlamLocalesGlobales(string tabla, int conmutadorID, int fecini, int fecFin, int emId, int cobroLlamadasLocales)
        {
            DataTable dt = new DataTable();

            StringBuilder query = new StringBuilder();

            query.AppendLine(" SELECT");
            query.AppendLine(" Extension AS de_extension,");
            query.AppendLine(" DuracionMin AS de_duracion,");
            query.AppendLine(" TelDest AS de_nummarcado,");
            query.AppendLine(" ISNULL(LocaliDesc,'') AS de_localidadKeytia,");
            query.AppendLine(" ROUND(SUM(Costo + CostoSM), 1) AS de_costo");
            query.AppendLine(" FROM " + esquema + ".[visDetallados('Detall','DetalleCDR','Español')] WITH(NOLOCK)");
            query.AppendLine(" WHERE Emple = " + emId + "");
            query.AppendLine(" AND CONVERT(VARCHAR(8),FechaInicio,112) >= '" + fecini + "'");
            query.AppendLine(" AND CONVERT(VARCHAR(8),FechaInicio,112) <= '" + fecFin + "'");
            query.AppendLine(" AND Sitio = " + conmutadorID + "");

            if (cobroLlamadasLocales != 1)
            {
                query.AppendLine(" AND TDestCod NOT IN ('001800','Local')");
            }
            query.AppendLine(" GROUP BY Extension,DuracionMin,TelDest,ISNULL(LocaliDesc,'')");
            query.AppendLine(" HAVING SUM(Costo + CostoSM) > 0");

            return dt = DSODataAccess.Execute(query.ToString(), connStr);
        }
        private DataTable ObtieneDetallCobrar(string tabla, int conmutadorID, int fecIni, int fecFin, int emId, int cobroLlamLoc,int tipoBusqueda)
        {
            /* '20140314.RJ.SEGUN LO PLATICADO CON LOURDES GARCIA, SOLO SE DEBEN COBRAR LAS LLAMADAS LOCALES CUANDO �STAS 
             'SE HAYAN REALIZADO DESDE UNO DE LOS CONMUTADORES DE SALTILLO, SIN IMPORTAR A D�NDE PERTENEZCA EL EMPLEADO
             'QUE LAS REALIZ� (ESTA CONDICI�N SOBRE-ESCRIBE LAS CONDICIONES ANTERIORES)*/
            StringBuilder query = new StringBuilder();
           
            query.AppendLine(" SELECT");
            query.AppendLine(" S.iCodCatalogo AS Sitio,");
            query.AppendLine(" Extension,");
            query.AppendLine(" TelDest AS NumMarcado,");
            query.AppendLine(" D.GEtiqueta AS TipoEtiqueta,");
            query.AppendLine(" ISNULL(D.AnchoDeBanda,1) AS De_Etiqueta,");
            query.AppendLine(" D.TDest,");
            if (tipoBusqueda == 1)
            {
                query.AppendLine(" COUNT(*) AS Cantidad,");
            }
            else
            {
                query.AppendLine(" CONVERT(VARCHAR(11),FechaInicio,103) AS Cantidad,");
            }
            query.AppendLine(" SUM(DuracionMin)AS Duracion,");
            query.AppendLine(" ISNULL(Etiqueta,'') AS Etiqueta,");
            query.AppendLine(" ISNULL(D.LocaliDesc,'')AS Localidad,");
            query.AppendLine(" SUM(Costo + CostoSM) AS Importe");
            query.AppendLine(" FROM " + esquema + ".[visDetallados('Detall','DetalleCDR','Español')] AS D WITH(NOLOCK)");
            query.AppendLine(" JOIN " + esquema + ".HistSitio AS S WITH (NOLOCK)");
            //query.AppendLine(" JOIN " + esquema + ".[VisHistoricos('Sitio','Sitio - Siemens','Español')] AS S");
            query.AppendLine(" ON D.Sitio = S.iCodCatalogo");
            query.AppendLine(" AND S.dtIniVigencia <> S.dtFinVigencia");
            query.AppendLine(" AND s.dtFinVigencia >= GETDATE()");
            query.AppendLine(" JOIN " + esquema + ".HistTdest AS T WITH(NOLOCK)");
            //query.AppendLine(" JOIN " + esquema + ".[vishistoricos('TDest','Tipo de destino','Español')] AS T");
            query.AppendLine(" ON D.TDest = T.iCodCatalogo");
            query.AppendLine(" AND T.dtIniVigencia<> T.dtFinVigencia");
            query.AppendLine(" AND T.dtFinVigencia >= GETDATE()");


            query.AppendLine(" WHERE CONVERT(VARCHAR(8),FechaInicio,112) >= '" + fecIni + "' AND CONVERT(VARCHAR(8),FechaInicio,112) <= '" + fecFin + "'");
            query.AppendLine(" AND D.Emple = " + emId + "");
            if (cobroLlamLoc == 0)
            {
                query.AppendLine(" AND T.vchCodigo NOT IN ('001800','Local','EnlTie')");
                //query.AppendLine("AND TDestCod NOT IN ('001800','Local')");
            }
            query.AppendLine(" AND D.Sitio = " + conmutadorID + "");
            query.AppendLine(" GROUP BY S.iCodCatalogo, Extension, TelDest, D.GEtiqueta, D.AnchoDeBanda, D.TDest, ISNULL(Etiqueta,''), ISNULL(D.LocaliDesc,'')");
            if (tipoBusqueda == 2)
            {
                query.AppendLine(", FechaInicio");
            }
            query.AppendLine(" HAVING SUM(Costo + CostoSM) > 0");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            return dt;

        }
        private int ValidaEmpleCuentaFac(int emId)
        {
            int cuenta = 0;
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT * FROM "+esquema+ ".[vishistoricos('CodAuto','Codigo Autorizacion','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND (BanderasCodAuto & 4)/4 = 0");
            query.AppendLine(" AND EMPLE = "+ emId + "");
            DataTable dt = DSODataAccess.Execute(query.ToString(),connStr);
            if(dt != null && dt.Rows.Count > 0)
            {
                cuenta = 1;
            }
            return cuenta;
        }
        #endregion Consultas
        #region Metodos
        private void ObtieneMesDisplay(string mes, string anioActual)
        {
            string mesDisplay = "";
            if (Session["mesDisplay"] != null) { Session["mesDisplay"] = null; }
            switch (mes)
            {
                case "01":
                    mesDisplay = "Enero de " + anioActual;
                    break;
                case "02":
                    mesDisplay = "Febrero de " + anioActual;
                    break;
                case "03":
                    mesDisplay = "Marzo de " + anioActual;
                    break;
                case "04":
                    mesDisplay = "Abril de " + anioActual;
                    break;
                case "05":
                    mesDisplay = "Mayo de " + anioActual;
                    break;
                case "06":
                    mesDisplay = "Junio de " + anioActual;
                    break;
                case "07":
                    mesDisplay = "Julio de " + anioActual;
                    break;
                case "08":
                    mesDisplay = "Agosto de " + anioActual;
                    break;
                case "09":
                    mesDisplay = "Septiembre de " + anioActual;
                    break;
                case "10":
                    mesDisplay = "Octubre de " + anioActual;
                    break;
                case "11":
                    mesDisplay = "Noviembre de " + anioActual;
                    break;
                case "12":
                    mesDisplay = "Diciembre de " + anioActual;
                    break;
            }

            Session["mesDisplay"] = mesDisplay;
        }
        private void ObtieneFechasConsulta(int mesSeleccionado,int anioSeleccionado)
        {
            int UltimoDia=0;
            string mesSel="0";
            string mesAnterior;
            int anioAnterior;
            switch(mesSeleccionado)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    UltimoDia = 31;
                    break;
                case 4:
                case 6:
                case 9:
                case 11:
                    UltimoDia = 30;
                    break;
                case 2:
                    int m = (anioSeleccionado % 4);
                    if(m == 0)
                    {
                        UltimoDia = 29;
                    }
                    else
                    {
                        UltimoDia = 28;
                    }

                    break;
            }
            
            if (mesSeleccionado.ToString().Length < 2)
            {
                mesSel = "0" + mesSeleccionado.ToString();
            }
            else
            {
                mesSel = mesSeleccionado.ToString();
            }

            fecIni = anioSeleccionado.ToString() + mesSel + "01";
            fecFin = anioSeleccionado.ToString() + mesSel + UltimoDia.ToString();

            fechaint = anioSeleccionado.ToString() + (Convert.ToInt32(mesSel) + 12).ToString();
            //if(mesSeleccionado == 1)
            //{
            //    mesAnterior = "12";
            //    anioAnterior = anioSeleccionado - 1;
            //    fechaIntCel = (anioSeleccionado - 1).ToString() + "24";
            //}
            //else
            //{
            //    mesAnterior = (mesSeleccionado - 1).ToString();
            //    anioAnterior = anioSeleccionado;
            //    fechaIntCel = anioSeleccionado.ToString() + ((mesSeleccionado)+12).ToString();
            //}
            fechaIntCel = anioSeleccionado.ToString() + ((mesSeleccionado) + 12).ToString();
            ObtieneMesDisplay(mesSel, anioSeleccionado.ToString());
        }
        private void ObtieneDatosEmpleado(int emId)
        {
            DataTable dt = ObtieneDatosEmple(emId);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                var numDepto = dr["vchCodigo"].ToString().Substring(4).Replace("-", "").Trim();
                txtNomEmple.Text = dr["NomCompleto"].ToString();//dr["em_apPaterno"].ToString() + " " + dr["em_apMaterno"].ToString() + " " + dr["em_nombre"].ToString();
                txtNumDepto.Text = numDepto;
                txtDepartamento.Text = dr["Descripcion"].ToString().ToUpper();//dr["cr_departamento"].ToString();
                var externo = dr["TipoEmCod"].ToString();//dr["em_recursos"].ToString();
                //Si el empleado es Externo Buscamos su Centro de Costo
                //Los empleados solo tienen departamemto
                if (externo == "X")
                {
                    txtNumEmpleado.Text = "00000";
                    rowCencos.Visible = true;
                    rowIdCencos.Visible = true;
                    //rowLocalidad.Visible = false;
                    //rowNumLocali.Visible = false;
                    ObtieneDatosExterno(emId);
                }
                else
                {
                    rowCencos.Visible = false;
                    rowIdCencos.Visible = false;
                    rowLocalidad.Visible = true;
                    rowNumLocali.Visible = true;
                    ObtieneLocalidadEmple(dr["vchCodigo"].ToString().Substring(0, 4));
                    txtNumEmpleado.Text = dr["NominaA"].ToString();
                }
                ObtieneLocalidadEmple(dr["vchCodigo"].ToString().Substring(0, 4));
            }

        }
        private DataTable ObtieneDatosEmple(int emId)
        {
            /*MODFICAR LA CONSULTA PARA QUE VALIDE CON EL HISTORICO DE USUARIOS*/
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" ISNULL(RFC,'') AS RFC,");
            query.AppendLine(" TE.vchCodigo AS TipoEmCod,");
            query.AppendLine(" NominaA,");
            query.AppendLine(" NomCompleto,");
            query.AppendLine(" C.vchCodigo,");
            query.AppendLine(" C.Descripcion");
            query.AppendLine(" FROM " + esquema + ".HistEmple AS E WITH(NOLOCK)");/*VARIABLE ESQUEMA*/
            query.AppendLine(" JOIN " + esquema + ".HistCenCos AS C WITH(NOLOCK)");
            //query.AppendLine(" FROM " + esquema + ".[vishistoricos('Emple','Empleados','Español')] AS E WITH(NOLOCK)");/*VARIABLE ESQUEMA*/
            //query.AppendLine(" JOIN " + esquema + ".[vishistoricos('CenCos','Centro de costos','Español')] AS C WITH(NOLOCK)");
            query.AppendLine(" ON E.CenCos = C.iCodCatalogo");
            query.AppendLine(" AND C.dtIniVigencia <> C.dtFinVigencia");
            query.AppendLine(" AND C.dtFinVigencia >= GETDATE()");
            query.AppendLine(" JOIN " + esquema + ".[VisHistoricos('TipoEm','Tipo Empleado','Español')]AS TE");
            query.AppendLine(" ON E.TipoEm = TE.iCodCatalogo");
            query.AppendLine(" AND TE.dtIniVigencia<> TE.dtFinVigencia");
            query.AppendLine(" AND TE.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE E.dtIniVigencia <> E.dtFinVigencia");
            query.AppendLine(" AND E.dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND E.iCodCatalogo = " + emId + "");/*VARIABLE EMPLEADO*/

            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            return dt;
        }
        private void ObtieneDatosExterno(int emId)
        {

            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT TOP 1");
            query.AppendLine(" CENCOSTO.vchCodigo,");
            query.AppendLine(" CENCOSTO.Descripcion");
            query.AppendLine(" FROM " + esquema + ".[visRelaciones('FCA CentroCosto-Externo','Español')] AS EXTERN WITH(NOLOCK)");
            query.AppendLine(" JOIN " + esquema + ".HistCenCos AS CENCOSTO WITH(NOLOCK)");
            //query.AppendLine(" JOIN " + esquema + ".[vishistoricos('CenCos','Centro de costos','Español')] AS CENCOSTO WITH(NOLOCK)");
            query.AppendLine(" ON EXTERN.CenCos = CENCOSTO.iCodCatalogo");
            query.AppendLine(" AND CENCOSTO.dtIniVigencia <> CENCOSTO.dtFinVigencia");
            query.AppendLine(" AND CENCOSTO.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE EXTERN.dtIniVigencia <> EXTERN.dtFinVigencia");
            query.AppendLine(" AND EXTERN.dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND EXTERN.Emple=" + emId + " ");
            //query.AppendLine(" ORDER BY FCANumeroLocalidad ASC ");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                txtCencos.Text = dr["Descripcion"].ToString();
                txtNumCencos.Text = dr["vchCodigo"].ToString();
            }
            else
            {
                txtCencos.Text = "N/A";
                txtNumCencos.Text = "N/A";
            }
        }
        private void ObtieneLocalidadEmple(string numDepto)
        {
            //SE OBTIENE LA LOCALIDAD CON EL ID MENOR QUE COINCIDA CON LOS PRIMEROS 4 DIGITOS DEL CENTRO DE COSTOS
            //SE HACE DE ESTA FORMA PUES PUEDE HABER VARIAS LOCALIDADES QUE CUMPLAN CON ESTA CONDICION

            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" iCodCatalogo,");
            query.AppendLine(" FCANumeroLocalidad,");
            query.AppendLine(" FCANombreLocalidad");
            query.AppendLine(" FROM " + esquema + ".[VisHistoricos('FCALocalidad','Localidades FCA','Español')] WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND SUBSTRING(FCANumeroLocalidad,0,5)= '" + numDepto + "'");
            query.AppendLine("ORDER BY FCANumeroLocalidad ASC");

            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                hfLocaliId.Value = dr["iCodCatalogo"].ToString();
                txtNumLocali.Text = dr["FCANumeroLocalidad"].ToString().Substring(0, 4);
                txtLocalidad.Text = dr["FCANombreLocalidad"].ToString();
            }
            else
            {
                hfLocaliId.Value = "0";
                txtLocalidad.Text = "N/A";
                txtNumLocali.Text = "N/A";
            }

        }
        private void ObtieneDatosLlamadas(int tipoBusqueda,int emId)
        {
            if(tipoBusqueda == 1)/*Resumen*/
            {
                GeneraDetalleLlam(emId, Convert.ToInt32(fecIni), Convert.ToInt32(fecFin), ref empleSitios1, ref consumo1, tipoBusqueda);
            }
            else if(tipoBusqueda == 2)/*Detalle*/
            {
                GeneraDetalleLlam(emId, Convert.ToInt32(fecIni), Convert.ToInt32(fecFin), ref empleSitios1, ref consumo1, tipoBusqueda);
            }
           
        }
        private void GeneraDetalleLlam(int emId, int fecini, int fecFin, ref List<SitiosEmple1> empleSitios1, ref List<ConsumoLlamLocales1> consumo1,int tipoBusqueda)
        {
            /*OBTIENE EL LISTADO DE SITIOS  */
            ObtieneSitiosEmple(ref empleSitios1);

            /*Genera tabla de detalle de llamadas locales*/
            ObtieneLlamLocales(emId, fecini, fecFin, ref empleSitios1, ref consumo1, tipoBusqueda);

            /*Genera tabla para el detalle de llamadas LDN,CELULAR Y TELENET*/
            ObtieneLlamadasaCobrar(emId, fecini, fecFin, ref empleSitios1, ref consumoCobrar1, tipoBusqueda);

            /*'*********************************
		        'APROBACION DE CELULARES
		        '*********************************
		        'Funciona muy similar a lo desplegado para las tablas de DetalleEmleado de los sitios, la variante es el query
		        'Busca directamente en la tabla de detalleempleadoTelcel
		        'El resto funciona igual,Obtenemos las llamadas de la tabla de DetalleEmpleadoTelcel
            */
            sumPer = 0;
            ObtieneLlamCelular(emId, fecini, fecFin, ref empleSitios1, ref consumoCelular1, fechaIntCel, tipoBusqueda);

            /*LLENA TOTALES GENERALES*/
            spnCelLd.InnerText = numLLam2.ToString();
            spnTiempoTot.InnerText = sumTP.ToString() + " minutos";
            /*importe total*/
            SpanTotal.InnerText = sumC.ToString("#0.00");

            ////Falta Realizar ciertas Validaciones para este dato importe personal aceptado
            spnTotalLlam.InnerText = sumPersonal.ToString("#0.00");
            ///*importe negocio*/
            SpanTotalNeg.InnerText = sumNeg.ToString("#0.00");

        }
        private void ObtieneSitiosEmple(ref List<SitiosEmple1> empleSitios1)
        {
            empleSitios1.Clear();
            consumo1.Clear();
            //fecini = 20190101;
            //fecFin = 20190131;
            //'QUERY PARA OBTENER EL NUMERO DE SITIOS EN LOS QUE TENGA CODIGO O EXTENSION EL EMPLEADO 
            //'La intenci�n de esto es saber en que tablas de detalle ha registrado consumo el empleado
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" iCodCatalogo AS Sitio,");
            query.AppendLine(" (BanderasSitio & 64) / 64 AS CobroLlamadasLocales");
            query.AppendLine(" FROM " + esquema + ".HistSitio WITH(NOLOCK)");
            //query.AppendLine(" FROM " + esquema + ".[VisHistoricos('Sitio','Sitio - Siemens','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");

            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    SitiosEmple1 sitio1 = new SitiosEmple1();
                    //sitio.Cencos = dr["cc_deteemp"].ToString();
                    sitio1.CencosId = Convert.ToInt32(dr["Sitio"]);
                    sitio1.CobroLlamLocales = Convert.ToInt32(dr["CobroLlamadasLocales"]);
                    empleSitios1.Add(sitio1);
                }
            }

        }
        private void ObtieneLlamLocales(int emId, int fecini, int fecFin, ref List<SitiosEmple1> empleSitios1, ref List<ConsumoLlamLocales1> consumo1,int tipoBusqueda)
        {
            //' Hacemos un ciclo que recorrer� todas las tablas a mostrar, 
            //' que son �nicamente aquellas en donde el usuario tenga extensi�n y/o c�digo
            //validar si existen sitios que se cobraran las llamadas locales 
            int totalLlamadas = 0;
            int totalMinutos = 0;
            decimal totalCosto = 0;
            /*OBTIENE LAS LLAMDAS LOCALES DEL DETALLE DE CDR CUANDO EL SITIO TIENE LA BANDERA APAGADA DE 
             COBRAR LLAMADAS LOCALES*/
            DataTable dt = OtieneDetallLlamLocales(fecini, fecFin, emId, tipoBusqueda);
            if (dt != null && dt.Rows.Count > 0)
            {
                row5.Visible = false;
                foreach (DataRow dr in dt.Rows)
                {
                    ConsumoLlamLocales1 con1 = new ConsumoLlamLocales1();
                    con1.TablaConsulta = Convert.ToInt32(dr["Sitio"]);
                    con1.Extension = dr["Extension"].ToString();
                    con1.NumMarcado = dr["NumMarcado"].ToString();
                    con1.Cantidad = dr["Cantidad"].ToString();
                    con1.Duracion = Convert.ToInt32(dr["Duracion"]);
                    con1.Localidad = string.IsNullOrEmpty(dr["Etiqueta"].ToString()) ? " " : dr["Etiqueta"].ToString().ToUpper();
                    con1.LocalidadKeytia = dr["Localidad"].ToString().ToUpper();
                    con1.Costo = Convert.ToDecimal(dr["Importe"]);
                    consumo1.Add(con1);

                    if(tipoBusqueda == 1 )
                    {
                        totalLlamadas += Convert.ToInt32(dr["Cantidad"]);
                    }
                    else
                    {
                        totalLlamadas += 1;
                    }
                    totalMinutos += Convert.ToInt32(dr["Duracion"]);
                    totalCosto += Convert.ToDecimal(dr["Importe"]);
                }
              
            }

            if (consumo1.Count > 0)/*si existen sitios con llamadas locales que no se cobraran*/
            {
                row5.Visible = true;
                row2.Visible = true;
                //row6.Visible = true;
                if (tipoBusqueda == 1)
                {
                    grdLlamLocales.Columns[2].HeaderText = "Cantidad";
                }
                else if (tipoBusqueda == 2)
                {
                    grdLlamLocales.Columns[2].HeaderText = "Fecha";
                }
                grdLlamLocales.DataSource = null;
                grdLlamLocales.DataBind();

                grdLlamLocales.DataSource = consumo1;
                grdLlamLocales.DataBind();
                grdLlamLocales.UseAccessibleHeader = true;
                grdLlamLocales.HeaderRow.TableSection = TableRowSection.TableHeader;
                if (consumoCobrar1.Count > 11)
                {
                    DivllamLocales.Style.Value = "height:210px;overflow-y:auto;overflow-x:auto;";
                }
                else
                {
                    DivllamLocales.Style.Value = "overflow-y:auto;overflow-x:auto;";
                }
                //'Validaci�n para que el despliegue de Totales no aplique a los Saltillo
                if (totalLlamadas > 0)
                {

                    List<TotalConsumo1> listConsumoTot1 = new List<TotalConsumo1>();
                    listConsumoTot1.Clear();
                    ObtieneTotLlam(totalLlamadas, totalMinutos, totalCosto, ref listConsumoTot1);
                    grdTotales.DataSource = listConsumoTot1;
                    grdTotales.DataBind();
                }

                string mesDisplay = Session["mesDisplay"].ToString();
                parrafo1.InnerText = "Detalle de llamadas locales - " + mesDisplay;
            }
            else
            {
                row2.Visible = false;
                row5.Visible = false;
                //row6.Visible = false;
            }
        }
        private void ObtieneLlamadasaCobrar(int emId, int fecini, int fecFin, ref List<SitiosEmple1> empleSitios1, ref List<ConsumoLlamCobrar1> consumoCobrar1, int tipoBusqueda)
        {
            consumoCobrar1.Clear();
            int totalLlamadas = 0;
            int totalMinutos = 0;
            decimal costoTotal = 0;
            foreach (var item in empleSitios1)
            {
                int conmutadorID = item.CencosId;
                string tabla = item.Cencos;
                int cobroLlamadasLocales = item.CobroLlamLocales;
                DataTable dt = ObtieneDetallCobrar(tabla, conmutadorID, fecini, fecFin, emId, cobroLlamadasLocales, tipoBusqueda);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        ConsumoLlamCobrar1 list1 = new ConsumoLlamCobrar1();
                        list1.CobroLlam = "";
                        list1.TablaConsulta = Convert.ToInt32(dr["Sitio"]);
                        list1.NumMarcado = dr["NumMarcado"].ToString();
                        list1.Idf = Convert.ToInt32(dr["TipoEtiqueta"]);/*Campo que define si una llamada es laboral o personal*/
                        list1.TipoEtiqueta = Convert.ToInt32(dr["De_Etiqueta"]);/*Campo para Validar si una llamada puede ser etiquetada o no*/
                        list1.TipoDestino = Convert.ToInt32(dr["TDest"]);
                        list1.Extension = dr["Extension"].ToString();
                        list1.Localidad = string.IsNullOrEmpty(dr["Etiqueta"].ToString()) ? " " : dr["Etiqueta"].ToString().ToUpper();
                        list1.Costo = Convert.ToDecimal(dr["Importe"]);
                        list1.Duracion = Convert.ToInt32(dr["Duracion"]);
                        list1.Cantidad = dr["Cantidad"].ToString();
                        list1.LocalidadKeytia = dr["Localidad"].ToString().ToUpper();
                        consumoCobrar1.Add(list1);

                      
                        totalMinutos += Convert.ToInt32(dr["Duracion"]);
                        costoTotal += Convert.ToDecimal(dr["Importe"]);

                        //Estos son contadores para totalizar las cuentas
                        numLLam += 1;//Numero de Llamadas Desplegadas(es el identificador de cada elemento)
                        if(tipoBusqueda == 1)
                        {
                            totalLlamadas += Convert.ToInt32(dr["Cantidad"]);
                            numLLam2 += Convert.ToInt32(dr["Cantidad"]);//Cantidad total de llamadas seg�n la agrupaci�n
                        }
                        else if (tipoBusqueda == 2)
                        {
                            totalLlamadas += 1;
                            numLLam2 += 1;
                        }

                        sumT += Convert.ToInt32(dr["Duracion"]);//Suma del tiempo total (Duraci�n)
                        sumTP += Convert.ToInt32(dr["Duracion"]);
                        sumC += Convert.ToDecimal(dr["Importe"]);

                        //Llevamos una suma para saber cuanto es el costo total a desplegar, sumamos el costo de la llamada segun el detalle
                        sumPer += Convert.ToDecimal(dr["Importe"]);

                    }
                }

                
            }

            if (consumoCobrar1.Count > 0)
            {
                if (consumoCobrar1.Count > 10)
                {
                    LlamadasDiv.Style.Value = "height:210px;overflow-y:auto;overflow-x:auto;";
                }
                else
                {
                    LlamadasDiv.Style.Value = "overflow-y:auto;overflow-x:auto;";
                }
                row5.Visible = true;
                //row6.Visible = true;
                if (tipoBusqueda == 1)
                {
                    grvLlamadas.Columns[3].HeaderText = "Cantidad";
                }
                else if (tipoBusqueda == 2)
                {
                    grvLlamadas.Columns[3].HeaderText = "Fecha";
                }
                grvLlamadas.DataSource = null;
                grvLlamadas.DataBind();

                grvLlamadas.DataSource = consumoCobrar1;
                grvLlamadas.DataBind();

                row3.Visible = true;
                string mesDisplay = Session["mesDisplay"].ToString();
                parrafo2.InnerText = "Detalle de llamadas de Larga Distancia y a Celular  - " + mesDisplay;

                /*Valida que registros de la tabla se van a deshabilitar el check box o checkear los mismos*/
                GridView grv = grvLlamadas;
                GridView grvTot = grdLlamCobrarTot;
                DatosGridview(grv, grvTot, totalLlamadas, totalMinutos, costoTotal);

            }
            else
            {
                row5.Visible = false;
                //row6.Visible = false;
                row3.Visible = false;
            }
        }
        private void ObtieneLlamCelular(int emId, int fecini, int fecFin, ref List<SitiosEmple1> empleSitios1, ref List<ConsumoCel1> consumoCelular1,string fechaIntCel, int tipoBusqueda)
        {
            consumoCelular1.Clear();
            ObtieneConsumoCel1(emId, ref consumoCelular1, fechaIntCel, tipoBusqueda);
            //ObtieneConsumoCel2(emId, ref consumoCelular1, fechaIntCel);

            if (consumoCelular1.Count > 0)
            {
                int totalLlamadas = 0;
                int totalMinutos = 0;
                decimal costoTotal = 0;

                if (tipoBusqueda == 1)
                {
                    grdMovil.Columns[3].HeaderText = "Cantidad";
                }
                else if (tipoBusqueda == 2)
                {
                    grdMovil.Columns[3].HeaderText = "Fecha";
                }

                foreach (var item in consumoCelular1)
                {
                    if (tipoBusqueda == 1)
                    {
                        totalLlamadas += Convert.ToInt32(item.Numero);
                        numLLam2 += Convert.ToInt32(item.Numero);
                    }
                    else if(tipoBusqueda == 2)
                    {
                        totalLlamadas += 1;
                        numLLam2 += 1;
                    }

                    totalMinutos += item.Duracion;
                    costoTotal += item.Costo;

                    numLLam += 1;
                   
                    sumT += item.Duracion;
                    sumTP += item.Duracion;
                    sumC += item.Costo;

                    sumPer += item.Costo;
                }

                row4.Visible = true;
                row5.Visible = true;
                string mesDisplay = Session["mesDisplay"].ToString();
                parrafo3.InnerText = " Detalle de llamadas de Telefonía Móvil" + " - " + mesDisplay;

                grdMovil.DataSource = consumoCelular1;
                grdMovil.DataBind();

                /*Valida que registros de la tabla se van a deshabilitar el check box o checkear los mismos*/
                GridView grv = grdMovil;
                GridView grvTot = grvTotalMovil;
                DatosGridview(grv, grvTot, totalLlamadas, totalMinutos, costoTotal);
            }
            else
            {
                row4.Visible = false;
            }
        }
        private void ObtieneTotLlam(int llamadas, int minutos, decimal costo, ref List<TotalConsumo1> listConsumoTot1)
        {
            TotalConsumo1 consTotalConsumo1 = new TotalConsumo1
            {
                Llamadas = "Total de llamadas: " + llamadas,
                Minutos = "Total de minutos: " + minutos,
                CostoTotal = "Total costo: " + "$" + costo.ToString("#0.00")
            };
            listConsumoTot1.Add(consTotalConsumo1);

        }
        private void DatosGridview(GridView grd, GridView grdTot, int totalLlamadas, int totalMinutos, decimal costoTotal)
        {
            grd.UseAccessibleHeader = true;
            grd.HeaderRow.TableSection = TableRowSection.TableHeader;


            List<TotalConsumo1> listConsumoTot1 = new List<TotalConsumo1>();
            listConsumoTot1.Clear();
            ObtieneTotLlam(totalLlamadas, totalMinutos, costoTotal, ref listConsumoTot1);
            grdTot.DataSource = listConsumoTot1;
            grdTot.DataBind();
            /*     
                 Esta l�nea despliega el Checkbox de cada llamada
                 El nombre inicia con PERSONAL y como identificador se le agrega el contador del Numero de llamadas desplegadas
                 El value es el Numero Marcado, y se valida el tipo de llamada que es para ponerlo checado o no
                 Asi mismo se valida que si viene etiquetada esa llamada entonces lo ponga deshabilitado (disabled)
                 Y tiene programado el evento OnClick para que al ser seleccionado ejecute la funci�n de Javascript ActualizaTotales()
              */
            for (int i = 0; i < grd.Rows.Count; i++)
            {
                var checkbox = grd.Rows[i].FindControl("chkRow") as System.Web.UI.WebControls.CheckBox;
                //var textBox = grd.Rows[i].FindControl("txtRferencia") as System.Web.UI.WebControls.TextBox;

                var fila = grd.Rows[i];
                int Idf = Convert.ToInt32(grd.DataKeys[i].Values[0]);
                int idEtiqueta = Convert.ToInt32(grd.DataKeys[i].Values[2]);
                decimal costo = Convert.ToDecimal(grd.DataKeys[i].Values[3]);

                string tipoLlam = "";
                if (Idf == 2)
                {
                    grd.Rows[i].Cells[0].Text = "X";
                    tipoLlam = "X";
                    //checkbox.Checked = true;
                }



                if (tipoLlam =="X")//(checkbox.Checked == true)
                {
                    //suma el costo de las llamadas de negocio
                    sumNeg += costo;
                }
                else
                {
                    //suma los totatales de las llamadas personales
                    sumPersonal += costo;
                }
            }

        }
      
        #endregion Metdos
        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            int anio = Convert.ToInt32(cboAnio.SelectedItem.ToString().Replace("TODOS","0"));
            int mes = Convert.ToInt32(cboMes.SelectedValue.ToString().Replace("TODOS","0"));
            if(anio > 0)
            {
                if(mes > 0)
                {
                    if(txtBusqueda.Text != "")
                    {
                        if(rbtnDetall.Checked == true || rbtResumen.Checked == true)
                        {
                            detalleEmpleados.Visible = true;
                            ObtieneFechasConsulta(mes, anio);
                            int icodEmple = Convert.ToInt32(txtEmpleId.Value);
                            ObtieneDatosEmpleado(icodEmple);
                            int tipoBusqueda = 0;
                            if(rbtResumen.Checked == true) { tipoBusqueda = 1; } else { tipoBusqueda = 2; }
                            /*MOSTRAR UN MENSAJE QUE SI CUENTA CON CLAVE FAC, FALTA REALIZAR LA VALIDACION DE ESTE MENSAJE*/
                            //int cuentaFac = ValidaEmpleCuentaFac(icodEmple);
                            //if(cuentaFac == 1)
                            //{
                            //    var_cuentaconFAC = "S";
                            //}
                            //else
                            //{
                            //    var_cuentaconFAC = "N";
                            //}

                            //if(var_cuentaconFAC == "S")
                            //{
                                ObtieneDatosLlamadas(tipoBusqueda, icodEmple);
                            //}
                            //else if(var_cuentaconFAC == "N")
                            //{
                            //    lblTituloModalMsn.Text = "¡Mensaje!";
                            //    lblBodyModalMsn.Text = "El empleado seleccionado no cuenta con clave FAC.";
                            //    mpeEtqMsn.Show();
                            //    row5.Visible = false;
                            //    row3.Visible = false;
                            //}

                            /*Muestra un mensaje en caso de que el empleado no tenga consumo en las fechas seleccionadas*/
                            if(consumoCobrar1.Count > 0 || consumoCelular1.Count > 0)
                            {
                                rowExportar.Visible = true;
                            }
                            else if(consumoCobrar1.Count <= 0 && consumoCelular1.Count <= 0)
                            {
                                lblTituloModalMsn.Text = "¡Mensaje!";
                                lblBodyModalMsn.Text = "El empleado Seleccionado, no genero consumo en el mes seleccionado.";
                                mpeEtqMsn.Show();
                                row3.Visible = false;
                                row4.Visible = false;
                                row5.Visible = false;
                            }
                        }
                        else
                        {
                            lblTituloModalMsn.Text = "¡Mensaje!";
                            lblBodyModalMsn.Text = "Debe de Seleccionar Un tipo de Reporte.";
                            mpeEtqMsn.Show();

                            return;
                        }
                    }
                    else
                    {
                        lblTituloModalMsn.Text = "¡Mensaje!";
                        lblBodyModalMsn.Text = "Debe de Seleccionar un empleado.";
                        mpeEtqMsn.Show();
                        return;
                    }
                }
                else
                {
                    lblTituloModalMsn.Text = "¡Mensaje!";
                    lblBodyModalMsn.Text = "Debe de Seleccionar el mes.";
                    mpeEtqMsn.Show();               
                    return;
                }
            }
            else
            {
                lblTituloModalMsn.Text = "¡Mensaje!";
                lblBodyModalMsn.Text = "Debe de Seleccionar el año.";
                mpeEtqMsn.Show();
                return;
            }
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            ExportaInfo export = new ExportaInfo();

            string[] datosE = new string[] { txtNomEmple.Text,txtDepartamento.Text, txtLocalidad.Text, txtCencos.Text, txtNumEmpleado.Text, txtNumDepto.Text, txtNumLocali.Text, txtNumCencos.Text};
            string[] totales = new string[] { spnCelLd.InnerText, spnTiempoTot.InnerText, SpanTotal.InnerText, spnTotalLlam.InnerText , SpanTotalNeg.InnerText };

            GridView grvLocal = grdLlamLocales;
            GridView grvTotLocal = grdTotales;
            GridView grvLlamCobrar = grvLlamadas;
            GridView grdTotCobrar = grdLlamCobrarTot;
            GridView grdMoviles = grdMovil;
            GridView grdTotMoviles = grvTotalMovil;
            string nameFile = "";
            if (rbtResumen.Checked == true) { nameFile = "ResumenConsumoPersonal"; } else { nameFile = "DetalleConsumoPersonal"; }         
            string file = export.GeneraArchivoExcel(nameFile, datosE, Session["mesDisplay"].ToString(), grvLocal, grvTotLocal, grvLlamCobrar, grdTotCobrar, grdMoviles, grdTotMoviles, totales,1);
            ExportFile(file);
        }

        public void ExportFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    HttpResponse response = HttpContext.Current.Response;
                    //response.Clear();
                    //response.Charset = "";
                    //response.ContentType = "application/octet-stream";
                    //response.AddHeader("Content-Disposition", "attachment;filename=\"" + Path.GetFileName(filePath) + "\"");
                    //response.TransmitFile(filePath);
                    ////response.End();
                    //HttpContext.Current.ApplicationInstance.CompleteRequest();

                    var buffer = File.ReadAllBytes(filePath);
                    Response.Clear();
                    Response.ClearHeaders();
                    Response.ClearContent();
                    Response.AddHeader("Content-Type", "application/octet-stream");
                    Response.AddHeader("Content-disposition", "attachment; filename=\"" + Path.GetFileName(filePath) + "\"");
                    Response.BinaryWrite(buffer);
                    Response.ContentType = "application/octet-stream";
                    Response.Flush();
                    //Response.Close();
                    buffer = null;
                }

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }
    }
    public class SitiosEmple1
    {
        public string Cencos { get; set; }
        public int CencosId { get; set; }
        public int CobroLlamLocales { get; set; }
    }
    public class ConsumoLlamLocales1
    {
        public int TablaConsulta { get; set; }
        public string NumMarcado { get; set; }
        public string Extension { get; set; }
        public string Localidad { get; set; }
        public decimal Costo { get; set; }
        public int Duracion { get; set; }
        public string Cantidad { get; set; }
        public string LocalidadKeytia { get; set; }
    }
    public class ConsumoLlamCobrar1
    {
        public string CobroLlam { get; set; }
        public int TablaConsulta { get; set; }
        public int Idf { get; set; }
        public int TipoEtiqueta { get; set; }
        public string NumMarcado { get; set; }
        public string Extension { get; set; }
        public string Localidad { get; set; }
        public int TipoDestino { get; set; }
        public decimal Costo { get; set; }
        public int Duracion { get; set; }
        public string Cantidad { get; set; }
        public string LocalidadKeytia { get; set; }
    }
    public class ConsumoCel1
    {
        public string CobroLlam { get; set; }
        public string Tabla { get; set; }
        public string CodigoAut { get; set; }
        public int ExtId { get; set; }
        public string NumMarcado { get; set; }
        public int Idf { get; set; }
        public int Etiqueta { get; set; }
        public string TipoDestino { get; set; }
        public string Extension { get; set; }
        public string Localidad { get; set; }      
        public decimal Costo { get; set; }
        public int Duracion { get; set; }
        public string Numero { get; set; }
        public string LocalidadKeytia { get; set; }
    }
    public class TotalConsumo1
    {
        public string Llamadas { get; set; }
        public string Minutos { get; set; }
        public string CostoTotal { get; set; }
    }
    public class DetalleLlamadas1
    {
        public string Extension { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string NumMarcado { get; set; }
        public int Duracion { get; set; }
        public string Localidad { get; set; }
        public decimal Costo { get; set; }
        public string Referencia { get; set; }
    }
}
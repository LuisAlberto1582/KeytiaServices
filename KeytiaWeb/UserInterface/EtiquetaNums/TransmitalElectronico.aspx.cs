using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.EtiquetaNums
{
    public partial class TransmitalElectronico : System.Web.UI.Page
    {
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        private string anioTransmital;
        private string fecIni;
        private string fecFin;
        private string fechaint;
        private string fechaIntCel;
        protected void Page_Load(object sender, EventArgs e)
        {
            ScriptManager.GetCurrent(Page).RegisterPostBackControl(lnkVerDetall);
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            if(!Page.IsPostBack)
            {
                IniciaProceso();
            }
        }
        #region METODOS
        private void IniciaProceso()
        {
            ObtieneSitos();
            cboAnio.DataSource = GetDataDropDownList("ANIO").DefaultView;
            cboAnio.DataBind();
            cboMes.DataSource = GetDataDropDownList("MES").DefaultView;
            cboMes.DataBind();
        }
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
        private void ObtieneFechasConsulta(int mesSeleccionado, int anioSeleccionado)
        {
            int UltimoDia = 0;
            string mesSel = "0";
            string mesAnterior;
            int anioAnterior;
            switch (mesSeleccionado)
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
                    if (m == 0)
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
            else {
                mesSel = mesSeleccionado.ToString();
            }

            fecIni = anioSeleccionado.ToString() + mesSel + "01";
            fecFin = anioSeleccionado.ToString() + mesSel + UltimoDia.ToString();
            anioTransmital = anioSeleccionado.ToString().Substring(2, 2);

            fechaint = anioSeleccionado.ToString() + (Convert.ToInt32(mesSel) + 12).ToString();
            //if (mesSeleccionado == 1)
            //{
            //    mesAnterior = "12";
            //    anioAnterior = anioSeleccionado - 1;
            //    fechaIntCel = (anioSeleccionado - 1).ToString() + "24";
            //}
            //else
            //{
            //    mesAnterior = (mesSeleccionado - 1).ToString();
            //    anioAnterior = anioSeleccionado;
            //    fechaIntCel = anioSeleccionado.ToString() + ((mesSeleccionado) + 12).ToString();
            //}
            fechaIntCel = anioSeleccionado.ToString() + ((mesSeleccionado) + 12).ToString();
            //ObtieneMesDisplay(mesSel, anioSeleccionado.ToString());
        }
        private void GeneraTransmital(string nomArchivo,string fechaTermina,string fechaInicio, string fechaFin,int sitio,int anio, int mes)
        {
            //lblMensajeInfo.Text = "Generando Transmital.";
            //pnlInfo.Visible = true;
            btnBuscar.Enabled = false;
            /*LLAMADAS DE SE ENCUENTRAN EN EL CDR*/
            DataTable dt = ObtieneConsumoSitio(fechaInicio,fechaFin,sitio);
            string pathArchivo = Server.MapPath("~/Transmital//");
            CreaRepositorio(pathArchivo);
            string archivoTransmital = pathArchivo + nomArchivo;
            StringBuilder linea = new StringBuilder();
            string nombreSitio = cboLocalidad.SelectedItem.ToString();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    decimal var_total_telcel = 0;
                    //var var_total_LD = 0;
                    //var Importe = 0;
                    var var_ubicacion_empleado = "";
                    int empleado = Convert.ToInt32(dr["iCodCatalogo"]);

                    /*  SE OBTIENE LA LOCALIDAD DEL EMPLEADO EN FUNCION DEL CENTRO DE COSTOS
                        CON EL QUE EST� RELACIONADO Y DEPENDIENDO DE LA LOCALIDAD*/
                    DataTable dt1 = ObtienLocalidadEmple(empleado);
                    if(dt1 != null && dt1.Rows.Count > 0)
                    {
                        DataRow dr2 = dt1.Rows[0];
                        var_ubicacion_empleado = dr2["Localidad"].ToString();
                    }

                    if(var_ubicacion_empleado.ToUpper().Trim() == nombreSitio.ToUpper().Trim() && empleado > 0)
                    {
                        /*obtiene el total de la tabla detalle Telcel*/
                        var totTelcel = ObtieneTotalTelcelEmple(sitio, empleado, fechaIntCel);
                        var_total_telcel += totTelcel;
                    }
                    else
                    {
                        var_total_telcel += 0;
                    }

                    string numPlanta = dr["Clave."].ToString();
                    string nomina = dr["NominaA"].ToString();
                    decimal importe = Convert.ToDecimal(dr["Importe"]);
                    string importeTotal = (importe + var_total_telcel).ToString("C");
                    importeTotal = importeTotal.Replace(".", "").Replace("$", "").Replace(",", "").Trim();
                    int caracteres = importeTotal.Length;/*Contamos la cantidad de caracteres del Importe ya sin puntos decimales*/
                    /*'El campo Importe del Transmital Electr�nico debe ser de 9 caracteres
                    'Si es menor a 9 debemos agregar ceros a la izquierda
                    'Entonces primero validamos que suceder�i si es mayor a 9 caracteres*/
                    string importeT;
                    if (caracteres > 9)
                    {
                        /*Si es mayor pues recortamos el importe a 9 caracteres*/
                        importeT = importeTotal.Substring(0,9);
                    }
                    else
                    {
                        /*'calculamos cuantos carcateres hay que agregar a la cadena Importe*/
                        int AgregaCaracteres = 9 - caracteres;
                        string d = new string('0', AgregaCaracteres);                      
                        importeT = d + importeTotal;
                    }
                    /*'Condicionamos que sean solo para empleados y las plantas que generan transmital*/
                    if (numPlanta != "" && nomina.Length == 5 && numPlanta!= "00")
                    {                       
                        string lineaArchivo = "N21"+ numPlanta+nomina+"2"+fechaTermina+"068"+ "       " + importeT + new string(' ', 90);
                        linea.AppendLine(lineaArchivo);
                    }
                }
            }

            /*falta la consulta de empleados telcel mes = 01,anio= 2019,fechaintcel= 201824*/
            DataTable dtMovil = ObtieneConsumoTelcel(mes,anio, fechaIntCel, sitio);
            if(dtMovil != null && dtMovil.Rows.Count > 0)
            {
                foreach (DataRow drMovil in dtMovil.Rows)
                {
                    decimal importeMovil = 0;
                    string nominaEmple = "";
                    string numPlantaMovil = "";
                    /*Falta llenar las variables de arriba*/
                    nominaEmple = drMovil["NominaA"].ToString();
                    numPlantaMovil = drMovil["Clave."].ToString();
                    importeMovil = Convert.ToDecimal(drMovil["Total"]);

                    string imp = importeMovil.ToString("C");
                    imp = imp.Replace(".", "").Replace("$", "").Replace(",", "").Trim();
                    int longitud = imp.Length;
                    string importeM;
                    if (longitud > 9)
                    {
                        /*Si es mayor pues recortamos el importe a 9 caracteres*/
                        importeM = imp.Substring(0, 9);
                    }
                    else
                    {
                        /*'calculamos cuantos carcateres hay que agregar a la cadena Importe*/
                        int AgregaCaracteres = 9 - longitud;
                        string m = new string('0', AgregaCaracteres);
                        importeM = m + imp;
                    }

                    if (numPlantaMovil != "" && nominaEmple.Length == 5 && numPlantaMovil != "00")
                    {
                        string lineaArchivo = "N21" + numPlantaMovil + nominaEmple + "2" + fechaTermina + "068" + "       " + importeM + new string(' ', 90);
                        //CreaArchivoTransmital(archivoTransmital, lineaArchivo);
                        linea.AppendLine(lineaArchivo);
                    }
                }
            }

            /*Crea el Archivo Transmital*/
            CreaArchivoTransmital(archivoTransmital, linea.ToString());
            /*validar si el archivo existe*/
            int existe = ValidaExisteArchivo(archivoTransmital);
            if(existe == 1)
            {

                lblMensajeInfo.Text = "El transmital electrónico de "+ nombreSitio + " se ha generado, para descargarlo da clic en el boton Descargar";
                pnlInfo.Visible = true;

                panelDescarga.Visible = true;
                hfpathArchivo.Value = archivoTransmital;
                btnBuscar.Enabled = true;
            }
            else
            {
                lblMensajeInfo.Text = "";
                pnlInfo.Visible = false;
                panelDescarga.Visible = false;
                btnBuscar.Enabled = true;
            }
        }
        private void CreaRepositorio(string path)
        {
            string archivoTransmital = Server.MapPath("~/Transmital//");         
            try
            {
                if (!Directory.Exists(archivoTransmital))
                {
                    Directory.CreateDirectory(archivoTransmital);               
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private int ValidaExisteArchivo(string pathArchivo)
        {
            int existe = 0;
            if (File.Exists(pathArchivo))
            {
                existe = 1;
            }
            return existe;
        }      
        private void CreaArchivoTransmital(string path, string mensaje)
        {
            using (StreamWriter archivoN = new StreamWriter(path,false,UTF8Encoding.UTF8))
            {
                archivoN.Write(mensaje);
            }
        }
        #endregion METODOS
        #region CONSULTAS
        private void ObtieneSitos()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT iCodCatalogo, vchDescripcion FROM "+esquema+ ".HistSitio WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND vchCodigo NOT IN('99999999','Telcel')");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if(dt != null && dt.Rows.Count > 0)
            {
                cboLocalidad.DataSource = dt;
                cboLocalidad.DataBind();
            }
        }
        private string ObtieneFechaTermina()
        {
            string fechaT="";
            StringBuilder query = new StringBuilder();
            query.AppendLine(" select case when len(datepart(mm, dateadd(mm, 2, '"+ fecIni + "'))) = 1 and len(datepart(dd, dateadd(mm, 2, '" + fecIni + "'))) = 1 then convert(varchar, datepart(yyyy, dateadd(mm, 2, '" + fecIni + "'))) + convert(varchar, '0') + convert(varchar, datepart(mm, dateadd(mm, 2, '" + fecIni + "'))) + convert(varchar, '0') + convert(varchar, datepart(dd, dateadd(mm, 2, '" + fecIni + "')))");
            query.AppendLine(" when len(datepart(mm, dateadd(mm, 2, '" + fecIni + "'))) = 1 and len(datepart(dd, dateadd(mm, 2, '" + fecIni + "'))) = 2 then convert(varchar, datepart(yyyy, dateadd(mm, 2, '" + fecIni + "'))) + convert(varchar, '0') + convert(varchar, datepart(mm, dateadd(mm, 2, '" + fecIni + "'))) + convert(varchar, datepart(dd, dateadd(mm, 2, '" + fecIni + "')))");
            query.AppendLine(" when len(datepart(mm, dateadd(mm, 2, '" + fecIni + "'))) = 2 and len(datepart(dd, dateadd(mm, 2, '" + fecIni + "'))) = 1 then convert(varchar, datepart(yyyy, dateadd(mm, 2, '" + fecIni + "'))) + convert(varchar, datepart(mm, dateadd(mm, 2, '" + fecIni + "'))) + convert(varchar, '0') + convert(varchar, datepart(dd, dateadd(mm, 2, '" + fecIni + "')))");
            query.AppendLine(" when len(datepart(mm, dateadd(mm, 2, '" + fecIni + "'))) = 2 and len(datepart(dd, dateadd(mm, 2, '" + fecIni + "'))) = 2 then convert(varchar, datepart(yyyy, dateadd(mm, 2, '" + fecIni + "'))) + convert(varchar, datepart(mm, dateadd(mm, 2, '" + fecIni + "'))) + convert(varchar, datepart(dd, dateadd(mm, 2, '" + fecIni + "')))");
            query.AppendLine(" end as Fecha");
            DataTable dt = DSODataAccess.Execute(query.ToString(),connStr);
            if(dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                fechaT = dr["Fecha"].ToString();
            }

            return fechaT;
        }
        private DataTable ObtieneConsumoSitio(string fechaInicio, string fechaFin, int sitio)
        {
            string sp = "EXEC FCATransmitalObtieneTotales @Esquema = '{0}', @FechaIni = '{1}', @FechaFin = '{2}', @Sitio = {3}";
            string query = string.Format(sp, esquema, fechaInicio, fechaFin, sitio);
            DataTable dt = DSODataAccess.Execute(query, connStr);
            return dt;
        }
        private DataTable ObtienLocalidadEmple(int emple)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" DECLARE @cencos INT ");
            query.AppendLine(" SELECT @cencos = CenCos FROM " + esquema + ".HistEmple WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND iCodCatalogo = " + emple + "");
            query.AppendLine(" SELECT cn.vchDescripcion AS Localidad");
            query.AppendLine(" FROM (");
            query.AppendLine(" SELECT");
            query.AppendLine(" SUBSTRING(LTRIM(RTRIM(FCANumeroLocalidad)),1,4) AS lo_numero,Sitio AS cn_idf");
            query.AppendLine(" FROM " + esquema + ".[VisHistoricos('FCALocalidad','Localidades FCA','Español')] WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" GROUP BY SUBSTRING(LTRIM(RTRIM(FCANumeroLocalidad)),1,4),Sitio ");
            query.AppendLine(" ) AS Locali");
            query.AppendLine(" INNER JOIN (");
            query.AppendLine(" SELECT iCodCatalogo AS cr_id, vchCodigo AS cr_numero FROM " + esquema + ".HistCenCos WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND iCodCatalogo = @cencos");
            query.AppendLine(" ) AS Cencos");
            query.AppendLine(" ON SUBSTRING(LTRIM(RTRIM(Locali.lo_numero)),1,4) = SUBSTRING(LTRIM(RTRIM(Cencos.cr_numero)),1,4)");
            query.AppendLine(" INNER JOIN " + esquema + ".HistSitio AS cn WITH(NOLOCK)");
            query.AppendLine(" ON cn.iCodCatalogo = Locali.cn_idf");
            query.AppendLine(" AND cn.dtIniVigencia <> cn.dtFinVigencia AND cn.dtFinVigencia >= GETDATE()");

            DataTable dt = DSODataAccess.Execute(query.ToString(),connStr);
            return dt;
        }
        private decimal ObtieneTotalTelcelEmple(int sitio, int emple,string fechaIntCel)
        {
            decimal total = 0;
            string sp = "EXEC [FCATransmitalTotalMovil] @Esquema = '{0}',@FechaInt = '{1}',@Sitio = {2},@Emple = {3}";
            string query = string.Format(sp,esquema, fechaIntCel, sitio, emple);
            DataTable dt = DSODataAccess.Execute(query,connStr);
            if(dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                total = Convert.ToDecimal(dr["totaltelcel"]);
            }
            return total;
        }
        private DataTable ObtieneConsumoTelcel(int mes,int anio,string fechaIntCel,int sitio)
        {
            DataTable dt = new DataTable();
            string sp = "EXEC FCATransmitalObtieneEmpleadosSoloTelcel @mesPbx = {0},@anioPbx = {1},@fechaintTelcel = {2},@sitio = {3},@iCodCatPlantaFCA = 0";
            string query = string.Format(sp, mes, anio, fechaIntCel, sitio);
            dt = DSODataAccess.Execute(query, connStr);
            return dt;
        }
        #endregion CONSULTAS

        protected void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {

                int anio = Convert.ToInt32(cboAnio.SelectedItem.ToString().Replace("TODOS", "0"));
                int mes = Convert.ToInt32(cboMes.SelectedValue.ToString().Replace("TODOS", "0"));
                if (anio > 0)
                {
                    if (mes > 0)
                    {
                        ObtieneFechasConsulta(mes, anio);
                        string fechaTermina = ObtieneFechaTermina();
                        string fecT = anioTransmital;
                        var fecha = DateTime.Now;
                        int sitio = Convert.ToInt32(cboLocalidad.SelectedValue);
                        string nomSitio = cboLocalidad.SelectedItem.ToString();
                        string nomArchivo = "transmital" + nomSitio + fecT + "_" + fecha.Hour.ToString() + fecha.Minute.ToString() + fecha.Second.ToString() + ".txt";
                        /**/
                        GeneraTransmital(nomArchivo, fechaTermina, fecIni, fecFin, sitio, anio, mes);
                    }
                    else
                    {
                        lblMensajeInfo.Text = "";
                        pnlInfo.Visible = false;
                        lblTituloModalMsn.Text = "Mensaje!";
                        lblBodyModalMsn.Text = "Debe de Seleccionar el mes!";
                        mpeEtqMsn.Show();
                        return;
                    }
                }
                else
                {
                    lblMensajeInfo.Text = "";
                    pnlInfo.Visible = false;
                    lblTituloModalMsn.Text = "Mensaje!";
                    lblBodyModalMsn.Text = "Debe de Seleccionar el año!";
                    mpeEtqMsn.Show();
                    return;
                }
            }
            catch
            {
                lblTituloModalMsn.Text = "Error!";
                lblBodyModalMsn.Text = "Ocurrio un Error al generar el Transmital..!";
                mpeEtqMsn.Show();
                return;

            }
        }

        protected void lnkVerDetall_Click(object sender, EventArgs e)
        {
            
            try
            {
                string filePath = hfpathArchivo.Value.ToString();
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment;filename=\"" + Path.GetFileName(filePath) + "\"");
                Response.TransmitFile(filePath);
                Response.End();
            }
            catch(Exception ex)
            {
                lblTituloModalMsn.Text = "Error!";
                lblBodyModalMsn.Text = "Ocurrio un Error al Descargar el transmital!";
                mpeEtqMsn.Show();
            }
        }
    }
}
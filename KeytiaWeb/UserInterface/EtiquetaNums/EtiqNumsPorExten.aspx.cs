using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace KeytiaWeb.UserInterface.EtiquetaNums
{
    public partial class EtiqNumsPorExten : System.Web.UI.Page
    {
        int emId;//= 204914;//2619;
        /*204914*/
        string fechaIni;
        string fechaFin;
        string fechaintcel = "0";
        int var_diacorte;
        string var_fechacorte;
        string var_fechaRealcorte="";
        int var_diaIni;
        int var_fechaEIni;
        int var_dias_antes_de_corte;
        int numLLam = 0;
        int sumT = 0;
        decimal sumC = 0;
        decimal sumPer = 0;
        decimal sumPersonal = 0;
        decimal sumNeg = 0;
        int sumTP = 0;
        int numLLam2 = 0;
        string var_muestrabotones = "S";
        string var_inhabilitarchkboxes = string.Empty;
        int usuarActivoMes=0;
        List<SitiosEmple> empleSitios = new List<SitiosEmple>();
        List<ConsumoLlamLocales> consumo = new List<ConsumoLlamLocales>();
        List<ConsumoLlamCobrar> consumoCobrar = new List<ConsumoLlamCobrar>();
        List<ConsumoCel> consumoCelular = new List<ConsumoCel>();

        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;
        int iCodUsuario;
        protected void Page_Load(object sender, EventArgs e)
        {
            Page.Form.Attributes.Add("enctype", "multipart/form-data");
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            iCodUsuario = Convert.ToInt32(Session["iCodUsuario"]);
            ObtieneIcodEmple(iCodUsuario);
            ObtieneFechasConsultas(emId);
           
            if (Session["fecIniFCA"] != null && Session["fecFinFCA"] != null)
            {
                fechaFin = Session["fecFinFCA"].ToString();
                fechaIni = Session["fecIniFCA"].ToString();
            }

            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((System.Web.UI.WebControls.Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion
            if (!Page.IsPostBack)
            {               
                IniciaProceso(emId);                             
            }                      
        }
        private void ObtieneFechasConsultas(int emId)
        {
            var mesFin = "0";
            var diaFin = "0";
          
            /*VALIDAR SI EMPLEADO SE ENCUENTRA EN EL HISTORICO FCAUsuarMesEtiquetacion ESTO PARA SABER SI
             PUEDE ETIQUETAR UN MES PASADO*/
            DataTable fechaMesEmple = ObtieneMesEtiquetacionExcepcion(iCodUsuario);
            if (fechaMesEmple != null && fechaMesEmple.Rows.Count > 0)
            {

                DataRow dr = fechaMesEmple.Rows[0];
                fechaintcel = dr["FechaInt"].ToString();
                if(Convert.ToInt32(fechaintcel) > 0)
                {
                    usuarActivoMes = 1;
                    Session["fecIniFCA"] = dr["FechaIni"].ToString();
                    Session["fecFinFCA"] = dr["FechaFin"].ToString();
                    DateTime fec1 = Convert.ToDateTime(dr["FECHAINICIO"]);

                    int anio1 = fec1.Year;
                    int mes1 = fec1.Month;
                    string mesDisp;
                    if (mes1 < 10)
                    {
                        mesDisp = "0" + mes1.ToString();
                    }
                    else { mesDisp = mes1.ToString(); }
                    ObtieneMesDisplay(mesDisp, anio1.ToString());
                }
            }

            if(Convert.ToInt32(fechaintcel) == 0)
            {
                DateTime fec = DateTime.Now;
                var anio_Fin = fec.Year.ToString();
                var mes_F = fec.Month;
                var dia_F = fec.Day;

                if (mes_F < 10)
                {
                    mesFin = "0" + mes_F.ToString();
                }
                else
                {
                    mesFin = mes_F.ToString();
                }
                if (dia_F < 10)
                {
                    diaFin = "0" + dia_F.ToString();
                }
                else
                {
                    diaFin = dia_F.ToString();
                }

                var fecFin = anio_Fin + mesFin + diaFin;
                var fecIni = anio_Fin + mesFin + "01";

                fechaFin = fecFin;
                fechaIni = fecIni;

                ObtieneFechaConsultas();

                Session["fechaIniFCA"] = fechaIni;
                Session["fechaFinFCA"] = fechaFin;

                var fechaInt = DateTime.Now.ToString("yyyy") + (Convert.ToInt32(mes_F) + 12).ToString();//201914


                if (Convert.ToInt32(mes_F) == 1)
                {
                    fechaintcel = DateTime.Now.AddYears(-1).ToString("yyyy") + "24";
                }
                else
                {
                    fechaintcel = DateTime.Now.ToString("yyyy") + ((mes_F - 1) + 12);

                    //if (Convert.ToInt32(mes_F) == 1)
                    //{
                    //    fechaintcel = DateTime.Now.AddYears(-1).ToString("yyyy") + "23";
                    //}
                    //else
                    //{
                    //    fechaintcel = DateTime.Now.ToString("yyyy") + ((mes_F - 1) + 12);
                    //}
                }

                ValidaEmpleTablaTiempoEnGracia(emId);
                /*SE COMPARA LA FECHA ACTUAL CONTRA LA FECHA DE CORTE, Y SI LA SEGUNDA ES MAYOR QUE LA FECHA ACTUAL, LA APLICACI�N BLOQUEA LA ETIQUETACION DE LLAMADAS.*/
                int dia_Corte = DateTime.Now.Day;
                if (dia_Corte <= var_diacorte && dia_Corte >= var_diaIni) //Si la fecha de hoy es menor a la fecha de corte:
                {
                    var_dias_antes_de_corte = (var_diacorte - dia_Corte);//DateTime.Now.Day;
                }
                else
                {
                    var_dias_antes_de_corte = -1;
                    var_inhabilitarchkboxes = "S";
                }


                if (var_dias_antes_de_corte >= 1)
                {
                    if (var_dias_antes_de_corte > 9)
                    {
                        //labelMensaje.InnerText = "PRUEBA Fecha límite para declaración de llamadas: " + var_fechaRealcorte;
                        labelMensaje.InnerText = "Fecha límite para declaración de llamadas: " + var_fechaRealcorte;
                    }
                    else
                    {
                        labelMensaje.InnerText = "Fecha límite etiquetar llamadas: " + var_fechaRealcorte;
                    }
                }
                else
                {
                    DateTime f = Convert.ToDateTime(var_fechacorte);
                    var dtfec = f.ToString("dd/MM/yyyy");
                    //var_fechacorte.ToString().Substring(0, 10);
                    //labelMensaje.Style.Value = "color:#FF0000;";
                    //labelMensaje.InnerText = "La Fecha límite para declarar llamadas de este periodo ha expirado: " + dtfec;
                }
            }
        }
        private void IniciaProceso(int emId)
        {
            ObtieneDatosEmpleado(emId);
            if(usuarActivoMes == 0)
            {
                ObtieneFechaConsultas();
            }

            var fecIni = Session["fecIniFCA"];
            var fecFin = Session["fecFinFCA"];

            fechaFin = fecFin.ToString();
            fechaIni = fecIni.ToString();

            GeneraDetalleLlam(emId, Convert.ToInt32(fecIni),Convert.ToInt32(fecFin), ref empleSitios,ref consumo);            
        }
        #region Metodos
        private void ObtieneFechaConsultas()
        {
            //Construyendo el formato de la fecha a emplear en las consultas
            var anioActual = DateTime.Now.Year.ToString();
            string mesF = "0";
            string mesAltF = "0";
            var anio = anioActual;
            var mes = DateTime.Now.Month;
            //'En Chrysler se siguen las siguientes reglas de fechas para el despliegue de la informaci�n:
            //' 1.- Las llamadas realizadas en el conmutador se muestran con un mes de antrioridad a la fecha actual
            //' ejemplo: Mes Actual = Marzo, Detalle a desplegar = febrero
            //' 2.- Las llamadas de Lineas Directas y Celulares se hace con un mes anterior al mes desplegado, 
            //' es decir, dos meses a la fecha actual debido que los datos llegan
            //' de la facturaci�n del Carrier (Telmex/Telcel) ejemplo: Mes desplegado = febrero, Linea Directas y Celular = Enero

            // Si el mes actual es enero, entonces el mes a Desplegar es Diciembre y por lo tanto, el a�o debe ser el anterior al actual
            if( mes == 1)
            {
                mes = 12; //Mes igual a Diciembre
                var myDate = DateTime.Now;
                var newDate = myDate.AddYears(-1);
                anioActual = newDate.Year.ToString();
                anio = anioActual;
            }
            else
            {
                mes = mes - 1; // Si el mes no es enero solo restamos el mes anterior
            }

            //'La fecha Alternativa (Alt) es la que usaremos para Lineas Directas y Celulares
            var mesAlt = DateTime.Now.Month;
            string anioActualAlt;
            int anioAlt;
            //' Si el mes actual es Enero, entonces el mes para Lineas Directas y Celulares es Noviembre
            if(mesAlt == 1)
            {
                mesAlt = 11;
                var myDate = DateTime.Now;
                var newDate = myDate.AddYears(-1);
                var a = newDate.Year;
                anioActualAlt = a.ToString();
                anioAlt = Convert.ToInt32(anioActualAlt);
            }
            else //' Si es febrero mostramos Diciembre
            {
                if(mesAlt == 2)
                {
                    mesAlt = 12;
                    var myDate = DateTime.Now;
                    var newDate = myDate.AddYears(-1);
                    var d = newDate.Year;
                    anioActualAlt = d.ToString();
                    anioAlt = Convert.ToInt32(anioActualAlt);
                }
                else
                {
                    mesAlt = mesAlt - 2;//' En el otro caso, restamos dos meses
                    anioActualAlt = DateTime.Now.Year.ToString();
                    anioAlt = Convert.ToInt32(anioActualAlt);

                }
            }

            //' Formato de fechas, le agregamos un cero a los meses menores al d�cimo
            //' Primero al mes del detalle
            if ( mes < 10 )
            {
                mesF = "0" + mes.ToString();
            }
            else
            {
                mesF =  mes.ToString();
            }
            //'Despu�s al mes de Lineas Directas y Celulares
            if(mesAlt < 10)
            {
                mesAltF = "0" + mesAlt.ToString();
            }
            else
            {
                mesAltF = mesAlt.ToString();
            }

            if (Session["fecIniFCA"] != null) { Session["fecIniFCA"] = null; }
            if (Session["fecFinFCA"] != null) { Session["fecFinFCA"] = null; }
            int ultimoDia = UltimoDiaDelMes(mesF,Convert.ToInt32(anio));

            Session["fecIniFCA"] = anio + mesF.ToString() + "01";
            Session["fecFinFCA"] = anio + mesF.ToString() + ultimoDia.ToString();

            if (Session["fecFiniAltFCA"] != null) { Session["fecFiniAltFCA"] = null; }
            if (Session["fecIniAltFCA"] != null) { Session["fecIniAltFCA"] = null; }
            int ultimoDiaAlt = UltimoDiaDelMes(mesF, Convert.ToInt32(anio));

            Session["fecIniAltFCA"] = anioAlt + mesAltF.ToString() + "01";
            Session["fecFiniAltFCA"] = anioAlt + mesAltF.ToString() + ultimoDiaAlt.ToString();

            ObtieneMesDisplay(mesF, anioActual);
        }
        private void ObtieneDatosEmpleado( int emId)
        {
            DataTable dt = ObtieneDatosEmple(emId);
            if( dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                var numDepto = dr["vchCodigo"].ToString().Substring(4).Replace("-", "").Trim();
                txtNomEmple.Text = dr["NomCompleto"].ToString();//dr["em_apPaterno"].ToString() + " " + dr["em_apMaterno"].ToString() + " " + dr["em_nombre"].ToString();
                txtNumDepto.Text = numDepto;
                txtDepartamento.Text = dr["Descripcion"].ToString();//dr["cr_departamento"].ToString();
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
                    ObtieneDatosExterno();
                }
                else
                {
                    rowCencos.Visible = false;
                    rowIdCencos.Visible = false;
                    //rowLocalidad.Visible = true;
                    //rowNumLocali.Visible = true;
                    //ObtieneLocalidadEmple(dr["vchCodigo"].ToString().Substring(0,4));
                    txtNumEmpleado.Text = dr["NominaA"].ToString();
                }

                rowLocalidad.Visible = true;
                rowNumLocali.Visible = true;
                ObtieneLocalidadEmple(dr["vchCodigo"].ToString().Substring(0, 4));
            }

        }
        private int UltimoDiaDelMes(string mesF, int anio)
        {
            int UltimoDia = 0;
            if (mesF == "01" || mesF == "03" || mesF == "05" || mesF == "07" || mesF == "08" || mesF == "10" || mesF == "12")
            {
                UltimoDia = 31;
            }
            if (mesF == "04" || mesF == "06" || mesF == "09" || mesF == "11")
            {
                UltimoDia = 30;
            }
            if(mesF == "02")
            {
                var anioA = "20" + anio;
                var result = Convert.ToInt32(anioA) % 4;
                if(result == 0)
                {
                    UltimoDia = 29;
                }
                else
                {
                    UltimoDia = 28;
                }
            }

           return UltimoDia;
        }
        private void ObtieneMesDisplay(string mes,string anioActual)
        {
            string mesDisplay="";
            if(Session["mesDisplay"] != null) { Session["mesDisplay"] = null; }
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
        private void GeneraDetalleLlam(int emId, int fecini, int fecFin, ref List<SitiosEmple> empleSitios, ref List<ConsumoLlamLocales> consumo)
        {
            /*OBTIENE EL LISTADO DE SITIOS  */
            ObtieneSitiosEmple(ref empleSitios);

            /*Genera tabla de detalle de llamadas locales*/
            ObtieneLlamLocales(emId, fecini, fecFin, ref empleSitios, ref consumo);

            /*Genera tabla para el detalle de llamadas LDN,CELULAR Y TELENET*/
            ObtieneLlamadasaCobrar(emId, fecini, fecFin, ref empleSitios, ref consumoCobrar);

            /*'*********************************
		        'APROBACION DE CELULARES
		        '*********************************
		        'Funciona muy similar a lo desplegado para las tablas de DetalleEmleado de los sitios, la variante es el query
		        'Busca directamente en la tabla de detalleempleadoTelcel
		        'El resto funciona igual,Obtenemos las llamadas de la tabla de DetalleEmpleadoTelcel
            */
            sumPer = 0;
            ObtieneLlamCelular(emId, fecini, fecFin, ref empleSitios,ref consumoCelular);

            /*LLENA TOTALES GENERALES*/
            spnCelLd.InnerText = numLLam2.ToString();
            spnTiempoTot.InnerText = sumTP.ToString() + " minutos";
            /*importe total*/
            SpanTotal.InnerText = sumC.ToString("#0.00");

            ////Falta Realizar ciertas Validaciones para este dato importe personal aceptado
            spnTotalLlam.InnerText = sumPersonal.ToString("#0.00");            
            ///*importe negocio*/
            SpanTotalNeg.InnerText = sumNeg.ToString("#0.00");


            if (var_muestrabotones == "S")
            {
                if (var_inhabilitarchkboxes == "S")
                {
                    btnGuardar.Enabled = false;
                }
            }
            else
            {
                if (var_inhabilitarchkboxes == "S")
                {
                    btnGuardar.Enabled = false;
                }
            }

            if (consumoCelular.Count > 0 || consumoCobrar.Count > 0)
            {
                row5.Visible = true;
                row6.Visible = true;
            }
            else if(consumoCelular.Count <= 0 && consumoCobrar.Count <= 0)
            {
                row5.Visible = false;
                row6.Visible = false;
            }
        }
        private void ObtieneLlamCelular(int emId, int fecini, int fecFin, ref List<SitiosEmple> empleSitios,ref List<ConsumoCel> consumoCelular)
        {
            consumoCelular.Clear();
            ObtieneConsumoCel1(emId, ref consumoCelular);
            ObtieneConsumoCel2(emId, ref consumoCelular);

            if(consumoCelular.Count > 0)
            {
                int totalLlamadas = 0;
                int totalMinutos = 0;
                decimal costoTotal = 0;
                foreach (var item in consumoCelular)
                {
                    totalLlamadas += item.Numero;
                    totalMinutos += item.Duracion;
                    costoTotal += item.Costo;

                    numLLam += 1;
                    numLLam2 += item.Numero;
                    sumT += item.Duracion;
                    sumTP += item.Duracion;
                    sumC += item.Costo;

                    sumPer += item.Costo;
                }

                row4.Visible = true;
                string mesDisplay = Session["mesDisplay"].ToString();
                parrafo3.InnerText = "Detalle de llamadas de Telefonía Móvil" + " - " + mesDisplay;

                grdMovil.DataSource = consumoCelular;
                grdMovil.DataBind();

                /*Valida que registros de la tabla se van a deshabilitar el check box o checkear los mismos*/
                GridView grv = grdMovil;
                GridView grvTot = grvTotalMovil;
                DatosGridview(grv, grvTot, totalLlamadas, totalMinutos, costoTotal);

                row5.Visible = true;
                row6.Visible = true;
                row3.Visible = true;
            }
            else
            {
                row4.Visible = false;
                //row5.Visible = false;
                //row6.Visible = false;
                //row3.Visible = false;
            }
        }
        private void ObtieneLlamLocales(int emId, int fecini, int fecFin, ref List<SitiosEmple> empleSitios, ref List<ConsumoLlamLocales> consumo)
        {
            //' Hacemos un ciclo que recorrer� todas las tablas a mostrar, 
            //' que son �nicamente aquellas en donde el usuario tenga extensi�n y/o c�digo
            //validar si existen sitios que se cobraran las llamadas locales 
            int totalLlamadas = 0;
            int totalMinutos = 0;
            decimal totalCosto = 0;
            /*OBTIENE LAS LLAMDAS LOCALES DEL DETALLE DE CDR CUANDO EL SITIO TIENE LA BANDERA APAGADA DE 
             COBRAR LLAMADAS LOCALES*/
            //foreach (var item in empleSitios)
            //{
            //    int conmutadorID = item.CencosId;
            //    string tabla = item.Cencos;
            //    int cobroLlamadasLocales = item.CobroLlamLocales;
            var_muestrabotones = "S";  //'<-- esta variable debe igualarse a "N" en caso de que se requiera bloquear las llamadas.
            //0
            //if (cobroLlamadasLocales != 1)/*Obtiene llamadas que no se cobran de los sitios*/
            //{
            // DataTable dt = OtieneDetallLlamLocales(tabla, conmutadorID, fecini, fecFin, emId);
            DataTable dt = OtieneDetallLlamLocales( fecini, fecFin, emId);
            if (dt != null && dt.Rows.Count > 0)
            {
                row5.Visible = false;
                        foreach (DataRow dr in dt.Rows)
                        {
                            ConsumoLlamLocales con = new ConsumoLlamLocales();
                            con.TablaConsulta = Convert.ToInt32(dr["Sitio"]);
                            con.Extension = dr["Extension"].ToString();
                            string numero = dr["NumMarcado"].ToString();
                            string numMarcado =  ValidaNum(numero);
                            con.NumMarcado = numero;
                            con.Cantidad = Convert.ToInt32(dr["Cantidad"]);
                            con.Duracion = Convert.ToInt32(dr["Duracion"]);
                            con.Localidad = string.IsNullOrEmpty(dr["Etiqueta"].ToString()) ? " " : dr["Etiqueta"].ToString().ToUpper();
                            con.LocalidadKeytia = dr["Localidad"].ToString().ToUpper();
                            con.Costo = Convert.ToDecimal(dr["Importe"]);
                            consumo.Add(con);

                            totalLlamadas += Convert.ToInt32(dr["Cantidad"]);
                            totalMinutos += Convert.ToInt32(dr["Duracion"]);
                            totalCosto += Convert.ToDecimal(dr["Importe"]);
                        }
                //    }
                //}
            }

            if (consumo.Count > 0)/*si existen sitios con llamadas locales que no se cobraran*/
            {
                row5.Visible = true;
                row2.Visible = true;
                row6.Visible = true;
                grdLlamLocales.DataSource = consumo;
                grdLlamLocales.DataBind();
                grdLlamLocales.UseAccessibleHeader = true;
                grdLlamLocales.HeaderRow.TableSection = TableRowSection.TableHeader;
                if (consumo.Count > 10)
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

                    List<TotalConsumo> listConsumoTot = new List<TotalConsumo>();
                    listConsumoTot.Clear();
                    ObtieneTotLlam(totalLlamadas, totalMinutos, totalCosto, ref listConsumoTot);
                    grdTotales.DataSource = listConsumoTot;
                    grdTotales.DataBind();
                }

                string mesDisplay = Session["mesDisplay"].ToString();
                parrafo1.InnerText = "Detalle de llamadas locales - " + mesDisplay;
            }
            else
            {
                row2.Visible = false;
                row5.Visible = false;
                row6.Visible = false;
            }
        }
        private void ObtieneLlamadasaCobrar(int emId, int fecini, int fecFin, ref List<SitiosEmple> empleSitios, ref List<ConsumoLlamCobrar> consumoCobrar)
        {
            consumoCobrar.Clear();
            int totalLlamadas = 0;
            int totalMinutos = 0;
            decimal costoTotal = 0;
            foreach (var item in empleSitios)
            {
                int conmutadorID = item.CencosId;
                string tabla = item.Cencos;
                int cobroLlamadasLocales = item.CobroLlamLocales;
                DataTable dt = ObtieneDetallCobrar(tabla, conmutadorID, fecini, fecFin, emId, cobroLlamadasLocales);
                if(dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        ConsumoLlamCobrar list = new ConsumoLlamCobrar();
                        list.TablaConsulta = Convert.ToInt32(dr["Sitio"]);
                        string numero = dr["NumMarcado"].ToString();
                        string numMarcado = ValidaNum(numero);
                        list.NumMarcado = numero;
                        list.Idf = Convert.ToInt32(dr["TipoEtiqueta"]);/*Campo que define si una llamada es laboral o personal*/
                        list.TipoEtiqueta = Convert.ToInt32(dr["De_Etiqueta"]);/*Campo para Validar si una llamada puede ser etiquetada o no*/
                        list.TipoDestino = Convert.ToInt32(dr["TDest"]);
                        list.Extension = dr["Extension"].ToString();
                        list.Localidad = string.IsNullOrEmpty(dr["Etiqueta"].ToString())?" ":dr["Etiqueta"].ToString().ToUpper();
                        list.Costo = Convert.ToDecimal(dr["Importe"]);
                        list.Duracion = Convert.ToInt32(dr["Duracion"]);
                        list.Cantidad = Convert.ToInt32(dr["Cantidad"]);
                        list.LocalidadKeytia = dr["Localidad"].ToString().ToUpper();
                        consumoCobrar.Add(list);

                        totalLlamadas += Convert.ToInt32(dr["Cantidad"]);
                        totalMinutos += Convert.ToInt32(dr["Duracion"]);
                        costoTotal += Convert.ToDecimal(dr["Importe"]);

                        //Estos son contadores para totalizar las cuentas
                        numLLam += 1;//Numero de Llamadas Desplegadas(es el identificador de cada elemento)
                        numLLam2 += Convert.ToInt32(dr["Cantidad"]);//Cantidad total de llamadas seg�n la agrupaci�n
                        sumT += Convert.ToInt32(dr["Duracion"]);//Suma del tiempo total (Duraci�n)
                        sumTP += Convert.ToInt32(dr["Duracion"]);
                        sumC += Convert.ToDecimal(dr["Importe"]);

                        //Llevamos una suma para saber cuanto es el costo total a desplegar, sumamos el costo de la llamada segun el detalle
                         sumPer += Convert.ToDecimal(dr["Importe"]);

                    }
                }


            }

            if(consumoCobrar.Count > 0)
            {
                //if (numLLam > 10)
                //{
                //    LlamadasDiv.Style.Value = "height:210px;overflow-y:auto;overflow-x:auto;";
                //}
                //else
                //{
                //    LlamadasDiv.Style.Value = "overflow-y:auto;overflow-x:auto;";
                //}

                row5.Visible = true;
                row6.Visible = true;

                grvLlamadas.DataSource = consumoCobrar;
                grvLlamadas.DataBind();

                row3.Visible = true;
                string mesDisplay = Session["mesDisplay"].ToString();
                parrafo2.InnerText = "Detalle de llamadas de Larga Distancia, a Celular y TELENET - " + mesDisplay;
                //Este es el checkbox todos, que manda llamar la funci�n de Javascript del mismo nombre
                //Contiene una validaci�n para mostrar o no el checkbox


                if (var_muestrabotones != "S" || var_inhabilitarchkboxes == "S")
                {
                    chkMostrar.Enabled = false;
                }
                else
                {
                    chkMostrar.Enabled = true;
                }

                /*Valida que registros de la tabla se van a deshabilitar el check box o checkear los mismos*/
                GridView grv = grvLlamadas;
                GridView grvTot = grdLlamCobrarTot;
                DatosGridview(grv, grvTot, totalLlamadas, totalMinutos, costoTotal);

            }
            else
            {
                //row5.Visible = false;
                //row6.Visible = false;
                row3.Visible = false;
            }
        }
        private void DatosGridview(GridView grd,GridView grdTot,int totalLlamadas, int totalMinutos,decimal costoTotal)
        {
            //if (consumoCobrar.Count > 3)
            //{
            //    grvLlamadas.CssClass = "fixed_header table table-bordered tableDashboard";
            //}
            //else
            //{
            //    grvLlamadas.CssClass = "tableHeaderStyle table table-bordered tableDashboard";
            //}

            grd.UseAccessibleHeader = true;
            grd.HeaderRow.TableSection = TableRowSection.TableHeader;


            List<TotalConsumo> listConsumoTot = new List<TotalConsumo>();
            listConsumoTot.Clear();
            ObtieneTotLlam(totalLlamadas, totalMinutos, costoTotal, ref listConsumoTot);
            grdTot.DataSource = listConsumoTot;
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
                var textBox = grd.Rows[i].FindControl("txtRferencia") as System.Web.UI.WebControls.TextBox;

                var fila = grd.Rows[i];
                int Idf = Convert.ToInt32(grd.DataKeys[i].Values[0]);
                int idEtiqueta = Convert.ToInt32(grd.DataKeys[i].Values[2]);
                decimal costo = Convert.ToDecimal(grd.DataKeys[i].Values[3]);

                if (Idf == 2)
                {
                    checkbox.Checked = true;
                }

                if (var_inhabilitarchkboxes == "S") 
                {
                    /* Se quita esta condicion en el if, a peticion del cliente 
                     * para que no bloque las llamadas que 
                     * fueron etiquetadas anteriorment  idEtiqueta != 0 || */
                    checkbox.Enabled = false;
                    textBox.Enabled = false;
                }

                if (checkbox.Checked == true)
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
        private void MuestraDetalleLlamadas(string numMarcado,string extension,string tipo,string localidad,string tablaConsulta,int extId)
        {
            MuestraDatosEmple(0);
            /*Obener el detalle de la llamada*/
            p1.InnerText = "Detalle de llamadas - " + numMarcado;
            ObtieneDetalleLlamadas(numMarcado, extension, tipo, localidad, tablaConsulta, extId);
        }
        private void MuestraDatosEmple(int opc)
        {
            mpeEditHallazo.Show();
            pnlPopupAsignaLinea.CssClass = "position:fixed;z-index:10001;left:100px;top:-60px;width:90%;height:100%;overflow:auto;";
            pnlPopupAsignaLinea.Style.Value = "position:fixed;z-index:10001;left:100px;top:-60px;width:90%;height:100%;overflow:auto;";
            if(opc == 1)
            {
                btnImprimirModal.Visible = true;
            }
            else
            {
                btnImprimirModal.Visible = false;
            }
            //rowCencos.Visible = true;
            //rowIdCencos.Visible = true;
            txtNomEmpleado.Text = txtNomEmple.Text;
            txtClaveEmple.Text = txtNumEmpleado.Text;
            txtClaveDepto.Text = txtNumDepto.Text;
            txtDep.Text = txtDepartamento.Text;

            if (rowCencos.Visible == true && rowIdCencos.Visible == true)
            {
                rowCencosto.Visible = true;
                rowNumCencosto.Visible = true;
                rowIdLocalidad.Visible = false;
                rowLocali.Visible = false;
                txtCencosto.Text = txtCencos.Text;
                txtNumCencosto.Text = txtNumCencos.Text;

            }
            else if (rowLocalidad.Visible == true && rowNumLocali.Visible == true)
            {
                txtLocali.Text = txtLocalidad.Text;
                txtLocalidadId.Text = txtNumLocali.Text;

                rowIdLocalidad.Visible = true;
                rowLocali.Visible = true;
                rowCencosto.Visible = false;
                rowNumCencosto.Visible = false;
            }

        }
        private void ObtieneDetalleLlamadasGlobal(int emId, int fecini, int fecFin, ref List<SitiosEmple> empleSitios, ref List<ConsumoLlamLocales> consumo)
        {
            string mesDisplay = Session["mesDisplay"].ToString();
            /*muestra el detalle de totas las llamadas del usuario*/
            /*EL NUMERO DE SITIOS EN LOS QUE TENGA CODIGO O EXTENSION EL EMPLEADO */
            ObtieneSitiosEmple(ref empleSitios);
            /*Genera tabla de detalle de llamadas locales*/
            ObtieneLlamLocalesGlobales(emId, fecini, fecFin, ref empleSitios, ref consumo);
            p1.InnerText = "Detalle de llamadas de Larga Distancia y a Celular - " + mesDisplay;
            /*Obtiene consumo telenet YA NO APLICA EN K5*/
            //ObtieneLlamadasTeleNet(fecini, fecFin, emId);
            /*Obtiene Consumo celular*/
            ObtieneDellGlobalLlamCelular(emId, ref consumoCelular);

            MuestraDatosEmple(1);
        }
        private void ObtieneLlamLocalesGlobales(int emId, int fecini, int fecFin, ref List<SitiosEmple> empleSitios, ref List<ConsumoLlamLocales> consumo)
        {
            //' Hacemos un ciclo que recorrer� todas las tablas a mostrar, 
            //' que son �nicamente aquellas en donde el usuario tenga extensi�n y/o c�digo
            //validar si existen sitios que se cobraran las llamadas locales
            List<DetalleLlamadas> listDetalle = new List<DetalleLlamadas>();
            listDetalle.Clear();
            int totalLlamadas = 0;
            int totalMinutos = 0;
            decimal totalCosto = 0;
            int contador = 0;
            foreach (var item in empleSitios)
            {
                int conmutadorID = item.CencosId;
                string tabla = item.Cencos;
                int cobroLlamadasLocales = item.CobroLlamLocales;
                    DataTable dt = ObtieneDetallLlamLocalesGlobales(tabla, conmutadorID, fecini, fecFin, emId, cobroLlamadasLocales);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                        DetalleLlamadas li = new DetalleLlamadas();
                        li.Extension = dr["de_extension"].ToString();
                        li.Fecha = dr["de_fecha"].ToString();
                        li.Hora = dr["de_hora"].ToString();
                        string numero = dr["de_nummarcado"].ToString();
                        string numeroMarcado = ValidaNum(numero);
                        li.NumMarcado = numero;
                        li.Duracion = Convert.ToInt32(dr["de_duracion"]);
                        li.Localidad = dr["de_localidadKeytia"].ToString();//dr["de_localidad"].ToString();
                        li.Referencia = "";
                        li.Costo = Convert.ToDecimal(dr["de_costo"]);
                        listDetalle.Add(li);

                         totalLlamadas += 1;
                         totalMinutos += Convert.ToInt32(dr["de_duracion"]);
                         totalCosto += Convert.ToDecimal(dr["de_costo"]);
                        contador += 1;
                        }
                    }               
                }

            if (contador > 5)
            {
                llamDetall.Style.Value = "height:210px;overflow-y:auto;overflow-x:auto;";
            }
            else
            {
                llamDetall.Style.Value = "overflow-y:auto;overflow-x:auto;";
            }

            //grdDetalle.Columns[1].Visible = false;
            //grdDetalle.Columns[2].Visible = false;

            grdDetalle.DataSource = null;
            grdDetalle.DataBind();

            grdDetalle.DataSource = listDetalle;
            grdDetalle.DataBind();

            if (totalLlamadas > 0)
            {
                grdViewTotal.DataSource = null;
                grdViewTotal.DataBind();

                List<TotalConsumo> listConsumoTot = new List<TotalConsumo>();
                listConsumoTot.Clear();
                ObtieneTotLlam(totalLlamadas, totalMinutos, totalCosto, ref listConsumoTot);
                grdViewTotal.DataSource = listConsumoTot;
                grdViewTotal.DataBind();
            }


        }
        private void ObtieneLlamadasTeleNet(int fecini, int fecFin,int emId)
        {
            int totalLlamadas = 0;
            int totalMinutos = 0;
            decimal costoTotal = 0;
            List<ConsumoLlamCobrar> detallTelenet = new List<ConsumoLlamCobrar>();
            detallTelenet.Clear();

            /*Obtiene consumo telenet*/
            DataTable dt2 = ObtieneConsumoTelenet(fecini, fecFin, emId);
            if (dt2 != null && dt2.Rows.Count > 0)
            {
                foreach (DataRow dr2 in dt2.Rows)
                {
                    ConsumoLlamCobrar list2 = new ConsumoLlamCobrar();
                    list2.Extension = dr2["de_extension"].ToString();
                    list2.Duracion = Convert.ToInt32(dr2["de_duracion"]);
                    list2.NumMarcado = dr2["de_Nummarcado"].ToString();
                    list2.Localidad = string.IsNullOrEmpty(dr2["de_localidad"].ToString()) ? " " : dr2["de_localidad"].ToString().ToUpper();                                    
                    list2.LocalidadKeytia = dr2["de_localidadKeytia"].ToString().ToUpper();
                    list2.Costo = Convert.ToDecimal(dr2["de_costo"]);
                    detallTelenet.Add(list2);

                    totalLlamadas += 1;
                    totalMinutos += Convert.ToInt32(dr2["de_duracion"]);
                    costoTotal += Convert.ToDecimal(dr2["de_costo"]);

                }
            }

            grdTelenet.DataSource = null;
            grdTelenet.DataBind();

            grdTelenet.DataSource = detallTelenet;
            grdTelenet.DataBind();

            if (totalLlamadas > 5)
            {
                grdTelenet.Style.Value = "height:210px;overflow-y:auto;overflow-x:auto;";
            }
            else
            {
                grdTelenet.Style.Value = "overflow-y:auto;overflow-x:auto;";
            }

            if (costoTotal > 0)
            {
                grdTotalTelenet.DataSource = null;
                grdTotalTelenet.DataBind();

                List<TotalConsumo> totales = new List<TotalConsumo>();
                TotalConsumo consTotalConsumo = new TotalConsumo();
                consTotalConsumo.Llamadas = "Total de llamadas Telenet: " + totalLlamadas;
                consTotalConsumo.Minutos = "Total de minutos: " + totalMinutos;
                consTotalConsumo.CostoTotal = "Total costo: " + "$" + costoTotal.ToString("#0.00");
                totales.Add(consTotalConsumo);

                grdTotalTelenet.DataSource = totales;
                grdTotalTelenet.DataBind();
            }

        }
        private string ValidaNum(string numero)
        {
            string numDevuelto = "";
            Regex regex01800 = new Regex("^(01800)");
            Regex rex = new Regex("^(044|045|01)");
            Match match = regex01800.Match(numero);
            Match match2 = rex.Match(numero);

            if (match.Success)
            {
                numDevuelto = numero;
            }
            else if (match2.Success)
            {
                if (numero.Length > 10)
                {
                    numDevuelto = rex.Replace(numero,"").Trim();                    
                }
                else
                {
                    numDevuelto = numero;
                }
            }
            else
            {
                numDevuelto = numero;
            }
            return numDevuelto;
        }
        #endregion Metodos
        #region consultas
        private void ObtieneConsumoCel1(int emid, ref List<ConsumoCel> consumoCelular)
        {

            string sp = "EXEC FCAObtieneDetallCelular @Esquema = '{0}',@Emple = {1},@FechaInt = {2},@Opcion={3}";
            string query = string.Format(sp, esquema, emid, fechaintcel, 1);
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if(dt != null && dt.Rows.Count  > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ConsumoCel c = new ConsumoCel();
                    c.Tabla = "detalleempleadotelcel";
                    //c.CodigoAut = dr["de_codigoaut"].ToString();/*en k5 ya no se utiliza*/
                    //c.ExtId = Convert.ToInt32(dr["de_ex_id"]);/* en K5 Ya no se utiliza*/
                    string numero  = dr["de_nummarcado"].ToString();
                    string numMarcado = ValidaNum(numero);/*ELIMINA LOS PREFIJOS 044,045,01*/
                    c.NumMarcado = numero;
                    c.Idf = Convert.ToInt32(dr["tl_idf"]);
                    c.Etiqueta = Convert.ToInt32(dr["de_etiqueta"]);
                    c.TipoDestino = dr["de_tipodestino"].ToString();
                    c.Extension = dr["de_extension"].ToString();
                    c.Localidad = dr["de_localidad"].ToString();
                    c.Costo = Convert.ToDecimal(dr["de_costo"]);
                    c.Duracion = Convert.ToInt32(dr["de_duracion"]);
                    c.Numero = Convert.ToInt32(dr["numero"]);
                    c.LocalidadKeytia = dr["de_localidadkeytia"].ToString();
                    consumoCelular.Add(c);
                }
            }

        }
        private void ObtieneConsumoCel2(int emid, ref List<ConsumoCel> consumoCelular)
        {
            string sp = "EXEC FCAObtieneDetallCelular @Esquema = '{0}',@Emple = {1},@FechaInt = {2},@Opcion={3}";
            string query = string.Format(sp, esquema, emid, fechaintcel, 2);
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if(dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ConsumoCel c = new ConsumoCel();
                    c.Tabla = "detalleempleadotelcel";
                    //c.CodigoAut = dr["de_codigoaut"].ToString();
                    //c.ExtId = Convert.ToInt32(dr["de_ex_id"]);
                    string numero = dr["de_nummarcado"].ToString();
                    string numMarcado = ValidaNum(numero);
                    c.NumMarcado = numero;
                    c.Idf = Convert.ToInt32(dr["tl_idf"]);
                    c.Etiqueta = Convert.ToInt32(dr["de_etiqueta"]);
                    c.TipoDestino = dr["de_tipodestino"].ToString();
                    c.Extension = dr["de_extension"].ToString();
                    c.Localidad = dr["de_localidad"].ToString();
                    c.Costo = Convert.ToDecimal(dr["de_costo"]);
                    c.Duracion = Convert.ToInt32(dr["de_duracion"]);
                    c.Numero = Convert.ToInt32(dr["numero"]);
                    c.LocalidadKeytia = dr["de_localidadkeytia"].ToString();
                    consumoCelular.Add(c);
                }
               
            }
        }
        private void ValidaEmpleTablaTiempoEnGracia(int emId)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(" SELECT TOP 1 FechaAcum AS Fecha, dia = DATEPART(dd, FechaAcum),");
            query.AppendLine(" CASE WHEN LEN(DATEPART(mm, DATEADD(dd, -1, FechaAcum))) = 1 AND LEN(DATEPART(dd, DATEADD(dd, -1, FechaAcum))) = 1 THEN SUBSTRING(CONVERT(VARCHAR, DATEPART(yyyy, DATEADD(dd, -1, FechaAcum))), 3, 2) + CONVERT(VARCHAR, '0') + CONVERT(VARCHAR, DATEPART(mm, DATEADD(dd, -1, FechaAcum))) + CONVERT(VARCHAR, '0') + CONVERT(VARCHAR, DATEPART(dd, DATEADD(dd, -1, FechaAcum)))");
            query.AppendLine(" WHEN LEN(DATEPART(mm, DATEADD(dd, -1, FechaAcum))) = 1 AND LEN(DATEPART(dd, DATEADD(dd, -1, FechaAcum))) = 2 THEN SUBSTRING(CONVERT(VARCHAR, DATEPART(yyyy, DATEADD(dd, -1, FechaAcum))), 3, 2) + CONVERT(VARCHAR, '0') + CONVERT(VARCHAR, DATEPART(mm, DATEADD(dd, -1, FechaAcum))) + CONVERT(VARCHAR, DATEPART(dd, DATEADD(dd, -1, FechaAcum)))");
            query.AppendLine(" WHEN LEN(DATEPART(mm, DATEADD(dd, -1, FechaAcum))) = 2 AND LEN(DATEPART(dd, DATEADD(dd, -1, FechaAcum))) = 1 THEN SUBSTRING(CONVERT(VARCHAR, DATEPART(yyyy, DATEADD(dd, -1, FechaAcum))), 3, 2) + CONVERT(VARCHAR, DATEPART(mm, DATEADD(dd, -1, FechaAcum))) + CONVERT(VARCHAR, '0') + CONVERT(VARCHAR, DATEPART(dd, DATEADD(dd, -1, FechaAcum)))");
            query.AppendLine(" WHEN LEN(DATEPART(mm, DATEADD(dd, -1, FechaAcum))) = 2 AND LEN(DATEPART(dd, DATEADD(dd, -1, FechaAcum))) = 2 THEN SUBSTRING(CONVERT(VARCHAR, DATEPART(yyyy, DATEADD(dd, -1, FechaAcum))), 3, 2) + CONVERT(VARCHAR, DATEPART(mm, DATEADD(dd, -1, FechaAcum))) + CONVERT(VARCHAR, DATEPART(dd, DATEADD(dd, -1, FechaAcum)))");
            query.AppendLine(" END AS UltimaFechaEtiquetar,");
            query.AppendLine(" UltimoDiaEtiquetar = DATEPART(dd, DATEADD(dd, -1, FechaAcum))");
            query.AppendLine(" FROM ["+esquema+ "].[VisHistoricos('EtiqTiempoGracia','Etiquetacion Tiempo Gracia','Español')] WITH (NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia");
            query.AppendLine(" AND Emple = "+ emId + "");
            query.AppendLine(" AND DATEPART(mm, FechaAcum) = DATEPART(mm, GETDATE())");
            query.AppendLine(" AND DATEPART(yyyy, FechaAcum) = DATEPART(yyyy, GETDATE()) ORDER BY FechaAcum DESC ");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                var_diacorte = Convert.ToInt32(dr["dia"]);
                var_fechacorte = dr["fecha"].ToString();
                var_fechaRealcorte = dr["UltimaFechaEtiquetar"].ToString();
            }
            else
            {
                StringBuilder query2 = new StringBuilder();

                query2.AppendLine(" SELECT FC.FechaAcum AS fecha,");
                query2.AppendLine(" CASE WHEN LEN(DATEPART(mm, DATEADD(dd, -1, FC.FechaAcum))) = 1 AND LEN(DATEPART(dd, DATEADD(dd, -1, FC.FechaAcum))) = 1 THEN SUBSTRING(CONVERT(VARCHAR, DATEPART(yyyy, DATEADD(dd, -1, FC.FechaAcum))), 3, 2) + CONVERT(VARCHAR, '0') + CONVERT(VARCHAR, DATEPART(mm, DATEADD(dd, -1, FC.FechaAcum))) + CONVERT(VARCHAR, '0') + CONVERT(VARCHAR, DATEPART(dd, DATEADD(dd, -1, FC.FechaAcum)))");
                query2.AppendLine(" WHEN LEN(DATEPART(mm, DATEADD(dd, -1, FC.FechaAcum))) = 1 AND LEN(DATEPART(dd, DATEADD(dd, -1, FC.FechaAcum))) = 2 THEN SUBSTRING(CONVERT(VARCHAR, DATEPART(yyyy, DATEADD(dd, -1, FC.FechaAcum))), 3, 2) + CONVERT(VARCHAR, '0') + CONVERT(VARCHAR, DATEPART(mm, DATEADD(dd, -1, FC.FechaAcum))) + CONVERT(VARCHAR, DATEPART(dd, DATEADD(dd, -1, FC.FechaAcum)))");
                query2.AppendLine(" WHEN LEN(DATEPART(mm, DATEADD(dd, -1, FC.FechaAcum))) = 2 AND LEN(DATEPART(dd, DATEADD(dd, -1, FC.FechaAcum))) = 1 THEN SUBSTRING(CONVERT(VARCHAR, DATEPART(yyyy, DATEADD(dd, -1, FC.FechaAcum))), 3, 2) + CONVERT(VARCHAR, DATEPART(mm, DATEADD(dd, -1, FC.FechaAcum))) + CONVERT(VARCHAR, '0') + CONVERT(VARCHAR, DATEPART(dd, DATEADD(dd, -1, FC.FechaAcum)))");
                query2.AppendLine(" WHEN LEN(DATEPART(mm, DATEADD(dd, -1, FC.FechaAcum))) = 2 AND LEN(DATEPART(dd, DATEADD(dd, -1, FC.FechaAcum))) = 2 THEN SUBSTRING(CONVERT(VARCHAR, DATEPART(yyyy, DATEADD(dd, -1, FC.FechaAcum))), 3, 2) + CONVERT(VARCHAR, DATEPART(mm, DATEADD(dd, -1, FC.FechaAcum))) + CONVERT(VARCHAR, DATEPART(dd, DATEADD(dd, -1, FC.FechaAcum)))");
                query2.AppendLine(" END AS UltimaFechaEtiquetar,");
                query2.AppendLine(" CASE WHEN LEN(DATEPART(mm, DATEADD(dd, -1, FI.FechaAcum))) = 1 AND LEN(DATEPART(dd, DATEADD(dd, -1, FI.FechaAcum))) = 1 THEN SUBSTRING(CONVERT(VARCHAR, DATEPART(yyyy, DATEADD(dd, -1, FI.FechaAcum))), 3, 2) + CONVERT(VARCHAR, '0') + CONVERT(VARCHAR, DATEPART(mm, DATEADD(dd, -1, FI.FechaAcum))) + CONVERT(VARCHAR, '0') + CONVERT(VARCHAR, DATEPART(dd, DATEADD(dd, -1, FI.FechaAcum)))");
                query2.AppendLine(" WHEN LEN(DATEPART(mm, DATEADD(dd, -1, FI.FechaAcum))) = 1 AND LEN(DATEPART(dd, DATEADD(dd, -1, FI.FechaAcum))) = 2 THEN SUBSTRING(CONVERT(VARCHAR, DATEPART(yyyy, DATEADD(dd, -1, FI.FechaAcum))), 3, 2) + CONVERT(VARCHAR, '0') + CONVERT(VARCHAR, DATEPART(mm, DATEADD(dd, -1, FI.FechaAcum))) + CONVERT(VARCHAR, DATEPART(dd, DATEADD(dd, -1, FI.FechaAcum)))");
                query2.AppendLine(" WHEN LEN(DATEPART(mm, DATEADD(dd, -1, FI.FechaAcum))) = 2 AND LEN(DATEPART(dd, DATEADD(dd, -1, FI.FechaAcum))) = 1 THEN SUBSTRING(CONVERT(VARCHAR, DATEPART(yyyy, DATEADD(dd, -1, FI.FechaAcum))), 3, 2) + CONVERT(VARCHAR, DATEPART(mm, DATEADD(dd, -1, FI.FechaAcum))) + CONVERT(VARCHAR, '0') + CONVERT(VARCHAR, DATEPART(dd, DATEADD(dd, -1, FI.FechaAcum)))");
                query2.AppendLine(" WHEN LEN(DATEPART(mm, DATEADD(dd, -1, FI.FechaAcum))) = 2 AND LEN(DATEPART(dd, DATEADD(dd, -1, FI.FechaAcum))) = 2 THEN SUBSTRING(CONVERT(VARCHAR, DATEPART(yyyy, DATEADD(dd, -1, FI.FechaAcum))), 3, 2) + CONVERT(VARCHAR, DATEPART(mm, DATEADD(dd, -1, FI.FechaAcum))) + CONVERT(VARCHAR, DATEPART(dd, DATEADD(dd, -1, FI.FechaAcum)))");
                query2.AppendLine(" END AS PrimeraFechaEtiquetar,");
                query2.AppendLine(" PrimerDiaEtiquetar = DATEPART(dd, DATEADD(dd, -1, FI.FechaAcum)),");
                query2.AppendLine(" UltimoDiaEtiquetar = DATEPART(dd, DATEADD(dd, -1, FC.FechaAcum)), diacorte = DATEPART(dd, FC.FechaAcum)");
                query2.AppendLine(" FROM ["+esquema+ "].[VisHistoricos('EtiqFechaCorte','Etiquetacion Fecha Corte','Español')] AS FC WITH (NOLOCK)");
                query2.AppendLine(" JOIN [" + esquema + "].[VisHistoricos('EtiqFechaInicioPeriodo','Etiquetacion Fecha Inicio Periodo','Español')] AS FI WITH (NOLOCK)");
                query2.AppendLine(" ON FC.Mes = FI.Mes");
                query2.AppendLine(" AND FI.dtIniVigencia <> FI.dtFinVigencia");
                query2.AppendLine(" AND FI.dtFinVigencia >= GETDATE()");
                query2.AppendLine(" WHERE FC.dtIniVigencia <> FC.dtFinVigencia");
                query2.AppendLine(" AND FC.dtFinVigencia >= GETDATE()");
                query2.AppendLine(" AND DATEPART(mm, FC.FechaAcum) = DATEPART(mm, GETDATE())");
                query2.AppendLine(" AND DATEPART(yyyy, FC.FechaAcum) = DATEPART(yyyy, GETDATE())");
                query2.AppendLine(" AND DATEPART(mm, FI.FechaAcum) = DATEPART(mm, GETDATE())");
                query2.AppendLine(" AND DATEPART(yyyy, FI.FechaAcum) = DATEPART(yyyy, GETDATE())");

               DataTable dt2 = DSODataAccess.Execute(query2.ToString(), connStr);
                if (dt2 != null && dt2.Rows.Count > 0)
                {
                    DataRow dr2 = dt2.Rows[0];
                    var_diacorte = Convert.ToInt32(dr2["diacorte"]);
                    var_fechacorte = dr2["fecha"].ToString();
                    var_diaIni = Convert.ToInt32(dr2["PrimerDiaEtiquetar"]);
                    var_fechaEIni = Convert.ToInt32(dr2["PrimeraFechaEtiquetar"]);
                    var_fechaRealcorte = dr2["UltimaFechaEtiquetar"].ToString();
                }
            }

            if(var_fechaRealcorte != null && var_fechaRealcorte != "")
            {
                var_fechaRealcorte = var_fechaRealcorte.Substring(4, 2) + "/" + var_fechaRealcorte.Substring(2, 2) + "/" + "20" + var_fechaRealcorte.Substring(0, 2);
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
            query.AppendLine(" FROM " + esquema + ".HistEmple AS E WITH (NOLOCK)");/*VARIABLE ESQUEMA*/
            //query.AppendLine(" FROM "+ esquema + ".[vishistoricos('Emple','Empleados','Español')] AS E WITH (NOLOCK)");/*VARIABLE ESQUEMA*/
            query.AppendLine(" JOIN "+ esquema + ".HistCenCos AS C WITH (NOLOCK)");
            //query.AppendLine(" JOIN "+ esquema + ".[vishistoricos('CenCos','Centro de costos','Español')] AS C WITH (NOLOCK)");
            query.AppendLine(" ON E.CenCos = C.iCodCatalogo");
            query.AppendLine(" AND C.dtIniVigencia <> C.dtFinVigencia");
            query.AppendLine(" AND C.dtFinVigencia >= GETDATE()");
            query.AppendLine(" JOIN " + esquema + ".[VisHistoricos('TipoEm','Tipo Empleado','Español')]AS TE WITH (NOLOCK)");
            query.AppendLine(" ON E.TipoEm = TE.iCodCatalogo");
            query.AppendLine(" AND TE.dtIniVigencia<> TE.dtFinVigencia");
            query.AppendLine(" AND TE.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE E.dtIniVigencia <> E.dtFinVigencia");
            query.AppendLine(" AND E.dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND E.iCodCatalogo = "+emId+"");/*VARIABLE EMPLEADO*/

            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            return dt;
        }
        private void ObtieneDatosExterno()
        {
            
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" CENCOSTO.vchCodigo,");
            query.AppendLine(" CENCOSTO.Descripcion");
            query.AppendLine(" FROM "+esquema+ ".[visRelaciones('FCA CentroCosto-Externo','Español')] AS EXTERN WITH (NOLOCK) ");
            query.AppendLine(" JOIN " + esquema + ".HistCenCos AS CENCOSTO WITH (NOLOCK) ");
            //query.AppendLine(" JOIN " + esquema + ".[vishistoricos('CenCos','Centro de costos','Español')] AS CENCOSTO WITH (NOLOCK) ");
            query.AppendLine(" ON EXTERN.CenCos = CENCOSTO.iCodCatalogo");
            query.AppendLine(" AND CENCOSTO.dtIniVigencia<> CENCOSTO.dtFinVigencia");
            query.AppendLine(" AND CENCOSTO.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE EXTERN.dtIniVigencia<> EXTERN.dtFinVigencia");
            query.AppendLine(" AND EXTERN.dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND EXTERN.Emple=" + emId + " ");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if(dt != null && dt.Rows.Count > 0)
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
            query.AppendLine(" SELECT TOP 1");
            query.AppendLine(" iCodCatalogo,");
            query.AppendLine(" FCANumeroLocalidad,");
            query.AppendLine(" FCANombreLocalidad");
            query.AppendLine(" FROM "+esquema+ ".[VisHistoricos('FCALocalidad','Localidades FCA','Español')] WITH (NOLOCK) ");
            query.AppendLine(" WHERE dtIniVigencia<> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND SUBSTRING(FCANumeroLocalidad,0,5)= '"+ numDepto + "'");
            query.AppendLine(" ORDER BY FCANumeroLocalidad ASC ");
            DataTable dt = DSODataAccess.Execute(query.ToString(),connStr);
            if( dt != null && dt.Rows.Count > 0)
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
        private void ObtieneSitiosEmple(ref List<SitiosEmple> empleSitios)
        {
            empleSitios.Clear();
            consumo.Clear();
            //fecini = 20190101;
            //fecFin = 20190131;
            //'QUERY PARA OBTENER EL NUMERO DE SITIOS EN LOS QUE TENGA CODIGO O EXTENSION EL EMPLEADO 
            //'La intenci�n de esto es saber en que tablas de detalle ha registrado consumo el empleado
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" iCodCatalogo AS Sitio,");
            query.AppendLine(" (BanderasSitio & 64) / 64 AS CobroLlamadasLocales");
            query.AppendLine(" FROM "+esquema+ ".HistSitio WITH (NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");

            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if(dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    SitiosEmple sitio = new SitiosEmple();
                    //sitio.Cencos = dr["cc_deteemp"].ToString();
                    sitio.CencosId = Convert.ToInt32(dr["Sitio"]);
                    sitio.CobroLlamLocales = Convert.ToInt32(dr["CobroLlamadasLocales"]);
                    empleSitios.Add(sitio);
                }
            }

        }       
        private DataTable OtieneDetallLlamLocales( int fecini,int fecFin, int emId)
        {
            DataTable dt = new DataTable();
            string sp = " EXEC FCAObtieneLlamLocales @Esquema = '{0}', @FechaIni = '{1}', @FechaFin = '{2}',@Emple = {3}";
            string query = string.Format(sp, esquema, fechaIni, fechaFin, emId);

            return dt = DSODataAccess.Execute(query.ToString(), connStr);
           
         }
        private void ObtieneTotLlam(int llamadas,int minutos,decimal costo,ref List<TotalConsumo> listConsumoTot)
        {
            TotalConsumo consTotalConsumo = new TotalConsumo();
            consTotalConsumo.Llamadas = "Total de llamadas: " + llamadas;
            consTotalConsumo.Minutos = "Total de minutos: " + minutos;
            consTotalConsumo.CostoTotal = "Total costo: " + "$"+costo.ToString("#0.00");
            listConsumoTot.Add(consTotalConsumo);
            
        }
        private DataTable ObtieneDetallCobrar(string tabla, int conmutadorID, int fecIni, int fecFin, int emId,int cobroLlamLoc)
        {
           /* '20140314.RJ.SEGUN LO PLATICADO CON LOURDES GARCIA, SOLO SE DEBEN COBRAR LAS LLAMADAS LOCALES CUANDO �STAS 
            'SE HAYAN REALIZADO DESDE UNO DE LOS CONMUTADORES DE SALTILLO, SIN IMPORTAR A D�NDE PERTENEZCA EL EMPLEADO
            'QUE LAS REALIZ� (ESTA CONDICI�N SOBRE-ESCRIBE LAS CONDICIONES ANTERIORES)*/
            //StringBuilder query = new StringBuilder();
            string sp = "EXEC FCAObtieneDetallCobrar @Esquema = '{0}',@FechaIni = '{1}',@FechaFin = '{2}',@ConmutadorID = {3},@Emple = {4},@CobroLlamLoc = {5}";
            string query = string.Format(sp, esquema, fechaIni, fechaFin, conmutadorID, emId, cobroLlamLoc);

            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            return dt;

        }
        private DataTable ObtieneConsumoTelenet(int fecIni, int fecFin, int emId)
        {
            string sp = "exec ObtieneConsumoTelenetUnEmpleado '{0}','{1}','{2}'";
            string query = string.Format(sp, fecIni, fecFin, emId);
            DataTable dt = DSODataAccess.Execute(query, connStr);
            return dt;
        }
        private void ObtieneDetalleLlamadas(string numMarcado,string extension,string tipo, string localidad,string tabla,int exId)
        {
            StringBuilder query = new StringBuilder();
            //string fecIniAlt = Session["fecIniAltFCA"].ToString();
            //string fecFinAlt = Session["fecFiniAltFCA"].ToString();
            int totalLlamadas = 0;
            int totalMinutos = 0;
            decimal costoTotal = 0;

            if(tabla.ToUpper() == "DETALLEEMPLEADOTELCEL")
            {
                //SE PIDIO QUE PARA EL CASO DE LOS MENSAJES DE TEXTO, NO SE REDONDEEN LOS IMPORTES, QUE APAREZCAN TAL CUAL.
                query.AppendLine(" SELECT");
                query.AppendLine(" Telefono AS de_extension,");
                query.AppendLine(" CONVERT(VARCHAR(10), FechaInicio, 121) AS de_fecha,");
                query.AppendLine(" CONVERT(VARCHAR(5), FechaInicio, 108) AS de_hora,");
                query.AppendLine(" TelDest AS de_nummarcado,");
                query.AppendLine(" DuracionMin AS de_duracion,");
                query.AppendLine(" ISNULL(LocalidadUsuario,'') AS de_localidad,");
                query.AppendLine(" CostoFac AS de_costo");
                query.AppendLine(" FROM "+esquema+ ".FCADetalleTelefoniaMovil WITH (NOLOCK)");
                query.AppendLine(" WHERE FechaInt = "+ fechaintcel + "");
                query.AppendLine(" AND Emple = " + emId + "");
                query.AppendLine(" AND TelDest = '" + numMarcado.Trim() + "'");
                query.AppendLine(" AND Telefono = '" + extension.Trim() + "'");
                query.AppendLine(" AND LocalidadUsuario LIKE '" + localidad + "'");
            }
            else
            {
                query.AppendLine(" SELECT");
                query.AppendLine(" Detall.Extension AS de_extension,");
                query.AppendLine(" CONVERT(VARCHAR(10), FechaInicio, 105) AS de_fecha,");
                query.AppendLine(" CONVERT(VARCHAR(5), FechaInicio, 108) AS de_hora,");
                query.AppendLine(" TelDest AS de_nummarcado,");
                query.AppendLine(" DuracionMin AS de_duracion,");
                query.AppendLine(" ISNULL(Etiqueta,'') AS de_localidad,");
                query.AppendLine(" ROUND(Costo + CostoSM, 1) AS de_costo");
                query.AppendLine(" FROM "+esquema+ ".[visDetallados('Detall','DetalleCDR','Español')] AS Detall WITH (NOLOCK)");
                query.AppendLine(" WHERE CONVERT(VARCHAR(8),FechaInicio,112) >= '" + fechaIni + "' ");
                query.AppendLine(" AND CONVERT(VARCHAR(8),FechaInicio,112) <= '" + fechaFin + "'");
                query.AppendLine(" AND Emple = "+ emId + "");
                query.AppendLine(" AND TelDest = '"+ numMarcado.Trim() + "'");
                query.AppendLine(" AND Extension = '"+ extension.Trim() + "'");
                if(localidad.Trim() == "")
                {
                    query.AppendLine(" AND UPPER(Etiqueta) LIKE ''");
                }
                else
                {
                    query.AppendLine(" AND UPPER(Etiqueta) LIKE '" + localidad + "'");
                }
                query.AppendLine(" AND Sitio = "+ Convert.ToInt32(tabla) + "");

            }


            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            List<DetalleLlamadas> listDetalle = new List<DetalleLlamadas>();
            listDetalle.Clear();

            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DetalleLlamadas li = new DetalleLlamadas();
                    li.Extension = dr["de_extension"].ToString().Trim();
                    li.Fecha = dr["de_fecha"].ToString();
                    li.Hora = dr["de_hora"].ToString();
                    string numero = dr["de_nummarcado"].ToString().Trim();
                    string numeroMarcado = ValidaNum(numero);
                    li.NumMarcado = numero;
                    li.Duracion = Convert.ToInt32(dr["de_duracion"]);
                    li.Localidad = dr["de_localidad"].ToString().Trim();
                    li.Costo = Convert.ToDecimal(dr["de_costo"]);
                    listDetalle.Add(li);

                    totalLlamadas += 1;
                    totalMinutos += Convert.ToInt32(dr["de_duracion"]);
                    costoTotal += Convert.ToDecimal(dr["de_costo"]);
                }
            }

            grdDetalle.DataSource = null;
            grdDetalle.DataBind();
            grdViewTotal.DataSource = null;
            grdViewTotal.DataBind();

            grdDetalle.DataSource = listDetalle;
            grdDetalle.DataBind();
            /*Calcula Totales*/
            List<TotalConsumo> listConsumoTot = new List<TotalConsumo>();
            listConsumoTot.Clear();
            ObtieneTotLlam(totalLlamadas, totalMinutos, costoTotal, ref listConsumoTot);
            grdViewTotal.DataSource = listConsumoTot;
            grdViewTotal.DataBind();

            if(listDetalle.Count > 0)
            {
                grdDetalle.UseAccessibleHeader = true;
                grdDetalle.HeaderRow.TableSection = TableRowSection.TableHeader;
                
            }
            grdDetalle.Columns[5].Visible = false;

            if (totalLlamadas > 5)
            {
                llamDetall.Style.Value = "height:210px;overflow-y:auto;overflow-x:auto;";
            }
            else
            {
                llamDetall.Style.Value = "overflow-y:auto;overflow-x:auto;";
            }
        }
        private string ObtieneConcepto(string localidad)
        {
            string concepto = "";
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT TOP 1 Descripcion FROM "+esquema+ ".HistFCAConceptoTelcelTipoLlamada WITH (NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND LTRIM(RTRIM(ConceptoTelcel)) LIKE LTRIM(RTRIM('"+ localidad.Trim() + "'))");

            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                concepto = dr["descripcion"].ToString();
            }
            return concepto;
        }
        private void ObtieneDetallCelLlamEntrada(string tabla,string NumMarcadoString,string NumExtensionString,string var_localidad)
        {
            int totalLlamadas = 0;
            int totalMinutos = 0;
            decimal costoTotal = 0;

            //Celulares LLAMADAS DE ENTRADA
            //SE PIDIO QUE PARA EL CASO DE LOS MENSAJES DE TEXTO, NO SE REDONDEEN LOS IMPORTES, QUE APAREZCAN TAL CUAL.
            StringBuilder query = new StringBuilder();
            query.AppendLine("select de_etiqueta, tl_idf,de_em_id, de_extension, de_ex_id, de_fecha, de_hora, " );
            query.AppendLine(" de_nummarcado, de_duracion, de_tipodestino, ");
            query.AppendLine(" de_costo,");
            query.AppendLine(" de_localidad from "+tabla +" ");
            query.AppendLine(" where de_em_id = "+emId + "");
            query.AppendLine(" and fechaint = "+fechaintcel+ " ");
            query.AppendLine(" and de_costo > 0 ");
            query.AppendLine(" and de_nummarcado = '"+NumMarcadoString+"' ");
            query.AppendLine(" and de_extension = '"+NumExtensionString+"' ");
            query.AppendLine(" and de_localidad like '"+var_localidad+"'");
            query.AppendLine(" and de_ex_id = 1");
            query.AppendLine(" order by de_fecha, de_hora");

            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            List<DetalleLlamadas> listDetalle = new List<DetalleLlamadas>();
            listDetalle.Clear();
            p3.Visible = false;
            if (dt != null && dt.Rows.Count > 0)
            {
                p3.Visible = true;
                foreach (DataRow dr in dt.Rows)
                {
                    DetalleLlamadas li = new DetalleLlamadas();
                    li.Extension = dr["de_extension"].ToString();
                    li.Fecha = dr["de_fecha"].ToString();
                    li.Hora = dr["de_hora"].ToString();
                    li.NumMarcado = dr["de_nummarcado"].ToString();
                    li.Duracion = Convert.ToInt32(dr["de_duracion"]);
                    li.Localidad = dr["de_localidad"].ToString();
                    li.Costo = Convert.ToDecimal(dr["de_costo"]);
                    listDetalle.Add(li);

                    totalLlamadas += 1;
                    totalMinutos += Convert.ToInt32(dr["de_duracion"]);
                    costoTotal += Convert.ToDecimal(dr["de_costo"]);
                }
            }

            grdDetalle.DataSource = null;
            grdDetalle.DataBind();
            grdViewTotal.DataSource = null;
            grdViewTotal.DataBind();

            grdDetalle.DataSource = listDetalle;
            grdDetalle.DataBind();
            /*Calcula Totales*/
            List<TotalConsumo> listConsumoTot = new List<TotalConsumo>();
            listConsumoTot.Clear();
            ObtieneTotLlam(totalLlamadas, totalMinutos, costoTotal, ref listConsumoTot);
            grdViewTotal.DataSource = listConsumoTot;
            grdViewTotal.DataBind();
        }
        private void ObtieneDetallCelLlamSalida(string tabla, string NumMarcadoString, string NumExtensionString, string var_localidad)
        {
            int totalLlamadas = 0;
            int totalMinutos = 0;
            decimal costoTotal = 0;

            StringBuilder query = new StringBuilder();
            query.AppendLine("select de_etiqueta, tl_idf,de_em_id, de_extension, de_ex_id, de_fecha, de_hora, ");
            query.AppendLine(" de_nummarcado, de_duracion, de_tipodestino, ");
            query.AppendLine(" de_costo, ");
            query.AppendLine(" de_localidad from "+tabla+"");
            query.AppendLine(" where de_em_id = "+emId+"");
            query.AppendLine(" and fechaint = "+fechaintcel + "");
            query.AppendLine(" and de_costo > 0");
            query.AppendLine(" and de_nummarcado = '"+NumMarcadoString+"'");
            query.AppendLine(" and de_extension = '"+NumExtensionString+"'");
            query.AppendLine(" and de_localidad like '"+var_localidad+"'");
            query.AppendLine(" and de_ex_id = 2 ");
            query.AppendLine(" order by de_fecha, de_hora");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            List<DetalleLlamadas> listDetalle = new List<DetalleLlamadas>();
            listDetalle.Clear();
            p4.Visible = false;
            if (dt != null && dt.Rows.Count > 0)
            {
                p4.Visible = true;
                foreach (DataRow dr in dt.Rows)
                {
                    DetalleLlamadas li = new DetalleLlamadas();
                    li.Extension = dr["de_extension"].ToString();
                    li.Fecha = dr["de_fecha"].ToString();
                    li.Hora = dr["de_hora"].ToString();
                    li.NumMarcado = dr["de_nummarcado"].ToString();
                    li.Duracion = Convert.ToInt32(dr["de_duracion"]);
                    li.Localidad = dr["de_localidad"].ToString();
                    li.Costo = Convert.ToDecimal(dr["de_costo"]);
                    listDetalle.Add(li);

                    totalLlamadas += 1;
                    totalMinutos += Convert.ToInt32(dr["de_duracion"]);
                    costoTotal += Convert.ToDecimal(dr["de_costo"]);
                }
            }

            grdLlamCelSal.DataSource = null;
            grdLlamCelSal.DataBind();
            gridTotal2.DataSource = null;
            gridTotal2.DataBind();

            grdLlamCelSal.DataSource = listDetalle;
            grdLlamCelSal.DataBind();
            /*Calcula Totales*/
            List<TotalConsumo> listConsumoTot = new List<TotalConsumo>();
            listConsumoTot.Clear();
            ObtieneTotLlam(totalLlamadas, totalMinutos, costoTotal, ref listConsumoTot);
            gridTotal2.DataSource = listConsumoTot;
            gridTotal2.DataBind();
        }
        private DataTable ObtieneDetallLlamLocalesGlobales(string tabla, int conmutadorID, int fecini, int fecFin, int emId,int cobroLlamadasLocales)
        {
            DataTable dt = new DataTable();

            StringBuilder query = new StringBuilder();

            query.AppendLine(" SELECT");
            query.AppendLine(" Extension AS de_extension,");
            query.AppendLine(" CONVERT(VARCHAR(10), FechaInicio, 103) AS de_fecha,");
            query.AppendLine(" CONVERT(VARCHAR(9), FechaInicio, 108) AS de_hora,");
            query.AppendLine(" DuracionMin AS de_duracion,");
            query.AppendLine(" TelDest AS de_nummarcado,");
            query.AppendLine(" ISNULL(LocaliDesc,'') AS de_localidadKeytia,");
            query.AppendLine(" ROUND(SUM(Costo + CostoSM), 1) AS de_costo");
            query.AppendLine(" FROM "+esquema+ ".[visDetallados('Detall','DetalleCDR','Español')] WITH (NOLOCK)");
            query.AppendLine(" WHERE Emple = " + emId + "");
            query.AppendLine(" AND CONVERT(VARCHAR(8),FechaInicio,112) >= '" + fecini + "'");
            query.AppendLine(" AND CONVERT(VARCHAR(8),FechaInicio,112) <= '" + fecFin + "'");
            query.AppendLine(" AND Sitio = "+ conmutadorID + "");

            if(cobroLlamadasLocales != 1)
            {
                query.AppendLine("AND TDestCod NOT IN ('001800','Local','EnlTie')");
            }
            query.AppendLine(" GROUP BY Extension,DuracionMin,TelDest,ISNULL(LocaliDesc,''),FechaInicio");
            /*query.AppendLine(" HAVING SUM(Costo + CostoSM) > 0");*/

            return dt = DSODataAccess.Execute(query.ToString(), connStr);
        }
        private void ObtieneDellGlobalLlamCelular(int emid, ref List<ConsumoCel> consumoCelular)
        {
            int totLlamada = 0;
            int minutos = 0;
            decimal costoTotal = 0;
            consumoCelular.Clear();
            ObtieneDetallLlamCelular(emid, ref consumoCelular);
            ObtieneDetallLlamCelular2(emid, ref consumoCelular);

            if(consumoCelular.Count > 0)
            {
                foreach (var item in consumoCelular)
                {
                    totLlamada += 1;
                    minutos += item.Duracion;
                    if(item.TipoDestino.ToUpper() != "LOCAL" && item.Idf != 2)/*Local*/
                    {
                        costoTotal += item.Costo;
                    }
                }

                if(totLlamada > 10)
                {
                    DetalleGlobalMoviles.Style.Value = "height:210px;overflow-y:auto;overflow-x:auto;";
                }
                else
                {
                    DetalleGlobalMoviles.Style.Value = "overflow-y:auto;overflow-x:auto;";
                }

                grdGlobalDetallMoviles.DataSource = null;
                grdGlobalDetallMoviles.DataBind();

                grdGlobalDetallMoviles.DataSource = consumoCelular;
                grdGlobalDetallMoviles.DataBind();

                if (costoTotal > 0)
                {
                    gridTotal2.DataSource = null;
                    gridTotal2.DataBind();

                    List<TotalConsumo> totales = new List<TotalConsumo>();
                    TotalConsumo consTotalConsumo = new TotalConsumo();
                    consTotalConsumo.Llamadas = "Total de llamadas Telefonía Móvil: " + totLlamada;
                    consTotalConsumo.Minutos = "Total de minutos: " + minutos;
                    consTotalConsumo.CostoTotal = "Total costo: " + "$" + costoTotal.ToString("#0.00");
                    totales.Add(consTotalConsumo);

                    gridTotal2.DataSource = totales;
                    gridTotal2.DataBind();
                }
            }

        }
        private void ObtieneDetallLlamCelular(int emid, ref List<ConsumoCel> consumoCelular)
        {
            string sp = "EXEC FCAObtieneDetallCelularGlobal @Esquema = '{0}',@Emple = {1},@FechaInt={2},@Opcion={3}";
            string query = string.Format(sp, esquema, emid, fechaintcel, 1);

            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if(dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ConsumoCel c = new ConsumoCel();
                    c.Extension = dr["de_extension"].ToString();
                    c.NumMarcado = dr["de_nummarcado"].ToString();
                    c.Duracion = Convert.ToInt32(dr["de_duracion"]);
                    c.Localidad = "";
                    c.LocalidadKeytia = dr["de_localidadKeytia"].ToString();
                    c.Costo = Convert.ToDecimal(dr["de_costo"]);
                    c.TipoDestino = dr["de_tipodestino"].ToString();
                    c.Idf = Convert.ToInt32(dr["tl_idf"]);
                    consumoCelular.Add(c);
                }
            }
        }
        private void ObtieneDetallLlamCelular2(int emid, ref List<ConsumoCel> consumoCelular)
        {

            string sp = "EXEC FCAObtieneDetallCelularGlobal @Esquema = '{0}',@Emple = {1},@FechaInt={2},@Opcion={3}";
            string query = string.Format(sp, esquema, emid, fechaintcel, 2);

            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ConsumoCel c = new ConsumoCel();
                    c.Extension = dr["de_extension"].ToString();
                    c.NumMarcado = dr["de_nummarcado"].ToString();
                    c.Duracion = Convert.ToInt32(dr["de_duracion"]);
                    c.Localidad = "";
                    c.LocalidadKeytia = dr["de_localidadKeytia"].ToString();
                    c.Costo = Convert.ToDecimal(dr["de_costo"]);
                    c.TipoDestino = dr["de_tipodestino"].ToString();
                    c.Idf = Convert.ToInt32(dr["tl_idf"]);
                    consumoCelular.Add(c);
                }
            }
        }
        private void ObtieneIcodEmple(int icodUsuario)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT E.iCodCatalogo FROM " + esquema + ".HistEmple AS E WITH (NOLOCK)");
            //query.AppendLine(" SELECT E.iCodCatalogo FROM " + esquema + ".[vishistoricos('Emple','Empleados','Español')] AS E WITH (NOLOCK)");
            query.AppendLine(" JOIN FCA.[VisHistoricos('Usuar','Usuarios','Español')] AS U WITH (NOLOCK)");
            query.AppendLine(" ON E.Usuar = U.iCodCatalogo");
            query.AppendLine(" WHERE E.dtIniVigencia <> E.dtFinVigencia AND E.dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND U.iCodCatalogo = "+ icodUsuario + "");
            DataTable dt = DSODataAccess.Execute(query.ToString(),connStr);
            if(dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                emId = Convert.ToInt32(dr["iCodCatalogo"]);
            }
        }
        private DataTable ObtieneMesEtiquetacionExcepcion(int usuario)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" DECLARE");
            query.AppendLine(" @Fecha DATE,");
            query.AppendLine(" @FechaIni DATE,");
            query.AppendLine(" @FechaFin DATE,");
            query.AppendLine(" @FechaInt VARCHAR(6)");
            query.AppendLine(" SELECT");
            query.AppendLine(" TOP 1 @Fecha = CONVERT(date,(AnioCod + '-' + MesCod + '-' + '1'))");
            query.AppendLine(" FROM "+ esquema + ".[VisHistoricos('FCAUsuarMesEtiquetacion','FCA Usuarios Mes Etiquetación','Español')]WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia<> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND Usuar = "+ usuario + "");
            query.AppendLine(" ORDER BY MesCod DESC, AnioCod DESC");
            query.AppendLine(" SET @FechaFin = CONVERT(VARCHAR(25), DATEADD(dd, -(DAY(DATEADD(mm, 1, @Fecha))), DATEADD(mm, 1, @Fecha)), 101)");
            query.AppendLine(" SET @FechaInt = CONVERT(VARCHAR, DATEADD(MONTH, -1, @Fecha), 112)");
            query.AppendLine(" SELECT CONVERT(VARCHAR(8),@Fecha,112) AS FechaIni, @Fecha AS FECHAINICIO,CONVERT(VARCHAR(8),@FechaFin,112) AS FechaFin,ISNULL(@FechaInt + 12,0) AS FechaInt");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
           
            return dt;
        }
        #endregion consultas

        protected void lnkVerDetall_Click(object sender, EventArgs e)
        {
            LinkButton lnkbtn1 = sender as LinkButton;
            GridViewRow gvrow1 = lnkbtn1.NamingContainer as GridViewRow;

            string numMarcado = grdLlamLocales.DataKeys[gvrow1.RowIndex].Values[0].ToString();
            string extension = grdLlamLocales.DataKeys[gvrow1.RowIndex].Values[1].ToString();
            string localidad = grdLlamLocales.DataKeys[gvrow1.RowIndex].Values[2].ToString();
            string tablaConsulta = grdLlamLocales.DataKeys[gvrow1.RowIndex].Values[3].ToString();

            MuestraDetalleLlamadas(numMarcado, extension, "0", localidad, tablaConsulta, 0);

        }

        protected void chkMostrar_CheckedChanged(object sender, EventArgs e)
        {
            decimal LlamPersonal = 0;
            decimal LlamLaboral = 0;

            if(chkMostrar.Checked == true) //si el checkbox esta checado
            {
                for (int i = 0; i < grvLlamadas.Rows.Count; i++)
                {
                    var checkbox = grvLlamadas.Rows[i].FindControl("chkRow") as System.Web.UI.WebControls.CheckBox;
                    decimal importe = Convert.ToDecimal(grvLlamadas.DataKeys[i].Values[3]);

                    if (checkbox.Enabled == true)
                    {
                        checkbox.Checked = true;
                    }

                    if (checkbox.Checked == true)
                    {
                        //suma el costo de las llamadas de negocio
                        LlamLaboral += importe;
                    }
                    else
                    {
                        //suma los totatales de las llamadas personales
                        LlamPersonal += importe;
                    }
                }

                if(row4.Visible ==  true)
                {
                    for (int i = 0; i < grdMovil.Rows.Count; i++)
                    {
                        var checkbox = grdMovil.Rows[i].FindControl("chkRow") as System.Web.UI.WebControls.CheckBox;
                        decimal importe = Convert.ToDecimal(grdMovil.DataKeys[i].Values[3]);

                        if (checkbox.Enabled == true)
                        {
                            checkbox.Checked = true;
                        }

                        if (checkbox.Checked == true)
                        {
                            //suma el costo de las llamadas de negocio
                            LlamLaboral += importe;
                        }
                        else
                        {
                            //suma los totatales de las llamadas personales
                            LlamPersonal += importe;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < grvLlamadas.Rows.Count; i++)
                {
                    var checkbox = grvLlamadas.Rows[i].FindControl("chkRow") as System.Web.UI.WebControls.CheckBox;
                    decimal importe = Convert.ToDecimal(grvLlamadas.DataKeys[i].Values[3]);

                    if (checkbox.Enabled == true)
                    {
                        checkbox.Checked = false;
                    }
                    if (checkbox.Checked == true)
                    {
                        //suma el costo de las llamadas de negocio
                        LlamLaboral += importe;
                    }
                    else
                    {
                        //suma los totatales de las llamadas personales
                        LlamPersonal += importe;
                    }
                }

                if (row4.Visible == true)
                {
                    for (int i = 0; i < grdMovil.Rows.Count; i++)
                    {
                        var checkbox = grdMovil.Rows[i].FindControl("chkRow") as System.Web.UI.WebControls.CheckBox;
                        decimal importe = Convert.ToDecimal(grdMovil.DataKeys[i].Values[3]);

                        if (checkbox.Enabled == true)
                        {
                            checkbox.Checked = false;
                        }

                        if (checkbox.Checked == true)
                        {
                            //suma el costo de las llamadas de negocio
                            LlamLaboral += importe;
                        }
                        else
                        {
                            //suma los totatales de las llamadas personales
                            LlamPersonal += importe;
                        }
                    }
                }
            }

           
            spnTotalLlam.InnerText = LlamPersonal.ToString("#0.00");
            ///*importe negocio*/
            SpanTotalNeg.InnerText = LlamLaboral.ToString("#0.00");

        }

        protected void chkRow_CheckedChanged(object sender, EventArgs e)
        {
            decimal negocio = Convert.ToDecimal(SpanTotalNeg.InnerText);
            decimal personal = Convert.ToDecimal(spnTotalLlam.InnerText);

            System.Web.UI.WebControls.CheckBox chk = sender as System.Web.UI.WebControls.CheckBox;
            GridViewRow gvrow1 = chk.NamingContainer as GridViewRow;
            string total = grvLlamadas.DataKeys[gvrow1.RowIndex].Values[3].ToString();
            string numM = grvLlamadas.DataKeys[gvrow1.RowIndex].Values[1].ToString();
            int posicion = gvrow1.RowIndex;

            decimal p = 0;
            decimal n = 0;
            decimal tot = 0;
            if (chk.Checked == false)/*Calcula las llamadas personales*/
            {
                chkMostrar.Checked = false;
                /*Validacion para saber si algun registro tiene el mismo numero marcado
                 que el registro seleccionado, si es asi el registro  deselecciona y se agrega el importe al 
                 total personal*/
                for (int i = 0; i < grvLlamadas.Rows.Count; i++)
                {
                    var checkbox = grvLlamadas.Rows[i].FindControl("chkRow") as System.Web.UI.WebControls.CheckBox;
                    string numMarcado = grvLlamadas.DataKeys[i].Values[1].ToString();
                    decimal t =  Convert.ToDecimal(grvLlamadas.DataKeys[i].Values[3]);

                    if (numMarcado == numM && i != posicion && checkbox.Checked == true)
                    {
                        checkbox.Checked = false;
                        tot = t;
                    }
                }

                p = personal + Convert.ToDecimal(total) + tot;
                n = negocio - Convert.ToDecimal(total) - tot;
            }
            else/*Calcula las llamdas laborales*/
            {

                /*Validacion para saber si algun registro tiene el mismo numero marcado
                 que el registro seleccionado, si es asi el registro se selecciona y se agrega el importe al 
                 total de negocio*/
                for (int i = 0; i < grvLlamadas.Rows.Count; i++)
                {
                    var checkbox = grvLlamadas.Rows[i].FindControl("chkRow") as System.Web.UI.WebControls.CheckBox;
                    string numMarcado = grvLlamadas.DataKeys[i].Values[1].ToString();
                    decimal t = Convert.ToDecimal(grvLlamadas.DataKeys[i].Values[3]);

                    if (numMarcado == numM && i != posicion && checkbox.Checked == false)
                    {
                        checkbox.Checked = true;
                        tot = t;
                    }
                }

                 n = negocio + Convert.ToDecimal(total) + tot;
                 p = personal - Convert.ToDecimal(total) - tot;
            }

            ////Falta Realizar ciertas Validaciones para este dato importe personal aceptado
            spnTotalLlam.InnerText = p.ToString("#0.00");
            ///*importe negocio*/
            SpanTotalNeg.InnerText = n.ToString("#0.00");
        }

        protected void chkRow_CheckedChanged1(object sender, EventArgs e)
        {
            decimal negocio = Convert.ToDecimal(SpanTotalNeg.InnerText);
            decimal personal = Convert.ToDecimal(spnTotalLlam.InnerText);

            System.Web.UI.WebControls.CheckBox chk = sender as System.Web.UI.WebControls.CheckBox;
            GridViewRow gvrow1 = chk.NamingContainer as GridViewRow;
            string total = grdMovil.DataKeys[gvrow1.RowIndex].Values[3].ToString();
            string numM = grdMovil.DataKeys[gvrow1.RowIndex].Values[0].ToString();
            int posicion = gvrow1.RowIndex;

            decimal p = 0;
            decimal n = 0;
            decimal tot = 0;
            if (chk.Checked == false)/*Calcula las llamadas personales*/
            {
                /*Validacion para saber si algun registro tiene el mismo numero marcado
                 que el registro seleccionado, si es asi el registro  deselecciona y se agrega el importe al 
                 total personal*/
                for (int i = 0; i < grdMovil.Rows.Count; i++)
                {
                    var checkbox1 = grdMovil.Rows[i].FindControl("chkRow") as System.Web.UI.WebControls.CheckBox;
                    string numMarcado = grdMovil.DataKeys[i].Values[1].ToString();
                    decimal t = Convert.ToDecimal(grdMovil.DataKeys[i].Values[3]);

                    if (numMarcado == numM && i != posicion && checkbox1.Checked == true)
                    {
                        checkbox1.Checked = false;
                        tot = t;
                    }
                }

                p = personal + Convert.ToDecimal(total) + tot;
                n = negocio - Convert.ToDecimal(total) - tot;
            }
            else/*Calcula las llamadas laborales*/
            {
                /*Validacion para saber si algun registro tiene el mismo numero marcado
                 que el registro seleccionado, si es asi el registro se selecciona y se agrega el importe al 
                 total de negocio*/
                for (int i = 0; i < grdMovil.Rows.Count; i++)
                {
                    var checkbox2 = grdMovil.Rows[i].FindControl("chkRow") as System.Web.UI.WebControls.CheckBox;
                    string numMarcado = grdMovil.DataKeys[i].Values[1].ToString();
                    decimal t = Convert.ToDecimal(grdMovil.DataKeys[i].Values[3]);

                    if (numMarcado == numM && i != posicion && checkbox2.Checked == false)
                    {
                        checkbox2.Checked = true;
                        tot = t;
                    }
                }

                n = negocio + Convert.ToDecimal(total) + tot;
                p = personal - Convert.ToDecimal(total) - tot;
            }

            ////Falta Realizar ciertas Validaciones para este dato importe personal aceptado
            spnTotalLlam.InnerText = p.ToString("#0.00");
            ///*importe negocio*/
            SpanTotalNeg.InnerText = n.ToString("#0.00");
        }

        protected void lnkVerDetalle_Click(object sender, EventArgs e)
        {
            LinkButton lnkbtn1 = sender as LinkButton;
            GridViewRow gvrow1 = lnkbtn1.NamingContainer as GridViewRow;

            string numMarcado = grvLlamadas.DataKeys[gvrow1.RowIndex].Values[1].ToString();
            string extension = grvLlamadas.DataKeys[gvrow1.RowIndex].Values[4].ToString();
            string tablaConsulta = grvLlamadas.DataKeys[gvrow1.RowIndex].Values[5].ToString();
            string localidad = grvLlamadas.DataKeys[gvrow1.RowIndex].Values[6].ToString();
            MuestraDetalleLlamadas(numMarcado, extension, "0", localidad, tablaConsulta, 0);
        }

        protected void lnkVerDetalle_Click1(object sender, EventArgs e)
        {
            LinkButton lnkbtn1 = sender as LinkButton;
            GridViewRow gvrow1 = lnkbtn1.NamingContainer as GridViewRow;

            string numMarcado = grdMovil.DataKeys[gvrow1.RowIndex].Values[1].ToString();
            string extension = grdMovil.DataKeys[gvrow1.RowIndex].Values[4].ToString();
            string localidad = grdMovil.DataKeys[gvrow1.RowIndex].Values[5].ToString();
            string tablaConsulta = "detalleempleadotelcel";//grdMovil.DataKeys[gvrow1.RowIndex].Values[5].ToString();
            /*se realizaran otras condiciones para el detalle de llamadas celular*/
            string concepto = ObtieneConcepto(localidad);
            if(concepto != "DATOS INT USAYCAN D")
            {
                p2.InnerText = concepto;
            }

            MuestraDetalleLlamadas(numMarcado, extension, "0", localidad, tablaConsulta, 0);
            if(numMarcado.ToUpper() == "")
            {
                p5.Visible = true;
            }
            else
            {
                p5.Visible = false;
            }

        }

        protected void btnDetalle_Click(object sender, EventArgs e)
        {
            var fecini = Convert.ToInt32(Session["fecIniFCA"]);
            var fecFin = Convert.ToInt32(Session["fecFinFCA"]);
            ObtieneDetalleLlamadasGlobal(emId, fecini, fecFin, ref empleSitios, ref consumo);
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            row2.Visible = false;
            rowTexto.Visible = false;
            panel2.Visible = false;
            lblTotGlobalLlam.Visible = false;
            lblTiempoTotGlobal.Visible = false;

            /*HACER UN CICLO QUE RECORRA TODOS LOS GRIDVIEWS EXCEPTO EL DE LLAMADAS LOCALES*/            
            try
            {
                AceptaImportesLLam();
                lblTituloModalMsn.Text = "Mensaje!";
                lblBodyModalMsn.Text = "Los Importes se Aceptaron Correctamente.";
                mpeEtqMsn.Show();
                btnGuardar.Enabled = false;
            }
            catch(Exception ex)
            {
                lblTituloModalMsn.Text = "Error!!";
                lblBodyModalMsn.Text = "Ocurrio Un Error al Aceptar los importes, intentelo mas tarde.";
                mpeEtqMsn.Show();
                btnGuardar.Enabled = true;
                return;
            }
           
        }
        #region AceptarImportes
        public void AceptaImportesLLam()
        {
            try
            {
                /*OBTIENE EL LISTADO DE SITIOS  */
                ObtieneSitiosEmple(ref empleSitios);
                if (grvLlamadas.Rows.Count > 0)
                {
                    for (int i = 0; i < grvLlamadas.Rows.Count; i++)
                    {
                        var checkbox = grvLlamadas.Rows[i].FindControl("chkRow") as System.Web.UI.WebControls.CheckBox;
                        string numMarcado = grvLlamadas.DataKeys[i].Values[1].ToString();
                        int tablaConsulta = Convert.ToInt32(grvLlamadas.DataKeys[i].Values[5].ToString());
                        var cobroLlamadasLocales = empleSitios.FirstOrDefault(x => x.CencosId == tablaConsulta);
                        var localidad = grvLlamadas.Rows[i].FindControl("txtRferencia") as System.Web.UI.WebControls.TextBox;
                        var lo = localidad.Text;
                        int cobro = cobroLlamadasLocales.CobroLlamLocales;
                        if (checkbox.Enabled == true)/*si el check box esta habilitado es una llamada que aun no se ah etiquetado*/
                        {
                            /*validar si el checkbox esta checado es una llamada laboral*/
                            string de_localidad = "";
                            string de_TipoDestino = "";
                            string de_TDest = "";
                            if (checkbox.Checked == true)
                            {
                                ActualizaDetalle(numMarcado, cobro, lo, tablaConsulta, 1);
                                DataTable dt = ObtieneData(numMarcado, cobro, lo, tablaConsulta);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    DataRow dr = dt.Rows[0];
                                    de_localidad = dr["Etiqueta"].ToString();
                                    de_TipoDestino = dr["TDestDesc"].ToString();
                                    de_TDest = dr["TDest"].ToString();
                                }
                                else
                                {
                                    de_localidad = "N/A";
                                    de_TipoDestino = "N/A";
                                    de_TDest = "null";
                                }

                                int veces = ValidaNumeroLaboral(numMarcado);
                                /*'Insertamos el Numero Marcado en la tabla de telefonoLaboral, 
                                'siempre y cuando no exista un registro de ese numero marcado y ese empleado.
                                'Se insertan para que sean tomados en cuenta por los procesos nocturnos*/
                                if (veces == 0)
                                {
                                    InsertaTelefonoLaboral(numMarcado, lo, de_TDest);
                                }
                                /*'Tambi�n se requiere insertar para Numero Localidad
                                'Primero actualizamos cualquier registro que exista en numerolocalidad 
                                'que haya etiquetado ese n�mero como personal. 
                                'Si no existe, simplemente no actualiza nada.*/
                                ActualizaNumLocalidad(numMarcado, 2);

                                /*'Despues validamos que no exista ya ese n�mero y posteriormente lo instertamos.*/
                                ValidaTelefonoLocalidad(numMarcado, lo, 2);

                                /*'SE INSERTAN TODOS LOS TELEFONOS ETIQUETADOS EN LA TABLA REGISTRONUMEROETIQUETADO, 
                                'EN ESTE CASO CON EL CAMPO TL_IDF IGUAL A 2, EN ESTA TABLA SE INSERTAR�N LOS 
                                ' NUMEROS MARCADOS SE ETIQUETEN COMO LABORALES O NO, 
                                'YA QUE SIRVE COMO UN HISTORICO DE LOS MOVIMIENTOS QUE REALIZAN LOS USUARIOS.*/

                                InsertaNumerosEtiquetados(numMarcado, 2);

                            }
                            else/*si el chekbox no esta checado es una llamada personal*/
                            {
                                /*'Si la llamada es personal, solo actualizamos la tabla de detalle correspondiente*/
                                ActualizaDetalle(numMarcado, cobro, lo, tablaConsulta, 0);

                                /*'Primero actualizamos cualquier registro que exista en numerolocalidad que haya etiquetado ese n�mero como laboral. 
                                'Si no existe, simplemente no actualiza nada.*/
                                ActualizaNumLocalidad(numMarcado, 1);
                                /* 'Despues validamos que no exista y posteriormente lo instertamos */
                                ValidaTelefonoLocalidad(numMarcado, lo, 1);

                                /*' LA INTENCION DE BORRAR LOS REGISTROS DE LA TABLA TELEFONOLABORAL DONDE EL NUMERO MARCADO NO SEA ETIQUETADO COMO LABORAL, ES QUE CUANDO SE PROCESE NUEVA INFORMACION
                                   ' NO SE TOME EN CUENTA ESE NUMERO Y SE MARQUE COMO LLAMADA PERSONAL. ANTERIORMENTE NO SE ELIMINABA DE ESTA TABLA AQUELLOS REGISTROS QUE YA EXISTIERAN AUNQUE NO SE MARCARAN
                                   ' COMO LABORALES, CON ELLO, AUNQUE EN EL MES QUE SE EST� ETIQUETANDO SE CAMBIABA EL TIPO DE LLAMADA, PARA FUTURAS REFERENCIAS SEGUIA APARECIENDO COMO LABORAL.
                                   ' SI SE REQUIERE ALGUN INFORME SOBRE LOS CAMBIOS QUE HUBIERA SUFRIDO UN NUMERO MARCADO SE UTILIZAR� LA TABLA "REGISTRONUMEROETIQUETADO".*/
                                EliminaRegTelefonoLaboral(numMarcado);
                                /*'SE INSERTAN TODOS LOS TELEFONOS ETIQUETADOS EN LA TABLA REGISTRONUMEROETIQUETADO, EN ESTE CASO CON EL CAMPO TL_IDF IGUAL A 1, EN ESTA TABLA SE INSERTAR�N LOS 
                                ' NUMEROS MARCADOS SE ETIQUETEN COMO LABORALES O NO, YA QUE SIRVE COMO UN HISTORICO DE LOS MOVIMIENTOS QUE REALIZAN LOS USUARIOS.*/
                                InsertaNumerosEtiquetados(numMarcado, 1);
                            }

                        }

                        checkbox.Enabled = false;
                        localidad.Enabled = false;
                    }
                }
                /*CONSUMO DE TELEFONIA MOVIL*/
                if (grdMovil.Rows.Count > 0)
                {
                    for (int i = 0; i < grdMovil.Rows.Count; i++)
                    {
                        var checkbox1 = grdMovil.Rows[i].FindControl("chkRow") as System.Web.UI.WebControls.CheckBox;
                        string numMarcadoMovil = grdMovil.DataKeys[i].Values[1].ToString();
                        //string localidad = grdMovil.DataKeys[i].Values[2].ToString();
                        // int tablaConsulta = Convert.ToInt32(grdMovil.DataKeys[i].Values[5].ToString());
                        //var cobroLlamadasLocales = empleSitios.FirstOrDefault(x => x.CencosId == tablaConsulta);
                        var localidad1 = grdMovil.Rows[i].FindControl("txtRferencia") as System.Web.UI.WebControls.TextBox;
                        var lo1 = localidad1.Text;
                        // int cobro = cobroLlamadasLocales.CobroLlamLocales;

                        if (checkbox1.Enabled == true)/*si el check box esta habilitado es una llamada que aun no se ah etiquetado*/
                        {
                            string llamada_localidad = "";
                            string llamada_tipodestino = "";
                            string de_TDestMovil = "";
                            if (checkbox1.Checked == true)/*si el checkbox esta checado es porque es una llamada laboral*/
                            {
                                ActualizaTelefoniaMovil(numMarcadoMovil, lo1, 1);
                                DataTable dt = ObtieneDatoMovil(numMarcadoMovil, lo1);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    DataRow dr = dt.Rows[0];
                                    llamada_localidad = dr["LocalidadUsuario"].ToString();
                                    llamada_tipodestino = dr["vchDescripcion"].ToString();
                                    de_TDestMovil = dr["TDest"].ToString();
                                }
                                else
                                {
                                    llamada_localidad = "N/A";
                                    llamada_tipodestino = "N/A";
                                    de_TDestMovil = "null";
                                }


                                int veces = ValidaNumeroLaboral(numMarcadoMovil);
                                /*'Insertamos el Numero Marcado en la tabla de telefonoLaboral, 
                                'siempre y cuando no exista un registro de ese numero marcado y ese empleado.
                                'Se insertan para que sean tomados en cuenta por los procesos nocturnos*/
                                if (veces == 0)
                                {
                                    InsertaTelefonoLaboral(numMarcadoMovil, lo1, de_TDestMovil);
                                }

                                /*'Tambi�n se requiere insertar para Numero Localidad
                                'Primero actualizamos cualquier registro que exista en numerolocalidad que haya etiquetado ese n�mero como personal. 
                                'Si no existe, simplemente no actualiza nada.*/
                                ActualizaNumLocalidad(numMarcadoMovil, 2);/*se manda el 2 ya que se valida si el numero fue etiquetado como personal, porque se etiquetara como laboral*/

                                /*'Despues validamos que no exista ya ese n�mero y posteriormente lo instertamos.*/
                                ValidaTelefonoLocalidad(numMarcadoMovil, lo1, 2);

                                /*'SE INSERTAN TODOS LOS TELEFONOS ETIQUETADOS EN LA TABLA REGISTRONUMEROETIQUETADO, EN ESTE CASO CON EL CAMPO TL_IDF IGUAL A 2, EN ESTA TABLA SE INSERTAR�N LOS 
                                ' NUMEROS MARCADOS SE ETIQUETEN COMO LABORALES O NO, YA QUE SIRVE COMO UN HISTORICO DE LOS MOVIMIENTOS QUE REALIZAN LOS USUARIOS.*/
                                InsertaNumerosEtiquetados(numMarcadoMovil, 2);
                            }
                            else /* si no esta seleccionado es porque es una llamada personal*/
                            {

                                /*Llamada Personal
                                 * 'Si la llamada es personal, solo actualizamos la tabla de detalle correspondiente
                                 */
                                ActualizaTelefoniaMovil(numMarcadoMovil, lo1, 0);

                                /*' LA INTENCION DE BORRAR LOS REGISTROS DE LA TABLA TELEFONOLABORAL DONDE EL NUMERO MARCADO NO SEA ETIQUETADO COMO LABORAL, ES QUE CUANDO SE PROCESE NUEVA INFORMACION
                                ' NO SE TOME EN CUENTA ESE NUMERO Y SE MARQUE COMO LLAMADA PERSONAL. ANTERIORMENTE NO SE ELIMINABA DE ESTA TABLA AQUELLOS REGISTROS QUE YA EXISTIERAN AUNQUE NO SE MARCARAN
                                ' COMO LABORALES, CON ELLO, AUNQUE EN EL MES QUE SE EST� ETIQUETANDO SE CAMBIABA EL TIPO DE LLAMADA, PARA FUTURAS REFERENCIAS SEGUIA APARECIENDO COMO LABORAL.
                                ' SI SE REQUIERE ALGUN INFORME SOBRE LOS CAMBIOS QUE HUBIERA SUFRIDO UN NUMERO MARCADO SE UTILIZAR� LA TABLA "REGISTRONUMEROETIQUETADO".*/
                                EliminaRegTelefonoLaboral(numMarcadoMovil);

                                /*'Tambi�n se requiere insertar para Numero Localidad
                                'Primero actualizamos cualquier registro que exista en numerolocalidad que haya etiquetado ese n�mero como laboral. 
                                'Si no existe, simplemente no actualiza nada.*/
                                ActualizaNumLocalidad(numMarcadoMovil, 1);
                                /* 'Despues validamos que no exista y posteriormente lo insertamos */
                                ValidaTelefonoLocalidad(numMarcadoMovil, lo1, 1);

                                /*'SE INSERTAN TODOS LOS TELEFONOS ETIQUETADOS EN LA TABLA REGISTRONUMEROETIQUETADO, EN ESTE CASO CON EL CAMPO TL_IDF IGUAL A 1, EN ESTA TABLA SE INSERTAR�N LOS 
                                ' NUMEROS MARCADOS SE ETIQUETEN COMO LABORALES O NO, YA QUE SIRVE COMO UN HISTORICO DE LOS MOVIMIENTOS QUE REALIZAN LOS USUARIOS.*/
                                InsertaNumerosEtiquetados(numMarcadoMovil, 1);

                            }
                        }

                        checkbox1.Enabled = false;
                        localidad1.Enabled = false;
                    }
                }
                /*INSERTA LOS IMPORTES */
                InsertaImportes();
            }
            catch
            {
                return;
                throw new Exception("Ocurrio un error");
  
            }
        }
        public void ActualizaDetalle(string numMarcado,int cobroLlamadasLocales,string localidad,int sitio,int tipoCobro)
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine(" DECLARE @TipoEtiq INT");
                if (tipoCobro == 0)
                {
                    query.AppendLine(" SET @TipoEtiq = 1 /*'1Personal'*/");
                }
                else if (tipoCobro == 1)
                {
                    query.AppendLine(" SET @TipoEtiq = 2 /*'2Laboral'*/");
                }

                query.AppendLine(" UPDATE D");
                query.AppendLine(" SET D.GEtiqueta = @TipoEtiq ,Etiqueta = '" + localidad.Trim() + "',");
                query.AppendLine(" dtFecha = GETDATE(),iCodUsuario = " + iCodUsuario + ",dtFecUltAct = GETDATE()");
                query.AppendLine(" FROM " + esquema + ".[visDetallados('Detall','DetalleCDR','Español')] AS D WITH (NOLOCK) ");
                query.AppendLine(" JOIN " + esquema + ".HistSitio AS S WITH (NOLOCK) ");
                query.AppendLine(" ON D.Sitio = S.iCodCatalogo");
                query.AppendLine(" AND S.dtIniVigencia <> S.dtFinVigencia");
                query.AppendLine(" AND S.dtFinVigencia >= GETDATE()");
                query.AppendLine(" WHERE CONVERT(VARCHAR(8),FechaInicio,112) >= '" + fechaIni + "' AND CONVERT(VARCHAR(8),FechaInicio,112) <= '" + fechaFin + "'");
                query.AppendLine(" AND D.Emple = " + emId + "");
                //query.AppendLine(" AND LTRIM(RTRIM(TelDest)) LIKE '%" + numMarcado.Trim() + "%'");
                query.AppendLine(" AND LTRIM(RTRIM(TelDest)) = '" + numMarcado.Trim() + "'");
                query.AppendLine(" AND D.Sitio = " + sitio + "");
                query.AppendLine(" AND Costo > 0");
                if (cobroLlamadasLocales == 0)
                {
                    /*descartar en la consulta los tipos de destino 001800 y Local*/
                    query.AppendLine(" AND TDestCod NOT IN ('001800','Local','EnlTie')");
                }

                DSODataAccess.ExecuteNonQuery(query.ToString(), connStr);
            }
            catch
            {
                return;
            }
        }
        public void ActualizaTelefoniaMovil(string telDest,string localidad,int tipoCobro)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(" DECLARE @TipoEtiq INT");
           
            if (tipoCobro == 0)
            {
                query.AppendLine(" SET @TipoEtiq = 1  /*'1Personal'*/");
            }
            else if (tipoCobro == 1)
            {
                query.AppendLine(" SET @TipoEtiq = 2  /*'2Laboral'*/");
            }
            query.AppendLine(" UPDATE D");
            query.AppendLine(" SET D.GEtiqueta = @TipoEtiq,");
            query.AppendLine(" LocalidadUsuario = '"+ localidad.Trim() + "',");
            query.AppendLine(" dtFecUltAct = GETDATE(),");
            query.AppendLine(" iCodUsuario = "+ iCodUsuario + "");
            query.AppendLine(" FROM "+ esquema + ".FCADetalleTelefoniaMovil AS D WITH (NOLOCK) ");
            query.AppendLine(" JOIN " + esquema + ".HistTdest AS T WITH (NOLOCK)");
            query.AppendLine(" ON D.TDest = T.iCodCatalogo");
            query.AppendLine(" AND T.dtIniVigencia <> T.dtFinVigencia");
            query.AppendLine(" AND T.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE Emple = "+ emId + "");
            query.AppendLine(" AND TelDest = '"+ telDest.Trim() + "'");
            query.AppendLine(" AND LTRIM(RTRIM(LocalidadUsuario)) = '"+ localidad.Trim()+ "'");
            query.AppendLine(" AND T.vchCodigo NOT IN('001800','Local','EnlTie')");
            query.AppendLine(" AND FechaInt = "+fechaintcel.Trim()+"");

            DSODataAccess.ExecuteNonQuery(query.ToString(),connStr);
        }
        public DataTable ObtieneData(string numMarcado, int cobroLlamadasLocales, string localidad, int sitio)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT TOP 1 Etiqueta,TDest,TDestDesc FROM "+esquema+ ".[visDetallados('Detall','DetalleCDR','Español')] WITH (NOLOCK)");
            query.AppendLine(" WHERE FechaInicio >= '"+ fechaIni + "' AND FechaInicio <= '"+fechaFin+"'");
            query.AppendLine(" AND Emple = "+ emId + "");
            query.AppendLine(" AND Sitio = "+ sitio + "");
            query.AppendLine(" AND Costo > 0");
            query.AppendLine(" AND TelDest = '"+ numMarcado + "'");
            if (cobroLlamadasLocales == 0)
            {
                /*descartar en la consulta los tipos de destino 001800 y Local*/
                query.AppendLine(" AND TDestCod NOT IN ('001800','Local','EnlTie')");
            }

            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            return dt;
        }
        public DataTable ObtieneDatoMovil(string numMarcado, string localidad)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT TOP 1 LocalidadUsuario,D.TDest,T.vchDescripcion");
            query.AppendLine(" FROM "+esquema+ ".FCADetalleTelefoniaMovil AS D WITH (NOLOCK)");
            query.AppendLine(" JOIN " + esquema + ".HistTdest AS T WITH (NOLOCK) ");
            query.AppendLine(" ON D.TDest = T.iCodCatalogo");
            query.AppendLine(" AND T.dtIniVigencia <> T.dtFinVigencia");
            query.AppendLine(" ANd T.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE TelDest = '"+ numMarcado + "'");
            query.AppendLine(" AND FechaInt = "+fechaintcel+"");
            query.AppendLine(" AND Emple = " + emId + "");
            query.AppendLine(" AND LTRIM(RTRIM(LocalidadUsuario)) ='"+localidad.Trim() + "'");

            DataTable dt = DSODataAccess.Execute(query.ToString(),connStr);
            return dt;
        }
        private int ValidaNumeroLaboral(string numMarcado)
        {
            int cantidad = 0;
            StringBuilder query = new StringBuilder();

            query.AppendLine(" SELECT COUNT(*) AS VecesEnTelefonoLaboral ");
            query.AppendLine(" FROM "+esquema+ ".TelefonoLaboral WITH (NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND  NumeroTelefonico ='" + numMarcado.Trim() +"'");
            query.AppendLine(" AND Emple = "+emId+"");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            if(dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                cantidad = Convert.ToInt32(dr["VecesEnTelefonoLaboral"]);

            }
            return cantidad;
        }
        private void InsertaTelefonoLaboral(string telefono,string etiqueta,string tDest)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(" INSERT INTO "+esquema+".TelefonoLaboral");
            query.AppendLine("VALUES('"+ telefono.Trim() + "','"+ etiqueta.Trim() + "',GETDATE(),"+ emId + ", "+ iCodUsuario +","+ tDest + ",1,'2011-01-01 00:00:00.000','2079-01-01 00:00:00.000',GETDATE())");

            DSODataAccess.ExecuteNonQuery(query.ToString(),connStr);
        }
        private void ActualizaNumLocalidad(string telefono,int tipoLLamada)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" UPDATE D");
            query.AppendLine(" SET Activo = 0,/*nl_borrado='S'*/");
            query.AppendLine(" dtFinVigencia = CONVERT(VARCHAR(11), GETDATE(), 121) + '00:00:00',");
            query.AppendLine(" FechaRegistro = GETDATE(),");
            query.AppendLine(" dtFecUltAct = GETDATE()");
            query.AppendLine(" FROM "+esquema+ ".[DirectorioTelefonico] AS D WITH (NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND LTRIM(RTRIM(NumeroTelefonico)) = '" + telefono.Trim() + "'");
            query.AppendLine(" AND Emple = "+emId+"");
            query.AppendLine(" AND Activo = 1 /*nl_borrado = 'N'*/");
            if(tipoLLamada == 1)/*personal*/
            {
                query.AppendLine(" AND GEtiqueta = 2");
            }
            else if(tipoLLamada == 2)/*Laboral*/
            {
                query.AppendLine(" AND GEtiqueta = 1");
            }

            DSODataAccess.ExecuteNonQuery(query.ToString(),connStr);
        }
        private void ValidaTelefonoLocalidad(string telefono, string etiqueta,int tipoLLamada)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" IF NOT EXISTS(SELECT * FROM " + esquema + ".[DirectorioTelefonico] WITH (NOLOCK) WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() ");
            query.AppendLine(" AND NumeroTelefonico = '" + telefono.Trim() + "' AND Emple = " + emId + " AND Activo = 1");
            if (tipoLLamada == 2)/*Laboral*/
            {
                query.AppendLine(" AND GEtiqueta = 2)");
            }
            else if (tipoLLamada == 1)/*personal*/
            {
                query.AppendLine(" AND GEtiqueta = 1)");
            }
            query.AppendLine(" BEGIN");
            query.AppendLine(" /*No existe, y por lo tanto se inserta en la tabla.*/");
            query.AppendLine(" INSERT INTO " + esquema + ".[DirectorioTelefonico]");
            query.AppendLine(" (");
            query.AppendLine(" NumeroTelefonico,");
            query.AppendLine(" Emple,");
            query.AppendLine(" GEtiqueta,");
            query.AppendLine(" Etiqueta,");
            query.AppendLine(" EsNumeroCorporativo,");
            query.AppendLine(" EsEditable,");
            query.AppendLine(" Activo,");
            query.AppendLine(" dtIniVigencia,");
            query.AppendLine(" dtFinVigencia,");
            query.AppendLine(" dtFecUltAct,");
            query.AppendLine(" FechaRegistro,");
            query.AppendLine(" BanderasParaDirectorioTelefonico");
            query.AppendLine(" )");
            query.AppendLine(" VALUES");
            query.AppendLine(" ( ");
            query.AppendLine(" '" + telefono.Trim() + "',");
            query.AppendLine(" " + emId + ",");
            if (tipoLLamada == 1)/*personal*/
            {
                query.AppendLine(" 1,");
            }
            else if (tipoLLamada == 2)/*Laboral*/
            {
                query.AppendLine(" 2,");
            }
            query.AppendLine(" '" + etiqueta.Trim() + "',");
            query.AppendLine(" 0,");
            query.AppendLine(" 1,");
            query.AppendLine(" 1,");
            query.AppendLine(" GETDATE(),");
            query.AppendLine(" '2079-01-01 00:00:00',");
            query.AppendLine(" GETDATE(),");
            query.AppendLine(" GETDATE(),");
            query.AppendLine(" 0");
            query.AppendLine(" ) ");
            query.AppendLine(" END");
            query.AppendLine(" ELSE");
            query.AppendLine(" BEGIN");
            query.AppendLine(" UPDATE D");
            query.AppendLine(" SET Etiqueta = '"+ etiqueta.Trim() + "',");
            query.AppendLine(" FechaRegistro = GETDATE(),");
            query.AppendLine(" dtFecUltAct = GETDATE()");
            query.AppendLine(" FROM " + esquema + ".[DirectorioTelefonico] AS D WITH (NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND NumeroTelefonico = '" + telefono.Trim() + "'");
            query.AppendLine(" AND Emple = "+emId+"");
            query.AppendLine(" AND Activo = 1 /*nl_borrado = 'N'*/");
            if (tipoLLamada == 2)/*Laboral*/
            {
                query.AppendLine(" AND GEtiqueta = 2");
            }
            else if (tipoLLamada == 1)/*personal*/
            {
                query.AppendLine(" AND GEtiqueta = 1");
            }
            query.AppendLine(" END");

            DSODataAccess.ExecuteNonQuery(query.ToString(), connStr);
        }
        private void InsertaNumerosEtiquetados(string telefono,int tipoLLamada)
        {
            StringBuilder query = new StringBuilder();
            int llam = 0;
            if (tipoLLamada == 1)/*personal*/
            {
                llam = 1;
            }
            else if (tipoLLamada == 2)/*Laboral*/
            {
                llam = 2;
            }
            query.AppendLine(" INSERT INTO "+esquema+".RegistroNumeroEtiquetado");
            query.AppendLine(" VALUES('"+ telefono.Trim() + "',GETDATE(),"+iCodUsuario+", "+emId+", "+ llam + ", GETDATE(),'2079-01-01 00:00:00.000', GETDATE())");
            DSODataAccess.ExecuteNonQuery(query.ToString(),connStr);
        }
        private void EliminaRegTelefonoLaboral(string telefono)
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(" DELETE FROM "+esquema+".TelefonoLaboral");
            query.AppendLine(" WHERE NumeroTelefonico = '"+ telefono.Trim() +"'");
            query.AppendLine(" AND Emple = "+emId+"");
            DSODataAccess.ExecuteNonQuery(query.ToString(),connStr);
        }
        private void InsertaImportes()
        {
            var fecini = Convert.ToInt32(Session["fecIniFCA"]);
            var fecFin = Convert.ToInt32(Session["fecFinFCA"]);

            decimal totalLlamPer = 0;
            decimal totalLlamNeg = 0;
            decimal total = 0;
            var totLlam =0;
            var totMin =0;

            totalLlamPer = Convert.ToDecimal(spnTotalLlam.InnerText);
            totalLlamNeg = Convert.ToDecimal(SpanTotalNeg.InnerText);
            total = Convert.ToDecimal(SpanTotal.InnerText);
            totLlam = Convert.ToInt32(spnCelLd.InnerText);
            string min = spnTiempoTot.InnerText.ToString().Replace("minutos"," ").Trim();
            totMin = Convert.ToInt32(min);
           
                StringBuilder query = new StringBuilder();
                query.AppendLine("INSERT INTO " + esquema + ".RegistroImporteEtiquetacion");
                query.AppendLine("VALUES ( CONVERT(VARCHAR(10),CONVERT(DATETIME,'" + fecini + "',112),121), CONVERT(VARCHAR(10),CONVERT(DATETIME,'" + fecFin + "',112),121)," + emId + ",GETDATE()," + totalLlamPer + "," + totalLlamNeg + "," + total + "," + totLlam + "," + totMin + "," + iCodUsuario + ",GETDATE(),'2079-01-01 00:00:00.000',GETDATE())");
                DSODataAccess.ExecuteNonQuery(query.ToString(), connStr);            
        }
        #endregion AceptarImportes

        protected void btnImprimir_Click(object sender, EventArgs e)
        {
            ExportaInfo exporta = new ExportaInfo();

            string[] datosE = new string[] { txtNomEmple.Text, txtDepartamento.Text, txtLocalidad.Text, txtCencos.Text, txtNumEmpleado.Text, txtNumDepto.Text, txtNumLocali.Text, txtNumCencos.Text };
            string[] totales = new string[] { spnCelLd.InnerText, spnTiempoTot.InnerText, SpanTotal.InnerText, spnTotalLlam.InnerText, SpanTotalNeg.InnerText };

            GridView grdLocal = grdLlamLocales;
            GridView grdTotLocal = grdTotales;
            GridView grdLlamCobrar = grvLlamadas;
            GridView gridTotCobrar = grdLlamCobrarTot;
            GridView gridMoviles = grdMovil;
            GridView gridTotMoviles = grvTotalMovil;

            string nameFile = "LlamConsumoEtiquetacion";
            string file = exporta.GeneraArchivoExcel(nameFile, datosE, Session["mesDisplay"].ToString(), grdLocal, grdTotLocal, grdLlamCobrar, gridTotCobrar, gridMoviles, gridTotMoviles, totales, 1);
            ExportFile(file);
        }

        protected void btnImprimirModal_Click(object sender, EventArgs e)
        {
            ExportaInfo export = new ExportaInfo();

            string[] datosE = new string[] { txtNomEmple.Text, txtDepartamento.Text, txtLocalidad.Text, txtCencos.Text, txtNumEmpleado.Text, txtNumDepto.Text, txtNumLocali.Text, txtNumCencos.Text };
            string[] totales = new string[] { spnCelLd.InnerText, spnTiempoTot.InnerText, SpanTotal.InnerText, spnTotalLlam.InnerText, SpanTotalNeg.InnerText };

            GridView grvLocal = null;
            GridView grvTotLocal = null;
            GridView grvLlamCobrar = grdDetalle;
            GridView grdTotCobrar = grdViewTotal;
            GridView grdMoviles = grdGlobalDetallMoviles;
            GridView grdTotMoviles = gridTotal2;
            string nameFile = "DetalleLlamConsumoEtiq";
            string file = export.GeneraArchivoExcel(nameFile, datosE, Session["mesDisplay"].ToString(), grvLocal, grvTotLocal, grvLlamCobrar, grdTotCobrar, grdMoviles, grdTotMoviles, totales,0);
            ExportFile(file);
        }
        public void ExportFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
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
                throw new KeytiaWebException("Ocurrio un error en : " + ex);

            }
        }
    }

    public class SitiosEmple
    {
        public string Cencos { get; set; }
        public int CencosId { get; set; }
        public int CobroLlamLocales { get; set; }
    }
    public class ConsumoLlamLocales
    {
        public int TablaConsulta { get; set; }
        public string NumMarcado { get; set; }
        public string Extension { get; set; }
        public string Localidad { get; set; }
        public decimal Costo { get; set; }
        public int Duracion { get; set; }
        public int Cantidad { get; set; }
        public string LocalidadKeytia { get; set; }
    }
    public class ConsumoLlamCobrar
    {
        public int TablaConsulta { get; set; }
        public int Idf { get; set; }
        public int TipoEtiqueta { get; set; }
        public string NumMarcado { get; set; }
        public string Extension { get; set; }
        public string Localidad { get; set; }
        public int TipoDestino { get; set; }
        public decimal Costo { get; set; }
        public int Duracion { get; set; }
        public int Cantidad { get; set; }
        public string LocalidadKeytia { get; set; }
    }
    public class ConsumoCel
    {
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
        public int Numero { get; set; }
        public string LocalidadKeytia { get; set; }
    }
    public class TotalConsumo
    {
        public string Llamadas { get; set; }
        public string Minutos { get; set; }
        public string CostoTotal { get; set; }
    }
    public class DetalleLlamadas
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
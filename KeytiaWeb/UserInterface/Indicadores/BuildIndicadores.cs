using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace KeytiaWeb.UserInterface.Indicadores
{
    public static class BuildIndicadores
    {
        //Nombre de los parametros a llenar de forma automatica.
        //Usuario:  @paramICodUsuario
        //Esquema:  @paramEsquema
        ////Empleado: @paramICodEmple
        //Moneda:   @paramCodMoneda
        //Perfil:   @paramICodPerfil
        //FechaIni: @paramFechaInicio
        //FechaFin: @paramFechaFin

        public static void ConstruirIndicadores(ref Panel contenedor, string pagina, string pathRequest)
        {
            try
            {
                //Get indicadores
                var dtResult = GetIndicares(pagina);
                if (dtResult.Rows.Count > 0)
                {
                    //Agregar una columna para saber guardar el color del indicador.
                    dtResult.Columns.Add("ColorRelleno");

                    //Get valores de todos los indicadores. 
                    EjecutarQueyIndicadores(ref dtResult);

                    //Get colores. Deacuerdo con el valor, evaluar los rangos para obtener el color final del indicador.
                    EvaluarRangos(ref dtResult);

                    //Generar HTML Indicadores
                    GenerarHTML(ref contenedor, ref dtResult, pathRequest);
                }
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al tratar de pintar los datos de los indicadores", ex);
            }
        }

        public static DataTable GetIndicares(string pagina)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC IndicadorObtieneConfiguraciones");
            query.AppendLine("	@esquema = '" + DSODataContext.Schema + "', ");
            query.AppendLine("	@perfil = " + HttpContext.Current.Session["iCodPerfil"].ToString() + ", ");
            query.AppendLine("	@usuario = " + HttpContext.Current.Session["iCodUsuario"].ToString() + ",");
            query.AppendLine("	@pagina = '" + pagina + "'");

            var dt = DSODataAccess.Execute(query.ToString());

            if (dt != null)
            {
                return dt;
            }
            else { return new DataTable(); }
        }

        public static void EjecutarQueyIndicadores(ref DataTable indicadores)
        {
            string query = string.Empty;
            string valor = string.Empty;

            //Agregar una columna para guardar el valor del indicador una vez ejecutado.
            indicadores.Columns.Add("Valor");

            foreach (DataRow row in indicadores.Rows)
            {
                try
                {
                    query = row["Consulta"].ToString();

                    //Se reemplazan los posibles parametros en el DataSource
                    ReplaceParametros(ref query);
                    valor = DSODataAccess.ExecuteScalar(query).ToString();

                    //Se convierte en el tipo de dato tipo numerico mas grande para poder almacenar cualquier valor.
                    row["Valor"] = Convert.ToDecimal(valor);
                }
                catch (Exception ex)
                {
                    KeytiaServiceBL.Util.LogException("Ocurrio un error al tratar de obtener los datos de un indicador", ex);
                }
            }
        }

        public static void ReplaceParametros(ref string dataSource)
        {
            if (dataSource.Contains("@paramEsquema"))
            {
                dataSource = dataSource.Replace("@paramEsquema", DSODataContext.Schema);
            }
            if (dataSource.Contains("@paramICodUsuario"))
            {
                dataSource = dataSource.Replace("@paramICodUsuario", HttpContext.Current.Session["iCodUsuario"].ToString());
            }
            if (dataSource.Contains("@paramICodPerfil"))
            {
                dataSource = dataSource.Replace("@paramICodPerfil", HttpContext.Current.Session["iCodPerfil"].ToString());
            }
            if (dataSource.Contains("@paramCodMoneda"))
            {
                dataSource = dataSource.Replace("@paramCodMoneda", HttpContext.Current.Session["Currency"].ToString());
            }
            if (dataSource.Contains("@paramFechaInicio"))
            {
                dataSource = dataSource.Replace("@paramFechaInicio", HttpContext.Current.Session["FechaInicio"].ToString() + " 00:00:00");
            }
            if (dataSource.Contains("@paramFechaFin"))
            {
                dataSource = dataSource.Replace("@paramFechaFin", HttpContext.Current.Session["FechaFin"].ToString() + " 23:59:59");
            }
            //if (dataSource.Contains("@paramICodEmple"))
            //{
            //    ///Ir por el empleado relacionado al usuario.
            //}

        }

        public static void EvaluarRangos(ref DataTable indicadores)
        {
            try
            {
                string ids = string.Empty;
                for (int i = 0; i < indicadores.Rows.Count; i++)
                {
                    ids += indicadores.Rows[i]["Indicador"].ToString() + ",";
                }
                ids += "0";

                var rangosList = GetRangos(ids);
                RangoIndicador rango = null;
                decimal valor = 0;
                foreach (DataRow row in indicadores.Rows)
                {
                    try
                    {
                        valor = Convert.ToDecimal(row["Valor"]);
                        rango = rangosList.FirstOrDefault(x => x.Indicador == Convert.ToInt32(row["Indicador"]) && valor >= x.LimiteInferior && valor <= x.LimiteSuperior);

                        if (rango != null)
                        {
                            row["ColorRelleno"] = rango.RangoColor;
                        }

                        if (string.IsNullOrEmpty(row["ColorRelleno"].ToString()) || row["ColorRelleno"] == DBNull.Value || row["ColorRelleno"] == null)
                        {
                            row["ColorRelleno"] = row["ColorDefault"];
                        }
                        if (string.IsNullOrEmpty(row["ColorRelleno"].ToString()) || row["ColorRelleno"] == DBNull.Value || row["ColorRelleno"] == null)
                        {
                            row["ColorRelleno"] = "gray";
                        }
                    }
                    catch (Exception)
                    {
                        row["Valor"] = 0;
                        row["ColorRelleno"] = "gray";
                    }                 
                }
            }
            catch (Exception)
            {

            }
        }

        public static List<RangoIndicador> GetRangos(string ids)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT");
            query.AppendLine("	Indicador		= Rango.Indicador,");
            query.AppendLine("	IndicadorCod	= Rango.IndicadorCod,");
            query.AppendLine("	RangoId			= Rango.iCodCatalogo,");
            query.AppendLine("	RangoCod		= Rango.vchCodigo,");
            query.AppendLine("	LimiteInferior	= ISNULL(Rango.LimiteInferior, 0),");
            query.AppendLine("	LimiteSuperior	= ISNULL(Rango.LimiteSuperior,0),");
            query.AppendLine("	RangoColorId	= Color.iCodCatalogo,");
            query.AppendLine("	RangoColor		= Color.Color");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('IndicadorRango','Rangos para uso en indicadores','Español')] Rango");
            query.AppendLine("	JOIN " + DSODataContext.Schema + ".[VisHistoricos('IndicadorColor','Colores para uso en indicadores','Español')] Color");
            query.AppendLine("		ON Color.iCodCatalogo = Rango.IndicadorColor");
            query.AppendLine("		AND Color.dtIniVigencia <> Color.dtFinVigencia");
            query.AppendLine("		AND Color.dtFinVigencia >= GETDATE()");
            query.AppendLine("WHERE Rango.dtIniVigencia <> Rango.dtFinVigencia");
            query.AppendLine("	AND Rango.dtFinVigencia >= GETDATE()");
            query.AppendLine("  AND Rango.Indicador IN (" + ids + ")");
            query.AppendLine("ORDER BY Rango.Indicador");

            var dt = DSODataAccess.Execute(query.ToString());

            if (dt != null)
            {
                List<RangoIndicador> listaRangos = new List<RangoIndicador>();
                foreach (DataRow row in dt.Rows)
                {
                    RangoIndicador rango = new RangoIndicador();
                    rango.Indicador = Convert.ToInt32(row["Indicador"]);
                    rango.IndicadorCod = row["IndicadorCod"].ToString();
                    rango.RangoId = Convert.ToInt32(row["RangoId"]);
                    rango.RangoCod = row["RangoCod"].ToString();
                    rango.LimiteInferior = Convert.ToDecimal(row["LimiteInferior"]);
                    rango.LimiteSuperior = Convert.ToDecimal(row["LimiteSuperior"]);
                    rango.RangoColorId = Convert.ToInt32(row["RangoColorId"]);
                    rango.RangoColor = row["RangoColor"].ToString();
                    listaRangos.Add(rango);
                }

                return listaRangos;
            }
            else { return new List<RangoIndicador>(); }
        }

        public static void GenerarHTML(ref Panel contenedor, ref DataTable config, string pathRequest)
        {
            //Ordenamos la configuracion por orden de posición
            config.DefaultView.Sort = "IndicePosicion";
            config = config.DefaultView.ToTable();
            pathRequest = pathRequest + "?";

            contenedor.Controls.Clear();
            foreach (DataRow row in config.Rows)
            {
                contenedor.Controls.Add(HTMLIndicador(row, pathRequest));
            }
        }

        public static Control HTMLIndicador(DataRow indicador, string pathRequest)
        {
            Panel contenedor = new Panel();
            contenedor.CssClass = "col-lg-2 col-md-4 col-sm-6 col-xs-12";

            HyperLink enlace = new HyperLink();
            enlace.CssClass = "dashboard-stat dashboard-stat-v2 " + indicador["ColorRelleno"].ToString();
            //enlace.NavigateUrl = indicador["LinkSiguienteNivel"].ToString() == string.Empty ? "#" : pathRequest + indicador["LinkSiguienteNivel"].ToString();

            if (!string.IsNullOrWhiteSpace(indicador["LinkSiguienteNivel"].ToString()))
            {
                if (!(indicador["LinkSiguienteNivel"].ToString().ToLower().Contains("nav=lineasinactivas") ||
                    indicador["LinkSiguienteNivel"].ToString().ToLower().Contains("nav=planbaselinea") ||
                     indicador["LinkSiguienteNivel"].ToString().ToLower().Contains("redirect=1")))
                {
                    enlace.NavigateUrl = pathRequest + indicador["LinkSiguienteNivel"].ToString();
                }
                else
                {
                    enlace.NavigateUrl = indicador["LinkSiguienteNivel"].ToString();
                }
            }
            else
            {
                enlace.NavigateUrl = "#";
            }

            Panel pnlIcono = new Panel();
            pnlIcono.CssClass = "visual";
            enlace.Controls.Add(pnlIcono);

            if (indicador["IconoNombre"].ToString().Contains("/"))
            {
                Image imagen = new Image();
                imagen.ImageUrl = VirtualPathUtility.ToAppRelative(indicador["IconoNombre"].ToString());
                pnlIcono.Controls.Add(imagen);
            }
            else
            {
                HtmlContainerControl icon = new HtmlGenericControl("I");
                icon.Attributes.Add("class", indicador["IconoNombre"].ToString());
                pnlIcono.Controls.Add(icon);
            }

            Panel pnlDetalle = new Panel();
            pnlDetalle.CssClass = "details";
            enlace.Controls.Add(pnlDetalle);

            Panel pnlNum = new Panel();
            pnlNum.CssClass = "number";
            pnlDetalle.Controls.Add(pnlNum);

            if (!string.IsNullOrEmpty(indicador["TipoDeDatoPrefijo"].ToString()))
            {
                HtmlContainerControl spanPrefix = new HtmlGenericControl("SPAN");
                spanPrefix.Attributes.Add("class", "unitStat");
                spanPrefix.InnerText = indicador["TipoDeDatoPrefijo"].ToString();
                pnlNum.Controls.Add(spanPrefix);
            }

            HtmlContainerControl spanValorIndicador = new HtmlGenericControl("SPAN");
            spanValorIndicador.Attributes.Add("data-counter", "counterup");
            spanValorIndicador.Attributes.Add("data-value", string.Format(indicador["FormatoTipoDeDato"].ToString(), indicador["Valor"].ToString()));  
            spanValorIndicador.InnerText = "0";
            pnlNum.Controls.Add(spanValorIndicador);

            if (!string.IsNullOrEmpty(indicador["TipoDeDatoSufijo"].ToString()))
            {
                HtmlContainerControl spanSufijo = new HtmlGenericControl("SPAN");
                spanSufijo.Attributes.Add("class", "unitStat");
                spanSufijo.InnerText = indicador["TipoDeDatoSufijo"].ToString();
                pnlNum.Controls.Add(spanSufijo);
            }

            HtmlContainerControl pnlDescripcion = new HtmlGenericControl("DIV");
            pnlDescripcion.Attributes.Add("class", "desc");
            pnlDescripcion.InnerText = indicador["IndicadorDescripcion"].ToString();
            pnlDetalle.Controls.Add(pnlDescripcion);

            contenedor.Controls.Add(enlace);
            return contenedor;
        }
    }
}
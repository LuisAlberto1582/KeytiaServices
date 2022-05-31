using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using KeytiaServiceBL;

//RZ.20130719
using DSOControls2008;
using System.Globalization;
using System.Web.UI.HtmlControls;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;


namespace KeytiaWeb.UserInterface.CCustodiaDTI
{
    public class NuevoEmpleadoBackend
    {

        public static bool BuscaBanderaValidaRestricciones(int iCodUsuario)
        {
            //indica si validar Restricciones o no
            bool ValidarRestricciones = false;

            StringBuilder query = new StringBuilder();


            query.AppendLine("SELECT 															");
            query.AppendLine("((isnull(BanderasUsuar,0) & 8) / 8) as AplicarRestriccionesAdmin	");
            query.AppendLine("FROM ["+DSODataContext.Schema+"].[VisHistoricos('Usuar','Usuarios','Español')]			");
            query.AppendLine("where icodcatalogo = "+iCodUsuario+"								");

            int valorBandera = 0;
            DataTable dtRes = DSODataAccess.Execute(query.ToString());
            if (dtRes.Rows.Count > 0 && dtRes.Columns.Count > 0)
            {
                int.TryParse(dtRes.Rows[0][0].ToString(), out valorBandera);
            }


            if (valorBandera == 1)
            {
                ValidarRestricciones = true;
            }

            return ValidarRestricciones;
        }

        #region  GETS
        public static int GetFolioCC()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("select ISNULL(MAX(FolioCCustodia), 0) +1 ");
            query.AppendLine("from [" + DSODataContext.Schema + "].[VisHistoricos('CCustodia','Cartas custodia','Español')]");

            int res = 0;
            DataTable dtRes = DSODataAccess.Execute(query.ToString());


            if (dtRes.Rows.Count > 0 && dtRes.Columns.Count > 0)
            {
                int.TryParse(dtRes.Rows[0][0].ToString(), out res);
            }
            return res;
            ;
        }

        public static int GetEstatusCC()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("select iCodCatalogo 															");
            query.AppendLine("from [" + DSODataContext.Schema + "].[VisHistoricos('EstCCustodia','Estatus CCustodia','Español')]		");
            query.AppendLine("where dtIniVigencia <> dtFinVigencia 											");
            query.AppendLine("and dtFinVigencia >= getdate() 												");
            query.AppendLine("and Value = 1																	");


            int res = 0;
            DataTable dtRes = DSODataAccess.Execute(query.ToString());


            if (dtRes.Rows.Count > 0 && dtRes.Columns.Count > 0)
            {
                int.TryParse(dtRes.Rows[0][0].ToString(), out res);
            }
            return res;

        }

        public static DataTable GetJefes()
        {
            StringBuilder lsbQuery = new StringBuilder();
            var datos = new DataTable();
            try
            {
                lsbQuery.Length = 0;
                lsbQuery.AppendLine("SELECT iCodCatalogo, vchDescripcion = rtrim(ltrim(NomCompleto))");
                lsbQuery.AppendLine("FROM [VisHistoricos('Emple','Empleados','Español')] ");
                lsbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia ");
                lsbQuery.AppendLine("   AND dtFinVigencia >= GETDATE() ");
                lsbQuery.AppendLine("   AND NomCompleto <> ''");
                lsbQuery.AppendLine("And NominaA <> '999999999'");
                lsbQuery.AppendLine("And  (TipoEmCod = 'x' or TipoEmCod = 'e')");
                lsbQuery.AppendLine("ORDER BY NomCompleto");

                datos = DSODataAccess.Execute(lsbQuery.ToString());

            }
            catch
            {

            }

            return datos;

        }
        public static DataTable GetJefesFCA()
        {
            StringBuilder lsbQuery = new StringBuilder();
            var datos = new DataTable();
            try
            {
                lsbQuery.Length = 0;
                lsbQuery.AppendLine(" SELECT");
                lsbQuery.AppendLine(" iCodCatalogo,");
                lsbQuery.AppendLine(" CASE WHEN");
                lsbQuery.AppendLine(" ISNULL(RTRIM(LTRIM(Paterno)) + ' ' + RTRIM(LTRIM(Materno)) + ' ' + RTRIM(LTRIM(Nombre)), '') = '' THEN RTRIM(LTRIM(NomCompleto))");
                lsbQuery.AppendLine(" ELSE RTRIM(LTRIM(Paterno)) +' ' + RTRIM(LTRIM(Materno)) + ' ' + RTRIM(LTRIM(Nombre))");
                lsbQuery.AppendLine(" END AS vchDescripcion");
                lsbQuery.AppendLine(" FROM [VisHistoricos('Emple','Empleados','Español')] WITH(NOLOCK)");
                lsbQuery.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia");
                lsbQuery.AppendLine(" AND dtFinVigencia >= GETDATE()");
                lsbQuery.AppendLine(" AND NomCompleto <> ''");
                lsbQuery.AppendLine(" AND NominaA <> '999999999'");
                lsbQuery.AppendLine(" AND(TipoEmCod = 'X' or TipoEmCod = 'E')");
                lsbQuery.AppendLine(" ORDER BY NomCompleto");

                datos = DSODataAccess.Execute(lsbQuery.ToString());

            }
            catch
            {

            }

            return datos;
        }
        public static DataTable GetPlantasFCA()
        {
            try
            {
                DataTable dtRes = new DataTable();

                StringBuilder query = new StringBuilder();

                query.AppendLine("select");
                query.AppendLine("    iCodCatalogo,");
                query.AppendLine("    vchDescripcion,");
                query.AppendLine("    Descripcion,");
                query.AppendLine("    dtIniVigencia,");
                query.AppendLine("    dtFinVigencia");
                query.AppendLine("From [" + DSODataContext.Schema + "].[VisHistoricos('PlantaFCA','Plantas FCA','Español')]");
                query.AppendLine("where dtIniVigencia<> dtFinVigencia");
                query.AppendLine("And dtFinVigencia >= GETDATE()");

                dtRes = DSODataAccess.Execute(query.ToString());

                return dtRes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetCenCos(bool BuscaDeptos = false)
        {
            try
            {
                string condicionAdicional = "";

                if (DSODataContext.Schema.ToLower() == "fca")
                {
                    if (BuscaDeptos)
                    {
                        condicionAdicional = "And (isnull(BanderasCenCos,0) & 2) / 2 = 1";
                    }
                    else
                    {
                        condicionAdicional = "And (isnull(BanderasCenCos,0) & 2) / 2 = 0";
                    }
                }


                StringBuilder query = new StringBuilder();

                query.Length = 0;
                query.AppendLine("SELECT iCodCatalogo,vchDescripcion =  ltrim(rtrim(vchDescripcion))");
                query.AppendLine("FROM [VisHistoricos('Cos','Cos','Español')] WITH(NOLOCK)");
                query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
                query.AppendLine("   AND dtFinVigencia >= GETDATE()");
                if (condicionAdicional.Length > 0)
                {
                    query.AppendLine(condicionAdicional);
                }
                query.AppendLine("ORDER BY vchDescripcion");

                return DSODataAccess.Execute(query.ToString());


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetCenCosFCA(bool BuscaDeptos = false)
        {
            try
            {
                string condicionAdicional = "";

                if (DSODataContext.Schema.ToLower() == "fca")
                {
                    if (BuscaDeptos)
                    {
                        condicionAdicional = "And (isnull(BanderasCenCos,0) & 2) / 2 = 1";
                    }
                    else
                    {
                        condicionAdicional = "And (isnull(BanderasCenCos,0) & 2) / 2 = 0";
                    }
                }


                StringBuilder query = new StringBuilder
                {
                    Length = 0
                };
                query.AppendLine("SELECT iCodCatalogo,LTRIM(RTRIM(vchCodigo))+ ' - ' + LTRIM(RTRIM(Descripcion)) AS vchDescripcion");
                query.AppendLine("FROM [VisHistoricos('CenCos','Centro de Costos','Español')] WITH(NOLOCK)");
                query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
                query.AppendLine("   AND dtFinVigencia >= GETDATE()");
                if (condicionAdicional.Length > 0)
                {
                    query.AppendLine(condicionAdicional);
                }
                query.AppendLine("ORDER BY vchDescripcion");

                return DSODataAccess.Execute(query.ToString());


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DataTable GetSitios(int iCodCatUsuario)
        {
            try
            {

                StringBuilder query = new StringBuilder();

                query.AppendLine("declare @schema  varchar(40) = '" + DSODataContext.Schema + "'														");
                query.AppendLine("declare @vchCodUsuar varchar(40) = '" + HttpContext.Current.Session["vchCodUsuario"] + "'	");
                query.AppendLine("declare @iCodCatUsuario  int =" + iCodCatUsuario + "								");
                query.AppendLine("declare @query nvarchar(max) = ''															");
                query.AppendLine("																							");
                query.AppendLine("Set @query = 																				");
                query.AppendLine("'																							");
                query.AppendLine("	Select @iCodCatUsuario = iCodCatalogo													");
                query.AppendLine("	From ['+@Schema+'].vUsuario usuar																		");
                query.AppendLine("	Where usuar.dtIniVigencia <> usuar.dtFinVigencia										");
                query.AppendLine("	And usuar.dtFinVigencia >= GETDATE()													");
                query.AppendLine("	And usuar.vchCodigo = '''+@vchCodUsuar+'''												");
                query.AppendLine("'																							");
                query.AppendLine("																							");
                query.AppendLine("EXEC sp_executesql @query, N'@iCodCatUsuario  INT OUTPUT ', @iCodCatUsuario  OUTPUT		");
                query.AppendLine("																							");
                query.AppendLine("--------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                query.AppendLine("/* OBTENER EL VALOR DE LAS BANDERAS DE OMITIR RESTRICCIONES DE EMPLE, CENCOS Y SITIO */");
                query.AppendLine("DECLARE @nQueryOmitirRestricciones NVARCHAR(MAX) = '';								");
                query.AppendLine("DECLARE @omitirRestSitio bit = 0														");
                query.AppendLine("DECLARE @omitirRestCenCos bit = 0														");
                query.AppendLine("DECLARE @omitirRestEmple bit = 0														");
                query.AppendLine("																						");
                query.AppendLine("SET @nQueryOmitirRestricciones = '													");
                query.AppendLine("		SELECT @omitirRestSitio = (isnull(banderasusuar,0) & 1)/1,						");
                query.AppendLine("				@omitirRestCenCos = (isnull(banderasusuar,0) & 2)/2,					");
                query.AppendLine("				@omitirRestEmple = (isnull(banderasusuar,0) & 4)/4						");
                query.AppendLine("		FROM ['+@Schema+'].vTIUsuarios  										");
                query.AppendLine("		WHERE dtFinVigencia >= GETDATE()												");
                query.AppendLine("		AND iCodCatalogo = ' + Convert(VARCHAR, ISNULL(@iCodCatUsuario,0))				");
                query.AppendLine("																						");
                query.AppendLine("EXEC sp_executesql @nQueryOmitirRestricciones, N'@omitirRestSitio INT OUTPUT, @omitirRestCenCos INT OUTPUT, @omitirRestEmple INT OUTPUT ', @omitirRestSitio OUTPUT, @omitirRestCenCos OUTPUT, @omitirRestEmple OUTPUT");
                query.AppendLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
                query.AppendLine("																							");
                query.AppendLine("																							");
                query.AppendLine("Set @query =																				");
                query.AppendLine("'																							");
                query.AppendLine("	select * ");
                query.AppendLine("	from ['+@Schema+'].HistSitio sitioComun      								");
                query.AppendLine("																							");
                query.AppendLine("'																							");
                query.AppendLine("																							");

                if (BuscaBanderaValidaRestricciones(iCodCatUsuario))
                {


                    query.AppendLine("		Set @Query = @Query + '																");
                    query.AppendLine("			INNER JOIN ['+@Schema+'].RestSitio RestSitio									");
                    query.AppendLine("				ON sitioComun.iCodCatalogo = RestSitio.Sitio								");
                    query.AppendLine("				And sitioComun.dtIniVigencia <= RestSitio.FechaInicio						");
                    query.AppendLine("				And sitioComun.dtFinVigencia >= RestSitio.Fechafin							");
                    query.AppendLine("				And RestSitio.Usuar = ' + convert(varchar, @iCodCatUsuario) + ''			");
                }
                query.AppendLine("																							");
                query.AppendLine("Set @query = @query+ 																		");
                query.AppendLine("'																							");
                query.AppendLine("	Where SitioComun.dtIniVigencia<> SitioComun.dtFinVigencia                    			");
                query.AppendLine("	and SitioComun.dtFinVigencia >= GETDATE()                         						");
                query.AppendLine("	and SitioComun.vchCodigo<> ''99999999''													");

                if (DSODataContext.Schema.ToUpper() == "FCA")
                {
                    query.AppendLine("	and SitioComun.vchCodigo<> ''Telcel''													");
                }

                query.AppendLine("   order by  sitioComun.vchDescripcion");
                query.AppendLine("");
                query.AppendLine("'																							");
                query.AppendLine("																							");
                query.AppendLine("																							");
                query.AppendLine("																							");
                query.AppendLine("exec(@query)																				");


                return DSODataAccess.Execute(query.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetTiposEmple()
        {
            try
            {
                StringBuilder query = new StringBuilder();
                query.Append("SELECT iCodCatalogo, vchDescripcion \r");
                query.Append("FROM [VisHistoricos('TipoEm','Tipo Empleado','Español')] \r");
                query.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
                query.Append("and dtFinVigencia >= GETDATE() \r");

                //Si el cliente no es FCA no se le mostrara el tipo de empleado de Sindicato
                if (DSODataContext.Schema.ToUpper() != "FCA")
                {
                    query.AppendLine("And vchCodigo <> 'CC.OO.'");
                }
                else
                {
                    query.AppendLine("And vchCodigo <> 'S'");
                }

                query.Append("order by vchDescripcion");


                return DSODataAccess.Execute(query.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetLocalidades()
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendLine("SELECT iCodCatalogo, vchDescripcion \r");
                query.AppendLine("FROM [VisHistoricos('Estados','Estados','Español')] \r");
                query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia \r");
                query.AppendLine("and dtFinVigencia >= GETDATE() \r");
                query.AppendLine("and Paises = 714 --Mexico \r");
                query.AppendLine("order by vchDescripcion");


                return DSODataAccess.Execute(query.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static DataTable GetProveedor()
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendLine("SELECT iCodCatalogo, vchDescripcion \r");
                query.AppendLine("FROM [VisHistoricos('Proveedor','Proveedor','Español')] \r");
                query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia \r");
                query.AppendLine("and dtFinVigencia >= GETDATE() \r");
                query.AppendLine("order by vchDescripcion");

                return DSODataAccess.Execute(query.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetCos()
        {
            try
            {
                StringBuilder query = new StringBuilder();


                query.AppendLine("SELECT iCodCatalogo, vchDescripcion = vchDescripcion + ' (' + ISNULL(vchCodigo,'') + ')'");
                query.AppendLine("FROM [VisHistoricos('Cos','Cos','Español')]");
                query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
                query.AppendLine("   AND dtFinVigencia >= GETDATE()");
                query.AppendLine("ORDER BY vchDescripcion");

                return DSODataAccess.Execute(query.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetEmpre()
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendLine("SELECT iCodCatalogo, vchDescripcion");
                query.AppendLine("FROM [VisHistoricos('Empre','Empresas','Español')]");
                query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
                query.AppendLine("   AND dtFinVigencia >= GETDATE()");
                query.AppendLine("   AND vchCodigo <> 'KeytiaE'");
                query.AppendLine("ORDER BY vchDescripcion");


                return DSODataAccess.Execute(query.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetCarrier()
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendLine("SELECT iCodCatalogo, vchDescripcion");
                query.AppendLine("FROM [VisHistoricos('Carrier','Carriers','Español')]");
                query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
                query.AppendLine("   AND dtFinVigencia >= GETDATE()");
                if (DSODataContext.Schema.ToLower() == "fca")
                {
                    query.AppendLine(" And vchCodigo like 'Telcel'");
                }
                query.AppendLine("ORDER BY vchDescripcion");

                return DSODataAccess.Execute(query.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetCtaMaestraCarrier(int Carrier)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendLine("SELECT iCodCatalogo, vchDescripcion");
                query.AppendLine("FROM [VisHistoricos('CtaMaestra','Cuenta Maestra Carrier','Español')] WITH(NOLOCK)");
                query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
                query.AppendLine("   AND dtFinVigencia >= GETDATE()");
                query.AppendLine("   AND Carrier = " + Carrier);  //Filtrar las cuentas del carrier del que sea la linea.
                query.AppendLine("ORDER BY vchDescripcion");


                return DSODataAccess.Execute(query.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DataTable GetPuestos()
        {
            StringBuilder query = new StringBuilder();
            StringBuilder condicionAdicional = new StringBuilder();


            if (DSODataContext.Schema.ToLower() == "fca")
            {

                condicionAdicional.AppendLine("And vchCodigo = 'SI' ");
                condicionAdicional.AppendLine("And vchDescripcion = 'Sin Identificar'");
            }

            query.AppendLine("SELECT MIN(iCodCatalogo) AS iCodCatalogo, UPPER(vchDescripcion) AS vchDescripcion");
            query.AppendLine("FROM [VisHistoricos('Puesto','Puestos Empleado','Español')]");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            if (DSODataContext.Schema.ToLower() == "fca")
            {
                query.AppendLine(condicionAdicional.ToString());
            }
            query.AppendLine("AND dtFinVigencia >= GETDATE()");

            query.AppendLine("GROUP BY vchDescripcion ORDER BY vchDescripcion");

            return DSODataAccess.Execute(query.ToString());

        }

        public static int ValidaExisteCod(string claveFac,int sitio)
        {
            int existe = 0;
            StringBuilder query = new StringBuilder();

            query.AppendLine(" IF EXISTS(");
            query.AppendLine(" SELECT vchCodigo FROM [vishistoricos('CodAuto','Codigo Autorizacion','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND vchCodigo = '"+ claveFac + "' AND sitio = "+ sitio + ")");
            query.AppendLine(" BEGIN");
            query.AppendLine(" SELECT 1 AS Existe");
            query.AppendLine(" END");
            query.AppendLine(" ELSE");
            query.AppendLine(" BEGIN");
            query.AppendLine(" SELECT 0 AS Existe");
            query.AppendLine(" END");
            DataTable dt = DSODataAccess.Execute(query.ToString());

            if(dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                existe = Convert.ToInt32(dr["Existe"]);
            }

            return existe;
        }

        #endregion

        #region Valida
        //AM 20130717 . Agrego metodo para Validar que el formato de fecha sea  DD/MM/AAAA
        /// <summary>
        /// Valida si el formato de fecha es dd/MM/YYYY
        /// </summary>
        /// <param name="Fecha">Fecha a validar.</param>
        /// <returns>Si se cumple el formato regresa true.</returns>
        public static bool validaFormatoFecha(string Fecha)
        {
            //bool fechaValida = false;
            DateTime dateTime;

            //RZ.20131128 Se cambia expresion regular ya que el formato de la fecha se encontraba fijo
            //Con este metodo hara la conversion a un datetime usando el formato de la cultura actual en la aplicacion
            return DateTime.TryParseExact(Fecha, CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
        }
        #endregion  




        public static string GenerarNominaAutomatica()
        {
            try
            {
                string nominaRes = "";

                StringBuilder query = new StringBuilder();

                query.AppendLine("if(Object_id('tempdb..#NominaTemp') is Not Null)													");
                query.AppendLine("Begin																								");
                query.AppendLine("	Drop table #NominaTemp																			");
                query.AppendLine("End 																								");
                query.AppendLine("																									");
                query.AppendLine("																									");
                query.AppendLine("																									");
                query.AppendLine("Select 	Nomina = Convert(int,NominaA)   														");
                query.AppendLine("--Nomina = Convert(int,Replace(Replace(Replace(NominaA,'MA',''),'Temp',''),'EX','')	) 			");
                query.AppendLine("into #NominaTemp																					");
                query.AppendLine("From [" + DSODataContext.Schema + "].[VisHistoricos('Emple','Empleados','Español')]				");
                query.AppendLine("Where dtIniVigencia <> dtFinVigencia																");
                query.AppendLine("And dtFinVigencia >= GETDATE()																	");
                query.AppendLine("And isNumeric(NominaA)  =1 																		");
                query.AppendLine("--And isNumeric(Replace(Replace(Replace(NominaA,'MA',''),'Temp',''),'EX','')	) = 1				");
                query.AppendLine("--And Len(Replace(Replace(Replace(NominaA,'MA',''),'Temp',''),'EX','')) <=9						");
                query.AppendLine("And NominaA <> '999999999'																		");
                query.AppendLine("And Len(NominaA) <9			                                                                    ");
                query.AppendLine("																									");
                query.AppendLine("Select max(Nomina) + 1																			");
                query.AppendLine("From #NominaTemp																					");

                DataTable dtRes = DSODataAccess.Execute(query.ToString());


                nominaRes = dtRes.Rows[0][0].ToString();

                return nominaRes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string GeneraNomAutomatica()
        {
            string nominaRes = "";
            try
            {
                StringBuilder query = new StringBuilder();
                query.AppendLine(" SELECT TOP 1 CONVERT(INT, REPLACE(NominaA,'EX','')) + 1 FROM " + DSODataContext.Schema + ".[vishistoricos('Emple','Empleados','Español')]");
                query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia ");
                query.AppendLine(" AND TipoEmCod = 'X'");
                query.AppendLine(" AND UPPER(NominaA) LIKE 'EX%'");
                query.AppendLine(" AND NominaA <> 'EX99999999'");
                query.AppendLine(" ORDER BY NominaA DESC");

                DataTable dtRes = DSODataAccess.Execute(query.ToString());


                nominaRes = dtRes.Rows[0][0].ToString();

                return nominaRes;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public static void getValBandera(string lsCodBandera, ref int liValBanderas)
        {
            string lsValorBandera = String.Empty;
            StringBuilder lsbConsulta = new StringBuilder();

            lsbConsulta.Append("SELECT Value \r");
            lsbConsulta.Append("FROM [VisHistoricos('Valores','Valores','Español')] \r");
            lsbConsulta.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
            lsbConsulta.Append("and dtFinVigencia >= GETDATE() \r");
            lsbConsulta.Append("and dtFinVigencia >= GETDATE() \r");
            lsbConsulta.Append("and vchCodigo = '" + lsCodBandera + "' \r");

            lsValorBandera = DSODataAccess.ExecuteScalar(lsbConsulta.ToString()).ToString();

            if (!String.IsNullOrEmpty(lsValorBandera))
            {
                liValBanderas += int.Parse(lsValorBandera);
            }
        }

        public static int obtenCatalogoUsuarioAsignado(string vchCodigoUsuar, string iCodUsuarioDB)
        {
            StringBuilder lsbQueryUsuar = new StringBuilder();

            lsbQueryUsuar.Append("SELECT icodcatalogo \r");
            lsbQueryUsuar.Append("FROM [VisHistoricos('Usuar','Usuarios','Español')] \r");
            lsbQueryUsuar.Append("WHERE dtInivigencia <> dtFinVigencia \r");
            lsbQueryUsuar.Append("and dtFinvigencia >= GETDATE() \r");
            lsbQueryUsuar.Append("and usuardb = " + iCodUsuarioDB + " \r");
            lsbQueryUsuar.Append("and vchcodigo = '" + vchCodigoUsuar + "' \r");

            DataRow ldr = DSODataAccess.ExecuteDataRow(lsbQueryUsuar.ToString());

            if (ldr != null)
            {
                return (int)ldr["iCodCatalogo"];
            }

            return -1;
        }

        /*RZ.20130722*/
        public static string GrabarEmpleado(Hashtable phtValuesEmple, DatosEmple objDatosEmple, string iCodUsuarioDB)
        {
            DALCCustodia dalCC = new DALCCustodia();

            Hashtable lhtEmpleB = new Hashtable();

            //Elementos para insert en CCustodia
            Hashtable lhtCCust = new Hashtable();

            string liFolioCCust = NuevoEmpleadoBackend.GetFolioCC().ToString();

            string iCodEstatusCCust = NuevoEmpleadoBackend.GetEstatusCC().ToString();

            string lsNomCompleto = phtValuesEmple["{NomCompleto}"].ToString();


            if (DSODataContext.Schema.ToUpper() == "FCA")
            {
                if (phtValuesEmple.ContainsKey("{CenCos}") && objDatosEmple.DepartamentoDesc.Length > 0)
                {
                    phtValuesEmple["{CenCos}"] = objDatosEmple.ICodCatDepartamento;
                }
            }

            int iCodRegEmple = dalCC.AltaEmple(phtValuesEmple);


            string iCodEmple = String.Empty;

            if (iCodRegEmple != -1)
            {
                objDatosEmple.iCodCatemple = iCodRegEmple;

                //El metodo regresa el icodcatalogo del empleado cuando es mayor a 0, que quiere decir que si se logro la alta.
                iCodEmple = iCodRegEmple.ToString();

                lhtEmpleB.Clear();
                lhtEmpleB.Add("vchCodigo", phtValuesEmple["vchCodigo"].ToString() + " (B)");
                lhtEmpleB.Add("vchDescripcion", lsNomCompleto + " (B)");
                lhtEmpleB.Add("{Emple}", iCodEmple);
                lhtEmpleB.Add("dtIniVigencia", Convert.ToDateTime(phtValuesEmple["dtIniVigencia"]));
                lhtEmpleB.Add("iCodUsuario", HttpContext.Current.Session["iCodUsuario"]);
                lhtEmpleB.Add("dtFecUltAct", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                dalCC.AltaEmpleB(lhtEmpleB);

                lhtCCust.Clear();
                lhtCCust.Add("iCodMaestro", DALCCustodia.getiCodMaestro("Cartas custodia", "CCustodia"));
                lhtCCust.Add("vchCodigo", "CCustodia " + liFolioCCust);
                lhtCCust.Add("vchDescripcion", phtValuesEmple["vchCodigo"].ToString() + " (Folio:" + liFolioCCust + ")");
                lhtCCust.Add("{Emple}", iCodEmple);
                lhtCCust.Add("{EstCCustodia}", iCodEstatusCCust);
                lhtCCust.Add("{FolioCCustodia}", liFolioCCust);
                lhtCCust.Add("{FechaCreacion}", Convert.ToDateTime(objDatosEmple.FechaInicioVigencia).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                lhtCCust.Add("{FechaResp}", null);
                lhtCCust.Add("{FechaCancelacion}", null);
                lhtCCust.Add("{ComentariosEmple}", null);
                lhtCCust.Add("{ComentariosAdmin}", null);
                lhtCCust.Add("dtIniVigencia", Convert.ToDateTime(objDatosEmple.FechaInicioVigencia));
                lhtCCust.Add("dtFinVigencia", new DateTime(2079, 1, 1));
                lhtCCust.Add("iCodUsuario", HttpContext.Current.Session["iCodUsuario"]);
                lhtCCust.Add("dtFecUltAct", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                dalCC.AltaCCustodia(lhtCCust);


                //Insert de la relacion del empleado con el CC seleccionado
                dalCC.AltaRelEmpleCenCos(iCodEmple, phtValuesEmple["{CenCos}"].ToString(), Convert.ToDateTime(phtValuesEmple["dtIniVigencia"]), Convert.ToDateTime(phtValuesEmple["dtFinVigencia"]));

                //Alta Empleado FCA
                if (DSODataContext.Schema.ToString().ToUpper() == "FCA")
                {
                    Dictionary<string, string> dctEmpleFCA = new Dictionary<string, string>();
                    dctEmpleFCA.Add("planta", objDatosEmple.ICodCatPlanta.ToString());
                    dctEmpleFCA.Add("iCodCatEmple", iCodRegEmple.ToString());
                    dctEmpleFCA.Add("Dc_id", objDatosEmple.DC_ID.ToString());
                    dctEmpleFCA.Add("T_id", objDatosEmple.T_ID.ToString());
                    dctEmpleFCA.Add("Estacion", objDatosEmple.Estatcion.ToString());
                    dctEmpleFCA.Add("iCodCatDiretor", objDatosEmple.ICodCatDirector.ToString());

                    AltaEmpleFCA(dctEmpleFCA);


                    if (objDatosEmple.ICodCatTipoEmple.ToString() == "446")
                    {
                        //Inserta relacion externo Cencos
                        dalCC.AltaRelExternoCenCos(iCodEmple, objDatosEmple.ICodCatCenCos.ToString(), Convert.ToDateTime(phtValuesEmple["dtIniVigencia"]), Convert.ToDateTime(phtValuesEmple["dtFinVigencia"]));
                    }
                }


                if(objDatosEmple.ClaveFAC.Length > 0 )
                {
                    NuevoEmpleadoBackend.ProcesoAltaDeCodAuto(objDatosEmple);
                }
                

                if (!objDatosEmple.SinExt && objDatosEmple.Extension.Length > 0)
                {
                    NuevoEmpleadoBackend.ProcesoDeAltaExten(objDatosEmple);
                }



                if (objDatosEmple.NumLineaDirecta.Length > 0)
                {

                    ProcesoAltaDeLinea(objDatosEmple);


                    if (DSODataContext.Schema.ToLower() == "fca")
                    {
                        //RM20190401 Alta Campos Linea FCA 
                        AltaLineaCamposFCA(objDatosEmple);
                    }
                }


                //Mandar a actualizar la jerarquia y restricciones del empleado creado
                ActualizaJerarquiaRest(iCodEmple, objDatosEmple, phtValuesEmple, iCodUsuarioDB);

            }

            Util.LogMessage("RZ. El empleado se ha dado de alta con el siguiente catalogo: " + iCodEmple);

            return iCodEmple;
        }

        public static bool AltaLineaCamposFCA(DatosEmple objDatosEmple)
        {
            try
            {
                bool altaExitosa = false;

                string lineaAlta = objDatosEmple.NumLineaDirecta;


                string queryAltaLineaFCA = ConsultaAltaLineaFCA(
                                                                    objDatosEmple.NumLineaDirecta,
                                                                    objDatosEmple.PentaSAPAccount,
                                                                    objDatosEmple.PentaSAPProfitCenter,
                                                                    objDatosEmple.PentaSAPCostCenter,
                                                                    objDatosEmple.PentaSAPFA,
                                                                    objDatosEmple.PentaSAPCCDescription
                                                                );


                DataTable dt = DSODataAccess.Execute(queryAltaLineaFCA);


                return altaExitosa;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static string ConsultaAltaLineaFCA
        (
            string LineaAlta,
            string PentaSAPAccount,
            string PentaSAPProfitCenter,
            string PentaSAPCostCenter,
            string PentaSAPFA,
            string PentaSAPCCDescription
        )
        {
            try
            {
                StringBuilder query = new StringBuilder();


                query.AppendLine("Exec  AltaLineaFCA					                ");
                query.AppendLine("@schema	= '" + DSODataContext.Schema + "',              ");
                query.AppendLine("@lineaCod = '" + LineaAlta + "',				            ");
                query.AppendLine("@PentaSAPAccount	 = " + PentaSAPAccount + ",			    ");
                query.AppendLine("@PentaSAPProfitCenter	 = " + PentaSAPProfitCenter + ",	");
                query.AppendLine("@PentaSAPCostCenter	  = " + PentaSAPCostCenter + ",		");
                query.AppendLine("@PentaSAPFA			= " + PentaSAPFA + ",		        ");
                query.AppendLine("@PentaSAPCCDescription = " + PentaSAPCCDescription + "	");

                return query.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ProcesoAltaDeLinea(DatosEmple objDatosEmple)
        {
            //Mapeo campos
            string codLinea = NoSQLCode(objDatosEmple.NumLineaDirecta);
            DateTime dtFechaInicio = Convert.ToDateTime(objDatosEmple.FechaInicioVigencia);
            string iCodCatCarrier = NoSQLCode(objDatosEmple.iCodcatCarrier.ToString());
            string iCodCatSitio = NoSQLCode(objDatosEmple.ICodCatSitio.ToString());
            string vchCodigoEmple = NoSQLCode(objDatosEmple.Nomina);

            DALCCustodia lineaCC = new DALCCustodia();
            StringBuilder lsbQuery = new StringBuilder();

            //Query para ver si la línea existe
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("SELECT iCodCatalogo, vchCodigo FROM [VisHistoricos('Linea','Lineas','Español')]");
            lsbQuery.AppendLine("WHERE dtInivigencia <> dtFinvigencia");
            lsbQuery.AppendLine("  AND dtFinvigencia >= GETDATE()");
            lsbQuery.AppendLine("  AND vchCodigo = '" + codLinea + "'");
            lsbQuery.AppendLine("  AND Sitio = " + iCodCatSitio);
            lsbQuery.AppendLine("  AND Carrier = " + iCodCatCarrier);
            DataRow drExisteLinea = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

            //La línea ya existe ?
            if (drExisteLinea != null)
            {
                #region La linea ya Existe

                string lsiCodCatalogoLinea = drExisteLinea["iCodCatalogo"].ToString();

                //Query para ver si la línea ya tiene una relación con otro empleado.
                lsbQuery.Length = 0;
                lsbQuery.AppendLine("SELECT Emple, EmpleDesc FROM [VisRelaciones('Empleado - Linea','Español')]");
                lsbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
                lsbQuery.AppendLine("  AND dtFinvigencia >= GETDATE()");
                lsbQuery.AppendLine("  AND Linea = " + lsiCodCatalogoLinea);
                DataRow drRelEmpLinea = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

                //La linea esta asignada a otro empleado ?
                if (drRelEmpLinea != null)
                {
                    #region La línea le pertenece a otro Empleado

                    string nombreEmpleRel = drRelEmpLinea["EmpleDesc"].ToString();

                    TextInfo miInfo = CultureInfo.CurrentCulture.TextInfo;
                    int inicioDeParentesis = nombreEmpleRel.IndexOf("(") + 1;
                    char[] parentesis = { ')', '(' };
                    string nombreEmpleado = nombreEmpleRel.Substring(0, inicioDeParentesis).TrimEnd(parentesis).Trim();

                    throw new ArgumentException("La Línea que selecciono ya esta asignada al empleado : " + miInfo.ToTitleCase(nombreEmpleado.ToLower()));

                    #endregion
                }
                string fechaInicioValida = ValidarTraslapeFechas(dtFechaInicio, lsiCodCatalogoLinea, "Linea", "Empleado - Linea", "null");

                if (fechaInicioValida == "1")
                {
                    string lsiCodCatalgoLinea = drExisteLinea["iCodCatalogo"].ToString();
                    string lsVchCodLinea = drExisteLinea["vchCodigo"].ToString();

                    //Validación para que la fecha inicio de la línea no pueda ser menor a la fecha inicio del empleado
                    string lsdtFechaInicioEmple = DSODataAccess.ExecuteScalar("SELECT dtIniVigencia FROM [VisHistoricos('Emple','Empleados','Español')] " +
                                            "WHERE iCodCatalogo = " + objDatosEmple.iCodCatemple.ToString() +
                                            "AND dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()").ToString();

                    DateTime dtFechaInicioEmple = Convert.ToDateTime(lsdtFechaInicioEmple);

                    //Si la fecha de inicio es mayor o igual a la fecha de alta del empleado entra a este bloque
                    if (dtFechaInicio >= dtFechaInicioEmple)
                    {
                        // Se hace un insert en [VisRelaciones('Empleado - Linea','Español')]  // Se hace un update a la vista [VisHistoricos('Linea','Lineas','Español')] en el campo Emple 
                        lineaCC.AltaRelacionEmpLinea(vchCodigoEmple, lsVchCodLinea, objDatosEmple.iCodCatemple.ToString(), lsiCodCatalgoLinea, dtFechaInicio);
                    }
                    else  //Si la fecha de inicio es menor a la fecha de alta del empleado manda un mensaje de notificación
                    {
                        throw new ArgumentException("La fecha de inicio debe ser mayor o igual a la fecha de alta del empleado.");
                    }
                }
                else //La fecha de inicio no es valida
                {
                    throw new ArgumentException("La fecha que seleccionó entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                }
                #endregion
            }
            else //La Línea no existe, entonces entro a este bloque
            {
                #region La Línea no Existe Activa

                //Validar si existio en el pasado y que ya no esta activa, para revisar traslape de relaciones.
                lsbQuery.Length = 0;
                lsbQuery.AppendLine("SELECT iCodCatalogo, vchCodigo FROM [VisHistoricos('Linea','Lineas','Español')]");
                lsbQuery.AppendLine("WHERE dtInivigencia <> dtFinvigencia");
                lsbQuery.AppendLine("  AND vchCodigo = '" + codLinea + "'");
                lsbQuery.AppendLine("  AND Sitio = " + iCodCatSitio);
                lsbQuery.AppendLine("  AND Carrier = " + iCodCatCarrier);
                DataRow drLineaPasado = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

                if (drLineaPasado != null)
                {
                    //Se valida traslape por si en el pasado ya habia existido.
                    string fechaInicioValida = ValidarTraslapeFechas(dtFechaInicio, drLineaPasado["iCodCatalogo"].ToString(), "Linea", "Empleado - Linea", "null");
                    if (fechaInicioValida != "1")
                    {
                        throw new ArgumentException("La fecha que selecciono entra dentro de los rangos de relación de otro empleado, favor de seleccionar una fecha validá.");
                    }
                }

                //Validación para que la fecha inicio de la linea no pueda ser menor a la fecha inicio del empleado
                string lsdtFechaInicioEmple = DSODataAccess.ExecuteScalar("SELECT dtIniVigencia FROM [VisHistoricos('Emple','Empleados','Español')] " +
                                        "WHERE iCodCatalogo = " + objDatosEmple.iCodCatemple.ToString() +
                                        "AND dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()").ToString();

                DateTime dtFechaInicioEmple = Convert.ToDateTime(lsdtFechaInicioEmple);

                //Si la fecha de inicio es mayor o igual a la fecha de alta del empleado entra a este bloque
                if (dtFechaInicio >= dtFechaInicioEmple)
                {
                    //Valida que el tipo de recurso exista
                    lsbQuery.Length = 0;
                    lsbQuery.AppendLine("SELECT iCodCatalogo");
                    lsbQuery.AppendLine("FROM [VisHistoricos('Recurs','Recursos','Español')]");
                    lsbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
                    lsbQuery.AppendLine("  AND dtFinVigencia >= GETDATE()");
                    lsbQuery.AppendLine("  AND Carrier = " + iCodCatCarrier);
                    lsbQuery.AppendLine("  AND EntidadCod = 'Linea'");
                    DataRow drRecursoLineaCarrier = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

                    if (drRecursoLineaCarrier == null)
                    {
                        throw new ArgumentException("No existe el Recurso de Linea para el Carrier seleccionado. Asegurese de haber seleccionado el carrier correcto.");
                    }
                    else
                    {
                        //Alta de la Línea
                        lineaCC.AltaLinea(codLinea, iCodCatCarrier, iCodCatSitio, dtFechaInicio, "0");

                        //Query para extraer datos de la línea que se acaba de dar de alta
                        lsbQuery.Length = 0;
                        lsbQuery.AppendLine("SELECT iCodCatalogo,vchCodigo FROM [VisHistoricos('Linea','Lineas','Español')]");
                        lsbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
                        lsbQuery.AppendLine("  AND dtFinVigencia >= GETDATE()");
                        lsbQuery.AppendLine("  AND vchCodigo = '" + codLinea + "'");
                        lsbQuery.AppendLine("  AND Sitio = " + iCodCatSitio);
                        lsbQuery.AppendLine("  AND Carrier = " + iCodCatCarrier);
                        DataRow drLineaReciente = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

                        string lsiCodCatalgoLinea = drLineaReciente["iCodCatalogo"].ToString();
                        string lsVchCodLinea = drLineaReciente["vchCodigo"].ToString();

                        // Se hace un insert en [VisRelaciones('Empleado - Linea','Español')]  // Se hace un update a la vista [VisHistoricos('Linea','Lineas','Español')] en el campo Emple 
                        lineaCC.AltaRelacionEmpLinea(vchCodigoEmple, lsVchCodLinea, objDatosEmple.iCodCatemple.ToString(), lsiCodCatalgoLinea, dtFechaInicio);
                    }
                }
                else  //Si la fecha de inicio es menor a la fecha de alta del empleado manda un mensaje de notificación
                {
                    throw new ArgumentException("La fecha de inicio debe ser mayor o igual a la fecha de alta del empleado.");
                }
                #endregion
            }
        }

        public static string ValidaCamposLineaAlta(DatosEmple objDatosEmple)
        {
            StringBuilder sbErrors = new StringBuilder(string.Empty);

            string linea = objDatosEmple.NumLineaDirecta;
            string sitio = objDatosEmple.SitioDesc;
            string carrier = objDatosEmple.CarrierDesc;
            string fechaIni = objDatosEmple.FechaInicioVigencia;

            //if (linea == string.Empty || linea == "")
            //{
            //    sbErrors.Append(@"*El campo (Línea) es requerido. \n");
            //}
            //else if (linea.Length >= 16)
            //{
            //    sbErrors.Append(@"*La Longitud del campo (Línea) es muy grande. \n");
            //}
            //else
            //{
            //    if (!System.Text.RegularExpressions.Regex.IsMatch(linea, @"^\d*$"))  //"^([0-9]|[*])*$"
            //    {
            //        sbErrors.Append(@"*El campo (Línea) solo debe contener números. \n");
            //    }
            //}

            if (string.IsNullOrEmpty(sitio))
            {
                sbErrors.Append(@"*El campo (Sitio) es requerido. \n");
            }

            if (string.IsNullOrEmpty(carrier))
            {
                sbErrors.Append(@"*El campo (Carrier) es requerido. \n");
            }

            if (fechaIni == string.Empty || fechaIni == "")
            {
                sbErrors.Append(@"*El campo (Fecha inicio) es requerido. \n");
            }

            if (!NuevoEmpleadoBackend.validaFormatoFecha(fechaIni))
            {
                sbErrors.Append(@"*El formato de (Fecha inicio) debe ser DD/MM/AAAA. Favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA. \n");
            }


            //if (DSODataContext.Schema.ToUpper() == "FCA")
            //{

            //    if (ddlPentaSAPAccount.SelectedValue == "0")
            //    {
            //        sbErrors.AppendLine(@"*El campo (PSAP Account) es requerido. ");
            //    }

            //    if (ddlPentaSAPCCDesc.SelectedValue == "0")
            //    {
            //        sbErrors.AppendLine(@"*El campo (PentaSAP Cost Center Description) es requerido. ");
            //    }

            //    if (ddlPentaSAPCostCenter.SelectedValue == "0")
            //    {
            //        sbErrors.AppendLine(@"*El campo (PentaSAP Cost Center) es requerido. ");
            //    }

            //    if (ddlPentaSAPFAFCA.SelectedValue == "0")
            //    {
            //        sbErrors.AppendLine(@"*El campo (PentaSAP FA) es requerido. ");
            //    }

            //    if (ddlPentaSAPProfitCenter.SelectedValue == "0")
            //    {
            //        sbErrors.AppendLine(@"*El campo (PentaSAP Profit Center) es requerido. ");
            //    }
            //}

            return sbErrors.ToString();
        }

        private static string NoSQLCode(string texto)
        {
            return texto.Replace("'", "").ToUpper().Replace(";", "")
                      .Replace("DROP", "").Replace("DELETE", "").Replace("INSERT", "").Replace("TRUNCATE", "").Replace("UPDATE", "")
                      .Replace("SELECT", "").Replace("FROM", "").Trim();
        }


        public static void IncializaCampos(Hashtable phtValuesEmple, string primerNombre, string segundoNombre, string apPaterno, string apMaterno)
        {
            //Obten el numero de nomina si no se capturo
            //string lsValue = "";
            DataTable ldt;
            StringBuilder psbQuery = new StringBuilder();

            //Incializa los valores de Codigo y Descripcion del Historico de Empleados.
            phtValuesEmple.Add("vchCodigo", phtValuesEmple["{NominaA}"].ToString());


            string lsNomEmpleado = primerNombre.Trim() + " " + segundoNombre.Trim() + " " +
                          apPaterno.Trim() + " " + apMaterno.Trim();

            if (DSODataContext.Schema.ToString().ToUpper() == "FCA")
            {
                lsNomEmpleado = apPaterno.Trim() + " " + apMaterno.Trim() + " " +
                    primerNombre.Trim() + " " + segundoNombre.Trim();

            }



            lsNomEmpleado = lsNomEmpleado.Replace("  ", " "); //Retirar espacios dobles entre el nombre completo

            phtValuesEmple.Add("{NomCompleto}", lsNomEmpleado.Trim());

            string lsCodEmpresa;

            int liCodCatalogo = int.Parse(phtValuesEmple["{CenCos}"].ToString());

            psbQuery.Length = 0;
            psbQuery.AppendLine("Select EmpreCod");
            psbQuery.AppendLine("from [VisHistoricos('CenCos','" + Globals.GetCurrentLanguage() + "')]");
            psbQuery.AppendLine("where iCodCatalogo = " + liCodCatalogo.ToString());
            psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia");
            psbQuery.AppendLine("and dtIniVigencia <= '" + Convert.ToDateTime(phtValuesEmple["dtIniVigencia"]).ToString("yyyy-MM-dd") + "'");
            psbQuery.AppendLine("and dtFinVigencia > '" + Convert.ToDateTime(phtValuesEmple["dtIniVigencia"]).ToString("yyyy-MM-dd") + "'");

            ldt = DSODataAccess.Execute(psbQuery.ToString());
            if (ldt == null || ldt.Rows.Count == 0 || ldt.Rows[0]["EmpreCod"] is DBNull)
            {
                //En caso de no poder agregar el emprecod en la descripcion del empleado, entonces el vchdescripcion quedara como el nomcompleto
                phtValuesEmple.Add("vchDescripcion", lsNomEmpleado.Trim());
                return;
            }
            lsCodEmpresa = ldt.Rows[0]["EmpreCod"].ToString();

            lsCodEmpresa = "(" + lsCodEmpresa.Substring(0, Math.Min(38, lsCodEmpresa.Length)) + ")";
            lsNomEmpleado = lsNomEmpleado.Trim();
            phtValuesEmple.Add("vchDescripcion", lsNomEmpleado.Substring(0, Math.Min(120, lsNomEmpleado.Length)) + lsCodEmpresa);

        }

        //protected bool ValidarClaves(Hashtable phtValuesEmple, int iCodCatDpto, string estado, int iCodCatemple)
        //{
        //    bool lbret = true;
        //    StringBuilder lsbErrores = new StringBuilder();
        //    StringBuilder lsbErroresCodigos = new StringBuilder();
        //    StringBuilder psbQuery = new StringBuilder();
        //    /*Extraer el icodregistro de la entidad de Empleados*/
        //    string iCodEntidad = "6";

        //    string lsError;
        //    string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloHistoricos"));
        //    DataTable ldt;

        //    IncializaCampos();

        //    string liCodEmpresa = "0";
        //    int liCodCatalogo = 0;

        //    if (DSODataContext.Schema.ToUpper() != "FCA")
        //    {
        //        int.TryParse(phtValuesEmple["{CenCos}"].ToString(), out liCodCatalogo);
        //    }
        //    else
        //    {
        //        int.TryParse(iCodCatDpto.ToString(), out liCodCatalogo);
        //    }


        //    psbQuery.Length = 0;
        //    psbQuery.AppendLine("Select Empre");
        //    psbQuery.AppendLine("from [VisHistoricos('CenCos','" + Globals.GetCurrentLanguage() + "')]");
        //    psbQuery.AppendLine("where iCodCatalogo = " + liCodCatalogo.ToString());

        //    ldt = DSODataAccess.Execute(psbQuery.ToString());

        //    if (ldt == null || ldt.Rows.Count == 0 || ldt.Rows[0]["Empre"] is DBNull)
        //    {
        //        string mensajeCCNoValido = "No se ha encontrado una empresa para el centro de costo. Elija un centro de costo válido";
        //        if (DSODataContext.Schema.ToUpper() == "FCA")
        //        {
        //            mensajeCCNoValido = "No se ha encontrado una empresa para el departamento. Elija un departamento válido"; ;
        //        }

        //        lsbErrores.Append("<li>" + mensajeCCNoValido + "</li>");

        //    }
        //    else
        //    {
        //        liCodEmpresa = ldt.Rows[0]["Empre"].ToString();
        //    }

        //    try
        //    {

        //        if (!String.IsNullOrEmpty(phtValuesEmple["{NominaA}"].ToString()))
        //        {
        //            //Valida el numero de Nomina que no se repita a menos que sea de diferente empresa
        //            psbQuery.Length = 0;
        //            psbQuery.AppendLine("select H.iCodRegistro, H.iCodCatalogo, H.iCodMaestro, H.dtIniVigencia, H.dtFinVigencia");
        //            psbQuery.AppendLine("from [" + DSODataContext.Schema + "].Historicos H, [" + DSODataContext.Schema + "].Catalogos C,");
        //            psbQuery.AppendLine("   [" + DSODataContext.Schema + "].[VisHistoricos('Emple','" + Globals.GetCurrentLanguage() + "')] V");
        //            psbQuery.AppendLine("where H.iCodRegistro = V.iCodRegistro");
        //            psbQuery.AppendLine("and H.iCodCatalogo = C.iCodRegistro");
        //            if (estado == "edit")
        //            {
        //                psbQuery.AppendLine("and H.iCodRegistro <> isnull(" + iCodCatemple + ",-1)"); //RZ. ojo con este campo validar
        //                psbQuery.AppendLine("and H.iCodCatalogo <> isnull(" + iCodCatemple + ",-1)");
        //            }


        //            psbQuery.AppendLine("and C.iCodCatalogo = " + iCodEntidad);
        //            psbQuery.AppendLine("and C.vchCodigo = '" + phtValuesEmple["{NominaA}"].ToString() + "'");
        //            psbQuery.AppendLine("and V.CenCos in (Select iCodCatalogo from ");
        //            psbQuery.AppendLine("   [" + DSODataContext.Schema + "].[VisHistoricos('CenCos','Español')] VC ");
        //            psbQuery.AppendLine("               Where Empre =  " + liCodEmpresa + ")");
        //            psbQuery.AppendLine("and H.dtIniVigencia <> H.dtFinVigencia");
        //            psbQuery.AppendLine("and ((H.dtIniVigencia <= '" + Convert.ToDateTime(phtValuesEmple["dtIniVigencia"]).ToString("yyyy-MM-dd hh:mm:ss.fff") + "'");
        //            psbQuery.AppendLine("       and H.dtFinVigencia > '" + Convert.ToDateTime(phtValuesEmple["dtIniVigencia"]).ToString("yyyy-MM-dd hh:mm:ss.fff") + "'" + ")");
        //            psbQuery.AppendLine("   or (H.dtIniVigencia <= '" + Convert.ToDateTime(phtValuesEmple["dtFinVigencia"]).ToString("yyyy-MM-dd hh:mm:ss.fff") + "'");
        //            psbQuery.AppendLine("       and H.dtFinVigencia > '" + Convert.ToDateTime(phtValuesEmple["dtFinVigencia"]).ToString("yyyy-MM-dd hh:mm:ss.fff") + "'" + ")");
        //            psbQuery.AppendLine("   or (H.dtIniVigencia >= '" + Convert.ToDateTime(phtValuesEmple["dtIniVigencia"]).ToString("yyyy-MM-dd hh:mm:ss.fff") + "'");
        //            psbQuery.AppendLine("       and H.dtFinVigencia <= '" + Convert.ToDateTime(phtValuesEmple["dtFinVigencia"]).ToString("yyyy-MM-dd hh:mm:ss.fff") + "'" + "))");
        //            psbQuery.AppendLine("order by H.dtIniVigencia, H.dtFinVigencia, H.iCodRegistro");

        //            ldt = DSODataAccess.Execute(psbQuery.ToString());

        //            string lsNetDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");
        //            foreach (DataRow ldataRow in ldt.Rows)
        //            {
        //                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", ((DateTime)ldataRow["dtIniVigencia"]).ToString(lsNetDateFormat), ((DateTime)ldataRow["dtFinVigencia"]).ToString(lsNetDateFormat)));
        //                lsbErroresCodigos.Append("<li>" + lsError + "</li>");
        //            }


        //            if (lsbErroresCodigos.Length > 0)
        //            {
        //                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValidaVchCodigo", "Clave para Empleado (nómina)"));
        //                lsError = "<span>" + lsError + "</span>";
        //                lsbErrores.Append("<li>" + lsError);
        //                lsbErrores.Append("<ul>" + lsbErroresCodigos.ToString() + "</ul>");
        //                lsbErrores.Append("</li>");
        //            }
        //        }

        //        else
        //        {

        //            lsbErrores.Append("<li>" + "El campo de Nómina es requerido" + "</li>");
        //        }

        //        if (String.IsNullOrEmpty(phtValuesEmple["vchDescripcion"].ToString()))
        //        {
        //            lsbErrores.Append("<li>" + "La descripción del empleado no se ha generado correctamente" + "</li>");
        //        }

        //        if (lsbErrores.Length > 0)
        //        {
        //            lbret = false;
        //            lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
        //            DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new KeytiaWebException("ErrValidateRecord", ex);
        //    }

        //    return lbret;
        //}


        public static bool IsExterno(Hashtable phtValuesEmple)
        {
            StringBuilder lsbQuery = new StringBuilder();

            lsbQuery.Length = 0;
            DateTime ldtIniVigencia;

            bool lbRet = false;

            //Obten el Tipo de empleado para determinar si es empleados
            int liCodCatalogo;

            if (phtValuesEmple.Contains("{TipoEm}"))
            {
                liCodCatalogo = int.Parse(phtValuesEmple["{TipoEm}"].ToString());
            }
            else
            {
                return lbRet;
            }

            if (phtValuesEmple.Contains("dtIniVigencia"))
            {
                ldtIniVigencia = Convert.ToDateTime(phtValuesEmple["dtIniVigencia"].ToString());
            }
            else
            {
                return lbRet;
            }

            lsbQuery.AppendLine("select vchCodigo from [VisHistoricos('TipoEm','" + Globals.GetCurrentLanguage() + "')]");
            lsbQuery.AppendLine("where iCodCatalogo = " + liCodCatalogo);
            lsbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia ");
            lsbQuery.AppendLine("and '" + ldtIniVigencia.Date.ToString("yyyy-MM-dd") + "' >= dtIniVigencia");
            lsbQuery.AppendLine("and '" + ldtIniVigencia.Date.ToString("yyyy-MM-dd") + "' < dtFinVigencia");
            DataTable ldt = DSODataAccess.Execute(lsbQuery.ToString());

            if (ldt != null && ldt.Rows.Count > 0 && !(ldt.Rows[0]["vchCodigo"] is DBNull)
                && ldt.Rows[0]["vchCodigo"].ToString() == "X")
            {
                lbRet = true;
            }

            return lbRet;
        }

        public static bool IsRespEmpleadoExterno(Hashtable phtValuesEmple)
        {

            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.Length = 0;
            DateTime ldtIniVigencia;

            bool lbRet = false;

            //Obten el Tipo de empleado para determinar si es empleados
            int liCodCatalogo;

            if (phtValuesEmple.Contains("{Emple}"))
            {
                liCodCatalogo = int.Parse(phtValuesEmple["{Emple}"].ToString());
            }
            else
            {
                return lbRet;
            }

            if (phtValuesEmple.Contains("dtIniVigencia"))
            {
                ldtIniVigencia = Convert.ToDateTime(phtValuesEmple["dtIniVigencia"].ToString());
            }
            else
            {
                return lbRet;
            }

            lsbQuery.AppendLine("select TipoEmCod from [VisHistoricos('Emple','" + Globals.GetCurrentLanguage() + "')]");
            lsbQuery.AppendLine("where iCodCatalogo = " + liCodCatalogo);
            lsbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia ");
            lsbQuery.AppendLine("and '" + ldtIniVigencia.Date.ToString("yyyy-MM-dd") + "' >= dtIniVigencia");
            lsbQuery.AppendLine("and '" + ldtIniVigencia.Date.ToString("yyyy-MM-dd") + "' < dtFinVigencia");

            DataTable ldt = DSODataAccess.Execute(lsbQuery.ToString());
            if (ldt == null || ldt.Rows.Count == 0 || ldt.Rows[0]["TipoEmCod"] is DBNull)
            {
                return lbRet;
            }

            if (ldt.Rows[0]["TipoEmCod"].ToString() == "E" || ldt.Rows[0]["TipoEmCod"].ToString() == "X")
            {
                lbRet = true;
            }

            return lbRet;
        }

        public static bool IsRespEmpleadoSame(Hashtable phtValuesEmple, string iCodCatalogoEmple)
        {
            bool lbValida = false;

            if (iCodCatalogoEmple != String.Empty)
            {
                return lbValida; //se trata de alta, no de edición
            }

            if (phtValuesEmple.Contains("{Emple}")) //valida si se selecciono algo en jefe inmediato.
            {
                if (phtValuesEmple["{Emple}"].ToString() == iCodCatalogoEmple) //el mismo empleado como jefe
                {
                    lbValida = true;
                }
            }

            return lbValida;
        }


        public static string UsuarioAsignado(Hashtable phtValuesEmple, string iCodCatalogoEmple)
        {
            StringBuilder lsbQuery = new StringBuilder();

            string lbRet = "";
            DataTable ldt;
            int liCodUsuario = 0;
            lsbQuery.Length = 0;
            string iCodCatUsuar = phtValuesEmple["{Usuar}"].ToString();
            //Obten el usuario si se capturo

            if (iCodCatUsuar != "null")
            {
                liCodUsuario = int.Parse(phtValuesEmple["{Usuar}"].ToString());
            }

            if (liCodUsuario == 0)
            {
                return lbRet;
            }

            int liCodCatalogo = -1;
            DateTime ldtIniVigencia;

            if (phtValuesEmple.Contains("dtIniVigencia"))
            {
                ldtIniVigencia = Convert.ToDateTime(phtValuesEmple["dtIniVigencia"].ToString());
            }
            else
            {
                return lbRet;
            }

            DateTime ldtFinVigencia = Convert.ToDateTime(phtValuesEmple["dtFinVigencia"].ToString());

            if (!String.IsNullOrEmpty(iCodCatalogoEmple))
            {
                liCodCatalogo = int.Parse(iCodCatalogoEmple);
            }

            lsbQuery.AppendLine("Select icodcatalogo from [VisHistoricos('Emple','Empleados','" + Globals.GetCurrentLanguage() + "')] ");
            lsbQuery.AppendLine("Where iCodCatalogo <> " + liCodCatalogo);
            lsbQuery.AppendLine("and [Usuar] = " + liCodUsuario);
            lsbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia ");
            lsbQuery.AppendLine("and ('" + ldtIniVigencia.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' between dtIniVigencia and dtFinVigencia ");
            lsbQuery.AppendLine("or '" + ldtFinVigencia.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' between dtIniVigencia and dtFinVigencia )");
            ldt = DSODataAccess.Execute(lsbQuery.ToString());

            if (ldt.Rows.Count > 0)
            {
                lbRet = "ValUsuarioAsignado";
            }

            return lbRet;
        }

        public static string GeneraUsuario(Hashtable phtValuesEmple, string UsuarDesc, string iCodUsuario, string iCodUsuarioDB)
        {
            /*RZ.20130730 Nuevos campos para usuarios*/
            string psNewUsuario;
            string psNewPassword;

            //Crea el usuario deacuerdo a lo seleccionado por el usuario
            psNewUsuario = UsuarDesc;

            if (psNewUsuario == String.Empty)
            {
                return "ErrCrearUsuario";
            }

            //'Crea el usuario deacuerdo a lo seleccionado por el usuario
            psNewPassword = NuevoEmpleadoBackend.ObtenPassword();

            string lsError = NuevoEmpleadoBackend.ExiUsuarioEmailPassword(phtValuesEmple["{Email}"].ToString(), psNewUsuario, psNewPassword);

            if (lsError != "")
            {
                return lsError;
            }
            int liCodRegistro = NuevoEmpleadoBackend.GrabarUsuario(phtValuesEmple, psNewUsuario, psNewPassword, iCodUsuario, iCodUsuarioDB);
            if (liCodRegistro > 0)
            {
                // Asigna el nuevo Usuario.
                phtValuesEmple["{Usuar}"] = liCodRegistro.ToString();

                DALCCustodia guardaUsuarBitacora = new DALCCustodia();

                guardaUsuarBitacora.guardaHistRecurso(liCodRegistro.ToString(), "Usu", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), "A");
            }
            else
            {
                return "ErrCrearUsuario";
            }

            return "";

        }


        public static int GrabarUsuario(Hashtable phtValuesEmple, string psNewUsuar, string psNewPass, string iCodUsuario, string iCodUsuarioDB)
        {
            Hashtable lhtValues;
            try
            {
                KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
                int liCodRegistro = 0;
                lhtValues = NuevoEmpleadoBackend.ObtenDatosUsuario(phtValuesEmple, psNewUsuar, psNewPass, iCodUsuario);

                //Mandar llamar al COM para grabar el usuario 
                liCodRegistro = lCargasCOM.GuardaUsuario(lhtValues, false, false, Convert.ToInt32(iCodUsuarioDB));

                return liCodRegistro;

            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveRecord", ex);
            }
        }

        public static string ExiUsuarioEmailPassword(string email, string psNewUsuario, string psNewPassword)
        {
            String lbret = "";

            Usuarios oUsuario = new Usuarios();

            oUsuario.vchEmail = email;
            oUsuario.vchCodUsuario = psNewUsuario;
            oUsuario.vchPwdUsuario = psNewPassword;

            lbret = oUsuario.ValUsuarioEmailPassword();

            return lbret;
        }


        public static Hashtable ObtenDatosUsuario(Hashtable phtValuesEmple, string psNewUsuario, string psNewPassword, string iCodUsuario)
        {

            KDBAccess pKDB = new KDBAccess();
            Hashtable lhtValues = new Hashtable();
            int liCodPerfil;
            DataTable ldt;
            DateTime ldtIniVigencia = Convert.ToDateTime(phtValuesEmple["dtIniVigencia"].ToString());
            DateTime ldtFinVigencia = Convert.ToDateTime(phtValuesEmple["dtFinVigencia"].ToString());

            string lsEmail = phtValuesEmple["{Email}"].ToString();

            int liCodMaestro = int.Parse(DSODataAccess.ExecuteScalar("select iCodRegistro from Maestros where vchDescripcion = 'Usuarios' and iCodEntidad = (Select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'Usuar') and dtIniVigencia <> dtFinVigencia").ToString());

            lhtValues.Add("vchCodigo", "'" + psNewUsuario + "'");
            lhtValues.Add("iCodMaestro", liCodMaestro);
            lhtValues.Add("vchDescripcion", "'" + phtValuesEmple["vchDescripcion"].ToString() + "'");
            lhtValues.Add("dtIniVigencia", ldtIniVigencia);
            lhtValues.Add("dtFinVigencia", ldtFinVigencia);
            lhtValues.Add("iCodUsuario", iCodUsuario);

            lhtValues.Add("{Email}", "'" + lsEmail + "'");
            lhtValues.Add("{UsuarDB}", iCodUsuario);

            /*RZ.20130801 Se retira este homepage y se pone el que se requiere para que 
             * el usuario entre directo a la Carta Custodia */
            lhtValues.Add("{HomePage}", "'~/UserInterface/Dashboard/Dashboard.aspx?Opc=OpcdshEmpleado'");

            /*RZ.20131106 Se deja como perfil default el tipo empleado */
            ldt = pKDB.GetHisRegByEnt("Perfil", "Perfiles", "vchCodigo ='Epmpl' ");
            if (ldt != null && !(ldt.Rows[0]["iCodCatalogo"] is DBNull))
            {
                liCodPerfil = (int)ldt.Rows[0]["iCodCatalogo"];
                lhtValues.Add("{Perfil}", liCodPerfil);
            }

            lhtValues.Add("{Password}", "'" + psNewPassword + "'");
            lhtValues.Add("{ConfPassword}", "'" + psNewPassword + "'");

            ldt = pKDB.GetHisRegByEnt("Usuar", "Usuarios", "iCodCatalogo =" + iCodUsuario);
            if (ldt != null && !(ldt.Rows[0]["{Empre}"] is DBNull))
            {
                lhtValues.Add("{Empre}", ldt.Rows[0]["{Empre}"]);
            }

            return lhtValues;
        }



        public static string ObtenPassword()
        {
            string lsPassword = "";
            GeneradorPassword oGenPws = new GeneradorPassword();

            lsPassword = oGenPws.GetNewPassword();
            if (lsPassword != "")
            {
                lsPassword = KeytiaServiceBL.Util.Encrypt(lsPassword);
            }

            return lsPassword;
        }
        public static string ObtienePWD(string password)
        {
            string lsPassword = "";
            if (password != "")
            {
                lsPassword = KeytiaServiceBL.Util.Encrypt(password);
            }

            return lsPassword;
        }

        public static string GetMsgError(string lsDesCampo, string lsMsgError)
        {
            string lsError = "";
            string lsValue = "";

            lsValue = lsDesCampo;

            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, lsMsgError, lsValue));
            lsError = "<span>" + lsError + "</span>";

            return lsError;
        }

        ////RZ.20130729 Se agrega metodo para validar la entrada de los datos
        //protected bool ValidaDatoEmpleados()
        //{
        //    bool lbRet = true;
        //    string lsError;
        //    StringBuilder lsbErrores = new StringBuilder();
        //    string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloEmpleados"));

        //    string lsValue;

        //    lsbErrores.Length = 0;

        //    // Valida el numero de Nomina
        //    lsValue = phtValuesEmple["{NominaA}"].ToString();
        //    if (lsValue.Length > 40 ||
        //        !System.Text.RegularExpressions.Regex.IsMatch(lsValue, "^([a-zA-Z]*[0-9]*[-]*[/]*[_]*[:]*[.]*[|]*)*$"))
        //    {
        //        lsError = GetMsgError("Nómina", "ValEmplFormato");
        //        lsbErrores.Append("<li>" + lsError);
        //    }

        //    // Valida el Nombre
        //    lsValue = phtValuesEmple["{Nombre}"].ToString();
        //    if (lsValue == "" || lsValue.Contains(","))
        //    {
        //        lsError = GetMsgError("Nombre", "ValEmplFormato");
        //        lsbErrores.Append("<li>" + lsError);
        //    }

        //    // Valida el Segundo Nombre
        //    lsValue = txtSegundoNombreEmple.Text;
        //    if (lsValue.Contains(","))
        //    {
        //        lsError = GetMsgError("Segundo Nombre", "ValEmplFormato");
        //        lsbErrores.Append("<li>" + lsError);
        //    }

        //    // Valida el Apellido Paterno
        //    lsValue = phtValuesEmple["{Paterno}"].ToString();
        //    if (lsValue.Contains(","))
        //    {
        //        lsError = GetMsgError("Apellido Paterno", "ValEmplFormato");
        //        lsbErrores.Append("<li>" + lsError);
        //    }

        //    // Valida el Apellido Materno
        //    lsValue = phtValuesEmple["{Materno}"].ToString();
        //    if (lsValue.Contains(","))
        //    {
        //        lsError = GetMsgError("Apellido Materno", "ValEmplFormato");
        //        lsbErrores.Append("<li>" + lsError);
        //    }

        //    // Valida la cuenta de correo Formato
        //    lsValue = phtValuesEmple["{Email}"].ToString();
        //    if (lsValue.Length > 0)
        //    {
        //        //RM 20190514 Expresion que se usaba anteriormente
        //        /*
        //         * ^(([\\w-]+\\.)+[\\w-]+|([a-zA-Z]{1}|[\\w-]{2,}))" + "@" +
        //            "((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\\.+([0-1]?[0-9]{1,2}|25[0-5]" +
        //            "|2[0-4][0-9])\\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z]+[\\w-]+\\.)+[a-zA-Z]{2,4})$
        //         */


        //        string pattern = @"^([^(áéíóúÁÉÍÓÚ()<>@,;:\[\] ç % &]+)(@)([^ áéíóúÁÉÍÓÚ() <>@,;:\[\]ç%&]{3,})([.][\w]{2,}){1,3}$";
        //        if (!System.Text.RegularExpressions.Regex.IsMatch(lsValue, pattern))
        //        {
        //            lsError = GetMsgError("E-mail", "ValEmplFormato");
        //            lsbErrores.Append("<li>" + lsError);
        //        }
        //    }
        //    // No se puede asignar al mismo empleado como responsable
        //    if (IsRespEmpleadoSame())
        //    {
        //        lsError = GetMsgError("Jefe Inmediato", "ErrEmplRespSame");
        //        lsbErrores.Append("<li>" + lsError);
        //    }
        //    // Valida si se captura el jefe debe ser un empleado
        //    if (phtValuesEmple.Contains("{Emple}"))
        //    {
        //        lsValue = phtValuesEmple["{Emple}"].ToString();

        //        // Es empleado debe ser un empleado
        //        // NZ 20160608 Se quita esta validación a peticion de RJ
        //        //if (IsEmpleado() && !IsRespEmpleado())
        //        //{
        //        //    lsError = GetMsgError("Jefe Inmediato", "ValJefeEmpleado");
        //        //    lsbErrores.Append("<li>" + lsError);
        //        //}
        //        // Es externo debe asignarsele un responsable que sea empleado o externo
        //        if (IsExterno() && !IsRespEmpleadoExterno())
        //        {
        //            lsError = GetMsgError("Jefe Inmediato", "ValJefeEmplExt");
        //            lsbErrores.Append("<li>" + lsError);
        //        }
        //    }
        //    //else
        //    //{
        //    //    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Jefe Inmediato"));
        //    //    lsbErrores.Append("<li>" + lsError + "</li>");
        //    //}

        //    //Validar que el usuario no este asignado a otro empleados
        //    lsError = UsuarioAsignado();

        //    if (lsError.Length > 0)
        //    {
        //        lsError = GetMsgError("Usuario", lsError);
        //        lsbErrores.Append("<li>" + lsError);
        //    }

        //    string lsvchCodUsuar = txtUsuarRedEmple.Text;
        //    DateTime ldtFinVigencia = Convert.ToDateTime(phtValuesEmple["dtFinVigencia"].ToString());

        //    //Si no hay errores entonces crear usuario, en caso de que no exista ya
        //    //RZ.20131201 Se retira llamada al metodo que valida si el empleado es tipo empleado para crear usuario && IsEmpleado()
        //    if (lsbErrores.Length == 0 && lsvchCodUsuar != String.Empty && phtValuesEmple["{Usuar}"].ToString() == "null")
        //    {
        //        if (ldtFinVigencia.Date > DateTime.Today)
        //        {
        //            lsError = GeneraUsuario();
        //            if (lsError != "")
        //            {
        //                lsError = GetMsgError("Usuario", lsError);
        //                lsbErrores.Append("<li>" + lsError);
        //            }
        //        }
        //    }


        //    if (lsbErrores.Length > 0)
        //    {
        //        lbRet = false;
        //        lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
        //        DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
        //    }

        //    return lbRet;
        //}

        //public static  bool IsRespEmpleadoSame(Hashtable phtValuesEmple, string iCodCatalogoEmple)
        //{
        //    bool lbValida = false;

        //    if (iCodCatalogoEmple != String.Empty)
        //    {
        //        return lbValida; //se trata de alta, no de edición
        //    }

        //    if (phtValuesEmple.Contains("{Emple}")) //valida si se selecciono algo en jefe inmediato.
        //    {
        //        if (phtValuesEmple["{Emple}"].ToString() == iCodCatalogoEmple) //el mismo empleado como jefe
        //        {
        //            lbValida = true;
        //        }
        //    }

        //    return lbValida;
        //}

        public static void IncializaNomina(Hashtable phtValuesEmple, string nomina)
        {
            //Obten el numero de nomina si no se capturo
            string lsValue = nomina;
            if (lsValue == "")
            {
                lsValue = ObtenNumeroNomina(phtValuesEmple);
                if (lsValue != "null")
                {
                    phtValuesEmple["{NominaA}"] = lsValue;
                }
            }
        }

        protected static string ObtenNumeroNomina(Hashtable phtValuesEmple)
        {

            /*RZ.20130719 Se agrega instancia a clasde KDBAAccess*/
            KDBAccess pKDB = new KDBAccess();

            DataTable ldt;
            DataRow[] ldr;

            string lsNomina = "null";
            string lsTipoEm = "";
            string lsColumField = "";
            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.Length = 0;

            //Obten el tipo de empleado si se capturo

            int liCodCatalogo = 0;

            if (phtValuesEmple.Contains("{TipoEm}"))
            {
                liCodCatalogo = int.Parse(phtValuesEmple["{TipoEm}"].ToString());

            }

            if (liCodCatalogo == 0)
            {
                return lsNomina;
            }

            //Obtiene los datos del tipo de Emplaedo
            ldt = pKDB.GetCatRegByEnt("TipoEm");
            ldr = ldt.Select("iCodRegistro = " + liCodCatalogo);

            //Si no Existe el tipo de Emplaedo
            if (ldr.Length == 0 || (ldr[0]["vchCodigo"] is DBNull))
            {
                return lsNomina;
            }

            lsTipoEm = ldr[0]["vchCodigo"].ToString();

            /*Si el tipo de empleado no es Recursos, Externo o Sistemas entonces no le puede
             generara nomina automaticamente*/
            if (lsTipoEm == "E")
            {
                return lsNomina;
            }

            /*Extraer el nombre del campo en la tabla historicos para el TipoEm*/
            lsColumField = "iCodCatalogo02";
            /*Extraer el icodregistro de la entidad de Empleados*/
            string iCodEntidad = "6";
            /*Extraer el icodregistro del maestro Empleados*/
            string iCodMaestro = DALCCustodia.getiCodMaestro("Empleados", "Emple");

            //Obten el numero de empleados con este tipo de empleado
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("Select Count = isnull(Count(*),0)  From Catalogos ");
            lsbQuery.AppendLine("Where iCodCatalogo = " + iCodEntidad);
            lsbQuery.AppendLine("And iCodRegistro in (Select iCodCatalogo From Historicos");
            lsbQuery.AppendLine("           			Where iCodMaestro = " + iCodMaestro + ")");
            /******************************************************************************************************************
            * AM 20130814 Se quita filtro de empleados externos
            Para adquirir la nomina se hacia un conteo del numero de registros de empleados con tipo externo, esto hacia que si 
            el NoNomina que se generaba era igual al NoNomina de algun empleado de otro tipo de empleado, marcaba error porque 
            las nominas no se pueden repetir. Al Comentar la siguiente linea se hace el conteo de todos los empleados sin importar
            el tipo.
             ******************************************************************************************************************/
            //lsbQuery.AppendLine("           			And " + lsColumField + " = " + liCodCatalogo + ")");

            int liCount = (int)DSODataAccess.ExecuteScalar(lsbQuery.ToString());

            liCount = liCount + 1;

            lsNomina = lsTipoEm.Trim() + liCount;

            return lsNomina;
        }


        //public static bool ValidarVigencias(Hashtable phtValuesEmple)
        //{

        //    //Para especificar en los jAlert de DSOControl
        //    string pjsObj;
        //    bool lbret = true;
        //    StringBuilder lsbErrores = new StringBuilder();
        //    string lsError;
        //    string lsTitulo = DSOControl.JScriptEncode("Empleados");
        //    pjsObj = "HistoricEdit1";

        //    DateTime pdtIniVigencia = Convert.ToDateTime(phtValuesEmple["dtIniVigencia"].ToString());

        //    DateTime pdtFinVigencia = Convert.ToDateTime(phtValuesEmple["dtFinVigencia"].ToString());

        //    try
        //    {
        //        if (String.IsNullOrEmpty(txtFecha.Text))
        //        {
        //            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Fecha Inicio"));
        //            lsbErrores.Append("<li>" + lsError + "</li>");
        //        }

        //        //Validar que fin de vigencia sea mayor o igual a inicio de vigencia
        //        if (pdtIniVigencia != null && pdtFinVigencia != null && pdtIniVigencia > pdtFinVigencia)
        //        {
        //            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "VigenciaFin", "Fecha Inicio", "Fecha Fin"));
        //            lsbErrores.Append("<li>" + lsError + "</li>");
        //        }

        //        if (lsbErrores.Length > 0)
        //        {
        //            lbret = false;
        //            lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
        //            DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new KeytiaWebException("ErrValidateRecord", ex);
        //    }

        //    return lbret;
        //}

        //public static bool ValidarCampos()
        //{

        //    /*Extraer el icodregistro de la entidad de Empleados*/
        //    //string iCodEntidad = "6";
        //    /*Extraer el icodregistro del maestro Empleados*/
        //    string iCodMaestro = DALCCustodia.getiCodMaestro("Empleados", "Emple");

        //    bool lbret = true;
        //    StringBuilder lsbErrores = new StringBuilder();
        //    DataRow lRowMaestro = DSODataAccess.ExecuteDataRow("select iCodRegistro from Maestros where iCodRegistro = " + iCodMaestro);

        //    string lsError;
        //    string lsTitulo = DSOControl.JScriptEncode("Empleados");

        //    try
        //    {

        //        if (DSODataContext.Schema.ToUpper() != "FCA")
        //        {
        //            if (!phtValuesEmple.ContainsKey("{CenCos}"))
        //            {
        //                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RelacionRequerida", "Centro de Costos"));
        //                lsbErrores.Append("<li>" + lsError + "</li>");
        //            }
        //        }


        //        if (!phtValuesEmple.ContainsKey("{TipoEm}"))
        //        {
        //            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Tipo de Empleado"));
        //            lsbErrores.Append("<li>" + lsError + "</li>");
        //        }

        //        if (!phtValuesEmple.ContainsKey("{Puesto}"))
        //        {
        //            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Puesto de Empleado"));
        //            lsbErrores.Append("<li>" + lsError + "</li>");
        //        }

        //        if (DSODataContext.Schema.ToUpper() == "FCA")
        //        {
        //            if (Convert.ToInt32(ddlDatosEmpleFCADpto.SelectedValue) <= 0)
        //            {
        //                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Departamento"));
        //                lsbErrores.Append("<li>" + lsError + "</li>");
        //            }


        //            if (txtDatosEmpleFCAT_ID.Text.Length <= 0)
        //            {
        //                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "T_ID"));
        //                lsbErrores.Append("<li>" + lsError + "</li>");
        //            }
        //        }

        //        if (String.IsNullOrEmpty(phtValuesEmple["{Nombre}"].ToString()))
        //        {
        //            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Nombre del Empleado"));
        //            lsbErrores.Append("<li>" + lsError + "</li>");
        //        }

        //        if (String.IsNullOrEmpty(phtValuesEmple["{NominaA}"].ToString()))
        //        {
        //            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Nómina del Empleado"));
        //            lsbErrores.Append("<li>" + lsError + "</li>");
        //        }

        //        //20161108 NZ Se agrega validacion de que si la bandera de omitir de la sincronización esta encendida entonces es necesario que
        //        //se introduzca un comentario para especificar el motivo.
        //        if (ckbOmiteSincro.Checked && ckbOmiteSincro.Visible == true && string.IsNullOrEmpty(txtComentariosSincro.Text.Trim()))
        //        {
        //            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Comentarios de Omitir de Sincronización"));
        //            lsbErrores.Append("<li>" + lsError + "</li>");
        //        }

        //        if (lsbErrores.Length > 0)
        //        {
        //            lbret = false;
        //            lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
        //            DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
        //        }
        //        return lbret;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new KeytiaWebException("ErrValidateRecord", ex);
        //    }
        //}


        public static Hashtable ObtenerHashEmpleado(DatosEmple objDatosEmple, string iCodUsuarioDB)
        {
            Hashtable lht = new Hashtable();
            DateTime ldtFechaInicio;
            DateTime.TryParse(objDatosEmple.FechaInicioVigencia, out ldtFechaInicio);

            if (!String.IsNullOrEmpty(objDatosEmple.DepartamentoDesc))
            {
                lht.Add("{CenCos}", objDatosEmple.ICodCatDepartamento);  //iCodCatalogo01
            }

            if (!String.IsNullOrEmpty(objDatosEmple.TipoEmpleDesc))
            {
                lht.Add("{TipoEm}", objDatosEmple.ICodCatTipoEmple.ToString()); //iCodCatalogo02
            }

            if (!String.IsNullOrEmpty(objDatosEmple.PuestoDesc))
            {
                lht.Add("{Puesto}", objDatosEmple.iCodCatPuesto);  //iCodCatalogo03
            }

            if (!String.IsNullOrEmpty(objDatosEmple.JefeDirectoDesc))
            {
                lht.Add("{Emple}", objDatosEmple.ICodCatJefeDirecto);
            }

            //No se capturo un usuario para el empleado
            if (!String.IsNullOrEmpty(objDatosEmple.UsuarDesc))
            {
                //Buscar si el usuario asignado ya existe, si no debe ser creado
                objDatosEmple.iCodCatUsuar = NuevoEmpleadoBackend.obtenCatalogoUsuarioAsignado(objDatosEmple.UsuarDesc, iCodUsuarioDB.ToString());

                //Saber si el usuario existe ligarlo al empleado
                if (objDatosEmple.iCodCatUsuar > 0)
                {
                    lht.Add("{Usuar}", objDatosEmple.UsuarDesc);
                }
                else
                {
                    //Se creará nuevo usuario
                    lht.Add("{Usuar}", "null");
                }

                //En caso de estar en edicion dar de baja el usuario anterior
                AsegurarBajaUsuarioEnEdit(objDatosEmple.iCodCatUsuar, objDatosEmple.estado, objDatosEmple.iCodCatemple.ToString());
            }
            else
            {


                //no se escribio un usuario
                lht.Add("{Usuar}", "null");
                //En caso de estar en edicion dar de baja el usuario anterior
                AsegurarBajaUsuarioEnEdit(-1, objDatosEmple.estado, objDatosEmple.iCodCatemple.ToString());
            }

            lht.Add("{TipoPr}", "null");
            lht.Add("{PeriodoPr}", "null");
            lht.Add("{Organizacion}", "null");

            lht.Add("{OpcCreaUsuar}", "0");   //{Integer01}

            int liBanderasEmple = 0;

            //RM Bandera esDirecctor
            if (objDatosEmple.EsDirector)
            {
                NuevoEmpleadoBackend.getValBandera("EmpleEsDirector", ref liBanderasEmple);
            }

            lht.Add("{BanderasEmple}", liBanderasEmple);
            lht.Add("{PresupFijo}", "null");
            lht.Add("{PresupProv}", "null");
            lht.Add("{Nombre}", objDatosEmple.PrimerNombre);  //VarChar01
            lht.Add("{Paterno}", objDatosEmple.ApPaterno); //VarChar02
            lht.Add("{Materno}", objDatosEmple.ApMaterno); //VarChar03
            lht.Add("{RFC}", "null");
            lht.Add("{Email}", objDatosEmple.Email);


            if (!String.IsNullOrEmpty(objDatosEmple.SitioDesc))
            {
                lht.Add("{Ubica}", objDatosEmple.SitioDesc);
            }

            if (objDatosEmple.Nomina != string.Empty)
            {
                lht.Add("{NominaA}", objDatosEmple.Nomina); //VarChar07
            }
            else
            {
                lht.Add("{NominaA}", String.Empty);
            }

            lht.Add("dtIniVigencia", ldtFechaInicio);
            lht.Add("iCodUsuario", (int)HttpContext.Current.Session["iCodUsuario"]);
            lht.Add("dtFecUltAct", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));



            if (objDatosEmple.estado == "alta" || String.IsNullOrEmpty(objDatosEmple.estado))
            {
                lht.Add("dtFinVigencia", new DateTime(2079, 1, 1));
            }

            if (objDatosEmple.estado == "edit")
            {
                DateTime ldtFinVigencia = Convert.ToDateTime(objDatosEmple.FechaFinVigencia);
                lht.Add("dtFinVigencia", ldtFinVigencia);
            }

            return lht;
        }

        public static void AsegurarBajaUsuarioEnEdit(int iCodCatUsuarAsignado, string estado, string iCodCatalogoEmple)
        {
            DALCCustodia usuar = new DALCCustodia();
            if (estado == "edit" && iCodCatalogoEmple != String.Empty && validaUsuarPrevio(iCodCatUsuarAsignado, iCodCatalogoEmple))
            {
                usuar.DarDeBajaUsuario(iCodCatalogoEmple, DateTime.Today);
            }
        }


        /*Validar si el empleado en edicion tenia un usuario anteriormente ligado*/
        public static bool validaUsuarPrevio(int iCodCatUsuarioAsignado, string iCodCatalogoEmple)
        {
            bool lbValidaUsuar = false;
            string lsConsulta;
            int iCodCatUsuar = 0;

            lsConsulta = "select isnull(usuar,0) " +
                        " from [VisHistoricos('Emple','Empleados','Español')] " +
                        " where icodcatalogo = " + iCodCatalogoEmple +
                        " and dtIniVigencia<>dtFinVigencia " +
                        " and dtFinVigencia >= getdate()" +
                        " and Usuar <> " + iCodCatUsuarioAsignado.ToString();

            DataRow ldr = DSODataAccess.ExecuteDataRow(lsConsulta);

            if (ldr != null)
            {
                iCodCatUsuar = (int)ldr[0];
            }

            if (iCodCatUsuar > 0)
            {
                lbValidaUsuar = true;
            }

            return lbValidaUsuar;
        }


        public static bool AltaEmpleFCA(Dictionary<string, string> dctEmpleFCA)
        {
            try
            {
                bool res = false;


                StringBuilder query = new StringBuilder();

                query.AppendLine("Exec[AltaEmpleadoFCA]");
                query.AppendLine("    @schema = '" + DSODataContext.Schema + "',");
                query.AppendLine("    @iCodCatEmple = " + dctEmpleFCA["iCodCatEmple"] + ",");
                query.AppendLine("    @iCodCatPlantaFCA = " + dctEmpleFCA["planta"] + ",");
                query.AppendLine("    @iCodCatDirector = " + dctEmpleFCA["iCodCatDiretor"] + ",");
                query.AppendLine("    @FCA_C_ID = '" + dctEmpleFCA["Dc_id"] + "',");
                query.AppendLine("    @FCA_T_ID = '" + dctEmpleFCA["T_id"] + "',");
                query.AppendLine("    @FCAestacion = '" + dctEmpleFCA["Estacion"] + "'");


                DSODataAccess.Execute(query.ToString());
                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static void ActualizaJerarquiaRest(string iCodCatalogoEmple, DatosEmple objDatosEmple, Hashtable phtValuesEmple, string iCodUsuarioDB)
        {
            string iCodPadre = "";
            //RZ.20130812 Solo se irá a consultar el jefe inmediato cuando no se este en la alta.
            if (objDatosEmple.estado != String.Empty || objDatosEmple.estado != "alta")
            {
                iCodPadre = DALCCustodia.getiCodCatHist(iCodCatalogoEmple, "Emple", "Empleados", "iCodCatalogo", "isnull(convert(varchar,Emple),'')");
            }
            else
            {
                if (phtValuesEmple.ContainsKey("{Emple}"))
                {
                    iCodPadre = phtValuesEmple["{Emple}"].ToString();
                }
            }

            KeytiaCOM.ICargasCOM pCargaCom = (KeytiaCOM.ICargasCOM)Marshal.BindToMoniker("queue:/new:KeytiaCOM.CargasCOM");

            int liCodUsuario = int.Parse(iCodUsuarioDB);
            pCargaCom.ActualizaJerarquiaRestEmple(iCodCatalogoEmple, iCodPadre, liCodUsuario);
            Marshal.ReleaseComObject(pCargaCom);
            pCargaCom = null;
        }

        public static void ProcesoAltaDeCodAuto(DatosEmple objDatosEmple)
        {

            StringBuilder lsbQuery = new StringBuilder();

            //Mapeo de Campos
            string codAutoCod = NoSQLCode(objDatosEmple.ClaveFAC);
            DateTime dtFechaInicioCodAuto = Convert.ToDateTime(objDatosEmple.FechaInicioVigencia);
            string iCodSitio = NoSQLCode(objDatosEmple.ICodCatSitio.ToString());
            string iCodCos = NoSQLCode(objDatosEmple.ICodCatCobertura.ToString());
            string vchCodigoEmple = NoSQLCode(objDatosEmple.Nomina);

            //Se crea un objeto con todos los datos del nuevo codigó de autorización
            DALCCustodia CodAuto = new DALCCustodia();

            //Query para ver si codigó de autorización ya existe
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("select iCodCatalogo, vchCodigo from [VisHistoricos('CodAuto','Codigo Autorizacion','Español')]");
            lsbQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
            lsbQuery.AppendLine("and dtfinvigencia >= getdate()");
            lsbQuery.AppendLine("and vchCodigo = '" + codAutoCod + "'");
            lsbQuery.AppendLine("and Sitio = " + iCodSitio);
            DataRow drExisteCodAuto = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

            //El codigó de autorización ya existe ?
            if (drExisteCodAuto != null)
            {
                #region El Código ya existe

                string lsiCodCatalogoCodAut = drExisteCodAuto["iCodCatalogo"].ToString();

                //Query para ver si el codigó de autorización ya tiene una relación con otro empleado.
                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select Emple, EmpleDesc from [VisRelaciones('Empleado - CodAutorizacion','Español')]");
                lsbQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
                lsbQuery.AppendLine("and dtfinvigencia >= getdate()");
                lsbQuery.AppendLine("and CodAuto = " + lsiCodCatalogoCodAut);
                DataRow drRelEmpCodAut = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

                //El codigó de autorización esta asignado a otro empleado ?
                if (drRelEmpCodAut != null)
                {
                    #region El Código le pertenece a otro Empleado
                    string nombreEmpleRel = drRelEmpCodAut["EmpleDesc"].ToString();

                    TextInfo miInfo = CultureInfo.CurrentCulture.TextInfo;
                    int inicioDeParentesis = nombreEmpleRel.IndexOf("(") + 1;
                    char[] parentesis = { ')', '(' };
                    string nombreEmpleado = nombreEmpleRel.Substring(0, inicioDeParentesis).TrimEnd(parentesis).Trim();

                    throw new ArgumentException("El codigó de autorización que selecciono ya esta asignada al empleado : " + miInfo.ToTitleCase(nombreEmpleado.ToLower()));

                    #endregion
                }

                string fechaInicioValida = ValidarTraslapeFechas(dtFechaInicioCodAuto, lsiCodCatalogoCodAut, "CodAuto", "Empleado - CodAutorizacion", "null");

                if (fechaInicioValida == "1")
                {
                    string lsiCodCatalgoCodAuto = drExisteCodAuto["iCodCatalogo"].ToString();
                    string lsVchCodCodAuto = drExisteCodAuto["vchCodigo"].ToString();

                    //20140424 AM. Se agrega validacion para que la fecha inicio del codigo no pueda ser menor a la fecha inicio del empleado
                    string lsdtFechaInicioEmple = DSODataAccess.ExecuteScalar("select dtIniVigencia from [VisHistoricos('Emple','Empleados','Español')] " +
                                            "where iCodCatalogo = " + objDatosEmple.iCodCatemple +
                                            "and dtIniVigencia<>dtFinVigencia and dtFinVigencia >= getdate()").ToString();

                    DateTime dtFechaInicioEmple = Convert.ToDateTime(lsdtFechaInicioEmple);

                    //Si la fecha de inicio es mayor o igual a la fecha de alta del empleado entra a este bloque
                    if (dtFechaInicioCodAuto >= dtFechaInicioEmple)
                    {
                        // Se hace un insert en [VisRelaciones('Empleado - CodAutorizacion','Español')] 
                        // Se hace un update a la vista [VisHistoricos('CodAuto','Codigo Autorizacion','Español')] en el campo Emple 
                        CodAuto.altaRelacionEmpCodAuto(vchCodigoEmple, lsVchCodCodAuto, objDatosEmple.iCodCatemple.ToString(), lsiCodCatalgoCodAuto, dtFechaInicioCodAuto);
                    }
                    else //Si la fecha de inicio es menor a la fecha de alta del empleado manda un mensaje de notificación
                    {
                        throw new ArgumentException("La fecha de inicio debe ser mayor o igual a la fecha de alta del empleado.");
                    }
                }
                else //La fecha de inicio no es valida
                {

                    throw new ArgumentException("La fecha que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                }

                #endregion
            }
            else //El codigó de autorización no existe, entonces entro a este bloque
            {
                #region El Código no existe Activo

                //Validar si existio en el pasado y que ya no esta activa, para revisar traslape de relaciones.
                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select iCodCatalogo,vchCodigo from [VisHistoricos('CodAuto','Codigo Autorizacion','Español')]");
                lsbQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
                lsbQuery.AppendLine("and vchCodigo = '" + codAutoCod + "'");
                lsbQuery.AppendLine("and Sitio = " + iCodSitio);
                DataRow drCodAutoPasado = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

                if (drCodAutoPasado != null)
                {
                    //Se valida traslape por si en el pasado ya habia existido.
                    string fechaInicioValida = ValidarTraslapeFechas(dtFechaInicioCodAuto, drCodAutoPasado["iCodCatalogo"].ToString(), "CodAuto", "Empleado - CodAutorizacion", "null");
                    if (fechaInicioValida != "1")
                    {

                        throw new ArgumentException("La fecha que selecciono entra dentro de los rangos de relación de otro empleado, favor de seleccionar una fecha validá.");
                    }
                }

                //Query para ver la descripcion del sitio
                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select vchDescripcion ");
                lsbQuery.AppendLine("from Historicos H ");
                lsbQuery.AppendLine("JOIN (select CatElemento.iCodRegistro ");
                lsbQuery.AppendLine("		from Catalogos CatElemento ");
                lsbQuery.AppendLine("		JOIN Catalogos CatEntidad ");
                lsbQuery.AppendLine("			ON CatElemento.iCodCatalogo = CatEntidad.iCodRegistro ");
                lsbQuery.AppendLine("			and CatEntidad.vchCodigo = 'Sitio' ");
                lsbQuery.AppendLine("			and CatEntidad.iCodCatalogo IS NULL ");
                lsbQuery.AppendLine("	) as Catalogos ");
                lsbQuery.AppendLine("	ON H.iCodCatalogo = Catalogos.iCodRegistro ");
                lsbQuery.AppendLine("where H.dtinivigencia <> H.dtfinvigencia ");
                lsbQuery.AppendLine("and H.dtfinvigencia >= getdate() ");
                lsbQuery.AppendFormat("and H.iCodCatalogo = {0} ", iCodSitio);

                DataRow drVchDescSitio = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

                string lsSitioDesc = drVchDescSitio["vchDescripcion"].ToString();

                //20140424 AM. Se agrega validacion para que la fecha inicio del codigo no pueda ser menor a la fecha inicio del empleado
                string lsdtFechaInicioEmple = DSODataAccess.ExecuteScalar("select dtIniVigencia from [VisHistoricos('Emple','Empleados','Español')] " +
                                        "where iCodCatalogo = " + objDatosEmple.iCodCatemple.ToString() +
                                        "and dtIniVigencia<>dtFinVigencia and dtFinVigencia >= getdate()").ToString();

                DateTime dtFechaInicioEmple = Convert.ToDateTime(lsdtFechaInicioEmple);

                //Si la fecha de inicio es mayor o igual a la fecha de alta del empleado entra a este bloque
                if (dtFechaInicioCodAuto >= dtFechaInicioEmple)
                {
                    //Alta de el codigó de autorización
                    
                    if (iCodCos == "0")
                    {
                        iCodCos = null;
                    }

                    CodAuto.altaCodAuto(codAutoCod, lsSitioDesc, iCodSitio, iCodCos, dtFechaInicioCodAuto, "0");

                    //Query para extraer datos del codigo de autorizacion que se acaba de dar de alta
                    lsbQuery.Length = 0;
                    lsbQuery.AppendLine("select iCodCatalogo,vchCodigo from [VisHistoricos('CodAuto','Codigo Autorizacion','Español')]");
                    lsbQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
                    lsbQuery.AppendLine("and dtfinvigencia >= getdate()");
                    lsbQuery.AppendLine("and vchCodigo = '" + codAutoCod + "'");
                    lsbQuery.AppendLine("and Sitio = " + iCodSitio);
                    DataRow drCodAutoReciente = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

                    string lsiCodCatalgoCodAuto = drCodAutoReciente["iCodCatalogo"].ToString();
                    string lsVchCodCodAuto = drCodAutoReciente["vchCodigo"].ToString();

                    // Se hace un insert en [VisRelaciones('Empleado - CodAutorizacion','Español')] 
                    // Se hace un update a la vista [VisHistoricos('CodAuto','Codigo Autorizacion','Español')] en el campo Emple 
                    CodAuto.altaRelacionEmpCodAuto(vchCodigoEmple, lsVchCodCodAuto, objDatosEmple.iCodCatemple.ToString(), lsiCodCatalgoCodAuto, dtFechaInicioCodAuto);
                }
                else //Si la fecha de inicio es menor a la fecha de alta del empleado manda un mensaje de notificación
                {
                    throw new ArgumentException("La fecha de inicio debe ser mayor o igual a la fecha de alta del empleado.");
                }

                #endregion
            }
        }

        public static string ValidarTraslapeFechas(DateTime fechaAValidar, string iCodRecurso, string nombreCampoRecurso, string nombreVistaRel, string iCodRegistroRel)
        {

            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("EXEC ValidaHistoriaRecurso");
            lsbQuery.AppendLine("   @Esquema = '" + DSODataContext.Schema + "',");
            lsbQuery.AppendLine("   @iCodRecurso = " + iCodRecurso + ",");
            lsbQuery.AppendLine("   @fechaweb = '" + fechaAValidar.ToString("yyyy-MM-dd HH:mm:ss") + "',");
            lsbQuery.AppendLine("   @iCodRegistroRel = " + iCodRegistroRel + ",");
            lsbQuery.AppendLine("   @nombreCampoiCodRecurso  = '" + nombreCampoRecurso + "',");
            lsbQuery.AppendLine("   @RelacionTripleComilla = '''" + nombreVistaRel + "'''");
            lsbQuery.AppendLine("");

            return DSODataAccess.ExecuteScalar(lsbQuery.ToString()).ToString();
        }

        public static void ProcesoDeAltaExten(DatosEmple objDatosEmple)
        {

            StringBuilder lsbQuery = new StringBuilder();
            StringBuilder query = new StringBuilder();
            int icodTipoExtenPrincipal = 0;



            query.AppendLine("select icodcatalogo													");
            query.AppendLine("From [" + DSODataContext.Schema + "].[vishistoricos('TipoRecurso','Tipos de recurso','Español')]	");
            query.AppendLine("where dtIniVigencia <> dtFinVigencia									");
            query.AppendLine("And dtFinVigencia >= GETDATE()										");
            query.AppendLine("and vchCodigo = 'EXTENPRINC'											");

            DataTable dtRes = new DataTable();

            dtRes = DSODataAccess.Execute(query.ToString());

            if (dtRes.Rows.Count > 0 && dtRes.Columns.Count > 0)
            {
                int.TryParse(dtRes.Rows[0][0].ToString(), out icodTipoExtenPrincipal);
            }

            //AM.20131205 Se valida si el empleado cuenta con una extensión principal
            if (!validaTipoExtension(objDatosEmple.iCodCatemple.ToString()))
            {
                throw new ArgumentException("El empleado ya cuenta con una extensión principal, favor de seleccionar otro tipo de extensión.");
            }
            else //Si el tipo de extensión no es principal sigue con el proceso
            {
                //Mapeo de campos
                string extensionCod = NoSQLCode(objDatosEmple.Extension);
                DateTime dtFechaInicioExten = Convert.ToDateTime(objDatosEmple.FechaInicioVigencia);
                string vchCodigoEmple = NoSQLCode(objDatosEmple.Nomina);
                string iCodSitio = NoSQLCode(objDatosEmple.ICodCatSitio.ToString());
                string iCodTipoExten = NoSQLCode(icodTipoExtenPrincipal.ToString());
                string iCodVisibleDir = NoSQLCode("0");
                string iCodCos = NoSQLCode(objDatosEmple.ICodCatCobertura.ToString());
                string comentarios = NoSQLCode("");

                #region La extensión a dar de alta no es una extensión principal

                if (validaFechaInicioExten(dtFechaInicioExten, objDatosEmple))  //Inicio de la relación
                {
                    string psFechaInicio = dtFechaInicioExten.ToString("yyyy-MM-dd HH:mm:ss.fff");

                    //Se crea un objeto con todos los datos de la nueva extensión
                    DALCCustodia extension = new DALCCustodia();

                    //Se valida si la extensión ya existe
                    DataRow drExisteExtension = ExisteLaExtension(extensionCod, iCodSitio);

                    if (drExisteExtension != null)
                    {
                        #region La extensión si existe

                        string lsiCodCatalogoExten = drExisteExtension["iCodCatalogo"].ToString();
                        DataRow drRelEmpExtQuery = ExisteRelacion(extensionCod, lsiCodCatalogoExten);
                        string fechaInicioValida = ValidarTraslapeFechas(dtFechaInicioExten, lsiCodCatalogoExten, "Exten", "Empleado - Extension", "null");

                        if (fechaInicioValida == "1") //La fecha de inicio de la extensión si es valida
                        {
                            //La extensión esta asignada a otro empleado ?
                            if (drRelEmpExtQuery != null)
                            {
                                #region La extensión ya esta asignada a otro empleado

                                TextInfo miInfo;
                                string nombreEmpleado;
                                ObtieneNombreEmpleadoRelacionado(drRelEmpExtQuery, out miInfo, out nombreEmpleado);


                                throw new ArgumentException("La extensión que seleccionó ya esta asignada al empleado : " + miInfo.ToTitleCase(nombreEmpleado.ToLower()));

                                #endregion //Fin del bloque --La extensión ya esta asignada a otro empleado
                            }
                            else  //Si la extension no tiene relacion entra a este bloque para dar de Alta la relación 'Empleado - Extension' y da de alta un registro en ExtensionesB
                            {
                                #region Alta de la relaciones
                                //Variables que se necesitan extraer para mandar como parametros en el metodo que da de alta un registro en relaciones
                                int iCodCatalogoExten = (int)drExisteExtension["iCodCatalogo"];
                                string vchCodigoExten = drExisteExtension["vchCodigo"].ToString();

                                // Se hace un insert en [VisRelaciones('Empleado - Extension','Español')] 
                                // Se hace un update a la vista [VisHistoricos('Exten','Extensiones','Español')] en el campo Emple
                                extension.altaRelacionEmpExt(iCodCatalogoExten, vchCodigoExten, objDatosEmple.iCodCatemple.ToString(), vchCodigoEmple, dtFechaInicioExten.ToString());
                                #endregion //Fin de bloque --Proceso de Alta de la relación

                                #region Validar Extensión B
                                int lint;
                                string iCodRegistroExtenB = string.Empty;
                                if (drExisteExtension != null)
                                {
                                    iCodRegistroExtenB = ExisteRegistroVigenteEnExtenB(drExisteExtension["iCodCatalogo"].ToString());
                                }

                                if (int.TryParse(iCodRegistroExtenB, out lint)) //Ya existe Extensión B
                                {
                                    //Se Actualizan Atributos de Extensiones B
                                    DSODataAccess.ExecuteNonQuery("update [VisHistoricos('ExtenB','Extensiones B','Español')]" +
                                                                  "set TipoRecurso = " + iCodTipoExten + ", " +
                                                                  "     Comentarios = " + comentarios + ", " +
                                                                  "     dtFecUltAct = getdate()" +
                                                                  "where iCodRegistro = " + iCodRegistroExtenB);
                                }
                                else
                                {
                                    extension.altaEnExtensionesB(extensionCod, drExisteExtension["SitioDesc"].ToString(),
                                                       drExisteExtension["iCodCatalogo"].ToString(), iCodTipoExten, comentarios, dtFechaInicioExten);
                                }
                                #endregion //Fin de bloque --Validar Extensión B

                                ActualizaAtributosDeExtensiones(drExisteExtension["iCodCatalogo"].ToString(), iCodVisibleDir);
                            }
                        }
                        else //La fecha de inicio no es valida
                        {

                            throw new ArgumentException("La fecha que selecciono entra dentro de los rangos de relación de otro empleado, favor de seleccionar una fecha validá.");
                        }

                        #endregion //Fin del bloque --La extensión si existe
                    }
                    else
                    {
                        #region La extensión no existe Activa

                        DataRow[] drArrayRangosExtension;
                        DataRow drRangosExtension;
                        ConsultaRangosConfigEnSitio(out drArrayRangosExtension, out drRangosExtension, iCodSitio);

                        //Variables necesarias para mandar parametros a metodo de altaExtension
                        string lsSitioDesc = drRangosExtension["vchDescripcion"].ToString();
                        string lsLongitudExtFin = drRangosExtension["ExtFin"].ToString();

                        bool estaEnRango = extension.ExtEnRango(extensionCod, drArrayRangosExtension);

                        //Validar sí no esta dentro del rango 
                        if (!estaEnRango)
                        {
                            if (true)
                            {
                                #region Dar de alta nuevo rango de extensión

                                string lsRangosExt = drRangosExtension["RangosExt"].ToString();
                                string lsExtIni = drRangosExtension["ExtIni"].ToString();
                                string lsExtFin = drRangosExtension["ExtFin"].ToString();
                                string lsiCodMaestroSitio = drRangosExtension["iCodMaestro"].ToString();

                                extension.altaNuevoRango(iCodSitio, extensionCod, lsRangosExt, lsiCodMaestroSitio, lsExtIni, lsExtFin);
                                estaEnRango = true;

                                #endregion //Fin de bloque --Dar de alta nuevo rango de extensión
                            }
                            else
                            {
                                throw new ArgumentException("El rango de extensión no existe en el sitio, si desea continuar con el alta de la extensión debe seleccionar la bandera (Dar de alta nuevo rango de extensión)");
                            }
                        }

                        if (estaEnRango)
                        {
                            //Validar si existio en el pasado y que ya no esta activa, para revisar traslape de relaciones.
                            lsbQuery.Length = 0;
                            lsbQuery.AppendLine("select iCodCatalogo,vchCodigo from [VisHistoricos('Exten','Extensiones','Español')]");
                            lsbQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
                            lsbQuery.AppendLine("and vchCodigo = '" + extensionCod + "'");
                            lsbQuery.AppendLine("and Sitio = " + iCodSitio);
                            DataRow drExtenPasada = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

                            if (drExtenPasada != null)
                            {
                                //Se valida traslape por si en el pasado ya habia existido.
                                string fechaInicioValida = ValidarTraslapeFechas(dtFechaInicioExten, drExtenPasada["iCodCatalogo"].ToString(), "Exten", "Empleado - Extension", "null");
                                if (fechaInicioValida != "1")
                                {

                                    throw new ArgumentException("La fecha que selecciono entra dentro de los rangos de relación de otro empleado, favor de seleccionar una fecha validá.");
                                }
                            }

                            //Dar de alta la extensión 
                            if (iCodCos == "0")
                            {
                                iCodCos = null;
                            }

                            extension.altaExtension(extensionCod, iCodSitio, iCodCos, lsSitioDesc, dtFechaInicioExten, iCodTipoExten, comentarios, iCodVisibleDir);

                            #region Alta en relaciones

                            DataRow drExtensionReciente = ExisteLaExtension(extensionCod, iCodSitio);

                            //Variables que se necesitan extraer para mandar como parametros en el metodo que da de alta un registro en relaciones
                            int iCodCatalogoExten = (int)drExtensionReciente["iCodCatalogo"];
                            string vchCodigoExten = drExtensionReciente["vchCodigo"].ToString();

                            // Se hace un insert en [VisRelaciones('Empleado - Extension','Español')] 
                            // Se hace un update a la vista [VisHistoricos('Exten','Extensiones','Español')] en el campo Emple
                            extension.altaRelacionEmpExt(iCodCatalogoExten, vchCodigoExten, objDatosEmple.iCodCatemple.ToString(), vchCodigoEmple, dtFechaInicioExten.ToString());

                            #endregion //Fin de bloque --Alta en relaciones
                        }
                        else
                        {
                            throw new ArgumentException("El rango de extensión no existe en el sitio, si desea continuar con el alta de la extensión debe seleccionar la bandera (Dar de alta nuevo rango de extensión)");
                        }

                        #endregion //Fin de bloque --La extensión no existe
                    }
                }
                else //Si la fecha de inicio es menor a la fecha de alta del empleado manda un mensaje de notificación
                {
                    throw new ArgumentException("La fecha de inicio debe ser mayor a la fecha de alta del empleado.");
                }
                #endregion //Fin del bloque --La extensión a dar de alta no es una extensión principal
            }
        }

        //AM 20131205 Valida si el empleado ya tiene una extensión principal.
        /// <summary>
        /// Si el empleado ya cuenta con una extensión principal regresa un "false"
        /// </summary>
        /// <returns></returns>
        public static bool validaTipoExtension(string iCodCatalogoEmple)
        {
            bool lb = true;
            StringBuilder lsb = new StringBuilder();
            try
            {
                #region Consulta el numero de extensiones principales del empleado
                lsb.Length = 0;
                lsb.AppendLine("select COUNT(*)");
                lsb.AppendLine("from [VisHistoricos('Exten','Extensiones','Español')] Ext");
                lsb.AppendLine("inner join [VisHistoricos('ExtenB','Extensiones B','Español')] ExtB");
                lsb.AppendLine("    on Ext.iCodCatalogo = ExtB.Exten");
                lsb.AppendLine("    and ExtB.dtIniVigencia <> ExtB.dtFinVigencia");
                lsb.AppendLine("    and ExtB.dtfinVigencia >= getdate()");
                lsb.AppendLine("where Ext.dtIniVigencia <> Ext.dtFinVigencia");
                lsb.AppendLine("    and Ext.dtfinVigencia >= getdate()");
                lsb.AppendLine("    and Ext.Emple = " + iCodCatalogoEmple);
                lsb.AppendLine("    and TipoRecursoDesc like '%EXTENSION%PRINCIPAL%'");
                #endregion

                int liNumExten = (int)DSODataAccess.ExecuteScalar(lsb.ToString());

                if (liNumExten >= 1)
                {
                    lb = false;
                }
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException(
                    "Ocurrio un error al consultar el numero de extensiones principales en validaTipoExtension() en AppCCustodia.aspx.cs '"
                    + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }

            return lb;
        }

        public static bool validaFechaInicioExten(DateTime dtFechaIniExt, DatosEmple objDatosEmple)
        {
            bool lb = false;

            string lsdtFechaInicioEmple =
                DSODataAccess.ExecuteScalar("select dtIniVigencia from [VisHistoricos('Emple','Empleados','Español')] " +
                                            "where iCodCatalogo = " + objDatosEmple.iCodCatemple +
                                            "and dtIniVigencia<>dtFinVigencia and dtFinVigencia >= getdate()").ToString();

            DateTime dtFechaInicioEmple = Convert.ToDateTime(lsdtFechaInicioEmple);

            if (dtFechaIniExt >= dtFechaInicioEmple)
            {
                lb = true;
            }

            return lb;
        }

        private static DataRow ExisteLaExtension(string extensionCod, string iCodSitio)
        {
            StringBuilder lsbQuery = new StringBuilder();
            //Query para ver si la extensión ya existe
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("select iCodCatalogo, vchCodigo, SitioDesc from [VisHistoricos('Exten','Extensiones','Español')]");
            lsbQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
            lsbQuery.AppendLine("and dtfinvigencia >= getdate()");
            lsbQuery.AppendLine("and vchCodigo = '" + extensionCod + "'");
            lsbQuery.AppendLine("and Sitio = " + iCodSitio);
            DataRow drExisteExtension = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());
            return drExisteExtension;

        }

        private static DataRow ExisteRelacion(string extensionCod, string lsiCodCatalogoExten)
        {
            //Query para ver si la extensión ya tiene una relación con otro empleado.
            StringBuilder sbRelEmpExtQuery = new StringBuilder();
            sbRelEmpExtQuery.AppendLine("select EmpleDesc from [VisRelaciones('Empleado - Extension','Español')]");
            sbRelEmpExtQuery.AppendLine("where dtinivigencia <> dtfinvigencia");
            sbRelEmpExtQuery.AppendLine("and dtfinvigencia >= getdate()");
            sbRelEmpExtQuery.AppendLine("and Exten = " + lsiCodCatalogoExten);
            DataRow drRelEmpExtQuery = DSODataAccess.ExecuteDataRow(sbRelEmpExtQuery.ToString());
            return drRelEmpExtQuery;
        }

        private static void ObtieneNombreEmpleadoRelacionado(DataRow drRelEmpExtQuery, out TextInfo miInfo, out string nombreEmpleado)
        {
            string nombreEmpleRel = drRelEmpExtQuery["EmpleDesc"].ToString();

            miInfo = CultureInfo.CurrentCulture.TextInfo;
            int inicioDeParentesis = nombreEmpleRel.IndexOf("(") + 1;
            char[] parentesis = { ')', '(' };
            nombreEmpleado = nombreEmpleRel.Substring(0, inicioDeParentesis).TrimEnd(parentesis).Trim();
        }

        /// <summary>
        /// Consulta de registros vigentes en Extensiones B para la extensión que se quiere dar de alta.
        /// </summary>
        /// <param name="iCodCatalogoExtension">iCodCatalogo de la extensión que se desea dar de alta.</param>
        /// <returns>Regresa el iCodRegistro de extensiones B</returns>
        private static string ExisteRegistroVigenteEnExtenB(string iCodCatalogoExtension)
        {
            string iCodRegistroExtenB = string.Empty;

            try
            {
                #region Consulta para ver si existen registros vigentes en extensiones B

                StringBuilder sbQuery = new StringBuilder();
                sbQuery.AppendLine("select Max(iCodRegistro) from [VisHistoricos('ExtenB','Extensiones B','Español')]");
                sbQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
                sbQuery.AppendLine("and dtfinvigencia >= getdate()");
                sbQuery.AppendLine("and Exten = " + iCodCatalogoExtension);

                #endregion

                iCodRegistroExtenB = DSODataAccess.ExecuteScalar(sbQuery.ToString()).ToString();
            }

            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException(
                    "Ocurrio un error en metodo ExisteRegistroVigenteEnExtenB(string iCodCatalogoExtension) en AppCCustodia.aspx.cs '"
                    + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }

            return iCodRegistroExtenB;
        }

        private static void ActualizaAtributosDeExtensiones(string lsiCodExten, string visibleDir)
        {
            DSODataAccess.ExecuteNonQuery("update [VisHistoricos('Exten','Extensiones','Español')] " +
                                          "set BanderasExtens = " + visibleDir + "," +
                                          "    dtFecUltAct = getdate()" +
                                          "where iCodCatalogo = " + lsiCodExten +
                                          "and dtIniVigencia<>dtFinVigencia and dtFinVigencia >= getdate()");
        }


        private static void ConsultaRangosConfigEnSitio(out DataRow[] drArrayRangosExtension, out DataRow drRangosExtension, string iCodSitio)
        {
            StringBuilder lsbQuery = new StringBuilder();
            //Query para ver si la extensión entra dentro de los rangos configurados
            drArrayRangosExtension = new DataRow[1];
            lsbQuery.Length = 0;
            lsbQuery.AppendFormat("EXEC ObtieneRangosExtensiones @esquema = '{0}', @iCodCatSitio = {1}", DSODataContext.Schema, iCodSitio);
            drRangosExtension = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());
            drArrayRangosExtension[0] = drRangosExtension;
        }





    }
}
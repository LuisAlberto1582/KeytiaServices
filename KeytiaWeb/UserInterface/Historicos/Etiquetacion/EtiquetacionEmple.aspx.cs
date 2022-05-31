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
using KeytiaServiceBL.Reportes;
using KeytiaWeb.UserInterface.DashboardFC;
using System.Web.UI.HtmlControls;

namespace KeytiaWeb.UserInterface
{
    public partial class EtiquetacionEmple : System.Web.UI.Page
    {
        DataTable dtGrupoEtiqueta = new DataTable();
        List<EtiquetacionModelView> listaEtiq = new List<EtiquetacionModelView>();
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

                if (!Page.IsPostBack)
                {
                    GetNumsEmplePorEtiquetar();
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        #region Logica Resumen

        private void GetNumsEmplePorEtiquetar()
        {
            var dtResult = DSODataAccess.Execute(ConsultaResumenEtiq());
            if (dtResult.Rows.Count > 0)
            {
                dtGrupoEtiqueta = DSODataAccess.Execute(GetGruposEtiqueta());  //Esta linea se tiene que ejecutar antes del enlace de datos con el GRID

                gridResumenNums.DataSource = dtResult;
                gridResumenNums.DataBind();

                var totales = GetTotalesResumenEtiq(dtResult);
                gridResumenNums.FooterRow.Controls[0].Controls.Add(new Label() { Text = "Total" });
                gridResumenNums.FooterRow.Controls[1].Controls.Add(new Label() { Text = string.Format("{0:#,#}", Convert.ToInt32(totales["Cantidad"])) });
                gridResumenNums.FooterRow.Controls[2].Controls.Add(new Label() { Text = string.Format("{0:#,#}", Convert.ToInt32(totales["Duracion"].ToString())) });
                gridResumenNums.FooterRow.Controls[3].Controls.Add(new Label() { Text = string.Format("$ {0:#,#.##}", Convert.ToDouble(totales["Costo"].ToString())) });
            }
            else
            {
                gridResumenNums.DataSource = dtResult;
                gridResumenNums.DataBind();

                btnCancelar.Visible = false;
                btnGuardar.Visible = false;
            }
        }

        private string ConsultaResumenEtiq()
        {
            query.Length = 0;
            query.AppendLine("DECLARE @gpoEtiqNoIdent INT = (SELECT GEtiqueta FROM " + DSODataContext.Schema + ".[VisHistoricos('GpoEtiqueta','Grupo Etiquetacion','Español')]");
            query.AppendLine("								 WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()	AND vchCodigo = '0NoIdent' )");
            query.AppendLine("");
            query.AppendLine("DECLARE @emple INT = (SELECT MAX(iCodCatalogo) FROM " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')]");
            query.AppendLine("					    WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND Usuar = " + Session["iCodUsuario"].ToString() + ")");
            query.AppendLine("");
            query.AppendLine("DECLARE @llamsEntrada INT = (SELECT MAX(iCodCatalogo) FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]");
            query.AppendLine("							   WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = 'Ent')");
            query.AppendLine("");
            query.AppendLine("DECLARE @llamsEnlace INT = (SELECT MAX(iCodCatalogo) FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]");
            query.AppendLine("							  WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = 'Enl')");
            query.AppendLine("");
            query.AppendLine("SELECT ");
            query.AppendLine("       [Numero]				= TelDest");
            query.AppendLine("      ,[Cantidad]				= COUNT(*)");
            query.AppendLine("      ,[Duracion]				= SUM(DuracionMin)");
            query.AppendLine("      ,[Costo]				= '$ ' + CONVERT(VARCHAR, CONVERT(MONEY,ROUND(SUM(Costo + CostoSM),2)), 1)");
            query.AppendLine("      ,[Grupo]                = GEtiqueta");
            query.AppendLine("      ,[Etiqueta]             = ''");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisDetallados('Detall','DetalleCDR','Español')]");
            query.AppendLine("WHERE GEtiqueta = @gpoEtiqNoIdent");
            query.AppendLine("	AND Emple = @emple");
            query.AppendLine("	AND TDest <> @llamsEntrada");
            query.AppendLine("	AND TDest <> @llamsEnlace");
            query.AppendLine("	AND ( LEN(TelDest) = 3 OR LEN(TelDest) >= 7 )");
            query.AppendLine("GROUP BY TelDest, GEtiqueta");
            query.AppendLine("ORDER BY SUM(Costo + CostoSM) DESC");
            return query.ToString();
        }

        private DataRow GetTotalesResumenEtiq(DataTable dtResultResumen)
        {
            int cantidad = 0;
            int duracion = 0;
            double costo = 0;
            foreach (DataRow row in dtResultResumen.Rows)
            {
                cantidad += Convert.ToInt32(row["Cantidad"]);
                duracion += Convert.ToInt32(row["Duracion"]);
                costo += Convert.ToDouble(row["Costo"].ToString().Replace("$", "").Trim());
            }

            DataRow newRow = dtResultResumen.NewRow();
            newRow["Cantidad"] = cantidad;
            newRow["Duracion"] = duracion;
            newRow["Costo"] = costo.ToString();

            return newRow;
        }

        private string GetGruposEtiqueta()
        {
            query.Length = 0;
            query.AppendLine("SELECT vchDescripcion, GEtiqueta");
            query.AppendLine("FROM [VisHistoricos('GpoEtiqueta','Grupo Etiquetacion','Español')]");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            return query.ToString();
        }

        protected void OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlGrupoEtiqueta = (e.Row.FindControl("ddlGrupoEtiqueta") as DropDownList);
                ddlGrupoEtiqueta.DataSource = dtGrupoEtiqueta;
                ddlGrupoEtiqueta.DataTextField = "vchDescripcion";
                ddlGrupoEtiqueta.DataValueField = "GEtiqueta";
                ddlGrupoEtiqueta.DataBind();

                string grupo = (e.Row.FindControl("lblGrupo") as Label).Text;
                ddlGrupoEtiqueta.Items.FindByValue(grupo).Selected = true;
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            int numsEtiquetados = 0;
            numsEtiquetados = ProcesarGridResumenNums();

            if (ValidarActivoProcesoBloqueo() && numsEtiquetados > 0 && EsReactivacion())
            {
                DSODataAccess.ExecuteNonQuery(CambioEstatusAPendienteReactivacion());
            }

            if (numsEtiquetados > 0)
            {
                if (Session["iCodPerfil"].ToString() == "370")
                {
                    HttpContext.Current.Response.Redirect(Session["HomePage"].ToString());
                }
                else
                {
                    HttpContext.Current.Response.Redirect("~/UserInterface/DashboardFC/Dashboard.aspx?MiConsumo=1&Opc=opcMiConsumo");
                } 
            }
        }

        private bool ValidarActivoProcesoBloqueo()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT iCodCatalogo");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Client','Clientes','Español')]");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia ");
            query.AppendLine("	AND dtFinVigencia >= GETDATE()");
            query.AppendLine("	AND (ISNULL(BanderasCliente,0) & 262144)/262144 = 1"); //Bandera del proceso de bloqueo de codigos.
            query.AppendLine("  AND UsuarDBCod = '" + DSODataContext.Schema + "'");

            var dtResult = DSODataAccess.Execute(query.ToString());
            if (dtResult != null && dtResult.Rows.Count > 0)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        private string CambioEstatusAPendienteReactivacion()
        {
            StringBuilder query = new StringBuilder();           

            query.AppendLine("Declare @esquema varchar(100)= '" + DSODataContext.Schema + "' ");
            query.AppendLine("Declare @usuar int = " + Session["iCodUsuario"].ToString() + " ");
            query.AppendLine("Declare @query varchar(max) =''                              ");
            query.AppendLine("   ");
            query.AppendLine("   ");
            query.AppendLine("Set @query ='                                                ");
            query.AppendLine("Declare @querySecundario varchar(max) =''''                  ");
            query.AppendLine("Declare @arregloCodigosInicializados varchar(max) =''''      ");
            query.AppendLine("Declare @contador	int		=	1                              ");
            query.AppendLine("Declare @numReg		int		=	0                          ");
            query.AppendLine("Declare @codigoActual varchar(20)=''''                       ");
            query.AppendLine("Declare @emple INT =	0                                      ");
            query.AppendLine("Declare @iCodCatestatusInicializado int =0                   ");
            query.AppendLine("Declare @iCodcatEstatusCancelado int =	0                  ");
            query.AppendLine("Declare @iCodCatEstatusPendienteReactivacion int =0          ");
            query.AppendLine("Declare @iCodCatEstatusBloqueoReactivado int = 0             ");
            query.AppendLine("       ");
            query.AppendLine("       ");
            query.AppendLine("select @iCodCatEstatusPendienteReactivacion	=	iCodCatalogo									");
            query.AppendLine("		From ['+@esquema+'].[VisHistoricos(''EstatusBloqueo'',''Estatus Bloqueos'',''Español'')]  ");
            query.AppendLine("	Where dtinivigencia <> dtfinvigencia                                                          ");
            query.AppendLine("	and dtfinvigencia >= getdate()                                                                ");
            query.AppendLine("	and vchCodigo = ''5PendienteReactivación ''                                                   ");
            query.AppendLine("         ");
            query.AppendLine("Select @iCodCatEstatusBloqueoReactivado = icodcatalogo                                          ");
            query.AppendLine("	From ['+@esquema+'].[VisHistoricos(''EstatusBloqueo'',''Estatus Bloqueos'',''Español'')]      ");
            query.AppendLine("Where dtinivigencia <> dtfinvigencia                                                            ");
            query.AppendLine("and dtfinvigencia >= getdate()                                                                  ");
            query.AppendLine("and vchCodigo =''7Reactivado ''                                                                 ");
            query.AppendLine("          ");
            query.AppendLine("          ");
            query.AppendLine("SELECT @emple = MAX(iCodCatalogo)                                                               ");
            query.AppendLine("FROM ['+@esquema+'].[VisHistoricos(''Emple'',''Empleados'',''Español'')]                        ");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia                                                            ");
            query.AppendLine("AND dtFinVigencia >= GETDATE()                                                                   ");
            query.AppendLine("AND Usuar ='+convert(varchar, @usuar)+'                                                          ");
            query.AppendLine("              ");
            query.AppendLine("SELECT @iCodCatEstatusinicializado	=	iCodCatalogo                                           ");
            query.AppendLine("	FROM Keytia5.['+@esquema+'].[VisHistoricos(''EstatusABCsEnPBX'',''Estatus ABCs En PBX'',''Español'')]		");
            query.AppendLine("	WHERE dtIniVigencia <> dtFinVigencia                                                                         ");
            query.AppendLine("		AND dtFinVigencia >= GETDATE()                                                                           ");
            query.AppendLine("		AND vchDescripcion = ''Inicializado''                                                                    ");
            query.AppendLine("           ");
            query.AppendLine("Select @iCodcatEstatusCancelado = icodcatalogo                                                                 ");
            query.AppendLine("FROM Keytia5.['+@esquema+'].[VisHistoricos(''EstatusABCsEnPBX'',''Estatus ABCs En PBX'',''Español'')]          ");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia              ");
            query.AppendLine("AND dtFinVigencia >= GETDATE()                    ");
            query.AppendLine("AND vchCodigo = ''Cancelado''						");
            query.AppendLine("  ");
            query.AppendLine("if(OBJECT_ID(''TEMPDB..#CodigosTemp'') is not null) ");
            query.AppendLine("begin                                               ");
            query.AppendLine("	drop table #CodigosTemp                           ");
            query.AppendLine("end ");
            query.AppendLine("    ");
            query.AppendLine("Create table #CodigosTemp                           ");
            query.AppendLine("(                                                   ");
            query.AppendLine("	id int primary key identity not null,             ");
            query.AppendLine("	codigo varchar(20)                                ");
            query.AppendLine(")    ");
            query.AppendLine("     ");
            query.AppendLine("insert into #CodigosTemp                            ");
            query.AppendLine("(codigo)                                            ");
            query.AppendLine("SELECT codigo                                        ");
            query.AppendLine("FROM Keytia5.['+@esquema+'].BitacoraCodigosABCsEnPBX ");
            query.AppendLine("where iCodCatProcesoEnPBX = 283101                   ");
            query.AppendLine("and iCodCatEstatusEnPBX = @iCodCatestatusInicializado");
            query.AppendLine("and iCodCatSolicitud = 1");
            query.AppendLine("and iCodCatEmple = @emple                            ");
            query.AppendLine("                                                     ");
            query.AppendLine("                                                     ");
            query.AppendLine("Select @numReg = count(*)                            ");
            query.AppendLine("FRom #codigosTemp                                    ");
            query.AppendLine("   ");
            query.AppendLine("   ");
            query.AppendLine("While(@contador <= @numreg)                          ");
            query.AppendLine("begin                                                ");
            query.AppendLine(" Select @codigoActual	=	codigo                     ");
            query.AppendLine(" From #CodigosTemp                                   ");
            query.AppendLine(" Where id = @contador                                ");
            query.AppendLine("  ");
            query.AppendLine(" Set @arregloCodigosInicializados	= @arregloCodigosInicializados +''''''''+@codigoActual+''''''''		");
            query.AppendLine("              ");
            query.AppendLine(" if(@contador < @numReg)                                                                              ");
            query.AppendLine(" begin                  ");
            query.AppendLine(" Set @arregloCodigosInicializados = @arregloCodigosInicializados +'',''                               ");
            query.AppendLine(" end                                                                                                  ");
            query.AppendLine("               ");
            query.AppendLine("Set @contador = @contador + 1                                                                         ");
            query.AppendLine("end              ");
            query.AppendLine("              ");
            query.AppendLine("'");
            query.AppendLine("Set @query = @query +' ");
            query.AppendLine("if(@arregloCodigosInicializados is not null and @arregloCodigosInicializados <> '''')                 ");
            query.AppendLine("begin                                                                                                 ");
            query.AppendLine("                                                                                                      ");
            query.AppendLine("Set @querySecundario = ''                                                                             ");
            query.AppendLine("                                                                                                      ");
            query.AppendLine("print (''''bitacoraABCsEnPBX'''')                                                                     ");
            query.AppendLine("	Update Keytia5.['+@esquema+'].BitacoraCodigosABCsEnPBX                                              ");
            query.AppendLine("		Set iCodCatEstatusEnPBX		= ''+convert(varchar,@iCodcatEstatusCancelado)+'',                  ");
            query.AppendLine("			dtFecUltAct			= GETDATE()	                                                            ");
            query.AppendLine("	Where dtIniVigencia <> dtFinVigencia                                                                ");
            query.AppendLine("	and dtfinVigencia >= GETDATE()                                                                      ");
            query.AppendLine("	and codigo in (''+@arregloCodigosInicializados+'')                                                  ");
            query.AppendLine(" ");
            query.AppendLine("	print (''''codigosBloqueo'''')                                                                      ");
            query.AppendLine("	Update ['+@esquema+'].[VisHistoricos(''''CodigoBloqueo'''',''''CodigosBloqueo'''',''''Español'''')] ");
            query.AppendLine("		Set		EstatusBloqueo = ''+convert(varchar,@iCodCatEstatusBloqueoReactivado)+'',               ");
            query.AppendLine("				dtFinVigencia = GETDATE(),                                                              ");
            query.AppendLine("              dtFecUltAct			= GETDATE()");
            query.AppendLine("	where dtinivigencia <> dtfinvigencia                                                                ");
            query.AppendLine("	and dtfinvigencia >= getdate()                                                                      ");
            query.AppendLine("	and CodAutocod  in(''+@arregloCodigosInicializados+'')");
            query.AppendLine("");
            query.AppendLine("	'' ");
            query.AppendLine("     ");
            query.AppendLine("	execute (@querySecundario) ");
            query.AppendLine("	 ");
            query.AppendLine("	 ");
            query.AppendLine("   ");
            query.AppendLine("end   ");
            query.AppendLine("      ");
            query.AppendLine("print ''reactivado''         ");
            query.AppendLine("Set @querySecundario = ''    ");
            query.AppendLine("Update ['+@esquema+'].[VisHistoricos(''''CodigoBloqueo'''',''''CodigosBloqueo'''',''''Español'''')]   ");
            query.AppendLine("	Set estatusBloqueo = ''+convert(varchar,@iCodCatEstatusPendienteReactivacion)+'',                    ");
            query.AppendLine("      dtFecUltAct			= GETDATE()");
            query.AppendLine("where dtIniVigencia <> dtFinVigencia                                                                  ");
            query.AppendLine("and dtFinVigencia >= getdate()                                                                        ");
            query.AppendLine("and emple=''+convert(varchar,@emple)+''                                                               ");
            query.AppendLine("''                                                                                                    ");
            query.AppendLine("                                                                                                      ");
            query.AppendLine("if(@arregloCodigosInicializados <> '''' and @arregloCodigosInicializados is not null)                 ");
            query.AppendLine("begin                                                                                                 ");
            query.AppendLine("	Set @querySecundario = @querySecundario+''and codAutocod not in (''+@arregloCodigosInicializados+'')''			");
            query.AppendLine("end                         ");
            query.AppendLine("                            ");
            query.AppendLine("execute (@querySecundario)  ");
            query.AppendLine("   ");
            query.AppendLine("'  ");
            query.AppendLine("   ");
            query.AppendLine("--print @query              ");
            query.AppendLine("execute (@query)            ");
            return query.ToString();
        }

        public bool EsReactivacion()
        {
            DataTable dtLlamadasSinEtiquetar = DSODataAccess.Execute(ConsultaLlamadasPorEtiquetar());
            if (dtLlamadasSinEtiquetar != null && dtLlamadasSinEtiquetar.Rows.Count > 0)
            {
                return false;
            }
            else { return true; }           
        }

        public string ConsultaLlamadasPorEtiquetar()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine("Declare @query			nvarchar(4000)=''                   ");
            query.AppendLine("Declare @esquema varchar(100)='" + DSODataContext.Schema + "' ");
            query.AppendLine("Declare @maestro		varchar(100)                            ");
            query.AppendLine("Declare @filler			varchar(100)                        ");
            query.AppendLine("Declare @arregloFillers varchar(max)                          ");
            query.AppendLine("                                                              ");
            query.AppendLine("Declare @Contador 	int = 1                                 ");
            query.AppendLine("Declare @NumRegs	int = 0                                     ");
            query.AppendLine("                                                              ");
            query.AppendLine("Declare @emple INT = 0                                        ");
            query.AppendLine("Declare @sitioEmple int =0                                    ");
            query.AppendLine("Declare @empleUbica varchar(100)                              ");
            query.AppendLine("Declare @icodCatalogoGrupoBloqueo int=0                       ");
            query.AppendLine("                                                              ");
            query.AppendLine("Declare @fechaInicioCDR DateTime                              ");
            query.AppendLine("Declare @fechaFinCDR Datetime                                 ");
            query.AppendLine("DECLARE @llamsEntrada INT = 0    ");
            query.AppendLine("DECLARE @llamsEnlace INT = 0     ");
            query.AppendLine("                                 ");
            query.AppendLine("SET @query = '                   ");
            query.AppendLine("SELECT @llamsEntradaOUT =  MAX(iCodCatalogo) ");
            query.AppendLine("	FROM ['+@esquema+'].[VisHistoricos(''TDest'',''Tipo de Destino'',''Español'')]   ");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia    ");
            query.AppendLine("AND dtFinVigencia >= GETDATE()          ");
            query.AppendLine("AND vchCodigo = ''Ent''                 ");
            query.AppendLine("'                                       ");
            query.AppendLine("                                        ");
            query.AppendLine("Execute sp_executesql	@query,           ");
            query.AppendLine("						N'@llamsEntradaOUT int output',					");
            query.AppendLine("						@llamsEntradaOUT = @llamsEntrada output         ");
            query.AppendLine("						                                                ");
            query.AppendLine("						                                                ");
            query.AppendLine("SET @query ='                                                         ");
            query.AppendLine("SELECT  @llamsEnlaceOUT	=	MAX(iCodCatalogo)                       ");
            query.AppendLine("	FROM ['+@esquema+'].[VisHistoricos(''TDest'',''Tipo de Destino'',''Español'')]  		");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia 														");
            query.AppendLine("AND dtFinVigencia >= GETDATE()                                                           ");
            query.AppendLine("AND vchCodigo = ''Enl''                                                                  ");
            query.AppendLine("'			                                                                               ");
            query.AppendLine("                                                                                         ");
            query.AppendLine("Execute sp_executesql	@query,                                                            ");
            query.AppendLine("						N'@llamsEnlaceOUT int output',                                     ");
            query.AppendLine("						@llamsEnlaceOUT = @llamsEnlace output	                           ");
            query.AppendLine("                                                              ");
            query.AppendLine("if(OBJECT_ID('Tempdb..#MaestrosSitios') is not null)          ");
            query.AppendLine("begin                                                         ");
            query.AppendLine("	drop table #MaestrosSitios                                  ");
            query.AppendLine("end                                                           ");
            query.AppendLine("                                                              ");
            query.AppendLine("create table #MaestrosSitios                                  ");
            query.AppendLine("(                                                             ");
            query.AppendLine("	id int primary key identity not null,                       ");
            query.AppendLine("	maestro varchar(100)                                        ");
            query.AppendLine(")                                                             ");
            query.AppendLine("                                                              ");
            query.AppendLine("                                                              ");
            query.AppendLine("if(OBJECT_ID('Tempdb..#FillersSitios') is not null)           ");
            query.AppendLine("begin                                                         ");
            query.AppendLine("	drop table #FillersSitios                                   ");
            query.AppendLine("end                                                           ");
            query.AppendLine("                                                              ");
            query.AppendLine("create table #FillersSitios                                   ");
            query.AppendLine("(                                                             ");
            query.AppendLine("	id int primary key identity not null,                       ");
            query.AppendLine("	filler varchar(100),                                        ");
            query.AppendLine("	iCodCatalogoSitio int                                       ");
            query.AppendLine(")   ");
            query.AppendLine(" ");
            query.AppendLine("SET @query = '");
            query.AppendLine("SELECT @empleOUT =MAX(iCodCatalogo) FROM ['+@esquema+'].[VisHistoricos(''Emple'',''Empleados'',''Español'')]    ");
            query.AppendLine("					    WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND Usuar = " + Session["iCodUsuario"].ToString() + "  ");
            query.AppendLine("'                                                                                                               ");
            query.AppendLine("Execute sp_executesql	@query,                                                                                   ");
            query.AppendLine("						N'@empleOUT int output',                                                                  ");
            query.AppendLine("						@empleOUT = @emple output                                                                 ");
            query.AppendLine("						                                                                                          ");
            query.AppendLine("Set @query='                                                                                                    ");
            query.AppendLine("insert into #MaestrosSitios                                                                                     ");
            query.AppendLine("select distinct(vchDesMaestro)                                                                                  ");
            query.AppendLine("from ['+@esquema+'].[vishistoricos(''sitioProcesoBloqueo'',''Sitios Proceso Bloqueo'',''Español'')] sitiosBloqueo ");
            query.AppendLine("                                                                                 ");
            query.AppendLine("INNER JOIN ['+@esquema+'].[vishisComun(''Sitio'',''Español'')] vComunSitios      ");
            query.AppendLine("	ON vComunSitios.iCodCatalogo = sitiosBloqueo.sitio                             ");
            query.AppendLine("	and vComunSitios.dtinivigencia <> vcomunSitios.dtfinvigencia                   ");
            query.AppendLine("	and vComunSitios.dtFinVigencia >= getdate()                                    ");
            query.AppendLine("                                                                                 ");
            query.AppendLine("where sitiosBloqueo.dtinivigencia <> sitiosBloqueo.dtfinvigencia                 ");
            query.AppendLine("and sitiosBloqueo.dtfinvigencia >= getdate()'                                    ");
            query.AppendLine("                                                                                 ");
            query.AppendLine("                                                                                 ");
            query.AppendLine("Execute sp_executesql @query                                                     ");
            query.AppendLine("                                                                                 ");
            query.AppendLine("Select @numRegs=Count(*)                                                         ");
            query.AppendLine("from #MaestrosSitios                                                             ");
            query.AppendLine("                                                                                 ");
            query.AppendLine("while (@contador <= @numRegs)                                                    ");
            query.AppendLine("begin                                                                            ");
            query.AppendLine("                                                                                 ");
            query.AppendLine("Select @maestro=maestro                                                          ");
            query.AppendLine("	From #MaestrosSitios                                                           ");
            query.AppendLine("Where id=@contador                                                               ");
            query.AppendLine("                                                                                 ");
            query.AppendLine("	Set @query='                                                                   ");
            query.AppendLine("				Select Top (1)  Filler,iCodCatalogo                                                    ");
            query.AppendLine("					From ['+@esquema+'].[VisHistoricos(''Sitio'','''+@maestro+''',''Español'')]        ");
            query.AppendLine("				where dtInivigencia <> dtFinVigencia                                                   ");
            query.AppendLine("				and dtFinVigencia >= getdate()'                                                        ");
            query.AppendLine("                                                                                                     ");
            query.AppendLine("	insert into #FillersSitios                                                                         ");
            query.AppendLine("	(filler,iCodCatalogoSitio)                                                                         ");
            query.AppendLine("	Execute (@query)                                                                                   ");
            query.AppendLine("                                                                                                     ");
            query.AppendLine("                                                                                                     ");
            query.AppendLine("	Set @contador=@contador+1                                                                          ");
            query.AppendLine("                                                                                                     ");
            query.AppendLine("end--termina el while que Llenara el arreglo de los fillers                                          ");
            query.AppendLine("                                                                                                     ");
            query.AppendLine("Set @numRegs=0                                                                                       ");
            query.AppendLine("                                                                                                     ");
            query.AppendLine("SET @query ='                                                                                        ");
            query.AppendLine("Select @empleUbicaOUT= Ubica                                                                         ");
            query.AppendLine("from ['+@esquema+'].[VisHistoricos(''emple'',''empleados'',''español'')]                             ");
            query.AppendLine("where dtinivigencia <> dtfinvigencia                                                                 ");
            query.AppendLine("and dtfinvigencia >= getdate()                                                                       ");
            query.AppendLine("and icodcatalogo = '+Convert(varchar,@emple)+''                                                      ");
            query.AppendLine("                                                                                                     ");
            query.AppendLine("                                                                                                     ");
            query.AppendLine("execute sp_executesql	@query,                                                                        ");
            query.AppendLine("						N'@empleUbicaOUT varchar(100) output',                                         ");
            query.AppendLine("						@empleUbicaOUT	= @empleUbica output                                           ");
            query.AppendLine("                                                                                                     ");
            query.AppendLine("                                                                                                     ");
            query.AppendLine("                                                                                                     ");
            query.AppendLine("SET @query ='                                                                                        ");
            query.AppendLine("Select @sitioEmpleOUT = iCodCatalogoSitio                                                            ");
            query.AppendLine("From #FillersSitios                                                                                  ");
            query.AppendLine("where Filler = '''+@empleUbica+''' '                                                                 ");
            query.AppendLine("                                                                                                     ");
            query.AppendLine("                                                                                                     ");
            query.AppendLine("execute sp_executesql	@query,                                                                        ");
            query.AppendLine("						N'@sitioEmpleOUT int output',                                                  ");
            query.AppendLine("						@sitioEmpleOUT	=	@sitioEmple output                                         ");
            query.AppendLine("                                                                                                     ");
            query.AppendLine("						                                                                               ");
            query.AppendLine("						                                                                               ");
            query.AppendLine("                                                                                                     ");
            query.AppendLine("SET @query ='                                                                                        ");
            query.AppendLine(" select @iCodCatalogoGrupoBloqueoOUT = GrupoBloqueo                                                  ");
            query.AppendLine(" from ['+@esquema+'].[VisHistoricos(''SitioProcesoBloqueo'',''Sitios Proceso Bloqueo'',''Español'')] ");
            query.AppendLine(" where dtinivigencia <> dtfinvigencia												                   ");
            query.AppendLine(" and dtfinvigencia >= getdate()                                                                      ");
            query.AppendLine(" and sitio = '+Convert(varchar,@sitioEmple)+''                                                       ");
            query.AppendLine("                                                                                                     ");
            query.AppendLine(" execute sp_executesql	@query,                                                                    ");
            query.AppendLine("						N'@iCodCatalogoGrupoBloqueoOUT int output',                                    ");
            query.AppendLine("						@iCodCatalogoGrupoBloqueoOUT	=	@iCodCatalogoGrupoBloqueo output           ");
            query.AppendLine("                                                                                                     ");
            query.AppendLine("                                                                                                     ");
            query.AppendLine(" SET @query ='                                                                                       ");
            query.AppendLine(" Select distinct(teldest)                                                                                      ");
            query.AppendLine(" From ['+@esquema+'].[VisDetallados(''Detall'',''DetalleCDR'',''Español'')] detall                   ");
            query.AppendLine(" ");
            query.AppendLine(" INNER JOIN  ['+@esquema+'].[VisHistoricos(''PeriodoBloqueo'',''Periodos de Bloqueo'',''Español'')] pBloqueo      ");
            query.AppendLine("  ON  dtinivigencia <> dtfinvigencia                                                         ");
            query.AppendLine(" and dtfinvigencia >= getdate()                                                              ");
            query.AppendLine(" and grupoBloqueo = '+Convert(varchar,@icodcatalogoGrupoBloqueo) +'                          ");
            query.AppendLine("                                                                                             ");
            query.AppendLine(" where Detall.fechaInicio >= CONVERT(VARCHAR(11), pBloqueo.FechaInicio, 120)+''00:00:00''    ");
            query.AppendLine(" and Detall.fechaInicio <= CONVERT(VARCHAR(11), pBloqueo.FechaFin, 120)+''23:59:59''         ");
            query.AppendLine(" and Detall.emple = '+Convert(varchar,@emple)+'                                              ");
            query.AppendLine(" and     detall.TDest <> '+Convert(varchar,@llamsEntrada)+'");
            query.AppendLine(" and     detall.TDest <> '+Convert(varchar,@llamsEnlace)+'");
            query.AppendLine(" and (etiqueta ='''' or etiqueta is null or gEtiqueta ='''' or gEtiqueta is null	)'	           ");
            query.AppendLine("                                                                                             ");
            query.AppendLine(" Execute sp_executesql @query                                                                ");

            return query.ToString();
        }

        private int ProcesarGridResumenNums()
        {
            listaEtiq.Clear();
            int gpoNoIdentificado = Convert.ToInt32(DSODataAccess.ExecuteScalar("SELECT GEtiqueta FROM [VisHistoricos('GpoEtiqueta','Grupo Etiquetacion','Español')] WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = '0NoIdent'"));
            string mensajeError = string.Empty;

            //NZ: Obtenemos todos los datos del GridView.
            foreach (GridViewRow row in gridResumenNums.Rows)
            {
                EtiquetacionModelView objEtiq = new EtiquetacionModelView();
                objEtiq.Numero = (row.FindControl("btnLinkNumero") as LinkButton).Text;
                objEtiq.Grupo = Convert.ToInt32((row.FindControl("ddlGrupoEtiqueta") as DropDownList).SelectedValue);
                objEtiq.Etiqueta = (row.FindControl("txtEtiqueta") as TextBox).Text.Replace("'", "").Trim();

                if (objEtiq.Grupo == gpoNoIdentificado && !string.IsNullOrEmpty(objEtiq.Etiqueta))
                {
                    mensajeError = "No se puede introducir una etiqueta para el Grupo 'No Identificada'.";
                    break;
                }
                else if (objEtiq.Grupo != gpoNoIdentificado && string.IsNullOrEmpty(objEtiq.Etiqueta))
                {
                    mensajeError = "No se puede dejar una etiqueta en blanco para un grupo diferente de 'No Identificada'.";
                    break;
                }
                else if (objEtiq.Grupo != gpoNoIdentificado && !string.IsNullOrEmpty(objEtiq.Etiqueta))
                {
                    listaEtiq.Add(objEtiq);
                }
            }

            if (!string.IsNullOrEmpty(mensajeError))
            {
                lblTituloModalMsn.Text = "Error en la etiquetación de números";
                lblBodyModalMsn.Text = mensajeError;
                mpeEtqMsn.Show();
                return 0;
            }
            else
            {
                EtiquetarNumeros(gpoNoIdentificado);
                return listaEtiq.Count;                
            }
        }

        public void EtiquetarNumeros(int gpoNoIdentificado)
        {
            if (listaEtiq.Count > 0)
            {
                query.Length = 0;
                query.AppendLine("SELECT MAX(iCodCatalogo) FROM " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')]");
                query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND Usuar = " + Session["iCodUsuario"].ToString());
                var iCodEmple = DSODataAccess.ExecuteScalar(query.ToString()).ToString();

                string exec = "EXEC ProcesoEtiquetacionLight @esquema='{0}', @numMarcado='{1}', @etiqueta='{2}', @gpoEtiqueta={3}, @Emple={4}";

                //Solo se actualizan los registros en donde el grupo etiqueta sea diferente de 0 y la etiqueta sea diferente de vacio.
                foreach (EtiquetacionModelView item in listaEtiq.Where(x => x.Grupo != gpoNoIdentificado && !string.IsNullOrEmpty(x.Etiqueta)))
                {
                    DSODataAccess.ExecuteNonQuery(string.Format(exec, DSODataContext.Schema, item.Numero, item.Etiqueta, item.Grupo, iCodEmple));
                }
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            if (Session["iCodPerfil"].ToString() == "370")
            {
                HttpContext.Current.Response.Redirect(Session["HomePage"].ToString());
            }
            else
            {
                HttpContext.Current.Response.Redirect("~/UserInterface/DashboardFC/Dashboard.aspx?MiConsumo=1&Opc=opcMiConsumo");
            }
        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            if (Session["iCodPerfil"].ToString() == "370")
            {
                HttpContext.Current.Response.Redirect(Session["HomePage"].ToString());
            }
            else
            {
                HttpContext.Current.Response.Redirect("~/UserInterface/DashboardFC/Dashboard.aspx?MiConsumo=1&Opc=opcMiConsumo");
            }
        }

        protected void btnMisNumEtiquetados_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Redirect("~/UserInterface/Historicos/Etiquetacion/CambioEtiqueta.aspx");
        }

        private void CrearBotonMiEtiquetacion()
        {
            HtmlButton pbtnEtiquetacion = new HtmlButton();
            pbtnEtiquetacion.ID = "btnMisNumEtiquetados";
            pbtnEtiquetacion.Attributes["class"] = "buttonEdit";
            pbtnEtiquetacion.Style["display"] = "none";
            pbtnEtiquetacion.ServerClick += new EventHandler(btnMisNumEtiquetados_Click);
            pbtnEtiquetacion.InnerText = "Mis números etiquetados";
            pToolBar.Controls.Add(pbtnEtiquetacion);
        }

        #endregion

        #region Logica Detalle

        private string ConsultaDetalleLlams(string telDest)
        {
            query.Length = 0;
            query.AppendLine("DECLARE @gpoEtiqNoIdent INT = (SELECT GEtiqueta FROM " + DSODataContext.Schema + ".[VisHistoricos('GpoEtiqueta','Grupo Etiquetacion','Español')]");
            query.AppendLine("								 WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()	AND vchCodigo = '0NoIdent' )");
            query.AppendLine("");
            query.AppendLine("DECLARE @emple INT = (SELECT MAX(iCodCatalogo) FROM " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','Español')]");
            query.AppendLine("					    WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND Usuar = " + Session["iCodUsuario"].ToString() + ")");
            query.AppendLine("");
            query.AppendLine("DECLARE @llamsEntrada INT = (SELECT MAX(iCodCatalogo) FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]");
            query.AppendLine("							   WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = 'Ent')");
            query.AppendLine("");
            query.AppendLine("DECLARE @llamsEnlace INT = (SELECT MAX(iCodCatalogo) FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]");
            query.AppendLine("							  WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() AND vchCodigo = 'Enl')");
            query.AppendLine("");
            query.AppendLine("SELECT ");
            query.AppendLine("       [Fecha]				= CONVERT(VARCHAR(10),FechaInicio,103) + ' ' + CONVERT(VARCHAR(8),FechaInicio,14)");
            query.AppendLine("      ,[Duracion]				= DuracionMin");
            query.AppendLine("      ,[Costo]				= '$ ' + CONVERT(VARCHAR, CONVERT(MONEY,ROUND((Costo + CostoSM),2)), 1)");
            query.AppendLine("      ,[Extension]            = Extension");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisDetallados('Detall','DetalleCDR','Español')]");
            query.AppendLine("WHERE GEtiqueta = @gpoEtiqNoIdent");
            query.AppendLine("	AND Emple = @emple");
            query.AppendLine("	AND TDest <> @llamsEntrada");
            query.AppendLine("	AND TDest <> @llamsEnlace");
            query.AppendLine("	AND ( LEN(TelDest) = 3 OR LEN(TelDest) >= 7 )");
            query.AppendLine("  AND TelDest = '" + telDest + "'");
            query.AppendLine("ORDER BY FechaInicio DESC");
            return query.ToString();
        }

        private DataRow GetTotalesDetalleLlams(DataTable dtResultDetall)
        {
            int duracion = 0;
            double costo = 0;
            foreach (DataRow row in dtResultDetall.Rows)
            {
                duracion += Convert.ToInt32(row["Duracion"]);
                costo += Convert.ToDouble(row["Costo"].ToString().Replace("$", "").Trim());
            }

            DataRow newRow = dtResultDetall.NewRow();
            newRow["Duracion"] = duracion;
            newRow["Costo"] = costo.ToString();

            return newRow;
        }

        protected void btnLinkNumero_Click(object sender, EventArgs e)
        {
            LinkButton lnkView = sender as LinkButton;
            string numTelDest = lnkView.CommandArgument;

            GetDetallLlamsEmpleDeNumMarcado(numTelDest);
            lblTituloDetallLlams.Text = "DETALLE DE LLAMADAS DEL NÚMERO " + numTelDest;
            mpeEtqDetallLlams.Show();
        }

        protected void GetDetallLlamsEmpleDeNumMarcado(string telDest)
        {
            var dtResult = DSODataAccess.Execute(ConsultaDetalleLlams(telDest));
            if (dtResult.Rows.Count > 0)
            {
                gridDetallLlams.DataSource = dtResult;
                gridDetallLlams.DataBind();

                var totales = GetTotalesDetalleLlams(dtResult);
                gridDetallLlams.FooterRow.Controls[0].Controls.Add(new Label() { Text = "Total" });
                gridDetallLlams.FooterRow.Controls[1].Controls.Add(new Label() { Text = string.Format("{0:#,#}", Convert.ToInt32(totales["Duracion"].ToString())) });
                gridDetallLlams.FooterRow.Controls[2].Controls.Add(new Label() { Text = string.Format("$ {0:#,#.##}", Convert.ToDouble(totales["Costo"].ToString())) });
            }
        }

        #endregion

    }
}

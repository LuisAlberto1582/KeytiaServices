using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using KeytiaServiceBL;
using System.Net.Mail;
using System.Configuration;


namespace KeytiaWeb.UserInterface
{
    public partial class CodigosBloqueoEtiquetacion : System.Web.UI.Page
    {
        private DataTable dtCodigosEnProceso = new DataTable();

        #region Eventos
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                
               
                if (!Page.IsPostBack)
                {
                    BuscarCodigosEnProceso();
                    cargarElementosGrid("0000", "0000");
                    cargarElementosDDLs();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected void AplicaFiltros_Click(Object sender, EventArgs e)
        {
            try
            {
                string iCodCatSitio = ddlSitio.SelectedValue.ToString();
                string iCodCatTecnologia = ddlTecnoligia.SelectedValue.ToString();


                BuscarCodigosEnProceso();
                cargarElementosGrid(iCodCatSitio, iCodCatTecnologia);


                //CheckElementosGrid(chBoxSeleccionarTodos.Checked);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected void AplicaCambios_Click(Object sender, EventArgs e)
        {
            try
            {
                string iCodRegistroCodigo = ""; ;
                string iCodRegistroBitacora = "";


                foreach (GridViewRow row in grdCodigosEnProceso.Rows)
                {

                    bool isCheckedgrrEnProceso = (row.FindControl("chBxSeleccionar") as CheckBox).Checked;

                    if (isCheckedgrrEnProceso)
                    {
                        iCodRegistroCodigo = (row.FindControl("lbliCodRegistroCodigo") as Label).Text;
                        iCodRegistroBitacora = (row.FindControl("lbliCodRegistroBitacora") as Label).Text;


                        //Cambiar el estatus de la respuesta a uno si es cero si no no hacer nada en la bitacora de codigos PBX 
                        CambiarEstatusBitacoraPBX(iCodRegistroBitacora, iCodRegistroCodigo);

                        BuscarCodigosEnProceso();
                        cargarElementosGrid(ddlSitio.SelectedValue.ToString(), ddlTecnoligia.SelectedValue.ToString());
                        
                       
                    }
                }
                btnYes.Enabled = false;
                btnYes.CommandArgument = "";
                
            }
            catch (Exception ex)
            {

                throw ex;
            }



        }

        protected void ModificarResultado_Click(Object sender, EventArgs e)
        {
            try
            {
                string siCodRegistroCodigo = ""; ;
                string siCodRegistroBitacora = "";
                string siCodCatEstatusCodigo = "";

                foreach (GridViewRow row in grdCodigosEnProceso.Rows)
                {
                    bool isCheckedgrrEnProceso = (row.FindControl("chBxSeleccionar") as CheckBox).Checked;

                    if (isCheckedgrrEnProceso)
                    {
                        siCodRegistroCodigo = (row.FindControl("lbliCodRegistroCodigo") as Label).Text;
                        siCodRegistroBitacora = (row.FindControl("lbliCodRegistroBitacora") as Label).Text;
                        siCodCatEstatusCodigo = (row.FindControl("lbliCodCatEstatus") as Label).Text;

                        ModificarRespuesta(siCodRegistroBitacora, siCodRegistroCodigo, siCodCatEstatusCodigo);
                        BuscarCodigosEnProceso();
                        cargarElementosGrid(ddlSitio.SelectedValue.ToString(), ddlTecnoligia.SelectedValue.ToString());

                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected void CancelarCodigos_Click(Object sender, EventArgs e)
        {
            try
            {
                string iCodRegistroCodigo = ""; ;
                string iCodRegistroBitacora = "";

                foreach (GridViewRow row in grdCodigosEnProceso.Rows)
                {
                    bool isCheckedgrrEnProceso = (row.FindControl("chBxSeleccionar") as CheckBox).Checked;

                    if (isCheckedgrrEnProceso)
                    {
                        iCodRegistroCodigo = (row.FindControl("lbliCodRegistroCodigo") as Label).Text;
                        iCodRegistroBitacora = (row.FindControl("lbliCodRegistroBitacora") as Label).Text;

                        //Cambiar el estatus de la respuesta a uno si es cero si no no hacer nada en la bitacora de codigos PBX 
                        CancelarCodigo(iCodRegistroBitacora, iCodRegistroCodigo);
                        BuscarCodigosEnProceso();
                        cargarElementosGrid(ddlSitio.SelectedValue.ToString(), ddlTecnoligia.SelectedValue.ToString());
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected void MostrarAviso_Click(Object sender, EventArgs e)
        {
            try
            {
                lblTituloModalMsn.Text = "Mensaje";
                lblBodyModalMsn.Text = "La acciones que se realizaran  no podran ser canceladas.  ¿Desea continuar?";
                btnYes.CommandArgument = (sender as Button).CommandArgument.ToString().ToUpper();
                btnYes.Enabled = true;
                mpeEtqMsn.Show();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        protected void Modal_Aceptar_Click(Object sender, EventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                string command = btn.CommandArgument.ToString().ToUpper();

                if (command == "INSERTAR")
                {
                    AplicaCambios_Click(new object(), new EventArgs());

                }
                else if (command == "MODIFICAR")
                {
                    ModificarResultado_Click(new object(), new EventArgs());
                }
                else if (command == "CANCELAR")
                {
                    CancelarCodigos_Click(new object(), new EventArgs());
                }

                Page.Response.Redirect("~/UserInterface/Historicos/Etiquetacion/CodigosBloqueoEtiquetacion.aspx", false);

            }

            catch (Exception ex)
            {
                throw ex;
            }
        }


        protected void CheckChanged(Object sender, EventArgs e)
        {
            try
            {
                CheckElementosGrid(chBoxSeleccionarTodos.Checked);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Métodos
        private bool cargarElementosDDLs()
        {
            bool respuesta = false;
            #region ddLists
            try
            {
                DataTable dtFiltadoSitios = new DataTable();
                DataTable dtFiltradoTecnologias = new DataTable();

                DataView vdtCodigosEnProceso = new DataView(dtCodigosEnProceso);
                dtFiltadoSitios = vdtCodigosEnProceso.ToTable(true, "sitio", "sitioDesc");

                dtFiltradoTecnologias = vdtCodigosEnProceso.ToTable(true, "tecnologia", "tecnologiaDesc");
                ddlSitio.Width = 200;
                ddlTecnoligia.Width = 200;

                ListItem item = new ListItem();
                item.Text = "Todos los sitios ...";
                item.Value = "0000";

                ddlSitio.Items.Add(item);

                foreach (DataRow row in dtFiltadoSitios.Rows)
                {
                    item = new ListItem();

                    item.Text = row["sitioDesc"].ToString();
                    item.Value = row["sitio"].ToString();

                    ddlSitio.Items.Add(item);
                }

                item = new ListItem();
                item.Text = "Todas las Tecnologías ...";
                item.Value = "0000";

                ddlTecnoligia.Items.Add(item);

                foreach (DataRow row in dtFiltradoTecnologias.Rows)
                {
                    item = new ListItem();

                    item.Text = row["tecnologiaDesc"].ToString();
                    item.Value = row["tecnologia"].ToString();

                    ddlTecnoligia.Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            #endregion

            return respuesta;
        }

        private void BuscarCodigosEnProceso()
        {
            try
            {
                dtCodigosEnProceso = new DataTable();
                string consultaCodigosEnProceso = "";

                consultaCodigosEnProceso = CrearConsultaCodigosEnProceso();

                dtCodigosEnProceso = DSODataAccess.Execute(consultaCodigosEnProceso);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        private string CrearConsultaCodigosEnProceso()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("");

            try
            {
                query.AppendLine("Declare @iCodCatEstatusEnProcesoDeBloqueo int		");
                query.AppendLine("Declare @iCodCarEnProcesoDeReactivacion int       ");
                query.AppendLine("");
                query.AppendLine("Select @iCodCatEstatusEnProcesoDeBloqueo = iCodCatalogo		");
                query.AppendLine("From [" + DSODataContext.Schema + "].[VisHistoricos('EstatusBloqueo','Estatus Bloqueos','Español')]	");
                query.AppendLine("Where vchCodigo = '3EnProcesoDeBloqueo'		");
                query.AppendLine("");
                query.AppendLine("select @iCodCarEnProcesoDeReactivacion = iCodcatalogo	");
                query.AppendLine("From [" + DSODataContext.Schema + "].[VisHistoricos('EstatusBloqueo','Estatus Bloqueos','Español')]	");
                query.AppendLine("Where vchCodigo = '6EnProcesoDeReactivación'	");
                query.AppendLine("");
                query.AppendLine("");
                query.AppendLine("Select ");
                query.AppendLine("		[iCodRegistroCodigo]		=	Consulta.[iCodRegistrioCodigo], 		");
                query.AppendLine("		[iCodRegistroBitacora]		=	Consulta.[iCodRegistroBitacora],        ");
                query.AppendLine("		[seleccionar]				=	0,                                      ");
                query.AppendLine("		[codAuto]					=	Consulta.[codAuto],                     ");
                query.AppendLine("		[codAutoCod]				=	Consulta.[codAutoCod],                  ");
                query.AppendLine("		[sitio]                     =   Consulta.[sitio],                       ");
                query.AppendLine("		[sitioDesc]					=	Consulta.[sitioDesc],                   ");
                query.AppendLine("		[tecnologia]                =   Consulta.[tecnologia],                  ");
                query.AppendLine("		[tecnologiaDesc]	        =	Consulta.[tecnologiaDesc],              ");
                query.AppendLine("		[ip]						=	Consulta.[ip],                          ");
                query.AppendLine("		[comentario]				=	bCodigos.Comentario,                    ");
                query.AppendLine("		[estatusBloqueo]			=	Consulta.[estatusBloqueo],              ");
                query.AppendLine("		[estatusBloqueoDesc]	    =	Consulta.[estatusDesc]          ");
                query.AppendLine("");
                query.AppendLine("");
                query.AppendLine("From (		");
                query.AppendLine("		Select	[iCodRegistrioCodigo]  = cBloqueo.iCodRegistro,			");
                query.AppendLine("				[iCodRegistroBitacora] = Max(bCodigos.iCodregistro),    ");
                query.AppendLine("				[codAuto]              = cBloqueo.CodAuto,              ");
                query.AppendLine("				[codAutoCod]           = cBloqueo.CodAutoCod,           ");
                query.AppendLine("				[sitio]				   = vComun.iCodCatalogo,	        ");
                query.AppendLine("				[sitioDesc]	           = vComun.vchDescripcion,         ");
                query.AppendLine("				[tecnologia]           = vComun.marcaSitio,             ");
                query.AppendLine("				[tecnologiaDesc]       = vComun.marcaSitioDesc,         ");
                query.AppendLine("				[ip]                   = bCodigos.ip,                   ");
                query.AppendLine("				[estatusBloqueo]       = cBloqueo.EstatusBloqueo,       ");
                query.AppendLine("				[estatusDesc]          = Case When cBloqueo.EstatusBloqueo = @iCodCatEstatusEnProcesoDeBloqueo	then 'Pendiente Bloqueo'		");
                query.AppendLine("											When cBloqueo.EstatusBloqueo = @iCodCarEnProcesoDeReactivacion		then 'Pendiente Reactivacion' 	");
                query.AppendLine("											else '' end		");
                query.AppendLine("");
                query.AppendLine("		From [" + DSODataContext.Schema + "].[VisHistoricos('CodigoBloqueo','CodigosBloqueo','Español')] cBloqueo		");
                query.AppendLine("");
                query.AppendLine("		INNER JOIN [" + DSODataContext.Schema + "].BitacoraCodigosABCsEnPBX bCodigos		");
                query.AppendLine("			On cBloqueo.codAutoCod = bCodigos.Codigo			");
                query.AppendLine("          and (bCodigos.movimientoExitoso = 0 or bCodigos.movimientoExitoso is null)		");
                query.AppendLine("	        and bCodigos.dtIniVigencia <> bCodigos.dtFinVigencia	");
                query.AppendLine("	        and bCodigos.dtFinVigencia >= GETDATE()					");
                query.AppendLine("");
                query.AppendLine("		INNER JOIN [" + DSODataContext.Schema + "].[visHisComun('sitio','Español')] vComun	");
                query.AppendLine("				On vComun.iCodCatalogo = bCodigos.iCodCatSitio 	");
                query.AppendLine("");
                query.AppendLine("		Where	cBloqueo.EstatusBloqueo in (@iCodCatEstatusEnProcesoDeBloqueo, @iCodCarEnProcesoDeReactivacion)	");
                query.AppendLine("			and cBloqueo.dtinivigencia <> cBloqueo.dtFinVigencia	");
                query.AppendLine("			and cBloqueo.dtFinVigencia >= GETDATE()		");
                query.AppendLine("");
                query.AppendLine("			Group by	cBloqueo.iCodRegistro,         ");
                query.AppendLine("						cBloqueo.CodAuto,              ");
                query.AppendLine("						cBloqueo.CodAutoCod,           ");
                query.AppendLine("						vComun.iCodCatalogo,           ");
                query.AppendLine("						vComun.vchDescripcion,         ");
                query.AppendLine("						vComun.marcaSitio,             ");
                query.AppendLine("						vComun.marcaSitioDesc,         ");
                query.AppendLine("						bCodigos.ip,                   ");
                query.AppendLine("						cBloqueo.EstatusBloqueo        ");
                query.AppendLine("			) As Consulta                              ");
                query.AppendLine("");
                query.AppendLine("			INNER JOIN [" + DSODataContext.Schema + "].BitacoraCodigosABCsEnPBX bCodigos	");
                query.AppendLine("				On iCodRegistroBitacora = bCodigos.iCodRegistro	");

            }
            catch (Exception ex)
            {

                throw ex;
            }
            return query.ToString();
        }

        private void CheckElementosGrid(bool isChecked)
        {
            try
            {
                foreach (GridViewRow row in grdCodigosEnProceso.Rows)
                {
                    CheckBox chBox = new CheckBox();

                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        chBox = row.FindControl("chBxSeleccionar") as CheckBox;
                        chBox.Checked = isChecked;
                    }

                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        public void ModificarRespuesta(string sicodRegistroBitacora, string siCodRegistroCodigo, string siCodCatEstatusCodigo)
        {
            try
            {
                DSODataAccess.Execute(ConsultaActualizarRegistroManual(sicodRegistroBitacora, siCodRegistroCodigo, siCodCatEstatusCodigo));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string ConsultaActualizarRegistroManual(string sicodRegistroBitacora, string siCodRegistroCodigo, string siCodCatEstatusCodigo)
        {
            StringBuilder query = new StringBuilder();


            try
            {
                query.AppendLine("Declare @icodRegistroCodigo int = " + siCodRegistroCodigo + " --Registro en Codigos																        ");
                query.AppendLine("Declare @iCodCatEstatusCodigo int = " + siCodCatEstatusCodigo + "  --estatusActual en Codigos                                                                 ");
                query.AppendLine("Declare @iCodCatEstatusBloqueoEnProcesoDeBloqueo int =0 --iCodCatalogo Estatus En proceso de Bloqueo Codigos                          ");
                query.AppendLine("Declare @iCodCatEstatusBloqueoEnProcesoDeReactivacion int =0--iCodCatalogo Estatus En proceso de Reactivacion Codigos                 ");
                query.AppendLine("Declare @iCodCatEstatusBloqueoBloqueado int =0 --iCodCatalogo Estatus Bloqueado Codigos                                               ");
                query.AppendLine("Declare @iCodCatEstatusBloqueoReactivado int =0 --iCodCatalogo Estatus Reactivado Codigos                                             ");
                query.AppendLine("Declare @siguienteEstado int =0 --Siguiente Estatus del registro de la vista de codigos                                               ");
                query.AppendLine("                                                                                                                                      ");
                query.AppendLine("Declare @iCodRegistroBitacora int = " + sicodRegistroBitacora + " --Registro en Bitacora                                                                         ");
                query.AppendLine("Declare @iCodCatEstatusBloqueo int = 0 -- Estatus Actual en Bitacora                                                                  ");
                query.AppendLine("Declare @movimientoExitoso int =0 -- movimiento Exitoso en Bitacora                                                                   ");
                query.AppendLine("Declare @iCodCatEstatusBloqueoPBXProcesando int = 0 -- iCodCatalogo estatus Procesando en Bitacora                                    ");
                query.AppendLine("Declare @iCodCatEstatusBloqueoPBXProcesado int = 0 --icodCatalogo Estatus Procesado en Bitacora                                       ");
                query.AppendLine("Declare @iCodCatEstatusBloqueoPBXFinalizadoManualmente int =0  --iCodCatalogoestatus Finalizado Manualmente                           ");
                query.AppendLine("                                                                                                                                      ");
                query.AppendLine("                                                                                                                                      ");
                query.AppendLine("Begin --Region Selects                                                                                                                ");
                query.AppendLine("	Begin --Codigos                                                                                                                     ");
                query.AppendLine("		Select @iCodCatEstatusBloqueoEnProcesoDeBloqueo = iCodCatalogo                                                                  ");
                query.AppendLine("			From [" + DSODataContext.Schema + "].[VisHistoricos('EstatusBloqueo','Estatus Bloqueos','Español')]                                                  ");
                query.AppendLine("		Where vchCodigo = '3EnProcesoDeBloqueo'                                                                                         ");
                query.AppendLine("                                                                                                                                      ");
                query.AppendLine("                                                                                                                                      ");
                query.AppendLine("		Select @iCodCatEstatusBloqueoEnProcesoDeReactivacion = iCodCatalogo                                                             ");
                query.AppendLine("			From [" + DSODataContext.Schema + "].[VisHistoricos('EstatusBloqueo','Estatus Bloqueos','Español')]                                                  ");
                query.AppendLine("		Where vchCodigo = '6EnProcesoDeReactivación'                                                                                    ");
                query.AppendLine("                                                                                                                                      ");
                query.AppendLine("                                                                                                                                      ");
                query.AppendLine("		Select @iCodCatEstatusBloqueoBloqueado= iCodCatalogo                                                                            ");
                query.AppendLine("			From [" + DSODataContext.Schema + "].[VisHistoricos('EstatusBloqueo','Estatus Bloqueos','Español')]                                                  ");
                query.AppendLine("		Where vchCodigo = '4Bloqueado '		                                                                                            ");
                query.AppendLine("                                                                                                                                      ");
                query.AppendLine("		Select @iCodCatEstatusBloqueoReactivado = iCodCatalogo                                                                          ");
                query.AppendLine("			From [" + DSODataContext.Schema + "].[VisHistoricos('EstatusBloqueo','Estatus Bloqueos','Español')]                                                  ");
                query.AppendLine("		Where vchCodigo = '7Reactivado '                                                                                                ");
                query.AppendLine("                                                                                                                                      ");
                query.AppendLine("		Select @iCodCatEstatusCodigo = estatusBloqueo                                                                                   ");
                query.AppendLine("			From [" + DSODataContext.Schema + "].[VisHistoricos('CodigoBloqueo','CodigosBloqueo','Español')]                                                     ");
                query.AppendLine("		Where icodRegistro = @iCodRegistroCodigo                                                                                        ");
                query.AppendLine("                                                                                                                                      ");
                query.AppendLine("		Select @siguienteEstado = case	When @iCodCatEstatusCodigo = @iCodCatEstatusBloqueoEnProcesoDeBloqueo                           ");
                query.AppendLine("											Then  @iCodCatEstatusBloqueoBloqueado                                                       ");
                query.AppendLine("										When  @iCodCatEstatusCodigo = @iCodCatEstatusBloqueoEnProcesoDeReactivacion                     ");
                query.AppendLine("											Then  @iCodCatEstatusBloqueoReactivado                                                      ");
                query.AppendLine("										else 0 end                                                                                      ");
                query.AppendLine("	End                                                                                                                                 ");
                query.AppendLine("                                                                                                                                      ");
                query.AppendLine("	Begin --BitacoraPBX	                                                                                                                ");
                query.AppendLine("		Select @iCodCatEstatusBloqueoPBXProcesado = iCodCatalogo                                                                        ");
                query.AppendLine("			From [" + DSODataContext.Schema + "].[VisHistoricos('EstatusABCsEnPBX','Estatus ABCs En PBX','Español')]                                             ");
                query.AppendLine("		Where vchCodigo = 'ProcesadoEnPBX'                                                                                              ");
                query.AppendLine("                                                                                                                                      ");
                query.AppendLine("		Select	@iCodCatEstatusBloqueoPBXProcesando = iCodCatalogo                                                                      ");
                query.AppendLine("			From [" + DSODataContext.Schema + "].[VisHistoricos('EstatusABCsEnPBX','Estatus ABCs En PBX','Español')]                                             ");
                query.AppendLine("		Where vchCodigo = 'ProcesandoEnPBX'                                                                                             ");
                query.AppendLine("                                                                                                                                      ");
                query.AppendLine("		Select @iCodCatEstatusBloqueoPBXFinalizadoManualmente = iCodCatalogo                                                            ");
                query.AppendLine("			From [" + DSODataContext.Schema + "].[VisHistoricos('EstatusABCsEnPBX','Estatus ABCs En PBX','Español')]                                             ");
                query.AppendLine("		Where vchCodigo ='FinalizadoManualmente'                                                                                        ");
                query.AppendLine("                                                                                                                                      ");
                query.AppendLine("		Select @movimientoExitoso = movimientoExitoso, @iCodCatEstatusBloqueo = iCodCatEstatusEnPBX                                     ");
                query.AppendLine("			From [" + DSODataContext.Schema + "].BitacoraCodigosABCsEnPBX                                                                                        ");
                query.AppendLine("		Where iCodRegistro = @iCodRegistroBitacora                                                                                      ");
                query.AppendLine("	End                                                                                                                                 ");
                query.AppendLine("End                                                                                                                                   ");
                query.AppendLine("                                                                                                                                      ");
                query.AppendLine("                                                                                                                                      ");
                query.AppendLine("if                                                                                                                                    ");
                query.AppendLine("(                                                                                                                                     ");
                query.AppendLine("	(@movimientoExitoso is null or @movimientoExitoso =0) and                                                                           ");
                query.AppendLine("	--(@iCodCatEstatusBloqueo in(@iCodCatEstatusBloqueoPBXProcesando, @iCodCatEstatusBloqueoPBXProcesado )) and                         ");
                query.AppendLine("	(@siguienteEstado is not null and @siguienteEstado <> 0) and                                                                        ");
                query.AppendLine("	(@iCodCatEstatusCodigo in (@iCodCatEstatusBloqueoEnProcesoDeBloqueo,@iCodCatEstatusBloqueoEnProcesoDeReactivacion ))                ");
                query.AppendLine(")                                                                                                                                     ");
                query.AppendLine("Begin                                                                                                                                 ");
                query.AppendLine("                                                                                                                                      ");
                query.AppendLine("	Update [" + DSODataContext.Schema + "].BitacoraCodigosABCsEnPBX                                                                                              ");
                query.AppendLine("		Set                                                                                                                             ");
                query.AppendLine("			movimientoExitoso = 1,			                                                                                            ");
                query.AppendLine("			FechaAtencion	= GETDATE(),                                                                                                ");
                query.AppendLine("			FechaRespuesta = GETDATE(),                                                                                                 ");
                query.AppendLine("			Comentario	='Operacion Manual (Modificacion Resultado)(Web)',                                                                                      ");
                query.AppendLine("			iCodCatEstatusEnPBX = @iCodCatEstatusBloqueoPBXProcesado,                                                                   ");
                query.AppendLine("			dtFecUltact = GETDATE()                                                                                                     ");
                query.AppendLine("	Where iCodRegistro = @iCodregistroBitacora                                                                                          ");
                query.AppendLine("                                                                                                                                      ");
                query.AppendLine("End 																																	");

            }
            catch (Exception ex)
            {

                throw ex;
            }


            return query.ToString();
        }

        public void CancelarCodigo(string iCodRegistroBitacora, string iCodRegistroCodigo)
        {
            try
            {
                DSODataAccess.ExecuteNonQuery(CrearConsultaCancelarCodigos(iCodRegistroBitacora, iCodRegistroCodigo));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string CrearConsultaCancelarCodigos(string iCodRegistroBitacora, string iCodRegistroCodigo)
        {
            StringBuilder query = new StringBuilder();


            try
            {
                query.AppendLine(" Declare @iCodRegistroBitacora int = " + iCodRegistroBitacora + "								             ");
                query.AppendLine(" Declare @iCodRegistroCodigo int = " + iCodRegistroCodigo + "                                          ");
                query.AppendLine(" Declare @estatusBitacora int = 0                                                  ");
                query.AppendLine(" Declare @estatusBitacoraProcesando int = 0										 ");
                query.AppendLine(" Declare @estatusBitacoraProcesado int = 0                                         ");
                query.AppendLine(" Declare @estatusCodigo int = 0                                                    ");
                query.AppendLine(" Declare @estatusCodigoEnProcesoDeBloqueo int =0                                   ");
                query.AppendLine(" Declare @estatusCodigoEnProcesoDeReactivacion int =0                              ");
                query.AppendLine(" Declare @hacerUpdate bit = 0                                                      ");
                query.AppendLine(" Declare @estatusFinalizadoConError int =0                                         ");
                query.AppendLine(" 																				     ");
                query.AppendLine(" begin -- region Selects estatus                                                   ");
                query.AppendLine(" 																				     ");
                query.AppendLine(" --Busca el estatus del Codigo                                                     ");
                query.AppendLine("                                                                                   ");
                query.AppendLine(" Select @estatusCodigo = estatusBloqueo                                            ");
                query.AppendLine(" From [" + DSODataContext.Schema + "].[VisHistoricos('CodigoBloqueo','CodigosBloqueo','Español')]           ");
                query.AppendLine(" Where icodregistro = @iCodRegistroCodigo                                          ");
                query.AppendLine(" 																				     ");
                query.AppendLine(" 																				     ");
                query.AppendLine(" --Busca el estatus correspondiente a el estatus de EnProcesodeBloqueo             ");
                query.AppendLine(" Select @estatusCodigoEnProcesoDeBloqueo =  iCodCatalogo                           ");
                query.AppendLine(" from [" + DSODataContext.Schema + "].[VisHistoricos('EstatusBloqueo','Estatus Bloqueos','Español')]        ");
                query.AppendLine(" Where vchCodigo = '3EnProcesoDeBloqueo'                                           ");
                query.AppendLine(" 																				     ");
                query.AppendLine(" --Busca el estatus correspondiente a el estatus de EnProcesodeReactivacion        ");
                query.AppendLine(" Select @estatusCodigoEnProcesoDeReactivacion=  iCodCatalogo                       ");
                query.AppendLine(" from [" + DSODataContext.Schema + "].[VisHistoricos('EstatusBloqueo','Estatus Bloqueos','Español')]        ");
                query.AppendLine(" Where vchCodigo = '6EnProcesoDeReactivación'                                      ");
                query.AppendLine(" 			                                                                         ");
                query.AppendLine(" 																					 ");
                query.AppendLine(" --Busca moviemto Extoso y el estatus actual en la bitacora                        ");
                query.AppendLine(" Select                                                                            ");
                query.AppendLine(" 	@hacerUpdate = MovimientoExitoso,                                                ");
                query.AppendLine(" 	@estatusBitacora = iCodCatEstatusEnPBX                                           ");
                query.AppendLine(" From [" + DSODataContext.Schema + "].BitacoraCodigosABCsEnPBX                                              ");
                query.AppendLine(" Where iCodRegistro = @iCodRegistroBitacora                                        ");
                query.AppendLine(" 																					 ");
                query.AppendLine(" --Busca el catalogo del estatus Procesado                                         ");
                query.AppendLine(" Select @estatusBitacoraProcesando = iCodCatalogo                                  ");
                query.AppendLine(" from [" + DSODataContext.Schema + "].[VisHistoricos('EstatusABCsEnPBX','Estatus ABCs En PBX','Español')]   ");
                query.AppendLine(" Where vchCodigo = 'ProcesandoEnPBX'                                               ");
                query.AppendLine(" 																				     ");
                query.AppendLine(" --Busca el catalogo del estatus Procesando                                        ");
                query.AppendLine(" Select @estatusBitacoraProcesado = iCodCatalogo                                   ");
                query.AppendLine(" From [" + DSODataContext.Schema + "].[VisHistoricos('EstatusABCsEnPBX','Estatus ABCs En PBX','Español')]   ");
                query.AppendLine(" Where vchCodigo ='ProcesadoEnPBX'                        ");
                query.AppendLine("	Select @estatusFinalizadoConError = iCodCatalogo                                            ");
                query.AppendLine("	From [" + DSODataContext.Schema + "].[VisHistoricos('EstatusABCsEnPBX','Estatus ABCs En PBX','Español')]       ");
                query.AppendLine("	Where vchCodigo ='FinalizadoConErrorEnPBX'               ");
                query.AppendLine(" end ");
                query.AppendLine("     ");
                query.AppendLine("     ");
                query.AppendLine("     ");
                query.AppendLine(" if  ");
                query.AppendLine(" (   ");
                query.AppendLine(" 	(@hacerUpdate is not null and @hacerUpdate =0) and      ");
                query.AppendLine(" 	--(@estatusBitacora in (@estatusBitacoraProcesado, @estatusBitacoraProcesando) ) and 	");
                query.AppendLine(" 	(@estatusCodigo in ( @estatusCodigoEnProcesoDeBloqueo, @estatusCodigoEnProcesoDeReactivacion) )	");
                query.AppendLine(" )                                                                            ");
                query.AppendLine(" begin                                                                        ");
                query.AppendLine(" 	Update [" + DSODataContext.Schema + "].BitacoraCodigosABCsEnPBX                                        ");
                query.AppendLine(" 		Set dtFecUltAct = GETDATE(),                                            ");
                query.AppendLine("			iCodCatEstatusEnPBX = 	@estatusFinalizadoConError,	                ");
                query.AppendLine("          comentario = 'Operacion Manual (CANCELACION)(Web)',                     ");
                query.AppendLine(" 			dtFinVigencia= GETDATE()                                            ");
                query.AppendLine(" 	Where icodregistro = @iCodRegistroBitacora                                  ");
                query.AppendLine("                                                                              ");
                query.AppendLine(" 	Update [" + DSODataContext.Schema + "].[VisHistoricos('CodigoBloqueo','CodigosBloqueo','Español')]		");
                query.AppendLine(" 		Set dtFecUltAct = GETDATE(),            ");
                query.AppendLine(" 			dtFinVigencia = GETDATE()           ");
                query.AppendLine(" 	Where icodRegistro = @iCodRegistroCodigo	");
                query.AppendLine("                                              ");
                query.AppendLine(" end											");
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return query.ToString();
        }

        public void CambiarEstatusBitacoraPBX(string iCodRegistroBitacora, string iCodRegistroCodigo)
        {
            try
            {
                DSODataAccess.ExecuteNonQuery(ConsultaUpdateBitacoraPBX(iCodRegistroBitacora, iCodRegistroCodigo));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public string ConsultaUpdateBitacoraPBX(string iCodRegistroBitacora, string iCodRegistroCodigo)
        {
            StringBuilder query = new StringBuilder();

            query.Append("");
            try
            {
                query.AppendLine("Declare @iCodRegistroBitacora int = " + iCodRegistroBitacora + "                                                                                                                                     ");
                query.AppendLine("Declare @iCodRegistroCodigo int = " + iCodRegistroCodigo + "                                                                                                                                   ");
                query.AppendLine("Declare @estatusBitacora int = 0                                                                                                                                           ");
                query.AppendLine("Declare @estatusBitacoraProcesando int = 0			                                                                                                                     ");
                query.AppendLine("Declare @estatusBitacoraProcesado int = 0                                                                                                                                  ");
                query.AppendLine("Declare @estatusCodigo int = 0                                                                                                                                             ");
                query.AppendLine("Declare @estatusCodigoEnProcesoDeBloqueo int =0                                                                                                                            ");
                query.AppendLine("Declare @estatusCodigoEnProcesoDeReactivacion int =0                                                                                                                       ");
                query.AppendLine("Declare @hacerUpdate bit = 0                                                                                                                                               ");
                query.AppendLine("Declare @estatusFinalizadoConError int =0                                                                                                                                  ");
                query.AppendLine("                                                                                                                                                                           ");
                query.AppendLine("Declare @estatusCodigoPendienteBloqueo int =0                                                                                                                              ");
                query.AppendLine("Declare @estatusCodigoPendienteReactivacion int =0                                                                                                                         ");
                query.AppendLine("                                                                                                                                                                           ");
                query.AppendLine("--Buscar el estatus del codigo en la vista de codigos                                                                                                                      ");
                query.AppendLine("                                                                                                                                                                           ");
                query.AppendLine("begin -- region Selects estatus                                                                                                                                            ");
                query.AppendLine("                                                                                                                                                                           ");
                query.AppendLine("	--Busca el estatus del Codigo                                                                                                                                            ");
                query.AppendLine("	Select @estatusCodigo = estatusBloqueo                                                                                                                                   ");
                query.AppendLine("	From [" + DSODataContext.Schema + "].[VisHistoricos('CodigoBloqueo','CodigosBloqueo','Español')]                                                                                                  ");
                query.AppendLine("	Where icodregistro = @iCodRegistroCodigo                                                                                                                                 ");
                query.AppendLine("                                                                                                                                                                           ");
                query.AppendLine("                                                                                                                                                                           ");
                query.AppendLine("	--Busca el estatus correspondiente a el estatus de EnProcesodeBloqueo                                                                                                    ");
                query.AppendLine("	Select @estatusCodigoEnProcesoDeBloqueo =  iCodCatalogo                                                                                                                  ");
                query.AppendLine("	from [" + DSODataContext.Schema + "].[VisHistoricos('EstatusBloqueo','Estatus Bloqueos','Español')]                                                                                               ");
                query.AppendLine("	Where vchCodigo = '3EnProcesoDeBloqueo'                                                                                                                                  ");
                query.AppendLine("                                                                                                                                                                           ");
                query.AppendLine("	--Busca el estatus correspondiente a el estatus de EnProcesodeReactivacion                                                                                               ");
                query.AppendLine("	Select @estatusCodigoEnProcesoDeReactivacion=  iCodCatalogo                                                                                                              ");
                query.AppendLine("	from [" + DSODataContext.Schema + "].[VisHistoricos('EstatusBloqueo','Estatus Bloqueos','Español')]                                                                                               ");
                query.AppendLine("	Where vchCodigo = '6EnProcesoDeReactivación'                                                                                                                             ");
                query.AppendLine("                                                                                                                                                                           ");
                query.AppendLine("	--Busca el estatus correspondiente a el estatus de EnProcesodeBloqueo                                                                                                    ");
                query.AppendLine("	Select @estatusCodigoPendienteBloqueo =  iCodCatalogo                                                                                                                    ");
                query.AppendLine("	from [" + DSODataContext.Schema + "].[VisHistoricos('EstatusBloqueo','Estatus Bloqueos','Español')]                                                                                               ");
                query.AppendLine("	Where vchCodigo = '2PendienteBloqueo'                                                                                                                                    ");
                query.AppendLine("                                                                                                                                                                           ");
                query.AppendLine("	--Busca el estatus correspondiente a el estatus de EnProcesodeReactivacion                                                                                               ");
                query.AppendLine("	Select @estatusCodigoPendienteReactivacion=  iCodCatalogo                                                                                                                ");
                query.AppendLine("	from [" + DSODataContext.Schema + "].[VisHistoricos('EstatusBloqueo','Estatus Bloqueos','Español')]                                                                                               ");
                query.AppendLine("	Where vchCodigo = '5PendienteReactivación '                                                                                                                              ");
                query.AppendLine("                                                                                                                                                                           ");
                query.AppendLine("									                                                                                                                                         ");
                query.AppendLine("									                                                                                                                                         ");
                query.AppendLine("									                                                                                                                                         ");
                query.AppendLine("	--Busca moviemto Extoso y el estatus actual en la bitacora                                                                                                               ");
                query.AppendLine("	Select                                                                                                                                                                   ");
                query.AppendLine("		@hacerUpdate = MovimientoExitoso,                                                                                                                                    ");
                query.AppendLine("		@estatusBitacora = iCodCatEstatusEnPBX                                                                                                                               ");
                query.AppendLine("	From [" + DSODataContext.Schema + "].BitacoraCodigosABCsEnPBX                                                                                                                                     ");
                query.AppendLine("	Where iCodRegistro = @iCodRegistroBitacora                                                                                                                               ");
                query.AppendLine("	                                                                                                                                                                         ");
                query.AppendLine("	--Busca el catalogo del estatus Procesado                                                                                                                                ");
                query.AppendLine("	Select @estatusBitacoraProcesando = iCodCatalogo                                                                                                                         ");
                query.AppendLine("	from [" + DSODataContext.Schema + "].[VisHistoricos('EstatusABCsEnPBX','Estatus ABCs En PBX','Español')]                                                                                          ");
                query.AppendLine("	Where vchCodigo = 'ProcesandoEnPBX'                                                                                                                                      ");
                query.AppendLine("                                                                                                                                                                           ");
                query.AppendLine("	--Busca el catalogo del estatus Procesando                                                                                                                               ");
                query.AppendLine("	Select @estatusBitacoraProcesado = iCodCatalogo                                                                                                                          ");
                query.AppendLine("	From [" + DSODataContext.Schema + "].[VisHistoricos('EstatusABCsEnPBX','Estatus ABCs En PBX','Español')]                                                                                          ");
                query.AppendLine("	Where vchCodigo ='ProcesadoEnPBX'                                                                                                                                        ");
                query.AppendLine("                                                                                                                                                                           ");
                query.AppendLine("	Select @estatusFinalizadoConError = iCodCatalogo                                                                                                                         ");
                query.AppendLine("	From [" + DSODataContext.Schema + "].[VisHistoricos('EstatusABCsEnPBX','Estatus ABCs En PBX','Español')]                                                                                          ");
                query.AppendLine("	Where vchCodigo ='FinalizadoConErrorEnPBX'                                                                                                                               ");
                query.AppendLine("end                                                                                                                                                                        ");
                query.AppendLine("                                                                                                                                                                           ");
                query.AppendLine("	                                                                                                                                                                         ");
                query.AppendLine("--Solo entra si la respuesta que tiene el codigo en esta momento es negativa y si el codigo esta en Pendiente Bloqueo o Pendiente Reactivacion                             ");
                query.AppendLine("if	(                                                                                                                                                                    ");
                query.AppendLine("		(@hacerUpdate is not null and @hacerUpdate = 0 ) and                                                                                                                 ");
                query.AppendLine("		--(@estatusBitacora <> 0 and @estatusBitacora in (@estatusBitacoraProcesando ,@estatusBitacoraProcesado)) and                                                          ");
                query.AppendLine("		(@estatusCodigo <> 0 and @estatusCodigo in (@estatusCodigoEnProcesoDeBloqueo, @estatusCodigoEnProcesoDeReactivacion))                                                ");
                query.AppendLine("	)                                                                                                                                                                        ");
                query.AppendLine("begin                                                                                                                                                                      ");
                query.AppendLine("                                                                                                                                                                           ");
                query.AppendLine("  Update [" + DSODataContext.Schema + "].BitacoraCodigosABCsEnPBX                                                                                                                                   ");
                query.AppendLine("	    Set dtFinVigencia = GETDATE(),                                                                                                                                       ");
                query.AppendLine("			iCodCatEstatusEnPBX = 	@estatusFinalizadoConError,	                                                                                                             ");
                query.AppendLine("		    dtFecUltAct= GETDATE()			                                                                                                                                 ");
                query.AppendLine("  Where icodregistro = @iCodRegistroBitacora	                                                                                                                             ");
                query.AppendLine("                                                                                                                                                                           ");
                query.AppendLine("	Update [" + DSODataContext.Schema + "].[VisHistoricos('CodigoBloqueo','CodigosBloqueo','Español')]                                                                                                ");
                query.AppendLine("		Set		estatusBloqueo = case When @estatusCodigo = @estatusCodigoEnProcesoDeBloqueo                                                                                 ");
                query.AppendLine("									Then @estatusCodigoPendienteBloqueo                                                                                                      ");
                query.AppendLine("									When  @estatusCodigo = @estatusCodigoEnProcesoDeReactivacion	                                                                         ");
                query.AppendLine("									Then @estatusCodigoPendienteReactivacion		                                                                                         ");
                query.AppendLine("									end,		                                                                                                                             ");
                query.AppendLine("                  				dtFecUltAct = GETDATE()	                                                                                                                 ");
                query.AppendLine("	Where icodregistro =  @iCodRegistroCodigo                                                                                                                                ");
                query.AppendLine("                                                                                                                                                                           ");
                query.AppendLine("end																																										");



            }
            catch (Exception ex)
            {
                throw ex;
            }

            return query.ToString();
        }

        private bool cargarElementosGrid(string FiltroSitio, string FiltroTecnologia)
        {
            DataTable dtCodigosFiltrados = new DataTable();
            bool resultado = false;
            try
            {


                dtCodigosFiltrados = dtCodigosEnProceso;

                if (
                        FiltroSitio != null &&
                        FiltroSitio != "" &&
                        FiltroSitio != "0000"
                   )
                {
                    var dt = dtCodigosFiltrados.AsEnumerable().Where(r => r["sitio"].ToString() == FiltroSitio).ToList<DataRow>();
                    dtCodigosFiltrados = dt.AsEnumerable().Count() > 0 ? dt.CopyToDataTable() : new DataTable();
                }

                if (
                        FiltroTecnologia != null &&
                        FiltroTecnologia != "" &&
                        FiltroTecnologia != "0000"
                   )
                {
                    var dt = dtCodigosFiltrados.AsEnumerable().Where(r => r["tecnologia"].ToString() == FiltroTecnologia).ToList<DataRow>();
                    dtCodigosFiltrados = dt.AsEnumerable().Count() > 0 ? dt.CopyToDataTable() : new DataTable();
                }


                #region CargarGrid

                grdCodigosEnProceso.DataSource = dtCodigosFiltrados.Rows.Count > 0 ? dtCodigosFiltrados : null;
                grdCodigosEnProceso.DataBind();


                #endregion
            }
            catch (Exception ex)
            {

                throw ex;
            }

            return resultado;
        }


        #endregion

    }
}

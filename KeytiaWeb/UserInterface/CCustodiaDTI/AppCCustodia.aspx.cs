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
using System.Web.Services;
using KeytiaWeb.UserInterface.DashboardFC;

namespace KeytiaWeb.UserInterface.CCustodiaDTI
{
    public partial class AppCCustodia : System.Web.UI.Page
    {
        protected StringBuilder lsbQuery = new StringBuilder();
        //protected DataTable dtInventarioAsignado = new DataTable();
        protected DataTable dtExtensiones = new DataTable();
        protected DataTable dtCodAuto = new DataTable();
        protected DataTable dtSitios = new DataTable();
        protected DataTable dtTipoExten = new DataTable();
        protected DataTable dtPuestoEmple = new DataTable();
        protected DataTable dtJefeEmple = new DataTable();
        protected DataTable dtCenCosEmple = new DataTable();
        protected DataTable dtLocaliEmple = new DataTable();
        protected DataTable dtTipoEmple = new DataTable();
        protected DataTable dtEmpreEmple = new DataTable();
        protected DataTable dtCos = new DataTable();
        protected DataTable dtEmpreCenCos = new DataTable();
        protected DataTable dtLinea = new DataTable();
        protected DataTable dtCarrier = new DataTable();
        protected DataTable dtCtaMaestraLinea = new DataTable();
        protected DataTable dtRazonSocialLinea = new DataTable();
        protected DataTable dtTipoPlanLinea = new DataTable();
        protected DataTable dtTipoEquipoLinea = new DataTable();
        protected DataTable dtAreaLinea = new DataTable();
        protected DataTable dtPlanTarifarioLinea = new DataTable();
        protected DataTable dtValorBanderaLinea = new DataTable();
        protected DataTable dtDatosFCADirectores = new DataTable();
        protected DataTable dtDatosFCAPlantas = new DataTable();

        /*RZ.20130719 Se agrega instancia a clasde KDBAAccess*/
        protected KDBAccess pKDB = new KDBAccess();
        //Hash para enviar a guardar el registro.
        protected Hashtable phtValuesEmple;
        //Para especificar en los jAlert de DSOControl
        protected string pjsObj;

        /*RZ.20130730 Nuevos campos para usuarios*/
        string psNewUsuario;
        string psNewPassword;

        protected StringBuilder psQuery = new StringBuilder();
        private string iCodCatalogoEmple;
        /*RZ.20130718 Dejar como campo en la clase el estado*/
        private string estado;

        protected string psFileKey; //**PT** PDF
        protected string psTempPath;//**PT** PDF

        private string tIdEmple;
        private string claveFacEmple;
        private string userEmple;
        private string passwordEmple;
        static int razonSocialId = 0;
        private string lineaImei = "";
        //Crear instancia del webservice
        public CCustodia webServiceCCustodia = new CCustodia();

        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            Panel pnlControlDeFechas = (Panel)Form.FindControl("pnlRangeFechas");
            pnlControlDeFechas.Visible = false;
            #endregion

            //**PT** Variables necesarias para la exportacion a pdf
            psFileKey = Guid.NewGuid().ToString();
            psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Session.SessionID);
            System.IO.Directory.CreateDirectory(psTempPath);

            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            //Esto solo aplica para el esquema de K5Banorte
            trOpcSincronizacion.Visible = DSODataContext.Schema.ToLower() != "k5banorte" ? false : true;

            if (DSODataContext.Schema.ToUpper() == "BAT")
            {
                btnEnviarCCustodiaEmple.Enabled = false;
                btnCambiarEstatusPte.Enabled = false;
            }
            iCodCatalogoEmple = Request.QueryString["iCodEmple"];

            /* Leer parametros para establecer modo en que la página se mostrará
                 * Edicion : edit 
                 * Lectura : ronly
                 * Alta : alta
                 * El parametro se recibe encriptado.
            */

            /*RZ.20130722*/
            estado = KeytiaServiceBL.Util.Decrypt(Request.QueryString["st"]);

            if (!Page.IsPostBack)
            {
                /*Agregar este event handler al dropdownlist de modelos*/
                //drpModeloPopUp.Attributes.Add("onchange", "GetModeloId(this.options[this.selectedIndex].value)");

                EstablecerEstado(estado);

                FillDropDowns();


                //RM 20190329
                //
                if (DSODataContext.Schema.ToUpper() == "FCA")
                {
                    FillDDLFCA();
                    ModifVisiilidadCamposFCA();
                    divLinea.Attributes["class"] = divLinea.Attributes["class"].Replace("col-md-2 col-sm-2", "col-md-3 col-sm-3");
                    divFechaIni.Attributes["class"] = divFechaIni.Attributes["class"].Replace("col-md-2 col-sm-2", "col-md-3 col-sm-3");

                    pnlDatosEmpleFCACol1.Visible = true;
                    pnlDatosEmpleFCAcol2.Visible = true;
                    if (String.IsNullOrEmpty(iCodCatalogoEmple))
                    {
                        pnlNcknamegroup.Visible = false;
                    }
                    cbDatosEmpleFCAEsDirector.Visible = true;
                    cbEsGerenteEmple.Visible = false;
                    cbVisibleDirEmple.Visible = false;

                    txtUsuarRedEmple.Text = "";
                    pnlDatosUsuar.Enabled = false;
                    pnlDatosUsuar.Visible = false;

                    /*Obtiene los datos de usuario del empleado seleccionado*/

                    ObtieneUserEmple(iCodCatalogoEmple);
                }
                /*RZ.20130718 Se valida si es icodcatalogo del empleado esta nulo o vacio para saber si se requiere cargas los datos y recursos del empleado*/
                if (!String.IsNullOrEmpty(iCodCatalogoEmple))
                {
                    /*Valida si el empleado cuenta con carta custodia*/
                    AltaCartaCustodia(iCodCatalogoEmple);

                    DataTable dtEmple = cargaDatosEmple(iCodCatalogoEmple);
                    
                    FillDatosEmple(dtEmple);

                    //20170620 NZ Se Comenta esta sección.
                    //FillInventarioGrid();

                    FillExtenGrid();

                    FillCodAutoGrid();

                    FillLineaGrid();

                    FillDDLCos();
                }

            }
            if (pnlAltaDeLinea.Visible == true)
            {
                if (rbtnAlta.Checked == true)
                {
                    divICCID.Visible = true;
                    divCarrierLineaReg.Visible = true;
                    divSitioLineaReg.Visible = true;
                }
                else
                {
                    divICCID.Visible = false;
                    divCarrierLineaReg.Visible = false;
                    divSitioLineaReg.Visible = false;
                }
            }
        }

        protected void drpJefeEmple_IndexChanged(object sender, EventArgs e)
        {
            //string iCodCatEmpleJefe = drpJefeEmple.SelectedValue;

            //txtEmailJefeEmple.Text = webServiceCCustodia.ObtieneEmpleMail(iCodCatEmpleJefe);
        }
        [WebMethod]
        public static object GetLocalidad(string texto)
        {
            string connStr = DSODataContext.ConnectionString;

            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" DISTINCT UPPER(Ubica) AS Descripcion");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".HISTEMPLE WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND Ubica LIKE '%" + texto + "%'");
            DataTable dtUbica = DSODataAccess.Execute(query.ToString(), connStr);
            return FCAndControls.ConvertDataTabletoJSONString(dtUbica);
        }
        [WebMethod]
        public static object GetRegionLinea(string texto)
        {
            string connStr = DSODataContext.ConnectionString;

            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" DISTINCT UPPER(Filler) AS Descripcion");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".HistLinea WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND Filler LIKE '%" + texto + "%'");
            DataTable dtRegion = DSODataAccess.Execute(query.ToString(), connStr);
            return FCAndControls.ConvertDataTabletoJSONString(dtRegion);
        }
        [WebMethod]
        public static object GetJefeEmple(string texto)
        {
            string connStr = DSODataContext.ConnectionString;

            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" iCodCatalogo AS ID,Nomcompleto AS Descripcion, ISNULL(Email,'') AS Email");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".HistEmple WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND Nomcompleto LIKE '%" + texto + "%'");
            DataTable dtJefe = DSODataAccess.Execute(query.ToString(), connStr);
            return FCAndControls.ConvertDataTabletoJSONString(dtJefe);
        }
        [WebMethod]
        public static object GetAreasLinea(string texto)
        {
            string connStr = DSODataContext.ConnectionString;

            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" ISNULL(iCodCatalogo, 0) AS iCodCatalogo,ISNULL(UPPER(Descripcion), '') AS Descripcion");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".[vishistoricos('Area','Areas','Español')] WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND Descripcion LIKE '%" + texto + "%'");
            DataTable dtAreas = DSODataAccess.Execute(query.ToString(), connStr);
            return FCAndControls.ConvertDataTabletoJSONString(dtAreas);
        }
        [WebMethod]
        public static object GetCencosRazon(string texto)
        {
            StringBuilder query = new StringBuilder();
            string connStr = DSODataContext.ConnectionString;
            //int idRazonSocial = Convert.ToInt32(hdfRazonId.Value);
            query.AppendLine(" SELECT");
            query.AppendLine(" C.iCodCatalogo,");
            query.AppendLine(" C.Descripcion ");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".[VisRelaciones('RazonSocial-CenCos','Español')] AS RC WITH(NOLOCK)");
            query.AppendLine(" JOIN " + DSODataContext.Schema + ".HistCenCos AS C WITH(NOLOCK)");
            query.AppendLine(" ON RC.CenCos = C.iCodCatalogo");
            query.AppendLine(" AND C.dtIniVigencia<> C.dtFinVigencia");
            query.AppendLine(" AND C.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE RC.dtIniVigencia<> RC.dtFinVigencia");
            query.AppendLine(" AND RC.dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND RC.RazonSocial = " + razonSocialId + "");
            query.AppendLine(" AND C.Descripcion LIKE '%" + texto + "%'");
            DataTable dtCencos = DSODataAccess.Execute(query.ToString(), connStr);
            return FCAndControls.ConvertDataTabletoJSONString(dtCencos);
        }
        /*RZ.20130718 Se agrega validacion para que no llene dropdownlist
         de tipo de extension cuando se trate de estado alta*/

        protected void FillDropDowns()
        {
            if (estado != "alta")
            {
                FillDDLTipoExten();
                //FillDDLCos(); NZ Se comentan por que ahora de filtraran antes de ser llenados.
                FillDDLCarrier();
                FillDDLRazonSocial();
                FillDDLTipoPlan();
                FillDDLTipoEquipo();
                FillDDLAreaLinea();
                FillDDLModeloEquCelular();
                FillEquipoCel();
                FillDDSitioLine();
                OcultaAsteriscos(false);
            }

            if (!String.IsNullOrEmpty(iCodCatalogoEmple) && estado == "edit")
            {
                drpNuevoEmpleResp.DataSource = fillNuevoEmpleResp(iCodCatalogoEmple);
                drpNuevoEmpleResp.DataValueField = "iCodCatalogo";
                drpNuevoEmpleResp.DataTextField = "vchDescripcion";
                drpNuevoEmpleResp.DataBind();
            }

            FillDDLSitios();

            FillDDLPuestoEmple();

            //FillDDLJefeEmple();

            FillDDLCenCosEmple();

            FillDDLLocaliEmple();

            FillDDLTipoEmple();

            FillDDLEmpreOuts();

            FillDDLEmpreCenCos();
            if (DSODataContext.Schema.ToUpper() == "FCA")
            {
                FillDDLDatosEmpleFCADirector();

                FillDDLDatosEmpleFCAPlanta();
            }

        }

        protected void FillDDLLocaliEmple()
        {
            lsbQuery.Length = 0;

            if (DSODataContext.Schema.ToUpper() == "BAT")
            {
                txtLocalidadEmple.Visible = true;
                drpLocalidadEmple.Visible = false;
            }
            else
            {
                drpLocalidadEmple.Visible = true;
                dtLocaliEmple = NuevoEmpleadoBackend.GetLocalidades();

                drpLocalidadEmple.DataSource = dtLocaliEmple;
                drpLocalidadEmple.DataValueField = "iCodCatalogo";
                drpLocalidadEmple.DataTextField = "vchDescripcion";
                drpLocalidadEmple.DataBind();
            }

        }

        protected void FillDDLCenCosEmple()
        {
            lsbQuery.Length = 0;

            lsbQuery.AppendLine("declare @schema  varchar(40) = '" + DSODataContext.Schema + "'														");
            lsbQuery.AppendLine("declare @vchCodUsuar varchar(40) = '" + HttpContext.Current.Session["vchCodUsuario"] + "'												");
            lsbQuery.AppendLine("declare @iCodCatUsuario  int =0															");
            lsbQuery.AppendLine("declare @query nvarchar(max) = ''															");
            lsbQuery.AppendLine("																							");
            lsbQuery.AppendLine("Set @query = 																				");
            lsbQuery.AppendLine("'																							");
            lsbQuery.AppendLine("	Select @iCodCatUsuario = iCodCatalogo													");
            lsbQuery.AppendLine("	From vUsuario usuar																		");
            lsbQuery.AppendLine("	Where usuar.dtIniVigencia <> usuar.dtFinVigencia										");
            lsbQuery.AppendLine("	And usuar.dtFinVigencia >= GETDATE()													");
            lsbQuery.AppendLine("	And usuar.vchCodigo = '''+@vchCodUsuar+'''												");
            lsbQuery.AppendLine("'																							");
            lsbQuery.AppendLine("																							");
            lsbQuery.AppendLine("EXEC sp_executesql @query, N'@iCodCatUsuario  INT OUTPUT ', @iCodCatUsuario  OUTPUT		");
            lsbQuery.AppendLine("																							");
            lsbQuery.AppendLine("--------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
            lsbQuery.AppendLine("/* OBTENER EL VALOR DE LAS BANDERAS DE OMITIR RESTRICCIONES DE EMPLE, CENCOS Y SITIO */");
            lsbQuery.AppendLine("DECLARE @nQueryOmitirRestricciones NVARCHAR(MAX) = '';								");
            lsbQuery.AppendLine("DECLARE @omitirRestSitio bit = 0														");
            lsbQuery.AppendLine("DECLARE @omitirRestCenCos bit = 0														");
            lsbQuery.AppendLine("DECLARE @omitirRestEmple bit = 0														");
            lsbQuery.AppendLine("																						");
            lsbQuery.AppendLine("SET @nQueryOmitirRestricciones = '													");
            lsbQuery.AppendLine("		SELECT @omitirRestSitio = (isnull(banderasusuar,0) & 1)/1,						");
            lsbQuery.AppendLine("				@omitirRestCenCos = (isnull(banderasusuar,0) & 2)/2,					");
            lsbQuery.AppendLine("				@omitirRestEmple = (isnull(banderasusuar,0) & 4)/4						");
            lsbQuery.AppendLine("		FROM Keytia5.['+@Schema+'].vTIUsuarios  										");
            lsbQuery.AppendLine("		WHERE dtFinVigencia >= GETDATE()												");
            lsbQuery.AppendLine("		AND iCodCatalogo = ' + Convert(VARCHAR, ISNULL(@iCodCatUsuario,0))				");
            lsbQuery.AppendLine("																						");
            lsbQuery.AppendLine("EXEC sp_executesql @nQueryOmitirRestricciones, N'@omitirRestSitio INT OUTPUT, @omitirRestCenCos INT OUTPUT, @omitirRestEmple INT OUTPUT ', @omitirRestSitio OUTPUT, @omitirRestCenCos OUTPUT, @omitirRestEmple OUTPUT");
            lsbQuery.AppendLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
            lsbQuery.AppendLine("																							");
            lsbQuery.AppendLine("																							");
            lsbQuery.AppendLine("Set @query =																				");
            lsbQuery.AppendLine("'																							");
            lsbQuery.AppendLine("	SELECT                                                  								");
            lsbQuery.AppendLine("		iCodCatalogo = cencos.iCodCatalogo,                                      					");
            lsbQuery.AppendLine("		vchDescripcion  = cencos.vchCodigo + '' '' + cencos.Descripcion                                   ");
            lsbQuery.AppendLine("	FROM ['+@Schema+'].[VisHistoricos(''CenCos'',''Centro de Costos'',''Español'')]	cencos		    ");
            lsbQuery.AppendLine("																							");
            lsbQuery.AppendLine("'																							");
            lsbQuery.AppendLine("																							");
            lsbQuery.AppendLine("if (@iCodCatUsuario is not null And @iCodCatUsuario > 0)									");
            lsbQuery.AppendLine("Begin																						");
            lsbQuery.AppendLine("	if	@omitirRestCenCos = 0																");
            lsbQuery.AppendLine("	Begin																					");
            lsbQuery.AppendLine("		Set @Query = @Query + '																");
            lsbQuery.AppendLine("			INNER JOIN ['+@Schema+'].RestCenCos RestCencos									");
            lsbQuery.AppendLine("				ON cencos.iCodCatalogo = RestCencos.Cencos								    ");
            lsbQuery.AppendLine("				And cencos.dtIniVigencia <= RestCencos.FechaInicio						    ");
            lsbQuery.AppendLine("				And cencos.dtFinVigencia >= RestCencos.Fechafin							    ");
            lsbQuery.AppendLine("				And RestCencos.Usuar = ' + convert(varchar, @iCodCatUsuario) + ''			");
            lsbQuery.AppendLine("	End																						");
            lsbQuery.AppendLine("																							");
            lsbQuery.AppendLine("End																						");
            lsbQuery.AppendLine("																							");
            lsbQuery.AppendLine("Set @query = @query+ 																		");
            lsbQuery.AppendLine("'																							");
            lsbQuery.AppendLine("	Where cencos.dtIniVigencia<> cencos.dtFinVigencia                    			    ");
            lsbQuery.AppendLine("	and cencos.dtFinVigencia >= GETDATE()                         						    ");
            lsbQuery.AppendLine("	and cencos.vchCodigo<> ''999999999''													");
            lsbQuery.AppendLine("   And Len(cencos.vchCodigo + '' '' + cencos.Descripcion) > 0");
            lsbQuery.AppendLine("   Order By (cencos.vchCodigo + '' '' + cencos.Descripcion)");
            lsbQuery.AppendLine("   ");
            lsbQuery.AppendLine("'																							");
            lsbQuery.AppendLine("																							");
            lsbQuery.AppendLine("																							");
            lsbQuery.AppendLine("																							");
            lsbQuery.AppendLine("exec(@query)																				");




            dtCenCosEmple = DSODataAccess.Execute(lsbQuery.ToString());



            drpCenCosEmple.DataSource = dtCenCosEmple;
            drpCenCosEmple.DataValueField = "iCodCatalogo";
            drpCenCosEmple.DataTextField = "vchDescripcion";
            drpCenCosEmple.DataBind();

            ddlDatosEmpleFCADpto.DataSource = dtCenCosEmple;
            ddlDatosEmpleFCADpto.DataValueField = "iCodCatalogo";
            ddlDatosEmpleFCADpto.DataTextField = "vchDescripcion";
            ddlDatosEmpleFCADpto.DataBind();
        }

        protected void FillDDLJefeEmple()
        {
            lsbQuery.Length = 0;


            dtJefeEmple = NuevoEmpleadoBackend.GetJefes();

            //drpJefeEmple.DataSource = dtJefeEmple;
            //drpJefeEmple.DataValueField = "iCodCatalogo";
            //drpJefeEmple.DataTextField = "vchDescripcion";
            //drpJefeEmple.DataBind();

        }

        protected void FillDDLPuestoEmple()
        {



            dtPuestoEmple = NuevoEmpleadoBackend.GetPuestos();

            drpPuestoEmple.Items.Clear();
            ListItem i;
            i = new ListItem("-- Selecciona Uno --", "");
            drpPuestoEmple.Items.Add(i);

            drpPuestoEmple.DataSource = dtPuestoEmple;
            drpPuestoEmple.DataValueField = "iCodCatalogo";
            drpPuestoEmple.DataTextField = "vchDescripcion";
            drpPuestoEmple.DataBind();
        }

        protected void FillDDLTipoExten()
        {
            lsbQuery.Length = 0;
            lsbQuery.Append("SELECT iCodCatalogo, vchDescripcion \r");
            lsbQuery.Append("FROM [vishistoricos('TipoRecurso','Tipos de recurso','Español')] WITH(NOLOCK)\r");
            lsbQuery.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
            lsbQuery.Append("and dtFinVigencia >= GETDATE() \r");
            lsbQuery.Append("order by CASE WHEN vchDescripcion like '%EXTENSION%PRINCIPAL%' THEN 1 ELSE 2 END");

            dtTipoExten = DSODataAccess.Execute(lsbQuery.ToString());

            drpTipoExten.DataSource = dtTipoExten;

            drpTipoExten.DataValueField = "iCodCatalogo";
            drpTipoExten.DataTextField = "vchDescripcion";
            drpTipoExten.DataBind();

            /*Controles de extensiones no pop-up*/
            drpTipoExtenNoPopUp.DataSource = dtTipoExten;

            drpTipoExtenNoPopUp.DataValueField = "iCodCatalogo";
            drpTipoExtenNoPopUp.DataTextField = "vchDescripcion";
            drpTipoExtenNoPopUp.DataBind();
        }

        protected void FillDDLSitios()
        {
            /*RZ.20130718 Se agrega validacion para que no llene los dropdowns de
            extensiones y codigos cuando se trate de alta*/

            lsbQuery.Length = 0;

            dtSitios = NuevoEmpleadoBackend.GetSitios(Convert.ToInt32(Session["iCodUsuario"].ToString()));

            if (estado != "alta")
            {
                /*DropDownsList para Extensiones*/
                drpSitio.DataSource = dtSitios;
                drpSitio.DataValueField = "iCodCatalogo";
                drpSitio.DataTextField = "vchDescripcion";
                drpSitio.DataBind();

                /*DropDownsList para Extensiones No pop-up*/
                drpSitioNoPopUp.DataSource = dtSitios;
                drpSitioNoPopUp.DataValueField = "iCodCatalogo";
                drpSitioNoPopUp.DataTextField = "vchDescripcion";
                drpSitioNoPopUp.DataBind();

                /*DropDownsList para Codigos de Autorizacion*/
                drpSitioCodAuto.DataSource = dtSitios;
                drpSitioCodAuto.DataValueField = "iCodCatalogo";
                drpSitioCodAuto.DataTextField = "vchDescripcion";
                drpSitioCodAuto.DataBind();

                /*DropDownsList para Codigos de Autorizacion No pop-up*/
                drpSitioCodAutoNoPopUp.DataSource = dtSitios;
                drpSitioCodAutoNoPopUp.DataValueField = "iCodCatalogo";
                drpSitioCodAutoNoPopUp.DataTextField = "vchDescripcion";
                drpSitioCodAutoNoPopUp.DataBind();

                /*DropDownsList para Lineas*/
                //NZ 20160622
                drpSitioLinea.DataSource = dtSitios;
                drpSitioLinea.DataValueField = "iCodCatalogo";
                drpSitioLinea.DataTextField = "vchDescripcion";
                drpSitioLinea.DataBind();

                /*DropDownsList para Lineas No pop-up*/
                //NZ 20160622
                drpSitioLineaNoPopUp.DataSource = dtSitios;
                drpSitioLineaNoPopUp.DataValueField = "iCodCatalogo";
                drpSitioLineaNoPopUp.DataTextField = "vchDescripcion";
                drpSitioLineaNoPopUp.DataBind();
            }

            /*DropDownList para Ubicacion del Empleado*/
            drpSitioEmple.DataSource = dtSitios;
            drpSitioEmple.DataValueField = "iCodCatalogo";
            drpSitioEmple.DataTextField = "vchDescripcion";
            drpSitioEmple.DataBind();
        }

        protected void FillDDLTipoEmple()
        {
            lsbQuery.Length = 0;

            dtTipoEmple = NuevoEmpleadoBackend.GetTiposEmple();

            /*DropDownsList para Tipo de Empleado*/
            drpTipoEmpleado.DataSource = dtTipoEmple;
            drpTipoEmpleado.DataValueField = "iCodCatalogo";
            drpTipoEmpleado.DataTextField = "vchDescripcion";
            drpTipoEmpleado.DataBind();
        }

        protected void FillDDLEmpreOuts()
        {
            lsbQuery.Length = 0;

            dtEmpreEmple = NuevoEmpleadoBackend.GetProveedor();

            /*DropDownsList para Empresa OutSource*/
            drpEmpresaEmple.DataSource = dtEmpreEmple;
            drpEmpresaEmple.DataValueField = "iCodCatalogo";
            drpEmpresaEmple.DataTextField = "vchDescripcion";
            drpEmpresaEmple.DataBind();
        }

        protected void FillDDLCos()
        {
            lsbQuery.Length = 0;


            dtCos = NuevoEmpleadoBackend.GetCenCos();

            /*DropDownsList para Cos CodAuto */
            drpCosCodAuto.DataSource = dtCos;
            drpCosCodAuto.DataBind();

            /*DropDownsList para Cos CodAuto no pop-up*/
            drpCosCodAutoNoPopUp.DataSource = dtCos;
            drpCosCodAutoNoPopUp.DataBind();

            /*DropDownsList para Extensiones */
            drpCosExten.DataSource = dtCos;
            drpCosExten.DataBind();

            /*DropDownsList para Cos Extensiones no pop-up*/
            drpCosExtenNoPopUp.DataSource = dtCos;
            drpCosExtenNoPopUp.DataBind();


        }

        protected void FillDDLEmpreCenCos()
        {
            lsbQuery.Length = 0;


            dtEmpreCenCos = NuevoEmpleadoBackend.GetEmpre();

            drpCenCosEmpresa.DataSource = dtEmpreCenCos;
            drpCenCosEmpresa.DataValueField = "iCodCatalogo";
            drpCenCosEmpresa.DataTextField = "vchDescripcion";
            drpCenCosEmpresa.DataBind();
        }

        protected void FillDDLCarrier()
        {
            lsbQuery.Length = 0;


            dtCarrier = NuevoEmpleadoBackend.GetCarrier();

            drpCarrierLinea.DataSource = dtCarrier;
            drpCarrierLinea.DataValueField = "iCodCatalogo";
            drpCarrierLinea.DataTextField = "vchDescripcion";
            drpCarrierLinea.DataBind();

            /*DropDownsList para Carrier no pop-up*/
            drpCarrierLineaNoPopUp.DataSource = dtCarrier;
            drpCarrierLineaNoPopUp.DataValueField = "iCodCatalogo";
            drpCarrierLineaNoPopUp.DataTextField = "vchDescripcion";
            drpCarrierLineaNoPopUp.DataBind();
        }

        protected void FillDDLCtaMaestra(string iCodCatCarrier)
        {
            lsbQuery.Length = 0;

            int carrier = 0;
            int.TryParse(iCodCatCarrier, out carrier);

            dtCtaMaestraLinea = NuevoEmpleadoBackend.GetCtaMaestraCarrier(carrier);

            /*DropDownsList para Cuenta Maestra del pop-up */
            drpCtaMaestraLinea.DataSource = dtCtaMaestraLinea;
            drpCtaMaestraLinea.DataValueField = "iCodCatalogo";
            drpCtaMaestraLinea.DataTextField = "vchDescripcion";
            drpCtaMaestraLinea.DataBind();
        }

        protected void FillDDLRazonSocial()
        {
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("SELECT iCodCatalogo, vchDescripcion");
            lsbQuery.AppendLine("FROM [VisHistoricos('RazonSocial','Razon Social','Español')] WITH(NOLOCK)");
            lsbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            lsbQuery.AppendLine("   AND dtFinVigencia >= GETDATE()");
            lsbQuery.AppendLine("ORDER BY vchDescripcion");

            dtRazonSocialLinea = DSODataAccess.Execute(lsbQuery.ToString());

            /*DropDownsList para Razón Social del pop-up */
            drpRazonSocialLinea.DataSource = dtRazonSocialLinea;
            drpRazonSocialLinea.DataValueField = "iCodCatalogo";
            drpRazonSocialLinea.DataTextField = "vchDescripcion";
            drpRazonSocialLinea.DataBind();

        }
        protected void FillDDCenCosto(int razonSocial)
        {
            StringBuilder q = new StringBuilder();
            if (DSODataContext.Schema.Trim().ToUpper() == "BAT")
            {
                q.AppendLine(" SELECT");
                q.AppendLine(" C.iCodCatalogo,");
                q.AppendLine(" C.Descripcion ");
                q.AppendLine(" FROM " + DSODataContext.Schema + ".[VisRelaciones('RazonSocial-CenCos','Español')] AS RC WITH(NOLOCK)");
                q.AppendLine(" JOIN " + DSODataContext.Schema + ".HistCenCos AS C WITH(NOLOCK)");
                q.AppendLine(" ON RC.CenCos = C.iCodCatalogo");
                q.AppendLine(" AND C.dtIniVigencia<> C.dtFinVigencia");
                q.AppendLine(" AND C.dtFinVigencia >= GETDATE()");
                q.AppendLine(" WHERE RC.dtIniVigencia<> RC.dtFinVigencia");
                q.AppendLine(" AND RC.dtFinVigencia >= GETDATE()");
                q.AppendLine(" AND RC.RazonSocial = " + razonSocial + "");
                //q.AppendLine(" DECLARE @IcodCencos INT");
                //q.AppendLine(" SELECT @IcodCencos = iCodCatalogo FROM [vishistoricos('CenCos','Centro de costos','Español')]WITH(NOLOCK)");
                //q.AppendLine(" WHERE dtIniVigencia<> dtFinVigencia AND dtFinVigencia >= GETDATE()");
                //q.AppendLine(" AND descripcion = '"+ razonSocial + "'");
                //q.AppendLine(" SELECT iCodCatalogo, Descripcion FROM [vishistoricos('CenCos','Centro de costos','Español')] WITH(NOLOCK)");
                //q.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
                //q.AppendLine(" AND CenCos= @IcodCencos");

            }
            else
            {
                q.AppendLine(" SELECT iCodCatalogo,Descripcion FROM [vishistoricos('CenCos','Centro de costos','Español')] WITH(NOLOCK)");
                q.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");

            }

            DataTable dt = DSODataAccess.Execute(q.ToString());
            drpCentroCosto.Items.Clear();
            drpCentroCosto.DataSource = null;
            drpCentroCosto.DataBind();

            //drpCentroCosto.Items.Insert(0, "-- Selecciona Uno --");
            //drpCentroCosto.SelectedIndex = 0;
            drpCentroCosto.DataSource = dt;
            drpCentroCosto.DataValueField = "iCodCatalogo";
            drpCentroCosto.DataTextField = "Descripcion";
            drpCentroCosto.DataBind();
        }
        protected void RelLineaCencos(int icodLine)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT iCodRegistro,CenCos,A.CenCosCod,dtIniVigencia");
            query.AppendLine(" FROM [VisRelaciones('CentroCosto-Lineas','Español')] AS A WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia");
            query.AppendLine(" AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND Linea = " + icodLine + "");
            DataTable dt = DSODataAccess.Execute(query.ToString());
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                Session["IdCencostoLinea"] = dr["CenCos"].ToString();
                Session["FecRelLineaCencos"] = Convert.ToDateTime(dr["dtIniVigencia"]).ToString("dd/MM/yyyy");
                Session["idRelacionCencostoLinea"] = dr["iCodRegistro"].ToString();

                txtCentroCosto.Text = dr["CenCosCod"].ToString();             
                hdfCentroCosto.Value = dr["CenCos"].ToString();

                //drpCentroCosto.Text = dr["CenCos"].ToString();

                txtFechaRelacion.Text = (dr["dtIniVigencia"] != DBNull.Value) ? Convert.ToDateTime(dr["dtIniVigencia"]).ToString("dd/MM/yyyy") : string.Empty;
            }

        }
        protected void FillDDLTipoPlan()
        {
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("SELECT iCodCatalogo, vchDescripcion");
            lsbQuery.AppendLine("FROM [VisHistoricos('TipoPlan','Tipo Plan','Español')] WITH(NOLOCK)");
            lsbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            lsbQuery.AppendLine("   AND dtFinVigencia >= GETDATE()");
            lsbQuery.AppendLine("ORDER BY vchDescripcion");

            dtTipoPlanLinea = DSODataAccess.Execute(lsbQuery.ToString());

            /*DropDownsList para Tipo Plan del pop-up */
            drpTipoPlanLinea.DataSource = dtTipoPlanLinea;
            drpTipoPlanLinea.DataValueField = "iCodCatalogo";
            drpTipoPlanLinea.DataTextField = "vchDescripcion";
            drpTipoPlanLinea.DataBind();
        }

        protected void FillDDLTipoEquipo()
        {
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("SELECT iCodCatalogo, vchDescripcion");
            lsbQuery.AppendLine("FROM [VisHistoricos('EqCelular','Equipo Celular','Español')] WITH(NOLOCK)");
            lsbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            lsbQuery.AppendLine("   AND dtFinVigencia >= GETDATE()");
            lsbQuery.AppendLine("ORDER BY vchDescripcion");

            dtTipoEquipoLinea = DSODataAccess.Execute(lsbQuery.ToString());

            drpTipoEquipoLinea.Items.Clear();
            ListItem i;
            i = new ListItem("-- Selecciona Uno --", "");
            drpTipoEquipoLinea.Items.Add(i);

            /*DropDownsList para Tipo Equipo del pop-up */
            drpTipoEquipoLinea.DataSource = dtTipoEquipoLinea;
            drpTipoEquipoLinea.DataValueField = "iCodCatalogo";
            drpTipoEquipoLinea.DataTextField = "vchDescripcion";
            drpTipoEquipoLinea.DataBind();
        }
        protected void FillDDLAreaLinea()
        {
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("SELECT iCodCatalogo, RTRIM(LTRIM(UPPER(Descripcion))) AS Descripcion");
            lsbQuery.AppendLine("FROM [vishistoricos('Area','Areas','Español')] WITH(NOLOCK)");
            lsbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            lsbQuery.AppendLine("   AND dtFinVigencia >= GETDATE()");
            lsbQuery.AppendLine("ORDER BY vchDescripcion");

            dtAreaLinea = DSODataAccess.Execute(lsbQuery.ToString());

            /*DropDownsList para Tipo Equipo del pop-up */
            drpAreaLinea.DataSource = dtAreaLinea;
            drpAreaLinea.DataValueField = "iCodCatalogo";
            drpAreaLinea.DataTextField = "Descripcion";
            drpAreaLinea.DataBind();
        }
        //RM 20190510  
        protected void FillDDLDatosEmpleFCADirector()
        {
            try
            {


                dtDatosFCADirectores = DSODataAccess.Execute(lsbQuery.ToString());

                /*DropDownList de los datos de empleado para directores*/
                if (dtDatosFCADirectores.Rows.Count > 0)
                {
                    ddlDatosEmpleFCADirector.DataSource = dtDatosFCADirectores;
                    ddlDatosEmpleFCADirector.DataValueField = "iCodCat";
                    ddlDatosEmpleFCADirector.DataTextField = "Nombre";
                    ddlDatosEmpleFCADirector.DataBind();
                }

            }
            catch (Exception ex)
            {
            }
        }

        public void FillDDLDatosEmpleFCAPlanta()
        {
            try
            {
                dtDatosFCAPlantas = NuevoEmpleadoBackend.GetPlantasFCA();

                /*DropDownList de los datos de empleado para directores*/
                if (dtDatosFCAPlantas.Rows.Count > 0)
                {
                    ddlDatosEmpleFCAPlanta.DataSource = dtDatosFCAPlantas;
                    ddlDatosEmpleFCAPlanta.DataValueField = "iCodCatalogo";
                    ddlDatosEmpleFCAPlanta.DataTextField = "Descripcion";
                    ddlDatosEmpleFCAPlanta.DataBind();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void FillDDLPlanTarifario(string iCodCatCarrier)
        {
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("SELECT iCodCatalogo, vchDescripcion");
            lsbQuery.AppendLine("FROM [VisHistoricos('PlanTarif','Plan Tarifario','Español')] WITH(NOLOCK)");
            lsbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            lsbQuery.AppendLine("   AND dtFinVigencia >= GETDATE()");
            lsbQuery.AppendLine("   AND Carrier = " + iCodCatCarrier);  //Filtrar las cuentas del carrier del que sea la linea.
            lsbQuery.AppendLine("ORDER BY vchDescripcion");

            dtPlanTarifarioLinea = DSODataAccess.Execute(lsbQuery.ToString());

            drpPlanTarifarioLinea.Items.Clear();
            ListItem i;
            i = new ListItem("-- Selecciona Uno --", "");
            drpPlanTarifarioLinea.Items.Add(i);
            /*DropDownsList para Plan Tarifario del pop-up */
            drpPlanTarifarioLinea.DataSource = dtPlanTarifarioLinea;
            drpPlanTarifarioLinea.DataValueField = "iCodCatalogo";
            drpPlanTarifarioLinea.DataTextField = "vchDescripcion";
            drpPlanTarifarioLinea.DataBind();
        }
        protected void FillDDlCategoriaAsignacion()
        {
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("SELECT icodCatalogo, Descripcion FROM " + DSODataContext.Schema + ".[VisHistoricos('LineaCategoriaAsignacion','Linea Categorías Asignación','Español')]");
            lsbQuery.AppendLine("WHERE dtIniVigencia<> dtfinvigencia");
            lsbQuery.AppendLine("AND dtfinvigencia >= GETDATE()");

            DataTable dt = DSODataAccess.Execute(lsbQuery.ToString());

            drpCategoriaAsignacion.Items.Clear();
            ListItem i;
            i = new ListItem("-- Selecciona Uno --", "");
            drpCategoriaAsignacion.Items.Add(i);

            drpCategoriaAsignacion.DataSource = dt;
            drpCategoriaAsignacion.DataValueField = "icodCatalogo";
            drpCategoriaAsignacion.DataTextField = "Descripcion";
            drpCategoriaAsignacion.DataBind();
        }
        protected void FillDDLTipoAsignacion()
        {
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("SELECT icodCatalogo, Descripcion FROM " + DSODataContext.Schema + ".[VisHistoricos('LineaTipoAsignacion','Linea Tipos Asignación','Español')]");
            lsbQuery.AppendLine("WHERE dtIniVigencia<> dtfinvigencia");
            lsbQuery.AppendLine("AND dtfinvigencia >= GETDATE()");

            DataTable dt = DSODataAccess.Execute(lsbQuery.ToString());

            drpTipoAsignacion.Items.Clear();
            ListItem i;
            i = new ListItem("-- Selecciona Uno --", "");
            drpTipoAsignacion.Items.Add(i);

            drpTipoAsignacion.DataSource = dt;
            drpTipoAsignacion.DataValueField = "icodCatalogo";
            drpTipoAsignacion.DataTextField = "Descripcion";
            drpTipoAsignacion.DataBind();
        }
        protected void FillDDLModeloEquCelular()
        {
            StringBuilder q = new StringBuilder();
            q.AppendLine(" SELECT");
            q.AppendLine(" icodCatalogo,");
            q.AppendLine(" Descripcion");
            q.AppendLine(" FROM " + DSODataContext.Schema + ".[VisHistoricos('ModeloEqCelular','Modelos equipo celular','Español')] WITH(NOLOCK)");
            q.AppendLine(" WHERE dtIniVigencia<> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            DataTable dt = DSODataAccess.Execute(q.ToString());

            drpModeloEquipo.Items.Clear();
            ListItem i;
            i = new ListItem("-- Selecciona Uno --", "");
            drpModeloEquipo.Items.Add(i);

            drpModeloEquipo.DataSource = dt;
            drpModeloEquipo.DataValueField = "icodCatalogo";
            drpModeloEquipo.DataTextField = "Descripcion";
            drpModeloEquipo.DataBind();
        }

        protected void FillEquipoCel()
        {
            StringBuilder q = new StringBuilder();
            q.AppendLine(" SELECT");
            q.AppendLine(" iCodCatalogo,");
            q.AppendLine(" Descripcion");
            q.AppendLine(" FROM " + DSODataContext.Schema + ".[VisHistoricos('EqCelular','Equipo Celular','Español')] WITH(NOLOCK)");
            q.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            DataTable dt = DSODataAccess.Execute(q.ToString());

            drpTipoEquipoLinea.Items.Clear();
            ListItem i;
            i = new ListItem("-- Selecciona Uno --", "");
            drpTipoEquipoLinea.Items.Add(i);

            drpTipoEquipoLinea.DataSource = dt;
            drpTipoEquipoLinea.DataValueField = "icodCatalogo";
            drpTipoEquipoLinea.DataTextField = "Descripcion";
            drpTipoEquipoLinea.DataBind();
        }
        protected void FillDDSitioLine()
        {
            StringBuilder q = new StringBuilder();
            q.AppendLine(" SELECT");
            q.AppendLine(" icodCataLogo,");
            q.AppendLine(" NombreUbicacion");
            q.AppendLine(" FROM " + DSODataContext.Schema + ".[vishistoricos('UbicacionLinea','Ubicación Líneas','español')] WITH(NOLOCK)");
            q.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            DataTable dt = DSODataAccess.Execute(q.ToString());

            drpLineaSitio.Items.Clear();
            ListItem i;
            i = new ListItem("-- Selecciona Uno --", "");
            drpLineaSitio.Items.Add(i);


            drpLineaSitio.DataSource = dt;
            drpLineaSitio.DataValueField = "icodCataLogo";
            drpLineaSitio.DataTextField = "NombreUbicacion";
            drpLineaSitio.DataBind();
        }
        protected void FillValoresBanderasLineas()
        {
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("SELECT vchCodigo, Value");
            lsbQuery.AppendLine("FROM [VisHistoricos('Valores','Valores','Español')] WITH(NOLOCK)");
            lsbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            lsbQuery.AppendLine("  AND dtFinVigencia >= GETDATE()");
            lsbQuery.AppendLine("  AND AtribCod = 'BanderasLinea'");

            dtValorBanderaLinea = DSODataAccess.Execute(lsbQuery.ToString());
        }

        /*RZ.20130718 Se agregan metodos para establecer el modo alta de empleado*/
        private void EstablecerEstado(string lsestado)
        {
            if (lsestado == "edit")
            {
                /*Habilitar modo de edicion*/

                //20140602 AM. Mostrar la bandera de usuario pendiente 
                if (txtUsuarRedEmple.Enabled)
                {
                    chkUsuarioPendiente.Visible = true;
                }

                cargaBotonesEstadoEditEmple();

                /*RZ.20130708 Agregar un evento del lado del cliente para comprobar cuando el valor de
                 campo de usuario de red ha sido cambiado*/

                txtUsuarRedEmple.Attributes.Add("onchange", "TextBoxChange()");

                txtComenariosEmple.ReadOnly = true;
                txtComenariosEmple.Enabled = false;

                /*Se desabilitan botones para agregar CenCos y Puesto*/
                lbtnAgregarCenCos.Visible = false;
                lbtnAgregarCenCos.Enabled = false;

                lbtnAgregarPuesto.Visible = false;
                lbtnAgregarPuesto.Enabled = false;

                return;
            }

            if (lsestado == "ronly")
            {
                /*Habilitar modo de lectura*/
                readOnlyCCust();

                /*Se desabilitan botones para agregar CenCos y Puesto*/
                lbtnAgregarCenCos.Visible = false;
                lbtnAgregarCenCos.Enabled = false;

                lbtnAgregarPuesto.Visible = false;
                lbtnAgregarPuesto.Enabled = false;

                return;
            }

            /*RZ.20131106 Saber si el usuario es tipo empleado, cargar su carta custodia*/
            if (String.IsNullOrEmpty(lsestado) && Session["vchCodPerfil"].ToString() == "Epmpl")
            {
                //Obtener el icodcatalogo del empleado ligado al usuario.
                iCodCatalogoEmple = DALCCustodia.getiCodCatHist(Session["iCodUsuario"].ToString(), "Emple", "Empleados",
                    "Usuar", "iCodCatalogo");

                /*Habilitar modo de lectura*/
                readOnlyCCust();

                lbtnRegresarPagBusqExternaCCust.Visible = false;
                lbtnRegresarPagBusqExternaCCust.Enabled = false;
                imgbPDFExport.Visible = true;
                imgbPDFExport.Enabled = true;

                return;
            }

            if (lsestado == "alta" || String.IsNullOrEmpty(lsestado))
            {
                //20140602 AM. Mostrar la bandera de usuario pendiente 
                chkUsuarioPendiente.Visible = true;

                /*RZ.20130718 Habilitar modo para alta de empleado y ccustodia*/
                OcultarPanelesRecursos();

                HabilitarControlesAltaEmple();
                cargaBotonesEstadoAltaEmple();

                /*Asegurar que el campo de estado como alta para que
                 en los metodos posteriores se pueda validar de esa manera.
                 */
                estado = "alta";

                return;
            }

            if (lsestado == "roemp")
            {
                /*Habilitar modo de lectura para empleado*/
                readOnlyCCust();

                lbtnRegresarPagBusqExternaCCust.Visible = false;
                lbtnRegresarPagBusqExternaCCust.Enabled = false;
                tblEmpleCCust.Visible = true;
                txtComenariosEmple.Enabled = true;

                /*Se desabilitan botones para agregar CenCos y Puesto*/
                lbtnAgregarCenCos.Visible = false;
                lbtnAgregarCenCos.Enabled = false;

                lbtnAgregarPuesto.Visible = false;
                lbtnAgregarPuesto.Enabled = false;

                return;
            }
        }

        /*RZ.20130718 Habilitar los controles para la alta del empleado y el boton de guardar
         el empleado, ocultando los de edicion.*/
        private void HabilitarControlesAltaEmple()
        {
            enableAllWebControlsEmple();

            lbtnEditEmple.Enabled = false;
            lbtnEditEmple.Visible = false;
            lbtnSaveEmple.Enabled = true;
            lbtnSaveEmple.Visible = true;
            lbtnDeleteEmple.Enabled = false;
            lbtnDeleteEmple.Visible = false;

            tblEditDeleteEmpleC1.HorizontalAlign = HorizontalAlign.Center;
        }

        /*RZ.20130718 Sirve para ocultar los paneles de inventario, recursos y politicas
        durante la alta del empleado               
         */
        private void OcultarPanelesRecursos()
        {
            //20170620 Se comenta esta sección
            //pHeaderInventario.Visible = false;
            //pHeaderInventario.Enabled = false;
            //pDatosInventario.Visible = false;
            //pDatosInventario.Enabled = false;
            pHeaderCodAutoExten.Visible = false;
            pHeaderCodAutoExten.Enabled = false;
            pDatosCodAutoExten.Visible = false;
            pDatosCodAutoExten.Enabled = false;
            //20170620 Se comenta esta sección
            //pPoliticasUso.Visible = false;
            //pPoliticasUso.Enabled = false;
            //pContentPoliticas.Visible = false;
            //pContentPoliticas.Enabled = false;
            pComentarios.Visible = false;
            pComentarios.Enabled = false;

            tblFechasCC.Visible = false;
            tblFechasCC.Enabled = false;

            imgbPDFExport.Visible = false;
            lbtnRegresarPaginaBusq.Enabled = false;
            lbtnRegresarPaginaBusq.Visible = false;

        }

        private void readOnlyCCust()
        {
            /*Se desabilita el boton de volver a busqueda de CCustodia interna y se habilita el control para volver a busqueda externa*/
            lbtnRegresarPagBusqExternaCCust.Visible = true;
            lbtnRegresarPagBusqExternaCCust.Enabled = true;
            lbtnRegresarPaginaBusq.Visible = false;
            lbtnRegresarPaginaBusq.Enabled = false;

            /*Se desabilitan controles de seccion de empleado*/
            lbtnDeleteEmple.Visible = false;
            lbtnDeleteEmple.Enabled = false;
            lbtnEditEmple.Visible = false;
            lbtnEditEmple.Enabled = false;
            lbtnSaveEmple.Visible = false;
            lbtnSaveEmple.Enabled = false;

            /*Se desabilitan controles de seccion de inventario*/
            //20170620 Se Comenta Seccion de Inventario
            //grvInventario.Columns[7].Visible = false;
            //tblAddInventario.Visible = false; //20160311 NZ Se oculta contenedor de los controles para agregar elementos al inventario.

            /*Se desabilitan controles de seccion de Codigos*/
            grvCodAuto.Columns[9].Visible = false; //20160311 Columna Editar
            grvCodAuto.Columns[10].Visible = false; //20160311 Columna Borrar
            pnlAltaDeCodigosAut.Visible = false;  //20160311 NZ Se oculta contenedor de los controles para agregar codigos          

            /*Se desabilitan controles de seccion de Extensiones*/
            grvExten.Columns[11].Visible = false;
            grvExten.Columns[12].Visible = false;
            pnlAltaDeExtensiones.Visible = false;  //20160311 NZ Se oculta contenedor de los controles para agregar extensiones.

            grvLinea.Columns[9].Visible = false;
            grvLinea.Columns[10].Visible = false;
            pnlAltaDeLinea.Visible = false;

            /*Se desabilita seccion de comentarios*/
            txtComentariosAdmin.Enabled = false;
            txtComenariosEmple.Enabled = false;

            /*Se desabilita seccion fechas*/
            tblFechasCC.Visible = false;
            tblFechasCC.Enabled = false;

            /*Se desabilita boton para exportar a PDF*/
            imgbPDFExport.Visible = false;
            imgbPDFExport.Enabled = false;

        }

        private void FillCodAutoGrid()
        {
            psQuery.Length = 0;


            psQuery.AppendLine("Declare @query varchar(max) = ''																			   ");
            psQuery.AppendLine("Declare @iCodCatUsuario int =" + Session["iCodUsuario"] + "																			   ");
            psQuery.AppendLine("----------------------------------------------------------------------------------------------  			   ");
            psQuery.AppendLine("/* OBTENER EL VALOR DE LAS BANDERAS DE OMITIR RESTRICCIONES DE EMPLE, CENCOS Y SITIO */		    			   ");
            psQuery.AppendLine("DECLARE @nQueryOmitirRestricciones NVARCHAR(MAX) = '';										    			   ");
            psQuery.AppendLine("DECLARE @omitirRestSitio bit = 0																			   ");
            psQuery.AppendLine("DECLARE @omitirRestCenCos bit = 0																			   ");
            psQuery.AppendLine("DECLARE @omitirRestEmple bit = 0																			   ");
            psQuery.AppendLine("																											   ");
            psQuery.AppendLine("SET @nQueryOmitirRestricciones = '															    			   ");
            psQuery.AppendLine("		SELECT @omitirRestSitio = (isnull(banderasusuar,0) & 1)/1,											   ");
            psQuery.AppendLine("				@omitirRestCenCos = (isnull(banderasusuar,0) & 2)/2,										   ");
            psQuery.AppendLine("				@omitirRestEmple = (isnull(banderasusuar,0) & 4)/4											   ");
            psQuery.AppendLine("		FROM Keytia5.[" + DSODataContext.Schema + "].vTIUsuarios  WITH(NOLOCK)															   ");
            psQuery.AppendLine("		WHERE dtFinVigencia >= GETDATE()																	   ");
            psQuery.AppendLine("		AND iCodCatalogo = ' + Convert(VARCHAR, ISNULL(@iCodCatUsuario,0))									   ");
            psQuery.AppendLine("																											   ");
            psQuery.AppendLine("EXEC sp_executesql 																			    			   ");
            psQuery.AppendLine("	@nQueryOmitirRestricciones, 																			   ");
            psQuery.AppendLine("	N'@omitirRestSitio INT OUTPUT,																			   ");
            psQuery.AppendLine("	@omitirRestCenCos INT OUTPUT, 																			   ");
            psQuery.AppendLine("	@omitirRestEmple INT OUTPUT ',																			   ");
            psQuery.AppendLine("	@omitirRestSitio OUTPUT,																				   ");
            psQuery.AppendLine("	@omitirRestCenCos OUTPUT,																				   ");
            psQuery.AppendLine("	@omitirRestEmple OUTPUT																					   ");
            psQuery.AppendLine("----------------------------------------------------------------------------------------------				   ");
            psQuery.AppendLine("																											   ");
            psQuery.AppendLine("																											   ");
            psQuery.AppendLine("																											   ");
            psQuery.AppendLine("																											   ");
            psQuery.AppendLine("Set @query = 																								   ");
            psQuery.AppendLine("'																											   ");
            psQuery.AppendLine("	SELECT CodAuto = CodAuto.iCodCatalogo, CodAutoCod = CodAuto.vchCodigo, Sitio = CodAuto.Sitio,			   ");
            psQuery.AppendLine("	SitioDesc = CodAuto.SitioDesc, Cos = CodAuto.Cos, CosDesc = CodAuto.CosDesc, FechaIni = Rel.dtinivigencia, ");
            psQuery.AppendLine("	FechaFin = Rel.dtFinVigencia,iCodRegRelEmpCodAuto = Rel.iCodRegistro					");
            psQuery.AppendLine("	FROM [VisRelaciones(''Empleado - CodAutorizacion'',''Español'')] Rel WITH(NOLOCK)					");
            psQuery.AppendLine("	INNER JOIN [VisHistoricos(''CodAuto'',''Codigo Autorizacion'',''Español'')] CodAuto WITH(NOLOCK)		");
            psQuery.AppendLine("		ON Rel.CodAuto = CodAuto.iCodCatalogo												");
            psQuery.AppendLine("		AND Rel.dtinivigencia <> Rel.dtfinvigencia											");
            psQuery.AppendLine("		AND Rel.dtfinvigencia >= GETDATE()													");
            psQuery.AppendLine("		AND CodAuto.dtinivigencia <> CodAuto.dtfinvigencia									");
            psQuery.AppendLine("		AND CodAuto.dtfinvigencia >= GETDATE()												");
            psQuery.AppendLine("	'																						");
            psQuery.AppendLine("																							");
            psQuery.AppendLine("																							");

            if (NuevoEmpleadoBackend.BuscaBanderaValidaRestricciones(int.Parse(Session["iCodUsuario"].ToString())))
            {


                psQuery.AppendLine("	set @query= @query + 																	");
                psQuery.AppendLine("				'																			");
                psQuery.AppendLine("				Inner Join [" + DSODataContext.Schema + "].[VisHisComun(''Sitio'',''Español'')] SitioComun WITH(NOLOCK)			");
                psQuery.AppendLine("					On CodAuto.Sitio = SitioComun.iCodCatalogo								");
                psQuery.AppendLine("					And SitioComun.dtIniVigencia <> SitioComun.dtFinVigencia				");
                psQuery.AppendLine("					And SitioComun.dtFinVigencia >= GETDATE()								");
                psQuery.AppendLine("				Inner JOIN [" + DSODataContext.Schema + "].RestSitio RestSitio	WITH(NOLOCK)									");
                psQuery.AppendLine("					ON sitioComun.iCodCatalogo = RestSitio.Sitio							");
                psQuery.AppendLine("					And sitioComun.dtIniVigencia <= RestSitio.FechaInicio					");
                psQuery.AppendLine("					And sitioComun.dtFinVigencia >= RestSitio.Fechafin						");
                psQuery.AppendLine("					And RestSitio.Usuar = '+Convert(varchar,@iCodCatUsuario)+'									");
                psQuery.AppendLine("				'																			");

            }
            psQuery.AppendLine("																							");
            psQuery.AppendLine("	set @query = @query + 																	");
            psQuery.AppendLine("	'																						");
            psQuery.AppendLine("	WHERE Rel.Emple = " + iCodCatalogoEmple + "																");
            psQuery.AppendLine("'																							");
            psQuery.AppendLine("																							");

            psQuery.AppendLine("  exec(@query)");

            dtCodAuto = DSODataAccess.Execute(psQuery.ToString());



            grvCodAuto.DataSource = dtCodAuto;
            grvCodAuto.DataBind();


            upDatosCodAutoExten2.Update();
        }

        private void FillExtenGrid()
        {
            psQuery.Length = 0;

            //psQuery.AppendLine("SELECT Exten = Exten.iCodCatalogo, ExtenCod = Exten.vchCodigo, Sitio = Exten.Sitio, SitioDesc = Exten.SitioDesc, Cos = Exten.Cos, CosDesc = Exten.CosDesc, FechaIni = Rel.dtinivigencia, FechaFin = Rel.dtFinVigencia,");
            //// 20140115 AM. Se agrega condicion a consulta para regresar un 0 cuando el campo ExtenB.TipoRecurso sea null
            ////psQuery.Append("TipoExten = ExtenB.TipoRecurso, TipoExtenDesc = isnull(ExtenB.TipoRecursoDesc,0), VisibleDir = CONVERT(bit,ISNULL(Exten.BanderasExtens,0)), ComentarioExten = isnull(ExtenB.Comentarios,'') \r");
            //psQuery.AppendLine("TipoExten = ISNULL(ExtenB.TipoRecurso, 0), TipoExtenDesc = isnull(ExtenB.TipoRecursoDesc,0), VisibleDir = CONVERT(bit,ISNULL(Exten.BanderasExtens,0)), ComentarioExten = isnull(ExtenB.Comentarios,'') ");
            //psQuery.AppendLine(",iCodRegRelEmpExt = Rel.iCodRegistro,");   //AM 20130717  Se agrega campo para obtener iCodRegistro de la Relacion
            //psQuery.AppendLine("permitido = Case When (RestSitio.iCodCatalogo is not null) then  'Permitido' else 'No Permitido' End");
            //psQuery.AppendLine("FROM [VisRelaciones('Empleado - Extension','Español')] Rel");
            //psQuery.AppendLine("INNER JOIN [VisHistoricos('Exten','Extensiones','Español')] Exten");
            //psQuery.AppendLine("    ON Rel.Exten = Exten.iCodCatalogo");
            //psQuery.AppendLine("    AND Rel.dtinivigencia <> Rel.dtfinvigencia");
            //psQuery.AppendLine("    AND Rel.dtfinvigencia >= GETDATE()");
            //psQuery.AppendLine("    AND Exten.dtinivigencia <> Exten.dtfinvigencia");
            //psQuery.AppendLine("    AND Exten.dtfinvigencia >= GETDATE()");
            //psQuery.AppendLine("LEFT OUTER JOIN [VisHistoricos('ExtenB','Extensiones B','Español')] ExtenB");
            //psQuery.AppendLine("    ON Exten.iCodCatalogo = ExtenB.Exten");
            //psQuery.AppendLine("    AND ExtenB.dtIniVigencia <> ExtenB.dtFinVigencia");
            //psQuery.AppendLine("    AND ExtenB.dtFinVigencia >= GETDATE()");
            //psQuery.AppendLine("WHERE Rel.Emple = " + iCodCatalogoEmple);


            psQuery.AppendLine("Declare @query varchar(max) = ''																	");
            psQuery.AppendLine("Declare @iCodCatUsuario int =" + Session["iCodUsuario"] + "																		");
            psQuery.AppendLine("----------------------------------------------------------------------------------------------  	");
            psQuery.AppendLine("/* OBTENER EL VALOR DE LAS BANDERAS DE OMITIR RESTRICCIONES DE EMPLE, CENCOS Y SITIO */		    	");
            psQuery.AppendLine("DECLARE @nQueryOmitirRestricciones NVARCHAR(MAX) = '';										    	");
            psQuery.AppendLine("DECLARE @omitirRestSitio bit = 0																	");
            psQuery.AppendLine("DECLARE @omitirRestCenCos bit = 0																	");
            psQuery.AppendLine("DECLARE @omitirRestEmple bit = 0																	");
            psQuery.AppendLine("																									");
            psQuery.AppendLine("SET @nQueryOmitirRestricciones = '															    	");
            psQuery.AppendLine("		SELECT @omitirRestSitio = (isnull(banderasusuar,0) & 1)/1,									");
            psQuery.AppendLine("				@omitirRestCenCos = (isnull(banderasusuar,0) & 2)/2,								");
            psQuery.AppendLine("				@omitirRestEmple = (isnull(banderasusuar,0) & 4)/4									");
            psQuery.AppendLine("		FROM Keytia5.[" + DSODataContext.Schema + "].vTIUsuarios  WITH(NOLOCK)														");
            psQuery.AppendLine("		WHERE dtFinVigencia >= GETDATE()															");
            psQuery.AppendLine("		AND iCodCatalogo = ' + Convert(VARCHAR, ISNULL(@iCodCatUsuario,0))									");
            psQuery.AppendLine("																									");
            psQuery.AppendLine("EXEC sp_executesql 																			    	");
            psQuery.AppendLine("	@nQueryOmitirRestricciones, 																	");
            psQuery.AppendLine("	N'@omitirRestSitio INT OUTPUT,																	");
            psQuery.AppendLine("	@omitirRestCenCos INT OUTPUT, 																	");
            psQuery.AppendLine("	@omitirRestEmple INT OUTPUT ',																	");
            psQuery.AppendLine("	@omitirRestSitio OUTPUT,																		");
            psQuery.AppendLine("	@omitirRestCenCos OUTPUT,																		");
            psQuery.AppendLine("	@omitirRestEmple OUTPUT																			");
            psQuery.AppendLine("----------------------------------------------------------------------------------------------		");
            psQuery.AppendLine("																									");
            psQuery.AppendLine("																									");
            psQuery.AppendLine("Set @query = 																						");
            psQuery.AppendLine("'																									");
            psQuery.AppendLine("SELECT 																								");
            psQuery.AppendLine("	Exten = Exten.iCodCatalogo,																		");
            psQuery.AppendLine("	ExtenCod = Exten.vchCodigo, 																	");
            psQuery.AppendLine("	Sitio = Exten.Sitio,																			");
            psQuery.AppendLine("	SitioDesc = Exten.SitioDesc,																	");
            psQuery.AppendLine("	Cos = Exten.Cos,																				");
            psQuery.AppendLine("	CosDesc = Exten.CosDesc, 																		");
            psQuery.AppendLine("	FechaIni = Rel.dtinivigencia,																	");
            psQuery.AppendLine("	FechaFin = Rel.dtFinVigencia,																	");
            psQuery.AppendLine("	TipoExten = ISNULL(ExtenB.TipoRecurso, 0),														");
            psQuery.AppendLine("	TipoExtenDesc = isnull(ExtenB.TipoRecursoDesc,0),												");
            psQuery.AppendLine("	VisibleDir = CONVERT(bit,ISNULL(Exten.BanderasExtens,0)),										");
            psQuery.AppendLine("	ComentarioExten = isnull(ExtenB.Comentarios,'''') 												");
            psQuery.AppendLine("	,iCodRegRelEmpExt = Rel.iCodRegistro															");
            psQuery.AppendLine("	'																								");
            psQuery.AppendLine("																									");
            psQuery.AppendLine("if (@iCodCatUsuario is not null And @iCodCatUsuario > 0)											");
            psQuery.AppendLine("Begin																								");
            psQuery.AppendLine("	if	@omitirRestSitio = 0																		");
            psQuery.AppendLine("	Begin																							");
            psQuery.AppendLine("		set @query = @query +																		");
            psQuery.AppendLine("		'																							");
            psQuery.AppendLine("			,permitido = Case When (RestSitio.iCodCatalogo is not null) then  ''Si'' else ''No'' End	");
            psQuery.AppendLine("		'																					");
            psQuery.AppendLine("	End																						");
            psQuery.AppendLine("	Else																					");
            psQuery.AppendLine("	Begin																					");
            psQuery.AppendLine("	set @query = @query +																	");
            psQuery.AppendLine("		'																					");
            psQuery.AppendLine("			,permitido = ''Si''																");
            psQuery.AppendLine("		'																					");
            psQuery.AppendLine("	End 																					");
            psQuery.AppendLine("End																							");
            psQuery.AppendLine("																							");
            psQuery.AppendLine("Set @query = @query +																		");
            psQuery.AppendLine("'																							");
            psQuery.AppendLine("FROM [" + DSODataContext.Schema + "].[VisRelaciones(''Empleado - Extension'',''Español'')] Rel	WITH(NOLOCK)					");
            psQuery.AppendLine("INNER JOIN [" + DSODataContext.Schema + "].[VisHistoricos(''Exten'',''Extensiones'',''Español'')] Exten	WITH(NOLOCK)		");
            psQuery.AppendLine("    ON Rel.Exten = Exten.iCodCatalogo														");
            psQuery.AppendLine("    AND Rel.dtinivigencia <> Rel.dtfinvigencia												");
            psQuery.AppendLine("    AND Rel.dtfinvigencia >= GETDATE()														");
            psQuery.AppendLine("    AND Exten.dtinivigencia <> Exten.dtfinvigencia											");
            psQuery.AppendLine("    AND Exten.dtfinvigencia >= GETDATE()													");
            psQuery.AppendLine("'																							");
            psQuery.AppendLine("																							");
            psQuery.AppendLine("																							");
            psQuery.AppendLine("																							");
            //if (NuevoEmpleadoBackend.BuscaBanderaValidaRestricciones(int.Parse(Session["iCodUsuario"].ToString())))
            //{


                psQuery.AppendLine("	set @query= @query + 																	");
                psQuery.AppendLine("				'																			");
                psQuery.AppendLine("				Inner Join [" + DSODataContext.Schema + "].[VisHisComun(''Sitio'',''Español'')] SitioComun	WITH(NOLOCK)		");
                psQuery.AppendLine("					On Exten.Sitio = SitioComun.iCodCatalogo								");
                psQuery.AppendLine("					And SitioComun.dtIniVigencia <> SitioComun.dtFinVigencia				");
                psQuery.AppendLine("					And SitioComun.dtFinVigencia >= GETDATE()								");
                psQuery.AppendLine("				Inner JOIN [" + DSODataContext.Schema + "].RestSitio RestSitio	WITH(NOLOCK)									");
                psQuery.AppendLine("					ON sitioComun.iCodCatalogo = RestSitio.Sitio							");
                psQuery.AppendLine("					And sitioComun.dtIniVigencia <= RestSitio.FechaInicio					");
                psQuery.AppendLine("					And sitioComun.dtFinVigencia >= RestSitio.Fechafin						");
                psQuery.AppendLine("					And RestSitio.Usuar = '+Convert(varchar,@iCodCatUsuario)+'											");
                psQuery.AppendLine("				'																			");

            //}
            psQuery.AppendLine("																							");
            psQuery.AppendLine("Set @query = @query +																		");
            psQuery.AppendLine("'																							");
            psQuery.AppendLine("LEFT OUTER JOIN [" + DSODataContext.Schema + "].[VisHistoricos(''ExtenB'',''Extensiones B'',''Español'')] ExtenB	WITH(NOLOCK)	");
            psQuery.AppendLine("    ON Exten.iCodCatalogo = ExtenB.Exten													");
            psQuery.AppendLine("    AND ExtenB.dtIniVigencia <> ExtenB.dtFinVigencia										");
            psQuery.AppendLine("    AND ExtenB.dtFinVigencia >= GETDATE()													");
            psQuery.AppendLine("WHERE Rel.Emple = " + iCodCatalogoEmple + "																	");
            psQuery.AppendLine("'																							");
            psQuery.AppendLine("exec(@query)																				");
            psQuery.AppendLine("--print(@query)																				");


            dtExtensiones = DSODataAccess.Execute(psQuery.ToString());

            grvExten.DataSource = dtExtensiones;
            grvExten.DataBind();




            //foreach (GridViewRow row in grvExten.Rows)
            //{
            //    bool permitido = false;
            //    string exten =grvExten.Rows[row.RowIndex].Cells[0].Text;
            //    ControlCollection edit = row.Cells[13].Controls;
            //    ControlCollection delete = row.Cells[14].Controls;

            //    foreach (DataRow dataRow in dtExtensiones.Rows)
            //    {

            //        string DataRowExtensions = dataRow["ExtenCod"].ToString().ToLower();
            //        string DataRowpermit = dataRow["permitido"].ToString().ToLower();
            //        if (
            //                DataRowExtensions== exten.ToLower() &&
            //                DataRowpermit == "si"
            //            )
            //        {
            //            permitido = true;
            //        }
            //    }

            //    if (!permitido)
            //    {
            //        foreach (Control  c in edit)
            //        {
            //            if (c is ImageButton)
            //            {
            //                ImageButton i = (ImageButton)c;
            //                i.Enabled = false;
            //            }

            //        }

            //        foreach (Control c in delete)
            //        {
            //            if (c is ImageButton)
            //            {
            //                ImageButton i = (ImageButton)c;
            //                i.Enabled = false;
            //            }
            //        }
            //    }
            //}
        }

        //20170620 NZ Se Comenta esta sección.
        //private void FillInventarioGrid()
        //{
        //    psQuery.Length = 0;

        //    psQuery.AppendLine("SELECT iCodMarca = Disp.MarcaDisp, Marca = Disp.MarcaDispDesc, iCodModelo = Disp.ModeloDisp, Modelo = Disp.ModeloDíspDesc, ");
        //    psQuery.AppendLine("TipoAparato = Disp.TipoDispositivoDesc, NoSerie = Disp.NSerie, MacAddress = Disp.MacAddress");
        //    psQuery.AppendLine("FROM [VisRelaciones('Dispositivo - Empleado','Español')] Rel");
        //    psQuery.AppendLine("INNER JOIN [VisHistoricos('Dispositivo','Inventario de dispositivos','Español')] Disp ");
        //    psQuery.AppendLine("    ON Rel.Dispositivo = Disp.iCodCatalogo");
        //    psQuery.AppendLine("    AND Rel.dtinivigencia <> Rel.dtfinvigencia ");
        //    psQuery.AppendLine("    AND Rel.dtfinvigencia >= GETDATE() ");
        //    psQuery.AppendLine("    AND Disp.dtinivigencia <> Disp.dtfinvigencia ");
        //    psQuery.AppendLine("    AND Disp.dtfinvigencia >= GETDATE() ");
        //    psQuery.AppendLine("WHERE Rel.Emple = " + iCodCatalogoEmple);
        //    dtInventarioAsignado = DSODataAccess.Execute(psQuery.ToString());

        //    grvInventario.DataSource = dtInventarioAsignado;
        //    grvInventario.DataBind();            

        //    upDatosInventario.Update();
        //}

        protected void FillDatosEmple(DataTable ldsEmple)
        {
            DataRow ldrEmple = ldsEmple.Rows[0];
            DateTime ldtFechaInicio = new DateTime();
            ldtFechaInicio = Convert.ToDateTime(ldrEmple["FechaInicio"].ToString());

            txtFecha.Text = ldtFechaInicio.ToString("dd/MM/yyyy");
            hdnFechaFinEmple.Value = ldrEmple["FechaFin"].ToString();
            txtFolioCCustodia.Text = ldrEmple["NoFolio"].ToString();
            ceSelectorFecha1.SelectedDate = ldtFechaInicio;
            txtEstatusCCustodia.Text = ldrEmple["Estatus"].ToString();
            txtNominaEmple.Text = ldrEmple["Empleado"].ToString();
            txtNombreEmple.Text = ldrEmple["Nombre"].ToString();
            txtSegundoNombreEmple.Text = ldrEmple["SegundoNombre"].ToString();
            txtApPaternoEmple.Text = ldrEmple["ApPaterno"].ToString();
            txtApMaternoEmple.Text = ldrEmple["ApMaterno"].ToString();
            drpCenCosEmple.SelectedValue = ldrEmple["CenCos"].ToString();
            drpPuestoEmple.SelectedValue = ldrEmple["Puesto"].ToString();
            //txtArea.Text = ldrEmple["Area"].ToString();
            txtPrepMovil.Text = ldrEmple["PresMovil"].ToString();
            txtPrepFija.Text = ldrEmple["PresFija"].ToString();
            txtPrepTemporal.Text = ldrEmple["PresTemp"].ToString();

            txtLocalidadEmple.Text = ldrEmple["Locali"].ToString();
            if (ldrEmple["Localidad"] != DBNull.Value && !string.IsNullOrEmpty(ldrEmple["Localidad"].ToString()))
            {
                drpLocalidadEmple.SelectedValue = ldrEmple["Localidad"].ToString();
            }
            txtEmailEmple.Text = ldrEmple["Email"].ToString();
            txtUsuarRedEmple.Text = ldrEmple["UsuarioRed"].ToString();
            if (ldrEmple["Gerente"].ToString() == "1")
            {
                cbEsGerenteEmple.Checked = true;
            }
            if (ldrEmple["VisibleDir"].ToString() == "1")
            {
                cbVisibleDirEmple.Checked = true;
            }
            //20161108 NZ Se agregan campos nuevos.
            if (ldrEmple["EmpleOmiteDeSincro"].ToString() == "1")
            {
                ckbOmiteSincro.Checked = true;
                txtComentariosSincro.Text = ldrEmple["ComentarioSincro"].ToString();
                txtComentariosSincro.Visible = true;
            }
            else { txtComentariosSincro.Visible = false; }

            //drpJefeEmple.SelectedValue = ldrEmple["JefeInmediato"].ToString();
            txtJefeEmple.Text = ldrEmple["NombreJefeEmple"].ToString();
            txtEmailJefeEmple.Text = ldrEmple["EmailJefe"].ToString();
            hdfJefeEmple.Value = ldrEmple["JefeInmediato"].ToString();
            if (ldrEmple["Ubicacion"] != DBNull.Value && !string.IsNullOrEmpty(ldrEmple["Ubicacion"].ToString()))
            {
                drpSitioEmple.SelectedValue = ldrEmple["Ubicacion"].ToString();
            }
            drpTipoEmpleado.SelectedValue = ldrEmple["TipoEmple"].ToString();
            drpEmpresaEmple.SelectedValue = ldrEmple["EmpreEmple"].ToString();

            txtComenariosEmple.Text = ldrEmple["ComentariosEmple"].ToString();

            if (KeytiaServiceBL.Util.Decrypt(Request.QueryString["st"]) == "roemp")
            {
                txtComenariosEmple.Enabled = true;
            }
            else
            {
                txtComenariosEmple.Enabled = false;
            }

            txtComentariosAdmin.Text = ldrEmple["ComentariosAdmin"].ToString();
            txtUltimaMod.Text = ldrEmple["FecUltModificacion"].ToString();

            txtUltimoEnvio.Text = consultaFechaEnvio();

            //RM20190510 Campos Datos Emple Solo FCA
            if (DSODataContext.Schema.ToUpper() == "FCA")
            {
                if (ldrEmple.Table.Columns.Contains("DC_id") && ldrEmple["DC_id"] != null)
                {
                    txtDatosEmpleFCADC_ID.Text = ldrEmple["DC_id"].ToString();

                }

                if (ldrEmple.Table.Columns.Contains("T_id") && ldrEmple["T_id"] != null)
                {
                    txtDatosEmpleFCAT_ID.Text = ldrEmple["T_id"].ToString();
                    Session["tIdEmple"] = ldrEmple["T_id"].ToString();
                    Session["T_id"] = ldrEmple["T_id"].ToString().ToString().Trim();
                }

                if (ldrEmple.Table.Columns.Contains("NickName") && ldrEmple["NickName"] != null)
                {
                    txtDatosEmpleFCANickName.Text = ldrEmple["NickName"].ToString();
                }

                if (ldrEmple.Table.Columns.Contains("estacion") && ldrEmple["estacion"] != null)
                {
                    txtDatosEmpleFCAEstacion.Text = ldrEmple["estacion"].ToString();
                }

                if (ldrEmple.Table.Columns.Contains("director") && ldrEmple["director"] != null && Convert.ToInt32(ldrEmple["director"].ToString()) > 0)
                {
                    ddlDatosEmpleFCADirector.SelectedValue = ldrEmple["director"].ToString();
                }

                if (ldrEmple.Table.Columns.Contains("esDirector") && ldrEmple["esDirector"] != null)
                {
                    cbDatosEmpleFCAEsDirector.Checked = ldrEmple["esDirector"].ToString() == "0" ? false : true;
                }


                if (ldrEmple.Table.Columns.Contains("Planta") && ldrEmple["Planta"] != null)
                {
                    ddlDatosEmpleFCAPlanta.SelectedValue = ldrEmple["Planta"].ToString();
                }

                if (ldrEmple.Table.Columns.Contains("Depto") && ldrEmple["Depto"] != null)
                {
                    ddlDatosEmpleFCADpto.SelectedValue = ldrEmple["Depto"].ToString();
                }
            }

        }

        protected void FillLineaGrid()
        {
            psQuery.Length = 0;
            //psQuery.AppendLine("SELECT Linea = Linea.iCodCatalogo, LineaCod = Linea.vchCodigo, ");
            //psQuery.AppendLine("		Carrier = Linea.Carrier, CarrierDesc = Linea.CarrierDesc,");
            //psQuery.AppendLine("		Sitio = Linea.Sitio, SitioDesc = Linea.SitioDesc,");
            //psQuery.AppendLine("		FechaIni = Rel.dtinivigencia,");
            //psQuery.AppendLine("		FechaFin = Rel.dtFinVigencia,iCodRegRelEmpLinea = Rel.iCodRegistro");
            //psQuery.AppendLine("FROM [VisRelaciones('Empleado - Linea','Español')] Rel");
            //psQuery.AppendLine("INNER JOIN [VisHistoricos('Linea','Lineas','Español')] Linea");
            //psQuery.AppendLine("    ON Rel.Linea = Linea.iCodCatalogo");
            //psQuery.AppendLine("    AND Rel.dtinivigencia <> Rel.dtFinvigencia");
            //psQuery.AppendLine("    AND Rel.dtfinvigencia >= GETDATE()");
            //psQuery.AppendLine("    AND Linea.dtinivigencia <> Linea.dtFinvigencia");
            //psQuery.AppendLine("    AND Linea.dtfinvigencia >= GETDATE()");
            //psQuery.AppendLine("WHERE Rel.Emple = " + iCodCatalogoEmple);

            //RM 20190403 se cambia select para que busque tambien en lineas FCA Chrysler
            StringBuilder selectCamposFCA = new StringBuilder();

            selectCamposFCA.AppendLine("	Linea = Linea.iCodCatalogo,													");
            selectCamposFCA.AppendLine("	LineaCod = Linea.vchCodigo, 												");
            selectCamposFCA.AppendLine("	Carrier = Linea.Carrier,													");
            selectCamposFCA.AppendLine("	CarrierDesc = Linea.CarrierDesc,											");
            selectCamposFCA.AppendLine("	Sitio = Linea.Sitio,														");
            selectCamposFCA.AppendLine("	SitioDesc = Linea.SitioDesc,												");
            selectCamposFCA.AppendLine("	FechaIni = Rel.dtinivigencia,												");
            selectCamposFCA.AppendLine("	FechaFin = Rel.dtFinVigencia,												");
            selectCamposFCA.AppendLine("	iCodRegRelEmpLinea = Rel.iCodRegistro,										");
            selectCamposFCA.AppendLine("	PentaSAPCCDescription = 	isNull(lineaFCA.PentaSAPCCDescription,''''),      ");
            selectCamposFCA.AppendLine("	PentaSAPCCDescriptionDesc=	isNull(lineaFCA.PentaSAPCCDescriptionDesc,''''),	");
            selectCamposFCA.AppendLine("	PentaSAPAccount= isNull(lineaFCA.PentaSAPAccount,''''),						");
            selectCamposFCA.AppendLine("	PentaSAPAccountDesc = isNull(lineaFCA.PentaSAPAccountDesc,'''')	,			");
            selectCamposFCA.AppendLine("	PentaSAPProfitCenter= isNull(lineaFCA.PentaSAPProfitCenter,''''),				");
            selectCamposFCA.AppendLine("	PentaSAPProfitCenterDesc= isNull(lineaFCA.PentaSAPProfitCenterDesc,''''),		");
            selectCamposFCA.AppendLine("	PentaSAPCostCenter= isNull(lineaFCA.PentaSAPCostCenter,''''),					");
            selectCamposFCA.AppendLine("	PentaSAPCostCenterDesc = isNull(lineaFCA.PentaSAPCostCenterDesc,'''')	,		");
            selectCamposFCA.AppendLine("	PentaSAPFA = isNull(lineaFCA.PentaSAPFA,'''')	,								");
            selectCamposFCA.AppendLine("	PentaSAPFADesc = isNull(lineaFCA.PentaSAPFADesc,'''')           				");


            StringBuilder selectCamposNOFCA = new StringBuilder();
            selectCamposNOFCA.AppendLine("	Linea = Linea.iCodCatalogo,					");
            selectCamposNOFCA.AppendLine("	LineaCod = Linea.vchCodigo, 				");
            selectCamposNOFCA.AppendLine("	Carrier = Linea.Carrier,					");
            selectCamposNOFCA.AppendLine("	CarrierDesc = Linea.CarrierDesc,			");
            selectCamposNOFCA.AppendLine("	Sitio = Linea.Sitio,						");
            selectCamposNOFCA.AppendLine("	SitioDesc = Linea.SitioDesc,				");
            selectCamposNOFCA.AppendLine("	FechaIni = Rel.dtinivigencia,				");
            selectCamposNOFCA.AppendLine("	FechaFin = Rel.dtFinVigencia,				");
            selectCamposNOFCA.AppendLine("	iCodRegRelEmpLinea = Rel.iCodRegistro,		");
            selectCamposNOFCA.AppendLine("	PentaSAPCCDescription = 	'''',				");
            selectCamposNOFCA.AppendLine("	PentaSAPCCDescriptionDesc=	'''', 			");
            selectCamposNOFCA.AppendLine("	PentaSAPAccount= '''',						");
            selectCamposNOFCA.AppendLine("	PentaSAPAccountDesc = ''''	,				");
            selectCamposNOFCA.AppendLine("	PentaSAPProfitCenter= '''',					");
            selectCamposNOFCA.AppendLine("	PentaSAPProfitCenterDesc= '''',				");
            selectCamposNOFCA.AppendLine("	PentaSAPCostCenter= '''',						");
            selectCamposNOFCA.AppendLine("	PentaSAPCostCenterDesc = ''''	,				");
            selectCamposNOFCA.AppendLine("	PentaSAPFA = ''''	,							");
            selectCamposNOFCA.AppendLine("	PentaSAPFADesc = ''''							");


            StringBuilder LeftLineaFCa = new StringBuilder();
            LeftLineaFCa.AppendLine("Left Join [visHistoricos(''lineaFCA'',''lineas FCA'',''Español'')] lineaFCA			");
            LeftLineaFCa.AppendLine("	On lineaFCA.linea = linea.iCodCatalogo										");
            LeftLineaFCa.AppendLine("	And lineaFCA.dtinivigencia <> lineaFCA.dtfinvigencia 						");
            LeftLineaFCa.AppendLine("	And lineaFCA.dtFinVigencia >= GETDATE()										");



            string select = string.Empty;
            string left = string.Empty;

            if (DSODataContext.Schema.ToUpper() == "FCA")
            {
                select = selectCamposFCA.ToString();
                left = LeftLineaFCa.ToString();
            }
            else
            {
                select = selectCamposNOFCA.ToString();
                left = "";
            }

            psQuery.AppendLine("Declare @query varchar(max) = ''																			  ");
            psQuery.AppendLine("Declare @iCodCatUsuario int =" + Session["iCodUsuario"] + "																			  ");
            psQuery.AppendLine("----------------------------------------------------------------------------------------------  			  ");
            psQuery.AppendLine("/* OBTENER EL VALOR DE LAS BANDERAS DE OMITIR RESTRICCIONES DE EMPLE, CENCOS Y SITIO */		    			  ");
            psQuery.AppendLine("DECLARE @nQueryOmitirRestricciones NVARCHAR(MAX) = '';										    			  ");
            psQuery.AppendLine("DECLARE @omitirRestSitio bit = 0																			  ");
            psQuery.AppendLine("DECLARE @omitirRestCenCos bit = 0																			  ");
            psQuery.AppendLine("DECLARE @omitirRestEmple bit = 0																			  ");
            psQuery.AppendLine("																											  ");
            psQuery.AppendLine("SET @nQueryOmitirRestricciones = '															    			  ");
            psQuery.AppendLine("		SELECT @omitirRestSitio = (isnull(banderasusuar,0) & 1)/1,											  ");
            psQuery.AppendLine("				@omitirRestCenCos = (isnull(banderasusuar,0) & 2)/2,										  ");
            psQuery.AppendLine("				@omitirRestEmple = (isnull(banderasusuar,0) & 4)/4											  ");
            psQuery.AppendLine("		FROM Keytia5.[" + DSODataContext.Schema + "].vTIUsuarios  WITH(NOLOCK)																  ");
            psQuery.AppendLine("		WHERE dtFinVigencia >= GETDATE()																	  ");
            psQuery.AppendLine("		AND iCodCatalogo = ' + Convert(VARCHAR, ISNULL(@iCodCatUsuario,0))									  ");
            psQuery.AppendLine("																											  ");
            psQuery.AppendLine("EXEC sp_executesql 																			    			  ");
            psQuery.AppendLine("	@nQueryOmitirRestricciones, 																			  ");
            psQuery.AppendLine("	N'@omitirRestSitio INT OUTPUT,																			  ");
            psQuery.AppendLine("	@omitirRestCenCos INT OUTPUT, 																			  ");
            psQuery.AppendLine("	@omitirRestEmple INT OUTPUT ',																			  ");
            psQuery.AppendLine("	@omitirRestSitio OUTPUT,																				  ");
            psQuery.AppendLine("	@omitirRestCenCos OUTPUT,																				  ");
            psQuery.AppendLine("	@omitirRestEmple OUTPUT																					  ");
            psQuery.AppendLine("----------------------------------------------------------------------------------------------				  ");
            psQuery.AppendLine("");


            psQuery.AppendLine("set @query  = '");
            psQuery.AppendLine("Select 																			");
            psQuery.AppendLine(select);
            psQuery.AppendLine("From vEmpleado Emple															");
            psQuery.AppendLine("Inner Join [visRelaciones(''empleado - linea'',''español'')] rel WITH(NOLOCK)				");
            psQuery.AppendLine("	On rel.emple = emple.icodcatalogo											");
            psQuery.AppendLine("	and rel.dtinivigencia <> rel.dtfinvigencia									");
            psQuery.AppendLine("	and rel.dtfinvigencia >= getdate()											");
            psQuery.AppendLine("Inner Join [visHistoricos(''linea'',''lineas'',''español'')] linea	WITH(NOLOCK)			");
            psQuery.AppendLine("	On rel.linea = linea.icodcatalogo											");
            psQuery.AppendLine("	and linea.dtinivigencia <> linea.dtFinVigencia								");
            psQuery.AppendLine("	and linea.dtfinvigencia >= getdate()										");
            psQuery.AppendLine(left);
            psQuery.AppendLine("'");
            if (NuevoEmpleadoBackend.BuscaBanderaValidaRestricciones(int.Parse(Session["iCodUsuario"].ToString())))
            {
                psQuery.AppendLine("	set @query= @query + 																	");
                psQuery.AppendLine("				'																			");
                psQuery.AppendLine("				Inner Join [VisHisComun(''Sitio'',''Español'')] SitioComun	WITH(NOLOCK)");
                psQuery.AppendLine("					On linea.Sitio = SitioComun.iCodCatalogo								");
                psQuery.AppendLine("					And SitioComun.dtIniVigencia <> SitioComun.dtFinVigencia				");
                psQuery.AppendLine("					And SitioComun.dtFinVigencia >= GETDATE()								");
                psQuery.AppendLine("				Inner JOIN RestSitio RestSitio	WITH(NOLOCK)								");
                psQuery.AppendLine("					ON sitioComun.iCodCatalogo = RestSitio.Sitio							");
                psQuery.AppendLine("					And sitioComun.dtIniVigencia <= RestSitio.FechaInicio					");
                psQuery.AppendLine("					And sitioComun.dtFinVigencia >= RestSitio.Fechafin						");
                psQuery.AppendLine("					And RestSitio.Usuar = '+convert(varchar,@icodCatUsuario)+'									");
                psQuery.AppendLine("				'																			");

            }

            psQuery.AppendLine("Set @query = @query + '");
            psQuery.AppendLine("where Emple.dtinivigencia <> Emple.dtFinvigencia								");
            psQuery.AppendLine("and Emple.dtFinVigencia >= GETDATE()											");
            psQuery.AppendLine("and Emple.iCodCatalogo = " + iCodCatalogoEmple + "									");
            psQuery.AppendLine("'exec(@query)");
            dtLinea = DSODataAccess.Execute(psQuery.ToString());

            grvLinea.DataSource = dtLinea;
            grvLinea.DataBind();

            UpDatosLinea.Update();
        }

        protected string consultaFechaEnvio()
        {
            string lsConsultaCCustodia = "select iCodCatalogo from [VisHistoricos('CCustodia','Cartas custodia','Español')] WITH(NOLOCK)" +
                               "where FolioCCustodia = " + txtFolioCCustodia.Text +
                               "and dtIniVigencia<>dtFinVigencia and dtFinVigencia >= getdate()";

            StringBuilder sbUltimoEnvio = new StringBuilder();
            sbUltimoEnvio.AppendLine("select top 1 FechaEnvio from [VisDetallados('Detall','Bitacora Envio CCustodia','Español')] WITH(NOLOCK)\r");
            sbUltimoEnvio.AppendLine("where CCustodia in (" + lsConsultaCCustodia + ")");
            sbUltimoEnvio.AppendLine("and FechaEnvio is not null order by dtFecUltAct desc");

            DataRow drUltimoEnvio = DSODataAccess.ExecuteDataRow(sbUltimoEnvio.ToString());

            if (drUltimoEnvio != null)
            {
                return drUltimoEnvio["FechaEnvio"].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        protected void ActualizaJerarquiaRest(string iCodCatalogoEmple)
        {
            string iCodPadre = "";
            //RZ.20130812 Solo se irá a consultar el jefe inmediato cuando no se este en la alta.
            if (estado != String.Empty || estado != "alta")
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

            int liCodUsuario = (int)Session["iCodUsuarioDB"];
            pCargaCom.ActualizaJerarquiaRestEmple(iCodCatalogoEmple, iCodPadre, liCodUsuario);
            Marshal.ReleaseComObject(pCargaCom);
            pCargaCom = null;
        }

        protected void lbtnRegresarPaginaBusq_Click(object sender, EventArgs e)
        {
            if (Request.QueryString["busq"] == "top10ccust")
            {
                HttpContext.Current.Response.Redirect("~/UserInterface/CCustodiaDTI/DashboardTop10CCustodia.aspx");
            }

            HttpContext.Current.Response.Redirect("~/UserInterface/CCustodiaDTI/BusquedaCartasCustodia.aspx?stCC=2");
        }

        protected void lbtnRegresarPagBusqExternaCCust_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Redirect("~/BusquedaExternaCCustodia/BusquedaExternaCCustodia.aspx");
        }

        #region Botones y Metodos de logica de negocios

        protected void btnEnviarCCustodiaEmple_Click(object sender, EventArgs e)
        {
            ////La carta esta en estatus pendiente?

            string iCodCCust = DSODataAccess.ExecuteScalar("select iCodCatalogo from [VisHistoricos('CCustodia','Cartas custodia','Español')] " +
                                                           "where FolioCCustodia = " + txtFolioCCustodia.Text +
                                                           "and dtIniVigencia<>dtFinVigencia and dtFinVigencia >= getdate()").ToString();

            //Query para revisar si no existe un envio programado de la carta custodia
            StringBuilder sbExisteEnvioProg = new StringBuilder();
            sbExisteEnvioProg.AppendLine("select iCodRegistro from [VisDetallados('Detall','Bitacora Envio CCustodia','Español')] ");
            sbExisteEnvioProg.AppendLine("where CCustodia =" + iCodCCust);
            sbExisteEnvioProg.AppendLine("and FechaEnvio is null");
            DataRow drExisteEnvio = DSODataAccess.ExecuteDataRow(sbExisteEnvioProg.ToString());

            //Ya existe un envio programado?
            if (drExisteEnvio != null)
            {
                MensajeDeAdvertencia("Ya existe un envio programado por enviar para esta carta custodia.");
            }

            //No existe un envio programado para esta carta entonces se hace un insert en bitacora envio ccust
            else
            {
                //Me traigo la ultima FechaEnvio de la CCustodia
                StringBuilder sbUltimoEnvio = new StringBuilder();
                sbUltimoEnvio.AppendLine("select top 1 FechaEnvio from [VisDetallados('Detall','Bitacora Envio CCustodia','Español')] ");
                sbUltimoEnvio.AppendLine("where CCustodia =" + iCodCCust + " and FechaEnvio is not null order by dtFecUltAct desc");
                DataRow drUltimoEnvio = DSODataAccess.ExecuteDataRow(sbUltimoEnvio.ToString());

                if (drUltimoEnvio != null)
                {
                    DateTime fechaEnvio = Convert.ToDateTime(drUltimoEnvio["FechaEnvio"].ToString());

                    DateTime fechaUltAct = Convert.ToDateTime(txtUltimaMod.Text);

                    //Se comparan la FechaEnvio de la CCust y la dtFecUltAct de la CCust para saber si ya se envio la carta 
                    if (fechaEnvio >= fechaUltAct)
                    {
                        //Se debe mostrar un pop_up con un mensaje diciendo que la carta ya se ha enviado 
                        //y preguntando si la desea enviar nuevamente

                        //RZ.20131202 Si ya detecto que el empleado tiene fecha de envio, entonces actualizar el textbox
                        txtUltimoEnvio.Text = drUltimoEnvio["FechaEnvio"].ToString();
                        mpeConfirmEnvio.Show();
                    }
                    //Si no se cumple la condicion entonces se programa el envio de la CCust
                    else
                    {
                        programaEnvioCCust();
                        //RZ.20131202 Notificar que ya se programo un envio
                        MensajeDeAdvertencia("Se ha programado un envío de la carta custodia al empleado");
                    }
                }
                else
                {
                    programaEnvioCCust();
                    //RZ.20131202 Notificar que ya se programo un envio
                    MensajeDeAdvertencia("Se ha programado un envío de la carta custodia al empleado");
                }
            }
        }

        protected void btnAceptarEnvioCCust_ConfEnvio(object sender, EventArgs e)
        {
            programaEnvioCCust();
            //RZ.20131202 Notificar que ya se programo un reenvio
            MensajeDeAdvertencia("Se ha programado un reenvío de la carta custodia");
        }

        protected void btnCambiarEstatusPte_Click(object sender, EventArgs e)
        {
            //El value para cambiar la carta a estatus pendiente en [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')] es 1
            CambiarEstatusCCust(1);
        }

        protected void btnAceptarCCust_Click(object sender, EventArgs e)
        {

            //El value para aceptar la carta en [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')] es 2
            CambiarEstatusCCust(2);

            OcultaBotonesAceptarYRechazarCCustodia();

            lblMensajeNotificaEmple1.Text = "La carta custodia ha sido aceptada, en unos momentos llegara un correo de notificación.";
            mpeNotificaEmple.Show();

            txtComenariosEmple.Enabled = false;

            //RZ.20131202 Se programa envio de la carta custodia.
            programaEnvioCCust();
        }

        /*RZ.20130713 Se ocultan botones de aceptar y rechazar carta custodia*/
        protected void OcultaBotonesAceptarYRechazarCCustodia()
        {
            btnAceptarCCust.Visible = false;
            btnAceptarCCust.Enabled = false;
            btnRechazarCCust.Visible = false;
            btnRechazarCCust.Enabled = false;
        }

        protected void btnRechazarCCust_Click(object sender, EventArgs e)
        {
            if (txtComenariosEmple.Text != string.Empty)
            {
                //El value para rechazar la carta en [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')] es 3
                CambiarEstatusCCust(3);

                OcultaBotonesAceptarYRechazarCCustodia();

                lblMensajeNotificaEmple1.Text = "La carta custodia ha sido rechazada, en unos momentos llegara un correo de notificación.";
                mpeNotificaEmple.Show();

                txtComenariosEmple.Enabled = false;

                //RZ.20131202 Se programa envio de la carta custodia.
                programaEnvioCCust();
            }
            else
            {
                MensajeDeAdvertencia("Por favor, especifique el motivo del rechazo de la carta usando el campo de comentarios.");
            }
        }

        protected void CambiarEstatusCCust(int Estatus)
        {
            try
            {
                string iCodEstatusCCust = DSODataAccess.ExecuteScalar("select iCodCatalogo from [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')]" +
                                                                      " where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate() and Value = " + Estatus).ToString();

                string iCodCCust = DSODataAccess.ExecuteScalar("select iCodCatalogo from [VisHistoricos('CCustodia','Cartas custodia','Español')] " +
                                              " where FolioCCustodia = " + txtFolioCCustodia.Text +
                                              " and dtIniVigencia<>dtFinVigencia and dtFinVigencia >= getdate()").ToString();

                //RZ.20130715 Se agrega update de comentarios de admin desde lo que contenga el textbox txtComentariosAdmin
                DSODataAccess.ExecuteNonQuery("update [VisHistoricos('CCustodia','Cartas custodia','Español')] " +
                                              " set EstCCustodia = " + iCodEstatusCCust + "," + "ComentariosEmple = '" + txtComenariosEmple.Text + "', ComentariosAdmin = '" + txtComentariosAdmin.Text + "', dtFecUltAct = getdate()" +
                                              " where FolioCCustodia = " + txtFolioCCustodia.Text +
                                              " and dtIniVigencia<>dtFinVigencia and dtFinVigencia >= getdate()");

                DataTable dtEmple = new DataTable();
                dtEmple = cargaDatosEmple(iCodCatalogoEmple);
                FillDatosEmple(dtEmple);
                upDatosEmple.Update();
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al intentar cambiar el estatus de la carta con folio '" + txtFolioCCustodia.Text + "'", ex);
                throw ex;
            }
        }

        protected void programaEnvioCCust()
        {
            if (!string.IsNullOrEmpty(txtEmailEmple.Text.Trim()))
            {
                string iCodCCust = DSODataAccess.ExecuteScalar("select iCodCatalogo from [VisHistoricos('CCustodia','Cartas custodia','Español')] " +
                                      "where FolioCCustodia = " + txtFolioCCustodia.Text +
                                      "and dtIniVigencia<>dtFinVigencia and dtFinVigencia >= getdate()").ToString();

                //Se inserta un registro en [VisDetallados('Detall','Bitacora Envio CCustodia','Español')]
                DALCCustodia dalCCust = new DALCCustodia();
                dalCCust.InsertRegEnBitacoraEnvioCCust(txtFolioCCustodia.Text, iCodCCust, txtEmailEmple.Text);
            }
            else
            {
                MensajeDeAdvertencia("El Empleado no cuenta con un Email configurado. Configure el Email del empleado e intente de nuevo.");
            }
        }

        #endregion


        #region Empleados

        //RZ.20130722 Se agrega fecha fin para estado edit
        protected DataTable cargaDatosEmple(string iCodCatEmple)
        {
            DataTable ldtEmple = new DataTable();

            psQuery.Length = 0;
            psQuery.AppendLine("SELECT Nombre = Emple.Nombre, ApPaterno = Emple.Paterno, ApMaterno = Emple.Materno, SegundoNombre = EmpleB.SegundoNombre,Area= ISNULL(Emple.Ubica,''),");
            psQuery.AppendLine("FechaInicio = Emple.dtIniVigencia, FechaFin = Emple.dtFinVigencia, NoFolio = CCustodia.FolioCCustodia, TipoEmple = Emple.TipoEm, EmpreEmple = EmpleB.Proveedor,");

            //20140829 AM. Se cambia la vista de donde toma el estatus la carta custodia
            //psQuery.Append("Estatus = CCustodia.EstCCustodiaDesc, Empleado = Emple.NominaA, Nombre = Emple.NomCompleto, \r");
            psQuery.AppendLine("Estatus = EstCCust.vchDescripcion, Empleado = Emple.NominaA, Nombre = Emple.NomCompleto,");
            psQuery.AppendLine("Ubicacion = Sitio.iCodCatalogo, CenCos = relcencosEmple.CenCos, Puesto = Emple.Puesto,");
            psQuery.AppendLine("Localidad = EmpleB.Estados,Locali= ISNULL(Emple.Ubica,''),  Email = Emple.Email, RadioNextel = CCustodia.NumRadio,");
            psQuery.AppendLine("UsuarioRed = Emple.UsuarCod, Celular = CCustodia.NumTelMovil, Gerente = ((isnull(emple.BanderasEmple,0)) & 2) / 2,");
            psQuery.AppendLine("ComentariosEmple = CCustodia.ComentariosEmple, ComentariosAdmin = CCustodia.ComentariosAdmin, FecUltModificacion = CCustodia.dtFecUltAct,");
            psQuery.AppendLine("JefeInmediato = ISNULL(Emple.Emple,'0'), NombreJefeEmple = ISNULL(EmpleJefe.NomCompleto,''),EmailJefe = ISNULL(EmpleJefe.Email,''), VisibleDir = ((isnull(emple.BanderasEmple,0)) & 1) / 1,");
            psQuery.AppendLine("ComentarioSincro = EmpleB.Comentarios, EmpleOmiteDeSincro = ((isnull(emple.BanderasEmple,0)) & 4) / 4");
            psQuery.AppendLine(",PresMovil = ISNULL(PREPM.PresupFijo,0),PresFija = ISNULL(PREPF.PresupFijo,0),PresTemp = ISNULL(PREPT.PresupProv,0)");
            //RM 20190510 Campos FCA se unen con un left  solo cuando es FCA
            if (DSODataContext.Schema.ToUpper() == "FCA")
            {
                psQuery.Append(",");
                psQuery.AppendLine("DC_id = efca.FCA_C_id,");
                psQuery.AppendLine("T_id = efca.FCA_T_id,");
                psQuery.AppendLine("NickName = Case When (emple.email is not null And len(emple.email) > 0 And charindex('@',emple.email) > 0) then  substring(isNull(emple.email, ''), 1, charIndex('@', emple.email) - 1) else ''end,");
                psQuery.AppendLine("estacion = FCA_estacion,");
                psQuery.AppendLine("director = isNull(efca.iCodCatalogodirector,0),");
                psQuery.AppendLine("esDirector = ((isNull(emple.banderasEmple, 0) & 32 )/ 32),");
                psQuery.AppendLine("Planta = PlantaFCA,");
                psQuery.AppendLine("Depto = Dpto.Cencos");
            }

            psQuery.AppendLine("FROM [VisHistoricos('Emple','Empleados','Español')] Emple WITH(NOLOCK)");
            psQuery.AppendLine("INNER JOIN[VisRelaciones('CentroCosto-Empleado','Español')] relcencosEmple WITH(NOLOCK)");
            psQuery.AppendLine("    On emple.iCodCatalogo = relcencosEmple.Emple");
            psQuery.AppendLine("    And relcencosEmple.dtIniVigencia<> relcencosEmple.dtFinVigencia");
            psQuery.AppendLine("    And relcencosEmple.dtFinVigencia >= GETDATE()");
            psQuery.AppendLine("INNER JOIN [VisHistoricos('CCustodia','Cartas custodia','Español')] CCustodia WITH(NOLOCK)");
            psQuery.AppendLine("    ON Emple.iCodCatalogo = CCustodia.Emple");
            psQuery.AppendLine("    and CCustodia.dtIniVigencia <> CCustodia.dtFinVigencia");
            psQuery.AppendLine("    and Emple.dtIniVigencia <> Emple.dtFinVigencia");
            psQuery.AppendLine("    and Emple.dtFinVigencia >= GETDATE()");
            psQuery.AppendLine("LEFT OUTER JOIN [VisHistoricos('Empleb','Empleados b','Español')] EmpleB WITH(NOLOCK)");
            psQuery.AppendLine("    ON Emple.iCodCatalogo = EmpleB.Emple");
            psQuery.AppendLine("    and EmpleB.dtIniVigencia <> EmpleB.dtFinVigencia");
            psQuery.AppendLine("    and EmpleB.dtFinVigencia >= GETDATE()");
            psQuery.AppendLine("LEFT OUTER JOIN (SELECT NomCompleto, Email, iCodCatalogo");
            psQuery.AppendLine("        FROM [VisHistoricos('Emple','Empleados','Español')] WITH(NOLOCK)");
            psQuery.AppendLine("        WHERE dtIniVigencia <> dtFinVigencia");
            psQuery.AppendLine("        and dtFinVigencia >= GETDATE()");
            psQuery.AppendLine("        ) as EmpleJefe");
            psQuery.AppendLine("    ON EmpleJefe.iCodCatalogo = Emple.Emple");
            psQuery.AppendLine("LEFT OUTER JOIN (SELECT iCodCatalogo, vchDescripcion");
            psQuery.AppendLine("        FROM Historicos");
            psQuery.AppendLine("        WHERE dtIniVigencia <> dtFinVigencia");
            psQuery.AppendLine("        and dtFinVigencia >= GETDATE()");
            psQuery.AppendLine("        and iCodMaestro in (select iCodRegistro");
            psQuery.AppendLine("            from Maestros");
            psQuery.AppendLine("            where iCodEntidad = 23 --Sitios");
            psQuery.AppendLine("             )");
            psQuery.AppendLine("        ) as Sitio");
            psQuery.AppendLine("    ON Sitio.vchDescripcion = Emple.Ubica");

            //20140829 AM. Se agrega el join con vista de estatus para sacar la descripcion del estatus en caso de que por vigencias no se muestre en la vista de cartas custodia.
            psQuery.AppendLine("JOIN [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')] EstCCust");
            psQuery.AppendLine("on EstCCust.iCodCatalogo = CCustodia.EstCCustodia");
            psQuery.AppendLine("and EstCCust.dtIniVigencia <> EstCCust.dtFinVigencia and EstCCust.dtFinVigencia >= GETDATE()");
            psQuery.AppendLine(" LEFT JOIN [VisHistoricos('PrepEmple','Presupuesto para moviles','Español')] AS PREPM WITH(NOLOCK)");
            psQuery.AppendLine("     ON Emple.icodCatalogo = PREPM.Emple");
            psQuery.AppendLine("     AND PREPM.dtIniVigencia <> PREPM.dtFinVigencia AND PREPM.dtFinVigencia >= GETDATE()");
            psQuery.AppendLine(" LEFT JOIN[VisHistoricos('PrepEmple','Presupuesto Fijo','Español')] AS PREPF WITH(NOLOCK)");
            psQuery.AppendLine("     ON Emple.icodCatalogo = PREPF.Emple");
            psQuery.AppendLine("     AND PREPF.dtIniVigencia <> PREPF.dtFinVigencia AND PREPF.dtFinVigencia >= GETDATE()");
            psQuery.AppendLine(" LEFT JOIN[VisHistoricos('PrepEmple','Presupuesto Temporal','Español')] AS PREPT WITH(NOLOCK)");
            psQuery.AppendLine("     ON Emple.icodCatalogo = PREPT.Emple");
            psQuery.AppendLine("     AND PREPT.dtIniVigencia <> PREPT.dtFinVigencia AND PREPT.dtFinVigencia >= GETDATE()");
            //RM 20190510 lefts join para buscar campos propios de FCA 
            if (DSODataContext.Schema.ToUpper() == "FCA")
            {
                psQuery.AppendLine("Left Join [visHistoricos('empleFCA','empleados fca','español')] efca		");
                psQuery.AppendLine("	On emple.iCodCatalogo = efca.emple										");
                psQuery.AppendLine("	And efca.dtIniVigencia <> efca.dtFinVigencia							");
                psQuery.AppendLine("	And efca.dtFinVigencia >= getdate()										");
                psQuery.AppendLine("Left Join [visHistoricos('emple','empleados','español')] director			");
                psQuery.AppendLine("	On director.iCodCatalogo = efca.iCodCatalogoDirector					");
                psQuery.AppendLine("	And director.dtIniVigencia <> director.dtFinVigencia					");
                psQuery.AppendLine("	And director.dtFinVigencia >= getdate()									");
                psQuery.AppendLine("Left Join [VisHistoricos('PlantaFCA','Plantas FCA','Español')] plantafca");
                psQuery.AppendLine("    On efca.plantafca = plantafca.icodcatalogo");
                psQuery.AppendLine("    And plantafca.dtinivigencia <> plantafca.dtfinvigencia");
                psQuery.AppendLine("    And plantafca.dtfinvigencia >= getdate()");
                psQuery.AppendLine("left join [VisRelaciones('FCA CentroCosto-Externo','Español')] dpto	");
                psQuery.AppendLine("	 On emple.icodcatalogo = dpto.emple									");
                psQuery.AppendLine("	 And Dpto.dtIniVigencia <> dpto.dtfinvigencia						");
                psQuery.AppendLine("	 And dpto.dtfinvigencia >= getdate()								");
            }

            psQuery.AppendLine(" WHERE Emple.iCodCatalogo = " + iCodCatEmple);

            //20150512.RJ Condicion agregada para que regrese solo una carta, en caso de que el empleado tuviera más
            psQuery.AppendLine(" and CCustodia.icodregistro = (select max(CCust2.icodregistro)");
            psQuery.AppendLine(" 					from [VisHistoricos('CCustodia','Cartas custodia','Español')] CCust2");
            psQuery.AppendLine(" 					where CCust2.Emple = " + iCodCatEmple);
            psQuery.AppendLine(" 					and dtinivigencia<>dtfinvigencia");
            psQuery.AppendLine(" 					and dtfinvigencia = (select max(dtfinvigencia)");
            psQuery.AppendLine(" 											from [VisHistoricos('CCustodia','Cartas custodia','Español')] CCust2");
            psQuery.AppendLine(" 											where CCust2.Emple = " + iCodCatEmple);
            psQuery.AppendLine(" 											and dtinivigencia<>dtfinvigencia");
            psQuery.AppendLine(" 										) ");
            psQuery.AppendLine(" 					) ");

            ldtEmple = DSODataAccess.Execute(psQuery.ToString());

            //Revisa si el cliente es FCA e intercambia los campos Cencos y Depto
            if (DSODataContext.Schema.ToUpper() == "FCA")
            {
                if (ldtEmple.Columns.Contains("Cencos") && ldtEmple.Columns.Contains("Depto"))
                {
                    foreach (DataRow row in ldtEmple.Rows)
                    {
                        string aux = row["Depto"].ToString();
                        row["Depto"] = row["CenCos"];
                        row["Cencos"] = Convert.ToInt32(aux.Length > 0 ? aux : row["Cencos"].ToString());
                    }
                }

                if (ldtEmple.Rows.Count > 0 && ldtEmple.Columns.Count > 0 && ldtEmple.Columns.Contains("TipoEmple") && ldtEmple.Rows[0]["TipoEmple"].ToString() == "444")
                {
                    pnlCencos.Visible = false;
                }

            }
            return ldtEmple;
        }
        /*JH Obtiene los datos de Usuario del empleado*/
        protected void ObtieneUserEmple(string iCodCatEmple)
        {
            DataTable ldtEmple = new DataTable();
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT");
            query.AppendLine(" U.vchCodigo,");
            query.AppendLine(" U.Password");
            query.AppendLine(" FROM " + DSODataContext.Schema.ToUpper() + ".HistEmple AS E WITH(NOLOCK)");
            query.AppendLine(" JOIN " + DSODataContext.Schema.ToUpper() + ".[VisHistoricos('Usuar','Usuarios','Español')] AS U WITH(NOLOCK)");
            query.AppendLine(" ON E.Usuar = U.iCodCatalogo");
            query.AppendLine(" AND U.dtIniVigencia<> U.dtFinVigencia");
            query.AppendLine(" AND U.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE E.dtIniVigencia<> E.dtFinVigencia");
            query.AppendLine(" AND E.dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND E.iCodCatalogo = " + iCodCatEmple + " ");

            ldtEmple = DSODataAccess.Execute(query.ToString());

            if (ldtEmple != null && ldtEmple.Rows.Count > 0)
            {
                DataRow dr = ldtEmple.Rows[0];
                Session["iCodCatEmpleFCA"] = iCodCatEmple.ToString();
                Session["userEmple"] = dr["vchCodigo"].ToString();
                Session["passwordEmple"] = dr["Password"].ToString();
            }
        }
        #region Baja Empleado
        /*20130718 Se agrega event handler de delete emple*/
        protected void lbtnDeleteEmple_Click(object sender, EventArgs e)
        {
            /*Validar si el empleado tiene o no otros empleados a cargo.*/
            validarBajaEmple();
        }

        protected void validarBajaEmple()
        {
            //se cambia por el > que tenia
            int cantEmple = esEmpleadoResponsable(iCodCatalogoEmple);

            if (cantEmple > 0)
            {
                mostrarModalPopReasignaEmpleados(cantEmple);
            }
            else
            {
                mostrarModalPopBajaEmple();
            }
        }

        protected int esEmpleadoResponsable(string iCodCatEmple)
        {
            StringBuilder lsbConsulta = new StringBuilder();

            lsbConsulta.AppendLine("SELECT isNull(COUNT(*),0) FROM Historicos");
            lsbConsulta.AppendLine("WHERE iCodMaestro in (");
            lsbConsulta.AppendLine("     SELECT iCodRegistro");
            lsbConsulta.AppendLine("     FROM Maestros WHERE icodentidad in (");
            lsbConsulta.AppendLine("        SELECT iCodRegistro");
            lsbConsulta.AppendLine("        FROM Catalogos");
            lsbConsulta.AppendLine("        WHERE vchCodigo like 'Emple'");
            lsbConsulta.AppendLine("            AND iCodCatalogo is null)");
            lsbConsulta.AppendLine("            AND vchDescripcion like 'Empleados')");
            lsbConsulta.AppendLine("AND dtIniVigencia <> dtFinVigencia");
            lsbConsulta.AppendLine("AND dtFinVigencia >= GETDATE()");
            lsbConsulta.AppendLine("AND iCodCatalogo04 = " + iCodCatEmple); //Empleado Responsable

            return (int)DSODataAccess.ExecuteScalar(lsbConsulta.ToString());
        }

        //RZ.20130725 Se agrega metodo que crea el contenido de la modalpopup
        protected void mostrarModalPopBajaEmple()
        {
            //pnlBajaEmple.Height = 150;
            StringBuilder lsbArmarMensajeEmpleBaja = new StringBuilder();

            lsbArmarMensajeEmpleBaja.Append("<p>Seleccione la fecha de baja para el empleado ");
            lsbArmarMensajeEmpleBaja.Append("<strong>" + armarNomCompleto() + "</strong>.");
            lsbArmarMensajeEmpleBaja.Append("</p><br />");

            lcEmpleEnBajaMsj.Text = lsbArmarMensajeEmpleBaja.ToString();

            mpeBajaEmple.Show();
        }

        protected void mostrarModalPopReasignaEmpleados(int cantEmple)
        {
            StringBuilder lsbArmarMensajeEmpleBaja = new StringBuilder();
            lsbArmarMensajeEmpleBaja.Append("<p style='margin:0px;'>El empleado ");
            lsbArmarMensajeEmpleBaja.Append("<strong>" + armarNomCompleto() + "</strong>");
            lsbArmarMensajeEmpleBaja.Append(" aún no puede ser borrado ya que cuenta con ");
            lsbArmarMensajeEmpleBaja.Append("<strong> " + cantEmple.ToString() + "</strong> empleados bajo su cargo: ");
            lsbArmarMensajeEmpleBaja.Append("</p>");
            lcEmpleReasigna.Text = lsbArmarMensajeEmpleBaja.ToString();

            grvEmpleDepende.DataSource = getEmpleDependientes(iCodCatalogoEmple);
            grvEmpleDepende.DataBind();
            lsbArmarMensajeEmpleBaja.Length = 0;
            lsbArmarMensajeEmpleBaja.Append("Para eliminar al empleado, se requiere que asigne un nuevo jefe a los empleados mencionados.");

            lcEmpleNuevoJefe.Text = lsbArmarMensajeEmpleBaja.ToString();
            mpeReasingarBajaEmple.Show();
        }

        protected DataTable fillNuevoEmpleResp(string iCodCatEmple)
        {
            return NuevoEmpleadoBackend.GetJefes();
        }

        protected DataTable getEmpleDependientes(string iCodCatEmple)
        {
            StringBuilder lsbConsulta = new StringBuilder();

            lsbConsulta.Append("SELECT TOP 100 NominaA, NomCompleto, PuestoDesc \r");
            lsbConsulta.Append("FROM [VisHistoricos('Emple','Empleados','Español')] \r");
            lsbConsulta.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
            lsbConsulta.Append("and dtFinVigencia >= GETDATE() \r");
            lsbConsulta.Append("and Emple = " + iCodCatEmple + " \r"); //Empleado Responsable

            return DSODataAccess.Execute(lsbConsulta.ToString());
        }

        protected void btnAceptarBajaEmple_Click(object sender, EventArgs e)
        {
            DALCCustodia dalCC = new DALCCustodia();
            DateTime dtFechaFinVigencia = DateTime.MinValue;
            DateTime dtFechaInicioEmple = DateTime.MinValue;
            StringBuilder lsbErrores = new StringBuilder();
            string lsError;
            string lsTitulo = DSOControl.JScriptEncode("Empleados");
            pjsObj = "HistoricEdit1";

            if (txtFechaBajaEmpleado.Text != String.Empty)
            {
                DateTime.TryParse(txtFechaBajaEmpleado.Text, out dtFechaFinVigencia);
                DateTime.TryParse(txtFecha.Text, out dtFechaInicioEmple);
            }
            else
            {
                dtFechaFinVigencia = DateTime.Today;
            }

            if (dtFechaFinVigencia == DateTime.MinValue)
            {
                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Fecha Fin"));
                lsbErrores.Append("<li>" + lsError + "</li>");
            }

            if (dtFechaFinVigencia < dtFechaInicioEmple)
            {
                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "VigenciaFin", "Fecha Inicio Empleado", "Fecha Fin"));
                lsbErrores.Append("<li>" + lsError + "</li>");
            }

            if (lsbErrores.Length > 0)
            {
                lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
            }
            else
            {
                //Se requiere mandar dtFechaFinVigencia como parametro para todas las bajas de recursos
                dalCC.DarDeBajaEmpleadoYRecursos(iCodCatalogoEmple, dtFechaFinVigencia);

                ActualizaJerarquiaRest(iCodCatalogoEmple);

                HttpContext.Current.Response.Redirect("~/UserInterface/CCustodiaDTI/BusquedaCartasCustodia.aspx?ne=" + iCodCatalogoEmple);
            }
        }

        protected void btnReasignarBajaEmple_Click(object sender, EventArgs e)
        {
            StringBuilder lsbUpdateEmple = new StringBuilder();
            string lsEmpleRespSelected = drpNuevoEmpleResp.SelectedValue;
            StringBuilder lsbErrores = new StringBuilder();
            string lsError;
            string lsTitulo = DSOControl.JScriptEncode("Empleados");
            pjsObj = "HistoricEdit1";

            lsbUpdateEmple.Append("UPDATE [VisHistoricos('Emple','Empleados','Español')] \r");
            lsbUpdateEmple.Append("SET EMPLE = " + lsEmpleRespSelected + " \r");
            lsbUpdateEmple.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
            lsbUpdateEmple.Append("and dtFinvigencia >= getdate() \r");
            lsbUpdateEmple.Append("and Emple = " + iCodCatalogoEmple + " \r"); //Empleado Responsable

            bool lbActualizaEmple = DSODataAccess.ExecuteNonQuery(lsbUpdateEmple.ToString());

            if (lbActualizaEmple)
            {
                mostrarModalPopBajaEmple();
            }
            else
            {
                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "FallaReasignaEmple", "Aviso"));
                lsbErrores.Append("<li>" + lsError + "</li>");

                lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
            }
        }

        protected string armarNomCompleto()
        {
            return txtNombreEmple.Text.Trim() + " " + txtSegundoNombreEmple.Text.Trim() + " " + txtApPaternoEmple.Text.Trim() + " " + txtApPaternoEmple.Text.Trim();
        }
        #endregion

        /*20130718 Se agrega metodo de cancelar emple*/
        protected void lbtnCancelarEmple_Click(object sender, EventArgs e)
        {
            enableAllWebControlsEmple();
            cargaBotonesCancelarEmple();
            OcultaAsteriscos(false);
            //Resetear los controles a sus valores iniciales
            DataTable dtEmple = new DataTable();
            dtEmple = cargaDatosEmple(iCodCatalogoEmple);
            FillDatosEmple(dtEmple);

            /*Se desabilitan botones para agregar CenCos y Puesto*/
            lbtnAgregarCenCos.Visible = false;
            lbtnAgregarCenCos.Enabled = false;

            lbtnAgregarPuesto.Visible = false;
            lbtnAgregarPuesto.Enabled = false;
        }

        //20130721
        protected void lbtnEditEmple_Click(object sender, EventArgs e)
        {
            enableAllWebControlsEmple();
            cargaBotonesModificarEmple();
            OcultaAsteriscos(true);
            /*Se habilitan botones para agregar CenCos y Puesto*/
            lbtnAgregarCenCos.Visible = true;
            lbtnAgregarCenCos.Enabled = true;

            lbtnAgregarPuesto.Visible = true;
            lbtnAgregarPuesto.Enabled = true;
            txtNominaEmple.Enabled = false;

        }

        //20130721
        protected void lbtnSaveEmple_Click(object sender, EventArgs e)
        {
            try
            {

                if (ValidarEmpleado())
                {
                    if (estado == "alta" || estado == String.Empty)
                    {
                        iCodCatalogoEmple = GrabarEmpleado();
                    }

                    if (estado == "edit")
                    {
                        EditarEmpleado();
                    }

                    if (!String.IsNullOrEmpty(iCodCatalogoEmple))
                    {
                        //Pasar al estado de consulta
                        string url = Page.Request.Url.ToString();

                        if (!url.Contains("&iCodEmple="))
                        {
                            url = url + "&iCodEmple=" + iCodCatalogoEmple + "&st=tLaJQx5zLrc=";
                        }

                        Page.Response.Redirect(url, true);
                    }

                    /*Se desabilitan botones para agregar CenCos y Puesto*/
                    lbtnAgregarCenCos.Visible = false;
                    lbtnAgregarCenCos.Enabled = false;

                    lbtnAgregarPuesto.Visible = false;
                    lbtnAgregarPuesto.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al intentar guardar los datos de empleado '" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        /*RZ.20130722*/
        protected string GrabarEmpleado()
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
                if (phtValuesEmple.ContainsKey("{CenCos}") && ddlDatosEmpleFCADpto.SelectedItem.Text.Length > 0)
                {
                    phtValuesEmple["{CenCos}"] = ddlDatosEmpleFCADpto.SelectedValue;
                }
            }

            phtValuesEmple.Remove("{PresupMovil}");
            int iCodRegEmple = dalCC.AltaEmple(phtValuesEmple);
            string iCodEmple = String.Empty;

            if (iCodRegEmple != -1)
            {
                //El metodo regresa el icodcatalogo del empleado cuando es mayor a 0, que quiere decir que si se logro la alta.
                iCodEmple = iCodRegEmple.ToString();

                lhtEmpleB.Clear();
                lhtEmpleB.Add("vchCodigo", phtValuesEmple["vchCodigo"].ToString() + " (B)");
                lhtEmpleB.Add("vchDescripcion", lsNomCompleto + " (B)");
                lhtEmpleB.Add("{Emple}", iCodEmple);
                if (!String.IsNullOrEmpty(NoSQLCode(drpLocalidadEmple.SelectedValue)))
                {
                    lhtEmpleB.Add("{Estados}", NoSQLCode(drpLocalidadEmple.SelectedValue));
                }
                if (!String.IsNullOrEmpty(NoSQLCode(drpEmpresaEmple.SelectedValue)))
                {
                    lhtEmpleB.Add("{Proveedor}", NoSQLCode(drpEmpresaEmple.SelectedValue));
                }
                lhtEmpleB.Add("{SegundoNombre}", NoSQLCode(txtSegundoNombreEmple.Text));
                //20161108 Se agrega campo de comentarios para cuando se marca al empleado para que se excluya de la sincronización
                if (ckbOmiteSincro.Checked == true)
                {
                    lhtEmpleB.Add("{Comentarios}", NoSQLCode(txtComentariosSincro.Text));
                }

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
                lhtCCust.Add("{FechaCreacion}", Convert.ToDateTime(txtFecha.Text).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                lhtCCust.Add("{FechaResp}", null);
                lhtCCust.Add("{FechaCancelacion}", null);
                lhtCCust.Add("{ComentariosEmple}", null);
                lhtCCust.Add("{ComentariosAdmin}", null);
                lhtCCust.Add("dtIniVigencia", Convert.ToDateTime(txtFecha.Text));
                lhtCCust.Add("dtFinVigencia", new DateTime(2079, 1, 1));
                lhtCCust.Add("iCodUsuario", HttpContext.Current.Session["iCodUsuario"]);
                lhtCCust.Add("dtFecUltAct", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                dalCC.AltaCCustodia(lhtCCust);

                /*Da de alta los presupuestos del empleado*/
                var prepTemporal = (txtPrepTemporal.Text == "") ? "0" : txtPrepTemporal.Text;
                dalCC.ActualizaPresupuestosEmple(iCodRegEmple.ToString(), txtPrepMovil.Text, txtPrepFija.Text, prepTemporal);

                /*
                 * 
                 */
                if (DSODataContext.Schema.ToUpper() == "FCA")
                {
                    //Insert de la relacion del empleado con el CC seleccionado
                    dalCC.AltaRelEmpleCenCos(iCodEmple, ddlDatosEmpleFCADpto.SelectedValue.ToString(), Convert.ToDateTime(phtValuesEmple["dtIniVigencia"]), Convert.ToDateTime(phtValuesEmple["dtFinVigencia"]));

                }
                else
                {
                    //Insert de la relacion del empleado con el CC seleccionado
                    dalCC.AltaRelEmpleCenCos(iCodEmple, phtValuesEmple["{CenCos}"].ToString(), Convert.ToDateTime(phtValuesEmple["dtIniVigencia"]), Convert.ToDateTime(phtValuesEmple["dtFinVigencia"]));

                }

                //Alta Empleado FCA
                if (DSODataContext.Schema.ToString().ToUpper() == "FCA")
                {
                    Dictionary<string, string> dctEmpleFCA = new Dictionary<string, string>();
                    dctEmpleFCA.Add("planta", ddlDatosEmpleFCAPlanta.SelectedValue);
                    dctEmpleFCA.Add("iCodCatEmple", iCodRegEmple.ToString());
                    dctEmpleFCA.Add("Dc_id", txtDatosEmpleFCADC_ID.Text);
                    dctEmpleFCA.Add("T_id", txtDatosEmpleFCAT_ID.Text);
                    dctEmpleFCA.Add("Estacion", txtDatosEmpleFCAEstacion.Text);
                    dctEmpleFCA.Add("iCodCatDiretor", ddlDatosEmpleFCADirector.SelectedValue);

                    AltaEmpleFCA(dctEmpleFCA);


                    if (drpTipoEmpleado.SelectedValue.ToString().ToLower() == "446")
                    {
                        //Inserta relacion externo Cencos
                        dalCC.AltaRelExternoCenCos(iCodEmple, drpCenCosEmple.SelectedValue.ToString(), Convert.ToDateTime(phtValuesEmple["dtIniVigencia"]), Convert.ToDateTime(phtValuesEmple["dtFinVigencia"]));
                    }
                }

                LimpiarCamposDatosEmple();

                //Mandar a actualizar la jerarquia y restricciones del empleado creado
                ActualizaJerarquiaRest(iCodEmple);

            }

            Util.LogMessage("RZ. El empleado se ha dado de alta con el siguiente catalogo: " + iCodEmple);

            return iCodEmple;
        }

        protected void AltaCartaCustodia(string iCodCatalogoEmple)
        {
            try
            {
                string sp = "EXEC dbo.AltaCartaCustodiaEmple @Esquema = '" + DSODataContext.Schema + "', @icodEmple = " + iCodCatalogoEmple + "";
                DSODataAccess.Execute(sp.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void LimpiarCamposDatosEmple()
        {
            try
            {

                txtFecha.Text = "";
                txtNominaEmple.Text = "";
                txtNombreEmple.Text = "";
                txtApPaternoEmple.Text = "";
                drpSitioEmple.SelectedIndex = 0;
                drpEmpresaEmple.SelectedIndex = 0;
                drpCenCosEmple.SelectedIndex = 0;
                drpLocalidadEmple.SelectedIndex = 0;
                //drpJefeEmple.SelectedIndex = 0;
                txtEmailJefeEmple.Text = "";
                txtDatosEmpleFCADC_ID.Text = "";
                txtDatosEmpleFCAT_ID.Text = "";
                ddlDatosEmpleFCAPlanta.SelectedIndex = 0;
                txtFolioCCustodia.Text = "";
                txtEstatusCCustodia.Text = "";
                txtSegundoNombreEmple.Text = "";
                txtApMaternoEmple.Text = "";
                drpTipoEmpleado.SelectedIndex = 0;
                drpPuestoEmple.SelectedIndex = 0;
                txtEmailEmple.Text = "";
                txtUsuarRedEmple.Text = "";
                txtDatosEmpleFCANickName.Text = "";
                txtDatosEmpleFCAEstacion.Text = "";
                ddlDatosEmpleFCADirector.SelectedIndex = 0;
                ddlDatosEmpleFCADpto.SelectedIndex = 0;

                cbEsGerenteEmple.Checked = false;
                cbVisibleDirEmple.Checked = false;
                cbDatosEmpleFCAEsDirector.Checked = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool AltaEmpleFCA(Dictionary<string, string> dctEmpleFCA)
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

        protected void EditarEmpleado()
        {
            DALCCustodia dalcc = new DALCCustodia();
            Hashtable lhtEmpleB = new Hashtable();
            Hashtable lhtCCust = new Hashtable();

            Dictionary<string, string> dctEmpleFCA = new Dictionary<string, string>();

            phtValuesEmple.Remove("{PresupMovil}");
            string lsNomCompleto = phtValuesEmple["{NomCompleto}"].ToString();


            if (DSODataContext.Schema.ToUpper() == "FCA")
            {
                /*RZ.20130808 Validar si el CenCos elegido cambio y no se trata del mismo 
                 que se tenia, de ser asi entonces proceder con la baja de la relacion del CC
                 y agregar la nueva con el CC*/
                if (ddlDatosEmpleFCADpto.SelectedValue.ToString() != "0")
                {
                    validaCambioCenCosEmple(ddlDatosEmpleFCADpto.SelectedValue.ToString());
                }


                if (phtValuesEmple["{TipoEm}"].ToString().ToLower() == "446")
                {
                    validaCambioCenCosEmpleExtern(phtValuesEmple["{CenCos}"].ToString());
                }


            }
            else
            {
                /*RZ.20130808 Validar si el CenCos elegido cambio y no se trata del mismo 
                 que se tenia, de ser asi entonces proceder con la baja de la relacion del CC
                 y agregar la nueva con el CC*/
                validaCambioCenCosEmple(phtValuesEmple["{CenCos}"].ToString());
            }

            bool lbActualizaEmple = dalcc.ActualizaEmple(phtValuesEmple, iCodCatalogoEmple);

            if (lbActualizaEmple)
            {

                lhtEmpleB.Clear();
                lhtEmpleB.Add("vchCodigo", phtValuesEmple["vchCodigo"].ToString() + " (B)");
                lhtEmpleB.Add("vchDescripcion", lsNomCompleto + " (B)");
                lhtEmpleB.Add("{Emple}", iCodCatalogoEmple);

                if (!String.IsNullOrEmpty(NoSQLCode(drpLocalidadEmple.SelectedValue)))
                {
                    lhtEmpleB.Add("{Estados}", NoSQLCode(drpLocalidadEmple.SelectedValue));
                }

                if (!String.IsNullOrEmpty(NoSQLCode(drpEmpresaEmple.SelectedValue)))
                {
                    lhtEmpleB.Add("{Proveedor}", NoSQLCode(drpEmpresaEmple.SelectedValue));
                }


                lhtEmpleB.Add("{SegundoNombre}", NoSQLCode(txtSegundoNombreEmple.Text));
                //20161108 Se agrega campo de comentarios para cuando se marca al empleado para que se excluya de la sincronización
                if (ckbOmiteSincro.Checked == true)
                {
                    lhtEmpleB.Add("{Comentarios}", NoSQLCode(txtComentariosSincro.Text));
                }
                else { lhtEmpleB.Add("{Comentarios}", string.Empty); }

                lhtEmpleB.Add("dtIniVigencia", Convert.ToDateTime(phtValuesEmple["dtIniVigencia"]));
                lhtEmpleB.Add("iCodUsuario", HttpContext.Current.Session["iCodUsuario"]);
                lhtEmpleB.Add("dtFecUltAct", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                dalcc.ActualizaEmpleB(lhtEmpleB, DALCCustodia.getiCodCatHist(iCodCatalogoEmple, "EmpleB", "Empleados B", "Emple", "iCodCatalogo"));

                //RM 20190513  Codigo para Actualizar datos en Emple FCA
                //Solo si se trata del cliente FCA ya que la vista no existe para otros clientes               

                if (DSODataContext.Schema.ToUpper() == "FCA")
                {
                    int iCodCatEmple = BuscarEmpleEnHistEmple();
                    if (iCodCatEmple > 0)
                    {
                        dctEmpleFCA.Add("Dc_id", NoSQLCode(txtDatosEmpleFCADC_ID.Text));
                        dctEmpleFCA.Add("T_id", NoSQLCode(txtDatosEmpleFCAT_ID.Text));
                        dctEmpleFCA.Add("NickName", NoSQLCode(txtDatosEmpleFCANickName.Text));
                        dctEmpleFCA.Add("Estacion", NoSQLCode(txtDatosEmpleFCAEstacion.Text));
                        dctEmpleFCA.Add("Director", ddlDatosEmpleFCADirector.SelectedValue.ToString());
                        dctEmpleFCA.Add("esDirector", cbDatosEmpleFCAEsDirector.Checked.ToString());
                        dctEmpleFCA.Add("planta", ddlDatosEmpleFCAPlanta.SelectedValue.ToString());

                        ActualizarDatosFCA(dctEmpleFCA, iCodCatEmple);
                    }
                }

                /////////////////////////////////
                lhtCCust.Clear();
                lhtCCust.Add("iCodMaestro", DALCCustodia.getiCodMaestro("Cartas custodia", "CCustodia"));
                lhtCCust.Add("vchCodigo", "CCustodia " + NoSQLCode(txtFolioCCustodia.Text));
                lhtCCust.Add("vchDescripcion", lsNomCompleto + " (Folio:" + NoSQLCode(txtFolioCCustodia.Text) + ")");
                lhtCCust.Add("{Emple}", iCodCatalogoEmple);
                lhtCCust.Add("{ComentariosAdmin}", NoSQLCode(txtComentariosAdmin.Text));
                lhtCCust.Add("dtIniVigencia", Convert.ToDateTime(phtValuesEmple["dtIniVigencia"]));
                lhtCCust.Add("dtFinVigencia", Convert.ToDateTime(phtValuesEmple["dtFinVigencia"]));
                lhtCCust.Add("iCodUsuario", HttpContext.Current.Session["iCodUsuario"]);
                lhtCCust.Add("dtFecUltAct", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                dalcc.ActualizaCCustodia(lhtCCust, DALCCustodia.getiCodCatHist(iCodCatalogoEmple, "CCustodia", "Cartas custodia", "Emple", "iCodCatalogo"));

                //Mandar a actualizar la jerarquia y restricciones del empleado
                ActualizaJerarquiaRest(iCodCatalogoEmple);
                /*actualiza los presupuestos del empleado*/
                if (txtPrepMovil.Text != "" && txtPrepFija.Text != "" && txtPrepTemporal.Text != "")
                {
                    dalcc.ActualizaPresupuestosEmple(iCodCatalogoEmple, txtPrepMovil.Text, txtPrepFija.Text, txtPrepTemporal.Text);
                }

            }
        }

        protected void validaCambioCenCosEmple(string iCodCatCenCosActual)
        {
            DALCCustodia cambiosCC = new DALCCustodia();

            /*Buscar la relacion actual que tiene el empleado con CC para saber si es la misma*/
            lsbQuery.Length = 0;
            lsbQuery.Append("SELECT iCodRegistro \r");
            lsbQuery.Append("FROM [VisRelaciones('CentroCosto-Empleado','Español')] \r");
            lsbQuery.Append("WHERE Emple = " + iCodCatalogoEmple + " \r");
            lsbQuery.Append("and CenCos = " + iCodCatCenCosActual + " \r");
            lsbQuery.Append("and dtIniVigencia <> dtFinVigencia \r");
            lsbQuery.Append("and dtFinVigencia >= getdate() ");

            DataTable ldt = DSODataAccess.Execute(lsbQuery.ToString());

            //Si encontro un registro de relacion valido en la relacion entonces es la misma no cambio
            if (ldt.Rows.Count == 0)
            {
                cambiosCC.DarDeBajaRelEmpleCenCos(iCodCatalogoEmple, DateTime.Today.AddDays(-1));

                cambiosCC.AltaRelEmpleCenCos(iCodCatalogoEmple, iCodCatCenCosActual, DateTime.Today, Convert.ToDateTime(phtValuesEmple["dtFinVigencia"]));
            }
        }

        protected void validaCambioCenCosEmpleExtern(string iCodCatCenCosActual)
        {
            DALCCustodia cambiosCC = new DALCCustodia();

            /*Buscar la relacion actual que tiene el empleado con CC para saber si es la misma*/
            lsbQuery.Length = 0;
            lsbQuery.Append("SELECT iCodRegistro \r");
            lsbQuery.Append("FROM [VisRelaciones('FCA CentroCosto-Externo','Español')] WITH(NOLOCK)\r");
            lsbQuery.Append("WHERE Emple = " + iCodCatalogoEmple + " \r");
            lsbQuery.Append("and CenCos = " + iCodCatCenCosActual + " \r");
            lsbQuery.Append("and dtIniVigencia <> dtFinVigencia \r");
            lsbQuery.Append("and dtFinVigencia >= getdate() ");

            DataTable ldt = DSODataAccess.Execute(lsbQuery.ToString());

            //Si encontro un registro de relacion valido en la relacion entonces es la misma no cambio
            if (ldt.Rows.Count == 0)
            {
                cambiosCC.DarDeBajaRelExternoEmpleCenCos(iCodCatalogoEmple, DateTime.Today.AddDays(-1));

                cambiosCC.AltaRelExternoCenCos(iCodCatalogoEmple, iCodCatCenCosActual, DateTime.Today, Convert.ToDateTime(phtValuesEmple["dtFinVigencia"]));
            }
        }

        /*RZ.20130819 Se agregan validaciones para empleado, englobadas en una región*/
        #region Validaciones para Empleado

        protected bool ValidarEmpleado()
        {
            phtValuesEmple = ObtenerHashEmpleado();

            IncializaNomina();

            return ValidarVigencias()
                && ValidarCampos()
                && ValidarClaves()
                && ValidaDatoEmpleados();
        }

        protected Hashtable ObtenerHashEmpleado()
        {
            Hashtable lht = new Hashtable();
            DateTime ldtFechaInicio;
            DateTime.TryParse(txtFecha.Text, out ldtFechaInicio);

            if (!String.IsNullOrEmpty(drpCenCosEmple.SelectedValue))
            {
                lht.Add("{CenCos}", int.Parse(drpCenCosEmple.SelectedValue));  //iCodCatalogo01
            }

            var tipoE = (String.IsNullOrEmpty(drpTipoEmpleado.SelectedValue)) ? "0" : drpTipoEmpleado.SelectedValue.ToString();
            lht.Add("{TipoEm}", tipoE);
            var puestoE = (String.IsNullOrEmpty(drpPuestoEmple.SelectedValue)) ? "0" : drpPuestoEmple.SelectedValue.ToString();
            lht.Add("{Puesto}", puestoE);
            //var JefeEm= (String.IsNullOrEmpty(drpJefeEmple.SelectedValue)) ? "0" : drpJefeEmple.SelectedValue.ToString();
            var JefeEm = (string.IsNullOrEmpty(hdfJefeEmple.Value.ToString())) ? "0" : hdfJefeEmple.Value.ToString();
            lht.Add("{Emple}", JefeEm);
            //if (String.IsNullOrEmpty(drpTipoEmpleado.SelectedValue))
            //{
            //    lht.Add("{TipoEm}", "0"); //iCodCatalogo02
            //}

            //if (String.IsNullOrEmpty(drpPuestoEmple.SelectedValue))
            //{
            //    lht.Add("{Puesto}", "0");  //iCodCatalogo03
            //}

            //if (String.IsNullOrEmpty(drpJefeEmple.SelectedValue))
            //{
            //    lht.Add("{Emple}", "0");
            //}

            //No se capturo un usuario para el empleado
            if (!String.IsNullOrEmpty(txtUsuarRedEmple.Text))
            {
                //Buscar si el usuario asignado ya existe, si no debe ser creado
                int liCodCatUsuar = obtenCatalogoUsuarioAsignado(txtUsuarRedEmple.Text);

                //Saber si el usuario existe ligarlo al empleado
                if (liCodCatUsuar > 0)
                {
                    lht.Add("{Usuar}", liCodCatUsuar.ToString());
                }
                else
                {
                    //Se creará nuevo usuario
                    lht.Add("{Usuar}", "null");
                }

                //En caso de estar en edicion dar de baja el usuario anterior
                AsegurarBajaUsuarioEnEdit(liCodCatUsuar);
            }
            else
            {


                //no se escribio un usuario
                lht.Add("{Usuar}", "null");
                //En caso de estar en edicion dar de baja el usuario anterior
                AsegurarBajaUsuarioEnEdit(-1);
            }

            lht.Add("{TipoPr}", "null");
            lht.Add("{PeriodoPr}", "null");
            lht.Add("{Organizacion}", "null");
            lht.Add("{OpcCreaUsuar}", "0");   //{Integer01}

            int liBanderasEmple = 0;

            if (cbVisibleDirEmple.Checked == true)
            {
                NuevoEmpleadoBackend.getValBandera("EmpleVisibleEnDirect", ref liBanderasEmple);

            }

            if (cbEsGerenteEmple.Checked == true)
            {
                NuevoEmpleadoBackend.getValBandera("EmpleEsGerente", ref liBanderasEmple);

            }

            //20161108 NZ Se agrega un valor de bandera: Omitir Emple de sincronización
            if (ckbOmiteSincro.Checked == true)
            {
                NuevoEmpleadoBackend.getValBandera("EmpleOmitirDeSincro", ref liBanderasEmple);
            }

            //RM Bandera esDirecctor
            if (cbDatosEmpleFCAEsDirector.Checked == true)
            {
                NuevoEmpleadoBackend.getValBandera("EmpleEsDirector", ref liBanderasEmple);
            }

            lht.Add("{BanderasEmple}", liBanderasEmple);
            lht.Add("{PresupFijo}", txtPrepFija.Text);
            lht.Add("{PresupMovil}", txtPrepMovil.Text);
            lht.Add("{PresupProv}", "null");
            lht.Add("{Nombre}", txtNombreEmple.Text);  //VarChar01
            lht.Add("{Paterno}", txtApPaternoEmple.Text); //VarChar02
            lht.Add("{Materno}", txtApMaternoEmple.Text); //VarChar03
            lht.Add("{RFC}", "null");
            lht.Add("{Email}", txtEmailEmple.Text);

            if (DSODataContext.Schema.ToString().ToUpper() == "BAT")
            {
                lht.Add("{Ubica}", txtLocalidadEmple.Text);
            }
            else
            {
                if (!String.IsNullOrEmpty(drpLocalidadEmple.SelectedValue))
                {
                    lht.Add("{Ubica}", drpLocalidadEmple.SelectedItem.Text);
                }
            }

            if (txtNominaEmple.Text != string.Empty)
            {
                lht.Add("{NominaA}", txtNominaEmple.Text); //VarChar07
            }
            else
            {
                lht.Add("{NominaA}", String.Empty);
            }

            lht.Add("dtIniVigencia", ldtFechaInicio);
            lht.Add("iCodUsuario", (int)HttpContext.Current.Session["iCodUsuario"]);
            lht.Add("dtFecUltAct", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));



            if (estado == "alta" || String.IsNullOrEmpty(estado))
            {
                lht.Add("dtFinVigencia", new DateTime(2079, 1, 1));
            }

            if (estado == "edit")
            {
                DateTime ldtFinVigencia = Convert.ToDateTime(hdnFechaFinEmple.Value);
                lht.Add("dtFinVigencia", ldtFinVigencia);
            }

            return lht;
        }

        /*RZ.20130807 Se agrega metodo que asegurar de baja del usuario en caso de haber tenido
         * uno asignado previamente */
        protected void AsegurarBajaUsuarioEnEdit(int iCodCatUsuarAsignado)
        {
            DALCCustodia usuar = new DALCCustodia();
            if (estado == "edit" && iCodCatalogoEmple != String.Empty && validaUsuarPrevio(iCodCatUsuarAsignado))
            {
                usuar.DarDeBajaUsuario(iCodCatalogoEmple, DateTime.Today);
            }
        }

        /*Validar si el empleado en edicion tenia un usuario anteriormente ligado*/
        protected bool validaUsuarPrevio(int iCodCatUsuarioAsignado)
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

        //protected void getValBandera(string lsCodBandera, ref int liValBanderas)
        //{
        //    string lsValorBandera = String.Empty;
        //    StringBuilder lsbConsulta = new StringBuilder();

        //    lsbConsulta.Append("SELECT Value \r");
        //    lsbConsulta.Append("FROM [VisHistoricos('Valores','Valores','Español')] \r");
        //    lsbConsulta.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
        //    lsbConsulta.Append("and dtFinVigencia >= GETDATE() \r");
        //    lsbConsulta.Append("and dtFinVigencia >= GETDATE() \r");
        //    lsbConsulta.Append("and vchCodigo = '" + lsCodBandera + "' \r");

        //    lsValorBandera = DSODataAccess.ExecuteScalar(lsbConsulta.ToString()).ToString();

        //    if (!String.IsNullOrEmpty(lsValorBandera))
        //    {
        //        liValBanderas += int.Parse(lsValorBandera);
        //    }
        //}

        protected int obtenCatalogoUsuarioAsignado(string vchCodigoUsuar)
        {
            StringBuilder lsbQueryUsuar = new StringBuilder();

            lsbQueryUsuar.Append("SELECT icodcatalogo \r");
            lsbQueryUsuar.Append("FROM [VisHistoricos('Usuar','Usuarios','Español')] \r");
            lsbQueryUsuar.Append("WHERE dtInivigencia <> dtFinVigencia \r");
            lsbQueryUsuar.Append("and dtFinvigencia >= GETDATE() \r");
            lsbQueryUsuar.Append("and usuardb = " + Session["iCodUsuarioDB"].ToString() + " \r");
            lsbQueryUsuar.Append("and vchcodigo = '" + vchCodigoUsuar + "' \r");

            DataRow ldr = DSODataAccess.ExecuteDataRow(lsbQueryUsuar.ToString());

            if (ldr != null)
            {
                return (int)ldr["iCodCatalogo"];
            }

            return -1;
        }

        protected void IncializaNomina()
        {
            //Obten el numero de nomina si no se capturo
            string lsValue = txtNominaEmple.Text;
            if (lsValue == "")
            {
                lsValue = ObtenNumeroNomina();
                if (lsValue != "null")
                {
                    phtValuesEmple["{NominaA}"] = lsValue;
                }
            }
        }

        protected string ObtenNumeroNomina()
        {
            DataTable ldt;
            DataRow[] ldr;

            string lsNomina = "null";
            string lsTipoEm = "";
            string lsColumField = "";
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

        protected bool ValidarVigencias()
        {
            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            string lsError;
            string lsTitulo = DSOControl.JScriptEncode("Empleados");
            pjsObj = "HistoricEdit1";

            DateTime pdtIniVigencia = Convert.ToDateTime(phtValuesEmple["dtIniVigencia"].ToString());

            DateTime pdtFinVigencia = Convert.ToDateTime(phtValuesEmple["dtFinVigencia"].ToString());

            try
            {
                if (String.IsNullOrEmpty(txtFecha.Text))
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Fecha Inicio"));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                //Validar que fin de vigencia sea mayor o igual a inicio de vigencia
                if (pdtIniVigencia != null && pdtFinVigencia != null && pdtIniVigencia > pdtFinVigencia)
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "VigenciaFin", "Fecha Inicio", "Fecha Fin"));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                if (lsbErrores.Length > 0)
                {
                    lbret = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }

            return lbret;
        }

        protected virtual bool ValidarCampos()
        {

            /*Extraer el icodregistro de la entidad de Empleados*/
            //string iCodEntidad = "6";
            /*Extraer el icodregistro del maestro Empleados*/
            string iCodMaestro = DALCCustodia.getiCodMaestro("Empleados", "Emple");

            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            DataRow lRowMaestro = DSODataAccess.ExecuteDataRow("select iCodRegistro from Maestros where iCodRegistro = " + iCodMaestro);

            string lsError;
            string lsTitulo = DSOControl.JScriptEncode("Empleados");

            try
            {
                if (String.IsNullOrEmpty(phtValuesEmple["{NominaA}"].ToString()))
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Nómina del Empleado"));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }
                if (String.IsNullOrEmpty(phtValuesEmple["{Nombre}"].ToString()))
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Nombre del Empleado"));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                if (DSODataContext.Schema.ToUpper() != "FCA")
                {
                    //if (!phtValuesEmple.ContainsKey("{CenCos}"))
                    //{
                    //    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RelacionRequerida", "Centro de Costos"));
                    //    lsbErrores.Append("<li>" + lsError + "</li>");
                    //}
                    if (phtValuesEmple["{CenCos}"].ToString() == "0")
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RelacionRequerida", "Centro de Costos"));
                        lsbErrores.Append("<li>" + lsError + "</li>");
                    }
                    if (String.IsNullOrEmpty(phtValuesEmple["{Ubica}"].ToString()))
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Ubicacion del Empleado"));
                        lsbErrores.Append("<li>" + lsError + "</li>");
                    }

                    if (phtValuesEmple["{Emple}"].ToString() == "0")
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Jefe Inmediato"));
                        lsbErrores.Append("<li>" + lsError + "</li>");
                    }
                    if (String.IsNullOrEmpty(phtValuesEmple["{PresupMovil}"].ToString()))
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Presupuesto Móvil"));
                        lsbErrores.Append("<li>" + lsError + "</li>");
                    }
                    if (String.IsNullOrEmpty(phtValuesEmple["{PresupFijo}"].ToString()))
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Presuesto Fija"));
                        lsbErrores.Append("<li>" + lsError + "</li>");
                    }
                }


                if (!phtValuesEmple.ContainsKey("{TipoEm}"))
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Tipo de Empleado"));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                if (!phtValuesEmple.ContainsKey("{Puesto}"))
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Puesto de Empleado"));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                if (DSODataContext.Schema.ToUpper() == "FCA")
                {
                    if (Convert.ToInt32(ddlDatosEmpleFCADpto.SelectedValue) <= 0)
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Departamento"));
                        lsbErrores.Append("<li>" + lsError + "</li>");
                    }


                    if (txtDatosEmpleFCAT_ID.Text.Length <= 0)
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "T_ID"));
                        lsbErrores.Append("<li>" + lsError + "</li>");
                    }
                }



                //20161108 NZ Se agrega validacion de que si la bandera de omitir de la sincronización esta encendida entonces es necesario que
                //se introduzca un comentario para especificar el motivo.
                if (ckbOmiteSincro.Checked && ckbOmiteSincro.Visible == true && string.IsNullOrEmpty(txtComentariosSincro.Text.Trim()))
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Comentarios de Omitir de Sincronización"));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                if (lsbErrores.Length > 0)
                {
                    lbret = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
                return lbret;
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
        }

        protected bool ValidarClaves()
        {
            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            StringBuilder lsbErroresCodigos = new StringBuilder();
            StringBuilder psbQuery = new StringBuilder();
            /*Extraer el icodregistro de la entidad de Empleados*/
            string iCodEntidad = "6";

            string lsError;
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloHistoricos"));
            DataTable ldt;

            IncializaCampos();

            string liCodEmpresa = "0";
            int liCodCatalogo = 0;

            if (DSODataContext.Schema.ToUpper() != "FCA")
            {
                int.TryParse(phtValuesEmple["{CenCos}"].ToString(), out liCodCatalogo);
            }
            else
            {
                int.TryParse(ddlDatosEmpleFCADpto.SelectedValue.ToString(), out liCodCatalogo);
            }


            psbQuery.Length = 0;
            psbQuery.AppendLine("Select Empre");
            psbQuery.AppendLine("from [VisHistoricos('CenCos','" + Globals.GetCurrentLanguage() + "')]");
            psbQuery.AppendLine("where iCodCatalogo = " + liCodCatalogo.ToString());

            ldt = DSODataAccess.Execute(psbQuery.ToString());

            if (ldt == null || ldt.Rows.Count == 0 || ldt.Rows[0]["Empre"] is DBNull)
            {
                string mensajeCCNoValido = "No se ha encontrado una empresa para el centro de costo. Elija un centro de costo válido";
                if (DSODataContext.Schema.ToUpper() == "FCA")
                {
                    mensajeCCNoValido = "No se ha encontrado una empresa para el departamento. Elija un departamento válido"; ;
                }

                lsbErrores.Append("<li>" + mensajeCCNoValido + "</li>");

            }
            else
            {
                liCodEmpresa = ldt.Rows[0]["Empre"].ToString();
            }

            try
            {

                if (!String.IsNullOrEmpty(phtValuesEmple["{NominaA}"].ToString()))
                {
                    //Valida el numero de Nomina que no se repita a menos que sea de diferente empresa
                    psbQuery.Length = 0;
                    psbQuery.AppendLine("select H.iCodRegistro, H.iCodCatalogo, H.iCodMaestro, H.dtIniVigencia, H.dtFinVigencia");
                    psbQuery.AppendLine("from [" + DSODataContext.Schema + "].Historicos H, [" + DSODataContext.Schema + "].Catalogos C,");
                    psbQuery.AppendLine("   [" + DSODataContext.Schema + "].[VisHistoricos('Emple','" + Globals.GetCurrentLanguage() + "')] V");
                    psbQuery.AppendLine("where H.iCodRegistro = V.iCodRegistro");
                    psbQuery.AppendLine("and H.iCodCatalogo = C.iCodRegistro");
                    if (estado == "edit")
                    {
                        psbQuery.AppendLine("and H.iCodRegistro <> isnull(" + iCodCatalogoEmple + ",-1)"); //RZ. ojo con este campo validar
                        psbQuery.AppendLine("and H.iCodCatalogo <> isnull(" + iCodCatalogoEmple + ",-1)");
                    }


                    psbQuery.AppendLine("and C.iCodCatalogo = " + iCodEntidad);
                    psbQuery.AppendLine("and C.vchCodigo = '" + phtValuesEmple["{NominaA}"].ToString() + "'");
                    psbQuery.AppendLine("and V.CenCos in (Select iCodCatalogo from ");
                    psbQuery.AppendLine("   [" + DSODataContext.Schema + "].[VisHistoricos('CenCos','Español')] VC ");
                    psbQuery.AppendLine("               Where Empre =  " + liCodEmpresa + ")");
                    psbQuery.AppendLine("and H.dtIniVigencia <> H.dtFinVigencia");
                    psbQuery.AppendLine("and ((H.dtIniVigencia <= '" + Convert.ToDateTime(phtValuesEmple["dtIniVigencia"]).ToString("yyyy-MM-dd hh:mm:ss.fff") + "'");
                    psbQuery.AppendLine("       and H.dtFinVigencia > '" + Convert.ToDateTime(phtValuesEmple["dtIniVigencia"]).ToString("yyyy-MM-dd hh:mm:ss.fff") + "'" + ")");
                    psbQuery.AppendLine("   or (H.dtIniVigencia <= '" + Convert.ToDateTime(phtValuesEmple["dtFinVigencia"]).ToString("yyyy-MM-dd hh:mm:ss.fff") + "'");
                    psbQuery.AppendLine("       and H.dtFinVigencia > '" + Convert.ToDateTime(phtValuesEmple["dtFinVigencia"]).ToString("yyyy-MM-dd hh:mm:ss.fff") + "'" + ")");
                    psbQuery.AppendLine("   or (H.dtIniVigencia >= '" + Convert.ToDateTime(phtValuesEmple["dtIniVigencia"]).ToString("yyyy-MM-dd hh:mm:ss.fff") + "'");
                    psbQuery.AppendLine("       and H.dtFinVigencia <= '" + Convert.ToDateTime(phtValuesEmple["dtFinVigencia"]).ToString("yyyy-MM-dd hh:mm:ss.fff") + "'" + "))");
                    psbQuery.AppendLine("order by H.dtIniVigencia, H.dtFinVigencia, H.iCodRegistro");

                    ldt = DSODataAccess.Execute(psbQuery.ToString());

                    string lsNetDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");
                    foreach (DataRow ldataRow in ldt.Rows)
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", ((DateTime)ldataRow["dtIniVigencia"]).ToString(lsNetDateFormat), ((DateTime)ldataRow["dtFinVigencia"]).ToString(lsNetDateFormat)));
                        lsbErroresCodigos.Append("<li>" + lsError + "</li>");
                    }


                    if (lsbErroresCodigos.Length > 0)
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValidaVchCodigo", "Clave para Empleado (nómina)"));
                        lsError = "<span>" + lsError + "</span>";
                        lsbErrores.Append("<li>" + lsError);
                        lsbErrores.Append("<ul>" + lsbErroresCodigos.ToString() + "</ul>");
                        lsbErrores.Append("</li>");
                    }
                }

                else
                {

                    lsbErrores.Append("<li>" + "El campo de Nómina es requerido" + "</li>");
                }

                if (String.IsNullOrEmpty(phtValuesEmple["vchDescripcion"].ToString()))
                {
                    lsbErrores.Append("<li>" + "La descripción del empleado no se ha generado correctamente" + "</li>");
                }

                if (lsbErrores.Length > 0)
                {
                    lbret = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }

            return lbret;
        }

        protected void IncializaCampos()
        {
            //Obten el numero de nomina si no se capturo
            //string lsValue = "";
            DataTable ldt;
            StringBuilder psbQuery = new StringBuilder();

            //Incializa los valores de Codigo y Descripcion del Historico de Empleados.
            phtValuesEmple.Add("vchCodigo", phtValuesEmple["{NominaA}"].ToString());


            string lsNomEmpleado = txtNombreEmple.Text.Trim() + " " + txtSegundoNombreEmple.Text.Trim() + " " +
                          txtApPaternoEmple.Text.Trim() + " " + txtApMaternoEmple.Text.Trim();

            if (DSODataContext.Schema.ToString().ToUpper() == "FCA")
            {
                lsNomEmpleado = txtApPaternoEmple.Text.Trim() + " " + txtApMaternoEmple.Text.Trim() + " " +
                    txtNombreEmple.Text.Trim() + " " + txtSegundoNombreEmple.Text.Trim();

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

        //RZ.20130729 Se agrega metodo para validar la entrada de los datos
        protected bool ValidaDatoEmpleados()
        {
            bool lbRet = true;
            string lsError;
            StringBuilder lsbErrores = new StringBuilder();
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloEmpleados"));

            string lsValue;

            lsbErrores.Length = 0;

            // Valida el numero de Nomina
            lsValue = phtValuesEmple["{NominaA}"].ToString();
            if (lsValue.Length > 40 ||
                !System.Text.RegularExpressions.Regex.IsMatch(lsValue, "^([a-zA-Z]*[0-9]*[-]*[/]*[_]*[:]*[.]*[|]*)*$"))
            {
                lsError = GetMsgError("Nómina", "ValEmplFormato");
                lsbErrores.Append("<li>" + lsError);
            }

            // Valida el Nombre
            lsValue = phtValuesEmple["{Nombre}"].ToString();
            if (lsValue == "" || lsValue.Contains(","))
            {
                lsError = GetMsgError("Nombre", "ValEmplFormato");
                lsbErrores.Append("<li>" + lsError);
            }

            // Valida el Segundo Nombre
            lsValue = txtSegundoNombreEmple.Text;
            if (lsValue.Contains(","))
            {
                lsError = GetMsgError("Segundo Nombre", "ValEmplFormato");
                lsbErrores.Append("<li>" + lsError);
            }

            // Valida el Apellido Paterno
            lsValue = phtValuesEmple["{Paterno}"].ToString();
            if (lsValue.Contains(","))
            {
                lsError = GetMsgError("Apellido Paterno", "ValEmplFormato");
                lsbErrores.Append("<li>" + lsError);
            }

            // Valida el Apellido Materno
            lsValue = phtValuesEmple["{Materno}"].ToString();
            if (lsValue.Contains(","))
            {
                lsError = GetMsgError("Apellido Materno", "ValEmplFormato");
                lsbErrores.Append("<li>" + lsError);
            }

            // Valida la cuenta de correo Formato
            lsValue = phtValuesEmple["{Email}"].ToString();
            if (lsValue.Length > 0)
            {
                //RM 20190514 Expresion que se usaba anteriormente
                /*
                 * ^(([\\w-]+\\.)+[\\w-]+|([a-zA-Z]{1}|[\\w-]{2,}))" + "@" +
                    "((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\\.+([0-1]?[0-9]{1,2}|25[0-5]" +
                    "|2[0-4][0-9])\\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z]+[\\w-]+\\.)+[a-zA-Z]{2,4})$
                 */


                string pattern = @"^([^(áéíóúÁÉÍÓÚ()<>@,;:\[\] ç % &]+)(@)([^ áéíóúÁÉÍÓÚ() <>@,;:\[\]ç%&]{3,})([.][\w]{2,}){1,3}$";
                if (!System.Text.RegularExpressions.Regex.IsMatch(lsValue, pattern))
                {
                    lsError = GetMsgError("E-mail", "ValEmplFormato");
                    lsbErrores.Append("<li>" + lsError);
                }
            }
            // No se puede asignar al mismo empleado como responsable
            if (IsRespEmpleadoSame())
            {
                lsError = GetMsgError("Jefe Inmediato", "ErrEmplRespSame");
                lsbErrores.Append("<li>" + lsError);
            }
            // Valida si se captura el jefe debe ser un empleado
            if (phtValuesEmple.Contains("{Emple}"))
            {
                lsValue = phtValuesEmple["{Emple}"].ToString();

                // Es empleado debe ser un empleado
                // NZ 20160608 Se quita esta validación a peticion de RJ
                //if (IsEmpleado() && !IsRespEmpleado())
                //{
                //    lsError = GetMsgError("Jefe Inmediato", "ValJefeEmpleado");
                //    lsbErrores.Append("<li>" + lsError);
                //}
                // Es externo debe asignarsele un responsable que sea empleado o externo
                if (IsExterno() && !IsRespEmpleadoExterno())
                {
                    lsError = GetMsgError("Jefe Inmediato", "ValJefeEmplExt");
                    lsbErrores.Append("<li>" + lsError);
                }
            }
            //else
            //{
            //    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Jefe Inmediato"));
            //    lsbErrores.Append("<li>" + lsError + "</li>");
            //}

            //Validar que el usuario no este asignado a otro empleados
            lsError = UsuarioAsignado();

            if (lsError.Length > 0)
            {
                lsError = GetMsgError("Usuario", lsError);
                lsbErrores.Append("<li>" + lsError);
            }

            string lsvchCodUsuar = txtUsuarRedEmple.Text;
            DateTime ldtFinVigencia = Convert.ToDateTime(phtValuesEmple["dtFinVigencia"].ToString());

            //Si no hay errores entonces crear usuario, en caso de que no exista ya
            //RZ.20131201 Se retira llamada al metodo que valida si el empleado es tipo empleado para crear usuario && IsEmpleado()
            if (lsbErrores.Length == 0 && lsvchCodUsuar != String.Empty && phtValuesEmple["{Usuar}"].ToString() == "null")
            {
                if (ldtFinVigencia.Date > DateTime.Today)
                {
                    lsError = GeneraUsuario();
                    if (lsError != "")
                    {
                        lsError = GetMsgError("Usuario", lsError);
                        lsbErrores.Append("<li>" + lsError);
                    }
                }
            }


            if (lsbErrores.Length > 0)
            {
                lbRet = false;
                lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
            }

            return lbRet;
        }

        protected bool IsRespEmpleadoSame()
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

        protected string GetMsgError(string lsDesCampo, string lsMsgError)
        {
            string lsError = "";
            string lsValue = "";

            lsValue = lsDesCampo;

            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, lsMsgError, lsValue));
            lsError = "<span>" + lsError + "</span>";

            return lsError;
        }

        protected bool IsEmpleado()
        {
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
                                && ldt.Rows[0]["vchCodigo"].ToString() == "E")
            {
                lbRet = true;
            }

            return lbRet;
        }

        protected bool IsRespEmpleado()
        {
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
            //AM 20131205. Se valida que el jefe este vigente.
            lsbQuery.AppendLine("and dtFinVigencia >= getdate() ");
            //lsbQuery.AppendLine("and '" + ldtIniVigencia.Date.ToString("yyyy-MM-dd") + "' >= dtIniVigencia");
            //lsbQuery.AppendLine("and '" + ldtIniVigencia.Date.ToString("yyyy-MM-dd") + "' < dtFinVigencia");

            DataTable ldt = DSODataAccess.Execute(lsbQuery.ToString());
            if (ldt == null || ldt.Rows.Count == 0 || ldt.Rows[0]["TipoEmCod"] is DBNull)
            {
                return lbRet;
            }

            if (ldt.Rows[0]["TipoEmCod"].ToString() == "E")
            {
                lbRet = true;
            }

            return lbRet;
        }

        protected bool IsExterno()
        {
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

        protected bool IsRespEmpleadoExterno()
        {
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

        protected string UsuarioAsignado()
        {
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

        protected string GeneraUsuario()
        {
            //Crea el usuario deacuerdo a lo seleccionado por el usuario
            psNewUsuario = txtUsuarRedEmple.Text;

            if (psNewUsuario == String.Empty)
            {
                return "ErrCrearUsuario";
            }

            //'Crea el usuario deacuerdo a lo seleccionado por el usuario
            psNewPassword = ObtenPassword();

            string lsError = ExiUsuarioEmailPassword();

            if (lsError != "")
            {
                return lsError;
            }
            int liCodRegistro = GrabarUsuario();
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


        //protected string GeneraUsuarioFCA(string vchCodigoFCA, string PassFCA)
        //{
        //    //Crea el usuario deacuerdo a lo seleccionado por el usuario
        //    psNewUsuario = vchCodigoFCA;

        //    if (psNewUsuario == String.Empty)
        //    {
        //        return "ErrCrearUsuario";
        //    }

        //    //'Crea el usuario deacuerdo a lo seleccionado por el usuario
        //    psNewPassword = PassFCA;

        //    string lsError = ExiUsuarioEmailPassword();

        //    if (lsError != "")
        //    {
        //        return lsError;
        //    }
        //    int liCodRegistro = GrabarUsuario();
        //    if (liCodRegistro > 0)
        //    {
        //        // Asigna el nuevo Usuario.
        //        phtValuesEmple["{Usuar}"] = liCodRegistro.ToString();

        //        DALCCustodia guardaUsuarBitacora = new DALCCustodia();

        //        guardaUsuarBitacora.guardaHistRecurso(liCodRegistro.ToString(), "Usu", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), "A");
        //    }
        //    else
        //    {
        //        return "ErrCrearUsuario";
        //    }

        //    return "";

        //}

        //protected string BuscaUsuario(string vchCodigoFCA)
        //{
        //    string usuar = "";

        //    StringBuilder query = new StringBuilder();

        //    query.AppendLine("declare @vchCodigoUsuar varchar(40) = '"+ vchCodigoFCA + "'                                   ");
        //    query.AppendLine("                                                                                        ");
        //    query.AppendLine("Select iCodCatalogo                                                                     ");
        //    query.AppendLine("From["+DSODataContext.Schema+"].[VisHistoricos('Usuar', 'Usuarios', 'Español')] usuar   ");
        //    query.AppendLine("Where dtIniVigencia<> dtFinVigencia                                                     ");
        //    query.AppendLine("And dtFinVigencia >= GETDATE()                                                          ");
        //    query.AppendLine("And vchCodigo = @vchCodigoUsuar                                                         ");

        //    return usuar;
        //}

        //protected string ModificarPassWordUsuarFCA(int iCodCatUsuar, string vchCodigoFCA, string PassFCA)
        //{
        //    string res = "";

        //    StringBuilder query = new StringBuilder ();

        //    query.AppendLine("Declare @newPassEncriptado varchar(300) = '"+PassFCA+"'			");
        //    query.AppendLine("Declare @iCodCatUsuar int ="+iCodCatUsuar+"   					");
        //    query.AppendLine("Declare @vchCodigo varchar(300) ='" + vchCodigoFCA + "'			");
        //    query.AppendLine("																	");
        //    query.AppendLine("Update usuar														");
        //    query.AppendLine("Set 																");
        //    query.AppendLine("	Password = @newPassEncriptado,									");
        //    query.AppendLine("	ConfPassword = @newPassEncriptado,								");
        //    query.AppendLine("	dtFecUltAct = GETDATE()											");
        //    query.AppendLine("From FCA.[VisHistoricos('Usuar','Usuarios','Español')] usuar		");
        //    query.AppendLine("Where dtIniVigencia <> dtFinVigencia								");
        //    query.AppendLine("And dtFinVigencia >= GETDATE()									");
        //    query.AppendLine("And vchCodigo = @vchCodigo                                        ");
        //    query.AppendLine("And iCodCatalogo = @iCodCatUsuar									");

        //    return res;
        //}

        //protected string AltaOModifUsuarioFCA(string vchCodigoFCA, string PassFCA)
        //{
        //    string res = "false";
        //    int iCodCatUsuar = 0;
        //    PassFCA = Util.Encrypt(PassFCA);


        //    int.TryParse(BuscaUsuario(vchCodigoFCA),out iCodCatUsuar);

        //    if (iCodCatUsuar == 0)
        //    {
        //        int.TryParse(GeneraUsuarioFCA(vchCodigoFCA, PassFCA), out iCodCatUsuar);
        //    }

        //    if (iCodCatUsuar > 0)
        //    {
        //        ModificarPassWordUsuarFCA(iCodCatUsuar, vchCodigoFCA, PassFCA);
        //    }



        //    return res;
        //}

        protected string ObtenPassword()
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

        protected string ExiUsuarioEmailPassword()
        {
            String lbret = "";

            Usuarios oUsuario = new Usuarios();

            oUsuario.vchEmail = phtValuesEmple["{Email}"].ToString();
            oUsuario.vchCodUsuario = psNewUsuario;
            oUsuario.vchPwdUsuario = psNewPassword;

            lbret = oUsuario.ValUsuarioEmailPassword();

            return lbret;
        }

        protected int GrabarUsuario()
        {
            Hashtable lhtValues;
            try
            {
                KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
                int liCodRegistro = 0;
                lhtValues = ObtenDatosUsuario();

                //Mandar llamar al COM para grabar el usuario 
                liCodRegistro = lCargasCOM.GuardaUsuario(lhtValues, false, false, (int)Session["iCodUsuarioDB"]);

                return liCodRegistro;

            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveRecord", ex);
            }
        }

        protected Hashtable ObtenDatosUsuario()
        {
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
            lhtValues.Add("iCodUsuario", Session["iCodUsuario"]);

            lhtValues.Add("{Email}", "'" + lsEmail + "'");
            lhtValues.Add("{UsuarDB}", Session["iCodUsuarioDB"]);

            /*RZ.20130801 Se retira este homepage y se pone el que se requiere para que 
             * el usuario entre directo a la Carta Custodia */
            //LAAV20210924 Correcion rapida para compania bat con diferente url, sugerencia corregir para evitar hardcoreado 
            string HomePage;
            HomePage = DSODataContext.Schema.ToUpper().Equals("BAT") ?
                "'~/UserInterface/DashboardFC/DashboardConsumoIndiv.aspx'":
                "'~/UserInterface/Dashboard/Dashboard.aspx?Opc=OpcdshEmpleado'";

            lhtValues.Add("{HomePage}", HomePage);

            /*RZ.20131106 Se deja como perfil default el tipo empleado */
            ldt = pKDB.GetHisRegByEnt("Perfil", "Perfiles", "vchCodigo ='Epmpl' ");
            if (ldt != null && !(ldt.Rows[0]["iCodCatalogo"] is DBNull))
            {
                liCodPerfil = (int)ldt.Rows[0]["iCodCatalogo"];
                lhtValues.Add("{Perfil}", liCodPerfil);
            }

            lhtValues.Add("{Password}", "'" + psNewPassword + "'");
            lhtValues.Add("{ConfPassword}", "'" + psNewPassword + "'");

            ldt = pKDB.GetHisRegByEnt("Usuar", "Usuarios", "iCodCatalogo =" + Session["iCodUsuario"]);
            if (ldt != null && !(ldt.Rows[0]["{Empre}"] is DBNull))
            {
                lhtValues.Add("{Empre}", ldt.Rows[0]["{Empre}"]);
            }

            return lhtValues;
        }

        #region Bandera Usuario Pendiente

        //20140602 AM. Se cambia el texto del control txtUsuarRedEmple dependiendo del estado de la bandera de usuario pendiente.
        protected void chkUsuarioPendiente_OnCheckedChanged(Object sender, EventArgs args)
        {
            if (chkUsuarioPendiente.Checked)
            {
                txtUsuarRedEmple.Text = getUsuarioPendiente();
            }
            else
            {
                txtUsuarRedEmple.Text = "";
            }
        }

        //20140602 AM. Se agrega metodo para generar nombre de usuario pendiente.
        protected string getUsuarioPendiente()
        {
            try
            {
                string usuario = "Usuario";

                StringBuilder lsbConsulta = new StringBuilder();
                lsbConsulta.AppendLine(" SELECT ISNULL(Max(Numero),0) AS Numero FROM ( ");
                lsbConsulta.AppendLine(" SELECT CONVERT(INT, REPLACE(vchCodigo,'Usuario','')) AS Numero ");
                lsbConsulta.AppendLine(" FROM [VisHistoricos('Usuar','Usuarios','Español')] ");
                lsbConsulta.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= getdate() ");
                lsbConsulta.AppendLine("    AND (vchCodigo LIKE 'Usuario[0-9]' ");
                lsbConsulta.AppendLine("    OR vchCodigo LIKE 'Usuario[0-9][0-9]' ");
                lsbConsulta.AppendLine("    OR vchCodigo LIKE 'Usuario[0-9][0-9][0-9]' ");
                lsbConsulta.AppendLine("    OR vchCodigo LIKE 'Usuario[0-9][0-9][0-9][0-9]' ");
                lsbConsulta.AppendLine("    OR vchCodigo LIKE 'Usuario[0-9][0-9][0-9][0-9][0-9]') ");
                lsbConsulta.AppendLine(" ) AS Rep \r");

                int numSiguiente = int.Parse(DSODataAccess.ExecuteScalar(lsbConsulta.ToString()).ToString());

                return usuario += (numSiguiente + 1).ToString();
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al intentar generar un usuario pendiente en metodo getUsuarioPendiente '" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                throw ex;
            }
        }


        #endregion

        #region Bandera de Omitir de Sincronización
        protected void ckbOmiteSincro_OnCheckedChanged(Object sender, EventArgs args)
        {
            if (ckbOmiteSincro.Checked)
            {
                txtComentariosSincro.ReadOnly = false;
                txtComentariosSincro.Visible = true;
                txtComentariosSincro.Enabled = true;
            }
            else
            {
                txtComentariosSincro.ReadOnly = true;
                txtComentariosSincro.Visible = false;
                txtComentariosSincro.Enabled = false;
            }
        }
        #endregion

        #endregion

        /*RZ.20130721*/
        #region Metodos para guardar y ocultar botones en la seccion de empleados

        public void cargaBotonesEstadoAltaEmple()
        {
            lbtnSaveEmple.Enabled = true;
            lbtnSaveEmple.Visible = true;
            lbtnEditEmple.Enabled = false;
            lbtnEditEmple.Visible = false;
            lbtnDeleteEmple.Enabled = false;
            lbtnDeleteEmple.Visible = false;
            lbtnCancelarEmple.Enabled = false;
            lbtnCancelarEmple.Visible = false;
        }

        public void cargaBotonesEstadoEditEmple()
        {
            lbtnDeleteEmple.Text = "Borrar";

            lbtnSaveEmple.Enabled = false;
            lbtnSaveEmple.Visible = false;
            lbtnEditEmple.Enabled = true;
            lbtnEditEmple.Visible = true;
            lbtnDeleteEmple.Enabled = true;
            lbtnDeleteEmple.Visible = true;
            lbtnCancelarEmple.Enabled = false;
            lbtnCancelarEmple.Visible = false;
        }

        public void cargaBotonesModificarEmple()
        {
            lbtnSaveEmple.Enabled = true;
            lbtnSaveEmple.Visible = true;
            lbtnEditEmple.Enabled = false;
            lbtnEditEmple.Visible = false;
            lbtnDeleteEmple.Enabled = false;
            lbtnDeleteEmple.Visible = false;
            lbtnCancelarEmple.Enabled = true;
            lbtnCancelarEmple.Visible = true;

            //20140602 AM. Mostrar la bandera de usuario pendiente 
            if (txtUsuarRedEmple.Enabled)
            {
                chkUsuarioPendiente.Visible = true;
            }
        }

        public void cargaBotonesBorrarEmple()
        {
            lbtnDeleteEmple.Text = "Aceptar";

            lbtnSaveEmple.Enabled = false;
            lbtnSaveEmple.Visible = false;
            lbtnEditEmple.Enabled = false;
            lbtnEditEmple.Visible = false;
            lbtnDeleteEmple.Enabled = true;
            lbtnDeleteEmple.Visible = true;
            lbtnCancelarEmple.Enabled = true;
            lbtnCancelarEmple.Visible = true;
        }

        public void cargaBotonesCancelarEmple()
        {
            lbtnDeleteEmple.Text = "Borrar";

            lbtnSaveEmple.Enabled = false;
            lbtnSaveEmple.Visible = false;
            lbtnEditEmple.Enabled = true;
            lbtnEditEmple.Visible = true;
            lbtnDeleteEmple.Enabled = true;
            lbtnDeleteEmple.Visible = true;
            lbtnCancelarEmple.Enabled = false;
            lbtnCancelarEmple.Visible = false;

            //20140602 AM. No mostrar la bandera de usuario pendiente 
            chkUsuarioPendiente.Visible = false;
        }

        #endregion

        /*RZ.20130718 Habilita o deshabilita los controles de los datos de Empleados para edición */
        protected void enableAllWebControlsEmple()
        {
            enableTxtBox(txtNominaEmple);
            enableTxtBox(txtFecha);
            enableDDL(drpCenCosEmple);
            enableTxtBox(txtNombreEmple);
            enableTxtBox(txtSegundoNombreEmple);
            enableTxtBox(txtApPaternoEmple);
            enableTxtBox(txtApMaternoEmple);
            enableTxtBox(txtPrepMovil);
            enableTxtBox(txtPrepFija);
            enableTxtBox(txtPrepTemporal);
            enableDDL(drpLocalidadEmple);
            enableTxtBox(txtEmailEmple);
            enableTxtBox(txtUsuarRedEmple);
            //enableDDL(drpJefeEmple);
            enableTxtBox(txtJefeEmple);
            enableTxtBox(txtLocalidadEmple);
            //enableTxtBox(txtEmailJefeEmple);
            enableCheckBox(cbEsGerenteEmple);
            enableCheckBox(cbVisibleDirEmple);
            enableDDL(drpSitioEmple);
            enableDDL(drpPuestoEmple);
            enableDDL(drpEmpresaEmple);
            enableDDL(drpTipoEmpleado);
            //20161108 Se agrega nuevo control
            enableTxtBox(txtComentariosSincro);

            if (DSODataContext.Schema.ToUpper() == "FCA")
            {
                //20190510 RM campos datos emple fca 
                enableTxtBox(txtDatosEmpleFCADC_ID);
                enableTxtBox(txtDatosEmpleFCAT_ID);
                enableTxtBox(txtDatosEmpleFCAEstacion);
                //enableTxtBox(txtDatosEmpleFCANickName);
                enableDDL(ddlDatosEmpleFCADirector);
                enableCheckBox(cbDatosEmpleFCAEsDirector);
                enableDDL(ddlDatosEmpleFCAPlanta);
                enableDDL(ddlDatosEmpleFCADpto);
            }

        }

        #endregion Empleados


        #region Inventario

        //20170620 Se comenta la sección de inventarios a petición de RJ
        /*
        //Agregar filas al grid de inventario        
        //RZ.20131204 Se retira edicion y alta de inventario, solo queda funcional la baja desde las gridview
        protected void btnGuardar_PopUpInventario(object sender, EventArgs e)
        {
            try
            {
                bajaDeInventario();
                CambiarEstatusCCust(1);
                dtInventarioAsignado.Clear();
                FillInventarioGrid();

            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al intentar guardar la relacion del Dispositivo - Empleado '" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        public void bajaDeInventario()
        {
            //Query para ver si la extensión ya tiene una relación con otro empleado.
            StringBuilder sbiCodDisp = new StringBuilder();
            sbiCodDisp.AppendLine("select iCodCatalogo from [VisHistoricos('Dispositivo','Inventario de dispositivos','Español')]");
            sbiCodDisp.AppendLine("where dtinivigencia<>dtfinvigencia");
            sbiCodDisp.AppendLine("and dtfinvigencia >= getdate()");
            sbiCodDisp.AppendLine("and NSerie = '" + txtNoSeriePopUp.Text + "'");
            DataRow driCodDisp = DSODataAccess.ExecuteDataRow(sbiCodDisp.ToString());

            string iCodDisp = driCodDisp["iCodCatalogo"].ToString();

            DALCCustodia dalCCust = new DALCCustodia();
            //RZ.20130805
            dalCCust.bajaInventario(iCodDisp, iCodCatalogoEmple, DateTime.Today);

            MensajeDeAdvertencia("Inventario: El equipo ha sido desvinculado del empleado correctamente ");
        }

        //Borrar filas inventario
        protected void grvInventario_DeleteRow(object sender, ImageClickEventArgs e)
        {
            ImageButton ibtn3 = sender as ImageButton;
            int rowIndex = Convert.ToInt32(ibtn3.Attributes["RowIndex"]);

            //Use this rowIndex in your code 

            GridViewRow selectedRow = (GridViewRow)grvInventario.Rows[rowIndex];

            //int iCodMarca = (int)grvInventario.DataKeys[rowIndex].Values[0];
            //int iCodModelo = (int)grvInventario.DataKeys[rowIndex].Values[1];

            //Se llenan los controles del pop-up
            //ccdMarcaPopUp.SelectedValue = iCodMarca.ToString();
            //ccdModelo.SelectedValue = iCodModelo.ToString();
            txtMarcaPopUp.Text = selectedRow.Cells[0].Text;
            txtModeloPopUp.Text = selectedRow.Cells[1].Text;
            txtTipoAparatoPopUp.Text = selectedRow.Cells[2].Text;
            txtNoSeriePopUp.Text = selectedRow.Cells[3].Text;
            txtMacAddressPopUp.Text = selectedRow.Cells[4].Text;

            mpeInventario.Show();
        }

        //RZ.20131203 Se agrega boton para busqueda de numeros de series
        protected void lbtnBuscaNoSerie_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlMarca.SelectedValue) || string.IsNullOrEmpty(ddlModelo.SelectedValue))
            {
                MensajeDeAdvertencia("Debe seleccionar una marca y modelo para filtrar números de series.");
                return;
            }

            CCustodia webServiceCCustodia = new CCustodia();
            DataTable ldtNoSeriesEncontrados;

            txtBuscaNoSerie.Text = txtNoSerie.Text;

            ldtNoSeriesEncontrados = webServiceCCustodia.ObtieneNoSeriePorModelo(ddlModelo.SelectedValue,
                ddlMarca.SelectedValue, txtNoSerie.Text, 50);

            grvResultInventario.DataSource = ldtNoSeriesEncontrados;
            grvResultInventario.DataBind();

            mpeBuscaNoSerie.Show();
        }

        //RZ.20131204 Se agrega event handler para boton de busqueda dentro de modal pop para asignar inventario
        protected void ibtnSearchNoSerie_Click(object sender, ImageClickEventArgs e)
        {
            DataTable ldtNoSeriesEncontrados;

            ldtNoSeriesEncontrados = webServiceCCustodia.ObtieneNoSeriePorModelo(ddlModelo.SelectedValue,
                ddlMarca.SelectedValue, txtBuscaNoSerie.Text, 50);

            grvResultInventario.DataSource = ldtNoSeriesEncontrados;
            grvResultInventario.DataBind();

            mpeBuscaNoSerie.Show();
        }

        //RZ.20131203 Se agrega boton asignar equipo a empleado
        protected void lbtnAsignaEquipo_Click(object sender, EventArgs e)
        {
            try
            {
                procesoAsignaInventarioEmpleado();
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al intentar guardar la relacion del Dispositivo - Empleado '" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);

                //MensajeDeAdvertencia("Ocurrio un error al intentar guardar la relacion del Dispositivo - Empleado '");
                //20150330 NZ De la línea de arriba que se comento se quito la comilla simple que se escuenta despues de "Empleado" por que causa errores en el navegador evitando que salga el mensaje. Se rompen los parentesis angulares.
                MensajeDeAdvertencia("Ocurrio un error al intentar guardar la relacion del Dispositivo - Empleado");
            }
        }

        //RZ.20131204 Se limpian los controles que sirven para agregar inventario a un empleado
        protected void clearControlsAddInventario(bool esNuevo)
        {
            if (esNuevo)
            {
                //RZ.20131205 Dejar los dropdowns con valores previamente seleccionados
                cddMarca.SelectedValue = cddMarca.PromptValue;
                cddModelo.SelectedValue = cddModelo.PromptValue;
                txtNoSerie.Text = String.Empty;
            }

            txtTipoAparato.Text = String.Empty;
            txtMACAddress.Text = String.Empty;
            hdnfDispositivo.Value = String.Empty;
        }

        //RZ.20131204 Se agrega funcionalidad para alta de relacion del empleado
        public void procesoAsignaInventarioEmpleado()
        {
            //Actualizar la grid en cada postback
            dtInventarioAsignado.Clear();
            FillInventarioGrid();

            if (validaAsignaInventarioEmpleado() && validaNoRelacionEquipo(hdnfDispositivo.Value)
                && validaMathcNSerieiCodCatalogo(txtNoSerie.Text, hdnfDispositivo.Value))
            {
                DateTime dtFechaInicioRelDispEmple = DateTime.Today;

                //Se crea un objeto para dar de alta la relacion Dispositivo - Empleado
                DALCCustodia Dispositivo = new DALCCustodia();

                Dispositivo.altaRelacionEmpDispositivo(hdnfDispositivo.Value, txtNoSerie.Text,
                    txtNominaEmple.Text, iCodCatalogoEmple, dtFechaInicioRelDispEmple);

                //RZ.20131204 Se agrega update en la macAddress en cuando se asigna inventario
                Dispositivo.editInventario(txtMACAddress.Text, hdnfDispositivo.Value);

                CambiarEstatusCCust(1);
                //Actualizar la grid en cada postback
                dtInventarioAsignado.Clear();
                FillInventarioGrid();

                clearControlsAddInventario(true);
                MensajeDeAdvertencia("Inventario: El equipo ha sido asignado correctamente al empleado");
            }
            else
            {
                clearControlsAddInventario(false);
            }
        }

        protected bool validaNoRelacionEquipo(string iCodDispositivo)
        {
            bool lbResult = true;

            StringBuilder lsbConsulta = new StringBuilder();

            lsbConsulta.Append("SELECT Cant = COUNT(*) \r");
            lsbConsulta.Append("FROM [VisRelaciones('Dispositivo - Empleado','Español')] \r");
            lsbConsulta.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
            lsbConsulta.Append("and dtFinVigencia >= GETDATE() \r");
            lsbConsulta.Append("and Dispositivo = " + iCodDispositivo + " \r");

            int liCant = (int)DSODataAccess.ExecuteScalar(lsbConsulta.ToString());

            if (liCant > 0)
            {
                lbResult = false;
                TextInfo miInfo;
                string nombreEmpleado;

                lsbConsulta.Length = 0;
                lsbConsulta.Append("SELECT NomCompleto + '(' as EmpleDesc \r");
                lsbConsulta.Append("from [VisHistoricos('Emple','Empleados','Español')] \r");
                lsbConsulta.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
                lsbConsulta.Append("and dtFinVigencia >= GETDATE() \r");
                lsbConsulta.Append("and iCodCatalogo = ( \r");
                lsbConsulta.Append("SELECT Top 1 Emple \r");
                lsbConsulta.Append("FROM [VisRelaciones('Dispositivo - Empleado','Español')] \r");
                lsbConsulta.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
                lsbConsulta.Append("and dtFinVigencia >= GETDATE() \r");
                lsbConsulta.Append("and Dispositivo = " + iCodDispositivo + " )\r");

                ObtieneNombreEmpleadoRelacionado(DSODataAccess.ExecuteDataRow(lsbConsulta.ToString()), out miInfo, out nombreEmpleado);

                MensajeDeAdvertencia("El dispositivo que seleccionó ya esta asignado al empleado : " + miInfo.ToTitleCase(nombreEmpleado.ToLower()));

                //Si ya existe la relacion entonces se restauran todos los controles
                clearControlsAddInventario(true);

                return lbResult;
            }

            return lbResult;

        }

        protected bool validaAsignaInventarioEmpleado()
        {
            bool lbResult = true;

            if (ddlMarca.SelectedValue == "" || ddlModelo.SelectedValue == "")
            {
                MensajeDeAdvertencia("Debe seleccionar una marca y modelo válido");
                lbResult = false;
                return lbResult;
            }

            if (txtNoSerie.Text == "" || string.IsNullOrEmpty(hdnfDispositivo.Value))
            {
                MensajeDeAdvertencia("Debe seleccionar un numero de serie válido, en la sección " + lbtnBuscaNoSerie.Text);
                lbResult = false;
                return lbResult;
            }

            return lbResult;
        }

        //RZ.20131205 Valida que lo que contenga el textbox y el hiddenfield que guarda el icodcatalogo hagan match
        protected bool validaMathcNSerieiCodCatalogo(string lsNoSerie, string iCodDispositivo)
        {
            bool lbReturnValue = false;
            StringBuilder lsbConsulta = new StringBuilder();

            lsbConsulta.Append("SELECT Cant = COUNT(*) \r");
            lsbConsulta.Append("FROM [VisHistoricos('Dispositivo','Inventario de dispositivos','Español')] Disp, \r");
            lsbConsulta.Append("(SELECT iCodCatalogo  \r");
            lsbConsulta.Append("FROM [VisHistoricos('Estatus','Estatus dispositivo','Español')] \r");
            lsbConsulta.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
            lsbConsulta.Append("and dtFinVigencia >= GETDATE() \r");
            lsbConsulta.Append("and Value = 1) as Estatus \r");
            lsbConsulta.Append("WHERE Disp.dtIniVigencia <> Disp.dtFinVigencia \r");
            lsbConsulta.Append("and Disp.dtFinVigencia >= GETDATE() \r");
            lsbConsulta.Append("and Disp.Estatus = Estatus.iCodCatalogo \r");
            lsbConsulta.Append("and Disp.iCodCatalogo = " + iCodDispositivo + " \r");
            lsbConsulta.Append("and Disp.NSerie = '" + lsNoSerie + "'");

            int liCant = (int)DSODataAccess.ExecuteScalar(lsbConsulta.ToString());

            if (liCant == 1)
            {
                lbReturnValue = true;
                return lbReturnValue;
            }

            MensajeDeAdvertencia("El número de serie seleccionado no disponible para asignar al empleado");
            return lbReturnValue;
        }

        //RZ.20131204 Se agrega evento para asignar equipo en la ventana de busqueda de no. serie
        protected void grvResultInventario_AsignaRow(object sender, ImageClickEventArgs e)
        {
            //Del parametro sender obtenerlo como un imagebutton
            ImageButton ibtnGrid = sender as ImageButton;
            //Tomar el atributo RowIndex del ImageButton esto nos regresara el numero de fila en al gridwiew
            int rowIndex = Convert.ToInt32(ibtnGrid.Attributes["RowIndex"]);

            //Usando el rowIndex obtenido tomar la fila de la gridview
            GridViewRow selectedRow = (GridViewRow)grvResultInventario.Rows[rowIndex];

            //Usando la propiedad DataKey de la gridview para obtener el valor del icodcatalogo del dispositivo
            int iCodDispositivo = (int)grvResultInventario.DataKeys[rowIndex].Values[0];

            //De la fila seleccionada establecer el valor del numero de serie, tipo de aparato, macAddress y el icodCAtalogo del dispositivo
            txtNoSerie.Text = selectedRow.Cells[0].Text.Replace("&nbsp;", "");
            txtTipoAparato.Text = selectedRow.Cells[1].Text.Replace("&nbsp;", "");
            txtMACAddress.Text = selectedRow.Cells[2].Text.Replace("&nbsp;", "");
            hdnfDispositivo.Value = iCodDispositivo.ToString();
        }
        
        protected void hdnValorNSerie_ValueChanged(object sender, EventArgs e)
        {
            string selectedNserie = ((HiddenField)sender).Value;

            //Extraer el iCodCatalogo del item seleccionado.
            int startSubstring = selectedNserie.IndexOf("(") + 1;
            int lengthSubstring = selectedNserie.Length - startSubstring - 1;
            string iCodCatNSerie = selectedNserie.Substring(startSubstring, lengthSubstring);

            Dictionary<string, string> dictDatosDispositivo =
                webServiceCCustodia.ObtieneDatosAdicDispositivo(iCodCatNSerie);


            if (dictDatosDispositivo.ContainsKey("TipoDispositivo"))
            {
                txtTipoAparatoPopUp.Text = dictDatosDispositivo["TipoDispositivo"].ToString();
            }

            if (dictDatosDispositivo.ContainsKey("MacAddress"))
            {
                txtMacAddressPopUp.Text = dictDatosDispositivo["MacAddress"].ToString();

            }

            mpeInventario.Show();

        }
       
        */

        #endregion


        #region Extensiones

        protected void btnGuardar_PopUpExten(object sender, EventArgs e)
        {
            //Cuando es Edición o Baja
            btnGuardarExten.Enabled = false;

            try
            {
                string tipoABC = string.Empty;
                string validaCampos = ValidaCamposExtensiones(true);

                //Se valida que los campos necesarios no esten vacios y tengan el formato correcto.
                if (validaCampos.Length > 0)
                {
                    throw new ArgumentException(validaCampos);
                }
                else
                {
                    if (cbEditarExtension.Checked == true)
                    {
                        EdicionDeExtension();
                        tipoABC = "C";
                    }
                    else if (cbBajaExtension.Checked == true)
                    {
                        BajaDeExtension();
                        tipoABC = "B";
                    }

                    DataRow drExtension = ExisteLaExtension(NoSQLCode(txtExtension.Text), NoSQLCode(drpSitio.Text));

                    if (drExtension != null)
                    {
                        DALCCustodia dalCC = new DALCCustodia();
                        dalCC.guardaHistRecurso(drExtension["iCodCatalogo"].ToString(), "Ext", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), tipoABC);
                    }
                    CambiarEstatusCCust(1);
                    dtExtensiones.Clear();
                    FillExtenGrid();

                    MensajeProcesoExitoso();
                }
            }
            catch (ArgumentException ex)
            {
                MensajeDeAdvertencia(ex.Message);
                mpeExten.Show();  //Aquí aplica esta linea por que los movimientos se estan haciendo desde un Modal.
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al intentar guardar la extensión '" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        protected void lbtnGuardar_ExtenNoPopUp(object sender, EventArgs e)
        {
            //Cuando es Alta. Puesto que no se usa un PopUp Sino que los campos estan fijos en la pagina.
            lbtnGuardarExtenNoPopUp.Enabled = false;

            try
            {
                string tipoABC = string.Empty;
                string validaCampos = ValidaCamposExtensiones(false);

                //Se valida que los campos necesarios no esten vacios y tengan el formato correcto.
                if (validaCampos.Length > 0)
                {
                    throw new ArgumentException(validaCampos);
                }
                else
                {
                    ProcesoDeAltaExten();
                    tipoABC = "A";

                    DataRow drExtension = ExisteLaExtension(NoSQLCode(txtExtensionNoPopUp.Text), NoSQLCode(drpSitioNoPopUp.Text));

                    if (drExtension != null)
                    {
                        DALCCustodia dalCC = new DALCCustodia();
                        dalCC.guardaHistRecurso(drExtension["iCodCatalogo"].ToString(), "Ext", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), tipoABC);
                    }
                    CambiarEstatusCCust(1);
                    dtExtensiones.Clear();
                    FillExtenGrid();

                    MensajeProcesoExitoso();
                    limpiaCamposNoPopUpExten();
                }
            }
            catch (ArgumentException ex)
            {
                MensajeDeAdvertencia(ex.Message);
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al intentar guardar la extensión '" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        public void ProcesoDeAltaExten()
        {
            //La alta no se hace a travez de los controles del PopUp.
            DropDownList ddLTipoExten = drpTipoExtenNoPopUp;

            //AM.20131205 Se valida si el empleado cuenta con una extensión principal
            if ((ddLTipoExten.SelectedItem.ToString() == "EXTENSIÓN PRINCIPAL" || ddLTipoExten.SelectedIndex == 0) && !validaTipoExtension())
            {
                throw new ArgumentException("El empleado ya cuenta con una extensión principal, favor de seleccionar otro tipo de extensión.");
            }
            else //Si el tipo de extensión no es principal sigue con el proceso
            {
                //Mapeo de campos
                string extensionCod = NoSQLCode(txtExtensionNoPopUp.Text);
                DateTime dtFechaInicioExten = Convert.ToDateTime(txtFechaInicioNoPopUp.Text);
                string vchCodigoEmple = NoSQLCode(txtNominaEmple.Text);
                string iCodSitio = NoSQLCode(drpSitioNoPopUp.Text);
                string iCodTipoExten = NoSQLCode(drpTipoExtenNoPopUp.Text);
                string iCodVisibleDir = NoSQLCode(drpVisibleDirNoPopUp.Text);
                string iCodCos = NoSQLCode(drpCosExtenNoPopUp.SelectedValue);
                string comentarios = NoSQLCode(txtComentariosExtenNoPopUp.Text);

                #region La extensión a dar de alta no es una extensión principal

                if (validaFechaInicioExten(dtFechaInicioExten))  //Inicio de la relación
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

                                txtExtensionNoPopUp.Text = string.Empty;
                                txtExtensionNoPopUp.Focus();
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
                                extension.altaRelacionEmpExt(iCodCatalogoExten, vchCodigoExten, iCodCatalogoEmple, vchCodigoEmple, dtFechaInicioExten.ToString());
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
                            txtFechaInicioNoPopUp.Text = string.Empty;
                            txtFechaInicioNoPopUp.Focus();
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
                            if (cbRangoExtenNoPopUp.Checked)
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
                                    txtFechaInicioNoPopUp.Text = string.Empty;
                                    txtFechaInicioNoPopUp.Focus();
                                    throw new ArgumentException("La fecha que selecciono entra dentro de los rangos de relación de otro empleado, favor de seleccionar una fecha validá.");
                                }
                            }

                            //Dar de alta la extensión 
                            extension.altaExtension(extensionCod, iCodSitio, iCodCos, lsSitioDesc, dtFechaInicioExten, iCodTipoExten, comentarios, iCodVisibleDir);

                            #region Alta en relaciones

                            DataRow drExtensionReciente = ExisteLaExtension(extensionCod, iCodSitio);

                            //Variables que se necesitan extraer para mandar como parametros en el metodo que da de alta un registro en relaciones
                            int iCodCatalogoExten = (int)drExtensionReciente["iCodCatalogo"];
                            string vchCodigoExten = drExtensionReciente["vchCodigo"].ToString();

                            // Se hace un insert en [VisRelaciones('Empleado - Extension','Español')] 
                            // Se hace un update a la vista [VisHistoricos('Exten','Extensiones','Español')] en el campo Emple
                            extension.altaRelacionEmpExt(iCodCatalogoExten, vchCodigoExten, iCodCatalogoEmple, vchCodigoEmple, dtFechaInicioExten.ToString());

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

        private void ConsultaRangosConfigEnSitio(out DataRow[] drArrayRangosExtension, out DataRow drRangosExtension, string iCodSitio)
        {
            //Query para ver si la extensión entra dentro de los rangos configurados
            drArrayRangosExtension = new DataRow[1];
            lsbQuery.Length = 0;
            lsbQuery.AppendFormat("EXEC ObtieneRangosExtensiones @esquema = '{0}', @iCodCatSitio = {1}", DSODataContext.Schema, iCodSitio);
            drRangosExtension = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());
            drArrayRangosExtension[0] = drRangosExtension;
        }

        private void ActualizaAtributosDeExtensiones(string lsiCodExten, string visibleDir)
        {
            DSODataAccess.ExecuteNonQuery("update [VisHistoricos('Exten','Extensiones','Español')] " +
                                          "set BanderasExtens = " + visibleDir + "," +
                                          "    dtFecUltAct = getdate()" +
                                          "where iCodCatalogo = " + lsiCodExten +
                                          "and dtIniVigencia<>dtFinVigencia and dtFinVigencia >= getdate()");
        }

        private static void ObtieneNombreEmpleadoRelacionado(DataRow drRelEmpExtQuery, out TextInfo miInfo, out string nombreEmpleado)
        {
            string nombreEmpleRel = drRelEmpExtQuery["EmpleDesc"].ToString();

            miInfo = CultureInfo.CurrentCulture.TextInfo;
            int inicioDeParentesis = nombreEmpleRel.IndexOf("(") + 1;
            char[] parentesis = { ')', '(' };
            nombreEmpleado = nombreEmpleRel.Substring(0, inicioDeParentesis).TrimEnd(parentesis).Trim();
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

        private DataRow ExisteLaExtension(string extensionCod, string iCodSitio)
        {
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

        //AM 20131205 Valida si el empleado ya tiene una extensión principal.
        /// <summary>
        /// Si el empleado ya cuenta con una extensión principal regresa un "false"
        /// </summary>
        /// <returns></returns>
        protected bool validaTipoExtension()
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

        //AM 20131212 Valida si el empleado ya tiene una extensión principal.
        /// <summary>
        /// Si el empleado ya cuenta con una extensión principal regresa un datarow con el iCodCatalogo de la extensión y el sitio.
        /// </summary>
        /// <returns></returns>
        protected DataRow validaTipoExtensionEnEdicion()
        {
            DataRow ldr = null;
            StringBuilder lsb = new StringBuilder();
            try
            {
                #region Consulta el numero de extensiones principales del empleado
                lsb.Length = 0;
                lsb.Append("select Ext.vchCodigo, Ext.Sitio \r");
                lsb.Append("from [VisHistoricos('Exten','Extensiones','Español')] Ext \r");
                lsb.Append("inner join [VisHistoricos('ExtenB','Extensiones B','Español')] ExtB \r");
                lsb.Append("on Ext.iCodCatalogo = ExtB.Exten \r");
                lsb.Append("and ExtB.dtIniVigencia <> ExtB.dtFinVigencia \r");
                lsb.Append("and ExtB.dtfinVigencia >= getdate() \r");
                lsb.Append("where Ext.dtIniVigencia <> Ext.dtFinVigencia \r");
                lsb.Append("and Ext.dtfinVigencia >= getdate() \r");
                lsb.Append("and Ext.Emple = " + iCodCatalogoEmple + "\r");
                lsb.Append("and TipoRecursoDesc like '%EXTENSION%PRINCIPAL%' \r");
                #endregion

                ldr = DSODataAccess.ExecuteDataRow(lsb.ToString());

            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException(
                    "Ocurrio un error al consultar el numero de extensiones principales en validaTipoExtension() en AppCCustodia.aspx.cs '"
                    + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }

            return ldr;
        }

        /// <summary>
        /// Valida que los campos requeridos para modulo de extensiones no esten vacios.
        /// </summary>
        /// <returns>Regresa una cadena con las notificaciones de los campos que son requeridos. </returns>
        private string ValidaCamposExtensiones(bool esMediantePopUp)
        {
            StringBuilder sbErrors = new StringBuilder(string.Empty);

            string exten = string.Empty;
            string sitio = string.Empty;
            string cos = string.Empty;
            string fechaIni = string.Empty;
            string fechaFin = string.Empty;
            string tipoExten = string.Empty;
            string comentarios = string.Empty;
            TextBox fechaI;
            TextBox fechaF;

            if (esMediantePopUp)  //Edicion o baja
            {
                exten = txtExtension.Text;
                sitio = drpSitio.SelectedValue;
                cos = drpCosExten.SelectedValue;
                fechaIni = txtFechaInicio.Text;
                fechaFin = txtFechaFinExten.Text;
                tipoExten = drpTipoExten.SelectedValue;
                fechaI = txtFechaInicio;
                fechaF = txtFechaFinExten;
            }
            else //Alta
            {
                exten = txtExtensionNoPopUp.Text;
                sitio = drpSitioNoPopUp.SelectedValue;
                cos = drpCosExtenNoPopUp.SelectedValue;
                fechaIni = txtFechaInicioNoPopUp.Text;
                tipoExten = drpTipoExtenNoPopUp.SelectedValue;
                fechaI = txtFechaInicioNoPopUp;
                fechaF = new TextBox();  //cuando no es por PopUp es por que se esta haciendo una alta y aqui no hay fecha fin.
            }

            if (exten == string.Empty || exten == "")
            {
                sbErrors.Append(@"*El campo (Extensión) es requerido. \n");
            }
            else if (exten.Length >= 16)
            {
                sbErrors.Append(@"*La Longitud del campo (Extensión) es muy grande. \n");
            }
            else
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(exten, @"^\d*$"))
                {
                    sbErrors.Append(@"*El campo (Extensión) solo debe contener números. \n");
                }
                else
                {
                    char primerDato = exten[0];
                    bool digitosIguales = true;
                    for (int i = 0; i < exten.Length; i++)
                    {
                        if (exten[i] != primerDato)
                        {
                            digitosIguales = false;
                            break;
                        }
                    }

                    if (digitosIguales)
                    {
                        sbErrors.Append(@"*Los digitos en el campo (Extensión) no pueden ser todos iguales. \n");
                    }
                }
            }

            if (string.IsNullOrEmpty(sitio))
            {
                sbErrors.Append(@"*El campo (Sitio) es requerido. \n");
            }

            //if (string.IsNullOrEmpty(cos))
            //{
            //    sbErrors.Append(@"*El campo (Cos) es requerido. \n");
            //}

            if (fechaIni == string.Empty || fechaIni == "")
            {
                sbErrors.Append(@"*El campo (Fecha inicio) es requerido. \n");
            }

            if (!validaFormatoFecha(fechaIni))
            {
                sbErrors.Append(@"*El formato de (Fecha inicio) debe ser DD/MM/AAAA. Favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA. \n");
                fechaI.Text = string.Empty;
            }

            if (string.IsNullOrEmpty(tipoExten))
            {
                sbErrors.Append(@"*El campo (Tipo de extensión) es requerido. \n");
            }

            if (esMediantePopUp && (fechaFin == string.Empty || fechaFin == ""))
            {
                sbErrors.Append(@"*El campo (Fecha Fin) es requerido. \n");
            }

            if (esMediantePopUp && !validaFormatoFecha(fechaFin))
            {
                sbErrors.Append(@"*El formato de (Fecha Fin) debe ser DD/MM/AAAA. Favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA. \n");
                fechaF.Text = string.Empty;
            }

            return sbErrors.ToString();
        }

        /// <summary>
        /// Valida que la fecha de inicio de la extensión sea mayor o igual a la fecha de inicio del empleado.
        /// </summary>
        /// <returns>Si la fecha de inicio de la extensión si es mayor o igual a la fecha de inicio del empleado, regresa un "true"</returns>
        private bool validaFechaInicioExten(DateTime dtFechaIniExt)
        {
            bool lb = false;

            string lsdtFechaInicioEmple =
                DSODataAccess.ExecuteScalar("select dtIniVigencia from [VisHistoricos('Emple','Empleados','Español')] " +
                                            "where iCodCatalogo = " + iCodCatalogoEmple +
                                            "and dtIniVigencia<>dtFinVigencia and dtFinVigencia >= getdate()").ToString();

            DateTime dtFechaInicioEmple = Convert.ToDateTime(lsdtFechaInicioEmple);

            if (dtFechaIniExt >= dtFechaInicioEmple)
            {
                lb = true;
            }

            return lb;
        }

        /// <summary>
        /// Consulta de registros vigentes en Extensiones B para la extensión que se quiere dar de alta.
        /// </summary>
        /// <param name="iCodCatalogoExtension">iCodCatalogo de la extensión que se desea dar de alta.</param>
        /// <returns>Regresa el iCodRegistro de extensiones B</returns>
        private string ExisteRegistroVigenteEnExtenB(string iCodCatalogoExtension)
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

        /// <summary>
        /// Consulta de registros vigentes en relaciones para la extensión que se quiere dar de alta.
        /// </summary>
        /// <param name="iCodCatalogoExtension">iCodCatalogo de la extensión que se desea dar de alta.</param>
        /// <returns>Regresa el iCodRegistro de relaciones</returns>
        private string ExisteRegistroVigenteEnRelac(string iCodCatalogoExtension)
        {
            string iCodRegistroRelac = string.Empty;

            try
            {
                #region Consulta para ver si existen registros vigentes en extensiones B

                StringBuilder sbQuery = new StringBuilder();
                sbQuery.AppendLine("select Max(iCodRegistro) from [VisRelaciones('Empleado - Extension','Español')]");
                sbQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
                sbQuery.AppendLine("and dtfinvigencia >= getdate()");
                sbQuery.AppendLine("and Exten = '" + iCodCatalogoExtension + "'");

                #endregion

                iCodRegistroRelac = DSODataAccess.ExecuteScalar(sbQuery.ToString()).ToString();
            }

            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException(
                    "Ocurrio un error en metodo ExisteRegistroVigenteEnExtenB(string iCodCatalogoExtension) en AppCCustodia.aspx.cs '"
                    + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }

            return iCodRegistroRelac;
        }

        private bool ValidaCosExten()
        {
            bool lb = true;
            StringBuilder lsb = new StringBuilder();
            try
            {
                #region Consulta sí el cos tiene la misma marca del sitio seleccionado.
                lsb.Length = 0;
                lsb.AppendLine("SELECT iCodCatalogo");
                lsb.AppendLine("FROM [VisHistoricos('Cos','Cos','Español')]");
                lsb.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
                lsb.AppendLine("	AND dtFinVigencia >= GETDATE()");
                lsb.AppendLine("	AND iCodCatalogo = " + drpCosExten.SelectedValue);
                lsb.AppendLine("	AND MarcaSitio IN (");
                lsb.AppendLine("						SELECT MarcaSitio");
                lsb.AppendLine("						FROM [VisHisComun('Sitio','Español')]");
                lsb.AppendLine("						WHERE dtIniVigencia <> dtFinVigencia");
                lsb.AppendLine("							AND dtFinVigencia >= GETDATE()");
                lsb.AppendLine("							AND iCodCatalogo = " + drpSitio.SelectedValue);
                lsb.AppendLine("						)");
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
                    "Ocurrio un error al consultar sí el cos tiene la misma marca del sitio seleccionado en ValidaCosExten() en AppCCustodia.aspx.cs '"
                    + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }

            return lb;
        }

        public void EdicionDeExtension()
        {
            //Mapeo de campos
            string exten = NoSQLCode(txtExtension.Text);
            string iCodSitio = NoSQLCode(drpSitio.Text);
            string iCodTipoExten = NoSQLCode(drpTipoExten.Text);
            DateTime dtFechaInicio = Convert.ToDateTime(txtFechaInicio.Text);
            DateTime dtFechaFin = Convert.ToDateTime(txtFechaFinExten.Text);
            string iCodRegistroRel = NoSQLCode(txtRegistroRelacion.Text);
            string iCodVisiDir = NoSQLCode(drpVisibleDir.Text);
            string comentarios = NoSQLCode(txtComentariosExten.Text);
            string iCodCos = drpCosExten.SelectedValue;

            lsbQuery.Length = 0;
            lsbQuery.AppendLine("SELECT iCodCatalogo, vchCodigo, SitioDesc FROM [VisHistoricos('Exten','Extensiones','Español')]");
            lsbQuery.AppendLine("WHERE dtIniVigencia<>dtFinVigencia");
            lsbQuery.AppendLine("   AND dtFinVigencia >= GETDATE()");
            lsbQuery.AppendLine("   AND vchCodigo = '" + exten + "'");
            lsbQuery.AppendLine("   AND Sitio = " + iCodSitio);
            lsbQuery.AppendLine("   AND iCodCatalogo IN(");
            lsbQuery.AppendLine("                       SELECT Exten");
            lsbQuery.AppendLine("                       FROM [VisRelaciones('Empleado - Extension','Español')]");
            lsbQuery.AppendLine("                       WHERE dtIniVigencia<>dtFinVigencia");
            lsbQuery.AppendLine("                               AND dtFinVigencia >= GETDATE()");
            lsbQuery.AppendLine("                               AND Emple = " + iCodCatalogoEmple);
            lsbQuery.AppendLine("                       )");
            DataRow drExtension = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

            if (drExtension != null)
            {
                string iCodCatalogoExten = drExtension["iCodCatalogo"].ToString();

                DataRow ldr = validaTipoExtensionEnEdicion();

                //AM.20131205 Se valida si el empleado cuenta con una extensión principal
                if ((drpTipoExten.SelectedItem.ToString() == "EXTENSIÓN PRINCIPAL" || drpTipoExten.SelectedIndex == 0) && ldr != null &&
                    ldr["vchCodigo"].ToString() != exten)
                {
                    throw new ArgumentException("El empleado ya cuenta con una extensión principal, favor de seleccionar otro tipo de extensión.");
                }
                else
                {
                    #region proceso edición

                    //Si la fecha de inicio es mayor o igual a la fecha de alta del empleado entra a este bloque
                    if (validaFechaInicioExten(dtFechaInicio))
                    {
                        string fechaInicioValida = ValidarTraslapeFechas(dtFechaInicio, iCodCatalogoExten, "Exten", "Empleado - Extension", iCodRegistroRel);

                        if (fechaInicioValida == "1")
                        {
                            string fechaFinValida = ValidarTraslapeFechas(dtFechaFin, iCodCatalogoExten, "Exten", "Empleado - Extension", iCodRegistroRel);

                            if (fechaFinValida == "1")
                            {
                                if (dtFechaFin >= dtFechaInicio)
                                {
                                    DALCCustodia dalCCust = new DALCCustodia();
                                    dalCCust.editExten(iCodVisiDir, dtFechaInicio.ToString(), dtFechaFin.ToString(), iCodCatalogoExten,
                                        iCodTipoExten, comentarios, iCodCatalogoEmple, iCodCos);
                                }
                                else //La fecha de fin no es valida
                                {
                                    txtFechaFinExten.Text = string.Empty;
                                    txtFechaFinExten.Focus();
                                    throw new ArgumentException("La fecha fin debe ser mayor a la fecha de inicio, favor de seleccionar una fecha valida.");
                                }
                            }
                            else
                            {
                                txtFechaFinExten.Text = string.Empty;
                                txtFechaFinExten.Focus();
                                throw new ArgumentException("La fecha fin que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                            }
                        }
                        else  //La fecha de inicio no es valida
                        {
                            txtFechaInicio.Text = string.Empty;
                            txtFechaInicio.Focus();
                            throw new ArgumentException("La fecha inicio que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                        }
                    }
                    else   //Si la fecha de inicio es menor a la fecha de alta del empleado manda un mensaje de notificación
                    {
                        throw new ArgumentException("La fecha de inicio debe ser mayor a la fecha de alta del empleado");
                    }
                    #endregion //Fin de bloque -- proceso de edicion
                }
            }
            else
            {
                dtExtensiones.Clear();
                FillExtenGrid();
                throw new ArgumentException("La extensión no existe o ya no se encuentra activa.");
            }
        }

        public void BajaDeExtension()
        {
            //Mapeo de campos
            string exten = NoSQLCode(txtExtension.Text);
            string iCodSitio = NoSQLCode(drpSitio.Text);
            DateTime dtFechaFin = Convert.ToDateTime(txtFechaFinExten.Text);
            DateTime dtFechaInicio = Convert.ToDateTime(txtFechaInicio.Text);
            string iCodRegistroRel = NoSQLCode(txtRegistroRelacion.Text);

            //Query para obtener iCodCatalogo de la extension
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("SELECT iCodCatalogo, vchCodigo, SitioDesc FROM [VisHistoricos('Exten','Extensiones','Español')]");
            lsbQuery.AppendLine("WHERE dtIniVigencia<>dtFinVigencia");
            lsbQuery.AppendLine("   AND dtFinVigencia >= GETDATE()");
            lsbQuery.AppendLine("   AND vchCodigo = '" + exten + "'");
            lsbQuery.AppendLine("   AND Sitio = " + iCodSitio);
            lsbQuery.AppendLine("   AND iCodCatalogo IN(");
            lsbQuery.AppendLine("                       SELECT Exten");
            lsbQuery.AppendLine("                       FROM [VisRelaciones('Empleado - Extension','Español')]");
            lsbQuery.AppendLine("                       WHERE dtIniVigencia<>dtFinVigencia");
            lsbQuery.AppendLine("                               AND dtFinVigencia >= GETDATE()");
            lsbQuery.AppendLine("                               AND Emple = " + iCodCatalogoEmple);
            lsbQuery.AppendLine("                       )");
            DataRow drExtension = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

            if (drExtension != null)
            {
                string iCodCatalogoExten = drExtension["iCodCatalogo"].ToString();

                string fechaFinValida = ValidarTraslapeFechas(dtFechaFin, iCodCatalogoExten, "Exten", "Empleado - Extension", iCodRegistroRel);

                if (fechaFinValida == "1")
                {
                    if (dtFechaFin >= dtFechaInicio)
                    {
                        DALCCustodia dalCCust = new DALCCustodia();
                        dalCCust.bajaExten(iCodCatalogoExten, iCodCatalogoEmple, dtFechaFin);
                    }
                    else //La Fecha de Fin no es valida
                    {
                        txtFechaFinExten.Text = string.Empty;
                        txtFechaFinExten.Focus();
                        throw new ArgumentException("La fecha fin debe ser mayor a la fecha de inicio, favor de seleccionar una fecha valida.");
                    }
                }
                else //La Fecha de Fin no es valida
                {
                    txtFechaFinExten.Text = string.Empty;
                    txtFechaFinExten.Focus();
                    throw new ArgumentException("La fecha fin que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                }
            }
            else
            {
                dtExtensiones.Clear();
                FillExtenGrid();
                throw new ArgumentException("La extensión no existe o ya no encuentra activa.");
            }
        }

        public void limpiaCamposNoPopUpExten()
        {
            txtExtensionNoPopUp.Text = null;
            drpSitioNoPopUp.Text = null;
            drpCosExtenNoPopUp.Text = null;
            txtFechaInicioNoPopUp.Text = null;
            drpTipoExtenNoPopUp.Text = null;
            drpVisibleDirNoPopUp.Text = null;
            txtComentariosExtenNoPopUp.Text = null;
            cbRangoExtenNoPopUp.Checked = false;
        }

        //Carga los valores de la propiedades de los controles del pop-up de Extensiones al Editar
        public void cargaPropControlesAlEditarExten()
        {
            //Controles de pop-up
            txtExtension.Enabled = false;
            txtExtension.ReadOnly = true;
            drpSitio.Enabled = false;
            drpCosExten.Enabled = true;
            txtFechaInicio.Enabled = true;
            txtFechaInicio.ReadOnly = false;
            drpTipoExten.Enabled = true;
            drpVisibleDir.Enabled = true;
            txtComentariosExten.Enabled = true;
            lblFechaFinExten.Visible = true;
            txtFechaFinExten.Visible = true;
            txtFechaFinExten.Enabled = false; //NZ Se cambia para que solo pueda editar la fecha fin en la baja.

            //Se cambian los titulos del pop-up de extensiones y el texto del boton
            lblTituloPopUpExten.Text = "Detalle de extensión";
            btnGuardarExten.Text = "Guardar";
            btnGuardarExten.Enabled = true;

            //Control para realizar el proceso de edicion
            cbEditarExtension.Checked = true;
            cbBajaExtension.Checked = false;
        }

        //Carga los valores de la propiedades de los controles del pop-up de Extensiones al Borrar
        public void cargaPropControlesAlBorrarExten()
        {
            //Controles de pop-up
            txtExtension.Enabled = false;
            txtExtension.ReadOnly = true;
            drpSitio.Enabled = false;
            drpCosExten.Enabled = false;
            txtFechaInicio.Enabled = false;
            txtFechaInicio.ReadOnly = true;
            drpTipoExten.Enabled = false;
            drpVisibleDir.Enabled = false;
            txtComentariosExten.Enabled = false;
            lblFechaFinExten.Visible = true;
            txtFechaFinExten.Visible = true;
            txtFechaFinExten.Enabled = true;

            //Se cambia la leyenda del pop-up
            lblTituloPopUpExten.Text = "¿Esta seguro que desea dar de baja la extensión ?";
            btnGuardarExten.Text = "Eliminar";
            btnGuardarExten.Enabled = true;

            //Control para realizar el proceso de baja
            cbEditarExtension.Checked = false;
            cbBajaExtension.Checked = true;
        }

        //Editar fila extensiones
        protected void grvExten_EditRow(object sender, ImageClickEventArgs e)
        {
            ImageButton ibtn1 = sender as ImageButton;
            int rowIndex = Convert.ToInt32(ibtn1.Attributes["RowIndex"]);

            //Use this rowIndex in your code 

            GridViewRow selectedRow = (GridViewRow)grvExten.Rows[rowIndex];

            int iCodExten = (int)grvExten.DataKeys[rowIndex].Values[0];
            int iCodSitio = (int)grvExten.DataKeys[rowIndex].Values[1];
            int iCodCos = grvExten.DataKeys[rowIndex].Values[2] == DBNull.Value ? 0 : (int)grvExten.DataKeys[rowIndex].Values[2];
            int iCodTipoExten = (int)grvExten.DataKeys[rowIndex].Values[3];
            int iCodRegistroRelacion = (int)grvExten.DataKeys[rowIndex].Values[4];

            //Query para extraer valores de los controles del pop-up de Extensiones
            StringBuilder sbExtensionQuery = new StringBuilder();
            sbExtensionQuery.AppendLine("select Comentarios from [VisHistoricos('ExtenB','Extensiones B','Español')]");
            sbExtensionQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
            sbExtensionQuery.AppendLine("and dtfinvigencia >= getdate()");
            sbExtensionQuery.AppendLine("and Exten = " + iCodExten);
            DataRow drExtension = DSODataAccess.ExecuteDataRow(sbExtensionQuery.ToString());

            //Se llenan los controles del pop-up
            txtExtension.Text = selectedRow.Cells[4].Text;
            drpSitio.Text = iCodSitio.ToString();
            FillDDLCosFiltro(drpCosExten, iCodSitio.ToString());
            if (iCodCos != 0)
            {
                drpCosExten.Text = iCodCos.ToString();
            }
            txtFechaInicio.Text = selectedRow.Cells[7].Text;
            txtFechaFinExten.Text = selectedRow.Cells[8].Text;
            // 20140115 AM. Se agrega condicion para evitar error al editar una extension
            if (iCodTipoExten != 0)
            {
                drpTipoExten.Text = iCodTipoExten.ToString();
            }
            txtRegistroRelacion.Text = iCodRegistroRelacion.ToString();
            string lsVisDirExten = DSODataAccess.ExecuteScalar("select CONVERT(bit,ISNULL(BanderasExtens,0)) from [VisHistoricos('Exten','Extensiones','Español')] " +
                                                               "where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate() " +
                                                               "and iCodCatalogo =  " + iCodExten).ToString();
            if (lsVisDirExten == "True")
            {
                drpVisibleDir.Text = "1";
            }
            else
            {
                drpVisibleDir.Text = "0";
            }

            txtComentariosExten.Text = (drExtension == null) ? "" : drExtension["Comentarios"].ToString();


            cargaPropControlesAlEditarExten();
            mpeExten.Show();
        }

        //Borrar filas extensiones
        protected void grvExten_DeleteRow(object sender, ImageClickEventArgs e)
        {
            ImageButton ibtn1 = sender as ImageButton;
            int rowIndex = Convert.ToInt32(ibtn1.Attributes["RowIndex"]);

            //Use this rowIndex in your code 

            GridViewRow selectedRow = (GridViewRow)grvExten.Rows[rowIndex];

            int iCodExten = (int)grvExten.DataKeys[rowIndex].Values[0];
            int iCodSitio = (int)grvExten.DataKeys[rowIndex].Values[1];
            int iCodCos = grvExten.DataKeys[rowIndex].Values[2] == DBNull.Value ? 0 : (int)grvExten.DataKeys[rowIndex].Values[2];
            int iCodTipoExten = (int)grvExten.DataKeys[rowIndex].Values[3];
            int iCodRegistroRelacion = (int)grvExten.DataKeys[rowIndex].Values[4];

            //Query para extraer valores de los controles del pop-up de Extensiones
            StringBuilder sbExtensionQuery = new StringBuilder();
            sbExtensionQuery.AppendLine("select Comentarios from [VisHistoricos('ExtenB','Extensiones B','Español')]");
            sbExtensionQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
            sbExtensionQuery.AppendLine("and dtfinvigencia >= getdate()");
            sbExtensionQuery.AppendLine("and Exten = " + iCodExten);
            DataRow drExtension = DSODataAccess.ExecuteDataRow(sbExtensionQuery.ToString());

            //Se llenan los controles del pop-up
            txtExtension.Text = selectedRow.Cells[4].Text;
            drpSitio.Text = iCodSitio.ToString();
            FillDDLCosFiltro(drpCosExten, iCodSitio.ToString());
            if (iCodCos != 0)
            {
                drpCosExten.Text = iCodCos.ToString();
            }
            txtFechaInicio.Text = selectedRow.Cells[7].Text;
            //CalendarExtender2.en
            // 20140115 AM. Se agrega condicion para evitar error al borrar una extension
            if (iCodTipoExten != 0)
            {
                drpTipoExten.Text = iCodTipoExten.ToString();
            }
            txtRegistroRelacion.Text = iCodRegistroRelacion.ToString();
            string lsVisDirExten = DSODataAccess.ExecuteScalar("select CONVERT(bit,ISNULL(BanderasExtens,0)) from [VisHistoricos('Exten','Extensiones','Español')] " +
                                                             "where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate() " +
                                                             "and iCodCatalogo =  " + iCodExten).ToString();
            if (lsVisDirExten == "True")
            {
                drpVisibleDir.Text = "1";
            }
            else
            {
                drpVisibleDir.Text = "0";
            }

            txtComentariosExten.Text = (drExtension == null) ? "" : drExtension["Comentarios"].ToString();
            txtFechaFinExten.Text = string.Empty;

            cargaPropControlesAlBorrarExten();
            mpeExten.Show();
        }

        #endregion


        #region Codigos de Autorizacion

        protected void btnGuardar_PopUpCodAuto(object sender, EventArgs e)
        {
            //Cuando es Edición o Baja
            btnGuardarCodAuto.Enabled = false;
            try
            {
                string validaCampos = ValidaCamposCodAuto(true);

                //Se valida que los campos necesarios no esten vacios y tengan el formato correcto.
                if (validaCampos.Length > 0)
                {
                    throw new ArgumentException(validaCampos);
                }
                else
                { //Los datos son correctos.

                    string tipoABC = string.Empty;
                    //Si se llega al pop-up mediante dar clic en el boton de edicion entonces llamara al proceso de edicion 
                    if (cbEditarCodAuto.Checked == true)
                    {
                        EdicionDeCodAuto();
                        tipoABC = "C";
                    }
                    else if (cbBajaCodAuto.Checked == true)
                    {
                        BajaDeCodAuto();/*DAR DE BAJA EL USUARIO QUE COINCIDA CON EL QUE ESTA EN EL PASSWORD DEL USUARIO*/
                        tipoABC = "B";
                    }

                    lsbQuery.Length = 0;
                    lsbQuery.AppendLine("select iCodCatalogo from [VisHistoricos('CodAuto','Codigo Autorizacion','Español')]");
                    lsbQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
                    lsbQuery.AppendLine("and dtfinvigencia >= getdate()");
                    lsbQuery.AppendLine("and vchCodigo = '" + NoSQLCode(txtCodAuto.Text) + "'");
                    lsbQuery.AppendLine("and Sitio = " + NoSQLCode(drpSitioCodAuto.Text));
                    DataRow drCodAuto = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

                    if (drCodAuto != null)
                    {
                        DALCCustodia dalCC = new DALCCustodia();
                        dalCC.guardaHistRecurso(drCodAuto["iCodCatalogo"].ToString(), "CodAut", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), tipoABC);
                    }
                    CambiarEstatusCCust(1);
                    dtCodAuto.Clear();
                    FillCodAutoGrid();
                    MensajeProcesoExitoso();
                }
            }
            catch (ArgumentException ex)
            {
                MensajeDeAdvertencia(ex.Message);
                mpeCodAuto.Show(); //Aquí aplica esta linea por que los movimientos se estan haciendo desde un Modal.
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al intentar guardar el codigo de autorización '" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        protected void lbtnGuardar_CodAutoNoPopUp(object sender, EventArgs e)
        {
            //Cuando es Alta. Puesto que no se usa un PopUp Sino que los campos estan fijos en la pagina.
            lbtnGuardarCodAutoNoPopUp.Enabled = false;
            try
            {
                string commantArgument = "CODAUTO";

                if (sender is LinkButton)
                {
                    LinkButton lnkBtn = sender as LinkButton;
                    if (lnkBtn.CommandName.ToUpper() == "TELENET")
                    {
                        commantArgument = "TELENET";
                    }

                }
                string tipoABC = string.Empty;
                string validaCampos = ValidaCamposCodAuto(false);

                //Se valida que los campos necesarios no esten vacios y tengan el formato correcto.
                if (validaCampos.Length > 0)
                {
                    throw new ArgumentException(validaCampos);
                }
                else
                {
                    ProcesoAltaDeCodAuto(commantArgument);
                    tipoABC = "A";

                    lsbQuery.Length = 0;
                    lsbQuery.AppendLine("select iCodCatalogo from [VisHistoricos('CodAuto','Codigo Autorizacion','Español')]");
                    lsbQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
                    lsbQuery.AppendLine("and dtfinvigencia >= getdate()");
                    lsbQuery.AppendLine("and vchCodigo = '" + NoSQLCode(txtCodAutoNoPopUp.Text) + "'");
                    lsbQuery.AppendLine("and Sitio = " + NoSQLCode(drpSitioCodAutoNoPopUp.Text));
                    DataRow drCodAuto = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

                    if (drCodAuto != null)
                    {
                        DALCCustodia dalCC = new DALCCustodia();
                        dalCC.guardaHistRecurso(drCodAuto["iCodCatalogo"].ToString(), "CodAut", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), tipoABC);
                    }
                    CambiarEstatusCCust(1);
                    dtCodAuto.Clear();
                    FillCodAutoGrid();

                    MensajeProcesoExitoso();
                    limpiaCamposNoPopUpCodAuto();
                }
            }
            catch (ArgumentException ex)
            {
                dtCodAuto.Clear();
                FillCodAutoGrid();
                MensajeDeAdvertencia(ex.Message);
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al intentar guardar el codigo de autorización '" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        public void ProcesoAltaDeCodAuto(string CommandArgument)
        {

            //Mapeo de Campos
            string codAutoCod = NoSQLCode(txtCodAutoNoPopUp.Text);
            DateTime dtFechaInicioCodAuto = Convert.ToDateTime(txtFechaInicioCodAutoNoPopUp.Text);
            string iCodSitio = NoSQLCode(drpSitioCodAutoNoPopUp.Text);
            string iCodCos = NoSQLCode(drpCosCodAutoNoPopUp.SelectedValue);
            string vchCodigoEmple = NoSQLCode(txtNominaEmple.Text);


            if (CommandArgument == "TELENET")
            {
                iCodSitio = BuscarSitioEmple(vchCodigoEmple).ToString();
                iCodCos = BuscarCos(vchCodigoEmple).ToString();
            }

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

                    txtCodAutoNoPopUp.Text = string.Empty;
                    txtCodAutoNoPopUp.Focus();

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
                                            "where iCodCatalogo = " + iCodCatalogoEmple +
                                            "and dtIniVigencia<>dtFinVigencia and dtFinVigencia >= getdate()").ToString();

                    DateTime dtFechaInicioEmple = Convert.ToDateTime(lsdtFechaInicioEmple);

                    //Si la fecha de inicio es mayor o igual a la fecha de alta del empleado entra a este bloque
                    if (dtFechaInicioCodAuto >= dtFechaInicioEmple)
                    {
                        // Se hace un insert en [VisRelaciones('Empleado - CodAutorizacion','Español')] 
                        // Se hace un update a la vista [VisHistoricos('CodAuto','Codigo Autorizacion','Español')] en el campo Emple 
                        CodAuto.altaRelacionEmpCodAuto(vchCodigoEmple, lsVchCodCodAuto, iCodCatalogoEmple, lsiCodCatalgoCodAuto, dtFechaInicioCodAuto);
                    }
                    else //Si la fecha de inicio es menor a la fecha de alta del empleado manda un mensaje de notificación
                    {
                        throw new ArgumentException("La fecha de inicio debe ser mayor o igual a la fecha de alta del empleado.");
                    }
                }
                else //La fecha de inicio no es valida
                {
                    txtFechaInicioCodAutoNoPopUp.Text = string.Empty;
                    txtFechaInicioCodAutoNoPopUp.Focus();
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
                        txtFechaInicioCodAutoNoPopUp.Text = string.Empty;
                        txtFechaInicioCodAutoNoPopUp.Focus();
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
                lsbQuery.AppendFormat("and H.iCodCatalogo = {0} ", iCodSitio.ToString());
                DataRow drVchDescSitio = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

                string lsSitioDesc = drVchDescSitio["vchDescripcion"].ToString();

                //20140424 AM. Se agrega validacion para que la fecha inicio del codigo no pueda ser menor a la fecha inicio del empleado
                string lsdtFechaInicioEmple = DSODataAccess.ExecuteScalar("select dtIniVigencia from [VisHistoricos('Emple','Empleados','Español')] " +
                                        "where iCodCatalogo = " + iCodCatalogoEmple +
                                        "and dtIniVigencia<>dtFinVigencia and dtFinVigencia >= getdate()").ToString();

                DateTime dtFechaInicioEmple = Convert.ToDateTime(lsdtFechaInicioEmple);

                //Si la fecha de inicio es mayor o igual a la fecha de alta del empleado entra a este bloque
                if (dtFechaInicioCodAuto >= dtFechaInicioEmple)
                {
                    //Alta de el codigó de autorización
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
                    CodAuto.altaRelacionEmpCodAuto(vchCodigoEmple, lsVchCodCodAuto, iCodCatalogoEmple, lsiCodCatalgoCodAuto, dtFechaInicioCodAuto);
                }
                else //Si la fecha de inicio es menor a la fecha de alta del empleado manda un mensaje de notificación
                {
                    throw new ArgumentException("La fecha de inicio debe ser mayor o igual a la fecha de alta del empleado.");
                }

                #endregion
            }

            /*VALIDAR SI EL CODAUTO O CLAVE FAC ES DIFERENTE AL QUE TIENE ASIGNADO */
            int idEmpleFCA = Convert.ToInt32(Session["iCodCatEmpleFCA"]);
            ObtieneUserEmple(idEmpleFCA.ToString());

            passwordEmple = KeytiaServiceBL.Util.Decrypt(Session["passwordEmple"].ToString());
            if (passwordEmple.Trim() != codAutoCod.Trim())
            {
                string newPassword = Util.Encrypt(codAutoCod.Trim());
                string usuario = Session["T_id"].ToString().Trim();
                string pas = newPassword;
                string sp = "EXEC [GeneraUsuariosEmpleV2] @Usuario = '{0}',@Password = '{1}',@Emple = {2}";
                string q = string.Format(sp, usuario, pas, idEmpleFCA);
                DSODataAccess.ExecuteNonQuery(q.ToString());
            }
        }

        public int BuscarSitioEmple(string nominaEmple)
        {
            try
            {
                int sitioIdEmple = 0;



                return sitioIdEmple;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int BuscarCos(string vchCodigoEmple)
        {
            try
            {
                return 0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void EdicionDeCodAuto()
        {
            //Mapeo de campos
            string codAutoCod = NoSQLCode(txtCodAuto.Text);
            string iCodSitio = NoSQLCode(drpSitioCodAuto.Text);
            DateTime dtFechaInicio = Convert.ToDateTime(txtFechaInicioCodAuto.Text);
            DateTime dtFechaFin = Convert.ToDateTime(txtFechaFinCodAuto.Text);
            string iCodRegistroRel = NoSQLCode(txtRegistroRelacionCodAuto.Text);
            string iCodCos = NoSQLCode(drpCosCodAuto.SelectedValue);

            //Query para extraer el iCodCatalogo del codigo de autorización
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("select iCodCatalogo from [VisHistoricos('CodAuto','Codigo Autorizacion','Español')]");
            lsbQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
            lsbQuery.AppendLine("   and dtfinvigencia >= getdate()");
            lsbQuery.AppendLine("   and vchCodigo = '" + codAutoCod + "'");
            lsbQuery.AppendLine("   and Sitio = " + iCodSitio);
            lsbQuery.AppendLine("   AND iCodCatalogo IN(");
            lsbQuery.AppendLine("                       SELECT CodAuto");
            lsbQuery.AppendLine("                       FROM [VisRelaciones('Empleado - CodAutorizacion','Español')]");
            lsbQuery.AppendLine("                       WHERE dtIniVigencia<>dtFinVigencia");
            lsbQuery.AppendLine("                               AND dtFinVigencia >= GETDATE()");
            lsbQuery.AppendLine("                               AND Emple = " + iCodCatalogoEmple);
            lsbQuery.AppendLine("                       )");
            DataRow drCodAuto = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

            if (drCodAuto != null)
            {
                string iCodCatalogoCodAuto = drCodAuto["iCodCatalogo"].ToString();
                string lsdtFechaInicioEmple = DSODataAccess.ExecuteScalar("select dtIniVigencia from [VisHistoricos('Emple','Empleados','Español')] " +
                                                               "where iCodCatalogo = " + iCodCatalogoEmple +
                                                               "and dtIniVigencia<>dtFinVigencia and dtFinVigencia >= getdate()").ToString();

                DateTime dtFechaInicioEmple = Convert.ToDateTime(lsdtFechaInicioEmple);

                //Si la fecha de inicio es mayor o igual a la fecha de alta del empleado entra a este bloque
                if (dtFechaInicio >= dtFechaInicioEmple)
                {
                    string fechaInicioValida = ValidarTraslapeFechas(dtFechaInicio, iCodCatalogoCodAuto, "CodAuto", "Empleado - CodAutorizacion", iCodRegistroRel);

                    if (fechaInicioValida == "1")
                    {
                        string fechaFinValida = ValidarTraslapeFechas(dtFechaFin, iCodCatalogoCodAuto, "CodAuto", "Empleado - CodAutorizacion", iCodRegistroRel);

                        if (fechaFinValida == "1")
                        {
                            if (dtFechaFin >= dtFechaInicio)
                            {
                                DALCCustodia dalCCust = new DALCCustodia();
                                dalCCust.editCodAuto("0", dtFechaInicio.ToString(), dtFechaFin.ToString(), iCodCatalogoCodAuto, iCodCatalogoEmple, iCodCos);
                            }
                            else //La fecha de fin no es valida
                            {
                                txtFechaFinCodAuto.Text = string.Empty;
                                txtFechaFinCodAuto.Focus();
                                throw new ArgumentException("La fecha fin debe ser mayor a la fecha de inicio, favor de seleccionar una fecha valida.");
                            }
                        }
                        else
                        {
                            txtFechaFinCodAuto.Text = string.Empty;
                            txtFechaFinCodAuto.Focus();
                            throw new ArgumentException("La fecha fin que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                        }
                    } //La fecha de inicio no es valida
                    else
                    {
                        txtFechaInicioCodAuto.Text = string.Empty;
                        txtFechaInicioCodAuto.Focus();
                        throw new ArgumentException("La fecha que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                    }
                }//Si la fecha de inicio es menor a la fecha de alta del empleado manda un mensaje de notificación
                else
                {
                    throw new ArgumentException("La fecha de inicio debe ser mayor a la fecha de alta del empleado.");
                }
            }
            else
            {
                dtCodAuto.Clear();
                FillCodAutoGrid();
                throw new ArgumentException("El código de autorización no existe o ya no se encuentra activo.");
            }
        }

        public void BajaDeCodAuto()
        {
            //Mapeo de campos
            string codAutoCod = NoSQLCode(txtCodAuto.Text);
            string iCodSitio = NoSQLCode(drpSitioCodAuto.Text);
            DateTime dtFechaFin = Convert.ToDateTime(txtFechaFinCodAuto.Text);
            DateTime dtFechaInicio = Convert.ToDateTime(txtFechaInicioCodAuto.Text);
            string iCodRegistroRel = NoSQLCode(txtRegistroRelacionCodAuto.Text);

            //Query para extraer el iCodCatalogo del codigo de autorización
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("select iCodCatalogo from [VisHistoricos('CodAuto','Codigo Autorizacion','Español')] WITH(NOLOCK)");
            lsbQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
            lsbQuery.AppendLine("   and dtfinvigencia >= getdate()");
            lsbQuery.AppendLine("   and vchCodigo = '" + codAutoCod + "'");
            lsbQuery.AppendLine("   and Sitio = " + iCodSitio);
            lsbQuery.AppendLine("   AND iCodCatalogo IN(");
            lsbQuery.AppendLine("                       SELECT CodAuto");
            lsbQuery.AppendLine("                       FROM [VisRelaciones('Empleado - CodAutorizacion','Español')] WITH(NOLOCK)");
            lsbQuery.AppendLine("                       WHERE dtIniVigencia<>dtFinVigencia");
            lsbQuery.AppendLine("                               AND dtFinVigencia >= GETDATE()");
            lsbQuery.AppendLine("                               AND Emple = " + iCodCatalogoEmple);
            lsbQuery.AppendLine("                       )");
            DataRow drCodAuto = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

            if (drCodAuto != null)
            {
                string iCodCatalogoCodAuto = drCodAuto["iCodCatalogo"].ToString();
                string fechaFinValida = ValidarTraslapeFechas(dtFechaFin, iCodCatalogoCodAuto, "CodAuto", "Empleado - CodAutorizacion", iCodRegistroRel);

                if (fechaFinValida == "1")
                {
                    if (dtFechaFin >= dtFechaInicio)
                    {
                        DALCCustodia dalCCust = new DALCCustodia();
                        dalCCust.bajaCodAuto(iCodCatalogoCodAuto, iCodCatalogoEmple, dtFechaFin);

                        if (DSODataContext.Schema.Trim().ToUpper() == "FCA")
                        {
                            /*BAJA DEL usuario siempre y cuando la clave fac sea  igual al password del usuario*/
                            string codNew = ObtieneCodEmple(Convert.ToInt32(iCodCatalogoEmple));
                            passwordEmple = Util.Decrypt(Session["passwordEmple"].ToString());

                            if (passwordEmple.Trim() == codAutoCod.Trim())
                            {
                                if (codNew.Trim() != "0")
                                {
                                    if (passwordEmple.Trim() == codAutoCod.Trim())
                                    {
                                        string newPassword = Util.Encrypt(codNew.Trim());
                                        int idEmpleFCA = Convert.ToInt32(Session["iCodCatEmpleFCA"]);
                                        string usuario = Session["T_id"].ToString().Trim();
                                        string pas = newPassword;
                                        string sp = "EXEC [GeneraUsuariosEmpleV2] @Usuario = '{0}',@Password = '{1}',@Emple = {2}";
                                        string q = string.Format(sp, usuario, pas, idEmpleFCA);
                                        DSODataAccess.ExecuteNonQuery(q.ToString());
                                    }
                                }
                                else
                                {
                                    /*SI EL EMPLEADO YA NO TIENE CODIGOS ASIGNADOS SE DA DE BAJA EL USUARIO*/
                                    BajaUsuariosEmple(Convert.ToInt32(iCodCatalogoEmple));
                                }
                            }
                            else
                            {
                                if (codNew.Trim() == "0")
                                {
                                    /*SI EL EMPLEADO YA NO TIENE CODIGOS ASIGNADOS SE DA DE BAJA EL USUARIO*/
                                    BajaUsuariosEmple(Convert.ToInt32(iCodCatalogoEmple));
                                }

                            }
                        }

                    }
                    else   //La Fecha de Fin no es valida
                    {
                        txtFechaFinCodAuto.Text = string.Empty;
                        txtFechaFinCodAuto.Focus();
                        throw new ArgumentException("La fecha fin debe ser mayor a la fecha de inicio, favor de seleccionar una fecha valida.");
                    }
                }
                else   //La Fecha de Fin no es valida
                {
                    txtFechaFinCodAuto.Text = string.Empty;
                    txtFechaFinCodAuto.Focus();
                    throw new ArgumentException("La fecha fin que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                }
            }
            else
            {
                dtCodAuto.Clear();
                FillCodAutoGrid();
                throw new ArgumentException("El código de autorización no existe o ya no se encuentra activo.");
            }
        }

        private string ObtieneCodEmple(int emple)
        {
            string codigo = "0";
            StringBuilder q = new StringBuilder();
            q.AppendLine(" DECLARE @Cant INT,@Codigo VARCHAR(20)");
            q.AppendLine(" SELECT @Cant = COUNT(*)");
            q.AppendLine(" FROM FCA.[VisRelaciones('Empleado - CodAutorizacion','Español')]WITH(NOLOCK)");
            q.AppendLine(" WHERE dtIniVigencia<>dtFinVigencia");
            q.AppendLine(" AND dtFinVigencia >= GETDATE()");
            q.AppendLine(" AND Emple = " + emple + "");
            q.AppendLine(" IF(@Cant > 0)");
            q.AppendLine(" BEGIN");
            q.AppendLine(" SELECT TOP 1 @Codigo = CodAutoCod");
            q.AppendLine(" FROM FCA.[VisRelaciones('Empleado - CodAutorizacion','Español')] WITH(NOLOCK)");
            q.AppendLine(" WHERE dtIniVigencia<>dtFinVigencia");
            q.AppendLine(" AND dtFinVigencia >= GETDATE()");
            q.AppendLine(" AND Emple = " + emple + "");
            q.AppendLine(" ORDER BY dtIniVigencia DESC");
            q.AppendLine(" END");
            q.AppendLine(" ELSE");
            q.AppendLine(" BEGIN");
            q.AppendLine(" SET @Codigo= '0'");
            q.AppendLine(" END");
            q.AppendLine(" SELECT ISNULL(@Codigo,'0') AS CODIGO");

            DataTable dt = DSODataAccess.Execute(q.ToString());
            if (dt != null && dt.Rows.Count >= 0)
            {
                DataRow dr = dt.Rows[0];
                codigo = dr["CODIGO"].ToString();
            }

            return codigo;
        }
        private void BajaUsuariosEmple(int emple)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendLine(" DECLARE @IcodRegistroUsuario INT,@IcodCatalogoUsuario INT");
                query.AppendLine(" SELECT");
                query.AppendLine(" @IcodRegistroUsuario = U.iCodRegistro,");
                query.AppendLine(" @IcodCatalogoUsuario = U.iCodCatalogo");
                query.AppendLine(" FROM FCA.HistEmple AS E");
                query.AppendLine(" JOIN FCA.[VisHistoricos('Usuar','Usuarios','Español')] AS U");
                query.AppendLine(" ON E.Usuar = U.iCodCatalogo");
                query.AppendLine(" AND U.dtIniVigencia<> U.dtFinVigencia");
                query.AppendLine(" AND U.dtFinVigencia >= GETDATE()");
                query.AppendLine(" WHERE E.dtIniVigencia<> E.dtFinVigencia");
                query.AppendLine(" AND E.dtFinVigencia >= GETDATE()");
                query.AppendLine(" AND E.iCodCatalogo = " + emple + "");
                query.AppendLine(" /*DA DE BAJA EL USUARIO ACTUAL PARA GENERAR UNO NUEVO*/");
                query.AppendLine(" UPDATE U");
                query.AppendLine(" SET U.dtFinVigencia = U.dtIniVigencia, U.dtFecUltAct = GETDATE()");
                query.AppendLine(" FROM FCA.HistEmple AS E");
                query.AppendLine(" JOIN FCA.[VisHistoricos('Usuar','Usuarios','Español')] AS U");
                query.AppendLine(" ON E.Usuar = U.iCodCatalogo");
                query.AppendLine(" AND U.dtIniVigencia<> U.dtFinVigencia");
                query.AppendLine(" AND U.dtFinVigencia >= GETDATE()");
                query.AppendLine(" WHERE E.dtIniVigencia<> E.dtFinVigencia");
                query.AppendLine(" AND E.dtFinVigencia >= GETDATE()");
                query.AppendLine(" AND E.iCodCatalogo =" + emple + "");
                query.AppendLine(" AND U.iCodRegistro = @IcodRegistroUsuario");
                query.AppendLine(" AND U.iCodCatalogo = @IcodCatalogoUsuario");

                DSODataAccess.ExecuteNonQuery(query.ToString());
            }
            catch
            {
                throw;
            }
        }
        private string ValidaCamposCodAuto(bool esMediantePopUp)
        {
            StringBuilder sbErrors = new StringBuilder(string.Empty);

            string codAuto = string.Empty;
            string sitio = string.Empty;
            string cos = string.Empty;
            string fechaIni = string.Empty;
            string fechaFin = string.Empty;
            TextBox fechaI;
            TextBox fechaF;

            if (esMediantePopUp)
            {
                codAuto = txtCodAuto.Text;
                sitio = drpSitioCodAuto.SelectedValue;
                cos = drpCosCodAuto.SelectedValue;
                fechaIni = txtFechaInicioCodAuto.Text;
                fechaFin = txtFechaFinCodAuto.Text;
                fechaI = txtFechaInicioCodAuto;
                fechaF = txtFechaFinCodAuto;
            }
            else
            {
                codAuto = txtCodAutoNoPopUp.Text;
                sitio = drpSitioCodAutoNoPopUp.SelectedValue;
                cos = drpCosCodAutoNoPopUp.SelectedValue;
                fechaIni = txtFechaInicioCodAutoNoPopUp.Text;
                fechaI = txtFechaInicioCodAutoNoPopUp;
                fechaF = new TextBox();  //cuando no es por PopUp es por que se esta haciendo una alta y aqui no hay fecha fin.
            }

            if (codAuto == string.Empty || codAuto == "")
            {
                sbErrors.Append(@"*El campo (Código) es requerido. \n");
            }
            else
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(codAuto, @"^\d*$"))
                {
                    sbErrors.Append(@"*El campo (Código) solo debe contener números. \n");
                }
                else
                {
                    char primerDato = codAuto[0];
                    bool digitosIguales = true;
                    for (int i = 0; i < codAuto.Length; i++)
                    {
                        if (codAuto[i] != primerDato)
                        {
                            digitosIguales = false;
                            break;
                        }
                    }

                    if (digitosIguales)
                    {
                        sbErrors.Append(@"*Los digitos en el campo (Código) no pueden ser todos iguales. \n");
                    }
                    else if (!string.IsNullOrEmpty(sitio) && codAuto.Length != GetLongitudCodAuto(sitio))
                    {
                        sbErrors.Append(@"*La Longitud del campo (Código) no es la configurada para el Sitio. \n");
                    }
                }
            }

            if (string.IsNullOrEmpty(sitio))
            {
                sbErrors.Append(@"*El campo (Sitio) es requerido. \n");
            }

            if (string.IsNullOrEmpty(cos))
            {
                sbErrors.Append(@"*El campo (Cos) es requerido. \n");
            }

            if (fechaIni == string.Empty || fechaIni == "")
            {
                sbErrors.Append(@"*El campo (Fecha inicio) es requerido. \n");
            }

            if (!validaFormatoFecha(fechaIni))
            {
                sbErrors.Append(@"*El formato de (Fecha inicio) debe ser DD/MM/AAAA. Favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA. \n");
                fechaI.Text = string.Empty;
            }

            if (esMediantePopUp && (fechaFin == string.Empty || fechaFin == ""))
            {
                sbErrors.Append(@"*El campo (Fecha Fin) es requerido. \n");
            }

            if (esMediantePopUp && !validaFormatoFecha(fechaFin))
            {
                sbErrors.Append(@"*El formato de (Fecha Fin) debe ser DD/MM/AAAA. Favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA. \n");
                fechaF.Text = string.Empty;
            }

            return sbErrors.ToString();
        }

        public void limpiaCamposNoPopUpCodAuto()
        {
            txtCodAutoNoPopUp.Text = null;
            drpSitioCodAutoNoPopUp.Text = null;
            txtFechaInicioCodAutoNoPopUp.Text = null;
            drpCosCodAutoNoPopUp.Text = null;
        }

        public void cargaPropControlesAlEditarCodAuto()
        {
            //Controles de pop-up
            txtCodAuto.Enabled = false;
            txtCodAuto.ReadOnly = true;
            drpSitioCodAuto.Enabled = false;
            drpCosCodAuto.Enabled = true;
            txtFechaInicioCodAuto.Enabled = true;
            txtFechaInicioCodAuto.ReadOnly = false;
            lblFechaFinCodAuto.Visible = true;
            txtFechaFinCodAuto.Visible = true;
            txtFechaFinCodAuto.Enabled = false; //NZ Se cambia para que solo pueda editar la fecha fin en la baja.

            //Se cambian los titulos del pop-up de codigos y el texto del boton
            lblTituloPopUpCodAuto.Text = "Detalle de códigos";
            btnGuardarCodAuto.Text = "Guardar";
            btnGuardarCodAuto.Enabled = true;

            //Control para realizar proceso de edicion
            cbEditarCodAuto.Checked = true;
            cbBajaCodAuto.Checked = false;
        }

        public void cargaPropControlesAlBorrarCodAuto()
        {
            //Controles de pop-up
            txtCodAuto.Enabled = false;
            txtCodAuto.ReadOnly = true;
            drpSitioCodAuto.Enabled = false;
            drpCosCodAuto.Enabled = false;
            txtFechaInicioCodAuto.Enabled = false;
            txtFechaInicioCodAuto.ReadOnly = true;
            lblFechaFinCodAuto.Visible = true;
            txtFechaFinCodAuto.Visible = true;
            txtFechaFinCodAuto.Enabled = true;

            //Se cambia la leyenda del pop-up
            lblTituloPopUpCodAuto.Text = "¿Esta seguro que desea dar de baja el código de autorización ?";
            btnGuardarCodAuto.Text = "Eliminar";
            btnGuardarCodAuto.Enabled = true;

            //Control para realizar proceso de baja
            cbEditarCodAuto.Checked = false;
            cbBajaCodAuto.Checked = true;
        }

        //Editar fila codigos autorización
        protected void grvCodAuto_EditRow(object sender, ImageClickEventArgs e)
        {
            ImageButton ibtn2 = sender as ImageButton;
            int rowIndex = Convert.ToInt32(ibtn2.Attributes["RowIndex"]);

            //Use this rowIndex in your code 
            GridViewRow selectedRow = (GridViewRow)grvCodAuto.Rows[rowIndex];

            int iCodCodAut = (int)grvCodAuto.DataKeys[rowIndex].Values[0];
            int iCodSitio = (int)grvCodAuto.DataKeys[rowIndex].Values[1];
            int iCodCos = (int)grvCodAuto.DataKeys[rowIndex].Values[2];
            int iCodRegRelEmpCodAuto = (int)grvCodAuto.DataKeys[rowIndex].Values[3];

            //Se llenan los controles del pop-up
            txtCodAuto.Text = selectedRow.Cells[0].Text;
            drpSitioCodAuto.Text = iCodSitio.ToString();

            FillDDLCosFiltro(drpCosCodAuto, iCodSitio.ToString());
            drpCosCodAuto.Text = iCodCos.ToString();

            txtFechaInicioCodAuto.Text = selectedRow.Cells[3].Text;
            txtFechaFinCodAuto.Text = selectedRow.Cells[4].Text;
            txtRegistroRelacionCodAuto.Text = iCodRegRelEmpCodAuto.ToString();

            cargaPropControlesAlEditarCodAuto();
            mpeCodAuto.Show();
        }

        //Borrar fila codigos autorización
        protected void grvCodAuto_DeleteRow(object sender, ImageClickEventArgs e)
        {
            ImageButton ibtn2 = sender as ImageButton;
            int rowIndex = Convert.ToInt32(ibtn2.Attributes["RowIndex"]);

            //Use this rowIndex in your code 
            GridViewRow selectedRow = (GridViewRow)grvCodAuto.Rows[rowIndex];

            int iCodCodAut = (int)grvCodAuto.DataKeys[rowIndex].Values[0];
            int iCodSitio = (int)grvCodAuto.DataKeys[rowIndex].Values[1];
            int iCodCos = (int)grvCodAuto.DataKeys[rowIndex].Values[2];
            int iCodRegRelEmpCodAuto = (int)grvCodAuto.DataKeys[rowIndex].Values[3];

            //Se llenan los controles del pop-up
            txtCodAuto.Text = selectedRow.Cells[0].Text;
            drpSitioCodAuto.Text = iCodSitio.ToString();

            FillDDLCosFiltro(drpCosCodAuto, iCodSitio.ToString());
            drpCosCodAuto.Text = iCodCos.ToString();

            txtFechaInicioCodAuto.Text = selectedRow.Cells[3].Text;
            txtFechaFinCodAuto.Text = string.Empty;
            txtRegistroRelacionCodAuto.Text = iCodRegRelEmpCodAuto.ToString();

            cargaPropControlesAlBorrarCodAuto();
            mpeCodAuto.Show();
        }

        //NZ 20160711 Se agrega funcionalidad para autogenerar un codigo de autorización
        protected void btnAutoGenerarCodigoNoPopUp_Click(object sender, EventArgs e)
        {
            try
            {
                if (drpSitioCodAutoNoPopUp.SelectedIndex != 0 && drpSitioCodAutoNoPopUp.SelectedIndex != -1)
                {
                    string iCodSitio = drpSitioCodAutoNoPopUp.Text;
                    int longitudCod = GetLongitudCodAuto(iCodSitio);
                    if (longitudCod > 0)
                    {
                        txtCodAutoNoPopUp.Text = DALCCustodia.AutogenerarCodAuto(Convert.ToInt32(iCodSitio), longitudCod);
                    }
                    else
                    {
                        throw new ArgumentException("No se tiene configurada un longitud para los códigos de este sitio. Favor de primero configurarlo.");
                    }
                }
                else
                {
                    throw new ArgumentException("Es necesario que primero se seleccione el sitio del código para la autogeneración.");
                }
            }
            catch (ArgumentException ex)
            {
                MensajeDeAdvertencia(ex.Message);
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al intentar autogenerar un código de autorización '" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        private int GetLongitudCodAuto(string iCodSitio)
        {
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("SELECT SUM(ISNULL(LongCodAuto,0) + ISNULL(LongCasilla,0)) AS LongCodAuto ");  //Se le suma la longitud de la casilla.
            lsbQuery.AppendLine("FROM [VisHistoricos('Sitio','Español')]");
            lsbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            lsbQuery.AppendLine(" AND dtFinvigencia >= GETDATE()");
            lsbQuery.AppendLine(" AND iCodCatalogo = " + iCodSitio);
            DataRow dtLongitud = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());
            if (dtLongitud != null)
            {
                return Convert.ToInt32(dtLongitud["LongCodAuto"]);
            }
            else { return 0; }
        }

        #endregion


        #region Líneas

        protected void btnGuardar_PopUpLinea(object sender, EventArgs e)
        {
            //Cuando es Edición o Baja
            btnGuardarLinea.Enabled = false;

            try
            {
                string tipoABC = string.Empty;
                string validaCampos = ValidaCamposLineEditarBorrar();

                //Se valida que los campos necesarios no esten vacios y tengan el formato correcto.
                if (validaCampos.Length > 0)
                {
                    throw new ArgumentException(validaCampos);
                }
                else
                {   //Los datos son correctos.

                    //Si se llega al pop-up mediante dar clic en el boton de edicion entonces llamara al proceso de edicion 
                    if (cbEditarLinea.Checked == true)
                    {
                        EdicionDeLinea();
                        tipoABC = "C";
                    }
                    else if (cbBajaLinea.Checked == true)
                    {
                        BajaDeLinea();
                        tipoABC = "B";
                    }

                    lsbQuery.Length = 0;
                    lsbQuery.AppendLine("SELECT iCodCatalogo FROM [VisHistoricos('Linea','Lineas','Español')] WITH(NOLOCK)");
                    lsbQuery.AppendLine("WHERE dtInivigencia <> dtFinvigencia");
                    lsbQuery.AppendLine("    AND dtFinvigencia >= GETDATE()");
                    lsbQuery.AppendLine("    AND TEL = '" + NoSQLCode(txtLinea.Text) + "'");
                    //lsbQuery.AppendLine("    AND Sitio = " + NoSQLCode(drpSitioLinea.Text));
                    lsbQuery.AppendLine("    AND Carrier = " + NoSQLCode(drpCarrierLinea.Text));
                    DataRow drLinea = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

                    if (drLinea != null)
                    {
                        DALCCustodia dalCC = new DALCCustodia();
                        dalCC.guardaHistRecurso(drLinea["iCodCatalogo"].ToString(), "Linea", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), tipoABC);
                    }

                    CambiarEstatusCCust(1);
                    dtLinea.Clear();
                    FillLineaGrid();
                    MensajeProcesoExitoso();
                }
            }
            catch (ArgumentException ex)
            {
                MensajeDeAdvertencia(ex.Message.ToString());
                mpeLinea.Show(); //Aquí aplica esta linea por que los movimientos se estan haciendo desde un Modal.
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al intentar guardar la línea '" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        protected void lbtnGuardar_LineaNoPopUp(object sender, EventArgs e)
        {
            //Cuando es Alta. Puesto que no se usa un PopUp Sino que los campos estan fijos en la pagina.
            lbtnGuardarLineaNoPopUp.Enabled = false;
            try
            {
                if (rbtnAlta.Checked == true)
                {
                    string tipoABC = string.Empty;
                    string validaCampos = ValidaCamposLineaAlta();

                    //Se valida que los campos necesarios no esten vacios y tengan el formato correcto.
                    if (validaCampos.Length > 0)
                    {
                        throw new ArgumentException(validaCampos);
                    }
                    else
                    {
                        //if (DSODataContext.Schema.ToUpper() == "BAT")
                        //{
                        //    ValidaExisteLinea();
                        //}

                        int res = ProcesoAltaDeLinea();

                        tipoABC = "A";

                        lsbQuery.Length = 0;
                        lsbQuery.AppendLine("SELECT iCodCatalogo FROM [VisHistoricos('Linea','Lineas','Español')]");
                        lsbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
                        lsbQuery.AppendLine("    AND dtFinVigencia >= GETDATE()");
                        lsbQuery.AppendLine("    AND Tel = '" + NoSQLCode(txtLineaNoPopUp.Text) + "'");
                        lsbQuery.AppendLine("    AND Sitio = " + NoSQLCode(drpSitioLineaNoPopUp.Text));
                        lsbQuery.AppendLine("    AND Carrier = " + NoSQLCode(drpCarrierLineaNoPopUp.Text));
                        DataRow drLinea = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

                        if (drLinea != null)
                        {
                            DALCCustodia dalCC = new DALCCustodia();
                            dalCC.guardaHistRecurso(drLinea["iCodCatalogo"].ToString(), "Linea", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), tipoABC);
                        }

                        if (DSODataContext.Schema.ToUpper() == "FCA")
                        {
                            //RM20190401 Alta Campos Linea FCA 
                            AltaLineaCamposFCA();
                        }

                        CambiarEstatusCCust(1);
                        dtLinea.Clear();
                        FillLineaGrid();
                        string mensajeIccd = "";
                        if (res == 1)
                        {
                            txtICCIDAlta.Text = "";
                            mensajeIccd = "\n El ICCID ya existe Asiganada a otra linea, para agregar uno difrenten \n es necesario hacerlo de la opcion editar.";
                        }

                        MensajeProcesoExitoso(mensajeIccd);
                        LimpiaCamposNoPopUpLinea();
                    }
                }
                else
                {
                    string validaCampos = ValidaCamposLineaAsignacion();
                    if (validaCampos.Length > 0)
                    {
                        throw new ArgumentException(validaCampos);
                    }
                    else
                    {
                        AltaRelacionEmpleLinea();

                        CambiarEstatusCCust(1);
                        dtLinea.Clear();
                        FillLineaGrid();
                        MensajeProcesoExitoso("Asignacion de linea");
                        LimpiaCamposNoPopUpLinea();
                    }
                }

            }
            catch (ArgumentException ex)
            {
                MensajeDeAdvertencia(ex.Message);
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al intentar guardar la línea '" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        public bool AltaLineaCamposFCA()
        {
            try
            {
                bool altaExitosa = false;

                string lineaAlta = txtLineaNoPopUp.Text;

                string PentaSAPAccount = ddlPentaSAPAccount.Text;
                string PentaSAPCCDesc = ddlPentaSAPCCDesc.Text;
                string PentaSAPCostCenter = ddlPentaSAPCostCenter.Text;
                string PentaSAPFAFCA = ddlPentaSAPFAFCA.Text;
                string PentaSAPProfitCenter = ddlPentaSAPProfitCenter.Text;


                string queryAltaLineaFCA = ConsultaAltaLineaFCA(
                                                                    lineaAlta,
                                                                    PentaSAPAccount,
                                                                    PentaSAPProfitCenter,
                                                                    PentaSAPCostCenter,
                                                                    PentaSAPFAFCA,
                                                                    PentaSAPCCDesc
                                                                );


                DataTable dt = DSODataAccess.Execute(queryAltaLineaFCA);


                return altaExitosa;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ConsultaAltaLineaFCA
            (string LineaAlta,
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

        public int ProcesoAltaDeLinea()
        {
            //Mapeo campos
            string codLinea = NoSQLCode(txtLineaNoPopUp.Text);
            int carrierIndex = drpCarrierLineaNoPopUp.SelectedIndex;
            int sitioIndex = drpSitioLineaNoPopUp.SelectedIndex;
            int existe = 0;
            DateTime dtFechaInicio = Convert.ToDateTime(txtFechaInicioLineaNoPopUp.Text);
            string iCodCatCarrier = NoSQLCode(drpCarrierLineaNoPopUp.Text);
            string nomCarrier = drpCarrierLineaNoPopUp.SelectedItem.Text;
            string iCodCatSitio = NoSQLCode(drpSitioLineaNoPopUp.Text);
            string vchCodigoEmple = NoSQLCode(txtNominaEmple.Text);

            DALCCustodia lineaCC = new DALCCustodia();


            //Query para ver si la línea existe
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("SELECT iCodCatalogo, Tel FROM [VisHistoricos('Linea','Lineas','Español')]");
            lsbQuery.AppendLine("WHERE dtInivigencia <> dtFinvigencia");
            lsbQuery.AppendLine("  AND dtFinvigencia >= GETDATE()");
            lsbQuery.AppendLine("  AND Tel = '" + codLinea + "'");
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
                                            "WHERE iCodCatalogo = " + iCodCatalogoEmple +
                                            "AND dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()").ToString();

                    DateTime dtFechaInicioEmple = Convert.ToDateTime(lsdtFechaInicioEmple);

                    //Si la fecha de inicio es mayor o igual a la fecha de alta del empleado entra a este bloque
                    if (dtFechaInicio >= dtFechaInicioEmple)
                    {
                        // Se hace un insert en [VisRelaciones('Empleado - Linea','Español')]  // Se hace un update a la vista [VisHistoricos('Linea','Lineas','Español')] en el campo Emple 
                        lineaCC.AltaRelacionEmpLinea(vchCodigoEmple, lsVchCodLinea, iCodCatalogoEmple, lsiCodCatalgoLinea, dtFechaInicio);
                        existe = (!string.IsNullOrEmpty(NoSQLCode(txtICCIDAlta.Text))) ? lineaCC.AltaICCIDLINEA(lsiCodCatalgoLinea, txtICCIDAlta.Text) : 0;

                    }
                    else  //Si la fecha de inicio es menor a la fecha de alta del empleado manda un mensaje de notificación
                    {
                        throw new ArgumentException("La fecha de inicio debe ser mayor o igual a la fecha de alta del empleado.");
                    }
                }
                else //La fecha de inicio no es valida
                {
                    txtFechaInicioLineaNoPopUp.Text = string.Empty;
                    txtFechaInicioLineaNoPopUp.Focus();
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
                lsbQuery.AppendLine("  AND Tel = '" + codLinea + "'");
                lsbQuery.AppendLine("  AND Sitio = " + iCodCatSitio);
                lsbQuery.AppendLine("  AND Carrier = " + iCodCatCarrier);

                DataRow drLineaPasado = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

                if (drLineaPasado != null)
                {
                    //Se valida traslape por si en el pasado ya habia existido.
                    string fechaInicioValida = ValidarTraslapeFechas(dtFechaInicio, drLineaPasado["iCodCatalogo"].ToString(), "Linea", "Empleado - Linea", "null");
                    if (fechaInicioValida != "1")
                    {
                        txtFechaInicioLineaNoPopUp.Text = string.Empty;
                        txtFechaInicioLineaNoPopUp.Focus();
                        throw new ArgumentException("La fecha que selecciono entra dentro de los rangos de relación de otro empleado, favor de seleccionar una fecha validá.");
                    }
                }

                //Validación para que la fecha inicio de la linea no pueda ser menor a la fecha inicio del empleado
                string lsdtFechaInicioEmple = DSODataAccess.ExecuteScalar("SELECT dtIniVigencia FROM [VisHistoricos('Emple','Empleados','Español')] " +
                                        "WHERE iCodCatalogo = " + iCodCatalogoEmple +
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
                        lineaCC.AltaLinea(codLinea.Trim(), iCodCatCarrier, iCodCatSitio, dtFechaInicio, "0", codLinea+" ("+ nomCarrier+")");

                        //Query para extraer datos de la línea que se acaba de dar de alta
                        lsbQuery.Length = 0;
                        lsbQuery.AppendLine("SELECT iCodCatalogo,vchCodigo FROM [VisHistoricos('Linea','Lineas','Español')]");
                        lsbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
                        lsbQuery.AppendLine("  AND dtFinVigencia >= GETDATE()");
                        lsbQuery.AppendLine("  AND Tel = '" + codLinea + "'");
                        lsbQuery.AppendLine("  AND Sitio = " + iCodCatSitio);
                        lsbQuery.AppendLine("  AND Carrier = " + iCodCatCarrier);
                        DataRow drLineaReciente = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

                        string lsiCodCatalgoLinea = drLineaReciente["iCodCatalogo"].ToString();
                        string lsVchCodLinea = drLineaReciente["vchCodigo"].ToString();

                        // Se hace un insert en [VisRelaciones('Empleado - Linea','Español')]  // Se hace un update a la vista [VisHistoricos('Linea','Lineas','Español')] en el campo Emple 
                        lineaCC.AltaRelacionEmpLinea(vchCodigoEmple, lsVchCodLinea, iCodCatalogoEmple, lsiCodCatalgoLinea, dtFechaInicio);

                        existe = (!string.IsNullOrEmpty(NoSQLCode(txtICCIDAlta.Text))) ? lineaCC.AltaICCIDLINEA(lsiCodCatalgoLinea, txtICCIDAlta.Text) : 0;
                    }
                }
                else  //Si la fecha de inicio es menor a la fecha de alta del empleado manda un mensaje de notificación
                {
                    throw new ArgumentException("La fecha de inicio debe ser mayor o igual a la fecha de alta del empleado.");
                }
                #endregion
            }
            return existe;
        }
        public void ValidaExisteLinea()
        {
            string codLinea = NoSQLCode(txtLineaNoPopUp.Text);
            int carrierIndex = drpCarrierLineaNoPopUp.SelectedIndex;
            int sitioIndex = drpSitioLineaNoPopUp.SelectedIndex;
            DateTime dtFechaInicio = Convert.ToDateTime(txtFechaInicioLineaNoPopUp.Text);
            string iCodCatCarrier = NoSQLCode(drpCarrierLineaNoPopUp.Text);
            string iCodCatSitio = NoSQLCode(drpSitioLineaNoPopUp.Text);
            string vchCodigoEmple = NoSQLCode(txtNominaEmple.Text);

            DALCCustodia lineCC = new DALCCustodia();

            lsbQuery.Length = 0;
            lsbQuery.AppendLine("SELECT iCodCatalogo, Tel FROM [VisHistoricos('Linea','Lineas','Español')]");
            lsbQuery.AppendLine("WHERE dtInivigencia <> dtFinvigencia");
            lsbQuery.AppendLine("  AND dtFinvigencia >= GETDATE()");
            lsbQuery.AppendLine("  AND Tel = '" + codLinea + "'");
            lsbQuery.AppendLine("  AND Sitio = " + iCodCatSitio);
            lsbQuery.AppendLine("  AND Carrier = " + iCodCatCarrier);
            DataRow drExisteLinea = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());
            //La línea ya existe ?
            if (drExisteLinea != null)
            {
                string lsiCodCatalogoLinea = drExisteLinea["iCodCatalogo"].ToString();

                //Query para ver si la línea ya tiene una relación con otro empleado.
                lsbQuery.Length = 0;
                lsbQuery.AppendLine("SELECT Emple, EmpleDesc FROM [VisRelaciones('Empleado - Linea','Español')]");
                lsbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
                lsbQuery.AppendLine("  AND dtFinvigencia >= GETDATE()");
                lsbQuery.AppendLine("  AND Linea = " + lsiCodCatalogoLinea);
                DataRow drRelEmpLinea = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());
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
                                            "WHERE iCodCatalogo = " + iCodCatalogoEmple +
                                            "AND dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()").ToString();

                    DateTime dtFechaInicioEmple = Convert.ToDateTime(lsdtFechaInicioEmple);

                    //Si la fecha de inicio es mayor o igual a la fecha de alta del empleado entra a este bloque
                    if (dtFechaInicio >= dtFechaInicioEmple)
                    {
                        // Se hace un insert en [VisRelaciones('Empleado - Linea','Español')]  // Se hace un update a la vista [VisHistoricos('Linea','Lineas','Español')] en el campo Emple 
                        lineCC.AltaRelacionEmpLinea(vchCodigoEmple, lsVchCodLinea, iCodCatalogoEmple, lsiCodCatalgoLinea, dtFechaInicio);
                    }
                    else  //Si la fecha de inicio es menor a la fecha de alta del empleado manda un mensaje de notificación
                    {
                        throw new ArgumentException("La fecha de inicio debe ser mayor o igual a la fecha de alta del empleado.");
                    }
                }
                else //La fecha de inicio no es valida
                {
                    txtFechaInicioLineaNoPopUp.Text = string.Empty;
                    txtFechaInicioLineaNoPopUp.Focus();
                    throw new ArgumentException("La fecha que seleccionó entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                }
            }
            else//la linea no existe
            {
                throw new ArgumentException("No existe la Linea para el Carrier seleccionado. Asegurese de haber seleccionado el carrier correcto o que la linea ya exista.");
            }
        }
        public void AltaRelacionEmpleLinea()
        {
            string codLinea = NoSQLCode(txtLineaNoPopUp.Text);
            DateTime dtFechaInicio = Convert.ToDateTime(txtFechaInicioLineaNoPopUp.Text);
            string vchCodigoEmple = NoSQLCode(txtNominaEmple.Text);

            DALCCustodia lineCC = new DALCCustodia();

            lsbQuery.Length = 0;
            lsbQuery.AppendLine("SELECT iCodCatalogo, Tel FROM [VisHistoricos('Linea','Lineas','Español')]");
            lsbQuery.AppendLine("WHERE dtInivigencia <> dtFinvigencia");
            lsbQuery.AppendLine("  AND dtFinvigencia >= GETDATE()");
            lsbQuery.AppendLine("  AND Tel = '" + codLinea.Trim() + "'");
            DataRow drExisteLinea = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());
            //La línea ya existe ?
            if (drExisteLinea != null)
            {
                string lsiCodCatalogoLinea = drExisteLinea["iCodCatalogo"].ToString();

                //Query para ver si la línea ya tiene una relación con otro empleado.
                lsbQuery.Length = 0;
                lsbQuery.AppendLine("SELECT Emple, EmpleDesc FROM [VisRelaciones('Empleado - Linea','Español')]");
                lsbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
                lsbQuery.AppendLine("  AND dtFinvigencia >= GETDATE()");
                lsbQuery.AppendLine("  AND Linea = " + lsiCodCatalogoLinea);
                DataRow drRelEmpLinea = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());
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
                    string lsVchCodLinea = drExisteLinea["Tel"].ToString();

                    //Validación para que la fecha inicio de la línea no pueda ser menor a la fecha inicio del empleado
                    string lsdtFechaInicioEmple = DSODataAccess.ExecuteScalar("SELECT dtIniVigencia FROM [VisHistoricos('Emple','Empleados','Español')] " +
                                            "WHERE iCodCatalogo = " + iCodCatalogoEmple +
                                            "AND dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()").ToString();

                    DateTime dtFechaInicioEmple = Convert.ToDateTime(lsdtFechaInicioEmple);

                    //Si la fecha de inicio es mayor o igual a la fecha de alta del empleado entra a este bloque
                    if (dtFechaInicio >= dtFechaInicioEmple)
                    {
                        // Se hace un insert en [VisRelaciones('Empleado - Linea','Español')]  // Se hace un update a la vista [VisHistoricos('Linea','Lineas','Español')] en el campo Emple 
                        lineCC.AltaRelacionEmpLinea(vchCodigoEmple, lsVchCodLinea, iCodCatalogoEmple, lsiCodCatalgoLinea, dtFechaInicio);
                    }
                    else  //Si la fecha de inicio es menor a la fecha de alta del empleado manda un mensaje de notificación
                    {
                        throw new ArgumentException("La fecha de inicio debe ser mayor o igual a la fecha de alta del empleado.");
                    }
                }
                else //La fecha de inicio no es valida
                {
                    txtFechaInicioLineaNoPopUp.Text = string.Empty;
                    txtFechaInicioLineaNoPopUp.Focus();
                    throw new ArgumentException("La fecha que seleccionó entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                }
            }
            else//la linea no existe
            {
                throw new ArgumentException("La linea que intenta asignar no existe.");
            }
        }
        public void EdicionDeLinea()
        {
            //Mapeo de campos
            string lineaCod = NoSQLCode(txtLinea.Text);
            string iCodSitio = NoSQLCode(drpSitioLinea.Text);
            DateTime dtFechaInicio = Convert.ToDateTime(txtFechaInicioLinea.Text);
            DateTime dtFechaFin = Convert.ToDateTime(txtFechaFinLinea.Text);
            string iCodRegistroRel = NoSQLCode(txtRegistroRelacionLinea.Text);
            string iCodCarrier = NoSQLCode(drpCarrierLinea.Text);
            string iccid = NoSQLCode(txtICCID.Text);

            //Query para extraer el iCodCatalogo de la línea
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("SELECT iCodCatalogo FROM [VisHistoricos('Linea','Lineas','Español')] WITH(NOLOCK)");
            lsbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            lsbQuery.AppendLine("   AND dtfinvigencia >= GETDATE()");
            lsbQuery.AppendLine("   AND TEL = '" + lineaCod + "'");
            //lsbQuery.AppendLine("   AND Sitio = " + iCodSitio);
            lsbQuery.AppendLine("   AND Carrier = " + iCodCarrier);
            lsbQuery.AppendLine("   AND iCodCatalogo IN(");
            lsbQuery.AppendLine("                       SELECT Linea");
            lsbQuery.AppendLine("                       FROM [VisRelaciones('Empleado - Linea','Español')] WITH(NOLOCK)");
            lsbQuery.AppendLine("                       WHERE dtIniVigencia<>dtFinVigencia");
            lsbQuery.AppendLine("                               AND dtFinVigencia >= GETDATE()");
            lsbQuery.AppendLine("                               AND Emple = " + iCodCatalogoEmple);
            lsbQuery.AppendLine("                       )");
            DataRow drLinea = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

            if (drLinea != null)
            {
                string iCodCatalogoLinea = drLinea["iCodCatalogo"].ToString();
                string lsdtFechaInicioEmple = DSODataAccess.ExecuteScalar("SELECT dtIniVigencia FROM [VisHistoricos('Emple','Empleados','Español')] " +
                                                               "WHERE iCodCatalogo = " + iCodCatalogoEmple +
                                                               "AND dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()").ToString();

                DateTime dtFechaInicioEmple = Convert.ToDateTime(lsdtFechaInicioEmple);

                //Si la fecha de inicio es mayor o igual a la fecha de alta del empleado entra a este bloque
                //if (dtFechaInicio >= dtFechaInicioEmple)
                //{
                //    string fechaInicioValida = ValidarTraslapeFechas(dtFechaInicio, iCodCatalogoLinea, "Linea", "Empleado - Linea", iCodRegistroRel);

                //    if (fechaInicioValida == "1")
                //    {
                //        string fechaFinValida = ValidarTraslapeFechas(dtFechaFin, iCodCatalogoLinea, "Linea", "Empleado - Linea", iCodRegistroRel);

                //        if (fechaFinValida == "1")
                //        {
                if (dtFechaFin >= dtFechaInicio)
                {
                    /*VALIDAR QUE ICCID NO ESTE RELACIONADO A OTRA LINEA*/

                    DALCCustodia dalCCust = new DALCCustodia();

                    string ctaMaestra = (drpCtaMaestraLinea.SelectedIndex != -1 && drpCtaMaestraLinea.SelectedIndex != 0) ? NoSQLCode(drpCtaMaestraLinea.Text) : "NULL";
                    string razonSocial = (drpRazonSocialLinea.SelectedIndex != -1 && drpRazonSocialLinea.SelectedIndex != 0) ? NoSQLCode(drpRazonSocialLinea.Text) : "0";
                    string tipoPlan = (drpTipoPlanLinea.SelectedIndex != -1 && drpTipoPlanLinea.SelectedIndex != 0) ? NoSQLCode(drpTipoPlanLinea.Text) : "NULL";
                    string tipoEquipo = (drpTipoEquipoLinea.SelectedIndex != -1 && drpTipoEquipoLinea.SelectedIndex != 0) ? NoSQLCode(drpTipoEquipoLinea.Text) : "NULL";
                    string planTarifario = (drpPlanTarifarioLinea.SelectedIndex != -1 && drpPlanTarifarioLinea.SelectedIndex != 0) ? NoSQLCode(drpPlanTarifarioLinea.Text) : "NULL";
                    string ubicacionLinea = (drpLineaSitio.SelectedIndex != -1 && drpLineaSitio.SelectedIndex != 0) ? NoSQLCode(drpLineaSitio.Text) : "0";
                    //string area = (drpAreaLinea.SelectedIndex != -1 && drpAreaLinea.SelectedIndex != 0) ? NoSQLCode(drpAreaLinea.Text) : "0";
                    
                    string area = (!string.IsNullOrEmpty(hdfAreaLinea.Value.ToString()))&&(!string.IsNullOrEmpty(NoSQLCode(txtAreaLinea.Text.ToString()))) ? NoSQLCode(hdfAreaLinea.Value.ToString()) : "0";

                    //string cenCosto = (drpCentroCosto.SelectedIndex != -1 && drpCentroCosto.SelectedIndex != 0) ? NoSQLCode(drpCentroCosto.Text) : "0";
                    string cenCosto = (!string.IsNullOrEmpty(hdfCentroCosto.Value.ToString())) && (!string.IsNullOrEmpty(NoSQLCode(txtCentroCosto.Text.ToString()))) ? NoSQLCode(hdfCentroCosto.Value.ToString()) : "0";

                    DateTime? finPlan = (!string.IsNullOrEmpty(txtFinPlanLinea.Text)) ? (DateTime?)Convert.ToDateTime(txtFinPlanLinea.Text) : null;
                    DateTime? fechaLimite = (!string.IsNullOrEmpty(txtFechaLimiteLinea.Text)) ? (DateTime?)Convert.ToDateTime(txtFechaLimiteLinea.Text) : null;
                    DateTime? fechaActivacion = (!string.IsNullOrEmpty(txtFechaActivacionLinea.Text)) ? (DateTime?)Convert.ToDateTime(txtFechaActivacionLinea.Text) : null;

                    string etiqueta = (!string.IsNullOrEmpty(NoSQLCode(txtEtiquetaLinea.Text))) ? "'" + NoSQLCode(txtEtiquetaLinea.Text) + "'" : "NULL";
                    string planLinea = (!string.IsNullOrEmpty(NoSQLCode(txtPlanLinea.Text))) ? "'" + NoSQLCode(txtPlanLinea.Text) + "'" : "NULL";
                    string imei = (!string.IsNullOrEmpty(NoSQLCode(txtIMEILinea.Text))) ? "'" + NoSQLCode(txtIMEILinea.Text) + "'" : "NULL";
                    //string modeloEquipo = (!string.IsNullOrEmpty(NoSQLCode(txtModeloEqLinea.Text))) ? "'" + NoSQLCode(txtModeloEqLinea.Text) + "'" : "NULL";
                    string modeloEquipo = (drpModeloEquipo.SelectedIndex != -1 && drpModeloEquipo.SelectedIndex != 0) ? "'" + NoSQLCode(drpModeloEquipo.SelectedItem.Text) + "'" : "NULL";
                    string idModelo = (drpModeloEquipo.SelectedIndex != -1 && drpModeloEquipo.SelectedIndex != 0) ? drpModeloEquipo.Text : "0";
                    string numOrden = (!string.IsNullOrEmpty(NoSQLCode(txtNumOrdenLinea.Text))) ? "'" + NoSQLCode(txtNumOrdenLinea.Text) + "'" : "NULL";
                    string cargoFijo = (!string.IsNullOrEmpty(NoSQLCode(txtCargoFijo.Text))) ? NoSQLCode(txtCargoFijo.Text) : "NULL";
                    string regionBat = (!string.IsNullOrEmpty(NoSQLCode(txtRegion.Text))) ? "'" + NoSQLCode(txtRegion.Text) + "'" : "NULL";/*campo Filler de la linea*/
                    string montoRenta = (!string.IsNullOrEmpty(NoSQLCode(txtMontoRenta.Text))) ? NoSQLCode(txtMontoRenta.Text) : "0";/*Monto Renta*/
                    string comentarios = (!string.IsNullOrEmpty(NoSQLCode(txtComentarios.Text))) ? "'" + NoSQLCode(txtComentarios.Text) + "'" : "NULL";
                    string asignacion = (drpCategoriaAsignacion.SelectedIndex != -1 && drpCategoriaAsignacion.SelectedIndex != 0) ? NoSQLCode(drpCategoriaAsignacion.Text) : "0";
                    string TipoAsignacion = (drpTipoAsignacion.SelectedIndex != -1 && drpTipoAsignacion.SelectedIndex != 0) ? NoSQLCode(drpTipoAsignacion.Text) : "0";
                    //RM 20190407 Campos FCA
                    int PentaSAPCostCenterPopUp = 0;
                    int PentaSAPFAPopUp = 0;
                    int PentaSAPCCDescriptionPopUp = 0;
                    int PentaSAPAccountPopUp = 0;
                    int PentaSAPProfitCenterPopUp = 0;


                    if (DSODataContext.Schema.ToLower() == "fca")
                    {
                        int.TryParse(ddlPentaSAPCostCenterPopUp.SelectedValue, out PentaSAPCostCenterPopUp);
                        int.TryParse(ddlPentaSAPFAPopUp.SelectedValue, out PentaSAPFAPopUp);
                        int.TryParse(ddlPentaSAPCCDescriptionPopUp.SelectedValue, out PentaSAPCCDescriptionPopUp);
                        int.TryParse(ddlPentaSAPAccountPopUp.SelectedValue, out PentaSAPAccountPopUp);
                        int.TryParse(ddlPentaSAPProfitCenterPopUp.SelectedValue, out PentaSAPProfitCenterPopUp);

                    }

                    //Calcular el valor de las banderas de la linea.
                    int valorBanderas = CalcularValoresDeBanderaLinea();
                    int existe = (!string.IsNullOrEmpty(NoSQLCode(txtICCID.Text))) ? dalCCust.AltaICCIDLINEA(iCodCatalogoLinea, txtICCID.Text) : 0;
                    int diferenteTipoEquipo = ValidaImeiEquipo(tipoEquipo, iCodCatalogoLinea, imei);
                    if(diferenteTipoEquipo == 0)
                    {
                        if (existe == 0)
                        {
                            dalCCust.EditLinea(iCodCatalogoLinea, iCodCatalogoEmple, ctaMaestra, razonSocial, tipoPlan, tipoEquipo, planTarifario, valorBanderas, cargoFijo,
                            fechaLimite, finPlan, fechaActivacion, etiqueta, planLinea, imei, modeloEquipo, numOrden, dtFechaInicio.ToString("yyyy-MM-dd HH:mm:ss"), Convert.ToDateTime(txtFechaFinLinea.Text).ToString("yyyy-MM-dd HH:mm:ss"), area, regionBat,
                            montoRenta, ubicacionLinea, cenCosto, comentarios, idModelo, asignacion, TipoAsignacion);

                            if (DSODataContext.Schema.ToUpper() == "FCA")
                            {
                                dalCCust.EditarLineaFCA(Convert.ToInt32(iCodCatalogoLinea), PentaSAPCostCenterPopUp, PentaSAPFAPopUp, PentaSAPCCDescriptionPopUp, PentaSAPAccountPopUp, PentaSAPProfitCenterPopUp);
                            }
                        }
                        else
                        {
                            txtICCID.Focus();
                            throw new ArgumentException("El ICCID ya existe Asiganada a otra linea\n");
                        }
                    }
                    else
                    {
                        string mensaje="";
                        if(lineaImei != "")
                        {
                            mensaje = "El imei ya se encuentra asignado a otro equipo con la linea : " + lineaImei;
                        }
                        else
                        {
                            mensaje = "El imei debe de ser diferente al cambiar de quipo\n";
                        }

                        throw new ArgumentException(mensaje);
                    }                 
                }
                else //La fecha de fin no es valida
                {
                    txtFechaFinLinea.Text = string.Empty;
                    txtFechaFinLinea.Focus();
                    throw new ArgumentException("La fecha fin debe ser mayor a la fecha de inicio, favor de seleccionar una fecha valida.");
                }
                //        }
                //        else
                //        {
                //            txtFechaFinLinea.Text = string.Empty;
                //            txtFechaFinLinea.Focus();
                //            throw new ArgumentException("La fecha fin que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                //        }
                //    }
                //    else  //La fecha de inicio no es valida
                //    {
                //        txtFechaInicioLinea.Text = string.Empty;
                //        txtFechaInicioLinea.Focus();
                //        throw new ArgumentException("La fecha que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                //    }
                //} //Si la fecha de inicio es menor a la fecha de alta del empleado manda un mensaje de notificación                
                //else
                //{
                //    throw new ArgumentException("La fecha de inicio debe ser mayor a la fecha de alta del empleado.");
                //}

            }
            else
            {
                dtLinea.Clear();
                FillLineaGrid();
                throw new ArgumentException("La línea no existe o ya no se encuentra activa.");
            }
        }
        public int ValidaImeiEquipo(string iCodCatEqCelular, string iCodCatLinea, string imei)
        {
            int tpEq = 0;
            int eqCelular = 0;
            int r = 1;
            if (iCodCatEqCelular != "NULL") { eqCelular = Convert.ToInt32(iCodCatEqCelular); }
            if (HttpContext.Current.Session["TipoEquipoLinea"] != null)
            {
                tpEq = Convert.ToInt32(HttpContext.Current.Session["TipoEquipoLinea"]);
            }

            if (eqCelular > 0 && Convert.ToInt32(iCodCatLinea) > 0)
            {
                if (eqCelular != tpEq)
                {
                    string imeLine = "";
                    string imeiL = imei.Replace("'", " ").Trim();
                    if (HttpContext.Current.Session["IMEILINEA"] != null) { imeLine = HttpContext.Current.Session["IMEILINEA"].ToString(); }
                    if (imeiL != "")
                    {
                        if (imeiL != imeLine)
                        {
                            /*valida ExisteImei en otro equipo*/
                            DataTable dt = ValidaExisteImei(imei, iCodCatEqCelular);
                            if (dt != null && dt.Rows.Count >= 0)
                            {
                                DataRow dr = dt.Rows[0];
                                int existe = Convert.ToInt32(dr["Existe"]);
                                lineaImei = dr["Linea"].ToString();
                                if (existe == 0)
                                {
                                    r = 0;
                                }
                            }
                            else
                            {
                                r = 0;
                            }

                        }

                    }
                }
                else
                {
                    r = 0;
                }
            }

            return r;
        }
        public DataTable ValidaExisteImei(string imei, string equipo)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine(" DECLARE @Equipo INT = " + equipo + "");
            query.AppendLine(" DECLARE @Existe INT = 0");
            query.AppendLine(" DECLARE @IcodEquipo INT = 0");
            query.AppendLine(" DECLARE @Linea VARCHAR(MAX)");
            query.AppendLine(" SELECT");
            query.AppendLine(" @IcodEquipo = EC.icodCatalogo,");
            query.AppendLine(" @Linea = LineaCod");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".[VisRelaciones('Linea-DispositivoEqCelular','Español')] AS RE WITH(NOLOCK)");
            query.AppendLine(" JOIN " + DSODataContext.Schema + ".[vishistoricos('DispositivoEqCelular','Dispositivos Equipo Celular','español')] AS EQ WITH(NOLOCK)");
            query.AppendLine(" ON RE.DispoSitivoEqCelular = EQ.iCodCatalogo");
            query.AppendLine(" AND EQ.dtIniVigencia<> EQ.dtFinVigencia");
            query.AppendLine(" AND EQ.dtFinVigencia >= GETDATE()");
            query.AppendLine(" JOIN " + DSODataContext.Schema + ".[VisHistoricos('ModeloEqCelular','Modelos equipo celular','Español')] AS MO WITH(NOLOCK)");
            query.AppendLine(" ON EQ.ModeloEqCelular = MO.iCodCatalogo");
            query.AppendLine(" AND MO.dtIniVigencia<> MO.dtFinVigencia");
            query.AppendLine(" AND MO.dtFinVigencia >= GETDATE()");
            query.AppendLine(" JOIN " + DSODataContext.Schema + ".[VisHistoricos('EqCelular','Equipo Celular','Español')] AS EC WITH(NOLOCK)");
            query.AppendLine(" ON MO.EqCelular = EC.icodCatalogo");
            query.AppendLine(" AND EC.dtIniVigencia<> EC.dtFinVigencia");
            query.AppendLine(" AND EC.dtFinVigencia >= GETDATE()");
            query.AppendLine(" WHERE RE.dtIniVigencia<> RE.dtFinVigencia");
            query.AppendLine(" AND RE.dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND EQ.IMEI = " + imei );
            query.AppendLine(" AND EQ.vchCodigo = " + imei );
            query.AppendLine(" IF(ISNULL(@IcodEquipo, 0) > 0)");
            query.AppendLine(" BEGIN");
            query.AppendLine(" IF(@IcodEquipo != @Equipo)");
            query.AppendLine(" BEGIN");
            query.AppendLine(" SET @Existe = 1");
            query.AppendLine(" END");
            query.AppendLine(" END");
            query.AppendLine(" SELECT @Existe AS Existe, ISNULL(@Linea, '')AS Linea");

            DataTable dt = DSODataAccess.Execute(query.ToString());

            return dt;
        }
        public void BajaDeLinea()
        {
            //Mapeo de campos
            string lineaCod = NoSQLCode(txtLinea.Text);
            string iCodSitio = NoSQLCode(drpSitioLinea.Text);
            DateTime dtFechaInicio = Convert.ToDateTime(txtFechaInicioLinea.Text);
            DateTime dtFechaFin = Convert.ToDateTime(txtFechaFinLinea.Text);
            string iCodRegistroRel = NoSQLCode(txtRegistroRelacionLinea.Text);
            string iCodCarrier = NoSQLCode(drpCarrierLinea.Text);

            //Query para extraer el iCodCatalogo de la Línea
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("SELECT iCodCatalogo FROM [VisHistoricos('Linea','Lineas','Español')]");
            lsbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            lsbQuery.AppendLine("   AND dtFinVigencia >= GETDATE()");
            lsbQuery.AppendLine("   AND Tel = '" + lineaCod + "'");
            //lsbQuery.AppendLine("   AND Sitio = " + iCodSitio);
            lsbQuery.AppendLine("   AND Carrier = " + iCodCarrier);
            lsbQuery.AppendLine("   AND iCodCatalogo IN(");
            lsbQuery.AppendLine("                       SELECT Linea");
            lsbQuery.AppendLine("                       FROM [VisRelaciones('Empleado - Linea','Español')]");
            lsbQuery.AppendLine("                       WHERE dtIniVigencia<>dtFinVigencia");
            lsbQuery.AppendLine("                               AND dtFinVigencia >= GETDATE()");
            lsbQuery.AppendLine("                               AND Emple = " + iCodCatalogoEmple);
            lsbQuery.AppendLine("                       )");
            DataRow drLinea = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

            if (drLinea != null)
            {
                string iCodCatalogoLinea = drLinea["iCodCatalogo"].ToString();

                string fechaFinValida = ValidarTraslapeFechas(dtFechaFin, iCodCatalogoLinea, "Linea", "Empleado - Linea", iCodRegistroRel);

                if (fechaFinValida == "1")
                {
                    if (dtFechaFin >= dtFechaInicio)
                    {
                        DALCCustodia dalCCust = new DALCCustodia();
                        dalCCust.BajaLinea(iCodCatalogoLinea, iCodCatalogoEmple, dtFechaFin);

                        if (DSODataContext.Schema.ToUpper() == "FCA")
                        {
                            //No se da de baja la linea FCA para que concuerde con que no se da de baja la linea , solo se da de baja la relacion con el empleado
                            //dalCCust.BajaLineaFCA(iCodCatalogoLinea, dtFechaFin);
                        }

                    }
                    else //La Fecha de Fin no es valida
                    {
                        txtFechaFinLinea.Text = string.Empty;
                        txtFechaFinLinea.Focus();
                        throw new ArgumentException("La fecha fin debe ser mayor a la fecha de inicio, favor de seleccionar una fecha valida.");
                    }
                }
                else //La Fecha de Fin no es valida
                {
                    txtFechaFinLinea.Text = string.Empty;
                    txtFechaFinLinea.Focus();
                    throw new ArgumentException("La fecha fin que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                }
            }
            else
            {
                dtLinea.Clear();
                FillLineaGrid();
                throw new ArgumentException("La línea no existe o ya no encuentra activa.");
            }
        }

        private string ValidaCamposLineaAlta()
        {
            StringBuilder sbErrors = new StringBuilder(string.Empty);

            string linea = txtLineaNoPopUp.Text;            
            string sitio = drpSitioLineaNoPopUp.Text;
            string carrier = drpCarrierLineaNoPopUp.Text;
            string fechaIni = txtFechaInicioLineaNoPopUp.Text;

            if (linea == string.Empty || linea == "")
            {
                sbErrors.Append(@"*El campo (Línea) es requerido. \n");
            }
            else if (linea.Length >= 16)
            {
                sbErrors.Append(@"*La Longitud del campo (Línea) es muy grande. \n");
            }
            else
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(linea, @"^\d*$"))  //"^([0-9]|[*])*$"
                {
                    sbErrors.Append(@"*El campo (Línea) solo debe contener números. \n");
                }
            }

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

            if (!validaFormatoFecha(fechaIni))
            {
                sbErrors.Append(@"*El formato de (Fecha inicio) debe ser DD/MM/AAAA. Favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA. \n");
                txtFechaInicioLineaNoPopUp.Text = string.Empty;
            }


            if (DSODataContext.Schema.ToUpper() == "FCA")
            {

                if (ddlPentaSAPAccount.SelectedValue == "0")
                {
                    sbErrors.AppendLine(@"*El campo (PSAP Account) es requerido. ");
                }

                if (ddlPentaSAPCCDesc.SelectedValue == "0")
                {
                    sbErrors.AppendLine(@"*El campo (PentaSAP Cost Center Description) es requerido. ");
                }

                if (ddlPentaSAPCostCenter.SelectedValue == "0")
                {
                    sbErrors.AppendLine(@"*El campo (PentaSAP Cost Center) es requerido. ");
                }

                if (ddlPentaSAPFAFCA.SelectedValue == "0")
                {
                    sbErrors.AppendLine(@"*El campo (PentaSAP FA) es requerido. ");
                }

                if (ddlPentaSAPProfitCenter.SelectedValue == "0")
                {
                    sbErrors.AppendLine(@"*El campo (PentaSAP Profit Center) es requerido. ");
                }
            }

            return sbErrors.ToString();
        }
        private string ValidaCamposLineaAsignacion()
        {
            StringBuilder sbErrors = new StringBuilder(string.Empty);

            string linea = txtLineaNoPopUp.Text;
            string fechaIni = txtFechaInicioLineaNoPopUp.Text;
            if (linea == string.Empty || linea == "")
            {
                sbErrors.Append(@"*El campo (Línea) es requerido. \n");
            }
            else if (linea.Length >= 16)
            {
                sbErrors.Append(@"*La Longitud del campo (Línea) es muy grande. \n");
            }
            else
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(linea, @"^\d*$"))  //"^([0-9]|[*])*$"
                {
                    sbErrors.Append(@"*El campo (Línea) solo debe contener números. \n");
                }
            }
            if (fechaIni == string.Empty || fechaIni == "")
            {
                sbErrors.Append(@"*El campo (Fecha inicio) es requerido. \n");
            }

            if (!validaFormatoFecha(fechaIni))
            {
                sbErrors.Append(@"*El formato de (Fecha inicio) debe ser DD/MM/AAAA. Favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA. \n");
                txtFechaInicioLineaNoPopUp.Text = string.Empty;
            }

            return sbErrors.ToString();
        }
        private string ValidaCamposLineEditarBorrar()
        {
            StringBuilder sbErrors = new StringBuilder(string.Empty);

            string linea = txtLinea.Text;
            string sitio = drpLineaSitio.Text;
            string carrier = drpCarrierLinea.Text;
            string fechaIni = txtFechaInicioLinea.Text;
            string fechaFin = txtFechaFinLinea.Text;
            string fechaFinPlan = txtFinPlanLinea.Text;
            string fechaLimite = txtFechaLimiteLinea.Text;
            string fechaActivacion = txtFechaActivacionLinea.Text;
            string cargofijo = txtCargoFijo.Text;
            string fechaRelCencostoLinea = txtFechaRelacion.Text;
            

            if (linea == string.Empty || linea == "")
            {
                sbErrors.Append(@"*El campo (Línea) es requerido. \n");
            }
            else if (linea.Length >= 16)
            {
                sbErrors.Append(@"*La Longitud del campo (Línea) es muy grande. \n");
            }
            else
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(linea, @"^\d*$"))
                {
                    sbErrors.Append(@"*El campo (Línea) solo debe contener números. \n");
                }
            }
            if (string.IsNullOrEmpty(sitio))
            {
                sbErrors.Append(@"*El campo (Sitio - Linea) es requerido. \n");
            }
            if (string.IsNullOrEmpty(carrier))
            {
                sbErrors.Append(@"*El campo (Carrier) es requerido. \n");
            }
            if (fechaIni == string.Empty || fechaIni == "")
            {
                sbErrors.Append(@"*El campo (Fecha inicio) es requerido. \n");
            }
            if (!validaFormatoFecha(fechaIni))
            {
                sbErrors.Append(@"*El formato de (Fecha inicio) debe ser DD/MM/AAAA. Favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA. \n");
                txtFechaInicioLinea.Text = string.Empty;
            }
            //if (fechaRelCencostoLinea == string.Empty || fechaRelCencostoLinea == "")
            //{
            //    sbErrors.Append(@"*El campo (Fecha Relacion-CentroCosto) es requerido. \n");
            //}
            //if (!validaFormatoFecha(fechaRelCencostoLinea))
            //{
            //    sbErrors.Append(@"*El formato de (Fecha Relacion-CentroCosto) debe ser DD/MM/AAAA. Favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA. \n");
            //    txtFechaRelacion.Text = string.Empty;
            //}
            if (fechaFin == string.Empty || fechaFin == "")
            {
                sbErrors.Append(@"*El campo (Fecha Fin) es requerido. \n");
            }
            if (!validaFormatoFecha(fechaFin))
            {
                sbErrors.Append(@"*El formato de (Fecha Fin) debe ser DD/MM/AAAA. Favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA. \n");
                txtFechaFinLinea.Text = string.Empty;
            }
            if (!string.IsNullOrEmpty(txtFinPlanLinea.Text) && !validaFormatoFecha(txtFinPlanLinea.Text))
            {
                sbErrors.Append(@"*El formato de (Fecha Fin de Plan) debe ser DD/MM/AAAA. Favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA. \n");
                txtFinPlanLinea.Text = string.Empty;
            }
            if (!string.IsNullOrEmpty(txtFechaLimiteLinea.Text) && !validaFormatoFecha(txtFechaLimiteLinea.Text))
            {
                sbErrors.Append(@"*El formato de (Fecha Limite) debe ser DD/MM/AAAA. Favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA. \n");
                txtFechaLimiteLinea.Text = string.Empty;
            }
            if (!string.IsNullOrEmpty(txtFechaActivacionLinea.Text) && !validaFormatoFecha(txtFechaActivacionLinea.Text))
            {
                sbErrors.Append(@"*El formato de (Fecha Activación) debe ser DD/MM/AAAA. Favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA. \n");
                txtFechaActivacionLinea.Text = string.Empty;
            }

            decimal varConvert = 0;
            if (!string.IsNullOrEmpty(cargofijo) && !decimal.TryParse(cargofijo, out varConvert))
            {
                sbErrors.Append(@"*El campo (Cargo Fijo) no tiene el formato correcto. Favor de introducir solo valores númericos. \n");
            }

            return sbErrors.ToString();
        }

        private int CalcularValoresDeBanderaLinea()
        {
            FillValoresBanderasLineas();

            int bandIsTelular = (cbTelularLinea.Checked) ? Convert.ToInt32(dtValorBanderaLinea.AsEnumerable().First(x => x.Field<string>("vchCodigo") == "BanderaLineaBit1")["Value"]) : 0;
            int bandIsTarjeta = (cbTarjetaVPNetLinea.Checked) ? Convert.ToInt32(dtValorBanderaLinea.AsEnumerable().First(x => x.Field<string>("vchCodigo") == "BanderaLineaBit2")["Value"]) : 0;
            int bandIsNoPublic = (cbNoPublicableLinea.Checked) ? Convert.ToInt32(dtValorBanderaLinea.AsEnumerable().First(x => x.Field<string>("vchCodigo") == "BanderaLineaBit3")["Value"]) : 0;
            int bandIsConmutada = (cbConmutadaLinea.Checked) ? Convert.ToInt32(dtValorBanderaLinea.AsEnumerable().First(x => x.Field<string>("vchCodigo") == "BanderaLineaBit4")["Value"]) : 0;

            return bandIsTelular + bandIsTarjeta + bandIsNoPublic + bandIsConmutada;
        }

        public void LimpiaCamposNoPopUpLinea()
        {
            txtLineaNoPopUp.Text = null;
            drpSitioLineaNoPopUp.Text = null;
            drpCarrierLineaNoPopUp.Text = null;
            txtFechaInicioLineaNoPopUp.Text = null;
            ddlPentaSAPFAFCA.Text = null;
            ddlPentaSAPAccount.Text = null;
            ddlPentaSAPCCDesc.Text = null;
            ddlPentaSAPProfitCenter.Text = null;
            ddlPentaSAPCostCenter.Text = null;
        }

        public void CargaPropControlesAlEditarLinea()
        {
            //Controles de pop-up
            txtLinea.Enabled = false;
            txtLinea.ReadOnly = true;
            drpCarrierLinea.Enabled = false;
            drpSitioLinea.Enabled = false;
            txtFechaInicioLinea.Enabled = false;
            txtFechaInicioLinea.ReadOnly = false;
            lblFechaFinLinea.Visible = true;
            txtFechaFinLinea.Visible = true;
            txtFechaFinLinea.Enabled = false;
            txtFechaFinLinea.ReadOnly = false;

            drpCtaMaestraLinea.Enabled = false;
            /*SI EL CLIENTE ES BAT SE DESHABILITA LA RAZON SOCIAL*/
            if (DSODataContext.Schema.Trim().ToUpper() == "BAT")
            {
                drpRazonSocialLinea.Enabled = false;
            }
            else
            {
                drpRazonSocialLinea.Enabled = true;
            }

            drpTipoPlanLinea.Enabled = true;
            drpTipoEquipoLinea.Enabled = true;
            drpPlanTarifarioLinea.Enabled = true;
            txtFinPlanLinea.Enabled = false;
            txtFechaLimiteLinea.Enabled = false;
            txtFechaActivacionLinea.Enabled = false;
            txtEtiquetaLinea.Enabled = true;
            txtPlanLinea.Enabled = true;
            txtIMEILinea.Enabled = true;
            txtModeloEqLinea.Enabled = true;
            txtNumOrdenLinea.Enabled = true;
            txtCargoFijo.Enabled = true;
            cbTelularLinea.Enabled = true;
            cbTarjetaVPNetLinea.Enabled = true;
            cbNoPublicableLinea.Enabled = true;
            cbConmutadaLinea.Enabled = true;

            //Se cambian los titulos del pop-up de lineas y el texto del botón
            lblTituloPopUpLinea.Text = "Detalle de líneas";
            btnGuardarLinea.Text = "Guardar";
            btnGuardarLinea.Enabled = true;

            //Control para realizar proceso de edicion
            cbEditarLinea.Checked = true;
            cbBajaLinea.Checked = false;
        }

        public void CargaPropControlesAlBorrarLinea()
        {
            //Controles de pop-up
            txtLinea.Enabled = false;
            txtLinea.ReadOnly = true;
            drpCarrierLinea.Enabled = false;
            drpSitioLinea.Enabled = false;
            txtFechaInicioLinea.Enabled = false;
            txtFechaInicioLinea.ReadOnly = true;
            lblFechaFinLinea.Visible = true;
            txtFechaFinLinea.Visible = true;
            txtFechaFinLinea.Enabled = true;

            drpCtaMaestraLinea.Enabled = false;
            drpRazonSocialLinea.Enabled = false;
            drpTipoPlanLinea.Enabled = false;
            drpTipoEquipoLinea.Enabled = false;
            drpPlanTarifarioLinea.Enabled = false;
            txtFinPlanLinea.Enabled = false;
            txtFechaLimiteLinea.Enabled = false;
            txtFechaActivacionLinea.Enabled = false;
            txtEtiquetaLinea.Enabled = false;
            txtPlanLinea.Enabled = false;
            txtIMEILinea.Enabled = false;
            txtModeloEqLinea.Enabled = false;
            txtNumOrdenLinea.Enabled = false;
            txtCargoFijo.Enabled = false;
            cbTelularLinea.Enabled = false;
            cbTarjetaVPNetLinea.Enabled = false;
            cbNoPublicableLinea.Enabled = false;
            cbConmutadaLinea.Enabled = false;

            ddlPentaSAPCCDescriptionPopUp.Enabled = false;
            ddlPentaSAPAccountPopUp.Enabled = false;
            ddlPentaSAPCostCenterPopUp.Enabled = false;
            ddlPentaSAPProfitCenterPopUp.Enabled = false;
            ddlPentaSAPFAPopUp.Enabled = false;


            //Se cambia la leyenda del pop-up
            lblTituloPopUpLinea.Text = "¿Esta seguro que desea dar de baja la línea?";
            btnGuardarLinea.Text = "Eliminar";
            btnGuardarLinea.Enabled = true;

            //Control para realizar proceso de baja
            cbEditarLinea.Checked = false;
            cbBajaLinea.Checked = true;
        }

        //Editar fila línea
        protected void grvLinea_EditRow(object sender, ImageClickEventArgs e)
        {

            ImageButton ibtn2 = sender as ImageButton;
            int rowIndex = Convert.ToInt32(ibtn2.Attributes["RowIndex"]);
            idLinea.Value = grvLinea.DataKeys[rowIndex].Values[0].ToString();
            Index.Value = rowIndex.ToString();
            LlenaPopUpLineaEdit(rowIndex);

        }

        private void LlenaPopUpLineaEdit(int rowIndex)
        {
            //Use this rowIndex in your code 
            GridViewRow selectedRow = (GridViewRow)grvLinea.Rows[rowIndex];

            int iCodLinea = (int)grvLinea.DataKeys[rowIndex].Values[0];
            int iCodCarrier = (int)grvLinea.DataKeys[rowIndex].Values[1];
            string iCodSitio = (grvLinea.DataKeys[rowIndex].Values[2].ToString() != "{}") ? "" : grvLinea.DataKeys[rowIndex].Values[2].ToString();
            int iCodRegRelEmpLinea = (int)grvLinea.DataKeys[rowIndex].Values[3];
            //Se llenan los controles del pop-up
            txtLinea.Text = selectedRow.Cells[0].Text;
            drpCarrierLinea.Text = iCodCarrier.ToString();
            drpSitioLinea.Text = iCodSitio.ToString();
            txtFechaInicioLinea.Text = selectedRow.Cells[3].Text;
            txtFechaFinLinea.Text = selectedRow.Cells[4].Text;
            txtRegistroRelacionLinea.Text = iCodRegRelEmpLinea.ToString();

            FillDDLCtaMaestra(iCodCarrier.ToString());
            FillDDLPlanTarifario(iCodCarrier.ToString());
            FillDDlCategoriaAsignacion();
            FillDDLTipoAsignacion();
            //FillDDLModeloEquCelular();
            //FillDDLICCID();
            //FillEquipoCel();
            //FillDDSitioLine();
            //FillDDLAreaLinea();
            //Mapear datos de info adicional en el pop-up
            GetDatosLinea(iCodLinea.ToString());

            //RM 20190408 Carga elementos en el modal
            string PentaSAPCCDescription = grvLinea.DataKeys[rowIndex].Values[4].ToString();
            string PentaSAPAccount = grvLinea.DataKeys[rowIndex].Values[5].ToString();
            string PentaSAPProfitCenter = grvLinea.DataKeys[rowIndex].Values[6].ToString();
            string PentaSAPCostCenter = grvLinea.DataKeys[rowIndex].Values[7].ToString();
            string PentaSAPFA = grvLinea.DataKeys[rowIndex].Values[8].ToString();


            ddlPentaSAPCCDescriptionPopUp.SelectedValue = PentaSAPCCDescription;
            ddlPentaSAPAccountPopUp.SelectedValue = PentaSAPAccount;
            ddlPentaSAPProfitCenterPopUp.SelectedValue = PentaSAPProfitCenter;
            ddlPentaSAPCostCenterPopUp.SelectedValue = PentaSAPCostCenter;
            ddlPentaSAPFAPopUp.SelectedValue = PentaSAPFA;

            CargaPropControlesAlEditarLinea();
            mpeLinea.Show();
        }
        //Borrar fila línea
        protected void grvLinea_DeleteRow(object sender, ImageClickEventArgs e)
        {
            ImageButton ibtn2 = sender as ImageButton;
            int rowIndex = Convert.ToInt32(ibtn2.Attributes["RowIndex"]);

            //Use this rowIndex in your code 
            GridViewRow selectedRow = (GridViewRow)grvLinea.Rows[rowIndex];

            int iCodLinea = (int)grvLinea.DataKeys[rowIndex].Values[0];
            int iCodCarrier = (int)grvLinea.DataKeys[rowIndex].Values[1];
            string iCodSitio = (grvLinea.DataKeys[rowIndex].Values[2].ToString() != "{}") ? "" : grvLinea.DataKeys[rowIndex].Values[2].ToString();
            int iCodRegRelEmpLinea = (int)grvLinea.DataKeys[rowIndex].Values[3];

            //Se llenan los controles del pop-up
            txtLinea.Text = selectedRow.Cells[0].Text;
            drpCarrierLinea.Text = iCodCarrier.ToString();
            drpSitioLinea.Text = iCodSitio.ToString();
            txtFechaInicioLinea.Text = selectedRow.Cells[3].Text;
            txtFechaFinLinea.Text = string.Empty;
            txtRegistroRelacionLinea.Text = iCodRegRelEmpLinea.ToString();

            FillDDLCtaMaestra(iCodCarrier.ToString());
            FillDDLPlanTarifario(iCodCarrier.ToString());
            FillDDlCategoriaAsignacion();
            FillDDLTipoAsignacion();
            //FillDDLModeloEquCelular();
            //FillDDLICCID();
            //FillEquipoCel();
            //FillDDSitioLine();

            //Mapear datos de info adicional en el pop-up
            GetDatosLinea(iCodLinea.ToString());


            //RM 20190408 Carga elementos en el modal
            string PentaSAPCCDescription = grvLinea.DataKeys[rowIndex].Values[4].ToString();
            string PentaSAPAccount = grvLinea.DataKeys[rowIndex].Values[5].ToString();
            string PentaSAPProfitCenter = grvLinea.DataKeys[rowIndex].Values[6].ToString();
            string PentaSAPCostCenter = grvLinea.DataKeys[rowIndex].Values[7].ToString();
            string PentaSAPFA = grvLinea.DataKeys[rowIndex].Values[8].ToString();


            ddlPentaSAPCCDescriptionPopUp.SelectedValue = PentaSAPCCDescription;
            ddlPentaSAPAccountPopUp.SelectedValue = PentaSAPAccount;
            ddlPentaSAPProfitCenterPopUp.SelectedValue = PentaSAPProfitCenter;
            ddlPentaSAPCostCenterPopUp.SelectedValue = PentaSAPCostCenter;
            ddlPentaSAPFAPopUp.SelectedValue = PentaSAPFA;

            CargaPropControlesAlBorrarLinea();
            mpeLinea.Show();
        }

        private void GetDatosLinea(string iCodCatalogoLinea)
        {
            lsbQuery.Length = 0;

            string sp = "EXEC dbo.ObtieneDatosLineas @Schema = '" + DSODataContext.Schema + "', @IcodLinea = " + iCodCatalogoLinea + "";
            DataRow drLinea = DSODataAccess.ExecuteDataRow(sp);

            if (drLinea != null)
            {


                drpCtaMaestraLinea.Text = drLinea["CtaMaestra"].ToString();
                drpRazonSocialLinea.Text = drLinea["RazonSocial"].ToString();
                Session["RazonSocialLinea"] = (drLinea["RazonSocial"].ToString()!="")? drLinea["RazonSocial"].ToString() :null;

                drpTipoPlanLinea.Text = drLinea["TipoPlan"].ToString();
                Session["TipoPlanLinea"] = (drLinea["TipoPlan"].ToString()!="")? drLinea["TipoPlan"].ToString() :null;

                drpPlanTarifarioLinea.Text = drLinea["PlanTarif"].ToString();
                Session["PlanTarifarioLinea"] = (drLinea["PlanTarif"].ToString()!="")? drLinea["PlanTarif"].ToString() :null;

                drpCategoriaAsignacion.Text = drLinea["CategoriaAsig"].ToString();
                Session["CategoriaAsignacion"] = (drLinea["CategoriaAsig"].ToString()!="")? drLinea["CategoriaAsig"].ToString() :null;

                drpTipoAsignacion.Text = drLinea["TipoAsig"].ToString();
                Session["TipoAsignacion"] = (drLinea["TipoAsig"].ToString()!="")? drLinea["TipoAsig"].ToString() :null;

                txtFinPlanLinea.Text = (drLinea["FechaFinPlan"] != DBNull.Value) ? Convert.ToDateTime(drLinea["FechaFinPlan"]).ToString("dd/MM/yyyy") : string.Empty;
                txtFechaLimiteLinea.Text = (drLinea["FecLimite"] != DBNull.Value) ? Convert.ToDateTime(drLinea["FecLimite"]).ToString("dd/MM/yyyy") : string.Empty;
                txtFechaActivacionLinea.Text = (drLinea["FechaDeActivacion"] != DBNull.Value) ? Convert.ToDateTime(drLinea["FechaDeActivacion"]).ToString("dd/MM/yyyy") : string.Empty;
                txtEtiquetaLinea.Text = drLinea["Etiqueta"].ToString();
                txtPlanLinea.Text = drLinea["PlanLineaFactura"].ToString();
                txtGBPlan.Text = drLinea["GBPLAN"].ToString();
                txtNumOrdenLinea.Text = drLinea["NumOrden"].ToString();
                txtCargoFijo.Text = drLinea["CargoFijo"].ToString();
                txtMontoRenta.Text = drLinea["Renta"].ToString();
                txtRegion.Text = drLinea["RegionBat"].ToString();
                txtComentarios.Text = drLinea["Comentarios"].ToString().ToUpper().Trim();

                /*OBTINE CENTRO DE COSTOS DE ACUERDO A LA RAZON SOCIAL ESTO SOLO  APLICA PARA BAT*/
                int razonSocial = (drLinea["RazonSocial"].ToString() != "") ? Convert.ToInt32(drLinea["RazonSocial"]) : 0;
                hdfRazonId.Value = razonSocial.ToString();
                razonSocialId = razonSocial;

                int valorTotalBandera = (drLinea["BanderasLinea"] != DBNull.Value) ? Convert.ToInt32(drLinea["BanderasLinea"].ToString()) : 0;

                FillValoresBanderasLineas();

                cbTelularLinea.Checked = (VerificarBandera(valorTotalBandera,
                                             Convert.ToInt32(dtValorBanderaLinea.AsEnumerable().First(x => x.Field<string>("vchCodigo") == "BanderaLineaBit1")["Value"]))) ? true : false;
                cbTarjetaVPNetLinea.Checked = (VerificarBandera(valorTotalBandera,
                                             Convert.ToInt32(dtValorBanderaLinea.AsEnumerable().First(x => x.Field<string>("vchCodigo") == "BanderaLineaBit2")["Value"]))) ? true : false;
                cbNoPublicableLinea.Checked = (VerificarBandera(valorTotalBandera,
                                             Convert.ToInt32(dtValorBanderaLinea.AsEnumerable().First(x => x.Field<string>("vchCodigo") == "BanderaLineaBit3")["Value"]))) ? true : false;
                cbConmutadaLinea.Checked = (VerificarBandera(valorTotalBandera,
                                             Convert.ToInt32(dtValorBanderaLinea.AsEnumerable().First(x => x.Field<string>("vchCodigo") == "BanderaLineaBit4")["Value"]))) ? true : false;

            }

            ObtieneEqCelular(iCodCatalogoLinea);

            ObtineICC(iCodCatalogoLinea);
            ObtieneSitioLinea(iCodCatalogoLinea);
            ObtieneAreaLinea(iCodCatalogoLinea);


            //FillDDCenCosto(razonSocial);

            RelLineaCencos(Convert.ToInt32(iCodCatalogoLinea));

            //txtIMEILinea.Text = drLinea["IMEI"].ToString();
            //txtModeloEqLinea.Text = drLinea["ModeloCel"].ToString();
            //drpTipoEquipoLinea.Text = drLinea["EqCelular"].ToString();
        }
        private void ObtieneAreaLinea(string iCodCatalogoLinea)
        {
            StringBuilder Query = new StringBuilder();
            Query.AppendLine(" SELECT");
            Query.AppendLine(" ISNULL(iCodCatalogo, 0) AS iCodCatalogo,");
            Query.AppendLine(" ISNULL(Descripcion, '') AS Descripcion");
            Query.AppendLine(" FROM " + DSODataContext.Schema + ".[vishistoricos('Area','Areas','Español')] AS A WITH(NOLOCK)");
            Query.AppendLine(" JOIN " + DSODataContext.Schema + ".[visRelaciones('Linea-Area','Español')] AS LA WITH(NOLOCK)");
            Query.AppendLine(" ON A.iCodCatalogo = LA.Area");
            Query.AppendLine(" AND LA.dtIniVigencia<> LA.dtFinVigencia");
            Query.AppendLine(" AND LA.dtFinVigencia >= GETDATE()");
            Query.AppendLine(" WHERE A.dtIniVigencia<> A.dtFinVigencia");
            Query.AppendLine(" AND A.dtFinVigencia >= GETDATE()");
            Query.AppendLine(" AND LA.Linea = " + iCodCatalogoLinea + "");
            DataRow drArea = DSODataAccess.ExecuteDataRow(Query.ToString());
            if (drArea != null)
            {
                hdfAreaLinea.Value = drArea["iCodCatalogo"].ToString();
                txtAreaLinea.Text= drArea["Descripcion"].ToString().ToUpper().Trim();
                //drpAreaLinea.Text = drArea["iCodCatalogo"].ToString().ToUpper().Trim();
            }
        }
        private void ObtieneEqCelular(string iCodCatalogoLinea)
        {
            StringBuilder Query = new StringBuilder();
            Query.AppendLine(" SELECT");
            Query.AppendLine(" ISNULL(EQ.IMEI, '') AS IMEI, EC.iCodCatalogo AS EqCelular,MO.iCodCatalogo AS IcodModelo, ");
            Query.AppendLine(" ISNULL(EC.Descripcion, '') AS DescripcionEquipo,");
            Query.AppendLine(" ISNULL(MO.Descripcion, '') AS DescripcionModelo");
            Query.AppendLine(" FROM " + DSODataContext.Schema + ".[VisRelaciones('Linea-DispositivoEqCelular','Español')]AS RE WITH(NOLOCK)");
            Query.AppendLine(" JOIN " + DSODataContext.Schema + ".[vishistoricos('DispositivoEqCelular','Dispositivos Equipo Celular','español')] AS EQ WITH(NOLOCK)");
            Query.AppendLine(" ON RE.DispoSitivoEqCelular = EQ.iCodCatalogo");
            Query.AppendLine(" AND EQ.dtIniVigencia <> EQ.dtFinVigencia");
            Query.AppendLine(" AND EQ.dtFinVigencia >= GETDATE()");
            Query.AppendLine(" JOIN " + DSODataContext.Schema + ".[VisHistoricos('ModeloEqCelular','Modelos equipo celular','Español')] AS MO WITH(NOLOCK)");
            Query.AppendLine(" ON EQ.ModeloEqCelular = MO.iCodCatalogo");
            Query.AppendLine(" AND MO.dtIniVigencia <> MO.dtFinVigencia");
            Query.AppendLine(" AND MO.dtFinVigencia >= GETDATE()");
            Query.AppendLine(" JOIN " + DSODataContext.Schema + ".[VisHistoricos('EqCelular','Equipo Celular','Español')] AS EC WITH(NOLOCK)");
            Query.AppendLine(" ON MO.EqCelular = EC.icodCatalogo");
            Query.AppendLine(" AND EC.dtIniVigencia <> EC.dtFinVigencia");
            Query.AppendLine(" AND EC.dtFinVigencia >= GETDATE()");
            Query.AppendLine(" WHERE RE.dtIniVigencia <> RE.dtFinVigencia");
            Query.AppendLine(" AND RE.dtFinVigencia >= GETDATE()");
            Query.AppendLine(" AND RE.Linea = " + iCodCatalogoLinea + "");
            DataRow drLinea = DSODataAccess.ExecuteDataRow(Query.ToString());
            if (drLinea != null)
            {
                drpTipoEquipoLinea.Text = drLinea["EqCelular"].ToString();
                txtIMEILinea.Text = drLinea["IMEI"].ToString();
                Session["IMEILINEA"] = drLinea["IMEI"].ToString();
                drpModeloEquipo.Text = drLinea["IcodModelo"].ToString();
                Session["TipoEquipoLinea"] = drLinea["EqCelular"].ToString();
            }

        }
        private void ObtineICC(string iCodCatalogoLinea)
        {
            StringBuilder Query = new StringBuilder();
            Query.AppendLine(" SELECT");
            Query.AppendLine(" ISNULL(RLIC.Descripcion,'') AS Descripcion");
            Query.AppendLine(" FROM " + DSODataContext.Schema + ".[VisRelaciones('Linea-CCID','Español')] AS LI WITH(NOLOCK)");
            Query.AppendLine(" JOIN " + DSODataContext.Schema + ".[VisHistoricos('ICCID','Integrated Circuit Card IDs','Español')]AS RLIC WITH(NOLOCK)");
            Query.AppendLine(" ON LI.ICCID = RLIC.iCodCatalogo");
            Query.AppendLine(" AND RLIC.dtIniVigencia<> RLIC.dtFinVigencia");
            Query.AppendLine(" AND RLIC.dtFinVigencia >= GETDATE()");
            Query.AppendLine(" WHERE LI.dtIniVigencia<> LI.dtFinVigencia");
            Query.AppendLine(" AND LI.dtFinVigencia >= GETDATE()");
            Query.AppendLine(" AND LI.Linea = " + iCodCatalogoLinea + "");
            DataRow drLineaICC = DSODataAccess.ExecuteDataRow(Query.ToString());
            if (drLineaICC != null)
            {
                txtICCID.Text = drLineaICC["Descripcion"].ToString();
            }

        }
        private void ObtieneSitioLinea(string iCodCatalogoLinea)
        {
            StringBuilder Query = new StringBuilder();
            Query.AppendLine(" SELECT");
            Query.AppendLine("  UL.iCodCatalogo,ISNULL(UL.NombreUbicacion,'') AS NombreUbicacion");
            Query.AppendLine(" FROM " + DSODataContext.Schema + ".[VisRelaciones('Linea-UbicacionLinea','Español')] AS RU WITH(NOLOCK)");
            Query.AppendLine(" JOIN " + DSODataContext.Schema + ".[vishistoricos('UbicacionLinea','Ubicación Líneas','español')] AS UL WITH(NOLOCK) ");
            Query.AppendLine(" ON RU.UbicacionLinea = UL.iCodCatalogo");
            Query.AppendLine(" AND UL.dtIniVigencia<> UL.dtFinVigencia");
            Query.AppendLine(" AND UL.dtFinVigencia >= GETDATE()");
            Query.AppendLine(" WHERE RU.dtIniVigencia<> RU.dtFinVigencia");
            Query.AppendLine(" AND RU.dtFinVigencia >= GETDATE()");
            Query.AppendLine(" AND RU.Linea = " + iCodCatalogoLinea + "");
            DataRow drLineaSitio = DSODataAccess.ExecuteDataRow(Query.ToString());
            if (drLineaSitio != null)
            {
                drpLineaSitio.Text = drLineaSitio["iCodCatalogo"].ToString();
                Session["LineaSitio"] = drLineaSitio["iCodCatalogo"].ToString();
            }

        }
        public bool VerificarBandera(int numValorTotal, int valorBandera)
        {
            //Hace una suma a nivel de bits de acuerdo al operador AND.
            return ((numValorTotal & valorBandera) == valorBandera) ? true : false;
        }
        #endregion
        private void FillDDLCosFiltro(DropDownList dropDown, string iCodSitio)
        {
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("SELECT iCodCatalogo = NULL, vchDescripcion = '-- Selecciona uno --'");
            lsbQuery.AppendLine("UNION");
            lsbQuery.AppendLine("SELECT iCodCatalogo, vchDescripcion = vchDescripcion + ' (' + ISNULL(vchCodigo,'') + ')'");
            lsbQuery.AppendLine("FROM [VisHistoricos('Cos','Cos','Español')]");
            lsbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            lsbQuery.AppendLine("   AND dtFinVigencia >= GETDATE()");
            //lsbQuery.AppendLine("	AND MarcaSitio IN (");
            //lsbQuery.AppendLine("						SELECT MarcaSitio");
            //lsbQuery.AppendLine("						FROM [VisHisComun('Sitio','Español')]");
            //lsbQuery.AppendLine("						WHERE dtIniVigencia <> dtFinVigencia");
            //lsbQuery.AppendLine("							AND dtFinVigencia >= GETDATE()");
            //lsbQuery.AppendLine("							AND iCodCatalogo = " + iCodSitio);
            //lsbQuery.AppendLine("						)");
            lsbQuery.AppendLine("ORDER BY vchDescripcion");

            dtCos = DSODataAccess.Execute(lsbQuery.ToString());

            dropDown.DataSource = null;
            dropDown.DataSource = dtCos;
            dropDown.DataBind();
        }

        protected void drpSitioCodAutoNoPopUp_Changed(Object sender, EventArgs e)
        {
            DropDownList dllSitio = sender as DropDownList;
            DropDownList dllCos = null;
            if (dllSitio.ID == "drpSitioCodAutoNoPopUp")
            {
                dllCos = drpCosCodAutoNoPopUp;
            }
            else if (dllSitio.ID == "drpSitioCodAuto")
            {
                dllCos = drpCosCodAuto;
            }
            else if (dllSitio.ID == "drpSitio") //Extensiones
            {
                dllCos = drpCosExten;
            }
            else if (dllSitio.ID == "drpSitioNoPopUp") //Extensiones
            {
                dllCos = drpCosExtenNoPopUp;
            }

            FillDDLCosFiltro(dllCos, dllSitio.Text);
        }

        private string ValidarTraslapeFechas(DateTime fechaAValidar, string iCodRecurso, string nombreCampoRecurso, string nombreVistaRel, string iCodRegistroRel)
        {
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

        private string NoSQLCode(string texto)
        {
            return texto.Replace("'", "").ToUpper().Replace(";", "")
                      .Replace("DROP", "").Replace("DELETE", "").Replace("INSERT", "").Replace("TRUNCATE", "").Replace("UPDATE", "")
                      .Replace("SELECT", "").Replace("FROM", "").Trim();
        }

        protected void enableLinkButton(LinkButton lbtnObject1, LinkButton lbntObject2)
        {
            if (lbtnObject1.Enabled && lbtnObject1.Visible)
            {
                lbtnObject1.Enabled = false;
                lbtnObject1.Visible = false;

                lbntObject2.Enabled = true;
                lbntObject2.Visible = true;
            }
            else
            {
                lbtnObject1.Enabled = true;
                lbtnObject1.Visible = true;

                lbntObject2.Enabled = false;
                lbntObject2.Visible = false;
            }
        }

        protected void enableDDL(DropDownList ddlObject)
        {
            if (ddlObject.Enabled)
            {
                ddlObject.Enabled = false;
            }
            else
            {
                ddlObject.Enabled = true;
            }
        }

        protected void enableCheckBox(CheckBox cbObject)
        {
            if (cbObject.Enabled)
            {
                cbObject.Enabled = false;
            }
            else
            {
                cbObject.Enabled = true;
            }
        }

        /* Deja el TextBox con las propiedades invertidas. */
        protected void enableTxtBox(TextBox txtObject)
        {
            if (txtObject.Enabled && !txtObject.ReadOnly)
            {
                txtObject.Enabled = false;
                txtObject.ReadOnly = true;
            }
            else
            {
                txtObject.Enabled = true;
                txtObject.ReadOnly = false;
            }
        }

        protected void imgbPDFExport_Click(object sender, ImageClickEventArgs e)
        {
            ExportPDF();
        }

        /* **PT** Metodos para exportar a pdf */
        #region exportacion a pdf
        public void ExportPDF()
        {
            CrearDOC(".pdf");
        }
        protected void ExportarArchivo(string lsExt)
        {
            string lsTitulo = HttpUtility.UrlEncode(Globals.GetMsgWeb(false, "TituloCartaCustodia"));
            Page.Response.Redirect("../DSOFileLinkHandler.ashx?key=" + psFileKey + "&fn=" + lsTitulo + lsExt);
        }
        protected string GetFileName(string lsExt)
        {
            string lsFileName = System.IO.Path.Combine(psTempPath, "cc." + psFileKey + ".temp" + lsExt);
            Session[psFileKey] = lsFileName;
            return lsFileName;
        }
        protected void CrearDOC(string lsExt)
        {
            WordAccess lWord = new WordAccess();
            try
            {

                string lsStylePath = HttpContext.Current.Server.MapPath(Session["StyleSheet"].ToString());

                lWord.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\CartasCustodia\PlantillaCartaCustodiaDTI.docx");

                lWord.Abrir();
                lWord.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                #region inserta logos
                string lsKeytiaWebFPath = HttpContext.Current.Server.MapPath("~");
                string lsImg;
                DataRow pRowCliente = DSODataAccess.ExecuteDataRow("SELECT LogoExportacion FROM [VisHistoricos('Client','Clientes','Español')] " +
                                                    " WHERE UsuarDB = " + DSODataContext.GetContext() +
                                                    "   AND dtIniVigencia <> dtFinVigencia " +
                                                    "   AND dtFinVigencia > GETDATE()");

                //NZ 20150508 Se cambio el nombre de la columna a la cual ira a buscar el logo del cliente para exportacion. pRowCliente["Logo"] POR pRowCliente["LogoExportacion"]
                //lsImg = System.IO.Path.Combine(lsKeytiaWebFPath, pRowCliente["Logo"].ToString().Replace("~/", ""));
                lsImg = System.IO.Path.Combine(lsKeytiaWebFPath, pRowCliente["LogoExportacion"].ToString().Replace("~/", ""));
                if (System.IO.File.Exists(lsImg))
                {
                    //lWord.ReemplazarTextoPorImagen("{LogoCliente}", lsImg);
                    lWord.PosicionaCursor("{LogoCliente}");
                    lWord.ReemplazarTexto("{LogoCliente}", "");
                    lWord.InsertarImagen(lsImg);//, 131, 40);
                }
                else
                {
                    lWord.ReemplazarTexto("{LogoCliente}", "");
                }

                #endregion

                //Obtener datos en datatable
                #region creaDatatables

                //20170620 NZ Se quita esta sección (Inventarios)
                //DataTable dtInventario = GetDataTable(grvInventario);
                //if (dtInventario.Rows.Count > 0)
                //{
                //    if (dtInventario.Columns.Contains("&nbsp;"))
                //        dtInventario.Columns.Remove("&nbsp;");
                //    if (dtInventario.Columns.Contains("&nbsp;6"))
                //        dtInventario.Columns.Remove("&nbsp;6");
                //    if (dtInventario.Columns.Contains("fecha inicial"))
                //        dtInventario.Columns.Remove("fecha inicial");
                //    if (dtInventario.Columns.Contains("fecha final"))
                //        dtInventario.Columns.Remove("fecha final");
                //    if (dtInventario.Columns.Contains("fecha fin"))
                //        dtInventario.Columns.Remove("fecha fin");
                //    //RZ.20131227 Se retira parte de edicion de inventario
                //    //dtInventario.Columns.Remove("Editar");
                //    dtInventario.Columns.Remove("Borrar");
                //}
                DataTable dtExtensiones = GetDataTable(grvExten);
                if (dtExtensiones.Rows.Count > 0)
                {
                    if (dtExtensiones.Columns.Contains("&nbsp;"))
                        dtExtensiones.Columns.Remove("&nbsp;");
                    if (dtExtensiones.Columns.Contains("&nbsp;1"))
                        dtExtensiones.Columns.Remove("&nbsp;1");
                    if (dtExtensiones.Columns.Contains("&nbsp;2"))
                        dtExtensiones.Columns.Remove("&nbsp;2");
                    if (dtExtensiones.Columns.Contains("&nbsp;3"))
                        dtExtensiones.Columns.Remove("&nbsp;3");
                    if (dtExtensiones.Columns.Contains("&nbsp;10"))
                        dtExtensiones.Columns.Remove("&nbsp;10");
                    if (dtExtensiones.Columns.Contains("&nbsp;12"))
                        dtExtensiones.Columns.Remove("&nbsp;12");
                    if (dtExtensiones.Columns.Contains("fecha inicial"))
                        dtExtensiones.Columns.Remove("fecha inicial");
                    if (dtExtensiones.Columns.Contains("fecha final"))
                        dtExtensiones.Columns.Remove("fecha final");
                    if (dtExtensiones.Columns.Contains("fecha fin"))
                        dtExtensiones.Columns.Remove("fecha fin");
                    dtExtensiones.Columns.Remove("Visible en Directorio");
                    dtExtensiones.Columns.Remove("Editar");
                    dtExtensiones.Columns.Remove("Borrar");
                }
                DataTable dtCodigos = GetDataTable(grvCodAuto);
                if (dtCodigos.Rows.Count > 0)
                {
                    if (dtCodigos.Columns.Contains("&nbsp;"))
                        dtCodigos.Columns.Remove("&nbsp;");
                    if (dtCodigos.Columns.Contains("&nbsp;5"))
                        dtCodigos.Columns.Remove("&nbsp;5");
                    if (dtCodigos.Columns.Contains("&nbsp;6"))
                        dtCodigos.Columns.Remove("&nbsp;6");
                    if (dtCodigos.Columns.Contains("&nbsp;7"))
                        dtCodigos.Columns.Remove("&nbsp;7");
                    if (dtCodigos.Columns.Contains("&nbsp;8"))
                        dtCodigos.Columns.Remove("&nbsp;8");
                    if (dtCodigos.Columns.Contains("fecha inicial"))
                        dtCodigos.Columns.Remove("fecha inicial");
                    if (dtCodigos.Columns.Contains("fecha final"))
                        dtCodigos.Columns.Remove("fecha final");
                    if (dtCodigos.Columns.Contains("fecha fin"))
                        dtCodigos.Columns.Remove("fecha fin");
                    //RZ.20131227 Se retira campo "Visible en Directorio"
                    //dtCodigos.Columns.Remove("Visible en Directorio");
                    dtCodigos.Columns.Remove("Editar");
                    dtCodigos.Columns.Remove("Borrar");
                }
                DataTable dtLineas = GetDataTable(grvLinea);
                if (dtLineas.Rows.Count > 0)
                {
                    if (dtLineas.Columns.Contains("L&#237;nea"))
                        dtLineas.Columns["L&#237;nea"].ColumnName = "Línea";
                    if (dtLineas.Columns.Contains("&nbsp;"))
                        dtLineas.Columns.Remove("&nbsp;");
                    if (dtLineas.Columns.Contains("&nbsp;6"))
                        dtLineas.Columns.Remove("&nbsp;6");
                    if (dtLineas.Columns.Contains("&nbsp;7"))
                        dtLineas.Columns.Remove("&nbsp;7");
                    if (dtLineas.Columns.Contains("&nbsp;8"))
                        dtLineas.Columns.Remove("&nbsp;8");
                    if (dtLineas.Columns.Contains("fecha inicial"))
                        dtLineas.Columns.Remove("fecha inicial");
                    if (dtLineas.Columns.Contains("fecha final"))
                        dtLineas.Columns.Remove("fecha final");
                    if (dtLineas.Columns.Contains("fecha fin"))
                        dtLineas.Columns.Remove("fecha fin");
                    dtLineas.Columns.Remove("Editar");
                    dtLineas.Columns.Remove("Borrar");
                }
                #endregion


                #region datos emple

                lWord.ReemplazarTexto("{Fecha}", txtFecha.Text);
                lWord.ReemplazarTexto("{Folio}", txtFolioCCustodia.Text);
                //lWord.ReemplazarTexto("{Estatus}", "");
                lWord.ReemplazarTexto("{Nomina}", txtNominaEmple.Text);
                lWord.ReemplazarTexto("{Nombre}", txtNombreEmple.Text);
                lWord.ReemplazarTexto("{SegNombre}", txtSegundoNombreEmple.Text);
                lWord.ReemplazarTexto("{ApPaterno}", txtApPaternoEmple.Text);
                lWord.ReemplazarTexto("{ApMaterno}", txtApMaternoEmple.Text);
                lWord.ReemplazarTexto("{Ubicacion}", (drpSitioEmple.SelectedItem.Text.Contains("-- Selecc")) ? string.Empty : drpSitioEmple.SelectedItem.Text);
                lWord.ReemplazarTexto("{Empresa}", (drpEmpresaEmple.SelectedItem.Text.Contains("-- Selecc")) ? string.Empty : drpEmpresaEmple.SelectedItem.Text);
                lWord.ReemplazarTexto("{TipoEmple}", (drpTipoEmpleado.SelectedItem.Text.Contains("-- Selecc")) ? string.Empty : drpTipoEmpleado.SelectedItem.Text);
                lWord.ReemplazarTexto("{Cencos}", (drpCenCosEmple.SelectedItem.Text.Contains("-- Selecc")) ? string.Empty : drpCenCosEmple.SelectedItem.Text);
                lWord.ReemplazarTexto("{Puesto}", (drpPuestoEmple.SelectedItem.Text.Contains("-- Selecc")) ? string.Empty : drpPuestoEmple.SelectedItem.Text);
                lWord.ReemplazarTexto("{Localidad}", (drpLocalidadEmple.SelectedItem.Text.Contains("-- Selecc")) ? string.Empty : drpLocalidadEmple.SelectedItem.Text);
                lWord.ReemplazarTexto("{Email}", txtEmailEmple.Text);
                lWord.ReemplazarTexto("{usuario}", txtUsuarRedEmple.Text);
                //lWord.ReemplazarTexto("{Jefe}", (drpJefeEmple.SelectedItem.Text.Contains("-- Selecc")) ? string.Empty : drpJefeEmple.SelectedItem.Text);
                lWord.ReemplazarTexto("{Jefe}", txtJefeEmple.Text);
                //if (DSODataContext.Schema.ToUpper() == "FCA")
                //{
                //    lWord.ReemplazarTexto("{EmailJefe}", txtEmailJefeEmple.Text);
                //    lWord.ReemplazarTexto("{Dc_id}", txtDatosEmpleFCADC_ID.Text);
                //    lWord.ReemplazarTexto("{T_id}", txtDatosEmpleFCAT_ID.Text);
                //    lWord.ReemplazarTexto("{NickName}", txtDatosEmpleFCANickName.Text);
                //    lWord.ReemplazarTexto("{Estación}", txtDatosEmpleFCAEstacion.Text);
                //    lWord.ReemplazarTexto("{Director}", (ddlDatosEmpleFCADirector.SelectedItem.Text.Contains("-- Selecc")) ? string.Empty : ddlDatosEmpleFCADirector.SelectedItem.Text);
                //}



                #endregion


                //20170620 NZ Se quita esta sección
                //lWord.PosicionaCursor("{InventarioEquipos}");
                //lWord.ReemplazarTexto("{InventarioEquipos}", "");
                //if (dtInventario.Rows.Count > 0)
                //    lWord.InsertarTabla(dtInventario, true);
                //else
                //    lWord.InsertarTexto("El empleado no cuenta con inventario asignado");


                lWord.PosicionaCursor("{Extensiones}");
                lWord.ReemplazarTexto("{Extensiones}", "");
                if (dtExtensiones.Rows.Count > 0)
                    lWord.InsertarTabla(dtExtensiones, true);

                else
                    lWord.InsertarTexto("El empleado no cuenta con extensiones asignadas");


                lWord.PosicionaCursor("{Codigos}");
                lWord.ReemplazarTexto("{Codigos}", "");
                if (dtCodigos.Rows.Count > 0)
                    lWord.InsertarTabla(dtCodigos, true);
                else
                    lWord.InsertarTexto("El empleado no cuenta con códigos asignados");


                lWord.PosicionaCursor("{Lineas}");
                lWord.ReemplazarTexto("{Lineas}", "");
                if (dtLineas.Rows.Count > 0)
                    lWord.InsertarTabla(dtLineas, true);
                else
                    lWord.InsertarTexto("El empleado no cuenta con lineas asignadas");


                lWord.PosicionaCursor("{ComAdmin}");
                if (!string.IsNullOrEmpty(txtComentariosAdmin.Text))
                {
                    lWord.ReemplazarTexto("{ComAdmin}", txtComentariosAdmin.Text);
                }
                else
                {
                    lWord.ReemplazarTexto("{ComAdmin}", "");
                }

                lWord.PosicionaCursor("{ComUsuario}");
                if (!string.IsNullOrEmpty(txtComenariosEmple.Text))
                {
                    lWord.ReemplazarTexto("{ComUsuario}", txtComenariosEmple.Text);
                }
                else
                {
                    lWord.ReemplazarTexto("{ComUsuario}", "");
                }
                //RZ.20131107 Se retira autofit, ya que no es necesario PT hizo cambios en la plantilla
                //lWord.Tabla.Columns.AutoFit();
                lWord.FilePath = GetFileName(lsExt);
                lWord.SalvarComo();


                ExportarArchivo(lsExt);
            }
            catch (System.Threading.ThreadAbortException tae) { } //Page.Response.Redirect puede arrojar esta excepcion
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrExportTo", e, lsExt);
            }
            finally
            {
                if (lWord != null)
                {
                    lWord.Cerrar(true);
                    lWord.Dispose();

                }
            }
        }
        private DataTable GetDataTable(GridView dtg)
        {
            DataTable dt = new DataTable();

            // add the columns to the datatable            
            if (dtg.HeaderRow != null)
            {

                for (int i = 0; i < dtg.HeaderRow.Cells.Count; i++)
                {
                    if (dt.Columns.Contains(dtg.HeaderRow.Cells[i].Text))
                    {
                        dt.Columns.Add(dtg.HeaderRow.Cells[i].Text + i);
                    }
                    else
                    {
                        dt.Columns.Add(dtg.HeaderRow.Cells[i].Text.Replace("&#243;", "ó"));
                    }
                }
            }

            //  add each of the data rows to the table
            foreach (GridViewRow row in dtg.Rows)
            {
                DataRow dr;
                dr = dt.NewRow();

                for (int i = 0; i < row.Cells.Count; i++)
                {
                    dr[i] = row.Cells[i].Text.Replace("&#211;", "Ó").Replace("&#243;", "ó").Replace("&nbsp;", "");
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        #endregion

        #region Controls postback
        public static Control GetPostBackControl(Page page)
        {
            Control postbackControlInstance = null;

            string postbackControlName = page.Request.Params.Get("__EVENTTARGET");
            if (postbackControlName != null && postbackControlName != string.Empty)
            {
                postbackControlInstance = page.FindControl(postbackControlName);
            }
            else
            {
                // handle the Button control postbacks
                for (int i = 0; i < page.Request.Form.Keys.Count; i++)
                {
                    postbackControlInstance = page.FindControl(page.Request.Form.Keys[i]);
                    if (postbackControlInstance is System.Web.UI.WebControls.Button)
                    {
                        return postbackControlInstance;
                    }
                }
            }
            // handle the ImageButton postbacks
            if (postbackControlInstance == null)
            {
                for (int i = 0; i < page.Request.Form.Count; i++)
                {
                    if ((page.Request.Form.Keys[i].EndsWith(".x")) || (page.Request.Form.Keys[i].EndsWith(".y")))
                    {
                        postbackControlInstance = page.FindControl(page.Request.Form.Keys[i].Substring(0, page.Request.Form.Keys[i].Length - 2));
                        return postbackControlInstance;
                    }
                }
            }
            return postbackControlInstance;
        }
        #endregion

        //AM 20130717 . Agrego metodo para mandar los mensajes de advertencia en javascript
        /// <summary>
        /// Muestra un mensaje de advertencia en la pagina web.
        /// </summary>
        /// <param name="mensaje"><li>Mensaje que se desea desplegar</li></param>
        protected void MensajeDeAdvertencia(string mensaje)
        {
            string script = @"<script type='text/javascript'>TextBoxModalGenerico('" + mensaje + "');</script>";
            ScriptManager.RegisterStartupScript(this, typeof(Page), "alerta", script, false);

            btnGuardarExten.Enabled = true;
            btnGuardarCodAuto.Enabled = true;
            btnGuardarLinea.Enabled = true;
            lbtnGuardarCodAutoNoPopUp.Enabled = true;
            lbtnGuardarExtenNoPopUp.Enabled = true;
            lbtnGuardarLineaNoPopUp.Enabled = true;

            lblBodyMensajeGenerico.Text = mensaje;
            //pnlPopupMensaje.Style.Remove("style");
            //pnlPopupMensaje.Style.Add("style", "z-index:15001;");
            mpeEtqMsn.Show();
        }

        private void MensajeProcesoExitoso(string mensaje = "")
        {
            MensajeDeAdvertencia("Se completó el proceso correctamente. " + mensaje);
        }

        //AM 20130717 . Agrego metodo para Validar que el formato de fecha sea  DD/MM/AAAA
        /// <summary>
        /// Valida si el formato de fecha es dd/MM/YYYY
        /// </summary>
        /// <param name="Fecha">Fecha a validar.</param>
        /// <returns>Si se cumple el formato regresa true.</returns>
        private static bool validaFormatoFecha(string Fecha)
        {
            //bool fechaValida = false;
            DateTime dateTime;
            //System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^\d{2}\/\d{2}\/\d{4}$");

            //RZ.20131128 Se cambia expresion regular ya que el formato de la fecha se encontraba fijo
            //Con este metodo hara la conversion a un datetime usando el formato de la cultura actual en la aplicacion
            return DateTime.TryParseExact(Fecha, CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
        }

        /// <summary>
        /// Valida que un string solo contenga numeros y/o letras.
        /// </summary>
        /// <param name="campoAValidar">Cadena a validar.</param>
        /// <returns>Si el formato es correcto regresa un true. </returns>
        private static bool validaFormatoCampos(string campoAValidar)
        {
            bool formatoValido = false;
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z0-9]*$");

            if (regex.IsMatch(campoAValidar))
            {
                return formatoValido = true;
            }

            return formatoValido;
        }

        //AM 20131105 Se agrega seccion para agregar puesto
        #region Agregar Puesto

        protected void btnAgregarPuesto_Click(object sender, EventArgs e)
        {
            mpeAddPuesto.Show();
        }

        protected void btnGuardarPuesto_Click(object sender, EventArgs e)
        {
            //Primero se valida que los campos no esten vacios.
            if (txtPuestoDesc.Text != string.Empty)
            {
                //Despues se hace una consulta para ver si la clave del puesto que se esta dando de alta no existe ya en el sistema
                StringBuilder sbQuery = new StringBuilder();

                sbQuery.Append("select count(*) from [VisHistoricos('Puesto','Puestos Empleado','Español')] \r");
                sbQuery.Append("where dtIniVigencia <> dtFinVigencia \r");
                sbQuery.Append("and dtFinVigencia >= getdate() \r");
                sbQuery.Append("and vchDescripcion like '%" + txtPuestoDesc.Text.Trim().Replace(' ', '%') + "%'");

                //Si el puesto no existe en el sistema entonces se manda llamar el metodo que agrega el puesto.
                if (Convert.ToInt32(DSODataAccess.ExecuteScalar(sbQuery.ToString())) == 0)
                {
                    DALCCustodia dalCCust = new DALCCustodia();
                    //dalCCust.AddPuesto(txtPuestoDesc.Text.Trim());
                    DSODataAccess.Execute("EXEC [dbo].[CreaPuestosEmples] @Esquema='"+DSODataContext.Schema+ "', @Puesto='"+ txtPuestoDesc.Text.Trim().ToUpper() + "' ");
                    //drpPuestoEmple.Items.Clear();
                    //drpPuestoEmple.Items.Add("-- Selecciona uno --");
                    FillDDLPuestoEmple();
                    drpPuestoEmple.Items.FindByText(txtPuestoDesc.Text.Trim().ToUpper()).Selected = true;
                    txtPuestoDesc.Text = string.Empty;
                }
                else
                {
                    MensajeDeAdvertencia("El puesto que intenta dar de alta ya existe en el sistema");
                    mpeAddPuesto.Show();
                    txtPuestoDesc.Focus();
                }
            }
            else
            {
                MensajeDeAdvertencia("Debe capturar una descripción del puesto");
                mpeAddPuesto.Show();
                txtPuestoDesc.Focus();
            }
        }

        #endregion

        #region Agregar CenCos

        protected void btnAgregarCenCos_Click(object sender, EventArgs e)
        {
            limpiaCamposPopUpAddCenCos();
            mpeAddCenCos.Show();
            FillDDLCenCosResponsable();
            FillDDLEmpleRespCenCos();
        }

        /// <summary>
        /// Llena el DropDownList de Centros de costos responsable en PopUp AddCenCos.
        /// </summary>
        protected void FillDDLCenCosResponsable()
        {

            DataTable dtCenCosResponsable = new DataTable();

            dtCenCosResponsable = NuevoEmpleadoBackend.GetCenCos();

            drpCenCosResponsable.DataSource = dtCenCosResponsable;
            drpCenCosResponsable.DataValueField = "iCodCatalogo";
            drpCenCosResponsable.DataTextField = "vchDescripcion";
            drpCenCosResponsable.DataBind();
        }

        /// <summary>
        /// Llena el DropDownList de Empleado responsable en PopUp AddCenCos.
        /// </summary>
        protected void FillDDLEmpleRespCenCos()
        {
            DataTable dtEmpleRespCenCos = new DataTable();
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("SELECT iCodCatalogo, vchDescripcion = NomCompleto");
            lsbQuery.AppendLine("FROM [VisHistoricos('Emple','Empleados','Español')]");
            lsbQuery.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            lsbQuery.AppendLine("   AND dtFinVigencia >= GETDATE()");
            lsbQuery.AppendLine("   AND NomCompleto <> ''");
            lsbQuery.AppendLine("   AND vchDescripcion not like '%identif%'");
            lsbQuery.AppendLine("ORDER BY NomCompleto");

            dtEmpleRespCenCos = DSODataAccess.Execute(lsbQuery.ToString());

            drpEmpleRespCenCos.DataSource = dtEmpleRespCenCos;
            drpEmpleRespCenCos.DataValueField = "iCodCatalogo";
            drpEmpleRespCenCos.DataTextField = "vchDescripcion";
            drpEmpleRespCenCos.DataBind();
        }

        protected void btnGuardarCenCos_Click(object sender, EventArgs e)
        {
            try
            {
                #region Proceso Alta CenCos
                string validaCampos = validaCamposCenCos();

                //Se valida que los campos necesarios no esten vacios y tengan el formato correcto.
                if (validaCampos.Length > 0)
                {
                    MensajeDeAdvertencia(validaCampos);
                    txtClaveCenCos.Focus();
                    mpeAddCenCos.Show();
                }
                else
                {
                    string validaVigencias = validaVigenciasResponsablesCenCos();

                    //Se valida que la fecha de inicio sea mayor a las fechas de inicio del CenCos y Empleado responsables.
                    if (validaVigencias.Length > 0)
                    {
                        MensajeDeAdvertencia(validaVigencias);
                        txtFechaInicioCenCos.Focus();
                        mpeAddCenCos.Show();
                    }
                    //Se valida que no exista ya un centro de costos con el número o descripción proporcionados.
                    else
                    {
                        string validaClaveDescNoDup = validaClaveYDescNoDuplicados();

                        if (validaClaveDescNoDup.Length > 0)
                        {
                            MensajeDeAdvertencia(validaClaveDescNoDup);
                            if (validaClaveDescNoDup.Contains("número"))
                            {
                                txtClaveCenCos.Text = string.Empty;
                                txtClaveCenCos.Focus();
                            }

                            if (validaClaveDescNoDup.Contains("nombre"))
                            {
                                txtCenCosDesc.Text = string.Empty;
                                if (txtClaveCenCos.Text == string.Empty)
                                {
                                    txtClaveCenCos.Focus();
                                }
                                else
                                {
                                    txtCenCosDesc.Focus();
                                }
                            }
                            mpeAddCenCos.Show();
                        }
                        //Si no hay errores se guarda el Centro de costos.
                        else
                        {
                            DALCCustodia dalCCust = new DALCCustodia();
                            dalCCust.AddCenCos(GetCenCosValuesHist());
                            drpCenCosEmple.Items.Clear();
                            drpCenCosEmple.Items.Add("-- Selecciona uno --");
                            drpCenCosEmple.Items.FindByText("-- Selecciona uno --").Value = "0";
                            FillDDLCenCosEmple();

                            #region ActualizaJerarquia
                            string iCodNuevoCenCos = drpCenCosEmple.Items.FindByText(txtCenCosDesc.Text + " (" + GetvchCodCenCosResp(drpCenCosResponsable.Text) + ")").Value.ToString();
                            drpCenCosEmple.Items.FindByValue(iCodNuevoCenCos).Selected = true;
                            ActualizaJerarquiaRestCenCos(iCodNuevoCenCos, drpCenCosResponsable.Text);
                            #endregion
                            mpeAddCenCos.Hide();
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al grabar el Centro de costos '" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        /// <summary>
        /// Inicializa los valores de los campos de la popup AddCenCos a sus valores default
        /// </summary>
        private void limpiaCamposPopUpAddCenCos()
        {
            try
            {
                #region Regresa valores por defecto de los campos para alta de CenCos
                txtClaveCenCos.Text = string.Empty;
                txtCenCosDesc.Text = string.Empty;
                txtFechaInicioCenCos.Text = string.Empty;
                drpCenCosResponsable.SelectedItem.Selected = false;
                drpEmpleRespCenCos.SelectedItem.Selected = false;
                drpCenCosResponsable.Items.FindByValue("0").Selected = true;
                drpEmpleRespCenCos.Items.FindByValue("0").Selected = true;
                #endregion
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException(
                    "Ocurrio un error al regresar campos a sus valores por defecto en metodo limpiaCamposPopUpAddCenCos() en AppCCustodia.aspx.cs '"
                    + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        /// <summary>
        /// Adquiere el valor de los campos que se grabaran en la BD.
        /// </summary>
        /// <returns></returns>
        public Hashtable GetCenCosValuesHist()
        {
            //string iCodCatalogoEmpresa = GetIcodCatalogoEmpresa("Nextel"); //Pendiente
            string iCodCatalogoEmpresa = drpCenCosEmpresa.Text;
            string vchCodigoCenCosResp = GetvchCodCenCosResp(drpCenCosResponsable.Text);

            Hashtable lht = new Hashtable();
            lht.Add("iCodMaestro", DALCCustodia.getiCodMaestro("Centro de Costos", "CenCos")); //iCodMaestro 
            lht.Add("vchCodigo", txtClaveCenCos.Text); //vchCodigo            
            lht.Add("vchDescripcion", txtCenCosDesc.Text + " (" + vchCodigoCenCosResp + ")"); //vchDescripcion
            lht.Add("[iCodCatalogo01]", (drpCenCosResponsable.Text != "0") ? drpCenCosResponsable.Text : null); //CenCos Responsable
            lht.Add("[iCodCatalogo02]", (drpEmpleRespCenCos.Text != "0") ? drpEmpleRespCenCos.Text : null); //Empleado Responsable
            lht.Add("[iCodCatalogo03]", null); //Tipo presup
            lht.Add("[iCodCatalogo04]", null); //PeriodoPr
            lht.Add("[iCodCatalogo05]", iCodCatalogoEmpresa); //Empresa
            lht.Add("[Integer01]", 0); //CenCos Duplicado (No Duplicado)
            lht.Add("[Integer02]", null); //Nivel Jerarquico
            lht.Add("[Float01]", null); //PresupFijo
            lht.Add("[VarChar02]", null); //Cuenta contable
            lht.Add("dtIniVigencia", Convert.ToDateTime(txtFechaInicioCenCos.Text));
            lht.Add("dtFecUltAct", DateTime.Now);
            lht.Add("iCodUsuario", (int)HttpContext.Current.Session["iCodUsuario"]); //iCodUsuario
            lht.Add("{Descripcion}", txtCenCosDesc.Text); //Descripcion

            return lht;
        }

        /// <summary>
        /// Adquiere el iCodCatalogo de la empresa.
        /// </summary>
        /// <returns></returns>
        private static string GetIcodCatalogoEmpresa(string Empresa)
        {
            StringBuilder lsbQueryEmpre = new StringBuilder();
            string empresa = string.Empty;

            try
            {
                #region Consulta el iCodCatalodo de la Empresa


                lsbQueryEmpre.Append("select iCodCatalogo from [VisHistoricos('Empre','Empresas','Español')] \r");
                lsbQueryEmpre.Append("where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate() \r");
                lsbQueryEmpre.Append("and vchDescripcion = '" + Empresa + "' \r");

                empresa = DSODataAccess.ExecuteScalar(lsbQueryEmpre.ToString()).ToString();
                #endregion
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException(
                    "Ocurrio un error al consultar la empresa en metodo GetIcodCatalogoEmpresa() en AppCCustodia.aspx.cs '"
                    + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }

            return empresa;
        }

        /// <summary>
        /// Regresa el vchCodigo del CenCos responsable.
        /// </summary>
        /// <param name="iCodCatCenCosResp">iCodCatalgo de CenCosResp. </param>
        /// <returns></returns>
        private static string GetvchCodCenCosResp(string iCodCatCenCosResp)
        {
            StringBuilder lsbQueryCenCosResp = new StringBuilder();
            string cenCosResp = string.Empty;
            try
            {
                #region Consulta el iCodCatalogo del CenCosResponsable
                lsbQueryCenCosResp.Append("select vchCodigo from [VisHistoricos('CenCos','Centro de Costos','Español')] \r");
                lsbQueryCenCosResp.Append("where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate() \r");
                lsbQueryCenCosResp.Append("and iCodCatalogo = '" + iCodCatCenCosResp + "' \r");

                cenCosResp = DSODataAccess.ExecuteScalar(lsbQueryCenCosResp.ToString()).ToString();

                #endregion
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException(
                    "Ocurrio un error al consultar el iCodCatalogo del CenCosResponsable en metodo GetvchCodCenCosResp(string iCodCatCenCosResp) en AppCCustodia.aspx.cs '"
                    + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }

            return cenCosResp;
        }

        /// <summary>
        /// Valida que los campos requeridos no esten vacios.
        /// </summary>
        /// <returns>Regresa una cadena con las notificaciones de los campos que son requeridos. </returns>
        private string validaCamposCenCos()
        {
            StringBuilder sbErrors = new StringBuilder(string.Empty);

            if (txtClaveCenCos.Text == string.Empty || txtClaveCenCos.Text == "")
            {
                sbErrors.Append(@"*El campo (Número) es requerido. \n");
            }
            if (!validaFormatoCampos(txtClaveCenCos.Text))
            {
                sbErrors.Append(@"*El campo (Número) solo debe contener números y/o letras. \n");
            }
            if (txtCenCosDesc.Text == string.Empty || txtCenCosDesc.Text == "")
            {
                sbErrors.Append(@"*El campo (Nombre) es requerido. \n");
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtCenCosDesc.Text, "^[a-zA-Z0-9() ]*$"))
            {
                sbErrors.Append(@"*El campo (Nombre) solo debe contener números, letras, parentesis y espacios. \n");
            }
            if (txtFechaInicioCenCos.Text == string.Empty || txtFechaInicioCenCos.Text == "")
            {
                sbErrors.Append(@"*El campo (Fecha inicio) es requerido. \n");
            }
            if (!validaFormatoFecha(txtFechaInicioCenCos.Text))
            {
                sbErrors.Append(@"*El formato de (Fecha inicio) debe ser DD/MM/AAAA. \n");
                txtFechaInicioCenCos.Text = string.Empty;
            }
            if (drpCenCosResponsable.Text == "0")
            {
                sbErrors.Append(@"*El campo (Centro de costos responsable) es requerido. \n");
            }
            if (drpCenCosEmpresa.Text == "0")
            {
                sbErrors.Append(@"*El campo (Empresa) es requerido. \n");
            }

            return sbErrors.ToString();
        }

        /// <summary>
        /// Valida que la fecha de inicio del nuevo CenCos no sea menor al inicio de vigencia del CenCos y Empleado Responsable.
        /// </summary>
        /// <returns></returns>
        private string validaVigenciasResponsablesCenCos()
        {
            StringBuilder sbErrors = new StringBuilder(string.Empty);
            StringBuilder sbQuery = new StringBuilder(string.Empty);

            //Valida que la fecha de inicio del centro de costos no sea menor a la fecha de inicio del CenCos Responsable.
            if (drpCenCosResponsable.Text != "0")
            {
                try
                {
                    #region Consulta dtIniVigencia del Centro de Costos responsable
                    sbQuery.Length = 0;
                    sbQuery.AppendLine("select dtIniVigencia from [VisHistoricos('CenCos','Centro de Costos','Español')]");
                    sbQuery.AppendLine("where dtIniVigencia<>dtFinVigencia");
                    sbQuery.AppendLine("and dtFinVigencia >= getdate()");
                    sbQuery.AppendLine("and iCodCatalogo = " + drpCenCosResponsable.Text);
                    DateTime fechaIniCenCosResp = (DateTime)DSODataAccess.ExecuteScalar(sbQuery.ToString());

                    if (Convert.ToDateTime(txtFechaInicioCenCos.Text) < fechaIniCenCosResp ||
                        Convert.ToDateTime(txtFechaInicioCenCos.Text) == fechaIniCenCosResp)
                    {
                        sbErrors.Append(@"*El inicio de vigencia del nuevo centro de costos no puede ser menor al inicio de vigencia del centro de costos responsable. \n");
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    KeytiaServiceBL.Util.LogException(
                        "Ocurrio un error al consultar dtIniVigencia del CenCosResponsable en metodo validaVigenciasResponsablesCenCos() en AppCCustodia.aspx.cs '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }

            //Valida que la fecha de inicio del centro de costos no sea menor a la fecha de inicio del Empleado Responsable.
            if (drpEmpleRespCenCos.Text != "0")
            {
                try
                {
                    #region Consulta dtIniVigencia del empleado responsable
                    sbQuery.Length = 0;
                    sbQuery.AppendLine("select dtIniVigencia from [VisHistoricos('Emple','Empleados','Español')]");
                    sbQuery.AppendLine("where dtIniVigencia<>dtFinVigencia");
                    sbQuery.AppendLine("and dtFinVigencia >= getdate()");
                    sbQuery.AppendLine("and iCodCatalogo = " + drpEmpleRespCenCos.Text);
                    DateTime fechaIniEmpleResp = (DateTime)DSODataAccess.ExecuteScalar(sbQuery.ToString());

                    if (Convert.ToDateTime(txtFechaInicioCenCos.Text) < fechaIniEmpleResp ||
                        Convert.ToDateTime(txtFechaInicioCenCos.Text) == fechaIniEmpleResp)
                    {
                        sbErrors.Append(@"*El inicio de vigencia del nuevo centro de costos no puede ser menor al inicio de vigencia del empleado responsable. \n");
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    KeytiaServiceBL.Util.LogException(
                        "Ocurrio un error al consultar dtIniVigencia del empleado responsable en metodo validaVigenciasResponsablesCenCos() en AppCCustodia.aspx.cs '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }
            }

            return sbErrors.ToString();
        }

        /// <summary>
        /// Valida que no exista la clave y descripción del CenCos que se desea dar de alta.
        /// </summary>
        /// <returns></returns>
        private string validaClaveYDescNoDuplicados()
        {
            StringBuilder sbErrors = new StringBuilder(string.Empty);
            StringBuilder sbQuery = new StringBuilder(string.Empty);

            //Valida si existe un centro de costos con la clave proporcionada.
            if (txtClaveCenCos.Text != string.Empty)
            {
                try
                {
                    #region Consulta para ver si existe un Centro de costos vigente con numero (clave) proporcionada

                    sbQuery.Length = 0;
                    sbQuery.AppendLine("select count(*) from [VisHistoricos('CenCos','Centro de Costos','Español')]");
                    sbQuery.AppendLine("where dtIniVigencia<>dtFinVigencia");
                    sbQuery.AppendLine("and dtFinVigencia >= getdate()");
                    sbQuery.AppendLine("and vchCodigo = '" + txtClaveCenCos.Text + "'");
                    string clave = DSODataAccess.ExecuteScalar(sbQuery.ToString()).ToString();

                    if (clave != "0")
                    {
                        sbErrors.Append(@"*Ya existe un Centro de costos con el número proporcionado. \n");
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    KeytiaServiceBL.Util.LogException(
                        "Ocurrio un error en metodo validaClaveYDescNoDuplicados() en la consulta de numero (clave) en AppCCustodia.aspx.cs '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }

            }

            //Valida si existe un centro de costos con la descripcion proporcionada.
            if (txtCenCosDesc.Text != string.Empty)
            {
                try
                {
                    #region Consulta para ver si existe un Centro de costos vigente con el nombre (descripcion) proporcionado
                    sbQuery.Length = 0;
                    sbQuery.AppendLine("select count(*) from [VisHistoricos('CenCos','Centro de Costos','Español')]");
                    sbQuery.AppendLine("where dtIniVigencia<>dtFinVigencia");
                    sbQuery.AppendLine("and dtFinVigencia >= getdate()");
                    sbQuery.AppendLine("and vchDescripcion like '%" + txtCenCosDesc.Text + "%'");
                    string descripcion = DSODataAccess.ExecuteScalar(sbQuery.ToString()).ToString();

                    if (descripcion != "0")
                    {
                        sbErrors.Append(@"*Ya existe un Centro de costos con el nombre proporcionado. \n");
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    KeytiaServiceBL.Util.LogException(
                        "Ocurrio un error en metodo validaClaveYDescNoDuplicados() en la consulta de nombre (descripcion) en AppCCustodia.aspx.cs '"
                        + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
                }

            }

            return sbErrors.ToString();
        }

        /// <summary>
        /// Actualiza jerarquia de CenCos
        /// </summary>
        /// <param name="iCodCatalogo">iCodCatalogo de nuevo CenCos</param>
        /// <param name="iCodPadre">iCodCatalogo de CenCos Responsable</param>
        protected void ActualizaJerarquiaRestCenCos(string iCodCatalogo, string iCodPadre)
        {
            KeytiaCOM.ICargasCOM pCargaCom = (KeytiaCOM.ICargasCOM)Marshal.BindToMoniker("queue:/new:KeytiaCOM.CargasCOM");

            int liCodUsuario = (int)Session["iCodUsuarioDB"];
            pCargaCom.ActualizaJerarquiaRestCenCos(iCodCatalogo, iCodPadre, liCodUsuario);
            Marshal.ReleaseComObject(pCargaCom);
            pCargaCom = null;
        }

        #endregion

        //RZ.20131129 Guardar Comentarios de Administrador
        protected void lbtnGuardarComentAdmin_Click(object sender, EventArgs e)
        {
            if (txtComentariosAdmin.Text.Length <= 250)
            {
                DALCCustodia.actualizaComentAdmin(iCodCatalogoEmple, txtComentariosAdmin.Text, Session["iCodUsuario"].ToString());
            }
            else
            {
                MensajeDeAdvertencia("Los comentarios no pueden exceder los 250 caracteres.");
            }
        }

        //RM 20190329 Método para cambiar la visibilidad de los campos prpios de FCA (Chrysler)
        private void ModifVisiilidadCamposFCA()
        {
            try
            {
                //DDLS
                divPentaSAPAccount.Visible = true;
                divPentaSAPCCDesc.Visible = true;
                divPentaSAPCostCenter.Visible = true;
                divPentaSAPFAFCA.Visible = true;
                divPentaSAPProfitCenter.Visible = true;



                //COLUMNS
                var PentaSAPCCDescriptionDesc = grvLinea.Columns[10];
                PentaSAPCCDescriptionDesc.Visible = true;

                var PentaSAPAccountDesc = grvLinea.Columns[12];
                PentaSAPAccountDesc.Visible = true;

                var PentaSAPProfitCenterDesc = grvLinea.Columns[14];
                PentaSAPProfitCenterDesc.Visible = true;

                var PentaSAPCostCenterDesc = grvLinea.Columns[16];
                PentaSAPCostCenterDesc.Visible = true;

                var PentaSAPFADesc = grvLinea.Columns[18];
                PentaSAPFADesc.Visible = true;


                //DDLs PopUp
                divPentaSAPAccountPopUp.Visible = true;
                divPentaSAPCCDescriptionPopUp.Visible = true;
                divPentaSAPCostCenterPopUp.Visible = true;
                divPentaSAPFAPopUp.Visible = true;
                divPentaSAPProfitCenterPopUp.Visible = true;
                divPopUpBR.Visible = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void FillDDLFCA()
        {
            try
            {
                DataTable dtPentaSAPCCDesc = BuscaPentaSAPCCDesc("VisHistoricos('PentaSAPCCDescription','PentaSAP CC Descriptions','Español')");
                DataTable dtPentaSAPAccount = BuscaPentaSAPAccount("VisHistoricos('PentaSAPAccount','PentaSAP Accounts','Español')");
                DataTable dtPentaSAPProfitCenter = BuscaPentaSAPProfitCenter("VisHistoricos('PentaSAPProfitCenter','PentaSAP Profit Centers','Español')");
                DataTable dtPentaSAPCostCenter = BuscaPentaSAPCostCenter("VisHistoricos('PentaSAPCostCenter','PentaSAP CostCenters','Español')");
                DataTable dtPentaSAPFA = BuscaPentaSAPFA("VisHistoricos('PentaSAPFA','PentaSAP FA','Español')");


                if (dtPentaSAPCCDesc != null && dtPentaSAPCCDesc.Rows.Count > 0)
                {

                    ddlPentaSAPCCDesc.DataSource = dtPentaSAPCCDesc;
                    ddlPentaSAPCCDesc.DataValueField = "iCodCat";
                    ddlPentaSAPCCDesc.DataTextField = "Descripcion";
                    ddlPentaSAPCCDesc.DataBind();

                    ddlPentaSAPCCDescriptionPopUp.DataSource = dtPentaSAPCCDesc;
                    ddlPentaSAPCCDescriptionPopUp.DataValueField = "iCodCat";
                    ddlPentaSAPCCDescriptionPopUp.DataTextField = "Descripcion";
                    ddlPentaSAPCCDescriptionPopUp.DataBind();
                }

                if (dtPentaSAPAccount != null && dtPentaSAPAccount.Rows.Count > 0)
                {
                    ddlPentaSAPAccount.DataSource = dtPentaSAPAccount;
                    ddlPentaSAPAccount.DataValueField = "iCodCat";
                    ddlPentaSAPAccount.DataTextField = "Descripcion";
                    ddlPentaSAPAccount.DataBind();

                    ddlPentaSAPAccountPopUp.DataSource = dtPentaSAPAccount;
                    ddlPentaSAPAccountPopUp.DataValueField = "iCodCat";
                    ddlPentaSAPAccountPopUp.DataTextField = "Descripcion";
                    ddlPentaSAPAccountPopUp.DataBind();
                }

                if (dtPentaSAPProfitCenter != null && dtPentaSAPProfitCenter.Rows.Count > 0)
                {
                    ddlPentaSAPProfitCenter.DataSource = dtPentaSAPProfitCenter;
                    ddlPentaSAPProfitCenter.DataValueField = "iCodCat";
                    ddlPentaSAPProfitCenter.DataTextField = "Descripcion";
                    ddlPentaSAPProfitCenter.DataBind();

                    ddlPentaSAPProfitCenterPopUp.DataSource = dtPentaSAPProfitCenter;
                    ddlPentaSAPProfitCenterPopUp.DataValueField = "iCodCat";
                    ddlPentaSAPProfitCenterPopUp.DataTextField = "Descripcion";
                    ddlPentaSAPProfitCenterPopUp.DataBind();
                }

                if (dtPentaSAPCostCenter != null && dtPentaSAPCostCenter.Rows.Count > 0)
                {
                    ddlPentaSAPCostCenter.DataSource = dtPentaSAPCostCenter;
                    ddlPentaSAPCostCenter.DataValueField = "iCodCat";
                    ddlPentaSAPCostCenter.DataTextField = "Descripcion";
                    ddlPentaSAPCostCenter.DataBind();

                    ddlPentaSAPCostCenterPopUp.DataSource = dtPentaSAPCostCenter;
                    ddlPentaSAPCostCenterPopUp.DataValueField = "iCodCat";
                    ddlPentaSAPCostCenterPopUp.DataTextField = "Descripcion";
                    ddlPentaSAPCostCenterPopUp.DataBind();


                }

                if (dtPentaSAPFA != null && dtPentaSAPFA.Rows.Count > 0)
                {
                    ddlPentaSAPFAFCA.DataSource = dtPentaSAPFA;
                    ddlPentaSAPFAFCA.DataValueField = "iCodCat";
                    ddlPentaSAPFAFCA.DataTextField = "Descripcion";
                    ddlPentaSAPFAFCA.DataBind();


                    ddlPentaSAPFAPopUp.DataSource = dtPentaSAPFA;
                    ddlPentaSAPFAPopUp.DataValueField = "iCodCat";
                    ddlPentaSAPFAPopUp.DataTextField = "Descripcion";
                    ddlPentaSAPFAPopUp.DataBind();

                }


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private DataTable BuscaPentaSAPCCDesc(string vista)
        {
            try
            {
                DataTable dt = new DataTable();
                string query = ConsultaBuscaInfoParaDDLFCA(vista);
                if (query != "")
                {
                    dt = DSODataAccess.Execute(query);
                }

                return dt;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private DataTable BuscaPentaSAPAccount(string vista)
        {
            try
            {
                DataTable dt = new DataTable();
                string query = ConsultaBuscaInfoParaDDLFCA(vista);
                if (query != "")
                {
                    dt = DSODataAccess.Execute(query);
                }

                return dt;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private DataTable BuscaPentaSAPProfitCenter(string vista)
        {
            try
            {
                DataTable dt = new DataTable();
                string query = ConsultaBuscaInfoParaDDLFCA(vista);
                if (query != "")
                {
                    dt = DSODataAccess.Execute(query);
                }

                return dt;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private DataTable BuscaPentaSAPCostCenter(string vista)
        {
            try
            {
                DataTable dt = new DataTable();
                string query = ConsultaBuscaInfoParaDDLFCA(vista);
                if (query != "")
                {
                    dt = DSODataAccess.Execute(query);
                }

                return dt;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private DataTable BuscaPentaSAPFA(string vista)
        {
            try
            {
                DataTable dt = new DataTable();
                string query = ConsultaBuscaInfoParaDDLFCA(vista);
                if (query != "")
                {
                    dt = DSODataAccess.Execute(query);
                }

                return dt;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        private int BuscarEmpleEnHistEmple()
        {
            try
            {
                int res = 0;
                StringBuilder query = new StringBuilder();

                query.AppendLine("select  Max(emple.iCodCatalogo)														");
                query.AppendLine("from [" + DSODataContext.Schema + "].[VisHistoricos('Emple','Empleados','Español')] emple							");
                query.AppendLine("Inner Join [" + DSODataContext.Schema + "].[VisHistoricos('EmpleFCA','Empleados FCA','Español')] emplefca			");
                query.AppendLine("	on emple.icodcatalogo = emplefca.emple												");
                query.AppendLine("	And emplefca.dtinivigencia <> emplefca.dtfinvigencia								");
                query.AppendLine("	And emplefca.dtfinvigencia >= getdate()												");
                query.AppendLine("where emple.dtinivigencia <> emple.dtFinvigencia 										");
                query.AppendLine("and emple.dtFinvigencia >= getdate()													");
                query.AppendLine("And emple.NominaA = '" + txtNominaEmple.Text + "'														");
                query.AppendLine("And emple.Email = '" + txtEmailEmple.Text + "'								");
                query.AppendLine("And Nombre = '" + txtNombreEmple.Text + "'																");
                query.AppendLine("And Materno = '" + txtApMaternoEmple.Text + "'																");
                query.AppendLine("And Paterno = '" + txtApPaternoEmple.Text + "'																");

                int.TryParse(DSODataAccess.ExecuteScalar(query.ToString()).ToString(), out res);

                return res;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool ActualizarDatosFCA(Dictionary<string, string> dctEmpleFCA, int iCodCatEmple)
        {
            try
            {
                bool res = false;
                StringBuilder query = new StringBuilder();

                if (DSODataContext.Schema.ToUpper() == "FCA")
                {
                    //Actualiza Banderaemple en Emple

                    query.AppendLine("Update emplefca																");
                    query.AppendLine("Set 																			");
                    query.AppendLine("	FCA_C_ID = '" + dctEmpleFCA["Dc_id"].ToString() + "',																");
                    query.AppendLine("	FCA_T_ID = '" + dctEmpleFCA["T_id"].ToString() + "',																");
                    query.AppendLine("	FCA_Estacion = '" + dctEmpleFCA["Estacion"].ToString() + "',															");
                    query.AppendLine("	iCodCatalogoDirector = " + dctEmpleFCA["Director"].ToString() + ",													");
                    query.AppendLine("  PlantaFCA  = " + dctEmpleFCA["planta"].ToString() + ",");
                    query.AppendLine("	dtFecUltAct = GETDATE()														");
                    query.AppendLine("From fca.[VisHistoricos('EmpleFCA','Empleados FCA','Español')]  emplefca		");
                    query.AppendLine("Where dtIniVigencia <> dtFinVigencia											");
                    query.AppendLine("And dtFinVigencia >= GETDATE()												");
                    query.AppendLine("And Emple = " + iCodCatEmple + "															");


                    if (dctEmpleFCA["esDirector"].ToString().ToLower() == "true")
                    {
                        query.AppendLine("declare @vEsdirector int =0											");
                        query.AppendLine("																		");
                        query.AppendLine("select  @vEsdirector = value 											");
                        query.AppendLine("From [" + DSODataContext.Schema + "].[VisHistoricos('Valores','Valores','Español')]				");
                        query.AppendLine("where dtIniVigencia <> dtFinVigencia									");
                        query.AppendLine("And dtFinVigencia >= GETDATE()										");
                        query.AppendLine("And vchCodigo = 'EmpleEsDirector'										");
                        query.AppendLine("And Atrib = 															");
                        query.AppendLine("(																		");
                        query.AppendLine("	Select iCodCatalogo                             					");
                        query.AppendLine("	From [" + DSODataContext.Schema + "].[VisHistoricos('Atrib','Atributos','Español')]				");
                        query.AppendLine("	Where dtIniVigencia <> dtFinVigencia								");
                        query.AppendLine("	And  dtFinVigencia >= GETDATE()										");
                        query.AppendLine("	And vchCodigo like 'BanderasEmple'									");
                        query.AppendLine(")																		");
                        query.AppendLine("																		");
                        query.AppendLine("																		");
                        query.AppendLine("if(@vEsdirector > 0)													");
                        query.AppendLine("Begin																	");
                        query.AppendLine("		Update Emple													");
                        query.AppendLine("	set 																");
                        query.AppendLine("		emple.BanderasEmple = emple.BanderasEmple +@vEsdirector ,   	");
                        query.AppendLine("		dtFecUltAct = GETDATE()											");
                        query.AppendLine("	From [" + DSODataContext.Schema + "].[VisHistoricos('Emple','Empleados','Español')] emple		");
                        query.AppendLine("	Where dtIniVigencia <> dtFinVigencia								");
                        query.AppendLine("	and dtFinVigencia >=getdate()										");
                        query.AppendLine("	And iCodCatalogo = " + iCodCatEmple + "							    ");
                        query.AppendLine("End																	");

                    }

                    DSODataAccess.Execute(query.ToString());
                    res = true;

                    tIdEmple = Session["tIdEmple"].ToString();

                    if (tIdEmple.Trim().ToUpper() != dctEmpleFCA["T_id"].ToString().Trim().ToUpper())
                    {
                        try
                        {
                            /*Se crea un nuevo usuario al empleado si el TID que tiene asigando es diferente al de la caja de texto*/
                            string usuario = dctEmpleFCA["T_id"].ToString().Trim();
                            string pas = Session["passwordEmple"].ToString();
                            string sp = "EXEC [GeneraUsuariosEmpleV2] @Usuario = '{0}',@Password = '{1}',@Emple = {2}";
                            string q = string.Format(sp, usuario, pas, iCodCatEmple);
                            DSODataAccess.ExecuteNonQuery(q.ToString());
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }

                    }
                }

                return res;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        private string ConsultaBuscaInfoParaDDLFCA(string vistaFCA)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendLine();
                if (vistaFCA.Length > 0)
                {

                    query.AppendLine("if(OBJECT_ID('Keytia5." + DSODataContext.Schema + ".[" + vistaFCA.Replace("'", "''") + "]') is not null)	");
                    query.AppendLine("Begin																								");
                    query.AppendLine("	select 																							");
                    query.AppendLine("		iCodCat = iCodCatalogo,																		");
                    query.AppendLine("		Cod = vchCodigo,																			");
                    query.AppendLine("		Descripcion = vchDescripcion																");
                    query.AppendLine("	from keytia5." + DSODataContext.Schema + ".[" + vistaFCA + "] PenataSAPAccount							");
                    query.AppendLine("	where dtIniVigencia <> dtFinVigencia															");
                    query.AppendLine("	and dtFinVigencia >= GETDATE()																	");
                    query.AppendLine("	order by vchDescripcion																			");
                    query.AppendLine("End																								");
                }

                return query.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void drpTipoEmpleado_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (drpTipoEmpleado.Text.Length > 0 && (drpTipoEmpleado.SelectedItem.Text.ToLower() == "externo" || drpTipoEmpleado.SelectedItem.Text.ToLower() == "recurso"))
            {
                txtNominaEmple.Text = GenerarNominaAutomatica();
            }


            //if (drpTipoEmpleado.SelectedItem.Text.ToLower() == "empleado" && DSODataContext.Schema.ToString().ToLower() == "fca")
            //{
            //    drpCenCosEmple.SelectedIndex = 0;
            //    drpCenCosEmple.Enabled = false;
            //}
            //else
            //{
            //    drpCenCosEmple.SelectedIndex = 0;
            //    drpCenCosEmple.Enabled = true;
            //}
        }


        public string GenerarNominaAutomatica()
        {
            try
            {
                string nominaRes = "";

                StringBuilder query = new StringBuilder();

                //if (DSODataContext.Schema.ToLower() == "fca")
                //{
                query.AppendLine("if(Object_id('tempdb..#NominaTemp') is Not Null)													");
                query.AppendLine("Begin																								");
                query.AppendLine("	Drop table #NominaTemp																			");
                query.AppendLine("End 																								");
                query.AppendLine("																									");
                query.AppendLine("																									");
                query.AppendLine("																									");
                query.AppendLine("Select 	Nomina = NominaA																		");
                query.AppendLine("--Nomina = Convert(int,Replace(Replace(Replace(NominaA,'MA',''),'Temp',''),'EX','')	) 			");
                query.AppendLine("into #NominaTemp																					");
                query.AppendLine("From [" + DSODataContext.Schema + "].[VisHistoricos('Emple','Empleados','Español')]				");
                query.AppendLine("Where dtIniVigencia <> dtFinVigencia																");
                query.AppendLine("And dtFinVigencia >= GETDATE()																	");
                query.AppendLine("And isNumeric(NominaA)  =1 																		");
                query.AppendLine("--And isNumeric(Replace(Replace(Replace(NominaA,'MA',''),'Temp',''),'EX','')	) = 1				");
                query.AppendLine("--And Len(Replace(Replace(Replace(NominaA,'MA',''),'Temp',''),'EX','')) <=9						");
                query.AppendLine("And NominaA <> '999999999'																		");
                query.AppendLine("																									");
                query.AppendLine("Select max(Nomina) + 1																			");
                query.AppendLine("From #NominaTemp																					");
                //}

                DataTable dtRes = DSODataAccess.Execute(query.ToString());


                nominaRes = dtRes.Rows[0][0].ToString();

                return nominaRes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        protected void btnAgregaTipoAsig_Click(object sender, ImageClickEventArgs e)
        {
            mpeAddTipo.Show();
        }

        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {
            mpAddCategoria.Show();
        }

        protected void btnAgregarTipo_Click(object sender, EventArgs e)
        {

            //Primero se valida que los campos no esten vacios.
            if (txtTipoAsigacion.Text != string.Empty)
            {
                //Despues se hace una consulta para ver si la clave del puesto que se esta dando de alta no existe ya en el sistema
                StringBuilder sbQuery = new StringBuilder();

                sbQuery.Append("select count(*) from [VisHistoricos('LineaTipoAsignacion','Linea Tipos Asignación','Español')] \r");
                sbQuery.Append("where dtIniVigencia <> dtFinVigencia \r");
                sbQuery.Append("and dtFinVigencia >= getdate() \r");
                sbQuery.Append("and Descripcion = '" + txtTipoAsigacion.Text.Trim() + "'");

                if (Convert.ToInt32(DSODataAccess.ExecuteScalar(sbQuery.ToString())) == 0)
                {
                    int icodLinea = Convert.ToInt32(idLinea.Value);
                    int rowIndex = Convert.ToInt32(Index.Value);

                    StringBuilder query = new StringBuilder();
                    query.AppendLine(" EXEC dbo.AltaTipoCategoria");
                    query.AppendLine(" @Esquema ='" + DSODataContext.Schema + "',");
                    query.AppendLine(" @IcodLinea = " + icodLinea + ",");
                    query.AppendLine(" @Dato = '" + txtTipoAsigacion.Text.Trim().ToUpper()+ "',");
                    query.AppendLine(" @TipoAlta = 1");
                    DSODataAccess.ExecuteNonQuery(query.ToString());

                    LlenaPopUpLineaEdit(rowIndex);
                }
                else
                {
                    //MensajeDeAdvertencia("El Tipo Asignación que intenta dar de alta ya existe en el sistema");
                    mpeAddTipo.Show();
                    txtTipoAsigacion.Focus();
                }
            }
            else
            {
                //MensajeDeAdvertencia("Debe capturar un Tipo Asignación");
                mpeAddTipo.Show();
                txtTipoAsigacion.Focus();
            }
        }

        protected void btnAgregarCategoria_Click(object sender, EventArgs e)
        {
            //Primero se valida que los campos no esten vacios.
            if (txtCategoriaAsig.Text != string.Empty)
            {
                //Despues se hace una consulta para ver si la clave del puesto que se esta dando de alta no existe ya en el sistema
                StringBuilder sbQuery = new StringBuilder();

                sbQuery.Append("select count(*) from [VisHistoricos('LineaCategoriaAsignacion','Linea Categorías Asignación','Español')] \r");
                sbQuery.Append("where dtIniVigencia <> dtFinVigencia \r");
                sbQuery.Append("and dtFinVigencia >= getdate() \r");
                sbQuery.Append("and Descripcion = '" + txtCategoriaAsig.Text.Trim() + "'");

                //Si el puesto no existe en el sistema entonces se manda llamar el metodo que agrega el puesto.
                if (Convert.ToInt32(DSODataAccess.ExecuteScalar(sbQuery.ToString())) == 0)
                {

                    int icodLinea = Convert.ToInt32(idLinea.Value);
                    int rowIndex = Convert.ToInt32(Index.Value);

                    StringBuilder query = new StringBuilder();
                    query.AppendLine(" EXEC dbo.AltaTipoCategoria");
                    query.AppendLine(" @Esquema ='" + DSODataContext.Schema + "',");
                    query.AppendLine(" @IcodLinea = " + icodLinea + ",");
                    query.AppendLine(" @Dato = '" + txtCategoriaAsig.Text.Trim().ToUpper() + "',");
                    query.AppendLine(" @TipoAlta = 2");

                    DSODataAccess.ExecuteNonQuery(query.ToString());

                    LlenaPopUpLineaEdit(rowIndex);
                }
                else
                {
                    //MensajeDeAdvertencia("La Categoría de Asignación que intenta dar de alta, ya existe en el sistema");
                    mpAddCategoria.Show();
                    txtCategoriaAsig.Focus();
                }
            }
            else
            {
                //MensajeDeAdvertencia("Debe capturar una Categoría de Asignación");
                mpAddCategoria.Show();
                txtCategoriaAsig.Focus();
            }
        }

        protected void btnAgregarSitioLinea_Click(object sender, ImageClickEventArgs e)
        {

            mpAddSitioLinea.Show();
        }

        protected void btnAgregarSitioLine_Click(object sender, EventArgs e)
        {
            //Primero se valida que los campos no esten vacios.
            if (txtSitioLinea.Text != string.Empty)
            {
                //Despues se hace una consulta para ver si la clave del puesto que se esta dando de alta no existe ya en el sistema
                StringBuilder sbQuery = new StringBuilder();

                sbQuery.Append("select count(*) from [vishistoricos('UbicacionLinea','Ubicación Líneas','español')] \r");
                sbQuery.Append("where dtIniVigencia <> dtFinVigencia \r");
                sbQuery.Append("and dtFinVigencia >= getdate() \r");
                sbQuery.Append("and NombreUbicacion = '" + txtSitioLinea.Text.Trim()+ "'");

                //Si el puesto no existe en el sistema entonces se manda llamar el metodo que agrega el puesto.
                if (Convert.ToInt32(DSODataAccess.ExecuteScalar(sbQuery.ToString())) == 0)
                {

                    int icodLinea = Convert.ToInt32(idLinea.Value);
                    int rowIndex = Convert.ToInt32(Index.Value);

                    StringBuilder query = new StringBuilder();
                    query.AppendLine(" EXEC dbo.AltaTipoCategoria");
                    query.AppendLine(" @Esquema ='" + DSODataContext.Schema + "',");
                    query.AppendLine(" @IcodLinea = " + icodLinea + ",");
                    query.AppendLine(" @Dato = '" + txtSitioLinea.Text.Trim().ToUpper() + "',");
                    query.AppendLine(" @TipoAlta = 3");

                    DSODataAccess.ExecuteNonQuery(query.ToString());

                    LlenaPopUpLineaEdit(rowIndex);
                }
                else
                {
                    //MensajeDeAdvertencia("La Categoría de Asignación que intenta dar de alta, ya existe en el sistema");
                    mpAddSitioLinea.Show();
                    txtSitioLinea.Focus();
                }
            }
            else
            {
                //MensajeDeAdvertencia("Debe capturar una Categoría de Asignación");
                mpAddSitioLinea.Show();
                txtSitioLinea.Focus();
            }
        }

        private void OcultaAsteriscos(bool oculta)
        {
            lblasterisk1.Visible = oculta;
            lblasterisk2.Visible = oculta;
            lblasterisk3.Visible = oculta;
            lblasterisk4.Visible = oculta;
            lblasterisk5.Visible = oculta;
            lblasterisk6.Visible = oculta;
            lblasterisk7.Visible = oculta;
            lblasterisk8.Visible = oculta;
            lblasterisk9.Visible = oculta;
            lblasterisk10.Visible = oculta;
            lblasterisk11.Visible = oculta;
        }

        protected void btnAgregarAreaLinea_Click(object sender, ImageClickEventArgs e)
        {
            mpAddAreaLinea.Show();
        }

        protected void btnAgregarAreaLine_Click(object sender, EventArgs e)
        {
            //Primero se valida que los campos no esten vacios.
            if (txtAddAreaLinea.Text != string.Empty)
            {
                //Despues se hace una consulta para ver si la clave del puesto que se esta dando de alta no existe ya en el sistema
                StringBuilder sbQuery = new StringBuilder();

                sbQuery.Append("select count(*) from [vishistoricos('Area','Areas','Español')]  WITH(NOLOCK) \r");
                sbQuery.Append("where dtIniVigencia <> dtFinVigencia \r");
                sbQuery.Append("and dtFinVigencia >= getdate() \r");
                sbQuery.Append("and Descripcion = '" + txtAddAreaLinea.Text.Trim()+"'");

                //Si el puesto no existe en el sistema entonces se manda llamar el metodo que agrega el puesto.
                if (Convert.ToInt32(DSODataAccess.ExecuteScalar(sbQuery.ToString())) == 0)
                {

                    int icodLinea = Convert.ToInt32(idLinea.Value);
                    int rowIndex = Convert.ToInt32(Index.Value);

                    StringBuilder query = new StringBuilder();
                    query.AppendLine(" EXEC dbo.AltaAreaNueva");
                    query.AppendLine(" @Esquema ='" + DSODataContext.Schema + "' ,");
                    query.AppendLine(" @icodLinea = " + icodLinea + ",");
                    query.AppendLine(" @Area = '" + txtAddAreaLinea.Text.Trim().ToUpper()+"'" );

                    DSODataAccess.ExecuteNonQuery(query.ToString());

                    LlenaPopUpLineaEdit(rowIndex);
                }
                else
                {
                    MensajeDeAdvertencia("El Area que intenta dar de alta, ya existe en el sistema");
                    mpAddAreaLinea.Show();
                    txtAddAreaLinea.Focus();
                }
            }
            else
            {
                //MensajeDeAdvertencia("Debe capturar una Categoría de Asignación");
                mpAddAreaLinea.Show();
                txtAddAreaLinea.Focus();
            }
        }
    }
}


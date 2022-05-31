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


namespace KeytiaWeb.UserInterface.CCustodia
{
    public partial class AppCCustodia : System.Web.UI.Page
    {
        protected DataTable dtInventarioAsignado = new DataTable();
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
        //NZ 20150827 Se agrega seccion Id Usuarios a carta custodia
        protected DataTable dtIdUsuarios = new DataTable();

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

        //Crear instancia del webservice
        public CCustodia webServiceCCustodia = new CCustodia();

        //protected void Page_PreLoad(object sender, EventArgs e)
        //{
        //    /*RZ.20130722*/
        //    estado = KeytiaServiceBL.Util.Decrypt(Request.QueryString["st"]);

        //    if (!Page.IsPostBack)
        //    {
        //        EstablecerEstado(estado);
        //    }
        //}

        //protected void Page_Init(object sender, EventArgs e)
        //{
        //    if (Page.IsPostBack)
        //    {

        //    }

        //}

        protected void Page_Load(object sender, EventArgs e)
        {
            //**PT** Variables necesarias para la exportacion a pdf
            psFileKey = Guid.NewGuid().ToString();
            psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Session.SessionID);
            System.IO.Directory.CreateDirectory(psTempPath);

            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion

            iCodCatalogoEmple = Request.QueryString["iCodEmple"];

            /* Leer parametros para establecer modo en que la página se mostrará
                 * Edicion : edit 
                 * Lectura : ronly
                 * Alta : alta
                 * El parametro se recibe encriptado.
            */

            /*RZ.20130722*/
            estado = KeytiaServiceBL.Util.Decrypt(Request.QueryString["st"]);

            string lsStyleSheet = (string)HttpContext.Current.Session["StyleSheet"];

            /*Se agrega css desde servidor para leerlo desde la carpeta de estilos elegida por la configuracion en K5*/
            Page.Header.Controls.Add(new LiteralControl("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + ResolveUrl(lsStyleSheet + "/CCustodia.css") + "\" />"));

            if (!Page.IsPostBack)
            {
                /*Agregar este event handler al dropdownlist de modelos*/
                //drpModeloPopUp.Attributes.Add("onchange", "GetModeloId(this.options[this.selectedIndex].value)");

                EstablecerEstado(estado);

                FillDropDowns();

                /*RZ.20130718 Se valida si es icodcatalogo del empleado esta nulo o vacio
                 para saber si se requiere cargas los datos y recursos del empleado*/
                if (!String.IsNullOrEmpty(iCodCatalogoEmple))
                {

                    DataTable dtEmple = cargaDatosEmple(iCodCatalogoEmple);

                    FillDatosEmple(dtEmple);

                    FillInventarioGrid();

                    FillExtenGrid();

                    FillCodAutoGrid();

                    FillUsuariosEmpleGrid();

                    //acNoSerie.ContextKey = "None";


                }
            }
            //else
            //{
            //    Control cause = GetPostBackControl(Page);

            //    if (cause != null)
            //    {
            //        if (cause.ID == "lbtnDeleteEmple" || cause.ID == "btnReasignarBajaEmple")
            //        {
            //            validarBajaEmple();
            //        }

            //    }                
            //}
        }

        protected void drpJefeEmple_IndexChanged(object sender, EventArgs e)
        {
            string iCodCatEmpleJefe = drpJefeEmple.SelectedValue;

            txtEmailJefeEmple.Text = webServiceCCustodia.ObtieneEmpleMail(iCodCatEmpleJefe); ;

        }

        /*RZ.20130718 Se agrega validacion para que no llene dropdownlist
         de tipo de extension cuando se trate de estado alta*/

        protected void FillDropDowns()
        {
            if (estado != "alta")
            {
                FillDDLTipoExten();
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

            FillDDLJefeEmple();

            FillDDLCenCosEmple();

            FillDDLLocaliEmple();

            FillDDLTipoEmple();

            FillDDLEmpreOuts();
        }

        protected void FillDDLLocaliEmple()
        {
            StringBuilder lsbQuery = new StringBuilder();

            lsbQuery.Append("SELECT iCodCatalogo, vchDescripcion \r");
            lsbQuery.Append("FROM [VisHistoricos('Estados','Estados','Español')] \r");
            lsbQuery.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
            lsbQuery.Append("and dtFinVigencia >= GETDATE() \r");
            lsbQuery.Append("and Paises = 714 --Mexico \r");
            lsbQuery.Append("order by vchDescripcion");

            dtLocaliEmple = DSODataAccess.Execute(lsbQuery.ToString());

            drpLocalidadEmple.DataSource = dtLocaliEmple;
            drpLocalidadEmple.DataValueField = "iCodCatalogo";
            drpLocalidadEmple.DataTextField = "vchDescripcion";
            drpLocalidadEmple.DataBind();
        }

        protected void FillDDLCenCosEmple()
        {
            StringBuilder lsbQuery = new StringBuilder();

            lsbQuery.Append("SELECT iCodCatalogo, vchCodigo + ' ' + Descripcion as vchDescripcion \r");
            lsbQuery.Append("FROM [VisHistoricos('CenCos','Centro de Costos','Español')] \r");
            lsbQuery.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
            lsbQuery.Append("and dtFinVigencia >= GETDATE() \r");
            lsbQuery.Append("order by vchCodigo + ' ' + Descripcion ");

            dtCenCosEmple = DSODataAccess.Execute(lsbQuery.ToString());

            drpCenCosEmple.DataSource = dtCenCosEmple;
            drpCenCosEmple.DataValueField = "iCodCatalogo";
            drpCenCosEmple.DataTextField = "vchDescripcion";
            drpCenCosEmple.DataBind();
        }

        protected void FillDDLJefeEmple()
        {
            StringBuilder lsbQuery = new StringBuilder();

            lsbQuery.Append("SELECT iCodCatalogo, vchDescripcion = NomCompleto \r");
            lsbQuery.Append("FROM [VisHistoricos('Emple','Empleados','Español')] \r");
            lsbQuery.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
            lsbQuery.Append("and dtFinVigencia >= GETDATE() \r");
            lsbQuery.Append("and NomCompleto <> '' \r");
            lsbQuery.Append("and vchDescripcion not like '%identif%' \r");
            lsbQuery.Append("order by NomCompleto");

            dtJefeEmple = DSODataAccess.Execute(lsbQuery.ToString());

            drpJefeEmple.DataSource = dtJefeEmple;
            drpJefeEmple.DataValueField = "iCodCatalogo";
            drpJefeEmple.DataTextField = "vchDescripcion";
            drpJefeEmple.DataBind();

        }

        protected void FillDDLPuestoEmple()
        {
            StringBuilder lsbQuery = new StringBuilder();

            lsbQuery.Append("SELECT MIN(iCodCatalogo) AS iCodCatalogo, UPPER(vchDescripcion) AS vchDescripcion \r");
            lsbQuery.Append("FROM [VisHistoricos('Puesto','Puestos Empleado','Español')] \r");
            lsbQuery.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
            lsbQuery.Append("AND dtFinVigencia >= GETDATE() \r");
            lsbQuery.Append("GROUP BY vchDescripcion ORDER BY vchDescripcion");

            dtPuestoEmple = DSODataAccess.Execute(lsbQuery.ToString());

            drpPuestoEmple.DataSource = dtPuestoEmple;
            drpPuestoEmple.DataValueField = "iCodCatalogo";
            drpPuestoEmple.DataTextField = "vchDescripcion";
            drpPuestoEmple.DataBind();
        }

        protected void FillDDLTipoExten()
        {
            StringBuilder lsbQuery = new StringBuilder();

            lsbQuery.Append("SELECT iCodCatalogo, vchDescripcion \r");
            lsbQuery.Append("FROM [vishistoricos('TipoRecurso','Tipos de recurso','Español')] \r");
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

        /*RZ.20130718 Se agrega validacion para que no llene los dropdowns de
         extensiones y codigos cuando se trate de alta*/
        protected void FillDDLSitios()
        {
            StringBuilder lsbQuery = new StringBuilder();

            lsbQuery.Append("SELECT iCodCatalogo, vchDescripcion \r");
            lsbQuery.Append("FROM Historicos \r");
            lsbQuery.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
            lsbQuery.Append("and dtFinVigencia >= GETDATE() \r");
            lsbQuery.Append("and iCodMaestro in (select iCodRegistro \r");
            lsbQuery.Append("\t from Maestros \r");
            lsbQuery.Append("\t where iCodEntidad = 23 --Sitios \r");
            lsbQuery.Append("\t ) order by vchDescripcion");

            dtSitios = DSODataAccess.Execute(lsbQuery.ToString());

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

            }

            /*DropDownList para Ubicacion del Empleado*/
            drpSitioEmple.DataSource = dtSitios;
            drpSitioEmple.DataValueField = "iCodCatalogo";
            drpSitioEmple.DataTextField = "vchDescripcion";
            drpSitioEmple.DataBind();


        }

        protected void FillDDLTipoEmple()
        {
            StringBuilder lsbQuery = new StringBuilder();

            lsbQuery.Append("SELECT iCodCatalogo, vchDescripcion \r");
            lsbQuery.Append("FROM [VisHistoricos('TipoEm','Tipo Empleado','Español')] \r");
            lsbQuery.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
            lsbQuery.Append("and dtFinVigencia >= GETDATE() \r");
            lsbQuery.Append("order by vchDescripcion");

            dtTipoEmple = DSODataAccess.Execute(lsbQuery.ToString());

            /*DropDownsList para Tipo de Empleado*/
            drpTipoEmpleado.DataSource = dtTipoEmple;
            drpTipoEmpleado.DataValueField = "iCodCatalogo";
            drpTipoEmpleado.DataTextField = "vchDescripcion";
            drpTipoEmpleado.DataBind();

        }

        protected void FillDDLEmpreOuts()
        {
            StringBuilder lsbQuery = new StringBuilder();

            lsbQuery.Append("SELECT iCodCatalogo, vchDescripcion \r");
            lsbQuery.Append("FROM [VisHistoricos('Proveedor','Proveedor','Español')] \r");
            lsbQuery.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
            lsbQuery.Append("and dtFinVigencia >= GETDATE() \r");
            lsbQuery.Append("order by vchDescripcion");

            dtEmpreEmple = DSODataAccess.Execute(lsbQuery.ToString());

            /*DropDownsList para Empresa OutSource*/
            drpEmpresaEmple.DataSource = dtEmpreEmple;
            drpEmpresaEmple.DataValueField = "iCodCatalogo";
            drpEmpresaEmple.DataTextField = "vchDescripcion";
            drpEmpresaEmple.DataBind();
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
                txtRadioNextelEmple.AutoPostBack = true;
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

                txtRadioNextelEmple.Text = "52*";

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
            //cbeSaveEmple.Enabled = true;
            lbtnDeleteEmple.Enabled = false;
            lbtnDeleteEmple.Visible = false;

            tblEditDeleteEmpleC1.HorizontalAlign = HorizontalAlign.Center;
            //tblEditDeleteEmpleC2.Visible = false;

        }

        /*RZ.20130718 Sirve para ocultar los paneles de inventario, recursos y politicas
        durante la alta del empleado               
         */
        private void OcultarPanelesRecursos()
        {
            pHeaderInventario.Visible = false;
            pHeaderInventario.Enabled = false;
            pDatosInventario.Visible = false;
            pDatosInventario.Enabled = false;
            pHeaderCodAutoExten.Visible = false;
            pHeaderCodAutoExten.Enabled = false;
            pDatosCodAutoExten.Visible = false;
            pDatosCodAutoExten.Enabled = false;
            pPoliticasUso.Visible = false;
            pPoliticasUso.Enabled = false;
            pContentPoliticas.Visible = false;
            pContentPoliticas.Enabled = false;
            pComentarios.Visible = false;
            pComentarios.Enabled = false;
            tblEditCC.Visible = false;
            tblEditCC.Enabled = false;
            imgbPDFExport.Visible = false;
            lbtnRegresarPaginaBusq.Enabled = false;
            lbtnRegresarPaginaBusq.Visible = false;

            //NZ
            pHeaderUsuarios.Visible = false;
            pHeaderUsuarios.Enabled = false;
            pDatosUsuarios.Visible = false;
            pDatosUsuarios.Enabled = false;
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
            //Se retira alta de inventario desde modalpopup
            //btnAgregar.Visible = false;
            //btnAgregar.Enabled = false;
            grvInventario.Columns[7].Visible = false;   //NZ: Aquí hace referencia a ocultar la columna de borrar.       
            //NZ: 20160311 Dejo de existir la Columna Editar para el Inventario. En el Diseño hay un comentario de RZ.20131204  mencionando que se remueve.
            //Como se comento en diseño esa columna, aqui fallaba por que ya no contenia ese número de columnas.
            //grvInventario.Columns[8].Visible = false; 
            tblAddInventario.Visible = false; //20160311 NZ Se oculta contenedor de los controles para agregar elementos al inventario.

            /*Se desabilitan controles de seccion de Codigos*/
            btnAgregarCodAuto.Visible = false;
            btnAgregarCodAuto.Enabled = false;
            //NZ: 20160311 Se reviso en el diseño de la pagina y hay columnas que fueron comentadas. Es decir ya no se incluyeron, y el numero de columnas
            //en esta parte ya no corresponde, por lo tanto marco error. Se hara el cambio con el numero de columnas a como esta en el diseñador.
            //grvCodAuto.Columns[8].Visible = false; 
            //grvCodAuto.Columns[9].Visible = false;
            grvCodAuto.Columns[7].Visible = false; //20160311 Columna Editar
            grvCodAuto.Columns[8].Visible = false; //20160311 Columna Borrar
            tblAltaDeCodigosAut.Visible = false;  //20160311 NZ Se oculta contenedor de los controles para agregar codigos          

            /*Se desabilitan controles de seccion de Extensiones*/
            btnAgregarExten.Visible = false;
            btnAgregarExten.Enabled = false;
            grvExten.Columns[11].Visible = false;
            grvExten.Columns[12].Visible = false;
            tblAltaDeExtensiones.Visible = false;  //20160311 NZ Se oculta contenedor de los controles para agregar extensiones.

            /*NZ Se desabilitan controles de seccion de ID's de Usuario*/
            btnAgregarUsuario.Visible = false;
            btnAgregarUsuario.Enabled = false;
            grvUsuarios.Columns[6].Visible = false;  //Columna Editar
            grvUsuarios.Columns[7].Visible = false;  //Columna Borrar
            tblAltaDeUsuarios.Visible = false;   //20160311 NZ Se oculta contenedor de los controles para agregar id de usuarios.

            /*Se desabilita seccion de comentarios*/
            //tblComentariosC1.Visible = false;
            //tblComentariosC2.Visible = false;
            //txtComentariosAdmin.Visible = false;
            txtComentariosAdmin.Enabled = false;
            //txtComenariosEmple.Visible = false;
            txtComenariosEmple.Enabled = false;

            /*Se desabilita seccion fechas*/
            tblFechasCC.Visible = false;
            tblFechasCC.Enabled = false;

            /*Se desabilitan botones enviar CCust*/
            tblEditCC.Visible = false;
            tblEditCC.Enabled = false;

            /*Se desabilita boton para exportar a PDF*/
            imgbPDFExport.Visible = false;
            imgbPDFExport.Enabled = false;

        }

        private void FillCodAutoGrid()
        {
            psQuery.Length = 0;

            psQuery.Append("SELECT CodAuto = CodAuto.iCodCatalogo, CodAutoCod = CodAuto.vchCodigo, Sitio = CodAuto.Sitio, " + "\r");
            //RZ.20131227 Se retira campo "Visible en Directorio"
            //VisibleDir = CONVERT(bit,ISNULL(CodAuto.BanderasCodAuto,0))
            psQuery.Append("SitioDesc = CodAuto.SitioDesc, FechaIni = Rel.dtinivigencia, FechaFin = Rel.dtFinVigencia \r");
            psQuery.Append(",iCodRegRelEmpCodAuto = Rel.iCodRegistro" + "\r");   //AM 20130717  Se agrega campo para obtener iCodRegistro de la Relacion
            psQuery.Append("FROM [VisRelaciones('Empleado - CodAutorizacion','Español')] Rel" + "\r");
            psQuery.Append("INNER JOIN [VisHistoricos('CodAuto','Codigo Autorizacion','Español')] CodAuto \r");
            psQuery.Append("\t ON Rel.CodAuto = CodAuto.iCodCatalogo" + "\r");
            psQuery.Append("\t AND Rel.dtinivigencia <> Rel.dtfinvigencia \r");
            psQuery.Append("\t AND Rel.dtfinvigencia >= GETDATE() \r");
            psQuery.Append("\t AND CodAuto.dtinivigencia <> CodAuto.dtfinvigencia \r");
            psQuery.Append("\t AND CodAuto.dtfinvigencia >= GETDATE() \r");
            psQuery.Append("WHERE Rel.Emple = " + iCodCatalogoEmple);
            dtCodAuto = DSODataAccess.Execute(psQuery.ToString());

            /*if (dtCodAuto.Rows.Count < 1)
            {
                dtCodAuto.Rows.Add(dtCodAuto.NewRow());
            }*/

            grvCodAuto.DataSource = dtCodAuto;
            grvCodAuto.DataBind();

            upDatosCodAutoExten.Update();
        }

        private void FillExtenGrid()
        {
            psQuery.Length = 0;

            psQuery.Append("SELECT Exten = Exten.iCodCatalogo, ExtenCod = Exten.vchCodigo, Sitio = Exten.Sitio, SitioDesc = Exten.SitioDesc, FechaIni = Rel.dtinivigencia, FechaFin = Rel.dtFinVigencia," + "\r");
            // 20140115 AM. Se agrega condicion a consulta para regresar un 0 cuando el campo ExtenB.TipoRecurso sea null
            //psQuery.Append("TipoExten = ExtenB.TipoRecurso, TipoExtenDesc = isnull(ExtenB.TipoRecursoDesc,0), VisibleDir = CONVERT(bit,ISNULL(Exten.BanderasExtens,0)), ComentarioExten = isnull(ExtenB.Comentarios,'') \r");
            psQuery.Append("TipoExten = ISNULL(ExtenB.TipoRecurso, 0), TipoExtenDesc = isnull(ExtenB.TipoRecursoDesc,0), VisibleDir = CONVERT(bit,ISNULL(Exten.BanderasExtens,0)), ComentarioExten = isnull(ExtenB.Comentarios,'') \r");
            psQuery.Append(",iCodRegRelEmpExt = Rel.iCodRegistro" + "\r");   //AM 20130717  Se agrega campo para obtener iCodRegistro de la Relacion
            psQuery.Append("FROM [VisRelaciones('Empleado - Extension','Español')] Rel" + "\r");
            psQuery.Append("INNER JOIN [VisHistoricos('Exten','Extensiones','Español')] Exten \r");
            psQuery.Append("\t ON Rel.Exten = Exten.iCodCatalogo" + "\r");
            psQuery.Append("\t AND Rel.dtinivigencia <> Rel.dtfinvigencia \r");
            psQuery.Append("\t AND Rel.dtfinvigencia >= GETDATE() \r");
            psQuery.Append("\t AND Exten.dtinivigencia <> Exten.dtfinvigencia \r");
            psQuery.Append("\t AND Exten.dtfinvigencia >= GETDATE() \r");
            psQuery.Append("LEFT OUTER JOIN [VisHistoricos('ExtenB','Extensiones B','Español')] ExtenB \r");
            psQuery.Append("\t ON Exten.iCodCatalogo = ExtenB.Exten" + "\r");
            psQuery.Append("\t AND ExtenB.dtIniVigencia <> ExtenB.dtFinVigencia \r");
            psQuery.Append("\t AND ExtenB.dtFinVigencia >= GETDATE() \r");
            psQuery.Append("WHERE Rel.Emple = " + iCodCatalogoEmple);
            dtExtensiones = DSODataAccess.Execute(psQuery.ToString());

            grvExten.DataSource = dtExtensiones;
            grvExten.DataBind();

            upDatosCodAutoExten.Update();
        }

        private void FillInventarioGrid()
        {
            psQuery.Length = 0;
            //DataRow dr = null;
            //int rowIndex = 0;

            /*
            dtInventarioAsignado.Columns.Add(new DataColumn("iCodMarca", typeof(Int32)));
            dtInventarioAsignado.Columns.Add(new DataColumn("iCodModelo", typeof(Int32)));
            dtInventarioAsignado.Columns.Add(new DataColumn("TipoAparato", typeof(string)));
            dtInventarioAsignado.Columns.Add(new DataColumn("NoSerie", typeof(string)));
            dtInventarioAsignado.Columns.Add(new DataColumn("MacAddress", typeof(string)));
            */

            psQuery.Append("SELECT iCodMarca = Disp.MarcaDisp, Marca = Disp.MarcaDispDesc, iCodModelo = Disp.ModeloDisp, Modelo = Disp.ModeloDíspDesc, " + "\r");
            psQuery.Append("TipoAparato = Disp.TipoDispositivoDesc, NoSerie = Disp.NSerie, MacAddress = Disp.MacAddress \r");
            psQuery.Append("FROM [VisRelaciones('Dispositivo - Empleado','Español')] Rel" + "\r");
            psQuery.Append("INNER JOIN [VisHistoricos('Dispositivo','Inventario de dispositivos','Español')] Disp \r");
            psQuery.Append("\t ON Rel.Dispositivo = Disp.iCodCatalogo" + "\r");
            psQuery.Append("\t AND Rel.dtinivigencia <> Rel.dtfinvigencia \r");
            psQuery.Append("\t AND Rel.dtfinvigencia >= GETDATE() \r");
            psQuery.Append("\t AND Disp.dtinivigencia <> Disp.dtfinvigencia \r");
            psQuery.Append("\t AND Disp.dtfinvigencia >= GETDATE() \r");
            psQuery.Append("WHERE Rel.Emple = " + iCodCatalogoEmple);
            dtInventarioAsignado = DSODataAccess.Execute(psQuery.ToString());

            /*
            dr = dtInventarioAsignado.NewRow();
            dr["iCodMarca"] = int.MinValue;
            dr["iCodModelo"] = int.MinValue;
            dr["TipoAparato"] = string.Empty;
            dr["NoSerie"] = string.Empty;
            dr["MacAddress"] = string.Empty;
            dtInventarioAsignado.Rows.Add(dr);
            */
            grvInventario.DataSource = dtInventarioAsignado;
            grvInventario.DataBind();

            upDatosInventario.Update();

            /*
            if (dtInventarioAsignado != null)
            {
                if (dtInventarioAsignado.Rows.Count > 0)
                {
                    for (int i = 0; i < dtInventarioAsignado.Rows.Count; i++)
                    {
                        TextBox TextBoxMarca = (TextBox)grvInventario.Rows[rowIndex].Cells[0].FindControl("txtMarca");
                        TextBox TextBoxModelo = (TextBox)grvInventario.Rows[rowIndex].Cells[1].FindControl("txtModelo");
                        TextBox TextBoxTipoAparato = (TextBox)grvInventario.Rows[rowIndex].Cells[2].FindControl("txtTipoAparato");
                        TextBox TextBoxNoSerie = (TextBox)grvInventario.Rows[rowIndex].Cells[3].FindControl("txtNoSerie");
                        TextBox TextBoxMacAddress = (TextBox)grvInventario.Rows[rowIndex].Cells[4].FindControl("txtMacAddress");

                        TextBoxMarca.Text = dtInventarioAsignado.Rows[i]["iCodMarca"].ToString();
                        TextBoxModelo.Text = dtInventarioAsignado.Rows[i]["iCodModelo"].ToString();
                        TextBoxTipoAparato.Text = dtInventarioAsignado.Rows[i]["TipoAparato"].ToString();
                        TextBoxNoSerie.Text = dtInventarioAsignado.Rows[i]["NoSerie"].ToString();
                        TextBoxMacAddress.Text = dtInventarioAsignado.Rows[i]["MacAddress"].ToString();

                        rowIndex++;
                    }
                }
            }
            */

            /*DropDownList drp = (DropDownList)grvInventario.Rows[0].Cells[1].FindControl("drpMarca");
            drp.Focus();

            Button btnAdd = (Button)grvInventario.FooterRow.Cells[5].FindControl("btnAgregar");
            Page.Form.DefaultFocus = btnAdd.ClientID;
            */
        }

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
            drpLocalidadEmple.SelectedValue = ldrEmple["Localidad"].ToString();
            txtEmailEmple.Text = ldrEmple["Email"].ToString();
            txtRadioNextelEmple.Text = ldrEmple["RadioNextel"].ToString();
            txtUsuarRedEmple.Text = ldrEmple["UsuarioRed"].ToString();
            txtNumCelularEmple.Text = ldrEmple["Celular"].ToString();
            if (ldrEmple["Gerente"].ToString() == "1")
            {
                cbEsGerenteEmple.Checked = true;
            }
            if (ldrEmple["VisibleDir"].ToString() == "1")
            {
                cbVisibleDirEmple.Checked = true;
            }
            drpJefeEmple.SelectedValue = ldrEmple["JefeInmediato"].ToString();
            txtEmailJefeEmple.Text = ldrEmple["EmailJefe"].ToString();
            drpSitioEmple.SelectedValue = ldrEmple["Ubicacion"].ToString();
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


        }

        protected string consultaFechaEnvio()
        {
            string lsConsultaCCustodia = "select iCodCatalogo from [VisHistoricos('CCustodia','Cartas custodia','Español')] " +
                               "where FolioCCustodia = " + txtFolioCCustodia.Text.ToString() +
                               "and dtIniVigencia<>dtFinVigencia and dtFinVigencia >= getdate()";

            StringBuilder sbUltimoEnvio = new StringBuilder();
            sbUltimoEnvio.AppendLine("select top 1 FechaEnvio from [VisDetallados('Detall','Bitacora Envio CCustodia','Español')] \r");
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

        //NZ 20150826
        protected void FillUsuariosEmpleGrid()
        {
            psQuery.Length = 0;

            psQuery.Append("SELECT iCodRegUsuario = Ids.iCodCatalogo, IdUsuario = Ids.vchCodigo, Pin = Ids.PinVarchar, " + "\r");
            psQuery.Append("FechaIni = Ids.dtinivigencia, FechaFin = Ids.dtFinVigencia, ComentariosUsuarios = Ids.Comentarios\r");
            psQuery.Append("FROM [VisHistoricos('NxtIDUsuario','Nxt IDs Usuarios','Español')] Ids" + "\r");
            //psQuery.Append("FROM [VisRelaciones('Empleado - ID Usuario','Español')] Rel" + "\r");   No se Requiere Relacion por el momento
            psQuery.Append("WHERE Ids.Emple = " + iCodCatalogoEmple);
            psQuery.Append(" AND Ids.dtinivigencia <> Ids.dtfinvigencia \r");
            psQuery.Append(" AND Ids.dtfinvigencia >= GETDATE() \r");
            dtIdUsuarios = DSODataAccess.Execute(psQuery.ToString());

            grvUsuarios.DataSource = dtIdUsuarios;
            grvUsuarios.DataBind();
            UpDatosUsuarios.Update();

        }

        //RZ.20130722 Se agrega fecha fin para estado edit
        protected DataTable cargaDatosEmple(string iCodCatEmple)
        {
            DataTable ldtEmple = new DataTable();

            psQuery.Length = 0;
            psQuery.Append("SELECT Nombre = Emple.Nombre, ApPaterno = Emple.Paterno, ApMaterno = Emple.Materno, SegundoNombre = EmpleB.SegundoNombre, \r");
            psQuery.Append("FechaInicio = Emple.dtIniVigencia, FechaFin = Emple.dtFinVigencia, NoFolio = CCustodia.FolioCCustodia, TipoEmple = Emple.TipoEm, EmpreEmple = EmpleB.Proveedor, \r");

            //20140829 AM. Se cambia la vista de donde toma el estatus la carta custodia
            //psQuery.Append("Estatus = CCustodia.EstCCustodiaDesc, Empleado = Emple.NominaA, Nombre = Emple.NomCompleto, \r");
            psQuery.Append("Estatus = EstCCust.vchDescripcion, Empleado = Emple.NominaA, Nombre = Emple.NomCompleto, \r");

            psQuery.Append("Ubicacion = Sitio.iCodCatalogo, CenCos = Emple.CenCos, Puesto = Emple.Puesto, \r");
            psQuery.Append("Localidad = EmpleB.Estados, Email = Emple.Email, RadioNextel = CCustodia.NumRadio, \r");
            psQuery.Append("UsuarioRed = Emple.UsuarCod, Celular = CCustodia.NumTelMovil, Gerente = ((isnull(BanderasEmple,0)) & 2) / 2, \r");
            psQuery.Append("ComentariosEmple = CCustodia.ComentariosEmple, ComentariosAdmin = CCustodia.ComentariosAdmin, FecUltModificacion = CCustodia.dtFecUltAct, \r");
            psQuery.Append("JefeInmediato = Emple.Emple, EmailJefe = EmpleJefe.Email, VisibleDir = ((isnull(BanderasEmple,0)) & 1) / 1\r");
            psQuery.Append("FROM [VisHistoricos('Emple','Empleados','Español')] Emple \r");
            psQuery.Append("INNER JOIN [VisHistoricos('CCustodia','Cartas custodia','Español')] CCustodia \r");
            psQuery.Append("\t ON Emple.iCodCatalogo = CCustodia.Emple \r");
            psQuery.Append("\t and CCustodia.dtIniVigencia <> CCustodia.dtFinVigencia \r");
            psQuery.Append("\t and Emple.dtIniVigencia <> Emple.dtFinVigencia \r");
            psQuery.Append("\t and Emple.dtFinVigencia >= GETDATE() \r");
            psQuery.Append("LEFT OUTER JOIN [VisHistoricos('Empleb','Empleados b','Español')] EmpleB \r");
            psQuery.Append("\t ON Emple.iCodCatalogo = EmpleB.Emple \r");
            psQuery.Append("\t and EmpleB.dtIniVigencia <> EmpleB.dtFinVigencia \r");
            psQuery.Append("\t and EmpleB.dtFinVigencia >= GETDATE() \r");
            psQuery.Append("LEFT OUTER JOIN (SELECT NomCompleto, Email, iCodCatalogo \r");
            psQuery.Append("\t\t\t FROM [VisHistoricos('Emple','Empleados','Español')] \r");
            psQuery.Append("\t\t\t WHERE dtIniVigencia <> dtFinVigencia \r");
            psQuery.Append("\t\t\t and dtFinVigencia >= GETDATE() \r");
            psQuery.Append("\t\t\t ) as EmpleJefe \r");
            psQuery.Append("\tON EmpleJefe.iCodCatalogo = Emple.Emple \r");
            psQuery.Append("LEFT OUTER JOIN (SELECT iCodCatalogo, vchDescripcion \r");
            psQuery.Append("\t\t\t FROM Historicos \r");
            psQuery.Append("\t\t\t WHERE dtIniVigencia <> dtFinVigencia \r");
            psQuery.Append("\t\t\t and dtFinVigencia >= GETDATE() \r");
            psQuery.Append("\t\t\t and iCodMaestro in (select iCodRegistro \r");
            psQuery.Append("\t\t\t\t from Maestros \r");
            psQuery.Append("\t\t\t\t where iCodEntidad = 23 --Sitios \r");
            psQuery.Append("\t\t\t\t ) \r");
            psQuery.Append("\t\t\t ) as Sitio \r");
            psQuery.Append("\t\t ON Sitio.vchDescripcion = Emple.Ubica \r");

            //20140829 AM. Se agrega el join con vista de estatus para sacar la descripcion del estatus en caso de que por vigencias no se muestre en la vista de cartas custodia.
            psQuery.Append("JOIN [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')] EstCCust \r");
            psQuery.Append("on EstCCust.iCodCatalogo = CCustodia.EstCCustodia \r");
            psQuery.Append("and EstCCust.dtIniVigencia <> EstCCust.dtFinVigencia and EstCCust.dtFinVigencia >= GETDATE() \r");

            psQuery.Append(" WHERE Emple.iCodCatalogo = " + iCodCatEmple);

            //20150512.RJ Condicion agregada para que regrese solo una carta, en caso de que el empleado tuviera más
            psQuery.Append(" and CCustodia.icodregistro = (select max(CCust2.icodregistro) ");
            psQuery.Append(" 					from [VisHistoricos('CCustodia','Cartas custodia','Español')] CCust2 ");
            psQuery.Append(" 					where CCust2.Emple = " + iCodCatEmple);
            psQuery.Append(" 					and dtinivigencia<>dtfinvigencia ");
            psQuery.Append(" 					and dtfinvigencia = (select max(dtfinvigencia) ");
            psQuery.Append(" 											from [VisHistoricos('CCustodia','Cartas custodia','Español')] CCust2 ");
            psQuery.Append(" 											where CCust2.Emple = " + iCodCatEmple);
            psQuery.Append(" 											and dtinivigencia<>dtfinvigencia ");
            psQuery.Append(" 										) ");
            psQuery.Append(" 					) ");


            ldtEmple = DSODataAccess.Execute(psQuery.ToString());

            return ldtEmple;
        }


        #region Baja Empleado
        /*20130718 Se agrega event handler de delete emple*/
        protected void lbtnDeleteEmple_Click(object sender, EventArgs e)
        {
            /*Validar si el empleado tiene o no otros empleados a cargo.*/


            validarBajaEmple();
            mpeBajaEmple.Show();
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

            lsbConsulta.Append("SELECT isNull(COUNT(*),0) FROM Historicos \r");
            lsbConsulta.Append("WHERE iCodMaestro in ( \r");
            lsbConsulta.Append("\t select iCodRegistro \r");
            lsbConsulta.Append("\t from Maestros WHERE icodentidad in ( \r");
            lsbConsulta.Append("\t\t SELECT iCodRegistro \r");
            lsbConsulta.Append("\t\t FROM Catalogos \r");
            lsbConsulta.Append("\t\t WHERE vchCodigo like 'Emple' \r");
            lsbConsulta.Append("\t\t and iCodCatalogo is null) \r");
            lsbConsulta.Append("\t and vchDescripcion like 'Empleados') \r");
            lsbConsulta.Append("and dtIniVigencia <> dtFinVigencia \r");
            lsbConsulta.Append("and dtFinVigencia >= GETDATE() \r");
            lsbConsulta.Append("and iCodCatalogo04 = " + iCodCatEmple + " \r"); //Empleado Responsable

            return (int)DSODataAccess.ExecuteScalar(lsbConsulta.ToString());
        }


        //RZ.20130725 Se agrega metodo que crea el contenido de la modalpopup
        protected void mostrarModalPopBajaEmple()
        {
            pnlBajaEmple.Height = 150;
            StringBuilder lsbArmarMensajeEmpleBaja = new StringBuilder();

            lsbArmarMensajeEmpleBaja.Append("<p>Seleccione la fecha de baja para el empleado ");
            lsbArmarMensajeEmpleBaja.Append("<strong>" + armarNomCompleto() + "</strong>.");
            lsbArmarMensajeEmpleBaja.Append("</p><br />");

            lcEmpleEnBajaMsj.Text = lsbArmarMensajeEmpleBaja.ToString();

            mpeBajaEmple.Show();
        }


        protected void mostrarModalPopReasignaEmpleados(int cantEmple)
        {
            //LiteralControl lcEmpleEnBaja = new LiteralControl();

            StringBuilder lsbArmarMensajeEmpleBaja = new StringBuilder();

            lsbArmarMensajeEmpleBaja.Append("<p>El empleado ");
            lsbArmarMensajeEmpleBaja.Append("<strong>" + armarNomCompleto() + "</strong>");
            lsbArmarMensajeEmpleBaja.Append(" aún no puede ser borrado ya que cuenta con ");
            lsbArmarMensajeEmpleBaja.Append("<strong> " + cantEmple.ToString() + "</strong> empleados bajo su cargo: ");
            lsbArmarMensajeEmpleBaja.Append("</p><br />");

            //lcEmpleEnBaja.Text += lsbArmarMensajeEmpleBaja.ToString();
            lcEmpleReasigna.Text = lsbArmarMensajeEmpleBaja.ToString();

            //pnlBajaEmple.Controls.Add(lcEmpleEnBaja);

            //Panel pnlGridView = new Panel();
            //pnlGridView.ID = "pnlGridView";
            //pnlGridView.HorizontalAlign = HorizontalAlign.Center;
            pnlGridView.Style.Add("width", "100%");
            pnlGridView.Style.Add("height", "400px");
            pnlGridView.Style.Add("overflow", "scroll");
            //pnlBajaEmple.Controls.Add(pnlGridView);

            //GridView grvEmpleDepende = new GridView();
            //grvEmpleDepende.ID = "grvEmpleDepende";
            //BoundField bfNoNomina = new BoundField();
            //bfNoNomina.HeaderText = "No. nómina";
            //bfNoNomina.DataField = "NominaA";
            //grvEmpleDepende.Columns.Add(bfNoNomina);

            //BoundField bfNomEmple = new BoundField();
            //bfNomEmple.HeaderText = "Nombre";
            //bfNomEmple.DataField = "NomCompleto";
            //grvEmpleDepende.Columns.Add(bfNomEmple);

            //BoundField bfPuestoEmple = new BoundField();
            //bfPuestoEmple.HeaderText = "Puesto";
            //bfPuestoEmple.DataField = "PuestoDesc";
            //grvEmpleDepende.Columns.Add(bfPuestoEmple);

            //grvEmpleDepende.AutoGenerateColumns = false;
            grvEmpleDepende.CssClass = "GridViewPopUp";
            grvEmpleDepende.RowStyle.CssClass = "GridRowOdd";
            grvEmpleDepende.AlternatingRowStyle.CssClass = "GridRowEven";
            grvEmpleDepende.Height = 300;
            grvEmpleDepende.HorizontalAlign = HorizontalAlign.Center;
            grvEmpleDepende.DataSource = getEmpleDependientes(iCodCatalogoEmple);
            grvEmpleDepende.DataBind();
            pnlReasignaEmple.Height = 600;
            pnlReasignaEmple.Width = 600;

            pnlGridView.Controls.Add(grvEmpleDepende);

            //LiteralControl lcAvisoNuevoJefe = new LiteralControl();
            lsbArmarMensajeEmpleBaja.Length = 0;
            lsbArmarMensajeEmpleBaja.Append("<p>Para eliminar al empleado, ");
            lsbArmarMensajeEmpleBaja.Append("se requiere que asigne un nuevo jefe ");
            lsbArmarMensajeEmpleBaja.Append("a los empleados mencionados.");
            lsbArmarMensajeEmpleBaja.Append("</p>");

            lcEmpleNuevoJefe.Text = lsbArmarMensajeEmpleBaja.ToString();
            //pnlBajaEmple.Controls.Add(lcAvisoNuevoJefe);

            //pnlBotonesReasignaEmple.Visible = true;
            //pnlBajaEmple.Controls.Add(pnlBotonesReasignaEmple);

            mpeReasingarBajaEmple.Show();
        }


        protected DataTable fillNuevoEmpleResp(string iCodCatEmple)
        {

            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.Append("SELECT iCodCatalogo, vchDescripcion = NomCompleto \r");
            lsbQuery.Append("FROM [VisHistoricos('Emple','Empleados','Español')] \r");
            lsbQuery.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
            lsbQuery.Append("and dtFinVigencia >= GETDATE() \r");
            lsbQuery.Append("and iCodCatalogo <> " + iCodCatEmple + " \r");
            lsbQuery.Append("and TipoEm = 444 \r"); //Solo Tipo Empleado
            lsbQuery.Append("order by NomCompleto");

            return DSODataAccess.Execute(lsbQuery.ToString());
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

                HttpContext.Current.Response.Redirect("~/UserInterface/CCustodia/BusquedaCartasCustodia.aspx?ne=" + iCodCatalogoEmple);
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

            /*Se habilitan botones para agregar CenCos y Puesto*/
            lbtnAgregarCenCos.Visible = true;
            lbtnAgregarCenCos.Enabled = true;

            lbtnAgregarPuesto.Visible = true;
            lbtnAgregarPuesto.Enabled = true;
        }

        //20130721
        protected void lbtnSaveEmple_Click(object sender, EventArgs e)
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


        /*RZ.20130722*/
        protected string GrabarEmpleado()
        {
            DALCCustodia dalCC = new DALCCustodia();

            Hashtable lhtEmpleB = new Hashtable();

            //Elementos para insert en CCustodia
            Hashtable lhtCCust = new Hashtable();
            string liFolioCCust = DSODataAccess.ExecuteScalar("select (MAX(FolioCCustodia)+1) " +
                                                              "from [VisHistoricos('CCustodia','Cartas custodia','Español')]").ToString();

            string iCodEstatusCCust = DSODataAccess.ExecuteScalar("select iCodCatalogo from [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')]" +
                                                                  "where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate() and Value = 1").ToString();



            string lsNomCompleto = phtValuesEmple["{NomCompleto}"].ToString();




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
                if (!String.IsNullOrEmpty(drpLocalidadEmple.SelectedValue))
                {
                    lhtEmpleB.Add("{Estados}", drpLocalidadEmple.SelectedValue);
                }
                if (!String.IsNullOrEmpty(drpEmpresaEmple.SelectedValue))
                {
                    lhtEmpleB.Add("{Proveedor}", drpEmpresaEmple.SelectedValue);
                }
                lhtEmpleB.Add("{SegundoNombre}", txtSegundoNombreEmple.Text.Trim());
                lhtEmpleB.Add("dtIniVigencia", Convert.ToDateTime(phtValuesEmple["dtIniVigencia"]));
                lhtEmpleB.Add("iCodUsuario", HttpContext.Current.Session["iCodUsuario"]);
                lhtEmpleB.Add("dtFecUltAct", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                dalCC.AltaEmpleB(lhtEmpleB);

                lhtCCust.Clear();
                lhtCCust.Add("iCodMaestro", 200064);
                lhtCCust.Add("vchCodigo", "CCustodia " + liFolioCCust);
                lhtCCust.Add("vchDescripcion", lsNomCompleto + " (Folio:" + liFolioCCust + ")");
                lhtCCust.Add("{Emple}", iCodEmple);
                lhtCCust.Add("{EstCCustodia}", iCodEstatusCCust);
                lhtCCust.Add("{FolioCCustodia}", liFolioCCust);
                lhtCCust.Add("{FechaCreacion}", Convert.ToDateTime(txtFecha.Text).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                lhtCCust.Add("{FechaResp}", null);
                lhtCCust.Add("{FechaCancelacion}", null);
                lhtCCust.Add("{NumTelMovil}", txtNumCelularEmple.Text.ToString());
                lhtCCust.Add("{NumRadio}", txtRadioNextelEmple.Text.ToString());
                lhtCCust.Add("{ComentariosEmple}", null);
                lhtCCust.Add("{ComentariosAdmin}", null);
                lhtCCust.Add("dtIniVigencia", Convert.ToDateTime(txtFecha.Text));
                lhtCCust.Add("dtFinVigencia", new DateTime(2079, 1, 1));
                lhtCCust.Add("iCodUsuario", HttpContext.Current.Session["iCodUsuario"]);
                lhtCCust.Add("dtFecUltAct", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                dalCC.AltaCCustodia(lhtCCust);

                //Insert de la relacion del empleado con el CC seleccionado
                dalCC.AltaRelEmpleCenCos(iCodEmple, phtValuesEmple["{CenCos}"].ToString(), Convert.ToDateTime(phtValuesEmple["dtIniVigencia"]), Convert.ToDateTime(phtValuesEmple["dtFinVigencia"]));

                //Mandar a actualizar la jerarquia y restricciones del empleado creado
                ActualizaJerarquiaRest(iCodEmple);

            }

            Util.LogMessage("RZ. El empleado se ha dado de alta con el siguiente catalogo: " + iCodEmple);

            return iCodEmple;
        }

        protected void EditarEmpleado()
        {
            DALCCustodia dalcc = new DALCCustodia();
            Hashtable lhtEmpleB = new Hashtable();
            Hashtable lhtCCust = new Hashtable();

            string lsNomCompleto = phtValuesEmple["{NomCompleto}"].ToString();

            /*RZ.20130808 Validar si el CenCos elegido cambio y no se trata del mismo 
                 que se tenia, de ser asi entonces proceder con la baja de la relacion del CC
                 y agregar la nueva con el CC*/
            validaCambioCenCosEmple(phtValuesEmple["{CenCos}"].ToString());

            bool lbActualizaEmple = dalcc.ActualizaEmple(phtValuesEmple, iCodCatalogoEmple);

            if (lbActualizaEmple)
            {
                lhtEmpleB.Clear();
                lhtEmpleB.Add("vchCodigo", phtValuesEmple["vchCodigo"].ToString() + " (B)");
                lhtEmpleB.Add("vchDescripcion", lsNomCompleto + " (B)");
                lhtEmpleB.Add("{Emple}", iCodCatalogoEmple);

                if (!String.IsNullOrEmpty(drpLocalidadEmple.SelectedValue))
                {
                    lhtEmpleB.Add("{Estados}", drpLocalidadEmple.SelectedValue);
                }

                if (!String.IsNullOrEmpty(drpEmpresaEmple.SelectedValue))
                {
                    lhtEmpleB.Add("{Proveedor}", drpEmpresaEmple.SelectedValue);
                }

                lhtEmpleB.Add("{SegundoNombre}", txtSegundoNombreEmple.Text.Trim());
                lhtEmpleB.Add("dtIniVigencia", Convert.ToDateTime(phtValuesEmple["dtIniVigencia"]));
                lhtEmpleB.Add("iCodUsuario", HttpContext.Current.Session["iCodUsuario"]);
                lhtEmpleB.Add("dtFecUltAct", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                dalcc.ActualizaEmpleB(lhtEmpleB, DALCCustodia.getiCodCatHist(iCodCatalogoEmple, "EmpleB", "Empleados B", "Emple", "iCodCatalogo"));

                lhtCCust.Clear();
                lhtCCust.Add("iCodMaestro", DALCCustodia.getiCodMaestro("Cartas custodia", "CCustodia"));
                lhtCCust.Add("vchCodigo", "CCustodia " + txtFolioCCustodia.Text.Trim());
                lhtCCust.Add("vchDescripcion", lsNomCompleto + " (Folio:" + txtFolioCCustodia.Text.Trim() + ")");
                lhtCCust.Add("{Emple}", iCodCatalogoEmple);
                //lhtCCust.Add("{EstCCustodia}", iCodEstatusCCust);
                //lhtCCust.Add("{FolioCCustodia}", txtFolioCCustodia.Text.Trim());
                //lhtCCust.Add("{FechaCreacion}", Convert.ToDateTime(txtFecha.Text).ToString("yyyy-MM-dd HH:mm:ss.fff"));
                //lhtCCust.Add("{FechaResp}", null);
                //lhtCCust.Add("{FechaCancelacion}", null);
                lhtCCust.Add("{NumTelMovil}", txtNumCelularEmple.Text.Trim());
                lhtCCust.Add("{NumRadio}", txtRadioNextelEmple.Text.Trim());
                //lhtCCust.Add("{ComentariosEmple}", null);
                lhtCCust.Add("{ComentariosAdmin}", txtComentariosAdmin.Text.Trim());
                lhtCCust.Add("dtIniVigencia", Convert.ToDateTime(phtValuesEmple["dtIniVigencia"]));
                lhtCCust.Add("dtFinVigencia", Convert.ToDateTime(phtValuesEmple["dtFinVigencia"]));
                lhtCCust.Add("iCodUsuario", HttpContext.Current.Session["iCodUsuario"]);
                lhtCCust.Add("dtFecUltAct", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                dalcc.ActualizaCCustodia(lhtCCust, DALCCustodia.getiCodCatHist(iCodCatalogoEmple, "CCustodia", "Cartas custodia", "Emple", "iCodCatalogo"));

                //Insert de la relacion del empleado con el CC seleccionado
                //dalcc.AltaRelEmpleCenCos(iCodEmple, phtValuesEmple["{CenCos}"].ToString(), Convert.ToDateTime(phtValuesEmple["dtIniVigencia"]), Convert.ToDateTime(phtValuesEmple["dtFinVigencia"]));

                //Mandar a actualizar la jerarquia y restricciones del empleado
                ActualizaJerarquiaRest(iCodCatalogoEmple);
            }
        }

        protected void validaCambioCenCosEmple(string iCodCatCenCosActual)
        {
            DALCCustodia cambiosCC = new DALCCustodia();

            /*Buscar la relacion actual que tiene el empleado con CC para saber si es la misma*/
            StringBuilder lsbQuery = new StringBuilder();

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
                HttpContext.Current.Response.Redirect("~/UserInterface/CCustodia/DashboardTop10CCustodia.aspx");
            }

            HttpContext.Current.Response.Redirect("~/UserInterface/CCustodia/BusquedaCartasCustodia.aspx?stCC=2");
        }

        protected void lbtnRegresarPagBusqExternaCCust_Click(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Redirect("~/BusquedaExternaCCustodia/BusquedaExternaCCustodia.aspx");
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


            DateTime.TryParse(txtFecha.Text.ToString(), out ldtFechaInicio);

            //lht.Add("vchCodigo", txtNominaEmple.Text);
            //lht.Add("vchDescripcion", primerNombre + " " + segundoNombre + " " + apPaterno + " " + apMaterno + "(" + DSODataContext.Schema + "Empre)");
            if (!String.IsNullOrEmpty(drpCenCosEmple.SelectedValue))
            {
                lht.Add("{CenCos}", int.Parse(drpCenCosEmple.SelectedValue));  //iCodCatalogo01
            }

            if (!String.IsNullOrEmpty(drpTipoEmpleado.SelectedValue))
            {
                lht.Add("{TipoEm}", drpTipoEmpleado.SelectedValue); //iCodCatalogo02
            }

            if (!String.IsNullOrEmpty(drpPuestoEmple.SelectedValue))
            {
                lht.Add("{Puesto}", drpPuestoEmple.SelectedValue);  //iCodCatalogo03
            }

            if (!String.IsNullOrEmpty(drpJefeEmple.SelectedValue))
            {
                lht.Add("{Emple}", drpJefeEmple.SelectedValue);
            }

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
                getValBandera("EmpleVisibleEnDirect", ref liBanderasEmple);

            }

            if (cbEsGerenteEmple.Checked == true)
            {
                getValBandera("EmpleEsGerente", ref liBanderasEmple);

            }

            lht.Add("{BanderasEmple}", liBanderasEmple);
            lht.Add("{PresupFijo}", "null");
            lht.Add("{PresupProv}", "null");
            lht.Add("{Nombre}", txtNombreEmple.Text);  //VarChar01
            lht.Add("{Paterno}", txtApPaternoEmple.Text); //VarChar02
            lht.Add("{Materno}", txtApMaternoEmple.Text); //VarChar03
            lht.Add("{RFC}", "null");
            lht.Add("{Email}", txtEmailEmple.Text);


            if (!String.IsNullOrEmpty(drpSitioEmple.SelectedValue))
            {
                lht.Add("{Ubica}", drpSitioEmple.SelectedItem.Text);
            }

            if (txtNominaEmple.Text != string.Empty)
            {
                lht.Add("{NominaA}", txtNominaEmple.Text); //VarChar07
            }
            else
            {
                lht.Add("{NominaA}", String.Empty);
            }

            //lht.Add("{NomCompleto}", txtNombreEmple.Text + " " + txtSegundoNombreEmple.Text + " " + txtApPaternoEmple.Text + " " + txtApMaternoEmple.Text);
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

        protected void getValBandera(string lsCodBandera, ref int liValBanderas)
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
            StringBuilder lsbQuery = new StringBuilder();

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

            //ldt = pKDB.GetHisRegByEnt("TipoEm", "Tipo Empleado", " iCodCatalogo = " + liCodCatalogo);
            ldt = pKDB.GetCatRegByEnt("TipoEm");
            ldr = ldt.Select("iCodRegistro = " + liCodCatalogo);

            //Si no Existe el tipo de Emplaedo
            if (ldr.Length == 0 || (ldr[0]["vchCodigo"] is DBNull))
            {
                return lsNomina;
            }

            //lsTipoEm = ldt.Rows[0]["vchCodigo"].ToString();
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
            string iCodMaestro = "7";

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
            string iCodMaestro = "7";

            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            DataRow lRowMaestro = DSODataAccess.ExecuteDataRow("select iCodRegistro from Maestros where iCodRegistro = " + iCodMaestro);

            string lsError;
            string lsTitulo = DSOControl.JScriptEncode("Empleados");
            //bool lbRequerido;

            try
            {
                if (!phtValuesEmple.ContainsKey("{CenCos}"))
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RelacionRequerida", "Centro de Costos"));
                    lsbErrores.Append("<li>" + lsError + "</li>");
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

                if (String.IsNullOrEmpty(phtValuesEmple["{Nombre}"].ToString()))
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Nombre del Empleado"));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                if (String.IsNullOrEmpty(phtValuesEmple["{NominaA}"].ToString()))
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Nómina del Empleado"));
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
            int liCodCatalogo = int.Parse(phtValuesEmple["{CenCos}"].ToString());

            psbQuery.Length = 0;
            psbQuery.AppendLine("Select Empre");
            psbQuery.AppendLine("from [VisHistoricos('CenCos','" + Globals.GetCurrentLanguage() + "')]");
            psbQuery.AppendLine("where iCodCatalogo = " + liCodCatalogo.ToString());

            ldt = DSODataAccess.Execute(psbQuery.ToString());

            if (ldt == null || ldt.Rows.Count == 0 || ldt.Rows[0]["Empre"] is DBNull)
            {
                lsbErrores.Append("<li>" + "No se ha encontrado una empresa para el centro de costo. Elija un centro de costo válido" + "</li>");

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

            lsNomEmpleado = lsNomEmpleado.Replace("  ", " "); //Retirar espacios dobles entre el nombre completo

            phtValuesEmple.Add("{NomCompleto}", lsNomEmpleado.Trim());

            //AsignarEntidadActual("CentroCosto-Empleado", vchCodEntidad);

            //Obten el Tipo de Presupuesto si no se capturo
            //lsValue = getValCampo("TipoPr", 0).ToString();
            //if (lsValue == "0")
            //{
            //    lsValue = ObtenTipoPresupuesto();
            //    if (lsValue != "null")
            //    {
            //        setValCampo("TipoPr", lsValue, true);
            //    }
            //}

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
            lsValue = txtSegundoNombreEmple.Text.ToString();
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
                if (!System.Text.RegularExpressions.Regex.IsMatch(lsValue, "^(([\\w-]+\\.)+[\\w-]+|([a-zA-Z]{1}|[\\w-]{2,}))" + "@" +
                    "((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\\.+([0-1]?[0-9]{1,2}|25[0-5]" +
                    "|2[0-4][0-9])\\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z]+[\\w-]+\\.)+[a-zA-Z]{2,4})$"))
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
                if (IsEmpleado() && !IsRespEmpleado())
                {
                    lsError = GetMsgError("Jefe Inmediato", "ValJefeEmpleado");
                    lsbErrores.Append("<li>" + lsError);
                }
                // Es externo debe asignarsele un responsable que sea empleado o externo
                if (IsExterno() && !IsRespEmpleadoExterno())
                {
                    lsError = GetMsgError("Jefe Inmediato", "ValJefeEmplExt");
                    lsbErrores.Append("<li>" + lsError);
                }
            }
            else
            {
                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Jefe Inmediato"));
                lsbErrores.Append("<li>" + lsError + "</li>");
            }

            //Validar que el usuario no este asignado a otro empleados
            lsError = UsuarioAsignado();

            if (lsError.Length > 0)
            {
                lsError = GetMsgError("Usuario", lsError);
                lsbErrores.Append("<li>" + lsError);
            }

            //int liOpc = int.Parse(getValCampo("OpcCreaUsuar", 0).ToString());
            //&& liOpc != 0

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

            /*Validaciones que aplican para Nextel*/

            //Validar que la longitud del campo nomina no exceda los 8 caracteres
            if (phtValuesEmple["{NominaA}"].ToString().Length > 8)
            {
                lsError = GetMsgError("Nómina", "ValLongCampo");
                lsbErrores.Append("<li>" + lsError);
            }

            //Validar que el campo del celular se ingresen solo numeros
            if (txtNumCelularEmple.Text.Length > 0)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(txtNumCelularEmple.Text, @"^[0-9]+$"))
                {
                    lsError = GetMsgError("Número Celular", "ValEmplFormato");
                    lsbErrores.Append("<li>" + lsError);
                }
            }

            lsValue = phtValuesEmple["{Email}"].ToString();
            if (lsValue.Length == 0)
            {
                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "E-mail"));
                lsbErrores.Append("<li>" + lsError + "</li>");
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
            StringBuilder lsbQuery = new StringBuilder();
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
            StringBuilder lsbQuery = new StringBuilder();
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
            StringBuilder lsbQuery = new StringBuilder();
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
            StringBuilder lsbQuery = new StringBuilder();
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
            StringBuilder lsbQuery = new StringBuilder();
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

            //RZ.20131201 Se retira llamada al metodo que valida si el empleado es tipo empleado para poderle asignar el usuario
            //if (!IsEmpleado())
            //{
            //    return "ValUsuarioEmpleado";
            //}

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

            int liCodMaestro = int.Parse(DSODataAccess.ExecuteScalar("select iCodRegistro from Maestros where vchDescripcion = 'Usuarios' and iCodEntidad = (Select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'Usuar') and dtIniVigencia <> dtFinVigencia").ToString()); ;

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
            lhtValues.Add("{HomePage}", "'~/UserInterface/Dashboard/Dashboard.aspx?Opc=OpcdshEmpleado'");


            /*lhtValues.Add("{HomePage}", "'~/UserInterface/CCustodia/BridgeAppCCustodiaFromEmail.aspx'");*/

            /*RZ.20131106 Se deja como perfil default el tipo empleado */
            ldt = pKDB.GetHisRegByEnt("Perfil", "Perfiles", "vchCodigo ='Epmpl' ");
            if (ldt != null && !(ldt.Rows[0]["iCodCatalogo"] is DBNull))
            {
                liCodPerfil = (int)ldt.Rows[0]["iCodCatalogo"];
                lhtValues.Add("{Perfil}", liCodPerfil);
            }

            /*ldt = pKDB.GetHisRegByEnt("Perfil", "Perfiles", "vchCodigo ='PerfCCustodia' ");
            if (ldt != null && !(ldt.Rows[0]["iCodCatalogo"] is DBNull))
            {
                liCodPerfil = (int)ldt.Rows[0]["iCodCatalogo"];
                lhtValues.Add("{Perfil}", liCodPerfil);
            }
            */

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
                string usuario = "UsuarioNxt";

                StringBuilder lsbConsulta = new StringBuilder();
                //20141224 AM. Se cambia la consulta para conseguir el numero consecutivo de usuarios
                //lsbConsulta.Append(" select isnull(Substring(MAX(vchCodigo),11,LEN(MAX(vchCodigo))),0) \r");
                //lsbConsulta.Append(" from [VisHistoricos('Usuar','Usuarios','Español')] \r");
                //lsbConsulta.Append(" where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate() \r");
                //lsbConsulta.Append(" and (vchCodigo like 'UsuarioNxt[0-9]' \r");
                //lsbConsulta.Append(" or vchCodigo like 'UsuarioNxt[0-9][0-9]' \r");
                //lsbConsulta.Append(" or vchCodigo like 'UsuarioNxt[0-9][0-9][0-9]' \r");
                //lsbConsulta.Append(" or vchCodigo like 'UsuarioNxt[0-9][0-9][0-9][0-9]' \r");
                //lsbConsulta.Append(" or vchCodigo like 'UsuarioNxt[0-9][0-9][0-9][0-9][0-9]') \r");

                lsbConsulta.Append(" select Max(Numero) as Numero from ( \r");
                lsbConsulta.Append(" select convert(int, REPLACE(vchCodigo,'UsuarioNxt','')) as Numero \r");
                lsbConsulta.Append(" from [VisHistoricos('Usuar','Usuarios','Español')] \r");
                lsbConsulta.Append(" where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate() \r");
                lsbConsulta.Append(" and (vchCodigo like 'UsuarioNxt[0-9]' \r");
                lsbConsulta.Append(" or vchCodigo like 'UsuarioNxt[0-9][0-9]' \r");
                lsbConsulta.Append(" or vchCodigo like 'UsuarioNxt[0-9][0-9][0-9]' \r");
                lsbConsulta.Append(" or vchCodigo like 'UsuarioNxt[0-9][0-9][0-9][0-9]' \r");
                lsbConsulta.Append(" or vchCodigo like 'UsuarioNxt[0-9][0-9][0-9][0-9][0-9]') \r");
                lsbConsulta.Append(" ) as Rep \r");

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

        /*
        protected bool ValidarAtribCatalogosVig()
        {
            //Valida la existencia de todos los catalogos para la vigencia del registro historico
            //se asume que ya se mando llamar ValidarCampos, ValidarVigencias
            //no se validan los campos de los catalogos que esten en null
            //se asume que no hay empalmes de vigencias para los registros de historicos con un mismo iCodCatalogo

            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            StringBuilder lsbErrorCampo = new StringBuilder();
            string lsError;
            string lsTitulo = DSOControl.JScriptEncode("Empleados");

            try
            {

                DataTable lDataTableHis;
                StringBuilder lsbQueryHis = new StringBuilder();
                DataRow[] ladrHis;
                string lsFiltro;
                string lsDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");

                lsbQueryHis.Length = 0;
                lsbQueryHis.AppendLine("select");
                lsbQueryHis.AppendLine("    H.iCodCatalogo,");
                lsbQueryHis.AppendLine("    H.dtIniVigencia,");
                lsbQueryHis.AppendLine("    H.dtFinVigencia");
                lsbQueryHis.AppendLine("from Historicos H");
                lsbQueryHis.AppendLine("where H.dtIniVigencia <> H.dtFinVigencia");
                lsbQueryHis.Append("and H.iCodCatalogo in(0");

                foreach (KeytiaBaseField lField in pFields)
                {
                    if (lField.Column.StartsWith("iCodCatalogo")
                        && lField.DSOControlDB.HasValue
                        && (lField.ConfigValue.ToString() != iCodEntidad
                        || lField.DataValue.ToString() != iCodCatalogo))
                    {





                        lsbQueryHis.Append("," + lField.DataValue);
                    }
                }


                lsbQueryHis.AppendLine(")");
                lsbQueryHis.AppendLine(DSOControl.ComplementaVigenciasJS(pdtIniVigencia.Date, pdtFinVigencia.Date, false, "H."));
                lsbQueryHis.AppendLine("and H.iCodCatalogo <> isnull(" + iCodCatalogo + ", 0)");
                lsbQueryHis.AppendLine("order by iCodCatalogo, dtIniVigencia");


                lDataTableHis = DSODataAccess.Execute(lsbQueryHis.ToString());


                foreach (KeytiaBaseField lField in pFields)
                {
                    if (lField.Column.StartsWith("iCodCatalogo")
                        && lField.DSOControlDB.HasValue
                        && (lField.ConfigValue.ToString() != iCodEntidad
                        || lField.DataValue.ToString() != iCodCatalogo))
                    {

                        lsbErrorCampo.Length = 0;
                        lsFiltro = lField.DataValue.ToString();

                        ladrHis = lDataTableHis.Select("iCodCatalogo = " + lsFiltro, "dtIniVigencia ASC");
                        if (ladrHis.Length == 0)
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", pdtIniVigencia.Date.ToString(lsDateFormat), pdtFinVigencia.Date.ToString(lsDateFormat)));
                            lsbErrorCampo.Append("<li>" + lsError + "</li>");
                        }
                        else if ((DateTime)ladrHis[0]["dtIniVigencia"] > pdtIniVigencia.Date)
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", pdtIniVigencia.Date.ToString(lsDateFormat), ((DateTime)ladrHis[0]["dtIniVigencia"]).ToString(lsDateFormat)));
                            lsbErrorCampo.Append("<li>" + lsError + "</li>");
                        }

                        for (int lidx = 0; lidx < ladrHis.Length; lidx++)
                        {
                            if (lidx > 0 && (DateTime)ladrHis[lidx]["dtIniVigencia"] > (DateTime)ladrHis[lidx - 1]["dtFinVigencia"])
                            {




                                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", ((DateTime)ladrHis[lidx - 1]["dtFinVigencia"]).ToString(lsDateFormat), ((DateTime)ladrHis[lidx]["dtIniVigencia"]).ToString(lsDateFormat)));
                                lsbErrorCampo.Append("<li>" + lsError + "</li>");
                            }
                        }


                        if (ladrHis.Length > 0 && pdtFinVigencia.Date > (DateTime)ladrHis[ladrHis.Length - 1]["dtFinVigencia"])
                        {




                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", ((DateTime)ladrHis[ladrHis.Length - 1]["dtFinVigencia"]).ToString(lsDateFormat), pdtFinVigencia.Date.ToString(lsDateFormat)));
                            lsbErrorCampo.Append("<li>" + lsError + "</li>");
                        }

                        if (lsbErrorCampo.Length > 0)
                        {
                            lsError = Globals.GetMsgWeb(false, "ValidarHisCatVig", lField.Descripcion);
                            lsError = "<span>" + DSOControl.JScriptEncode(lsError) + "</span>";
                            lsbErrores.Append("<li>" + lsError);
                            lsbErrores.Append("<ul>" + lsbErrorCampo.ToString() + "</ul>");
                            lsbErrores.Append("</li>");
                        }
                    }
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
        */

        #endregion

        #region Extensiones

        //Agregar filas al grid de Extensiones 
        protected void btnAgregarExten_Click(object sender, EventArgs e)
        {
            btnGuardarExten.Enabled = true;
            cargaPropControlesAlAgregarExten();
            mpeExten.Show();
            limpiaCamposDePopUpExten();
        }

        protected void btnGuardar_PopUpExten(object sender, EventArgs e)
        {
            btnGuardarExten.Enabled = false;

            try
            {
                string tipoABC = string.Empty;
                //Si se llega al pop-up mediante dar clic en el boton de edicion entonces llamara al proceso de edicion 
                if (cbEditarExtension.Checked == true)
                {
                    if (validaFormatoFecha(txtFechaInicio.Text.ToString()))
                    {
                        if (validaFormatoFecha(txtFechaFinExten.Text.ToString()))
                        {
                            edicionDeExtension();
                            tipoABC = "C";

                        }

                        else
                        {
                            mensajeDeAdvertencia("Formato de Fecha Fin incorrecto, favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA");
                            txtFechaFinExten.Text = "";
                            txtFechaFinExten.Focus();
                            mpeExten.Show();
                        }
                    }

                    else
                    {
                        mensajeDeAdvertencia("Formato de Fecha Inicio incorrecto, favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA");
                        txtFechaInicio.Text = "";
                        txtFechaInicio.Focus();
                        mpeExten.Show();
                    }
                }

                //Si se llega al pop-up mediante dar clic en el boton de baja entonces llamara al proceso de baja 
                else
                {
                    if (cbBajaExtension.Checked == true)
                    {
                        if (validaFormatoFecha(txtFechaInicio.Text.ToString()))
                        {
                            if (validaFormatoFecha(txtFechaFinExten.Text.ToString()))
                            {
                                bajaDeExtension();
                                tipoABC = "B";

                            }

                            else
                            {
                                mensajeDeAdvertencia("Formato de Fecha Fin incorrecto, favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA");
                                txtFechaFinExten.Text = "";
                                txtFechaFinExten.Focus();
                                mpeExten.Show();
                            }
                        }

                        else
                        {
                            mensajeDeAdvertencia("Formato de Fecha Inicio incorrecto, favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA");
                            txtFechaInicio.Text = "";
                            txtFechaInicio.Focus();
                            mpeExten.Show();
                        }
                    }

                    //Si se llega al pop-up mediante dar clic en el boton de agregar entonces llamara al proceso de alta de una extension 
                    else
                    {
                        if (validaFormatoFecha(txtFechaInicio.Text.ToString()))
                        {
                            procesoDeAltaExten();

                            tipoABC = "A";
                        }

                        else
                        {
                            mensajeDeAdvertencia("Formato de Fecha Inicio incorrecto, favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA");
                            txtFechaInicio.Text = "";
                            txtFechaInicio.Focus();
                            mpeExten.Show();
                        }
                    }
                }

                DataRow drExtension = ExisteLaExtension(txtExtension.Text, drpSitio.Text);

                DALCCustodia dalCC = new DALCCustodia();
                dalCC.guardaHistRecurso(drExtension["iCodCatalogo"].ToString(), "Ext", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), tipoABC);
                CambiarEstatusCCust(1);
                dtExtensiones.Clear();
                FillExtenGrid();

                //AM 20131212. Se agrega mensaje de proceso terminado.
                mensajeDeAdvertencia("Se completo el proceso correctamente.");
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al intentar guardar la extensión '" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        protected void lbtnGuardar_ExtenNoPopUp(object sender, EventArgs e)
        {
            try
            {
                string tipoABC = string.Empty;

                if (validaFormatoFecha(txtFechaInicioNoPopUp.Text.ToString()))
                {
                    procesoDeAltaExtenNoPopUp();

                    tipoABC = "A";
                }

                else
                {
                    mensajeDeAdvertencia("Formato de Fecha Inicio incorrecto, favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA");
                    txtFechaInicioNoPopUp.Text = "";
                    txtFechaInicioNoPopUp.Focus();
                }

                DataRow drExtension = ExisteLaExtension(txtExtensionNoPopUp.Text, drpSitioNoPopUp.Text);

                DALCCustodia dalCC = new DALCCustodia();
                dalCC.guardaHistRecurso(drExtension["iCodCatalogo"].ToString(), "Ext", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), tipoABC);
                CambiarEstatusCCust(1);
                dtExtensiones.Clear();
                FillExtenGrid();

                mensajeDeAdvertencia("Se completo el proceso correctamente.");
                limpiaCamposNoPopUpExten();
            }

            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al intentar guardar la extensión '" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        public void procesoDeAltaExten()
        {
            #region Los campos no contienen información o el formato es incorrecto.
            string validaCampos = validaCamposExtensiones();

            //Se valida que los campos necesarios no esten vacios y tengan el formato correcto.
            if (validaCampos.Length > 0)
            {
                mensajeDeAdvertencia(validaCampos);
                mpeExten.Show();
            }
            #endregion//Fin del bloque --Los campos no contienen información o el formato es incorrecto.

            #region Los campos contienen información y el formato es correcto.
            //Si los campos contienen información y tienen un formato correcto continua el proceso.
            else
            {
                #region La extensión a dar de alta es principal y el empleado ya cuenta con una extensión principal
                //AM.20131205 Se valida si el empleado cuenta con una extensión principal
                if ((drpTipoExten.SelectedItem.ToString() == "EXTENSIÓN PRINCIPAL" || drpTipoExten.SelectedIndex == 0) && !validaTipoExtension())
                {
                    mensajeDeAdvertencia("El empleado ya cuenta con una extensión principal, favor de seleccionar otro tipo de extensión.");
                    mpeExten.Show();
                }
                #endregion

                #region La extensión a dar de alta no es una extensión principal
                //Si el tipo de extensión no es principal sigue con el proceso
                else
                {
                    string extensionCod = txtExtension.Text.ToString().Trim();
                    DateTime dtFechaInicio = Convert.ToDateTime(txtFechaInicio.Text.ToString());

                    #region La fecha de inicio de la extensión es mayor o igual a la fecha de inicio del empleado.
                    if (validaFechaInicioExten(dtFechaInicio))
                    {
                        string psFechaInicio = dtFechaInicio.ToString("yyyy-MM-dd HH:mm:ss.fff");

                        //Se crea un objeto con todos los datos de la nueva extensión
                        DALCCustodia extension = new DALCCustodia();

                        //Se valida si la extensión ya existe
                        DataRow drExisteExtension = ExisteLaExtension(extensionCod, drpSitio.Text);
                        #region La extensión si existe
                        if (drExisteExtension != null)
                        {
                            string lsiCodCatalogoExten = drExisteExtension["iCodCatalogo"].ToString();
                            DataRow drRelEmpExtQuery = ExisteRelacion(extensionCod, lsiCodCatalogoExten);
                            string fechaInicioValida = ValidaHistoriaDeExtensión(ref dtFechaInicio, lsiCodCatalogoExten);

                            #region La fecha de inicio de la extensión si es valida
                            if (fechaInicioValida == "1")
                            {
                                #region La extensión ya esta asignada a otro empleado
                                //La extensión esta asignada a otro empleado ?
                                if (drRelEmpExtQuery != null)
                                {
                                    TextInfo miInfo;
                                    string nombreEmpleado;
                                    ObtieneNombreEmpleadoRelacionado(drRelEmpExtQuery, out miInfo, out nombreEmpleado);

                                    mensajeDeAdvertencia("La extensión que seleccionó ya esta asignada al empleado : " + miInfo.ToTitleCase(nombreEmpleado.ToLower()));
                                    mpeExten.Show();
                                    txtExtension.Text = "";
                                    txtExtension.Focus();
                                }
                                #endregion //Fin del bloque --La extensión ya esta asignada a otro empleado

                                #region La extensión no tiene relación con ningun empleado
                                //Si la extension no tiene relacion entra a este bloque para dar de Alta la relación 'Empleado - Extension' y da de alta un registro en ExtensionesB
                                else
                                {
                                    #region Existe un registro vigente en relaciones para la extension que se desea dar de alta.

                                    int lintrel;
                                    string iCodRegistroRelac = string.Empty;
                                    if (drExisteExtension != null)
                                    {
                                        iCodRegistroRelac = ExisteRegistroVigenteEnRelac(drExisteExtension["iCodCatalogo"].ToString());
                                    }

                                    if (int.TryParse(iCodRegistroRelac, out lintrel))
                                    {
                                        #region Baja de registro en relaciones.


                                        DateTime ldtIniVig = (DateTime)DSODataAccess.ExecuteScalar(
                                            "select dtIniVigencia from [VisRelaciones('Empleado - Extension','Español')]" +
                                            "where iCodRegistro = " + iCodRegistroRelac);

                                        if (DateTime.Today.AddDays(-1) >= ldtIniVig)
                                        {
                                            DSODataAccess.ExecuteNonQuery("update [VisRelaciones('Empleado - Extension','Español')]" +
                                                                          "set dtFinVigencia = getdate()-1, dtFecUltAct = getdate()" +
                                                                          "where iCodRegistro = " + iCodRegistroRelac);
                                        }

                                        else
                                        {
                                            DSODataAccess.ExecuteNonQuery("update [VisRelaciones('Empleado - Extension','Español')]" +
                                                                          "set dtFinVigencia = dtIniVigencia, dtFecUltAct = getdate()" +
                                                                          "where iCodRegistro = " + iCodRegistroRelac);
                                        }

                                        #endregion //Fin de bloque --Baja de registro en relaciones.
                                    }

                                    #endregion //Fin de bloque --Existe un registro vigente en relaciones para la extension que se desea dar de alta.

                                    #region Alta de la relaciones
                                    //Variables que se necesitan extraer para mandar como parametros en el metodo que da de alta un registro en relaciones
                                    int iCodCatalogoExten = (int)drExisteExtension["iCodCatalogo"];
                                    string vchCodigoExten = drExisteExtension["vchCodigo"].ToString();
                                    string vchCodigoEmple = txtNominaEmple.Text.ToString();
                                    DateTime dtIniVigenciaEmple = Convert.ToDateTime(txtFecha.Text.ToString());
                                    string dtIniVigenciaRelacion = Convert.ToDateTime(txtFechaInicio.Text).ToString("yyyy-MM-dd HH:mm:ss.fff");

                                    // Se hace un insert en [VisRelaciones('Empleado - Extension','Español')] 
                                    // Se hace un update a la vista [VisHistoricos('Exten','Extensiones','Español')] en el campo Emple
                                    extension.altaRelacionEmpExt(iCodCatalogoExten, vchCodigoExten, iCodCatalogoEmple, vchCodigoEmple,
                                                                 dtIniVigenciaRelacion);
                                    #endregion //Fin de bloque --Proceso de Alta de la relación

                                    #region Existe un registro vigente de esta extensión en Extensiones B

                                    int lint;
                                    string iCodRegistroExtenB = string.Empty;
                                    if (drExisteExtension != null)
                                    {
                                        iCodRegistroExtenB = ExisteRegistroVigenteEnExtenB(drExisteExtension["iCodCatalogo"].ToString());
                                    }

                                    if (int.TryParse(iCodRegistroExtenB, out lint))
                                    {
                                        #region Baja de registro en Extensiones B

                                        DateTime ldtIniVig = (DateTime)DSODataAccess.ExecuteScalar(
                                            "select dtIniVigencia from [VisHistoricos('ExtenB','Extensiones B','Español')]" +
                                            "where iCodRegistro = " + iCodRegistroExtenB);

                                        if (DateTime.Today.AddDays(-1) >= ldtIniVig)
                                        {
                                            DSODataAccess.ExecuteNonQuery("update [VisHistoricos('ExtenB','Extensiones B','Español')]" +
                                                                          "set dtFinVigencia = getdate()-1, dtFecUltAct = getdate()" +
                                                                          "where iCodRegistro = " + iCodRegistroExtenB);
                                        }

                                        else
                                        {
                                            DSODataAccess.ExecuteNonQuery("update [VisHistoricos('ExtenB','Extensiones B','Español')]" +
                                                                          "set dtFinVigencia = dtIniVigencia, dtFecUltAct = getdate()" +
                                                                          "where iCodRegistro = " + iCodRegistroExtenB);
                                        }

                                        #endregion //Fin de bloque --Baja de registro en Extensiones B
                                    }

                                    #endregion //Fin de bloque --Existe un registro vigente de esta extensión en Extensiones B

                                    #region Proceso de Alta en Extensiónes B
                                    DataRow driCodExten = ExisteLaExtension(extensionCod, drpSitio.Text);
                                    string lsiCodExten = driCodExten["iCodCatalogo"].ToString();
                                    string lsSitioDesc = driCodExten["SitioDesc"].ToString();


                                    extension.altaEnExtensionesB(extensionCod, lsSitioDesc, lsiCodExten, drpTipoExten.Text.ToString(),
                                                                 txtComentariosExten.Text.ToString(), dtFechaInicio);
                                    #endregion //Fin del bloque --Proceso de Alta en Extensiónes B

                                    ActualizaAtributosDeExtensiones(lsiCodExten, drpVisibleDir.Text);
                                    mpeExten.Hide();
                                }
                                #endregion //Fin del bloque --La extensión no tiene relación con ningun empleado
                            }
                            #endregion //Fin del bloque --La fecha de inicio de la extensión si es valida

                            #region La fecha de inicio de la extensión no es valida.
                            //La fecha de inicio no es valida
                            else
                            {
                                mensajeDeAdvertencia("La fecha que selecciono entra dentro de los rangos de relación de otro empleado, favor de seleccionar una fecha validá.");
                                mpeExten.Show();
                                txtFechaInicio.Text = "";
                                txtFechaInicio.Focus();
                            }
                            #endregion
                        }
                        #endregion //Fin del bloque --La extensión si existe

                        #region La extensión no existe

                        else
                        {
                            DataRow[] drArrayRangosExtension;
                            DataRow drRangosExtension;
                            ConsultaRangosConfigEnSitio(out drArrayRangosExtension, out drRangosExtension, drpSitio.Text);

                            //Variables necesarias para mandar parametros a metodo de altaExtension
                            string lsSitioDesc = drRangosExtension["vchDescripcion"].ToString();
                            string lsLongitudExtFin = drRangosExtension["ExtFin"].ToString();

                            #region La extensión entra dentro de los rangos configurados y la longitud de la extension es menor o igual a la longitud de la ExtFin
                            if (extension.ExtEnRango(extensionCod, drArrayRangosExtension) && extensionCod.Length <= lsLongitudExtFin.Length)
                            {
                                #region Existe un registro vigente de esta extensión en Extensiones B

                                int lint;
                                string iCodRegistroExtenB = string.Empty;
                                if (drExisteExtension != null)
                                {
                                    iCodRegistroExtenB = ExisteRegistroVigenteEnExtenB(drExisteExtension["iCodCatalogo"].ToString());
                                }

                                if (int.TryParse(iCodRegistroExtenB, out lint))
                                {
                                    #region Baja de registro en Extensiones B


                                    DateTime ldtIniVig = (DateTime)DSODataAccess.ExecuteScalar(
                                        "select dtIniVigencia from [VisHistoricos('ExtenB','Extensiones B','Español')]" +
                                        "where iCodRegistro = " + iCodRegistroExtenB);

                                    if (DateTime.Today.AddDays(-1) >= ldtIniVig)
                                    {
                                        DSODataAccess.ExecuteNonQuery("update [VisHistoricos('ExtenB','Extensiones B','Español')]" +
                                                                      "set dtFinVigencia = getdate()-1, dtFecUltAct = getdate()" +
                                                                      "where iCodRegistro = " + iCodRegistroExtenB);
                                    }

                                    else
                                    {
                                        DSODataAccess.ExecuteNonQuery("update [VisHistoricos('ExtenB','Extensiones B','Español')]" +
                                                                      "set dtFinVigencia = dtIniVigencia, dtFecUltAct = getdate()" +
                                                                      "where iCodRegistro = " + iCodRegistroExtenB);
                                    }

                                    #endregion //Fin de bloque --Baja de registro en Extensiones B
                                }

                                #endregion //Fin de bloque --Existe un registro vigente de esta extensión en Extensiones B

                                #region Alta de la extensión en extensiones y extensiones B
                                //Dar de Alta de la extensión
                                extension.altaExtension(extensionCod, drpSitio.Text.ToString(), lsSitioDesc, dtFechaInicio, drpTipoExten.Text.ToString(), txtComentariosExten.Text.ToString(), drpVisibleDir.Text.ToString());
                                #endregion //Fin del bloque --Alta de la extensión

                                #region Existe un registro vigente en relaciones para la extension que se desea dar de alta.

                                int lintrel;
                                string iCodRegistroRelac = string.Empty;
                                if (drExisteExtension != null)
                                {
                                    iCodRegistroRelac = ExisteRegistroVigenteEnRelac(drExisteExtension["iCodCatalogo"].ToString());
                                }

                                if (int.TryParse(iCodRegistroRelac, out lintrel))
                                {
                                    #region Baja de registro en relaciones.


                                    DateTime ldtIniVig = (DateTime)DSODataAccess.ExecuteScalar(
                                        "select dtIniVigencia from [VisRelaciones('Empleado - Extension','Español')]" +
                                        "where iCodRegistro = " + iCodRegistroRelac);

                                    if (DateTime.Today.AddDays(-1) >= ldtIniVig)
                                    {
                                        DSODataAccess.ExecuteNonQuery("update [VisRelaciones('Empleado - Extension','Español')]" +
                                                                      "set dtFinVigencia = getdate()-1, dtFecUltAct = getdate()" +
                                                                      "where iCodRegistro = " + iCodRegistroRelac);
                                    }

                                    else
                                    {
                                        DSODataAccess.ExecuteNonQuery("update [VisRelaciones('Empleado - Extension','Español')]" +
                                                                      "set dtFinVigencia = dtIniVigencia, dtFecUltAct = getdate()" +
                                                                      "where iCodRegistro = " + iCodRegistroRelac);
                                    }

                                    #endregion //Fin de bloque --Baja de registro en relaciones.
                                }

                                #endregion //Fin de bloque --Existe un registro vigente en relaciones para la extension que se desea dar de alta.

                                #region Alta en relaciones
                                DataRow drExtensionReciente = ExisteLaExtension(extensionCod, drpSitio.Text);

                                //Variables que se necesitan extraer para mandar como parametros en el metodo que da de alta un registro en relaciones
                                int iCodCatalogoExten = (int)drExtensionReciente["iCodCatalogo"];
                                string vchCodigoExten = drExtensionReciente["vchCodigo"].ToString();
                                string vchCodigoEmple = txtNominaEmple.Text.ToString();
                                string dtIniVigenciaRelacion = dtFechaInicio.ToString("yyyy-MM-dd HH:mm:ss.fff");

                                // Se hace un insert en [VisRelaciones('Empleado - Extension','Español')] 
                                // Se hace un update a la vista [VisHistoricos('Exten','Extensiones','Español')] en el campo Emple
                                extension.altaRelacionEmpExt(iCodCatalogoExten, vchCodigoExten, iCodCatalogoEmple, vchCodigoEmple, dtIniVigenciaRelacion);
                                #endregion //Fin del bloque --Alta en relaciones

                                mpeExten.Hide();
                            }
                            #endregion //Fin del bloque --La extensión entra dentro de los rangos configurados y la longitud de la extension es menor o igual a la longitud de la ExtFin

                            #region La extensión no entra dentro de los rangos configurados en el sitio.

                            else
                            {
                                #region La bandera dar de alta nuevo rango esta activada.

                                if (cbRangoExten.Checked)
                                {
                                    #region Alta de la extensión
                                    //Dar de alta la extension 
                                    extension.altaExtension(extensionCod, drpSitio.Text.ToString(), lsSitioDesc, dtFechaInicio,
                                                            drpTipoExten.Text.ToString(), txtComentariosExten.Text.ToString(),
                                                            drpVisibleDir.Text.ToString());
                                    #endregion //Fin de bloque --Alta de la extensión

                                    #region Alta en relaciones
                                    DataRow drExtensionReciente = ExisteLaExtension(extensionCod, drpSitio.Text);
                                    //Variables que se necesitan extraer para mandar como parametros en el metodo que da de alta un registro en relaciones
                                    int iCodCatalogoExten = (int)drExtensionReciente["iCodCatalogo"];
                                    string vchCodigoExten = drExtensionReciente["vchCodigo"].ToString();
                                    string vchCodigoEmple = txtNominaEmple.Text.ToString();
                                    string dtIniVigenciaRelacion = dtFechaInicio.ToString("yyyy-MM-dd HH:mm:ss.fff");

                                    // Se hace un insert en [VisRelaciones('Empleado - Extension','Español')] 
                                    // Se hace un update a la vista [VisHistoricos('Exten','Extensiones','Español')] en el campo Emple
                                    extension.altaRelacionEmpExt(iCodCatalogoExten, vchCodigoExten, iCodCatalogoEmple, vchCodigoEmple,
                                                                 dtIniVigenciaRelacion);
                                    #endregion //Fin de bloque --Alta en relaciones

                                    #region Dar de alta nuevo rango de extensión

                                    string lsRangosExt = drRangosExtension["RangosExt"].ToString();
                                    string lsExtIni = drRangosExtension["ExtIni"].ToString();
                                    string lsExtFin = drRangosExtension["ExtFin"].ToString();
                                    string lsiCodMaestroSitio = drRangosExtension["iCodMaestro"].ToString();

                                    extension.altaNuevoRango(drpSitio.Text.ToString(), extensionCod, lsRangosExt, lsiCodMaestroSitio,
                                                             lsExtIni, lsExtFin);

                                    #endregion //Fin de bloque --Dar de alta nuevo rango de extensión

                                    mpeExten.Hide();
                                }

                                #endregion //Fin de bloque --La bandera dar de alta nuevo rango esta activada.

                                #region La bandera dar de alta nuevo rango "NO" esta activada.

                                else
                                {
                                    mensajeDeAdvertencia("El rango de extensión no existe en el sitio, si desea continuar con el alta de la extensión debe seleccionar la bandera (Dar de alta nuevo rango de extensión) ");
                                    mpeExten.Show();
                                }

                                #endregion //Fin de bloque --La bandera dar de alta nuevo rango "NO" esta activada.
                            }

                            #endregion //Fin de bloque --La extensión no entra dentro de los rangos configurados en el sitio.
                        }

                        #endregion //Fin de bloque --La extensión no existe
                    }
                    #endregion //Fin del bloque --La fecha de inicio de la estensión es mayor o igual a la fecha de inicio del empleado.

                    #region La fecha de inicio de la extensión es menor a la fecha de inicio del empleado.

                    //Si la fecha de inicio es menor a la fecha de alta del empleado manda un mensaje de notificación
                    else
                    {
                        mensajeDeAdvertencia("La fecha de inicio debe ser mayor a la fecha de alta del empleado.");
                        mpeExten.Show();
                    }

                    #endregion
                }
                #endregion //Fin del bloque --La extensión a dar de alta no es una extensión principal
            }
            #endregion  //Fin del bloque --Los campos contienen información y el formato es correcto.
        }

        public void procesoDeAltaExtenNoPopUp()
        {
            #region Los campos no contienen información o el formato es incorrecto.
            string validaCampos = validaCamposExtensionesNoPopUp();

            //Se valida que los campos necesarios no esten vacios y tengan el formato correcto.
            if (validaCampos.Length > 0)
            {
                mensajeDeAdvertencia(validaCampos);
            }
            #endregion//Fin del bloque --Los campos no contienen información o el formato es incorrecto.

            #region Los campos contienen información y el formato es correcto.
            //Si los campos contienen información y tienen un formato correcto continua el proceso.
            else
            {
                #region La extensión a dar de alta es principal y el empleado ya cuenta con una extensión principal
                //AM.20131205 Se valida si el empleado cuenta con una extensión principal
                if ((drpTipoExtenNoPopUp.SelectedItem.ToString() == "EXTENSIÓN PRINCIPAL" || drpTipoExtenNoPopUp.SelectedIndex == 0) && !validaTipoExtension())
                {
                    mensajeDeAdvertencia("El empleado ya cuenta con una extensión principal, favor de seleccionar otro tipo de extensión.");
                }
                #endregion

                #region La extensión a dar de alta no es una extensión principal
                //Si el tipo de extensión no es principal sigue con el proceso
                else
                {
                    string extensionCod = txtExtensionNoPopUp.Text.ToString().Trim();
                    DateTime dtFechaInicio = Convert.ToDateTime(txtFechaInicioNoPopUp.Text.ToString());

                    #region La fecha de inicio de la extensión es mayor o igual a la fecha de inicio del empleado.
                    if (validaFechaInicioExten(dtFechaInicio))
                    {
                        string psFechaInicio = dtFechaInicio.ToString("yyyy-MM-dd HH:mm:ss.fff");

                        //Se crea un objeto con todos los datos de la nueva extensión
                        DALCCustodia extension = new DALCCustodia();

                        //Se valida si la extensión ya existe
                        DataRow drExisteExtension = ExisteLaExtension(extensionCod, drpSitioNoPopUp.Text);
                        #region La extensión si existe
                        if (drExisteExtension != null)
                        {
                            string lsiCodCatalogoExten = drExisteExtension["iCodCatalogo"].ToString();
                            DataRow drRelEmpExtQuery = ExisteRelacion(extensionCod, lsiCodCatalogoExten);
                            string fechaInicioValida = ValidaHistoriaDeExtensión(ref dtFechaInicio, lsiCodCatalogoExten);

                            #region La fecha de inicio de la extensión si es valida
                            if (fechaInicioValida == "1")
                            {
                                #region La extensión ya esta asignada a otro empleado
                                //La extensión esta asignada a otro empleado ?
                                if (drRelEmpExtQuery != null)
                                {
                                    TextInfo miInfo;
                                    string nombreEmpleado;
                                    ObtieneNombreEmpleadoRelacionado(drRelEmpExtQuery, out miInfo, out nombreEmpleado);

                                    mensajeDeAdvertencia("La extensión que seleccionó ya esta asignada al empleado : " + miInfo.ToTitleCase(nombreEmpleado.ToLower()));
                                    txtExtensionNoPopUp.Text = "";
                                    txtExtensionNoPopUp.Focus();
                                }
                                #endregion //Fin del bloque --La extensión ya esta asignada a otro empleado

                                #region La extensión no tiene relación con ningun empleado
                                //Si la extension no tiene relacion entra a este bloque para dar de Alta la relación 'Empleado - Extension' y da de alta un registro en ExtensionesB
                                else
                                {
                                    #region Existe un registro vigente en relaciones para la extension que se desea dar de alta.

                                    int lintrel;
                                    string iCodRegistroRelac = string.Empty;
                                    if (drExisteExtension != null)
                                    {
                                        iCodRegistroRelac = ExisteRegistroVigenteEnRelac(drExisteExtension["iCodCatalogo"].ToString());
                                    }

                                    if (int.TryParse(iCodRegistroRelac, out lintrel))
                                    {
                                        #region Baja de registro en relaciones.


                                        DateTime ldtIniVig = (DateTime)DSODataAccess.ExecuteScalar(
                                            "select dtIniVigencia from [VisRelaciones('Empleado - Extension','Español')]" +
                                            "where iCodRegistro = " + iCodRegistroRelac);

                                        if (DateTime.Today.AddDays(-1) >= ldtIniVig)
                                        {
                                            DSODataAccess.ExecuteNonQuery("update [VisRelaciones('Empleado - Extension','Español')]" +
                                                                          "set dtFinVigencia = getdate()-1, dtFecUltAct = getdate()" +
                                                                          "where iCodRegistro = " + iCodRegistroRelac);
                                        }

                                        else
                                        {
                                            DSODataAccess.ExecuteNonQuery("update [VisRelaciones('Empleado - Extension','Español')]" +
                                                                          "set dtFinVigencia = dtIniVigencia, dtFecUltAct = getdate()" +
                                                                          "where iCodRegistro = " + iCodRegistroRelac);
                                        }

                                        #endregion //Fin de bloque --Baja de registro en relaciones.
                                    }

                                    #endregion //Fin de bloque --Existe un registro vigente en relaciones para la extension que se desea dar de alta.

                                    #region Alta de la relaciones
                                    //Variables que se necesitan extraer para mandar como parametros en el metodo que da de alta un registro en relaciones
                                    int iCodCatalogoExten = (int)drExisteExtension["iCodCatalogo"];
                                    string vchCodigoExten = drExisteExtension["vchCodigo"].ToString();
                                    string vchCodigoEmple = txtNominaEmple.Text.ToString();
                                    DateTime dtIniVigenciaEmple = Convert.ToDateTime(txtFecha.Text.ToString());
                                    string dtIniVigenciaRelacion = Convert.ToDateTime(txtFechaInicioNoPopUp.Text).ToString("yyyy-MM-dd HH:mm:ss.fff");

                                    // Se hace un insert en [VisRelaciones('Empleado - Extension','Español')] 
                                    // Se hace un update a la vista [VisHistoricos('Exten','Extensiones','Español')] en el campo Emple
                                    extension.altaRelacionEmpExt(iCodCatalogoExten, vchCodigoExten, iCodCatalogoEmple, vchCodigoEmple,
                                                                 dtIniVigenciaRelacion);
                                    #endregion //Fin de bloque --Proceso de Alta de la relación

                                    #region Existe un registro vigente de esta extensión en Extensiones B

                                    int lint;
                                    string iCodRegistroExtenB = string.Empty;
                                    if (drExisteExtension != null)
                                    {
                                        iCodRegistroExtenB = ExisteRegistroVigenteEnExtenB(drExisteExtension["iCodCatalogo"].ToString());
                                    }

                                    if (int.TryParse(iCodRegistroExtenB, out lint))
                                    {
                                        #region Baja de registro en Extensiones B

                                        DateTime ldtIniVig = (DateTime)DSODataAccess.ExecuteScalar(
                                            "select dtIniVigencia from [VisHistoricos('ExtenB','Extensiones B','Español')]" +
                                            "where iCodRegistro = " + iCodRegistroExtenB);

                                        if (DateTime.Today.AddDays(-1) >= ldtIniVig)
                                        {
                                            DSODataAccess.ExecuteNonQuery("update [VisHistoricos('ExtenB','Extensiones B','Español')]" +
                                                                          "set dtFinVigencia = getdate()-1, dtFecUltAct = getdate()" +
                                                                          "where iCodRegistro = " + iCodRegistroExtenB);
                                        }

                                        else
                                        {
                                            DSODataAccess.ExecuteNonQuery("update [VisHistoricos('ExtenB','Extensiones B','Español')]" +
                                                                          "set dtFinVigencia = dtIniVigencia, dtFecUltAct = getdate()" +
                                                                          "where iCodRegistro = " + iCodRegistroExtenB);
                                        }

                                        #endregion //Fin de bloque --Baja de registro en Extensiones B
                                    }

                                    #endregion //Fin de bloque --Existe un registro vigente de esta extensión en Extensiones B

                                    #region Proceso de Alta en Extensiónes B
                                    DataRow driCodExten = ExisteLaExtension(extensionCod, drpSitioNoPopUp.Text);
                                    string lsiCodExten = driCodExten["iCodCatalogo"].ToString();
                                    string lsSitioDesc = driCodExten["SitioDesc"].ToString();


                                    extension.altaEnExtensionesB(extensionCod, lsSitioDesc, lsiCodExten, drpTipoExtenNoPopUp.Text.ToString(),
                                                                 txtComentariosExtenNoPopUp.Text.ToString(), dtFechaInicio);
                                    #endregion //Fin del bloque --Proceso de Alta en Extensiónes B

                                    ActualizaAtributosDeExtensiones(lsiCodExten, drpVisibleDirNoPopUp.Text);
                                }
                                #endregion //Fin del bloque --La extensión no tiene relación con ningun empleado
                            }
                            #endregion //Fin del bloque --La fecha de inicio de la extensión si es valida

                            #region La fecha de inicio de la extensión no es valida.
                            //La fecha de inicio no es valida
                            else
                            {
                                mensajeDeAdvertencia("La fecha que selecciono entra dentro de los rangos de relación de otro empleado, favor de seleccionar una fecha validá.");
                                txtFechaInicioNoPopUp.Text = "";
                                txtFechaInicioNoPopUp.Focus();
                            }
                            #endregion
                        }
                        #endregion //Fin del bloque --La extensión si existe

                        #region La extensión no existe

                        else
                        {
                            DataRow[] drArrayRangosExtension;
                            DataRow drRangosExtension;
                            ConsultaRangosConfigEnSitio(out drArrayRangosExtension, out drRangosExtension, drpSitioNoPopUp.Text);

                            //Variables necesarias para mandar parametros a metodo de altaExtension
                            string lsSitioDesc = drRangosExtension["vchDescripcion"].ToString();
                            string lsLongitudExtFin = drRangosExtension["ExtFin"].ToString();

                            #region La extensión entra dentro de los rangos configurados y la longitud de la extension es menor o igual a la longitud de la ExtFin
                            if (extension.ExtEnRango(extensionCod, drArrayRangosExtension) && extensionCod.Length <= lsLongitudExtFin.Length)
                            {
                                #region Existe un registro vigente de esta extensión en Extensiones B

                                int lint;
                                string iCodRegistroExtenB = string.Empty;
                                if (drExisteExtension != null)
                                {
                                    iCodRegistroExtenB = ExisteRegistroVigenteEnExtenB(drExisteExtension["iCodCatalogo"].ToString());
                                }

                                if (int.TryParse(iCodRegistroExtenB, out lint))
                                {
                                    #region Baja de registro en Extensiones B


                                    DateTime ldtIniVig = (DateTime)DSODataAccess.ExecuteScalar(
                                        "select dtIniVigencia from [VisHistoricos('ExtenB','Extensiones B','Español')]" +
                                        "where iCodRegistro = " + iCodRegistroExtenB);

                                    if (DateTime.Today.AddDays(-1) >= ldtIniVig)
                                    {
                                        DSODataAccess.ExecuteNonQuery("update [VisHistoricos('ExtenB','Extensiones B','Español')]" +
                                                                      "set dtFinVigencia = getdate()-1, dtFecUltAct = getdate()" +
                                                                      "where iCodRegistro = " + iCodRegistroExtenB);
                                    }

                                    else
                                    {
                                        DSODataAccess.ExecuteNonQuery("update [VisHistoricos('ExtenB','Extensiones B','Español')]" +
                                                                      "set dtFinVigencia = dtIniVigencia, dtFecUltAct = getdate()" +
                                                                      "where iCodRegistro = " + iCodRegistroExtenB);
                                    }

                                    #endregion //Fin de bloque --Baja de registro en Extensiones B
                                }

                                #endregion //Fin de bloque --Existe un registro vigente de esta extensión en Extensiones B

                                #region Alta de la extensión en extensiones y extensiones B
                                //Dar de Alta de la extensión
                                extension.altaExtension(extensionCod, drpSitioNoPopUp.Text.ToString(), lsSitioDesc, dtFechaInicio,
                                    drpTipoExtenNoPopUp.Text.ToString(), txtComentariosExtenNoPopUp.Text.ToString(), drpVisibleDirNoPopUp.Text.ToString());
                                #endregion //Fin del bloque --Alta de la extensión

                                #region Existe un registro vigente en relaciones para la extension que se desea dar de alta.

                                int lintrel;
                                string iCodRegistroRelac = string.Empty;
                                if (drExisteExtension != null)
                                {
                                    iCodRegistroRelac = ExisteRegistroVigenteEnRelac(drExisteExtension["iCodCatalogo"].ToString());
                                }

                                if (int.TryParse(iCodRegistroRelac, out lintrel))
                                {
                                    #region Baja de registro en relaciones.


                                    DateTime ldtIniVig = (DateTime)DSODataAccess.ExecuteScalar(
                                        "select dtIniVigencia from [VisRelaciones('Empleado - Extension','Español')]" +
                                        "where iCodRegistro = " + iCodRegistroRelac);

                                    if (DateTime.Today.AddDays(-1) >= ldtIniVig)
                                    {
                                        DSODataAccess.ExecuteNonQuery("update [VisRelaciones('Empleado - Extension','Español')]" +
                                                                      "set dtFinVigencia = getdate()-1, dtFecUltAct = getdate()" +
                                                                      "where iCodRegistro = " + iCodRegistroRelac);
                                    }

                                    else
                                    {
                                        DSODataAccess.ExecuteNonQuery("update [VisRelaciones('Empleado - Extension','Español')]" +
                                                                      "set dtFinVigencia = dtIniVigencia, dtFecUltAct = getdate()" +
                                                                      "where iCodRegistro = " + iCodRegistroRelac);
                                    }

                                    #endregion //Fin de bloque --Baja de registro en relaciones.
                                }

                                #endregion //Fin de bloque --Existe un registro vigente en relaciones para la extension que se desea dar de alta.

                                #region Alta en relaciones
                                DataRow drExtensionReciente = ExisteLaExtension(extensionCod, drpSitioNoPopUp.Text);

                                //Variables que se necesitan extraer para mandar como parametros en el metodo que da de alta un registro en relaciones
                                int iCodCatalogoExten = (int)drExtensionReciente["iCodCatalogo"];
                                string vchCodigoExten = drExtensionReciente["vchCodigo"].ToString();
                                string vchCodigoEmple = txtNominaEmple.Text.ToString();
                                string dtIniVigenciaRelacion = dtFechaInicio.ToString("yyyy-MM-dd HH:mm:ss.fff");

                                // Se hace un insert en [VisRelaciones('Empleado - Extension','Español')] 
                                // Se hace un update a la vista [VisHistoricos('Exten','Extensiones','Español')] en el campo Emple
                                extension.altaRelacionEmpExt(iCodCatalogoExten, vchCodigoExten, iCodCatalogoEmple, vchCodigoEmple, dtIniVigenciaRelacion);
                                #endregion //Fin del bloque --Alta en relaciones
                            }
                            #endregion //Fin del bloque --La extensión entra dentro de los rangos configurados y la longitud de la extension es menor o igual a la longitud de la ExtFin

                            #region La extensión no entra dentro de los rangos configurados en el sitio.

                            else
                            {
                                #region La bandera dar de alta nuevo rango esta activada.

                                if (cbRangoExtenNoPopUp.Checked)
                                {
                                    #region Alta de la extensión
                                    //Dar de alta la extension 
                                    extension.altaExtension(extensionCod, drpSitioNoPopUp.Text.ToString(), lsSitioDesc, dtFechaInicio,
                                                            drpTipoExtenNoPopUp.Text.ToString(), txtComentariosExtenNoPopUp.Text.ToString(),
                                                            drpVisibleDirNoPopUp.Text.ToString());
                                    #endregion //Fin de bloque --Alta de la extensión

                                    #region Alta en relaciones
                                    DataRow drExtensionReciente = ExisteLaExtension(extensionCod, drpSitioNoPopUp.Text);
                                    //Variables que se necesitan extraer para mandar como parametros en el metodo que da de alta un registro en relaciones
                                    int iCodCatalogoExten = (int)drExtensionReciente["iCodCatalogo"];
                                    string vchCodigoExten = drExtensionReciente["vchCodigo"].ToString();
                                    string vchCodigoEmple = txtNominaEmple.Text.ToString();
                                    string dtIniVigenciaRelacion = dtFechaInicio.ToString("yyyy-MM-dd HH:mm:ss.fff");

                                    // Se hace un insert en [VisRelaciones('Empleado - Extension','Español')] 
                                    // Se hace un update a la vista [VisHistoricos('Exten','Extensiones','Español')] en el campo Emple
                                    extension.altaRelacionEmpExt(iCodCatalogoExten, vchCodigoExten, iCodCatalogoEmple, vchCodigoEmple,
                                                                 dtIniVigenciaRelacion);
                                    #endregion //Fin de bloque --Alta en relaciones

                                    #region Dar de alta nuevo rango de extensión

                                    string lsRangosExt = drRangosExtension["RangosExt"].ToString();
                                    string lsExtIni = drRangosExtension["ExtIni"].ToString();
                                    string lsExtFin = drRangosExtension["ExtFin"].ToString();
                                    string lsiCodMaestroSitio = drRangosExtension["iCodMaestro"].ToString();

                                    extension.altaNuevoRango(drpSitioNoPopUp.Text.ToString(), extensionCod, lsRangosExt, lsiCodMaestroSitio,
                                                             lsExtIni, lsExtFin);

                                    #endregion //Fin de bloque --Dar de alta nuevo rango de extensión
                                }

                                #endregion //Fin de bloque --La bandera dar de alta nuevo rango esta activada.

                                #region La bandera dar de alta nuevo rango "NO" esta activada.

                                else
                                {
                                    mensajeDeAdvertencia("El rango de extensión no existe en el sitio, si desea continuar con el alta de la extensión debe seleccionar la bandera (Dar de alta nuevo rango de extensión) ");
                                }

                                #endregion //Fin de bloque --La bandera dar de alta nuevo rango "NO" esta activada.
                            }

                            #endregion //Fin de bloque --La extensión no entra dentro de los rangos configurados en el sitio.
                        }

                        #endregion //Fin de bloque --La extensión no existe
                    }
                    #endregion //Fin del bloque --La fecha de inicio de la estensión es mayor o igual a la fecha de inicio del empleado.

                    #region La fecha de inicio de la extensión es menor a la fecha de inicio del empleado.

                    //Si la fecha de inicio es menor a la fecha de alta del empleado manda un mensaje de notificación
                    else
                    {
                        mensajeDeAdvertencia("La fecha de inicio debe ser mayor a la fecha de alta del empleado.");
                    }

                    #endregion
                }
                #endregion //Fin del bloque --La extensión a dar de alta no es una extensión principal
            }
            #endregion  //Fin del bloque --Los campos contienen información y el formato es correcto.
        }

        private void ConsultaRangosConfigEnSitio(out DataRow[] drArrayRangosExtension, out DataRow drRangosExtension, string iCodSitio)
        {
            //Query para ver si la extensión entra dentro de los rangos configurados
            drArrayRangosExtension = new DataRow[1];
            StringBuilder sbRangosExtensionQuery = new StringBuilder();

            sbRangosExtensionQuery.AppendFormat("EXEC ObtieneRangosExtensiones @esquema = '{0}', @iCodCatSitio = {1}", DSODataContext.Schema, iCodSitio);
            drRangosExtension = DSODataAccess.ExecuteDataRow(sbRangosExtensionQuery.ToString());
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

        private static string ValidaHistoriaDeExtensión(ref DateTime dtFechaInicio, string lsiCodCatalogoExten)
        {
            //Validar las fechas de la historia de la extension seleccionada
            string fechaInicioValida = DSODataAccess.ExecuteScalar("ValidaHistoriaRecurso   @iCodRecurso = " + lsiCodCatalogoExten + "," +
                                                                   "                        @fechaweb = '" + dtFechaInicio.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," +
                                                                   "                        @iCodRegistroRel = null," +
                                                                   "                        @nombreCampoiCodRecurso  = 'Exten'," +
                                                                   "                        @RelacionTripleComilla = '''Empleado - Extension'''").ToString();
            return fechaInicioValida;
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
            sbRelEmpExtQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
            sbRelEmpExtQuery.AppendLine("and dtfinvigencia >= getdate()");
            sbRelEmpExtQuery.AppendLine("and Exten = " + lsiCodCatalogoExten);
            sbRelEmpExtQuery.AppendLine("and ExtenCod = '" + extensionCod + "'");
            DataRow drRelEmpExtQuery = DSODataAccess.ExecuteDataRow(sbRelEmpExtQuery.ToString());
            return drRelEmpExtQuery;
        }

        private DataRow ExisteLaExtension(string extensionCod, string iCodSitio)
        {
            //Query para ver si la extensión ya existe
            StringBuilder sbExisteExtensionQuery = new StringBuilder();
            sbExisteExtensionQuery.AppendLine("select iCodCatalogo, vchCodigo, SitioDesc from [VisHistoricos('Exten','Extensiones','Español')]");
            sbExisteExtensionQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
            sbExisteExtensionQuery.AppendLine("and dtfinvigencia >= getdate()");
            sbExisteExtensionQuery.AppendLine("and vchCodigo = '" + extensionCod + "'");
            sbExisteExtensionQuery.AppendLine("and Sitio = " + iCodSitio);
            DataRow drExisteExtension = DSODataAccess.ExecuteDataRow(sbExisteExtensionQuery.ToString());
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
                lsb.Append("select COUNT(*) \r");
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
        private string validaCamposExtensiones()
        {
            StringBuilder sbErrors = new StringBuilder(string.Empty);

            if (txtExtension.Text == string.Empty || txtExtension.Text == "")
            {
                sbErrors.Append(@"*El campo (Extensión) es requerido. \n");
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(txtExtension.Text, @"^\d*$"))
            {
                sbErrors.Append(@"*El campo (Extensión) solo debe contener números. \n");
            }

            if (String.IsNullOrEmpty(drpSitio.SelectedValue))
            {
                sbErrors.Append(@"*El campo (Sitio) es requerido. \n");
            }

            if (txtFechaInicio.Text == string.Empty || txtFechaInicio.Text == "")
            {
                sbErrors.Append(@"*El campo (Fecha inicio) es requerido. \n");
            }

            if (!validaFormatoFecha(txtFechaInicio.Text))
            {
                sbErrors.Append(@"*El formato de (Fecha inicio) debe ser DD/MM/AAAA. \n");
                txtFechaInicioCenCos.Text = string.Empty;
            }

            if (String.IsNullOrEmpty(drpTipoExten.SelectedValue))
            {
                sbErrors.Append(@"*El campo (Tipo de extensión) es requerido. \n");
            }

            return sbErrors.ToString();
        }

        private string validaCamposExtensionesNoPopUp()
        {
            StringBuilder sbErrors = new StringBuilder(string.Empty);

            if (txtExtensionNoPopUp.Text == string.Empty || txtExtensionNoPopUp.Text == "")
            {
                sbErrors.Append(@"*El campo (Extensión) es requerido. \n");
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(txtExtensionNoPopUp.Text, @"^\d*$"))
            {
                sbErrors.Append(@"*El campo (Extensión) solo debe contener números. \n");
            }

            if (String.IsNullOrEmpty(drpSitioNoPopUp.SelectedValue))
            {
                sbErrors.Append(@"*El campo (Sitio) es requerido. \n");
            }

            if (txtFechaInicioNoPopUp.Text == string.Empty || txtFechaInicioNoPopUp.Text == "")
            {
                sbErrors.Append(@"*El campo (Fecha inicio) es requerido. \n");
            }

            if (!validaFormatoFecha(txtFechaInicioNoPopUp.Text))
            {
                sbErrors.Append(@"*El formato de (Fecha inicio) debe ser DD/MM/AAAA. \n");
                txtFechaInicioCenCos.Text = string.Empty;
            }

            if (String.IsNullOrEmpty(drpTipoExtenNoPopUp.SelectedValue))
            {
                sbErrors.Append(@"*El campo (Tipo de extensión) es requerido. \n");
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
                sbQuery.AppendLine("and Exten = '" + iCodCatalogoExtension + "'");

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

        public void edicionDeExtension()
        {
            DataRow drExtension = ExisteLaExtension(txtExtension.Text, drpSitio.Text);

            string iCodCatalogoExten = drExtension["iCodCatalogo"].ToString();

            if (txtExtension.Text != "" && drpSitio.Text != "" && txtFechaInicio.Text != "" && drpTipoExten.Text != "")
            {
                #region La extensión a dar de alta es principal y el empleado ya cuenta con una extensión principal

                DataRow ldr = validaTipoExtensionEnEdicion();

                //AM.20131205 Se valida si el empleado cuenta con una extensión principal
                if ((drpTipoExten.SelectedItem.ToString() == "EXTENSIÓN PRINCIPAL" || drpTipoExten.SelectedIndex == 0) && ldr != null &&
                    ldr["vchCodigo"].ToString() != txtExtension.Text)
                {
                    mensajeDeAdvertencia("El empleado ya cuenta con una extensión principal, favor de seleccionar otro tipo de extensión.");
                    mpeExten.Show();
                }
                #endregion

                else
                {
                    #region proceso edicion

                    DateTime dtFechaInicio = Convert.ToDateTime(txtFechaInicio.Text.ToString());

                    //Si la fecha de inicio es mayor o igual a la fecha de alta del empleado entra a este bloque
                    if (validaFechaInicioExten(dtFechaInicio))
                    {
                        string fechaInicioValida = DSODataAccess.ExecuteScalar("ValidaHistoriaRecurso   @iCodRecurso = " + iCodCatalogoExten + "," +
                                                                               "                        @fechaweb = '" + dtFechaInicio.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," +
                                                                               "                        @iCodRegistroRel = " + txtRegistroRelacion.Text.ToString() + "," +
                                                                               "                        @nombreCampoiCodRecurso  = 'Exten'," +
                                                                               "                        @RelacionTripleComilla = '''Empleado - Extension'''").ToString();

                        //La fecha de inicio es valida?  -------------------- AM. 20130711
                        if (fechaInicioValida == "1")
                        {
                            string fechaFinValida = DSODataAccess.ExecuteScalar("ValidaHistoriaRecurso   @iCodRecurso = " + iCodCatalogoExten + "," +
                                                                   "                        @fechaweb = '" + Convert.ToDateTime(txtFechaFinExten.Text.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," +
                                                                   "                        @iCodRegistroRel = " + txtRegistroRelacion.Text.ToString() + "," +
                                                                   "                        @nombreCampoiCodRecurso  = 'Exten'," +
                                                                   "                        @RelacionTripleComilla = '''Empleado - Extension'''").ToString();

                            //La fecha de fin es valida?  -------------------- AM. 20130715
                            if (fechaFinValida == "1")
                            {
                                if (Convert.ToDateTime(txtFechaFinExten.Text.ToString()) >= Convert.ToDateTime(txtFechaInicio.Text.ToString()))
                                {

                                    DALCCustodia dalCCust = new DALCCustodia();
                                    dalCCust.editExten(drpVisibleDir.Text.ToString(), txtFechaInicio.Text.ToString(), txtFechaFinExten.Text.ToString(),
                                                       iCodCatalogoExten, drpTipoExten.Text.ToString(), txtComentariosExten.Text.ToString(), iCodCatalogoEmple);
                                }

                                //La fecha de fin no es valida
                                else
                                {
                                    mensajeDeAdvertencia("La fecha fin debe ser mayor a la fecha de inicio, favor de seleccionar una fecha valida.");
                                    mpeExten.Show();
                                    txtFechaFinExten.Text = "";
                                    txtFechaFinExten.Focus();
                                }
                            }

                            else
                            {
                                mensajeDeAdvertencia("La fecha fin que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                                mpeExten.Show();
                                txtFechaFinExten.Text = "";
                                txtFechaFinExten.Focus();
                            }
                        }

                        //La fecha de inicio no es valida
                        else
                        {
                            mensajeDeAdvertencia("La fecha inicio que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                            mpeExten.Show();
                            txtFechaInicio.Text = "";
                            txtFechaInicio.Focus();
                        }
                    }


                    //Si la fecha de inicio es menor a la fecha de alta del empleado manda un mensaje de notificación
                    else
                    {
                        mensajeDeAdvertencia("La fecha de inicio debe ser mayor a la fecha de alta del empleado");
                        mpeExten.Show();
                    }
                    #endregion //Fin de bloque -- proceso de edicion
                }

            }

            else
            {
                mensajeDeAdvertencia("Los campos Extension, Sitio, Fecha Inicial y Tipo son requeridos.");
                mpeExten.Show();
            }
        }

        public void bajaDeExtension()
        {
            if (txtFechaFinExten.Text != "")
            {
                //Query para obtener iCodCatalogo de la extension
                DataRow drExtension = ExisteLaExtension(txtExtension.Text, drpSitio.Text);

                string iCodCatalogoExten = drExtension["iCodCatalogo"].ToString();

                string fechaFinValida = DSODataAccess.ExecuteScalar("ValidaHistoriaRecurso   @iCodRecurso = " + iCodCatalogoExten + "," +
                                           "                        @fechaweb = '" + Convert.ToDateTime(txtFechaFinExten.Text.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," +
                                           "                        @iCodRegistroRel = " + txtRegistroRelacion.Text.ToString() + "," +
                                           "                        @nombreCampoiCodRecurso  = 'Exten'," +
                                           "                        @RelacionTripleComilla = '''Empleado - Extension'''").ToString();

                //La fecha de fin es valida?  -------------------- AM. 20130715
                if (fechaFinValida == "1" && Convert.ToDateTime(txtFechaFinExten.Text.ToString()) >= Convert.ToDateTime(txtFechaInicio.Text.ToString()))
                {
                    DALCCustodia dalCCust = new DALCCustodia();
                    dalCCust.bajaExten(iCodCatalogoExten, iCodCatalogoEmple, Convert.ToDateTime(txtFechaFinExten.Text));
                }

                //La Fecha de Fin no es valida
                else
                {
                    mensajeDeAdvertencia("La fecha fin que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                    mpeExten.Show();
                    txtFechaFinExten.Text = "";
                    txtFechaFinExten.Focus();
                }
            }

            else
            {
                mensajeDeAdvertencia("Favor de seleccionar una Fecha de Fin Valida.");
                mpeExten.Show();
                txtFechaInicio.Text = "";
                txtFechaInicio.Focus();
            }

        }

        //Inicializa los valores de todos los campos de PopUpExten en "null"
        public void limpiaCamposDePopUpExten()
        {
            txtExtension.Text = null;
            drpSitio.Text = null;
            txtFechaInicio.Text = null;
            drpTipoExten.Text = null;
            drpVisibleDir.Text = null;
            txtComentariosExten.Text = null;
            cbRangoExten.Checked = false;
        }

        public void limpiaCamposNoPopUpExten()
        {
            txtExtensionNoPopUp.Text = null;
            drpSitioNoPopUp.Text = null;
            txtFechaInicioNoPopUp.Text = null;
            drpTipoExtenNoPopUp.Text = null;
            drpVisibleDirNoPopUp.Text = null;
            txtComentariosExtenNoPopUp.Text = null;
            cbRangoExtenNoPopUp.Checked = false;
        }

        //Carga los valores de la propiedades de los controles del pop-up de Extensiones al Agregar
        public void cargaPropControlesAlAgregarExten()
        {
            txtExtension.Enabled = true;
            txtExtension.ReadOnly = false;
            drpSitio.Enabled = true;
            txtFechaInicio.Enabled = true;
            txtFechaInicio.ReadOnly = false;
            drpTipoExten.Enabled = true;
            drpVisibleDir.Enabled = true;
            txtComentariosExten.Enabled = true;
            txtComentariosExten.ReadOnly = false;
            cbRangoExten.Visible = true;
            lblRangoExten.Visible = true;
            lblFechaFinExten.Visible = false;
            txtFechaFinExten.Visible = false;
            txtFechaFinExten.Enabled = false;

            //Se cambian los titulos del pop-up de extensiones y el texto del boton
            lblTituloPopUpExten.Text = "Detalle de extensión";
            btnGuardarExten.Text = "Guardar";

            //Controles para indicar el proceso de agregar nueva extension
            cbEditarExtension.Checked = false;
            cbBajaExtension.Checked = false;
        }


        //Carga los valores de la propiedades de los controles del pop-up de Extensiones al Editar
        public void cargaPropControlesAlEditarExten()
        {
            //Controles de pop-up
            txtExtension.Enabled = false;
            txtExtension.ReadOnly = true;
            drpSitio.Enabled = false;
            txtFechaInicio.Enabled = true;
            txtFechaInicio.ReadOnly = false;
            drpTipoExten.Enabled = true;
            drpVisibleDir.Enabled = true;
            txtComentariosExten.Enabled = true;
            cbRangoExten.Visible = false;
            lblRangoExten.Visible = false;
            lblFechaFinExten.Visible = true;
            txtFechaFinExten.Visible = true;
            txtFechaFinExten.Enabled = true;

            //Se cambian los titulos del pop-up de extensiones y el texto del boton
            lblTituloPopUpExten.Text = "Detalle de extensión";
            btnGuardarExten.Text = "Guardar";

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
            txtFechaInicio.Enabled = false;
            txtFechaInicio.ReadOnly = true;
            drpTipoExten.Enabled = false;
            drpVisibleDir.Enabled = false;
            txtComentariosExten.Enabled = false;
            txtComentariosExten.ReadOnly = true;
            cbRangoExten.Visible = false;
            lblRangoExten.Visible = false;
            lblFechaFinExten.Visible = true;
            txtFechaFinExten.Visible = true;
            txtFechaFinExten.Enabled = true;

            //Se cambia la leyenda del pop-up
            lblTituloPopUpExten.Text = "¿Esta seguro que desea dar de baja la extensión ?";
            btnGuardarExten.Text = "Eliminar";

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
            int iCodTipoExten = (int)grvExten.DataKeys[rowIndex].Values[2];
            int iCodRegistroRelacion = (int)grvExten.DataKeys[rowIndex].Values[3];

            //Query para extraer valores de los controles del pop-up de Extensiones
            StringBuilder sbExtensionQuery = new StringBuilder();
            sbExtensionQuery.AppendLine("select Comentarios from [VisHistoricos('ExtenB','Extensiones B','Español')]");
            sbExtensionQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
            sbExtensionQuery.AppendLine("and dtfinvigencia >= getdate()");
            sbExtensionQuery.AppendLine("and Exten = " + iCodExten);
            DataRow drExtension = DSODataAccess.ExecuteDataRow(sbExtensionQuery.ToString());

            //Se llenan los controles del pop-up
            txtExtension.Text = selectedRow.Cells[3].Text;
            drpSitio.Text = iCodSitio.ToString();
            txtFechaInicio.Text = selectedRow.Cells[5].Text;
            txtFechaFinExten.Text = selectedRow.Cells[6].Text;
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
            int iCodTipoExten = (int)grvExten.DataKeys[rowIndex].Values[2];
            int iCodRegistroRelacion = (int)grvExten.DataKeys[rowIndex].Values[3];

            //Query para extraer valores de los controles del pop-up de Extensiones
            StringBuilder sbExtensionQuery = new StringBuilder();
            sbExtensionQuery.AppendLine("select Comentarios from [VisHistoricos('ExtenB','Extensiones B','Español')]");
            sbExtensionQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
            sbExtensionQuery.AppendLine("and dtfinvigencia >= getdate()");
            sbExtensionQuery.AppendLine("and Exten = " + iCodExten);
            DataRow drExtension = DSODataAccess.ExecuteDataRow(sbExtensionQuery.ToString());

            //Se llenan los controles del pop-up
            txtExtension.Text = selectedRow.Cells[3].Text;
            drpSitio.Text = iCodSitio.ToString();
            txtFechaInicio.Text = selectedRow.Cells[5].Text;
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


            cargaPropControlesAlBorrarExten();
            mpeExten.Show();
        }

        #endregion

        #region Codigos de Autorizacion

        //Agregar filas al grid de Codigos 
        protected void btnAgregarCodAuto_Click(object sender, EventArgs e)
        {
            btnGuardarCodAuto.Enabled = true;
            cargaPropControlesAlAgregarCodAuto();
            mpeCodAuto.Show();
            limpiaCamposDePopUpCodAuto();
        }

        protected void btnGuardar_PopUpCodAuto(object sender, EventArgs e)
        {
            btnGuardarCodAuto.Enabled = false;
            try
            {
                string tipoABC = string.Empty;
                //Si se llega al pop-up mediante dar clic en el boton de edicion entonces llamara al proceso de edicion 
                if (cbEditarCodAuto.Checked == true)
                {
                    if (validaFormatoFecha(txtFechaInicioCodAuto.Text.ToString()))
                    {
                        if (validaFormatoFecha(txtFechaFinCodAuto.Text.ToString()))
                        {
                            edicionDeCodAuto();

                            tipoABC = "C";
                        }

                        else
                        {
                            mensajeDeAdvertencia("Formato de Fecha Fin incorrecto, favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA");
                            txtFechaFinCodAuto.Text = "";
                            txtFechaFinCodAuto.Focus();
                            mpeCodAuto.Show();
                        }
                    }

                    else
                    {
                        mensajeDeAdvertencia("Formato de Fecha Inicio incorrecto, favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA");
                        txtFechaInicioCodAuto.Text = "";
                        txtFechaInicioCodAuto.Focus();
                        mpeCodAuto.Show();
                    }

                }

                //Si se llega al pop-up mediante dar clic en el boton de baja entonces llamara al proceso de baja 
                else
                {
                    if (cbBajaCodAuto.Checked == true)
                    {
                        if (validaFormatoFecha(txtFechaInicioCodAuto.Text.ToString()))
                        {
                            if (validaFormatoFecha(txtFechaFinCodAuto.Text.ToString()))
                            {
                                bajaDeCodAuto();
                                tipoABC = "B";
                            }

                            else
                            {
                                mensajeDeAdvertencia("Formato de Fecha Fin incorrecto, favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA");
                                txtFechaFinCodAuto.Text = "";
                                txtFechaFinCodAuto.Focus();
                                mpeCodAuto.Show();
                            }
                        }

                        else
                        {
                            mensajeDeAdvertencia("Formato de Fecha Inicio incorrecto, favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA");
                            txtFechaInicioCodAuto.Text = "";
                            txtFechaInicioCodAuto.Focus();
                            mpeCodAuto.Show();
                        }


                    }

                    //Si se llega al pop-up mediante dar clic en el boton de agregar entonces llamara al proceso de alta de un codigo de autorizacion
                    else
                    {
                        if (validaFormatoFecha(txtFechaInicioCodAuto.Text.ToString()))
                        {
                            procesoAltaDeCodAuto();
                            tipoABC = "A";
                        }

                        else
                        {
                            mensajeDeAdvertencia("Formato de Fecha Inicio incorrecto, favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA");
                            txtFechaInicioCodAuto.Text = "";
                            txtFechaInicioCodAuto.Focus();
                            mpeCodAuto.Show();
                        }

                    }
                }

                StringBuilder sbCodAuto = new StringBuilder();
                sbCodAuto.AppendLine("select iCodCatalogo from [VisHistoricos('CodAuto','Codigo Autorizacion','Español')]");
                sbCodAuto.AppendLine("where dtinivigencia<>dtfinvigencia");
                sbCodAuto.AppendLine("and dtfinvigencia >= getdate()");
                sbCodAuto.AppendLine("and vchCodigo = '" + txtCodAuto.Text + "'");
                sbCodAuto.AppendLine("and Sitio = " + drpSitioCodAuto.Text);
                DataRow drCodAuto = DSODataAccess.ExecuteDataRow(sbCodAuto.ToString());

                DALCCustodia dalCC = new DALCCustodia();
                dalCC.guardaHistRecurso(drCodAuto["iCodCatalogo"].ToString(), "CodAut", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), tipoABC);

                CambiarEstatusCCust(1);
                dtCodAuto.Clear();
                FillCodAutoGrid();
                mensajeDeAdvertencia("Se completo el proceso correctamente.");
            }

            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al intentar guardar el codigo de autorización '" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        protected void lbtnGuardar_CodAutoNoPopUp(object sender, EventArgs e)
        {
            try
            {
                string tipoABC = string.Empty;

                if (validaFormatoFecha(txtFechaInicioCodAutoNoPopUp.Text.ToString()))
                {
                    procesoAltaDeCodAutoNoPopUp();
                    tipoABC = "A";
                }

                else
                {
                    mensajeDeAdvertencia("Formato de Fecha Inicio incorrecto, favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA");
                    txtFechaInicioCodAutoNoPopUp.Text = "";
                    txtFechaInicioCodAutoNoPopUp.Focus();
                }

                StringBuilder sbCodAuto = new StringBuilder();
                sbCodAuto.AppendLine("select iCodCatalogo from [VisHistoricos('CodAuto','Codigo Autorizacion','Español')]");
                sbCodAuto.AppendLine("where dtinivigencia<>dtfinvigencia");
                sbCodAuto.AppendLine("and dtfinvigencia >= getdate()");
                sbCodAuto.AppendLine("and vchCodigo = '" + txtCodAutoNoPopUp.Text + "'");
                sbCodAuto.AppendLine("and Sitio = " + drpSitioCodAutoNoPopUp.Text);
                DataRow drCodAuto = DSODataAccess.ExecuteDataRow(sbCodAuto.ToString());

                DALCCustodia dalCC = new DALCCustodia();
                dalCC.guardaHistRecurso(drCodAuto["iCodCatalogo"].ToString(), "CodAut", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), tipoABC);

                CambiarEstatusCCust(1);
                dtCodAuto.Clear();
                FillCodAutoGrid();
                mensajeDeAdvertencia("Se completo el proceso correctamente.");
                limpiaCamposNoPopUpCodAuto();
            }

            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al intentar guardar el codigo de autorización '" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        public void procesoAltaDeCodAuto()
        {
            //Verifica que los campos necesarios contengan información
            if (txtCodAuto.Text != "" && drpSitioCodAuto.SelectedIndex != 0 && txtFechaInicioCodAuto.Text != "")
            {
                //AM. 20130725 Se agrega instrucción de eliminar espacios en blanco en el codigo de autorización
                string codAutoCod = txtCodAuto.Text.ToString().Trim();
                bool flag;
                Int64 liCodAut;
                DateTime dtFechaInicioCodAuto = Convert.ToDateTime(txtFechaInicioCodAuto.Text.ToString());
                string psFechaInicioCodAut = dtFechaInicioCodAuto.ToString("yyyy-MM-dd HH:mm:ss.fff");

                //Se crea un objeto con todos los datos del nuevo codigó de autorización
                DALCCustodia CodAuto = new DALCCustodia();

                //El codigó de autorización es numérico ?
                flag = Int64.TryParse(codAutoCod, out liCodAut);
                if (flag == true)
                {
                    //Query para ver si codigó de autorización ya existe
                    StringBuilder sbExisteCodAutoQuery = new StringBuilder();
                    sbExisteCodAutoQuery.AppendLine("select iCodCatalogo, vchCodigo from [VisHistoricos('CodAuto','Codigo Autorizacion','Español')]");
                    sbExisteCodAutoQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
                    sbExisteCodAutoQuery.AppendLine("and dtfinvigencia >= getdate()");
                    sbExisteCodAutoQuery.AppendLine("and vchCodigo = '" + codAutoCod + "'");
                    sbExisteCodAutoQuery.AppendLine("and Sitio = " + drpSitioCodAuto.Text);
                    DataRow drExisteCodAuto = DSODataAccess.ExecuteDataRow(sbExisteCodAutoQuery.ToString());

                    //El codigó de autorización ya existe ?
                    if (drExisteCodAuto != null)
                    {
                        string lsiCodCatalogoCodAut = drExisteCodAuto["iCodCatalogo"].ToString();

                        //Query para ver si el codigó de autorización ya tiene una relación con otro empleado.
                        StringBuilder sbRelEmpCodAutQuery = new StringBuilder();
                        sbRelEmpCodAutQuery.AppendLine("select EmpleDesc from [VisRelaciones('Empleado - CodAutorizacion','Español')]");
                        sbRelEmpCodAutQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
                        sbRelEmpCodAutQuery.AppendLine("and dtfinvigencia >= getdate()");
                        sbRelEmpCodAutQuery.AppendLine("and CodAuto = " + lsiCodCatalogoCodAut);
                        sbRelEmpCodAutQuery.AppendLine("and CodAutoCod = '" + codAutoCod + "'");
                        DataRow drRelEmpCodAut = DSODataAccess.ExecuteDataRow(sbRelEmpCodAutQuery.ToString());

                        string fechaInicioValida = DSODataAccess.ExecuteScalar("ValidaHistoriaRecurso   @iCodRecurso = " + lsiCodCatalogoCodAut + "," +
                                                                                                           "                        @fechaweb = '" + dtFechaInicioCodAuto.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," +
                                                                                                           "                        @iCodRegistroRel = null," +
                                                                                                           "                        @nombreCampoiCodRecurso  = 'CodAuto'," +
                                                                                                           "                        @RelacionTripleComilla = '''Empleado - CodAutorizacion'''").ToString();

                        //La fecha de inicio es valida?  -------------------- AM. 20130711
                        if (fechaInicioValida == "1")
                        {
                            //El codigó de autorización esta asignado a otro empleado ?
                            if (drRelEmpCodAut != null)
                            {
                                string nombreEmpleRel = drRelEmpCodAut["EmpleDesc"].ToString();

                                TextInfo miInfo = CultureInfo.CurrentCulture.TextInfo;
                                int inicioDeParentesis = nombreEmpleRel.IndexOf("(") + 1;
                                char[] parentesis = { ')', '(' };
                                string nombreEmpleado = nombreEmpleRel.Substring(0, inicioDeParentesis).TrimEnd(parentesis).Trim();

                                mensajeDeAdvertencia("El codigó de autorización que selecciono ya esta asignada al empleado : " + miInfo.ToTitleCase(nombreEmpleado.ToLower()));


                                mpeCodAuto.Show();
                                txtCodAuto.Text = "";
                                txtCodAuto.Focus();
                            }

                            //Si el codigó de autorización no tiene relacion entra a este bloque para dar de Alta la relación 'Empleado - CodigoAutorizacion'
                            else
                            {
                                string lsiCodCatalgoCodAuto = drExisteCodAuto["iCodCatalogo"].ToString();
                                string lsVchCodCodAuto = drExisteCodAuto["vchCodigo"].ToString();

                                // Se hace un insert en [VisRelaciones('Empleado - CodAutorizacion','Español')] 
                                // Se hace un update a la vista [VisHistoricos('CodAuto','Codigo Autorizacion','Español')] en el campo Emple 
                                CodAuto.altaRelacionEmpCodAuto(txtNominaEmple.Text.ToString(), lsVchCodCodAuto, iCodCatalogoEmple, lsiCodCatalgoCodAuto, dtFechaInicioCodAuto);

                                mpeCodAuto.Hide();

                            }
                        }

                        //La fecha de inicio no es valida
                        else
                        {

                            mensajeDeAdvertencia("La fecha que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                            mpeCodAuto.Show();

                            txtFechaInicioCodAuto.Text = "";
                            txtFechaInicioCodAuto.Focus();
                        }

                    }

                    //El codigó de autorización no existe, entonces entro a este bloque
                    else
                    {
                        //Query para ver la descripcion del sitio
                        StringBuilder lsbQuery = new StringBuilder();
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
                        lsbQuery.AppendFormat("and H.iCodCatalogo = {0} ", drpSitioCodAuto.Text);
                        DataRow drVchDescSitio = DSODataAccess.ExecuteDataRow(lsbQuery.ToString());

                        string lsSitioDesc = drVchDescSitio["vchDescripcion"].ToString();

                        //Alta de el codigó de autorización
                        //RZ.20131227 Se retira bandera "Visible en directorio" para codigos
                        //drpVisibleDirCodAuto.Text.ToString()
                        CodAuto.altaCodAuto(codAutoCod, lsSitioDesc, drpSitioCodAuto.Text.ToString(), dtFechaInicioCodAuto, "0");

                        //Query para extraer datos del codigo de autorizacion que se acaba de dar de alta
                        StringBuilder sbCodAutoRecienteQuery = new StringBuilder();
                        sbCodAutoRecienteQuery.AppendLine("select iCodCatalogo,vchCodigo from [VisHistoricos('CodAuto','Codigo Autorizacion','Español')]");
                        sbCodAutoRecienteQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
                        sbCodAutoRecienteQuery.AppendLine("and dtfinvigencia >= getdate()");
                        sbCodAutoRecienteQuery.AppendLine("and vchCodigo = '" + codAutoCod + "'");
                        sbCodAutoRecienteQuery.AppendLine("and Sitio = " + drpSitioCodAuto.Text);
                        DataRow drCodAutoReciente = DSODataAccess.ExecuteDataRow(sbCodAutoRecienteQuery.ToString());

                        string lsiCodCatalgoCodAuto = drCodAutoReciente["iCodCatalogo"].ToString();
                        string lsVchCodCodAuto = drCodAutoReciente["vchCodigo"].ToString();

                        // Se hace un insert en [VisRelaciones('Empleado - CodAutorizacion','Español')] 
                        // Se hace un update a la vista [VisHistoricos('CodAuto','Codigo Autorizacion','Español')] en el campo Emple 
                        CodAuto.altaRelacionEmpCodAuto(txtNominaEmple.Text.ToString(), lsVchCodCodAuto, iCodCatalogoEmple, lsiCodCatalgoCodAuto, dtFechaInicioCodAuto);

                        mpeCodAuto.Hide();
                    }
                }

                //El codigó de autorización no es numerico y manda mensaje de error
                else
                {

                    mensajeDeAdvertencia("El codigó de autorización debe ser numérico y no debe contener espacios en blanco");
                    mpeCodAuto.Show();
                    txtCodAuto.Text = "";
                    txtCodAuto.Focus();
                }
            }

            //Si los campos requeridos no tienen informacion, mostrara un mensaje indicando que los campos son necesarios para continuar
            else
            {
                mensajeDeAdvertencia("Los campos Codigó de autorización, Sitio, Fecha son requeridos.");
                mpeCodAuto.Show();
            }
        }

        public void procesoAltaDeCodAutoNoPopUp()
        {
            //Verifica que los campos necesarios contengan información
            if (txtCodAutoNoPopUp.Text != "" && drpSitioCodAutoNoPopUp.SelectedIndex != 0 && txtFechaInicioCodAutoNoPopUp.Text != "")
            {
                //AM. 20130725 Se agrega instrucción de eliminar espacios en blanco en el codigo de autorización
                string codAutoCod = txtCodAutoNoPopUp.Text.ToString().Trim();
                bool flag;
                Int64 liCodAut;
                DateTime dtFechaInicioCodAuto = Convert.ToDateTime(txtFechaInicioCodAutoNoPopUp.Text.ToString());
                string psFechaInicioCodAut = dtFechaInicioCodAuto.ToString("yyyy-MM-dd HH:mm:ss.fff");

                //Se crea un objeto con todos los datos del nuevo codigó de autorización
                DALCCustodia CodAuto = new DALCCustodia();

                //El codigó de autorización es numérico ?
                flag = Int64.TryParse(codAutoCod, out liCodAut);
                if (flag == true)
                {
                    //Query para ver si codigó de autorización ya existe
                    StringBuilder sbExisteCodAutoQuery = new StringBuilder();
                    sbExisteCodAutoQuery.AppendLine("select iCodCatalogo, vchCodigo from [VisHistoricos('CodAuto','Codigo Autorizacion','Español')]");
                    sbExisteCodAutoQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
                    sbExisteCodAutoQuery.AppendLine("and dtfinvigencia >= getdate()");
                    sbExisteCodAutoQuery.AppendLine("and vchCodigo = '" + codAutoCod + "'");
                    sbExisteCodAutoQuery.AppendLine("and Sitio = " + drpSitioCodAutoNoPopUp.Text);
                    DataRow drExisteCodAuto = DSODataAccess.ExecuteDataRow(sbExisteCodAutoQuery.ToString());

                    //El codigó de autorización ya existe ?
                    if (drExisteCodAuto != null)
                    {
                        string lsiCodCatalogoCodAut = drExisteCodAuto["iCodCatalogo"].ToString();

                        //Query para ver si el codigó de autorización ya tiene una relación con otro empleado.
                        StringBuilder sbRelEmpCodAutQuery = new StringBuilder();
                        sbRelEmpCodAutQuery.AppendLine("select EmpleDesc from [VisRelaciones('Empleado - CodAutorizacion','Español')]");
                        sbRelEmpCodAutQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
                        sbRelEmpCodAutQuery.AppendLine("and dtfinvigencia >= getdate()");
                        sbRelEmpCodAutQuery.AppendLine("and CodAuto = " + lsiCodCatalogoCodAut);
                        sbRelEmpCodAutQuery.AppendLine("and CodAutoCod = '" + codAutoCod + "'");
                        DataRow drRelEmpCodAut = DSODataAccess.ExecuteDataRow(sbRelEmpCodAutQuery.ToString());

                        string fechaInicioValida = DSODataAccess.ExecuteScalar("ValidaHistoriaRecurso   @iCodRecurso = " + lsiCodCatalogoCodAut + "," +
                                                                                                           "                        @fechaweb = '" + dtFechaInicioCodAuto.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," +
                                                                                                           "                        @iCodRegistroRel = null," +
                                                                                                           "                        @nombreCampoiCodRecurso  = 'CodAuto'," +
                                                                                                           "                        @RelacionTripleComilla = '''Empleado - CodAutorizacion'''").ToString();

                        //La fecha de inicio es valida?  -------------------- AM. 20130711
                        if (fechaInicioValida == "1")
                        {
                            //El codigó de autorización esta asignado a otro empleado ?
                            if (drRelEmpCodAut != null)
                            {
                                string nombreEmpleRel = drRelEmpCodAut["EmpleDesc"].ToString();

                                TextInfo miInfo = CultureInfo.CurrentCulture.TextInfo;
                                int inicioDeParentesis = nombreEmpleRel.IndexOf("(") + 1;
                                char[] parentesis = { ')', '(' };
                                string nombreEmpleado = nombreEmpleRel.Substring(0, inicioDeParentesis).TrimEnd(parentesis).Trim();

                                mensajeDeAdvertencia("El codigó de autorización que selecciono ya esta asignada al empleado : " + miInfo.ToTitleCase(nombreEmpleado.ToLower()));

                                txtCodAutoNoPopUp.Text = "";
                                txtCodAutoNoPopUp.Focus();
                            }

                            //Si el codigó de autorización no tiene relacion entra a este bloque para dar de Alta la relación 'Empleado - CodigoAutorizacion'
                            else
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
                                    CodAuto.altaRelacionEmpCodAuto(txtNominaEmple.Text.ToString(), lsVchCodCodAuto, iCodCatalogoEmple, lsiCodCatalgoCodAuto, dtFechaInicioCodAuto);
                                }
                                //Si la fecha de inicio es menor a la fecha de alta del empleado manda un mensaje de notificación
                                else
                                {
                                    mensajeDeAdvertencia("La fecha de inicio debe ser mayor o igual a la fecha de alta del empleado.");
                                }
                            }
                        }

                        //La fecha de inicio no es valida
                        else
                        {
                            mensajeDeAdvertencia("La fecha que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");

                            txtFechaInicioCodAutoNoPopUp.Text = "";
                            txtFechaInicioCodAutoNoPopUp.Focus();
                        }

                    }

                    //El codigó de autorización no existe, entonces entro a este bloque
                    else
                    {
                        //Query para ver la descripcion del sitio
                        StringBuilder lsbQuery = new StringBuilder();
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
                        lsbQuery.AppendFormat("and H.iCodCatalogo = {0} ", drpSitioCodAutoNoPopUp.Text);
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
                            //RZ.20131227 Se retira bandera "Visible en directorio" para codigos
                            //drpVisibleDirCodAuto.Text.ToString()
                            CodAuto.altaCodAuto(codAutoCod, lsSitioDesc, drpSitioCodAutoNoPopUp.Text.ToString(), dtFechaInicioCodAuto, "0");

                            //Query para extraer datos del codigo de autorizacion que se acaba de dar de alta
                            StringBuilder sbCodAutoRecienteQuery = new StringBuilder();
                            sbCodAutoRecienteQuery.AppendLine("select iCodCatalogo,vchCodigo from [VisHistoricos('CodAuto','Codigo Autorizacion','Español')]");
                            sbCodAutoRecienteQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
                            sbCodAutoRecienteQuery.AppendLine("and dtfinvigencia >= getdate()");
                            sbCodAutoRecienteQuery.AppendLine("and vchCodigo = '" + codAutoCod + "'");
                            sbCodAutoRecienteQuery.AppendLine("and Sitio = " + drpSitioCodAutoNoPopUp.Text);
                            DataRow drCodAutoReciente = DSODataAccess.ExecuteDataRow(sbCodAutoRecienteQuery.ToString());

                            string lsiCodCatalgoCodAuto = drCodAutoReciente["iCodCatalogo"].ToString();
                            string lsVchCodCodAuto = drCodAutoReciente["vchCodigo"].ToString();

                            // Se hace un insert en [VisRelaciones('Empleado - CodAutorizacion','Español')] 
                            // Se hace un update a la vista [VisHistoricos('CodAuto','Codigo Autorizacion','Español')] en el campo Emple 
                            CodAuto.altaRelacionEmpCodAuto(txtNominaEmple.Text.ToString(), lsVchCodCodAuto, iCodCatalogoEmple, lsiCodCatalgoCodAuto, dtFechaInicioCodAuto);
                        }

                         //Si la fecha de inicio es menor a la fecha de alta del empleado manda un mensaje de notificación
                        else
                        {
                            mensajeDeAdvertencia("La fecha de inicio debe ser mayor o igual a la fecha de alta del empleado.");
                        }
                    }
                }

                //El codigó de autorización no es numerico y manda mensaje de error
                else
                {

                    mensajeDeAdvertencia("El codigó de autorización debe ser numérico y no debe contener espacios en blanco");
                    txtCodAutoNoPopUp.Text = "";
                    txtCodAutoNoPopUp.Focus();
                }
            }

            //Si los campos requeridos no tienen informacion, mostrara un mensaje indicando que los campos son necesarios para continuar
            else
            {
                mensajeDeAdvertencia("Los campos Codigó de autorización, Sitio, Fecha son requeridos.");
            }
        }

        public void edicionDeCodAuto()
        {
            //Query para extraer el iCodCatalogo del codigo de autorización
            StringBuilder sbCodAutoQuery = new StringBuilder();
            sbCodAutoQuery.AppendLine("select iCodCatalogo from [VisHistoricos('CodAuto','Codigo Autorizacion','Español')]");
            sbCodAutoQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
            sbCodAutoQuery.AppendLine("and dtfinvigencia >= getdate()");
            sbCodAutoQuery.AppendLine("and vchCodigo = '" + txtCodAuto.Text + "'");
            sbCodAutoQuery.AppendLine("and Sitio = " + drpSitioCodAuto.Text);
            DataRow drCodAuto = DSODataAccess.ExecuteDataRow(sbCodAutoQuery.ToString());

            string iCodCatalogoCodAuto = drCodAuto["iCodCatalogo"].ToString();

            if (txtCodAuto.Text != "" && drpSitioCodAuto.Text != ""
                && txtFechaInicioCodAuto.Text != ""
                //RZ.20131227 Se retira bandera "Visible en directorio" para codigos
                //&& drpVisibleDirCodAuto.Text != ""
                )
            {
                string lsdtFechaInicioEmple = DSODataAccess.ExecuteScalar("select dtIniVigencia from [VisHistoricos('Emple','Empleados','Español')] " +
                                                               "where iCodCatalogo = " + iCodCatalogoEmple +
                                                               "and dtIniVigencia<>dtFinVigencia and dtFinVigencia >= getdate()").ToString();

                DateTime dtFechaInicioEmple = Convert.ToDateTime(lsdtFechaInicioEmple);

                DateTime dtFechaInicio = Convert.ToDateTime(txtFechaInicioCodAuto.Text.ToString());


                //Si la fecha de inicio es mayor o igual a la fecha de alta del empleado entra a este bloque
                if (dtFechaInicio >= dtFechaInicioEmple)
                {
                    string fechaInicioValida = DSODataAccess.ExecuteScalar("ValidaHistoriaRecurso   @iCodRecurso = " + iCodCatalogoCodAuto + "," +
                                                                                                        "                        @fechaweb = '" + dtFechaInicio.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," +
                                                                                                        "                        @iCodRegistroRel = " + txtRegistroRelacionCodAuto.Text.ToString() + "," +//txtRegistroRelacionCodAuto
                                                                                                        "                        @nombreCampoiCodRecurso  = 'CodAuto'," +
                                                                                                        "                        @RelacionTripleComilla = '''Empleado - CodAutorizacion'''").ToString();

                    //La fecha de inicio es valida?  -------------------- AM. 20130711
                    if (fechaInicioValida == "1")
                    {
                        string fechaFinValida = DSODataAccess.ExecuteScalar("ValidaHistoriaRecurso   @iCodRecurso = " + iCodCatalogoCodAuto + "," +
                                       "                        @fechaweb = '" + Convert.ToDateTime(txtFechaFinCodAuto.Text.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," +
                                       "                        @iCodRegistroRel = " + txtRegistroRelacionCodAuto.Text.ToString() + "," +
                                       "                        @nombreCampoiCodRecurso  = 'CodAuto'," +
                                       "                        @RelacionTripleComilla = '''Empleado - CodAutorizacion'''").ToString();

                        //La fecha de fin es valida?  -------------------- AM. 20130715
                        if (fechaFinValida == "1")
                        {
                            if (Convert.ToDateTime(txtFechaFinCodAuto.Text.ToString()) >= Convert.ToDateTime(txtFechaInicioCodAuto.Text.ToString()))
                            {
                                DALCCustodia dalCCust = new DALCCustodia();
                                //RZ.20131227 Se retira bandera "Visible en directorio" para codigos
                                //drpVisibleDirCodAuto.Text.ToString()
                                dalCCust.editCodAuto("0", txtFechaInicioCodAuto.Text.ToString(),
                                                     txtFechaFinCodAuto.Text.ToString(), iCodCatalogoCodAuto, iCodCatalogoEmple);
                            }

                            //La fecha de fin no es valida
                            else
                            {
                                mensajeDeAdvertencia("La fecha fin debe ser mayor a la fecha de inicio, favor de seleccionar una fecha valida.");
                                mpeCodAuto.Show();
                                txtFechaFinCodAuto.Text = "";
                                txtFechaFinCodAuto.Focus();
                            }
                        }

                        else
                        {
                            mensajeDeAdvertencia("La fecha fin que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                            mpeCodAuto.Show();
                            txtFechaFinCodAuto.Text = "";
                            txtFechaFinCodAuto.Focus();
                        }

                    }

                    //La fecha de inicio no es valida
                    else
                    {


                        mensajeDeAdvertencia("La fecha que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                        mpeCodAuto.Show();
                        txtFechaInicioCodAuto.Text = "";
                        txtFechaInicioCodAuto.Focus();
                    }
                }

                //Si la fecha de inicio es menor a la fecha de alta del empleado manda un mensaje de notificación
                else
                {


                    mensajeDeAdvertencia("La fecha de inicio debe ser mayor a la fecha de alta del empleado.");

                    mpeCodAuto.Show();
                }

            }

            else
            {

                //RZ.20131227 Se retira bandera "Visible en directorio" para codigos
                mensajeDeAdvertencia("Los campos Codigó de autorización, Sitio, Fecha son requeridos.");
                mpeCodAuto.Show();
            }
        }

        public void bajaDeCodAuto()
        {
            if (txtFechaFinCodAuto.Text != "")
            {
                //Query para extraer el iCodCatalogo del codigo de autorización
                StringBuilder sbCodAutoQuery = new StringBuilder();
                sbCodAutoQuery.AppendLine("select iCodCatalogo from [VisHistoricos('CodAuto','Codigo Autorizacion','Español')]");
                sbCodAutoQuery.AppendLine("where dtinivigencia<>dtfinvigencia");
                sbCodAutoQuery.AppendLine("and dtfinvigencia >= getdate()");
                sbCodAutoQuery.AppendLine("and vchCodigo = '" + txtCodAuto.Text + "'");
                sbCodAutoQuery.AppendLine("and Sitio = " + drpSitioCodAuto.Text);
                DataRow drCodAuto = DSODataAccess.ExecuteDataRow(sbCodAutoQuery.ToString());

                string iCodCatalogoCodAuto = drCodAuto["iCodCatalogo"].ToString();

                string fechaFinValida = DSODataAccess.ExecuteScalar("ValidaHistoriaRecurso   @iCodRecurso = " + iCodCatalogoCodAuto + "," +
                                                                    "                        @fechaweb = '" + Convert.ToDateTime(txtFechaFinCodAuto.Text.ToString()).ToString("yyyy-MM-dd HH:mm:ss") + "'" + "," +
                                                                    "                        @iCodRegistroRel = " + txtRegistroRelacionCodAuto.Text.ToString() + "," +
                                                                    "                        @nombreCampoiCodRecurso  = 'CodAuto'," +
                                                                    "                        @RelacionTripleComilla = '''Empleado - CodAutorizacion'''").ToString();

                //La fecha de fin es valida?  -------------------- AM. 20130715
                if (fechaFinValida == "1" && Convert.ToDateTime(txtFechaFinCodAuto.Text.ToString()) >= Convert.ToDateTime(txtFechaInicioCodAuto.Text.ToString()))
                {
                    DALCCustodia dalCCust = new DALCCustodia();
                    //dalCCust.fechaFinCodAuto = Convert.ToDateTime(txtFechaFinCodAuto.Text).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    dalCCust.bajaCodAuto(iCodCatalogoCodAuto, iCodCatalogoEmple, Convert.ToDateTime(txtFechaFinCodAuto.Text));
                }

                //La Fecha de Fin no es valida
                else
                {
                    mensajeDeAdvertencia("La fecha fin que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                    mpeCodAuto.Show();
                    txtFechaFinCodAuto.Text = "";
                    txtFechaFinCodAuto.Focus();
                }
            }
            else
            {
                mensajeDeAdvertencia("Favor de seleccionar una Fecha de Fin Valida.");
                mpeCodAuto.Show();
                txtFechaInicioCodAuto.Text = "";
                txtFechaInicioCodAuto.Focus();
            }
        }

        //Inicializa los valores de todos los campos de PopUpExten en "null"
        public void limpiaCamposDePopUpCodAuto()
        {
            txtCodAuto.Text = null;
            drpSitioCodAuto.Text = null;
            txtFechaInicioCodAuto.Text = null;
            //RZ.20131227 Se retira bandera "Visible en directorio" para codigos
            //drpVisibleDirCodAuto.Text = null;
        }

        public void limpiaCamposNoPopUpCodAuto()
        {
            txtCodAutoNoPopUp.Text = null;
            drpSitioCodAutoNoPopUp.Text = null;
            txtFechaInicioCodAutoNoPopUp.Text = null;
        }

        public void cargaPropControlesAlAgregarCodAuto()
        {
            //Controles de pop-up
            txtCodAuto.Enabled = true;
            txtCodAuto.ReadOnly = false;
            drpSitioCodAuto.Enabled = true;
            txtFechaInicioCodAuto.Enabled = true;
            txtFechaInicioCodAuto.ReadOnly = false;
            //RZ.20131227 Se retira bandera "Visible en directorio" para codigos
            //drpVisibleDirCodAuto.Enabled = true;
            //drpVisibleDirCodAuto.Visible = true;
            lblFechaFinCodAuto.Visible = false;
            txtFechaFinCodAuto.Visible = false;
            txtFechaFinCodAuto.Enabled = false;

            //Se cambian los titulos del pop-up de codigos y el texto del boton
            lblTituloPopUpCodAuto.Text = "Detalle de códigos";
            btnGuardarCodAuto.Text = "Guardar";

            //Controles para indicar el proceso de agregar el nuevo codigo de autorizacion
            cbEditarCodAuto.Checked = false;
            cbBajaCodAuto.Checked = false;

        }


        public void cargaPropControlesAlEditarCodAuto()
        {
            //Controles de pop-up
            txtCodAuto.Enabled = false;
            txtCodAuto.ReadOnly = true;
            drpSitioCodAuto.Enabled = false;
            txtFechaInicioCodAuto.Enabled = true;
            txtFechaInicioCodAuto.ReadOnly = false;
            //RZ.20131227 Se retira bandera "Visible en directorio" para codigos
            //drpVisibleDirCodAuto.Visible = true;
            //lblVisibleDirCodAuto.Visible = true;
            //drpVisibleDirCodAuto.Enabled = true;
            lblFechaFinCodAuto.Visible = true;
            txtFechaFinCodAuto.Visible = true;
            txtFechaFinCodAuto.Enabled = true;

            //Se cambian los titulos del pop-up de codigos y el texto del boton
            lblTituloPopUpCodAuto.Text = "Detalle de códigos";
            btnGuardarCodAuto.Text = "Guardar";

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
            txtFechaInicioCodAuto.Enabled = false;
            txtFechaInicioCodAuto.ReadOnly = true;
            //RZ.20131227 Se retira bandera "Visible en directorio" para codigos
            //drpVisibleDirCodAuto.Visible = true;
            //lblVisibleDirCodAuto.Visible = true;
            //drpVisibleDirCodAuto.Enabled = false;
            lblFechaFinCodAuto.Visible = true;
            txtFechaFinCodAuto.Visible = true;
            txtFechaFinCodAuto.Enabled = true;

            //Se cambia la leyenda del pop-up
            lblTituloPopUpCodAuto.Text = "¿Esta seguro que desea dar de baja el código de autorización ?";
            btnGuardarCodAuto.Text = "Eliminar";

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
            int iCodRegRelEmpCodAuto = (int)grvCodAuto.DataKeys[rowIndex].Values[2];

            //Se llenan los controles del pop-up
            txtCodAuto.Text = selectedRow.Cells[0].Text;
            drpSitioCodAuto.Text = iCodSitio.ToString();
            txtFechaInicioCodAuto.Text = selectedRow.Cells[2].Text;
            txtFechaFinCodAuto.Text = selectedRow.Cells[3].Text;
            txtRegistroRelacionCodAuto.Text = iCodRegRelEmpCodAuto.ToString();
            //RZ.20131227 Se retira bandera "Visible en directorio" para codigos
            //string lsVisDirCodAuto = DSODataAccess.ExecuteScalar("select CONVERT(bit,ISNULL(BanderasCodAuto,0)) from [VisHistoricos('CodAuto','Codigo Autorizacion','Español')] " +
            //                                                        "where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate() " +
            //                                                        "and iCodCatalogo =  " + iCodCodAut).ToString();
            //if (lsVisDirCodAuto == "True")
            //{
            //    drpVisibleDirCodAuto.Text = "1";
            //}
            //else
            //{
            //    drpVisibleDirCodAuto.Text = "0";
            //}

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
            int iCodRegRelEmpCodAuto = (int)grvCodAuto.DataKeys[rowIndex].Values[2];

            //Se llenan los controles del pop-up
            txtCodAuto.Text = selectedRow.Cells[0].Text;
            drpSitioCodAuto.Text = iCodSitio.ToString();
            txtFechaInicioCodAuto.Text = selectedRow.Cells[2].Text;
            txtFechaFinCodAuto.Text = string.Empty;
            txtRegistroRelacionCodAuto.Text = iCodRegRelEmpCodAuto.ToString();
            //RZ.20131227 Se retira bandera "Visible en directorio" para codigos
            //string lsVisDirCodAuto = DSODataAccess.ExecuteScalar("select CONVERT(bit,ISNULL(BanderasCodAuto,0)) from [VisHistoricos('CodAuto','Codigo Autorizacion','Español')] " +
            //                                                        "where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate() " +
            //                                                        "and iCodCatalogo =  " + iCodCodAut.ToString()).ToString();
            //if (lsVisDirCodAuto == "True")
            //{
            //    drpVisibleDirCodAuto.Text = "1";
            //}
            //else
            //{
            //    drpVisibleDirCodAuto.Text = "0";
            //}

            cargaPropControlesAlBorrarCodAuto();
            mpeCodAuto.Show();
        }

        #endregion

        #region Inventario

        //Agregar filas al grid de inventario
        /* RZ.20131204 Se retira agregar inventario desde modalpopup
        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            ccdMarcaPopUp.SelectedValue = ccdMarcaPopUp.PromptValue;
            ccdModelo.SelectedValue = ccdModelo.PromptValue;

            cargaPropControlesAlAgregarInventario();
            mpeInventario.Show();
            limpiaCamposDePopUpInventario();
        }
        */
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

        /*RZ.20131204 Se retira la alta de inventario desde modalpopup*/
        /*
        public void procesoAltaInventario()
        {
            //Verifica que los campos necesarios contengan información
            if (drpMarcaPopUp.Text != "" && txtTipoAparatoPopUp.Text != "" && txtNoSeriePopUp.Text != "" && txtMacAddressPopUp.Text != "")
            {
                DateTime dtFechaInicioRelDispEmple = DateTime.Today;

                string lsNoSerieText = txtNoSeriePopUp.Text.ToString();
                int IniIcodCatalogoDisp = lsNoSerieText.IndexOf("(") + 1;
                char[] MyChar = { ')', '(' };
                string lsiCodCatalogoDispositivo = lsNoSerieText.Substring(IniIcodCatalogoDisp).TrimEnd(MyChar).Trim();
                string lsNoSerie = lsNoSerieText.Substring(0, IniIcodCatalogoDisp).TrimEnd(MyChar).Trim();

                //Se crea un objeto para dar de alta la relacion Dispositivo - Empleado
                DALCCustodia Dispositivo = new DALCCustodia();

                Dispositivo.altaRelacionEmpDispositivo(lsiCodCatalogoDispositivo, lsNoSerie, txtNominaEmple.Text.ToString(), iCodCatalogoEmple, dtFechaInicioRelDispEmple);
                limpiaCamposDePopUpInventario();
            }

            else
            {
                mensajeDeAdvertencia("Los campos Marca, Tipo de aparato, No. de Serie y Mac Address son requeridos.");
                mpeInventario.Show();
            }

        }

        /*20131204 Se retira edicion de inventario*/
        /*
        public void edicionDeInventario()
        {
            StringBuilder sbiCodDisp = new StringBuilder();
            sbiCodDisp.AppendLine("select iCodCatalogo from [VisHistoricos('Dispositivo','Inventario de dispositivos','Español')]");
            sbiCodDisp.AppendLine("where dtinivigencia<>dtfinvigencia");
            sbiCodDisp.AppendLine("and dtfinvigencia >= getdate()");
            sbiCodDisp.AppendLine("and NSerie = '" + txtNoSeriePopUp.Text.ToString() + "'");
            DataRow driCodDisp = DSODataAccess.ExecuteDataRow(sbiCodDisp.ToString());

            string iCodDisp = driCodDisp["iCodCatalogo"].ToString();

            if (txtMacAddressPopUp.Text != "")
            {

                DALCCustodia dalCCust = new DALCCustodia();
                dalCCust.editInventario(txtMacAddressPopUp.Text.ToString(), iCodDisp);

            }

            else
            {
                mensajeDeAdvertencia("El campo MAC ADDRESS es requerido.");
                mpeCodAuto.Show();
            }
        }
        */

        public void bajaDeInventario()
        {
            //Query para ver si la extensión ya tiene una relación con otro empleado.
            StringBuilder sbiCodDisp = new StringBuilder();
            sbiCodDisp.AppendLine("select iCodCatalogo from [VisHistoricos('Dispositivo','Inventario de dispositivos','Español')]");
            sbiCodDisp.AppendLine("where dtinivigencia<>dtfinvigencia");
            sbiCodDisp.AppendLine("and dtfinvigencia >= getdate()");
            sbiCodDisp.AppendLine("and NSerie = '" + txtNoSeriePopUp.Text.ToString() + "'");
            DataRow driCodDisp = DSODataAccess.ExecuteDataRow(sbiCodDisp.ToString());

            string iCodDisp = driCodDisp["iCodCatalogo"].ToString();

            DALCCustodia dalCCust = new DALCCustodia();
            //RZ.20130805
            dalCCust.bajaInventario(iCodDisp, iCodCatalogoEmple, DateTime.Today);

            mensajeDeAdvertencia("Inventario: El equipo ha sido desvinculado del empleado correctamente ");

        }

        //Inicializa los valores de todos los campos de PopUpExten en "null"
        //RZ.20131204 Se retira metodo ya que no se esta haciendo uso de esta funcionalidad
        /*public void limpiaCamposDePopUpInventario()
        {
            //drpMarcaPopUp.Text = null;
            //drpModeloPopUp.Text = null;
            txtTipoAparatoPopUp.Text = null;
            txtNoSeriePopUp.Text = null;
            txtMacAddressPopUp.Text = null;
        }
        */
        /* RZ.20131204 Se retira agregar inventario desde modalpopup
        public void cargaPropControlesAlAgregarInventario()
        {
            //Controles de pop-up
            drpMarcaPopUp.Enabled = true;
            drpModeloPopUp.Enabled = true;
            txtTipoAparatoPopUp.Enabled = false;
            txtTipoAparatoPopUp.ReadOnly = true;
            txtNoSeriePopUp.Enabled = true;
            txtNoSeriePopUp.ReadOnly = false;
            txtMacAddressPopUp.Enabled = true;
            txtMacAddressPopUp.ReadOnly = false;

            //Se cambian los titulos del pop-up de inventario y el texto del boton
            lblTituloPopUp.Text = "Detalle de inventario";
            btnGuardarPopUp.Text = "Guardar";

            //Controles para indicar el proceso de agregar el nuevo dispositivo
            //cbEditarInventario.Checked = false;
            cbBajaInventario.Checked = false;
        }
        */

        /*RZ.20131204 Se retira la edicion de inventario*/
        /*
        public void cargaPropControlesAlEditarInventario()
        {
            //Controles de pop-up
            drpMarcaPopUp.Enabled = false;
            drpModeloPopUp.Enabled = false;
            txtTipoAparatoPopUp.Enabled = false;
            txtTipoAparatoPopUp.ReadOnly = true;
            txtNoSeriePopUp.Enabled = false;
            txtNoSeriePopUp.ReadOnly = true;
            txtMacAddressPopUp.Enabled = true;
            txtMacAddressPopUp.ReadOnly = false;

            //Se cambian los titulos del pop-up de inventario y el texto del boton
            lblTituloPopUp.Text = "Detalle de inventario";
            btnGuardarPopUp.Text = "Guardar";

            //Controles para indicar el proceso de edicion
            cbEditarInventario.Checked = true;

        }
        */

        /*RZ.20131204 Se retira este metodo ya que no es necesario debdio a que el modalpopup sera solo para baja*/
        /*
        public void cargaPropControlesAlBorrarInventario()
        {
            //Controles de pop-up
            //drpMarcaPopUp.Enabled = false;
            //drpModeloPopUp.Enabled = false;
            txtTipoAparatoPopUp.Enabled = false;
            txtTipoAparatoPopUp.ReadOnly = true;
            txtNoSeriePopUp.Enabled = false;
            txtNoSeriePopUp.ReadOnly = true;
            txtMacAddressPopUp.Enabled = false;
            txtMacAddressPopUp.ReadOnly = true;

            //Se cambian los titulos del pop-up de inventario y el texto del boton
            lblTituloPopUp.Text = "¿Esta seguro que desea dar de baja la relación de este dispositivo con este empleado?";
            btnGuardarPopUp.Text = "Eliminar";

            //Controles para indicar el proceso de edicion
            //RZ.20131204 Se retira checbox
            //cbBajaInventario.Checked = true;

        }*/

        //RZ.20131204 Se retira la edicion de inventario
        //Editar fila inventario
        /*
        protected void grvInventario_EditRow(object sender, ImageClickEventArgs e)
        {
            ImageButton ibtn3 = sender as ImageButton;
            int rowIndex = Convert.ToInt32(ibtn3.Attributes["RowIndex"]);

            //Use this rowIndex in your code 

            GridViewRow selectedRow = (GridViewRow)grvInventario.Rows[rowIndex];

            int iCodMarca = (int)grvInventario.DataKeys[rowIndex].Values[0];
            int iCodModelo = (int)grvInventario.DataKeys[rowIndex].Values[1];

            //Se llenan los controles del pop-up
            ccdMarcaPopUp.SelectedValue = iCodMarca.ToString();
            ccdModelo.SelectedValue = iCodModelo.ToString();
            txtTipoAparatoPopUp.Text = selectedRow.Cells[2].Text;
            txtNoSeriePopUp.Text = selectedRow.Cells[3].Text;
            txtMacAddressPopUp.Text = selectedRow.Cells[4].Text;

            cargaPropControlesAlEditarInventario();
            mpeInventario.Show();
        }
        */

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


        #endregion

        #region Botones y Metodos de logica de negocios

        protected void btnEnviarCCustodiaEmple_Click(object sender, EventArgs e)
        {
            ////La carta esta en estatus pendiente?
            //if (txtEstatusCCustodia.Text == "PENDIENTE")
            //{
            string iCodCCust = DSODataAccess.ExecuteScalar("select iCodCatalogo from [VisHistoricos('CCustodia','Cartas custodia','Español')] " +
                                                           "where FolioCCustodia = " + txtFolioCCustodia.Text.ToString() +
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

                mensajeDeAdvertencia("Ya existe un envio programado por enviar para esta carta custodia.");

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

                    DateTime fechaUltAct = Convert.ToDateTime(txtUltimaMod.Text.ToString());

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
                        mensajeDeAdvertencia("Se ha programado un envío de la carta custodia al empleado");
                    }
                }

                else
                {
                    programaEnvioCCust();
                    //RZ.20131202 Notificar que ya se programo un envio
                    mensajeDeAdvertencia("Se ha programado un envío de la carta custodia al empleado");
                }
            }
            //}

            //else
            //{

            //    //Mensaje de warning para cambiar estatus a pendiente
            //    string script = @"<script type='text/javascript'>alerta(""Debe cambiar el estatus de la carta custodia a 'PENDIENTE' antes de enviar la carta custodia."");</script>";

            //    ScriptManager.RegisterStartupScript(this, typeof(Page), "alerta", script, false);

            //}
        }

        protected void btnAceptarEnvioCCust_ConfEnvio(object sender, EventArgs e)
        {
            programaEnvioCCust();
            //RZ.20131202 Notificar que ya se programo un reenvio
            mensajeDeAdvertencia("Se ha programado un reenvío de la carta custodia");
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
                mensajeDeAdvertencia("Por favor, especifique el motivo del rechazo de la carta usando el campo de comentarios.");
            }
        }

        protected void CambiarEstatusCCust(int Estatus)
        {
            try
            {
                string iCodEstatusCCust = DSODataAccess.ExecuteScalar("select iCodCatalogo from [VisHistoricos('EstCCustodia','Estatus CCustodia','Español')]" +
                                                                      " where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= getdate() and Value = " + Estatus).ToString();

                string iCodCCust = DSODataAccess.ExecuteScalar("select iCodCatalogo from [VisHistoricos('CCustodia','Cartas custodia','Español')] " +
                                              " where FolioCCustodia = " + txtFolioCCustodia.Text.ToString() +
                                              " and dtIniVigencia<>dtFinVigencia and dtFinVigencia >= getdate()").ToString();

                //RZ.20130715 Se agrega update de comentarios de admin desde lo que contenga el textbox txtComentariosAdmin
                DSODataAccess.ExecuteNonQuery("update [VisHistoricos('CCustodia','Cartas custodia','Español')] " +
                                              " set EstCCustodia = " + iCodEstatusCCust + "," + "ComentariosEmple = '" + txtComenariosEmple.Text.ToString() + "', ComentariosAdmin = '" + txtComentariosAdmin.Text + "', dtFecUltAct = getdate()" +
                                              " where FolioCCustodia = " + txtFolioCCustodia.Text.ToString() +
                                              " and dtIniVigencia<>dtFinVigencia and dtFinVigencia >= getdate()");

                DataTable dtEmple = new DataTable();
                dtEmple = cargaDatosEmple(iCodCatalogoEmple);
                FillDatosEmple(dtEmple);
                upDatosEmple.Update();
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al intentar cambiar el estatus de la carta con folio '" + txtFolioCCustodia.Text.ToString() + "'", ex);
                throw ex;
            }
        }

        protected void programaEnvioCCust()
        {
            string iCodCCust = DSODataAccess.ExecuteScalar("select iCodCatalogo from [VisHistoricos('CCustodia','Cartas custodia','Español')] " +
                              "where FolioCCustodia = " + txtFolioCCustodia.Text.ToString() +
                              "and dtIniVigencia<>dtFinVigencia and dtFinVigencia >= getdate()").ToString();

            //Se inserta un registro en [VisDetallados('Detall','Bitacora Envio CCustodia','Español')]
            DALCCustodia dalCCust = new DALCCustodia();
            dalCCust.InsertRegEnBitacoraEnvioCCust(txtFolioCCustodia.Text.ToString(), iCodCCust, txtEmailEmple.Text.ToString());
        }

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
            lbtnDeleteEmple.Text = "[ Borrar ]";

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
            lbtnDeleteEmple.Text = "[ Aceptar ]";

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
            lbtnDeleteEmple.Text = "[ Borrar ]";

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
            enableDDL(drpLocalidadEmple);
            enableTxtBox(txtEmailEmple);
            enableTxtBox(txtRadioNextelEmple);
            enableTxtBox(txtUsuarRedEmple);
            enableTxtBox(txtNumCelularEmple);
            enableDDL(drpJefeEmple);
            //enableTxtBox(txtEmailJefeEmple);
            enableCheckBox(cbEsGerenteEmple);
            enableCheckBox(cbVisibleDirEmple);
            enableDDL(drpSitioEmple);
            enableDDL(drpPuestoEmple);
            enableDDL(drpEmpresaEmple);
            enableDDL(drpTipoEmpleado);
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
                lWord.FilePath = System.IO.Path.Combine(lsStylePath, @"plantillas\CartasCustodia\PlantillaCartaCustodia.docx");
                lWord.Abrir();
                lWord.XmlPalettePath = System.IO.Path.Combine(lsStylePath, @"chart.xml");

                #region inserta logos
                string lsKeytiaWebFPath = HttpContext.Current.Server.MapPath("~");
                string lsImg;
                DataRow pRowCliente = DSODataAccess.ExecuteDataRow("select LogoExportacion from [vishistoricos('client','clientes','español')] " +
                                                    " where usuardb = " + DSODataContext.GetContext() +
                                                    " and dtinivigencia <> dtfinVigencia " +
                                                    " and dtfinVigencia>getdate()");

                //NZ 20150508 Se cambio el nombre de la columna a la cual ira a buscar el logo del cliente para exportacion. pRowCliente["Logo"] POR pRowCliente["LogoExportacion"]
                //lsImg = System.IO.Path.Combine(lsKeytiaWebFPath, pRowCliente["Logo"].ToString().Replace("~/", ""));
                lsImg = System.IO.Path.Combine(lsKeytiaWebFPath, pRowCliente["LogoExportacion"].ToString().Replace("~/", ""));
                if (System.IO.File.Exists(lsImg))
                {
                    //lWord.ReemplazarTextoPorImagen("{LogoCliente}", lsImg);
                    lWord.PosicionaCursor("{LogoCliente}");
                    lWord.ReemplazarTexto("{LogoCliente}", "");
                    lWord.InsertarImagen(lsImg, 131, 40);
                }
                else
                {
                    lWord.ReemplazarTexto("{LogoCliente}", "");
                }
                /* //LogoKeytia
                 * lsImg = System.IO.Path.Combine(lsStylePath, @"images\KeytiaHeader.png");
                if (System.IO.File.Exists(lsImg))
                {
                    lWord.ReemplazarTextoPorImagen("{LogoKeytia}", lsImg);
                }
                else
                {
                    lWord.ReemplazarTexto("{LogoKeytia}", "");
                }
                 */
                #endregion

                //Obtener datos en datatable
                #region creaDatatables
                DataTable dtInventario = GetDataTable(grvInventario);
                if (dtInventario.Rows.Count > 0)
                {
                    if (dtInventario.Columns.Contains("&nbsp;"))
                        dtInventario.Columns.Remove("&nbsp;");
                    if (dtInventario.Columns.Contains("&nbsp;6"))
                        dtInventario.Columns.Remove("&nbsp;6");
                    if (dtInventario.Columns.Contains("fecha inicial"))
                        dtInventario.Columns.Remove("fecha inicial");
                    if (dtInventario.Columns.Contains("fecha final"))
                        dtInventario.Columns.Remove("fecha final");
                    if (dtInventario.Columns.Contains("fecha fin"))
                        dtInventario.Columns.Remove("fecha fin");
                    //RZ.20131227 Se retira parte de edicion de inventario
                    //dtInventario.Columns.Remove("Editar");
                    dtInventario.Columns.Remove("Borrar");
                }
                DataTable dtExtensiones = GetDataTable(grvExten);
                if (dtExtensiones.Rows.Count > 0)
                {
                    if (dtExtensiones.Columns.Contains("&nbsp;"))
                        dtExtensiones.Columns.Remove("&nbsp;");
                    if (dtExtensiones.Columns.Contains("&nbsp;1"))
                        dtExtensiones.Columns.Remove("&nbsp;1");
                    if (dtExtensiones.Columns.Contains("&nbsp;2"))
                        dtExtensiones.Columns.Remove("&nbsp;2");
                    if (dtExtensiones.Columns.Contains("&nbsp;10"))
                        dtExtensiones.Columns.Remove("&nbsp;10");
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
                DataTable dtIDsUsuarios = GetDataTable(grvUsuarios);
                if (dtIDsUsuarios.Rows.Count > 0)
                {
                    if (dtIDsUsuarios.Columns.Contains("&nbsp;"))
                        dtIDsUsuarios.Columns.Remove("&nbsp;");
                    dtIDsUsuarios.Columns.Remove("Editar");
                    dtIDsUsuarios.Columns.Remove("Borrar");
                }

                #endregion


                #region datos emple

                lWord.ReemplazarTexto("{Fecha}", txtFecha.Text);
                lWord.ReemplazarTexto("{Folio}", txtFolioCCustodia.Text);
                lWord.ReemplazarTexto("{Estatus}", txtEstatusCCustodia.Text);
                lWord.ReemplazarTexto("{Nomina}", txtNominaEmple.Text);
                lWord.ReemplazarTexto("{Nombre}", txtNombreEmple.Text);
                lWord.ReemplazarTexto("{SegNombre}", txtSegundoNombreEmple.Text);
                lWord.ReemplazarTexto("{ApPaterno}", txtApPaternoEmple.Text);
                lWord.ReemplazarTexto("{ApMaterno}", txtApMaternoEmple.Text);
                lWord.ReemplazarTexto("{Ubicacion}", drpSitioEmple.SelectedItem.Text);
                lWord.ReemplazarTexto("{Empresa}", drpEmpresaEmple.SelectedItem.Text);
                lWord.ReemplazarTexto("{TipoEmple}", drpTipoEmpleado.SelectedItem.Text);
                lWord.ReemplazarTexto("{Cencos}", drpCenCosEmple.SelectedItem.Text);
                lWord.ReemplazarTexto("{Puesto}", drpPuestoEmple.SelectedItem.Text);
                lWord.ReemplazarTexto("{Localidad}", drpLocalidadEmple.SelectedItem.Text);
                lWord.ReemplazarTexto("{Email}", txtEmailEmple.Text);
                lWord.ReemplazarTexto("{RadioNextel}", txtRadioNextelEmple.Text);
                lWord.ReemplazarTexto("{usuario}", txtUsuarRedEmple.Text);
                lWord.ReemplazarTexto("{Celular}", txtNumCelularEmple.Text);
                lWord.ReemplazarTexto("{Jefe}", drpJefeEmple.SelectedItem.Text);
                lWord.ReemplazarTexto("{EmailJefe}", txtEmailJefeEmple.Text);


                #endregion


                lWord.PosicionaCursor("{InventarioEquipos}");
                lWord.ReemplazarTexto("{InventarioEquipos}", "");
                if (dtInventario.Rows.Count > 0)
                    lWord.InsertarTabla(dtInventario, true);
                else
                    lWord.InsertarTexto("El empleado no cuenta con inventario asignado");


                lWord.PosicionaCursor("{Extensiones}");
                lWord.ReemplazarTexto("{Extensiones}", "");
                if (dtExtensiones.Rows.Count > 0)
                {
                    lWord.InsertarTabla(dtExtensiones, true);
                    lWord.Tabla.Columns.AutoFit();
                }
                else
                    lWord.InsertarTexto("El empleado no cuenta con extensiones asignadas");


                lWord.PosicionaCursor("{Codigos}");
                lWord.ReemplazarTexto("{Codigos}", "");
                if (dtCodigos.Rows.Count > 0)
                    lWord.InsertarTabla(dtCodigos, true);
                else
                    lWord.InsertarTexto("El empleado no cuenta con codigos asignados");


                lWord.PosicionaCursor("{IDUsuarios}");
                lWord.ReemplazarTexto("{IDUsuarios}", "");
                if (dtIDsUsuarios.Rows.Count > 0)
                    lWord.InsertarTabla(dtIDsUsuarios, true);
                else
                    lWord.InsertarTexto("El empleado no cuenta con ID's de usuario asignados");


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
        protected void mensajeDeAdvertencia(string mensaje)
        {
            string mensajeJQuery = "<p>" + mensaje + "</p>";
            string script = @"<script type='text/javascript'>alerta('" + mensajeJQuery + "');</script>";
            ScriptManager.RegisterStartupScript(this, typeof(Page), "alerta", script, false);
            btnGuardarExten.Enabled = true;
            btnGuardarCodAuto.Enabled = true;
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

            //if (regex.IsMatch(Fecha))
            //{
            //    return fechaValida = true;
            //}

            //return fechaValida;
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
                    dalCCust.AddPuesto(txtPuestoDesc.Text.Trim());
                    drpPuestoEmple.Items.Clear();
                    drpPuestoEmple.Items.Add("-- Selecciona uno --");
                    FillDDLPuestoEmple();
                    drpPuestoEmple.Items.FindByText(txtPuestoDesc.Text.ToString()).Selected = true;
                    txtPuestoDesc.Text = string.Empty;
                }

                else
                {
                    mensajeDeAdvertencia("El puesto que intenta dar de alta ya existe en el sistema");
                    mpeAddPuesto.Show();
                    txtPuestoDesc.Focus();
                }
            }

            else
            {
                mensajeDeAdvertencia("Debe capturar una descripción del puesto");
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
            StringBuilder lsbQuery = new StringBuilder();

            lsbQuery.Append("SELECT iCodCatalogo, vchDescripcion \r");
            lsbQuery.Append("FROM [VisHistoricos('CenCos','Centro de Costos','Español')] \r");
            lsbQuery.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
            lsbQuery.Append("and dtFinVigencia >= GETDATE() \r");
            lsbQuery.Append("order by vchDescripcion");

            dtCenCosResponsable = DSODataAccess.Execute(lsbQuery.ToString());

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
            StringBuilder lsbQuery = new StringBuilder();

            lsbQuery.Append("SELECT iCodCatalogo, vchDescripcion = NomCompleto \r");
            lsbQuery.Append("FROM [VisHistoricos('Emple','Empleados','Español')] \r");
            lsbQuery.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
            lsbQuery.Append("and dtFinVigencia >= GETDATE() \r");
            lsbQuery.Append("and NomCompleto <> '' \r");
            lsbQuery.Append("and vchDescripcion not like '%identif%' \r");
            lsbQuery.Append("order by NomCompleto");

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
                    mensajeDeAdvertencia(validaCampos);
                    txtClaveCenCos.Focus();
                    mpeAddCenCos.Show();
                }

                else
                {
                    string validaVigencias = validaVigenciasResponsablesCenCos();

                    //Se valida que la fecha de inicio sea mayor a las fechas de inicio del CenCos y Empleado responsables.
                    if (validaVigencias.Length > 0)
                    {
                        mensajeDeAdvertencia(validaVigencias);
                        txtFechaInicioCenCos.Focus();
                        mpeAddCenCos.Show();
                    }

                    //Se valida que no exista ya un centro de costos con el número o descripción proporcionados.
                    else
                    {
                        string validaClaveDescNoDup = validaClaveYDescNoDuplicados();

                        if (validaClaveDescNoDup.Length > 0)
                        {
                            mensajeDeAdvertencia(validaClaveDescNoDup);
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
            string iCodCatalogoEmpresa = GetIcodCatalogoEmpresa("Nextel");
            string vchCodigoCenCosResp = GetvchCodCenCosResp(drpCenCosResponsable.Text);

            Hashtable lht = new Hashtable();
            lht.Add("iCodMaestro", 18); //iCodMaestro
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
            if (txtComentariosAdmin.Text.Length <= 250) //Si execede estos caracteres no se podran guardar cambios.
            {
                DALCCustodia.actualizaComentAdmin(iCodCatalogoEmple, txtComentariosAdmin.Text, Session["iCodUsuario"].ToString());
            }
            else 
            {
                mensajeDeAdvertencia("Los comentarios no pueden exceder los 250 caracteres.");
            }
        }

        //RZ.20131203 Se agrega boton para busqueda de numeros de series
        protected void lbtnBuscaNoSerie_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlMarca.SelectedValue) || string.IsNullOrEmpty(ddlModelo.SelectedValue))
            {
                mensajeDeAdvertencia("Debe seleccionar una marca y modelo para filtrar números de series.");
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

                //mensajeDeAdvertencia("Ocurrio un error al intentar guardar la relacion del Dispositivo - Empleado '");
                //20150330 NZ De la línea de arriba que se comento se quito la comilla simple que se escuenta despues de "Empleado" por que causa errores en el navegador evitando que salga el mensaje. Se rompen los parentesis angulares.
                mensajeDeAdvertencia("Ocurrio un error al intentar guardar la relacion del Dispositivo - Empleado");
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
                    txtNominaEmple.Text.ToString(), iCodCatalogoEmple, dtFechaInicioRelDispEmple);

                //RZ.20131204 Se agrega update en la macAddress en cuando se asigna inventario
                Dispositivo.editInventario(txtMACAddress.Text, hdnfDispositivo.Value);

                CambiarEstatusCCust(1);
                //Actualizar la grid en cada postback
                dtInventarioAsignado.Clear();
                FillInventarioGrid();

                clearControlsAddInventario(true);
                mensajeDeAdvertencia("Inventario: El equipo ha sido asignado correctamente al empleado");
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

            //20140516 AM. Se cambia condición para que regrese false en caso de encontrar una relación vigente, ademas se 
            //agrega mensaje que indica a quien esta asignado el dispositivo.
            //if (liCant == 0)
            //{
            //    lbResult = true;
            //    return lbResult;
            //}
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

                mensajeDeAdvertencia("El dispositivo que seleccionó ya esta asignado al empleado : " + miInfo.ToTitleCase(nombreEmpleado.ToLower()));

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
                mensajeDeAdvertencia("Debe seleccionar una marca y modelo válido");
                lbResult = false;
                return lbResult;
            }

            if (txtNoSerie.Text == "" || string.IsNullOrEmpty(hdnfDispositivo.Value))
            {
                mensajeDeAdvertencia("Debe seleccionar un numero de serie válido, en la sección " + lbtnBuscaNoSerie.Text);
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

            mensajeDeAdvertencia("El número de serie seleccionado no disponible para asignar al empleado");
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


        #region Id Usuarios - Empleado

        protected void btnAgregarUsuario_Click(object sender, EventArgs e)
        {
            btnGuardarUsuario.Enabled = true;
            CargaPropControlesAlAgregarUsuario();
            mpeUsuarios.Show();
            limpiaCamposDePopUpUsuarios();
        }

        protected void btnGuardarUsuario_Click(object sender, EventArgs e)
        {
            btnGuardarUsuario.Enabled = false;
            try
            {
                string tipoABC = string.Empty;
                string resultValid = ValidarCamposPopUp();
                //Si se llega al pop-up mediante dar clic en el boton de edicion entonces llamara al proceso de edicion 
                if (cbEditarUsuario.Checked == true)
                {
                    if (string.IsNullOrEmpty(resultValid))
                    {
                        EdicionDeUsuario();
                        tipoABC = "C";
                    }
                    else
                    {
                        mensajeDeAdvertencia(resultValid);
                        mpeUsuarios.Show();
                    }
                }
                //Si se llega al pop-up mediante dar clic en el boton de baja entonces llamara al proceso de baja 
                else
                {
                    if (cbBajaUsuario.Checked == true)
                    {
                        if (string.IsNullOrEmpty(resultValid))
                        {
                            BajaDeUsuario();
                            tipoABC = "B";
                        }
                        else
                        {
                            mensajeDeAdvertencia(resultValid);
                            mpeUsuarios.Show();
                        }
                    }
                    //Si se llega al pop-up mediante dar clic en el boton de agregar entonces llamara al proceso de alta de un codigo de autorizacion
                    else
                    {
                        if (string.IsNullOrEmpty(resultValid))
                        {
                            ProcesoAltaDeUsuario(true);
                            tipoABC = "A";
                        }
                        else
                        {
                            mensajeDeAdvertencia(resultValid);
                            mpeUsuarios.Show();
                        }
                    }
                }
                btnGuardarUsuario.Enabled = true;
                StringBuilder sbIdUsuario = new StringBuilder();
                sbIdUsuario.AppendLine("select TOP(1) iCodCatalogo from [visHistoricos('NxtIDUsuario','Nxt IDs Usuarios','Español')] \r"); //Se agrega el Top 1 por que cuando es baja puede encontrar mas de uno.
                sbIdUsuario.AppendLine("where dtinivigencia <> dtfinvigencia \r");
                if (tipoABC != "B")
                {
                    sbIdUsuario.AppendLine("and dtfinvigencia >= getdate()  \r");
                }
                else
                {
                    sbIdUsuario.AppendLine("and dtfinvigencia = '" + Convert.ToDateTime(txtFechaFinUsuario.Text).ToString("yyyy-MM-dd HH:mm:ss") + "'  \r");
                }
                sbIdUsuario.AppendLine("and vchCodigo = '" + txtIdUsuario.Text + "' \r");
                sbIdUsuario.AppendLine("and PinVarchar = '" + txtPin.Text + "' \r");
                DataRow drIdUsuario = DSODataAccess.ExecuteDataRow(sbIdUsuario.ToString());

                if (drIdUsuario != null)
                {
                    DALCCustodia idUsuarioCC = new DALCCustodia();
                    idUsuarioCC.guardaHistRecurso(drIdUsuario["iCodCatalogo"].ToString(), "NxtIDsUsuarios", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), tipoABC);
                }

                if (!string.IsNullOrEmpty(tipoABC))
                {
                    CambiarEstatusCCust(1);
                    dtIdUsuarios.Clear();
                    FillUsuariosEmpleGrid();
                    mensajeDeAdvertencia("Se completo el proceso correctamente.");
                }
            }
            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al intentar guardar el ID de Usuario '" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        private string ValidarCamposPopUp()
        {
            double pin = 0;
            string mensajeError = string.Empty;

            if (!validaFormatoCampos(txtIdUsuario.Text) || string.IsNullOrEmpty(txtIdUsuario.Text))
            {
                if (string.IsNullOrEmpty(txtIdUsuario.Text))
                { mensajeError = "El ID de Usuario es requerido."; }
                else { mensajeError = "Formato de ID de Usuario incorrecto, favor de escribir un ID de Usuario alfanumerico."; }
            }
            else if (!validaFormatoFecha(txtFechaIniUsuario.Text) || string.IsNullOrEmpty(txtFechaIniUsuario.Text))
            {
                if (string.IsNullOrEmpty(txtFechaIniUsuario.Text))
                { mensajeError = "La fecha de inicio es requerida."; }
                else { mensajeError = "Formato de Fecha Inicio incorrecto, favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA"; }
            }
            else if (!string.IsNullOrEmpty(txtFechaFinUsuario.Text) && !validaFormatoFecha(txtFechaFinUsuario.Text))
            {
                mensajeError = "Formato de Fecha Fin incorrecto, favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA";
            }
            else if (!double.TryParse(txtPin.Text, out pin) || string.IsNullOrEmpty(txtPin.Text) || pin < 0)
            {
                mensajeError = "Formato de PIN incorrecto, favor de escribir un PIN numerico";
            }
            else if (!string.IsNullOrEmpty(txtFechaFinUsuario.Text))
            {
                if (Convert.ToDateTime(txtFechaIniUsuario.Text) > Convert.ToDateTime(txtFechaFinUsuario.Text))
                {
                    mensajeDeAdvertencia("La fecha fin debe ser mayor a la fecha de inicio, favor de seleccionar una fecha valida. ");
                }
            }
            return mensajeError;
        }

        protected void lbtnGuardarUsuarioNoPopUp_Click(object sender, EventArgs e)
        {
            try
            {
                string tipoABC = string.Empty;

                string mensajeError = string.Empty;
                double pin = 0; ;

                if (!validaFormatoCampos(txtIDUsuarioNoPopUp.Text) || string.IsNullOrEmpty(txtIDUsuarioNoPopUp.Text))
                {
                    if (string.IsNullOrEmpty(txtIDUsuarioNoPopUp.Text))
                    { mensajeError = "El ID de Usuario es requerido."; }
                    else { mensajeError = "Formato de ID de Usuario incorrecto, favor de escribir un ID de Usuario alfanumerico."; }
                }
                else if (!validaFormatoFecha(txtFechaInicioUsuarioNoPopUp.Text) || string.IsNullOrEmpty(txtFechaInicioUsuarioNoPopUp.Text))
                {
                    if (string.IsNullOrEmpty(txtFechaInicioUsuarioNoPopUp.Text))
                    { mensajeError = "La fecha de inicio es requerida."; }
                    else { mensajeError = "Formato de Fecha Inicio incorrecto, favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA"; }
                }
                else if (!string.IsNullOrEmpty(txtFechaFinUsuarioNoPopUp.Text) && !validaFormatoFecha(txtFechaFinUsuarioNoPopUp.Text))
                {
                    mensajeError = "Formato de Fecha Fin incorrecto, favor de seleccionar la fecha desde el calendario o introducir una fecha con formato DD/MM/AAAA";
                }
                else if (!double.TryParse(txtPinNoPopUp.Text, out pin) || string.IsNullOrEmpty(txtPinNoPopUp.Text) || pin < 0)
                {
                    if (string.IsNullOrEmpty(txtPinNoPopUp.Text))
                    { mensajeError = "El PIN es requerido."; }
                    else { mensajeError = "Formato de PIN incorrecto, favor de escribir un PIN numerico"; }
                }
                else if (!string.IsNullOrEmpty(txtFechaFinUsuarioNoPopUp.Text))
                {
                    if (Convert.ToDateTime(txtFechaInicioUsuarioNoPopUp.Text) > Convert.ToDateTime(txtFechaFinUsuarioNoPopUp.Text))
                    {
                        mensajeDeAdvertencia("La fecha fin debe ser mayor a la fecha de inicio, favor de seleccionar una fecha valida. ");
                    }
                }

                if (string.IsNullOrEmpty(mensajeError))
                {
                    ProcesoAltaDeUsuarioNoPopUp();
                    tipoABC = "A";
                }
                else
                {
                    mensajeDeAdvertencia(mensajeError);
                }

                StringBuilder sbUsuarioID = new StringBuilder();
                sbUsuarioID.AppendLine("select iCodCatalogo from [VisHistoricos('NxtIDUsuario','Nxt IDs Usuarios','Español')] ");
                sbUsuarioID.AppendLine("where dtinivigencia <> dtfinvigencia ");
                sbUsuarioID.AppendLine("and dtfinvigencia >= getdate() ");
                sbUsuarioID.AppendLine("and vchCodigo = '" + txtIDUsuarioNoPopUp.Text + "' ");
                sbUsuarioID.AppendLine("and PinVarchar = '" + txtPinNoPopUp.Text + "' ");
                DataRow drIdUsuario = DSODataAccess.ExecuteDataRow(sbUsuarioID.ToString());

                if (drIdUsuario != null)
                {
                    DALCCustodia dalCC = new DALCCustodia();
                    dalCC.guardaHistRecurso(drIdUsuario["iCodCatalogo"].ToString(), "NxtIDsUsuarios", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), tipoABC);
                }

                if (!string.IsNullOrEmpty(tipoABC) && drIdUsuario != null)
                {
                    CambiarEstatusCCust(1);
                    dtIdUsuarios.Clear();
                    FillUsuariosEmpleGrid();
                    mensajeDeAdvertencia("Se completo el proceso correctamente.");
                    limpiaCamposNoPopUpUsuario();
                }
            }

            catch (Exception ex)
            {
                KeytiaServiceBL.Util.LogException("Ocurrio un error al intentar guardar el ID de Usuario '" + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        private string ConsultaValidarFechas(string idUsuario, string fecha, string iCodRegistro, bool isFechaInicio)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT COUNT(*) ");
            query.AppendLine("FROM [VisHistoricos('NxtIDUsuario','Nxt IDs Usuarios','Español')]");
            query.AppendLine("WHERE dtinivigencia <> dtfinvigencia ");
            query.AppendLine("AND dtFinVigencia <> '2079-01-01 00:00:00.000'");
            query.AppendLine("AND iCodRegistro <> " + iCodRegistro); // Descartando el registro actual.
            /////query.AppendLine("AND iCodCatalogo = " + iCodCatalogo);
            query.AppendLine("AND vchCodigo = '" + idUsuario + "'");
            query.AppendLine("AND '" + fecha + "' between dtIniVigencia and dtFinVigencia");
            if (isFechaInicio)
            {
                //ESTA VALIDACION ES POR QUE SE PUEDE DAR DE BAJA UN ELEMENTO PERO EL MISMO DIA PUEDE DARSE DE ALTA DE NUEVO. PARA QUE NO
                //MARQUE EL ERROR DE QUE SE ENCUENTRA DENTRO DEL RANGO DE FECHAS DE OTRO ELEMENTO.
                query.AppendLine("AND '" + fecha + "' <> dtFinVigencia");
            }

            return query.ToString();
        }

        public void ProcesoAltaDeUsuario(bool desdePopUp)
        {
            string idUsuario = string.Empty;
            string pin = string.Empty;
            string fechaInicio = string.Empty;
            string fechaFin = string.Empty;
            string comentarios = string.Empty;

            #region Mapeo de controles
            if (desdePopUp)
            {
                idUsuario = txtIdUsuario.Text.Trim();
                pin = txtPin.Text.Trim();
                fechaInicio = txtFechaIniUsuario.Text;
                fechaFin = txtFechaFinUsuario.Text;
                comentarios = txtComentariosUsuarios.Text.Trim();
            }
            else
            {
                idUsuario = txtIDUsuarioNoPopUp.Text.Trim();
                pin = txtPinNoPopUp.Text.Trim();
                fechaInicio = txtFechaInicioUsuarioNoPopUp.Text;
                fechaFin = txtFechaFinUsuarioNoPopUp.Text;
                comentarios = txtComentariosUsuarioNoPopUp.Text.Trim();
            }
            #endregion Mapeo de controles

            //Logica de negocio//El metodo previo ya valido que los datos sean correctos.        
            #region Validaciones de fechas y pin
            DateTime dtFechaInicio = Convert.ToDateTime(fechaInicio);
            DateTime dtFechaFin;
            DateTime dtFechaMax = new DateTime(2079, 1, 1, 0, 0, 0);
            if (string.IsNullOrEmpty(fechaFin))
            {
                dtFechaFin = dtFechaMax;
            }
            else
            {
                dtFechaFin = Convert.ToDateTime(fechaFin);
                dtFechaFin = (dtFechaFin.Year <= 2078) ? dtFechaFin : dtFechaMax;
            }

            double ipin = 0;
            if (!string.IsNullOrEmpty(pin))
            {
                double.TryParse(pin, out ipin);
            }
            #endregion Validaciones de fechas y pin

            //Validar que el id de usuario no exista vigente en BD.
            StringBuilder sbExisteUsr = new StringBuilder();
            sbExisteUsr.AppendLine("select iCodCatalogo, vchCodigo, EmpleDesc from [VisHistoricos('NxtIDUsuario','Nxt IDs Usuarios','Español')] \r");
            sbExisteUsr.AppendLine("where dtinivigencia <> dtfinvigencia \r");
            sbExisteUsr.AppendLine("and dtfinvigencia >= getdate()");
            sbExisteUsr.AppendLine("and vchCodigo = '" + idUsuario + "' \r");

            DataRow drExisteUsr = DSODataAccess.ExecuteDataRow(sbExisteUsr.ToString());

            //Obtiene la fecha de alta del empleado.
            string lsdtFechaInicioEmple = DSODataAccess.ExecuteScalar("select dtIniVigencia from [VisHistoricos('Emple','Empleados','Español')] " +
                                                               "where iCodCatalogo = " + iCodCatalogoEmple +
                                                               " and dtIniVigencia<>dtFinVigencia and dtFinVigencia >= getdate()").ToString();
            DateTime dtFechaInicioEmple = Convert.ToDateTime(lsdtFechaInicioEmple);

            //Validamos si las fechas proporcionadasson validas. Si la consulta regresa algo, indica que hay periodos que se empalman
            int countFechaIni = (int)DSODataAccess.ExecuteScalar(ConsultaValidarFechas(idUsuario, dtFechaInicio.ToString("yyyy-MM-dd HH:mm:ss"), "0", true));
            int countFechaFin = 0;
            if (dtFechaFin != dtFechaMax)
            {
                countFechaFin = (int)DSODataAccess.ExecuteScalar(ConsultaValidarFechas(idUsuario, dtFechaFin.ToString("yyyy-MM-dd HH:mm:ss"), "0", false));
            }

            if (countFechaIni > 0)
            {
                mensajeDeAdvertencia("La fecha de inicio que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                if (desdePopUp) { mpeUsuarios.Show(); }
            }
            else if (countFechaFin > 0)
            {
                mensajeDeAdvertencia("La fecha fin que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                if (desdePopUp) { mpeUsuarios.Show(); }
            }
            else if (drExisteUsr != null)
            {
                string nombreEmpleRel = drExisteUsr["EmpleDesc"].ToString();

                TextInfo miInfo = CultureInfo.CurrentCulture.TextInfo;
                int inicioDeParentesis = nombreEmpleRel.IndexOf("(") + 1;
                char[] parentesis = { ')', '(' };
                string nombreEmpleado = nombreEmpleRel.Substring(0, inicioDeParentesis).TrimEnd(parentesis).Trim();

                mensajeDeAdvertencia("El Id de Usuario que selecciono ya esta asignado al empleado : " + miInfo.ToTitleCase(nombreEmpleado.ToLower()));
                if (desdePopUp) { mpeUsuarios.Show(); }
            }
            else if (dtFechaInicio < dtFechaInicioEmple)
            {
                mensajeDeAdvertencia("La fecha de inicio debe ser mayor o igual a la fecha de alta del empleado.");
                if (desdePopUp) { mpeUsuarios.Show(); }
            }
            else
            {
                //En historicos se guarda la historia.//Se crea Registro de ID de Usuario 
                DALCCustodia idUsuarioCC = new DALCCustodia();

                idUsuarioCC.AltaIDUsuarios(idUsuario, pin, iCodCatalogoEmple, dtFechaInicio, dtFechaFin, comentarios, "0");
                if (desdePopUp) { mpeUsuarios.Hide(); }
            }
        }

        public void ProcesoAltaDeUsuarioNoPopUp()
        {
            ProcesoAltaDeUsuario(false);
        }

        public void EdicionDeUsuario()
        {
            //Query para extraer el iCodCatalogo del Id de Usuario. //Verificar que solo afecte al activo.
            StringBuilder sbIdUsuario = new StringBuilder();
            sbIdUsuario.AppendLine("select iCodRegistro, iCodCatalogo, vchCodigo from [VisHistoricos('NxtIDUsuario','Nxt IDs Usuarios','Español')] ");
            sbIdUsuario.AppendLine("where dtinivigencia <> dtfinvigencia ");
            sbIdUsuario.AppendLine("and dtfinvigencia >= getdate()");
            sbIdUsuario.AppendLine("and vchCodigo = '" + txtIdUsuario.Text + "'");
            sbIdUsuario.AppendLine("and PinVarchar = '" + Convert.ToDouble(txtPin.Text) + "'");
            DataRow drIdUsuario = DSODataAccess.ExecuteDataRow(sbIdUsuario.ToString());

            string iCodCatalogoIdUsuario = drIdUsuario["iCodCatalogo"].ToString();
            string idUsuario = drIdUsuario["vchCodigo"].ToString();
            string iCodRegistro = drIdUsuario["iCodRegistro"].ToString();

            DateTime dtFechaInicio = Convert.ToDateTime(txtFechaIniUsuario.Text);
            DateTime dtFechaFin;
            DateTime dtFechaMax = new DateTime(2079, 1, 1, 0, 0, 0);
            if (string.IsNullOrEmpty(txtFechaFinUsuario.Text))
            {
                dtFechaFin = dtFechaMax;
            }
            else
            {
                dtFechaFin = Convert.ToDateTime(txtFechaFinUsuario.Text);
                dtFechaFin = (dtFechaFin.Year <= 2078) ? dtFechaFin : dtFechaMax;
            }

            string lsdtFechaInicioEmple = DSODataAccess.ExecuteScalar("select dtIniVigencia from [VisHistoricos('Emple','Empleados','Español')] " +
                                                           "where iCodCatalogo = " + iCodCatalogoEmple +
                                                           " and dtIniVigencia<>dtFinVigencia and dtFinVigencia >= getdate()").ToString();
            DateTime dtFechaInicioEmple = Convert.ToDateTime(lsdtFechaInicioEmple);

            //Validamos si las fechas proporcionadas son validas. Si la consulta regresa algo, indica que hay periodos que se empalman
            int countFechaIni = (int)DSODataAccess.ExecuteScalar(ConsultaValidarFechas(idUsuario, dtFechaInicio.ToString("yyyy-MM-dd HH:mm:ss"), iCodRegistro, true));
            int countFechaFin = 0;
            if (dtFechaFin != dtFechaMax)
            {
                countFechaFin = (int)DSODataAccess.ExecuteScalar(ConsultaValidarFechas(idUsuario, dtFechaFin.ToString("yyyy-MM-dd HH:mm:ss"), iCodRegistro, false));
            }

            if (countFechaIni > 0)
            {
                mensajeDeAdvertencia("La fecha de inicio que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                mpeUsuarios.Show();
            }
            else if (countFechaFin > 0)
            {
                mensajeDeAdvertencia("La fecha fin que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                mpeUsuarios.Show();
            }
            else if (dtFechaInicio < dtFechaInicioEmple)
            {
                mensajeDeAdvertencia("La fecha de inicio debe ser mayor o igual a la fecha de alta del empleado.");
                mpeUsuarios.Show();
            }
            else
            {
                //La historia la guarda Historicos.
                //Se hace la edicion de los datos de Fechas y comentario unicamente. Lo de demas no se puede modificar.
                DALCCustodia idUsuarioCC = new DALCCustodia();
                idUsuarioCC.EditIDUsuarios(dtFechaInicio, dtFechaFin, txtComentariosUsuarios.Text, iCodCatalogoIdUsuario);
            }
        }

        public void BajaDeUsuario()
        {
            //Query para extraer el iCodCatalogo del Id de Usuario. 
            StringBuilder sbIdUsuario = new StringBuilder();
            sbIdUsuario.AppendLine("select iCodRegistro, iCodCatalogo, vchCodigo from [VisHistoricos('NxtIDUsuario','Nxt IDs Usuarios','Español')] ");
            sbIdUsuario.AppendLine("where dtinivigencia <> dtfinvigencia ");
            sbIdUsuario.AppendLine("and dtfinvigencia >= getdate()");
            sbIdUsuario.AppendLine("and vchCodigo = '" + txtIdUsuario.Text + "' ");
            sbIdUsuario.AppendLine("and PinVarchar = '" + txtPin.Text + "' ");
            DataRow drIdUsuario = DSODataAccess.ExecuteDataRow(sbIdUsuario.ToString());

            string iCodCatalogoIdUsuario = drIdUsuario["iCodCatalogo"].ToString();
            string idUsuario = drIdUsuario["vchCodigo"].ToString();
            string iCodRegistro = drIdUsuario["iCodRegistro"].ToString();
            string fecha;
            DateTime dtFechaFinParaBorrar;
            if (!string.IsNullOrEmpty(txtFechaFinUsuario.Text))
            {
                fecha = txtFechaFinUsuario.Text;
                dtFechaFinParaBorrar = Convert.ToDateTime(fecha);

                //Validamos si las fechas proporcionadas son validas. Si la consulta regresa algo, indica que hay periodos que se empalman               
                int countFechaFin = (int)DSODataAccess.ExecuteScalar(ConsultaValidarFechas(idUsuario, dtFechaFinParaBorrar.ToString("yyyy-MM-dd HH:mm:ss"), iCodRegistro, false));

                if (countFechaFin > 0)
                {
                    mensajeDeAdvertencia("La fecha fin que selecciono entra dentro de los rangos de relacion de otro empleado, favor de seleccionar una fecha valida.");
                    mpeUsuarios.Show();
                }
                else
                {
                    //La historia esta en el mismo historico.
                    DALCCustodia idUsuarioCC = new DALCCustodia();

                    idUsuarioCC.BajaIDUsuarios(iCodCatalogoIdUsuario, dtFechaFinParaBorrar);
                    btnGuardarUsuario.Enabled = true;
                }
            }
            else
            {
                mensajeDeAdvertencia("La fecha fin es requerida.");
                mpeUsuarios.Show();
            }
        }

        private void limpiaCamposDePopUpUsuarios()
        {
            txtIdUsuario.Text = null;
            txtPin.Text = null;
            txtFechaIniUsuario.Text = null;
            txtFechaFinUsuario.Text = null;
            txtComentariosUsuarios.Text = null;
        }

        private void limpiaCamposNoPopUpUsuario()
        {
            txtIDUsuarioNoPopUp.Text = null;
            txtPinNoPopUp.Text = null;
            txtFechaInicioUsuarioNoPopUp.Text = null;
            txtFechaFinUsuarioNoPopUp.Text = null;
            txtComentariosUsuarioNoPopUp.Text = null;
        }

        private void CargaPropControlesAlAgregarUsuario()
        {
            //Controles de pop-up
            txtIdUsuario.Enabled = true;
            txtIdUsuario.ReadOnly = false;
            txtPin.Enabled = true;
            txtPin.ReadOnly = false;
            txtFechaIniUsuario.Enabled = true;
            txtFechaIniUsuario.ReadOnly = false;
            txtFechaFinUsuario.Enabled = true;
            txtFechaFinUsuario.ReadOnly = false;
            txtComentariosUsuarios.Enabled = true;
            txtComentariosUsuarios.ReadOnly = false;

            //Se cambian los titulos del pop-up de Id de Usuario y el texto del boton
            lblTituloPopUpUsuarios.Text = "Detalle de Id de Usuario";
            btnGuardarUsuario.Text = "Guardar";

        }

        private void CargaPropControlesAlEditarUsuario()
        {
            //Controles de pop-up
            txtIdUsuario.Enabled = false;
            txtIdUsuario.ReadOnly = true;
            txtPin.Enabled = false;
            txtPin.ReadOnly = true;
            txtFechaIniUsuario.Enabled = true;
            txtFechaIniUsuario.ReadOnly = false;
            txtFechaFinUsuario.Enabled = true;
            txtFechaFinUsuario.ReadOnly = false;
            txtComentariosUsuarios.Enabled = true;
            txtComentariosUsuarios.ReadOnly = false;

            //Se cambian los titulos del pop-up de Id de Usuario y el texto del boton
            lblTituloPopUpUsuarios.Text = "Detalle de Id de Usuario";
            btnGuardarUsuario.Text = "Guardar";

            //Control para realizar proceso de edicion
            cbEditarUsuario.Checked = true;
            cbBajaUsuario.Checked = false;

        }

        private void CargaPropControlesAlBorrarUsuario()
        {
            //Controles de pop-up
            txtIdUsuario.Enabled = false;
            txtIdUsuario.ReadOnly = true;
            txtPin.Enabled = false;
            txtPin.ReadOnly = true;
            txtFechaIniUsuario.Enabled = false;
            txtFechaIniUsuario.ReadOnly = true;

            txtFechaFinUsuario.Enabled = true;
            txtFechaFinUsuario.ReadOnly = false;

            txtComentariosUsuarios.Enabled = false;
            txtComentariosUsuarios.ReadOnly = true;

            //Se cambian los titulos del pop-up de Id de Usuario y el texto del boton
            //Se cambia la leyenda del pop-up
            lblTituloPopUpUsuarios.Text = "¿Esta seguro que desea dar de baja el Id de Usuario?";
            btnGuardarUsuario.Text = "Eliminar";

            //Control para realizar proceso de baja
            cbEditarUsuario.Checked = false;
            cbBajaUsuario.Checked = true;
        }

        protected void grvUsuarios_EditRow(object sender, ImageClickEventArgs e)
        {
            ImageButton ibtn2 = sender as ImageButton;
            int rowIndex = Convert.ToInt32(ibtn2.Attributes["RowIndex"]);

            //Use this rowIndex in your code 

            GridViewRow selectedRow = (GridViewRow)grvUsuarios.Rows[rowIndex];

            //Se llenan los controles del pop-up
            txtIdUsuario.Text = selectedRow.Cells[0].Text;
            txtPin.Text = selectedRow.Cells[1].Text;
            txtFechaIniUsuario.Text = selectedRow.Cells[2].Text;
            txtFechaFinUsuario.Text = selectedRow.Cells[3].Text;
            txtComentariosUsuarios.Text = (selectedRow.Cells[4].Text == "&nbsp;") ? "" : selectedRow.Cells[4].Text;

            CargaPropControlesAlEditarUsuario();
            mpeUsuarios.Show();
        }

        protected void grvUsuarios_DeleteRow(object sender, ImageClickEventArgs e)
        {
            ImageButton ibtn2 = sender as ImageButton;
            int rowIndex = Convert.ToInt32(ibtn2.Attributes["RowIndex"]);

            //Use this rowIndex in your code 
            GridViewRow selectedRow = (GridViewRow)grvUsuarios.Rows[rowIndex];

            //Se llenan los controles del pop-up
            txtIdUsuario.Text = selectedRow.Cells[0].Text;
            txtPin.Text = selectedRow.Cells[1].Text;
            txtFechaIniUsuario.Text = selectedRow.Cells[2].Text;
            //txtFechaFinUsuario.Text = selectedRow.Cells[3].Text;
            txtFechaFinUsuario.Text = "";
            txtComentariosUsuarios.Text = (selectedRow.Cells[4].Text == "&nbsp;") ? "" : selectedRow.Cells[4].Text;

            CargaPropControlesAlBorrarUsuario();
            mpeUsuarios.Show();
        }

        #endregion Id Usuarios - Empleado


    }
}


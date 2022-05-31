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
    public partial class AppCCustodiaFCA : System.Web.UI.Page
    {
        #region Eventos

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                FillDLLS();
            }
        }

        protected void btnGrabar_Click(object sender, EventArgs e)
        {
            AltaEmple();
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {

        }

        protected void ddlTipoEmple_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool esEmple = false;
            if (ddlTipoEmple.SelectedItem.Text.ToLower() != "--seleccionar--")
            {
                esEmple = true;
            }

            EnableAll(esEmple);

            if (ddlTipoEmple.SelectedItem.Text.ToLower() == "externo")
            {
                var nomExterno = NuevoEmpleadoBackend.GeneraNomAutomatica();
                txtDatosEmpleNomina.Text = "EX" + nomExterno;
            }
            else
            {
                txtDatosEmpleNomina.Text = "";
            }
        }

        #endregion

        #region FillDDL
        protected void FillDLLS()
        {

            //Fill ddl tiposEmple
            FillDdLTipoEmple();

            //Fill ddl Planta
            FillDdLPlanta();

            //Fill ddl Sitio
            FillDdLSitio();

            //fill ddl Cobetura
            FillDdlCobertura();

            //Fill ddl jefeDirecto llena 2 ddls jefe directo y director
            FillDdlJefeDirecto();

            //Fill CenCos
            FillDdLCenCos();

            //Fill Depto
            FillDdlDpto();


        }

        protected void FillDdLTipoEmple()
        {
            var dtRes = NuevoEmpleadoBackend.GetTiposEmple();
            ddlTipoEmple.DataSource = dtRes;
            ddlTipoEmple.DataTextField = "vchDescripcion";
            ddlTipoEmple.DataValueField = "iCodCatalogo";
            ddlTipoEmple.DataBind();

        }

        protected void FillDdLPlanta()
        {
            var dtRes = NuevoEmpleadoBackend.GetPlantasFCA();

            ddlDatosEmplePlanta.DataSource = dtRes;
            ddlDatosEmplePlanta.DataTextField = "vchDescripcion";
            ddlDatosEmplePlanta.DataValueField = "iCodCatalogo";
            ddlDatosEmplePlanta.DataBind();

        }

        protected void FillDdLSitio()
        {
            var dtRes = NuevoEmpleadoBackend.GetSitios(Convert.ToInt32(Session["iCodUsuario"]));

            ddlDatosEmpleSitio.DataSource = dtRes;
            ddlDatosEmpleSitio.DataTextField = "vchDescripcion".ToUpper();
            ddlDatosEmpleSitio.DataValueField = "iCodCatalogo";
            ddlDatosEmpleSitio.DataBind();

        }

        protected void FillDdlCobertura()
        {
            var dtRes = NuevoEmpleadoBackend.GetCos();

            ddlDatosEmpleCobertura.DataSource = dtRes;
            ddlDatosEmpleCobertura.DataTextField = "vchDescripcion";
            ddlDatosEmpleCobertura.DataValueField = "iCodCatalogo";
            ddlDatosEmpleCobertura.DataBind();

        }

        protected void FillDdLCenCos()
        {
            var dtRes = NuevoEmpleadoBackend.GetCenCosFCA(false);

            ddlDatosEmpleCentroCostos.DataSource = dtRes;
            ddlDatosEmpleCentroCostos.DataTextField = "vchDescripcion";
            ddlDatosEmpleCentroCostos.DataValueField = "iCodCatalogo";
            ddlDatosEmpleCentroCostos.DataBind();
        }

        protected void FillDdlDpto()
        {

            var dtRes = NuevoEmpleadoBackend.GetCenCosFCA(true);

            ddlDatosEmpleDepartamento.DataSource = dtRes;
            ddlDatosEmpleDepartamento.DataTextField = "vchDescripcion";
            ddlDatosEmpleDepartamento.DataValueField = "iCodCatalogo";
            ddlDatosEmpleDepartamento.DataBind();
        }

        protected void FillDdlJefeDirecto()
        {
            var dtRes = NuevoEmpleadoBackend.GetJefesFCA();

            ddlDatosEmpleDirector.DataSource = dtRes;
            ddlDatosEmpleDirector.DataTextField = "vchDescripcion";
            ddlDatosEmpleDirector.DataValueField = "iCodCatalogo";
            ddlDatosEmpleDirector.DataBind();

            ddlDatosEmpleJefeDirecto.DataSource = dtRes;
            ddlDatosEmpleJefeDirecto.DataTextField = "vchDescripcion";
            ddlDatosEmpleJefeDirecto.DataValueField = "iCodCatalogo";
            ddlDatosEmpleJefeDirecto.DataBind();

        }

        #endregion

        #region Validaciones
        protected bool ValidaDatosEmple(Hashtable phtValuesEmple, DatosEmple objDatosEmple)
        {
            NuevoEmpleadoBackend.IncializaNomina(phtValuesEmple, objDatosEmple.Nomina);

            string pjsObj = string.Empty;

            return ValidarCamposRequeridosFCA(objDatosEmple, ref pjsObj)
                && ValidarVigencias(phtValuesEmple, objDatosEmple, ref pjsObj)
                && ValidarCampos(phtValuesEmple, objDatosEmple, ref pjsObj)
                && ValidarClaves(phtValuesEmple, objDatosEmple, ref pjsObj)
                && ValidaDatoEmpleados(phtValuesEmple, objDatosEmple, ref pjsObj);
        }



        protected bool ValidarCamposRequeridosFCA(DatosEmple objDatosEmple, ref string pjsObj)
        {
            try
            {

                bool lbret = true;
                StringBuilder lsbErrores = new StringBuilder();

                string lsError;
                string lsTitulo = DSOControl.JScriptEncode("Empleados");


                if (objDatosEmple.T_ID.Length == 0)
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "T_ID"));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                if (objDatosEmple.ICodCatSitio == 0)
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Sitio"));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                if (objDatosEmple.DepartamentoDesc.Length == 0)
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Departamento"));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                //if (objDatosEmple.NumLineaDirecta.Length == 0)
                //{
                //    lsError = DSOControl.JScriptEncode("El campo número línea directa es requerido.");
                //    lsbErrores.Append("<li>" + lsError + "</li>");
                //}

                if (objDatosEmple.ICodCatDepartamento == 0)
                {
                    lsError = DSOControl.JScriptEncode("El campo departamento es requerido.");
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                if (objDatosEmple.TipoEmpleDesc.ToLower() == "externo")
                {
                    if (objDatosEmple.ICodCatCenCos == 0)
                    {
                        lsError = DSOControl.JScriptEncode("El campo centro de costos es requerido.");
                        lsbErrores.Append("<li>" + lsError + "</li>");
                    }
                }
                

                if (BuscaUsuarMismoNombre(objDatosEmple.T_ID))
                {
                    lsError = DSOControl.JScriptEncode("Usuario existente");
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                if (BuscaCodAutoMismoCodigo(objDatosEmple.ClaveFAC))
                {
                    lsError = DSOControl.JScriptEncode("Codigo FAC existente");
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                if (BuscaCodAutoMismoCodigo(objDatosEmple.ClaveTelenet))
                {
                    lsError = DSOControl.JScriptEncode("Codigo Telenet existente");
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                if (BuscaCodAutoMismoCodigo(objDatosEmple.Extension) && !objDatosEmple.SinExt)
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "Extension Existente", "Extension"));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }


                if (!ValidaCodAuto(objDatosEmple))
                {
                    lsError = DSOControl.JScriptEncode("La Fecha de inicio de la relación del codigo con el empleado se traslapa.");
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }

                if (!ValidaExten(objDatosEmple))
                {
                    lsError = DSOControl.JScriptEncode("La Fecha de inicio de la relación de la extensión con el empleado se traslapa.");
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
                throw ex;
            }
        }

        public bool ValidaCodAuto(DatosEmple objDatosEmple)
        {
            bool res = false;

            StringBuilder query = new StringBuilder();

            query.AppendLine("declare @iCodCatClave int =0											   ");
            query.AppendLine("Select  @iCodCatClave = iCodCatalogo									   ");
            query.AppendLine("From [" + DSODataContext.Schema + "].[VisHistoricos('CodAuto','Codigo Autorizacion','Español')]	   ");
            query.AppendLine("Where dtIniVigencia <> dtFinVigencia									   ");
            query.AppendLine("And dtFinVigencia >= GETDATE()										   ");
            query.AppendLine("And vchCodigo = '"+objDatosEmple.ClaveFAC+"'											   ");
            query.AppendLine("																		   ");
            query.AppendLine("																		   ");
            query.AppendLine("if(@iCodCatClave > 0)													   ");
            query.AppendLine("begin																	   ");
            query.AppendLine("	EXEC ValidaHistoriaRecurso											   ");
            query.AppendLine("	   @Esquema = '" + DSODataContext.Schema + "',												   ");
            query.AppendLine("	   @iCodRecurso = @iCodCatClave,									   ");
            query.AppendLine("	   @fechaweb = '"+Convert.ToDateTime(objDatosEmple.FechaInicioVigencia).ToString("yyyy-MM-dd") +"',								   ");
            query.AppendLine("	   @iCodRegistroRel = null,											   ");
            query.AppendLine("	   @nombreCampoiCodRecurso  = 'CodAuto',							   ");
            query.AppendLine("	   @RelacionTripleComilla = '''Empleado - CodAutorizacion'''		   ");
            query.AppendLine("																		   ");
            query.AppendLine("end																	   ");
            query.AppendLine("Else																	   ");
            query.AppendLine("Begin																	   ");
            query.AppendLine("Select 1																	   ");
            query.AppendLine("end																	   ");


            DataTable dt = DSODataAccess.Execute(query.ToString());

            if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
            {
                res = dt.Rows[0][0].ToString() == "1" ? true : false;
            }
            return res;
        }

        public bool ValidaExten(DatosEmple objDatosEmple)
        {
            bool res =false;

            StringBuilder query = new StringBuilder();
            query.AppendLine("declare @icodcatExten int =0									");
            query.AppendLine("select  @icodcatExten =iCodCatalogo							");
            query.AppendLine("From ["+DSODataContext.Schema+"].[VisHistoricos('Exten','Extensiones','Español')]		");
            query.AppendLine("Where dtIniVigencia <> dtFinVigencia							");
            query.AppendLine("And dtFinVigencia >= GETDATE()								");
            query.AppendLine("And vchCodigo = '" + objDatosEmple.Extension  + "'");
            query.AppendLine("And Sitio = "+objDatosEmple.ICodCatSitio);
            query.AppendLine("																");
            query.AppendLine("if(@icodcatExten > 0)											");
            query.AppendLine("begin															");
            query.AppendLine("	EXEC ValidaHistoriaRecurso									");
            query.AppendLine("	   @Esquema = '" + DSODataContext.Schema + "',  				");
            query.AppendLine("	   @iCodRecurso = @icodcatExten,							");
            query.AppendLine("	   @fechaweb = '" + Convert.ToDateTime(objDatosEmple.FechaInicioVigencia).ToString("yyyy-MM-dd") + "',  	");
            query.AppendLine("	   @iCodRegistroRel = null,									");
            query.AppendLine("	   @nombreCampoiCodRecurso  = 'Exten',						");
            query.AppendLine("	   @RelacionTripleComilla = '''Empleado - Extension'''		");
            query.AppendLine("																");
            query.AppendLine("end															");
            query.AppendLine("Else																	   ");
            query.AppendLine("Begin																	   ");
            query.AppendLine("Select 1																	   ");
            query.AppendLine("end																	   ");

            DataTable dt = DSODataAccess.Execute(query.ToString());

            if (dt.Rows.Count>0 && dt.Columns.Count > 0)
            {
                res = dt.Rows[0][0].ToString() == "1" ? true: false;
            }
            return res;
        }

        protected bool ValidarVigencias(Hashtable phtValuesEmple, DatosEmple objDatosEmple, ref string pjsObj)
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
                if (String.IsNullOrEmpty(objDatosEmple.FechaInicioVigencia))
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

        protected virtual bool ValidarCampos(Hashtable phtValuesEmple, DatosEmple objDatosEmple, ref string pjsObj)
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

                if (DSODataContext.Schema.ToUpper() != "FCA")
                {
                    if (!phtValuesEmple.ContainsKey("{CenCos}"))
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RelacionRequerida", "Centro de Costos"));
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
                    if (Convert.ToInt32(objDatosEmple.ICodCatDepartamento) <= 0)
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "Departamento"));
                        lsbErrores.Append("<li>" + lsError + "</li>");
                    }


                    if (objDatosEmple.T_ID.Length <= 0)
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", "T_ID"));
                        lsbErrores.Append("<li>" + lsError + "</li>");
                    }
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

                //20161108 NZ Se agrega validacion de que si la bandera de omitir de la sincronización esta encendida entonces es necesario que
                //se introduzca un comentario para especificar el motivo.

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

        protected bool ValidarClaves(Hashtable phtValuesEmple, DatosEmple objDatosEmple, ref string pjsObj)
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

            NuevoEmpleadoBackend.IncializaCampos(phtValuesEmple, objDatosEmple.PrimerNombre, objDatosEmple.SegundoNombre, objDatosEmple.ApPaterno, objDatosEmple.ApMaterno);

            string liCodEmpresa = "0";
            int liCodCatalogo = 0;

            if (DSODataContext.Schema.ToUpper() != "FCA")
            {
                int.TryParse(phtValuesEmple["{CenCos}"].ToString(), out liCodCatalogo);
            }
            else
            {
                int.TryParse(objDatosEmple.ICodCatDepartamento.ToString(), out liCodCatalogo);
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
                    if (objDatosEmple.estado == "edit")
                    {
                        psbQuery.AppendLine("and H.iCodRegistro <> isnull(" + objDatosEmple.iCodCatemple + ",-1)"); //RZ. ojo con este campo validar
                        psbQuery.AppendLine("and H.iCodCatalogo <> isnull(" + objDatosEmple.iCodCatemple + ",-1)");
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

        //RZ.20130729 Se agrega metodo para validar la entrada de los datos
        protected bool ValidaDatoEmpleados(Hashtable phtValuesEmple, DatosEmple objDatosEmple, ref string pjsObj)
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
                lsError = NuevoEmpleadoBackend.GetMsgError("Nómina", "ValEmplFormato");
                lsbErrores.Append("<li>" + lsError);
            }

            // Valida el Nombre
            lsValue = phtValuesEmple["{Nombre}"].ToString();
            if (lsValue == "" || lsValue.Contains(","))
            {
                lsError = NuevoEmpleadoBackend.GetMsgError("Nombre", "ValEmplFormato");
                lsbErrores.Append("<li>" + lsError);
            }

            // Valida el Segundo Nombre
            lsValue = objDatosEmple.SegundoNombre;
            if (lsValue.Contains(","))
            {
                lsError = NuevoEmpleadoBackend.GetMsgError("Segundo Nombre", "ValEmplFormato");
                lsbErrores.Append("<li>" + lsError);
            }

            // Valida el Apellido Paterno
            lsValue = phtValuesEmple["{Paterno}"].ToString();
            if (lsValue.Contains(","))
            {
                lsError = NuevoEmpleadoBackend.GetMsgError("Apellido Paterno", "ValEmplFormato");
                lsbErrores.Append("<li>" + lsError);
            }

            // Valida el Apellido Materno
            lsValue = phtValuesEmple["{Materno}"].ToString();
            if (lsValue.Contains(","))
            {
                lsError = NuevoEmpleadoBackend.GetMsgError("Apellido Materno", "ValEmplFormato");
                lsbErrores.Append("<li>" + lsError);
            }

            // Valida la cuenta de correo Formato
            lsValue = phtValuesEmple["{Email}"].ToString();
            if (lsValue.Length > 0)
            {
                string pattern = @"^([^(áéíóúÁÉÍÓÚ()<>@,;:\[\] ç % &]+)(@)([^ áéíóúÁÉÍÓÚ() <>@,;:\[\]ç%&]{3,})([.][\w]{2,}){1,3}$";
                if (!System.Text.RegularExpressions.Regex.IsMatch(lsValue, pattern))
                {
                    lsError = NuevoEmpleadoBackend.GetMsgError("E-mail", "ValEmplFormato");
                    lsbErrores.Append("<li>" + lsError);
                }
            }
            // No se puede asignar al mismo empleado como responsable
            if (NuevoEmpleadoBackend.IsRespEmpleadoSame(phtValuesEmple, objDatosEmple.iCodCatemple.ToString()))
            {
                lsError = NuevoEmpleadoBackend.GetMsgError("Jefe Inmediato", "ErrEmplRespSame");
                lsbErrores.Append("<li>" + lsError);
            }
            // Valida si se captura el jefe debe ser un empleado
            if (phtValuesEmple.Contains("{Emple}"))
            {
                lsValue = phtValuesEmple["{Emple}"].ToString();


                // Es externo debe asignarsele un responsable que sea empleado o externo
                if (NuevoEmpleadoBackend.IsExterno(phtValuesEmple) && !NuevoEmpleadoBackend.IsRespEmpleadoExterno(phtValuesEmple))
                {
                    lsError = NuevoEmpleadoBackend.GetMsgError("Jefe Inmediato", "ValJefeEmplExt");
                    lsbErrores.Append("<li>" + lsError);
                }
            }

            //Validar que el usuario no este asignado a otro empleados
            lsError = NuevoEmpleadoBackend.UsuarioAsignado(phtValuesEmple, objDatosEmple.iCodCatemple.ToString());

            if (lsError.Length > 0)
            {
                lsError = NuevoEmpleadoBackend.GetMsgError("Usuario", lsError);
                lsbErrores.Append("<li>" + lsError);
            }

            string lsvchCodUsuar = objDatosEmple.UsuarDesc;
            DateTime ldtFinVigencia = Convert.ToDateTime(phtValuesEmple["dtFinVigencia"].ToString());

            //Si no hay errores entonces crear usuario, en caso de que no exista ya
            //RZ.20131201 Se retira llamada al metodo que valida si el empleado es tipo empleado para crear usuario && IsEmpleado()
            //if (lsbErrores.Length == 0 && lsvchCodUsuar != String.Empty && phtValuesEmple["{Usuar}"].ToString() == "null")
            //{
            //    if (ldtFinVigencia.Date > DateTime.Today)
            //    {
            //        lsError = NuevoEmpleadoBackend.GeneraUsuario(phtValuesEmple, objDatosEmple.T_ID, Session["iCodUsuario"].ToString(), Session["iCodUsuarioDB"].ToString());
            //        if (lsError != "")
            //        {

            //            lsError = NuevoEmpleadoBackend.GetMsgError("Usuario", lsError);
            //            lsbErrores.Append("<li>" + lsError);
            //        }
            //    }
            //}


            if (lsbErrores.Length > 0)
            {
                lbRet = false;
                lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
            }

            return lbRet;
        }


        protected bool BuscaUsuarMismoNombre(string vchCodUsuar)
        {
            StringBuilder query = new StringBuilder();

            bool res = true;

            if (vchCodUsuar.Length > 0)
            {
                query.AppendLine("select   count(*)												");
                query.AppendLine("From [" + DSODataContext.Schema + "].[VisHistoricos('Usuar','Usuarios','Español')] usuar	");
                query.AppendLine("Where dtIniVigencia <> dtFinVigencia							");
                query.AppendLine("And dtFinVigencia >= GETDATE()								");
                query.AppendLine("And vchCodigo = '" + vchCodUsuar + "'										");


                DataTable dtRes = new DataTable();
                dtRes = DSODataAccess.Execute(query.ToString());

                if (dtRes.Rows.Count > 0 && dtRes.Columns.Count > 0)
                {
                    int cant = -1;

                    int.TryParse(dtRes.Rows[0][0].ToString(), out cant);

                    if (cant == 0)
                    {
                        res = false;
                    }
                }
            }


            return res;


        }

        protected bool BuscaCodAutoMismoCodigo(string vchCodigoCodAuto)
        {
            bool res = true;

            StringBuilder query = new StringBuilder();

            query.AppendLine("Select count(*)															");
            query.AppendLine("From [" + DSODataContext.Schema + "].[VisHistoricos('CodAuto','Codigo Autorizacion','Español')]		");
            query.AppendLine("where dtIniVigencia <> dtFinVigencia										");
            query.AppendLine("And dtFinVigencia >= GETDATE()											");
            query.AppendLine("And vchCodigo = '" + vchCodigoCodAuto + "'														");

            DataTable dtRes = new DataTable();
            dtRes = DSODataAccess.Execute(query.ToString());

            if (dtRes.Rows.Count > 0 && dtRes.Columns.Count > 0)
            {
                int cant = -1;

                int.TryParse(dtRes.Rows[0][0].ToString(), out cant);

                if (cant == 0)
                {
                    res = false;
                }
            }
            return res;
        }

        protected bool BuscaExtensionMismoCodigo(string vchCodigoExtension)
        {
            bool res = true;

            StringBuilder query = new StringBuilder();

            query.AppendLine("select count(*)											");
            query.AppendLine("From [" + DSODataContext.Schema + "].[VisHistoricos('Exten','Extensiones','Español')]	");
            query.AppendLine("where dtIniVigencia <> dtFinVigencia						");
            query.AppendLine("And dtFinVigencia >= GETDATE()							");
            query.AppendLine("And vchCodigo = '" + vchCodigoExtension + "'					");

            DataTable dtRes = new DataTable();
            dtRes = DSODataAccess.Execute(query.ToString());

            if (dtRes.Rows.Count > 0 && dtRes.Columns.Count > 0)
            {
                int cant = -1;

                int.TryParse(dtRes.Rows[0][0].ToString(), out cant);

                if (cant == 0)
                {
                    res = false;
                }
            }
            return res;
        }

        #endregion

        protected void EnableAll(bool status = true)
        {
            txtDatosEmpleNombre.Enabled = status;
            txtDatosEmpleNomina.Enabled = status;
            txtDatosEmpleDc_id.Enabled = status;
            txtxDatosEmpleApPaterno.Enabled = status;
            txtDatosEmpleApMaterno.Enabled = status;
            txtDatosEmpleNickName.Enabled = status;
            //txtDatosEmpleNumLineaDirecta.Enabled = status;
            txtDatosEmpleEstacion.Enabled = status;
            txtEmailEmple.Enabled = status;
            txtDatosEmpleFechaInicio.Enabled = status;
            //txtDatosempleClaveTelenet.Enabled = false;
            txtDatosEmpleT_ID.Enabled = status;
            ddlDatosEmpleDirector.Enabled = status;
            txtDatosEmpleExtension.Enabled = status;
            txtDatosEmpleClaveFAC.Enabled = status;
            ddlDatosEmpleCobertura.Enabled = status;
            rdDatosEmpleesDirectorSI.Enabled = status;
            rdDatosEmpleesDirectorNO.Enabled = status;
            ddlDatosEmpleJefeDirecto.Enabled = status;
            ddlDatosEmpleCentroCostos.Enabled = status;
            ddlDatosEmpleDepartamento.Enabled = status;
            cbSinExt.Enabled = status;
            chkMostrar.Enabled = status;
            ddlDatosEmplePlanta.Enabled = status;
            ddlDatosEmpleSitio.Enabled = status;

            btnCancelar.Enabled = status;
            btnGrabar.Enabled = status;

            if (ddlTipoEmple.SelectedItem.Text.ToLower() == "externo")
            {
                divCenCos.Visible = status;
            }
            else
            {
                divCenCos.Visible = false;
            }

        }
        protected void LimpiaControles()
        {
            txtDatosEmpleNombre.Text ="";
            txtDatosEmpleNomina.Text ="";
            txtDatosEmpleDc_id.Text ="";
            txtxDatosEmpleApPaterno.Text = "";
            txtDatosEmpleApMaterno.Text = "";
            txtDatosEmpleNickName.Text = "";
            //txtDatosEmpleNumLineaDirecta.Text = "";
            txtDatosEmpleEstacion.Text = "";
            txtDatosEmpleFechaInicio.Text = "";
            txtEmailEmple.Text = "";
            //txtDatosempleClaveTelenet.Enabled = false;
            txtDatosEmpleT_ID.Text = "";
            txtDatosEmpleExtension.Text = "";
            txtDatosEmpleClaveFAC.Text = "";
            cbSinExt.Checked = false;
            chkMostrar.Checked=false;

            if (ddlTipoEmple.SelectedItem.Text.ToLower() == "externo")
            {
                divCenCos.Visible = true;
            }
            else
            {
                divCenCos.Visible = false;
            }
        }
        protected bool AltaEmple()
        {
            bool res = false;
            string mensajeRes = "";

            try
            {
                DatosEmple objDatosEmple = new DatosEmple();
                objDatosEmple = BuscaDatosEmple();

                Hashtable phtValuesEmple = NuevoEmpleadoBackend.ObtenerHashEmpleado(objDatosEmple, Session["iCodUsuarioDB"].ToString());

                if (ValidaDatosEmple(phtValuesEmple, objDatosEmple))
                {
                    string ValidacionLineaRes = NuevoEmpleadoBackend.ValidaCamposLineaAlta(objDatosEmple);

                    if (ValidacionLineaRes.Length > 0)
                    {
                        throw new ArgumentException(ValidacionLineaRes);
                    }
                    else
                    {
                        objDatosEmple.iCodCatemple = Convert.ToInt32(
                                                                        NuevoEmpleadoBackend.GrabarEmpleado(
                                                                                                                phtValuesEmple,
                                                                                                                objDatosEmple,
                                                                                                                Session["iCodUsuarioDB"].ToString()
                                                                                                            )
                                                                    );
                        if (txtDatosEmpleT_ID.Text != "" && txtDatosEmpleClaveFAC.Text != "")
                        {
                            string pwd = txtDatosEmpleClaveFAC.Text;
                            string password = NuevoEmpleadoBackend.ObtienePWD(pwd);
                            if (password != "")
                            {
                                CreaUsuarios(objDatosEmple.iCodCatemple, txtDatosEmpleT_ID.Text, password, txtEmailEmple.Text);
                            }
                        }
                    }

                }

                if(objDatosEmple.iCodCatemple > 0)
                {
                    lblTituloModalMsn.Text = "¡Mensaje!";
                    lblBodyModalMsn.Text = "El empleado se creo correctamente.";
                    mpeEtqMsn.Show();
                    LimpiaControles();
                }
            }
            catch(Exception ex)
            {
                lblTituloModalMsn.Text = "¡Error!";
                lblBodyModalMsn.Text = "Ocurrió un errro al dar de alta el empleado.";
                mpeEtqMsn.Show();
            }

            return res;
        }

        protected DatosEmple BuscaDatosEmple()
        {
            DatosEmple objDatosEmple = new DatosEmple();

            objDatosEmple.ICodCatTipoEmple = Convert.ToInt32(ddlTipoEmple.SelectedValue);
            objDatosEmple.TipoEmpleDesc = ddlTipoEmple.SelectedItem.Text.Trim();
            objDatosEmple.Nomina = txtDatosEmpleNomina.Text.Trim();
            objDatosEmple.DC_ID = txtDatosEmpleDc_id.Text.Trim();
            objDatosEmple.PrimerNombre = txtDatosEmpleNombre.Text.Trim();
            objDatosEmple.SegundoNombre = "";
            objDatosEmple.ApPaterno = txtxDatosEmpleApPaterno.Text.Trim();
            objDatosEmple.ApMaterno = txtDatosEmpleApMaterno.Text.Trim();
            objDatosEmple.NickName = txtDatosEmpleNickName.Text.Trim();
            objDatosEmple.NumLineaDirecta = "";//txtDatosEmpleNumLineaDirecta.Text.Trim();
            objDatosEmple.Estatcion = txtDatosEmpleEstacion.Text.Trim();
            objDatosEmple.FechaInicioVigencia = txtDatosEmpleFechaInicio.Text.Trim();
            objDatosEmple.FechaFinVigencia = "2079-01-01 00:00:00";
            objDatosEmple.ICodCatPlanta = Convert.ToInt32(ddlDatosEmplePlanta.SelectedValue);
            objDatosEmple.Plantadesc = ddlDatosEmplePlanta.SelectedItem.Text.Trim();
            //objDatosEmple.ClaveTelenet = txtDatosempleClaveTelenet.Text.Trim();
            objDatosEmple.T_ID = txtDatosEmpleT_ID.Text.Trim();
            objDatosEmple.ICodCatDirector = Convert.ToInt32(ddlDatosEmpleDirector.SelectedValue);
            objDatosEmple.DirectorDesc = ddlDatosEmpleDirector.SelectedItem.Text.Trim();
            objDatosEmple.Extension = txtDatosEmpleExtension.Text.Trim();
            objDatosEmple.SinExt = cbSinExt.Checked;
            objDatosEmple.ICodCatSitio = Convert.ToInt32(ddlDatosEmpleSitio.SelectedValue);
            objDatosEmple.SitioDesc = ddlDatosEmpleSitio.SelectedItem.Text.Trim();
            objDatosEmple.ClaveFAC = txtDatosEmpleClaveFAC.Text.Trim();
            objDatosEmple.ICodCatCobertura = Convert.ToInt32(ddlDatosEmpleCobertura.SelectedValue);
            objDatosEmple.CoberturaDesc = ddlDatosEmpleCobertura.SelectedItem.Text.Trim();
            objDatosEmple.EsDirector = rdDatosEmpleesDirectorSI.Checked;
            objDatosEmple.ICodCatJefeDirecto = Convert.ToInt32(ddlDatosEmpleJefeDirecto.SelectedValue);
            objDatosEmple.JefeDirectoDesc = ddlDatosEmpleJefeDirecto.SelectedItem.Text.ToString().Replace("--Selecciona--", "").Trim();
            objDatosEmple.ICodCatDepartamento = Convert.ToInt32(ddlDatosEmpleDepartamento.SelectedValue);
            objDatosEmple.DepartamentoDesc = ddlDatosEmpleDepartamento.SelectedItem.Text.Trim();
            objDatosEmple.ICodCatCenCos = Convert.ToInt32(ddlDatosEmpleCentroCostos.SelectedValue);
            objDatosEmple.CencosDesc = ddlDatosEmpleCentroCostos.SelectedItem.Text.Trim();

            DataTable dtPuestos = NuevoEmpleadoBackend.GetPuestos();

            if (dtPuestos.Rows.Count > 0 && dtPuestos.Columns.Count > 0)
            {
                objDatosEmple.iCodCatPuesto = Convert.ToInt32(dtPuestos.Rows[0]["iCodcatalogo"].ToString());
                objDatosEmple.PuestoDesc = dtPuestos.Rows[0]["vchDescripcion"].ToString();
            }

            DataTable dtCarrier = NuevoEmpleadoBackend.GetCarrier();

            if (dtCarrier.Rows.Count > 0 && dtCarrier.Columns.Count > 0)
            {
                objDatosEmple.iCodcatCarrier = Convert.ToInt32(dtCarrier.Rows[0]["iCodCatalogo"].ToString());
                objDatosEmple.CarrierDesc = dtCarrier.Rows[0]["vchDescripcion"].ToString();
            }



            objDatosEmple.Email = txtEmailEmple.Text.Trim();


            objDatosEmple.PentaSAPAccount = "0";
            objDatosEmple.PentaSAPProfitCenter = "0";
            objDatosEmple.PentaSAPCostCenter = "0";
            objDatosEmple.PentaSAPFA = "0";
            objDatosEmple.PentaSAPCCDescription = "0";




            return objDatosEmple;
        }

        protected void chkMostrar_CheckedChanged(object sender, EventArgs e)
        {
            if(chkMostrar.Checked == true)
            {
                int idSitio = Convert.ToInt32(ddlDatosEmpleSitio.SelectedValue);
                string claveFac = "";
                claveFac = GeneraClaveFac();
                var existe = NuevoEmpleadoBackend.ValidaExisteCod(claveFac, idSitio);
                if(existe == 0)
                {
                    claveFac = GeneraClaveFac();
                }

                txtDatosEmpleClaveFAC.Text = claveFac;
            }
            else
            {
                txtDatosEmpleClaveFAC.Text = "";
            }

        }
        private string GeneraClaveFac()
        {
            string claveFac = "";
            for (int i = 0; i < 8; i++)
            {
                var guid = Guid.NewGuid();
                var justNumbers = new String(guid.ToString().Where(Char.IsDigit).ToArray());
                var seed = int.Parse(justNumbers.Substring(0, 4));

                var random = new Random(seed);
                var value = random.Next(0, 9);
                claveFac += value.ToString();
            }
            return claveFac;

        }
        private void CreaUsuarios(int emple,string usuario, string password, string email)
        {
            try
            {
                string sp = "EXEC GeneraUsuariosEmple @Usuario = '{0}',@Password = '{1}',@Emple = {2},@Email = '{3}'";
                var query = string.Format(sp, usuario, password, emple, email);
                DSODataAccess.ExecuteNonQuery(query.ToString());
            }
            catch(Exception ex)
            {

            }
        }
    }


    /// <summary>
    /// Clase DatosEmple
    /// </summary>

    public class DatosEmple
    {

        public string estado { get; set; }
        public int FolioCC { get; set; }
        public int iCodCatEstatudCC { get; set; }


        public int iCodCatemple { get; set; }
        public int ICodCatTipoEmple { get; set; }
        public string TipoEmpleDesc { get; set; }
        public string Nomina { get; set; }
        public string PrimerNombre { get; set; }
        public string SegundoNombre { get; set; }
        public string ApPaterno { get; set; }
        public string ApMaterno { get; set; }
        public int ICodCatDepartamento { get; set; }
        public string DepartamentoDesc { get; set; }
        public int ICodCatCenCos { get; set; }
        public string CencosDesc { get; set; }
        public string NickName { get; set; }
        public string NumLineaDirecta { get; set; }
        public string Estatcion { get; set; }
        public string FechaInicioVigencia { get; set; }
        public string FechaFinVigencia { get; set; }
        public int ICodCatPlanta { get; set; }
        public string Plantadesc { get; set; }
        public string ClaveTelenet { get; set; }
        public string T_ID { get; set; }
        public string DC_ID { get; set; }
        public int ICodCatDirector { get; set; }
        public string DirectorDesc { get; set; }
        public string Extension { get; set; }
        public bool SinExt { get; set; }
        public int ICodCatSitio { get; set; }
        public string SitioDesc { get; set; }
        public string ClaveFAC { get; set; }
        public int ICodCatCobertura { get; set; }
        public string CoberturaDesc { get; set; }
        public int ICodCatJefeDirecto { get; set; }
        public string JefeDirectoDesc { get; set; }
        public bool EsDirector { get; set; }

        public int iCodCatUsuar { get; set; }
        public string UsuarDesc { get; set; }
        public string Email { get; set; }
        public int iCodCatPuesto { get; set; }
        public string PuestoDesc { get; set; }
        public int iCodcatCarrier { get; set; }
        public string CarrierDesc { get; set; }

        public string PentaSAPAccount { get; set; }
        public string PentaSAPProfitCenter { get; set; }
        public string PentaSAPCostCenter { get; set; }
        public string PentaSAPFA { get; set; }
        public string PentaSAPCCDescription { get; set; }

    }
}
/*
Nombre:		    SCB
Fecha:		    2011-09-06
Descripción:	Configuración específica para Empleados.
Modificación:	20120314-PGS Campo Nombre Completo
*/

using System;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace KeytiaWeb.UserInterface
{
    public class CnfgEmpleadosEdit : HistoricEdit
    {

        protected List<string> psListFlagsExcl;
        protected List<string> psListFlagsResp;

        protected List<KeytiaBaseField> psListCatalogos;

        protected KeytiaBaseField pFieldEntidad;
        protected KeytiaRelationField pRelField;

        protected String psNewUsuario;
        protected String psNewPassword;

        protected virtual string iCodUsuarioEmple
        {
            get
            {
                return (string)ViewState["iCodUsuarioEmple"];
            }
            set
            {
                ViewState["iCodUsuarioEmple"] = value;
            }
        }

        //RZ.20140128 Se agregan para alta de presupuesto fijo en Modelo
        protected DateTime pdtIniPeriodoActual;
        protected DateTime pdtIniPeriodoSiguiente;
        protected double pdPrepEmple = 0;
        protected DataRow pRowConfigEmpre = null;

        protected override void InitGrid()
        {
            DSOGridClientColumn lCol;
            int lTarget = 0;

            pHisGrid.ClearConfig();
            pHisGrid.Config.sDom = "<\"H\"Rlf>tr<\"F\"pi>"; //con filtro global
            pHisGrid.Config.bAutoWidth = true;
            pHisGrid.Config.sScrollX = "100%";
            pHisGrid.Config.sScrollY = "400px";
            pHisGrid.Config.sPaginationType = "full_numbers";
            pHisGrid.Config.bJQueryUI = true;
            pHisGrid.Config.bProcessing = true;
            pHisGrid.Config.bServerSide = true;
            pHisGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerData(this, sSource, aoData, fnCallback);}";
            pHisGrid.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetHisDataEmple"); //PT.20140410 Cambio el webmethod

            lCol = new DSOGridClientColumn();
            lCol.sName = "iCodRegistro";
            lCol.aTargets.Add(lTarget++);
            lCol.bVisible = false;
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "vchCodigo";
            lCol.aTargets.Add(lTarget++);
            lCol.bVisible = false;
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "vchDescripcion";
            lCol.aTargets.Add(lTarget++);
            lCol.bVisible = false;
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "Consultar";
            lCol.aTargets.Add(lTarget++);
            lCol.sWidth = "50px";
            lCol.sClass = "TdConsult";
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            InitGridFields();

            lTarget = pHisGrid.Config.aoColumnDefs.Count;

            lCol = new DSOGridClientColumn();
            lCol.sName = "dtIniVigencia";
            lCol.aTargets.Add(lTarget++);
            lCol.sWidth = "120px";
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "dtFinVigencia";
            lCol.aTargets.Add(lTarget++);
            lCol.sWidth = "120px";
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "dtFecUltAct";
            lCol.aTargets.Add(lTarget++);
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            if (pHisGrid.Config.aoColumnDefs.Count > 10)
            {
                pHisGrid.Config.sScrollXInner = (pHisGrid.Config.aoColumnDefs.Count * 20).ToString() + "%";
            }

            pHisGrid.Fill();
        }

        protected override void InitHisGridLanguage()
        {
            base.InitHisGridLanguage();
            KeytiaBaseField lField;
            DSOControlDB lFiltro;

            foreach (DSOGridClientColumn lCol in pHisGrid.Config.aoColumnDefs)
            {
                if (pFields.Contains(lCol.sName))
                {
                    lField = pFields[lCol.sName];

                    if (lField.ConfigName == "Emple")
                    {
                        lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "MsgEmpleJefe"));
                        if (phtFiltros.ContainsKey(lCol.sName))
                        {
                            lFiltro = (DSOControlDB)phtFiltros[lCol.sName];
                            lFiltro.Descripcion = lCol.sTitle;
                        }

                    }
                }
            }

        }
        public override void InitLanguage()
        {
            base.InitLanguage();
            if (pFields != null)
            {
                if (pFields.ContainsConfigName("Emple"))
                {
                    KeytiaBaseField lField = pFields.GetByConfigName("Emple");
                    //lField.DSOControlDB.TcLbl.Text = Globals.GetMsgWeb(false, "MsgEmpleJefe");
                    lField.DSOControlDB.Descripcion = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "MsgEmpleJefe"));
                }

                //Oculta campo de uso administrativo.
                if (pFields.ContainsConfigName("NomCompleto") && pTablaAtributos != null)
                {
                    pTablaAtributos.Rows[pFields.GetByConfigName("NomCompleto").DSOControlDB.Row - 1].Visible = false;
                    pFields.GetByConfigName("NomCompleto").DSOControlDB.Visible = false;
                }
            }
        }

        protected override void pbtnEditar_ServerClick(object sender, EventArgs e)
        {
            base.pbtnEditar_ServerClick(sender, e);
            BloqueaCampos();
        }

        protected virtual void BloqueaCampos()
        {
            //Si no se cargo ningun campo no hacer nada
            if (pFields == null)
            {
                return;
            }
            //Bloquea el Centro de Costos
            if (pFields.ContainsConfigName("CenCos"))
            {
                pFields.GetByConfigName("CenCos").DisableField();
            }

            //Boquea los campos que no se pueden modificar cuando se agrega un registro 
            if (iCodRegistro == "null")
            {
                if (pFields.ContainsConfigName("Usuar"))
                {
                    pFields.GetByConfigName("Usuar").DisableField();
                }
                return;
            }

            //Boquea los campos que no se pueden modificar en edicion
            if (pFields.ContainsConfigName("TipoEm"))
            {
                pFields.GetByConfigName("TipoEm").DisableField();
            }
            if (pFields.ContainsConfigName("NominaA"))
            {
                pFields.GetByConfigName("NominaA").DisableField();
            }
            if (pFields.ContainsConfigName("OpcCreaUsuar"))
            {
                pFields.GetByConfigName("OpcCreaUsuar").DisableField();
            }
        }

        protected override bool ValidarClaves()
        {

            //si el registro se esta eliminando entonces no es necesaria la validacion de claves
            if (pdtIniVigencia.HasValue && pdtFinVigencia.HasValue && pdtIniVigencia.Date == pdtFinVigencia.Date)
            {
                return true;
            }

            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            StringBuilder lsbErroresCodigos = new StringBuilder();
            string lsError;
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloHistoricos"));
            DataTable ldt;

            IncializaCampos();

            string liCodEmpresa;
            int liCodCatalogo = int.Parse(getValCampo("CenCos", -1).ToString());

            psbQuery.Length = 0;
            psbQuery.AppendLine("Select Empre");
            psbQuery.AppendLine("from [VisHistoricos('CenCos','" + Globals.GetCurrentLanguage() + "')]");
            psbQuery.AppendLine("where iCodCatalogo = " + liCodCatalogo.ToString());

            ldt = DSODataAccess.Execute(psbQuery.ToString());

            if (ldt == null || ldt.Rows.Count == 0 || ldt.Rows[0]["Empre"] is DBNull)
            {
                return base.ValidarClaves();

            }
            liCodEmpresa = ldt.Rows[0]["Empre"].ToString();

            try
            {

                if (pvchCodigo.HasValue)
                {
                    //Valida el numero de Nomina que no se repita a menos que sea de diferente empresa
                    psbQuery.Length = 0;
                    psbQuery.AppendLine("select H.iCodRegistro, H.iCodCatalogo, H.iCodMaestro, H.dtIniVigencia, H.dtFinVigencia");
                    psbQuery.AppendLine("from Historicos H, Catalogos C,");
                    psbQuery.AppendLine("   [" + DSODataContext.Schema + "].[VisHistoricos('Emple','" + Globals.GetCurrentLanguage() + "')] V");
                    psbQuery.AppendLine("where H.iCodRegistro = V.iCodRegistro");
                    psbQuery.AppendLine("and H.iCodCatalogo = C.iCodRegistro");
                    psbQuery.AppendLine("and H.iCodRegistro <> isnull(" + iCodRegistro + ",-1)");
                    psbQuery.AppendLine("and H.iCodCatalogo <> isnull(" + iCodCatalogo + ",-1)");
                    psbQuery.AppendLine("and C.iCodCatalogo = " + iCodEntidad);
                    psbQuery.AppendLine("and C.vchCodigo = " + pvchCodigo.DataValue);
                    psbQuery.AppendLine("and V.CenCos in (Select iCodCatalogo from ");
                    psbQuery.AppendLine("   [" + DSODataContext.Schema + "].[VisHistoricos('CenCos','Español')] VC ");
                    psbQuery.AppendLine("               Where Empre =  " + liCodEmpresa + ")");
                    psbQuery.AppendLine("and H.dtIniVigencia <> H.dtFinVigencia");
                    psbQuery.AppendLine("and ((H.dtIniVigencia <= " + pdtIniVigencia.DataValue);
                    psbQuery.AppendLine("       and H.dtFinVigencia > " + pdtIniVigencia.DataValue + ")");
                    psbQuery.AppendLine("   or (H.dtIniVigencia <= " + pdtFinVigencia.DataValue);
                    psbQuery.AppendLine("       and H.dtFinVigencia > " + pdtFinVigencia.DataValue + ")");
                    psbQuery.AppendLine("   or (H.dtIniVigencia >= " + pdtIniVigencia.DataValue);
                    psbQuery.AppendLine("       and H.dtFinVigencia <= " + pdtFinVigencia.DataValue + "))");
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
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValidaVchCodigo", pvchCodigo.ToString()));
                        lsError = "<span>" + lsError + "</span>";
                        lsbErrores.Append("<li>" + lsError);
                        lsbErrores.Append("<ul>" + lsbErroresCodigos.ToString() + "</ul>");
                        lsbErrores.Append("</li>");
                    }
                }
                else
                {
                    lsbErrores.Append("<li>" + pvchCodigo.RequiredMessage + "</li>");
                }

                if (!pvchDescripcion.HasValue)
                {
                    lsbErrores.Append("<li>" + pvchDescripcion.RequiredMessage + "</li>");
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

        protected override bool ValidarRegistro()
        {
            IncializaNomina();
            return base.ValidarRegistro();
        }

        protected void IncializaNomina()
        {
            //Obten el numero de nomina si no se capturo
            string lsValue = getValCampo("NominaA", "").ToString();
            if (lsValue == "")
            {
                lsValue = ObtenNumeroNomina();
                if (lsValue != "null")
                {
                    setValCampo("NominaA", lsValue, true);
                }
            }

        }

        protected void IncializaCampos()
        {
            //Obten el numero de nomina si no se capturo
            string lsValue = "";
            DataTable ldt;

            //Incializa los valores de Codigo y Descripcion del Historico de Empleados.
            pvchCodigo.DataValue = getValCampo("NominaA", "").ToString().Trim();

            string lsNomEmpleado = getValCampo("Nombre", "").ToString();
            lsNomEmpleado += (getValCampo("Paterno", "").ToString() != "" ? " " + getValCampo("Paterno", "").ToString() : "");
            lsNomEmpleado += (getValCampo("Materno", "").ToString() != "" ? " " + getValCampo("Materno", "").ToString() : "");

            pvchDescripcion.DataValue = lsNomEmpleado.Trim();

            AsignarEntidadActual("CentroCosto-Empleado", vchCodEntidad);

            //Obten el Tipo de Presupuesto si no se capturo
            lsValue = getValCampo("TipoPr", 0).ToString();
            if (lsValue == "0")
            {
                lsValue = ObtenTipoPresupuesto();
                if (lsValue != "null")
                {
                    setValCampo("TipoPr", lsValue, true);
                }
            }

            string lsCodEmpresa;

            int liCodCatalogo = int.Parse(getValCampo("CenCos", -1).ToString());

            psbQuery.Length = 0;
            psbQuery.AppendLine("Select EmpreCod");
            psbQuery.AppendLine("from [VisHistoricos('CenCos','" + Globals.GetCurrentLanguage() + "')]");
            psbQuery.AppendLine("where iCodCatalogo = " + liCodCatalogo.ToString());
            psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia");
            psbQuery.AppendLine("and dtIniVigencia <= '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "'");
            psbQuery.AppendLine("and dtFinVigencia > '" + dtIniVigencia.Date.ToString("yyyy-MM-dd") + "'");

            ldt = DSODataAccess.Execute(psbQuery.ToString());
            if (ldt == null || ldt.Rows.Count == 0 || ldt.Rows[0]["EmpreCod"] is DBNull)
            {
                return;
            }
            lsCodEmpresa = ldt.Rows[0]["EmpreCod"].ToString();

            lsCodEmpresa = "(" + lsCodEmpresa.Substring(0, Math.Min(38, lsCodEmpresa.Length)) + ")";
            lsNomEmpleado = lsNomEmpleado.Trim();
            pvchDescripcion.DataValue = lsNomEmpleado.Substring(0, Math.Min(120, lsNomEmpleado.Length)) + lsCodEmpresa;

        }

        protected override bool ValidarAtribCatalogosVig()
        {
            //Valida la existencia de todos los catalogos para la vigencia del registro historico
            //se asume que ya se mando llamar ValidarCampos, ValidarVigencias
            //no se validan los campos de los catalogos que esten en null
            //se asume que no hay empalmes de vigencias para los registros de historicos con un mismo iCodCatalogo


            //si el registro se esta eliminando entonces no es necesaria la validacion
            if (pdtIniVigencia.HasValue && pdtFinVigencia.HasValue && pdtIniVigencia.Date == pdtFinVigencia.Date)
            {
                return true;
            }

            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            StringBuilder lsbErrorCampo = new StringBuilder();
            string lsError;
            string lsTitulo = DSOControl.JScriptEncode(AlertTitle);

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
                        else if ((DateTime)ladrHis[0]["dtIniVigencia"] > pdtIniVigencia.Date && lField.Descripcion.ToLower() != "jefe")
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

        protected override bool ValidarDatos()
        {
            //Realiza las validaciones genericas de historicos como los valores obligatorios
            bool lbRet = base.ValidarDatos();

            if (!lbRet)
            {
                return lbRet;
            }

            if (State != HistoricState.Baja)
            {
                //Validar Datos de empleados
                lbRet = ValidaDatoEmpleados() && ValidaRelEmpleados();
            }
            else
            {
                if (!EmpladoResponsable())
                {
                    lbRet = false;
                }
            }

            return lbRet;
        }

        protected bool ValidaDatoEmpleados()
        {
            bool lbRet = true;
            string lsError;
            StringBuilder lsbErrores = new StringBuilder();
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloEmpleados"));

            string lsValue;

            lsbErrores.Length = 0;

            // Valida el numero de Nomina
            lsValue = getValCampo("NominaA", "").ToString();
            if (lsValue.Length > 40 ||
                !System.Text.RegularExpressions.Regex.IsMatch(lsValue, "^([a-zA-Z]*[0-9]*[-]*[/]*[_]*[:]*[.]*[|]*)*$"))
            {
                lsError = GetMsgError("NominaA", "Nomina", "ValEmplFormato");
                lsbErrores.Append("<li>" + lsError);
            }

            // Valida el RFC
            lsError = ValidaRFC();
            if (lsError.Length > 0)
            {
                lsError = GetMsgError("RFC", "RFC", lsError);
                lsbErrores.Append("<li>" + lsError);
            }

            // Valida el Nombre
            lsValue = getValCampo("Nombre", "").ToString();
            if (lsValue == "" || lsValue.Contains(","))
            {
                lsError = GetMsgError("Nombre", "Nombre", "ValEmplFormato");
                lsbErrores.Append("<li>" + lsError);
            }

            // Valida el Apellido Paterno
            lsValue = getValCampo("Paterno", "").ToString();
            if (lsValue.Contains(","))
            {
                lsError = GetMsgError("Paterno", "Apellido Paterno", "ValEmplFormato");
                lsbErrores.Append("<li>" + lsError);
            }

            // Valida el Apellido Materno
            lsValue = getValCampo("Materno", "").ToString();
            if (lsValue.Contains(","))
            {
                lsError = GetMsgError("Materno", "Apellido Materno", "ValEmplFormato");
                lsbErrores.Append("<li>" + lsError);
            }

            // Valida la cuenta de correo Formato
            lsValue = getValCampo("Email", "").ToString();
            if (lsValue.Length > 0)
            {
                if (!System.Text.RegularExpressions.Regex.IsMatch(lsValue, "^(([\\w-]+\\.)+[\\w-]+|([a-zA-Z]{1}|[\\w-]{2,}))" + "@" +
                    "((([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])\\.+([0-1]?[0-9]{1,2}|25[0-5]" +
                    "|2[0-4][0-9])\\.([0-1]?[0-9]{1,2}|25[0-5]|2[0-4][0-9])){1}|([a-zA-Z]+[\\w-]+\\.)+[a-zA-Z]{2,4})$"))
                {
                    lsError = GetMsgError("Email", "Cuenta de Correo", "ValEmplFormato");
                    lsbErrores.Append("<li>" + lsError);
                }
            }
            // No se puede asignar al mismo empleado como responsable
            if (IsRespEmpleadoSame())
            {
                lsError = GetMsgError("Emple", "Empleado Responsable", "ErrEmplRespSame");
                lsbErrores.Append("<li>" + lsError);
            }
            // Valida si se captura el jefe debe ser un empleado
            lsValue = getValCampo("Emple", 0).ToString();
            if (lsValue != "0")
            {
                // Es empleado debe ser un empleado
                if (IsEmpleado() && !IsRespEmpleado())
                {
                    lsError = GetMsgError("Emple", "Empleado Responsable", "ValJefeEmpleado");
                    lsbErrores.Append("<li>" + lsError);
                }
                // Es externo debe asignarsele un responsable que sea empleado o externo
                if (IsExterno() && !IsRespEmpleadoExterno())
                {
                    lsError = GetMsgError("Emple", "Empleado Responsable", "ValJefeEmplExt");
                    lsbErrores.Append("<li>" + lsError);
                }

            }
            //Validar que el usuario no este asignado a otro empleados
            lsError = UsuarioAsignado();

            if (lsError.Length > 0)
            {
                lsError = GetMsgError("Usuar", "Usuario", lsError);
                lsbErrores.Append("<li>" + lsError);
            }

            int liOpc = int.Parse(getValCampo("OpcCreaUsuar", 0).ToString());

            if (iCodRegistro == "null" && lsbErrores.Length == 0 && IsEmpleado() && liOpc != 0)
            {
                if (pdtFinVigencia.Date > DateTime.Today)
                {
                    lsError = GeneraUsuario();
                    if (lsError != "")
                    {
                        lsError = GetMsgError("Usuar", "Usuario", lsError);
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

        protected string ValidaRFC()
        {
            string lbRet = "";
            string lsValue;


            lsValue = getValCampo("RFC", "").ToString().ToUpper();

            if (lsValue.Length == 0)
            {
                return lbRet;
            }

            if (lsValue.Replace("-", "").Length > 13 ||
                !System.Text.RegularExpressions.Regex.IsMatch(lsValue, "^([a-zA-Z]*[0-9]*[-]*)*$"))
            {
                return "ValEmplFormato";
            }

            //Validar el RFC no este asignado a otro empleado
            if (lsValue.Replace("-", "").Length == 13)
            {
                psbQuery.Length = 0;
                psbQuery.AppendLine("Select * from [VisHistoricos('Emple','Empleados','" + Globals.GetCurrentLanguage() + "')] ");
                psbQuery.AppendLine("Where iCodCatalogo <> isNull(" + iCodCatalogo + ",-1)");
                psbQuery.AppendLine("and [RFC] = '" + lsValue + "'");
                psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia ");
                psbQuery.AppendLine("and ('" + pdtIniVigencia.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' between dtIniVigencia and dtFinVigencia ");
                psbQuery.AppendLine("or '" + pdtFinVigencia.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' between dtIniVigencia and dtFinVigencia )");
                DataTable ldt = DSODataAccess.Execute(psbQuery.ToString());

                if (ldt != null && ldt.Rows.Count > 0)
                {
                    return "ValAsignadoRFC";
                }
            }

            setValCampo("RFC", lsValue, true);

            return lbRet;
        }

        protected virtual bool ValidaRelEmpleados()
        {
            //Validar las Relaciones correspondientes a los empleados
            bool lbRet = true;

            //Validar la relación de Centros de Costos
            lbRet = ValidarRelTraslapeVig("CentroCosto-Empleado");
            if (!lbRet) return lbRet;

            // Validar que no se repitan las relaciones  
            lbRet = ValidarDuplicados();
            if (!lbRet) return lbRet;

            // Valida Exclusividad y Responsabilidad de los recursos
            lbRet = ValidarExclusividadResponsabilidad();
            if (!lbRet) return lbRet;

            return lbRet;
        }

        protected virtual bool ValidarDuplicados()
        {
            bool lbRet = true;

            //Valida que no se dupliquen las relaciones de Cada Relacion modificada
            foreach (DataTable lDataTable in pdsRelValues.Tables)
            {
                // Si no hubo cambios en la relacion continuar con las siguiente relacion
                // La realcion de centro de costos se quita pues ya se valido que no exista traslapes
                if (lDataTable.Rows.Count == 0 || lDataTable.TableName.Contains("CentroCosto"))
                {
                    continue;
                }

                // Valida que no se dupliquen las relaciones si ya existe una que se amplie su rango de
                // vigencia o que se acorte la ya existente
                lbRet = ValidarDuplicados(lDataTable.TableName);
                if (!lbRet) return lbRet;

            }
            return lbRet;
        }

        protected virtual bool ValidarDuplicados(string lsRelacion)
        {
            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            StringBuilder lsbErroresRelacion = new StringBuilder();
            string lsError;
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloEmpleados"));

            try
            {
                KeytiaBaseField lFieldEntHis = null;
                KeytiaBaseField lFieldEntidad = null;

                KeytiaRelationField lRelField;
                StringBuilder lsbErrorRegistroRel = new StringBuilder();
                DataTable lDataTable;
                StringBuilder lsbQuery = new StringBuilder();
                string lsFiltro;
                string lsDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");

                DataTable lEditedData = pdsRelValues.Tables[lsRelacion];

                lRelField = ((KeytiaRelationField)pFields.GetByConfigName(lEditedData.TableName));

                lsbQuery.AppendLine("select iCodRegistro");
                foreach (KeytiaBaseField lFieldAux in lRelField.Fields)
                {
                    if (lFieldAux.Column.StartsWith("iCodCatalogo"))
                    {
                        lsbQuery.AppendLine("   ," + lFieldAux.Column);
                        if (lFieldAux.ConfigValue.ToString() == iCodEntidad)
                        {
                            lFieldEntHis = lFieldAux;
                        }
                        else
                        {
                            lFieldEntidad = lFieldAux;
                        }
                    }
                }
                lsbQuery.AppendLine("   ,dtIniVigencia");
                lsbQuery.AppendLine("   ,dtFinVigencia");
                lsbQuery.AppendLine("from Relaciones");
                lsbQuery.AppendLine("where iCodRelacion  = " + lRelField.ConfigValue);
                lsbQuery.AppendLine("and " + lFieldEntHis.Column + " = isnull(" + iCodCatalogo + ",0)");

                List<string> lstRegistros = new List<string>();
                List<string> lstValoresEnt = new List<string>();
                foreach (DataRow lEditedRow in lEditedData.Rows)
                {
                    //se convierte a entero para validar que efectivamente traiga un entero ya que estos valores
                    //se agregan con js del lado del cliente
                    lstRegistros.Add(int.Parse(lEditedRow["iCodRegistro"].ToString()).ToString());
                    if (lEditedRow[lFieldEntidad.Column] != DBNull.Value)
                    {
                        lstValoresEnt.Add(int.Parse(lEditedRow[lFieldEntidad.Column].ToString()).ToString());
                    }
                    else
                    {
                        lstValoresEnt.Add("0");
                    }
                }
                if (lstRegistros.Count > 0)
                {
                    lsFiltro = String.Join(",", lstRegistros.ToArray());
                    lsbQuery.AppendLine("and iCodRegistro not in(" + lsFiltro + ")");
                }
                if (lstValoresEnt.Count > 0)
                {
                    lsFiltro = String.Join(",", lstValoresEnt.ToArray());
                    lsbQuery.AppendLine("and isnull(" + lFieldEntidad.Column + ",0) in(" + lsFiltro + ")");
                }

                lsbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia");
                lsbQuery.AppendLine("order by iCodRegistro");

                lDataTable = DSODataAccess.Execute(lsbQuery.ToString());

                //Agrego los datos editados a la tabla que contiene los datos guardados en la base de datos
                //para despues validar con una sola tabla
                foreach (DataRow lEditedRow in lEditedData.Rows)
                {
                    DataRow lDataRow = lDataTable.NewRow();
                    foreach (DataColumn lDataCol in lDataTable.Columns)
                    {
                        if (lEditedData.Columns.Contains(lDataCol.ColumnName))
                        {
                            lDataRow[lDataCol] = lEditedRow[lDataCol.ColumnName];
                        }
                    }
                    lDataTable.Rows.Add(lDataRow);
                }

                DataView lDataView = lDataTable.DefaultView;
                lDataView.Sort = "iCodRegistro ASC";

                lsbErroresRelacion.Length = 0;
                DataRow[] larTraslape;
                foreach (DataRowView lViewRow in lDataView)
                {
                    lsbErrorRegistroRel.Length = 0;
                    lsFiltro = "iCodRegistro <> " + lViewRow["iCodRegistro"];
                    if (lViewRow[lFieldEntHis.Column] == DBNull.Value)
                    {
                        lsFiltro += " And " + lFieldEntHis.Column + " is null";
                    }
                    else
                    {
                        lsFiltro += " And " + lFieldEntHis.Column + " = " + lViewRow[lFieldEntHis.Column];
                    }
                    if (lViewRow[lFieldEntidad.Column] == DBNull.Value)
                    {
                        lsFiltro += " And " + lFieldEntidad.Column + " is null";
                    }
                    else
                    {
                        lsFiltro += " And " + lFieldEntidad.Column + " = " + lViewRow[lFieldEntidad.Column];
                    }

                    lsFiltro += DSOControl.ComplementaVigenciasJS(lViewRow["dtIniVigencia"], lViewRow["dtFinVigencia"], false);
                    larTraslape = lDataTable.Select(lsFiltro);
                    if (larTraslape.Length > 0)
                    {

                        lstRegistros = new List<string>();
                        foreach (DataRow lDataRow in larTraslape)
                        {
                            lstRegistros.Add(lDataRow["iCodRegistro"].ToString());
                        }
                        lsError = String.Join(", ", lstRegistros.ToArray());
                        //                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RelTraslape", lsError));
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RelDuplicado", lsError));
                        lsbErrorRegistroRel.Append("<li>" + lsError + "</li>");
                    }

                    if (lsbErrorRegistroRel.Length > 0)
                    {
                        lsError = Globals.GetMsgWeb(false, "RegistroRel");
                        lsError += "(" + lViewRow["iCodRegistro"] + ")";
                        lsError = "<span>" + DSOControl.JScriptEncode(lsError) + "</span>";
                        lsbErroresRelacion.Append("<li>" + lsError);
                        lsbErroresRelacion.Append("<ul>" + lsbErrorRegistroRel.ToString() + "</ul>");
                        lsbErroresRelacion.Append("</li>");
                    }
                }

                if (lsbErroresRelacion.Length > 0)
                {
                    lsError = DSOControl.JScriptEncode(lRelField.DSOControlDB.Descripcion);
                    lsError = "<span>" + lsError + "</span>";
                    lsbErrores.Append("<li>" + lsError);
                    lsbErrores.Append("<ul>" + lsbErroresRelacion.ToString() + "</ul>");
                    lsbErrores.Append("</li>");
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

        protected virtual bool ValidarExclusividadResponsabilidad()
        {
            bool lbRet = true;
            string lsRecurso = "";
            //Valida que no se dupliquen las relaciones de Cada Relacion modificada
            foreach (DataTable lDataTable in pdsRelValues.Tables)
            {
                lsRecurso = "";
                // Si no hubo cambios en la relacion continuar con las siguiente relacion
                // La realcion de centro de costos se quita pues ya se valido que no exista traslapes
                if (lDataTable.Rows.Count == 0 || lDataTable.TableName.Contains("CentroCosto"))
                {
                    continue;
                }
                // Valida Exclusividad de Codigos de Autorizacion 
                if (lDataTable.TableName.Contains("CodAutorizacion"))
                {
                    lsRecurso = "CodAuto";
                }
                if (lDataTable.TableName.Contains("Extension"))
                {
                    lsRecurso = "Exten";
                }
                if (lDataTable.TableName.Contains("Linea"))
                {
                    lsRecurso = "Linea";
                }
                if (lsRecurso == "")
                {
                    continue;
                }
                if (lsRecurso != "Exten")
                {
                    lbRet = ValidarRelTraslapeVig(lDataTable.TableName, lsRecurso);
                }
                else
                {
                    lbRet = ValidarExclusividadResponsabilidad(lDataTable.TableName, lsRecurso);
                }
                if (!lbRet) return lbRet;

                // Valida Exclusividad y Responsabilidad de Extensiones 
                if (!lbRet) return lbRet;

            }
            return lbRet;
        }

        protected virtual bool ValidarExclusividadResponsabilidad(string lsRelacion, string lsEntidad)
        {
            //Valida que no se tralapen las vigencias de los registros de la relacion para los valores de la entidad
            //la entidad que recibe debe de ser diferente a la del historico ya que si es la misma se debe de llamar
            //el metodo que no recibe el parametro de entidad
            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            StringBuilder lsbErroresRelacion = new StringBuilder();
            string lsError;
            string lsTitulo = DSOControl.JScriptEncode(AlertTitle);

            try
            {
                KeytiaBaseField lFieldEntHis = null;
                KeytiaBaseField lFieldEntidad = null;
                KeytiaBaseField lFieldAux1 = null;
                KeytiaBaseField lFlagsEntHis = null;
                KeytiaBaseField lFlagsEntidad = null;

                KeytiaRelationField lRelField;
                StringBuilder lsbErrorRegistroRel = new StringBuilder();
                StringBuilder lsbErrorCampoRel = new StringBuilder();
                DataTable lDataTable;
                StringBuilder lsbQuery = new StringBuilder();
                string lsFiltro;
                string lsDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");

                DataTable ldtAtributos = HistoricFieldCollection.GetAtrib();
                int liCodIdioma = (int)ldtAtributos.Select("vchCodigo = '" + Globals.GetCurrentLanguage() + "'")[0]["iCodCatalogo"];

                DataTable lEditedData = pdsRelValues.Tables[lsRelacion];

                lRelField = ((KeytiaRelationField)pFields.GetByConfigName(lEditedData.TableName));

                lsbQuery.AppendLine("select iCodRegistro");
                foreach (KeytiaBaseField lFieldAux in lRelField.Fields)
                {
                    if (lFieldAux.Column.StartsWith("iCodCatalogo"))
                    {
                        lsbQuery.AppendLine("   ," + lFieldAux.Column);
                        if (lFieldAux.ConfigValue.ToString() == iCodEntidad)
                        {
                            lFieldEntHis = lFieldAux;
                        }
                        if (lFieldAux.ConfigName == lsEntidad)
                        {
                            lFieldEntidad = lFieldAux;
                        }
                    }
                    if (lFieldAux.Column.StartsWith("iFlags"))
                    {
                        lsbQuery.AppendLine("   ," + lFieldAux.Column);
                        lFieldAux1 = lRelField.Fields[lFieldAux.Column.Replace("iFlags", "iCodCatalogo")];
                        if (lFieldAux1.ConfigValue.ToString() == iCodEntidad)
                        {
                            lFlagsEntHis = lFieldAux;
                        }
                        if (lFieldAux1.ConfigName == lsEntidad)
                        {
                            lFlagsEntidad = lFieldAux;
                        }
                    }

                }
                lsbQuery.AppendLine("," + lFieldEntHis.Column + "Display = " + DSODataContext.Schema + ".GetCatDesc(" + lFieldEntHis.Column + "," + liCodIdioma + ",dtIniVigencia)");

                lsbQuery.AppendLine("   ,dtIniVigencia");
                lsbQuery.AppendLine("   ,dtFinVigencia");
                lsbQuery.AppendLine("from Relaciones");
                lsbQuery.AppendLine("where iCodRelacion  = " + lRelField.ConfigValue);

                List<string> lstRegistros = new List<string>();
                List<string> lstValoresEnt = new List<string>();
                foreach (DataRow lEditedRow in lEditedData.Rows)
                {
                    //se convierte a entero para validar que efectivamente traiga un entero ya que estos valores
                    //se agregan con js del lado del cliente
                    lstRegistros.Add(int.Parse(lEditedRow["iCodRegistro"].ToString()).ToString());
                    if (lEditedRow[lFieldEntidad.Column] != DBNull.Value)
                    {
                        lstValoresEnt.Add(int.Parse(lEditedRow[lFieldEntidad.Column].ToString()).ToString());
                    }
                    else
                    {
                        lstValoresEnt.Add("0");
                    }
                }
                if (lstRegistros.Count > 0)
                {
                    lsFiltro = String.Join(",", lstRegistros.ToArray());
                    lsbQuery.AppendLine("and iCodRegistro not in(" + lsFiltro + ")");
                }
                if (lstValoresEnt.Count > 0)
                {
                    lsFiltro = String.Join(",", lstValoresEnt.ToArray());
                    lsbQuery.AppendLine("and isnull(" + lFieldEntidad.Column + ",0) in(" + lsFiltro + ")");
                }

                lsbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia");
                lsbQuery.AppendLine("order by iCodRegistro");

                lDataTable = DSODataAccess.Execute(lsbQuery.ToString());

                //Agrego los datos editados a la tabla que contiene los datos guardados en la base de datos
                //para despues validar con una sola tabla
                foreach (DataRow lEditedRow in lEditedData.Rows)
                {
                    DataRow lDataRow = lDataTable.NewRow();
                    foreach (DataColumn lDataCol in lDataTable.Columns)
                    {
                        if (iCodCatalogo == "null" && lDataCol.ColumnName == lFieldEntHis.Column)
                        {
                            lDataRow[lDataCol] = 0;
                        }
                        else if (lEditedData.Columns.Contains(lDataCol.ColumnName))
                        {
                            lDataRow[lDataCol] = lEditedRow[lDataCol.ColumnName];
                        }
                    }
                    lDataTable.Rows.Add(lDataRow);
                }

                DataView lDataView = lDataTable.DefaultView;
                lDataView.Sort = "iCodRegistro ASC";
                int liFlags = 0;

                //Valida la responsabilidad del recurso
                lsbErroresRelacion.Length = 0;
                DataRow[] larTraslape;
                foreach (DataRowView lViewRow in lDataView)
                {
                    if ((iCodCatalogo != "null" && lViewRow[lFieldEntHis.Column].ToString() == iCodCatalogo)
                        || (iCodCatalogo == "null" && lViewRow[lFieldEntHis.Column].ToString() == "0"))
                    {
                        lsbErrorRegistroRel.Length = 0;
                        lsFiltro = "iCodRegistro <> " + lViewRow["iCodRegistro"];
                        if (lViewRow[lFlagsEntHis.Column] == DBNull.Value)
                        {
                            continue;
                        }
                        liFlags = int.Parse(lViewRow[lFlagsEntHis.Column].ToString());
                        if ((liFlags & 2) != 2)
                        {
                            continue;
                        }
                        if (lViewRow[lFieldEntidad.Column] == DBNull.Value)
                        {
                            lsFiltro += " And " + lFieldEntidad.Column + " is null";
                        }
                        else
                        {
                            lsFiltro += " And " + lFieldEntidad.Column + " = " + lViewRow[lFieldEntidad.Column];
                        }

                        lsFiltro += DSOControl.ComplementaVigenciasJS(lViewRow["dtIniVigencia"], lViewRow["dtFinVigencia"], false);
                        larTraslape = lDataTable.Select(lsFiltro);
                        if (larTraslape.Length > 0)
                        {
                            lsbErrorCampoRel.Length = 0;
                            foreach (DataRow lDataRow in larTraslape)
                            {
                                if (lDataRow[lFlagsEntHis.Column] == DBNull.Value)
                                {
                                    continue;
                                }
                                liFlags = int.Parse(lDataRow[lFlagsEntHis.Column].ToString());
                                if ((liFlags & 2) != 2)
                                {
                                    continue;
                                }

                                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RelTraslapeEntReg", lDataRow["iCodRegistro"].ToString(), lFieldEntHis.Descripcion, lDataRow[lFieldEntHis.Column + "Display"].ToString()));
                                lsbErrorCampoRel.Append("<li>" + lsError + "</li>");
                            }
                            if (lsbErrorCampoRel.Length > 0)
                            {
                                lsError = Globals.GetMsgWeb(false, "RelTraslapeResp");
                                lsError = "<span>" + DSOControl.JScriptEncode(lsError) + "</span>";
                                lsbErrorRegistroRel.Append("<li>" + lsError);
                                lsbErrorRegistroRel.Append("<ul>" + lsbErrorCampoRel.ToString() + "</ul>");
                                lsbErrorRegistroRel.Append("</li>");
                            }
                        }

                        if (lsbErrorRegistroRel.Length > 0)
                        {
                            lsError = Globals.GetMsgWeb(false, "RegistroRel");
                            lsError += "(" + lViewRow["iCodRegistro"] + ")";
                            lsError = "<span>" + DSOControl.JScriptEncode(lsError) + "</span>";
                            lsbErroresRelacion.Append("<li>" + lsError);
                            lsbErroresRelacion.Append("<ul>" + lsbErrorRegistroRel.ToString() + "</ul>");
                            lsbErroresRelacion.Append("</li>");
                        }
                    }
                }

                //Valida la Exclusividad del recurso                
                foreach (DataRowView lViewRow in lDataView)
                {
                    lsbErrorRegistroRel.Length = 0;
                    lsFiltro = "iCodRegistro <> " + lViewRow["iCodRegistro"];
                    if (lViewRow[lFlagsEntidad.Column] == DBNull.Value)
                    {
                        continue;
                    }
                    liFlags = int.Parse(lViewRow[lFlagsEntidad.Column].ToString());
                    if ((liFlags & 1) != 1)
                    {
                        continue;
                    }

                    if (lViewRow[lFieldEntidad.Column] == DBNull.Value)
                    {
                        lsFiltro += " And " + lFieldEntidad.Column + " is null";
                    }
                    else
                    {
                        lsFiltro += " And " + lFieldEntidad.Column + " = " + lViewRow[lFieldEntidad.Column];
                    }

                    lsFiltro += DSOControl.ComplementaVigenciasJS(lViewRow["dtIniVigencia"], lViewRow["dtFinVigencia"], false);
                    larTraslape = lDataTable.Select(lsFiltro);
                    if (larTraslape.Length > 0)
                    {
                        lsbErrorCampoRel.Length = 0;
                        foreach (DataRow lDataRow in larTraslape)
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RelTraslapeEntReg", lDataRow["iCodRegistro"].ToString(), lFieldEntHis.Descripcion, lDataRow[lFieldEntHis.Column + "Display"].ToString()));
                            lsbErrorCampoRel.Append("<li>" + lsError + "</li>");
                        }
                        if (lsbErrorCampoRel.Length > 0)
                        {
                            lsError = Globals.GetMsgWeb(false, "RelTraslapeExcl");
                            lsError = "<span>" + DSOControl.JScriptEncode(lsError) + "</span>";
                            lsbErrorRegistroRel.Append("<li>" + lsError);
                            lsbErrorRegistroRel.Append("<ul>" + lsbErrorCampoRel.ToString() + "</ul>");
                            lsbErrorRegistroRel.Append("</li>");
                        }
                    }

                    if (lsbErrorRegistroRel.Length > 0)
                    {
                        lsError = Globals.GetMsgWeb(false, "RegistroRel");
                        lsError += "(" + lViewRow["iCodRegistro"] + ")";
                        lsError = "<span>" + DSOControl.JScriptEncode(lsError) + "</span>";
                        lsbErroresRelacion.Append("<li>" + lsError);
                        lsbErroresRelacion.Append("<ul>" + lsbErrorRegistroRel.ToString() + "</ul>");
                        lsbErroresRelacion.Append("</li>");
                    }
                }

                if (lsbErroresRelacion.Length > 0)
                {
                    lsError = DSOControl.JScriptEncode(lRelField.DSOControlDB.Descripcion);
                    lsError = "<span>" + lsError + "</span>";
                    lsbErrores.Append("<li>" + lsError);
                    lsbErrores.Append("<ul>" + lsbErroresRelacion.ToString() + "</ul>");
                    lsbErrores.Append("</li>");
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

        protected virtual void ObtenCatalogoRelacion(DataTable lDataTable)
        {
            KeytiaBaseField lField;
            psListCatalogos = new List<KeytiaBaseField>();

            foreach (DataColumn lDataCol in lDataTable.Columns)
            {
                if (lDataCol.ColumnName.Contains("iCodCatalogo")
                    && lDataCol.ColumnName != pFieldEntidad.Column
                    && !lDataCol.ColumnName.Contains("Display"))
                {
                    lField = pRelField.Fields[lDataCol.ColumnName];
                    psListCatalogos.Add(lField);
                }
            }
        }

        protected virtual void ObtenDatosRelacion(DataTable lDataTable)
        {
            KeytiaBaseField lFieldFlags;
            int liFlagsRel;
            string lFlagEntidad;

            psListFlagsExcl = new List<string>();
            psListFlagsResp = new List<string>();

            lFlagEntidad = pFieldEntidad.Column.Replace("iCodCatalogo", "iFlags");

            ObtenCatalogoRelacion(lDataTable);

            foreach (DataColumn lDataCol in lDataTable.Columns)
            {
                if (lDataCol.ColumnName.Contains("iFlags"))
                {
                    lFieldFlags = pRelField.Fields[lDataCol.ColumnName];
                    liFlagsRel = 0;
                    if (lFieldFlags != null)
                    {
                        liFlagsRel = (int)lFieldFlags.ConfigValue;
                    }
                    if ((liFlagsRel & 1) == 1 && !lDataCol.ColumnName.Contains(lFlagEntidad))
                    {
                        psListFlagsExcl.Add(lDataCol.ColumnName);
                    }
                    if ((liFlagsRel & 2) == 2)
                    {
                        psListFlagsResp.Add(lDataCol.ColumnName);
                    }
                }
            }
        }

        protected virtual bool EmpladoResponsable()
        {
            //Validar relaciones de Responsable del recurso 
            bool lbret = true;
            DataTable lDataTable;
            DataTable ldtRealaciones;
            KDBAccess lkdb = new KDBAccess();

            DateTime ldtVigencia = DateTime.Now;

            string lsError;

            StringBuilder lsbCodRecurso = new StringBuilder();
            StringBuilder lsbErrorRelacion = new StringBuilder();
            StringBuilder lsbErrorResponsabilidad = new StringBuilder();
            StringBuilder lsbErrorRegistroRel = new StringBuilder();
            StringBuilder lsbErrores = new StringBuilder();

            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloEmpleados"));

            //Obtener las relaciones del empleados donde exsita y tenga el flag de Responsabilidad
            ldtRealaciones = ObtenRelResponsable();

            lsbErrorResponsabilidad.Length = 0;
            lsbErrores.Length = 0;
            lsbErrorRegistroRel.Length = 0;

            //Validar las realciones donde el empleado es el Responsables
            //Despues de su fecha de baja
            lkdb.FechaVigencia = pdtFinVigencia.Date;

            foreach (DataRow lDataRow in ldtRealaciones.Rows)
            {
                lsbErrorResponsabilidad.Length = 0;
                if (!(lDataRow["vchDescripcion"] is DBNull))
                {
                    lDataTable = lkdb.GetRelRegByDes(lDataRow["vchDescripcion"].ToString(),
                                                " {Emple} = " + iCodCatalogo);

                    if (lDataTable != null && lDataTable.Rows.Count > 0)
                    {
                        lsbErrorResponsabilidad = IsResponsable(lDataTable, lDataRow);
                    }
                }

                if (lsbErrorResponsabilidad.Length > 0)
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ErrIsResponsableRel",
                        lDataRow["vchDescripcion"].ToString()));
                    lsError = "<span>" + lsError + "</span>";
                    lsbErrores.Append("<li>" + lsError);
                    lsbErrores.Append("<ul>" + lsbErrorResponsabilidad.ToString() + "</ul>");
                    lsbErrores.Append("</li>");
                }
            }

            // Valida si no es responsable en el centro de costos
            lsbErrorResponsabilidad.Length = 0;
            lDataTable = lkdb.GetHisRegByEnt("CenCos", "Centro de Costos", "{Emple}= " + iCodCatalogo);
            if (lDataTable != null && lDataTable.Rows.Count > 0)
            {
                foreach (DataRow lDataRow in lDataTable.Rows)
                {
                    lsError = lDataRow["vchDescripcion"].ToString();
                    lsbErrorResponsabilidad.Append("<li>" + lsError + "</li>");
                }
            }

            if (lsbErrorResponsabilidad.Length > 0)
            {
                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ErrIsResponsableCC"));
                lsError = "<span>" + lsError + "</span>";
                lsbErrores.Append("<li>" + lsError);
                lsbErrores.Append("<ul>" + lsbErrorResponsabilidad.ToString() + "</ul>");
                lsbErrores.Append("</li>");
            }

            // Valida si no es responsable en el centro de costos
            lsbErrorResponsabilidad.Length = 0;
            lDataTable = lkdb.GetHisRegByEnt("Emple", "Empleados", "{Emple}= " + iCodCatalogo);
            if (lDataTable != null && lDataTable.Rows.Count > 0)
            {
                foreach (DataRow lDataRow in lDataTable.Rows)
                {
                    lsError = lDataRow["vchDescripcion"].ToString();
                    lsbErrorResponsabilidad.Append("<li>" + lsError + "</li>");
                }
            }

            if (lsbErrorResponsabilidad.Length > 0)
            {
                lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ErrIsResponsable"));
                lsError = "<span>" + lsError + "</span>";
                lsbErrores.Append("<li>" + lsError);
                lsbErrores.Append("<ul>" + lsbErrorResponsabilidad.ToString() + "</ul>");
                lsbErrores.Append("</li>");
            }

            if (lsbErrores.Length > 0)
            {
                lbret = false;
                lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
            }
            return lbret;
        }

        protected StringBuilder IsResponsable(DataTable lDataTable, DataRow lRelRow)
        {
            StringBuilder lsbResponsable = new StringBuilder();
            StringBuilder lsbErrorResponsabilidad = new StringBuilder();

            string lsError = "";
            string lsResponsable = "";
            string lsCatalogo = "";
            string lsRecurso = "";
            int liFlags;
            string lsFlagsResp = "";
            string lsRecursoComp = "";

            KeytiaBaseField lFieldEntidad;
            KeytiaBaseField lFieldRecurso;
            KeytiaRelationField lRelField;

            int liCodEntidad = int.Parse(iCodEntidad);

            lRelField = ((KeytiaRelationField)pFields.GetByConfigName(lRelRow["vchDescripcion"].ToString()));
            lFieldEntidad = lRelField.Fields.GetByConfigValue(liCodEntidad);

            lsbErrorResponsabilidad.Length = 0;

            foreach (DataColumn lDataCol in lRelRow.Table.Columns)
            {
                if (!lDataCol.ColumnName.Contains("iFlags") ||
                    lRelRow[lDataCol.ColumnName] is DBNull)
                {
                    continue;
                }

                liFlags = int.Parse(lRelRow[lDataCol.ColumnName].ToString());
                if ((liFlags & 2) == 2)
                {
                    lsFlagsResp = lsFlagsResp + "," + lDataCol.ColumnName;
                }
            }
            string[] lst_FlagsResp = lsFlagsResp.Split(',');

            // Si no contiene las banderas no se requiere validar 
            if (lst_FlagsResp.Length < 0)
            {
                return lsbErrorResponsabilidad;

            }
            DataTable lDataTableRec;

            for (int i = 1; i < lst_FlagsResp.Length; i++)
            {
                //Verificar si es un recurso compartido
                lsCatalogo = lst_FlagsResp[i].Replace("iFlags", "iCodCatalogo");
                if (lsCatalogo == "iCodCatalogo01")
                {
                    lsRecurso = "iCodCatalogo02";
                }
                else
                {
                    lsRecurso = "iCodCatalogo01";
                }
                lFieldRecurso = lRelField.Fields[lsRecurso];

                lsbResponsable.Length = 0;
                foreach (DataRow lDataRow in lDataTable.Rows)
                {
                    if (!lDataTable.Columns.Contains(lst_FlagsResp[i]) ||
                        lDataRow[lst_FlagsResp[i]] is DBNull)
                    {
                        continue;
                    }
                    liFlags = 0;
                    liFlags = int.Parse(lDataRow[lst_FlagsResp[i]].ToString());
                    if ((liFlags & 2) == 2 &&
                        !(lDataRow[lsCatalogo] is DBNull) &&
                        !(lRelRow[lsCatalogo] is DBNull) &&
                        !(lDataRow[lsRecurso] is DBNull) &&
                        !(lRelRow[lsRecurso] is DBNull))
                    {
                        // Obten Todas las relaciones que tienen el mismo recurso 
                        psbQuery.Length = 0;
                        psbQuery.AppendLine("select * From [VisRelaciones('" + lRelRow["vchDescripcion"] + "','" + Globals.GetCurrentLanguage() + "')] R");
                        psbQuery.AppendLine("Where iCodRelacion = " + lRelRow["iCodRegistro"]);
                        psbQuery.AppendLine("and  " + lFieldEntidad.ConfigName + " <> " + lDataRow[lsCatalogo]);
                        psbQuery.AppendLine(" and " + lFieldRecurso.ConfigName + " = " + lDataRow[lsRecurso]);
                        psbQuery.AppendLine("and   dtIniVigencia <>  dtFinVigencia ");
                        psbQuery.AppendLine("and ((dtIniVigencia <= '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "'");
                        psbQuery.AppendLine("       and dtFinVigencia > '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "')");
                        psbQuery.AppendLine("   or (dtIniVigencia <= '" + pdtFinVigencia.Date.ToString("yyyy-MM-dd") + "'");
                        psbQuery.AppendLine("       and dtFinVigencia >= '" + pdtFinVigencia.Date.ToString("yyyy-MM-dd") + "')");
                        psbQuery.AppendLine("   or (dtIniVigencia >= '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "'");
                        psbQuery.AppendLine("       and dtFinVigencia <= '" + pdtFinVigencia.Date.ToString("yyyy-MM-dd") + "'))");

                        lDataTableRec = DSODataAccess.Execute(psbQuery.ToString());
                        if (lDataTableRec == null || lDataTableRec.Rows.Count == 0)
                        {
                            continue;
                        }

                        // Obtener la descripcion del recurso compartido
                        lsRecursoComp = lDataTableRec.Rows[0][lFieldRecurso.ConfigName + "Desc"].ToString();
                        // Obten los empleados que comparten el mismo recurso
                        foreach (DataRow lRowResp in lDataTableRec.Rows)
                        {
                            lsResponsable = lRowResp[lFieldEntidad.ConfigName + "Desc"].ToString();
                            lsbResponsable.Append("<ul>" + lsResponsable + "</ul>");
                        }
                    }

                    if (lsbResponsable.Length > 0)
                    {
                        lsError = Globals.GetMsgWeb(false, "RegistroRel") + " (" + lDataRow["iCodRegistro"] + ") ";
                        lsError += DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RecursoCompartido", lsRecursoComp));
                        lsError = "<span>" + DSOControl.JScriptEncode(lsError) + "</span>";
                        lsbErrorResponsabilidad.Append("<li>" + lsError);
                        lsbErrorResponsabilidad.Append(lsbResponsable.ToString());
                        lsbErrorResponsabilidad.Append("</li>");
                    }
                }
            }

            return lsbErrorResponsabilidad;

        }

        protected DataTable ObtenRelResponsable()
        {
            DataTable ldt = null;
            DataRow[] dr;
            DataRow lRow;
            int liFlags;
            bool lbFlagsResp = false;

            object loColEmpleado = iCodEntidad;

            // Obten los campos de la relacion
            string lsFK_Relaciones = RegEdit.getColsTable("iCodCatalogo", "Relaciones");
            string lsFK_Flags = RegEdit.getColsTable("iFlags", "Relaciones");

            string[] lstFK_Relaciones = lsFK_Relaciones.Split(',');
            string[] lstFK_Flags = lsFK_Flags.Split(',');

            // Si no contiene las banderas no se requiere validar 
            if (lstFK_Flags.Length < 0)
            {
                return ldt;
            }

            // Construye el query para seleccionar aquellas relaciones donde la entidad 
            // del empleados se encuentre en la relacion
            psbQuery.Length = 0;
            psbQuery.AppendLine("Select * from Relaciones ");
            psbQuery.AppendLine("where iCodRelacion is null");
            psbQuery.AppendLine("and " + loColEmpleado + " in (" + lsFK_Relaciones + ")");
            psbQuery.AppendLine("and ( isNull(" + lstFK_Flags[0] + ",0) <> 0");

            for (int i = 1; i < lstFK_Flags.Length; i++)
            {
                psbQuery.AppendLine(" or isNull(" + lstFK_Flags[i] + ",0) <> 0");
            }
            psbQuery.AppendLine(")");

            ldt = DSODataAccess.Execute(psbQuery.ToString());

            if (ldt == null || ldt.Rows.Count == 0)
            {
                return ldt;
            }

            // Elimar las relaciones que no tienen la bandera de responsabilidad
            dr = ldt.Select("1=1");
            for (int idxRows = 0; idxRows < dr.Length; idxRows++)
            {
                //Determinar si el maestro de empleados tiene configurada la relacion de Centro de Costos - Empleados
                lbFlagsResp = false;
                foreach (DataColumn lDataCol in dr[idxRows].Table.Columns)
                {
                    if (lDataCol.ColumnName.Contains("iFlags"))
                    {
                        liFlags = 0;
                        if (!(dr[idxRows][lDataCol.ColumnName] is DBNull))
                        {
                            lRow = dr[idxRows];
                            liFlags = int.Parse(lRow[lDataCol.ColumnName].ToString());
                        }
                        if ((liFlags & 2) == 2)
                        {
                            lbFlagsResp = true;
                            continue;
                        }
                    }
                }
                if (!lbFlagsResp)
                {
                    ldt.Rows.Remove(dr[idxRows]);
                }
            }

            return ldt;

        }

        protected override void GrabarRegistro()
        {
            //RZ.20140128 Solo en modelo y cuando sea alta se dara de alta un presupuesto fijo de $50
            bool lbAltaEmple = false;

            if (State == HistoricState.Edicion && iCodRegistro == "null")
            {
                lbAltaEmple = true;
            }

            //bool lbActualizaUsuario = false;
            //if (iCodRegistro != "null")
            //{
            //    lbActualizaUsuario = CambioDatosUsuario();
            //}

            //Automáticamente forma el valor del Nombre Completo del empleado
            string lsNomCompleto = getValCampo("Nombre", "").ToString();
            lsNomCompleto = lsNomCompleto + (getValCampo("Paterno", "").ToString().Trim() == "" ? "" : " " + getValCampo("Paterno", "").ToString().Trim());
            lsNomCompleto = lsNomCompleto + (getValCampo("Materno", "").ToString().Trim() == "" ? "" : " " + getValCampo("Materno", "").ToString().Trim());
            setValCampo("NomCompleto", lsNomCompleto, true);

            base.GrabarRegistro();

            if (State != HistoricState.Baja)
            {

                //Agregar el registro de la relacion de Centro de Costo si el ultimo registro no es igual 
                // al capturado en centros de costo
                //GrabarRelCentroCostos();
                GrabarEmpledoEnRecurso();
                EnviarCartasCustodia();
            }
            else
            {
                BorrarUsuario();
                BajaCartaCustodia();

                //20141113 AM. Actualiza las llamadas de CDR que no esten dentro de las vigencias del empleado 
                //que se da de baja
                ActualizarLlamadasDeCDR();
            }
            ////Realiza la actualizacion del usuario por si el empleado se dio de baja o se actualizo su correo
            //if (lbActualizaUsuario)
            //{
            //    ActualizaUsuario();
            //}

            if (lbAltaEmple && DSODataContext.Schema == "Modelo")
            {
                //Si se trata de alta del empleado, entonces se requiere guardar el presupuesto fijo.
                int liCodRegistro;
                KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
                Hashtable lhtValues = new Hashtable();
                int iCodMaePrepEmple;

                try
                {
                    iCodMaePrepEmple = int.Parse(CCustodia.DALCCustodia.getiCodMaestro("Presupuesto Fijo", "PrepEmple"));
                    lhtValues = ObtenerConfiguracionEmpresa();
                    lhtValues.Add("{Emple}", iCodCatalogo);
                    lhtValues.Add("iCodMaestro", iCodMaePrepEmple); //Maestro de PrepEmple
                    lhtValues.Add("vchCodigo", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    lhtValues.Add("vchDescripcion", pvchDescripcion.DataValue.ToString().Replace("'", "")); //La descripcion del empleado
                    lhtValues.Add("{FechaInicioPrep}", pdtIniPeriodoActual);
                    lhtValues.Add("dtIniVigencia", DateTime.Today);
                    lhtValues.Add("dtFinVigencia", new DateTime(2079, 1, 1));
                    lhtValues.Add("iCodUsuario", Session["iCodUsuario"]);

                    if (pdPrepEmple > 0)
                    {
                        liCodRegistro = lCargasCOM.InsertaRegistro(lhtValues, "Historicos", "PrepEmple", "Presupuesto Fijo", (int)Session["iCodUsuarioDB"]);
                    }
                    else
                    {
                        //string lsError = "<ul><li>" + DSOControl.JScriptEncode("Presupuesto default para empleado no pudo ser generado, revise la configuracion de la empresa") + "</li></ul>";
                        //string lsTitulo = DSOControl.JScriptEncode("Empleados");
                        //DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                        Util.LogMessage("Presupuesto default para empleado no puede ser creado, revise la configuracion de la empresa [Presupuesto Default]");
                    }

                }
                catch (Exception ex)
                {

                    Util.LogException("Ocurrio un error al dar de alta el presupuesto fijo ", ex);
                }

                lhtValues.Clear();
            }
        }

        /// <summary>
        /// Actualiza las llamadas de CDR que no esten dentro de las vigencias del empleado que se da de baja
        /// </summary>
        private void ActualizarLlamadasDeCDR()
        {
            #region Consulta datos del empleado

            //20141113 AM. Se consultan los datos del empleado
            StringBuilder consultaDatosEmple = new StringBuilder();
            consultaDatosEmple.Append("select iCodCatalogo, dtFinVigencia from ");
            consultaDatosEmple.Append(DSODataContext.Schema);
            consultaDatosEmple.Append(".[VisHistoricos('Emple','Empleados','Español')] \r");
            consultaDatosEmple.Append("where iCodRegistro = ");
            consultaDatosEmple.Append(iCodRegistro.ToString());

            DataRow datosEmpleado = DSODataAccess.ExecuteDataRow(consultaDatosEmple.ToString());

            string iCodEmple = datosEmpleado["iCodCatalogo"].ToString();
            DateTime dtFinVigenciaEmple = Convert.ToDateTime(datosEmpleado["dtFinVigencia"]);

            #endregion Consulta datos del empleado

            #region Consulta catalogo del empleado por identificar

            //20141113 AM. Se consulta el empleado "POR IDENTIFICAR"
            StringBuilder consultaEmplePorIdentificar = new StringBuilder();
            consultaEmplePorIdentificar.Append("select max(iCodCatalogo) as iCodCatalogo from ");
            consultaEmplePorIdentificar.Append(DSODataContext.Schema);
            consultaEmplePorIdentificar.Append(".[VisHistoricos('Emple','Empleados','Español')] \r");
            consultaEmplePorIdentificar.Append("where dtIniVigencia <> dtFinVigencia and dtFinVigencia >= ");
            consultaEmplePorIdentificar.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' \r");
            consultaEmplePorIdentificar.Append("and NomCompleto like '%POR%IDENTIFICAR%'");

            DataRow datosEmplePorIdent = DSODataAccess.ExecuteDataRow(consultaEmplePorIdentificar.ToString());
            string iCodEmplePorIdent = datosEmplePorIdent["iCodCatalogo"].ToString();

            #endregion Consulta catalogo del empleado por identificar

            #region Arma Update de detalle CDR

            //20141113 AM. Se arma el query para la actualizacion de CDR
            StringBuilder updateCDR = new StringBuilder();
            updateCDR.Append("update ");
            updateCDR.Append(DSODataContext.Schema);
            updateCDR.Append(".[VisDetallados('Detall','DetalleCDR','Español')] \r");
            updateCDR.Append("set Emple = ");
            if (iCodEmplePorIdent.Length > 0)
            {
                updateCDR.Append(iCodEmplePorIdent);
            }
            else
            {
                updateCDR.Append("null");
            }
            updateCDR.Append(", dtFecUltAct = ");
            updateCDR.Append("'" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "' \r");
            updateCDR.Append("\r where Emple = ");
            updateCDR.Append(iCodEmple);
            updateCDR.Append("\r and FechaInicio >= ");
            updateCDR.Append("'" + dtFinVigenciaEmple.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'");

            #endregion Arma Update de detalle CDR

            DSODataAccess.ExecuteNonQuery(updateCDR.ToString());
        }

        /// <summary>
        /// Sirve para obtener los datos de la empresa en donde se encuentran configurados los presupuestos
        /// </summary>
        protected Hashtable ObtenerConfiguracionEmpresa()
        {
            DateTime ldtAhora = DateTime.Now;
            Hashtable lhtValuesEmpre = new Hashtable();

            psbQuery.Length = 0;
            psbQuery.AppendLine("select PeriodoPrCod, DiaInicioPeriodo, TipoPrCod, TipoPr, PeriodoPr, Empre ");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('NotifPrepEmpre','Notificaciones de Presupuestos de Empresas','" + Globals.GetCurrentLanguage() + "')] PrepEmpre");
            psbQuery.AppendLine("where PrepEmpre.Empre = (select CenCos.Empre");
            psbQuery.AppendLine("   from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCos");
            psbQuery.AppendLine("   where CenCos.iCodCatalogo = (select Emple.CenCos");
            psbQuery.AppendLine("       from " + DSODataContext.Schema + ".[VisHistoricos('Emple','Empleados','" + Globals.GetCurrentLanguage() + "')] Emple");
            psbQuery.AppendLine("       where Emple.iCodCatalogo = " + iCodCatalogo);
            psbQuery.AppendLine("       and Emple.dtIniVigencia <> Emple.dtFinVigencia");
            psbQuery.AppendLine("       and Emple.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("       and Emple.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "')");
            psbQuery.AppendLine("    and CenCos.dtIniVigencia <> CenCos.dtFinVigencia");
            psbQuery.AppendLine("    and CenCos.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("    and CenCos.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "')");
            psbQuery.AppendLine("and PrepEmpre.dtIniVigencia <> PrepEmpre.dtFinVigencia");
            psbQuery.AppendLine("and PrepEmpre.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and PrepEmpre.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");

            DataTable lDataTable = DSODataAccess.Execute(psbQuery.ToString());

            //Util.LogMessage(psbQuery.ToString());

            if (lDataTable.Rows.Count > 0)
            {
                pRowConfigEmpre = lDataTable.Rows[0];

                lhtValuesEmpre.Add("{TipoPr}", (int)pRowConfigEmpre["TipoPr"]);
                lhtValuesEmpre.Add("{PeriodoPr}", (int)pRowConfigEmpre["PeriodoPr"]);


                psbQuery.Length = 0;
                psbQuery.Append("SELECT isNull(PrepDefault,0) \r");
                psbQuery.Append("FROM " + DSODataContext.Schema + ".[VisHistoricos('Empre','Empresas','" + Globals.GetCurrentLanguage() + "')] \r");
                psbQuery.Append("WHERE dtIniVigencia <> dtFinVigencia \r");
                psbQuery.Append("and dtFinVigencia >= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                psbQuery.Append("and iCodCatalogo = " + pRowConfigEmpre["Empre"].ToString());

                //Util.LogMessage(psbQuery.ToString());

                pdPrepEmple = double.Parse(DSODataAccess.ExecuteScalar(psbQuery.ToString()).ToString());

                lhtValuesEmpre.Add("{PresupFijo}", pdPrepEmple);

                //Calcular el inicio de periodo de presupuesto
                if (pRowConfigEmpre["PeriodoPrCod"].ToString() == "Diario")
                {
                    pdtIniPeriodoActual = new DateTime(ldtAhora.Year, ldtAhora.Month, ldtAhora.Day);
                    pdtIniPeriodoSiguiente = pdtIniPeriodoActual.AddDays(1);
                }
                else if (pRowConfigEmpre["PeriodoPrCod"].ToString() == "Semanal")
                {
                    double lDias;
                    if ((int)ldtAhora.DayOfWeek + 1 < (int)pRowConfigEmpre["DiaInicioPeriodo"])
                    {
                        lDias = (7 - (int)pRowConfigEmpre["DiaInicioPeriodo"]) + (int)ldtAhora.DayOfWeek + 1;
                    }
                    else
                    {
                        lDias = (int)ldtAhora.DayOfWeek + 1 - (int)pRowConfigEmpre["DiaInicioPeriodo"];
                    }

                    pdtIniPeriodoActual = ldtAhora.AddDays(-lDias);
                    pdtIniPeriodoSiguiente = ldtAhora.AddDays(7);

                }
                else if (pRowConfigEmpre["PeriodoPrCod"].ToString() == "Mensual")
                {
                    DateTime ldtMesActual = new DateTime(ldtAhora.Year, ldtAhora.Month, 01); //fecha de inicio de periodo para el mes actual
                    DateTime ldtMesAnterior = ldtMesActual.AddMonths(-1);   //fecha de inicio de periodo para el mes anterior
                    ldtMesActual = ldtMesActual.AddDays((int)pRowConfigEmpre["DiaInicioPeriodo"] - 1);
                    if (ldtMesActual.Month > ldtAhora.Month)
                    {
                        ldtMesActual = ldtMesActual.AddDays(-ldtMesActual.Day);
                    }

                    ldtMesAnterior = ldtMesAnterior.AddDays((int)pRowConfigEmpre["DiaInicioPeriodo"] - 1);
                    if (ldtMesAnterior.Month == ldtAhora.Month)
                    {
                        ldtMesAnterior = ldtMesAnterior.AddDays(-ldtMesAnterior.Day);
                    }

                    //Si la fecha del ahora es menor que el inicio de periodo para el mes actual
                    //entonces nos encontramos en el periodo que inicio en el mes anterior
                    if (ldtAhora < ldtMesActual)
                    {
                        pdtIniPeriodoActual = ldtMesAnterior;
                    }
                    else
                    {
                        pdtIniPeriodoActual = ldtMesActual;
                    }
                    pdtIniPeriodoSiguiente = pdtIniPeriodoActual.AddMonths(1);
                }
            }

            return lhtValuesEmpre;
        }

        protected virtual void GrabarEmpledoEnRecurso()
        {
            //Actualizar el empleado del recurso 
            KeytiaBaseField lFieldCatalogo;
            DataTable ldtRecursos;
            DataTable ldtRelRecursos;
            object liEntidadCatalogo;
            DateTime ldtIniVigencia;
            DateTime ldtFinVigencia;
            try
            {
                int liCodEntidad = int.Parse(iCodEntidad);

                //Valida Cada Relacion modificada
                foreach (DataTable lDataTable in pdsRelValues.Tables)
                {
                    // Si no hubo cambios en la relacion o la relacion es la de Centro de Costos
                    //continuar con las siguiente relacion
                    if (lDataTable.Rows.Count == 0 || lDataTable.TableName == "CentroCosto-Empleado")
                    {
                        continue;
                    }

                    // Inicializa Campos

                    pRelField = ((KeytiaRelationField)pFields.GetByConfigName(lDataTable.TableName));
                    pFieldEntidad = pRelField.Fields.GetByConfigValue(liCodEntidad);
                    string lsDesRelacion = lDataTable.TableName;
                    string liCodCatalogo;

                    // Obten el Catalogo que corresponde al Recurso
                    ObtenDatosRelacion(lDataTable);
                    lFieldCatalogo = GetFieldRecurso();

                    string lvchCodEntidad = lFieldCatalogo.ConfigName;
                    liEntidadCatalogo = lFieldCatalogo.ConfigValue;
                    foreach (DataRow lDataRow in lDataTable.Rows)
                    {

                        if (lDataRow[lFieldCatalogo.Column] is DBNull)
                        {
                            continue;
                        }
                        liCodCatalogo = lDataRow[lFieldCatalogo.Column].ToString();

                        ldtIniVigencia = (DateTime)lDataRow["dtIniVigencia"];
                        ldtFinVigencia = (DateTime)lDataRow["dtFinVigencia"];

                        psbQuery.Length = 0;
                        psbQuery.AppendLine("select * from [VisHistoricos('" + lvchCodEntidad + "','" + Globals.GetCurrentLanguage() + "')] H");
                        psbQuery.AppendLine("where H.iCodCatalogo =" + liCodCatalogo);
                        psbQuery.AppendLine("and H.dtIniVigencia <> H.dtFinVigencia");
                        psbQuery.AppendLine("and (H.dtIniVigencia <= '" + ldtIniVigencia.Date.ToString("yyyy-MM-dd") + "'");
                        psbQuery.AppendLine("       and H.dtFinVigencia > '" + ldtIniVigencia.Date.ToString("yyyy-MM-dd") + "')");
                        psbQuery.AppendLine("and (H.dtIniVigencia <= '" + ldtFinVigencia.Date.ToString("yyyy-MM-dd") + "'");
                        psbQuery.AppendLine("       and H.dtFinVigencia >= '" + ldtFinVigencia.Date.ToString("yyyy-MM-dd") + "')");

                        ldtRecursos = DSODataAccess.Execute(psbQuery.ToString());

                        if (ldtRecursos == null || ldtRecursos.Rows.Count == 0)
                        {
                            continue;
                        }

                        ldtIniVigencia = (DateTime)ldtRecursos.Rows[0]["dtIniVigencia"];
                        ldtFinVigencia = (DateTime)ldtRecursos.Rows[0]["dtFinVigencia"];

                        psbQuery.Length = 0;
                        psbQuery.AppendLine("select * from [VisRelaciones('" + lsDesRelacion + "','" + Globals.GetCurrentLanguage() + "')]");
                        psbQuery.AppendLine("where " + lvchCodEntidad + "=" + liCodCatalogo);
                        psbQuery.AppendLine("and " + RangoVigente(ldtIniVigencia, ldtFinVigencia));
                        psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia");
                        if (lvchCodEntidad == "Exten")
                        {
                            psbQuery.AppendLine("and (( 1 = ((IsNull(FlagEmple,0) & 2) / 2) ) or ( 1 = ((IsNull(FlagExten,0) & 1) / 1) ))");
                        }

                        psbQuery.AppendLine("order by dtIniVigencia desc");

                        ldtRelRecursos = DSODataAccess.Execute(psbQuery.ToString());


                        // Existen Relaciones 
                        if (ldtRelRecursos.Rows.Count > 0 && !(ldtRelRecursos.Rows[0]["Emple"] is DBNull)
                            && ldtRelRecursos.Rows[0]["Emple"].ToString() == iCodCatalogo)
                        {
                            //liCodEmpleResp = (int)Util.IsDBNull(ldtEmpleadoAnt.Rows[0][lsEntidadRel], 0);
                            if (ldtRecursos.Rows[0]["Emple"] is DBNull || ldtRecursos.Rows[0]["Emple"].ToString() != iCodCatalogo)
                            {
                                GrabarAtributoEmpleado(lvchCodEntidad, ldtRecursos.Rows[0]);
                            }
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }

        }

        protected void GrabarAtributoEmpleado(string lvchCodEntidad, DataRow lDataRow)
        {
            Hashtable lhtValues = new Hashtable();
            try
            {
                //Llena los Datos necesarios 
                string lvchDesMaestro = lDataRow["vchDesMaestro"].ToString();
                int liCodRegistro = (int)lDataRow["iCodRegistro"];

                lhtValues.Add("{Emple}", iCodCatalogo);

                //Mandar llamar al COM para grabar el empleados en el recurso 
                KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
                if (!lCargasCOM.ActualizaRegistro("Historicos", lvchCodEntidad, lvchDesMaestro, lhtValues, liCodRegistro, false, (int)Session["iCodUsuarioDB"], false))
                {
                    throw new KeytiaWebException("ErrSaveRecord");
                }

            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveRecord", ex);
            }
        }

        protected string RangoVigente(DateTime ldtIniVigencia, DateTime ldtFinVigencia)
        {
            return string.Format(
                @"(   (   (    IsNull(dtIniVigencia, '{2}') <= '{0}' and IsNull(dtFinVigencia, '2079-01-01') >  '{0}')
                       or (    IsNull(dtIniVigencia, '{2}') <  '{1}' and IsNull(dtFinVigencia, '2079-01-01') >= '{1}'))
                   or (   (    '{0}' <= IsNull(dtIniVigencia, '{2}')        and '{1}' >  IsNull(dtIniVigencia, '{2}'))
                       or (    '{0}' <  IsNull(dtFinVigencia, '2079-01-01') and '{1}' >= IsNull(dtFinVigencia, '2079-01-01'))))",
                ldtIniVigencia.ToString("yyyy-MM-dd HH:mm:ss"),
                ldtFinVigencia.ToString("yyyy-MM-dd HH:mm:ss"),
                DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);

            ((TableRow)pvchCodigo.TcCtl.Parent).Style["display"] = "none";
            ((TableRow)pvchDescripcion.TcCtl.Parent).Style["display"] = "none";

            if (s == HistoricState.Consulta)
            {
                ((TableRow)pvchCodigo.TcCtl.Parent).Style.Remove("display");
                ((TableRow)pvchDescripcion.TcCtl.Parent).Style.Remove("display");
                pvchCodigo.TextBox.Enabled = false;
                pvchDescripcion.TextBox.Enabled = false;
                InitConfigPresupuestos();
            }
            else if (s == HistoricState.Edicion)
            {
                InitConfigPresupuestos();
            }

            if (pFields != null && pFields.ContainsConfigName("TipoPr"))
            {
                pTablaAtributos.Rows[pFields.GetByConfigName("TipoPr").Row - 1].Visible = false;
            }
            if (pFields != null && pFields.ContainsConfigName("PeriodoPr"))
            {
                pTablaAtributos.Rows[pFields.GetByConfigName("PeriodoPr").Row - 1].Visible = false;
            }
            if (pFields != null && pFields.ContainsConfigName("PresupFijo"))
            {
                pTablaAtributos.Rows[pFields.GetByConfigName("PresupFijo").Row - 1].Visible = false;
            }
            if (pFields != null && pFields.ContainsConfigName("PresupProv"))
            {
                pTablaAtributos.Rows[pFields.GetByConfigName("PresupProv").Row - 1].Visible = false;
            }
        }

        protected virtual void InitConfigPresupuestos()
        {
            //Revisa si la empresa a la que pertenece el empleado tiene prendida la bandera de Activar notificaciones de presupuestos
            //para mostrar el panel de subhistoricos en el cual se encuentran los configuradores de presupuesto fijo y presupuesto
            //temporal del empleado

            pPanelSubHistoricos.Visible = false;

            if (!pFields.GetByConfigName("CenCos").DSOControlDB.HasValue)
            {
                return;
            }

            DateTime ldtAhora = DateTime.Now;

            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.AppendLine("select BanderasEmpre = isnull(Empre.BanderasEmpre,0)");
            lsbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('Empre','Empresas','" + Globals.GetCurrentLanguage() + "')] Empre");
            lsbQuery.AppendLine("where Empre.iCodCatalogo = (select CenCos.Empre");
            lsbQuery.AppendLine("    from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCos");
            lsbQuery.AppendLine("    where CenCos.iCodCatalogo = " + pFields.GetByConfigName("CenCos").DataValue);
            lsbQuery.AppendLine("    and CenCos.dtIniVigencia <> CenCos.dtFinVigencia");
            lsbQuery.AppendLine("    and CenCos.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsbQuery.AppendLine("    and CenCos.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "')");
            lsbQuery.AppendLine("and Empre.dtIniVigencia <> Empre.dtFinVigencia");
            lsbQuery.AppendLine("and Empre.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsbQuery.AppendLine("and Empre.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");

            DataTable lDataTable = DSODataAccess.Execute(lsbQuery.ToString());

            if (lDataTable.Rows.Count > 0
                && ((int)lDataTable.Rows[0]["BanderasEmpre"] & 1) == 1)
            {
                lsbQuery.Length = 0;
                lsbQuery.AppendLine("select PrepEmpre.iCodCatalogo");
                lsbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('NotifPrepEmpre','Notificaciones de Presupuestos de Empresas','" + Globals.GetCurrentLanguage() + "')] PrepEmpre");
                lsbQuery.AppendLine("where PrepEmpre.Empre = (select CenCos.Empre");
                lsbQuery.AppendLine("   from " + DSODataContext.Schema + ".[VisHistoricos('CenCos','Centro de Costos','" + Globals.GetCurrentLanguage() + "')] CenCos");
                lsbQuery.AppendLine("   where CenCos.iCodCatalogo = " + pFields.GetByConfigName("CenCos").DataValue);
                lsbQuery.AppendLine("   and CenCos.dtIniVigencia <> CenCos.dtFinVigencia");
                lsbQuery.AppendLine("   and CenCos.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                lsbQuery.AppendLine("   and CenCos.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "')");
                lsbQuery.AppendLine("and PrepEmpre.dtIniVigencia <> PrepEmpre.dtFinVigencia");
                lsbQuery.AppendLine("and PrepEmpre.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                lsbQuery.AppendLine("and PrepEmpre.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                lsbQuery.AppendLine("and not exists(select Rel.Emple from " + DSODataContext.Schema + ".[VisRelaciones('Notificaciones de Presupuestos de Empresas - Excepciones de Empleados','" + Globals.GetCurrentLanguage() + "')] Rel");
                lsbQuery.AppendLine("where Rel.Emple = " + iCodCatalogo);
                lsbQuery.AppendLine("   and Rel.NotifPrepEmpre = PrepEmpre.iCodCatalogo");
                lsbQuery.AppendLine("   and Rel.dtIniVigencia <> Rel.dtFinVigencia");
                lsbQuery.AppendLine("   and Rel.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                lsbQuery.AppendLine("   and Rel.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "')");

                lDataTable = DSODataAccess.Execute(lsbQuery.ToString());

                if (lDataTable.Rows.Count > 0)
                {
                    pPanelSubHistoricos.Visible = true;
                }
            }
        }

        protected override void AgregarRegistro()
        {
            base.AgregarRegistro();
            BloqueaCampos();
        }

        protected KeytiaBaseField GetFieldRecurso()
        {
            KeytiaBaseField loField;


            for (int idxRows = 0; idxRows < psListCatalogos.Count; idxRows++)
            {
                loField = psListCatalogos[idxRows];
                if (loField.Column != pFieldEntidad.Column)
                {
                    return loField;
                }
            }

            return null;
        }

        protected Object getValCampo(string lsCampo, Object defaultValue)
        {
            Object loValue;

            if (!pFields.ContainsConfigName(lsCampo) || pFields.GetByConfigName(lsCampo).DataValue == "null")
            {
                return defaultValue;
            }

            if (defaultValue == "")
            {
                DSOTextBox ltxtContenido = (DSOTextBox)pFields.GetByConfigName(lsCampo).DSOControlDB;
                loValue = Util.IsDBNull(ltxtContenido.TextBox.Text, defaultValue);
            }
            else
            {
                loValue = Util.IsDBNull(pFields.GetByConfigName(lsCampo).DataValue, defaultValue);
            }

            return loValue;
        }

        protected void setValCampo(String lsCampo, Object defaultValue, bool lbValues)
        {
            KeytiaBaseField lField;
            if (pFields.ContainsConfigName(lsCampo))
            {
                lField = pFields.GetByConfigName(lsCampo);
                lField.DataValue = defaultValue;

                if (lbValues)
                {
                    phtValues[lField.Column] = lField.DataValue;
                }
            }
        }

        protected Object getDesCampo(String lsCampo, Object defaultValue)
        {

            KeytiaBaseField lField;

            if (pFields.ContainsConfigName(lsCampo))
            {
                lField = pFields.GetByConfigName(lsCampo);
                return Util.IsDBNull(lField.Descripcion, defaultValue);
            }

            return defaultValue;
        }

        protected string GetMsgError(string lsCampo, string lsDesCampo, string lsMsgError)
        {
            string lsError = "";
            string lsValue = "";

            lsValue = getDesCampo(lsCampo, lsDesCampo).ToString();

            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, lsMsgError, lsValue));
            lsError = "<span>" + lsError + "</span>";

            return lsError;
        }

        protected override void pbtnBaja_ServerClick(object sender, EventArgs e)
        {
            base.pbtnBaja_ServerClick(sender, e);

            iCodUsuarioEmple = getValCampo("Usuar", 0).ToString();

        }

        

        protected void EnviarCartasCustodia()
        {
            bool lbEnviaCarta = false;
            try
            {
                foreach (DataTable lDataTable in pdsRelValues.Tables)
                {
                    // Si no hubo cambios en la relacion o la relacion es la de Centro de Costos
                    //continuar con las siguiente relacion
                    if (lDataTable.Rows.Count > 0 && !(lDataTable.TableName == "CentroCosto-Empleado"))
                    {
                        lbEnviaCarta = true;
                    }

                }
                if (lbEnviaCarta && iCodCatalogo != "null")
                {
                    CartasCustodia loCartasCust = new CartasCustodia(int.Parse(iCodCatalogo));
                    loCartasCust.CartaProcesada();
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveRecord", ex);
            }
        }

        protected void BajaCartaCustodia()
        {
            KDBAccess lkdb = new KDBAccess();
            Hashtable lhtValues = new Hashtable();
            DataTable ldt;
            string liCodRegistro = "0";
            //Verificar si tiene Carta Custodia
            try
            {
                psbQuery.Length = 0;
                psbQuery.AppendLine("select * from [VisHistoricos('CartaCust','Carta Custodia','" + Globals.GetCurrentLanguage() + "')]");
                psbQuery.AppendLine("where Emple = " + iCodCatalogo);
                psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia ");
                psbQuery.AppendLine("and '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' between dtIniVigencia and dtFinVigencia");
                psbQuery.AppendLine("and '" + pdtFinVigencia.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' between dtIniVigencia and dtFinVigencia");

                ldt = DSODataAccess.Execute(psbQuery.ToString());
                if (ldt != null && ldt.Rows.Count > 0)
                {
                    liCodRegistro = ldt.Rows[0]["iCodRegistro"].ToString();
                }
                if (liCodRegistro == "0")
                {
                    return;
                }

                KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();

                lhtValues.Add("dtFinVigencia", pdtFinVigencia.Date);
                lhtValues.Add("iCodUsuario", Session["iCodUsuario"]);

                lCargasCOM.BajaHistorico(int.Parse(liCodRegistro), lhtValues, int.Parse(Session["iCodUsuarioDB"].ToString()), false, false);

            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveRecord", ex);
            }
        }

        protected void AsignarEntidadActual(string lsDesRelacion, string lsCodEntidad)
        {
            DataTable ldtEntidadAnt;
            DataTable ldtModificados;
            DateTime ldtIniVigencia;
            DateTime ldtFinVigencia;
            string lsEntidad1;
            string lsEntidad2;
            string lsEntidadRel;
            string lsColEntidadRel;
            string lsDateFormat = Globals.GetLangItem("NetDateFormat");

            if (!pFields.ContainsConfigName(lsDesRelacion)) return;

            ldtModificados = (DataTable)pFields.GetByConfigName(lsDesRelacion).DataValue;

            if (ldtModificados.Rows.Count == 0) return;

            // Obtengo el nombre de la entidad relacionada con el recurso actual (para seleccionar los históricos) y su valor
            lsEntidad1 = ((KeytiaRelationField)pFields.GetByConfigName(lsDesRelacion)).Fields["iCodCatalogo01"].ConfigName;
            lsEntidad2 = ((KeytiaRelationField)pFields.GetByConfigName(lsDesRelacion)).Fields["iCodCatalogo02"].ConfigName;
            if (lsEntidad1 == lsCodEntidad)
            {
                lsEntidadRel = lsEntidad2;
                lsColEntidadRel = "iCodCatalogo02";
            }
            else
            {
                lsEntidadRel = lsEntidad1;
                lsColEntidadRel = "iCodCatalogo01";
            }

            if (!pFields.ContainsConfigName(lsEntidadRel)) return;

            List<string> lstModificados = new List<string>();
            foreach (DataRow ldr in ldtModificados.Rows)
            {
                lstModificados.Add(ldr["iCodRegistro"].ToString());
            }
            string lsModificados = lstModificados.Count == 0 ? "0" : string.Join(",", lstModificados.ToArray());
            if (ldtModificados.Rows.Count > 0)
            {
                int liCodEntidad = 0;
                DataRow ldr = ldtModificados.Select("dtIniVigencia <> ISNULL(dtFinVigencia, #01/01/2079#)", "dtIniVigencia desc")[0];
                getVigencias(dtIniVigencia.Date, dtFinVigencia.Date, out ldtIniVigencia, out ldtFinVigencia);
                ldtEntidadAnt = DSODataAccess.Execute(string.Format(@"select * 
                    from [{0}].[VisRelaciones('{1}','{2}')]
                    where {3} = {4}
                    and not iCodRegistro in ({5})
                    and {6}
                    and dtIniVigencia <> dtFinVigencia
                    order by dtIniVigencia desc",
                DSODataContext.Schema,
                lsDesRelacion,
                Globals.GetCurrentLanguage(),
                vchCodEntidad,
                iCodCatalogo,
                lsModificados,
                RangoVigente(ldtIniVigencia, ldtFinVigencia)));

                liCodEntidad = (int)Util.IsDBNull(ldr[lsColEntidadRel], 0);
                if (ldtEntidadAnt.Rows.Count > 0)
                {
                    DateTime ldtIniVigenciaAnt;
                    getVigencias(ldtEntidadAnt.Rows[0]["dtIniVigencia"], ldr["dtIniVigencia"], out ldtIniVigenciaAnt, out ldtIniVigencia);
                    if (ldtIniVigenciaAnt.CompareTo(ldtIniVigencia) > 0)
                    {
                        liCodEntidad = (int)Util.IsDBNull(ldtEntidadAnt.Rows[0][lsEntidadRel], 0);
                    }
                }
                if (liCodEntidad > 0)
                {
                    pFields.GetByConfigName(lsEntidadRel).DataValue = liCodEntidad;
                    phtValues[pFields.GetByConfigName(lsEntidadRel).Column] = liCodEntidad;
                }
            }
        }

        protected void getVigencias(object odtIniVigencia, object odtFinVigencia, out DateTime ldtIniVigencia, out DateTime ldtFinVigencia)
        {
            if (odtIniVigencia is DBNull ||
                !(odtIniVigencia is DateTime) ||
                (DateTime)odtIniVigencia == DateTime.MinValue)
            {
                ldtIniVigencia = DateTime.Today;
            }
            else
            {
                ldtIniVigencia = (DateTime)odtIniVigencia;
            }

            if (odtFinVigencia is DBNull ||
                !(odtFinVigencia is DateTime) ||
                (DateTime)odtFinVigencia == DateTime.MinValue)
            {
                ldtFinVigencia = new DateTime(2079, 1, 1);
            }
            else
            {
                ldtFinVigencia = (DateTime)odtFinVigencia;
            }
        }

        protected override void pbtnGrabar_ServerClick(object sender, EventArgs e)
        {
            base.pbtnGrabar_ServerClick(sender, e);
            ActualizaJerarquiaRest();
        }

        protected void ActualizaJerarquiaRest()
        {
            string iCodPadre = "";
            if (pFields.ContainsConfigName(vchCodEntidad))
            {
                iCodPadre = pFields.GetByConfigName(vchCodEntidad).DataValue.ToString();
            }

            KeytiaCOM.ICargasCOM pCargaCom = (KeytiaCOM.ICargasCOM)Marshal.BindToMoniker("queue:/new:KeytiaCOM.CargasCOM");

            int liCodUsuario = (int)Session["iCodUsuarioDB"];
            pCargaCom.ActualizaJerarquiaRestEmple(iCodCatalogo, iCodPadre, liCodUsuario);
            Marshal.ReleaseComObject(pCargaCom);
            pCargaCom = null;
        }

        #region Usuarios

        protected string GeneraUsuario()
        {
            //Crea el usuario deacuerdo a lo seleccionado por el usuario
            psNewUsuario = CrearUsuario();

            if (psNewUsuario == "null")
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
                // Asigana el nuevo Usuario.
                setValCampo("Usuar", liCodRegistro, true);
            }
            else
            {
                return "ErrCrearUsuario";
            }

            return "";

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

        protected void ActualizaUsuario()
        {
            KDBAccess lkdb = new KDBAccess();
            Hashtable lhtValues = new Hashtable();
            DataTable ldt;

            try
            {
                KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();

                int liCodUsuario = int.Parse(getValCampo("Usuar", 0).ToString());

                ldt = lkdb.GetHisRegByEnt("Usuar", "Usuarios", "iCodCatalogo= " + liCodUsuario);
                if (ldt == null && ldt.Rows.Count == 0)
                {
                    return;
                }
                int liCodRegistro = (int)ldt.Rows[0]["iCodRegistro"];

                string lsEmail = getValCampo("Email", "").ToString();

                lhtValues.Add("dtIniVigencia", pdtIniVigencia.Date);
                lhtValues.Add("dtFinVigencia", pdtFinVigencia.Date);
                lhtValues.Add("{Email}", "'" + lsEmail + "'");

                //lhtValues.Add("bBajaUsuario", false);

                //Mandar llamar al COM para grabar el usuario 
                if (!lCargasCOM.GuardaUsuario(lhtValues, liCodRegistro, false, false, (int)Session["iCodUsuarioDB"]))
                {
                    throw new KeytiaWebException("ErrSaveRecord");
                }

            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveRecord", ex);
            }
        }

        protected void BorrarUsuario()
        {
            //No tiene usuario asignado
            if (iCodUsuarioEmple == "0")
            {
                return;
            }
            KDBAccess lkdb = new KDBAccess();
            Hashtable lhtValues = new Hashtable();
            DataTable ldt;

            try
            {
                KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();

                ldt = lkdb.GetHisRegByEnt("Usuar", "Usuarios", "iCodCatalogo= " + iCodUsuarioEmple);
                if (ldt == null || ldt.Rows.Count == 0)
                {
                    return;
                }
                int liCodRegistro = (int)ldt.Rows[0]["iCodRegistro"];

                lhtValues.Add("dtIniVigencia", pdtIniVigencia.Date);
                lhtValues.Add("dtFinVigencia", pdtFinVigencia.Date);
                lhtValues.Add("bBajaUsuario", true);

                //Mandar llamar al COM para grabar el usuario 
                if (!lCargasCOM.GuardaUsuario(lhtValues, liCodRegistro, false, false, (int)Session["iCodUsuarioDB"]))
                {
                    throw new KeytiaWebException("ErrSaveRecord");
                }

            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveRecord", ex);
            }
        }

        protected String CrearUsuario()
        {
            String lsUsuario = "null";

            //Crea el usuario deacuerdo en base a lo seleccionado por el usuario
            int liOpc = int.Parse(getValCampo("OpcCreaUsuar", 0).ToString());
            switch (liOpc)
            {
                case 1:
                    {
                        lsUsuario = ObtenUsuarioEnBaseNombre();
                        break;
                    }
                case 2:
                    {
                        lsUsuario = ObtenUsuarioEnBaseEmail();
                        break;
                    }

                case 3:
                    {
                        lsUsuario = ObtenUsuarioEnBaseNomina();
                        break;
                    }
            }

            return lsUsuario;
        }

        protected String ObtenUsuarioEnBaseNombre()
        {
            Usuarios oUsuario = new Usuarios();

            string lsNombre = getValCampo("Nombre", "").ToString();
            lsNombre = lsNombre.Trim().ToLower();

            string lsPaterno = getValCampo("Paterno", "").ToString();
            lsPaterno = lsPaterno.Trim().ToLower();

            string lsMaterno = getValCampo("Materno", "").ToString();
            lsMaterno = lsMaterno.Trim().ToLower();

            return oUsuario.CreaUsuario(lsNombre, lsPaterno, lsMaterno);
        }

        protected String ObtenUsuarioEnBaseEmail()
        {
            String lsUsuario = "null";
            Usuarios oUsuario = new Usuarios();

            string lsEmail = getValCampo("Email", "").ToString();
            if (lsEmail != "")
            {
                lsUsuario = oUsuario.CreaUsuario(lsEmail, true);
            }
            if (lsUsuario == "null")
            {
                lsUsuario = ObtenUsuarioEnBaseNombre();
            }

            return lsUsuario;
        }

        protected String ObtenUsuarioEnBaseNomina()
        {
            String lsUsuario = "null";

            Usuarios oUsuario = new Usuarios();

            string lsNomina = getValCampo("NominaA", "").ToString();
            if (lsNomina != "")
            {
                lsUsuario = oUsuario.CreaUsuario(lsNomina);
            }
            return lsUsuario;
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

        protected string UsuarioAsignado()
        {
            string lbRet = "";
            DataTable ldt;

            //Obten el usuario si se capturo
            int liCodUsuario = int.Parse(getValCampo("Usuar", 0).ToString());

            if (liCodUsuario == 0)
            {
                return lbRet;
            }

            if (!IsEmpleado())
            {
                return "ValUsuarioEmpleado";
            }
            int liCodCatalogo = -1;
            if (iCodCatalogo != "null")
            {
                liCodCatalogo = int.Parse(iCodCatalogo);
            }
            psbQuery.Length = 0;
            psbQuery.AppendLine("Select * from [VisHistoricos('Emple','Empleados','" + Globals.GetCurrentLanguage() + "')] ");
            psbQuery.AppendLine("Where iCodCatalogo <> " + liCodCatalogo);
            psbQuery.AppendLine("and [Usuar] = " + liCodUsuario);
            psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia ");
            psbQuery.AppendLine("and ('" + pdtIniVigencia.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' between dtIniVigencia and dtFinVigencia ");
            psbQuery.AppendLine("or '" + pdtFinVigencia.Date.ToString("yyyy-MM-dd HH:mm:ss") + "' between dtIniVigencia and dtFinVigencia )");
            ldt = DSODataAccess.Execute(psbQuery.ToString());

            if (ldt.Rows.Count > 0)
            {
                lbRet = "ValUsuarioAsignado";
            }

            return lbRet;
        }

        protected string ExiUsuarioEmailPassword()
        {
            String lbret = "";

            Usuarios oUsuario = new Usuarios();

            oUsuario.vchEmail = getValCampo("Email", "").ToString();
            oUsuario.vchCodUsuario = psNewUsuario;
            oUsuario.vchPwdUsuario = psNewPassword;

            lbret = oUsuario.ValUsuarioEmailPassword();

            return lbret;
        }

        protected Hashtable ObtenDatosUsuario()
        {
            Hashtable lhtValues = new Hashtable();
            int liCodPerfil;
            DataTable ldt;

            string lsEmail = getValCampo("Email", "").ToString();

            int liCodMaestro = int.Parse(DSODataAccess.ExecuteScalar("select iCodRegistro from Maestros where vchDescripcion = 'Usuarios' and iCodEntidad = (Select iCodRegistro from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia and vchCodigo = 'Usuar') and dtIniVigencia <> dtFinVigencia").ToString()); ;

            lhtValues.Add("vchCodigo", "'" + psNewUsuario + "'");
            lhtValues.Add("iCodMaestro", liCodMaestro);
            lhtValues.Add("vchDescripcion", pvchDescripcion.DataValue);
            lhtValues.Add("dtIniVigencia", pdtIniVigencia.Date);
            lhtValues.Add("dtFinVigencia", pdtFinVigencia.Date);
            lhtValues.Add("iCodUsuario", Session["iCodUsuario"]);

            lhtValues.Add("{Email}", "'" + lsEmail + "'");
            lhtValues.Add("{UsuarDB}", Session["iCodUsuarioDB"]);
            //NZ 20151027 Se quita este codigo hardcode y se tomara de la configuración de la empresa.
            //lhtValues.Add("{HomePage}", "'~/UserInterface/Dashboard/Dashboard.aspx?Opc=OpcdshEmpleado'");

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
                //NZ 20151027
                lhtValues.Add("{HomePage}", ObtenerHomePage(Convert.ToInt32(ldt.Rows[0]["{Empre}"])));
            }


            return lhtValues;
        }

        //NZ 20151027
        private string ObtenerHomePage(int iCodEmpre) 
        {
            StringBuilder consulta = new StringBuilder();
            consulta.AppendLine("SELECT HomePage");
            consulta.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Empre','Empresas','Español')]");
            consulta.AppendLine("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            consulta.AppendLine("   AND iCodCatalogo = " + iCodEmpre.ToString());
            DataTable dtResultado = DSODataAccess.Execute(consulta.ToString());
            if (dtResultado.Rows.Count > 0)
            {
                return "'" + dtResultado.Rows[0][0].ToString() + "'";
            }
            else 
            {
                return "'~/UserInterface/DashboardFC/Dashboard.aspx'";
            }
        }

        #endregion

        #region Tablas

        protected bool CambioDatosUsuario()
        {

            KDBAccess lkdb = new KDBAccess();

            bool lbRet = false;
            DataTable ldt;

            //Datos que pueden cambiar para actualizar el usuario
            string lsAntEmail = "";
            DateTime ldtIniVigencia;
            DateTime ldtFinVigencia;

            //Obten el usuario si se capturo
            int liCodUsuario = int.Parse(getValCampo("Usuar", 0).ToString());

            if (liCodUsuario == 0)
            {
                return lbRet;
            }


            ldt = lkdb.GetHisRegByEnt("Emple", "Empleados", "iCodRegistro= " + iCodRegistro);
            if (ldt == null && ldt.Rows.Count == 0)
            {
                return lbRet;
            }

            if (!(ldt.Rows[0]["{Email}"] is DBNull))
            {
                lsAntEmail = (string)ldt.Rows[0]["{Email}"];
            }

            string lsEmail = getValCampo("Email", "").ToString();

            if (lsAntEmail != lsEmail)
            {
                return true;
            }

            ldtIniVigencia = (DateTime)ldt.Rows[0]["dtIniVigencia"];
            ldtFinVigencia = (DateTime)ldt.Rows[0]["dtFinVigencia"];

            if (ldtIniVigencia.Date != pdtIniVigencia.Date)
            {
                return true;
            }

            if (ldtFinVigencia.Date != pdtFinVigencia.Date)
            {
                return true;
            }
            return lbRet;
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
            int liCodCatalogo = int.Parse(getValCampo("TipoEm", 0).ToString());

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

            //Si el tipo de empleado no es Recursos, Externo o Sistemas
            if (lsTipoEm == "E")
            {
                return lsNomina;
            }

            lsColumField = pFields.GetByConfigName("TipoEm").Column;

            //Obten el numero de empleados con este tipo de empleado
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("Select Count = isnull(Count(*),0)  From Catalogos ");
            lsbQuery.AppendLine("Where iCodCatalogo = " + iCodEntidad);
            lsbQuery.AppendLine("And iCodRegistro in (Select iCodCatalogo From Historicos");
            lsbQuery.AppendLine("           			Where iCodMaestro = " + iCodMaestro + ")");
            /******************************************************************************************************************
            AM 20130814 Se quita filtro de empleados externos
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

        protected string ObtenTipoPresupuesto()
        {
            string lsTipoPresupuesto = "null";
            DataTable ldt;

            //Obten el Centro de Costo si se capturo 
            int liCodCatalogo = int.Parse(getValCampo("CenCos", 0).ToString());

            if (liCodCatalogo == 0)
            {
                return lsTipoPresupuesto;
            }

            pKDB.FechaVigencia = pdtIniVigencia.Date;
            ldt = pKDB.GetHisRegByEnt("CenCos", "Centro de Costos", "iCodCatalogo= " + liCodCatalogo);
            if (ldt != null && ldt.Rows.Count > 0 && !(ldt.Rows[0]["{TipoPr}"] is DBNull))
            {
                lsTipoPresupuesto = ldt.Rows[0]["{TipoPr}"].ToString();
            }

            return lsTipoPresupuesto;
        }

        protected bool IsEmpleado()
        {
            bool lbRet = false;

            //Obten el Tipo de empleado para determinar si es empleados
            int liCodCatalogo = int.Parse(getValCampo("TipoEm", 0).ToString());
            if (liCodCatalogo == 0)
            {
                return lbRet;
            }
            psbQuery.Length = 0;
            psbQuery.AppendLine("select * from [VisHistoricos('TipoEm','" + Globals.GetCurrentLanguage() + "')]");
            psbQuery.AppendLine("where iCodCatalogo = " + liCodCatalogo);
            psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia ");
            psbQuery.AppendLine("and '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "' >= dtIniVigencia");
            psbQuery.AppendLine("and '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "' < dtFinVigencia");
            DataTable ldt = DSODataAccess.Execute(psbQuery.ToString());

            if (ldt != null && ldt.Rows.Count > 0 && !(ldt.Rows[0]["vchCodigo"] is DBNull)
                                && (ldt.Rows[0]["vchCodigo"].ToString() == "E" || ldt.Rows[0]["vchCodigo"].ToString() == "X"))
            {
                lbRet = true;
            }

            return lbRet;
        }

        protected bool IsExterno()
        {
            bool lbRet = false;

            //Obten el Tipo de empleado para determinar si es externo
            int liCodCatalogo = int.Parse(getValCampo("TipoEm", 0).ToString());

            if (liCodCatalogo == 0)
            {
                return lbRet;
            }
            psbQuery.Length = 0;
            psbQuery.AppendLine("select * from [VisHistoricos('TipoEm','" + Globals.GetCurrentLanguage() + "')]");
            psbQuery.AppendLine("where iCodCatalogo = " + liCodCatalogo);
            psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia ");
            psbQuery.AppendLine("and '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "' >= dtIniVigencia");
            psbQuery.AppendLine("and '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "' < dtFinVigencia");
            DataTable ldt = DSODataAccess.Execute(psbQuery.ToString());

            if (ldt != null && ldt.Rows.Count > 0 && !(ldt.Rows[0]["vchCodigo"] is DBNull)
                && ldt.Rows[0]["vchCodigo"].ToString() == "X")
            {
                lbRet = true;
            }

            return lbRet;
        }

        protected bool IsRespEmpleadoSame()
        {
            bool lbRet = false;

            //Obten el codigo del empleado para determinar si es externo
            int liCodCatalogo = int.Parse(getValCampo("Emple", 0).ToString());
            if (iCodCatalogo == "null")
            {
                return lbRet;
            }
            int liEntidad = int.Parse(iCodCatalogo);
            if (liCodCatalogo == liEntidad)
            {
                return true;
            }

            return lbRet;
        }

        protected bool IsRespEmpleadoExterno()
        {
            bool lbRet = false;

            //Obten el codigo del empleado para determinar si tiene valor
            int liCodCatalogo = int.Parse(getValCampo("Emple", 0).ToString());

            if (liCodCatalogo == 0)
            {
                return lbRet;
            }

            psbQuery.Length = 0;
            psbQuery.AppendLine("select * from [VisHistoricos('Emple','" + Globals.GetCurrentLanguage() + "')]");
            psbQuery.AppendLine("where iCodCatalogo = " + liCodCatalogo);
            psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia ");
            //NZ 20170720 Se omite esta validación: Que la fecha de inicio del recurso actual sea posterior al del Jefe.
            //psbQuery.AppendLine("and '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "' >= dtIniVigencia");
            psbQuery.AppendLine("and dtFinVigencia >= GETDATE()");
            psbQuery.AppendLine("and '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "' < dtFinVigencia");
            psbQuery.AppendLine("");
            

            DataTable ldt = DSODataAccess.Execute(psbQuery.ToString());
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
        protected bool IsRespEmpleado()
        {
            bool lbRet = false;

            //Obten el codigo del empleado para determinar si tiene valor
            int liCodCatalogo = int.Parse(getValCampo("Emple", 0).ToString());

            if (liCodCatalogo == 0)
            {
                return lbRet;
            }

            psbQuery.Length = 0;
            psbQuery.AppendLine("select * from [VisHistoricos('Emple','" + Globals.GetCurrentLanguage() + "')]");
            psbQuery.AppendLine("where iCodCatalogo = " + liCodCatalogo);
            psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia ");
            //NZ 20170720 Se omite esta validación: Que la fecha de inicio del recurso actual sea posterior al del Jefe.
            //psbQuery.AppendLine("and '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "' >= dtIniVigencia");
            psbQuery.AppendLine("and dtFinVigencia >= GETDATE()");
            psbQuery.AppendLine("and '" + pdtIniVigencia.Date.ToString("yyyy-MM-dd") + "' < dtFinVigencia");

            DataTable ldt = DSODataAccess.Execute(psbQuery.ToString());
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

        #endregion
    }
}

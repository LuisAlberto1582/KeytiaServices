/*
 * Nombre:		    DMM
 * Fecha:		    20111019
 * Descripción:	    Clase para el configurador de Recursos
 * Modificación:	
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Web.UI;
using System.Web;
using System.Text;
using System.Globalization;
using System.Text.RegularExpressions;

namespace KeytiaWeb.UserInterface
{
    public class CnfgRecursosEdit : HistoricEdit
    {
        public CnfgRecursosEdit()
        {
            Init += new EventHandler(CnfgRecursosEdit_Init);
        }

        void CnfgRecursosEdit_Init(object sender, EventArgs e)
        {
            this.CssClass = "HistoricEdit CnfgRecursosEdit";
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);
            pExpFiltros.Visible = false;
            pExpRegistro.Visible = false;
            pExpAtributos.Visible = false;
            pbtnConsultar.Visible = false;
            pbtnAgregar.Visible = false;
            pbtnEditar.Visible = false;
            pbtnGrabar.Visible = false;
            pbtnCancelar.Visible = false;
            pbtnBaja.Visible = false;
        }

        protected override DataTable GetDatosRegistro()
        {
            DataTable lDataTable = base.GetDatosRegistro();
            base.InitFields();
            return lDataTable;
        }
    }

    public class CnfgHisRecursosEdit : HistoricEdit
    {
        protected StringBuilder psbErrores;
        protected string psDesRelEmpleRecurs;
        protected List<Parametro> plstParams = null;
        protected bool pbEmpleadoAsignado;
        public bool AgregarDesdeEmpleado
        {
            get
            {
                if (ViewState["AgregarDesdeEmpleado"] == null)
                {
                    ViewState["AgregarDesdeEmpleado"] = false;
                }
                return (bool)ViewState["AgregarDesdeEmpleado"];
            }
            set
            {
                ViewState["AgregarDesdeEmpleado"] = value;
            }
        }

        protected int piTipoRecurso
        {
            get
            {
                if (ViewState["piTipoRecurso"] == null)
                {
                    ViewState["piTipoRecurso"] = 0;
                }
                return (int)ViewState["piTipoRecurso"];
            }
            set
            {
                ViewState["piTipoRecurso"] = value;
            }
        }

        public CnfgHisRecursosEdit()
        {
            Init += new EventHandler(CnfgRecursosEdit_Init);
            piTipoRecurso = (int)DSODataAccess.ExecuteScalar(
                "select IsNull(Recurs.iCodCatalogo, 0)\r\n" +
                "from	[" + DSODataContext.Schema + "].[VisHistoricos('Aplic','Español')] Aplic,\r\n" +
                "		[" + DSODataContext.Schema + "].[VisHistoricos('Recurs','Español')] Recurs\r\n" +
                "where	Recurs.Aplic = Aplic.iCodCatalogo\r\n" +
                "and		Aplic.ParamVarChar3 = '" + this.ToString() + "'\r\n", (Object)0);

        }

        void CnfgRecursosEdit_Init(object sender, EventArgs e)
        {
            this.CssClass = "HistoricEdit CnfgHisRecursosEdit";
        }

        public override void InitLanguage()
        {
            DeshabilitarCampos();
            AsignarTipoRecurso();
            OcultarCampos();
            base.InitLanguage();
        }

        protected void AsignarTipoRecurso()
        {
            if (pFields != null)
            {
                pFields.GetByConfigName("Recurs").DisableField();
                if (!pFields.GetByConfigName("Recurs").DSOControlDB.HasValue)
                {
                    pFields.GetByConfigName("Recurs").DataValue = (piTipoRecurso > 0 ? piTipoRecurso.ToString() : "null");
                }
            }
        }

        protected virtual void AsignarValoresCampos()
        {
            pvchDescripcion.DataValue = vchCodigo;
            if (pFields.ContainsConfigName("Sitio") && pFields.GetByConfigName("Sitio").DSOControlDB.HasValue)
            {
                try
                {
                    string lsDesSitio = DSODataAccess.ExecuteScalar(
                        String.Format("Select vchDescripcion from Historicos where iCodCatalogo = {0} and dtIniVigencia<>dtFinVigencia {1}",
                        pFields.GetByConfigName("Sitio").DSOControlDB.DataValue.ToString(),
                        DSOControl.ComplementaVigenciasJS(DateTime.Today, DateTime.Today.AddDays(1)))).ToString();
                    pvchDescripcion.DataValue = vchCodigo + " (" + lsDesSitio + ")";
                }
                catch (Exception ex)
                {
                    Util.LogException("Error al actualizar la descripción del recurso.", ex);
                }
            }
        }

        protected virtual void DeshabilitarCampos()
        {
            vchDescripcion.TextBox.Enabled = false;
        }

        protected virtual void DeshabilitarEmpleado()
        {
            if (pFields != null && pFields.ContainsConfigName("Emple"))
            {
                KeytiaBaseField lField = pFields.GetByConfigName("Emple");
                lField.DSOControlDB.Descripcion = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "Responsable"));
                psDesRelEmpleRecurs = BusquedaGenerica.getDesRelacion("Emple", vchCodEntidad);
                if (!string.IsNullOrEmpty(psDesRelEmpleRecurs) && pFields.ContainsConfigName(psDesRelEmpleRecurs))
                {
                    lField.DisableField();
                }
            }
        }

        protected virtual void OcultarCampos()
        {
            if (pFields != null && pFields.ContainsConfigName("EnviarCartaCust") && !ClienteCartaCust())
            {
                OcultarCampo("EnviarCartaCust");
            }
        }

        protected void OcultarCampo(string lsConfigName)
        {
            if (!string.IsNullOrEmpty(lsConfigName) && pFields.ContainsConfigName(lsConfigName))
            {
                DSOControlDB lCtl = pFields.GetByConfigName(lsConfigName).DSOControlDB;
                if (lCtl.Table.Rows[lCtl.Row - 1].Cells.Count == 2)
                {
                    lCtl.Table.Rows[lCtl.Row - 1].Visible = false;
                }
                else
                {
                    lCtl.TcCtl.Visible = false;
                    lCtl.TcLbl.Visible = false;
                    lCtl.Table.Rows[lCtl.Row - 1].Cells[1].ColumnSpan = 3;
                    lCtl.Table.Rows[lCtl.Row - 1].Cells[1].CssClass = "DSOTcCtl ColSpan3";
                }
            }
        }

        protected bool ClienteCartaCust()
        {
            //Checa si el cliente tiene la bandera prendida de Cartas Custodia
            // Si el cliente requiere Cuenta Maestra, valida que el campo no esté vacío
            DataRow ldrCliente;
            int liValBanderasCliente = 0;
            int liCodBanderas;
            DataTable ldtBanderasCliente;

            if (!pFields.ContainsConfigName("EnviarCartaCust")) return false;

            ldrCliente = getCliente();

            if (ldrCliente == null) return false;

            if (ldrCliente.Table.Columns.Contains("{BanderasCliente}"))
            {
                liValBanderasCliente = (int)ldrCliente["{BanderasCliente}"];
            }

            liCodBanderas = (int)Util.IsDBNull(pKDB.GetHisRegByEnt("Atrib", "Atributos",
            "vchCodigo = 'BanderasCliente'").Rows[0]["iCodCatalogo"], 0);
            ldtBanderasCliente = pKDB.GetHisRegByEnt("Valores", "Valores",
            new string[] { "{Atrib}", "{Value}" }, "[{Atrib}] = " + liCodBanderas);

            return KeytiaServiceBL.Alarmas.Alarma.getValBandera(
                                                        ldtBanderasCliente,
                                                        liValBanderasCliente,
                                                        "ClienteCartaCust",
                                                        true);
        }

        #region Grid

        protected override void InitGrid()
        {
            base.InitGrid();
            if (pFields != null && pFields.ContainsConfigName("Sitio"))
            {
                DateTime ldtIniVigencia, ldtFinVigencia;
                ldtIniVigencia = DateTime.Today;
                ldtFinVigencia = DateTime.Today.AddDays(1);

                DataTable ldtSitios = DSODataAccess.Execute(string.Format(
                    ////"Select iCodCatalogo from [{0}].GetRestriccionVigencia({1}, {2},'Sitio','{3}','{4}',1)", //20170614 NZ Se cambia funcion
                    "Select iCodCatalogo from [{0}].GetRestricPorEntidad({1}, {2},'Sitio','{3}','{4}',default)",
                    DSODataContext.Schema,
                    Session["iCodUsuario"].ToString(),
                    Session["iCodPerfil"].ToString(),
                    ldtIniVigencia.ToString("yyyy-MM-dd"),
                    ldtFinVigencia.ToString("yyyy-MM-dd")));

                pHisGrid.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetVisHistoricoParam");

                plstParams = new List<Parametro>();
                Parametro lParam = new Parametro();
                lParam.Name = "Sitio";
                lParam.Value = DataTableToString(ldtSitios, "iCodCatalogo");
                plstParams.Add(lParam);

                pHisGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerVisHistoricoParam(this, sSource, aoData, fnCallback," + DSOControl.SerializeJSON<List<Parametro>>(plstParams) + ");}";
            }
        }

        protected override void InitGridFields()
        {
            if (!(pFields != null && pFields.ContainsConfigName("Sitio")))
            {
                base.InitGridFields();
                return;
            }

            DSOGridClientColumn lCol;
            int lTarget = pHisGrid.Config.aoColumnDefs.Count;

            foreach (KeytiaBaseField lField in pFields)
            {
                if (lField.ShowInGrid)
                {
                    lCol = new DSOGridClientColumn();
                    if (lField.Column.StartsWith("iCodCatalogo"))
                    {
                        lCol.sName = lField.ConfigName + "Desc";
                    }
                    else
                    {
                        lCol.sName = lField.ConfigName;
                    }
                    lCol.aTargets.Add(lTarget++);
                    pHisGrid.Config.aoColumnDefs.Add(lCol);
                }
            }
        }

        protected override void InitHisGridLanguage()
        {
            DeshabilitarEmpleado();
            if (!(pFields != null && pFields.ContainsConfigName("Sitio")))
            {
                base.InitHisGridLanguage();
                return;
            }

            KeytiaBaseField lField;
            DSOControlDB lFiltro;
            foreach (DSOGridClientColumn lCol in pHisGrid.Config.aoColumnDefs)
            {
                lField = null;
                if (pFields.ContainsConfigName(lCol.sName))
                {
                    lField = pFields.GetByConfigName(lCol.sName);
                    lCol.sTitle = DSOControl.JScriptEncode(lField.Descripcion);
                }
                else if (lCol.sName.EndsWith("Desc") && pFields.ContainsConfigName(lCol.sName.Substring(0, lCol.sName.Length - 4)))
                {
                    lField = pFields.GetByConfigName(lCol.sName.Substring(0, lCol.sName.Length - 4));
                    lCol.sTitle = DSOControl.JScriptEncode(lField.Descripcion);
                }
                else if ((
                    lCol.sName == "vchCodigo" ||
                    lCol.sName == "vchDescripcion" ||
                    lCol.sName == "dtIniVigencia" ||
                    lCol.sName == "dtFinVigencia" ||
                    lCol.sName == "dtFecUltAct"))
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, lCol.sName));
                }
                else if (lCol.sName == "Consultar")
                {
                    string lsdoPostBack = DSOControl.JScriptEncode(Page.ClientScript.GetPostBackEventReference(this, "btnConsultar:{0}"));
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "btnConsultar"));
                    lCol.bUseRendered = false;
                    lCol.fnRender = "function(obj){ return " + pjsObj + ".fnRender(obj,\"custom-img-search\",\"" + ResolveUrl("~/images/searchsmall.png") + "\",\"" + lsdoPostBack + "\");}";
                }
                if (lCol.sName == "dtFecUltAct")
                {
                    lCol.bVisible = false;
                }
                if (phtFiltros.ContainsKey(lCol.sName))
                {
                    lFiltro = (DSOControlDB)phtFiltros[lCol.sName];
                    lFiltro.Descripcion = lCol.sTitle;
                }
                else if (lField != null && phtFiltros.ContainsKey(lField.Column))
                {
                    lFiltro = (DSOControlDB)phtFiltros[lField.Column];
                    lFiltro.Descripcion = lCol.sTitle;
                }
            }
        }

        #endregion

        #region Validaciones

        protected override bool ValidarRegistro()
        {
            psDesRelEmpleRecurs = BusquedaGenerica.getDesRelacion("Emple", vchCodEntidad);
            AsignarValoresCampos();

            //si el registro se esta eliminando entonces no es necesaria la validacion de campos obligatorios
            if (pdtIniVigencia.HasValue && pdtFinVigencia.HasValue && pdtIniVigencia.Date == pdtFinVigencia.Date)
            {
                return true;
            }

            return base.ValidarRegistro();
        }

        protected override bool ValidarRelaciones()
        {
            bool lbret = base.ValidarRelaciones();
            if (lbret)
            {
                psbErrores = new StringBuilder();

                ValidarResponsable(psDesRelEmpleRecurs);
                ValidarExclusivo(psDesRelEmpleRecurs);

                if (psbErrores.Length > 0)
                {
                    lbret = (psbErrores.Length == 0);
                    string lsError = "<ul>" + psbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, DSOControl.JScriptEncode(Title));
                }
                psbErrores = null;
            }
            return lbret;
        }

        protected void ValidarResponsable(string lsDesRelacion)
        {
            DataTable ldtResponsableNvo;
            DataTable ldtResponsableAnt;
            DataTable ldtModificados;
            DataRow[] ldrVigentes;
            DataRow[] ldrResponsableAnt;
            string lsEntidad = "Emple";
            string lsColEntidadRel;
            string lsFlag;
            string lsDateFormat = Globals.GetLangItem("NetDateFormat");
            int liCodEmpleResp = 0;
            KeytiaRelationField lRelField;
            KeytiaBaseField lEntField;

            pbEmpleadoAsignado = false;

            if (!pdsRelValues.Tables.Contains(lsDesRelacion)) return;

            lRelField = ((KeytiaRelationField)pFields.GetByConfigName(lsDesRelacion));
            if (!lRelField.Fields.ContainsConfigName(lsEntidad)) return;

            ldtModificados = pdsRelValues.Tables[lsDesRelacion];
            if (ldtModificados.Rows.Count == 0) return;

            lsColEntidadRel = lRelField.Fields.GetByConfigName(lsEntidad).Column;

            lsFlag = lsColEntidadRel.Replace("iCodCatalogo", "iFlags");
            if (!ldtModificados.Columns.Contains(lsFlag)) return;

            List<string> lstModificados = new List<string>();
            foreach (DataRow ldr in ldtModificados.Rows)
            {
                lstModificados.Add(ldr["iCodRegistro"].ToString());
            }
            string lsModificados = string.Join(",", lstModificados.ToArray());

            ldtResponsableAnt = DSODataAccess.Execute(string.Format(
                @"select iCodRegistro, {4}, dtIniVigencia, dtFinVigencia
                    from Relaciones
                    where iCodRelacion = (Select iCodRegistro from Relaciones where vchDescripcion = '{0}')
                    and {1} = {2}
                    and {3} & 2 = 2
                    and not iCodRegistro in ({4})
                    and dtIniVigencia <> dtFinVigencia",
                lsDesRelacion,
                lRelField.Fields.GetByConfigName(vchCodEntidad).Column,
                iCodCatalogo,
                lsFlag,
                lsModificados,
                lsColEntidadRel));


            //Selecciono los registros modificados marcados con la bandera de responsable
            ldtResponsableNvo = ldtModificados.Clone();
            foreach (DataRow ldr in ldtModificados.Select("dtIniVigencia <> dtFinVigencia", "dtIniVigencia desc"))
            {
                if ((int.Parse(Util.IsDBNull(ldr[lsFlag], 0).ToString()) & 2) == 2)
                {
                    DataRow ldrNvo = ldtResponsableNvo.NewRow();
                    ldrNvo.ItemArray = ldr.ItemArray;
                    ldtResponsableNvo.Rows.Add(ldrNvo);
                }
            }

            foreach (DataRow ldr in ldtResponsableNvo.Rows)
            {
                // Primero busco repetidos entre los registros agregados/modificados,  sin consultar la base de datos
                ldrVigentes = ldtResponsableNvo.Select(string.Format(
                      @"iCodRegistro <> {0}
                        and IsNull(dtIniVigencia, '{1}') <> IsNull(dtFinVigencia, '2079-01-01')
                        {2}",
                    ldr["iCodRegistro"].ToString(),
                    DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss"),
                    DSOControl.ComplementaVigenciasJS(ldr["dtIniVigencia"], ldr["dtFinVigencia"], false, "")));

                if (ldrVigentes.Length > 0)
                {
                    psbErrores.Append("<li>" + Globals.GetMsgWeb("ErrResponsableRepetido",
                       ldr[lsColEntidadRel + "Display"].ToString(),
                       ((DateTime)ldr["dtIniVigencia"]).ToString(lsDateFormat),
                       ((DateTime)ldr["dtFinVigencia"]).ToString(lsDateFormat)) + "</li>");
                }
                else
                {
                    // Comparo los registros modificados con los que existen en la base de datos
                    ldrResponsableAnt = ldtResponsableAnt.Select("1=1" +
                        DSOControl.ComplementaVigenciasJS(ldr["dtIniVigencia"], ldr["dtFinVigencia"], false, ""));

                    if (ldrResponsableAnt.Length > 0)
                    {
                        psbErrores.Append("<li>" + Globals.GetMsgWeb("ErrResponsableRepetido",
                           ldr[lsColEntidadRel + "Display"].ToString(),
                           ((DateTime)ldr["dtIniVigencia"]).ToString(lsDateFormat),
                           ((DateTime)ldr["dtFinVigencia"]).ToString(lsDateFormat)) + "</li>");
                    }
                }
            }

            ldrResponsableAnt = ldtResponsableAnt.Select("1=1" +
                DSOControl.ComplementaVigenciasJS(dtIniVigencia.Date, dtFinVigencia.Date, false, ""), "dtIniVigencia desc");

            if (ldtResponsableNvo.Rows.Count > 0)
            {
                DataRow ldr = ldtResponsableNvo.Select("", "dtIniVigencia desc")[0];
                liCodEmpleResp = (int)Util.IsDBNull(ldr[lsColEntidadRel], 0);
                if (ldrResponsableAnt.Length > 0)
                {
                    DateTime ldtIniVigenciaAnt = (DateTime)ldtResponsableAnt.Rows[0]["dtIniVigencia"];
                    if (ldtIniVigenciaAnt.CompareTo((DateTime)ldr["dtIniVigencia"]) > 0)
                    {
                        liCodEmpleResp = (int)Util.IsDBNull(ldtResponsableAnt.Rows[0][lsColEntidadRel], 0);
                    }
                }
                if (pFields.ContainsConfigName(lsEntidad) && liCodEmpleResp > 0)
                {
                    lEntField = pFields.GetByConfigName(lsEntidad);
                    lEntField.DataValue = liCodEmpleResp;
                    phtValues[lEntField.Column] = liCodEmpleResp;
                    pbEmpleadoAsignado = true;
                }
            }
            else if (ldrResponsableAnt.Length == 0)
            {
                if (pFields.ContainsConfigName(lsEntidad))
                {
                    lEntField = pFields.GetByConfigName(lsEntidad);
                    lEntField.DataValue = System.DBNull.Value;
                    phtValues[lEntField.Column] = "null";
                    pbEmpleadoAsignado = true;
                }
            }
        }

        protected void ValidarExclusivo(string lsDesRelacion)
        {
            DataTable ldtExclusivoNvo;
            DataTable ldtNoExclusivoNvo;
            DataTable ldtExclusivoAnt;
            DataTable ldtModificados;
            DataRow[] ldrVigentes;
            string lsColEntidadRel;
            string lsFlag;
            string lsDateFormat = Globals.GetLangItem("NetDateFormat");
            List<string> lstModificados = new List<string>();
            KeytiaRelationField lRelField;

            if (!pdsRelValues.Tables.Contains(lsDesRelacion)) return;

            lRelField = ((KeytiaRelationField)pFields.GetByConfigName(lsDesRelacion));

            ldtModificados = pdsRelValues.Tables[lsDesRelacion];
            if (ldtModificados.Rows.Count == 0) return;

            if (!lRelField.Fields.ContainsConfigName("Emple")) return;
            lsColEntidadRel = lRelField.Fields.GetByConfigName("Emple").Column;

            lsFlag = "iFlags" + lRelField.Fields.GetByConfigName(vchCodEntidad).Column.Replace("iCodCatalogo", ""); //La exclusividad va en el recurso
            if (!ldtModificados.Columns.Contains(lsFlag)) return;

            if (ldtModificados.Rows.Count == 0) return;

            foreach (DataRow ldr in ldtModificados.Rows)
            {
                lstModificados.Add(ldr["iCodRegistro"].ToString());
            }
            string lsModificados = string.Join(",", lstModificados.ToArray());
            ldtExclusivoAnt = DSODataAccess.Execute(string.Format(
                    @"select iCodRegistro, {4}, dtIniVigencia, dtFinVigencia
                        from Relaciones
                        where iCodRelacion = (select iCodRegistro from Relaciones where vchDescripcion = '{0}')
                        and {1} = {2}
                        and not iCodRegistro in ({3})
                        and dtIniVigencia <> dtFinVigencia",
                lsDesRelacion,
                lRelField.Fields.GetByConfigName(vchCodEntidad).Column,
                iCodCatalogo,
                string.Join(",", lstModificados.ToArray()),
                lsFlag));

            //Selecciono los registros modificados marcados con la bandera de Exclusivo
            ldtExclusivoNvo = ldtModificados.Clone();
            ldtNoExclusivoNvo = ldtModificados.Clone();
            foreach (DataRow ldr in ldtModificados.Rows)
            {
                if ((int.Parse(Util.IsDBNull(ldr[lsFlag], 0).ToString()) & 1) == 1)
                {
                    DataRow ldrNvo = ldtExclusivoNvo.NewRow();
                    ldrNvo.ItemArray = ldr.ItemArray;
                    ldtExclusivoNvo.Rows.Add(ldrNvo);
                }
                else
                {
                    DataRow ldrNvo = ldtNoExclusivoNvo.NewRow();
                    ldrNvo.ItemArray = ldr.ItemArray;
                    ldtNoExclusivoNvo.Rows.Add(ldrNvo);
                }
            }

            foreach (DataRow ldr in ldtExclusivoNvo.Rows)
            {
                // Primero busco repetidos entre los registros agregados/modificados,  sin consultar la base de datos
                ldrVigentes = ldtModificados.Select(string.Format(
                      @"iCodRegistro <> {0}
                        and IsNull(dtIniVigencia, '{1}') <> IsNull(dtFinVigencia, '2079-01-01')
                        {2}",
                    ldr["iCodRegistro"].ToString(),
                    DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss"),
                    DSOControl.ComplementaVigenciasJS((DateTime)ldr["dtIniVigencia"], (DateTime)ldr["dtFinVigencia"], false, "")));

                if (ldrVigentes.Length > 0)
                {
                    StringBuilder lsbVigencias = new StringBuilder();
                    foreach (DataRow ldrVigente in ldrVigentes)
                    {
                        lsbVigencias.Append("<li>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", ((DateTime)ldrVigente["dtIniVigencia"]).ToString(lsDateFormat), ((DateTime)ldrVigente["dtFinVigencia"]).ToString(lsDateFormat))) + "</li>");
                    }
                    psbErrores.Append("<li><span>" + Globals.GetMsgWeb("ErrExclusivo", ldr[lsColEntidadRel + "Display"].ToString()) + "</span>");
                    psbErrores.Append("<ul>" + lsbVigencias.ToString() + "</ul></li>");
                }
                else
                {
                    // Comparo los registros modificados con los que existen en la base de datos
                    DataRow[] ldrExclusivoAnt = ldtExclusivoAnt.Select("1=1" +
                        DSOControl.ComplementaVigenciasJS((DateTime)ldr["dtIniVigencia"], (DateTime)ldr["dtFinVigencia"], false, ""));

                    if (ldrExclusivoAnt.Length > 0)
                    {
                        StringBuilder lsbVigencias = new StringBuilder();
                        foreach (DataRow ldrVigente in ldrExclusivoAnt)
                        {
                            lsbVigencias.Append("<li>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", ((DateTime)ldrVigente["dtIniVigencia"]).ToString(lsDateFormat), ((DateTime)ldrVigente["dtFinVigencia"]).ToString(lsDateFormat))) + "</li>");
                        }
                        psbErrores.Append("<li><span>" + Globals.GetMsgWeb("ErrExclusivo", ldr[lsColEntidadRel + "Display"].ToString()) + "</span>");
                        psbErrores.Append("<ul>" + lsbVigencias.ToString() + "</ul></li>");
                    }
                }
            }

            ldtExclusivoNvo = ldtExclusivoAnt.Clone();
            foreach (DataRow ldr in ldtExclusivoAnt.Rows)
            {
                if ((int.Parse(Util.IsDBNull(ldr[lsFlag], 0).ToString()) & 1) == 1)
                {
                    DataRow ldrNvo = ldtExclusivoNvo.NewRow();
                    ldrNvo.ItemArray = ldr.ItemArray;
                    ldtExclusivoNvo.Rows.Add(ldrNvo);
                }
            }
            foreach (DataRow ldr in ldtNoExclusivoNvo.Rows)
            {
                // No valido entre los modificados porque ya validé en los exclusivos
                // Comparo los registros modificados con los que existen en la base de datos
                DataRow[] ldrExclusivoAnt = ldtExclusivoNvo.Select("1=1" +
                    DSOControl.ComplementaVigenciasJS((DateTime)ldr["dtIniVigencia"], (DateTime)ldr["dtFinVigencia"], false, ""));

                if (ldrExclusivoAnt.Length > 0)
                {
                    StringBuilder lsbVigencias = new StringBuilder();
                    foreach (DataRow ldrVigente in ldrExclusivoAnt)
                    {
                        lsbVigencias.Append("<li>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "RangoVigencias", ((DateTime)ldrVigente["dtIniVigencia"]).ToString(lsDateFormat), ((DateTime)ldrVigente["dtFinVigencia"]).ToString(lsDateFormat))) + "</li>");
                    }
                    psbErrores.Append("<li><span>" + Globals.GetMsgWeb("ErrExclusivo", ldr[lsColEntidadRel + "Display"].ToString()) + "</span>");
                    psbErrores.Append("<ul>" + lsbVigencias.ToString() + "</ul></li>");
                }
            }
        }

        protected virtual void ValidarFormatoNumero()
        {
            ValidarLongitud(vchCodigo, @"^\d{1,32}$", "1 - 32");
        }

        protected void ValidarLongitud(DSOControlDB ctl, string lsRegEx, string lsLongitudValida)
        {
            if (!Regex.IsMatch(ctl.ToString(), lsRegEx, RegexOptions.IgnoreCase))
            {
                psbErrores.Append("<li>" + Globals.GetMsgWeb("ErrLongitudNoValida",
                    ctl.Descripcion,
                    ctl.ToString(), lsLongitudValida) + "</li>");
            }
        }

        protected void ValidarConfigSitio()
        {
            if (pFields.ContainsConfigName("Sitio") && pFields.GetByConfigName("Sitio").DSOControlDB.HasValue)
            {
                DataTable ldtGuardados = DSODataAccess.Execute(string.Format(@"select * 
                  from [VisHistoricos('Sitio','{0}')]
                  where iCodCatalogo = {1}
                  and dtIniVigencia <> dtFinVigencia
                  {2}",
                    Globals.GetCurrentLanguage(),
                    pFields.GetByConfigName("Sitio").DataValue.ToString(),
                    DSOControl.ComplementaVigenciasJS(dtIniVigencia.Date, dtFinVigencia.Date, false, "")));

                foreach (DataRow ldr in ldtGuardados.Rows)
                {
                    ValidarSitio(ldr);
                }
            }

        }

        protected virtual void ValidarSitio(DataRow ldrSitio)
        {
            //Se sobrescribe en cada recurso
        }

        protected void AsignarEntidadActual(string lsDesRelacion, string lsEntidad)
        {
            DataTable ldtGuardados;
            DataTable ldtModificados;
            string lsColEntidadRel;
            KeytiaRelationField lRelField;
            KeytiaBaseField lEntField;

            if (!pdsRelValues.Tables.Contains(lsDesRelacion)) return;
            if (!pFields.ContainsConfigName(lsEntidad)) return;

            lRelField = ((KeytiaRelationField)pFields.GetByConfigName(lsDesRelacion));
            if (!lRelField.Fields.ContainsConfigName(lsEntidad)) return;

            ldtModificados = pdsRelValues.Tables[lsDesRelacion];
            if (ldtModificados.Rows.Count == 0) return;

            lEntField = pFields.GetByConfigName(lsEntidad);
            lsColEntidadRel = lRelField.Fields.GetByConfigName(lsEntidad).Column;

            List<string> lstModificados = new List<string>();
            foreach (DataRow ldr in ldtModificados.Rows)
            {
                lstModificados.Add(ldr["iCodRegistro"].ToString());
            }
            string lsModificados = string.Join(",", lstModificados.ToArray());
            DataRow[] ldrModificadosActivos = ldtModificados.Select("dtIniVigencia <> dtFinVigencia", "dtIniVigencia desc");
            int liCodEntidadActual = 0;
            DateTime ldtIniVigenciaActual = DateTime.MinValue;
            if (ldrModificadosActivos.Length > 0)
            {
                liCodEntidadActual = (int)Util.IsDBNull(ldrModificadosActivos[0][lsColEntidadRel], 0);
                ldtIniVigenciaActual = (DateTime)ldrModificadosActivos[0]["dtIniVigencia"];
            }

            ldtGuardados = DSODataAccess.Execute(string.Format(@"
                select top 1 {0}
                from Relaciones
                where iCodRelacion = (Select iCodRegistro from Relaciones where vchDescripcion = '{1}')
                and {2} = {3}
                and not iCodRegistro in ({4})
                and dtIniVigencia > '{5}'
                and dtIniVigencia <> dtFinVigencia
                {6}
                order by dtIniVigencia desc",
            lsColEntidadRel,
            lsDesRelacion,
            lRelField.Fields.GetByConfigName(vchCodEntidad).Column,
            iCodCatalogo,
            lsModificados,
            ldtIniVigenciaActual.ToString("yyyy-MM-dd HH:mm:ss"),
            DSOControl.ComplementaVigenciasJS(dtIniVigencia.Date, dtFinVigencia.Date, false, "")));

            if (ldtGuardados.Rows.Count > 0)
            {
                liCodEntidadActual = (int)Util.IsDBNull(ldtGuardados.Rows[0][lsColEntidadRel], 0);
            }
            if (liCodEntidadActual > 0)
            {
                lEntField.DataValue = liCodEntidadActual;
                phtValues[lEntField.Column] = liCodEntidadActual;
            }
            else
            {
                lEntField.DataValue = System.DBNull.Value;
                phtValues[lEntField.Column] = "null";
            }
        }

        #endregion

        protected override void GrabarRegistro()
        {
            base.GrabarRegistro();
            EnviarCartasCustodia();
        }

        protected void EnviarCartasCustodia()
        {
            try
            {
                if (pFields != null && pFields.ContainsConfigName(psDesRelEmpleRecurs)
                    && pFields.ContainsConfigName("EnviarCartaCust")
                    && pFields.GetByConfigName("EnviarCartaCust").DSOControlDB.HasValue
                    && ((int)pFields.GetByConfigName("EnviarCartaCust").DataValue & 1) == 1)
                {
                    DataTable ldtModificados = (DataTable)pFields.GetByConfigName(psDesRelEmpleRecurs).DataValue;
                    string lsEntidad1 = ((KeytiaRelationField)pFields.GetByConfigName(psDesRelEmpleRecurs)).Fields["iCodCatalogo01"].ConfigName;
                    string lsEntidad2 = ((KeytiaRelationField)pFields.GetByConfigName(psDesRelEmpleRecurs)).Fields["iCodCatalogo02"].ConfigName;
                    string lsColEntidadRel;
                    if (lsEntidad1 == vchCodEntidad)
                    {
                        lsColEntidadRel = "iCodCatalogo02";
                    }
                    else
                    {
                        lsColEntidadRel = "iCodCatalogo01";
                    }

                    foreach (DataRow ldr in ldtModificados.Rows)
                    {
                        if (!(ldr[lsColEntidadRel] is DBNull))
                        {
                            CartasCustodia loCartasCust = new CartasCustodia((int)ldr[lsColEntidadRel]);
                            loCartasCust.CartaProcesada();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveRecord", ex);
            }
        }


        public static string DataTableToString(DataTable ldt, string lsColumna)
        {
            List<string> lstValores = new List<string>();
            foreach (DataRow ldr in ldt.Rows)
            {
                lstValores.Add(Util.IsDBNull(ldr[lsColumna], 0).ToString());
            }
            if (lstValores.Count == 0)
            {
                lstValores.Add("0");
            }
            return string.Join(",", lstValores.ToArray());
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

        protected DataRow getCliente()
        {
            int liCodUsuario = (int)Session["iCodUsuario"];
            DataRow ldr = null;
            try
            {
                DataTable ldt = pKDB.GetHisRegByEnt("Usuar", "Usuarios",
                    new string[] { "{Empre}" },
                    "iCodCatalogo = " + liCodUsuario);
                if (ldt != null && ldt.Rows.Count > 0 && !(ldt.Rows[0]["{Empre}"] is DBNull))
                {
                    ldt = pKDB.GetHisRegByEnt("Empre", "Empresas",
                        new string[] { "{Client}" },
                        "iCodCatalogo = " + ldt.Rows[0]["{Empre}"].ToString());
                    if (ldt != null && ldt.Rows.Count > 0 && !(ldt.Rows[0]["{Client}"] is DBNull))
                    {
                        ldt = pKDB.GetHisRegByEnt("Client", "Clientes",
                            "iCodCatalogo = " + ldt.Rows[0]["{Client}"].ToString());
                        if (ldt != null && ldt.Rows.Count > 0)
                        {
                            ldr = ldt.Rows[0];
                        }
                    }
                }
            }
            catch (Exception ex) { }
            return ldr;
        }

    }

    public class CnfgExtenEdit : CnfgHisRecursosEdit
    {
        protected override bool ValidarClaves()
        {
            if (!pFields.ContainsConfigName("Sitio"))
                return base.ValidarClaves();

            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            StringBuilder lsbErroresCodigos = new StringBuilder();
            string lsError;
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, lblTitle.Text));
            DataTable ldt;

            try
            {
                if (pvchCodigo.HasValue)
                {
                    psbQuery.Length = 0;
                    psbQuery.AppendLine("select H.iCodRegistro, H.iCodCatalogo, H.iCodMaestro, H.dtIniVigencia, H.dtFinVigencia");
                    psbQuery.AppendLine("from Historicos H, Catalogos C,");
                    psbQuery.AppendLine("   [" + DSODataContext.Schema + "].[VisHistoricos('" + vchCodEntidad + "','" + Globals.GetCurrentLanguage() + "')] V");
                    psbQuery.AppendLine("where H.iCodRegistro = V.iCodRegistro");
                    psbQuery.AppendLine("and H.iCodCatalogo = C.iCodRegistro");
                    psbQuery.AppendLine("and H.iCodRegistro <> isnull(" + iCodRegistro + ",-1)");
                    psbQuery.AppendLine("and H.iCodCatalogo <> isnull(" + iCodCatalogo + ",-1)");
                    psbQuery.AppendLine("and C.iCodCatalogo = " + iCodEntidad);
                    psbQuery.AppendLine("and C.vchCodigo = " + pvchCodigo.DataValue);
                    psbQuery.AppendLine("and V.Sitio = IsNull(" + pFields.GetByConfigName("Sitio").DataValue.ToString() + ", -1)");
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

        protected override bool ValidarDatos()
        {
            bool lbret = true;
            psbErrores = new StringBuilder();

            ValidarConfigSitio();
            ValidarFormatoNumero();

            if (psbErrores.Length > 0)
            {
                lbret = false;
                string lsError = "<ul>" + psbErrores.ToString() + "</ul>";
                DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, DSOControl.JScriptEncode(Title));
            }
            psbErrores = null;
            return lbret;
        }

        protected override void ValidarSitio(DataRow ldrSitio)
        {
            if (!ExtEnRango(vchCodigo.ToString(), ldrSitio))
            {
                psbErrores.Append("<li>" + Globals.GetMsgWeb("ErrExtFueraRango",
                   ldrSitio["vchDescripcion"].ToString()) + "</li>"); //La longitud del rango no cumple con la validación del sitio
            }
        }

        protected override void GrabarRegistro()
        {
            base.GrabarRegistro();
        }

        //20140626 AM. Se agrega metodo sobrecargado para no validar que la fecha de inicio de la relacion empleado - extension 
        //                    sea menor a la fecha de inicio de la extensión.
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

                //20140626 AM. Se agrega iCodMaestro a la consulta
                lsbQueryHis.AppendLine("    H.iCodMaestro,");

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
                        //20140626 AM. Se agrega a la condicion que no tome en cuenta la validacion de vigencias si el registro tiene iCodMaestro = 7  --iCodMaestro de empleados
                        //else if ((DateTime)ladrHis[0]["dtIniVigencia"] > pdtIniVigencia.Date)
                        else if ((DateTime)ladrHis[0]["dtIniVigencia"] > pdtIniVigencia.Date && (int)ladrHis[0]["iCodMaestro"] != 7)
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

        protected Boolean ExtEnRango(string lsExtension, DataRow ldrSitio)
        {
            int lAux;
            int liExtMin;
            int liExtMax;
            int liExtension;

            string[] lsRangos;
            string[] lsExtMinMax;

            int liCodBanderas;
            DataTable ldtBanderasSitio;

            int.TryParse(lsExtension, out liExtension);
            if (ldrSitio.Table.Columns.Contains("BanderasSitio"))
            {
                liCodBanderas = (int)Util.IsDBNull(pKDB.GetHisRegByEnt("Atrib", "Atributos", "vchCodigo = 'BanderasSitio'").Rows[0]["iCodCatalogo"], 0);
                ldtBanderasSitio = pKDB.GetHisRegByEnt("Valores", "Valores", new string[] { "{Atrib}", "{Value}" }, "[{Atrib}] = " + liCodBanderas);

                if (KeytiaServiceBL.Alarmas.Alarma.getValBandera(
                        ldtBanderasSitio,
                        (int)Util.IsDBNull(ldrSitio["BanderasSitio"], 0),
                        "ExtUnDigito",
                        false)
                    && lsExtension.Trim().Length == 1)
                {
                    return true;
                }
            }
            if (ldrSitio.Table.Columns.Contains("RangosExt") && !Util.IsDBNull(ldrSitio["RangosExt"], "").ToString().Equals(""))
            {
                lsRangos = ldrSitio["RangosExt"].ToString().Split(new Char[] { ',' });
                liExtMin = int.MaxValue;
                liExtMax = int.MinValue;
                for (lAux = 0; lAux < lsRangos.Length; lAux++)
                {
                    lsExtMinMax = lsRangos[lAux].Split(new Char[] { '-' });
                    if (lsExtMinMax.Length == 1)
                    {
                        int.TryParse(lsExtMinMax[0], out liExtMin);
                        int.TryParse(lsExtMinMax[0], out liExtMax);
                    }
                    if (lsExtMinMax.Length == 2)
                    {
                        int.TryParse(lsExtMinMax[0], out liExtMin);
                        int.TryParse(lsExtMinMax[1], out liExtMax);
                    }

                    if (liExtension >= liExtMin && liExtension <= liExtMax)
                    {
                        return true;
                    }
                }
            }
            else if (ldrSitio.Table.Columns.Contains("ExtIni") && ldrSitio.Table.Columns.Contains("ExtFin")
                && !(ldrSitio["ExtIni"] is DBNull) && !(ldrSitio["ExtFin"] is DBNull))
            {
                if (liExtension >= (int)ldrSitio["ExtIni"] && liExtension <= (int)ldrSitio["ExtFin"])
                {
                    return true;
                }
            }
            else if (ldrSitio.Table.Columns.Contains("LongExt") && !(ldrSitio["LongExt"] is DBNull))
            {
                if (lsExtension.Trim().Length == (int)ldrSitio["LongExt"])
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class CnfgCodigosAutorizacionEdit : CnfgHisRecursosEdit
    {
        protected override void InitFields()
        {
            base.InitFields();
            if (pFields != null)
            {
                /* En el configurador de códigos debe existir la opción de modificar el COS de cada código */
                AgregarBoton("Cos");
            }
        }

        protected override bool ValidarClaves()
        {
            if (!pFields.ContainsConfigName("Sitio"))
                return base.ValidarClaves();

            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            StringBuilder lsbErroresCodigos = new StringBuilder();
            string lsError;
            DataTable ldt;

            try
            {
                if (pvchCodigo.HasValue)
                {
                    psbQuery.Length = 0;
                    psbQuery.AppendLine("select H.iCodRegistro, H.iCodCatalogo, H.iCodMaestro, H.dtIniVigencia, H.dtFinVigencia");
                    psbQuery.AppendLine("from Historicos H, Catalogos C,");
                    psbQuery.AppendLine("   [" + DSODataContext.Schema + "].[VisHistoricos('" + vchCodEntidad + "','" + Globals.GetCurrentLanguage() + "')] V");
                    psbQuery.AppendLine("where H.iCodRegistro = V.iCodRegistro");
                    psbQuery.AppendLine("and H.iCodCatalogo = C.iCodRegistro");
                    psbQuery.AppendLine("and H.iCodRegistro <> isnull(" + iCodRegistro + ",-1)");
                    psbQuery.AppendLine("and H.iCodCatalogo <> isnull(" + iCodCatalogo + ",-1)");
                    psbQuery.AppendLine("and C.iCodCatalogo = " + iCodEntidad);
                    psbQuery.AppendLine("and C.vchCodigo = " + pvchCodigo.DataValue);
                    psbQuery.AppendLine("and V.Sitio = IsNull(" + pFields.GetByConfigName("Sitio").DataValue.ToString() + ", -1)");
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
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, DSOControl.JScriptEncode(Title));
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }

            return lbret;
        }

        protected override bool ValidarRelaciones()
        {
            bool lbret = base.ValidarRelaciones() &&
                ValidarRelTraslapeVig(psDesRelEmpleRecurs);


            if (lbret && !pbEmpleadoAsignado)
            {
                psbErrores = new StringBuilder();

                AsignarEntidadActual(psDesRelEmpleRecurs, "Emple");

                if (psbErrores.Length > 0)
                {
                    lbret = false;
                    string lsError = "<ul>" + psbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, DSOControl.JScriptEncode(Title));
                }
                psbErrores = null;
            }
            return lbret;
        }

        protected override bool ValidarDatos()
        {
            bool lbret = base.ValidarDatos();
            if (lbret)
            {
                psbErrores = new StringBuilder();

                //ValidarFormatoNumero();
                ValidarConfigSitio();

                if (psbErrores.Length > 0)
                {
                    lbret = false;
                    string lsError = "<ul>" + psbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, DSOControl.JScriptEncode(Title));
                }
                psbErrores = null;
            }
            return lbret;
        }

        protected override void ValidarSitio(DataRow ldrSitio)
        {
            ValidarLongitudCasilla(ldrSitio);
        }

        protected override void GrabarRegistro()
        {
            base.GrabarRegistro();
        }

        //20140626 AM. Se agrega metodo sobrecargado para no validar que la fecha de inicio de la relacion empleado - codigo de autorización 
        //                    sea <= a la fecha de inicio de la extensión.
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

                //20140626 AM. Se agrega iCodMaestro a la consulta
                lsbQueryHis.AppendLine("    H.iCodMaestro,");

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
                        //20140626 AM. Se agrega a la condicion que no tome en cuenta la validacion de vigencias si el registro tiene iCodMaestro = 7  --iCodMaestro de empleados
                        //else if ((DateTime)ladrHis[0]["dtIniVigencia"] > pdtIniVigencia.Date)
                        else if ((DateTime)ladrHis[0]["dtIniVigencia"] > pdtIniVigencia.Date && (int)ladrHis[0]["iCodMaestro"] != 7)
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

        protected void ValidarLongitudCasilla(DataRow ldrSitio)
        {
            int liLongCasilla = 0;
            DataTable lDataTable;
            string lsDateFormat = Globals.GetLangItem("NetDateFormat");

            if (ldrSitio.Table.Columns.Contains("LongCasilla"))
            {
                liLongCasilla = (int)Util.IsDBNull(ldrSitio["LongCasilla"], 0);
            }
            KeytiaBaseField lFieldSitio = pFields.GetByConfigName("Sitio");

            lDataTable = DSODataAccess.Execute(
                string.Format(@"
                        Select iCodCatalogo,
                                vchCodigo,
                                Casilla = Left(vchCodigo, {5}),
                                Codigo = Right(vchCodigo, Len(vchCodigo) - {5})
                        from Historicos H
                            inner join (select iCodRegistroCat = iCodRegistro, vchCodigo from catalogos) cat
                            on cat.iCodRegistroCat = H.iCodCatalogo
                        where H.iCodMaestro = {0}
                        and H.iCodCatalogo <> IsNull({1}, -1)
                        and H.{2} = IsNull({3}, -1)
                        and H.dtIniVigencia <> H.dtFinVigencia
                        {4}",
                    iCodMaestro,
                    iCodCatalogo,
                    lFieldSitio.Column,
                    lFieldSitio.DataValue.ToString(),
                    DSOControl.ComplementaVigenciasJS(dtIniVigencia.Date, dtFinVigencia.Date, false, "H."),
                    liLongCasilla));

            // Validar Código
            /*RZ.20130213 Se realiza un ajuste en metodo select, cuando el Cod. Aut es mayor a el maximo valor 
             * de un int este select marca un error al no poder comparar implicitamente un string con un entero
             * por lo que en la consulta ahora se compara un string (codigos dados de alta) con otro string (codigo a validar si no existe ya)
             * Cambio aplica tanto en la validacion del codigo, como en las casillas.
             */
            if (lDataTable.Select("Codigo = '" + vchCodigo.ToString().Substring(liLongCasilla) + "'").Length > 0)
            {
                // Código repetido
                psbErrores.Append("<li>" + Globals.GetMsgWeb("ErrCodigoRepetido",
                   vchCodigo.ToString().Substring(liLongCasilla),
                   dtIniVigencia.Date.ToString(lsDateFormat),
                   dtFinVigencia.Date.ToString(lsDateFormat)) + "</li>");
            }

            // Validar Casilla
            if (liLongCasilla > 0 && lDataTable.Select("Casilla = '" + vchCodigo.ToString().Substring(0, liLongCasilla) + "'").Length > 0)
            {
                // Casilla repetida
                psbErrores.Append("<li>" + Globals.GetMsgWeb("ErrCasillaRepetida",
                   vchCodigo.ToString().Substring(0, liLongCasilla),
                   dtIniVigencia.Date.ToString(lsDateFormat),
                   dtFinVigencia.Date.ToString(lsDateFormat)) + "</li>");
            }
        }

        protected void ReplicarCodAuto()
        {
            if (Replicar())
            {
                DateTime ldtIniVigencia = dtIniVigencia.Date;
                DateTime ldtFinVigencia = dtFinVigencia.Date;
                KeytiaBaseField lField = pFields.GetByConfigName("Sitio");
                string liCodSitio = lField.DataValue.ToString();
                DataTable ldtSitios = DSODataAccess.Execute(string.Format(
                    @"select iCodCatalogo from Historicos where iCodMaestro = (Select iCodMaestro
	                    from Historicos where iCodCatalogo = {0}
                        and dtIniVigencia <> dtFinVigencia
                        {1}) 
                    and iCodCatalogo <> {0}
                    and dtIniVigencia <> dtFinVigencia
                    {1}",
                    liCodSitio,
                    DSOControl.ComplementaVigenciasJS(ldtIniVigencia, ldtFinVigencia, false, "")));

                DataTable ldtCodAuto = DSODataAccess.Execute(string.Format(
                    "select * from Historicos where iCodMaestro = {0} and iCodRegistro <> {1} and {2} in ({3})" +
                    "and (select vchCodigo from Catalogos where iCodRegistro = Historicos.iCodCatalogo) = '{4}'",
                    iCodMaestro,
                    iCodRegistro,
                    lField.Column,
                    DataTableToString(ldtSitios, "iCodCatalogo"),
                    vchCodigo));

                KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
                foreach (DataRow ldrSitio in ldtSitios.Rows)
                {
                    Hashtable lhtValores = new Hashtable(phtValues);
                    lhtValores[lField.Column] = ldrSitio["iCodCatalogo"];
                    DataRow[] ldrCodAuto = ldtCodAuto.Select(lField.Column + " = " + ldrSitio["iCodCatalogo"]);
                    if (ldrCodAuto.Length > 0)
                    {
                        foreach (DataColumn ldc in ldtCodAuto.Columns)
                        {
                            if (ldc.ColumnName != "iCodRegistro" &&
                                ldc.ColumnName != "iCodCatalogo" &&
                                ldc.ColumnName != "iCodMaestro" &&
                                ldc.ColumnName != "dtIniVigencia" &&
                                ldc.ColumnName != "dtFinVigencia" &&
                                ldc.ColumnName != "dtFecUltAct" &&
                                lhtValores.ContainsKey(ldc.ColumnName)
                                )
                            {
                                if (ldc.DataType == typeof(string))
                                {
                                    string lsValor = Util.IsDBNull(ldrCodAuto[0][ldc], "").ToString();
                                    lhtValores[ldc.ColumnName] = string.IsNullOrEmpty(lsValor) ? "null" : "'" + lsValor + "'";
                                }
                                else
                                {
                                    lhtValores[ldc.ColumnName] = ldrCodAuto[0][ldc];
                                }
                            }
                        }
                        if (!lCargasCOM.ActualizaRegistro("Historicos", vchCodEntidad, vchDesMaestro, lhtValores, (int)ldrCodAuto[0]["iCodRegistro"], false, (int)Session["iCodUsuarioDB"], pbReplicarClientes.CheckBox.Checked))
                        {
                            throw new KeytiaWebException("ErrSaveRecord");
                        }
                    }
                    else
                    {
                        lCargasCOM.InsertaRegistro(lhtValores, "Historicos", vchCodEntidad, vchDesMaestro, false, (int)Session["iCodUsuarioDB"], pbReplicarClientes.CheckBox.Checked);
                    }
                }
            }
        }

        protected bool Replicar()
        {
            //Checa si está prendida la bandera de Replicar en todos los sitios de la misma tecnología
            if (pFields != null && pFields.ContainsConfigName("BanderasCodAuto"))
            {
                int liValBanderasCodAuto = 0;
                int liCodBanderas;
                DataTable ldtBanderasCodAuto;
                KeytiaFlagField lFlags = (KeytiaFlagField)pFields.GetByConfigName("BanderasCodAuto");
                liValBanderasCodAuto = (int)lFlags.DataValue;

                liCodBanderas = lFlags.ConfigValue;
                ldtBanderasCodAuto = pKDB.GetHisRegByEnt("Valores", "Valores",
                    new string[] { "{Atrib}", "{Value}" }, "[{Atrib}] = " + liCodBanderas);

                return KeytiaServiceBL.Alarmas.Alarma.
                    getValBandera(
                        ldtBanderasCodAuto,
                        liValBanderasCodAuto,
                        "ReplicarCodAuto",
                        false);
            }
            return false;
        }

    }

    public class CnfgLineasEdit : CnfgHisRecursosEdit
    {
        protected string psCodCarrier;
        protected string psRelacionRequerida;
        private string psEntidadRequerida;
        protected string psRelacionNoRequerida;
        protected string psEntidadNoRequerida;
        public string EntidadRequerida
        {
            get
            {
                return psEntidadRequerida;
            }
            protected set
            {
                psEntidadRequerida = value;
                psEntidadNoRequerida = (psEntidadRequerida == "CenCos" ? "Emple" : "CenCos");
            }
        }

        protected override void InitGrid()
        {
            base.InitGrid();
            if (piTipoRecurso > 0)
            {
                if (plstParams == null)
                {
                    plstParams = new List<Parametro>();
                }
                Parametro lParam = new Parametro();
                lParam.Name = "Recurs";
                lParam.Value = piTipoRecurso;
                plstParams.Add(lParam);

                pHisGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerVisHistoricoParam(this, sSource, aoData, fnCallback," + DSOControl.SerializeJSON<List<Parametro>>(plstParams) + ");}";
            }
        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            if (pFields != null)
            {
                AsignarCarrier();
            }
        }

        protected void AsignarCarrier()
        {
            if (!pFields.ContainsConfigName("Carrier")) return;

            DateTime ldtIniVigencia;
            DateTime ldtFinVigencia;

            getVigencias(dtIniVigencia.Date, dtFinVigencia.Date, out ldtIniVigencia, out ldtFinVigencia);

            DataTable lDataTable = DSODataAccess.Execute(string.Format(@"Select * from [{0}].[VisHistoricos('Carrier','{1}')]
                where vchCodigo = '{2}'
                and dtIniVigencia <> dtFinVigencia
                {3}",
                DSODataContext.Schema,
                Globals.GetCurrentLanguage(),
                psCodCarrier,
                DSOControl.ComplementaVigenciasJS(ldtIniVigencia, ldtFinVigencia, false, "")));
            if (lDataTable.Rows.Count > 0)
            {
                pFields.GetByConfigName("Carrier").DataValue = lDataTable.Rows[0]["iCodCatalogo"];
            }

            //20140425.PT: Se comenta esta linea para permitir el cambio del carrier
            //pFields.GetByConfigName("Carrier").DisableField();
        }

        protected override void AsignarValoresCampos()
        {
            if (pFields != null && pFields.ContainsConfigName("Tel"))
            {
                vchCodigo.DataValue = pFields.GetByConfigName("Tel").DSOControlDB.ToString();
                vchDescripcion.DataValue = pFields.GetByConfigName("Tel").DSOControlDB.ToString() + " (" + psCodCarrier + ")";
            }
        }

        protected override void DeshabilitarCampos()
        {
            if (pFields != null && pFields.ContainsConfigName("Tel"))
            {
                vchCodigo.TextBox.Enabled = false;
            }
            if (pFields != null && pFields.ContainsConfigName("CenCos"))
            {
                KeytiaBaseField lField = pFields.GetByConfigName("CenCos");
                string lsDesRelCenCosRecurs = BusquedaGenerica.getDesRelacion("CenCos", vchCodEntidad);
                if (!string.IsNullOrEmpty(lsDesRelCenCosRecurs) && pFields.ContainsConfigName(lsDesRelCenCosRecurs))
                {
                    lField.DisableField();
                }
            }

            vchDescripcion.TextBox.Enabled = false;

        }

        protected override bool ValidarRegistro()
        {
            psRelacionRequerida = BusquedaGenerica.getDesRelacion(psEntidadRequerida, vchCodEntidad);
            psRelacionNoRequerida = BusquedaGenerica.getDesRelacion(psEntidadNoRequerida, vchCodEntidad);
            return base.ValidarRegistro();
        }

        protected override bool ValidarDatos()
        {
            bool lbret = base.ValidarDatos();
            if (lbret)
            {
                psbErrores = new StringBuilder();

                ValidarFormatoNumero();
                ValidarNumeroRepetido();
                ValidarCargoFijo();

                if (psbErrores.Length > 0)
                {
                    lbret = false;
                    string lsError = "<ul>" + psbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, DSOControl.JScriptEncode(Title));
                }
                psbErrores = null;
            }
            return lbret;
        }

        protected virtual void ValidarNumeroRepetido()
        {
            //El campo Número no podrá ser igual a otro campo Número del mismo Tipo de recurso.
            DataTable lDataTable = DSODataAccess.Execute(String.Format(
                @"Select iCodCatalogo from Historicos h
                    where iCodMaestro = {0}
                    and iCodCatalogo <> IsNull({1}, -1)
                    and (Select vchCodigo from Catalogos where iCodRegistro = h.iCodCatalogo) = '{2}'
                    and dtIniVigencia <> dtFinVigencia
                    {3}",
                iCodMaestro,
                iCodCatalogo,
                vchCodigo.ToString(),
                DSOControl.ComplementaVigenciasJS(dtIniVigencia.Date, dtFinVigencia.Date, false, "")));

            if (lDataTable.Rows.Count > 0)
            {
                psbErrores.Append("<li>" + Globals.GetMsgWeb("ErrValorRepetido", vchCodigo.Descripcion, vchCodigo.ToString()) + "</li>");
            }
        }

        protected override void ValidarFormatoNumero()
        {
            if (pFields.ContainsConfigName("Tel"))
            {
                DSOControlDB ctlTel = pFields.GetByConfigName("Tel").DSOControlDB;
                ValidarLongitud(ctlTel, @"^\d{10}$", "10");
            }
        }

        protected override bool ValidarRelaciones()
        {
            bool lbret = base.ValidarRelaciones() &&
                ValidarRelTraslapeVig(psRelacionRequerida) &&
                ValidarRelTraslapeVig(psRelacionNoRequerida);

            if (lbret)
            {
                psbErrores = new StringBuilder();

                ValidarExistenRelaciones();
                AsignarEntidadActual(psRelacionRequerida, psEntidadRequerida);
                AsignarEntidadActual(psRelacionNoRequerida, psEntidadNoRequerida);

                if (psbErrores.Length > 0)
                {
                    lbret = false;
                    string lsError = "<ul>" + psbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, DSOControl.JScriptEncode(Title));
                }
                psbErrores = null;
            }
            return lbret;
        }

        protected bool RelacionVigenteEnRango(string lsEntidadRel, string lsDesRelacion, ref DateTime ldtIniVigencia, DateTime ldtFinVigencia)
        {
            DataTable ldtGuardados;
            DataTable ldtModificados;
            bool lbExito = false;
            List<string> lstModificados = new List<string>() { "0" };
            if (!lbExito && pFields.ContainsConfigName(lsDesRelacion))
            {
                DataRow[] rows = null;
                lbExito = true;

                ldtModificados = (DataTable)pFields.GetByConfigName(lsDesRelacion).DataValue;
                foreach (DataRow ldr in ldtModificados.Rows)
                {
                    lstModificados.Add(ldr["iCodRegistro"].ToString());
                }

                ldtGuardados = DSODataAccess.Execute(String.Format(@"Select iCodRegistro, dtIniVigencia, dtFinVigencia
                        from [{0}].[VisRelaciones('{1}','{2}')]
                        where {3} = IsNull({4}, -1)
                        and not iCodRegistro in ({5})
                        and dtIniVigencia <> dtFinVigencia",
                    DSODataContext.Schema,
                    lsDesRelacion,
                    Globals.GetCurrentLanguage(),
                    vchCodEntidad,
                    iCodCatalogo,
                    string.Join(",", lstModificados.ToArray())));

                if (ldtModificados.Rows.Count > 0)
                {
                    rows = ldtModificados.Select(string.Format(
                        @"IsNull(dtIniVigencia, '{1}') <= '{0}' and IsNull(dtFinVigencia, '2079-01-01') > '{0}'",
                        ldtIniVigencia.ToString("yyyy-MM-dd HH:mm:ss"),
                        DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss")), "dtIniVigencia desc");
                }
                if (rows == null || rows.Length == 0)
                {
                    rows = ldtGuardados.Select(string.Format(
                        @"dtIniVigencia <= '{0}' and dtFinVigencia > '{0}'",
                        ldtIniVigencia.ToString("yyyy-MM-dd HH:mm:ss")), "dtIniVigencia desc");
                }
                if (rows.Length == 0)
                {
                    lbExito = false;
                }
                else
                {
                    ldtIniVigencia = (DateTime)Util.IsDBNull(rows[0]["dtFinVigencia"], new DateTime(2079, 1, 1));
                }
                while (lbExito && ldtIniVigencia < ldtFinVigencia)
                {
                    rows = null;
                    if (ldtModificados.Rows.Count > 0)
                    {
                        rows = ldtModificados.Select(string.Format(
                        @"dtIniVigencia = '{0}' and dtFinVigencia > '{0}'",
                        ldtIniVigencia.ToString("yyyy-MM-dd HH:mm:ss")));
                    }
                    if (rows == null || rows.Length == 0)
                    {
                        rows = ldtGuardados.Select(string.Format(
                            @"dtIniVigencia = '{0}' and dtFinVigencia > '{0}'",
                            ldtIniVigencia.ToString("yyyy-MM-dd HH:mm:ss")));
                    }
                    if (rows.Length == 0)
                    {
                        lbExito = false;
                    }
                    else
                    {
                        ldtIniVigencia = (DateTime)Util.IsDBNull(rows[0]["dtFinVigencia"], new DateTime(2079, 1, 1));
                    }
                }
            }
            return lbExito;
        }

        protected virtual void ValidarExistenRelaciones()
        {
            if (!(psEntidadRequerida == "Emple" && AgregarDesdeEmpleado))
            {
                bool lbExisteRel = false;
                bool lbExito = true;
                DateTime ldtIniVigencia = pdtIniVigencia.Date;
                DateTime ldtFinVigencia = pdtFinVigencia.Date;
                DateTime ldtIniVigenciaRel = ldtIniVigencia;

                if (!string.IsNullOrEmpty(psRelacionRequerida))
                {
                    while (lbExito && !lbExisteRel)
                    {
                        DateTime ldtIniVigenciaAnt = ldtIniVigenciaRel;
                        lbExisteRel = RelacionVigenteEnRango(psEntidadRequerida, psRelacionRequerida, ref ldtIniVigenciaRel, ldtFinVigencia);
                        lbExito = ldtIniVigenciaRel != ldtIniVigenciaAnt;
                    }
                    if (!lbExisteRel)
                    {
                        // Debe existir una relación de tipo Línea-Empleado/CenCos
                        psbErrores.Append("<li>" + Globals.GetMsgWeb("ErrRelRequerida", Globals.GetLangItem("", "Entidades", psEntidadRequerida)) + "</li>");
                    }
                }
            }
        }

        protected virtual void ValidarCargoFijo()
        {
            //Cargo Fijo sólo se tomará en cuenta si la bandera de Carga Fijo esta activa.
            if (pFields.ContainsConfigName("BanderasLinea"))
            {
                int liCodBanderas = (int)Util.IsDBNull(pKDB.GetHisRegByEnt("Atrib", "Atributos",
                    "vchCodigo = 'BanderasLinea'").Rows[0]["iCodCatalogo"], 0);

                int liValBanderasLineas = (int)pFields.GetByConfigName("BanderasLinea").DataValue;
                DataTable ldtBanderasLineas = pKDB.GetHisRegByEnt("Valores", "Valores",
                    new string[] { "{Atrib}", "{Value}" }, "[{Atrib}] = " + liCodBanderas);

                if (ldtBanderasLineas.Select("vchCodigo = 'CargoFijo'").Length > 0)
                {
                    if (KeytiaServiceBL.Alarmas.Alarma.getValBandera(
                        ldtBanderasLineas,
                        liValBanderasLineas,
                        "CargoFijo",
                        false))
                    {
                        if (!pFields.GetByConfigName("CargoFijo").DSOControlDB.HasValue)
                        {
                            psbErrores.Append("<li>" + Globals.GetMsgWeb("CampoRequerido",
                                pFields.GetByConfigName("CargoFijo").DSOControlDB.Descripcion) + "</li>");
                        }
                    }
                    else
                    {
                        pFields.GetByConfigName("CargoFijo").DataValue = DBNull.Value;
                        phtValues[pFields.GetByConfigName("CargoFijo").Column] = "null";
                    }
                }
            }
        }
    }

    public class CnfgLinTelmEdit : CnfgLineasEdit
    {
        public CnfgLinTelmEdit()
        {
            psCodCarrier = "Telmex";
            EntidadRequerida = "CenCos";
        }

        protected override void ValidarCargoFijo()
        {
            // No se hace la validación para Telmex
        }
    }

    public class CnfgLinTelcEdit : CnfgLineasEdit
    {
        public CnfgLinTelcEdit()
        {
            psCodCarrier = "Telcel";
            EntidadRequerida = "Emple";
        }

        protected void ValidarCnfgCliente()
        {
            // Si el cliente requiere Cuenta Maestra, valida que el campo no esté vacío
            DataRow ldrCliente;
            int liValBanderasCliente = 0;
            int liCodBanderas;
            DataTable ldtBanderasCliente;

            if (!pFields.ContainsConfigName("CtaMaestra")) return;

            ldrCliente = getCliente();

            if (ldrCliente == null) return;

            if (ldrCliente.Table.Columns.Contains("{BanderasCliente}"))
            {
                liValBanderasCliente = (int)ldrCliente["{BanderasCliente}"];
            }

            liCodBanderas = (int)Util.IsDBNull(pKDB.GetHisRegByEnt("Atrib", "Atributos",
            "vchCodigo = 'BanderasCliente'").Rows[0]["iCodCatalogo"], 0);
            ldtBanderasCliente = pKDB.GetHisRegByEnt("Valores", "Valores",
            new string[] { "{Atrib}", "{Value}" }, "[{Atrib}] = " + liCodBanderas);

            if (KeytiaServiceBL.Alarmas.Alarma.getValBandera(
                                                        ldtBanderasCliente,
                                                        liValBanderasCliente,
                                                        "CtaMaeTelcel",
                                                        false))
            {
                if (!pFields.GetByConfigName("CtaMaestra").DSOControlDB.HasValue)
                {
                    psbErrores.Append("<li>" + Globals.GetMsgWeb("CampoRequerido",
                        pFields.GetByConfigName("CtaMaestra").DSOControlDB.Descripcion) + "</li>");
                }
            }
        }

        protected override bool ValidarDatos()
        {
            Boolean lbret = base.ValidarDatos();
            if (lbret)
            {
                psbErrores = new StringBuilder();

                ValidarCnfgCliente();

                if (psbErrores.Length > 0)
                {
                    lbret = false;
                    string lsError = "<ul>" + psbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, DSOControl.JScriptEncode(Title));
                }
                psbErrores = null;
            }
            return lbret;
        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            if (pFields != null && pFields.ContainsConfigName("CtaMaestra"))
            {
                pFields.GetByConfigName("CtaMaestra").DSOControlDB.Descripcion = Globals.GetMsgWeb("CtaPadre");
            }
        }
    }

    public class CnfgLinNextEdit : CnfgLineasEdit
    {
        public CnfgLinNextEdit()
        {
            psCodCarrier = "Nextel";
            EntidadRequerida = "Emple";
        }

        protected override void DeshabilitarCampos() { }

        protected override void AsignarValoresCampos() { }

        protected override void ValidarNumeroRepetido()
        {
            base.ValidarNumeroRepetido();
            // Como en Nextel no se asigna el campo Teléfono a la clave, se necesita validar este también

            if (!pFields.ContainsConfigName("Tel")) return;

            DSOControlDB ctlTel = pFields.GetByConfigName("Tel").DSOControlDB;

            if (!ctlTel.HasValue) return;

            DataTable lDataTable = DSODataAccess.Execute(String.Format(
                @"Select * from Historicos h
                    where iCodMaestro = {0}                    
                    and iCodCatalogo <> IsNull({1}, -1)
                    and {2} = '{3}'
                    and dtIniVigencia <> dtFinVigencia
                    {4}",
                iCodMaestro,
                iCodCatalogo,
                pFields.GetByConfigName("Tel").Column,
                ctlTel.ToString(),
                DSOControl.ComplementaVigenciasJS(dtIniVigencia.Date, dtFinVigencia.Date, false, "")));

            if (lDataTable.Rows.Count > 0)
            {
                psbErrores.Append("<li>" + Globals.GetMsgWeb("ErrValorRepetido",
                    ctlTel.Descripcion,
                    ctlTel.ToString()) + "</li>");
            }
        }

        protected override void ValidarFormatoNumero()
        {
            if (pFields.ContainsConfigName("Tel"))
            {
                DSOControlDB ctlTel = pFields.GetByConfigName("Tel").DSOControlDB;
                //20140331 AM. Se cambia el formato en el que se puede capturar el campo "Telefono" para que acepte capturar un radio de nextel.
                //ValidarLongitud(ctlTel, @"^$|^\d{10}$", "10");
                ValidarLongitud(ctlTel, @"^$|^(\d*\*\d*){2}$", "12");
            }

            string lsRegEx;
            lsRegEx = @"^(\d*\*\d*){2}$";
            if (!Regex.IsMatch(vchCodigo.ToString(), lsRegEx, RegexOptions.IgnoreCase))
            {
                psbErrores.Append("<li>" +
                    Globals.GetMsgWeb("ErrValorNoValido",
                    vchCodigo.Descripcion,
                    vchCodigo.ToString())
                    + "</li>");
            }
        }
    }

    public class CnfgLinMoviEdit : CnfgLineasEdit
    {
        public CnfgLinMoviEdit()
        {
            psCodCarrier = "Movistar";
            EntidadRequerida = "Emple";
        }
    }

    public class CnfgLinMovistarArgentinaEdit : CnfgLineasEdit
    {
        public CnfgLinMovistarArgentinaEdit()
        {
            psCodCarrier = "MovistarArg";
            EntidadRequerida = "Emple";
        }
    }    
    
    public class CnfgLinMovistarColEdit : CnfgLineasEdit
    {
        public CnfgLinMovistarColEdit()
        {
            psCodCarrier = "MovistarCol";
            EntidadRequerida = "Emple";
        }
    }

    public class CnfgLinAxteEdit : CnfgLineasEdit
    {
        public CnfgLinAxteEdit()
        {
            psCodCarrier = "Axtel";
            EntidadRequerida = "CenCos";
        }
    }

    public class CnfgLinIusaEdit : CnfgLineasEdit
    {
        public CnfgLinIusaEdit()
        {
            psCodCarrier = "Iusacell";
            EntidadRequerida = "Emple";
        }
    }

    public class CnfgLinAlesEdit : CnfgLineasEdit
    {
        public CnfgLinAlesEdit()
        {
            psCodCarrier = "Alestra";
            EntidadRequerida = "CenCos";
        }
    }

    public class CnfgLinATTEdit : CnfgLineasEdit
    {
        public CnfgLinATTEdit()
        {
            psCodCarrier = "ATT";
            EntidadRequerida = "Emple";
        }
    }

    public class CnfgLinVPNetEdit : CnfgLineasEdit
    {
        public CnfgLinVPNetEdit()
        {
            psCodCarrier = "VPNet";
            EntidadRequerida = "Emple";
        }
    }

    //RZ.20131127 Se agrega nueva clase para tipo de linea Claro
    /// <summary>
    /// Clase para definir el tipo de linea Claro.
    /// </summary>
    public class CnfgLinClaroEdit : CnfgLineasEdit
    {
        public CnfgLinClaroEdit()
        {
            psCodCarrier = "Claro";
            EntidadRequerida = "Emple";
        }

        protected override void ValidarCargoFijo()
        {
            // No se hace la validación para Claro
        }
    }

    
    public class CnfgLinClaroCelUrEdit : CnfgLineasEdit
    {
        public CnfgLinClaroCelUrEdit()
        {   
            psCodCarrier = "ClaroCelUruguay";
            EntidadRequerida = "Emple";
        }       
    }

    
    public class CnfgLinClaroElSalvCelEdit : CnfgLineasEdit
    {
        public CnfgLinClaroElSalvCelEdit()
        {
            psCodCarrier = "ClaroElSalvadorCel";
            EntidadRequerida = "Emple";
        }
    }

    
    public class CnfgLinLvl3ArgEdit : CnfgLineasEdit
    {
        public CnfgLinLvl3ArgEdit()
        {   
            psCodCarrier = "Level3Arg";
            EntidadRequerida = "Emple";
        }  
    }

    
    public class CnfgLinLvl3ColEdit : CnfgLineasEdit
    {
        public CnfgLinLvl3ColEdit()
        {
            psCodCarrier = "Level3Col";
            EntidadRequerida = "Emple";
        }
    }


    public class CnfgLinLvl3EUAEdit : CnfgLineasEdit
    {
        public CnfgLinLvl3EUAEdit()
        {
            psCodCarrier = "Level3EUA";
            EntidadRequerida = "Emple";
        }
    }


    public class CnfgLinLvl3GtlEdit : CnfgLineasEdit
    {
        public CnfgLinLvl3GtlEdit()
        {
            psCodCarrier = "Level3Gtl";
            EntidadRequerida = "Emple";
        }
    }

    public class CnfgLinLvl3MtyEdit : CnfgLineasEdit
    {
        public CnfgLinLvl3MtyEdit()
        {
            psCodCarrier = "Level3Mty";
            EntidadRequerida = "Emple";
        }
    }


    public class CnfgLinPersonalArgEdit : CnfgLineasEdit
    {
        public CnfgLinPersonalArgEdit()
        {
            psCodCarrier = "PersonalArg";
            EntidadRequerida = "Emple";
        }
    }

    public class CnfgLinTigoGtlCelEdit : CnfgLineasEdit
    {
        public CnfgLinTigoGtlCelEdit()
        {
            psCodCarrier = "TigoGtlCel";
            EntidadRequerida = "Emple";
        }
    }

    //RZ.20140117 Se agrega nueva clase para tipo de linea Personal
    /// <summary>
    /// Clase para definir el tipo de linea Personal.
    /// </summary>
    public class CnfgLinPersonalEdit : CnfgLineasEdit
    {
        public CnfgLinPersonalEdit()
        {
            psCodCarrier = "Personal";
            EntidadRequerida = "Emple";
        }

        protected override void ValidarCargoFijo()
        {
            // No se hace la validación para Personal
        }
    }

    //RZ.20140522 Se agrega nueva clase para tipo de linea Verizon
    /// <summary>
    /// Clase para definir el tipo de linea Verizon.
    /// </summary>
    public class CnfgLinVerizonEdit : CnfgLineasEdit
    {
        public CnfgLinVerizonEdit()
        {
            psCodCarrier = "Verizon";
            EntidadRequerida = "Emple";
        }

        protected override void ValidarCargoFijo()
        {
            // No se hace la validación para Claro
        }
    }

    //RZ.20140609 Tipos de lineas Telum
    /// <summary>
    /// Clase para definir el tipo de linea Verizon.
    /// </summary>
    public class CnfgLinTelumEdit : CnfgLineasEdit
    {
        public CnfgLinTelumEdit()
        {
            psCodCarrier = "Telum";
            EntidadRequerida = "Emple";
        }

        protected override void ValidarCargoFijo()
        {
            // No se hace la validación para Claro
        }

        protected override void ValidarFormatoNumero()
        {
            //No se valida logitud ni formato numerico del telefono
            //if (pFields.ContainsConfigName("Tel"))
            //{
            //    DSOControlDB ctlTel = pFields.GetByConfigName("Tel").DSOControlDB;
            //    ValidarLongitud(ctlTel, @"^\d{10}$", "10");
            //}
        }
    }

    //AM.20141120 Se agrega nueva clase para tipo de linea celtel
    /// <summary>
    /// Clase para definir el tipo de linea celtel.
    /// </summary>
    public class CnfgLinCeltelEdit : CnfgLineasEdit
    {
        public CnfgLinCeltelEdit()
        {
            psCodCarrier = "Celtel";
            EntidadRequerida = "Emple";
        }

        protected override void ValidarCargoFijo()
        {
            // No se hace la validación para Celtel
        }
    }

    public class CnfgLineasRelEdit : HistoricEdit
    {
        protected DSODropDownList pddlTipoRecurso;

        public virtual string iCodTipoRecurso
        {
            get
            {
                return (string)ViewState["iCodTipoRecurso"];
            }
            protected set
            {
                ViewState["iCodTipoRecurso"] = value;
            }
        }

        public CnfgLineasRelEdit()
        {
            Init += new EventHandler(CnfgLineasRelEdit_Init);
            Load += new EventHandler(CnfgLineasRelEdit_Load);
        }

        void CnfgLineasRelEdit_Init(object sender, EventArgs e)
        {
            pddlTipoRecurso = new DSODropDownList();
        }

        void CnfgLineasRelEdit_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || pddlTipoRecurso.DataSource == null)
            {
                FillControls();
            }
        }

        public override void FillControls()
        {
            base.FillControls();
            pddlTipoRecurso.DataSource = "select iCodRegistro, vchDescripcion from [VisHistoricos('Recurs','Recursos','Español')] where EntidadCod = 'Linea'";
            pddlTipoRecurso.Fill();
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);
            if (State == HistoricState.MaestroSeleccionado)
            {
                pbtnAgregar.Visible = false;
                pTablaRegistro.Rows[piCodEntidad.Row - 1].Visible = false;
                pTablaRegistro.Rows[piCodMaestro.Row - 1].Visible = false;
                pExpRegistro.Visible = true;
            }
        }

        protected override void InitRegistro()
        {
            base.InitRegistro();
            pddlTipoRecurso.ID = "iCodTipoRecurso";
            pddlTipoRecurso.Table = pTablaRegistro;
            pddlTipoRecurso.Row = pTablaRegistro.Rows.Count + 1;
            pddlTipoRecurso.ColumnSpan = 3;
            pddlTipoRecurso.SelectItemText = " ";
            pddlTipoRecurso.DataField = "iCodTipoRecurso";
            pddlTipoRecurso.CreateControls();
            pddlTipoRecurso.AutoPostBack = true;
            pddlTipoRecurso.DropDownListChange += new EventHandler(pddlTipoRecurso_SelectedIndexChanged);
        }

        void pddlTipoRecurso_SelectedIndexChanged(object sender, EventArgs e)
        {
            FillAjaxControls();
            iCodTipoRecurso = pddlTipoRecurso.HasValue ? pddlTipoRecurso.DataValue.ToString() : "null";

            psbQuery.Length = 0;
            psbQuery.AppendLine("select * from [VisHistoricos('Aplic','Español')]");
            psbQuery.AppendLine("where iCodCatalogo = (select Aplic from [VisHistoricos('Recurs','Español')]");
            psbQuery.AppendLine("where iCodCatalogo = {0})");
            psbQuery = psbQuery.Replace("{0}", iCodTipoRecurso);

            DataTable ldt = DSODataAccess.Execute(psbQuery.ToString());

            if (ldt != null && ldt.Rows.Count > 0)
            {
                psbQuery.Length = 0;
                psbQuery.AppendLine("select * from [VisHistoricos('Recurs','Recursos','Español')]");
                psbQuery.AppendLine("where iCodCatalogo = {0}");
                psbQuery = psbQuery.Replace("{0}", iCodTipoRecurso);

                DataRow ldrRecurs = DSODataAccess.Execute(psbQuery.ToString()).Rows[0];

                string lsMaestro = DSODataAccess.ExecuteScalar("select vchDescripcion from Maestros where iCodEntidad = " + ldrRecurs["Entidad"] + " and dtIniVigencia <> dtFinVigencia order by iCodRegistro").ToString();

                PrevState = State;

                SubHistoricClass = ldt.Rows[0]["ParamVarChar3"].ToString();

                if (ldt.Rows[0]["ParamVarChar4"].ToString() != "")
                {
                    SubCollectionClass = ldt.Rows[0]["ParamVarChar4"].ToString();
                }
                else
                {
                    SubCollectionClass = "KeytiaWeb.UserInterface.HistoricFieldCollection";
                }

                InitSubHistorico(this.ID + "Ent" + ldt.Rows[0]["ParamVarChar1"].ToString());

                pSubHistorico.SetEntidad(ldrRecurs["EntidadCod"].ToString());
                pSubHistorico.SetMaestro(lsMaestro);

                pSubHistorico.EsSubHistorico = true;
                pSubHistorico.FillControls();

                SetHistoricState(HistoricState.SubHistorico);
                pSubHistorico.InitMaestro();

                if (pSubHistorico is CnfgHisRecursosEdit)
                {
                    ((CnfgHisRecursosEdit)pSubHistorico).AgregarDesdeEmpleado = true;
                }
                pSubHistorico.Fields.EnableFields();
                pSubHistorico.SetHistoricState(HistoricState.Edicion);
            }
        }

        protected override void SubHistorico_PostGrabarClick(object sender, EventArgs e)
        {
            FillAjaxControls();
            if (pSubHistorico.State != HistoricState.Edicion)
            {
                if (pFields.Contains(pSubHistorico.vchCodEntidad))
                {
                    base.SubHistorico_PostGrabarClick(sender, e);
                }
                else
                {
                    iCodRegistro = pSubHistorico.iCodRegistro;
                    ConsultarRegistro();
                    RemoverSubHistorico();
                    FirePostGrabarClick();
                }
            }
        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            pExpRegistro.Title = Globals.GetMsgWeb("btnAgregar");
            pExpRegistro.ToolTip = Globals.GetMsgWeb("btnAgregar");
            pddlTipoRecurso.Descripcion = Globals.GetMsgWeb(false, "TipoRecurso");
        }
    }

    

}
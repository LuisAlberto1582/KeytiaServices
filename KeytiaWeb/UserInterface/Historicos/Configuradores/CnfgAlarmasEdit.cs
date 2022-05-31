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
using KeytiaServiceBL.Alarmas;

namespace KeytiaWeb.UserInterface
{
    public class AlarmaState : HistoricState
    {
        public static readonly HistoricState ConsultarSubHis = NewState(9, "ConsultarSubHis");
    }

    public class CnfgAlarmasEdit : HistoricEdit
    {
        protected string lsError;

        protected StringBuilder lsbErrores = new StringBuilder();

        protected string emailregex
        {
            get
            {
                return "^[a-zA-Z]{1}[a-zA-Z0-9_-]{2,}([.][a-zA-Z0-9_-]+)*[@][a-zA-Z0-9_-]{3,}([.][a-zA-Z0-9_-]+)*[.][a-zA-Z]{2,4}$";
            }
        }

        public CnfgAlarmasEdit()
        {
            Init += new EventHandler(CnfgAlarmasEdit_Init);
        }

        protected virtual void CnfgAlarmasEdit_Init(object sender, EventArgs e)
        {
            this.CssClass = "HistoricEdit CnfgAlarmasEdit";
        }

        protected override void InitFields()
        {
            base.InitFields();
            if (pFields != null)
            {
                AgregarBoton("RepEstUbica");
                AgregarBoton("RepEstCenCos");
                AgregarBoton("Asunto");
                if (vchCodEntidad == "Alarm")
                {
                    KeytiaAutoCompleteField lField = (KeytiaAutoCompleteField)pFields.GetByConfigName("EstCarga");
                    lField.DSOControlDB.TcCtl.Visible = false;
                    lField.DSOControlDB.TcLbl.Visible = false;
                }
            }
        }

        public override void LoadScripts()
        {
            base.LoadScripts();
            DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), "CnfgAlarmasEdit.js", "<script src='" + ResolveClientUrl("~/UserInterface/Historicos/Configuradores/CnfgAlarmasEdit.js") + "' type='text/javascript'></script>\r\n", true, false);
        }

        protected override void InitAccionesSecundarias()
        {
            base.InitAccionesSecundarias();
            if (pFields != null)
            {
                EmailField("CtaDe", "EmailEmisor");
                EmailField("CtaPara", "EmailOpcional"); //La cuenta destinatario se toma de los empleados, adicionalmente se puede agregar otros destinatarios
                EmailField("DestPrueba", "EmailOpcional");
                EmailField("CtaCC", "EmailOpcional");
                EmailField("CtaCCO", "EmailOpcional");
                EmailField("CtaNoValidos", "EmailOpcional");
                //RZ.20130502 Se retira llamada a este metodo, ya que este atributo fue reemplazado por DSFiltroAlarm
                //EmailField("CtaSoporte", "EmailOpcional");
            }
            if (State == HistoricState.CnfgSubHistoricField)
            {
                pSubHistorico.PostRegresarClick += new EventHandler(CnfgSubHistoricField_PostRegresarClick);
            }
        }

        protected override void AgregarBoton(string lConfigName)
        {
            AgregarBoton(lConfigName, "KeytiaWeb.UserInterface.CnfgAlarmasEdit", "KeytiaWeb.UserInterface.HistoricFieldCollection");
        }

        protected void EmailField(string lConfigName, string tipoEmail)
        {
            if (pFields.ContainsConfigName(lConfigName))
            {
                KeytiaBaseField lField = pFields.GetByConfigName(lConfigName);
                lField.DSOControlDB.AddClientEvent("Email", tipoEmail);
            }
        }

        protected override bool ValidarDatos()
        {
            bool lbret = true;
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloHistoricos"));

            ValidarEmail("CtaDe");
            ValidarEmail("CtaPara");
            ValidarEmail("DestPrueba");
            ValidarEmail("CtaCC");
            ValidarEmail("CtaCCO");
            ValidarPlantilla("Plantilla");
            ValidarPlantillaReporte("Plantilla");
            ValidarEmail("CtaNoValidos");
            //RZ.20130502 Se retira validación de email de este atributo, que ha sido reemplazado por DSFiltroAlarm
            //ValidarEmail("CtaSoporte");

            if (pFields != null)
            {
                if (pFields.ContainsConfigName("EstCarga") && !pFields.GetByConfigName("EstCarga").DSOControlDB.HasValue)
                {
                    DataTable ldt = pKDB.GetHisRegByEnt("EstCarga", "Estatus Cargas",
                        new string[] { "iCodCatalogo" }, "vchCodigo = 'CarEspera'");
                    if (ldt.Rows.Count > 0)
                        phtValues[pFields.GetByConfigName("EstCarga").Column] = (int)ldt.Rows[0]["iCodCatalogo"];
                }
            }



            if (lsbErrores.Length > 0)
            {
                lbret = false;
                lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
            }
            return lbret;
        }

        protected void ValidarEmail(string lConfigName)
        {
            if (pFields.ContainsConfigName(lConfigName))
            {
                KeytiaBaseField lField = pFields.GetByConfigName(lConfigName);
                string email = lField.DSOControlDB.ToString().Trim();
                string tipoEmail = lField.DSOControlDB.GetClientEvent("Email");
                Regex regexp = new Regex(emailregex);

                switch (tipoEmail)
                {
                    case "EmailEmisor":
                        if (String.IsNullOrEmpty(email))
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", lField.Descripcion));
                            lsbErrores.Append("<li>" + lsError + "</li>");
                        }
                        else if (email.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Length > 1)
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "SoloUnEmail", lField.Descripcion));
                            lsbErrores.Append("<li>" + lsError + "</li>");
                        }
                        else if (!regexp.IsMatch(email))
                        {
                            lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "EmailFormat", email, lField.Descripcion));
                            lsbErrores.Append("<li>" + lsError + "</li>");
                            break;
                        }
                        break;
                    case "EmailOpcional":
                        if (!String.IsNullOrEmpty(email))
                        {
                            foreach (string mail in email.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (!regexp.IsMatch(mail))
                                {
                                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "EmailFormat", mail, lField.Descripcion));
                                    lsbErrores.Append("<li>" + lsError + "</li>");
                                }
                            }
                        }
                        break;
                }
            }
        }

        protected void ValidarPlantilla(string lConfigName)
        {
            if (pFields.ContainsConfigName(lConfigName))
            {
                KeytiaBaseField lField = pFields.GetByConfigName(lConfigName);
                string lsPlantilla = lField.DSOControlDB.ToString();
                if (!String.IsNullOrEmpty(lsPlantilla))
                {
                    string lsFilePath = lsPlantilla;
                    if (System.IO.File.Exists(lsFilePath))
                        return;

                    lsFilePath = System.IO.Path.Combine(lsFilePath, Globals.GetCurrentLanguage());
                    if (System.IO.Directory.Exists(lsFilePath))
                    {
                        foreach (string lsFile in System.IO.Directory.GetFiles(lsFilePath))
                        {
                            if (lsFile.Length > 0 && (lsFile.EndsWith(".docx") || lsFile.EndsWith(".doc")))
                            {
                                return;
                            }
                        }
                    }
                    lsFilePath = lsPlantilla;
                    if (System.IO.Directory.Exists(lsFilePath))
                    {
                        foreach (string lsFile in System.IO.Directory.GetFiles(lsFilePath))
                        {
                            if (lsFile.Length > 0 && (lsFile.EndsWith(".docx") || lsFile.EndsWith(".doc")))
                            {
                                return;
                            }
                        }
                    }
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "PlantAlarmas"));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }
            }
        }

        protected void ValidarPlantillaReporte(string lConfigName)
        {
            if (pFields.ContainsConfigName(lConfigName))
            {
                KeytiaBaseField lField = pFields.GetByConfigName(lConfigName);
                string lsPlantilla = lField.DSOControlDB.ToString();
                int liValBanderas;
                DataTable ldtBanderas = getBanderas(out liValBanderas);
                bool lbHayError = false;
                bool lbSeleccionóReporte = false;
                if (getValBandera(ldtBanderas, liValBanderas, "ReporteEnMensaje"))
                {
                    if (pFields.GetByConfigName("RepEst").DSOControlDB.HasValue)
                    {
                        lbSeleccionóReporte = true;
                    }
                    else
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", pFields.GetByConfigName("RepEst").Descripcion));
                        lsbErrores.Append("<li>" + lsError + "</li>");
                        lbHayError = true;
                    }
                }

                if (getValBandera(ldtBanderas, liValBanderas, "EnviarConsPromUbica"))
                {
                    if (pFields.GetByConfigName("RepEstUbica").DSOControlDB.HasValue)
                    {
                        lbSeleccionóReporte = true;
                    }
                    else
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", pFields.GetByConfigName("RepEstUbica").Descripcion));
                        lsbErrores.Append("<li>" + lsError + "</li>");
                        lbHayError = true;
                    }
                }

                if (getValBandera(ldtBanderas, liValBanderas, "EnviarConsPromCC"))
                {
                    if (pFields.GetByConfigName("RepEstCenCos").DSOControlDB.HasValue)
                    {
                        lbSeleccionóReporte = true;
                    }
                    else
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", pFields.GetByConfigName("RepEstCenCos").Descripcion));
                        lsbErrores.Append("<li>" + lsError + "</li>");
                        lbHayError = true;
                    }
                }

                if (lbSeleccionóReporte && !lbHayError)
                {
                    if (String.IsNullOrEmpty(lsPlantilla) || !pFields.GetByConfigName("Idioma").DSOControlDB.HasValue)
                    {
                        lbHayError = true;
                    }
                    else
                    {
                        DataTable loTabla = new DataTable();
                        loTabla.Columns.Add("c1");
                        DataRow ldr = loTabla.NewRow();
                        ldr["c1"] = "c1";
                        loTabla.Rows.Add(ldr);
                        string lsFilePath = UtilAlarma.buscarPlantilla(lsPlantilla,
                            Alarma.getIdioma(int.Parse(pFields.GetByConfigName("Idioma").DSOControlDB.DataValue.ToString())));
                        if (!string.IsNullOrEmpty(lsFilePath))
                        {
                            WordAccess loWord = new WordAccess();
                            loWord.FilePath = lsFilePath;
                            try
                            {
                                loWord.Abrir();

                                EstiloTablaWord pEstiloTablaWord = new EstiloTablaWord();
                                pEstiloTablaWord.Estilo = "KeytiaGrid";
                                pEstiloTablaWord.FilaEncabezado = true;
                                pEstiloTablaWord.FilasBandas = true;
                                pEstiloTablaWord.PrimeraColumna = false;
                                pEstiloTablaWord.UltimaColumna = false;
                                pEstiloTablaWord.ColumnasBandas = false;
                                loWord.InsertarTabla(loTabla, pEstiloTablaWord.FilaEncabezado, pEstiloTablaWord.Estilo, pEstiloTablaWord);
                                pEstiloTablaWord.Estilo = "KeytiaHeaderRep";
                                loWord.InsertarTabla(loTabla, pEstiloTablaWord.FilaEncabezado, pEstiloTablaWord.Estilo, pEstiloTablaWord);
                                loWord.InsertarTexto("Titulo");
                                loWord.SetStyle("TituloReporte");
                            }
                            catch (Exception ex)
                            {
                                lbHayError = true;
                            }
                            finally
                            {
                                loWord.Cerrar();
                                loWord.Salir();
                                loWord = null;
                            }
                        }
                    }
                    if (lbHayError)
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "PlantRptAlarmas"));
                        lsbErrores.Append("<li>" + lsError + "</li>");
                    }
                }
            }
        }

        protected DataTable getBanderas(out int liValorBanderas)
        {
            DataTable ldtBanderas = null;
            KeytiaFlagField lfBanderas = null;
            liValorBanderas = 0;
            if (pFields != null)
            {
                foreach (KeytiaBaseField lField in pFields)
                {
                    if (lField is KeytiaFlagField)
                    {
                        lfBanderas = (KeytiaFlagField)lField;
                        break;
                    }
                }
                if (lfBanderas != null)
                {
                    liValorBanderas = lfBanderas.DataValue.ToString() == "null" ? 0 : int.Parse(lfBanderas.DataValue.ToString());
                    int liCodBandera = (int)Util.IsDBNull(pKDB.GetHisRegByEnt("Atrib", "Atributos", "vchCodigo = '" + lfBanderas.ConfigName + "'").Rows[0]["iCodCatalogo"], 0);
                    ldtBanderas = pKDB.GetHisRegByEnt("Valores", "Valores", new string[] { "{Atrib}", "{Value}" }, "[{Atrib}] = " + liCodBandera);
                }
            }
            return ldtBanderas;
        }

        protected bool getValBandera(DataTable ldtBanderas, int liValBanderas, string lsCodBandera)
        {
            bool lbRet = false;
            switch (vchDesMaestro)
            {
                case "Alarma Diaria":
                    lbRet = Alarma.getValBandera(ldtBanderas, liValBanderas, lsCodBandera + "D", false);
                    break;
                case "Alarma Semanal":
                case "Alarma Quincenal":
                    lbRet = Alarma.getValBandera(ldtBanderas, liValBanderas, lsCodBandera + "SQ", false);
                    break;
                default:
                    lbRet = Alarma.getValBandera(ldtBanderas, liValBanderas, lsCodBandera, false);
                    break;
            }
            return lbRet;
        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            if (pFields != null)
            {
                if (pFields.ContainsConfigName("EstCarga"))
                {
                    //pFields.GetByConfigName("EstCarga").DisableField();
                    if (!pFields.GetByConfigName("EstCarga").DSOControlDB.HasValue)
                    {
                        DataTable ldt = pKDB.GetHisRegByEnt("EstCarga", "Estatus Cargas",
                            new string[] { "iCodCatalogo" }, "vchCodigo = 'CarEspera'");
                        if (ldt.Rows.Count > 0)
                            pFields.GetByConfigName("EstCarga").DataValue = (int)ldt.Rows[0]["iCodCatalogo"];
                    }
                }
            }
        }

        public override void RaisePostBackEvent(string eventArgument)
        {
            base.RaisePostBackEvent(eventArgument);
            if (eventArgument.StartsWith("btnConsultarSubHis"))
            {
                int liCodRegistro;
                int liConfigValue;
                if (eventArgument.Split(':').Length == 3
                    && int.TryParse(eventArgument.Split(':')[1], out liCodRegistro)
                    && int.TryParse(eventArgument.Split(':')[2], out liConfigValue))
                {
                    KeytiaBaseField lField = pFields.GetByConfigValue(liConfigValue);
                    DataTable dtMaestro = DSODataAccess.Execute(@"select  c.vchcodigo,
	                                                                      m.vchdescripcion 
                                                                  from historicos h
                                                                  join maestros m
                                                                  on h.icodmaestro = m.icodregistro
                                                                  join catalogos c
                                                                  on c.icodregistro = m.icodentidad
                                                                  where c.dtIniVigencia <> c.dtFinVigencia
                                                                  and   m.dtIniVigencia <> m.dtFinVigencia
                                                                  and   h.icodregistro = " + liCodRegistro);

                    PrevState = State;
                    SubHistoricClass = lField.SubHistoricClass;
                    SubCollectionClass = lField.SubCollectionClass;

                    InitSubHistorico(this.ID + "ConsultarEnt" + iCodRegistro);

                    pSubHistorico.SetEntidad((dtMaestro.Rows[0]["vchcodigo"]).ToString());
                    pSubHistorico.SetMaestro((dtMaestro.Rows[0]["vchdescripcion"]).ToString());

                    DataTable lKDBTable = pKDB.GetHisRegByEnt("MsgWeb", "Mensajes Web", " vchDescripcion = '" + pSubHistorico.vchDesMaestro + "'");
                    if (lKDBTable.Rows.Count > 0)
                    {
                        pSubHistorico.vchCodTitle = lKDBTable.Rows[0]["vchCodigo"].ToString();
                    }

                    pSubHistorico.EsSubHistorico = true;
                    pSubHistorico.FillControls();

                    SetHistoricState(HistoricState.CnfgSubHistoricField);
                    pSubHistorico.InitMaestro();

                    pSubHistorico.iCodRegistro = liCodRegistro.ToString();
                    pSubHistorico.ConsultarRegistro();
                    pSubHistorico.SetHistoricState(AlarmaState.ConsultarSubHis);
                    pSubHistorico.Fields.DisableFields();
                }
            }
        }

        public override void SetHistoricState(HistoricState s)
        {
            StringBuilder lsb = new StringBuilder();
            base.SetHistoricState(s);
            if (s == AlarmaState.ConsultarSubHis)
            {
                pbtnRegresar.Visible = true;
                pExpAtributos.Visible = true;
                pExpRegistro.Visible = true;
                pPanelSubHistoricos.Visible = true;

                pTablaRegistro.Rows[pvchDescripcion.Row - 1].Visible = true;
                pvchCodigo.TextBox.Enabled = false;
                pvchDescripcion.TextBox.Enabled = false;

                pTablaRegistro.Rows[pdtIniVigencia.Row - 1].Visible = true;
                pdtIniVigencia.DateTimeBox.Enabled = false;
                pdtFinVigencia.DateTimeBox.Enabled = false;

                pTablaRegistro.Rows[pbReplicarClientes.Row - 1].Visible = false;

                lsb.Length = 0;
                lsb.AppendLine("<script type='text/javascript'>");
                lsb.AppendLine("jQuery(function($){");
                lsb.AppendLine(pjsObj + ".iCodRegistro = " + iCodRegistro + ";");
                lsb.AppendLine(pjsObj + ".iCodCatalogo = " + iCodCatalogo + ";");
                lsb.AppendLine(pjsObj + ".iCodMaestro = " + iCodMaestro + ";");
                lsb.AppendLine(pjsObj + ".iCodEntidad = " + iCodEntidad + ";");
                lsb.AppendLine("});");
                lsb.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), pjsObj + "RegistrosC", lsb.ToString(), true, false);
            }
        }

        protected virtual void CnfgSubHistoricField_PostRegresarClick(object sender, EventArgs e)
        {
            RemoverSubHistorico();
        }

        #region WebMethods
        public static DSOGridServerResponse GetSubHisDataAlarmas(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro, List<Parametro> parametros)
        {
            return CnfgAlarmasEdit.GetSubHisDataAlarmas(gsRequest, iCodEntidad, iCodMaestro, DateTime.Now, parametros);
        }

        public static DSOGridServerResponse GetSubHisDataAlarmas(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro, object ldtVigencia, List<Parametro> parametros)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                KDBAccess lKDB = new KDBAccess();
                HistoricFieldCollection lFields = new HistoricFieldCollection(iCodEntidad, iCodMaestro);
                StringBuilder lsbSelectTop = new StringBuilder();
                StringBuilder lsbColumnas = new StringBuilder();
                StringBuilder lsbFrom = new StringBuilder();
                StringBuilder lsbWhere = new StringBuilder();
                StringBuilder lsbOrderBy = new StringBuilder();
                DataTable ldtAtributos = HistoricFieldCollection.GetAtrib();

                int liCodIdioma = (int)ldtAtributos.Select("vchCodigo = '" + Globals.GetCurrentLanguage() + "'")[0]["iCodCatalogo"];

                DSOGridServerResponse lgsrRet = new DSOGridServerResponse();
                DataTable ldt;

                string lsOrderCol = "";
                string lsOrderDir = "";
                string lsOrderDirInv = "";
                if (gsRequest.iSortCol.Count > 0)
                {
                    lsOrderCol = gsRequest.sColumns.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[gsRequest.iSortCol[0]];

                    if (!lFields.Contains(lsOrderCol)
                        && lsOrderCol != "vchCodigo"
                        && lsOrderCol != "vchDescripcion"
                        && lsOrderCol != "dtIniVigencia"
                        && lsOrderCol != "dtFinVigencia"
                        && lsOrderCol != "Consultar")
                    {
                        lsOrderCol = "dtFecUltAct";
                    }

                    switch (gsRequest.sSortDir[0].ToLower())
                    {
                        case "desc":
                            lsOrderDir = " desc";
                            lsOrderDirInv = " asc";
                            break;
                        default:
                            lsOrderDir = " asc";
                            lsOrderDirInv = " desc";
                            break;
                    }
                }
                else
                {
                    lsOrderCol = "dtFecUltAct";
                    lsOrderDir = " asc";
                    lsOrderDirInv = " desc";
                }

                lsbSelectTop.AppendLine("select * from (");
                lsbSelectTop.AppendLine("   select top " + gsRequest.iDisplayLength + " * from (");
                lsbSelectTop.AppendLine("       select top " + (gsRequest.iDisplayStart + gsRequest.iDisplayLength));

                ////////////////////////////////////////////////////////////////////////////

                lsbColumnas.AppendLine("a.iCodRegistro");
                if (gsRequest.sColumns.Contains("Editar"))
                {
                    lsbColumnas.AppendLine(",Editar = null");
                }
                if (gsRequest.sColumns.Contains("Consultar"))
                {
                    lsbColumnas.AppendLine(",Consultar = null");
                }

                if (gsRequest.sColumns.Contains("Eliminar"))
                {
                    lsbColumnas.AppendLine(",Eliminar = null");
                } 

                lsbColumnas.AppendLine(",C.vchCodigo");
                lsbColumnas.AppendLine(",a.vchDescripcion");

                foreach (KeytiaBaseField lField in lFields)
                {
                    lsbColumnas.AppendLine(",a." + lField.Column);
                    if (lField.Column.StartsWith("iCodCatalogo"))
                    {
                        lsbColumnas.AppendLine("," + lField.Column + "Desc = " + DSODataContext.Schema + ".GetCatDesc(a." + lField.Column + ", " + liCodIdioma + " , a.dtIniVigencia)");
                    }
                }

                lsbColumnas.AppendLine(",a.dtIniVigencia");
                lsbColumnas.AppendLine(",a.dtFinVigencia");
                lsbColumnas.AppendLine(",a.dtFecUltAct");

                //////////////////////////////////////////////////////////////////////////7

                lsbFrom.AppendLine("      from Historicos a, Catalogos C");
                lsbFrom.AppendLine("      where a.iCodMaestro = " + iCodMaestro);
                lsbFrom.AppendLine("      and a.iCodCatalogo = C.iCodRegistro");
                lsbFrom.AppendLine("      and a.iCodCatalogo in(select E.iCodRegistro from Catalogos E where E.iCodCatalogo = " + iCodEntidad + ")");
                lsbFrom.AppendLine("      and a.dtIniVigencia <> a.dtFinVigencia");
                if (ldtVigencia != null)
                {
                    lsbFrom.AppendLine("      and '" + DSOControl.ParseDateTimeJS(ldtVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "' >= a.dtIniVigencia");
                    lsbFrom.AppendLine("      and '" + DSOControl.ParseDateTimeJS(ldtVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "' < a.dtFinVigencia");
                }

                //formatos de conversion de fechas de sql
                string lsSqlDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlDateFormat");
                string lsSqlTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlTimeFormat");

                if (parametros != null)
                {
                    foreach (Parametro lParam in parametros)
                    {
                        if (lParam.Value == null)
                        {
                            lParam.Value = "null";
                        }
                        string lsFiltro = lParam.Value.ToString().Replace("'", "''").Trim();
                        string lsColumna = null;
                        if (lFields.Contains(lParam.Name))
                        {
                            lsColumna = lParam.Name;
                        }
                        else if (lFields.ContainsConfigName(lParam.Name))
                        {
                            lsColumna = lFields.GetByConfigName(lParam.Name).Column;
                        }
                        else if (lParam.Name == "vchCodigo"
                            || lParam.Name == "vchDescripcion"
                            || lParam.Name == "dtIniVigencia"
                            || lParam.Name == "dtFinVigencia"
                            || lParam.Name == "iCodRegistro"
                            || lParam.Name == "NotiCodRegistro"
                            || lParam.Name == "iCodCatalogo"
                            || lParam.Name == "NotiCodCatalogo")
                        {
                            lsColumna = lParam.Name;
                        }

                        if (lsColumna != null)
                        {
                            lsbFrom.Append("and ");

                            if (lsColumna == "iCodRegistro")
                            {
                                lsbFrom.AppendLine("a.iCodRegistro in(" + lsFiltro + ")");
                            }
                            else if (lsColumna == "NotiCodRegistro")
                            {
                                lsbFrom.AppendLine("a.iCodRegistro not in(" + lsFiltro + ")");
                            }
                            else if (lsColumna == "iCodCatalogo")
                            {
                                lsbFrom.AppendLine("a.iCodCatalogo in(" + lsFiltro + ")");
                            }
                            else if (lsColumna == "NotiCodCatalogo")
                            {
                                lsbFrom.AppendLine("a.iCodCatalogo not in(" + lsFiltro + ")");
                            }
                            else if (lsColumna.StartsWith("Date")
                                || lsColumna == "dtIniVigencia"
                                || lsColumna == "dtFinVigencia")
                            {
                                if (lParam.Value is DateTime)
                                {
                                    lsbFrom.AppendLine("a." + lsColumna + " = '" + ((DateTime)lParam.Value).ToString("yyyy-MM-dd HH:mm:ss") + "'");
                                }
                                else
                                {
                                    lsbFrom.Append("(");
                                    lsbFrom.AppendLine("convert(varchar, a." + lsColumna + ", " + lsSqlDateFormat + ") like '%" + lsFiltro + "%'");
                                    lsbFrom.AppendLine("or convert(varchar, a." + lsColumna + ", " + lsSqlTimeFormat + ") like '%" + lsFiltro + "%'");
                                    lsbFrom.AppendLine(")");
                                }
                            }
                            else if (lsColumna.StartsWith("VarChar"))
                            {
                                lsbFrom.AppendLine("a." + lsColumna + " = '" + lsFiltro + "'");
                            }
                            else if (lsColumna == "vchCodigo")
                            {
                                lsbFrom.AppendLine("C." + lsColumna + " = '" + lsFiltro + "'");
                            }
                            else if (lParam.Name == "vchDescripcion")
                            {
                                lsbFrom.AppendLine("a." + lsColumna + " = '" + lsFiltro + "'");
                            }
                            else if (lsColumna.StartsWith("iCodCatalogo"))
                            {
                                lsbFrom.AppendLine("a." + lsColumna + " in(" + lsFiltro + ")");
                            }
                            else
                            {
                                lsbFrom.AppendLine("a." + lsColumna + " = " + lsFiltro);
                            }
                        }
                    }
                }

                if (gsRequest.sSearchGlobal.Trim() != "")
                {
                    string lsTerm = gsRequest.sSearchGlobal.Replace("'", "''").Trim();
                    StringBuilder lsbColTodas = new StringBuilder();
                    lsbColTodas.AppendLine("isnull(C.vchCodigo,'')");
                    lsbColTodas.AppendLine("+ ' ' + isnull(a.vchDescripcion,'')");

                    lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.dtIniVigencia," + lsSqlDateFormat + "),'')+ ' ' + isnull(convert(varchar,a.dtIniVigencia," + lsSqlTimeFormat + "),'')");
                    lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.dtFinVigencia," + lsSqlDateFormat + "),'')+ ' ' + isnull(convert(varchar,a.dtFinVigencia," + lsSqlTimeFormat + "),'')");

                    foreach (KeytiaBaseField lField in lFields)
                    {
                        if (lField.Column.StartsWith("Date"))
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a." + lField.Column + "," + lsSqlDateFormat + "),'') + ' ' + isnull(convert(varchar,a." + lField.Column + "," + lsSqlTimeFormat + "),'')");
                        }
                        else if (!lField.Column.StartsWith("VarChar")
                            && !lField.Column.StartsWith("iCodCatalogo"))
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar," + lField.Column + "),'')");
                        }
                        else if (lField.Column.StartsWith("iCodCatalogo"))
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar," + DSODataContext.Schema + ".GetCatDesc(a." + lField.Column + ", " + liCodIdioma + " , a.dtFecUltAct)" + "),'')");
                        }
                        else
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(a." + lField.Column + ",'')");
                        }
                    }
                    lsbWhere.AppendLine("and " + lsbColTodas.ToString() + " like '%" + lsTerm + "%'");
                }

                string[] lsColumns = gsRequest.sColumns.Split(',');
                int lidx;
                for (lidx = 0; lidx < lsColumns.Length; lidx++)
                {
                    if (gsRequest.bSearchable[lidx])
                    {
                        string lsColumn = lsColumns[lidx];
                        string lsFiltro = gsRequest.sSearch[lidx].Replace("'", "''").Trim();

                        if (!String.IsNullOrEmpty(lsFiltro)
                            && (lFields.Contains(lsColumn)
                            || lsColumn == "vchCodigo"
                            || lsColumn == "vchDescripcion"
                            || lsColumn == "dtIniVigencia"
                            || lsColumn == "dtFinVigencia"))
                        {
                            lsbWhere.Append("and ");

                            if (lsColumn.StartsWith("Date")
                                || lsColumn == "dtIniVigencia"
                                || lsColumn == "dtFinVigencia")
                            {

                                lsbWhere.Append("(");
                                lsbWhere.AppendLine("convert(varchar, a." + lsColumn + ", " + lsSqlDateFormat + ") like '%" + lsFiltro + "%'");
                                lsbWhere.AppendLine("or convert(varchar, a." + lsColumn + ", " + lsSqlTimeFormat + ") like '%" + lsFiltro + "%'");
                                lsbWhere.AppendLine(")");
                            }
                            else if (lsColumn.StartsWith("iCodCatalogo") && lsColumn.EndsWith("Desc"))
                            {
                                lsbWhere.AppendLine(DSODataContext.Schema + ".GetCatDesc(a." + lsColumn + ", " + liCodIdioma + " , a.dtFecUltAct) like '%" + lsFiltro + "%'");
                            }
                            else if (lsColumn == "vchCodigo")
                            {
                                lsbWhere.AppendLine("C." + lsColumn + " like '%" + lsFiltro + "%'");
                            }
                            else
                            {
                                lsbWhere.AppendLine("a." + lsColumn + " like '%" + lsFiltro + "%'");
                            }
                        }
                    }
                }

                if (lsOrderCol == "vchCodigo")
                {
                    lsbOrderBy.AppendLine("       order by C." + lsOrderCol + lsOrderDir + ", a.iCodRegistro" + lsOrderDir);
                }
                else
                {
                    lsbOrderBy.AppendLine("       order by a." + lsOrderCol + lsOrderDir + ", a.iCodRegistro" + lsOrderDir);
                }
                lsbOrderBy.AppendLine("   ) as a order by " + lsOrderCol + lsOrderDirInv + ", iCodRegistro" + lsOrderDirInv);
                lsbOrderBy.AppendLine(") as a order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);


                string lsSelectCount = "select count(a.iCodRegistro) ";
                string lsSelectTop = lsbSelectTop.ToString();
                string lsColumnas = lsbColumnas.ToString();
                string lsFrom = lsbFrom.ToString();
                string lsWhere = lsbWhere.ToString();
                string lsOrderBy = lsbOrderBy.ToString();

                lgsrRet.sEcho = gsRequest.sEcho;
                lgsrRet.iTotalRecords = (int)DSODataAccess.ExecuteScalar(lsSelectCount + lsFrom);
                if (!String.IsNullOrEmpty(lsWhere))
                {
                    lgsrRet.iTotalDisplayRecords = (int)DSODataAccess.ExecuteScalar(lsSelectCount + lsFrom + lsWhere);
                }
                else
                {
                    lgsrRet.iTotalDisplayRecords = lgsrRet.iTotalRecords;
                }

                ldt = DSODataAccess.Execute(lsSelectTop + lsColumnas + lsFrom + lsWhere + lsOrderBy);

                lgsrRet.ProcesarDatos(gsRequest, ldt);

                if (lgsrRet.iTotalDisplayRecords > 0 && ldt.Rows.Count == 0)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                lFields.FormatGridData(lgsrRet, ldt);

                return lgsrRet;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }
        #endregion

    }
}

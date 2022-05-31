using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using KeytiaServiceBL;
using System.Text;
using DSOControls2008;
using System.Data;
using System.Net.Mail;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace KeytiaWeb.UserInterface
{
    public class CartasCustodia : HistoricEdit
    {
        protected DSOExpandable pExpMot;
        protected Table pTablaMotivo;
        protected DSOTextBox pMotivoRec;
        protected string psCodEmpleado = "-1";
        protected string psNombreEmpleado = "";
        protected string psNominaEmpleado = "";
        protected string psCorreoEmpleado = "";
        protected int piFolio;
        protected string psMensajeCorreo;
        protected string psiCodCliente = "";
        DataTable pdtRegistro;

        //Atributos que se utilizaran para el envio de Emails
        protected MailAccess poMail;
        protected string psMailPara = "";
        protected string psMailCC = "";
        protected string psMailRemitente = "";
        protected string psNomRemitente = "";
        protected string psPathCartasProcesada = "";
        protected string psPathCartasAceptacion = "";
        protected string psPathCartasRechazo = "";
        protected string psLogoClientePath;
        protected string psLogoKeytiaPath;
        protected string psTempPath;

        public DSOTextBox MotivoRec
        {
            get
            {
                return pMotivoRec;
            }
        }

        public CartasCustodia()
        {
            Init += new EventHandler(CartasCustodia_Init);
        }

        public CartasCustodia(int iCodEmpleado)
        {
            psCodEmpleado = iCodEmpleado.ToString();
            Init += new EventHandler(CartasCustodia_Init);
        }

        protected override void InitAccionesSecundarias()
        {
            base.InitAccionesSecundarias();
            pExpMot.ID = "MotWrapper";
            pExpMot.StartOpen = true;
            pExpMot.CreateControls();
            pExpMot.Panel.Controls.Clear();
            pExpMot.Panel.Controls.Add(pTablaMotivo);

            pMotivoRec.ID = "MotivoRec";
            pMotivoRec.Table = pTablaMotivo;
            pMotivoRec.Row = 1;
            pMotivoRec.DataField = "MotivoRec";
            pMotivoRec.CreateControls();
            //pMotivoRec.TextBox.TextMode = TextBoxMode.MultiLine;
        }

        protected virtual void CartasCustodia_Init(object sender, EventArgs e)
        {
            pExpMot = new DSOExpandable();
            pTablaMotivo = new Table();
            pMotivoRec = new DSOTextBox();

            Controls.Add(pExpMot);

            this.CssClass = "HistoricEdit CartasCustodiaEdit";
            SetEmpleado();
            GetClientConfig();
        }

        public override void LoadScripts()
        {
            base.LoadScripts();
            DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), "CartasCustodia.js", "<script src='" + ResolveClientUrl("~/UserInterface/Historicos/Configuradores/CartasCustodia.js") + "' type='text/javascript'></script>\r\n", true, false);
        }

        protected virtual void SetEmpleado()
        {
            DataTable ldt;
            if (psCodEmpleado == "-1")
            {
                if (Session["iCodEmpleado"] == null)
                {
                    ldt = pKDB.GetHisRegByEnt("Emple", "Empleados", "{Usuar}='" + Session["iCodUsuario"] + "'");
                    if (ldt != null && ldt.Rows.Count > 0)
                    {
                        psCodEmpleado = ldt.Rows[0]["iCodCatalogo"].ToString();
                        psNombreEmpleado = ldt.Rows[0]["vchDescripcion"].ToString();
                        psNominaEmpleado = ldt.Rows[0]["vchCodigo"].ToString();
                        if (!(ldt.Rows[0]["{Email}"] is DBNull))
                        {
                            psCorreoEmpleado = ldt.Rows[0]["{Email}"].ToString();
                        }

                    }
                }
                else
                {
                    psCodEmpleado = Session["iCodEmpleado"].ToString();
                    ldt = pKDB.GetHisRegByEnt("Emple", "Empleados", "iCodCatalogo = " + psCodEmpleado);
                    if (ldt != null && ldt.Rows.Count > 0)
                    {
                        psNombreEmpleado = ldt.Rows[0]["vchDescripcion"].ToString();
                        psNominaEmpleado = ldt.Rows[0]["vchCodigo"].ToString();
                        if (!(ldt.Rows[0]["{Email}"] is DBNull))
                        {
                            psCorreoEmpleado = ldt.Rows[0]["{Email}"].ToString();
                        }
                    }
                }
            }
            else
            {
                ldt = pKDB.GetHisRegByEnt("Emple", "Empleados", "iCodCatalogo = " + psCodEmpleado);
                if (ldt != null && ldt.Rows.Count > 0)
                {
                    psNombreEmpleado = ldt.Rows[0]["vchDescripcion"].ToString();
                    psNominaEmpleado = ldt.Rows[0]["vchCodigo"].ToString();
                    if (!(ldt.Rows[0]["{Email}"] is DBNull))
                    {
                        psCorreoEmpleado = ldt.Rows[0]["{Email}"].ToString();
                    }
                    GetClientConfig();
                }
            }
        }

        protected override void InitGrid()
        {
            DSOGridClientColumn lCol;
            int lTarget = 0;

            pHisGrid.ClearConfig();
            pHisGrid.Config.sDom = "<\"H\"Rlf>tr<\"F\"pi>"; //con filtro global
            pHisGrid.Config.bAutoWidth = true;
            pHisGrid.Config.sScrollX = "100%";
            pHisGrid.Config.sPaginationType = "full_numbers";
            pHisGrid.Config.bJQueryUI = true;
            pHisGrid.Config.bProcessing = false;
            pHisGrid.Config.bServerSide = true;
            pHisGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerData(this, sSource, aoData, fnCallback);}";
            pHisGrid.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetHisRecAceptados");
            
            lCol = new DSOGridClientColumn();
            lCol.sName = "iCodRegistro";
            lCol.aTargets.Add(lTarget++);
            lCol.bVisible = false;
            pHisGrid.Config.aoColumnDefs.Add(lCol);
            
            InitGridFields();


            //lCol = new DSOGridClientColumn();
            //lCol.sName = "DescRecurso";
            //lCol.aTargets.Add(lTarget++);
            //lCol.sWidth = "100px";
            //pHisGrid.Config.aoColumnDefs.Add(lCol);

            //lCol = new DSOGridClientColumn();
            //lCol.sName = "Recurs";
            //lCol.aTargets.Add(lTarget++);
            //lCol.sWidth = "100px";
            //pHisGrid.Config.aoColumnDefs.Add(lCol);

            //lCol = new DSOGridClientColumn();
            //lCol.sName = "FechaAsignacion";
            //lCol.aTargets.Add(lTarget++);
            //lCol.sWidth = "100px";
            //pHisGrid.Config.aoColumnDefs.Add(lCol);

            //lCol = new DSOGridClientColumn();
            //lCol.sName = "FechaAceptacion";
            //lCol.aTargets.Add(lTarget++);
            //lCol.sWidth = "100px";
            //pHisGrid.Config.aoColumnDefs.Add(lCol);

            lTarget = pHisGrid.Config.aoColumnDefs.Count;

            if (pHisGrid.Config.aoColumnDefs.Count > 10)
            {
                pHisGrid.Config.sScrollXInner = (pHisGrid.Config.aoColumnDefs.Count * 20).ToString() + "%";
            }

            pHisGrid.Fill();
        }

        protected override void InitGridFields()
        {
            DSOGridClientColumn lCol;
            int lTarget = pHisGrid.Config.aoColumnDefs.Count;

            foreach (KeytiaBaseField lField in pFields)
            {
                if (lField.ShowInGrid && (lField.ConfigName != "Emple" && lField.ConfigName != "RegRelacion" && lField.ConfigName != "Recursos Pendientes" && lField.ConfigName != "Recursos Pendientes por Liberar"))
                {
                    lCol = new DSOGridClientColumn();
                    lCol.sName = lField.ConfigName;
                    lCol.aTargets.Add(lTarget++);
                    lCol.sWidth = "100px";
                    pHisGrid.Config.aoColumnDefs.Add(lCol);
                }
            }
        }

        public void RecursosPendientes()
        {
            SetEmpleado();
            if (psCodEmpleado != "-1")
            {
                ObtenerQueryRecursos();
                //DataTable dtListaRecursos = RemoverAceptados(DSODataAccess.Execute(psbQuery.ToString()));
                DataTable dtListaRecursos = RemoverRecAceptados(DSODataAccess.Execute(psbQuery.ToString()));

                psbQuery.Length = 0;
                psbQuery.AppendLine("select *");
                psbQuery.AppendLine("from [VisHistoricos('CartaCust','Recursos Aceptados','" + Globals.GetCurrentLanguage() + "')]");
                psbQuery.AppendLine("where Emple = " + psCodEmpleado);
                psbQuery.AppendLine("   and dtIniVigencia < '" + DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                psbQuery.AppendLine("   and dtFinVigencia > '" + DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                //DataTable dtListaRecursosALiberar = PendientesLiberar(DSODataAccess.Execute(psbQuery.ToString()));
                DataTable dtListaRecursosALiberar = RecPendientesLiberar(DSODataAccess.Execute(psbQuery.ToString()));

                if (dtListaRecursos.Rows.Count > 0 || dtListaRecursosALiberar.Rows.Count > 0)
                {
                    Session["HomePage"] = "~/UserInterface/Historicos/Historicos.aspx?Opc=OpcCarCust";
                }
            }
        }

        public string ObtenerQueryRecursos()
        {
            SetEmpleado();
            string lsVistaRecurso = "";
            string lsEntMae = "";

            psbQuery.Length = 0;
            //string[] lsRec = { "CodAcc", "CodAuto", "Exten", "Linea" };
            string[] lsRec = { "CodAuto", "Exten", "Linea" };
            foreach (string lsRecurso in lsRec)
            {
                //if (lsRecurso == "CodAcc")
                //{
                //    lsVistaRecurso = "CodAcceso";
                //    lsEntMae = "'CodAcc','Codigo de Acceso'";
                //}
                //else 
                if (lsRecurso == "CodAuto")
                {
                    lsVistaRecurso = "CodAutorizacion";
                    lsEntMae = "'CodAuto','Codigo Autorizacion'";
                }
                else if (lsRecurso == "Exten")
                {
                    lsVistaRecurso = "Extension";
                    lsEntMae = "'Exten','Extensiones'";
                }
                else if (lsRecurso == "Linea")
                {
                    lsVistaRecurso = "Linea";
                    lsEntMae = "'Linea','Lineas'";
                }
                if (psbQuery.Length != 0)
                {
                    psbQuery.AppendLine("union");
                }
                psbQuery.AppendLine("select Recurso = VR." + lsRecurso + "Desc, ");
                psbQuery.AppendLine("       TipoRecurso = VH.RecursDesc,");
                psbQuery.AppendLine("       FechaAsignacion = VR.[dtIniVigencia],");
                psbQuery.AppendLine("       VR.iCodRegistro, VR.[iCodRelacion],");
                psbQuery.AppendLine("       RecursoCod = VR." + lsRecurso + ",");
                psbQuery.AppendLine("       TipoRecursoCod = VH.Recurs,");
                psbQuery.AppendLine("       VR.[Emple],VR.[EmpleDesc]");
                psbQuery.AppendLine("from [VisRelaciones('Empleado - " + lsVistaRecurso + "','" + Globals.GetCurrentLanguage() + "')] VR");
                psbQuery.AppendLine("join [VisHistoricos(" + lsEntMae + ",'" + Globals.GetCurrentLanguage() + "')] VH");
                psbQuery.AppendLine("on VR." + lsRecurso + " = VH.iCodCatalogo");
                psbQuery.AppendLine("where VR.Emple = " + psCodEmpleado);
                psbQuery.AppendLine("and VH.EnviarCartaCust = 1");
                psbQuery.AppendLine("and VR.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                psbQuery.AppendLine("and VR.dtFinVigencia > '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                psbQuery.AppendLine("and VH.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                psbQuery.AppendLine("and VH.dtFinVigencia > '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                if (lsRecurso == "Exten")
                {
                    psbQuery.AppendLine("and (( 1 = ((IsNull(VR.FlagEmple,0) & 2) / 2) ) or ( 1 = ((IsNull(VR.FlagExten,0) & 1) / 1) ))");

                }
            }

            return psbQuery.ToString();
        }
        
        public DataTable RemoverRecAceptados(DataTable ldtRecursos)
        {
            if (ldtRecursos == null || ldtRecursos.Rows.Count == 0)
            {
                return ldtRecursos;
            }

           //Obtener los recursos que ya accepto
            psbQuery.Length = 0;
            psbQuery.AppendLine("select RegRelacion from [VisHistoricos('CartaCust','Recursos Aceptados','" + Globals.GetCurrentLanguage() + "')]");
            psbQuery.AppendLine("where Emple = " + psCodEmpleado);

            DataTable dtRecAceptados = DSODataAccess.Execute(psbQuery.ToString());
            if (dtRecAceptados == null || dtRecAceptados.Rows.Count == 0)
            {
                return ldtRecursos;
            }
            StringBuilder lsQuery = new StringBuilder();
            lsQuery.Length = 0;
            lsQuery.AppendLine("iCodRegistro not in ( ");
            foreach (DataRow drRecurso in dtRecAceptados.Rows)
            {
                lsQuery.AppendLine(drRecurso["RegRelacion"].ToString() + ",");
            }
            lsQuery.AppendLine("0) ");

            DataRow[] lrPend = ldtRecursos.Select(lsQuery.ToString());


            DataTable ldtRecPendientes = ldtRecursos.Clone();


            for (int idxRows = 0; idxRows < lrPend.Length; idxRows++)
            {
                ldtRecPendientes.ImportRow(lrPend[idxRows]);
            }

            return ldtRecPendientes;
        }

        public DataTable RecPendientesLiberar(DataTable ldtRecAceptados)
        {
            if (ldtRecAceptados == null || ldtRecAceptados.Rows.Count == 0)
            {
                return ldtRecAceptados;
            }

            //Obtener los recursos que ya accepto
            StringBuilder lsQuery = new StringBuilder();
            lsQuery.Length = 0;
            lsQuery.AppendLine("select * from Relaciones where iCodRegistro in ( ");
            foreach (DataRow drRecurso in ldtRecAceptados.Rows)
            {
                lsQuery.AppendLine(drRecurso["RegRelacion"].ToString() + ",");
            }
            lsQuery.AppendLine("0) ");

            DataTable ldtRecursos = DSODataAccess.Execute(lsQuery.ToString());
            if (ldtRecursos == null || ldtRecursos.Rows.Count == 0)
            {
                return ldtRecAceptados;
            }
            DataTable ldtRecLiberar = ldtRecAceptados.Clone();
            DataRow[] ldrRecAceptado;
            foreach (DataRow drRecurso in ldtRecursos.Rows)
            {
                ldrRecAceptado = ldtRecAceptados.Select("RegRelacion = " + drRecurso["iCodRegistro"]);
                if (ldrRecAceptado[0]["dtIniVigencia"].ToString() != drRecurso["dtIniVigencia"].ToString()
                    || ldrRecAceptado[0]["dtFinVigencia"].ToString() != drRecurso["dtFinVigencia"].ToString())
                {
                    ldtRecLiberar.ImportRow(ldrRecAceptado[0]);
                }
            }
            return ldtRecLiberar;
        }


        public DataTable RemoverAceptados(DataTable ldtRecPendientes)
        {
            //DataTable dtRecAceptados = DSODataAccess.Execute("select * from [VisHistoricos('CartaCust','Recursos Aceptados','" + Globals.GetCurrentLanguage() + "')]");
            psbQuery.Length = 0;
            psbQuery.AppendLine("select * from [VisHistoricos('CartaCust','Recursos Aceptados','" + Globals.GetCurrentLanguage() + "')]");
            psbQuery.AppendLine("where Emple = " + psCodEmpleado);

            DataTable dtRecAceptados = DSODataAccess.Execute(psbQuery.ToString());
            int lRow = 0;
            foreach (DataRow drRecursoPendiente in ldtRecPendientes.Rows)
            {
                foreach (DataRow drRecursoAceptado in dtRecAceptados.Rows)
                {
                    if (drRecursoPendiente["iCodRegistro"].ToString() == drRecursoAceptado["RegRelacion"].ToString())
                    {
                        ldtRecPendientes.Rows[lRow].Delete();
                        break;
                    }
                }
                lRow++;
            }
            ldtRecPendientes.AcceptChanges();

            return ldtRecPendientes;
        }

        
        public DataTable PendientesLiberar(DataTable ldtRecAceptados)
        {
            ldtRecAceptados.Select("Emple = " + psCodEmpleado);

            int lRow = 0;

            foreach (DataRow drRecursoALiberar in ldtRecAceptados.Rows)
            {
                DataRow ldtRecurso = DSODataAccess.Execute("select * from Relaciones where iCodRegistro = " + drRecursoALiberar["RegRelacion"]).Rows[0];
                if (drRecursoALiberar["dtIniVigencia"].ToString() == ldtRecurso["dtIniVigencia"].ToString()
                    && drRecursoALiberar["dtFinVigencia"].ToString() == ldtRecurso["dtFinVigencia"].ToString())
                {
                    ldtRecAceptados.Rows[lRow].Delete();
                }

                lRow++;
            }
            ldtRecAceptados.AcceptChanges();
            return ldtRecAceptados;
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);
            if (psCodEmpleado != "-1")
            {
                pbtnAgregar.Visible = true;
                pbtnBaja.Visible = true;
            }
            else
            {
                pbtnAgregar.Visible = false;
                pbtnBaja.Visible = false;
            }
            pExpFiltros.Visible = false;
            pPanelSubHistoricos.Visible = true;
        }

        protected override void pbtnAgregar_ServerClick(object sender, EventArgs e)
        {
            string g = State.ToString();
            CartaAceptacion();
            SetHistoricState(HistoricState.MaestroSeleccionado);
            pbtnAgregar.Visible = false;
            pbtnBaja.Visible = false;
        }

        protected override void pbtnBaja_ServerClick(object sender, EventArgs e)
        {
            string lsMotivo = MotivoRec.DataValue.ToString().Replace("'", "").Trim();
            string psError;
            StringBuilder psbErrores = new StringBuilder();
            string psTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloCartaCustodia"));
            if (lsMotivo != "null" && !String.IsNullOrEmpty(lsMotivo))
            {
                pbtnAgregar.Visible = false;
                pbtnBaja.Visible = false;
                CartaRechazo();
            }
            else
            {
                psError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ErrCartCustRechazo"));
                psbErrores.Append("<li>" + psError + "</li>");
                psError = "<ul>" + psbErrores.ToString() + "</ul>";
                DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", psError, psTitulo);
            }
        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            pExpMot.Title = "Motivo Rechazo";
            pExpMot.ToolTip = "Motivo Rechazo";
            pMotivoRec.Descripcion = "Motivo Rechazo";
            pbtnAgregar.InnerText = Globals.GetMsgWeb(false, "Aceptar");
            pbtnBaja.InnerText = Globals.GetMsgWeb(false, "Rechazar");
            //pbtnGrabar.InnerText = Globals.GetMsgWeb("btnAgregar");

            pHisGrid.Config.oLanguage = Globals.GetGridLanguage();
            foreach (DSOGridClientColumn lCol in pHisGrid.Config.aoColumnDefs)
            {
                if (lCol.sName == "DescRecurso")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "Recurso"));
                }
                else if (lCol.sName == "Recurs")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TipoRecurso"));
                }
                else if (lCol.sName == "FechaAsignacion")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "FecAsignacion"));
                }
                else if (lCol.sName == "FechaAceptacion")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "FecAceptacion"));
                }
            }
        }

        protected int ObtenerFolio()
        {
            psbQuery.Length = 0;
            psbQuery.AppendLine("select vchCodigo = isnull(max(vchCodigo),0)");
            psbQuery.AppendLine("from [VisHistoricos('CartaCust','Carta Custodia','Español')]");
            psbQuery.AppendLine("where Emple = " + psCodEmpleado);
            psbQuery.AppendLine("   and isnumeric(vchCodigo) > 0");
            psbQuery.AppendLine("   and dtIniVigencia < '" + DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("   and dtFinVigencia > '" + DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss") + "'");

            int liFolio = int.Parse(DSODataAccess.ExecuteScalar(psbQuery.ToString()).ToString());

            if (liFolio == 0)
            {
                psbQuery.Length = 0;
                psbQuery.AppendLine("select vchCodigo = isnull(max(vchCodigo),0)");
                psbQuery.AppendLine("from [VisHistoricos('CartaCust','Carta Custodia','Español')]");
                psbQuery.AppendLine("where isnumeric(vchCodigo) > 0");

                liFolio = int.Parse(DSODataAccess.ExecuteScalar(psbQuery.ToString()).ToString());

                liFolio++;
            }

            return liFolio;
        }

        protected void CartaAceptacion()
        {
            StringBuilder lsbRecurso = new StringBuilder();
            DataRow drVigencias;
            KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
            int liCodRegistro;

            try
            {
                psMensajeCorreo = "Aceptado";

                #region Insertar Registros a Recursos Aceptados

                piFolio = ObtenerFolio();

                lsbRecurso.AppendLine("select * from (");
                lsbRecurso.AppendLine(ObtenerQueryRecursos());
                lsbRecurso.AppendLine(") as Recurso");

                //pdtRegistro = RemoverAceptados(DSODataAccess.Execute(lsbRecurso.ToString()));
                pdtRegistro = RemoverRecAceptados(DSODataAccess.Execute(lsbRecurso.ToString()));

                foreach (DataRow drRegistro in pdtRegistro.Rows)
                {
                    phtValues = new Hashtable();
                    foreach (KeytiaBaseField lField in pFields)
                    {
                        if (lField.ConfigName == "DescRecurso")
                        {
                            phtValues.Add(lField.Column, "'" + drRegistro["Recurso"].ToString() + "'");
                        }
                        else if (lField.ConfigName == "Recurs")
                        {
                            phtValues.Add(lField.Column, drRegistro["TipoRecursoCod"].ToString());
                        }
                        else if (lField.ConfigName == "FechaAsignacion")
                        {
                            phtValues.Add(lField.Column, "'" + ((DateTime)drRegistro["FechaAsignacion"]).ToString("yyyy-MM-dd") + "'");
                        }
                        else if (lField.ConfigName == "FechaAceptacion")
                        {
                            phtValues.Add(lField.Column, "'" + DateTime.Today.ToString("yyyy-MM-dd") + "'");
                        }
                        else if (lField.ConfigName == "RegRelacion")
                        {
                            phtValues.Add(lField.Column, drRegistro["iCodRegistro"].ToString());
                        }
                        else if (lField.ConfigName == "Emple")
                        {
                            phtValues.Add(lField.Column, drRegistro["Emple"].ToString());
                        }
                        else if (lField.ConfigName == "FolioCartCust")
                        {
                            phtValues.Add(lField.Column, piFolio);
                        }
                    }
                    psbQuery.Length = 0;
                    psbQuery.AppendLine("select top 1 dtIniVigencia, dtFinVigencia");
                    psbQuery.AppendLine("from relaciones");
                    psbQuery.AppendLine("where icodregistro = " + drRegistro["iCodRegistro"].ToString());
                    psbQuery.AppendLine("order by dtfecultact desc");

                    drVigencias = (DSODataAccess.Execute(psbQuery.ToString())).Rows[0];

                    phtValues.Add("iCodMaestro", int.Parse(iCodMaestro));
                    phtValues.Add("vchCodigo", "'" + piFolio + "'");
                    phtValues.Add("vchDescripcion", "'" + drRegistro["EmpleDesc"].ToString() + " (" + drRegistro["Recurso"].ToString() + ")'");
                    phtValues.Add("dtIniVigencia", drVigencias["dtIniVigencia"]);
                    phtValues.Add("dtFinVigencia", drVigencias["dtFinVigencia"]);
                    phtValues.Add("iCodUsuario", Session["iCodUsuario"]);

                    liCodRegistro = lCargasCOM.InsertaRegistro(phtValues, "Historicos", vchCodEntidad, vchDesMaestro, false, (int)Session["iCodUsuarioDB"], pbReplicarClientes.CheckBox.Checked);
                    if (liCodRegistro < 0)
                    {
                        throw new KeytiaWebException("ErrSaveRecord");
                    }
                    phtValues.Clear();
                    phtValues = null;
                }
                #endregion

                #region Liberar Recursos Aceptados
                psbQuery.Length = 0;
                psbQuery.AppendLine("select Recurso = RecursCod,");
                psbQuery.AppendLine("       TipoRecurso = RecursDesc,");
                psbQuery.AppendLine("       FechaAsignacion = dtIniVigencia,");
                psbQuery.AppendLine("       RecursoCod = RecursCod,");
                psbQuery.AppendLine("       TipoRecursoCod = Recurs,");
                psbQuery.AppendLine("       iCodRegistro,");
                psbQuery.AppendLine("       Emple,");
                psbQuery.AppendLine("       EmpleDesc,");
                psbQuery.AppendLine("       RegRelacion,");
                psbQuery.AppendLine("       dtinivigencia,");
                psbQuery.AppendLine("       dtfinvigencia");
                psbQuery.AppendLine("from [VisHistoricos('CartaCust','Recursos Aceptados','" + Globals.GetCurrentLanguage() + "')]");
                psbQuery.AppendLine("where Emple = " + psCodEmpleado);
                psbQuery.AppendLine("   and dtIniVigencia < '" + DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                psbQuery.AppendLine("   and dtFinVigencia > '" + DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                //DataTable ldtRecursosLiberar = PendientesLiberar(DSODataAccess.Execute(psbQuery.ToString()));
                DataTable ldtRecursosLiberar = RecPendientesLiberar(DSODataAccess.Execute(psbQuery.ToString()));

                phtValues = new Hashtable();
                phtValues.Add("dtFinVigencia", DateTime.Today);

                foreach (DataRow drLiberar in ldtRecursosLiberar.Rows)
                {
                    if (!lCargasCOM.ActualizaRegistro("Historicos", "CartaCust", "Recursos Aceptados", phtValues, int.Parse(drLiberar["iCodRegistro"].ToString()), (int)Session["iCodUsuarioDB"]))
                    {
                        throw new KeytiaWebException("ErrSaveRecord");
                    }
                }
                phtValues.Clear();
                phtValues = null;
                #endregion

                if (pdtRegistro.Rows.Count > 0 || ldtRecursosLiberar.Rows.Count > 0)
                {
                    #region Insertar Carta Custodia
                    if (psCodEmpleado != "-1")
                    {
                        drVigencias = (pKDB.GetHisRegByEnt("Emple", "Empleados", "iCodCatalogo = " + psCodEmpleado)).Rows[0];

                        string liCodMaestroSeleccionado = DSODataAccess.ExecuteScalar("select iCodRegistro from Maestros where iCodEntidad = " + iCodEntidad + " and vchDescripcion = 'Carta Custodia' and dtIniVigencia <> dtFinVigencia").ToString();

                        Hashtable lhtEmpleado = new Hashtable();
                        lhtEmpleado.Add("{Emple}", psCodEmpleado);
                        Hashtable lhtValores = Util.TraducirHistoricos("CartaCust", "Carta Custodia", lhtEmpleado);
                        lhtValores.Add("iCodMaestro", int.Parse(liCodMaestroSeleccionado));
                        lhtValores.Add("vchCodigo", "'" + piFolio + "'");
                        lhtValores.Add("vchDescripcion", "'" + psNombreEmpleado + "'");
                        lhtValores.Add("{NominaA}", "'" + psNominaEmpleado + "'");
                        lhtValores.Add("{Email}", "'" + psCorreoEmpleado + "'");
                        lhtValores.Add("dtIniVigencia", drVigencias["dtIniVigencia"]);
                        lhtValores.Add("dtFinVigencia", drVigencias["dtFinVigencia"]);
                        lhtValores.Add("iCodUsuario", Session["iCodUsuario"]);

                        if (!TieneCartaCustodia())
                        {
                            liCodRegistro = lCargasCOM.InsertaRegistro(lhtValores, "Historicos", vchCodEntidad, "Carta Custodia", false, (int)Session["iCodUsuarioDB"], pbReplicarClientes.CheckBox.Checked);
                            if (liCodRegistro < 0)
                            {
                                throw new KeytiaWebException("ErrSaveRecord");
                            }
                        }
                        lhtEmpleado.Clear();
                        lhtValores.Clear();
                        lhtEmpleado = null;
                        lhtValores = null;
                    }
                    #endregion

                    EnviarCorreoConfirmacion();
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
        }

        protected bool TieneCartaCustodia()
        {
            bool lbret = false;
            int liCodRegistro;
            psbQuery.Length = 0;
            psbQuery.AppendLine("select iCodRegistro = isnull(count(icodregistro),0)");
            psbQuery.AppendLine("from [VisHistoricos('CartaCust','Carta Custodia','Español')]");
            psbQuery.AppendLine("where Emple = " + psCodEmpleado);
            psbQuery.AppendLine("   and dtIniVigencia < '" + DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("   and dtFinVigencia > '" + DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss") + "'");

            liCodRegistro = int.Parse(DSODataAccess.ExecuteScalar(psbQuery.ToString()).ToString());

            if (liCodRegistro > 0)
            {
                lbret = true;
            }

            return lbret;
        }

        protected void CartaRechazo()
        {
            KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
            StringBuilder lsbRecurso = new StringBuilder();
            DataRow drVigencias;
            int liCodRegistro;

            try
            {
                psMensajeCorreo = "Rechazado";

                piFolio = ObtenerFolio();

                #region Validar que existan recursos pendientes por aceptar
                lsbRecurso.AppendLine("select * from (");
                lsbRecurso.AppendLine(ObtenerQueryRecursos());
                lsbRecurso.AppendLine(") as Recurso");

                //pdtRegistro = RemoverAceptados(DSODataAccess.Execute(lsbRecurso.ToString()));
                pdtRegistro = RemoverRecAceptados(DSODataAccess.Execute(lsbRecurso.ToString()));
                #endregion

                #region Validar que existan recursos pendientes por liberar.
                psbQuery.Length = 0;
                psbQuery.AppendLine("select Recurso = RecursCod,");
                psbQuery.AppendLine("       TipoRecurso = RecursDesc,");
                psbQuery.AppendLine("       FechaAsignacion = dtIniVigencia,");
                psbQuery.AppendLine("       RecursoCod = RecursCod,");
                psbQuery.AppendLine("       TipoRecursoCod = Recurs,");
                psbQuery.AppendLine("       iCodRegistro,");
                psbQuery.AppendLine("       Emple,");
                psbQuery.AppendLine("       RegRelacion,");
                psbQuery.AppendLine("       dtinivigencia,");
                psbQuery.AppendLine("       dtfinvigencia");
                psbQuery.AppendLine("from [VisHistoricos('CartaCust','Recursos Aceptados','" + Globals.GetCurrentLanguage() + "')]");
                psbQuery.AppendLine("where Emple = " + psCodEmpleado);
                psbQuery.AppendLine("   and dtIniVigencia < '" + DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                psbQuery.AppendLine("   and dtFinVigencia > '" + DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss") + "'");

                //DataTable ldtRecursosLiberar = PendientesLiberar(DSODataAccess.Execute(psbQuery.ToString()));
                DataTable ldtRecursosLiberar = RecPendientesLiberar(DSODataAccess.Execute(psbQuery.ToString()));
                #endregion

                if (pdtRegistro.Rows.Count > 0 || ldtRecursosLiberar.Rows.Count > 0)
                {
                    #region Insertar Carta Custodia
                    if (psCodEmpleado != "-1")
                    {
                        drVigencias = (pKDB.GetHisRegByEnt("Emple", "Empleados", "iCodCatalogo = " + psCodEmpleado)).Rows[0];

                        string liCodMaestroSeleccionado = DSODataAccess.ExecuteScalar("select iCodRegistro from Maestros where iCodEntidad = " + iCodEntidad + " and vchDescripcion = 'Carta Custodia' and dtIniVigencia <> dtFinVigencia").ToString();

                        Hashtable lhtEmpleado = new Hashtable();
                        lhtEmpleado.Add("{Emple}", psCodEmpleado);
                        Hashtable lhtValores = Util.TraducirHistoricos("CartaCust", "Carta Custodia", lhtEmpleado);
                        lhtValores.Add("iCodMaestro", int.Parse(liCodMaestroSeleccionado));
                        lhtValores.Add("vchCodigo", "'" + piFolio + "'");
                        lhtValores.Add("vchDescripcion", "'" + psNombreEmpleado + "'");
                        lhtValores.Add("dtIniVigencia", drVigencias["dtIniVigencia"]);
                        lhtValores.Add("dtFinVigencia", drVigencias["dtFinVigencia"]);
                        lhtValores.Add("iCodUsuario", Session["iCodUsuario"]);

                        if (!TieneCartaCustodia())
                        {
                            liCodRegistro = lCargasCOM.InsertaRegistro(lhtValores, "Historicos", vchCodEntidad, "Carta Custodia", false, (int)Session["iCodUsuarioDB"], pbReplicarClientes.CheckBox.Checked);
                            if (liCodRegistro < 0)
                            {
                                throw new KeytiaWebException("ErrSaveRecord");
                            }
                        }
                        lhtEmpleado.Clear();
                        lhtValores.Clear();
                        lhtEmpleado = null;
                        lhtValores = null;
                    }
                    #endregion
                    EnviarCorreoConfirmacion();
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
        }

        public void CartaProcesada()
        {
            psMensajeCorreo = "Procesada";
            ObtenerQueryRecursos();            
            //DataTable ldtRecursos = RemoverAceptados(DSODataAccess.Execute(psbQuery.ToString()));
            DataTable ldtRecursos = RemoverRecAceptados(DSODataAccess.Execute(psbQuery.ToString()));

            if (ldtRecursos != null && ldtRecursos.Rows.Count > 0)
            {
                EnviarCorreoProcesada();
            }
        }

        public void CartaProcesada(string lsEntidad,string liCodRecurso)
        {
            string lsVistaRecurso;
            string lsEntMae;
            StringBuilder lsQuery = new StringBuilder();
             SetEmpleado();

            if (lsEntidad == "")
            {
                return;
            }
            if (lsEntidad == "Emple")
            {
                CartaProcesada();
                return;
            }
            if (lsEntidad == "CodAuto")
            {
                lsVistaRecurso = "CodAutorizacion";
                lsEntMae = "'CodAuto','Codigo Autorizacion'";
            }
            else if (lsEntidad == "Exten")
            {
                lsVistaRecurso = "Extension";
                lsEntMae = "'Exten','Extensiones'";
            }
            else if (lsEntidad == "Linea")
            {
                lsVistaRecurso = "Linea";
                lsEntMae = "'Linea','Lineas'";
            }
            else
            {
                return;
            }

             lsQuery = FormarQueryRecursos(lsEntidad, lsVistaRecurso, lsEntMae, liCodRecurso);

            psMensajeCorreo = "Procesada";
            //ObtenerQueryRecursos();
            DataTable ldtRecursos = RemoverRecAceptados(DSODataAccess.Execute(lsQuery.ToString()));


            if (ldtRecursos != null && ldtRecursos.Rows.Count > 0)
            {
                EnviarCorreoProcesada();
            }
        }

        protected StringBuilder FormarQueryRecursos(string lsRecurso, string lsVistaRecurso, string lsEntMae, 
             string liCodRecurso)
        {
            StringBuilder lsbQuery = new StringBuilder();

            lsbQuery.Length = 0;

            lsbQuery.AppendLine("select Recurso = VR." + lsRecurso + "Desc, ");
            lsbQuery.AppendLine("       TipoRecurso = VH.RecursDesc,");
            lsbQuery.AppendLine("       FechaAsignacion = VR.[dtIniVigencia],");
            lsbQuery.AppendLine("       VR.iCodRegistro, VR.[iCodRelacion],");
            lsbQuery.AppendLine("       RecursoCod = VR." + lsRecurso + ",");
            lsbQuery.AppendLine("       TipoRecursoCod = VH.Recurs,");
            lsbQuery.AppendLine("       VR.[Emple],VR.[EmpleDesc]");
            lsbQuery.AppendLine("from [VisRelaciones('Empleado - " + lsVistaRecurso + "','" + Globals.GetCurrentLanguage() + "')] VR");
            lsbQuery.AppendLine("join [VisHistoricos(" + lsEntMae + ",'" + Globals.GetCurrentLanguage() + "')] VH");
            lsbQuery.AppendLine("on VR." + lsRecurso + " = VH.iCodCatalogo");
            lsbQuery.AppendLine("where VR.Emple = " + psCodEmpleado);
            lsbQuery.AppendLine("And VR." + lsRecurso + " = isNull(" + liCodRecurso + ",VR." + lsRecurso + "," + liCodRecurso + ")");
            lsbQuery.AppendLine("and VH.EnviarCartaCust = 1");
            lsbQuery.AppendLine("and VR.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsbQuery.AppendLine("and VR.dtFinVigencia > '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsbQuery.AppendLine("and VH.dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsbQuery.AppendLine("and VH.dtFinVigencia > '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            if (lsRecurso == "Exten")
            {
                lsbQuery.AppendLine("and (( 1 = ((IsNull(VR.FlagEmple,0) & 2) / 2) ) or ( 1 = ((IsNull(VR.FlagExten,0) & 1) / 1) ))");

            }
            return lsbQuery;
        }

        #region Envio de Correos

        protected void GetClientConfig()
        {
            DataRow ldrCliente;
            KDBAccess kdb = new KDBAccess();
            DataTable ldtEmpleColaborador = new DataTable();

            if (psCodEmpleado != "-1")
            {
                ldrCliente = KeytiaServiceBL.Alarmas.Alarma.getCliente(int.Parse(psCodEmpleado));
                if (ldrCliente == null)
                {
                    return;
                }
                psiCodCliente = Util.IsDBNull(ldrCliente["iCodCatalogo"], "").ToString();

                psMailRemitente = Util.IsDBNull(ldrCliente["{CtaDe}"], "").ToString();
                psNomRemitente = Util.IsDBNull(ldrCliente["{NomRemitente}"], "").ToString();
                psLogoClientePath = Util.IsDBNull(ldrCliente["{Logo}"], "").ToString().Replace("/", "\\");
                psLogoClientePath = psLogoClientePath.Replace("~", HttpContext.Current.Server.MapPath("~"));
                psLogoKeytiaPath = Session["StyleSheet"].ToString().Replace("/", "\\");
                psLogoKeytiaPath = psLogoKeytiaPath.Replace("~", HttpContext.Current.Server.MapPath("~"));
                psLogoKeytiaPath = System.IO.Path.Combine(psLogoKeytiaPath, @"images\KeytiaHeader.png");
                if (ldrCliente.Table.Columns.Contains("{CartaCust}") 
                    && ldrCliente["{CartaCust}"].ToString() != "")
//                if (ldrCliente["{CartaCust}"].ToString() != "")
                {
                    DataRow ldtPathCartasCustodia = kdb.GetHisRegByEnt("CartaCust", "Configuracion Cartas Custodia", "iCodCatalogo = " + ldrCliente["{CartaCust}"].ToString()).Rows[0];

                    psMailCC = Util.IsDBNull(ldtPathCartasCustodia["{CtaCCCartaCust}"], "").ToString();
                    psPathCartasProcesada = ldtPathCartasCustodia["{PathPlantillaCartCustProcesadas}"].ToString();
                    psPathCartasAceptacion = ldtPathCartasCustodia["{PathPlantillaCartCustAceptadas}"].ToString();
                    psPathCartasRechazo = ldtPathCartasCustodia["{PathPlantillaCartCustRechazadas}"].ToString();
                }
            }
        }

        private string buscarPlantilla()
        {
            string lsFilePath = "";
            string lsFile = "";
            lsFilePath = Session["StyleSheet"].ToString().Replace("/", "\\");
            lsFilePath = lsFilePath.Replace("~", HttpContext.Current.Server.MapPath("~"));
            if (psMensajeCorreo == "Aceptado")
            {
                lsFile = "PlantillaCartCustAceptacion.docx";
            }
            else if (psMensajeCorreo == "Rechazado")
            {
                lsFile = "PlantillaCartCustRechazo.docx";
            }
            else
            {
                lsFile = "PlantillaCartCustProcesada.docx";
            }
            lsFilePath = System.IO.Path.Combine(lsFilePath, @"plantillas\CartasCustodia\" + lsFile);
            if (!System.IO.File.Exists(lsFilePath))
            {
                return "";
            }
            return lsFilePath;
        }

        public void EnviarCorreoConfirmacion()
        {
            int liCodEmpleado = int.Parse(psCodEmpleado);
            string lsWordPath;
            if (psMensajeCorreo == "Aceptado")
            {
                lsWordPath = psPathCartasAceptacion;
            }
            else
            {
                lsWordPath = psPathCartasRechazo;
            }

            WordAccess loWord = new WordAccess();
            if (lsWordPath != "" && System.IO.File.Exists(lsWordPath))
            {
                #region Con Plantilla del Cliente
                //Con Plantilla del Cliente
                loWord.FilePath = lsWordPath;

                loWord.Abrir(true);
                if (System.IO.File.Exists(psLogoClientePath))
                {
                    loWord.ReemplazarTextoPorImagen("{LogoCliente}", psLogoClientePath);
                }
                else
                {
                    loWord.ReemplazarTexto("{LogoCliente}", "");
                }
                if (System.IO.File.Exists(psLogoKeytiaPath))
                {
                    loWord.ReemplazarTextoPorImagen("{LogoKeytia}", psLogoKeytiaPath);
                }
                else
                {
                    loWord.ReemplazarTexto("{LogoKeytia}", "");
                }
                loWord.ReemplazarTexto("{FECHA}", DateTime.Today.ToString("dd-MMMM-yyyy").ToUpper());
                loWord.ReemplazarTexto("{NOMBRE EMPLEADO}", psNombreEmpleado);
                loWord.ReemplazarTexto("{NOMINA EMPLEADO}", psNominaEmpleado);
                loWord.ReemplazarTexto("{FOLIO CARTA CUSTODIA}", piFolio.ToString());
                loWord.ReemplazarTexto("{MOTIVO}", MotivoRec.DataValue.ToString().Replace("'",""));
                #endregion
            }
            else
            {
                #region Plantilla Default
                lsWordPath = buscarPlantilla();

                if (lsWordPath != "" && System.IO.File.Exists(lsWordPath))
                {
                    //Con Plantilla Default
                    loWord.FilePath = lsWordPath;

                    loWord.Abrir(true);
                    if (System.IO.File.Exists(psLogoClientePath))
                    {
                        loWord.ReemplazarTextoPorImagen("{LogoCliente}", psLogoClientePath);
                    }
                    else
                    {
                        loWord.ReemplazarTexto("{LogoCliente}", "");
                    }
                    if (System.IO.File.Exists(psLogoKeytiaPath))
                    {
                        loWord.ReemplazarTextoPorImagen("{LogoKeytia}", psLogoKeytiaPath);
                    }
                    else
                    {
                        loWord.ReemplazarTexto("{LogoKeytia}", "");
                    }
                    loWord.ReemplazarTexto("{FECHA}", DateTime.Today.ToString("dd-MMMM-yyyy").ToUpper());
                    loWord.ReemplazarTexto("{NOMBRE EMPLEADO}", psNombreEmpleado);
                    loWord.ReemplazarTexto("{NOMINA EMPLEADO}", psNominaEmpleado);
                    loWord.ReemplazarTexto("{FOLIO CARTA CUSTODIA}", piFolio.ToString());
                    loWord.ReemplazarTexto("{MOTIVO}", MotivoRec.DataValue.ToString().Replace("'", ""));
                }
                #endregion
                else
                {
                    #region Sin Plantilla
                    //Sin Plantilla
                    string SaltoLinea = "\n";

                    loWord.Abrir(true);
                    if (System.IO.File.Exists(psLogoClientePath))
                    {
                        loWord.InsertarImagen(psLogoClientePath);
                    }

                    if (System.IO.File.Exists(psLogoKeytiaPath))
                    {
                        loWord.InsertarImagen(psLogoKeytiaPath);
                    }
                    loWord.NuevoParrafo();
                    loWord.InsertarTexto(SaltoLinea + DateTime.Today.ToString("dd-MMMM-yyyy").ToUpper());
                    loWord.NuevoParrafo();
                    loWord.NuevoParrafo();
                    string lsMensaje;
                    if (psMensajeCorreo == "Aceptado")
                    {
                        lsMensaje = Globals.GetMsgWeb(false, "CuerpoCorreoAceptado", psNombreEmpleado, piFolio.ToString());
                        loWord.InsertarTexto(lsMensaje);
                        loWord.NuevoParrafo();
                        if (lsMensaje.StartsWith("#undefined-"))
                        {
                            loWord.InsertarTexto(psNombreEmpleado + " acepto el folio " + piFolio);
                        }
                    }
                    else
                    {
                        lsMensaje = Globals.GetMsgWeb(false, "CuerpoCorreoRechazo", psNombreEmpleado, piFolio.ToString(), pMotivoRec.DataValue.ToString().Replace("'", ""));
                        loWord.InsertarTexto(lsMensaje);
                        loWord.NuevoParrafo();
                        if (lsMensaje.StartsWith("#undefined-"))
                        {
                            loWord.InsertarTexto(psNombreEmpleado + " rechazo el folio " + piFolio + SaltoLinea);
                            loWord.InsertarTexto("su motivo fue: " + pMotivoRec.DataValue.ToString().Replace("'", ""));
                        }
                    }
                    loWord.NuevoParrafo();
                    loWord.NuevoParrafo();
                    #endregion
                }
            }

            string lsFileName = GetFileNameCorreo(liCodEmpleado);
            loWord.FilePath = lsFileName;
            loWord.SalvarComo();
            loWord.Cerrar();
            loWord.Salir();
            loWord = null;

            poMail = new MailAccess();
            poMail.NotificarSiHayError = false;
            poMail.IsHtml = true;
            poMail.OnSendCompleted = SendCompleted;
            poMail.De = GetRemitente();
            poMail.Asunto = Globals.GetLangItem("CartCustConfirmacion", psNombreEmpleado);
            if (psMensajeCorreo == "Aceptado")
            {
                poMail.Adjuntos.Add(new Attachment(lsFileName));
                poMail.CC.Add(psMailCC);
                poMail.Para.Add(GetMailPara());
            }
            else
            {
                poMail.Para.Add(psMailCC);
            }
            poMail.AgregarWord(lsFileName);
            poMail.EnviarAsincrono(liCodEmpleado);
        }

        public void EnviarCorreoProcesada()
        {
            int liCodEmpleado = int.Parse(psCodEmpleado);
            string lsWordPath = psPathCartasProcesada;

            WordAccess loWord = new WordAccess();
            if (lsWordPath != "" && System.IO.File.Exists(lsWordPath))
            {
                #region Con Plantilla del Cliente
                //Con Plantilla del Cliente
                loWord.FilePath = lsWordPath;

                loWord.Abrir(true);
                if (System.IO.File.Exists(psLogoClientePath))
                {
                    loWord.ReemplazarTextoPorImagen("{LogoCliente}", psLogoClientePath);
                }
                else
                {
                    loWord.ReemplazarTexto("{LogoCliente}", "");
                }
                if (System.IO.File.Exists(psLogoKeytiaPath))
                {
                    loWord.ReemplazarTextoPorImagen("{LogoKeytia}", psLogoKeytiaPath);
                }
                else
                {
                    loWord.ReemplazarTexto("{LogoKeytia}", "");
                }
                loWord.ReemplazarTexto("{FECHA}", DateTime.Today.ToString("dd-MMMM-yyyy").ToUpper());
                loWord.ReemplazarTexto("{NOMBRE EMPLEADO}", psNombreEmpleado);
                loWord.ReemplazarTexto("{NOMINA EMPLEADO}", psNominaEmpleado);
                #endregion
            }
            else
            {
                #region Plantilla Default
                lsWordPath = buscarPlantilla();

                if (lsWordPath != "" && System.IO.File.Exists(lsWordPath))
                {
                    //Con Plantilla Default
                    loWord.FilePath = lsWordPath;

                    loWord.Abrir(true);
                    if (System.IO.File.Exists(psLogoClientePath))
                    {
                        loWord.ReemplazarTextoPorImagen("{LogoCliente}", psLogoClientePath);
                    }
                    else
                    {
                        loWord.ReemplazarTexto("{LogoCliente}", "");
                    }
                    if (System.IO.File.Exists(psLogoKeytiaPath))
                    {
                        loWord.ReemplazarTextoPorImagen("{LogoKeytia}", psLogoKeytiaPath);
                    }
                    else
                    {
                        loWord.ReemplazarTexto("{LogoKeytia}", "");
                    }
                    loWord.ReemplazarTexto("{FECHA}", DateTime.Today.ToString("dd-MMMM-yyyy").ToUpper());
                    loWord.ReemplazarTexto("{NOMBRE EMPLEADO}", psNombreEmpleado);
                    loWord.ReemplazarTexto("{NOMINA EMPLEADO}", psNominaEmpleado);
                }
                #endregion
                else
                {
                    #region Sin Plantilla
                    //Sin Plantilla
                    string SaltoLinea = "\n";

                    loWord.Abrir(true);
                    if (System.IO.File.Exists(psLogoClientePath))
                    {
                        loWord.InsertarImagen(psLogoClientePath);
                    }

                    if (System.IO.File.Exists(psLogoKeytiaPath))
                    {
                        loWord.InsertarImagen(psLogoKeytiaPath);
                    }
                    loWord.NuevoParrafo();
                    loWord.InsertarTexto(SaltoLinea + DateTime.Today.ToString("dd-MMMM-yyyy").ToUpper());
                    loWord.NuevoParrafo();
                    loWord.NuevoParrafo();
                    string lsMensaje;
                    lsMensaje = Globals.GetMsgWeb(false, "CuerpoCorreoProcesada", psNombreEmpleado);
                    loWord.InsertarTexto(lsMensaje);
                    loWord.NuevoParrafo();
                    if (lsMensaje.StartsWith("#undefined-"))
                    {
                        loWord.InsertarTexto(psNombreEmpleado + " , Su carta custodia ha sido procesada, por lo cual le pedimos revise que sus datos estén correctos.");
                    }
                    loWord.NuevoParrafo();
                    loWord.NuevoParrafo();
                    #endregion
                }
            }

            string lsFileName = GetFileNameCorreo(liCodEmpleado);
            loWord.FilePath = lsFileName;
            loWord.SalvarComo();
            loWord.Cerrar();
            loWord.Salir();
            loWord = null;

            poMail = new MailAccess();
            poMail.NotificarSiHayError = false;
            poMail.IsHtml = true;
            poMail.OnSendCompleted = SendCompleted;
            poMail.De = GetRemitente();
            poMail.Asunto = Globals.GetLangItem("CartCustConfirmacion", psNombreEmpleado);
            poMail.AgregarWord(lsFileName);
            poMail.Para.Add(GetMailPara());
            poMail.EnviarAsincrono(liCodEmpleado);
        }

        private string GetFileNameCorreo(int liCodEmpleado)
        {
            psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), DSOUpload.EscapeFolderName(Session.SessionID));
            DataTable ldtEmpleado = pKDB.GetHisRegByEnt("Emple", "Empleados", "iCodCatalogo = " + liCodEmpleado);
            System.IO.Directory.CreateDirectory(psTempPath);
            string lsNameFile = Globals.GetMsgWeb(false, "TituloCartaCustodia").Trim() + "_Emp" + ldtEmpleado.Rows[0]["vchCodigo"].ToString().Trim() + "_" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (lsNameFile.StartsWith("#undefined"))
            {
                lsNameFile = lsNameFile.Replace("#", "");
                lsNameFile = lsNameFile.Replace("undefined-Titulo", "");
            }
            lsNameFile = lsNameFile.Replace(" ", "_");
            lsNameFile = lsNameFile.Replace("-", "_");
            lsNameFile = lsNameFile.Replace(":", "_");
            return System.IO.Path.GetFullPath(System.IO.Path.Combine(psTempPath, lsNameFile + ".docx"));
        }

        protected string GetMailPara()
        {
            DataTable lKDBTable = pKDB.GetHisRegByEnt("Emple", "Empleados", "iCodCatalogo = " + psCodEmpleado);
            psMailPara = "";

            if (lKDBTable.Rows.Count > 0)
            {
                DataRow lDataRow = lKDBTable.Rows[0];
                string lsMailEmple = (string)Util.IsDBNull(lDataRow["{Email}"], "");
                if (lsMailEmple.Length > 0 && !psMailPara.Contains(lsMailEmple))
                {
                    psMailPara = (psMailPara.Length > 0 ? (";" + lsMailEmple) : lsMailEmple);
                }
            }
            return psMailPara;
        }

        protected MailAddress GetRemitente()
        {
            if (string.IsNullOrEmpty(psMailRemitente))
            {
                return new MailAddress(Util.AppSettings("appeMailID"));
            }
            else if (string.IsNullOrEmpty(psNomRemitente))
            {
                return new MailAddress(psMailRemitente);
            }
            else
            {
                return new MailAddress(psMailRemitente, psNomRemitente);
            }
        }

        private void SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            int liCodEmpleado = (int)e.UserState;

            if (e.Error != null)
            {
                EnviarNotificacionCorreoNoValido(liCodEmpleado);
            }
            return;
        }

        private void EnviarNotificacionCorreoNoValido(int liCodEmpleado)
        {
            DataTable ldtEmpleado = pKDB.GetHisRegByEnt("Emple", "Empleados", "iCodCatalogo = " + liCodEmpleado);
            string[] lsParam = new string[] { (string)Util.IsDBNull(ldtEmpleado.Rows[0]["{Email}"], ""), (string)Util.IsDBNull(ldtEmpleado.Rows[0]["vchDescripcion"], "") };
            string lsErrMailAsunto = Globals.GetLangItem("ErrMailAsuntoEtiqueta", null); // "Error de envío de correo automático";
            string lsMensaje = Globals.GetLangItem("ErrMailAsuntoEtiqueta", lsParam);
            if (lsErrMailAsunto.StartsWith("#undefined-"))
            {
                lsErrMailAsunto = "Error de envío de correo automático";
            }
            if (lsMensaje.StartsWith("#undefined-"))
            {
                lsMensaje = "Surgió un error durante el envío de correo automático\r\nPara: {0}\r\nEmpleado: {1}";
                lsMensaje = string.Format(lsMensaje, lsParam);
            }

            MailAccess loMail = new MailAccess();
            loMail.NotificarSiHayError = false;
            loMail.IsHtml = true;
            loMail.De = GetRemitente();
            loMail.Asunto = "Cartas Custodia Aceptacion";
            loMail.Asunto = lsErrMailAsunto + ": " + loMail.Asunto;
            loMail.Para.Add(psMailPara);
            loMail.EnviarAsincrono(liCodEmpleado);
        }

        #endregion

        #region WebMethods
        public static DSOGridServerResponse GetHisRecAceptados(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloCartaCustodia");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            if (HttpContext.Current.Session["iCodEmpleado"] == null)
            {
                CartasCustodia CartCust = new CartasCustodia();
                CartCust.SetEmpleado();
                HttpContext.Current.Session["iCodEmpleado"] = CartCust.psCodEmpleado;
            }

            try
            {
                KDBAccess lKDB = new KDBAccess();
                CartasCustodiaFieldCollection lFields = new CartasCustodiaFieldCollection(iCodEntidad, iCodMaestro);
                StringBuilder lsbSelectTop = new StringBuilder();
                StringBuilder lsbQuery = new StringBuilder();
                StringBuilder lsbIcodUsuario = new StringBuilder();
                StringBuilder lsbFrom = new StringBuilder();
                StringBuilder lsbWhere = new StringBuilder();
                StringBuilder lsbOrderBy = new StringBuilder();
                DateTime ldtVigencia = DateTime.Now;
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

                    if (lsOrderCol == "iCodRegistro")
                    {
                        lsOrderCol = "vchCodigo";
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
                    lsOrderCol = "iCodRelacion";
                    lsOrderDir = " asc";
                    lsOrderDirInv = " desc";
                }
                #region Top, Select From
                lsbQuery.AppendLine("select * from (");
                lsbQuery.AppendLine("   select top " + gsRequest.iDisplayLength + " * from (");
                lsbQuery.AppendLine("       select top " + (gsRequest.iDisplayStart + gsRequest.iDisplayLength) + " * from (");
                //lsbQuery.AppendLine("           select * from [VisHistoricos('CartaCust','Recursos Aceptados','" + Globals.GetCurrentLanguage() + "')]");
                lsbQuery.AppendLine("           select  iCodRegistro,iCodCatalogo,iCOdMaestro,vchCodigo,vchDescripcion,");
                lsbQuery.AppendLine("                   DescRecurso, Recurs = RecursDesc,FechaAsignacion,FechaAceptacion,");
                lsbQuery.AppendLine("                   Emple,EmpleCod,EmpleDesc,RegRelacion,dtIniVigencia,dtFinVigencia,iCodUsuario");
                lsbQuery.AppendLine("           from [VisHistoricos('CartaCust','Recursos Aceptados','" + Globals.GetCurrentLanguage() + "')]");
                lsbQuery.AppendLine("           where Emple = " + HttpContext.Current.Session["iCodEmpleado"]);
                lsbQuery.AppendLine("                 and dtIniVigencia < '" + DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                lsbQuery.AppendLine("                 and dtFinVigencia > '" + DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                lsbQuery.AppendLine(") as RegistrosAMostrar");
                #endregion
                //////////////////////////////////////////////////////////////////////////7

                //formatos de conversion de fechas de sql
                string lsSqlDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlDateFormat");
                string lsSqlTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlTimeFormat");

                #region Where
                string[] lsColumns = gsRequest.sColumns.Split(',');
                foreach (string lsColumn in lsColumns)
                {
                    string lsFiltro = gsRequest.sSearchGlobal.Replace("'", "''").Trim();

                    if (!String.IsNullOrEmpty(lsFiltro)
                        && (lsColumn == "DescRecurso"
                        || lsColumn == "Recurs"
                        || lsColumn == "FechaAsignacion"
                        || lsColumn == "FechaAceptacion"))
                    {
                        if (lsbWhere.Length == 0)
                        {
                            lsbWhere.AppendLine("where");
                        }
                        else
                        {
                            lsbWhere.Append("or ");
                        }

                        if (lsColumn == "FechaAsignacion" || lsColumn == "FechaAceptacion")
                        {
                            lsbWhere.Append("(");
                            lsbWhere.AppendLine("convert(varchar, " + lsColumn + ", " + lsSqlDateFormat + ") like '%" + lsFiltro + "%'");
                            lsbWhere.AppendLine("or convert(varchar, " + lsColumn + ", " + lsSqlTimeFormat + ") like '%" + lsFiltro + "%'");
                            lsbWhere.AppendLine(")");
                        }
                        else
                        {
                            lsbWhere.AppendLine(lsColumn + " like '%" + lsFiltro + "%'");
                        }
                    }
                }
                #endregion

                #region Order
                if (lsOrderCol == "vchCodigo")
                {
                    lsbOrderBy.AppendLine("       order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);
                }
                else
                {
                    lsbOrderBy.AppendLine("       order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);
                }
                lsbOrderBy.AppendLine("   ) as Paginacion order by " + lsOrderCol + lsOrderDirInv + ", iCodRegistro" + lsOrderDirInv);
                lsbOrderBy.AppendLine(") as RecursosPendientes order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);
                #endregion

                string lsQuery = lsbQuery.ToString();
                string lsWhere = lsbWhere.ToString();
                string lsOrderBy = lsbOrderBy.ToString();


                ldt = DSODataAccess.Execute(lsQuery + lsWhere + lsOrderBy);

                lgsrRet.sEcho = gsRequest.sEcho;
                lgsrRet.iTotalRecords = ldt.Rows.Count; ;
                lgsrRet.iTotalDisplayRecords = lgsrRet.iTotalRecords;

                lgsrRet.ProcesarDatos(gsRequest, ldt);

                if (lgsrRet.iTotalDisplayRecords > 0 && ldt.Rows.Count == 0)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloCartaCustodia");
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                string lsDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");
                Dictionary<string, string> lColStringFormat = new Dictionary<string, string>();
                Dictionary<string, IFormatProvider> lColFormatter = new Dictionary<string, IFormatProvider>();
                
                lColStringFormat.Add("FechaAsignacion", lsDateFormat);
                lColStringFormat.Add("FechaAceptacion", lsDateFormat);

                lgsrRet.SetDataFromDataTable(ldt, lColStringFormat, lColFormatter);

                return lgsrRet;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloCartaCustodia");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }
        public static DSOGridServerResponse GetHisRecPend(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro, List<Parametro> parametros)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloCartaCustodia");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                KDBAccess lKDB = new KDBAccess();
                CartasCustodiaFieldCollection lFields = new CartasCustodiaFieldCollection(iCodEntidad, iCodMaestro);
                CartasCustodia CartCust = new CartasCustodia();
                StringBuilder lsbSelectTop = new StringBuilder();
                StringBuilder lsbQuery = new StringBuilder();
                StringBuilder lsbIcodUsuario = new StringBuilder();
                StringBuilder lsbFrom = new StringBuilder();
                StringBuilder lsbWhere = new StringBuilder();
                StringBuilder lsbOrderBy = new StringBuilder();
                DateTime ldtVigencia = DateTime.Now;
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

                    if (lsOrderCol == "iCodRegistro")
                    {
                        lsOrderCol = "iCodRelacion";
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
                    lsOrderCol = "iCodRelacion";
                    lsOrderDir = " asc";
                    lsOrderDirInv = " desc";
                }
                #region Top, Select From
                lsbQuery.AppendLine("select * from (");
                lsbQuery.AppendLine("   select top " + gsRequest.iDisplayLength + " * from (");
                lsbQuery.AppendLine("       select top " + (gsRequest.iDisplayStart + gsRequest.iDisplayLength) + " * from (");
                lsbQuery.AppendLine(CartCust.ObtenerQueryRecursos());
                lsbQuery.AppendLine(") as RegistrosAMostrar");
                #endregion
                //////////////////////////////////////////////////////////////////////////7

                //formatos de conversion de fechas de sql
                string lsSqlDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlDateFormat");
                string lsSqlTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlTimeFormat");

                #region Where
                string[] lsColumns = gsRequest.sColumns.Split(',');
                foreach (string lsColumn in lsColumns)
                {
                    string lsFiltro = gsRequest.sSearchGlobal.Replace("'", "''").Trim();

                    if (!String.IsNullOrEmpty(lsFiltro)
                        && (lsColumn == "Recurso"
                        || lsColumn == "TipoRecurso"
                        || lsColumn == "FechaAsignacion"))
                    {
                        if (lsbWhere.Length == 0)
                        {
                            lsbWhere.AppendLine("where");
                        }
                        else
                        {
                            lsbWhere.Append("or ");
                        }

                        if (lsColumn == "FechaAsignacion")
                        {
                            lsbWhere.Append("(");
                            lsbWhere.AppendLine("convert(varchar, " + lsColumn + ", " + lsSqlDateFormat + ") like '%" + lsFiltro + "%'");
                            lsbWhere.AppendLine("or convert(varchar, " + lsColumn + ", " + lsSqlTimeFormat + ") like '%" + lsFiltro + "%'");
                            lsbWhere.AppendLine(")");
                        }
                        else
                        {
                            lsbWhere.AppendLine(lsColumn + " like '%" + lsFiltro + "%'");
                        }
                    }
                }
                #endregion

                #region Order
                if (lsOrderCol == "iCodRelacion")
                {
                    lsbOrderBy.AppendLine("       order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);
                }
                else
                {
                    lsbOrderBy.AppendLine("       order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);
                }
                lsbOrderBy.AppendLine("   ) as Paginacion order by " + lsOrderCol + lsOrderDirInv + ", iCodRegistro" + lsOrderDirInv);
                lsbOrderBy.AppendLine(") as RecursosPendientes order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);
                #endregion

                string lsQuery = lsbQuery.ToString();
                string lsWhere = lsbWhere.ToString();
                string lsOrderBy = lsbOrderBy.ToString();


                //ldt = CartCust.RemoverAceptados(DSODataAccess.Execute(lsQuery + lsWhere + lsOrderBy));
                ldt = CartCust.RemoverRecAceptados(DSODataAccess.Execute(lsQuery + lsWhere + lsOrderBy));

                lgsrRet.sEcho = gsRequest.sEcho;
                lgsrRet.iTotalRecords = ldt.Rows.Count; ;
                lgsrRet.iTotalDisplayRecords = lgsrRet.iTotalRecords;

                lgsrRet.ProcesarDatos(gsRequest, ldt);

                if (lgsrRet.iTotalDisplayRecords > 0 && ldt.Rows.Count == 0)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloCartaCustodia");
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                string lsDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");
                Dictionary<string, string> lColStringFormat = new Dictionary<string, string>();
                Dictionary<string, IFormatProvider> lColFormatter = new Dictionary<string, IFormatProvider>();

                lColStringFormat.Add("FechaAsignacion", lsDateFormat);

                lgsrRet.SetDataFromDataTable(ldt, lColStringFormat, lColFormatter);

                return lgsrRet;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloCartaCustodia");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }
        public static DSOGridServerResponse GetHisRecPendLiberar(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro, List<Parametro> parametros)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloCartaCustodia");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                KDBAccess lKDB = new KDBAccess();
                CartasCustodiaFieldCollection lFields = new CartasCustodiaFieldCollection(iCodEntidad, iCodMaestro);
                CartasCustodia CartCust = new CartasCustodia();
                StringBuilder lsbSelectTop = new StringBuilder();
                StringBuilder lsbQuery = new StringBuilder();
                StringBuilder lsbIcodUsuario = new StringBuilder();
                StringBuilder lsbFrom = new StringBuilder();
                StringBuilder lsbWhere = new StringBuilder();
                StringBuilder lsbOrderBy = new StringBuilder();
                DateTime ldtVigencia = DateTime.Now;
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

                    if (lsOrderCol == "iCodRegistro")
                    {
                        lsOrderCol = "RecursoCod";
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
                    lsOrderCol = "RecursoCod";
                    lsOrderDir = " asc";
                    lsOrderDirInv = " desc";
                }
                #region Top, Select From
                CartCust.SetEmpleado();
                lsbQuery.AppendLine("select * from (");
                lsbQuery.AppendLine("   select top " + gsRequest.iDisplayLength + " * from (");
                lsbQuery.AppendLine("       select top " + (gsRequest.iDisplayStart + gsRequest.iDisplayLength) + " * from (");
                //lsbQuery.AppendLine("           select Recurso = RecursCod,");
                lsbQuery.AppendLine("           select Recurso = DescRecurso,");
                lsbQuery.AppendLine("                  TipoRecurso = RecursDesc,");
                lsbQuery.AppendLine("                  FechaAsignacion = dtIniVigencia,");
                lsbQuery.AppendLine("                  RecursoCod = RecursCod,");
                lsbQuery.AppendLine("                  TipoRecursoCod = Recurs,");
                lsbQuery.AppendLine("                  iCodRegistro,");
                lsbQuery.AppendLine("                  Emple,");
                lsbQuery.AppendLine("                  EmpleDesc,");
                lsbQuery.AppendLine("                  RegRelacion,");
                lsbQuery.AppendLine("                  dtinivigencia,");
                lsbQuery.AppendLine("                  dtfinvigencia");
                lsbQuery.AppendLine("           from [VisHistoricos('CartaCust','Recursos Aceptados','" + Globals.GetCurrentLanguage() + "')]");
                lsbQuery.AppendLine("           where Emple = " + CartCust.psCodEmpleado);
                lsbQuery.AppendLine("                 and dtIniVigencia < '" + DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                lsbQuery.AppendLine("                 and dtFinVigencia > '" + DateTime.Today.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                lsbQuery.AppendLine(") as RegistrosAMostrar");
                #endregion
                //////////////////////////////////////////////////////////////////////////7

                //formatos de conversion de fechas de sql
                string lsSqlDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlDateFormat");
                string lsSqlTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlTimeFormat");

                #region Where
                string[] lsColumns = gsRequest.sColumns.Split(',');
                foreach (string lsColumn in lsColumns)
                {
                    string lsFiltro = gsRequest.sSearchGlobal.Replace("'", "''").Trim();

                    if (!String.IsNullOrEmpty(lsFiltro)
                        && (lsColumn == "Recurso"
                        || lsColumn == "TipoRecurso"
                        || lsColumn == "FechaAsignacion"))
                    {
                        if (lsbWhere.Length == 0)
                        {
                            lsbWhere.AppendLine("where");
                        }
                        else
                        {
                            lsbWhere.Append("or ");
                        }

                        if (lsColumn == "FechaAsignacion")
                        {
                            lsbWhere.Append("(");
                            lsbWhere.AppendLine("convert(varchar, " + lsColumn + ", " + lsSqlDateFormat + ") like '%" + lsFiltro + "%'");
                            lsbWhere.AppendLine("or convert(varchar, " + lsColumn + ", " + lsSqlTimeFormat + ") like '%" + lsFiltro + "%'");
                            lsbWhere.AppendLine(")");
                        }
                        else
                        {
                            lsbWhere.AppendLine(lsColumn + " like '%" + lsFiltro + "%'");
                        }
                    }
                }
                #endregion

                #region Order
                if (lsOrderCol == "iCodRelacion")
                {
                    lsbOrderBy.AppendLine("       order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);
                }
                else
                {
                    lsbOrderBy.AppendLine("       order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);
                }
                lsbOrderBy.AppendLine("   ) as Paginacion order by " + lsOrderCol + lsOrderDirInv + ", iCodRegistro" + lsOrderDirInv);
                lsbOrderBy.AppendLine(") as RecursosPendientes order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);
                #endregion

                string lsQuery = lsbQuery.ToString();
                string lsWhere = lsbWhere.ToString();
                string lsOrderBy = lsbOrderBy.ToString();


                //ldt = CartCust.PendientesLiberar(DSODataAccess.Execute(lsQuery + lsWhere + lsOrderBy));
                ldt = CartCust.RecPendientesLiberar(DSODataAccess.Execute(lsQuery + lsWhere + lsOrderBy));

                lgsrRet.sEcho = gsRequest.sEcho;
                lgsrRet.iTotalRecords = ldt.Rows.Count; ;
                lgsrRet.iTotalDisplayRecords = lgsrRet.iTotalRecords;

                lgsrRet.ProcesarDatos(gsRequest, ldt);

                if (lgsrRet.iTotalDisplayRecords > 0 && ldt.Rows.Count == 0)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloCartaCustodia");
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                string lsDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");
                Dictionary<string, string> lColStringFormat = new Dictionary<string, string>();
                Dictionary<string, IFormatProvider> lColFormatter = new Dictionary<string, IFormatProvider>();

                lColStringFormat.Add("FechaAsignacion", lsDateFormat);

                lgsrRet.SetDataFromDataTable(ldt, lColStringFormat, lColFormatter);

                return lgsrRet;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloCartaCustodia");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }
        #endregion
    }
}
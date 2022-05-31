using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DSOControls2008;
using KeytiaServiceBL;
using SeeYouOnServiceBL;
using System.Text;
using System.Data;

namespace KeytiaWeb.UserInterface
{
    public class CnfgProyectos : HistoricEdit
    {
        protected StringBuilder psbErrores;
        protected HashSet<string> pHTOcultos;
        public CnfgProyectos()
        {
            Init += new EventHandler(CnfgProyectos_Init);
        }

        void CnfgProyectos_Init(object sender, EventArgs e)
        {
            this.CssClass = "HistoricEdit CnfgProyectos";

            pHTOcultos = new HashSet<string>();
            if (Session["vchCodPerfil"].ToString() != "SeeYouOnAdmin")
            {
                pHTOcultos.Add("Client");
            }
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);
            foreach (string lsCampo in pHTOcultos)
            {
                OcultaCampo(lsCampo);
            }
        }

        protected override void InitFiltros()
        {
            base.InitFiltros();
            foreach (string lsCampo in pHTOcultos)
            {
                OcultaCampoFiltro(lsCampo);
            }
        }

        protected override void InitFiltrosFields()
        {
            DSOTextBox lDSOtxt;
            foreach (KeytiaBaseField lField in pFields)
            {
                if (lField.ShowInGrid)
                {
                    lDSOtxt = new DSOTextBox();
                    lDSOtxt.ID = lField.Column;
                    if (lField.Column.StartsWith("iCodCatalogo"))
                    {
                        lDSOtxt.AddClientEvent("dataFilter", lField.ConfigName + "Desc");
                    }
                    else
                    {
                        lDSOtxt.AddClientEvent("dataFilter", lField.ConfigName);
                    }
                    lDSOtxt.Row = lField.Row + 2;
                    lDSOtxt.ColumnSpan = lField.ColumnSpan;
                    lDSOtxt.Table = pTablaFiltros;
                    lDSOtxt.CreateControls();

                    phtFiltros.Add(lDSOtxt.ID, lDSOtxt);
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
            pHisGrid.Config.sScrollY = "400px";
            pHisGrid.Config.sPaginationType = "full_numbers";
            pHisGrid.Config.bJQueryUI = true;
            pHisGrid.Config.bProcessing = true;
            pHisGrid.Config.bServerSide = true;
            pHisGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerData(this, sSource, aoData, fnCallback);}";
            pHisGrid.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetSeeYouOnProyectos");

            lCol = new DSOGridClientColumn();
            lCol.sName = "iCodRegistro";
            lCol.aTargets.Add(lTarget++);
            lCol.bVisible = false;
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "Consultar";
            lCol.aTargets.Add(lTarget++);
            lCol.sWidth = "50px";
            lCol.sClass = "TdConsult";
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            lCol = new DSOGridClientColumn();
            lCol.sName = "vchCodigo";
            lCol.aTargets.Add(lTarget++);
            pHisGrid.Config.aoColumnDefs.Add(lCol);

            //lCol = new DSOGridClientColumn();
            //lCol.sName = "vchDescripcion";
            //lCol.aTargets.Add(lTarget++);
            //pHisGrid.Config.aoColumnDefs.Add(lCol);

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

        protected override void InitGridFields()
        {
            foreach (KeytiaBaseField lField in pFields)
            {
                if (lField.ShowInGrid &&
                    (lField.ConfigName != "Client" ||
                    (lField.ConfigName == "Client" && Session["vchCodPerfil"].ToString() == "SeeYouOnAdmin")))
                {
                    AgregarGridField(lField);
                }
            }
        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            pvchDescripcion.TextBox.Enabled = false;
            pvchDescripcion.TcCtl.Visible = false;
            pvchDescripcion.TcLbl.Visible = false;
        }

        protected override void InitHisGridLanguage()
        {
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
                else if (lCol.sName == "vchCodigo")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "vchCodigo"));
                }
                else if (lCol.sName == "vchDescripcion")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "vchDescripcion"));
                }
                else if (lCol.sName == "dtIniVigencia")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "dtIniVigencia"));
                }
                else if (lCol.sName == "dtFinVigencia")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "dtFinVigencia"));
                }
                else if (lCol.sName == "dtFecUltAct")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "dtFecUltAct"));
                    lCol.bVisible = false;
                }
                else if (lCol.sName == "Consultar")
                {
                    string lsdoPostBack = DSOControl.JScriptEncode(Page.ClientScript.GetPostBackEventReference(this, "btnConsultar:{0}"));
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "btnConsultar"));
                    lCol.bUseRendered = false;
                    lCol.fnRender = "function(obj){ return " + pjsObj + ".fnRender(obj,\"custom-img-search\",\"" + ResolveUrl("~/images/searchsmall.png") + "\",\"" + lsdoPostBack + "\");}";
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

        protected virtual void AgregarGridField(KeytiaBaseField lField)
        {
            DSOGridClientColumn lCol;
            int lTarget = pHisGrid.Config.aoColumnDefs.Count;

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

        protected override void AgregarRegistro()
        {
            base.AgregarRegistro();
            pdtIniVigencia.DataValue = DateTime.Today;
            pdtFinVigencia.DataValue = new DateTime(2079, 1, 1);

            pFields.GetByConfigName("Client").DataValue = GetClienteUsuario((int)Session["iCodUsuario"], pdtIniVigencia.Date);

            phtValues = pFields.GetValues();
            pdsRelValues = pFields.GetRelationValues();
        }

        protected static int GetClienteUsuario(int liCodUsuario, DateTime ldtVigencia)
        {
            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.Length = 0;
            lsbQuery.AppendLine("select Empre.Client");
            lsbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('Empre','Empresas','" + Globals.GetCurrentLanguage() + "')] Empre,");
            lsbQuery.AppendLine(DSODataContext.Schema + ".[VisHistoricos('Usuar','Usuarios','" + Globals.GetCurrentLanguage() + "')] Usuar");
            lsbQuery.AppendLine("where Empre.iCodCatalogo = Usuar.Empre");
            lsbQuery.AppendLine("and Usuar.iCodCatalogo = " + liCodUsuario);
            lsbQuery.AppendLine("and Empre.dtIniVigencia <> Empre.dtFinVigencia");
            lsbQuery.AppendLine("and Empre.dtIniVigencia <= '" + ldtVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsbQuery.AppendLine("and Empre.dtFinVigencia > '" + ldtVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsbQuery.AppendLine("and Usuar.dtIniVigencia <> Usuar.dtFinVigencia");
            lsbQuery.AppendLine("and Usuar.dtIniVigencia <= '" + ldtVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsbQuery.AppendLine("and Usuar.dtFinVigencia > '" + ldtVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");

            return (int)DSODataAccess.ExecuteScalar(lsbQuery.ToString());
        }

        protected override bool ValidarClaves()
        {

            bool lbret = true;
            StringBuilder lsbErrores = new StringBuilder();
            StringBuilder lsbErroresCodigos = new StringBuilder();
            string lsError;
            string lsTitulo = DSOControl.JScriptEncode(AlertTitle);
            DataTable ldt;

            //Agregar el vchCodigo del cliente a la descripcion
            if (pFields.ContainsConfigName("Descripcion") && pFields.GetByConfigName("Descripcion").DSOControlDB.HasValue)
            {
                string lsDescripcion;
                DSOControlDB lCtl = pFields.GetByConfigName("Descripcion").DSOControlDB;
                lCtl.DataValueDelimiter = "";
                lsDescripcion = lCtl.ToString();
                lCtl.DataValueDelimiter = "'";

                pvchDescripcion.DataValue = lsDescripcion.Substring(0, Math.Min(lsDescripcion.Length, 110)) + " ";

            }
            if (pFields.ContainsConfigName("Client") && pFields.GetByConfigName("Client").DSOControlDB.HasValue)
            {
                string lsDescripcion;
                pvchDescripcion.DataValueDelimiter = "";
                lsDescripcion = pvchDescripcion.ToString();
                pvchDescripcion.DataValueDelimiter = "'";
                pvchDescripcion.DataValue = lsDescripcion
                    + "(" + DSODataAccess.ExecuteScalar(
                    "Select IsNull(vchCodigo, '') " +
                    "from Catalogos C, Historicos H " +
                    "where H.iCodCatalogo = C.iCodRegistro " +
                    "and H.dtIniVigencia <> H.dtFinVigencia " +
                    "and H.dtIniVigencia <= " + pdtIniVigencia.DataValue.ToString() + " " +
                    "and H.dtFinVigencia >  " + pdtIniVigencia.DataValue.ToString() + " " +
                    "and H.iCodCatalogo = " + pFields.GetByConfigName("Client").DataValue.ToString(), (object)"").ToString() + ")";
            }

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
                    psbQuery.AppendLine("and V.Client = IsNull(" + pFields.GetByConfigName("Client").DataValue.ToString() + ", -1)");
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

        #region WebMethods
        public static DSOGridServerResponse GetSeeYouOnProyectos(DSOGridServerRequest gsRequest, int iCodEntidad, int iCodMaestro)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloHistoricos");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                DateTime ldtVigencia = DateTime.Now;
                HistoricFieldCollection lFields = new HistoricFieldCollection(iCodEntidad, iCodMaestro);
                StringBuilder lsbSelectTop = new StringBuilder();
                StringBuilder lsbColumnas = new StringBuilder();
                StringBuilder lsbFrom = new StringBuilder();
                StringBuilder lsbWhere = new StringBuilder();
                StringBuilder lsbOrderBy = new StringBuilder();

                string lsEntidad = DSODataAccess.ExecuteScalar("select vchCodigo from Catalogos where iCodCatalogo is null and iCodRegistro = " + iCodEntidad).ToString();
                string lsMaestro = DSODataAccess.ExecuteScalar("select vchDescripcion from Maestros where iCodRegistro = " + iCodMaestro).ToString();

                DSOGridServerResponse lgsrRet = new DSOGridServerResponse();
                DataTable ldt;

                string lsOrderCol = "";
                string lsOrderDir = "";
                string lsOrderDirInv = "";
                string[] larrColumns = gsRequest.sColumns.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (gsRequest.iSortCol.Count > 0)
                {
                    lsOrderCol = larrColumns[gsRequest.iSortCol[0]];

                    if ((!lFields.ContainsConfigName(lsOrderCol)
                        || (lsOrderCol.EndsWith("Desc")
                            && !lFields.ContainsConfigName(lsOrderCol.Substring(0, lsOrderCol.Length - 4))))
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
                    lsOrderCol = "dtIniVigencia";
                    lsOrderDir = " asc";
                    lsOrderDirInv = " desc";
                }

                lsbSelectTop.AppendLine("select * from (");
                lsbSelectTop.AppendLine("   select top " + gsRequest.iDisplayLength + " * from (");
                lsbSelectTop.AppendLine("       select top " + (gsRequest.iDisplayStart + gsRequest.iDisplayLength));

                ////////////////////////////////////////////////////////////////////////////

                lsbColumnas.AppendLine("iCodRegistro");
                lsbColumnas.AppendLine(",Consultar = null");
                lsbColumnas.AppendLine(",vchCodigo");
                lsbColumnas.AppendLine(",vchDescripcion");

                foreach (KeytiaBaseField lField in lFields)
                {
                    if (lField.Column.StartsWith("iCodCatalogo")
                        && larrColumns.Contains<string>(lField.ConfigName + "Desc"))
                    {
                        lsbColumnas.AppendLine(",[" + lField.ConfigName + "Desc]");
                    }
                    else if (larrColumns.Contains<string>(lField.ConfigName))
                    {
                        lsbColumnas.AppendLine(",[" + lField.ConfigName + "]");
                    }
                }

                lsbColumnas.AppendLine(",dtIniVigencia");
                lsbColumnas.AppendLine(",dtFinVigencia");
                lsbColumnas.AppendLine(",dtFecUltAct");

                //////////////////////////////////////////////////////////////////////////7

                lsbFrom.AppendLine("      from " + DSODataContext.Schema + ".[VisHistoricos('" + lsEntidad + "','" + lsMaestro + "','" + Globals.GetCurrentLanguage() + "')] a");
                lsbFrom.AppendLine("      where dtIniVigencia <> dtFinVigencia");

                if (ldtVigencia != null)
                {
                    lsbFrom.AppendLine("      and '" + DSOControl.ParseDateTimeJS(ldtVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "' >= dtIniVigencia");
                    lsbFrom.AppendLine("      and '" + DSOControl.ParseDateTimeJS(ldtVigencia, true).ToString("yyyy-MM-dd HH:mm:ss") + "' < dtFinVigencia");
                }

                if (HttpContext.Current.Session["vchCodPerfil"].ToString() != "SeeYouOnAdmin")
                {
                    int liCodUsuario = (int)HttpContext.Current.Session["iCodUsuario"];
                    int liCodCliente = GetClienteUsuario(liCodUsuario, DSOControl.ParseDateTimeJS(ldtVigencia, false));
                    lsbFrom.AppendLine("      and Client = " + liCodCliente);
                }

                lsbOrderBy.AppendLine("       order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);
                lsbOrderBy.AppendLine("   ) as a order by " + lsOrderCol + lsOrderDirInv + ", iCodRegistro" + lsOrderDirInv);
                lsbOrderBy.AppendLine(") as a order by " + lsOrderCol + lsOrderDir + ", iCodRegistro" + lsOrderDir);

                //formatos de conversion de fechas de sql
                string lsSqlDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlDateFormat");
                string lsSqlTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlTimeFormat");


                if (gsRequest.sSearchGlobal.Trim() != "")
                {
                    string lsTerm = gsRequest.sSearchGlobal.Replace("'", "''").Trim();
                    StringBuilder lsbColTodas = new StringBuilder();
                    lsbColTodas.AppendLine("isnull(a.vchCodigo,'')");
                    lsbColTodas.AppendLine("+ ' ' + isnull(a.vchDescripcion,'')");

                    lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.dtIniVigencia," + lsSqlDateFormat + "),'')+ ' ' + isnull(convert(varchar,a.dtIniVigencia," + lsSqlTimeFormat + "),'')");
                    lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.dtFinVigencia," + lsSqlDateFormat + "),'')+ ' ' + isnull(convert(varchar,a.dtFinVigencia," + lsSqlTimeFormat + "),'')");

                    foreach (KeytiaBaseField lField in lFields)
                    {
                        if (larrColumns.Contains<string>(lField.ConfigName)
                            || larrColumns.Contains<string>(lField.ConfigName + "Desc"))
                        {
                            if (lField.Column.StartsWith("Date"))
                            {
                                lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.[" + lField.ConfigName + "]," + lsSqlDateFormat + "),'') + ' ' + isnull(convert(varchar,a.[" + lField.ConfigName + "]," + lsSqlTimeFormat + "),'')");
                            }
                            else if (lField.Column.StartsWith("iCodCatalogo"))
                            {
                                lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.[" + lField.ConfigName + "Desc]),'')");
                            }
                            else if (!lField.Column.StartsWith("VarChar"))
                            {
                                lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a.[" + lField.ConfigName + "]),'')");
                            }
                            else
                            {
                                lsbColTodas.AppendLine("+ ' ' + isnull(a.[" + lField.ConfigName + "],'')");
                            }
                        }
                    }
                    lsbWhere.AppendLine("and " + lsbColTodas.ToString() + " like '%" + lsTerm + "%'");
                }

                int lidx;
                for (lidx = 0; lidx < larrColumns.Length; lidx++)
                {
                    if (gsRequest.bSearchable[lidx])
                    {
                        string lsColumn = larrColumns[lidx];
                        string lsFiltro = gsRequest.sSearch[lidx].Replace("'", "''").Trim();

                        if (!String.IsNullOrEmpty(lsFiltro)
                            && (lFields.ContainsConfigName(lsColumn)
                            || (lsColumn.EndsWith("Desc") && lFields.ContainsConfigName(lsColumn.Substring(0, lsColumn.Length - 4)))
                            || lsColumn == "vchCodigo"
                            || lsColumn == "vchDescripcion"
                            || lsColumn == "dtIniVigencia"
                            || lsColumn == "dtFinVigencia"))
                        {
                            lsbWhere.Append("and ");

                            if ((lFields.ContainsConfigName(lsColumn)
                                    && lFields.GetByConfigName(lsColumn) is KeytiaDateTimeField)
                                || lsColumn == "dtIniVigencia"
                                || lsColumn == "dtFinVigencia")
                            {

                                lsbWhere.Append("(");
                                lsbWhere.AppendLine("convert(varchar, a.[" + lsColumn + "], " + lsSqlDateFormat + ") like '%" + lsFiltro + "%'");
                                lsbWhere.AppendLine("or convert(varchar, a.[" + lsColumn + "], " + lsSqlTimeFormat + ") like '%" + lsFiltro + "%'");
                                lsbWhere.AppendLine(")");
                            }
                            else
                            {
                                lsbWhere.AppendLine("a.[" + lsColumn + "] like '%" + lsFiltro + "%'");
                            }
                        }
                    }
                }

                string lsSelectCount = "select count(iCodRegistro) ";
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

                lFields.FormatGridData(lgsrRet, ldt, true);

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

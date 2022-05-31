/*
Nombre:		    PGS
Fecha:		    20111001
Descripción:	Clase que se utiliza en los detalles que se muestran desde la aplicación Etiquetación.
Modificación:	
*/
using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using DSOControls2008;
using KeytiaServiceBL;
using System.Data;
using System.Reflection;

namespace KeytiaWeb.UserInterface
{
    public class EtiquetaDetalle : EtiquetaEdit
    {
        private string psLinea = "";
        private string psGrupo = "";

        public string Linea
        {
            get
            {
                return psLinea;
            }
            set
            {
                psLinea = value;
            }
        }

        public string Grupo
        {
            get
            {
                return psGrupo;
            }
            set
            {
                psGrupo = value;
            }
        }

        //NZ
        protected static bool pbEtiquetarIncluirLlamadasEntrada;

        public virtual bool pbDetalleLinea
        {
            get
            {
                if (ViewState["bDetLinea"] == null)
                {
                    ViewState["bDetLinea"] = false;
                }
                return (bool)ViewState["bDetLinea"];
            }
            set
            {
                ViewState["bDetLinea"] = value;
            }
        }

        protected override void SetMaestros()
        {
            iCodEntidadEtq = DSODataAccess.Execute("Select iCodRegistro from Catalogos where vchCodigo='EtiquetaApp' and iCodCatalogo is null and dtIniVigencia <> dtFinVigencia").Rows[0]["iCodRegistro"].ToString();
            iCodMaestroEtq = DSODataAccess.Execute("Select iCodRegistro from Maestros where iCodEntidad = " + iCodEntidadEtq + " and vchDescripcion='Etiquetacion' and dtIniVigencia <> dtFinVigencia").Rows[0]["iCodRegistro"].ToString();
            iCodMaestroResumen = DSODataAccess.Execute("Select iCodRegistro from Maestros where iCodEntidad = " + iCodEntidadEtq + " and vchDescripcion='" + vchDesMaestroResumen + "' and dtIniVigencia <> dtFinVigencia").Rows[0]["iCodRegistro"].ToString();
        }

        public EtiquetaDetalle(string liCodEmpleado, string lvchCodEntidad, string lvchDesMaestro)
        {
            iCodEmpleado = liCodEmpleado;
            vchCodEntidad = lvchCodEntidad;
            vchDesMaestro = lvchDesMaestro;
        }

        protected override void EtiquetaEdit_Init(object sender, EventArgs e)
        {
            SetMaestros();
            iCodEntidad = iCodEntidadEtq;
            iCodMaestro = iCodMaestroEtq;
            Title = "";

            //NZ 20150831
            //Verificar si se deben de incluir llamadas de entrada para este cliente. Bandera BanderasEtiquetacion Bit 128.
            BanderaLlamsEntrada();


            pExpResumen = new DSOExpandable();
            pTablaResumen = new Table();
            pResumenGrid = new DSOGrid();

            this.CssClass = "EtiquetaDetalle";

            Controls.Add(pExpResumen);
            Controls.Add(pResumenGrid);

            CreateControls();
        }

        private static void BanderaLlamsEntrada()
        {
            pbEtiquetarIncluirLlamadasEntrada = false;
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT iCodCatalogo ");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Client','Clientes','Español')]");
            query.AppendLine("WHERE dtinivigencia <> dtfinvigencia ");
            query.AppendLine("AND dtfinvigencia >= getdate()");
            query.AppendLine("AND (ISNULL(BanderasEtiquetacion,0) & 128)/128=1");
            DataTable dtResult = DSODataAccess.Execute(query.ToString());
            if (dtResult.Rows.Count > 0)
            {
                pbEtiquetarIncluirLlamadasEntrada = true;
            }
        }

        protected override void InitAccionesToolBar()
        {
        }

        protected override void InitAccionesSecundarias()
        {
            InitRegistroResumen();
            CreateGridResumen();

            SetHistoricState(HistoricState.Edicion);
            InitMaestro();
            InitLanguage();
        }

        public override void InitMaestro()
        {
            PrevState = State;

            iCodRegistro = "null";
            InitFields();

            if (iCodMaestro != null)
            {
                SetHistoricState(HistoricState.Edicion);
                FillFields();
                DisableFields();
                CreateGridResumen();
                InitGridResumen();
            }
            else
            {
                iCodRegistro = "null";
                SetHistoricState(HistoricState.Inicio);
            }
        }

        protected override void InitFields()
        {
            pExpAtributos.ID = (pbDetalleLinea == true ? "AtribDetLinWrapper" : "AtribDetWrapper");
            pExpAtributos.StartOpen = true;
            pExpAtributos.CreateControls();
            pExpAtributos.Panel.Controls.Clear();
            pExpAtributos.Panel.Controls.Add(pTablaAtributos);

            pTablaAtributos.Controls.Clear();
            pTablaAtributos.ID = (pbDetalleLinea == true ? "AtributosDetLin" : "AtributosDet");
            pTablaAtributos.Width = Unit.Percentage(100);

            if (!String.IsNullOrEmpty(iCodEntidad) && !String.IsNullOrEmpty(iCodMaestro))
            {
                pFields = (HistoricFieldCollection)Activator.CreateInstanceFrom(Assembly.GetAssembly(typeof(HistoricFieldCollection)).CodeBase, CollectionClass).Unwrap();
                pFields.InitCollection(this, int.Parse(iCodEntidad), int.Parse(iCodMaestro), pTablaAtributos, this.ValidarPermiso);
                pFields.InitFields();
            }

        }

        protected override void InitRegistroResumen()
        {
            pExpResumen.ID = (pbDetalleLinea == true ? "DetLinWrapper" : "DetWrapper");
            pExpResumen.StartOpen = true;
            pExpResumen.CreateControls();
            pExpResumen.Panel.Controls.Clear();
            pExpResumen.Panel.Controls.Add(pTablaResumen);
        }

        protected override void InitGridResumen()
        {
            DSOGridClientColumn lCol;
            int lTarget = 0;

            pResumenGrid.ClearConfig();
            pResumenGrid.Config.sDom = "<\"H\"lf>tr<\"F\"pi>"; //con filtro global
            pResumenGrid.Config.bAutoWidth = true;
            pResumenGrid.Config.sScrollX = "100%";
            pResumenGrid.Config.sScrollY = "100%";
            pResumenGrid.Config.sPaginationType = "full_numbers";
            pResumenGrid.Config.bJQueryUI = true;
            pResumenGrid.Config.bProcessing = true;
            pResumenGrid.Config.bServerSide = true;
            pResumenGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerDetalle(this, sSource, aoData, fnCallback," + iCodEntidadEtq + "," + iCodMaestroResumen + ");}";
            pResumenGrid.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetEtiquetaDet");

            pFieldsResumen = new HistoricFieldCollection(int.Parse(iCodEntidadEtq), int.Parse(iCodMaestroResumen));

            if (pFieldsResumen != null)
            {
                foreach (KeytiaBaseField lField in pFieldsResumen)
                {
                    if (lField.ShowInGrid)
                    {
                        lCol = new DSOGridClientColumn();
                        lCol.sName = lField.Column;
                        lCol.aTargets.Add(lTarget++);
                        pResumenGrid.Config.aoColumnDefs.Add(lCol);
                    }
                }
                pResumenGrid.Fill();
            }

        }

        protected override void DisableFields()
        {
            pFields.DisableFields();

            //Ocultar Fields No necesarios en Datos.
            if (pFieldsNoVisibles == null)
            {
                return;
            }

            foreach (string lField in pFieldsNoVisibles)
            {
                if (pFields.GetByConfigName(lField.ToString()) != null)
                {
                    pTablaAtributos.Rows[pFields.GetByConfigName(lField.ToString()).DSOControlDB.Row - 1].Visible = false;
                    pFields.GetByConfigName(lField.ToString()).DSOControlDB.Visible = false;
                }
            }
        }

        protected override void InitLanguageEtiqueta()
        {
            pExpResumen.Title = Globals.GetMsgWeb("EtiquetaDataTitle");
            pExpResumen.ToolTip = Globals.GetMsgWeb("EtiquetaDataTitle");

            if (State == HistoricState.Edicion)
            {
                StringBuilder lsb = new StringBuilder();
                lsb.Length = 0;
                lsb.AppendLine("<script type='text/javascript'>");
                lsb.AppendLine("jQuery(function($){");
                lsb.AppendLine(pjsObj + ".iCodEmpleado = " + iCodEmpleado + ";");
                lsb.AppendLine("});");
                lsb.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), pjsObj + "Wd", lsb.ToString(), true, false);
            }

            if (pFieldsResumen != null)
            {
                InitLanguageGridResumen(pFieldsResumen, pResumenGrid);
            }
        }

        protected override void InitLanguageGridResumen(HistoricFieldCollection pFieldsGrid, DSOGrid pGridDP)
        {
            pFieldsGrid.InitLanguage();
            KeytiaBaseField lField;

            pGridDP.Config.oLanguage = Globals.GetGridLanguage();


            foreach (DSOGridClientColumn lCol in pGridDP.Config.aoColumnDefs)
            {
                if (pFieldsGrid.Contains(lCol.sName))
                {
                    lField = pFieldsGrid[lCol.sName];
                    if (lField.ConfigName == "TelDest")
                    {
                        lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "NomColNumeroEtq"));
                        lCol.bVisible = true;
                    }
                    else if (lField.ConfigName == "CostoFac")
                    {
                        lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "NomColConsumoEtq"));
                        lCol.bVisible = true;
                    }
                    else if (lField.ConfigName == "DuracionMin")
                    {
                        lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "NomColDuracionMinEtq"));
                        lCol.bVisible = true;
                    }
                    else if (lField.ConfigName == "Cantidad")
                    {
                        lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "NomColCantidadEtq"));
                        lCol.bVisible = true;
                    }
                    else if (lField.ConfigName == "FechaInicio")
                    {
                        lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "Fecha"));
                        lCol.bVisible = true;
                    }
                    else
                    {
                        lCol.sTitle = DSOControl.JScriptEncode(lField.Descripcion);
                        lCol.bVisible = true;
                    }
                }
                else if (lCol.sName == "vchDescripcion")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "vchDescripcion"));
                    lCol.bVisible = false;
                }
                else if (lCol.sName == "dtIniVigencia")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "dtIniVigencia"));
                    lCol.bVisible = false;
                }
                else if (lCol.sName == "dtFinVigencia")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "dtFinVigencia"));
                    lCol.bVisible = false;
                }
                else if (lCol.sName == "dtFecUltAct")
                {
                    lCol.sTitle = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "dtFecUltAct"));
                    lCol.bVisible = false;
                }

            }
        }

        protected override void CreateGridResumen()
        {
            pResumenGrid.ID = (pbDetalleLinea == true ? "DetLinGrid" : "DetGrid");
            pResumenGrid.CreateControls();
        }

        public override void SetHistoricState(HistoricState s)
        {
            pToolBar.Visible = false;
            pExpResumen.Visible = false;
            pResumenGrid.Visible = false;
            pExpFiltros.Visible = false;
            pExpAtributos.Visible = false;
            pExpRegistro.Visible = false;
            pHisGrid.Visible = false;

            if (s == HistoricState.Edicion)
            {
                pExpAtributos.Visible = true;
                pResumenGrid.Visible = true;
            }

            State = s;
        }

        protected override DataTable GetExportGrid(string lsTipoDoc)
        {
            DataTable ldt = null;

            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                if (State != HistoricState.Edicion || iCodEntidadEtq == null || iCodEntidadEtq == "" || iCodMaestroResumen == null || iCodMaestroResumen == "")
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                pFieldsResumen = new HistoricFieldCollection(int.Parse(iCodEntidadEtq), int.Parse(iCodMaestroResumen));

                KDBAccess lKDB = new KDBAccess();
                StringBuilder lsbSelectTop = new StringBuilder();
                StringBuilder lsbColumnas = new StringBuilder();
                StringBuilder lsbFrom = new StringBuilder();
                StringBuilder lsbWhere = new StringBuilder();
                KeytiaBaseField lField;
                string lsColumn = "";
                string lsConvToText = "";
                string lsSqlDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlDateFormat");
                string lsSqlTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlTimeFormat");

                DataTable ldtAtributos;
                if (DSODataContext.GetObject("Atrib") == null)
                {
                    ldtAtributos = lKDB.GetHisRegByEnt("Atrib", "Atributos", new string[] { "iCodCatalogo", "{Types}", "{Controles}" });
                    DSODataContext.SetObject("Atrib", ldtAtributos);
                }
                else
                {
                    ldtAtributos = (DataTable)DSODataContext.GetObject("Atrib");
                }
                int liCodIdioma = (int)ldtAtributos.Select("vchCodigo = '" + Globals.GetCurrentLanguage() + "'")[0]["iCodCatalogo"];

                if (lsTipoDoc != "WORD")
                {
                    lsConvToText = "'''' + ";
                }

                lsbSelectTop.AppendLine("select  ");

                ////////////////////////////////////////////////////////////////////////////
                foreach (DSOGridClientColumn lCol in pResumenGrid.Config.aoColumnDefs)
                {
                    if (pFieldsResumen.Contains(lCol.sName))
                    {
                        lField = pFieldsResumen[lCol.sName];
                        lCol.sClass = lField.ConfigName;

                        if (!lCol.bVisible)
                        {
                            continue;
                        }
                        if (lField.ConfigName == "TelDest")
                        {
                            lsbColumnas.AppendLine(lsConvToText + lCol.sName + " as '" + Globals.GetMsgWeb(false, "NomColNumeroEtq") + "',");
                        }
                        else if (lField.ConfigName == "CostoFac")
                        {
                            lsbColumnas.AppendLine(lCol.sName + " as '" + Globals.GetMsgWeb(false, "NomColConsumoEtq") + "',");
                        }
                        else if (lField.ConfigName == "DuracionMin")
                        {
                            lsbColumnas.AppendLine(lCol.sName + " as '" + Globals.GetMsgWeb(false, "NomColDuracionMinEtq") + "',");
                        }
                        else if (lField.ConfigName == "Cantidad")
                        {
                            lsbColumnas.AppendLine(lCol.sName + " as '" + Globals.GetMsgWeb(false, "NomColCantidadEtq") + "',");
                        }
                        else if (lField.ConfigName == "FechaInicio")
                        {
                            lsColumn = "convert(varchar, " + lCol.sName + ", " + lsSqlDateFormat + ")";
                            lsbColumnas.AppendLine(lsColumn + " as '" + Globals.GetMsgWeb(false, "Fecha") + "',");
                        }
                        else
                        {
                            lsColumn = lCol.sName;
                            if (lCol.sName.StartsWith("Date"))
                            {
                                lsColumn = "convert(varchar, " + lCol.sName + ", " + lsSqlTimeFormat + ")";
                            }
                            lsbColumnas.AppendLine(lsColumn + " as '" + lCol.sTitle + "',");
                        }
                    }
                }
                //////////////////////////////////////////////////////////////////////////7

                lsbFrom.AppendLine("      from [" + DSODataContext.Schema + "].GetEtiquetaDetPer(" + iCodEmpleado + "," + liCodIdioma + ",'" + base.IniPeriodo.ToString("yyyy-MM-dd") + "'").ToString();
                lsbFrom.AppendLine("                              ,'" + base.FinPeriodo.ToString("yyyy-MM-dd") + "') ");
                if (psLinea.Length > 0)
                {
                    lsbWhere.Append("where vchDescripcion ='" + psLinea + "' ");
                }

                string lsSelectTop = lsbSelectTop.ToString();
                string lsColumnas = lsbColumnas.ToString().Remove(lsbColumnas.Length - 3, 1); //remueve última coma
                string lsFromGetDetalle = lsbFrom.ToString();
                string lsWhere = lsbWhere.ToString();
                ldt = DSODataAccess.Execute(lsSelectTop + lsColumnas + lsFromGetDetalle + lsWhere);

                return ldt;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }

        }

        public static DataTable GetTDest()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT iCodCatalogo, vchCodigo, vchDescripcion");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('TDest','Tipo de Destino','Español')]");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine("  AND (vchCodigo = 'Local'");
            query.AppendLine("      OR vchCodigo = 'LDNac'");
            query.AppendLine("      OR vchCodigo = 'Enl')");
            return DSODataAccess.Execute(query.ToString());
        }

        #region WebMethods
        public static DSOGridServerResponse GetEtiquetaDet(DSOGridServerRequest gsRequest, int liCodEmpleado, int liCodMaestro, int liCodEntidad, string lsLinea, DateTime ldtIniPeriodo, DateTime ldtFinPeriodo)
        {
            if (HttpContext.Current.Session["iCodUsuario"] == null)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                throw new KeytiaWebSessionException(true, lsTitulo);
            }

            try
            {
                KDBAccess lKDB = new KDBAccess();
                HistoricFieldCollection lFields = new HistoricFieldCollection(liCodEntidad, liCodMaestro);
                StringBuilder lsbSelectTop = new StringBuilder();
                StringBuilder lsbColumnas = new StringBuilder();
                StringBuilder lsbFrom = new StringBuilder();
                StringBuilder lsbWhere = new StringBuilder();
                StringBuilder lsbOrderBy = new StringBuilder();
                DateTime ldtVigencia = DateTime.Now;
                //NZ 20170425 
                DataTable dtTDest = new DataTable();                
                dtTDest = GetTDest();

                DSOGridServerResponse lgsrRet = new DSOGridServerResponse();
                DataTable ldt;
                DataTable ldtAtributos;
                if (DSODataContext.GetObject("Atrib") == null)
                {
                    ldtAtributos = lKDB.GetHisRegByEnt("Atrib", "Atributos", new string[] { "iCodCatalogo", "{Types}", "{Controles}" });
                    DSODataContext.SetObject("Atrib", ldtAtributos);
                }
                else
                {
                    ldtAtributos = (DataTable)DSODataContext.GetObject("Atrib");
                }

                int liCodIdioma = (int)ldtAtributos.Select("vchCodigo = '" + Globals.GetCurrentLanguage() + "'")[0]["iCodCatalogo"];

                //formatos de conversion de fechas de sql
                string lsSqlDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlDateFormat");
                string lsSqlTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "SqlTimeFormat");

                string lsOrderCol = "";
                string lsOrderDir = "";
                string lsOrderDirInv = "";
                if (gsRequest.iSortCol.Count > 0)
                {
                    lsOrderCol = gsRequest.sColumns.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[gsRequest.iSortCol[0]];

                    if (!lFields.Contains(lsOrderCol))
                    {
                        lsOrderCol = "vchDescripcion";
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
                    lsOrderCol = "vchDescripcion";
                    lsOrderDir = " asc";
                    lsOrderDirInv = " desc";
                }

                lsbSelectTop.AppendLine("select * from (");
                lsbSelectTop.AppendLine("   select top " + gsRequest.iDisplayLength + " * from (");
                lsbSelectTop.AppendLine("       select top " + (gsRequest.iDisplayStart + gsRequest.iDisplayLength));

                ////////////////////////////////////////////////////////////////////////////
                lgsrRet.sColumns = "iCodRegistro";
                lsbColumnas.AppendLine("iCodRegistro");

                lgsrRet.sColumns += ",vchDescripcion";
                lsbColumnas.AppendLine(",vchDescripcion");

                foreach (KeytiaBaseField lField in lFields)
                {
                    lgsrRet.sColumns += "," + lField.Column;
                    if (lField.Column.StartsWith("Date01"))
                    {
                        lsbColumnas.AppendLine(", Date01 = convert(varchar, a." + lField.Column + ", " + lsSqlDateFormat + ")");
                    }
                    else if (lField.Column.StartsWith("Date02"))
                    {
                        lsbColumnas.AppendLine(", Date02 = convert(varchar, a." + lField.Column + ", " + lsSqlTimeFormat + ")");
                    }
                    else
                    {
                        lsbColumnas.AppendLine("," + lField.Column);
                    }
                }
                lgsrRet.sColumns += ",dtIniVigencia";
                lsbColumnas.AppendLine(",dtIniVigencia");

                lgsrRet.sColumns += ",dtFinVigencia";
                lsbColumnas.AppendLine(",dtFinVigencia");

                //////////////////////////////////////////////////////////////////////////7
                lsbFrom.AppendLine("      from " + DSODataContext.Schema + ".GetEtiquetaDetPer(" + liCodEmpleado + "," + liCodIdioma + ",'" + ldtIniPeriodo.ToString("yyyy-MM-dd") + "'");
                lsbFrom.AppendLine("                              ,'" + ldtFinPeriodo.ToString("yyyy-MM-dd") + "') ");

                lsbOrderBy.AppendLine("       order by " + lsOrderCol + lsOrderDir);
                lsbOrderBy.AppendLine("   ) as a order by " + lsOrderCol + lsOrderDirInv);
                lsbOrderBy.AppendLine(") as a order by " + lsOrderCol + lsOrderDir);

                bool lbPrimero = true;

                string lsQueryLinea = "";

                //NZ 20170425 Se excluyen las llamadas de enlace para todos los esquemas. Por lo que se inicializa la variable de Where con esta condicion
                dtTDest = GetTDest();
                lsbWhere.AppendLine(" WHERE TDest <> " + dtTDest.AsEnumerable().First(x => x.Field<string>("vchCodigo") == "Enl")["iCodCatalogo"].ToString());

                //NZ
                if (!pbEtiquetarIncluirLlamadasEntrada) //Si esta bandera No esta prendida...se excluyen llamadas de entrada.
                {
                    lsbWhere.Append(" AND TpLlam <> 'Entrada'");
                }
                //NZ 20170425
                if (DSODataContext.Schema.ToLower() == "cide") //Prueba evox
                {
                    lsbWhere.AppendLine(" AND TDest <> " + dtTDest.AsEnumerable().First(x => x.Field<string>("vchCodigo") == "Local")["iCodCatalogo"].ToString());
                    lsbWhere.AppendLine(" AND TDest <> " + dtTDest.AsEnumerable().First(x => x.Field<string>("vchCodigo") == "LDNac")["iCodCatalogo"].ToString());
                }

                if (!String.IsNullOrEmpty(lsLinea))
                {
                    lsbWhere.Append(" and vchDescripcion ='" + lsLinea + "' ");
                    ////if (!pbEtiquetarIncluirLlamadasEntrada) //Checa la bandera para ver si ya se agrego el filtro Where del Tipo de llamada antes.
                    ////{
                    ////    lsbWhere.Append(" and vchDescripcion ='" + lsLinea + "' ");
                    ////}
                    //////else { lsbWhere.Append("where vchDescripcion ='" + lsLinea + "' "); }

                    lsQueryLinea = lsbWhere + "Group By vchDescripcion";
                    lbPrimero = false;
                }
                else { lsQueryLinea = lsbWhere + ""; }

                string[] lsColumns = gsRequest.sColumns.Split(',');
                if (gsRequest.sSearchGlobal.Trim() != "")
                {
                    string lsTerm = gsRequest.sSearchGlobal.Replace("'", "''").Trim();
                    StringBuilder lsbColTodas = new StringBuilder();
                    lsbColTodas.AppendLine("isnull(a.vchDescripcion,'')");

                    foreach (KeytiaBaseField lField in lFields)
                    {
                        if (lField.Column.StartsWith("Date"))
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar,a." + lField.Column + "," + lsSqlDateFormat + "),'') + ' ' + isnull(convert(varchar,a." + lField.Column + "," + lsSqlTimeFormat + "),'')");
                        }
                        else if (!(lField.Column.StartsWith("VarChar"))
                            || lField.Column.StartsWith("iCodCatalogo"))
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(convert(varchar," + lField.Column + "),'')");
                        }
                        else
                        {
                            lsbColTodas.AppendLine("+ ' ' + isnull(" + lField.Column + ",'')");
                        }
                    }
                    if (lbPrimero)
                    {
                        lsbWhere.Append("where ");
                        lbPrimero = false;
                    }
                    else
                    {
                        lsbWhere.Append("and ");
                    }
                    lsbWhere.AppendLine(lsbColTodas.ToString() + " like '%" + lsTerm + "%'");
                    lbPrimero = false;
                }

                int lidx;
                for (lidx = 0; lidx < lsColumns.Length; lidx++)
                {
                    if (gsRequest.bSearchable[lidx])
                    {
                        string lsColumn = lsColumns[lidx];
                        string lsFiltro = gsRequest.sSearch[lidx].Replace("'", "''").Trim();

                        if (!String.IsNullOrEmpty(lsFiltro)
                            && (lFields.Contains(lsColumn)
                            || lsColumn == "vchDescripcion"
                            || lsColumn == "dtIniVigencia"
                            || lsColumn == "dtFinVigencia"))
                        {
                            if (lbPrimero)
                            {
                                lsbWhere.Append("where ");
                                lbPrimero = false;
                            }
                            else
                            {
                                lsbWhere.Append("and ");
                            }

                            if (lsColumn.StartsWith("Date")
                                || lsColumn == "dtIniVigencia"
                                || lsColumn == "dtFinVigencia")
                            {

                                lsbWhere.Append("(");
                                lsbWhere.AppendLine("convert(varchar, a." + lsColumn + ", " + lsSqlDateFormat + ") like '%" + lsFiltro + "%'");
                                lsbWhere.AppendLine("or convert(varchar, a." + lsColumn + ", " + lsSqlTimeFormat + ") like '%" + lsFiltro + "%'");
                                lsbWhere.AppendLine(")");
                            }
                            else
                            {
                                lsbWhere.AppendLine("a." + lsColumn + " like '%" + lsFiltro + "%'");
                            }
                        }
                    }
                }

                string lsSelectCount = "select count(vchDescripcion) ";
                string lsSelectTop = lsbSelectTop.ToString();
                string lsColumnas = lsbColumnas.ToString();
                string lsFrom = lsbFrom.ToString();
                string lsFromGetResumen = "from " + DSODataContext.Schema + ".GetEtiquetaDetPer(" + liCodEmpleado + "," + liCodIdioma + ",'" + ldtIniPeriodo.ToString("yyyy-MM-dd") + "','" + ldtFinPeriodo.ToString("yyyy-MM-dd") + "') a \r\n";
                string lsWhere = lsbWhere.ToString();
                string lsOrderBy = lsbOrderBy.ToString();

                lgsrRet.sEcho = gsRequest.sEcho;

                lgsrRet.iTotalRecords = (int)DSODataAccess.ExecuteScalar("select IsNull((" + lsSelectCount + lsFrom + lsQueryLinea + "),0)");

                if (!String.IsNullOrEmpty(lsWhere))
                {
                    lgsrRet.iTotalDisplayRecords = (int)DSODataAccess.ExecuteScalar(lsSelectCount + lsFromGetResumen + lsWhere);
                }
                else
                {
                    lgsrRet.iTotalDisplayRecords = lgsrRet.iTotalRecords;
                }

                ldt = DSODataAccess.Execute(lsSelectTop + lsColumnas + lsFromGetResumen + lsWhere + lsOrderBy);

                lgsrRet.ProcesarDatos(gsRequest, ldt);

                if (lgsrRet.iTotalDisplayRecords > 0 && ldt.Rows.Count == 0)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                    throw new KeytiaWebException(true, "ErrGridData", null, lsTitulo);
                }

                lFields.FormatGridData(lgsrRet, ldt);
                return lgsrRet;
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                throw new KeytiaWebException(true, "ErrGridData", e, lsTitulo);
            }
        }
        #endregion

    }
}

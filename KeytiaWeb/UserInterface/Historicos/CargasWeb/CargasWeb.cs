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


namespace KeytiaWeb.UserInterface
{
    public class CargasWeb: CargasWebEdit
    {
        protected DSOExpandable pExpGrid;
        protected Table pTablaDetalle;
        protected DSOGrid pDetaGrid;
        protected DSODropDownList piCodMaestroDeta;
        protected HistoricFieldCollection pFieldsDeta;
        protected DSOExpandable pExpDetaFiltros;
        protected Table pTblDetaFiltros;
        protected HtmlButton pbtnDetaFiltro;
        protected HtmlButton pbtnLimpiaDetaFiltro;
        protected Hashtable phtDetaFiltros;

        protected HtmlButton pbtnExpArchDeta;

        protected virtual string iCodMaestroDeta
        {
            get
            {
                return (string)ViewState["iCodMaestroDeta"];
            }
            set
            {
                ViewState["iCodMaestroDeta"] = value;
            }
        }

        public CargasWeb()
        {
            Init += new EventHandler(CargasWeb_Init);
        }

        protected virtual void CargasWeb_Init(object sender, EventArgs e)
        {
            // Controles para la seccion de Detallados
            pDetaGrid = new DSOGrid();
            pExpGrid = new DSOExpandable();
            pTablaDetalle = new Table();
            pExpDetaFiltros = new DSOExpandable();
            pTblDetaFiltros = new Table();
            pbtnDetaFiltro = new HtmlButton();
            pbtnLimpiaDetaFiltro = new HtmlButton();
            pbtnExpArchDeta = new HtmlButton();

            Controls.Add(pDetaGrid);
            pToolBar.Controls.Add(pbtnExpArchDeta);

        }

        protected override void InitAccionesSecundarias()
        {
            base.InitAccionesSecundarias();
            pbtnExpArchDeta.Attributes["class"] = "buttonXLS";
            pbtnExpArchDeta.Style["display"] = "none";

            pbtnExpArchDeta.ServerClick += new EventHandler(pbtnExpArchDeta_ServerClick);

            InitRegistroDeta();
        }
        
        public override void InitLanguage()
        {
            base.InitLanguage();

            pbtnExpArchDeta.InnerText = Globals.GetMsgWeb("btnExpDeta");

            piCodMaestroDeta.Descripcion = Globals.GetMsgWeb(false, "Maestro");
            pExpGrid.Title = Globals.GetMsgWeb("CargasDataTitle");
            pExpGrid.ToolTip = Globals.GetMsgWeb("CargasDataTitle");
            pDetaGrid.Config.oLanguage = Globals.GetGridLanguage();
            pbtnDetaFiltro.InnerText = Globals.GetMsgWeb("btnFiltro");
            pbtnLimpiaDetaFiltro.InnerText = Globals.GetMsgWeb("btnLimpiarFiltro");
            pExpDetaFiltros.Title = Globals.GetMsgWeb("HistoricFilterTitle");
            pExpDetaFiltros.ToolTip = Globals.GetMsgWeb("HistoricFilterTitle");

            if (pFieldsDeta != null)
            {
                InitLanguageGrid(pFieldsDeta, pDetaGrid,phtDetaFiltros,piCodMaestroDeta.ToString());
            }

        }

        protected virtual void InitRegistroDeta()
        {
            pExpGrid.ID = "GridDetaWrapper";
            pExpGrid.StartOpen = true;
            pExpGrid.CreateControls();
            pExpGrid.Panel.Controls.Clear();
            pExpGrid.Panel.Controls.Add(pTablaDetalle);
            pExpGrid.Panel.Controls.Add(pExpDetaFiltros);
            pExpGrid.OnOpen = "function(){" + pjsObj + ".fnInitGrids();}";

            pTablaDetalle.Controls.Clear();
            pTablaDetalle.ID = "TablaDetalle";
            pTablaDetalle.Width = Unit.Percentage(100);

            pDetaGrid.ID = "DetaGrid";
            pDetaGrid.Wrapper = pExpGrid;
            pDetaGrid.CreateControls();

            int liRow = 1;

            piCodMaestroDeta = new DSODropDownList();
            piCodMaestroDeta.ID = "iCodMaestroDeta";
            piCodMaestroDeta.Table = pTablaDetalle;
            piCodMaestroDeta.Row = liRow++;
            piCodMaestroDeta.DataField = "iCodMaestroDeta";
            piCodMaestroDeta.SelectItemValue = "";
            piCodMaestroDeta.ColumnSpan = 3;
            piCodMaestroDeta.CreateControls();
            piCodMaestroDeta.AutoPostBack = true;
            piCodMaestroDeta.DropDownListChange += new EventHandler(piCodMaestroDeta_SelectedIndexChanged);

            pExpDetaFiltros.ID = "DetaFiltrosWrapper";
            pExpDetaFiltros.StartOpen = false;
            pExpDetaFiltros.CreateControls();
            pExpDetaFiltros.Panel.Controls.Clear();

            pExpDetaFiltros.Panel.Controls.Add(pTblDetaFiltros);
            pExpDetaFiltros.Panel.Controls.Add(pbtnDetaFiltro);
            pExpDetaFiltros.Panel.Controls.Add(pbtnLimpiaDetaFiltro);

            pbtnDetaFiltro.ID = "btnDetaFiltro";
            pbtnLimpiaDetaFiltro.ID = "btnLimpiaDetaFiltro";

            pbtnDetaFiltro.Attributes["class"] = "buttonSearch";
            pbtnLimpiaDetaFiltro.Attributes["class"] = "buttonCancel";

            pbtnPendFiltro.Style["display"] = "none";
            pbtnLimpiaPendFiltro.Style["display"] = "none";

            pbtnDetaFiltro.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = pjsObj + ".filtrarDeta();return false;";
            pbtnLimpiaDetaFiltro.Attributes[HtmlTextWriterAttribute.Onclick.ToString()] = pjsObj + ".limpiarFiltroDeta();return false;";

        }

        protected override void ConsultarDetalles()
        {
            base.ConsultarDetalles();
            if (iCodMaestroDeta != null)
            {
                InitGridDeta();
                InitDetaFiltros();
                ClearFiltros(phtDetaFiltros);
            }
            else
            {
                pDetaGrid.Grid.Visible = false;
            }
        }
        
        protected virtual void InitGridDeta()
        {
            DSOGridClientColumn lCol;
            int lTarget = 0;

            pDetaGrid.ClearConfig();
            pDetaGrid.Config.sDom = "<\"H\"Rlf>tr<\"F\"pi>"; //con filtro global
            pDetaGrid.Config.bAutoWidth = true;
            pDetaGrid.Config.sScrollX = "100%";
            pDetaGrid.Config.sScrollY = "400px";
            pDetaGrid.Config.sPaginationType = "full_numbers";
            pDetaGrid.Config.bJQueryUI = true;
            pDetaGrid.Config.bProcessing = true;
            pDetaGrid.Config.bServerSide = true;
            pDetaGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerDetalle(this, sSource, aoData, fnCallback);}";
            pDetaGrid.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetHisDetallados");

            if (iCodMaestroDeta != null)
            {            
                pFieldsDeta = new HistoricFieldCollection(int.Parse(iCodCarga), int.Parse(iCodMaestroDeta));
            }
            else
            {
                pFieldsDeta = null;
            }

            if (pFieldsDeta != null)
            {
                lCol = new DSOGridClientColumn();
                lCol.sName = "iCodRegistro";
                lCol.aTargets.Add(lTarget++);
                lCol.bVisible = false;
                pDetaGrid.Config.aoColumnDefs.Add(lCol);

                foreach (KeytiaBaseField lField in pFieldsDeta)
                {
                    if (lField.ShowInGrid)
                    {
                        lCol = new DSOGridClientColumn();
                        lCol.sName = lField.Column;
                        lCol.aTargets.Add(lTarget++);
                        pDetaGrid.Config.aoColumnDefs.Add(lCol);
                    }
                }

                lCol = new DSOGridClientColumn();
                lCol.sName = "dtFecha";
                lCol.bVisible = false;
                lCol.aTargets.Add(lTarget++);
                lCol.sWidth = "120px";
                pDetaGrid.Config.aoColumnDefs.Add(lCol);

                lCol = new DSOGridClientColumn();
                lCol.sName = "dtFecUltAct";
                lCol.aTargets.Add(lTarget++);
                pDetaGrid.Config.aoColumnDefs.Add(lCol);

                if (pDetaGrid.Config.aoColumnDefs.Count > 10)
                {
                    pDetaGrid.Config.sScrollXInner = (pDetaGrid.Config.aoColumnDefs.Count * 20).ToString() + "%";
                }

                pDetaGrid.Grid.Visible = true;
                pDetaGrid.Fill();
            }
        }

        protected virtual void InitDetaFiltros()
        {
            pTblDetaFiltros.Controls.Clear();
            pTblDetaFiltros.ID = "DetaFiltros";
            pTblDetaFiltros.Width = Unit.Percentage(100);

            DSOTextBox lDSOtxt;
            if (pFieldsDeta != null)
            {
                phtDetaFiltros = new Hashtable();

                foreach (KeytiaBaseField lField in pFieldsDeta)
                {
                    if (lField.ShowInGrid)
                    {
                        lDSOtxt = new DSOTextBox();
                        lDSOtxt.ID = lField.Column;
                        lDSOtxt.AddClientEvent("dataFilterDeta", lField.Column);
                        lDSOtxt.Row = lField.Row;
                        lDSOtxt.ColumnSpan = lField.ColumnSpan;
                        lDSOtxt.Table = pTblDetaFiltros;
                        lDSOtxt.CreateControls();

                        phtDetaFiltros.Add(lDSOtxt.ID, lDSOtxt);
                    }
                }
            }
        }
       
        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);

            pDetaGrid.Visible = false;
            pExpDetaFiltros.Visible = false;
            pbtnExpArchDeta.Visible = false;

            if (s == HistoricState.Consulta)
            {
                pDetaGrid.Visible = true;
                pExpDetaFiltros.Visible = true;
                pbtnExpArchDeta.Visible = true;

            }

            State = s;
        }

        protected virtual void piCodMaestroDeta_SelectedIndexChanged(object sender, EventArgs e)
        {
            iCodMaestroDeta = piCodMaestroDeta.HasValue ? piCodMaestroDeta.DataValue.ToString() : null;

            if (iCodRegistro != null)
            {
                SetHistoricState(HistoricState.Consulta);
                if (iCodMaestroDeta != null)
                {
                    pDetaGrid.TxtState.Text = "";
                    InitGridDeta();
                    InitDetaFiltros();
                    ClearFiltros(phtDetaFiltros);
                    pbtnExpArchDeta.Visible = true;
                }
            }
            else
            {
                SetHistoricState(HistoricState.Inicio);
            }
        }

        protected override void IniMaestrosCargas()
        {
            base.IniMaestrosCargas();

            if (iCodRegistro != "null")
            {
                psbQuery.Length = 0;
                psbQuery.AppendLine("Select Top 1 convert(varchar(20),iCodRegistro) from Maestros ");
                psbQuery.AppendLine("Where iCodRegistro in (Select iCodMaestro from Detallados ");
                psbQuery.AppendLine("                       Where iCodCatalogo = " + iCodCarga.ToString() + ") ");
                psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia ");
                psbQuery.AppendLine("Order by vchDescripcion");

                iCodMaestroDeta = (string)DSODataAccess.ExecuteScalar(psbQuery.ToString());

                psbQuery.Length = 0;
                psbQuery.AppendLine("Select iCodRegistro, vchDescripcion from Maestros ");
                psbQuery.AppendLine("Where iCodRegistro in (Select iCodMaestro from Detallados ");
                psbQuery.AppendLine("                       Where iCodCatalogo = " + iCodCarga.ToString() + ") ");
                psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia ");
                psbQuery.AppendLine("Order by vchDescripcion ");

                piCodMaestroDeta.DataSource = psbQuery.ToString();
                piCodMaestroDeta.Fill();
           }
        }
        
        protected virtual void pbtnExpArchDeta_ServerClick(object sender, EventArgs e)
        {
            PrevState = State;
            if (iCodMaestroDeta != null)
            {
                ExportXLSDetllados(0, int.Parse(iCodCarga), int.Parse(iCodMaestroDeta));
            }
            FirePostConsultarClick();
        }

    }
}

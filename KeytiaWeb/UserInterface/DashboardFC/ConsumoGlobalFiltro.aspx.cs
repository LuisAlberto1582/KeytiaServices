using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using KeytiaWeb.UserInterface.DashboardLT;
using AjaxControlToolkit;
using DSOControls2008;
using System.ComponentModel;
using System.Globalization;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    public partial class ConsumoGlobalFiltro : System.Web.UI.Page
    {
        Dictionary<string, List<Catalogo>> datosFiltros = new Dictionary<string, List<Catalogo>>();
        List<BDDemo> bd;

        public void Page_PreInit(object o, EventArgs e)
        {
            EnsureChildControls();
            Page.ClientScript.GetPostBackEventReference(this, "");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
                ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
                #endregion

                TablasFiltros();
                ControlesFiltros();
                //if (!ScriptManager.GetCurrent(this.Page).IsInAsyncPostBack)
                //{ 
                //    ControlesFiltros(); 
                //}

            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            RecuperarFitros();
            Reporte();
        }


        private void RecuperarFitros()
        {
            int numFiltros = 0;
            bool seEncontro = true;
            CheckBoxList checkList;
            Control myControl;
            List<Catalogo> lista;

            do
            {
                myControl = Rep1.FindControl("chkList" + numFiltros.ToString());
                if (myControl != null)
                {
                    checkList = myControl as CheckBoxList;
                    List<string> selectedValues = checkList.Items.Cast<ListItem>().Where(li => li.Selected).Select(li => li.Value).ToList();

                    if (datosFiltros.TryGetValue(checkList.ToolTip, out lista))
                    {
                        datosFiltros[checkList.ToolTip].Where(x => selectedValues.Any(w => w == x.Id.ToString()))
                                                       .ToList().ForEach(x => x.Filtro = true);

                        seEncontro = true;
                    }
                    else { seEncontro = true; }
                }
                else { seEncontro = false; }

                numFiltros += 1;
            } while (seEncontro);
        }

        private void TablasFiltros()
        {
            datosFiltros.Add("Compañía", new
                List<Catalogo>() 
                {
                    new Catalogo() { Id = 1, Nombre = "Unifar" },
                    new Catalogo() { Id = 2, Nombre = "Pharma" },
                    new Catalogo() { Id = 3, Nombre = "Rama" }
                });

            datosFiltros.Add("Dirección", new List<Catalogo>() 
            {
                new Catalogo() { Id = 1, Nombre = "Operaciones" },
                new Catalogo() { Id = 2, Nombre = "Desarrollo Humano" },
                new Catalogo() { Id = 3, Nombre = "Finanzas" },
                new Catalogo() { Id = 4, Nombre = "Comercial" },
                new Catalogo() { Id = 5, Nombre = "Dir. General" },
                new Catalogo() { Id = 6, Nombre = "IT" },
                new Catalogo() { Id = 7, Nombre = "Marketing" },
                new Catalogo() { Id = 8, Nombre = "Compras" }
            });

            datosFiltros.Add("Concepto", new List<Catalogo>() 
            {
                new Catalogo() { Id = 1, Nombre = "Telefonía fija" },
                new Catalogo() { Id = 2, Nombre = "Telefonía móvil" },
                new Catalogo() { Id = 3, Nombre = "Enlaces" },
                new Catalogo() { Id = 4, Nombre = "Impresoras" },
                new Catalogo() { Id = 5, Nombre = "Camaras" },
            });


        }

        private void ControlesFiltros()
        {
            Rep1.Controls.Add(TituloYBordesFiltros(Filtros(585, datosFiltros), "Aplicar Filtro", 0, "ActualizarRep();return false", "btnFiltros", this));
        }

        private Control Filtros(int heightPanel, Dictionary<string, List<Catalogo>> datosFiltros)
        {
            Panel panelPrincipal = new Panel();
            panelPrincipal.ID = "PanelFiltros";
            panelPrincipal.Height = heightPanel;
            panelPrincipal.Width = Unit.Percentage(100);
            panelPrincipal.CssClass = "maxWidth100Perc";
            panelPrincipal.ScrollBars = ScrollBars.Vertical;
            panelPrincipal.ScrollBars = ScrollBars.Horizontal;

            for (int item = 0; item < datosFiltros.Keys.Count; item++)
            {
                Panel panelTitulo = new Panel();
                panelTitulo.CssClass = "headerConsumoGlobalFiltro";
                panelTitulo.Width = Unit.Percentage(100);

                Table tableTitle = new Table();
                panelTitulo.ID = "pntTitulo" + item;
                tableTitle.Width = Unit.Percentage(100);

                TableRow tRow = new TableRow();
                tableTitle.ID = "table" + item;
                tRow.Width = Unit.Percentage(100);

                TableCell tCell1 = new TableCell() { HorizontalAlign = HorizontalAlign.Left };
                tCell1.Width = Unit.Percentage(100);
                tCell1.Controls.Add(new Label() { CssClass = "titleConsumoGlobalFiltro", Text = datosFiltros.ElementAt(item).Key });
                tRow.Cells.Add(tCell1);

                TableCell tCell2 = new TableCell() { HorizontalAlign = HorizontalAlign.Right };
                tCell2.Width = Unit.Percentage(100);
                Image imgExpand = new Image() { ImageAlign = ImageAlign.Middle, ID = "imgExpandCollapse" + item };
                tCell2.Controls.Add(imgExpand);
                tRow.Cells.Add(tCell2);

                tableTitle.Rows.Add(tRow);
                panelTitulo.Controls.Add(tableTitle);

                Panel panelContent = new Panel();
                panelContent.ID = "pnlCont" + item;
                panelContent.Width = Unit.Percentage(100);

                CheckBoxList chkList = new CheckBoxList();
                chkList.ID = "chkList" + item;
                chkList.CssClass = "alineacionFiltros";
                chkList.Width = Unit.Percentage(100);
                chkList.ToolTip = datosFiltros.ElementAt(item).Key;
                chkList.DataTextField = "Nombre";
                chkList.DataValueField = "Id";
                chkList.DataSource = datosFiltros.ElementAt(item).Value;
                chkList.DataBind();
                panelContent.Controls.Add(chkList);

                CollapsiblePanelExtender collapsiblePanelExtender = new CollapsiblePanelExtender();
                collapsiblePanelExtender.ID = "collapsibleExtender" + item;
                collapsiblePanelExtender.TargetControlID = panelContent.ID;
                collapsiblePanelExtender.ExpandControlID = panelTitulo.ID;
                collapsiblePanelExtender.CollapseControlID = panelTitulo.ID;
                collapsiblePanelExtender.Collapsed = false;
                collapsiblePanelExtender.ExpandDirection = CollapsiblePanelExpandDirection.Vertical;
                //--collapsiblePanelExtender.SuppressPostBack = true;
                collapsiblePanelExtender.CollapsedText = "Mostrar...";
                collapsiblePanelExtender.ExpandedText = "Ocultar";
                collapsiblePanelExtender.ImageControlID = imgExpand.ID;
                collapsiblePanelExtender.ExpandedImage = "~/images/up-arrow-square-blue.png";
                collapsiblePanelExtender.CollapsedImage = "~/images/down-arrow-square-blue.png";

                panelPrincipal.Controls.Add(panelTitulo);
                panelPrincipal.Controls.Add(panelContent);
                panelPrincipal.Controls.Add(collapsiblePanelExtender);
            }
            return panelPrincipal;
        }

        private Control TituloYBordesFiltros(Control control, string tituloReporte, int height,
            string nameFuncion, string nameControl, object target)
        {
            Panel panel = new Panel();
            panel.HorizontalAlign = HorizontalAlign.Center;
            if (height > 0)
            {
                panel.CssClass = "PanelTitulosYBordeReportes maxWidth100Perc";
                panel.Height = height;
            }
            else { panel.CssClass = "PanelTitulosYBordeReportes AutoHeight maxWidth100Perc"; }

            Panel header = new Panel();
            header.CssClass = "titulosReportes";
            header.HorizontalAlign = HorizontalAlign.Left;

            header.Height = 5;

            Table tblTituloReporte = new Table();
            tblTituloReporte.Width = Unit.Percentage(100);
            TableRow tblTituloReporteTr1 = new TableRow();
            TableCell tblTituloReporteTc1 = new TableCell();
            tblTituloReporteTc1.Width = Unit.Percentage(95);
            tblTituloReporteTc1.HorizontalAlign = HorizontalAlign.Left;
            TableCell tblTituloReporteTc2 = new TableCell();
            tblTituloReporteTc2.Width = Unit.Percentage(5);
            tblTituloReporteTc2.HorizontalAlign = HorizontalAlign.Center;

            Label lblTitulo = new Label();
            lblTitulo.Text = tituloReporte;
            lblTitulo.CssClass = "tituloHeaderReportes";
            lblTitulo.Width = Unit.Percentage(100);

            tblTituloReporteTc1.Controls.Add(lblTitulo);

            Button btnNav = new Button();
            btnNav.ID = nameControl;
            btnNav.OnClientClick = nameFuncion;
            btnNav.Width = 28;
            btnNav.Height = 28;
            lblTitulo.Width = Unit.Percentage(100);
            btnNav.CssClass = "ui-button-icon-primary ui-icon custom-icon-search ui-widget ui-state-default ui-corner-all AutoHeight noRepeat";

            tblTituloReporteTc2.Controls.Add(btnNav);

            tblTituloReporteTr1.Controls.Add(tblTituloReporteTc1);
            tblTituloReporteTr1.Controls.Add(tblTituloReporteTc2);
            tblTituloReporte.Controls.Add(tblTituloReporteTr1);

            panel.Controls.Add(tblTituloReporte);
            panel.Controls.Add(header);
            panel.Controls.Add(control);

            return panel;
        }


        private DataTable FiltrarInfo()
        {
            DataTable dt = new DataTable();
            bool hayFiltro = false;

            for (int i = 0; i < datosFiltros.Keys.Count; i++)
            {
                if (datosFiltros.ElementAt(i).Value.Where(x => x.Filtro).Count() > 0)
                {
                    hayFiltro = true;
                    break;
                }
            }

            if (hayFiltro)
            {
                if (datosFiltros.ElementAt(0).Value.Count(w => w.Filtro) > 0)
                {
                    bd = bd.Where(x => datosFiltros.ElementAt(0).Value.Any(w => w.Id == x.IdCompañia && w.Filtro)).ToList();
                }
                if (datosFiltros.ElementAt(1).Value.Count(w => w.Filtro) > 0)
                {
                    bd = bd.Where(x => datosFiltros.ElementAt(1).Value.Any(w => w.Id == x.IdDireccion && w.Filtro)).ToList();
                }
                if (datosFiltros.ElementAt(2).Value.Count(w => w.Filtro) > 0)
                {
                    bd = bd.Where(x => datosFiltros.ElementAt(2).Value.Any(w => w.Id == x.IdConcepto && w.Filtro)).ToList();
                }
            }
            dt = ConvertToDataTable(bd);

            return dt;
        }

        public DataTable ConvertToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
            {
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;

        }

        private void Reporte()
        {
            BDEjemplo();
            DataTable dt = FiltrarInfo();

            if (dt.Rows.Count > 0 && dt.Columns.Count > 0)
            {
                DataView dvldt = new DataView(dt);
                dt = dvldt.ToTable(false, new string[] { "NombreCompañia", "NombreDireccion", "NombreConcepto", 
                    "Ene","Feb","Mar","Abr","May","Jun","Jul","Ago","Sep","Oct","Nov","Dic" });

                dt.Columns["NombreCompañia"].ColumnName = "Compañía";
                dt.Columns["NombreDireccion"].ColumnName = "Dirección";
                dt.Columns["NombreConcepto"].ColumnName = "Concepto";
                dt.Columns["Ene"].ColumnName = "ENE";
                dt.Columns["Feb"].ColumnName = "FEB";
                dt.Columns["Mar"].ColumnName = "MAR";
                dt.Columns["Abr"].ColumnName = "ABR";
                dt.Columns["May"].ColumnName = "MAY";
                dt.Columns["Jun"].ColumnName = "JUN";
                dt.Columns["Jul"].ColumnName = "JUL";
                dt.Columns["Ago"].ColumnName = "AGO";
                dt.Columns["Sep"].ColumnName = "SEP";
                dt.Columns["Oct"].ColumnName = "OCT";
                dt.Columns["Nov"].ColumnName = "NOV";
                dt.Columns["Dic"].ColumnName = "DIC";

                dt.AcceptChanges();
            }

            Rep2.Controls.Add(
               TituloYBordesReporte(
                       DTIChartsAndControls.GridView("RepMat1", dt, true, "Totales", /*true, 294,*/
                       new string[] { "", "", "", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}", "{0:c}" }),
                       300));



            //Hay que trasponer los renglones y columnas
            if (dt.Rows[dt.Rows.Count - 1][0].ToString() == "Totales")
            {
                dt.Rows.RemoveAt(dt.Rows.Count - 1);

                DataView dvldt = new DataView(dt);
                dt = dvldt.ToTable(false, new string[] { "Concepto", "ENE", "FEB", "MAR", "ABR", "MAY", "JUN", "JUL", "AGO", "SEP", "OCT", "NOV", "DIC" });
                dt.AcceptChanges();
            }

            dt = dt.AsEnumerable()
                   .GroupBy(r => r.Field<string>("Concepto"))
                   .Select(g =>
                  {
                      var row = dt.NewRow();

                      row["Concepto"] = g.Key;
                      row["ENE"] = g.Sum(r => r.Field<double>("ENE")); row["FEB"] = g.Sum(r => r.Field<double>("FEB"));
                      row["MAR"] = g.Sum(r => r.Field<double>("MAR")); row["ABR"] = g.Sum(r => r.Field<double>("ABR"));
                      row["MAY"] = g.Sum(r => r.Field<double>("MAY")); row["JUN"] = g.Sum(r => r.Field<double>("JUN"));
                      row["JUL"] = g.Sum(r => r.Field<double>("JUL")); row["AGO"] = g.Sum(r => r.Field<double>("AGO"));
                      row["SEP"] = g.Sum(r => r.Field<double>("SEP")); row["OCT"] = g.Sum(r => r.Field<double>("OCT"));
                      row["NOV"] = g.Sum(r => r.Field<double>("NOV")); row["DIC"] = g.Sum(r => r.Field<double>("DIC"));

                      return row;
                  }).CopyToDataTable();


            dt = Transpose(dt);


            #region Grafica

            Rep4.Controls.Add(TituloYBordesReporte(new LiteralControl(
                FCAndControls.CreaContenedorGraficaYRadioButtonsGrafMultiSerie(
                "GrafRepMat1", "ControlesAlCentro", "msline")), 310));

            string[] lsaDataSource = FCAndControls.ConvertDataTabletoJSONString(
                                         FCAndControls.ConvertDataTabletoDataTableArray(dt));

            ScriptManager.RegisterStartupScript(this, typeof(Page), "GrafRepMat1",
                FCAndControls.graficaMultiSeries(lsaDataSource, FCAndControls.extraeNombreColumnas(dt), "msline", "GrafRepMat1",
                "", "", "Mes", "Importe", "ocean", "98%", "270", "GrafRepMat1", true), false);

            #endregion Grafica

        }

        private string MonthName(int month)
        {
            DateTimeFormatInfo dtinfo = new CultureInfo("es-ES", false).DateTimeFormat;
            return dtinfo.GetMonthName(month).ToUpper().Substring(0, 3);
        }

        private DataTable Transpose(DataTable dt)
        {
            DataTable dtNew = new DataTable();
            dtNew.Columns.Add("Mes");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dtNew.Columns.Add(dt.Rows[i][0].ToString());
            }

            for (int k = 1; k < dt.Columns.Count; k++)
            {
                DataRow r = dtNew.NewRow();
                r[0] = dt.Columns[k].ToString();
                for (int j = 1; j <= dt.Rows.Count; j++)
                    r[j] = dt.Rows[j - 1][k];
                dtNew.Rows.Add(r);
            }

            return dtNew;
        }

        private void BDEjemplo()
        {
            Random random = new Random();
            bd = new List<BDDemo>();
            for (int c = 0; c < datosFiltros.ElementAt(0).Value.Count; c++)
            {
                for (int d = 0; d < datosFiltros.ElementAt(1).Value.Count; d++)
                {
                    for (int co = 0; co < datosFiltros.ElementAt(2).Value.Count; co++)
                    {
                        BDDemo obj = new BDDemo();
                        obj.IdCompañia = datosFiltros.ElementAt(0).Value[c].Id;
                        obj.NombreCompañia = datosFiltros.ElementAt(0).Value[c].Nombre;

                        obj.IdDireccion = datosFiltros.ElementAt(1).Value[d].Id;
                        obj.NombreDireccion = datosFiltros.ElementAt(1).Value[d].Nombre;

                        obj.IdConcepto = datosFiltros.ElementAt(2).Value[co].Id;
                        obj.NombreConcepto = datosFiltros.ElementAt(2).Value[co].Nombre;

                        obj.Ene = Convert.ToDouble(random.Next(1000, 2000));
                        obj.Feb = Convert.ToDouble(random.Next(1000, 2000));
                        obj.Mar = Convert.ToDouble(random.Next(1000, 2000));
                        obj.Abr = Convert.ToDouble(random.Next(1000, 2000));
                        obj.May = Convert.ToDouble(random.Next(1000, 2000));
                        obj.Jun = Convert.ToDouble(random.Next(1000, 2000));
                        obj.Jul = Convert.ToDouble(random.Next(1000, 2000));
                        obj.Ago = Convert.ToDouble(random.Next(1000, 2000));
                        obj.Sep = Convert.ToDouble(random.Next(1000, 2000));
                        obj.Oct = Convert.ToDouble(random.Next(1000, 2000));
                        obj.Nov = Convert.ToDouble(random.Next(1000, 2000));
                        obj.Dic = Convert.ToDouble(random.Next(1000, 2000));

                        bd.Add(obj);
                    }
                }
            }
        }

        public static Control TituloYBordesReporte(Control control, int height)
        {
            Panel panel = new Panel();
            panel.HorizontalAlign = HorizontalAlign.Center;
            if (height > 0)
            {
                panel.CssClass = "PanelTitulosYBordeReportes maxWidth100Perc";
                panel.Height = height;
            }
            else
            {
                panel.CssClass = "PanelTitulosYBordeReportes AutoHeight maxWidth100Perc";
            }
            panel.Controls.Add(control);

            return panel;
        }
    }


    public class Catalogo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Filtro { get; set; }
    }

    public class BDDemo
    {
        public int IdCompañia { get; set; }
        public string NombreCompañia { get; set; }

        public int IdDireccion { get; set; }
        public string NombreDireccion { get; set; }

        public int IdConcepto { get; set; }
        public string NombreConcepto { get; set; }

        public double Ene { get; set; }
        public double Feb { get; set; }
        public double Mar { get; set; }
        public double Abr { get; set; }
        public double May { get; set; }
        public double Jun { get; set; }
        public double Jul { get; set; }
        public double Ago { get; set; }
        public double Sep { get; set; }
        public double Oct { get; set; }
        public double Nov { get; set; }
        public double Dic { get; set; }
    }
}

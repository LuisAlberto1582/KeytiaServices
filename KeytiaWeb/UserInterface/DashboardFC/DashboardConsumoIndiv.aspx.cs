using KeytiaServiceBL;
using KeytiaWeb.UserInterface.DashboardLT;
using System;
using System.Data;
using System.Text;
using System.Web.UI;
using KeytiaServiceBL.DataAccess;
using System.Linq;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Collections.Generic;
using KeytiaWeb.UserInterface.DashboardFC.ConsumoIndividualUserControls;

namespace KeytiaWeb.UserInterface.DashboardFC
{
    
    public partial class DashboardConsumoIndiv : System.Web.UI.Page
    {
        static string fechaInicio = "";
        static string fechaFinal = "";
        int iCodEmpleado;
        Random r = new Random();
        ConsumoIndividualUserControls.Consultas Consultas = new ConsumoIndividualUserControls.Consultas();
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion
            if (!IsPostBack)
            {
                IniciaProceso();
                ObtieneFechaFact();
            }
            var vchCodUsuario = Session["vchCodUsuario"].ToString();
            var emple =  DSODataAccess.Execute(Consultas.GetUsuario(vchCodUsuario)).AsEnumerable().FirstOrDefault();
            if (emple != null)
            {
                lblCorreo.Text = emple["Email"].ToString();
                lblNombre.Text = emple["Nombre"].ToString();
                lblPuesto.Text = emple["PuestoDesc"].ToString();

                iCodEmpleado = Convert.ToInt32(emple["iCodCatalogo"]);
            } 
            GetConfiguration();
            AddControls();
        }
        private void GetConfiguration()
        {
            CalculaFechas();
        }
        private void AddControls()
        {

            try
            {
                if (DateTime.Parse(fechaFinal) > ObtieneFechaLimite())
                {
                    pnlMainHolder.Controls.RemoveAt(1);
                    pnlMainHolder.Controls.Add(new Label() { Text = "No existe información para el mes seleccionado" });
                }
                else
                {


                    AgregaIndicadores(iCodEmpleado);
                    //Consumo Historico
                    Rep1.Controls.Add(ControlConsumoHistorico(iCodEmpleado));
                    //Movil
                    var lineas = DSODataAccess.Execute(Consultas.ObtieneLineasMoviles(iCodEmpleado, fechaInicio));
                    if (lineas.Rows.Count != 0)
                    {
                        pnlMainHolder.Controls.Add(new Panel() { ID = "Moviles" });
                        foreach (DataRow item in lineas.Rows)
                        {
                            var control = ControlesTelefoniaMovil(int.Parse(item["linea"].ToString()), item["LineaDesc"].ToString());
                            if (control != null)
                            {
                                pnlMainHolder.Controls.Add(control);
                                pnlMainHolder.Controls.Add(ControlesApps(int.Parse(item["linea"].ToString())));

                            }

                        }
                    }

                    ////Fija
                    DataTable telfija = DSODataAccess.Execute(Consultas.ConsumoTelefoniaFija(iCodEmpleado, fechaInicio));
                    if (telfija.Rows.Count != 0)
                    {
                        pnlMainHolder.Controls.Add(new Panel() { ID = "Fija" });
                        pnlMainHolder.Controls.Add(ControlesTelefoniaFija(iCodEmpleado, telfija));
                    }
                }
            }
            catch (Exception ex)
            {

                pnlMainHolder.Controls.RemoveAt(1);
                pnlMainHolder.Controls.Add(new Label() { Text = "Ha ocurrido un error.Por favor, inténtelo de nuevo más tarde."});
            }

          

           
        }
        private void AgregaIndicadores(int empleado)
        {
            var cantidadextensionies = DSODataAccess.ExecuteScalar(Consultas.ConsultaCantidadExtensiones(empleado, fechaInicio));
            var cantidadlineasmoviles = DSODataAccess.ExecuteScalar(Consultas.ConsultaCantidadLineasMoviles(empleado, fechaInicio));
            var cantidadclavesfac = DSODataAccess.ExecuteScalar(Consultas.ConsultaCantidadClavesFac(empleado, fechaInicio));
            var extensiones = DSODataAccess.Execute(Consultas.GetExtensiones(empleado, fechaInicio));
            var clavesfac = DSODataAccess.Execute(Consultas.GetCalvesFac(empleado, fechaInicio));
            var lineasmoviles = DSODataAccess.Execute(Consultas.ObtieneLineasMoviles(empleado, fechaInicio));
            lblExtensiones.Text = cantidadextensionies.ToString();
            lnkExtensiones.Title= "Extensiones: "+ string.Join(",", extensiones.AsEnumerable().SelectMany(x => x.ItemArray.Take(1)));
            lnkClaves.Title = "Claves Fac: " + string.Join(",", clavesfac.AsEnumerable().SelectMany(x => x.ItemArray.Take(1)));
            lnkMoviles.Title = "Lineas Móviles: " + string.Join(",", lineasmoviles.AsEnumerable().SelectMany(x=>x.ItemArray.Skip(1).Take(1)));
            lblMoviles.Text = cantidadlineasmoviles.ToString();
            lblClavesFac.Text = cantidadclavesfac.ToString();
        }
        private Control ControlesApps(int linea)
        {
            Panel apps = new Panel
            {
                CssClass = "row "
            };
            var desglocenacional = DSODataAccess.Execute(Consultas.ConsultaDesgloceAPP(linea, "Nac", fechaInicio));
            var desgloceint = DSODataAccess.Execute(Consultas.ConsultaDesgloceAPP(linea, "Int", fechaInicio));

            List<string> Apps = new List<string>() { "INSTAGRAM", "DATOS INTERNET", "FACEBOOK", "WHATSAPP", "DATOS SIN COSTO" };

            if (desglocenacional.Rows.Count != 0)
            {                    
                foreach (var item in Apps)
                {
                    switch (item)
                    {
                        case "INSTAGRAM":

                            if (desglocenacional.AsEnumerable().Where(x => x.Field<string>("SERVICIO") == "INSTAGRAM").FirstOrDefault() != null)
                            {
                                DataRow r = desglocenacional.NewRow();
                                r = desglocenacional.AsEnumerable().Where(x => x.Field<string>("SERVICIO") == "INSTAGRAM").FirstOrDefault();
                                apps.Controls.Add(GenericControl(double.Parse(r["MBInternet"].ToString()), double.Parse(r["IMPORTE"].ToString()), "Instagram"));


                            }
                            else
                            {
                                apps.Controls.Add(GenericControl(0, 0, "Instagram"));
                            }
                            break;
                        case "DATOS INTERNET":
                            if (desglocenacional.AsEnumerable().Where(x => x.Field<string>("SERVICIO") == "DATOS INTERNET").FirstOrDefault() != null)
                            {
                                DataRow r = desglocenacional.NewRow();
                                r = desglocenacional.AsEnumerable().Where(x => x.Field<string>("SERVICIO") == "DATOS INTERNET").FirstOrDefault();
                                apps.Controls.Add(GenericControl(double.Parse(r["MBInternet"].ToString()), double.Parse(r["IMPORTE"].ToString()), "Wifi Nac"));

                            }
                            else
                            {
                                apps.Controls.Add(GenericControl(0, 0, "Wifi"));
                            }
                            break;
                        case "FACEBOOK":
                            if (desglocenacional.AsEnumerable().Where(x => x.Field<string>("SERVICIO") == "FACEBOOK").FirstOrDefault() != null)
                            {
                                DataRow r = desglocenacional.NewRow();
                                r = desglocenacional.AsEnumerable().Where(x => x.Field<string>("SERVICIO") == "FACEBOOK").FirstOrDefault();
                                apps.Controls.Add(GenericControl(double.Parse(r["MBInternet"].ToString()), double.Parse(r["IMPORTE"].ToString()), "Facebook"));

                            }
                            else
                            {
                                apps.Controls.Add(GenericControl(0, 0, "Facebook"));
                            }
                            break;
                        case "WHATSAPP":
                            if (desglocenacional.AsEnumerable().Where(x => x.Field<string>("SERVICIO") == "WHATSAPP").FirstOrDefault() != null)
                            {
                                DataRow r = desglocenacional.NewRow();
                                r = desglocenacional.AsEnumerable().Where(x => x.Field<string>("SERVICIO") == "WHATSAPP").FirstOrDefault();
                                apps.Controls.Add(GenericControl(double.Parse(r["MBInternet"].ToString()), double.Parse(r["IMPORTE"].ToString()), "Whatsapp"));

                            }
                            else
                            {
                                apps.Controls.Add(GenericControl(0, 0, "Whatsapp"));
                            }
                            break;
                        case "DATOS SIN COSTO":
                            if (desglocenacional.AsEnumerable().Where(x => x.Field<string>("SERVICIO") == "DATOS SIN COSTO").FirstOrDefault() != null)
                            {
                                DataRow r = desglocenacional.NewRow();
                                r = desglocenacional.AsEnumerable().Where(x => x.Field<string>("SERVICIO") == "DATOS SIN COSTO").FirstOrDefault();
                                apps.Controls.Add(GenericControl(double.Parse(r["MBInternet"].ToString()), double.Parse(r["IMPORTE"].ToString()), "SinCosto"));

                            }
                            else
                            {
                                apps.Controls.Add(GenericControl(0, 0, "SinCosto"));
                            }
                            break;
                        default:
                            break;
                    }
                }
                if (desgloceint.Rows.Count != 0)
                {
                    var ServicioOtros = desgloceint.AsEnumerable().Where(x => x.Field<string>("Servicio") == "OTROS").FirstOrDefault();
                    if (ServicioOtros != null)
                    {
                        var otros = ServicioOtros.Field<decimal>("MBInternet");
                        var importe = ServicioOtros.Field<double>("IMPORTE");
                        if (otros != 0)
                        {
                            apps.Controls.Add(GenericControl((double)otros, importe, "Otros"));
                        }
                            
                    }
                    if (desgloceint.AsEnumerable().Where(x => x.Field<string>("SERVICIO") == "DATOS INTERNET").FirstOrDefault() != null)
                    {
                        var otrosint = desgloceint.AsEnumerable().Where(x => x.Field<string>("Servicio") == "DATOS INTERNET").FirstOrDefault().Field<decimal>("MBInternet");
                        var importeint = (ServicioOtros != null)? ServicioOtros.Field<double>("IMPORTE"):0;
                        if (otrosint != 0)
                        {
                            apps.Controls.Add(GenericControl((double)otrosint, importeint, "Wifi Int"));
                        } 
                    }
                    else
                    {
                        apps.Controls.Add(GenericControl(0, 0, "Wifi Int"));
                    }
                }
                else
                {
                    apps.Controls.Add(GenericControl(0, 0, "Otros"));
                    apps.Controls.Add(GenericControl(0, 0, "Wifi Int"));
                }
            }
            else
            {
                apps.Controls.Add(GenericControl(0,0, "Instagram"));            
                apps.Controls.Add(GenericControl(0, 0, "Wifi Nac"));
                apps.Controls.Add(GenericControl(0, 0, "Facebook"));
                apps.Controls.Add(GenericControl(0, 0, "Whatsapp"));
                apps.Controls.Add(GenericControl(0, 0, "SinCosto"));
                apps.Controls.Add(GenericControl(0, 0, "Otros"));
                apps.Controls.Add(GenericControl(0, 0, "Wifi Int"));
            }

            return apps;
        }
        private Control ControlesTelefoniaMovil(int linea, string LineaDesc)
        {

            DataTable presupuesto = ObtienePresupuestoGastoTelMovil(iCodEmpleado, linea);
            double psptop = 0;
            if (presupuesto.Rows.Count != 0)
            {
                psptop = presupuesto.Rows[0].Field<double>("Presupuesto");
             
            }
            #region Paneles
            Panel p = new Panel()
            {
                CssClass = "row rowback",
            };

            Panel pchild = new Panel
            {
                CssClass = "col-md-2 col-sm-2"
            };
            Label label = new Label()
            {
                CssClass = "MessageLabels",
                Text = "Telefonía Movil"

            };
            Label label2 = new Label()
            {
                CssClass = "MessageLabels"
            };
            Image img = new Image()
            {
                CssClass = "img-fluid"
            };
            if (presupuesto.Rows.Count != 0 && (double)presupuesto.Rows[presupuesto.Rows.Count - 1][1] > (double)presupuesto.Rows[presupuesto.Rows.Count - 1][2])
            {
                img.ImageUrl = "../../images/ConsumoIndividual/rojo.png";
                label2.Text = "¡CUIDADO! Este mes excediste tu presupuesto";
            }
            else
            {
                img.ImageUrl = "../../images/ConsumoIndividual/verde.png";
                label2.Text = "Gracias Por Contribuir al ahorro de este mes!";
            }
            Button linkButton = new Button
            {
                Text = "Llamadas",
                OnClientClick = $"DesgloceLlamadasMovil({linea});  return false;",
                CssClass = "boton"

            };
            linkButton.Attributes.Add("linea", linea.ToString());
            linkButton.Attributes.Add("tipo", "movil");
            linkButton.Attributes.Add("data-toggle", "modal");
            linkButton.Attributes.Add("data-target", "#modalmovil");
            Button linkButtonDatos = new Button
            {
                Text = "Datos",
                OnClientClick = $"DesgloceConsumoDeDatos({linea}); return false;",
                CssClass = "boton"
            };
            linkButtonDatos.Attributes.Add("linea", linea.ToString());
            linkButtonDatos.Attributes.Add("tipo", "datos");
            linkButtonDatos.Attributes.Add("data-toggle", "modal");
            linkButtonDatos.Attributes.Add("data-target", "#modaldatos");

            Button linkButtonDesgloce = new Button
            {
                Text = "Resumen",
                OnClientClick = $"DesgloceConceptos({linea},{iCodEmpleado},\"{LineaDesc}\"); return false;",
                CssClass = "boton"
            };
            linkButtonDesgloce.Attributes.Add("linea", linea.ToString());
            linkButtonDesgloce.Attributes.Add("tipo", "datos");
            linkButtonDesgloce.Attributes.Add("data-toggle", "modal");
            linkButtonDesgloce.Attributes.Add("data-target", "#modaldesgloce");

            pchild.Controls.Add(label);
            pchild.Controls.Add(img);
            pchild.Controls.Add(label2);
            pchild.Controls.Add(linkButton);
            pchild.Controls.Add(linkButtonDatos);
            pchild.Controls.Add(linkButtonDesgloce);
            Label lbllinea = new Label()
            {
                CssClass = "MessageTitles",
                Text = LineaDesc

            };

            p.Controls.Add(lbllinea);
            p.Controls.Add(pchild);

            Panel pchildchart = new Panel
            {
                CssClass = "col-md-5 col-sm-5"
            };
            #endregion
            Guid g = Guid.NewGuid();
            var gn = g.ToString().Replace('-', '_');
            pchildchart.Controls.Add(
                     DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                     DTIChartsAndControls.GridView($"TablaMovil_{gn}", presupuesto, true, "Totales"
                                     ),
                                    $"PrespvsGast_G_{gn}", "Plan Base / Gasto Tel Movil", "", 2, FCGpoGraf.TabularLiBaCoTa)
                     );

            DataView dvldt = new DataView(presupuesto);
            presupuesto = dvldt.ToTable(false, new string[] { "Mes", "Consumo" });
            if (presupuesto.Rows.Count != 0 && presupuesto.Rows[presupuesto.Rows.Count - 1]["Mes"].ToString() == "Totales")
            {
                presupuesto.Rows[presupuesto.Rows.Count - 1].Delete();
            }
            presupuesto.Columns["Mes"].ColumnName = "label";
            presupuesto.Columns["Consumo"].ColumnName = "value";


            Page.ClientScript.RegisterStartupScript(this.GetType(), $"PrespvsGast_G_{gn}",
                Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(presupuesto),
                $"PrespvsGast_G_{gn}", "", "", "", "", 2, FCGpoGraf.Matricial, psptop, "P. Base", alto: "170", nombreTemaGraf: "Line"), false);



            p.Controls.Add(pchildchart);


            //Reporte por categoría
            Panel pchildchart2 = new Panel
            {
                CssClass = "col-md-5 col-sm-5"
            };

            var dt = RepPorCategoria(linea);
            if (dt.Rows.Count != 0)
            {
                dt.Columns["Total llamadas"].ColumnName = "T. Llamadas";
                dt.Columns["Total minutos"].ColumnName = "T. Minutos";

            }

            pchildchart2.Controls.Add(
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                    DTIChartsAndControls.GridView($"GrafRepPorCategoria_{gn}", dt, true, "Totales", new string[] { "", "{0:c}", "", "" }
                                    ),
                                   $"RepPorCategoriaGraf_{gn}", "Conceptos De Factura", "", 3, FCGpoGraf.TabularLiBaCoTa)
                    );


            if (dt.Rows.Count != 0)
            {
                DataView ldt = new DataView(dt);
                dt = ldt.ToTable(false, new string[] { "Concepto", "Consumo" });
                dt.Columns["Concepto"].ColumnName = "label";
                dt.Columns["Consumo"].ColumnName = "value";
                if (dt.Rows[dt.Rows.Count - 1]["label"].ToString() == "Totales")
                {
                    dt.Rows[dt.Rows.Count - 1].Delete();
                }


                dt.AcceptChanges();
            }

            dt.AcceptChanges();
            Page.ClientScript.RegisterStartupScript(this.GetType(), $"RepPorCategoriaGraf_{gn}",
              Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dt),
              $"RepPorCategoriaGraf_{gn}", "", "", "", "", 3, FCGpoGraf.Matricial, 0, null, alto: "170", nombreTemaGraf: "Line"), false);
            p.Controls.Add(pchildchart2);
            return p;



        }
        private Control ControlesTelefoniaFija(int empleado, DataTable telfija)
        {
            Panel p = new Panel
            {
                CssClass = "row rowback",
                ID = "Fija"

            };
            p.Controls.Add(new LiteralControl("<br />"));
            p.Controls.Add(new LiteralControl("<br />"));
            Panel pchild = new Panel
            {
                CssClass = "col-md-2 col-sm-2"
            };
            Label label = new Label()
            {
                CssClass = "MessageLabels",
                Text = "Telefonía Fija"

            };
            Label label2 = new Label()
            {
                CssClass = "MessageLabels"

            };
            Image img = new Image()
            {
                CssClass = "img-fluid"
            };
            if ((double)telfija.Rows[telfija.Rows.Count - 1][1] > (double)telfija.Rows[telfija.Rows.Count - 1][2])
            {
                img.ImageUrl = "../../images/ConsumoIndividual/rojo.png";
                label2.Text = "¡CUIDADO! Este mes excediste tu presupuesto";
            }
            else
            {
                img.ImageUrl = "../../images/ConsumoIndividual/verde.png";
                label2.Text = "Gracias Por Contribuir al ahorro de este mes!";
            }
            Button linkButton = new Button
            {
                Text = "Llamadas",
                OnClientClick = $"DesgloceLlamadasFija({empleado});  return false;",
                CssClass = "boton"

            };
            linkButton.Attributes.Add("data-toggle", "modal");
            linkButton.Attributes.Add("data-target", "#modalfija");
            pchild.Controls.Add(label);
            pchild.Controls.Add(img);
            pchild.Controls.Add(label2);
            pchild.Controls.Add(linkButton);
            p.Controls.Add(pchild);
            p.Controls.Add(ControlPresupuestoVsGastoFija(empleado, telfija));
            p.Controls.Add(ControlConsumoTipoDestino(iCodEmpleado));
            return p;

        }
        private Control ControlConsumoTipoDestino(int empleado)
        {

            Panel pchildchart = new Panel
            {
                CssClass = "col-md-5 col-sm-5"
            };
            DataTable tipoDestino = DSODataAccess.Execute(Consultas.ConsumoIndividualTipoDestino(empleado, fechaInicio));
            //  var control = DTIChartsAndControls.TituloYPestañasRepNNvlGraficas("ConsumoTipoDest_G", "Consumo Por Tipo De Destino", 1, FCGpoGraf.Tabular, "");

            var control =
                   DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                   DTIChartsAndControls.GridView("GrafConsHistGridTab5", tipoDestino, true, "Totales", new string[] { "", "{0:c}", "", "" }
                                   ),
                                  "ConsumoTipoDest_G", "Consumo Por Tipo De Destino", "", 3, FCGpoGraf.TabularLiBaCoTa);
            DataView dtviewtipodest = new DataView(tipoDestino);

            var dttipodest = dtviewtipodest.ToTable(false, new string[] { "Tipo Destino", "Consumo" });
            dttipodest.Columns[0].ColumnName = "label";
            dttipodest.Columns[1].ColumnName = "value";

            if (dttipodest.Rows.Count != 0)
            {
                if (dttipodest.Rows[dttipodest.Rows.Count - 1]["label"].ToString() == "Totales")
                {
                    dttipodest.Rows[dttipodest.Rows.Count - 1].Delete();
                }
            }
            
            dttipodest.AcceptChanges();


            Page.ClientScript.RegisterStartupScript(this.GetType(), "ConsumoTipoDest_G",
                Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dttipodest),
                "ConsumoTipoDest_G", "", "", "", "", 3, FCGpoGraf.Matricial, 0, null,alto:"170", nombreTemaGraf: "Line"), false);

            pchildchart.Controls.Add(control);
            return pchildchart;
        }
        private Control ControlPresupuestoVsGastoFija(int empleado, DataTable telfija)
        {

            Panel pchildchart = new Panel
            {
                CssClass = "col-md-5 col-sm-5"
            };

            var control =
                    DTIChartsAndControls.TituloYPestañasRep1Nvl(
                                    DTIChartsAndControls.GridView("GrafConsHistGridTab3", telfija, true, "Totales", new string[] { "", "{0:c}", "", "{0}%" }
                                    ),
                                   "PrespvsGast_G", "Gasto Tel. Fija / Presupuesto", "", 2, FCGpoGraf.TabularLiBaCoTa)
                    ;
            DataView dtview = new DataView(telfija);
            var dtconsumofija = dtview.ToTable(false, new string[] { "Mes", "Consumo", "Presupuesto" });
            dtconsumofija.Columns[0].ColumnName = "label";
            dtconsumofija.Columns[1].ColumnName = "value";
            double psptop = 0;
            if (dtconsumofija.Rows.Count != 0)
            {
                if (dtconsumofija.Rows[dtconsumofija.Rows.Count - 1]["label"].ToString() == "Totales")
                {
                    dtconsumofija.Rows[dtconsumofija.Rows.Count - 1].Delete();
                }

                psptop = dtconsumofija.Rows[dtconsumofija.Rows.Count - 1].Field<double>("Presupuesto");
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "PrespvsGast_G",
                Grafica1Serie(FCAndControls.ConvertDataTabletoJSONString(dtconsumofija),
                "PrespvsGast_G", "", "", "", "", 2, FCGpoGraf.Matricial, psptop, "Ppto.", alto: "170", nombreTemaGraf: "Line"), false);

            pchildchart.Controls.Add(control);
            return pchildchart;
        }
        private Control ControlConsumoHistorico(int empleado)
        {
            DataTable ConsumoHistorico = DSODataAccess.Execute(Consultas.ConsumoHistorico(empleado, fechaInicio));

            var control = DTIChartsAndControls.TituloYPestañasRep1Nvl(
                             DTIChartsAndControls.GridView("GrafConsHist_G", ConsumoHistorico, false, "",
                              new string[] { "", "{0:c}", "{0:c}" }), "GrafConsHist_G",
                             "Consumo Historico", "", 0, FCGpoGraf.MatricialConStack2);


            string[] lsdt = FCAndControls.ConvertDataTabletoJSONString(FCAndControls.ConvertDataTabletoDataTableArray(ConsumoHistorico));

            Page.ClientScript.RegisterStartupScript(this.GetType(), "GrafConsHist_G",
                FCAndControls.GraficaMultiSeries(lsdt, FCAndControls.extraeNombreColumnas(ConsumoHistorico),
                "GrafConsHist_G", "", "", "", "", 1, FCGpoGraf.MatricialConStack2, alto: "280", nombreTemaGraf: "Line", numberSuffix: ""), false);

            return control;

        }
        private Control GenericControl(double consumo, double importe, string tipo)
        {
            
            System.Web.UI.HtmlControls.HtmlGenericControl divgen = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            //divgen.Attributes.Add("class", "col-sm-2 col-md-2");

            System.Web.UI.HtmlControls.HtmlGenericControl div = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            div.Attributes.Add("class", "columnapps");

            System.Web.UI.HtmlControls.HtmlGenericControl div2 = new System.Web.UI.HtmlControls.HtmlGenericControl("div");
            div2.Attributes.Add("class", "card");
            System.Web.UI.HtmlControls.HtmlGenericControl anchortag = new System.Web.UI.HtmlControls.HtmlGenericControl("a");
            anchortag.Attributes.Add("href", "#");
            System.Web.UI.HtmlControls.HtmlGenericControl fnti = new System.Web.UI.HtmlControls.HtmlGenericControl("a");

            Label label = new Label()
            {
                CssClass = "MessageLabels",

            };
            Label labelconsumo = new Label()
            {
                CssClass = "MessageLabels",
                Text = $"{consumo}MB"

            };
            Label labelImporte = new Label()
            {
                Text = $"${importe}"

            };
            if(importe > 0)
            {
                labelImporte.CssClass = "MessageLabelsAppsRojo";
            }
            else
            {
                labelImporte.CssClass = "MessageLabelsApps";

            }

            switch (tipo)
            {
                case "Facebook":
                    fnti.Attributes.Add("class", "fab fa-facebook fa-3x");
                    label.Text = "Facebook";
                    break;
                case "Instagram":
                    fnti.Attributes.Add("class", "fab fa-instagram fa-3x");
                    label.Text = "Instagram";
                    break;
                case "Whatsapp":
                    fnti.Attributes.Add("class", "fab fa-whatsapp fa-3x");
                    label.Text = "Whatsapp";
                    break;
                case "Wifi Int":
                    fnti.Attributes.Add("class", "fa fa-rss fa-3x");
                    label.Text = "Datos Internet Int.";
                    break;
                case "Wifi Nac":
                    fnti.Attributes.Add("class", "fa fa-wifi fa-3x");
                    label.Text = "Datos Internet Nac.";
                    break;
                case "Otros":
                    fnti.Attributes.Add("class", "fa fa-hashtag fa-3x");
                    label.Text = "Otros";
                    break;
                case "SinCosto":
                    fnti.Attributes.Add("class", "fa fa-check fa-3x");
                    label.Text = "Datos sin costo";
                    break;

            }
            anchortag.Controls.Add(fnti);
            div2.Controls.Add(anchortag);
            div2.Controls.Add(label);
            div2.Controls.Add(labelconsumo);
            div2.Controls.Add(labelImporte);
            div.Controls.Add(div2);
            divgen.Controls.Add(div);

            return divgen;
        }
        private DataTable ObtienePresupuestoGastoTelMovil(int emple, int linea)
        {
            var dt = DSODataAccess.Execute(Consultas.PresupuestoGastoTelMovilPorLinea(emple, linea, fechaInicio));
            DataView dtview = new DataView(dt);
            var dtconsumomovil = dtview.ToTable(false, new string[] { "Mes", "Consumo", "Presupuesto" });
            //dtconsumomovil.Columns[0].ColumnName = "label";
            //dtconsumomovil.Columns[1].ColumnName = "value";

            return dtconsumomovil;
        }

        //FD 20/06/2021 TrendingLines
        public static string Grafica1Serie(string DataSourceJSon, string idContenedor, string titulo, string segundoTitulo, string ejeX, string ejeY, int pestañaActiva, FCGpoGraf tiposGraficas, double top, string trendinglineName,
                                                 string numberprefix = "$ ", string numberSuffix = "", string nombreTemaGraf = "dti", string ancho = "98%", string alto = "385")
        {
            var tipoDefault = DTIChartsAndControls.GetListaPestañasGenericas(tiposGraficas);

            StringBuilder lsb = new StringBuilder();
            lsb.AppendLine("<script type=\"text/javascript\">");
            lsb.AppendLine("  FusionCharts.ready(function(){");
            lsb.AppendLine("        var FC_" + idContenedor + " = new FusionCharts({ ");
            lsb.AppendLine("        \"type\": \"" + tipoDefault.Keys.ElementAt(pestañaActiva) + "\", ");
            lsb.AppendLine("        \"width\": \"" + ancho + "\", ");
            lsb.AppendLine("        \"height\": \"" + alto + "\", ");
            lsb.AppendLine("        \"dataFormat\": \"json\", ");
            lsb.AppendLine("        \"dataSource\":  { ");
            lsb.AppendLine("          \"chart\": { ");
            lsb.AppendLine("            \"caption\": \"" + titulo + "\", ");
            lsb.AppendLine("            \"subCaption\": \"" + segundoTitulo + "\", ");
            lsb.AppendLine("            \"setAdaptiveYMin\": \"0\", ");
            lsb.AppendLine("            \"xAxisName\": \"" + ejeX + "\", ");
            lsb.AppendLine("            \"yAxisMaxValue\": \"" + top + "\", ");
            lsb.AppendLine("            \"yAxisName\": \"" + ejeY + "\", ");
            lsb.AppendLine("            \"formatNumberScale\": \"0\", ");
            lsb.AppendLine("            \"numberPrefix\": \"" + numberprefix + "\", ");
            lsb.AppendLine("            \"numberSuffix\": \"" + numberSuffix + "\",");
            lsb.AppendLine("            \"showlabels\": \"1\", ");
            lsb.AppendLine("            \"showvalues\": \"0\", ");
            lsb.AppendLine("            \"decimals\": \"2\", ");
            lsb.AppendLine("            \"decimalSeparator\": \".\", ");
            lsb.AppendLine("            \"thousandSeparator\": \",\", ");
            lsb.AppendLine("            \"theme\": \"" + nombreTemaGraf + "\", ");
            lsb.AppendLine("            \"labelDisplay\": \"rotate\", ");
            lsb.AppendLine("            \"slantLabels\": \"1\", ");
            lsb.AppendLine("            \"maxLabelHeight\": \"10\", ");
            lsb.AppendLine("            \"rotateLabels\": \"1\", ");
            lsb.AppendLine("            \"maxLabelWidthPercent\": \"20\", ");
        

            lsb.AppendLine("         }, ");
            lsb.AppendLine("         \"data\": " + DataSourceJSon + ",");
            if(trendinglineName != null)
            {
                lsb.AppendLine("\"trendlines\": [{" + "\"line\": [{ \"startvalue\": \"" + top.ToString() + "\", " +
                   "\"valueOnRight\": \"1\"," +
                    "\"parentYAxis\": \"S\"," +
                    "\"valuePadding\": \"20\"," +
                    "\"color\": \"#009405\"," +
                    "\"thickness\": \"3\"," +
                    "\"showOnTop\": \"1\"," +
                    "\"displayvalue\": \"" + trendinglineName+" $"+ top.ToString()+ 
                    "\"}]}]");

            }
            lsb.AppendLine("      } ");
            lsb.AppendLine("  }); ");
            lsb.AppendLine("");
            lsb.AppendLine("    radio = document.getElementsByClassName('" + idContenedor + "');");
            lsb.AppendLine("    for (i = 0; i < radio.length; i++) { ");
            lsb.AppendLine("        radElem = radio[i];");
            lsb.AppendLine("        if (radElem.localName === 'a') { ");
            lsb.AppendLine("            radElem.onclick = function(){ ");
            lsb.AppendLine("                val = this.getAttribute('attr');");
            lsb.AppendLine("                tipo = this.getAttribute('attrTipo');");
            lsb.AppendLine("                FC_" + idContenedor + ".chartType(tipo);");
            lsb.AppendLine("                FC_" + idContenedor + ".render(val, undefined, undefined); ");
            lsb.AppendLine("            };");
            lsb.AppendLine("        }");
            lsb.AppendLine("    }");
            lsb.AppendLine("");
            lsb.AppendLine("    if(radio.length > 0 && " + pestañaActiva + " < radio.length){");
            lsb.AppendLine("        radio[" + pestañaActiva + "].click();");
            lsb.AppendLine("        FC_" + idContenedor + ".render();");
            lsb.AppendLine("    }");
            lsb.AppendLine("});");
            lsb.AppendLine("</script> ");

            string chart = lsb.ToString();
            return chart;
        }

        public static string Grafica1Serie(string DataSourceJSon, string idContenedor, string titulo, string segundoTitulo, string ejeX, string ejeY, string tipoGrafica,
                                                 string numberprefix = "$ ", string numberSuffix = "", string nombreTemaGraf = "dti", string ancho = "98%", string alto = "385", bool agregarTagScript = true)
        {
            StringBuilder lsb = new StringBuilder();
            if (agregarTagScript)
            {
                lsb.AppendLine("<script type=\"text/javascript\">");
            }
            lsb.AppendLine("  FusionCharts.ready(function(){");
            lsb.AppendLine("        var " + idContenedor + " = new FusionCharts({ ");
            lsb.AppendLine("        \"type\": \"" + tipoGrafica + "\", ");
            lsb.AppendLine("        \"renderAt\": \"" + idContenedor + "\", ");
            lsb.AppendLine("        \"width\": \"" + ancho + "\", ");
            lsb.AppendLine("        \"height\": \"" + alto + "\", ");
            lsb.AppendLine("        \"dataFormat\": \"json\", ");
            lsb.AppendLine("        \"dataSource\":  { ");
            lsb.AppendLine("          \"chart\": { ");
            lsb.AppendLine("            \"caption\": \"" + titulo + "\", ");
            lsb.AppendLine("            \"subCaption\": \"" + segundoTitulo + "\", ");
            lsb.AppendLine("            \"setAdaptiveYMin\": \"0\", ");
            lsb.AppendLine("            \"xAxisName\": \"" + ejeX + "\", ");
            lsb.AppendLine("            \"yAxisName\": \"" + ejeY + "\", ");
            lsb.AppendLine("            \"formatNumberScale\": \"0\", ");
            lsb.AppendLine("            \"numberPrefix\": \"" + numberprefix + "\", ");
            lsb.AppendLine("            \"numberSuffix\": \"" + numberSuffix + "\",");
            lsb.AppendLine("            \"showlabels\": \"1\", ");
            lsb.AppendLine("            \"showvalues\": \"0\", ");
            lsb.AppendLine("            \"decimals\": \"2\", ");
            lsb.AppendLine("            \"decimalSeparator\": \".\", ");
            lsb.AppendLine("            \"thousandSeparator\": \",\", ");
            lsb.AppendLine("            \"theme\": \"" + nombreTemaGraf + "\", ");

          //  lsb.AppendLine("            \"labelDisplay\": \"rotate\", ");
            lsb.AppendLine("            \"maxLabelWidthPercent\": \"20\", ");
            //lsb.AppendLine("            \"slantLabels\": \"1\", ");
            lsb.AppendLine("         }, ");
            lsb.AppendLine("         \"data\": " + DataSourceJSon);
            lsb.AppendLine("      } ");
            lsb.AppendLine("  }); ");
            lsb.AppendLine("");
            lsb.AppendLine(idContenedor + ".configure(\"ChartNoDataText\", \"No existen datos de este reporte.\"); ");
            lsb.AppendLine(idContenedor + ".render();");
            lsb.AppendLine("});");
            if (agregarTagScript)
            {
                lsb.AppendLine("</script> ");
            }
            return lsb.ToString();
        }
        private DataTable RepPorCategoria(int linea)
        {
            DataTable dt = DSODataAccess.Execute(Consultas.RepPorCategoria(linea, fechaInicio));
            return dt;
        }
       
        protected void cboMes_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        protected void cboAnio_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
        public void ObtieneAnio(int anios)
        {
            if (anios == 0)
                anios = 1;
            StringBuilder query = new StringBuilder();
            query.AppendLine(" SELECT iCodCatalogo AS vchCodigo, vchDescripcion AS Descripcion FROM " + DSODataContext.Schema + ".[VisHistoricos('Anio','Años','Español')] WITH(NOLOCK)");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND vchDescripcion IN(  ");
            query.AppendLine(" DATEPART(YEAR, GETDATE()), ");

            for (int i = 1; i <= anios; i++)
            {
                query.AppendLine($"DATEPART(YEAR, DATEADD(YEAR, -{i}, GETDATE())),");
            }
            query.Remove(query.Length -3, 3);
            query.Append(") ");
            query.AppendLine(" ORDER BY vchDescripcion DESC");
            DataTable dt = DSODataAccess.Execute(query.ToString());
            if (dt != null && dt.Rows.Count > 0)
            {
                cboAnio.DataSource = dt;
                cboAnio.DataBind();
            }
        }
        private DataTable GetDataDropDownList(string clave)
        {
           
            StringBuilder query = new StringBuilder();
            query.Length = 0;
            query.AppendLine("SELECT [CAMPOS]");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[NOMVISTA]");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia");
            query.AppendLine("	AND dtFinVigencia >= GETDATE()");
            query = query.Replace("[CAMPOS]", "CASE WHEN LEN(VCHCODIGO) = 1 THEN '0' + VCHCODIGO ELSE VCHCODIGO END AS vchCodigo, UPPER(Español) AS Descripcion");
            query = query.Replace("[NOMVISTA]", "[VisHistoricos('Mes','Meses','Español')]");
            return DSODataAccess.Execute(query.ToString());
        }
        private void IniciaProceso()
        {
            //cboAnio.DataSource = GetDataDropDownList("ANIO").DefaultView;
            //cboAnio.DataBind();
            cboMes.DataSource = GetDataDropDownList("MES").DefaultView;
            cboMes.DataBind();
        }
        private void ObtieneFechaFact()
        {
            DataTable dt = DSODataAccess.Execute(ObtieneFechas());
            
            if (dt != null && dt.Rows.Count > 0)
            {
                
                DataRow dr = dt.Rows[0];
                ObtieneAnio(DateTime.Now.Year - int.Parse(dr["Anio"].ToString()));
                ValidaSelectCombo(dr["Anio"].ToString().ToString(), cboAnio);
                ValidaSelectCombo(dr["Mes"].ToString(), cboMes);
                fechaInicio = dr["FechaInicio"].ToString() + " 00:00:00";
                fechaFinal = dr["FechaFin"].ToString() + " 23:59:59";
            }
        }


        private DateTime ObtieneFechaLimite()
        {
            DataTable dt = DSODataAccess.Execute(ObtieneFechas());
            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
                //ValidaSelectCombo(dr["Anio"].ToString().ToString(), cboAnio);
                //ValidaSelectCombo(dr["Mes"].ToString(), cboMes);
                //fechaInicio = dr["FechaInicio"].ToString() + " 00:00:00";
                //fechaFinal = 

                return DateTime.Parse( dr["FechaFin"].ToString() + " 23:59:59");
            }
            else
            {
                return DateTime.Now;
            }
        }
        private void ValidaSelectCombo(string valor, DropDownList cbo)
        {
            string itemToCompare = string.Empty;
            string itemOrigin = valor.ToUpper();
            foreach (ListItem item in cbo.Items)
            {
                itemToCompare = item.Text.ToUpper();
                if (itemOrigin == itemToCompare)
                {
                    cbo.ClearSelection();
                    item.Selected = true;
                }
            }
        }
        //FD 20211015 No funciona en sql 2008. Funcion Format
        //private string ObtieneFechas()
        //{
        //    StringBuilder query = new StringBuilder();
        //    query.AppendLine(" DECLARE @FechaFin DATE");
        //    query.AppendLine(" SELECT");
        //    query.AppendLine(" @FechaFin = MAX(FechaPub)");
        //    query.AppendLine(" FROM " + DSODataContext.Schema + ".ResumenFacturasDeMoviles");
        //    query.AppendLine(" SELECT");
        //    query.AppendLine(" ISNULL(CONVERT(VARCHAR, CONVERT(DATE, DATEADD(mm, DATEDIFF(mm, 0, @FechaFin), 0))), '') AS FechaInicio,");
        //    query.AppendLine(" ISNULL(CONVERT(VARCHAR, CONVERT(DATE, CONVERT(VARCHAR(25), DATEADD(dd, -(DAY(DATEADD(mm, 1, @FechaFin))), DATEADD(mm, 1, @FechaFin)), 101))), '') AS FechaFin,");
        //    query.AppendLine(" ISNULL(UPPER(FORMAT(CONVERT(DATE, @FechaFin), N'Y', 'es-MX')), '') AS PeriodoFac,");
        //    query.AppendLine(" ISNULL(UPPER(FORMAT(CONVERT(DATE, @FechaFin), N'MMMM', 'es-MX')), '') AS Mes,");
        //    query.AppendLine(" ISNULL(UPPER(FORMAT(CONVERT(DATE, @FechaFin), N'yyyy', 'es-MX')), '') AS Anio");
        //    return query.ToString();
        //}
        private string ObtieneFechas()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine($"exec ObtieneFechasConsumoIndiv @Esquema = '{DSODataContext.Schema}'");
            return query.ToString();
        }
        private void CalculaFechas()
        {
            string anio = cboAnio.SelectedItem.ToString();
            string mes = cboMes.SelectedValue.ToString();
            DateTime F = Convert.ToDateTime((anio + "-" + mes + "-" + "01"));
            DateTime fecha2 = F.AddMonths(1).AddDays(-1);
            fechaInicio = F.ToString("yyyy-MM-dd 00:00:00");
            fechaFinal = fecha2.ToString("yyyy-MM-dd 23:59:59");

        }
        protected void btnAplicarFecha_Click(object sender, EventArgs e)
        {
            GetConfiguration();
         
        }

        #region WebMethod Json
        [WebMethod]
        public static  object DetalleLLamadasMoviles(object linea)
        {
            try
            {
                ConsumoIndividualUserControls.Consultas Consultas = new ConsumoIndividualUserControls.Consultas();
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = 500000000;
                var dt = DSODataAccess.Execute(Consultas.ReporteLlamadasMoviles(int.Parse(linea.ToString()), fechaInicio, fechaFinal));
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;
                int recid = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    row = new Dictionary<string, object>();
                    recid++;
                    foreach (DataColumn col in dt.Columns)
                    {
                        if (col.ColumnName.ToUpper() == "LOCALIDAD ORIGEN")
                        {
                            string c = dr[col].ToString();
                            if (c == "")
                            {
                                c = "NULL";
                            }
                            row.Add(col.ColumnName, c);
                        }
                        else
                        {
                            row.Add(col.ColumnName, dr[col]);
                        }

                    }
                    row.Add("recid", recid);
                    rows.Add(row);

                }
                //  return "{\"records\":" + serializer.Serialize(rows) + "}";
                object json = new { data = rows };
                return json;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [WebMethod]
        public static object DetalleLLamadasFija(object emple)
        {
            try
            {
                ConsumoIndividualUserControls.Consultas Consultas = new ConsumoIndividualUserControls.Consultas();
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = 500000000;
                var dt = DSODataAccess.Execute(Consultas.ReporteLlamadasFija(int.Parse(emple.ToString()), fechaInicio));
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;

                int recid = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    row = new Dictionary<string, object>();
                    recid++;
                    foreach (DataColumn col in dt.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    row.Add("recid", recid);
                    rows.Add(row);

                }
                // return "{\"records\":" + serializer.Serialize(rows) + "}";

                object json = new { data = rows };
                return json;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [WebMethod]
        public static object RepConsumoDeDatos(object linea)
        {
            try
            {
                ConsumoIndividualUserControls.Consultas Consultas = new ConsumoIndividualUserControls.Consultas();
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = 500000000;
                var dt = DSODataAccess.Execute(Consultas.ReporteConsumoDeDatos(int.Parse(linea.ToString()),fechaInicio));
                List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
                Dictionary<string, object> row;

                int recid = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    row = new Dictionary<string, object>();
                    recid++;
                    foreach (DataColumn col in dt.Columns)
                    {
                        row.Add(col.ColumnName, dr[col]);
                    }
                    row.Add("recid", recid);
                    rows.Add(row);

                }
                

                // return "{\"data\":" + serializer.Serialize(rows) + "}";
                object json = new { data = rows };
                return json;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [WebMethod]
        public static object RepDesgloceConceptos(object linea, object empleado)
        {
            try
            {
                ConsumoIndividualUserControls.Consultas Consultas = new ConsumoIndividualUserControls.Consultas();
                System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                serializer.MaxJsonLength = 500000000;
                var dt = DSODataAccess.Execute(Consultas.DesgloceConceptos((int)empleado , int.Parse(linea.ToString()), fechaInicio, fechaFinal));

                StringBuilder html = new StringBuilder();
                foreach (DataRow item in dt.Rows)
                {
                    html.AppendLine($"<h3>{item.Field<string>("Concepto")}</h3>");
                    html.Append("<table class='table table-striped table-bordered'>");
                    html.Append("<thead>");
                    html.Append("<tr>");
                    html.Append("<th width='70%'>Concepto</th>");
                    html.Append("<th width='70%'>Importe</th>");
                    html.Append("</tr>");
                    html.Append("</thead>");
                    var servicios = DSODataAccess.Execute( Consultas.DetalleServicios((int)empleado, (int)linea, fechaInicio, fechaFinal, item.Field<int>("idConcepto")));
                    
                    foreach (DataRow serv in servicios.Rows)
                    {
                        html.Append("<tr>");
                        html.AppendLine($"<td>{serv.Field<string>("Concepto")}</td>");
                        html.AppendLine($"<td>${serv.Field<double>("Total")}</td>");
                        html.Append("</tr>");
                    }
                    html.Append("<tr>");
                    html.AppendLine($"<th>Total</th>");
                    html.AppendLine($"<th>${servicios.AsEnumerable().Sum(x=>x.Field<double>("Total"))}</th>");
                    html.Append("</tr>");
                    html.Append("</table>");

                }
                return html.ToString() ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
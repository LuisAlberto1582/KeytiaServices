using KeytiaServiceBL;
using KeytiaServiceBL.Handler;
using KeytiaWeb.UserInterface.DashboardFC;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.Administracion.CMP
{
    public partial class AltaCMPReporte : System.Web.UI.Page
    {
        static string esquema = DSODataContext.Schema;
        static string connStr = DSODataContext.ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
            ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
            #endregion
            esquema = DSODataContext.Schema;
            connStr = DSODataContext.ConnectionString;
            if (!IsPostBack)
            {
                PopulateCencosCheckBoxList();
                FillDropDownLists();
               


            }

        }

        private void FillDropDownLists()
        {
            var lstAnios = AnioHandler.GetAll(DSODataContext.ConnectionString).Where(x => x.NumeroAnio >= (DateTime.Now.Year - 1));
            var lstMeses = MesHandler.GetAll(DSODataContext.ConnectionString);
            var lstEmpresas = new EmpresaHandler().GetAll(DSODataContext.ConnectionString);
            var lstMonedas = new MonedaHandler().GetAll(DSODataContext.ConnectionString);

            var anioDefault = lstAnios.FirstOrDefault(x => x.NumeroAnio == DateTime.Now.Year).ICodCatalogo;
            var mesDefault = lstMeses.FirstOrDefault(x => x.NumeroMes == DateTime.Now.Month).ICodCatalogo;
            // var empreDefault = (DSODataContext.Schema.ToUpper() != "KEYTIA") ? lstEmpresas.FirstOrDefault(x => x.VchCodigo != "KeytiaE").ICodCatalogo :
            //                                                                    lstEmpresas.FirstOrDefault(x => x.VchCodigo == "KeytiaE").ICodCatalogo;

            ddlYears.DataValueField = "iCodCatalogo";
            ddlYears.DataTextField = "NumeroAnio";
            ddlYears.DataSource = lstAnios;
            ddlYears.DataBind();
            ddlYears.SelectedValue = ddlYears.Items.FindByValue(anioDefault.ToString()).Value;

            ddlMonths.DataValueField = "iCodCatalogo";
            ddlMonths.DataTextField = "Español";
            ddlMonths.DataSource = lstMeses;
            ddlMonths.DataBind();
            ddlMonths.SelectedValue = ddlMonths.Items.FindByValue(mesDefault.ToString()).Value;

            //ddlEmpresa.DataValueField = "iCodCatalogo";
            //ddlEmpresa.DataTextField = "vchDescripcion";
            //ddlEmpresa.DataSource = lstEmpresas;
            //ddlEmpresa.DataBind();
            //ddlEmpresa.SelectedValue = ddlEmpresa.Items.FindByValue(empreDefault.ToString()).Value;


        }

        private void PopulateComboBoxMonths()
        {
            ddlMonths.DataTextField = "vchDescripcion";
            ddlMonths.DataValueField = "iCodCatalogo";
            ddlMonths.DataSource = null;
            ddlMonths.DataSource = GetDataMonths();
            ddlMonths.DataBind();
        }

        private void PopulateComboBoxYears()
        {
            ddlYears.DataTextField = "vchDescripcion";
            ddlYears.DataValueField = "iCodCatalogo";
            ddlYears.DataSource = null;
            ddlYears.DataSource = GetDataYears();
            ddlYears.DataBind();



        }


        private void PopulateCencosCheckBoxList()
        {

            cblElementos.DataTextField = "vchDescripcion";
            cblElementos.DataValueField = "iCodCatalogo";
            cblElementos.DataSource = null;
            cblElementos.DataSource = GetDataCenCos();
            cblElementos.DataBind();


        }

        [WebMethod]
        public static object GetDataCenCosAJAX()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT iCodCatalogo, vchDescripcion, EmpleDesc ");

            query.AppendLine("FROM " + esquema + ".[vishistoricos('CenCos','Centro de costos','Español')] WITH(NOLOCK)");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE() And Emple != '' ");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            return FCAndControls.ConvertDataTabletoJSONString(dt);

        }



        public DataTable GetDataCenCos()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT iCodCatalogo, vchDescripcion, EmpleDesc ");

            query.AppendLine("FROM " + esquema + ".[vishistoricos('CenCos','Centro de costos','Español')] WITH(NOLOCK)");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            return dt;

        }


        private DataTable GetDataYears()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT iCodCatalogo, vchDescripcion ");

            query.AppendLine("FROM " + esquema + ".[visHistoricos('Anio','Años','Español')] ");
            query.AppendLine("where dtfinvigencia >= getdate() order by vchcodigo");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            return dt;

        }

        private DataTable GetDataMonths()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT iCodCatalogo, vchDescripcion ");

            query.AppendLine("FROM " + esquema + ".[visHistoricos('Mes','Meses','Español')]  ");
            query.AppendLine("where dtfinvigencia >= getdate() order by vchcodigo");
            DataTable dt = DSODataAccess.Execute(query.ToString(), connStr);
            return dt;

        }

        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cblElementos.SelectedIndex != -1 && ddlYears.SelectedIndex != -1 && ddlMonths.SelectedIndex != -1)
                {
                    var query = InsertarDatos();
                    DataTable dt = DSODataAccess.Execute(query, connStr);
                    Response.Redirect("~/UserInterface/Administracion/CMP/ListadoReportes.aspx", false);
                }
                else
                {
                    lblMsj.Text = "Debe seleccionar al menos un centro de costos";


                }

            }
            catch (Exception ex)
            {

                lblMsj.Text = ex.Message;
            }

        }

        private string InsertarDatos()
        {
            List<string> cencos = new List<string>();
            foreach (ListItem item in cblElementos.Items)
            {
                if (item.Selected)
                {
                    cencos.Add(item.Value);

                }
            }
            int general = 0;
            if (chkGeneral.Checked)
            {
                general = 1;
            }

            var CC = txtCC.Text != "" ? txtCC.Text : "NULL";
            var CCO = txtCCO.Text != "" ? txtCCO.Text : "NULL";
            var CCGeneral = txtCCGeneral.Text != "" ? txtCCGeneral.Text : "NULL";
            var CCOGeneral = txtCCOGeneral.Text != "" ? txtCCOGeneral.Text : "NULL";

            StringBuilder consulta = new StringBuilder();

            consulta.AppendLine("declare @iCodCatAnio int = " + ddlYears.SelectedValue.ToString());
            consulta.AppendLine("declare @iCodCatMes int = " + ddlMonths.SelectedValue.ToString());
            consulta.AppendLine("declare @iCodCatUsuar int = " + Session["iCodUsuario"].ToString());
            consulta.AppendLine($"declare @lstCenCos varchar(8000) =  '" + string.Join(",", cencos) + "'");
            consulta.AppendLine("declare @esquema varchar(40) = '" + esquema + "'");
            consulta.AppendLine("declare @iCodCatUsuarConfig int = " + Session["iCodUsuario"].ToString());
            consulta.AppendLine("declare @bandera int =  " + general);
            consulta.AppendLine("declare @CC varchar(8000) =  '" + CC + "'");
            consulta.AppendLine("declare @CCO varchar(8000) =  '" + CCO + "'");
            consulta.AppendLine("declare @CCGeneral varchar(8000) =  '" + CCGeneral + "'");
            consulta.AppendLine("declare @CCOGeneral varchar(8000) =  '" + CCOGeneral + "'");








            consulta.AppendLine("declare @query varchar(max) ");



            consulta.AppendLine("declare @icodEntidad int");
            consulta.AppendLine("declare @icodMaestro int");
            consulta.AppendLine("declare @icodCatalogo int");
            consulta.AppendLine("declare @vchCodigo varchar(40)");
            consulta.AppendLine("declare @vchDescripcion varchar(160)");
            consulta.AppendLine("select @icodEntidad = iCodRegistro");
            consulta.AppendLine("from " + esquema + ".Catalogos");
            consulta.AppendLine("where icodCatalogo is null");
            consulta.AppendLine("and vchCodigo = 'ReporteAutomatizado'");
            consulta.AppendLine("select @icodMaestro = iCodRegistro ");
            consulta.AppendLine("from " + esquema + ".Maestros");
            consulta.AppendLine("where icodEntidad = @icodEntidad");
            consulta.AppendLine("and vchDescripcion = 'Envío reportes para directivos v1'");
            consulta.AppendLine("if @icodEntidad is not null and @icodMaestro is not null");
            consulta.AppendLine("begin");
            consulta.AppendLine("select @vchCodigo = convert(varchar, getdate(), 121)");
            consulta.AppendLine("select @vchDescripcion = @vchCodigo");
            consulta.AppendLine("insert into " + esquema + ".Catalogos");
            consulta.AppendLine("(iCodRegistro, iCodCatalogo, vchCodigo, dtIniVigencia, dtFinVigencia, vchDescripcion, iCodUsuario, dtFecUltAct)");
            consulta.AppendLine("values ");
            consulta.AppendLine("(");
            consulta.AppendLine("(select MAX(icodregistro)+1 from " + esquema + ".Catalogos), ");
            consulta.AppendLine("@icodEntidad, ");
            consulta.AppendLine("@vchCodigo, ");
            consulta.AppendLine("'2011-01-01', ");
            consulta.AppendLine("'2079-01-01', ");
            consulta.AppendLine("@vchDescripcion, ");
            consulta.AppendLine("@iCodCatUsuarConfig, ");
            consulta.AppendLine("GETDATE()");
            consulta.AppendLine(")");
            consulta.AppendLine("select @iCodCatalogo = iCodRegistro ");
            consulta.AppendLine("from " + esquema + ".Catalogos ");
            consulta.AppendLine("where isnull(iCodCatalogo, 0) = @icodEntidad ");
            consulta.AppendLine("and vchcodigo = @vchCodigo ");
            consulta.AppendLine("and vchDescripcion = @vchDescripcion ");
            consulta.AppendLine("if not exists(select icodregistro ");
            consulta.AppendLine("from " + esquema + ".[vishistoricos('ReporteAutomatizado','Envío reportes para directivos v1','Español')]");
            consulta.AppendLine("where dtfinvigencia>=getdate() ");
            consulta.AppendLine("and icodcatalogo = @iCodCatalogo ");
            consulta.AppendLine(")");
            consulta.AppendLine("begin");
            consulta.AppendLine("insert into " + esquema + ".[vishistoricos('ReporteAutomatizado','Envío reportes para directivos v1','Español')]");
            consulta.AppendLine("(iCodRegistro, iCodCatalogo, iCodMaestro, vchDescripcion, ");
            consulta.AppendLine("EstCarga, ");
            consulta.AppendLine("Anio, ");
            consulta.AppendLine("Mes, ");
            consulta.AppendLine("Usuar, ");
            consulta.AppendLine("BanderasEnvioReportesParaDirectivosV1, ");
            consulta.AppendLine("ListaCenCos, ");
            consulta.AppendLine("CtaCC, ");
            consulta.AppendLine("CtaCCO, ");
            consulta.AppendLine("CtaPara, ");
            consulta.AppendLine("CorreoElectronicoCC, ");
            consulta.AppendLine("dtIniVigencia, dtFinVigencia, iCodUsuario, dtFecUltAct) ");
            consulta.AppendLine("values ");
            consulta.AppendLine("(");
            consulta.AppendLine("(select MAX(icodregistro)+1 from " + esquema + ".Historicos), ");
            consulta.AppendLine("@iCodCatalogo, ");
            consulta.AppendLine("@iCodMaestro, ");
            consulta.AppendLine("@vchDescripcion, ");
            consulta.AppendLine("(select icodcatalogo from " + esquema + ".[visHistoricos('EstCarga','Estatus cargas','Español')] where dtfinvigencia >= getdate() and vchcodigo = 'CarEspera'), ");
            consulta.AppendLine("@iCodCatAnio , "); //@iCodCatAnio
            consulta.AppendLine("@iCodCatMes  , "); //@iCodCatMes int = 7276
            consulta.AppendLine("@iCodCatUsuar , "); // @iCodCatUsuar int = 200003
            consulta.AppendLine("@bandera , "); // @iCodCatUsuar int = 200003
            consulta.AppendLine("@lstCenCos , ");//@lstCenCos
            consulta.AppendLine("@CC , ");//@lstCenCos
            consulta.AppendLine("@CCO , ");//@lstCenCos
            consulta.AppendLine("@CCGeneral , ");//@lstCenCos
            consulta.AppendLine("@CCOGeneral , ");//@lstCenCos
            consulta.AppendLine("'2014-01-01', ");
            consulta.AppendLine("'2079-01-01', ");
            consulta.AppendLine("@iCodCatUsuarConfig , "); //@iCodCatUsuarConfig int = 200024
            consulta.AppendLine("getdate()");
            consulta.AppendLine(")");
            consulta.AppendLine("end");
            consulta.AppendLine("end");







            return consulta.ToString();
        }


        protected void cblElementos_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void chkPastConf_CheckedChanged(object sender, EventArgs e)
        {

            cblElementos.DataSource = null;
            cblElementos.Items.Clear();
            PopulateCencosCheckBoxList();

            CheckBox chk = (CheckBox)sender;
            if (chk.Checked)
            {
                StringBuilder consulta = new StringBuilder();
                consulta.Append("select Top 1 ListaCenCos, CtaCC, CtaCCO, CtaPara, CorreoElectronicoCC from " + esquema + " .[vishistoricos('ReporteAutomatizado','Envío reportes para directivos v1','Español')]");
                consulta.Append("Order by vchDescripcion DESC");

                DataTable dt = DSODataAccess.Execute(consulta.ToString(), connStr);

                if (dt.Rows.Count != 0)
                {
                    string[] cencos = dt.Rows[0].Field<string>("ListaCencos").Split(',');
                    List<ListItem> newlist = new List<ListItem>();



                    foreach (ListItem item in cblElementos.Items)
                    {
                        if (cencos.Contains(item.Value))
                        {
                            item.Selected = true; ;
                        }
                    }

                    txtCC.Text = dt.Rows[0].Field<string>("CtaCC") != "NULL" ? dt.Rows[0].Field<string>("CtaCC") : string.Empty;
                    txtCCO.Text = dt.Rows[0].Field<string>("CtaCCO") != "NULL" ? dt.Rows[0].Field<string>("CtaCCO") : string.Empty;
                    txtCCGeneral.Text = dt.Rows[0].Field<string>("CtaPara") != "NULL" ? dt.Rows[0].Field<string>("CtaPara") : string.Empty;
                    txtCCOGeneral.Text = dt.Rows[0].Field<string>("CorreoElectronicoCC") != "NULL" ? dt.Rows[0].Field<string>("CorreoElectronicoCC") : string.Empty;
                }

            }
            else
            {

                txtCC.Text = string.Empty;
                txtCCO.Text = string.Empty;
                txtCCGeneral.Text = string.Empty;
                txtCCOGeneral.Text = string.Empty;
            }

            //Page.ClientScript.RegisterStartupScript(this.GetType(), "CallMyFunction", "PopulateTable()", true);
            //cblElementos.DataBind();
            ScriptManager.RegisterStartupScript(this, GetType(), "displayalertmessage", "PopulateTable()", true);



        }

        protected void btnRegresar_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/UserInterface/Administracion/CMP/ListadoReportes.aspx", false);
        }
    }
}
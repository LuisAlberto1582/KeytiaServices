using DSOControls2008;
using KeytiaServiceBL;
using QlikAuthNet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace KeytiaWeb.UserInterface.Qlik
{
    public partial class Dashboard : System.Web.UI.Page
    {
        StringBuilder query = new StringBuilder();
        string ticket = string.Empty;
        DataTable dtAppPage = new DataTable();
        string icodApp="";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                #region Buscar el control de fecha que hereda de la Master y lo oculta ya que aquí no se usa.
                ((Panel)Form.FindControl("pnlRangeFechas")).Visible = false;
                #endregion

                bool acceso = true;
                ValidaBanderaCollapsable();

                if (Request.QueryString["App"] != null && Request.QueryString["App"]!="")
                {
                    icodApp = Request.QueryString["App"];

                    GetInfoAppAndPages();
                    if ((ticket = GetTicket()) == string.Empty)
                    {
                        mpeEtqMsn.Show();
                        acceso = false;
                    }
                }
                else
                {
                    GetInfoAppAndPages();

                    if (!Page.IsPostBack)
                    {
                        if ((ticket = GetTicket()) != string.Empty)
                        {
                            CondigFilterAndControls();
                        }
                        else
                        {
                            mpeEtqMsn.Show();
                            acceso = false;
                        }
                    }
                }             

                if (acceso) { BuildDashboard(); }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("Ocurrio un error en " + Request.Path + HttpContext.Current.Session["vchCodUsuario"] + "'", ex);
            }
        }


        #region Ticket

        private string GetTicket()
        {
            if (dtAppPage != null && dtAppPage.Rows.Count > 0)
            {
                string userD = dtAppPage.Rows[0]["UserDirectory"].ToString();
                string usrId = dtAppPage.Rows[0]["UserIdQlik"].ToString();
                string virtualProxy = dtAppPage.Rows[0]["VirtualProxy"].ToString();

                var req = new Ticket
                {
                    UserDirectory = userD,
                    UserId = usrId,
                };

                string host = "https://bikeytia.dti.com.mx";
                string uri = host + ":4243/qps/" + virtualProxy;            //  :4243/qps/
                req.ProxyRestUri = uri;

                return req.TicketRequest();
            }
            else { return string.Empty; }
        }


        #endregion

        #region Dashboard

        void CondigFilterAndControls()
        {
            if (dtAppPage != null && dtAppPage.Rows.Count > 0)
            {
                var dtResult = dtAppPage.Copy();
                //Se Configuran los filtros
                DataView dvResultado = new DataView(dtResult);
                dtResult = dvResultado.ToTable(true, new string[] { "iCodAppQlik", "NombreAppQlik", "AppDefault" });
                dtResult.AcceptChanges();
                dtResult.DefaultView.Sort = "AppDefault DESC";

                ddlApps.DataSource = dtAppPage.DefaultView.ToTable();
                ddlApps.DataTextField = "NombreAppQlik";
                ddlApps.DataValueField = "iCodAppQlik";
                ddlApps.DataBind();

                ddlApps.SelectedIndex = 0;                
            }
        }


        void BuildDashboard()
        {
            //if (ddlApps.SelectedIndex != -1)
            //{
                //var dtResultado = dtAppPage.AsEnumerable().First(x => x.Field<int>("iCodAppQlik") == Convert.ToInt32(ddlApps.SelectedValue));
                //if (dtResultado != null)
                //{
                //    ////Se inserta el HTML de Reportes
                //    replaceHTML.Controls.Clear();
                //    string control = dtResultado["HTML"].ToString().Replace("TICKETQLIK", ticket);
                //    //.Replace("border:none;height:660px", "border:none;height:840px;width:100%")
                //    replaceHTML.Controls.Add(new Literal() { Text = control.Replace("class="+"\""+ "col-sm-12" +"\"","") });
                //}
            //}

            var dtResultado = dtAppPage.AsEnumerable().First();
            if (dtResultado != null)
            {
                ////Se inserta el HTML de Reportes
                replaceHTML.Controls.Clear();
                string control = dtResultado["HTML"].ToString().Replace("TICKETQLIK", ticket);
                replaceHTML.Controls.Add(new Literal() { Text = control.Replace("class=" + "\"" + "col-sm-12" + "\"", "") });
            }

            if (dtAppPage != null && dtAppPage.Rows.Count > 1)
            {
                pToolBar.Attributes.Remove("style");
            }
            else { pToolBar.Attributes.Add("style", "display:none;"); }
        }

        #endregion

        #region Query

        void GetInfoAppAndPages()
        {
            query.Length = 0;
            query.AppendLine("DECLARE @Usuar INT = " + Session["iCodUsuario"].ToString());
            query.AppendLine("");
            query.AppendLine("SELECT");
            query.AppendLine("  --iCodLicencia      = Lic.iCodCatalogo,");
            query.AppendLine("	UserDirectory       = Lic.UserDirectory,");
            query.AppendLine("	UserIdQlik          = Lic.NombreUsuario,");
            query.AppendLine("	VirtualProxy        = Lic.VirtualProxy,");
            query.AppendLine("	iCodAppQlik		    = App.iCodCatalogo,");
            query.AppendLine("	IdAplicacionQlik	= App.IdAplicacion,");
            query.AppendLine("	NombreAppQlik		= App.Nombre,");
            query.AppendLine("	AppDefault			= (ISNULL(App.BanderasAppQlik,0) & 1)/1,");
            query.AppendLine("	HTML				= App.HTMLParte1");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('QlikApp','Qlik Aplicaciones','Español')] AS App WITH(NOLOCK)");           
            query.AppendLine("");
            query.AppendLine("    JOIN " + DSODataContext.Schema + ".[VisHistoricos('QlikLicencia','Qlik Licencias','Español')] AS Lic WITH(NOLOCK)");
            query.AppendLine("        ON Lic.Usuar = @Usuar");
            query.AppendLine("");
            query.AppendLine("        AND Lic.dtIniVigencia <> Lic.dtFinVigencia");
            query.AppendLine("        AND Lic.dtFinVigencia >= GETDATE()");
            query.AppendLine("");

            query.AppendLine("");
            query.AppendLine("    JOIN " + DSODataContext.Schema + ".[VisRelaciones('usuar - QlikApp','Español')] AS UsuarQlikApp WITH(NOLOCK) ");
            query.AppendLine("        ON UsuarQlikApp.Usuar = @Usuar ");
            query.AppendLine("        And UsuarQlikApp.QlikApp = App.iCodCatalogo ");
            query.AppendLine("");
            query.AppendLine("        AND UsuarQlikApp.dtIniVigencia <> UsuarQlikApp.dtFinVigencia");
            query.AppendLine("        AND UsuarQlikApp.dtFinVigencia >= GETDATE()");
            query.AppendLine("");

            query.AppendLine("WHERE App.dtIniVigencia<> App.dtFinVigencia");
            query.AppendLine("    AND App.dtFinVigencia >= GETDATE()");
            if(icodApp != "")
            {
                query.AppendLine(" AND App.iCodCatalogo = " + Convert.ToInt32(icodApp) + "");
            }
            else
            {
                query.AppendLine("ORDER BY AppDefault DESC");
            }
           

            dtAppPage = DSODataAccess.Execute(query.ToString());
        }
        void ValidaBanderaCollapsable()
        {
            StringBuilder query = new StringBuilder();

            query.AppendLine(" DECLARE @valueCollapse int = 0,");
            query.AppendLine(" @valueBanderaUsuarDB int = 0,");
            query.AppendLine(" @bandera INT,");
            query.AppendLine(" @iCodCatAtrib int = 0,");
            query.AppendLine(" @activo INT = 0");
            query.AppendLine(" SELECT");
            query.AppendLine(" @iCodCatAtrib = iCodCatalogo");
            query.AppendLine(" FROM ["+ DSODataContext.Schema + "].[VisHistoricos('Atrib','Atributos','Español')]AS atrib");
            query.AppendLine(" WHERE dtIniVigencia<> dtFinVigencia");
            query.AppendLine(" AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND vchCodigo = 'BanderasCliente'");
            query.AppendLine(" SELECT");
            query.AppendLine(" @valueBanderaUsuarDB = Value");
            query.AppendLine(" FROM [" + DSODataContext.Schema + "].[VisHistoricos('Valores','Valores','Español')]");
            query.AppendLine(" WHERE dtinivigencia<> dtfinvigencia");
            query.AppendLine(" AND dtfinVigencia >= GETDATE()");
            query.AppendLine(" AND atrib = @iCodCatAtrib");
            query.AppendLine(" AND vchCodigo = 'MenuColapsado'");
            query.AppendLine(" IF(@valueBanderaUsuarDB > 0)");
            query.AppendLine(" BEGIN");
            query.AppendLine(" SELECT");
            query.AppendLine(" @bandera = (ISNULL(BanderasCliente, 0))");
            query.AppendLine(" FROM [" + DSODataContext.Schema + "].[VisHistoricos('Client','Clientes','Español')] AS cliente WHERE dtIniVigencia<> dtFinVigencia");
            query.AppendLine(" AND dtFinVigencia >= GETDATE()");
            query.AppendLine(" AND vchCodigo <> 'KeytiaC'");
            query.AppendLine(" END");
            query.AppendLine(" IF((ISNULL(@bandera, 0) & @valueBanderaUsuarDB) / @valueBanderaUsuarDB = 1)");
            query.AppendLine(" BEGIN");
            query.AppendLine(" SET @activo = 1");
            query.AppendLine(" END");
            query.AppendLine(" SELECT @activo AS ACTIVO");

            DataTable dt = DSODataAccess.Execute(query.ToString());
            if(dt != null && dt.Rows.Count >0)
            {
                DataRow dr = dt.Rows[0];
                int activo = Convert.ToInt32(dr["ACTIVO"].ToString());
                if(activo == 0)
                {
                    string javaScript = "showContent();";
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "script", javaScript, true);
                }
            }
        }
        #endregion

    }
}
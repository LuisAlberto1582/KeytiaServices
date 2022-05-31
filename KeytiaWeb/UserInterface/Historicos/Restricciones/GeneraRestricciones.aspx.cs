using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using KeytiaServiceBL;

namespace KeytiaWeb.UserInterface
{
    public partial class GeneraRestricciones : System.Web.UI.Page
    {
        private string esquema = DSODataContext.Schema;
        private string connStr = DSODataContext.ConnectionString;


        #region Eventos

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                esquema = DSODataContext.Schema;
                connStr = DSODataContext.ConnectionString;
                if (!Page.IsPostBack)
                {
                    InicioProceso();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void OnSelectedIndexChanged_ddlPerfiles(object sender, EventArgs e)
        {
            try
            {
                CargarInfo();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        protected void OnClick_btnGenerar(object sender, EventArgs e)
        {
            try
            {
                txtbxMsg.Text = "";
                ProcesoGeneracionRestricciones();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region Logica

        public bool InicioProceso()
        {
            try
            {
                bool respuesta = false;


                CargarInfo();

                return respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool CargarInfo()
        {
            try
            {
                bool respuesta = false;
                string perfil = "";
                int iPerfil = 0;

                perfil = ddlPerfiles.SelectedValue.ToString();
                int.TryParse(perfil, out iPerfil);
                perfil = iPerfil > 0 ? perfil : "0";

                CargarPerfiles(esquema, perfil);

                CargarUsuarios(esquema, perfil);

                return respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool CargarPerfiles(string esquema, string perfil)
        {
            try
            {
                bool respuesta = false;

                DataTable dtPerfiles = BuscarPerfiles(esquema);

                if (dtPerfiles.Rows.Count > 0)
                {
                    ddlPerfiles.DataSource = dtPerfiles;

                    ddlPerfiles.DataValueField = "iCodCatPerfil";
                    ddlPerfiles.DataTextField = "PerfilDesc";

                    ddlPerfiles.DataBind();

                    ddlPerfiles.SelectedValue = perfil;

                    respuesta = true;
                }

                return respuesta;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public bool CargarUsuarios(string esquema, string perfil)
        {
            try
            {
                bool respuesta = false;

                DataTable dtUsuarios = new DataTable();

                dtUsuarios = BuscarUsuarios(esquema, perfil);


                ddlUsuarios.DataSource = dtUsuarios;
                ddlUsuarios.DataValueField = "iCodCatUsuar";
                ddlUsuarios.DataTextField = "DescUsuario";
                ddlUsuarios.DataBind();

                respuesta = true;

                return respuesta;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable BuscarPerfiles(string esquema)
        {
            try
            {
                return DSODataAccess.Execute(ConsultaBuscarPerfiles(esquema), DSODataContext.ConnectionString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public DataTable BuscarUsuarios(string esquema, string perfil)
        {
            try
            {
                return DSODataAccess.Execute(ConsultaBuscarUsuarios(esquema, perfil), DSODataContext.ConnectionString);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool ProcesoGeneracionRestricciones()
        {
            try
            {

                bool resultado = false;
                string perfil = ddlPerfiles.SelectedValue.ToString();
                string iCodCatUsuar = ddlUsuarios.SelectedValue.ToString();
                List<string> listaUsuariosGenRest = new List<string>();

                DataTable dtUsuarios = BuscarUsuarios(esquema, perfil);

                if (Convert.ToInt32(iCodCatUsuar) > 0)
                {
                    listaUsuariosGenRest = dtUsuarios.AsEnumerable().Where(r => r["iCodCatUsuar"].ToString() == iCodCatUsuar).Select(r => r["iCodCatUsuar"].ToString()).ToList<string>();
                }
                else
                {
                    if (Convert.ToInt32(perfil) > 0)
                    {
                        listaUsuariosGenRest = dtUsuarios.AsEnumerable().Select(r => r["iCodCatUsuar"].ToString()).ToList<string>();
                    }
                    else
                    {
                        //Mensaje el perfil o el usuario deben de encontrarse
                        mpeEtqMsn.Show();
                    }
                }


                txtbxMsg.Text = "";
                int contador = 0;
                bool procesoExitoso = true;
                foreach (string usuar in listaUsuariosGenRest)
                {


                    string esqquema = esquema;
                    string icodCatUsuar = usuar;
                    string entidad = "";

                    if (chbxSitio.Checked)
                    {
                        entidad = "Sitio";
                        if
                        (
                            (esquema != "" && esquema != null) &&
                            (icodCatUsuar != null && icodCatUsuar != "0") &&
                            (entidad != null && entidad != "")
                        )
                        {
                            resultado = GenerarRestriciones(esquema, icodCatUsuar, entidad);

                            if (!resultado)
                            {
                                procesoExitoso = false;
                            }
                        }

                        entidad = string.Empty;

                    }

                    if (chbxCenCos.Checked)
                    {
                        entidad = "CenCos";
                        if
                        (
                            (esquema != "" && esquema != null) &&
                            (icodCatUsuar != null && icodCatUsuar != "0") &&
                            (entidad != null && entidad != "")
                        )
                        {
                            resultado = GenerarRestriciones(esquema, icodCatUsuar, entidad);

                            if (!resultado)
                            {
                                procesoExitoso = false;
                            }
                        }


                        entidad = string.Empty;

                    }


                    if (chbxEmple.Checked)
                    {
                        entidad = "Emple";
                        if
                        (
                            (esquema != "" && esquema != null) &&
                            (icodCatUsuar != null && icodCatUsuar != "0") &&
                            (entidad != null && entidad != "")
                        )
                        {
                            resultado = GenerarRestriciones(esquema, icodCatUsuar, entidad);

                            if (!resultado)
                            {
                                procesoExitoso = false;
                            }
                        }


                        entidad = string.Empty;


                    }
                    if (Convert.ToInt32(usuar) > 0)
                    {
                        string msgtxtbx = "";
                        string vchCodigoUsuario = dtUsuarios.AsEnumerable().Where(r => r["iCodCatUsuar"].ToString() == usuar).Select(r => r["vchCodUsuar"]).FirstOrDefault().ToString();

                        msgtxtbx = procesoExitoso == true ? string.Format("{0} Procesado con exito\r\n", vchCodigoUsuario) : string.Format("{0} Procesado con error\r\n", vchCodigoUsuario);
                        txtbxMsg.Text += msgtxtbx;
                    }


                    if (Convert.ToInt32(usuar) > 0)
                    {
                        contador++;
                    }


                }
                resultado = true;
                txtbxMsg.Text += string.Format("\r\n\r\n{0} usuarios procesados", contador);
                return resultado;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool GenerarRestriciones(string esquema, string iCodCatUsuar, string Entidad)
        {
            try
            {
                bool respuesta = false;

                Convert.ToInt32(DSODataAccess.ExecuteNonQuery(ConsultaGenerarRestriciones(esquema, iCodCatUsuar, Entidad), DSODataContext.ConnectionString));

                respuesta = true;
                return respuesta;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion

        #region Consultas

        public string ConsultaBuscarPerfiles(string esquema)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendLine("Select 																	   ");
                query.AppendLine("	iCodCatPerfil	= 0,                                                       ");
                query.AppendLine("	PerfilCod       = 'Seleccionar perfil..',                                  ");
                query.AppendLine("	perfilDesc		= 'Seleccionar perfil..'                                   ");
                query.AppendLine("                                                                             ");
                query.AppendLine("Union All                                                                    ");
                query.AppendLine("                                                                             ");
                query.AppendLine("Select                                                                       ");
                query.AppendLine("	iCodCatPerfil	= perfil.iCodCatalogo,                                     ");
                query.AppendLine("	PerfilCod		= perfil.vchCodigo,                                        ");
                query.AppendLine("	perfilDesc		= perfil.vchDescripcion                                    ");
                query.AppendLine("from [" + esquema + "].[VisHistoricos('Perfil','Perfiles','Español')] perfil     ");
                query.AppendLine("                                                                             ");
                query.AppendLine("inner Join [" + esquema + "].[visHistoricos('usuar','usuarios','español')] usuar ");
                query.AppendLine("	On  usuar.perfil = perfil.iCodCatalogo                                     ");
                query.AppendLine("	and usuar.dtIniVigencia <> usuar.dtFinVigencia                             ");
                query.AppendLine("	and usuar.dtFinVigencia >= GETDATE()                                       ");
                query.AppendLine("                                                                             ");
                query.AppendLine("                                                                             ");
                query.AppendLine("Where perfil.dtIniVigencia <> perfil.dtFinVigencia                           ");
                query.AppendLine("and perfil.dtFinVigencia >= GETDATE()                                        ");
                query.AppendLine("                                                                             ");
                query.AppendLine("group by                                                                     ");
                query.AppendLine("		perfil.iCodCatalogo,                                                   ");
                query.AppendLine("		perfil.vchCodigo,                                                      ");
                query.AppendLine("		perfil.vchDescripcion                                                  ");
                return query.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ConsultaBuscarUsuarios(string esquema, string perfil)
        {
            try
            {
                StringBuilder query = new StringBuilder();


                query.AppendLine("Select															 ");
                query.AppendLine("  icodCatUsuar	= 0,                                             ");
                query.AppendLine("  vchCodUsuar		='Seleccionar un usuario ..',                    ");
                query.AppendLine("  vchDescUsuario	='',                                             ");
                query.AppendLine("  perfil          =0,                                              ");
                query.AppendLine("  perfilCod       ='',                                             ");
                query.AppendLine("  perfilDesc      ='',                                             ");
                query.AppendLine("  DescUsuario		='Seleccionar un usuario ..'                     ");
                query.AppendLine("                                                                   ");
                query.AppendLine("Union All                                                          ");
                query.AppendLine("                                                                   ");
                query.AppendLine("select                                                             ");
                query.AppendLine("	icodCatUsuar		= iCodCatalogo,                              ");
                query.AppendLine("	vchCodUsuar			= vchCodigo,                                 ");
                query.AppendLine("	vchDescUsuario		= vchDescripcion,                            ");
                query.AppendLine("	perfil              = perfil,                                    ");
                query.AppendLine("	perfilCod           = perfilCod,                                 ");
                query.AppendLine("	perfilDesc          = perfilDesc,                                ");
                query.AppendLine("	DescUsuario			= Upper(vchDescripcion)+' ( '+vchCodigo+' )' ");
                query.AppendLine("from [" + esquema + "].[visHistoricos('usuar','usuarios','español')]   ");
                query.AppendLine("where dtIniVigencia <> dtFinVigencia                               ");
                query.AppendLine("and dtFinVigencia >= GETDATE()                                     ");

                if (Convert.ToInt32(perfil) > 0)
                {
                    query.AppendLine("and perfil = " + perfil + "                                    ");
                }
                query.AppendLine("order by vchDescUsuario                                             ");


                return query.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ConsultaGenerarRestriciones(string esquema, string icodcatUsuario, string entidad)
        {
            try
            {
                StringBuilder query = new StringBuilder();

                query.AppendLine("");

                query.AppendLine("exec GeneraRestriccionesUsuario               ");
                query.AppendLine("    @esquema = '" + esquema + "',             ");
                query.AppendLine("    @iCodCatUsuar = " + icodcatUsuario + ",   ");
                query.AppendLine("    @entidad = '" + entidad + "'	            ");

                return query.ToString();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #endregion


    }
}

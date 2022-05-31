/*
 * Nombre:		    DMM
 * Fecha:		    20110520
 * Descripción:	    Navegador Keytia
 * Modificación:	
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using KeytiaServiceBL;
using System.Web.UI.HtmlControls;
using DSOControls2008;
using System.Web.SessionState;
using System.Text;
using System.IO;

[assembly: System.Web.UI.WebResource("KeytiaWeb.UserInterface.Navegador.jquery.ui.smoothMenu.js", "text/javascript")]
namespace KeytiaWeb.UserInterface
{
    public enum Permiso
    {
        Restringir,
        Consultar,
        Editar,
        Agregar,
        Eliminar,
        Administrar,
        Replicar
    }
    public class DSONavegador : DSOControl
    {
        protected HttpSessionState Session = HttpContext.Current.Session;
        protected string psLang;
        protected HtmlContainerControl pLista;
        protected Literal pHtml;
        protected TextBox pState;
        protected int piCodUsuario;
        protected int piCodPerfil = (int)HttpContext.Current.Session["iCodPerfil"];
        protected List<string> piCodPaquete = new List<string>();
        protected int piCodCliente = 0;
        protected DataTable pdtAplicaciones;
        protected DataTable pdtOpciones;
        protected DataTable pdtPermisos;
        protected KDBAccess kdb = new KDBAccess();
        protected Orientation pOrientacionMenu = Orientation.Horizontal;
        protected DataRow[] ldrRestringido;
        public Orientation OrientacionMenu
        {
            get
            {
                return pOrientacionMenu;
            }
            set
            {
                pOrientacionMenu = value;
            }
        }

        public DSONavegador()
        {
            Init += new EventHandler(DSONavegador_Init);
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            pHtml = new Literal();
            pState = new TextBox();
            pState.ID = "State";
            pState.Style.Add("display", "none");
            Controls.Add(pState);

            pLista = new HtmlGenericControl("UL");
            pLista.ID = ClientID + "_Navegador";
            pLista.Attributes.Add("class", "page-sidebar-menu page-sidebar-menu-light page-sidebar-closed page-sidebar-menu-hover-submenu");
            pLista.Attributes.Add("data-keep-expanded", "false");
            pLista.Attributes.Add("data-auto-scroll", "true");
            pLista.Attributes.Add("data-slide-speed", "500");
            pLista.Attributes.Add("style", "padding-top: 50px");

            Controls.Add(pHtml);
            ChildControlsCreated = true;
        }

        protected void DSONavegador_Init(object sender, EventArgs e)
        {
            LoadUserInfo();
            getOpciones();
        }

        public void LoadUserInfo()
        {
            piCodUsuario = (int)Session["iCodUsuario"];
            DataTable ldt = null;

            try
            {
                ldt = kdb.GetHisRegByEnt("Usuar", "Usuarios",
                    new string[] { "{Empre}", "{Perfil}" },
                    "iCodCatalogo = " + piCodUsuario.ToString());
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "Navegador");
                throw new KeytiaWebException(true, "ErrorConsulta", e, lsTitulo);
            }
            if (ldt != null && ldt.Rows.Count > 0)
            {
                if (!(ldt.Rows[0]["{Empre}"] is DBNull))
                    piCodCliente = (int)kdb.ExecuteScalar("Empre", "Empresas", "Select {Client} from Historicos where iCodCatalogo = " + ldt.Rows[0]["{Empre}"] + " and '" + DateTime.Today.ToString("yyyy-MM-dd") + "' between dtIniVigencia and dtFinVigencia");
                //if (!(ldt.Rows[0]["{Perfil}"] is DBNull))
                //    piCodPerfil = (int)ldt.Rows[0]["{Perfil}"];

                try
                {
                    ldt = kdb.GetRelRegByDes("Cliente - Paquete", "{Client} = " + piCodCliente.ToString(), new string[] { "{Paque}" });
                }
                catch (Exception e)
                {
                    throw new KeytiaWebException("ErrorConsulta", e, "Cliente - Paquete");
                }

                if (ldt != null && ldt.Rows.Count > 0)
                {
                    foreach (DataRow ldr in ldt.Rows)
                    {
                        piCodPaquete.Add(((int)ldt.Rows[0]["{Paque}"]).ToString());
                    }
                }
            }

        }

        protected DataRow[] Consultar(int iCodOpcionMenu)
        {
            return pdtOpciones.Select("IsNull([{OpcMnu}], 0) = " + iCodOpcionMenu + " and IsNull([{vchPermiso}], 'Restringir') <> 'Restringir'", "{OrdenMenu}");
        }

        public void getOpciones()
        {
            if (DSODataContext.GetObject("Aplic") == null)
            {
                try
                {
                    pdtAplicaciones = kdb.GetHisRegByEnt("Aplic", "", new string[] { "iCodRegistro", "iCodCatalogo", "{URL}" });
                    DSODataContext.SetObject("Aplic", pdtAplicaciones);
                }
                catch (Exception e)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "Aplicaciones");
                    throw new KeytiaWebException(true, "ErrorConsulta", e, lsTitulo);
                }
            }
            else
            {
                pdtAplicaciones = (DataTable)DSODataContext.GetObject("Aplic");
            }
            if (DSODataContext.GetObject("Permiso") == null)
            {
                try
                {
                    pdtPermisos = kdb.GetHisRegByEnt("Permiso", "Permisos", new string[] { "iCodCatalogo", "vchDescripcion", "{OpcionesPermisos}" });
                    DSODataContext.SetObject("Permiso", pdtPermisos);
                }
                catch (Exception e)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "Permiso");
                    throw new KeytiaWebException(true, "ErrorConsulta", e, lsTitulo);
                }
            }
            else
            {
                pdtPermisos = (DataTable)DSODataContext.GetObject("Permiso");
            }
            ldrRestringido = pdtPermisos.Select("vchDescripcion = 'Restringir'");
            if (Session["OpcionesUsuario"] == null)
            {
                DataTable ldtUsuario, ldtPerfil, ldtPaquete, ldtCliente;
                try
                {
                    psLang = Globals.GetCurrentLanguage();

                    int liRestringido = (int)pdtPermisos.Select("vchDescripcion = 'Restringir'")[0]["iCodCatalogo"];
                    pdtOpciones = kdb.GetHisRegByEnt("OpcMnu", "Opciones de Menu", new string[] { "iCodRegistro", "iCodCatalogo", "{OpcMnu}", "{Aplic}", "{" + psLang + "}", "{OrdenMenu}", "{Icono}"});
                    ldtUsuario = kdb.GetRelRegByDes("Usuario - Opción - Permiso", "{Usuar} = " + piCodUsuario.ToString(), new string[] { "{OpcMnu}", "{Permiso}" });
                    ldtPerfil = kdb.GetRelRegByDes("Perfil - Opción - Permiso", "IsNull([{Permiso}], 0) <> " + liRestringido + " and {Perfil} = " + piCodPerfil.ToString(), new string[] { "{OpcMnu}", "{Permiso}" });
                    ldtCliente = kdb.GetRelRegByDes("Cliente - Opción - Permiso", "{Client} = " + piCodCliente.ToString(), new string[] { "{OpcMnu}", "{Permiso}" });
                    ldtPaquete = kdb.GetRelRegByDes("Paquete - Opción - Permiso", "IsNull([{Permiso}], 0) <> " + liRestringido + " and {Paque} in (0" + String.Join(", ", piCodPaquete.ToArray()) + ")", new string[] { "{OpcMnu}", "{Permiso}" });
                }
                catch (Exception e)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "Opciones");
                    throw new KeytiaWebException(true, "ErrorConsulta", e, lsTitulo);
                }

                AgregarPermisos(ldtUsuario);
                AgregarPermisos(ldtPerfil);
                AgregarPermisos(ldtCliente);
                AgregarPermisos(ldtPaquete);

                DataColumn dc = new DataColumn();
                dc.ColumnName = "{vchPermiso}";
                dc.DataType = System.Type.GetType("System.String");
                pdtOpciones.Columns.Add(dc);
                dc = new DataColumn();
                dc.ColumnName = "{OpcionesPermisos}";
                dc.DataType = System.Type.GetType("System.Int32");
                pdtOpciones.Columns.Add(dc);
                DataTable ldtOpcionesCliente = getOpciones(ldtPaquete, ldtCliente, pdtOpciones);
                DataTable ldtOpcionesUsuario = getOpciones(ldtPerfil, ldtUsuario, ldtOpcionesCliente);

                pdtOpciones = ldtOpcionesUsuario;
                Session["OpcionesUsuario"] = pdtOpciones;
            }
            else
            {
                pdtOpciones = (DataTable)Session["OpcionesUsuario"];
            }
        }

        protected DataTable getOpciones(DataTable ldtPerfil, DataTable ldtUsuario, DataTable ldtOpciones)
        {
            DataTable ldtOpcionesPerfilUsuario = ldtOpciones.Clone();
            foreach (DataRow ldrPerfil in ldtPerfil.Rows)
            {
                foreach (DataRow ldrOpciones in ldtOpciones.Select("IsNull(iCodCatalogo, 0) = " + ldrPerfil["{OpcMnu}"].ToString()))
                {
                    DataRow dr = ldtOpcionesPerfilUsuario.NewRow();
                    dr.ItemArray = ldrOpciones.ItemArray;
                    if (dr["{OpcionesPermisos}"] is DBNull || (int)dr["{OpcionesPermisos}"] > (int)Util.IsDBNull(ldrPerfil["{OpcionesPermisos}"], -1))
                    {
                        dr["{vchPermiso}"] = ldrPerfil["{vchPermiso}"];
                        dr["{OpcionesPermisos}"] = ldrPerfil["{OpcionesPermisos}"];
                    }
                    ldtOpcionesPerfilUsuario.Rows.Add(dr);
                }
            }
            foreach (DataRow ldrUsuario in ldtUsuario.Rows)
            {
                foreach (DataRow ldrOpciones in ldtOpciones.Select("IsNull(iCodCatalogo, 0) = " + ldrUsuario["{OpcMnu}"].ToString()))
                {
                    DataRow[] rows = ldtOpcionesPerfilUsuario.Select("IsNull(iCodCatalogo, 0) = " + ldrUsuario["{OpcMnu}"].ToString());
                    if (ldrUsuario["{OpcionesPermisos}"] is DBNull || (int)ldrUsuario["{OpcionesPermisos}"] == 0)
                    {
                        if (rows.Length > 0)
                        {
                            ldtOpcionesPerfilUsuario.Rows.Remove(rows[0]);
                        }
                    }
                    else
                    {
                        if (rows.Length == 0)
                        {
                            DataRow dr = ldtOpcionesPerfilUsuario.NewRow();
                            dr.ItemArray = ldrOpciones.ItemArray;
                            dr["{vchPermiso}"] = ldrUsuario["{vchPermiso}"];
                            dr["{OpcionesPermisos}"] = ldrUsuario["{OpcionesPermisos}"];
                            ldtOpcionesPerfilUsuario.Rows.Add(dr);
                        }
                        else if ((int)rows[0]["{OpcionesPermisos}"] != (int)ldrUsuario["{OpcionesPermisos}"])
                        {
                            ldtOpcionesPerfilUsuario.Rows.Remove(rows[0]);
                            DataRow dr = ldtOpcionesPerfilUsuario.NewRow();
                            dr.ItemArray = ldrOpciones.ItemArray;
                            dr["{vchPermiso}"] = ldrUsuario["{vchPermiso}"];
                            dr["{OpcionesPermisos}"] = ldrUsuario["{OpcionesPermisos}"];
                            ldtOpcionesPerfilUsuario.Rows.Add(dr);
                        }
                    }
                }
            }
            return ldtOpcionesPerfilUsuario;
        }

        protected void AgregarPermisos(DataTable ldtTabla)
        {
            DataColumn dc = new DataColumn();
            dc.ColumnName = "{vchPermiso}";
            dc.DataType = System.Type.GetType("System.String");
            ldtTabla.Columns.Add(dc);

            dc = new DataColumn();
            dc.ColumnName = "{OpcionesPermisos}";
            dc.DataType = System.Type.GetType("System.Int32");
            ldtTabla.Columns.Add(dc);


            foreach (DataRow dr in ldtTabla.Rows)
            {
                if (!(dr["{Permiso}"] is DBNull))
                {
                    DataRow[] drPermisos = pdtPermisos.Select("iCodCatalogo = " + dr["{Permiso}"].ToString());
                    if (drPermisos.Length > 0)
                    {
                        dr["{vchPermiso}"] = drPermisos[0]["vchDescripcion"];
                        dr["{OpcionesPermisos}"] = drPermisos[0]["{OpcionesPermisos}"];
                    }
                }
                else if (ldrRestringido.Length > 0)
                {
                    dr["{vchPermiso}"] = ldrRestringido[0]["vchDescripcion"];
                    dr["{OpcionesPermisos}"] = ldrRestringido[0]["{OpcionesPermisos}"];
                }
            }
        }

        protected void CrearMenu()
        {
            StringBuilder lsb = new StringBuilder();
            TextWriter ltw = new StringWriter(lsb);
            HtmlTextWriter lhtw = new HtmlTextWriter(ltw);

            if (pdtOpciones.Rows.Count == 0) return;

            AgregarOpcionesH(pLista, Consultar(0), true);
            pLista.RenderControl(lhtw);
            pHtml.Text = lsb.ToString();

        }

        protected void AgregarOpcionesH(HtmlContainerControl pLista, DataRow[] ldrOpciones, bool primerNivel)
        {
            foreach (DataRow dr in ldrOpciones)
            {
                HtmlContainerControl liNav = new HtmlGenericControl("LI");
                liNav.Attributes.Add("class", "nav-item");

                HtmlAnchor a = new HtmlAnchor();
                DataRow[] ldrConsulta = Consultar((int)dr["iCodCatalogo"]);

                HtmlContainerControl spanTitle = new HtmlGenericControl("SPAN");
                spanTitle.Attributes.Add("class", "title");
                spanTitle.InnerText = dr["{" + psLang + "}"].ToString();

                if (primerNivel || ldrConsulta.Length > 0)
                {
                    a.Attributes.Add("class", "nav-link nav-toggle");                                     

                    if (primerNivel)
                    {
                        if (dr["{" + psLang + "}"].ToString().Length > 8)
                        {
                            liNav.Attributes.Add("class", "nav-item format-title");
                        }

                        var icono = dr["{Icono}"].ToString();
                        if (!string.IsNullOrEmpty(icono))
                        {
                            if (!icono.Contains("~"))
                            {
                                HtmlContainerControl liPrimerLevel = new HtmlGenericControl("I");
                                liPrimerLevel.Attributes.Add("class", icono);
                                a.Controls.Add(liPrimerLevel);
                            }
                            else 
                            {
                                HtmlContainerControl imgPrimerLevel = new HtmlGenericControl("IMG");
                                imgPrimerLevel.Attributes.Add("src", ResolveClientUrl(icono));
                                a.Controls.Add(imgPrimerLevel);
                            }
                        }
                        else 
                        {
                            HtmlContainerControl liPrimerLevel = new HtmlGenericControl("I");
                            liPrimerLevel.Attributes.Add("class", "fa fa-th");
                            a.Controls.Add(liPrimerLevel);
                        }                        

                        spanTitle.Attributes.Add("class", "title-item-menu");
                    }          

                    a.Controls.Add(spanTitle);

                    if (ldrConsulta.Length > 0)
                    {
                        HtmlContainerControl spanArrowRight = new HtmlGenericControl("DIV");
                        spanArrowRight.Attributes.Add("class", "arrowRight");
                        a.Controls.Add(spanArrowRight);

                        HtmlContainerControl spanArrow = new HtmlGenericControl("SPAN");
                        spanArrow.Attributes.Add("class", "arrow");
                        a.Controls.Add(spanArrow);
                    }
                }
                else
                {
                    a.Attributes.Add("class", "nav-link");
                    a.Controls.Add(spanTitle);
                }


                liNav.Controls.Add(a);
                pLista.Controls.Add(liNav);

                if (dr["{Aplic}"] is System.DBNull || dr["{Aplic}"].ToString() == "")
                {
                    a.HRef = "#";
                }
                else
                {
                    a.HRef = getHRef((int)dr["{Aplic}"], dr["vchCodigo"].ToString());
                    if (a.HRef == "#")
                    {
                        string msg = Globals.GetMsgWeb(true, "AplicMalConfig", dr["vchCodigo"].ToString());
                        string title = Globals.GetMsgWeb(true, "TituloNavegador");
                        a.Attributes.Add(HtmlTextWriterAttribute.Onclick.ToString(),
                        "javascript:jAlert('" + msg + "', '" + title + "'); return;");
                    }
                }

                if (ldrConsulta.Length > 0)
                {
                    HtmlContainerControl ulNav = new HtmlGenericControl("UL");
                    ulNav.Attributes.Add("class", "sub-menu");

                    HtmlContainerControl iNav = new HtmlGenericControl("I");
                    iNav.Attributes.Add("class", "fas fa-caret-left");

                    ulNav.Controls.Add(iNav);
                    liNav.Controls.Add(ulNav);

                    AgregarOpcionesH(ulNav, ldrConsulta, false);
                }
            }
        }       

        protected string getHRef(int iCodAplic, string vchCodigo)
        {
            string HRef = "";
            DataRow drAplic = null;
            if (pdtAplicaciones.Select("iCodCatalogo = " + iCodAplic.ToString()).Length == 0)
            {
                pdtAplicaciones = kdb.GetHisRegByEnt("Aplic", "", new string[] { "iCodRegistro", "iCodCatalogo", "{URL}" });
                DSODataContext.SetObject("Aplic", pdtAplicaciones);
            }
            if (pdtAplicaciones.Select("iCodCatalogo = " + iCodAplic.ToString()).Length == 0)
            {
                HRef = "#";
                return HRef;
            }
            drAplic = pdtAplicaciones.Select("iCodCatalogo = " + iCodAplic.ToString())[0];

            HRef = drAplic["{URL}"].ToString();

            if (HRef.IndexOf("?") < 0)
            {
                HRef += "?";
            }


            if (HRef.IndexOf("Opc=") < 0)
            {
                if (!HRef.EndsWith("&") && !HRef.EndsWith("?"))
                {
                    HRef += "&";
                }
                HRef += "Opc=" + vchCodigo;
            }
            else
            {
                string queryString = HRef.Substring(HRef.IndexOf("Opc="));
                if (queryString.IndexOf("&") > 0)
                {
                    queryString = queryString.Substring(0, queryString.IndexOf("&"));
                }
                HRef.Replace(queryString, "Opc=" + vchCodigo);
            }
            return HRef;
        }

        protected override void AttachClientEvents()
        {
            LoadControlScriptBlock(typeof(DSONavegador), "NavegadorInit", "<script src='" + ResolveClientUrl("~/UserInterface/Navegador/Navegador.js") + "' type='text/javascript'></script>", true, false);
            LoadControlScript(typeof(DSONavegador), "KeytiaWeb.UserInterface.Navegador.jquery.ui.smoothMenu.js", true, true);
            foreach (string key in pHTClientEvents.Keys)
            {
                pLista.Attributes[key] = (string)pHTClientEvents[key];
            }
            pLista.Attributes["direction"] = pOrientacionMenu.ToString().ToLower();

            psLang = Globals.GetCurrentLanguage();
            if (!((DataTable)Session["OpcionesUsuario"]).Columns.Contains("{" + psLang + "}"))
            {
                Session["OpcionesUsuario"] = null;
                getOpciones();
            }
            CrearMenu();
        }

        public static string getPermiso(string lsOpcMnu)
        {
            if (HttpContext.Current.Session["OpcionesUsuario"] == null) return "Restringir";

            string lsPermiso;
            DataRow[] dr = ((DataTable)HttpContext.Current.Session["OpcionesUsuario"]).Select("vchCodigo = '" + lsOpcMnu + "'");

            if (dr.Length == 0)
            {
                lsPermiso = "Restringir";
            }
            else
            {
                lsPermiso = (string)dr[0]["{vchPermiso}"];
            }
            return lsPermiso;
        }

        public static bool getPermiso(string lsOpcMnu, Permiso lpPermiso)
        {
            return getPermiso(lsOpcMnu, lpPermiso.ToString());
        }

        public static bool getPermiso(string lsOpcMnu, string lsPermiso)
        {
            DataTable ldtPermisos;
            KDBAccess kdb = new KDBAccess();
            if (DSODataContext.GetObject("Permiso") == null)
            {
                try
                {
                    ldtPermisos = kdb.GetHisRegByEnt("Permiso", "Permisos", new string[] { "iCodCatalogo", "vchDescripcion", "{OpcionesPermisos}" });
                    DSODataContext.SetObject("Permiso", ldtPermisos);
                }
                catch (Exception e)
                {
                    string lsTitulo = Globals.GetMsgWeb(false, "Permiso");
                    throw new KeytiaWebException(true, "ErrorConsulta", e, lsTitulo);
                }
            }
            else
            {
                ldtPermisos = (DataTable)DSODataContext.GetObject("Permiso");
            }

            int liPermiso = 0;
            DataRow[] lRows = ldtPermisos.Select("vchDescripcion = '" + lsPermiso + "'");
            if (lRows.Length > 0)
            {
                liPermiso = (int)Util.IsDBNull(lRows[0]["{OpcionesPermisos}"], 0);
            }
            int liPermisoOpcMnu = -1;
            lRows = ldtPermisos.Select("vchDescripcion = '" + getPermiso(lsOpcMnu) + "'");
            if (lRows.Length > 0)
            {
                liPermisoOpcMnu = (int)Util.IsDBNull(lRows[0]["{OpcionesPermisos}"], -1);
            }
            return (liPermiso <= liPermisoOpcMnu);
        }

    }
}

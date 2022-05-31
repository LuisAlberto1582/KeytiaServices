/*
Nombre:		    SCB
Fecha:		    2011-10-13
Descripción:	Configuración específica para Usuarios.
Modificación:	
*/
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
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace KeytiaWeb.UserInterface
{
    public class CnfgUsuariosEdit : HistoricEdit
    {
        protected List<Parametro> plstParams = null;

        protected override bool ValidarDatos()
        {
            //Realiza las validaciones genericas
            bool lbRet = base.ValidarDatos();
            if (!lbRet)
                return lbRet;

            if (State != HistoricState.Baja)
            {
                //Validar Datos del usuario
                string lsError;
                StringBuilder lsbErrores = new StringBuilder();
                KeytiaBaseField lField;

                string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloUsuarios"));
                lsbErrores.Length = 0;

                lsError = getValidaUsuario();
                if (lsError.Length > 0)
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, lsError, "4", "32", vchCodigo.Descripcion));
                    lsError = "<span>" + lsError + "</span>";
                    lsbErrores.Append("<li>" + lsError);
                }

                lsError = getValidaPassword().ToString();
                if (lsError.Length > 0)
                {
                    lField = pFields.GetByConfigName("Password");
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, lsError, "8", "32", lField.Descripcion));
                    lsError = "<span>" + lsError + "</span>";
                    lsbErrores.Append("<li>" + lsError);
                }

                lsError = ExiUsuarioEmailPassword();
                if (lsError != "")
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, lsError, vchCodigo.Descripcion));
                    lsError = "<span>" + lsError + "</span>";
                    lsbErrores.Append("<li>" + lsError);
                }

                if (lsbErrores.Length > 0)
                {
                    lbRet = false;
                    lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }
            }

            return lbRet;


        }

        protected string ExiUsuarioEmailPassword()
        {
            string lbret = "";

            Usuarios oUsuario = new Usuarios();

            oUsuario.vchCodUsuario = pvchCodigo.DataValue.ToString().Replace("'", "");
            oUsuario.vchPwdUsuario = getphtValues("Password", "").ToString().Replace("'", "");
            oUsuario.vchEmail = getValCampo("Email", "").ToString();

            lbret = oUsuario.ValUsuarioEmailPassword();

            return lbret;
        }

        protected override void GrabarRegistro()
        {
            string liCodRegistro = iCodRegistro;
            string liCodPerfil = getPerfilUsuario().ToString();
            base.GrabarRegistro();
            GrabarUsuario(liCodRegistro);
            GrabarRestricciones(liCodRegistro, liCodPerfil);
        }

        protected void GrabarUsuario(string liCodRegistro)
        {
            string lsError;
            StringBuilder lsbErrores = new StringBuilder();

            Hashtable lhtValues;
            try
            {
                KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
                int liCodReg = 0;
                string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloUsuarios"));

                lhtValues = ObtenDatosUsuario();

                //Mandar llamar al COM para grabar el usuario 
                if (liCodRegistro == "null")
                {
                    liCodReg = lCargasCOM.GuardaUsuarioEnKeytia(lhtValues, false, false, (int)Session["iCodUsuarioDB"]);
                    if (liCodReg < 0)
                    {
                        //throw new KeytiaWebException("ErrSaveRecord");
                    }
                }
                else
                {
                    liCodReg = int.Parse(liCodRegistro);
                    if (State != HistoricState.Baja)
                    {
                        lhtValues.Add("bBajaUsuario", false);
                    }
                    else
                    {
                        lhtValues.Add("bBajaUsuario", true);
                    }

                    if (!lCargasCOM.GuardaUsuarioEnKeytia(lhtValues, liCodReg, false, false, (int)Session["iCodUsuarioDB"]))
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ErrNoUsuario", vchCodigo.Descripcion));
                        lsError = "<span>" + lsError + "</span>";
                        lsbErrores.Append("<li>" + lsError);

                        lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                        DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                    }

                }

            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveRecord", ex);
            }
        }

        protected Hashtable ObtenDatosUsuario()
        {
            Hashtable lhtValues = new Hashtable();

            string lsEmail = getphtValues("Email", "''").ToString();
            string vchPassword = getphtValues("Password", "''").ToString();

            lhtValues.Add("iCodRegistro", iCodRegistro);
            lhtValues.Add("iCodCatalogo", iCodCatalogo);
            lhtValues.Add("vchCodigo", pvchCodigo.DataValue);
            lhtValues.Add("vchDescripcion", pvchDescripcion.DataValue);
            lhtValues.Add("dtIniVigencia", pdtIniVigencia.Date);
            lhtValues.Add("dtFinVigencia", pdtFinVigencia.Date);
            lhtValues.Add("iCodUsuario", Session["iCodUsuario"]);
            lhtValues.Add("{Email}", lsEmail);
            lhtValues.Add("{UsuarDB}", Session["iCodUsuarioDB"]);
            lhtValues.Add("{Password}", vchPassword);

            return lhtValues;
        }

        private string getValidaUsuario()
        {
            //Validar Codigo de Usuario
            string vchCodUsuario = pvchCodigo.DataValue.ToString().Replace("'", "");

            if (vchCodUsuario.Length < 4 || vchCodUsuario.Length > 32)
                return "ValLength";

            if (!System.Text.RegularExpressions.Regex.IsMatch(vchCodUsuario, "^([a-zA-Z]*[0-9]*[-]*[@]*[_]*[\\.]*)*$"))
                return "ValEmplFormato";

            return "";
        }

        private string getValidaPassword()
        {
            //Validar Datos del usuario
            string lsValue;
            string vchPassword = getphtValues("Password", "").ToString().Replace("'", "");
            string vchConfPassword = getphtValues("ConfPassword", "").ToString().Replace("'", "");

            if (vchPassword != vchConfPassword)
                return "ValConfPassword";

            lsValue = "";
            if (vchPassword.Length > 0)
                lsValue = KeytiaServiceBL.Util.Decrypt(vchPassword);

            if (lsValue.Length < 6 || lsValue.Length > 32) //PT.20131008: se cambio la longitud minima a 6
                return "ValLength";

            if (!System.Text.RegularExpressions.Regex.IsMatch(lsValue, "^([a-zA-Z]*[0-9]*[%]*[$]*[#]*[@]*[+]*[-]*[=]*[&]*)*$"))
                return "ValEmplFormato";

            return "";

        }

        protected Object getphtValues(String lsCampo, Object defaultValue)
        {
            Object loValue;
            KeytiaBaseField lField;

            if (!pFields.ContainsConfigName(lsCampo) || pFields.GetByConfigName(lsCampo).DataValue == "null")
            {
                return defaultValue;
            }

            lField = ((KeytiaBaseField)pFields.GetByConfigName(lsCampo));

            if (!phtValues.Contains(lField.Column) || phtValues[lField.Column] == "null")
            {
                return defaultValue;
            }

            loValue = Util.IsDBNull(phtValues[lField.Column], defaultValue);

            return loValue;

        }

        protected Object getValCampo(String lsCampo, Object defaultValue)
        {
            Object loValue;
            if (!pFields.ContainsConfigName(lsCampo) || pFields.GetByConfigName(lsCampo).DataValue == "null")
            {
                return defaultValue;
            }
            if (defaultValue == "")
            {
                DSOTextBox ltxtContenido = (DSOTextBox)pFields.GetByConfigName(lsCampo).DSOControlDB;
                loValue = Util.IsDBNull(ltxtContenido.TextBox.Text, defaultValue);
            }
            else
            {
                loValue = Util.IsDBNull(pFields.GetByConfigName(lsCampo).DataValue, defaultValue);
            }
            return loValue;
        }

        protected virtual void InizializaCampos()
        {

            // Inicializa el Esquema del usuario
            int liCtx = getUsuarDB();

            if (pFields.ContainsConfigName("UsuarDB"))
            {
                pFields.GetByConfigName("UsuarDB").DataValue = liCtx;
            }
        }

        protected override void AgregarRegistro()
        {
            base.AgregarRegistro();
            InizializaCampos();
            BloqueaCampos();
        }

        protected override void EditarRegistro()
        {
            base.EditarRegistro();
            BloqueaCampos();
        }

        protected virtual void BloqueaCampos()
        {
            if (pFields.ContainsConfigName("UsuarDB") && !DSODataContext.Schema.Equals("Keytia"))
            {
                pFields.GetByConfigName("UsuarDB").DisableField();
            }

            if (pFields.ContainsConfigName("UltAcc"))
            {
                pFields.GetByConfigName("UltAcc").DisableField();
            }
        }

        protected override void InitGrid()
        {
            base.InitGrid();
            if (!DSODataContext.Schema.Equals("Keytia", StringComparison.CurrentCultureIgnoreCase))
            {
                plstParams = new List<Parametro>();
                Parametro lParam = new Parametro();
                lParam.Name = "UsuarDB";
                lParam.Value = getUsuarDB();
                plstParams.Add(lParam);

                pHisGrid.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetVisHistoricoParam");
                pHisGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerVisHistoricoParam(this, sSource, aoData, fnCallback," + DSOControl.SerializeJSON<List<Parametro>>(plstParams) + ");}";
            }
        }

        protected override void InitGridFields()
        {
            if (!DSODataContext.Schema.Equals("Keytia", StringComparison.CurrentCultureIgnoreCase))
            {
                DSOGridClientColumn lCol;
                int lTarget = pHisGrid.Config.aoColumnDefs.Count;
                foreach (KeytiaBaseField lField in pFields)
                {
                    if (lField.ShowInGrid)
                    {
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
                }
            }
            else
            {
                base.InitGridFields();
            }
        }

        protected override void InitHisGridLanguage()
        {
            if (DSODataContext.Schema.Equals("Keytia", StringComparison.CurrentCultureIgnoreCase))
            {
                base.InitHisGridLanguage();
            }
            else
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
        }

        protected override void CnfgSubHistoricField_PostGrabarClick(object sender, EventArgs e)
        {
            ActualizarRestricciones();
            base.CnfgSubHistoricField_PostGrabarClick(sender, e);
        }

        protected void ActualizarRestricciones()
        {
            KeytiaCOM.ICargasCOM pCargaCom = (KeytiaCOM.ICargasCOM)Marshal.BindToMoniker("queue:/new:KeytiaCOM.CargasCOM");
            int liCodUsuario = (int)Session["iCodUsuarioDB"];

            string liCodPerfil = "0";
            if (pFields.ContainsConfigName("Perfil") && pFields.GetByConfigName("Perfil").DSOControlDB.HasValue)
            {
                liCodPerfil = pFields.GetByConfigName("Perfil").DataValue.ToString();
            }

            switch (pSubHistorico.vchDesMaestro)
            {
                case "Centro de Costos":
                    pCargaCom.ActualizaRestUsuario(iCodCatalogo, liCodPerfil, "CenCos", "RestCenCos", liCodUsuario);
                    break;
                case "Empleados":
                    pCargaCom.ActualizaRestUsuario(iCodCatalogo, liCodPerfil, "Emple", "RestEmple", liCodUsuario);
                    break;
                case "Sitios":
                    pCargaCom.ActualizaRestUsuario(iCodCatalogo, liCodPerfil, "Sitio", "RestSitio", liCodUsuario);
                    break;
            }
            Marshal.ReleaseComObject(pCargaCom);
            pCargaCom = null;
        }

        protected void GrabarRestricciones(string liCodRegistro, string liCodPerfilAnt)
        {
            string liCodPerfil = "0";
            if (pFields.ContainsConfigName("Perfil") && pFields.GetByConfigName("Perfil").DSOControlDB.HasValue)
            {
                liCodPerfil = pFields.GetByConfigName("Perfil").DataValue.ToString();
            }

            if (liCodRegistro == "null" || liCodPerfil != liCodPerfilAnt)
            {
                KeytiaCOM.ICargasCOM pCargaCom = (KeytiaCOM.ICargasCOM)Marshal.BindToMoniker("queue:/new:KeytiaCOM.CargasCOM");
                int liCodUsuario = (int)Session["iCodUsuarioDB"];

                pCargaCom.ActualizaRestUsuario(iCodCatalogo, liCodPerfil, "CenCos", "RestCenCos", liCodUsuario);
                pCargaCom.ActualizaRestUsuario(iCodCatalogo, liCodPerfil, "Sitio", "RestSitio", liCodUsuario);

                Marshal.ReleaseComObject(pCargaCom);
                pCargaCom = null;
            }
        }

        protected int getUsuarDB()
        {
            return (int)DSODataAccess.ExecuteScalar(
                "select UsuarDB = IsNull(UsuarDB, 0) from [VisHistoricos('Usuar','" + vchDesMaestro + "','Español')] " + "\r\n" +
                "where dtIniVigencia <> dtFinVigencia" + "\r\n" +
                "and dtIniVigencia <= '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "\r\n" +
                "and dtFinVigencia >  '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "'" + "\r\n" +
                "and iCodCatalogo = " + Session["iCodUsuario"].ToString(), (object)0);
        }

        protected int getPerfilUsuario()
        {
            return (int)DSODataAccess.ExecuteScalar(
                "select Perfil = IsNull(Perfil, 0) from [VisHistoricos('Usuar','" + vchDesMaestro + "','Español')] " + "\r\n" +
                "where iCodRegistro = " + iCodRegistro, (object)0);
        }
    }
}

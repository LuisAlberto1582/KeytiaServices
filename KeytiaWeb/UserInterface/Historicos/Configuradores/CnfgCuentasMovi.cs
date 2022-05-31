using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DSOControls2008;
using KeytiaServiceBL;
using SeeYouOnServiceBL;
using System.Text;
using System.Data;

using System.Runtime.InteropServices;

namespace KeytiaWeb.UserInterface
{
    public class CnfgCuentasMovi : HistoricEdit
    {
        //TODO: Validar que en el grid se vea unicamente los datos del cliente del usuario cuando no sea de perfil SeeYouOnAdmin

        public CnfgCuentasMovi()
        {
            Init += new EventHandler(CnfgCuentasMovi_Init);
        }

        void CnfgCuentasMovi_Init(object sender, EventArgs e)
        {
            this.CssClass = "HistoricEdit CnfgCuentasMovi";
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);

            OcultaCampo("Password");
            OcultaCampo("CuentaMovi");
            OcultaCampo("DireccionLDAP");
            OcultaCampo("TMSSysId");

        }

        protected override void EditarRegistro()
        {
            base.EditarRegistro();

            if (State == HistoricState.Edicion)
            {
                KeytiaBaseField lfUsuarioTMS = pFields.GetByConfigName("UsuarioTMS");
                if (iCodRegistro != "null")
                {
                    lfUsuarioTMS.DisableField();
                }
                else
                {
                    lfUsuarioTMS.EnableField();
                }
            }
        }

        protected override void InitFiltros()
        {
            base.InitFiltros();
            OcultaCampoFiltro("Password");
            OcultaCampoFiltro("CuentaMovi");
            OcultaCampoFiltro("DireccionLDAP");
            OcultaCampoFiltro("TMSSysId");
        }

        protected override bool ValidarDatos()
        {
            bool lbRet = base.ValidarDatos();

            if (lbRet)
            {
                if (State == HistoricState.Edicion)
                {
                    KeytiaBaseField lfUsuarioTMS = pFields.GetByConfigName("UsuarioTMS");
                    // Si estamos creando un nuevo registro
                    if (iCodRegistro == "null")
                    {
                        if (lfUsuarioTMS != null)
                        {
                            string lsUsuarioTMS = lfUsuarioTMS.DataValue.ToString();
                            // No permitimos que se ingrese el caracter @
                            if (lsUsuarioTMS.Contains("@"))
                            {
                                string[] lsParametros = new string[2];
                                lsParametros[0] = lfUsuarioTMS.Descripcion;
                                lsParametros[1] = "@";
                                lbRet = false;
                                string lsTitulo = DSOControl.JScriptEncode(this.AlertTitle);
                                string lsMsg = "<span>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoCaracterInvalido", lsParametros));
                                DSOControl.jAlert(Page, pjsObj + ".ErrTMSMoviUsuarioArroba", lsMsg, lsTitulo);
                            }
                        }
                    }
                    // Si estamos editando un registro
                    else
                    {
                        if (lfUsuarioTMS != null)
                        {
                            string lsUsuarioTMS = lfUsuarioTMS.DataValue.ToString();
                            // Si la cuenta movi fue creada en el TMS e importada a SYO
                            // el campo UsuarioTMS tendrá una @, y esto trae problemas
                            // cuando se actualizan este tipo de cuentas en SYO,
                            // por eso eliminamos todo lo que esté después de la @
                            if (lsUsuarioTMS.Contains("@"))
                            {
                                lsUsuarioTMS = lsUsuarioTMS.Substring(0, lsUsuarioTMS.IndexOf("@"));
                                phtValues[lfUsuarioTMS.Column] = lsUsuarioTMS + "'";
                            }
                        }
                    }
                }
            }
            return lbRet;
        }

        protected override void GrabarRegistro()
        {

            try
            {
                SeeYouOnCOM.SyncCOM lSyncCOM = new SeeYouOnCOM.SyncCOM();
                if (State != HistoricState.Baja)
                {
                    base.GrabarRegistro();

                    lSyncCOM.TMSMoviSave(int.Parse(iCodCatalogo), (int)Session["iCodUsuarioDB"]);

                    //Mandar llamar el envio de la cuenta de correo
                    int liCodCatalogo;
                    if (int.TryParse(iCodCatalogo, out liCodCatalogo))
                    {
                        SeeYouOnCOM.ISyncCOM lSyncComMOVI = (SeeYouOnCOM.ISyncCOM)Marshal.BindToMoniker("queue:/new:SeeYouOnCOM.SyncCOM");
                        lSyncComMOVI.EnviaCtaMOVI(liCodCatalogo, Globals.GetCurrentLanguage(),
                            Session["StyleSheet"].ToString(), (int)Session["iCodUsuarioDB"]);
                        Marshal.ReleaseComObject(lSyncComMOVI);
                        Util.LogMessage("Se ha programado el envío de correo de cuenta Movi " + pvchDescripcion + " (" + pvchCodigo + ").");
                        lSyncComMOVI = null;
                    }
                }
                else
                {
                    //Checar si la fecha de baja es menor o igual a la de hoy.
                    if (pdtFinVigencia.HasValue && pdtFinVigencia.Date < DateTime.Now)
                        lSyncCOM.TMSMoviDelete(int.Parse(iCodCatalogo), (int)Session["iCodUsuarioDB"]);
                    base.GrabarRegistro();
                }


            }
            catch (Exception ex)
            {
                string lsTitulo = DSOControl.JScriptEncode(this.AlertTitle);
                string lsMsg = "<span>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ErrTMSMoviUpdate"));
                DSOControl.jAlert(Page, pjsObj + ".ErrTMSMoviUpdate", lsMsg, lsTitulo);
                Util.LogException(ex);
            }
        }
    }
}

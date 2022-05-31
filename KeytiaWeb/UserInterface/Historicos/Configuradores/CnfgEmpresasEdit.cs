using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using KeytiaServiceBL;
using System.Text;
using DSOControls2008;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Runtime.InteropServices;

namespace KeytiaWeb.UserInterface
{
    public class CnfgEmpresasEdit : HistoricEdit
    {
        protected HtmlButton pbtnNotifPrep;

        public CnfgEmpresasEdit()
        {
            Init += new EventHandler(CnfgEmpresasEdit_Init);
        }

        void CnfgEmpresasEdit_Init(object sender, EventArgs e)
        {
            this.CssClass = "HistoricEdit CnfgEmpresasEdit";
            pbtnNotifPrep = new HtmlButton();

            pToolBar.Controls.Add(pbtnNotifPrep);
        }

        protected override void InitAcciones()
        {
            base.InitAcciones();

            pbtnNotifPrep.ID = "btnNotifPrep";
            pbtnNotifPrep.Attributes["class"] = "buttonEdit";
            pbtnNotifPrep.Style["display"] = "none";

            pbtnNotifPrep.ServerClick += new EventHandler(pbtnNotifPrep_ServerClick);

        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);
            pbtnNotifPrep.Visible = false;

            if (s == HistoricState.Consulta)
            {
                if (((int)pFields.GetByConfigName("BanderasEmpre").DataValue & 1) == 1)
                {
                    pbtnNotifPrep.Visible = true;
                }
            }
        }

        protected virtual void pbtnNotifPrep_ServerClick(object sender, EventArgs e)
        {
            FillAjaxControls();

            PrevState = State;
            SubHistoricClass = "KeytiaWeb.UserInterface.CnfgNotifPrepEmpre";
            SubCollectionClass = "KeytiaWeb.UserInterface.CnfgNotifPrepEmpreFieldCollection";

            InitSubHistorico(this.ID + "CnfgNotifPrepEmpre");

            pSubHistorico.SetEntidad("NotifPrepEmpre");
            pSubHistorico.SetMaestro("Notificaciones de Presupuestos de Empresas");

            pSubHistorico.EsSubHistorico = true;
            pSubHistorico.FillControls();

            SetHistoricState(HistoricState.CnfgSubHistoricField);
            pSubHistorico.InitMaestro();

            DateTime ldtAhora = DateTime.Now;
            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.AppendLine("select iCodRegistro");
            lsbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('NotifPrepEmpre','Notificaciones de Presupuestos de Empresas','" + Globals.GetCurrentLanguage() + "')] PrepEmpre");
            lsbQuery.AppendLine("where PrepEmpre.Empre = " + iCodCatalogo);
            lsbQuery.AppendLine("and PrepEmpre.dtIniVigencia <> PrepEmpre.dtFinVigencia");
            lsbQuery.AppendLine("and PrepEmpre.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsbQuery.AppendLine("and PrepEmpre.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");

            DataTable lDataTable = DSODataAccess.Execute(lsbQuery.ToString());

            if (lDataTable.Rows.Count > 0)
            {
                pSubHistorico.iCodRegistro = (lDataTable.Rows[0]["iCodRegistro"]).ToString();
                pSubHistorico.ConsultarRegistro();
            }


            pSubHistorico.Fields.EnableFields();

            pSubHistorico.Fields.GetByConfigName(vchCodEntidad).DataValue = iCodCatalogo;
            pSubHistorico.Fields.GetByConfigName(vchCodEntidad).DisableField();

            pSubHistorico.Fields.GetByConfigName("EliminarConfigPresupuesto").DataValue = 0;

            pSubHistorico.SetHistoricState(HistoricState.Edicion);
        }

        protected override bool ValidarDatos()
        {
            bool lbRet = true;
            if (State == HistoricState.Baja)
            {
                lbRet = ValidarFechaBaja();
            }
            else
            {
                lbRet = ValidarDescripcion();
            }

            return base.ValidarDatos() && lbRet;
        }

        protected bool ValidarDescripcion()
        {
            bool lbRet = true;
            string lsError;
            StringBuilder lsbErrores = new StringBuilder();
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloEmpresas"));

            try
            {
                psbQuery.Length = 0;
                psbQuery.AppendLine("select iCodRegistro = isnull(count(iCodRegistro),0)");
                psbQuery.AppendLine("from [VisHistoricos('Empre','Empresas','" + Globals.GetCurrentLanguage() + "')]");
                psbQuery.AppendLine("where vchDescripcion = " + pvchDescripcion.DataValue);
                psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia");
                psbQuery.AppendLine("and ((dtIniVigencia <= " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("      and dtFinVigencia > " + pdtIniVigencia.DataValue + ")");
                psbQuery.AppendLine("  or (dtIniVigencia <= " + pdtFinVigencia.DataValue);
                psbQuery.AppendLine("      and dtFinVigencia > " + pdtFinVigencia.DataValue + ")");
                psbQuery.AppendLine("  or (dtIniVigencia >= " + pdtIniVigencia.DataValue);
                psbQuery.AppendLine("      and dtFinVigencia <= " + pdtFinVigencia.DataValue + "))");

                if (pFields.GetByConfigName("Client").DSOControlDB.HasValue)
                {
                    psbQuery.AppendLine("and Client = " + pFields.GetByConfigName("Client").DataValue);
                }

                if (iCodCatalogo != "null")
                {
                    psbQuery.AppendLine("and iCodCatalogo <> " + iCodCatalogo);
                }

                int liCountEmpre = (int)DSODataAccess.ExecuteScalar(psbQuery.ToString());

                if (liCountEmpre > 0)
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "DescEmpreRep", pvchDescripcion.DataValue.ToString().Replace("'", "")));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
            if (lsbErrores.Length > 0)
            {
                lbRet = false;
                lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
            }

            return lbRet;
        }

        protected bool ValidarFechaBaja()
        {
            bool lbRet = true;
            string lsQuery = "";
            string lsError;
            StringBuilder lsbErrores = new StringBuilder();
            string lsTitulo = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "TituloEmpresas"));
            int lRegRel;

            try
            {
                psbQuery.Length = 0;
                psbQuery.AppendLine("select iCodRegistro = isnull(count(iCodRegistro),0)");
                psbQuery.AppendLine("from @Vista");
                psbQuery.AppendLine("where Empre = " + iCodCatalogo);
                psbQuery.AppendLine("and dtIniVigencia <> dtFinVigencia");
                psbQuery.AppendLine("and dtIniVigencia <= " + pdtFinVigencia.DataValue);
                psbQuery.AppendLine("and dtFinVigencia > " + pdtFinVigencia.DataValue);

                #region Validacion en Centros de Costos Relacionados
                lsQuery = psbQuery.ToString().Replace("@Vista", "[VisHistoricos('CenCos','Español')]");
                lRegRel = (int)DSODataAccess.ExecuteScalar(lsQuery);
                if (lRegRel > 0)
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "EmpreRegActivo", Globals.GetMsgWeb(false, "TituloCentrosdeCostos")));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }
                #endregion

                #region Validacion en Sitios Relacionados
                lsQuery = psbQuery.ToString().Replace("@Vista", "[VisHistoricos('Sitio','Español')]");
                lRegRel = (int)DSODataAccess.ExecuteScalar(lsQuery);
                if (lRegRel > 0)
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "EmpreRegActivo", Globals.GetMsgWeb(false, "TituloSitios")));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }
                #endregion

                #region Validacion en Usuarios Relacionados
                lsQuery = psbQuery.ToString().Replace("@Vista", "[VisHistoricos('Usuar','Español')]");
                lRegRel = (int)DSODataAccess.ExecuteScalar(lsQuery);
                if (lRegRel > 0)
                {
                    lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "EmpreRegActivo", Globals.GetMsgWeb(false, "TituloUsuarios")));
                    lsbErrores.Append("<li>" + lsError + "</li>");
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }

            if (lsbErrores.Length > 0)
            {
                lbRet = false;
                lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
            }

            return lbRet;
        }

        public override void InitLanguage()
        {
            base.InitLanguage();

            pbtnNotifPrep.InnerText = Globals.GetMsgWeb(false, "btnNotifPrep");
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

            switch (pSubHistorico.vchDesMaestro)
            {
                case "Centro de Costos":
                    pCargaCom.ActualizaRestEmpresa(iCodCatalogo, "CenCos", "RestCenCos", liCodUsuario);
                    break;
                case "Empleados":
                    pCargaCom.ActualizaRestEmpresa(iCodCatalogo, "Emple", "RestEmple", liCodUsuario);
                    break;
                case "Sitios":
                    pCargaCom.ActualizaRestEmpresa(iCodCatalogo, "Sitio", "RestSitio", liCodUsuario);
                    break;
            }
            Marshal.ReleaseComObject(pCargaCom);
            pCargaCom = null;
        }

    }
}

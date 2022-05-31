using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Web;
using KeytiaServiceBL;
using System.Text;
using DSOControls2008;
using System.Data;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.HtmlControls;

namespace KeytiaWeb.UserInterface
{
    public class CnfgNotifPrepSitio : HistoricEdit
    {
        protected StringBuilder psbErrores;
        protected HtmlButton pbtnPrepSitio;
        protected HtmlButton pbtnPrepSitioBajas;

        public CnfgNotifPrepSitio()
        {
            Init += new EventHandler(CnfgNotifPrepSitio_Init);
        }

        void CnfgNotifPrepSitio_Init(object sender, EventArgs e)
        {
            this.CssClass = "HistoricEdit CnfgNotifPrepSitio";
            pbtnPrepSitio = new HtmlButton();
            pbtnPrepSitioBajas = new HtmlButton();

            pToolBar.Controls.Add(pbtnPrepSitio);
            pToolBar.Controls.Add(pbtnPrepSitioBajas);
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);
            pExpRegistro.Visible = false;
            pbtnPrepSitio.Visible = false;
            pbtnPrepSitioBajas.Visible = false;

            if (s == HistoricState.Edicion)
            {
                if (iCodCatalogo != "null")
                {
                    pbtnPrepSitio.Visible = true;
                    pbtnPrepSitioBajas.Visible = true;
                }
                pFields.EnableFields();
                pFields.GetByConfigName("Sitio").DisableField();
                pFields.GetByConfigName("SigAct").DisableField();
            }
        }

        protected override void InitAcciones()
        {
            base.InitAcciones();

            pbtnPrepSitio.ID = "btnPrepSitio";
            pbtnPrepSitio.Attributes["class"] = "buttonEdit";
            pbtnPrepSitio.Style["display"] = "none";

            pbtnPrepSitio.ServerClick += new EventHandler(pbtnPrepSitio_ServerClick);

            pbtnPrepSitioBajas.ID = "btnPrepSitioBajas";
            pbtnPrepSitioBajas.Attributes["class"] = "buttonEdit";
            pbtnPrepSitioBajas.Style["display"] = "none";

            pbtnPrepSitioBajas.ServerClick += new EventHandler(pbtnPrepSitioBajas_ServerClick);
        }

        protected virtual void pbtnPrepSitio_ServerClick(object sender, EventArgs e)
        {
            InitPrepSitio();
        }

        protected virtual void pbtnPrepSitioBajas_ServerClick(object sender, EventArgs e)
        {
            FillAjaxControls();

            PrevState = State;
            SubHistoricClass = "KeytiaWeb.UserInterface.CnfgPrepSitioEnvio";
            SubCollectionClass = "KeytiaWeb.UserInterface.HistoricFieldCollection";

            InitSubHistorico(this.ID + "CnfgPrepSitioEnvio");

            pSubHistorico.SetEntidad("PrepSitio");
            pSubHistorico.SetMaestro("Notificación recursos dados de baja");

            pSubHistorico.EsSubHistorico = true;
            pSubHistorico.FillControls();

            SetHistoricState(HistoricState.CnfgSubHistoricField);
            pSubHistorico.InitMaestro();

            DateTime ldtAhora = DateTime.Now;
            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.AppendLine("select iCodRegistro");
            lsbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('PrepSitio','Notificación recursos dados de baja','" + Globals.GetCurrentLanguage() + "')] PrepSitio");
            lsbQuery.AppendLine("where PrepSitio.NotifPrepSitio = " + iCodCatalogo);
            lsbQuery.AppendLine("and PrepSitio.dtIniVigencia <> PrepSitio.dtFinVigencia");
            lsbQuery.AppendLine("and PrepSitio.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsbQuery.AppendLine("and PrepSitio.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");

            DataTable lDataTable = DSODataAccess.Execute(lsbQuery.ToString());

            if (lDataTable.Rows.Count > 0)
            {
                pSubHistorico.iCodRegistro = (lDataTable.Rows[0]["iCodRegistro"]).ToString();
                pSubHistorico.ConsultarRegistro();
            }

            pSubHistorico.Fields.EnableFields();

            pSubHistorico.Fields.GetByConfigName(vchCodEntidad).DataValue = iCodCatalogo;
            pSubHistorico.Fields.GetByConfigName(vchCodEntidad).DisableField();

            pSubHistorico.SetHistoricState(HistoricState.Edicion);
            pSubHistorico.vchCodTitle = "btnPrepSitioBajas";
        }

        public virtual void ConfigPrepSitio()
        {
            string lsTitulo = DSOControl.JScriptEncode(AlertTitle);
            string lsMsg = "<span>" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ConfigPrepSitio")) + "</span>";
            DSOControl.jAlert(Page, pjsObj + ".ConfigPrepSitio", lsMsg, lsTitulo);

            InitPrepSitio();
        }

        protected virtual void InitPrepSitio()
        {
            FillAjaxControls();

            PrevState = State;
            SubHistoricClass = "KeytiaWeb.UserInterface.CnfgPrepSitioEnvio";
            SubCollectionClass = "KeytiaWeb.UserInterface.HistoricFieldCollection";

            InitSubHistorico(this.ID + "CnfgPrepSitioEnvio");

            pSubHistorico.SetEntidad("PrepSitio");
            pSubHistorico.SetMaestro("Notificación por Presupuesto Temporal a Empleado");

            pSubHistorico.EsSubHistorico = true;
            pSubHistorico.FillControls();

            SetHistoricState(HistoricState.CnfgSubHistoricField);
            pSubHistorico.InitMaestro();

            DateTime ldtAhora = DateTime.Now;
            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.AppendLine("select iCodRegistro");
            lsbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('PrepSitio','Notificación por Presupuesto Temporal a Empleado','" + Globals.GetCurrentLanguage() + "')] PrepSitio");
            lsbQuery.AppendLine("where PrepSitio.NotifPrepSitio = " + iCodCatalogo);
            lsbQuery.AppendLine("and PrepSitio.dtIniVigencia <> PrepSitio.dtFinVigencia");
            lsbQuery.AppendLine("and PrepSitio.dtInivigencia <= '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsbQuery.AppendLine("and PrepSitio.dtFinVigencia > '" + ldtAhora.ToString("yyyy-MM-dd HH:mm:ss") + "'");

            DataTable lDataTable = DSODataAccess.Execute(lsbQuery.ToString());

            if (lDataTable.Rows.Count > 0)
            {
                pSubHistorico.iCodRegistro = (lDataTable.Rows[0]["iCodRegistro"]).ToString();
                pSubHistorico.ConsultarRegistro();
            }

            pSubHistorico.Fields.EnableFields();

            pSubHistorico.Fields.GetByConfigName(vchCodEntidad).DataValue = iCodCatalogo;
            pSubHistorico.Fields.GetByConfigName(vchCodEntidad).DisableField();

            pSubHistorico.SetHistoricState(HistoricState.Edicion);
            pSubHistorico.vchCodTitle = "btnPrepSitio";
        }

        protected override bool ValidarVigencias()
        {
            if (State == HistoricState.Edicion)
            {
                pdtIniVigencia.DataValue = DateTime.Today;
                pdtFinVigencia.DataValue = new DateTime(2079, 1, 1);
            }

            //el usuario nunca edita las vigencias por lo que esta validacion no es necesaria
            return true;
        }

        protected override bool ValidarClaves()
        {
            //el usuario nunca edita las claves por lo que esta validacion no es necesaria
            return true;
        }

        protected override bool ValidarAtribCatalogosVig()
        {
            //el usuario solamente puede seleccionar catalogos de entidades que estan vigentes todo el tiempo
            return true;
        }

        protected override bool ValidarDatos()
        {
            bool lbret = true;
            string lsTitulo = DSOControl.JScriptEncode(AlertTitle);
            string lsError;

            try
            {
                psbErrores = new StringBuilder();

                pvchCodigo.DataValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                pvchDescripcion.DataValue = pFields.GetByConfigName("Sitio").ToString();

                if ((int.Parse(pFields.GetByConfigName("BanderasPrepSitios").DataValue.ToString()) & 1) == 1)
                {
                    if (!pFields.GetByConfigName("HoraAlarma").DSOControlDB.HasValue)
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", pFields.GetByConfigName("HoraAlarma").Descripcion));
                        psbErrores.Append("<li>" + lsError + "</li>");
                    }
                    else if (!pFields.GetByConfigName("Minutos").DSOControlDB.HasValue)
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "CampoRequerido", pFields.GetByConfigName("Minutos").Descripcion));
                        psbErrores.Append("<li>" + lsError + "</li>");
                    }
                    else if (int.Parse(pFields.GetByConfigName("Minutos").DataValue.ToString()) < 1)
                    {
                        lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ValIntervaloMinutos"));
                        psbErrores.Append("<li>" + lsError + "</li>");
                    }
                    else
                    {
                        //Calcular la hora siguiente para generar el archivo de altas por presupuestos temporales
                        DateTime ldtSigAct = DateTime.Today;
                        ldtSigAct = ldtSigAct.AddHours(((DSODateTimeBox)pFields.GetByConfigName("HoraAlarma").DSOControlDB).Date.Hour);
                        ldtSigAct = ldtSigAct.AddMinutes(((DSODateTimeBox)pFields.GetByConfigName("HoraAlarma").DSOControlDB).Date.Minute);
                        ldtSigAct = ldtSigAct.AddSeconds(((DSODateTimeBox)pFields.GetByConfigName("HoraAlarma").DSOControlDB).Date.Second);
                        DateTime ldtAhora = DateTime.Now;
                        int liMinutos = int.Parse(pFields.GetByConfigName("Minutos").DataValue.ToString());
                        while (ldtSigAct < ldtAhora)
                        {
                            ldtSigAct = ldtSigAct.AddMinutes(liMinutos);
                        }
                        pFields.GetByConfigName("SigAct").DataValue = ldtSigAct;
                    }
                }

                if (psbErrores.Length > 0)
                {
                    lbret = false;
                    lsError = "<ul>" + psbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, lsTitulo);
                }

                phtValues = pFields.GetValues();
                pdsRelValues = pFields.GetRelationValues();
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }

            return lbret;
        }

        public override void InitLanguage()
        {
            base.InitLanguage();

            pbtnPrepSitio.InnerText = Globals.GetMsgWeb(false, "btnPrepSitio");
            pbtnPrepSitioBajas.InnerText = Globals.GetMsgWeb(false, "btnPrepSitioBajas");
        }
    }
}

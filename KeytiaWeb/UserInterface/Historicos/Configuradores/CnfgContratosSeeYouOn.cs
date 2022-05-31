using System;
using System.Linq;
using System.Web;
using DSOControls2008;
using System.Text;

namespace KeytiaWeb.UserInterface
{
    public class CnfgContratosSeeYouOn : HistoricEdit
    {
        protected StringBuilder psbErrores;
        public CnfgContratosSeeYouOn()
        {
            Init += new EventHandler(CnfgContratosSeeYouOn_Init);
        }

        void CnfgContratosSeeYouOn_Init(object sender, EventArgs e)
        {
            this.CssClass = "HistoricEdit CnfgContratosSeeYouOn";
        }

        protected override bool ValidarDatos()
        {
            bool lbRet = base.ValidarDatos();
            if (State != HistoricState.Baja && lbRet)
            {
                psbErrores = new StringBuilder();

                ValidarFechasContrato();

                if (psbErrores.Length > 0)
                {
                    lbRet = false;
                    string lsError = "<ul>" + psbErrores.ToString() + "</ul>";
                    DSOControl.jAlert(Page, pjsObj + ".ValidarRegistro", lsError, DSOControl.JScriptEncode(Title));
                }
                psbErrores = null;
            }
            return lbRet;
        }

        protected void ValidarFechasContrato()
        {
            if (pFields.ContainsConfigName("FechaInicioContrato") && pFields.ContainsConfigName("FechaFinContrato"))
            {
                KeytiaBaseField lFieldIni = pFields.GetByConfigName("FechaInicioContrato");
                KeytiaBaseField lFieldFin = pFields.GetByConfigName("FechaFinContrato");
                if (lFieldIni.DSOControlDB.HasValue && lFieldFin.DSOControlDB.HasValue)
                {
                    DateTime ldtFechaInicioContrato = ((DSODateTimeBox)lFieldIni.DSOControlDB).Date;
                    DateTime ldtFechaFinContrato = ((DSODateTimeBox)lFieldFin.DSOControlDB).Date;
                    if (ldtFechaInicioContrato.CompareTo(ldtFechaFinContrato) > 0)
                    {
                        //Fecha de Inicio debe ser menor a Fecha de Fin
                        string lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "FechasEquivocadas",
                            lFieldIni.Descripcion,
                            lFieldFin.Descripcion));
                        psbErrores.Append("<li>" + lsError + "</li>");
                    }
                }
            }
        }
    }
}

using System;
using System.Data;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Text;
using System.Web;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;

namespace KeytiaWeb.UserInterface
{
    public class CnfgAsistentesFieldCollection : HistoricFieldCollection
    {
        public CnfgAsistentesFieldCollection()
        {
        }

        public CnfgAsistentesFieldCollection(WebControl lContainer, int liCodEntidad, int liCodMaestro, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
            : base(lContainer, liCodEntidad, liCodMaestro, lTablaAtributos, lValidarPermiso) { }

        public CnfgAsistentesFieldCollection(int liCodEntidad, int liCodMaestro)
            : base(liCodEntidad, liCodMaestro, true) { }

        public override void InitFields()
        {
            base.InitFields();
            if (ContainsConfigName("Emple"))
            {
                string liCodClient = getCliente();
                KeytiaAutoCompleteField lFieldEmple = (KeytiaAutoCompleteField)GetByConfigName("Emple");
                ((DSOAutocomplete)lFieldEmple.DSOControlDB).Source = pContainer.ResolveUrl("~/WebMethods.aspx/SearchEmpleByClient");
                ((DSOAutocomplete)lFieldEmple.DSOControlDB).FnSearch = "function(request, response){" + lFieldEmple.JsObj + ".fnSearchEmpleByClient(this, request, response); }";
                lFieldEmple.DSOControlDB.AddClientEvent("clienteId", liCodClient);
            }
        }

        private string getCliente()
        {
            string liCodClient = "";
            HistoricEdit lCnfgAsistentes = (HistoricEdit)pContainer;
            if (lCnfgAsistentes.Historico != null)
            {
                HistoricEdit lCnfgParticipantes = lCnfgAsistentes.Historico;
                if (lCnfgParticipantes.Historico != null)
                {
                    HistoricEdit lCnfgConferencias = lCnfgParticipantes.Historico;
                    if (lCnfgConferencias.Fields != null && lCnfgConferencias.Fields.ContainsConfigName("Client"))
                    {
                        liCodClient = lCnfgConferencias.Fields.GetByConfigName("Client").DataValue.ToString();
                    }
                }
            }
            else
            {
                if (lCnfgAsistentes.Fields.ContainsConfigName("Participante"))
                {
                    liCodClient = DSODataAccess.ExecuteScalar(
                        "select TMSConf.Client" + "\r\n" +
                        "from [" + DSODataContext.Schema + "].[VisHistoricos('TMSConf','Conferencia','Español')] TMSConf," + "\r\n" +
                        "     [" + DSODataContext.Schema + "].[VisHistoricos('Participante','Participante','Español')] Participante" + "\r\n" +
                        "where Participante.iCodCatalogo = " + lCnfgAsistentes.Fields.GetByConfigName("Participante").DataValue.ToString() + "\r\n" +
                        "and Participante.TMSConf = TMSConf.iCodCatalogo" + "\r\n" +
                        "and Participante.dtIniVigencia <> Participante.dtFinVigencia" + "\r\n" +
                        DSOControl.ComplementaVigenciasJS(lCnfgAsistentes.dtIniVigencia.Date, lCnfgAsistentes.dtFinVigencia.Date, false, "Participante") + "\r\n" +
                        "and TMSConf.dtIniVigencia <> TMSConf.dtFinVigencia" + "\r\n" +
                        DSOControl.ComplementaVigenciasJS(lCnfgAsistentes.dtIniVigencia.Date, lCnfgAsistentes.dtFinVigencia.Date, false, "TMSConf") + "\r\n", (object)"0").ToString();
                }
            }
            if (string.IsNullOrEmpty(liCodClient))
            {
                liCodClient = "null";
            }
            return liCodClient;
        }


    }
}

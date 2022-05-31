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
using System.Runtime.InteropServices;

namespace KeytiaWeb.UserInterface
{
    public class CargasWebCentrosCosto : CargasWebEmpleados
    {

        public override void FillAjaxControls(bool lbIncluirFechaFin)
        {
            //Metodo para llenar controles cuyo valor puede cambiar mediante ajax
            psbQuery.Length = 0;
            psbQuery.AppendLine("Select iCodRegistro, vchDescripcion from Maestros ");
            psbQuery.AppendLine("Where iCodEntidad = " + piCodEntidad.DataValue.ToString());
            psbQuery.AppendLine(" and vchDescripcion like '%Carga%Centro%Costo%' ");
            psbQuery.AppendLine(" and dtIniVigencia <> dtFinVigencia");
            psbQuery.AppendLine(" order by vchDescripcion");

            piCodMaestro.DataSource = psbQuery.ToString();
            piCodMaestro.Fill();

            if (!pbEnableMaestro)
            {
                piCodMaestro.DataValue = iCodMaestro;
                if (State == HistoricState.Inicio)
                {
                    SetHistoricState(HistoricState.MaestroSeleccionado);
                    InitMaestro();
                }
            }

            if (pFields != null)
            {
                IniciaVigencia(lbIncluirFechaFin);
                pFields.FillAjaxControls();
            }
        }

        public override void InitLanguageGrid(HistoricFieldCollection pFieldsGrid, DSOGrid pGridDP, Hashtable phtFiltrosDP, string liCodMaestroDeta)
        {
            base.InitLanguageGrid(pFieldsGrid, pGridDP, phtFiltrosDP, liCodMaestroDeta);
            KeytiaBaseField lField;
            string lsCenCostP;
            foreach (DSOGridClientColumn lCol in pGridDP.Config.aoColumnDefs)
            {
                if (pFieldsGrid.Contains(lCol.sName))
                {
                    lField = pFieldsGrid[lCol.sName];
                    if (lField.ConfigName == "CenCos")
                    {
                        lsCenCostP = Globals.GetMsgWeb(false, "CenCosPadre");
                        if (lsCenCostP.StartsWith("#undefined"))
                        {
                            lCol.sTitle = DSOControl.JScriptEncode(lField.Descripcion);
                        }
                        else
                        {
                            lCol.sTitle = DSOControl.JScriptEncode(lsCenCostP);
                        }
                    }
                }
            }
        }

        protected override bool ValidarRegistro()
        {
            bool lbRet = true;

            //si el registro se esta eliminando entonces no es necesaria la validacion de campos obligatorios
            if (State == HistoricState.Baja)
            {
                return true;
            }

            lbRet = base.ValidarRegistro();
            if (!lbRet)
            {
                return lbRet;
            }

            string lsError = "";

            if (vchDesMaestro.Contains("(BD)"))
            {
                lbRet = ValidarCamposConexion();
                if (lbRet)
                {
                    lbRet = GeneraArchivoCarga();
                }
                if (!lbRet)
                {
                    lsError = "<ul>" + psbErrores.ToString() + "</ul>";
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloCargasWebCentrosCosto");
                    DSOControl.jAlert(Page, "Carga.ValidarRegistro", lsError, lsTitulo);
                }
                else
                {
                    //Asignar el archivo de Excel que se genero al archivo para que lo coloque correctamente 
                    if (pFields.ContainsConfigName("Archivo01"))
                    {
                        pFields.GetByConfigName("Archivo01").DataValue = Session[psFileKey];
                    }
                }
            }

            return lbRet;
        }

        protected override void pbtnExpArchDeta_ServerClick(object sender, EventArgs e)
        {
            PrevState = State;
            if (iCodMaestroDeta != null)
            {
                ExportXLSDetllados(3, int.Parse(iCodCarga), int.Parse(iCodMaestroDeta));
            }
            FirePostConsultarClick();

        }
    }
}

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
    public class CargasWebTasacion : CargasWeb
    {
        protected override void InitGridFields()
        {
            DSOGridClientColumn lCol;
            int lTarget = pHisGrid.Config.aoColumnDefs.Count;

            foreach (KeytiaBaseField lField in pFields)
            {
                if (lField.ShowInGrid
                    && lField.ConfigName != "FechaFin"
                    && lField.ConfigName != "FechaInicio")
                {
                    lCol = new DSOGridClientColumn();
                    lCol.sName = lField.Column;
                    lCol.aTargets.Add(lTarget++);
                    pHisGrid.Config.aoColumnDefs.Add(lCol);
                }
            }
        }
        protected override void InitFiltrosFields()
        {
            DSOTextBox lDSOtxt;
            foreach (KeytiaBaseField lField in pFields)
            {
                if (lField.ShowInGrid
                    && lField.ConfigName != "FechaFin"
                    && lField.ConfigName != "FechaInicio")
                {
                    lDSOtxt = new DSOTextBox();
                    lDSOtxt.ID = lField.Column;
                    lDSOtxt.AddClientEvent("dataFilter", lField.Column);
                    lDSOtxt.Row = lField.Row + 2;
                    lDSOtxt.ColumnSpan = lField.ColumnSpan;
                    lDSOtxt.Table = pTablaFiltros;
                    lDSOtxt.CreateControls();

                    phtFiltros.Add(lDSOtxt.ID, lDSOtxt);
                }
            }

        }
        public override void FillAjaxControls(bool lbIncluirFechaFin)
        {
            //Metodo para llenar controles cuyo valor puede cambiar mediante ajax
            psbQuery.Length = 0;
            psbQuery.AppendLine("Select iCodRegistro, vchDescripcion from Maestros ");
            psbQuery.AppendLine("Where iCodEntidad = " + piCodEntidad.DataValue.ToString());
            psbQuery.AppendLine(" and vchDescripcion like '%Carga%CDRs%' ");
            psbQuery.AppendLine(" and dtIniVigencia <> dtFinVigencia ");
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
        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);
            OcultaCampos();
        }

    }
}

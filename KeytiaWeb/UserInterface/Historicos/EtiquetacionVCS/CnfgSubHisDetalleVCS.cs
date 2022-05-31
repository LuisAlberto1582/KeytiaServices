/*
Nombre:		    DMM
Fecha:		    20120621
Descripción:	Aplicación de Etiquetación de SeeYouOn.
*/
using System;
using System.Web;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Web.SessionState;
using System.Data;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Net.Mail;

namespace KeytiaWeb.UserInterface
{
    public class CnfgSubHisDetalleVCS : HistoricEdit
    {
        public CnfgSubHisDetalleVCS()
        {
            Init += new EventHandler(CnfgSubHisDetalleVCS_Init);
        }

        void CnfgSubHisDetalleVCS_Init(object sender, EventArgs e)
        {
            this.CssClass = "CnfgSubHisDetalleVCS";
        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            pFields.DisableFields();
            if (pFields.ContainsConfigName("Proyecto"))
            {
                pFields.GetByConfigName("Proyecto").EnableField();
            }
            if (pFields.ContainsConfigName("TipoConferencia"))
            {
                pFields.GetByConfigName("TipoConferencia").EnableField();
            }
        }

        public override void ConsultarRegistro()
        {
            if (iCodRegistro != "null")
            {
                DataTable lDataTable = GetDatosRegistro();

                if (lDataTable.Rows.Count > 0)
                {
                    DataRow lDataRow = lDataTable.Rows[0];
                    pFields.SetValues(lDataRow);

                    pvchCodigo.DataValue = lDataRow["vchCodigo"];

                    SetHistoricState(HistoricState.Consulta);
                }
                else
                {
                    iCodRegistro = "null";
                    SetHistoricState(HistoricState.MaestroSeleccionado);
                    pHisGrid.TxtState.Text = "";

                    InitFields();
                    pFields.FillControls();
                }
            }
            else
            {
                SetHistoricState(HistoricState.MaestroSeleccionado);

                InitFields();
                pFields.FillControls();
            }
            pFields.DisableFields();
        }

        protected override DataTable GetDatosRegistro()
        {
            StringBuilder lsbQuery = new StringBuilder();
            lsbQuery.AppendLine("select");
            lsbQuery.AppendLine("   DetalleVCSSystem.iCodRegistro,");
            lsbQuery.AppendLine("   DetalleVCSSystem.iCodCatalogo,");
            lsbQuery.AppendLine("   DetalleVCSSystem.vchCodigo,");
            lsbQuery.AppendLine("   DetalleVCS.FechaInicio,");
            lsbQuery.AppendLine("   DetalleVCS.DuracionSeg,");
            lsbQuery.AppendLine("   DetalleVCS.VCSSourceSystem,");
            lsbQuery.AppendLine("   DetalleVCS.VCSDestinationSystem,");
            lsbQuery.AppendLine("   DetalleVCSSystem.Proyecto,");
            lsbQuery.AppendLine("   DetalleVCSSystem.TipoConferencia");
            lsbQuery.AppendLine("from  [VisDetallados('Detall','DetalleVCS','Español')] DetalleVCS,");
            lsbQuery.AppendLine("      [VisDetallados('Detall','DetalleVCSSystem','Español')] DetalleVCSSystem");
            lsbQuery.AppendLine("where DetalleVCS.iCodCatalogo = DetalleVCSSystem.iCodCatalogo");
            lsbQuery.AppendLine("and DetalleVCS.RegCarga = DetalleVCSSystem.RegCarga");
            lsbQuery.AppendLine("and DetalleVCSSystem.iCodRegistro = " + iCodRegistro);

            DataTable lDataTable = DSODataAccess.Execute(lsbQuery.ToString());
            if (lDataTable.Rows.Count > 0)
            {
                iCodCatalogo = lDataTable.Rows[0]["iCodCatalogo"].ToString();
            }
            else
            {
                iCodCatalogo = null;
            }
            return lDataTable;
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);
            pExpRegistro.Visible = false;
        }

        protected override bool ValidarRegistro()
        {
            return ValidarDatos();
        }

        protected override void GrabarRegistro()
        {
            try
            {
                KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
                if (State != HistoricState.Baja)
                {
                    //Mandar llamar al COM para grabar los datos del historico
                    if (iCodRegistro != "null")
                    {
                        int liCodRegistro = int.Parse(iCodRegistro);
                        if (!lCargasCOM.ActualizaRegistro("Detallados", "Detall", "DetalleVCSSystem", phtValues, liCodRegistro, false, (int)Session["iCodUsuarioDB"]))
                        {
                            throw new KeytiaWebException("ErrSaveRecord");
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveRecord", ex);
            }
        }
    }
}
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
    public class CargasWebResponsables : CargasWebEmpleados
    {

        public override void FillAjaxControls(bool lbIncluirFechaFin)
        {
            //Metodo para llenar controles cuyo valor puede cambiar mediante ajax
            psbQuery.Length = 0;
            psbQuery.AppendLine("Select iCodRegistro, vchDescripcion from Maestros ");
            psbQuery.AppendLine("Where iCodEntidad = " + piCodEntidad.DataValue.ToString());
            psbQuery.AppendLine(" and vchDescripcion like '%Carga%Responsable%' ");
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
        protected override void InitGridDeta()
        {
            DSOGridClientColumn lCol;
            int lTarget = 0;

            pDetaGrid.ClearConfig();
            pDetaGrid.Config.sDom = "<\"H\"Rlf>tr<\"F\"pi>"; //con filtro global
            pDetaGrid.Config.bAutoWidth = true;
            pDetaGrid.Config.sScrollX = "100%";
            pDetaGrid.Config.sScrollY = "400px";
            pDetaGrid.Config.sPaginationType = "full_numbers";
            pDetaGrid.Config.bJQueryUI = true;
            pDetaGrid.Config.bProcessing = true;
            pDetaGrid.Config.bServerSide = true;
            pDetaGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj + ".fnServerDetalleE(this, sSource, aoData, fnCallback);}";
            pDetaGrid.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetHisDetallados");

            if (iCodMaestroDeta != null)
            {
                pFieldsDeta = new HistoricFieldCollection(int.Parse(iCodCarga), int.Parse(iCodMaestroDeta));
            }
            else
            {
                pFieldsDeta = null;
            }

            if (pFieldsDeta != null)
            {
                lCol = new DSOGridClientColumn();
                lCol.sName = "iCodRegistro";
                lCol.aTargets.Add(lTarget++);
                lCol.bVisible = false;
                pDetaGrid.Config.aoColumnDefs.Add(lCol);


                lCol = new DSOGridClientColumn();
                lCol.sName = "vchCodigo";
                lCol.aTargets.Add(lTarget++);
                pDetaGrid.Config.aoColumnDefs.Add(lCol);


                lCol = new DSOGridClientColumn();
                lCol.sName = "vchDescripcion";
                lCol.aTargets.Add(lTarget++);
                pDetaGrid.Config.aoColumnDefs.Add(lCol);

                foreach (KeytiaBaseField lField in pFieldsDeta)
                {
                    if (lField.ShowInGrid)
                    {
                        lCol = new DSOGridClientColumn();
                        lCol.sName = lField.Column;
                        lCol.aTargets.Add(lTarget++);
                        pDetaGrid.Config.aoColumnDefs.Add(lCol);
                    }
                }

                lCol = new DSOGridClientColumn();
                lCol.sName = "dtFecha";
                lCol.aTargets.Add(lTarget++);
                lCol.sWidth = "120px";
                pDetaGrid.Config.aoColumnDefs.Add(lCol);

                lCol = new DSOGridClientColumn();
                lCol.sName = "dtFecUltAct";
                lCol.aTargets.Add(lTarget++);
                pDetaGrid.Config.aoColumnDefs.Add(lCol);

                if (pDetaGrid.Config.aoColumnDefs.Count > 10)
                {
                    pDetaGrid.Config.sScrollXInner = (pDetaGrid.Config.aoColumnDefs.Count * 20).ToString() + "%";
                }

                pDetaGrid.Grid.Visible = true;
                pDetaGrid.Fill();
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
                    string lsTitulo = Globals.GetMsgWeb(false, "TituloCargasWebResponsables");
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
        protected override void BorrarRegDetallados()
        {
            try
            {
                KeytiaCOM.ICargasCOM pCargaCom = (KeytiaCOM.ICargasCOM)Marshal.BindToMoniker("queue:/new:KeytiaCOM.CargasCOM");
                Hashtable lsHtCamposActualizar  = new Hashtable();

                lsHtCamposActualizar.Add("{Emple}", null);

                int liCodUsuario = (int)Session["iCodUsuarioDB"];
                //pCargaCom.ActualizarCarga(int.Parse(iCodCarga), Util.Ht2Xml(lsHtCamposActualizar), liCodUsuario);
                pCargaCom.ActualizarCarga(int.Parse(iCodRegistro), Util.Ht2Xml(lsHtCamposActualizar), liCodUsuario);
                Marshal.ReleaseComObject(pCargaCom);
                pCargaCom = null;
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveRecord", ex);
            }

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DSOControls2008;
using System.Text;

namespace KeytiaWeb.UserInterface
{
    public class CargasWebConferenciasProgramadas : CargasWeb
    {
        protected override void InitGridFields()
        {
            DSOGridClientColumn lCol;
            int lTarget = pHisGrid.Config.aoColumnDefs.Count;

            foreach (KeytiaBaseField lField in pFields)
            {
                if (lField.ShowInGrid
                    && lField.ConfigName != "FecIniCargaVC"
                    && lField.ConfigName != "FecFinCargaVC")
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
                    && lField.ConfigName != "FecIniCargaVC"
                    && lField.ConfigName != "FecFinCargaVC")
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

        //public override void FillAjaxControls(bool lbIncluirFechaFin)
        //{
        //    //Metodo para llenar controles cuyo valor puede cambiar mediante ajax
        //    psbQuery.Length = 0;
        //    psbQuery.AppendLine("Select iCodRegistro, vchDescripcion from Maestros ");
        //    psbQuery.AppendLine("Where iCodEntidad = " + piCodEntidad.DataValue.ToString());
        //    psbQuery.AppendLine(" and vchDescripcion like '%Carga%Conferencias%Programadas%' ");
        //    psbQuery.AppendLine(" and dtIniVigencia <> dtFinVigencia ");
        //    psbQuery.AppendLine(" order by vchDescripcion");

        //    piCodMaestro.DataSource = psbQuery.ToString();
        //    piCodMaestro.Fill();

        //    if (!pbEnableMaestro)
        //    {
        //        piCodMaestro.DataValue = iCodMaestro;
        //        if (State == HistoricState.Inicio)
        //        {
        //            SetHistoricState(HistoricState.MaestroSeleccionado);
        //            InitMaestro();
        //        }
        //    }

        //    if (pFields != null)
        //    {
        //        IniciaVigencia(lbIncluirFechaFin);
        //        pFields.FillAjaxControls();
        //    }
        //}

        protected override bool ValidarDatos()
        {
            bool lbRet = base.ValidarDatos();
            if (!lbRet)
            {
                return lbRet;
            }
            string lsError = "";
            //KeytiaBaseField lField;
            StringBuilder lsbErrores = new StringBuilder();

            if (!lbRet)
            {
                lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                string lsTitulo = Globals.GetMsgWeb(false, "TituloCargasConfProgram");
                DSOControl.jAlert(Page, "Carga.ValidarRegistro", lsError, lsTitulo);
            }
            return lbRet;

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
            }
            if (!lbRet)
            {
                lsError = "<ul>" + psbErrores.ToString() + "</ul>";
                string lsTitulo = Globals.GetMsgWeb(false, "TituloCargasConfProgram");
                DSOControl.jAlert(Page, "Carga.ValidarRegistro", lsError, lsTitulo);
            }
            else
            {
                //Asignar el archivo de Excel que se genero al archivo para que lo coloque correctamente 
                if (pFields.ContainsConfigName("Archivo01") && vchDesMaestro.Contains("(BD)"))
                {
                    pFields.GetByConfigName("Archivo01").DataValue = Session[psFileKey];
                }
            }

            return lbRet;

        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);

            OcultaCampos();

        }
        protected override void BloqueaCampos()
        {
            base.BloqueaCampos();
            if (vchDesMaestro.Contains("(BD)"))
            {
                if (pFields.ContainsConfigName("Archivo01"))
                {
                    pFields.GetByConfigName("Archivo01").DisableField();
                }
            }

            pFields.GetByConfigName("Clase").DataValue = "SeeYouOnServiceBL.CargasSYO.CargaConferenciasProgramadas";
            pFields.GetByConfigName("Clase").DisableField();

            pFields.GetByConfigName("FecIniCargaVC").DisableField();
            pFields.GetByConfigName("FecFinCargaVC").DisableField();

            pvchCodigo.DataValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            pvchCodigo.TextBox.Enabled = false;

            if (!pvchDescripcion.HasValue)
            {
                pvchDescripcion.DataValue = "Default";
            }

        }

        protected override void OcultaCampos()
        {
            base.OcultaCampos();
            //Dejar deshabilitado el panel de Registro (contiene clave y descripcion
            //pExpRegistro.Visible = false;
        }
    }
}

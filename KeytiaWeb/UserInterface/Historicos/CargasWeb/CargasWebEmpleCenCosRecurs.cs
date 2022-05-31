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
    public class CargasWebEmpleCenCosRecurs : CargasWeb
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
            psbQuery.AppendLine(" and vchDescripcion like '%Carga%Empleado%Recursos%' ");
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

            /*
            //Validar que se capture la opcion para crear el usuario
            if (!pFields.ContainsConfigName("OpcCreaUsuar"))
            {
                return lbRet;
            }

            if (pFields.GetByConfigName("OpcCreaUsuar").DataValue == "null")
            {
                lField = pFields.GetByConfigName("OpcCreaUsuar");
                lsError = Globals.GetMsgWeb("CampoRequerido", lField.Descripcion);
                lsbErrores.Append("<li>" + lsError + "</li>");
                lbRet = false;
            }

            int liOpc = int.Parse(pFields.GetByConfigName("OpcCreaUsuar").DataValue.ToString());
            //El valor de la Opcion no debe ser Ninguna
            if (liOpc == 0)
            {
                lField = pFields.GetByConfigName("OpcCreaUsuar");
                lsError = Globals.GetMsgWeb("OpcNoValida", lField.Descripcion);
                lsbErrores.Append("<li>" + lsError + "</li>");
                lbRet = false;
            }
            */
            if (!lbRet)
            {
                lsError = "<ul>" + lsbErrores.ToString() + "</ul>";
                string lsTitulo = Globals.GetMsgWeb(false, "TituloCargasWebEmpleados");
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
                string lsTitulo = Globals.GetMsgWeb(false, "TituloCargasWebEmpleados");
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
            pFields.GetByConfigName("Clase").DataValue = "KeytiaServiceBL.CargaRecursos.CargaEmpleCenCosRecurs";
            pFields.GetByConfigName("Clase").DisableField();
        }
    }
}

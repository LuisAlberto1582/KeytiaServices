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
using System.Collections.Generic;

namespace KeytiaWeb.UserInterface
{
    public class CargasWebPlantillas : HistoricEdit
    {
        protected DSODropDownList piCodReporte;

        protected string iCodReporte;

        public CargasWebPlantillas()
        {
            Init += new EventHandler(CargasWebPlantillasEdit_Init);
        }

        protected virtual void CargasWebPlantillasEdit_Init(object sender, EventArgs e)
        {
            //Controles utilizados en esta pagina
            piCodReporte = new DSODropDownList();
        }

        protected override void InitFields()
        {
            base.InitFields();
            if (pFields != null)
            {
                AgregarBoton("Asunto");
                foreach (KeytiaBaseField lField in pFields)
                {
                    if (lField is KeytiaRelationField)
                    {
                        KeytiaRelationField lRelFields = (KeytiaRelationField)lField;
                        lRelFields.Fields.GetByConfigName("dtIniVigencia").DisableField();
                        lRelFields.Fields.GetByConfigName("dtFinVigencia").DisableField();
                    }
                }
            }
        }

        protected override void InitRegistro()
        {

            base.InitRegistro();

            int liRow = pTablaRegistro.Rows.Count;

            liRow++;

            piCodReporte.ID = "iCodReporte";
            piCodReporte.Table = pTablaRegistro;
            piCodReporte.Row = liRow;
            piCodReporte.ColumnSpan = 3;
            piCodReporte.SelectItemText = " ";
            piCodReporte.DataField = "iCodReporte";
            piCodReporte.CreateControls();
            //piCodMaestro.DropDownList.AutoPostBack = true;
            //piCodMaestro.DropDownList.SelectedIndexChanged += new EventHandler(piCodMaestro_SelectedIndexChanged);
            piCodReporte.AutoPostBack = true;
            piCodReporte.DropDownListChange += new EventHandler(piCodReporte_SelectedIndexChanged);

        }

        public override void InitLanguage()
        {
            base.InitLanguage();

            //Inicializa los controles de la pagina
            piCodReporte.Descripcion = Globals.GetMsgWeb(false, "RepEspecial");

        }

        public override void FillAjaxControls(bool lbIncluirFechaFin)
        {
            //Metodo para llenar controles cuyo valor puede cambiar mediante ajax
            psbQuery.Length = 0;
            psbQuery.AppendLine("Select iCodRegistro, vchDescripcion from Maestros ");
            psbQuery.AppendLine("Where iCodEntidad = " + piCodEntidad.DataValue.ToString());
            psbQuery.AppendLine(" and (vchDescripcion in ( Select  vchDescripcion from Maestros ");
            psbQuery.AppendLine("               Where iCodEntidad = (Select iCodRegistro From Catalogos ");
            psbQuery.AppendLine("                                       Where vchCodigo = 'ReporteEspecial' ");
            psbQuery.AppendLine("                                       AND iCodCatalogo is Null and dtIniVigencia <> dtFinVigencia )");
            psbQuery.AppendLine("               and dtIniVigencia <> dtFinVigencia )) ");
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

        protected virtual void piCodReporte_SelectedIndexChanged(object sender, EventArgs e)
        {
            iCodReporte = piCodReporte.HasValue ? piCodReporte.DataValue.ToString() : null;

            InitReporte();

        }

        protected override void pbtnBaja_ServerClick(object sender, EventArgs e)
        {
            base.pbtnBaja_ServerClick(sender, e);
            ActualizaCarga();
        }

        protected override void AgregarRegistro()
        {
            base.AgregarRegistro();

            IniCarga();
            LlenaiCodReporteEspecial();
            BloqueaCampos();

        }

        protected override void EditarRegistro()
        {
            base.EditarRegistro();
            BloqueaCampos();

        }

        protected void LlenaiCodReporteEspecial()
        {
            StringBuilder lsbQuery = new StringBuilder();

            if (iCodMaestro != null)
            {
                //Metodo para llenar controles cuyo valor puede cambiar mediante ajax
                lsbQuery.Length = 0;
                lsbQuery.Append("Select iCodRegistro, vchDescripcion from [VisHistoricos('ReporteEspecial',");
                lsbQuery.Append("'" + vchDesMaestro + "','" + Globals.GetCurrentLanguage() + "')] ");
                lsbQuery.Append("\r\n  where dtIniVigencia <> dtFinVigencia\r\n");
                lsbQuery.Append(DSOControl.ComplementaVigenciasJS(pdtIniVigencia.Date, pdtFinVigencia.Date));
                lsbQuery.AppendLine(" order by vchDescripcion");

                piCodReporte.DataSource = lsbQuery.ToString();
                piCodReporte.Fill();
                piCodReporte.DataValue = DBNull.Value;
            }

        }

        protected void InitReporte()
        {
            string lsNameColum = "";
            if (iCodReporte == null)
            {
                return;
            }

            DataTable lDataTable = pKDB.GetHisRegByEnt("ReporteEspecial", vchDesMaestro, "iCodRegistro = " + iCodReporte);

            if (lDataTable.Rows.Count > 0)
            {
                DataRow lDataRow = lDataTable.Rows[0];
                int liCodReporte = (int)lDataTable.Rows[0]["iCodCatalogo"];

                if (pFields.ContainsConfigName("ReporteEspecial"))
                {
                    pFields.GetByConfigName("ReporteEspecial").DataValue = liCodReporte;
                }

                // Obtener el Campo de los Catalogos de la Relacion
                foreach (DataColumn lDataCol in lDataTable.Columns)
                {
                    if (lDataCol.ColumnName.Contains("{")
                        && lDataCol.ColumnName.Contains("}"))
                    {
                        lsNameColum = lDataCol.ColumnName.Replace("{", "").Replace("}", "");
                        if (pFields.ContainsConfigName(lsNameColum))
                        {
                            pFields.GetByConfigName(lsNameColum).DataValue = lDataTable.Rows[0][lDataCol.ColumnName];
                        }
                    }
                }

                IniciaRelaciones(lDataTable);
            }
        }

        protected void IniciaRelaciones(DataTable lDataTable)
        {
            string lsRelName = "";
            string lsRelPlantilla = "";
            DataTable ldtRelacion;

            StringBuilder lsbQuery = new StringBuilder();

            int liCodReporte = (int)lDataTable.Rows[0]["iCodCatalogo"];

            // Obtener el Campo de los Catalogos de la Relacion
            foreach (DataColumn lDataCol in lDataTable.Columns)
            {
                if (lDataCol.ColumnName.Contains("Reporte Especial -"))
                {
                    lsRelName = lDataCol.ColumnName.Replace("{", "").Replace("}", "");
                    lsRelPlantilla = lsRelName.Replace("Reporte Especial -", "Solicitud Reporte Especial -");

                    lsbQuery.Length = 0;
                    lsbQuery.Append("Select * from [VisRelaciones('" + lsRelName + "','" + Globals.GetCurrentLanguage() + "')] ");
                    lsbQuery.Append("\r\n  where ReporteEspecial = " + liCodReporte);
                    lsbQuery.Append("\r\n  And dtIniVigencia <> dtFinVigencia \r\n");
                    lsbQuery.Append(DSOControl.ComplementaVigenciasJS(pdtIniVigencia.Date, pdtFinVigencia.Date));

                    ldtRelacion = DSODataAccess.Execute(lsbQuery.ToString());
                    if (pFields.ContainsConfigName(lsRelPlantilla) && ldtRelacion.Rows.Count > 0)
                    {
                        IniciaRelCampos(ldtRelacion, lsRelName, lsRelPlantilla);
                    }
                }
            }
        }

        protected void IniciaRelCampos(DataTable lDataTable, string lsRelName, string lsRelPlantilla)
        {

            KeytiaRelationField lRelFields;

            int liCodRelacionOrigen;
            int liCodEntidadAux;
            DataTable ldtRelNameConfig = DSODataAccess.Execute("select * from Relaciones where iCodRelacion is null and vchDescripcion = '" + lsRelName + "'");
            liCodRelacionOrigen = (int)ldtRelNameConfig.Rows[0]["iCodRegistro"];
            liCodEntidadAux = (int)ldtRelNameConfig.Rows[0]["iCodCatalogo01"];

            RelationFieldCollection lRelNameCollection = new RelationFieldCollection(liCodEntidadAux, liCodRelacionOrigen);

            lRelFields = ((KeytiaRelationField)pFields.GetByConfigName(lsRelPlantilla));

            ((DSOGrid)lRelFields.DSOControlDB).ClearEditedData();
            DataTable lEditedData = ((DSOGrid)lRelFields.DSOControlDB).EditedData;
            foreach (KeytiaBaseField lField in lRelFields.Fields)
            {
                if (lField.Column.StartsWith("iCodCatalogo")
                    && !lField.Column.EndsWith("Display")
                    && !lEditedData.Columns.Contains(lField.Column + "Display"))
                {
                    lEditedData.Columns.Add(new DataColumn(lField.Column + "Display", typeof(string)));
                }
            }

            lRelFields.Fields.GetByConfigName("dtIniVigencia").DisableField();
            lRelFields.Fields.GetByConfigName("dtFinVigencia").DisableField();

            DataRow lEditedRow;
            KeytiaBaseField lRelField;
            int liCodRegistro = 0;
            foreach (DataRow lDataRow in lDataTable.Rows)
            {
                lEditedRow = lEditedData.NewRow();
                liCodRegistro--;
                lEditedRow["iCodRegistro"] = liCodRegistro;
                foreach (KeytiaBaseField lField in lRelNameCollection)
                {
                    if (lRelFields.Fields.ContainsConfigName(lField.ConfigName) &&
                        (lField.ConfigName != "dtIniVigencia") &&
                        (lField.ConfigName != "dtFinVigencia"))
                    {
                        lRelField = lRelFields.Fields.GetByConfigName(lField.ConfigName);
                        lEditedRow[lRelField.Column] = lDataRow[lField.ConfigName];
                        if (lRelField.Column.StartsWith("iCodCatalogo"))
                        {
                            lEditedRow[lRelField.Column + "Display"] = lDataRow[lField.ConfigName + "Desc"];
                        }
                    }
                }
                lEditedRow["Editar"] = 1;
                lEditedRow["Eliminar"] = 1;
                lEditedData.Rows.Add(lEditedRow);
            }
        }

        protected void IniCarga()
        {
            KDBAccess lKDB = new KDBAccess();

            // Inicializa el Estatus de la carga
            DataTable lKDBTable = pKDB.GetHisRegByEnt("EstCarga", "Estatus Cargas", "vchCodigo = 'CarEspera'");

            if (lKDBTable != null && lKDBTable.Rows.Count > 0)
            {
                int liCodEstatus = (int)lKDBTable.Rows[0]["iCodCatalogo"];

                if (pFields.ContainsConfigName("EstCarga"))
                {
                    pFields.GetByConfigName("EstCarga").DataValue = liCodEstatus;
                }
            }

            //Si no se proporcionaron valores para las vigencias entonces establezco los valores default
            if (!pdtIniVigencia.HasValue)
            {
                pdtIniVigencia.DataValue = DateTime.Today;
            }
            if (!pdtFinVigencia.HasValue)
            {
                pdtFinVigencia.DataValue = new DateTime(2079, 1, 1);
            }

        }

        protected void BloqueaCampos()
        {
            if (pFields != null)
            {
                if (pFields.ContainsConfigName("ReporteEspecial"))
                {
                    pFields.GetByConfigName("ReporteEspecial").DisableField();
                }
            }
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);

            StringBuilder lsb = new StringBuilder();

            piCodReporte.DropDownList.Enabled = false;
            ((TableRow)piCodReporte.TcCtl.Parent).Style["display"] = "none";

            if (s == HistoricState.Consulta)
            {
                if (!CargaActiva())
                {
                    pbtnBaja.Visible = false;
                }
            }
            else if (s == HistoricState.Edicion)
            {
                if (iCodRegistro == "null")
                {
                    piCodReporte.DropDownList.Enabled = true;
                    ((TableRow)piCodReporte.TcCtl.Parent).Style.Remove("display");
                }
            }
            else if (s == HistoricState.MaestroSeleccionado)
            {
                lsb.Length = 0;
                lsb.AppendLine("<script type='text/javascript'>");
                lsb.AppendLine("jQuery(function($){");
                lsb.AppendLine(pjsObj + ".refreshGrid = 10000;");
                lsb.AppendLine("});");
                lsb.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), pjsObj + "refreshGrid", lsb.ToString(), true, false);

            }


            ((TableRow)pdtFinVigencia.TcCtl.Parent).Style["display"] = "none";
            ((TableRow)pdtIniVigencia.TcCtl.Parent).Style["display"] = "none";

        }

        protected bool CargaActiva()
        {
            bool lbRet = true;

            //Estatus de cargas 
            int liCodCargaInicializada = 0;
            int liCodCargaEnEspera = 0;
            int liCodEstatus = 0;
            DataTable lKDBTable = pKDB.GetHisRegByEnt("EstCarga", "Estatus Cargas", "vchCodigo = 'CarInicial'");
            if (lKDBTable != null && lKDBTable.Rows.Count > 0)
            {
                liCodCargaInicializada = (int)lKDBTable.Rows[0]["iCodCatalogo"];
            }
            // Estatus de la carga en espera de servicio

            lKDBTable = pKDB.GetHisRegByEnt("EstCarga", "Estatus Cargas", "vchCodigo = 'CarEspera'");
            if (lKDBTable != null && lKDBTable.Rows.Count > 0)
            {
                liCodCargaEnEspera = (int)lKDBTable.Rows[0]["iCodCatalogo"];
            }

            if (pFields.ContainsConfigName("EstCarga") &&
                pFields.GetByConfigName("EstCarga").DataValue != "null")
            {
                string lsEstatus = pFields.GetByConfigName("EstCarga").DataValue.ToString();
                liCodEstatus = int.Parse(lsEstatus);
            }

            if (liCodEstatus == liCodCargaEnEspera || liCodEstatus == liCodCargaInicializada)
            {
                lbRet = false;
            }
            return lbRet;

        }

        protected void ActualizaCarga()
        {
            try
            {
                //Obtener el codigo de estatus de cargas eliminada
                DataTable lKDBTable = pKDB.GetHisRegByEnt("EstCarga", "Estatus Cargas", "vchCodigo = 'CarElimina'");
                int liCodRegistro = (int)lKDBTable.Rows[0]["iCodCatalogo"];

                if (pFields.ContainsConfigName("EstCarga"))
                {
                    pFields.GetByConfigName("EstCarga").DataValue = liCodRegistro;
                    pdtFinVigencia.DataValue = DateTime.Today;
                }

            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveRecord", ex);
            }
        }

        protected override bool ValidarRelaciones()
        {
            //Validar relaciones
            KeytiaRelationField lRelField;
            foreach (DataTable lDataTable in pdsRelValues.Tables)
            {
                lRelField = ((KeytiaRelationField)pFields.GetByConfigName(lDataTable.TableName));
                foreach (DataRow lDataRow in lDataTable.Rows)
                {
                    //Validar que fecha de inicio de vigencia tenga valor
                    if (lDataRow["dtIniVigencia"] == DBNull.Value)
                    {
                        lDataRow["dtIniVigencia"] = pdtIniVigencia.Date;
                    }
                }
            }
            return base.ValidarRelaciones();
        }
    }
}

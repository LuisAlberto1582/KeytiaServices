/*
 * Nombre:		    DMM
 * Fecha:		    20110607
 * Descripción:	    Clase para el configurador de Maestros
 * Modificación:	
 */

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;

namespace KeytiaWeb.UserInterface
{
    public class MaestroEdit : RegEdit
    {
        #region Propiedades
        protected DSODropDownList piCodEntidad;
        protected DSOCheckBox pbActualizaHistoria;
        protected string psFiltroRelacion;
        protected string psFiltroCatalogo;
        protected string psFiltroInteger;
        protected string psFiltroFloat;
        protected string psFiltroDate;
        protected string psFiltroVarChar;
        protected DataTable dtRows;
        protected DataTable dtCols;
        public DSODropDownList iCodEntidad
        {
            get
            {
                return piCodEntidad;
            }
        }
        #endregion

        public MaestroEdit()
        {
            psNombreTabla = "Maestros";
            piMaxColumnSpan = 7;
            psFiltroDescripcion = " and dtIniVigencia <> dtFinVigencia ";
            Init += new EventHandler(MaestroEdit_Init);
        }

        protected void MaestroEdit_Init(System.Object sender, EventArgs e)
        {
            piCodEntidad = new DSODropDownList();
            pbActualizaHistoria = new DSOCheckBox();
        }

        protected override void CreateFiltros()
        {
            piCodEntidad.ID = "iCodEntidad";
            piCodEntidad.Table = pTablaEdit;
            piCodEntidad.Row = pTablaEdit.Rows.Count + 1;
            piCodEntidad.ColumnSpan = piMaxColumnSpan;
            piCodEntidad.Descripcion = Globals.GetMsgWeb("Entidad");
            piCodEntidad.CreateControls();
            piCodEntidad.DataValueDelimiter = "";
            base.CreateFiltros();
        }

        protected override void CreateTablaEdit()
        {
            SortedList<string, int> Renglones = new SortedList<string, int>();
            DataTable dtEdit;
            try
            {
                dtEdit = DSODataAccess.Execute("Select * from " + psNombreTabla + " where 1 = 2"); //Leo la estructura de la tabla
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "Titulo" + psNombreTabla);
                throw new KeytiaWebException(true, "ErrorConsulta", e, lsTitulo);
            }

            foreach (DataColumn dc in dtEdit.Columns)
            {
                string lsTipoCampo = "";
                int iRow = 0;
                if (IsRen(dc.ColumnName) || IsCol(dc.ColumnName) || IsReq(dc.ColumnName))
                {
                    lsTipoCampo = dc.ColumnName.Substring(dc.ColumnName.Length - 3);
                    iRow = Renglones[dc.ColumnName.Substring(0, dc.ColumnName.Length - 3)];
                }
                else if (IsAtrib(dc.ColumnName))
                {
                    lsTipoCampo = dc.ColumnName.Substring(0, dc.ColumnName.Length - 2);
                    iRow = pTablaEdit.Rows.Count + 1;
                    Renglones.Add(dc.ColumnName, iRow);
                }
                else
                {
                    continue;
                }
                DSOControlDB ddl; ;
                if (lsTipoCampo == "Req")
                {
                    ddl = new DSOCheckBox();
                    ddl.ID = dc.ColumnName;
                    ddl.DataField = dc.ColumnName;
                    ddl.Table = pTablaEdit;
                    ddl.Row = iRow;
                    ddl.CreateControls();
                    ddl.DataValueDelimiter = "";
                }
                else
                {
                    ddl = new DSODropDownList();
                    ddl.ID = dc.ColumnName;
                    ddl.DataField = dc.ColumnName;
                    ddl.Table = pTablaEdit;
                    ddl.Row = iRow;
                    ddl.CreateControls();
                    ((DSODropDownList)ddl).SelectItemValue = "null";
                    ((DSODropDownList)ddl).SelectItemText = " ";
                    ddl.DataValueDelimiter = "";
                }

                if (lsTipoCampo != "Ren" && lsTipoCampo != "Col" && lsTipoCampo != "Req")
                {
                    ((DSODropDownList)ddl).TcCtl.CssClass = "DSOTcCtl DSOTcCtlMain";
                    ddl.TcCtl.Style["width"] = "50%";

                    ddl.AddClientEvent("tipoCampo", lsTipoCampo);
                }
                else
                {
                    ddl.TcCtl.CssClass = "DSOTcCtl DSOTcCtlSub";
                    ddl.AddClientEvent(lsTipoCampo, iRow.ToString());
                }

                pHTControls.Add(ddl.ID, ddl);
            }

            piCodUsuario.ID = "iCodUsuario";
            piCodUsuario.CreateControls();

            pdtFecUltAct.ID = "dtFecUltAct";
            pdtFecUltAct.ShowHour = false;
            pdtFecUltAct.ShowMinute = false;
            pdtFecUltAct.ShowSecond = false;
            pdtFecUltAct.CreateControls();

            pbActualizaHistoria.ID = "bActualizaHistoria";
            pbActualizaHistoria.ColumnSpan = piMaxColumnSpan;
            pbActualizaHistoria.Row = pTablaEdit.Rows.Count + 1;
            pbActualizaHistoria.Table = pTablaEdit;
            pbActualizaHistoria.DataField = "bActualizaHistoria";
            pbActualizaHistoria.CreateControls();
            pbActualizaHistoria.DataValueDelimiter = "";
            pHTControls.Add(pbActualizaHistoria.ID, pbActualizaHistoria);
        }

        protected override bool getPermiso(Permiso lpPermiso)
        {
            return DSONavegador.getPermiso("OpcMae", lpPermiso);
        }

        protected override DataTable getDataSource(string lsDataField, object Value)
        {
            DataTable dataSource = null;
            string lsFiltroCat = "";
            string lsFiltroReg = "";
            if (Value != null && Value != DBNull.Value && Value.ToString() != "" && Value != System.Type.Missing)
            {
                lsFiltroCat = " and iCodCatalogo in (" + Value.ToString() + ")";
                lsFiltroReg = " and iCodRegistro in (" + Value.ToString() + ")";
            }
            if (IsRen(lsDataField))
            {
                dataSource = dtRows;
            }
            else if (IsCol(lsDataField))
            {
                dataSource = dtCols;
            }
            else if (lsDataField.StartsWith("iCodRelacion"))
            {
                dataSource = getDataSource(psFiltroRelacion + lsFiltroReg, false);
            }
            else if (lsDataField.StartsWith("iCodCatalogo"))
            {
                dataSource = getDataSource(psFiltroCatalogo + lsFiltroReg, false);
            }
            else if (lsDataField.StartsWith("Integer"))
            {
                dataSource = getDataSource(psFiltroInteger + lsFiltroCat, true);
            }
            else if (lsDataField.StartsWith("Float"))
            {
                dataSource = getDataSource(psFiltroFloat + lsFiltroCat, true);
            }
            else if (lsDataField.StartsWith("Date"))
            {
                dataSource = getDataSource(psFiltroDate + lsFiltroCat, true);
            }
            else if (lsDataField.StartsWith("VarChar"))
            {
                dataSource = getDataSource(psFiltroVarChar + lsFiltroCat, true);
            }

            return dataSource;
        }

        protected override void InitData()
        {
            try
            {
                DataTable dtTypes = kdb.GetCatRegByEnt("Types");
                psFiltroRelacion = getFiltroAtrib("iCodRelacion", iCodEntidad.DataValue.ToString(), dtTypes);
                psFiltroCatalogo = getFiltroAtrib("iCodCatalogo", dtTypes);
                psFiltroInteger = getFiltroAtrib("Integer", dtTypes);
                psFiltroFloat = getFiltroAtrib("Float", dtTypes);
                psFiltroDate = getFiltroAtrib("Date", dtTypes);
                psFiltroVarChar = getFiltroAtrib("VarChar", dtTypes);
                dtRows = DataSourceNumbers(50);
                dtCols = DataSourceNumbers(2);
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "Titulo" + psNombreTabla);
                throw new KeytiaWebException(true, "ErrorConsulta", e, lsTitulo);
            }
        }

        public static string getFiltroAtrib(string tipoCampo)
        {
            KDBAccess kdb = new KDBAccess();
            DataTable dtTypes = kdb.GetCatRegByEnt("Types");
            return getFiltroAtrib(tipoCampo, "null", dtTypes);
        }

        public static string getFiltroAtrib(string tipoCampo, string iCodEntidad)
        {
            KDBAccess kdb = new KDBAccess();
            DataTable dtTypes = kdb.GetCatRegByEnt("Types");
            return getFiltroAtrib(tipoCampo, iCodEntidad, dtTypes);
        }

        public static string getFiltroAtrib(string tipoCampo, DataTable dtTypes)
        {
            return getFiltroAtrib(tipoCampo, "null", dtTypes);
        }

        public static string getFiltroAtrib(string tipoCampo, string iCodEntidad, DataTable dtTypes)
        {
            string lsFiltro = "";
            switch (tipoCampo)
            {
                case "iCodRelacion":
                    lsFiltro = "Select iCodRegistro, vchDescripcion from Relaciones where iCodRelacion is null" + // and IsNull(bBajaCascada, 0) = 0" +
                                " and dtIniVigencia <> dtFinVigencia " +
                                (iCodEntidad != "null" && iCodEntidad != "" ?
                                " and " + iCodEntidad + " in (" + getColCatRel() + ")" : "");
                    break;
                case "iCodCatalogo":
                    lsFiltro = "Select iCodRegistro, vchDescripcion from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia";
                    break;
                case "Integer":
                    lsFiltro = "{Types} in (" + dtTypes.Select("vchCodigo = 'Int'")[0]["iCodRegistro"].ToString() +
                                         ", " + dtTypes.Select("vchCodigo = 'IntFormat'")[0]["iCodRegistro"].ToString() +
                                         ", " + dtTypes.Select("vchCodigo = 'Flags'")[0]["iCodRegistro"].ToString() +
                                         ", " + dtTypes.Select("vchCodigo = 'Mopcion'")[0]["iCodRegistro"].ToString() + ")";
                    break;
                case "Float":
                    lsFiltro = "{Types} in (" + dtTypes.Select("vchCodigo = 'Float'")[0]["iCodRegistro"].ToString() +
                                         ", " + dtTypes.Select("vchCodigo = 'Currency'")[0]["iCodRegistro"].ToString() + ")";
                    break;
                case "Date":
                    lsFiltro = "{Types} in (" + dtTypes.Select("vchCodigo = 'Date'")[0]["iCodRegistro"].ToString() +
                                         ", " + dtTypes.Select("vchCodigo = 'DateTime'")[0]["iCodRegistro"].ToString() + ")";
                    break;
                case "VarChar":
                    lsFiltro = "{Types} in (" + dtTypes.Select("vchCodigo = 'VChar'")[0]["iCodRegistro"].ToString() +
                        /*", " + dtTypes.Select("vchCodigo = 'HyperLink'")[0]["iCodRegistro"].ToString() +
                        ", " + dtTypes.Select("vchCodigo = 'Password'")[0]["iCodRegistro"].ToString() +
                        ", " + dtTypes.Select("vchCodigo = 'Upload'")[0]["iCodRegistro"].ToString() +*/
                                                                                                                         ")";
                    break;

            }
            return lsFiltro;
        }

        protected override void DeshabilitarCatalogos()
        {
            try
            {
                if (piCodRegistro.DataValue.ToString() == "null")
                {
                    return;
                }

                DataTable ldt;

                ldt = DSODataAccess.Execute("select top 1 iCodRegistro from Historicos where dtIniVigencia <> dtFinVigencia and iCodMaestro = " + piCodRegistro.DataValue.ToString());
                if (ldt.Rows.Count == 0)
                {
                    ldt = DSODataAccess.Execute("select top 1 iCodRegistro from Detallados where iCodMaestro = " + piCodRegistro.DataValue.ToString());
                }
                if (ldt.Rows.Count == 0)
                {
                    ldt = DSODataAccess.Execute("select top 1 iCodRegistro from Pendientes where iCodMaestro = " + piCodRegistro.DataValue.ToString());
                }

                if (ldt.Rows.Count > 0)
                {
                    foreach (DSOControlDB ctl in pHTControls.Values)
                    {
                        if (Regex.IsMatch(ctl.DataField, "iCodCatalogo[0-9][0-9]$", RegexOptions.IgnoreCase)
                            && ctl.HasValue)
                        {
                            ((WebControl)ctl.Control).Enabled = false;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new KeytiaWebException("ErrInitControls", e);
            }
        }

        protected override void ValidarDatos()
        {
            try
            {
                if (ViewState["Estado"].ToString() != "ConfirmarBaja")
                {
                    return;
                }

                DataTable ldt;

                ldt = DSODataAccess.Execute("select top 1 iCodRegistro from Historicos where dtIniVigencia <> dtFinVigencia and iCodMaestro = " + piCodRegistro.DataValue.ToString());
                if (ldt.Rows.Count > 0)
                {
                    psbErrores.Append("<li>" + Globals.GetMsgWeb("MaeDatosHistoricos") + "</li>");
                }

                ldt = DSODataAccess.Execute("select top 1 iCodRegistro from Detallados where iCodMaestro = " + piCodRegistro.DataValue.ToString());
                if (ldt.Rows.Count > 0)
                {
                    psbErrores.Append("<li>" + Globals.GetMsgWeb("MaeDatosDetallados") + "</li>");
                }

                ldt = DSODataAccess.Execute("select top 1 iCodRegistro from Pendientes where iCodMaestro = " + piCodRegistro.DataValue.ToString());
                if (ldt.Rows.Count > 0)
                {
                    psbErrores.Append("<li>" + Globals.GetMsgWeb("MaeDatosPendientes") + "</li>");
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
        }

        protected override void InitFiltros()
        {
            piCodEntidad.DataField = "iCodEntidad";
            piCodEntidad.SelectItemValue = "null";
            piCodEntidad.SelectItemText = " ";
            piCodEntidad.DataSource = "Select iCodRegistro, vchDescripcion from Catalogos where dtIniVigencia <> dtFinVigencia and iCodCatalogo is null order by vchDescripcion";
            piCodEntidad.Fill();

            piCodRegistro.Source = ResolveClientUrl("~/WebMethods.aspx/GetMaestros");
            piCodRegistro.FnSearch = "Maestros.Autocomplete.Source";
            piCodRegistro.AddClientEvent("iCodEntidad", "#" + piCodEntidad.TextValue.ClientID);
            base.InitFiltros();
        }

        protected override void InitTablaEdit()
        {
            foreach (DSOControlDB ctl in pHTControls.Values)
            {
                if (ctl is DSODropDownList && !IsAtrib(ctl.DataField))
                {
                    DSODropDownList ddl = (DSODropDownList)ctl;
                    DataTable dataSource = getDataSource(ddl.DataField, null);
                    if (dataSource != null)
                    {
                        ddl.DataSource = dataSource;
                        ddl.Fill();
                    }
                }
            }
        }

        protected override void setRequiredMessages()
        {
            base.setRequiredMessages();
            foreach (DSOControlDB ctl in pHTControls.Values)
            {
                if (IsRen(ctl.DataField) || IsCol(ctl.DataField))
                {
                    string ColumnName = ctl.DataField.Substring(0, ctl.DataField.Length - 3);
                    if (((IDSOFillableInput)pHTControls[ColumnName]).TextValue.Text != "null")
                    {
                        if (IsRen(ctl.DataField))
                        {
                            ctl.RequiredMessage = Globals.GetMsgWeb("CampoRequerido", Globals.GetMsgWeb("Renglon") + ": " + ColumnName);
                        }
                        else if (IsCol(ctl.DataField))
                        {
                            ctl.RequiredMessage = Globals.GetMsgWeb("CampoRequerido", Globals.GetMsgWeb("Columna") + ": " + ColumnName);
                        }
                    }
                    else
                    {
                        ctl.RequiredMessage = "";
                    }
                }
            }
        }

        protected override void ValidarRepetidos()
        {
            DataTable ldtiCodRelacion = getDataSource(psFiltroRelacion, false);
            DataTable ldtiCodCatalogo = getDataSource(psFiltroCatalogo, false);
            DataTable ldtInteger = getDataSource(psFiltroInteger, true);
            DataTable ldtFloat = getDataSource(psFiltroFloat, true);
            DataTable ldtDate = getDataSource(psFiltroDate, true);
            DataTable ldtVarChar = getDataSource(psFiltroVarChar, true);
            List<string> RenCol = new List<string>();
            ArrayList lstiCodRelacionRepetido = new ArrayList();
            ArrayList lstiCodCatalogoRepetido = new ArrayList();
            ArrayList lstIntegerRepetido = new ArrayList();
            ArrayList lstFloatRepetido = new ArrayList();
            ArrayList lstDateRepetido = new ArrayList();
            ArrayList lstVarCharRepetido = new ArrayList();
            bool bRelacionAtributo = false;
            foreach (DSOControlDB ctl in pHTControls.Values)
            {
                if (ctl.DataValue.ToString() != "null" && IsAtrib(ctl.DataField))
                {
                    string lsTipoCampo = ctl.DataField.Substring(0, ctl.DataField.Length - 2);
                    DataRow[] dr;
                    switch (lsTipoCampo)
                    {
                        case "iCodRelacion":
                            bRelacionAtributo = true;
                            dr = ldtiCodRelacion.Select("iCodRegistro = " + ctl.DataValue.ToString());
                            if (dr.Length > 0)
                            {
                                ldtiCodRelacion.Rows.Remove(dr[0]);
                            }
                            else
                            {
                                lstiCodRelacionRepetido.Add(ctl.Descripcion);
                            }
                            break;
                        case "iCodCatalogo":
                            bRelacionAtributo = true;
                            dr = ldtiCodCatalogo.Select("iCodRegistro = " + ctl.DataValue.ToString());
                            if (dr.Length > 0)
                            {
                                ldtiCodCatalogo.Rows.Remove(dr[0]);
                            }
                            else
                            {
                                lstiCodCatalogoRepetido.Add(ctl.Descripcion);
                            }
                            break;
                        case "Integer":
                            bRelacionAtributo = true;
                            dr = ldtInteger.Select("iCodCatalogo = " + ctl.DataValue.ToString());
                            if (dr.Length > 0)
                            {
                                ldtInteger.Rows.Remove(dr[0]);
                            }
                            else
                            {
                                lstIntegerRepetido.Add(ctl.Descripcion);
                            }
                            break;
                        case "Float":
                            bRelacionAtributo = true;
                            dr = ldtFloat.Select("iCodCatalogo = " + ctl.DataValue.ToString());
                            if (dr.Length > 0)
                            {
                                ldtFloat.Rows.Remove(dr[0]);
                            }
                            else
                            {
                                lstFloatRepetido.Add(ctl.Descripcion);
                            }
                            break;
                        case "Date":
                            bRelacionAtributo = true;
                            dr = ldtDate.Select("iCodCatalogo = " + ctl.DataValue.ToString());
                            if (dr.Length > 0)
                            {
                                ldtDate.Rows.Remove(dr[0]);
                            }
                            else
                            {
                                lstDateRepetido.Add(ctl.Descripcion);
                            }
                            break;
                        case "VarChar":
                            bRelacionAtributo = true;
                            dr = ldtVarChar.Select("iCodCatalogo = " + ctl.DataValue.ToString());
                            if (dr.Length > 0)
                            {
                                ldtVarChar.Rows.Remove(dr[0]);
                            }
                            else
                            {
                                lstVarCharRepetido.Add(ctl.Descripcion);
                            }
                            break;
                    }
                }
                else if (ctl.DataValue.ToString() == "null" && !string.IsNullOrEmpty(ctl.RequiredMessage))
                {
                    psbErrores.Append("<li>" + ctl.RequiredMessage + "</li>");
                }
                else if (IsRen(ctl.DataField))
                {
                    string Ren = ctl.DataValue.ToString();
                    string Col = ((DSODropDownList)pHTControls[ctl.DataField.Substring(0, ctl.DataField.Length - 3) + "Col"]).DataValue.ToString();
                    if (Col != "null")
                    {
                        if (RenCol.Contains(Ren + "," + Col))
                            psbErrores.Append("<li>" + Globals.GetMsgWeb("RenglonColumnaRepetido", new string[2] { Ren, Col }) + "</li>");
                        else
                            RenCol.Add(Ren + "," + Col);
                    }
                }
            }

            if (!bRelacionAtributo)
                psbErrores.Append("<li>" + Globals.GetMsgWeb("CamposInsuficientesMaestros") + "</li>");

            foreach (string lsCampo in lstiCodRelacionRepetido)
                psbErrores.Append("<li>" + Globals.GetMsgWeb("CampoRepetido", lsCampo) + "</li>");
            foreach (string lsCampo in lstiCodCatalogoRepetido)
                psbErrores.Append("<li>" + Globals.GetMsgWeb("CampoRepetido", lsCampo) + "</li>");
            foreach (string lsCampo in lstIntegerRepetido)
                psbErrores.Append("<li>" + Globals.GetMsgWeb("CampoRepetido", lsCampo) + "</li>");
            foreach (string lsCampo in lstFloatRepetido)
                psbErrores.Append("<li>" + Globals.GetMsgWeb("CampoRepetido", lsCampo) + "</li>");
            foreach (string lsCampo in lstDateRepetido)
                psbErrores.Append("<li>" + Globals.GetMsgWeb("CampoRepetido", lsCampo) + "</li>");
            foreach (string lsCampo in lstVarCharRepetido)
                psbErrores.Append("<li>" + Globals.GetMsgWeb("CampoRepetido", lsCampo) + "</li>");
        }

        protected override Hashtable getValoresCampos()
        {
            Hashtable phtValoresCampos = base.getValoresCampos();
            phtValoresCampos.Add("iCodEntidad", piCodEntidad.DataValue.ToString());
            return phtValoresCampos;
        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            piCodEntidad.Descripcion = Globals.GetMsgWeb("Entidad");

            foreach (DSOControlDB ctl in pHTControls.Values)
            {
                if (IsRen(ctl.DataField))
                {
                    ctl.Descripcion = Globals.GetMsgWeb("Renglon");
                }
                else if (IsCol(ctl.DataField))
                {
                    ctl.Descripcion = Globals.GetMsgWeb("Columna");
                }
                if (ctl is DSOCheckBox)
                {
                    ctl.Descripcion = Globals.GetMsgWeb("Requerido");
                    ((DSOCheckBox)ctl).TrueText = Globals.GetMsgWeb("Si");
                    ((DSOCheckBox)ctl).FalseText = Globals.GetMsgWeb("No");
                }
            }

            pbActualizaHistoria.Descripcion = Globals.GetMsgWeb("bActualizaHistoria");
            pbActualizaHistoria.TrueText = Globals.GetMsgWeb("Si");
            pbActualizaHistoria.FalseText = Globals.GetMsgWeb("No");

        }

        protected override void setEstado(string Estado)
        {
            base.setEstado(Estado);
            switch (Estado)
            {
                case "ConfirmarBaja":
                case "Edicion":
                    ((WebControl)piCodEntidad.Control).Enabled = false;
                    break;
                default:
                    ((WebControl)piCodEntidad.Control).Enabled = true;
                    break;
            }
        }

        protected bool IsRen(string lsDataField)
        {
            return lsDataField.EndsWith("Ren");
        }

        protected bool IsCol(string lsDataField)
        {
            return lsDataField.EndsWith("Col");
        }

        protected bool IsReq(string lsDatafield)
        {
            return lsDatafield.EndsWith("Req");
        }

        protected override void DeshabilitarOpciones(bool value)
        {
            foreach (DSOControlDB ctl in pHTControls.Values)
            {
                if (IsAtrib(ctl.DataField) && ctl.DataValue.ToString() == "null")
                {
                    DSOControlDB ctlEnabled;

                    ctlEnabled = (DSOControlDB)pHTControls[ctl.DataField + "Req"];
                    ((WebControl)ctlEnabled.Control).Enabled = value;

                    ctlEnabled = (DSOControlDB)pHTControls[ctl.DataField + "Ren"];
                    ((WebControl)ctlEnabled.Control).Enabled = value;

                    ctlEnabled = (DSOControlDB)pHTControls[ctl.DataField + "Col"];
                    ((WebControl)ctlEnabled.Control).Enabled = value;
                }
            }
        }

        protected override object SaveViewState()
        {
            Object baseState = base.SaveViewState();
            Object[] allStates = new Object[7];
            allStates[0] = baseState;
            allStates[1] = psFiltroRelacion;
            allStates[2] = psFiltroCatalogo;
            allStates[3] = psFiltroInteger;
            allStates[4] = psFiltroFloat;
            allStates[5] = psFiltroDate;
            allStates[6] = psFiltroVarChar;
            return allStates;
        }

        protected override void LoadViewState(object savedState)
        {
            if (savedState != null)
            {
                object[] myState = (object[])savedState;
                if (myState[0] != null)
                    base.LoadViewState(myState[0]);
                if (myState[1] != null)
                    psFiltroRelacion = (string)myState[1];
                if (myState[2] != null)
                    psFiltroCatalogo = (string)myState[2];
                if (myState[3] != null)
                    psFiltroInteger = (string)myState[3];
                if (myState[4] != null)
                    psFiltroFloat = (string)myState[4];
                if (myState[5] != null)
                    psFiltroDate = (string)myState[5];
                if (myState[6] != null)
                    psFiltroVarChar = (string)myState[6];
            }
        }

        protected override void ValidarCampos()
        {
            psbErrores = new System.Text.StringBuilder();
            string lFiltroDescripcion = psFiltroDescripcion;
            DateTime ldtIniVigencia, ldtFinVigencia;
            int liRepetidos = 0;
            DateTime Today = DateTime.Today;

            if (pdtIniVigencia.DataValue.ToString() == "null")
            {
                pdtIniVigencia.DataValue = Today;
            }

            if (pdtFinVigencia.DataValue.ToString() == "null")
            {
                pdtFinVigencia.DataValue = new DateTime(2079, 1, 1);
            }

            ldtIniVigencia = pdtIniVigencia.Date;
            ldtFinVigencia = pdtFinVigencia.Date;

            setRequiredMessages();
            ValidarRepetidos();
            //Validar que no se repita la Descripción
            if (piCodRegistro.DataValue.ToString() != "null")//Edicion
            {
                lFiltroDescripcion += " and iCodRegistro <> " + piCodRegistro.DataValue;
            }
            try
            {
                liRepetidos = (int)DSODataAccess.ExecuteScalar("Select Count(*) from " + psNombreTabla + " where vchDescripcion = " + pvchDescripcion.DataValue + lFiltroDescripcion + " and iCodEntidad = " + piCodEntidad.DataValue);
            }
            catch (Exception e)
            {
                string lsTitulo = Globals.GetMsgWeb(false, "Titulo" + psNombreTabla);
                throw new KeytiaWebException(true, "ErrorConsulta", e, lsTitulo);
            }
            if (liRepetidos > 0)
            {
                psbErrores.Append("<li>" + Globals.GetMsgWeb("DescripcionRepetida") + "</li>");
            }

            if (ldtFinVigencia.CompareTo(ldtIniVigencia) < 0)
                psbErrores.Append("<li>" + Globals.GetMsgWeb("VigenciaFin", pdtIniVigencia.Descripcion, pdtFinVigencia.Descripcion) + "</li>");

        }
    }
}

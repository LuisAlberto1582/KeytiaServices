/*
 * Nombre:		    DMM
 * Fecha:		    20110610
 * Descripción:	    Clase para el configurador de Relaciones
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
    public class RelacionEdit : RegEdit
    {
        #region Propiedades
        //protected DSOCheckBox pbBajaCascada;
        protected string psFiltroCatalogo;
        protected DataTable dtFlags;
        #endregion

        public RelacionEdit()
        {
            psNombreTabla = "Relaciones";
            piMaxColumnSpan = 3;
            psFiltroDescripcion = " and iCodRelacion is null and dtIniVigencia <> dtFinVigencia ";
            Init += new EventHandler(RelacionEdit_Init);
        }
        
        protected void RelacionEdit_Init(System.Object sender, EventArgs e)
        {
            //pbBajaCascada = new DSOCheckBox();
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
                DSOControlDB ctl;
                int iRow = 0;
                if (IsFlag(dc.ColumnName))
                {
                    ctl = new DSOFlags();
                    iRow = Renglones["iCodCatalogo" + dc.ColumnName.Substring(dc.ColumnName.Length - 2)];
                }
                else if (IsAtrib(dc.ColumnName))
                {
                    ctl = new DSODropDownList();
                    ctl.DataValueDelimiter = "";
                    iRow = pTablaEdit.Rows.Count + 1;
                    Renglones.Add(dc.ColumnName, iRow);
                }
                else
                {
                    continue;
                }

                ctl.ID = dc.ColumnName;
                ctl.DataField = dc.ColumnName;
                ctl.Table = pTablaEdit;
                ctl.Row = iRow;
                ctl.CreateControls();

                if (IsFlag(dc.ColumnName))
                {
                    ctl.TcCtl.CssClass = "DSOTcCtl DSOTcCtlSub";
                }
                else if (IsAtrib(dc.ColumnName))
                {
                    ((DSODropDownList)ctl).SelectItemValue = "null";
                    ((DSODropDownList)ctl).SelectItemText = " ";
                    ctl.TcCtl.CssClass = "DSOTcCtl DSOTcCtlMain";
                    ctl.AddClientEvent("tipoCampo", "iCodCatalogo");
                }

                pHTControls.Add(ctl.ID, ctl);
            }
            //pbBajaCascada.ID = "bBajaCascada";
            //pbBajaCascada.ColumnSpan = piMaxColumnSpan;
            //pbBajaCascada.Row = pTablaEdit.Rows.Count + 1;
            //pbBajaCascada.Table = pTablaEdit;
            //pbBajaCascada.DataField = "bBajaCascada";
            //pbBajaCascada.CreateControls();
            //pHTControls.Add(pbBajaCascada.ID, pbBajaCascada);
            base.CreateTablaEdit();
        }

        protected override bool getPermiso(Permiso lpPermiso)
        {
            return DSONavegador.getPermiso("OpcRel", lpPermiso);
        }
        
        protected override DataTable getDataSource(string lsDataField, object Value)
        {
            DataTable dataSource = null;
            string lsFiltro = "";
            if (Value != null && Value != DBNull.Value && Value != System.Type.Missing)
            {
                lsFiltro = " and iCodRegistro in (" + Value.ToString() + ")";
            }
            if (IsAtrib(lsDataField))
            {
                dataSource = getDataSource(psFiltroCatalogo + lsFiltro, false);
            }
            else if (IsFlag(lsDataField))
            {
                dataSource = dtFlags;
            }

            return dataSource;
        }

        protected override void InitData()
        {
            psFiltroCatalogo = "Select iCodRegistro, vchDescripcion from Catalogos where iCodCatalogo is null and dtIniVigencia <> dtFinVigencia";

            dtFlags = new DataTable();
            dtFlags.Columns.Add(new DataColumn("id", typeof(int)));
            dtFlags.Columns.Add(new DataColumn("descripcion", typeof(string)));
            dtFlags.Rows.Add(new object[] { 1, Globals.GetMsgWeb("Exclusividad") });
            dtFlags.Rows.Add(new object[] { 2, Globals.GetMsgWeb("Responsabilidad") });
            dtFlags.Rows.Add(new object[] { 4, Globals.GetMsgWeb("Baja Cascada") });
        }
        
        protected override void InitFiltros()
        {
            piCodRegistro.Source = ResolveClientUrl("~/WebMethods.aspx/GetRelaciones");
            base.InitFiltros();
        }
        
        protected override void InitTablaEdit()
        {

        }

        protected override void ValidarRepetidos()
        {
            DataTable ldtiCodCatalogo = getDataSource(psFiltroCatalogo, false);
            DataTable ldtFlags = dtFlags;
            ArrayList lstiCodCatalogoRepetido = new ArrayList();
            ArrayList lstEntidadesRelacion = new ArrayList();
            string lsColCatRel = getColCatRel();
            System.Text.StringBuilder lsQuery = new System.Text.StringBuilder();
            bool bAtributo = false;
            foreach (DSOControlDB ctl in pHTControls.Values)
            {

                if (IsAtrib(ctl.DataField))
                {
                    if (ctl.DataValue.ToString() != "null")
                    {
                        bAtributo = true;
                        DataRow[] dr = ldtiCodCatalogo.Select("iCodRegistro = " + ctl.DataValue.ToString());
                        if (dr.Length > 0)
                        {
                            ldtiCodCatalogo.Rows.Remove(dr[0]);
                        }
                        else
                        {
                            lstiCodCatalogoRepetido.Add(ctl.Descripcion);
                        }
                        lstEntidadesRelacion.Add(ctl.DataValue.ToString() + " in (" + lsColCatRel + ")");
                    }
                    else
                    {
                        lstEntidadesRelacion.Add(ctl.DataField + " is null");
                    }
                }
                else if (ctl.DataValue.ToString() == "null" && !string.IsNullOrEmpty(ctl.RequiredMessage))
                {
                    psbErrores.Append("<li>" + ctl.RequiredMessage + "</li>");
                }
            }

            if (!bAtributo)
                psbErrores.Append("<li>" + Globals.GetMsgWeb("CamposInsuficientesRelaciones") + "</li>");

            foreach (string lsCampo in lstiCodCatalogoRepetido)
                psbErrores.Append("<li>" + Globals.GetMsgWeb("CampoRepetido", lsCampo) + "</li>");

            //Validar que no exista la relación
            lsQuery.Append("Select vchDescripcion from Relaciones where 1=1");// + pbBajaCascada.DataValue.ToString());
            
            if(piCodRegistro.HasValue)
                lsQuery.Append(" and iCodRegistro <> " + piCodRegistro.DataValue.ToString());

            if (lstEntidadesRelacion.Count > 0)
            {
                lsQuery.Append(" and ");
                lsQuery.Append(string.Join(" and ", (string[])lstEntidadesRelacion.ToArray(typeof(string))));
            }

            DataTable dtExiste = DSODataAccess.Execute(lsQuery.ToString());
            foreach (DataRow drExiste in dtExiste.Rows)
                psbErrores.Append("<li>" + Globals.GetMsgWeb("RegistroRepetido", drExiste["vchDescripcion"].ToString()) + "</li>");

        }
        
        public override void InitLanguage()
        {
            base.InitLanguage();

            dtFlags.Clear();
            dtFlags.Rows.Add(new object[] { 1, Globals.GetMsgWeb("Exclusividad") });
            dtFlags.Rows.Add(new object[] { 2, Globals.GetMsgWeb("Responsabilidad") });
            dtFlags.Rows.Add(new object[] { 4, Globals.GetMsgWeb("Baja Cascada") });
            foreach (DSOControlDB ctl in pHTControls.Values)
            {
                if (IsFlag(ctl.DataField))
                {
                    ctl.Descripcion = Globals.GetMsgWeb("Flags");
                    ((DSOFlags)ctl).DataSource = dtFlags;
                    ((DSOFlags)ctl).Fill();
                }
            }
            //pbBajaCascada.TrueText = Globals.GetMsgWeb("Si");
            //pbBajaCascada.FalseText = Globals.GetMsgWeb("No");
            piCodUsuario.Descripcion = Globals.GetMsgWeb("NomUsr");
            pdtFecUltAct.Descripcion = Globals.GetMsgWeb("dtFecUltAct");


        }
        
        protected override object SaveViewState()
        {
            Object baseState = base.SaveViewState();
            Object[] allStates = new Object[3];
            allStates[0] = baseState;
            allStates[1] = psFiltroCatalogo;
            allStates[2] = dtFlags;
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
                    psFiltroCatalogo = (string)myState[1];
                if (myState[2] != null)
                    dtFlags = (DataTable)myState[2];
            }
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

                ldt = DSODataAccess.Execute("select top 1 iCodRegistro from Relaciones where dtIniVigencia <> dtFinVigencia and iCodRelacion = " + piCodRegistro.DataValue.ToString());
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

                ldt = DSODataAccess.Execute("select top 1 iCodRegistro from Relaciones where dtIniVigencia <> dtFinVigencia and iCodRelacion = " + piCodRegistro.DataValue.ToString());
                if (ldt.Rows.Count > 0)
                {
                    psbErrores.Append("<li>" + Globals.GetMsgWeb("RelDatosRelaciones") + "</li>");
                }

                ldt = DSODataAccess.Execute("select top 1 iCodRegistro from Maestros where dtIniVigencia <> dtFinVigencia and " + piCodRegistro.DataValue.ToString() + " in(" + getColsTable("iCodRelacion","Maestros",true) + ")");
                if (ldt.Rows.Count > 0)
                {
                    psbErrores.Append("<li>" + Globals.GetMsgWeb("RelMaeConfig") + "</li>");
                }
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrValidateRecord", ex);
            }
        }

    }
}

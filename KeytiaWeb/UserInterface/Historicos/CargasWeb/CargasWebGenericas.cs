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

namespace KeytiaWeb.UserInterface
{
    public class CargasWebGenericas: CargasWeb
    {
        protected virtual string iCodRegistroAdd
        {
            get
            {
                return (string)ViewState["iCodRegistroAdd"];
            }
            set
            {
                ViewState["iCodRegistroAdd"] = value;
            }
        }

        protected string vchCodEntidadAdd
        {
            get
            {
                return (string)ViewState["vchCodEntidadAdd"];
            }
            set
            {
                ViewState["vchCodEntidadAdd"] = value;
            }
        }

        protected string vchCodMaestroAdd
        {
            get
            {
                return (string)ViewState["vchCodMaestroAdd"];
            }
            set
            {
                ViewState["vchCodMaestroAdd"] = value;
            }
        }

        public override void FillAjaxControls(bool lbIncluirFechaFin)
        {
            //Metodo para llenar controles cuyo valor puede cambiar mediante ajax
            psbQuery.Length = 0;
            psbQuery.AppendLine("Select iCodRegistro, vchDescripcion from Maestros ");
            psbQuery.AppendLine("Where iCodEntidad = " + piCodEntidad.DataValue.ToString());
            psbQuery.AppendLine(" and vchDescripcion like '%Carga%Generica%' ");
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

        protected override void InitGridPend()
        {
            DSOGridClientColumn lCol;
            int lTarget = 0;

            pPendGrid.ClearConfig();
            pPendGrid.Config.sDom = "<\"H\"Rlf>tr<\"F\"pi>"; //con filtro global
            pPendGrid.Config.bAutoWidth = true;
            pPendGrid.Config.sScrollX = "100%";
            pPendGrid.Config.sScrollY = "400px";
            pPendGrid.Config.sPaginationType = "full_numbers";
            pPendGrid.Config.bJQueryUI = true;
            pPendGrid.Config.bProcessing = true;
            pPendGrid.Config.bServerSide = true;
            //            pPendGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj
            //                                            + ".fnServerPendFacturas(this, sSource, aoData, fnCallback);}";
            pPendGrid.Config.fnServerData = "function(sSource, aoData, fnCallback){" + pjsObj
                                            + ".fnServerPendientes(this, sSource, aoData, fnCallback);}";
            pPendGrid.Config.sAjaxSource = ResolveUrl("~/WebMethods.aspx/GetHisDetallados");

            pFieldsPend = null;

            if (iCodMaestroPend != null)
            {
                pFieldsPend = new HistoricFieldCollection(int.Parse(iCodCarga), int.Parse(iCodMaestroPend));
            }

            if (pFieldsPend != null)
            {

                //lCol = new DSOGridClientColumn();
                //lCol.sName = "vchBoton";
                //lCol.aTargets.Add(lTarget++);
                //lCol.sWidth = "50px";
                //lCol.sClass = "td.TdEdit";
                //pPendGrid.Config.aoColumnDefs.Add(lCol);

                lCol = new DSOGridClientColumn();
                lCol.sName = "iCodRegistro";
                lCol.aTargets.Add(lTarget++);
                lCol.bVisible = false;
                pPendGrid.Config.aoColumnDefs.Add(lCol);

                lCol = new DSOGridClientColumn();
                lCol.sName = "vchDescripcion";
                lCol.aTargets.Add(lTarget++);
                pPendGrid.Config.aoColumnDefs.Add(lCol);

                foreach (KeytiaBaseField lField in pFieldsPend)
                {
                    if (lField.ShowInGrid)
                    {
                        lCol = new DSOGridClientColumn();
                        lCol.sName = lField.Column;
                        lCol.aTargets.Add(lTarget++);
                        pPendGrid.Config.aoColumnDefs.Add(lCol);
                    }
                }

                lCol = new DSOGridClientColumn();
                lCol.sName = "dtFecha";
                lCol.bVisible = false;
                lCol.aTargets.Add(lTarget++);
                lCol.sWidth = "120px";
                pPendGrid.Config.aoColumnDefs.Add(lCol);

                lCol = new DSOGridClientColumn();
                lCol.sName = "dtFecUltAct";
                lCol.aTargets.Add(lTarget++);
                pPendGrid.Config.aoColumnDefs.Add(lCol);

                if (pPendGrid.Config.aoColumnDefs.Count > 10)
                {
                    pPendGrid.Config.sScrollXInner = (pPendGrid.Config.aoColumnDefs.Count * 20).ToString() + "%";
                }

                pPendGrid.Grid.Visible = true;
                pPendGrid.Fill();
            }
        }

        protected override void InitAccionesSecundarias()
        {
            base.InitAccionesSecundarias();

            if (!String.IsNullOrEmpty(vchCodEntidadAdd))
            {
                pSubHistorico.SetEntidad(vchCodEntidadAdd);
                if (!String.IsNullOrEmpty(vchCodMaestroAdd))
                {
                    pSubHistorico.SetMaestro(vchCodMaestroAdd);
                }
                pSubHistorico.PostGrabarClick += new EventHandler(pHisClavesXDefinir_PostGrabarClick);
                pSubHistorico.PostCancelarClick += new EventHandler(pHisClavesXDefinir_PostCancelarClick);
            }


        }

        public override void InitLanguage()
        {
            base.InitLanguage();
            pExpGridPend.Title = Globals.GetMsgWeb("CargPendXIndenTitle");

            if (State == HistoricState.SubHistorico)
            {
                pSubHistorico.Title = this.Title + " / " + Globals.GetLangItem("", "Entidades", vchCodEntidadAdd);
                pSubHistorico.AlertTitle = this.AlertTitle + " / " + Globals.GetLangItem("", "Entidades", vchCodEntidadAdd);
                pSubHistorico.InitLanguage();
            }
        }

        public override void SetHistoricState(HistoricState s)
        {
            base.SetHistoricState(s);
            pbtnEditar.Visible = false;
            this.Visible = true;

            if (s == HistoricState.SubHistorico)
            {
                this.Visible = false;
                pSubHistorico.Visible = true;
            }

            OcultaCampos();

            State = s;
        }

        //protected override void IniMaestrosCargas()
        //{
        //    if (iCodRegistro != "null")
        //    {
        //        InitAccionesSecundarias();

        //        iCodCarga = iCodCatalogo;

        //        psbQuery.Length = 0;
        //        psbQuery.AppendLine("Select Top 1 convert(varchar(20),iCodRegistro) from Maestros ");
        //        psbQuery.AppendLine("Where iCodRegistro in (Select iCodMaestro from Pendientes ");
        //        psbQuery.AppendLine("                          Where iCodCatalogo = " + iCodCarga.ToString() + ") ");
        //        //psbQuery.AppendLine("And vchDescripcion not like '%Factura%'");
        //        psbQuery.AppendLine("Order by vchDescripcion");

        //        iCodMaestroPend = (string)DSODataAccess.ExecuteScalar(psbQuery.ToString());

        //        psbQuery.Length = 0;
        //        psbQuery.AppendLine("Select iCodRegistro, vchDescripcion from Maestros ");
        //        psbQuery.AppendLine("Where iCodRegistro in (Select iCodMaestro from Pendientes ");
        //        psbQuery.AppendLine("                       Where iCodCatalogo = " + iCodCarga.ToString() + ") ");
        //        //psbQuery.AppendLine("And vchDescripcion not like '%Factura%'");
        //        psbQuery.AppendLine("Order by vchDescripcion ");

        //        piCodMaestroPend.DataSource = psbQuery.ToString();
        //        piCodMaestroPend.Fill();
        //    }
        //}

        protected virtual void ConsultarDatos()
        {
            String vchCodigoAdd = "";
            int liCodRegistro = 0;
            Boolean lbExisteRegistro = false;
            DataTable lKDBTable;

            if (iCodRegistroAdd != "null")
            {
                vchCodEntidadAdd = DSODataAccess.ExecuteScalar(
                    "Select vchCodigo from Catalogos " +
                    " Where iCodRegistro = (Select iCodEntidad from Maestros " +
                    "                       Where iCodRegistro = " + iCodMaestroPend.ToString() + ")").ToString();

                vchCodMaestroAdd = DSODataAccess.ExecuteScalar(" Select vchDescripcion from Maestros " +
                                                                " Where iCodRegistro = " + iCodMaestroPend.ToString()).ToString();

                //Obtener la descripcion del registro
                DataRow lDataRow = DSODataAccess.ExecuteDataRow(" Select * from Pendientes " +
                                                                " Where iCodRegistro = " + iCodRegistroAdd.ToString());

                vchCodigoAdd = lDataRow["vchDescripcion"].ToString();

                //Obtener el codigo si existe
                lKDBTable = pKDB.GetHisRegByEnt(vchCodEntidadAdd, vchCodMaestroAdd, "vchCodigo = '" + vchCodigoAdd.Trim() + "'");
                if (lKDBTable.Rows.Count > 0)
                {
                    liCodRegistro = (int)lKDBTable.Rows[0]["iCodRegistro"];
                    lKDBTable = pKDB.GetHisRegByEnt(vchCodEntidadAdd, vchCodMaestroAdd, "iCodRegistro = " + liCodRegistro.ToString());
                    if (lKDBTable.Rows.Count > 0)
                    {
                        lDataRow = lKDBTable.Rows[0];
                        vchCodigoAdd = lDataRow["vchCodigo"].ToString();
                        lbExisteRegistro = true;
                    }
                }

                if (!String.IsNullOrEmpty(vchCodEntidadAdd))
                {
                    PrevState = State;
                    InitSubHistorico("HisClavesXDefinir" + liCodRegistro);
                    SetHistoricState(HistoricState.SubHistorico);
                    pSubHistorico.SetEntidad(vchCodEntidadAdd);
                    pSubHistorico.SetMaestro(vchCodMaestroAdd);
                    pSubHistorico.FillControls();
                    pSubHistorico.InitMaestro();
                    if (lbExisteRegistro)
                    {
                        pSubHistorico.iCodRegistro = liCodRegistro.ToString();
                        pSubHistorico.ConsultarRegistro();
                    }
                    else
                    {
                        pSubHistorico.Fields.SetValues(lDataRow);
                        pSubHistorico.vchCodigo.DataValue = vchCodigoAdd;
                        pSubHistorico.vchDescripcion.DataValue = lDataRow["vchDescripcion"];
                    }
                    pSubHistorico.SetHistoricState(HistoricState.Edicion);
                    pSubHistorico.Fields.EnableFields();
                }
            }
        }

        public override void RaisePostBackEvent(string eventArgument)
        {
            base.RaisePostBackEvent(eventArgument);
            if (eventArgument.StartsWith("btnAgregarPend"))
            {
                int liCodRegistro;
                if (eventArgument.Split(':').Length == 2
                    && int.TryParse(eventArgument.Split(':')[1], out liCodRegistro))
                {
                    iCodRegistroAdd = liCodRegistro.ToString();
                    ConsultarDatos();
                }
            }
        }

        void pHisClavesXDefinir_PostCancelarClick(object sender, EventArgs e)
        {
            vchCodEntidadAdd = null;
            vchCodMaestroAdd = null;
            pSubHistorico.CleanEntidad();
            pSubHistorico.SetHistoricState(HistoricState.Inicio);
            this.Parent.Controls.Remove(pSubHistorico);
            this.SubHistoricoID = null;
            SetHistoricState(PrevState);
        }

        void pHisClavesXDefinir_PostGrabarClick(object sender, EventArgs e)
        {
            if (pSubHistorico.State != HistoricState.Edicion)
            {
                vchCodEntidadAdd = null;
                vchCodMaestroAdd = null;
                pSubHistorico.CleanEntidad();
                pSubHistorico.SetHistoricState(HistoricState.Inicio);
                this.Parent.Controls.Remove(pSubHistorico);
                this.SubHistoricoID = null;
                SetHistoricState(PrevState);
            }
        }

        //RZ.20131127 Se añade override al metodo 
        /// <summary>
        /// Se encarga de bloquear algunos campos y ademas de establecer algunos valores default en la carga
        /// </summary>
        protected override void BloqueaCampos()
        {
            //Mandar llamar el metodo de la clase base
            base.BloqueaCampos();

            //Solo si la carga es de tipo Claro
            if (vchDesMaestro == "Cargas Factura Claro")
            {
                //Establecer el valor de la clase a instanciar y deshabilitar el campo
                pFields.GetByConfigName("Clase").DataValue = "KeytiaServiceBL.CargaFacturas.CargaFacturaClaro";
                pFields.GetByConfigName("Clase").DisableField();

                //Agregar como clave del historico un timestamp e inhabilitar el campo
                pvchCodigo.DataValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                pvchCodigo.TextBox.Enabled = false;

                //Si no tiene valor el campo Descripción entonces...
                if (!pvchDescripcion.HasValue)
                {
                    pvchDescripcion.DataValue = "Carga Factura Claro";
                }
            }

            //RZ.20140117 Personal. Solo cuando se trate de la carga de factura de personal dejara esta configuracion
            if (vchDesMaestro == "Cargas Factura Personal")
            {
                //Establecer el valor de la clase a instanciar y deshabilitar el campo
                pFields.GetByConfigName("Clase").DataValue = "KeytiaServiceBL.CargaFacturas.CargaFacturaPersonal";
                pFields.GetByConfigName("Clase").DisableField();

                //Agregar como clave del historico un timestamp e inhabilitar el campo
                pvchCodigo.DataValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                pvchCodigo.TextBox.Enabled = false;

                //Si no tiene valor el campo Descripción entonces...
                if (!pvchDescripcion.HasValue)
                {
                    pvchDescripcion.DataValue = "Carga Factura Personal";
                }
            }

            //RZ.20140513 Personal. Solo cuando se trate de la carga de factura de personal dejara esta configuracion
            if (vchDesMaestro == "Cargas Factura Claro Colombia")
            {
                //Establecer el valor de la clase a instanciar y deshabilitar el campo
                pFields.GetByConfigName("Clase").DataValue = "KeytiaServiceBL.CargaFacturas.CargaFacturaClaroColombia";
                pFields.GetByConfigName("Clase").DisableField();

                //Agregar como clave del historico un timestamp e inhabilitar el campo
                pvchCodigo.DataValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                pvchCodigo.TextBox.Enabled = false;

                //Si no tiene valor el campo Descripción entonces...
                if (!pvchDescripcion.HasValue)
                {
                    pvchDescripcion.DataValue = "Carga Factura Claro Colombia";
                }
            }

            //RZ.20140522 Verizon. Solo cuando se trate de la carga de factura de verizon dejara esta configuracion
            if (vchDesMaestro == "Cargas Factura Verizon")
            {
                //Establecer el valor de la clase a instanciar y deshabilitar el campo
                pFields.GetByConfigName("Clase").DataValue = "KeytiaServiceBL.CargaFacturas.CargaFacturaVerizon";
                pFields.GetByConfigName("Clase").DisableField();

                //Agregar como clave del historico un timestamp e inhabilitar el campo
                pvchCodigo.DataValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                pvchCodigo.TextBox.Enabled = false;

                //Si no tiene valor el campo Descripción entonces...
                if (!pvchDescripcion.HasValue)
                {
                    pvchDescripcion.DataValue = "Carga Factura Verizon";
                }
            }

            //RZ.20140609 Telum. Solo cuando se trate de la carga de factura de telum dejara esta configuracion
            if (vchDesMaestro == "Cargas Factura Telum")
            {
                //Establecer el valor de la clase a instanciar y deshabilitar el campo
                pFields.GetByConfigName("Clase").DataValue = "KeytiaServiceBL.CargaFacturas.CargaFacturaTelum";
                pFields.GetByConfigName("Clase").DisableField();

                //Agregar como clave del historico un timestamp e inhabilitar el campo
                pvchCodigo.DataValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
                pvchCodigo.TextBox.Enabled = false;

                //Si no tiene valor el campo Descripción entonces...
                if (!pvchDescripcion.HasValue)
                {
                    pvchDescripcion.DataValue = "Carga Factura Telum";
                }
            }

            //NZ 20180418 Se agrega un pequeño mensaje para advertir al usuario de lo que implica activar la bandera de Generar Inventario
            if (vchDesMaestro == "Cargas Factura Axtel TIM")
            {
                pFields.GetByConfigName("BanderasCargaAxtelTIM").DataValue = 0;
                //KeytiaBaseField lField = pFields.GetByConfigName("BanderasCargaAxtelTIM");
                //var check = lField.DSOControlDB as DSOCheckBoxList;

                string title = "Advertencia";
                StringBuilder lsb = new StringBuilder(); lsb.Length = 0;

                lsb.AppendLine("<script type='text/javascript'>");
                lsb.AppendLine("    window.onload = function () {");
                lsb.AppendLine("        var chk = document.querySelectorAll('[keytiafield=\"BanderasCargaAxtelTIM\"]')[0]");
                lsb.AppendLine("            var checkboxes = chk.getElementsByTagName('INPUT');");
                lsb.AppendLine("            for (var i = 0; i < checkboxes.length; i++) {");
                lsb.AppendLine("                checkboxes[i].onchange = function () {");
                lsb.AppendLine("                    for (var i = 0; i < checkboxes.length; i++) {");
                lsb.AppendLine("                        if (checkboxes[i].checked && checkboxes[i].parentNode.getElementsByTagName('LABEL')[0].innerHTML == 'Generar Inventario de Recursos') {");
                lsb.AppendLine("                            jAlert(\"" + Globals.GetMsgWeb(false, "AdverGenerarInventarioRecursos") + "\", \"" + title + "\");");
                lsb.AppendLine("                        }");
                lsb.AppendLine("                    }");
                lsb.AppendLine("                };");
                lsb.AppendLine("            }");
                lsb.AppendLine("        };");
                lsb.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), pjsObj, lsb.ToString(), true, false);
            }
        }

    }
}
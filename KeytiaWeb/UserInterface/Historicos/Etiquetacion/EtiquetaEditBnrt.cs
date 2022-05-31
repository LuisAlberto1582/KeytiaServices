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
using KeytiaWeb.UserInterface.DashboardFC;

namespace KeytiaWeb.UserInterface
{
    public class EtiquetaEditBnrt : EtiquetaEdit
    {
        #region Campos
        protected DSONumberEdit pNumTotProveedor; //RJ.20160616
        protected DSONumberEdit pNumTotOutsourcing; //RJ.20160616
        protected DSOTextBox pTxtNumTotRServer; //Proveedor RJ.20160616
        protected DSOTextBox pTxtNumTotOServer; //OutSourcing RJ.20160616
        #endregion


        #region Metodos

        public override void LoadScripts()
        {
            base.LoadScripts();
            DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), "EtiquetaEditBnrt.js", "<script src='" + ResolveClientUrl("~/UserInterface/Historicos/Etiquetacion/EtiquetaEditBnrt.js?V=1") + "' type='text/javascript'></script>\r\n", true, false);
        }

        /// <summary>
        /// Obtiene el listado de registros que se deben actualizar en DetalleCDR,
        /// mismos que corresponden al Empleado cuya fecha sea igual o mayor al periodo 
        /// que se está etiquetando
        /// </summary>
        /// <param name="iCodEmpleado">Emple</param>
        /// <param name="IniPeriodo">Fecha inicio del periodo</param>
        /// <param name="FinPeriodo">Fecha fin del periodo (ya no se utiliza)</param>
        /// <returns></returns>
        protected override bool ActualizarEtiquetaEnDetalleCDR(string iCodEmpleado, DateTime IniPeriodo, DateTime FinPeriodo)
        {
            //Obtiene el listado de registros que se requiere actualizar en Detalle
            //la actualización se hace tomando como base el campo icodRegistro
            psbQuery.Length = 0;
            psbQuery.AppendLine("select CDR.iCodRegistro, GEtiqueta = isnull(Dir.GEtiqueta,0), Etiqueta = isnull(Dir.Etiqueta,'')");
            psbQuery.AppendLine("from  " + DSODataContext.Schema + ".[VisDetallados('Detall','DetalleCDR','Español')] CDR,");
            psbQuery.AppendLine(DSODataContext.Schema + ".[VisDirectorio('Español')] Dir");
            psbQuery.AppendLine("where Dir.Emple = " + iCodEmpleado);
            psbQuery.AppendLine("and Dir.dtIniVigencia <= '" + IniPeriodo.ToString("yyyy-MM-dd") + "'");
            psbQuery.AppendLine("and Dir.dtFinVigencia > '" + IniPeriodo.ToString("yyyy-MM-dd") + "'");
            psbQuery.AppendLine("and CDR.Emple = Dir.Emple");
            psbQuery.AppendLine("and CDR.Emple = " + iCodEmpleado);
            psbQuery.AppendLine("and CDR.TelDest = Dir.vchCodigo");
            psbQuery.AppendLine("and CDR.TpLlam <> 'Entrada'");
            psbQuery.AppendLine("and Dir.dtIniVigencia <= CDR.FechaInicio");
            psbQuery.AppendLine("and Dir.dtFinVigencia > CDR.FechaInicio");
            psbQuery.AppendLine("and CDR.FechaInicio >= '" + IniPeriodo.ToString("yyyy-MM-dd") + "'");

            //20141114.RJ COMENTO ESTA LÍNEA PARA QUE ACTUALICE LAS LLAMADAS NO SÓLO DEL PERIODO EN CURSO SINO TODAS LAS QUE SE HAYAN GENERADO DE ÉL EN ADELANTE
            //psbQuery.AppendLine("and CDR.FechaInicio < '" + FinPeriodo.AddDays(1).ToString("yyyy-MM-dd") + "'"); 

            DataTable lCDR = DSODataAccess.Execute(psbQuery.ToString());


            foreach (DataRow lDataRowCDR in lCDR.Rows)
            {
                //Actualiza el campo GEtiqueta y Etiqueta en DetalleCDR
                psbQuery.Length = 0;
                psbQuery.AppendLine("Update CDR Set");
                psbQuery.AppendLine("   CDR.GEtiqueta = " + lDataRowCDR["GEtiqueta"] + ",");
                psbQuery.AppendLine("   CDR.Etiqueta = '" + lDataRowCDR["Etiqueta"].ToString().Replace("'", "''") + "'");
                psbQuery.AppendLine("from  " + DSODataContext.Schema + ".[VisDetallados('Detall','DetalleCDR','Español')] CDR");
                psbQuery.AppendLine("where CDR.iCodRegistro = " + lDataRowCDR["iCodRegistro"]);

                pbEtiquetacionCorrecta = DSODataAccess.ExecuteNonQuery(psbQuery.ToString());

                if (!pbEtiquetacionCorrecta)
                {
                    break;
                }
            }

            return pbEtiquetacionCorrecta;
        }

        protected override void BajaProcesoEtiqueta()
        {
            try
            {
                KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
                phtValues = new Hashtable();

                DataTable ldt = pKDB.GetHisRegByEnt("ProEtiqueta", "Proceso Etiquetacion", "iCodRegistro ='" + piProcesoEtiqueta + "'");
                if (ldt == null || ldt.Rows.Count == 0)
                {
                    return;
                }
                phtValues.Add("dtFinVigencia", ldt.Rows[0]["dtIniVigencia"]);

                //Da de baja el Proceso de Etiquetación
                lCargasCOM.ActualizaRegistro("Historicos", "ProEtiqueta", "Proceso Etiquetacion", phtValues, piProcesoEtiqueta, (int)Session["iCodUsuarioDB"]);


                //---------------------------------------------------------------------------------------
                //RJ.20160616
                //Aplica la baja en la tabla que contiene los consumos de todas las categorías
                StringBuilder lsbQuery = new StringBuilder();
                lsbQuery.AppendLine("update ProcesoEtiquetacion");
                lsbQuery.AppendLine("set dtFinvigencia = dtInivigencia, dtfecultact = getdate()");
                lsbQuery.AppendLine("where iCodRegistroEnHist = " + piProcesoEtiqueta.ToString());
                DSODataAccess.ExecuteNonQuery(lsbQuery.ToString());
                //---------------------------------------------------------------------------------------
            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveRecord", ex);
            }
        }

        protected override void InitRegistroResumen()
        {
            int liRow = 1;
            pTablaResumen.ID = "TablaTotales";
            pTablaResumen.Width = Unit.Percentage(100);

            pExpResumen.ID = "ResumenWrapper";
            pExpResumen.StartOpen = true;
            pExpResumen.CreateControls();
            pExpResumen.Panel.Controls.Clear();
            pExpResumen.Panel.Controls.Add(pTablaResumen);

            liRow = 1;

            pNumTotPersonal.ID = "dTotPersonal";
            pNumTotPersonal.Table = pTablaResumen;
            pNumTotPersonal.Row = liRow++;
            pNumTotPersonal.ColumnSpan = 3;
            pNumTotPersonal.DataField = "dTotPersonal";
            pNumTotPersonal.CreateControls();
            pNumTotPersonal.NumberBox.ReadOnly = true;

            pNumTotLaboral.ID = "dTotLaboral";
            pNumTotLaboral.Table = pTablaResumen;
            pNumTotLaboral.Row = liRow++;
            pNumTotLaboral.ColumnSpan = 3;
            pNumTotLaboral.DataField = "dTotLaboral";
            pNumTotLaboral.CreateControls();
            pNumTotLaboral.NumberBox.ReadOnly = true;



            //RJ.20160616
            pNumTotProveedor.ID = "dTotProveedor";
            pNumTotProveedor.Table = pTablaResumen;
            pNumTotProveedor.Row = liRow++;
            pNumTotProveedor.ColumnSpan = 3;
            pNumTotProveedor.DataField = "dTotProveedor";
            pNumTotProveedor.CreateControls();
            pNumTotProveedor.NumberBox.ReadOnly = true;

            //RJ.20160616
            pNumTotOutsourcing.ID = "dTotOutsourcing";
            pNumTotOutsourcing.Table = pTablaResumen;
            pNumTotOutsourcing.Row = liRow++;
            pNumTotOutsourcing.ColumnSpan = 3;
            pNumTotOutsourcing.DataField = "dTotOutsourcing";
            pNumTotOutsourcing.CreateControls();
            pNumTotOutsourcing.NumberBox.ReadOnly = true;


            pNumTotNI.ID = "dTotNI";
            pNumTotNI.Table = pTablaResumen;
            pNumTotNI.Row = liRow++;
            pNumTotNI.ColumnSpan = 3;
            pNumTotNI.DataField = "dTotNI";
            pNumTotNI.CreateControls();
            pNumTotNI.NumberBox.ReadOnly = true;
            pNumTotal.ID = "dTotal";
            pNumTotal.Table = pTablaResumen;
            pNumTotal.Row = liRow++;
            pNumTotal.ColumnSpan = 3;
            pNumTotal.DataField = "dTotal";
            pNumTotal.CreateControls();
            pNumTotal.NumberBox.ReadOnly = true;



            //Controles con valores ocultos para trabajar ajax
            pTxtNumTotPServer.ID = "sTotPServer";
            pTxtNumTotPServer.CreateControls();
            pTxtNumTotPServer.TextBox.Style["display"] = "none";

            pTxtNumTotLServer.ID = "sTotLServer";
            pTxtNumTotLServer.CreateControls();
            pTxtNumTotLServer.TextBox.Style["display"] = "none";


            //RJ.20160616
            pTxtNumTotRServer.ID = "sTotRServer";
            pTxtNumTotRServer.CreateControls();
            pTxtNumTotRServer.TextBox.Style["display"] = "none";
            pTxtNumTotOServer.ID = "sTotOServer";
            pTxtNumTotOServer.CreateControls();
            pTxtNumTotOServer.TextBox.Style["display"] = "none";


            pTxtNumTotNServer.ID = "sTotNServer";
            pTxtNumTotNServer.CreateControls();
            pTxtNumTotNServer.TextBox.Style["display"] = "none";
            pTxtNumTotServer.ID = "sTotServer";
            pTxtNumTotServer.CreateControls();
            pTxtNumTotServer.TextBox.Style["display"] = "none";



            pTxtWindowVisible.ID = "txtWdVisible";
            pTxtWindowVisible.CreateControls();
            pTxtWindowVisible.TextBox.Style["display"] = "none";

            InitRegistroFiltros(liRow);
        }

        /// <summary>
        /// Inserta un registro en la vista visHistoricos('ProEtiqueta','Proceso Etiquetacion')
        /// con los datos del Empleado, el periodo y los totales de cada categoría
        /// </summary>
        protected override void GrabarProcesoEtiqueta()
        {
            try
            {
                pbEtiquetacionCorrecta = false;
                KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
                phtValues = new Hashtable();


                //RJ.20160616
                string vchCodigo = "'" + iCodEmpleado + "_" + IniPeriodo.Month.ToString() + "/" + IniPeriodo.Year.ToString() + "_" + piNumEtiquetaPeriodo + "'";
                string vchDescripcion = "'Per: " + IniPeriodo.Month.ToString() + "/" + IniPeriodo.Year.ToString() + "_[" + piNumEtiquetaPeriodo + "]'";
                phtValues.Add("vchCodigo", vchCodigo);
                phtValues.Add("vchDescripcion", vchDescripcion);

                phtValues.Add("{Emple}", iCodEmpleado);
                phtValues.Add("{IniPer}", IniPeriodo);
                phtValues.Add("{FinPer}", FinPeriodo);
                phtValues.Add("dtIniVigencia", DateTime.Today);
                phtValues.Add("{TOTALSUMO}", pTxtNumTotServer.TextBox.Text);
                phtValues.Add("{ConsumoLab}", pTxtNumTotLServer.TextBox.Text);
                phtValues.Add("{ConsumoPer}", pTxtNumTotPServer.TextBox.Text);
                phtValues.Add("{ConsumoNI}", pTxtNumTotNServer.TextBox.Text);
                phtValues.Add("iCodUsuario", Session["iCodUsuario"]);

                //RJ.20160616
                phtValues.Add("{ConsumoProveedor}", pTxtNumTotRServer.TextBox.Text);
                //phtValues.Add("{ConsumoOutsourcing}", pTxtNumTotOServer.TextBox.Text);

                //Mandar llamar al COM para grabar los datos del historico
                piProcesoEtiqueta = lCargasCOM.InsertaRegistro(phtValues, "Historicos", "ProEtiqueta", "Proceso Etiquetacion", false, (int)Session["iCodUsuarioDB"], false);

                if (piProcesoEtiqueta < 0)
                {
                    throw new KeytiaWebException("ErrSaveRecord");
                }

                //-------------------------------------------------------------------------------------
                //RJ.20160616
                //Se inserta un registro en la tabla ProcesoEtiquetacion
                string piCodMaestro = Convert.ToString(DSODataAccess.ExecuteScalar("select min(icodRegistro) from Maestros where vchdescripcion = 'Proceso Etiquetacion'"));
                string piCodCatalogoRegistro = Convert.ToString(DSODataAccess.ExecuteScalar("select icodcatalogo from Historicos where icodregistro = " + piProcesoEtiqueta.ToString()));

                StringBuilder lsbQueryInsertTabla = new StringBuilder();
                lsbQueryInsertTabla.AppendLine("insert into ProcesoEtiquetacion");
                lsbQueryInsertTabla.AppendLine("(iCodRegistroEnHist, iCodCatalogo,iCodMaestro,vchCodigo,vchDescripcion,Emple,ConsumoLab,ConsumoPer,ConsumoNI,TotalSumo,ConsumoProveedor,ConsumoOutsourcing,IniPer,FinPer,dtIniVigencia,dtFinVigencia,iCodUsuario,dtFecUltAct)");
                lsbQueryInsertTabla.AppendLine("values (");
                lsbQueryInsertTabla.AppendLine(piProcesoEtiqueta.ToString());//
                lsbQueryInsertTabla.AppendLine("," + piCodCatalogoRegistro);
                lsbQueryInsertTabla.AppendLine("," + piCodMaestro);
                lsbQueryInsertTabla.AppendLine("," + vchCodigo + "");
                lsbQueryInsertTabla.AppendLine("," + vchDescripcion + "");
                lsbQueryInsertTabla.AppendLine("," + iCodEmpleado);
                lsbQueryInsertTabla.AppendLine("," + pTxtNumTotLServer.TextBox.Text); //ConsumoLab
                lsbQueryInsertTabla.AppendLine("," + pTxtNumTotPServer.TextBox.Text); //ConsumoPer
                lsbQueryInsertTabla.AppendLine("," + pTxtNumTotNServer.TextBox.Text); //ConsumoNI
                lsbQueryInsertTabla.AppendLine("," + pTxtNumTotServer.TextBox.Text); //ConsumoTotal
                lsbQueryInsertTabla.AppendLine("," + pTxtNumTotRServer.TextBox.Text); //ConsumoProveedor
                lsbQueryInsertTabla.AppendLine("," + pTxtNumTotOServer.TextBox.Text); //ConsumoOutSourcing
                lsbQueryInsertTabla.AppendLine(", '" + IniPeriodo.ToString("yyyy-MM-dd HH:mm:ss") + "'"); //IniPer
                lsbQueryInsertTabla.AppendLine(", '" + FinPeriodo.ToString("yyyy-MM-dd HH:mm:ss") + "'"); //FinPer
                lsbQueryInsertTabla.AppendLine(", convert(varchar,GetDate(),112) "); //Inivigencia
                lsbQueryInsertTabla.AppendLine("," + "'2079-01-01'"); //Inivigencia
                lsbQueryInsertTabla.AppendLine("," + "NULL"); //iCodUsuar
                lsbQueryInsertTabla.AppendLine(", convert(varchar,GetDate(),112) "); //dtfecultact
                lsbQueryInsertTabla.AppendLine(")");

                pbEtiquetacionCorrecta = DSODataAccess.ExecuteNonQuery(lsbQueryInsertTabla.ToString());

                //pbEtiquetacionCorrecta = true;
                //-------------------------------------------------------------------------------------


            }
            catch (Exception ex)
            {
                throw new KeytiaWebException("ErrSaveRecord", ex);
            }

        }


        

        /// <summary>
        /// Actualiza el Directorio con los números que etiquetó el Empleado
        /// Actualiza los campos GEtiqueta y Etiqueta en DetalleCDR
        /// </summary>
        protected override void GrabarRegistro()
        {
            KeytiaCOM.CargasCOM lCargasCOM = new KeytiaCOM.CargasCOM();
            DataTable ldtDirectorioEmple = new DataTable();
            phtValues = new Hashtable();
            pbEtiquetacionCorrecta = true;

            DateTime ldtFinPerido = new DateTime(2079, 1, ValidarDiasMes(2079, 1, piDiaCorte));
            if (piHisPreviaEtiqueta == 0 || pdtCorte > FinPeriodo.AddDays(1))
            {
                //La vigencia de la etiqueta del telefono sólo será para el periodo señalado si:
                //  *El cliente No tiene activa la bandera "Guardar Historia de Etiqueta" 
                //  *El periodo que se está etiquetando fue habilitado extemporáneo
                //La fecha de periodo fin muestra un día menos a la vigencia almacenada.
                ldtFinPerido = FinPeriodo.AddDays(1);
            }

            pKDB.FechaVigencia = IniPeriodo.Date;
            ldtDirectorioEmple = pKDB.GetHisRegByEnt("Directorio", "Directorio Telefonico", "{Emple} = " + iCodEmpleado);
            pKDB.FechaVigencia = DateTime.Today;
            int liCatProEtiqueta = (int)pKDB.GetHisRegByEnt("ProEtiqueta", "Proceso Etiquetacion", "iCodRegistro=" + piProcesoEtiqueta).Rows[0]["iCodCatalogo"];

            List<string> lstRegsDir = new List<string>();
            try
            {
                //Se graba registro en Directorio y en Detallados
                DataRow[] ldrArray;
                int liRegTelDirectorio = int.MinValue;
                bool lbActualizo = true;
                string lsEtiqueta = "";

                pFieldsResumen = new HistoricFieldCollection(int.Parse(iCodEntidadResumen), int.Parse(iCodMaestroResumen));


                if (pdtResumenConsultado == null || pdtResumenConsultado.Rows.Count == 0)
                {
                    goto GrabaDetalle;  //Actualiza los números en DetalleCDR
                }


                for (int liRow = 0; liRow < pdtResumenConsultado.Rows.Count; liRow++)
                {
                    liRegTelDirectorio = int.MinValue;
                    string lsNumero = (string)KeytiaServiceBL.Util.IsDBNull(pdtResumenConsultado.Rows[liRow]["vchDescripcion"].ToString().Replace("'", ""), "");

                    if (ldtDirectorioEmple != null && ldtDirectorioEmple.Rows.Count > 0)
                    {
                        //Busca si el empleado ya ha etiquetado anteriormente el número                        
                        ldrArray = ldtDirectorioEmple.Select("vchCodigo = '" + lsNumero + "'");
                        if (ldrArray != null && ldrArray.Length > 0)
                        {
                            liRegTelDirectorio = (int)ldrArray[0]["iCodRegistro"];
                        }
                    }

                    lbActualizo = true; //inicializa variable
                    phtValues.Clear();
                    phtValues.Add("{Emple}", iCodEmpleado);
                    phtValues.Add("dtIniVigencia", IniPeriodo);
                    phtValues.Add("dtFinVigencia", ldtFinPerido);
                    phtValues.Add("iCodUsuario", Session["iCodUsuario"]);

                    int liGpo = int.Parse(KeytiaServiceBL.Util.IsDBNull(pdtResumenConsultado.Rows[liRow]["GEtiqueta"], 0).ToString());
                    phtValues.Add("{GEtiqueta}", liGpo);
                    phtValues.Add("vchCodigo", (string)KeytiaServiceBL.Util.IsDBNull(pdtResumenConsultado.Rows[liRow][pFieldsResumen.GetByConfigName("TelDest").Column], ""));
                    phtValues.Add("vchDescripcion", (string)KeytiaServiceBL.Util.IsDBNull(pdtResumenConsultado.Rows[liRow][pFieldsResumen.GetByConfigName("TelDest").Column], "") + "_" + iCodEmpleado);
                    lsEtiqueta = (string)KeytiaServiceBL.Util.IsDBNull(pdtResumenConsultado.Rows[liRow][pFieldsResumen.GetByConfigName("Etiqueta").Column], "");
                    phtValues.Add("{Etiqueta}", (liGpo == 0 ? "" : lsEtiqueta));
                    phtValues.Add("{BanderasEtiqueta}", 1); //Etiquetable

                    if (liRegTelDirectorio != int.MinValue)
                    {
                        lbActualizo = lCargasCOM.ActualizaRegistro("Historicos", "Directorio", "Directorio Telefonico", phtValues, liRegTelDirectorio, true, (int)Session["iCodUsuarioDB"]);
                    }
                    else if (liGpo == 0)
                    {
                        //Si el número fué grabado como "No Identificada", no se almacenará su directorio.
                        liRegTelDirectorio = 0;
                    }
                    else
                    {
                        liRegTelDirectorio = lCargasCOM.InsertaRegistro(phtValues, "Historicos", "Directorio", "Directorio Telefonico", true, (int)Session["iCodUsuarioDB"], false);
                    }

                    if (liRegTelDirectorio < 0 && !lbActualizo)
                    {
                        pbEtiquetacionCorrecta = false;
                        return;
                    }
                    else
                    {
                        //Almacena Directorios que se ReEtiquetarán en Llamadas:
                        lstRegsDir.Add(liRegTelDirectorio.ToString());
                    }
                }


            GrabaDetalle:

                DataTable ldtDataTable = HistoricFieldCollection.GetAtrib();
                int liCodIdioma = (int)ldtDataTable.Select("vchCodigo = '" + Globals.GetCurrentLanguage() + "'")[0]["iCodCatalogo"];

                try
                {
                    int liMaeDetEtiquetacion = (int)DSODataAccess.ExecuteScalar("Select iCodRegistro From [" + DSODataContext.Schema + "].Maestros Where vchDescripcion = 'Detalle Proceso de Etiquetacion' and iCodEntidad = " + iCodEntidadResumen + " and dtIniVigencia <> dtFinVigencia");

                    //Almacena la imagen del Proceso de Etiquetación en Detallados:
                    psbQuery.Length = 0;
                    psbQuery.AppendLine("Insert Into [" + DSODataContext.Schema + "].Detallados");
                    psbQuery.AppendLine("       (iCodUsuario,iCodCatalogo,iCodMaestro,{ProEtiqueta},{Locali},{TDest},{Tel},{Etiqueta},{Gpo},{CostoFac},");
                    psbQuery.AppendLine("        {Cantidad},{DuracionMin},{GEtiqueta},dtFecUltAct)");
                    psbQuery.AppendLine("Select iCodUsuario = " + Session["iCodUsuario"] + "," + "iCodCatalogo = " + iCodEntidadResumen + ",");
                    psbQuery.AppendLine("       iCodMaestro = " + liMaeDetEtiquetacion + ",");
                    psbQuery.AppendLine("       ProEtiqueta = " + liCatProEtiqueta + "," + pFieldsResumen.GetByConfigName("Locali").Column + ",");
                    psbQuery.AppendLine("       " + pFieldsResumen.GetByConfigName("TDest").Column + "," + pFieldsResumen.GetByConfigName("TelDest").Column + ",");
                    psbQuery.AppendLine("       Etiqueta = CASE WHEN GEtiqueta = 0 THEN '' ELSE " + pFieldsResumen.GetByConfigName("Etiqueta").Column + " END,");
                    psbQuery.AppendLine("       " + pFieldsResumen.GetByConfigName("Gpo").Column + "," + pFieldsResumen.GetByConfigName("CostoFac").Column + ",");
                    psbQuery.AppendLine("       " + pFieldsResumen.GetByConfigName("Cantidad").Column + "," + pFieldsResumen.GetByConfigName("DuracionMin").Column + ",");
                    psbQuery.AppendLine("       GEtiqueta,dtFecUltAct=GetDate()");
                    psbQuery.AppendLine("from [" + DSODataContext.Schema + "].GetResumen(" + iCodEmpleado + "," + liCodIdioma);
                    psbQuery.AppendLine("      ,'" + IniPeriodo.ToString("yyyy-MM-dd") + "','" + FinPeriodo.ToString("yyyy-MM-dd") + "')");
                    //psbQuery.AppendLine("Where EdodeReg > 0 --Numeros Etiquetables");                    
                    pKDB.ExecuteQuery("Detall", "Detalle Proceso de Etiquetacion", psbQuery.ToString());

                }
                catch (Exception ex)
                {
                    Util.LogException(ex);
                    pbEtiquetacionCorrecta = false;
                    return;
                }

                //RJ.20140411. SE OMITE LA INVOCACIÓN DEL MÉTODO ActualizaRegSesion
                //DEBIDO A QUE SE CREÓ LA TABLA REGSESION PARA SUSTITUIR HISTORICOS
                //Y ESTE CAMBIO IMPACTA LA FORMA EN QUE SE ACTUALIZA REGSESION
                //ADEMÁS SE CONSULTÓ CON OPERACIONES Y ESTE DATO NO ERA NECESARIO
                //if (pdtResumenConsultado != null && pdtResumenConsultado.Rows.Count > 0)
                //    ActualizaRegSesion();



                //Calcula Totales:
                psbQuery.Length = 0;
                psbQuery.Append("select Consumo = Sum(IsNull(G.{CostoFac},0)), Grupo = IsNull(G.{GEtiqueta},0) ");
                psbQuery.Append("from [" + DSODataContext.Schema + "].GetResumen(" + iCodEmpleado + "," + liCodIdioma + ",");
                psbQuery.Append("'" + IniPeriodo.ToString("yyyy-MM-dd") + "','" + FinPeriodo.ToString("yyyy-MM-dd") + "') As G Group By G.{GEtiqueta}");
                DataTable ldtConsumos = pKDB.ExecuteQuery("Detall", "Resumen Etiquetacion Temp", psbQuery.ToString());

                pNumTotNI.DataValue = 0;
                pNumTotPersonal.DataValue = 0;
                pNumTotLaboral.DataValue = 0;

                pNumTotProveedor.DataValue = 0; //RJ.20160616
                pNumTotOutsourcing.DataValue = 0;  //RJ.20160616

                for (int liRow = 0; liRow < ldtConsumos.Rows.Count; liRow++)
                {
                    switch (ldtConsumos.Rows[liRow]["Grupo"].ToString())
                    {
                        case "0":
                            pNumTotNI.DataValue = ldtConsumos.Rows[liRow]["Consumo"].ToString();
                            break;
                        case "1":
                            pNumTotPersonal.DataValue = ldtConsumos.Rows[liRow]["Consumo"].ToString();
                            break;
                        case "2":
                            pNumTotLaboral.DataValue = ldtConsumos.Rows[liRow]["Consumo"].ToString();
                            break;
                        case "3": //RJ.20160616
                            pNumTotProveedor.DataValue = ldtConsumos.Rows[liRow]["Consumo"].ToString();
                            break;
                        case "4": //RJ.20160616
                            pNumTotOutsourcing.DataValue = ldtConsumos.Rows[liRow]["Consumo"].ToString();
                            break;
                        default:
                            break;
                    }
                }
                pNumTotal.DataValue = double.Parse(pNumTotNI.DataValue.ToString()) + double.Parse(pNumTotPersonal.DataValue.ToString()) + double.Parse(pNumTotLaboral.DataValue.ToString()) + double.Parse(pNumTotProveedor.DataValue.ToString()) + double.Parse(pNumTotOutsourcing.DataValue.ToString()); //RJ.20160616
                //pNumTotal.DataValue = double.Parse(pNumTotNI.DataValue.ToString()) + double.Parse(pNumTotPersonal.DataValue.ToString()) + double.Parse(pNumTotLaboral.DataValue.ToString());


                //Obtiene el listado de registros que se requiere actualizar en DetalleCDR
                //posteriormente aplica la actualización, 
                //misma que se hace tomando como base el campo icodRegistro
                //El listado corresponde a las llamadas del periodo abierto para etiquetación
                pbEtiquetacionCorrecta = ActualizarEtiquetaEnDetalleCDR(iCodEmpleado, IniPeriodo, FinPeriodo);


            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                pbEtiquetacionCorrecta = false;
                return;
            }
        }


        /// <summary>
        /// Prepara y envía el correo de confirmación, adjuntando el archivo de word
        /// con el reporte de números etiquetados
        /// </summary>
        protected override void EnviarCorreo()
        {
            int liCodEmpleado = int.Parse(iCodEmpleado);
            string lsDateFormat = Globals.GetMsgWeb(false, "NetDateTimeFormat");

            DataTable ldt;


            ldt = GetExportGrid("WORD");



            //Obtiene la ruta y nombre del archivo que se utiliza como plantilla
            string lsWordPath = buscarPlantilla(false, true, ".docx");

            WordAccess loWord = new WordAccess();
            if (lsWordPath != "")
            {
                //Con Plantilla
                loWord.FilePath = lsWordPath;

                loWord.Abrir(true);  //Abre la plantilla

                //Agrega logo del cliente
                if (System.IO.File.Exists(psLogoClientePath))
                {
                    loWord.ReemplazarTextoPorImagen("{LogoCliente}", psLogoClientePath);
                }
                else
                {
                    loWord.ReemplazarTexto("{LogoCliente}", "");
                }

                //Agrega logo de Keytia
                if (System.IO.File.Exists(psLogoKeytiaPath))
                {
                    loWord.ReemplazarTextoPorImagen("{LogoKeytia}", psLogoKeytiaPath);
                }
                else
                {
                    loWord.ReemplazarTexto("{LogoKeytia}", "");
                }

                //Reemplaza los placeholders con los datos del reporte
                loWord.ReemplazarTexto("{TituloReporte}", Globals.GetMsgWeb(false, "TituloEtiqueta"));
                loWord.ReemplazarTexto("{HeaderEmple}", pFields.GetByConfigName("Emple").Descripcion);
                loWord.ReemplazarTexto("{Emple}", pFields.GetByConfigName("Emple").DSOControlDB.ToString());
                loWord.ReemplazarTexto("{HeaderCenCos}", pFields.GetByConfigName("CenCos").Descripcion);
                loWord.ReemplazarTexto("{CenCos}", pFields.GetByConfigName("CenCos").DSOControlDB.ToString());
                loWord.ReemplazarTexto("{HeaderIniPeriodo}", pFields.GetByConfigName("IniPer").Descripcion);
                loWord.ReemplazarTexto("{IniPeriodo}", IniPeriodo.Date.ToString(lsDateFormat).Substring(0, 10));
                loWord.ReemplazarTexto("{HeaderFinPeriodo}", pFields.GetByConfigName("FinPer").Descripcion);
                loWord.ReemplazarTexto("{FinPeriodo}", FinPeriodo.Date.ToString(lsDateFormat).Substring(0, 10));
                loWord.ReemplazarTexto("{HeaderNumTotPersonal}", pNumTotPersonal.Descripcion);
                loWord.ReemplazarTexto("{NumTotPersonal}", (pNumTotPersonal.DataValue.ToString() == "null" || pNumTotPersonal.DataValue.ToString() == "0" ? "$0.00" : "$" + pNumTotPersonal.DataValue.ToString()));
                loWord.ReemplazarTexto("{HeaderNumTotLaboral}", pNumTotLaboral.Descripcion);
                loWord.ReemplazarTexto("{NumTotLaboral}", (pNumTotLaboral.DataValue.ToString() == "null" || pNumTotLaboral.DataValue.ToString() == "0" ? "$0.00" : "$" + pNumTotLaboral.DataValue.ToString()));
                loWord.ReemplazarTexto("{HeaderNumTotNI}", pNumTotNI.Descripcion);
                loWord.ReemplazarTexto("{NumTotNI}", (pNumTotNI.DataValue.ToString() == "null" || pNumTotNI.DataValue.ToString() == "0" ? "$0.00" : "$" + pNumTotNI.DataValue.ToString()));
                loWord.ReemplazarTexto("{HeaderNumTotal}", pNumTotal.Descripcion);
                loWord.ReemplazarTexto("{NumTotal}", (pNumTotal.DataValue.ToString() == "null" || pNumTotal.DataValue.ToString() == "0" ? "$0.00" : "$" + pNumTotal.DataValue.ToString()));

                //RJ.20160616
                loWord.ReemplazarTexto("{HeaderNumTotProveedor}", pNumTotProveedor.Descripcion);
                loWord.ReemplazarTexto("{NumTotProveedor}", (pNumTotProveedor.DataValue.ToString() == "null" || pNumTotProveedor.DataValue.ToString() == "0" ? "$0.00" : "$" + pNumTotProveedor.DataValue.ToString()));
                loWord.ReemplazarTexto("{HeaderNumTotOutsourcing}", pNumTotOutsourcing.Descripcion);
                loWord.ReemplazarTexto("{NumTotOutsourcing}", (pNumTotOutsourcing.DataValue.ToString() == "null" || pNumTotOutsourcing.DataValue.ToString() == "0" ? "$0.00" : "$" + pNumTotOutsourcing.DataValue.ToString()));


                //Inserta el detalle obtenido en el método GetExportGrid
                loWord.ReemplazarTexto("{GridResumen}", "");


                try
                {
                    loWord.InsertarTabla(ldt, true, "KeytiaGrid");
                }
                catch
                {
                    loWord.InsertarTabla(ldt, true);
                }


            }
            else
            {
                //Sin Plantilla
                loWord.Abrir(true);
                if (System.IO.File.Exists(psLogoClientePath))
                {
                    loWord.InsertarImagen(psLogoClientePath);
                }
                if (System.IO.File.Exists(psLogoKeytiaPath))
                {
                    loWord.InsertarImagen(psLogoKeytiaPath);
                }
                loWord.InsertarTexto(Globals.GetMsgWeb(false, "TituloEtiqueta"));
                loWord.NuevoParrafo();
                loWord.InsertarTexto(pFields.GetByConfigName("Emple").Descripcion + ": " + pFields.GetByConfigName("Emple").DSOControlDB.ToString());
                loWord.NuevoParrafo();
                loWord.InsertarTexto(pFields.GetByConfigName("CenCos").Descripcion + ": " + pFields.GetByConfigName("CenCos").DSOControlDB.ToString());
                loWord.NuevoParrafo();
                loWord.InsertarTexto(pFields.GetByConfigName("IniPer").Descripcion + ": " + IniPeriodo.Date.ToString(lsDateFormat));
                loWord.NuevoParrafo();
                loWord.InsertarTexto(pFields.GetByConfigName("FinPer").Descripcion + ": " + FinPeriodo.Date.ToString(lsDateFormat));
                //Detalle
                loWord.NuevoParrafo();
                loWord.NuevoParrafo();
                loWord.InsertarTabla(ldt, true);
            }


            //Obtiene la ruta y nombre del archivo en donde se guardará 
            //la información etiquetada (.doc). El nombre del archivo corresponde al
            //vchcodigo del Empleado
            string lsFileName = GetFileNameCorreo(liCodEmpleado);
            loWord.FilePath = lsFileName;


            try
            {
                loWord.SalvarComo(); //Guarda la plantilla como el nuevo archivo
            }
            catch (Exception ex)
            {
                lsFileName = GetFileNameCorreo(liCodEmpleado, true);
                loWord.FilePath = lsFileName;
                loWord.SalvarComo();
            }
            finally
            {
                if (loWord != null)
                {
                    loWord.Cerrar();
                    loWord.Salir();
                    loWord.Dispose();
                    loWord = null;
                }
            }


            //Configura y envía correo de confirmación
            poMail = new MailAccess();
            poMail.NotificarSiHayError = false;
            poMail.IsHtml = true;
            poMail.OnSendCompleted = SendCompleted;
            poMail.De = GetRemitente();
            poMail.Asunto = Globals.GetMsgWeb(false, "TituloEtiqueta") + ": " + IniPeriodo.Date.ToString(lsDateFormat) + " - " + FinPeriodo.Date.ToString(lsDateFormat);
            poMail.Adjuntos.Add(new Attachment(lsFileName));
            poMail.AgregarWord(lsFileName);
            poMail.CC.Add(psMailCC);
            poMail.BCC.Add(psMailCCO);
            poMail.Para.Add(GetMailPara());
            poMail.Enviar();
        }


        protected override void InitLanguageEtiqueta()
        {
            piCodGrupo.DSOControlDB.Descripcion = Globals.GetMsgWeb(false, "GrupoEtiqueta");
            //NZ 201508112 Se incluye filtro por Tipo Destino
            piCodTDest.DSOControlDB.Descripcion = Globals.GetMsgWeb(false, "NomColDestinoEtq");
            //

            pNumTotPersonal.Descripcion = Globals.GetMsgWeb(false, "ConsumoPerEtiqueta");
            pNumTotLaboral.Descripcion = Globals.GetMsgWeb(false, "ConsumoLabEtiqueta");
            pNumTotNI.Descripcion = Globals.GetMsgWeb(false, "ConsumoNIEtiqueta");
            pNumTotal.Descripcion = Globals.GetMsgWeb(false, "ConsumoGenEtiqueta");
            pbPersonales.Descripcion = Globals.GetMsgWeb(false, "TodasPersonalesEtiqueta");
            pbLaborales.Descripcion = Globals.GetMsgWeb(false, "TodasLaboralesEtiqueta");

            //RJ.20160616
            pNumTotProveedor.Descripcion = Globals.GetMsgWeb(false, "ConsumoProveedorEtiqueta");
            pNumTotOutsourcing.Descripcion = Globals.GetMsgWeb(false, "ConsumoOutSourcingEtiqueta");


            pwndDetalle.Title = Globals.GetMsgWeb(false, "WdDetalleEtiqueta");
            pwndDetLinea.Title = Globals.GetMsgWeb(false, "WdDetLineaEtiqueta");

            pbtnDirPersonal.InnerText = Globals.GetMsgWeb("btnDirPersonal");
            pbtnDirCorporativo.InnerText = Globals.GetMsgWeb("btnDirCorporativo");
            pbtnVerDetalle.InnerText = Globals.GetMsgWeb("btnVerDetalle");

            piCodGrupo.InitDSOControlDBLanguage();
            DataTable ldtFiltroGEtiqueta = (DataTable)((DSODropDownList)piCodGrupo.DSOControlDB).DataSource;
            pdtGrupoCol = ldtFiltroGEtiqueta.Clone();
            foreach (DataRow lDataRow in ldtFiltroGEtiqueta.Select("value >= 0"))
            {
                pdtGrupoCol.ImportRow(lDataRow);
            }

            pExpResumen.Title = Globals.GetMsgWeb("EtiquetaDataTitle");
            pExpResumen.ToolTip = Globals.GetMsgWeb("EtiquetaDataTitle");
            pbPersonales.CheckBox.Enabled = true;
            pbLaborales.CheckBox.Enabled = true;

            if (State == HistoricState.Edicion && pbEnableEmpleado)
            {
                StringBuilder lsb = new StringBuilder();
                lsb.Length = 0;
                lsb.AppendLine("<script type='text/javascript'>");
                lsb.AppendLine("jQuery(function($){");
                lsb.AppendLine(pjsObj + ".iCodEmpleado = " + iCodEmpleado + ";");
                lsb.AppendLine(pjsObj + ".iCodEntidadResum = " + iCodEntidadResumen + ";");
                lsb.AppendLine(pjsObj + ".iCodMaestroResum = " + iCodMaestroResumen + ";");
                lsb.AppendLine(pjsObj + ".dtCorte = new Date(parseInt(" + DSOControl.SerializeJSON<DateTime>(pdtCorte) + ".substr(6)));");
                lsb.AppendLine(pjsObj + ".dtLimite = new Date(parseInt(" + DSOControl.SerializeJSON<DateTime>(pdtLimite) + ".substr(6)));");
                lsb.AppendLine(pjsObj + ".liDiaCorte = " + piDiaCorte + ";");
                lsb.AppendLine(pjsObj + ".liLongEtq = " + piLongEtiqueta + ";");
                lsb.AppendLine(pjsObj + ".liHisPrevEtq = " + piHisPreviaEtiqueta + ";");
                lsb.AppendLine(pjsObj + ".$gridData = $('#" + pResumenGrid.Grid.ClientID + "__hidden');");
                lsb.AppendLine(pjsObj + ".confirmGrabar = \"" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ConfirmarGrabarEtq")) + "\";");
                lsb.AppendLine(pjsObj + ".confirmTitleGrabar = \"" + DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "ConfirmarTitulo")) + "\";");
                lsb.AppendLine("});");
                lsb.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), pjsObj, lsb.ToString(), true, false);

                int liHabil = IsPeriodoValido(int.Parse(iCodEmpleado), IniPeriodo, FinPeriodo, pdtCorte, pdtLimite);
                lsb.Length = 0;
                lsb.AppendLine("<script type='text/javascript'>");
                lsb.AppendLine("jQuery(function($){");
                lsb.AppendLine(" var $chkPersonal =  $('#" + pbPersonales.CheckBox.ClientID + "');");
                lsb.AppendLine(" var $chkLaboral =  $('#" + pbLaborales.CheckBox.ClientID + "');");
                lsb.AppendLine(" var $SaveBtn =  $('#" + pbtnGrabar.ClientID + "');");
                lsb.AppendLine(" $chkPersonal[0].status = false;");
                if (liHabil == 1)
                {
                    lsb.AppendLine(" $SaveBtn.removeAttr(\"disabled\");");
                    lsb.AppendLine(" $chkPersonal.removeAttr(\"disabled\");");
                    lsb.AppendLine(" $chkLaboral.removeAttr(\"disabled\");");
                }
                else
                {
                    lsb.AppendLine(" $SaveBtn.attr(\"disabled\", true);");
                    lsb.AppendLine(" $chkPersonal.attr(\"disabled\", true);");
                    lsb.AppendLine(" $chkLaboral.attr(\"disabled\", true);");
                }
                lsb.AppendLine("});");
                lsb.AppendLine("</script>");
                DSOControl.LoadControlScriptBlock(Page, typeof(HistoricEdit), pjsObj + "NewValidate", lsb.ToString(), true, false);

            }


            if (pFieldsResumen != null)
            {
                InitLanguageGridResumen(pFieldsResumen, pResumenGrid);
            }

            pNumTotal.NumberBox.Text = pTxtNumTotServer.TextBox.Text;
            pNumTotPersonal.NumberBox.Text = pTxtNumTotPServer.TextBox.Text;
            pNumTotLaboral.NumberBox.Text = pTxtNumTotLServer.TextBox.Text;
            pNumTotNI.NumberBox.Text = pTxtNumTotNServer.TextBox.Text;

            //RJ.20160616
            pNumTotProveedor.NumberBox.Text = pTxtNumTotRServer.TextBox.Text;
            pNumTotOutsourcing.NumberBox.Text = pTxtNumTotOServer.TextBox.Text;

            IniDetalleTemporal();

            //NZ 201508112 Se incluye filtro por Tipo Destino
            LlenarComboTDest();
        }

        #endregion


        #region Eventos

        protected override void EtiquetaEdit_Init(object sender, EventArgs e)
        {
            DataTable lKDBTable = pKDB.GetHisRegByEnt("Aplic", "Aplicaciones del Sistema", "iCodCatalogo = " + iCodAplicacion);

            SetMaestros();
            SetEmpleado();

            if (pbEnableEmpleado)
            {
                GetClientConfig();
                GetPeriodoEtiquetaIni();
            }
            else
            {
                string lsTitulo = Globals.GetMsgWeb(false, "TituloEtiqueta");
                string lsError = DSOControl.JScriptEncode(Globals.GetMsgWeb(false, "UsuarioSinEmpleado"));
                DSOControl.jAlert(Page, pjsObj, lsError, lsTitulo);
            }

            if ((int)Util.IsDBNull(lKDBTable.Rows[0]["{ParamInteger1}"], 0) != 0 || !pbEnableEmpleado)
            {
                pbEtiquetacionColaborador = false;
                iCodEntidad = iCodEntidadEtq;
                iCodMaestro = iCodMaestroEtq;
            }

            //Valida Permiso Directorio Corporativo            
            string liCodPerfilConfig = pKDB.GetHisRegByCod("Perfil", new string[] { "Config" }, new string[] { "iCodCatalogo" }).Rows[0]["iCodCatalogo"].ToString();
            if (Session["iCodPerfil"].ToString() == liCodPerfilConfig)
            {
                pbDirCorporativo = true;
            }
            else
            {
                //Valida permisio directorio corporativo aplicable a perfil Administrador, RZ.20120607
                string liCodPerfilAdmin = pKDB.GetHisRegByCod("Perfil", new string[] { "Admin" }, new string[] { "iCodCatalogo" }).Rows[0]["iCodCatalogo"].ToString();

                if (Session["iCodPerfil"].ToString() == liCodPerfilAdmin)
                {
                    pbDirCorporativo = true;
                }
            }

            pFieldsNoVisibles = new string[] { "GEtiqueta", "TelDest" };

            // Controles para la seccion de Resumen                                    
            pbtnDirPersonal = new HtmlButton();
            pbtnDirCorporativo = new HtmlButton();
            pbtnVerDetalle = new HtmlButton();
            //plBlanco = new Label();
            pExpResumen = new DSOExpandable();
            pTablaResumen = new Table();
            pResumenGrid = new DSOGrid();
            piCodGrupo = new KeytiaDropDownOptionField();

            //NZ 201508112 Se incluye filtro por Tipo Destino  
            piCodTDest = new KeytiaDropDownOptionField();
            //

            pbPersonales = new DSOCheckBox();
            pbLaborales = new DSOCheckBox();
            pNumTotLaboral = new DSONumberEdit();
            pNumTotNI = new DSONumberEdit();
            pNumTotPersonal = new DSONumberEdit();
            pNumTotal = new DSONumberEdit();

            //RJ.20160616
            pNumTotProveedor = new DSONumberEdit();
            pNumTotOutsourcing = new DSONumberEdit();

            pTxtNumTotPServer = new DSOTextBox();
            pTxtNumTotLServer = new DSOTextBox();
            pTxtNumTotNServer = new DSOTextBox();
            pTxtNumTotServer = new DSOTextBox();

            //RJ.20160616
            pTxtNumTotRServer = new DSOTextBox();
            pTxtNumTotOServer = new DSOTextBox();

            pwndDetalle = new DSOWindow();
            pwndDetLinea = new DSOWindow();
            pTxtWindowVisible = new DSOTextBox();


            this.CssClass = "EtiquetaEdit";

            pToolBar.Controls.Add(pbtnVerDetalle);
            pToolBar.Controls.Add(pbtnDirCorporativo);
            pToolBar.Controls.Add(pbtnDirPersonal);

            Controls.Add(pExpResumen);
            Controls.Add(pResumenGrid);
            Controls.Add(pwndDetalle);
            Controls.Add(pwndDetLinea);
            Controls.Add(pTxtWindowVisible);
            Controls.Add(pTxtNumTotPServer);
            Controls.Add(pTxtNumTotLServer);
            Controls.Add(pTxtNumTotNServer);

            //RJ.20160616
            Controls.Add(pTxtNumTotRServer);
            Controls.Add(pTxtNumTotOServer);


            Controls.Add(pTxtNumTotServer);


        }

        #endregion

    }
}

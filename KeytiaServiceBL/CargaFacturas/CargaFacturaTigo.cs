/*
Nombre:		    PMTL
Fecha:		    20140521
Descripción:	Clase con la lógica for cargar las facturas de Tigo.
Modificación:	
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeytiaServiceBL.CargaFacturas
{
    public class CargaFacturaTigo : CargaServicioFactura
    {
        private int piArchivo;
        private int piCatClaveCargoConst;
        private string psValor;
        private string psTipoDato;
        private string psAtributo;
        private object poValor;
        private Hashtable phtMaestrosEnvio = new Hashtable();
        private System.Data.DataTable pdtEncabezados = new System.Data.DataTable();
        private System.Data.DataTable pdtMaestros = new System.Data.DataTable();
        private DateTime fechaFactura;



        
        public CargaFacturaTigo()
        {
            pfrXLS = new FileReaderXLS();
            psSPDetalleFacCDR = "GeneraDetalleFacturaCDRTigo";
            /*RZ.20140422*/
            psSPResumenFacturasDeMoviles = "GeneraResumenFacturasDeMovilesTigo";
            
        }


        protected override bool ValidarInitCarga()
        {
            if (pdrConf == null)
            {
                Util.LogMessage("Error en Carga. Carga no Identificada.");
                return false;
            }
            if (piCatServCarga == int.MinValue)
            {
                ActualizarEstCarga("CarNoSrv", psDescMaeCarga);
                return false;
            }
            if (kdb.FechaVigencia == DateTime.MinValue)
            {
                ActualizarEstCarga("CarNoFec", psDescMaeCarga);
                return false;
            }
            return true;
        }

        protected override bool ValidarIdentificadorSitio()
        {
            if (pdrLinea["{Sitio}"] == System.DBNull.Value)
            {
                psMensajePendiente.Append("[" + psEntRecurso + " sin Sitio Asignado.]");
                return false;
            }
            return true;
        }

        public override void IniciarCarga()
        {
            int liCantArchivos = 0;
            //string[] lsArchivos = new string[] { "", "", "", "", "" };
            string[] lsArchivos = new string[] { "" }; //PT: por ahora solo es 1 archivo

            ConstruirCarga("Tigo", "Cargas Factura Tigo", "Carrier", "Linea");
            fechaFactura = Convert.ToDateTime(pdrConf["{FechaFactura}"].ToString());
            if (!ValidarInitCarga())
            {
                return;
            }

            //Identifica los archivos que se cargarán en sistema            
            for (int liArchivo = 1; liArchivo <= 1; liArchivo++) //PT: en este caso solo sera un archivo
            {
                if (pdrConf["{Archivo0" + liArchivo.ToString() + "}"] != null && pdrConf["{Archivo0" + liArchivo.ToString() + "}"].ToString().Trim().Length > 0)
                {
                    lsArchivos[liArchivo - 1] = (string)pdrConf["{Archivo0" + liArchivo.ToString() + "}"];
                }
            }

            if (pdtMaestros == null || pdtMaestros.Rows.Count == 0)
            {
                //No se encontraron maestros para Nextel
                ActualizarEstCarga("CarNoMae", psDescMaeCarga);
                return;
            }

            System.Data.DataRow pdrClaveCargo = GetClaveCargo("TigoCargo");
            if (pdrClaveCargo == null || pdrClaveCargo.ItemArray.Length == 0)
            {
                ActualizarEstCarga("CarNoTpSrv", psDescMaeCarga);
                return;
            }
            piCatClaveCargoConst = (int)pdrClaveCargo["iCodCatalogo"];

            //Valida cada uno de los archivos que se cargaron en los campos Upload de la carga Web
            for (int liArchivo = 1; liArchivo <= lsArchivos.Length; liArchivo++)
            {
                if (lsArchivos[liArchivo - 1].Length == 0)
                {
                    //No se seleccionó archivo para el campo Upload por validar
                    continue;
                }
                liCantArchivos++;
                if (!pfrXLS.Abrir(lsArchivos[liArchivo - 1]))
                {
                    ActualizarEstCarga("ArchNoVal" + liArchivo.ToString(), psDescMaeCarga);
                    return;
                }

                piArchivo = liArchivo;
                if (!ValidarArchivo())
                {
                    //Al validar cada uno de los archivos, se va llenando la tabla pdtEncabezados
                    pfrXLS.Cerrar();
                    ActualizarEstCarga(psMensajePendiente.ToString(), psDescMaeCarga);
                    return;
                }
                pfrXLS.Cerrar();
            }
            pdtMaestros.Clear(); //Ya no se utlizará

            if (liCantArchivos == 0)
            {
                ActualizarEstCarga("CarNoArchs", psDescMaeCarga);
                return;
            }

            int piRegistroTotal = 0;

            for (int liArchivo = 1; liArchivo <= 1; liArchivo++)//cantidad de archivos disponibles a cargar 
            {
                piArchivo = liArchivo;
                if (pdrConf["{Archivo0" + liArchivo.ToString() + "}"] == null || pdrConf["{Archivo0" + liArchivo.ToString() + "}"].ToString().Trim().Length == 0)
                {
                    //No se seleccionó archivo for el campo Upload por cargar
                    continue;
                }
                pfrXLS.Abrir(pdrConf["{Archivo0" + liArchivo.ToString() + "}"].ToString().Trim());
                piRegistro = 0;
                while (piRegistro < 1)
                {
                    //1 Registros de Encabezados
                    pfrXLS.SiguienteRegistro();
                    piRegistro++;
                }
                piRegistro = 0;
                while ((psaRegistro = pfrXLS.SiguienteRegistro()) != null)
                {
                    piRegistro++;
                    ProcesarRegistro();
                }
                pfrXLS.Cerrar();
                piRegistroTotal += piRegistro;
            }
            piRegistro = piRegistroTotal;
            ActualizarEstCarga("CarFinal", psDescMaeCarga);
        }

        protected override bool ValidarArchivo()
        {
            int liColIdentificador;
            int liRegsIni = 1;
            psMensajePendiente.Length = 0;


            if (liRegsIni < 1 || (psaRegistro = pfrXLS.SiguienteRegistro()) == null)
            {
                psMensajePendiente.Length = 0;
                psMensajePendiente.Append("ArchNoVal" + piArchivo.ToString());
                return false;
            }

            for (int liEnc = 0; liEnc < psaRegistro.Length; liEnc++)
            {
                //guarda los nombres de las columnas en pdtEncabezados
                //pdtEncabezados: ["NumColumna","NomColumna","NumArchivo","Atributo","Maestro","TipoDato"]
                pdtEncabezados.Rows.Add(new object[] { liEnc, psaRegistro[liEnc].Trim(), piArchivo, "", "", "" });
            }

            if (pdtEncabezados == null || pdtEncabezados.Rows.Count == 0)
            {
                psMensajePendiente.Length = 0;
                psMensajePendiente.Append("ArchNoVal" + piArchivo.ToString());
                return false;
            }

            if ((psaRegistro = pfrXLS.SiguienteRegistro()) == null)
            {
                psMensajePendiente.Length = 0;
                psMensajePendiente.Append("ArchNoDet" + piArchivo.ToString());
                return false;
            }

            do
            {
                pdrArray = pdtEncabezados.Select("NumArchivo=" + piArchivo.ToString()+ " and NomColumna='Telefono:'");
                if (pdrArray == null || pdrArray.Length == 0)
                {
                    //No se encontó la columna Telefono la cual es obligatoria
                    psMensajePendiente.Length = 0;
                    psMensajePendiente.Append("ArchNoVal" + piArchivo.ToString());
                    return false;
                }
                liColIdentificador = (int)Util.IsDBNull(pdrArray[0]["NumColumna"], int.MinValue);
                if (liColIdentificador != int.MinValue)
                {
                     psIdentificador = psaRegistro[liColIdentificador].Trim();
                }
                else
                {
                    psMensajePendiente.Length = 0;
                    psMensajePendiente.Append("ArchNoVal" + piArchivo.ToString());
                    return false;
                }

                pdrLinea = GetLinea(psIdentificador);
                if (pdrLinea != null && pdrLinea["{Sitio}"] != System.DBNull.Value)
                {
                    break;
                }
            }
            while ((psaRegistro = pfrXLS.SiguienteRegistro()) != null);

            if (pdrLinea == null && !pbSinLineaEnDetalle)
            {
                //No se permite almacenar en Detallados registros con lineas que no aparecen en sistema.
                psMensajePendiente.Length = 0;
                psMensajePendiente.Append("ArchNoVal" + piArchivo.ToString());
                return false;
            }
            else if (pdrLinea != null && !ValidarIdentificadorSitio())
            {
                psMensajePendiente.Length = 0;
                psMensajePendiente.Append("CarSitNoVal" + piArchivo.ToString());
                return false;
            }

            liRegsIni = 0;
            //Valida que haya una atributo por NomColumna e identifica en que maestro se encuentra
            for (int liCount = 0; liCount < pdtEncabezados.Rows.Count; liCount++)
            {
                if ((int)pdtEncabezados.Rows[liCount]["NumArchivo"] != piArchivo)
                {
                    continue;
                }
                psAtributo = "{" + pdtEncabezados.Rows[liCount]["NomColumna"].ToString().Replace(" ", "").Replace(":","").Trim() + "}";
                switch (psAtributo.Replace("{", "").Replace("}", ""))
                {
                    
                    /*case "Telefono":
                        {
                            psAtributo = "{Tel}";
                            break;
                        }
                    */
                    case "Telefono":
                        {
                            psAtributo = "{Ident}";
                            break;
                        }
                    case "Tot.Monto":
                        { 
                            psAtributo = "{TotMonto}";
                            break;
                        }

                    case "Periodo":
                        {
                            psAtributo = "{PeriodoFact}";
                            break;
                        }
                    default:
                        {
                            psAtributo = psAtributo.Replace("Consumo", "C").Replace("Monto", "M").Replace(".", "").Replace("á","a");
                            psAtributo = psAtributo.Replace("-","").Replace("(","").Replace(")","");
                            

                            /*//Si el NomColumna tiene una longitud mayor a 9, se tomarán los primeros 5 caracteres + los últimos 4
                            if (psAtributo.Length > 11)
                            {
                                psAtributo = psAtributo.Substring(0, 6) + psAtributo.Substring(psAtributo.Length - 5, 5);
                            }*/
                            break;
                        }
                }

                pdrArray = pdtMaestros.Select("Atributo='" + psAtributo + "'");
                if (pdrArray == null || pdrArray.Length == 0)
                {
                    pdtEncabezados.Rows[liCount]["Atributo"] = psAtributo;
                    pdtEncabezados.Rows[liCount]["Maestro"] = "";
                    pdtEncabezados.Rows[liCount]["TipoDato"] = "";
                    continue;
                }
                else if ((pdrArray.Length == 2 || pdrArray.Length == 3) && pdrArray[0]["TipoDato"].ToString() == "System.Int32")
                {
                    //Existen columnas con el mismo nombre pero su tipo de dato es distinto. Si es Int se agrega un "." al nombre del atributo.
                    pdtEncabezados.Rows[liCount]["Atributo"] = psAtributo.Replace("}", ".}");
                }
                else if (pdrArray.Length == 3 && pdrArray[0]["TipoDato"].ToString() == "System.Double" && liRegsIni > 0)
                {
                    //Existen 3 columnas con el mismo nombre, dos float y un int. El int se captura en el if anterior, al segundo double se le antepone "IMP"
                    liRegsIni++;
                    pdtEncabezados.Rows[liCount]["Atributo"] = psAtributo.Replace("{", "{IMP");
                }
                else
                {
                    pdtEncabezados.Rows[liCount]["Atributo"] = psAtributo;
                }
                pdtEncabezados.Rows[liCount]["Maestro"] = pdrArray[0]["Maestro"].ToString();
                pdtEncabezados.Rows[liCount]["TipoDato"] = pdrArray[0]["TipoDato"].ToString();
            }
            return true;
        }

        protected override void ProcesarRegistro()
        {
            pbPendiente = false;
            psMensajePendiente.Length = 0;
            InitValores();
            Hashtable lhtAuxiliar = new Hashtable();
            string lsMaeDetalle = "";
            DateTime ldtFechaCorte = new DateTime(1900, 1, 1);
            //string lsCuenta = "";
            string lsIdent = "";

            for (int liColReg = 0; liColReg < psaRegistro.Length; liColReg++)
            {
                psValor = psaRegistro[liColReg].Trim();
                
                pdrArray = pdtEncabezados.Select("NumArchivo=" + piArchivo.ToString() + " and NumColumna=" + liColReg.ToString());
                psTipoDato = pdrArray[0]["TipoDato"].ToString();
                psAtributo = pdrArray[0]["Atributo"].ToString();
                lsMaeDetalle = pdrArray[0]["Maestro"].ToString();

                //20140523 si el valor es minutos el excel lo combierte a decimal y es necesario multiplicarlo por 24 para que nos de los minutos adecuados

                if (psAtributo.StartsWith("{C") && psAtributo.Contains("Min"))
                {
                    double val = 0;

                    if (Double.TryParse(psValor, out val)) {
                        val *= 24;
                    }
                    int valInt = (int)val;
                    psValor = valInt.ToString();
                    psTipoDato = "System.Int32";
                }

                if (lsMaeDetalle == "")
                {
                    //Atributo sin maestro no almacena el valor
                    continue;
                }
                if (!phtMaestrosEnvio.Contains(lsMaeDetalle))
                {
                    Hashtable phtMae = new Hashtable();
                    phtMaestrosEnvio.Add(lsMaeDetalle, phtMae);
                }

                lhtAuxiliar = (Hashtable)phtMaestrosEnvio[lsMaeDetalle];
                if (ValidarRegistro())
                {
                    //RZ.20140228 Se retira la condicion para que aunque el valor sea cero en el campo, lo agregue al hash
                    //if ((poValor is int && (int)poValor == 0) || (poValor is double && (double)poValor == 0))
                    //{
                    //    //No almacena el valor
                    //    continue;
                    //}
                    lhtAuxiliar.Add(psAtributo, poValor);
                    if (psAtributo == "{Ident}")
                    {
                        lhtAuxiliar.Add("{Tel}", poValor);
                        lhtAuxiliar.Add("{Linea}", piCatIdentificador);
                        lsIdent = (string)poValor;
                    }


                }
                else
                {
                    pbPendiente = true;
                }
            }

            //Hashtable lhtDetA = new Hashtable();
            foreach (DictionaryEntry ldeTablaEnvio in phtMaestrosEnvio)
            {
                lsMaeDetalle = ldeTablaEnvio.Key.ToString().Substring(0, 15); //DetalleFactura[A-Z]
              //  lhtDetA = (Hashtable)phtMaestrosEnvio["DetalleFacturaATigoDet"];
                //psTpRegFac = ldeTablaEnvio.Key.ToString().Substring(21); //Enc-Det
                psTpRegFac = "Det";

                if (!SetCatTpRegFac(psTpRegFac))
                {
                    pbPendiente = true;
                }

                phtTablaEnvio.Clear();
                phtTablaEnvio = (Hashtable)ldeTablaEnvio.Value;
                phtTablaEnvio.Add("{IdArchivo}", piArchivo);

                if (lsMaeDetalle == "DetalleFacturaA")
                {
                    /* RZ.20120928 Inlcuir fecha de publicación para la factura */
                    phtTablaEnvio.Add("{FechaPub}", pdtFechaPublicacion);
                    phtTablaEnvio.Add("{ClaveCar}", piCatClaveCargoConst);
                    //PT.20131206 Se agrega la fecha de factura
                    phtTablaEnvio.Add("{FechaFactura}", fechaFactura);
                    //RZ.20140221 Multiplicar por el tipo de cambio encontrado en base a la moneda de la carga

                    //RZ.20140221 Agregar el tipo de cambio al hash de detalle
                    phtTablaEnvio.Add("{TipoCambioVal}", pdTipoCambioVal);
                }
                

                //ValidarCargaUnica                      
                System.Data.DataTable ldtHisCargas = null;
                ldtHisCargas = kdb.ExecuteQuery("Detall", "DetalleFacturaATigoDet", "Select iCodRegistro From Detallados Where " +//"{FechaCorte} = '" + ldtFechaCorte.ToString("yyyy-MM-dd HH:mm:ss") "' and " +
                          "{Ident} = '" + lsIdent + "' and " +
                          "{FechaPub} = '" + pdtFechaPublicacion.ToString("yyyy-MM-dd") + "' and " +
                          "(iCodCatalogo <> " + CodCarga.ToString() + " or (iCodCatalogo = " + CodCarga.ToString() + " and {IdArchivo} <> " + piArchivo.ToString() + "))");
                if (ldtHisCargas != null && ldtHisCargas.Rows.Count > 0)
                {
                    if (!psMensajePendiente.ToString().Contains("Registro procesado"))
                    {
                        psMensajePendiente.Append("[Registro procesado en un archivo distinto]");
                    }
                    pbPendiente = true;
                }
                if (phtTablaEnvio.Values.Count > 0)
                {
                    InsertarRegistroDet(lsMaeDetalle, psTpRegFac, KDBAccess.ArrayToList(psaRegistro));
                }
            }
        }

        protected override void InitValores()
        {
            base.InitValores();
            poValor = null;
            psValor = "";
            psTipoDato = "";
            psAtributo = "";
            phtMaestrosEnvio.Clear();
        }

        protected override bool ValidarRegistro()
        {
            bool lbRegValido = true;

            switch (psTipoDato)
            {
                case "System.Int32":
                    {
                        int liAux = 0;
                        if (psValor.Length > 0 && !int.TryParse(psValor, out liAux))
                        {
                            psMensajePendiente.Append("[Formato Incorrecto. " + psAtributo + ".]");
                            lbRegValido = false;
                        }
                        poValor = liAux;
                        break;
                    }
                case "System.DateTime":
                    {
                        DateTime ldtAux;
                        ldtAux = Util.IsDate(psValor, "dd/MM/yyyy HH:mm:ss");
                        if (psValor.Length > 0 && ldtAux == DateTime.MinValue)
                        {
                            psMensajePendiente.Append("[Formato Incorrecto. " + psAtributo + ".]");
                            lbRegValido = false;
                        }
                        poValor = ldtAux;
                        break;
                    }
                case "System.Double":
                    {
                        double ldAux = 0;

                        if (psValor.Length > 0 && !double.TryParse(psValor.Replace(',', '.'), out ldAux))
                        {
                            psMensajePendiente.Append("[Formato Incorrecto. " + psAtributo + ".]");
                            lbRegValido = false;
                        }
                        //20140521 PT: se asume que todos los valores double son importes y se multiplican por el tipo de cambio
                        poValor = ldAux * pdTipoCambioVal;
                        //poValor = ldAux;
                        break;
                    }
                default:
                    {
                        poValor = psValor;
                        break;
                    }
            }

            if (psAtributo != "{Ident}")
            {

                return lbRegValido;
            }
            if (psValor == "Total") //20140505.PT: si es la linea de totales lo toma como registro no valido
            {
                psMensajePendiente.Append("[Linea de Totales]");
                lbRegValido = false;
                return lbRegValido;
            }

                psIdentificador = psValor;
                pdrLinea = GetLinea(psIdentificador);
             if (pdrLinea != null)
            {
                if (pdrLinea["iCodCatalogo"] != System.DBNull.Value)
                {
                    piCatIdentificador = (int)pdrLinea["iCodCatalogo"];
                }
                else
                {
                    psMensajePendiente.Append("[Error al asignar Línea]");
                    lbRegValido = false;
                }
                if (lbRegValido && !ValidarIdentificadorSitio())
                {
                    lbRegValido = false;
                }
                if (lbRegValido && !ValidarLineaExcepcion(piCatIdentificador))
                {
                    lbRegValido = false;
                }

                /*RZ.20130815 Validar si la linea es publicable*/
                if (!ValidarLineaNoPublicable())
                {
                    lbRegValido = false;
                }
            }
            else if (!pbSinLineaEnDetalle)
            {
                psMensajePendiente.Append("[La Línea no se encuentra en el sistema]");
                lbRegValido = false;
                InsertarLinea(psIdentificador);
            }

            return lbRegValido;
        }

        protected override void LlenarBDLocal()
        {
            base.LlenarBDLocal();
            System.Data.DataTable ldtAuxMae = new System.Data.DataTable();
            System.Data.DataTable ldtAuxAtrib = new System.Data.DataTable();
            string lsMaestro;

            pdtMaestros.Columns.Add("Atributo");
            pdtMaestros.Columns.Add("Maestro");
            pdtMaestros.Columns.Add("TipoDato");

            ldtAuxMae = DSODataAccess.Execute("Select vchDescripcion from Maestros where vchDescripcion like 'DetalleFactura%Tigo%' and dtIniVigencia <> dtFinVigencia order by vchDescripcion");

            for (int liMae = 0; liMae < ldtAuxMae.Rows.Count; liMae++)
            {
                lsMaestro = ldtAuxMae.Rows[liMae]["vchDescripcion"].ToString();
                ldtAuxAtrib = kdb.GetHisRegByEnt("Detall", lsMaestro, "iCodRegistro is null");
                for (int liAtt = 0; liAtt < ldtAuxAtrib.Columns.Count; liAtt++)
                {
                    psAtributo = ldtAuxAtrib.Columns[liAtt].ColumnName.Replace(".", "");
                    if (!psAtributo.StartsWith("{"))
                    {
                        continue;
                    }
                    psTipoDato = ldtAuxAtrib.Columns[liAtt].DataType.ToString();
                    pdtMaestros.Rows.Add(new object[] { psAtributo, lsMaestro, psTipoDato });
                }
            }

            pdtEncabezados.Columns.Add("NumColumna");
            pdtEncabezados.Columns.Add("NomColumna");
            pdtEncabezados.Columns.Add("NumArchivo");
            pdtEncabezados.Columns.Add("Atributo");
            pdtEncabezados.Columns.Add("Maestro");
            pdtEncabezados.Columns.Add("TipoDato");

            pdtEncabezados.Columns["NumColumna"].DataType = System.Type.GetType("System.Int32");
            pdtEncabezados.Columns["NumArchivo"].DataType = System.Type.GetType("System.Int32");

            ldtAuxMae.Clear();
            ldtAuxAtrib.Clear();
        }

    }
}

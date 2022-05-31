﻿/*
Nombre:		    PGS
Fecha:		    20110302
Descripción:	Clase con la lógica para cargar las facturas de Telmex.
Modificación:	PGS-2011/04/06
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace KeytiaServiceBL.CargaFacturas
{
    public class CargaFacturaTelmex : CargaServicioFactura
    {
        private int piFormato;

        private string psPoblacionDestino;
        private string psTelDestino;
        private string psPlanTarifario;
        private string psFolioLD;
        private string psModalidad;
        private string psMarcaLargaDuracion;
        private double pdImporte;
        private double pdDuracion;
        private int piNoTarjetaTelCard;
        private int piIdPoblacionDestino;
        private DateTime pdtFechaFacturacion;
        private DateTime pdtFechaInicio;
        private DateTime pdtHoraInicio;
        private string psTraficoVPN;
        private string psClavesVarias;
        private string psNo800;
        private string psCilliCode;
        private string psCarrier;
        private string psMarcaCodigoLada;
        private string psTarjetaVPNet;

        private string psUnidad;
        private int piCantidad;
        private DateTime pdtFechaFin;

        private string psCuenta;
        private string psConceptoFS;
        private string psLadaTelefono;
        private string psClaveNombre;
        private string psNombre;
        private string psReferenciaSisa;
        private string psCalle;
        private double pdPorcentajeIVA;

        private string psTroncal;
        private string psDescripcion;
        private string psEstadoDeRegistro;
        private string psSector;
        private string psDivision;
        private DateTime pdtFechaCargo;

        private string psTipoLadaEnlace;
        private string psPuntaA;
        private string psPoblacionA;
        private string psPuntaB;
        private string psPoblacionB;
        private string psAsignacion;
        private string psActaRecepcion;
        private double pdKM;

        public CargaFacturaTelmex()
        {
            pfrTXT = new FileReaderTXT();
            psSPDetalleFacCDR = "GeneraDetalleFacturaCDRTelmex";
            /*RZ.20140422*/
            psSPResumenFacturasDeMoviles = "GeneraResumenFacturasDeMovilesTelmex";
        }

        public override void IniciarCarga()
        {
            ConstruirCarga("Telmex", "Cargas Factura Telmex", "Carrier", "Linea");

            /*RZ.20140605 La actualizacion de importes en SM solo será si se activa la bandera*/
            if ((((int)Util.IsDBNull(pdrConf["{BanderasCarga" + psServicioCarga + "}"], 0) & 0x02) / 0x02) == 1)
            {
                pbActualizaTelmexSM = true;
            }

            if (!ValidarInitCarga())
            {
                return;
            }
            //if (!pfrTXT.Abrir(Archivo1))
            if (pdrConf["{Archivo01}"] == System.DBNull.Value || !pfrTXT.Abrir(pdrConf["{Archivo01}"].ToString()))
            {
                ActualizarEstCarga("ArchNoVal1", psDescMaeCarga);
                return;
            }
            if (!ValidarArchivo())
            {
                pfrTXT.Cerrar(); //Archivo Abierto en if previo al actual
                ActualizarEstCarga(psMensajePendiente.ToString(), psDescMaeCarga);
                return;
            }
            pfrTXT.Cerrar();

            piRegistro = 0;
            pfrTXT.Abrir(pdrConf["{Archivo01}"].ToString());
            while ((psaRegistro = pfrTXT.SiguienteRegistro()) != null)
            {
                piRegistro++;
                psRegistro = psaRegistro[0];
                ProcesarRegistro();
            }
            pfrTXT.Cerrar();
            ActualizarEstCarga("CarFinal", psDescMaeCarga);
        }

        protected override void ProcesarRegistro()
        {
            pbPendiente = false;
            psMensajePendiente.Length = 0;
            InitValores();
            int.TryParse(psRegistro.Substring(17, 1), out piFormato);

            switch (piFormato)
            {
                case 1:
                    {
                        //Información de Larga Distancia
                        psTpRegFac = "LD";
                        DetLargaDistancia();
                        break;
                    }
                case 2:
                    {
                        //Información de Servicio Medido
                        psTpRegFac = "SM";
                        DetServicioMedido();
                        break;
                    }
                case 3:
                    {
                        //Información de Renta y Otros Cargos
                        psClaveNombre = psRegistro.Substring(24, 1);
                        psTpRegFac = "RyOC";
                        if (psClaveNombre == "1")
                        {
                            DetRentasYOtrosCargosRFC();
                        }
                        else
                        {
                            DetRentasYOtrosCargosImportes();
                        }
                        break;
                    }
                case 4:
                    {
                        //Información de Lada Enlaces
                        psTpRegFac = "Enlace";
                        DetEnlaces();
                        break;
                    }
                default:
                    {
                        phtTablaEnvio.Clear();
                        InsertarRegistroDet("DetalleFacturaA", "LD", psRegistro);
                        InsertarRegistroDet("DetalleFacturaB", "LD", psRegistro);
                        break;
                    }
            }
        }

        protected override bool ValidarArchivo()
        {
            psMensajePendiente.Length = 0;
            InitValores();

            //Valida que haya registros
            psaRegistro = pfrTXT.SiguienteRegistro();
            if (psaRegistro == null)
            {
                psMensajePendiente.Length = 0;
                psMensajePendiente.Append("ArchNoDet1");
                return false;
            }

            do
            {
                psRegistro = psaRegistro[0];
                piFormato = 0;
                if (psRegistro.Length >= 18)
                {
                    int.TryParse(psRegistro.Substring(17, 1), out piFormato);
                }
                switch (piFormato)
                {
                    case 1:
                        {
                            psTpRegFac = "LD";
                            break;
                        }
                    case 2:
                        {
                            psTpRegFac = "SM";
                            break;
                        }
                    case 3:
                        {
                            psTpRegFac = "RyOC";
                            break;
                        }
                    case 4:
                        {
                            psTpRegFac = "Enlace";
                            break;
                        }
                    default:
                        {
                            continue;
                        }
                }

                //Busca el primer registro con Identificador que esté dado de alta en BD
                psCuentaMaestra = psRegistro.Substring(0, 6).Trim();
                if (psRegistro.Substring(17, 1) == "4")
                {
                    psIdentificador = psCuentaMaestra;
                }
                else if (psRegistro.Substring(17, 1) == "1" || psRegistro.Substring(17, 1) == "2" || psRegistro.Substring(17, 1) == "3")
                {
                    psIdentificador = psRegistro.Substring(7, 10).Trim();
                }

                pdrLinea = GetLinea(psIdentificador);
                if (pdrLinea != null && pdrLinea["{Sitio}"] != System.DBNull.Value)
                {
                    break;
                }
            }
            while ((psaRegistro = pfrTXT.SiguienteRegistro()) != null);

            if (!ValidarCargaUnica(psDescMaeCarga, psCuentaMaestra, psTpRegFac))
            {
                psMensajePendiente.Append("1");
                return false;
            }

            if (pdrLinea == null && !pbSinLineaEnDetalle)
            {
                //No se encontraron líneas almacenadas previamente en sistema.
                psMensajePendiente.Length = 0;
                psMensajePendiente.Append("ArchNoVal1");
                return false;
            }
            else if (pdrLinea != null && !ValidarIdentificadorSitio())
            {
                psMensajePendiente.Length = 0;
                psMensajePendiente.Append("CarSitNoVal1");
                return false;
            }

            return true;
        }

        private void DetLargaDistancia()
        {
            //Tipo Registro  = Telmex Larga Distancia   
            if (!SetCatTpRegFac(psTpRegFac))
            {
                pbPendiente = true;
                psMensajePendiente.Length = 0;
                psMensajePendiente.Append("[Tipo Registro de Factura No Identificado]");
            }
            if (psRegistro.Length < 182)
            {
                pbPendiente = true;
                psMensajePendiente.Length = 0;
                psMensajePendiente.Append("[La Longitud del Registro no es correcta]");
            }
            if (pbPendiente)
            {
                phtTablaEnvio.Clear();
                InsertarRegistroDet("DetalleFacturaA", psTpRegFac, psRegistro);
                InsertarRegistroDet("DetalleFacturaB", psTpRegFac, psRegistro);
                return;
            }

            //Definiendo valores
            try
            {
                psCuentaMaestra = psRegistro.Substring(0, 7).Trim();
                psIdentificador = psRegistro.Substring(7, 10).Trim();
                CodClaveCargo = psRegistro.Substring(161, 2).Trim();
                psPoblacionDestino = psRegistro.Substring(90, 17).Trim();
                psTelDestino = psRegistro.Substring(37, 17).Trim();
                psPlanTarifario = psRegistro.Substring(113, 2).Trim();
                psFolioLD = psRegistro.Substring(115, 5).Trim();
                psModalidad = psRegistro.Substring(181, 1).Trim();
                psMarcaLargaDuracion = psRegistro.Substring(180, 1).Trim();
                if (psRegistro.Substring(133, 3).Trim().Length > 0 && !(int.TryParse(psRegistro.Substring(133, 3).Trim(), out piNoTarjetaTelCard)))
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. No. Tarjeta TelCard.]");
                }
                if (psRegistro.Substring(30, 7).Trim().Length > 0 && !(int.TryParse(psRegistro.Substring(30, 7).Trim(), out piIdPoblacionDestino)))
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. ID Población Destino.]");
                }
                if (psRegistro.Substring(77, 12).Trim().Length > 0 && !(double.TryParse(psRegistro.Substring(77, 12).Trim(), out pdImporte)))
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Importe.]");
                }
                pdImporte = CalcularImporte(psRegistro.Substring(77, 13).Trim());
                if (pdImporte == double.MinValue && psRegistro.Substring(77, 13).Length > 0)
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Importe.]");
                }
                if (psRegistro.Substring(72, 5).Trim().Length > 0 && !(double.TryParse(psRegistro.Substring(72, 5).Trim(), out pdDuracion)))
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Duración.]");
                }
                pdtFechaFacturacion = Util.IsDate(psRegistro.Substring(18, 6).Trim(), "yyyyMM");
                if (psRegistro.Substring(18, 6).Trim().Length > 0 && pdtFechaFacturacion == DateTime.MinValue)
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Fecha Facturación.]");
                }
                pdtFechaInicio = Util.IsDate(psRegistro.Substring(64, 8).Trim(), "yyyyMMdd");
                if (psRegistro.Substring(64, 8).Trim().Length > 0 && pdtFechaInicio == DateTime.MinValue)
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Fecha Inicio.]");
                }
                pdtHoraInicio = Util.IsDate("1900/01/01 " + psRegistro.Substring(107, 6).Trim(), "yyyy/MM/dd HHmmss");
                if (psRegistro.Substring(107, 6).Trim().Length > 0 && pdtHoraInicio == DateTime.MinValue && psRegistro.Substring(107, 6).Trim() != "000000")
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Hora Inicio.]");
                }
                CodTpLlamLD = psRegistro.Substring(28, 1).Trim();
                CodOperLada = psRegistro.Substring(25, 1).Trim();
                CodCveCobrar = psRegistro.Substring(29, 1).Trim();
                CodTpLlam = psRegistro.Substring(27, 1).Trim();
                psTraficoVPN = psRegistro.Substring(179, 1).Trim();
                psClavesVarias = psRegistro.Substring(24, 1).Trim();
                psNo800 = psRegistro.Substring(123, 10).Trim();
                psCilliCode = psRegistro.Substring(143, 11).Trim();
                psMarcaCodigoLada = psRegistro.Substring(157, 1).Trim();
                psCarrier = psRegistro.Substring(140, 3).Trim();
                psTarjetaVPNet = psRegistro.Substring(165, 13).Trim();
            }
            catch
            {
                psMensajePendiente.Append("[Error al Asignar Datos]");
                pbPendiente = true;
            }

            if (pbPendiente)
            {
                phtTablaEnvio.Clear();
                InsertarRegistroDet("DetalleFacturaA", psTpRegFac, psRegistro);
                InsertarRegistroDet("DetalleFacturaB", psTpRegFac, psRegistro);
                return;
            }

            //switch (psClavesVarias.Trim())
            //{
            //    case "A":
            //        {
            //            CodClaveCargo = "PM";
            //            break;
            //        }
            //    case "B":
            //        {
            //            CodClaveCargo = "PM";
            //            break;
            //        }
            //    case "U":
            //        {
            //            CodClaveCargo = "TB";
            //            break;
            //        }
            //    case "I":
            //        {
            //            CodClaveCargo = "IR";
            //            break;
            //        }
            //    case "E":
            //        {
            //            CodClaveCargo = "TC";
            //            break;
            //        }
            //}

            if (!ValidarRegistro() && !pbPendiente)
            {
                pbPendiente = true;
            }

            if (CodTpLlam.Replace(psServicioCarga, "").Length > 0 && piCatTpLlam == int.MinValue)
            {
                pbPendiente = true;
                psMensajePendiente.Append("[No se identificó el Tipo de Llamada]");
                InsertarTpLlam(CodTpLlam);

            }
            if (CodOperLada.Replace(psServicioCarga, "").Length > 0 && piCatOperLada == int.MinValue)
            {
                pbPendiente = true;
                psMensajePendiente.Append("[No se encontró Operador Lada]");
                InsertarOperLada(CodOperLada);
            }

            if (CodTpLlamLD.Replace(psServicioCarga, "").Length > 0 && piCatTpLlamLD == int.MinValue)
            {
                pbPendiente = true;
                psMensajePendiente.Append("[No se identificó el Tipo de Llamada de Larga Distancia]");
                InsertarTpLlamLD(CodTpLlamLD);
            }

            if (CodCveCobrar.Replace(psServicioCarga, "").Length > 0 && piCatCveCobrar == int.MinValue)
            {
                pbPendiente = true;
                psMensajePendiente.Append("[No se identificó si la llamada es de Entrada o Salida]");
                InsertarCveCobrar(CodCveCobrar);
            }

            phtTablaEnvio.Clear();
            phtTablaEnvio.Add("{ClaveCar}", piCatClaveCargo);
            phtTablaEnvio.Add("{Linea}", piCatIdentificador);
            phtTablaEnvio.Add("{TpLlamLD}", piCatTpLlamLD);
            phtTablaEnvio.Add("{OperLada}", piCatOperLada);
            phtTablaEnvio.Add("{CveCobrar}", piCatCveCobrar);
            phtTablaEnvio.Add("{TpLlam}", piCatTpLlam);
            phtTablaEnvio.Add("{CtaMae}", psCuentaMaestra);
            phtTablaEnvio.Add("{Ident}", psIdentificador);
            phtTablaEnvio.Add("{CveCargo}", CodClaveCargo);
            phtTablaEnvio.Add("{TarjVPNet}", psTarjetaVPNet);
            phtTablaEnvio.Add("{PobDest}", psPoblacionDestino);
            phtTablaEnvio.Add("{TelDest}", psTelDestino);
            phtTablaEnvio.Add("{PlanTarifa}", psPlanTarifario);
            phtTablaEnvio.Add("{FolioLD}", psFolioLD);
            phtTablaEnvio.Add("{Modalidad}", psModalidad);
            phtTablaEnvio.Add("{MLDura}", psMarcaLargaDuracion);
            phtTablaEnvio.Add("{TarjTelCard}", piNoTarjetaTelCard);
            if (CodCveCobrar == "Telmex2")
            {
                //Llamadas por Cobrar
                phtTablaEnvio.Add("{IdPobOrig}", piIdPoblacionDestino);
            }
            else
            {
                phtTablaEnvio.Add("{IdPobDest}", piIdPoblacionDestino);
            }
            //RZ.20140221 Multiplicar por el tipo de cambio encontrado en base a la moneda de la carga
            phtTablaEnvio.Add("{Importe}", pdImporte * pdTipoCambioVal);
            //RZ.20140221 Agregar el tipo de cambio al hash de detalle
            phtTablaEnvio.Add("{TipoCambioVal}", pdTipoCambioVal);
            if (pdDuracion != double.MinValue)
            {
                phtTablaEnvio.Add("{DuracionSeg}", pdDuracion * 60.0);
            }
            phtTablaEnvio.Add("{FechaFactura}", pdtFechaFacturacion);
            phtTablaEnvio.Add("{FechaInicio}", pdtFechaInicio);
            phtTablaEnvio.Add("{HoraInicio}", pdtHoraInicio);
            /* RZ.20120928 Inlcuir fecha de publicación para la factura */
            phtTablaEnvio.Add("{FechaPub}", pdtFechaPublicacion);

            InsertarRegistroDet("DetalleFacturaA", psTpRegFac, psRegistro);

            phtTablaEnvio.Clear();
            phtTablaEnvio.Add("{TrafVPN}", psTraficoVPN);
            phtTablaEnvio.Add("{CveVarias}", psClavesVarias);
            phtTablaEnvio.Add("{No800}", psNo800);
            phtTablaEnvio.Add("{CilliCode}", psCilliCode);
            phtTablaEnvio.Add("{MCodLada}", psMarcaCodigoLada);
            phtTablaEnvio.Add("{NomCarrier}", psCarrier);

            InsertarRegistroDet("DetalleFacturaB", psTpRegFac, psRegistro);

        }

        private void DetServicioMedido()
        {
            // TipoRegistro = Telmex Servicio Medido            
            if (!SetCatTpRegFac(psTpRegFac))
            {
                pbPendiente = true;
                psMensajePendiente.Length = 0;
                psMensajePendiente.Append("[Tipo Registro de Factura No Identificado]");
            }
            if (psRegistro.Length < 165)
            {
                pbPendiente = true;
                psMensajePendiente.Length = 0;
                psMensajePendiente.Append("[La Longitud del Registro no es correcta]");
            }
            if (pbPendiente)
            {
                phtTablaEnvio.Clear();
                InsertarRegistroDet("DetalleFacturaA", psTpRegFac, psRegistro);
                return;
            }

            //Definiendo valores
            try
            {
                psCuentaMaestra = psRegistro.Substring(0, 7).Trim();
                psIdentificador = psRegistro.Substring(7, 10).Trim();
                CodClaveCargo = psRegistro.Substring(161, 2).Trim();
                psUnidad = psRegistro.Substring(67, 1).Trim();
                if (psRegistro.Substring(40, 7).Trim().Length > 0 && !(int.TryParse(psRegistro.Substring(40, 7).Trim(), out piCantidad)))
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Cantidad.]");
                }
                if (psRegistro.Substring(47, 12).Trim().Length > 0 && !(double.TryParse(psRegistro.Substring(47, 12).Trim(), out pdImporte)))
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Importe.]");
                }
                pdImporte = CalcularImporte(psRegistro.Substring(47, 13));
                if (pdImporte == double.MinValue && psRegistro.Substring(47, 13).Length > 0)
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Importe.]");
                }
                if (psRegistro.Substring(60, 7).Trim().Length > 0 && !(double.TryParse(psRegistro.Substring(60, 7).Trim(), out pdDuracion)))
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Duración.]");
                }
                pdtFechaFacturacion = Util.IsDate(psRegistro.Substring(18, 6).Trim(), "yyyyMM");
                if (psRegistro.Substring(18, 6).Trim().Length > 0 && pdtFechaFacturacion == DateTime.MinValue)
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Fecha Facturación.]");
                }
                pdtFechaInicio = Util.IsDate(psRegistro.Substring(24, 8).Trim(), "yyyyMMdd");
                if (psRegistro.Substring(24, 8).Trim().Length > 0 && pdtFechaInicio == DateTime.MinValue)
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Fecha Inicio.]");
                }
                pdtFechaFin = Util.IsDate(psRegistro.Substring(32, 8).Trim(), "yyyyMMdd");
                if (psRegistro.Substring(32, 8).Trim().Length > 0 && pdtFechaFin == DateTime.MinValue)
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Fecha Fin.]");
                }
            }
            catch
            {
                psMensajePendiente.Append("[Error al Asignar Datos]");
                pbPendiente = true;
            }

            if (pbPendiente)
            {
                phtTablaEnvio.Clear();
                InsertarRegistroDet("DetalleFacturaA", psTpRegFac, psRegistro);
                return;
            }

            if (!ValidarRegistro() && !pbPendiente)
            {
                pbPendiente = true;
            }

            phtTablaEnvio.Clear();
            phtTablaEnvio.Add("{ClaveCar}", piCatClaveCargo);
            phtTablaEnvio.Add("{Linea}", piCatIdentificador);
            phtTablaEnvio.Add("{CtaMae}", psCuentaMaestra);
            phtTablaEnvio.Add("{Ident}", psIdentificador);
            phtTablaEnvio.Add("{CveCargo}", CodClaveCargo);
            phtTablaEnvio.Add("{Unidad}", psUnidad);
            phtTablaEnvio.Add("{Cantidad}", piCantidad);
            phtTablaEnvio.Add("{Importe}", pdImporte);
            if (pdDuracion != double.MinValue)
            {
                phtTablaEnvio.Add("{DuracionSeg}", pdDuracion * 60.0);
            }
            phtTablaEnvio.Add("{FechaFactura}", pdtFechaFacturacion);
            phtTablaEnvio.Add("{FechaInicio}", pdtFechaInicio);
            phtTablaEnvio.Add("{FechaFin}", pdtFechaFin);
            /* RZ.20120928 Inlcuir fecha de publicación para la factura */
            phtTablaEnvio.Add("{FechaPub}", pdtFechaPublicacion);

            InsertarRegistroDet("DetalleFacturaA", psTpRegFac, psRegistro);

        }

        private void DetRentasYOtrosCargosRFC()
        {
            // TipoRegistro = Telmex Rentas y Otros Cargos            
            if (!SetCatTpRegFac(psTpRegFac))
            {
                pbPendiente = true;
                psMensajePendiente.Length = 0;
                psMensajePendiente.Append("[Tipo Registro de Factura No Identificado]");
            }
            if (psRegistro.Length < 184)
            {
                pbPendiente = true;
                psMensajePendiente.Length = 0;
                psMensajePendiente.Append("[La Longitud del Registro no es correcta]");
            }
            if (pbPendiente)
            {
                phtTablaEnvio.Clear();
                InsertarRegistroDet("DetalleFacturaA", psTpRegFac, psRegistro);
                InsertarRegistroDet("DetalleFacturaB", psTpRegFac, psRegistro);
                return;
            }
            //Definiendo valores
            try
            {
                psCuentaMaestra = psRegistro.Substring(0, 7).Trim();
                psIdentificador = psRegistro.Substring(7, 10).Trim();
                psCuenta = psRegistro.Substring(106, 7).Trim();
                CodClaveCargo = "InfC"; //Información del Cliente
                psClaveNombre = psRegistro.Substring(24, 1).Trim();
                psCalle = psRegistro.Substring(54, 52).Trim();
                psNombre = psRegistro.Substring(25, 29).Trim();
                if (psRegistro.Substring(117, 14).Trim().Length > 0 && !(double.TryParse(psRegistro.Substring(117, 14).Trim(), out pdImporte)))
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Importe.]");
                }
                pdImporte = CalcularImporte(psRegistro.Substring(117, 15));
                if (pdImporte == double.MinValue && psRegistro.Substring(117, 15).Length > 0)
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Importe.]");
                }
                if (psRegistro.Substring(161, 2).Trim().Length > 0 && !(double.TryParse(psRegistro.Substring(161, 2).Trim(), out pdPorcentajeIVA)))
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Porcentaje IVA.]");
                }
                pdtFechaFacturacion = Util.IsDate(psRegistro.Substring(18, 6).Trim(), "yyyyMM");
                if (psRegistro.Substring(18, 6).Trim().Length > 0 && pdtFechaFacturacion == DateTime.MinValue)
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Fecha Facturación.]");
                }
                psEstadoDeRegistro = psRegistro.Substring(183, 1).Trim();
                psSector = psRegistro.Substring(180, 3).Trim();
                psDivision = psRegistro.Substring(179, 1).Trim();
            }
            catch
            {
                psMensajePendiente.Append("[Error al Asignar Datos]");
                pbPendiente = true;
            }

            if (pbPendiente)
            {
                phtTablaEnvio.Clear();
                InsertarRegistroDet("DetalleFacturaA", psTpRegFac, psRegistro);
                InsertarRegistroDet("DetalleFacturaB", psTpRegFac, psRegistro);
                return;
            }

            if (!ValidarRegistro() && !pbPendiente)
            {
                pbPendiente = true;
            }

            phtTablaEnvio.Clear();
            phtTablaEnvio.Add("{ClaveCar}", piCatClaveCargo);
            phtTablaEnvio.Add("{Linea}", piCatIdentificador);
            phtTablaEnvio.Add("{CtaMae}", psCuentaMaestra);
            phtTablaEnvio.Add("{Ident}", psIdentificador);
            phtTablaEnvio.Add("{Cuenta}", psCuenta);
            phtTablaEnvio.Add("{CveNombre}", psClaveNombre);
            phtTablaEnvio.Add("{Calle}", psCalle);
            phtTablaEnvio.Add("{Nombre}", psNombre);
            //RZ.20140221 Multiplicar por el tipo de cambio encontrado en base a la moneda de la carga
            phtTablaEnvio.Add("{Importe}", pdImporte * pdTipoCambioVal);
            phtTablaEnvio.Add("{PorcIVA}", pdPorcentajeIVA * pdTipoCambioVal);
            //RZ.20140221 Agregar el tipo de cambio al hash de detalle
            phtTablaEnvio.Add("{TipoCambioVal}", pdTipoCambioVal);
            phtTablaEnvio.Add("{FechaFactura}", pdtFechaFacturacion);
            /* RZ.20120928 Inlcuir fecha de publicación para la factura */
            phtTablaEnvio.Add("{FechaPub}", pdtFechaPublicacion);

            InsertarRegistroDet("DetalleFacturaA", psTpRegFac, psRegistro);

            phtTablaEnvio.Clear();
            phtTablaEnvio.Add("{EdodeReg}", psEstadoDeRegistro);
            phtTablaEnvio.Add("{Sector}", psSector);
            phtTablaEnvio.Add("{Division}", psDivision);

            InsertarRegistroDet("DetalleFacturaB", psTpRegFac, psRegistro);
        }

        private void DetRentasYOtrosCargosImportes()
        {
            // TipoRegistro = Telmex Rentas y Otros Cargos            
            if (!SetCatTpRegFac(psTpRegFac))
            {
                pbPendiente = true;
                psMensajePendiente.Length = 0;
                psMensajePendiente.Append("[Tipo Registro de Factura No Identificado]");
            }
            if (psRegistro.Length < 175)
            {
                pbPendiente = true;
                psMensajePendiente.Length = 0;
                psMensajePendiente.Append("[La Longitud del Registro no es correcta]");
            }
            if (pbPendiente)
            {
                phtTablaEnvio.Clear();
                InsertarRegistroDet("DetalleFacturaA", psTpRegFac, psRegistro);
                InsertarRegistroDet("DetalleFacturaB", psTpRegFac, psRegistro);
                return;
            }

            //Definiendo valores
            try
            {
                psCuentaMaestra = psRegistro.Substring(0, 7).Trim();
                psIdentificador = psRegistro.Substring(7, 10).Trim();
                CodClaveCargo = psRegistro.Substring(161, 2).Trim();
                psConceptoFS = psRegistro.Substring(117, 2).Trim();
                psLadaTelefono = psRegistro.Substring(165, 10).Trim();
                psClaveNombre = psRegistro.Substring(24, 1).Trim();
                psReferenciaSisa = psRegistro.Substring(87, 15).Trim();
                if (psRegistro.Substring(48, 5).Trim().Length > 0 && !(int.TryParse(psRegistro.Substring(48, 5).Trim(), out piCantidad)))
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Cantidad.]");
                }
                if (psRegistro.Substring(53, 12).Length > 0 && !(double.TryParse(psRegistro.Substring(53, 12), out pdImporte)))
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Importe.]");
                }
                pdImporte = CalcularImporte(psRegistro.Substring(53, 13));
                if (pdImporte == double.MinValue && psRegistro.Substring(58, 13).Length > 0)
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Importe.]");
                }
                if (psRegistro.Substring(66, 11).Trim().Length > 0 && !(double.TryParse(psRegistro.Substring(66, 11).Trim(), out pdDuracion)))
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Duración.]");
                }
                pdtFechaFacturacion = Util.IsDate(psRegistro.Substring(18, 6).Trim(), "yyyyMM");
                if (psRegistro.Substring(18, 6).Trim().Length > 0 && pdtFechaFacturacion == DateTime.MinValue)
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Fecha Facturación.]");
                }
                pdtFechaCargo = Util.IsDate(psRegistro.Substring(119, 6), "yyyyMM");
                if (psRegistro.Substring(119, 6).Trim().Length > 0 && pdtFechaCargo == DateTime.MinValue)
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Fecha Cargo.]");
                }
                psTroncal = psRegistro.Substring(25, 10).Trim();
                psDescripcion = psRegistro.Substring(35, 13).Trim();
            }
            catch
            {
                psMensajePendiente.Append("[Error al Asignar Datos]"); ;
                pbPendiente = true;
            }

            if (pbPendiente)
            {
                phtTablaEnvio.Clear();
                InsertarRegistroDet("DetalleFacturaA", psTpRegFac, psRegistro);
                InsertarRegistroDet("DetalleFacturaB", psTpRegFac, psRegistro);
                return;
            }

            if (!ValidarRegistro() && !pbPendiente)
            {
                pbPendiente = true;
            }

            phtTablaEnvio.Clear();
            phtTablaEnvio.Add("{ClaveCar}", piCatClaveCargo);
            phtTablaEnvio.Add("{Linea}", piCatIdentificador);
            phtTablaEnvio.Add("{CtaMae}", psCuentaMaestra);
            phtTablaEnvio.Add("{Ident}", psIdentificador);
            phtTablaEnvio.Add("{CveCargo}", CodClaveCargo);
            phtTablaEnvio.Add("{CptoFS}", psConceptoFS);
            phtTablaEnvio.Add("{LadaTel}", psLadaTelefono);
            phtTablaEnvio.Add("{CveNombre}", psClaveNombre);
            phtTablaEnvio.Add("{RefSisa}", psReferenciaSisa);
            phtTablaEnvio.Add("{Cantidad}", piCantidad);
            //RZ.20140221 Multiplicar por el tipo de cambio encontrado en base a la moneda de la carga
            phtTablaEnvio.Add("{Importe}", pdImporte * pdTipoCambioVal);
            //RZ.20140221 Agregar el tipo de cambio al hash de detalle
            phtTablaEnvio.Add("{TipoCambioVal}", pdTipoCambioVal);
            if (pdDuracion != double.MinValue)
            {
                phtTablaEnvio.Add("{Intercnx}", pdDuracion * 60.0);
            }
            phtTablaEnvio.Add("{FechaFactura}", pdtFechaFacturacion);
            phtTablaEnvio.Add("{FechaCargo}", pdtFechaCargo);
            /* RZ.20120928 Inlcuir fecha de publicación para la factura */
            phtTablaEnvio.Add("{FechaPub}", pdtFechaPublicacion);

            InsertarRegistroDet("DetalleFacturaA", psTpRegFac, psRegistro);

            phtTablaEnvio.Clear();
            phtTablaEnvio.Add("{Troncal}", psTroncal);
            phtTablaEnvio.Add("{Descripcion}", psDescripcion);

            InsertarRegistroDet("DetalleFacturaB", psTpRegFac, psRegistro);

        }

        private void DetEnlaces()
        {
            // TipoRegistro = Telmex Enlaces            
            if (!SetCatTpRegFac(psTpRegFac))
            {
                pbPendiente = true;
                psMensajePendiente.Length = 0;
                psMensajePendiente.Append("[Tipo Registro de Factura No Identificado]");
            }
            if (psRegistro.Length < 185)
            {
                pbPendiente = true;
                psMensajePendiente.Length = 0;
                psMensajePendiente.Append("[La Longitud del Registro no es correcta]");
            }
            if (pbPendiente)
            {
                phtTablaEnvio.Clear();
                InsertarRegistroDet("DetalleFacturaA", psTpRegFac, psRegistro);
                return;
            }
            //Definiendo propiedades
            try
            {
                psCuentaMaestra = psRegistro.Substring(0, 7).Trim();
                psIdentificador = psRegistro.Substring(0, 7).Trim();
                CodClaveCargo = psRegistro.Substring(161, 2).Trim();
                psTipoLadaEnlace = psRegistro.Substring(24, 4).Trim();
                psPuntaA = psRegistro.Substring(165, 10).Trim();
                psPoblacionA = psRegistro.Substring(28, 23).Trim();
                psPuntaB = psRegistro.Substring(175, 10).Trim();
                psPoblacionB = psRegistro.Substring(51, 23).Trim();
                psAsignacion = psRegistro.Substring(7, 7).Trim();
                psActaRecepcion = psRegistro.Substring(123, 12).Trim();
                if (psRegistro.Substring(83, 9).Length > 0 && !(double.TryParse(psRegistro.Substring(83, 9), out pdImporte)))
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Importe.]");
                }
                pdImporte = CalcularImporte(psRegistro.Substring(83, 10));
                if (pdImporte == double.MinValue && psRegistro.Substring(83, 10).Length > 0)
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Importe.]");
                }
                if (psRegistro.Substring(78, 5).Trim().Length > 0 && !(double.TryParse(psRegistro.Substring(78, 5).Trim(), out pdKM)))
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. KM.]");
                }
                if (psRegistro.Substring(74, 4).Trim().Length > 0 && !(int.TryParse(psRegistro.Substring(74, 4).Trim(), out piCantidad)))
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Cantidad.]");
                }
                pdtFechaFacturacion = Util.IsDate(psRegistro.Substring(18, 6), "yyyyMM");
                if (psRegistro.Substring(18, 6).Trim().Length > 0 && pdtFechaFacturacion == DateTime.MinValue)
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Fecha Facturación.]");
                }
                pdtFechaInicio = Util.IsDate(psRegistro.Substring(117, 6), "yyMMdd");
                if (psRegistro.Substring(117, 6).Trim().Length > 0 && pdtFechaInicio == DateTime.MinValue)
                {
                    pbPendiente = true;
                    psMensajePendiente.Append("[Formato Incorrecto. Fecha Inicio.]");
                }
            }
            catch
            {
                psMensajePendiente.Append("[Error al Asignar Datos]");
                pbPendiente = true;
            }

            if (pbPendiente)
            {
                phtTablaEnvio.Clear();
                InsertarRegistroDet("DetalleFacturaA", psTpRegFac, psRegistro);
                return;
            }

            if (!ValidarRegistro() && !pbPendiente)
            {
                pbPendiente = true;
            }

            phtTablaEnvio.Clear();
            phtTablaEnvio.Add("{ClaveCar}", piCatClaveCargo);
            phtTablaEnvio.Add("{Linea}", piCatIdentificador);
            phtTablaEnvio.Add("{CtaMae}", psCuentaMaestra);
            phtTablaEnvio.Add("{CveCargo}", CodClaveCargo);
            phtTablaEnvio.Add("{TpLadaEn}", psTipoLadaEnlace);
            phtTablaEnvio.Add("{PuntaA}", psPuntaA);
            phtTablaEnvio.Add("{PobA}", psPoblacionA);
            phtTablaEnvio.Add("{PuntaB}", psPuntaB);
            phtTablaEnvio.Add("{PobB}", psPoblacionB);
            phtTablaEnvio.Add("{Asigna}", psAsignacion);
            phtTablaEnvio.Add("{ActRecep}", psActaRecepcion);
            phtTablaEnvio.Add("{Cantidad}", piCantidad);
            //RZ.20140221 Multiplicar por el tipo de cambio encontrado en base a la moneda de la carga
            phtTablaEnvio.Add("{Importe}", pdImporte * pdTipoCambioVal);
            phtTablaEnvio.Add("{KM}", pdKM * pdTipoCambioVal);
            //RZ.20140221 Agregar el tipo de cambio al hash de detalle
            phtTablaEnvio.Add("{TipoCambioVal}", pdTipoCambioVal);
            phtTablaEnvio.Add("{FechaFactura}", pdtFechaFacturacion);
            phtTablaEnvio.Add("{FechaInicio}", pdtFechaInicio);
            /* RZ.20120928 Inlcuir fecha de publicación para la factura */
            phtTablaEnvio.Add("{FechaPub}", pdtFechaPublicacion);

            InsertarRegistroDet("DetalleFacturaA", psTpRegFac, psRegistro);
        }

        protected override bool ValidarRegistro()
        {
            bool lbRegValido = true;

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
                    return false;
                }

                if (!ValidarIdentificadorSitio())
                {
                    return false;
                }

                /*RZ.20130815 Validar si la linea es publicable*/
                if (!ValidarLineaNoPublicable())
                {
                    lbRegValido = false;
                    pbPendiente = true;
                }
            }
            else if (!pbSinLineaEnDetalle)
            {
                psMensajePendiente.Append("[La Línea no se encuentra en el sistema]");
                InsertarLinea(psIdentificador);
                lbRegValido = false;
            }

            pdrClaveCargo = GetClaveCargo(CodClaveCargo);

            if (pdrClaveCargo != null)
            {

                if (pdrClaveCargo["{ClaveCar}"] == System.DBNull.Value)
                {

                    if (pdrClaveCargo["iCodCatalogo"] != System.DBNull.Value)
                    {
                        piCatClaveCargo = (int)pdrClaveCargo["iCodCatalogo"];
                    }
                    else
                    {
                        psMensajePendiente.Append("[Error al asignar Clave Cargo]");
                        return false;
                    }
                }
                else
                {
                    //Si la Clave Cargo es de Tipo Impuesto Especial, la Clave Cargo del Registro debe ser 'IEsp'
                    if (pdrClaveCargo["iCodCatalogo"] is int)
                    {
                        piCatClaveCargo = (int)pdrClaveCargo["iCodCatalogo"];
                    }
                    else
                    {
                        psMensajePendiente.Append("[Error al asignar Clave Cargo]");
                        return false;
                    }
                }

                /*RZ.20130815 Validar si la linea esta como conmutada y si la calve de cargo es no publicable
                 * Solo para cuando traigo la linea identificada
                 */
                if (pdrLinea != null)
                {
                    if (!ValidarLineaConmutadaClaveCargo())
                    {
                        lbRegValido = false;
                        pbPendiente = true;
                    }
                }

            }
            else if (CodClaveCargo.Length > psServicioCarga.Length)
            {

                psMensajePendiente.Append("[La Clave Cargo no se encuentra en el sistema]");
                InsertarClaveCargo(CodClaveCargo);
                lbRegValido = false;
            }

            return lbRegValido;
        }

        private double CalcularImporte(string lsImporte)
        {
            string lsImportetxt;
            double ldImporte;
            /* Este campo contiene el importe de la llamada sin punto y dos decimales,  las dos últimas 
            cifras representan los centavos no considera el I.V.A.*/
            lsImportetxt = lsImporte.Substring(0, lsImporte.Length - 2) + "." +
                            lsImporte.Substring(lsImporte.Length - 2, 1);

            /* el 1er. decimal de derecha a izquierda puede aparecer con los siguientes símbolos  ASCII, con el 
            valor equivalente conforme con la siguiente tabla*/
            switch (lsImporte.Substring(lsImporte.Length - 1, 1))
            {
                case "0":
                    {
                        ldImporte = double.Parse(lsImportetxt + "0");
                        break;
                    }
                case "{":
                    {
                        ldImporte = double.Parse(lsImportetxt + "0");
                        break;
                    }
                case "A":
                    {
                        ldImporte = double.Parse(lsImportetxt + "1");
                        break;
                    }
                case "B":
                    {
                        ldImporte = double.Parse(lsImportetxt + "2");
                        break;
                    }
                case "C":
                    {
                        ldImporte = double.Parse(lsImportetxt + "3");
                        break;
                    }
                case "D":
                    {
                        ldImporte = double.Parse(lsImportetxt + "4");
                        break;
                    }
                case "E":
                    {
                        ldImporte = double.Parse(lsImportetxt + "5");
                        break;
                    }
                case "F":
                    {
                        ldImporte = double.Parse(lsImportetxt + "6");
                        break;
                    }
                case "G":
                    {
                        ldImporte = double.Parse(lsImportetxt + "7");
                        break;
                    }
                case "H":
                    {
                        ldImporte = double.Parse(lsImportetxt + "8");
                        break;
                    }
                case "I":
                    {
                        ldImporte = double.Parse(lsImportetxt + "9");
                        break;
                    }
                case "U":
                    {
                        ldImporte = double.Parse(lsImportetxt + "4");
                        break;
                    }
                case "J":
                    {
                        ldImporte = double.Parse(lsImportetxt + "1") * -1;
                        break;
                    }
                case "K":
                    {
                        ldImporte = double.Parse(lsImportetxt + "2") * -1;
                        break;
                    }
                case "L":
                    {
                        ldImporte = double.Parse(lsImportetxt + "3") * -1;
                        break;
                    }
                case "M":
                    {
                        ldImporte = double.Parse(lsImportetxt + "4") * -1;
                        break;
                    }
                case "N":
                    {
                        ldImporte = double.Parse(lsImportetxt + "5") * -1;
                        break;
                    }
                case "O":
                    {
                        ldImporte = double.Parse(lsImportetxt + "6") * -1;
                        break;
                    }
                case "P":
                    {
                        ldImporte = double.Parse(lsImportetxt + "7") * -1;
                        break;
                    }
                case "Q":
                    {
                        ldImporte = double.Parse(lsImportetxt + "8") * -1;
                        break;
                    }
                case "R":
                    {
                        ldImporte = double.Parse(lsImportetxt + "9") * -1;
                        break;
                    }
                case "}":
                    {
                        ldImporte = double.Parse(lsImportetxt + "0") * -1;
                        break;
                    }
                default:
                    {
                        return double.MinValue;
                    }
            }
            return ldImporte;
        }



        protected override void InitValores()
        {
            base.InitValores();
            psClaveNombre = "";
            psPoblacionDestino = "";
            psTelDestino = "";
            psPlanTarifario = "";
            psFolioLD = "";
            psModalidad = "";
            psMarcaLargaDuracion = "";
            piNoTarjetaTelCard = int.MinValue;
            piCantidad = int.MinValue;
            piIdPoblacionDestino = int.MinValue;
            pdImporte = double.MinValue;
            pdDuracion = double.MinValue;
            pdtFechaFacturacion = DateTime.MinValue;
            pdtFechaInicio = DateTime.MinValue;
            pdtFechaFin = DateTime.MinValue;
            pdtHoraInicio = DateTime.MinValue;
            pdtFechaCargo = DateTime.MinValue;
            psTraficoVPN = "";
            psClavesVarias = "";
            psNo800 = "";
            psCilliCode = "";
            psMarcaCodigoLada = "";
            psCarrier = "";
            psTarjetaVPNet = "";
            psUnidad = "";
            psCuenta = "";
            psCalle = "";
            psNombre = "";
            pdPorcentajeIVA = double.MinValue;
            psEstadoDeRegistro = "";
            psSector = "";
            psDivision = "";
            psConceptoFS = "";
            psLadaTelefono = "";
            psReferenciaSisa = "";
            psTroncal = "";
            psDescripcion = "";
            psTipoLadaEnlace = "";
            psPuntaA = "";
            psPoblacionA = "";
            psPuntaB = "";
            psPoblacionB = "";
            psAsignacion = "";
            psActaRecepcion = "";
            pdKM = double.MinValue;
        }

        protected override void LlenarBDLocal()
        {
            base.LlenarBDLocal();
            LlenarDTCatalogo(new string[] { "OperLada", "TpLlamLD", "CveCobrar" });
            LlenarDTHisOperLada();
            LlenarDTHisTpLlamLD();
            LlenarDTHisCveCobrar();
        }
    }
}

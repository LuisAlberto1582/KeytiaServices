/*
Nombre:		    Daniel Medina Moreno
Fecha:		    20110930
Descripción:	Clase para generar y enviar reportes especiales
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.ComponentModel;
using System.Net.Mail;
using KeytiaServiceBL.Alarmas;
using System.Collections;

namespace KeytiaServiceBL.ReporteEspecial
{
    class AplicaReporteEspecial
    {
        protected bool pbSitio;
        protected bool pbCenCos;
        protected bool pbEmple;
        protected bool pbVicepre;
        protected bool pbTDest;
        protected bool pbEmpre;
        protected bool pbExten;
        protected bool pbLocali;
        protected bool pbCarrier;
        protected bool pbClaveCar;
        protected bool pbIncluirJerarquias;
        protected bool pbAdicionarUsuarioWEB;
        protected bool pbEnviarUltimoAcceso;
        protected bool pbSinCtaEnviarAlReponsableCC;
        protected bool pbSinCtaEnviarASoporteInterno;
        protected bool pbServicioMedido;
        protected bool pbLlamadasLocales;
        protected bool pbClient;

        public bool Sitio
        {
            get { return pbSitio; }
            set { pbSitio = value; }
        }
        public bool CenCos
        {
            get { return pbCenCos; }
            set { pbCenCos = value; }
        }
        public bool Emple
        {
            get { return pbEmple; }
            set { pbEmple = value; }
        }
        public bool Vicepre
        {
            get { return pbVicepre; }
            set { pbVicepre = value; }
        }
        public bool TDest
        {
            get { return pbTDest; }
            set { pbTDest = value; }
        }
        public bool Empre
        {
            get { return pbEmpre; }
            set { pbEmpre = value; }
        }
        public bool Exten
        {
            get { return pbExten; }
            set { pbExten = value; }
        }
        public bool Locali
        {
            get { return pbLocali; }
            set { pbLocali = value; }
        }
        public bool Carrier
        {
            get { return pbCarrier; }
            set { pbCarrier = value; }
        }
        public bool ClaveCar
        {
            get { return pbClaveCar; }
            set { pbClaveCar = value; }
        }
        public bool IncluirJerarquias
        {
            get { return pbIncluirJerarquias; }
            set { pbIncluirJerarquias = value; }
        }
        public bool AdicionarUsuarioWEB
        {
            get { return pbAdicionarUsuarioWEB; }
            set { pbAdicionarUsuarioWEB = value; }
        }
        public bool EnviarUltimoAcceso
        {
            get { return pbEnviarUltimoAcceso; }
            set { pbEnviarUltimoAcceso = value; }
        }
        public bool SinCtaEnviarAlReponsableCC
        {
            get { return pbSinCtaEnviarAlReponsableCC; }
            set { pbSinCtaEnviarAlReponsableCC = value; }
        }
        public bool SinCtaEnviarASoporteInterno
        {
            get { return pbSinCtaEnviarASoporteInterno; }
            set { pbSinCtaEnviarASoporteInterno = value; }
        }
        public bool ServicioMedido
        {
            get { return pbServicioMedido; }
            set { pbServicioMedido = value; }
        }
        public bool LlamadasLocales
        {
            get { return pbLlamadasLocales; }
            set { pbLlamadasLocales = value; }
        }
        public bool Client
        {
            get { return pbClient; }
            set { pbClient = value; }
        }
    }

    class RelacionReporteEspecial
    {
        KDBAccess kdb = new KDBAccess();
        protected string psIdioma;
        protected int piCodUsuarioDB;
        protected int piCodSolicitud;
        protected int piCodCatalogo;
        protected string pvchDescripcion;
        protected string psEntidad;
        protected bool pbIncluir;
        protected bool pbAplicaProcesarEnviar;
        protected bool pbProcesar;
        protected bool pbEnviar;
        protected bool pbIncluirJerarquias;
        protected HashSet<int> phsIntValues = new HashSet<int>();
        protected string psStringValues;
        public RelacionReporteEspecial(string lvchDescripcion, int liCodSolicitud, string lvchCodEntidad, bool lbIncluir, 
            bool lbAplicaProcesarEnviar, bool lbProcesar, bool lbEnviar, bool lbIncluirJerarquias, string lsIdioma, int liCodUsuarioDB)
        {
            piCodSolicitud = liCodSolicitud;
            psEntidad = lvchCodEntidad;
            pbIncluir = lbIncluir;
            pbAplicaProcesarEnviar = lbAplicaProcesarEnviar;
            pbProcesar = lbProcesar;
            pbEnviar = lbEnviar;
            psIdioma = lsIdioma;
            pbIncluirJerarquias = lbIncluirJerarquias;
            piCodUsuarioDB = liCodUsuarioDB;
            pvchDescripcion = lvchDescripcion;
        }

        public HashSet<int> getValues()
        {
            phsIntValues.Clear();
            psStringValues = "";
            DataTable ldtRelacion = kdb.GetRelRegByDes(pvchDescripcion, "{Cargas} = " + piCodSolicitud);
            if (pbIncluir)
            {
                if (pbIncluirJerarquias)
                {
                    foreach (DataRow ldr in ldtRelacion.Rows)
                    {
                        if (!(ldr["{" + psEntidad + "}"] is DBNull))
                        {
                            getDependientes((int)ldr["{" + psEntidad + "}"], phsIntValues, null);
                        }
                    }
                }
                else
                {
                    foreach (DataRow ldr in ldtRelacion.Rows)
                    {
                        if (!(ldr["{" + psEntidad + "}"] is DBNull))
                        {
                            phsIntValues.Add((int)ldr["{" + psEntidad + "}"]);
                        }
                    }
                }
            }
            else
            {
                HashSet<string> lhsValuesExcl = new HashSet<string>() { "0" };
                foreach (DataRow ldr in ldtRelacion.Rows)
                {
                    if (!(ldr["{" + psEntidad + "}"] is DBNull))
                    {
                        lhsValuesExcl.Add(ldr["{" + psEntidad + "}"].ToString());
                    }
                }

                DataTable ldtValues = kdb.GetHisRegByEnt(psEntidad, "", new string[] { "iCodCatalogo" },
                    "iCodCatalogo not in (" + string.Join(",", lhsValuesExcl.ToArray()) + ")");

                if (pbIncluirJerarquias)
                {
                    foreach (DataRow ldr in ldtValues.Rows)
                    {
                        getDependientes((int)ldr["iCodCatalogo"], phsIntValues, lhsValuesExcl);
                    }
                }
                else
                {
                    foreach (DataRow ldr in ldtValues.Rows)
                    {
                        phsIntValues.Add((int)ldr["iCodCatalogo"]);
                    }
                }
            }
            psStringValues = UtilAlarma.ToStringList(phsIntValues);
            return phsIntValues;
        }

        public HashSet<int> getValues(RelacionReporteEspecial loRelRptSpc)
        {
            phsIntValues.Clear();
            psStringValues = "";
            HashSet<int> lhsRptSpc = loRelRptSpc.getValues();
            DataTable ldtHistoricos = kdb.GetHisRegByEnt(psEntidad, "",
                                            new string[] { "iCodCatalogo" },
                                            "{" + loRelRptSpc.Entidad + "} in (" + loRelRptSpc.StringValues + ")");
            DataTable ldtRelacion = kdb.GetRelRegByDes(pvchDescripcion, "{Cargas} = " + piCodSolicitud);

            if (pbIncluirJerarquias)
            {
                foreach (DataRow ldr in ldtRelacion.Rows)
                {
                    if (!(ldr["{" + psEntidad + "}"] is DBNull))
                    {
                        getDependientes((int)ldr["{" + psEntidad + "}"], phsIntValues, null);
                    }
                }
            }
            else
            {
                foreach (DataRow ldr in ldtRelacion.Rows)
                {
                    if (!(ldr["{" + psEntidad + "}"] is DBNull))
                    {
                        phsIntValues.Add((int)ldr["{" + psEntidad + "}"]);
                    }
                }
            }


            if (pbIncluir)
            {
                foreach (DataRow ldr in ldtHistoricos.Rows)
                {
                    phsIntValues.Add((int)ldr["iCodCatalogo"]);
                }
            }
            else
            {
                HashSet<int> lhsHistoricValues = new HashSet<int>();
                foreach (DataRow ldr in ldtHistoricos.Rows)
                {
                    if (!phsIntValues.Contains((int)ldr["iCodCatalogo"]))
                    {
                        lhsHistoricValues.Add((int)ldr["iCodCatalogo"]);
                    }
                }
                phsIntValues = lhsHistoricValues;
            }
            psStringValues = UtilAlarma.ToStringList(phsIntValues);
            return phsIntValues;
        }

        protected void getDependientes(int liCodCatalogo, HashSet<int> lhsValues, HashSet<string> lhsValuesExcl)
        {
            try
            {
                if (lhsValuesExcl == null || !lhsValuesExcl.Contains(liCodCatalogo.ToString()))
                {
                    lhsValues.Add(liCodCatalogo);
                    DataTable ldtValues = kdb.GetHisRegByEnt(psEntidad, "", "[{" + psEntidad + "}] = " + liCodCatalogo);
                    foreach (DataRow ldr in ldtValues.Rows)
                    {
                        if (!lhsValues.Contains((int)ldr["iCodCatalogo"]) &&
                            (lhsValuesExcl == null || !lhsValuesExcl.Contains(ldr["iCodCatalogo"].ToString())))
                        {
                            getDependientes((int)ldr["iCodCatalogo"], lhsValues, lhsValuesExcl);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Util.LogException(String.Format("Error al consultar dependientes de la entidad {0}.", psEntidad), ex);
            }
        }
        
        public string StringValues
        {
            get { return psStringValues; }
        }

        public string Idioma
        {
            get { return psIdioma; }
            set { psIdioma = value; }
        }

        public string vchDescripcion
        {
            get { return pvchDescripcion; }
            set { pvchDescripcion = value; }
        }

        public int iCodSolicitud
        {
            get { return piCodSolicitud; }
            set { piCodSolicitud = value; }
        }

        public int iCodCatalogo
        {
            get { return piCodCatalogo; }
            set { piCodCatalogo = value; }
        }

        public string Entidad
        {
            get { return psEntidad; }
            set { psEntidad = value; }
        }

        public bool Incluir
        {
            get { return pbIncluir; }
            set { pbIncluir = value; }
        }

        public bool Procesar
        {
            get { return pbProcesar; }
            set { pbProcesar = value; }
        }

        public bool Enviar
        {
            get { return pbEnviar; }
            set { pbEnviar = value; }
        }

        public bool AplicaProcesarEnviar
        {
            get { return pbAplicaProcesarEnviar; }
            set { pbAplicaProcesarEnviar = value; }
        }

        protected void Baja(int liCodCatalogo)
        {
            Relaciones rel = new Relaciones();
            rel.iCodUsuarioDB = piCodUsuarioDB;
            rel.vchDescripcion = pvchDescripcion;
            rel.Baja("{" + psEntidad + "} = " + liCodCatalogo);
        }

        protected void Agregar(int liCodCatalogo)
        {
            Hashtable lhtValores = new Hashtable();
            lhtValores.Add("{Cargas}", piCodSolicitud);
            lhtValores.Add("{" + psEntidad + "}", liCodCatalogo);
            Relaciones rel = new Relaciones();
            rel.iCodUsuarioDB = piCodUsuarioDB;
            rel.vchDescripcion = pvchDescripcion;
            rel.Agregar(Util.TraducirRelacion(pvchDescripcion, lhtValores));
        }

        public void Actualizar(int liCodCatalogo)
        {
            if (pbIncluir)
            {
                Baja(liCodCatalogo);
            }
            else
            {
                Agregar(liCodCatalogo);
            }
        }

        public string getDescripcion()
        {
            string lsDescripcion = "";
            DataTable ldtDesc = kdb.GetHisRegByEnt(psEntidad, "", "iCodCatalogo = " + piCodCatalogo);
            if (ldtDesc != null && ldtDesc.Rows.Count > 0)
            {
                if (ldtDesc.Columns.Contains("{" + psIdioma + "}") && !(ldtDesc.Rows[0]["{" + psIdioma + "}"] is DBNull))
                {
                    lsDescripcion = ldtDesc.Rows[0]["{" + psIdioma + "}"].ToString();
                }
                else if (psEntidad == "Emple" && ldtDesc.Columns.Contains("{NomCompleto}"))
                {
                    lsDescripcion = ldtDesc.Rows[0]["{NomCompleto}"].ToString();
                }
                else
                {
                    lsDescripcion = ldtDesc.Rows[0]["vchDescripcion"].ToString();
                }
            }
            return lsDescripcion;
        }

        public string getDescripciones()
        {
            List<string> lstDesc = new List<string>();
            DataTable ldtDesc = kdb.GetHisRegByEnt(psEntidad, "", "iCodCatalogo in (" + psStringValues + ")");
            foreach (DataRow ldr in ldtDesc.Rows)
            {
                if (ldtDesc.Columns.Contains("{" + psIdioma + "}") && !(ldr["{" + psIdioma + "}"] is DBNull))
                {
                    lstDesc.Add(ldr["{" + psIdioma + "}"].ToString());
                }
                else if (psEntidad == "Emple" && ldtDesc.Columns.Contains("{NomCompleto}"))
                {
                    lstDesc.Add(ldr["{NomCompleto}"].ToString());
                }
                else
                {
                    lstDesc.Add(ldr["vchDescripcion"].ToString());
                }
            }
            return string.Join(", ", lstDesc.ToArray());
        }
    }

    class ReporteEspecial : CargaServicio
    {
        #region Propiedades

        protected MailAccess poMail;
        protected DataTable pdtBanderasReporte;
        protected int piBanderasReporte;
        protected string psPathPlantillaBase;
        protected string psPathArchivos;
        protected string psCC;
        protected string psCCO;
        protected string psCtaRemitente;
        protected string psNomRemitente;
        protected string psDestinatarioPrueba;
        protected string psCtaSoporte;
        protected int piCodAsunto;
        protected string psPlantillaMail;
        protected string psIdioma;
        protected int piCodCliente;

        protected bool pbGenerarPlantillas;
        protected bool pbEnviarPlantillas;
        protected bool pbIncluirJerarquias;
        protected bool pbAdicionarUsuarioWEB;
        protected bool pbEnviarUltimoAcceso;
        protected bool pbSinCtaEnviarAlReponsableCC;
        protected bool pbSinCtaEnviarASoporteInterno;
        protected bool pbServicioMedido;
        protected bool pbLlamadasLocales;
        protected bool pbDestinatarioPrueba;
        protected int piCodSolicitud;
        protected DateTime pdtFecIni;
        protected DateTime pdtFecFin;
        protected int piAnioProceso;
        protected int piMesProceso;
        protected string psMesProceso;

        protected string pvchCodBanderas;
        protected string psTempPath;
        protected List<int> plstCorreosEnBlanco = new List<int>();
        protected Hashtable phtValoresCampos = null;

        protected AplicaReporteEspecial pAplicaReporte;
        protected Hashtable phtRelacionesReporte = new Hashtable();

        protected string psNomCampoDescEmple = "vchDescripcion";

        #endregion

        #region Constructor

        public ReporteEspecial()
        {
            psTempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString());
            DataTable ldtEmpleado = kdb.GetHisRegByEnt("Emple", "Empleados", "1=2");
            psNomCampoDescEmple = ldtEmpleado.Columns.Contains("{NomCompleto}") ? "{NomCompleto}" : "vchDescripcion";
        }

        protected virtual void InitVars()
        {
            getBanderas();

            piCodSolicitud = (int)getValCampo("iCodCatalogo", 0);

            psPathPlantillaBase = (string)getValCampo("{PathPlantillaBase}", "");
            psPathArchivos = (string)getValCampo("{PathArchivos}", "");
            psCC = (string)getValCampo("{CtaCC}", "");
            psCCO = (string)getValCampo("{CtaCCO}", "");
            psCtaRemitente = (string)getValCampo("{CtaDe}", "");
            psNomRemitente = (string)getValCampo("{NomRemitente}", "");
            psDestinatarioPrueba = (string)getValCampo("{DestPrueba}", "");
            piCodAsunto = (int)getValCampo("{Asunto}", 0);
            psCtaSoporte = (string)getValCampo("{CtaSoporte}", "");
            psPlantillaMail = (string)getValCampo("{Plantilla}", "");

            psIdioma = UtilAlarma.getIdioma((int)getValCampo("{Idioma}", 0));
            piCodCliente = (int)getValCampo("{Client}", 0);

            pbGenerarPlantillas = getValBandera("GenerarPlantillas", true);
            pbEnviarPlantillas = getValBandera("EnviarPlantillas", true);

            pbIncluirJerarquias = getValBandera("IncluirJerarquias");
            pbAdicionarUsuarioWEB = getValBandera("AdicionarUsuarioWEB");
            pbEnviarUltimoAcceso = getValBandera("EnviarUltimoAcceso");
            
            pbSinCtaEnviarAlReponsableCC = getValBandera("SinCtaEnviarAlReponsableCC");
            pbSinCtaEnviarASoporteInterno = getValBandera("SinCtaEnviarASoporteInterno");
            
            pbServicioMedido = getValBandera("ServicioMedido");
            pbLlamadasLocales = getValBandera("LlamadasLocales");

            pbDestinatarioPrueba = getValBandera("DestinatarioPrueba");

            pdtFecIni = (DateTime)getValCampo("{FechaIniRep}", getValCampo("{FechaInicio}", DateTime.MinValue));
            pdtFecFin = (DateTime)getValCampo("{FechaFinRep}", getValCampo("{FechaFin}", DateTime.MinValue));
            if (pdtFecFin.CompareTo(DateTime.MinValue) != 0)
            {
                pdtFecFin = pdtFecFin.AddHours(23).AddMinutes(59).AddSeconds(59);
            }
            piAnioProceso = int.Parse(getCodEntidad("Anio", "Años", (int)getValCampo("{Anio}", 0), "0"));
            piMesProceso = int.Parse(getCodEntidad("Mes", "Meses", (int)getValCampo("{Mes}", 0), "0"));
            psMesProceso = UtilAlarma.GetLangItem(psIdioma, "Mes", "Meses", piMesProceso.ToString());

            InitRelaciones();
        }

        protected virtual void InitRelaciones()
        {
            string REL_REPORTE_SITIOS = "Solicitud Reporte Especial - Sitio";
            string REL_REPORTE_CENCOS = "Solicitud Reporte Especial - Centro de Costos";
            string REL_REPORTE_EMPLEADOS = "Solicitud Reporte Especial - Empleado";
            string REL_REPORTE_VICEPRESIDENCIA = "Solicitud Reporte Especial - Vicepresidencia";
            string REL_REPORTE_TIPODESTINO = "Solicitud Reporte Especial - Tipo Destino";
            string REL_REPORTE_EMPRESAS = "Solicitud Reporte Especial - Empresa";
            string REL_REPORTE_EXTENSIONES = "Solicitud Reporte Especial - Extensiones";
            string REL_REPORTE_LOCALIDADES = "Solicitud Reporte Especial - Localidades";
            string REL_REPORTE_CARRIER = "Solicitud Reporte Especial - Carrier";
            string REL_REPORTE_CLAVECAR = "Solicitud Reporte Especial - Clave de Cargo";

            phtRelacionesReporte.Clear();
            if (pAplicaReporte.Sitio)
            {
                phtRelacionesReporte.Add("Sitio",
                    new RelacionReporteEspecial(REL_REPORTE_SITIOS,
                        piCodSolicitud,
                        "Sitio",
                        getValBandera("IncluirSitio"),
                        true,
                        getValBandera("ProcesarSitio", true),
                        getValBandera("EnviarSitio", true),
                        pAplicaReporte.IncluirJerarquias && pbIncluirJerarquias,
                        psIdioma,
                        CodUsuarioDB));
            }

            if (pAplicaReporte.CenCos)
            {
                phtRelacionesReporte.Add("CenCos",
                    new RelacionReporteEspecial(REL_REPORTE_CENCOS,
                        piCodSolicitud,
                        "CenCos",
                        getValBandera("IncluirCenCos"),
                        true,
                        getValBandera("ProcesarCenCos", true),
                        getValBandera("EnviarCenCos", true),
                        pAplicaReporte.IncluirJerarquias && pbIncluirJerarquias,
                        psIdioma,
                        CodUsuarioDB));
            }

            if (pAplicaReporte.Emple)
            {
                phtRelacionesReporte.Add("Emple",
                    new RelacionReporteEspecial(REL_REPORTE_EMPLEADOS,
                        piCodSolicitud,
                        "Emple",
                        getValBandera("IncluirEmple"),
                        true,
                        getValBandera("ProcesarEmple", true),
                        getValBandera("EnviarEmple", true),
                        false, //pAplicaReporte.IncluirJerarquias && pbIncluirJerarquias,
                        psIdioma,
                        CodUsuarioDB));
            }

            if (pAplicaReporte.Vicepre)
            {
                phtRelacionesReporte.Add("Vicepre",
                    new RelacionReporteEspecial(REL_REPORTE_VICEPRESIDENCIA,
                        piCodSolicitud,
                        "Vicepre",
                        getValBandera("IncluirVicepre"),
                        true,
                        getValBandera("ProcesarVicepre", true),
                        getValBandera("EnviarVicepre", true),
                        false, //pAplicaReporte.IncluirJerarquias && pbIncluirJerarquias,
                        psIdioma,
                        CodUsuarioDB));
            }

            if (pAplicaReporte.Empre)
            {
                phtRelacionesReporte.Add("Empre",
                    new RelacionReporteEspecial(REL_REPORTE_EMPRESAS,
                        piCodSolicitud,
                        "Empre",
                        getValBandera("IncluirEmpre"),
                        false,
                        getValBandera("ProcesarEmpre", true),
                        getValBandera("EnviarEmpre", true),
                        false, //IncluirJerarquias
                        psIdioma,
                        CodUsuarioDB));
            }

            if (pAplicaReporte.TDest)
            {
                phtRelacionesReporte.Add("TDest",
                    new RelacionReporteEspecial(REL_REPORTE_TIPODESTINO,
                        piCodSolicitud,
                        "TDest",
                        getValBandera("IncluirTDest"),
                        false, //AplicaProcesarEnviar
                        false, //Procesar
                        false, //Enviar
                        false, //IncluirJerarquias
                        psIdioma,
                        CodUsuarioDB));
            }

            if (pAplicaReporte.Exten)
            {
                phtRelacionesReporte.Add("Exten",
                    new RelacionReporteEspecial(REL_REPORTE_EXTENSIONES,
                        piCodSolicitud,
                        "Exten",
                        getValBandera("IncluirExten"),
                        false, //AplicaProcesarEnviar
                        false, //Procesar
                        false, //Enviar
                        false, //IncluirJerarquias
                        psIdioma,
                        CodUsuarioDB));
            }

            if (pAplicaReporte.Locali)
            {
                phtRelacionesReporte.Add("Locali",
                    new RelacionReporteEspecial(REL_REPORTE_LOCALIDADES,
                        piCodSolicitud,
                        "Locali",
                        getValBandera("IncluirLocali"),
                        false, //AplicaProcesarEnviar
                        false, //Procesar
                        false, //Enviar
                        false, //IncluirJerarquias
                        psIdioma,
                        CodUsuarioDB));
            }

            if (pAplicaReporte.Carrier)
            {
                phtRelacionesReporte.Add("Carrier",
                    new RelacionReporteEspecial(REL_REPORTE_CARRIER,
                        piCodSolicitud,
                        "Carrier",
                        getValBandera("IncluirCarrier"),
                        false, //AplicaProcesarEnviar
                        false, //Procesar
                        false, //Enviar
                        false, //IncluirJerarquias
                        psIdioma,
                        CodUsuarioDB));
            }

            if (pAplicaReporte.ClaveCar)
            {
                phtRelacionesReporte.Add("ClaveCar",
                    new RelacionReporteEspecial(REL_REPORTE_CLAVECAR,
                        piCodSolicitud,
                        "ClaveCar",
                        getValBandera("IncluirClaveCar"),
                        false, //AplicaProcesarEnviar
                        false, //Procesar
                        false, //Enviar
                        false, //IncluirJerarquias
                        psIdioma,
                        CodUsuarioDB));
            }
        }

        protected void getBanderas()
        {
            int liCodBandera = (int)Util.IsDBNull(kdb.GetHisRegByEnt("Atrib", "Atributos", "vchCodigo = '" + pvchCodBanderas + "'").Rows[0]["iCodCatalogo"], 0);
            piBanderasReporte = (int)getValCampo("{" + pvchCodBanderas + "}", 0);
            pdtBanderasReporte = kdb.GetHisRegByEnt("Valores", "Valores", new string[] { "{Atrib}", "{Value}" }, "[{Atrib}] = " + liCodBandera);
        }

        public static string getCodEntidad(string lsEntidad, string lsMaestro, int liCodCatalogo)
        {
            return getCodEntidad(lsEntidad,lsMaestro,liCodCatalogo, "");
        }

        public static string getCodEntidad(string lsEntidad, string lsMaestro, int liCodCatalogo, string defaultValue)
        {
            KDBAccess kdb = new KDBAccess();
            string vchCodigo = defaultValue;
            DataTable ldt = kdb.GetHisRegByEnt(lsEntidad, lsMaestro, new string[] { "iCodCatalogo" }, "iCodCatalogo = " + liCodCatalogo);
            if (ldt.Rows.Count > 0)
            {
                vchCodigo = ldt.Rows[0]["vchCodigo"].ToString();
            }
            return vchCodigo;
        }

        protected virtual bool getValBandera(string vchCodigo)
        {
            return Alarma.getValBandera(pdtBanderasReporte, piBanderasReporte, vchCodigo, false);
        }

        protected virtual bool getValBandera(string vchCodigo, bool defaultValue)
        {
            return Alarma.getValBandera(pdtBanderasReporte, piBanderasReporte, vchCodigo, defaultValue);
        }

        protected Object getValCampo(string lsCampo, Object defaultValue)
        {
            if (pdrConf.Table.Columns.Contains(lsCampo))
                return Util.IsDBNull(pdrConf[lsCampo], defaultValue);
            return defaultValue;
        }

        #endregion

        public override void IniciarCarga()
        {
            GetConfiguracion();

            if (pdrConf == null)
            {
                Util.LogMessage("Error en Solicitud de Reporte Especial. Carga no Identificada.");
                return;
            }

            try
            {
                InitVars();
            }
            catch (Exception ex)
            {
                Util.LogException("No se pudo inicializar las propiedades de la Solicitud del Reporte Especial. " + Maestro, ex);
                throw ex;
            }

            Procesar();
        }

        protected void Procesar()
        {
            foreach (RelacionReporteEspecial loRelRptSpc in phtRelacionesReporte.Values)
            {
                if (loRelRptSpc.AplicaProcesarEnviar && (loRelRptSpc.Procesar || loRelRptSpc.Enviar))
                {
                    plstCorreosEnBlanco.Clear();
                    HashSet<int> lhsValues = getValues(loRelRptSpc);
                    List<int> lstValues = HashSetToList(lhsValues);
                    for (int i = 0; i < lstValues.Count; i++ )
                    {
                        bool lbRet = true;
                        int liCodCatalogo = lstValues[i];
                        loRelRptSpc.iCodCatalogo = liCodCatalogo;
                        string lsPlantilla = getFileName(psPathArchivos, loRelRptSpc, ".xlsx");
                        if (pbGenerarPlantillas && loRelRptSpc.Procesar)
                        {
                            GenerarPlantilla(loRelRptSpc, lsPlantilla);
                        }
                        if (pbEnviarPlantillas && loRelRptSpc.Enviar && System.IO.File.Exists(lsPlantilla))
                        {
                            lbRet = EnviarCorreo(loRelRptSpc, lsPlantilla);
                        }
                        if (lbRet)
                        {
                            loRelRptSpc.Actualizar(loRelRptSpc.iCodCatalogo);
                        }
                    }
                    EnviarNotificacionCorreosEnBlanco(loRelRptSpc);
                }
            }
            ActualizarEstCarga();
        }

        private List<int> HashSetToList(HashSet<int> lhsValues)
        {
            List<int> lstLista = new List<int>();
            foreach (int liValue in lhsValues)
            {
                lstLista.Add(liValue);
            }
            return lstLista;
        }

        private HashSet<int> getValues(RelacionReporteEspecial loRelRptSpc)
        {
            HashSet<int> lhsValues = new HashSet<int>();
            if (loRelRptSpc.Entidad == "Emple")
            {
                if (phtRelacionesReporte.ContainsKey("CenCos") && getValBandera("EmpleadosPorCenCos",true))
                {
                    lhsValues = loRelRptSpc.getValues((RelacionReporteEspecial)phtRelacionesReporte["CenCos"]);
                }
                else if (phtRelacionesReporte.ContainsKey("Vicepre"))
                {
                    lhsValues = loRelRptSpc.getValues((RelacionReporteEspecial)phtRelacionesReporte["Vicepre"]);
                }
                else
                {
                    lhsValues = loRelRptSpc.getValues();
                }
            }
            else
            {
                lhsValues = loRelRptSpc.getValues();
            }
            return lhsValues;
        }

        #region Plantillas

        protected void GenerarPlantilla(RelacionReporteEspecial loRelRptSpc, string lsPathPlantilla)
        {
            EvaluadorMetaTags eval = new EvaluadorMetaTags();
            eval.Params = getParamsPlantilla(loRelRptSpc);
            eval.PathEntrada = psPathPlantillaBase;
            eval.PathSalida = lsPathPlantilla;
            eval.Idioma = psIdioma;
            eval.Procesar();
        }

        protected Hashtable getParamsPlantilla(RelacionReporteEspecial loRelRptSpc)
        {
            Hashtable Params = new Hashtable();
            Params.Add("Schema", DSODataContext.Schema);
            Params.Add("Cargas", piCodSolicitud);

            Params.Add("iCodUsuario", pdrConf["iCodUsuario"].ToString());
            Params.Add("iCodPerfil", DSODataAccess.ExecuteScalar("Select top 1 Perfil = IsNull(Perfil, 0) from [" + DSODataContext.Schema + "].[VisHistoricos('Usuar','Español')] where iCodCatalogo = " + pdrConf["iCodUsuario"].ToString()));

            foreach (DataColumn dc in pdrConf.Table.Columns)
            {
                if (dc.ColumnName.StartsWith("{"))
                {
                    string lsParam = dc.ColumnName.Substring(0, dc.ColumnName.Length - 1).Substring(1);
                    if (pdrConf[dc] is DateTime)
                    {
                        if (dc.ColumnName == "{FechaFinRep}")
                        {
                            Params.Add(lsParam, pdtFecFin.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        else
                        {
                            Params.Add(lsParam, ((DateTime)pdrConf[dc]).ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                    }
                    else
                    {
                        Params.Add(lsParam, pdrConf[dc].ToString());
                    }
                }
            }

            if (Params.ContainsKey("Anio"))
            {
                Params["Anio"] = piAnioProceso;
            }

            if (Params.ContainsKey("Mes"))
            {
                Params["Mes"] = piMesProceso;
            }
            foreach (DataRow dr in pdtBanderasReporte.Rows)
            {
                Params.Add(dr["vchCodigo"].ToString(), getValBandera(dr["vchCodigo"].ToString(), false) ? "1" : "0");
            }

            foreach (RelacionReporteEspecial rel in phtRelacionesReporte.Values)
            {
                if (loRelRptSpc == rel)
                {
                    if (Params.ContainsKey(rel.Entidad))
                    {
                        Params[rel.Entidad] = rel.iCodCatalogo;
                    }
                    else
                    {
                        Params.Add(rel.Entidad, rel.iCodCatalogo);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(rel.StringValues))
                    {
                        getValues(rel);
                    }
                    if (Params.ContainsKey(rel.Entidad))
                    {
                        Params[rel.Entidad] = rel.StringValues;
                    }
                    else
                    {
                        Params.Add(rel.Entidad, rel.StringValues);
                    }
                }
            }
            return Params;
        }

        protected Hashtable getParamsCorreo(RelacionReporteEspecial loRelRptSpc)
        {
            Hashtable Params = new Hashtable();
            Params.Add("Cargas", pdrConf["vchCodigo"].ToString());
            string lsDateFormat = UtilAlarma.GetLangItem(psIdioma, "MsgWeb", "Mensajes Web", "NetDateFormat");
            foreach (DataColumn dc in pdrConf.Table.Columns)
            {
                if (dc.ColumnName.StartsWith("{"))
                {
                    string lsParam = dc.ColumnName.Substring(0, dc.ColumnName.Length - 1).Substring(1);
                    if (pdrConf[dc] is DateTime)
                    {
                        if (dc.ColumnName == "{FechaFinRep}")
                        {
                            Params.Add(lsParam, pdtFecFin.ToString(lsDateFormat));
                        }
                        else
                        {
                            Params.Add(lsParam, ((DateTime)pdrConf[dc]).ToString(lsDateFormat));
                        }
                    }
                    else
                    {
                        Params.Add(lsParam, pdrConf[dc].ToString());
                    }
                }
            }

            if (Params.ContainsKey("Anio"))
            {
                Params["Anio"] = piAnioProceso;
            }

            if (Params.ContainsKey("Mes"))
            {
                Params["Mes"] = psMesProceso;
            }
            foreach (DataRow dr in pdtBanderasReporte.Rows)
            {
                Params.Add(dr["vchCodigo"].ToString(), getValBandera(dr["vchCodigo"].ToString()) ? GetLangItem("Si") : GetLangItem("No"));
            }

            Params.Add("ResponsableSitio", getResponsableSitio(loRelRptSpc));
            Params.Add("ResponsableCenCos", getResponsableCenCos(loRelRptSpc));

            if (pAplicaReporte.AdicionarUsuarioWEB)
            {
                if (pbAdicionarUsuarioWEB)
                {
                    Params.Add("UsrWeb", getUsrWeb(loRelRptSpc));
                }
                else
                {
                    Params.Add("UsrWeb", "");
                }
            }
            if (pAplicaReporte.EnviarUltimoAcceso)
            {
                if (pbEnviarUltimoAcceso)
                {
                    Params.Add("UltimoAcceso", getUltimoAcceso(loRelRptSpc));
                }
                else
                {
                    Params.Add("UltimoAcceso", "");
                }
            }
            if (!string.IsNullOrEmpty(psCtaSoporte))
            {
                Params.Add("SoporteInterno", getSoporteInterno());
            }

            foreach (RelacionReporteEspecial rel in phtRelacionesReporte.Values)
            {
                if (loRelRptSpc == rel)
                {
                    if (Params.ContainsKey(rel.Entidad))
                    {
                        Params[rel.Entidad] = rel.getDescripcion();
                    }
                    else
                    {
                        Params.Add(rel.Entidad, rel.getDescripcion());
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(rel.StringValues))
                    {
                        getValues(rel);
                    }
                    if (Params.ContainsKey(rel.Entidad))
                    {
                        Params[rel.Entidad] = rel.getDescripciones();
                    }
                    else
                    {
                        Params.Add(rel.Entidad, rel.getDescripciones());
                    }
                }
            }
            return Params;
        }

        protected string getResponsableSitio(RelacionReporteEspecial loRelRptSpc)
        {
            string lsResponsableSitio = "";
            if (loRelRptSpc.Entidad == "Sitio")
            {
                DataRow ldrEmpleado = getEmpleadoRel(loRelRptSpc);
                if (ldrEmpleado != null)
                {
                    lsResponsableSitio = ldrEmpleado[psNomCampoDescEmple].ToString();
                }
            }
            return lsResponsableSitio;
        }

        protected string getResponsableCenCos(RelacionReporteEspecial loRelRptSpc)
        {
            string lsSupervisor = "";
            DataRow ldrSupervisorCC = getSupervisorCC(loRelRptSpc);
            if (ldrSupervisorCC != null && !(ldrSupervisorCC[psNomCampoDescEmple] is DBNull))
            {
                lsSupervisor = ldrSupervisorCC[psNomCampoDescEmple].ToString();
            }

            return lsSupervisor;
        }
        
        #endregion

        protected bool EnviarCorreo(RelacionReporteEspecial loRelRptSpc, string lsPathPlantilla)
        {
            bool lbRet = false;
            string lsWordPath = buscarPlantilla();

            WordAccess loWord = new WordAccess();
            loWord.FilePath = lsWordPath;
            loWord.Abrir(true);

            if (pAplicaReporte.Client && piCodCliente > 0)
            {
                encabezadoCorreo(loWord, piCodCliente);
            }

            ReemplazarMetaTags(loWord, loRelRptSpc);

            string lsFileName = getFileName(psTempPath, loRelRptSpc, ".docx");
            loWord.FilePath = lsFileName;
            loWord.SalvarComo();
            loWord.Cerrar();
            loWord.Salir();
            loWord = null;

            poMail = new MailAccess();
            poMail.NotificarSiHayError = false;
            poMail.IsHtml = true;
            if (pAplicaReporte.SinCtaEnviarAlReponsableCC
                && pbSinCtaEnviarAlReponsableCC)
            {
                string lsCtaSupervisor = getCtaSupervisorCC(loRelRptSpc);
                if (!string.IsNullOrEmpty(lsCtaSupervisor))
                {
                    poMail.ReplyTo = new MailAddress(lsCtaSupervisor.Split(';')[0]);
                }
            }
            else if (pAplicaReporte.SinCtaEnviarASoporteInterno
                && pbSinCtaEnviarASoporteInterno
                && !string.IsNullOrEmpty(psCtaSoporte))
            {
                poMail.ReplyTo = new MailAddress(psCtaSoporte.Split(';')[0]);
            }
            poMail.De = getRemitente();
            poMail.Asunto = getAsunto(loRelRptSpc);
            poMail.Adjuntos.Add(new Attachment(lsPathPlantilla));
            poMail.AgregarWord(lsFileName);
            if (!string.IsNullOrEmpty(psDestinatarioPrueba) && pbDestinatarioPrueba)
            {
                poMail.Para.Add(psDestinatarioPrueba);
            }
            else
            {
                poMail.Para.Add(getDestinatario(loRelRptSpc));
                poMail.CC.Add(psCC);
                poMail.BCC.Add(psCCO);
            }
            try
            {
                poMail.Enviar();
            }
            catch (Exception e) { }
            if (!poMail.HayError)
            {
                lbRet = true;
            }
            else
            {
                lbRet = false;
                EnviarNotificacionCorreoNoValido(loRelRptSpc);
            }
            return lbRet;
        }

        private void encabezadoCorreo(WordAccess loWord, int piCodCliente)
        {
            string lsImgCte = "";
            string lsImgKeytia = "";
            DataTable ldtCte = kdb.GetHisRegByEnt("Client", "", "iCodCatalogo = " + piCodCliente);
            if (ldtCte != null && ldtCte.Rows.Count > 0)
            {
                DataRow ldrCte = ldtCte.Rows[0];
                if (!(ldrCte["{StyleSheet}"] is DBNull))
                {
                    lsImgKeytia = ldrCte["{StyleSheet}"].ToString();
                    lsImgKeytia = lsImgKeytia.Replace("~", Util.AppSettings("KeytiaWebFPath"));
                    lsImgKeytia = System.IO.Path.Combine(lsImgKeytia, @"images\KeytiaHeader.png");
                    if (System.IO.File.Exists(lsImgKeytia))
                    {
                        loWord.InsertarImagen(lsImgKeytia);
                    }
                }
                if (!(ldrCte["{Logo}"] is DBNull))
                {
                    lsImgCte = ldrCte["{Logo}"].ToString();
                    lsImgCte = lsImgCte.Replace("~", Util.AppSettings("KeytiaWebFPath"));
                    if (System.IO.File.Exists(lsImgCte))
                    {
                        loWord.InsertarImagen(lsImgCte);
                    }
                }

                loWord.NuevoParrafo();
                loWord.NuevoParrafo();
            }
        }

        protected void ReemplazarMetaTags(WordAccess loWord, RelacionReporteEspecial loRelRptSpc)
        {
            Hashtable Param = getParamsCorreo(loRelRptSpc);
            foreach (string lsKey in Param.Keys)
            {
                string lsMetaTag = string.Format("Param({0})", lsKey);
                string lsParam = Param[lsKey].ToString();
                lsParam = lsParam.Length > 255 ? lsParam.Substring(0, 255) : lsParam;
                bool lbReemplazo = loWord.ReemplazarTexto(lsMetaTag, lsParam);
                if (!lbReemplazo && (
                    lsKey == "UsrWeb" ||
                    lsKey == "UltimoAcceso" ||
                    lsKey == "SoporteInterno"))
                {
                    loWord.NuevoParrafo();
                    loWord.InsertarTexto(Param[lsKey].ToString());
                }
            }
        }
        
        #region Notificaciones

        protected void EnviarNotificacionCorreoNoValido(RelacionReporteEspecial loRelRptSpc)
        {
            string lsDestinatario = "";
            if (pAplicaReporte.SinCtaEnviarAlReponsableCC && pbSinCtaEnviarAlReponsableCC)
            {
                lsDestinatario= getCtaSupervisorCC(loRelRptSpc);
            }
            else if (pAplicaReporte.SinCtaEnviarASoporteInterno && pbSinCtaEnviarASoporteInterno)
            {
                lsDestinatario= psCtaSoporte;
            }

            if (string.IsNullOrEmpty(lsDestinatario)) return;

            DataRow ldrEmpleado = getEmpleadoRel(loRelRptSpc);
            string[] lsParam = new string[] { 
                                                (string)Util.IsDBNull(ldrEmpleado["{Email}"], ""), 
                                                (string)Util.IsDBNull(ldrEmpleado[psNomCampoDescEmple], "") 
                                            };
            string lsErrMailAsunto = GetLangItem("ErrMailAsuntoRepEsp"); // "Error de envío de correo automático";
            string lsMensaje = GetLangItem("ErrMailMensajeRepEsp", lsParam);

            if (lsErrMailAsunto.StartsWith("#undefined-"))
            {
                lsErrMailAsunto = "Error de envío de correo automático. Reporte Especial";
            }
            
            if (lsMensaje.StartsWith("#undefined-"))
            {
                lsMensaje = "Surgió un error durante el envío de correo automático\r\nPara: {0}\r\nEmpleado: {1}";
                lsMensaje = string.Format(lsMensaje, lsParam);
            }

            WordAccess loWord = new WordAccess();
            loWord.Abrir(true);
            UtilAlarma.encabezadoCorreo(loWord, (int)ldrEmpleado["iCodCatalogo"]);
            foreach (string lsLinea in lsMensaje.Split(new string[] { "\\r\\n" }, StringSplitOptions.None))
            {
                loWord.NuevoParrafo();
                loWord.InsertarTexto(lsLinea);
            }

            string lsFileName = getFileName(psTempPath, loRelRptSpc, "_NoValido.docx");
            loWord.FilePath = lsFileName;
            loWord.SalvarComo();
            loWord.Cerrar();
            loWord.Salir();
            loWord = null;

            MailAccess loMail = new MailAccess();
            loMail.NotificarSiHayError = false;
            loMail.IsHtml = true;
            loMail.De = getRemitente();
            loMail.Asunto = lsErrMailAsunto + ": " + getAsunto(loRelRptSpc);
            loMail.AgregarWord(lsFileName);
            loMail.Para.Add(lsDestinatario);
            loMail.EnviarAsincrono(loRelRptSpc);
        }

        protected void EnviarNotificacionCorreosEnBlanco(RelacionReporteEspecial loRelRptSpc)
        {
            string lsDestinatario = "";
            if (pAplicaReporte.SinCtaEnviarAlReponsableCC && pbSinCtaEnviarAlReponsableCC)
            {
                lsDestinatario = getCtaSupervisorCC(loRelRptSpc);
            }
            else if (pAplicaReporte.SinCtaEnviarASoporteInterno && pbSinCtaEnviarASoporteInterno)
            {
                lsDestinatario = psCtaSoporte;
            }

            if (plstCorreosEnBlanco.Count == 0 || string.IsNullOrEmpty(lsDestinatario)) return;

            StringBuilder lstEmpleados = new StringBuilder();
            DataTable ldtEmpleados = kdb.GetHisRegByEnt("Emple", "Empleados", "iCodCatalogo in (" + UtilAlarma.ToStringList(plstCorreosEnBlanco) + ")");
            foreach (DataRow ldrEmpleado in ldtEmpleados.Rows)
            {
                lstEmpleados.Append(ldrEmpleado[psNomCampoDescEmple].ToString() + "\r\n");
            }
            string lsNullMailAsunto = GetLangItem("NullMailAsuntoRepEsp"); // "Error de envío de correo automático";
            string lsMensaje = GetLangItem("NullMailMensajeRepEsp", lstEmpleados);
            if (lsNullMailAsunto.StartsWith("#undefined-"))
            {
                lsNullMailAsunto = "Notificación de Cuentas en Blanco. Reporte Especial";
            }
            if (lsMensaje.StartsWith("#undefined-"))
            {
                lsMensaje = "Se le notifica que los siguientes empleados no han recibido el correo automático debido a que no cuentan con la dirección de correo configurada:\r\n{0}";
                lsMensaje = string.Format(lsMensaje, lstEmpleados);
            }

            WordAccess loWord = new WordAccess();
            //loWord.FilePath = lsWordPath;
            loWord.Abrir(true);
            DataRow ldr = getEmpleadoRel(loRelRptSpc);
            if (ldr != null)
            {
                UtilAlarma.encabezadoCorreo(loWord, (int)Util.IsDBNull(ldr["{Emple}"], 0));
            }
            foreach (string lsLinea in lsMensaje.Split(new string[] { "\\r\\n" }, StringSplitOptions.None))
            {
                loWord.NuevoParrafo();
                loWord.InsertarTexto(lsLinea);
            }

            string lsFileName = getFileName(psTempPath, loRelRptSpc, "_EnBlanco.docx");
            loWord.FilePath = lsFileName;
            loWord.SalvarComo();
            loWord.Cerrar();
            loWord.Salir();
            loWord = null;

            MailAccess loMail = new MailAccess();
            loMail.NotificarSiHayError = false;
            loMail.IsHtml = true;
            loMail.De = getRemitente();
            loMail.Asunto = lsNullMailAsunto + ": " + getAsunto(loRelRptSpc);
            loMail.AgregarWord(lsFileName);
            loMail.Para.Add(lsDestinatario);

            loMail.EnviarAsincrono(loRelRptSpc);
        }

        #endregion
        
        protected string getAsunto(RelacionReporteEspecial loRelRptSpc) {
            string lsAsunto = UtilAlarma.GetLangItem(psIdioma, "Asunto", "Asunto de Correo Electrónico",
                getCodEntidad("Asunto", "Asunto de Correo Electrónico", piCodAsunto));

            StringBuilder lsbAsunto = new StringBuilder(lsAsunto);
            Hashtable Param = getParamsCorreo(loRelRptSpc);
            foreach (string lsKey in Param.Keys)
            {
                lsbAsunto = lsbAsunto.Replace("Param(" + lsKey + ")", Param[lsKey].ToString());
            }
            
            return lsbAsunto.ToString();
        }

        protected MailAddress getRemitente()
        {
            if (string.IsNullOrEmpty(psCtaRemitente))
            {
                return new MailAddress(Util.AppSettings("appeMailID"));
            }
            else if (string.IsNullOrEmpty(psNomRemitente))
            {
                return new MailAddress(psCtaRemitente);
            }
            else
            {
                return new MailAddress(psCtaRemitente, psNomRemitente);
            }
        }

        protected string getFileName(string lsPath, RelacionReporteEspecial loRelRptSpc, string lsExtension)
        {
            string lsFileName = string.Format("Cargas_{0}_{1}_{2}_({3}){4}" ,
                getCodEntidad("Cargas", Maestro, loRelRptSpc.iCodSolicitud),
                loRelRptSpc.Entidad,
                getCodEntidad(loRelRptSpc.Entidad, "", loRelRptSpc.iCodCatalogo),
                loRelRptSpc.iCodCatalogo,
                lsExtension);

            System.IO.Directory.CreateDirectory(lsPath);
            return System.IO.Path.GetFullPath(System.IO.Path.Combine(lsPath, lsFileName.Replace(" ", "_")));
        }

        protected string buscarPlantilla()
        {
            string lsPlantilla = psPlantillaMail;
            string lsFilePath = lsPlantilla;
            if (System.IO.File.Exists(lsFilePath))
                return lsFilePath;

            lsFilePath = System.IO.Path.Combine(lsFilePath, psIdioma);
            if (System.IO.Directory.Exists(lsFilePath))
            {
                foreach (string lsFile in System.IO.Directory.GetFiles(lsFilePath))
                {
                    if (lsFile.Length > 0 && (lsFile.EndsWith(".docx") || lsFile.EndsWith(".doc")))
                    {
                        return lsFile;
                    }
                }
            }
            lsFilePath = lsPlantilla;
            if (System.IO.Directory.Exists(lsFilePath))
            {
                foreach (string lsFile in System.IO.Directory.GetFiles(lsFilePath))
                {
                    if (lsFile.Length > 0 && (lsFile.EndsWith(".docx") || lsFile.EndsWith(".doc")))
                    {
                        return lsFile;
                    }
                }
            }
            return "";
        }

        protected void ActualizarEstCarga()
        {
            if (phtValoresCampos == null)
            {
                phtValoresCampos = new Hashtable();
                phtValoresCampos.Add("{EstCarga}", GetEstatusCarga("CarFinal"));
            }
            KeytiaCOM.CargasCOM cargasCOM = new KeytiaCOM.CargasCOM();
            kdb.Update("Historicos", "Cargas", Maestro, phtValoresCampos, (int)pdrConf["iCodRegistro"]);
        }

        protected DataRow getEmpleadoRel(RelacionReporteEspecial loRelRptSpc)
        {
            DataRow ldrEmpleado = null;
            DataTable ldtEmpleado = null;
            if (loRelRptSpc.AplicaProcesarEnviar)
            {
                switch (loRelRptSpc.Entidad)
                {
                    case "Emple":
                        ldtEmpleado = kdb.GetHisRegByEnt("Emple", "Empleados", "iCodCatalogo = " + loRelRptSpc.iCodCatalogo);
                        break;
                    default:
                        DataTable ldtRel = kdb.GetHisRegByEnt(loRelRptSpc.Entidad, "", "iCodCatalogo = " + loRelRptSpc.iCodCatalogo);
                        if (ldtRel != null && ldtRel.Rows.Count > 0 && ldtRel.Columns.Contains("{Emple}"))
                        {
                            ldtEmpleado = kdb.GetHisRegByEnt("Emple", "Empleados", "iCodCatalogo = " + ldtRel.Rows[0]["{Emple}"].ToString());
                        }
                        break;
                }
            }
            if (ldtEmpleado != null && ldtEmpleado.Rows.Count > 0)
            {
                ldrEmpleado = ldtEmpleado.Rows[0];
            }
            return ldrEmpleado;
        }

        protected DataRow getUsuarioRel(RelacionReporteEspecial loRelRptSpc)
        {
            DataRow ldrUsuario = null;
            DataRow ldrEmpleado = getEmpleadoRel(loRelRptSpc);
            if (ldrEmpleado != null && !(ldrEmpleado["{Usuar}"] is DBNull))
            {
                DataTable ldtUsuario = kdb.GetHisRegByEnt("Usuar", "Usuarios", "iCodCatalogo = " + ldrEmpleado["{Usuar}"].ToString());
                if (ldtUsuario != null && ldtUsuario.Rows.Count > 0)
                {
                    ldrUsuario = ldtUsuario.Rows[0];
                }
            }
            return ldrUsuario;
        }

        protected string getUsrWeb(RelacionReporteEspecial loRelRptSpc)
        {
            string lsUsrWeb = "";
            DataRow ldrUsuario = null;
            ldrUsuario = getUsuarioRel(loRelRptSpc);
            if (ldrUsuario != null)
            {
                lsUsrWeb = GetLangItem("UsrWeb", ldrUsuario["vchCodigo"].ToString());
            }
            return lsUsrWeb;
        }

        protected string getUltimoAcceso(RelacionReporteEspecial loRelRptSpc)
        {
            DateTime ldtUltAcceso = new DateTime(2011, 1, 1);
            string lsDateFormat = UtilAlarma.GetLangItem(psIdioma, "MsgWeb", "Mensajes Web", "NetDateTimeFormat");
            DataRow ldrUsuario = getUsuarioRel(loRelRptSpc);
            if (ldrUsuario != null && !(ldrUsuario["{UltAcc}"] is DBNull))
            {
                ldtUltAcceso = (DateTime)ldrUsuario["{UltAcc}"];
            }
            return GetLangItem("UltimoAccesoRE", ldtUltAcceso.ToString(lsDateFormat));
        }

        protected string getSoporteInterno()
        {
            return GetLangItem("SoporteInternoRE", psCtaSoporte);
        }

        protected string getDestinatario(RelacionReporteEspecial loRelRptSpc)
        {
            string lsEMail = "";
            DataRow ldrEmpleado = getEmpleadoRel(loRelRptSpc);
            if (ldrEmpleado != null && !(ldrEmpleado["{Email}"] is DBNull))
            {
                lsEMail = ldrEmpleado["{Email}"].ToString();
            }

            if (string.IsNullOrEmpty(lsEMail))
            {
                if (pAplicaReporte.SinCtaEnviarAlReponsableCC &&  pbSinCtaEnviarAlReponsableCC)
                {
                    lsEMail = getCtaSupervisorCC(loRelRptSpc);
                }
                else if (pAplicaReporte.SinCtaEnviarASoporteInterno && pbSinCtaEnviarASoporteInterno && !string.IsNullOrEmpty(psCtaSoporte))
                {
                    lsEMail = psCtaSoporte;
                }
                plstCorreosEnBlanco.Add(loRelRptSpc.iCodCatalogo);
            }

            return lsEMail;
        }

        protected DataRow getSupervisorCC(RelacionReporteEspecial loRelRptSpc)
        {
            int liCodSupervisorCC = 0;
            DataRow ldrSupervisorCC = null;
            DataRow ldrEmpleado = getEmpleadoRel(loRelRptSpc);
            if (ldrEmpleado != null)
            {
                if (ldrEmpleado.Table.Columns.Contains("{Emple}") && !(ldrEmpleado["{Emple}"] is DBNull))
                {
                    liCodSupervisorCC = (int)ldrEmpleado["{Emple}"];
                }
                else if (ldrEmpleado.Table.Columns.Contains("{CenCos}") && !(ldrEmpleado["{CenCos}"] is DBNull))
                {
                    DataTable ldt = kdb.GetHisRegByEnt("CenCos", "",
                        new string[] { "{Emple}" },
                        "iCodCatalogo = " + ldrEmpleado["{CenCos}"].ToString());
                    if (ldt != null && ldt.Rows.Count > 0 && !(ldt.Rows[0]["{Emple}"] is DBNull))
                    {
                        liCodSupervisorCC = (int)ldt.Rows[0]["{Emple}"];
                    }
                }
            }
            if (liCodSupervisorCC > 0)
            {
                DataTable ldt = kdb.GetHisRegByEnt("Emple", "Empleados",
                                    "iCodCatalogo = " + liCodSupervisorCC);
                if (ldt != null && ldt.Rows.Count > 0)
                {
                    ldrSupervisorCC = ldt.Rows[0];
                }
            }

            return ldrSupervisorCC;
        }

        protected string getCtaSupervisorCC(RelacionReporteEspecial loRelRptSpc)
        {
            string lsSupervisor = "";
            DataRow ldrSupervisorCC = getSupervisorCC(loRelRptSpc);
            if (ldrSupervisorCC != null && !(ldrSupervisorCC["{Email}"] is DBNull))
            {
                    lsSupervisor = ldrSupervisorCC["{Email}"].ToString();
            }

            return lsSupervisor;
        }

        #region Idioma
        
        protected string GetLangItem(string lsElemento, params object[] lsParam)
        {
            return UtilAlarma.GetLangItem(psIdioma, "MsgWeb", "Mensajes Reporte Especial", lsElemento, lsParam);
        }
        
        #endregion
    }
}
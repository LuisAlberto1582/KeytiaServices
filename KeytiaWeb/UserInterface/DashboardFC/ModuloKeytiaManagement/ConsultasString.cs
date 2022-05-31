using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace KeytiaWeb.UserInterface.DashboardFC.ModuloKeytiaManagement
{
    public class ConsultasString
    {
        public static string CencosQuery()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT");
            query.AppendLine("iCodCatalogo AS [ID CenCos],");
            query.AppendLine("vchCodigo AS [Código CenCos],");
            query.AppendLine("RTRIM(LTRIM(SUBSTRING(vchDescripcion,0,CHARINDEX('(', vchDescripcion, 1)))) AS [Descripción CenCos],");
            query.AppendLine("CenCos AS [ID CenCos Padre],");
            query.AppendLine("CenCosCod AS [Código CenCos Padre],");
            query.AppendLine("RTRIM(LTRIM(SUBSTRING(CenCosDesc,0,CHARINDEX('(',CenCosDesc,1)))) AS [Descripción CenCos Padre]");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Cencos','Centro de Costos','Español')]");
            query.AppendLine("WHERE dtIniVigencia <> dtFinVigencia AND dtFinVigencia >= GETDATE()");
            query.AppendLine("ORDER BY iCodCatalogo");
            return query.ToString();
        }
        public static string EmpleQuery()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC DBModel_EmpleGetActivos @Esquema = '" + DSODataContext.Schema + "'");
            return query.ToString();
        }
        public static string CodAutQuery()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC DBModel_CodAutoGetActivos @Esquema = '" + DSODataContext.Schema + "'");
            return query.ToString();
        }
        public static string ExtensionesActivasQuery()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC DBModel_ExtenGetActivas @Esquema = '" + DSODataContext.Schema + "'");
            return query.ToString();
        }
        public static string LineasQuery()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("EXEC DBModel_LineaGetActivas @Esquema = '" + DSODataContext.Schema + "'");
            return query.ToString();
        }
        public static string SitioQuery()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT ICodRegistro, ");
            query.AppendLine("	ICodCatalogo,");
            query.AppendLine("	ICodMaestro,");
            query.AppendLine("	VchCodigo,");
            query.AppendLine("	VchDescripcion,");
            query.AppendLine("	ICodEntidad,");
            query.AppendLine("	VchDesMaestro,");
            query.AppendLine("	Empre,");
            query.AppendLine("	EmpreCod,");
            query.AppendLine("	EmpreDesc,");
            query.AppendLine("	Locali,");
            query.AppendLine("	LocaliCod,");
            query.AppendLine("	LocaliDesc,");
            query.AppendLine("	TipoSitio,");
            query.AppendLine("	TipoSitioCod,");
            query.AppendLine("	TipoSitioDesc,");
            query.AppendLine("	MarcaSitio,");
            query.AppendLine("	MarcaSitioCod,");
            query.AppendLine("	MarcaSitioDesc,");
            query.AppendLine("	Emple,");
            query.AppendLine("	EmpleCod,");
            query.AppendLine("	EmpleDesc,");
            query.AppendLine("	BanderasSitio,");
            query.AppendLine("	LongExt,");
            query.AppendLine("	ExtIni,");
            query.AppendLine("	ExtFin,");
            query.AppendLine("	DtIniVigencia,");
            query.AppendLine("	DtFinVigencia,");
            query.AppendLine("	ICodUsuario,");
            query.AppendLine("	DtFecUltAct");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".[VisHisComun('Sitio','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia ");
            query.AppendLine(" AND dtFinVigencia >= GETDATE() ");
            return query.ToString();
        }
        public static string TipoEmQuery()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT ICodRegistro, ");
            query.AppendLine("     ICodCatalogo, ");
            query.AppendLine("     ICodMaestro, ");
            query.AppendLine("     VchCodigo, ");
            query.AppendLine("     VchDescripcion, ");
            query.AppendLine("     DtIniVigencia, ");
            query.AppendLine("     DtFinVigencia, ");
            query.AppendLine("     ICodUsuario, ");
            query.AppendLine("     DtFecUltAct ");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".[VisHistoricos('TipoEm','Tipo Empleado','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia ");
            query.AppendLine(" and dtFinVigencia >= GETDATE() ");
            return query.ToString();
        }
        public static string PuestoQuery()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT ICodRegistro, ");
            query.AppendLine("     ICodCatalogo, ");
            query.AppendLine("     ICodMaestro, ");
            query.AppendLine("     VchCodigo, ");
            query.AppendLine("     VchDescripcion, ");
            query.AppendLine("     DtIniVigencia, ");
            query.AppendLine("     DtFinVigencia, ");
            query.AppendLine("     ICodUsuario, ");
            query.AppendLine("     DtFecUltAct ");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".[VisHistoricos('Puesto','Puestos Empleado','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia ");
            query.AppendLine(" and dtFinVigencia >= GETDATE() ");

            return query.ToString();
        }
        public static string CosQuery()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT ICodRegistro, ");
            query.AppendLine("	ICodCatalogo,");
            query.AppendLine("	ICodMaestro,");
            query.AppendLine("	VchCodigo,");
            query.AppendLine("	VchDescripcion,");
            query.AppendLine("	MarcaSitio,");
            query.AppendLine("	DtIniVigencia,");
            query.AppendLine("	DtFinVigencia,");
            query.AppendLine("	ICodUsuario,");
            query.AppendLine("	DtFecUltAct");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".[VisHistoricos('Cos','Cos','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia ");
            query.AppendLine(" and dtFinVigencia >= GETDATE() ");

            return query.ToString();
        }
        public static string CarrierQuery()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT ICodRegistro, ");
            query.AppendLine("	ICodCatalogo,");
            query.AppendLine("	ICodMaestro,");
            query.AppendLine("	VchCodigo,");
            query.AppendLine("	VchDescripcion,");
            query.AppendLine("	DtIniVigencia,");
            query.AppendLine("	DtFinVigencia,");
            query.AppendLine("	ICodUsuario,");
            query.AppendLine("	DtFecUltAct");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Carrier','Carriers','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia ");
            query.AppendLine(" AND dtFinVigencia >= GETDATE() ");

            return query.ToString();
        }
        public static string CtaMaestraQuery()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT ICodRegistro, ");
            query.AppendLine("     ICodCatalogo, ");
            query.AppendLine("     ICodMaestro, ");
            query.AppendLine("     VchCodigo, ");
            query.AppendLine("     VchDescripcion, ");
            query.AppendLine("     Carrier, ");
            query.AppendLine("     CarrierCod, ");
            query.AppendLine("     CarrierDesc, ");
            query.AppendLine("     Empre, ");
            query.AppendLine("     EmpreCod, ");
            query.AppendLine("     EmpreDesc, ");
            query.AppendLine("     RazonSocial, ");
            query.AppendLine("     RazonSocialCod, ");
            query.AppendLine("     RazonSocialDesc, ");
            query.AppendLine("     DtIniVigencia, ");
            query.AppendLine("     DtFinVigencia, ");
            query.AppendLine("     ICodUsuario, ");
            query.AppendLine("     DtFecUltAct ");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".[VisHistoricos('CtaMaestra','Cuenta Maestra Carrier','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia ");
            query.AppendLine(" and dtFinVigencia >= GETDATE() ");

            return query.ToString();
        }
        public static string TipoPlanQuery()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT ICodRegistro, ");
            query.AppendLine("     ICodCatalogo, ");
            query.AppendLine("     ICodMaestro, ");
            query.AppendLine("     VchCodigo, ");
            query.AppendLine("     VchDescripcion, ");
            query.AppendLine("     DtIniVigencia, ");
            query.AppendLine("     DtFinVigencia, ");
            query.AppendLine("     ICodUsuario, ");
            query.AppendLine("     DtFecUltAct ");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".[VisHistoricos('TipoPlan','Tipo Plan','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia ");
            query.AppendLine(" and dtFinVigencia >= GETDATE() ");

            return query.ToString();
        }
        public static string EquipoCelularQuery()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT ICodRegistro, ");
            query.AppendLine("     ICodCatalogo, ");
            query.AppendLine("     ICodMaestro, ");
            query.AppendLine("     VchCodigo, ");
            query.AppendLine("     VchDescripcion, ");
            query.AppendLine("     Descripcion, ");
            query.AppendLine("     DtIniVigencia, ");
            query.AppendLine("     DtFinVigencia, ");
            query.AppendLine("     ICodUsuario, ");
            query.AppendLine("     DtFecUltAct ");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".[VisHistoricos('EqCelular','Equipo Celular','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia ");
            query.AppendLine(" and dtFinVigencia >= GETDATE() ");

            return query.ToString();
        }
        public static string PlanTarifarioQuery()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT ICodRegistro, ");
            query.AppendLine("     ICodCatalogo, ");
            query.AppendLine("     ICodMaestro, ");
            query.AppendLine("     VchCodigo, ");
            query.AppendLine("     VchDescripcion, ");
            query.AppendLine("     Carrier, ");
            query.AppendLine("     CarrierCod, ");
            query.AppendLine("     CarrierDesc, ");
            query.AppendLine("     MinutosMismoCarrier, ");
            query.AppendLine("     MinutosOtrosCarrier, ");
            query.AppendLine("     SMSIncluidos, ");
            query.AppendLine("     DatosMBIncluidos, ");
            query.AppendLine("     RentaTelefonia, ");
            query.AppendLine("     Descripcion, ");
            query.AppendLine("     DtIniVigencia, ");
            query.AppendLine("     DtFinVigencia, ");
            query.AppendLine("     ICodUsuario, ");
            query.AppendLine("     DtFecUltAct ");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".[VisHistoricos('PlanTarif','Plan Tarifario','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia ");
            query.AppendLine(" and dtFinVigencia >= GETDATE() ");

            return query.ToString();
        }
        public static string CodAutoSinEmpleQuery()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT CodAuto.ICodCatalogo, CodAuto.VchCodigo, CodAuto.VchDescripcion, Recurs, RecursCod, RecursDesc, Sitio, SitioCod, SitioDesc, Cos, CosCod, CosDesc,");
            query.AppendLine("	CodAuto.dtIniVigencia, CodAuto.dtFinVigencia");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('CodAuto','Codigo Autorizacion','Español')] CodAuto");
            query.AppendLine("	LEFT JOIN " + DSODataContext.Schema + ".[VisRelaciones('Empleado - CodAutorizacion','Español')] RelCodAuto");
            query.AppendLine("		ON RelCodAuto.CodAuto = CodAuto.iCodCatalogo");
            query.AppendLine("		AND RelCodAuto.dtIniVigencia <> RelCodAuto.dtFinVigencia");
            query.AppendLine("		AND RelCodAuto.dtFinVigencia >= GETDATE()");
            query.AppendLine("WHERE CodAuto.dtIniVigencia <> CodAuto.dtFinVigencia");
            query.AppendLine("	AND CodAuto.dtFinVigencia >= GETDATE()");
            query.AppendLine("	AND RelCodAuto.iCodRegistro IS NULL");

            return query.ToString();
        }
        public static string ExtenSinEmpleQuery()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT Exten.ICodCatalogo, Exten.VchCodigo, Exten.VchDescripcion, Recurs, RecursCod, RecursDesc, Sitio, SitioCod, SitioDesc, Cos, CosCod, CosDesc, ");
            query.AppendLine("	Masc, Exten.dtIniVigencia, Exten.dtFinVigencia");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Exten','Extensiones','Español')] Exten");
            query.AppendLine("	LEFT JOIN " + DSODataContext.Schema + ".[VisRelaciones('Empleado - Extension','Español')] RelExten");
            query.AppendLine("		ON RelExten.Exten = Exten.iCodCatalogo");
            query.AppendLine("		AND RelExten.dtIniVigencia <> RelExten.dtFinVigencia");
            query.AppendLine("		AND RelExten.dtFinVigencia >= GETDATE()");
            query.AppendLine("WHERE Exten.dtIniVigencia <> Exten.dtFinVigencia");
            query.AppendLine("	AND Exten.dtFinVigencia >= GETDATE()");
            query.AppendLine("	AND RelExten.iCodRegistro IS NULL");

            return query.ToString();
        }
        public static string LineasSinEmpleQuery()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT Linea.ICodCatalogo, Linea.VchCodigo, Linea.VchDescripcion, Recurs, RecursCod, RecursDesc, Sitio, SitioCod, SitioDesc,");
            query.AppendLine("		Carrier, CarrierCod, CarrierDesc, CtaMaestra, CtaMaestraCod, CtaMaestraDesc,");
            query.AppendLine("		RazonSocial, RazonSocialCod, RazonSocialDesc, TipoPlan, TipoPlanCod, TipoPlanDesc,");
            query.AppendLine("		EqCelular, EqCelularCod, EqCelularDesc, PlanTarif, PlanTarifCod, PlanTarifDesc,");
            query.AppendLine("		CargoFijo, FecLimite, FechaFinPlan, FechaDeActivacion, Etiqueta, Tel, PlanLineaFactura, ");
            query.AppendLine("		IMEI, ModeloCel, NumOrden, Linea.dtIniVigencia, Linea.dtFinVigencia ");
            query.AppendLine("FROM " + DSODataContext.Schema + ".[VisHistoricos('Linea','Lineas','Español')] Linea");
            query.AppendLine("	LEFT JOIN " + DSODataContext.Schema + ".[VisRelaciones('Empleado - Linea','Español')] RelLinea");
            query.AppendLine("		ON RelLinea.Linea = Linea.iCodCatalogo");
            query.AppendLine("		AND RelLinea.dtIniVigencia <> RelLinea.dtFinVigencia");
            query.AppendLine("		AND RelLinea.dtFinVigencia >= GETDATE()");
            query.AppendLine("WHERE Linea.dtIniVigencia <> Linea.dtFinVigencia");
            query.AppendLine("	AND Linea.dtFinVigencia >= GETDATE()");
            query.AppendLine("	AND RelLinea.iCodRegistro IS NULL");

            return query.ToString();
        }
        public static string OrganizacionQuery()
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT ICodRegistro, ");
            query.AppendLine("     ICodCatalogo, ");
            query.AppendLine("     ICodMaestro, ");
            query.AppendLine("     VchCodigo, ");
            query.AppendLine("     VchDescripcion, ");
            query.AppendLine("     CuentaContable, ");
            query.AppendLine("     EntidadContable, ");
            query.AppendLine("     DtIniVigencia, ");
            query.AppendLine("     DtFinVigencia, ");
            query.AppendLine("     ICodUsuario, ");
            query.AppendLine("     DtFecUltAct ");
            query.AppendLine(" FROM " + DSODataContext.Schema + ".[VisHistoricos('Organizacion','Organizacion','Español')]");
            query.AppendLine(" WHERE dtIniVigencia <> dtFinVigencia ");
            query.AppendLine(" AND dtFinVigencia >= GETDATE() ");

            return query.ToString();
        }
    }
}
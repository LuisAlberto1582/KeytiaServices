using KeytiaServiceBL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace KeytiaWeb.UserInterface.DashboardFC.ModuloKeytiaManagement
{
    public class ConsultasDb
    {
        public static DataTable CencosData()
        {
            DataTable dt = DSODataAccess.Execute(ConsultasString.CencosQuery());
            return dt;
        }
        public static DataTable EmpleActivosData()
        {
            DataTable dt = DSODataAccess.Execute(ConsultasString.EmpleQuery());
            return dt;
        }
        public static DataTable CodAutData()
        {
            DataTable dt = DSODataAccess.Execute(ConsultasString.CodAutQuery());
            return dt;
        }
        public static DataTable ExtensionesData()
        {
            DataTable dt = DSODataAccess.Execute(ConsultasString.ExtensionesActivasQuery());
            return dt;
        }
        public static DataTable LineasData()
        {
            DataTable dt = DSODataAccess.Execute(ConsultasString.LineasQuery());
            return dt;
        }
        public static DataTable SitioData()
        {
            DataTable dt = DSODataAccess.Execute(ConsultasString.SitioQuery());

            string[] columnasSeleccionar = { "iCodCatalogo", "vchCodigo", "vchDescripcion", "MarcaSitio", "MarcaSitioCod", "MarcaSitioDesc" };
            string[] columnasCambiosNombre = { "ID Sitio", "Clave Sitio", "Descripción Sitio", "ID Marca", "Clave Marca", "Descripción Marca" };

            DataView dataView = new DataView(dt);
            DataTable nuevaDt = dataView.ToTable(false, columnasSeleccionar);

            for (int i = 0; i < nuevaDt.Columns.Count; i++)
            {
                nuevaDt.Columns[i].ColumnName = columnasCambiosNombre[i];
            }

            nuevaDt.AcceptChanges();

            return nuevaDt;
        }
        public static DataTable TipoEmData()
        {
            DataTable dt = DSODataAccess.Execute(ConsultasString.TipoEmQuery());

            // Hacemos modificacion de los datos necesarios y nombres de columnas
            string[] columnasSeleccionar = { "iCodCatalogo", "vchCodigo", "vchDescripcion" };
            string[] columnasCambiosNombre = { "ID Tipo Empleado", "Clave Tipo Empleado", "Descripción Tipo Empleado" };

            DataView dataView = new DataView(dt);
            DataTable nuevaDt = dataView.ToTable(false, columnasSeleccionar);

            for (int i = 0; i < nuevaDt.Columns.Count; i++)
            {
                nuevaDt.Columns[i].ColumnName = columnasCambiosNombre[i];
            }

            nuevaDt.AcceptChanges();

            return nuevaDt;
        }
        public static DataTable PuestoData()
        {
            DataTable dt = DSODataAccess.Execute(ConsultasString.PuestoQuery());

            // Hacemos modificacion de los datos necesarios y nombres de columnas
            string[] columnasSeleccionar = { "iCodCatalogo", "vchCodigo", "vchDescripcion" };
            string[] columnasCambiosNombre = { "ID Puesto", "Clave Puesto", "Descripción Puesto" };

            DataView dataView = new DataView(dt);
            DataTable nuevaDt = dataView.ToTable(false, columnasSeleccionar);

            for (int i = 0; i < nuevaDt.Columns.Count; i++)
            {
                nuevaDt.Columns[i].ColumnName = columnasCambiosNombre[i];
            }

            nuevaDt.AcceptChanges();

            return nuevaDt;
        }
        public static DataTable CosData()
        {
            DataTable dt = DSODataAccess.Execute(ConsultasString.CosQuery());

            // Hacemos modificacion de los datos necesarios y nombres de columnas
            string[] columnasSeleccionar = { "iCodCatalogo", "vchCodigo", "vchDescripcion" };
            string[] columnasCambiosNombre = { "ID Cos", "Clave Cos", "Descripción Cos" };

            DataView dataView = new DataView(dt);
            DataTable nuevaDt = dataView.ToTable(false, columnasSeleccionar);

            for (int i = 0; i < nuevaDt.Columns.Count; i++)
            {
                nuevaDt.Columns[i].ColumnName = columnasCambiosNombre[i];
            }

            nuevaDt.AcceptChanges();

            return nuevaDt;
        }
        public static DataTable CarrierData()
        {
            DataTable dt = DSODataAccess.Execute(ConsultasString.CarrierQuery());

            // Hacemos modificacion de los datos necesarios y nombres de columnas
            string[] columnasSeleccionar = { "iCodCatalogo", "vchCodigo", "vchDescripcion" };
            string[] columnasCambiosNombre = { "ID Carrier", "Clave Carrier", "Descripción Carrier" };

            DataView dataView = new DataView(dt);
            DataTable nuevaDt = dataView.ToTable(false, columnasSeleccionar);

            for (int i = 0; i < nuevaDt.Columns.Count; i++)
            {
                nuevaDt.Columns[i].ColumnName = columnasCambiosNombre[i];
            }

            nuevaDt.AcceptChanges();

            return nuevaDt;
        }
        public static DataTable CuentaMaestraData()
        {
            DataTable dt = DSODataAccess.Execute(ConsultasString.CtaMaestraQuery());

            // Hacemos modificacion de los datos necesarios y nombres de columnas
            string[] columnasSeleccionar = { "iCodCatalogo", "vchCodigo", "vchDescripcion", "Carrier", "CarrierCod", "CarrierDesc",
                                "Empre", "EmpreCod", "EmpreDesc", "RazonSocial", "RazonSocialCod", "RazonSocialDesc" };
            string[] columnasCambiosNombre = { "ID Cuenta Maestra", "Clave Cuenta Maestra", "Descripción Cuenta Maestra", "ID Carrier", "Clave Carrier", "Descripción Carrier",
                                            "ID Empresa", "Clave Empresa", "Descripción Empresa", "ID Razón Social", "Clave Razón Social", "Descripción Razón Social"};

            DataView dataView = new DataView(dt);
            DataTable nuevaDt = dataView.ToTable(false, columnasSeleccionar);

            for (int i = 0; i < nuevaDt.Columns.Count; i++)
            {
                nuevaDt.Columns[i].ColumnName = columnasCambiosNombre[i];
            }

            nuevaDt.AcceptChanges();

            return nuevaDt;
        }
        public static DataTable TipoPlanData()
        {
            DataTable dt = DSODataAccess.Execute(ConsultasString.TipoPlanQuery());

            // Hacemos modificacion de los datos necesarios y nombres de columnas
            string[] columnasSeleccionar = { "iCodCatalogo", "vchCodigo", "vchDescripcion" };
            string[] columnasCambiosNombre = { "ID Tipo Plan", "Clave Tipo Plan", "Descripción Tipo Plan" };

            DataView dataView = new DataView(dt);
            DataTable nuevaDt = dataView.ToTable(false, columnasSeleccionar);

            for (int i = 0; i < nuevaDt.Columns.Count; i++)
            {
                nuevaDt.Columns[i].ColumnName = columnasCambiosNombre[i];
            }

            nuevaDt.AcceptChanges();

            return nuevaDt;
        }
        public static DataTable EquipoCelularData()
        {
            DataTable dt = DSODataAccess.Execute(ConsultasString.EquipoCelularQuery());

            // Hacemos modificacion de los datos necesarios y nombres de columnas
            string[] columnasSeleccionar = { "iCodCatalogo", "vchCodigo", "vchDescripcion" };
            string[] columnasCambiosNombre = { "ID Equipo Celular", "Clave Equipo Celular", "Descripción Equipo Celular" };

            DataView dataView = new DataView(dt);
            DataTable nuevaDt = dataView.ToTable(false, columnasSeleccionar);

            for (int i = 0; i < nuevaDt.Columns.Count; i++)
            {
                nuevaDt.Columns[i].ColumnName = columnasCambiosNombre[i];
            }

            nuevaDt.AcceptChanges();

            return nuevaDt;
        }
        public static DataTable PlanTarifarioData()
        {
            DataTable dt = DSODataAccess.Execute(ConsultasString.PlanTarifarioQuery());

            // Hacemos modificacion de los datos necesarios y nombres de columnas
            string[] columnasSeleccionar = { "iCodCatalogo", "vchCodigo", "vchDescripcion", "Carrier", "CarrierCod", "CarrierDesc",
                                    "MinutosMismoCarrier", "MinutosOtrosCarrier", "SMSIncluidos", "DatosMBIncluidos", "RentaTelefonia" };
            string[] columnasCambiosNombre = { "ID Plan Tarifario", "Clave Plan Tarifario", "Descripción Plan Tarifario", "ID Carrier", "Clave Carrier", "Descripción Carrier",
                                    "Minutos Mismo Carrier", "Minutos Otros Carrier", "SMS Incluidos", "Datos MB Incluidos", "Renta" };

            DataView dataView = new DataView(dt);
            DataTable nuevaDt = dataView.ToTable(false, columnasSeleccionar);

            for (int i = 0; i < nuevaDt.Columns.Count; i++)
            {
                nuevaDt.Columns[i].ColumnName = columnasCambiosNombre[i];
            }

            nuevaDt.AcceptChanges();

            return nuevaDt;
        }
        public static DataTable CodAutoSinEmpleData()
        {
            DataTable dt = DSODataAccess.Execute(ConsultasString.CodAutoSinEmpleQuery());

            // Hacemos modificacion de los datos necesarios y nombres de columnas
            string[] columnasSeleccionar = { "ICodCatalogo", "VchCodigo", "VchDescripcion",
                             "Recurs", "RecursCod", "RecursDesc", "Sitio", "SitioCod", "SitioDesc", "Cos", "CosCod", "CosDesc", "dtIniVigencia", "dtFinVigencia" };
            string[] columnasCambiosNombre = { "ID Codigo", "Clave Codigo", "Descripción Codigo", "ID Recurso", "Clave Recurso", "Descripción Recurso",
                                  "ID Sitio", "Clave Sitio", "Descripción Sitio", "ID Cos", "Clave Cos", "Descripción Cos", "Fecha Inicio", "Fecha Fin"  };

            DataView dataView = new DataView(dt);
            DataTable nuevaDt = dataView.ToTable(false, columnasSeleccionar);

            for (int i = 0; i < nuevaDt.Columns.Count; i++)
            {
                nuevaDt.Columns[i].ColumnName = columnasCambiosNombre[i];
            }

            nuevaDt.AcceptChanges();

            return nuevaDt;
        }
        public static DataTable ExtenSinEmpleData()
        {
            DataTable dt = DSODataAccess.Execute(ConsultasString.ExtenSinEmpleQuery());

            // Hacemos modificacion de los datos necesarios y nombres de columnas
            string[] columnasSeleccionar = { "ICodCatalogo", "VchCodigo", "VchDescripcion",
                             "Recurs", "RecursCod", "RecursDesc", "Sitio", "SitioCod", "SitioDesc", "Cos", "CosCod", "CosDesc", "Masc", "dtIniVigencia", "dtFinVigencia" };
            string[] columnasCambiosNombre = { "ID Extensión", "Clave Extensión", "Descripción Extensión", "ID Recurso", "Clave Recurso", "Descripción Recurso",
                                  "ID Sitio", "Clave Sitio", "Descripción Sitio", "ID Cos", "Clave Cos", "Descripción Cos", "Mascara", "Fecha Inicio", "Fecha Fin"  };

            DataView dataView = new DataView(dt);
            DataTable nuevaDt = dataView.ToTable(false, columnasSeleccionar);

            for (int i = 0; i < nuevaDt.Columns.Count; i++)
            {
                nuevaDt.Columns[i].ColumnName = columnasCambiosNombre[i];
            }

            nuevaDt.AcceptChanges();

            return nuevaDt;
        }
        public static DataTable LineasSinEmpleData()
        {
            DataTable dt = DSODataAccess.Execute(ConsultasString.LineasSinEmpleQuery());

            // Hacemos modificacion de los datos necesarios y nombres de columnas
            string[] columnasSeleccionar = { "ICodCatalogo", "VchCodigo", "VchDescripcion",
                             "Recurs", "RecursCod", "RecursDesc", "Sitio", "SitioCod", "SitioDesc", "Carrier", "CarrierCod", "CarrierDesc", "CtaMaestra",
                             "CtaMaestraCod", "CtaMaestraDesc", "RazonSocial", "RazonSocialCod", "RazonSocialDesc", "TipoPlan", "TipoPlanCod", "TipoPlanDesc",
                             "EqCelular", "EqCelularCod", "EqCelularDesc", "PlanTarif", "PlanTarifCod", "PlanTarifDesc", "CargoFijo", "FecLimite", "FechaFinPlan",
                             "FechaDeActivacion", "Etiqueta", "Tel", "PlanLineaFactura", "IMEI", "ModeloCel", "NumOrden", "dtIniVigencia", "dtFinVigencia" };
            string[] columnasCambiosNombre = { "ID Línea", "Clave Línea", "Descripción Línea", "ID Recurso", "Clave Recurso", "Descripción Recurso",
                                  "ID Sitio", "Clave Sitio", "Descripción Sitio", "ID Carrier", "Clave Carrier", "Descripción Carrier",
                                  "ID Cuenta Maestra", "Clave Cuenta Maestra", "Descripción Cuenta Maestra", "ID Razón Social", "Clave Razón Social", "Descripción Razón Social",
                                  "ID Tipo Plan", "Clave Tipo Plan", "Descripción Tipo Plan", "ID Equipo Celular", "Clave Equipo Celular", "Descripción Equipo Celular",
                                  "ID Plan Tarifario", "Clave Plan Tarifario", "Descripción Plan Tarifario", "Cargo Fijo", "Fecha Limite", "Fecha Fin Plan",
                                  "Fecha de Activación", "Etiqueta", "Teléfono", "Plan Factura", "IMEI", "Modelo Celular", "Número de Orden", "Fecha Inicio", "Fecha Fin"  };

            DataView dataView = new DataView(dt);
            DataTable nuevaDt = dataView.ToTable(false, columnasSeleccionar);

            for (int i = 0; i < nuevaDt.Columns.Count; i++)
            {
                nuevaDt.Columns[i].ColumnName = columnasCambiosNombre[i];
            }

            nuevaDt.AcceptChanges();

            return nuevaDt;
        }
        public static DataTable OrganizacionData()
        {
            DataTable dt = DSODataAccess.Execute(ConsultasString.OrganizacionQuery());

            // Hacemos modificacion de los datos necesarios y nombres de columnas
            string[] columnasSeleccionar = { "iCodCatalogo", "vchCodigo", "vchDescripcion", "CuentaContable", "EntidadContable" };
            string[] columnasCambiosNombre = { "ID Organización", "Clave Organización", "Descripción Organización", "Cuenta Contable", "Entidad Contable" };

            DataView dataView = new DataView(dt);
            DataTable nuevaDt = dataView.ToTable(false, columnasSeleccionar);

            for (int i = 0; i < nuevaDt.Columns.Count; i++)
            {
                nuevaDt.Columns[i].ColumnName = columnasCambiosNombre[i];
            }

            nuevaDt.AcceptChanges();

            return nuevaDt;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Runtime.Serialization;
using DSOControls2008;
using System.Data;
using System.Text;
using KeytiaServiceBL;
using System.Collections;
using System.Web.SessionState;


namespace KeytiaWeb.UserInterface
{
    public class DashboardFieldCollection : ICollection, IEnumerable
    {
        protected HttpSessionState Session = HttpContext.Current.Session;
        protected Hashtable pHtFields = new Hashtable(); //Acceso por codigo de bloque
        protected List<DashboardReportField> pLstFields = new List<DashboardReportField>(); //Acceso por indice

        protected DashboardControl pDashboard;
        protected Table pTablaReportes;
        protected ValidacionPermisos ValidarPermiso;

        protected StringBuilder psbQuery = new StringBuilder();

        protected DataTable pVisConsultas;
        protected DataTable pVisBloques;
        protected DataTable pVisAreaBloque;
        protected DataTable pVisConfigPersonalizada;

        protected DateTime pIniVigencia = DateTime.Now;

        protected List<int> plstConsultasSeleccionadas;

        public DashboardFieldCollection(DashboardControl lDashboard, Table lTablaReportes, ValidacionPermisos lValidarPermiso)
        {
            this.pDashboard = lDashboard;
            this.pTablaReportes = lTablaReportes;
            this.ValidarPermiso = lValidarPermiso;

            CreateFields();
            FillControls();
        }

        public DateTime IniVigencia
        {
            get
            {
                return pIniVigencia;
            }
            set
            {
                pIniVigencia = value;
                foreach (DashboardReportField lField in this)
                {
                    lField.IniVigencia = value;
                }
            }
        }

        public DataTable VisConsultas
        {
            get
            {
                return pVisConsultas;
            }
        }

        public DataTable VisAreaBloque
        {
            get
            {
                return pVisAreaBloque;
            }
        }

        public DataTable VisConfigPersonalizada
        {
            get
            {
                return pVisConfigPersonalizada;
            }
        }

        public List<int> ConsultasSeleccionadas
        {
            get
            {
                return plstConsultasSeleccionadas;
            }
        }

        protected List<int> GetConsultasSeleccionadas()
        {
            List<int> lstConsultas = new List<int>();

            foreach (DashboardReportField lField in this)
            {
                lstConsultas.Add(lField.Consulta);
            }

            return lstConsultas;
        }

        protected void CreateFields()
        {
            pVisConsultas = DSODataAccess.Execute(GetVisConsultasQuery());

            psbQuery.Length = 0;
            psbQuery.AppendLine("select Top " + pVisConsultas.Rows.Count + " * ");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('BloqueDashboard','Bloque de dashboard','" + Globals.GetCurrentLanguage() + "')] His");
            psbQuery.AppendLine("where His.dtIniVigencia <= '" + pIniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and His.dtFinVigencia > '" + pIniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and His.Aplic = " + pDashboard.iCodDashboard);
            psbQuery.AppendLine("and His.Consul in(" + GetVisConsultasQuery("Rel.Aplic") + ")");
            psbQuery.AppendLine("order by His.OrdenBloque");

            pVisBloques = DSODataAccess.Execute(psbQuery.ToString());

            psbQuery.Length = 0;
            psbQuery.AppendLine("select * ");
            psbQuery.AppendLine("from " + DSODataContext.Schema + ".[VisHistoricos('BloqueDashboard','Configuración Personalizada','" + Globals.GetCurrentLanguage() + "')] His");
            psbQuery.AppendLine("where His.dtIniVigencia <= '" + pIniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and His.dtFinVigencia > '" + pIniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and His.dtIniVigencia <> His.dtFinVigencia");
            psbQuery.AppendLine("and His.Usuar = " + Session["iCodUsuario"]);
            psbQuery.AppendLine("and His.Consul in(" + GetVisConsultasQuery("Rel.Aplic") + ")");
            psbQuery.AppendLine("and His.BloqueDashboard is not null");

            List<string> lstBloques = new List<string>();
            foreach (DataRow lRowBloque in pVisBloques.Rows)
            {
                lstBloques.Add(lRowBloque["iCodCatalogo"].ToString()); 
            }
            if (lstBloques.Count == 0)
            {
                lstBloques.Add("0");
            }
            psbQuery.AppendLine("and His.BloqueDashboard in("+String.Join(",",lstBloques.ToArray())+")");

            pVisConfigPersonalizada = DSODataAccess.Execute(psbQuery.ToString());

            int liRow = 1;
            int liCol = 0;
            int liRowCols = 0;
            int liBlockSize = 0;
            DashboardReportField lField;
            foreach (DataRow lRowBloque in pVisBloques.Rows)
            {
                liBlockSize = (int)lRowBloque["BlockSize"];
                if (liRowCols + liBlockSize > 2)
                {
                    liRow = liRow + 1;
                    liCol = 1;
                    liRowCols = 0;
                }
                else
                {
                    liCol = 2;
                }

                lField = new DashboardReportField(pDashboard, pTablaReportes, ValidarPermiso);
                lField.iCodBloque = (int)lRowBloque["iCodCatalogo"];
                lField.vchCodBloque = (string)lRowBloque["vchCodigo"];
                lField.Row = liRow;
                lField.Col = liCol;
                lField.BlockSize = liBlockSize;
                lField.BanderasBloque = (int)lRowBloque["BanderasBloque"];
                lField.ConsultaDefault = (int)lRowBloque["Consul"];
                lField.AreaBloqueDefault = (int)lRowBloque["AreaBloque"];
                lField.Collection = this;
                lField.CreateField();
                Add(lField);

                liRowCols = liRowCols + liBlockSize;
            }
        }

        public string GetVisConsultasQuery()
        {
            return GetVisConsultasQuery("distinct Rel.Aplic, Rel.AplicCod, Rel.AplicDesc, Rel.RepEst, Rel.RepEstCod, Rel.RepEstDesc");
        }

        public string GetVisConsultasQuery(string lsCampos)
        {
            StringBuilder lsb = new StringBuilder();
            
            //RJ.20140414 Se cambia la vista VisHistoricos('Aplic','Español') por AplicSoloCatalogo en la consulta del método GetVisConsultasQuery(str)
            //RJ.20140414 Se cambia la vista VisHistoricos('RepEst','Español') por RepEstSoloCatalogo en la consulta del método GetVisConsultasQuery(str)
            lsb.Length = 0;
            lsb.AppendLine("select");
            lsb.AppendLine(lsCampos);
            lsb.AppendLine("from " + DSODataContext.Schema + ".[VisRelaciones('Aplicación - Estado - Perfil - Atributo - Consulta - Reporte','" + Globals.GetCurrentLanguage() + "')] Rel");
            lsb.AppendLine("where Rel.dtIniVigencia <= '" + pIniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsb.AppendLine("and Rel.dtFinVigencia > '" + pIniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsb.AppendLine("and Rel.Aplic is not null");
            lsb.AppendLine("and Rel.Aplic in(select Aplic.iCodCatalogo from " + DSODataContext.Schema + ".[AplicSoloCatalogo] Aplic");
            lsb.AppendLine("        where Aplic.dtIniVigencia <= '" + pIniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsb.AppendLine("        and Aplic.dtFinVigencia > '" + pIniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "')");
            lsb.AppendLine("and Rel.EstadoConsulta is null");
            lsb.AppendLine("and Rel.Perfil = " + pDashboard.iCodPerfil);
            lsb.AppendLine("and Rel.Atrib is null");
            lsb.AppendLine("and Rel.Consul is null");
            lsb.AppendLine("and Rel.RepEst is not null");
            lsb.AppendLine("and Rel.RepEst in(select RepEst.iCodCatalogo from " + DSODataContext.Schema + ".[RepEstSoloCatalogo] RepEst");
            lsb.AppendLine("        where RepEst.dtIniVigencia <= '" + pIniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            lsb.AppendLine("        and RepEst.dtFinVigencia > '" + pIniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "')");
            lsb.AppendLine("and Rel.Ruta is null");

            return lsb.ToString();
        }

        public void InitFields()
        {
            foreach (DashboardReportField lField in this)
            {
                lField.InitField();
            }
        }

        protected void Add(DashboardReportField lField)
        {
            pHtFields.Add(lField.iCodBloque, lField);
            pLstFields.Add(lField);
        }

        public void FillControls()
        {
            pVisConsultas = DSODataAccess.Execute(GetVisConsultasQuery());
            plstConsultasSeleccionadas = GetConsultasSeleccionadas();

            psbQuery.Length = 0;
            psbQuery.AppendLine("select * from " + DSODataContext.Schema + ".[VisHistoricos('Valores','Valores','"+Globals.GetCurrentLanguage()+"')] AreaBloque");
            psbQuery.AppendLine("where AreaBloque.dtIniVigencia <= '" + pIniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and AreaBloque.dtFinVigencia > '" + pIniVigencia.ToString("yyyy-MM-dd HH:mm:ss") + "'");
            psbQuery.AppendLine("and AreaBloque.AtribCod = 'AreaBloque'");
            psbQuery.AppendLine("order by AreaBloque.OrdenPre, AreaBloque.Value");

            pVisAreaBloque = DSODataAccess.Execute(psbQuery.ToString());

            foreach (DashboardReportField lField in this)
            {
                lField.Fill();
            }
        }

        public virtual void InitLanguage()
        {
            foreach (DashboardReportField lField in this)
            {
                lField.InitLanguage();
            }
        }

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return pLstFields.GetEnumerator();
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            pHtFields.CopyTo(array, index);
        }

        public int Count
        {
            get
            {
                return pHtFields.Count;
            }
        }

        int ICollection.Count
        {
            get
            {
                return Count;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return pHtFields.IsSynchronized;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return pHtFields.SyncRoot;
            }
        }

        #endregion
    }
}

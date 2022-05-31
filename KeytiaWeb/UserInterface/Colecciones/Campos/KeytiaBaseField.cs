using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using DSOControls2008;
using KeytiaServiceBL;
using System.Web.UI;
using System.Web;
using System.Text;
using System.Globalization;
using System.Web.SessionState;

namespace KeytiaWeb.UserInterface
{
    public abstract class KeytiaBaseField
    {
        protected HttpSessionState Session = HttpContext.Current.Session;
        protected string pColumn;
        protected int pConfigValue;
        protected string pConfigName;
        protected int piCodEntidad;
        protected int pRow = 1;
        protected int pCol = 1;
        protected int pColumnSpan = 1;
        protected string pDescripcion;
        protected Table pTable;
        protected ValidacionPermisos pValidarPermiso;

        protected string pLangEntity;
        protected int pLangValue;
        protected string psLang;

        protected WebControl pContainer;
        protected StringBuilder psbQuery = new StringBuilder();
        protected KDBAccess pKDB = new KDBAccess();

        protected KeytiaFieldCollection pCollection;
        protected string pSubHistoricClass = "KeytiaWeb.UserInterface.HistoricEdit";
        protected string pSubCollectionClass = "KeytiaWeb.UserInterface.HistoricFieldCollection";

        protected DateTime pIniVigencia = DateTime.Now;
        protected object pFinVigencia = new DateTime(2079, 01, 01);

        public DateTime IniVigencia
        {
            get
            {
                return pIniVigencia;
            }
            set
            {
                pIniVigencia = value;
                pKDB.FechaVigencia = value;
            }
        }

        public DateTime FinVigencia
        {
            set
            {
                pFinVigencia = value;
            }
        }

        public object GetFinVigencia()
        {
            return pFinVigencia;
        }

        public void ClearFinVigencia()
        {
            pFinVigencia = new DateTime(2079, 01, 01);
        }

        public string SubCollectionClass
        {
            get
            {
                return pSubCollectionClass;
            }
            set
            {
                pSubCollectionClass = value;
            }
        }

        public string SubHistoricClass
        {
            get
            {
                return pSubHistoricClass;
            }
            set
            {
                pSubHistoricClass = value;
            }
        }

        public KeytiaFieldCollection Collection
        {
            get
            {
                return pCollection;
            }
            set
            {
                pCollection = value;
            }
        }

        protected string pjsObj;
        public string JsObj
        {
            get { return pjsObj; }
        }

        public KeytiaBaseField()
        {
            psLang = "{" + Globals.GetCurrentLanguage() + "}";
            pKDB.FechaVigencia = pIniVigencia;
        }

        public WebControl Container
        {
            get
            {
                return pContainer;
            }
            set
            {
                pContainer = value;
                if (pContainer != null)
                {
                    pjsObj = pContainer.ID;
                }
            }
        }

        public ValidacionPermisos ValidarPermiso
        {
            get
            {
                return pValidarPermiso;
            }
            set
            {
                pValidarPermiso = value;
            }
        }

        public string Column
        {
            get
            {
                return pColumn;
            }
            set
            {
                pColumn = value;
            }
        }

        public virtual int ConfigValue
        {
            get
            {
                return pConfigValue;
            }
            set
            {
                pConfigValue = value;
            }
        }

        public virtual string ConfigName
        {
            get
            {
                if (String.IsNullOrEmpty(pConfigName))
                {
                    pConfigName = (string)DSODataAccess.ExecuteScalar("select vchCodigo from Catalogos where iCodRegistro = " + pConfigValue);
                }
                return pConfigName;
            }
            set
            {
                pConfigName = value;
            }
        }

        public virtual int iCodEntidad
        {
            get
            {
                return piCodEntidad;
            }
            set
            {
                piCodEntidad = value;
            }
        }

        public virtual object DataValue
        {
            get
            {
                return DSOControlDB.DataValue;
            }
            set
            {
                DSOControlDB.DataValue = value;
            }
        }

        public int Row
        {
            get
            {
                if (DSOControlDB != null)
                {
                    return DSOControlDB.Row;
                }
                else
                {
                    return pRow;
                }
            }
            set
            {
                pRow = value;
            }
        }

        public int Col
        {
            get
            {
                return pCol;
            }
            set
            {
                pCol = value;
            }
        }

        public int ColumnSpan
        {
            get
            {
                return pColumnSpan;
            }
            set
            {
                pColumnSpan = value;
            }
        }

        public string Descripcion
        {
            get
            {
                if (DSOControlDB != null)
                {
                    return DSOControlDB.Descripcion;
                }
                else
                {
                    return pDescripcion;
                }
            }
        }

        public string LangEntity
        {
            get
            {
                return pLangEntity;
            }
            set
            {
                pLangEntity = value;
            }
        }

        public int LangValue
        {
            get
            {
                return pLangValue;
            }
            set
            {
                pLangValue = value;
            }
        }

        public Table Table
        {
            get
            {
                return pTable;
            }
            set
            {
                pTable = value;
            }
        }

        public virtual bool ShowInGrid
        {
            get
            {
                return true;
            }
        }

        public abstract DSOControlDB DSOControlDB { get; }

        public abstract void CreateField();

        protected virtual void InitDSOControlDB()
        {
            this.DSOControlDB.ID = pColumn;
            this.DSOControlDB.DataField = pColumn;
            this.DSOControlDB.Table = pTable;
            this.DSOControlDB.Row = pRow;
            this.DSOControlDB.ColumnSpan = pColumnSpan;
            this.DSOControlDB.AddClientEvent("Col", pCol.ToString());
            this.DSOControlDB.AddClientEvent("keytiaField", ConfigName);
        }

        public virtual void InitField()
        {
            this.DSOControlDB.CreateControls();
        }

        public virtual void InitLanguage()
        {
            psLang = "{" + Globals.GetCurrentLanguage() + "}";
            KDBAccess lKDB = new KDBAccess();
            if (!String.IsNullOrEmpty(pLangEntity) && pLangValue != 0)
            {
                DataTable lKDBTable = lKDB.GetHisRegByEnt(pLangEntity, "", "iCodCatalogo = " + pLangValue);
                if (lKDBTable.Columns.Contains(psLang) && lKDBTable.Rows[0][psLang] != DBNull.Value)
                {
                    pDescripcion = lKDBTable.Rows[0][psLang].ToString();
                }
                else
                {
                    pDescripcion = lKDBTable.Rows[0]["vchDescripcion"].ToString();
                }
            }
            else if (!pColumn.StartsWith("iCodCatalogo") && !String.IsNullOrEmpty(ConfigName))
            {
                pDescripcion = Globals.GetLangItem("Atrib", "Atributos", ConfigName);
            }

            if (String.IsNullOrEmpty(pDescripcion) || pDescripcion.StartsWith("#undefined-"))
            {
                pDescripcion = ConfigName;
            }
        }

        public virtual void InitDSOControlDBLanguage()
        {
            if (String.IsNullOrEmpty(pDescripcion))
            {
                InitLanguage();
            }
            DSOControlDB.Descripcion = pDescripcion;
        }

        public virtual void DisableField()
        {
            ((WebControl)DSOControlDB.Control).Enabled = false;
        }

        public virtual void EnableField()
        {
            ((WebControl)DSOControlDB.Control).Enabled = true;
        }

        public override string ToString()
        {
            if (DSOControlDB != null)
            {
                return DSOControlDB.ToString();
            }
            else
            {
                return base.ToString();
            }
        }
    }
}

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
    public delegate bool ValidacionPermisos(Permiso p); //Metodo para delegar la validacion de permisos a la pagina u objeto contenedor de la coleccion

    public interface IKeytiaFillableField
    {
        void Fill();
    }

    public interface IKeytiaFillableAjaxField
    {
        void FillAjax();
    }

    public abstract class KeytiaFieldCollection : ICollection, IEnumerable
    {
        protected Hashtable pHtFields = new Hashtable(); //Acceso por nombre de columna
        protected Hashtable pHtConfigNameFields = new Hashtable(); //Acceso por nombre de entidad
        protected Hashtable pHtConfigValueFields = new Hashtable(); //Acceso por iCodRegistro de entidad
        protected List<KeytiaBaseField> pLstFields = new List<KeytiaBaseField>(); //Acceso por indice
        protected int piCodEntidad;
        protected int piCodConfig;
        protected Table pTablaAtritubos;
        protected DataTable pMetaData;
        protected WebControl pContainer;
        protected ValidacionPermisos ValidarPermiso;

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
                foreach (KeytiaBaseField lField in this)
                {
                    lField.IniVigencia = value;
                }
            }
        }

        public DateTime FinVigencia
        {
            set
            {
                pFinVigencia = value;
                foreach (KeytiaBaseField lField in this)
                {
                    lField.FinVigencia = value;
                }
            }
        }

        public object GetFinVigencia()
        {
            return pFinVigencia;
        }

        public void ClearFinVigencia()
        {
            pFinVigencia = new DateTime(2079, 01, 01);
            foreach (KeytiaBaseField lField in this)
            {
                lField.ClearFinVigencia();
            }
        }

        public int iCodEntidad
        {
            get
            {
                return piCodEntidad;
            }
        }

        public int iCodConfig
        {
            get
            {
                return piCodConfig;
            }
        }

        protected KeytiaFieldCollection()
        {
        }

        protected KeytiaFieldCollection(WebControl lContainer, int liCodEntidad, int liCodConfig, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
        {
            InitCollection(lContainer, liCodEntidad, liCodConfig, lTablaAtributos, lValidarPermiso);
        }

        public virtual void InitCollection(WebControl lContainer, int liCodEntidad, int liCodConfig, Table lTablaAtributos, ValidacionPermisos lValidarPermiso)
        {
            this.pContainer = lContainer;
            this.piCodEntidad = liCodEntidad;
            this.piCodConfig = liCodConfig;
            this.pTablaAtritubos = lTablaAtributos;
            this.ValidarPermiso = lValidarPermiso;

            InitMetaData();
            FillMetaData();
            CreateFields();
        }

        protected virtual void InitMetaData()
        {
            pMetaData = new DataTable();
            pMetaData.Columns.Add(new DataColumn("Column", typeof(string)));  //ID del control principal del campo
            pMetaData.Columns.Add(new DataColumn("iCodEntidad", typeof(int))); //Valor de configuracion de la entidad
            pMetaData.Columns.Add(new DataColumn("ConfigValue", typeof(int))); //Valor de configuracion de la columna (iCodCatalogo)
            pMetaData.Columns.Add(new DataColumn("ConfigName", typeof(string))); //Valor de configuracion de la columna (vchCodigo)
            pMetaData.Columns.Add(new DataColumn("Row", typeof(int)));
            pMetaData.Columns.Add(new DataColumn("Col", typeof(int)));
            pMetaData.Columns.Add(new DataColumn("ColumnSpan", typeof(int)));
        }

        protected abstract void FillMetaData();

        protected abstract void CreateFields();

        public virtual void InitFields()
        {
            foreach (KeytiaBaseField lField in this)
            {
                lField.InitField();
            }
        }

        public virtual void InitLanguage()
        {
            foreach (KeytiaBaseField lField in this)
            {
                lField.InitLanguage();
                if (lField.DSOControlDB != null)
                {
                    lField.InitDSOControlDBLanguage();
                }
            }
        }

        public KeytiaBaseField this[string key]
        {
            get
            {
                return pHtFields[key] as KeytiaBaseField;
            }
            set
            {
                pHtFields[key] = value;
            }
        }

        public KeytiaBaseField this[int index]
        {
            get
            {
                return pLstFields[index];
            }
            set
            {
                pLstFields[index] = value;
            }
        }

        public KeytiaBaseField GetByConfigName(string configName)
        {
            return pHtConfigNameFields[configName] as KeytiaBaseField;
        }

        public KeytiaBaseField GetByConfigValue(int configValue)
        {
            return pHtConfigValueFields[configValue] as KeytiaBaseField;
        }

        public virtual void EnableFields()
        {
            foreach (KeytiaBaseField lField in this)
            {
                lField.EnableField();
            }
        }

        public virtual void DisableFields()
        {
            foreach (KeytiaBaseField lField in this)
            {
                lField.DisableField();
            }
        }

        public virtual void SetValues(DataRow lDataRow)
        {
            foreach (KeytiaBaseField lField in this)
            {
                if (!(lField is KeytiaRelationField) && lDataRow.Table.Columns.Contains(lField.Column))
                {
                    lField.DataValue = lDataRow[lField.Column];
                }
                else
                {
                    lField.DataValue = lDataRow["iCodCatalogo"];
                }
            }
        }

        public bool Contains(string key)
        {
            return pHtFields.ContainsKey(key);
        }

        public bool ContainsConfigName(string config)
        {
            return pHtConfigNameFields.ContainsKey(config);
        }

        public bool ContainsConfigValue(int config)
        {
            return pHtConfigValueFields.ContainsKey(config);
        }

        public bool Contains(KeytiaBaseField lField)
        {
            return pHtFields.Contains(lField);
        }

        protected void Add(KeytiaBaseField lField)
        {
            pHtFields.Add(lField.Column, lField);

            //Se supone que los valores de configuracion NO PUEDEN REPETIRSE 
            //pero de todas formas los valido por que de momento si hay repetidos
            if (pHtConfigNameFields.ContainsKey(lField.ConfigName))
            {
                //si el valor esta repetido entonces le pongo la clave de la columna
                pHtConfigNameFields.Add(lField.Column, lField);
            }
            else
            {
                pHtConfigNameFields.Add(lField.ConfigName, lField);
            }
            if (pHtConfigValueFields.ContainsKey(lField.ConfigValue))
            {
                //si el valor esta repetido entonces le pongo la clave de la columna
                pHtConfigValueFields.Add(lField.Column, lField);
            }
            else
            {
                pHtConfigValueFields.Add(lField.ConfigValue, lField);
            }

            pLstFields.Add(lField);
        }

        public void FillControls()
        {
            foreach (KeytiaBaseField lField in this)
            {
                if (lField is IKeytiaFillableField)
                {
                    ((IKeytiaFillableField)lField).Fill();
                }
            }
        }

        public void FillAjaxControls()
        {
            foreach (KeytiaBaseField lField in this)
            {
                if (lField is IKeytiaFillableAjaxField)
                {
                    ((IKeytiaFillableAjaxField)lField).FillAjax();
                }
            }
        }

        public void FormatGridData(DSOGridServerResponse lgsrRet, DataTable ldt)
        {
            FormatGridData(lgsrRet, ldt, false);
        }

        public void FormatGridData(DSOGridServerResponse lgsrRet, DataTable ldt, bool lbUseConfigName)
        {
            string lsDateFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateFormat");
            string lsDateTimeFormat = Globals.GetLangItem("MsgWeb", "Mensajes Web", "NetDateTimeFormat");
            Dictionary<string, string> lColStringFormat = new Dictionary<string, string>();
            Dictionary<string, IFormatProvider> lColFormatter = new Dictionary<string, IFormatProvider>();
            string lsColumn;
            string lsLang = Globals.GetCurrentLanguage();

            foreach (KeytiaBaseField lField in this)
            {
                if (lbUseConfigName)
                {
                    lsColumn = lField.ConfigName;
                }
                else
                {
                    lsColumn = lField.Column;
                }

                if (lField is KeytiaDateField)
                {
                    lColStringFormat.Add(lsColumn, lsDateFormat);
                }
                else if (lField is KeytiaTimeField)
                {
                    lColStringFormat.Add(lsColumn, "HH:mm:ss");
                }
                else if (lField is KeytiaDateTimeField)
                {
                    lColStringFormat.Add(lsColumn, lsDateTimeFormat);
                }
                else if (lField is KeytiaNumericField)
                {
                    lColStringFormat.Add(lsColumn, ((KeytiaNumericField)lField).StringFormat);
                    lColFormatter.Add(lsColumn, ((KeytiaNumericField)lField).FormatInfo);
                }
                //else if (lField is KeytiaPasswordField)
                //{
                //    foreach (DataRow ldataRow in ldt.Rows)
                //    {
                //        ldataRow[lsColumn] = Util.Decrypt(ldataRow[lsColumn].ToString());
                //    }
                //}
            }
            lColStringFormat.Add("dtIniVigencia", lsDateFormat);
            lColStringFormat.Add("dtFinVigencia", lsDateFormat);

            lgsrRet.SetDataFromDataTable(ldt, lColStringFormat, lColFormatter, lsLang);
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

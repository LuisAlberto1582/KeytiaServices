using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using DSOControls2008;
using KeytiaServiceBL;
using KeytiaCOM;

namespace KeytiaWeb.UserInterface
{
    [DataContract]
    public class ReporteUsuarioCriteria
    {
        private int piId;
        private string psRow;
        private ReporteUsuarioField poField;
        private int piFieldId;
        private int piOperator;
        private string psValue;

        [DataMember(Name = "id")]
        public int Id
        {
            get { return piId; }
            set { piId = value; }
        }

        [DataMember(Name = "row")]
        public string Row
        {
            get { return psRow; }
            set { psRow = value; }
        }

        [DataMember(Name = "fieldId")]
        public int FieldId
        {
            get { return (poField != null ? poField.Id : piFieldId); }
            set
            {
                poField = null;
                piFieldId = value;
            }
        }

        [DataMember(Name = "operator")]
        public int Operator
        {
            get { return piOperator; }
            set { piOperator = value; }
        }

        [DataMember(Name = "value")]
        public string Value
        {
            get { return psValue; }
            set { psValue = value; }
        }

        public ReporteUsuarioField Field
        {
            get { return poField; }
            set { poField = value; }
        }
    }
}
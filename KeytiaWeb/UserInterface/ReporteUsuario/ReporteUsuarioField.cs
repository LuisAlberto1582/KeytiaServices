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
    public class ReporteUsuarioField
    {
        private int piId;
        private string psName = "";
        private string psType = "";
        private ReporteUsuarioCategory poCategory;
        private int piCategoryId;
        private bool pbIsSelected = false;
        private ListItem pliItem;
        private string psItemId = "";
        private int piFieldOrder = -1;
        private string psAggregateFn = "";
        private int piDataOrder = -1;
        private int piDataOrderType = -1;
        private int piGroup = -1;
        private int piGroupType = -1;
        private string psDataField = "";

        private int piGroupMatX = -1;
        private int piGroupMatY = -1;

        private int piTipoPeriodoGrp = 0;
        private int piTipoPeriodoGrpX = 0;
        private int piTipoPeriodoGrpY = 0;

        public ReporteUsuarioField(int liId, string lsName, string lsDataField, string lsType, ReporteUsuarioCategory loCategory)
        {
            piId = liId;
            psName = lsName;
            psDataField = lsDataField;
            psType = lsType;
            poCategory = loCategory;
        }

        [DataMember(Name = "id")]
        public int Id
        {
            get { return piId; }
            set { piId = value; }
        }

        [DataMember(Name = "name")]
        public string Name
        {
            get { return psName; }
            set { psName = value; }
        }

        [DataMember(Name = "type")]
        public string Type
        {
            get { return psType; }
            set { psType = value; }
        }

        [DataMember(Name = "categoryId")]
        public int CategoryId
        {
            get { return (poCategory != null ? poCategory.Id : piCategoryId); }
            set
            {
                poCategory = null;
                piCategoryId = value;
            }
        }

        [DataMember(Name = "isSelected")]
        public bool IsSelected
        {
            get
            {
                if (pliItem != null)
                    pbIsSelected = pliItem.Selected;

                return pbIsSelected;
            }
            set
            {
                pbIsSelected = value;

                if (pliItem != null)
                    pliItem.Selected = pbIsSelected;
            }
        }

        [DataMember(Name = "itemId")]
        public string ItemId
        {
            get { return psItemId; }
            set { psItemId = value; }
        }

        [DataMember(Name = "fieldOrder")]
        public int FieldOrder
        {
            get { return piFieldOrder; }
            set { piFieldOrder = value; }
        }

        [DataMember(Name = "aggregateFn")]
        public string AggregateFn
        {
            get { return psAggregateFn; }
            set { psAggregateFn = value; }
        }

        [DataMember(Name = "dataOrder")]
        public int DataOrder
        {
            get { return piDataOrder; }
            set { piDataOrder = value; }
        }

        [DataMember(Name = "dataOrderType")]
        public int DataOrderType
        {
            get { return piDataOrderType; }
            set { piDataOrderType = value; }
        }

        [DataMember(Name = "group")]
        public int Group
        {
            get { return piGroup; }
            set { piGroup = value; }
        }

        [DataMember(Name = "groupType")]
        public int GroupType
        {
            get { return piGroupType; }
            set { piGroupType = value; }
        }

        [DataMember(Name = "dataField")]
        public string DataField
        {
            get { return psDataField; }
            set { psDataField = value; }
        }

        [DataMember(Name = "groupMatX")]
        public int GroupMatX
        {
            get { return piGroupMatX; }
            set { piGroupMatX = value; }
        }

        [DataMember(Name = "groupMatY")]
        public int GroupMatY
        {
            get { return piGroupMatY; }
            set { piGroupMatY = value; }
        }

        [DataMember(Name = "tipoPeriodoGrp")]
        public int TipoPeriodoGrp
        {
            get { return piTipoPeriodoGrp; }
            set { piTipoPeriodoGrp = value; }
        }

        [DataMember(Name = "tipoPeriodoGrpX")]
        public int TipoPeriodoGrpX
        {
            get { return piTipoPeriodoGrpX; }
            set { piTipoPeriodoGrpX = value; }
        }

        [DataMember(Name = "tipoPeriodoGrpY")]
        public int TipoPeriodoGrpY
        {
            get { return piTipoPeriodoGrpY; }
            set { piTipoPeriodoGrpY = value; }
        }

        public ReporteUsuarioCategory Category
        {
            get { return poCategory; }
            set { poCategory = value; }
        }

        public ListItem Item
        {
            get { return pliItem; }
            set { pliItem = value; }
        }
    }
}
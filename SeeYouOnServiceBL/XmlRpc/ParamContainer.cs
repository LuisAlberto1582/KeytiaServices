using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;

namespace SeeYouOnServiceBL.XmlRpc
{
    public class ParamContainer
    {
        protected List<Param> plParams = new List<Param>();

        [XmlArray("params")]
        [XmlArrayItem("param")]
        public List<Param> Params
        {
            get { return plParams; }
            set { plParams = value; }
        }

        [XmlIgnore()]
        public ValueContainer Param
        {
            get
            {
                if (plParams == null)
                    plParams = new List<Param>();

                if (plParams.Count == 0)
                    plParams.Add(new Param());

                return plParams[0];
            }
        }
    }
}

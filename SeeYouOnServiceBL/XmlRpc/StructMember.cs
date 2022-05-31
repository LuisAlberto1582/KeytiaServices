using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;

namespace SeeYouOnServiceBL.XmlRpc
{
    public class StructMember : ValueContainer
    {
        protected string psName = "";

        public StructMember() { }

        public StructMember(string lsName, object loValue)
        {
            psName = lsName;
            poValue = new Value(loValue);
        }

        [XmlElement("name")]
        public string Name
        {
            get { return psName; }
            set { psName = value; }
        }
    }
}

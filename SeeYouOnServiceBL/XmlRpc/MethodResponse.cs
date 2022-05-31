using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;

namespace SeeYouOnServiceBL.XmlRpc
{
    [XmlRoot("methodResponse")]
    public class MethodResponse : ParamContainer
    {
        protected ValueContainer loFault;

        [XmlElement("fault")]
        public ValueContainer Fault
        {
            get { return loFault; }
            set { loFault = value; }
        }
    }
}

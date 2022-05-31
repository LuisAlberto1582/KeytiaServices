using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;

namespace SeeYouOnServiceBL.XmlRpc
{
    public class ValueContainer
    {
        protected Value poValue = new Value();

        [XmlElement("value")]
        public Value Value
        {
            get { return poValue; }
            set { poValue = value; }
        }

        [XmlIgnore()]
        public object this[string lsName]
        {
            get
            {
                return poValue[lsName];
            }

            set
            {
                if (poValue.Object == null && poValue.ObjectStruct == null)
                    poValue.ObjectStruct = new Struct();

                poValue[lsName] = value;
            }
        }

        public void Remove(string lsName)
        {
            poValue.Remove(lsName);
        }
    }
}

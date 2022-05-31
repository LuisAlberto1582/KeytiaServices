using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;

namespace SeeYouOnServiceBL.XmlRpc
{
    public class Param : ValueContainer
    {
        public Param()
        {
            poValue.ObjectStruct = new Struct();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;

namespace SeeYouOnServiceBL.XmlRpc
{
    public class ValueArray
    {
        protected List<Value> lstValues = new List<Value>();

        [XmlIgnore]
        public Value this[int liIndex]
        {
            get { return lstValues[liIndex]; }
            set { lstValues[liIndex] = value; }
        }

        [XmlIgnore]
        public int Count
        {
            get { return lstValues.Count; }
        }

        public void Add(Value loValue)
        {
            lstValues.Add(loValue);
        }

        public void Remove(Value loValue)
        {
            lstValues.Remove(loValue);
        }

        [XmlArray("data")]
        [XmlArrayItem("value")]
        public List<Value> Values
        {
            get { return lstValues; }
            set { lstValues = value; }
        }
    }
}

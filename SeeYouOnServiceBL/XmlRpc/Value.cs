using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;

namespace SeeYouOnServiceBL.XmlRpc
{
    public class Value
    {
        protected object poObject = null;

        public Value()
        {

        }

        public Value(object loValue)
        {
            poObject = loValue;
        }

        [XmlIgnore]
        public object Object
        {
            get { return poObject; }
            set { poObject = value; }
        }

        [XmlIgnore]
        public object this[string lsName]
        {
            get
            {
                object loRet = null;

                if (Object == null)
                    throw new Exception("El valor no está definido");
                else if (ObjectStruct == null)
                    throw new Exception("El valor no es un Struct");

                if (ObjectStruct.ContainsKey(lsName))
                    loRet = ObjectStruct[lsName];

                return loRet;
            }

            set
            {
                if (Object != null && ObjectStruct == null)
                    throw new Exception("El valor no es un Struct");
                else if (Object == null && ObjectStruct == null)
                    ObjectStruct = new Struct();

                ObjectStruct[lsName] = value;
            }
        }

        public void Remove(string lsName)
        {
            if (ObjectStruct != null && ObjectStruct.ContainsKey(lsName))
                ObjectStruct.Remove(lsName);
        }

        [XmlElement("string", typeof(string))]
        [XmlElement("int", typeof(int))]
        [XmlElement("boolean", typeof(bool))]
        public object ObjectGeneric
        {
            get
            {
                return (poObject is string || poObject is int || poObject is bool) ? poObject : null;
            }
            set { poObject = value; }
        }

        [XmlElement("dateTime.iso8601")]
        public string ObjectDate
        {
            get { return poObject is DateTime ? ((DateTime)poObject).ToString("yyyyMMddTHH:mm:ss") : null; }
            set { poObject = DateTime.ParseExact(value, "yyyyMMddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture); }
        }

        [XmlElement("array", typeof(ValueArray))]
        public ValueArray ObjectArray
        {
            get { return poObject is ValueArray ? (ValueArray)poObject : null; }
            set { poObject = value; }
        }

        [XmlIgnore()]
        public Struct ObjectStruct
        {
            get { return poObject is Struct ? (Struct)poObject : null; }
            set { poObject = value; }
        }

        [XmlArray("struct")]
        [XmlArrayItem("member")]
        public StructMember[] ObjectStructMembers
        {
            get { return poObject is Struct ? ((Struct)poObject).Members.ToArray() : null; }
            set
            {
                if (ObjectStruct == null)
                    poObject = new Struct();

                ObjectStruct.Members = new List<StructMember>(value);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;

namespace SeeYouOnServiceBL.XmlRpc
{
    public class Struct
    {
        protected Dictionary<string, StructMember> lstMembers = new Dictionary<string, StructMember>();

        [XmlIgnore]
        public object this[string lsName]
        {
            get
            {
                object loRet = null;

                if (lstMembers.ContainsKey(lsName))
                    loRet = lstMembers[lsName].Value.Object;

                return loRet;
            }
            set
            {
                if (lstMembers.ContainsKey(lsName))
                    lstMembers[lsName] = new StructMember(lsName, value);
                else
                    lstMembers.Add(lsName, new StructMember(lsName, value));
            }
        }

        [XmlIgnore]
        public int Count
        {
            get { return lstMembers.Count; }
        }

        public bool ContainsKey(string lsName)
        {
            return lstMembers.ContainsKey(lsName);
        }

        public void Remove(string lsName)
        {
            if (lstMembers.ContainsKey(lsName))
                lstMembers.Remove(lsName);
        }

        public List<StructMember> Members
        {
            get
            {
                List<StructMember> lstSM = null;

                lstSM = new List<StructMember>();
                foreach (StructMember loSM in lstMembers.Values)
                    lstSM.Add(loSM);

                return lstSM;
            }
            set
            {
                lstMembers.Clear();

                foreach (StructMember loSM in value)
                    lstMembers.Add(loSM.Name, loSM);
            }
        }
    }
}

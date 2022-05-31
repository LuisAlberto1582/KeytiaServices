using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeeYouOnServiceBL
{
    public class MCUException : Exception
    {
        protected int piFaultCode = int.MinValue;
        protected string psFaultString = "";

        public MCUException(string lsMessage, int liFaultCode, string lsFaultString) :
            base(lsMessage + "\r\n" + liFaultCode + " " + lsFaultString)
        {
            piFaultCode = liFaultCode;
            psFaultString = lsFaultString;
        }

        public int FaultCode
        {
            get { return piFaultCode; }
        }

        public string FaultString
        {
            get { return psFaultString; }
        }
    }
}

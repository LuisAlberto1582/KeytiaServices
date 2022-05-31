using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace KeytiaWeb
{
    [Serializable]
    public class KeytiaWebException : Exception, ISerializable
    {
        private string pMessage;
        private bool pbLogException;

        public KeytiaWebException(string lsCodigoMsg) : this(false, lsCodigoMsg, null) { }

        public KeytiaWebException(string lsCodigoMsg, Exception inner) : this(false, lsCodigoMsg, inner) { }

        public KeytiaWebException(bool lbLogException, string lsCodigoMsg) : this(lbLogException, lsCodigoMsg, null) { }

        public KeytiaWebException(bool lbLogException, string lsCodigoMsg, Exception inner)
            : base(lsCodigoMsg, inner)
        {
            if (lbLogException)
            {
                try
                {
                    KeytiaServiceBL.Util.LogException("Surgió un error en la aplicación Web.", inner);
                }
                catch { }
            }
            try
            {
                pMessage = Globals.GetLangItem("MsgWeb", "Errores", lsCodigoMsg);
            }
            catch
            {
                pMessage = lsCodigoMsg;
            }
        }

        public KeytiaWebException(string lsCodigoMsg, Exception inner, params string[] lsParam) : this(false, lsCodigoMsg, inner, lsParam) { }

        public KeytiaWebException(bool lbLogException, string lsCodigoMsg, Exception inner, params string[] lsParam)
            : base(lsCodigoMsg, inner)
        {
            if (lbLogException)
            {
                try
                {
                    KeytiaServiceBL.Util.LogException("Surgió un error en la aplicación Web.", inner);
                }
                catch { }
            }
            try
            {
                pMessage = Globals.GetLangItem("MsgWeb", "Errores", lsCodigoMsg, lsParam);
            }
            catch
            {
                pMessage = lsCodigoMsg;
            }
        }

        public override string Message
        {
            get
            {
                return pMessage;
            }
        }

        public bool bLogException
        {
            get
            {
                return pbLogException;
            }
        }
    }

    [Serializable]
    public class KeytiaWebSessionException : KeytiaWebException, ISerializable
    {
        public KeytiaWebSessionException(bool lbLogException, params string[] lsParam)
            : base(lbLogException, "ErrSession", null, lsParam) { }
    }
}

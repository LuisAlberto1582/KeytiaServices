using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Xml.Serialization;
using System.Text;

using KeytiaServiceBL;

namespace SeeYouOnServiceBL.XmlRpc
{
    [XmlRoot("methodCall")]
    public class MethodCall : ParamContainer
    {
        protected string psMethodName = "";
        protected string psUrl = "";
        protected string psUser = "";
        protected string psPassword = "";
        protected string psResponse = "";
        protected string psCall = "";

        public MethodCall()
        {

        }

        public MethodCall(string lsUrl, string lsUser, string lsPassword)
        {
            psUrl = lsUrl;
            psUser = lsUser;
            psPassword = lsPassword;
        }

        [XmlElement("methodName")]
        public string MethodName
        {
            get { return psMethodName; }
            set { psMethodName = value; }
        }

        [XmlIgnore()]
        public string Url
        {
            get { return psUrl; }
            set { psUrl = value; }
        }

        [XmlIgnore()]
        public string User
        {
            get { return psUser; }
            set { psUser = value; }
        }

        [XmlIgnore()]
        public string Password
        {
            get { return psPassword; }
            set { psPassword = value; }
        }

        [XmlIgnore()]
        public string ResponseString
        {
            get { return psResponse; }
        }

        [XmlIgnore()]
        public string CallString
        {
            get { return psCall; }
        }

        public void Clear()
        {
            psCall = "";
            psResponse = "";
            psMethodName = "";
            plParams = new List<Param>();
        }

        public MethodResponse Call()
        {
            MethodResponse loRet = null;
            byte[] lbResp = null;
            bool lbHayError = false;

            //Asigna el usuario y password
            if (this.Param.Value != null)
            {
                if (String.IsNullOrEmpty(psUser))
                    this.Param.Remove("authenticationUser");
                else
                    this.Param["authenticationUser"] = psUser;

                if (String.IsNullOrEmpty(psPassword))
                    this.Param.Remove("authenticationPassword");
                else
                    this.Param["authenticationPassword"] = psPassword;
            }

            //Serializa la llamada
            XmlSerializer lxsCall = new XmlSerializer(this.GetType());
            MemoryStream loMem = new System.IO.MemoryStream();
            lxsCall.Serialize(loMem, this);

            StringBuilder lsb = new StringBuilder();
            StringWriter loSW = new StringWriter(lsb);
            lxsCall.Serialize(loSW, this);
            psCall = lsb.ToString();

            //Hace la llamada
            RemoteCertificateValidationCallback loCertVal = ServicePointManager.ServerCertificateValidationCallback;

            try
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                WebClient loWeb = new WebClient();
                lbResp = loWeb.UploadData(psUrl, "POST", loMem.ToArray());
            }
            catch (Exception ex)
            {
                lbHayError = true;
                Util.LogException("Surgió un error en la llamada XML-RPC", ex);
                throw ex;
            }
            finally
            {
                System.Net.ServicePointManager.ServerCertificateValidationCallback = loCertVal;
            }

            if (!lbHayError)
            {
                UTF8Encoding loEnc = new UTF8Encoding();
                psResponse = loEnc.GetString(lbResp);

                //Deserializa la respuesta
                XmlSerializer lxsResp = new XmlSerializer(typeof(MethodResponse));
                loRet = (MethodResponse)lxsResp.Deserialize(new System.IO.MemoryStream(lbResp));
            }

            return loRet;
        }
    }
}

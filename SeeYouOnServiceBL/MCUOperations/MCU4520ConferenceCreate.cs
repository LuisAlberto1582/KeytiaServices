using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KeytiaServiceBL.XmlRpc;
using SeeYouOnServiceBL.Models;

namespace SeeYouOnServiceBL.MCUOperations
{
    public static class MCU4520ConferenceCreate
    {
        public static XmlRpcResponse Execute(MCUCredentials credentials, MCU4520Conference conference)
        {
            XmlRpcRequest request = new XmlRpcRequest("conference.create");

            Dictionary<string, object> confParams = new Dictionary<string, object>();
            confParams.Add("authenticationUser", credentials.Username);
            confParams.Add("authenticationPassword", credentials.Password);

            confParams.Add("conferenceName", conference.Name);
            confParams.Add("description", conference.Description);
            confParams.Add("numericId", conference.NumericId);
            confParams.Add("registerWithSIPRegistrar", conference.RegisterWithSIPRegistrar);
            confParams.Add("startTime", conference.StartTime);
            confParams.Add("durationSeconds", conference.DurationSeconds);
            confParams.Add("preconfiguredParticipantsDefer", conference.PreconfiguredParticipantsDefer);

            request.AddParam(confParams);

            XmlRpcClient client = new XmlRpcClient();
            client.Url = credentials.URI;

            XmlRpcResponse responseCreate = client.Execute(request);

            return responseCreate;
        }
    }
}

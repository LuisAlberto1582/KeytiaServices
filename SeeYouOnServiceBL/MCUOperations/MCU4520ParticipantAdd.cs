using KeytiaServiceBL.XmlRpc;
using SeeYouOnServiceBL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeeYouOnServiceBL.MCUOperations
{
    public static class MCU4520ParticipantAdd
    {
        public static XmlRpcResponse Execute(MCUCredentials credentials, string conferenceName, MCU4520Participant participant)
        {
            XmlRpcRequest request = new XmlRpcRequest("participant.add");

            Dictionary<string, object> participantParams = new Dictionary<string, object>();
            participantParams.Add("authenticationUser", credentials.Username);
            participantParams.Add("authenticationPassword", credentials.Password);

            participantParams.Add("conferenceName", conferenceName);
            participantParams.Add("participantName", participant.ParticipantName);
            participantParams.Add("participantProtocol", participant.ParticipantProtocol);
            participantParams.Add("participantType", participant.ParticipantType);
            participantParams.Add("address", participant.Address);

            request.AddParam(participantParams);


            XmlRpcClient client = new XmlRpcClient();
            client.Url = credentials.URI;
            XmlRpcResponse responseAdd = client.Execute(request);
            return responseAdd;
        }
    }
}

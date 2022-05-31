using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Collections;
using System.Data;

using KeytiaServiceBL;
using SeeYouOnServiceBL.XmlRpc;

namespace SeeYouOnServiceBL
{
    public class MCU
    {
        protected object poLock = new object();
        protected KDBAccess kdb = new KDBAccess();
        protected int piLastUsedMCU = int.MinValue;

        /// <summary>
        /// Guardar la conferencia en el MCU
        /// </summary>
        /// <param name="liCodConf">iCodCatalogo de la Conferencia</param>
        public void SaveConferenceSYO2MCU(int liCodConf)
        {
            try
            {
                //TODO: Obtener el TMS de forma dinámica
                //TMS
                //Se busca el servidor TMS, esta parte esta fija ya que se trae todos los historicos de la entidad
                DataRow ldrTMS = KDBUtil.SearchHistoricRow("ServidorTMS", "Servidor TMS",
                    new string[] { "iCodCatalogo" }, "");

                //Si no existen registros, entonces manda una excepcion notificando que no existe el TMS
                if (ldrTMS == null)
                    throw new Exception("No se encontró servidor TMS para la conferencia '" + liCodConf + "'");


                //Conferencia
                //Extrae la configuracion de la conferencia a crear en el MCU
                DataRow ldrConf = KDBUtil.SearchHistoricRow("TMSConf", "Conferencia",
                    new string[] { "iCodRegistro", "iCodCatalogo", "{TMSSystems}", "{ConfNumericId}",
                    "{FechaInicioReservacion}", "{FechaFinReservacion}" },
                    "iCodCatalogo = " + liCodConf);

                //Si no logro encontrar la conferencia manda error al log
                if (ldrConf == null)
                    throw new Exception("No se encontró la conferencia '" + liCodConf + "'");


                //Participantes
                //Extraer los participantes de la conferencia
                DataTable ldtPart = kdb.GetHisRegByEnt("Participante", "Participante",
                    new string[] { "iCodCatalogo", "{TMSPhoneBookContact}", "{Address}" },
                    "{TMSConf} = " + liCodConf);

                //Si no se encuentran los participantes entonces
                if (ldtPart == null || ldtPart.Rows.Count == 0)
                    throw new Exception("No se encontraron participantes para la conferencia '" + liCodConf + "'");

                //Extraer el NumericId de la conferencia
                string lsNumericId = (string)Util.IsDBNull(ldrConf["{ConfNumericId}"], "");
                //Extraer el iCodCatalogo del ServidorTMS
                int liCodTMS = (int)ldrTMS["iCodCatalogo"];
                //Extraer el iCodCatalogo del TMS Systems o MCU
                int liCodMCU = (int)Util.IsDBNull(ldrConf["{TMSSystems}"], int.MinValue);

                Hashtable lhtVal = new Hashtable();
                MCU loMCU = new MCU();
                Dictionary<string, Struct> lstPart = null;
                int liEstConferencia = 0;
                //Si el numericId es vacio o nulo
                if (string.IsNullOrEmpty(lsNumericId))
                {
                    //Mandar guardar la conferencia
                    lsNumericId = loMCU.SaveConference(
                        liCodTMS,
                        ldtPart.Rows.Count,
                        (DateTime)Util.IsDBNull(ldrConf["{FechaInicioReservacion}"], DateTime.MinValue),
                        (DateTime)Util.IsDBNull(ldrConf["{FechaFinReservacion}"], DateTime.MinValue));

                    //Extraer el iCodCatalogo del Ultimo MCU utilizado
                    liCodMCU = loMCU.GetLastUsedMCU();

                    //Limpia el hash y agrega los siguientes elementos
                    lhtVal.Clear();
                    lhtVal.Add("iCodRegistro", ldrConf["iCodRegistro"]);
                    lhtVal.Add("{ConfNumericId}", lsNumericId);
                    lhtVal.Add("{TMSSystems}", liCodMCU);
                    //Guarda el historico, de la nueva conferencia.
                    KDBUtil.SaveHistoric("TMSConf", "Conferencia", (string)ldrConf["vchCodigo"], null, lhtVal);
                    //Extrae el iCodCatalogo del estatus de conferencia "Programada"
                    liEstConferencia = KDBUtil.SearchICodCatalogo("EstConferencia", "Programada", true);
                }
                else
                {
                    //Se guarda la conferencia, en caso de ser una existente, cambiara estatus a Re-Programada
                    lsNumericId = loMCU.SaveConference(
                        liCodTMS, lsNumericId,
                        (DateTime)Util.IsDBNull(ldrConf["{FechaInicioReservacion}"], DateTime.MinValue),
                        (DateTime)Util.IsDBNull(ldrConf["{FechaFinReservacion}"], DateTime.MinValue),
                        liCodMCU); //TODO: Ahorita se asume el mismo MCU
                    //Extrer el icodcatalogo estatus de conferencia "ReProgramada"
                    liEstConferencia = KDBUtil.SearchICodCatalogo("EstConferencia", "ReProgramada", true);
                    //Obtiene los participantes de la conferencia en el MCU
                    lstPart = loMCU.GetParticipants(lsNumericId, liCodMCU);
                }

                //Ciclo para recorrer cada participante en la conferencia RZ.20131018
                foreach (DataRow ldrPart in ldtPart.Rows)
                {
                    try
                    {
                        //Contacto
                        
                        DataRow ldrContact = KDBUtil.SearchHistoricRow("TMSPhoneBookContact", "PhoneBook",
                            new string[] { "vchDescripcion" },
                            "iCodCatalogo = " + (int)Util.IsDBNull(ldrPart["{TMSPhoneBookContact}"], int.MinValue));

                        if (ldrContact == null)
                            throw new Exception("No se encontró el contacto '" + ldrPart["{TMSPhoneBookContact}"] + "'");

                        if (string.IsNullOrEmpty((string)Util.IsDBNull(ldrContact["vchDescripcion"], "")))
                            throw new Exception("El contacto '" + ldrPart["{TMSPhoneBookContact}"] + "' tiene nombre inválido");


                        //Dirección
                        DataRow ldrAddress = KDBUtil.SearchHistoricRow("Address", "Address",
                            new string[] { "{SystemAddress}", "{MCUCallProtocol}" },
                            "iCodCatalogo = " + (int)Util.IsDBNull(ldrPart["{Address}"], int.MinValue));

                        if (ldrAddress == null)
                            throw new Exception("No se encontró la direccion '" + ldrPart["{Address}"] + "'");

                        if (string.IsNullOrEmpty((string)Util.IsDBNull(ldrAddress["{SystemAddress}"], "")))
                            throw new Exception("El participante '" + ldrPart["iCodCatalogo"] + "' tiene dirección inválida");


                        //Protocolo
                        string lsProtocol = (String)Util.IsDBNull(KDBUtil.SearchScalar("MCUCallProtocol",
                            (int)Util.IsDBNull(ldrAddress["{MCUCallProtocol}"], int.MinValue),
                            "vchCodigo", true), "SIP");

                        string lsProtocolMCU = (lsProtocol == "SIP" ? "sip" : (lsProtocol == "H.323" ? "h323" : (lsProtocol == "VNC" ? "vnc" : "")));


                        //Guarda el participante en el MCU
                        if (lstPart != null && lstPart.ContainsKey(lsProtocolMCU + ":" + (string)ldrAddress["{SystemAddress}"]))
                            ((Struct)lstPart[lsProtocolMCU + ":" + (string)ldrAddress["{SystemAddress}"]])["MCUSaved"] = true;
                        else
                        {
                            loMCU.SaveParticipant(lsNumericId,
                                (string)ldrContact["vchDescripcion"],
                                (string)ldrAddress["{SystemAddress}"],
                                lsProtocolMCU, liCodMCU);
                        }
                    }
                    catch (Exception ex)
                    {
                        Util.LogException("No se pudo agendar el participante '" + ldrPart["iCodCatalogo"] + "' en la conferencia '" + liCodConf + "'", ex);
                        throw ex;
                    }
                }

                //Borra los participantes de la conferencia que ya no están configurados en SeeYouON
                if (lstPart != null)
                    foreach (Struct loPart in lstPart.Values)
                        if (loPart["MCUSaved"] == null || (bool)loPart["MCUSaved"] == false)
                            loMCU.DeleteParticipant(lsNumericId, (string)loPart["participantName"], (string)loPart["participantProtocol"], liCodMCU);

                //Actualiza el estatus de la conferencia
                lhtVal.Clear();
                lhtVal.Add("iCodRegistro", ldrConf["iCodRegistro"]);
                lhtVal.Add("{EstConferencia}", liEstConferencia);
                KDBUtil.SaveHistoric("TMSConf", "Conferencia", (string)ldrConf["vchCodigo"], null, lhtVal);
            }
            catch (Exception ex)
            {
                Util.LogException("No se pudo agendar la conferencia '" + liCodConf + "'", ex);
                throw ex;
            }
        }

        public void DeleteConferenceSYO2MCU(int liCodConf)
        {
            try
            {
                //Conferencia
                DataRow ldrConf = KDBUtil.SearchHistoricRow("TMSConf", "Conferencia",
                    new string[] { "iCodRegistro", "{TMSSystems}", "{ConfNumericId}" },
                    "iCodCatalogo = " + liCodConf);

                if (ldrConf == null)
                    throw new Exception("No se encontró la conferencia '" + liCodConf + "'");


                Hashtable lhtVal = new Hashtable();
                MCU loMCU = new MCU();

                if ((string)Util.IsDBNull(ldrConf["{ConfNumericId}"], "") != "" &&
                    (int)Util.IsDBNull(ldrConf["{TMSSystems}"], "") != int.MinValue)
                    loMCU.DeleteConference((string)ldrConf["{ConfNumericId}"], (int)ldrConf["{TMSSystems}"]);

                lhtVal.Clear();
                lhtVal.Add("iCodRegistro", ldrConf["iCodRegistro"]);
                lhtVal.Add("{EstConferencia}", KDBUtil.SearchICodCatalogo("EstConferencia", "Eliminada", true));
                KDBUtil.SaveHistoric("TMSConf", "Conferencia", (string)ldrConf["vchCodigo"], null, lhtVal);
            }
            catch (Exception ex)
            {
                Util.LogException("No se pudo eliminar la conferencia '" + liCodConf + "'", ex);
                throw ex;
            }
        }

        /// <summary>
        /// Guarda la conferencia, manda a llamar otro metodo con el lsNumericId vacio
        /// </summary>
        /// <param name="liCodTMS">iCodCatalogo del servidor TMS</param>
        /// <param name="liPartCount">Numero de Participantes</param>
        /// <param name="ldtStart">Hora de inicio</param>
        /// <param name="ldtEnd">Hora de fin</param>
        /// <returns></returns>
        public string SaveConference(int liCodTMS, int liPartCount, DateTime ldtStart, DateTime ldtEnd)
        {
            //TODO: buscar el MCU donde se grabará la conferencia
            //Extrer el mejor MCU
            int liCodMCU = GetBestMCU(liCodTMS, liPartCount, ldtStart, ldtEnd);
            //Manda a guardar la conferencia
            return SaveConference(liCodTMS, "", ldtStart, ldtEnd, liCodMCU);
        }

        /// <summary>
        /// Guarda una conferencia en el TMS
        /// </summary>
        /// <param name="liCodTMS">iCodCatalogo del TMS</param>
        /// <param name="lsNumericId">Numeric Id</param>
        /// <param name="ldtStart">Fecha Inicio</param>
        /// <param name="ldtEnd">Fecha Fin</param>
        /// <param name="liCodMCU">iCodCatalogo del MCU</param>
        /// <returns></returns>
        public string SaveConference(int liCodTMS, string lsNumericId, DateTime ldtStart, DateTime ldtEnd, int liCodMCU)
        {
            //Almacena como ultimo MCU el enviado como parametro
            piLastUsedMCU = liCodMCU;

            try
            {
                bool lbCreate = false;
                TimeSpan loDuration = TimeSpan.MinValue;

                MethodCall loCall = GetCallObject(liCodMCU);
                MethodResponse loResp;

                //Conferencia nueva
                if (string.IsNullOrEmpty(lsNumericId))
                {
                    lsNumericId = GetNewId(liCodTMS);
                    lbCreate = true;
                }
                else //Modificacion
                {
                    loResp = GetConference(lsNumericId, liCodMCU);

                    //Existe? la modifica
                    if (loResp.Fault == null)
                        lbCreate = false;
                    else if ((int)loResp.Fault["faultCode"] == 4) //No existe? la crea
                        lbCreate = true;
                    else //Otro error?
                        throw new MCUException("Surgió un error al consultar la conferencia '" + lsNumericId + "' en el MCU '" + liCodMCU + "'",
                            (int)loResp.Fault["faultCode"], (string)loResp.Fault["faultString"]);
                }

                loCall.Clear();

                if (ldtStart != DateTime.MinValue && ldtEnd != DateTime.MinValue)
                    loDuration = new TimeSpan(ldtEnd.Ticks - ldtStart.Ticks);

                if (lbCreate)
                {
                    loCall.MethodName = "conference.create";
                    loCall.Param["conferenceName"] = GetConferenceName(lsNumericId);
                    loCall.Param["numericId"] = lsNumericId;
                    loCall.Param["registerWithSIPRegistrar"] = true;
                    //loCall.Param["registerWithGatekeeper"] = true;
                }
                else
                {
                    loCall.MethodName = "conference.modify";
                    loCall.Param["conferenceName"] = GetConferenceName(lsNumericId);
                }

                if (ldtStart != DateTime.MinValue)
                    loCall.Param["startTime"] = ldtStart;

                if (loDuration != TimeSpan.MinValue)
                    loCall.Param["durationSeconds"] = int.Parse(loDuration.TotalSeconds.ToString());

                loResp = loCall.Call();

                //Verifica si hubo error
                if (loResp.Fault != null)
                    throw new MCUException("Surgió un error al grabar la conferencia '" + lsNumericId + "' en el MCU '" + liCodMCU + "'",
                        (int)loResp.Fault["faultCode"], (string)loResp.Fault["faultString"]);
            }
            catch (MCUException ex)
            {
                Util.LogException(ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw ex;
            }

            return lsNumericId;
        }

        public void DeleteConference(int liCodTMS, string lsNumericId)
        {
            int liCodMCU = GetMCUForConference(liCodTMS, lsNumericId); //TODO: buscar el mcu donde se encuentra el NumericId
            DeleteConference(lsNumericId, liCodMCU);
        }

        public void DeleteConference(string lsNumericId, int liCodMCU)
        {
            try
            {
                MethodCall loCall = GetCallObject(liCodMCU);
                MethodResponse loResp;

                loCall.MethodName = "conference.destroy";
                loCall.Param["conferenceName"] = GetConferenceName(lsNumericId);
                loResp = loCall.Call();

                //Verifica si hubo error
                if (loResp.Fault != null)
                    throw new MCUException("Surgió un error al grabar la conferencia '" + lsNumericId + "' en el MCU '" + liCodMCU + "'",
                        (int)loResp.Fault["faultCode"], (string)loResp.Fault["faultString"]);
            }
            catch (MCUException ex)
            {
                Util.LogException(ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw ex;
            }
        }

        public void SaveParticipant(string lsNumericId, string lsName, string lsAddress, string lsProtocol, int liCodMCU)
        {
            try
            {
                MethodCall loCall = GetCallObject(liCodMCU);
                MethodResponse loResp;

                loCall.MethodName = "participant.add";
                loCall.Param["conferenceName"] = GetConferenceName(lsNumericId);
                loCall.Param["displayNameOverrideStatus"] = true;
                loCall.Param["displayNameOverrideValue"] = lsName;
                loCall.Param["participantName"] = lsAddress;
                loCall.Param["participantType"] = "by_address";
                loCall.Param["participantProtocol"] = lsProtocol;
                loCall.Param["address"] = lsAddress;
                loCall.Param["useSIPRegistrar"] = true;
                loResp = loCall.Call();

                //Verifica si hubo error
                if (loResp.Fault != null && (int)loResp.Fault["faultCode"] != 3)
                    throw new MCUException("Surgió un error al grabar el participante '" + lsName + " " + lsProtocol + " " +
                        lsAddress + "' en la conferencia '" + lsNumericId + "' en el MCU '" + liCodMCU + "'",
                        (int)loResp.Fault["faultCode"], (string)loResp.Fault["faultString"]);
            }
            catch (MCUException ex)
            {
                Util.LogException(ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw ex;
            }
        }
        public void SaveParticipant(string lsConferenceName, string lsNumericId, string lsName, string lsAddress, string lsProtocol, int liCodMCU)
        {
            try
            {
                MethodCall loCall = GetCallObject(liCodMCU);
                MethodResponse loResp;

                loCall.MethodName = "participant.add";
                loCall.Param["conferenceName"] = lsConferenceName;
                loCall.Param["displayNameOverrideStatus"] = true;
                loCall.Param["displayNameOverrideValue"] = lsName;
                loCall.Param["participantName"] = lsAddress;
                loCall.Param["participantType"] = "by_address";
                loCall.Param["participantProtocol"] = lsProtocol;
                loCall.Param["address"] = lsAddress;
                loCall.Param["useSIPRegistrar"] = true;
                loResp = loCall.Call();

                //Verifica si hubo error
                if (loResp.Fault != null)
                {
                    if ((int)loResp.Fault["faultCode"] == 3) //Ya esta el participante se reconectara
                    {
                        loCall.Clear();
                        loCall.MethodName = "participant.connect";
                        loCall.Param["conferenceName"] = lsConferenceName;
                        loCall.Param["participantName"] = lsAddress;
                        loCall.Param["participantProtocol"] = lsProtocol;
                        loCall.Param["participantType"] = "by_address";
                        loResp = loCall.Call();

                        //Verifica si hubo error
                        if (loResp.Fault != null)
                            throw new MCUException("Surgió un error al grabar el participante '" + lsName + " " + lsProtocol + " " +
                            lsAddress + "' en la conferencia '" + lsConferenceName + "' en el MCU '" + liCodMCU + "'",
                            (int)loResp.Fault["faultCode"], (string)loResp.Fault["faultString"]);

                    }
                    else
                    {
                        throw new MCUException("Surgió un error al grabar el participante '" + lsName + " " + lsProtocol + " " +
                         lsAddress + "' en la conferencia '" + lsConferenceName + "' en el MCU '" + liCodMCU + "'",
                            (int)loResp.Fault["faultCode"], (string)loResp.Fault["faultString"]);
                    }
                }
            }
            catch (MCUException ex)
            {
                Util.LogException(ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw ex;
            }
        }

        //public void DeleteParticipant(int liCodTMS, string lsNumericId, int liCodSystem)
        //{
        //    int liCodMCU = GetMCUForConference(liCodTMS, lsNumericId); //TODO: buscar el MCU que contiene la conferencia
        //    DeleteParticipant(lsNumericId, liCodSystem, liCodMCU);
        //}

        public void DeleteParticipant(string lsNumericId, string lsName, string lsProtocol, int liCodMCU)
        {
            try
            {
                MethodCall loCall = GetCallObject(liCodMCU);
                MethodResponse loResp;

                loCall.MethodName = "participant.remove";
                loCall.Param["conferenceName"] = GetConferenceName(lsNumericId);
                loCall.Param["participantName"] = lsName;
                loCall.Param["participantType"] = "by_address";
                loCall.Param["participantProtocol"] = lsProtocol;
                loResp = loCall.Call();

                //Verifica si hubo error
                if (loResp.Fault != null)
                    throw new MCUException("Surgió un error al borrar el participante '" + lsName + " " + lsProtocol +
                    "' en la conferencia '" + lsNumericId + "' en la conferencia '" + lsNumericId + "' en el MCU '" + liCodMCU + "'",
                    (int)loResp.Fault["faultCode"], (string)loResp.Fault["faultString"]);
            }
            catch (MCUException ex)
            {
                Util.LogException(ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw ex;
            }
        }
        public void DeleteParticipant(string lsConferenceName, string lsNumericId, string lsName, string lsProtocol, int liCodMCU)
        {
            try
            {
                MethodCall loCall = GetCallObject(liCodMCU);
                MethodResponse loResp;

                loCall.MethodName = "participant.remove";
                loCall.Param["conferenceName"] = lsConferenceName;
                loCall.Param["participantName"] = lsName;
                loCall.Param["participantType"] = "by_address";
                loCall.Param["participantProtocol"] = lsProtocol;
                loResp = loCall.Call();

                //Verifica si hubo error
                if (loResp.Fault != null)
                    if ((int)loResp.Fault["faultCode"] != 5) //Ya No existe el participante a  remover
                        throw new MCUException("Surgió un error al borrar el participante '" + lsName + " " + lsProtocol +
                            "' en la conferencia '" + lsConferenceName + "' en el MCU '" + liCodMCU + "'",
                            (int)loResp.Fault["faultCode"], (string)loResp.Fault["faultString"]);
            }
            catch (MCUException ex)
            {
                Util.LogException(ex);
                throw ex;
            }
            catch (Exception ex)
            {
                Util.LogException(ex);
                throw ex;
            }
        }

        public string GetConferenceName(string lsNumericId)
        {
            return "SeeYouOn " + lsNumericId;
        }

        public MethodResponse GetConference(int liCodTMS, string lsNumericId)
        {
            int liCodMCU = GetMCUForConference(liCodTMS, lsNumericId); //TODO: Buscar el MCU donde se encuentra la conferencia
            return GetConference(lsNumericId, liCodMCU);
        }

        public MethodResponse GetConference(string lsNumericId, int liCodMCU)
        {
            MethodCall loCall = GetCallObject(liCodMCU);
            MethodResponse loResp;

            loCall.MethodName = "conference.status";
            loCall.Param["conferenceName"] = GetConferenceName(lsNumericId);

            loResp = loCall.Call();

            return loResp;
        }

        public int GetLastUsedMCU()
        {
            return piLastUsedMCU;
        }

        public int GetMCUForConference(int liCodTMS, string lsNumericId)
        {
            //TODO: Buscar MCU por orden y espacio disponible
            DataRow ldrConf = KDBUtil.SearchHistoricRow("TMSConf", "Conferencia",
                new string[] { "{TMSSystems}" },
                "{ServidorTMS} = " + liCodTMS + " and {ConfNumericId} = '" + lsNumericId + "'");

            if (ldrConf == null)
                throw new Exception("No hay MCU configurado para la conferencia con NumericId '" + lsNumericId + "'");

            return (int)ldrConf["{TMSSystems}"];
        }

        /// <summary>
        /// Obtener el MCU disponible
        /// </summary>
        /// <param name="liCodTMS">iCodCatalogo del TMS</param>
        /// <param name="liPartCount">Conteo de participantes</param>
        /// <param name="ldtStart">Hora inicio</param>
        /// <param name="ldtEnd">Hora Fin</param>
        /// <returns></returns>
        public int GetBestMCU(int liCodTMS, int liPartCount, DateTime ldtStart, DateTime ldtEnd)
        {
            //TODO: Buscar MCU por orden y espacio disponible
            DataTable ldtMCU = kdb.GetHisRegByEnt("TMSSystems", "MCU",
                new string[] { "iCodCatalogo", "{NumPuertos}", "{OrdenPre}" },
                "{ServidorTMS} = " + liCodTMS + " and isnull({URLXmlRpc}, '') <> ''", "{OrdenPre} asc");

            //Si no existe el MCU, entonces arroja una excepcion
            if (ldtMCU == null || ldtMCU.Rows.Count == 0)
                throw new Exception("No hay MCU configurado para llamadas XML-RPC");

            int liMaxPorts = 0;
            int liAvailablePorts = 0;
            foreach (DataRow ldrMCU in ldtMCU.Rows)
            {
                //Cantidad maxima de puertos
                liMaxPorts = (int)Util.IsDBNull(ldrMCU["{NumPuertos}"], 0);
                //Si la cantidad de participantes es menor o igual a la cantidad maxima de puertos
                if (liPartCount <= liMaxPorts)
                {
                    //Obtener la cantidad de puertos disponibles
                    liAvailablePorts = liMaxPorts - GetUsedPorts(ldtStart, ldtEnd, (int)(ldrMCU["iCodCatalogo"]));
                    //Si la cantidad de puertos disponibles es mayor o igual a los que se necesitan en la conferencia entonces...
                    if (liAvailablePorts >= liPartCount)
                    {
                        return (int)ldrMCU["iCodCatalogo"]; //Regresa el MCU disponible
                    }
                }
            }

            throw new Exception("No hay MCU disponible"); //Si no se encontro un mcu disponible arroja una excepcion
        }

        public MethodCall GetCallObject(int liCodMCU)
        {
            DataRow ldrMCU = KDBUtil.SearchHistoricRow("TMSSystems", "MCU",
                new string[] { "{URLXmlRpc}", "{UsuarioMCU}", "{Password}" },
                "iCodCatalogo = " + liCodMCU);

            if (ldrMCU == null)
                throw new Exception("No fue posible obtener la configuración de XML-RPC del MCU '" + liCodMCU + "'");

            return new MethodCall(
                (string)Util.IsDBNull(ldrMCU["{URLXmlRpc}"], ""),
                (string)Util.IsDBNull(ldrMCU["{UsuarioMCU}"], ""),
                Util.Decrypt((string)Util.IsDBNull(ldrMCU["{Password}"], "")));
        }

        public string GetNewId(int liCodTMS)
        {
            int liRet = int.MinValue;

            lock (poLock)
            {
                DataRow ldrTMS = KDBUtil.SearchHistoricRow("ServidorTMS", "Servidor TMS",
                    new string[] { "iCodRegistro", "vchCodigo", "{NumericIdIni}", "{NumericIdFin}", "{NumericIdSig}" },
                    "iCodCatalogo = " + liCodTMS);

                if (ldrTMS == null)
                    throw new Exception("No fue posible obtener el NumericID siguiente para una conferencia del TMS '" + liCodTMS + "'");

                liRet = Convert.ToInt32(ldrTMS["{NumericIdSig}"]);

                Hashtable lhtVal = new Hashtable();
                lhtVal.Add("iCodRegistro", ldrTMS["iCodRegistro"]);
                lhtVal.Add("{NumericIdSig}", (liRet + 1 > Convert.ToInt32(ldrTMS["{NumericIdFin}"]) ? ldrTMS["{NumericIdIni}"] : liRet + 1));

                KDBUtil.SaveHistoric("ServidorTMS", "Servidor TMS", (string)ldrTMS["vchCodigo"], null, lhtVal);
            }

            return liRet.ToString();
        }

        public Dictionary<string, Struct> GetParticipants(int liCodTMS, string lsNumericId)
        {
            int liCodMCU = GetMCUForConference(liCodTMS, lsNumericId);
            return GetParticipants(lsNumericId, liCodMCU);
        }

        public Dictionary<string, Struct> GetParticipants(string lsConfName, string lsNumericId, int liCodMCU)
        {
            Dictionary<string, Struct> lstRet = new Dictionary<string, Struct>();

            MethodCall loCall = GetCallObject(liCodMCU);
            MethodResponse loResp = null;

            do
            {
                loCall.MethodName = "participant.enumerate";

                loCall.Param["operationScope"] = new ValueArray();
                ((ValueArray)loCall.Param["operationScope"]).Add(new Value("configuredState"));

                if (loResp != null && loResp.Param["enumerateID"] != null)
                    loCall.Param["enumerateID"] = loResp.Param["enumerateID"];

                loResp = loCall.Call();

                foreach (Value loPart in ((ValueArray)loResp.Param["participants"]).Values)
                    if ((string)loPart["conferenceName"] == lsConfName)
                        lstRet.Add(loPart["participantProtocol"] + ":" + loPart["participantName"], loPart.ObjectStruct);

            } while (loResp.Param["enumerateID"] != null);

            return lstRet;
        }
        public Dictionary<string, Struct> GetParticipants(string lsNumericId, int liCodMCU)
        {
            Dictionary<string, Struct> lstRet = new Dictionary<string, Struct>();
            string lsConfName = GetConferenceName(lsNumericId);

            MethodCall loCall = GetCallObject(liCodMCU);
            MethodResponse loResp = null;

            do
            {
                loCall.MethodName = "participant.enumerate";

                loCall.Param["operationScope"] = new ValueArray();
                ((ValueArray)loCall.Param["operationScope"]).Add(new Value("configuredState"));

                if (loResp != null && loResp.Param["enumerateID"] != null)
                    loCall.Param["enumerateID"] = loResp.Param["enumerateID"];

                loResp = loCall.Call();

                foreach (Value loPart in ((ValueArray)loResp.Param["participants"]).Values)
                    if ((string)loPart["conferenceName"] == lsConfName)
                        lstRet.Add(loPart["participantProtocol"] + ":" + loPart["participantName"], loPart.ObjectStruct);

            } while (loResp.Param["enumerateID"] != null);

            return lstRet;
        }

        /// <summary>
        /// Obtener los puertos usados en el MCU, envia como int.minvalue el icodcatalogo de la conferencia
        /// </summary>
        /// <param name="ldtInicio">Fecha inicio</param>
        /// <param name="ldtFin">Fecha Fin</param>
        /// <param name="liCodMCU">iCodCatalogo del MCU</param>
        /// <returns></returns>
        public int GetUsedPorts(DateTime ldtInicio, DateTime ldtFin, int liCodMCU)
        {
            return GetUsedPorts(ldtInicio, ldtFin, int.MinValue, liCodMCU);
        }

        /// <summary>
        /// Obtener los puertos usados en el MCU
        /// </summary>
        /// <param name="ldtInicio">Fecha inicio</param>
        /// <param name="ldtFin">Fecha fin</param>
        /// <param name="liCodConf">iCodCatalogo de la conferencia</param>
        /// <param name="liCodMCU">iCodCatalogo del MCU</param>
        /// <returns></returns>
        public int GetUsedPorts(DateTime ldtInicio, DateTime ldtFin, int liCodConf, int liCodMCU)
        {
            int liRet = 0;

            //Estatus de la conferencia Eliminada
            int liEstConferencia = (int)Util.IsDBNull(KDBUtil.SearchScalar("EstConferencia", "Eliminada",
                "iCodCatalogo", true), 0);

            string lsdtInicio = ldtInicio.ToString("yyyy-MM-dd HH:mm:ss");
            string lsdtFin = ldtFin.ToString("yyyy-MM-dd HH:mm:ss");

            //Buscar todas las conferencia que se traslapen activas
            DataTable ldtConf = kdb.GetHisRegByEnt("TMSConf", "Conferencia",
                new string[] { "iCodCatalogo", "{TMSSystems}", "{FechaInicioReservacion}", "{FechaFinReservacion}" },
                "{TMSSystems} = " + liCodMCU + " and " +
                " ( '" + lsdtInicio + "' Between {FechaInicioReservacion} and {FechaFinReservacion} " +
                " or '" + lsdtFin + "' Between {FechaInicioReservacion} and {FechaFinReservacion} " +
                " or ( {FechaInicioReservacion} >= '" + lsdtInicio + "' and {FechaFinReservacion} <= '" + lsdtFin + "'))" +
                " and {EstConferencia} <> " + liEstConferencia,
                " {FechaInicioReservacion} desc");

            if (ldtConf == null || ldtConf.Rows.Count == 0)
                return liRet;

            //Crear una tabla con las conferencias y el numero de participantes
            DataTable ldtConfPart = new DataTable();
            DataRow ldrPart;
            int liPart;

            //Si la tabla de conferencias activas con traslape es 1
            if (ldtConf.Rows.Count == 1)
            {
                //Buscar la cantidad de participantes
                ldrPart = KDBUtil.SearchHistoricRow("Participante", "Participante",
                            new string[] { "iCodRegistro", "vchCodigo" },
                            "{TMSConf} = " + ldtConf.Rows[0]["iCodCatalogo"]);
                //Guarda la cantidad de participantes, y lo regresa con el return
                liRet = ldrPart.Table.Rows.Count;
                return liRet;
            }

            //Agregar las siguienteas columnas a la tabla de Participantes en conferencias
            ldtConfPart.Columns.Add(new DataColumn("TMSConf", typeof(int)));
            ldtConfPart.Columns.Add(new DataColumn("dtInicio", typeof(DateTime)));
            ldtConfPart.Columns.Add(new DataColumn("dtFin", typeof(DateTime)));
            ldtConfPart.Columns.Add(new DataColumn("NumPart", typeof(int)));

            //Inicia un ciclo recorriendo todas las conferencias que se translapan
            foreach (DataRow ldrConf in ldtConf.Rows)
            {
                //Se buscan la cantidad de participantes de la conferencia en curso
                ldrPart = KDBUtil.SearchHistoricRow("Participante", "Participante",
                    new string[] { "iCodRegistro", "vchCodigo" },
                    "{TMSConf} = " + ldrConf["iCodCatalogo"]);

                liPart = ldrPart.Table.Rows.Count;

                //Se agrega una nueva fila al DataTable ldtConfPart
                DataRow lDataRow = ldtConfPart.NewRow();
                lDataRow["TMSConf"] = ldrConf["iCodCatalogo"];
                lDataRow["dtInicio"] = ldrConf["{FechaInicioReservacion}"];
                lDataRow["dtFin"] = ldrConf["{FechaFinReservacion}"];
                lDataRow["NumPart"] = liPart;
                ldtConfPart.Rows.Add(lDataRow);
            }

            //Obtener el numero maximo de participantes de todos los periodos que se traslapan
            DataRow[] ldtFragmentos = ldtConfPart.Select();
            DataRow[] ldrParts;
            int liPartFrag = 0;
            string lsdtIni = "";
            
            //Ciclo que recorre el arreglo de DataRows derivado de las conferencias que se traslapan
            foreach (DataRow ldrConf in ldtFragmentos)
            {

                lsdtIni = ((DateTime)ldrConf["dtInicio"]).ToString("yyyy-MM-dd HH:mm:ss");
                //Extraer las conferencias en las que no se traslapa el horario
                ldrParts = ldtConfPart.Select("dtInicio <= '" + lsdtIni + "' And dtFin > '" + lsdtIni + "' " +
                            " And TMSConf <> " + ldrConf["TMSConf"]);

                //Extraer el numero de participantes
                liPartFrag = (int)ldrConf["NumPart"];
                //Si se encontraron conferencias...
                if (ldrParts.Length > 0)
                {
                    //Por cada datarow
                    foreach (DataRow lDataRow in ldrParts)
                    {
                        //Extramos el numero de participantes
                        liPart = (int)lDataRow["NumPart"];
                        //Se acumulan los participantes
                        liPartFrag += liPart;
                    }
                }
                //Si la cantidad participantes es mayor al valor de retorno entonces reemplazo su valor
                if (liPartFrag > liRet)
                    liRet = liPartFrag;
            }

            return liRet;
        }

        public int GetAvailablePorts(DateTime ldtInicio, DateTime ldtFin, int liCodConf, int liCodMCU)
        {
            int liRet = 0;
            int liMaxPorts = 0;

            DataTable ldtMCU = kdb.GetHisRegByEnt("TMSSystems", "MCU",
                        new string[] { "iCodCatalogo", "{NumPuertos}" }, "iCodCatalogo = " + liCodMCU);

            liMaxPorts = (int)Util.IsDBNull(ldtMCU.Rows[0]["{NumPuertos}"], 0);

            //Puertos del MCU - GetUsedPorts
            liRet = liMaxPorts - GetUsedPorts(ldtInicio, ldtFin, liCodConf, liCodMCU);

            return liRet;

        }
        public int GetAvailablePorts(int liCodConf)
        {
            int liRet = 0;

            //Conferencia
            DataRow ldrConf = KDBUtil.SearchHistoricRow("TMSConf", "Conferencia",
                new string[] { "iCodRegistro", "iCodCatalogo", "{TMSSystems}", "{ConfNumericId}",
                    "{FechaInicioReservacion}", "{FechaFinReservacion}" },
                "iCodCatalogo = " + liCodConf);
            if (ldrConf == null)
                throw new Exception("No se encontró la conferencia '" + liCodConf + "'");

            int liCodMCU = (int)Util.IsDBNull(ldrConf["{TMSSystems}"], int.MinValue);
            DateTime ldtInicio = (DateTime)Util.IsDBNull(ldrConf["{FechaInicioReservacion}"], DateTime.MinValue);
            DateTime ldtFin = (DateTime)Util.IsDBNull(ldrConf["{FechaFinReservacion}"], DateTime.MinValue);

            DataTable ldtMCU = kdb.GetHisRegByEnt("TMSSystems", "MCU",
                        new string[] { "iCodCatalogo", "{NumPuertos}" }, "iCodCatalogo = " + liCodMCU);

            int liMaxPorts = (int)Util.IsDBNull(ldtMCU.Rows[0]["{NumPuertos}"], 0);

            //Puertos del MCU - GetUsedPorts
            liRet = liMaxPorts - GetUsedPorts(ldtInicio, ldtFin, liCodConf, liCodMCU);

            return liRet;

        }


        #region SYO Conferences y Salas Virtuales MCU
        /// <summary>
        /// Método para iniciar o terminar las conferencias que tendrán lugar en salas virtuales
        /// </summary>
        public void SyncSYOConferencesMCU()
        {
            //Eliminar las conferencias que ya pasaron y eliminar los participantes para las conferencias
            // de salas virtuales
            SYOConferenceMCUFinaliza();

            //Inciar las conferencias que ya empezaron y conectar a los participantes para las conferencias
            // de salas virtuales
            SYOConferenceMCUInicia();

        }

        /// <summary>
        /// Método que busca conferencias que ya deben empezar.
        /// </summary>
        public void SYOConferenceMCUInicia()
        {
            DataRow[] laRows = null;
            DataTable ldtSSV = kdb.GetHisRegByEnt("ServicioSeeYouOn", "Sala Virtual",
                                    new string[] { "iCodCatalogo", "vchDescripcion" });

            int liEstProg = KDBUtil.SearchICodCatalogo("EstConferencia", "Programada", true);
            int liEstReProg = KDBUtil.SearchICodCatalogo("EstConferencia", "ReProgramada", true);

            // Busca las conferencias que estan programadas y la fecha actual esté 
            // dentro del rango de la Reservación programada
            DataTable ldtConf = kdb.GetHisRegByEnt("TMSConf", "Conferencia",
                new string[] { "iCodRegistro", "iCodCatalogo", "{ServicioSeeYouOn}" },
                    "{FechaInicioReservacion} < GETDATE() and {FechaFinReservacion} > GETDATE()" +
                    " and {EstConferencia} in (" + liEstProg + "," + liEstReProg + ")");
            int liCodServicio = 0;
            if (ldtConf != null)
                foreach (DataRow ldr in ldtConf.Rows)
                {
                    liCodServicio = (int)Util.IsDBNull(ldr["{ServicioSeeYouOn}"], int.MinValue);
                    if ((laRows = ldtSSV.Select("iCodCatalogo = " + liCodServicio)).Length > 0)
                        SYOConferenceMCU((int)ldr["iCodCatalogo"], (string)laRows[0]["vchDescripcion"]);
                    else
                        SYOConferenceEstatus(ldr, "Iniciada");
                }

        }

        /// <summary>
        /// Método para finalizar las conferencias que tienen estatus 
        /// "Iniciada" y su fecha de fin de reservación (FechaFinReservacion)
        /// es menor a la fecha actual.
        /// </summary>
        public void SYOConferenceMCUFinaliza()
        {
            DataRow[] laRows = null;
            // Obtenemos todas las Salas Virtuales configuradas
            DataTable ldtSSV = kdb.GetHisRegByEnt("ServicioSeeYouOn", "Sala Virtual",
                                    new string[] { "iCodCatalogo", "vchDescripcion" });

            // Obtenemos el estatus de conferencia "Iniciada"
            int liEstIniciada = KDBUtil.SearchICodCatalogo("EstConferencia", "Iniciada", true);

            // Se buscan las conferencias ya iniciadas y que su fecha de fin de reservación es menor a la actual
            DataTable ldtConf = kdb.GetHisRegByEnt("TMSConf", "Conferencia",
                new string[] { "iCodRegistro", "iCodCatalogo", "{ServicioSeeYouOn}", "{TMSSystems}" ,
                        "{ConfNumericId}" },
                        " {FechaFinReservacion} < GETDATE() " +
                        " and {EstConferencia} = " + liEstIniciada);

            int liCodServicio = 0;
            int liCodMCU = 0;
            MCU loMCU = new MCU();

            // Si se encontraron conferencias que eliminar, las recorremos en un ciclo
            if (ldtConf != null)
            {
                foreach (DataRow ldrConf in ldtConf.Rows)
                {
                    try
                    {
                        liCodServicio = (int)Util.IsDBNull(ldrConf["{ServicioSeeYouOn}"], int.MinValue);
                        liCodMCU = (int)Util.IsDBNull(ldrConf["{TMSSystems}"], int.MinValue);
                        if (liCodMCU != int.MinValue)
                        {
                            // Si la conferencia se está efectuando en una Sala Virtual configurada
                            if ((laRows = ldtSSV.Select("iCodCatalogo = " + liCodServicio)).Length > 0)
                            {
                                // Se elimina la conferencia
                                SYOConferenceMCUDel(liCodMCU, (int)ldrConf["iCodCatalogo"], (string)laRows[0]["vchDescripcion"]);
                                // Se actualiza el estatus de la conferencia
                                SYOConferenceEstatus(ldrConf, "Finalizada");
                            }
                            // Si no se está llevando a cabao en una Sala Virtual
                            else
                            {
                                // Si tiene un NumericId
                                if ((string)Util.IsDBNull(ldrConf["{ConfNumericId}"], "") != "")
                                {
                                    try
                                    {
                                        // Se indica al MCU que termine la conferencia
                                        loMCU.DeleteConference((string)ldrConf["{ConfNumericId}"], liCodMCU);
                                    }
                                    catch (Exception ex)
                                    {
                                        Util.LogException(ex);
                                    }
                                }
                                // Se actualiza el estatus de la conferencia
                                SYOConferenceEstatus(ldrConf, "Finalizada");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Util.LogException("Error finalizando las conferencias.", ex);
                    }
                }
            }

        }

        /// <summary>
        /// Método para cambiar el estatus de una conferencia en SYO.
        /// </summary>
        /// <param name="ldrConf">DataRow con el registro de conferencia a la que se le actualizará el estatus</param>
        /// <param name="lsEstatus">Cadena con el vchCodigo del nuevo estatus</param>
        public void SYOConferenceEstatus(DataRow ldrConf, string lsEstatus)
        {
            try
            {

                Hashtable lhtVal = new Hashtable();
                //Actualiza el estatus de la conferencia
                lhtVal.Clear();
                lhtVal.Add("iCodRegistro", ldrConf["iCodRegistro"]);
                lhtVal.Add("{EstConferencia}", KDBUtil.SearchICodCatalogo("EstConferencia", lsEstatus, true));
                KDBUtil.SaveHistoric("TMSConf", "Conferencia", (string)ldrConf["vchCodigo"], null, lhtVal);

            }
            catch (Exception ex)
            {
                Util.LogException("Surgió un error durante la sincronización de conferecias de SeeYouOn en MCU.", ex);
                throw ex;
            }
        }

        /// <summary>
        /// Método que inicia una conferencia en el MCU. Conecta a los participantes.
        /// </summary>
        /// <param name="liCodConf">iCodCatalogo de la conferencia que debe iniciar.</param>
        /// <param name="lsConference">vchDescripcion de la Sala Virtual donde tendrá lugar la conferencia.</param>
        public void SYOConferenceMCU(int liCodConf, string lsConference)
        {
            int liCodMCU = int.MinValue;
            string lsNumericId = "";
            string lsTitle = "";
            MCU loMCU = new MCU();
            Hashtable lhtVal = new Hashtable();

            try
            {
                // Busca la conferencia
                DataRow ldrConf = KDBUtil.SearchHistoricRow("TMSConf", "Conferencia",
                    new string[] { "iCodRegistro", "iCodCatalogo", "vchDescripcion", "{ServidorTMS}", 
                        "{TMSSystems}","{ConfNumericId}" ,"{FechaInicioReservacion}","{FechaFinReservacion}"},
                    "iCodCatalogo = " + liCodConf);

                if (ldrConf == null)
                    throw new Exception("Error al buscar la conferencia previamente guardada.");

                liCodMCU = (int)Util.IsDBNull(ldrConf["{TMSSystems}"], int.MinValue);
                lsNumericId = (string)Util.IsDBNull(ldrConf["{ConfNumericId}"], "");
                lsTitle = (string)Util.IsDBNull(ldrConf["vchDescripcion"], "");

                // Estatus Eliminado y Agregado
                int liEstEliminado = (int)Util.IsDBNull(KDBUtil.SearchScalar("EstParticipante", "Eliminado", "iCodCatalogo", true), 0);
                int liEstParticipante = KDBUtil.SearchICodCatalogo("EstParticipante", "Agregado", true);

                // Buscar los Participantes
                DataTable ldtPart = kdb.GetHisRegByEnt("Participante", "Participante",
                    new string[] { "iCodRegistro", "iCodCatalogo", "{TMSPhoneBookContact}", "{Address}" },
                    "{TMSConf} = " + liCodConf + " and isnull({EstParticipante}, 0) <> " + liEstEliminado);

                if (ldtPart == null || ldtPart.Rows.Count == 0)
                    throw new Exception("No se encontraron participantes para la conferencia '" + liCodConf + "'");

                // Recorrer los participantes
                foreach (DataRow ldrPart in ldtPart.Rows)
                {
                    try
                    {
                        // Obtener su Contacto
                        DataRow ldrContact = KDBUtil.SearchHistoricRow("TMSPhoneBookContact", "PhoneBook",
                            new string[] { "vchDescripcion" },
                            "iCodCatalogo = " + (int)Util.IsDBNull(ldrPart["{TMSPhoneBookContact}"], int.MinValue));

                        if (ldrContact == null)
                            throw new Exception("No se encontró el contacto '" + ldrPart["{TMSPhoneBookContact}"] + "'");

                        if (string.IsNullOrEmpty((string)Util.IsDBNull(ldrContact["vchDescripcion"], "")))
                            throw new Exception("El contacto '" + ldrPart["{TMSPhoneBookContact}"] + "' tiene nombre inválido");


                        // Obtener su Dirección
                        DataRow ldrAddress = KDBUtil.SearchHistoricRow("Address", "Address",
                            new string[] { "{SystemAddress}", "{MCUCallProtocol}" },
                            "iCodCatalogo = " + (int)Util.IsDBNull(ldrPart["{Address}"], int.MinValue));

                        if (ldrAddress == null)
                            throw new Exception("No se encontró la direccion '" + ldrPart["{Address}"] + "'");

                        if (string.IsNullOrEmpty((string)Util.IsDBNull(ldrAddress["{SystemAddress}"], "")))
                            throw new Exception("El participante '" + ldrPart["iCodCatalogo"] + "' tiene dirección inválida");


                        // Obtener el Protocolo
                        string lsProtocol = (String)Util.IsDBNull(KDBUtil.SearchScalar("MCUCallProtocol",
                            (int)Util.IsDBNull(ldrAddress["{MCUCallProtocol}"], int.MinValue),
                            "vchCodigo", true), "SIP");

                        string lsProtocolMCU = (lsProtocol == "SIP" ? "sip" : (lsProtocol == "H.323" ? "h323" : (lsProtocol == "VNC" ? "vnc" : "")));

                        // Guarda el participante en el MCU
                        loMCU.SaveParticipant(lsConference, lsNumericId,
                            (string)ldrContact["vchDescripcion"],
                            (string)ldrAddress["{SystemAddress}"],
                            lsProtocolMCU, liCodMCU);

                        // Actualizar el estatus del participante a "Agregado"
                        lhtVal.Clear();
                        lhtVal.Add("iCodRegistro", ldrPart["iCodRegistro"]);
                        lhtVal.Add("{EstParticipante}", liEstParticipante);
                        KDBUtil.SaveHistoric("Participante", "Participante", (string)ldrPart["vchCodigo"], null, lhtVal);

                    }
                    catch (Exception ex)
                    {
                        Util.LogException("No se pudo agendar el participante '" + ldrPart["iCodCatalogo"] + "' en la conferencia '" + liCodConf + "'", ex);
                    }
                }

                // Actualiza el estatus de la conferencia a "Iniciada"
                lhtVal.Clear();
                lhtVal.Add("iCodRegistro", ldrConf["iCodRegistro"]);
                lhtVal.Add("{EstConferencia}", KDBUtil.SearchICodCatalogo("EstConferencia", "Iniciada", true));
                KDBUtil.SaveHistoric("TMSConf", "Conferencia", (string)ldrConf["vchCodigo"], null, lhtVal);

            }
            catch (Exception ex)
            {
                Util.LogException("Error al grabar los participantes de la conferencia SYO '" + lsNumericId + " " + lsTitle + "' en TMS.", ex);
            }

        }

        /// <summary>
        /// Método usado para terminar una conferencia que tiene lugar en el MCU.
        /// Lo hace desconectando a todos los participantes.
        /// </summary>
        /// <param name="liCodMCU">iCodCatalogo del MCU en que tiene lugar la conferencia.</param>
        /// <param name="liCodConf">iCodCatalogo de la conferencia que se terminará.</param>
        /// <param name="lsConference">vchDescripcion de la conferencia que se terminará.</param>
        public void SYOConferenceMCUDel(int liCodMCU, int liCodConf, string lsConference)
        {
            string lsNumericId = "";
            MCU loMCU = new MCU();
            Dictionary<string, Struct> lstPart = null;
            Hashtable lhtVal = new Hashtable();

            try
            {

                // Obtiene los participantes de la conferencia en el MCU
                lstPart = loMCU.GetParticipants(lsConference, lsNumericId, liCodMCU);

                // Borra todos los participantes de la conferencia 
                if (lstPart != null)
                    foreach (Struct loPart in lstPart.Values)
                        loMCU.DeleteParticipant(lsConference, lsNumericId, (string)loPart["participantName"], (string)loPart["participantProtocol"], liCodMCU);

            }
            catch (Exception ex)
            {
                Util.LogException("Error al Remover los participantes de la conferencia SYO '" + lsConference + "'  en TMS.", ex);
            }

        }

        #endregion

    }
}

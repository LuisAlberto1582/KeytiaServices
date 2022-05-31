using System.Xml.Serialization;
using System;

namespace KeytiaServiceBL
{
    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class cdr_events
    {

        private cdr_eventsEvent[] itemsField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("event", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public cdr_eventsEvent[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cdr_eventsEvent: IComparable
    {

        public int CompareTo(object x)
        {
            if (x is cdr_eventsEvent)
            {
                cdr_eventsEvent eventoX = (cdr_eventsEvent)x;
                int miIndex = int.Parse(this.index);
                int otroIndex = int.Parse(eventoX.index);
                return miIndex.CompareTo(otroIndex);
            }
            else
            {
                return 1;
            }
        }

        private cdr_eventsEventConference[] conferenceField;

        private cdr_eventsEventLimits[] limitsField;

        private cdr_eventsEventEndpoint_details[] endpoint_detailsField;

        private cdr_eventsEventParticipants[] participantsField;

        private cdr_eventsEventGatekeeper[] gatekeeperField;

        private cdr_eventsEventCall[] callField;

        private cdr_eventsEventMedia_from_endpoint[] media_from_endpointField;

        private cdr_eventsEventMedia_to_endpoint[] media_to_endpointField;

        private cdr_eventsEventConference_details[] conference_detailsField;

        private cdr_eventsEventOwner[] ownerField;

        private cdr_eventsEventEnd[] endField;

        private string indexField;

        private string dateField;

        private string timeField;

        private string typeField;

        [XmlIgnore]
        public DateTime Fecha
        {
            get
            {
                DateTime ldtRet = DateTime.MinValue;
                string lsDate = date + " " + time;
                string lsFormat = "d MMMM yyyy HH:mm:ss";
                if (DateTime.TryParseExact(lsDate, lsFormat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out ldtRet))
                    return ldtRet;
                else
                    return ldtRet;
            }
        }

        public override string ToString()
        {
            System.Text.StringBuilder sbMCUEvent = new System.Text.StringBuilder();
            sbMCUEvent.Append("index=\"");
            sbMCUEvent.Append(index);
            sbMCUEvent.Append("\" ");
            sbMCUEvent.Append("date=\"");
            sbMCUEvent.Append(date);
            sbMCUEvent.Append("\" ");
            sbMCUEvent.Append("time=\"");
            sbMCUEvent.Append(time);
            sbMCUEvent.Append("\" ");
            sbMCUEvent.Append("type=\"");
            sbMCUEvent.Append(type);
            sbMCUEvent.Append("\" ");
            return sbMCUEvent.ToString();
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("conference", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public cdr_eventsEventConference[] conference
        {
            get
            {
                return this.conferenceField;
            }
            set
            {
                this.conferenceField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("limits", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public cdr_eventsEventLimits[] limits
        {
            get
            {
                return this.limitsField;
            }
            set
            {
                this.limitsField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("endpoint_details", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public cdr_eventsEventEndpoint_details[] endpoint_details
        {
            get
            {
                return this.endpoint_detailsField;
            }
            set
            {
                this.endpoint_detailsField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("participants", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public cdr_eventsEventParticipants[] participants
        {
            get
            {
                return this.participantsField;
            }
            set
            {
                this.participantsField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("gatekeeper", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public cdr_eventsEventGatekeeper[] gatekeeper
        {
            get
            {
                return this.gatekeeperField;
            }
            set
            {
                this.gatekeeperField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("call", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public cdr_eventsEventCall[] call
        {
            get
            {
                return this.callField;
            }
            set
            {
                this.callField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("media_from_endpoint", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public cdr_eventsEventMedia_from_endpoint[] media_from_endpoint
        {
            get
            {
                return this.media_from_endpointField;
            }
            set
            {
                this.media_from_endpointField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("media_to_endpoint", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public cdr_eventsEventMedia_to_endpoint[] media_to_endpoint
        {
            get
            {
                return this.media_to_endpointField;
            }
            set
            {
                this.media_to_endpointField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("conference_details", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public cdr_eventsEventConference_details[] conference_details
        {
            get
            {
                return this.conference_detailsField;
            }
            set
            {
                this.conference_detailsField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("owner", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public cdr_eventsEventOwner[] owner
        {
            get
            {
                return this.ownerField;
            }
            set
            {
                this.ownerField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlElementAttribute("end", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public cdr_eventsEventEnd[] end
        {
            get
            {
                return this.endField;
            }
            set
            {
                this.endField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string index
        {
            get
            {
                return this.indexField;
            }
            set
            {
                this.indexField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string date
        {
            get
            {
                return this.dateField;
            }
            set
            {
                this.dateField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string time
        {
            get
            {
                return this.timeField;
            }
            set
            {
                this.timeField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cdr_eventsEventConference
    {

        private string unique_idField;

        private string nameField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string unique_id
        {
            get
            {
                return this.unique_idField;
            }
            set
            {
                this.unique_idField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cdr_eventsEventLimits
    {

        private string audio_video_participantsField;

        private string audio_only_participantsField;

        private string streaming_participants_allowedField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string audio_video_participants
        {
            get
            {
                return this.audio_video_participantsField;
            }
            set
            {
                this.audio_video_participantsField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string audio_only_participants
        {
            get
            {
                return this.audio_only_participantsField;
            }
            set
            {
                this.audio_only_participantsField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string streaming_participants_allowed
        {
            get
            {
                return this.streaming_participants_allowedField;
            }
            set
            {
                this.streaming_participants_allowedField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cdr_eventsEventEndpoint_details
    {

        private string ip_addressField;

        private string dnField;

        private string h323_aliasField;

        private string configured_nameField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ip_address
        {
            get
            {
                return this.ip_addressField;
            }
            set
            {
                this.ip_addressField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string dn
        {
            get
            {
                return this.dnField;
            }
            set
            {
                this.dnField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string h323_alias
        {
            get
            {
                return this.h323_aliasField;
            }
            set
            {
                this.h323_aliasField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string configured_name
        {
            get
            {
                return this.configured_nameField;
            }
            set
            {
                this.configured_nameField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cdr_eventsEventParticipants
    {

        private string participant_idField;

        private string particpant_idField;

        private string max_simultaneous_audio_videoField;

        private string max_simultaneous_audio_onlyField;

        private string max_simultaneous_streamingField;

        private string total_audio_videoField;

        private string total_audio_onlyField;

        private string total_streamingField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string participant_id
        {
            get
            {
                return this.participant_idField;
            }
            set
            {
                this.participant_idField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string particpant_id
        {
            get
            {
                return this.particpant_idField;
            }
            set
            {
                this.particpant_idField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string max_simultaneous_audio_video
        {
            get
            {
                return this.max_simultaneous_audio_videoField;
            }
            set
            {
                this.max_simultaneous_audio_videoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string max_simultaneous_audio_only
        {
            get
            {
                return this.max_simultaneous_audio_onlyField;
            }
            set
            {
                this.max_simultaneous_audio_onlyField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string max_simultaneous_streaming
        {
            get
            {
                return this.max_simultaneous_streamingField;
            }
            set
            {
                this.max_simultaneous_streamingField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string total_audio_video
        {
            get
            {
                return this.total_audio_videoField;
            }
            set
            {
                this.total_audio_videoField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string total_audio_only
        {
            get
            {
                return this.total_audio_onlyField;
            }
            set
            {
                this.total_audio_onlyField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string total_streaming
        {
            get
            {
                return this.total_streamingField;
            }
            set
            {
                this.total_streamingField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cdr_eventsEventGatekeeper
    {

        private string registered_with_gatekeeperField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string registered_with_gatekeeper
        {
            get
            {
                return this.registered_with_gatekeeperField;
            }
            set
            {
                this.registered_with_gatekeeperField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cdr_eventsEventCall
    {

        private string time_in_conferenceField;

        private string time_in_conference_in_minutesField;

        private string disconnect_reasonField;

        private string directionField;

        private string protocolField;

        private string media_encryption_statusField;

        [XmlIgnore]
        public int DuracionSegs
        {
            get
            {
                if (string.IsNullOrEmpty(time_in_conference))
                    return 0;

                int liSegundo = 1;
                int liMinuto = liSegundo * 60;
                int liHora = liMinuto * 60;
                int liDia = liHora * 24;

                int[] alTiempos = new int[] { };

                int iSegundos = 0;
                int iMinutos = 0;
                int iHoras = 0;
                int iDias = 0;

                string lsAux = "";
                lsAux = time_in_conference.Replace("days", "").Replace("hrs", "").Replace("mins", "").Replace("sec", "");

                string[] lasTiempo = System.Text.RegularExpressions.Regex.Split(lsAux, "  ");

                if (lasTiempo != null && lasTiempo.Length > 0)
                {
                    if (lasTiempo.Length == 4)
                    {
                        iDias = liDia * int.Parse(lasTiempo[0].Trim());
                        iHoras = liHora * int.Parse(lasTiempo[1].Trim());
                        iMinutos = liMinuto * int.Parse(lasTiempo[2].Trim());
                        iSegundos = liSegundo * int.Parse(lasTiempo[3].Trim());
                    }
                    if (lasTiempo.Length == 3)
                    {
                        iHoras = liHora * int.Parse(lasTiempo[0].Trim());
                        iMinutos = liMinuto * int.Parse(lasTiempo[1].Trim());
                        iSegundos = liSegundo * int.Parse(lasTiempo[2].Trim());
                    }
                    else if (lasTiempo.Length == 2)
                    {
                        iMinutos = liMinuto * int.Parse(lasTiempo[0].Trim());
                        iSegundos = liSegundo * int.Parse(lasTiempo[1].Trim());
                    }
                    else
                    {
                        iSegundos = liSegundo * int.Parse(lasTiempo[0].Trim());
                    }
                }
                return iSegundos + iMinutos + iHoras + iDias;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string time_in_conference
        {
            get
            {
                return this.time_in_conferenceField;
            }
            set
            {
                this.time_in_conferenceField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string time_in_conference_in_minutes
        {
            get
            {
                return this.time_in_conference_in_minutesField;
            }
            set
            {
                this.time_in_conference_in_minutesField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string disconnect_reason
        {
            get
            {
                return this.disconnect_reasonField;
            }
            set
            {
                this.disconnect_reasonField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string direction
        {
            get
            {
                return this.directionField;
            }
            set
            {
                this.directionField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string protocol
        {
            get
            {
                return this.protocolField;
            }
            set
            {
                this.protocolField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string media_encryption_status
        {
            get
            {
                return this.media_encryption_statusField;
            }
            set
            {
                this.media_encryption_statusField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cdr_eventsEventMedia_from_endpoint
    {

        private string resolutionField;

        private string video_codecField;

        private string audio_codecField;

        private string bandwidthField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string resolution
        {
            get
            {
                return this.resolutionField;
            }
            set
            {
                this.resolutionField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string video_codec
        {
            get
            {
                return this.video_codecField;
            }
            set
            {
                this.video_codecField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string audio_codec
        {
            get
            {
                return this.audio_codecField;
            }
            set
            {
                this.audio_codecField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string bandwidth
        {
            get
            {
                return this.bandwidthField;
            }
            set
            {
                this.bandwidthField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cdr_eventsEventMedia_to_endpoint
    {

        private string resolutionField;

        private string video_codecField;

        private string audio_codecField;

        private string bandwidthField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string resolution
        {
            get
            {
                return this.resolutionField;
            }
            set
            {
                this.resolutionField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string video_codec
        {
            get
            {
                return this.video_codecField;
            }
            set
            {
                this.video_codecField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string audio_codec
        {
            get
            {
                return this.audio_codecField;
            }
            set
            {
                this.audio_codecField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string bandwidth
        {
            get
            {
                return this.bandwidthField;
            }
            set
            {
                this.bandwidthField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cdr_eventsEventConference_details
    {

        private string numeric_idField;

        private string has_pinField;

        private string billing_codeField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string numeric_id
        {
            get
            {
                return this.numeric_idField;
            }
            set
            {
                this.numeric_idField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string has_pin
        {
            get
            {
                return this.has_pinField;
            }
            set
            {
                this.has_pinField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string billing_code
        {
            get
            {
                return this.billing_codeField;
            }
            set
            {
                this.billing_codeField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cdr_eventsEventOwner
    {

        private string nameField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <comentarios/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "2.0.50727.3038")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class cdr_eventsEventEnd
    {

        private string scheduled_timeField;

        private string durationField;

        private string duration_in_minutesField;

        private string scheduled_dateField;

        private string scheduled_duration_in_minutesField;

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string scheduled_time
        {
            get
            {
                return this.scheduled_timeField;
            }
            set
            {
                this.scheduled_timeField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string duration
        {
            get
            {
                return this.durationField;
            }
            set
            {
                this.durationField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string duration_in_minutes
        {
            get
            {
                return this.duration_in_minutesField;
            }
            set
            {
                this.duration_in_minutesField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string scheduled_date
        {
            get
            {
                return this.scheduled_dateField;
            }
            set
            {
                this.scheduled_dateField = value;
            }
        }

        /// <comentarios/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string scheduled_duration_in_minutes
        {
            get
            {
                return this.scheduled_duration_in_minutesField;
            }
            set
            {
                this.scheduled_duration_in_minutesField = value;
            }
        }

        [XmlIgnore]
        public int DuracionSegs
        {
            get
            {
                if (string.IsNullOrEmpty(duration))
                    return 0;

                int liSegundo = 1;
                int liMinuto = liSegundo * 60;
                int liHora = liMinuto * 60;
                int liDia = liHora * 24;

                int[] alTiempos = new int[] { };

                int iSegundos = 0;
                int iMinutos = 0;
                int iHoras = 0;
                int iDias = 0;

                string lsAux = "";
                lsAux = duration.Replace("days", "").Replace("hrs", "").Replace("mins", "").Replace("sec", "");

                string[] lasTiempo = System.Text.RegularExpressions.Regex.Split(lsAux, "  ");

                if (lasTiempo != null && lasTiempo.Length > 0)
                {
                    if (lasTiempo.Length == 4)
                    {
                        iDias = liDia * int.Parse(lasTiempo[0].Trim());
                        iHoras = liHora * int.Parse(lasTiempo[1].Trim());
                        iMinutos = liMinuto * int.Parse(lasTiempo[2].Trim());
                        iSegundos = liSegundo * int.Parse(lasTiempo[3].Trim());
                    }
                    if (lasTiempo.Length == 3)
                    {
                        iHoras = liHora * int.Parse(lasTiempo[0].Trim());
                        iMinutos = liMinuto * int.Parse(lasTiempo[1].Trim());
                        iSegundos = liSegundo * int.Parse(lasTiempo[2].Trim());
                    }
                    else if (lasTiempo.Length == 2)
                    {
                        iMinutos = liMinuto * int.Parse(lasTiempo[0].Trim());
                        iSegundos = liSegundo * int.Parse(lasTiempo[1].Trim());
                    }
                    else
                    {
                        iSegundos = liSegundo * int.Parse(lasTiempo[0].Trim());
                    }
                }
                return iSegundos + iMinutos + iHoras + iDias;
            }
        }
    }
}

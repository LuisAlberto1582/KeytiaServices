using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SeeYouOnServiceBL.Models
{
    public class MCU4520Conference
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string NumericId { get; set; }
        public bool RegisterWithSIPRegistrar { get; set; }
        public DateTime StartTime { get; set; }
        public int DurationSeconds { get; set; }
        public bool PreconfiguredParticipantsDefer { get; set; }

    }
}

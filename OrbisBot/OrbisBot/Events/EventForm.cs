using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.Events
{
    enum EventType { ChannelEvent, ServerEvent, UserEvent }
    class EventForm
    {
        public long EventId { get; set; }
        public long ServerId { get; set; }
        public long ChannelId { get; set; }
        public long UserId { get; set; }
        public string Message { get; set; }
        public List<long> TargetUsers { get; set; }
        public bool TargetEveryone { get; set; }
        public DateTime DispatchTime { get; set; }
        public EventType EventType { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.Events
{
    public enum EventType { ChannelEvent, ServerEvent, UserEvent }
    class EventForm
    {
        public long EventId { get; set; }
        public ulong ServerId { get; set; }
        public ulong ChannelId { get; set; }
        public ulong UserId { get; set; }
        public string Message { get; set; }
        public List<ulong> TargetUsers { get; set; }
        public bool TargetEveryone { get; set; }
        public DateTime DispatchTime { get; set; }
        public EventType EventType { get; set; }
    }
}

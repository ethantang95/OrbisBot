using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.Models
{
    public class EventModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public ulong UserID { get; set; }
        public ulong ChannelID { get; set; }
        public ulong ServerID { get; set; }
        public string Message { get; set; }
        public string TargetUsersJSON { get; set; }
        public ulong TargetRole { get; set; }
        public bool? TargetEveryone { get; set; }
        public long NextDispatch { get; set; }
        public long DispatchDelay { get; set; }
        public string EventType { get; set; }
    }
}

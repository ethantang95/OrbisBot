using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.Models
{
    class EventModel
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public long ChannelID { get; set; }
        public long ServerID { get; set; }
        public string Message { get; set; }
        public string TargetUsersJSON { get; set; }
        public bool TargetEveryone { get; set; }
        public DateTime NextDispatch { get; set; }
        public DateTime DispatchDelay { get; set; }
        public string EventType { get; set; }
    }
}

using DatabaseConnector.DAO;
using DatabaseConnector.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.Events
{
    class EventAccessor
    {
        EventDAO _eventDAO;
        public EventAccessor(EventDAO dao)
        {
            _eventDAO = dao;
        }

        public List<EventForm> GetEvents(DateTime start, DateTime end)
        {
            var startUXTime = CommonTools.ToUnixMilliTime(start);
            var endUXTime = CommonTools.ToUnixMilliTime(end);

            var results = _eventDAO.GetEventByTimeRange(startUXTime, endUXTime);

            return results.Select(EventFormParser).ToList();
        }

        public bool CreateEvent(EventForm form)
        {
            var model = EventModelParser(form);

            var result = _eventDAO.InsertObject(model);

            return result;
        }

        public bool UpdateEvent(EventForm form)
        {
            var model = EventModelParser(form);

            var result = _eventDAO.UpdateObject(model);

            return result;
        }

        public bool SetNextDispatchByDelay(long delay, long eventId)
        {
            var result = _eventDAO.SetNextDispatch(eventId, delay);

            return result;
        }

        public bool RemoveEvent(long id)
        {
            var result = _eventDAO.DeleteObject(id);

            return result;
        }

        public List<EventForm> FindEventByName(string search, long channelID)
        {
            var result = _eventDAO.FindObjectByName(search, channelID);

            return result.Select(EventFormParser).ToList();
        }

        private EventForm EventFormParser(EventModel model)
        {
            var toReturn = new EventForm();

            toReturn.EventId = model.ID;
            toReturn.EventName = model.Name;
            toReturn.ServerId = (ulong)model.ServerID;
            toReturn.ChannelId = (ulong)model.ChannelID;
            toReturn.UserId = (ulong)model.UserID;
            toReturn.Message = model.Message;
            toReturn.TargetUsers = JsonConvert.DeserializeObject<List<ulong>>(model.TargetUsersJSON);
            toReturn.TargetRole = model.TargetRole;
            toReturn.TargetEveryone = model.TargetEveryone;
            toReturn.DispatchTime = new DateTime(model.NextDispatch);
            toReturn.NextDispatchPeriod = model.DispatchDelay;
            toReturn.EventType = EnumParser.ParseString(model.EventType, EventType.InternalError);
            
            return toReturn;
        }

        private EventModel EventModelParser(EventForm form)
        {
            var toReturn = new EventModel();

            toReturn.ID = form.EventId;
            toReturn.Name = form.EventName;
            toReturn.ServerID = (long)form.ServerId;
            toReturn.ChannelID = (long)form.ChannelId;
            toReturn.UserID = (long)form.UserId;
            toReturn.Message = form.Message;
            toReturn.TargetUsersJSON = JsonConvert.SerializeObject(form.TargetEveryone);
            toReturn.TargetRole = form.TargetRole;
            toReturn.TargetEveryone = form.TargetEveryone;
            toReturn.NextDispatch = CommonTools.ToUnixMilliTime(form.DispatchTime);
            toReturn.DispatchDelay = form.NextDispatchPeriod;
            toReturn.EventType = form.EventType.ToString();

            return toReturn;
        }
    }
}

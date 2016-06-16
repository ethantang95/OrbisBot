using DatabaseConnector.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace OrbisBot.Events
{
    //this class is the overarching class for events. It will be referenced in Context
    //it is responsible for initalizing fetching of new event from an event finder class
    internal class EventManager
    {
        public const int EVENT_FETCH_INTERVAL = 30;
        public const int EVENT_GRACE_LATE_PERIOD = -10;

        public EventAccessor EventDAOAccessor { get; private set; }

        EventScheduler _scheduler;
        Timer _eventFetcherTimer;

        public EventManager(EventDAO dao)
        {
            _scheduler = new EventScheduler(this);
            EventDAOAccessor = new EventAccessor(dao);
            _eventFetcherTimer = new Timer(new TimeSpan(0, EVENT_FETCH_INTERVAL, 0).TotalMilliseconds);
            _eventFetcherTimer.Elapsed += FetchAndHandleEvents;
            _eventFetcherTimer.Start();
        }

        public void GetEvents()
        {
            FetchAndHandleEvents(null, null);
        }

        private void FetchAndHandleEvents(object o, ElapsedEventArgs args)
        {
            //fetch also any events that hasn't been updated
            var events = EventDAOAccessor.GetEvents(new DateTime(0), DateTime.UtcNow.AddMinutes(EVENT_FETCH_INTERVAL + 1));

            //update and destroy the events that are somehow late?
            //1 minute buffer for task delay
            var scheduleEvents = events.Where(s => s.DispatchTime > DateTime.UtcNow.AddMinutes(EVENT_GRACE_LATE_PERIOD));

            foreach(var scheduleEvent in scheduleEvents) 
            {
                AddEventToDispatch(scheduleEvent);
            }

            var scheduledEventHashSet = scheduleEvents.Select(s => s.EventId).ToList();

            var lateEvents = events.Where(s => !scheduledEventHashSet.Contains(s.EventId));

            foreach (var lateEvent in lateEvents)
            {
                try
                {
                    ScheduleNextDispatch(lateEvent);
                }
                catch (Exception e)
                {
                    DiscordMethods.SendExceptionMsg(e);
                }
            }

        }

        private void AddEventToDispatch(EventForm form)
        {
            _scheduler.AddEvent(form);
        }

        public void CreateEvent(EventForm form)
        {
            var result = EventDAOAccessor.CreateEvent(form);

            if (result == -1)
            {
                throw new EventOperationsException($"Failure to add the event {form.EventId}", form);
            }

            //local 30 minutes, might as well queue it
            if (form.DispatchTime <= DateTime.UtcNow.AddMinutes(EVENT_FETCH_INTERVAL))
            {
                form.EventId = result;
                AddEventToDispatch(form);
            }
        }

        public void RemoveEvent(EventForm form)
        {
            _scheduler.RemoveEvent(form.EventId);

            var result = EventDAOAccessor.RemoveEvent(form.EventId);

            if (!result)
            {
                throw new EventOperationsException($"Failure to delete the event {form.EventId}", form);
            }
        }

        public void SkipEvent(long eventId)
        {
            _scheduler.RemoveEvent(eventId);

            var eventModel = EventDAOAccessor.FindEventById(eventId);

            ScheduleNextDispatch(eventModel);
        }

        public void ScheduleNextDispatch(EventForm form)
        {
            bool result;
            if (form.NextDispatchPeriod == -1)
            {
                result = EventDAOAccessor.RemoveEvent(form.EventId);
            }
            else
            {
                result = EventDAOAccessor.SetNextDispatchByDelay(form.NextDispatchPeriod, form.EventId);
            }

            if (!result)
            {
                throw new EventOperationsException($"Failure to set the next dispatch of event {form.EventId}", form);
            }
            else if (form.NextDispatchPeriod != -1)
            {
                //if it is a close rescheduling, we can just quickly put it back in the queue again
                var eventModel = EventDAOAccessor.FindEventById(form.EventId);
                if (eventModel.DispatchTime < DateTime.UtcNow.AddMinutes(30))
                {
                    AddEventToDispatch(eventModel);
                }
            }
        }
    }

    internal class EventOperationsException : Exception
    {
        public EventForm Form { get; private set; }
        public EventOperationsException(string message, EventForm form) : base(message)
        {
            Form = form;
        }
    }
}

﻿using DatabaseConnector.DAO;
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

        public EventAccessor EventDAOAccessor { get; private set; }

        EventScheduler _scheduler;
        Timer _eventFetcherTimer;

        public EventManager(EventDAO dao)
        {
            _scheduler = new EventScheduler(this);
            EventDAOAccessor = new EventAccessor(dao);
            _eventFetcherTimer = new Timer(new TimeSpan(0, EVENT_FETCH_INTERVAL, 0).TotalMilliseconds);
            FetchAndHandleEvents(null, null);
            _eventFetcherTimer.Elapsed += FetchAndHandleEvents;
        }

        private void FetchAndHandleEvents(object o, ElapsedEventArgs args)
        {
            //fetch also any events that hasn't been updated
            var events = EventDAOAccessor.GetEvents(new DateTime(0), DateTime.Now.AddMinutes(EVENT_FETCH_INTERVAL + 1));

            //update and destroy the events that are somehow late?
            //1 minute buffer for task delay
            var scheduleEvents = events.Where(s => s.DispatchTime > DateTime.Now.AddMinutes(-5));

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
            //local 30 minutes, might as well queue it
            if (form.DispatchTime <= DateTime.Now.AddMinutes(EVENT_FETCH_INTERVAL))
            {
                AddEventToDispatch(form);
            }
            var result = EventDAOAccessor.CreateEvent(form);

            if (!result)
            {
                throw new EventOperationsException($"Failure to add the event {form.EventId}");
            }
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
                throw new EventOperationsException($"Failure to set the next dispatch of event {form.EventId}");
            }
        }
    }

    internal class EventOperationsException : Exception
    {
        public EventOperationsException(string message) : base(message)
        {
        }
    }
}

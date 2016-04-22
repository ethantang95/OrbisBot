﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace OrbisBot.Events
{

    internal delegate void OnEventFinished(object sender, EventForm form);
    //specifically used for scheduling events that are obtained by the manager
    class EventScheduler
    {
        Dictionary<long, EventObjects> _events;
        EventManager _manager;

        public EventScheduler(EventManager manager)
        {
            _events = new Dictionary<long, EventObjects>();
            _manager = manager;
        }

        public void AddEvent(EventForm form)
        {

            if (form.DispatchTime < DateTime.Now)
            {
                EventDispatcher.Dispatch(form);
                return;
            }

            var eventObj = new EventObjects(form);
            eventObj.EventFinished += CleanUpEvent;

            if (_events.ContainsKey(form.EventId))
            {
                _events[form.EventId].DestroyEvent();
                _events[form.EventId] = eventObj;
            }
            else
            {
                _events.Add(form.EventId, eventObj);
            }
        }

        private void CleanUpEvent(object sender, EventForm form)
        {
            _events.Remove(form.EventId);
            _manager.ScheduleNextDispatch(form);
        }
    }

    class EventObjects
    {
        public event OnEventFinished EventFinished;

        Timer _timer;
        EventForm _form;

        public EventObjects(EventForm form)
        {
            _form = form;

            var timeToDispatch = CommonTools.ToUnixMilliTime(form.DispatchTime) - CommonTools.ToUnixMilliTime(DateTime.Now);
            _timer = new Timer(timeToDispatch);
            _timer.Elapsed += DispatchEvent;
            _timer.Start();
        }

        public void DestroyEvent()
        {
            _timer.Stop();
            _timer.Dispose();
        }

        private void DispatchEvent(object sender, ElapsedEventArgs e)
        {
            _timer.Stop();
            _timer.Dispose();
            EventDispatcher.Dispatch(_form);
            if (EventFinished != null)
            {
                EventFinished(this, _form);
            }
        }
    }
}

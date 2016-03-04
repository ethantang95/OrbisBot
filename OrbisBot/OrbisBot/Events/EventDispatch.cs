using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.Events
{
    //event dispatch life cycle
    //an event is first sent from an event bank, which contains all the events in memory
    //it will call this abstract to dispatch the event
    //this abstract will first check the channel permissions to see if a dispatch is available
    //if not, the event will be discarded

    //if the event dispatch is available, the event will then call the main body
    //which is an abstract, and it will be dispatched with respect to its event type

    //an event object is not persistent as compared to tasks, it will get discarded after the
    //event has been executed

    class EventDispatch
    {
        public EventDispatch()
        { }

        //maybe change this to a boolean in the future where an event failed to run, this way
        //it can saved and dispatched later
        public void RunEvent(EventForm eventForm) 
        {
            if (!ProceedWithCommand(eventForm))
            {
                return;
            }
            try
            {
                EventDispatcher(eventForm);
            }
            catch (Exception e)
            {
                DiscordMethods.OnEventFailure(e, eventForm);
            }
        }

        private bool ProceedWithCommand(EventForm eventForm)
        {
            //we will take the channel Id and see that if it is muted or not
            //future permissions will take place also
            if (Context.Instance.ChannelPermission.ContainsChannel(eventForm.ChannelId))
            {
                return !Context.Instance.ChannelPermission.ChannelPermissions[eventForm.ChannelId].Muted;
            }

            return true;
        }

        private void EventDispatcher(EventForm eventForm)
        {
            switch (eventForm.EventType)
            {
                case EventType.ChannelEvent: DispatchChannelEvent(eventForm);
                    break;
                default: throw new NotImplementedException($"An event type of {eventForm.EventType} has been tried to be executed");

            }
        }

        private void DispatchChannelEvent(EventForm eventForm)
        {

        }
    }
}

using System;
using RogueEssence.Data;
using RogueElements;

namespace RogueEssence.Dungeon
{
    /// <summary>
    /// Abstract base class for objects that can own and trigger game events.
    /// Provides the foundation for items, abilities, statuses, and other sources of game effects.
    /// </summary>
    [Serializable]
    public abstract class GameEventOwner
    {
        /// <summary>
        /// Gets the cause type of events from this owner.
        /// </summary>
        /// <returns>The event cause classification.</returns>
        public abstract GameEventPriority.EventCause GetEventCause();

        /// <summary>
        /// Gets the unique identifier for this event owner.
        /// </summary>
        /// <returns>The identifier string.</returns>
        public abstract string GetID();

        /// <summary>
        /// Gets the display name for this event owner.
        /// </summary>
        /// <returns>The formatted display name.</returns>
        public abstract string GetDisplayName();

        /// <summary>
        /// Adds events from an effect list to a priority queue for processing.
        /// </summary>
        /// <typeparam name="T">The type of game event.</typeparam>
        /// <param name="queue">The priority queue to add events to.</param>
        /// <param name="maxPriority">The maximum priority to consider.</param>
        /// <param name="nextPriority">Reference to the next priority level to process.</param>
        /// <param name="effectList">The list of effects to add.</param>
        /// <param name="targetChar">The target character for the events.</param>
        public void AddEventsToQueue<T>(StablePriorityQueue<GameEventPriority, EventQueueElement<T>> queue, Priority maxPriority, ref Priority nextPriority, PriorityList<T> effectList, Character targetChar) where T : GameEvent
        {
            foreach(Priority priority in effectList.GetPriorities())
            {
                //if an item has the same priority variable as the nextPriority, enqueue it
                //if an item has a higher priority variable than nextPriority, ignore it
                //if an item has a lower priority variable than nextPriority, check against maxPriority
                for (int ii = 0; ii < effectList.GetCountAtPriority(priority); ii++)
                {
                    if (priority == nextPriority)
                    {
                        GameEventPriority gameEventPriority = new GameEventPriority(priority, GameEventPriority.USER_PORT_PRIORITY, GetEventCause(), GetID(), ii);
                        EventQueueElement<T> eventQueueElement = new EventQueueElement<T>(this, null, effectList.Get(priority, ii), targetChar);
                        queue.Enqueue(gameEventPriority, eventQueueElement);
                    }
                    else if (priority < nextPriority || nextPriority == Priority.Invalid)
                    {
                        //if the item has a lower priority variable than maxPriority, ignore it
                        //if the item has a higher priority variable than maxPriority, clear the queue and add the new item
                        if (priority > maxPriority || maxPriority == Priority.Invalid)
                        {
                            nextPriority = priority;
                            queue.Clear();
                            GameEventPriority gameEventPriority = new GameEventPriority(priority, GameEventPriority.USER_PORT_PRIORITY, GetEventCause(), GetID(), ii);
                            EventQueueElement<T> eventQueueElement = new EventQueueElement<T>(this, null, effectList.Get(priority, ii), targetChar);
                            queue.Enqueue(gameEventPriority, eventQueueElement);
                        }
                    }
                }
            }
        }
    }


    /// <summary>
    /// Abstract base class for passive abilities and active effects that provide data-driven behaviors.
    /// </summary>
    [Serializable]
    public abstract class PassiveActive : GameEventOwner
    {
        /// <summary>
        /// Gets the passive data associated with this ability or effect.
        /// </summary>
        /// <returns>The passive data containing effect definitions.</returns>
        public abstract PassiveData GetData();

        //TODO: Created v0.5.20, revert on v1.1
        //public override string GetID() { return ID; }
        //public abstract string ID { get; set; }

        /// <summary>
        /// Initializes a new PassiveActive instance.
        /// </summary>
        public PassiveActive()
        {
            //ID = "";
        }
        //public PassiveActive(PassiveActive other)
        //{
        //    ID = other.ID;
        //}
    }

    /// <summary>
    /// Represents an element in the event processing queue, containing the event, its owner, and target.
    /// </summary>
    /// <typeparam name="T">The type of game event.</typeparam>
    public class EventQueueElement<T>
        where T : GameEvent
    {
        /// <summary>
        /// The owner of the event (item, ability, status, etc.).
        /// </summary>
        public GameEventOwner Owner;

        /// <summary>
        /// The character that owns the event source.
        /// </summary>
        public Character OwnerChar;

        /// <summary>
        /// The actual event to be processed.
        /// </summary>
        public T Event;

        /// <summary>
        /// The target character of the event.
        /// </summary>
        public Character TargetChar;

        /// <summary>
        /// Initializes a new EventQueueElement with all required data.
        /// </summary>
        /// <param name="owner">The event owner.</param>
        /// <param name="ownerChar">The character owning the event source.</param>
        /// <param name="newEvent">The event to process.</param>
        /// <param name="targetChar">The target of the event.</param>
        public EventQueueElement(GameEventOwner owner, Character ownerChar, T newEvent, Character targetChar)
        {
            Owner = owner;
            OwnerChar = ownerChar;
            Event = newEvent;
            TargetChar = targetChar;
        }
    }

    /// <summary>
    /// Context information for processing passive effects, containing the passive source and priority information.
    /// </summary>
    public class PassiveContext
    {
        /// <summary>
        /// The passive ability or effect source.
        /// </summary>
        public PassiveActive Passive;

        /// <summary>
        /// The passive data containing effect definitions.
        /// </summary>
        public PassiveData EventData;

        /// <summary>
        /// The port priority for event ordering.
        /// </summary>
        public Priority PortPriority;

        /// <summary>
        /// The character affected by the passive.
        /// </summary>
        public Character EventChar;

        /// <summary>
        /// Initializes a new PassiveContext with all required data.
        /// </summary>
        /// <param name="passive">The passive ability source.</param>
        /// <param name="passiveEntry">The passive data entry.</param>
        /// <param name="portPriority">The port priority level.</param>
        /// <param name="effectChar">The affected character.</param>
        public PassiveContext(PassiveActive passive, PassiveData passiveEntry, Priority portPriority, Character effectChar)
        {
            Passive = passive;
            EventData = passiveEntry;
            PortPriority = portPriority;
            EventChar = effectChar;
        }


        /// <summary>
        /// Adds events from an effect list to a priority queue for processing.
        /// </summary>
        /// <typeparam name="T">The type of game event.</typeparam>
        /// <param name="queue">The priority queue to add events to.</param>
        /// <param name="maxPriority">The maximum priority to consider.</param>
        /// <param name="nextPriority">Reference to the next priority level to process.</param>
        /// <param name="effectList">The list of effects to add.</param>
        /// <param name="targetChar">The target character for the events.</param>
        public void AddEventsToQueue<T>(StablePriorityQueue<GameEventPriority, EventQueueElement<T>> queue, Priority maxPriority, ref Priority nextPriority, PriorityList<T> effectList, Character targetChar) where T : GameEvent
        {
            foreach(Priority priority in effectList.GetPriorities())
            {
                //if an item has the same priority variable as the nextPriority, enqueue it
                //if an item has a higher priority variable than nextPriority, ignore it
                //if an item has a lower priority variable than nextPriority, check against maxPriority
                for (int ii = 0; ii < effectList.GetCountAtPriority(priority); ii++)
                {
                    if (priority == nextPriority)
                    {
                        GameEventPriority gameEventPriority = new GameEventPriority(priority, PortPriority, Passive.GetEventCause(), Passive.GetID(), ii);
                        EventQueueElement<T> queueElement = new EventQueueElement<T>(Passive, EventChar, effectList.Get(priority, ii), targetChar);
                        queue.Enqueue(gameEventPriority, queueElement);
                    }
                    else if (priority < nextPriority || nextPriority == Priority.Invalid)
                    {
                        //if the item has a lower priority variable than maxPriority, ignore it
                        //if the item has an equal or higher priority variable than maxPriority, clear the queue and add the new item
                        if (priority > maxPriority || maxPriority == Priority.Invalid)
                        {
                            nextPriority = priority;
                            queue.Clear();
                            GameEventPriority gameEventPriority = new GameEventPriority(priority, PortPriority, Passive.GetEventCause(), Passive.GetID(), ii);
                            EventQueueElement<T> queueElement = new EventQueueElement<T>(Passive, EventChar, effectList.Get(priority, ii), targetChar);
                            queue.Enqueue(gameEventPriority, queueElement);
                        }
                    }
                }
            }
        }
    }

}

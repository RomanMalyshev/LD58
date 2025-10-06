using System.Collections.Generic;
using UnityEngine;

namespace Events
{
    [CreateAssetMenu(fileName = "EventsPool_", menuName = "Events/Events Pool")]
    public class EventsPool : ScriptableObject
    {
        [Header("Pool Settings")]
        public EventType PoolType;
        
        [Header("Events in Pool")]
        public List<EventData> Events = new List<EventData>();

        
        [Range(0f,1f)]
        public float Chance;
        
        public EventData GetRandomEvent(string lastEventID = "")
        {
            if (Events == null || Events.Count == 0)
            {
                Debug.LogWarning($"EventsPool {name} is empty!");
                return null;
            }

            // Filter out last event to prevent repetition
            List<EventData> availableEvents = new List<EventData>();
            foreach (var evt in Events)
            {
                if (evt != null && evt.EventID != lastEventID)
                {
                    availableEvents.Add(evt);
                }
            }

            if (availableEvents.Count == 0)
            {
                // If all events were filtered, just pick any
                availableEvents = Events;
            }

            int randomIndex = Random.Range(0, availableEvents.Count);
            return availableEvents[randomIndex];
        }
    }
}


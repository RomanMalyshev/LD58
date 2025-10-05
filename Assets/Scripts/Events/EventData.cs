using UnityEngine;
using DefaultNamespace;

namespace Events
{
    [CreateAssetMenu(fileName = "Event_", menuName = "Events/Event Data")]
    public class EventData : ScriptableObject
    {
        [Header("Event Identity")]
        public string EventID;
        public string EventName;
        [TextArea(3, 6)]
        public string EventDescription;

        [Header("Event Type")]
        public EventType Type = EventType.Global;
        
        [Header("Tile Requirements (for tile events)")]
        public TileType RequiredTileType = TileType.Simple;

        [Header("Choices")]
        public EventChoice[] Choices = new EventChoice[2];

        [Header("Passive Effect (no choice)")]
        public bool IsPassiveEvent = false;
        public EventEffect PassiveEffect;
    }

    public enum EventType
    {
        Global,         // Happens on random occupied tiles at turn start
        Tavern,         // Triggered when capturing tavern tile
        Camp,           // Triggered when capturing camp tile
        Chest           // Triggered when capturing chest tile
    }
}


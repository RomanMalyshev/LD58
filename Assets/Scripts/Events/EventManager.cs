using System.Collections.Generic;
using UnityEngine;
using Map;

namespace Events
{
    public class EventManager
    {
        private EventsPool _globalEventsPool;
        private EventsPool _tavernEventsPool;
        private EventsPool _campEventsPool;

        private string _lastGlobalEventID = "";
        private string _lastTavernEventID = "";
        private string _lastCampEventID = "";

        public EventManager(EventsPool globalPool, EventsPool tavernPool, EventsPool campPool)
        {
            _globalEventsPool = globalPool;
            _tavernEventsPool = tavernPool;
            _campEventsPool = campPool;
        }

        public EventData GetRandomGlobalEvent()
        {
            if (_globalEventsPool == null) return null;
            
            var evt = _globalEventsPool.GetRandomEvent(_lastGlobalEventID);
            if (evt != null)
            {
                _lastGlobalEventID = evt.EventID;
            }
            return evt;
        }

        public EventData GetRandomTavernEvent()
        {
            if (_tavernEventsPool == null) return null;
            
            var evt = _tavernEventsPool.GetRandomEvent(_lastTavernEventID);
            if (evt != null)
            {
                _lastTavernEventID = evt.EventID;
            }
            return evt;
        }

        public EventData GetRandomCampEvent()
        {
            if (_campEventsPool == null) return null;
            
            var evt = _campEventsPool.GetRandomEvent(_lastCampEventID);
            if (evt != null)
            {
                _lastCampEventID = evt.EventID;
            }
            return evt;
        }

        public Tile GetRandomOccupiedTile(List<Tile> occupiedTiles, DefaultNamespace.TileType? requiredType = null)
        {
            if (occupiedTiles == null || occupiedTiles.Count == 0)
                return null;

            List<Tile> eligibleTiles = new List<Tile>();

            if (requiredType.HasValue)
            {
                foreach (var tile in occupiedTiles)
                {
                    if (tile.TileType == requiredType.Value)
                    {
                        eligibleTiles.Add(tile);
                    }
                }
            }
            else
            {
                eligibleTiles = occupiedTiles;
            }

            if (eligibleTiles.Count == 0)
                return null;

            int randomIndex = Random.Range(0, eligibleTiles.Count);
            return eligibleTiles[randomIndex];
        }
    }
}


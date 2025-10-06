using System;
using System.Linq;
using Map;
using Model;
using State_Machine;
using UI;
using Events;
using UnityEngine;

namespace GameStates
{
    public class RandomEvent : IState
    {
        public Action OnEventCompleted;

        private MapController _map;
        private Hud _hud;
        private Player _player;
        private EventManager _eventManager;

        private EventData _currentEvent;
        private Tile _affectedTile;
        private bool _waitingForChoice;
        private bool _eventCompleted;

        public RandomEvent(MapController map, Hud hud, Player playerModel, EventManager eventManager)
        {
            _map = map;
            _hud = hud;
            _player = playerModel;
            _eventManager = eventManager;
        }

        public void Enter()
        {
            _waitingForChoice = false;
            _eventCompleted = false;
            _currentEvent = null;
            _affectedTile = null;

            TriggerRandomEvent();
        }

        public void Execute()
        {
            if (_eventCompleted)
            {
                OnEventCompleted?.Invoke();
            }
        }

        public void Exit()
        {
            _hud.OnPopupAccept -= OnAccept;
            _hud.OnPopupDecline -= OnDecline;
            _hud.HidePopup();
        }

        private void TriggerRandomEvent()
        {
            var occupiedTiles = _map.GetOccupiedTiles();
            if (occupiedTiles.Count == 0)
            {
                // No tiles to trigger events on
                _eventCompleted = true;
                return;
            }

            _currentEvent = _eventManager.GetRandomGlobalEvent();

            if (_currentEvent == null)
            {
                _eventCompleted = true;
                return;
            }

            if (occupiedTiles.All(it => it.TileType != _currentEvent.RequiredTileType))
            {
                _eventCompleted = true;
                return;
            }

            // Get random occupied tile for the event
            _affectedTile = _eventManager.GetRandomOccupiedTile(occupiedTiles);

            if (_currentEvent.IsPassiveEvent)
            {
                HandlePassiveEvent();
            }
            else
            {
                ShowEventChoices();
            }
        }

        private void HandlePassiveEvent()
        {
            string message = $"{_currentEvent.EventName}\n{_currentEvent.EventDescription}";

            if (_currentEvent.PassiveEffect != null)
            {
                _currentEvent.PassiveEffect.Apply(_player, _affectedTile, _map);
            }

            _hud.ShowPopup(message, "Continue", null);
            _hud.OnPopupAccept += () => { _eventCompleted = true; };
        }

        private void ShowEventChoices()
        {
            string message = $"{_currentEvent.EventName}\n{_currentEvent.EventDescription}";

            string acceptLabel = _currentEvent.Choices[0] != null ? _currentEvent.Choices[0].ChoiceText : "Accept";
            string declineLabel = _currentEvent.Choices[1] != null ? _currentEvent.Choices[1].ChoiceText : "Decline";

            _hud.ShowPopup(message, acceptLabel, declineLabel);
            _hud.OnPopupAccept += OnAccept;
            _hud.OnPopupDecline += OnDecline;
            _waitingForChoice = true;
        }

        private void OnAccept()
        {
            if (!_waitingForChoice) return;
            _waitingForChoice = false;

            var choice = _currentEvent.Choices[0];
            if (choice != null)
            {
                ApplyChoice(choice);
            }

            _eventCompleted = true;
        }

        private void OnDecline()
        {
            if (!_waitingForChoice) return;
            _waitingForChoice = false;

            var choice = _currentEvent.Choices[1];
            if (choice != null)
            {
                ApplyChoice(choice);
            }

            _eventCompleted = true;
        }

        private void ApplyChoice(EventChoice choice)
        {
            // Check requirements
            if (choice.RequirePower > 0 && _player.Power < choice.RequirePower)
            {
                Debug.Log("Not enough Power!");
                return;
            }

            if (choice.RequireGold > 0 && _player.Gold < choice.RequireGold)
            {
                Debug.Log("Not enough Gold!");
                return;
            }

            if (choice.RequireWood > 0 && _player.Wood < choice.RequireWood)
            {
                Debug.Log("Not enough Wood!");
                return;
            }

            if (choice.RequireMetal > 0 && _player.Metal < choice.RequireMetal)
            {
                Debug.Log("Not enough Metal!");
                return;
            }

            // Pay costs
            _player.Power -= choice.RequirePower;
            _player.Gold -= choice.RequireGold;
            _player.Wood -= choice.RequireWood;
            _player.Metal -= choice.RequireMetal;
            _player.Food -= choice.RequireFood;
            _player.Influence -= choice.RequireInfluence;

            // Handle battle
            if (choice.IsBattle)
            {
                bool victory = _player.Power >= choice.BattleDifficulty;
                if (victory)
                {
                    Debug.Log("Battle won!");
                    choice.Effect.Apply(_player, _affectedTile, _map);
                }
                else
                {
                    Debug.Log("Battle lost!");
                    // Apply negative effect
                    _player.Power -= 1;
                }
            }
            else
            {
                // Apply effect
                if (choice.Effect != null)
                {
                    choice.Effect.Apply(_player, _affectedTile, _map);
                }
            }
        }
    }
}
using System;
using Map;
using Model;
using State_Machine;
using UI;
using UnityEngine;
using Events;
using DefaultNamespace;

namespace GameStates
{
    public class Occupy : IState
    {
        public Action OnTileCaptured;

        private MapController _map;
        private Hud _hud;
        private Player _player;
        private EventManager _eventManager;

        private Tile _selectedTile;
        private bool _waitingForConfirmation;
        private bool _captureCompleted;
        private bool _tileEventTriggered;

        public Occupy(MapController map, Hud hud, Player playerModel, EventManager eventManager)
        {
            _map = map;
            _hud = hud;
            _player = playerModel;
            _eventManager = eventManager;
        }

        public void Enter()
        {
            _waitingForConfirmation = false;
            _captureCompleted = false;
            _tileEventTriggered = false;
            _selectedTile = null;

            _map.SetTilesForOccupyInteractionState(true);
            SubscribeToTileClicks();
        }

        public void Execute()
        {
            if (_captureCompleted)
            {
                OnTileCaptured?.Invoke();
            }
        }

        public void Exit()
        {
            _map.SetTilesForOccupyInteractionState(false);
            UnsubscribeFromTileClicks();
            _hud.OnPopupAccept -= OnConfirmCapture;
            _hud.OnPopupDecline -= OnCancelCapture;
            _hud.HidePopup();
        }

        private void SubscribeToTileClicks()
        {
            var occupiedTiles = _map.GetOccupiedTiles();
            foreach (var tile in occupiedTiles)
            {
                var neighborOffsets = new[]
                {
                    new Vector2Int(1, -1), new Vector2Int(0, -1), new Vector2Int(-1, 0),
                    new Vector2Int(-1, 1), new Vector2Int(0, 1), new Vector2Int(1, 0)
                };

                foreach (var offset in neighborOffsets)
                {
                    var tilePos = tile.GetTilePosition();
                    var neighborCoords = new Vector2Int(tilePos.y, tilePos.z) + offset;
                    var neighborTile = _map.GetTileAt(neighborCoords);
                    
                    if (neighborTile != null && !neighborTile.IsOccupied)
                    {
                        neighborTile.OnClicked += () => OnTileClicked(neighborTile);
                        neighborTile.OnHoverEnter += () => OnTileHoverEnter(neighborTile);
                        neighborTile.OnHoverExit += () => OnTileHoverExit(neighborTile);
                    }
                }
            }
        }

        private void UnsubscribeFromTileClicks()
        {
            // Note: In production, you'd want to properly unsubscribe
            // For now, the Exit clears the state
        }

        private void OnTileClicked(Tile tile)
        {
            if (_waitingForConfirmation || _captureCompleted) return;
            if (tile.Config == null) return;

            _selectedTile = tile;
            ShowCaptureConfirmation();
        }

        private void OnTileHoverEnter(Tile tile)
        {
            if (_waitingForConfirmation || _captureCompleted) return;
            if (tile.Config == null) return;

            string info = $"{tile.Config.DisplayName}\n\n";
            info += $"Cost: {tile.Config.InfluenceCost} Influence";
            if (tile.Config.PowerCost > 0) info += $", {tile.Config.PowerCost} Power";
            if (tile.Config.WoodCost > 0) info += $", {tile.Config.WoodCost} Wood";
            if (tile.Config.GoldCost > 0) info += $", {tile.Config.GoldCost} Gold";
            if (tile.Config.MetalCost > 0) info += $", {tile.Config.MetalCost} Metal";

            info += "\n\nIncome per turn:\n";
            if (tile.Config.IncomeFood > 0) info += $"Food: +{tile.Config.IncomeFood}\n";
            if (tile.Config.IncomePower > 0) info += $"Power: +{tile.Config.IncomePower}\n";
            if (tile.Config.IncomeWood > 0) info += $"Wood: +{tile.Config.IncomeWood}\n";
            if (tile.Config.IncomeGold > 0) info += $"Gold: +{tile.Config.IncomeGold}\n";
            if (tile.Config.IncomeMetal > 0) info += $"Metal: +{tile.Config.IncomeMetal}\n";

            _hud.ShowTileInfo(info);
        }

        private void OnTileHoverExit(Tile tile)
        {
            _hud.HideTileInfo();
        }

        private void ShowCaptureConfirmation()
        {
            var config = _selectedTile.Config;
            
            string message = $"Capture {config.DisplayName}?\n\n";
            message += $"Cost: {config.InfluenceCost} Influence";
            
            if (config.PowerCost > 0) message += $", {config.PowerCost} Power";
            if (config.WoodCost > 0) message += $", {config.WoodCost} Wood";
            if (config.GoldCost > 0) message += $", {config.GoldCost} Gold";
            if (config.MetalCost > 0) message += $", {config.MetalCost} Metal";
            
            message += "\n\nRewards:\n";
            if (config.RewardInfluence != 0) message += $"Influence: {config.RewardInfluence:+0;-0}\n";
            if (config.RewardPower != 0) message += $"Power: {config.RewardPower:+0;-0}\n";
            if (config.RewardFood != 0) message += $"Food: {config.RewardFood:+0;-0}\n";

            _hud.ShowPopup(message, "Yes", "No");
            _hud.OnPopupAccept += OnConfirmCapture;
            _hud.OnPopupDecline += OnCancelCapture;
            _waitingForConfirmation = true;
        }

        private void OnConfirmCapture()
        {
            if (!_waitingForConfirmation) return;
            _waitingForConfirmation = false;

            _hud.OnPopupAccept -= OnConfirmCapture;
            _hud.OnPopupDecline -= OnCancelCapture;
            _hud.HidePopup();

            if (!CanAffordTile(_selectedTile))
            {
                _hud.ShowPopup("Not enough resources!", "OK", null);
                _hud.OnPopupAccept += () => { _selectedTile = null; };
                return;
            }

            CaptureTile(_selectedTile);
        }

        private void OnCancelCapture()
        {
            if (!_waitingForConfirmation) return;
            _waitingForConfirmation = false;

            _hud.OnPopupAccept -= OnConfirmCapture;
            _hud.OnPopupDecline -= OnCancelCapture;
            _hud.HidePopup();
            _selectedTile = null;
        }

        private bool CanAffordTile(Tile tile)
        {
            var config = tile.Config;
            if (_player.Influence < config.InfluenceCost) return false;
            if (_player.Power < config.PowerCost) return false;
            if (_player.Wood < config.WoodCost) return false;
            if (_player.Gold < config.GoldCost) return false;
            if (_player.Metal < config.MetalCost) return false;
            return true;
        }

        private void CaptureTile(Tile tile)
        {
            var config = tile.Config;

            // Pay costs
            _player.Influence -= config.InfluenceCost;
            _player.Power -= config.PowerCost;
            _player.Wood -= config.WoodCost;
            _player.Gold -= config.GoldCost;
            _player.Metal -= config.MetalCost;

            // Occupy tile
            var tilePos = tile.GetTilePosition();
            _map.OccupyTile(new Vector2Int(tilePos.y, tilePos.z));
            _player.TilesCaptured++;

            // Apply rewards
            _player.Influence += config.RewardInfluence;
            _player.Power += config.RewardPower;
            _player.Food += config.RewardFood;
            _player.Wood += config.RewardWood;
            _player.Gold += config.RewardGold;
            _player.Metal += config.RewardMetal;

            // Check for castle capture
            if (config.IsCastle)
            {
                _player.CapturedCastles++;
            }

            // Handle special tile events
            HandleTileEvents(tile);
        }

        private void HandleTileEvents(Tile tile)
        {
            if (tile.Config.IsChest)
            {
                // Chest gives random resource bonus
                int randomResource = UnityEngine.Random.Range(0, 3);
                switch (randomResource)
                {
                    case 0: _player.Wood += 5; break;
                    case 1: _player.Gold += 5; break;
                    case 2: _player.Metal += 5; break;
                }
                _hud.ShowPopup("Chest opened! +5 random resource", "OK", null);
                _hud.OnPopupAccept += () => { _captureCompleted = true; };
            }
            else if (tile.Config.TriggersTavernEvent)
            {
                TriggerTavernEvent();
            }
            else if (tile.Config.TriggersCampEvent)
            {
                TriggerCampEvent();
            }
            else
            {
                _captureCompleted = true;
            }
        }

        private void TriggerTavernEvent()
        {
            var tavernEvent = _eventManager.GetRandomTavernEvent();
            if (tavernEvent == null)
            {
                _captureCompleted = true;
                return;
            }

            ShowEventPopup(tavernEvent);
        }

        private void TriggerCampEvent()
        {
            var campEvent = _eventManager.GetRandomCampEvent();
            if (campEvent == null)
            {
                _captureCompleted = true;
                return;
            }

            ShowEventPopup(campEvent);
        }

        private void ShowEventPopup(EventData eventData)
        {
            string message = $"{eventData.EventName}\n\n{eventData.EventDescription}";
            string acceptLabel = eventData.Choices[0]?.ChoiceText ?? "Accept";
            string declineLabel = eventData.Choices[1]?.ChoiceText ?? "Decline";

            _hud.ShowPopup(message, acceptLabel, declineLabel);
            _hud.OnPopupAccept += () => ApplyEventChoice(eventData, 0);
            _hud.OnPopupDecline += () => ApplyEventChoice(eventData, 1);
        }

        private void ApplyEventChoice(EventData eventData, int choiceIndex)
        {
            _hud.OnPopupAccept -= () => ApplyEventChoice(eventData, 0);
            _hud.OnPopupDecline -= () => ApplyEventChoice(eventData, 1);

            var choice = eventData.Choices[choiceIndex];
            if (choice != null)
            {
                // Check requirements
                if (choice.RequirePower > 0 && _player.Power < choice.RequirePower)
                {
                    _hud.ShowPopup("Not enough Power!", "OK", null);
                    _hud.OnPopupAccept += () => { _captureCompleted = true; };
                    return;
                }
                if (choice.RequireGold > 0 && _player.Gold < choice.RequireGold)
                {
                    _hud.ShowPopup("Not enough Gold!", "OK", null);
                    _hud.OnPopupAccept += () => { _captureCompleted = true; };
                    return;
                }
                if (choice.RequireWood > 0 && _player.Wood < choice.RequireWood)
                {
                    _hud.ShowPopup("Not enough Wood!", "OK", null);
                    _hud.OnPopupAccept += () => { _captureCompleted = true; };
                    return;
                }
                if (choice.RequireMetal > 0 && _player.Metal < choice.RequireMetal)
                {
                    _hud.ShowPopup("Not enough Metal!", "OK", null);
                    _hud.OnPopupAccept += () => { _captureCompleted = true; };
                    return;
                }

                // Pay costs
                _player.Power -= choice.RequirePower;
                _player.Gold -= choice.RequireGold;
                _player.Wood -= choice.RequireWood;
                _player.Metal -= choice.RequireMetal;

                // Handle battle
                if (choice.IsBattle)
                {
                    bool victory = _player.Power >= choice.BattleDifficulty;
                    if (victory)
                    {
                        if (choice.Effect != null)
                        {
                            choice.Effect.Apply(_player, _selectedTile, _map);
                        }
                    }
                    else
                    {
                        // Apply negative effect for losing
                        _player.Power -= 1;
                    }
                }
                else
                {
                    // Apply effect
                    if (choice.Effect != null)
                    {
                        choice.Effect.Apply(_player, _selectedTile, _map);
                    }
                }
            }

            _captureCompleted = true;
        }
    }
}
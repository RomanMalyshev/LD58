using System;
using System.Collections.Generic;
using State_Machine;
using Map;
using Model;
using UI;

namespace GameStates
{
    public class TileUpgrade : IState
    {
        public Action OnUpgradeCompleted;

        private MapController _map;
        private Hud _hud;
        private Player _player;

        private List<Tile> _upgradeableTiles;
        private Tile _selectedTile;
        private bool _waitingForConfirmation;
        private bool _upgradeCompleted;

        public TileUpgrade(MapController map, Hud hud, Player player)
        {
            _map = map;
            _hud = hud;
            _player = player;
        }

        public void Enter()
        {
            _waitingForConfirmation = false;
            _upgradeCompleted = false;
            _selectedTile = null;

            _upgradeableTiles = _map.GetUpgradeableTiles();

            if (_upgradeableTiles.Count == 0)
            {
                // No tiles can be upgraded, skip this phase
                _upgradeCompleted = true;
                return;
            }

            ShowUpgradePrompt();
            SetupUpgradeableTilesInteraction();
        }

        public void Execute()
        {
            if (_upgradeCompleted)
            {
                OnUpgradeCompleted?.Invoke();
            }
        }

        public void Exit()
        {
            CleanupUpgradeableTilesInteraction();
            
            // Deselect tile on exit
            if (_selectedTile != null)
            {
                _selectedTile.SetSelected(false);
            }
            
            _hud.OnPopupAccept -= OnConfirmUpgrade;
            _hud.OnPopupDecline -= OnCancelUpgrade;
            _hud.OnPopupDecline -= OnSkipUpgrade;
            _hud.OnPopupAccept -= OnReturnToSelection;
            _hud.HideTileInfo();
            _hud.HidePopup();
        }

        private void ShowUpgradePrompt()
        {
            string message = "Choose a tile to upgrade.\n\n";
            message += $"Available: {_upgradeableTiles.Count} tile(s)\n\n";
            message += "Click on a highlighted tile to see upgrade details.";

            _hud.ShowPopup(message, null, "Skip");
            _hud.OnPopupDecline += OnSkipUpgrade;
        }

        private void SetupUpgradeableTilesInteraction()
        {
            foreach (var tile in _upgradeableTiles)
            {
                tile.SetInteractable(true);// Визуально подсвечиваем тайл
                tile.OnClicked += () => OnTileClicked(tile);
                tile.OnHoverEnter += () => OnTileHoverEnter(tile);
                tile.OnHoverExit += () => OnTileHoverExit(tile);
                tile.SetReadyToUpgrade(true); 
            }
        }

        private void CleanupUpgradeableTilesInteraction()
        {
            foreach (var tile in _upgradeableTiles)
            {
                tile.SetInteractable(false);
                tile.SetReadyToUpgrade(false); 
            }
        }

        private void OnTileClicked(Tile tile)
        {
            if (_waitingForConfirmation || _upgradeCompleted) return;
            if (tile.Config == null) return;

            // Deselect previous tile
            if (_selectedTile != null)
            {
                _selectedTile.SetSelected(false);
            }

            // Select new tile
            _selectedTile = tile;
            _selectedTile.SetSelected(true);
            ShowUpgradeConfirmation();
        }

        private void OnTileHoverEnter(Tile tile)
        {
            if (_waitingForConfirmation || _upgradeCompleted) return;
            if (tile.Config == null) return;

            string info = $"{tile.Config.DisplayName} (Level {tile.UpgradeLevel})\n\n";
            info += "Upgrade Cost:\n";
            
            if (tile.Config.UpgradeWoodCost > 0) 
                info += $"<Sprite index=1> {tile.Config.UpgradeWoodCost}\n";
            if (tile.Config.UpgradeGoldCost > 0) 
                info += $"<Sprite index=3> {tile.Config.UpgradeGoldCost}\n";
            if (tile.Config.UpgradeMetalCost > 0) 
                info += $"<Sprite index=4> {tile.Config.UpgradeMetalCost}\n";

            info += "\nUpgrade Bonus:\n";
            if (tile.Config.UpgradeFoodBonus > 0) 
                info += $"<Sprite index=2> +{tile.Config.UpgradeFoodBonus}\n";
            if (tile.Config.UpgradePowerBonus > 0) 
                info += $"<Sprite index=5> +{tile.Config.UpgradePowerBonus}\n";
            if (tile.Config.UpgradeWoodBonus > 0) 
                info += $"<Sprite index=1> +{tile.Config.UpgradeWoodBonus}\n";
            if (tile.Config.UpgradeGoldBonus > 0) 
                info += $"<Sprite index=3> +{tile.Config.UpgradeGoldBonus}\n";
            if (tile.Config.UpgradeMetalBonus > 0) 
                info += $"<Sprite index=4> +{tile.Config.UpgradeMetalBonus}\n";

            _hud.ShowTileInfo(info);
        }

        private void OnTileHoverExit(Tile tile)
        {
            _hud.HideTileInfo();
        }

        private void ShowUpgradeConfirmation()
        {
            var config = _selectedTile.Config;
            
            string message = $"Upgrade {config.DisplayName}?\n";
            message += $"Current Level: {_selectedTile.UpgradeLevel}\n\n";
            
            message += "Cost: ";
            bool hasCost = false;
            if (config.UpgradeWoodCost > 0) 
            {
                message += $"{config.UpgradeWoodCost} <Sprite index=1>";
                hasCost = true;
            }
            if (config.UpgradeGoldCost > 0) 
            {
                if (hasCost) message += ", ";
                message += $"{config.UpgradeGoldCost} <Sprite index=3>";
                hasCost = true;
            }
            if (config.UpgradeMetalCost > 0) 
            {
                if (hasCost) message += ", ";
                message += $"{config.UpgradeMetalCost} <Sprite index=4>";
            }

            message += "\n\nBonus per turn:\n";
            if (config.UpgradeFoodBonus > 0) 
                message += $"<Sprite index=2> +{config.UpgradeFoodBonus}\n";
            if (config.UpgradePowerBonus > 0) 
                message += $"<Sprite index=5> +{config.UpgradePowerBonus}\n";
            if (config.UpgradeWoodBonus > 0) 
                message += $"<Sprite index=1> +{config.UpgradeWoodBonus}\n";
            if (config.UpgradeGoldBonus > 0) 
                message += $"<Sprite index=3> +{config.UpgradeGoldBonus}\n";
            if (config.UpgradeMetalBonus > 0) 
                message += $"<Sprite index=4> +{config.UpgradeMetalBonus}\n";

            // Скрываем начальный popup
            _hud.HidePopup();
            _hud.OnPopupDecline -= OnSkipUpgrade;

            _hud.ShowPopup(message, "Upgrade", "Cancel");
            _hud.OnPopupAccept += OnConfirmUpgrade;
            _hud.OnPopupDecline += OnCancelUpgrade;
            _waitingForConfirmation = true;
        }

        private void OnConfirmUpgrade()
        {
            if (!_waitingForConfirmation) return;
            _waitingForConfirmation = false;

            _hud.OnPopupAccept -= OnConfirmUpgrade;
            _hud.OnPopupDecline -= OnCancelUpgrade;
            _hud.HidePopup();

            if (!CanAffordUpgrade(_selectedTile))
            {
                _hud.ShowPopup("Not enough resources!", "OK", null);
                _hud.OnPopupAccept += OnReturnToSelection;
                return;
            }

            UpgradeTile(_selectedTile);
        }

        private void OnCancelUpgrade()
        {
            if (!_waitingForConfirmation) return;
            _waitingForConfirmation = false;

            _hud.OnPopupAccept -= OnConfirmUpgrade;
            _hud.OnPopupDecline -= OnCancelUpgrade;
            _hud.HidePopup();
            
            // Deselect tile
            if (_selectedTile != null)
            {
                _selectedTile.SetSelected(false);
            }
            
            _selectedTile = null;
            ShowUpgradePrompt();
        }

        private void OnReturnToSelection()
        {
            _hud.OnPopupAccept -= OnReturnToSelection;
            _hud.HidePopup();
            
            // Deselect tile
            if (_selectedTile != null)
            {
                _selectedTile.SetSelected(false);
            }
            
            _selectedTile = null;
            ShowUpgradePrompt();
        }

        private void OnSkipUpgrade()
        {
            _hud.OnPopupDecline -= OnSkipUpgrade;
            _hud.HidePopup();
            _upgradeCompleted = true;
        }

        private bool CanAffordUpgrade(Tile tile)
        {
            var config = tile.Config;
            if (_player.Wood < config.UpgradeWoodCost) return false;
            if (_player.Gold < config.UpgradeGoldCost) return false;
            if (_player.Metal < config.UpgradeMetalCost) return false;
            return true;
        }

        private void UpgradeTile(Tile tile)
        {
            var config = tile.Config;

            // Deselect tile
            tile.SetSelected(false);

            // Pay costs
            _player.Wood -= config.UpgradeWoodCost;
            _player.Gold -= config.UpgradeGoldCost;
            _player.Metal -= config.UpgradeMetalCost;

            // Upgrade tile
            tile.UpgradeTile();

            // Update income after upgrade
            _player.TriggerIncomeUpdate();

            string message = $"{config.DisplayName} upgraded to Level {tile.UpgradeLevel}!";
            _hud.ShowPopup(message, "OK", null);
            _hud.OnPopupAccept += () => { _upgradeCompleted = true; };
        }
    }
}
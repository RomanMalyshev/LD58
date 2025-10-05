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
        private bool _waitingForChoice;
        private bool _upgradeCompleted;

        public TileUpgrade(MapController map, Hud hud, Player player)
        {
            _map = map;
            _hud = hud;
            _player = player;
        }

        public void Enter()
        {
            _waitingForChoice = false;
            _upgradeCompleted = false;
            _selectedTile = null;

            _upgradeableTiles = _map.GetUpgradeableTiles();

            if (_upgradeableTiles.Count == 0)
            {
                // No tiles can be upgraded, skip this phase
                _upgradeCompleted = true;
                return;
            }

            ShowUpgradeOptions();
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
            _hud.OnPopupAccept -= OnAcceptUpgrade;
            _hud.OnPopupDecline -= OnSkipUpgrade;
            _hud.HidePopup();
        }

        private void ShowUpgradeOptions()
        {
            string message = "Would you like to upgrade a tile?\n\n";
            message += "Upgradeable tiles:\n";

            foreach (var tile in _upgradeableTiles)
            {
                var config = tile.Config;
                message += $"- {config.DisplayName} (Lvl {tile.UpgradeLevel})\n";
                message += $"  Cost: ";
                
                if (config.UpgradeWoodCost > 0) message += $"{config.UpgradeWoodCost} Wood ";
                if (config.UpgradeGoldCost > 0) message += $"{config.UpgradeGoldCost} Gold ";
                if (config.UpgradeMetalCost > 0) message += $"{config.UpgradeMetalCost} Metal ";
                
                message += "\n";
            }

            _hud.ShowPopup(message, "Upgrade", "Skip");
            _hud.OnPopupAccept += OnAcceptUpgrade;
            _hud.OnPopupDecline += OnSkipUpgrade;
            _waitingForChoice = true;
        }

        private void OnAcceptUpgrade()
        {
            if (!_waitingForChoice) return;
            _waitingForChoice = false;

            _hud.OnPopupAccept -= OnAcceptUpgrade;
            _hud.OnPopupDecline -= OnSkipUpgrade;

            // For simplicity, upgrade first affordable tile
            Tile tileToUpgrade = null;
            foreach (var tile in _upgradeableTiles)
            {
                if (CanAffordUpgrade(tile))
                {
                    tileToUpgrade = tile;
                    break;
                }
            }

            if (tileToUpgrade != null)
            {
                UpgradeTile(tileToUpgrade);
            }
            else
            {
                _hud.ShowPopup("Not enough resources to upgrade!", "OK", null);
                _hud.OnPopupAccept += () => { _upgradeCompleted = true; };
            }
        }

        private void OnSkipUpgrade()
        {
            if (!_waitingForChoice) return;
            _waitingForChoice = false;

            _hud.OnPopupAccept -= OnAcceptUpgrade;
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

            // Pay costs
            _player.Wood -= config.UpgradeWoodCost;
            _player.Gold -= config.UpgradeGoldCost;
            _player.Metal -= config.UpgradeMetalCost;

            // Upgrade tile
            tile.UpgradeTile();

            string message = $"{config.DisplayName} upgraded to Level {tile.UpgradeLevel}!";
            _hud.ShowPopup(message, "OK", null);
            _hud.OnPopupAccept += () => { _upgradeCompleted = true; };
        }
    }
}
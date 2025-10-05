using System;
using UnityEngine;
using Model;
using Map;

namespace Events
{
    [Serializable]
    public class EventEffect
    {
        [Header("Resource Changes")]
        public int InfluenceChange = 0;
        public int PowerChange = 0;
        public int FoodChange = 0;
        public int WoodChange = 0;
        public int GoldChange = 0;
        public int MetalChange = 0;

        [Header("Special Effects")]
        public bool LoseTile = false;
        public bool RandomResourceBonus = false;
        public int RandomResourceAmount = 5;
        public bool RandomInfluenceChange = false; // ±1
        public bool ConvertShaftToGold = false;

        public void Apply(Player player, Tile affectedTile = null, MapController map = null)
        {
            // Apply resource changes
            player.Influence += InfluenceChange;
            player.Power += PowerChange;
            player.Food += FoodChange;
            player.Wood += WoodChange;
            player.Gold += GoldChange;
            player.Metal += MetalChange;

            // Handle special effects
            if (RandomResourceBonus)
            {
                ApplyRandomResourceBonus(player);
            }

            if (RandomInfluenceChange)
            {
                int change = UnityEngine.Random.Range(0, 2) == 0 ? -1 : 1;
                player.Influence += change;
            }

            if (LoseTile && affectedTile != null && map != null)
            {
                var tilePos = affectedTile.GetTilePosition();
                map.DeOccupyTile(new Vector2Int(tilePos.y, tilePos.z));
                player.TilesCaptured--;
            }

            if (ConvertShaftToGold && affectedTile != null)
            {
                // This would require destroying the tile and creating a gold mine
                // For now, just log
                Debug.Log("Shaft converted to gold mine!");
            }
        }

        private void ApplyRandomResourceBonus(Player player)
        {
            int randomResource = UnityEngine.Random.Range(0, 3);
            switch (randomResource)
            {
                case 0:
                    player.Wood += RandomResourceAmount;
                    break;
                case 1:
                    player.Gold += RandomResourceAmount;
                    break;
                case 2:
                    player.Metal += RandomResourceAmount;
                    break;
            }
        }
    }
}


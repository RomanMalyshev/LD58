using System;
using DefaultNamespace;
using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "TilesConfig", menuName = "Configs/Tiles Config")]
    public class TilesConfig : ScriptableObject
    {
        [SerializeField] private TileConfig[] _tiles;

        public TileConfig[] Tiles => _tiles;

        public TileConfig GetConfigByType(TileType type)
        {
            foreach (var tile in _tiles)
            {
                if (tile.Type == type)
                    return tile;
            }
            Debug.LogWarning($"TileConfig for type {type} not found!");
            return null;
        }
    }

    [Serializable]
    public class TileConfig
    {
        [Header("Base Settings")]
        public TileType Type;
        public string DisplayName;
        [TextArea(2, 4)]
        public string Description;

        [Header("Occupation Costs")]
        public int InfluenceCost = 1;
        public int PowerCost = 0;
        public int WoodCost = 0;
        public int GoldCost = 0;
        public int MetalCost = 0;

        [Header("Immediate Rewards (on capture)")]
        public int RewardInfluence = 0;
        public int RewardPower = 0;
        public int RewardFood = 0;
        public int RewardWood = 0;
        public int RewardGold = 0;
        public int RewardMetal = 0;

        [Header("Passive Income (per turn)")]
        public int IncomeFood = 0;
        public int IncomePower = 0;
        public int IncomeWood = 0;
        public int IncomeGold = 0;
        public int IncomeMetal = 0;

        [Header("Upgrade Settings")]
        public bool CanBeUpgraded = false;
        public int UpgradeWoodCost = 0;
        public int UpgradeGoldCost = 0;
        public int UpgradeMetalCost = 0;
        
        [Header("Upgrade Bonuses (per level)")]
        public int UpgradeFoodBonus = 0;
        public int UpgradePowerBonus = 0;
        public int UpgradeWoodBonus = 0;
        public int UpgradeGoldBonus = 0;
        public int UpgradeMetalBonus = 0;

        [Header("Special Properties")]
        public bool IsWalkable = true;
        public bool IsCastle = false;
        public bool TriggersTavernEvent = false;
        public bool TriggersCampEvent = false;
        public bool IsChest = false;
    }
}
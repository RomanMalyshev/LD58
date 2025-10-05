// This file contains example configurations for tiles based on the GDD
// To use these, create a TilesConfig ScriptableObject asset in Unity Editor:
// Assets -> Create -> Configs -> Tiles Config

/*
===================
TILES CONFIGURATION GUIDE
===================

Create one TilesConfig asset with an array of TileConfig for each tile type:

-------------------
SIMPLE (Empty tiles: поляна, дорога, земля)
-------------------
Type: Simple
DisplayName: "Empty Land"
InfluenceCost: 1
CanBeOccupied: true
All other values: 0/false

-------------------
FIELD (Поле)
-------------------
Type: Field
DisplayName: "Field"
InfluenceCost: 1
IncomeFood: 2
CanBeUpgraded: true
UpgradeWoodCost: 5
UpgradeFoodBonus: 1

-------------------
VILLAGE (Деревня)
-------------------
Type: Village
DisplayName: "Village"
InfluenceCost: 1
IncomePower: 1
IncomeFood: -1 (villages consume food!)
CanBeUpgraded: true
UpgradeMetalCost: 5
UpgradePowerBonus: 1

-------------------
PLAYER CASTLE (Starting tile)
-------------------
Type: PlayerCastle
DisplayName: "Your Castle"
InfluenceCost: 0
IsCastle: true
CanBeOccupied: true

-------------------
ENEMY CASTLE (Замок)
-------------------
Type: EnemyCastle
DisplayName: "Enemy Castle"
InfluenceCost: 3
PowerCost: 5 (requires military strength)
RewardInfluence: 2
IsCastle: true
CanBeOccupied: true

-------------------
FOREST (Лес)
-------------------
Type: Forest
DisplayName: "Forest"
InfluenceCost: 1
IncomeWood: 1
CanBeUpgraded: true
UpgradeGoldCost: 5
UpgradeWoodBonus: 1

-------------------
MINE (Рудник - Gold)
-------------------
Type: Mine
DisplayName: "Gold Mine"
InfluenceCost: 1
IncomeGold: 1

-------------------
SHAFT (Шахта - Iron)
-------------------
Type: Shaft
DisplayName: "Iron Shaft"
InfluenceCost: 1
IncomeMetal: 1
CanBeUpgraded: true
UpgradeGoldCost: 5
UpgradeMetalBonus: 1

-------------------
CHEST (Сундук)
-------------------
Type: Chest
DisplayName: "Treasure Chest"
InfluenceCost: 1
IsChest: true
(Gives random +5 resource on capture)

-------------------
TAVERN (Таверна)
-------------------
Type: Tavern
DisplayName: "Tavern"
InfluenceCost: 1
TriggersTavernEvent: true

-------------------
CAMP (Лагерь)
-------------------
Type: Camp
DisplayName: "Camp"
InfluenceCost: 1
TriggersCampEvent: true

-------------------
RIVER (Река - special cost)
-------------------
Type: River
DisplayName: "River Crossing"
InfluenceCost: 1
WoodCost: 5
(Requires 5 Wood to build bridge)

-------------------
MOUNTAIN (Непроходимые)
-------------------
Type: Mountain
DisplayName: "Mountain"
CanBeOccupied: false
(Cannot be captured - impassable terrain)

===================
HOW TO CREATE IN UNITY:
===================

1. Right-click in Assets/Configs folder
2. Create -> Configs -> Tiles Config
3. Name it "MainTilesConfig"
4. Set Tiles array size to 13 (for all tile types)
5. Fill each element with configs above
6. Assign to GameFlow component in scene

===================
SETTING TILE TYPES ON MAP:
===================

For each Tile GameObject in your scene:
1. Select the tile in hierarchy
2. Find the Tile component
3. Set the "Tile Type" enum to match the tile's purpose
4. The config will be automatically assigned when game starts

Note: Make sure every tile in your scene has a corresponding
TileConfig in the TilesConfig asset!
*/

// This is just a documentation file. 
// The actual ScriptableObjects must be created in Unity Editor.


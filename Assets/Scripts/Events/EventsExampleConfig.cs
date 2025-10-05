// This file contains example configurations for events based on the GDD
// To use these, create ScriptableObject assets in Unity Editor:
// Assets -> Create -> Events -> Event Data
// Assets -> Create -> Events -> Events Pool

/*
===================
GLOBAL EVENTS (Passive - no choice)
===================

1. Drought (Засуха)
   - Event: Field doesn't produce food this turn
   - Effect: Specific field tile gets -IncomeFood for 1 turn

2. Disease (Болезнь)  
   - Event: Village doesn't produce power this turn
   - Effect: Specific village tile gets -IncomePower for 1 turn

3. Collapse (Обвал)
   - Event: Mine/Shaft doesn't produce resources this turn
   - Effect: Specific mine/shaft gets -Income for 1 turn

===================
GLOBAL EVENTS (With choice)
===================

4. Strike (Забастовка)
   EventID: "strike"
   EventName: "Strike!"
   EventDescription: "Workers at your tile are on strike! Pay 5 Gold to resolve it, or lose the tile."
   Choice 1: "Pay 5 Gold" -> RequireGold: 5, Effect: None
   Choice 2: "Refuse" -> Effect: LoseTile = true

5. Tile Rejoining (Присоединение тайла)
   EventID: "tile_return"
   EventName: "Tile Wants to Return"
   EventDescription: "A lost tile wants to rejoin your kingdom. Pay 1 Influence?"
   Choice 1: "Pay 1 Influence" -> RequireInfluence: 1, Effect: Regain tile
   Choice 2: "Refuse" -> Effect: None

===================
TAVERN EVENTS
===================

6. Fugitive Knight (Беглый рыцарь)
   EventID: "fugitive_knight"
   EventName: "Fugitive Knight"
   EventDescription: "A knight offers his service. Accept him for +3 Power but -1 Influence?"
   Choice 1: "Accept" -> Effect: PowerChange: +3, InfluenceChange: -1
   Choice 2: "Refuse" -> Effect: None

7. Musicians (Музыканты)
   EventID: "musicians"
   EventName: "Traveling Musicians"
   EventDescription: "Musicians offer entertainment. Hire them for random influence change (±1)?"
   Choice 1: "Hire" -> Effect: RandomInfluenceChange: true
   Choice 2: "Refuse" -> Effect: InfluenceChange: -2

8. Alchemist (Алхимик)
   EventID: "alchemist"
   EventName: "Mysterious Alchemist"
   EventDescription: "An alchemist offers to turn your shaft into gold mine. Accept?"
   Choice 1: "Accept" -> Effect: ConvertShaftToGold: true
   Choice 2: "Refuse" -> Effect: None

===================
CAMP EVENTS
===================

9. Workers Camp (Лагерь рабочих)
   EventID: "workers_camp"
   EventName: "Workers Camp"
   EventDescription: "Hire workers to randomly upgrade one of your resource tiles?"
   Choice 1: "Hire for 5 Gold" -> RequireGold: 5, Effect: Random tile upgrade
   Choice 2: "Refuse" -> Effect: None

10. Bandits Camp (Лагерь разбойников)
    EventID: "bandits_camp"
    EventName: "Bandits!"
    EventDescription: "You encountered bandits! Flee or fight?"
    Choice 1: "Fight" -> IsBattle: true, BattleDifficulty: 5, Effect: PowerChange: +1
    Choice 2: "Flee" -> Effect: Tile remains uncaptured

===================
HOW TO CREATE IN UNITY:
===================

1. Right-click in Assets/Configs folder
2. Create -> Events -> Event Data
3. Name it "Event_StrikeBandits"
4. Fill in the fields according to above specs

5. Create three pools:
   - GlobalEventsPool (add events 1-5)
   - TavernEventsPool (add events 6-8)
   - CampEventsPool (add events 9-10)

6. Assign pools to GameFlow component in scene
*/

// This is just a documentation file. 
// The actual ScriptableObjects must be created in Unity Editor.


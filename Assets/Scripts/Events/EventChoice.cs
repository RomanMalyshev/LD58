using System;
using UnityEngine;

namespace Events
{
    [Serializable]
    public class EventChoice
    {
        [Header("Choice Settings")]
        public string ChoiceText = "Accept";
        [TextArea(2, 3)]
        public string ChoiceDescription = "";

        [Header("Requirements (0 = no requirement)")]
        public int RequirePower = 0;
        public int RequireGold = 0;
        public int RequireWood = 0;
        public int RequireMetal = 0;
        public int RequireFood = 0;

        [Header("Effects")]
        public EventEffect Effect = new EventEffect();

        [Header("Battle Settings (for Camp events)")]
        public bool IsBattle = false;
        public int BattleDifficulty = 5; // Power required to win
    }
}


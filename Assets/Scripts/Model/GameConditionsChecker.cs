namespace Model
{
    public static class GameConditionsChecker
    {
        public const int CASTLES_TO_WIN = 1;
        public const int MAX_TURNS = 20;

        public static GameEndCondition CheckGameConditions(Player player)
        {
            // Check defeat conditions first
            if (player.Food <= 0)
            {
                return GameEndCondition.DefeatNoFood;
            }

            if (player.Influence <= 0)
            {
                return GameEndCondition.DefeatNoInfluence;
            }

            if (player.CurrentTurn > MAX_TURNS)
            {
                return GameEndCondition.DefeatTimeOut;
            }

            // Check victory condition
            if (player.CapturedCastles >= CASTLES_TO_WIN)
            {
                return GameEndCondition.Victory;
            }

            // Game continues
            return GameEndCondition.Continue;
        }
    }

    public enum GameEndCondition
    {
        Continue,
        Victory,
        DefeatNoFood,
        DefeatNoInfluence,
        DefeatTimeOut
    }
}


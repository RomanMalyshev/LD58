using System.Collections.Generic;
using Map;

namespace Model
{
    public static class ResourceCalculator
    {
        public static ResourceIncome CalculateTotalIncome(List<Tile> occupiedTiles)
        {
            var income = new ResourceIncome();

            foreach (var tile in occupiedTiles)
            {
                if (tile.Config == null) continue;

                income.Food += tile.GetTotalFoodIncome();
                income.Power += tile.GetTotalPowerIncome();
                income.Wood += tile.GetTotalWoodIncome();
                income.Gold += tile.GetTotalGoldIncome();
                income.Metal += tile.GetTotalMetalIncome();
            }

            return income;
        }

        public static int CalculateFoodConsumption(int power)
        {
            // Food consumption = Power × 1
            return power;
        }
    }

    public class ResourceIncome
    {
        public int Food;
        public int Power;
        public int Wood;
        public int Gold;
        public int Metal;
    }
}


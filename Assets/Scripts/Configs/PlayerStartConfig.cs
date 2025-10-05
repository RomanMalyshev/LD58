using UnityEngine;

namespace Configs
{
    [CreateAssetMenu(fileName = "PlayerStartConfig", menuName = "Configs/Player Start Config")]
    public class PlayerStartConfig : ScriptableObject
    {
        [Header("Starting Resources")]
        [SerializeField] private int _influence = 20;
        [SerializeField] private int _power = 0;
        [SerializeField] private int _food = 0;
        [SerializeField] private int _gold = 0;
        [SerializeField] private int _metal = 0;
        [SerializeField] private int _wood = 0;

        public int Influence => _influence;
        public int Power => _power;
        public int Food => _food;
        public int Gold => _gold;
        public int Metal => _metal;
        public int Wood => _wood;
    }
}


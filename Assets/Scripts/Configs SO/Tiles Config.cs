using System;
using DefaultNamespace;
using UnityEngine;

[CreateAssetMenu(fileName = "TilesConfig", menuName = "Scriptable Objects/TilesConfig")]
public class TilesConfig : ScriptableObject
{
    [Serializable]
    public class TileConfig
    {
        public TileType Type;
        
    }
}
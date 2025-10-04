using System;
using System.Collections.Generic;
using UnityEngine;
using View.Map;

namespace RedBjorn.ProtoTiles
{
    public class MapView : MonoBehaviour
    {
        GameObject Grid;
        public Dictionary<Vector3Int, TileView> Tiles = new ();

        public void Awake()
        {
            var tiles = GetComponentsInChildren<TileView>();
            foreach (var tile in tiles)
            {
                Tiles[tile.GetTilePosition()] = tile;
            }
        }

        public void Init(MapEntity map)
        {
            Grid = new GameObject("Grid");
            Grid.transform.SetParent(transform);
            Grid.transform.localPosition = Vector3.zero;
            map.CreateGrid(Grid.transform);
        }

        public void GridEnable(bool enable)
        {
            if (Grid)
            {
                Grid.SetActive(enable);
            }
            else
            {
                Log.E($"Can't enable Grid state: {enable}. It wasn't created");
            }
        }

        public void GridToggle()
        {
            if (Grid)
            {
                Grid.SetActive(!Grid.activeSelf);
            }
            else
            {
                Log.E("Can't toggle Grid state. It wasn't created");
            }
        }
    }
}
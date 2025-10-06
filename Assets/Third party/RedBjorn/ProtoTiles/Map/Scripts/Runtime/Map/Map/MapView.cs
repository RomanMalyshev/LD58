using System.Collections;
using System.Collections.Generic;
using Map;
using UnityEngine;
using View.Map;

namespace RedBjorn.ProtoTiles
{
    public class MapView : MonoBehaviour
    {
        GameObject Grid;
        public Dictionary<Vector3Int, Tile> Tiles = new ();
        
        [SerializeField] private Color _radiusHighlightColor = Color.cyan;
        [SerializeField] private float _radiusDisplayDuration = 1.5f;
        [SerializeField] private float _fadeInDuration = 0.5f;
        [SerializeField] private float _fadeOutDuration = 0.5f;

        public void Awake()
        {
            var tiles = GetComponentsInChildren<Tile>();
            foreach (var tile in tiles)
            {
                Tiles[tile.GetTilePosition()] = tile;
            }
        }

        [ContextMenu("Set Order Layer")]
        public void ResetLayerOrder()
        {
            var tiles = GetComponentsInChildren<Tile>();
            foreach (var tileView in tiles)
            {
                tileView.UpdateOrderLayer(60 );
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
        
        public List<Tile> GetTilesAtRadius(int radius)
        {
            var tilesByRadius = GetTilesByRadius(Vector3Int.zero, radius);
            return tilesByRadius.ContainsKey(radius) ? tilesByRadius[radius] : new List<Tile>();
        }
        
        public Dictionary<int, List<Tile>> GetTilesByRadius(Vector3Int centerTile, int maxRadius = -1)
        {
            var tiles = GetComponentsInChildren<Tile>();
            foreach (var tile in tiles)
            {
                Tiles[tile.GetTilePosition()] = tile;
            }
            var result = new Dictionary<int, List<Tile>>();
            var visited = new HashSet<Vector3Int>();
            var queue = new Queue<(Vector3Int position, int radius)>();

            // Проверяем, что центральный тайл существует
            if (!Tiles.ContainsKey(centerTile))
            {
                Log.E($"Center tile at position {centerTile} not found in map");
                return result;
            }

            // Добавляем центральный тайл (радиус 0)
            queue.Enqueue((centerTile, 0));
            visited.Add(centerTile);

            while (queue.Count > 0)
            {
                var (currentPos, currentRadius) = queue.Dequeue();

                // Если достигли максимального радиуса, пропускаем
                if (maxRadius >= 0 && currentRadius > maxRadius)
                {
                    continue;
                }

                // Добавляем текущий тайл в результат
                if (!result.ContainsKey(currentRadius))
                {
                    result[currentRadius] = new List<Tile>();
                }
                result[currentRadius].Add(Tiles[currentPos]);

                // Если достигли максимального радиуса, не добавляем соседей
                if (maxRadius >= 0 && currentRadius >= maxRadius)
                {
                    continue;
                }

                // Добавляем всех соседей
                foreach (var neighborOffset in Hex.Neighbour)
                {
                    var neighborPos = currentPos + neighborOffset;

                    // Проверяем, что сосед существует и ещё не посещён
                    if (Tiles.ContainsKey(neighborPos) && !visited.Contains(neighborPos))
                    {
                        visited.Add(neighborPos);
                        queue.Enqueue((neighborPos, currentRadius + 1));
                    }
                }
            }

            return result;
        }

    
    }
}
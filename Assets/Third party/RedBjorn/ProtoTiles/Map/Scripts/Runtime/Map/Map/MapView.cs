using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using View.Map;

namespace RedBjorn.ProtoTiles
{
    public class MapView : MonoBehaviour
    {
        GameObject Grid;
        public Dictionary<Vector3Int, TileView> Tiles = new ();
        
        [SerializeField] private Color _radiusHighlightColor = Color.cyan;
        [SerializeField] private float _radiusDisplayDuration = 1.5f;
        [SerializeField] private float _fadeInDuration = 0.5f;
        [SerializeField] private float _fadeOutDuration = 0.5f;

        public void Awake()
        {
            var tiles = GetComponentsInChildren<TileView>();
            foreach (var tile in tiles)
            {
                Tiles[tile.GetTilePosition()] = tile;
            }
        }

        [ContextMenu("Set Order Layer")]
        public void Reset()
        {
            var tiles = GetComponentsInChildren<TileView>();
            foreach (var tileView in tiles)
            {
                tileView.UpdateOrderLayer(20  );
            }
        }

        public void Init(MapEntity map)
        {
            Grid = new GameObject("Grid");
            Grid.transform.SetParent(transform);
            Grid.transform.localPosition = Vector3.zero;
            map.CreateGrid(Grid.transform);
            StartCoroutine(VisualizeRadiuses());
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

        
        private IEnumerator VisualizeRadiuses()
        {
            var centerPos = Vector3Int.zero;
            
            var allTilesByRadius = GetTilesByRadius(centerPos, -1);
            int maxRadius = 0;
            foreach (var radius in allTilesByRadius.Keys)
            {
                if (radius > maxRadius) maxRadius = radius;
            }

            for (int radius = 0; radius <= maxRadius; radius++)
            {
                if (!allTilesByRadius.ContainsKey(radius))
                {
                    continue;
                }

                var tiles = allTilesByRadius[radius];

                yield return FadeTiles(tiles, Color.white, _radiusHighlightColor, _fadeInDuration);

                yield return new WaitForSeconds(_radiusDisplayDuration);

                yield return FadeTiles(tiles, _radiusHighlightColor, Color.white, _fadeOutDuration);
            }

            ResetAllTileColors();
            StartCoroutine(VisualizeRadiuses());
        }

        private IEnumerator FadeTiles(List<TileView> tiles, Color fromColor, Color toColor, float duration)
        {
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                Color currentColor = Color.Lerp(fromColor, toColor, t);

                foreach (var tile in tiles)
                {
                    if (tile != null)
                    {
                        tile.SetColor(currentColor);
                    }
                }

                yield return null;
            }

            // Устанавливаем финальный цвет
            foreach (var tile in tiles)
            {
                if (tile != null)
                {
                    tile.SetColor(toColor);
                }
            }
        }

        private void ResetAllTileColors()
        {
            foreach (var tilePair in Tiles)
            {
                if (tilePair.Value != null)
                {
                    tilePair.Value.ResetColor();
                }
            }
        }

        public List<TileView> GetTilesAtRadius(Vector3Int centerTile, int radius)
        {
            var tilesByRadius = GetTilesByRadius(centerTile, radius);
            return tilesByRadius.ContainsKey(radius) ? tilesByRadius[radius] : new List<TileView>();
        }
        
        public List<TileView> GetTilesAtRadius(int radius)
        {
            return GetTilesAtRadius(Vector3Int.zero, radius);
        }
        
        private Dictionary<int, List<TileView>> GetTilesByRadius(Vector3Int centerTile, int maxRadius = -1)
        {
            var tiles = GetComponentsInChildren<TileView>();
            foreach (var tile in tiles)
            {
                Tiles[tile.GetTilePosition()] = tile;
            }
            var result = new Dictionary<int, List<TileView>>();
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
                    result[currentRadius] = new List<TileView>();
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
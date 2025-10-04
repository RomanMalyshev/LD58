using Model.Map;
using RedBjorn.ProtoTiles;
using UnityEngine;

public static class MapEditorToGameMap
{
    public static MapModel GetMapModel(MapSettings mapSettings, MapView mapView)
    {
        MapEntity map = new MapEntity(mapSettings, mapView);
        mapView.Init(map);

        MapModel mapModel = new();
        foreach (var tile in map.Tiles)
        {
            //TODO: create transfer function
            //if (tile.Value.Preset.Id == "Forest")
            mapModel.Tiles[new Vector2Int(tile.Key.y, tile.Key.z)] = new EmptyTile();
        }

        return mapModel;
    }
    
    
}
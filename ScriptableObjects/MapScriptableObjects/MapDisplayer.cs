using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDisplayer", menuName = "ScriptableObjects/MapDisplayer", order = 1)]
public class MapDisplayer : ScriptableObject
{
    public int layer = 0;
    public List<SpriteContainer> layerSprites;
    
    public void DisplayCurrentTiles(List<MapTile> mapTiles, List<string> mapInfo, List<int> currentTiles)
    {
        int nextTile = -1;
        for (int i = 0; i < (mapTiles.Count); i++)
        {
            nextTile = currentTiles[i];
            if (nextTile < 0 || mapInfo[i].Length < 1)
            {
                mapTiles[i].ResetLayerSprite(layer);
                continue;
            }
            mapTiles[i].UpdateLayerSprite(layerSprites[layer].SpriteDictionary(mapInfo[nextTile]), layer);
        }
    }

    public void HighlightCurrentTiles(List<MapTile> mapTiles, List<string> mapInfo, List<int> currentTiles)
    {
        int nextTile = -1;
        for (int i = 0; i < (mapTiles.Count); i++)
        {
            nextTile = currentTiles[i];
            if (nextTile < 0)
            {
                mapTiles[i].HighlightLayer(layer);
                continue;
            }
            mapTiles[i].HighlightLayer(layer, mapInfo[nextTile]);
        }
    }

    public void HighlightTilesInSetColor(List<MapTile> mapTiles, List<int> mapInfo, List<int> currentTiles, string color)
    {
        int nextTile = -1;
        for (int i = 0; i < (mapTiles.Count); i++)
        {
            nextTile = currentTiles[i];
            if (nextTile < 0)
            {
                mapTiles[i].HighlightLayer(layer);
                continue;
            }
            if (mapInfo.IndexOf(nextTile) >= 0)
            {
                mapTiles[i].HighlightLayer(layer, color);
            }
        }
    }
}

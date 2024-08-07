using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDisplayer", menuName = "ScriptableObjects/MapDisplayer", order = 1)]
public class MapDisplayer : ScriptableObject
{
    public MapUtility mapUtility;
    public int mapSize;
    public int gridSize = 9;
    public int layer = 0;
    public List<string> mapInfo;
    public List<int> currentTiles;
    public List<MapTile> mapTiles;
    public List<SpriteContainer> layerSprites;

    public void SetMapTiles(List<MapTile> newTiles)
    {
        mapTiles = newTiles;
        gridSize = (int) Mathf.Sqrt(mapTiles.Count);
    }

    public void SetMapInfo(List<string> newInfo)
    {
        mapInfo = newInfo;
        mapSize = (int) Mathf.Sqrt(mapInfo.Count);
    }

    public void UpdateMap(int nextRow, int nextCol)
    {
        // Need to make sure the corner is in the right spot.
        currentTiles.Clear();
        int cRow = nextRow;
        int cCol = nextCol;
        int nextTile = mapUtility.ReturnTileNumberFromRowCol(cRow, cCol, mapSize);
        int tileNumber = 0;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                if (nextTile < 0)
                {
                    mapTiles[tileNumber].ResetLayerSprite(layer);
                }
                else
                {
                    mapTiles[tileNumber].UpdateLayerSprite(layerSprites[layer].SpriteDictionary(mapInfo[nextTile]), layer);
                }
                currentTiles.Add(nextTile);
                tileNumber++;
                cCol++;
                nextTile = mapUtility.ReturnTileNumberFromRowCol(cRow, cCol, mapSize);
            }
            cCol -= gridSize;
            cRow++;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapDisplayer", menuName = "ScriptableObjects/MapDisplayer", order = 1)]
public class MapDisplayer : ScriptableObject
{
    public MapUtility mapUtility;
    public int gridSize = 9;
    public int layer = 0;
    public List<string> mapInfo;
    public List<int> currentTiles;
    public List<SpriteContainer> layerSprites;

    public void UpdateMapGivenCenter(int centerTile, int size, List<MapTile> mapTiles, List<string> mapInfo)
    {
        int row = mapUtility.GetRow(centerTile, size);
        int col = mapUtility.GetColumn(centerTile, size);
        UpdateMap(row, col, size, mapTiles, mapInfo);
    }
    
    protected void UpdateMap(int nextRow, int nextCol, int size, List<MapTile> mapTiles, List<string> mapInfo)
    {
        // Need to make sure the corner is in the right spot.
        currentTiles.Clear();
        int cRow = nextRow;
        int cCol = nextCol;
        int nextTile = mapUtility.ReturnTileNumberFromRowCol(cRow, cCol, size);
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
                nextTile = mapUtility.ReturnTileNumberFromRowCol(cRow, cCol, size);
            }
            cCol -= gridSize;
            cRow++;
        }
    }
}

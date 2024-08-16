using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrentTiles", menuName = "ScriptableObjects/CurrentTiles", order = 1)]
public class MapCurrentTiles : ScriptableObject
{
    public MapUtility mapUtility;
    
    public List<int> GetCurrentTiles(int start, int mapSize, int gridSize)
    {
        List<int> currentTiles = new List<int>();
        int row = mapUtility.GetRow(start, mapSize);
        int col = mapUtility.GetColumn(start, mapSize);
        int nextTile = -1;
        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                nextTile = mapUtility.ReturnTileNumberFromRowCol(row, col, mapSize);
                currentTiles.Add(nextTile);
                col++;
            }
            col -= gridSize;
            row++;
        }
        return currentTiles;
    }
}

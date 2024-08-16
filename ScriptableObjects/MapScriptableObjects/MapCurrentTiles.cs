using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CurrentTiles", menuName = "ScriptableObjects/CurrentTiles", order = 1)]
public class MapCurrentTiles : ScriptableObject
{
    public MapUtility mapUtility;

    public int CheckCol(int col)
    {
        if (col%2 == 0){return col;}
        if (col < 0){return col + 1;}
        return col - 1;
    }
    
    public List<int> GetCurrentTilesFromStart(int start, int mapSize, int gridSize)
    {
        int row = mapUtility.GetRow(start, mapSize);
        int col = mapUtility.GetColumn(start, mapSize);
        col = CheckCol(col);
        return CurrentTilesFromRowCol(row, col, mapSize, gridSize);
    }

    public List<int> GetCurrentTilesFromCenter(int center, int mapSize, int gridSize)
    {
        int row = mapUtility.GetRow(center, mapSize);
        int col = mapUtility.GetColumn(center, mapSize);
        row -= gridSize/2;
        col -= gridSize/2;
        col = CheckCol(col);
        return CurrentTilesFromRowCol(row, col, mapSize, gridSize);
    }

    protected List<int> CurrentTilesFromRowCol(int row, int col, int mapSize, int gridSize)
    {
        List<int> currentTiles = new List<int>();
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

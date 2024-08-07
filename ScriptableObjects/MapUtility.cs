using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapUtility", menuName = "ScriptableObjects/MapUtility", order = 1)]
public class MapUtility : ScriptableObject
{
    public int ReturnTileNumberFromRowCol(int row, int col, int size)
    {
        // Out of bounds.
        if (row < 0 || col < 0 || row >= size || col >= size){return -1;}
        return (row * size) + col;
    }

    public int ReturnRowFromTile(int tile, int size)
    {
        int row = 0;
        for (int i = 0; i < size; i++)
        {
            if (tile >= size)
            {
                row++;
                tile -= size;
            }
            else{break;}
        }
        return row;
    }

    public int ReturnColFromTile(int tile, int size)
    {
        return tile%size;
    }
}

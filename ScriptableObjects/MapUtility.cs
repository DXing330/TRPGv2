using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapUtility", menuName = "ScriptableObjects/MapUtility", order = 1)]
public class MapUtility : ScriptableObject
{
    public int DistanceBetweenTiles(int tileOne, int tileTwo, int size)
    {
        return (Mathf.Abs(GetHexQ(tileOne, size)-GetHexQ(tileTwo, size))+Mathf.Abs(GetHexR(tileOne, size)-GetHexR(tileTwo, size))+Mathf.Abs(GetHexS(tileOne, size)-GetHexS(tileTwo, size)))/2;
    }

    public int ReturnTileNumberFromRowCol(int row, int col, int size)
    {
        // Out of bounds.
        if (row < 0 || col < 0 || row >= size || col >= size){return -1;}
        return (row * size) + col;
    }

    public int GetRow(int tile, int size)
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

    public int GetColumn(int tile, int size)
    {
        return tile%size;
    }

    public int ReturnTileNumberFromQRS(int Q, int R, int S, int size)
    {
        return ReturnTileNumberFromRowCol(GetRowFromQRS(Q,R,S,size), GetColFromQRS(Q,R,S,size), size);
    }

    public int GetColFromQRS(int Q, int R, int S, int size)
    {
        return Q;
    }

    public int GetRowFromQRS(int Q, int R, int S, int size)
    {
        return R + (Q - Q%2)/2;
    }

    public int GetHexQ(int location, int size)
    {
        return GetColumn(location, size);
    }

    public int GetHexR(int location, int size)
    {
        return GetRow(location, size) - (GetColumn(location, size) - GetColumn(location, size)%2) / 2;
    }

    public int GetHexS(int location, int size)
    {
        return -GetHexQ(location, size)-GetHexR(location, size);
    }

    public int PointInDirection(int location, int direction, int size)
    {
        int hexQ = GetHexQ(location, size);
        int hexR = GetHexR(location, size);
        int hexS = GetHexS(location, size);
        switch (direction)
        {
            // Up.
            case 0:
                return ReturnTileNumberFromQRS(hexQ, hexR-1, hexS+1, size);
            // UpRight.
            case 1:
                return ReturnTileNumberFromQRS(hexQ+1, hexR-1, hexS, size);
            // DownRight.
            case 2:
                return ReturnTileNumberFromQRS(hexQ+1, hexR, hexS-1, size);
            // Down.
            case 3:
                return ReturnTileNumberFromQRS(hexQ, hexR+1, hexS-1, size);
            // DownLeft.
            case 4:
                return ReturnTileNumberFromQRS(hexQ-1, hexR+1, hexS, size);
            // UpLeft.
            case 5:
                return ReturnTileNumberFromQRS(hexQ-1, hexR, hexS+1, size);
        }
        return location;
    }

    public bool DirectionCheck(int location, int direction, int size)
    {
        return (PointInDirection(location, direction, size) >= 0);
    }
    
    public List<int> AdjacentTiles(int location, int size)
    {
        List<int> adjacent = new List<int>();
        int adjacentTile = -1;
        for (int i = 0; i < 6; i++)
        {
            adjacentTile = PointInDirection(location, i, size);
            if (adjacentTile < 0){continue;}
            adjacent.Add(adjacentTile);
        }
        return adjacent;
    }
}

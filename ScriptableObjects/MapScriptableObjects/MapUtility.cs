using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapUtility", menuName = "ScriptableObjects/Utility/MapUtility", order = 1)]
public class MapUtility : ScriptableObject
{
    public int DistanceBetweenTiles(int tileOne, int tileTwo, int size)
    {
        return (Mathf.Abs(GetHexQ(tileOne, size)-GetHexQ(tileTwo, size))+Mathf.Abs(GetHexR(tileOne, size)-GetHexR(tileTwo, size))+Mathf.Abs(GetHexS(tileOne, size)-GetHexS(tileTwo, size)))/2;
    }

    public int ReturnTileNumberFromRowCol(int row, int col, int size)
    {
        // Out of bounds.
        if (row < 0 || col < 0 || row >= size || col >= size)
        {
            return -1;
        }
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

    public List<int> GetTilesInLineDirection(int location, int direction, int range, int size)
    {
        List<int> tiles = new List<int>();
        int current = location;
        for (int i = 0; i < range; i++)
        {
            if (DirectionCheck(current, direction, size))
            {
                current = PointInDirection(current, direction, size);
                tiles.Add(current);
            }
        }
        return tiles;
    }

    public List<int> GetTilesInConeShape(int startTile, int range, int coneCenter, int size)
    {
        List<int> tiles = new List<int>();
        List<int> leftCone = new List<int>();
        List<int> rightCone = new List<int>();
        List<int> forwardCone = new List<int>();
        int mainDirection = DirectionBetweenLocations(coneCenter, startTile, size);
        int leftDirection = (mainDirection+5)%6;
        int rightDirection = (mainDirection+1)%6;
        forwardCone.AddRange(GetTilesInLineDirection(coneCenter, mainDirection, range, size));
        leftCone.AddRange(GetTilesInLineDirection(coneCenter, leftDirection, range, size));
        rightCone.AddRange(GetTilesInLineDirection(coneCenter, rightDirection, range, size));
        int listCount = leftCone.Count;
        for (int i = 0; i < listCount; i++)
        {
            leftCone.AddRange(GetTilesInLineDirection(leftCone[i], rightDirection, range, size));
        }
        listCount = rightCone.Count;
        for (int i = 0; i < listCount; i++)
        {
            rightCone.AddRange(GetTilesInLineDirection(rightCone[i], leftDirection, range, size));
        }
        listCount = forwardCone.Count;
        for (int i = 0; i < listCount; i++)
        {
            forwardCone.AddRange(GetTilesInLineDirection(forwardCone[i], (leftDirection+3)%6, (i+1), size));
            forwardCone.AddRange(GetTilesInLineDirection(forwardCone[i], (rightDirection+3)%6, (i+1), size));
        }
        tiles.AddRange(leftCone);
        tiles.AddRange(rightCone);
        tiles.AddRange(forwardCone);
        tiles = tiles.Distinct().ToList();
        return tiles;
    }
    
    public List<int> GetTilesInCircleShape(int startTile, int range, int size)
    {
        List<int> tiles = new List<int>();
        for (int i = 0; i < 6; i++)
        {
            int nextTile = PointInDirection(startTile, i, size);
            if (nextTile == startTile){continue;}
            tiles.AddRange(GetTilesInConeShape(nextTile, range, startTile, size));
        }
        tiles = tiles.Distinct().ToList();
        return tiles;
    }

    public List<int> GetTileInRingShape(int startTile, int range, int size)
    {
        List<int> tiles = new List<int>();
        tiles = GetTilesInCircleShape(startTile, range, size);
        tiles = tiles.Except(GetTilesInCircleShape(startTile, range-1,size)).ToList();
        tiles = tiles.Distinct().ToList();
        return tiles;
    }

    public bool DirectionCheck(int location, int direction, int size)
    {
        return (PointInDirection(location, direction, size) >= 0);
    }

    public int DirectionBetweenLocations(int start, int end, int size)
    {
        for (int i = 0; i < 6; i++)
        {
            if (PointInDirection(start, i, size) == end)
            {
                return i;
            }
        }
        int q1 = GetHexQ(start, size);
        int q2 = GetHexQ(end, size);
        int r1 = GetHexR(start, size);
        int r2 = GetHexR(end, size);
        int s1 = GetHexS(start, size);
        int s2 = GetHexS(end, size);
        if (q1 == q2)
        {
            if (r1 > r2 && s1 < s2){return 0;}
            else if (r1 < r2 && s1 > s2){return 3;}
        }
        // Needs more edge case testing.
        else if (q1 < q2)
        {
            if (r1 <= r2 && s1 > s2){return 2;}
            else{return 1;}
        }
        else if (q1 > q2)
        {
            if (r1 < r2 && s1 >= s2){return 4;}
            else{return 5;}
        }
        return -1;
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

    public int RandomPointLeft(int location, int size)
    {
        int leftUp = PointInDirection(location, 5, size);
        int leftDown = PointInDirection(location, 4, size);
        // No points.
        if (leftUp < 0 && leftDown < 0){return location;}
        if (leftUp >= 0 && leftDown < 0){return leftUp;}
        if (leftUp < 0 && leftDown >= 0){return leftDown;}
        int choice = Random.Range(0, 2);
        if (choice == 0){return leftUp;}
        return leftDown;
    }

    public int RandomPointRight(int location, int size)
    {
        int up = PointInDirection(location, 1, size);
        int down = PointInDirection(location, 2, size);
        // No points.
        if (up < 0 && down < 0){return location;}
        if (up >= 0 && down < 0){return up;}
        if (up < 0 && down >= 0){return down;}
        int choice = Random.Range(0, 2);
        if (choice == 0){return up;}
        return down;
    }

    public int RandomPointDown(int location, int size)
    {
        int choice = Random.Range(2, 5);
        int newPoint = PointInDirection(location, choice, size);
        if (newPoint >= 0){return newPoint;}
        return location;
    }

    public int RandomPointUp(int location, int size)
    {
        int choice = Random.Range(5, 8)%6;
        int newPoint = PointInDirection(location, choice, size);
        if (newPoint >= 0){return newPoint;}
        return location;
    }
    
    public int DetermineCenterTile(int size)
    {
        if (size%2 == 1)
        {
            return (size*size)/2;
        }
        return ReturnTileNumberFromRowCol(size/2, size/2, size);
    }
}

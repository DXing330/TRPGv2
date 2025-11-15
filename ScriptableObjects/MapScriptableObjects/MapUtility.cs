using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapUtility", menuName = "ScriptableObjects/Utility/MapUtility", order = 1)]
public class MapUtility : ScriptableObject
{
    public bool flatTop = true;

    public int DistanceBetweenTiles(int tileOne, int tileTwo, int size)
    {
        return (Mathf.Abs(GetHexQ(tileOne, size) - GetHexQ(tileTwo, size)) + Mathf.Abs(GetHexR(tileOne, size) - GetHexR(tileTwo, size)) + Mathf.Abs(GetHexS(tileOne, size) - GetHexS(tileTwo, size))) / 2;
    }

    public int HorizontalDistanceBetweenTiles(int tileOne, int tileTwo, int size)
    {
        return GetColumn(tileTwo, size) - GetColumn(tileOne, size);
    }

    public int VerticalDistanceBetweenTiles(int tileOne, int tileTwo, int size)
    {
        return GetRow(tileTwo, size) - GetRow(tileOne, size);
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

    public int RandomTileInColumn(int col, int size)
    {
        int row = Random.Range(0, size);
        return ReturnTileNumberFromRowCol(row, col, size);
    }

    public int RandomTileInRow(int row, int size)
    {
        int col = Random.Range(0, size);
        return ReturnTileNumberFromRowCol(row, col, size);
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
            else { break; }
        }
        return row;
    }

    public int GetColumn(int tile, int size)
    {
        return tile % size;
    }

    public int ReturnTileNumberFromQRS(int Q, int R, int S, int size)
    {
        return ReturnTileNumberFromRowCol(GetRowFromQRS(Q, R, S, size), GetColFromQRS(Q, R, S, size), size);
    }

    public int GetColFromQRS(int Q, int R, int S, int size)
    {
        if (flatTop)
        {
            return Q;
        }
        else
        {
            return Q + (R - R % 2) / 2;
        }
    }

    public int GetRowFromQRS(int Q, int R, int S, int size)
    {
        if (flatTop)
        {
            return R + (Q - Q % 2) / 2;
        }
        else
        {
            return R;
        }
    }

    public int GetHexQ(int location, int size)
    {
        if (flatTop)
        {
            return GetColumn(location, size);
        }
        else
        {
            return GetColumn(location, size) - ((GetRow(location, size) - GetRow(location, size) % 2) / 2);
        }
    }

    public int GetHexR(int location, int size)
    {
        if (flatTop)
        {
            return GetRow(location, size) - (GetColumn(location, size) - GetColumn(location, size) % 2) / 2;
        }
        else
        {
            return GetRow(location, size);
        }
    }

    public int GetHexS(int location, int size)
    {
        return -GetHexQ(location, size) - GetHexR(location, size);
    }

    public int PointInOppositeDirection(int location, int otherPoint, int size)
    {
        int direction = DirectionBetweenLocations(location, otherPoint, size);
        return PointInDirection(location, (direction + 3) % 6, size);
    }

    public int PointInDirection(int location, int direction, int size)
    {
        int hexQ = GetHexQ(location, size);
        int hexR = GetHexR(location, size);
        int hexS = GetHexS(location, size);
        if (flatTop)
        {
            switch (direction)
            {
                // Up.
                case 0:
                    return ReturnTileNumberFromQRS(hexQ, hexR - 1, hexS + 1, size);
                // UpRight.
                case 1:
                    return ReturnTileNumberFromQRS(hexQ + 1, hexR - 1, hexS, size);
                // DownRight.
                case 2:
                    return ReturnTileNumberFromQRS(hexQ + 1, hexR, hexS - 1, size);
                // Down.
                case 3:
                    return ReturnTileNumberFromQRS(hexQ, hexR + 1, hexS - 1, size);
                // DownLeft.
                case 4:
                    return ReturnTileNumberFromQRS(hexQ - 1, hexR + 1, hexS, size);
                // UpLeft.
                case 5:
                    return ReturnTileNumberFromQRS(hexQ - 1, hexR, hexS + 1, size);
            }
        }
        else
        {
            switch (direction)
            {
                // UpRight.
                case 0:
                    return ReturnTileNumberFromQRS(hexQ + 1, hexR - 1, hexS, size);
                // Right.
                case 1:
                    return ReturnTileNumberFromQRS(hexQ + 1, hexR, hexS - 1, size);
                // DownRight.
                case 2:
                    return ReturnTileNumberFromQRS(hexQ, hexR + 1, hexS - 1, size);
                // DownLeft.
                case 3:
                    return ReturnTileNumberFromQRS(hexQ - 1, hexR + 1, hexS, size);
                // Left.
                case 4:
                    return ReturnTileNumberFromQRS(hexQ - 1, hexR, hexS + 1, size);
                // UpLeft.
                case 5:
                    return ReturnTileNumberFromQRS(hexQ, hexR - 1, hexS + 1, size);
            }
        }
        return location;
    }

    public List<int> GetTilesInColumn(int location, int span, int size)
    {
        List<int> tiles = new List<int>();
        List<int> cols = new List<int>();
        int startingCol = GetColumn(location, size);
        cols.Add(startingCol);
        for (int i = 0; i < span; i++)
        {
            cols.Add(startingCol + i + 1);
            cols.Add(startingCol - i - 1);
        }
        for (int i = cols.Count - 1; i >= 0; i--)
        {
            if (cols[i] < 0 || cols[i] >= size)
            {
                cols.RemoveAt(i);
            }
        }
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < cols.Count; j++)
            {
                tiles.Add(ReturnTileNumberFromRowCol(i, cols[j], size));
            }
        }
        return tiles;
    }

    public List<int> GetTilesInRow(int location, int span, int size)
    {
        List<int> tiles = new List<int>();
        List<int> rows = new List<int>();
        int startingRow = GetRow(location, size);
        rows.Add(startingRow);
        for (int i = 0; i < span; i++)
        {
            rows.Add(startingRow + i + 1);
            rows.Add(startingRow - i - 1);
        }
        for (int i = rows.Count - 1; i >= 0; i--)
        {
            if (rows[i] < 0 || rows[i] >= size)
            {
                rows.RemoveAt(i);
            }
        }
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < rows.Count; j++)
            {
                tiles.Add(ReturnTileNumberFromRowCol(rows[j], i, size));
            }
        }
        return tiles;
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
        int leftDirection = (mainDirection + 5) % 6;
        int rightDirection = (mainDirection + 1) % 6;
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
            forwardCone.AddRange(GetTilesInLineDirection(forwardCone[i], (leftDirection + 3) % 6, (i + 1), size));
            forwardCone.AddRange(GetTilesInLineDirection(forwardCone[i], (rightDirection + 3) % 6, (i + 1), size));
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
            if (nextTile == startTile) { continue; }
            tiles.AddRange(GetTilesInConeShape(nextTile, range, startTile, size));
        }
        tiles.Add(startTile);
        tiles = tiles.Distinct().ToList();
        return tiles;
    }

    public List<int> GetTilesInRingShape(int startTile, int range, int size, int width = 1)
    {
        List<int> tiles = new List<int>();
        tiles = GetTilesInCircleShape(startTile, range, size);
        if (width >= range) { return tiles; }
        tiles = tiles.Except(GetTilesInCircleShape(startTile, range - width, size)).ToList();
        tiles = tiles.Distinct().ToList();
        return tiles;
    }

    public List<int> GetTilesInLineShape(int startTile, int range, int size)
    {
        List<int> tiles = new List<int>();
        int start = startTile;
        for (int i = 0; i < 6; i++)
        {
            tiles.AddRange(GetTilesInLineDirection(start, i, range, size));
        }
        return tiles;
    }

    public List<int> GetTilesInBeamShape(int startTile, int direction, int span, int size)
    {
        List<int> tiles = new List<int>();
        tiles.AddRange(GetTilesInLineDirection(startTile, direction, size, size));
        List<int> startingTiles = new List<int>();
        if (span > 0)
        {
            startingTiles.AddRange(GetTilesInLineDirection(startTile, (direction + 1) % 6, span, size));
            startingTiles.AddRange(GetTilesInLineDirection(startTile, (direction + 5) % 6, span, size));
        }
        for (int i = 0; i < startingTiles.Count; i++)
        {
            tiles.AddRange(GetTilesInLineDirection(startingTiles[i], direction, size - 1, size));
        }
        tiles.AddRange(startingTiles);
        return tiles;
    }

    public bool DirectionCheck(int location, int direction, int size)
    {
        return (PointInDirection(location, direction, size) >= 0);
    }

    public int DirectionBetweenLocations(int start, int end, int size)
    {
        if (start < 0 || end < 0)
        {
            return -1;
        }
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
        if (flatTop)
        {
            int row1 = GetRow(start, size);
            int row2 = GetRow(end, size);
            if (q1 == q2)
            {
                if (row1 > row2) { return 3; }
                else { return 0; }
            }
            // Needs more edge case testing.
            else if (q1 < q2)
            {
                if (r1 <= r2 && s1 > s2) { return 2; }
                else { return 1; }
            }
            else if (q1 > q2)
            {
                if (r1 < r2 && s1 >= s2) { return 4; }
                else { return 5; }
            }
        }
        else
        {
            int col1 = GetColumn(start, size);
            int col2 = GetColumn(end, size);
            if (r1 == r2) // P1 is same row as P2
            {
                if (q1 < q2){return 1;}
                else{return 4;}
            }
            else if (r1 < r2) // P1 is above P2
            {
                if (col1 < col2){return 2;}
                else{return 3;}
            }
            else if (r1 > r2) // P1 is below P2
            {
                if (col1 < col2){return 0;}
                else{return 5;}
            }
        }
        return -1;
    }

    public string IntDirectionToString(int direction)
    {
        if (flatTop)
        {
            switch (direction)
            {
                case 0:
                    return "North";
                case 1:
                    return "North-East";
                case 2:
                    return "South-East";
                case 3:
                    return "South";
                case 4:
                    return "South-West";
                case 5:
                    return "North-West";
            }
        }
        else
        {
            switch (direction)
            {
                case 0:
                    return "North-East";
                case 1:
                    return "East";
                case 2:
                    return "South-East";
                case 3:
                    return "South-West";
                case 4:
                    return "West";
                case 5:
                    return "North-West";
            }
        }
        return "";
    }

    public List<int> AdjacentTiles(int location, int size)
    {
        List<int> adjacent = new List<int>();
        int adjacentTile = -1;
        for (int i = 0; i < 6; i++)
        {
            adjacentTile = PointInDirection(location, i, size);
            if (adjacentTile < 0) { continue; }
            adjacent.Add(adjacentTile);
        }
        return adjacent;
    }

    public bool TilesAdjacent(int location, int location2, int size)
    {
        return AdjacentTiles(location, size).Contains(location2);
    }

    public int RandomPointLeft(int location, int size)
    {
        int up = PointInDirection(location, 5, size);
        int down = PointInDirection(location, 4, size);
        // No points.
        if (up < 0 && down < 0) { return location; }
        if (up >= 0 && down < 0) { return up; }
        if (up < 0 && down >= 0) { return down; }
        int choice = Random.Range(0, 2);
        if (choice == 0) { return up; }
        return down;
    }

    public int RandomPointRight(int location, int size)
    {
        int up = PointInDirection(location, 1, size);
        int down = PointInDirection(location, 2, size);
        // No points.
        if (up < 0 && down < 0) { return location; }
        if (up >= 0 && down < 0) { return up; }
        if (up < 0 && down >= 0) { return down; }
        int choice = Random.Range(0, 2);
        if (choice == 0) { return up; }
        return down;
    }

    public int RandomPointDown(int location, int size)
    {
        int choice = Random.Range(2, 5);
        int newPoint = PointInDirection(location, choice, size);
        if (newPoint >= 0) { return newPoint; }
        return location;
    }

    public int RandomPointUp(int location, int size)
    {
        int choice = Random.Range(5, 8) % 6;
        int newPoint = PointInDirection(location, choice, size);
        if (newPoint >= 0) { return newPoint; }
        return location;
    }

    public int DetermineCenterTile(int size)
    {
        if (flatTop)
        {
            if (size % 2 == 1)
            {
                return (size * size) / 2;
            }
            return ReturnTileNumberFromRowCol(size / 2, size / 2, size);
        }
        return ReturnTileNumberFromRowCol(size / 2, size / 2, size);
    }

    public List<int> GetTilesByShapeSpan(int selected, string shape, int span, int size, int start = -1)
    {
        List<int> tiles = new List<int>();
        int direction = DirectionBetweenLocations(start, selected, size);
        switch (shape)
        {
            case "Circle":
                return GetTilesInCircleShape(selected, span, size);
            case "ECircle":
                tiles = GetTilesInCircleShape(selected, span, size);
                tiles.Remove(selected);
                return tiles;
            case "Line":
                return GetTilesInLineShape(selected, span, size);
            case "ELine":
                if (DistanceBetweenTiles(start, selected, size) <= 1)
                {
                    // Then go in the direction from the starting tile.
                    return GetTilesInLineDirection(start, direction, span, size);
                }
                else
                {
                    int eLineLocation = PointInDirection(selected, (direction + 3) % 6, size);
                    return GetTilesInLineDirection(eLineLocation, direction, span, size);
                }
            case "Cone":
                if (DistanceBetweenTiles(start, selected, size) <= 1)
                {
                    return GetTilesInConeShape(selected, span, start, size);
                }
                int coneLocation = PointInDirection(selected, (direction + 3) % 6, size);
                return GetTilesInConeShape(selected, span, coneLocation, size);
            case "Beam":
                return GetTilesInBeamShape(start, direction, span, size);
            case "Row":
                tiles.AddRange(GetTilesInRow(selected, span, size));
                tiles.Remove(selected);
                return tiles;
            case "Column":
                tiles.AddRange(GetTilesInColumn(selected, span, size));
                tiles.Remove(selected);
                return tiles;
            case "RowCol":
                tiles.AddRange(GetTilesInRow(selected, span, size));
                tiles.AddRange(GetTilesInColumn(selected, span, size));
                tiles = new List<int>(tiles.Distinct());
                tiles.Remove(selected);
                return tiles;
        }
        tiles.Add(selected);
        return tiles;
    }

    public int CountTilesByShapeSpan(string shape, int span)
    {
        if (span <= 0) { return 1; }
        switch (shape)
        {
            case "None":
                return 1;
            case "Circle":
                return CountTilesInCircleSpan(span);
            case "ECircle":
                return CountTilesInCircleSpan(span) - 1;
            case "Line":
                return CountTilesInLineSpan(span);
            case "ELine":
                return span;
            case "Cone":
                return CountTilesInConeSpan(span);
        }
        return 1;
    }

    protected int CountTilesInCircleSpan(int span)
    {
        return 1 + (3 * span * (span + 1));
    }

    protected int CountTilesInLineSpan(int span)
    {
        return 6 * span;
    }

    protected int CountTilesInConeSpan(int span)
    {
        return ((span + 1)*(span + 1))-1;
    }
}

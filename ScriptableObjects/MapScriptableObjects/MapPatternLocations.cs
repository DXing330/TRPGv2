using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapPatterns", menuName = "ScriptableObjects/MapPatterns", order = 1)]
public class MapPatternLocations : ScriptableObject
{
    public MapUtility mapUtility;

    protected int ReturnOscillatingFromMiddle(int number)
    {
        int sign = 1;
        if (number%2 == 0){sign = -1;}
        return (((number+1)/2)*(sign));
    }

    public List<int> ReturnTilesOfPattern(int pattern, int number, int mapSize)
    {
        List<int> patternTiles = new List<int>();
        for (int i = 0; i < number; i++)
        {
            patternTiles.Add(SingleSideSpawnPattern(pattern, i, mapSize));
        }
        return patternTiles;
    }

    protected int SingleSideSpawnPattern(int pattern, int index, int mapSize)
    {
        int row = -1;
        int column = -1;
        int adjustment = ReturnOscillatingFromMiddle(index);
        switch (pattern)
        {
            // Right.
            case 1:
                column = mapSize - 1;
                row = mapSize/2 + adjustment;
                if (row < 0 || row >= mapSize)
                {
                    for (int i = 0; i < mapSize; i++)
                    {
                        column--;
                        row = mapSize/2 + ReturnOscillatingFromMiddle(index - mapSize);
                        if (row >= 0 && row < mapSize){break;}
                    }
                }
                break;
            // Left.
            case 3:
                column = 0;
                row = mapSize/2 + adjustment;
                if (row < 0 || row >= mapSize)
                {
                    for (int i = 0; i < mapSize; i++)
                    {
                        column++;
                        row = mapSize/2 + ReturnOscillatingFromMiddle(index - mapSize);
                        if (row >= 0 && row < mapSize){break;}
                    }
                }
                break;
            // Up.
            case 0:
                row = 0;
                column = mapSize/2 + adjustment;
                if (column < 0 || column >= mapSize)
                {
                    for (int i = 0; i < mapSize; i++)
                    {
                        row++;
                        column = mapSize/2 + ReturnOscillatingFromMiddle(index - mapSize);
                        if (column >= 0 && column < mapSize){break;}
                    }
                }
                break;
            // Down.
            case 2:
                row = mapSize - 1;
                column = mapSize/2 + adjustment;
                if (column < 0 || column >= mapSize)
                {
                    for (int i = 0; i < mapSize; i++)
                    {
                        row--;
                        column = mapSize/2 + ReturnOscillatingFromMiddle(index - mapSize);
                        if (column >= 0 && column < mapSize){break;}
                    }
                }
                break;
        }
        return mapUtility.ReturnTileNumberFromRowCol(row, column, mapSize);
    }
}

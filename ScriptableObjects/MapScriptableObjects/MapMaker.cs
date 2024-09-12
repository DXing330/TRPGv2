using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapMaker", menuName = "ScriptableObjects/Utility/MapMaker", order = 1)]
public class MapMaker : ScriptableObject
{
    public MapUtility mapUtility;
    public SpriteContainer possibleSprites;
    public void SetPossibleSprites(SpriteContainer newSprites){possibleSprites = newSprites;}
    public List<string> possibleTiles;
    protected int defaultSize = 36;
    public int mapSize = 36;

    [ContextMenu("Make Map")]
    // Start by making the base layer.
    public List<string> MakeRandomMap(int newSize = -1)
    {
        UpdatePossibleTiles();
        if (newSize > 0){mapSize = newSize;}
        else {mapSize = defaultSize;}
        List<string> mapTiles = new List<string>();
        // Add a bunch of tiles.
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                mapTiles.Add(possibleTiles[Random.Range(0,possibleTiles.Count)]);
            }
        }
        return mapTiles;
    }

    protected void UpdatePossibleTiles()
    {
        possibleTiles.Clear();
        for (int i = 0; i < possibleSprites.sprites.Count; i++)
        {
            possibleTiles.Add(possibleSprites.sprites[i].name);
        }
    }

    public List<string> MakeBasicMap(int newSize = -1)
    {
        UpdatePossibleTiles();
        if (newSize > 0){mapSize = newSize;}
        else {mapSize = defaultSize;}
        List<string> mapTiles = new List<string>();
        // Add a bunch of tiles.
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                mapTiles.Add(possibleTiles[0]);
            }
        }
        return mapTiles;
    }

    public List<string> AddFeature(List<string> originalMap, string featureType, string pattern, string patternSpecifics = "")
    {
        switch (pattern)
        {
            case "River":
            return AddRiver(originalMap, featureType, patternSpecifics);
            case "Forest":
            return AddForest(originalMap, featureType, patternSpecifics);
            case "Wall":
            return AddWall(originalMap, featureType, patternSpecifics);
        }
        return originalMap;
    }

    protected List<string> AddForest(List<string> originalMap, string featureType, string specifics)
    {
        int startTile = Random.Range(1, mapSize-1) * Random.Range(1, mapSize-1);
        List<int> allTiles = mapUtility.AdjacentTiles(startTile, mapSize);
        allTiles.Add(startTile);
        for (int i = 0; i < allTiles.Count; i++)
        {
            originalMap[allTiles[i]] = featureType;
        }
        return originalMap;
    }

    // Rivers flow from left to right.
    protected List<string> AddRiver(List<string> originalMap, string featureType, string specifics)
    {
        // Pick a starting point.
        int currentPoint = Random.Range(1, mapSize-1) * mapSize;
        int newPoint = -1;
        for (int i = 0; i < mapSize; i++)
        {
            originalMap[currentPoint] = featureType;
            newPoint = mapUtility.RandomPointRight(currentPoint, mapSize);
            if (newPoint == currentPoint){break;}
            currentPoint = newPoint;
        }
        return originalMap;
    }

    // Walls can go straight from top to bottom or they can try to curve.
    protected List<string> AddWall(List<string> originalMap, string featureType, string specifics)
    {
        int currentPoint = Random.Range(1, mapSize - 1);
        int newPoint = -1;
        for (int i = 0; i < 2*mapSize; i++)
        {
            originalMap[currentPoint] = featureType;
            newPoint = mapUtility.RandomPointDown(currentPoint, mapSize);
            if (newPoint == currentPoint){break;}
            currentPoint = newPoint;
        }
        return originalMap;
    }
}

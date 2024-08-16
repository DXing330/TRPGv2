using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapMaker", menuName = "ScriptableObjects/MapMaker", order = 1)]
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
        if (pattern == "River"){return AddRiver(originalMap, featureType, patternSpecifics);}
        return originalMap;
    }

    protected List<string> AddForest(List<string> originalMap, string featureType, string specifics)
    {
        // Goes from top to bottom or from left to right.
        return originalMap;
    }

    protected List<string> AddRiver(List<string> originalMap, string featureType, string specifics)
    {
        int size = (int) Mathf.Sqrt(originalMap.Count);
        // Goes from top to bottom or from left to right.
        // Pick a starting point.
        int currentPoint = Random.Range(0, size) * size;
        int newPoint = -1;
        for (int i = 0; i < size + 1; i++)
        {
            originalMap[currentPoint] = featureType;
            newPoint = mapUtility.RandomPointRight(currentPoint, size);
            if (newPoint == currentPoint){break;}
            currentPoint = newPoint;
        }
        return originalMap;
    }
}

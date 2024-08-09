using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapMaker", menuName = "ScriptableObjects/MapMaker", order = 1)]
public class MapMaker : ScriptableObject
{
    public SpriteContainer possibleSprites;
    public void SetPossibleSprites(SpriteContainer newSprites){possibleSprites = newSprites;}
    public List<string> possibleTiles;
    // Should add up to 100.
    public List<int> tileProbabilities;
    protected int defaultSize = 36;
    public int mapSize = 36;

    [ContextMenu("Make Map")]
    // Start by making the base layer.
    public List<string> MakeMap(int newSize = -1)
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
}

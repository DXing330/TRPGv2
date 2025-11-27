using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPathfinderTester : MapManager
{
    public MapPathfinder pathfinder;
    protected override void Start()
    {
        pathfinder.SetMapSize(mapSize);
        ResetPath();
    }
    protected void ResetPath()
    {
        startPathTile = -1;
        endPathTile = -1;
        path.Clear();
        ResetHighlights();
    }
    public int startPathTile;
    public string startPathColor;
    public int endPathTile;
    public string endPathColor;
    public List<int> path;
    public string pathColor;
    public override void ClickOnTile(int tileNumber)
    {
        if (startPathTile < 0)
        {
            startPathTile = tileNumber;
            HighlightTiles();
            return;
        }
        if (endPathTile < 0)
        {
            endPathTile = tileNumber;
            // Make the path.
            path = pathfinder.BasicPathToTile(startPathTile, endPathTile);
            HighlightTiles();
            return;
        }
        else
        {
            ResetPath();
        }
    }
    public void ResetHighlights()
    {
        InitializeEmptyList();
        for (int i = 0; i < mapTiles.Count; i++)
        {
            mapTiles[i].HighlightLayer(3);
        }
    }
    public void HighlightTiles()
    {
        for (int i = 0; i < path.Count; i++)
        {
            mapTiles[path[i]].HighlightLayer(3, pathColor);
        }
        if (startPathTile >= 0)
        {
            mapTiles[startPathTile].HighlightLayer(3, startPathColor);
        }
        if (endPathTile >= 0)
        {
            mapTiles[endPathTile].HighlightLayer(3, endPathColor);
        }
    }
}

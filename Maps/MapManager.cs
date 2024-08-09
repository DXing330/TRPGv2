using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public MapMaker mapMaker;
    public MapDisplayer mapDisplayer;
    public List<MapTile> mapTiles;
    public List<string> mapInfo;

    [ContextMenu("Make Map")]
    public List<string> MakeMap()
    {
        return mapMaker.MakeMap();
    }

    [ContextMenu("Get New Map")]
    public void GetNewMap()
    {
        mapInfo = MakeMap();
        mapDisplayer.SetMapTiles(mapTiles);
        mapDisplayer.SetMapInfo(mapInfo);
        mapDisplayer.UpdateMap(0,0);
    }
}

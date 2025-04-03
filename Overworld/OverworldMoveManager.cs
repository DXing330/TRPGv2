using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldMoveManager : MonoBehaviour
{
    public List<string> tileTypes;
    public List<int> baseMoveCosts;

    public int ReturnMoveCost(string tileName)
    {
        int indexOf = tileTypes.IndexOf(tileName);
        if (indexOf == -1){return 1;}
        return baseMoveCosts[indexOf];
    }
}

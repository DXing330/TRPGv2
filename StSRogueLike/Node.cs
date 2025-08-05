using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public string delimiter = "|";
    public string delimiter2 = ",";
    public int nodeIndex;
    public int x_pos;
    public int y_pos;
    // 0 = battle, 1 = event, 2 = shop, 3 = rest, 4 = elite, 5 = treasure, 6 = boss
    public int nodeType;
    public List<int> incomingNodeIndexes;
    public List<int> outgoingNodeIndexes;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldWagon : MonoBehaviour
{
    public int weight;
    public int GetWeight(){return weight;}
    public int maxCarryWeight;
    public int GetCarryWeight(){return maxCarryWeight;}
    public string wheelsType;
    public string coverType;

    public void LoadStats(string newStats)
    {
        string[] data = newStats.Split("|");
        weight = int.Parse(data[0]);
        maxCarryWeight = int.Parse(data[1]);
        wheelsType = data[2];
        coverType = data[3];
    }

    public string ReturnStats()
    {
        return weight+"|"+maxCarryWeight+"|"+wheelsType+"|"+coverType;
    }
}

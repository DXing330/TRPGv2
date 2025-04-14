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
    public int maxDurability;
    public int GetMaxDurability(){return maxDurability;}
    public int currentDurability;
    public int GetDurability(){return currentDurability;}

    protected void ResetStats()
    {
        weight = 0;
        maxCarryWeight = 0;
        wheelsType = "";
        coverType = "";
        maxDurability = 0;
        currentDurability = 0;
    }

    public void LoadAllStats(string newStats)
    {
        if (newStats.Length < 6)
        {
            ResetStats();
            return;
        }
        string[] data = newStats.Split("|");
        weight = int.Parse(data[0]);
        maxCarryWeight = int.Parse(data[1]);
        wheelsType = data[2];
        coverType = data[3];
        maxDurability = int.Parse(data[4]);
        currentDurability = int.Parse(data[5]);
    }

    public string ReturnStats()
    {
        return weight+"|"+maxCarryWeight+"|"+wheelsType+"|"+coverType+"|"+maxDurability+"|"+currentDurability;
    }
}

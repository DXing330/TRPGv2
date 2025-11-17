using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FactionData", menuName = "ScriptableObjects/FactionObjects/FactionData", order = 1)]
public class FactionData : SavedData
{
    // Every faction will keep track of the same map data which is used for calculations.
    //public FactionMap map;
    public string factionName;
    public List<string> allFactionsColors;
    public string factionColor; // Each faction tile will high the map based on it's color.
    public string factionCapital;
    public int capitalHealth;
    public int capitalLocation; // If it falls then the faction is destroyed.
    public void CountAllResources()
    {
        // Count the resources gained from all owned tiles.
    }
    public int morale; // Unhappy means worse at fighting, happy means better at fighting. Calculated each upkeep based on population, unit count and tile outputs.
    public int treasury; // Factions should have gold far beyond you, their gold is measured in 1000s. Updated each upkeep.
    public int food;
    public int foodPerCity;
    public int foodPerUnit;
    public int ReturnFoodUpkeep()
    {
        return (foodPerCity * (cityLocations.Count + 1)) + (foodPerUnit * ownedUnits.Count);
    }
    public int materials;
    public int materialsPerCity;
    // If you fail to pay upkeep, then lose gold, if fail to pay gold then lose a city.
    public int ReturnMaterialUpkeep()
    {
        return materialsPerCity * (cityLocations.Count + 1);
    }
    public string factionLeader; // The Waifu
    //public List<string> factionMembers; // Not needed, just a leader is enough for Civ V, it's enough for us.
    public int reputation; // High enough = allies, negative enough = enemies.
    public List<int> cityLocations; // Build cities in order to expand.
    // Gain all unowned tiles adjacent to your city when you build a new city.
    // During upkeep obtain resources adjacent to all cities.
    public List<int> ownedTiles; // Determines food/resources/money.
    public void GainTile(int tile)
    {
        ownedTiles.Add(tile);
    }
    public void LoseTile(int tile)
    {
        int indexOf = ownedTiles.IndexOf(tile);
        if (indexOf >= 0)
        {
            ownedTiles.RemoveAt(indexOf);
        }
    }
    // Standing armies, might not be visible to you on the map.
    public List<string> ownedUnits; // Units die as they perform actions, every upkeep period spawn more units depending on various factors.
    public List<int> unitLocations; // Units spawn at cities but can move around.
    public List<string> factionBattleModifiers; // Tech stuff, they can increase their battle modifiers as time progresses, eventually they will be very strong.
    public List<string> otherFactions;
    public List<int> otherFactionRelations; // Politics handled by a single tracker. Main idea is tit for tat, but dislike whatever the Civ V AI dislikes as well.
    public List<string> requests; // Might be stored somewhere else later. Each faction has jobs that you can take, completing jobs increases reputation, but it might hurt reputation with other factions.
    public List<int> requestDeadlines;
}
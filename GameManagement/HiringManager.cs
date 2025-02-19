using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiringManager : MonoBehaviour
{
    void Start()
    {
        GenerateHirelings();
    }
    public StatDatabase firstNames;
    public StatDatabase lastNames;
    public string GenerateRandomName()
    {
        return firstNames.ReturnRandomValue()+" "+lastNames.ReturnRandomValue();
    }
    public StatDatabase actorData;
    public TacticActor dummyActor;
    public PartyDataManager partyData;
    // Classes
    public List<string> hireableActors;
    public List<string> basePrices;
    public List<string> possibleNames;
    // Only so many hireables appear each day/week/etc?
    public SelectStatTextList hirelingList;
    public StatTextList hirelingStats;
    // As you perform you can hire more/better hirelings.
    public int minHirelings = 3;

    public List<string> currentHirelingClasses;
    public List<string> currentHirelingNames;

    protected void GenerateHirelings()
    {
        currentHirelingClasses.Clear();
        currentHirelingNames.Clear();
        for (int i = 0; i < minHirelings; i++)
        {
            currentHirelingClasses.Add(hireableActors[Random.Range(0, hireableActors.Count)]);
            currentHirelingNames.Add(GenerateRandomName());
        }
        hirelingList.SetStatsAndData(currentHirelingClasses, currentHirelingNames);
    }

    public void ViewStats()
    {
        int index = hirelingList.GetSelected();
        if (index == -1){return;}
        string className = currentHirelingClasses[index];
    }
}

using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiringManager : MonoBehaviour
{
    public StatDatabase firstNames;
    public StatDatabase lastNames;
    public string GenerateRandomName()
    {
        return firstNames.ReturnRandomValue()+" "+lastNames.ReturnRandomValue();
    }
    public StatDatabase actorData;
    public PartyDataManager partyData;
    // Classes
    public List<string> hireableActors;
    public List<string> basePrices;
    public List<string> possibleNames;
    // Only so many hireables appear each day/week/etc?
    // As you perform you can hire more/better hirelings.
}

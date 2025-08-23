using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleSimulator : MonoBehaviour
{
    // Determine the characters.
    public StatDatabase actorStats;
    public List<string> partyOne;
    public List<string> partyTwo;
    public CharacterList partyOneList;
    public CharacterList partyTwoList;
    // Create the map.
    public BattleMap battleMap;
    // Spawn in the characters.
    public ActorMaker actorMaker;
    // Take turns in order until the battle is over.
    public BattleEndManager battleEndManager;
    public ActorAI actorAI;
    public AttackManager attackManager;
}

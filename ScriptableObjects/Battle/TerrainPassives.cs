using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TerrainPassives", menuName = "ScriptableObjects/BattleLogic/TerrainPassives", order = 1)]
public class TerrainPassives : ScriptableObject
{
    public string attacking;
    public string GetAttackingPassive(){return attacking;}
    public string defending;
    public string GetDefendingPassive(){return defending;}
    public string moving;
    public string GetMovingPassive(){return moving;}
    public string start;
    public string GetStartPassive(){return start;}
    public string end;
    public string GetEndPassive(){return end;}
}

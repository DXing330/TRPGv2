using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

[CreateAssetMenu(fileName = "TerrainPassives", menuName = "ScriptableObjects/BattleLogic/TerrainPassives", order = 1)]
public class TerrainPassives : StatDatabase
{
    public override void Initialize()
    {
        string[] blocks = allKeysAndValues.Split(keyValueDelimiter);
        attacking = blocks[0];
        defending = blocks[1];
        moving = blocks[2];
        start = blocks[3];
        end = blocks[4];
        #if UNITY_EDITOR
                EditorUtility.SetDirty(this);
        #endif
    }
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

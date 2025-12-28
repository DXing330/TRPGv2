using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveStatTester : MonoBehaviour
{
    public TacticActor dummyActor;
    public PassiveSkill passive;
    public StatDatabase allPassives;
    public TerrainPassivesList tilePassives;
    public TerrainPassivesList tEffectPassives;
    [ContextMenu("Debug TEffect Passives")]
    public void DebugTEffectPassives()
    {
        List<string> tEffects = tEffectPassives.keys;
        for (int i = 0; i < tEffects.Count; i++)
        {
            Debug.Log(tEffects[i]);
            Debug.Log("Attack:"+tEffectPassives.ReturnAttackingPassive(tEffects[i]));
            Debug.Log("Defend:"+tEffectPassives.ReturnDefendingPassive(tEffects[i]));
            Debug.Log("Move:"+tEffectPassives.ReturnMovingPassive(tEffects[i]));
            Debug.Log("Start:"+tEffectPassives.ReturnStartPassive(tEffects[i]));
            Debug.Log("End:"+tEffectPassives.ReturnEndPassive(tEffects[i]));
        }
    }
}

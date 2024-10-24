using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleUIManager : MonoBehaviour
{
    public BattleStats battleStats;
    public DisplayTurnOrder turnOrder;

    public void UpdateStatSheet(TacticActor actor)
    {

    }

    public void UpdateTurnOrder(BattleManager manager)
    {
        turnOrder.UpdateTurnOrder(manager.map.battlingActors, manager.GetTurnIndex());
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactionUnitManager : MonoBehaviour
{
    public FactionMap map;
    public FactionManager factionManager;
    public FactionUnit dummyUnit;
    public CombatUnit dummyCombatUnit;
    public void AllTurns(List<FactionData> factions)
    {
        for (int i = 0; i < factions.Count; i++)
        {
            FactionsTurn(factions[i]);
        }
    }

    protected void FactionsTurn(FactionData faction)
    {
        for (int i = 0; i < faction.unitData.unitData.Count; i++)
        {
            UnitTurn(faction.unitData, i);
        }
        for (int i = 0; i < faction.unitData.combatUnitData.Count; i++)
        {
            CombatUnitTurn(faction.unitData, i);
        }
    }

    protected void UnitTurn(FactionUnitDataManager faction, int index)
    {
        // Act based on your goals.
    }

    protected void CombatUnitTurn(FactionUnitDataManager faction, int index)
    {
        // Act based on goals.
    }
}

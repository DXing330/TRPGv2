using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public PassiveOrganizer passiveOrganizer;
    public PassiveSkill passive;
    public StatDatabase passiveData;
    // Condition is a bad name, since passives have conditions to activate.
    public Condition status;
    public StatDatabase statusData;
    public WeatherEffect weather;
    public StatDatabase weatherData;
    public BattleState battleState;


    public void StartBattle(TacticActor actor)
    {
        passive.ApplyStartBattlePassives(actor, passiveData, battleState);
    }

    public void StartTurn(TacticActor actor, BattleMap map)
    {
        weather.LoadWeather(weatherData.ReturnValue(map.GetWeather()));
        weather.ApplyEffects(actor, "Start");
        passive.ApplyPassives(actor, passiveData, "Start", map);
        // Status effects apply last so that passives have a chance to remove negative status effects.
        status.ApplyEffects(actor, statusData, "Start");
    }

    public void EndTurn(TacticActor actor, BattleMap map)
    {
        weather.LoadWeather(weatherData.ReturnValue(map.GetWeather()));
        weather.ApplyEffects(actor, "End");
        passive.ApplyPassives(actor, passiveData, "End", map);
        status.ApplyEffects(actor, statusData, "End");
        List<string> removedPassives = actor.DecreaseTempPassiveDurations();
        if (removedPassives.Count > 0)
        {
            for (int i = 0; i < removedPassives.Count; i++)
            {
                passiveOrganizer.RemoveSortedPassive(actor, removedPassives[i]);
            }
        }
    }
}

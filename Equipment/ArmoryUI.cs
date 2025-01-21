using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmoryUI : MonoBehaviour
{
    public SelectStatTextList actorStats;
    public SelectStatTextList actorPassives;
    public ActorSpriteHPList allActors;
    public TacticActor selectedActor;
    public string selectedPassive;
    public string selectedPassiveLevel;
    public PassiveDetailViewer detailViewer;

    public void UpdateSelectedActor()
    {
        selectedActor.SetStatsFromString(allActors.actorData[allActors.GetSelected()]);
        actorStats.UpdateActorStatTexts(selectedActor);
        actorPassives.UpdateActorPassiveTexts(selectedActor);
    }

    public void ViewPassiveDetails()
    {
        selectedPassive = actorPassives.stats[actorPassives.GetSelected()];
        selectedPassiveLevel = actorPassives.data[actorPassives.GetSelected()];
        detailViewer.UpdatePassiveNames(selectedPassive, selectedPassiveLevel);
    }
}

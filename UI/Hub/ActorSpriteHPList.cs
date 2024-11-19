using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorSpriteHPList : MonoBehaviour
{
    public GeneralUtility utility;
    int page = 0;
    public StatDatabase actorSpriteNames;
    public List<GameObject> objects;
    public List<ActorSpriteAndHP> actors;
    public TacticActor dummyActor;
    public CharacterList savedData;
    public List<string> actorNames;
    public List<string> actorData;

    void Start()
    {
        UpdateList();
    }

    public void UpdateList()
    {
        utility.DisableGameObjects(objects);
        actorNames = utility.GetCurrentPageStrings(page, objects, savedData.characters);
        actorData = utility.GetCurrentPageStrings(page, objects, savedData.stats);
        for (int i = 0; i < actorNames.Count; i++)
        {
            objects[i].SetActive(true);
            dummyActor.SetSpecies(actorNames[i]);
            dummyActor.SetSpriteName(actorSpriteNames.ReturnValue(actorNames[i]));
            dummyActor.SetStatsFromString(actorData[i]);
            actors[i].ShowActorSpriteAndHP(dummyActor);
        }
    }
}

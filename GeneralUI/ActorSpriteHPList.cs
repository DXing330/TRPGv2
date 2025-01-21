using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorSpriteHPList : MonoBehaviour
{
    public GeneralUtility utility;
    public int page = 0;
    public StatDatabase actorSpriteNames;
    public List<GameObject> objects;
    public List<ActorSprite> actors;
    public TacticActor dummyActor;
    public int selectedIndex;
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
            dummyActor.SetPersonalName(actorNames[i]);
            dummyActor.SetSpriteName(actorSpriteNames.ReturnValue(actorNames[i]));
            dummyActor.SetStatsFromString(actorData[i]);
            actors[i].ShowActorInfo(dummyActor);
        }
    }

    public void SelectActor(int index)
    {
        selectedIndex = index + (page * objects.Count);
    }

    public int GetSelected()
    {
        return selectedIndex;
    }
}

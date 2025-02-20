using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorSpriteHPList : MonoBehaviour
{
    public GeneralUtility utility;
    public int page = 0;
    public void ChangePage(bool right = true)
    {
        page = utility.ChangePage(page, right, objects, allActorNames);
        UpdateList();
    }
    public StatDatabase actorSpriteNames;
    public List<GameObject> objects;
    public List<ActorSprite> actors;
    public TacticActor dummyActor;
    public int selectedIndex = -1;
    public CharacterList savedData;
    public List<string> allActorNames;
    public List<string> allActorData;
    public List<string> actorNames;
    public List<string> actorData;

    void Start()
    {
        allActorNames = new List<string>(savedData.characters);
        allActorData = new List<string>(savedData.stats);
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
            dummyActor.SetSpriteName((actorNames[i]));
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorSpriteHPList : MonoBehaviour
{
    public GeneralUtility utility;
    public ColorDictionary colors;
    public int page = 0;
    protected void DisableChangePage(){utility.DisableGameObjects(changePageObjects);}
    protected void EnableChangePage(){utility.EnableGameObjects(changePageObjects);}
    public void ChangePage(bool right = true)
    {
        page = utility.ChangePage(page, right, objects, allActorNames);
        ResetSelected();
        UpdateList();
    }
    public void ResetSelected()
    {
        selectedIndex = -1;
        ResetHighlights();
    }
    protected void ResetHighlights()
    {
        for (int i = 0; i < actors.Count; i++)
        {
            actors[i].ChangeTextColor(colors.GetColor("Default"));
        }
    }
    protected void HighlightSelected()
    {
        ResetHighlights();
        if (GetSelected() < 0){return;}
        actors[GetSelected()%objects.Count].ChangeTextColor(colors.GetColor("Highlight"));
    }
    public List<GameObject> objects;
    public List<ActorSprite> actors;
    public int textSize;
    [ContextMenu("UpdateTextSize")]
    public void UpdateTextSize()
    {
        for (int i = 0; i < actors.Count; i++)
        {
            actors[i].SetTextSize(textSize);
        }
    }
    public List<GameObject> changePageObjects;
    public TacticActor dummyActor;
    public int selectedIndex = -1;
    public CharacterList savedData;
    public List<string> allActorNames;
    public List<string> allActorData;
    public List<string> actorNames;
    public List<string> actorData;

    void Start()
    {
        RefreshData();
    }

    public void RefreshData()
    {
        allActorNames = new List<string>(savedData.characters);
        allActorData = new List<string>(savedData.stats);
        ResetSelected();
        UpdateList();
    }

    public void UpdateList()
    {
        utility.DisableGameObjects(objects);
        DisableChangePage();
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
        if (allActorNames.Count > objects.Count)
        {
            EnableChangePage();
        }
    }

    public void SelectActor(int index)
    {
        selectedIndex = index + (page * objects.Count);
        HighlightSelected();
    }

    public int GetSelected()
    {
        return selectedIndex;
    }
}

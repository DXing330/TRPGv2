using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MedicinePanel : MonoBehaviour
{
    void Start()
    {
        UpdateItems();
    }
    // Only some items can be used while resting.
    public List<string> usableItems;
    List<string> currentUsableItems;
    public PartyDataManager partyData;
    public ActorSpriteHPList actorSelect;
    public TacticActor dummyActor;
    public TMP_Text actorHealthText;
    public SelectStatTextList statusList;
    public SelectList medicineSelect;
    public void ResetSelectedMedicine()
    {
        medicineSelect.ResetSelected();
        medicineEffectText.text = "";
        medicineQuantityText.text = "";
    }
    public SkillEffect medicineEffect;
    public StatDatabase activeData;
    public ActiveSkill dummyActive;
    public ActiveDescriptionViewer activeDescriptionViewer;
    public TMP_Text medicineEffectText;
    public TMP_Text medicineQuantityText;

    public void UpdateItems()
    {
        currentUsableItems = new List<string>();
        for (int i = 0; i < usableItems.Count; i++)
        {
            if (partyData.inventory.ItemExists(usableItems[i]))
            {
                currentUsableItems.Add(usableItems[i]);
            }
        }
        medicineSelect.SetSelectables(currentUsableItems);
        ResetSelectedMedicine();
    }

    public void SelectItem()
    {
        int index = medicineSelect.GetSelected();
        if (index < 0) { return; }
        dummyActive.LoadSkillFromString(activeData.ReturnValue(currentUsableItems[index]));
        medicineEffectText.text = activeDescriptionViewer.ReturnActiveDescriptionOnly(dummyActive);
        medicineQuantityText.text = partyData.inventory.ReturnQuantityOfItem(currentUsableItems[index]).ToString();
    }

    public void UseItem()
    {
        if (medicineSelect.GetSelected() < 0 || actorSelect.GetSelected() < 0)
        {
            return;
        }
        medicineEffect.AffectActor(dummyActor, dummyActive.GetEffect(), dummyActive.GetSpecifics(), dummyActive.GetPower());
        partyData.inventory.RemoveItemQuantity(1, currentUsableItems[medicineSelect.GetSelected()]);
        if (partyData.inventory.ReturnQuantityOfItem(currentUsableItems[medicineSelect.GetSelected()]) > 0)
        {
            medicineQuantityText.text = (int.Parse(medicineQuantityText.text) - 1).ToString();
        }
        else
        {
            UpdateItems();
        }
        // Update the party data.
        partyData.UpdatePartyMember(dummyActor, actorSelect.GetSelected());
        partyData.SetFullParty();
        int selectedIndex = actorSelect.GetSelected();
        int page = actorSelect.page;
        actorSelect.RefreshData();
        actorSelect.SetPage(page);
        actorSelect.SetSelectedIndex(selectedIndex);
        // Update the actor stats view.
        ViewActorStats();
    }

    public void ViewActorStats()
    {
        if (actorSelect.GetSelected() < 0) { return; }
        dummyActor.SetInitialStatsFromString(actorSelect.allActorData[actorSelect.GetSelected()]);
        actorHealthText.text = dummyActor.GetHealth() + " / " + dummyActor.GetBaseHealth();
        statusList.SetStatsAndData(dummyActor.GetUniqueStatuses(), dummyActor.GetUniqueStatusStacks());
    }
}

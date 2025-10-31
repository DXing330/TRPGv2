using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StorageUI : MonoBehaviour
{
    void Start()
    {
        storage.Load();
        UpdateDungeonStorage();
    }
    public GeneralUtility utility;
    public PartyDataManager partyData;
    public GuildStorage storage;
    public List<GameObject> panels;
    public void ActivatePanel(int index)
    {
        utility.DisableGameObjects(panels);
        panels[index].SetActive(true);
    }
    public SelectList dungeonBagSelect;
    public TMP_Text dungeonBagLimitText;
    public SelectList dungeonStorageSelect;
    public TMP_Text dungeonStorageLimitText;
    public void UpdateDungeonStorage(bool savePage = false)
    {
        int bagPage = 0;
        int storagePage = 0;
        if (savePage)
        {
            bagPage = dungeonBagSelect.GetPage();
            storagePage = dungeonStorageSelect.GetPage();
        }
        dungeonBagSelect.SetSelectables(partyData.dungeonBag.GetItems());
        dungeonBagLimitText.text = partyData.dungeonBag.ReturnBagLimitString();
        dungeonStorageSelect.SetSelectables(storage.GetStoredDungeonItems());
        dungeonStorageLimitText.text = storage.ReturnDungeonStorageLimitString();
        dungeonBagSelect.SetPage(bagPage);
        dungeonBagSelect.SetPage(storagePage);
    }
    public void StoreItem()
    {
        if (storage.MaxedDungeonStorage()){return;}
        int selected = dungeonBagSelect.GetSelected();
        if (selected < 0){return;}
        partyData.dungeonBag.UseItem(dungeonBagSelect.GetSelectedString());
        storage.StoreDungeonItem(dungeonBagSelect.GetSelectedString());
        UpdateDungeonStorage(true);
    }
    public void WithdrawItem()
    {
        if (partyData.dungeonBag.BagFull()){return;}
        int selected = dungeonStorageSelect.GetSelected();
        if (selected < 0){return;}
        partyData.dungeonBag.GainItem(dungeonStorageSelect.GetSelectedString());
        storage.WithdrawDungeonItem(dungeonStorageSelect.GetSelectedString());
        UpdateDungeonStorage(true);
    }
    public TMP_Text storedGold;
    public TMP_Text withdrawnGoldText;
    public int withdrawnGold;
    public TMP_Text bagGold;
    public TMP_Text depositedGoldText;
    public int depositedGold;
    public void UpdateGoldStorage()
    {
        storedGold.text = storage.GetStoredGold().ToString();
        bagGold.text = partyData.inventory.ReturnGold().ToString();
        withdrawnGold = 0;
        depositedGold = 0;
        withdrawnGoldText.text = withdrawnGold.ToString();
        depositedGoldText.text = depositedGold.ToString();
    }
    public void UpdateDWText()
    {
        withdrawnGoldText.text = withdrawnGold.ToString();
        depositedGoldText.text = depositedGold.ToString();
    }
    public void UpdateDeposit(int amount)
    {
        if (amount == -1)
        {
            depositedGold = partyData.inventory.ReturnGold();
        }
        else if (amount == 0)
        {
            depositedGold = 0;
        }
        else
        {
            int difference = partyData.inventory.ReturnGold() - depositedGold;
            if (difference >= amount)
            {
                depositedGold += amount;
            }
        }
        UpdateDWText();
    }
    public void UpdateWithdrawal(int amount)
    {
        if (amount == -1)
        {
            withdrawnGold = storage.GetStoredGold();
        }
        else if (amount == 0)
        {
            withdrawnGold = 0;
        }
        else
        {
            int difference = storage.GetStoredGold() - withdrawnGold;
            if (difference >= amount)
            {
                withdrawnGold += amount;
            }
        }
        UpdateDWText();
    }
    public void ConfirmDeposit()
    {
        partyData.inventory.RemoveItemQuantity(depositedGold);
        storage.StoreGold(depositedGold);
        UpdateGoldStorage();
    }
    public void ConfirmWithdrawl()
    {
        partyData.inventory.GainGold(withdrawnGold);
        storage.WithdrawGold(withdrawnGold);
        UpdateGoldStorage();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameManager : MonoBehaviour
{
    public PartyDataManager partyManager;

    public void StartGame()
    {
        partyManager.Load();
    }
}

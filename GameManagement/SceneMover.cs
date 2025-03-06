using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMover : MonoBehaviour
{
    public LoadingScreen loadingScreen;
    public SceneTracker sceneTracker;
    public bool loadingRequired = false;
    public string hubSceneName = "Hub";
    public string dungeonSceneName = "Dungeon";
    public PartyData permanentParty;
    public PartyData mainParty;

    public void LoadScene(string sceneName)
    {
        sceneTracker.SetPreviousScene(SceneManager.GetActiveScene().name);
        if (sceneName == hubSceneName)
        {
            ReturnToHub();
            return;
        }
        if (loadingRequired)
        {
            StartCoroutine(LoadingScreenMoveScene(sceneName));
        }
        else
        {
            StartCoroutine(LoadAsyncScene(sceneName));
        }
    }

    public void ReturnToHub()
    {
        sceneTracker.SetPreviousScene(SceneManager.GetActiveScene().name);
        permanentParty.ResetCurrentStats();
        mainParty.ResetCurrentStats();
        if (loadingRequired)
        {
            StartCoroutine(LoadingScreenMoveScene(hubSceneName));
        }
        else
        {
            StartCoroutine(LoadAsyncScene(hubSceneName));
        }
    }

    public void MoveToDungeon()
    {
        sceneTracker.SetPreviousScene(hubSceneName);
        if (loadingRequired)
        {
            StartCoroutine(LoadingScreenMoveScene(dungeonSceneName));
        }
        else
        {
            StartCoroutine(LoadAsyncScene(dungeonSceneName));
        }
    }

    public void MoveToBattle()
    {
        sceneTracker.SetPreviousScene(SceneManager.GetActiveScene().name);
        if (loadingRequired)
        {
            StartCoroutine(LoadingScreenMoveScene("BattleScene"));
        }
        else
        {
            StartCoroutine(LoadAsyncScene("BattleScene"));
        }
    }

    public void ReturnFromDungeon(bool clear = true)
    {
        if (clear)
        {
            if (loadingRequired)
            {
                StartCoroutine(LoadingScreenMoveScene("DungeonRewards"));
            }
            else
            {
                StartCoroutine(LoadAsyncScene("DungeonRewards"));
            }
        }
        else{ReturnToHub();}
    }

    public void ReturnFromBattle(int victory = 0)
    {
        // If you lose go somewhere else.
        if (victory != 0)
        {
            // Fail any quest in the dungeon.
            ReturnToHub();
            return;
        }
        //else
        if (sceneTracker.GetPreviousScene() == hubSceneName)
        {
            ReturnToHub();
            return;
        }
        if (loadingRequired)
        {
            StartCoroutine(LoadingScreenMoveScene(sceneTracker.GetPreviousScene()));
        }
        else
        {
            StartCoroutine(LoadAsyncScene(sceneTracker.GetPreviousScene()));
        }
    }

    IEnumerator LoadAsyncScene(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    IEnumerator LoadingScreenMoveScene(string sceneName)
    {
        for (int i = 0; i < 2; i++)
        {
            if (i == 0)
            {
                loadingScreen.StartLoadingScreen();
            }
            if (i == 1)
            {
                StartCoroutine(LoadAsyncScene(sceneName));
            }
            yield return new WaitForSeconds(loadingScreen.totalFadeTime);
        }
    }
}

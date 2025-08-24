using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMover : MonoBehaviour
{
    public string mainMenuSceneName = "Start";
    public void ReturnToMainMenu()
    {
        StartCoroutine(LoadAsyncScene(mainMenuSceneName));
    }
    public void DebugMoveToScene(string sceneName)
    {
        StartCoroutine(LoadAsyncScene(sceneName));
    }
    public LoadingScreen loadingScreen;
    public SceneTracker sceneTracker;
    public bool loadingRequired = false;
    public string overworldSceneName = "Overworld";
    public OverworldState overworldState;
    public string hubSceneName = "Hub";
    public string dungeonSceneName = "Dungeon";
    public DungeonState dungeonState;
    public string battleSceneName = "BattleScene";
    public BattleState battleState;
    public PartyData permanentParty;
    public PartyData mainParty;
    public PartyData tempParty;

    public void StartGame()
    {
        sceneTracker.Load();
        string currentScene = sceneTracker.GetCurrentScene();
        if (currentScene == battleSceneName)
        {
            battleState.Load();
            if (loadingRequired)
            {
                StartCoroutine(LoadingScreenMoveScene(currentScene));
            }
            else
            {
                StartCoroutine(LoadAsyncScene(currentScene));
            }
        }
        else if (currentScene == dungeonSceneName)
        {
            dungeonState.Load();
            if (loadingRequired)
            {
                StartCoroutine(LoadingScreenMoveScene(currentScene));
            }
            else
            {
                StartCoroutine(LoadAsyncScene(currentScene));
            }
        }
        else
        {
            LoadScene(sceneTracker.GetCurrentScene());
        }
    }

    public void LoadScene(string sceneName)
    {
        sceneTracker.SetPreviousScene(SceneManager.GetActiveScene().name);
        sceneTracker.SetCurrentScene(sceneName);
        sceneTracker.Save();
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
        sceneTracker.SetCurrentScene(hubSceneName);
        permanentParty.ResetCurrentStats();
        mainParty.ResetCurrentStats();
        tempParty.ResetCurrentStats();
        if (loadingRequired)
        {
            StartCoroutine(LoadingScreenMoveScene(hubSceneName));
        }
        else
        {
            StartCoroutine(LoadAsyncScene(hubSceneName));
        }
    }

    // Only called when first entering the dungeon from another scene.
    public void MoveToDungeon()
    {
        sceneTracker.SetPreviousScene(SceneManager.GetActiveScene().name);
        sceneTracker.SetCurrentScene(dungeonSceneName);
        sceneTracker.Save();
        // Only set when first moving to the dungeon.
        dungeonState.UpdatePreviousScene();
        dungeonState.Save();
        if (loadingRequired)
        {
            StartCoroutine(LoadingScreenMoveScene(dungeonSceneName));
        }
        else
        {
            StartCoroutine(LoadAsyncScene(dungeonSceneName));
        }
    }
    // Needs another one from the dungeon?
    public void MoveToBattle()
    {
        sceneTracker.SetPreviousScene(SceneManager.GetActiveScene().name);
        sceneTracker.SetCurrentScene(battleSceneName);
        sceneTracker.Save();
        battleState.UpdatePreviousScene();
        battleState.SetTerrainType();
        battleState.UpdateEnemyNames();
        battleState.Save();
        if (SceneManager.GetActiveScene().name == dungeonSceneName)
        {
            // Only save the dungeon state if entering a battle from the dungeon.
            dungeonState.Save();
        }
        if (loadingRequired)
        {
            StartCoroutine(LoadingScreenMoveScene(battleSceneName));
        }
        else
        {
            StartCoroutine(LoadAsyncScene(battleSceneName));
        }
    }

    // Only used in the dungeon rewards scene
    public void ReturnFromDungeonRewards()
    {
        sceneTracker.SetPreviousScene(dungeonState.GetPreviousScene());
        LoadScene(dungeonState.GetPreviousScene());
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
        else
        {
            sceneTracker.SetPreviousScene(dungeonState.GetPreviousScene());
            // Otherwise just go back to the previous scene.
            if (loadingRequired)
            {
                StartCoroutine(LoadingScreenMoveScene(sceneTracker.GetPreviousScene()));
            }
            else
            {
                StartCoroutine(LoadAsyncScene(sceneTracker.GetPreviousScene()));
            }
        }
    }

    public void ReturnFromBattle(int victory = 0)
    {
        // Fail any quest in the dungeon.
        if (victory != 0 && sceneTracker.GetPreviousScene() == dungeonSceneName)
        {
            mainParty.ClearAllStats();
            // If you die in the dungeon, basically game over, go back home.
            // Need to set the overworldstate location to the hub location.
            overworldState.SetLocation(overworldState.savedOverworld.GetCenterCityLocation());
            // Maybe lose some supplies/goods.
            ReturnToHub();
            return;
        }
        else if (sceneTracker.GetPreviousScene() == hubSceneName)
        {
            ReturnToHub();
            return;
        }
        else if (sceneTracker.GetPreviousScene() == dungeonSceneName)
        {
            dungeonState.Load();
        }
        sceneTracker.SetCurrentScene(sceneTracker.GetPreviousScene());
        sceneTracker.Save();
        // Otherwise just go back to the previous scene.
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

using System;
using UnityEngine;
using System.Collections;
using Data;
using UnityEngine.SceneManagement;
using Puzzles;
public class SceneController : MonoBehaviour
{
    public event Action BeforeSceneUnload;
    public event Action AfterSceneLoad;
    public CanvasGroup LevelSelect;
    public CanvasGroup faderCanvasGroup;
    public GameObject homeButton;
    public GameObject settingsButton;
    public float fadeDuration = 1f;

    public string startingSceneName = "SplashScene";
    public string mainMenuName = "MainMenuV3";

    //public SaveData playerSaveData;
    private bool isFading;
    private bool newGame = false;
    private bool inMenu = false;
    private int sceneIndex = 1;

    private IEnumerator Start()
    {
        // Start with a black screen.
        faderCanvasGroup.alpha = 1f;

        yield return StartCoroutine(LoadSceneAndSetActive(startingSceneName));
        inMenu = true;

        // Fade into the scene.
        StartCoroutine(Fade(0f));
        Logger.I.LogInitialized(this);

        UpdateSceneIndex();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void UpdateSceneIndex()
    {
        Progression.Load();
        sceneIndex = Progression.CurrentlyLoaded.Current;
        if (sceneIndex == 1)
            newGame = true;
    }

    public int SceneIndex
    {
        get
        {
            return sceneIndex;
        }
        set
        {
            if (sceneIndex == value) return;
            sceneIndex = value;
        }
    }

    public void ReturnToMenu()
    {
        UpdateSceneIndex();
        PuzzleMaster.onPuzzleComplete -= BeginAdvanceToNextLevel;
        FadeAndLoadScene(mainMenuName);
        inMenu = true;
    }

    public void ResetSaveData()
    {
        Progression.Delete();
        SceneIndex = 1;
        FadeAndLoadScene(mainMenuName);
        newGame = true;
        inMenu = true;
    }

    //public void 

    public void FadeAndLoadScene(string sceneName)
    {
        if (!isFading)
        {
            StartCoroutine(FadeAndSwitchScenes(sceneName));
        }
    }

    private IEnumerator FadeAndSwitchScenes(string sceneName)
    {
        yield return StartCoroutine(Fade(1f));
        BeforeSceneUnload?.Invoke();
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));
        if (sceneName == mainMenuName)
        {
            settingsButton.SetActive(true);
            homeButton.SetActive(true);
        }
        LevelSelect.gameObject.SetActive(false);
        AfterSceneLoad?.Invoke();
        yield return StartCoroutine(Fade(0f));
        
        if(PuzzleMaster.instance != null && !newGame && inMenu)
        {
            PuzzleMaster.onPuzzleComplete += OpenLevelSelect;
            inMenu = false;
        }
        else if(PuzzleMaster.instance != null)
        {
            PuzzleMaster.onPuzzleComplete += BeginAdvanceToNextLevel;
            newGame = false;
            inMenu = false;
        }
    }

    void OpenLevelSelect()
    {
        Debug.Log("not a new game, opening level select");
        LevelSelect.gameObject.SetActive(true);
        LevelSelect.GetComponent<SceneManagement.LevelSelect>().SetSceneIndex(this, sceneIndex);
        PuzzleMaster.onPuzzleComplete -= OpenLevelSelect;
    }

    void BeginAdvanceToNextLevel()
    {
        Debug.Log("starting scene " + sceneIndex);
        PuzzleMaster.onPuzzleComplete -= BeginAdvanceToNextLevel;
        if(Progression.CurrentlyLoaded.Current < sceneIndex)
        {
            Progression.CurrentlyLoaded.Current = sceneIndex;
            Progression.Save();
        }
        FadeAndLoadScene("Level" + sceneIndex);
        sceneIndex++;
    }



    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        
        // Gets reference to the most recently loaded scene.
        Scene newScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        if (newScene == null)
        {
            yield return SceneManager.LoadSceneAsync("GameOver");
        }

        // Sets the active scene so we can reference current active scene later.
        SceneManager.SetActiveScene(newScene);
    }

    private IEnumerator Fade(float alpha)
    {
        // Set is fading flag to true while the fader is fading.
        isFading = true;

        // Block raycasts.
        faderCanvasGroup.blocksRaycasts = true;

        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - alpha) / fadeDuration;

        // While the current alpha level isn't equal to the alpha provided, lerp to alpha. - My Solution
        // Don't do the above. 
        // Instead, while the faderCanvasGroup.alpha isn't mathf.approximately the given alpha, mathf.movetowards the alpha at a constant rate. - Their Solution
        while (!Mathf.Approximately(faderCanvasGroup.alpha, alpha))
        {
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha, alpha, fadeSpeed * Time.deltaTime);
            yield return null;
        }

        // Set the fading flag to false
        isFading = false;

        // Stop blocking raycasts.
        faderCanvasGroup.blocksRaycasts = false;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransitioner : MonoBehaviour
{
    public string nextSceneName;

    protected SceneController sceneController;

    protected virtual void Start()
    {
        sceneController = FindObjectOfType<SceneController>();
        if(sceneController == null)
            Debug.LogWarning("SceneController not present to load new scene. Make sure you're starting from SceneManager");
    }

    protected void LoadSceneAsync(string sceneName)
    {
        if (sceneController != null)
            sceneController.FadeAndLoadScene(sceneName);
        else
            Debug.LogWarning("SceneController not present to load new scene. Make sure you're starting from SceneManager");
    }
}

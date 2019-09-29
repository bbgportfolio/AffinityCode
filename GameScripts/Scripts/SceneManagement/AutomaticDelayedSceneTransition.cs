using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticDelayedSceneTransition : SceneTransitioner
{
    public float timeToWait;
    
    protected override void Start()
    {
        base.Start();
        StartCoroutine(WaitAndLoadScene());
    }

    private IEnumerator WaitAndLoadScene()
    {
        WaitForSeconds wait = new WaitForSeconds(timeToWait);
        yield return wait;
        LoadSceneAsync(nextSceneName);
    }
}

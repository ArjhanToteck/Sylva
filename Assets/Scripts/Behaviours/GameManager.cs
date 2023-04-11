using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Scene Settings")]
    public bool fadeIn = true;

    [Header("GameObject References")]
    public Animator sceneTransitionPanel;

    void Start()
    {
        if (fadeIn)
        {
            sceneTransitionPanel.gameObject.SetActive(true);
        }
    }

    public void SceneTransition(string sceneName)
    {
        sceneTransitionPanel.gameObject.SetActive(true);
        sceneTransitionPanel.SetTrigger("fadeOut");

        StartCoroutine(LoadSceneWhenReady());

        IEnumerator LoadSceneWhenReady()
        {
            while (!sceneTransitionPanel.GetCurrentAnimatorStateInfo(0).IsName("Done"))
            {
                yield return null;
            }

            SceneManager.LoadScene(sceneName);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Ephymeral.Events;

public class SceneManagerScript : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private SceneEvent sceneEvent;
    #endregion


    private void OnEnable()
    {
        sceneEvent.gameOverEvent.AddListener(loadScene);
    }

    private void OnDisable()
    {
        sceneEvent.gameOverEvent.RemoveListener(loadScene);
    }

    public void loadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void exitGame()
    {
        UnityEditor.EditorApplication.isPlaying = false;
        Application.Quit();
    }

    public void pauseUnPause()
    {
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }
    }
}

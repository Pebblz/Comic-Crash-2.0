using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
public class LevelManager : MonoBehaviour
{
    [SerializeField]
    GameObject LoadingScreen;

    [SerializeField]
    Slider LoadingBar;

    [SerializeField]
    Text ProgressText;
    //this loads the level based on the index it's given
    public void LoadLevel(int sceneIndex)
    {
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        if(sceneIndex == 0)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LeaveLobby();
        }
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }
    //this quits the game (Only works inside a build)
    public void QuitGame()
    {
        Application.Quit();
    }
    //this creates the loading screen
    IEnumerator LoadAsynchronously(int sceneIndex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneIndex);
        LoadingScreen.SetActive(true);
        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            LoadingBar.value = progress;
            ProgressText.text = progress * 100f + "%";
            yield return null;
        }
    }
}

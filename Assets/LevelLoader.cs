using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelLoader : MonoBehaviour
{
    public GameObject loadingPanel;
    public Slider progressBar;
    public void LoadLevel(string sceneName)
    {
        StartCoroutine(LoadAsynchrously(sceneName));
       
    }

    IEnumerator LoadAsynchrously(string sceneName)
    {
        loadingPanel.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            progressBar.value = progress;
            yield return null;
        }
    }
}

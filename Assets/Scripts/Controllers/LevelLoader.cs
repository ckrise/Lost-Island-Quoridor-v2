using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LevelLoader : MonoBehaviour
{
    public GameObject loadingPanel;
    public Slider progressBar;
    public Text progressText;
    public void LoadLevel(string sceneName)
    {
        StartCoroutine(LoadAsynchrously(sceneName));
    }

    IEnumerator LoadAsynchrously(string sceneName)
    {
        loadingPanel.SetActive(true);
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        int i = 0;
        float progress = 0;
        while(!operation.isDone && progress != 1)
        {            
            progress = Mathf.Clamp01(operation.progress / .90f);
            progressBar.value = progress;
            progressText.text = Mathf.FloorToInt(progress * 100f) + "%";
            Debug.Log($"{i++}: {progressText.text}");
            if (i % 10 == 0)
            {
                yield return null;
            }
        }
    }
}

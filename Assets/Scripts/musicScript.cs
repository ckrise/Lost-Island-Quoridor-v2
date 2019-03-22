using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicScript : MonoBehaviour
{
    public AudioSource mainGameMusic;
    public AudioSource defeatMusic;
    public AudioSource winMusic;
    public static musicScript musicScriptReference;
    // Start is called before the first frame update
    void Start()
    {
        mainGameMusic.Play();
        musicScriptReference = this;
    }

    public void playWin()
    {
        mainGameMusic.Stop();
        winMusic.Play();
    }

    public void playLose()
    {
        mainGameMusic.Stop();
        defeatMusic.Play();
    }
}

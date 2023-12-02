using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class CerrarVideo : MonoBehaviour
{
    public VideoPlayer video;

    private void Awake()
    {
        video = GetComponent<VideoPlayer>();
        Debug.Log("play alcanzado");
        video.Play();
        Debug.Log("CheckOver alcanzado");
        video.loopPointReached += CheckOver;
    }

    void CheckOver(VideoPlayer vp)
    {
        Debug.Log("CheckOver inicia");
        gameObject.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Debug.Log("CheckOver termina");
    }
}

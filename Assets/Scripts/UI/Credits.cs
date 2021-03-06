﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{

    private RectTransform rt;
    private Vector3 temp;

    [SerializeField]
    private float speed = 1f;

    [SerializeField]
    private float fastSpeed = 3f;

    [SerializeField]
    private FadeImage fadeImage;

    private bool fadingToScene;
    private SceneLoader sceneLoader;

    [SerializeField]
    private RectTransform endPoint;

    // Start is called before the first frame update
    void Start()
    {
        sceneLoader = GetComponent<SceneLoader>();
        rt = GetComponent<RectTransform>();

        AudioManager.instance?.Playmusic(Music.museum, .5f);

        StartCoroutine(fadeImage.FadeTo(1, 0, 1f));
    }

    // Update is called once per frame
    void Update()
    {
        if (endPoint.position.y > (float)Screen.height / 2f)
        {
            if (fadingToScene == false)
                StartCoroutine(FadeToScene());
        } else
        {
            temp = rt.position;
            temp.y += (Input.anyKey ? fastSpeed : speed) * (Screen.height / 400f);
            rt.position = temp;
        }

        Debug.Log("pos: " + (endPoint.position.y) + " | end: " + Screen.height / 3);
    }

    public IEnumerator FadeToScene(string sceneName = "MainMenu")
    {
        if (!fadingToScene)
        {
            fadingToScene = true;

            yield return new WaitForSeconds(.5f);
            yield return StartCoroutine(fadeImage.FadeTo(0, 1, 1f));
            if (sceneLoader == null)
            {
                sceneLoader = GetComponent<SceneLoader>();
            }
            sceneLoader.LoadNewScene(sceneName);
        }
    }
}

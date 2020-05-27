using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class PostProcessingHandeler : MonoBehaviour
{
    [SerializeField]
    private PostProcessVolume volume;

    private ChromaticAberration chrome;

    public void ChangeChromaticDistribution(float val = 0)
    {
        StopAllCoroutines();
        StartCoroutine(ChangingChromatic(val));
        //volume.profile.TryGetSettings(out chrome);
        //chrome.intensity.value = val;
    }

    public float Chromatic
    {
        get {
            volume.profile.TryGetSettings(out chrome);
            return chrome.intensity.value;
        }
        set
        {
            volume.profile.TryGetSettings(out chrome);
            chrome.intensity.value = value;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChangeChromaticDistribution(1);
        }
    }
    IEnumerator ChangingChromatic(float end)
    {
        float begin = Chromatic;
        float index = 0;
        while (index < 1) {
            index += Time.deltaTime;
            Chromatic = Mathf.Lerp(begin, end, index);
            yield return new WaitForFixedUpdate();
        }
        Chromatic = end;

    }
}

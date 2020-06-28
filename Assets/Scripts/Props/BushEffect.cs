using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushEffect : MonoBehaviour
{

    private GameObject bushTransform;

    private AudioSource audioS;
    private ParticleSystem ps;
    private float index = 0;
    private float scaleDelta = .1f;
    private float scaleSpeed = .2f;
    // Start is called before the first frame update
    void Start()
    {
        audioS = GetComponent<AudioSource>();
        ps = GetComponent<ParticleSystem>();
        audioS.volume = 0;
        ps.emissionRate = 0;

    }

    // Update is called once per frame
    public void UpdateBush(float precentage)
    {
        if (bushTransform == null) return;


        index += precentage * scaleSpeed;
        bushTransform.transform.localScale = new Vector3(1 - Mathf.Sin(index) * scaleDelta, 1 + Mathf.Sin(index) * scaleDelta, 1 - Mathf.Sin(index) * scaleDelta) ;

        audioS.volume = .5f * precentage * Settings.SFX;
        ps.emissionRate = precentage * 10f;
    }
    public void EnterBush(GameObject bush)
    {
        index = 0;
        bushTransform = bush;
    }
    public void LeaveBush(GameObject bush) 
    {
        bush.transform.localScale = Vector3.one;
        if (bushTransform == bush)
        { 
            bushTransform = null;
            audioS.volume = 0;
            ps.emissionRate = 0;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walkable : MonoBehaviour
{

    [Header("Walk info")]
    [SerializeField]
    private float walkDistance;

    [SerializeField]
    private float pitch = 1f;
    [SerializeField]
    private float volume = 1f;

    private float cDelta = 0;

    private Vector3 oldPos;

    [SerializeField]
    protected ParticleSystem walkParticle;

    [SerializeField]
    private SFXInstance walkSoundInfo;

    public virtual void Start()
    {
        oldPos = transform.position;
        walkSoundInfo.audioS = GetComponent<AudioSource>();
    }

    protected bool WalkCycle()
    {
        cDelta = (oldPos - transform.position).magnitude;
        if (cDelta > walkDistance)
        {
            WalkStep();
            cDelta = 0;
            oldPos = transform.position;
            return true;
        }
        return false;
    }

    protected virtual void WalkStep()
    {
        PlayParticle();
        PlayWalkSound();
    }

    private void PlayParticle()
    {
        if (walkParticle == null) return;
        walkParticle.Emit(1);
    }

    private void PlayWalkSound()
    {
        if (walkSoundInfo == null) return;

        walkSoundInfo.audioS.clip = walkSoundInfo.getClip;
        walkSoundInfo.audioS.volume = volume * Settings.SFX;
        walkSoundInfo.audioS.pitch = pitch;
        walkSoundInfo.audioS.Play();
}
}

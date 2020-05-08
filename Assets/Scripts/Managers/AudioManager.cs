using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum AudioEffect
{
}
public enum Music
{
    menu,
    level,
    guardSeesYou
}

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{

    [SerializeField]
    private List<SFXInstance> soundEffectInstances;

    [SerializeField]
    private List<MusicInstance> musicClips;

    private AudioSource musicSource;

    public static AudioManager instance;
    void Awake()
    {
        instance = this;
        musicSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioEffect audioEffect, float volume = -1, bool makeInstace = false)
    {
        SFXInstance selectedAudio = soundEffectInstances.Find(x => x.audioEffect == audioEffect);

        if (makeInstace)
        {
            selectedAudio = new SFXInstance();
            GameObject newObj = Instantiate(selectedAudio.audioS.gameObject);
            selectedAudio.audioS = newObj.GetComponent<AudioSource>();
        }
        selectedAudio.audioS.spatialBlend = 0;
        if (volume != -1)
        {
            selectedAudio.audioS.volume = volume * Settings.SFX;
        }
        selectedAudio.audioS.Play();
    }


    public void Play3DSound(AudioEffect audioEffect, float volume, Vector3 position, bool makeInstace = false, float pitch = 1)
    {
        SFXInstance selectedAudio = soundEffectInstances.Find(x => x.audioEffect == audioEffect);

        if (makeInstace)
        {
            selectedAudio = new SFXInstance();
            GameObject newObj = Instantiate(selectedAudio.audioS.gameObject);
            selectedAudio.audioS = newObj.GetComponent<AudioSource>();
        }
        selectedAudio.audioS.pitch = pitch;
        selectedAudio.audioS.spatialBlend = 1;
        selectedAudio.audioS.gameObject.transform.position = position;
        selectedAudio.audioS.volume = volume * Settings.SFX;
        selectedAudio.audioS.Play();
    }

    public void StopSound(AudioEffect audioEffect)
    {
        SFXInstance selectedAudio = soundEffectInstances.Find(x => x.audioEffect == audioEffect);
        selectedAudio.audioS.Stop();
    }

    public void Playmusic(Music music, float volume = -1)
    {
        MusicInstance selectedAudio = musicClips.Find(x => x.music == music);
        musicSource.clip = selectedAudio.clip;

        if (volume != -1)
        {
            musicSource.volume = volume * Settings.SFX;
        }
        musicSource.Play();

    }
    public void StopMusic()
    {
        musicSource.Stop();

    }
    public void CHangeMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }
}

[System.Serializable]
public class SFXInstance
{
    public AudioEffect audioEffect;
    public AudioSource audioS;
}
[System.Serializable]
public class MusicInstance
{
    public Music music;
    public AudioClip clip;
}

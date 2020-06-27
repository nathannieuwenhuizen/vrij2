using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public enum AudioEffect
{
    button_click,
    congrats,
    fountain,
    guard_catches_you,
    normal_gibberish,
    gnome_yell,
    popup_show,
    popup_hide,
    painting_steal,
    statue_steal,
    trenchCoat_wear,
    crown_steal,
    destruction
}
public enum Music
{
    museum,
    guardSeesYou,
    menu,
    park,
    king
}

[RequireComponent(typeof(AudioSource))]
public class AudioManager : MonoBehaviour
{

    [SerializeField]
    private List<SFXInstance> soundEffectInstances;

    [SerializeField]
    private List<MusicInstance> musicClips;

    [HideInInspector]
    public AudioSource musicSource;
    private bool musicIsFading = false;

    public static AudioManager instance;
    void Awake()
    {
        instance = this;
        musicSource = GetComponent<AudioSource>();
        musicSource.loop = true;

        InitialiseAudioSources();
    }

    public void InitialiseAudioSources()
    {
        foreach(SFXInstance sfx in soundEffectInstances)
        {
            if (sfx.clip.Length > 0)
            {
                GameObject temp = new GameObject(sfx.clip[0].name);
                temp.transform.parent = transform;
                sfx.audioS = temp.AddComponent<AudioSource>();
                sfx.audioS.playOnAwake = false;
                sfx.audioS.loop = false;
            }
        }
    }

    public void PlaySound(AudioEffect audioEffect, float volume)
    {
        SFXInstance selectedAudio = soundEffectInstances.Find(x => x.audioEffect == audioEffect);
        if (selectedAudio == null) return;

        selectedAudio.audioS.spatialBlend = 0;
        selectedAudio.audioS.clip = selectedAudio.getClip;
        selectedAudio.audioS.volume = volume * Settings.SFX;
        selectedAudio.audioS.Play();
    }

    public void Play3DSound(AudioEffect audioEffect, float volume, Vector3 position, float pitch = 1)
    {
        SFXInstance selectedAudio = soundEffectInstances.Find(x => x.audioEffect == audioEffect);
        if (selectedAudio == null) return;

        selectedAudio.audioS.pitch = pitch;
        selectedAudio.audioS.spatialBlend = 1;
        selectedAudio.audioS.gameObject.transform.position = position;
        selectedAudio.audioS.volume = volume * Settings.SFX;
        selectedAudio.audioS.Play();
    }

    public void StopSound(AudioEffect audioEffect)
    {
        SFXInstance selectedAudio = soundEffectInstances.Find(x => x.audioEffect == audioEffect);
        if (selectedAudio == null) return;
        selectedAudio.audioS.Stop();
    }

    public void Playmusic(Music music, float volume)
    {
        MusicInstance selectedAudio = musicClips.Find(x => x.music == music);

        musicSource.clip = selectedAudio.clip;
        musicSource.volume = volume * Settings.Music;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void ChangeMusicVolume(float volume)
    {
        musicSource.volume = volume * Settings.Music;
    } 
    public void FadeMusic(Music music, float duration)
    {
        StopAllCoroutines();
        ChangeMusicVolume(GameManager.instance.musicVolume);
        //if (musicIsFading) return;
        StartCoroutine(FadingMusic(music, duration));
    }
    private IEnumerator FadingMusic(Music music, float duration)
    {
        musicIsFading = true;
        float volume = musicSource.volume;
        yield return StartCoroutine(ChangeVolume(musicSource.volume, 0, duration / 2f));
        MusicInstance selectedMusic = musicClips.Find(x => x.music == music);

        if (selectedMusic != null)
        {
            musicSource.clip = selectedMusic.clip;
            musicSource.Play();
            yield return StartCoroutine(ChangeVolume(0, volume, duration / 2f));
        }
        musicIsFading = false;
    }


    private IEnumerator ChangeVolume(float begin, float end, float duration)
    {
        float index = 0;
        
        while (index < duration)
        {
            index += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(begin, end, index / duration);
            yield return new WaitForFixedUpdate();
        }
        musicSource.volume = end;
    }
}

[System.Serializable]
public class SFXInstance
{
    public AudioEffect audioEffect;
    [HideInInspector]
    public AudioSource audioS;
    public AudioClip[] clip;

    public AudioClip getClip
    {
        get
        {
            if (clip.Length <= 1)
            {
                return clip[0];
            }
            else
            {
                return clip[Random.Range(0, clip.Length)]; //return random sound
            }
        }
    }

}
[System.Serializable]
public class MusicInstance
{
    public Music music;
    public AudioClip clip;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Alarm : MonoBehaviour
{


    [SerializeField]
    private float alarmRange = 30f;

    private AudioSource audioS;
    private bool isOn = false;

    private Vector3 rotation = new Vector3(0, 0, 5f);

    private float volume = .2f;
    private float alarmDuration = 10f;

    void Start()
    {
        audioS = GetComponent<AudioSource>();
    }

    public void Turn(bool on)
    {
        if (isOn == on) return;
        isOn = on;

        if (on)
        {
            GameManager.instance.Alarm();
            AlertHumans();
            audioS.volume = volume * Settings.SFX;
            audioS.Play();
            StartCoroutine(AutoOff());
        } else
        {
            audioS.Stop();
        }
    }

    IEnumerator AutoOff()
    {
        yield return new WaitForSeconds(alarmDuration);
        Turn(false);
    }

    public void Update()
    {
        if (isOn)
        {
            transform.Rotate(rotation);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, alarmRange);
    }


    public void AlertHumans()
    {
        foreach(Human human in GameManager.instance.humans)
        {
            if (Vector3.Distance(human.transform.position, transform.position) < alarmRange)
            {
                //change their behaviour to search
                human.searchState.searchPos = transform.position - transform.forward * 2f;
                human.stateMachine.cState.OnStateSwitch(human.searchState);
            }
        }
    }
}

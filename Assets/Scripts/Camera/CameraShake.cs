using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{

    public static CameraShake instance;
    public void Awake()
    {
        instance = this;
    }

    public void Shake(float duration, float intensity = 1f, float frequencyInSeconds = 5f)
    {
        StopAllCoroutines();
        StartCoroutine(Shaking(duration, intensity, frequencyInSeconds));
    }

    private IEnumerator Shaking(float duration, float intensity, float frequencyInSeconds)
    {
        float index = 0;

        float cIntensity = 0f;
        float cValue = 0f;
        while (index < duration)
        {
            cIntensity = Mathf.Sin((index / duration) * Mathf.PI) * intensity;
            cValue = Mathf.Sin((index * (Mathf.PI * 2)) * frequencyInSeconds) * cIntensity;
            Angle = cValue;

            index += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }

        Angle = 0;
    }

    private float Angle
    {
        get {
            return transform.localRotation.eulerAngles.z;
        }
        set
        {
            Vector3 cRot = transform.localRotation.eulerAngles;
            cRot.z = value;

            transform.rotation = Quaternion.Euler(cRot);
        }
    }
}

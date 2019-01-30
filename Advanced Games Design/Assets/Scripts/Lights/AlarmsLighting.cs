using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlarmsLighting : MonoBehaviour {

    [SerializeField] float lightFadeSpeed = 1.5f;
    [SerializeField] float maxIntensity = 2.0f;
    [SerializeField] float minIntensity = 0.5f;
    [SerializeField] float lerpSpeed = 0.2f;

    public bool isAlarmOn;

    private float targetIntensity;

    private void Awake()
    {
        GetComponent<Light>().intensity = 0.0f;
        targetIntensity = maxIntensity;
    }

    private void Update()
    {
        if(isAlarmOn)
        {
            GetComponent<Light>().intensity = Mathf.Lerp(GetComponent<Light>().intensity, targetIntensity, lightFadeSpeed * Time.deltaTime);
            CheckTargetIntensity();
        }
        else
        {
            GetComponent<Light>().intensity = Mathf.Lerp(GetComponent<Light>().intensity, 0.0f, lightFadeSpeed * Time.deltaTime);
        }
    }

    void CheckTargetIntensity()
    {
        if(Mathf.Abs (targetIntensity - GetComponent<Light>().intensity) < lerpSpeed)
        {
            if(targetIntensity == maxIntensity)
            {
                targetIntensity = minIntensity;
            }
            else
            {
                targetIntensity = maxIntensity;
            }
        }
    }

}

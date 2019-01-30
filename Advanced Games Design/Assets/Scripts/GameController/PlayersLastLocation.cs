using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersLastLocation : MonoBehaviour
{
    public Vector3 playerOnePosition = new Vector3(1000.0f, 1000.0f, 1000.0f); //When the enemies see this position, they will ignore it and continue to patrol the area
    public Vector3 playerTwoPosition = new Vector3(1000.0f, 1000.0f, 1000.0f); 

    public Vector3 resetPosition = new Vector3(1000.0f, 1000.0f, 1000.0f); //Set the position back to the default position when needed
    [SerializeField] float lightMaxIntensity = 0.25f; //Fade the main directional light between max and min intensity's
    [SerializeField] float lightMinIntensity = 0.0f;
    [SerializeField] float lightFadeSpeed = 5.0f; //Fade speed between both intensity's
    [SerializeField] float musicFadeSpeed = 1.0f; //Fade speed for the main music (when alarm music plays, main music will quieten)

    private AlarmsLighting[] alarmsLighting;
    private Light mainLight;
    private AudioSource sightedAudio;
    private AudioSource[] sirens;

    private void Awake()
    {
        GameObject[] alarmLightingObjects = GameObject.FindGameObjectsWithTag(GameTags.alarmLight);
        alarmsLighting = new AlarmsLighting[alarmLightingObjects.Length];
        for (int i = 0; i < alarmsLighting.Length; i++)
        {
            alarmsLighting[i] = alarmLightingObjects[i].GetComponent<AlarmsLighting>();
        }

        //mainLight = GameObject.FindGameObjectWithTag(GameTags.mainLight).GetComponent<Light>();
        //sightedAudio = transform.Find("sightedAudio").GetComponent<AudioSource>();
    }

    private void Update()
    {
        SetAlarms();
    }

    void SetAlarms()
    {
        for(int i = 0; i < alarmsLighting.Length; i++)
        {
            if(playerOnePosition != resetPosition || playerTwoPosition != resetPosition && !alarmsLighting[i].isAlarmOn)
            {
                alarmsLighting[i].isAlarmOn = true;
            }
            else if (playerOnePosition == resetPosition && playerTwoPosition == resetPosition)
            {
                alarmsLighting[i].isAlarmOn = false;
            }
        }


       // alarmsLightingArray.isAlarmOn = (position != resetPosition); //If the position variable is not equal to the reset variable (i.e. the player has been sighted) turn the alarm on

        float newIntensity;

        if(playerOnePosition != resetPosition && playerTwoPosition != resetPosition)
        {
            newIntensity = lightMinIntensity;
        }
        else
        {
            newIntensity = lightMaxIntensity;
        }

        //mainLight.intensity = Mathf.Lerp(mainLight.intensity, newIntensity, lightFadeSpeed * Time.deltaTime);
    }
}

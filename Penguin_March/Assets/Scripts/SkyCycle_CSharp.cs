﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyCycle_CSharp : MonoBehaviour {

    public float secondsPerMinute = 0.625f; 

    //starting time in hours, use decimal points for minutes
    public float startTime = 12.0f; 

    //show date/time information?
    public bool showGUI = false;

    //this variable is for the position of the area in degrees from the equator, therfore it must stay between 0 and 90.
    //It determines now high the sun rises throughout the day, but not the length of the day yet.
    public float latitudeAngle = 45.0f;

    //The transform component of the empty that tilts the sun's roataion.(the SunTilt object, not the Sun object itself)
    public Transform sunTilt;

    
    public float hourTime;

    private float day ;
    private float min ;
    private float smoothMin ;

    private float texOffset ;
    private Material skyMat ;
    private Transform sunOrbit;

    public bool paused = false;

    // Use this for initialization
    void Start () {
        skyMat = GetComponent<Renderer>().sharedMaterial;
        sunOrbit = sunTilt.GetChild(0);

        Vector3 newRot = Vector3.zero;
        newRot.x = Mathf.Clamp(latitudeAngle, 0, 90); //set the sun tilt
        sunTilt.eulerAngles = newRot;

        if (secondsPerMinute == 0)
        {
            Debug.LogError("Error! Can't have a time of zero, changed to 0.01 instead.");
            secondsPerMinute = 0.01f;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (paused) { return; }
        UpdateSky();
    }

    void UpdateSky()
    {
        smoothMin = (Time.time / secondsPerMinute) + (startTime * 60);
        day = Mathf.Floor(smoothMin / 1440) + 1;

        smoothMin = smoothMin - (Mathf.Floor(smoothMin / 1440) * 1440); //clamp smoothMin between 0-1440
        min = Mathf.Round(smoothMin);

        hourTime = smoothMin / 60 ;

        Vector3 newRot = Vector3.zero;
        newRot.y = smoothMin / 4;
        sunOrbit.localEulerAngles = newRot;
        texOffset = Mathf.Cos((((smoothMin) / 1440) * 2) * Mathf.PI) * 0.25f + 0.25f;
        skyMat.mainTextureOffset = new Vector2(Mathf.Round((texOffset - (Mathf.Floor(texOffset / 360) * 360)) * 1000) / 1000, 0);
    }

    void OnGUI()
    {
        if (showGUI)
        {
            GUI.Label(new Rect(10, 0, 100, 20), "Day " + day.ToString());
            GUI.Label(new Rect(10, 20, 100, 40), digitalDisplay(Mathf.Floor(min / 60).ToString()) + ":" + digitalDisplay((min - Mathf.Floor(min / 60) * 60).ToString()));
        }
        //GUI.Label(Rect(10,40,100,60),texOffset.ToString()); //texture offset
    }

    string digitalDisplay(string num)
    { //converts a number into a digital display (adds a zero if it's a single figure)
        if (num.Length == 2)
        {
            return num;
        }
        else
        {
            return "0" + num;
        }
    }

}
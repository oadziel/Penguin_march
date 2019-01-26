using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [HideInInspector]public Resource_Containter resourceContainer;

    public int startPenguinNum = 25;
    [HideInInspector]
    public int currPenguinNum;

    GameObject HUD_GO;
    HUD_Manager HUD_M;

    string timeOfDay = "Morning";
    string[] timesOfDay = { "Morning", "MidDay", "Night" };
    int currTimeOfDayID = 0;

    int penguinCounter;
    string action;

    string[] actionNames = { "Fish", "Ice", "Penguins", "Coffee" };
    Action[] actions ;

    // Use this for initialization
    void Start ()
    {
        actions = new Action[actionNames.Length];
        for(int i = 0; i < actionNames.Length; i++)
        {
            actions[i] = new Action(actionNames[i]);
        }

        ResourceManagerReset();
        // Spawn Penguins.
        currPenguinNum = startPenguinNum;
        SetResourceValue("Penguins", startPenguinNum);

        GameObject penguinPrefab = (GameObject)Resources.Load("Penguin");
        for(int i = 0; i < startPenguinNum; i++)
        {
            Instantiate(penguinPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        }

        // HUD manager
        HUD_M = new HUD_Manager();
        HUD_GO = GameObject.Find("HUD");
        //HUD_GO = Instantiate(HUD_GO, Vector3.zero, Quaternion.identity);
        HUD_M.Start(HUD_GO);
        HUD_M.UpdateResources(resourceContainer);
    }

    public void DoAction(string action)
    {
        HUD_M.EnableNumChooser();
        UpdateAvaliablePenguins();
        this.action = action;
    }

    public void CountPenguins(int inc)
    {
        penguinCounter += inc;
        
        if (penguinCounter < 0) { penguinCounter = 0; }
        else if (currPenguinNum <= 0) { penguinCounter -= inc; }
        else { currPenguinNum -= inc; }
        Debug.Log(currPenguinNum);

        HUD_M.UpdateCounterHUD(penguinCounter);
        UpdateAvaliablePenguins();
    }

    public void ConfirmPenguinNum()
    {
        Debug.Log(action + ", penguins:" + penguinCounter);
        HUD_M.EnableBase();

        for(int i = 0; i < actions.Length; i++)
        {
            if(actions[i].ActionName == action)
            {
                actions[i].AddPenguins(penguinCounter);
                //currPenguinNum -= penguinCounter;
            }
        }
        penguinCounter = 0;
        HUD_M.UpdateCounterHUD(penguinCounter);
        UpdateAvaliablePenguins();
        HUD_M.UpdateResources(resourceContainer);
    }

    private void ResourceManagerReset()
    {
        resourceContainer.resource1 = actionNames[0];
        resourceContainer.resource2 = actionNames[1];
        resourceContainer.resource3 = actionNames[2];
        resourceContainer.resource4 = actionNames[3];

        resourceContainer.r1 = 0;
        resourceContainer.r2 = 0;
        resourceContainer.r3 = 0;
        resourceContainer.r4 = 0;
    }

    public void SetResourceValue(string resourceString, int changeValue)
    {
        switch (resourceString)
        {
            case "Fish":
                resourceContainer.r1 += changeValue;
                break;
            case "Ice":
                resourceContainer.r2 += changeValue;
                break;
            case "Penguins":
                resourceContainer.r3 += changeValue;
                break;
            case "Coffee":
                resourceContainer.r4 += changeValue;
                break;
        }
    }

    public void NextTimeOfDay()
    {
        currTimeOfDayID++;
        if (currTimeOfDayID >= timesOfDay.Length) { currTimeOfDayID = 0; }

        // Do Each action.
        for(int i = 0; i < actions.Length; i++)
        {
            SetResourceValue(actions[i].ActionName, actions[i].penguinsAssigned * 3);
        }

        HUD_M.UpdateResources(resourceContainer);

        currPenguinNum = resourceContainer.r3;

        timeOfDay = timesOfDay[currTimeOfDayID];
        HUD_M.UpdateTimeOfDay(timeOfDay);
    }

    public void UpdateAvaliablePenguins()
    {
        HUD_M.UpdateAvaliablePenguinUI(currPenguinNum);
    }
}
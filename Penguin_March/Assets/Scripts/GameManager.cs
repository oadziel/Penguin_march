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

    int penguinCounter;
    string action;

	// Use this for initialization
	void Start ()
    {
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
        this.action = action;
    }

    public void CountPenguins(int inc)
    {
        penguinCounter += inc;

        if (penguinCounter < 0) { penguinCounter = 0; }
        else if (penguinCounter > currPenguinNum) { penguinCounter = currPenguinNum; }
        HUD_M.UpdateCounterHUD(penguinCounter);
    }

    public void ConfirmPenguinNum()
    {
        Debug.Log(action + ", penguins:" + penguinCounter);
        HUD_M.EnableBase();

        SetResourceValue(action, 3 * penguinCounter);
        penguinCounter = 0;
        HUD_M.UpdateCounterHUD(penguinCounter);

        HUD_M.UpdateResources(resourceContainer);
    }

    private void ResourceManagerReset()
    {
        resourceContainer.resource1 = "Fish";
        resourceContainer.resource2 = "Ice";
        resourceContainer.resource3 = "Penguins";
        resourceContainer.resource4 = "Coffee";

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
}

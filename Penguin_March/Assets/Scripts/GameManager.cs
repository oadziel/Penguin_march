using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

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
        // Spawn Penguins.
        currPenguinNum = startPenguinNum;

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
    }
}

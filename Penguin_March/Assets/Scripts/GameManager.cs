using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    bool waitingForPenguins = false;
    string action;

    string[] actionNames = { "Fish", "Ice", "Penguins", "Coffee" };
    Action[] actions ;

    List<GameObject> penguin_GOs = new List<GameObject>();
    List<GameObject> fishingPenguins = new List<GameObject>();
    List<GameObject> icePenguins = new List<GameObject>();
    List<GameObject> penguinPenguins = new List<GameObject>();
    List<GameObject> coffeePenguins = new List<GameObject>();

    SkyCycle_CSharp skyCycle;

    int penguinsReturned = 0;

    float cameraTargetRandomiseCountdown;
    const float timeToWatchEachPenguin = 15.5f;

    // Locations for penguins to travel to
    Vector3 fishingSpot;
    Vector3 iceSpot;
    Vector3 penguinSpot;
    Vector3 coffeeSpot;

    CameraController cameraController;

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

        skyCycle = GameObject.FindObjectOfType<SkyCycle_CSharp>();
        skyCycle.paused = true;

        SpawnPenguin(startPenguinNum);

        cameraController = GameObject.FindObjectOfType<CameraController>();

        fishingSpot = GameObject.Find("FishingSpot").transform.position;
        iceSpot = GameObject.Find("IceSpot").transform.position;
        penguinSpot = GameObject.Find("PenguinSpot").transform.position;
        coffeeSpot = GameObject.Find("CoffeeSpot").transform.position;

        // HUD manager
        HUD_M = new HUD_Manager();
        HUD_GO = GameObject.Find("HUD");
        //HUD_GO = Instantiate(HUD_GO, Vector3.zero, Quaternion.identity);
        HUD_M.Start(HUD_GO);
        HUD_M.UpdateResources(resourceContainer);
    }

    public void Update()
    {
        if(!skyCycle.paused)
        {
            if(skyCycle.hourTime > 12 && waitingForPenguins == false && timeOfDay == timesOfDay[0])
            {
                SendPenguinsToBase();
                waitingForPenguins = true;
            }else if(skyCycle.hourTime > 20 && waitingForPenguins == false && timeOfDay == timesOfDay[1])
            {
                SendPenguinsToBase();
                waitingForPenguins = true;
            }else if(skyCycle.hourTime > 3 && skyCycle.hourTime < 4 && waitingForPenguins == false && timeOfDay == timesOfDay[2])
            {
                SendPenguinsToBase();
                waitingForPenguins = true;
            }

            if(waitingForPenguins)
            {
                if(penguinsReturned == resourceContainer.r3 - currPenguinNum)
                {
                    waitingForPenguins = false;

                    System.Random random = new System.Random((int)(Time.time * 1000));
                    if(actions != null)
                    {
                        for (int i = 0; i < actions.Length; i++)
                        {
                            SetResourceValue(actions[i].ActionName, GetResourceAmount(actions[i], random));
                            actions[i].Reset();
                        }
                    }
                    
                    // Update UI.
                    HUD_M.UpdateResources(resourceContainer);

                    AddNeccessaryPenguins();

                    // Reset penguin num.
                    currPenguinNum = resourceContainer.r3;

                    // update time of day UI.
                    HUD_M.EnableBase();
                    timeOfDay = timesOfDay[currTimeOfDayID];
                    HUD_M.UpdateTimeOfDay(timeOfDay);
                    skyCycle.paused = true;
                    penguinsReturned = 0;
                }
            }

            cameraTargetRandomiseCountdown -= Time.deltaTime;
            if(cameraTargetRandomiseCountdown < 0)
            {
                ChooseRandomPenguinTarget();
                cameraTargetRandomiseCountdown = timeToWatchEachPenguin;
            }
        }
    }

    void SpawnPenguin(int numOfPenguins)
    {
        System.Random rand = new System.Random();
        Vector3 spawnPos = GameObject.Find("SpawnPoint").transform.position;

        GameObject penguinPrefab = (GameObject)Resources.Load("Penguin");
        for(int i = 0; i < numOfPenguins; i++)
        {
            spawnPos.x += rand.Next(1, 10) - 5;
            spawnPos.z += rand.Next(1, 10) - 5;
            penguin_GOs.Add(Instantiate(penguinPrefab, spawnPos, Quaternion.identity));
        }
        currPenguinNum = resourceContainer.r3;

        spawnPos = GameObject.Find("SpawnPoint").transform.position;
        for(int i = 0; i < penguin_GOs.Count; i++)
        {
            penguin_GOs[i].GetComponent<NavMeshAgent>().destination = spawnPos;
        }
    }

    public void IncReturnedPenguins()
    {
        if (!waitingForPenguins) { return; }
        penguinsReturned++;
    }


    void AddNeccessaryPenguins()
    {
        int neededPenguins = resourceContainer.r3 - penguin_GOs.Count;

        SpawnPenguin(neededPenguins);
    }

    void SendPenguinsToBase()
    {
        Vector3 target = GameObject.Find("SpawnPoint").transform.position;
        for(int i = 0; i < penguin_GOs.Count; i++)
        {
            penguin_GOs[i].GetComponent<NavMeshAgent>().destination = target;
        }
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

        HUD_M.UpdateCounterHUD(penguinCounter);
        UpdateAvaliablePenguins();
    }

    public void ConfirmPenguinNum()
    {
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
        // Increase the time of day.
        currTimeOfDayID++;
        if (currTimeOfDayID >= timesOfDay.Length) { currTimeOfDayID = 0; }

        // Do Each action.
        int penguinsAssignedPlaces = 0;
        for(int i = 0; i < actions.Length; i++)
        {
            

            int penguinsAssigned = 0;
            // Get penguins to move to their respective place.
            for(int j = 0; j < actions[i].penguinsAssigned; j++)
            {
                Vector3 target = Vector3.zero;
                switch (actions[i].ActionName)
                {
                    case "Fish":
                        target = fishingSpot;
                        break;
                    case "Ice":
                        target = iceSpot;
                        break;
                    case "Penguins":
                        target = penguinSpot;
                        break;
                    case "Coffee":
                        target = coffeeSpot;
                        break;
                }

                int id = j + penguinsAssignedPlaces;
                penguin_GOs[j + penguinsAssignedPlaces].GetComponent<NavMeshAgent>().destination = target;

                penguinsAssigned++;
            }
            penguinsAssignedPlaces += penguinsAssigned;
        }

        ChooseRandomPenguinTarget();
        

        HUD_M.HideAll();

        skyCycle.paused = false;
    }

    void ChooseRandomPenguinTarget()
    {
        System.Random rand = new System.Random((int)Time.time);
        int randPenguin = rand.Next(0, penguin_GOs.Count);

        cameraController.target = penguin_GOs[randPenguin];
    }

    int GetResourceAmount(Action action, System.Random rand)
    {
        float resourceNum = 0;
        float randNum = rand.Next(0, 100);
        //randNum++;
        if (action.ActionName == "Fish" || action.ActionName == "Ice")
        {
            resourceNum = (120 / (action.penguinsAssigned * randNum)) * 100;
            if (resourceNum > 120) { resourceNum = 120; }
        }
        else if(action.ActionName == "Penguins")
        {
            resourceNum = (12 / (action.penguinsAssigned * randNum)) * 60;
            if (resourceNum > 12) { resourceNum = 12; }
        }
        else if(action.ActionName == "Coffee")
        {
            resourceNum = (70 / (action.penguinsAssigned * randNum)) * 100;
            if (resourceNum > 70) { resourceNum = 70; }
        }

        if (resourceNum < 0) { resourceNum = 0; }

        return (int)resourceNum;
    }

    public void UpdateAvaliablePenguins()
    {
        HUD_M.UpdateAvaliablePenguinUI(currPenguinNum);
    }
}
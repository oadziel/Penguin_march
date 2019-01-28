using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using FMODUnity;

public class GameManager : MonoBehaviour {
    [HideInInspector]public Resource_Containter resourceContainer;

    public int startPenguinNum = 25;
    public int seelNumber = 50;
    public int babyPenguinNum = 15;
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

    float colonyHappiness = 100;

    float cameraTargetRandomiseCountdown;
    const float timeToWatchEachPenguin = 15.5f;

    // Locations for penguins to travel to
    Vector3 fishingSpot;
    Vector3 iceSpot;
    Vector3 penguinSpot;
    Vector3 coffeeSpot;

    Vector3 seelSpawnPoint;

    CameraController cameraController;

    string actionFollowing;

    float decisionTimer = 10;
    int decisionID = 1;

    // Use this for initialization
    void Start ()
    {
        seelSpawnPoint = GameObject.Find("SeelSpawnPoint").transform.position;
        GameObject seelPrefab = (GameObject)Resources.Load("Seel");
        for(int i = 0; i < seelNumber; i++)
        {
            Instantiate(seelPrefab, seelSpawnPoint, Quaternion.identity);
        }
        Vector3 spawnPoint = GameObject.Find("SpawnPoint").transform.position;
        GameObject babyPenguinPrefab = (GameObject)Resources.Load("BabyPenguin");
        for(int i = 0; i < babyPenguinNum; i++)
        {
            Instantiate(babyPenguinPrefab, spawnPoint, Quaternion.identity);
        }

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
        HUD_M.UpdateHappinessBar(100, colonyHappiness);
    }

    public void Update()
    {
        if(!skyCycle.paused)
        {
            if(skyCycle.hourTime > 12 && waitingForPenguins == false && timeOfDay == timesOfDay[0])
            {
                SendPenguinsToBase();
                waitingForPenguins = true;
                FishPenguinAnimUpdate();
            }else if(skyCycle.hourTime > 20 && waitingForPenguins == false && timeOfDay == timesOfDay[1])
            {
                SendPenguinsToBase();
                waitingForPenguins = true;
                FishPenguinAnimUpdate();
            }
            else if(skyCycle.hourTime > 3 && skyCycle.hourTime < 4 && waitingForPenguins == false && timeOfDay == timesOfDay[2])
            {
                SendPenguinsToBase();
                waitingForPenguins = true;
                FishPenguinAnimUpdate();
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


                    System.Random rand = new System.Random((int)(Time.time * 1000));
                    int soundID = rand.Next(0, 4);
                    if (soundID < 1) { soundID = 1; } else if (soundID > 3) { soundID = 3; }
                    Debug.Log(soundID);
                    RuntimeManager.PlayOneShot("event:/Penguin Survives " + soundID, Camera.main.transform.position);

                }
            }

            cameraTargetRandomiseCountdown -= Time.deltaTime;
            if(cameraTargetRandomiseCountdown < 0)
            {
                ChooseRandomPenguinTarget();
                cameraTargetRandomiseCountdown = timeToWatchEachPenguin;
            }

            //Check for win
            if(skyCycle.day >= 5)
            {
                HUD_M.EnableWinState();
            }
        }
        else
        {
            decisionTimer += Time.deltaTime;
            // Play sound.
            
            if(decisionTimer > 18)
            {
                Debug.Log("playing sound");
                RuntimeManager.PlayOneShot("event:/Player is taking to long to decide " + decisionID, Camera.main.transform.position);
                decisionID++;
                if (decisionID > 2) { decisionID = 1; }
                decisionTimer = 0;
            }
        }
    }

    void FishPenguinAnimUpdate()
    {
        for(int i =0; i < fishingPenguins.Count; i++)
        {
            fishingPenguins[i].GetComponent<AnimControlScript>().ChangeToFishCarry();
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

        fishingPenguins.Clear();
        icePenguins.Clear();
        penguinPenguins.Clear();
        coffeePenguins.Clear();

        decisionTimer = 0;

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
                        fishingPenguins.Add(penguin_GOs[j + penguinsAssignedPlaces]);
                        break;
                    case "Ice":
                        target = iceSpot;
                        icePenguins.Add(penguin_GOs[j + penguinsAssignedPlaces]);
                        break;
                    case "Penguins":
                        target = penguinSpot;
                        penguinPenguins.Add(penguin_GOs[j + penguinsAssignedPlaces]);
                        break;
                    case "Coffee":
                        target = coffeeSpot;
                        coffeePenguins.Add(penguin_GOs[j + penguinsAssignedPlaces]);
                        break;
                }

                int id = j + penguinsAssignedPlaces;
                penguin_GOs[j + penguinsAssignedPlaces].GetComponent<NavMeshAgent>().destination = target;
                penguin_GOs[j + penguinsAssignedPlaces].GetComponent<AnimControlScript>().ChangeState(j + penguinsAssignedPlaces);

                penguinsAssigned++;
            }
            penguinsAssignedPlaces += penguinsAssigned;
        }

        ChooseRandomPenguinTarget();

        TakeResources();

        HUD_M.HideAll();

        skyCycle.paused = false;
    }

    void ChooseRandomPenguinTarget()
    {
        System.Random rand = new System.Random((int)Time.time);
        

        GameObject target = null;

            int randPenguin = rand.Next(0, 5) - 1;
        if (randPenguin < 0) { randPenguin = 0; }
            switch (randPenguin)
            {
                case 0:
                if(fishingPenguins.Count > 0)
                {
                    target = fishingPenguins[0];
                    actionFollowing = actionNames[0];
                } 
                    break;

                case 1:
                if(icePenguins.Count > 0)
                {
                    target = icePenguins[0];
                    actionFollowing = actionNames[1];
                }
                    break;

                case 2:
                if(penguinPenguins.Count > 0)
                {
                    target = penguinPenguins[0];
                    actionFollowing = actionNames[2];
                }
                
                break;

                case 3:
                if (coffeePenguins.Count > 0)
                {
                    target = coffeePenguins[0];
                    actionFollowing = actionNames[3];
                }
                
                    break;
            }


        if(target == null)
        {
            target = penguin_GOs[0];
        }




        cameraController.target = target;
    }

    void TakeResources()
    {
        resourceContainer.r1 -= (int)1.5f * resourceContainer.r3;
        if (resourceContainer.r1 < 0) { resourceContainer.r1 = 0; }
        
        // If it is the day time the ice will melt.
        if(timeOfDay == timesOfDay[0] || timeOfDay == timesOfDay[1])
        {
            resourceContainer.r2 -= 25;
            if (resourceContainer.r2 < 0) { resourceContainer.r2 = 0; }
        }

        // If morning each penguin drinks a morning coffee.
        if(timeOfDay == timesOfDay[0])
        {
            resourceContainer.r4 -= resourceContainer.r3;
            if (resourceContainer.r4 < 0) { resourceContainer.r4 = 0; }
        }

        float happinesToTake = 0;
        // No food or ice or coffee
        if(resourceContainer.r1 == 0 )
        {
            happinesToTake += resourceContainer.r3;
        }else
        {
            happinesToTake -= 10;
        }

        if(resourceContainer.r2 == 0)
        {
            happinesToTake += 10;
        }
        else
        {
            happinesToTake -= 5;
        }

        if(resourceContainer.r4 == 0)
        {
            happinesToTake += 2 * (resourceContainer.r3 * 0.75f);
        }
        else
        {
            happinesToTake -= 15;
        }

        if (happinesToTake > 40) { happinesToTake = 40; }

        colonyHappiness -= happinesToTake;

        if (colonyHappiness > 100) { colonyHappiness = 100; }

        HUD_M.UpdateHappinessBar(100, colonyHappiness);
        HUD_M.UpdateResources(resourceContainer);

        if(colonyHappiness <=0)
        {
            System.Random rand = new System.Random((int)(Time.time * 1000));
            int soundID = rand.Next(0, 4); Debug.Log(soundID);
            if (soundID < 1) { soundID = 1; }
            else if (soundID > 3) { soundID = 3; }
            HUD_M.Fail();
            RuntimeManager.PlayOneShot("event:/death " + soundID, Camera.main.transform.position);

        }

    }

    int GetResourceAmount(Action action, System.Random rand)
    {
        float resourceNum = 0;
        float randNum = rand.Next(0, 100);
        if (action.ActionName == "Fish" || action.ActionName == "Ice")
        {
            resourceNum = ( (action.penguinsAssigned * randNum)/ 120) * 60;
            if (resourceNum > 120) { resourceNum = 120; }
        }
        else if(action.ActionName == "Penguins")
        {
            resourceNum = ( (action.penguinsAssigned * randNum) / 12) * 30;
            if (resourceNum > 12) { resourceNum = 12; }
        }
        else if(action.ActionName == "Coffee")
        {
            resourceNum = ( (action.penguinsAssigned * randNum) / 70) * 60 ;
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
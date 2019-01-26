using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Manager {

    GameObject HUD;

    GameObject baseMenu;
    GameObject NumberChooser;
    GameObject resourcesTab;
    GameObject TimeOfDay;

    Text penguinAssignNumberText;
    Text penguinsAvaliable;

    Text resourceFish;
    Text resourceIce;
    Text resourcePenguins;
    Text resourceCoffee;

    RawImage timeOfDayImg;

    public void Start(GameObject HUD)
    {
        this.HUD = HUD;


        baseMenu = HUD.transform.Find("BaseMenu").gameObject;

        NumberChooser = HUD.transform.Find("NumberChooser").gameObject;

        penguinAssignNumberText = GameObject.Find("Counter").GetComponentInChildren<Text>();
        penguinsAvaliable = GameObject.Find("Avaliable").GetComponent<Text>();

        resourcesTab = HUD.transform.Find("Resources").gameObject;
        resourceFish = resourcesTab.transform.Find("Fish").GetComponentInChildren<Text>();
        resourceIce = resourcesTab.transform.Find("Ice").GetComponentInChildren<Text>();
        resourcePenguins = resourcesTab.transform.Find("Penguins").GetComponentInChildren<Text>();
        resourceCoffee = resourcesTab.transform.Find("Coffee").GetComponentInChildren<Text>();

        TimeOfDay = HUD.transform.Find("TimeOfDay").gameObject;
        timeOfDayImg = TimeOfDay.transform.Find("Panel").GetComponentInChildren<RawImage>();

        NumberChooser.SetActive(false);


    }

    public void EnableNumChooser()
    {
        HideAll();
        NumberChooser.SetActive(true);
    }

    public void UpdateAvaliablePenguinUI(int avaliableNumOfPenguins)
    {
        penguinsAvaliable.text = "Penguins avaliable: " + avaliableNumOfPenguins;
    }

    public void HideAll()
    {
        baseMenu.SetActive(false);
        NumberChooser.SetActive(false);
        TimeOfDay.SetActive(false);
    }

    public void UpdateCounterHUD(int numToDisplay)
    {
        penguinAssignNumberText.text = "" + numToDisplay;
    }

    public void EnableBase()
    {
        HideAll();
        baseMenu.SetActive(true);
        TimeOfDay.SetActive(true);
    }

    public void UpdateResources(Resource_Containter RC)
    {
        
        resourceFish.text = RC.r1.ToString();
        resourceIce.text = RC.r2.ToString();
        resourcePenguins.text = RC.r3.ToString();
        resourceCoffee.text = RC.r4.ToString();
    }
	

    public void UpdateTimeOfDay(string timeOfDay)
    {
        switch (timeOfDay)
        {
            case "Morning":
                timeOfDayImg.texture = (Texture)Resources.Load("RisingSun");
                break;

            case "MidDay":
                timeOfDayImg.texture = (Texture)Resources.Load("Sun");
                break;

            case "Night":
                timeOfDayImg.texture = (Texture)Resources.Load("Moon");
                break;
        }


    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Manager {

    GameObject HUD;

    GameObject baseMenu;
    GameObject NumberChooser;
    GameObject resourcesTab;

    Text penguinAssignNumberText;

    Text resourceFish;
    Text resourceIce;
    Text resourcePenguins;
    Text resourceCoffee;

    public void Start(GameObject HUD)
    {
        this.HUD = HUD;


        baseMenu = HUD.transform.Find("BaseMenu").gameObject;
        NumberChooser = HUD.transform.Find("NumberChooser").gameObject;
        penguinAssignNumberText = GameObject.Find("Counter").GetComponentInChildren<Text>();
        resourcesTab = HUD.transform.Find("Resources").gameObject;
        resourceFish = resourcesTab.transform.Find("Fish").GetComponentInChildren<Text>();
        resourceIce = resourcesTab.transform.Find("Ice").GetComponentInChildren<Text>();
        resourcePenguins = resourcesTab.transform.Find("Penguins").GetComponentInChildren<Text>();
        resourceCoffee = resourcesTab.transform.Find("Coffee").GetComponentInChildren<Text>();

        NumberChooser.SetActive(false);


    }

    public void EnableNumChooser()
    {
        HideAll();
        NumberChooser.SetActive(true);
    }

    void HideAll()
    {
        baseMenu.SetActive(false);
        NumberChooser.SetActive(false);
    }

    public void UpdateCounterHUD(int numToDisplay)
    {
        penguinAssignNumberText.text = "" + numToDisplay;
    }

    public void EnableBase()
    {
        HideAll();
        baseMenu.SetActive(true);
    }

    public void UpdateResources(Resource_Containter RC)
    {
        
        resourceFish.text = RC.r1.ToString();
        resourceIce.text = RC.r2.ToString();
        resourcePenguins.text = RC.r3.ToString();
        resourceCoffee.text = RC.r4.ToString();
    }
	
}

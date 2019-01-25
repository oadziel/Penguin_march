using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Manager {

    GameObject HUD;

    GameObject baseMenu;
    GameObject NumberChooser;

    Text penguinAssignNumberText;

    public void Start(GameObject HUD)
    {
        this.HUD = HUD;


        baseMenu = HUD.transform.Find("BaseMenu").gameObject;
        NumberChooser = HUD.transform.Find("NumberChooser").gameObject;
        penguinAssignNumberText = GameObject.Find("Counter").GetComponentInChildren<Text>();

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
	
}

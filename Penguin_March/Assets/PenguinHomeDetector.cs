using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinHomeDetector : MonoBehaviour {

    GameManager manager;

    public void Start()
    {
        manager = GameObject.FindObjectOfType<GameManager>();
    }

    void OnTriggerEnter(Collider other)
    {
        manager.IncReturnedPenguins();
    }
}

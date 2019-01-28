using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BabyPenguin_AI : MonoBehaviour {

    NavMeshAgent agent;
    Vector3 spawnPoint;
    Vector3 offset;

    float timer;

	// Use this for initialization
	void Start ()
    {
        offset = new Vector3(0.1f, 0, 0);
        agent = GetComponent<NavMeshAgent>();
        spawnPoint = GameObject.Find("SpawnPoint").transform.position;
        agent.destination = spawnPoint + offset;
        timer = randomTime();
	}

    void Update()
    {

        if(timer < 0)
        {
            GetComponentInChildren<Animator>().SetTrigger("sneeze");

            timer = randomTime();
        }

        timer -= Time.deltaTime;
    }

    float randomTime()
    {
        float time = 0;
        System.Random rand = new System.Random((int)(Time.time * 1000));

        time = rand.Next(0, 12);

        return time;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Seel_AI : MonoBehaviour {

    NavMeshAgent NMA;
    public float timeToReEvaluate = 15.0f;
    float timer;

    public float wanderRadius = 30;
	// Use this for initialization
	void Start () {
        NMA = GetComponent<NavMeshAgent>();
        timer = timeToReEvaluate;
        SetPos();

	}
	
	// Update is called once per frame
	void Update ()
    {
        if(timer < 0)
        {
            SetPos();
        }

        timer -= Time.deltaTime;
	}

     void SetPos()
    {
        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
        NMA.SetDestination(newPos);
        timer = timeToReEvaluate;
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDir = Random.insideUnitSphere * dist;
        randDir += origin;

        NavMeshHit hit;

        NavMesh.SamplePosition(randDir, out hit, dist, layermask);

        return hit.position;
    }
}

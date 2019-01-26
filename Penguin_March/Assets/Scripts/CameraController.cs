using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public GameObject target;
    public Vector3 offset;
    public float cameraMoveSpeed = 1.5f;

	
	// Update is called once per frame
	void Update ()
    {
        Vector3 newPos = Vector3.Slerp(transform.position + offset, target.transform.position, cameraMoveSpeed * Time.deltaTime);
        transform.position = newPos;
        transform.LookAt(target.transform.position);
	}
}

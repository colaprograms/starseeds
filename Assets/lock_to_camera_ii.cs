using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lock_to_camera_ii : MonoBehaviour {
    public GameObject cam;
    Vector3 relative_position;
    public bool doRotation;

	// Use this for initialization
	void Start () {
        relative_position = transform.position -
            cam.transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        // whee
        transform.position = cam.transform.position +
            cam.transform.TransformDirection(
                relative_position);
        if(doRotation)
            transform.rotation = cam.transform.rotation;
	}
}

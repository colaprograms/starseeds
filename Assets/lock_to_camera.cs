using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lock_to_camera : MonoBehaviour {
    public GameObject cam;
    float dist = 0.2f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        dist = Mathf.Clamp(dist + Input.mouseScrollDelta.y * 0.01f, 0.02f, 0.4f);
        transform.position = cam.transform.position +
            dist * cam.transform.forward;
	}
}

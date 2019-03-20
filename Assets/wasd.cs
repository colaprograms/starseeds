using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wasd : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
    
    void move(Vector3 direction) {
        transform.position += 0.2f * Time.deltaTime * direction;
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey("a"))
            move(-transform.right);
        if(Input.GetKey("w"))
            move(transform.forward);
        if(Input.GetKey("d"))
            move(transform.right);
        if(Input.GetKey("s"))
            move(-transform.forward);
        if(Input.GetKey("left shift"))
            move(transform.up);
        if(Input.GetKey("left ctrl"))
            move(-transform.up);
	}
}

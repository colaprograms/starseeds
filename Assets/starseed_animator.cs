using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class starseed_animator : MonoBehaviour {
    public Vector3 fromwhere;
    public Vector3 towhere;
    public float distance = 0;
    public float speed = 1;
    public bool stop = false;
    public Action callback = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(stop)
            return;
        distance += speed * Time.deltaTime;
        float d = Vector3.Distance(fromwhere, towhere);
        if(d <= distance) {
            moveto(1);
            stop = true;
            if(callback != null)
                callback();
        }
        else {
            moveto(distance / d);
        }
	}
    
    void moveto(float t) {
        transform.position = (1-t) * fromwhere + t * towhere;
    }
}

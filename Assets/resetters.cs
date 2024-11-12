using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if HAVE_COMPILER

public class resetters : MonoBehaviour {
    public rezz rezzer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey("f4"))
            rezzer.reboot();
	}
}

#else
public class resetters : MonoBehaviour
{   
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }
}

#endif
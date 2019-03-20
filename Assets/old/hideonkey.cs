using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hideonkey : MonoBehaviour
{
    bool active = true;
    public GameObject[] tohide;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("f2")) {
            active = !active;
            foreach(GameObject o in tohide)
                o.SetActive(active);
        }
    }
}

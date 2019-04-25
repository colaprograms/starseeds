using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loading : MonoBehaviour {
    const int NSTARS = 24685; //119614;
    const float SCALE = 0.72f;
    
    public GameObject loadBar;
    Vector3 s;

	// Use this for initialization
	void Start () {
        s = loadBar.transform.localPosition;
	}
	
	// Update is called once per frame
	void Update () {
        if(GameDad.manystars != null) {
            Destroy(gameObject);
        }
        else {
            int sp = GameDad.starprogress;
            float x = sp / (float)NSTARS;
            float angle =
                x < 0.98f? 0f: 180f * (x - 0.98f) / 0.02f;
            loadBar.transform.localScale = new Vector3(SCALE*2*x, 0.1f, 0.1f);
            loadBar.transform.localPosition = s + new Vector3(-SCALE+SCALE*x, 0, 0);
            loadBar.transform.rotation = Quaternion.AngleAxis(angle, Vector3.left);
        }
	}
}

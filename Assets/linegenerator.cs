using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class linegenerator : MonoBehaviour {
    public LineRenderer template;
    LineRenderer line;
    //float rotation = 0f;

	// Use this for initialization
	void Start () {
        const int COUNT = 3;
        const float SCALE = 1f;
		line = Instantiate(template);
        line.gameObject.SetActive(true);
        line.transform.parent = transform;
        line.transform.localPosition = new Vector3(0f, 0f, 0f);
        line.transform.localScale = new Vector3(SCALE, SCALE, SCALE);
        Vector3[] positions = new Vector3[COUNT];
        positions[0] = new Vector3(0.1f, 0.0f, 0.0f);
        positions[1] = new Vector3(0.0f, 0.0f, 0.0f);
        positions[2] = new Vector3(0.0f, 0.1f, 0.0f);
        line.positionCount = positions.Length;
        line.SetPositions(positions);
	}
	
	// Update is called once per frame
	void Update () {
	}
}

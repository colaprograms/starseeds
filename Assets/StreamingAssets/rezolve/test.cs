using System;
using UnityEngine;

class test: Rezolve {
    GameObject sphere; // test
    float time = 0f;
    
    public override void start() {
        throw new Exception("yikes!");
        //sphere = Rez(GameObject.Find("Sphere"));
        //sphere.GetComponent<Renderer>().material = Resources.Load("glitchmaterial") as Material;
        //sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //sphere.transform.position = new Vector3(0.0f, -1.2f, 0.4f);
    }
    
    public override void update() {
        //time += Time.deltaTime;
        //Find("tinysphere").transform.rotation = Quaternion.AngleAxis(-30*time, Vector3.left);
        //time += Time.deltaTime;
        //sphere.transform.position = new Vector3(0.02f * (float) Math.Sin(time),
        //    -1.2f + 0.02f * (float) Math.Cos(time), 0.4f);
    }
}
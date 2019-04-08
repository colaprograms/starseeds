using System;
using UnityEngine;

class annulu: Rezolve {
    GameObject cyl;
    bool _active = false;
    void setactive(bool active) {
        // loadin
        if(cyl != null && _active != active) {
            start_rotating();
            cyl.SetActive(active);
        }
        _active = active;
    }
    
    public override void start() {
        cyl = RezFind("cylindrical_annulus");
        //cyl = RezFind("Sphere");
    }
    
    float time;
    
    void start_rotating() { // test
        time = 0;
    }
    
    public override void update() {
        if(GameDad.selectedStar != -1) {
            time += 40 * Time.deltaTime;
            setactive(true); // test
            cyl.transform.position = GameDad.selectedStarLocation;
            float s = 0.004f * (1 + Mathf.Exp(-time/20f));
            cyl.transform.localScale = new Vector3(s, s, s * 0.1f);
            //cyl.transform.rotation = Quaternion.Euler(0, time, 0);
        }
        else {
            setactive(false);
        }
    }
}
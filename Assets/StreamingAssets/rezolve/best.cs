using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

class best: Rezolve {
    GameObject curdistance;
    GameObject maxdistance;
    float cur = 0f, max = 0f, lagcur = 0f, lagmax = 0f, SPEED = 3f;
    bool max_is_on = false;
    
    public override void start() {
        curdistance = GameObject.Find("curdistance");
        maxdistance = GameObject.Find("maxdistance");
        GameDad.update_best = update_best;
        curdistance.GetComponent<Text>().text = String.Format("", cur);
        maxdistance.GetComponent<Text>().text = "";
    }
    
    public void update_best() {
        cur = GameDad.farthest_green_star();
        if(cur > max)
            max = cur;
        if(cur < max)
            max_is_on = true;
    }
    
    public float lag(float x, float l) {
        if(x > l) {
            l += Mathf.Sqrt(x - l) * SPEED * Time.deltaTime;
            if(l > x)
                return x;
            else
                return l;
        }
        else {
            l -= Mathf.Sqrt(l - x) * SPEED * Time.deltaTime;
            if(l < x)
                return x;
            else
                return l;
        }
    }
    
    public override void update() {
        if(cur != lagcur || max != lagmax) {
            lagcur = lag(cur, lagcur);
            lagmax = lag(max, lagmax);
            curdistance.GetComponent<Text>().text = String.Format("{0:00.00} PC", lagcur);
            maxdistance.GetComponent<Text>().text = max_is_on? String.Format("MAXIMUM DISTANCE\n{0:00.00} PC", lagmax): "";
        }
    }
}
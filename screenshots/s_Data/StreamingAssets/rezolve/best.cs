using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

class best: Rezolve {
    GameObject curdistance;
    GameObject maxdistance, time_elapsed;
    float cur = 0f, max = 0f, lagcur = 0f, lagmax = 0f, SPEED = 1f, time = -1f;
    bool max_is_on = false, died = false;
    
    public override void start() {
        //curdistance = GameObject.Find("curdistance");
        //maxdistance = GameObject.Find("maxdistance");
        curdistance = GameObject.Find("curdistance_1");
        maxdistance = GameObject.Find("maxdistance_1");
        time_elapsed = GameObject.Find("time_elapsed");
        // rar
        GameDad.update_best = update_best;
        //curdistance.GetComponent<Text>().text = String.Format("", cur);
        //maxdistance.GetComponent<Text>().text = "";
        curdistance.GetComponent<TextMesh>().text = String.Format("", cur);
        maxdistance.GetComponent<TextMesh>().text = "";
    }
    
    public void update_best() {
        cur = GameDad.farthest_green_star();
        if(time == -1f) {
            if(GameDad.are_there_green_starseeds != null && GameDad.are_there_green_starseeds())
                time = 0;
        }
        if(cur == -1) {
            if(GameDad.are_there_green_starseeds != null && !GameDad.are_there_green_starseeds()) {
                died = true;
                //var t = curdistance.GetComponent<Text>();
                var t = curdistance.GetComponent<TextMesh>();
                var m = maxdistance.GetComponent<TextMesh>();
                t.color = new Color(0f, 0.92f, 0f, 1f);
                t.text = String.Format("GAME OVER");
                m.text = String.Format("BEST MAX {0:00.00}", max);
                return;
            }
            else
                cur = 0f;
        }
        if(cur > max)
            max = cur;
        max_is_on = cur < max;
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
        if(died)
            return;
        if(time >= 0) {
            time += Time.deltaTime;
            time_elapsed.GetComponent<TextMesh>().text = String.Format("TIME: {0:00}:{1:00}", Mathf.RoundToInt(time / 60),
                                                                                              Mathf.RoundToInt(time) % 60);
        }
        if(cur != lagcur || max != lagmax) {
            lagcur = lag(cur, lagcur);
            lagmax = lag(max, lagmax);
            curdistance.GetComponent<TextMesh>().text = String.Format("CURRENT MAX {0:00.00}", lagcur);
            maxdistance.GetComponent<TextMesh>().text = max_is_on? String.Format("BEST MAX {0:00.00}", lagmax): "";
            //curdistance.GetComponent<Text>().text = String.Format("CURRENT DISTANCE\n{0:00.00} PC", lagcur);
            //maxdistance.GetComponent<Text>().text = max_is_on? String.Format("MAXIMUM DISTANCE\n{0:00.00} PC", lagmax): "";
        }
    }
}
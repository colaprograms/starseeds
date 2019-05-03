using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI; // test

class best: Rezolve {
    GameObject curdistance, maxdistance, time_elapsed, gameend, bestmax, hitf;
    float cur = 0f, max = 0f, lagcur = 0f, lagmax = 0f, time = -1f;
    bool max_is_on = false, died = false;
    
    public override void start() {
        GameDad.gamesover = false;
        curdistance = GameObject.Find("curdistance_1");
        maxdistance = GameObject.Find("maxdistance_1");
        time_elapsed = GameObject.Find("time_elapsed");
        bestmax = GameObject.Find("bestmax");
        gameend = GameObject.Find("gameend");
        hitf = GameObject.Find("hitf");
        GameDad.update_best = update_best;
        put(curdistance, "");
        put(maxdistance, "");
        put(time_elapsed, "");
        put(bestmax, "");
        put(gameend, "");
        put(hitf, "");
        
        rezz.Log("pie is delicious");
    }
    
    void put(GameObject what, string text) {
        what.GetComponent<TextMesh>().text = text;
    }
    
    public void update_best() {
        if(died)
            return;
        if(GameDad.gamesover)
            return;
        
        cur = GameDad.farthest_green_star();
        if(time == -1f)
            start_timer_if_there_are_green_starseeds();
        if(cur == -1) {
            if(GameDad.are_there_green_starseeds != null && !GameDad.are_there_green_starseeds()) {
                gameover("HUMANS EXTINCT");
            }
            else
                cur = 0f;
        }
        if(cur > max)
            max = cur;
        max_is_on = cur < max;
    }
    
    void start_timer_if_there_are_green_starseeds() {
        if(GameDad.are_there_green_starseeds != null && GameDad.are_there_green_starseeds())
            time = 0;
    }
    
    void gameover(string whyend) {
        GameDad.gamesover = true;
        Find("reticle").SetActive(false);
        put(curdistance, "");
        put(maxdistance, "");
        put(time_elapsed, "");
        put(gameend, whyend);
        put(bestmax, String.Format("BEST MAX {0:00.00}", max));
        put(hitf, "HIT F4 TO RESTART");
    }

    
    public float lag(float x, float l) {
        if(x > l) {
            l += Mathf.Sqrt(x - l) * Config.starseed_speed * Time.deltaTime;
            if(l > x)
                return x;
            else
                return l;
        }
        else {
            l -= Mathf.Sqrt(l - x) * Config.starseed_speed * Time.deltaTime;
            if(l < x)
                return x;
            else
                return l;
        }
    }
    
    public override void update() {
        if(died)
            return;
        if(GameDad.gamesover)
            return; // rar
        if(time >= 0) {
            time += Time.deltaTime;
            time_elapsed.GetComponent<TextMesh>().text = String.Format("TIME: {0:00}:{1:00}", Mathf.FloorToInt(time / 60),
                                                                                              Mathf.FloorToInt(time) % 60);
        }
        if(time >= Config.length_of_game) {
            gameover("TIME'S UP");
            return;
        }
        if(cur != lagcur || max != lagmax) {
            lagcur = lag(cur, lagcur);
            lagmax = lag(max, lagmax);
            put(curdistance, String.Format("CURRENT MAX {0:00.00}", lagcur));
            put(maxdistance, max_is_on? String.Format("BEST MAX {0:00.00}", lagmax): "");
            //curdistance.GetComponent<Text>().text = String.Format("CURRENT DISTANCE\n{0:00.00} PC", lagcur);
            //maxdistance.GetComponent<Text>().text = max_is_on? String.Format("MAXIMUM DISTANCE\n{0:00.00} PC", lagmax): "";
        }
    }
}
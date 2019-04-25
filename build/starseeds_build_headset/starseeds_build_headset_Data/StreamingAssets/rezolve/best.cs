using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

class best: Rezolve {
    GameObject curdistance;
    GameObject maxdistance, time_elapsed;
    float cur = 0f, max = 0f, lagcur = 0f, lagmax = 0f, time = -1f;
    bool max_is_on = false, died = false;
    
    public override void start() {
        GameDad.gamesover = false;
        
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
        time_elapsed.GetComponent<TextMesh>().text = "";
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
                gameover("GAME OVER");
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
    
    void gameover(string whysover) {
        GameDad.gamesover = true;
        //var t = curdistance.GetComponent<Text>();
        var t = curdistance.GetComponent<TextMesh>();
        var m = maxdistance.GetComponent<TextMesh>();
        t.color = Config.game_over_text_colour;
        t.text = String.Format(whysover);
        m.text = String.Format("BEST MAX {0:00.00}", max);
        return;
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
            curdistance.GetComponent<TextMesh>().text = String.Format("CURRENT MAX {0:00.00}", lagcur);
            maxdistance.GetComponent<TextMesh>().text = max_is_on? String.Format("BEST MAX {0:00.00}", lagmax): "";
            //curdistance.GetComponent<Text>().text = String.Format("CURRENT DISTANCE\n{0:00.00} PC", lagcur);
            //maxdistance.GetComponent<Text>().text = max_is_on? String.Format("MAXIMUM DISTANCE\n{0:00.00} PC", lagmax): "";
        }
    }
}
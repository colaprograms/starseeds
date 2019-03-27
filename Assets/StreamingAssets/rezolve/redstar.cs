using UnityEngine;
using System;
using System.Collections.Generic;

public class redstardata {
    public greenstar star;
    public int ix;
    
    public redstardata(int _ix, greenstar _star) {
        ix = _ix;
        star = _star;
    }
    
    public void flip() {
        star.size = 0.036f - star.size;
        GameDad.update_green(ix);
    }
    
    public void update()
    {
    }
}

public class redstar: Rezolve
{
    Dictionary<int, redstardata> red_stars;
    float time = 0f;
    
    public override void start() // test
    {
        red_stars = new Dictionary<int, redstardata>();
        GameDad.give_chance_to_make_red_star = give_chance_to_make_red_star;
    }
    
    public redstardata make_new_red_star(int ix)
    {
        greenstar star = new greenstar();
        star.isred = true;
        star.size = 0.02f;
        redstardata r = new redstardata(ix, star);
        return r;
    }
    
    public bool give_chance_to_make_red_star(int ix) {
        red_stars[ix] = make_new_red_star(ix);
        GameDad.add_green(ix, red_stars[ix].star);
        return true;
        
        /*
        if(UnityEngine.Random.value < 0.1) {
            return true;
        }
        */
        return false;
    }
    
    public void starseed_lands_on_star(int ix) {
    }
    
    public override void update()
    {
        if(red_stars.Count > 0) {
            time += Time.deltaTime;
            if(time > 0.2f) {
                time -= 0.2f;
                foreach(var red in red_stars.Values) {
                    red.flip();
                }
            }
            foreach(var red in red_stars.Values) {
                red.update();
            }
        }
        else {
            time = 0;
        }
    }
}
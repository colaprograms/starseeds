using UnityEngine;
using System;
using System.Collections.Generic;

public class redstardata {
    public greenstar star;
    public int ix;
    public float launch_clock = 0f;
    public float die_clock = 0f;
    public Vector3 vector;
    public const float DIE_SPEED = 30f;
    
    public redstardata(int _ix, greenstar _star, Vector3 _vecto) {
        ix = _ix;
        star = _star;
        vector = _vecto;
    }
    
    public void flip() {
        star.size = 0.036f - star.size;
        GameDad.update_green(ix);
    }
    
    public void update()
    {
        launch_clock += Time.deltaTime;
        if(redstar.exponential_decay(1/DIE_SPEED)) {
            GameDad.red_star_evaporate(ix);
            return;
        }
    }
}

public class redstar: Rezolve // test
{
    Dictionary<int, redstardata> red_stars;
    Dictionary<int, float> quiet_stars;
    float time = 0f;
    const float QUIET_DURATION = 10f;
    
    public static bool exponential_decay(float speed) {
        return UnityEngine.Random.value > Mathf.Exp(-Time.deltaTime * speed);
    }
    
    public override void start() // test
    {
        red_stars = new Dictionary<int, redstardata>();
        GameDad.give_chance_to_make_red_star = give_chance_to_make_red_star;
        GameDad.spamseed_lands_on_star = spamseed_lands_on_star;
        GameDad.red_star_evaporate = red_star_evaporate;
        quiet_stars = new Dictionary<int, float>();
    }
    
    public redstardata make_new_red_star(int ix, Vector3 vector)
    {
        greenstar star = new greenstar();
        star.type = greenstar.Type.Red;
        star.size = 0.02f;
        redstardata r = new redstardata(ix, star, vector);
        return r;
    }
    
    public void gamedad_make_red(int ix, Vector3 vector)
    {
        red_stars[ix] = make_new_red_star(ix, vector);
        GameDad.add_green(ix, red_stars[ix].star);
    }
    
    public void red_star_evaporate(int ix)
    {
        red_stars.Remove(ix);
        GameDad.remove_green(ix);
        quiet_stars[ix] = QUIET_DURATION;
    }
    
    public bool give_chance_to_make_red_star(int start, Vector3 startLocation, int end, Vector3 endLocation) {
        if(GameDad.add_green == null)
            return false;
        if(quiet_stars.ContainsKey(end))
            return false; // star is quiet QUIET_DURATION seconds after spambots die
        Debug.Log("making red star maybe");
        if(UnityEngine.Random.value > 0.1)
            return false;
        Debug.Log("trying red star");
        gamedad_make_red(end, startLocation - endLocation);
        if(GameDad.send_red_hook != null)
            GameDad.send_red_hook(end, start);
        return true;
    }
    
    public void spamseed_lands_on_star(int ix, Vector3 vector) {
        if(GameDad.is_green(ix) && GameDad.get_green(ix).type == greenstar.Type.Green) {
            GameDad.remove_green(ix);
            gamedad_make_red(ix, vector);
        }
    }
    
    public void expirequiet()
    {
        foreach(var ix in quiet_stars.Keys.ToArray()) {
            if((quiet_stars[ix] -= Time.deltaTime) < 0)
                quiet_stars.Remove(ix);
        }
    }
    
    public override void update()
    {
        expirequiet();
        
        if(red_stars.Count > 0) {
            redstardata[] stars = red_stars.Values.ToArray();
            time += Time.deltaTime;
            if(time > 0.2f) {
                time -= 0.2f;
                foreach(var red in stars)
                    red.flip();
            }
            foreach(var red in stars)
                red.update();
        }
        else {
            time = 0;
        }
    }
}
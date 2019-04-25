using UnityEngine;
using System; // rar
using System.Collections.Generic;

// wraps a starseed gameobject and stuff
public class starseed_owner
{
    GameObject ob;
    bool stop = false;
    Action callback = null; // called when it reaches the end
    Vector3 startLocation, endLocation;
    float speed = 1f;
    float distance = 0f;
    float period_time = 0f;
    const float PERIOD = 0.6f;
    bool must_delete_material = false;
    float[] pattern = { 1f, 0.8f, 0.6f, 0.4f, 0.4f, 0.4f, 0.4f, 0.4f };
    public bool isred = false;
    
    public starseed_owner(Vector3 _startLocation, Vector3 _endLocation, float _speed, GameObject _ob, Action _callback, bool _isred)
    {
        ob = _ob;
        callback = _callback;
        speed = _speed;
        startLocation = _startLocation;
        endLocation = _endLocation;
        isred = _isred;
        moveto(0);
        ob.SetActive(true);
        setcolor();
    }
    
    public void setcolor()
    {
        period_time += Time.deltaTime / PERIOD;
        while(period_time >= 1)
            period_time -= 1;
        
        int where = Mathf.FloorToInt(period_time * pattern.Length);
        float c = pattern[where];
        Color cc = isred? Config.starseed_red * c:
                          Config.starseed_green * c;
        ob.GetComponent<Renderer>().material.SetColor("_EmissionColor", cc);
        // apparently unity generates a new material when you do stuff to
        // the material on an object, and you have to delete the material
        // when the object vanishes or you get a memory leak.
        must_delete_material = true;
    }
    
    public bool update()
    {
        if(stop)
            return false;
        setcolor();
        if(GameDad.gamesover) { // starseed stop when game is over (but they keep on blinking)
            return true;
        }
        distance += speed * Time.deltaTime;
        float d = Vector3.Distance(startLocation, endLocation);
        if(d <= distance) {
            moveto(1);
            stop = true;
            if(callback != null)
                callback();
        }
        else {
            moveto(distance / d);
        }
        return true;
    }
    
    void moveto(float t) {
        ob.transform.position = (1-t) * startLocation + t * endLocation;
    }
    
    public void derez(Action<GameObject> DeRez) {
        if(ob != null) {
            if(must_delete_material)
                GameObject.Destroy(ob.GetComponent<Renderer>().material);
            DeRez(ob);
            ob = null;
        }
    }
}

public class launch: Rezolve
{
    int id = 0;
    Dictionary<int, starseed_owner> starseeds;
    
    public override void start()
    {
        GameDad.linedrawn_hook = linedrawn;
        GameDad.send_red_hook = send_red;
        GameDad.are_there_green_starseeds = are_there_green_starseeds;
        starseeds = new Dictionary<int, starseed_owner>();
    }
    
    public override void update()
    {
        List<int> toupdate = new List<int>();
        foreach(var idx in starseeds.Keys)
            toupdate.Add(idx);
        foreach(var idx in toupdate) {
            if(!starseeds[idx].update()) {
                starseeds[idx].derez(DeRez);
                starseeds.Remove(idx);
            }
        }
    }
    
    public void linedrawn(int start, Vector3 startLocation, int end, Vector3 endLocation)
    {
        if(GameDad.gamesover) {
            return; // no starseeds launch after game over
        }
        make_starseed(start, startLocation, end, endLocation, 0.01f);
    }
    
    public void send_red(int start, int end)
    {
        if(GameDad.gamesover) {
            return; // no starseeds launch after game over
        }
        Vector3 startLocation = GameDad.whereisstar(start);
        Vector3 endLocation = GameDad.whereisstar(end);
        make_starseed(start, startLocation, end, endLocation, 0.01f, true);
    }
    
    public bool are_there_green_starseeds()
    {
        bool there_are_green_starseeds = false;
        foreach(var seed in starseeds.Values) {
            if(!seed.isred)
                there_are_green_starseeds = true;
        }
        return there_are_green_starseeds;
    }
    
    public void make_starseed(int start, Vector3 startLocation, int end, Vector3 endLocation, float speed, bool isred = false)
    {
        GameObject starseed = isred? RezFind("spamseed_icon"): RezFind("starseed_icon");
        if(GameDad.get_particle_system_scale != null) {
            float scale = GameDad.get_particle_system_scale();
            starseed.transform.localScale *= scale;
            speed *= Mathf.Sqrt(scale);
        }
        Action whenhit = delegate() {
            if(!isred) {
                // a green starseed has hit a star
                
                // check if the hooks we need even exist
                if(GameDad.is_green == null || GameDad.get_green == null)
                    return;
                
                // if the destination star is bare, then:
                // 1. call give_chance_to_make_red_star
                // 2. if this is false, then make it green
                if(!GameDad.is_green(end)) {
                    bool stop = false;
                    if(GameDad.give_chance_to_make_red_star != null)
                        stop = GameDad.give_chance_to_make_red_star(start, startLocation, end, endLocation);
                    if(GameDad.is_green(end))
                        stop = true;
                    
                    if(!stop) {
                        GameDad.add_green(end, null);
                        if(GameDad.update_best != null) GameDad.update_best();
                    }
                }
                else if(GameDad.get_green(end).type == greenstar.Type.Red) { // test
                    // a green starseed has hit a red star.
                    // send a red starseed back
                    if(GameDad.send_red_hook != null)
                        GameDad.send_red_hook(end, start);
                }
                /*
                var star = new greenstar();
                star.isred = true;
                GameDad.add_green(end, star);
                */
            }
            else {
                // a red starseed has hit a star
                
                if(GameDad.spamseed_lands_on_star != null) {
                    GameDad.spamseed_lands_on_star(end, endLocation - startLocation); // test
                    if(GameDad.update_best != null) GameDad.update_best();
                }
            }
        };
        starseeds[id++] = new starseed_owner(startLocation, endLocation, speed, starseed, whenhit, isred);
    }
}
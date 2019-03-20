using UnityEngine;
using System;
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
    
    public starseed_owner(Vector3 _startLocation, Vector3 _endLocation, float _speed, GameObject _ob, Action _callback)
    {
        ob = _ob;
        callback = _callback;
        speed = _speed;
        startLocation = _startLocation;
        endLocation = _endLocation;
        moveto(0);
        ob.SetActive(true);
        setcolor();
    }
    
    public void setcolor()
    {
        period_time += Time.deltaTime / PERIOD;
        while(period_time > 1)
            period_time -= 1;
        
        int where = Mathf.FloorToInt(period_time * pattern.Length);
        float c = pattern[where];
        ob.GetComponent<Renderer>().material.SetColor("_EmissionColor",
            new Color(0f, c, 0f, c));
        // apparently unity generates a new material when you do stuff to
        // the material on an object, and you have to delete the material
        // when the object vanishes or you get a memory leak.
        must_delete_material = true;
    }
    
    public bool update()
    {
        if(stop)
            return false;
        distance += speed * Time.deltaTime;
        setcolor();
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
        starseeds = new Dictionary<int, starseed_owner>();
    }
    
    public override void update()
    {
        List<int> toremove = new List<int>();
        foreach(var idx in starseeds.Keys) {
            if(!starseeds[idx].update()) {
                // update returned false, so we should remove this starseed, but we
                // shouldn't mess around with the dictionary while we're iterating over it
                toremove.Add(idx);
            }
        }
        // remove all the starseeds that we need to remove
        foreach(var idx in toremove) {
            starseeds[idx].derez(DeRez);
            starseeds.Remove(idx);
        }
    }
    
    public void linedrawn(int start, Vector3 startLocation, int end, Vector3 endLocation)
    {
        make_starseed(start, startLocation, end, endLocation, 0.01f);
    }
    
    public void make_starseed(int start, Vector3 startLocation, int end, Vector3 endLocation, float speed)
    {
        GameObject starseed = RezFind("starseed_icon");
        Action whenhit = delegate() {
            if(!GameDad.is_green(end))
                GameDad.add_green(end);
        };
        starseeds[id++] = new starseed_owner(startLocation, endLocation, speed, starseed, whenhit);
    }
}
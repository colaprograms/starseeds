using UnityEngine;
using System;
using System.Collections.Generic;

public class redstar_organizer {
    public SortedDictionary<float, int> candidates;
    public Vector3 vector;
    public Vector3 loc;
    
    public redstar_organizer(int star, Vector3 _vector) {
        candidates = new SortedDictionary<float, int>();
        starmap sm = GameDad.manystars;
        loc = sm.getstar(star).vec;
        vector = _vector;
        foreach(var ix in sm.stars_near_vec(loc, 4f)) {
            if(ix == star)
                continue;
            star s = sm.getstar(ix);
            candidates[starscore(s)] = ix;
        }
    }
    
    float starscore(star s) {
        Vector3 v = s.vec - loc;
        v /= v.magnitude;
        return -Vector3.Dot(v, vector);
    }
    
    public int[] array() {
        return candidates.Values.ToArray();
    }
}

public class redstardata {
    public greenstar star;
    public int ix;
    public bool has_a_launcher = false;
    public float launch_clock = 0f;
    public float die_clock = 0f;
    public Vector3 vector;
    public int[] candidates;
    int current_candidate = 0;
    
    public redstardata(int _ix, greenstar _star, Vector3 _vecto) {
        ix = _ix;
        star = _star;
        vector = _vecto;
        candidates = (new redstar_organizer(ix, vector)).array();
    }
    
    public void flip() {
        if(ix == GameDad.sol_index)
            return; // this is handled by earthblinks
        if(!GameDad.is_green(ix))
            throw new Exception("red star was not green??");
        if(star.size == 0.02f) {
            star.size = 0.011f;
            star.color = greenstar.StarColor.RedSmall;
        }
        else {
            star.size = 0.02f;
            star.color = greenstar.StarColor.RedLarge;
        }
        //star.size = 0.031f - star.size;
        GameDad.update_green(ix);
    }
    
    public void update()
    {
        if(has_a_launcher) {
            launch_clock += Time.deltaTime;
            if(launch_clock > 2f) {
                launch_clock -= 2f;
                launch();
            }
        }
        if(redstar.exponential_decay(1/Config.red_average_life)) {
            GameDad.red_star_evaporate(ix);
            return;
        }
    }
    
    public void launch() {
        bool stop = false;
        while(0 <= current_candidate && current_candidate < candidates.Length && !stop) {
//            if(GameDad.star_corresponds_to_particle(candidates[current_candidate])) {
//                GameDad.send_red_hook(ix, candidates[current_candidate]);
//                stop = true;
//            }
            GameDad.send_red_hook(ix, candidates[current_candidate]);
            stop = true;
            current_candidate++;
        }
        if(!stop) {
            if(candidates.Length > 0) {
                int r = UnityEngine.Random.Range(0, candidates.Length);
                GameDad.send_red_hook(ix, candidates[r]);
//                if(GameDad.star_corresponds_to_particle(candidates[r])) {
//                    GameDad.send_red_hook(ix, candidates[r]);
//                }
            }
        }
    }
}

public class redstar: Rezolve // test
{
    Dictionary<int, redstardata> red_stars;
    Dictionary<int, float> quiet_stars;
    float time = 0f;
    
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
    
    public void gamedad_make_red(int ix, Vector3 vector, bool has_a_launcher)
    {
        red_stars[ix] = make_new_red_star(ix, vector);
        if(has_a_launcher)
            red_stars[ix].has_a_launcher = true;
        GameDad.add_green(ix, red_stars[ix].star);
    }
    
    public void red_star_evaporate(int ix)
    {
        red_stars.Remove(ix);
        GameDad.remove_green(ix);
        quiet_stars[ix] = Config.quiettime;
    }
    
    public bool give_chance_to_make_red_star(int start, Vector3 startLocation, int end, Vector3 endLocation) {
        if(GameDad.add_green == null)
            return false;
        if(quiet_stars.ContainsKey(end))
            return false; // star is quiet QUIET_DURATION seconds after spambots die
        float r = UnityEngine.Random.value;
        if(r > Config.chance_of_red) // test
            return false;
        gamedad_make_red(end, startLocation - endLocation, true); // test
        return true;
    }
    
    public void spamseed_lands_on_star(int ix, Vector3 vector) {
        if(GameDad.is_green(ix) && GameDad.get_green(ix).type == greenstar.Type.Green) {
            GameDad.remove_green(ix);
            gamedad_make_red(ix, vector, true);
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
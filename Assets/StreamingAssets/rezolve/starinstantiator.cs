using UnityEngine;
using System;
using System.Collections.Generic;

public static class gaussian { // test
    static double nonzero_random() {
        double z = 0.0;
        while(z == 0)
            z = UnityEngine.Random.value;
        return z;
    }
    public static Vector2 two() {
        double z1 = nonzero_random();
        double z2 = nonzero_random() * 2 * System.Math.PI;
        double ra = System.Math.Sqrt(-2 * System.Math.Log(z1));
        return new Vector2(
            (float) (ra * System.Math.Cos(z2)),
            (float) (ra * System.Math.Sin(z2))
        );
    }
    public static Vector3 three() {
        Vector2 a = two();
        Vector2 b = two();
        return new Vector3(a.x, a.y, b.x);
    }
    public static Vector3 three(float cutoff) {
        Vector3 a = three();
        while(a.magnitude > 1) // test
            a = three();
        return a;
    }
}

struct range {
    public int start;
    public int length;
}

public class starinstantiator : Rezolve
{
    public GameObject system; // particle system to copy
    public GameObject reticle; // the positioning reticle glued to the player's head
    GameObject instantiated; // the copied particle system
    
    StarInstantiatorMode mode = StarInstantiatorMode.GreenSelect;
    int mouseoverindex = -1; // star moused over
    
    ParticleSystem.Particle[] particles;
    int nvalidparticle = 0;
    Dictionary<int, range> star_to_particle_range;
    Dictionary<int, int> particle_to_first_star;
    Dictionary<int, greenstar> which_stars_are_green;
    bool setparticles_later = false;
    
    // This is the largest distance that you can select a star from
    const float SELECT_POSSIBLE_DISTANCE = 0.08f;
    float time = 0;
    
    bool started = false;
    const float SPACE_SCALE = 0.2f / 18;
    
    public override void start()
    {
        star_to_particle_range = new Dictionary<int, range>();
        particle_to_first_star = new Dictionary<int, int>();
        which_stars_are_green = new Dictionary<int, greenstar>();
        GameDad.setmode = setmode;
        GameDad.add_green = add_green;
        GameDad.remove_green = remove_green;
        GameDad.update_green = update_green;
        GameDad.is_green = delegate(int i) {
            return which_stars_are_green.ContainsKey(i);
        };
        GameDad.get_green = delegate(int i) {
            return which_stars_are_green[i];
        };
        GameDad.whereisstar = whereisstar;
        GameDad.realvector_to_spacevector = realvector_to_spacevector;
        GameDad.star_corresponds_to_particle = star_corresponds_to_particle;
        GameDad.farthest_green_star = farthest_green_star;
    }
    
    void setmode(StarInstantiatorMode _mode) {
        mode = _mode;
    }
    
    void setparticles()
    {
        if(particles == null)
            throw new System.Exception("particle array not initialized");
        // Documentation claims that you can call SetParticles(array, size, index)
        // to set a range of particles but apparently not.
        instantiated.GetComponent<ParticleSystem>()
            .SetParticles(particles, nvalidparticle);
    }
    
    public void remove_green(int i)
    {
        if(which_stars_are_green.ContainsKey(i)) { // test
            range r = star_to_particle_range[i];
            int count = set_particles_from_star(r.start, GameDad.manystars.starlist(i));
            if(count != r.length)
                throw new Exception("starlist length was different from the expected range");
            which_stars_are_green.Remove(i);
            setparticles_later = true;
        }
        else {
            throw new System.Exception("not yet green");
        }
    }
    
    public void add_green(int i, greenstar star)
    {
        if(which_stars_are_green.ContainsKey(i))
            throw new System.Exception("already green");
        else {
            if(star == null)
                star = new greenstar();
            //Debug.Log("running");
            //Debug.Log(star);
            range r = star_to_particle_range[i];
            bool large_star = false;
            bool reveal_neighbourhood = false;
            Color? thecolor = null;
            switch(star.type) {
                case greenstar.Type.Green:
                    thecolor = Config.getcolor(greenstar.StarColor.Green);
                    reveal_neighbourhood = true;
                    break;
                case greenstar.Type.Red:
                    thecolor = Config.getcolor(greenstar.StarColor.RedLarge);
                    large_star = true;
                    break;
                case greenstar.Type.Quiet:
                    break;
                default:
                    throw new Exception("unknown type");
            }
            if(thecolor.HasValue) {
                particles[r.start].startColor = thecolor.Value;
                particles[r.start].startSize = large_star? 0.02f: 0.01f;
                for(int j = 1; j < r.length; j++) {
                    //particles[r.start + j].startSize = 0f;
                    particles[r.start + j].startColor = thecolor.Value;
                }
            }
            else {
                int count = set_particles_from_star(r.start, GameDad.manystars.starlist(i));
                if(count != r.length)
                    throw new Exception("starlist length was different from the expected range");
            }
            which_stars_are_green[i] = star;
            if(reveal_neighbourhood) {
                foreach(var starix in stars_visible_from(GameDad.manystars.getstar(i).vec)) {
                    allocate_particle_for_star(starix);
                }
            }
            setparticles_later = true;
        }
    }
    
    void mouseover_nothing() {
        if(mouseoverindex != -1)
            mouseoverindex = -1;
        GameDad.selectedStar = -1;
        GameDad.selectedStarLocation = new Vector3(0, 0, 0);
        if(GameDad.starmouseover_hook != null)
            GameDad.starmouseover_hook(-1, new Vector3(0, 0, 0));
    }
    
    void mouseover_star(int index) {
        if(index < 0 || index >= particles.Length)
            throw new System.Exception("hey index was invalid");
        int starix = particle_to_first_star[index];
        if(index != star_to_particle_range[starix].start)
            throw new System.Exception("hey star was not the first star in group");
        mouseoverindex = index;
        Vector3 loca = particlepos(mouseoverindex);
        GameDad.selectedStar = starix;
        GameDad.selectedStarLocation = loca;
        if(GameDad.starmouseover_hook != null)
            GameDad.starmouseover_hook(starix,
                                       loca);
    }
    
    Vector3 get_inv_reticle_posn()
    {
        // reticle position in the particle system frame
        return instantiated.transform.InverseTransformPoint(
            reticle.transform.position);
    }
    
    int find_closest(IEnumerable<int> indices, Func<int, float> closeness)
    {
        float dist = SELECT_POSSIBLE_DISTANCE;
        float curdist;
        int closestindex = -1;
        foreach(int i in indices) {
            curdist = closeness(i);
            if(curdist < dist) {
                dist = curdist;
                closestindex = i;
            }
        }
        return closestindex;
    }
    
    IEnumerable<int> inrange(int start, int length) {
        for(int i = 0; i < length; i++)
            yield return i + start;
    }
    
    IEnumerable<int> greenr() {
        // whee
        foreach(var ix in which_stars_are_green.Keys) {
            if(which_stars_are_green[ix].type == greenstar.Type.Green)
                yield return star_to_particle_range[ix].start;
        }
    }
    
    bool check_if_green(int ix) {
        if(which_stars_are_green.ContainsKey(ix))
            if(which_stars_are_green[ix].type == greenstar.Type.Green)
                return true;
        return false;
    }
    
    IEnumerable<int> nongreenr() {
        for(int i = 0; i < nvalidparticle; i++) {
            int ix = particle_to_first_star[i];
            if(check_if_green(ix))
                continue;
            yield return i;
        }
    }
    
    int closest_valid_star_particle(bool must_have_starseed_launcher = false) {
        Vector3 reticle_posn = get_inv_reticle_posn();
        IEnumerable<int> indices;
        switch(mode) {
            case StarInstantiatorMode.AnyStarSelect:
                indices = inrange(0, nvalidparticle);
                break;
            case StarInstantiatorMode.GreenSelect:
                indices = greenr(); //which_stars_are_green.Keys;
                break;
            case StarInstantiatorMode.NonGreenSelect:
                indices = nongreenr();
                break;
            case StarInstantiatorMode.NoSelect:
                indices = inrange(0, 0);
                break;
            default:
                throw new Exception("no such mode");
        }
        return find_closest(indices, delegate(int i) {
            if(star_to_particle_range[particle_to_first_star[i]].start != i) {
                // the star is not the first star in its dang group
                return Mathf.Infinity;
            }
            return Vector3.Distance(reticle_posn, particles[i].position);
        });
    }
    
    ParticleSystem.Particle get_particle_from_star(star st)
    {
        float lum = Mathf.Exp(-st.absmag / 5 * Mathf.Log(100));
        lum = Mathf.Pow(lum, 1f/4);
        /* Ballesteros' formula (from Wikipedia) */
        double T = 4600 * (1 / (0.92 * st.ci + 1.7) + 1/(0.92 * st.ci + 0.62));
        Color temp = Mathf.CorrelatedColorTemperatureToRGB((float) T);
        return new ParticleSystem.Particle {
            position = star_to_position_in_particle_reference_frame(st),
            startLifetime = Mathf.Infinity,
            startSize = Mathf.Max(st.part_of_group? 0.001f: 0.003f, 0.02f * lum), //startSize = Mathf.Max(0.002f, 0.004f * Mathf.Sqrt(star.radius)),
            startColor = temp
        };
    }
    
    int set_particles_from_star(int starix, IEnumerable<star> stars)
    {
        int count = 0;
        foreach(var star in stars) {
            particles[starix + count] = get_particle_from_star(star);
            count++;
        }
        return count;
    }
    
    void allocate_particle_for_star(int ix)
    {
        if(star_to_particle_range.ContainsKey(ix))
            return;
        int count = set_particles_from_star(nvalidparticle, GameDad.manystars.starlist(ix));
        star_to_particle_range[ix] = new range {
            start = nvalidparticle,
            length = count
        };
        for(int i = 0; i < count; i++) {
            particle_to_first_star[nvalidparticle++] = ix;
        }
    }
    
    IEnumerable<int> stars_visible_from(Vector3 th)
    {
        return GameDad.manystars.stars_near_vec(th, 4);
        
        /*int nstars = GameDad.manystars.nstars();
        Func<star, bool> near = delegate(star st) {
            return Vector3.Distance(st.vec, th) < 4;
        };
        for(int j = 0; j < nstars; j++) {
            star s = GameDad.manystars.getstar(j);
            if(s.part_of_group)
                continue;
            if(near(s))
                yield return j;
        }*/
    }
    
    void updateselect()
    {
        if(reticle) {
            int closestindex =
                mode == StarInstantiatorMode.NoSelect? -1: closest_valid_star_particle();
            if(closestindex != -1)
                mouseover_star(closestindex);
            else
                mouseover_nothing();
        }
    }
    
    /* There are three different position things here:
     * 1. The positions in the star database (in parsecs), st.vec
     * 2. The positions in the particle system frame, particles[index].position or st.vec * SPACE_SCALE
     * 3. The position in the world frame, instantiated.transform.TransformPoint(particles[index].position) */
    
    public Vector3 particlepos(int index) {
        // particle position in the world frame
        return instantiated.transform.TransformPoint(particles[index].position);
    }
    
    public Vector3 realvector_to_spacevector(Vector3 vector) {
        return instantiated.transform.InverseTransformVector(vector) / SPACE_SCALE;
    }
    
    public Vector3 star_to_position_in_particle_reference_frame(star st) {
        return SPACE_SCALE * st.vec;
    }
    
    public Vector3 whereisstar(int starix) {
        //throw new Exception(String.Format("there is no star with index {0}", starix));
        if(!star_corresponds_to_particle(starix)) {
            return instantiated.transform.TransformPoint(
                star_to_position_in_particle_reference_frame(GameDad.manystars.getstar(starix))); // test
//            throw new Exception(String.Format("star {0} does not correspond to a particle", starix));
        }
        range r = star_to_particle_range[starix];
        return particlepos(r.start);
    }
    
    public bool star_corresponds_to_particle(int starix) {
        if(starix < 0 || starix >= GameDad.manystars.nstars())
            throw new Exception(String.Format("there is no star with index {0}", starix));
        return star_to_particle_range.ContainsKey(starix);
    }
    
    public void update_green(int index) {
        greenstar star = which_stars_are_green[index];
        if(!float.IsNaN(star.size)) {
            range s = star_to_particle_range[index];
            particles[s.start].startSize = star.size;
            particles[s.start].startColor = Config.getcolor(star.color);
            setparticles_later = true;
        }
    }
    
    public float farthest_green_star() {
        float cur = -1f;
        if(GameDad.manystars == null)
            return 0f;
        foreach(var ix in which_stars_are_green.Keys) {
            if(which_stars_are_green[ix].type != greenstar.Type.Green)
                continue;
            float m = GameDad.manystars.getstar(ix).vec.magnitude;
            if(cur < m)
                cur = m;
        }
        return cur;
    }
    
    // Update is called once per frame
    public override void update()
    {
        if(GameDad.manystars == null)
            return;
        if(!started) {
            started = true;
            reticle = GameObject.Find("reticle");
            system = Find("star_particles");
            
            instantiated = Rez(system);
            int nstars = GameDad.manystars.nstars();
            particles = new ParticleSystem.Particle[nstars+1];
            instantiated.SetActive(true);
            
            allocate_particle_for_star(0);
            add_green(0, null);
            GameDad.sol_index = 0;
            foreach(var starix in stars_visible_from(new Vector3(0, 0, 0))) {
                allocate_particle_for_star(starix);
                //Debug.Log(String.Format("allocated {0}", starix));
            }
            setparticles_later = true;
            Debug.Log(System.String.Format("{0} stars", nvalidparticle));
        }
        if(!instantiated)
            throw new System.Exception("not instantiated?");
        if(setparticles_later) {
            setparticles_later = false;
            setparticles();
        }
        updateselect();
        time += Time.deltaTime;
    }
}

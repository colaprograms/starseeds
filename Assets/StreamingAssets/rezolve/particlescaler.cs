using UnityEngine;
using System;
using System.Collections.Generic;

public class particlescaler: Rezolve
{
    float scale = 2f;
    Vector3 position = new Vector3(0f, -0.2f, 0.4f);
    bool dirty = false; // test
    const float SPEED = 0.1f;
    
    public override void start() {
        GameDad.get_particle_system_scale = get_particle_system_scale;
        GameDad.get_particle_system_position = get_particle_system_position;
        Debug.Log("set up");
    }
    
    public float get_particle_system_scale() {
        return scale;
    }
    
    public Vector3 get_particle_system_position() {
        return position;
    }
    
    void scaleby(float s) {
        scale *= s;
        dirty = true;
    }
    
    void moveby(Vector3 z) {
        position += scale * z * Time.deltaTime * SPEED;
        dirty = true;
    }
    
    public override void update() {
        if(GameDad.update_particle_system_tf == null)
            return;
        if(Input.GetKeyDown("-"))
            scaleby(1/1.1f);
        if(Input.GetKeyDown("="))
            scaleby(1.1f);
        if(Input.GetKey("left"))
            moveby(Vector3.left);
        if(Input.GetKey("right"))
            moveby(Vector3.right);
        if(Input.GetKey("up"))
            moveby(Vector3.forward);
        if(Input.GetKey("down"))
            moveby(Vector3.back);
        if(dirty) {
            Debug.Log(String.Format("updating {0}", scale));
            GameDad.update_particle_system_tf();
            dirty = false;
        }
    }
}
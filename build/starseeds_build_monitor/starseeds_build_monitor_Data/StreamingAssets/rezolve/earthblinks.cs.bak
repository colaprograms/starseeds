using UnityEngine;
using System;
using System.Collections.Generic;

public class earthblinks: Rezolve
{
    const float PERIOD = 60f;
    GameObject annulus;
    float time;
    
    GameObject light;
    Color RED_COLOR = new Color(1f, 0f, 0f, 1f);
    Color GREEN_COLOR = new Color(0f, 1f, 0f, 1f);
    Color YELLOW_COLOR = new Color(.2f, .2f, 0f, 1f);
    
    enum Status { Uninitialized, Green, Red, Black };
    Status status;
    
    public override void start() {
        annulus = RezFind("suns_annulus");
        light = GameObject.Find("sunlight");
        annulus.transform.localScale = new Vector3(0.002f, 0.002f, 0.0012f);
        annulus.transform.rotation = Quaternion.Euler(0f, 0, 0);
    }
    
    public override void update() {
        if(GameDad.sol_index == -1)
            return;
        if(GameDad.whereisstar == null)
            return;
        if(GameDad.get_green == null)
            return;
        if(GameDad.is_green(GameDad.sol_index)) {
            greenstar sol = GameDad.get_green(GameDad.sol_index); //GameDad.get_green((((((((((((GameDad.sol_index);
            if(sol.type == greenstar.Type.Green) {
                updategreen();
            }
            else if(sol.type == greenstar.Type.Red) {
                updatered();
            }
        }
        else
            updateblack();
        rotate();
        /*
        bool earth_is_green = false;
        if(GameDad.sol_index == -1)
            return;
        if(GameDad.get_green == null)
            return;
        if(GameDad.is_green(GameDad.sol_index)) {
            greenstar sol = GameDad.get_green(GameDad.sol_index);
            if(sol.type == greenstar.Type.Green) {
                // earth is green
                earth_is_green = true;
                if(!started) {
                    sol.size = 0.01f;
                    time = 0f;
                    started = true;
                }
                else {
                    time += Time.deltaTime;
                    if(time > 0.2f) {
                        time -= 0.2f;
                        if(sol.size == 0.01f)
                            sol.size = 0.009f;
                        else
                            sol.size = 0.01f;
                        GameDad.update_green(GameDad.sol_index);
                    }
                }
            }
        }
        if(!earth_is_green) {
            // stop blink
            started = false;
        }
        */
    }
    
    void reset() {
        annulus.transform.position = GameDad.whereisstar(GameDad.sol_index);
        annulus.transform.rotation = Quaternion.identity;
        annulus.SetActive(false);
    }
    
    void rotate() {
        time += Time.deltaTime;
        if(time > PERIOD)
            time -= PERIOD;
        annulus.transform.rotation = Quaternion.Euler(0f, time * 360 / PERIOD, 0);
    }
    
    void updategreen() {
        if(status != Status.Green) {
            reset();
            annulus.GetComponent<MeshRenderer>().material = GameDad.shiny_green;
            annulus.SetActive(true);
            status = Status.Green;
            greenstar sol = GameDad.get_green(GameDad.sol_index);
            sol.size = 0.004f;
            GameDad.update_green(GameDad.sol_index);
            light.GetComponent<Light>().color = GREEN_COLOR;
        }
    }
    
    void updatered() {
        if(status != Status.Red) {
            reset();
            annulus.GetComponent<MeshRenderer>().material = GameDad.shiny_red;
            annulus.SetActive(true);
            status = Status.Red;
            greenstar sol = GameDad.get_green(GameDad.sol_index);
            sol.size = 0.004f;
            GameDad.update_green(GameDad.sol_index);
            light.GetComponent<Light>().color = RED_COLOR;
        }
    }
    
    void updateblack() {
        if(status != Status.Black) {
            reset();
            annulus.GetComponent<MeshRenderer>().material = GameDad.shiny_black;
            annulus.SetActive(true);
            status = Status.Black;
            light.GetComponent<Light>().color = YELLOW_COLOR;
        }
    }
}
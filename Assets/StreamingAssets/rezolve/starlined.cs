using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

// handle drawing lines, and dispatch linedrawn_hook
public class starlined: Rezolve
{
    GameObject startext; // text on the panel showing stellar class
    GameObject reticle; // the reticle
    
    bool pressed = false; // are we in space bar pressed mode?
    int pressed_star = -1; // star that was selected when space was pressed, -1 if none
    Vector3 pressed_star_position; // position of that star in world space
    GameObject line = null; // line from that star to the currently selected star or reticle
    bool line_currently_red = false;
    
    public override void start()
    {
        GameDad.starmouseover_hook = starmouseover;
        //startext = GameObject.Find("paneltext");
        reticle = GameObject.Find("reticle");
    }
    
    public override void update()
    {
        if(GameDad.setmode == null || GameDad.linedrawn_hook == null) {
            // the hooks are not set up, we should wait for the other scripts
            // setmode should come from starinstantiator
            // and linedrawn_hook should come from launch
            return;
        }
        if(Input.GetKey("space")) {
            if(pressed == false) {
                pressed = true;
                if(line != null)
                    throw new Exception("line already created even though pressed is false");
                if(pressed_star != -1)
                    throw new Exception("pressed_star != -1 even though pressed is false");
                if(GameDad.selectedStar != -1) {
                    pressed_star = GameDad.selectedStar;
                    pressed_star_position = GameDad.selectedStarLocation;
                    line = Rez(Find("line_template"));
                    line.SetActive(true);
                    line_currently_red = false;
                    GameDad.setmode(StarInstantiatorMode.AnyStarSelect);
                }
                // If there is no selected star, we still set pressed to true,
                // but we leave pressed_star at -1.
            }
            updateline();
        }
        else {
            if(pressed == true) {
                pressed = false;
                if(pressed_star != -1) {
                    // There is a line from a star.
                    if(GameDad.selectedStar != -1) {
                        // The line goes to another star.
                        if(!too_far(pressed_star_position - GameDad.selectedStarLocation)) {
                            // The line is not too long.
                            GameDad.linedrawn_hook(pressed_star,
                                           pressed_star_position,
                                           GameDad.selectedStar,
                                           GameDad.selectedStarLocation);
                        }
                    }
                }
                else {
                    // The player held the space key down without having a star selected.
                    if(line != null)
                        throw new Exception("there should be no line allocated, but there is");
                }
                pressed_star = -1;
                GameDad.setmode(StarInstantiatorMode.GreenSelect);
            }
            if(line != null)
                DeRez(line);
        }
    }
    
    // Hook to run when a star is selected
    public void starmouseover(int starix, Vector3 particlepos)
    {
        if(startext != null)
            changepanel(starix, particlepos);
    }
    
    public void changepanel(int starix, Vector3 particlepos)
    {
        var tt = startext.GetComponent<TextMesh>();
        if(starix == -1) {
            tt.text = "";
            return;
        }
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        bool first = true;
        foreach(var s in GameDad.manystars.starlist(starix)) {
            if(!first)
                sb.Append("\n");
            else
                first = false;
            string name = "";
            if(s.proper != "")
                name = s.proper;
            else if(s.hd != 0)
                name = String.Format("HD {0} in {1}", s.hd, s.constellation);
            else if(s.constellation != "")
                name = String.Format("Star in {0}", s.constellation);
            sb.Append(String.Format(
                "{0}\n{1}\n{2}\n",
                    name,
                    s.spect,
                    s.absmag
                    ));
        }
        tt.text = sb.ToString();
    }
    
    bool too_far(Vector3 diff) {
        return GameDad.realvector_to_spacevector(diff).magnitude > 4;
    }
    
    void make_line_red(LineRenderer l, bool isred) {
        if(!line_currently_red && isred) {
            l.startColor = new Color(1f, 0f, 0f, 1f);
            l.endColor = new Color(0.2f, 0f, 0f, 0.4f);
            line_currently_red = true;
        }
        else if(line_currently_red && !isred) { // test
            l.startColor = new Color(125f/255f, 1f, 0f, 1f);
            l.endColor = new Color(20f/255f, 131f/255f, 11f/255f, 127/255f);
            line_currently_red = false;
        }
    }
    
    // Function to move the gameobject "line" (called from update)
    void updateline() {
        if(line == null)
            return;
        LineRenderer l = line.GetComponent<LineRenderer>();
        l.positionCount = 2;
        Vector3 oldposition = pressed_star_position;
        Vector3 newposition =
            GameDad.selectedStar == -1?
                reticle.transform.position:
                GameDad.selectedStarLocation;
        make_line_red(l, too_far(newposition - oldposition)); // test
        Vector3[] positions = new Vector3[] {
            0.9f * oldposition + 0.1f * newposition,
            0.2f * oldposition + 0.8f * newposition
        };
        l.SetPositions(positions);
        //Debug.Log(positions[0]);
        //Debug.Log(positions[1]);
    }
};
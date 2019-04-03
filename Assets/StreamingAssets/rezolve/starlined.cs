using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class constellation_abbreviations
{
    public static Dictionary<string, string> longnames = new Dictionary<string, string>() {
        { "and", "Andromeda" },
        { "ant", "Antlia" },
        { "aps", "Apus" },
        { "aqr", "Aquarius" },
        { "aql", "Aquila" },
        { "ara", "Ara" },
        { "ari", "Aries" },
        { "aur", "Auriga" },
        { "boo", "Bootes" },
        { "cae", "Caelum" },
        { "cam", "Camelopardalis" },
        { "cnc", "Cancer" },
        { "cvn", "Canes Venatici" },
        { "cma", "Canis Major" },
        { "cmi", "Canis Minor" },
        { "cap", "Capricornus" },
        { "car", "Carina" },
        { "cas", "Cassiopeia" },
        { "cen", "Centaurus" },
        { "cep", "Cepheus" },
        { "cet", "Cetus" },
        { "cha", "Chamaeleon" },
        { "cir", "Circinus" },
        { "col", "Columba" },
        { "com", "Coma Berenices" },
        { "cra", "Corona Austrina" },
        { "crb", "Corona Borealis" },
        { "crv", "Corvus" },
        { "crt", "Crater" },
        { "cru", "Crux" },
        { "cyg", "Cygnus" },
        { "del", "Delphinus" },
        { "dor", "Dorado" },
        { "dra", "Draco" },
        { "equ", "Equuleus" },
        { "eri", "Eridanus" },
        { "for", "Fornax" },
        { "gem", "Gemini" },
        { "gru", "Grus" },
        { "her", "Hercules" },
        { "hor", "Horologium" },
        { "hya", "Hydra" },
        { "hyi", "Hydrus" },
        { "ind", "Indus" },
        { "lac", "Lacerta" },
        { "leo", "Leo" },
        { "lmi", "Leo Minor" },
        { "lep", "Lepus" },
        { "lib", "Libra" },
        { "lup", "Lupus" },
        { "lyn", "Lynx" },
        { "lyr", "Lyra" },
        { "men", "Mensa" },
        { "mic", "Microscopium" },
        { "mon", "Monoceros" },
        { "mus", "Musca" },
        { "nor", "Norma" },
        { "oct", "Octans" },
        { "oph", "Ophiuchus" },
        { "ori", "Orion" },
        { "pav", "Pavo" },
        { "peg", "Pegasus" },
        { "per", "Perseus" },
        { "phe", "Phoenix" },
        { "pic", "Pictor" },
        { "psc", "Pisces" },
        { "psa", "Piscis Austrinus" },
        { "pup", "Puppis" },
        { "pyx", "Pyxis" },
        { "ret", "Reticulum" },
        { "sge", "Sagitta" },
        { "sgr", "Sagittarius" },
        { "sco", "Scorpius" },
        { "scl", "Sculptor" },
        { "sct", "Scutum" },
        { "ser", "Serpens" },
        { "sex", "Sextans" },
        { "tau", "Taurus" },
        { "tel", "Telescopium" },
        { "tri", "Triangulum" },
        { "tra", "Triangulum Australe" },
        { "tuc", "Tucana" },
        { "uma", "Ursa Major" },
        { "umi", "Ursa Minor" },
        { "vel", "Vela" },
        { "vir", "Virgo" },
        { "vol", "Volans" },
        { "vul", "Vulpecula" },
    };
}

// handle drawing lines, and dispatch linedrawn_hook
public class starlined: Rezolve
{
    GameObject startext; // text on the panel showing stellar class
    GameObject reticle; // the reticle
    GameObject stardistance, starnames;
    
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
        stardistance = GameObject.Find("stardistance");
        starnames = GameObject.Find("starnames");
    }
    
    public override void update()
    {
        if(GameDad.setmode == null || GameDad.linedrawn_hook == null) {
            // the hooks are not set up, we should wait for the other scripts
            // setmode should come from starinstantiator
            // and linedrawn_hook should come from launch
            return;
        }
        if(Input.GetKey("space") || (GameDad.headset_button_is_pushed != null && GameDad.headset_button_is_pushed())) {
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
                    if(GameDad.is_green(pressed_star) && GameDad.get_green(pressed_star).type == greenstar.Type.Green) {
                        // The star is green.
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
        //if(startext != null)
        //Debug.Log("starmouseover");
        if(starnames != null && stardistance != null)
            changepanel(starix, particlepos);
    }
    
    public void changepanel(int starix, Vector3 particlepos)
    {
        //var tt = startext.GetComponent<TextMesh>();
        if(starix == -1) {
            starnames.GetComponent<TextMesh>().text = "";
            stardistance.GetComponent<TextMesh>().text = "";
            return;
        }
        //if(starix == -1) {
        //    tt.text = "";
        //    return;
        //}
        string proper_name = "";
        float magnitude = 999f;
        string constellation = "";
        float distance = -1f;
        int count = 0;
        foreach(var s in GameDad.manystars.starlist(starix)) {
            count++;
            if(distance == -1f)
                distance = s.vec.magnitude;
            if(s.proper != "") {
                if(s.absmag < magnitude) {
                    magnitude = s.absmag;
                    proper_name = s.proper;
                }
            }
            if(s.constellation != "")
                constellation = constellation_abbreviations.longnames[s.constellation.ToLower()];
        }
        string name = "";
        if(proper_name != "") {
            name = proper_name;
            if(count > 1) {
                name += String.Format(" + {0} OTHER{1}", count - 1, count>2? "S": "");
            }
        }
        else if(constellation != "") {
            if(count > 1)
                name = String.Format("{0} STARS IN {1}", count, constellation);
            else
                name = "STAR IN " + constellation;
        }
        else
            name = "UNKNOWN NAME";
        starnames.GetComponent<TextMesh>().text = name;
        stardistance.GetComponent<TextMesh>().text = String.Format("DISTANCE {0:00.00} PC", distance);
        
        /*
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
        starnames.GetComponent<TextMesh>().text = sb.ToString();
        */
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
        bool stillgreen = GameDad.is_green(pressed_star) &&
                                (GameDad.get_green(pressed_star).type == greenstar.Type.Green);
        make_line_red(l, !stillgreen || too_far(newposition - oldposition)); // test
        Vector3[] positions = new Vector3[] {
            0.9f * oldposition + 0.1f * newposition,
            0.2f * oldposition + 0.8f * newposition
        };
        l.SetPositions(positions);
        //Debug.Log(positions[0]);
        //Debug.Log(positions[1]);
    }
};
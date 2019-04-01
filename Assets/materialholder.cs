using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class materialholder : MonoBehaviour {
    public Material shiny_red, shiny_green, shiny_black;

	// Use this for initialization
	void Start () {
        GameDad.shiny_red = shiny_red;
        GameDad.shiny_green = shiny_green;
        GameDad.shiny_black = shiny_black;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

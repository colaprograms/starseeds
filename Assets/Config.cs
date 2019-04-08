using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Config {
    public static Color star_green = new Color(0f, 1f, 0f, 1f);
    public static Color star_redLarge = new Color(1f, 0f, 0f, 1f);
    public static Color star_redSmall = new Color(1f, 0f, 0f, 1f);
    
    public static Color starseed_green = new Color(0f, 1f, 0f, 1f);
    public static Color starseed_red = new Color(1f, 0f, 0f, 1f);
    
    public static Color game_over_text_colour = new Color(0f, 0.92f, 0f, 1f);
    
    public static Color line_green_start = new Color(125f/255f, 1f, 0f, 1f);
    public static Color line_green_end = new Color(20f/255f, 131f/255f, 11f/255f, 127/255f);
    public static Color line_red_start = new Color(1f, 0f, 0f, 1f);
    public static Color line_red_end = new Color(1f, 0f, 0f, 0.4f);
    
    public static float red_average_life = 60f; // 1 minute
    public static float starseed_speed = 1f; // 1 parsec/second
    public static float length_of_game = 300f; // 5 minutes
    
    public static float chance_of_red = 0.1f;
    public static float quiettime = 60f;
    
    public static bool red_should_be_dashed = false;
    public static float red_line_startwidth = 0.001f;
    public static float green_line_startwidth = 0f;
    
    public static Color getcolor(greenstar.StarColor c) {
        if(c == greenstar.StarColor.Green)
            return star_green;
        else if(c == greenstar.StarColor.RedLarge)
            return star_redLarge;
        else if(c == greenstar.StarColor.RedSmall)
            return star_redSmall;
        else
            throw new Exception("unknown star color");
    }
};
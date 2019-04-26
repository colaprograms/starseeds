using UnityEngine;

public class config: Rezolve
{
    public override void start() {
        your_config();
    }
    
    void your_config() {
        // Note: Unity does everything as floats, and C# wants you to write floats with an "f" on the end.
        
        Config.length_of_game = 300f; // length of game (seconds)
        Config.chance_of_red = 0.1f; // chance of red star
        Config.red_average_life = 60f; // lifetime of a red star (seconds)
        
        Config.star_green = rgb(0, 1, 0); // colour of green star
        
        // The red stars blink. You can change the colours separately for each size.
        Config.star_redLarge = rgb(1, 0, 1); // colour 1 for red star
        Config.star_redSmall = rgb(1, 0, 1); // colour 2 for red star
        
        Config.starseed_green = rgb(0, 1, 0); // colour of green starseeds
        Config.starseed_red = rgb(1, 0, 0); // colour of red starseeds
        Config.starseed_speed = 1f; // speed of a starseed (parsecs per second)
        
        Config.line_green_start = rgba(125, 255, 0, 255) / 255;
        Config.line_green_end = rgba(20, 131, 11, 127) / 255;
        Config.line_red_start = rgba(1, 0, 0, 1);
        Config.line_red_end = rgba(1, 0, 0, .4);
        
        // The dashed line looks better to me when it is slightly less tapered,
        // so the start of the red line is set to 0.001f instead of 0f.
        Config.red_should_be_dashed = true; // should the red line have dashes?
        Config.red_line_startwidth = 0.001f; // how tapered should the red line be?
        Config.green_line_startwidth = 0f; // how tapered should the green line be?
        
        // A star is quiet after it dies for a little while.
        // If you land on the star during the quiet time, it is guaranteed to be green.
        Config.quiettime = 10f; // quiet time (seconds)
    }
    
    Color rgb(double r, double g, double b) {
        return new Color((float) r, (float) g, (float) b, 1f);
    }
    
    Color rgba(double r, double g, double b, double a) {
        return new Color((float) r, (float) g, (float) b, (float) a);
    }
}

/*
    Default configuration:
    
    void your_config() {
        // Note: Unity does everything as floats, and C# wants you to write floats with an "f" on the end.
        
        Config.length_of_game = 300f; // length of game (seconds)
        Config.chance_of_red = 0.1f; // chance of red star
        Config.red_average_life = 60f; // lifetime of a red star (seconds)
        
        Config.star_green = rgb(0, 1, 0); // colour of green star
        
        // The red stars blink. You can change the colours separately for each size.
        Config.star_redLarge = rgb(1, 0, 0); // colour 1 for red star
        Config.star_redSmall = rgb(1, 0, 0); // colour 2 for red star
        
        Config.starseed_green = rgb(0, 1, 0); // colour of green starseeds
        Config.starseed_red = rgb(1, 0, 0); // colour of red starseeds
        Config.starseed_speed = 1f; // speed of a starseed (parsecs per second)
        
        Config.line_green_start = rgba(125, 255, 0, 255) / 255;
        Config.line_green_end = rgba(20, 131, 11, 127) / 255;
        Config.line_red_start = rgba(1, 0, 0, 1);
        Config.line_red_end = rgba(1, 0, 0, .4);
        
        // The dashed line looks better to me when it is slightly less tapered,
        // so the start of the red line is set to 0.001f instead of 0f.
        Config.red_should_be_dashed = true; // should the red line have dashes?
        Config.red_line_startwidth = 0.001f; // how tapered should the red line be?
        Config.green_line_startwidth = 0f; // how tapered should the green line be?
        
        // A star is quiet after it dies for a little while.
        // If you land on the star during the quiet time, it is guaranteed to be green.
        Config.quiettime = 10f; // quiet time (seconds)
    }
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum StarInstantiatorMode {
    NoSelect, GreenSelect, NonGreenSelect, AnyStarSelect
}

public static class GameDadExtensions {
    public static string ToLongString(this Vector3 what) {
        return String.Format("({0:0.0000}, {1:0.0000}, {2:0.0000})",
                             what.x,
                             what.y,
                             what.z);
    }
    
    public static T[] ToArray<T>(this ICollection<T> z) {
        T[] a = new T[z.Count];
        z.CopyTo(a, 0);
        return a;
    }
};

public class GameDad {
    public static int selectedStar = -1;
    public static Vector3 selectedStarLocation;
    public static starmap manystars;
    public static int starprogress = 0;
    
    public static int sol_index = -1;
    
    public static bool gamesover = false;
    
    public static Action<int, Vector3> starmouseover_hook = null;
    public static Action<int, Vector3, int, Vector3> linedrawn_hook = null;
    public static Action<int, int> send_red_hook = null;
    public static Func<int, Vector3> whereisstar = null;
    
    public static Func<Vector3, Vector3> realvector_to_spacevector = null;
    
    public static Func<int, bool> star_corresponds_to_particle = null;
    
    public static Action<StarInstantiatorMode> setmode = null;
    public static Action<int, greenstar> add_green = null;
    public static Action<int> remove_green = null;
    public static Func<int, bool> is_green = null;
    
    public static Action<int> update_green = null;
    public static Func<int, greenstar> get_green = null;
    public static Func<int, Vector3, int, Vector3, bool> give_chance_to_make_red_star = null;
    public static Action<int, Vector3> spamseed_lands_on_star = null;
    public static Action<int> red_star_evaporate = null;
    
    public static Action update_best = null;
    public static Func<float> farthest_green_star = null;
    public static Func<bool> headset_button_is_pushed = null;
    public static Func<bool> are_there_green_starseeds = null;
    
    public static Material shiny_red, shiny_green, shiny_black;
    
    public static Func<float> get_particle_system_scale = null;
    public static Func<Vector3> get_particle_system_position = null;
    public static Action update_particle_system_tf = null;
}

public class greenstar {
    public enum Type {
        Green, Red, Quiet
    };
    public enum StarColor {
        Green, RedLarge, RedSmall
    };
    
    public Type type = Type.Green;
    
    public float size = float.NaN;
    public StarColor color = StarColor.Green;
}
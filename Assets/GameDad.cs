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
};

public class GameDad {
    public static int selectedStar = -1;
    public static Vector3 selectedStarLocation;
    public static starmap manystars;
    public static int starprogress = 0;
    
    public static Action<int, Vector3> starmouseover_hook = null;
    public static Action<int, Vector3, int, Vector3> linedrawn_hook = null;
    
    public static Action<StarInstantiatorMode> setmode = null;
    public static Action<int> add_green = null;
    public static Action<int> add_red = null;
    public static Action<int> remove_green = null;
    public static Func<int, bool> is_green = null;
}

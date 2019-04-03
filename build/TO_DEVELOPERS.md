If you also want to write a program for the headset, check out the Rezolve System in rezz.cs.  
You can put cs files in Assets\StreamingAssets\rezolve and they will be compiled and run automatically as the game is playing!
Even better, the objects created by the old files will be automatically deallocated, if you create all your objects with Rez or RezFind.

A simple example of a Rezolve object:

    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class i_love_sphere: Rezolve {
        GameObject largesphere, smallsphere;
        
        public override void start() {
            /* Rez copies a game object and associates it with this Rezolve object.
             * But GameObject.Find can't find objects that are inactive.
             *
             * Note that Rez is an instance method. If you want to call it from
             * another class, then pass it as a callback or something similar. */
            smallsphere = Rez(GameObject.Find("small_sphere"));
            
            /* Find finds a child of the rezolve object, even if it is inactive.
             * You can also call RezFind() instead of Rez(Find()). */
            largesphere = Rez(Find("large_sphere"));
            largesphere.SetActive(true);
        }

        public override void update() {
            if(sphere == null)
                return;
            if(Input.GetKey("e")) {
                /* DeRez destroys a game object that has been Rez()ed by this object. */
                DeRez(sphere);
                sphere = null;
                DeRez(smallsphere);
                smallsphere = null;
            }
        }
    }

NOTE that code in one Rezolve file can't see the code in another Rezolve file, although it can see all the classes in the non-dynamic Unity code. I suggest using callbacks.

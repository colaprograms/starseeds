# starseeds

This small AR game was written for the Meta 2 headset by Hannah Cairns <hannah.abigail.cairns@gmail.com>.

Send little green ships from stars you control to ones that you don't, and then build new launchers. Try to get as far from Earth as possible. Avoid getting a virus.

It is not finished. See issues for status.

Note: If you also want to play with the headset, check out the dynamic recompilation system in rezz.cs.
You can put cs files in Assets\StreamingAssets\rezolve and they will be compiled and run automatically as the game is playing!
An example empty file would be
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class i_love_sphere: Rezolve {
        GameObject sphere;
        GameObject smallSphere;
        
        public override void start() {
            /* Rez copies a game object and associates it with this Rezolve object.
             * But GameObject.Find can't find objects that are inactive. */
            smallsphere = Rez(GameObject.Find("small_sphere_for_i_love_sphere"));
            /* Find finds a child of the rezolve object, even if it is inactive.
             * You can also call RezFind() instead of Rez(Find()). */
            sphere = Rez(Find("sphere_for_i_love_sphere"));
            sphere.SetActive(true);
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

## Gameplay

#### Controls

1. Select a green star with the chevron that is stuck to your head.
2. Hit the space bar, and select another star.
3. Let go of the space bar. A little green dot will travel slowly to the new star.
4. Repeat!

#### I want to see a screenshot!

Okay.

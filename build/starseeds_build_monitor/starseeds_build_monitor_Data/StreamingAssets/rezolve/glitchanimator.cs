using System;
using UnityEngine;

class glitchanimator: Rezolve { // test
    float time = 0.0f;
    Material glitchmaterial;
    
    public override void start() {
        glitchmaterial = Resources.Load("glitchmaterial") as Material;
    }
    
    public override void update() { // test
        time += Time.deltaTime;
        glitchmaterial.SetVector("_DisplacementAmount",
            new Vector4(0.01f * (float) Math.Sin(time), 0.0f, 0.0f, 0.01f)
            );
        glitchmaterial.SetFloat(
            "_WavyDisplFreq", 20 + 10.0f * (float) Math.Sin(time));
    }
}
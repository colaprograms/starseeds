using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class annulus_generator : ProgrammableMesh {
    public int vertexCount;
    public float outRadius, inRadius, height;
    public string mesh_name = "anonymous annulus";
    
    override public string meshname() {
        return mesh_name;
    }
    
    override public void CreateMesh() {
        initialize(vertexCount * 4);
        int OUT_TOP = 0,
            OUT_BOT = vertexCount,
            IN_TOP = vertexCount * 2,
            IN_BOT = vertexCount * 3;
        for(int i = 0; i < vertexCount; i++) {
            float t = i * 2 * Mathf.PI / vertexCount;
            float s = Mathf.Sin(t);
            float c = Mathf.Cos(t);
            V(OUT_TOP + i, outRadius * s, outRadius * c, height/2f);
            V(OUT_BOT + i, outRadius * s, outRadius * c, -height/2f);
            V(IN_TOP + i, inRadius * s, inRadius * c, height/2f);
            V(IN_BOT + i, inRadius * s, inRadius * c, -height/2f);
            
            int next = (i + 1) % vertexCount;
            T(OUT_TOP + i, OUT_TOP + next, OUT_BOT + i);
            T(OUT_TOP + next, OUT_BOT + next, OUT_BOT + i);
            T(OUT_TOP + i, IN_TOP + i, IN_TOP + next);
            T(IN_TOP + next, OUT_TOP + next, OUT_TOP + i);
            Trev(OUT_BOT + i, IN_BOT + i, IN_BOT + next);
            Trev(IN_BOT + next, OUT_BOT + next, OUT_BOT + i);
            Trev(IN_TOP + i, IN_TOP + next, IN_BOT + i);
            Trev(IN_TOP + next, IN_BOT + next, IN_BOT + i);
        }
    }
}
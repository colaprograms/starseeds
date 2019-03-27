using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class programmablemesh_launcher : ProgrammableMesh {
    public float size;
    
    override public void CreateMesh() {
        initialize(18);
        Vector3[] vertices = {
            new Vector3(-size, -size, 0),
            new Vector3(-size, size, 0),
            new Vector3(size, size, 0),
            new Vector3(size, -size, 0),
            new Vector3(0, 0, size*2)
        };
        Action<int, int, int, int> Tsharp = delegate(int o, int i, int j, int k) {
            V(o, vertices[i]);
            V(o+1, vertices[j]);
            V(o+2, vertices[k]);
            T(o, o+1, o+2);
        };
        Tsharp(0, 0, 1, 2);
        Tsharp(3, 0, 2, 3);
        Tsharp(6, 4, 1, 0);
        Tsharp(9, 4, 2, 1);
        Tsharp(12, 4, 3, 2);
        Tsharp(15, 4, 0, 3);
    }
    
    override public string meshname() {
        return "launcher model";
    }
}
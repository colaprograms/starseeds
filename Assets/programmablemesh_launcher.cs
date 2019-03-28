using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class programmablemesh_launcher : ProgrammableMesh {
    public float size;
    public int sides = 4;
    
    override public void CreateMesh() {
        initialize(18 * sides);
        Vector3[] v = new Vector3[sides * 4];
        int verytop = 0;
        int topside = sides;
        int bottomside = sides * 2;
        int verybottom = sides * 3;
        for(int i = 0; i < sides; i++) {
            float angle = i * Mathf.PI * 2 / sides;
            float c = Mathf.Cos(angle), s = Mathf.Sin(angle);
            v[verytop + i] = new Vector3(c * size * 0.2f, -s * size * 0.2f, size * 1f);
            v[verybottom + i] = new Vector3(c * size * 0.2f, -s * size * 0.2f, -size * 0.505f);
            v[topside + i] = new Vector3(c * size, -s * size, size * 0.5f);
            v[bottomside + i] = new Vector3(c * size, -s * size, -size * 0.5f);
        }
        int offset = 0;
        Action<int, int, int> Tsharp = delegate(int i, int j, int k) {
            T(offset, offset + 1, offset + 2);
            V(offset++, v[i]);
            V(offset++, v[j]);
            V(offset++, v[k]);
        };
        Action<int, int, int> Tsharprev = delegate(int i, int j, int k) { Tsharp(i, k, j); };
        Action<int, int, int, int> square = delegate(int i, int j, int k, int l) {
            Tsharp(i, j, k);
            Tsharp(j, l, k);
        };
        Action<int, int, int, int> erauqs = delegate(int i, int j, int k, int l) {
            Tsharp(i, k, j);
            Tsharp(j, k, l);
        };
        for(int i = 0; i < sides; i++) {
            if((i & 3) != 3)
                continue;
            int next = (i + 1) % sides;
            square(verytop + i, verytop + next, topside + i, topside + next);
            square(topside + i, topside + next, bottomside + i, bottomside + next);
            square(bottomside + i, bottomside + next, verybottom + i, verybottom + next);
            
            erauqs(verytop + i, verytop + next, topside + i, topside + next);
            erauqs(topside + i, topside + next, bottomside + i, bottomside + next);
            erauqs(bottomside + i, bottomside + next, verybottom + i, verybottom + next);
        }
    }
    
    public void CreateMeshbak() {
        initialize(12 * sides);
        Vector3[] vertices = new Vector3[2 + sides * 2];
        vertices[0] = new Vector3(0, 0, size*3);
        vertices[1] = new Vector3(0, 0, -size*3);
        int top = 0;
        int bottom = 1;
        int topside = 2;
        int bottomside = 2 + sides;
        for(int i = 0; i < sides; i++) {
            float angle = i * Mathf.PI * 2 / sides;
            float c = Mathf.Cos(angle), s = Mathf.Sin(angle);
            vertices[topside + i] = new Vector3(c * size, -s * size, size);
            vertices[bottomside + i] = new Vector3(c * size, -s * size, -size);
        }
        int offset = 0;
        Action<int, int, int> Tsharp = delegate(int i, int j, int k) {
            T(offset, offset + 1, offset + 2);
            V(offset++, vertices[i]);
            V(offset++, vertices[j]);
            V(offset++, vertices[k]);
        };
        Action<int, int, int> Tsharprev = delegate(int i, int j, int k) { Tsharp(i, k, j); };
        for(int i = 0; i < sides; i++) {
            if((i & 1) == 0)
                continue;
            int next = (i + 1) % sides;
            Tsharp(top, topside + next, topside + i);
            Tsharp(bottom, bottomside + i, bottomside + next);
            Tsharp(topside + i, topside + next, bottomside + i);
            Tsharp(topside + next, bottomside + next, bottomside + i);
            
            Tsharprev(top, topside + next, topside + i);
            Tsharprev(bottom, bottomside + i, bottomside + next);
            Tsharprev(topside + i, topside + next, bottomside + i);
            Tsharprev(topside + next, bottomside + next, bottomside + i);
        }
        /*
        Vector3[] vertices = {
            new Vector3(-size, -size, size),
            new Vector3(-size, size, size),
            new Vector3(size, size, size),
            new Vector3(size, -size, size),
            new Vector3(-size, -size, -size),
            new Vector3(-size, size, -size),
            new Vector3(size, size, -size),
            new Vector3(size, -size, -size),
            new Vector3(0, 0, size*3),
            new Vector3(0, 0, -size*3)
        };
        int offset = 0;
        Action<int, int, int> Tsharp = delegate(int i, int j, int k) {
            T(offset, offset + 1, offset + 2);
            V(offset++, vertices[i]);
            V(offset++, vertices[j]);
            V(offset++, vertices[k]);
        };
        for(int i = 0; i < 4; i++) {
            int next = (i+1)&3;
            
            //pyramids
            Tsharp(8, next, i);
            Tsharp(9, 4 + i, 4 + next);
            
            //cube sides
            Tsharp(next, 4 + next, i);
            Tsharp(i, 4 + next, 4 + i);
        }
        /*
        Tsharp(0, 0, 1, 2);
        Tsharp(3, 0, 2, 3);
        Tsharp(6, 4, 1, 0);
        Tsharp(9, 4, 2, 1);
        Tsharp(12, 4, 3, 2);
        Tsharp(15, 4, 0, 3);*/
    }
    
    override public string meshname() {
        return "launcher model";
    }
}
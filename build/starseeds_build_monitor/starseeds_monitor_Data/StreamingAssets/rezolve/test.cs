using System;
using UnityEngine;
using System.Collections.Generic;

public class meshspec {
    Vector3[] vertices;
    List<int> tris;
    
    protected void initialize(int m) {
        vertices = new Vector3[m];
        tris = new List<int>();
    }
    
    protected void V(int i, float x, float y, float z) {
        vertices[i] = new Vector3(x, y, z);
    }
    
    protected void V(int i, Vector3 z) {
        vertices[i] = z;
    }
    
    protected void T(int i0, int i1, int i2) {
        tris.Add(i0);
        tris.Add(i1);
        tris.Add(i2);
    }
    
    protected void Trev(int i0, int i1, int i2) {
        T(i0, i2, i1);
    }
    
    public void create_mesh_on(MeshFilter mf) {
        var mesh = new Mesh();
        mesh.name = meshname();
        
        CreateMesh();
        
        mesh.vertices = vertices;
        mesh.triangles = tris.ToArray();
        Debug.Log(mesh.vertices.Length);
        Debug.Log(mesh.triangles.Length);
        mesh.RecalculateNormals(); // test
        Debug.Log(mesh.vertices[1]);
        mf.mesh = mesh;
    }
    
    virtual public string meshname() {
        throw new System.Exception("not implemented");
    }
    
    virtual public void CreateMesh() {
        initialize(0);
    }
}

public class programmablemesh_launcher_two : meshspec { // test
    public float size = 0.2f;
    public int sides = 18;
    
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
    
    override public string meshname() {
        return "launcher model";
    }
}

class test: Rezolve {
    float time = 0f;
    
    public override void start() {
        /*
        var x = new programmablemesh_launcher_two(); // test
        x.create_mesh_on(GameObject.Find("launcher_model").GetComponent<MeshFilter>());
        */
        //sphere = Rez(GameObject.Find("Sphere"));
        //sphere.GetComponent<Renderer>().material = Resources.Load("glitchmaterial") as Material;
        //sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        //sphere.transform.position = new Vector3(0.0f, -1.2f, 0.4f);
    }
    
    public override void update() {
        //if(GameDad.selectedStar != -1) {
        //    Debug.Log(GameDad.manystars.getstar(GameDad.selectedStar).vec.ToLongString());
        //}
        //time += Time.deltaTime;
        //Find("launcher_model").transform.rotation = Quaternion.AngleAxis(30*time, Vector3.up) * Quaternion.AngleAxis(90, Vector3.left);
        //time += Time.deltaTime;
        //sphere.transform.position = new Vector3(0.02f * (float) Math.Sin(time),
        //    -1.2f + 0.02f * (float) Math.Cos(time), 0.4f);
    }
}
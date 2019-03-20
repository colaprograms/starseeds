using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgrammableMesh : MonoBehaviour {
    Vector3[] vertices;
    List<int> tris;
    
    protected void initialize(int m) {
        vertices = new Vector3[m];
        tris = new List<int>();
    }
    
    protected void V(int i, float x, float y, float z) {
        vertices[i] = new Vector3(x, y, z);
    }
    
    protected void T(int i0, int i1, int i2) {
        tris.Add(i0);
        tris.Add(i1);
        tris.Add(i2);
    }
    
    protected void Trev(int i0, int i1, int i2) {
        T(i0, i2, i1);
    }
    
    [ContextMenu("Create mesh")]
    void Create() {
        var mf = GetComponent<MeshFilter>();
        var mesh = new Mesh();
        mesh.name = "cylindrical annulus";
        
        CreateMesh();
        
        mesh.vertices = vertices;
        mesh.triangles = tris.ToArray();
        mf.mesh = mesh;
        Debug.Log("run");
    }
    
    virtual public void CreateMesh() {
        initialize(0);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
using UnityEngine;

public class linedrawer: Rezolve
{
    GameObject line;
    
    public override void start() {
        /*
        line = Rez(Find("line_template"));
        line.gameObject.SetActive(true);
        line.transform.localPosition = new Vector3(0f, -0.3f, 0.4f);
        
        var linerenderer = line.GetComponent<LineRenderer>();
        Vector3[] positions = new Vector3[2];
        positions[0] = new Vector3(0.1f, 0.0f, 0.0f);
        positions[1] = new Vector3(0.0f, 0.4f, 0.0f);
        linerenderer.positionCount = positions.Length;
        linerenderer.SetPositions(positions);
        */
    }
    
    public override void update() {
    }
}
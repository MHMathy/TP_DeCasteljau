using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public Camera camera;
    
    private Plane plane;
    static Material lineMaterial;
    private Line[] axis;
    private List<Line> lineArray;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        lineArray = new List<Line>();
        lineArray.Add(new Line(Color.black));

        axis = new Line[2];
        axis[0] = new Line(Color.red,new Vector3(-1,0,0),new Vector3(1,0,0));
        axis[1] = new Line(Color.green,new Vector3(0,-1,0),new Vector3(0,1,0));

        plane = new Plane(new Vector3(0, 0, -1), 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            float enter = 0f;
            Vector3 position;
            plane.Raycast(ray, out enter);
            position = ray.GetPoint(enter);
            Debug.Log(position);
            lineArray[0].add(position);
        }
    }
    
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        CreateLineMaterial();
        // Apply the line material
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        //GL.MultMatrix(transform.localToWorldMatrix);

        // Draw lines
        GL.Begin(GL.LINES);
        axis[0].drawLine2D();
        axis[1].drawLine2D();
        lineArray[0].drawLine2D();


        GL.End();
        GL.PopMatrix();
    }
}

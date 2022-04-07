using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;
using Plane = UnityEngine.Plane;
using Vector3 = UnityEngine.Vector3;

public class Example : MonoBehaviour
{
    // When added to an object, draws colored rays from the
    // transform position.
    public int lineCount = 100;
    public float radius = 3.0f;
    public Camera camera;
    
    private Plane plane;
    static Material lineMaterial;
    private Line mainLine;
    private Line XAxis;
    private Line YAxis;
    
    // Line struct
    class Line
    {
        // All points of the line
        private List<Vector3> points;
        // The line color
        private Color color;
        public Line(Color col)
        {
            points = new List<Vector3>();
            color = col;
        }
        public Line(Color col,Vector3 p0, Vector3 p1)
        {
            points = new List<Vector3>();
            add(p0);
            add(p1);
            color = col;
        }

        public void add(float x, float y, float z)
        {
            points.Add(new Vector3(x, y, z));
        }
        public void add(Vector3 p)
        {
            points.Add(p);
        }
        public void add(Vector3[] tab)
        {
            foreach (var p in tab)
            {
                points.Add(p);
            }
        }
        
        // Draw the line
        public void drawLine2D()
        {
            if (points.Count >= 2)
            {
                GL.Color(color);
                for(int i = 0; i < points.Count - 1; i++)
                {
                    GL.Vertex3(points[i].x,points[i].y,points[i].z);
                    GL.Vertex3(points[i+1].x,points[i+1].y,points[i+1].z);
                }
            }
            
        }
    }
    
    public void Start()
    {
        mainLine = new Line(Color.black);
        XAxis = new Line(Color.red);
        XAxis.add(-1,0,0);
        XAxis.add(1,0,0);
        
        YAxis = new Line(Color.green);
        YAxis.add(0,-1,0);
        YAxis.add(0,1,0);

        plane = new Plane(new Vector3(0, 0, -1), 0);
    }   

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            float enter = 0f;
            Vector3 position;
            plane.Raycast(ray, out enter);
            position = ray.GetPoint(enter);
            Debug.Log(position);
            mainLine.add(position);
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
        XAxis.drawLine2D();
        YAxis.drawLine2D();
        mainLine.drawLine2D();


        GL.End();
        GL.PopMatrix();
    }
}

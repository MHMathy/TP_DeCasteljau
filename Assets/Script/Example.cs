using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
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
    public float pas;
    
    private Plane plane;
    static Material lineMaterial;
    private Line mainLine;
    private int IndexlastPoint;
    Line temp = new Line(Color.red);
    Line BezierLine = new Line(Color.blue);
    private Line XAxis;
    private Line YAxis;
    private float t = 0;
    
    // Line struct
    class Line
    {
        // All points of the line
        public List<Vector3> points;
        // The line color
        public Color color;
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

        public List<Vector3> fillList(List<Vector3> target ,List<Vector3> source)
        {
            for (int i = 0; i < source.Count; i++)
            {
                target.Add(source[i]);
            }

            return target;
        }

        public Vector3 getPoint(int index)
        {
            return this.points[index];
        }

        public int getPointSize()
        {
            return this.points.Count;
        }
        
        public Color getColor()
        {
            return this.color;
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
            
            IndexlastPoint = mainLine.getPointSize();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            for (float i = 0; i <= 1; i += pas )
            {
                
                BezierLine.points.Add(CalculateBezier(i));
            }
            
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
        BezierLine.drawLine2D();

        GL.End();
        GL.PopMatrix();
    }

    // on calcul un point de la bezier
    public Vector3 CalculateBezier(float t)
    {
        List<Vector3> bezierPoint = new List<Vector3>();

        for (int level = mainLine.points.Count - 1; level >= 0; level--)
        {
            if (level == mainLine.points.Count - 1)
            {
                for (int i = 0; i <= mainLine.points.Count - 1; i++)
                {
                    bezierPoint.Add(mainLine.points[i]);
                }

                continue;
            }
            int lastIndex = bezierPoint.Count;
            int levelIndex = level + 2;
            int index = lastIndex - levelIndex;
            for (int i = 0; i <= level; i++)
            {
                Vector3 Point = (1 - t) * bezierPoint[index] + t * bezierPoint[index + 1];
                bezierPoint.Add(Point);
                ++index;
            }
        }
        int lastElem = bezierPoint.Count -1;
        return bezierPoint[lastElem];
    }
}

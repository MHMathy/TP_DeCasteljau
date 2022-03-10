using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Example : MonoBehaviour
{
    // When added to an object, draws colored rays from the
    // transform position.
    public int lineCount = 100;
    public float radius = 3.0f;

    static Material lineMaterial;
    

    struct Line
    {
        public Vector3[] points;
        public Color color;

        public Line(int nbPoints, Color col)
        {
            points = new Vector3[nbPoints];
            color = col;
        }
        public void drawLine()
        {
            GL.Color(color);
            for (int i = 0; i < points.Length - 1; i++)
            {
                GL.Vertex3(points[i].x,points[i].y,points[i].z);
                GL.Vertex3(points[i+1].x,points[i+1].y,points[i+1].z);
            }
        }
    }
    
    private Line mainLine;
    
    public void Start()
    {
        mainLine = new Line(6,Color.red);
        mainLine.points[0]= new Vector3(0,0,0);
        mainLine.points[1]= new Vector3(1,1,0);
        mainLine.points[2]= new Vector3(2,1,0);
        mainLine.points[3]= new Vector3(3,3,0);
        mainLine.points[4]= new Vector3(4,2,0);
        mainLine.points[5]= new Vector3(5,1,0);
        
        mainLine.color = Color.red;
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
        
        mainLine.drawLine();


        GL.End();
        GL.PopMatrix();
    }
}

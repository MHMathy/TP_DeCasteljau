using System.Collections.Generic;
using UnityEngine;

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
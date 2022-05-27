using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bezier : MonoBehaviour
{
    public GameObject pointPrefab;
    public Camera cam;
    public Plane plane;

    private List<Vector3> PosPointExterne = new List<Vector3>();
    private List<Vector3> PosPointBezier = new List<Vector3>();
    public float pas;
    
    
    
    private LineRenderer line;
    private int curveCount = 0;
    private int SEGMENT_COUNT = 50;
    
    
    private void Start()
    {
        plane = new Plane(new Vector3(0, 0, 0.2f),0);
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            float enter = 0f;
            Vector3 position;
            plane.Raycast(ray, out enter);
            position = ray.GetPoint(enter);
            Debug.Log(position);
            Instantiate(pointPrefab,position,Quaternion.identity);
            PosPointExterne.Add(position);
            curveCount = PosPointExterne.Count ;
            line.positionCount = PosPointExterne.Count;
        }
        
        if (Input.GetKeyUp(KeyCode.Z))
        {
            if (PosPointExterne.Count > 1)
            {
                for (int i = 0; i < PosPointExterne.Count; i++)
                {
                    line.SetPosition(i,PosPointExterne[i]);
                }
                
            }
        } 

        if (Input.GetKeyDown(KeyCode.A))
        {
            for (float i = 0; i <= 1; i += pas )
            {
                PosPointBezier.Add(CalculateBezier(i));
            }
            line.positionCount += PosPointBezier.Count;
        }
        
        if (Input.GetKeyUp(KeyCode.A))
        {
            line.SetVertexCount(PosPointBezier.Count);
            line.SetPositions(PosPointBezier.ToArray());
            
        }
    }
    
    
    // on calcul un point de la bezier
    public Vector3 CalculateBezier(float t)
    {
        List<Vector3> bezierPoint = new List<Vector3>();

        for (int level = PosPointExterne.Count - 1; level >= 0; level--)
        {
            if (level == PosPointExterne.Count - 1)
            {
                for (int i = 0; i <= PosPointExterne.Count - 1; i++)
                {
                    bezierPoint.Add(PosPointExterne[i]);
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

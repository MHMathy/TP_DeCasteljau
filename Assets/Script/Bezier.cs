using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Plane = UnityEngine.Plane;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;


public class Bezier : MonoBehaviour
{
    // When added to an object, draws colored rays from the
    // transform position.
    public int lineCount = 100;
    public float radius = 3.0f;
    public Camera camera;
    public float pas;
    
    private Plane plane;
    static Material lineMaterial;


    private List<GameObject> GameobjectList = new List<GameObject>();
    private List<GameObject> BezierList = new List<GameObject>();

    private List<GameObject> GameobjectList2 = new List<GameObject>();
    private List<GameObject> BezierList2 = new List<GameObject>();
    
    private List<GameObject> raccordList = new List<GameObject>();
    private List<GameObject> raccordBezierList = new List<GameObject>();

    private List<GameObject> EnveloppeConvexe1 = new List<GameObject>();
    private List<GameObject> EnveloppeConvexe2 = new List<GameObject>();
    private List<GameObject> BezierEnveloppeConvexe1 = new List<GameObject>();
    private List<GameObject> BezierEnveloppeConvexe2 = new List<GameObject>();
    private float t = 0;

    private List<List<GameObject>> ListeCourbe = new List<List<GameObject>>();
    
    public GameObject pointPrefab;
    public GameObject pointPrefab2;
    public GameObject pointBezier;

    public Material green;
    public Material red;

    private GameObject selectedObject = null;
    public GameObject courbe;
    private GameObject child;
    public Canvas ui;
    public Text pasValue;
    private Slider slider;
    public void Start()
    {
 
        plane = new Plane(new Vector3(0, 0, -1), 0);
        slider = ui.GetComponentInChildren<Slider>();
        pasValue.text = pas.ToString();
        slider.value = pas;
    }   

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) )
        {
            DessinerPoints(GameobjectList,pointPrefab);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            DessinerPoints(GameobjectList2,pointPrefab2);
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            GameObject courbe = GameObject.Find("Courbe Exterieur");
            GameObject bezier = GameObject.Find("Courbe Bezier");
            GameobjectList.Clear();
            BezierList.Clear();
            Destroy(courbe);
            Destroy(bezier);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            GameObject courbe = GameObject.Find("Courbe Exterieur 2");
            GameObject bezier = GameObject.Find("Courbe Bezier 2");
            GameobjectList2.Clear();
            BezierList2.Clear();
            Destroy(courbe);
            Destroy(bezier);
        }
        
        if (Input.GetKeyDown(KeyCode.I))
        {
            GameObject courbe = GameObject.Find("Concat");
            GameObject bezier = GameObject.Find("Concat Bezier");
            raccordList.Clear();
            raccordBezierList.Clear();
            Destroy(courbe);
            Destroy(bezier);
        }

        if (Input.GetMouseButtonDown(1))
        {
            DeletePoint();
        }
        pas = slider.value;
        pasValue.text = slider.value.ToString();
    }
    
    public void Draw()
    {
        // on dessine la courbe externe
        if (GameObject.Find("Courbe Exterieur"))
        {
            GameObject.Destroy(GameObject.Find("Courbe Exterieur"));
        }
        GameObject courbeExt = Instantiate(courbe);
        Transform child;
        courbeExt.name = "Courbe Exterieur";
        for (int i = 0; i < GameobjectList.Count; i++)
        {
            GameobjectList[i].transform.SetParent(courbeExt.transform);
            Relier(GameobjectList);
        }
        //on dessine la bezier
        if (GameObject.Find("Courbe Bezier"))
        {
            GameObject.Destroy(GameObject.Find("Courbe Bezier"));
        }
        GameObject courbeBez = Instantiate(courbe);
        courbeBez.name = "Courbe Bezier"; 
        BezierList.Clear();
        for (float i = 0; i <= 1; i += pas )
        {
            Vector3 position = CalculateBezier(i, GameobjectList);
            GameObject pointBez =Instantiate(pointBezier,position,Quaternion.identity);
            pointBez.transform.SetParent(courbeBez.transform);
            BezierList.Add(pointBez);
        }
        if (GameObject.Find("Courbe Bezier")!= null)
            Relier(BezierList);
        
    }
    
    
    public void Draw2()
    {
        //dessiner la deuxieme courbe
        GameObject courbeExt = Instantiate(courbe);
        Transform child;
        courbeExt.name = "Courbe Exterieur 2";
        for (int i = 0; i < GameobjectList2.Count; i++)
        {
            GameobjectList2[i].transform.SetParent(courbeExt.transform);
            Relier(GameobjectList2);
        }
        
        // Dessiner la deuxieme Bezier
        if (GameObject.Find("Courbe Bezier 2"))
        {
            GameObject.Destroy(GameObject.Find("Courbe Bezier 2"));
        }
        GameObject courbeBez = Instantiate(courbe);
        courbeBez.name = "Courbe Bezier 2"; 
        BezierList2.Clear();
        for (float i = 0; i <= 1; i += pas )
        {
            ListeCourbe.Add(GameobjectList);
            Vector3 position = CalculateBezier(i, GameobjectList2);
            GameObject pointBez =Instantiate(pointBezier,position,Quaternion.identity);
            pointBez.transform.SetParent(courbeBez.transform);
            BezierList2.Add(pointBez);
        }
        if (GameObject.Find("Courbe Bezier")!= null)
            Relier(BezierList2);
        
    }
    
    void DessinerPoints(List<GameObject> lineExt, GameObject gameObject)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        float enter = 0f;
        Vector3 position;
        plane.Raycast(ray, out enter);
        position = ray.GetPoint(enter);
        GameObject point =Instantiate(gameObject,position,Quaternion.identity);
        lineExt.Add(point);
    }

    public void Relier(List<GameObject> list)
    {
        if (list.Count > 1)
        {
            for (int i = 0; i < list.Count - 1; i++)
            {
                
                if (!list[i].GetComponent<LineRenderer>())
                {
                    Vector3 nextPoint;
                    if (i == list.Count - 1 && list.Count > 2)
                    {
                        nextPoint = list[0].transform.position;
                    }
                    else if (list.Count == 2)
                    {
                        break;
                    }
                    else
                    { 
                        nextPoint = list[i + 1].transform.position;
                    }

                    LineRenderer _lineRenderer = list[i].AddComponent<LineRenderer>();
                    _lineRenderer.startWidth = 0.1f;
                    _lineRenderer.endWidth = 0.1f;
                    _lineRenderer.positionCount = 2;
                    _lineRenderer.SetPosition(0, list[i].transform.position);
                    _lineRenderer.SetPosition(1,nextPoint);

                }
                else
                {
                    Vector3 nextPoint;
                    if (i == list.Count - 1 && list.Count > 2)
                    {
                        nextPoint = list[0].transform.position;
                    }
                    else if (list.Count == 2)
                    {
                        break;
                    }
                    else
                    {
                        nextPoint = list[i + 1].transform.position;
                    }

                    LineRenderer _lineRenderer = list[i].GetComponent<LineRenderer>();
                    _lineRenderer.SetPosition(0, list[i].transform.position);
                    _lineRenderer.SetPosition(1,nextPoint);
                }
            }
        }
    }
    

    // on calcul un point de la bezier
    public Vector3 CalculateBezier(float t, List<GameObject> CurrentLine)
    {
        List<Vector3> bezierPoint = new List<Vector3>();

        for (int level = CurrentLine.Count - 1; level >= 0; level--)
        {
            if (level == CurrentLine.Count - 1)
            {
                for (int i = 0; i <= CurrentLine.Count - 1; i++)
                {
                    bezierPoint.Add(CurrentLine[i]. transform.position);
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

    public void DeletePoint()
    {

        if (selectedObject == null)
        {
            RaycastHit hit = castRay();
            if (hit.collider != null)
            {
                if (!hit.collider.CompareTag("cube"))
                {
                    return;
                }

                selectedObject = hit.collider.gameObject;
                Transform tmp = selectedObject.transform.parent;
                if (tmp.name == "Courbe Exterieur")
                {
                    GameobjectList.Remove(selectedObject);
                    Destroy(selectedObject);
                    selectedObject = null; 
                } else if (tmp.name == "Courbe Exterieur 2")
                {
                    GameobjectList2.Remove(selectedObject);
                    Destroy(selectedObject);
                    selectedObject = null; 
                }
            }
        }

    }

    private RaycastHit castRay()
    {
        Vector3 screenMousePosFar = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane);
        Vector3 screenMousePosNear = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.nearClipPlane);
        Vector3 worldMousPosfar = Camera.main.ScreenToWorldPoint(screenMousePosFar);
        Vector3 worldMousPosNear = Camera.main.ScreenToWorldPoint(screenMousePosNear);
        RaycastHit hit;
        Physics.Raycast(worldMousPosNear, worldMousPosfar - worldMousPosNear, out hit);
        return hit;
    }


    public void RaccordC0()
    {
        raccordList = GameobjectList.Concat(GameobjectList2).ToList();
        
        GameObject courbeExt = Instantiate(courbe);
        Transform child;
        courbeExt.name = "Concat";
        for (int i = 0; i < raccordList.Count; i++)
        {
            raccordList[i].transform.SetParent(courbeExt.transform);
            Relier(raccordList);
        }
        
        // Dessiner la deuxieme Bezier
        if (GameObject.Find("Concat Bezier"))
        {
            GameObject.Destroy(GameObject.Find("Concat Bezier"));
        }
        GameObject courbeBez = Instantiate(courbe);
        courbeBez.name = "Concat Bezier"; 
        raccordBezierList.Clear();
        for (float i = 0; i <= 1; i += pas )
        {
            Vector3 position = CalculateBezier(i, raccordList);
            GameObject pointBez =Instantiate(pointBezier,position,Quaternion.identity);
            pointBez.transform.SetParent(courbeBez.transform);
            raccordBezierList.Add(pointBez);
        }
        if (GameObject.Find("Concat Bezier")!= null)
            Relier(raccordBezierList);
        
    }
    
    public List<GameObject> Jarvis(List<GameObject> liste)
    {
        List<GameObject> resultat = new List<GameObject>();
        int indexMostLeftPoint = 0;

        for (int i = 0; i < liste.Count; i++)
        {
            if (liste[indexMostLeftPoint].transform.position.x > liste[i].transform.position.x)
            {
                indexMostLeftPoint = i;
            }
        }
        resultat.Add(liste[indexMostLeftPoint]);
        
        List<GameObject> pointColineaire = new List<GameObject>();
        GameObject pointActuel = liste[indexMostLeftPoint];

        while (true)
        {
            GameObject pointSuivant = liste[0];
            for (int i = 1; i < liste.Count; i++)
            {
                if (liste[i] == pointActuel)
                    continue;
                float x1, x2, y1, y2;
                x1 = pointActuel.transform.position.x - pointSuivant.transform.position.x;
                x2 = pointActuel.transform.position.x - liste[i].transform.position.x;

                y1 = pointActuel.transform.position.y - pointSuivant.transform.position.y;
                y2 = pointActuel.transform.position.y - liste[i].transform.position.y;

                float val = (y2 * x1) - (y1 * x2);
                if (val > 0)
                {
                    pointSuivant = liste[i];
                    pointColineaire = new List<GameObject>();
                }else if (val == 0 )
                {
                    if (Vector3.Distance(pointActuel.transform.position, pointSuivant.transform.position) < Vector3.Distance(pointActuel.transform.position, liste[i].transform.position))
                    {
                        pointColineaire.Add(pointSuivant);
                        pointSuivant = liste[i];
                    }
                    else
                        pointColineaire.Add(liste[i]);    
                }
            }
            
            foreach (GameObject t in pointColineaire)
                resultat.Add(t);            
            if (pointSuivant == liste[indexMostLeftPoint])
                break;
            resultat.Add(pointSuivant);
            pointActuel = pointSuivant;
        }
        resultat.Add(resultat[0]);
        return resultat;
    }
    
    public void Enveloppe1()
    {
       EnveloppeConvexe1= Jarvis(GameobjectList);
       
       GameObject courbeExt = Instantiate(courbe);
       courbeExt.name = "Enveloppe1";
       for (int i = 0; i < EnveloppeConvexe1.Count; i++)
       {
           EnveloppeConvexe1[i].transform.SetParent(courbeExt.transform);
           Relier(EnveloppeConvexe1);
       }
       
       if (GameObject.Find("Courbe Bezier Enveloppe"))
       {
           GameObject.Destroy(GameObject.Find("Courbe Bezier Enveloppe"));
       }
       GameObject courbeBez = Instantiate(courbe);
       courbeBez.name = "Courbe Bezier Enveloppe"; 
       BezierEnveloppeConvexe1.Clear();
       for (float i = 0; i <= 1; i += pas )
       {
           Vector3 position = CalculateBezier(i, EnveloppeConvexe1);
           GameObject pointBez =Instantiate(pointBezier,position,Quaternion.identity);
           pointBez.transform.SetParent(courbeBez.transform);
           BezierEnveloppeConvexe1.Add(pointBez);
       }
       if (GameObject.Find("Courbe Bezier Enveloppe")!= null)
           Relier(BezierEnveloppeConvexe1);
       
    }
    
    public void Enveloppe2()
    {
       EnveloppeConvexe2 = Jarvis(GameobjectList2);
       GameObject courbeExt = Instantiate(courbe);
       courbeExt.name = "Enveloppe2";
       for (int i = 0; i < EnveloppeConvexe2.Count; i++)
       {
           EnveloppeConvexe2[i].transform.SetParent(courbeExt.transform);
           Relier(EnveloppeConvexe2);
       }
       
       if (GameObject.Find("Courbe Bezier Enveloppe2"))
       {
           GameObject.Destroy(GameObject.Find("Courbe Bezier Enveloppe2"));
       }
       GameObject courbeBez = Instantiate(courbe);
       courbeBez.name = "Courbe Bezier Enveloppe2"; 
       BezierEnveloppeConvexe2.Clear();
       for (float i = 0; i <= 1; i += pas )
       {
           Vector3 position = CalculateBezier(i, EnveloppeConvexe2);
           GameObject pointBez =Instantiate(pointBezier,position,Quaternion.identity);
           pointBez.transform.SetParent(courbeBez.transform);
           BezierEnveloppeConvexe2.Add(pointBez);
       }
       if (GameObject.Find("Courbe Bezier Enveloppe2")!= null)
           Relier(BezierEnveloppeConvexe2);
    }

    public void testIntersection()
    {
        Vector3 posIntersection = Vector3.zero;
        
        
        for (int i = 0; i < EnveloppeConvexe1.Count-1; i++)
        {
            GameObject PointActuelPoly1 = EnveloppeConvexe1[i];
            GameObject PointSuivantPoly1 = EnveloppeConvexe1[i+1];

            for (int j = 0; j < EnveloppeConvexe2.Count-1; j++)
            {
               
                var resVal = Intersection(PointActuelPoly1, PointSuivantPoly1, EnveloppeConvexe2[j], EnveloppeConvexe2[j + 1], posIntersection);
                if (resVal.Item1 == true)
                {
                    Debug.Log("INTERSECTION EN : " + resVal.Item2);
                    
                }
            }
            
        }
    }


    public (bool ,Vector3) Intersection(GameObject Point1Poly1,GameObject Point2Poly1,GameObject Point1Poly2,GameObject Point2Poly2 ,Vector3 posIntersection)
    {
        float s1_x, s1_y, s2_x, s2_y;
        (bool, Vector3) posIntersectionPosition = (false, default);
        s1_x = Point2Poly1.transform.position.x - Point1Poly1.transform.position.x;
        s1_y = Point2Poly1.transform.position.y - Point1Poly1.transform.position.y;
        s2_x = Point2Poly2.transform.position.x - Point1Poly2.transform.position.x; 
        s2_y = Point2Poly2.transform.position.y - Point1Poly2.transform.position.y;
        float s, t;
        s = (-s1_y * (Point1Poly1.transform.position.x - Point1Poly2.transform.position.x) + s1_x * (Point1Poly1.transform.position.y - Point1Poly2.transform.position.y)) / (-s2_x * s1_y + s1_x * s2_y);
        t = (s2_x * (Point1Poly1.transform.position.y - Point1Poly2.transform.position.y) - s2_y * (Point1Poly1.transform.position.x - Point1Poly2.transform.position.x)) / (-s2_x * s1_y + s1_x * s2_y);
 
        if (s >= 0 && s <= 1 && t >= 0 && t <= 1)
        {
            // Collison
            posIntersectionPosition.Item1 = true;
            
            if (posIntersection != null)
            {
                
                posIntersectionPosition.Item2.x = Point1Poly1.transform.position.x + (t * s1_x);
                posIntersectionPosition.Item2.y = Point1Poly1.transform.position.y + (t * s1_y);
                posIntersectionPosition.Item2.z = 0;
               
            }
                
            return posIntersectionPosition;            
        }
        return posIntersectionPosition; // No Collision      
    }
}


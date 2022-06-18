using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Plane = UnityEngine.Plane;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using Matrice3 = Matrice.Matrice3x3;
using UnityEngine.UI;
public class ExtrusionBezier : MonoBehaviour
{
    
    public Camera camera;
    public float pas;
    private int size;
    private float scale = 0.5f;
    private Plane plane;
    public Canvas ui;
    
    //liste des points des courbes 
    private List<GameObject> GameobjectList = new List<GameObject>();
    private List<GameObject> BezierList = new List<GameObject>();
    
    private List<GameObject> GameobjectList2 = new List<GameObject>();
    private List<GameObject> BezierList2 = new List<GameObject>();
    
    private List<GameObject> ListeSelectioned = new List<GameObject>();
    
    public GameObject pointPrefab;
    public GameObject pointBezier;
    public GameObject courbe;
    private GameObject selectedObject = null;

    
    public Text pasValue;
    private Slider slider;
    public Text scaleValue;
    public Slider sliderScale;

    private void Start()
    {
        plane = new Plane(new Vector3(0, 0, -1), 0);
        slider = ui.GetComponentInChildren<Slider>();
        pasValue.text = size.ToString();
        slider.value = size;
        scaleValue.text = scale.ToString();
        sliderScale.value = (float)scale;
    }

    

    private void Update()
    {
        
        size = (int)slider.value;
        pasValue.text = "Distance : " +slider.value.ToString();
        scale = sliderScale.value;
        scaleValue.text = "Scale : " +sliderScale.value.ToString();
        
        
        //Vider la liste des points selectionés 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ListeSelectioned.Clear();
            selectedObject = null;
            Debug.Log("Liste vidée");
        }
        
        //permet de creer un point 
        if (Input.GetKeyDown(KeyCode.Z) )
        {
            
            DessinerPoints(GameobjectList,pointPrefab);
        }
        
        
        //Changer vue Camera
        if (Input.GetKeyDown(KeyCode.T))
        {
            camera.transform.position =new Vector3(0,14,0) ;
            camera.transform.Rotate(90,0,0);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            camera.transform.position =new Vector3(0,0,-15) ;
            camera.transform.Rotate(-90,0,0); 
        }
        
        
        // Selectioner un point et l'ajouter a la liste des point selectioné
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedObject == null)
            {
                RaycastHit hit = castRay();
                if (hit.collider != null)
                {
                    if (!hit.collider.CompareTag("BezPoint"))
                        return;
                    selectedObject = hit.collider.gameObject.transform.parent.gameObject;
                    ListeSelectioned.Add(selectedObject);
                    Debug.Log("Selectioned");
                }

                selectedObject = null;
            }
        }
        

    }

    private void LierExtrude(List<GameObject> list1, List<GameObject> list2,Color color)
    {
        if (list2.Count > 1)
        {
            for (int i = 0; i < list2.Count - 1 && i < list1.Count -1; i++)
            {
                if (!list2[i].GetComponent<LineRenderer>())
                {
                    Vector3 nextPoint;
                    if (i == list2.Count - 1 && list2.Count > 2)
                    {
                        nextPoint = list2[0].transform.position;
                    }
                    else if (list2.Count == 2)
                    {
                        break;
                    }
                    else
                    { 
                        nextPoint = list2[i + 1].transform.position;
                    }

                    LineRenderer _lineRenderer = list2[i].AddComponent<LineRenderer>();
                    _lineRenderer.startWidth = 0.1f;
                    _lineRenderer.endWidth = 0.1f;
                    _lineRenderer.positionCount = 3;
                    _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                    _lineRenderer.startColor = color;
                    _lineRenderer.endColor = color;
                    _lineRenderer.SetPosition(0,nextPoint);
                    _lineRenderer.SetPosition(1,list2[i].transform.position);
                    _lineRenderer.SetPosition(2, list1[i].transform.position);
                    
                }
                else
                {
                    Vector3 nextPoint;
                    if (i == list2.Count - 1 && list2.Count > 2)
                    {
                        nextPoint = list2[0].transform.position;
                    }
                    else if (list2.Count == 2)
                    {
                        break;
                    }
                    else
                    {
                        nextPoint = list2[i + 1].transform.position;
                    }
                    LineRenderer _lineRenderer = list2[i].GetComponent<LineRenderer>();
                    _lineRenderer.positionCount = 3;
                    _lineRenderer.SetPosition(0,nextPoint);
                    _lineRenderer.SetPosition(1,list2[i].transform.position);
                    _lineRenderer.SetPosition(2, list1[i].transform.position);
                }
            }
        }
        Debug.Log("fin lier");
    }


    // Dessiner 1 point
    void DessinerPoints(List<GameObject> lineExt, GameObject gameObject)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        float enter = 0f;
        Vector3 position;
        plane.Raycast(ray, out enter);
        position = ray.GetPoint(enter);
        RaycastHit hit = castRay();
        if (hit.collider != null)
        {
            if (hit.collider.CompareTag("cube"))
            {
                GameObject tmp;
                tmp = hit.collider.gameObject;
                PoidList poidlist =tmp.GetComponent<PoidList>();
                poidlist.poid += 1;
            }
            else
            {
                GameObject point =Instantiate(gameObject,position,Quaternion.identity);  
                lineExt.Add(point);
            }
        }
    }
    
    // Relie les point entre eux 
    public void Relier(List<GameObject> list, Color color)
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
                    _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                    _lineRenderer.startColor = color;
                    _lineRenderer.endColor = color;
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
    
    //Permet de Calculer les points de La courbe de Bezier avec l'algo de Casteljaw
    public Vector3 CalculateBezier(float t, List<GameObject> CurrentLine)
    {
        
        List<GameObject> CurrentLine2 = new List<GameObject>();
        foreach (var ptscontrol in CurrentLine)
        {
            for (int j = 0; j <= ptscontrol.GetComponent<PoidList>().poid; j++)
            {
                CurrentLine2.Add(ptscontrol);
            }
        }
        List<Vector3> bezierPoint = new List<Vector3>();
        for (int level = CurrentLine2.Count - 1; level >= 0; level--)
        {
            if (level == CurrentLine2.Count - 1)
            {
                for (int i = 0; i <= CurrentLine2.Count - 1; i++)
                {
                    bezierPoint.Add(CurrentLine2[i].transform.position);
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
    //Permet de draw la courbe principale
    public void Draw()
    {
        // on dessine la courbe externe
        if (GameObject.Find("Courbe Exterieur"))
        {
            GameObject.Destroy(GameObject.Find("Courbe Exterieur"));
        } 
        GameObject courbeExt = Instantiate(courbe);
        courbeExt.name = "Courbe Exterieur";
        for (int i = 0; i < GameobjectList.Count; i++)
        {
            GameobjectList[i].transform.SetParent(courbeExt.transform);
            Relier(GameobjectList,Color.red);
        }
        
       
        //on dessine la bezier
        if (GameObject.Find("Courbe Bezier"))
        {
            GameObject.Destroy(GameObject.Find("Courbe Bezier"));
        } 
        GameObject courbeBez = Instantiate(courbe);
        courbeBez.name = "Courbe Bezier";
        courbeBez.tag = "BezPoint";
        BezierList.Clear();
        for (float i = 0; i <= 1; i += pas )
        {
            if (GameobjectList.Count < 2)
            {
                return;
            }
            Vector3 position = CalculateBezier(i, GameobjectList);
            GameObject pointBez =Instantiate(pointBezier,position,Quaternion.identity);
            pointBez.transform.SetParent(courbeBez.transform);
            BezierList.Add(pointBez);
        }
        GameObject lastpointBez =Instantiate(pointBezier,GameobjectList[GameobjectList.Count-1].transform.position,Quaternion.identity);
        lastpointBez.transform.SetParent(courbeBez.transform);
        BezierList.Add(lastpointBez);
        if (GameObject.Find("Courbe Bezier")!= null)
            Relier(BezierList,Color.magenta);
        
    }
    
    
    public void DupliqueCourbe(List<GameObject> env , List<GameObject> bez, String nameBez)
    {
        //on dessine la bezier
        if (GameObject.Find(nameBez))
        {
            GameObject.Destroy(GameObject.Find(nameBez));
        } 
        GameObject courbeBez = Instantiate(courbe);
        courbeBez.name = nameBez; 
        bez.Clear();
        for (float i = 0; i <= 1; i += pas )
        {
            if (env.Count < 2)
            {
                return;
            }
            Vector3 position = CalculateBezier(i, env);
            GameObject pointBez =Instantiate(pointBezier,position,Quaternion.identity);
            pointBez.transform.SetParent(courbeBez.transform);
            bez.Add(pointBez);
        }
        GameObject lastpointBez =Instantiate(pointBezier,GameobjectList[GameobjectList.Count-1].transform.position,Quaternion.identity);
        lastpointBez.transform.SetParent(courbeBez.transform);
        bez.Add(lastpointBez);
        if (GameObject.Find(nameBez)!= null)
            Relier(bez,Color.magenta);
        
    }

    public void Extrude1()
    {
        if (ListeSelectioned != null)
        {
            if (GameObject.Find("extrude"))
            {
                Destroy(GameObject.Find("extrude"));
            }
            GameObject Extrude = Instantiate(ListeSelectioned[0], ListeSelectioned[0].transform.position,
                Quaternion.identity);
            Extrude.name = "extrude";
                
            for (int i = 0; i < Extrude.transform.childCount ; i++)
            {
                Matrix4x4 m = Matrix4x4.Translate(new Vector3(0f, 0, size));
                Vector3 posGameObject = Extrude.transform.GetChild(i).position;
                Extrude.transform.GetChild(i).position = m.MultiplyPoint3x4(posGameObject);
                    
                Matrix4x4 scaleMatrix = Matrix4x4.Scale(new Vector3(scale,scale,scale));
                Extrude.transform.GetChild(i).position += scaleMatrix.MultiplyPoint3x4(posGameObject);
                GameobjectList2.Add(Extrude.transform.GetChild(i).gameObject);
            }
                
            Relier(GameobjectList2,Color.blue);
            LierExtrude(BezierList,GameobjectList2,Color.green);
            Debug.Log(GameobjectList2.Count);
            Debug.Log(BezierList.Count);
        }
    }
}

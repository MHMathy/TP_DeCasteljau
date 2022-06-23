using System;
using System.Collections.Generic;
using UnityEngine;
using Plane = UnityEngine.Plane;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.UI;

public class ExtrusionBezier : MonoBehaviour
{

    public Camera cam;
    public float pas;
    private int size;
    private float scale = 0f;
    private Plane plane;
    public Canvas ui;
    private Vector3 centreBezier = new Vector3();

    //liste des points des courbes 
    private List<GameObject> GameobjectList = new List<GameObject>();
    private List<GameObject> BezierList = new List<GameObject>();
    private List<GameObject> CourbeExtrude1 = new List<GameObject>();
    private List<GameObject> CourbeExtrudePrec = new List<GameObject>();
    private List<GameObject> ListeSelectioned = new List<GameObject>();
    private List<GameObject> ListeNull = new List<GameObject>();
    private List<List<GameObject>> ListeExtrude = new List<List<GameObject>>();
    
    public GameObject pointPrefab;
    public GameObject pointBezierPrefab;
    public GameObject courbe;
    public GameObject pivotPrefab;
    private GameObject selectedObject = null;
    private GameObject currentPivot = null;
    
    public Material mat;

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
        pasValue.text = "Distance : " + slider.value.ToString();
        scale = sliderScale.value;
        scaleValue.text = "Scale : " + sliderScale.value.ToString();

        //Vider la liste des points selectionés 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ListeSelectioned.Clear();
            selectedObject = null;
            Debug.Log("Liste vidée");
        }

        //permet de creer un point 
        if (Input.GetKeyDown(KeyCode.Z))
            DessinerPoints(GameobjectList, pointPrefab);
        
        //permet de creer un point 
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (GameObject.Find("pivot"))
            {
                Destroy(GameObject.Find("pivot"));
            }
            DessinerPoints(ListeNull, pivotPrefab);
            currentPivot = GameObject.Find("pivot(Clone)");
            currentPivot.name = "pivot";
            
                
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

    private void LierExtrude(List<GameObject> list1, List<GameObject> list2, Color color)
    {
        if (list2.Count > 1)
        {
            for (int i = 0; i < list2.Count - 1 && i < list1.Count - 1; i++)
            {
                if (!list2[i].GetComponent<LineRenderer>())
                {
                    Vector3 nextPoint;

                    if (i == list2.Count - 1 && list2.Count > 2)
                        nextPoint = list2[0].transform.position;
                    else if (list2.Count == 2)
                        break;
                    else
                        nextPoint = list2[i + 1].transform.position;

                    LineRenderer lineRenderer = list2[i].AddComponent<LineRenderer>();
                    lineRenderer.startWidth = 0.1f;
                    lineRenderer.endWidth = 0.1f;
                    lineRenderer.positionCount = 3;
                    lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                    lineRenderer.startColor = color;
                    lineRenderer.endColor = color;
                    lineRenderer.SetPosition(0, nextPoint);
                    lineRenderer.SetPosition(1, list2[i].transform.position);
                    lineRenderer.SetPosition(2, list1[i].transform.position);
                }
                else
                {
                    Vector3 nextPoint;

                    if (i == list2.Count - 1 && list2.Count > 2)
                        nextPoint = list2[0].transform.position;
                    else if (list2.Count == 2)
                        break;
                    else
                        nextPoint = list2[i + 1].transform.position;

                    LineRenderer lineRenderer = list2[i].GetComponent<LineRenderer>();
                    lineRenderer.positionCount = 3;
                    lineRenderer.startColor = color;
                    lineRenderer.endColor = color;
                    lineRenderer.SetPosition(0, nextPoint);
                    lineRenderer.SetPosition(1, list2[i].transform.position);
                    lineRenderer.SetPosition(2, list1[i].transform.position);
                }
            }
        }
        Debug.Log("fin lier");
    }

    // Dessiner 1 point
    private void DessinerPoints(List<GameObject> lineExt, GameObject gameObject)
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
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
                Poid poidlist = tmp.GetComponent<Poid>();
                poidlist.poid += 1;
            }
            else
            {
                GameObject point = Instantiate(gameObject, position, Quaternion.identity);
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
                        nextPoint = list[0].transform.position;
                    else if (list.Count == 2)
                        break;
                    else
                        nextPoint = list[i + 1].transform.position;

                    LineRenderer lineRenderer = list[i].AddComponent<LineRenderer>();
                    lineRenderer.startWidth = 0.1f;
                    lineRenderer.endWidth = 0.1f;
                    lineRenderer.positionCount = 2;
                    lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
                    lineRenderer.startColor = color;
                    lineRenderer.endColor = color;
                    lineRenderer.SetPosition(0, list[i].transform.position);
                    lineRenderer.SetPosition(1, nextPoint);
                }
                else
                {
                    Vector3 nextPoint;

                    if (i == list.Count - 1 && list.Count > 2)
                        nextPoint = list[0].transform.position;
                    else if (list.Count == 2)
                        break;
                    else
                        nextPoint = list[i + 1].transform.position;

                    LineRenderer lineRenderer = list[i].GetComponent<LineRenderer>();
                    lineRenderer.SetPosition(0, list[i].transform.position);
                    lineRenderer.SetPosition(1, nextPoint);
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
            for (int j = 0; j <= ptscontrol.GetComponent<Poid>().poid; j++)
                CurrentLine2.Add(ptscontrol);
        }

        List<Vector3> bezierPoint = new List<Vector3>();
        for (int level = CurrentLine2.Count - 1; level >= 0; level--)
        {
            if (level == CurrentLine2.Count - 1)
            {
                for (int i = 0; i <= CurrentLine2.Count - 1; i++)
                    bezierPoint.Add(CurrentLine2[i].transform.position);
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
        int lastElem = bezierPoint.Count - 1;
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
            GameObject.Destroy(GameObject.Find("Courbe Exterieur"));

        GameObject courbeExt = Instantiate(courbe);
        courbeExt.name = "Courbe Exterieur";

        for (int i = 0; i < GameobjectList.Count; i++)
        {
            GameobjectList[i].transform.SetParent(courbeExt.transform);
            Relier(GameobjectList, Color.red);
        }

        //on dessine la bezier
        if (GameObject.Find("Courbe Bezier"))
            GameObject.Destroy(GameObject.Find("Courbe Bezier"));

        GameObject courbeBez = Instantiate(courbe);
        courbeBez.name = "Courbe Bezier";
        courbeBez.tag = "BezPoint";
        BezierList.Clear();

        for (float i = 0; i <= 1; i += pas)
        {
            if (GameobjectList.Count < 2)
                return;

            Vector3 position = CalculateBezier(i, GameobjectList);
            GameObject pointBez = Instantiate(pointBezierPrefab, position, Quaternion.identity);
            pointBez.transform.SetParent(courbeBez.transform);
            BezierList.Add(pointBez);
        }

        GameObject lastpointBez = Instantiate(pointBezierPrefab, GameobjectList[GameobjectList.Count - 1].transform.position, Quaternion.identity);
        lastpointBez.transform.SetParent(courbeBez.transform);
        BezierList.Add(lastpointBez);
        centreBezier = CalculcentreBezier(BezierList);
        if (GameObject.Find("Courbe Bezier") != null)
            Relier(BezierList, Color.magenta);
    }

    public void Extrude1()
    {
        if (ListeSelectioned != null)
        {
            if (GameObject.Find("extrude"))
                Destroy(GameObject.Find("extrude"));

            GameObject Extrude = Instantiate(ListeSelectioned[0], ListeSelectioned[0].transform.position, Quaternion.identity);
            Extrude.name = "extrude";

            for (int i = 0; i < Extrude.transform.childCount; i++)
            {
                Matrix4x4 m = Matrix4x4.Translate(new Vector3(0f, 0, size));
                Vector3 posGameObject = Extrude.transform.GetChild(i).position;
                Extrude.transform.GetChild(i).position = m.MultiplyPoint3x4(posGameObject);

                Matrix4x4 scaleMatrix = Matrix4x4.Scale(new Vector3(scale, scale, scale));
                Extrude.transform.GetChild(i).position += scaleMatrix.MultiplyPoint3x4(posGameObject);
                CourbeExtrude1.Add(Extrude.transform.GetChild(i).gameObject);
            }

            Relier(CourbeExtrude1, Color.blue);
            LierExtrude(BezierList, CourbeExtrude1, Color.green);

            // afficher face de l'extrusion
            for (int l = 0; l < BezierList.Count - 2; l++)
            {
                GameObject face = Instantiate(new GameObject(), BezierList[l].transform.position, new Quaternion(0f, 0f, 0f, 0f));
                MeshFilter mf = face.AddComponent(typeof(MeshFilter)) as MeshFilter;
                MeshRenderer mr = face.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

                Mesh m = new Mesh();

                float P2X = BezierList[l + 1].transform.position.x - BezierList[l].transform.position.x;
                float P2Y = BezierList[l + 1].transform.position.y - BezierList[l].transform.position.y;

                float P3X = CourbeExtrude1[l + 1].transform.position.x - BezierList[l].transform.position.x;
                float P3Y = CourbeExtrude1[l + 1].transform.position.y - BezierList[l].transform.position.y;
                float P3Z = CourbeExtrude1[l + 1].transform.position.z - BezierList[l].transform.position.z;

                float P4X = CourbeExtrude1[l].transform.position.x - BezierList[l].transform.position.x;
                float P4Y = CourbeExtrude1[l].transform.position.y - BezierList[l].transform.position.y;
                float P4Z = CourbeExtrude1[l].transform.position.z - BezierList[l].transform.position.z;

                m.vertices = new Vector3[]{
                    new Vector3(0f, 0f, 0f),
                    new Vector3(P2X, P2Y, 0f),
                    new Vector3(P3X, P3Y, P3Z),
                    new Vector3(P4X, P4Y, P4Z)
                };

                m.uv = new Vector2[] {
                    new Vector2 (0, 0),
                    new Vector2 (0, 1),
                    new Vector2(1, 1),
                    new Vector2 (1, 0)
                };

                m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
                mf.mesh = m;
                mr.material = mat;
            }
        }
        else Debug.Log("selectionner point");
    }

    public Vector3 CalculcentreBezier(List<GameObject> listeGo)
    {
        Vector3 res = new Vector3();

        for (int i = 0; i < listeGo.Count; i++)
        {
            res.x += listeGo[i].transform.position.x;
            res.y += listeGo[i].transform.position.y;
            res.z += listeGo[i].transform.position.z;
            
        }

        res = res / listeGo.Count;
        return res;
    }
    
    public void ExtrudeRevolution()
    {
        if (ListeSelectioned != null && currentPivot != null)
        {
            var distancePivot = Vector3.Distance(centreBezier , currentPivot.transform.position);
            
            for (int j = 20; j < 360; j += 20)
            {
                var sinTheta = Mathf.Sin(j * Mathf.Deg2Rad);
                var cosTheta = Mathf.Cos(j * Mathf.Deg2Rad);
                
                GameObject Extrude = Instantiate(ListeSelectioned[0], ListeSelectioned[0].transform.position,
                    Quaternion.identity); 
                Extrude.name = "extrudeRevolution" + j;
                for (int i = 0; i < Extrude.transform.childCount; i++)
                {
                    Vector3 pos = new Vector3();
                    pos.x = Extrude.transform.GetChild(i).position.x * cosTheta+Extrude.transform.GetChild(i).position.z * sinTheta ;
                    pos.y = Extrude.transform.GetChild(i).position.y;
                    pos.z = Extrude.transform.GetChild(i).position.z * cosTheta - Extrude.transform.GetChild(i).position.x * sinTheta + currentPivot.transform.position.z;
                    Extrude.transform.GetChild(i).position = pos;
                    CourbeExtrude1.Add(Extrude.transform.GetChild(i).gameObject);
                } 
                
                ListeExtrude.Add(CourbeExtrude1);
                Debug.Log(CourbeExtrude1.Count);
                Relier(CourbeExtrude1, Color.blue);
                
                Debug.Log(CourbeExtrude1.Count + " et " + CourbeExtrudePrec.Count);
                LierExtrude(CourbeExtrudePrec, CourbeExtrude1, Color.red);
                CourbeExtrudePrec.Clear();
                foreach (var point in CourbeExtrude1)
                {
                    CourbeExtrudePrec.Add(point);
                }
                CourbeExtrude1.Clear();
            }
            LierExtrude(CourbeExtrudePrec, BezierList, Color.red);
            List<GameObject> l = new List<GameObject>();
            for (int i = 0; i < ListeExtrude[0].Count; i++)
            {
                l.Add(ListeExtrude[0][i]);
            }
            LierExtrude(BezierList, l, Color.red);
        }
    }

    public void ExtrudeGeneralise()
    {
        if (ListeSelectioned != null)
        {
            if (GameObject.Find("extrude"))
                Destroy(GameObject.Find("extrude"));

            GameObject Extrude = Instantiate(ListeSelectioned[0], ListeSelectioned[0].transform.position,
                Quaternion.identity);
            Extrude.name = "extrude";

            for (int i = 0; i < Extrude.transform.childCount; i++)
            {
                Matrix4x4 m = Matrix4x4.Translate(new Vector3(0f, 0, size));
                Vector3 posGameObject = Extrude.transform.GetChild(i).position;
                Extrude.transform.GetChild(i).position = m.MultiplyPoint3x4(posGameObject);

                Matrix4x4 scaleMatrix = Matrix4x4.Scale(new Vector3(scale, scale, scale));
                Extrude.transform.GetChild(i).position += scaleMatrix.MultiplyPoint3x4(posGameObject);
                CourbeExtrude1.Add(Extrude.transform.GetChild(i).gameObject);
            }

            Relier(CourbeExtrude1, Color.blue);
            LierExtrude(BezierList, CourbeExtrude1, Color.green);
        }
    }
}

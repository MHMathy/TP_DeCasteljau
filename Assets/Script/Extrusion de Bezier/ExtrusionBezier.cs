using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Plane = UnityEngine.Plane;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using UnityEngine.UI;

public class ExtrusionBezier : MonoBehaviour
{

    public Camera cam;
    public float pas;
    private float pasCourbe = 0.08f;
    private int size = 0;
    private float scale = 1f;
    private Plane plane;
    public Canvas ui;
    private Vector3 centreBezier = new Vector3();

    //liste des points des courbes 
    private List<GameObject> GameobjectList = new List<GameObject>();
    private List<GameObject> BezierList = new List<GameObject>();
    private List<GameObject> CourbeExtrude1 = new List<GameObject>();
    private List<GameObject> CourbeExtrudePrec = new List<GameObject>();
    private List<GameObject> ListeSelectioned = new List<GameObject>();
    private List<GameObject> ListeSelectionedCourbe = new List<GameObject>();
    private List<List<GameObject>> ListeExtrude = new List<List<GameObject>>();
    private List<GameObject> CourbeBez2Gen = new List<GameObject>();
    private List<GameObject> CourbeBezierGeneralise = new List<GameObject>();
    
    
    public GameObject pointPrefab;
    public GameObject pointPrefab2;
    public GameObject pointBezierPrefab;
    public GameObject courbe;
    private GameObject selectedObject = null;
    private GameObject SelectedCourbe = null;

    public Material mat;

    public Text distanceValue;
    private Slider slider;
    public Text scaleValue;
    public Slider sliderScale;
    public Text pasValue;
    public Slider sliderpas;
    private MoveCam _MoveCam;
    private bool Ismoving = false;
    private bool IsVisible = true;
    private void Start()
    {
        plane = new Plane(new Vector3(0, 0, -1), 0);
        slider = ui.GetComponentInChildren<Slider>();
        distanceValue.text = size.ToString();
        slider.value = size;
        scaleValue.text = scale.ToString();
        sliderScale.value = (float)scale;
        _MoveCam = cam.GetComponent<MoveCam>();
        _MoveCam.enabled = false;
         IsVisible = true;
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.P))
        {
            Cursor.visible = !IsVisible;
            _MoveCam.enabled =!Ismoving;
            Ismoving = !Ismoving;
        }
        
        size = (int)slider.value;
        distanceValue.text = "Distance : " + slider.value;
        scale = sliderScale.value;
        scaleValue.text = "Scale : " + sliderScale.value;
        
        //Vider la liste des points selectionés 
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ListeSelectioned.Clear();
            selectedObject = null;
            SelectedCourbe = null;
            Debug.Log("Liste vidée");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        //permet de creer un point 
        if (Input.GetKeyDown(KeyCode.Z))
            DessinerPoints(GameobjectList, pointPrefab);
        
        if (Input.GetKeyDown(KeyCode.E))
            DessinerPoints(CourbeBez2Gen, pointPrefab2);

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

        if (Input.GetMouseButtonDown(1))
        {
            if (SelectedCourbe == null)
            {
                RaycastHit hit = castRay();
                if (hit.collider != null)
                {
                    if (!hit.collider.CompareTag("Cube2"))
                        return;
                    SelectedCourbe = hit.collider.gameObject.transform.parent.gameObject;
                    ListeSelectionedCourbe.Add(SelectedCourbe);
                    Debug.Log("SelectionedCourbe");
                }
                SelectedCourbe = null;
            }
        }


        if (ListeSelectionedCourbe == null && ListeSelectioned == null)
        {
            Debug.Log("null");
        }
        if (ListeSelectionedCourbe != null)
        {
            if (Input.GetKey(KeyCode.C))
            {
                Quaternion rotation = Quaternion.Euler(0, 0, 10);
                Matrix4x4 m = Matrix4x4.Rotate(rotation);
                for (int i = 0; i < ListeSelectionedCourbe[0].transform.childCount; i++)
                    ListeSelectionedCourbe[0].transform.GetChild(i).position =
                        m.MultiplyVector(ListeSelectionedCourbe[0].transform.GetChild(i).position);

            }

            //Rota Y
            if (Input.GetKey(KeyCode.B))
            {
                Quaternion rotation = Quaternion.Euler(0, 10, 0);
                Matrix4x4 m = Matrix4x4.Rotate(rotation);
                for (int i = 0; i < ListeSelectionedCourbe[0].transform.childCount; i++)
                    ListeSelectionedCourbe[0].transform.GetChild(i).position =
                        m.MultiplyVector(ListeSelectionedCourbe[0].transform.GetChild(i).position);
            }

            //rota X
            if (Input.GetKey(KeyCode.V))
            {
                Quaternion rotation = Quaternion.Euler(10, 0, 0);
                Matrix4x4 m = Matrix4x4.Rotate(rotation);
                for (int i = 0; i < ListeSelectionedCourbe[0].transform.childCount; i++)
                    ListeSelectionedCourbe[0].transform.GetChild(i).position =
                        m.MultiplyVector(ListeSelectionedCourbe[0].transform.GetChild(i).position);
                
            }
            
            if (Input.GetKey(KeyCode.N))
            {
                Matrix4x4 m = Matrix4x4.Translate(new Vector3(0f, 0, 1));
                for (int i = 0; i < ListeSelectionedCourbe[0].transform.childCount; i++)
                {
                    Vector3 posGameObject = ListeSelectionedCourbe[0].transform.GetChild(i).position;
                    ListeSelectionedCourbe[0].transform.GetChild(i).position = m.MultiplyPoint3x4(posGameObject);
                }
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
    
    public void Draw2()
    {
        // on dessine la courbe externe
        if (GameObject.Find("CourbeBez2gen"))
            GameObject.Destroy(GameObject.Find("CourbeBez2gen"));

        GameObject courbeExt = Instantiate(courbe);
        courbeExt.name = "CourbeBez2gen";

        for (int i = 0; i < CourbeBez2Gen.Count; i++)
        {
            CourbeBez2Gen[i].transform.SetParent(courbeExt.transform);
            Relier(CourbeBez2Gen, Color.blue);
        }

        //on dessine la bezier
        if (GameObject.Find("Courbe bezier Pour generaliser"))
            GameObject.Destroy(GameObject.Find("Courbe bezier Pour generaliser"));

        GameObject courbeBez = Instantiate(courbe);
        courbeBez.name = "Courbe bezier Pour generaliser";
        CourbeBezierGeneralise.Clear();

        for (float i = 0; i <= 1; i += pas)
        {
            if (CourbeBez2Gen.Count < 2)
                return;

            Vector3 position = CalculateBezier(i, CourbeBez2Gen);
            GameObject pointBez = Instantiate(pointBezierPrefab, position, Quaternion.identity);
            pointBez.transform.SetParent(courbeBez.transform);
            CourbeBezierGeneralise.Add(pointBez);
        }

        GameObject lastpointBez = Instantiate(pointBezierPrefab, CourbeBez2Gen[CourbeBez2Gen.Count - 1].transform.position, Quaternion.identity);
        lastpointBez.transform.SetParent(courbeBez.transform);
        CourbeBezierGeneralise.Add(lastpointBez);
        
        if (GameObject.Find("Courbe bezier Pour generaliser") != null)
            Relier(CourbeBezierGeneralise, Color.green);
        
        Debug.Log(CourbeBezierGeneralise.Count);
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
               
               var transformPosition = Extrude.transform.GetChild(i).transform.position;
               transformPosition = scalePoint(transformPosition, scale);
               Extrude.transform.GetChild(i).transform.position = transformPosition;
               Matrice.Matrice3x3 Mat = Matrice.Matrice3x3.CreateTranslation(new Vector3(0f, 0, size));
                Vector3 posGameObject = Extrude.transform.GetChild(i).position;
                Extrude.transform.GetChild(i).position  = Mat * posGameObject;
                CourbeExtrude1.Add(Extrude.transform.GetChild(i).gameObject); 
            }
            Relier(CourbeExtrude1, Color.blue);
            LierExtrude(BezierList, CourbeExtrude1, Color.green);
            // afficher face de l'extrusion
            CreeFace(BezierList, CourbeExtrude1);
        }
    }
    
    public static Vector3 scalePoint(Vector3 point, float scale)
    {
        Matrix4x4 mat = new Matrix4x4();

        mat[0, 0] = scale;
        mat[1, 1] = scale;
        mat[2, 2] = scale;
        mat[3, 3] = 1;

        point = mat.MultiplyPoint(point);

        return point;
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
        if (ListeSelectioned != null)
        {
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
                    pos.x = Extrude.transform.GetChild(i).position.x * cosTheta + Extrude.transform.GetChild(i).position.z * sinTheta ;
                    pos.y = Extrude.transform.GetChild(i).position.y;
                    pos.z = Extrude.transform.GetChild(i).position.z * cosTheta - Extrude.transform.GetChild(i).position.x * sinTheta + ListeSelectioned[0].transform.GetChild(0).transform.position.z;
                    Extrude.transform.GetChild(i).position = pos;
                    CourbeExtrude1.Add(Extrude.transform.GetChild(i).gameObject);
                }
                ListeExtrude.Add(CourbeExtrude1);
                Relier(CourbeExtrude1, Color.blue);
                
                if (ListeExtrude.Count == 1)
                {
                    LierExtrude(BezierList, CourbeExtrude1, Color.green); 
                    CreeFace(BezierList, CourbeExtrude1);
                }
                LierExtrude(CourbeExtrudePrec, CourbeExtrude1, Color.green);
                CreeFace(CourbeExtrudePrec, CourbeExtrude1);
                CourbeExtrudePrec.Clear();
                foreach (var point in CourbeExtrude1)
                {
                    CourbeExtrudePrec.Add(point);
                }
                CourbeExtrude1.Clear();
            }
            LierExtrude(CourbeExtrudePrec, BezierList, Color.green);
            CreeFace(CourbeExtrudePrec, BezierList);
        }
    }

    public void ExtrudeGeneralise()
    {
        if (ListeSelectioned != null && ListeSelectionedCourbe!= null)
        {
            for (int i = 0; i < CourbeBezierGeneralise.Count-1; i++)
            {
                GameObject Extrude = Instantiate(ListeSelectioned[0], ListeSelectioned[0].transform.position,
                    Quaternion.identity);
                Extrude.name = "extrude"+i;
                Vector3 dif = CalculcentreBezier(Extrude);
                for (int j = 0; j < Extrude.transform.childCount; j++)
                {
                    float offsetx = Mathf.Abs(dif.x - Extrude.transform.GetChild(j).position.x);
                    float offsety = Mathf.Abs(dif.y - Extrude.transform.GetChild(j).position.y);
                    Vector3 pos = new Vector3();
                    pos.x = Extrude.transform.GetChild(j).position.x + CourbeBezierGeneralise[i].transform.position.x  ;
                    pos.y = Extrude.transform.GetChild(j).position.y + CourbeBezierGeneralise[i].transform.position.y ;
                    pos.z = Extrude.transform.GetChild(j).position.z + CourbeBezierGeneralise[i].transform.position.z ;
                    Extrude.transform.GetChild(j).position = pos;
                    CourbeExtrude1.Add(Extrude.transform.GetChild(j).gameObject);
                }
                ListeExtrude.Add(CourbeExtrude1);
                Relier(CourbeExtrude1, Color.blue);
                
                if (ListeExtrude.Count == 1)
                {
                    LierExtrude(BezierList, CourbeExtrude1, Color.red); 
                    CreeFace(BezierList, CourbeExtrude1);
                }
                LierExtrude(CourbeExtrudePrec, CourbeExtrude1, Color.red);
                CreeFace(CourbeExtrudePrec, CourbeExtrude1);
                CourbeExtrudePrec.Clear();
                foreach (var point in CourbeExtrude1)
                {
                    CourbeExtrudePrec.Add(point);
                }
                CourbeExtrude1.Clear(); 
            }
        }
    }
    private Vector3 CalculcentreBezier(GameObject listeGo)
    {
        Vector3 res = new Vector3();

        for (int i = 0; i < listeGo.transform.childCount; i++)
        {
            res.x += listeGo.transform.GetChild(i).transform.position.x;
            res.y += listeGo.transform.GetChild(i).transform.position.y;
            res.z += listeGo.transform.GetChild(i).transform.position.z;
            
        }

        res = res / listeGo.transform.childCount;
        return res;
    }

    public void CreeFace(List<GameObject> list1 , List<GameObject> list2)
    {
        for (int l = 0; l < list1.Count - 2; l++)
        {
            GameObject face = Instantiate(new GameObject(), list1[l].transform.position, new Quaternion(0f, 0f, 0f, 0f));
            MeshFilter mf = face.AddComponent(typeof(MeshFilter)) as MeshFilter;
            MeshRenderer mr = face.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            
            Mesh m = new Mesh();

            float P2X = list1[l + 1].transform.position.x - list1[l].transform.position.x;
            float P2Y = list1[l + 1].transform.position.y - list1[l].transform.position.y;

            float P3X = list2[l + 1].transform.position.x - list1[l].transform.position.x;
            float P3Y = list2[l + 1].transform.position.y - list1[l].transform.position.y;
            float P3Z = list2[l + 1].transform.position.z - list1[l].transform.position.z;

            float P4X = list2[l].transform.position.x - list1[l].transform.position.x;
            float P4Y = list2[l].transform.position.y - list1[l].transform.position.y;
            float P4Z = list2[l].transform.position.z - list1[l].transform.position.z;

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
    
    
}


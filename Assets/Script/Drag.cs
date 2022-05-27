using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drag : MonoBehaviour
{
 
    private Vector3 getMouseWorldPosition()
    {
        Vector3 mousePoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePoint.z = -1;
        return mousePoint;
    }

    void OnMouseDrag()
    {
        transform.position = getMouseWorldPosition();
    }
}

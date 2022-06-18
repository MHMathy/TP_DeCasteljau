using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCam : MonoBehaviour
{
    public CharacterController _controller;
    public float speed = 12f;
    public float rotationSpeed = 180;
    private Vector3 rotation;
    private void Update()
    {
        this.rotation = new Vector3(0, Input.GetAxisRaw("Horizontal") * rotationSpeed * Time.deltaTime, 0);
 
        Vector3 move = new Vector3(0, 0, Input.GetAxisRaw("Vertical") * Time.deltaTime);
        move = this.transform.TransformDirection(move);
        _controller.Move(move * speed);
        this.transform.Rotate(this.rotation);
        
    }
    
}

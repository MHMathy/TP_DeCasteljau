using UnityEditor;
using UnityEngine;

public class MoveCam : MonoBehaviour
{
  /*public CharacterController controller;
    public float speed = 12f;
    Vector3 mPrevPos = Vector3.zero;
    Vector3 mPosDelta = Vector3.zero;
    private void Update()
    {
        var x = Input.GetAxis("Horizontal");
        var z = Input.GetAxis("Vertical");
        var move = transform.right * x + transform.forward * z;
        controller.Move(move * (speed * Time.deltaTime));
        if (Input.GetKey(KeyCode.O))
        {
            mPosDelta = Input.mousePosition - mPrevPos;
            transform.Rotate(transform.up, Vector3.Dot(mPosDelta, Camera.main.transform.right), Space.World);
        }
        mPrevPos = Input.mousePosition;
    }*/
  
  public CharacterController controller;
  public float speed = 12f; 
  public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
  public RotationAxes axes = RotationAxes.MouseXAndY;
  public float sensitivityX = 15F;
  public float sensitivityY = 15F;
  public float minimumX = -360F;
  public float maximumX = 360F;
  public float minimumY = -60F;
  public float maximumY = 60F;
  float rotationY = 0F;
  void Update ()
  {
      var x = Input.GetAxis("Horizontal");
      var z = Input.GetAxis("Vertical");
      var move = transform.right * x + transform.forward * z;
      controller.Move(move * (speed * Time.deltaTime));
      if (axes == RotationAxes.MouseXAndY)
      {
          float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
          rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
          rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
          transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
      }
      else if (axes == RotationAxes.MouseX)
      {
          transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
      }
      else
      {
          rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
          rotationY = Mathf.Clamp (rotationY, minimumY, maximumY);
          transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y, 0);
      }
  }
}

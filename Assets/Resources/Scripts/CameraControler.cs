using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] float rotationSpeed = 50f;
    [SerializeField] Transform cameraObject;
    [SerializeField] KeyCode leftRotate;
    [SerializeField] KeyCode rightRotate;
    [SerializeField] KeyCode upRotate;
    [SerializeField] KeyCode downRotate;
    [SerializeField] float xMinAngel;
    [SerializeField] float xMaxAngel;
    [SerializeField] bool invertY = false;
    [SerializeField] bool invertMouseY = false;
    int invert = -1;
    int invertMouse = -1;
    [SerializeField][Range(1, 15)]  float mouseSensitivity = 1f;

    private void Start()
    {
        if (invertY)
        {
            invert = 1;
        }
        if (invertMouseY)
        {
            invertMouse = 1;
        }
    }

    void Update()
    {
        //Get input from player
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        MoveCamera(new Vector2(horizontal, vertical));

        //get input from player
        Vector2 rotate = Vector2.zero;

        //if mouse clicked get input from mouse
        if (Input.GetMouseButton(0))
        {
            rotate.x = Input.GetAxis("Mouse X") * mouseSensitivity;
            rotate.y = Input.GetAxis("Mouse Y") * invertMouse * mouseSensitivity;
        }

        if (Input.GetKey(leftRotate))
        {
            Debug.Log("Left");
            rotate.x += -1;
        }
        if (Input.GetKey(rightRotate))
        {
            Debug.Log("right");
            rotate.x += 1;
        }
        if (Input.GetKey(upRotate))
        {
            Debug.Log("up");
            rotate.y += 1 * invert;
        }
        if (Input.GetKey(downRotate))
        {
            Debug.Log("down");
            rotate.y += -1 * invert;
        }
        RotateCamera(rotate);
    }

    private void RotateCamera(Vector2 angel)
    {
        transform.Rotate(Vector3.up, angel.x * rotationSpeed * Time.unscaledDeltaTime);

        //check if the camera is not going to rotate more than the limit
        float xAngel = cameraObject.localEulerAngles.x;
        if (xAngel > 180)
        {
            xAngel -= 360;
        }
        xAngel = Mathf.Clamp(xAngel + angel.y * rotationSpeed * Time.unscaledDeltaTime, xMinAngel, xMaxAngel);
        cameraObject.localEulerAngles = new Vector3(xAngel, 0, 0);
    }

    private void MoveCamera(Vector2 distance)
    {
        transform.position += (transform.forward * distance.y + transform.right * distance.x)*speed * Time.unscaledDeltaTime;
    }


}

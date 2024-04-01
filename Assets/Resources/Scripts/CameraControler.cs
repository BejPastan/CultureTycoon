using UnityEngine;
using System;

public class CameraControler : MonoBehaviour
{
    public static CameraControler instance;
    private void Awake()
    {
        instance = this;
    }

    [SerializeField] Transform cameraObject;
    [Header("SEttings")]
    [SerializeField][Range(1, 15)] float mouseSensitivity = 1f;
    [SerializeField] float speed = 10f;
    [SerializeField] float rotationSpeed = 50f;

    [Header("Rotation")]
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

    [Header("Zoom")]
    [SerializeField] KeyCode zoomOut;
    [SerializeField] KeyCode zoomIn;
    [SerializeField] float zoomSpeed = 10f;
    [SerializeField] float zoomMin = 1f;
    [SerializeField] float zoomMax = 10f;

    bool canZoom = true;

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
        if (Input.GetMouseButton(2))
        {
            rotate.x = Input.GetAxis("Mouse X") * mouseSensitivity;
            rotate.y = Input.GetAxis("Mouse Y") * invertMouse * mouseSensitivity;
        }

        if (Input.GetKey(leftRotate))
        {
            rotate.x += -1;
        }
        if (Input.GetKey(rightRotate))
        {
            rotate.x += 1;
        }
        if (Input.GetKey(upRotate))
        {
            rotate.y += 1 * invert;
        }
        if (Input.GetKey(downRotate))
        {
            rotate.y += -1 * invert;
        }
        RotateCamera(rotate);

        //if is not over UI
        if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && (canZoom || Input.GetKey(KeyCode.LeftAlt)))
        {
            float zoom = 0;
            zoom = Input.mouseScrollDelta.y;
            if (Input.GetKey(zoomOut))
            {
                zoom += 1;
            }
            if (Input.GetKey(zoomIn))
            {
                zoom -= 1;
            }
            if (zoom != 0)
                Zoom(zoom);
        }
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
        //call event
        //get rotation of camera in angles
        Vector3 rotation = new Vector3(xAngel, transform.eulerAngles.y, 0);
        onCameraRotation?.Invoke(rotation);
    }


    public delegate void CameraRotation(Vector3 rotation);
    public static event CameraRotation onCameraRotation;

    private void MoveCamera(Vector2 distance)
    {
        transform.position += (transform.forward * distance.y + transform.right * distance.x)*speed * Time.unscaledDeltaTime;
    }

    private void Zoom(float distance)
    {
        Vector3 dist = (distance * zoomSpeed * Time.unscaledDeltaTime * cameraObject.forward);
        //check if the camera is not going to be closer than the limit
        if ((transform.position +dist).y < zoomMin)
        {
            float y = zoomMin - transform.position.y;
            float proportion  = y / dist.y;
            distance *= proportion;
        }
        if ((transform.position + dist).y > zoomMax)
        {
            float y = zoomMax - transform.position.y;
            float proportion = y / dist.y;
            distance *= proportion;
        }
        transform.position += cameraObject.forward * distance * zoomSpeed * Time.unscaledDeltaTime;
    }

    public void EnableZoom()
    {
        canZoom = true;
    }

    public void DisableZoom()
    {
        canZoom = false;
    }
}

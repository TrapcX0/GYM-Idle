using UnityEngine;
 
public class CameraController : MonoBehaviour

{

    [Header("Camera Settings")]

    public float panSpeed = 2f;

    public float zoomSpeed = 3f;

    public float minZoom = 8f;

    public float maxZoom = 20f;
 
    [Header("Movement Limits")]

    public float minX = -12f;
    
    public float maxX = 12f;

    public float minZ = -12f;

    public float maxZ = 8f;
 
    private Vector3 lastTouchPosition;

    private Camera cam;

    private Vector3 initialPosition;
 
    void Start()

    {

        cam = GetComponent<Camera>();

        initialPosition = transform.position;
 
        #if UNITY_ANDROID || UNITY_IOS

        cam.orthographicSize = 12f;

        #endif

    }
 
    void Update()

    {

        if (Input.touchCount == 1)

        {

            HandleSingleTouch();

        }

        else if (Input.touchCount == 2)

        {

            HandlePinchZoom();

        }
 
        #if UNITY_EDITOR

        HandleMouseInput();

        #endif

    }
 
    void HandleSingleTouch()

    {

        Touch touch = Input.GetTouch(0);
 
        if (touch.phase == TouchPhase.Began)

        {

            lastTouchPosition = touch.position;

        }

        else if (touch.phase == TouchPhase.Moved)

        {

            Vector3 delta = cam.ScreenToViewportPoint(lastTouchPosition - (Vector3)touch.position);
 
            Vector3 move = new Vector3(delta.x * panSpeed, 0, delta.y * panSpeed);
 
            Vector3 newPosition = transform.position + move;
 
            // Sınır uygula

            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);

            newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);
 
            transform.position = newPosition;
 
            lastTouchPosition = touch.position;

        }

    }
 
    void HandlePinchZoom()

    {

        Touch touch1 = Input.GetTouch(0);

        Touch touch2 = Input.GetTouch(1);
 
        Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

        Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;
 
        float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;

        float touchDeltaMag = (touch1.position - touch2.position).magnitude;
 
        float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
 
        if (cam.orthographic)

        {

            cam.orthographicSize += deltaMagnitudeDiff * zoomSpeed * 0.01f;

            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);

        }

    }
 
    #if UNITY_EDITOR

    void HandleMouseInput()

    {

        if (Input.GetMouseButtonDown(0))

        {

            lastTouchPosition = Input.mousePosition;

        }
 
        if (Input.GetMouseButton(0))

        {

            Vector3 delta = cam.ScreenToViewportPoint(lastTouchPosition - Input.mousePosition);
 
            Vector3 move = new Vector3(delta.x * panSpeed, 0, delta.y * panSpeed);
 
            Vector3 newPosition = transform.position + move;

            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);

            newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);
 
            transform.position = newPosition;
 
            lastTouchPosition = Input.mousePosition;

        }
 
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (cam.orthographic)

        {

            cam.orthographicSize -= scroll * zoomSpeed;

            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);

        }

    }

    #endif
 
    public void ResetCamera()

    {

        transform.position = initialPosition;

        if (cam.orthographic)

        {

            cam.orthographicSize = 5f;

        }

    }

}

 
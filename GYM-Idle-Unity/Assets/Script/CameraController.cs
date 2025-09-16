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
    
    // Mobil için size ayarı
    #if UNITY_ANDROID || UNITY_IOS
    cam.orthographicSize = 12f;
    #endif
    }
    
    void Update()
    {
        // Touch input (mobil için)
        if (Input.touchCount == 1)
        {
            HandleSingleTouch();
        }
        else if (Input.touchCount == 2)
        {
            HandlePinchZoom();
        }
        
        // Mouse input (test için - editörde)
        #if UNITY_EDITOR
        HandleMouseInput();
        #endif
    }
    
    void HandleSingleTouch()
    {
        Touch touch = Input.GetTouch(0);
        
        if (touch.phase == TouchPhase.Began)
        {
            lastTouchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, transform.position.y));
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            Vector3 currentTouchPosition = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, transform.position.y));
            Vector3 difference = lastTouchPosition - currentTouchPosition;
            
            Vector3 newPosition = transform.position + difference;
            
            // Sınırları uygula
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);
            
            transform.position = newPosition;
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
        // Mouse ile pan (test için)
        if (Input.GetMouseButtonDown(0))
        {
            lastTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        
        if (Input.GetMouseButton(0))
        {
            Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 difference = lastTouchPosition - currentMousePosition;
            
            Vector3 newPosition = transform.position + difference;
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);
            
            transform.position = newPosition;
        }
        
        // Mouse scroll ile zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (cam.orthographic)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }
    #endif
    
    // Kamerayı başlangıç pozisyonuna döndür (isteğe bağlı)
    public void ResetCamera()
    {
        transform.position = initialPosition;
        if (cam.orthographic)
        {
            cam.orthographicSize = 5f;
        }
    }
}
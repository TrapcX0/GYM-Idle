using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Movement")]
    public float panSpeed = 3f;
    
    [Header("Movement Limits")]
    public float minX = -15f;
    public float maxX = 15f;
    public float minZ = -10f;
    public float maxZ = 15f;
    
    private Vector3 lastTouchPosition;
    private Camera cam;
    private bool isDragging = false;
    
    void Start()
    {
        cam = GetComponent<Camera>();
        
        // Hafif eğimli görünüm için kamera ayarları
        transform.position = new Vector3(0, 16, -6);
        transform.rotation = Quaternion.Euler(65, 0, 0); // 65° açı - hafif eğim
        
        // Orthographic projection
        cam.orthographic = true;
        cam.orthographicSize = 10f; // Biraz daha yakın görünüm
    }
    
    void Update()
    {
        // Touch input (mobil için)
        if (Input.touchCount == 1)
        {
            HandleSingleTouch();
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
            Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, cam.transform.position.y));
            lastTouchPosition = worldPos;
            isDragging = true;
        }
        else if (touch.phase == TouchPhase.Moved && isDragging)
        {
            Vector3 currentWorldPos = cam.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, cam.transform.position.y));
            Vector3 difference = lastTouchPosition - currentWorldPos;
            
            // Sadece X ve Z eksenlerinde hareket
            Vector3 newPosition = transform.position;
            newPosition.x += difference.x;
            newPosition.z += difference.z;
            
            // Sınırları uygula
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);
            
            // Y pozisyonunu sabit tut (her zaman 16'da kalsın)
            newPosition.y = 16f;
            
            transform.position = newPosition;
        }
        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
        {
            isDragging = false;
        }
    }
    
    #if UNITY_EDITOR
    void HandleMouseInput()
    {
        // Mouse ile drag (test için)
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.transform.position.y));
            lastTouchPosition = worldPos;
            isDragging = true;
        }
        
        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 currentWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, cam.transform.position.y));
            Vector3 difference = lastTouchPosition - currentWorldPos;
            
            Vector3 newPosition = transform.position;
            newPosition.x += difference.x;
            newPosition.z += difference.z;
            
            // Sınırları uygula
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);
            
            // Y pozisyonunu sabit tut
            newPosition.y = 16f;
            
            transform.position = newPosition;
        }
        
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }
    #endif
    
    // Kamerayı merkeze döndür
    public void CenterCamera()
    {
        transform.position = new Vector3(0, 16, -6);
    }
}
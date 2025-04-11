using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.SceneManagement;

public class CameraMovementHandler : MonoBehaviour
{
    private CinemachineVirtualCamera VirtualCamera;

    // Movement
    public float CameraSpeed = 5f;
    private InputAction.CallbackContext callbackContextMovement;
    private bool isMoving;

    private float horizontalCallbackValue;
    private float verticalCallbackValue;
    private Vector3 directionCallbackValue;

    public float HorizontalRestriction = 10f;
    public float MinHorizontalRestriction = 10f;
    public float MaxHorizontalRestriction = 19f;
    public float VerticalRestriction = 5f;
    public float MinVerticalRestriction = 5f;
    public float MaxVerticalRestriction = 12f;
    private float cameraVelocity = 0f;
    public float CameraSmoothTime = 0.1f;

    // Scroll
    private float zoom;
    private float zoomMultiplier = 0.01f;
    public float MinZoom = 15f;
    public float MaxZoom = 20f;
    private float zoomVelocity = 0f;
    private float zoomSmoothTime = 0.25f;
    private InputAction.CallbackContext _callbackContextZoom;
    private bool isZooming;

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindVirtualCamera();
        SetCameraFollowTarget();
    }

    private void Start()
    {
        FindVirtualCamera();
        SetCameraFollowTarget();
    }

    private void Update()
    {
        if (callbackContextMovement.phase != InputActionPhase.Waiting)
        {
            horizontalCallbackValue = callbackContextMovement.ReadValue<Vector2>().x;
            verticalCallbackValue = callbackContextMovement.ReadValue<Vector2>().y;
            directionCallbackValue = new Vector3(horizontalCallbackValue, 0, verticalCallbackValue).normalized;
        }
    }

    private void LateUpdate()
    {
        Movement();
        Zooming();
        AdjustRestrictions();
        CameraRestrictions();
    }

    public void OnMove(InputAction.CallbackContext _context)
    {
        callbackContextMovement = _context;

        if (_context.canceled)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }
    }

    public void OnZoom(InputAction.CallbackContext _context)
    {
        _callbackContextZoom = _context;

        if (_context.canceled)
        {
            isZooming = false;
        }
        else
        {
            isZooming = true;
        }
    }

    void AdjustRestrictions()
    {
        float _zoomValue = Mathf.InverseLerp(MinZoom, MaxZoom, zoom);
        HorizontalRestriction = Mathf.Lerp(MaxHorizontalRestriction, MinHorizontalRestriction, _zoomValue);
        VerticalRestriction = Mathf.Lerp(MaxVerticalRestriction, MinVerticalRestriction, _zoomValue);
        cameraVelocity = directionCallbackValue.magnitude;
    }

    void Zooming()
    {
        if (isZooming && VirtualCamera != null)
        {
            float scroll = _callbackContextZoom.ReadValue<float>();
            zoom -= scroll * zoomMultiplier;
            zoom = Mathf.Clamp(zoom, MinZoom, MaxZoom);
            VirtualCamera.m_Lens.OrthographicSize = Mathf.SmoothDamp(VirtualCamera.m_Lens.OrthographicSize, zoom, ref zoomVelocity, zoomSmoothTime);
        }
    }

    void Movement()
    {
        if (isMoving)
        {
            transform.Translate(CameraSpeed * Time.deltaTime * directionCallbackValue);
        }
    }

    void CameraRestrictions()
    {
        if (transform.position.x >= HorizontalRestriction)
        {
            transform.position = new Vector3(Mathf.SmoothDamp(transform.position.x, HorizontalRestriction, ref cameraVelocity, CameraSmoothTime), transform.position.y, transform.position.z);
            if (horizontalCallbackValue > 0)
            {
                directionCallbackValue.x = 0;
            }
        }
        if (transform.position.x <= -HorizontalRestriction)
        {
            transform.position = new Vector3(Mathf.SmoothDamp(transform.position.x, -HorizontalRestriction, ref cameraVelocity, CameraSmoothTime), transform.position.y, transform.position.z);
            if (horizontalCallbackValue < 0)
            {
                directionCallbackValue.x = 0;
            }
        }

        if (transform.position.z >= VerticalRestriction)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.SmoothDamp(transform.position.z, VerticalRestriction, ref cameraVelocity, CameraSmoothTime));
            if (verticalCallbackValue > 0)
            {
                directionCallbackValue.z = 0;
            }
        }
        if (transform.position.z <= -VerticalRestriction)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.SmoothDamp(transform.position.z, -VerticalRestriction, ref cameraVelocity, CameraSmoothTime));
            if (verticalCallbackValue < 0)
            {
                directionCallbackValue.z = 0;
            }
        }
    }

    private void FindVirtualCamera()
    {
        // Najpierw szukaj po nazwie "Combat Cameras"
        GameObject combatCameras = GameObject.Find("Combat Cameras");
        if (combatCameras != null)
        {
            Transform cmVcam1 = combatCameras.transform.Find("CM vcam1");
            if (cmVcam1 != null)
            {
                VirtualCamera = cmVcam1.GetComponent<CinemachineVirtualCamera>();
                if (VirtualCamera != null)
                {
                    zoom = VirtualCamera.m_Lens.OrthographicSize;
                    return;
                }
            }
        }

        // Fallback – szukaj po tagu "VirtualCamera"
        GameObject cameraObj = GameObject.FindGameObjectWithTag("VirtualCamera");
        if (cameraObj != null)
        {
            VirtualCamera = cameraObj.GetComponent<CinemachineVirtualCamera>();
            if (VirtualCamera != null)
            {
                zoom = VirtualCamera.m_Lens.OrthographicSize;
            }
        }
    }

    private void SetCameraFollowTarget()
    {
        if (VirtualCamera != null)
        {
            Transform cube = transform.Find("Cube");
            if (cube != null)
            {
                VirtualCamera.Follow = cube;
            }
        }
    }
}
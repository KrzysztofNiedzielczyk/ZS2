using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamerasSynchronizer : MonoBehaviour
{
    public CinemachineVirtualCamera VirtualCamera;
    public Camera WorldUICamera;
    public Camera HighlightCamera;

    private void Start()
    {
        VirtualCamera = GameObject.FindGameObjectWithTag("VirtualCamera").GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        WorldUICamera.orthographicSize = VirtualCamera.m_Lens.OrthographicSize;
        HighlightCamera.orthographicSize = VirtualCamera.m_Lens.OrthographicSize;
    }
}

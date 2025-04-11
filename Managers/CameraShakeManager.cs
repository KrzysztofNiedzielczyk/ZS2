using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance;
    [SerializeField] private float globalShakeForce = 1f;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void CameraShake(CinemachineImpulseSource _impulseSource)
    {
        _impulseSource.GenerateImpulseWithForce(globalShakeForce);
    }
}

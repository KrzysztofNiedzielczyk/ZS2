using UnityEngine;
using Cinemachine;
using System.Collections;

public class CameraFollowController : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;

    void Awake()
    {
        // Znajd� Cinemachine Virtual Camera w scenie
        virtualCamera = FindFirstObjectByType<CinemachineVirtualCamera>();
        if (virtualCamera == null)
        {
            Debug.LogError("No Cinemachine Virtual Camera found in the scene!");
            return;
        }
    }

    IEnumerator Start()
    {
        // Czekaj, a� TruckManager.Instance b�dzie dost�pny
        yield return new WaitUntil(() => TruckManager.Instance != null);

        // Przypisz ci�ar�wk� do kamery
        AssignTruckToCamera();
    }

    void AssignTruckToCamera()
    {
        Transform truckTransform = TruckManager.Instance.transform;
        if (truckTransform != null)
        {
            virtualCamera.Follow = truckTransform;
            Debug.Log("Cinemachine camera assigned to follow the truck in DrivingScene.");
        }
        else
        {
            Debug.LogError("TruckManager.Instance.transform is null!");
        }
    }

    // Opcjonalne: R�czne wywo�anie w razie potrzeby (np. debugowanie)
    public void UpdateFollowTarget()
    {
        AssignTruckToCamera();
    }
}
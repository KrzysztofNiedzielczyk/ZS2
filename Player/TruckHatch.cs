using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class TruckHatch : MonoBehaviour
{
    private Quaternion closedRotation;
    private Quaternion openRotation;

    void Awake()
    {
        closedRotation = transform.localRotation; // Pozycja zamkniêta (np. X=0)
        openRotation = Quaternion.Euler(-180f, 0f, 0f) * closedRotation; // Pozycja otwarta (X=-180)
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Main Scene")
        {
            StartCoroutine(OpenHatch(1.5f)); // Otwórz w scenach walki
        }
        else
        {
            StartCoroutine(CloseHatch(1.5f)); // Zamknij w Main Scene
        }
    }

    IEnumerator OpenHatch(float duration)
    {
        float time = 0f;
        Quaternion startRotation = transform.localRotation;

        while (time < duration)
        {
            transform.localRotation = Quaternion.Slerp(startRotation, openRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = openRotation; // Koñcowa pozycja otwarta
    }

    IEnumerator CloseHatch(float duration)
    {
        float time = 0f;
        Quaternion startRotation = transform.localRotation;

        while (time < duration)
        {
            transform.localRotation = Quaternion.Slerp(startRotation, closedRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localRotation = closedRotation; // Koñcowa pozycja zamkniêta
    }
}
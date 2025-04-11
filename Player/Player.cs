using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour, IStunnable
{
    private static Player _instance;
    public static Player Instance
    {
        get
        {
            if (_instance == null)
            {
                TruckManager truck = FindObjectOfType<TruckManager>();
                if (truck != null)
                {
                    _instance = truck.GetComponentInChildren<Player>(true);
                }
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<Player>();
                }
                if (_instance == null)
                {
                    GameObject playerObj = new GameObject("Player");
                    _instance = playerObj.AddComponent<Player>();
                }
                // Usuniêto DontDestroyOnLoad – za³atwia to TruckManager
            }
            return _instance;
        }
    }

    private Coroutine stunnedCoroutine;
    public WeaponShooting WeaponShooting;
    public WeaponRotation WeaponRotation;

    public bool Stunned = false;

    private Vector3 defaultPosition;

    void Awake()
    {
        defaultPosition = transform.localPosition;
        transform.localPosition = defaultPosition + new Vector3(0, -1f, 0);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Main Scene" && gameObject.activeSelf)
        {
            StartCoroutine(EmergeFromHatch(1.5f));
        }
    }

    private void Start()
    {
        WeaponShooting = GetComponent<WeaponShooting>();
        WeaponRotation = GetComponent<WeaponRotation>();
    }

    public virtual void GetStunned(float stunDuration, GameObject stunEffect)
    {
        Stunned = true;
        GameObject _stunEffect = Instantiate(stunEffect, transform.position + new Vector3(0, 1, 0), Quaternion.identity, transform);
        Destroy(_stunEffect, stunDuration);

        if (stunnedCoroutine != null)
        {
            StopCoroutine(stunnedCoroutine);
        }

        stunnedCoroutine = StartCoroutine(StunProcess(stunDuration, stunEffect));
    }

    public virtual IEnumerator StunProcess(float stunDuration, GameObject stunEffect)
    {
        yield return new WaitForSeconds(stunDuration);
        Stunned = false;
    }

    IEnumerator EmergeFromHatch(float duration)
    {
        float time = 0f;
        Vector3 startPosition = transform.localPosition;
        Vector3 endPosition = startPosition + new Vector3(0, 1f, 0);

        while (time < duration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = endPosition;
    }
}
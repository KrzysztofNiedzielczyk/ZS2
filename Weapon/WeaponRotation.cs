using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponRotation : MonoBehaviour
{
    private Transform _mousePosition;
    [SerializeField] private float _rotationSpeed = 10f;
    private Quaternion _lookRotation;
    private Vector3 _direction;
    public Transform Gun;

    public Player Player;
    public WeaponShooting WeaponShooting;

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
        if (scene.name != "Main Scene")
        {
            FindMousePosition();
        }
    }

    private void Start()
    {
        FindMousePosition();
        Player = GetComponent<Player>();
        WeaponShooting = GetComponent<WeaponShooting>();
    }

    void Update()
    {
        Rotate();
    }

    void Rotate()
    {
        if (Player.Stunned == false && _mousePosition != null)
        {
            _direction = (_mousePosition.position - Gun.position).normalized;
            _lookRotation = Quaternion.LookRotation(_direction);
            Gun.rotation = Quaternion.Slerp(Gun.rotation, _lookRotation, Time.deltaTime * _rotationSpeed);
        }
    }

    private void FindMousePosition()
    {
        GameObject mouseObj = GameObject.FindGameObjectWithTag("MousePosition");
        if (mouseObj != null)
        {
            _mousePosition = mouseObj.transform;
        }
    }
}
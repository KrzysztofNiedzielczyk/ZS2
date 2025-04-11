using UnityEngine;
using System.Collections.Generic;

public class BaseBarricadePart : MonoBehaviour
{
    public ParticleSystem DamageSmoke;
    public List<Transform> DamagePlaces;

    private Vector3 originalLocalPosition;
    private float shakeDuration = 0f;
    private float shakeMagnitude = 0.1f;
    private float shakeTimeRemaining = 0f;

    void Start()
    {
        originalLocalPosition = transform.localPosition; // Zapisz oryginaln� pozycj� lokaln�
    }

    void Update()
    {
        if (shakeTimeRemaining > 0)
        {
            // Trz�sienie tylko w przestrzeni lokalnej
            transform.localPosition = originalLocalPosition + Random.insideUnitSphere * shakeMagnitude;
            shakeTimeRemaining -= Time.deltaTime;
            if (shakeTimeRemaining <= 0)
            {
                transform.localPosition = originalLocalPosition; // Przywr�� oryginaln� pozycj� lokaln�
            }
        }
    }

    public void IsDamaged()
    {
        int _index = Random.Range(0, DamagePlaces.Count);
        Instantiate(DamageSmoke, DamagePlaces[_index].position, Quaternion.identity);

        // Uruchom efekt trz�sienia
        shakeTimeRemaining = shakeDuration = 0.5f;
    }
}
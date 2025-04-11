using UnityEngine;
using System.Collections.Generic;

public class BaseBarricade : MonoBehaviour, IDamageable
{
    public float MaxHealth = 30f;
    private float currentHealth;
    private Rigidbody rb;

    [Header("Damage Effects")]
    public ParticleSystem DamageSmoke;
    public List<Transform> DamagePlaces;
    public float ShakeMagnitude = 0.1f;
    public float ShakeDuration = 0.5f;
    private float shakeTimeRemaining = 0f;
    private Vector3 originalLocalPosition;
    private DefenceItem defenceItem; // Referencja do DefenceItem

    void Start()
    {
        currentHealth = MaxHealth;
        Debug.Log($"Barykada zainicjowana z HP: {currentHealth}/{MaxHealth}");

        rb = gameObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;

        defenceItem = GetComponent<DefenceItem>();
        if (defenceItem == null)
        {
            Debug.LogError("Brak komponentu DefenceItem na barykadzie!");
        }
    }

    void Update()
    {
        // Zapisz pozycjê lokaln¹ po rozstawieniu
        if (defenceItem != null && defenceItem.isPlaced && shakeTimeRemaining == 0 && originalLocalPosition == Vector3.zero)
        {
            originalLocalPosition = transform.localPosition;
            Debug.Log($"Zapisano oryginaln¹ pozycjê lokaln¹: {originalLocalPosition}");
        }

        if (shakeTimeRemaining > 0)
        {
            transform.localPosition = originalLocalPosition + Random.insideUnitSphere * ShakeMagnitude;
            shakeTimeRemaining -= Time.deltaTime;
            if (shakeTimeRemaining <= 0)
            {
                transform.localPosition = originalLocalPosition;
                Debug.Log($"Trzêsienie zakoñczone, pozycja lokalna przywrócona: {transform.localPosition}");
            }
        }
    }

    public void TakeDamage(float amount)
    {
        Debug.Log($"Barykada otrzymuje {amount} obra¿eñ. Aktualne HP przed: {currentHealth}");
        currentHealth -= amount;

        if (currentHealth > 0)
        {
            TriggerDamageEffects();
        }

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            DestroyBarricade();
        }
        Debug.Log($"Pozosta³e HP po obra¿eniach: {currentHealth}/{MaxHealth}");
    }

    private void TriggerDamageEffects()
    {
        if (DamageSmoke != null && DamagePlaces != null && DamagePlaces.Count > 0)
        {
            int index = Random.Range(0, DamagePlaces.Count);
            Instantiate(DamageSmoke, DamagePlaces[index].position, Quaternion.identity);
        }

        shakeTimeRemaining = ShakeDuration;
    }

    private void DestroyBarricade()
    {
        Debug.Log("Barykada zniszczona!");
        Destroy(gameObject);
    }
}
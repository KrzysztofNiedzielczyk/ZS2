using UnityEngine;

public enum DefenceType { FireBarrel, Spotlight, Mine, BarbedWire, Barricade, ExplosiveBarrel }

public class DefenceItem : MonoBehaviour
{
    public DefenceType type;
    public int scrapMetalCost;
    public int fuelCost;
    public int ammunitionCost;
    public float health; // Dla barykady
    public bool isPlaced = false; // Czy ju¿ rozstawione
    public float cooldown; // Dla min (czas odnowienia)
    private float cooldownTimer;

    void Update()
    {
        if (type == DefenceType.Mine && isPlaced)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                Explode(); // Wybuch miny
                cooldownTimer = cooldown; // Reset odnowienia
            }
        }
    }

    void Explode()
    {
        // Logika wybuchu miny – obra¿enia w promieniu
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5f);
        foreach (var hit in hitColliders)
        {
            if (hit.CompareTag("Zombie")) // Zak³adam tag "Zombie"
            {
                // Zadaj obra¿enia zombie (np. 50)
                Debug.Log("Mina wybuch³a – zombie trafione!");
            }
        }
    }

    public void OnHit() // Dla eksploduj¹cej beczki
    {
        if (type == DefenceType.ExplosiveBarrel)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 7f);
            foreach (var hit in hitColliders)
            {
                if (hit.CompareTag("Zombie"))
                {
                    // Zadaj obra¿enia (np. 75)
                    Debug.Log("Eksploduj¹ca beczka wybuch³a!");
                }
            }
            Destroy(gameObject); // Usuñ po wybuchu
        }
    }
}
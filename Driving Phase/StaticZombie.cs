using UnityEngine;

public class StaticZombie : MonoBehaviour, IDamageable
{
    public float MaxHealth = 50f; // Mniejsza wytrzyma�o��, bo to przeszkoda
    public float Health;
    public ParticleSystem BloodSplashFX; // Efekt krwi przy trafieniu
    public ParticleSystem DieGibbletsFX; // Efekt �mierci
    public ParticleSystem DieBloodExplosionFX; // Efekt wybuchu krwi
    public bool IsDead = false;
    public Animator Animator; // Komponent animacji (opcjonalny, dla pewno�ci)

    void Start()
    {
        Health = MaxHealth;
        // Nie ustawiamy parametru IsIdle, bo Idle jest domy�lne
    }

    public virtual void TakeDamage(float amount)
    {
        if (IsDead) return;

        Health -= amount;

        if (Health <= 0)
        {
            IsDead = true;
            Die();
        }
    }

    public virtual void Die()
    {
        if (DieGibbletsFX != null)
            Instantiate(DieGibbletsFX, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        if (DieBloodExplosionFX != null)
            Instantiate(DieBloodExplosionFX, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
        Destroy(gameObject);
    }
}
using UnityEngine;

public class StaticZombie : MonoBehaviour, IDamageable
{
    public float MaxHealth = 50f; // Mniejsza wytrzyma³oœæ, bo to przeszkoda
    public float Health;
    public ParticleSystem BloodSplashFX; // Efekt krwi przy trafieniu
    public ParticleSystem DieGibbletsFX; // Efekt œmierci
    public ParticleSystem DieBloodExplosionFX; // Efekt wybuchu krwi
    public bool IsDead = false;
    public Animator Animator; // Komponent animacji (opcjonalny, dla pewnoœci)

    void Start()
    {
        Health = MaxHealth;
        // Nie ustawiamy parametru IsIdle, bo Idle jest domyœlne
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
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowerRock : MonoBehaviour, IDamageable, IAimable
{
    public float Damage = 200f;
    public float Health = 100f;
    public float MaxHealth = 100f;
    public float StunDuration = 3f;
    public GameObject StunEffect;
    public GameObject RockImpactFX;
    public List<GameObject> RockImpactAudio = new List<GameObject>();
    public CinemachineImpulseSource cameraImpulseSource;
    public bool IsDead = false;
    public ParticleSystem DieGibbletsFX;
    public ParticleSystem DieBloodExplosionFX;

    [field: SerializeField]
    public Transform AimTransform { get; set; }

    private void Start()
    {
        Health = MaxHealth;
        Destroy(gameObject, 10);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 8)
        {
            GameObject _playerObject = other.gameObject;


            if (_playerObject.transform.TryGetComponent(out Player _player))
            {
                _player.GetStunned(StunDuration, StunEffect);

                int _index = Random.Range(0, RockImpactAudio.Count);

                GameObject _explosion = Instantiate(RockImpactFX, transform.position, Quaternion.Euler(0, 0, 0));
                GameObject _explosionAudio = Instantiate(RockImpactAudio[_index], transform.position, Quaternion.identity);
                CameraShakeManager.Instance.CameraShake(cameraImpulseSource);

                Destroy(_explosion, 5f);
                Destroy(_explosionAudio, 10f);
                Destroy(gameObject);
            }
        }
    }

    public virtual void TakeDamage(float amount)
    {
        Health -= amount;

        if (IsDead == false && Health <= 0)
        {
            IsDead = true;
            Die();
        }
    }

    public virtual void Die()
    {
        Instantiate(DieGibbletsFX, transform.position, Quaternion.identity);
        Instantiate(DieBloodExplosionFX, transform.position, Quaternion.identity);
        EventManager.OnEnemyDied(gameObject);
        Destroy(gameObject);
    }
}

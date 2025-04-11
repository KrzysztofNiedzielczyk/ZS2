using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float RotationSpeed;
    public float Damage;
    public float FireRate;
    public float Range;
    public int PenetrationLevel;
    public float StaggeringPower;
    public int AmmoAmount;
    public float ReloadTime;

    public Transform Muzzle;
    public GameObject ImpactEffect;
    public GameObject ImpactEffectMetal;
    public TrailRenderer GunBulletTrail;
    public List<ParticleSystem> MuzzleFlash = new List<ParticleSystem>();
    public List<AudioSource> ShootAudio = new List<AudioSource>();
    public List<AudioSource> WeaponReloadSFXList = new List<AudioSource>();
    public List<AudioSource> WeaponFoleySFXList = new List<AudioSource>();
}

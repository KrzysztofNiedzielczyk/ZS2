using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalZombie : Enemy
{
    // die gibblets instantiated as gameobjects
    /*public override void Die()
    {
        for(int i = 0; i < DieGibletsNumber; i++)
        {
            GameObject _dieGibblet = Instantiate(DieGibblets[i], transform.position + new Vector3(Random.Range(-0.5f, 0.5f), 1 + Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)), Quaternion.identity);
            Destroy(_dieGibblet, 5f);
        }
        Destroy(gameObject);
    }*/
}

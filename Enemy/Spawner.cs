using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject EnemyPrefab;
    public Transform DestinationTarget;
    public float SpawnSpeed;
    public float SpawnAmount;
    public bool AmountChanger = false;

    private void Start()
    {
        DestinationTarget = GameObject.FindGameObjectWithTag("DestinationTarget").transform;
        StartCoroutine(SpawnCycle());
        StartCoroutine(IncreaseDifficulty());
    }

    IEnumerator SpawnCycle()
    {
        while (true)
        {
            for (int i = 0; i < SpawnAmount; i++)
            {
                float _randomX = Random.Range(-5.0f, 5.0f);
                float _randomZ = Random.Range(-5.0f, 5.0f);

                GameObject _enemy = Instantiate(EnemyPrefab, transform.position + new Vector3(_randomX, 0, _randomZ), Quaternion.identity);
                _enemy.GetComponent<Enemy>().AIDestinationSetter.target = DestinationTarget;
            }

            yield return new WaitForSeconds(SpawnSpeed);
        }
    }
    IEnumerator IncreaseDifficulty()
    {
        while (true)
        {
            if (AmountChanger)
            {
                yield return new WaitForSeconds(60);

                SpawnAmount++;
            }
            else
            {
                yield return new WaitForSeconds(60);

                if (SpawnSpeed > 10)
                {
                    SpawnSpeed--;
                }
                else
                {
                    SpawnAmount++;
                }
            }
        }
    }
}

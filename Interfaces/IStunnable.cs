using System.Collections;
using UnityEngine;

public interface IStunnable
{
    void GetStunned(float stunDuration, GameObject stunEffect)
    {

    }

    IEnumerator StunProcess(float stunDuration, GameObject stunEffect)
    {
        yield return new WaitForSeconds(stunDuration);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : MonoBehaviour, IPenetrable
{
    [field: SerializeField]
    public bool IsPenetrable { get; set; } = true;
}

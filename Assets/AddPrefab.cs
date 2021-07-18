using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddPrefab : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] Transform parentTransform;

    public void Add()
    {
        GameObject clone = Instantiate(prefab);
        clone.transform.parent = parentTransform;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField] string key;
    [SerializeField] GameObject activateOnDisable;
    void OnEnable()
    {
        if (PlayerPrefs.GetInt(key, 0) != 0)
        {
            gameObject.SetActive(false);
        }
        else PlayerPrefs.SetInt(key, 1);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        if(activateOnDisable != null)
        activateOnDisable.SetActive(true);
    }
}

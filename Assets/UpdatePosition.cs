using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePosition : MonoBehaviour
{
    bool exec = false;
    // Start is called before the first frame update
    void LateUpdate()
    {
        if(exec)
         transform.position = new Vector3(transform.position.x, transform.position.y + 0.0001f, transform.position.z);
        else transform.position = new Vector3(transform.position.x, transform.position.y - 0.0001f, transform.position.z);

        exec = !exec;
    }
}

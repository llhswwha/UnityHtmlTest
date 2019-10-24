using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{

    public Vector3 angle = Vector3.zero;

    // Update is called once per frame
    void Update()
    {

        if (angle != Vector3.zero)
        {
            transform.Rotate(angle);
        }
        
    }
}

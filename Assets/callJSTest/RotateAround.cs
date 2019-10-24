using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
    public Transform Center;

    // Start is called before the first frame update
    void Start()
    {

    }

    public Vector3 axis = Vector3.zero;

    public float angle = 0;

    // Update is called once per frame
    void Update()
    {
        if (angle != 0)
        {
            transform.RotateAround(Center.position, axis, angle);
        }

    }
}

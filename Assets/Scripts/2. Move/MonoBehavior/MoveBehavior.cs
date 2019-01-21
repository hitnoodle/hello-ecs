using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBehavior : MonoBehaviour
{
    public float SpeedX;
    public float SpeedZ;

    public float TopBound;
    public float BottomBound;
    public float LeftBound;
    public float RightBound;

    void Update()
    {
        float deltaTime     = Time.deltaTime;
        Vector3 value       = transform.position;

        if (value.z <= BottomBound || value.z >= TopBound)
        {
            SpeedZ *= -1;
        }

        if (value.x <= LeftBound || value.x >= RightBound)
        {
            SpeedX *= -1;
        }

        value.x += deltaTime * SpeedX;
        value.z += deltaTime * SpeedZ;

        transform.position = value;
    }
}

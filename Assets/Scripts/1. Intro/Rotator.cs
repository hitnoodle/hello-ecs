using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float Speed;
}

class RotatorSystem : ComponentSystem
{
    struct Components
    {
        public Rotator Rotator;
        public Transform Transform;
    }

    protected override void OnUpdate()
    {
        float deltaTime = Time.deltaTime;
        ComponentGroupArray<Components> components = GetEntities<Components>();

        for (int i = 0; i < components.Length; i++)
        {
            Components c = components[i];
            c.Transform.Rotate(0f, c.Rotator.Speed * deltaTime, 0f);
        }

        components.Dispose();
    }
}

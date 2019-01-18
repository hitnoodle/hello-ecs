using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[System.Serializable]
public struct MoveSpeed : IComponentData
{
    public float X;
    public float Z;
}

public class MoveSpeedComponent : ComponentDataWrapper<MoveSpeed> { }

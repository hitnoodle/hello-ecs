using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Boundary
{
    public Vector3 Position { get; private set; }
    public Vector3 Size     { get; private set; }

    public float Top        { get; private set; }
    public float Bottom     { get; private set; }
    public float Right      { get; private set; }
    public float Left       { get; private set; }
    public float Forward    { get; private set; }
    public float Back       { get; private set; }

    public Boundary(Vector3 pos, Vector3 size)
    {
        Position    = pos;
        Size        = size;

        float halfHeight    = size.y / 2;
        float halfWidth     = size.x / 2;
        float halfForward   = size.z / 2;

        Top     = pos.y + halfHeight;
        Bottom  = pos.y - halfHeight;
        Right   = pos.x + halfWidth;
        Left    = pos.x - halfWidth;
        Forward = pos.z + halfForward;
        Back    = pos.z - halfForward;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct BoidParameter
{
    public float MinSpeed;
    public float MaxSpeed;
    public float InitialSpeed;
    public float WallDistance;
    public float WallWeight;
    public float NeighbourFOV;
    public float NeighbourDistance;
    public float SeparationWeight;
    public float AlignmentWeight;
    public float CohesionWeight;
}

public class Boid : MonoBehaviour
{
    public static List<Boid> AllBoids;

    public Vector3 Position { get; private set; }
    public Vector3 Velocity { get; private set; }

    private Boundary        _Boundary;
    private BoidParameter   _Parameter;
    private Transform       _Transform;
    private Vector3         _Acceleration;
    private List<Boid>      _Neighbours;
    private bool            _IsInitialized;

    public void Initialize(Boundary boundary, BoidParameter parameter)
    {
        _Boundary       = boundary;
        _Parameter      = parameter;
        _Transform      = transform;
        _Acceleration   = Vector3.zero;
        _Neighbours     = new List<Boid>();
        _IsInitialized  = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        Position = _Transform.position;
        Velocity = _Transform.forward * _Parameter.InitialSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_IsInitialized)
        { return; }

        UpdateNeighbours();
        UpdateWalls();
        UpdateSeparation();
        UpdateAlignment();
        UpdateCohesion();
        UpdateMove();
    }

    void UpdateNeighbours()
    {
        _Neighbours.Clear();

        float eyeAngleThreshold     = Mathf.Cos(_Parameter.NeighbourFOV * Mathf.Deg2Rad);
        float distanceThreshold     = _Parameter.NeighbourDistance;
        Vector3 velocityDirection   = Velocity.normalized;

        int totalCount = AllBoids.Count;
        for (int i = 0; i < totalCount; i++)
        {
            Boid other = AllBoids[i];

            if (other == this)
            { continue; }

            Vector3 distanceVector  = other.Position - Position;
            float distance          = distanceVector.magnitude;

            if (distance < distanceThreshold)
            {
                Vector3 direction   = distanceVector.normalized;
                float angle         = Vector3.Dot(velocityDirection, direction);
                if (angle > eyeAngleThreshold)
                {
                    _Neighbours.Add(other);
                }
            }
        }
    }

    private void UpdateWalls()
    {
        _Acceleration +=    CalculateAccelAgainstBoundary(_Boundary.Left    - Position.x, Vector3.right)    +
                            CalculateAccelAgainstBoundary(_Boundary.Bottom  - Position.y, Vector3.up)       +
                            CalculateAccelAgainstBoundary(_Boundary.Back    - Position.z, Vector3.forward)  +
                            CalculateAccelAgainstBoundary(_Boundary.Right   - Position.x, Vector3.left)     +
                            CalculateAccelAgainstBoundary(_Boundary.Top     - Position.y, Vector3.down)     +
                            CalculateAccelAgainstBoundary(_Boundary.Forward - Position.z, Vector3.back);
    }

    private Vector3 CalculateAccelAgainstBoundary(float distance, Vector3 direction)
    {
        if (distance < _Parameter.WallDistance)
        {
            return direction * (_Parameter.WallWeight / Mathf.Abs(distance / _Parameter.WallDistance));
        }

        return Vector3.zero;
    }

    private void UpdateSeparation()
    {
        if (_Neighbours.Count == 0)
        { return; }

        Vector3 force = Vector3.zero;
        for (int i = 0; i < _Neighbours.Count; i++)
        {
            force += (Position - _Neighbours[i].Position).normalized;
        }

        _Acceleration += force * _Parameter.SeparationWeight;
    }

    private void UpdateAlignment()
    {
        if (_Neighbours.Count == 0)
        { return; }

        Vector3 averageVelocity = Vector3.zero;
        for (int i = 0; i < _Neighbours.Count; i++)
        {
            averageVelocity += _Neighbours[i].Velocity;
        }
        averageVelocity /= _Neighbours.Count;

        _Acceleration += (averageVelocity - Velocity) * _Parameter.AlignmentWeight;
    }

    private void UpdateCohesion()
    {
        if (_Neighbours.Count == 0)
        { return; }

        Vector3 averagePos = Vector3.zero;
        for (int i = 0; i < _Neighbours.Count; i++)
        {
            averagePos += _Neighbours[i].Position;
        }
        averagePos /= _Neighbours.Count;

        _Acceleration += (averagePos - Position) * _Parameter.CohesionWeight;
    }

    private void UpdateMove()
    {
        float dt = Time.deltaTime;

        Velocity            += _Acceleration * dt;
        Vector3 direction   = Velocity.normalized;
        float speed         = Velocity.magnitude;
        Velocity            = Mathf.Clamp(speed, _Parameter.MinSpeed, _Parameter.MaxSpeed) * direction;
        Position            += Velocity * dt;

        Quaternion rot = Quaternion.LookRotation(Velocity);
        _Transform.SetPositionAndRotation(Position, rot);

        _Acceleration = Vector3.zero;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviorSceneController : MonoBehaviour
{
    [Header("Bounds")]
    public Transform TopLeft;
    public Transform TopRight;
    public Transform BottomLeft;
    public Transform BottomRight;

    [Header("Entities")]
    public GameObject EntityPrefab;
    public int EntityCount = 0;
    public int EntityInitialCount = 1000;
    public int EntityIncrement = 500;
    public Vector2 SpeedMin;
    public Vector2 SpeedMax;

    [Header("UI")]
    public FPSText _FPS;

    private float _topBound;
    private float _bottomBound;
    private float _leftBound;
    private float _rightBound;

    private void Awake()
    {
        _topBound = TopRight.position.z;
        _bottomBound = BottomLeft.position.z;
        _leftBound = BottomLeft.position.x;
        _rightBound = TopRight.position.x;
    }

    // Start is called before the first frame update
    void Start()
    {
        AddEntity(EntityInitialCount);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AddEntity(EntityIncrement);
        }
    }

    void AddEntity(int count)
    {
        for (int i = 0; i < count; i++)
        {
            float xVal      = Random.Range(_leftBound, _rightBound);
            float zVal      = Random.Range(_bottomBound, _topBound);
            float xSpeed    = Random.Range(SpeedMin.x, SpeedMax.x);
            float zSpeed    = Random.Range(SpeedMin.y, SpeedMax.y);

            GameObject entityObject         = Instantiate(EntityPrefab);
            entityObject.transform.position = new Vector3(xVal, 0, zVal);

            MoveBehavior move   = entityObject.GetComponent<MoveBehavior>();
            move.SpeedX         = xSpeed;
            move.SpeedZ         = zSpeed;
            move.TopBound       = _topBound;
            move.BottomBound    = _bottomBound;
            move.LeftBound      = _leftBound;
            move.RightBound     = _rightBound;
        }

        EntityCount += count;
        _FPS.UpdateCount(EntityCount);
    }
}

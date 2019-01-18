using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FPSText : MonoBehaviour
{
    private TextMeshProUGUI _text;
    private int _entityCount;

    private int     _frameCount = 0;
    private float   _dt = 0f;
    private float   _updateRate = 4.0f;
    private float   _fps = 0f;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateCount(int count)
    {
        _entityCount = count;
    }

    // Update is called once per frame
    void Update()
    {
        _frameCount++;
        _dt += Time.deltaTime;
        if (_dt > 1.0f/_updateRate)
        {
            _fps        = _frameCount / _dt;
            _frameCount = 0;
            _dt         -= 1.0f / _updateRate;

            _text.text = string.Format("{0} {1}", _entityCount, _fps);
        }
    }
}

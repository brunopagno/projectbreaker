using System;
using UnityEngine;

public class Cooldown
{
    private float _timer;
    private float _duration;
    private bool ready = true;

    public bool Ready
    {
        get
        {
            return ready;
        }
    }

    public float Readyness
    {
        get
        {
            float ready = _timer / _duration;
            if (ready > 1) ready = 1;
            return ready;
        }
    }

    public Cooldown(float duration)
    {
        _duration = duration;
        _timer = duration;
    }

    public void Reset()
    {
        _timer = 0;
        ready = false;
    }

    public void Update()
    {
        if (ready) return;

        _timer += Time.deltaTime;
        if (_timer > _duration)
        {
            ready = true;
        }
    }
}

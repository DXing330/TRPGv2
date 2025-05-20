using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Timer : MonoBehaviour
{
    Stopwatch stopwatch = new Stopwatch();

    public void ResetTime()
    {
        stopwatch.Reset();
    }

    public void StartTime()
    {
        stopwatch.Start();
    }

    public void StopTime()
    {
        stopwatch.Stop();
    }

    public void ShowTime()
    {
        UnityEngine.Debug.Log((float)stopwatch.Elapsed.TotalMilliseconds);
    }
}

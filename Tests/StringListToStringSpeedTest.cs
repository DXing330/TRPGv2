using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class StringListToStringSpeedTest : MonoBehaviour
{
    public List<string> testList;
    public string delimiter = "|";

    [ContextMenu("JoinTest")]
    public float JoinTest()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        string combined = String.Join(delimiter, testList);
        stopwatch.Stop();
        UnityEngine.Debug.Log((float)stopwatch.Elapsed.TotalMilliseconds);
        return (float)stopwatch.Elapsed.TotalMilliseconds;
    }
    [ContextMenu("ForLoopTest")]
    public float ForLoopTest()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        string combined = "";
        for (int i = 0; i < testList.Count; i++)
        {
            combined += testList[i];
            if (i < testList.Count - 1) { combined += delimiter; }
        }
        stopwatch.Stop();
        UnityEngine.Debug.Log((float)stopwatch.Elapsed.TotalMilliseconds);
        return (float)stopwatch.Elapsed.TotalMilliseconds;
    }

    public float StringPlusOperatorTest()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        string sWord1 = "One";
        string sWord2 = "Two";
        string sWords = sWord1 + sWord2;
        stopwatch.Stop();
        return (float)stopwatch.Elapsed.TotalMilliseconds;
    }
    public float StringInterpolationTest()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        string sWord1 = "One";
        string sWord2 = "Two";
        string sWords = $"{sWord1}{sWord2}";
        stopwatch.Stop();
        return (float)stopwatch.Elapsed.TotalMilliseconds;
    }
    public float StringDotConcatTest()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        string sWord1 = "One";
        string sWord2 = "Two";
        string sWords = string.Concat(sWord1, sWord2);
        stopwatch.Stop();
        return (float)stopwatch.Elapsed.TotalMilliseconds;
    }
    public float StringDotFormatTest()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        string sWord1 = "One";
        string sWord2 = "Two";
        string sWords = System.String.Format("{0}{1}", sWord1, sWord2);
        stopwatch.Stop();
        return (float)stopwatch.Elapsed.TotalMilliseconds;
    }
}

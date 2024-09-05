using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscTester : MonoBehaviour
{
    public int testDir;
    public string CheckRelativeDirections(int dir1, int dir2)
    {
        int directionDiff = Mathf.Abs(dir1 - dir2);
        switch (directionDiff)
        {
            case 0:
            return "Same";
            case 1:
            return "Back";
            case 2:
            return "Face";
            case 3:
            return "Opposite";
            case 4:
            return "Face";
            case 5:
            return "Back";
        }
        return "None";
    }

    [ContextMenu("Check Directions")]
    public void CheckDirections()
    {
        for (int i = 0; i < 6; i++)
        {
            Debug.Log(CheckRelativeDirections(testDir, i));
        }
    }
}

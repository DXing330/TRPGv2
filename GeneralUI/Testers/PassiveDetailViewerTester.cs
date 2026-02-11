using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveDetailViewerTester : MonoBehaviour
{
    public StatDatabase allPassives;
    public PassiveDetailViewer detailViewer;
    public List<string> passiveGroupNames;
    public List<string> passiveLevels;
    public string allPassiveNames;
    public List<string> passiveNames;
    public string allPassiveInfo;
    public List<string> passiveInfo;
    public string testPassiveName;

    [ContextMenu("Test All Passives")]
    public void TestPassiveDescriptions()
    {
        passiveNames = new List<string>(allPassives.keys);
        passiveInfo = new List<string>(allPassives.values);
        for (int i = 0; i < passiveNames.Count; i++)
        {
            Debug.Log(passiveNames[i]);
            Debug.Log(detailViewer.ReturnPassiveDetails(passiveInfo[i]));
        }
    }

    [ContextMenu("Test Single Passive")]
    public void TestSinglePassiveDescription()
    {
        string testPassiveInfo = allPassives.ReturnValue(testPassiveName);
        Debug.Log(testPassiveName);
        Debug.Log(detailViewer.ReturnPassiveDetails(testPassiveInfo));
    }
}

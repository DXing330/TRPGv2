using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatabaseInitializer : MonoBehaviour
{
    public List<StatDatabase> statData;

    [ContextMenu("Initialize")]
    public void Initialize()
    {
        for (int i = 0; i < statData.Count; i++)
        {
            statData[i].Initialize();
        }
    }
}

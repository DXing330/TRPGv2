using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
    using UnityEditor;
#endif

public class DatabaseInitializer : MonoBehaviour
{
    public List<StatDatabase> statData;

    [ContextMenu("Initialize")]
    public void Initialize()
    {
        for (int i = 0; i < statData.Count; i++)
        {
            statData[i].Initialize();
            #if UNITY_EDITOR
                EditorUtility.SetDirty(statData[i]);
            #endif
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiLevelDBInitializer : MonoBehaviour
{
    public List<DatabaseInitializer> allDBInit;
    public List<SpriteContainer> spriteContainers;

    [ContextMenu("Initialize")]
    public void Initialize()
    {
        for (int i = 0; i < allDBInit.Count; i++)
        {
            allDBInit[i].Initialize();
        }
        for (int i = 0; i < spriteContainers.Count; i++)
        {
            spriteContainers[i].Initialize();
        }
    }
}

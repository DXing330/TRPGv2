using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
    using UnityEditor;
#endif


[CreateAssetMenu(fileName = "SpriteContainer", menuName = "ScriptableObjects/DataContainers/SpriteContainer", order = 1)]
public class SpriteContainer : ScriptableObject
{
    public bool elevationDifferences = false;
    public bool copySprites = false;
    public SpriteContainer copiedSprites;
    public GeneralUtility utility;
    public List<Sprite> sprites;
    public string allKeysAndValues;
    public virtual void SetAllData(string newInfo)
    {
        allKeysAndValues = newInfo;
    }
    public string delimiter;
    public string delimiterTwo;
    public virtual void Initialize()
    {
        string[] blocks = allKeysAndValues.Split(delimiter);
        keys = blocks[0].Split(delimiterTwo).ToList();
        values = blocks[1].Split(delimiterTwo).ToList();
        if (copySprites)
        {
            keys = new List<string>(copiedSprites.keys);
            values = new List<string>(copiedSprites.values);
            sprites = copiedSprites.sprites;
        }
        #if UNITY_EDITOR
                EditorUtility.SetDirty(this);
        #endif
    }
    public List<string> keys;
    public string RandomSpriteName()
    {
        return sprites[Random.Range(0, sprites.Count)].name;
    }
    public List<string> values;

    public string SpriteNameByIndex(int index)
    {
        if (index < 0 || index >= sprites.Count){return "";}
        return sprites[index].name;
    }

    public Sprite SpriteDictionary(string spriteName)
    {
        // Maybe try using a key first.
        int indexOf = keys.IndexOf(spriteName);
        if (indexOf >= 0)
        {
            spriteName = values[indexOf];
        }
        for (int i = 0; i < sprites.Count; i++)
        {
            if (sprites[i].name == spriteName){return sprites[i];}
        }
        if (elevationDifferences)
        {
            if (!spriteName.Contains("E"))
            {
                spriteName += "E0";
            }
        }
        for (int i = 0; i < sprites.Count; i++)
        {
            if (sprites[i].name == spriteName){return sprites[i];}
        }
        if (elevationDifferences)
        {
            // Try to use a lower elevation.
            string[] blocks = spriteName.Split("E");
            int elevation = int.Parse(blocks[1]);
            if (elevation > 0)
            {
                return SpriteDictionary(blocks[0]+"E"+(elevation-1));
            }
        }
        return null;
    }

    public Sprite SpriteByIndex(int index)
    {
        if (index < 0 || index >= sprites.Count){return null;}
        return sprites[index];
    }

    public Sprite SpriteByKey(string nKey)
    {
        int indexOf = keys.IndexOf(nKey);
        if (indexOf < 0){return null;}
        return SpriteDictionary(values[indexOf]);
    }

    public Sprite GetSprite(string spriteName)
    {
        return SpriteDictionary(spriteName);
    }
}

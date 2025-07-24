using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SpriteContainer", menuName = "ScriptableObjects/DataContainers/SpriteContainer", order = 1)]
public class SpriteContainer : ScriptableObject
{
    public List<Sprite> sprites;
    public string allKeysAndValues;
    public string delimiter;
    public string delimiterTwo;
    public void Initialize()
    {
        string[] blocks = allKeysAndValues.Split(delimiter);
        keys = blocks[0].Split(delimiterTwo).ToList();
        values = blocks[1].Split(delimiterTwo).ToList();
    }
    public List<string> keys;
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "SpriteContainer", menuName = "ScriptableObjects/SpriteContainer", order = 1)]
public class SpriteContainer : ScriptableObject
{
    public List<Sprite> sprites;
    public List<string> keys;
    public List<string> values;

    public Sprite SpriteDictionary(string spriteName)
    {
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
}

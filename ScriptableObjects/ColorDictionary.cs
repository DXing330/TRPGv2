using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Colors", menuName = "ScriptableObjects/Colors", order = 1)]
public class ColorDictionary : ScriptableObject
{
    public List<string> colorNames;
    public List<Color> colors;
}

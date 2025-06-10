using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeatherFilter : MonoBehaviour
{
    void Start()
    {
        UpdateFilter(overworldState.GetWeather());
    }
    public OverworldState overworldState;
    public SpriteContainer sprites;
    public Image filter;
    public void UpdateFilter(string weather)
    {
        SetImage(sprites.SpriteDictionary(weather));
    }
    protected void SetImage(Sprite newSprite) { filter.sprite = newSprite; }
}

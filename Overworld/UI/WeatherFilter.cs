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
    public GameObject filterObject;
    public Image filter;
    public void UpdateFilter(string weather)
    {
        filterObject.SetActive(true);
        SetImage(sprites.SpriteDictionary(weather));
        if (filter.sprite == null){ filterObject.SetActive(false); }
    }
    protected void SetImage(Sprite newSprite) { filter.sprite = newSprite; }
}

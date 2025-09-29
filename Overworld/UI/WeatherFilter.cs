using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeatherFilter : MonoBehaviour
{
    void Start()
    {
        if (!subGame)
        {
            UpdateFilter(overworldState.GetWeather());
        }
    }
    public bool subGame = false;
    public string currentWeather;
    public OverworldState overworldState;
    public SpriteContainer sprites;
    public GameObject filterObject;
    public Image filter;
    public StatDatabase weatherData;
    public StatusDetailViewer weatherDetailViewer;
    public PopUpMessage weatherDetails;
    public void UpdateFilter(string weather)
    {
        currentWeather = weather;
        filterObject.SetActive(true);
        SetImage(sprites.SpriteDictionary(weather));
        if (filter.sprite == null){ filterObject.SetActive(false); }
    }
    protected void SetImage(Sprite newSprite) { filter.sprite = newSprite; }
    public void ShowWeatherDetails()
    {
        weatherDetails.SetMessage(weatherDetailViewer.ReturnStatusDetails(weatherData.ReturnValue(currentWeather)));
    }
}

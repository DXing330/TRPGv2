using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayNightFilter : MonoBehaviour
{
    public Image filter;
    public List<Color> dayNightColors;
    public void UpdateFilter(int hour)
    {
        filter.color = new Color(dayNightColors[hour].r,dayNightColors[hour].g,dayNightColors[hour].b,dayNightColors[hour].a);
    }
}

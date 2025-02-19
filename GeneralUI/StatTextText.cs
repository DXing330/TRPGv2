using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatTextText : MonoBehaviour
{
    public TMP_Text statText;
    public TMP_Text text;


    public virtual void SetStatText(string statName){statText.text = statName;}
    public string GetStatText(){return statText.text;}
    public void SetText(string newText){text.text = newText;}
    public string GetText(){return text.text;}

    public virtual void Reset()
    {
        text.text = "";
        statText.text = "";
    }

    public virtual void SetColor(Color newColor)
    {
        text.color = newColor;
        statText.color = newColor;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayTurnOrder : MonoBehaviour
{
    public SpriteContainer actorSprites;
    public List<GameObject> displayObjects;
    public List<TMP_Text> turnNumberTexts;
    public List<TMP_Text> teamNumberTexts;
    public List<LayeredImage> actorImages;
    public int displayLayer = 1;
    public int currentTurn;
    public int actorCount;

    [ContextMenu("Reset Actor Images")]
    public void ResetActorImages()
    {
        for (int i = 0; i < actorImages.Count; i++)
        {
            turnNumberTexts[i].text = "";
            teamNumberTexts[i].text = "";
            displayObjects[i].SetActive(false);
            actorImages[i].SetSprite(null, displayLayer);
            actorImages[i].BackgroundColor(displayLayer);
        }
    }

    public void UpdateTurnOrder(List<TacticActor> actors, int turnIndex)
    {
        currentTurn = turnIndex;
        actorCount = actors.Count;
        ResetActorImages();
        if (turnIndex < 0) { return; }
        int index = 0;
        for (int i = turnIndex; i < actors.Count; i++)
        {
            if (index >= actorImages.Count){break;}
            displayObjects[index].SetActive(true);
            actorImages[index].SetSprite(actorSprites.GetSprite(actors[i].GetSpriteName()), displayLayer);
            actorImages[index].DefaultColor(displayLayer);
            turnNumberTexts[i].text = (i + 1).ToString();
            teamNumberTexts[i].text = (actors[i].GetTeam()).ToString();
            index++;
        }
    }

    // Later enable scrolling up and down.
    public int currentIndex;
}

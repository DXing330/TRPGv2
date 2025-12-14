using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayTurnOrder : MonoBehaviour
{
    public SpriteContainer actorSprites;
    public List<LayeredImage> actorImages;
    public int displayLayer = 1;
    public int currentTurn;
    public int actorCount;

    [ContextMenu("Reset Actor Images")]
    public void ResetActorImages()
    {
        for (int i = 0; i < actorImages.Count; i++)
        {
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
            actorImages[index].SetSprite(actorSprites.GetSprite(actors[i].GetSpriteName()), displayLayer);
            actorImages[index].DefaultColor(displayLayer);
            index++;
        }
    }
}

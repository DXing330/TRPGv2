using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActorSprite : MonoBehaviour
{
    public SpriteContainer actorSprites;
    public Image actorSprite;
    public virtual void SetTextSize(int newSize)
    {
    }

    public void ShowActorSprite(TacticActor actor)
    {
        actorSprite.sprite = actorSprites.GetSprite(actor.GetSpriteName());
    }

    public virtual void ShowActorInfo(TacticActor actor)
    {
        Debug.Log(actor.GetPersonalName());
    }

    public virtual void ChangeTextColor(Color newColor){}
}

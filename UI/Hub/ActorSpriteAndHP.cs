using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActorSpriteAndHP : MonoBehaviour
{
    public SpriteContainer actorSprites;
    public Image actorSprite;
    public TMP_Text actorHealth;
    public GameObject healthBar;

    public void ShowActorSpriteAndHP(TacticActor actor)
    {
        actorSprite.sprite = actorSprites.GetSprite(actor.GetSpriteName());
        int cHP = actor.GetHealth();
        int mHP = actor.GetBaseHealth();
        actorHealth.text = cHP+" / "+mHP;
        healthBar.transform.localScale = new Vector3(Mathf.Min(1, (float)cHP/(float)mHP),1,0);
    }
}

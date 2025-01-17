using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MapTile : MonoBehaviour
{
    public int tileNumber;
    public void SetTileNumber(int newNumber){tileNumber = newNumber;}
    public MapManager cMap;
    public List<GameObject> layerObjects;
    // Tile, Character, Tile Effect, Highlight
    public List<Image> layers;
    public ColorDictionary colorDictionary;
    public TMP_Text tileText;

    public void UpdateText(string newText = ""){tileText.text = newText;}

    public void DisableLayers()
    {
        for (int i = 0; i < layers.Count; i++)
        {
            layerObjects[i].SetActive(false);
        }
    }

    public void UpdateLayerSprite(Sprite newSprite, int layer = 0)
    {
        if (newSprite == null || layers[layer].sprite == null)
        {
            return;
        }
        if (layer < 0 || layer > layers.Count){return;}
        layerObjects[layer].SetActive(true);
        layers[layer].sprite = newSprite;
    }

    public void ResetLayerSprite(int layer)
    {
        layerObjects[layer].SetActive(false);
    }

    protected void ResetHighlight(int layer)
    {
        layers[layer].color = colorDictionary.GetDefaultColor();
    }

    public void HighlightLayer(int layer, string color = "")
    {
        if (colorDictionary.ColorNameExists(color))
        {
            layerObjects[layer].SetActive(true);
            layers[layer].color = colorDictionary.GetColorByName(color);
        }
        else
        {
            ResetLayerSprite(layer);
            ResetHighlight(layer);
            return;
        }
    }

    public void ClickTile()
    {
        cMap.ClickOnTile(tileNumber);
    }
}

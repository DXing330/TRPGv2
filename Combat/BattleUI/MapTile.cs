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
    public GameObject mainObject;
    public List<GameObject> subObjects;
    public List<RectTransform> subObjectTransforms;
    public List<GameObject> highlightObjects;
    public List<RectTransform> highlightObjectTransforms;
    public int elevation = 0;
    public float originalWidth;
    public float originalHeight;
    public float subWidth;
    public float subHeight;
    public float highlightWidth;
    public float highlightHeight;
    public List<float> scalePerElevation;
    public List<float> subYPivots;
    public List<float> highlightYPivots;
    public void SetElevation(int newInfo)
    {
        elevation = newInfo;
        // Scale up the tile based on elevation.
        mainObject.transform.localScale = new Vector3(originalWidth, originalHeight * scalePerElevation[elevation], 0);
        // Make sure that the other images are the same size.
        for (int i = 0; i < subObjects.Count; i++)
        {
            subObjects[i].transform.localScale = new Vector3(subWidth, subHeight / scalePerElevation[elevation], 0);
            subObjectTransforms[i].pivot = new Vector2(0.5f, subYPivots[elevation]);
        }
        for (int i = 0; i < highlightObjects.Count; i++)
        {
            highlightObjects[i].transform.localScale = new Vector3(highlightWidth, highlightHeight / scalePerElevation[elevation], 0);
            highlightObjectTransforms[i].pivot = new Vector2(0.5f, highlightYPivots[elevation]);
        }
        // Adjust the pivots so things look good.
    }
    public int GetElevation()
    {
        return elevation;
    }
    public GeneralUtility utility;
    public List<GameObject> layerObjects;
    // Tile, Character, Tile Effect, Highlight
    public List<Image> layers;
    public List<GameObject> directionObjects;
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

    public void EnableLayer(int layer = 0)
    {
        layerObjects[layer].SetActive(true);
    }

    public void EnableLayers()
    {
        for (int i = 0; i < layers.Count; i++)
        {
            layerObjects[i].SetActive(true);
        }
    }

    public void UpdateDirectionArrow(string direction)
    {
        ResetDirectionArrows();
        if (direction == "") { return; }
        int dirInt = int.Parse(direction);
        if (dirInt >= 0 && dirInt < directionObjects.Count)
        {
            directionObjects[dirInt].SetActive(true);
        }
    }

    public void ResetDirectionArrows()
    {
        for (int i = 0; i < directionObjects.Count; i++)
        {
            directionObjects[i].SetActive(false);
        }
    }

    public void ActivateDirectionArrow(int direction)
    {
        if (direction < 0) { return; }
        directionObjects[direction].SetActive(true);
    }

    public void UpdateLayerSprite(Sprite newSprite, int layer = 0)
    {
        //if (newSprite == null || layers[layer].sprite == null) // Not sure why it was like this or if removing it will make a difference.
        if (newSprite == null) { return; }
        if (layer < 0 || layer > layers.Count) { return; }
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
        try
        {
            cMap.ClickOnTile(tileNumber);
        }
        catch
        {
            Debug.Log(tileNumber);
        }
    }
}

using System;
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
    public int elevation = 0;
    public Image elevationImage;
    public GameObject elevationObject;
    public Image highlightImage;
    public GameObject highlightObject;
    public Color defaultColor;
    public void SetElevation(int newInfo)
    {
        elevation = newInfo;
    }
    public void UpdateElevationSprite(Sprite newSprite)
    {
        if (elevation == 0)
        {
            elevationObject.SetActive(false);
            return;
        }
        elevationObject.SetActive(true);
        elevationImage.sprite = newSprite;
    }
    public int GetElevation()
    {
        return elevation;
    }
    public List<GameObject> layerObjects;
    // Tile, Character, Tile Effect, Highlight
    public List<Image> layers;
    public List<GameObject> directionObjects;
    public List<string> borderDetails;
    public List<GameObject> borderObjects;
    public List<Image> borderImages;
    public void ResetBorders()
    {
        borderDetails.Clear();
        for (int i = 0; i < borderObjects.Count; i++)
        {
            borderObjects[i].SetActive(false);
            borderDetails.Add("");
        }
    }
    [ContextMenu("Show Borders")]
    public void ShowBorders()
    {
        for (int i = 0; i < borderObjects.Count; i++)
        {
            borderObjects[i].SetActive(true);
        }
    }
    public void AddBorder(int direction)
    {
        borderDetails[direction] = "Border";
        borderObjects[direction].SetActive(true);
    }
    public string ReturnBorderString()
    {
        return String.Join("|", borderDetails);
    }
    public void SetBorders(List<string> newInfo)
    {
        borderDetails = new List<string>(newInfo);
        for (int i = 0; i < borderObjects.Count; i++)
        {
            if (borderDetails[i] != "")
            {
                borderObjects[i].SetActive(true);
            }
        }
    }
    public void SetAllBorders(string newBorder, Sprite borderSprite = null)
    {
        for (int i = 0; i < borderObjects.Count; i++)
        {
            SetBorder(newBorder, i ,borderSprite);
        }
    }
    public void SetBorder(string newBorder, int direction, Sprite borderSprite = null)
    {
        borderDetails[direction] = newBorder;
        if (newBorder == "")
        {
            borderObjects[direction].SetActive(false);
            return;
        }
        borderObjects[direction].SetActive(true);
        borderImages[direction].sprite = borderSprite;
    }
    public void UpdateBorderImage(int direction, Sprite borderSprite)
    {
        borderImages[direction].sprite = borderSprite;
    }
    public List<string> GetBorders()
    {
        return borderDetails;
    }
    public string GetBorderInDirection(int direction)
    {
        if (direction < 0 || direction >= borderDetails.Count)
        {
            return "";
        }
        return borderDetails[direction];
    }
    public bool BorderInDirection(int direction)
    {
        if (direction < 0 || direction >= borderDetails.Count)
        {
            return false;
        }
        // Anything besides blank is a border.
        return borderDetails[direction] != "";
    }
    public TMP_Text tileText;
    public GameObject textObject;

    public void UpdateText(string newText = "")
    {
        if (newText == "")
        {
            textObject.SetActive(false);
        }
        textObject.SetActive(true);
        tileText.text = newText;
    }

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
        if (layer < 0 || layer > layers.Count) { return; }
        if (newSprite == null)
        {
            layerObjects[layer].SetActive(false);
            return;
        }
        layerObjects[layer].SetActive(true);
        layers[layer].sprite = newSprite;
    }

    public void ResetAllLayers()
    {
        for (int i = 0; i < layerObjects.Count; i++)
        {
            ResetLayerSprite(i);
        }
    }

    public void ResetLayerSprite(int layer)
    {
        layerObjects[layer].SetActive(false);
    }

    public void ResetHighlight()
    {
        highlightObject.SetActive(false);
        highlightImage.color = defaultColor;
    }

    public void HighlightTile(Color newColor)
    {
        highlightObject.SetActive(true);
        highlightImage.color = newColor;
    }

    public void HighlightLayer(int layer, Color newColor)
    {
        layerObjects[layer].SetActive(true);
        layers[layer].color = newColor;
    }

    public void ClickTile()
    {
        cMap.ClickOnTile(tileNumber);
    }
}

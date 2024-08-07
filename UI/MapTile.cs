using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapTile : MonoBehaviour
{
    public int tileNumber;
    public void SetTileNumber(int newNumber){tileNumber = newNumber;}
    //public Map cMap;
    public List<GameObject> layerObjects;
    public List<Image> layers;

    public void DisableLayers()
    {
        for (int i = 0; i < layers.Count; i++)
        {
            layerObjects[i].SetActive(false);
        }
    }

    public void UpdateLayerSprite(Sprite newSprite, int layer = 0)
    {
        if (layer < 0 || layer > layers.Count){return;}
        layers[layer].sprite = newSprite;
        layerObjects[layer].SetActive(true);
    }

    public void ResetLayerSprite(int layer)
    {
        layerObjects[layer].SetActive(true);
    }

}

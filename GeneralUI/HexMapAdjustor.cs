using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexMapAdjustor : MonoBehaviour
{
    public Sprite defaultSprite;
    public int gridSize = 9;
    public List<RectTransform> hexTiles;
    public List<MapTile> mapTiles;
    

    [ContextMenu("Initialize")]
    protected virtual void InitializeTiles()
    {
        int tileIndex = 0;
        float scale = 1f/(gridSize+1);
        float xPivot = 0f;
        float yCenter = 1f - (1f/(2*gridSize));
        float yPivot = 1f;
        for (int i = 0; i < gridSize; i++)
        {
            xPivot = 0f;
            for (int j = 0; j < gridSize; j++)
            {
                if (j%2 == 0)
                {
                    yPivot = yCenter + 1f/(4*gridSize);
                }
                else
                {
                    yPivot = yCenter - 1f/(4*gridSize);
                }
                hexTiles[tileIndex].pivot = new Vector2(xPivot, yPivot);
                mapTiles[tileIndex].SetTileNumber(tileIndex);
                mapTiles[tileIndex].UpdateLayerSprite(defaultSprite);
                //mapTiles[tileIndex].UpdateText(tileIndex.ToString());
                //tiles[tileIndex].SetTileText("("+GetHexQ(tileIndex)+","+GetHexR(tileIndex)+","+GetHexS(tileIndex)+")");
                tileIndex++;
                xPivot += 1f/(gridSize - 1);
            }
            yCenter -= 1f/(gridSize);
        }
    }

    [ContextMenu("ResetText")]
    public void ResetTileText()
    {
        for (int i = 0; i < mapTiles.Count; i++)
        {
            mapTiles[i].UpdateText();
        }
    }
}

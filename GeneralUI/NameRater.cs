using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NameRater : MonoBehaviour
{
    public TMP_Text newNameText;
    public string newNameString;
    public int characterLimit = 13;

    public void ResetNewName()
    {
        newNameString = "";
        newNameText.text = newNameString;
    }

    public void AddCharacterToName(string character)
    {
        if (newNameString.Length >= characterLimit) { return; }
        newNameString += character;
        newNameText.text = newNameString;
    }

    public void RemoveCharacterFromName()
    {
        if (newNameString.Length <= 0){ return; }
        newNameString = newNameString.Remove(newNameString.Length - 1, 1);
        newNameText.text = newNameString;
    }

    public string ConfirmName()
    {
        return newNameString;
    }
}

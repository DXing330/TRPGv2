using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SkillDisplay : MonoBehaviour
{
    public StatDatabase activeData;
    public ActiveDescriptionViewer descriptionViewer;
    public TMP_Text skillName;
    public TMP_Text skillDescription;

    public void SetSkill(string newInfo)
    {
        skillName.text = newInfo;
    }
}

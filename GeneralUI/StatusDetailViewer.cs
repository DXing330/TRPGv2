using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusDetailViewer : PassiveDetailViewer
{
    public StatDatabase allStatuses;
    public SelectStatTextList statusSelect;
    public PopUpMessage popUp;
    public void SelectStatus()
    {
        if (statusSelect.GetSelected() < 0){return;}
        popUp.SetMessage(ReturnStatusDetails(allStatuses.ReturnValue(statusSelect.GetSelectedStat())));
    }

    public string ReturnStatusDetails(string newInfo)
    {
        if (!newInfo.Contains("|"))
        {
            return "";
        }
        string[] dataBlocks = newInfo.Split("|");
        string description = "";
        description += PassiveTiming(dataBlocks[0]);
        string[] effects = dataBlocks[1].Split(",");
        string[] specifics = dataBlocks[2].Split(",");
        for (int i = 0; i < effects.Length; i++)
        {
            description += PassiveEffect(effects[i], specifics[i], "target");
            if (i < effects.Length - 1)
            {
                description += " and";
            }
        }
        return description;
    }
}

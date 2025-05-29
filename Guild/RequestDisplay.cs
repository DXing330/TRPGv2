using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestDisplay : MonoBehaviour
{
    public SavedOverworld overworldTiles;
    public OverworldState overworldState;
    public Request dummyRequest;

    public string DisplayRequestDescription(string requestInfo)
    {
        dummyRequest.Load(requestInfo);
        switch (dummyRequest.GetGoal())
        {
            case "Deliver":
                return UpdateDeliveryDescription();
        }
        return "";
    }

    protected string UpdateDeliveryDescription()
    {
        string description = "I need you to deliver these " + dummyRequest.GetGoalAmount() + " shipments of " + dummyRequest.GetGoalSpecifics();
        string cityName = overworldTiles.GetCityNameFromDemandedLuxury(dummyRequest.GetGoalSpecifics());
        int direction = overworldTiles.mapUtility.DirectionBetweenLocations(overworldState.GetLocation(), dummyRequest.GetLocation(), overworldTiles.GetSize());
        string directionName = overworldTiles.mapUtility.IntDirectionToString(direction);
        switch (dummyRequest.GetLocationSpecifics())
        {
            case "City":
                description += " to " + cityName;
                break;
            case "Merchant":
                description += " about halfway to " + cityName;
                break;
        }
        description += ", to the " + directionName + " of here,";
        description += " within " + dummyRequest.GetDeadline() + " days.";
        int failPenalty = dummyRequest.GetFailPenalty();
        if (failPenalty > 0)
        {
            description += "\n" + "Note: " + dummyRequest.GetFailPenalty() + " GOLD fine if the delivery is not completed.";
        }
        return description;
    }
}

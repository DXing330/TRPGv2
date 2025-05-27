using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Generates requests. Displays available requests. Changes the overworld based on accepted quests.
public class RequestBoard : MonoBehaviour
{
    public SavedOverworld overworldTiles;
    public OverworldState overworldState;
    public SavedCaravan caravan;
    public GuildCard guildCard;
    public List<string> requestGoals;
    public Request dummyRequest;
    public List<string> availableRequests;
    public SelectStatTextList questSelect;
    public TMP_Text questDetails;
    public int baseRequestCount = 6;
    public int amountVariation = 9;
    public int distanceVariation = 3;

    void Start()
    {
        availableRequests = guildCard.availableQuests;
        if (guildCard.RefreshQuests()) { GenerateRequests(); }
        UpdateSelectableQuests();
    }

    public void UpdateSelectableQuests()
    {
        List<string> goals = new List<string>();
        List<string> rewards = new List<string>();
        for (int i = 0; i < availableRequests.Count; i++)
        {
            dummyRequest.Load(availableRequests[i]);
            goals.Add(dummyRequest.GetGoal());
            rewards.Add(dummyRequest.GetReward()+" Gold");
        }
        questSelect.SetStatsAndData(goals, rewards);
    }

    public void DisplayRequestDescription()
    {
        int selected = questSelect.GetSelected();
        string selectedRequest = availableRequests[selected];
        dummyRequest.Load(selectedRequest);
        switch (dummyRequest.GetGoal())
        {
            case "Deliver":
                questDetails.text = UpdateDeliveryDescription();
            break;
        }
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
        return description;
    }

    public void GenerateRequests()
    {
        availableRequests.Clear();
        for (int i = 0; i < baseRequestCount; i++)
        {
            dummyRequest.Reset();
            // difficulty is based on a variety of factors, including the type of request
            //int difficulty = 1; // Do we even need a difficulty rating, just judge it for yourself kek?
            string requestGoal = requestGoals[Random.Range(0, requestGoals.Count)];
            switch (requestGoal)
            {
                case "Deliver":
                    GenerateDeliveryRequest();
                    break;
            }
            dummyRequest.SetGoal(requestGoal);
            availableRequests.Add(dummyRequest.ReturnDetails());
        }
        guildCard.Save();
    }

    protected void GenerateDeliveryRequest()
    {
        // Either go all the way or around halfway to a city.
        // Going all the way pays ~1.5x, since if you go all the way you can sell goods at the city yourself
        // Pick a luxury at random.
        string requestedLuxury = overworldTiles.RandomLuxury();
        dummyRequest.SetGoalSpecifics(requestedLuxury);
        dummyRequest.SetGoalAmount(Random.Range(1, amountVariation + 1));
        dummyRequest.SetReward((int) Mathf.Sqrt(dummyRequest.GetGoalAmount()));
        int distance = Random.Range(0, 3); // 0 = full, else half
        if (distance == 0)
        {
            dummyRequest.SetLocation(overworldTiles.GetCityLocationFromLuxuryDemanded(requestedLuxury));
            dummyRequest.SetLocationSpecifics("City");
            dummyRequest.SetDeadline(overworldTiles.mapUtility.DistanceBetweenTiles(dummyRequest.GetLocation(), overworldState.GetLocation(), overworldTiles.GetSize()));
            dummyRequest.SetReward(dummyRequest.GetReward() * dummyRequest.GetDeadline() * 3 / 4);
        }
        else
        {
            // Go roughly in the middle of the target city and the current location.
            int cityLocation = overworldTiles.GetCityLocationFromLuxuryDemanded(requestedLuxury);
            int currentLocation = overworldState.GetLocation();
            int row = (overworldTiles.mapUtility.GetRow(cityLocation, overworldTiles.GetSize()) + overworldTiles.mapUtility.GetRow(currentLocation, overworldTiles.GetSize())) / 2;
            int col = (overworldTiles.mapUtility.GetColumn(cityLocation, overworldTiles.GetSize()) + overworldTiles.mapUtility.GetColumn(currentLocation, overworldTiles.GetSize())) / 2;
            row += Random.Range(-distanceVariation, distanceVariation + 1); // Add a small variation
            col += Random.Range(-distanceVariation, distanceVariation + 1);
            dummyRequest.SetLocation(overworldTiles.mapUtility.ReturnTileNumberFromRowCol(row, col, overworldTiles.GetSize()));
            dummyRequest.SetLocationSpecifics("Merchant");
            // Deadline is assuming you travel 1 tile/day.
            dummyRequest.SetDeadline(overworldTiles.mapUtility.DistanceBetweenTiles(dummyRequest.GetLocation(), overworldState.GetLocation(), overworldTiles.GetSize()));
            dummyRequest.SetReward(dummyRequest.GetReward() * dummyRequest.GetDeadline());
            // Add some slight RNG to the reward amount.
            dummyRequest.SetReward(dummyRequest.GetReward() + Random.Range(-amountVariation, amountVariation + 1));
        }
    }

    protected void GenerateEscortRequest()
    {
        // Escort people to key features (dungeons).
        // Used to generate features.
    }

    protected void GenerateDefeatRequest()
    {
        // Basically either beasts/bandit camp.
        // Generally the least travel required.
    }
}
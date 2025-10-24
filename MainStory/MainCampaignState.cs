using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StSState", menuName = "ScriptableObjects/StS/StSState", order = 1)]
public class MainCampaignState : SavedState
{
    public StatDatabase campaignData;
    // 0-prologue, 1-become strong, 2-conqueor, 3-demon army, 4-postgame
    public int currentAct;
    /*
    0-0 : Introduce setting, clear dungeons for rewards and grow stronger
    0-1 : Clear dungeon as an initiation quest
    1-0 : Recruit party member / Train feat / buy equipment / learn spells
    1-1 : Rescue client from dungeon
    1-2 : Clear dungeon with escort
    1-3 : Pass battle check to ensure you're strong enough
    2-0 : Visit other cities and meet their leaders
    2-1 : Pick a city and help them sabotage their competitiors
    1. Destroy luxury resources by clearing dungeons (normal)
    2. Destroy villages by clearing dungeons (normal)
    3. Rob merchants by clearing dungeons (reverse rescue)
    4. Kidnap nobles by clearing dungeons (reverse rescue)
    5. Insert spies by clearing dungeons (escort)
    Each success will permanently weaken a city
    2-2 : Attack other cities
    Each city is a dungeon
    Each city you attack makes the other cities stronger (deeper dungeon, more buffs, stronger enemies)
    Build up your army each time and move to cities to conqueor them
    Clear the city dungeon to conqueor the city
    2-3 : Betray or submit to your chosen city
    2-4 : City building minigame
    1. Build outposts/villages through initial investments
    2. Protect them from bandits/monsters
    2a. Failure to protect means they will lose health and eventually be destroyed
    2b. Receive reports are random intervals to let you know if danger is near
    3. Collect tribute from them
    4. Hire underlings and expand your operations
    2-5 : Play for a few in game years?
    3-0 : New reports of demons appearing
    3-1 : Remove demons as they appear on overworld
    3-2 : Track down the demon general dungeons on the overworld
    2a. Either by yourself or through reports
    3-3 : Demon general dungeons
    3-3 : Demon lord dungeon
    4-0 : Post game super dungeons
    */
    public int currentChapter;
    // Deliver on time, rescue on time, defeat on time, escort on time, explore first, defend til the end.
    public int chapterDeadline;
    // Deliver, rescue, defeat, escort, explore, defend.
    public string currentRequest;
    // Deliver what, rescue who, defeat who, escort who, explore where, defend what?
    public string requestSpecifics;
    // Drop off site, last seen location, last seen location, drop off location, dungeon location, defense location.
    public int requestLocation;

    public override void NewGame()
    {
        currentAct = 0;
        currentChapter = 0;
        currentRequest = "";
        requestSpecifics = "";
        requestLocation = -1;
    }
}
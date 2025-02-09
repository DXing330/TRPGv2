using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class References : MonoBehaviour
{
    // Used to prevent resources from being unloaded since they are not referenced.
    public Dungeon dungeonReference;
    public CharacterList partyReference;
}

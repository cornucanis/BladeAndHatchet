using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RoomDirectionDictionary {
	public Room[] rooms;
	public RoomTransitionTrigger.Direction[] directions;
}

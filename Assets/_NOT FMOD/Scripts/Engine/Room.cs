using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

	public RoomData roomData;
	public PolygonCollider2D bounds;
	public GameObject[] enemiesInRoom;

	void Awake() {
		bounds = GetComponent<PolygonCollider2D> ();
		roomData.roomBounds = bounds;
	}
}

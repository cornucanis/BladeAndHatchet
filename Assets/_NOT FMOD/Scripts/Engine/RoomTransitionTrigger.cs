using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTransitionTrigger : MonoBehaviour {

	public RoomDirectionDictionary connectedRooms;

	public enum Direction {Left, Right, Up, Down}


	void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag ("Player")) {
			RoomTransitionHandler.Instance.TriggerTransition (connectedRooms);
		}
	}
}

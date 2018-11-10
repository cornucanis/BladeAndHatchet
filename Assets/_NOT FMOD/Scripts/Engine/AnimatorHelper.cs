using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHelper : MonoBehaviour {

	PlayerStatus playerStatus;
	PlayerMovement playerMovement;


	void Awake() {
		playerStatus = GetComponentInChildren<PlayerStatus> ();
		playerMovement = GetComponentInChildren<PlayerMovement> ();
	}

	public void ChangeState(PlayerStatus.State newState) {
		playerStatus.CurrentState = newState;
	}

	public void Jump(){
		playerStatus.Launch ();
	}
}

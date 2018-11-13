using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHelper : MonoBehaviour {

	PlayerStatus playerStatus;
	PlayerMovement playerMovement;
	PlayerCombat playerCombat;


	void Awake() {
		playerStatus = GetComponentInChildren<PlayerStatus> ();
		playerMovement = GetComponentInChildren<PlayerMovement> ();
		playerCombat = GetComponentInChildren<PlayerCombat> ();
	}

	public void ChangeState(PlayerStatus.State newState) {
		playerStatus.CurrentState = newState;
	}

	public void Jump(){
		playerStatus.Launch ();
	}

	public void EndAttackAnimation() {
		playerStatus.AttackAnimEnded = true;
	}
}

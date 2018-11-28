using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHelper : MonoBehaviour {

	PlayerStatus playerStatus;
	PlayerMovement playerMovement;
	PlayerCombat playerCombat;
	PlayerWeapon weapon;
	Rigidbody2D rb;


	void Awake() {
		playerStatus = GetComponentInChildren<PlayerStatus> ();
		playerMovement = GetComponentInChildren<PlayerMovement> ();
		playerCombat = GetComponentInChildren<PlayerCombat> ();
		weapon = GetComponentInChildren<PlayerWeapon> ();
		rb = playerStatus.GetComponent<Rigidbody2D> ();
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

	public void SetWeaponDamage(int newDamage) {
		weapon.currentDamage = newDamage;
	}

	public void ResetPosition() {
		Vector3 offset = playerStatus.transform.localPosition;
		playerStatus.transform.localPosition = Vector3.zero;
		transform.position += offset;
	}

	public void AddForce(float addedForce) {
		playerStatus.AddForce (addedForce);
	}

	public void ResetVelocity() {
		rb.velocity = Vector2.zero;
	}

	public void Throw() {
		playerStatus.Armed = false;
		weapon.Throw (playerMovement.facingRight);
	}
}

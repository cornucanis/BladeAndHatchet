using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadSaint : Enemy {

	[SerializeField] GameObject fireballPrefab;
	[SerializeField] Transform fireballHome;

	[Header("Activation config")]
	[SerializeField] float activationRange = 8f;
	[SerializeField] float deactivationRangeMulti = 1.5f;
	[SerializeField] float initialGracePeriod = 0.5f;

	[Header("Fireball config")]
	[SerializeField] float fireballDelay = 0.67f;
	[SerializeField] float fireballCooldown = 8f;

	Transform playerTransform;


	State currentState;
	Animator anim;

	float nextSalvoTime = 0f;

	public enum State {Inactive, Activation, Deactivation, Idle}

	public State CurrentState {
		get {
			return currentState;
		}

		set {
			ChangeStates (value);
		}	
	}

	protected override void Initialize() {
		anim = GetComponent<Animator> ();
		playerTransform = GameObject.FindWithTag ("Player").transform;
		CurrentState = State.Inactive;
	}

	void Update () {
		Invoke (currentState.ToString () + "Stay", 0f);
	}

	public void ChangeStates(State newState) {
		string currentStateName = currentState.ToString ();
		string newStateName = newState.ToString ();
		Invoke (currentStateName + "Exit", 0f);
		currentState = newState;
		Invoke (newStateName + "Enter", 0f);
		anim.SetBool (currentStateName, false);
		anim.SetBool (newStateName, true);
	}

	IEnumerator SummonFireballs() {
		WaitForSeconds delay = new WaitForSeconds (fireballDelay);
		for (int i = 0; i < 3; i++) {
			DeadSaintFireball newBall = Instantiate (fireballPrefab, transform.position, Quaternion.identity).GetComponent<DeadSaintFireball> ();
			newBall.playerTransform = playerTransform;
			newBall.fireballHome = fireballHome.position;
			yield return delay;	
		}
		nextSalvoTime = Time.time + (fireballCooldown - fireballDelay);
	}

	#region State Methods

	void InactiveEnter() {

	}

	void InactiveExit() {

	}

	void InactiveStay() {
		float distance = Vector3.Distance (transform.position, playerTransform.position);
		if (distance <= activationRange) {
			CurrentState = State.Activation;
		}
	}

	void ActivationEnter() {
		

	}

	void ActivationExit() {

	}

	void ActivationStay() {

	}

	void DeactivationEnter() {

	}

	void DeactivationExit() {

	}

	void DeactivationStay() {

	}

	void IdleEnter() {
		nextSalvoTime = Time.time + initialGracePeriod;
	}

	void IdleExit() {

	}

	void IdleStay() {
		float distance = Vector3.Distance (transform.position, playerTransform.position);
		if (distance >= activationRange * deactivationRangeMulti) {
			CurrentState = State.Deactivation;
		} else if (Time.time >= nextSalvoTime) {
			StartCoroutine (SummonFireballs ());
			nextSalvoTime = Time.time + 20f;
		}

	}

	#endregion
}

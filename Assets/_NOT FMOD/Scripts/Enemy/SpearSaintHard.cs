using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearSaintHard : MonoBehaviour {

	[SerializeField] float idleWaitTime = 1f;
	[SerializeField] float telegraphWaitTime = 2f;
	[SerializeField] float indicatorLifetime;
	[SerializeField] float descentTime = 0.5f;
	[SerializeField] float descentDistance = 3f;
	[SerializeField] float vulnerableWaitTime = 2f;
	[SerializeField] float flightSpeed = 10f;
	[SerializeField] float spearVerticalOffset;
	[SerializeField] Transform[] movePoints;
	[SerializeField] Transform[] attackPoints;
	[SerializeField] GameObject telegraphIndicatorPrefab;
	[SerializeField] GameObject spearPrefab;
	[SerializeField] GameObject rewardPrefab;

	State currentState;
	Animator anim;
	Rigidbody2D rb;

	float idleExitTime = 0f;
	float attackTime = 0f;
	float stopTime = 0f;
	float moveTime = 0f;
	Transform[] currentTargets;
	Transform currentPosition;
	bool defeated = false;
	Vector3 rewardLoc;

	public enum State {Idle, Telegraphing, Attacking, Vulnerable, Moving}

	public State CurrentState {
		get {
			return currentState;
		}

		set {
			ChangeStates (value);
		}	
	}

	void Awake() {
		anim = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody2D> ();
		currentPosition = movePoints [0];
		CurrentState = State.Idle;
	}

	void OnEnable() {
		if (defeated) {
			gameObject.SetActive (false);
		}
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

	public void SummonSpears() {
		for (int i = 0; i < currentTargets.Length; i++) {
			Destroy (currentTargets [i].gameObject, indicatorLifetime);
			Instantiate (spearPrefab, currentTargets [i].position + Vector3.up * spearVerticalOffset, Quaternion.identity);
		}
	}

	public void AcquireTargets() {
		List<Transform> returnList = new List<Transform>();
		List<int> selectedIndices = new List<int> ();
		int nextTarget;
		for (int i = 0; i < 3; i++) {
			do {
				nextTarget = Random.Range (0, attackPoints.Length);
			} while (selectedIndices.Contains (nextTarget));
			GameObject newIndicator = Instantiate (telegraphIndicatorPrefab, attackPoints [nextTarget].position, Quaternion.identity);
			returnList.Add(newIndicator.transform);
			selectedIndices.Add (nextTarget);
		}
		currentTargets = returnList.ToArray ();
	}

	public void StartDescent() {
		rb.velocity = Vector2.down * (descentDistance / descentTime);
		stopTime = Time.time + descentTime;
		Debug.Log ("Stop time: " + stopTime + ", " + Time.time);
	}

	public void StoreLocation() {
		rewardLoc = transform.position;
	}

	public void DropReward() {
		Instantiate (rewardPrefab, rewardLoc + Vector3.up * 0.5f, Quaternion.identity);
		defeated = true;
	}

	#region State Methods

	void IdleEnter() {
		idleExitTime = Time.time + idleWaitTime;
	}

	void IdleExit() {

	}

	void IdleStay() {
		if (Time.time >= idleExitTime) {
			CurrentState = State.Telegraphing;
		}
	}

	void TelegraphingEnter() {
		attackTime = Time.time + telegraphWaitTime;
	}

	void TelegraphingExit() {

	}

	void TelegraphingStay() {
		CurrentState = State.Attacking;
	}

	void AttackingEnter() {
		AcquireTargets ();
		SummonSpears ();
	}

	void AttackingExit() {

	}

	void AttackingStay() {

	}

	void VulnerableEnter() {
		StartDescent ();
		moveTime = -1f;
	}

	void VulnerableExit() {

	}

	void VulnerableStay() {
		if (moveTime == -1f && Time.time >= stopTime) {
			rb.velocity = Vector2.zero;
			moveTime = Time.time + vulnerableWaitTime;
			Debug.Log ("Stopped. Time: " + Time.time + ", stop time: " + stopTime + ", move time: " + moveTime);
		}
		if (moveTime != -1f && Time.time >= moveTime) {
			CurrentState = State.Moving;
		}
	}

	void MovingEnter() {
		Transform newPosition;
		do {
			newPosition = movePoints[Random.Range(0, movePoints.Length)];
			Debug.Log( newPosition.name + ", " + currentPosition.name);
		} while (newPosition == currentPosition);
		currentPosition = newPosition;
		Vector2 speedDirection = (currentPosition.position - transform.position);
		rb.velocity = (speedDirection.normalized * flightSpeed);
	}

	void MovingExit() {

	}

	void MovingStay() {
		if (Vector2.Distance (transform.position, currentPosition.position) < 0.5f) {
			rb.velocity = Vector2.zero;
			transform.position = currentPosition.position;
			CurrentState = State.Idle;
		} else {
			Vector2 speedDirection = (currentPosition.position - transform.position);
			rb.velocity = (speedDirection.normalized * flightSpeed);
		}
	}
	#endregion
}

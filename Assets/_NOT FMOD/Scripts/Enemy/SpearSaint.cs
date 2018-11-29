using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearSaint : MonoBehaviour {

	State currentState;
	Animator anim;

	public enum State {Idle, Attacking}

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
		CurrentState = State.Idle;
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


	#region State Methods

	void IdleEnter() {

	}

	void IdleExit() {

	}

	void IdleStay() {

	}

	#endregion
}

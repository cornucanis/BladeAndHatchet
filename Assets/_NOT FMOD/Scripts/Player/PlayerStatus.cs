using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour {

	//config params
	[Header("Sprite Configuration")]
	[SerializeField] SpriteRenderer characterSprite;
	[SerializeField] SpriteRenderer weaponSprite;

	public static List<SpriteRenderer> palList;

	//cached references
	Animator anim;
	PlayerMovement playerMovement;
	PlayerCombat playerCombat;
	Rigidbody2D rb;
	CapsuleCollider2D coll;

	//state variables
	State currentState;
	int jumpMode;
	bool isSword;


	public enum State {Idle, Walk, Jump, Attack, Stunned, Death}

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
		playerMovement = GetComponent<PlayerMovement> ();
		playerCombat = GetComponent<PlayerCombat> ();
		rb = GetComponent<Rigidbody2D> ();
		coll = GetComponent<CapsuleCollider2D> ();
		CurrentState = State.Idle;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
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

	private void CharacterSwap() {
		isSword = !isSword;
		Vector3 charPos = characterSprite.transform.position;
		characterSprite.transform.position = weaponSprite.transform.position;
		weaponSprite.transform.position = charPos;
		if (isSword) {

		} else {

		}
	}

	#region State Methods

	void IdleEnter() {

	}

	void IdleExit() {

	}

	void IdleStay() {

	}

	void WalkEnter() {

	}

	void WalkExit() {

	}

	void WalkStay() {
		playerMovement.OnWalkStay ();
	}

	void AttackEnter() {
		rb.velocity = Vector2.zero;
	}

	void AttackExit() {

	}

	void AttackStay() {

	}

	void JumpEnter() {

	}

	void JumpExit() {

	}

	void JumpStay() {

	}

	void DeathEnter() {
		rb.velocity = Vector2.zero;
	}

	void DeathExit() {

	}

	void DeathStay() {

	}

	void StunnedEnter() {
		playerMovement.enabled = false;
	}

	void StunnedExit() {
		playerMovement.enabled = true;
	}

	void StunnedStay() {

	}



	#endregion
}

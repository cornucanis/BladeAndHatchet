using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour {

	//config params

	[SerializeField] float jumpForce = 200f;
	[SerializeField] float jumpCastOffset = 0.1f;

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
	[SerializeField] LayerMask jumpMask;

	//state variables
	State currentState;
	int jumpMode;
	bool isSword = true;
	bool armed = true;
	Vector2 storedVelocity = Vector2.zero;


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
		anim = GetComponentInParent<Animator> ();
		playerMovement = GetComponent<PlayerMovement> ();
		playerCombat = GetComponent<PlayerCombat> ();
		rb = GetComponent<Rigidbody2D> ();
		coll = GetComponent<CapsuleCollider2D> ();
		CurrentState = State.Idle;
	}

	// Use this for initialization
	void Start () {
		jumpMask = LayerMask.GetMask ("Foreground");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Switch")) {
			CharacterSwap ();
		} else if (Input.GetButtonDown ("Throw")) {
			ThrowWeapon ();
		} else if (Input.GetButtonDown ("Jump")) {
			Jump ();
		}
		Invoke (currentState.ToString () + "Stay", 0f);
		TrackWeapon ();
	}

	void TrackWeapon() {
		if (armed) {
			weaponSprite.transform.position = transform.position;
		}
	}

	public bool CanMove() {
		if (currentState != State.Death && currentState != State.Stunned && !(currentState == State.Jump && jumpMode == -1)) { // Note: The last one is to check that we're not in the middle of launching
			return true;
		}
		return false;
	}

	public bool CanJump() {
		if (currentState != State.Death && currentState != State.Stunned && currentState != State.Jump && currentState != State.Attack) {
			return true;
		}
		return false; 
	}

	public bool Jump() {
		if (CheckGrounded() && CanJump()) {
			CurrentState = State.Jump;
			anim.SetTrigger ("launch");
			jumpMode = -1;
			anim.SetInteger ("jumpMode", 0);
			storedVelocity = rb.velocity;
			rb.velocity = Vector2.zero;
			//Debug.Log ("Triggered.. Stored: " + storedVelocity + ", current: " + rb.velocity);
			return true;
		}
		return false;
	}

	public void Launch() {
		//Debug.Log ("Launch");
		rb.velocity = storedVelocity;
		storedVelocity = Vector2.zero;
		rb.AddForce (new Vector2 (0f, jumpForce));
		jumpMode = 0;
	}

	private bool CheckGrounded() {
		Collider2D colliderCheck = Physics2D.OverlapCapsule (coll.bounds.center + Vector3.down * jumpCastOffset, new Vector2 (coll.size.x * 0.9f, coll.size.y * 0.9f), CapsuleDirection2D.Vertical, 0f, jumpMask);
		if (colliderCheck) {
			return true;
		} else {
			return false;
		}
		//return Physics.CheckCapsule (coll.bounds.center, new Vector3 (coll.bounds.center.x, coll.bounds.min.y, coll.bounds.center.z), coll.bounds.extents.x - 1f, jumpMask);
	}

	private bool CheckFalling() {
		Debug.Log (CheckGrounded () + ", " + rb.velocity.y);
		if (rb.velocity.y < 0f && !CheckGrounded()) {
			Debug.Log ("Falling");
			return true;
		}
		Debug.Log ("Not Falling");
		return false;
	}

	private void TriggerFall() {
		CurrentState = State.Jump;
		jumpMode = 1;
		anim.SetInteger ("jumpMode", 1);
		anim.SetTrigger ("fall");
	}

	public void ChangeStates(State newState) {
		string currentStateName = currentState.ToString ();
		string newStateName = newState.ToString ();
		Invoke (currentStateName + "Exit", 0f);
		currentState = newState;
		Invoke (newStateName + "Enter", 0f);
		anim.SetBool (currentStateName, false);
		anim.SetBool (newStateName, true);
		playerMovement.currentState = currentState;
	}

	private void CharacterSwap() {
		isSword = !isSword;
		anim.SetBool ("isSword", isSword);
		anim.SetFloat ("isSwordFloat", isSword ? 1f : 0f);
		Vector3 charPos = characterSprite.transform.position;
		characterSprite.transform.position = weaponSprite.transform.position;
		weaponSprite.transform.position = charPos;
		if (isSword) {
			
		} else {

		}
	}

	private void ThrowWeapon() {
		if (armed) {
			armed = false;

		} else {
			armed = true;
		}
		weaponSprite.enabled = !armed;
		anim.SetBool ("armed", armed);
		anim.SetFloat ("armedFloat", armed ? 1f : 0f);
	}

	#region State Methods

	void IdleEnter() {

	}

	void IdleExit() {

	}

	void IdleStay() {
		if (CheckFalling ()) {
			TriggerFall ();
		}
	}

	void WalkEnter() {

	}

	void WalkExit() {

	}

	void WalkStay() {
		if (CheckFalling ()) {
			TriggerFall ();
		} else {
			playerMovement.OnWalkStay ();
		}
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
		jumpMode = -1;
		anim.SetInteger ("jumpMode", 0);
	}

	void JumpStay() {
		if (jumpMode == 0 && rb.velocity.y <= 0f) {
			jumpMode = 1;
			anim.SetInteger ("jumpMode", 1);
		} else if (jumpMode == 1 && Mathf.Abs (rb.velocity.y) < Mathf.Epsilon) {
			//Debug.Log (rb.velocity.y);
			jumpMode = 2;
			anim.SetInteger ("jumpMode", 2);
			rb.velocity = Vector2.zero;
		}
		//Debug.Log (jumpMode + ", " + rb.velocity.y + ", " + anim.GetInteger("jumpMode"));

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

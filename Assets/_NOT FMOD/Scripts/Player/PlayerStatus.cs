using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour {

	//config params

	[SerializeField] float jumpForce = 200f;

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
	LayerMask jumpMask;

	//state variables
	State currentState;
	int jumpMode;
	bool isSword = true;
	bool armed = true;


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
		//Debug.Log (coll.bounds + " " + coll.bounds.extents);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Switch")) {
			CharacterSwap ();
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
		if (currentState != State.Death && currentState != State.Stunned && currentState != State.Jump) {
			return true;
		}
		return false;
	}

	public bool Jump() {

		if (CheckGrounded()) {
			rb.AddForce (new Vector2 (0f, jumpForce));
			return true;
		}
		return false;
	}

	private bool CheckGrounded() {
		//Debug.Log ("center: " + (coll.bounds.center - Vector3.down * 0.1f) + ", truecenter: " + coll.bounds.center + ", size: " + new Vector2 (coll.size.x - 0.1f, coll.size.y));
		Collider2D colliderCheck = Physics2D.OverlapCapsule (coll.bounds.center - Vector3.down * 0.5f, new Vector2 (coll.size.x - 0.1f, coll.size.y), CapsuleDirection2D.Vertical, 0f, jumpMask);
		if (colliderCheck) {
			Debug.Log (colliderCheck.name);
			return true;
		} else {
			return false;
		}
		//return Physics.CheckCapsule (coll.bounds.center, new Vector3 (coll.bounds.center.x, coll.bounds.min.y, coll.bounds.center.z), coll.bounds.extents.x - 1f, jumpMask);
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

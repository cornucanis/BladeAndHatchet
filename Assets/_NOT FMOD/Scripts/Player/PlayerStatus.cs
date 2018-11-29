using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour {

	//config params

	[Header("Jump Configuration")]
	[SerializeField] float axeJumpForce = 1300f;
	[SerializeField] float swordJumpforce = 2000f;
	[SerializeField] float jumpCastOffset = 0.1f;
	[SerializeField] float dropThruTimer = 0.5f;
	[SerializeField] float minimumWallJumpVelocity = 1f;
	[SerializeField] float wallJumpDetectionSizeMod = 1.07f;
	[SerializeField] float wallJumpWindow = 0.2f;
	[SerializeField] float wallJumpVerticalForce = 1000f;
	[SerializeField] float wallJumpHorizontalForce = 750f;
	[SerializeField] float wallJumpHitstopDuration = 0.1f;
	[SerializeField] float wallJumpHitstopZoom = 0.95f;
	[SerializeField] float wallJumpHitstopTimecsale = 0.6f;


	[Header("Sprite Configuration")]
	[SerializeField] SpriteRenderer characterSprite;
	[SerializeField] SpriteRenderer weaponSprite;

	[Header("Weapon Collider Parameters")]
	[SerializeField] BoxCollider2D weaponColl;
	[SerializeField] Vector2 swordFinisherOffset;
	[SerializeField] Vector2 swordFinisherSize;
	[SerializeField] Vector2 axeFinisherOffset;
	[SerializeField] Vector2 axeFinisherSize;
	[SerializeField] Vector2 thrownOffset;
	[SerializeField] Vector2 thrownSize;

	public static List<SpriteRenderer> palList;
    private FMOD.Studio.EventInstance hitSnap;

    //cached references
    Animator anim;
	PlayerMovement playerMovement;
	PlayerCombat playerCombat;
	PlayerWeapon weapon;
	Rigidbody2D rb;
	CapsuleCollider2D coll;
	[SerializeField] LayerMask jumpMask;

	//state variables
	State currentState;
	int jumpMode = 0;
	float initialGravity;
	float nextPossibleWalljump;
	bool isSword = true;
	bool armed = true;
	bool frozen = false;
	bool falling = false;
	bool wallJumpDisqualified = false;
	[HideInInspector] public bool attackAnimEnded = false;
	Vector2 storedVelocity = Vector2.zero;
	Vector3 weaponPositionOffset;
	Vector2 weaponCollOffset;
	Vector2 weaponCollSize;

	public enum State {Idle, Walk, Jump, Attack, Throw, Stunned, Death}

	public bool Frozen {
		get {
			return frozen;
		}
		set {
			//Debug.Log ("frozen set to " + value);
			frozen = value;
		}
	}

	public bool AttackAnimEnded {
		get {
			return attackAnimEnded;
		}
		set {
			attackAnimEnded = value;
			if (value) {
				int nextAttack = playerCombat.CheckAttackQueue ();
				if (nextAttack != -1) {
					Attack (nextAttack);
				}
			} else {
				Debug.LogWarning ("Why are you setting attack animation ending to false from another script?");
			}
		}
	}

	public State CurrentState {
		get {
			return currentState;
		}

		set {
			ChangeStates (value);
		}	
	}

	public bool Armed {
		get {
			return armed;
		}
		set {
			armed = value;
			weaponSprite.enabled = !armed;
			anim.SetBool ("armed", armed);
			anim.SetFloat ("armedFloat", armed ? 1f : 0f);
		}
	}

	void Awake() {
		anim = GetComponentInParent<Animator> ();
		playerMovement = GetComponent<PlayerMovement> ();
		playerCombat = GetComponent<PlayerCombat> ();
		weapon = FindObjectOfType<PlayerWeapon> ();
		rb = GetComponent<Rigidbody2D> ();
		coll = GetComponent<CapsuleCollider2D> ();
		CurrentState = State.Idle;
	}

	// Use this for initialization
	void Start () {
		jumpMask = LayerMask.GetMask ("Foreground");
		initialGravity = rb.gravityScale;
		weaponCollOffset = weaponColl.offset;
		weaponCollSize = weaponColl.size;
		weaponPositionOffset = weaponSprite.transform.position - transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		CheckInput ();
		Invoke (currentState.ToString () + "Stay", 0f);
		TrackWeapon ();
	}

	void CheckInput() {
		if (Input.GetButtonDown ("Switch")) {
			CharacterSwap ();
		} else if (Input.GetButtonDown ("Throw")) {
			ThrowWeapon ();
		} else if (Input.GetButtonDown ("Jump")) {
			Collider2D fallColl = CanDropThrough ();
			if (fallColl && Input.GetAxisRaw ("Vertical") <= -0.1f) {
				DropDown ();
			} else {
				Jump ();
			}
		} /* else if (Input.GetAxis ("Vertical") <= -0.2f) {
			DropDown ();
		}*/
	}

	void TrackWeapon() {
		if (armed) {
			weaponSprite.transform.position = transform.position + weaponPositionOffset;
			//Debug.Log ("weapon position updated due to being armed");
		}
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

	private void DropDown() {
		Collider2D colliderCheck = Physics2D.OverlapCapsule (coll.bounds.center + Vector3.down * jumpCastOffset, new Vector2 (coll.size.x * 0.9f, coll.size.y * 0.9f), CapsuleDirection2D.Vertical, 0f, jumpMask);
		if (!falling && !frozen && colliderCheck && colliderCheck.CompareTag("DropThrough")) {
			if (currentState == State.Idle || currentState == State.Walk) {
				TriggerFall ();
				Physics2D.IgnoreCollision (coll, colliderCheck);
				//Debug.Log ("Collision disabled");
				StartCoroutine (RestoreCollision (colliderCheck));
			}             
		}
	}

	IEnumerator WallStick() {
		float finishTime = Time.time + wallJumpWindow;
		float trueHorizontal = playerMovement.facingRight ? -wallJumpHorizontalForce : wallJumpHorizontalForce;
		HitStopper.Instance.StartHitStop (wallJumpHitstopZoom, wallJumpHitstopTimecsale, wallJumpHitstopDuration);
		while (Time.time < finishTime) {
			if (Input.GetButtonDown("Jump")) {
				rb.AddForce (new Vector2 ( trueHorizontal, wallJumpVerticalForce));
				wallJumpDisqualified = true;
			}
			yield return null;
		}
	}

	IEnumerator RestoreCollision(Collider2D other) {
		yield return new WaitForSeconds (dropThruTimer);
		//Debug.Log ("Collision Restored");
		Physics2D.IgnoreCollision (coll, other, false);
	}

	private void TriggerFall() {
		//CurrentState = State.Jump;
		falling = true;
		Invoke("JumpState", 0.05f);
		jumpMode = 1;
		//Debug.Log ("Fall triggered");
		rb.velocity = new Vector2(rb.velocity.x, Mathf.Min(rb.velocity.y, 0.00001f));
		anim.SetInteger ("jumpMode", 1);
		anim.SetTrigger ("fall");
	}

	private void JumpState() {
		CurrentState = State.Jump;
		anim.SetTrigger ("fall");
	}

	public void ChangeStates(State newState) {
		string currentStateName = currentState.ToString ();
		string newStateName = newState.ToString ();
		if (currentState != State.Attack || newState != State.Attack) { 
			Invoke (currentStateName + "Exit", 0f);
		}
		currentState = newState;
		Invoke (newStateName + "Enter", 0f);
		anim.SetBool (currentStateName, false);
		anim.SetBool (newStateName, true);
		playerMovement.currentState = currentState;
		playerCombat.currentState = currentState;
		//ResetPosition ();
	}

	private void CharacterSwap() {
		if (CanSwitch()) {
			isSword = !isSword;
			playerCombat.isSword = isSword;
			weapon.IsSword = isSword;
			anim.SetBool ("isSword", isSword);
			anim.SetFloat ("isSwordFloat", isSword ? 1f : 0f);
			Vector3 charPos = characterSprite.transform.position;
			if (armed) {

			} else {
				characterSprite.transform.position = weaponSprite.transform.position;
				weaponSprite.transform.position = charPos;
			}
			if (isSword) {
			
			} else {

			}
		}
	}

	private void ThrowWeapon() {
		if (CanThrow ()) {
			if (armed) {
				ChangeStates (State.Throw);
			} else {
				weapon.Recall ();
			}
		}
	}

	public void ResetPosition() {
		Vector3 offset = transform.localPosition;
		transform.localPosition = Vector3.zero;
		transform.parent.position += offset;
	}

	public void AddForce (float addedForce) {
		addedForce = playerMovement.facingRight ? addedForce : -addedForce;
		rb.AddForce (new Vector2 (addedForce, 0f));
	}

	#region bool checks

	public bool CanSwitch() {
		if (currentState == State.Attack || currentState == State.Stunned || currentState == State.Death || currentState == State.Throw || frozen) {
			return false;
		}
		return true;
	}

	public bool CanMove() {
		if ((currentState == State.Attack && attackAnimEnded == false) || currentState == State.Throw) {
			return false;
		}
		if (currentState != State.Death && currentState != State.Stunned && !frozen && !(currentState == State.Jump && jumpMode == -1)) { // Note: The last one is to check that we're not in the middle of launching
			return true;
		}
		return false;
	}

	public bool CanJump() {
//		if (currentState == State.Attack && attackAnimEnded == false) {
//			return false;
//		}
		if (currentState != State.Death && currentState != State.Stunned && !frozen) {
			if (currentState != State.Jump || jumpMode == 2)
				return true;
		}
		return false; 
	}

	public bool CanThrow() {
		if (currentState != State.Death && currentState != State.Throw && currentState != State.Stunned && CurrentState != State.Jump && CurrentState != State.Attack && !frozen) {
			return true;
		}
		return false;
	}

	public bool CanAttack() {
		if (armed && currentState != State.Death && currentState != State.Throw && currentState != State.Stunned && !frozen) {
			return true;
		}
		return false;
	}

	public Collider2D CanDropThrough() {
		Collider2D colliderCheck = Physics2D.OverlapCapsule (coll.bounds.center + Vector3.down * jumpCastOffset, new Vector2 (coll.size.x * 0.9f, coll.size.y * 0.9f), CapsuleDirection2D.Vertical, 0f, jumpMask);
		if (!falling && !frozen && colliderCheck && colliderCheck.CompareTag ("DropThrough")) {
			if (currentState == State.Idle || currentState == State.Walk) {
				return colliderCheck;
			}
		}
		return null;
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

	private bool CheckSideCollision() {
		Collider2D colliderCheck = Physics2D.OverlapCapsule (coll.bounds.center, new Vector2 (coll.size.x * wallJumpDetectionSizeMod, coll.size.y * 0.9f), CapsuleDirection2D.Vertical, 0f, jumpMask);
		if (colliderCheck) {
			return true;
		} else {
			return false;
		}
	}

	private bool CheckFalling() {
		//Debug.Log (CheckGrounded () + ", " + rb.velocity.y);
		if (!falling && rb.velocity.y < 0f && !CheckGrounded()) {
			//Debug.Log ("Falling");
			return true;
		}
		//Debug.Log ("Not Falling");
		return false;
	}

	#endregion

	#region methods for other classes


	public void Launch() {
		//Debug.Log ("Launch");
		rb.velocity = storedVelocity;
		storedVelocity = Vector2.zero;
		rb.AddForce (new Vector2 (0f, isSword ? swordJumpforce : axeJumpForce));
		jumpMode = 0;
	}
		
	public void Attack(int attackMode) {
		anim.SetInteger ("attackMode", attackMode);
		playerCombat.ResetComboWindow ();
		ChangeStates (State.Attack);
		if (playerMovement.facingRight) {
			weaponCollOffset.x = Mathf.Abs (weaponCollOffset.x);
			swordFinisherOffset.x = Mathf.Abs (swordFinisherOffset.x);
			axeFinisherOffset.x = Mathf.Abs (axeFinisherOffset.x);
		} else {
			weaponCollOffset.x = -Mathf.Abs (weaponCollOffset.x);
			swordFinisherOffset.x = -Mathf.Abs (swordFinisherOffset.x);
			axeFinisherOffset.x = -Mathf.Abs (axeFinisherOffset.x);
		}
		//Debug.Log ((playerMovement.facingRight ? "Facing right. Offset: " : "Facing left. Offset: ") + weaponColl.offset + ", " + -Mathf.Abs (weaponColl.offset.x));
		if (attackMode == 3) {
			if (isSword) {
				weaponColl.size = swordFinisherSize;
				weaponColl.offset = swordFinisherOffset;
			} else {
				weaponColl.size = axeFinisherSize;
				weaponColl.offset = axeFinisherOffset;
				//Debug.Log ("current: " + weaponColl.offset + ", intended: " + axeFinisherOffset);
			}
		} else {
			weaponColl.size = weaponCollSize;
			weaponColl.offset = weaponCollOffset;
		}
		attackAnimEnded = false;
	}

	public void SetVelocity(Vector2 newVelocity) {
		rb.velocity = newVelocity;
		Debug.Log (newVelocity);
	}

	public void SetGravityEnabled(bool newGrav) {
		rb.gravityScale = newGrav ? initialGravity : 0f;
	}

	#endregion

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
		if (rb.velocity.magnitude > 1f) {
			rb.velocity *= 0.4f;
		}
	}

	void AttackExit() {
		weapon.currentDamage = 0;
		weaponColl.offset = weaponCollOffset;
	}

	void AttackStay() {
		//Debug.Log (weaponColl.offset);
	}

	void ThrowEnter() {

	}

	void ThrowStay() {

	}

	void ThrowExit() {

	}

	void JumpEnter() {
		wallJumpDisqualified = false;
		nextPossibleWalljump = Time.time + 0.1f;
	}

	void JumpExit() {
		jumpMode = -1;
		falling = false;
		anim.SetInteger ("jumpMode", 0);
	}

	void JumpStay() {
		if (jumpMode == 0 && rb.velocity.y <= 0f) {
			jumpMode = 1;
			anim.SetInteger ("jumpMode", 1);
		} else if (jumpMode == 1 && Mathf.Abs (rb.velocity.y) < Mathf.Epsilon && !frozen) {
			//Debug.Log ("Reset jump. velocity: " + rb.velocity.y + ", epsilon: " + Mathf.Epsilon);
			jumpMode = 2;
			anim.SetInteger ("jumpMode", 2);
			rb.velocity = Vector2.zero;
		}
		if (!wallJumpDisqualified && jumpMode == 0 && CheckSideCollision ()) {
			if (Time.time >= nextPossibleWalljump) {
				if (Mathf.Abs (rb.velocity.x) >= minimumWallJumpVelocity) {
					Debug.Log ("HITSTOP FRAME: " + rb.velocity);
					wallJumpDisqualified = true;
					StartCoroutine (WallStick ());
				} else {
					Debug.Log ("Collision, velocity low: " + rb.velocity);
				}
			}
			nextPossibleWalljump = Time.time + 0.1f;
		} else {
			//Debug.Log ("No collision: " + rb.velocity);
		}
		//Debug.Log (jumpMode + ", " + rb.velocity.y + ", " + anim.GetInteger("jumpMode") + ", " + anim.GetCurrentAnimatorClipInfo(0)[0].clip.name);
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
        FMODUnity.RuntimeManager.PlayOneShotAttached(FMODPaths.TAKE_DAMAGE, this.gameObject);
        hitSnap = FMODUnity.RuntimeManager.CreateInstance(FMODPaths.GOT_HIT);
        hitSnap.start();
	}

	void StunnedExit() {
		playerMovement.enabled = true;
        hitSnap.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
	}

	void StunnedStay() {

	}



	#endregion
}

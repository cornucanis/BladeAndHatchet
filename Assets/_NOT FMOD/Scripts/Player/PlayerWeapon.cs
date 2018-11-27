using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour {

	[SerializeField] int throwDamage = 2;
	[SerializeField] float throwForce = 1000f;
	[SerializeField] float throwRange = 12f;
	[SerializeField] float returnSpeed = 8f;
	[SerializeField] float returnCollectionRadius = 0.5f;
	[SerializeField] float superGravityScale = 10f;
	[SerializeField] float fallTransitionDuration = 1f;
	[SerializeField] float fallTransitionStep = 0.1f;
	[SerializeField] LayerMask groundMask;

	[Header("Temp hitstop test")]
	[SerializeField] float zoomMulti = 0.75f;
	[SerializeField] float timeScale = 0.7f;
	[SerializeField] float duration = 2f;


	Animator anim;
	Rigidbody2D rb;
	CapsuleCollider2D coll;

	PlayerStatus playerStatus;
	Transform playerTransform;

    private FMOD.Studio.EventInstance throwSFX;
    private FMOD.Studio.EventInstance throwImpactSFX;
    private FMOD.Studio.EventInstance throwRecallSFX;

    bool isSword;
	[HideInInspector] public int currentDamage = 0;
	State currentState;
	float groundedTimer;
	bool gravityEnabled = false;
	Vector3 throwingStartPoint;


	public enum State {Wielded, Throwing, Grounded, Returning}

	public State CurrentState {
		get {
			return currentState;
		}

		set {
			ChangeStates (value);
		}	
	}

	public bool IsSword {
		get {
			return isSword;
		}

		set {
			isSword = value;
			anim.SetFloat ("isSword", value ? 1f : 0f);
		}
	}

	void Awake() {
		anim = GetComponent<Animator> ();
		rb = GetComponent<Rigidbody2D> ();
		playerStatus = transform.parent.GetComponentInChildren<PlayerStatus> ();
		playerTransform = playerStatus.transform;
		CurrentState = State.Wielded;
		coll = GetComponent<CapsuleCollider2D> ();
	}

	void Start() {
		groundMask = LayerMask.GetMask ("Foreground");
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

	void OnTriggerEnter2D(Collider2D other) {
		if (currentState == State.Throwing) { 
			Debug.Log (other.name);
			if (gravityEnabled == false) {
				EnableSuperGravity ();
				/* OnTriggerEnter is most likely to occur when the axe hits an enemy, but could accidentally happen in other cases which is why I added this but commented it out
				throwSFXStop(); 
				throwRecallSFXStop();     
				throwImpactSFXPlay(); */	
			}
		}
		EnemyHealth enemy = other.GetComponent<EnemyHealth> ();
		if (enemy) {
			enemy.TakeDamage (currentDamage);
		}
	}

	void OnCollisionEnter2D(Collision2D other) {
		if (currentState == State.Throwing) { 
			if (gravityEnabled == false) {
				EnableSuperGravity ();
				throwSFXStop();
				throwRecallSFXStop();
				throwImpactSFXPlay();	
			}
		}
		EnemyHealth enemy = other.gameObject.GetComponent<EnemyHealth> ();
		//Debug.Log ("Collision");
		if (enemy) {
			//Debug.Log ("enemy confirmed");
			enemy.TakeDamage (currentDamage);
		}
	}


	void TrackPlayer() {
		Vector3 distance = playerTransform.position - transform.position;
		rb.velocity = distance.normalized * returnSpeed;
		//Debug.Log ("Tracking");
	}

	void EnableSuperGravity() {
		if (gravityEnabled == false) {
			//HitStopper.Instance.StartHitStop (zoomMulti, timeScale, duration);
			gravityEnabled = true;
			rb.gravityScale = superGravityScale;
			anim.speed = 0f;
			anim.SetBool ("Falling", true);
			StartCoroutine (LerpToDown ());
		}
	}

	IEnumerator LerpToDown() {
		float startingRot = CalculateStartingFallRotation ();
		if (startingRot != 270f) {
			float currentRot = startingRot;
			float thisDuration = (startingRot == 450f) ? fallTransitionDuration : fallTransitionDuration / 2;
			float startTime = Time.time;
			WaitForSeconds delay = new WaitForSeconds (fallTransitionStep);
			while (currentRot != 270f && CurrentState == State.Throwing) {
				currentRot = Mathf.Lerp (startingRot, 270f, (Time.time - startTime) / thisDuration);
				//Debug.Log (currentRot);
				transform.rotation = Quaternion.AngleAxis (currentRot, Vector3.forward);
				yield return delay;
			}
		}
		//Debug.Log ("Ended");
		yield return null;
	}

	float CalculateStartingFallRotation() {
		float currentAnimTime = anim.GetCurrentAnimatorStateInfo (0).normalizedTime % 1;
		int currentFrame = Mathf.Clamp(Mathf.CeilToInt(currentAnimTime * 4 - 1), 0, 3);
		switch (currentFrame) {
			case 0:
				return 450f;
			case 1:
				return 360f;
			case 2:
				return 270f;
			case 3:
				return 180f;
			default:
				Debug.LogError ("No current frame to calculate weapon starting rotation. WHAT??");
				break;
		}
		return 0f;
	}

	bool CheckGrounded() {
		Collider2D colliderCheck = Physics2D.OverlapCapsule (coll.bounds.center + Vector3.down * 0.1f, new Vector2 (coll.size.x * 0.9f, coll.size.y * 0.9f), CapsuleDirection2D.Vertical, 0f, groundMask);
		if (colliderCheck) {
			return true;
		} else {
			return false;
		}
	}

	public void Throw(bool facingRight) {
		CurrentState = State.Throwing;
		float trueForce = facingRight ? throwForce : -throwForce;
		rb.AddForce (new Vector2 (trueForce, 0f), ForceMode2D.Impulse);
		//Debug.Log (rb.velocity);
	}

	public void Recall() {
		ChangeStates (State.Returning);
	}

	#region State Methods

	void WieldedEnter() {
		coll.enabled = false;
		rb.velocity = Vector2.zero;
	}

	void WieldedExit() {

	}

	void WieldedStay() {

	}

	void ThrowingEnter() {
		coll.enabled = true;
		coll.isTrigger = false;
		rb.gravityScale = 0f;
		gravityEnabled = false;
		throwingStartPoint = transform.position;
		currentDamage = throwDamage;
		anim.SetBool ("Falling", false);
        throwSFXPlay();
    }

	void ThrowingExit() {
		coll.enabled = false;
		groundedTimer = -1f;
		rb.gravityScale = 0f;
		currentDamage = 0;
		anim.SetBool ("Falling", false);
		anim.speed = 1f;
		if (currentState != State.Grounded) {
			transform.rotation = Quaternion.AngleAxis (0f, Vector3.forward);
		}
	}

	void ThrowingStay() {
		CalculateStartingFallRotation ();
		if (Vector3.Distance (transform.position, throwingStartPoint) >= throwRange) {
			if (gravityEnabled == false) {
				EnableSuperGravity ();
				throwSFXStop();
				throwRecallSFXStop();
				throwImpactSFXPlay();	
			}
		}
		if (rb.velocity.magnitude < Mathf.Epsilon && CheckGrounded()) {
			CurrentState = State.Grounded;
		}
	}

	void GroundedEnter() {
		transform.rotation = Quaternion.AngleAxis (270f, Vector3.forward);
		coll.enabled = false;
	}

	void GroundedExit() {
		transform.rotation = Quaternion.AngleAxis (0f, Vector3.forward);
	}

	void GroundedStay() {

	}

	void ReturningEnter() {
		coll.enabled = true;
		coll.isTrigger = true;
		TrackPlayer ();
		currentDamage = throwDamage;
		if (gravityEnabled) {
			throwSFXStop ();
			throwRecallSFXPlay ();
		}
    }

	void ReturningExit() {
		coll.isTrigger = false;
		coll.enabled = false;
		currentDamage = 0;        
        throwRecallSFXStop();
        throwSFXStop();
    }

	void ReturningStay() {
		if (Vector2.Distance (playerTransform.position, transform.position) <= returnCollectionRadius) {
			playerStatus.Armed = true;
			CurrentState = State.Wielded;
		} else {
			TrackPlayer ();
		}
	}

    #endregion

    #region SFX

    void throwSFXPlay()
    {
        throwSFX = FMODUnity.RuntimeManager.CreateInstance(FMODPaths.THROW);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(throwSFX, GetComponent<Transform>(), GetComponent<Rigidbody>());
        throwSFX.start();
    }

    void throwSFXStop()
    {
        throwSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    void throwImpactSFXPlay()
    {
        throwImpactSFX = FMODUnity.RuntimeManager.CreateInstance(FMODPaths.THROW_IMPACT);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(throwImpactSFX, GetComponent<Transform>(), GetComponent<Rigidbody>());
        throwImpactSFX.start();
        throwImpactSFX.release();
    }

    void throwRecallSFXPlay()
    {
        throwRecallSFX = FMODUnity.RuntimeManager.CreateInstance(FMODPaths.THROW_RECALL);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(throwRecallSFX, GetComponent<Transform>(), GetComponent<Rigidbody>());
        throwRecallSFX.start();
    }

    void throwRecallSFXStop()
    {
        throwRecallSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        throwRecallSFX.release();
    }



    #endregion
}

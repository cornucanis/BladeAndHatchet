using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour {

	[SerializeField] int throwDamage = 2;
	[SerializeField] float throwForce = 1000f;
	[SerializeField] float returnSpeed = 8f;
	[SerializeField] float returnCollectionRadius = 0.5f;
	[SerializeField] float gravityIncreaseStep = 0.5f;

	Animator anim;
	Rigidbody2D rb;
	CapsuleCollider2D coll;

	PlayerStatus playerStatus;
	Transform playerTransform;

    private FMOD.Studio.EventInstance throwSFX;
    private FMOD.Studio.EventInstance throwImpactSFX;
    private FMOD.Studio.EventInstance throwRecallSFX;
    bool impactHappened = false;

    [HideInInspector] public bool isSword;
	[HideInInspector] public int currentDamage = 0;
	State currentState;
	float groundedTimer;

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
		EnemyHealth enemy = other.GetComponent<EnemyHealth> ();
		if (enemy) {
			enemy.TakeDamage (currentDamage);
		}
	}

	void OnCollisionEnter2D(Collision2D other) {
        throwSFXStop();
        throwRecallSFXStop();
        throwImpactSFXPlay();
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
		currentDamage = throwDamage;
        throwSFXPlay();
        impactHappened = false;
    }

	void ThrowingExit() {
		coll.enabled = false;
		groundedTimer = -1f;
		rb.gravityScale = 0f;
		currentDamage = 0;
    }

	void ThrowingStay() {
		if (rb.velocity.magnitude < Mathf.Epsilon) {
			if (groundedTimer == -1f) {
				groundedTimer = Time.time + 0.1f;
			} else {
				if (rb.velocity.magnitude > Mathf.Epsilon) {
					groundedTimer = -1f;
				} else if (Time.time >= groundedTimer) {
					CurrentState = State.Grounded;

				}
			}
		} else {
			rb.gravityScale += gravityIncreaseStep * Time.deltaTime;
		}
	}

	void GroundedEnter() {
		coll.enabled = false;
	}

	void GroundedExit() {

	}

	void GroundedStay() {

	}

	void ReturningEnter() {
		coll.enabled = true;
		coll.isTrigger = true;
		TrackPlayer ();
		currentDamage = throwDamage;
        if (impactHappened) { 
        throwSFXStop();
        throwRecallSFXPlay();
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
        if (!impactHappened)
        { 
        impactHappened = true;
        throwImpactSFX = FMODUnity.RuntimeManager.CreateInstance(FMODPaths.THROW_IMPACT);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(throwImpactSFX, GetComponent<Transform>(), GetComponent<Rigidbody>());
        throwImpactSFX.start();
        throwImpactSFX.release();
        }
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

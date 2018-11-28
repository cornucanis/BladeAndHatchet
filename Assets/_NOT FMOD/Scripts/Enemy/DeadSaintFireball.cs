using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadSaintFireball : MonoBehaviour {


	[SerializeField] int fireballDamage = 1;
	[SerializeField] float fireballSpeed = 3f;
	[SerializeField] float rotationalSpeed = 180f;
	[SerializeField] float idleDelay = 2.5f;
	[SerializeField] float delayDecayFactor = 0.9f;
	[SerializeField] float verticalAimingOffset = 1f;
	[SerializeField] float lifetime = 4f;

	public Transform playerTransform;
	public Vector3 fireballHome;
	Rigidbody2D rb;
	State currentState;
	Animator anim;
	float fireTime;
	float deathTime;
	[HideInInspector] public int fireballNumber;
    private FMOD.Studio.EventInstance saintFireSFX;

    public enum State {Appearing, Idle, Homing}

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
		CurrentState = State.Appearing;
		rb.freezeRotation = true;
    }

	void Start() {
		deathTime = Time.time + lifetime;
	}

	void Update () {
		Invoke (currentState.ToString () + "Stay", 0f);
		if (Time.time >= deathTime) {
			Destroy (gameObject);
		}
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
		if (other.CompareTag ("Player")) {
			other.GetComponent<PlayerCombat> ().TakeDamage (fireballDamage, transform);
			Destroy (gameObject);
		}
	}


	#region State Methods

	void AppearingEnter() {
		rb.velocity = Vector2.up * 3.5f;
	}

	void AppearingExit() {
		rb.velocity = Vector2.zero;
	}

	void AppearingStay() {

	}

	void IdleEnter() {
		fireTime = Time.time + idleDelay * Mathf.Pow(delayDecayFactor, fireballNumber);
		transform.localScale = new Vector3 (1, -1, 1);
	}

	void IdleExit() {
		transform.localScale = new Vector3 (1, 1, 1);
	}

	void IdleStay() {
		if (Time.time >= fireTime) {
			CurrentState = State.Homing;
		}
		transform.RotateAround (fireballHome, Vector3.forward, rotationalSpeed * Time.deltaTime);
	}

	void HomingEnter() {
        Vector2 newVelocity = (playerTransform.position + Vector3.up * verticalAimingOffset) - transform.position;
		rb.velocity = newVelocity.normalized * fireballSpeed;
		rb.freezeRotation = false;
		float zRotation = Mathf.Atan2( newVelocity.y, newVelocity.x )*Mathf.Rad2Deg;
		transform.rotation = Quaternion.Euler(new Vector3 ( 0, 0, zRotation + 180));
    }

    void HomingExit() {

	}

	void HomingStay() {
        
    }

    #endregion

    #region SFX

    public void FireSFXAppear()  
    {
        saintFireSFX = FMODUnity.RuntimeManager.CreateInstance(FMODPaths.SAINT_FIRE);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(saintFireSFX, GetComponent<Transform>(), GetComponent<Rigidbody>());
        saintFireSFX.start();
    }

    public void FireSFXFire() 
    {
        saintFireSFX.setParameterValue(FMODPaths.FIRE_START, 1f); 
        saintFireSFX.release();
        
    }



    #endregion




}

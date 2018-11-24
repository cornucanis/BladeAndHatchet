using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaper : Enemy {

	[SerializeField] float leapCooldown = 2f;
	[SerializeField] float activationRange = 8f;
	[SerializeField] float leapingSpeed = 800f; // how fast he moves in the air
	[SerializeField] float leapDelay; // delay between player sighting and leap activation
	[SerializeField] float leapVerticalMod = 1.3f;
	[SerializeField] float leapHorizontalMod = 2f;
	[SerializeField] float jumpMinClamp = 100f;
	[SerializeField] float jumpMaxClamp = 1000f;

	State currentState;
	Animator anim;
	Rigidbody2D rb;

	Transform playerTransform;

	bool jumping;
	float nextLeap = 0f;
	Vector3 storedDistance;

    private FMOD.Studio.EventInstance leaperIdleSFX;
    private FMOD.Studio.EventInstance leaperPrelapSFX;
    private FMOD.Studio.EventInstance leaperLeapingSFX;

    public enum State {Idle, Preleap, Leaping}

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
		CurrentState = State.Idle;
		playerTransform = FindObjectOfType<PlayerStatus> ().transform;
		rb = GetComponent<Rigidbody2D> ();
		storedDistance = Vector3.zero;
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
		Debug.Log ("Entering " + newStateName); 
	}

	void Jump() {
		Vector2 jumpForce = new Vector2 (storedDistance.x * leapHorizontalMod, storedDistance.y * leapVerticalMod) * leapingSpeed;
		if (jumpForce.magnitude < jumpMinClamp) {
			//Debug.Log ("Min clamp triggered. Magnitude of " + jumpForce + " " + jumpForce.magnitude);
			jumpForce = jumpForce.normalized * jumpMinClamp;
		}
		if (jumpForce.magnitude > jumpMaxClamp) {
			//Debug.Log ("Max clamp triggered. Magnitude of " + jumpForce + " " + jumpForce.magnitude);
		}
		//Debug.Log ("Adding force: " + jumpForce);
		rb.AddForce (Vector2.ClampMagnitude(jumpForce, jumpMaxClamp));
		//Debug.Log ("Force addition completed: " + rb.velocity);
		jumping = true;
	}

	void FlipSprite() {
		int playerSideMod;
		if (playerTransform.position.x > transform.position.x) {
			playerSideMod = -1; // left of player
		} else {
			playerSideMod = 1; // right of player
		}
		transform.localScale = new Vector3 (Mathf.Abs (transform.localScale.x) * playerSideMod, transform.localScale.y, transform.localScale.z);
	}

	#region Animator Methods

	public void StoreDirection() {
		storedDistance = playerTransform.position - transform.position;
	}

	#endregion

	#region State Methods

	void IdleEnter() {
        LeaperIdleSFXPlay();

    }

	void IdleExit() {
        LeaperIdleSFXStop();
    }

	void IdleStay() {
		if (Time.time >= nextLeap) {
			FlipSprite ();
			float distance = Vector3.Distance (transform.position, playerTransform.position);
			//Debug.Log (distance);
			if (distance <= activationRange) {
				CurrentState = State.Preleap;
			}
		}
	}

	void PreleapEnter() {
        LeaperPrelapSFXPlay();
    }

	void PreleapExit() {

	}

	void PreleapStay() {

	}

	void LeapingEnter() {
		jumping = false;
        LeaperLeapingSFXPlay();
    }

	void LeapingExit() {
		nextLeap = Time.time + leapCooldown;
	}

	void LeapingStay() {
		Debug.Log (rb.velocity.magnitude);
		if (jumping && rb.velocity.magnitude < 1.5f) {
			//Debug.Log ("resetting");
			jumping = false;
			ChangeStates (State.Idle);
		}
	}

    private void OnDestroy()
    {
        LeaperIdleSFXStop();
        LeaperPrelapSFXStop();
        LeaperLeapingSFXStop();
    }

    #endregion

    #region SFX

    void LeaperIdleSFXPlay()
    {
        leaperIdleSFX = FMODUnity.RuntimeManager.CreateInstance(FMODPaths.LEAPER_IDLE);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(leaperIdleSFX, GetComponent<Transform>(), GetComponent<Rigidbody>());
        leaperIdleSFX.start();
    }

    void LeaperIdleSFXStop()
    {
        leaperIdleSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        leaperIdleSFX.release();
    }


    void LeaperPrelapSFXPlay()
    {
        leaperPrelapSFX = FMODUnity.RuntimeManager.CreateInstance(FMODPaths.LEAPER_PRELAP);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(leaperPrelapSFX, GetComponent<Transform>(), GetComponent<Rigidbody>());
        leaperPrelapSFX.start();
    }

    void LeaperPrelapSFXStop()
    {
        leaperPrelapSFX.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        leaperPrelapSFX.release();
    }

    void LeaperLeapingSFXPlay()
    {
        leaperLeapingSFX = FMODUnity.RuntimeManager.CreateInstance(FMODPaths.LEAPER_LEAPING);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(leaperLeapingSFX, GetComponent<Transform>(), GetComponent<Rigidbody>());
        leaperLeapingSFX.start();
    }

    void LeaperLeapingSFXStop()
    {
        leaperLeapingSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        leaperLeapingSFX.release();
    }



    #endregion

    

}

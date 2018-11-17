using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	[SerializeField] float runSpeed = 2f;
	[SerializeField] float airMoveDamp = 0.2f;
	[SerializeField] float airDampWindow = 0.1f;

	//cached references
	PlayerStatus playerStatus;
	Rigidbody2D rb;

	//state variables
	float horizontalInput;
	float verticalInput;
	public PlayerStatus.State currentState;
	bool facingRight = true;

	// Use this for initialization
	void Start () {
		playerStatus = GetComponent<PlayerStatus> ();
		rb = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		CheckInput ();
		HandleInput ();
	}

	void CheckInput() {
		horizontalInput = Input.GetAxis ("Horizontal");
		verticalInput = Input.GetAxis ("Vertical");
	}

	void HandleInput() {
		if (playerStatus.CanMove ()) {
			if (Mathf.Abs (horizontalInput) > Mathf.Epsilon) {
				Move ();
			} else {
				rb.velocity = new Vector2 (0f, rb.velocity.y);
				//Debug.Log ("Velocity reset for lack of input");
			}
		} else {

		}
	}

	void Move() {
		FlipSprite ();
		if (Mathf.Abs (rb.velocity.y) < Mathf.Epsilon) {
			if (currentState != PlayerStatus.State.Walk && currentState != PlayerStatus.State.Jump) {
				playerStatus.CurrentState = PlayerStatus.State.Walk;
			}
			rb.velocity = new Vector2 (horizontalInput * runSpeed, rb.velocity.y);
		} else {
			float newX = Mathf.Lerp (rb.velocity.x, horizontalInput * runSpeed, airMoveDamp);
			//Debug.Log ("OldV: " + rb.velocity + ", Newv: " + new Vector2 (Mathf.Clamp (newX, rb.velocity.x - airDampWindow, rb.velocity.x + airDampWindow), rb.velocity.y) + ", newx: " + newX + ", min: " + (rb.velocity.x - airDampWindow) + ", max: " + (rb.velocity.x + airDampWindow));
			rb.velocity = new Vector2 (Mathf.Clamp(newX, rb.velocity.x - airDampWindow, rb.velocity.x + airDampWindow), rb.velocity.y);

		}
	}

	void FlipSprite() {
		if (facingRight) {
			if (horizontalInput < 0f) {
				transform.localScale = new Vector3 (-1, 1, 1);
				facingRight = false;
			}
		} else {
			if (horizontalInput > 0f) {
				transform.localScale = new Vector3 (1, 1, 1);
				facingRight = true;
			}
		}
	}

	void StopMovement() {
		//Debug.Log ("Stop Movement method called");
		rb.velocity = Vector2.zero;
	}

	public void OnWalkStay() {

		if (Mathf.Abs (horizontalInput) < Mathf.Epsilon && !playerStatus.Frozen) {
			//Debug.Log ("Reset velocity by onwalkstay");
			rb.velocity = Vector2.zero;
			playerStatus.CurrentState = PlayerStatus.State.Idle;
		}
	}

	public void TranslateX (float amt) {
		transform.position += new Vector3 (amt, 0f, 0f);
	}

	public void TranslateY(float amt) {
		transform.position += new Vector3 (0f, amt, 0f);
	}
}

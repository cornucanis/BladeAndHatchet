using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	[SerializeField] float runSpeed = 2f;

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
		if (Input.GetButtonDown ("Jump")) {
			Debug.Log(playerStatus.Jump ());
		}
	}

	void HandleInput() {
		if (Mathf.Abs (horizontalInput) > Mathf.Epsilon && playerStatus.CanMove()) {
			Move ();
		}
	}

	void Move() {
		if (currentState != PlayerStatus.State.Walk) {
			playerStatus.CurrentState = PlayerStatus.State.Walk;
		}
		FlipSprite ();
		rb.velocity = new Vector2( horizontalInput * runSpeed, 0f);
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
		rb.velocity = Vector2.zero;
	}

	public void OnWalkStay() {

		if (Mathf.Abs (horizontalInput) < Mathf.Epsilon) {
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour {

	//config params
	[SerializeField] float comboWindowLength = 3.0f;
	[SerializeField] int maxHealth = 10;

	//cached references
	PlayerStatus playerStatus;

	//state variables
	int axeHealth;
	int swordHealth;
	int comboCount = 0;
	int highestQueuedCombo = 0;
	float comboWindowEnd;
	[HideInInspector] public bool isSword;
	[HideInInspector] public PlayerStatus.State currentState;


	// Use this for initialization
	void Start () {
		playerStatus = GetComponent<PlayerStatus> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Attack") && playerStatus.CanAttack ()) {
			if (currentState == PlayerStatus.State.Jump) {
				//jump attack goes here
			} else {
				if (currentState == PlayerStatus.State.Attack && playerStatus.attackAnimEnded == false) {
					
					Debug.Log ("Queue set to " + Mathf.Min (3, Mathf.Max(highestQueuedCombo + 1, comboCount + 1)) + " and the last hit was " + comboCount);
					highestQueuedCombo = Mathf.Min (3, Mathf.Max(highestQueuedCombo + 1, comboCount + 1));
					if (highestQueuedCombo == 1) {
						highestQueuedCombo = 0;
					}
				} else {
					if (comboCount > 0 && Time.time > comboWindowEnd) {
						Debug.Log ("Combo window expired");
						comboCount = 0;
						highestQueuedCombo = 0;
					}
					comboCount++;
					//Debug.Log ("Executing standard free attack at combo count " + comboCount);
					playerStatus.Attack (comboCount);
					if (comboCount == 3) {
						comboCount = 0;
						highestQueuedCombo = 0;
					}
				}
			}
		}
	}

	public void ResetComboWindow() {
		comboWindowEnd = Time.time + comboWindowLength;
	}

	public int CheckAttackQueue() {
		if (comboCount >= 3 || (highestQueuedCombo >= 3 && comboCount == 0)) {
			comboCount = 0;
			highestQueuedCombo = 0; 
			//Debug.Log ("Empty queue 1");
			return -1;
		} 
		if (highestQueuedCombo > comboCount) {
			comboCount++;
			//Debug.Log ("queue of " + comboCount + " and highest queue of " + highestQueuedCombo);
			return comboCount;
		}
		//Debug.Log ("Empty queue 2");
		return -1;
	}

	public void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag ("Hazard")) {

		} else if (other.CompareTag ("Enemy")) {
			EnemyHealth enemy = other.GetComponent<EnemyHealth> ();
			if (enemy) {
				TakeDamage (enemy.contactDamage);
			} else {
				Debug.LogWarning ("There is an object named " + other.name + " with an enemy tag but no enemy health component. This should not happen.");
			}
		}
	}

	public void TakeDamage(int damage) {
		if (isSword) {
			swordHealth -= damage;
		} else {
			axeHealth -= damage;
		}
		if (swordHealth <= 0 || axeHealth <= 0) {
			Die ();
		}
	}

	void Die() {
		Destroy (gameObject);
	}
}

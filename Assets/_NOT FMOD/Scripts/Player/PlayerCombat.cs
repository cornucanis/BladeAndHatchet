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
	int comboCount;
	float comboWindowEnd;
	[HideInInspector] public PlayerStatus.State currentState;


	// Use this for initialization
	void Start () {
		playerStatus = GetComponent<PlayerStatus> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown("Attack") && playerStatus.CanAttack()) {
			if (currentState == PlayerStatus.State.Jump) {
				//jump attack goes here
			} else {

			}
		}
	}
}

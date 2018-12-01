using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : Collectible {

	public int healAmt = 2;
	[SerializeField] float lifetime = 8f;

	float deathTime;
	bool isSword;
	Animator anim;
	PlayerStatus player;

	void Awake() {
		anim = GetComponentInParent<Animator> ();
		player = PlayerInstance.Instance.GetComponent<PlayerStatus> ();
	}

	void Start() {
		deathTime = Time.time + lifetime;
	} 

	public override void Collect() {
		PlayerInstance.Instance.GetComponent<PlayerCombat>().RestoreHealth (healAmt);
	}

	void Update() {
		if (Time.time >= deathTime) {
			Destroy (gameObject);
		} else {
			UpdateAnimator ();
		}
	}

	void UpdateAnimator() {
		if (isSword) {
			if (!player.IsSword) {
				isSword = false;
				anim.SetBool ("isSword", false);
			}
		} else {
			if (player.IsSword) {
				isSword = true;
				anim.SetBool ("isSword", true);
			}
		}
	}
}

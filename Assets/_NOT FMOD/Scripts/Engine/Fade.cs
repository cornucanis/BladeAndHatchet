using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fade : MonoBehaviour {

	Animator anim;
	PlayerCombat player;

	static Fade instance;

	public static Fade Instance {
		get {
			if (instance == null) {
				Debug.LogWarning ("Attempted to access Fade instance with no available instance.");//This shouldn't happen
			}
			return instance;
		}
	}

	void Awake() {
		if (instance != null && instance != this) {
			Destroy (gameObject);
		} else {
			instance = this;
			DontDestroyOnLoad (gameObject);
		}
		anim = GetComponent<Animator> ();
	} 	

	void Start() {
		player = PlayerInstance.Instance.GetComponent<PlayerCombat> ();
	}

	public void TriggerFade() {
		anim.SetTrigger ("Full");
	}

	public void MovePlayer() {
		player.ReturnToCheckpoint ();
	}
}

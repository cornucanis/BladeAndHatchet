using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInstance : MonoBehaviour {
	static PlayerInstance instance;

	public static PlayerInstance Instance {
		get {
			if (instance == null) {
				Debug.LogWarning ("Attempted to access GameSession instance with no available instance.");//This shouldn't happen
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
	} 	
}

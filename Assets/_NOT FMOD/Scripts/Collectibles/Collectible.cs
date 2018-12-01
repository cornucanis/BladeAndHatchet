using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour {
	
	void OnTriggerEnter2D (Collider2D other) {
		if (other.CompareTag("Player")) {
			Destroy (transform.parent.gameObject);
			Collect ();
		}
	}

	public virtual void Collect() {

	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour {


	public int currentDamage = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D(Collider2D other) {
		EnemyHealth enemy = other.GetComponent<EnemyHealth> ();
		if (enemy) {
			enemy.TakeDamage (currentDamage);
		}
	}
}

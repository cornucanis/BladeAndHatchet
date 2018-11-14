using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

	[SerializeField] int maxHealth = 3;
	public int contactDamage = 1;


	int currentHealth;




	// Use this for initialization
	void Start () {
		currentHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void TakeDamage(int damage) {
		currentHealth -= damage;
		if (currentHealth <= 0) {
			Die ();
		}
	}

	void Die() {
		Destroy (gameObject);
	}
}

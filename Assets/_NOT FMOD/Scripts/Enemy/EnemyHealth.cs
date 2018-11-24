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
        FMODUnity.RuntimeManager.PlayOneShot(FMODPaths.LEAPER_IMPACT, GetComponent<Transform>().position);
        if (currentHealth <= 0) {
			Die ();
		}
	}

	void Die() {
        FMODUnity.RuntimeManager.PlayOneShot(FMODPaths.LEAPER_DEAD, GetComponent<Transform>().position);
        Destroy (gameObject);

    }
}

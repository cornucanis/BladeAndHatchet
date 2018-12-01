using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearSaintSpear : MonoBehaviour {

	[SerializeField] GameObject indicatorPrefab;
	[SerializeField] int spearDamage = 2;
	[SerializeField] float flyingSpeed = 6f;
	[SerializeField] float trackTime = 0.5f;
	[SerializeField] float gravityScale = 2f;
	public float initialDelay = 1f;
	[SerializeField] float lifetime = 3f;

	[HideInInspector] public Vector3 target;

	Rigidbody2D rb;
	float dropTime;
	float deathTime;

	void Awake() {
		rb = GetComponent<Rigidbody2D> ();
	}

	void Start() {
		dropTime = Time.time + initialDelay;
		rb.gravityScale = 0f;
		deathTime = Time.time + lifetime;
		//StartCoroutine (TrackTarget ());
	}

	/*IEnumerator TrackTarget() {
		float startTime = Time.time;
		float finishTime = Time.time + trackTime;
		GameObject indicator = Instantiate (indicatorPrefab, transform.position, Quaternion.identity);
		while (Time.time < finishTime) {
			indicator.transform.position = Vector3.Lerp(transform.position, target, (Time.time - startTime) / trackTime);



		}
	}*/

	void Update() {
		if (Time.time >= dropTime) {
			rb.gravityScale = gravityScale;
		}
		if (Time.time >= deathTime) {
			Destroy (gameObject);
		}
	}

	public void Attack() {

	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag ("Player")) {
			other.GetComponent<PlayerCombat> ().TakeDamage (spearDamage, transform);
			Destroy (gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D collData) {
		Collider2D other = collData.collider;
		if (other.CompareTag ("Player")) {
			other.GetComponent<PlayerCombat> ().TakeDamage (spearDamage, transform);
			Destroy (gameObject);
		}
	}

}

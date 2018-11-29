using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearSaintSpear : MonoBehaviour {

	[SerializeField] GameObject indicatorPrefab;
	[SerializeField] int SpearDamage = 2;
	[SerializeField] float flyingSpeed = 6f;
	[SerializeField] float trackTime = 0.5f;

	[HideInInspector] public Vector3 target;

	Rigidbody2D rb;

	void Awake() {
		rb = GetComponent<Rigidbody2D> ();
	}

	void Start() {
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

	public void Attack() {

	}

}

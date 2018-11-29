using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

	[SerializeField] float iFrameDuration = 0.5f;
	[SerializeField] float blinkDuration = .04f;
	[SerializeField] int maxHealth = 3;
	[SerializeField] float knockbackHorizontalForce = 400f;
	[SerializeField] float knockbackVerticalForce = 150f;
	public int contactDamage = 1;


	SpriteRenderer spriteRenderer;

	int currentHealth;
	bool iFramesActive = false;
	Color defaultSpriteColor;
	Color transparentSpriteColor;


	void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}

	// Use this for initialization
	void Start () {
		currentHealth = maxHealth;
		defaultSpriteColor = Color.white;
		transparentSpriteColor = Color.white;
		transparentSpriteColor.a = 0;
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
		currentHealth = maxHealth;
		transform.position = Vector3.left * 100f;
		gameObject.SetActive (false);

    }

	private IEnumerator iFrames() {
		WaitForSeconds delay = new WaitForSeconds (blinkDuration);
		iFramesActive = true;
		bool visible = true;
		float finishTime = Time.time + iFrameDuration;
		while (Time.time < finishTime) {
			visible = !visible;
			if (visible) {
				spriteRenderer.color = defaultSpriteColor;
			} else {
				spriteRenderer.color = transparentSpriteColor;
			}
			yield return delay;
		}
		iFramesActive = false;
		spriteRenderer.color = defaultSpriteColor;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

	[SerializeField] float iFrameDuration = 0.5f;
	[SerializeField] float blinkDuration = .04f;
	[SerializeField] int maxHealth = 3;
	[SerializeField] float knockbackHorizontalForce = 400f;
	[SerializeField] float knockbackVerticalForce = 150f;
	[SerializeField] float healthPickupDropChance = 0.25f;
	[SerializeField] int healthPickupAmount = 2;
	[SerializeField] GameObject healthPickupPrefab;
	public int contactDamage = 1;


	SpriteRenderer spriteRenderer;
	Rigidbody2D rb;
	Animator anim;

	int currentHealth;
	bool iFramesActive = false;
	Color defaultSpriteColor;
	Color transparentSpriteColor;


	void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		rb = GetComponent<Rigidbody2D> ();
		anim = GetComponent<Animator> ();
	}

	// Use this for initialization
	void Start () {
		defaultSpriteColor = Color.white;
		transparentSpriteColor = Color.white;
		transparentSpriteColor.a = 0;
	}

	void OnEnable() {
		currentHealth = maxHealth;
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void TakeDamage(int damage) {
		currentHealth -= damage;
		Knockback (transform.position.x > PlayerInstance.Instance.transform.position.x);
        FMODUnity.RuntimeManager.PlayOneShot(FMODPaths.LEAPER_IMPACT, GetComponent<Transform>().position);
        if (currentHealth <= 0) {
			Die ();
		}
	}

	void Die() {
		FMODUnity.RuntimeManager.PlayOneShot (FMODPaths.LEAPER_DEAD, GetComponent<Transform> ().position);
		currentHealth = maxHealth;
		if (Random.value < healthPickupDropChance) {
			GameObject pickup = Instantiate (healthPickupPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
			pickup.GetComponentInChildren<HealthPickup> ().healAmt = healthPickupAmount;
		}
		if (rb) {
			rb.velocity = Vector2.zero;
		}
		anim.SetTrigger ("Die");
	}

	public void MoveSprite() {
		transform.position = Vector3.left * 100f;
	}

	public void FinishDeath() {
		gameObject.SetActive (false);
	}

	void Knockback(bool toRight) {
		if (rb) {
			rb.AddForce (new Vector2 (toRight ? knockbackHorizontalForce : -knockbackHorizontalForce, knockbackVerticalForce));
		}
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

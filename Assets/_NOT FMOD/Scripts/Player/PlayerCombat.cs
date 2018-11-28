using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour {

	//config params
	[SerializeField] Text axeHealthText;
	[SerializeField] Text swordHealthText;
	[SerializeField] float comboWindowLength = 3.0f;
	[SerializeField] int maxHealth = 10;
	[SerializeField] float iFrameDuration = 1.5f;
	[SerializeField] float blinkDuration = 1f / 14f;
	[SerializeField] float knockbackHorizontalForce = 600f;
	[SerializeField] float knockbackVerticalForce = 200f;
	[SerializeField] float deathReloadDelay = 3f;

	//cached references
	PlayerStatus playerStatus;
	SpriteRenderer spriteRenderer;
	Rigidbody2D rb;

	Color defaultSpriteColor;
	Color transparentSpriteColor;

	//state variables
	int axeHealth;
	int swordHealth;
	int comboCount = 0;
	int highestQueuedCombo = 0;
	float comboWindowEnd;
	bool iFramesActive = false;
	[HideInInspector] public bool isSword;
	[HideInInspector] public PlayerStatus.State currentState;

	void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		playerStatus = GetComponent<PlayerStatus> ();
		rb = GetComponent<Rigidbody2D> ();
	}

	// Use this for initialization
	void Start () {
		axeHealth = maxHealth;
		swordHealth = maxHealth;
		UpdateHealthText ();
		defaultSpriteColor = Color.white;
		transparentSpriteColor = Color.white;
		transparentSpriteColor.a = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Attack") && playerStatus.CanAttack ()) {
			if (currentState == PlayerStatus.State.Jump) {
				//jump attack goes here
			} else {
				if (currentState == PlayerStatus.State.Attack && playerStatus.attackAnimEnded == false) {
					
					//Debug.Log ("Queue set to " + Mathf.Min (3, Mathf.Max(highestQueuedCombo + 1, comboCount + 1)) + " and the last hit was " + comboCount);
					highestQueuedCombo = Mathf.Min (3, Mathf.Max(highestQueuedCombo + 1, comboCount + 1));
					if (highestQueuedCombo == 1) {
						highestQueuedCombo = 0;
					}
				} else {
					if (comboCount > 0 && Time.time > comboWindowEnd) {
						//Debug.Log ("Combo window expired");
						comboCount = 0;
						highestQueuedCombo = 0;
					}
					comboCount++;
					//Debug.Log ("Executing standard free attack at combo count " + comboCount);
					playerStatus.Attack (comboCount);
					if (comboCount == 3) {
						comboCount = 0;
						highestQueuedCombo = 0;
					}
				}
			}
		}
	}

	public void ResetComboWindow() {
		comboWindowEnd = Time.time + comboWindowLength;
	}

	public int CheckAttackQueue() {
		if (comboCount >= 3 || (highestQueuedCombo >= 3 && comboCount == 0)) {
			comboCount = 0;
			highestQueuedCombo = 0; 
			//Debug.Log ("Empty queue 1");
			return -1;
		} 
		if (highestQueuedCombo > comboCount) {
			comboCount++;
			//Debug.Log ("queue of " + comboCount + " and highest queue of " + highestQueuedCombo);
			return comboCount;
		}
		//Debug.Log ("Empty queue 2");
		return -1;
	}

	public void OnTriggerEnter2D(Collider2D other) {
		if (other.CompareTag ("Hazard")) {
			Hazard hazard = other.GetComponent<Hazard> ();
			if (hazard) {
				TakeDamage (hazard.contactDamage, other.transform);
			} else {
				Debug.LogWarning ("There is an object named " + other.name + " with a hazard tag but no hazard component. This should not happen.");
			}
		} else if (other.CompareTag ("Enemy")) {
			EnemyHealth enemy = other.GetComponent<EnemyHealth> ();
			if (enemy) {
				TakeDamage (enemy.contactDamage, other.transform);
			} else {
				Debug.LogWarning ("There is an object named " + other.name + " with an enemy tag but no enemy health component. This should not happen.");
			}
		}
	}

	public void OnCollisionEnter2D(Collision2D otherColl) {
		GameObject other = otherColl.gameObject;
		if (other.CompareTag ("Hazard")) {
			Hazard hazard = other.GetComponent<Hazard> ();
			if (hazard) {
				TakeDamage (hazard.contactDamage, other.transform);
			} else {
				Debug.LogWarning ("There is an object named " + other.name + " with a hazard tag but no hazard component. This should not happen.");
			}
		} else if (other.CompareTag ("Enemy")) {
			EnemyHealth enemy = other.GetComponent<EnemyHealth> ();
			if (enemy) {
				TakeDamage (enemy.contactDamage, other.transform);
			} else {
				Debug.LogWarning ("There is an object named " + other.name + " with an enemy tag but no enemy health component. This should not happen.");
			}
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

	public bool CheckIframes() {
		return iFramesActive;
	}

	public void TakeDamage(int damage, Transform damager) {
		if (!iFramesActive && currentState != PlayerStatus.State.Death) {
			if (isSword) {
				swordHealth -= damage;
			} else {
				axeHealth -= damage;
			}
			UpdateHealthText ();
			//Debug.Log ("Took " + damage + " damage. Remaining: " + axeHealth + swordHealth);
			if (swordHealth <= 0 || axeHealth <= 0) {
				//Debug.Log ("Dead. " + axeHealth + swordHealth);
				Die ();
			} else {
				playerStatus.CurrentState = PlayerStatus.State.Stunned;
				StartCoroutine (iFrames ());
				Knockback (transform.position.x > damager.position.x);
			}
		}
	}

	void Knockback(bool toRight) {
		rb.AddForce (new Vector2 (toRight ? knockbackHorizontalForce : -knockbackHorizontalForce, knockbackVerticalForce));
	}

	void Die() {
		playerStatus.CurrentState = PlayerStatus.State.Death;
		SceneHandler.Instance.Invoke ("ReloadScene", deathReloadDelay);
	}

	void UpdateHealthText() {
		axeHealthText.text = Mathf.Max(axeHealth, 0).ToString();
		swordHealthText.text = Mathf.Max(swordHealth, 0).ToString();
	} 
}

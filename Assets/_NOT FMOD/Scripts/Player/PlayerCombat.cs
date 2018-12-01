using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour {

	//config params
	[SerializeField] Image[] axeHealthDots = new Image[0];
	[SerializeField] Image[] swordHealthDots = new Image[0];
	[SerializeField] Text axeHealthText;
	[SerializeField] Text swordHealthText;
	[SerializeField] float comboWindowLength = 3.0f;
	[SerializeField] int maxHealth = 5;
	[SerializeField] float iFrameDuration = 1.5f;
	[SerializeField] float blinkDuration = .04f;
	[SerializeField] float knockbackHorizontalForce = 600f;
	[SerializeField] float knockbackVerticalForce = 200f;
	[SerializeField] float deathReloadDelay = 3f;

	[Header("Hitstop config")]
	[SerializeField] float hitZoomMulti = 0.9f;
	[SerializeField] float hitTimeScale = 0.55f;
	[SerializeField] float hitDuration = 0.3f;

	//cached references
	PlayerStatus playerStatus;
	SpriteRenderer spriteRenderer;
	Rigidbody2D rb;
	HitStopper stopper;

	Color defaultSpriteColor;
	Color transparentSpriteColor;
	Color fullDotColor = Color.white;
	Color emptyDotColor;

	//state variables
	int axeHealth;
	int swordHealth;
	int comboCount = 0;
	int highestQueuedCombo = 0;
	float comboWindowEnd;
	bool iFramesActive = false;
	[HideInInspector] public bool isSword = true;
	[HideInInspector] public PlayerStatus.State currentState;

	void Awake() {
		spriteRenderer = GetComponent<SpriteRenderer> ();
		playerStatus = GetComponent<PlayerStatus> ();
		rb = GetComponent<Rigidbody2D> ();
	}

	// Use this for initialization
	void Start () {
		stopper = HitStopper.Instance;
		axeHealth = maxHealth;
		swordHealth = maxHealth;
		emptyDotColor = new Color (0.7f, 0.3f, 0.3f, 0.75f);
		UpdateHealthText ();
		PopulateHealthDots ();
		defaultSpriteColor = Color.white;
		transparentSpriteColor = Color.white;
		transparentSpriteColor.a = 0;
		isSword = true;
		Checkpoint.InitializeCheckpoints (transform.position);
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
			PopulateHealthDots ();
			//Debug.Log ("Took " + damage + " damage. Remaining: " + axeHealth + swordHealth);
			if (swordHealth <= 0 || axeHealth <= 0) {
				//Debug.Log ("Dead. " + axeHealth + swordHealth);
				Die ();
				stopper.StartHitStop (hitZoomMulti * 0.9f, hitTimeScale * 0.9f, hitDuration * 1.2f);
			} else {
				playerStatus.CurrentState = PlayerStatus.State.Stunned;
				stopper.StartHitStop (hitZoomMulti, hitTimeScale, hitDuration);
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
	}

	public void ReturnToCheckpoint() {
		transform.position = Checkpoint.currentCheckpointLocation;
		RoomTransitionHandler.Instance.ENTERROOMINSTANT (Checkpoint.currentCheckpointRoom);
		swordHealth = maxHealth;
		axeHealth = maxHealth;
		UpdateHealthText ();
		PopulateHealthDots ();
		playerStatus.CurrentState = PlayerStatus.State.Idle;
	}

	void UpdateHealthText() {
		axeHealthText.text = Mathf.Max(axeHealth, 0).ToString();
		swordHealthText.text = Mathf.Max(swordHealth, 0).ToString();
	} 

	void PopulateHealthDots() {
		if (axeHealthDots.Length != 0) {
			for (int i = 0; i < axeHealthDots.Length; i++) {
				if (axeHealth > i) {
					axeHealthDots [i].color = fullDotColor;
				} else {
					axeHealthDots [i].color = emptyDotColor;
				}
			}
		}
		if (swordHealthDots.Length != 0) {
			for (int i = 0; i < swordHealthDots.Length; i++) {
				if (swordHealth > i) {
					swordHealthDots [i].color = fullDotColor;
				} else {
					swordHealthDots [i].color = emptyDotColor;
				}
			}
		}
	}

	public void UpgradeMaxHealth(int amount) {
		maxHealth += amount;
		axeHealthDots [maxHealth - 1].enabled = true;
		swordHealthDots [maxHealth - 1].enabled = true;
		FullHealthRestore ();
	}

	public void RestoreHealth (int amount) {
		//Debug.Log ("is sword: " + isSword);
		if (isSword) {
			//Debug.Log ("sword health");
			swordHealth = Mathf.Min (swordHealth + amount, maxHealth);
		} else {
			//Debug.Log ("axe health");
			axeHealth = Mathf.Min (axeHealth + amount, maxHealth);
		}
		UpdateHealthText ();
		PopulateHealthDots ();
	}

	public void FullHealthRestore() {
		swordHealth = maxHealth;
		axeHealth = maxHealth;
		PopulateHealthDots ();
	}
}

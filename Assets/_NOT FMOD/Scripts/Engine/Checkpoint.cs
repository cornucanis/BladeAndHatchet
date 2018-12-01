using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour {

	[SerializeField] Room myRoom;

	Animator anim;

	public static Vector3 currentCheckpointLocation;
	public static Room currentCheckpointRoom;

	void Awake() {
		anim = GetComponent<Animator> ();
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.CompareTag ("Player")) {
			currentCheckpointLocation = transform.position;
			currentCheckpointRoom = myRoom;
			anim.SetBool ("Stop", true);
            FMODUnity.RuntimeManager.PlayOneShot(FMODPaths.GONG, transform.position);
        }
	}

	public static void InitializeCheckpoints(Vector3 startingPos) {
		currentCheckpointLocation = startingPos;
		currentCheckpointRoom = RoomTransitionHandler.Instance.startingRoom;
	}
}

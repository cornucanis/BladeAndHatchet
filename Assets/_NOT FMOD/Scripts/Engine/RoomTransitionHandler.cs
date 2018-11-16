using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RoomTransitionHandler : MonoBehaviour {

	[SerializeField] CinemachineConfiner cameraConfiner;
	[SerializeField] PolygonCollider2D globalBounds;
	[SerializeField] Room startingRoom;
	[SerializeField] float horizontalTransitionalDistance = 3f;
	[SerializeField] float verticalTransitionalDistance = 5f;
	[SerializeField] float roomTransitionTime = 1f;
	[SerializeField] float cameraHorizontalMoveDistance = 28f;
	[SerializeField] float cameraVerticalMoveDistance = 15.75f;


	CinemachineVirtualCamera cam;
	PlayerStatus playerStatus;
	Room currentRoom;
	static RoomTransitionHandler instance;

	public Room CurrentRoom {
		get {
			return currentRoom;
		}
		private set {

		}
	}

	public static RoomTransitionHandler Instance {
		get {
			if (instance == null) {
				Debug.LogWarning ("Attempted to access GameSession instance with no available instance.");//This should not happen
			}
			return instance;
		}
	}

	void Awake() {
		if (instance != null && instance != this) {
			Destroy (gameObject);
		} else {
			instance = this;
			DontDestroyOnLoad (gameObject);
		}
	}

	void Start() {
		playerStatus = FindObjectOfType<PlayerStatus> ();
		cam = cameraConfiner.GetComponent<CinemachineVirtualCamera> ();
		currentRoom = startingRoom;
		UpdateCameraBounds (currentRoom.bounds);
	}

	void UpdateCameraBounds(Collider2D newBounds) {
		cameraConfiner.m_BoundingShape2D = newBounds;
		cameraConfiner.InvalidatePathCache ();
	}

	public void TriggerTransition(RoomDirectionDictionary potentialRooms) {
		for (int i = 0; i < potentialRooms.rooms.Length; i++) {
			if (potentialRooms.rooms [i] != currentRoom) {
				StartCoroutine (TransitionCutscene (roomTransitionTime, potentialRooms.rooms [i], potentialRooms.directions [i]));
				//currentRoom = potentialRooms.rooms [i];
				//UpdateCameraBounds ();
				break;
			}
		}
	}

	private IEnumerator TransitionCutscene(float transitionTime, Room newRoom, RoomTransitionTrigger.Direction movementDirection) {
		float startTime = Time.time;
		float finishTime = startTime + transitionTime;
		Vector3 directionalVector = DirectionToVector (movementDirection);
		Vector3 camInitialPos = Camera.main.transform.position;
		bool isVertical = (directionalVector.x == 0);
		float cameraMoveDistance = isVertical ? cameraVerticalMoveDistance : cameraHorizontalMoveDistance;
		float transitionalDistance = isVertical ? verticalTransitionalDistance : horizontalTransitionalDistance;
		Vector3 camFinalPos = camInitialPos + directionalVector * cameraMoveDistance;
		cam.m_Follow = null;
		UpdateCameraBounds (globalBounds);
		playerStatus.Frozen = true;
		playerStatus.SetGravityEnabled (false);
		//playerStatus.CurrentState = PlayerStatus.State.Walk;
		playerStatus.SetVelocity ((directionalVector * transitionalDistance) / roomTransitionTime);
		Debug.Log ("Intended v: " + (directionalVector * transitionalDistance) / roomTransitionTime + ", actual v: " + playerStatus.GetComponent<Rigidbody2D> ().velocity);
		while (Time.time < finishTime) {
			cam.transform.position = Vector3.Lerp (camInitialPos, camFinalPos, (Time.time - startTime) / transitionTime);
			yield return null;
		}
		Debug.Log ("FINAL Intended v: " + (directionalVector * transitionalDistance) / roomTransitionTime + ", FINAL actual v: " + playerStatus.GetComponent<Rigidbody2D> ().velocity);

		currentRoom = newRoom;
		UpdateCameraBounds (currentRoom.bounds);
		cam.m_Follow = playerStatus.transform;
		playerStatus.Frozen = false;
		playerStatus.SetGravityEnabled (true);
	}

	Vector2 DirectionToVector (RoomTransitionTrigger.Direction dir) {
		switch (dir) {
			case RoomTransitionTrigger.Direction.Left:
				return Vector2.left;
			case RoomTransitionTrigger.Direction.Right:
				return Vector2.right;
			case RoomTransitionTrigger.Direction.Up:
				return Vector2.up;
			case RoomTransitionTrigger.Direction.Down:
				return Vector2.down;
			default:
				Debug.LogWarning ("Direction to vector method called with no valid direction??");
				return Vector2.zero;
		}
	}
}

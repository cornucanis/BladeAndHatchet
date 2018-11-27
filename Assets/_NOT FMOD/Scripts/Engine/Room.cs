using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour {

	public RoomData roomData;
	public PolygonCollider2D bounds;
	[SerializeField] Transform enemyParent;
	[SerializeField] Transform hazardParent;

	List<ObjectData> enemyData = new List<ObjectData> ();
	List<ObjectData> hazardData = new List<ObjectData> ();

	void Awake() {
		bounds = GetComponent<PolygonCollider2D> ();
		roomData.roomBounds = bounds;
		InitializeData ();
		ClearRoom ();
	}

	void InitializeData() {
		foreach (Transform enemyChild in enemyParent) {
			enemyData.Add (new ObjectData(PrefabChecker.GetPrefab(enemyChild.gameObject), enemyChild.position, enemyChild.rotation, enemyChild.localScale));
		}
		foreach (Transform hazardChild in hazardParent) {
			hazardData.Add (new ObjectData(PrefabChecker.GetPrefab(hazardChild.gameObject), hazardChild.position, hazardChild.rotation, hazardChild.localScale));
		}
		/* 
		int ii = 0;
		foreach (KeyValuePair<GameObject, Vector3> enemyData in enemyLocations) {
			Debug.Log ("Enemy " + ii + ".. Name: " + enemyData.Key.name + ", position: " + enemyData.Value);
			ii++;
		}
		*/
	}

	public void PopulateRoom() {
		foreach (ObjectData currentData in enemyData) {
			Instantiate (currentData.prefab, currentData.position, currentData.rotation, enemyParent);
		}
		foreach (ObjectData currentData in hazardData) {
			Instantiate (currentData.prefab, currentData.position, currentData.rotation, hazardParent);
		}
	}

	public void ClearRoom() {
		foreach (Transform enemyChild in enemyParent) {
			Destroy (enemyChild.gameObject);
		}
		foreach (Transform hazardChild in hazardParent) {
			Destroy (hazardChild.gameObject);
		}
	}
}

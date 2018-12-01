using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ObjectData {
	public GameObject prefab;
	public Vector2 position;
	public Quaternion rotation;
	public Vector3 scale;

	public ObjectData(GameObject pre, Vector2 pos, Quaternion rot, Vector3 scl) {
		prefab = pre;
		position = pos;
		rotation = rot;
		scale = scl;
	}
}

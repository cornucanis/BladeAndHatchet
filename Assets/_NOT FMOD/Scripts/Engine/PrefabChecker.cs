using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PrefabChecker : MonoBehaviour {

	public static GameObject GetPrefab(Object source) {
		return PrefabUtility.GetCorrespondingObjectFromSource (source) as GameObject;
	}
}

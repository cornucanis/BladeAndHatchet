using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public GameObject myPrefab;

	void OnEnable() {
		Initialize ();
	}

	protected virtual void Initialize() {

	}
}

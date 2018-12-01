using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockablePickup : Collectible {

	[SerializeField] bool isThrow = false;

	public override void Collect() {
		if (isThrow) {
			PlayerInstance.Instance.GetComponent<PlayerStatus> ().unlockedThrow = true;
		} else {
			PlayerInstance.Instance.GetComponent<PlayerStatus> ().unlockedWallJump = true;
		}
	}
}


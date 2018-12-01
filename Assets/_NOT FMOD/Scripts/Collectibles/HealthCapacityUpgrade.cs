using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCapacityUpgrade : Collectible {

	[SerializeField] int increase = 1;

	public override void Collect() {
		FindObjectOfType<PlayerCombat> ().UpgradeMaxHealth (increase);
	}
}

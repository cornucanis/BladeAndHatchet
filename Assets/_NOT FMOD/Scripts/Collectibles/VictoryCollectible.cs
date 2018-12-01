using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryCollectible : Collectible {


	public override void Collect() {
		SceneHandler.Instance.LoadNextScene ();		
	}
}

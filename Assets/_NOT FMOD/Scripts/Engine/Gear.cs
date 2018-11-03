using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour {

	//config params
	[SerializeField] Gear[] childGears;
	public int teeth;


	public enum Direction {Clockwise, Counterclockwise}

	public void Rotate(float rotationAmount, Direction dir) {
		bool isClockwise = dir == Direction.Clockwise;
		int dirMulti = isClockwise ? 1 : -1;
		transform.Rotate (0, 0, rotationAmount * dirMulti);
		for (int i = 0; i < childGears.Length; i++) {
			float toothRatio = 1f * teeth / childGears [i].teeth;
			childGears [i].Rotate (rotationAmount * toothRatio, isClockwise ? Direction.Counterclockwise : Direction.Clockwise);
			if (childGears [i].name == "Big Gear") {
				Debug.Log ("Ratio: " + toothRatio + ", amount: " + rotationAmount + ", total: " + rotationAmount * toothRatio + ", teeth: " + teeth + " " + childGears [i].teeth);
			}
		}
	}
}

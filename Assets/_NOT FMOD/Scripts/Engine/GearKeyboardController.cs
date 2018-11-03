using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearKeyboardController : MonoBehaviour {

	[SerializeField] Gear driver;
	[SerializeField] float rotationSpeed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		float horizontalInput = Input.GetAxisRaw ("Horizontal");
		if (horizontalInput != 0f) {
			Gear.Direction dir = horizontalInput > 0 ? Gear.Direction.Clockwise : Gear.Direction.Counterclockwise;
			driver.Rotate (rotationSpeed, dir);
		}
	}

	public void ChangeSpeed (float newSpeed) {
		rotationSpeed = newSpeed;
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserMovement : MonoBehaviour {
	public bool isVerticalSliding;
	private bool slidingUp = true;
	private bool slidingDown;
	public bool isHorizontalSlidingX;
	private bool slidingLeftX = true;
	private bool slidingRightX;
	public bool isHorizontalSlidingZ;
	private bool slidingLeftZ = true;
	private bool slidingRightZ;
	private Vector3 startingTransform;
	public float movementAmount;
	public float laserSpeed;

	private void Start() {
		startingTransform.x = transform.position.x;
		startingTransform.y = transform.position.y;
		startingTransform.z = transform.position.z;
		Debug.Log("Y: " + startingTransform.y);
	}
	void Update() {
		if (isVerticalSliding) {
			//move the laser up and down
			if (slidingUp) {
				transform.position = Vector3.MoveTowards(transform.position, new Vector3(startingTransform.x, startingTransform.y + movementAmount, startingTransform.z), laserSpeed * Time.deltaTime);
			} else if (slidingDown) {
				transform.position = Vector3.MoveTowards(transform.position, new Vector3(startingTransform.x, startingTransform.y - movementAmount, startingTransform.z), laserSpeed * Time.deltaTime);
			}
			if (transform.position.y.Equals(startingTransform.y + movementAmount)) {
				slidingUp = false;
				slidingDown = true;
			} else if (transform.position.y.Equals(startingTransform.y - movementAmount)) {
				slidingUp = true;
				slidingDown = false;
			}
		} else if (isHorizontalSlidingX) {
			//move the laser left and right
			if (slidingLeftX) {
				transform.position = Vector3.MoveTowards(transform.position, new Vector3(startingTransform.x + movementAmount, startingTransform.y, startingTransform.z), laserSpeed * Time.deltaTime);
			} else if (slidingRightX) {
				transform.position = Vector3.MoveTowards(transform.position, new Vector3(startingTransform.x - movementAmount, startingTransform.y, startingTransform.z), laserSpeed * Time.deltaTime);
			}
			if (transform.position.x.Equals(startingTransform.x + movementAmount)) {
				slidingLeftX = false;
				slidingRightX = true;
			} else if (transform.position.x.Equals(startingTransform.x - movementAmount)) {
				slidingLeftX = true;
				slidingRightX = false;
			}
		} else if (isHorizontalSlidingZ) {
			//move the laser left and right
			if (slidingLeftZ) {
				transform.position = Vector3.MoveTowards(transform.position, new Vector3(startingTransform.x, startingTransform.y, startingTransform.z + movementAmount), laserSpeed * Time.deltaTime);
			} else if (slidingRightZ) {
				transform.position = Vector3.MoveTowards(transform.position, new Vector3(startingTransform.x, startingTransform.y, startingTransform.z - movementAmount), laserSpeed * Time.deltaTime);
			}
			if (transform.position.z.Equals(startingTransform.z + movementAmount)) {
				slidingLeftZ = false;
				slidingRightZ = true;
			} else if (transform.position.z.Equals(startingTransform.z - movementAmount)) {
				slidingLeftZ = true;
				slidingRightZ = false;
			}
		}
	}
}

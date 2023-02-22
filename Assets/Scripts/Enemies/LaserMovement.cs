using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LaserMovement : MonoBehaviour
{
	public bool isVerticalSliding;
	public bool isHorizontalSlidingX;
	public bool isHorizontalSlidingZ;
	public float movementAmount;
	public float laserSpeed;

	private int direction = 1;
	private Vector3 pos1;
	private Vector3 pos2;

	private void Start()
	{
		if (isVerticalSliding)
		{
			pos1 = new Vector3(transform.position.x, transform.position.y + movementAmount, transform.position.z);
			pos2 = new Vector3(transform.position.x, transform.position.y - movementAmount, transform.position.z);
		}
		else if (isHorizontalSlidingX)
		{
			pos1 = new Vector3(transform.position.x + movementAmount, transform.position.y, transform.position.z);
			pos2 = new Vector3(transform.position.x - movementAmount, transform.position.y, transform.position.z);
		}
		else if (isHorizontalSlidingZ)
		{
			pos1 = new Vector3(transform.position.x, transform.position.y, transform.position.z + movementAmount);
			pos2 = new Vector3(transform.position.x, transform.position.y, transform.position.z - movementAmount);
		}
		else
		{ Debug.LogWarning("No direction was selected for " + gameObject.name); }
	}
	void Update()
	{
		if (transform.position == pos1 || transform.position == pos2) direction *= -1;
		if (direction == 1) transform.position = Vector3.MoveTowards(transform.position, pos1, laserSpeed * Time.deltaTime);
		else transform.position = Vector3.MoveTowards(transform.position, pos2, laserSpeed * Time.deltaTime);
	}
	private void OnDrawGizmosSelected()
	{
		if (!isVerticalSliding && !isHorizontalSlidingX && !isHorizontalSlidingZ) return;
		Vector3 position1 = Vector3.zero;
		Vector3 position2 = Vector3.zero;
		if (isVerticalSliding)
		{
			position1 = new Vector3(transform.position.x, transform.position.y + movementAmount, transform.position.z);
			position2 = new Vector3(transform.position.x, transform.position.y - movementAmount, transform.position.z);
		}
		else if (isHorizontalSlidingX)
		{
			position1 = new Vector3(transform.position.x + movementAmount, transform.position.y, transform.position.z);
			position2 = new Vector3(transform.position.x - movementAmount, transform.position.y, transform.position.z);
		}
		else if (isHorizontalSlidingZ)
		{
			position1 = new Vector3(transform.position.x, transform.position.y, transform.position.z + movementAmount);
			position2 = new Vector3(transform.position.x, transform.position.y, transform.position.z - movementAmount);
		}
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(transform.position, 0.1f);
		Gizmos.color = Color.white;
		if (Application.isPlaying)
		{
			Gizmos.DrawSphere(pos1, 0.1f);
			Gizmos.DrawSphere(pos2, 0.1f);
		}
		else
		{
			Gizmos.DrawSphere(position1, 0.1f);
			Gizmos.DrawSphere(position2, 0.1f);
		}
	}
}

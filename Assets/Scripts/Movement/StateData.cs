using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This describes data useful to the current state.
/// </summary>
public class StateData : MonoBehaviour
{
	[SerializeField, Tooltip("How far from the center of the spider it should check for clingable surfaces. This should match or be less than the farthest a leg can reach."), Min(0.000001f)]
	public float attachmentDistance;
	[SerializeField, Tooltip("The distance which the spider attaches from when it is falling. Must be less than attachment distance or else spider will instantly attach again when it falls or jumps."), Min(0.000001f)]
	public float lesserAttachmentDistance;
	[SerializeField, Tooltip("What layers should be considered walkable.")]
	public LayerMask walkableLayers;
	[SerializeField, Tooltip("The main camera")]
	public new Transform camera;
	[SerializeField, Tooltip("The density of points on the icosphere, used for calculating raycast points around the player."), Min(2)]
	public int icosphereDensity = 2;

	// An icosphere is just a geometric shape. It's being used to generate a sphere of raycasts.
	private Mesh icosphere;
	private Mesh Icosphere
	{
		get
		{
			if (icosphere == null)
			{
				icosphere = IcosphereCreator.Create(icosphereDensity, 1);
			}
			return icosphere;
		}
	}

	private void Start()
	{
		if (lesserAttachmentDistance > attachmentDistance) Debug.LogError("Lesser Attachment Distance must be less than Attachment Distance in 'State Data'");
		if ((walkableLayers & LayerMask.GetMask("Player")) > 0) Debug.LogError("Player cannot be in the layermask for 'State Data' Walkable Layers.");
		if (camera == null) Debug.LogError("Camera is not assigned in 'State Data'");
	}

	private float lastCheckDistance;

	#region ClosestPoint
	private Vector3? closestPoint = null;
	/// <summary>
	/// Returns the closest (walkable) point to this transform.position. If there are no objects within radius then null is returned.
	/// </summary>
	/// <param name="radius">The spherical radius to check for nearby objects.</param>
	/// <returns></returns>
	public Vector3? GetClosestPoint(float checkDistance)
	{
		if (checkDistance <= 0)
		{
			Debug.LogError("You cannot check for walkable objects in a radius less than or equal to zero.");
			return null;
		}
		lastCheckDistance = checkDistance;
		// Collect the list of hits.
		List<RaycastHit> hits = new List<RaycastHit>();
		foreach (var v in Icosphere.vertices)
		{
			Physics.Raycast(transform.position, v, out RaycastHit hit, checkDistance, walkableLayers);
			if (hit.collider != null) hits.Add(hit);
		}

		// Calculate the closest point of those hits.
		closestPoint = null;
		float closestPointSqrDistance = float.MaxValue;
		foreach (RaycastHit hit in hits)
		{
			Vector3 point = hit.point;
			float distance = (transform.position - point).sqrMagnitude;
			if (distance < closestPointSqrDistance)
			{
				closestPointSqrDistance = distance;
				closestPoint = point;
			}
		}
		return closestPoint;
	}
	#endregion

	private void OnDrawGizmos()
	{
		// If the program isn't running I can't get the icosphere mesh in order to calculate the raycast points,
		// so here are some consolation spheres.
		if (Icosphere == null)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireSphere(transform.position, attachmentDistance);
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, lesserAttachmentDistance);
			return;
		}

		// If the program is running you get to see where the raycasts actually are.
		Gizmos.color = Color.yellow;
		foreach (Vector3 v in Icosphere.vertices)
		{
			Gizmos.DrawLine(transform.position + Vector3.zero, transform.position + v * lastCheckDistance);
		}

		// This draws a line to the closest point.
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, closestPoint != null ? (Vector3)closestPoint : transform.position);
	}
}

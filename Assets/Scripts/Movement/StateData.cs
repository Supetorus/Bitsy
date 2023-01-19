using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This describes data useful to the current state.
/// </summary>
public class StateData : MonoBehaviour
{
	[SerializeField, Tooltip("How far from the center of the spider it should check for clingable surfaces. This should match or be less than the farthest a leg can reach.")]
	public float attachmentDistance;
	[SerializeField, Tooltip("What layers should be considered walkable.")]
	public LayerMask walkableLayers;
	[SerializeField, Tooltip("The main camera")]
	public new Transform camera;
	[SerializeField, Tooltip("The density of points on the icosphere, ")]
	public int icosphereDensity = 2;

	private Mesh icosphere;

	private void Start()
	{
	}

	private void Update()
	{
		icosphere = IcosphereCreator.Create(icosphereDensity, attachmentDistance);
		closestPointCalculatedThisFrame = false;
	}

	#region ClosestPoint
	private Vector3? closestPoint = null;
	private bool closestPointCalculatedThisFrame = false;
	/// <summary>
	/// Returns the closest (walkable) point to this transform.position. If there is no objects within radius then null is returned.
	/// </summary>
	/// <param name="radius">The spherical radius to check for nearby objects.</param>
	/// <returns></returns>
	public Vector3? GetClosestPoint()
	{
		if (closestPointCalculatedThisFrame) return closestPoint;
		if (icosphere == null) return null;
		closestPoint = null;
		List<RaycastHit> hits = new List<RaycastHit>();
		foreach (var v in icosphere.vertices)
		{
			Physics.Raycast(transform.position, v, out RaycastHit hit, attachmentDistance, walkableLayers);
			hits.Add(hit);
		}

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
		closestPointCalculatedThisFrame = true;
		return closestPoint;
	}
	#endregion

	private void OnDrawGizmos()
	{
		if (icosphere == null) return;
		Gizmos.color = Color.yellow;
		foreach (Vector3 v in icosphere.vertices)
		{
			Gizmos.DrawLine(transform.position + Vector3.zero, transform.position + v);
			Gizmos.DrawSphere(transform.position + v, 0.005f);
		}
		// This draws all of the gizmo lines which show the orientation of this object.
		//Gizmos.color = Color.white;
		//Gizmos.DrawWireSphere(transform.position, attachmentDistance);
		//Gizmos.color = Color.yellow;
		//Gizmos.DrawLine(transform.position, closestPoint != null ? (Vector3)closestPoint : transform.position);
		//Gizmos.color = Color.green;
		//Gizmos.DrawLine(transform.position, transform.position + transform.up * 0.1f);
		//Gizmos.color = Color.red;
		//Gizmos.DrawLine(transform.position, transform.position + transform.right * 0.1f);
		//Gizmos.color = Color.blue;
		//Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.1f);
	}
}

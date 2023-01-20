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
	[SerializeField, Tooltip("The distance which the spider attaches from when it is falling. Must be less than attachment distance or else spider will instantly attach again when it falls or jumps.")]
	public float lesserAttachmentDistance;
	[SerializeField, Tooltip("What layers should be considered walkable.")]
	public LayerMask walkableLayers;
	[SerializeField, Tooltip("The main camera")]
	public new Transform camera;
	[SerializeField, Tooltip("The density of points on the icosphere, ")]
	public int icosphereDensity = 2;

	private Mesh icosphere;

	private void Start()
	{
		icosphere = IcosphereCreator.Create(icosphereDensity, 1);
	}

	private float lastCheckDistance;

	private void Update()
	{
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
	public Vector3? GetClosestPoint(float checkDistance)
	{
		//if (closestPointCalculatedThisFrame) return closestPoint;
		lastCheckDistance = checkDistance;
		if (icosphere == null) return null;
		closestPoint = null;
		List<RaycastHit> hits = new List<RaycastHit>();
		foreach (var v in icosphere.vertices)
		{
			Physics.Raycast(transform.position, v, out RaycastHit hit, checkDistance, walkableLayers);
			if (hit.collider != null) hits.Add(hit);
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
			Gizmos.DrawLine(transform.position + Vector3.zero, transform.position + v * lastCheckDistance);
			Gizmos.DrawSphere(transform.position + v * lastCheckDistance, 0.005f);
		}
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, closestPoint != null ? (Vector3)closestPoint : transform.position);
	}
}
